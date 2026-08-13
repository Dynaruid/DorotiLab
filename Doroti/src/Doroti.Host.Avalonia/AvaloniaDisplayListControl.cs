using System.Globalization;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Doroti.Composition;
using Doroti.Graphics;
using Doroti.Platform;
using Doroti.Rendering;
using AvaloniaColor = Avalonia.Media.Color;
using AvaloniaMatrix = Avalonia.Matrix;
using AvaloniaRect = Avalonia.Rect;
using DorotiColor = Doroti.Graphics.Color;
using DorotiMatrix = Doroti.Graphics.Matrix;
using DorotiRect = Doroti.Graphics.Rect;

namespace Doroti.Host.Avalonia;

internal sealed class AvaloniaDisplayListControl : Control, IAvaloniaDisplayListPresenter, IBgra8888FramebufferTarget, IAvaloniaFrameTestController
{
    private readonly object _frameGate = new();
    private DisplayList? _displayList;
    private IReadOnlyDictionary<ResourceId, IResourceSnapshot> _resources = new Dictionary<ResourceId, IResourceSnapshot>();
    private Window? _window;
    private Func<WindowMetrics>? _readMetrics;
    private AvaloniaHostDiagnostics? _diagnostics;
    private WindowId _windowId;
    private byte[]? _pendingPixels;
    private int _pendingWidth;
    private int _pendingHeight;
    private int _pendingRowBytes;
    private WriteableBitmap? _frameBitmap;
    private long _bitmapsCreated;
    private long _bitmapsReleased;
    private long _framesStaged;
    private long _framesImported;
    private long _invalidationsCoalesced;
    private long _pendingArrayAllocationBytes;
    private long _stagingCopyBytes;
    private long _bitmapUploadCopyBytes;
    private int _stagingThreadId;
    private int _importThreadId;
    private int _uploadScheduled;
    private int _failNextPresent;
    private int _staleNextPresent;
    private int _pauseNextPresent;
    private readonly ManualResetEventSlim _presentPaused = new();
    private readonly ManualResetEventSlim _resumePresent = new(true);
    private bool _disposed;
    private AvaloniaAccessibilityBridge? _accessibility;

    internal void Attach(Window window, WindowId windowId, Func<WindowMetrics> readMetrics, AvaloniaHostDiagnostics diagnostics)
    {
        _window = window;
        _windowId = windowId;
        _readMetrics = readMetrics;
        _diagnostics = diagnostics;
        Focusable = true;
    }

    internal void AttachAccessibility(AvaloniaAccessibilityBridge accessibility) => _accessibility = accessibility;

    protected override AutomationPeer OnCreateAutomationPeer() =>
        _accessibility?.CreatePeer() ?? base.OnCreateAutomationPeer();

    public WindowMetrics Metrics => _readMetrics?.Invoke()
        ?? throw new InvalidOperationException("The Avalonia framebuffer target is not attached to a window.");

    internal AvaloniaFrameUploadSnapshot UploadSnapshot => new(
        checked((int)(Interlocked.Read(ref _bitmapsCreated) - Interlocked.Read(ref _bitmapsReleased))),
        Interlocked.Read(ref _bitmapsCreated),
        Interlocked.Read(ref _bitmapsReleased),
        Interlocked.Read(ref _framesStaged),
        Interlocked.Read(ref _framesImported),
        Interlocked.Read(ref _invalidationsCoalesced),
        Volatile.Read(ref _stagingThreadId),
        Volatile.Read(ref _importThreadId),
        Interlocked.Read(ref _pendingArrayAllocationBytes),
        Interlocked.Read(ref _stagingCopyBytes),
        Interlocked.Read(ref _bitmapUploadCopyBytes));

    public void Present(DisplayList displayList, IReadOnlyDictionary<ResourceId, IResourceSnapshot> resources)
    {
        AvaloniaWindowBackend.RequireUiThread();
        AvaloniaHostDisplayListSupport.Validate(displayList, resources);
        _displayList = displayList;
        _resources = resources.ToDictionary(item => item.Key, item => item.Value);
        DisposeFrameBitmap();
        InvalidateVisual();
    }

