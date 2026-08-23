using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Skia.RuntimeEffects;

internal static class DorotiSkiaImageFilterRenderer
{
    private const int MaxPooledSurfacesPerFrame = 8;
    private const int MaxCachedImages = 32;
    private const long MaxCacheableImagePixels = 4L * 1024 * 1024;
    private const long MaxCachedImagePixels = 16L * 1024 * 1024;
    private static readonly object PoolGate = new();
    private static readonly Dictionary<(string Backend, long ContextGeneration), SurfacePool> SurfacePools = [];
    private static long _surfacesCreated;
    private static long _surfaceReuses;
    private static long _imageCacheHits;
    private static long _imageCacheMisses;

    internal static (long Created, long Reused, long Active, long CacheHits, long CacheMisses) Diagnostics
    {
        get
        {
            lock (PoolGate)
                return (Interlocked.Read(ref _surfacesCreated), Interlocked.Read(ref _surfaceReuses),
                    SurfacePools.Values.Sum(pool => (long)pool.ActiveCount),
                    Interlocked.Read(ref _imageCacheHits), Interlocked.Read(ref _imageCacheMisses));
        }
    }

    internal static void BeginFrame(string backend, long contextGeneration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(backend);
        lock (PoolGate)
            GetOrCreatePool(backend, contextGeneration).BeginFrame();
    }

    internal static void InvalidateContext(string backend, long currentContextGeneration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(backend);
        lock (PoolGate)
        {
            foreach (var key in SurfacePools.Keys.Where(key =>
                         string.Equals(key.Backend, backend, StringComparison.Ordinal) &&
                         key.ContextGeneration != currentContextGeneration).ToArray())
            {
                SurfacePools.Remove(key, out var pool);
                pool?.Dispose();
            }
        }
    }

    internal static void ReleaseContext(string backend, long contextGeneration)
    {
        lock (PoolGate)
        {
            if (!SurfacePools.Remove((backend, contextGeneration), out var pool)) return;
            pool.Dispose();
        }
    }

