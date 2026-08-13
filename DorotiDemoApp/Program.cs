using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Doroti.Flutter.Hosting;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using Doroti.Generated.Framework.Foundation;
using Doroti.Generated.Framework.Painting;
using Doroti.Generated.Framework.Widgets;
using Doroti.Host.Desktop.Flutter;
using Doroti.Target.Windows;
using Material = Doroti.Generated.Framework.Material;
using IOPath = System.IO.Path;
using UiColor = Doroti.Flutter.Ui.Color;

internal static class Program
{
    private const ulong DemoViewId = 580;
    private const string FlutterRevision = "56b8e1a851a594b1a154f8ea93270807dab22b9a";
    private const string AvaloniaRevision = "f159423f691946e713f454447a780d4677d8a0d2";

    [STAThread]
    public static int Main(string[] args)
    {
        var options = DemoOptions.Parse(args);
        if (!OperatingSystem.IsWindows())
        {
            return FailBeforeTarget(options, new PlatformNotSupportedException(
                "DorotiDemoApp currently requires the G5-6W Windows target package."));
        }

        return Run(options);
    }

    private static int Run(DemoOptions options)
    {
        var entrypoint = new MaterialDemoEntrypoint(options.EntryMode, options.RequireExternalUia);
        DesktopFlutterTargetDiagnostics? diagnostics = null;
        DesktopFlutterPixelReadback? readback = null;
        Exception? unhandled = null;
        var timedOut = false;

        try
        {
            using var target = new WindowsFlutterTarget();
            using var session = new FlutterHostSession(entrypoint);
            using var scope = session.dispatcher.EnterScope();

            session.Start(deferFrameworkBootstrap: true);
            var view = target.CreateView(
                session,
                DemoViewId,
                new FlutterViewConfiguration("Doroti Material Demo", new Size(720, 640)));
            view.Show();
            session.dispatcher.setSemanticsTreeEnabled(true);
            var deadline = DateTime.UtcNow + options.Timeout;

            while (session.dispatcher.views.Any(candidate => candidate.viewId == DemoViewId))
            {
                target.PumpPendingMessages();
                diagnostics = target.CaptureDiagnostics(DemoViewId);
                if (entrypoint.FirstFrameworkError is not null)
                {
                    WriteSmokeEvidence(options, target, entrypoint, diagnostics, null, null, false);
                    Console.Error.WriteLine(FormatFirstCause(entrypoint.FirstFrameworkError.exceptionThrown));
                    session.Shutdown();
                    return 1;
                }

                if (options.Smoke && diagnostics.Frame.Presented >= 1 && entrypoint.GalleryState is null &&
                    diagnostics.Frame.QueueDepth == 0 && diagnostics.Frame.ActiveFrames == 0)
                {
                    entrypoint.RequestFrame();
                }

                if (options.Smoke && diagnostics.Frame.Presented >= 1 &&
                    entrypoint.GalleryState is not null && entrypoint.RootScaffold is not null)
                {
                    var beforeInitialCapture = diagnostics.Frame.Presented;
                    var initialReadbackTask = target.CaptureNextFrameAsync(DemoViewId);
                    entrypoint.RequestFrame();
                    WaitUntil(() => target.CaptureDiagnostics(DemoViewId).Frame.Presented > beforeInitialCapture,
                        target, entrypoint, TimeSpan.FromSeconds(10));
                    entrypoint.InitialReadback = initialReadbackTask.WaitAsync(TimeSpan.FromSeconds(10))
                        .GetAwaiter().GetResult();
                    entrypoint.InitialStateSignature = entrypoint.GalleryState.StateSignature;
                    entrypoint.NativePointerHitTestTargets = entrypoint.HitTestTargetsAt(80, 200);
                    var beforeNativePointerInteraction = entrypoint.GalleryState.InteractionCount;
                    target.PostPointerTapForValidation(DemoViewId, 80, 200);
                    WaitUntil(() => entrypoint.GalleryState.InteractionCount > beforeNativePointerInteraction,
                        target, entrypoint, TimeSpan.FromSeconds(10));
                    entrypoint.NativePointerInteractionCount =
                        entrypoint.GalleryState.InteractionCount - beforeNativePointerInteraction;
                    var beforeInteractionBuild = entrypoint.GalleryState.BuildCount;
                    entrypoint.ExerciseAll();
                    entrypoint.RequestFrame();
                    WaitUntil(() => entrypoint.GalleryState.BuildCount > beforeInteractionBuild,
                        target, entrypoint, TimeSpan.FromSeconds(10));
                    var changedReadbackTask = target.CaptureNextFrameAsync(DemoViewId);
                    entrypoint.RequestFrame();
                    WaitUntil(() => changedReadbackTask.IsCompleted,
                        target, entrypoint, TimeSpan.FromSeconds(10));
                    readback = changedReadbackTask.WaitAsync(TimeSpan.FromSeconds(10)).GetAwaiter().GetResult();
                    entrypoint.ChangedReadback = readback;
                    entrypoint.ChangedStateSignature = entrypoint.GalleryState.StateSignature;
                    var beforeExternalUiaFrame = target.CaptureDiagnostics(DemoViewId).Frame.Presented;
                    WriteReady(options, target);
                    if (entrypoint.RequireExternalUia)
                    {
                        var expectedWithExternalUia = MaterialGalleryState.InteractiveLabels.Length * 2 + 1;
                        WaitUntil(() =>
                        {
                            var frame = target.CaptureDiagnostics(DemoViewId).Frame;
                            return entrypoint.GalleryState.InteractionCount >= expectedWithExternalUia &&
                                frame.Presented > beforeExternalUiaFrame &&
                                frame.QueueDepth == 0 && frame.ActiveFrames == 0;
                        },
                            target, entrypoint, TimeSpan.FromSeconds(20));
                    }

                    var cadence = TimeSpan.FromMilliseconds(
                        options.CadenceDuration.TotalMilliseconds / Math.Max(1, options.Frames - 1));
                    var cadenceRun = Stopwatch.StartNew();
                    var cadenceBaseline = target.CaptureDiagnostics(DemoViewId).Frame.Presented;
                    for (var index = 0; index < options.Frames; index++)
                    {
                        var due = TimeSpan.FromTicks(cadence.Ticks * index);
                        while (cadenceRun.Elapsed < due)
                        {
                            target.PumpPendingMessages();
                            Thread.Sleep(1);
                        }
                        var beforeFrame = target.CaptureDiagnostics(DemoViewId).Frame.Presented;
                        entrypoint.RequestFrame();
                        WaitUntil(() => target.CaptureDiagnostics(DemoViewId).Frame.Presented > beforeFrame,
                            target, entrypoint, TimeSpan.FromSeconds(5));
                    }
                    while (cadenceRun.Elapsed < options.CadenceDuration)
                    {
                        target.PumpPendingMessages();
                        Thread.Sleep(1);
                    }
                    entrypoint.CadenceDuration = cadenceRun.Elapsed;
                    entrypoint.CadencePresented = target.CaptureDiagnostics(DemoViewId).Frame.Presented - cadenceBaseline;
                    WaitUntil(() =>
                    {
                        var frame = target.CaptureDiagnostics(DemoViewId).Frame;
                        var terminal = frame.Presented + frame.Superseded + frame.Stale + frame.Failed + frame.Cancelled;
                        return frame.QueueDepth == 0 && frame.ActiveFrames == 0 && frame.Submitted == terminal;
                    }, target, entrypoint, TimeSpan.FromSeconds(10));
                    diagnostics = target.CaptureDiagnostics(DemoViewId);
                    ValidateMaterialSmoke(entrypoint, diagnostics, readback);
                    WriteArtifacts(options, entrypoint.InitialReadback, readback);
                    session.DetachView(view);
                    session.Shutdown();
                    view.Dispose();
                    for (var pump = 0; pump < 20; pump++) target.PumpPendingMessages();
                    var resourceClosure = target.CaptureResourceSnapshot();
                    if (!resourceClosure.IsBalanced)
                        throw new InvalidDataException($"Material native resource closure is not balanced: {resourceClosure}.");
                    WriteSmokeEvidence(options, target, entrypoint, diagnostics, readback, null, false, resourceClosure);
                    Console.WriteLine(
                        $"DorotiDemoApp Material: PASS ({target.Identity.Rid}; {entrypoint.RootApp.GetType().FullName})");
                    return 0;
                }

                if (options.Smoke && DateTime.UtcNow >= deadline)
                {
                    timedOut = true;
                    throw new TimeoutException(
                        $"DorotiDemoApp did not present its Material frame within {options.Timeout}.");
                }
                Thread.Sleep(1);
            }

            session.Shutdown();
            Console.WriteLine(
                $"DorotiDemoApp Material: PASS ({target.Identity.Rid}; {entrypoint.RootApp.GetType().FullName})");
            return 0;
        }
        catch (Exception exception)
        {
            unhandled = exception;
            WriteSmokeEvidence(options, null, entrypoint, diagnostics, readback, unhandled, timedOut);
            Console.Error.WriteLine(FormatFirstCause(exception));
            return 1;
        }
    }

