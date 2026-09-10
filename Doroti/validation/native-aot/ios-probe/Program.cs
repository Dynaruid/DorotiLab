using Doroti.Host.Maui;
using Doroti.Hosting;
using Doroti.Runtime;
using Doroti.Ui;
using Foundation;
using UIKit;
using Native = DorotiTestbedApp.iOS.Native.DorotiNativeInterop;
using Canvas = Doroti.Ui.Canvas;
using Paint = Doroti.Ui.Paint;
using Rect = Doroti.Ui.Rect;
using BlendMode = Doroti.Ui.BlendMode;

namespace Doroti.Validation.NativeAot;

public static class Program
{
    public static void Main(string[] args) => UIApplication.Main(args, null, typeof(AppDelegate));
}

[Register("AppDelegate")]
public sealed class AppDelegate : DorotiMauiUIApplicationDelegate
{
    protected override DorotiApplicationDescriptor CreateApplicationDescriptor() =>
        new(() => new ProbeEntrypoint(), typeof(Program).Assembly, typeof(Program).Assembly,
            new("NativeAOT host probe", new(390, 844)),
            DorotiLaunchContext.Create("iOS", "ios-arm64"), [], []);
}

public sealed class ProbeEntrypoint : IDorotiViewEntrypoint
{
    private PlatformDispatcher? _dispatcher;
    private DorotiView? _view;
    private int _frames;
    private Doroti.Ui.Image? _image;
    private FragmentShader? _shader;

    public void Bootstrap(PlatformDispatcher dispatcher)
    {
        UIApplication.SharedApplication.IdleTimerDisabled = true;
        _dispatcher = dispatcher;
        dispatcher.onDrawFrame = Draw;
        dispatcher.onMetricsChanged = view => view.ScheduleFrame(DartUiInvocation.Managed("aot-probe#metrics"));
        Console.WriteLine($"NATIVEAOT_PROBE dynamicCode={System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported}");
    }

    public void AttachView(DorotiView view)
    {
        _view = view;
        view.ScheduleFrame(DartUiInvocation.Managed("aot-probe#attach"));
        var info = DorotiNativePlatformInfo.Parse(Native.PlatformInfo());
        const string payload = "NativeAOT 한글 callback";
        if (Native.Echo(payload) != payload) throw new InvalidOperationException("Native echo mismatch.");
        Native.EchoOnMainThread(payload, value =>
        {
            if (value != payload || !NSThread.IsMain) throw new InvalidOperationException("Native callback mismatch.");
            Console.WriteLine($"NATIVEAOT_PROBE native-roundtrip=pass info={info}");
        });
        GC.Collect();
        GC.WaitForPendingFinalizers();
        DartRuntimePrimitives.ObserveTask(PrepareResourcesAsync(view), "NativeAOT probe resources");
    }

    private async Task PrepareResourcesAsync(DorotiView view)
    {
        using var bitmap = new SkiaSharp.SKBitmap(2, 2);
        bitmap.SetPixel(0, 0, SkiaSharp.SKColors.Red);
        bitmap.SetPixel(1, 0, SkiaSharp.SKColors.Green);
        bitmap.SetPixel(0, 1, SkiaSharp.SKColors.Blue);
        bitmap.SetPixel(1, 1, SkiaSharp.SKColors.White);
        using var encoded = bitmap.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100);
        using var buffer = await ImmutableBuffer.fromUint8List(new Uint8List(encoded.ToArray()));
        using var codec = await Dart_uiLibrary.instantiateImageCodecFromBuffer(buffer);
        var frame = await codec.getNextFrame();
        if (frame.image.width != 2 || frame.image.height != 2)
            throw new InvalidOperationException("Decoded image dimensions differ.");
        if (!ReferenceEquals(_view, view)) { frame.image.Dispose(); return; }
        _image = frame.image;
        _shader = FragmentProgram.fromSource("""
            uniform float u_phase;
            half4 main(float2 p) {
                return half4(0.3 + 0.3 * sin(p.x * 0.03 + u_phase), 0.6, 0.9, 1.0);
            }
            """, "native-aot-probe").fragmentShader();
        _shader.setFloat(0, 0.5);
        Console.WriteLine("NATIVEAOT_PROBE image-decoded=pass shader-prepared=pass");
        view.ScheduleFrame(DartUiInvocation.Managed("aot-probe#resources-ready"));
    }

    private void Draw()
    {
        if (_view is not { } view) return;
        var recorder = new PictureRecorder();
        var canvas = new Canvas(recorder);
        canvas.drawColor(new(0xff10243a), BlendMode.src);
        var primary = MaterialColorSchemeRuntime.GetArgb(0xff6750a4, false, "tonalSpot", 0, "primary");
        canvas.drawRect(Rect.fromLTWH(24, 80, 240, 100), new Paint { color = new(primary) });
        canvas.drawCircle(new(120, 280), 60, new Paint { color = new(0xff4fc3f7) });
        var text = new ParagraphBuilder(new ParagraphStyle(fontSize: 24));
        text.pushStyle(new Doroti.Ui.TextStyle(color: new(0xffffffff)));
        text.addText("NativeAOT · Doroti · 한글");
        using var paragraph = text.build();
        paragraph.layout(new ParagraphConstraints(340));
        canvas.drawParagraph(paragraph, new(24, 190));
        if (_image is not null)
            canvas.drawImageRect(_image, Rect.fromLTWH(0, 0, 2, 2), Rect.fromLTWH(220, 240, 100, 100), new Paint());
        if (_shader is not null)
            canvas.drawRect(Rect.fromLTWH(24, 380, 296, 100), new Paint { shader = _shader });
        using var picture = recorder.endRecording();
        var builder = new SceneBuilder(view.viewId);
        builder.addPicture(Offset.zero, picture);
        using var scene = builder.build();
        view.render(scene);
        Console.WriteLine($"NATIVEAOT_PROBE scene-submitted={++_frames}");
    }

    public void DetachView(DorotiView view)
    {
        if (!ReferenceEquals(_view, view)) return;
        _view = null;
        _image?.Dispose(); _image = null;
        _shader?.dispose(); _shader = null;
    }
    public void Shutdown()
    {
        UIApplication.SharedApplication.IdleTimerDisabled = false;
        if (_view is not null) DetachView(_view);
        if (_dispatcher is not null) _dispatcher.onDrawFrame = null;
        _dispatcher = null;
    }
}