    public AvaloniaPixelReadback Capture(string? screenshotPath = null)
    {
        AvaloniaWindowBackend.RequireUiThread();
        if (_displayList is null || Bounds.Width <= 0 || Bounds.Height <= 0)
        {
            if (_frameBitmap is null || Bounds.Width <= 0 || Bounds.Height <= 0)
            {
                throw new InvalidOperationException("The Avalonia frame scene must be laid out before capture.");
            }
        }
        var scale = Metrics.ScaleFactor;
        var extent = PixelExtentPolicy.ToPixelSize(new(Bounds.Width, Bounds.Height), scale);
        var pixelSize = new PixelSize(Math.Max(1, checked((int)extent.Width)), Math.Max(1, checked((int)extent.Height)));
        using var bitmap = new RenderTargetBitmap(pixelSize, new Vector(96 * scale, 96 * scale));
        bitmap.Render(this);
        if (!string.IsNullOrWhiteSpace(screenshotPath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(screenshotPath))!);
            bitmap.Save(screenshotPath, PngBitmapEncoderOptions.Default);
        }

        var rowBytes = checked(pixelSize.Width * 4);
        var pixels = GC.AllocateUninitializedArray<byte>(checked(rowBytes * pixelSize.Height));
        using var target = new WriteableBitmap(pixelSize, new Vector(96 * scale, 96 * scale), PixelFormat.Bgra8888, AlphaFormat.Premul);
        using (var framebuffer = target.Lock())
        {
            bitmap.CopyPixels(framebuffer);
            for (var y = 0; y < pixelSize.Height; y++)
            {
                Marshal.Copy(framebuffer.Address + (y * framebuffer.RowBytes), pixels, y * rowBytes, rowBytes);
            }
        }
        return new(new(pixelSize.Width, pixelSize.Height), rowBytes, pixels);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (_frameBitmap is not null)
        {
            context.DrawImage(_frameBitmap, new AvaloniaRect(_frameBitmap.Size), new AvaloniaRect(Bounds.Size));
            return;
        }
        if (_displayList is null)
        {
            return;
        }

