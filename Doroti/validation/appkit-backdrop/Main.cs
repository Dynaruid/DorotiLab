using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using AppKit;
using CoreGraphics;
using Doroti.Host.Maui;
using Doroti.Ui;
using Foundation;
using SkiaSharp;
using Path = System.IO.Path;

NSApplication.Init();
NSApplication.SharedApplication.Delegate = new BackdropDelegate();
NSApplication.Main(args);

public sealed class BackdropDelegate : NSApplicationDelegate
{
    private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    private readonly WindowBackdropMode[] _modes = [WindowBackdropMode.transparent, WindowBackdropMode.acrylic,
        WindowBackdropMode.liquidGlass, WindowBackdropMode.solid, WindowBackdropMode.system, WindowBackdropMode.acrylic];
    private readonly List<object> _results = [];
    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private readonly string _output = Environment.GetEnvironmentVariable("DOROTI_BACKDROP_EVIDENCE") ?? "/tmp/doroti-backdrop";
    private NSWindow _window = null!;
    private NSWindow _background = null!;
    private DorotiMacOSMetalView _view = null!;
    private DorotiMacOSMetalSurface _owner = null!;
    private NSTimer _timer = null!;
    private int _stage = -1;
    private double _stageStart;
    private long _previousFrames;
    private bool _detached;

    public override void DidFinishLaunching(NSNotification notification)
    {
        NSApplication.SharedApplication.ActivationPolicy = NSApplicationActivationPolicy.Regular;
        Directory.CreateDirectory(_output);
        _background = new NSWindow(new CGRect(100, 150, 760, 540), NSWindowStyle.Borderless, NSBackingStore.Buffered, false)
        { ContentView = new PatternView { Frame = new CGRect(0, 0, 760, 540) } };
        _background.OrderFront(null);
        _window = new NSWindow(new CGRect(160, 210, 600, 400), NSWindowStyle.Titled | NSWindowStyle.Resizable,
            NSBackingStore.Buffered, false) { Title = "Doroti native backdrop", BackgroundColor = NSColor.Orange, IsOpaque = true };
        _view = new DorotiMacOSMetalView { Frame = new CGRect(0, 0, 600, 400),
            AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable };
        _window.ContentView!.AddSubview(_view);
        _owner = (DorotiMacOSMetalSurface)Activator.CreateInstance(typeof(DorotiMacOSMetalSurface), Private, null, new object[] { 1UL }, null)!;
        var paint = _owner.GetType().GetEvent("Paint", Private)!;
        paint.GetAddMethod(true)!.Invoke(_owner, [Delegate.CreateDelegate(paint.EventHandlerType!, this, GetType().GetMethod(nameof(Paint))!)]);
        typeof(DorotiMacOSMetalView).GetMethod("Connect", Private)!.Invoke(_view, [_owner]);
        _window.MakeKeyAndOrderFront(null);
        NSApplication.SharedApplication.Activate();
        _timer = NSTimer.CreateRepeatingScheduledTimer(TimeSpan.FromMilliseconds(100), _ => Tick());
    }

    public void Paint(object context)
    {
        var surface = (SKSurface)context.GetType().GetProperty("Surface", Private)!.GetValue(context)!;
        surface.Canvas.Clear(SKColors.Transparent);
        using var ink = new SKPaint { Color = SKColors.Magenta };
        surface.Canvas.DrawRect(30, 30, 100, 60, ink);
    }

    private T Field<T>(string name) => (T)_view.GetType().GetField(name, Private)!.GetValue(_view)!;
    private static void Check(bool value, string message) { if (!value) throw new InvalidOperationException(message); }
    private void Tick()
    {
        try
        {
            Check(_clock.Elapsed.TotalSeconds < 35, "Backdrop fixture timed out.");
            Check(Field<long>("_commandBuffersErrored") == 0 && !Field<bool>("_faulted"), "Metal rendering failed.");
            if (_detached)
            {
                if (!Field<bool>("_resourcesReleased")) return;
                Check(!_window.ContentView!.Subviews.Any(v => v.Identifier == "doroti-window-backdrop"), "Effect leaked after detachment.");
                Check(_window.IsOpaque && _window.BackgroundColor.Equals(NSColor.Orange), "Original window state was not restored.");
                Finish(null);
                return;
            }
            if (_stage >= 0)
            {
                if (_clock.Elapsed.TotalSeconds - _stageStart < 1 || Field<long>("_commandBuffersCompleted") <= _previousFrames)
                { _view.NeedsDisplay = true; return; }
                ValidateStage();
            }
            if (++_stage == _modes.Length)
            {
                typeof(DorotiMacOSMetalView).GetMethod("Disconnect", Private)!.Invoke(_view, null);
                _view.RemoveFromSuperview();
                _detached = true;
                return;
            }
            _window.Title = _modes[_stage].ToString();
            _view.SetBackdrop(new(_modes[_stage], theme: WindowBackdropTheme.light));
            _window.SetContentSize(new CGSize(600 + _stage * 12, 400 + _stage * 8));
            _view.LayoutSubtreeIfNeeded();
            _stageStart = _clock.Elapsed.TotalSeconds;
            _previousFrames = Field<long>("_commandBuffersCompleted");
        }
        catch (Exception error) { Finish(error); }
    }

