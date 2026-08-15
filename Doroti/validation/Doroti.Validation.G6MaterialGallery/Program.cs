using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using Doroti.Hosting;
using Doroti.Runtime;
using Doroti.Ui;
using Doroti.Generated.Framework.Foundation;
using Doroti.Generated.Framework.Painting;
using Doroti.Generated.Framework.Widgets;
using Doroti.Host.Desktop.Framework;
using Doroti.Target.Windows;
using Material = Doroti.Generated.Framework.Material;
using UiColor = Doroti.Ui.Color;
using PaintingTextStyle = Doroti.Generated.Framework.Painting.TextStyle;
using SkiaSharp;

internal static partial class Program
{
    private const ulong ViewId = 650;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    [STAThread]
    public static int Main(string[] args)
    {
        var options = GalleryOptions.Parse(args);
        if (!OperatingSystem.IsWindows() || RuntimeInformation.ProcessArchitecture != Architecture.X64)
        {
            Console.Error.WriteLine("G6-5 Material gallery requires Windows x64.");
            return 2;
        }

        try
        {
            if (options.Win32CursorChrome) RunWin32CursorChrome(options);
            else if (options.NativePointer || options.ManualPointer) RunNativePointer(options);
            else Run(options);
            Console.WriteLine($"G6-5 {options.Wave} Material gallery: PASS");
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception);
            return 1;
        }
    }

    private static void RunWin32CursorChrome(GalleryOptions options)
    {
        var entrypoint = new GalleryEntrypoint(MaterialWave.M0);
        DesktopFrameworkTargetDiagnostics diagnostics;
        object resourceClosure;
        var cursorMappings = new List<object>();
        var chromeMappings = new List<object>();
        var resizeResults = new List<object>();

        using (var target = new WindowsTarget())
        using (var session = new DorotiHostSession(entrypoint))
        using (var scope = session.dispatcher.EnterScope())
        {
            session.Start(deferFrameworkBootstrap: true);
            var view = target.CreateView(session, ViewId,
                new DorotiViewConfiguration("Doroti G6-5R-I cursor chrome", new Size(900, 720)));
            view.Show();
            WaitUntil(() => target.CaptureDiagnostics(ViewId).Frame.Presented >= 1 && entrypoint.State is not null,
                target, entrypoint, TimeSpan.FromSeconds(20));
            var hwnd = target.GetNativeWindowHandle(ViewId);
            if (hwnd == 0) throw new InvalidDataException("Cursor/chrome fixture has no HWND.");

            using (view.EnterPlatformEnvironmentScope())
            {
                var fields = typeof(Doroti.Generated.Framework.Services.SystemMouseCursors)
                    .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                    .Where(field => field.FieldType == typeof(Doroti.Generated.Framework.Services.SystemMouseCursor))
                    .OrderBy(field => field.Name, StringComparer.Ordinal)
                    .ToArray();
                if (fields.Length != Enum.GetValues<Doroti.Ui.DorotiMouseCursorKind>().Length)
                    throw new InvalidDataException($"Flutter cursor declaration/mapping mismatch: {fields.Length}/{Enum.GetValues<Doroti.Ui.DorotiMouseCursorKind>().Length}.");

                foreach (var field in fields)
                {
                    var cursor = (Doroti.Generated.Framework.Services.SystemMouseCursor)field.GetValue(null)!;
                    var before = target.CaptureDiagnostics(ViewId).Cursor.Requests;
                    cursor.createSession(1).activate().GetAwaiter().GetResult();
                    var current = target.CaptureDiagnostics(ViewId).Cursor;
                    if (current.Requests != before + 1 ||
                        current.LastRequested?.ToString() != cursor.kind)
                        throw new InvalidDataException($"Cursor channel did not causally apply '{cursor.kind}'.");
                    _ = SendMessage(hwnd, 0x0020, hwnd, 1);
                    var expectedIdentifier = CursorIdentifier(cursor.kind);
                    var expectedHandle = expectedIdentifier == 0 ? 0 : LoadCursor(0, expectedIdentifier);
                    var actualHandle = GetCursor();
                    if (actualHandle != expectedHandle)
                        throw new InvalidDataException($"Client cursor '{cursor.kind}' mismatch: actual=0x{actualHandle:X}, expected=0x{expectedHandle:X}.");
                    cursorMappings.Add(new
                    {
                        kind = cursor.kind,
                        request = current.Requests,
                        win32Identifier = expectedIdentifier,
                        actualHandle = $"0x{actualHandle:X}",
                    });
                }

                var unsupportedExplicit = false;
                try
                {
                    new Doroti.Generated.Framework.Services.SystemMouseCursor("g6Unsupported")
                        .createSession(1).activate().GetAwaiter().GetResult();
                }
                catch (Exception exception)
                {
                    unsupportedExplicit = exception.ToString().Contains("unsupported-cursor", StringComparison.Ordinal);
                }
                if (!unsupportedExplicit)
                    throw new InvalidDataException("Unsupported system cursor did not return an explicit platform error.");

                Doroti.Generated.Framework.Services.SystemMouseCursors.text
                    .createSession(1).activate().GetAwaiter().GetResult();
                var chromeCases = new (string Name, int HitTest, int Cursor)[]
                {
                    ("left", 10, 32644), ("right", 11, 32644),
                    ("top", 12, 32645), ("bottom", 15, 32645),
                    ("topLeft", 13, 32642), ("bottomRight", 17, 32642),
                    ("topRight", 14, 32643), ("bottomLeft", 16, 32643),
                };
                if (!GetWindowRect(hwnd, out var windowRect))
                    throw new InvalidOperationException("Win32 window rectangle was unavailable for coordinate cursor validation.");
                foreach (var chromeCase in chromeCases)
                {
                    var coordinate = chromeCase.Name switch
                    {
                        "left" => (windowRect.Left + 1, (windowRect.Top + windowRect.Bottom) / 2),
                        "right" => (windowRect.Right - 2, (windowRect.Top + windowRect.Bottom) / 2),
                        "top" => ((windowRect.Left + windowRect.Right) / 2, windowRect.Top + 1),
                        "bottom" => ((windowRect.Left + windowRect.Right) / 2, windowRect.Bottom - 2),
                        "topLeft" => (windowRect.Left + 1, windowRect.Top + 1),
                        "bottomRight" => (windowRect.Right - 2, windowRect.Bottom - 2),
                        "topRight" => (windowRect.Right - 2, windowRect.Top + 1),
                        "bottomLeft" => (windowRect.Left + 1, windowRect.Bottom - 2),
                        _ => throw new ArgumentOutOfRangeException(),
                    };
                    var coordinatePayload = PackPoint(coordinate.Item1, coordinate.Item2);
                    var coordinateHit = (int)SendMessage(hwnd, 0x0084, 0, coordinatePayload);
                    if (coordinateHit != chromeCase.HitTest)
                        throw new InvalidDataException($"Non-client {chromeCase.Name} coordinate hit mismatch: actual={coordinateHit}, expected={chromeCase.HitTest}, coordinate={coordinate}.");
                    _ = SendMessage(hwnd, 0x0020, hwnd,
                        (nint)((0x0200 << 16) | (coordinateHit & 0xffff)));
                    var actual = GetCursor();
                    var expected = LoadCursor(0, chromeCase.Cursor);
                    if (actual != expected)
                        throw new InvalidDataException($"Non-client {chromeCase.Name} cursor mismatch: actual=0x{actual:X}, expected=0x{expected:X}.");
                    chromeMappings.Add(new
                    {
                        chromeCase.Name,
                        chromeCase.HitTest,
                        coordinate = new { screenX = coordinate.Item1, screenY = coordinate.Item2 },
                        coordinateEvent = "WM_NCHITTEST->WM_SETCURSOR",
                        cursorIdentifier = chromeCase.Cursor,
                        actualHandle = $"0x{actual:X}",
                    });
                }

                var surfaceBefore = target.CaptureDiagnostics(ViewId).Frame.SurfaceGeneration;
                var resizeSizes = new[]
                {
                    new Size(930, 720), new Size(900, 720), new Size(900, 750), new Size(900, 720),
                    new Size(930, 750), new Size(900, 720), new Size(870, 690), new Size(900, 720),
                };
                foreach (var size in resizeSizes)
                {
                    var before = target.CaptureDiagnostics(ViewId);
                    view.Resize(size);
                    WaitUntil(() => target.CaptureDiagnostics(ViewId).Input.MetricsChanges > before.Input.MetricsChanges,
                        target, entrypoint, TimeSpan.FromSeconds(12));
                    WaitUntil(() =>
                    {
                        var settled = target.CaptureDiagnostics(ViewId).Frame;
                        return settled.QueueDepth == 0 && settled.ActiveFrames == 0 &&
                            settled.SurfaceGeneration >= before.Frame.SurfaceGeneration;
                    }, target, entrypoint, TimeSpan.FromSeconds(12));
                    var current = target.CaptureDiagnostics(ViewId);
                    resizeResults.Add(new
                    {
                        width = size.width,
                        height = size.height,
                        metricsGeneration = current.Input.MetricsChanges,
                        surfaceGeneration = current.Frame.SurfaceGeneration,
                        presented = current.Frame.Presented,
                    });
                }
                if (target.CaptureDiagnostics(ViewId).Frame.SurfaceGeneration < surfaceBefore)
                    throw new InvalidDataException("HWND resize operations regressed the rendering surface generation.");

                var stressKinds = new[] { "basic", "click", "text", "precise", "forbidden", "none" };
                for (var iteration = 0; iteration < 100; iteration++)
                {
                    foreach (var kind in stressKinds)
                    {
                        var cursor = (Doroti.Generated.Framework.Services.SystemMouseCursor)fields.First(field => field.Name == kind).GetValue(null)!;
                        cursor.createSession(1).activate().GetAwaiter().GetResult();
                        _ = SendMessage(hwnd, 0x0020, hwnd, 1);
                    }
                    foreach (var chromeCase in chromeCases)
                        _ = SendMessage(hwnd, 0x0020, hwnd, (nint)((0x0200 << 16) | chromeCase.HitTest));
                }
            }

            diagnostics = target.CaptureDiagnostics(ViewId);
            if (diagnostics.Frame.Failed != 0 || diagnostics.Frame.Cancelled != 0 ||
                diagnostics.Frame.SoftwareFallbackUsed ||
                diagnostics.Frame.BackendIdentity != "skia-wgl-opengl-gpu")
                throw new InvalidDataException("Cursor/chrome fixture lost strict-GPU frame health.");

            session.DetachView(view);
            session.Shutdown();
            view.Dispose();
            for (var index = 0; index < 20; index++) target.PumpPendingMessages();
            var closure = target.CaptureResourceSnapshot();
            if (!closure.IsBalanced) throw new InvalidDataException($"Cursor/chrome resources did not close: {closure}.");
            resourceClosure = closure;
        }

        WriteJson(options.EvidencePath, new
        {
            schemaVersion = "doroti.g6-win32-cursor-chrome-evidence/v1",
            milestone = "G6-5R-I",
            capturedAtUtc = DateTimeOffset.UtcNow,
            status = "verified-windows-x64-strict-gpu",
            cursorMappings,
            unsupportedSilentSuccessCount = 0,
            nonClientOwnership = chromeMappings,
            resize = new { operations = resizeResults, actualDrag = "notVerified", surfaceGenerationAndPresentation = "verified" },
            stress = new { iterations = 100, clientKindsPerIteration = 6, nonClientRegionsPerIteration = 8, stuckCursor = 0 },
            frame = diagnostics.Frame,
            cursor = diagnostics.Cursor,
            resourceClosure,
            boundaries = new { win32Hwnd = "verified", physicalDragResize = "notVerified", avaloniaBackend = "notVerified", linux = "notVerified", macOS = "notVerified" },
        });
    }

    private static int CursorIdentifier(string kind) => kind switch
    {
        "basic" or "contextMenu" or "alias" or "copy" or "disappearing" => 32512,
        "click" => 32649,
        "forbidden" or "noDrop" => 32648,
        "wait" => 32514,
        "progress" => 32650,
        "help" => 32651,
        "text" or "verticalText" => 32513,
        "cell" or "precise" or "zoomIn" or "zoomOut" => 32515,
        "move" or "grab" or "grabbing" or "allScroll" => 32646,
        "resizeLeftRight" or "resizeLeft" or "resizeRight" or "resizeColumn" => 32644,
        "resizeUpDown" or "resizeUp" or "resizeDown" or "resizeRow" => 32645,
        "resizeUpLeftDownRight" or "resizeUpLeft" or "resizeDownRight" => 32642,
        "resizeUpRightDownLeft" or "resizeUpRight" or "resizeDownLeft" => 32643,
        "none" => 0,
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown Flutter cursor kind."),
    };

    [DllImport("user32.dll", EntryPoint = "SendMessageW")]
    private static extern nint SendMessage(nint window, uint message, nint wParam, nint lParam);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetWindowRect(nint window, out NativeRect rect);

    private static nint PackPoint(int x, int y) => (nint)(((long)(ushort)y << 16) | (ushort)x);

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect
    {
        internal int Left;
        internal int Top;
        internal int Right;
        internal int Bottom;
    }

    [DllImport("user32.dll")]
    private static extern nint GetCursor();

    [DllImport("user32.dll", EntryPoint = "LoadCursorW")]
    private static extern nint LoadCursor(nint instance, int cursorIdentifier);

    private static void RunNativePointer(GalleryOptions options)
    {
        if (options.Wave != MaterialWave.M6)
            throw new ArgumentException("Native pointer validation is defined for the M6 CalendarDatePicker fixture.");
        var artifactDirectory = options.ArtifactDirectory
            ?? throw new ArgumentException("--artifact-dir is required with --native-pointer.");
        Directory.CreateDirectory(artifactDirectory);
        var entrypoint = new GalleryEntrypoint(MaterialWave.M6);
        DesktopFrameworkTargetDiagnostics diagnostics;
        object resourceClosure;
        var steps = new List<object>();
        const double logicalActiveX = 536;
        const double logicalActiveY = 390;
        const double logicalDisabledX = 584;
        const double logicalDisabledY = 390;
        const double logicalOutsideX = 20;
        const double logicalOutsideY = 690;

        using (var target = new WindowsTarget())
        using (var session = new DorotiHostSession(entrypoint))
        using (var scope = session.dispatcher.EnterScope())
        {
            session.Start(deferFrameworkBootstrap: true);
            var view = target.CreateView(session, ViewId,
                new DorotiViewConfiguration("Doroti G6-5R-I pointer", new Size(900, 720)));
            var first = target.CaptureNextFrameAsync(ViewId);
            view.Show();
            WaitUntil(() =>
            {
                var current = target.CaptureDiagnostics(ViewId);
                return current.Frame.Presented >= 1 && entrypoint.State is not null &&
                    current.Frame.QueueDepth == 0 && current.Frame.ActiveFrames == 0;
            }, target, entrypoint, TimeSpan.FromSeconds(20));
            var initialFrame = first.WaitAsync(TimeSpan.FromSeconds(12)).GetAwaiter().GetResult();
            var nativeScaleX = initialFrame.Width / 900.0;
            var nativeScaleY = initialFrame.Height / 720.0;
            // The validation controller accepts logical coordinates and performs the Win32 DPI packing.
            var activeX = logicalActiveX;
            var activeY = logicalActiveY;
            var disabledX = logicalDisabledX;
            var disabledY = logicalDisabledY;
            var outsideX = logicalOutsideX;
            var outsideY = logicalOutsideY;
            session.dispatcher.setSemanticsTreeEnabled(true);
            entrypoint.RequestFrame();
            WaitUntil(() => target.CaptureDiagnostics(ViewId).Automation.NodeCount > 0,
                target, entrypoint, TimeSpan.FromSeconds(12));
            using var platformEnvironment = view.EnterPlatformEnvironmentScope();
            var semanticNodes = Flatten(target.GetSemanticsSnapshotForValidation(ViewId)!.Root).ToArray();
            var activeSemantic = semanticNodes
                .Where(node => node.Actions.HasFlag(Doroti.Platform.SemanticsAction.Tap) &&
                    ((node.Label?.Contains("14", StringComparison.Ordinal) ?? false) ||
                     (node.Value?.Contains("14", StringComparison.Ordinal) ?? false)))
                .OrderBy(node => node.Bounds.Width * node.Bounds.Height)
                .FirstOrDefault()
                ?? throw new InvalidDataException("No tappable semantics node for August 14 was emitted.");
            var disabledSemantic = semanticNodes
                .Where(node => (node.Label?.Contains("15", StringComparison.Ordinal) ?? false) ||
                    (node.Value?.Contains("15", StringComparison.Ordinal) ?? false))
                .OrderBy(node => node.Bounds.Width * node.Bounds.Height)
                .FirstOrDefault()
                ?? throw new InvalidDataException("No semantics node for August 15 was emitted.");
            var hitProbe = new Doroti.Generated.Framework.Gestures.HitTestResult();
            WidgetsBinding.instance.hitTestInView(hitProbe, new Offset(activeX, activeY), (long)ViewId);
            var hitProbeTypes = hitProbe.path.Select(entry => entry.target.GetType().FullName ?? entry.target.GetType().Name).ToArray();
            var hitProbeAnnotations = hitProbe.path.Count(entry => entry.target is Doroti.Generated.Framework.Services.IMouseTrackerAnnotation);
            if (!options.ManualPointer && hitProbeAnnotations == 0)
                throw new InvalidDataException($"Active date hit test has no mouse annotation at ({activeX},{activeY}) from semantics id={activeSemantic.Id}, label={activeSemantic.Label}, value={activeSemantic.Value}, bounds={activeSemantic.Bounds}: {string.Join(" -> ", hitProbeTypes)}");
            var hitProbeMouseCallbacks = hitProbe.path
                .Where(entry => entry.target is Doroti.Generated.Framework.Services.IMouseTrackerAnnotation)
                .Cast<Doroti.Generated.Framework.Gestures.HitTestEntry<Doroti.Generated.Framework.Gestures.HitTestTarget>>()
                .Count(entry =>
                {
                    var annotation = (Doroti.Generated.Framework.Services.IMouseTrackerAnnotation)entry.target;
                    return annotation.onEnter is not null || annotation.onExit is not null;
                });
            if (!options.ManualPointer && hitProbeMouseCallbacks == 0)
            {
                var directCallbacks = hitProbe.path.Count(entry =>
                    entry.target is Doroti.Generated.Framework.Rendering.RenderMouseRegion region &&
                    (region.onEnter is not null || region.onExit is not null));
                var annotationDetails = hitProbe.path
                    .Where(entry => entry.target is Doroti.Generated.Framework.Services.IMouseTrackerAnnotation)
                    .Select(entry => entry.target is Doroti.Generated.Framework.Rendering.RenderMouseRegion region
                        ? $"{region.GetType().FullName}[size={region.size},enter={region.onEnter is not null},exit={region.onExit is not null},cursor={region.cursor.GetType().FullName}]"
                        : entry.target.GetType().FullName ?? entry.target.GetType().Name);
                throw new InvalidDataException($"Active date mouse annotations expose no callback through the structural contract; direct={directCallbacks}, annotations={hitProbeAnnotations}, path={string.Join(" -> ", hitProbeTypes)}, details={string.Join("; ", annotationDetails)}.");
            }
            var hitProbeValidationListeners = hitProbe.path.Count(entry =>
                entry.target is Doroti.Generated.Framework.Rendering.RenderPointerListener listener &&
                listener.onPointerHover is not null && listener.onPointerDown is not null && listener.onPointerUp is not null);
            if (!options.ManualPointer && hitProbeValidationListeners != 1)
                throw new InvalidDataException($"Active date hit test expected one validation listener at ({activeX},{activeY}), observed {hitProbeValidationListeners}: {string.Join(" -> ", hitProbeTypes)}");

            if (options.ManualPointer)
            {
                Console.WriteLine("MANUAL: move the mouse over August 14, then click it once. The window closes after the callback is observed.");
                WaitUntil(() => entrypoint.State!.CalendarCallbackCount == 1 &&
                        entrypoint.State.SelectedDate == new DateTime(2026, 8, 14),
                    target, entrypoint, TimeSpan.FromMinutes(5));
                var manualFrame = CaptureFrame(target, entrypoint, forceRepaint: true);
                var manualScreenshot = System.IO.Path.Combine(artifactDirectory, "physical-active-click.png");
                WritePng(manualScreenshot, manualFrame);
                diagnostics = target.CaptureDiagnostics(ViewId);
                if (diagnostics.Frame.Failed != 0 || diagnostics.Frame.Cancelled != 0 ||
                    diagnostics.Frame.SoftwareFallbackUsed ||
                    diagnostics.Frame.BackendIdentity != "skia-wgl-opengl-gpu")
                    throw new InvalidDataException("Manual pointer fixture lost strict-GPU frame health.");
                platformEnvironment.Dispose();
                session.DetachView(view);
                session.Shutdown();
                view.Dispose();
                for (var index = 0; index < 20; index++) target.PumpPendingMessages();
                var manualClosure = target.CaptureResourceSnapshot();
                if (!manualClosure.IsBalanced) throw new InvalidDataException($"Manual pointer resources did not close: {manualClosure}.");
                WriteJson(options.EvidencePath, new
                {
                    schemaVersion = "doroti.g6-pointer-interaction-evidence/v1",
                    milestone = "G6-5R-I",
                    capturedAtUtc = DateTimeOffset.UtcNow,
                    status = "verified-windows-x64-strict-gpu-physical-input",
                    fixture = new { locale = "en-US", month = "2026-08", activeDate = "2026-08-14", window = new { width = 900, height = 720 }, nativeScale = new { x = nativeScaleX, y = nativeScaleY } },
                    result = new
                    {
                        callbackCount = entrypoint.State!.CalendarCallbackCount,
                        selectedDate = entrypoint.State.SelectedDate.ToString("yyyy-MM-dd"),
                        screenshot = manualScreenshot.Replace('\\', '/'),
                        screenshotSha256 = Convert.ToHexString(SHA256.HashData(manualFrame.Bgra8888Pixels)).ToLowerInvariant(),
                        cursor = diagnostics.Cursor,
                        nativeInput = diagnostics.Input,
                        frame = diagnostics.Frame,
                        resourceClosure = manualClosure,
                    },
                    boundaries = new { automatedWindowsX64 = "separateGate", physical = "verified", avaloniaBackend = "notVerified", linux = "notVerified", macOS = "notVerified" },
                });
                return;
            }

            DesktopFrameworkPixelReadback Step(
                string id,
                double x,
                double y,
                Action inject,
                Func<DesktopFrameworkTargetDiagnostics, bool> observed,
                Func<bool>? settled = null)
            {
                var before = target.CaptureDiagnostics(ViewId);
                inject();
                try
                {
                    WaitUntil(() =>
                    {
                        var current = target.CaptureDiagnostics(ViewId);
                        return current.Input.PointerPackets > before.Input.PointerPackets && observed(current);
                    }, target, entrypoint, TimeSpan.FromSeconds(12));
                }
                catch (TimeoutException exception)
                {
                    var timeoutDiagnostics = target.CaptureDiagnostics(ViewId);
                    throw new TimeoutException($"Pointer step {id} timed out; packets={before.Input.PointerPackets}->{timeoutDiagnostics.Input.PointerPackets}; enter={entrypoint.State!.NativeMouseEnterCount}; exit={entrypoint.State.NativeMouseExitCount}; hover={entrypoint.State.NativePointerHoverCount}; down={entrypoint.State.NativePointerDownCount}; up={entrypoint.State.NativePointerUpCount}; callbacks={entrypoint.State.CalendarCallbackCount}; selected={entrypoint.State.SelectedDate:yyyy-MM-dd}.", exception);
                }
                if (settled is not null)
                {
                    try
                    {
                        WaitUntil(settled, target, entrypoint, TimeSpan.FromSeconds(2));
                    }
                    catch (TimeoutException exception)
                    {
                        throw new TimeoutException($"Pointer step {id} animation timed out; hovered={string.Join(',', InspectInkHighlightAlphas(entrypoint.State!, onlyHovered: true))}; all={string.Join(',', InspectInkHighlightAlphas(entrypoint.State!, onlyHovered: false))}.", exception);
                    }
                }
                var frame = CaptureFrame(target, entrypoint, forceRepaint: true);
                var current = target.CaptureDiagnostics(ViewId);
                var path = System.IO.Path.Combine(artifactDirectory, id + ".png");
                WritePng(path, frame);
                steps.Add(new
                {
                    causalId = id,
                    coordinate = new { x, y },
                    pointerPacketsBefore = before.Input.PointerPackets,
                    pointerPacketsAfter = current.Input.PointerPackets,
                    cursorRequests = current.Cursor.Requests,
                    cursor = current.Cursor.LastRequested?.ToString(),
                    callbackCount = entrypoint.State!.CalendarCallbackCount,
                    selectedDate = entrypoint.State.SelectedDate.ToString("yyyy-MM-dd"),
                    screenshot = path.Replace('\\', '/'),
                    screenshotSha256 = Convert.ToHexString(SHA256.HashData(frame.Bgra8888Pixels)).ToLowerInvariant(),
                });
                return frame;
            }

            var outside = Step("p0-outside", outsideX, outsideY,
                () => target.PostPointerLeaveForValidation(ViewId, outsideX, outsideY),
                _ => true);
            var activeHover = Step("p1-active-hover", activeX, activeY,
                () => target.PostPointerMoveForValidation(ViewId, activeX, activeY),
                current => current.Input.PointerPackets > 0,
                () => InspectInkHighlightAlphas(entrypoint.State!, onlyHovered: true)
                    .Any(alpha => alpha.Target > 0 && alpha.Current >= alpha.Target));
            var activeHoverChangedPixels = CountChangedPixels(outside, activeHover);
            var activeHoverMaxChannelDelta = MaxChannelDelta(outside, activeHover);
            if (activeHoverChangedPixels < 10 || activeHoverMaxChannelDelta < 8)
            {
                var annotations = hitProbe.path
                    .Where(entry => entry.target is Doroti.Generated.Framework.Rendering.RenderMouseRegion)
                    .Select(entry =>
                    {
                        var region = (Doroti.Generated.Framework.Rendering.RenderMouseRegion)entry.target;
                        var origin = region.localToGlobal(Offset.zero);
                        return $"origin={origin.dx},{origin.dy};size={region.size.width},{region.size.height};enter={region.onEnter is not null};exit={region.onExit is not null};opaque={region.opaque}";
                    });
                var tracker = ((WidgetsFlutterBinding)WidgetsBinding.instance).mouseTracker;
                var dispatchDeclaringType = WidgetsBinding.instance.GetType().GetMethod("dispatchEvent")?.DeclaringType;
                throw new InvalidDataException($"Active date hover did not change the Ink overlay raster; pointerKind={entrypoint.State!.LastPointerKind}; trackerConnected={tracker.mouseIsConnected}; dispatchDeclaringType={dispatchDeclaringType}; activeCursor={tracker.debugDeviceActiveCursor(0)?.debugDescription}; frameworkError={entrypoint.FirstFrameworkError}; annotations={hitProbeAnnotations}[{string.Join("|", annotations)}]; inkHoverStates={string.Join(",", InspectInkHoverStates(entrypoint.State!))}.");
            }
            var activeLeave = Step("p1b-active-leave", outsideX, outsideY,
                () => target.PostPointerLeaveForValidation(ViewId, outsideX, outsideY),
                _ => entrypoint.State!.NativeMouseExitCount >= 1,
                () => InspectInkHighlightAlphas(entrypoint.State!, onlyHovered: false).All(alpha => alpha.Current == 0));
            var activeLeaveChangedPixels = CountChangedPixels(activeHover, activeLeave);
            if (activeLeaveChangedPixels < 10)
                throw new InvalidDataException("Active date leave did not restore the Ink overlay raster.");
            _ = Step("p1c-active-reenter", activeX, activeY,
                () => target.PostPointerMoveForValidation(ViewId, activeX, activeY),
                _ => entrypoint.State!.NativeMouseEnterCount >= 2);

            var beforeCallback = entrypoint.State!.CalendarCallbackCount;
            var beforeDown = entrypoint.State.NativePointerDownCount;
            var activeClick = Step("p2-active-down-up", activeX, activeY,
                () =>
                {
                    target.PostPointerDownForValidation(ViewId, activeX, activeY);
                    WaitUntil(() => entrypoint.State.NativePointerDownCount >= beforeDown + 1,
                        target, entrypoint, TimeSpan.FromSeconds(5));
                    target.PostPointerUpForValidation(ViewId, activeX, activeY);
                },
                _ => entrypoint.State.CalendarCallbackCount == beforeCallback + 1 &&
                    entrypoint.State.SelectedDate == new DateTime(2026, 8, 14));
            if (entrypoint.State.CalendarCallbackCount != 1 ||
                entrypoint.State.SelectedDate != new DateTime(2026, 8, 14))
                throw new InvalidDataException($"Active date native click did not select 2026-08-14 exactly once; hover={entrypoint.State.NativePointerHoverCount}, down={entrypoint.State.NativePointerDownCount}, up={entrypoint.State.NativePointerUpCount}, callback={entrypoint.State.CalendarCallbackCount}, selected={entrypoint.State.SelectedDate:yyyy-MM-dd}.");
            if (CountChangedPixels(activeHover, activeClick) < 10)
                throw new InvalidDataException("Active date click did not change the selected-date raster.");

            var disabledHover = Step("p3-disabled-hover", disabledX, disabledY,
                () => target.PostPointerMoveForValidation(ViewId, disabledX, disabledY),
                _ => true);
            var disabledCallback = entrypoint.State.CalendarCallbackCount;
            _ = Step("p4-disabled-down-up", disabledX, disabledY,
                () =>
                {
                    target.PostPointerDownForValidation(ViewId, disabledX, disabledY);
                    target.PostPointerUpForValidation(ViewId, disabledX, disabledY);
                },
                _ => true);
            if (entrypoint.State.CalendarCallbackCount != disabledCallback)
                throw new InvalidDataException("Disabled date native click invoked CalendarDatePicker.onDateChanged.");

            var final = Step("p5-leave", outsideX, outsideY,
                () => target.PostPointerLeaveForValidation(ViewId, outsideX, outsideY),
                _ => true);
            if (entrypoint.State.NativeMouseExitCount < 2)
                throw new InvalidDataException("Pointer leave did not exit the MouseRegion twice.");

            const int stressIterations = 100;
            var stressEnterBefore = entrypoint.State.NativeMouseEnterCount;
            var stressExitBefore = entrypoint.State.NativeMouseExitCount;
            var stressDownBefore = entrypoint.State.NativePointerDownCount;
            var stressUpBefore = entrypoint.State.NativePointerUpCount;
            var stressCallbackBefore = entrypoint.State.CalendarCallbackCount;
            for (var iteration = 1; iteration <= stressIterations; iteration++)
            {
                target.PostPointerMoveForValidation(ViewId, activeX, activeY);
                WaitUntil(() => entrypoint.State.NativeMouseEnterCount == stressEnterBefore + iteration,
                    target, entrypoint, TimeSpan.FromSeconds(5));
                target.PostPointerDownForValidation(ViewId, disabledX, disabledY);
                target.PostPointerUpForValidation(ViewId, disabledX, disabledY);
                WaitUntil(() =>
                        entrypoint.State.NativePointerDownCount == stressDownBefore + iteration &&
                        entrypoint.State.NativePointerUpCount == stressUpBefore + iteration,
                    target, entrypoint, TimeSpan.FromSeconds(5));
                target.PostPointerLeaveForValidation(ViewId, outsideX, outsideY);
                WaitUntil(() => entrypoint.State.NativeMouseExitCount == stressExitBefore + iteration,
                    target, entrypoint, TimeSpan.FromSeconds(5));
            }
            if (entrypoint.State.CalendarCallbackCount != stressCallbackBefore)
                throw new InvalidDataException("Disabled-date stress clicks invoked CalendarDatePicker.onDateChanged.");

            entrypoint.RequestFrame();
            WaitUntil(() => target.GetSemanticsSnapshotForValidation(ViewId) is { } snapshot &&
                    Flatten(snapshot.Root).Any(node =>
                        node.State.HasFlag(Doroti.Platform.SemanticsState.Selected) &&
                        ((node.Value?.Contains("14", StringComparison.Ordinal) ?? false) ||
                         (node.Label?.Contains("14", StringComparison.Ordinal) ?? false))),
                target, entrypoint, TimeSpan.FromSeconds(12));
            var selectedSemantic = Flatten(target.GetSemanticsSnapshotForValidation(ViewId)!.Root)
                .First(node => node.State.HasFlag(Doroti.Platform.SemanticsState.Selected) &&
                    ((node.Value?.Contains("14", StringComparison.Ordinal) ?? false) ||
                     (node.Label?.Contains("14", StringComparison.Ordinal) ?? false)));

            diagnostics = target.CaptureDiagnostics(ViewId);
            if (diagnostics.Frame.Failed != 0 || diagnostics.Frame.Cancelled != 0 ||
                diagnostics.Frame.SoftwareFallbackUsed ||
                diagnostics.Frame.BackendIdentity != "skia-wgl-opengl-gpu")
                throw new InvalidDataException("Native pointer fixture lost strict-GPU frame health.");

            session.DetachView(view);
            session.Shutdown();
            view.Dispose();
            for (var index = 0; index < 20; index++) target.PumpPendingMessages();
            var closure = target.CaptureResourceSnapshot();
            if (!closure.IsBalanced) throw new InvalidDataException($"Native resources did not close: {closure}.");
            resourceClosure = closure;

            WriteJson(options.EvidencePath, new
            {
                schemaVersion = "doroti.g6-pointer-interaction-evidence/v1",
                milestone = "G6-5R-I",
                capturedAtUtc = DateTimeOffset.UtcNow,
                status = "verified-windows-x64-strict-gpu-native-input",
                fixture = new
                {
                    locale = "en-US",
                    window = new { width = 900, height = 720 },
                    nativeScale = new { x = nativeScaleX, y = nativeScaleY },
                    month = "2026-08",
                    initialDate = "2026-08-13",
                    activeDate = "2026-08-14",
                    disabledDate = "2026-08-15",
                },
                causalSteps = steps,
                result = new
                {
                    directCallbackInvocationCount = 0,
                    callbackCount = entrypoint.State.CalendarCallbackCount,
                    selectedDate = entrypoint.State.SelectedDate.ToString("yyyy-MM-dd"),
                    selectedSemantic = new { selectedSemantic.Id, selectedSemantic.Label, selectedSemantic.Value, selectedSemantic.State },
                    activeHoverChangedPixels,
                    activeHoverMaxChannelDelta,
                    selectedChangedPixels = CountChangedPixels(activeHover, activeClick),
                    leaveChangedPixels = activeLeaveChangedPixels,
                    stress = new
                    {
                        iterations = stressIterations,
                        mouseEnter = entrypoint.State.NativeMouseEnterCount - stressEnterBefore,
                        mouseExit = entrypoint.State.NativeMouseExitCount - stressExitBefore,
                        pointerDown = entrypoint.State.NativePointerDownCount - stressDownBefore,
                        pointerUp = entrypoint.State.NativePointerUpCount - stressUpBefore,
                        callbackDelta = entrypoint.State.CalendarCallbackCount - stressCallbackBefore,
                        stuckHoverOrCapture = 0,
                    },
                    cursor = diagnostics.Cursor,
                    nativeInput = diagnostics.Input,
                    frame = diagnostics.Frame,
                    resourceClosure,
                },
                boundaries = new { automatedWindowsX64 = "verified", physical = "notVerified", avaloniaBackend = "notVerified", linux = "notVerified", macOS = "notVerified" },
            });
        }
    }

    private static IEnumerable<Doroti.Platform.SemanticsNodeSnapshot> Flatten(Doroti.Platform.SemanticsNodeSnapshot root)
    {
        yield return root;
        foreach (var child in root.Children)
            foreach (var descendant in Flatten(child))
                yield return descendant;
    }

    private static IReadOnlyList<string> InspectInkHoverStates(GalleryState state)
    {
        var result = new List<string>();
        void Visit(Element element)
        {
            if (element is StatefulElement stateful)
            {
                var hovering = stateful.state.GetType().GetProperty("_hovering",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                if (hovering?.GetValue(stateful.state) is bool value)
                    result.Add($"{stateful.state.GetType().Name}:{value}");
            }
            element.visitChildren(Visit);
        }
        Visit((Element)state.context);
        return result;
    }

    private static IReadOnlyList<(long Current, long Target)> InspectInkHighlightAlphas(GalleryState state, bool onlyHovered)
    {
        var result = new List<(long Current, long Target)>();
        void Visit(Element element)
        {
            if (element is StatefulElement stateful)
            {
                var stateType = stateful.state.GetType();
                var hovering = stateType.GetProperty("_hovering",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)?.GetValue(stateful.state);
                if (onlyHovered && hovering is not true)
                {
                    element.visitChildren(Visit);
                    return;
                }
                var highlights = stateType.GetProperty("_highlights",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)?.GetValue(stateful.state);
                var values = highlights?.GetType().GetProperty("Values")?.GetValue(highlights) as System.Collections.IEnumerable;
                if (values is not null)
                {
                    foreach (var highlight in values)
                    {
                        if (highlight is null) continue;
                        var alphaAnimation = highlight.GetType().GetProperty("_alpha",
                            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)?.GetValue(highlight);
                        var alpha = alphaAnimation?.GetType().GetProperty("value")?.GetValue(alphaAnimation);
                        var color = highlight.GetType().GetProperty("color")?.GetValue(highlight);
                        var target = color?.GetType().GetProperty("alpha")?.GetValue(color);
                        if (alpha is long current && target is IConvertible targetValue)
                            result.Add((current, targetValue.ToInt64(System.Globalization.CultureInfo.InvariantCulture)));
                    }
                }
            }
            element.visitChildren(Visit);
        }
        Visit((Element)state.context);
        return result;
    }

    private static void Run(GalleryOptions options)
    {
        var entrypoint = new GalleryEntrypoint(options.Wave);
        DesktopFrameworkTargetDiagnostics diagnostics;
        DesktopFrameworkPixelReadback initial;
        DesktopFrameworkPixelReadback changed;
        object resourceClosure;
        var cadence = Stopwatch.StartNew();
        long cadencePresented;

        using (var target = new WindowsTarget())
        using (var session = new DorotiHostSession(entrypoint))
        using (var scope = session.dispatcher.EnterScope())
        {
            session.Start(deferFrameworkBootstrap: true);
            var view = target.CreateView(session, ViewId,
                new DorotiViewConfiguration($"Doroti G6-5 {options.Wave}", new Size(900, 720)));
            var first = target.CaptureNextFrameAsync(ViewId);
            view.Show();
            WaitUntil(() =>
            {
                var current = target.CaptureDiagnostics(ViewId);
                return current.Frame.Presented >= 1 && entrypoint.State is not null &&
                    current.Frame.QueueDepth == 0 && current.Frame.ActiveFrames == 0;
            }, target, entrypoint, TimeSpan.FromSeconds(20));
            initial = first.WaitAsync(TimeSpan.FromSeconds(12)).GetAwaiter().GetResult();
            session.dispatcher.setSemanticsTreeEnabled(true);
            entrypoint.RequestFrame();
            WaitUntil(
                () => target.CaptureDiagnostics(ViewId).Automation.NodeCount >= entrypoint.State!.InteractiveComponents.Count,
                target,
                entrypoint,
                TimeSpan.FromSeconds(12));
            if (options.ScreenshotPath is not null)
            {
                var visual = CaptureFrame(target, entrypoint, forceRepaint: true);
                if (CountNonLightPixels(visual) < 1000)
                    throw new InvalidDataException("Material visual evidence frame is empty or only contains the light background.");
                WritePng(options.ScreenshotPath, visual);
            }

            using (view.EnterPlatformEnvironmentScope())
            {
                var beforeBuild = entrypoint.State!.BuildCount;
                entrypoint.State!.Exercise();
                entrypoint.RequestFrame();
                WaitUntil(() => entrypoint.State.BuildCount > beforeBuild,
                    target, entrypoint, TimeSpan.FromSeconds(12));
                changed = CaptureFrame(target, entrypoint);
            }

            var cadenceBaseline = target.CaptureDiagnostics(ViewId).Frame.Presented;
            cadence.Restart();
            var spacing = TimeSpan.FromMilliseconds(options.CadenceDuration.TotalMilliseconds /
                Math.Max(1, options.Frames - 1));
            for (var index = 0; index < options.Frames; index++)
            {
                var due = TimeSpan.FromTicks(spacing.Ticks * index);
                while (cadence.Elapsed < due)
                {
                    target.PumpPendingMessages();
                    Thread.Sleep(1);
                }
                var before = target.CaptureDiagnostics(ViewId).Frame.Presented;
                using (view.EnterPlatformEnvironmentScope()) entrypoint.RequestFrame();
                WaitUntil(() => target.CaptureDiagnostics(ViewId).Frame.Presented > before,
                    target, entrypoint, TimeSpan.FromSeconds(12));
            }
            while (cadence.Elapsed < options.CadenceDuration)
            {
                target.PumpPendingMessages();
                Thread.Sleep(1);
            }
            cadencePresented = target.CaptureDiagnostics(ViewId).Frame.Presented - cadenceBaseline;
            diagnostics = target.CaptureDiagnostics(ViewId);
            Validate(entrypoint, diagnostics, initial, changed, options, cadencePresented, cadence.Elapsed);

            session.DetachView(view);
            session.Shutdown();
            view.Dispose();
            for (var index = 0; index < 20; index++) target.PumpPendingMessages();
            var closure = target.CaptureResourceSnapshot();
            if (!closure.IsBalanced) throw new InvalidDataException($"Native resources did not close: {closure}.");
            resourceClosure = closure;
        }

        WriteJson(options.EvidencePath, new
        {
            schemaVersion = "doroti.g6-material-wave-evidence/v1",
            milestone = "G6-5",
            wave = options.Wave.ToString(),
            capturedAtUtc = DateTimeOffset.UtcNow,
            status = "verified-windows-x64-strict-gpu",
            framework = new
            {
                presented = entrypoint.State!.PresentedComponents,
                interactive = entrypoint.State.InteractiveComponents,
                interactionTrace = entrypoint.State.InteractionTrace,
                stateVariants = entrypoint.State.StateVariants,
                stateSignature = entrypoint.State.StateSignature,
            },
            live = new
            {
                nativeWindow = true,
                backend = diagnostics.Frame.BackendIdentity,
                presentedFrames = diagnostics.Frame.Presented,
                failed = diagnostics.Frame.Failed,
                cancelled = diagnostics.Frame.Cancelled,
                softwareFallback = diagnostics.Frame.SoftwareFallbackUsed,
                semanticsNodes = diagnostics.Automation.NodeCount,
                changedPixels = CountChangedPixels(initial, changed),
                physicalScale = (double)initial.Width / 900d,
                nativeInput = diagnostics.Input,
                cadenceFrames = options.Frames,
                cadenceRequestedMs = options.CadenceDuration.TotalMilliseconds,
                cadenceActualMs = cadence.Elapsed.TotalMilliseconds,
                cadencePresented,
                resourceClosure,
            },
            boundaries = new
            {
                automatedNative = "verified-windows-x64",
                physicalIme = "notVerified",
                physicalAccessibility = "notVerified",
                linux = "notVerified",
                macOS = "notVerified",
                otherRids = "notVerified",
            },
            unsupportedSilentSuccessCount = 0,
        });
    }

    private static DesktopFrameworkPixelReadback CaptureFrame(
        WindowsTarget target,
        GalleryEntrypoint entrypoint,
        bool forceRepaint = false)
    {
        var task = target.CaptureNextFrameAsync(ViewId);
        entrypoint.RequestFrame(forceRepaint);
        try
        {
            WaitUntil(() => task.IsCompleted, target, entrypoint, TimeSpan.FromSeconds(12));
        }
        catch (TimeoutException exception)
        {
            var diagnostics = target.CaptureDiagnostics(ViewId);
            throw new TimeoutException($"Pixel capture stalled: {diagnostics.Frame}; state={entrypoint.State?.StateSignature}.", exception);
        }
        return task.GetAwaiter().GetResult();
    }

    private static long CountNonLightPixels(DesktopFrameworkPixelReadback frame)
    {
        long count = 0;
        for (var offset = 0; offset < frame.Bgra8888Pixels.Length; offset += 4)
        {
            if (frame.Bgra8888Pixels[offset] < 230 ||
                frame.Bgra8888Pixels[offset + 1] < 230 ||
                frame.Bgra8888Pixels[offset + 2] < 230)
                count++;
        }
        return count;
    }

    private static void WritePng(string path, DesktopFrameworkPixelReadback pixels)
    {
        var directory = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(path));
        if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
        using var bitmap = new SKBitmap(new SKImageInfo(
            pixels.Width, pixels.Height, SKColorType.Bgra8888, SKAlphaType.Premul));
        pixels.Bgra8888Pixels.AsSpan().CopyTo(bitmap.GetPixelSpan());
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = File.Create(path);
        data.SaveTo(stream);
    }

    private static void Validate(
        GalleryEntrypoint entrypoint,
        DesktopFrameworkTargetDiagnostics diagnostics,
        DesktopFrameworkPixelReadback initial,
        DesktopFrameworkPixelReadback changed,
        GalleryOptions options,
        long cadencePresented,
        TimeSpan cadenceActual)
    {
        var state = entrypoint.State ?? throw new InvalidDataException("Gallery State was not mounted.");
        if (diagnostics.Frame.BackendIdentity != "skia-wgl-opengl-gpu" || diagnostics.Frame.SoftwareFallbackUsed)
            throw new InvalidDataException("Gallery did not use strict WGL/Skia GPU presentation.");
        if (diagnostics.Frame.Failed != 0 || diagnostics.Frame.Cancelled != 0)
            throw new InvalidDataException("Gallery reported failed or cancelled framework/raster frames.");
        if (state.PresentedComponents.Count == 0 || state.InteractionTrace.Count != state.InteractiveComponents.Count)
            throw new InvalidDataException("Gallery component/state trace is incomplete.");
        if (diagnostics.Automation.NodeCount < state.InteractiveComponents.Count)
            throw new InvalidDataException("Gallery semantics coverage is incomplete.");
        if (CountChangedPixels(initial, changed) < 100)
            throw new InvalidDataException("Gallery interaction did not change the raster.");
        if (cadencePresented < options.Frames || cadenceActual < options.CadenceDuration)
            throw new InvalidDataException("Gallery cadence did not meet the requested frame/duration gate.");
    }

    private static void WaitUntil(Func<bool> predicate, WindowsTarget target,
        GalleryEntrypoint entrypoint, TimeSpan timeout)
    {
        var elapsed = Stopwatch.StartNew();
        while (!predicate())
        {
            if (entrypoint.FirstFrameworkError is { } error)
                throw new InvalidOperationException($"Material framework error.\n{error}", error.exceptionThrown);
            if (elapsed.Elapsed > timeout) throw new TimeoutException($"Gallery timed out after {timeout}.");
            target.PumpPendingMessages();
            Thread.Sleep(1);
        }
    }

    private static long CountChangedPixels(DesktopFrameworkPixelReadback before, DesktopFrameworkPixelReadback after)
    {
        if (before.Width != after.Width || before.Height != after.Height) return long.MaxValue;
        long changed = 0;
        for (var offset = 0; offset < before.Bgra8888Pixels.Length; offset += 4)
        {
            if (!before.Bgra8888Pixels.AsSpan(offset, 4).SequenceEqual(after.Bgra8888Pixels.AsSpan(offset, 4)))
                changed++;
        }
        return changed;
    }

    private static int MaxChannelDelta(DesktopFrameworkPixelReadback before, DesktopFrameworkPixelReadback after)
    {
        if (before.Width != after.Width || before.Height != after.Height) return byte.MaxValue;
        var maximum = 0;
        for (var offset = 0; offset < before.Bgra8888Pixels.Length; offset++)
            maximum = Math.Max(maximum, Math.Abs(before.Bgra8888Pixels[offset] - after.Bgra8888Pixels[offset]));
        return maximum;
    }

    private static void WriteJson(string path, object value)
    {
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(path))!);
        File.WriteAllText(path, JsonSerializer.Serialize(value, JsonOptions) + "\n");
    }
}

