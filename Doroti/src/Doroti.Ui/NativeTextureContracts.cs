namespace Doroti.Ui;

public enum NativeTexturePlatform
{
    Windows,
    Apple,
    Linux,
}

public enum NativeTextureFormat
{
    Rgba8888,
    Bgra8888,
}

public sealed record NativeTextureDevice(NativeTexturePlatform Platform, long AdapterLuid = 0);

/// <summary>Immutable, producer-complete sRGB GPU buffer metadata with premultiplied alpha.
/// Storage must not change until its final release.</summary>
public abstract record NativeTextureBuffer(int Width, int Height, NativeTextureFormat Format)
{
    public abstract NativeTexturePlatform Platform { get; }
}

/// <summary>Reference-counted native ownership. Each retained handle must be disposed once.
/// release runs on the final releasing thread and must not throw; marshal it to the producer if necessary.</summary>
public sealed class NativeTextureFrame : IDisposable
{
    private sealed class Storage(NativeTextureBuffer buffer, Action release)
    {
        internal readonly NativeTextureBuffer Buffer = buffer;
        internal readonly Action Release = release;
        internal int References = 1;
        internal object? Consumer;
    }

    private readonly Storage _storage;
    private readonly object _gate = new();
    private bool _disposed;

    public NativeTextureFrame(NativeTextureBuffer buffer, Action release)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentNullException.ThrowIfNull(release);
        if (
            buffer.Width is < 1 or > 8192
            || buffer.Height is < 1 or > 8192
            || !Enum.IsDefined(buffer.Format)
        )
            throw new ArgumentOutOfRangeException(nameof(buffer));
        _storage = new(buffer, release);
    }

    private NativeTextureFrame(Storage storage)
    {
        _storage = storage;
        Interlocked.Increment(ref storage.References);
    }

    public NativeTextureBuffer Buffer => _storage.Buffer;

    public NativeTextureFrame Retain()
    {
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return new(_storage);
        }
    }

    /// <summary>Infrastructure: a frame allocation can belong to one rendering consumer only.
    /// Multiple views must use separate native allocations or perform an explicit producer GPU copy.</summary>
    public NativeTextureFrame RetainForConsumer(object consumer)
    {
        ArgumentNullException.ThrowIfNull(consumer);
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            lock (_storage)
            {
                if (_storage.Consumer is not null && !ReferenceEquals(_storage.Consumer, consumer))
                    throw new InvalidOperationException(
                        "A native allocation cannot be imported by multiple rendering consumers."
                    );
                _storage.Consumer = consumer;
                return new(_storage);
            }
        }
    }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed)
                return;
            _disposed = true;
        }
        if (Interlocked.Decrement(ref _storage.References) == 0)
            _storage.Release();
    }
}

public abstract class NativeTextureEntry : IDisposable
{
    public abstract long Id { get; }
    public abstract NativeTextureDevice Device { get; }

    /// <summary>Retains a producer-complete immutable frame; the caller keeps ownership of its own handle.</summary>
    public abstract void PushFrame(NativeTextureFrame frame);
    public abstract void Dispose();
}
