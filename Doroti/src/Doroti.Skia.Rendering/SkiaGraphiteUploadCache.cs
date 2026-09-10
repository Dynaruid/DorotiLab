using SkiaSharp;

namespace Doroti.Skia.Rendering;

// Recorder-thread only. Keep submitted uploads when a later recording is
// discarded; only entries first uploaded by that recording are invalid.
internal sealed class SkiaGraphiteUploadCache : IDisposable
{
    private const int Capacity = 256;
    private readonly Dictionary<(uint Id, bool Mipmapped), Entry> _entries = [];
    private readonly LinkedList<(uint Id, bool Mipmapped)> _lru = [];
    private readonly HashSet<(uint Id, bool Mipmapped)> _pending = [];
    internal long Uploads { get; private set; }
    internal long Hits { get; private set; }
    internal long Discarded { get; private set; }

    internal SKImage? FindOrCreate(SKGraphiteRecorder recorder, SKImage image, bool mipmapped)
    {
        var key = (image.UniqueId, mipmapped);
        if (_entries.TryGetValue(key, out var entry))
        {
            _lru.Remove(entry.Node);
            _lru.AddLast(entry.Node);
            Hits++;
            return entry.Cache.FindOrCreate(recorder, image, mipmapped);
        }
        // Use SkiaSharp's cache for its native reference-transfer contract. Each
        // shard contains one image so cancellation can release only new uploads.
        var cache = new SKGraphiteImageCache();
        var uploaded = cache.FindOrCreate(recorder, image, mipmapped);
        if (uploaded is null) { cache.Dispose(); return null; }
        while (_entries.Count >= Capacity) Remove(_lru.First!.Value);
        _entries.Add(key, new(cache, _lru.AddLast(key)));
        _pending.Add(key);
        Uploads++;
        return uploaded;
    }

    internal void Commit() => _pending.Clear();
    internal void Cancel()
    {
        foreach (var key in _pending.ToArray()) { Remove(key); Discarded++; }
    }
    private void Remove((uint Id, bool Mipmapped) key)
    {
        if (_entries.Remove(key, out var entry)) { _lru.Remove(entry.Node); entry.Cache.Dispose(); }
        _pending.Remove(key);
    }
    public void Dispose()
    {
        foreach (var entry in _entries.Values) entry.Cache.Dispose();
        _entries.Clear(); _lru.Clear(); _pending.Clear();
    }
    private sealed record Entry(SKGraphiteImageCache Cache, LinkedListNode<(uint Id, bool Mipmapped)> Node);
}
