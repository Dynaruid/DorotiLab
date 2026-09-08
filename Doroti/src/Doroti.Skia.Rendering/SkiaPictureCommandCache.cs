using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Skia.Rendering;

public sealed partial class SkiaSceneRenderer
{
    // Retain native drawing commands, not pixels: fractional translation, scale,
    // clipping and destination surface font policy are applied at playback time.
    // A second use pays the recording cost; one-frame animation pictures do not.
    private const int MaxPictureCommandEntries = 128;
    private const int MaxRetainedPictureCommands = 32768;
    private const long MaxPictureCommandBytes = 4 * 1024 * 1024;
    private readonly Dictionary<object, PictureCommandEntry> _pictureCommandCache =
        new(ReferenceEqualityComparer.Instance);
    private readonly LinkedList<object> _pictureCommandOrder = new();
    private int _pictureCommandCount;
    private long _pictureCommandBytes;
    private long _pictureCommandHits, _pictureCommandRecordings;

    private void DrawRetainedPicture(SKCanvas canvas, ScenePicturePayload payload)
    {
        var commands = payload.Commands;
        if (payload.WillChangeHint || commands.Count == 0 || commands.Count > MaxRetainedPictureCommands ||
            payload.CanvasBounds is not { } bounds || !bounds.IsFinite || bounds.isEmpty)
        {
            DrawPicture(canvas, commands);
            return;
        }

        var cullRect = ToRect(bounds);
        if (!IsFinite(cullRect))
        {
            DrawPicture(canvas, commands);
            return;
        }
        if (_pictureCommandCache.TryGetValue(commands, out var entry) && entry.Bounds != cullRect)
        {
            RemovePictureCommands(commands);
            entry = null;
        }
        if (entry is null)
        {
            // DrawShadow maps into device space and resets the matrix; recording
            // it at identity would change shadow geometry at another transform.
            // Unbounded color/paint operations also need the real destination.
            if (commands.Any(static command => command.Operation is "drawShadow" or "drawColor" or "drawPaint"))
            {
                DrawPicture(canvas, commands);
                return;
            }
            while (_pictureCommandCache.Count >= MaxPictureCommandEntries ||
                   _pictureCommandCount + commands.Count > MaxRetainedPictureCommands)
                RemovePictureCommands(_pictureCommandOrder.First!.Value);
            entry = new(_pictureCommandOrder.AddLast(commands), commands.Count, cullRect);
            _pictureCommandCache.Add(commands, entry);
            _pictureCommandCount += commands.Count;
            DrawPicture(canvas, commands);
            return;
        }

        _pictureCommandOrder.Remove(entry.Node);
        _pictureCommandOrder.AddLast(entry.Node);
        if (entry.Picture is null)
        {
            using var recorder = new SKPictureRecorder();
            var recording = recorder.BeginRecording(cullRect);
            DrawPicture(recording, commands);
            entry.Picture = recorder.EndRecording();
            entry.Bytes = entry.Picture.ApproximateBytesUsed;
            _pictureCommandBytes += entry.Bytes;
            _pictureCommandRecordings++;
        }
        else _pictureCommandHits++;
        canvas.DrawPicture(entry.Picture);
        // Evict only after drawing: an oversized current recording may itself
        // be the victim. Native command bytes exclude referenced image pixels.
        while (_pictureCommandBytes > MaxPictureCommandBytes)
            RemovePictureCommands(_pictureCommandOrder.First!.Value);
    }

    private void RemovePictureCommands(object key)
    {
        var entry = _pictureCommandCache[key];
        _pictureCommandCache.Remove(key);
        _pictureCommandOrder.Remove(entry.Node);
        _pictureCommandCount -= entry.CommandCount;
        _pictureCommandBytes -= entry.Bytes;
        entry.Picture?.Dispose();
    }

    private void ClearPictureCommandCache()
    {
        foreach (var entry in _pictureCommandCache.Values) entry.Picture?.Dispose();
        _pictureCommandCache.Clear();
        _pictureCommandOrder.Clear();
        _pictureCommandCount = 0;
        _pictureCommandBytes = 0;
    }

    private sealed class PictureCommandEntry(LinkedListNode<object> node, int commandCount, SKRect bounds)
    {
        internal readonly LinkedListNode<object> Node = node;
        internal readonly int CommandCount = commandCount;
        internal readonly SKRect Bounds = bounds;
        internal SKPicture? Picture;
        internal long Bytes;
    }
}