    private static int FailBeforeTarget(DemoOptions options, Exception exception)
    {
        WriteSmokeEvidence(options, null, null, null, null, exception, false);
        Console.Error.WriteLine(FormatFirstCause(exception));
        return 1;
    }

    private static void WaitUntil(
        Func<bool> predicate,
        WindowsFlutterTarget target,
        MaterialDemoEntrypoint entrypoint,
        TimeSpan timeout)
    {
        var elapsed = Stopwatch.StartNew();
        while (!predicate())
        {
            if (entrypoint.FirstFrameworkError is { } error)
                throw new InvalidOperationException("Material framework error.", error.exceptionThrown);
            if (elapsed.Elapsed > timeout)
                throw new TimeoutException($"Material frame condition timed out after {timeout}.");
            target.PumpPendingMessages();
            Thread.Sleep(1);
        }
    }

    private static void ValidateMaterialSmoke(
        MaterialDemoEntrypoint entrypoint,
        DesktopFlutterTargetDiagnostics diagnostics,
        DesktopFlutterPixelReadback readback)
    {
        if (entrypoint.RootApp is not Material.MaterialApp materialApp ||
            entrypoint.RootScaffold is null ||
            (entrypoint.EntryMode == DemoEntryMode.Builder && materialApp.builder is null) ||
            (entrypoint.EntryMode == DemoEntryMode.Home && materialApp.home is null))
        {
            throw new InvalidDataException("The demo root is not a reviewed MaterialApp/Scaffold tree.");
        }
        if (diagnostics.Frame.BackendIdentity != "skia-wgl-opengl-gpu" ||
            diagnostics.Frame.SoftwareFallbackUsed)
        {
            throw new InvalidDataException(
                $"The Material demo did not use strict GPU rendering: {diagnostics.Frame.BackendIdentity}.");
        }
        if (!FlutterCapabilityIds.RequiredDesktop.All(
                id => diagnostics.CapabilityIds.Contains(id, StringComparer.Ordinal)))
        {
            throw new InvalidDataException("The Material demo target capability closure is incomplete.");
        }
        if (diagnostics.Resources.ActiveWindows != 1 ||
            diagnostics.Resources.ActiveOpenGlContexts != 1)
        {
            throw new InvalidDataException("The Material demo did not retain one native window and GPU context.");
        }
        if (MeasurePixels(readback).NonEmptyPixelCount == 0)
        {
            throw new InvalidDataException("The Material demo presented an empty transparent frame.");
        }
        var expectedInteractions = MaterialGalleryState.InteractiveLabels.Length *
            (entrypoint.RequireExternalUia ? 2 : 1) + 1;
        if ((entrypoint.GalleryState?.InteractionCount ?? 0) < expectedInteractions ||
            entrypoint.NativePointerInteractionCount != 1 ||
            !entrypoint.NativePointerHitTestTargets.Any(target =>
                target.EndsWith("RenderPointerListener", StringComparison.Ordinal)) ||
            entrypoint.InitialStateSignature == entrypoint.ChangedStateSignature)
        {
            throw new InvalidDataException("The native pointer tap and six Material controls did not update local State.");
        }
        if (entrypoint.InitialReadback is null || CountChangedPixels(entrypoint.InitialReadback, readback) < 100)
        {
            throw new InvalidDataException("Material interaction did not produce a measurable raster change.");
        }
        var initialColors = MeasurePixels(entrypoint.InitialReadback).Colors;
        var changedColors = MeasurePixels(readback).Colors;
        foreach (var required in new[] { "appBarAndFab", "cardSurface", "primaryControls", "initialStack", "blackInk" })
        {
            if (!initialColors.TryGetValue(required, out var sample) || sample.Count == 0)
                throw new InvalidDataException($"Material initial raster is missing the {required} color region.");
        }
        if (!changedColors.TryGetValue("changedStackAndSwitch", out var changedSample) || changedSample.Count == 0)
            throw new InvalidDataException("Material changed raster is missing the changed stack/switch color region.");
        if (diagnostics.Automation.NodeCount < MaterialGalleryState.InteractiveLabels.Length)
        {
            throw new InvalidDataException("The Material component semantics tree is incomplete.");
        }
        if (entrypoint.CadencePresented < 1 || diagnostics.Frame.Failed != 0 || diagnostics.Frame.Cancelled != 0)
        {
            throw new InvalidDataException("The strict-GPU cadence reported a failed/cancelled frame.");
        }
    }

