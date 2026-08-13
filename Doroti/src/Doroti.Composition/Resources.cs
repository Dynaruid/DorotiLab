using System.Collections.Immutable;
using Doroti.Graphics;

namespace Doroti.Composition;

public interface IResourceSnapshot
{
    ResourceId Id { get; }
}

public sealed class ImageResourceSnapshot : IResourceSnapshot
{
    public ImageResourceSnapshot(ResourceId id, int width, int height, ReadOnlySpan<byte> bgra8888Pixels)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);
        var expected = checked(width * height * 4);
        if (bgra8888Pixels.Length != expected)
        {
            throw new ArgumentException($"Expected {expected} BGRA8888 bytes.", nameof(bgra8888Pixels));
        }

        Id = id;
        Width = width;
        Height = height;
        Pixels = ImmutableArray.Create(bgra8888Pixels.ToArray());
    }

    public ResourceId Id { get; }

    public int Width { get; }

    public int Height { get; }

    public Size Size => new(Width, Height);

    public ImmutableArray<byte> Pixels { get; }
}

public sealed class ResourceRegistry : IResourceRegistry, IDisposable
{
    private readonly object _gate = new();
    private readonly Dictionary<ResourceId, Entry> _entries = [];
    private ulong _nextId = 1;
    private bool _disposed;

    public int RegisteredCount
    {
        get
        {
            lock (_gate)
            {
                return _entries.Count;
            }
        }
    }

    public int ActiveLeaseCount
    {
        get
        {
            lock (_gate)
            {
                return _entries.Values.Sum(entry => entry.LeaseCount);
            }
        }
    }

    public ResourceId RegisterImage(int width, int height, ReadOnlySpan<byte> bgra8888Pixels)
    {
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (_nextId == 0)
            {
                throw new InvalidOperationException("Resource identifier overflowed.");
            }
            var id = new ResourceId(_nextId++);
            _entries.Add(id, new(new ImageResourceSnapshot(id, width, height, bgra8888Pixels)));
            return id;
        }
    }

    public bool Remove(ResourceId resource)
    {
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (!_entries.TryGetValue(resource, out var entry))
            {
                return false;
            }
            if (entry.LeaseCount != 0)
            {
                throw new InvalidOperationException($"Resource {resource.Value} still has active frame leases.");
            }
            return _entries.Remove(resource);
        }
    }

    public IResourceLease Retain(ResourceId resource)
    {
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (!_entries.TryGetValue(resource, out var entry))
            {
                throw new KeyNotFoundException($"Resource {resource.Value} is not registered.");
            }
            entry.LeaseCount++;
            return new Lease(this, entry.Snapshot);
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
            if (_entries.Values.Any(entry => entry.LeaseCount != 0))
            {
                throw new InvalidOperationException("Cannot dispose the resource registry while frame leases are active.");
            }
            _disposed = true;
            _entries.Clear();
        }
    }

    private void Release(ResourceId resource)
    {
        lock (_gate)
        {
            if (!_entries.TryGetValue(resource, out var entry) || entry.LeaseCount == 0)
            {
                throw new InvalidOperationException($"Resource {resource.Value} lease was released more than once.");
            }
            entry.LeaseCount--;
        }
    }

    private sealed class Entry(IResourceSnapshot snapshot)
    {
        internal IResourceSnapshot Snapshot { get; } = snapshot;

        internal int LeaseCount { get; set; }
    }

    private sealed class Lease(ResourceRegistry owner, IResourceSnapshot snapshot) : IResourceLease
    {
        private int _disposed;

        public ResourceId Resource => snapshot.Id;

        public IResourceSnapshot Snapshot => snapshot;

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
            {
                owner.Release(Resource);
            }
        }
    }
}
