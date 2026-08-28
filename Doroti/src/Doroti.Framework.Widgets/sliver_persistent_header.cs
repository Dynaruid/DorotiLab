// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/sliver_persistent_header.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

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
        if ((this.floating && this.pinned))
        {
            return ((Widget)(object?)new _SliverFloatingPinnedPersistentHeader__sliver_persistent_header(@delegate: this.@delegate));
        }
        if (this.pinned)
        {
            return ((Widget)(object?)new _SliverPinnedPersistentHeader__sliver_persistent_header(@delegate: this.@delegate));
        }
        if (this.floating)
        {
            return ((Widget)(object?)new _SliverFloatingPersistentHeader__sliver_persistent_header(@delegate: this.@delegate));
        }
        return ((Widget)(object?)new _SliverScrollingPersistentHeader__sliver_persistent_header(@delegate: this.@delegate));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SliverPersistentHeaderDelegate>("delegate", this.@delegate));
        var flags = new List<string>();
        if (!System.Linq.Enumerable.Any(flags))
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
        if ((this._position is not null))
        {
            this._position!.isScrollingNotifier.removeListener(this._isScrollingListener);
        }
        _position = Scrollable.maybeOf(this.context)?.position;
        if ((this._position is not null))
        {
            this._position!.isScrollingNotifier.addListener(this._isScrollingListener);
        }
    }

    public override void dispose()
    {
        if ((this._position is not null))
        {
            this._position!.isScrollingNotifier.removeListener(this._isScrollingListener);
        }
        base.dispose();
    }

    internal virtual global::Doroti.Framework.Rendering.RenderSliverFloatingPersistentHeader? _headerRenderer()
    {
        return ((global::Doroti.Framework.Rendering.RenderSliverFloatingPersistentHeader?)(object?)this.context.findAncestorRenderObjectOfType<global::Doroti.Framework.Rendering.RenderSliverFloatingPersistentHeader>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _isScrollingListener()
    {
        DartRuntimePrimitives.Assert(() => (this._position is not null));
        global::Doroti.Framework.Rendering.RenderSliverFloatingPersistentHeader? header = ((global::Doroti.Framework.Rendering.RenderSliverFloatingPersistentHeader?)(object?)_headerRenderer());
        if (this._position!.isScrollingNotifier.value)
        {
            header?.updateScrollStartDirection(this._position!.userScrollDirection);
            header?.maybeStopSnapAnimation(this._position!.userScrollDirection);
        }
        else
        {
            header?.maybeStartSnapAnimation(this._position!.userScrollDirection);
        }
    }

    public override Widget build(BuildContext context) => ((_FloatingHeader__sliver_persistent_header)this.widget).child;
}

public class _SliverPersistentHeaderElement__sliver_persistent_header : RenderObjectElement
{
    public virtual bool floating { get; private set; } = default!;
    public virtual Element? child { get; set; } = default;

    internal _SliverPersistentHeaderElement__sliver_persistent_header(_SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header widget, bool floating = false) : base(widget)
    {
        this.floating = floating;
    }

    public override global::Doroti.Framework.Rendering.RenderObject renderObject => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(((_RenderSliverPersistentHeaderForWidgetsMixin__sliver_persistent_header?)(object?)base.renderObject)!);
    public override void mount(Element? parent, object? newSlot)
    {
        base.mount(parent, newSlot);
        ((dynamic)this.renderObject)._element = this;
    }

    public override void unmount()
    {
        ((dynamic)this.renderObject)._element = null;
        base.unmount();
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (_SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header)(object)newWidget;
        var oldWidget = ((_SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header?)(object?)this.widget)!;
        base.update(__newWidget);
        SliverPersistentHeaderDelegate newDelegate = ((_SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header)__newWidget).@delegate;
        SliverPersistentHeaderDelegate oldDelegate = ((_SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header)oldWidget).@delegate;
        if (((!object.Equals(newDelegate, oldDelegate)) && (((!object.Equals(DartRuntimePrimitives.RuntimeType(newDelegate), DartRuntimePrimitives.RuntimeType(oldDelegate))) || newDelegate.shouldRebuild(oldDelegate)))))
        {
            _RenderSliverPersistentHeaderForWidgetsMixin__sliver_persistent_header renderObjectLocal = DartRuntimePrimitives.ConvertValue<_RenderSliverPersistentHeaderForWidgetsMixin__sliver_persistent_header>(this.renderObject);
            _updateChild(newDelegate, ((double)((dynamic)renderObjectLocal).lastShrinkOffset), ((bool)((dynamic)renderObjectLocal).lastOverlapsContent));
            ((dynamic)renderObjectLocal).triggerRebuild();
        }
    }

    public override void performRebuild()
    {
        base.performRebuild();
        ((dynamic)this.renderObject).triggerRebuild();
    }

    internal virtual void _updateChild(SliverPersistentHeaderDelegate @delegate, double shrinkOffset, bool overlapsContent)
    {
        Widget newWidget = ((Widget)(object?)@delegate.build(this, shrinkOffset, overlapsContent));
        child = updateChild(this.child, (this.floating ? new _FloatingHeader__sliver_persistent_header(child: newWidget) : newWidget), null);
    }

    internal virtual void _build(double shrinkOffset, bool overlapsContent)
    {
        this.owner!.buildScope(this, ((global::System.Action)(() =>
        {
            var sliverPersistentHeaderRenderObjectWidget = ((_SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header?)(object?)this.widget)!;
            _updateChild(((_SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header)sliverPersistentHeaderRenderObjectWidget).@delegate, shrinkOffset, overlapsContent);
        })));
    }

    public override void forgetChild(Element child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child, ((Element?)((dynamic)this).child))));
        ((dynamic)this).child = null;
        base.forgetChild(child);
    }

    public override void insertRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)(object)child;
        DartRuntimePrimitives.Assert(() => ((bool)((dynamic)this.renderObject).debugValidateChild(__child)));
        ((dynamic)this.renderObject).child = __child;
    }

    public override void moveRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? oldSlot, object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => false);
    }

    public override void removeRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        ((dynamic)this.renderObject).child = null;
    }

    public override void visitChildren(global::System.Action<Element> visitor)
    {
        if ((this.child is not null))
        {
            visitor(this.child!);
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

    public override _SliverPersistentHeaderElement__sliver_persistent_header createElement() => new _SliverPersistentHeaderElement__sliver_persistent_header(this, floating: this.floating);
    public abstract override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context);
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SliverPersistentHeaderDelegate>("delegate", this.@delegate));
    }

}

