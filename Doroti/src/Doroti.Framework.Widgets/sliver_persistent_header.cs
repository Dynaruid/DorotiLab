// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/sliver_persistent_header.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public abstract class SliverPersistentHeaderDelegate
{
    protected SliverPersistentHeaderDelegate()
    {
    }

    public abstract Widget build(BuildContext context, double shrinkOffset, bool overlapsContent);
    public abstract double minExtent { get; }
    public abstract double maxExtent { get; }
    public virtual global::Doroti.Framework.Scheduler.TickerProvider? vsync => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Scheduler.TickerProvider>(null);
    public virtual global::Doroti.Framework.Rendering.FloatingHeaderSnapConfiguration? snapConfiguration => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.FloatingHeaderSnapConfiguration>(null);
    public virtual global::Doroti.Framework.Rendering.OverScrollHeaderStretchConfiguration? stretchConfiguration => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.OverScrollHeaderStretchConfiguration>(null);
    public virtual global::Doroti.Framework.Rendering.PersistentHeaderShowOnScreenConfiguration? showOnScreenConfiguration => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.PersistentHeaderShowOnScreenConfiguration>(null);
    public abstract bool shouldRebuild(SliverPersistentHeaderDelegate oldDelegate);
}

public class SliverPersistentHeader : StatelessWidget
{
    public virtual SliverPersistentHeaderDelegate @delegate { get; private set; } = default!;
    public virtual bool pinned { get; private set; } = default!;
    public virtual bool floating { get; private set; } = default!;

    public SliverPersistentHeader(global::Doroti.Framework.Foundation.Key? key = null, SliverPersistentHeaderDelegate @delegate = default!, bool pinned = false, bool floating = false) : base(key: key)
    {
        this.@delegate = @delegate;
        this.pinned = pinned;
        this.floating = floating;
    }

    public override Widget build(BuildContext context)
    {
        if (floating && pinned)
        {
            return new _SliverFloatingPinnedPersistentHeader__sliver_persistent_header(@delegate: @delegate);
        }
        if (pinned)
        {
            return new _SliverPinnedPersistentHeader__sliver_persistent_header(@delegate: @delegate);
        }
        if (floating)
        {
            return new _SliverFloatingPersistentHeader__sliver_persistent_header(@delegate: @delegate);
        }
        return new _SliverScrollingPersistentHeader__sliver_persistent_header(@delegate: @delegate);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SliverPersistentHeaderDelegate>("delegate", @delegate));
        var flags = new List<string>();
        if (!Enumerable.Any(flags))
        {
            flags.Add("normal");
        }
        properties.add(new global::Doroti.Framework.Foundation.IterableProperty<string>("mode", flags.Cast<string>()));
    }

}

internal class _FloatingHeader__sliver_persistent_header : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;

    internal _FloatingHeader__sliver_persistent_header(Widget child)
    {
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _FloatingHeaderState__sliver_persistent_header());
}

internal class _FloatingHeaderState__sliver_persistent_header : State<_FloatingHeader__sliver_persistent_header>
{
    internal virtual ScrollPosition? _position { get; set; } = default;

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        if (_position is not null)
        {
            _position!.isScrollingNotifier.removeListener(_isScrollingListener);
        }
        _position = Scrollable.maybeOf(context)?.position;
        if (_position is not null)
        {
            _position!.isScrollingNotifier.addListener(_isScrollingListener);
        }
    }

    public override void dispose()
    {
        if (_position is not null)
        {
            _position!.isScrollingNotifier.removeListener(_isScrollingListener);
        }
        base.dispose();
    }

    internal virtual global::Doroti.Framework.Rendering.RenderSliverFloatingPersistentHeader? _headerRenderer()
    {
        return context.findAncestorRenderObjectOfType<global::Doroti.Framework.Rendering.RenderSliverFloatingPersistentHeader>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _isScrollingListener()
    {
        DartRuntimePrimitives.Assert(() => _position is not null);
        global::Doroti.Framework.Rendering.RenderSliverFloatingPersistentHeader? header = _headerRenderer();
        if (_position!.isScrollingNotifier.value)
        {
            header?.updateScrollStartDirection(_position!.userScrollDirection);
            header?.maybeStopSnapAnimation(_position!.userScrollDirection);
        }
        else
        {
            header?.maybeStartSnapAnimation(_position!.userScrollDirection);
        }
    }

    public override Widget build(BuildContext context) => widget.child;
}