    internal static bool Draw(
        SKCanvas target,
        int pixelWidth,
        int pixelHeight,
        FragmentShaderSnapshot shader,
        SKRect childBounds,
        SKPoint childOffset,
        SKSamplingOptions inputSampling,
        Func<Image, SKShader> imageShaderFactory,
        Action<SKCanvas, int, int> drawChild,
        string backend,
        long contextGeneration,
        object? cacheKey,
        long cacheGeneration,
        out bool cacheHit)
    {
        cacheHit = false;
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(shader);
        ArgumentNullException.ThrowIfNull(imageShaderFactory);
        ArgumentNullException.ThrowIfNull(drawChild);
        if (pixelWidth <= 0 || pixelHeight <= 0)
            throw new ArgumentOutOfRangeException(nameof(pixelWidth), "The GPU filter target must have positive dimensions.");
        if (target.Context is not { } context)
            throw new NotSupportedException(
                "Doroti ImageFilter.shader requires the active Skia GPU recording context; software capture is forbidden.");

        var offsetBounds = new SKRect(
            childBounds.Left + childOffset.X,
            childBounds.Top + childOffset.Y,
            childBounds.Right + childOffset.X,
            childBounds.Bottom + childOffset.Y);
        var mappedBounds = target.TotalMatrix.MapRect(offsetBounds);
        var clip = target.DeviceClipBounds;
        var visibleLeft = Math.Max(0, Math.Max(clip.Left, (int)Math.Floor(mappedBounds.Left)));
        var visibleTop = Math.Max(0, Math.Max(clip.Top, (int)Math.Floor(mappedBounds.Top)));
        var visibleRight = Math.Min(pixelWidth, Math.Min(clip.Right, (int)Math.Ceiling(mappedBounds.Right)));
        var visibleBottom = Math.Min(pixelHeight, Math.Min(clip.Bottom, (int)Math.Ceiling(mappedBounds.Bottom)));
        if (visibleRight <= visibleLeft || visibleBottom <= visibleTop) return false;

        var matrix = target.TotalMatrix;
        var cacheWidth = checked((int)Math.Ceiling(mappedBounds.Width));
        var cacheHeight = checked((int)Math.Ceiling(mappedBounds.Height));
        var cachePixels = (long)cacheWidth * cacheHeight;
        var canCache = cacheKey is not null && cacheWidth > 0 && cacheHeight > 0 &&
                       cachePixels <= MaxCacheableImagePixels && IsFinite(mappedBounds);
        var signature = TransformSignature.From(matrix);
        if (canCache && TryDrawCached(
                target, backend, contextGeneration, cacheKey!, cacheWidth, cacheHeight,
                signature, cacheGeneration, mappedBounds.Left, mappedBounds.Top))
        {
            cacheHit = true;
            return true;
        }

        if (canCache)
        {
            var cacheLease = RentSurface(context, backend, contextGeneration, cacheWidth, cacheHeight);
            try
            {
                RenderChild(cacheLease.Surface, mappedBounds.Left, mappedBounds.Top,
                    matrix, childOffset, drawChild, cacheWidth, cacheHeight);
                using var inputImage = cacheLease.Surface.Snapshot(new SKRectI(0, 0, cacheWidth, cacheHeight))
                    ?? throw new InvalidOperationException("Doroti ImageFilter.shader could not snapshot its GPU input surface.");
                using var runtimeShader = DorotiSkiaRuntimeEffects.CreateImageFilterShader(
                    shader, inputImage, inputSampling, imageShaderFactory, backend, contextGeneration);
                using var outputSurface = CreateSurface(context, cacheWidth, cacheHeight);
                using (var paint = new SKPaint { Shader = runtimeShader, BlendMode = SKBlendMode.SrcOver })
                    outputSurface.Canvas.DrawRect(SKRect.Create(cacheWidth, cacheHeight), paint);
                outputSurface.Canvas.Flush();
                var outputImage = outputSurface.Snapshot()
                    ?? throw new InvalidOperationException("Doroti ImageFilter.shader could not snapshot its cached output.");
                StoreAndDrawCached(target, backend, contextGeneration, cacheKey!, cacheWidth,
                    cacheHeight, signature, cacheGeneration, mappedBounds.Left, mappedBounds.Top, outputImage);
                Interlocked.Increment(ref _imageCacheMisses);
                return true;
            }
            finally
            {
                if (cacheLease.IsTemporary) cacheLease.Surface.Dispose();
            }
        }

        var left = Math.Max(0, Math.Max(clip.Left, (int)Math.Floor(mappedBounds.Left)));
        var top = Math.Max(0, Math.Max(clip.Top, (int)Math.Floor(mappedBounds.Top)));
        var right = Math.Min(pixelWidth, Math.Min(clip.Right, (int)Math.Ceiling(mappedBounds.Right)));
        var bottom = Math.Min(pixelHeight, Math.Min(clip.Bottom, (int)Math.Ceiling(mappedBounds.Bottom)));
        if (right <= left || bottom <= top) return false;

        var width = checked(right - left);
        var height = checked(bottom - top);
        var lease = RentSurface(context, backend, contextGeneration, width, height);
        try
        {
            RenderChild(lease.Surface, left, top, matrix, childOffset, drawChild, width, height);

            using var inputImage = lease.Surface.Snapshot(new SKRectI(0, 0, width, height))
                ?? throw new InvalidOperationException("Doroti ImageFilter.shader could not snapshot its GPU input surface.");
            using var runtimeShader = DorotiSkiaRuntimeEffects.CreateImageFilterShader(
                shader,
                inputImage,
                inputSampling,
                imageShaderFactory,
                backend,
                contextGeneration);
            using var paint = new SKPaint { Shader = runtimeShader, BlendMode = SKBlendMode.SrcOver };
            target.Save();
            target.ResetMatrix();
            target.Translate(left, top);
            target.DrawRect(SKRect.Create(width, height), paint);
            target.Restore();
            return true;
        }
        finally
        {
            if (lease.IsTemporary) lease.Surface.Dispose();
        }
    }