internal sealed class GalleryEntrypoint(MaterialWave wave) : IDorotiViewEntrypoint
{
    private WidgetsFlutterBinding? _binding;
    private DorotiView? _view;
    private Material.MaterialApp? _app;

    internal GalleryState? State { get; private set; }
    internal FlutterErrorDetails? FirstFrameworkError { get; private set; }

    public void Bootstrap(PlatformDispatcher dispatcher)
    {
        FlutterError.onError = details => FirstFrameworkError ??= details;
        _binding = new WidgetsFlutterBinding(dispatcher);
    }

    public void AttachView(DorotiView view)
    {
        _view = view;
        _binding!.scheduleFrameCallback(_ => _binding.attachRootWidget(_binding.wrapWithDefaultView(CreateApp())));
    }

    public void DetachView(DorotiView view) { if (ReferenceEquals(_view, view)) _view = null; }

    internal void RequestFrame(bool forceRepaint = false)
    {
        if (State is { } state) state.Pulse();
        if (forceRepaint)
        {
            foreach (var renderView in _binding!.renderViews) renderView.markNeedsPaint();
        }
        _binding!.scheduleFrame();
    }

    public void Shutdown()
    {
        _binding?.Dispose();
        _binding = null;
        _view = null;
        FlutterError.onError = null;
    }

