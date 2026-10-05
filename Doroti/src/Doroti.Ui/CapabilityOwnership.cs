namespace Doroti.Ui;

public enum DorotiCapabilityOwnership
{
    Owned,
    Borrowed,
}

/// <summary>The provider can register implementations, but cannot seal or dispose the view.</summary>
public interface IDorotiCapabilityRegistrar
{
    void Register<TCapability>(string id, TCapability capability, DorotiCapabilityOwnership ownership)
        where TCapability : class;
}

/// <summary>Application-owned services outlive their borrowing views and active calls.</summary>
public sealed class DorotiCapabilityLifetime : IDisposable
{
    private sealed class Registration(object owner)
    {
        public object Owner { get; } = owner;
        public HashSet<object> Borrowers { get; } = new(ReferenceEqualityComparer.Instance);
        public int ActiveCalls { get; set; }
    }

    private readonly object _gate = new();
    private readonly Dictionary<object, Registration> _services = new(ReferenceEqualityComparer.Instance);
    private bool _disposed;

    public TCapability Own<TCapability>(TCapability capability) where TCapability : class
    {
        ArgumentNullException.ThrowIfNull(capability);
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (!_services.TryAdd(capability, new(this)))
                throw new InvalidOperationException("A capability already has an owner in this lifetime.");
        }
        return capability;
    }

    internal void Register(object registry, object capability, DorotiCapabilityOwnership ownership)
    {
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (ownership == DorotiCapabilityOwnership.Owned)
            {
                if (_services.TryGetValue(capability, out var existing))
                {
                    if (!ReferenceEquals(existing.Owner, registry))
                        throw new InvalidOperationException("A capability cannot be owned by two scopes.");
                }
                else _services.Add(capability, new(registry));
            }
            else
            {
                if (!_services.TryGetValue(capability, out var service) || !ReferenceEquals(service.Owner, this))
                    throw new InvalidOperationException("Borrowed capabilities require a live application owner.");
                service.Borrowers.Add(registry);
            }
        }
    }

    public DorotiCapabilityLease<TCapability> AcquireOwned<TCapability>(TCapability capability) where TCapability : class
    {
        ArgumentNullException.ThrowIfNull(capability);
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (!_services.TryGetValue(capability, out var service) || !ReferenceEquals(service.Owner, this))
                throw new InvalidOperationException("Typed invocation requires an application-owned handler.");
            return new(capability, Acquire(capability));
        }
    }

    internal Action Acquire(object capability)
    {
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (!_services.TryGetValue(capability, out var service))
                throw new ObjectDisposedException("capability");
            service.ActiveCalls++;
        }
        return () =>
        {
            lock (_gate) _services[capability].ActiveCalls--;
        };
    }

    internal void ValidateRelease(object registry, IEnumerable<object> capabilities)
    {
        lock (_gate)
        {
            foreach (var capability in capabilities)
                if (_services.TryGetValue(capability, out var service) &&
                    ReferenceEquals(service.Owner, registry) && service.ActiveCalls != 0)
                    throw new InvalidOperationException("Drain active capability calls before disposing the view.");
        }
    }

    internal void Release(object registry, object capability, DorotiCapabilityOwnership ownership)
    {
        lock (_gate)
        {
            if (!_services.TryGetValue(capability, out var service)) return;
            if (ownership == DorotiCapabilityOwnership.Borrowed) service.Borrowers.Remove(registry);
            else _services.Remove(capability);
        }
    }

    public void Dispose()
    {
        IDisposable[] owned;
        lock (_gate)
        {
            if (_disposed) return;
            if (_services.Values.Any(service => service.Borrowers.Count != 0 || service.ActiveCalls != 0 || !ReferenceEquals(service.Owner, this)))
                throw new InvalidOperationException("Drain views and active calls before disposing application capabilities.");
            _disposed = true;
            owned = _services.Keys.OfType<IDisposable>().Reverse().ToArray();
            _services.Clear();
        }
        Doroti.Runtime.DorotiCleanup.Run(owned.Select<IDisposable, Action>(service => service.Dispose).ToArray());
    }
}

/// <summary>Retains a service during a typed invocation. Disposal is idempotent.</summary>
public sealed class DorotiCapabilityLease<TCapability> : IDisposable where TCapability : class
{
    private Action? _release;
    internal DorotiCapabilityLease(TCapability value, Action release)
    {
        Value = value;
        _release = release;
    }
    public TCapability Value { get; }
    public void Dispose() => Interlocked.Exchange(ref _release, null)?.Invoke();
}
