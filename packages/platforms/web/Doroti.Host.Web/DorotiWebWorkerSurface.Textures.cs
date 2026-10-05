using System.Globalization;
using System.Runtime.InteropServices.JavaScript;
using Doroti.Skia.Rendering;
using SkiaSharp;

namespace Doroti.Host.Web;

public static partial class DorotiWebWorkerSurface
{
    private static readonly Dictionary<ulong, SkiaSceneRenderer> TextureRenderers = [];
    private static readonly Dictionary<string, BrowserTextureEntry> BrowserTextures = [];
    private static readonly Dictionary<int, BrowserTextureImage> BrowserTextureImages = [];
    private static JSObject? _textureModule;

    internal static void CaptureTextureRaster(
        SKCanvas canvas,
        int order,
        SKRectI bounds,
        double scaleX,
        double scaleY
    )
    {
        canvas.Flush();
        if (_graphiteRecorder is not null)
        {
            using var recording =
                _graphiteRecorder.Snap()
                ?? throw new InvalidOperationException("Missing texture slice recording.");
            if (
                _graphiteContext!.InsertRecording(recording) != SKGraphiteInsertStatus.Success
                || !_graphiteContext.Submit(new SKGraphiteSubmitInfo { Sync = false })
            )
                throw new InvalidOperationException("Texture slice GPU submission failed.");
        }
        else
            _context!.Flush();
        CaptureBrowserTextureRaster(
            order,
            bounds.Left,
            bounds.Top,
            bounds.Width,
            bounds.Height,
            scaleX,
            scaleY
        );
    }

    internal static void AttachTextureRenderer(ulong view, SkiaSceneRenderer renderer) =>
        TextureRenderers.Add(view, renderer);

    internal static void DetachTextureRenderer(SkiaSceneRenderer renderer)
    {
        foreach (
            var view in TextureRenderers
                .Where(pair => ReferenceEquals(pair.Value, renderer))
                .Select(pair => pair.Key)
                .ToArray()
        )
            TextureRenderers.Remove(view);
    }

    [JSExport]
    public static async Task InitializeBrowserTextures(string url) =>
        _textureModule = await JSHost.ImportAsync("doroti-textures", url);

    [JSExport]
    public static string RegisterBrowserTexture()
    {
        if (!_initialized || !TextureRenderers.TryGetValue(_viewId, out var renderer))
            throw new InvalidOperationException("Browser texture view is unavailable.");
        var entry = new BrowserTextureEntry();
        entry.Registration = renderer.RegisterExternalTexture(entry);
        var id = entry.Registration.Id.ToString(CultureInfo.InvariantCulture);
        entry.Id = id;
        BrowserTextures.Add(id, entry);
        return id;
    }

    [JSExport]
    public static void MarkBrowserTexture(string id)
    {
        if (BrowserTextures.TryGetValue(id, out var entry))
            entry.Registration!.MarkFrameAvailable();
    }

    [JSExport]
    public static void UnregisterBrowserTexture(string id)
    {
        if (BrowserTextures.Remove(id, out var entry))
            entry.Registration!.Dispose();
    }

    [JSExport]
    public static void ReleaseBrowserTextureImage(int token)
    {
        if (BrowserTextureImages.Remove(token, out var image))
            image.Dispose();
    }

    private sealed class BrowserTextureImage(SKImage image, IDisposable backend) : IDisposable
    {
        internal SKImage Image => image;

        public void Dispose()
        {
            image.Dispose();
            backend.Dispose();
        }
    }

    private sealed class BrowserTextureEntry : ISkiaExternalTextureSource
    {
        internal string Id = "";
        internal SkiaExternalTextureRegistration? Registration;
        private int _current;

        public void Draw(
            SKCanvas canvas,
            SKRect destination,
            SKSamplingOptions sampling,
            bool freeze
        )
        {
            if (!freeze || _current == 0)
            {
                using var frame = AcquireBrowserTexture(Id);
                if (frame is not null)
                {
                    var token = frame.GetPropertyAsInt32("token");
                    var handle = frame.GetPropertyAsInt32("handle");
                    var width = frame.GetPropertyAsInt32("width");
                    var height = frame.GetPropertyAsInt32("height");
                    IDisposable? backend = null;
                    try
                    {
                        using var color = SKColorSpace.CreateSrgb();
                        SKImage? image;
                        if (_graphiteRecorder is not null)
                        {
                            var dawn =
                                SKGraphiteBackendTexture.CreateDawn(handle)
                                ?? throw new InvalidOperationException(
                                    "Browser Dawn texture wrapper failed."
                                );
                            backend = dawn;
                            image = SKImage.FromTexture(
                                _graphiteRecorder,
                                dawn,
                                SKColorType.Rgba8888,
                                SKAlphaType.Premul,
                                color
                            );
                        }
                        else
                        {
                            _context!.ResetContext(GRGlBackendState.All);
                            var gl = new GRBackendTexture(
                                width,
                                height,
                                false,
                                new GRGlTextureInfo(
                                    0x0DE1,
                                    (uint)handle,
                                    SKColorType.Rgba8888.ToGlSizedFormat()
                                )
                            );
                            backend = gl;
                            image = SKImage.FromTexture(
                                _context,
                                gl,
                                GRSurfaceOrigin.TopLeft,
                                SKColorType.Rgba8888,
                                SKAlphaType.Premul,
                                color
                            );
                        }
                        if (image is null)
                            throw new InvalidOperationException(
                                "Browser GPU texture image wrapping failed."
                            );
                        BrowserTextureImages.Add(token, new(image, backend));
                        backend = null;
                        if (_current != 0)
                            RetireBrowserTexture(_current);
                        _current = token;
                    }
                    catch
                    {
                        backend?.Dispose();
                        RetireBrowserTexture(token);
                        throw;
                    }
                }
            }
            if (_current != 0 && BrowserTextureImages.TryGetValue(_current, out var current))
            {
                canvas.DrawImage(current.Image, destination, sampling);
                BrowserTextureDrawn(_current);
            }
        }

        public void Dispose()
        {
            BrowserTextures.Remove(Id);
            if (_current != 0)
                RetireBrowserTexture(_current);
            _current = 0;
        }
    }

    [JSImport("acquireTexture", "doroti-textures")]
    private static partial JSObject? AcquireBrowserTexture(string id);

    [JSImport("retireTexture", "doroti-textures")]
    private static partial void RetireBrowserTexture(int token);

    [JSImport("textureDrawn", "doroti-textures")]
    private static partial void BrowserTextureDrawn(int token);

    [JSImport("captureTextureRaster", "doroti-textures")]
    private static partial void CaptureBrowserTextureRaster(
        int order,
        int left,
        int top,
        int width,
        int height,
        double scaleX,
        double scaleY
    );
}
