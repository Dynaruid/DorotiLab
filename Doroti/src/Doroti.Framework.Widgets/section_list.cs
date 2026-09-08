using Doroti.Framework.Rendering;

namespace Doroti.Framework.Widgets;

/// <summary>Variable-height indexed sections with explicit metric revisions and
/// retained State ownership. Item builders must return KeepAlive outside their
/// render-object wrappers when retention is requested.</summary>
public sealed class SectionList : SliverMultiBoxAdaptorWidget
{
    /// <summary>Stop an old scroll activity before requesting a new item anchor.</summary>
    public static void RequestItem(ScrollController scroll, SectionExtentIndex index, int item, double offset = 0)
    {
        if ((uint)item >= index.Count || !double.IsFinite(offset) || offset < 0) throw new ArgumentOutOfRangeException(nameof(item));
        foreach (var position in scroll.positions)
            position.jumpToWithoutSettling(position.pixels);
        index.RequestItem(item, offset);
    }
    private readonly SectionExtentIndex _index;
    private readonly object? _revision;
    private readonly int[] _retain;
    private readonly bool _suspended, _restoreRetainedChildren;
    /// <param name="suspended">For an offstage retained owner only: keep existing
    /// children and scroll extent without materializing or measuring children.</param>
    /// <param name="restoreRetainedChildren">Re-adopt requested retained children
    /// before finalizeTree when ownership returns from another viewport.</param>
    public SectionList(SliverChildDelegate children, SectionExtentIndex index,
        object? metricRevision = null, int[]? retainIndices = null,
        bool suspended = false, bool restoreRetainedChildren = false) : base(@delegate: children)
    {
        _index = index; _revision = metricRevision;
        _suspended = suspended; _restoreRetainedChildren = restoreRetainedChildren;
        _retain = retainIndices is null ? [] : (int[])retainIndices.Clone();
        if (_retain.Any(item => (uint)item >= index.Count)) throw new ArgumentOutOfRangeException(nameof(retainIndices));
    }
    public override RenderObject createRenderObject(BuildContext context) =>
        new RenderSectionList((SliverMultiBoxAdaptorElement)context, _index, _revision, _retain, _suspended);
    public override void updateRenderObject(BuildContext context, RenderObject renderObject) =>
        ((RenderSectionList)renderObject).Update(_index, _revision, _retain, _suspended, _restoreRetainedChildren);
}