    private static void WriteReady(DemoOptions options, WindowsFlutterTarget target)
    {
        if (options.ReadyPath is null) return;
        WriteJson(options.ReadyPath, new
        {
            schemaVersion = "doroti.g6-material-demo-ready/v1",
            processId = Environment.ProcessId,
            hwnd = target.GetNativeWindowHandle(DemoViewId).ToInt64(),
            windowTitle = "Doroti Material Demo",
            semanticsNames = MaterialGalleryState.InteractiveLabels,
        });
    }

    private static void WriteArtifacts(
        DemoOptions options,
        DesktopFlutterPixelReadback? initial,
        DesktopFlutterPixelReadback changed)
    {
        if (options.ArtifactDirectory is null || initial is null) return;
        var directory = IOPath.GetFullPath(options.ArtifactDirectory);
        Directory.CreateDirectory(directory);
        WriteBmp(IOPath.Combine(directory, "material-initial.bmp"), initial);
        WriteBmp(IOPath.Combine(directory, "material-changed.bmp"), changed);
    }

    private static void WriteBmp(string path, DesktopFlutterPixelReadback readback)
    {
        var pixelBytes = checked(readback.Width * readback.Height * 4);
        using var stream = File.Create(path);
        using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: false);
        writer.Write((byte)'B'); writer.Write((byte)'M');
        writer.Write(54 + pixelBytes); writer.Write(0); writer.Write(54);
        writer.Write(40); writer.Write(readback.Width); writer.Write(-readback.Height);
        writer.Write((short)1); writer.Write((short)32); writer.Write(0);
        writer.Write(pixelBytes); writer.Write(2835); writer.Write(2835); writer.Write(0); writer.Write(0);
        for (var y = 0; y < readback.Height; y++)
            writer.Write(readback.Bgra8888Pixels, y * readback.RowBytes, readback.Width * 4);
    }

    private static long CountChangedPixels(DesktopFlutterPixelReadback before, DesktopFlutterPixelReadback after)
    {
        if (before.Width != after.Width || before.Height != after.Height || before.RowBytes != after.RowBytes)
            return long.MaxValue;
        long changed = 0;
        for (var index = 0; index < before.Bgra8888Pixels.Length; index += 4)
        {
            if (!before.Bgra8888Pixels.AsSpan(index, 4).SequenceEqual(after.Bgra8888Pixels.AsSpan(index, 4)))
                changed++;
        }
        return changed;
    }

    private static void WriteSmokeEvidence(
        DemoOptions options,
        WindowsFlutterTarget? target,
        MaterialDemoEntrypoint? entrypoint,
        DesktopFlutterTargetDiagnostics? diagnostics,
        DesktopFlutterPixelReadback? readback,
        Exception? unhandled,
        bool timedOut,
        object? resourceClosure = null)
    {
        if (options.EvidencePath is null)
        {
            return;
        }

        var root = options.PackageOnlyConsumer ? null : TryFindRepositoryRoot();
        var flutterError = entrypoint?.FirstFrameworkError;
        var firstCause = flutterError?.exceptionThrown ?? unhandled;
        var pixels = readback is null ? PixelEvidence.Empty : MeasurePixels(readback);
        var initialPixels = entrypoint?.InitialReadback is null
            ? PixelEvidence.Empty
            : MeasurePixels(entrypoint.InitialReadback);
        var outcome = flutterError is not null
            ? "frameworkError"
            : timedOut
                ? "frameTimeout"
                : unhandled is not null
                    ? "unhandledException"
                    : "presented";
        var evidence = new
        {
            schemaVersion = "doroti.g6-material-demo-run/v1",
            milestone = "G6-3",
            mode = "reviewed-promoted-material-gallery",
            outcome,
            source = new
            {
                flutterRevision = FlutterRevision,
                avaloniaRevision = AvaloniaRevision,
                flutterSourceLockSha256 = root is null ? null : HashFile(IOPath.Combine(root, "Doroti", "validation", "flutter-source.lock.json")),
                reviewedFrameworkDigest = root is null ? null : HashReviewedFramework(root),
                materialProductAssembly = typeof(Material.MaterialApp).Assembly.GetName().Name,
                consumer = root is null ? "clean-package-only-external" : "repository-product-project",
            },
            fixture = new
            {
                reviewedCSharp = "DorotiDemoApp/Program.cs",
                entryMode = entrypoint?.EntryMode.ToString().ToLowerInvariant(),
                frameworkPath = new[] { "MaterialApp", "Theme", "Navigator", "Scaffold", "AppBar", "Card", "ListTile", "ElevatedButton", "Checkbox", "Radio", "Switch", "Slider", "FloatingActionButton", "Row", "Column", "Stack", "SingleChildScrollView", "ListView.builder" },
                rootWidget = entrypoint?.RootApp.GetType().FullName,
            },
            interaction = new
            {
                controls = MaterialGalleryState.InteractiveLabels,
                count = entrypoint?.GalleryState?.InteractionCount ?? 0,
                nativePointerCount = entrypoint?.NativePointerInteractionCount ?? 0,
                nativePointerHitTestTargets = entrypoint?.NativePointerHitTestTargets ?? [],
                beforeState = entrypoint?.InitialStateSignature,
                afterState = entrypoint?.ChangedStateSignature,
                changedPixelCount = entrypoint?.InitialReadback is null || readback is null ? 0 : CountChangedPixels(entrypoint.InitialReadback, readback),
            },
            cadence = new
            {
                requestedFrames = options.Frames,
                requestedDurationMs = options.CadenceDuration.TotalMilliseconds,
                presented = entrypoint?.CadencePresented ?? 0,
                actualDurationMs = entrypoint?.CadenceDuration.TotalMilliseconds ?? 0,
            },
            firstFlutterError = flutterError is null ? null : new
            {
                exceptionType = flutterError.exceptionThrown.GetType().FullName,
                flutterError.exceptionThrown.Message,
                flutterError.library,
                context = flutterError.context?.toDescription(),
                widget = InferWidget(flutterError.exceptionThrown),
                sourceLibrary = InferLibrary(flutterError.exceptionThrown),
                stack = NormalizeStack(flutterError.exceptionThrown, root),
            },
            unhandledException = unhandled is null ? null : new
            {
                exceptionType = firstCause!.GetType().FullName,
                firstCause.Message,
                stack = NormalizeStack(firstCause, root),
            },
            frame = diagnostics?.Frame,
            automation = diagnostics?.Automation,
            backend = diagnostics?.Frame.BackendIdentity ?? target?.Identity.GraphicsBackend,
            initialPixels = new
            {
                width = entrypoint?.InitialReadback?.Width ?? 0,
                height = entrypoint?.InitialReadback?.Height ?? 0,
                nonEmptyPixelBounds = initialPixels.Bounds,
                initialPixels.NonEmptyPixelCount,
                initialPixels.Colors,
            },
            changedPixels = new
            {
                width = readback?.Width ?? 0,
                height = readback?.Height ?? 0,
                nonEmptyPixelBounds = pixels.Bounds,
                pixels.NonEmptyPixelCount,
                pixels.Colors,
            },
            resources = diagnostics?.Resources,
            resourceClosure,
            activeResourceCount = diagnostics is null
                ? 0
                : diagnostics.Resources.ActiveWindows + diagnostics.Resources.ActiveOpenGlContexts + diagnostics.Frame.ActiveFrames,
            frameTimeout = timedOut,
        };
        var fullPath = IOPath.GetFullPath(options.EvidencePath);
        Directory.CreateDirectory(IOPath.GetDirectoryName(fullPath)!);
        WriteJson(fullPath, evidence);
    }

    private static PixelEvidence MeasurePixels(DesktopFlutterPixelReadback readback)
    {
        var expectedColors = new Dictionary<uint, string>
        {
            [0xfffffbfe] = "background",
            [0xffeaddff] = "appBarAndFab",
            [0xfff3edf7] = "cardSurface",
            [0xff6750a4] = "primaryControls",
            [0xffb3261e] = "initialStack",
            [0xffd0bcff] = "changedStackAndSwitch",
            [0xff1d1b20] = "darkInk",
            [0xff21005d] = "appBarInk",
            [0xff000000] = "blackInk",
        };
        var colorAccumulators = expectedColors.ToDictionary(
            pair => pair.Key,
            pair => new ColorAccumulator(readback.Width, readback.Height));
        var width = readback.Width;
        var height = readback.Height;
        var left = width;
        var top = height;
        var right = -1;
        var bottom = -1;
        long count = 0;
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var offset = y * readback.RowBytes + x * 4;
                var argb = (uint)(readback.Bgra8888Pixels[offset + 3] << 24 |
                                  readback.Bgra8888Pixels[offset + 2] << 16 |
                                  readback.Bgra8888Pixels[offset + 1] << 8 |
                                  readback.Bgra8888Pixels[offset]);
                if (colorAccumulators.TryGetValue(argb, out var accumulator)) accumulator.Add(x, y);
                if (readback.Bgra8888Pixels[offset + 3] == 0)
                {
                    continue;
                }
                count++;
                left = Math.Min(left, x);
                top = Math.Min(top, y);
                right = Math.Max(right, x);
                bottom = Math.Max(bottom, y);
            }
        }
        var colors = expectedColors.ToDictionary(
            pair => pair.Value,
            pair => colorAccumulators[pair.Key].ToEvidence(),
            StringComparer.Ordinal);
        return count == 0
            ? new PixelEvidence(null, 0, colors)
            : new(new PixelBounds(left, top, right - left + 1, bottom - top + 1), count, colors);
    }

    private static string InferWidget(Exception exception) =>
        exception.StackTrace?.Contains("._AppBarState", StringComparison.Ordinal) == true ? "AppBar" : "unknown";

    private static string InferLibrary(Exception exception) =>
        exception.StackTrace?.Contains("app_bar.g.cs", StringComparison.OrdinalIgnoreCase) == true
            ? "package:flutter/src/material/app_bar.dart"
            : "unknown";

    private static string[] NormalizeStack(Exception exception, string? root)
    {
        var rootPrefix = root is null
            ? null
            : root.TrimEnd(IOPath.DirectorySeparatorChar) + IOPath.DirectorySeparatorChar;
        return (exception.StackTrace ?? string.Empty)
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Select(line => (rootPrefix is null ? line.Trim() : line.Trim().Replace(rootPrefix, string.Empty, StringComparison.OrdinalIgnoreCase)).Replace('\\', '/'))
            .ToArray();
    }

    private static string FormatFirstCause(Exception exception)
    {
        var cause = exception;
        while (cause.InnerException is not null)
        {
            cause = cause.InnerException;
        }
        return $"{cause.GetType().FullName}: {cause.Message}{Environment.NewLine}{cause.StackTrace}";
    }

    private static string HashReviewedFramework(string root)
    {
        var reviewedRoot = IOPath.Combine(root, "Doroti", "migration", "generated-candidates", "g5-4-reviewed");
        using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        foreach (var path in Directory.EnumerateFiles(reviewedRoot, "*", SearchOption.AllDirectories)
                     .Where(path => IOPath.GetExtension(path) is ".cs" or ".csproj" or ".props" or ".targets")
                     .OrderBy(path => path, StringComparer.Ordinal))
        {
            var relative = IOPath.GetRelativePath(reviewedRoot, path).Replace('\\', '/');
            hash.AppendData(Encoding.UTF8.GetBytes(relative + "\n"));
            hash.AppendData(File.ReadAllBytes(path));
        }
        return Convert.ToHexString(hash.GetHashAndReset()).ToLowerInvariant();
    }

    private static string HashFile(string path) =>
        Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant();

    private static string? TryFindRepositoryRoot()
    {
        foreach (var start in new[] { Environment.CurrentDirectory, AppContext.BaseDirectory })
        {
            for (var directory = new DirectoryInfo(start); directory is not null; directory = directory.Parent)
            {
                if (File.Exists(IOPath.Combine(directory.FullName, "Doroti", "Doroti.Product.slnx")))
                {
                    return directory.FullName;
                }
            }
        }
        return null;
    }

    private static void WriteJson(string path, object value)
    {
        var fullPath = IOPath.GetFullPath(path);
        Directory.CreateDirectory(IOPath.GetDirectoryName(fullPath)!);
        var temporary = fullPath + ".tmp-" + Guid.NewGuid().ToString("N");
        File.WriteAllText(temporary, JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
        }) + "\n", new UTF8Encoding(false));
        File.Move(temporary, fullPath, true);
    }

    private sealed record PixelBounds(int X, int Y, int Width, int Height);
    private sealed record ColorEvidence(PixelBounds? Bounds, long Count);
    private sealed class ColorAccumulator(int width, int height)
    {
        private int _left = width, _top = height, _right = -1, _bottom = -1;
        private long _count;
        internal void Add(int x, int y)
        {
            _count++;
            _left = Math.Min(_left, x); _top = Math.Min(_top, y);
            _right = Math.Max(_right, x); _bottom = Math.Max(_bottom, y);
        }
        internal ColorEvidence ToEvidence() => _count == 0
            ? new(null, 0)
            : new(new PixelBounds(_left, _top, _right - _left + 1, _bottom - _top + 1), _count);
    }
    private sealed record PixelEvidence(
        PixelBounds? Bounds,
        long NonEmptyPixelCount,
        IReadOnlyDictionary<string, ColorEvidence> Colors)
    {
        internal static PixelEvidence Empty { get; } = new(null, 0,
            new Dictionary<string, ColorEvidence>(StringComparer.Ordinal));
    }
}

