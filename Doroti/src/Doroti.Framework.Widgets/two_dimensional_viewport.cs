// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/two_dimensional_viewport.dart
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

public delegate Widget? TwoDimensionalIndexedWidgetBuilder(BuildContext context, ChildVicinity vicinity);

public abstract class TwoDimensionalViewport : RenderObjectWidget
{
    public virtual global::Doroti.Framework.Rendering.ViewportOffset verticalOffset { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.AxisDirection verticalAxisDirection { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.ViewportOffset horizontalOffset { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.AxisDirection horizontalAxisDirection { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.Axis mainAxis { get; private set; } = default!;
    public virtual double? cacheExtent { get; private set; }
    public virtual global::Doroti.Framework.Rendering.CacheExtentStyle? cacheExtentStyle { get; private set; }
    public virtual global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual TwoDimensionalChildDelegate @delegate { get; private set; } = default!;

    protected TwoDimensionalViewport(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Rendering.ViewportOffset verticalOffset = default!, global::Doroti.Framework.Painting.AxisDirection verticalAxisDirection = default!, global::Doroti.Framework.Rendering.ViewportOffset horizontalOffset = default!, global::Doroti.Framework.Painting.AxisDirection horizontalAxisDirection = default!, TwoDimensionalChildDelegate @delegate = default!, global::Doroti.Framework.Painting.Axis mainAxis = default!, double? cacheExtent = null, global::Doroti.Framework.Rendering.CacheExtentStyle? cacheExtentStyle = null, global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, Clip clipBehavior = Clip.hardEdge) : base(key: key)
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
        System.Diagnostics.Debug.Assert(((object.Equals(verticalAxisDirection, global::Doroti.Framework.Painting.AxisDirection.down)) || (object.Equals(verticalAxisDirection, global::Doroti.Framework.Painting.AxisDirection.up))));
        System.Diagnostics.Debug.Assert(((object.Equals(horizontalAxisDirection, global::Doroti.Framework.Painting.AxisDirection.left)) || (object.Equals(horizontalAxisDirection, global::Doroti.Framework.Painting.AxisDirection.right))));
    }

    public override RenderObjectElement createElement() => DartRuntimePrimitives.ConvertValue<RenderObjectElement>(new _TwoDimensionalViewportElement__two_dimensional_viewport(this));
    public abstract override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context);
    public abstract override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject);
}

internal class _TwoDimensionalViewportElement__two_dimensional_viewport : RenderObjectElement, NotifiableElementMixin, ViewportElementMixin, TwoDimensionalChildManager
{
    internal virtual DartMap<ChildVicinity, Element> _vicinityToChild { get; set; } = new DartMap<ChildVicinity, Element>();
    internal virtual DartMap<global::Doroti.Framework.Foundation.Key, Element> _keyToChild { get; set; } = new DartMap<global::Doroti.Framework.Foundation.Key, Element>();
    internal virtual DartMap<ChildVicinity, Element>? _newVicinityToChild { get; set; } = default;
    internal virtual DartMap<global::Doroti.Framework.Foundation.Key, Element>? _newKeyToChild { get; set; } = default;

    internal _TwoDimensionalViewportElement__two_dimensional_viewport(RenderObjectWidget widget) : base(widget)
    {
    }

    public override global::Doroti.Framework.Rendering.RenderObject renderObject => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(((RenderTwoDimensionalViewport?)(object?)base.renderObject)!);
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

