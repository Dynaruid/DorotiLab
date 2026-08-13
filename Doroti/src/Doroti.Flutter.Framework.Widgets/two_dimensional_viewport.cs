// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/two_dimensional_viewport.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public delegate Widget? TwoDimensionalIndexedWidgetBuilder(BuildContext context, ChildVicinity vicinity);

public abstract class TwoDimensionalViewport : RenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Rendering.ViewportOffset verticalOffset { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.AxisDirection verticalAxisDirection { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.ViewportOffset horizontalOffset { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.AxisDirection horizontalAxisDirection { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.Axis mainAxis { get; private set; } = default!;
    public virtual double? cacheExtent { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.CacheExtentStyle? cacheExtentStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual TwoDimensionalChildDelegate @delegate { get; private set; } = default!;

    protected TwoDimensionalViewport(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Rendering.ViewportOffset verticalOffset = default!, global::Doroti.Generated.Framework.Painting.AxisDirection verticalAxisDirection = default!, global::Doroti.Generated.Framework.Rendering.ViewportOffset horizontalOffset = default!, global::Doroti.Generated.Framework.Painting.AxisDirection horizontalAxisDirection = default!, TwoDimensionalChildDelegate @delegate = default!, global::Doroti.Generated.Framework.Painting.Axis mainAxis = default!, double? cacheExtent = null, global::Doroti.Generated.Framework.Rendering.CacheExtentStyle? cacheExtentStyle = null, global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, Clip clipBehavior = Clip.hardEdge) : base(key: key)
    {
        this.verticalOffset = verticalOffset;
        this.verticalAxisDirection = verticalAxisDirection;
        this.horizontalOffset = horizontalOffset;
        this.horizontalAxisDirection = horizontalAxisDirection;
        this.@delegate = @delegate;
        this.mainAxis = mainAxis;
        this.cacheExtent = cacheExtent;
        this.cacheExtentStyle = cacheExtentStyle;
        this.scrollCacheExtent = scrollCacheExtent;
        this.clipBehavior = clipBehavior;
        System.Diagnostics.Debug.Assert(((object.Equals(verticalAxisDirection, global::Doroti.Generated.Framework.Painting.AxisDirection.down)) || (object.Equals(verticalAxisDirection, global::Doroti.Generated.Framework.Painting.AxisDirection.up))));
        System.Diagnostics.Debug.Assert(((object.Equals(horizontalAxisDirection, global::Doroti.Generated.Framework.Painting.AxisDirection.left)) || (object.Equals(horizontalAxisDirection, global::Doroti.Generated.Framework.Painting.AxisDirection.right))));
    }

    public override RenderObjectElement createElement() => DartRuntimePrimitives.ConvertValue<RenderObjectElement>(new _TwoDimensionalViewportElement__two_dimensional_viewport(this));
    public abstract override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context);
    public abstract override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject);
}

internal class _TwoDimensionalViewportElement__two_dimensional_viewport : RenderObjectElement, NotifiableElementMixin, ViewportElementMixin, TwoDimensionalChildManager
{
    internal virtual DartMap<ChildVicinity, Element> _vicinityToChild { get; set; } = new DartMap<ChildVicinity, Element>();
    internal virtual DartMap<global::Doroti.Generated.Framework.Foundation.Key, Element> _keyToChild { get; set; } = new DartMap<global::Doroti.Generated.Framework.Foundation.Key, Element>();
    internal virtual DartMap<ChildVicinity, Element>? _newVicinityToChild { get; set; } = default;
    internal virtual DartMap<global::Doroti.Generated.Framework.Foundation.Key, Element>? _newKeyToChild { get; set; } = default;