internal sealed class MaterialDemoEntrypoint(DemoEntryMode entryMode, bool requireExternalUia) : IFlutterViewEntrypoint
{
    private WidgetsFlutterBinding? _binding;
    private FlutterView? _view;

    internal Material.Scaffold? RootScaffold { get; private set; }
    internal MaterialGalleryState? GalleryState { get; private set; }
    internal Widget RootApp => _rootApp ??= CreateRootApp();
    internal DemoEntryMode EntryMode { get; } = entryMode;
    internal bool RequireExternalUia { get; } = requireExternalUia;
    internal DesktopFlutterPixelReadback? InitialReadback { get; set; }
    internal DesktopFlutterPixelReadback? ChangedReadback { get; set; }
    internal string? InitialStateSignature { get; set; }
    internal string? ChangedStateSignature { get; set; }
    internal long CadencePresented { get; set; }
    internal TimeSpan CadenceDuration { get; set; }
    internal int NativePointerInteractionCount { get; set; }
    internal IReadOnlyList<string> NativePointerHitTestTargets { get; set; } = [];

    private Material.MaterialApp? _rootApp;

    internal FlutterErrorDetails? FirstFrameworkError { get; private set; }

    public void Bootstrap(PlatformDispatcher dispatcher)
    {
        FlutterError.onError = details => FirstFrameworkError ??= details;
        _binding = new WidgetsFlutterBinding(dispatcher);
    }