public interface _RenderSliverPersistentHeaderForWidgetsMixin__sliver_persistent_header
{
    _SliverPersistentHeaderElement__sliver_persistent_header? _element { get; set; }

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
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderSliverScrollingPersistentHeaderForWidgets__sliver_persistent_header(stretchConfiguration: ((SliverPersistentHeaderDelegate)this.@delegate).stretchConfiguration));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderSliverScrollingPersistentHeaderForWidgets__sliver_persistent_header)(object)renderObject;
        __renderObject.stretchConfiguration = ((SliverPersistentHeaderDelegate)this.@delegate).stretchConfiguration;
    }

}

public class _RenderSliverScrollingPersistentHeaderForWidgets__sliver_persistent_header : global::Doroti.Framework.Rendering.RenderSliverScrollingPersistentHeader, _RenderSliverPersistentHeaderForWidgetsMixin__sliver_persistent_header
{
    public virtual _SliverPersistentHeaderElement__sliver_persistent_header? _element { get; set; } = default;

    internal _RenderSliverScrollingPersistentHeaderForWidgets__sliver_persistent_header(global::Doroti.Framework.Rendering.OverScrollHeaderStretchConfiguration? stretchConfiguration = null) : base(stretchConfiguration: stretchConfiguration)
    {
    }

    public override double minExtent => (((_SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header?)(object?)this._element!.widget)!).@delegate.minExtent;
    public override double maxExtent => (((_SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header?)(object?)this._element!.widget)!).@delegate.maxExtent;
    public override void updateChild(double shrinkOffset, bool overlapsContent)
    {
        DartRuntimePrimitives.Assert(() => (this._element is not null));
        this._element!._build(shrinkOffset, overlapsContent);
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
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderSliverPinnedPersistentHeaderForWidgets__sliver_persistent_header(stretchConfiguration: ((SliverPersistentHeaderDelegate)this.@delegate).stretchConfiguration, showOnScreenConfiguration: ((SliverPersistentHeaderDelegate)this.@delegate).showOnScreenConfiguration));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderSliverPinnedPersistentHeaderForWidgets__sliver_persistent_header)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderSliverPinnedPersistentHeaderForWidgets__sliver_persistent_header>)(() =>
{
    var __cascade = __renderObject;
    __cascade.stretchConfiguration = ((SliverPersistentHeaderDelegate)this.@delegate).stretchConfiguration;
    __cascade.showOnScreenConfiguration = ((SliverPersistentHeaderDelegate)this.@delegate).showOnScreenConfiguration;
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

    public override double minExtent => (((_SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header?)(object?)this._element!.widget)!).@delegate.minExtent;
    public override double maxExtent => (((_SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header?)(object?)this._element!.widget)!).@delegate.maxExtent;
    public override void updateChild(double shrinkOffset, bool overlapsContent)
    {
        DartRuntimePrimitives.Assert(() => (this._element is not null));
        this._element!._build(shrinkOffset, overlapsContent);
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
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderSliverFloatingPersistentHeaderForWidgets__sliver_persistent_header(vsync: ((SliverPersistentHeaderDelegate)this.@delegate).vsync, snapConfiguration: ((SliverPersistentHeaderDelegate)this.@delegate).snapConfiguration, stretchConfiguration: ((SliverPersistentHeaderDelegate)this.@delegate).stretchConfiguration, showOnScreenConfiguration: ((SliverPersistentHeaderDelegate)this.@delegate).showOnScreenConfiguration));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderSliverFloatingPersistentHeaderForWidgets__sliver_persistent_header)(object)renderObject;
        __renderObject.vsync = ((SliverPersistentHeaderDelegate)this.@delegate).vsync;
        __renderObject.snapConfiguration = ((SliverPersistentHeaderDelegate)this.@delegate).snapConfiguration;
        __renderObject.stretchConfiguration = ((SliverPersistentHeaderDelegate)this.@delegate).stretchConfiguration;
        __renderObject.showOnScreenConfiguration = ((SliverPersistentHeaderDelegate)this.@delegate).showOnScreenConfiguration;
    }

}