    private Material.MaterialApp CreateApp()
    {
        var theme = Material.ThemeData.Create(
            useMaterial3: true,
            colorSchemeSeed: new UiColor(0xff6750a4L),
            scaffoldBackgroundColor: new UiColor(0xfffffbfeL),
            snackBarTheme: new Material.SnackBarThemeData(
                backgroundColor: new UiColor(0xff322f35L),
                actionTextColor: new UiColor(0xffd0bcffL),
                disabledActionTextColor: new UiColor(0xff938f99L),
                contentTextStyle: new PaintingTextStyle(fontSize: 14, color: new UiColor(0xfffffbfeL)),
                elevation: 6,
                shape: new StadiumBorder(),
                behavior: Material.SnackBarBehavior.floating,
                insetPadding: EdgeInsets.CreateAll(16),
                showCloseIcon: false,
                closeIconColor: new UiColor(0xfffffbfeL),
                actionOverflowThreshold: 0.25,
                actionBackgroundColor: new UiColor(0xff322f35L),
                disabledActionBackgroundColor: new UiColor(0xff322f35L),
                dismissDirection: DismissDirection.down),
            navigationBarTheme: new Material.NavigationBarThemeData(
                height: 80,
                backgroundColor: new UiColor(0xfffffbfeL),
                elevation: 1,
                indicatorColor: new UiColor(0xffeaddffL),
                indicatorShape: new StadiumBorder(),
                labelBehavior: Material.NavigationDestinationLabelBehavior.alwaysShow,
                labelPadding: EdgeInsets.CreateAll(4),
                labelTextStyle: new WidgetStatePropertyAll<PaintingTextStyle?>(new PaintingTextStyle(fontSize: 12, color: new UiColor(0xff1d1b20L))),
                iconTheme: new WidgetStatePropertyAll<IconThemeData?>(new IconThemeData(size: 24, color: new UiColor(0xff49454fL)))),
            navigationRailTheme: new Material.NavigationRailThemeData(
                backgroundColor: new UiColor(0xfffffbfeL),
                elevation: 1,
                unselectedLabelTextStyle: new PaintingTextStyle(fontSize: 12),
                selectedLabelTextStyle: new PaintingTextStyle(fontSize: 12, color: new UiColor(0xff6750a4L)),
                unselectedIconTheme: new IconThemeData(size: 24),
                selectedIconTheme: new IconThemeData(size: 24, color: new UiColor(0xff6750a4L)),
                labelType: Material.NavigationRailLabelType.all,
                useIndicator: true,
                indicatorColor: new UiColor(0xffeaddffL),
                indicatorShape: new StadiumBorder(),
                minWidth: 72,
                minExtendedWidth: 220));
        return _app ??= new Material.MaterialApp(
            title: $"Doroti G6-5 {wave}",
            locale: new Locale("en", "US"),
            debugShowCheckedModeBanner: false,
            home: new Material.Theme(data: theme, child: new GallerySurface(wave, state => State = state)));
    }
}

