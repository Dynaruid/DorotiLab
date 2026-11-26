using System.Globalization;
using System.Runtime.CompilerServices;
using Doroti.Framework.Painting;
using Doroti.Framework.Services;
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

internal static class RenderSemanticsTextGeometry
{
    private static long _revision;
    // A TextPainter owns its layout on the framework thread. Keep one immutable
    // geometry snapshot per painter, without extending the painter's lifetime.
    private static readonly ConditionalWeakTable<TextPainter, Cache> Geometry = new();
    private sealed class Cache
    {
        internal long LayoutRevision;
        internal Offset Offset = Offset.zero;
        internal SemanticsTextGeometry? Snapshot;
    }
    internal static SemanticsTextGeometry? Capture(TextPainter painter, Offset offset, bool obscured = false)
    {
        if (obscured || !painter.hasLayoutGeometry)
        {
            Geometry.Remove(painter);
            return null;
        }
        if (Geometry.TryGetValue(painter, out var cached)
            && cached.LayoutRevision == painter.layoutGeometryRevision && cached.Offset == offset)
            return cached.Snapshot;
        var text = painter.plainText;
        if (text.Length > 32768)
        {
            Geometry.Remove(painter);
            return null; // Explicit bounded geometry; oversized text has no TextPattern.
        }
        var revision = Interlocked.Increment(ref _revision);
        var snapshot = painter.CaptureHostTextGeometry(text, revision, offset)
            ?? CaptureFallback(painter, text, revision, offset);
        var cache = Geometry.GetValue(painter, static _ => new Cache());
        cache.LayoutRevision = painter.layoutGeometryRevision;
        cache.Offset = offset;
        cache.Snapshot = snapshot;
        return snapshot;
    }

    private static SemanticsTextGeometry CaptureFallback(TextPainter painter, string text, long revision, Offset offset)
    {
        var boundaries = StringInfo.ParseCombiningCharacters(text).Append(text.Length).ToArray();
        var runs = new List<SemanticsTextRun>();
        for (var i = 0; i < boundaries.Length - 1; i++)
        {
            var start = boundaries[i]; var end = boundaries[i + 1];
            var line = painter.getLineBoundary(new TextPosition(offset: start));
            foreach (var box in painter.getBoxesForSelection(new TextSelection(baseOffset: start, extentOffset: end)))
                runs.Add(new(start, end, box.toRect().shift(offset), checked((int)line.start), checked((int)line.end), box.direction));
        }
        return new SemanticsTextGeometry(text, revision, runs);
    }

    internal static void ShowRange(SemanticsTextGeometry? geometry, int start, int end, Action<Rect> show)
    {
        if (geometry is null || start < 0 || end < start || end > geometry.Text.Length) return;
        var bounds = geometry.Bounds(geometry.Snap(start), geometry.Snap(end, true));
        if (bounds.Count > 0) show(bounds.Aggregate((a, b) => a.expandToInclude(b)));
    }
}
