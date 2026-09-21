#if ANDROID
using System.Runtime.InteropServices;
using Android.Graphics;
using Android.Hardware;
using Android.Media;
using Android.Opengl;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using Java.Nio;
using SkiaSharp;
using AndroidImage = Android.Media.Image;

namespace Doroti.Host.Maui;

/// <summary>Camera2/MediaCodec/MediaPlayer producer Surface. OES conversion and Vulkan import stay on GPU.
/// The Surface is borrowed: stop the native producer before disposing this entry.</summary>
[System.Runtime.Versioning.SupportedOSPlatform("android33.0")]
public sealed class AndroidSurfaceTextureEntry : SurfaceTextureEntry
{
    private readonly Producer _producer;
    private readonly SkiaExternalTextureRegistration _registration;

    private AndroidSurfaceTextureEntry(
        Producer producer,
        SkiaExternalTextureRegistration registration
    )
    {
        _producer = producer;
        _registration = registration;
    }

    public override long Id => _registration.Id;
    public override object Surface => AndroidSurface;
    public Surface AndroidSurface =>
        _producer.InputSurface
        ?? throw new ObjectDisposedException(nameof(AndroidSurfaceTextureEntry));
    public override string? Error => _producer.Error;
    public long ReceivedFrames => Interlocked.Read(ref _producer.Received);
    public long DrawnFrames => Interlocked.Read(ref _producer.Drawn);
    public (long Imported, long Retired) GpuFrameCounts => _producer.GpuFrameCounts;

    public override void Dispose() => _registration.Dispose();

    internal static async ValueTask<SurfaceTextureEntry> CreateAsync(
        SkiaSceneRenderer renderer,
        DorotiAndroidVulkanView surface,
        int width,
        int height,
        CancellationToken cancellationToken
    )
    {
        if (width < 1 || height < 1 || width > 4096 || height > 4096)
            throw new ArgumentOutOfRangeException(
                nameof(width),
                "Native texture dimensions must be between 1 and 4096."
            );
        cancellationToken.ThrowIfCancellationRequested();
        var producer = new Producer(surface, width, height);
        try
        {
            await producer.InitializeAsync().ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            var registration = renderer.RegisterExternalTexture(producer);
            producer.Registration = registration;
            registration.MarkFrameAvailable();
            return new AndroidSurfaceTextureEntry(producer, registration);
        }
        catch
        {
            producer.Dispose();
            throw;
        }
    }

