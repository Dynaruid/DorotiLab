using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Doroti.Flutter.Hosting;
using Doroti.Flutter.Ui;
using Doroti.Generated.Framework.Services;
using Doroti.Host.Desktop.Flutter;
using Doroti.Target.Windows;
using Path = System.IO.Path;
using UiColor = Doroti.Flutter.Ui.Color;

if (!OperatingSystem.IsWindows())
{
    Console.Error.WriteLine("G5-6W package smoke requires Windows.");
    return 2;
}

var evidencePath = ReadEvidencePath(args);
var trace = new List<string>();
var entrypoint = new SmokeEntrypoint(trace);
DesktopFlutterTargetDiagnostics diagnostics;
WindowsTargetIdentity identity;

using (var target = new WindowsFlutterTarget())
using (var session = new FlutterHostSession(entrypoint))
using (var scope = session.dispatcher.EnterScope())
{
    identity = target.Identity;
    session.Start(deferFrameworkBootstrap: true);
    var view = target.CreateView(session, 560, new("Doroti G5-6W package smoke", new(420, 280)));
    view.Show();
    PumpUntil(target, () => target.CaptureDiagnostics(view.viewId).Frame.Presented >= 1, TimeSpan.FromSeconds(8));

    var hwnd = target.GetNativeWindowHandle(view.viewId);
    _ = SetFocus(hwnd);
    _ = PostMessage(hwnd, 0x0200, 0, MakeLParam(42, 44));
    _ = PostMessage(hwnd, 0x0201, 0x0001, MakeLParam(42, 44));
    _ = PostMessage(hwnd, 0x0202, 0, MakeLParam(42, 44));
    _ = PostMessage(hwnd, 0x0100, 0x09, 0);
    _ = PostMessage(hwnd, 0x0101, 0x09, 0);
    PumpUntil(target, () =>
    {
        var current = target.CaptureDiagnostics(view.viewId);
        return current.Input.PointerPackets >= 3 && current.Input.KeyPackets >= 2;
    }, TimeSpan.FromSeconds(3));

    session.dispatcher.setSemanticsTreeEnabled(true);
    view.updateSemantics(new SemanticsUpdate(7,
    [
        new SemanticsNodeUpdate(1, Rect.fromLTWH(0, 0, 420, 280), "G5-6W root", null, SemanticsAction.none, [2]),
        new SemanticsNodeUpdate(2, Rect.fromLTWH(20, 20, 180, 48), "G5-6W action", null,
            SemanticsAction.tap | SemanticsAction.focus, [], new(isButton: true, isEnabled: Tristate.isTrue)),
    ]));
    PumpUntil(target, () => target.CaptureDiagnostics(view.viewId).Automation.NodeCount == 2, TimeSpan.FromSeconds(3));
    if (SendMessage(hwnd, 0x003D, 0, -25) == 0)
    {
        throw new InvalidDataException("The packaged target did not expose its WM_GETOBJECT automation root.");
    }

    target.FailNextGpuFrameForValidation(view.viewId);
    entrypoint.RequestFrame();
    PumpUntil(target, () => target.CaptureDiagnostics(view.viewId).Frame.RecoveryCount >= 1, TimeSpan.FromSeconds(8));
    entrypoint.RequestFrame();
    PumpUntil(target, () => target.CaptureDiagnostics(view.viewId).Frame.Presented >= 2, TimeSpan.FromSeconds(8));
    diagnostics = target.CaptureDiagnostics(view.viewId);

    Require(diagnostics.SchemaVersion == target.Manifest.DiagnosticSchema, "Diagnostic schema drifted from the RID manifest.");
    Require(diagnostics.TargetIdentity.EndsWith("/win32-wgl", StringComparison.Ordinal), "Runtime target identity is not Win32/WGL.");
    Require(FlutterCapabilityIds.RequiredDesktop.All(id => diagnostics.CapabilityIds.Contains(id, StringComparer.Ordinal)), "The packaged target capability closure is incomplete.");
    Require(diagnostics.Frame.BackendIdentity == "skia-wgl-opengl-gpu", "Strict WGL/OpenGL was not selected.");
    Require(!diagnostics.Frame.SoftwareFallbackUsed, "The packaged target silently used software rendering.");
    Require(diagnostics.Frame.RecoveryCount >= 1, "The packaged target did not recover from injected GPU failure.");
    Require(diagnostics.Frame.QueueHighWatermark <= 2, "The frame mailbox exceeded the two-frame bound.");
    Require(diagnostics.Input.PointerPackets >= 3 && diagnostics.Input.KeyPackets >= 2, "Synthetic HWND input did not reach Flutter.");
    Require(diagnostics.Automation.Generation == 7 && diagnostics.Automation.NodeCount == 2, "Automation diagnostics did not observe the Flutter semantics tree.");
    Require(diagnostics.Resources.ActiveWindows == 1 && diagnostics.Resources.ActiveOpenGlContexts == 1, "Native resource diagnostics did not observe the active target resources.");

    session.Shutdown();
}