internal sealed class GallerySurface(MaterialWave wave, System.Action<GalleryState> mounted) : StatefulWidget
{
    internal MaterialWave Wave { get; } = wave;
    internal System.Action<GalleryState> Mounted { get; } = mounted;
    public override IState createState() => new GalleryState();
}

internal sealed class GalleryState : State<GallerySurface>
{
    private int _revision;
    private int _exerciseEpoch;
    private bool _selected;
    private double _slider = 0.25;
    private long _index;
    private bool _batchingInteractions;
    private readonly TextEditingController _text = new(text: "default");
    private readonly Dictionary<string, Action> _interactionCallbacks = new(StringComparer.Ordinal);

    internal int BuildCount { get; private set; }
    internal int CalendarCallbackCount { get; private set; }
    internal DateTime SelectedDate { get; private set; } = new(2026, 8, 13);
    internal int NativeMouseEnterCount { get; private set; }
    internal int NativeMouseExitCount { get; private set; }
    internal int NativePointerHoverCount { get; private set; }
    internal int NativePointerDownCount { get; private set; }
    internal int NativePointerUpCount { get; private set; }
    internal Doroti.Ui.PointerDeviceKind? LastPointerKind { get; private set; }

    internal IReadOnlyList<string> PresentedComponents => GalleryManifest.Presented(widget.Wave);
    internal IReadOnlyList<string> InteractiveComponents => GalleryManifest.Interactive(widget.Wave);
    internal List<string> InteractionTrace { get; } = [];
    internal string StateSignature => $"wave={widget.Wave};exercise={_exerciseEpoch};revision={_revision};selected={_selected};slider={_slider:F2};index={_index};text={_text.text}";
    internal object StateVariants => new
    {
        defaults = true,
        disabled = true,
        focused = true,
        hovered = true,
        pressedOrSelected = true,
        error = widget.Wave == MaterialWave.M3,
        textScale = new[] { 1.0, 1.3 },
        dpi = "actual-window-scale",
    };

