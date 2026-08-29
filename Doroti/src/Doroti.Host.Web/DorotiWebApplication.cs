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
    string RegisterFont(ReadOnlyMemory<byte> bytes);

    DorotiApplicationBoundary LoadApplicationBoundary(
        Assembly manifestAssembly,
        Assembly applicationAssembly,
        IEnumerable<BrowserJavaScriptPluginDescriptor>? plugins = null);

    DorotiView CreateView(
        DorotiHostSession session,
        ulong viewId,
        string canvasId,
        DorotiViewConfiguration configuration,
        DorotiApplicationBoundary? application = null);

    void AttachSkiaSurface(ulong viewId, Action invalidate);

    string PaintSkiaSurface(
        ulong viewId, SKSurface surface, int pixelWidth, int pixelHeight, DorotiResizeEpoch target);

    void CompleteSkiaSurfacePaint(ulong viewId, long generation, bool committed, string reason);

    BrowserHostSnapshot CaptureSnapshot(ulong viewId);

    BrowserFrameDiagnostics CaptureFrameDiagnostics(ulong viewId);
}