public class _SliverPersistentHeaderElement__sliver_persistent_header : RenderObjectElement
{
    public virtual bool floating { get; private set; } = default!;
    public virtual Element? child { get; set; } = default;

    internal _SliverPersistentHeaderElement__sliver_persistent_header(_SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header widget, bool floating = false) : base(widget)
    {
        this.floating = floating;
    }

    public override global::Doroti.Framework.Rendering.RenderSliverPersistentHeader renderObject => (global::Doroti.Framework.Rendering.RenderSliverPersistentHeader)base.renderObject;
    private _RenderSliverPersistentHeaderForWidgetsMixin__sliver_persistent_header header => (_RenderSliverPersistentHeaderForWidgetsMixin__sliver_persistent_header)renderObject;
    public override void mount(Element? parent, object? newSlot)
    {
        base.mount(parent, newSlot);
        header._element = this;
    }

    public override void unmount()
    {
        header._element = null;
        base.unmount();
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (_SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header)newWidget;
        var oldWidget = ((_SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header?)widget)!;
        base.update(__newWidget);
        SliverPersistentHeaderDelegate newDelegate = __newWidget.@delegate;
        SliverPersistentHeaderDelegate oldDelegate = oldWidget.@delegate;
        if ((!Equals(newDelegate, oldDelegate)) && ((!Equals(DartRuntimePrimitives.RuntimeType(newDelegate), DartRuntimePrimitives.RuntimeType(oldDelegate))) || newDelegate.shouldRebuild(oldDelegate)))
        {
            _RenderSliverPersistentHeaderForWidgetsMixin__sliver_persistent_header renderObjectLocal = DartRuntimePrimitives.ConvertValue<_RenderSliverPersistentHeaderForWidgetsMixin__sliver_persistent_header>(renderObject);
            _updateChild(newDelegate, (double)renderObjectLocal.lastShrinkOffset, renderObjectLocal.lastOverlapsContent);
            renderObjectLocal.triggerRebuild();
        }
    }

    public override void performRebuild()
    {
        base.performRebuild();
        header.triggerRebuild();
    }

    internal virtual void _updateChild(SliverPersistentHeaderDelegate @delegate, double shrinkOffset, bool overlapsContent)
    {
        Widget newWidget = @delegate.build(this, shrinkOffset, overlapsContent);
        child = updateChild(child, floating ? new _FloatingHeader__sliver_persistent_header(child: newWidget) : newWidget, null);
    }

    internal virtual void _build(double shrinkOffset, bool overlapsContent)
    {
        owner!.buildScope(this, () =>
        {
            var sliverPersistentHeaderRenderObjectWidget = ((_SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header?)widget)!;
            _updateChild(sliverPersistentHeaderRenderObjectWidget.@delegate, shrinkOffset, overlapsContent);
        });
    }

    public override void forgetChild(Element child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child, this.child));
        this.child = null;
        base.forgetChild(child);
    }

    public override void insertRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)child;
        DartRuntimePrimitives.Assert(() => renderObject.debugValidateChild(__child));
        renderObject.child = __child;
    }

    public override void moveRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? oldSlot, object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => false);
    }

    public override void removeRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        renderObject.child = null;
    }

    public override void visitChildren(global::System.Action<Element> visitor)
    {
        if (child is not null)
        {
            visitor(child!);
        }
    }

}

public abstract class _SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header : RenderObjectWidget
{
    public virtual SliverPersistentHeaderDelegate @delegate { get; private set; } = default!;
    public virtual bool floating { get; private set; } = default!;

    internal _SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header(SliverPersistentHeaderDelegate @delegate, bool floating = false)
    {
        this.@delegate = @delegate;
        this.floating = floating;
    }

    public override _SliverPersistentHeaderElement__sliver_persistent_header createElement() => new _SliverPersistentHeaderElement__sliver_persistent_header(this, floating: floating);
    public abstract override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context);
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SliverPersistentHeaderDelegate>("delegate", @delegate));
    }

}

public interface _RenderSliverPersistentHeaderForWidgetsMixin__sliver_persistent_header
{
    _SliverPersistentHeaderElement__sliver_persistent_header? _element { get; set; }

    public double lastShrinkOffset { get; }
    public bool lastOverlapsContent { get; }
    public double minExtent { get; }
    public double maxExtent { get; }
    public void updateChild(double shrinkOffset, bool overlapsContent);
    public void triggerRebuild();
}