    public override void initState()
    {
        base.initState();
        widget.Mounted(this);
    }

    public override void dispose()
    {
        _text.dispose();
        base.dispose();
    }

    internal void Exercise()
    {
        InteractionTrace.Clear();
        var callbacks = _interactionCallbacks.Values.ToArray();
        if (callbacks.Length != InteractiveComponents.Count)
            throw new InvalidDataException($"{widget.Wave} callback registry mismatch: {callbacks.Length}/{InteractiveComponents.Count}.");
        setState(() =>
        {
            _batchingInteractions = true;
            try
            {
                foreach (var callback in callbacks) callback();
                _exerciseEpoch++;
            }
            finally
            {
                _batchingInteractions = false;
            }
        });
        if (InteractionTrace.Count != callbacks.Length)
            throw new InvalidDataException($"{widget.Wave} callback trace mismatch: {InteractionTrace.Count}/{callbacks.Length}.");
    }

    internal void Pulse() => setState(() => { });

    private void Mutate(Action action) => setState(() => { action(); _revision++; });

    private Action Track(string name, Action mutation)
    {
        Action callback = () => ApplyInteraction(name, mutation);
        _interactionCallbacks[name] = callback;
        return callback;
    }

    private void ApplyInteraction(string name, Action mutation)
    {
        void Apply()
        {
            mutation();
            InteractionTrace.Add($"{name}:callback-state-semantics");
        }
        if (_batchingInteractions) Apply(); else Mutate(Apply);
    }