    public void AttachView(FlutterView view)
    {
        if (_binding is null)
        {
            throw new InvalidOperationException("The Material framework binding was not bootstrapped.");
        }
        if (_view is not null)
        {
            throw new InvalidOperationException("DorotiDemoApp owns exactly one Flutter view.");
        }

        _view = view;
        _binding.scheduleFrameCallback(_ =>
        {
            _binding.attachRootWidget(_binding.wrapWithDefaultView(RootApp));
        });
    }

    public void DetachView(FlutterView view)
    {
        if (ReferenceEquals(_view, view))
        {
            _view = null;
        }
    }

    internal void ExerciseAll() =>
        (GalleryState ?? throw new InvalidOperationException("The Material gallery State is not mounted.")).ExerciseAll();

    internal void RequestFrame()
    {
        if (GalleryState is { } galleryState)
        {
            galleryState.PulseFrame();
            return;
        }
        (_binding ?? throw new InvalidOperationException("The Material binding is not initialized.")).scheduleFrame();
    }

    internal IReadOnlyList<string> HitTestTargetsAt(double x, double y)
    {
        var binding = _binding ?? throw new InvalidOperationException("The Material binding is not initialized.");
        var result = new Doroti.Generated.Framework.Gestures.HitTestResult();
        binding.hitTestInView(
            result,
            new Offset(x, y),
            checked((long)(_view ?? throw new InvalidOperationException("The Flutter view is not attached.")).viewId));
        return result.path.Select(entry => entry.target.GetType().FullName ?? entry.target.GetType().Name).ToArray();
    }