var report = new
{
    schemaVersion = "doroti.g5-6w-package-smoke/v1",
    status = "PASS",
    identity,
    diagnostics,
    lifecycleTrace = trace,
    physical = new
    {
        status = "notVerified",
        deferredTo = "G5-8 DorotiDemoApp",
        items = new[] { "physical mouse", "precision touchpad", "touch", "Korean IME", "cross-monitor DPI", "external physical accessibility", "sustained GPU" },
    },
};
if (evidencePath is not null)
{
    Directory.CreateDirectory(Path.GetDirectoryName(evidencePath)!);
    File.WriteAllText(evidencePath, JsonSerializer.Serialize(report, new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    }) + "\n", new UTF8Encoding(false));
}
Console.WriteLine("G5-6W-WINDOWS-TARGET-PACKAGE-SMOKE-PASS");
return 0;

static string? ReadEvidencePath(string[] arguments)
{
    if (arguments.Length == 0)
    {
        return null;
    }
    if (arguments.Length != 2 || arguments[0] != "--evidence")
    {
        throw new ArgumentException("Usage: [--evidence <path>]");
    }
    return Path.GetFullPath(arguments[1]);
}

static void PumpUntil(WindowsFlutterTarget target, Func<bool> completed, TimeSpan timeout)
{
    var deadline = DateTime.UtcNow + timeout;
    while (!completed() && DateTime.UtcNow < deadline)
    {
        target.PumpPendingMessages();
        Thread.Sleep(1);
    }
    target.PumpPendingMessages();
    if (!completed())
    {
        throw new TimeoutException("The packaged Windows target smoke timed out.");
    }
}

static void Require(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidDataException(message);
    }
}

static nint MakeLParam(short x, short y) => (nint)((ushort)x | (y << 16));

[DllImport("user32.dll")]
static extern bool PostMessage(nint window, uint message, nint wParam, nint lParam);

[DllImport("user32.dll")]
static extern nint SetFocus(nint window);

[DllImport("user32.dll")]
static extern nint SendMessage(nint window, uint message, nint wParam, nint lParam);

sealed class SmokeEntrypoint(List<string> trace) : IFlutterViewEntrypoint
{
    private readonly List<string> _trace = trace;
    private SmokeBinding? _binding;
    private FlutterView? _view;

    public void Bootstrap(PlatformDispatcher dispatcher)
    {
        _trace.Add("bootstrap");
        _binding = new(dispatcher, DrawFrame);
    }

    public void AttachView(FlutterView view)
    {
        _view = view;
        _trace.Add($"attach:{view.viewId}");
        RequestFrame();
    }

    public void DetachView(FlutterView view)
    {
        if (ReferenceEquals(_view, view))
        {
            _trace.Add($"detach:{view.viewId}");
            _view = null;
        }
    }

    public void RequestFrame() => _binding?.scheduleFrame();

    public void Shutdown()
    {
        _binding?.Dispose();
        _binding = null;
        _trace.Add("shutdown");
    }

    private void DrawFrame()
    {
        var view = _view ?? throw new InvalidOperationException("A frame arrived without a view.");
        var recorder = new PictureRecorder();
        var canvas = new Canvas(recorder, Rect.fromLTWH(0, 0, view.metrics.logicalSize.width, view.metrics.logicalSize.height));
        canvas.drawPaint(new Paint { color = new UiColor(0xff10233f) });
        canvas.drawRect(Rect.fromLTWH(24, 28, 180, 96), new Paint { color = new UiColor(0xff36c2a3) });
        using var picture = recorder.endRecording();
        var builder = new SceneBuilder(view.viewId);
        builder.addPicture(Offset.zero, picture);
        using var scene = builder.build();
        view.SubmitScene(scene, DartUiInvocation.Managed("package:g5_6w/smoke.dart#drawFrame"));
    }
}

sealed class SmokeBinding : ServicesBinding
{
    private readonly Action _drawFrame;

    internal SmokeBinding(PlatformDispatcher dispatcher, Action drawFrame)
        : base(dispatcher) => _drawFrame = drawFrame;

    public override void handleDrawFrame()
    {
        base.handleDrawFrame();
        _drawFrame();
    }
}