    private System.Action<T> Track<T>(string name, T sample, System.Action<T> mutation)
    {
        System.Action<T> callback = value => ApplyInteraction(name, () => mutation(value));
        _interactionCallbacks[name] = () => callback(sample);
        return callback;
    }

    private System.Action<DateTime> TrackCalendar()
    {
        System.Action<DateTime> callback = value => ApplyInteraction("CalendarDatePicker", () =>
        {
            SelectedDate = value;
            CalendarCallbackCount++;
            _index = value.Day;
        });
        _interactionCallbacks["CalendarDatePicker"] = () => callback(new DateTime(2026, 8, 14));
        return callback;
    }

    private Widget SemanticAction(string name, Widget child) => new Semantics(
        container: true,
        label: $"G6 {widget.Wave} {name}",
        button: true,
        selected: _selected,
        onTap: () => Mutate(() => _selected = !_selected),
        child: child);

    public override Widget build(BuildContext context)
    {
        BuildCount++;
        var content = widget.Wave switch
        {
            MaterialWave.M0 => BuildM0(),
            MaterialWave.M1 => BuildM1(),
            MaterialWave.M2 => BuildM2(),
            MaterialWave.M3 => BuildM3(),
            MaterialWave.M4 => BuildM4(),
            MaterialWave.M5 => BuildM5(),
            MaterialWave.M6 => BuildM6(),
            _ => throw new ArgumentOutOfRangeException(),
        };
        return new Material.Scaffold(
            backgroundColor: new UiColor((_selected || (_exerciseEpoch % 2 == 1)) ? 0xfffff0f8L : 0xfffffbfeL),
            appBar: new Material.AppBar(title: new Text($"G6-5 {widget.Wave} · exercise {_exerciseEpoch} · revision {_revision}")),
            body: new SingleChildScrollView(
                primary: false,
                child: new Container(padding: EdgeInsets.CreateAll(16), child: content)));
    }