    public void Shutdown()
    {
        _binding?.Dispose();
        _binding = null;
        _view = null;
        FlutterError.onError = null;
    }

    private Material.MaterialApp CreateRootApp()
    {
        var theme = Material.ThemeData.Create(
            useMaterial3: true,
            colorSchemeSeed: new UiColor(0xff6750a4L),
            scaffoldBackgroundColor: new UiColor(0xfffffbfeL));
        Widget Gallery() => new MaterialGallery(
                state => GalleryState = state,
                scaffold => RootScaffold = scaffold);

        return EntryMode == DemoEntryMode.Builder
            ? new Material.MaterialApp(
                title: "Doroti Material Demo",
                color: new UiColor(0xff6750a4L),
                locale: new Locale("en", "US"),
                debugShowCheckedModeBanner: false,
                builder: (_, _) => new Material.Theme(
                    data: theme,
                    child: new Overlay(initialEntries:
                    [
                        new OverlayEntry(builder: _ => Gallery()),
                    ])))
            : new Material.MaterialApp(
                title: "Doroti Material Demo",
                color: new UiColor(0xff6750a4L),
                locale: new Locale("en", "US"),
                debugShowCheckedModeBanner: false,
                home: new Material.Theme(data: theme, child: Gallery()));
    }
}

internal sealed class MaterialGallery(
    System.Action<MaterialGalleryState> mounted,
    System.Action<Material.Scaffold> scaffoldBuilt) : StatefulWidget
{
    internal System.Action<MaterialGalleryState> Mounted { get; } = mounted;
    internal System.Action<Material.Scaffold> ScaffoldBuilt { get; } = scaffoldBuilt;
    public override IState createState() => new MaterialGalleryState();
}

internal sealed class MaterialGalleryState : State<MaterialGallery>
{
    internal static readonly string[] InteractiveLabels =
    [
        "G6 Material button", "G6 Material checkbox", "G6 Material radio",
        "G6 Material switch", "G6 Material slider", "G6 Material FAB",
    ];

    private int _buttonCount;
    private bool _checked;
    private long _radio;
    private bool _switched;
    private double _slider = 0.2;
    private int _fabCount;

    internal int InteractionCount { get; private set; }
    internal int BuildCount { get; private set; }
    internal string StateSignature =>
        $"button={_buttonCount};checked={_checked};radio={_radio};switch={_switched};slider={_slider:F1};fab={_fabCount}";

    public override void initState()
    {
        base.initState();
        widget.Mounted(this);
    }