    public override void insertRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)(object)child;
        var __slot = (ChildVicinity)(object)slot;
        ((dynamic)this.renderObject)._insertChild(__child, __slot);
    }

    public override void moveRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? oldSlot, object? newSlot)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)(object)child;
        var __oldSlot = (ChildVicinity)(object)oldSlot;
        var __newSlot = (ChildVicinity)(object)newSlot;
        ((dynamic)this.renderObject)._moveChild(__child, from: __oldSlot, to: __newSlot);
    }

    public override void removeRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)(object)child;
        var __slot = (ChildVicinity)(object)slot;
        ((dynamic)this.renderObject)._removeChild(__child, __slot);
    }

    public override void visitChildren(global::System.Action<Element> visitor)
    {
        this._vicinityToChild.Values.forEach((__arg0) => ((global::System.Action<Element>)visitor)(__arg0));
    }

    public override List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        List<Element> children = ((Func<List<Element>>)(() =>
{
    var __cascade = this._vicinityToChild.Values.ToList();
    __cascade.sort(_compareChildren);
    return __cascade;
}))().ToList();
        return new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static long _compareChildren(Element a, Element b)
    {
        var aSlot = ((ChildVicinity?)(object?)((Element)a).slot!)!;
        var bSlot = ((ChildVicinity?)(object?)((Element)b).slot!)!;
        return aSlot.compareTo(bSlot);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _debugIsDoingLayout => DartRuntimePrimitives.ConvertValue<bool>(((this._newKeyToChild is not null) && (this._newVicinityToChild is not null)));
    public virtual void _startLayout()
    {
        DartRuntimePrimitives.Assert(() => !this._debugIsDoingLayout);
        _newVicinityToChild = new DartMap<ChildVicinity, Element>().cast<ChildVicinity, Element>();
        _newKeyToChild = new DartMap<global::Doroti.Framework.Foundation.Key, Element>().cast<global::Doroti.Framework.Foundation.Key, Element>();
    }

    public virtual void _buildChild(ChildVicinity vicinity)
    {
        DartRuntimePrimitives.Assert(() => this._debugIsDoingLayout);
        this.owner!.buildScope(this, ((global::System.Action)(() =>
        {
            Widget? newWidget = ((Widget?)(object?)(((TwoDimensionalViewport?)(object?)this.widget)!).@delegate.build(this, vicinity));
            if ((newWidget is null))
            {
                return;
            }
            Element? oldElement = ((Element?)(object?)_retrieveOldElement(newWidget, vicinity));
            Element? newChild = ((Element?)(object?)updateChild(oldElement, newWidget, vicinity));
            DartRuntimePrimitives.Assert(() => (newChild is not null));
            DartRuntimePrimitives.Assert(() => (!this._newVicinityToChild!.ContainsKey(vicinity)));
            this._newVicinityToChild![vicinity] = newChild!;
            if ((((Widget)newWidget).key is not null))
            {
                DartRuntimePrimitives.Assert(() => (!this._newKeyToChild!.ContainsKey(((Widget)newWidget).key!)));
                this._newKeyToChild![((Widget)newWidget).key!] = newChild;
            }
        })));
    }

    internal virtual Element? _retrieveOldElement(Widget newWidget, ChildVicinity vicinity)
    {
        if ((((Widget)newWidget).key is not null))
        {
            Element? result = this._keyToChild.remove(((Widget)newWidget).key);
            if ((result is not null))
            {
                this._vicinityToChild.remove(((ChildVicinity)(object)((Element)result).slot));
            }
            return result;
        }
        Element? potentialOldElement = this._vicinityToChild.GetValueOrDefault(vicinity);
        if (((potentialOldElement is not null) && (((Element)potentialOldElement).widget.key is null)))
        {
            return this._vicinityToChild.remove(vicinity);
        }
        return ((Element)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _reuseChild(ChildVicinity vicinity)
    {
        DartRuntimePrimitives.Assert(() => this._debugIsDoingLayout);
        Element? elementToReuse = this._vicinityToChild.remove(vicinity);
        DartRuntimePrimitives.Assert(() => (elementToReuse is not null), () => (object?)$"Expected to re-use an element at {vicinity}, but none was found.");
        this._newVicinityToChild![vicinity] = elementToReuse!;
        if ((((Element)elementToReuse).widget.key is not null))
        {
            DartRuntimePrimitives.Assert(() => this._keyToChild.ContainsKey(((Element)elementToReuse).widget.key));
            DartRuntimePrimitives.Assert(() => (object.Equals(this._keyToChild.GetValueOrDefault(DartRuntimePrimitives.RequireReference(((Element)elementToReuse).widget.key)), elementToReuse)));
            this._newKeyToChild![((Element)elementToReuse).widget.key!] = this._keyToChild.remove(((Element)elementToReuse).widget.key)!;
        }
    }

    public virtual void _endLayout()
    {
        DartRuntimePrimitives.Assert(() => this._debugIsDoingLayout);
        foreach (Element element in this._vicinityToChild.Values)
        {
            if ((((Element)element).widget.key is null))
            {
                updateChild(element, ((Widget)(object)null), null);
            }
            else
            {
                DartRuntimePrimitives.Assert(() => this._keyToChild.containsValue(element));
            }
        }
        foreach (Element elementLocal in this._keyToChild.Values)
        {
            DartRuntimePrimitives.Assert(() => (((Element)elementLocal).widget.key is not null));
            updateChild(elementLocal, ((Widget)(object)null), null);
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

public class TwoDimensionalViewportParentData : global::Doroti.Framework.Rendering.ParentData, global::Doroti.Framework.Rendering.KeepAliveParentDataMixin
{
    public virtual Offset? layoutOffset { get; set; } = default;
    public virtual ChildVicinity vicinity { get; set; } = ChildVicinity.invalid;
    internal virtual Size? _paintExtent { get; set; } = default;
    internal virtual global::Doroti.Framework.Rendering.RenderBox? _previousSibling { get; set; } = default;
    internal virtual global::Doroti.Framework.Rendering.RenderBox? _nextSibling { get; set; } = default;
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
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("The paint extent of the child has not been determined yet."), new global::Doroti.Framework.Foundation.ErrorDescription("The paint extent, and therefore the visibility, of a child of a " + "RenderTwoDimensionalViewport is computed after " + "RenderTwoDimensionalViewport.layoutChildSequence.") }));
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

public abstract class RenderTwoDimensionalViewport : global::Doroti.Framework.Rendering.RenderBox
{
    internal virtual global::Doroti.Framework.Rendering.ViewportOffset _horizontalOffset { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.AxisDirection _horizontalAxisDirection { get; set; } = default!;
    internal virtual global::Doroti.Framework.Rendering.ViewportOffset _verticalOffset { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.AxisDirection _verticalAxisDirection { get; set; } = default!;
    internal virtual TwoDimensionalChildDelegate _delegate { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.Axis _mainAxis { get; set; } = default!;
    internal virtual global::Doroti.Framework.Rendering.ScrollCacheExtent _scrollCacheExtent { get; set; } = default!;
    internal virtual Clip _clipBehavior { get; set; } = default!;
    internal virtual TwoDimensionalChildManager _childManager { get; private set; } = default!;
    internal virtual DartMap<ChildVicinity, global::Doroti.Framework.Rendering.RenderBox> _children { get; private set; } = new DartMap<ChildVicinity, global::Doroti.Framework.Rendering.RenderBox>();
    internal virtual DartMap<ChildVicinity, global::Doroti.Framework.Rendering.RenderBox> _activeChildrenForLayoutPass { get; private set; } = new DartMap<ChildVicinity, global::Doroti.Framework.Rendering.RenderBox>();
    internal virtual DartMap<ChildVicinity, global::Doroti.Framework.Rendering.RenderBox> _keepAliveBucket { get; private set; } = new DartMap<ChildVicinity, global::Doroti.Framework.Rendering.RenderBox>();
    internal virtual List<global::Doroti.Framework.Rendering.RenderBox> _debugDanglingKeepAlives { get; set; } = default!;
    internal virtual bool _hasVisualOverflow { get; set; } = false;
    internal virtual global::Doroti.Framework.Rendering.LayerHandle<global::Doroti.Framework.Rendering.ClipRectLayer> _clipRectLayer { get; private set; } = new global::Doroti.Framework.Rendering.LayerHandle<global::Doroti.Framework.Rendering.ClipRectLayer>();
    internal virtual List<ChildVicinity> _currentChildVicinities { get; private set; } = new List<ChildVicinity>();
    internal virtual global::Doroti.Framework.Rendering.RenderBox? _firstChild { get; set; } = default;
    internal virtual global::Doroti.Framework.Rendering.RenderBox? _lastChild { get; set; } = default;
    internal virtual bool _didResize { get; set; } = true;
    internal virtual bool _needsDelegateRebuild { get; set; } = true;
    internal virtual List<global::Doroti.Framework.Rendering.RenderBox>? _debugOrphans { get; set; } = default;

    protected RenderTwoDimensionalViewport(global::Doroti.Framework.Rendering.ViewportOffset horizontalOffset, global::Doroti.Framework.Painting.AxisDirection horizontalAxisDirection, global::Doroti.Framework.Rendering.ViewportOffset verticalOffset, global::Doroti.Framework.Painting.AxisDirection verticalAxisDirection, TwoDimensionalChildDelegate @delegate, global::Doroti.Framework.Painting.Axis mainAxis, TwoDimensionalChildManager childManager, double? cacheExtent = null, global::Doroti.Framework.Rendering.CacheExtentStyle? cacheExtentStyle = null, global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, Clip clipBehavior = Clip.hardEdge)
    {
        this._childManager = childManager;
        this._horizontalOffset = horizontalOffset;
        this._horizontalAxisDirection = horizontalAxisDirection;
        this._verticalOffset = verticalOffset;
        this._verticalAxisDirection = verticalAxisDirection;
        this._delegate = @delegate;
        this._mainAxis = mainAxis;
        this._scrollCacheExtent = (scrollCacheExtent ?? (((cacheExtent is not null) ? (cacheExtentStyle switch { global::Doroti.Framework.Rendering.CacheExtentStyle.pixel => global::Doroti.Framework.Rendering.ScrollCacheExtent.CreatePixels(DartRuntimePrimitives.RequireValue(cacheExtent)), null => global::Doroti.Framework.Rendering.ScrollCacheExtent.CreatePixels(DartRuntimePrimitives.RequireValue(cacheExtent)), global::Doroti.Framework.Rendering.CacheExtentStyle.viewport => global::Doroti.Framework.Rendering.ScrollCacheExtent.CreateViewport(DartRuntimePrimitives.RequireValue(cacheExtent)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }) : global::Doroti.Framework.Rendering.ScrollCacheExtent.CreatePixels(global::Doroti.Framework.Rendering.RenderAbstractViewport.defaultCacheExtent))));
        this._clipBehavior = clipBehavior;
        System.Diagnostics.Debug.Assert(((object.Equals(verticalAxisDirection, global::Doroti.Framework.Painting.AxisDirection.down)) || (object.Equals(verticalAxisDirection, global::Doroti.Framework.Painting.AxisDirection.up))));
        System.Diagnostics.Debug.Assert(((object.Equals(horizontalAxisDirection, global::Doroti.Framework.Painting.AxisDirection.left)) || (object.Equals(horizontalAxisDirection, global::Doroti.Framework.Painting.AxisDirection.right))));
    }

    public virtual global::Doroti.Framework.Rendering.ViewportOffset horizontalOffset
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
                this._horizontalOffset.removeListener(this.markNeedsLayout);
            }
            _horizontalOffset = __value;
            if (this.attached)
            {
                this._horizontalOffset.addListener(this.markNeedsLayout);
            }
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Painting.AxisDirection horizontalAxisDirection
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
    public virtual global::Doroti.Framework.Rendering.ViewportOffset verticalOffset
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
                this._verticalOffset.removeListener(this.markNeedsLayout);
            }
            _verticalOffset = __value;
            if (this.attached)
            {
                this._verticalOffset.addListener(this.markNeedsLayout);
            }
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Painting.AxisDirection verticalAxisDirection
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
                this._delegate.removeListener(this._handleDelegateNotification);
            }
            TwoDimensionalChildDelegate oldDelegate = this._delegate;
            _delegate = __value;
            if (this.attached)
            {
                this._delegate.addListener(this._handleDelegateNotification);
            }
            if (((!object.Equals(DartRuntimePrimitives.RuntimeType(this._delegate), DartRuntimePrimitives.RuntimeType(oldDelegate))) || this._delegate.shouldRebuild(oldDelegate)))
            {
                _handleDelegateNotification();
            }
        }
    }
    public virtual global::Doroti.Framework.Painting.Axis mainAxis
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
        get => ((global::Doroti.Framework.Rendering.ScrollCacheExtent)this._scrollCacheExtent).value;
        set
        {
            double? __value = value;
            if ((__value == this.cacheExtent))
            {
                return;
            }
            if ((__value is null))
            {
                _scrollCacheExtent = global::Doroti.Framework.Rendering.ScrollCacheExtent.CreatePixels(global::Doroti.Framework.Rendering.RenderAbstractViewport.defaultCacheExtent);
            }
            else
            {
                _scrollCacheExtent = (this.cacheExtentStyle switch { global::Doroti.Framework.Rendering.CacheExtentStyle.pixel => global::Doroti.Framework.Rendering.ScrollCacheExtent.CreatePixels(DartRuntimePrimitives.RequireValue(__value)), global::Doroti.Framework.Rendering.CacheExtentStyle.viewport => global::Doroti.Framework.Rendering.ScrollCacheExtent.CreateViewport(DartRuntimePrimitives.RequireValue(__value)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            }
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Rendering.CacheExtentStyle cacheExtentStyle
    {
        get => ((global::Doroti.Framework.Rendering.ScrollCacheExtent)this._scrollCacheExtent).style;
        set
        {
            global::Doroti.Framework.Rendering.CacheExtentStyle? __value = value;
            if ((object.Equals(__value, this.cacheExtentStyle)))
            {
                return;
            }
            if ((__value is null))
            {
                _scrollCacheExtent = global::Doroti.Framework.Rendering.ScrollCacheExtent.CreatePixels(this.cacheExtent);
            }
            else
            {
                _scrollCacheExtent = (DartRuntimePrimitives.RequireValue(__value) switch { global::Doroti.Framework.Rendering.CacheExtentStyle.pixel => global::Doroti.Framework.Rendering.ScrollCacheExtent.CreatePixels(this.cacheExtent), global::Doroti.Framework.Rendering.CacheExtentStyle.viewport => global::Doroti.Framework.Rendering.ScrollCacheExtent.CreateViewport(this.cacheExtent), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            }
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Rendering.ScrollCacheExtent scrollCacheExtent
    {
        get => this._scrollCacheExtent;
        set
        {
            global::Doroti.Framework.Rendering.ScrollCacheExtent? __value = value;
            if ((object.Equals(this._scrollCacheExtent, __value)))
            {
                return;
            }
            if ((__value is null))
            {
                _scrollCacheExtent = global::Doroti.Framework.Rendering.ScrollCacheExtent.CreatePixels(global::Doroti.Framework.Rendering.RenderAbstractViewport.defaultCacheExtent);
            }
            else
            {
                _scrollCacheExtent = __value;
            }
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.Clip clipBehavior
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
    public virtual global::Doroti.Framework.Rendering.RenderBox? firstChild => this._firstChild;
    public virtual global::Doroti.Framework.Rendering.RenderBox? lastChild => this._lastChild;
    public virtual global::Doroti.Framework.Rendering.RenderBox? childBefore(global::Doroti.Framework.Rendering.RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        return parentDataOf(child)._previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Rendering.RenderBox? childAfter(global::Doroti.Framework.Rendering.RenderBox child)
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

    public override void setupParentData(global::Doroti.Framework.Rendering.RenderObject child)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)(object)child;
        if ((__child.parentData is not TwoDimensionalViewportParentData))
        {
            __child.parentData = new TwoDimensionalViewportParentData();
        }
    }

    public virtual TwoDimensionalViewportParentData parentDataOf(global::Doroti.Framework.Rendering.RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => ((this._children.containsValue(child) || this._keepAliveBucket.containsValue(child)) || this._debugOrphans!.Contains(child)));
        return ((TwoDimensionalViewportParentData?)(object?)child.parentData!)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Rendering.RenderBox? getChildFor(ChildVicinity vicinity) => this._children.GetValueOrDefault(vicinity);
    public override void attach(global::Doroti.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        this._horizontalOffset.addListener(this.markNeedsLayout);
        this._verticalOffset.addListener(this.markNeedsLayout);
        this._delegate.addListener(this._handleDelegateNotification);
        foreach (global::Doroti.Framework.Rendering.RenderBox child in this._children.Values)
        {
            child.attach(owner);
        }
        foreach (global::Doroti.Framework.Rendering.RenderBox childLocal in this._keepAliveBucket.Values)
        {
            childLocal.attach(owner);
        }
    }

    public override void detach()
    {
        base.detach();
        this._horizontalOffset.removeListener(this.markNeedsLayout);
        this._verticalOffset.removeListener(this.markNeedsLayout);
        this._delegate.removeListener(this._handleDelegateNotification);
        foreach (global::Doroti.Framework.Rendering.RenderBox child in this._children.Values)
        {
            child.detach();
        }
        foreach (global::Doroti.Framework.Rendering.RenderBox childLocal in this._keepAliveBucket.Values)
        {
            childLocal.detach();
        }
    }

    public override void redepthChildren()
    {
        foreach (global::Doroti.Framework.Rendering.RenderBox child in this._children.Values)
        {
            child.redepthChildren();
        }
        this._keepAliveBucket.Values.forEach((__arg0) => ((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)this.redepthChild)(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(__arg0)));
    }

    public override void visitChildren(global::System.Action<global::Doroti.Framework.Rendering.RenderObject> visitor)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            visitor(child);
            child = parentDataOf(child)._nextSibling;
        }
        this._keepAliveBucket.Values.forEach((__arg0) => ((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)visitor)(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(__arg0)));
    }

    public override void visitChildrenForSemantics(global::System.Action<global::Doroti.Framework.Rendering.RenderObject> visitor)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            TwoDimensionalViewportParentData childParentData = ((TwoDimensionalViewportParentData)(object?)parentDataOf(child));
            visitor(child);
            child = ((TwoDimensionalViewportParentData)childParentData)._nextSibling;
        }
    }

    public override List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var debugChildren = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        return debugChildren;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Rendering.DebugLibrary.debugCheckHasBoundedAxis(global::Doroti.Framework.Painting.Axis.vertical, constraints));
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Rendering.DebugLibrary.debugCheckHasBoundedAxis(global::Doroti.Framework.Painting.Axis.horizontal, constraints));
        return ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).biggest;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestChildren(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        foreach (global::Doroti.Framework.Rendering.RenderBox child in this._children.Values)
        {
            TwoDimensionalViewportParentData childParentData = ((TwoDimensionalViewportParentData)(object?)parentDataOf(child));
            if (!((TwoDimensionalViewportParentData)childParentData).isVisible)
            {
                continue;
            }
            bool isHit = result.addWithPaintOffset(offset: ((TwoDimensionalViewportParentData)childParentData).paintOffset, position: position, hitTest: ((global::System.Func<global::Doroti.Framework.Rendering.BoxHitTestResult, Offset, bool>)((result, transformed) =>
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - DartRuntimePrimitives.RequireValue(((TwoDimensionalViewportParentData)childParentData).paintOffset)))));
                return child.hitTest(result, position: transformed);
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
            if (isHit)
            {
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Size viewportDimension
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
        global::Doroti.Ui.Size? oldSize = ((global::Doroti.Ui.Size?)(object?)(this.hasSize ? this.size : null));
        base.performResize();
        this.horizontalOffset.applyViewportDimension(this.size.width);
        this.verticalOffset.applyViewportDimension(this.size.height);
        if ((!object.Equals(oldSize, this.size)))
        {
            _didResize = true;
        }
    }

    public virtual global::Doroti.Framework.Rendering.RevealedOffset getOffsetToReveal(global::Doroti.Framework.Rendering.RenderObject target, double alignment, Rect? rect = null, global::Doroti.Framework.Painting.Axis? axis = null)
    {
        axis ??= this.mainAxis;
        var (offsetLocal, axisDirection) = (DartRuntimePrimitives.RequireValue(axis) switch { global::Doroti.Framework.Painting.Axis.vertical => (((double, global::Doroti.Framework.Painting.AxisDirection))((((global::Doroti.Framework.Rendering.ViewportOffset)this.verticalOffset).pixels, this.verticalAxisDirection))), global::Doroti.Framework.Painting.Axis.horizontal => (((double, global::Doroti.Framework.Painting.AxisDirection))((((global::Doroti.Framework.Rendering.ViewportOffset)this.horizontalOffset).pixels, this.horizontalAxisDirection))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        rect ??= ((global::Doroti.Framework.Rendering.RenderObject)target).paintBounds;
        var child = target;
        while ((!object.Equals(((global::Doroti.Framework.Rendering.RenderObject)child).parent, this)))
        {
            child = ((global::Doroti.Framework.Rendering.RenderObject)child).parent!;
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(((global::Doroti.Framework.Rendering.RenderObject)child).parent, this)));
        var box = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)child)!;
        global::Doroti.Ui.Rect rectLocal = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(((Matrix4)((dynamic)target).getTransformTo(((global::Doroti.Framework.Rendering.RenderBox)child))), DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(rect))));
        var leadingScrollOffset = offsetLocal;
        leadingScrollOffset += (DartRuntimePrimitives.RequireValue(axisDirection) switch { global::Doroti.Framework.Painting.AxisDirection.up => (((global::Doroti.Framework.Rendering.RenderBox)((global::Doroti.Framework.Rendering.RenderBox)child)).size.height - rectLocal.bottom), global::Doroti.Framework.Painting.AxisDirection.left => (((global::Doroti.Framework.Rendering.RenderBox)((global::Doroti.Framework.Rendering.RenderBox)child)).size.width - rectLocal.right), global::Doroti.Framework.Painting.AxisDirection.right => rectLocal.left, global::Doroti.Framework.Painting.AxisDirection.down => rectLocal.top, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Ui.Offset paintOffsetLocal = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(parentDataOf(box).paintOffset));
        leadingScrollOffset += (DartRuntimePrimitives.RequireValue(axisDirection) switch { global::Doroti.Framework.Painting.AxisDirection.up => ((this.viewportDimension.height - paintOffsetLocal.dy) - ((global::Doroti.Framework.Rendering.RenderBox)box).size.height), global::Doroti.Framework.Painting.AxisDirection.left => ((this.viewportDimension.width - paintOffsetLocal.dx) - ((global::Doroti.Framework.Rendering.RenderBox)box).size.width), global::Doroti.Framework.Painting.AxisDirection.right => paintOffsetLocal.dx, global::Doroti.Framework.Painting.AxisDirection.down => paintOffsetLocal.dy, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        Matrix4 transform = ((Matrix4)(object?)((Matrix4)((dynamic)target).getTransformTo(this)));
        global::Doroti.Ui.Rect targetRect = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(transform, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(rect))));
        double mainAxisExtentDifference = (DartRuntimePrimitives.RequireValue(axis) switch { global::Doroti.Framework.Painting.Axis.horizontal => (this.viewportDimension.width - rectLocal.width), global::Doroti.Framework.Painting.Axis.vertical => (this.viewportDimension.height - rectLocal.height), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        double targetOffset = (leadingScrollOffset - (mainAxisExtentDifference * alignment));
        double offsetDifference = (DartRuntimePrimitives.RequireValue(axis) switch { global::Doroti.Framework.Painting.Axis.horizontal => (((global::Doroti.Framework.Rendering.ViewportOffset)this.horizontalOffset).pixels - targetOffset), global::Doroti.Framework.Painting.Axis.vertical => (((global::Doroti.Framework.Rendering.ViewportOffset)this.verticalOffset).pixels - targetOffset), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        targetRect = (DartRuntimePrimitives.RequireValue(axisDirection) switch { global::Doroti.Framework.Painting.AxisDirection.up => targetRect.translate(0.0, -offsetDifference), global::Doroti.Framework.Painting.AxisDirection.down => targetRect.translate(0.0, offsetDifference), global::Doroti.Framework.Painting.AxisDirection.left => targetRect.translate(-offsetDifference, 0.0), global::Doroti.Framework.Painting.AxisDirection.right => targetRect.translate(offsetDifference, 0.0), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        var revealedOffset = new global::Doroti.Framework.Rendering.RevealedOffset(offset: targetOffset, rect: targetRect);
        return revealedOffset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void showOnScreen(global::Doroti.Framework.Rendering.RenderObject? descendant = null, Rect? rect = null, Duration duration = default, global::Doroti.Framework.Animation.Curve curve = default!)
    {
        bool allowHorizontal = ((global::Doroti.Framework.Rendering.ViewportOffset)this.horizontalOffset).allowImplicitScrolling;
        bool allowVertical = ((global::Doroti.Framework.Rendering.ViewportOffset)this.verticalOffset).allowImplicitScrolling;
        global::Doroti.Framework.Painting.AxisDirection? axisDirectionLocal = default!;
        switch ((allowHorizontal, allowVertical))
        {
            case (true, true):
                {
                    break;
                }
            case (false, true):
                {
                    axisDirectionLocal = this.verticalAxisDirection;
                    break;
                }
            case (true, false):
                {
                    axisDirectionLocal = this.horizontalAxisDirection;
                    break;
                }
            case (false, false):
                {
                    base.showOnScreen(descendant: descendant, rect: rect, duration: duration, curve: curve);
                    return;
                }
        }
        global::Doroti.Ui.Rect? newRect = ((global::Doroti.Ui.Rect?)(object?)RenderTwoDimensionalViewport.showInViewport(descendant: descendant, viewport: this, axisDirection: axisDirectionLocal, rect: rect, duration: duration, curve: curve));
        base.showOnScreen(rect: newRect, duration: duration, curve: curve);
    }

    public static global::Doroti.Ui.Rect? showInViewport(global::Doroti.Framework.Rendering.RenderObject? descendant = null, Rect? rect = null, RenderTwoDimensionalViewport viewport = default!, Duration duration = default, global::Doroti.Framework.Animation.Curve curve = default!, global::Doroti.Framework.Painting.AxisDirection? axisDirection = null)
    {
        if ((descendant is null))
        {
            return rect;
        }
        Rect? showVertical(Rect? rect)
        {
            return RenderTwoDimensionalViewport._showInViewportForAxisDirection(descendant: descendant, viewport: viewport, axis: global::Doroti.Framework.Painting.Axis.vertical, rect: rect, duration: duration, curve: curve);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        Rect? showHorizontal(Rect? rect)
        {
            return RenderTwoDimensionalViewport._showInViewportForAxisDirection(descendant: descendant, viewport: viewport, axis: global::Doroti.Framework.Painting.Axis.horizontal, rect: rect, duration: duration, curve: curve);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        switch (axisDirection)
        {
            case global::Doroti.Framework.Painting.AxisDirection.left:
            case global::Doroti.Framework.Painting.AxisDirection.right:
                {
                    return showHorizontal(rect);
                }
            case global::Doroti.Framework.Painting.AxisDirection.up:
            case global::Doroti.Framework.Painting.AxisDirection.down:
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
                        Matrix4 transform = ((Matrix4)(object?)((Matrix4)((dynamic)descendant).getTransformTo(viewport.parent)));
                        return ((global::Doroti.Ui.Rect?)(object?)MatrixUtils.transformRect(transform, ((rect ?? (Rect)((global::Doroti.Framework.Rendering.RenderObject)descendant).paintBounds))));
                    }
                    return DartRuntimePrimitives.RequireValue(rect);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Ui.Rect? _showInViewportForAxisDirection(global::Doroti.Framework.Rendering.RenderObject descendant, Rect? rect = null, RenderTwoDimensionalViewport viewport = default!, global::Doroti.Framework.Painting.Axis axis = default!, Duration duration = default, global::Doroti.Framework.Animation.Curve curve = default!)
    {
        global::Doroti.Framework.Rendering.ViewportOffset offsetLocal = (DartRuntimePrimitives.RequireValue(axis) switch { global::Doroti.Framework.Painting.Axis.vertical => ((RenderTwoDimensionalViewport)viewport).verticalOffset, global::Doroti.Framework.Painting.Axis.horizontal => ((RenderTwoDimensionalViewport)viewport).horizontalOffset, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Framework.Rendering.RevealedOffset leadingEdgeOffsetLocal = ((global::Doroti.Framework.Rendering.RevealedOffset)(object?)viewport.getOffsetToReveal(descendant, 0.0, rect: rect, axis: DartRuntimePrimitives.RequireValue(axis)));
        global::Doroti.Framework.Rendering.RevealedOffset trailingEdgeOffsetLocal = ((global::Doroti.Framework.Rendering.RevealedOffset)(object?)viewport.getOffsetToReveal(descendant, 1.0, rect: rect, axis: DartRuntimePrimitives.RequireValue(axis)));
        double currentOffsetLocal = ((global::Doroti.Framework.Rendering.ViewportOffset)offsetLocal).pixels;
        global::Doroti.Framework.Rendering.RevealedOffset? targetOffset = ((global::Doroti.Framework.Rendering.RevealedOffset?)(object?)RevealedOffset.clampOffset(leadingEdgeOffset: leadingEdgeOffsetLocal, trailingEdgeOffset: trailingEdgeOffsetLocal, currentOffset: currentOffsetLocal));
        if ((targetOffset is null))
        {
            return ((global::Doroti.Ui.Rect)(object)null);
        }
        DartRuntimePrimitives.Ignore(offsetLocal.moveTo(((global::Doroti.Framework.Rendering.RevealedOffset)targetOffset).offset, duration: duration, curve: curve));
        return ((global::Doroti.Framework.Rendering.RevealedOffset)targetOffset).rect;
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
        invokeLayoutCallback<global::Doroti.Framework.Rendering.BoxConstraints>(((global::System.Action<global::Doroti.Framework.Rendering.BoxConstraints>)((_) =>
        {
            this._childManager._endLayout();
            DartRuntimePrimitives.Assert(() => ((this._debugOrphans is { } __items48761 ? !System.Linq.Enumerable.Any(__items48761) : (bool?)null) ?? true));
            DartRuntimePrimitives.Assert(() => !System.Linq.Enumerable.Any(this._debugDanglingKeepAlives));
            DartRuntimePrimitives.Assert(() => !System.Linq.Enumerable.Any(this._keepAliveBucket.Values.where(((child) =>
            {
                return !parentDataOf(child).keepAlive;
                throw new InvalidOperationException("Dart closure completed without a value.");
            }))));
            _reifyChildren();
        })));
    }

    internal virtual void _cacheKeepAlives()
    {
        List<global::Doroti.Framework.Rendering.RenderBox> remainingChildren = this._children.Values.toSet().difference<global::Doroti.Framework.Rendering.RenderBox>(this._activeChildrenForLayoutPass.Values.toSet()).ToList().ToList();
        foreach (var child in remainingChildren)
        {
            TwoDimensionalViewportParentData childParentData = ((TwoDimensionalViewportParentData)(object?)parentDataOf(child));
            if (childParentData.keepAlive)
            {
                this._keepAliveBucket[((TwoDimensionalViewportParentData)childParentData).vicinity] = child;
                this._childManager._reuseChild(((TwoDimensionalViewportParentData)childParentData).vicinity);
            }
        }
    }

    internal virtual void _sortByYIndex()
    {
        this._currentChildVicinities.sort(((a, b) =>
        {
            long yComparison = ((ChildVicinity)a).yIndex.CompareTo(((ChildVicinity)b).yIndex);
            if ((yComparison != 0L))
            {
                return yComparison;
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
        global::Doroti.Framework.Rendering.RenderBox? previousChildLocal = default!;
        switch (this.mainAxis)
        {
            case global::Doroti.Framework.Painting.Axis.vertical:
                {
                    _sortByYIndex();
                    break;
                }
            case global::Doroti.Framework.Painting.Axis.horizontal:
                {
                    _sortByXIndex();
                    break;
                }
        }
        foreach (ChildVicinity vicinity in this._currentChildVicinities)
        {
            previousChildLocal = (_completeChildParentData(vicinity, previousChild: previousChildLocal) ?? previousChildLocal);
        }
        _lastChild = previousChildLocal;
        if ((this._lastChild is not null))
        {
            parentDataOf(this._lastChild!)._nextSibling = null;
        }
        this._currentChildVicinities.Clear();
    }

    internal virtual global::Doroti.Framework.Rendering.RenderBox? _completeChildParentData(ChildVicinity vicinity, global::Doroti.Framework.Rendering.RenderBox? previousChild = null)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(vicinity, ChildVicinity.invalid)));
        if (this._children.ContainsKey(vicinity))
        {
            global::Doroti.Framework.Rendering.RenderBox child = this._children.GetValueOrDefault(vicinity)!;
            DartRuntimePrimitives.Assert(() => (object.Equals(parentDataOf(child).vicinity, vicinity)));
            updateChildPaintData(child);
            if ((previousChild is null))
            {
                DartRuntimePrimitives.Assert(() => (this._firstChild is null));
                _firstChild = child;
            }
            else
            {
                parentDataOf(previousChild)._nextSibling = child;
                parentDataOf(child)._previousSibling = previousChild;
            }
            return child;
        }
        return ((global::Doroti.Framework.Rendering.RenderBox)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _debugCheckContentDimensions()
    {
        var hint = "Subclasses should call applyContentDimensions on the " + "verticalOffset and horizontalOffset to set the min and max scroll offset. " + "If the contents exceed one or both sides of the viewportDimension, " + "ensure the viewportDimension height or width is subtracted in that axis " + "for the correct extent.";
        DartRuntimePrimitives.Assert(() =>
            {
                if (!(((ScrollPosition?)(object?)this.verticalOffset)!).hasContentDimensions)
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("The verticalOffset was not given content dimensions during " + "layoutChildSequence."), new global::Doroti.Framework.Foundation.ErrorHint(hint) }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() =>
            {
                if (!(((ScrollPosition?)(object?)this.horizontalOffset)!).hasContentDimensions)
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("The horizontalOffset was not given content dimensions during " + "layoutChildSequence."), new global::Doroti.Framework.Foundation.ErrorHint(hint) }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Rendering.RenderBox? buildOrObtainChildFor(ChildVicinity vicinity)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(vicinity, ChildVicinity.invalid)));
        DartRuntimePrimitives.Assert(() => this.debugDoingThisLayout);
        if ((this._needsDelegateRebuild || ((!this._children.ContainsKey(vicinity) && !this._keepAliveBucket.ContainsKey(vicinity)))))
        {
            invokeLayoutCallback<global::Doroti.Framework.Rendering.BoxConstraints>(((global::System.Action<global::Doroti.Framework.Rendering.BoxConstraints>)((_) =>
            {
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
            return ((global::Doroti.Framework.Rendering.RenderBox)(object)null);
        }
        DartRuntimePrimitives.Assert(() => this._children.ContainsKey(vicinity));
        global::Doroti.Framework.Rendering.RenderBox child = this._children.GetValueOrDefault(vicinity)!;
        this._activeChildrenForLayoutPass[vicinity] = child;
        parentDataOf(child).vicinity = vicinity;
        this._currentChildVicinities.Add(vicinity);
        return child;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void updateChildPaintData(global::Doroti.Framework.Rendering.RenderBox child)
    {
        TwoDimensionalViewportParentData childParentData = ((TwoDimensionalViewportParentData)(object?)parentDataOf(child));
        DartRuntimePrimitives.Assert(() => (((TwoDimensionalViewportParentData)childParentData).layoutOffset is not null), () => (object?)$"The child with ChildVicinity(xIndex: {((TwoDimensionalViewportParentData)childParentData).vicinity.xIndex}, " + $"yIndex: {((TwoDimensionalViewportParentData)childParentData).vicinity.yIndex}) was not provided a " + "layoutOffset. This should be set during layoutChildSequence, " + "representing the position of the child.");
        DartRuntimePrimitives.Assert(() => ((global::Doroti.Framework.Rendering.RenderBox)child).hasSize);
        childParentData._paintExtent = computeChildPaintExtent(DartRuntimePrimitives.RequireValue(((TwoDimensionalViewportParentData)childParentData).layoutOffset), ((global::Doroti.Framework.Rendering.RenderBox)child).size);
        childParentData.paintOffset = computeAbsolutePaintOffsetFor(child, layoutOffset: DartRuntimePrimitives.RequireValue(((TwoDimensionalViewportParentData)childParentData).layoutOffset));
        _hasVisualOverflow = ((this._hasVisualOverflow || (!object.Equals(((TwoDimensionalViewportParentData)childParentData).layoutOffset, ((TwoDimensionalViewportParentData)childParentData)._paintExtent))) || !((TwoDimensionalViewportParentData)childParentData).isVisible);
    }

    public virtual global::Doroti.Ui.Size computeChildPaintExtent(Offset layoutOffset, Size childSize)
    {
        if ((((object.Equals(childSize, Size.zero)) || (childSize.height == 0.0)) || (childSize.width == 0.0)))
        {
            return Size.zero;
        }
        double widthLocal = default!;
        if ((layoutOffset.dx < 0.0))
        {
            if (((layoutOffset.dx + childSize.width) <= 0.0))
            {
                return Size.zero;
            }
            widthLocal = (layoutOffset.dx + childSize.width);
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
                    widthLocal = (this.viewportDimension.width - layoutOffset.dx);
                }
                else
                {
                    DartRuntimePrimitives.Assert(() => ((layoutOffset.dx + childSize.width) <= this.viewportDimension.width));
                    widthLocal = childSize.width;
                }
            }
        }
        double heightLocal = default!;
        if ((layoutOffset.dy < 0.0))
        {
            if (((layoutOffset.dy + childSize.height) <= 0.0))
            {
                return Size.zero;
            }
            heightLocal = (layoutOffset.dy + childSize.height);
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
                    heightLocal = (this.viewportDimension.height - layoutOffset.dy);
                }
                else
                {
                    DartRuntimePrimitives.Assert(() => ((layoutOffset.dy + childSize.height) <= this.viewportDimension.height));
                    heightLocal = childSize.height;
                }
            }
        }
        return new global::Doroti.Ui.Size(widthLocal, heightLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Offset computeAbsolutePaintOffsetFor(global::Doroti.Framework.Rendering.RenderBox child, Offset layoutOffset)
    {
        DartRuntimePrimitives.Assert(() => this.hasSize);
        DartRuntimePrimitives.Assert(() => ((global::Doroti.Framework.Rendering.RenderBox)child).hasSize);
        double xOffset = (this.horizontalAxisDirection switch { global::Doroti.Framework.Painting.AxisDirection.right => layoutOffset.dx, global::Doroti.Framework.Painting.AxisDirection.left => (this.viewportDimension.width - ((layoutOffset.dx + ((global::Doroti.Framework.Rendering.RenderBox)child).size.width))), global::Doroti.Framework.Painting.AxisDirection.up => throw new Exception("This should not happen"), global::Doroti.Framework.Painting.AxisDirection.down => throw new Exception("This should not happen"), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        double yOffset = (this.verticalAxisDirection switch { global::Doroti.Framework.Painting.AxisDirection.up => (this.viewportDimension.height - ((layoutOffset.dy + ((global::Doroti.Framework.Rendering.RenderBox)child).size.height))), global::Doroti.Framework.Painting.AxisDirection.down => layoutOffset.dy, global::Doroti.Framework.Painting.AxisDirection.right => throw new Exception("This should not happen"), global::Doroti.Framework.Painting.AxisDirection.left => throw new Exception("This should not happen"), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return new global::Doroti.Ui.Offset(xOffset, yOffset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        if (!System.Linq.Enumerable.Any(this._children))
        {
            return;
        }
        if ((this._hasVisualOverflow && (!object.Equals(this.clipBehavior, Clip.none))))
        {
            this._clipRectLayer.layer = context.pushClipRect(this.needsCompositing, offset, (Offset.zero & this.viewportDimension), (global::System.Action<global::Doroti.Framework.Rendering.PaintingContext, Offset>)this._paintChildren, clipBehavior: this.clipBehavior, oldLayer: ((global::Doroti.Framework.Rendering.LayerHandle<global::Doroti.Framework.Rendering.ClipRectLayer>)this._clipRectLayer).layer);
        }
        else
        {
            this._clipRectLayer.layer = null;
            _paintChildren(context, offset);
        }
    }

    internal virtual void _paintChildren(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            TwoDimensionalViewportParentData childParentData = ((TwoDimensionalViewportParentData)(object?)parentDataOf(child));
            if (((TwoDimensionalViewportParentData)childParentData).isVisible)
            {
                context.paintChild(child, (offset + DartRuntimePrimitives.RequireValue(((TwoDimensionalViewportParentData)childParentData).paintOffset)));
            }
            child = ((TwoDimensionalViewportParentData)childParentData)._nextSibling;
        }
    }

    internal virtual void _insertChild(global::Doroti.Framework.Rendering.RenderBox child, ChildVicinity slot)
    {
        DartRuntimePrimitives.Assert(() => _debugTrackOrphans(newOrphan: this._children.GetValueOrDefault(slot)));
        DartRuntimePrimitives.Assert(() => !this._keepAliveBucket.containsValue(child));
        this._children[slot] = child;
        adoptChild(child);
    }

    internal virtual void _moveChild(global::Doroti.Framework.Rendering.RenderBox child, ChildVicinity from, ChildVicinity to)
    {
        TwoDimensionalViewportParentData childParentData = ((TwoDimensionalViewportParentData)(object?)parentDataOf(child));
        if (!((TwoDimensionalViewportParentData)childParentData).keptAlive)
        {
            if ((object.Equals(this._children.GetValueOrDefault(from), child)))
            {
                this._children.remove(from);
            }
            DartRuntimePrimitives.Assert(() => _debugTrackOrphans(newOrphan: this._children.GetValueOrDefault(to), noLongerOrphan: child));
            this._children[to] = child;
            return;
        }
        if ((object.Equals(this._keepAliveBucket.GetValueOrDefault(((TwoDimensionalViewportParentData)childParentData).vicinity), child)))
        {
            this._keepAliveBucket.remove(((TwoDimensionalViewportParentData)childParentData).vicinity);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugDanglingKeepAlives.Remove(child);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() =>
            {
                if (this._keepAliveBucket.ContainsKey(((TwoDimensionalViewportParentData)childParentData).vicinity))
                {
                    this._debugDanglingKeepAlives.Add(this._keepAliveBucket.GetValueOrDefault(((TwoDimensionalViewportParentData)childParentData).vicinity)!);
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._keepAliveBucket[((TwoDimensionalViewportParentData)childParentData).vicinity] = child;
    }

    internal virtual void _removeChild(global::Doroti.Framework.Rendering.RenderBox child, ChildVicinity slot)
    {
        TwoDimensionalViewportParentData childParentData = ((TwoDimensionalViewportParentData)(object?)parentDataOf(child));
        if (!((TwoDimensionalViewportParentData)childParentData).keptAlive)
        {
            if ((object.Equals(this._children.GetValueOrDefault(slot), child)))
            {
                this._children.remove(slot);
            }
            DartRuntimePrimitives.Assert(() => _debugTrackOrphans(noLongerOrphan: child));
            if ((object.Equals(this._keepAliveBucket.GetValueOrDefault(((TwoDimensionalViewportParentData)childParentData).vicinity), child)))
            {
                this._keepAliveBucket.remove(((TwoDimensionalViewportParentData)childParentData).vicinity);
            }
            DartRuntimePrimitives.Assert(() => (!object.Equals(this._keepAliveBucket.GetValueOrDefault(((TwoDimensionalViewportParentData)childParentData).vicinity), child)));
            dropChild(child);
            return;
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(this._keepAliveBucket.GetValueOrDefault(((TwoDimensionalViewportParentData)childParentData).vicinity), child)));
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugDanglingKeepAlives.Remove(child);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._keepAliveBucket.remove(((TwoDimensionalViewportParentData)childParentData).vicinity);
        dropChild(child);
    }

    internal virtual bool _debugTrackOrphans(global::Doroti.Framework.Rendering.RenderBox? newOrphan = null, global::Doroti.Framework.Rendering.RenderBox? noLongerOrphan = null)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                _debugOrphans ??= new List<global::Doroti.Framework.Rendering.RenderBox>();
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
                if (!global::Doroti.Framework.Rendering.RenderObject.debugCheckingIntrinsics)
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this.GetType()} does not support returning intrinsic dimensions."), new global::Doroti.Framework.Foundation.ErrorDescription("Calculating the intrinsic dimensions would require instantiating every child of " + "the viewport, which defeats the point of viewports being lazy.") }));
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

    public override void applyPaintTransform(global::Doroti.Framework.Rendering.RenderObject child, Matrix4 transform)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)(object)child;
        global::Doroti.Ui.Offset paintOffsetLocal = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(parentDataOf(__child).paintOffset));
        transform.translate(paintOffsetLocal.dx, paintOffsetLocal.dy);
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