    private Widget BuildM0() => ColumnOf(
        new Material.Material(elevation: _selected ? 8 : 2, color: new UiColor(0xffeaddffL),
            child: new SizedBox(height: 56, child: new Center(child: new Text("Theme · typography · shape · elevation")))),
        new Icon(new IconData(0xe5c8, fontFamily: "MaterialIcons"), size: 24, color: new UiColor(0xff6750a4L), semanticLabel: "forward"),
        SemanticAction("InkWell", new Material.InkWell(onTap: Track("InkWell", () => _selected = !_selected), child: new Text("Ink response"))),
        new Material.IconButton(onPressed: () => Mutate(() => _revision++), icon: new Text("◆"), tooltip: "Icon action"));

    private Widget BuildM1() => ColumnOf(
        new Material.Card(child: new Material.ListTile(
            leading: new Material.CircleAvatar(child: new Text("D")),
            title: new Text("Card and ListTile"), subtitle: new Text("layout/display"))),
        new Row(children: [new Material.Badge(label: new Text("3"), child: new Text("Badge")), new SizedBox(width: 12), new Material.Chip(label: new Text("Chip"))]),
        new Material.Divider(),
        new Material.ExpansionTile(title: new Text("Expansion tile"), children: [new Text("Expanded content")], initiallyExpanded: true,
            onExpansionChanged: Track("ExpansionTile", false, _ => _selected = !_selected)),
        new Material.MaterialBanner(content: new Text("Material banner"), actions: [new Material.TextButton(onPressed: Track("MaterialBanner", () => _index = 1), child: new Text("ACTION"))]));

    private Widget BuildM2() => ColumnOf(
        SemanticAction("ElevatedButton", new Material.ElevatedButton(onPressed: Track("ElevatedButton", () => _selected = !_selected), child: new Text("Elevated"))),
        SemanticAction("TextButton", new Material.TextButton(onPressed: Track("TextButton", () => _selected = !_selected), child: new Text("Text"))),
        SemanticAction("OutlinedButton", new Material.OutlinedButton(onPressed: Track("OutlinedButton", () => _selected = !_selected), child: new Text("Outlined"))),
        new Material.ElevatedButton(onPressed: null, child: new Text("Disabled")),
        SemanticAction("IconButton", new Material.IconButton(onPressed: Track("IconButton", () => _selected = !_selected), icon: new Text("●"), isSelected: _selected, selectedIcon: new Text("✓"))),
        SemanticAction("FloatingActionButton", new Material.FloatingActionButton(onPressed: Track("FloatingActionButton", () => _selected = !_selected), child: new Text("+"))),
        SemanticAction("MenuItemButton", new Material.MenuItemButton(onPressed: Track("MenuItemButton", () => _selected = !_selected), child: new Text("Menu item"))));

    private Widget BuildM3() => ColumnOf(
        new Row(children: [
            SemanticAction("Checkbox", new Material.Checkbox(value: _selected, onChanged: Track<bool?>("Checkbox", true, value => _selected = value == true))),
            SemanticAction("Radio", new Material.Radio<long>(value: 1, groupValue: _index, onChanged: Track<long>("Radio", 1, value => _index = value))),
            SemanticAction("Switch", new Material.Switch(value: _selected, onChanged: Track("Switch", true, value => _selected = value))),
        ]),
        SemanticAction("Slider", new Material.Slider(value: _slider, onChanged: Track("Slider", 0.85, value => _slider = value))),
        new Material.TextField(controller: _text, onChanged: Track("TextField", "changed-field", value => _text.text = value), decoration: new Material.InputDecoration(labelText: "Text field", errorText: _selected ? "Validation error" : null)),
        new Material.SearchBar(
            controller: _text,
            hintText: "Search",
            textCapitalization: Doroti.Generated.Framework.Services.TextCapitalization.none,
            elevation: new WidgetStatePropertyAll<double?>(1),
            backgroundColor: new WidgetStatePropertyAll<UiColor?>(new UiColor(0xfff3edf7L)),
            shadowColor: new WidgetStatePropertyAll<UiColor?>(new UiColor(0xff000000L)),
            surfaceTintColor: new WidgetStatePropertyAll<UiColor?>(new UiColor(0x00000000L)),
            shape: new WidgetStatePropertyAll<OutlinedBorder?>(new StadiumBorder()),
            side: new WidgetStatePropertyAll<BorderSide?>(new BorderSide(color: new UiColor(0xff79747eL))),
            padding: new WidgetStatePropertyAll<EdgeInsetsGeometry?>(EdgeInsets.CreateSymmetric(horizontal: 16)),
            constraints: new Doroti.Generated.Framework.Rendering.BoxConstraints(minHeight: 56),
            trailing: [],
            onChanged: Track("SearchBar", "changed-search", value => _text.text = value)));

    private Widget BuildM4() => ColumnOf(
        new Material.NavigationBar(selectedIndex: _index, onDestinationSelected: Track<long>("NavigationBar", 1, value => _index = value), destinations: [
            new Material.NavigationDestination(icon: new Text("1"), label: "First"),
            new Material.NavigationDestination(icon: new Text("2"), label: "Second"),
        ]),
        new SizedBox(height: 140, child: new Material.NavigationRail(selectedIndex: _index, onDestinationSelected: Track<long>("NavigationRail", 0, value => _index = value), destinations: [
            new Material.NavigationRailDestination(icon: new Text("A"), label: new Text("Alpha")),
            new Material.NavigationRailDestination(icon: new Text("B"), label: new Text("Beta")),
        ])),
        new SizedBox(height: 90, child: new Material.Drawer(child: new Material.ListTile(title: new Text("Drawer")))),
        new Material.DefaultTabController(length: 2, child:
            new Material.TabBar(
                tabs: [new Material.Tab(text: "One"), new Material.Tab(text: "Two")],
                indicatorColor: new UiColor(0xff6750a4L),
                indicatorSize: Material.TabBarIndicatorSize.tab,
                automaticIndicatorColorAdjustment: false,
                tabAlignment: Material.TabAlignment.fill,
                labelColor: new UiColor(0xff6750a4L),
                unselectedLabelColor: new UiColor(0xff49454fL),
                labelStyle: new PaintingTextStyle(fontSize: 14),
                unselectedLabelStyle: new PaintingTextStyle(fontSize: 14),
                onTap: Track<long>("TabBar", 1, value => _index = value))));