    private static void RenderChild(
        SKSurface surface,
        float originX,
        float originY,
        SKMatrix parentMatrix,
        SKPoint childOffset,
        Action<SKCanvas, int, int> drawChild,
        int width,
        int height)
    {
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.Transparent);
        canvas.Save();
        canvas.Translate(-originX, -originY);
        canvas.Concat(in parentMatrix);
        canvas.Translate(childOffset.X, childOffset.Y);
        drawChild(canvas, width, height);
        canvas.Restore();
        canvas.Flush();
    }

    private static bool TryDrawCached(
        SKCanvas target,
        string backend,
        long contextGeneration,
        object cacheKey,
        int width,
        int height,
        TransformSignature signature,
        long generation,
        float left,
        float top)
    {
        lock (PoolGate)
        {
            var pool = GetOrCreatePool(backend, contextGeneration);
            if (!pool.Images.TryGetValue(cacheKey, out var cached) || cached.Width != width ||
                cached.Height != height || cached.Transform != signature ||
                cached.Generation != generation)
            {
                if (cached is not null) pool.RemoveImage(cacheKey, cached);
                return false;
            }
            cached.LastUsedFrame = pool.FrameNumber;
            DrawImage(target, cached.Image, left, top);
            Interlocked.Increment(ref _imageCacheHits);
            return true;
        }
    }

    private static void StoreAndDrawCached(
        SKCanvas target,
        string backend,
        long contextGeneration,
        object cacheKey,
        int width,
        int height,
        TransformSignature signature,
        long generation,
        float left,
        float top,
        SKImage image)
    {
        lock (PoolGate)
        {
            var pool = GetOrCreatePool(backend, contextGeneration);
            if (pool.Images.Remove(cacheKey, out var replaced)) pool.DisposeImage(replaced);
            var cached = new CachedImage(image, width, height, signature, generation, pool.FrameNumber);
            pool.Images.Add(cacheKey, cached);
            pool.CachedPixels += cached.Pixels;
            pool.TrimImageCache();
            DrawImage(target, image, left, top);
        }
    }

    private static void DrawImage(SKCanvas target, SKImage image, float left, float top)
    {
        target.Save();
        target.ResetMatrix();
        target.DrawImage(image, left, top, SKSamplingOptions.Default);
        target.Restore();
    }

    private static bool IsFinite(SKRect rect) =>
        float.IsFinite(rect.Left) && float.IsFinite(rect.Top) &&
        float.IsFinite(rect.Right) && float.IsFinite(rect.Bottom);

    private static SurfaceLease RentSurface(
        GRRecordingContext context,
        string backend,
        long contextGeneration,
        int width,
        int height)
    {
        lock (PoolGate)
        {
            var pool = GetOrCreatePool(backend, contextGeneration);
            var slot = pool.NextSlot++;
            if (slot >= MaxPooledSurfacesPerFrame)
                return new(CreateSurface(context, width, height), true);

            while (pool.Surfaces.Count <= slot) pool.Surfaces.Add(null);
            var surface = pool.Surfaces[slot];
            // GPU snapshots are the implicit texture passed to the runtime
            // effect. Keep that texture exact-sized: reusing a larger pooled
            // surface after a shrink asks the backend for a subset snapshot,
            // which is not reliable for the D3D12 render target path and can
            // return null during rapid small-window layout changes.
            if (surface is null || surface.Canvas.DeviceClipBounds.Width != width ||
                surface.Canvas.DeviceClipBounds.Height != height)
            {
                surface?.Dispose();
                surface = CreateSurface(context, width, height);
                pool.Surfaces[slot] = surface;
            }
            else
            {
                Interlocked.Increment(ref _surfaceReuses);
            }
            return new(surface, false);
        }
    }

    private static SKSurface CreateSurface(GRRecordingContext context, int width, int height)
    {
        var info = new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
        var surface = SKSurface.Create(context, true, info)
            ?? throw new InvalidOperationException(
                $"Doroti ImageFilter.shader could not allocate a {width}x{height} GPU input surface.");
        Interlocked.Increment(ref _surfacesCreated);
        return surface;
    }

    private static SurfacePool GetOrCreatePool(string backend, long contextGeneration)
    {
        var key = (backend, contextGeneration);
        if (!SurfacePools.TryGetValue(key, out var pool))
            SurfacePools.Add(key, pool = new SurfacePool());
        return pool;
    }

    private readonly record struct SurfaceLease(SKSurface Surface, bool IsTemporary);

    private readonly record struct TransformSignature(
        float ScaleX,
        float SkewX,
        float SkewY,
        float ScaleY,
        float Persp0,
        float Persp1,
        float Persp2)
    {
        internal static TransformSignature From(SKMatrix matrix) => new(
            matrix.ScaleX, matrix.SkewX, matrix.SkewY, matrix.ScaleY,
            matrix.Persp0, matrix.Persp1, matrix.Persp2);
    }

    private sealed class CachedImage(
        SKImage image,
        int width,
        int height,
        TransformSignature transform,
        long generation,
        long lastUsedFrame)
    {
        internal SKImage Image { get; } = image;
        internal int Width { get; } = width;
        internal int Height { get; } = height;
        internal TransformSignature Transform { get; } = transform;
        internal long Generation { get; } = generation;
        internal long LastUsedFrame { get; set; } = lastUsedFrame;
        internal long Pixels => (long)Width * Height;
    }

    private sealed class SurfacePool : IDisposable
    {
        internal List<SKSurface?> Surfaces { get; } = [];
        internal Dictionary<object, CachedImage> Images { get; } =
            new(ReferenceEqualityComparer.Instance);
        internal int NextSlot { get; set; }
        internal long FrameNumber { get; private set; }
        internal long CachedPixels { get; set; }
        internal int ActiveCount => Surfaces.Count(surface => surface is not null);

        internal void BeginFrame()
        {
            NextSlot = 0;
            FrameNumber++;
        }

        internal void RemoveImage(object key, CachedImage image)
        {
            Images.Remove(key);
            DisposeImage(image);
        }

        internal void DisposeImage(CachedImage image)
        {
            CachedPixels -= image.Pixels;
            image.Image.Dispose();
        }

        internal void TrimImageCache()
        {
            while (Images.Count > MaxCachedImages || CachedPixels > MaxCachedImagePixels)
            {
                var oldest = Images.MinBy(pair => pair.Value.LastUsedFrame);
                if (oldest.Key is null) break;
                RemoveImage(oldest.Key, oldest.Value);
            }
        }

        public void Dispose()
        {
            foreach (var surface in Surfaces) surface?.Dispose();
            Surfaces.Clear();
            foreach (var image in Images.Values) image.Image.Dispose();
            Images.Clear();
            CachedPixels = 0;
        }
    }
}