    internal void ExerciseAll() => setState(() =>
    {
        _buttonCount++;
        _checked = !_checked;
        _radio = _radio == 1 ? 0 : 1;
        _switched = !_switched;
        _slider = _slider < 0.7 ? 0.8 : 0.2;
        _fabCount++;
        InteractionCount += InteractiveLabels.Length;
    });

    internal void PulseFrame() => setState(() => { });

    private void Mutate(System.Action mutation) => setState(() =>
    {
        mutation();
        InteractionCount++;
    });

    private Widget ActionSemantics(string label, Widget child, System.Action action, string value) => new Semantics(
        container: true,
        excludeSemantics: true,
        identifier: label.Replace(' ', '-').ToLowerInvariant(),
        label: label,
        value: value,
        button: label is "G6 Material button" or "G6 Material FAB",
        @checked: label == "G6 Material checkbox" ? _checked : null,
        selected: label == "G6 Material radio" ? _radio == 1 : null,
        inMutuallyExclusiveGroup: label == "G6 Material radio" ? true : null,
        toggled: label == "G6 Material switch" ? _switched : null,
        slider: label == "G6 Material slider" ? true : null,
        onTap: () => Mutate(action),
        child: child);

    public override Widget build(BuildContext context)
    {
        BuildCount++;
        var button = ActionSemantics(InteractiveLabels[0], new Material.ElevatedButton(
            onPressed: () => Mutate(() => _buttonCount++),
            child: new Text("Press button")), () => _buttonCount++, _buttonCount.ToString());
        var checkbox = ActionSemantics(InteractiveLabels[1], new Material.Checkbox(
            value: _checked,
            semanticLabel: "Gallery checkbox",
            activeColor: new UiColor(0xff6750a4L),
            fillColor: new WidgetStatePropertyAll<UiColor?>(new UiColor(0xff6750a4L)),
            overlayColor: new WidgetStatePropertyAll<UiColor?>(new UiColor(0x226750a4L)),
            checkColor: new UiColor(0xffffffffL),
            focusColor: new UiColor(0x226750a4L),
            hoverColor: new UiColor(0x226750a4L),
            splashRadius: 20,
            side: new BorderSide(color: new UiColor(0xff49454fL), width: 2),
            shape: new RoundedRectangleBorder(),
            materialTapTargetSize: Material.MaterialTapTargetSize.padded,
            visualDensity: Material.VisualDensity.standard,
            onChanged: value => Mutate(() => _checked = value == true)), () => _checked = !_checked, _checked.ToString());
        var radio = ActionSemantics(InteractiveLabels[2], new Material.Radio<long>(
            value: 1,
            groupValue: _radio,
            activeColor: new UiColor(0xff6750a4L),
            fillColor: new WidgetStatePropertyAll<UiColor?>(new UiColor(0xff6750a4L)),
            backgroundColor: new WidgetStatePropertyAll<UiColor?>(new UiColor(0x00000000L)),
            overlayColor: new WidgetStatePropertyAll<UiColor?>(new UiColor(0x226750a4L)),
            focusColor: new UiColor(0x226750a4L),
            hoverColor: new UiColor(0x226750a4L),
            splashRadius: 20,
            side: new BorderSide(color: new UiColor(0xff49454fL), width: 2),
            materialTapTargetSize: Material.MaterialTapTargetSize.padded,
            visualDensity: Material.VisualDensity.standard,
            onChanged: value => Mutate(() => _radio = value)), () => _radio = _radio == 1 ? 0 : 1, _radio.ToString());
        var toggle = ActionSemantics(InteractiveLabels[3], new Material.Switch(
            value: _switched,
            activeThumbColor: new UiColor(0xff6750a4L),
            activeTrackColor: new UiColor(0xffd0bcffL),
            inactiveThumbColor: new UiColor(0xff79747eL),
            inactiveTrackColor: new UiColor(0xffe7e0ecL),
            thumbColor: new WidgetStatePropertyAll<UiColor?>(new UiColor(_switched ? 0xff6750a4L : 0xff79747eL)),
            trackColor: new WidgetStatePropertyAll<UiColor?>(new UiColor(_switched ? 0xffd0bcffL : 0xffe7e0ecL)),
            trackOutlineColor: new WidgetStatePropertyAll<UiColor?>(new UiColor(0xff79747eL)),
            trackOutlineWidth: new WidgetStatePropertyAll<double?>(1),
            overlayColor: new WidgetStatePropertyAll<UiColor?>(new UiColor(0x226750a4L)),
            focusColor: new UiColor(0x226750a4L),
            hoverColor: new UiColor(0x226750a4L),
            splashRadius: 20,
            onChanged: value => Mutate(() => _switched = value)), () => _switched = !_switched, _switched.ToString());
        var slider = ActionSemantics(InteractiveLabels[4], new Material.Slider(
            value: _slider,
            min: 0,
            max: 1,
            divisions: 10,
            activeColor: new UiColor(0xff6750a4L),
            inactiveColor: new UiColor(0xffcac4d0L),
            thumbColor: new UiColor(0xff6750a4L),
            overlayColor: new WidgetStatePropertyAll<UiColor?>(new UiColor(0x226750a4L)),
            showValueIndicator: Material.ShowValueIndicator.never,
            onChanged: value => Mutate(() => _slider = value)), () => _slider = _slider < 0.7 ? 0.8 : 0.2, $"{_slider:F1}");

        var lazyList = ListView.CreateBuilder(
            primary: false,
            itemCount: 12,
            itemExtent: 30,
            itemBuilder: (_, index) => new Text($"Lazy item {index + 1}"));

        var scaffold = new Material.Scaffold(
            appBar: new Material.AppBar(
                title: new Text("Doroti Material Gallery"),
                backgroundColor: new UiColor(0xffeaddffL),
                foregroundColor: new UiColor(0xff21005dL),
                iconTheme: new IconThemeData(color: new UiColor(0xff1d1b20L), size: 24),
                actionsIconTheme: new IconThemeData(color: new UiColor(0xff49454fL), size: 24)),
            body: new SingleChildScrollView(
                primary: false,
                child: new Container(
                    padding: EdgeInsets.CreateAll(16),
                    child: new Column(
                        crossAxisAlignment: Doroti.Generated.Framework.Rendering.CrossAxisAlignment.start,
                        spacing: 10,
                        children:
                        [
                            new Text("Reviewed Material · promoted product · strict Skia GPU"),
                            new Material.Card(
                                color: new UiColor(0xfff3edf7L),
                                child: new Material.ListTile(
                                    title: new Text("Material components"),
                                    subtitle: new Text("Card + ListTile + local state"))),
                            new Row(spacing: 12, children: [button, new Text($"Pressed {_buttonCount}")]),
                            new Row(spacing: 12, children: [checkbox, new Text("Checkbox"), radio, new Text("Radio")]),
                            new Row(spacing: 12, children: [toggle, new Text("Switch")]),
                            slider,
                            new SizedBox(height: 64, child: new Stack(
                                alignment: Alignment.center,
                                children:
                                [
                                    new Container(width: 260, height: 56, color: new UiColor(_switched ? 0xffd0bcffL : 0xffb3261eL)),
                                    new Text($"Stack state · {StateSignature}"),
                                ])),
                            new Text("Lazy ListView.builder"),
                            new SizedBox(height: 100, child: lazyList),
                        ]))),
            floatingActionButton: ActionSemantics(InteractiveLabels[5], new Material.FloatingActionButton(
                tooltip: "Material action",
                backgroundColor: new UiColor(0xffeaddffL),
                foregroundColor: new UiColor(0xff21005dL),
                onPressed: () => Mutate(() => _fabCount++),
                child: new Text("+")), () => _fabCount++, _fabCount.ToString()));
        widget.ScaffoldBuilt(scaffold);
        return scaffold;
    }
}