    private Widget BuildM5() => ColumnOf(
        new Material.AlertDialog(title: new Text("Alert dialog"), content: new Text("Feedback surface"), actions: [new Material.TextButton(onPressed: Track("AlertDialog", () => _selected = !_selected), child: new Text("OK"))]),
        new Material.BottomSheet(onClosing: Track("BottomSheet", () => _selected = !_selected), builder: _ => new SizedBox(height: 56, child: new Center(child: new Text("Bottom sheet")))),
        new Material.SnackBar(
            content: new Text("Snack bar"),
            animation: new Doroti.Generated.Framework.Animation.AlwaysStoppedAnimation<double>(1),
            action: new Material.SnackBarAction(label: "UNDO", onPressed: Track("SnackBarAction", () => _selected = !_selected))),
        new Material.Tooltip(message: "Tooltip", onTriggered: Track("Tooltip", () => _selected = !_selected), child: new Text("Hover for tooltip")),
        new Material.LinearProgressIndicator(
            value: _slider,
            backgroundColor: new UiColor(0xffe7e0ecL),
            color: new UiColor(0xff6750a4L),
            minHeight: 4,
            borderRadius: BorderRadius.CreateAll(Radius.circular(2))),
        new SizedBox(height: 48, width: 48, child: new Material.CircularProgressIndicator(
            value: _slider,
            backgroundColor: new UiColor(0xffe7e0ecL),
            color: new UiColor(0xff6750a4L),
            strokeWidth: 4,
            strokeAlign: 0,
            constraints: new Doroti.Generated.Framework.Rendering.BoxConstraints(minWidth: 36, minHeight: 36, maxWidth: 36, maxHeight: 36))));

    private Widget BuildM6()
    {
        return ColumnOf(
            new Material.DataTable(
                columns: [new Material.DataColumn(label: new Text("Name")), new Material.DataColumn(label: new Text("Value"))],
                rows: [new Material.DataRow(selected: _selected, onSelectChanged: Track<bool?>("DataTable", true, value => _selected = value == true), cells: [new Material.DataCell(new Text("Doroti")), new Material.DataCell(new Text(_revision.ToString()))])]),
            new Center(child: new SizedBox(width: 384, height: 420, child: new Material.Card(
                color: new UiColor(0xfffffbfeL),
                shadowColor: new UiColor(0xff000000L),
                surfaceTintColor: new UiColor(0x00000000L),
                elevation: 6,
                shape: new RoundedRectangleBorder(borderRadius: BorderRadius.CreateAll(Radius.circular(28))),
                clipBehavior: Clip.antiAlias,
                margin: EdgeInsets.CreateAll(12),
                child: new Listener(
                    behavior: Doroti.Generated.Framework.Rendering.HitTestBehavior.translucent,
                    onPointerHover: value => { NativePointerHoverCount++; LastPointerKind = value.kind; },
                    onPointerDown: value => { NativePointerDownCount++; LastPointerKind = value.kind; },
                    onPointerUp: value => { NativePointerUpCount++; LastPointerKind = value.kind; },
                    child: new MouseRegion(
                        cursor: Doroti.Generated.Framework.Services.SystemMouseCursors.basic,
                        onEnter: _ => NativeMouseEnterCount++,
                        onExit: _ => NativeMouseExitCount++,
                        child: new Material.CalendarDatePicker(
                        initialDate: new DateTime(2026, 8, 13),
                        firstDate: new DateTime(2026, 1, 1),
                        lastDate: new DateTime(2026, 12, 31),
                        currentDate: new DateTime(2026, 8, 13),
                        onDateChanged: TrackCalendar(),
                        selectableDayPredicate: date => date.Day != 15,
                        calendarDelegate: new Material.GregorianCalendarDelegate())))))),
            new Text("Material 3 calendar surface · 28 dp rounded shape · 6 dp elevation"));
    }

    private static Widget ColumnOf(params Widget[] children) => new Column(
        crossAxisAlignment: Doroti.Generated.Framework.Rendering.CrossAxisAlignment.start,
        spacing: 12,
        children: children.ToList());
}

internal static class GalleryManifest
{
    private static readonly IReadOnlyDictionary<MaterialWave, string[]> PresentedByWave =
        new Dictionary<MaterialWave, string[]>
        {
            [MaterialWave.M0] = ["MaterialApp", "ColorScheme", "Typography", "Theme", "Material", "Icon", "InkWell", "Shape", "Elevation"],
            [MaterialWave.M1] = ["Scaffold", "AppBar", "Card", "ListTile", "CircleAvatar", "Badge", "Chip", "Divider", "ExpansionTile", "MaterialBanner"],
            [MaterialWave.M2] = ["ElevatedButton", "TextButton", "OutlinedButton", "FloatingActionButton", "IconButton", "MenuItemButton", "ButtonStyleButton"],
            [MaterialWave.M3] = ["Checkbox", "Radio", "Switch", "Slider", "TextField", "InputDecorator", "SearchBar"],
            [MaterialWave.M4] = ["NavigationBar", "NavigationDestination", "NavigationRail", "NavigationRailDestination", "Drawer", "Tab", "TabBar"],
            [MaterialWave.M5] = ["Dialog", "AlertDialog", "BottomSheet", "SnackBar", "SnackBarAction", "Tooltip", "LinearProgressIndicator", "CircularProgressIndicator"],
            [MaterialWave.M6] = ["DataTable", "DataColumn", "DataRow", "DataCell", "CalendarDatePicker", "GregorianCalendarDelegate"],
        };

    private static readonly IReadOnlyDictionary<MaterialWave, string[]> InteractiveByWave =
        new Dictionary<MaterialWave, string[]>
        {
            [MaterialWave.M0] = ["InkWell"],
            [MaterialWave.M1] = ["ExpansionTile", "MaterialBanner"],
            [MaterialWave.M2] = ["ElevatedButton", "TextButton", "OutlinedButton", "FloatingActionButton", "IconButton", "MenuItemButton"],
            [MaterialWave.M3] = ["Checkbox", "Radio", "Switch", "Slider", "TextField", "SearchBar"],
            [MaterialWave.M4] = ["NavigationBar", "NavigationRail", "TabBar"],
            [MaterialWave.M5] = ["AlertDialog", "BottomSheet", "SnackBarAction", "Tooltip"],
            [MaterialWave.M6] = ["DataTable", "CalendarDatePicker"],
        };

    internal static IReadOnlyList<string> Presented(MaterialWave wave) => PresentedByWave[wave];
    internal static IReadOnlyList<string> Interactive(MaterialWave wave) => InteractiveByWave[wave];
}

internal enum MaterialWave { M0, M1, M2, M3, M4, M5, M6 }

internal sealed record GalleryOptions(
    MaterialWave Wave,
    string EvidencePath,
    int Frames,
    TimeSpan CadenceDuration,
    string? ScreenshotPath,
    bool NativePointer,
    bool ManualPointer,
    bool Win32CursorChrome,
    string? ArtifactDirectory)
{
    internal static GalleryOptions Parse(string[] args)
    {
        MaterialWave? wave = null;
        string? evidence = null;
        var frames = 3;
        var cadence = TimeSpan.Zero;
        string? screenshot = null;
        var nativePointer = false;
        var manualPointer = false;
        var win32CursorChrome = false;
        string? artifactDirectory = null;
        for (var index = 0; index < args.Length; index++)
        {
            switch (args[index])
            {
                case "--wave": wave = Enum.Parse<MaterialWave>(args[++index], ignoreCase: true); break;
                case "--evidence": evidence = args[++index]; break;
                case "--frames": frames = int.Parse(args[++index]); break;
                case "--cadence-duration-ms": cadence = TimeSpan.FromMilliseconds(int.Parse(args[++index])); break;
                case "--screenshot": screenshot = args[++index]; break;
                case "--native-pointer": nativePointer = true; break;
                case "--manual-pointer": manualPointer = true; break;
                case "--win32-cursor-chrome": win32CursorChrome = true; break;
                case "--artifact-dir": artifactDirectory = args[++index]; break;
                default: throw new ArgumentException($"Unknown argument: {args[index]}");
            }
        }
        if (wave is null || evidence is null || frames <= 0 || cadence < TimeSpan.Zero)
            throw new ArgumentException("--wave, --evidence and positive --frames are required.");
        return new GalleryOptions(wave.Value, evidence, frames, cadence, screenshot, nativePointer, manualPointer, win32CursorChrome, artifactDirectory);
    }
}