internal class _SliverScrollingPersistentHeader__sliver_persistent_header : _SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header
{
    internal _SliverScrollingPersistentHeader__sliver_persistent_header(SliverPersistentHeaderDelegate @delegate) : base(@delegate: @delegate)
    {
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderSliverScrollingPersistentHeaderForWidgets__sliver_persistent_header(stretchConfiguration: @delegate.stretchConfiguration);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderSliverScrollingPersistentHeaderForWidgets__sliver_persistent_header)renderObject;
        __renderObject.stretchConfiguration = @delegate.stretchConfiguration;
    }

}

public class _RenderSliverScrollingPersistentHeaderForWidgets__sliver_persistent_header : global::Doroti.Framework.Rendering.RenderSliverScrollingPersistentHeader, _RenderSliverPersistentHeaderForWidgetsMixin__sliver_persistent_header
{
    public virtual _SliverPersistentHeaderElement__sliver_persistent_header? _element { get; set; } = default;

    internal _RenderSliverScrollingPersistentHeaderForWidgets__sliver_persistent_header(global::Doroti.Framework.Rendering.OverScrollHeaderStretchConfiguration? stretchConfiguration = null) : base(stretchConfiguration: stretchConfiguration)
    {
    }

    public override double minExtent => ((_SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header?)_element!.widget)!.@delegate.minExtent;
    public override double maxExtent => ((_SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header?)_element!.widget)!.@delegate.maxExtent;
    public override void updateChild(double shrinkOffset, bool overlapsContent)
    {
        DartRuntimePrimitives.Assert(() => _element is not null);
        _element!._build(shrinkOffset, overlapsContent);
    }

    public virtual void triggerRebuild()
    {
        markNeedsLayout();
    }

}

internal class _SliverPinnedPersistentHeader__sliver_persistent_header : _SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header
{
    internal _SliverPinnedPersistentHeader__sliver_persistent_header(SliverPersistentHeaderDelegate @delegate) : base(@delegate: @delegate)
    {
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderSliverPinnedPersistentHeaderForWidgets__sliver_persistent_header(stretchConfiguration: @delegate.stretchConfiguration, showOnScreenConfiguration: @delegate.showOnScreenConfiguration);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderSliverPinnedPersistentHeaderForWidgets__sliver_persistent_header)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderSliverPinnedPersistentHeaderForWidgets__sliver_persistent_header>)(() =>
{
    var __cascade = __renderObject;
    __cascade.stretchConfiguration = @delegate.stretchConfiguration;
    __cascade.showOnScreenConfiguration = @delegate.showOnScreenConfiguration;
    return __cascade;
}))());
    }

}

public class _RenderSliverPinnedPersistentHeaderForWidgets__sliver_persistent_header : global::Doroti.Framework.Rendering.RenderSliverPinnedPersistentHeader, _RenderSliverPersistentHeaderForWidgetsMixin__sliver_persistent_header
{
    public virtual _SliverPersistentHeaderElement__sliver_persistent_header? _element { get; set; } = default;

    internal _RenderSliverPinnedPersistentHeaderForWidgets__sliver_persistent_header(global::Doroti.Framework.Rendering.OverScrollHeaderStretchConfiguration? stretchConfiguration = null, global::Doroti.Framework.Rendering.PersistentHeaderShowOnScreenConfiguration? showOnScreenConfiguration = default!) : base(stretchConfiguration: stretchConfiguration, showOnScreenConfiguration: showOnScreenConfiguration ?? new global::Doroti.Framework.Rendering.PersistentHeaderShowOnScreenConfiguration())
    {
    }

    public override double minExtent => ((_SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header?)_element!.widget)!.@delegate.minExtent;
    public override double maxExtent => ((_SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header?)_element!.widget)!.@delegate.maxExtent;
    public override void updateChild(double shrinkOffset, bool overlapsContent)
    {
        DartRuntimePrimitives.Assert(() => _element is not null);
        _element!._build(shrinkOffset, overlapsContent);
    }

    public virtual void triggerRebuild()
    {
        markNeedsLayout();
    }

}

internal class _SliverFloatingPersistentHeader__sliver_persistent_header : _SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header
{
    internal _SliverFloatingPersistentHeader__sliver_persistent_header(SliverPersistentHeaderDelegate @delegate) : base(@delegate: @delegate, floating: true)
    {
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderSliverFloatingPersistentHeaderForWidgets__sliver_persistent_header(vsync: @delegate.vsync, snapConfiguration: @delegate.snapConfiguration, stretchConfiguration: @delegate.stretchConfiguration, showOnScreenConfiguration: @delegate.showOnScreenConfiguration);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderSliverFloatingPersistentHeaderForWidgets__sliver_persistent_header)renderObject;
        __renderObject.vsync = @delegate.vsync;
        __renderObject.snapConfiguration = @delegate.snapConfiguration;
        __renderObject.stretchConfiguration = @delegate.stretchConfiguration;
        __renderObject.showOnScreenConfiguration = @delegate.showOnScreenConfiguration;
    }

}