    internal _TwoDimensionalViewportElement__two_dimensional_viewport(RenderObjectWidget widget) : base(widget)
    {
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject renderObject => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(((RenderTwoDimensionalViewport?)(object?)base.renderObject)!);
    public override void performRebuild()
    {
        base.performRebuild();
        ((dynamic)this.renderObject).markNeedsLayout(withDelegateRebuild: true);
    }

    public override void forgetChild(Element child)
    {
        DartRuntimePrimitives.Assert(() => !this._debugIsDoingLayout);
        base.forgetChild(child);
        this._vicinityToChild.remove(((ChildVicinity)(object)((Element)child).slot));
        if ((((Element)child).widget.key is not null))
        {
            this._keyToChild.remove(((Element)child).widget.key);
        }
    }

    public override void insertRenderObjectChild(global::Doroti.Generated.Framework.Rendering.RenderObject child, object? slot)
    {
        var __child = (global::Doroti.Generated.Framework.Rendering.RenderBox)(object)child;
        var __slot = (ChildVicinity)(object)slot;
        ((dynamic)this.renderObject)._insertChild(__child, __slot);
    }

    public override void moveRenderObjectChild(global::Doroti.Generated.Framework.Rendering.RenderObject child, object? oldSlot, object? newSlot)
    {
        var __child = (global::Doroti.Generated.Framework.Rendering.RenderBox)(object)child;
        var __oldSlot = (ChildVicinity)(object)oldSlot;
        var __newSlot = (ChildVicinity)(object)newSlot;
        ((dynamic)this.renderObject)._moveChild(__child, from: __oldSlot, to: __newSlot);
    }

    public override void removeRenderObjectChild(global::Doroti.Generated.Framework.Rendering.RenderObject child, object? slot)
    {
        var __child = (global::Doroti.Generated.Framework.Rendering.RenderBox)(object)child;
        var __slot = (ChildVicinity)(object)slot;
        ((dynamic)this.renderObject)._removeChild(__child, __slot);
    }

    public override void visitChildren(global::System.Action<Element> visitor)
    {
        this._vicinityToChild.Values.forEach((__arg0) => ((global::System.Action<Element>)visitor)(__arg0));
    }

    public override List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        List<Element> children__11839 = ((Func<List<Element>>)(() =>
{            var __cascade = this._vicinityToChild.Values.ToList();
            __cascade.sort(_compareChildren);
            return __cascade;        }))().ToList();
        return new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static long _compareChildren(Element a, Element b)
    {
        var aSlot__12112 = ((ChildVicinity?)(object?)((Element)a).slot!)!;
        var bSlot__12156 = ((ChildVicinity?)(object?)((Element)b).slot!)!;
        return aSlot__12112.compareTo(bSlot__12156);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _debugIsDoingLayout => DartRuntimePrimitives.ConvertValue<bool>(((this._newKeyToChild is not null) && (this._newVicinityToChild is not null)));
    public virtual void _startLayout()
    {
        DartRuntimePrimitives.Assert(() => !this._debugIsDoingLayout);
        _newVicinityToChild = new DartMap<ChildVicinity, Element>().cast<ChildVicinity, Element>();
        _newKeyToChild = new DartMap<global::Doroti.Generated.Framework.Foundation.Key, Element>().cast<global::Doroti.Generated.Framework.Foundation.Key, Element>();
    }

    public virtual void _buildChild(ChildVicinity vicinity)
    {
        DartRuntimePrimitives.Assert(() => this._debugIsDoingLayout);
        this.owner!.buildScope(this, ((global::System.Action)(() => {
Widget? newWidget__12675 = ((Widget?)(object?)(((TwoDimensionalViewport?)(object?)this.widget)!).@delegate.build(this, vicinity));
if ((newWidget__12675 is null))
{
    return;
}
Element? oldElement__12830 = ((Element?)(object?)_retrieveOldElement(newWidget__12675, vicinity));
Element? newChild__12906 = ((Element?)(object?)updateChild(oldElement__12830, newWidget__12675, vicinity));
DartRuntimePrimitives.Assert(() => (newChild__12906 is not null));
DartRuntimePrimitives.Assert(() => (!this._newVicinityToChild!.ContainsKey(vicinity)));
this._newVicinityToChild![vicinity] = newChild__12906!;
if ((((Widget)newWidget__12675).key is not null))
{
    DartRuntimePrimitives.Assert(() => (!this._newKeyToChild!.ContainsKey(((Widget)newWidget__12675).key!)));
    this._newKeyToChild![((Widget)newWidget__12675).key!] = newChild__12906;
}
})));
    }

    internal virtual Element? _retrieveOldElement(Widget newWidget, ChildVicinity vicinity)
    {
        if ((((Widget)newWidget).key is not null))
        {
            Element? result__13508 = this._keyToChild.remove(((Widget)newWidget).key);
            if ((result__13508 is not null))
            {
                this._vicinityToChild.remove(((ChildVicinity)(object)((Element)result__13508).slot));
            }
            return result__13508;
        }
        Element? potentialOldElement__13680 = this._vicinityToChild.GetValueOrDefault(vicinity);
        if (((potentialOldElement__13680 is not null) && (((Element)potentialOldElement__13680).widget.key is null)))
        {
            return this._vicinityToChild.remove(vicinity);
        }
        return ((Element)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _reuseChild(ChildVicinity vicinity)
    {
        DartRuntimePrimitives.Assert(() => this._debugIsDoingLayout);
        Element? elementToReuse__13996 = this._vicinityToChild.remove(vicinity);
        DartRuntimePrimitives.Assert(() => (elementToReuse__13996 is not null), () => (object?)$"Expected to re-use an element at {vicinity}, but none was found.");
        this._newVicinityToChild![vicinity] = elementToReuse__13996!;
        if ((((Element)elementToReuse__13996).widget.key is not null))
        {
            DartRuntimePrimitives.Assert(() => this._keyToChild.ContainsKey(((Element)elementToReuse__13996).widget.key));
            DartRuntimePrimitives.Assert(() => (object.Equals(this._keyToChild.GetValueOrDefault(DartRuntimePrimitives.RequireReference(((Element)elementToReuse__13996).widget.key)), elementToReuse__13996)));
            this._newKeyToChild![((Element)elementToReuse__13996).widget.key!] = this._keyToChild.remove(((Element)elementToReuse__13996).widget.key)!;
        }
    }

    public virtual void _endLayout()
    {
        DartRuntimePrimitives.Assert(() => this._debugIsDoingLayout);
        foreach (Element element__14684 in this._vicinityToChild.Values)
        {
            if ((((Element)element__14684).widget.key is null))
            {
                updateChild(element__14684, ((Widget)(object)null), null);
            }
            else
            {
                DartRuntimePrimitives.Assert(() => this._keyToChild.containsValue(element__14684));
            }
        }
        foreach (Element element__14956 in this._keyToChild.Values)
        {
            DartRuntimePrimitives.Assert(() => (((Element)element__14956).widget.key is not null));
            updateChild(element__14956, ((Widget)(object)null), null);
        }
        _vicinityToChild = this._newVicinityToChild!;
        _keyToChild = this._newKeyToChild!;
        _newVicinityToChild = null;
        _newKeyToChild = null;
        DartRuntimePrimitives.Assert(() => !this._debugIsDoingLayout);
    }

    public override void attachNotificationTree()
    {
        _notificationTree = new _NotificationNode__framework(this._parent?._notificationTree, this);
    }

    public virtual bool onNotification(Notification notification)
    {
        if ((notification is ViewportNotificationMixin))
        {
            ((ViewportNotificationMixin)notification)._depth += 1L;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class TwoDimensionalViewportParentData : global::Doroti.Generated.Framework.Rendering.ParentData, global::Doroti.Generated.Framework.Rendering.KeepAliveParentDataMixin
{
    public virtual Offset? layoutOffset { get; set; } = default;
    public virtual ChildVicinity vicinity { get; set; } = ChildVicinity.invalid;
    internal virtual Size? _paintExtent { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Rendering.RenderBox? _previousSibling { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Rendering.RenderBox? _nextSibling { get; set; } = default;
    public virtual Offset? paintOffset { get; set; } = default;
    public virtual bool keepAlive { get; set; } = false;

    public virtual bool isVisible
    {
        get
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((this._paintExtent is null))
                    {
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary("The paint extent of the child has not been determined yet."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("The paint extent, and therefore the visibility, of a child of a " + "RenderTwoDimensionalViewport is computed after " + "RenderTwoDimensionalViewport.layoutChildSequence.") }));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            return (((!object.Equals(this._paintExtent, Size.zero)) || (DartRuntimePrimitives.RequireValue(this._paintExtent).height != 0.0)) || (DartRuntimePrimitives.RequireValue(this._paintExtent).width != 0.0));
            return default!;
        }
    }
    public virtual bool keptAlive => DartRuntimePrimitives.ConvertValue<bool>((this.keepAlive && !this.isVisible));
    public override string ToString()
    {
        return $"vicinity={this.vicinity}; " + $"layoutOffset={this.layoutOffset}; " + $"paintOffset={this.paintOffset}; " + $"{((this._paintExtent is null) ? "not visible; " : $"{(!this.isVisible ? "not " : "")}visible - paintExtent={this._paintExtent}; ")}" + $"{(this.keepAlive ? "keepAlive; " : "")}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class RenderTwoDimensionalViewport : global::Doroti.Generated.Framework.Rendering.RenderBox
{
    internal virtual global::Doroti.Generated.Framework.Rendering.ViewportOffset _horizontalOffset { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Painting.AxisDirection _horizontalAxisDirection { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Rendering.ViewportOffset _verticalOffset { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Painting.AxisDirection _verticalAxisDirection { get; set; } = default!;
    internal virtual TwoDimensionalChildDelegate _delegate { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Painting.Axis _mainAxis { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent _scrollCacheExtent { get; set; } = default!;
    internal virtual Clip _clipBehavior { get; set; } = default!;
    internal virtual TwoDimensionalChildManager _childManager { get; private set; } = default!;
    internal virtual DartMap<ChildVicinity, global::Doroti.Generated.Framework.Rendering.RenderBox> _children { get; private set; } = new DartMap<ChildVicinity, global::Doroti.Generated.Framework.Rendering.RenderBox>();
    internal virtual DartMap<ChildVicinity, global::Doroti.Generated.Framework.Rendering.RenderBox> _activeChildrenForLayoutPass { get; private set; } = new DartMap<ChildVicinity, global::Doroti.Generated.Framework.Rendering.RenderBox>();
    internal virtual DartMap<ChildVicinity, global::Doroti.Generated.Framework.Rendering.RenderBox> _keepAliveBucket { get; private set; } = new DartMap<ChildVicinity, global::Doroti.Generated.Framework.Rendering.RenderBox>();
    internal virtual List<global::Doroti.Generated.Framework.Rendering.RenderBox> _debugDanglingKeepAlives { get; set; } = default!;
    internal virtual bool _hasVisualOverflow { get; set; } = false;
    internal virtual global::Doroti.Generated.Framework.Rendering.LayerHandle<global::Doroti.Generated.Framework.Rendering.ClipRectLayer> _clipRectLayer { get; private set; } = new global::Doroti.Generated.Framework.Rendering.LayerHandle<global::Doroti.Generated.Framework.Rendering.ClipRectLayer>();
    internal virtual List<ChildVicinity> _currentChildVicinities { get; private set; } = new List<ChildVicinity>();
    internal virtual global::Doroti.Generated.Framework.Rendering.RenderBox? _firstChild { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Rendering.RenderBox? _lastChild { get; set; } = default;
    internal virtual bool _didResize { get; set; } = true;
    internal virtual bool _needsDelegateRebuild { get; set; } = true;
    internal virtual List<global::Doroti.Generated.Framework.Rendering.RenderBox>? _debugOrphans { get; set; } = default;

    protected RenderTwoDimensionalViewport(global::Doroti.Generated.Framework.Rendering.ViewportOffset horizontalOffset, global::Doroti.Generated.Framework.Painting.AxisDirection horizontalAxisDirection, global::Doroti.Generated.Framework.Rendering.ViewportOffset verticalOffset, global::Doroti.Generated.Framework.Painting.AxisDirection verticalAxisDirection, TwoDimensionalChildDelegate @delegate, global::Doroti.Generated.Framework.Painting.Axis mainAxis, TwoDimensionalChildManager childManager, double? cacheExtent = null, global::Doroti.Generated.Framework.Rendering.CacheExtentStyle? cacheExtentStyle = null, global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, Clip clipBehavior = Clip.hardEdge)
    {
        this._childManager = childManager;
        this._horizontalOffset = horizontalOffset;
        this._horizontalAxisDirection = horizontalAxisDirection;
        this._verticalOffset = verticalOffset;
        this._verticalAxisDirection = verticalAxisDirection;
        this._delegate = @delegate;
        this._mainAxis = mainAxis;
        this._scrollCacheExtent = (scrollCacheExtent ?? (((cacheExtent is not null) ? (cacheExtentStyle switch { global::Doroti.Generated.Framework.Rendering.CacheExtentStyle.pixel => global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent.CreatePixels(DartRuntimePrimitives.RequireValue(cacheExtent)), null => global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent.CreatePixels(DartRuntimePrimitives.RequireValue(cacheExtent)), global::Doroti.Generated.Framework.Rendering.CacheExtentStyle.viewport => global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent.CreateViewport(DartRuntimePrimitives.RequireValue(cacheExtent)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }) : global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent.CreatePixels(global::Doroti.Generated.Framework.Rendering.RenderAbstractViewport.defaultCacheExtent))));
        this._clipBehavior = clipBehavior;
        System.Diagnostics.Debug.Assert(((object.Equals(verticalAxisDirection, global::Doroti.Generated.Framework.Painting.AxisDirection.down)) || (object.Equals(verticalAxisDirection, global::Doroti.Generated.Framework.Painting.AxisDirection.up))));
        System.Diagnostics.Debug.Assert(((object.Equals(horizontalAxisDirection, global::Doroti.Generated.Framework.Painting.AxisDirection.left)) || (object.Equals(horizontalAxisDirection, global::Doroti.Generated.Framework.Painting.AxisDirection.right))));
    }

    public virtual global::Doroti.Generated.Framework.Rendering.ViewportOffset horizontalOffset
    {
        get => this._horizontalOffset;
        set
        {
            var __value = value;
            if ((object.Equals(this._horizontalOffset, __value)))
            {
                return;
            }
            if (this.attached)
            {
                this._horizontalOffset.removeListener(() => this.markNeedsLayout());
            }
            _horizontalOffset = __value;
            if (this.attached)
            {
                this._horizontalOffset.addListener(() => this.markNeedsLayout());
            }
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Generated.Framework.Painting.AxisDirection horizontalAxisDirection
    {
        get => this._horizontalAxisDirection;
        set
        {
            var __value = value;
            if ((object.Equals(this._horizontalAxisDirection, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _horizontalAxisDirection = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Generated.Framework.Rendering.ViewportOffset verticalOffset
    {
        get => this._verticalOffset;
        set
        {
            var __value = value;
            if ((object.Equals(this._verticalOffset, __value)))
            {
                return;
            }
            if (this.attached)
            {
                this._verticalOffset.removeListener(() => this.markNeedsLayout());
            }
            _verticalOffset = __value;
            if (this.attached)
            {
                this._verticalOffset.addListener(() => this.markNeedsLayout());
            }
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Generated.Framework.Painting.AxisDirection verticalAxisDirection
    {
        get => this._verticalAxisDirection;
        set
        {
            var __value = value;
            if ((object.Equals(this._verticalAxisDirection, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _verticalAxisDirection = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual TwoDimensionalChildDelegate @delegate
    {
        get => this._delegate;
        set
        {
            var __value = value;
            if ((object.Equals(this._delegate, __value)))
            {
                return;
            }
            if (this.attached)
            {
                this._delegate.removeListener(() => this._handleDelegateNotification());
            }
            TwoDimensionalChildDelegate oldDelegate__25773 = this._delegate;
            _delegate = __value;
            if (this.attached)
            {
                this._delegate.addListener(() => this._handleDelegateNotification());
            }
            if (((!object.Equals(DartRuntimePrimitives.RuntimeType(this._delegate), DartRuntimePrimitives.RuntimeType(oldDelegate__25773))) || this._delegate.shouldRebuild(oldDelegate__25773)))
            {
                _handleDelegateNotification();
            }
        }
    }
    public virtual global::Doroti.Generated.Framework.Painting.Axis mainAxis
    {
        get => this._mainAxis;
        set
        {
            var __value = value;
            if ((object.Equals(this._mainAxis, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _mainAxis = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual double cacheExtent
    {
        get => ((global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent)this._scrollCacheExtent).value;
        set
        {
            double? __value = value;
            if ((__value == this.cacheExtent))
            {
                return;
            }
            if ((__value is null))
            {
                _scrollCacheExtent = global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent.CreatePixels(global::Doroti.Generated.Framework.Rendering.RenderAbstractViewport.defaultCacheExtent);
            }
            else
            {
                _scrollCacheExtent = (this.cacheExtentStyle switch { global::Doroti.Generated.Framework.Rendering.CacheExtentStyle.pixel => global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent.CreatePixels(DartRuntimePrimitives.RequireValue(__value)), global::Doroti.Generated.Framework.Rendering.CacheExtentStyle.viewport => global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent.CreateViewport(DartRuntimePrimitives.RequireValue(__value)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            }
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Generated.Framework.Rendering.CacheExtentStyle cacheExtentStyle
    {
        get => ((global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent)this._scrollCacheExtent).style;
        set
        {
            global::Doroti.Generated.Framework.Rendering.CacheExtentStyle? __value = value;
            if ((object.Equals(__value, this.cacheExtentStyle)))
            {
                return;
            }
            if ((__value is null))
            {
                _scrollCacheExtent = global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent.CreatePixels(this.cacheExtent);
            }
            else
            {
                _scrollCacheExtent = (DartRuntimePrimitives.RequireValue(__value) switch { global::Doroti.Generated.Framework.Rendering.CacheExtentStyle.pixel => global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent.CreatePixels(this.cacheExtent), global::Doroti.Generated.Framework.Rendering.CacheExtentStyle.viewport => global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent.CreateViewport(this.cacheExtent), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            }
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent scrollCacheExtent
    {
        get => this._scrollCacheExtent;
        set
        {
            global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent? __value = value;
            if ((object.Equals(this._scrollCacheExtent, __value)))
            {
                return;
            }
            if ((__value is null))
            {
                _scrollCacheExtent = global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent.CreatePixels(global::Doroti.Generated.Framework.Rendering.RenderAbstractViewport.defaultCacheExtent);
            }
            else
            {
                _scrollCacheExtent = __value;
            }
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Flutter.Ui.Clip clipBehavior
    {
        get => this._clipBehavior;
        set
        {
            var __value = value;
            if ((object.Equals(this._clipBehavior, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _clipBehavior = DartRuntimePrimitives.RequireValue(__value);
            markNeedsPaint();
            markNeedsSemanticsUpdate();
        }
    }
    public override bool isRepaintBoundary => true;
    public override bool sizedByParent => true;
    public virtual global::Doroti.Generated.Framework.Rendering.RenderBox? firstChild => this._firstChild;
    public virtual global::Doroti.Generated.Framework.Rendering.RenderBox? lastChild => this._lastChild;
    public virtual global::Doroti.Generated.Framework.Rendering.RenderBox? childBefore(global::Doroti.Generated.Framework.Rendering.RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        return parentDataOf(child)._previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Rendering.RenderBox? childAfter(global::Doroti.Generated.Framework.Rendering.RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        return parentDataOf(child)._nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleDelegateNotification()
    {
        markNeedsLayout(withDelegateRebuild: true);
        return;
    }

    public override void setupParentData(global::Doroti.Generated.Framework.Rendering.RenderObject child)
    {
        var __child = (global::Doroti.Generated.Framework.Rendering.RenderBox)(object)child;
        if ((__child.parentData is not TwoDimensionalViewportParentData))
        {
            __child.parentData = new TwoDimensionalViewportParentData();
        }
    }

    public virtual TwoDimensionalViewportParentData parentDataOf(global::Doroti.Generated.Framework.Rendering.RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => ((this._children.containsValue(child) || this._keepAliveBucket.containsValue(child)) || this._debugOrphans!.Contains(child)));
        return ((TwoDimensionalViewportParentData?)(object?)child.parentData!)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Rendering.RenderBox? getChildFor(ChildVicinity vicinity) => this._children.GetValueOrDefault(vicinity);
    public override void attach(global::Doroti.Generated.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        this._horizontalOffset.addListener(() => this.markNeedsLayout());
        this._verticalOffset.addListener(() => this.markNeedsLayout());
        this._delegate.addListener(() => this._handleDelegateNotification());
        foreach (global::Doroti.Generated.Framework.Rendering.RenderBox child__33787 in this._children.Values)
        {
            child__33787.attach(owner);
        }
        foreach (global::Doroti.Generated.Framework.Rendering.RenderBox child__33874 in this._keepAliveBucket.Values)
        {
            child__33874.attach(owner);
        }
    }

    public override void detach()
    {
        base.detach();
        this._horizontalOffset.removeListener(() => this.markNeedsLayout());
        this._verticalOffset.removeListener(() => this.markNeedsLayout());
        this._delegate.removeListener(() => this._handleDelegateNotification());
        foreach (global::Doroti.Generated.Framework.Rendering.RenderBox child__34190 in this._children.Values)
        {
            child__34190.detach();
        }
        foreach (global::Doroti.Generated.Framework.Rendering.RenderBox child__34272 in this._keepAliveBucket.Values)
        {
            child__34272.detach();
        }
    }

    public override void redepthChildren()
    {
        foreach (global::Doroti.Generated.Framework.Rendering.RenderBox child__34405 in this._children.Values)
        {
            child__34405.redepthChildren();
        }
        this._keepAliveBucket.Values.forEach((__arg0) => ((global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject>)this.redepthChild)(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(__arg0)));
    }

    public override void visitChildren(global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject> visitor)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__34606 = this._firstChild;
        while ((child__34606 is not null))
        {
            visitor(child__34606);
            child__34606 = parentDataOf(child__34606)._nextSibling;
        }
        this._keepAliveBucket.Values.forEach((__arg0) => ((global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject>)visitor)(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(__arg0)));
    }

    public override void visitChildrenForSemantics(global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject> visitor)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__34970 = this._firstChild;
        while ((child__34970 is not null))
        {
            TwoDimensionalViewportParentData childParentData__35064 = ((TwoDimensionalViewportParentData)(object?)parentDataOf(child__34970));
            visitor(child__34970);
            child__34970 = ((TwoDimensionalViewportParentData)childParentData__35064)._nextSibling;
        }
    }

    public override List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var debugChildren__35304 = new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>();
        return debugChildren__35304;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugCheckHasBoundedAxis(global::Doroti.Generated.Framework.Painting.Axis.vertical, constraints));
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugCheckHasBoundedAxis(global::Doroti.Generated.Framework.Painting.Axis.horizontal, constraints));
        return ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).biggest;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestChildren(global::Doroti.Generated.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        foreach (global::Doroti.Generated.Framework.Rendering.RenderBox child__35893 in this._children.Values)
        {
            TwoDimensionalViewportParentData childParentData__35967 = ((TwoDimensionalViewportParentData)(object?)parentDataOf(child__35893));
            if (!((TwoDimensionalViewportParentData)childParentData__35967).isVisible)
            {
                continue;
            }
            bool isHit__36139 = result.addWithPaintOffset(offset: ((TwoDimensionalViewportParentData)childParentData__35967).paintOffset, position: position, hitTest: ((global::System.Func<global::Doroti.Generated.Framework.Rendering.BoxHitTestResult, Offset, bool>)((result, transformed) => {
DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - DartRuntimePrimitives.RequireValue(((TwoDimensionalViewportParentData)childParentData__35967).paintOffset)))));
return child__35893.hitTest(result, position: transformed);
throw new InvalidOperationException("Dart closure completed without a value.");
})));
            if (isHit__36139)
            {
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Flutter.Ui.Size viewportDimension
    {
        get
        {
            DartRuntimePrimitives.Assert(() => this.hasSize);
            return this.size;
            return default!;
        }
    }
    public override void performResize()
    {
        global::Doroti.Flutter.Ui.Size? oldSize__36788 = ((global::Doroti.Flutter.Ui.Size?)(object?)(this.hasSize ? this.size : null));
        base.performResize();
        this.horizontalOffset.applyViewportDimension(this.size.width);
        this.verticalOffset.applyViewportDimension(this.size.height);
        if ((!object.Equals(oldSize__36788, this.size)))
        {
            _didResize = true;
        }
    }

    public virtual global::Doroti.Generated.Framework.Rendering.RevealedOffset getOffsetToReveal(global::Doroti.Generated.Framework.Rendering.RenderObject target, double alignment, Rect? rect = null, global::Doroti.Generated.Framework.Painting.Axis? axis = null)
    {
        axis ??= this.mainAxis;
        var (offset__37432, axisDirection__37454) = (DartRuntimePrimitives.RequireValue(axis) switch { global::Doroti.Generated.Framework.Painting.Axis.vertical => (((double, global::Doroti.Generated.Framework.Painting.AxisDirection))((((global::Doroti.Generated.Framework.Rendering.ViewportOffset)this.verticalOffset).pixels, this.verticalAxisDirection))), global::Doroti.Generated.Framework.Painting.Axis.horizontal => (((double, global::Doroti.Generated.Framework.Painting.AxisDirection))((((global::Doroti.Generated.Framework.Rendering.ViewportOffset)this.horizontalOffset).pixels, this.horizontalAxisDirection))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        rect ??= ((global::Doroti.Generated.Framework.Rendering.RenderObject)target).paintBounds;
        var child__37787 = target;
        while ((!object.Equals(((global::Doroti.Generated.Framework.Rendering.RenderObject)child__37787).parent, this)))
        {
            child__37787 = ((global::Doroti.Generated.Framework.Rendering.RenderObject)child__37787).parent!;
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(((global::Doroti.Generated.Framework.Rendering.RenderObject)child__37787).parent, this)));
        var box__37918 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)child__37787)!;
        global::Doroti.Flutter.Ui.Rect rectLocal__37959 = ((global::Doroti.Flutter.Ui.Rect)(object?)MatrixUtils.transformRect(((Matrix4)((dynamic)target).getTransformTo(((global::Doroti.Generated.Framework.Rendering.RenderBox)child__37787))), DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(rect))));
        var leadingScrollOffset__38043 = offset__37432;
        leadingScrollOffset__38043 += (DartRuntimePrimitives.RequireValue(axisDirection__37454) switch { global::Doroti.Generated.Framework.Painting.AxisDirection.up => (((global::Doroti.Generated.Framework.Rendering.RenderBox)((global::Doroti.Generated.Framework.Rendering.RenderBox)child__37787)).size.height - rectLocal__37959.bottom), global::Doroti.Generated.Framework.Painting.AxisDirection.left => (((global::Doroti.Generated.Framework.Rendering.RenderBox)((global::Doroti.Generated.Framework.Rendering.RenderBox)child__37787)).size.width - rectLocal__37959.right), global::Doroti.Generated.Framework.Painting.AxisDirection.right => rectLocal__37959.left, global::Doroti.Generated.Framework.Painting.AxisDirection.down => rectLocal__37959.top, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Flutter.Ui.Offset paintOffset__38470 = ((global::Doroti.Flutter.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(parentDataOf(box__37918).paintOffset));
        leadingScrollOffset__38043 += (DartRuntimePrimitives.RequireValue(axisDirection__37454) switch { global::Doroti.Generated.Framework.Painting.AxisDirection.up => ((this.viewportDimension.height - paintOffset__38470.dy) - ((global::Doroti.Generated.Framework.Rendering.RenderBox)box__37918).size.height), global::Doroti.Generated.Framework.Painting.AxisDirection.left => ((this.viewportDimension.width - paintOffset__38470.dx) - ((global::Doroti.Generated.Framework.Rendering.RenderBox)box__37918).size.width), global::Doroti.Generated.Framework.Painting.AxisDirection.right => paintOffset__38470.dx, global::Doroti.Generated.Framework.Painting.AxisDirection.down => paintOffset__38470.dy, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        Matrix4 transform__39047 = ((Matrix4)(object?)((Matrix4)((dynamic)target).getTransformTo(this)));
        global::Doroti.Flutter.Ui.Rect targetRect__39097 = ((global::Doroti.Flutter.Ui.Rect)(object?)MatrixUtils.transformRect(transform__39047, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(rect))));
        double mainAxisExtentDifference__39172 = (DartRuntimePrimitives.RequireValue(axis) switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => (this.viewportDimension.width - rectLocal__37959.width), global::Doroti.Generated.Framework.Painting.Axis.vertical => (this.viewportDimension.height - rectLocal__37959.height), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        double targetOffset__39376 = (leadingScrollOffset__38043 - (mainAxisExtentDifference__39172 * alignment));
        double offsetDifference__39469 = (DartRuntimePrimitives.RequireValue(axis) switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => (((global::Doroti.Generated.Framework.Rendering.ViewportOffset)this.horizontalOffset).pixels - targetOffset__39376), global::Doroti.Generated.Framework.Painting.Axis.vertical => (((global::Doroti.Generated.Framework.Rendering.ViewportOffset)this.verticalOffset).pixels - targetOffset__39376), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        targetRect__39097 = (DartRuntimePrimitives.RequireValue(axisDirection__37454) switch { global::Doroti.Generated.Framework.Painting.AxisDirection.up => targetRect__39097.translate(0.0, -offsetDifference__39469), global::Doroti.Generated.Framework.Painting.AxisDirection.down => targetRect__39097.translate(0.0, offsetDifference__39469), global::Doroti.Generated.Framework.Painting.AxisDirection.left => targetRect__39097.translate(-offsetDifference__39469, 0.0), global::Doroti.Generated.Framework.Painting.AxisDirection.right => targetRect__39097.translate(offsetDifference__39469, 0.0), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        var revealedOffset__39991 = new global::Doroti.Generated.Framework.Rendering.RevealedOffset(offset: targetOffset__39376, rect: targetRect__39097);
        return revealedOffset__39991;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void showOnScreen(global::Doroti.Generated.Framework.Rendering.RenderObject? descendant = null, Rect? rect = null, Duration duration = default, global::Doroti.Generated.Framework.Animation.Curve curve = default!)
    {
        bool allowHorizontal__40429 = ((global::Doroti.Generated.Framework.Rendering.ViewportOffset)this.horizontalOffset).allowImplicitScrolling;
        bool allowVertical__40503 = ((global::Doroti.Generated.Framework.Rendering.ViewportOffset)this.verticalOffset).allowImplicitScrolling;
        global::Doroti.Generated.Framework.Painting.AxisDirection? axisDirection__40577 = default!;
        switch ((allowHorizontal__40429, allowVertical__40503))
        {
            case (true, true):
                {
                    break;
                }
            case (false, true):
                {
                    axisDirection__40577 = this.verticalAxisDirection;
                    break;
                }
            case (true, false):
                {
                    axisDirection__40577 = this.horizontalAxisDirection;
                    break;
                }
            case (false, false):
                {
                    base.showOnScreen(descendant: descendant, rect: rect, duration: duration, curve: curve);
                    return;
                }
        }
        global::Doroti.Flutter.Ui.Rect? newRect__41255 = ((global::Doroti.Flutter.Ui.Rect?)(object?)RenderTwoDimensionalViewport.showInViewport(descendant: descendant, viewport: this, axisDirection: axisDirection__40577, rect: rect, duration: duration, curve: curve));
        base.showOnScreen(rect: newRect__41255, duration: duration, curve: curve);
    }

    public static global::Doroti.Flutter.Ui.Rect? showInViewport(global::Doroti.Generated.Framework.Rendering.RenderObject? descendant = null, Rect? rect = null, RenderTwoDimensionalViewport viewport = default!, Duration duration = default, global::Doroti.Generated.Framework.Animation.Curve curve = default!, global::Doroti.Generated.Framework.Painting.AxisDirection? axisDirection = null)
    {
        if ((descendant is null))
        {
            return rect;
        }
        Rect? showVertical(Rect? rect)
        {
            return RenderTwoDimensionalViewport._showInViewportForAxisDirection(descendant: descendant, viewport: viewport, axis: global::Doroti.Generated.Framework.Painting.Axis.vertical, rect: rect, duration: duration, curve: curve);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        Rect? showHorizontal(Rect? rect)
        {
            return RenderTwoDimensionalViewport._showInViewportForAxisDirection(descendant: descendant, viewport: viewport, axis: global::Doroti.Generated.Framework.Painting.Axis.horizontal, rect: rect, duration: duration, curve: curve);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        switch (axisDirection)
        {
            case global::Doroti.Generated.Framework.Painting.AxisDirection.left:
            case global::Doroti.Generated.Framework.Painting.AxisDirection.right:
                {
                    return showHorizontal(rect);
                }
            case global::Doroti.Generated.Framework.Painting.AxisDirection.up:
            case global::Doroti.Generated.Framework.Painting.AxisDirection.down:
                {
                    return showVertical(rect);
                }
            case null:
                {
                    rect = (showHorizontal(rect) ?? rect);
                    rect = showVertical(rect);
                    if ((rect is null))
                    {
                        DartRuntimePrimitives.Assert(() => (viewport.parent is not null));
                        Matrix4 transform__44547 = ((Matrix4)(object?)((Matrix4)((dynamic)descendant).getTransformTo(viewport.parent)));
                        return ((global::Doroti.Flutter.Ui.Rect?)(object?)MatrixUtils.transformRect(transform__44547, ((rect ?? (Rect)((global::Doroti.Generated.Framework.Rendering.RenderObject)descendant).paintBounds))));
                    }
                    return DartRuntimePrimitives.RequireValue(rect);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Flutter.Ui.Rect? _showInViewportForAxisDirection(global::Doroti.Generated.Framework.Rendering.RenderObject descendant, Rect? rect = null, RenderTwoDimensionalViewport viewport = default!, global::Doroti.Generated.Framework.Painting.Axis axis = default!, Duration duration = default, global::Doroti.Generated.Framework.Animation.Curve curve = default!)
    {
        global::Doroti.Generated.Framework.Rendering.ViewportOffset offset__45013 = (DartRuntimePrimitives.RequireValue(axis) switch { global::Doroti.Generated.Framework.Painting.Axis.vertical => ((RenderTwoDimensionalViewport)viewport).verticalOffset, global::Doroti.Generated.Framework.Painting.Axis.horizontal => ((RenderTwoDimensionalViewport)viewport).horizontalOffset, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Generated.Framework.Rendering.RevealedOffset leadingEdgeOffset__45171 = ((global::Doroti.Generated.Framework.Rendering.RevealedOffset)(object?)viewport.getOffsetToReveal(descendant, 0.0, rect: rect, axis: DartRuntimePrimitives.RequireValue(axis)));
        global::Doroti.Generated.Framework.Rendering.RevealedOffset trailingEdgeOffset__45316 = ((global::Doroti.Generated.Framework.Rendering.RevealedOffset)(object?)viewport.getOffsetToReveal(descendant, 1.0, rect: rect, axis: DartRuntimePrimitives.RequireValue(axis)));
        double currentOffset__45454 = ((global::Doroti.Generated.Framework.Rendering.ViewportOffset)offset__45013).pixels;
        global::Doroti.Generated.Framework.Rendering.RevealedOffset? targetOffset__45512 = ((global::Doroti.Generated.Framework.Rendering.RevealedOffset?)(object?)RevealedOffset.clampOffset(leadingEdgeOffset: leadingEdgeOffset__45171, trailingEdgeOffset: trailingEdgeOffset__45316, currentOffset: currentOffset__45454));
        if ((targetOffset__45512 is null))
        {
            return ((global::Doroti.Flutter.Ui.Rect)(object)null);
        }
        DartRuntimePrimitives.Ignore(offset__45013.moveTo(((global::Doroti.Generated.Framework.Rendering.RevealedOffset)targetOffset__45512).offset, duration: duration, curve: curve));
        return ((global::Doroti.Generated.Framework.Rendering.RevealedOffset)targetOffset__45512).rect;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool didResize => this._didResize;
    public virtual bool needsDelegateRebuild => this._needsDelegateRebuild;
    public virtual void markNeedsLayout(bool withDelegateRebuild = false)
    {
        _needsDelegateRebuild = (this._needsDelegateRebuild || withDelegateRebuild);
        base.markNeedsLayout();
    }

    public abstract void layoutChildSequence();
    public override void performLayout()
    {
        _firstChild = null;
        _lastChild = null;
        this._activeChildrenForLayoutPass.Clear();
        this._childManager._startLayout();
        layoutChildSequence();
        DartRuntimePrimitives.Assert(() => _debugCheckContentDimensions());
        _didResize = false;
        _needsDelegateRebuild = false;
        _cacheKeepAlives();
        invokeLayoutCallback<global::Doroti.Generated.Framework.Rendering.BoxConstraints>(((global::System.Action<global::Doroti.Generated.Framework.Rendering.BoxConstraints>)((_) => {
this._childManager._endLayout();
DartRuntimePrimitives.Assert(() => ((this._debugOrphans is { } __items48761 ? !System.Linq.Enumerable.Any(__items48761) : (bool?)null) ?? true));
DartRuntimePrimitives.Assert(() => !System.Linq.Enumerable.Any(this._debugDanglingKeepAlives));
DartRuntimePrimitives.Assert(() => !System.Linq.Enumerable.Any(this._keepAliveBucket.Values.where(((child) => {
return !parentDataOf(child).keepAlive;
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
_reifyChildren();
})));
    }

    internal virtual void _cacheKeepAlives()
    {
        List<global::Doroti.Generated.Framework.Rendering.RenderBox> remainingChildren__49300 = this._children.Values.toSet().difference<global::Doroti.Generated.Framework.Rendering.RenderBox>(this._activeChildrenForLayoutPass.Values.toSet()).ToList().ToList();
        foreach (var child__49453 in remainingChildren__49300)
        {
            TwoDimensionalViewportParentData childParentData__49528 = ((TwoDimensionalViewportParentData)(object?)parentDataOf(child__49453));
            if (childParentData__49528.keepAlive)
            {
                this._keepAliveBucket[((TwoDimensionalViewportParentData)childParentData__49528).vicinity] = child__49453;
                this._childManager._reuseChild(((TwoDimensionalViewportParentData)childParentData__49528).vicinity);
            }
        }
    }

    internal virtual void _sortByYIndex()
    {
        this._currentChildVicinities.sort(((a, b) => {
long yComparison__49891 = ((ChildVicinity)a).yIndex.CompareTo(((ChildVicinity)b).yIndex);
if ((yComparison__49891 != 0L))
{
    return yComparison__49891;
}
return ((ChildVicinity)a).xIndex.CompareTo(((ChildVicinity)b).xIndex);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
    }

    internal virtual void _sortByXIndex()
    {
        this._currentChildVicinities.sort();
    }

    internal virtual void _reifyChildren()
    {
        DartRuntimePrimitives.Assert(() => (this._firstChild is null));
        DartRuntimePrimitives.Assert(() => (this._lastChild is null));
        global::Doroti.Generated.Framework.Rendering.RenderBox? previousChild__50351 = default!;
        switch (this.mainAxis)
        {
            case global::Doroti.Generated.Framework.Painting.Axis.vertical:
                {
                    _sortByYIndex();
                    break;
                }
            case global::Doroti.Generated.Framework.Painting.Axis.horizontal:
                {
                    _sortByXIndex();
                    break;
                }
        }
        foreach (ChildVicinity vicinity__50904 in this._currentChildVicinities)
        {
            previousChild__50351 = (_completeChildParentData(vicinity__50904, previousChild: previousChild__50351) ?? previousChild__50351);
        }
        _lastChild = previousChild__50351;
        if ((this._lastChild is not null))
        {
            parentDataOf(this._lastChild!)._nextSibling = null;
        }
        this._currentChildVicinities.Clear();
    }

    internal virtual global::Doroti.Generated.Framework.Rendering.RenderBox? _completeChildParentData(ChildVicinity vicinity, global::Doroti.Generated.Framework.Rendering.RenderBox? previousChild = null)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(vicinity, ChildVicinity.invalid)));
        if (this._children.ContainsKey(vicinity))
        {
            global::Doroti.Generated.Framework.Rendering.RenderBox child__51670 = this._children.GetValueOrDefault(vicinity)!;
            DartRuntimePrimitives.Assert(() => (object.Equals(parentDataOf(child__51670).vicinity, vicinity)));
            updateChildPaintData(child__51670);
            if ((previousChild is null))
            {
                DartRuntimePrimitives.Assert(() => (this._firstChild is null));
                _firstChild = child__51670;
            }
            else
            {
                parentDataOf(previousChild)._nextSibling = child__51670;
                parentDataOf(child__51670)._previousSibling = previousChild;
            }
            return child__51670;
        }
        return ((global::Doroti.Generated.Framework.Rendering.RenderBox)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _debugCheckContentDimensions()
    {
        var hint__52174 = "Subclasses should call applyContentDimensions on the " + "verticalOffset and horizontalOffset to set the min and max scroll offset. " + "If the contents exceed one or both sides of the viewportDimension, " + "ensure the viewportDimension height or width is subtracted in that axis " + "for the correct extent.";
        DartRuntimePrimitives.Assert(() =>
            {
                if (!(((ScrollPosition?)(object?)this.verticalOffset)!).hasContentDimensions)
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary("The verticalOffset was not given content dimensions during " + "layoutChildSequence."), new global::Doroti.Generated.Framework.Foundation.ErrorHint(hint__52174) }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() =>
            {
                if (!(((ScrollPosition?)(object?)this.horizontalOffset)!).hasContentDimensions)
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary("The horizontalOffset was not given content dimensions during " + "layoutChildSequence."), new global::Doroti.Generated.Framework.Foundation.ErrorHint(hint__52174) }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Rendering.RenderBox? buildOrObtainChildFor(ChildVicinity vicinity)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(vicinity, ChildVicinity.invalid)));
        DartRuntimePrimitives.Assert(() => this.debugDoingThisLayout);
        if ((this._needsDelegateRebuild || ((!this._children.ContainsKey(vicinity) && !this._keepAliveBucket.ContainsKey(vicinity)))))
        {
            invokeLayoutCallback<global::Doroti.Generated.Framework.Rendering.BoxConstraints>(((global::System.Action<global::Doroti.Generated.Framework.Rendering.BoxConstraints>)((_) => {
this._childManager._buildChild(vicinity);
})));
        }
        else
        {
            this._keepAliveBucket.remove(vicinity);
            this._childManager._reuseChild(vicinity);
        }
        if (!this._children.ContainsKey(vicinity))
        {
            return ((global::Doroti.Generated.Framework.Rendering.RenderBox)(object)null);
        }
        DartRuntimePrimitives.Assert(() => this._children.ContainsKey(vicinity));
        global::Doroti.Generated.Framework.Rendering.RenderBox child__54594 = this._children.GetValueOrDefault(vicinity)!;
        this._activeChildrenForLayoutPass[vicinity] = child__54594;
        parentDataOf(child__54594).vicinity = vicinity;
        this._currentChildVicinities.Add(vicinity);
        return child__54594;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void updateChildPaintData(global::Doroti.Generated.Framework.Rendering.RenderBox child)
    {
        TwoDimensionalViewportParentData childParentData__55058 = ((TwoDimensionalViewportParentData)(object?)parentDataOf(child));
        DartRuntimePrimitives.Assert(() => (((TwoDimensionalViewportParentData)childParentData__55058).layoutOffset is not null), () => (object?)$"The child with ChildVicinity(xIndex: {((TwoDimensionalViewportParentData)childParentData__55058).vicinity.xIndex}, " + $"yIndex: {((TwoDimensionalViewportParentData)childParentData__55058).vicinity.yIndex}) was not provided a " + "layoutOffset. This should be set during layoutChildSequence, " + "representing the position of the child.");
        DartRuntimePrimitives.Assert(() => ((global::Doroti.Generated.Framework.Rendering.RenderBox)child).hasSize);
        childParentData__55058._paintExtent = computeChildPaintExtent(DartRuntimePrimitives.RequireValue(((TwoDimensionalViewportParentData)childParentData__55058).layoutOffset), ((global::Doroti.Generated.Framework.Rendering.RenderBox)child).size);
        childParentData__55058.paintOffset = computeAbsolutePaintOffsetFor(child, layoutOffset: DartRuntimePrimitives.RequireValue(((TwoDimensionalViewportParentData)childParentData__55058).layoutOffset));
        _hasVisualOverflow = ((this._hasVisualOverflow || (!object.Equals(((TwoDimensionalViewportParentData)childParentData__55058).layoutOffset, ((TwoDimensionalViewportParentData)childParentData__55058)._paintExtent))) || !((TwoDimensionalViewportParentData)childParentData__55058).isVisible);
    }

    public virtual global::Doroti.Flutter.Ui.Size computeChildPaintExtent(Offset layoutOffset, Size childSize)
    {
        if ((((object.Equals(childSize, Size.zero)) || (childSize.height == 0.0)) || (childSize.width == 0.0)))
        {
            return Size.zero;
        }
        double width__56926 = default!;
        if ((layoutOffset.dx < 0.0))
        {
            if (((layoutOffset.dx + childSize.width) <= 0.0))
            {
                return Size.zero;
            }
            width__56926 = (layoutOffset.dx + childSize.width);
        }
        else
        {
            if ((layoutOffset.dx >= this.viewportDimension.width))
            {
                return Size.zero;
            }
            else
            {
                DartRuntimePrimitives.Assert(() => ((layoutOffset.dx >= 0L) && (layoutOffset.dx < this.viewportDimension.width)));
                if (((layoutOffset.dx + childSize.width) > this.viewportDimension.width))
                {
                    width__56926 = (this.viewportDimension.width - layoutOffset.dx);
                }
                else
                {
                    DartRuntimePrimitives.Assert(() => ((layoutOffset.dx + childSize.width) <= this.viewportDimension.width));
                    width__56926 = childSize.width;
                }
            }
        }
        double height__58052 = default!;
        if ((layoutOffset.dy < 0.0))
        {
            if (((layoutOffset.dy + childSize.height) <= 0.0))
            {
                return Size.zero;
            }
            height__58052 = (layoutOffset.dy + childSize.height);
        }
        else
        {
            if ((layoutOffset.dy >= this.viewportDimension.height))
            {
                return Size.zero;
            }
            else
            {
                DartRuntimePrimitives.Assert(() => ((layoutOffset.dy >= 0L) && (layoutOffset.dy < this.viewportDimension.height)));
                if (((layoutOffset.dy + childSize.height) > this.viewportDimension.height))
                {
                    height__58052 = (this.viewportDimension.height - layoutOffset.dy);
                }
                else
                {
                    DartRuntimePrimitives.Assert(() => ((layoutOffset.dy + childSize.height) <= this.viewportDimension.height));
                    height__58052 = childSize.height;
                }
            }
        }
        return new global::Doroti.Flutter.Ui.Size(width__56926, height__58052);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Flutter.Ui.Offset computeAbsolutePaintOffsetFor(global::Doroti.Generated.Framework.Rendering.RenderBox child, Offset layoutOffset)
    {
        DartRuntimePrimitives.Assert(() => this.hasSize);
        DartRuntimePrimitives.Assert(() => ((global::Doroti.Generated.Framework.Rendering.RenderBox)child).hasSize);
        double xOffset__59899 = (this.horizontalAxisDirection switch { global::Doroti.Generated.Framework.Painting.AxisDirection.right => layoutOffset.dx, global::Doroti.Generated.Framework.Painting.AxisDirection.left => (this.viewportDimension.width - ((layoutOffset.dx + ((global::Doroti.Generated.Framework.Rendering.RenderBox)child).size.width))), global::Doroti.Generated.Framework.Painting.AxisDirection.up => throw new Exception("This should not happen"), global::Doroti.Generated.Framework.Painting.AxisDirection.down => throw new Exception("This should not happen"), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        double yOffset__60197 = (this.verticalAxisDirection switch { global::Doroti.Generated.Framework.Painting.AxisDirection.up => (this.viewportDimension.height - ((layoutOffset.dy + ((global::Doroti.Generated.Framework.Rendering.RenderBox)child).size.height))), global::Doroti.Generated.Framework.Painting.AxisDirection.down => layoutOffset.dy, global::Doroti.Generated.Framework.Painting.AxisDirection.right => throw new Exception("This should not happen"), global::Doroti.Generated.Framework.Painting.AxisDirection.left => throw new Exception("This should not happen"), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return new global::Doroti.Flutter.Ui.Offset(xOffset__59899, yOffset__60197);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        if (!System.Linq.Enumerable.Any(this._children))
        {
            return;
        }
        if ((this._hasVisualOverflow && (!object.Equals(this.clipBehavior, Clip.none))))
        {
            this._clipRectLayer.layer = context.pushClipRect(this.needsCompositing, offset, (Offset.zero & this.viewportDimension), (global::System.Action<global::Doroti.Generated.Framework.Rendering.PaintingContext, Offset>)this._paintChildren, clipBehavior: this.clipBehavior, oldLayer: ((global::Doroti.Generated.Framework.Rendering.LayerHandle<global::Doroti.Generated.Framework.Rendering.ClipRectLayer>)this._clipRectLayer).layer);
        }
        else
        {
            this._clipRectLayer.layer = null;
            _paintChildren(context, offset);
        }
    }

    internal virtual void _paintChildren(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__61115 = this._firstChild;
        while ((child__61115 is not null))
        {
            TwoDimensionalViewportParentData childParentData__61209 = ((TwoDimensionalViewportParentData)(object?)parentDataOf(child__61115));
            if (((TwoDimensionalViewportParentData)childParentData__61209).isVisible)
            {
                context.paintChild(child__61115, (offset + DartRuntimePrimitives.RequireValue(((TwoDimensionalViewportParentData)childParentData__61209).paintOffset)));
            }
            child__61115 = ((TwoDimensionalViewportParentData)childParentData__61209)._nextSibling;
        }
    }

    internal virtual void _insertChild(global::Doroti.Generated.Framework.Rendering.RenderBox child, ChildVicinity slot)
    {
        DartRuntimePrimitives.Assert(() => _debugTrackOrphans(newOrphan: this._children.GetValueOrDefault(slot)));
        DartRuntimePrimitives.Assert(() => !this._keepAliveBucket.containsValue(child));
        this._children[slot] = child;
        adoptChild(child);
    }

    internal virtual void _moveChild(global::Doroti.Generated.Framework.Rendering.RenderBox child, ChildVicinity from, ChildVicinity to)
    {
        TwoDimensionalViewportParentData childParentData__61849 = ((TwoDimensionalViewportParentData)(object?)parentDataOf(child));
        if (!((TwoDimensionalViewportParentData)childParentData__61849).keptAlive)
        {
            if ((object.Equals(this._children.GetValueOrDefault(from), child)))
            {
                this._children.remove(from);
            }
            DartRuntimePrimitives.Assert(() => _debugTrackOrphans(newOrphan: this._children.GetValueOrDefault(to), noLongerOrphan: child));
            this._children[to] = child;
            return;
        }
        if ((object.Equals(this._keepAliveBucket.GetValueOrDefault(((TwoDimensionalViewportParentData)childParentData__61849).vicinity), child)))
        {
            this._keepAliveBucket.remove(((TwoDimensionalViewportParentData)childParentData__61849).vicinity);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugDanglingKeepAlives.Remove(child);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() =>
            {
                if (this._keepAliveBucket.ContainsKey(((TwoDimensionalViewportParentData)childParentData__61849).vicinity))
                {
                    this._debugDanglingKeepAlives.Add(this._keepAliveBucket.GetValueOrDefault(((TwoDimensionalViewportParentData)childParentData__61849).vicinity)!);
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._keepAliveBucket[((TwoDimensionalViewportParentData)childParentData__61849).vicinity] = child;
    }

    internal virtual void _removeChild(global::Doroti.Generated.Framework.Rendering.RenderBox child, ChildVicinity slot)
    {
        TwoDimensionalViewportParentData childParentData__63116 = ((TwoDimensionalViewportParentData)(object?)parentDataOf(child));
        if (!((TwoDimensionalViewportParentData)childParentData__63116).keptAlive)
        {
            if ((object.Equals(this._children.GetValueOrDefault(slot), child)))
            {
                this._children.remove(slot);
            }
            DartRuntimePrimitives.Assert(() => _debugTrackOrphans(noLongerOrphan: child));
            if ((object.Equals(this._keepAliveBucket.GetValueOrDefault(((TwoDimensionalViewportParentData)childParentData__63116).vicinity), child)))
            {
                this._keepAliveBucket.remove(((TwoDimensionalViewportParentData)childParentData__63116).vicinity);
            }
            DartRuntimePrimitives.Assert(() => (!object.Equals(this._keepAliveBucket.GetValueOrDefault(((TwoDimensionalViewportParentData)childParentData__63116).vicinity), child)));
            dropChild(child);
            return;
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(this._keepAliveBucket.GetValueOrDefault(((TwoDimensionalViewportParentData)childParentData__63116).vicinity), child)));
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugDanglingKeepAlives.Remove(child);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._keepAliveBucket.remove(((TwoDimensionalViewportParentData)childParentData__63116).vicinity);
        dropChild(child);
    }

    internal virtual bool _debugTrackOrphans(global::Doroti.Generated.Framework.Rendering.RenderBox? newOrphan = null, global::Doroti.Generated.Framework.Rendering.RenderBox? noLongerOrphan = null)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                _debugOrphans ??= new List<global::Doroti.Generated.Framework.Rendering.RenderBox>();
                if ((newOrphan is not null))
                {
                    this._debugOrphans!.Add(newOrphan);
                }
                if ((noLongerOrphan is not null))
                {
                    this._debugOrphans!.Remove(noLongerOrphan);
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool debugThrowIfNotCheckingIntrinsics()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (!global::Doroti.Generated.Framework.Rendering.RenderObject.debugCheckingIntrinsics)
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this.GetType()} does not support returning intrinsic dimensions."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("Calculating the intrinsic dimensions would require instantiating every child of " + "the viewport, which defeats the point of viewports being lazy.") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        DartRuntimePrimitives.Assert(() => debugThrowIfNotCheckingIntrinsics());
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        DartRuntimePrimitives.Assert(() => debugThrowIfNotCheckingIntrinsics());
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        DartRuntimePrimitives.Assert(() => debugThrowIfNotCheckingIntrinsics());
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        DartRuntimePrimitives.Assert(() => debugThrowIfNotCheckingIntrinsics());
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void applyPaintTransform(global::Doroti.Generated.Framework.Rendering.RenderObject child, Matrix4 transform)
    {
        var __child = (global::Doroti.Generated.Framework.Rendering.RenderBox)(object)child;
        global::Doroti.Flutter.Ui.Offset paintOffset__65926 = ((global::Doroti.Flutter.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(parentDataOf(__child).paintOffset));
        transform.translate(paintOffset__65926.dx, paintOffset__65926.dy);
    }

    public override void dispose()
    {
        this._clipRectLayer.layer = null;
        base.dispose();
    }

}

public interface TwoDimensionalChildManager
{
    public void _startLayout();
    public void _buildChild(ChildVicinity vicinity);
    public void _reuseChild(ChildVicinity vicinity);
    public void _endLayout();
}

public class ChildVicinity : IComparable<ChildVicinity>
{
    public static ChildVicinity invalid = new ChildVicinity(xIndex: -1L, yIndex: -1L);
    public virtual long xIndex { get; private set; } = default!;
    public virtual long yIndex { get; private set; } = default!;

    public ChildVicinity(long xIndex, long yIndex)
    {
        this.xIndex = xIndex;
        this.yIndex = yIndex;
        System.Diagnostics.Debug.Assert((xIndex >= -1L));
        System.Diagnostics.Debug.Assert((yIndex >= -1L));
    }

    public override bool Equals(object? other)
    {
        var __other = other as ChildVicinity;
        if (__other is null) return false;
        return (((__other is ChildVicinity) && (((ChildVicinity)((ChildVicinity)__other)).xIndex == this.xIndex)) && (((ChildVicinity)((ChildVicinity)__other)).yIndex == this.yIndex));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.xIndex, this.yIndex));
    public virtual long compareTo(ChildVicinity other)
    {
        if ((this.xIndex == ((ChildVicinity)other).xIndex))
        {
            return (this.yIndex - ((ChildVicinity)other).yIndex);
        }
        return (this.xIndex - ((ChildVicinity)other).xIndex);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"(xIndex: {this.xIndex}, yIndex: {this.yIndex})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public int CompareTo(ChildVicinity? other) => checked((int)compareTo(other!));
}

