using Android.Content;
using Android.Graphics;
using Android.Hardware.Camera2;
using Android.Hardware.Camera2.Params;
using Android.Media;
using Android.OS;
using Android.Views;
using Doroti.Host.Maui;
using Doroti.Ui;
using Java.Util.Concurrent;
using Paint = Android.Graphics.Paint;

namespace DorotiTestbedApp.Android;

// Opt-in acceptance producer, not a camera/video plugin bundled into Doroti.
internal sealed class AndroidTextureProbeProducer : IDisposable
{
    private readonly Surface _surface;
    private readonly Handler _handler = new(Looper.MainLooper!);
    private readonly SurfaceTextureEntry _entry;
    private readonly string _source;
    private MediaPlayer? _player;
    private CameraDevice? _camera;
    private CameraCaptureSession? _session;
    private System.Threading.Timer? _timer;
    private bool _disposed;
    private int _frames,
        _queued;
    private readonly Paint _paint = new();

    internal AndroidTextureProbeProducer(SurfaceTextureEntry entry, string source)
    {
        _entry = entry;
        _source = source;
        _surface = (Surface)entry.Surface;
        if (source == "video")
        {
            _player = new MediaPlayer();
            _player.SetSurface(_surface);
            _player.Looping = true;
            _player.SetVolume(0, 0);
            using var asset = global::Android.App.Application.Context.Assets!.OpenFd(
                "texture-pattern.mp4"
            );
            _player.SetDataSource(asset.FileDescriptor, asset.StartOffset, asset.DeclaredLength);
            _player.Prepared += (_, _) =>
            {
                if (!_disposed)
                    _player.Start();
            };
            _player.Error += (_, e) =>
                global::Android.Util.Log.Error(
                    "DorotiTextureProbe",
                    $"MediaPlayer {e.What}/{e.Extra}"
                );
            _player.PrepareAsync();
        }
        else if (source == "camera")
        {
            var manager = (CameraManager)
                global::Android.App.Application.Context.GetSystemService(Context.CameraService)!;
            manager.OpenCamera(manager.GetCameraIdList()[0], new CameraState(this), _handler);
        }
        _timer = new System.Threading.Timer(
            _ =>
            {
                if (Interlocked.Exchange(ref _queued, 1) != 0)
                    return;
                _handler.Post(() =>
                {
                    Interlocked.Exchange(ref _queued, 0);
                    if (_disposed)
                        return;
                    if (_source == "canvas")
                        Draw();
                    if (
                        ++_frames % 30 == 0
                        && OperatingSystem.IsAndroidVersionAtLeast(33)
                        && _entry is AndroidSurfaceTextureEntry native
                    )
                        global::Android.Util.Log.Info(
                            "DorotiTextureProbe",
                            $"source={_source} received={native.ReceivedFrames} drawn={native.DrawnFrames} imported={native.GpuFrameCounts.Imported} retired={native.GpuFrameCounts.Retired} transport=oes-ahb-vulkan error={native.Error ?? "none"}"
                        );
                });
            },
            null,
            33,
            33
        );
    }

    private void Draw()
    {
        var canvas =
            _surface.LockHardwareCanvas()
            ?? throw new InvalidOperationException("No hardware canvas.");
        try
        {
            canvas.DrawColor(global::Android.Graphics.Color.Black);
            _paint.Color = global::Android.Graphics.Color.Red;
            canvas.DrawRect(0, 0, 160, 90, _paint);
            _paint.Color = global::Android.Graphics.Color.Lime;
            canvas.DrawRect(160, 0, 320, 90, _paint);
            _paint.Color = global::Android.Graphics.Color.Blue;
            canvas.DrawRect(0, 90, 160, 180, _paint);
            _paint.Color = global::Android.Graphics.Color.Yellow;
            canvas.DrawRect(160, 90, 320, 180, _paint);
            _paint.Color = global::Android.Graphics.Color.White;
            canvas.DrawRect((_frames * 3) % 290, 75, (_frames * 3) % 290 + 30, 105, _paint);
        }
        finally
        {
            _surface.UnlockCanvasAndPost(canvas);
        }
    }

    public void Dispose()
    {
        _disposed = true;
        _timer?.Dispose();
        _session?.Close();
        _session?.Dispose();
        _camera?.Close();
        _camera?.Dispose();
        _player?.Release();
        _player?.Dispose();
        _paint.Dispose();
    }

    private sealed class MainExecutor(Handler handler) : Java.Lang.Object, IExecutor
    {
        public void Execute(Java.Lang.IRunnable? command)
        {
            if (command is not null)
                handler.Post(command);
        }
    }

    private sealed class CameraState(AndroidTextureProbeProducer owner) : CameraDevice.StateCallback
    {
        public override void OnOpened(CameraDevice camera)
        {
            if (!OperatingSystem.IsAndroidVersionAtLeast(28))
            {
                camera.Close();
                return;
            }
            if (owner._disposed)
            {
                camera.Close();
                return;
            }
            owner._camera = camera;
            using var output = new OutputConfiguration(owner._surface);
            using var config = new SessionConfiguration(
                0,
                [output],
                new MainExecutor(owner._handler),
                new SessionState(owner)
            );
            camera.CreateCaptureSession(config);
        }

        public override void OnDisconnected(CameraDevice camera) => camera.Close();

        public override void OnError(CameraDevice camera, CameraError error)
        {
            global::Android.Util.Log.Error("DorotiTextureProbe", $"Camera error {error}");
            camera.Close();
        }
    }

    private sealed class SessionState(AndroidTextureProbeProducer owner)
        : CameraCaptureSession.StateCallback
    {
        public override void OnConfigured(CameraCaptureSession session)
        {
            if (owner._disposed)
            {
                session.Close();
                return;
            }
            owner._session = session;
            using var request = owner._camera!.CreateCaptureRequest(CameraTemplate.Preview);
            request.AddTarget(owner._surface);
            using var built = request.Build();
            session.SetRepeatingRequest(built, null, owner._handler);
        }

        public override void OnConfigureFailed(CameraCaptureSession session) =>
            global::Android.Util.Log.Error(
                "DorotiTextureProbe",
                "Camera session configuration failed"
            );
    }
}
