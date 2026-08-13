using Doroti.Graphics;

namespace Doroti.Composition;

public sealed record DecodedImage(int Width, int Height, byte[] Bgra8888Pixels)
{
    public Size Size => new(Width, Height);

    public void Validate()
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(Width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(Height);
        ArgumentNullException.ThrowIfNull(Bgra8888Pixels);
        if (Bgra8888Pixels.Length != checked(Width * Height * 4))
        {
            throw new InvalidDataException("Decoded image must contain tightly packed BGRA8888 pixels.");
        }
    }
}

public interface IImageProvider
{
    string CacheKey { get; }

    ValueTask<DecodedImage> LoadAsync(CancellationToken cancellationToken);
}

public interface IImageDecoder
{
    ValueTask<DecodedImage> DecodeAsync(ReadOnlyMemory<byte> encodedBytes, CancellationToken cancellationToken = default);
}

public sealed class EncodedImageProvider : IImageProvider
{
    private readonly ReadOnlyMemory<byte> _encodedBytes;
    private readonly IImageDecoder _decoder;

    public EncodedImageProvider(string cacheKey, ReadOnlyMemory<byte> encodedBytes, IImageDecoder decoder)
    {
        CacheKey = string.IsNullOrWhiteSpace(cacheKey)
            ? throw new ArgumentException("An image cache key is required.", nameof(cacheKey))
            : cacheKey;
        if (encodedBytes.IsEmpty)
        {
            throw new ArgumentException("Encoded image bytes are required.", nameof(encodedBytes));
        }
        _encodedBytes = encodedBytes;
        _decoder = decoder ?? throw new ArgumentNullException(nameof(decoder));
    }

    public string CacheKey { get; }

    public ValueTask<DecodedImage> LoadAsync(CancellationToken cancellationToken) =>
        _decoder.DecodeAsync(_encodedBytes, cancellationToken);
}

public sealed class RawBgraImageProvider(string cacheKey, DecodedImage image) : IImageProvider
{
    public string CacheKey { get; } = string.IsNullOrWhiteSpace(cacheKey)
        ? throw new ArgumentException("An image cache key is required.", nameof(cacheKey))
        : cacheKey;

    public ValueTask<DecodedImage> LoadAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        image.Validate();
        return ValueTask.FromResult(image);
    }
}

public sealed class ImageCache(ResourceRegistry registry) : IDisposable
{
    private readonly object _gate = new();
    private readonly Dictionary<string, CacheEntry> _entries = new(StringComparer.Ordinal);
    private bool _disposed;

    public int Count
    {
        get
        {
            lock (_gate)
            {
                return _entries.Count;
            }
        }
    }

    public async ValueTask<ImageLease> ResolveAsync(IImageProvider provider, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(provider);
        Task<DecodedImage> load;
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (!_entries.TryGetValue(provider.CacheKey, out var entry))
            {
                entry = new(provider.LoadAsync(CancellationToken.None).AsTask());
                _entries.Add(provider.CacheKey, entry);
            }
            load = entry.Load;
        }

        DecodedImage decoded;
        try
        {
            decoded = await load.WaitAsync(cancellationToken).ConfigureAwait(false);
            decoded.Validate();
        }
        catch
        {
            lock (_gate)
            {
                if (_entries.TryGetValue(provider.CacheKey, out var failed) && ReferenceEquals(failed.Load, load) && failed.RefCount == 0)
                {
                    _entries.Remove(provider.CacheKey);
                }
            }
            throw;
        }

        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            var entry = _entries[provider.CacheKey];
            entry.Resource ??= registry.RegisterImage(decoded.Width, decoded.Height, decoded.Bgra8888Pixels);
            entry.Size = decoded.Size;
            entry.RefCount++;
            return new(this, provider.CacheKey, entry.Resource.Value, entry.Size);
        }
    }

    public bool Evict(string cacheKey)
    {
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (!_entries.TryGetValue(cacheKey, out var entry))
            {
                return false;
            }
            entry.EvictWhenUnused = true;
            return TryEvict(cacheKey, entry);
        }
    }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }
            if (_entries.Values.Any(entry => entry.RefCount != 0))
            {
                throw new InvalidOperationException("Cannot dispose ImageCache while image leases are active.");
            }
            foreach (var entry in _entries.Values)
            {
                if (entry.Resource is { } resource)
                {
                    registry.Remove(resource);
                }
            }
            _entries.Clear();
            _disposed = true;
        }
    }

    private void Release(string key)
    {
        lock (_gate)
        {
            if (!_entries.TryGetValue(key, out var entry) || entry.RefCount == 0)
            {
                throw new InvalidOperationException("Image lease was released more than once.");
            }
            entry.RefCount--;
            if (entry.EvictWhenUnused)
            {
                TryEvict(key, entry);
            }
        }
    }

    private bool TryEvict(string key, CacheEntry entry)
    {
        if (entry.RefCount != 0)
        {
            return false;
        }
        if (entry.Resource is { } resource)
        {
            try
            {
                if (!registry.Remove(resource))
                {
                    return false;
                }
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
        return _entries.Remove(key);
    }

    private sealed class CacheEntry(Task<DecodedImage> load)
    {
        internal Task<DecodedImage> Load { get; } = load;

        internal ResourceId? Resource { get; set; }

        internal Size Size { get; set; }

        internal int RefCount { get; set; }

        internal bool EvictWhenUnused { get; set; }
    }

    public sealed class ImageLease : IDisposable
    {
        private readonly ImageCache _owner;
        private readonly string _key;
        private int _disposed;

        internal ImageLease(ImageCache owner, string key, ResourceId resource, Size size)
        {
            _owner = owner;
            _key = key;
            Resource = resource;
            Size = size;
        }

        public ResourceId Resource { get; }

        public Size Size { get; }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
            {
                _owner.Release(_key);
            }
        }
    }
}
