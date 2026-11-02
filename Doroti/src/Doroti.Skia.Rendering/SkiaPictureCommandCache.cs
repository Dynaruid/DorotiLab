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
    private readonly Dictionary<long, PictureCommandEntry> _pictureCommandCache = new();
    private readonly LinkedList<long> _pictureCommandOrder = new();
    private int _pictureCommandCount;
    private long _pictureCommandBytes;
    private long _pictureCommandHits,
        _pictureCommandRecordings;

    private void DrawRetainedPicture(SKCanvas canvas, ScenePicturePayload payload)
    {
        var commands = payload.Commands;
        // Freeze copies the command storage for every submission. The picture's
        // immutable recording identity survives that copy and owns cache reuse.
        var cacheKey = payload.SnapshotIdentity;
        if (
            payload.WillChangeHint
            || commands.Count == 0
            || commands.Count > MaxRetainedPictureCommands
            || HasBlurredPaint(commands)
            || payload.CanvasBounds is not { } bounds
            || !bounds.IsFinite
            || bounds.isEmpty
        )
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
        _pictureCommandCache.TryGetValue(cacheKey, out var entry);
        SKMatrix matrix = default;
        if (entry is not null && (entry.Bounds != cullRect
            || entry.DeviceBasis is { } previousBasis
                && (!TryPictureDeviceBasis(matrix = canvas.TotalMatrix, out var currentBasis)
                    || currentBasis != previousBasis || entry.ShadowDeviceScale != _shadowDeviceScale)))
        {
            RemovePictureCommands(cacheKey);
            entry = null;
        }
        if (entry is null)
        {
            // Unbounded color/paint operations need the real destination.
            var hasShadow = false;
            var maximumElevation = 0.0;
            foreach (var command in commands)
            {
                if (command.Operation is "drawColor" or "drawPaint")
                {
                    DrawPicture(canvas, commands);
                    return;
                }
                if (command.HostPayload is CanvasShadowPayload shadow)
                {
                    hasShadow = true;
                    maximumElevation = Math.Max(maximumElevation, shadow.Elevation);
                }
            }
            SKMatrix? deviceBasis = null;
            var recordingBounds = cullRect;
            if (hasShadow)
            {
                // Shadows map their path into device space and reset the CTM.
                // Record that same linear transform, then replay under only
                // the destination translation. Fractional scrolling remains
                // vector playback, without resampling cached shadow/text pixels.
                matrix = canvas.TotalMatrix;
                if (!TryPictureDeviceBasis(matrix, out var basis) || !double.IsFinite(_shadowDeviceScale))
                {
                    DrawPicture(canvas, commands);
                    return;
                }
                deviceBasis = basis;
                recordingBounds = basis.MapRect(cullRect);
                // CanvasBounds contains unfiltered geometry. Cover the spot
                // offset plus blur/stroke outsets used by DrawShadow; otherwise
                // a native picture's cull rect can discard the shadow halo.
                var outset = (float)(4 * maximumElevation * Math.Max(0, _shadowDeviceScale) + 16);
                recordingBounds.Inflate(outset, outset);
                if (!IsFinite(recordingBounds) || recordingBounds.IsEmpty)
                {
                    DrawPicture(canvas, commands);
                    return;
                }
            }
            while (
                _pictureCommandCache.Count >= MaxPictureCommandEntries
                || _pictureCommandCount + commands.Count > MaxRetainedPictureCommands
            )
            {
                RemovePictureCommands(_pictureCommandOrder.First!.Value);
            }

            entry = new(_pictureCommandOrder.AddLast(cacheKey), commands.Count, cullRect,
                recordingBounds, deviceBasis, _shadowDeviceScale);
            _pictureCommandCache.Add(cacheKey, entry);
            _pictureCommandCount += commands.Count;
            DrawPicture(canvas, commands);
            return;
        }

        _pictureCommandOrder.Remove(entry.Node);
        _pictureCommandOrder.AddLast(entry.Node);
        if (entry.Picture is null)
        {
            using var recorder = new SKPictureRecorder();
            var recording = recorder.BeginRecording(entry.RecordingBounds);
            if (entry.DeviceBasis is { } basis) recording.SetMatrix(basis);
            DrawPicture(recording, commands);
            entry.Picture = recorder.EndRecording();
            entry.Bytes = entry.Picture.ApproximateBytesUsed;
            _pictureCommandBytes += entry.Bytes;
            _pictureCommandRecordings++;
        }
        else
        {
            _pictureCommandHits++;
        }

        if (entry.DeviceBasis is null) canvas.DrawPicture(entry.Picture);
        else
        {
            matrix = canvas.TotalMatrix;
            var saved = canvas.Save();
            try
            {
                canvas.ResetMatrix();
                canvas.Translate(matrix.TransX, matrix.TransY);
                canvas.DrawPicture(entry.Picture);
            }
            finally { canvas.RestoreToCount(saved); }
        }
        // Evict only after drawing: an oversized current recording may itself
        // be the victim. Native command bytes exclude referenced image pixels.
        while (_pictureCommandBytes > MaxPictureCommandBytes)
        {
            RemovePictureCommands(_pictureCommandOrder.First!.Value);
        }
    }

    private static bool TryPictureDeviceBasis(SKMatrix matrix, out SKMatrix basis)
    {
        basis = matrix;
        basis.TransX = basis.TransY = 0;
        return matrix.Persp0 == 0 && matrix.Persp1 == 0 && matrix.Persp2 == 1
            && float.IsFinite(matrix.ScaleX) && float.IsFinite(matrix.ScaleY)
            && float.IsFinite(matrix.SkewX) && float.IsFinite(matrix.SkewY)
            && float.IsFinite(matrix.TransX) && float.IsFinite(matrix.TransY);
    }

    private void RemovePictureCommands(long key)
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
        foreach (var entry in _pictureCommandCache.Values)
        {
            entry.Picture?.Dispose();
        }

        _pictureCommandCache.Clear();
        _pictureCommandOrder.Clear();
        _pictureCommandCount = 0;
        _pictureCommandBytes = 0;
    }

    private sealed class PictureCommandEntry(
        LinkedListNode<long> node,
        int commandCount,
        SKRect bounds,
        SKRect recordingBounds,
        SKMatrix? deviceBasis,
        double shadowDeviceScale
    )
    {
        internal readonly LinkedListNode<long> Node = node;
        internal readonly int CommandCount = commandCount;
        internal readonly SKRect Bounds = bounds;
        internal readonly SKRect RecordingBounds = recordingBounds;
        internal readonly SKMatrix? DeviceBasis = deviceBasis;
        internal readonly double ShadowDeviceScale = shadowDeviceScale;
        internal SKPicture? Picture;
        internal long Bytes;
    }

    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<
        object,
        BlurPolicy
    > BlurPolicies = new();

    // CanvasBounds describes the unfiltered geometry. Until filter outsets are
    // tracked through transforms, replay blurred pictures on the destination:
    // both a recording's cull rect and a raster cache can otherwise cut off halos.
    private static bool HasBlurredPaint(IReadOnlyList<PathCommand> commands) =>
        BlurPolicies
            .GetValue(
                commands,
                static key =>
                    new(
                        ((IReadOnlyList<PathCommand>)key).Any(static command =>
                            (
                                command.HostPayload switch
                                {
                                    PaintSnapshot paint => paint,
                                    CanvasSaveLayerPayload layer => layer.Paint,
                                    CanvasPathPayload draw => draw.Paint,
                                    CanvasRectPayload draw => draw.Paint,
                                    CanvasRRectPayload draw => draw.Paint,
                                    CanvasRSuperellipsePayload draw => draw.Paint,
                                    CanvasDRRectPayload draw => draw.Paint,
                                    CanvasImagePayload draw => draw.Paint,
                                    CanvasImageNinePayload draw => draw.Paint,
                                    CanvasCirclePayload draw => draw.Paint,
                                    CanvasLinePayload draw => draw.Paint,
                                    CanvasPointsPayload draw => draw.Paint,
                                    CanvasOvalPayload draw => draw.Paint,
                                    CanvasArcPayload draw => draw.Paint,
                                    _ => null,
                                }
                            )?.MaskFilter
                                is { sigma: > 0 }
                        )
                    )
            )
            .HasBlur;

    private sealed record BlurPolicy(bool HasBlur);
}