        var rootStates = new List<DrawingContext.PushedState>();
        var savedStates = new Stack<List<DrawingContext.PushedState>>();
        var activeStates = rootStates;
        try
        {
            foreach (var command in _displayList.Commands)
            {
                switch (command)
                {
                    case SaveCommand:
                        activeStates = [];
                        savedStates.Push(activeStates);
                        break;
                    case RestoreCommand:
                        DisposeReverse(activeStates);
                        savedStates.Pop();
                        activeStates = savedStates.Count > 0 ? savedStates.Peek() : rootStates;
                        break;
                    case TransformCommand transform:
                        activeStates.Add(context.PushTransform(ToAvalonia(transform.Transform)));
                        break;
                    case ClipRectCommand clip:
                        activeStates.Add(context.PushClip(ToAvalonia(clip.Rect)));
                        break;
                    case ClipPathCommand clip:
                        activeStates.Add(context.PushGeometryClip(ToAvalonia(clip.Path)));
                        break;
                    case DrawColorCommand color:
                        context.FillRectangle(new SolidColorBrush(ToAvalonia(color.Color)), new AvaloniaRect(Bounds.Size));
                        break;
                    case DrawRectCommand rect:
                        context.FillRectangle(new SolidColorBrush(ToAvalonia(rect.Paint.Color, rect.Paint.Opacity)), ToAvalonia(rect.Rect));
                        break;
                    case DrawPathCommand path:
                        context.DrawGeometry(new SolidColorBrush(ToAvalonia(path.Paint.Color, path.Paint.Opacity)), null, ToAvalonia(path.Path));
                        break;
                    case DrawTextCommand text:
                        var formatted = new FormattedText(
                            text.Text,
                            CultureInfo.InvariantCulture,
                            FlowDirection.LeftToRight,
                            text.FontFamily is null ? Typeface.Default : new Typeface(text.FontFamily),
                            text.FontSize,
                            new SolidColorBrush(ToAvalonia(text.Paint.Color, text.Paint.Opacity)));
                        context.DrawText(formatted, new(text.Origin.X, text.Origin.Y - text.FontSize));
                        break;
                    case DrawImageCommand image:
                        if (!_resources.TryGetValue(image.Resource, out var snapshot) || snapshot is not ImageResourceSnapshot pixels)
                        {
                            throw new InvalidOperationException($"Image resource {image.Resource.Value} is unavailable.");
                        }
                        using (var bitmap = CreateBitmap(pixels))
                        {
                            context.DrawImage(bitmap, ToAvalonia(image.Source), ToAvalonia(image.Destination));
                        }
                        break;
                }
            }
        }
        finally
        {
            while (savedStates.Count > 0)
            {
                DisposeReverse(savedStates.Pop());
            }
            DisposeReverse(rootStates);
        }
    }

    void IBgra8888FramebufferTarget.Present(ReadOnlySpan<byte> pixels, int width, int height, int rowBytes)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (Interlocked.Exchange(ref _pauseNextPresent, 0) != 0)
        {
            _resumePresent.Reset();
            _presentPaused.Set();
            _resumePresent.Wait(TimeSpan.FromSeconds(10));
            _presentPaused.Reset();
        }
        if (Interlocked.Exchange(ref _staleNextPresent, 0) != 0)
        {
            throw new SurfaceStaleFrameException("The Avalonia target rejected a forced stale frame.");
        }
        if (Interlocked.Exchange(ref _failNextPresent, 0) != 0)
        {
            throw new SurfaceDeviceLostException("The Avalonia target rejected a forced present for recovery validation.");
        }
        var expected = Metrics.PixelSize;
        var expectedWidth = Math.Max(1, checked((int)expected.Width));
        var expectedHeight = Math.Max(1, checked((int)expected.Height));
        if (width != expectedWidth || height != expectedHeight)
        {
            throw new SurfaceStaleFrameException(
                $"Frame size {width}x{height} is stale; the Avalonia target is {expectedWidth}x{expectedHeight}.");
        }
        var sourceRowBytes = checked(width * 4);
        if (rowBytes < sourceRowBytes || pixels.Length < checked(rowBytes * height))
        {
            throw new ArgumentException("The presented BGRA8888 buffer is smaller than its declared extent.", nameof(pixels));
        }
        var copy = GC.AllocateUninitializedArray<byte>(checked(sourceRowBytes * height));
        Interlocked.Add(ref _pendingArrayAllocationBytes, copy.Length);
        for (var row = 0; row < height; row++)
        {
            pixels.Slice(row * rowBytes, sourceRowBytes).CopyTo(copy.AsSpan(row * sourceRowBytes, sourceRowBytes));
        }
        Interlocked.Add(ref _stagingCopyBytes, copy.Length);
        _diagnostics?.Record(
            "frame-staged",
            _windowId,
            Metrics,
            $"size={width}x{height};pending-array-bytes={copy.Length};staging-copy-bytes={copy.Length}");
        lock (_frameGate)
        {
            if (_pendingPixels is not null)
            {
                Interlocked.Increment(ref _invalidationsCoalesced);
            }
            _pendingPixels = copy;
            _pendingWidth = width;
            _pendingHeight = height;
            _pendingRowBytes = sourceRowBytes;
        }
        Volatile.Write(ref _stagingThreadId, Environment.CurrentManagedThreadId);
        Interlocked.Increment(ref _framesStaged);
        if (Interlocked.Exchange(ref _uploadScheduled, 1) == 0)
        {
            Dispatcher.UIThread.Post(ImportLatestFrame, DispatcherPriority.Render);
        }
    }

    public void PauseNextPresent() => Interlocked.Exchange(ref _pauseNextPresent, 1);

    public bool WaitForPausedPresent(TimeSpan timeout) => _presentPaused.Wait(timeout);

    public void ResumePresent() => _resumePresent.Set();

    public void FailNextPresent() => Interlocked.Exchange(ref _failNextPresent, 1);

    public void StaleNextPresent() => Interlocked.Exchange(ref _staleNextPresent, 1);

    internal void DisposeFrameResources()
    {
        AvaloniaWindowBackend.RequireUiThread();
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        _resumePresent.Set();
        lock (_frameGate)
        {
            _pendingPixels = null;
        }
        DisposeFrameBitmap();
        _presentPaused.Dispose();
        _resumePresent.Dispose();
    }

    private void ImportLatestFrame()
    {
        AvaloniaWindowBackend.RequireUiThread();
        byte[]? pixels;
        int width;
        int height;
        int rowBytes;
        lock (_frameGate)
        {
            pixels = _pendingPixels;
            width = _pendingWidth;
            height = _pendingHeight;
            rowBytes = _pendingRowBytes;
            _pendingPixels = null;
        }
        Interlocked.Exchange(ref _uploadScheduled, 0);
        if (_disposed || pixels is null)
        {
            return;
        }
        var scale = _window?.RenderScaling ?? 1;
        var bitmap = new WriteableBitmap(new(width, height), new(96 * scale, 96 * scale), PixelFormat.Bgra8888, AlphaFormat.Premul);
        using (var framebuffer = bitmap.Lock())
        {
            for (var row = 0; row < height; row++)
            {
                Marshal.Copy(pixels, row * rowBytes, framebuffer.Address + (row * framebuffer.RowBytes), rowBytes);
            }
        }
        Interlocked.Add(ref _bitmapUploadCopyBytes, checked((long)rowBytes * height));
        Interlocked.Increment(ref _bitmapsCreated);
        DisposeFrameBitmap();
        _frameBitmap = bitmap;
        _displayList = null;
        Volatile.Write(ref _importThreadId, Environment.CurrentManagedThreadId);
        Interlocked.Increment(ref _framesImported);
        _diagnostics?.Record("frame-imported", _windowId, Metrics, $"size={width}x{height};staging-thread={_stagingThreadId}");
        InvalidateVisual();
        lock (_frameGate)
        {
            if (_pendingPixels is not null && Interlocked.Exchange(ref _uploadScheduled, 1) == 0)
            {
                Dispatcher.UIThread.Post(ImportLatestFrame, DispatcherPriority.Render);
            }
        }
    }

    private void DisposeFrameBitmap()
    {
        if (_frameBitmap is null)
        {
            return;
        }
        _frameBitmap.Dispose();
        _frameBitmap = null;
        Interlocked.Increment(ref _bitmapsReleased);
    }

    private static WriteableBitmap CreateBitmap(ImageResourceSnapshot image)
    {
        var bitmap = new WriteableBitmap(new(image.Width, image.Height), new(96, 96), PixelFormat.Bgra8888, AlphaFormat.Premul);
        using var framebuffer = bitmap.Lock();
        var rowBytes = checked(image.Width * 4);
        var pixels = image.Pixels.AsSpan();
        for (var row = 0; row < image.Height; row++)
        {
            Marshal.Copy(pixels.Slice(row * rowBytes, rowBytes).ToArray(), 0, framebuffer.Address + (row * framebuffer.RowBytes), rowBytes);
        }
        return bitmap;
    }

    private static void DisposeReverse(List<DrawingContext.PushedState> states)
    {
        for (var index = states.Count - 1; index >= 0; index--)
        {
            states[index].Dispose();
        }
        states.Clear();
    }

    private static AvaloniaColor ToAvalonia(DorotiColor color, double opacity = 1) => AvaloniaColor.FromArgb(
        (byte)Math.Round(color.Alpha * opacity),
        color.Red,
        color.Green,
        color.Blue);

    private static AvaloniaRect ToAvalonia(DorotiRect rect) => new(rect.Left, rect.Top, rect.Width, rect.Height);

    private static AvaloniaMatrix ToAvalonia(DorotiMatrix matrix) => new(
        matrix.M11,
        matrix.M21,
        matrix.M12,
        matrix.M22,
        matrix.M14,
        matrix.M24);

    private static Geometry ToAvalonia(Doroti.Graphics.PathGeometry path)
    {
        var geometry = new StreamGeometry();
        using var context = geometry.Open();
        context.SetFillRule(path.FillRule == Doroti.Graphics.PathFillRule.EvenOdd ? FillRule.EvenOdd : FillRule.NonZero);
        var first = path.Points[0];
        context.BeginFigure(new(first.X, first.Y), path.IsClosed);
        foreach (var point in path.Points.Skip(1))
        {
            context.LineTo(new(point.X, point.Y));
        }
        context.EndFigure(path.IsClosed);
        return geometry;
    }
}
