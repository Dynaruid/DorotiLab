using System.Runtime.InteropServices.JavaScript;
using System.Text;
using Doroti.Skia.Rendering;
using Doroti.Skia.RuntimeEffects;
using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Host.Web;

public static partial class DorotiWebWorkerSurface
{
    private sealed class BrowserGpuEffectBackend : ISkiaGpuEffectBackend
    {
        public void Draw(SKCanvas destination, int width, int height, Action<SKCanvas> capture,
            GpuEffectProgram program, GpuEffectParameters parameters, float logicalWidth = 0, float logicalHeight = 0)
        {
            var variant = program.GetVariant(_graphiteRecorder is null ? "webgl2-fragment" : "webgpu-fragment");
            if (parameters.Bytes.Length != program.ParameterByteCount)
                throw new ArgumentException("GPU effect parameter ABI mismatch.");
            using var allocation = AllocateBrowserEffect(width, height);
            var token = allocation.GetPropertyAsInt32("token");
            var wrappers = new EffectWrappers();
            SKImage? image = null;
            var retained = false;
            try
            {
                SKSurface input;
                using var color = SKColorSpace.CreateSrgb();
                if (_graphiteRecorder is not null)
                {
                    var inputTexture = SKGraphiteBackendTexture.CreateDawn(allocation.GetPropertyAsInt32("input"))
                        ?? throw new InvalidOperationException("Cannot wrap Dawn effect input.");
                    wrappers.Items.Add(inputTexture);
                    var outputTexture = SKGraphiteBackendTexture.CreateDawn(allocation.GetPropertyAsInt32("output"))
                        ?? throw new InvalidOperationException("Cannot wrap Dawn effect output.");
                    wrappers.Items.Add(outputTexture);
                    input = SkiaGpuSurfaces.Register(SKSurface.Create(_graphiteRecorder, inputTexture, SKColorType.Rgba8888)
                        ?? throw new InvalidOperationException("Cannot create Dawn effect input surface."), _graphiteRecorder);
                    wrappers.Items.Add(input);
                    image = SKImage.FromTexture(_graphiteRecorder, outputTexture, SKColorType.Rgba8888, SKAlphaType.Premul, color);
                }
                else
                {
                    _context!.ResetContext(GRGlBackendState.All);
                    GRBackendTexture Texture(string name)
                    {
                        var texture = new GRBackendTexture(width, height, false,
                            new GRGlTextureInfo(0x0DE1, (uint)allocation.GetPropertyAsInt32(name), SKColorType.Rgba8888.ToGlSizedFormat()));
                        wrappers.Items.Add(texture);
                        return texture;
                    }
                    var inputTexture = Texture("input");
                    var outputTexture = Texture("output");
                    input = SKSurface.Create(_context, inputTexture, GRSurfaceOrigin.TopLeft, SKColorType.Rgba8888)
                        ?? throw new InvalidOperationException("Cannot wrap WebGL effect input.");
                    wrappers.Items.Add(input);
                    image = SKImage.FromTexture(_context, outputTexture, GRSurfaceOrigin.TopLeft,
                        SKColorType.Rgba8888, SKAlphaType.Premul, color);
                }
                if (image is null) throw new InvalidOperationException("Cannot wrap GPU effect output image.");
                BrowserTextureImages.Add(token, new(image, wrappers));
                retained = true;
                capture(input.Canvas);
                input.Canvas.Flush();
                if (_graphiteRecorder is not null)
                {
                    using var segment = _graphiteRecorder.Snap()
                        ?? throw new InvalidOperationException("GPU effect capture did not produce a Dawn recording.");
                    if (_graphiteContext!.InsertRecording(segment) != SKGraphiteInsertStatus.Success ||
                        !_graphiteContext.Submit(new SKGraphiteSubmitInfo { Sync = false }))
                        throw new InvalidOperationException("GPU effect Dawn capture submission failed.");
                    SkiaGpuSurfaces.CompleteRecording(_graphiteRecorder, discarded: false);
                }
                else _context!.Flush();
                ExecuteBrowserEffect(token, program.ContentHash, Encoding.UTF8.GetString(variant.Vertex),
                    Encoding.UTF8.GetString(variant.Fragment), variant.EntryPoint, variant.BindingMetadata, Convert.ToBase64String(parameters.Bytes), parameters.Time, parameters.DeltaTime,
                    logicalWidth > 0 ? logicalWidth : width, logicalHeight > 0 ? logicalHeight : height);
                _context?.ResetContext(GRGlBackendState.All);
                destination.DrawImage(image, 0, 0, new SKSamplingOptions(SKFilterMode.Nearest));
            }
            finally
            {
                if (!retained) { image?.Dispose(); wrappers.Dispose(); }
                // Worker flushRetired inserts its fence after the final Skia flush,
                // including a failed frame. The managed wrappers live to that fence.
                RetireBrowserTexture(token);
                _context?.ResetContext(GRGlBackendState.All);
            }
        }
    }

    private sealed class EffectWrappers : IDisposable
    {
        internal readonly List<IDisposable> Items = [];
        public void Dispose()
        {
            for (var i = Items.Count - 1; i >= 0; i--) Items[i].Dispose();
            Items.Clear();
        }
    }

    [JSImport("allocateEffect", "doroti-textures")]
    private static partial JSObject AllocateBrowserEffect(int width, int height);

    [JSImport("executeEffect", "doroti-textures")]
    private static partial void ExecuteBrowserEffect(int token, string key, string vertex,
        string fragment, string entry, string metadata, string parameters, double time, double deltaTime, double logicalWidth, double logicalHeight);
}
