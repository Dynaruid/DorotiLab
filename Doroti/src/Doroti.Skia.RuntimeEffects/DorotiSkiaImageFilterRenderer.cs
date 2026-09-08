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
    private static readonly Dictionary<(string Backend, long ContextGeneration, object? Owner), SurfacePool> SurfacePools = [];
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

    internal static void BeginFrame(string backend, long contextGeneration, object? contextOwner = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(backend);
        lock (PoolGate)
            GetOrCreatePool(backend, contextGeneration, contextOwner).BeginFrame();
    }

    internal static void InvalidateContext(string backend, long currentContextGeneration, object? contextOwner = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(backend);
        lock (PoolGate)
        {
            foreach (var key in SurfacePools.Keys.Where(key =>
                         ReferenceEquals(key.Owner, contextOwner) && string.Equals(key.Backend, backend, StringComparison.Ordinal) &&
                         key.ContextGeneration != currentContextGeneration).ToArray())
            {
                SurfacePools.Remove(key, out var pool);
                pool?.Dispose();
            }
        }
    }

    internal static void ReleaseContext(string backend, long contextGeneration, object? contextOwner = null)
    {
        lock (PoolGate)
        {
            if (!SurfacePools.Remove((backend, contextGeneration, contextOwner), out var pool)) return;
            pool.Dispose();
        }
    }

    internal static void InvalidateSurface(string backend, long contextGeneration, object? contextOwner = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(backend);
        lock (PoolGate)
        {
            if (!SurfacePools.Remove((backend, contextGeneration, contextOwner), out var pool)) return;
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
        out bool cacheHit, object? contextOwner = null)
    {
        cacheHit = false;
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(shader);
        ArgumentNullException.ThrowIfNull(imageShaderFactory);
        ArgumentNullException.ThrowIfNull(drawChild);
        if (pixelWidth <= 0 || pixelHeight <= 0)
            throw new ArgumentOutOfRangeException(nameof(pixelWidth), "The GPU filter target must have positive dimensions.");
        if (!SkiaGpuSurfaces.IsGpu(target))
            throw new NotSupportedException(
                "Doroti ImageFilter.shader requires the active Skia GPU recording context; software capture is forbidden.");

        var offsetBounds = new SKRect(
            childBounds.Left + childOffset.X,
            childBounds.Top + childOffset.Y,
            childBounds.Right + childOffset.X,
            childBounds.Bottom + childOffset.Y);
        var mappedBounds = target.TotalMatrix.MapRect(offsetBounds);
        if (!IsFinite(mappedBounds)) return false;
        var clip = target.DeviceClipBounds;
        // Intersect in wide coordinates before converting to device integers.
        // A huge but finite child can cover a small viewport without requiring
        // a huge allocation or overflowing an integer conversion.
        var visibleLeft = Math.Max(0, Math.Max(clip.Left, Math.Floor(mappedBounds.Left)));
        var visibleTop = Math.Max(0, Math.Max(clip.Top, Math.Floor(mappedBounds.Top)));
        var visibleRight = Math.Min(pixelWidth, Math.Min(clip.Right, Math.Ceiling(mappedBounds.Right)));
        var visibleBottom = Math.Min(pixelHeight, Math.Min(clip.Bottom, Math.Ceiling(mappedBounds.Bottom)));
        if (visibleRight <= visibleLeft || visibleBottom <= visibleTop) return false;

        var matrix = target.TotalMatrix;
        var cacheLeft = MathF.Floor(mappedBounds.Left);
        var cacheTop = MathF.Floor(mappedBounds.Top);
        var widthExtent = Math.Ceiling(mappedBounds.Right) - cacheLeft;
        var heightExtent = Math.Ceiling(mappedBounds.Bottom) - cacheTop;
        var cacheWidth = widthExtent > 0 && widthExtent <= MaxCacheableImagePixels ? (int)widthExtent : 0;
        var cacheHeight = heightExtent > 0 && heightExtent <= MaxCacheableImagePixels ? (int)heightExtent : 0;
        var cachePixels = (long)cacheWidth * cacheHeight;
        var canCache = cacheKey is not null && cacheWidth > 0 && cacheHeight > 0 &&
                       cachePixels <= MaxCacheableImagePixels &&
                       matrix.Persp0 == 0 && matrix.Persp1 == 0 && matrix.Persp2 == 1;
        using var properties = target.Surface?.SurfaceProperties;
        var signature = TransformSignature.From(matrix, mappedBounds.Left - cacheLeft, mappedBounds.Top - cacheTop,
            properties?.Flags ?? SKSurfacePropsFlags.None, properties?.PixelGeometry ?? SKPixelGeometry.Unknown);
        if (canCache && TryDrawCached(
                target, backend, contextGeneration, cacheKey!, cacheWidth, cacheHeight,
                signature, cacheGeneration, cacheLeft, cacheTop, contextOwner))
        {
            cacheHit = true;
            return true;
        }

        if (canCache)
        {
            var cacheLease = RentSurface(target, backend, contextGeneration, cacheWidth, cacheHeight, contextOwner, properties);
            try
            {
                RenderChild(cacheLease.Surface, cacheLeft, cacheTop,
                    matrix, childOffset, drawChild, cacheWidth, cacheHeight);
                using var inputImage = cacheLease.Surface.Snapshot(new SKRectI(0, 0, cacheWidth, cacheHeight))
                    ?? throw new InvalidOperationException("Doroti ImageFilter.shader could not snapshot its GPU input surface.");
                using var runtimeShader = DorotiSkiaRuntimeEffects.CreateImageFilterShader(
                    shader, inputImage, inputSampling, imageShaderFactory, backend, contextGeneration, contextOwner);
                using var outputSurface = CreateSurface(target, cacheWidth, cacheHeight, properties);
                using (var paint = new SKPaint { Shader = runtimeShader, BlendMode = SKBlendMode.SrcOver })
                    outputSurface.Canvas.DrawRect(SKRect.Create(cacheWidth, cacheHeight), paint);
                outputSurface.Canvas.Flush();
                var outputImage = outputSurface.Snapshot()
                    ?? throw new InvalidOperationException("Doroti ImageFilter.shader could not snapshot its cached output.");
                StoreAndDrawCached(target, backend, contextGeneration, cacheKey!, cacheWidth,
                    cacheHeight, signature, cacheGeneration, cacheLeft, cacheTop, outputImage, contextOwner);
                Interlocked.Increment(ref _imageCacheMisses);
                return true;
            }
            finally
            {
                if (cacheLease.IsTemporary) cacheLease.Surface.Dispose();
            }
        }

        var left = (int)visibleLeft;
        var top = (int)visibleTop;
        var right = (int)visibleRight;
        var bottom = (int)visibleBottom;

        var width = checked(right - left);
        var height = checked(bottom - top);
        var lease = RentSurface(target, backend, contextGeneration, width, height, contextOwner, properties);
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
                contextGeneration, contextOwner);
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
        var saveCount = canvas.SaveCount;
        canvas.Save();
        try
        {
            canvas.Translate(-originX, -originY);
            canvas.Concat(in parentMatrix);
            canvas.Translate(childOffset.X, childOffset.Y);
            drawChild(canvas, width, height);
        }
        finally { canvas.RestoreToCount(saveCount); }
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
        float top, object? contextOwner)
    {
        lock (PoolGate)
        {
            var pool = GetOrCreatePool(backend, contextGeneration, contextOwner);
            if (!pool.Images.TryGetValue(cacheKey, out var cached) || cached.Width != width ||
                cached.Height != height || cached.Transform != signature ||
                cached.Generation != generation)
            {
                if (cached is not null) pool.RemoveImage(cacheKey, cached);
                return false;
            }
            cached.LastUsedSequence = ++pool.UseSequence;
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
        SKImage image, object? contextOwner)
    {
        lock (PoolGate)
        {
            var pool = GetOrCreatePool(backend, contextGeneration, contextOwner);
            if (pool.Images.Remove(cacheKey, out var replaced)) pool.DisposeImage(replaced);
            var cached = new CachedImage(image, width, height, signature, generation, ++pool.UseSequence);
            pool.Images.Add(cacheKey, cached);
            pool.CachedPixels += cached.Pixels;
            DrawImage(target, image, left, top);
            pool.TrimImageCache();
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
        SKCanvas context,
        string backend,
        long contextGeneration,
        int width,
        int height, object? contextOwner, SKSurfaceProperties? properties)
    {
        lock (PoolGate)
        {
            var pool = GetOrCreatePool(backend, contextGeneration, contextOwner);
            var slot = pool.NextSlot++;
            if (slot >= MaxPooledSurfacesPerFrame)
                return new(CreateSurface(context, width, height, properties), true);

            while (pool.Surfaces.Count <= slot) pool.Surfaces.Add(null);
            var surface = pool.Surfaces[slot];
            // GPU snapshots are the implicit texture passed to the runtime
            // effect. Keep that texture exact-sized: reusing a larger pooled
            // surface after a shrink asks the backend for a subset snapshot,
            // which is not reliable for the D3D12 render target path and can
            // return null during rapid small-window layout changes.
            if (surface is null || !SameSurfacePolicy(surface, properties) || surface.Canvas.DeviceClipBounds.Width != width ||
                surface.Canvas.DeviceClipBounds.Height != height)
            {
                surface?.Dispose();
                surface = CreateSurface(context, width, height, properties);
                pool.Surfaces[slot] = surface;
            }
            else
            {
                Interlocked.Increment(ref _surfaceReuses);
            }
            return new(surface, false);
        }
    }

    private static bool SameSurfacePolicy(SKSurface surface, SKSurfaceProperties? properties)
    {
        using var existing = surface.SurfaceProperties;
        return existing.Flags == (properties?.Flags ?? SKSurfacePropsFlags.None) &&
            existing.PixelGeometry == (properties?.PixelGeometry ?? SKPixelGeometry.Unknown);
    }

    private static SKSurface CreateSurface(SKCanvas context, int width, int height, SKSurfaceProperties? properties)
    {
        var info = new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
        var surface = SkiaGpuSurfaces.CreateCompatible(context, info, properties)
            ?? throw new InvalidOperationException(
                $"Doroti ImageFilter.shader could not allocate a {width}x{height} GPU input surface.");
        Interlocked.Increment(ref _surfacesCreated);
        return surface;
    }

    private static SurfacePool GetOrCreatePool(string backend, long contextGeneration, object? contextOwner)
    {
        var key = (backend, contextGeneration, contextOwner);
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
        float Persp2,
        float PhaseX,
        float PhaseY,
        SKSurfacePropsFlags SurfaceFlags,
        SKPixelGeometry PixelGeometry)
    {
        internal static TransformSignature From(SKMatrix matrix, float phaseX, float phaseY,
            SKSurfacePropsFlags surfaceFlags, SKPixelGeometry pixelGeometry) => new(
            matrix.ScaleX, matrix.SkewX, matrix.SkewY, matrix.ScaleY,
            matrix.Persp0, matrix.Persp1, matrix.Persp2, phaseX, phaseY, surfaceFlags, pixelGeometry);
    }

    private sealed class CachedImage(
        SKImage image,
        int width,
        int height,
        TransformSignature transform,
        long generation,
        long lastUsedSequence)
    {
        internal SKImage Image { get; } = image;
        internal int Width { get; } = width;
        internal int Height { get; } = height;
        internal TransformSignature Transform { get; } = transform;
        internal long Generation { get; } = generation;
        internal long LastUsedSequence { get; set; } = lastUsedSequence;
        internal long Pixels => (long)Width * Height;
    }

    private sealed class SurfacePool : IDisposable
    {
        internal List<SKSurface?> Surfaces { get; } = [];
        internal Dictionary<object, CachedImage> Images { get; } =
            new(ReferenceEqualityComparer.Instance);
        internal int NextSlot { get; set; }
        internal long FrameNumber { get; private set; }
        internal long UseSequence { get; set; }
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
                var oldest = Images.MinBy(pair => pair.Value.LastUsedSequence);
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
