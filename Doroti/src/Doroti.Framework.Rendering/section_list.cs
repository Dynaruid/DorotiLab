using Doroti.Ui;

namespace Doroti.Framework.Rendering;

/// <summary>Optional notification for an indexed owner to release a cached child
/// without scanning every retained child on each frame.</summary>
public interface KeepAliveReleaseListener
{
    void ReleaseKeepAlive(RenderBox child);
}

/// <summary>Indexed section layout using the standard sliver child manager and
/// keep-alive ownership. No asynchronous work runs inside layout.</summary>
public sealed class RenderSectionList : RenderSliverMultiBoxAdaptor, KeepAliveReleaseListener
{
    private readonly HashSet<RenderBox> _pendingReleases = [];
    private SectionExtentIndex _index;
    private object? _revision;
    private int[] _retain;
    private bool _restore = true;
    private bool _configured;
    private bool _suspended;
    private double _width;
    private object? _configuredRevision;
    public RenderSectionList(RenderSliverBoxChildManager manager, SectionExtentIndex index,
        object? metricRevision, int[] retain, bool suspended = false) : base(manager)
    { _index = index; _revision = metricRevision; _retain = retain; _suspended = suspended; index.Changed = markNeedsLayout; }

    public void Update(SectionExtentIndex index, object? metricRevision, int[] retain,
        bool suspended = false, bool restoreRetainedChildren = false)
    {
        if (!ReferenceEquals(_index, index))
        { if (_index.Changed == markNeedsLayout) _index.Changed = null; _index = index; _restore = true; _configured = false; }
        _index.Changed = markNeedsLayout;
        _revision = metricRevision; _retain = retain; markNeedsLayout();
        _suspended = suspended;
        if (restoreRetainedChildren) _restore = true;
    }
    public override void dispose()
    { if (_index.Changed == markNeedsLayout) _index.Changed = null; base.dispose(); }

    public void ReleaseKeepAlive(RenderBox child)
    { _pendingReleases.Add(child); markNeedsLayout(); }

    private void Trim(long leading, long trailing)
    {
        if (leading == 0 && trailing == 0) return;
        invokeLayoutCallback<SliverConstraints>(_ =>
        {
            while (leading-- > 0) _destroyOrCacheChild(firstChild!);
            while (trailing-- > 0) _destroyOrCacheChild(lastChild!);
        });
    }

    public override void performLayout()
    {
        var c = constraints;
        childManager.didStartLayout(); childManager.setDidUnderflow(false);
        if (_pendingReleases.Count != 0)
        {
            var releases = _pendingReleases.ToArray(); _pendingReleases.Clear();
            invokeLayoutCallback<SliverConstraints>(_ =>
            {
                foreach (var child in releases)
                    if (ReferenceEquals(child.parent, this) && child.parentData is SliverMultiBoxAdaptorParentData { keepAlive: false, keptAlive: true })
                        childManager.removeChild(child);
            });
        }
        // An explicitly hidden owner keeps its existing elements but must not
        // rebuild a child taken by another viewport through a GlobalKey. Doing
        // so would steal that child back on the hidden viewport's next layout.
        if (_suspended)
        {
            geometry = new SliverGeometry(scrollExtent: _index.Total, maxPaintExtent: _index.Total);
            childManager.didFinishLayout();
            return;
        }
        if (_configured && (_width != c.crossAxisExtent || !Equals(_configuredRevision, _revision)) && _index.RequestedAnchor is null)
        {
            var oldAnchor = _index.Find(c.scrollOffset);
            _index.RequestedAnchor = (oldAnchor, c.scrollOffset - _index.Prefix(oldAnchor));
        }
        _index.Configure(c.crossAxisExtent, _revision);
        _configured = true; _width = c.crossAxisExtent; _configuredRevision = _revision;
        // Transfer already visited subtrees before finalizeTree. They enter the
        // keep-alive bucket without being measured at an offscreen width.
        if (_restore)
        {
            Trim(childCount, 0);
            foreach (var i in _retain)
            {
                if (_keepAliveBucket.ContainsKey(i)) continue;
                if (addInitialChild(i, _index.Prefix(i))) Trim(1, 0);
            }
            _restore = false;
        }
        if (_index.RequestedAnchor is { } request)
        {
            var correction = _index.Prefix(request.Index) + request.Offset - c.scrollOffset;
            _index.RequestedAnchor = null;
            if (Math.Abs(correction) > .001)
            { geometry = new SliverGeometry(scrollOffsetCorrection: correction); return; }
        }
        var anchor = _index.Find(c.scrollOffset);
        var inside = c.scrollOffset - _index.Prefix(anchor);
        var start = _index.Find(Math.Max(0, c.scrollOffset + c.cacheOrigin));
        var target = c.scrollOffset + c.cacheOrigin + c.remainingCacheExtent;
        // Retain ordinary State until the owner explicitly permits eviction.
        // Active linked children stay contiguous; the bucket is never painted.
        if (firstChild is { } first)
        {
            if (start < indexOf(first) || start > indexOf(lastChild!)) Trim(childCount, 0);
            else
            {
                var leading = calculateLeadingGarbage(start);
                if (leading != 0) Trim(leading, 0);
            }
        }
        if (firstChild is null && !addInitialChild(start, _index.Prefix(start)))
        { geometry = SliverGeometry.zero; childManager.didFinishLayout(); return; }
        var child = firstChild!;
        var box = c.asBoxConstraints();
        var end = _index.Prefix(start);
        var last = start;
        for (var i = start; i < _index.Count; i++)
        {
            if (i == start) child.layout(box, parentUsesSize: true);
            else
            {
                var next = childAfter(child);
                if (next is not null) next.layout(box, parentUsesSize: true);
                else next = insertAndLayoutChild(box, child, parentUsesSize: true);
                if (next is null) break;
                child = next;
            }
            ((SliverMultiBoxAdaptorParentData)child.parentData!).layoutOffset = end;
            var extent = paintExtentOf(child);
            _index.Measure(i, extent); end += extent; last = i;
            if (end >= target && i >= anchor) break;
        }
        var trailing = calculateTrailingGarbage(last);
        if (trailing != 0) Trim(0, trailing);
        var correctionAfterMeasure = _index.Prefix(anchor) + inside - c.scrollOffset;
        if (Math.Abs(correctionAfterMeasure) > .001)
        { geometry = new SliverGeometry(scrollOffsetCorrection: correctionAfterMeasure); return; }
        _index.AnchorIndex = anchor; _index.AnchorOffset = inside;
        geometry = new SliverGeometry(scrollExtent: _index.Total, maxPaintExtent: _index.Total,
            paintExtent: calculatePaintOffset(c, from: _index.Prefix(start), to: end),
            cacheExtent: calculateCacheOffset(c, from: _index.Prefix(start), to: end),
            hasVisualOverflow: c.scrollOffset > 0 || end > c.scrollOffset + c.remainingPaintExtent);
        childManager.setDidUnderflow(last == _index.Count - 1);
        childManager.didFinishLayout();
    }
}