internal enum DemoEntryMode { Builder, Home }

internal sealed record DemoOptions(
    bool Smoke,
    TimeSpan Timeout,
    string? EvidencePath,
    DemoEntryMode EntryMode,
    int Frames,
    TimeSpan CadenceDuration,
    string? ArtifactDirectory,
    string? ReadyPath,
    bool RequireExternalUia,
    bool PackageOnlyConsumer)
{
    internal static DemoOptions Parse(string[] arguments)
    {
        var smoke = false;
        var timeout = TimeSpan.FromSeconds(8);
        string? evidencePath = null;
        var entryMode = DemoEntryMode.Builder;
        var frames = 1;
        var cadenceDuration = TimeSpan.Zero;
        string? artifactDirectory = null;
        string? readyPath = null;
        var requireExternalUia = false;
        var packageOnlyConsumer = false;
        for (var index = 0; index < arguments.Length; index++)
        {
            switch (arguments[index])
            {
                case "--smoke":
                    smoke = true;
                    break;
                case "--g6-baseline":
                    smoke = true;
                    evidencePath = arguments[++index];
                    break;
                case "--evidence":
                    evidencePath = arguments[++index];
                    break;
                case "--artifact-dir":
                    artifactDirectory = arguments[++index];
                    break;
                case "--ready":
                    readyPath = arguments[++index];
                    break;
                case "--require-external-uia":
                    requireExternalUia = true;
                    break;
                case "--package-only-consumer":
                    packageOnlyConsumer = true;
                    break;
                case "--duration-ms":
                    timeout = TimeSpan.FromMilliseconds(
                        int.Parse(arguments[++index], System.Globalization.CultureInfo.InvariantCulture));
                    if (timeout <= TimeSpan.Zero)
                    {
                        throw new ArgumentOutOfRangeException(
                            nameof(arguments),
                            "Duration must be positive.");
                    }
                    break;
                case "--entry":
                    entryMode = arguments[++index].ToLowerInvariant() switch
                    {
                        "builder" => DemoEntryMode.Builder,
                        "home" => DemoEntryMode.Home,
                        var value => throw new ArgumentException($"Unknown Material entry mode: {value}"),
                    };
                    break;
                case "--frames":
                    frames = int.Parse(arguments[++index], System.Globalization.CultureInfo.InvariantCulture);
                    if (frames <= 0) throw new ArgumentOutOfRangeException(nameof(arguments), "Frames must be positive.");
                    break;
                case "--cadence-duration-ms":
                    cadenceDuration = TimeSpan.FromMilliseconds(
                        int.Parse(arguments[++index], System.Globalization.CultureInfo.InvariantCulture));
                    if (cadenceDuration < TimeSpan.Zero)
                        throw new ArgumentOutOfRangeException(nameof(arguments), "Cadence duration cannot be negative.");
                    break;
                default:
                    throw new ArgumentException($"Unknown argument: {arguments[index]}");
            }
        }
        return new(smoke, timeout, evidencePath, entryMode, frames, cadenceDuration,
            artifactDirectory, readyPath, requireExternalUia, packageOnlyConsumer);
    }
}
