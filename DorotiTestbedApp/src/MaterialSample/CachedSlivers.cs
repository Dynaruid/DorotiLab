// Copyright 2021 The Flutter team. All rights reserved.
// Adapted from component_screen.dart; BSD license in LICENSE.flutter.
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;

namespace MaterialSample;

internal sealed class MeasuredSlivers(double?[] heights, int firstIndex, int count, Func<BuildContext, long, Widget?> builder)
    : SliverChildBuilderDelegate(builder, childCount: count)
{
    private readonly int _start = firstIndex;
    public override double? estimateMaxScrollOffset(long firstIndex, long lastIndex, double leadingScrollOffset, double trailingScrollOffset) => heights.Skip(_start).Take(count).Sum(height => height ?? 0);
}

internal sealed class CacheHeight(double?[] heights, int index, Widget child) : SingleChildRenderObjectWidget(child: child)
{
    public override RenderObject createRenderObject(BuildContext context) => new RenderCacheHeight(heights, index);
    public override void updateRenderObject(BuildContext context, RenderObject renderObject) => ((RenderCacheHeight)renderObject).Update(heights, index);
}

internal sealed class RenderCacheHeight(double?[] heights, int index) : RenderProxyBox
{
    private double?[] _heights = heights;
    private int _index = index;
    internal void Update(double?[] heights, int index)
    {
        if (ReferenceEquals(_heights, heights) && _index == index) return;
        _heights = heights; _index = index; markNeedsLayout();
    }
    public override void performLayout() { base.performLayout(); _heights[_index] = size.height; }
}
