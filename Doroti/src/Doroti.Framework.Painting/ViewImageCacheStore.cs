using System.Runtime.CompilerServices;
using Doroti.Ui;
using Doroti.Runtime;

namespace Doroti.Framework.Painting;

/// <summary>Decoded image storage belongs to its view/render consumer, while
/// immutable application asset bytes can still be shared by the provider.</summary>
public sealed class ViewImageCacheStore(Func<ImageCache> factory) : IDisposable
{
    private sealed class Retired;
    private readonly Dictionary<DorotiView, ImageCache> _caches = new(ReferenceEqualityComparer.Instance);
    private readonly ConditionalWeakTable<DorotiView, Retired> _retired = new();
    private bool _disposed;

    private DorotiView CurrentView()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var view = PlatformDispatcher.instance.RequireInvocationView(new("view-image-cache"));
        if (_retired.TryGetValue(view, out _)) throw new ObjectDisposedException("View image cache");
        return view;
    }

    public ImageCache Current
    {
        get
        {
            var view = CurrentView();
            if (!_caches.TryGetValue(view, out var cache))
            {
                cache = factory();
                if (_caches.Values.Any(existing => ReferenceEquals(existing, cache)))
                    throw new InvalidOperationException("Decoded image caches cannot be shared between rendering views.");
                _caches.Add(view, cache);
            }
            return cache;
        }
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            var view = CurrentView();
            if (_caches.Any(pair => !ReferenceEquals(pair.Key, view) && ReferenceEquals(pair.Value, value)))
                throw new InvalidOperationException("Decoded image caches cannot be shared between rendering views.");
            if (_caches.TryGetValue(view, out var previous) && !ReferenceEquals(previous, value)) previous.Dispose();
            _caches[view] = value;
        }
    }

    public void Release(DorotiView view)
    {
        _retired.GetValue(view, _ => new());
        if (_caches.Remove(view, out var cache)) cache.Dispose();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        var caches = _caches.Values.ToArray();
        _caches.Clear();
        DorotiCleanup.Run(caches.Select<ImageCache, Action>(cache => cache.Dispose).ToArray());
    }
}
