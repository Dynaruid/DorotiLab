using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Skia.Rendering;

public sealed partial class SkiaSceneRenderer
{
    public void EnableNativeTextures(
        NativeTexturePlatform platform,
        Func<long>? adapterLuid = null
    ) => _textures.NativeFactory = () => new SkiaNativeTextureEntry(this, platform, adapterLuid);
}

internal sealed class SkiaNativeTextureEntry : NativeTextureEntry
{
    private readonly Source _source;
    private readonly SkiaExternalTextureRegistration _registration;
    private readonly NativeTexturePlatform _platform;
    private readonly Func<long>? _adapterLuid;

    internal SkiaNativeTextureEntry(
        SkiaSceneRenderer renderer,
        NativeTexturePlatform platform,
        Func<long>? adapterLuid
    )
    {
        _platform = platform;
        _adapterLuid = adapterLuid;
        _source = new Source(renderer.Textures);
        _registration = renderer.RegisterExternalTexture(_source);
    }

    public override long Id => _registration.Id;
    public override NativeTextureDevice Device => new(_platform, _adapterLuid?.Invoke() ?? 0);

    public override void PushFrame(NativeTextureFrame frame)
    {
        ArgumentNullException.ThrowIfNull(frame);
        if (frame.Buffer.Platform != _platform)
            throw new ArgumentException(
                "Native buffer belongs to another platform.",
                nameof(frame)
            );
        _source.Push(frame);
        _registration.MarkFrameAvailable();
    }

    public override void Dispose() => _registration.Dispose();

    private sealed class Source(object consumer) : ISkiaExternalTextureSource
    {
        private readonly object _gate = new();
        private NativeTextureFrame? _pending,
            _current;
        private bool _disposed;

        internal void Push(NativeTextureFrame frame)
        {
            NativeTextureFrame? dropped;
            lock (_gate)
            {
                ObjectDisposedException.ThrowIf(_disposed, this);
                var owned = frame.RetainForConsumer(consumer);
                dropped = _pending;
                _pending = owned;
            }
            dropped?.Dispose();
        }

        public void Draw(
            SKCanvas canvas,
            SKRect destination,
            SKSamplingOptions sampling,
            bool freeze
        )
        {
            NativeTextureFrame? frame,
                retired = null;
            lock (_gate)
            {
                if (_disposed)
                    return;
                if (_pending is not null && (!freeze || _current is null))
                {
                    retired = _current;
                    _current = _pending;
                    _pending = null;
                }
                frame = _current?.Retain();
            }
            retired?.Dispose();
            using (frame)
                if (frame is not null)
                    (
                        SkiaGraphiteSession.CurrentRecording
                        ?? throw new PlatformNotSupportedException(
                            "Native textures require a Graphite recording."
                        )
                    ).DrawNativeTexture(canvas, frame, destination, sampling);
        }

        public void Dispose()
        {
            NativeTextureFrame? pending,
                current;
            lock (_gate)
            {
                if (_disposed)
                    return;
                _disposed = true;
                pending = _pending;
                current = _current;
                _pending = _current = null;
            }
            pending?.Dispose();
            current?.Dispose();
        }
    }
}