public class _RenderSliverFloatingPinnedPersistentHeaderForWidgets__sliver_persistent_header : global::Doroti.Framework.Rendering.RenderSliverFloatingPinnedPersistentHeader, _RenderSliverPersistentHeaderForWidgetsMixin__sliver_persistent_header
{
    public virtual _SliverPersistentHeaderElement__sliver_persistent_header? _element { get; set; } = default;

    internal _RenderSliverFloatingPinnedPersistentHeaderForWidgets__sliver_persistent_header(global::Doroti.Framework.Scheduler.TickerProvider? vsync, global::Doroti.Framework.Rendering.FloatingHeaderSnapConfiguration? snapConfiguration = null, global::Doroti.Framework.Rendering.OverScrollHeaderStretchConfiguration? stretchConfiguration = null, global::Doroti.Framework.Rendering.PersistentHeaderShowOnScreenConfiguration? showOnScreenConfiguration = null) : base(vsync: vsync, snapConfiguration: snapConfiguration, stretchConfiguration: stretchConfiguration, showOnScreenConfiguration: showOnScreenConfiguration)
    {
    }

    public override double minExtent => ((_SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header?)_element!.widget)!.@delegate.minExtent;
    public override double maxExtent => ((_SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header?)_element!.widget)!.@delegate.maxExtent;
    public override void updateChild(double shrinkOffset, bool overlapsContent)
    {
        DartRuntimePrimitives.Assert(() => _element is not null);
        _element!._build(shrinkOffset, overlapsContent);
    }

    public virtual void triggerRebuild()
    {
        markNeedsLayout();
    }

}

internal class _SliverFloatingPinnedPersistentHeader__sliver_persistent_header : _SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header
{
    internal _SliverFloatingPinnedPersistentHeader__sliver_persistent_header(SliverPersistentHeaderDelegate @delegate) : base(@delegate: @delegate, floating: true)
    {
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderSliverFloatingPinnedPersistentHeaderForWidgets__sliver_persistent_header(vsync: @delegate.vsync, snapConfiguration: @delegate.snapConfiguration, stretchConfiguration: @delegate.stretchConfiguration, showOnScreenConfiguration: @delegate.showOnScreenConfiguration);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderSliverFloatingPinnedPersistentHeaderForWidgets__sliver_persistent_header)renderObject;
        __renderObject.vsync = @delegate.vsync;
        __renderObject.snapConfiguration = @delegate.snapConfiguration;
        __renderObject.stretchConfiguration = @delegate.stretchConfiguration;
        __renderObject.showOnScreenConfiguration = @delegate.showOnScreenConfiguration;
    }

}

public class _RenderSliverFloatingPersistentHeaderForWidgets__sliver_persistent_header : global::Doroti.Framework.Rendering.RenderSliverFloatingPersistentHeader, _RenderSliverPersistentHeaderForWidgetsMixin__sliver_persistent_header
{
    public virtual _SliverPersistentHeaderElement__sliver_persistent_header? _element { get; set; } = default;

    internal _RenderSliverFloatingPersistentHeaderForWidgets__sliver_persistent_header(global::Doroti.Framework.Scheduler.TickerProvider? vsync, global::Doroti.Framework.Rendering.FloatingHeaderSnapConfiguration? snapConfiguration = null, global::Doroti.Framework.Rendering.OverScrollHeaderStretchConfiguration? stretchConfiguration = null, global::Doroti.Framework.Rendering.PersistentHeaderShowOnScreenConfiguration? showOnScreenConfiguration = null) : base(vsync: vsync, snapConfiguration: snapConfiguration, stretchConfiguration: stretchConfiguration, showOnScreenConfiguration: showOnScreenConfiguration)
    {
    }

    public override double minExtent => ((_SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header?)_element!.widget)!.@delegate.minExtent;
    public override double maxExtent => ((_SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header?)_element!.widget)!.@delegate.maxExtent;
    public override void updateChild(double shrinkOffset, bool overlapsContent)
    {
        DartRuntimePrimitives.Assert(() => _element is not null);
        _element!._build(shrinkOffset, overlapsContent);
    }

    public virtual void triggerRebuild()
    {
        markNeedsLayout();
    }

}