    private void ValidateStage()
    {
        var requested = _modes[_stage];
        var expected = requested == WindowBackdropMode.liquidGlass && !OperatingSystem.IsMacOSVersionAtLeast(26)
            ? WindowBackdropMode.acrylic : requested;
        Check(_view.AppliedBackdropMode == expected, "Unexpected applied mode.");
        Check(!_view.IsOpaque && _view.Layer?.Opaque == false, "Metal surface blocks alpha compositing.");
        var effects = _window.ContentView!.Subviews.Where(v => v.Identifier == "doroti-window-backdrop").ToArray();
        var needsEffect = expected is WindowBackdropMode.acrylic or WindowBackdropMode.liquidGlass;
        Check(effects.Length == (needsEffect ? 1 : 0), "Missing or duplicate native backdrop.");
        if (needsEffect)
        {
            var effect = effects.Single();
            Check(effect.Frame.Equals(_view.Frame), "Backdrop did not follow resized Metal bounds.");
            Check(effect.HitTest(new CGPoint(50, 50)) is null, "Backdrop intercepted input.");
            if (expected == WindowBackdropMode.acrylic)
                Check(effect is NSVisualEffectView blur && blur.BlendingMode == NSVisualEffectBlendingMode.BehindWindow,
                    "Acrylic did not select a behind-window effect.");
            if (OperatingSystem.IsMacOSVersionAtLeast(26) && expected == WindowBackdropMode.liquidGlass)
                Check(effect is NSGlassEffectView, "Liquid Glass did not use the native glass API.");
        }
        Check(_window.IsOpaque == (expected is WindowBackdropMode.solid or WindowBackdropMode.system), "Window opacity mismatch.");
        var start = new ProcessStartInfo("/usr/sbin/screencapture") { RedirectStandardError = true };
        foreach (var arg in new[] { "-x", "-o", "-l", _window.WindowNumber.ToString(), Path.Combine(_output, $"{_stage}-{requested}.png") })
            start.ArgumentList.Add(arg);
        using var capture = Process.Start(start)!;
        capture.WaitForExit();
        Check(capture.ExitCode == 0, "Screenshot failed: " + capture.StandardError.ReadToEnd());
        // Capture the entire display: window-only capture omits the windows
        // sampled by a backdrop and cannot validate desktop composition.
        var composite = new ProcessStartInfo("/usr/sbin/screencapture") { RedirectStandardError = true };
        foreach (var arg in new[] { "-x", "-m", Path.Combine(_output, $"{_stage}-{requested}-fullscreen.png") })
            composite.ArgumentList.Add(arg);
        using var compositeCapture = Process.Start(composite)!;
        compositeCapture.WaitForExit();
        Check(compositeCapture.ExitCode == 0, "Desktop composite capture failed: " + compositeCapture.StandardError.ReadToEnd());
        _results.Add(new { requested = requested.ToString(), applied = expected.ToString(),
            nativeType = effects.FirstOrDefault()?.GetType().BaseType?.Name,
            windowActive = _window.IsKeyWindow,
            completed = Field<long>("_commandBuffersCompleted"), width = (double)_view.Bounds.Width, height = (double)_view.Bounds.Height });
    }

    private void Finish(Exception? error)
    {
        _timer?.Invalidate();
        var json = JsonSerializer.Serialize(new { status = error is null ? "PASS" : "FAIL", error = error?.ToString(),
            renderer = Environment.GetEnvironmentVariable("DOROTI_MACOS_GRAPHITE") == "0" ? "Ganesh" : "Graphite",
            stages = _results, released = Field<bool>("_resourcesReleased") }, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path.Combine(_output, "result.json"), json);
        Console.WriteLine(json);
        Environment.Exit(error is null ? 0 : 1);
    }
}

public sealed class PatternView : NSView
{
    public override void DrawRect(CGRect dirtyRect)
    {
        NSColor.White.SetFill();
        NSBezierPath.FillRect(Bounds);
        for (var x = 0; x < Bounds.Width; x += 160)
        {
            (x % 320 == 0 ? NSColor.SystemBlue : NSColor.SystemOrange).SetFill();
            NSBezierPath.FillRect(new CGRect(x, 0, 80, Bounds.Height));
        }
    }
}