public class _RenderSliverFloatingPinnedPersistentHeaderForWidgets__sliver_persistent_header : global::Doroti.Framework.Rendering.RenderSliverFloatingPinnedPersistentHeader, _RenderSliverPersistentHeaderForWidgetsMixin__sliver_persistent_header
{
    public virtual _SliverPersistentHeaderElement__sliver_persistent_header? _element { get; set; } = default;

    internal _RenderSliverFloatingPinnedPersistentHeaderForWidgets__sliver_persistent_header(global::Doroti.Framework.Scheduler.TickerProvider? vsync, global::Doroti.Framework.Rendering.FloatingHeaderSnapConfiguration? snapConfiguration = null, global::Doroti.Framework.Rendering.OverScrollHeaderStretchConfiguration? stretchConfiguration = null, global::Doroti.Framework.Rendering.PersistentHeaderShowOnScreenConfiguration? showOnScreenConfiguration = null) : base(vsync: vsync, snapConfiguration: snapConfiguration, stretchConfiguration: stretchConfiguration, showOnScreenConfiguration: showOnScreenConfiguration)
    {
    }

    public override double minExtent => (((_SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header?)(object?)this._element!.widget)!).@delegate.minExtent;
    public override double maxExtent => (((_SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header?)(object?)this._element!.widget)!).@delegate.maxExtent;
    public override void updateChild(double shrinkOffset, bool overlapsContent)
    {
        DartRuntimePrimitives.Assert(() => (this._element is not null));
        this._element!._build(shrinkOffset, overlapsContent);
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
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderSliverFloatingPinnedPersistentHeaderForWidgets__sliver_persistent_header(vsync: ((SliverPersistentHeaderDelegate)this.@delegate).vsync, snapConfiguration: ((SliverPersistentHeaderDelegate)this.@delegate).snapConfiguration, stretchConfiguration: ((SliverPersistentHeaderDelegate)this.@delegate).stretchConfiguration, showOnScreenConfiguration: ((SliverPersistentHeaderDelegate)this.@delegate).showOnScreenConfiguration));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderSliverFloatingPinnedPersistentHeaderForWidgets__sliver_persistent_header)(object)renderObject;
        __renderObject.vsync = ((SliverPersistentHeaderDelegate)this.@delegate).vsync;
        __renderObject.snapConfiguration = ((SliverPersistentHeaderDelegate)this.@delegate).snapConfiguration;
        __renderObject.stretchConfiguration = ((SliverPersistentHeaderDelegate)this.@delegate).stretchConfiguration;
        __renderObject.showOnScreenConfiguration = ((SliverPersistentHeaderDelegate)this.@delegate).showOnScreenConfiguration;
    }

}

public class _RenderSliverFloatingPersistentHeaderForWidgets__sliver_persistent_header : global::Doroti.Framework.Rendering.RenderSliverFloatingPersistentHeader, _RenderSliverPersistentHeaderForWidgetsMixin__sliver_persistent_header
{
    public virtual _SliverPersistentHeaderElement__sliver_persistent_header? _element { get; set; } = default;

    internal _RenderSliverFloatingPersistentHeaderForWidgets__sliver_persistent_header(global::Doroti.Framework.Scheduler.TickerProvider? vsync, global::Doroti.Framework.Rendering.FloatingHeaderSnapConfiguration? snapConfiguration = null, global::Doroti.Framework.Rendering.OverScrollHeaderStretchConfiguration? stretchConfiguration = null, global::Doroti.Framework.Rendering.PersistentHeaderShowOnScreenConfiguration? showOnScreenConfiguration = null) : base(vsync: vsync, snapConfiguration: snapConfiguration, stretchConfiguration: stretchConfiguration, showOnScreenConfiguration: showOnScreenConfiguration)
    {
    }

    public override double minExtent => (((_SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header?)(object?)this._element!.widget)!).@delegate.minExtent;
    public override double maxExtent => (((_SliverPersistentHeaderRenderObjectWidget__sliver_persistent_header?)(object?)this._element!.widget)!).@delegate.maxExtent;
    public override void updateChild(double shrinkOffset, bool overlapsContent)
    {
        DartRuntimePrimitives.Assert(() => (this._element is not null));
        this._element!._build(shrinkOffset, overlapsContent);
    }

    public virtual void triggerRebuild()
    {
        markNeedsLayout();
    }

}