    private sealed class Producer
        : Java.Lang.Object,
            ISkiaExternalTextureSource,
            SurfaceTexture.IOnFrameAvailableListener,
            ImageReader.IOnImageAvailableListener
    {
        private readonly DorotiAndroidVulkanView _surface;
        private readonly int _width,
            _height;
        private readonly object _gate = new();
        private readonly HandlerThread _thread = new("Doroti texture producer");
        private readonly Handler _handler;
        private EGLDisplay? _display;
        private EGLContext? _context;
        private EGLSurface? _output;
        private ImageReader? _reader;
        private SurfaceTexture? _input;
        private FloatBuffer? _vertices;
        private int _program,
            _texture;
        private readonly float[] _matrix = new float[16];
        private Frame? _pending,
            _current;
        private int _liveFrames;
        private bool _closed,
            _cleaned;
        internal Surface? InputSurface;
        internal SkiaExternalTextureRegistration? Registration;
        internal string? Error;
        internal long Received,
            Drawn;
        internal (long Imported, long Retired) GpuFrameCounts => _surface.TextureFrameCounts;

        internal Producer(DorotiAndroidVulkanView surface, int width, int height)
        {
            _surface = surface;
            _width = width;
            _height = height;
            _thread.Start();
            _handler = new Handler(_thread.Looper!);
        }

        internal Task InitializeAsync()
        {
            var completion = new TaskCompletionSource(
                TaskCreationOptions.RunContinuationsAsynchronously
            );
            _handler.Post(() =>
            {
                try
                {
                    Initialize();
                    completion.SetResult();
                }
                catch (Exception e)
                {
                    completion.SetException(e);
                }
            });
            return completion.Task;
        }

        private void Initialize()
        {
            _display = EGL14.EglGetDisplay(0);
            if (!EGL14.EglInitialize(_display, new int[2], 0, new int[2], 0))
                Fail("EGL initialize");
            EGLConfig[] configs = new EGLConfig[1];
            int[] count = new int[1];
            int[] attributes =
            [
                0x3024,
                8,
                0x3023,
                8,
                0x3022,
                8,
                0x3021,
                8,
                0x3033,
                4,
                0x3040,
                4,
                0x3038,
            ];
            if (
                !EGL14.EglChooseConfig(_display, attributes, 0, configs, 0, 1, count, 0)
                || count[0] == 0
            )
                Fail("EGL RGBA config");
            _context = EGL14.EglCreateContext(
                _display,
                configs[0],
                EGL14.EglNoContext,
                [0x3098, 2, 0x3038],
                0
            );
            _reader = ImageReader.NewInstance(
                _width,
                _height,
                (ImageFormatType)1,
                4,
                (long)(
                    HardwareBufferUsage.UsageGpuSampledImage
                    | HardwareBufferUsage.UsageGpuColorOutput
                )
            );
            _reader.SetOnImageAvailableListener(this, _handler);
            _output = EGL14.EglCreateWindowSurface(
                _display,
                configs[0],
                _reader.Surface,
                [0x3038],
                0
            );
            if (!EGL14.EglMakeCurrent(_display, _output, _output, _context))
                Fail("EGL producer context");
            int[] textures = new int[1];
            GLES20.GlGenTextures(1, textures, 0);
            _texture = textures[0];
            GLES20.GlBindTexture(0x8D65, _texture);
            GLES20.GlTexParameteri(0x8D65, GLES20.GlTextureMinFilter, GLES20.GlLinear);
            GLES20.GlTexParameteri(0x8D65, GLES20.GlTextureMagFilter, GLES20.GlLinear);
            GLES20.GlTexParameteri(0x8D65, GLES20.GlTextureWrapS, GLES20.GlClampToEdge);
            GLES20.GlTexParameteri(0x8D65, GLES20.GlTextureWrapT, GLES20.GlClampToEdge);
            _input = new SurfaceTexture(_texture);
            _input.SetDefaultBufferSize(_width, _height);
            _input.SetOnFrameAvailableListener(this, _handler);
            InputSurface = new Surface(_input);
            var vertex = Shader(
                GLES20.GlVertexShader,
                "attribute vec2 p; attribute vec2 uv; uniform mat4 transform; varying vec2 tex; void main(){ gl_Position=vec4(p,0.0,1.0); tex=(transform*vec4(uv,0.0,1.0)).xy; }"
            );
            var fragment = Shader(
                GLES20.GlFragmentShader,
                "#extension GL_OES_EGL_image_external : require\nprecision mediump float; uniform samplerExternalOES source; varying vec2 tex; void main(){ gl_FragColor=texture2D(source,tex); }"
            );
            _program = GLES20.GlCreateProgram();
            GLES20.GlAttachShader(_program, vertex);
            GLES20.GlAttachShader(_program, fragment);
            GLES20.GlLinkProgram(_program);
            GLES20.GlDeleteShader(vertex);
            GLES20.GlDeleteShader(fragment);
            int[] linked = new int[1];
            GLES20.GlGetProgramiv(_program, GLES20.GlLinkStatus, linked, 0);
            if (linked[0] == 0)
                throw new InvalidOperationException(GLES20.GlGetProgramInfoLog(_program));
            _vertices = ByteBuffer
                .AllocateDirect(16 * sizeof(float))!
                .Order(ByteOrder.NativeOrder()!)!
                .AsFloatBuffer();
            // SurfaceTexture supplies the producer transform (including its Y flip).
            // The RGBA buffer is sampled by Vulkan with top-left image coordinates.
            _vertices!.Put([-1f, -1f, 0f, 0f, 1f, -1f, 1f, 0f, -1f, 1f, 0f, 1f, 1f, 1f, 1f, 1f]);
            CheckGl();
        }

        public void OnFrameAvailable(SurfaceTexture? surfaceTexture)
        {
            lock (_gate)
                if (_closed)
                    return;
            try
            {
                _input!.UpdateTexImage();
                _input.GetTransformMatrix(_matrix);
                GLES20.GlViewport(0, 0, _width, _height);
                GLES20.GlUseProgram(_program);
                GLES20.GlActiveTexture(GLES20.GlTexture0);
                GLES20.GlBindTexture(0x8D65, _texture);
                GLES20.GlUniform1i(GLES20.GlGetUniformLocation(_program, "source"), 0);
                GLES20.GlUniformMatrix4fv(
                    GLES20.GlGetUniformLocation(_program, "transform"),
                    1,
                    false,
                    _matrix,
                    0
                );
                var position = GLES20.GlGetAttribLocation(_program, "p");
                var uv = GLES20.GlGetAttribLocation(_program, "uv");
                _vertices!.Position(0);
                GLES20.GlVertexAttribPointer(position, 2, GLES20.GlFloat, false, 16, _vertices);
                _vertices.Position(2);
                GLES20.GlVertexAttribPointer(uv, 2, GLES20.GlFloat, false, 16, _vertices);
                GLES20.GlEnableVertexAttribArray(position);
                GLES20.GlEnableVertexAttribArray(uv);
                GLES20.GlDrawArrays(GLES20.GlTriangleStrip, 0, 4);
                CheckGl();
                if (!EGL14.EglSwapBuffers(_display, _output))
                    Fail("EGL native texture publication");
            }
            catch (Exception e)
            {
                Report(e);
            }
        }

        public void OnImageAvailable(ImageReader? reader)
        {
            AndroidImage? image = null;
            try
            {
                lock (_gate)
                    if (_closed)
                        return;
                image = reader!.AcquireLatestImage();
                if (image is null)
                    return;
                // Explicit bounded producer-fence wait on the producer thread, never the UI/raster thread.
                using (var fence = image.Fence)
                    if (fence is not null && !fence.Await(Java.Time.Duration.OfMillis(1000)!))
                        throw new TimeoutException("Native texture acquire fence timed out.");
                var buffer =
                    image.HardwareBuffer
                    ?? throw new InvalidOperationException(
                        "ImageReader returned no hardware buffer."
                    );
                var frame = new Frame(image, buffer, this);
                image = null;
                lock (_gate)
                {
                    _liveFrames++;
                    if (_closed)
                        frame.Dispose();
                    else
                    {
                        _pending?.Dispose();
                        _pending = frame;
                    }
                }
                Interlocked.Increment(ref Received);
                Registration?.MarkFrameAvailable();
            }
            catch (Exception e)
            {
                Report(e);
            }
            finally
            {
                image?.Close();
                image?.Dispose();
            }
        }

        public void Draw(
            SKCanvas canvas,
            SKRect destination,
            SKSamplingOptions sampling,
            bool freeze
        )
        {
            Frame? frame;
            lock (_gate)
            {
                if (_closed)
                    return;
                if (_pending is not null && (!freeze || _current is null))
                {
                    _current?.Dispose();
                    _current = _pending;
                    _pending = null;
                }
                frame = _current?.Retain();
            }
            if (frame is null)
                return;
            if (!_surface.SupportsHardwareBuffer)
            {
                frame.Dispose();
                Error = "This Vulkan device does not support Android hardware-buffer import.";
                return;
            }
            _surface.DrawHardwareBuffer(
                canvas,
                frame.Handle,
                _width,
                _height,
                destination,
                sampling,
                frame.Dispose
            );
            Interlocked.Increment(ref Drawn);
        }

        private void Report(Exception exception)
        {
            Error = exception.Message;
            Android.Util.Log.Error("DorotiTexture", exception.ToString());
        }

        private static void Fail(string step) =>
            throw new InvalidOperationException($"{step}: EGL 0x{EGL14.EglGetError():x}");

        private static void CheckGl()
        {
            var error = GLES20.GlGetError();
            if (error != 0)
                throw new InvalidOperationException($"Texture GLES error 0x{error:x}");
        }

        private static int Shader(int kind, string source)
        {
            var shader = GLES20.GlCreateShader(kind);
            GLES20.GlShaderSource(shader, source);
            GLES20.GlCompileShader(shader);
            int[] compiled = new int[1];
            GLES20.GlGetShaderiv(shader, GLES20.GlCompileStatus, compiled, 0);
            if (compiled[0] == 0)
            {
                var error = GLES20.GlGetShaderInfoLog(shader);
                GLES20.GlDeleteShader(shader);
                throw new InvalidOperationException(error);
            }
            return shader;
        }

        public new void Dispose()
        {
            lock (_gate)
            {
                if (_closed)
                    return;
                _closed = true;
                _pending?.Dispose();
                _current?.Dispose();
                _pending = _current = null;
            }
            _handler.Post(Cleanup);
        }

        private void FrameReleased()
        {
            bool cleanup;
            lock (_gate)
            {
                _liveFrames--;
                cleanup = _closed;
            }
            if (cleanup)
                _handler.Post(Cleanup);
        }

        private void Cleanup()
        {
            lock (_gate)
            {
                if (!_closed || _cleaned || _liveFrames != 0)
                    return;
                _cleaned = true;
            }
            // The reader stays alive until every Vulkan source lease has retired.
            _input?.SetOnFrameAvailableListener(null);
            _reader?.SetOnImageAvailableListener(null, null);
            GLES20.GlFinish();
            InputSurface?.Release();
            InputSurface?.Dispose();
            InputSurface = null;
            _input?.Release();
            _input?.Dispose();
            if (_program != 0)
                GLES20.GlDeleteProgram(_program);
            if (_texture != 0)
                GLES20.GlDeleteTextures(1, [_texture], 0);
            if (_display is not null)
            {
                EGL14.EglMakeCurrent(
                    _display,
                    EGL14.EglNoSurface,
                    EGL14.EglNoSurface,
                    EGL14.EglNoContext
                );
                if (_output is not null)
                    EGL14.EglDestroySurface(_display, _output);
                if (_context is not null)
                    EGL14.EglDestroyContext(_display, _context);
                EGL14.EglTerminate(_display);
                EGL14.EglReleaseThread();
            }
            _reader?.Close();
            _reader?.Dispose();
            _vertices?.Dispose();
            _thread.QuitSafely();
        }

        private sealed class Frame(AndroidImage image, HardwareBuffer buffer, Producer owner)
            : IDisposable
        {
            private int _references = 1;
            internal nint Handle => FromJava(JNIEnv.Handle, buffer.Handle);

            internal Frame Retain()
            {
                Interlocked.Increment(ref _references);
                return this;
            }

            public void Dispose()
            {
                if (Interlocked.Decrement(ref _references) != 0)
                    return;
                buffer.Close();
                buffer.Dispose();
                image.Close();
                image.Dispose();
                owner.FrameReleased();
            }
        }
    }

    [DllImport("android", EntryPoint = "AHardwareBuffer_fromHardwareBuffer")]
    private static extern nint FromJava(nint environment, nint buffer);
}
#endif
