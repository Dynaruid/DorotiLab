using System.Reflection;
using Doroti.Hosting;
using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Host.Web;

public sealed record BrowserFrameDiagnostics(
    long Submitted,
    long Presented,
    long Replayed,
    long Failed,
    long ContextGeneration,
    long SurfaceGeneration,
    bool InvalidatePending,
    string BackendIdentity);

/// <summary>Composition-root boundary implemented by Doroti.Target.Web.browser-wasm.</summary>
public interface IDorotiBrowserTarget : IDisposable
{
    DorotiApplicationBoundary LoadApplicationBoundary(
        Assembly applicationAssembly,
        IEnumerable<BrowserJavaScriptPluginDescriptor>? plugins = null);

    DorotiView CreateView(
        DorotiHostSession session,
        ulong viewId,
        string canvasId,
        DorotiViewConfiguration configuration,
        DorotiApplicationBoundary? application = null);

    void AttachSkiaSurface(ulong viewId, Action invalidate);

    void PaintSkiaSurface(ulong viewId, SKSurface surface, int pixelWidth, int pixelHeight);

    BrowserFrameDiagnostics CaptureFrameDiagnostics(ulong viewId);
}
