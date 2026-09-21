// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/two_dimensional_viewport.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public delegate Widget? TwoDimensionalIndexedWidgetBuilder(
    BuildContext context,
    ChildVicinity vicinity
);

public abstract class TwoDimensionalViewport : RenderObjectWidget
{
    public virtual ViewportOffset verticalOffset { get; private set; } = default!;
    public virtual AxisDirection verticalAxisDirection { get; private set; } = default!;
    public virtual ViewportOffset horizontalOffset { get; private set; } = default!;
    public virtual AxisDirection horizontalAxisDirection { get; private set; } = default!;
    public virtual Axis mainAxis { get; private set; } = default!;
    public virtual double? cacheExtent { get; private set; }
    public virtual CacheExtentStyle? cacheExtentStyle { get; private set; }
    public virtual ScrollCacheExtent? scrollCacheExtent { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual TwoDimensionalChildDelegate @delegate { get; private set; } = default!;

    protected TwoDimensionalViewport(
        Key? key = null,
        ViewportOffset verticalOffset = default!,
        AxisDirection verticalAxisDirection = default!,
        ViewportOffset horizontalOffset = default!,
        AxisDirection horizontalAxisDirection = default!,
        TwoDimensionalChildDelegate @delegate = default!,
        Axis mainAxis = default!,
        double? cacheExtent = null,
        CacheExtentStyle? cacheExtentStyle = null,
        ScrollCacheExtent? scrollCacheExtent = null,
        Clip clipBehavior = Clip.hardEdge
    )
        : base(key: key)
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
        System.Diagnostics.Debug.Assert(
            Equals(verticalAxisDirection, AxisDirection.down)
                || Equals(verticalAxisDirection, AxisDirection.up)
        );
        System.Diagnostics.Debug.Assert(
            Equals(horizontalAxisDirection, AxisDirection.left)
                || Equals(horizontalAxisDirection, AxisDirection.right)
        );
    }

    public override RenderObjectElement createElement() =>
        DartRuntimePrimitives.ConvertValue<RenderObjectElement>(
            new _TwoDimensionalViewportElement__two_dimensional_viewport(this)
        );

    public abstract override RenderObject createRenderObject(BuildContext context);
    public abstract override void updateRenderObject(
        BuildContext context,
        RenderObject renderObject
    );
}

internal class _TwoDimensionalViewportElement__two_dimensional_viewport
    : RenderObjectElement,
        NotifiableElementMixin,
        ViewportElementMixin,
        TwoDimensionalChildManager
{
    internal virtual DartMap<ChildVicinity, Element> _vicinityToChild { get; set; } =
        new DartMap<ChildVicinity, Element>();
    internal virtual DartMap<Key, Element> _keyToChild { get; set; } = new DartMap<Key, Element>();
    internal virtual DartMap<ChildVicinity, Element>? _newVicinityToChild { get; set; } = default;
    internal virtual DartMap<Key, Element>? _newKeyToChild { get; set; } = default;

    internal _TwoDimensionalViewportElement__two_dimensional_viewport(RenderObjectWidget widget)
        : base(widget) { }

    public override RenderTwoDimensionalViewport renderObject =>
        (RenderTwoDimensionalViewport)base.renderObject;

    public override void performRebuild()
    {
        base.performRebuild();
        renderObject.markNeedsLayout(withDelegateRebuild: true);
    }

    public override void forgetChild(Element child)
    {
        DartRuntimePrimitives.Assert(() => !_debugIsDoingLayout);
        base.forgetChild(child);
        _vicinityToChild.remove(
            child.slot as ChildVicinity
                ?? throw new InvalidOperationException("The viewport child has no vicinity.")
        );
        if (child.widget.key is not null)
        {
            _keyToChild.remove(child.widget.key);
        }
    }

    public override void insertRenderObjectChild(RenderObject child, object? slot)
    {
        var __child = (RenderBox)child;
        var __slot =
            slot as ChildVicinity
            ?? throw new ArgumentException("A viewport child requires a vicinity.", nameof(slot));
        renderObject._insertChild(__child, __slot);
    }

    public override void moveRenderObjectChild(RenderObject child, object? oldSlot, object? newSlot)
    {
        var __child = (RenderBox)child;
        var __oldSlot =
            oldSlot as ChildVicinity
            ?? throw new ArgumentException(
                "A viewport child requires a vicinity.",
                nameof(oldSlot)
            );
        var __newSlot =
            newSlot as ChildVicinity
            ?? throw new ArgumentException(
                "A viewport child requires a vicinity.",
                nameof(newSlot)
            );
        renderObject._moveChild(__child, from: __oldSlot, to: __newSlot);
    }

    public override void removeRenderObjectChild(RenderObject child, object? slot)
    {
        var __child = (RenderBox)child;
        var __slot =
            slot as ChildVicinity
            ?? throw new ArgumentException("A viewport child requires a vicinity.", nameof(slot));
        renderObject._removeChild(__child, __slot);
    }

    public override void visitChildren(Action<Element> visitor)
    {
        _vicinityToChild.Values.forEach((__arg0) => visitor(__arg0));
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        List<Element> children = (
            (Func<List<Element>>)(
                () =>
                {
                    var __cascade = _vicinityToChild.Values.ToList();
                    __cascade.sort(_compareChildren);
                    return __cascade;
                }
            )
        )().ToList();
        return new List<DiagnosticsNode>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static long _compareChildren(Element a, Element b)
    {
        var aSlot = ((ChildVicinity?)a.slot!)!;
        var bSlot = ((ChildVicinity?)b.slot!)!;
        return aSlot.compareTo(bSlot);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _debugIsDoingLayout =>
        DartRuntimePrimitives.ConvertValue<bool>(
            (_newKeyToChild is not null) && (_newVicinityToChild is not null)
        );

    public virtual void _startLayout()
    {
        DartRuntimePrimitives.Assert(() => !_debugIsDoingLayout);
        _newVicinityToChild = new DartMap<ChildVicinity, Element>().cast<ChildVicinity, Element>();
        _newKeyToChild = new DartMap<Key, Element>().cast<Key, Element>();
    }

    public virtual void _buildChild(ChildVicinity vicinity)
    {
        DartRuntimePrimitives.Assert(() => _debugIsDoingLayout);
        owner!.buildScope(
            this,
            () =>
            {
                Widget? newWidget = ((TwoDimensionalViewport?)widget)!.@delegate.build(
                    this,
                    vicinity
                );
                if (newWidget is null)
                {
                    return;
                }
                Element? oldElement = _retrieveOldElement(newWidget, vicinity);
                Element newChild =
                    updateChild(oldElement, newWidget, vicinity)
                    ?? throw new InvalidOperationException(
                        "Updating a non-null widget must produce an element."
                    );
                DartRuntimePrimitives.Assert(() => newChild is not null);
                DartRuntimePrimitives.Assert(() => !_newVicinityToChild!.ContainsKey(vicinity));
                _newVicinityToChild![vicinity] = newChild!;
                if (newWidget.key is not null)
                {
                    DartRuntimePrimitives.Assert(() =>
                        !_newKeyToChild!.ContainsKey(newWidget.key!)
                    );
                    _newKeyToChild![newWidget.key!] = newChild;
                }
            }
        );
    }

    internal virtual Element? _retrieveOldElement(Widget newWidget, ChildVicinity vicinity)
    {
        if (newWidget.key is not null)
        {
            Element? result = _keyToChild.remove(newWidget.key);
            if (result is not null)
            {
                _vicinityToChild.remove(
                    result.slot as ChildVicinity
                        ?? throw new InvalidOperationException(
                            "The viewport child has no vicinity."
                        )
                );
            }
            return result;
        }
        Element? potentialOldElement = _vicinityToChild.GetValueOrDefault(vicinity);
        if ((potentialOldElement is not null) && (potentialOldElement.widget.key is null))
        {
            return _vicinityToChild.remove(vicinity);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _reuseChild(ChildVicinity vicinity)
    {
        DartRuntimePrimitives.Assert(() => _debugIsDoingLayout);
        Element elementToReuse =
            _vicinityToChild.remove(vicinity)
            ?? throw new InvalidOperationException($"No element exists at {vicinity} to reuse.");
        DartRuntimePrimitives.Assert(
            () => elementToReuse is not null,
            () => (object?)$"Expected to re-use an element at {vicinity}, but none was found."
        );
        _newVicinityToChild![vicinity] = elementToReuse!;
        if (elementToReuse.widget.key is not null)
        {
            DartRuntimePrimitives.Assert(() => _keyToChild.ContainsKey(elementToReuse.widget.key));
            DartRuntimePrimitives.Assert(() =>
                Equals(
                    _keyToChild.GetValueOrDefault(
                        DartRuntimePrimitives.RequireReference(elementToReuse.widget.key)
                    ),
                    elementToReuse
                )
            );
            _newKeyToChild![elementToReuse.widget.key!] = _keyToChild.remove(
                elementToReuse.widget.key
            )!;
        }
    }

    public virtual void _endLayout()
    {
        DartRuntimePrimitives.Assert(() => _debugIsDoingLayout);
        foreach (Element element in _vicinityToChild.Values)
        {
            if (element.widget.key is null)
            {
                updateChild(element, null, null);
            }
            else
            {
                DartRuntimePrimitives.Assert(() => _keyToChild.containsValue(element));
            }
        }
        foreach (Element elementLocal in _keyToChild.Values)
        {
            DartRuntimePrimitives.Assert(() => elementLocal.widget.key is not null);
            updateChild(elementLocal, null, null);
        }
        _vicinityToChild = _newVicinityToChild!;
        _keyToChild = _newKeyToChild!;
        _newVicinityToChild = null;
        _newKeyToChild = null;
        DartRuntimePrimitives.Assert(() => !_debugIsDoingLayout);
    }

    public override void attachNotificationTree()
    {
        _notificationTree = new _NotificationNode__framework(_parent?._notificationTree, this);
    }

    public virtual bool onNotification(Notification notification)
    {
        if (notification is ViewportNotificationMixin)
        {
            ((ViewportNotificationMixin)notification)._depth += 1L;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class TwoDimensionalViewportParentData : ParentData, KeepAliveParentDataMixin
{
    public virtual Offset? layoutOffset { get; set; } = default;
    public virtual ChildVicinity vicinity { get; set; } = ChildVicinity.invalid;
    internal virtual Size? _paintExtent { get; set; } = default;
    internal virtual RenderBox? _previousSibling { get; set; } = default;
    internal virtual RenderBox? _nextSibling { get; set; } = default;
    public virtual Offset? paintOffset { get; set; } = default;
    public virtual bool keepAlive { get; set; } = false;

    public virtual bool isVisible
    {
        get
        {
            DartRuntimePrimitives.Assert(() =>
            {
                if (_paintExtent is null)
                {
                    throw DartRuntimePrimitives.AsException(
                        new FlutterError(
                            new List<DiagnosticsNode>
                            {
                                new ErrorSummary(
                                    "The paint extent of the child has not been determined yet."
                                ),
                                new ErrorDescription(
                                    "The paint extent, and therefore the visibility, of a child of a "
                                        + "RenderTwoDimensionalViewport is computed after "
                                        + "RenderTwoDimensionalViewport.layoutChildSequence."
                                ),
                            }
                        )
                    );
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
            return (!Equals(_paintExtent, Size.zero))
                || (DartRuntimePrimitives.RequireValue(_paintExtent).height != 0.0)
                || (DartRuntimePrimitives.RequireValue(_paintExtent).width != 0.0);
        }
    }
    public virtual bool keptAlive =>
        DartRuntimePrimitives.ConvertValue<bool>(keepAlive && !isVisible);

    public override string ToString()
    {
        return $"vicinity={vicinity}; "
            + $"layoutOffset={layoutOffset}; "
            + $"paintOffset={paintOffset}; "
            + $"{((_paintExtent is null) ? "not visible; " : $"{(!isVisible ? "not " : "")}visible - paintExtent={_paintExtent}; ")}"
            + $"{(keepAlive ? "keepAlive; " : "")}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public abstract class RenderTwoDimensionalViewport : RenderBox
{
    internal virtual ViewportOffset _horizontalOffset { get; set; } = default!;
    internal virtual AxisDirection _horizontalAxisDirection { get; set; } = default!;
    internal virtual ViewportOffset _verticalOffset { get; set; } = default!;
    internal virtual AxisDirection _verticalAxisDirection { get; set; } = default!;
    internal virtual TwoDimensionalChildDelegate _delegate { get; set; } = default!;
    internal virtual Axis _mainAxis { get; set; } = default!;
    internal virtual ScrollCacheExtent _scrollCacheExtent { get; set; } = default!;
    internal virtual Clip _clipBehavior { get; set; } = default!;
    internal virtual TwoDimensionalChildManager _childManager { get; private set; } = default!;
    internal virtual DartMap<ChildVicinity, RenderBox> _children { get; private set; } =
        new DartMap<ChildVicinity, RenderBox>();
    internal virtual DartMap<ChildVicinity, RenderBox> _activeChildrenForLayoutPass
    {
        get;
        private set;
    } = new DartMap<ChildVicinity, RenderBox>();
    internal virtual DartMap<ChildVicinity, RenderBox> _keepAliveBucket { get; private set; } =
        new DartMap<ChildVicinity, RenderBox>();
    internal virtual List<RenderBox> _debugDanglingKeepAlives { get; set; } = default!;
    internal virtual bool _hasVisualOverflow { get; set; } = false;
    internal virtual LayerHandle<ClipRectLayer> _clipRectLayer { get; private set; } =
        new LayerHandle<ClipRectLayer>();
    internal virtual List<ChildVicinity> _currentChildVicinities { get; private set; } =
        new List<ChildVicinity>();
    internal virtual RenderBox? _firstChild { get; set; } = default;
    internal virtual RenderBox? _lastChild { get; set; } = default;
    internal virtual bool _didResize { get; set; } = true;
    internal virtual bool _needsDelegateRebuild { get; set; } = true;
    internal virtual List<RenderBox>? _debugOrphans { get; set; } = default;

    protected RenderTwoDimensionalViewport(
        ViewportOffset horizontalOffset,
        AxisDirection horizontalAxisDirection,
        ViewportOffset verticalOffset,
        AxisDirection verticalAxisDirection,
        TwoDimensionalChildDelegate @delegate,
        Axis mainAxis,
        TwoDimensionalChildManager childManager,
        double? cacheExtent = null,
        CacheExtentStyle? cacheExtentStyle = null,
        ScrollCacheExtent? scrollCacheExtent = null,
        Clip clipBehavior = Clip.hardEdge
    )
    {
        _childManager = childManager;
        _horizontalOffset = horizontalOffset;
        _horizontalAxisDirection = horizontalAxisDirection;
        _verticalOffset = verticalOffset;
        _verticalAxisDirection = verticalAxisDirection;
        _delegate = @delegate;
        _mainAxis = mainAxis;
        _scrollCacheExtent =
            scrollCacheExtent
            ?? (
                (cacheExtent is not null)
                    ? (
                        cacheExtentStyle switch
                        {
                            CacheExtentStyle.pixel => ScrollCacheExtent.CreatePixels(
                                DartRuntimePrimitives.RequireValue(cacheExtent)
                            ),
                            null => ScrollCacheExtent.CreatePixels(
                                DartRuntimePrimitives.RequireValue(cacheExtent)
                            ),
                            CacheExtentStyle.viewport => ScrollCacheExtent.CreateViewport(
                                DartRuntimePrimitives.RequireValue(cacheExtent)
                            ),
                            _ => throw new InvalidOperationException(
                                "Non-exhaustive Dart switch value."
                            ),
                        }
                    )
                    : ScrollCacheExtent.CreatePixels(RenderAbstractViewport.defaultCacheExtent)
            );
        _clipBehavior = clipBehavior;
        System.Diagnostics.Debug.Assert(
            Equals(verticalAxisDirection, AxisDirection.down)
                || Equals(verticalAxisDirection, AxisDirection.up)
        );
        System.Diagnostics.Debug.Assert(
            Equals(horizontalAxisDirection, AxisDirection.left)
                || Equals(horizontalAxisDirection, AxisDirection.right)
        );
    }

    public virtual ViewportOffset horizontalOffset
    {
        get => _horizontalOffset;
        set
        {
            var __value = value;
            if (Equals(_horizontalOffset, __value))
            {
                return;
            }
            if (attached)
            {
                _horizontalOffset.removeListener(markNeedsLayout);
            }
            _horizontalOffset = __value;
            if (attached)
            {
                _horizontalOffset.addListener(markNeedsLayout);
            }
            markNeedsLayout();
        }
    }
    public virtual AxisDirection horizontalAxisDirection
    {
        get => _horizontalAxisDirection;
        set
        {
            var __value = value;
            if (Equals(_horizontalAxisDirection, DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _horizontalAxisDirection = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual ViewportOffset verticalOffset
    {
        get => _verticalOffset;
        set
        {
            var __value = value;
            if (Equals(_verticalOffset, __value))
            {
                return;
            }
            if (attached)
            {
                _verticalOffset.removeListener(markNeedsLayout);
            }
            _verticalOffset = __value;
            if (attached)
            {
                _verticalOffset.addListener(markNeedsLayout);
            }
            markNeedsLayout();
        }
    }
    public virtual AxisDirection verticalAxisDirection
    {
        get => _verticalAxisDirection;
        set
        {
            var __value = value;
            if (Equals(_verticalAxisDirection, DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _verticalAxisDirection = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual TwoDimensionalChildDelegate @delegate
    {
        get => _delegate;
        set
        {
            var __value = value;
            if (Equals(_delegate, __value))
            {
                return;
            }
            if (attached)
            {
                _delegate.removeListener(_handleDelegateNotification);
            }
            TwoDimensionalChildDelegate oldDelegate = _delegate;
            _delegate = __value;
            if (attached)
            {
                _delegate.addListener(_handleDelegateNotification);
            }
            if (
                (
                    !Equals(
                        DartRuntimePrimitives.RuntimeType(_delegate),
                        DartRuntimePrimitives.RuntimeType(oldDelegate)
                    )
                ) || _delegate.shouldRebuild(oldDelegate)
            )
            {
                _handleDelegateNotification();
            }
        }
    }
    public virtual Axis mainAxis
    {
        get => _mainAxis;
        set
        {
            var __value = value;
            if (Equals(_mainAxis, DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _mainAxis = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual double cacheExtent
    {
        get => _scrollCacheExtent.value;
        set
        {
            double? __value = value;
            if (__value == cacheExtent)
            {
                return;
            }
            if (__value is null)
            {
                _scrollCacheExtent = ScrollCacheExtent.CreatePixels(
                    RenderAbstractViewport.defaultCacheExtent
                );
            }
            else
            {
                _scrollCacheExtent = cacheExtentStyle switch
                {
                    CacheExtentStyle.pixel => ScrollCacheExtent.CreatePixels(
                        DartRuntimePrimitives.RequireValue(__value)
                    ),
                    CacheExtentStyle.viewport => ScrollCacheExtent.CreateViewport(
                        DartRuntimePrimitives.RequireValue(__value)
                    ),
                    _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
                };
            }
            markNeedsLayout();
        }
    }
    public virtual CacheExtentStyle cacheExtentStyle
    {
        get => _scrollCacheExtent.style;
        set
        {
            CacheExtentStyle? __value = value;
            if (Equals(__value, cacheExtentStyle))
            {
                return;
            }
            if (__value is null)
            {
                _scrollCacheExtent = ScrollCacheExtent.CreatePixels(cacheExtent);
            }
            else
            {
                _scrollCacheExtent = DartRuntimePrimitives.RequireValue(__value) switch
                {
                    CacheExtentStyle.pixel => ScrollCacheExtent.CreatePixels(cacheExtent),
                    CacheExtentStyle.viewport => ScrollCacheExtent.CreateViewport(cacheExtent),
                    _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
                };
            }
            markNeedsLayout();
        }
    }
    public virtual ScrollCacheExtent scrollCacheExtent
    {
        get => _scrollCacheExtent;
        set
        {
            ScrollCacheExtent? __value = value;
            if (Equals(_scrollCacheExtent, __value))
            {
                return;
            }
            if (__value is null)
            {
                _scrollCacheExtent = ScrollCacheExtent.CreatePixels(
                    RenderAbstractViewport.defaultCacheExtent
                );
            }
            else
            {
                _scrollCacheExtent = __value;
            }
            markNeedsLayout();
        }
    }
    public virtual Clip clipBehavior
    {
        get => _clipBehavior;
        set
        {
            var __value = value;
            if (Equals(_clipBehavior, DartRuntimePrimitives.RequireValue(__value)))
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
    public virtual RenderBox? firstChild => _firstChild;
    public virtual RenderBox? lastChild => _lastChild;

    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        return parentDataOf(child)._previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        return parentDataOf(child)._nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleDelegateNotification()
    {
        markNeedsLayout(withDelegateRebuild: true);
        return;
    }

    public override void setupParentData(RenderObject child)
    {
        var __child = (RenderBox)child;
        if (__child.parentData is not TwoDimensionalViewportParentData)
        {
            __child.parentData = new TwoDimensionalViewportParentData();
        }
    }

    public virtual TwoDimensionalViewportParentData parentDataOf(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() =>
            _children.containsValue(child)
            || _keepAliveBucket.containsValue(child)
            || _debugOrphans!.Contains(child)
        );
        return ((TwoDimensionalViewportParentData?)child.parentData!)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? getChildFor(ChildVicinity vicinity) =>
        _children.GetValueOrDefault(vicinity);

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        _horizontalOffset.addListener(markNeedsLayout);
        _verticalOffset.addListener(markNeedsLayout);
        _delegate.addListener(_handleDelegateNotification);
        foreach (RenderBox child in _children.Values)
        {
            child.attach(owner);
        }
        foreach (RenderBox childLocal in _keepAliveBucket.Values)
        {
            childLocal.attach(owner);
        }
    }

    public override void detach()
    {
        base.detach();
        _horizontalOffset.removeListener(markNeedsLayout);
        _verticalOffset.removeListener(markNeedsLayout);
        _delegate.removeListener(_handleDelegateNotification);
        foreach (RenderBox child in _children.Values)
        {
            child.detach();
        }
        foreach (RenderBox childLocal in _keepAliveBucket.Values)
        {
            childLocal.detach();
        }
    }

    public override void redepthChildren()
    {
        foreach (RenderBox child in _children.Values)
        {
            child.redepthChildren();
        }
        _keepAliveBucket.Values.forEach(
            (__arg0) =>
                ((Action<RenderObject>)redepthChild)(
                    DartRuntimePrimitives.ConvertValue<RenderObject>(__arg0)
                )
        );
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            visitor(child);
            child = parentDataOf(child)._nextSibling;
        }
        _keepAliveBucket.Values.forEach(
            (__arg0) => visitor(DartRuntimePrimitives.ConvertValue<RenderObject>(__arg0))
        );
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            TwoDimensionalViewportParentData childParentData = parentDataOf(child);
            visitor(child);
            child = childParentData._nextSibling;
        }
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        var debugChildren = new List<DiagnosticsNode>();
        return debugChildren;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() =>
            Rendering.DebugLibrary.debugCheckHasBoundedAxis(Axis.vertical, constraints)
        );
        DartRuntimePrimitives.Assert(() =>
            Rendering.DebugLibrary.debugCheckHasBoundedAxis(Axis.horizontal, constraints)
        );
        return constraints.biggest;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        foreach (RenderBox child in _children.Values)
        {
            TwoDimensionalViewportParentData childParentData = parentDataOf(child);
            if (!childParentData.isVisible)
            {
                continue;
            }
            bool isHit = result.addWithPaintOffset(
                offset: childParentData.paintOffset,
                position: position,
                hitTest: (result, transformed) =>
                {
                    DartRuntimePrimitives.Assert(() =>
                        Equals(
                            transformed,
                            position
                                - DartRuntimePrimitives.RequireValue(childParentData.paintOffset)
                        )
                    );
                    return child.hitTest(result, position: transformed);
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }
            );
            if (isHit)
            {
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Size viewportDimension
    {
        get
        {
            DartRuntimePrimitives.Assert(() => hasSize);
            return size;
        }
    }

    public override void performResize()
    {
        Size? oldSize = hasSize ? size : null;
        base.performResize();
        horizontalOffset.applyViewportDimension(size.width);
        verticalOffset.applyViewportDimension(size.height);
        if (!Equals(oldSize, size))
        {
            _didResize = true;
        }
    }

    public virtual RevealedOffset getOffsetToReveal(
        RenderObject target,
        double alignment,
        Rect? rect = null,
        Axis? axis = null
    )
    {
        axis ??= mainAxis;
        var (offsetLocal, axisDirection) = DartRuntimePrimitives.RequireValue(axis) switch
        {
            Axis.vertical => (verticalOffset.pixels, verticalAxisDirection),
            Axis.horizontal => ((double, AxisDirection))
                (horizontalOffset.pixels, horizontalAxisDirection),
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        rect ??= target.paintBounds;
        var child = target;
        while (!Equals(child.parent, this))
        {
            child = child.parent!;
        }
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var box = ((RenderBox?)child)!;
        Rect rectLocal = MatrixUtils.transformRect(
            target.getTransformTo((RenderBox)child),
            DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(rect))
        );
        var leadingScrollOffset = offsetLocal;
        leadingScrollOffset += DartRuntimePrimitives.RequireValue(axisDirection) switch
        {
            AxisDirection.up => ((RenderBox)child).size.height - rectLocal.bottom,
            AxisDirection.left => ((RenderBox)child).size.width - rectLocal.right,
            AxisDirection.right => rectLocal.left,
            AxisDirection.down => rectLocal.top,
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        Offset paintOffsetLocal = DartRuntimePrimitives.RequireValue(parentDataOf(box).paintOffset);
        leadingScrollOffset += DartRuntimePrimitives.RequireValue(axisDirection) switch
        {
            AxisDirection.up => viewportDimension.height - paintOffsetLocal.dy - box.size.height,
            AxisDirection.left => viewportDimension.width - paintOffsetLocal.dx - box.size.width,
            AxisDirection.right => paintOffsetLocal.dx,
            AxisDirection.down => paintOffsetLocal.dy,
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        Matrix4 transform = target.getTransformTo(this);
        Rect targetRect = MatrixUtils.transformRect(
            transform,
            DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(rect))
        );
        double mainAxisExtentDifference = DartRuntimePrimitives.RequireValue(axis) switch
        {
            Axis.horizontal => viewportDimension.width - rectLocal.width,
            Axis.vertical => viewportDimension.height - rectLocal.height,
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        double targetOffset = leadingScrollOffset - (mainAxisExtentDifference * alignment);
        double offsetDifference = DartRuntimePrimitives.RequireValue(axis) switch
        {
            Axis.horizontal => horizontalOffset.pixels - targetOffset,
            Axis.vertical => verticalOffset.pixels - targetOffset,
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        targetRect = DartRuntimePrimitives.RequireValue(axisDirection) switch
        {
            AxisDirection.up => targetRect.translate(0.0, -offsetDifference),
            AxisDirection.down => targetRect.translate(0.0, offsetDifference),
            AxisDirection.left => targetRect.translate(-offsetDifference, 0.0),
            AxisDirection.right => targetRect.translate(offsetDifference, 0.0),
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        var revealedOffset = new RevealedOffset(offset: targetOffset, rect: targetRect);
        return revealedOffset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void showOnScreen(
        RenderObject? descendant = null,
        Rect? rect = null,
        Duration duration = default,
        Curve curve = default!
    )
    {
        bool allowHorizontal = horizontalOffset.allowImplicitScrolling;
        bool allowVertical = verticalOffset.allowImplicitScrolling;
        AxisDirection? axisDirectionLocal = default!;
        switch ((allowHorizontal, allowVertical))
        {
            case (true, true):
            {
                break;
            }
            case (false, true):
            {
                axisDirectionLocal = verticalAxisDirection;
                break;
            }
            case (true, false):
            {
                axisDirectionLocal = horizontalAxisDirection;
                break;
            }
            case (false, false):
            {
                base.showOnScreen(
                    descendant: descendant,
                    rect: rect,
                    duration: duration,
                    curve: curve
                );
                return;
            }
        }
        Rect? newRect = showInViewport(
            descendant: descendant,
            viewport: this,
            axisDirection: axisDirectionLocal,
            rect: rect,
            duration: duration,
            curve: curve
        );
        base.showOnScreen(rect: newRect, duration: duration, curve: curve);
    }

    public static Rect? showInViewport(
        RenderObject? descendant = null,
        Rect? rect = null,
        RenderTwoDimensionalViewport viewport = default!,
        Duration duration = default,
        Curve curve = default!,
        AxisDirection? axisDirection = null
    )
    {
        if (descendant is null)
        {
            return rect;
        }
        Rect? showVertical(Rect? rect)
        {
            return _showInViewportForAxisDirection(
                descendant: descendant,
                viewport: viewport,
                axis: Axis.vertical,
                rect: rect,
                duration: duration,
                curve: curve
            );
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        Rect? showHorizontal(Rect? rect)
        {
            return _showInViewportForAxisDirection(
                descendant: descendant,
                viewport: viewport,
                axis: Axis.horizontal,
                rect: rect,
                duration: duration,
                curve: curve
            );
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        switch (axisDirection)
        {
            case AxisDirection.left:
            case AxisDirection.right:
            {
                return showHorizontal(rect);
            }
            case AxisDirection.up:
            case AxisDirection.down:
            {
                return showVertical(rect);
            }
            case null:
            {
                rect = showHorizontal(rect) ?? rect;
                rect = showVertical(rect);
                if (rect is null)
                {
                    DartRuntimePrimitives.Assert(() => viewport.parent is not null);
                    Matrix4 transform = descendant.getTransformTo(viewport.parent);
                    return (Rect?)
                        (object?)
                            MatrixUtils.transformRect(transform, rect ?? descendant.paintBounds);
                }
                return DartRuntimePrimitives.RequireValue(rect);
            }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static Rect? _showInViewportForAxisDirection(
        RenderObject descendant,
        Rect? rect = null,
        RenderTwoDimensionalViewport viewport = default!,
        Axis axis = default!,
        Duration duration = default,
        Curve curve = default!
    )
    {
        ViewportOffset offsetLocal = DartRuntimePrimitives.RequireValue(axis) switch
        {
            Axis.vertical => viewport.verticalOffset,
            Axis.horizontal => viewport.horizontalOffset,
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        RevealedOffset leadingEdgeOffsetLocal = viewport.getOffsetToReveal(
            descendant,
            0.0,
            rect: rect,
            axis: DartRuntimePrimitives.RequireValue(axis)
        );
        RevealedOffset trailingEdgeOffsetLocal = viewport.getOffsetToReveal(
            descendant,
            1.0,
            rect: rect,
            axis: DartRuntimePrimitives.RequireValue(axis)
        );
        double currentOffsetLocal = offsetLocal.pixels;
        RevealedOffset? targetOffset = RevealedOffset.clampOffset(
            leadingEdgeOffset: leadingEdgeOffsetLocal,
            trailingEdgeOffset: trailingEdgeOffsetLocal,
            currentOffset: currentOffsetLocal
        );
        if (targetOffset is null)
        {
            return null;
        }
        DartRuntimePrimitives.Ignore(
            offsetLocal.moveTo(targetOffset.offset, duration: duration, curve: curve)
        );
        return targetOffset.rect;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool didResize => _didResize;
    public virtual bool needsDelegateRebuild => _needsDelegateRebuild;

    public override void markNeedsLayout() => markNeedsLayout(withDelegateRebuild: false);

    public virtual void markNeedsLayout(bool withDelegateRebuild)
    {
        _needsDelegateRebuild = _needsDelegateRebuild || withDelegateRebuild;
        base.markNeedsLayout();
    }

    public abstract void layoutChildSequence();

    public override void performLayout()
    {
        _firstChild = null;
        _lastChild = null;
        _activeChildrenForLayoutPass.Clear();
        _childManager._startLayout();
        layoutChildSequence();
        DartRuntimePrimitives.Assert(() => _debugCheckContentDimensions());
        _didResize = false;
        _needsDelegateRebuild = false;
        _cacheKeepAlives();
        invokeLayoutCallback<BoxConstraints>(
            (_) =>
            {
                _childManager._endLayout();
                DartRuntimePrimitives.Assert(() =>
                    (
                        _debugOrphans is { } __items48761
                            ? !Enumerable.Any(__items48761)
                            : (bool?)null
                    ) ?? true
                );
                DartRuntimePrimitives.Assert(() => !Enumerable.Any(_debugDanglingKeepAlives));
                DartRuntimePrimitives.Assert(() =>
                    !Enumerable.Any(
                        _keepAliveBucket.Values.where(
                            (child) =>
                            {
                                return !parentDataOf(child).keepAlive;
                                throw new InvalidOperationException(
                                    "Dart closure completed without a value."
                                );
                            }
                        )
                    )
                );
                _reifyChildren();
            }
        );
    }

    internal virtual void _cacheKeepAlives()
    {
        List<RenderBox> remainingChildren = _children
            .Values.toSet()
            .difference(_activeChildrenForLayoutPass.Values.toSet())
            .ToList()
            .ToList();
        foreach (var child in remainingChildren)
        {
            TwoDimensionalViewportParentData childParentData = parentDataOf(child);
            if (childParentData.keepAlive)
            {
                _keepAliveBucket[childParentData.vicinity] = child;
                _childManager._reuseChild(childParentData.vicinity);
            }
        }
    }

    internal virtual void _sortByYIndex()
    {
        _currentChildVicinities.sort(
            (a, b) =>
            {
                long yComparison = a.yIndex.CompareTo(b.yIndex);
                if (yComparison != 0L)
                {
                    return yComparison;
                }
                return a.xIndex.CompareTo(b.xIndex);
                throw new InvalidOperationException("Dart closure completed without a value.");
            }
        );
    }

    internal virtual void _sortByXIndex()
    {
        _currentChildVicinities.sort();
    }

    internal virtual void _reifyChildren()
    {
        DartRuntimePrimitives.Assert(() => _firstChild is null);
        DartRuntimePrimitives.Assert(() => _lastChild is null);
        RenderBox? previousChildLocal = default!;
        switch (mainAxis)
        {
            case Axis.vertical:
            {
                _sortByYIndex();
                break;
            }
            case Axis.horizontal:
            {
                _sortByXIndex();
                break;
            }
        }
        foreach (ChildVicinity vicinity in _currentChildVicinities)
        {
            previousChildLocal =
                _completeChildParentData(vicinity, previousChild: previousChildLocal)
                ?? previousChildLocal;
        }
        _lastChild = previousChildLocal;
        if (_lastChild is not null)
        {
            parentDataOf(_lastChild!)._nextSibling = null;
        }
        _currentChildVicinities.Clear();
    }

    internal virtual RenderBox? _completeChildParentData(
        ChildVicinity vicinity,
        RenderBox? previousChild = null
    )
    {
        DartRuntimePrimitives.Assert(() => !Equals(vicinity, ChildVicinity.invalid));
        if (_children.ContainsKey(vicinity))
        {
            RenderBox child = _children.GetValueOrDefault(vicinity)!;
            DartRuntimePrimitives.Assert(() => Equals(parentDataOf(child).vicinity, vicinity));
            updateChildPaintData(child);
            if (previousChild is null)
            {
                DartRuntimePrimitives.Assert(() => _firstChild is null);
                _firstChild = child;
            }
            else
            {
                parentDataOf(previousChild)._nextSibling = child;
                parentDataOf(child)._previousSibling = previousChild;
            }
            return child;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _debugCheckContentDimensions()
    {
        var hint =
            "Subclasses should call applyContentDimensions on the "
            + "verticalOffset and horizontalOffset to set the min and max scroll offset. "
            + "If the contents exceed one or both sides of the viewportDimension, "
            + "ensure the viewportDimension height or width is subtracted in that axis "
            + "for the correct extent.";
        DartRuntimePrimitives.Assert(() =>
        {
            if (!((ScrollPosition?)verticalOffset)!.hasContentDimensions)
            {
                throw DartRuntimePrimitives.AsException(
                    new FlutterError(
                        new List<DiagnosticsNode>
                        {
                            new ErrorSummary(
                                "The verticalOffset was not given content dimensions during "
                                    + "layoutChildSequence."
                            ),
                            new ErrorHint(hint),
                        }
                    )
                );
            }
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        DartRuntimePrimitives.Assert(() =>
        {
            if (!((ScrollPosition?)horizontalOffset)!.hasContentDimensions)
            {
                throw DartRuntimePrimitives.AsException(
                    new FlutterError(
                        new List<DiagnosticsNode>
                        {
                            new ErrorSummary(
                                "The horizontalOffset was not given content dimensions during "
                                    + "layoutChildSequence."
                            ),
                            new ErrorHint(hint),
                        }
                    )
                );
            }
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? buildOrObtainChildFor(ChildVicinity vicinity)
    {
        DartRuntimePrimitives.Assert(() => !Equals(vicinity, ChildVicinity.invalid));
        DartRuntimePrimitives.Assert(() => debugDoingThisLayout);
        if (
            _needsDelegateRebuild
            || (!_children.ContainsKey(vicinity) && !_keepAliveBucket.ContainsKey(vicinity))
        )
        {
            invokeLayoutCallback<BoxConstraints>(
                (_) =>
                {
                    _childManager._buildChild(vicinity);
                }
            );
        }
        else
        {
            _keepAliveBucket.remove(vicinity);
            _childManager._reuseChild(vicinity);
        }
        if (!_children.ContainsKey(vicinity))
        {
            return null;
        }
        DartRuntimePrimitives.Assert(() => _children.ContainsKey(vicinity));
        RenderBox child = _children.GetValueOrDefault(vicinity)!;
        _activeChildrenForLayoutPass[vicinity] = child;
        parentDataOf(child).vicinity = vicinity;
        _currentChildVicinities.Add(vicinity);
        return child;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void updateChildPaintData(RenderBox child)
    {
        TwoDimensionalViewportParentData childParentData = parentDataOf(child);
        DartRuntimePrimitives.Assert(
            () => childParentData.layoutOffset is not null,
            () =>
                (object?)$"The child with ChildVicinity(xIndex: {childParentData.vicinity.xIndex}, "
                + $"yIndex: {childParentData.vicinity.yIndex}) was not provided a "
                + "layoutOffset. This should be set during layoutChildSequence, "
                + "representing the position of the child."
        );
        DartRuntimePrimitives.Assert(() => child.hasSize);
        childParentData._paintExtent = computeChildPaintExtent(
            DartRuntimePrimitives.RequireValue(childParentData.layoutOffset),
            child.size
        );
        childParentData.paintOffset = computeAbsolutePaintOffsetFor(
            child,
            layoutOffset: DartRuntimePrimitives.RequireValue(childParentData.layoutOffset)
        );
        _hasVisualOverflow =
            _hasVisualOverflow
            || (!Equals(childParentData.layoutOffset, childParentData._paintExtent))
            || !childParentData.isVisible;
    }

    public virtual Size computeChildPaintExtent(Offset layoutOffset, Size childSize)
    {
        if (Equals(childSize, Size.zero) || (childSize.height == 0.0) || (childSize.width == 0.0))
        {
            return Size.zero;
        }
        double widthLocal = default!;
        if (layoutOffset.dx < 0.0)
        {
            if ((layoutOffset.dx + childSize.width) <= 0.0)
            {
                return Size.zero;
            }
            widthLocal = layoutOffset.dx + childSize.width;
        }
        else
        {
            if (layoutOffset.dx >= viewportDimension.width)
            {
                return Size.zero;
            }
            else
            {
                DartRuntimePrimitives.Assert(() =>
                    (layoutOffset.dx >= 0L) && (layoutOffset.dx < viewportDimension.width)
                );
                if ((layoutOffset.dx + childSize.width) > viewportDimension.width)
                {
                    widthLocal = viewportDimension.width - layoutOffset.dx;
                }
                else
                {
                    DartRuntimePrimitives.Assert(() =>
                        (layoutOffset.dx + childSize.width) <= viewportDimension.width
                    );
                    widthLocal = childSize.width;
                }
            }
        }
        double heightLocal = default!;
        if (layoutOffset.dy < 0.0)
        {
            if ((layoutOffset.dy + childSize.height) <= 0.0)
            {
                return Size.zero;
            }
            heightLocal = layoutOffset.dy + childSize.height;
        }
        else
        {
            if (layoutOffset.dy >= viewportDimension.height)
            {
                return Size.zero;
            }
            else
            {
                DartRuntimePrimitives.Assert(() =>
                    (layoutOffset.dy >= 0L) && (layoutOffset.dy < viewportDimension.height)
                );
                if ((layoutOffset.dy + childSize.height) > viewportDimension.height)
                {
                    heightLocal = viewportDimension.height - layoutOffset.dy;
                }
                else
                {
                    DartRuntimePrimitives.Assert(() =>
                        (layoutOffset.dy + childSize.height) <= viewportDimension.height
                    );
                    heightLocal = childSize.height;
                }
            }
        }
        return new Size(widthLocal, heightLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Offset computeAbsolutePaintOffsetFor(RenderBox child, Offset layoutOffset)
    {
        DartRuntimePrimitives.Assert(() => hasSize);
        DartRuntimePrimitives.Assert(() => child.hasSize);
        double xOffset = horizontalAxisDirection switch
        {
            AxisDirection.right => layoutOffset.dx,
            AxisDirection.left => viewportDimension.width - (layoutOffset.dx + child.size.width),
            AxisDirection.up => throw new Exception("This should not happen"),
            AxisDirection.down => throw new Exception("This should not happen"),
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        double yOffset = verticalAxisDirection switch
        {
            AxisDirection.up => viewportDimension.height - (layoutOffset.dy + child.size.height),
            AxisDirection.down => layoutOffset.dy,
            AxisDirection.right => throw new Exception("This should not happen"),
            AxisDirection.left => throw new Exception("This should not happen"),
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        return new Offset(xOffset, yOffset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (!Enumerable.Any(_children))
        {
            return;
        }
        if (_hasVisualOverflow && (!Equals(clipBehavior, Clip.none)))
        {
            _clipRectLayer.layer = context.pushClipRect(
                needsCompositing,
                offset,
                Offset.zero & viewportDimension,
                _paintChildren,
                clipBehavior: clipBehavior,
                oldLayer: _clipRectLayer.layer
            );
        }
        else
        {
            _clipRectLayer.layer = null;
            _paintChildren(context, offset);
        }
    }

    internal virtual void _paintChildren(PaintingContext context, Offset offset)
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            TwoDimensionalViewportParentData childParentData = parentDataOf(child);
            if (childParentData.isVisible)
            {
                context.paintChild(
                    child,
                    offset + DartRuntimePrimitives.RequireValue(childParentData.paintOffset)
                );
            }
            child = childParentData._nextSibling;
        }
    }

    internal virtual void _insertChild(RenderBox child, ChildVicinity slot)
    {
        DartRuntimePrimitives.Assert(() =>
            _debugTrackOrphans(newOrphan: _children.GetValueOrDefault(slot))
        );
        DartRuntimePrimitives.Assert(() => !_keepAliveBucket.containsValue(child));
        _children[slot] = child;
        adoptChild(child);
    }

    internal virtual void _moveChild(RenderBox child, ChildVicinity from, ChildVicinity to)
    {
        TwoDimensionalViewportParentData childParentData = parentDataOf(child);
        if (!childParentData.keptAlive)
        {
            if (Equals(_children.GetValueOrDefault(from), child))
            {
                _children.remove(from);
            }
            DartRuntimePrimitives.Assert(() =>
                _debugTrackOrphans(
                    newOrphan: _children.GetValueOrDefault(to),
                    noLongerOrphan: child
                )
            );
            _children[to] = child;
            return;
        }
        if (Equals(_keepAliveBucket.GetValueOrDefault(childParentData.vicinity), child))
        {
            _keepAliveBucket.remove(childParentData.vicinity);
        }
        DartRuntimePrimitives.Assert(() =>
        {
            _debugDanglingKeepAlives.Remove(child);
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        DartRuntimePrimitives.Assert(() =>
        {
            if (_keepAliveBucket.ContainsKey(childParentData.vicinity))
            {
                _debugDanglingKeepAlives.Add(
                    _keepAliveBucket.GetValueOrDefault(childParentData.vicinity)!
                );
            }
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        _keepAliveBucket[childParentData.vicinity] = child;
    }

    internal virtual void _removeChild(RenderBox child, ChildVicinity slot)
    {
        TwoDimensionalViewportParentData childParentData = parentDataOf(child);
        if (!childParentData.keptAlive)
        {
            if (Equals(_children.GetValueOrDefault(slot), child))
            {
                _children.remove(slot);
            }
            DartRuntimePrimitives.Assert(() => _debugTrackOrphans(noLongerOrphan: child));
            if (Equals(_keepAliveBucket.GetValueOrDefault(childParentData.vicinity), child))
            {
                _keepAliveBucket.remove(childParentData.vicinity);
            }
            DartRuntimePrimitives.Assert(() =>
                !Equals(_keepAliveBucket.GetValueOrDefault(childParentData.vicinity), child)
            );
            dropChild(child);
            return;
        }
        DartRuntimePrimitives.Assert(() =>
            Equals(_keepAliveBucket.GetValueOrDefault(childParentData.vicinity), child)
        );
        DartRuntimePrimitives.Assert(() =>
        {
            _debugDanglingKeepAlives.Remove(child);
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        _keepAliveBucket.remove(childParentData.vicinity);
        dropChild(child);
    }

    internal virtual bool _debugTrackOrphans(
        RenderBox? newOrphan = null,
        RenderBox? noLongerOrphan = null
    )
    {
        DartRuntimePrimitives.Assert(() =>
        {
            _debugOrphans ??= new List<RenderBox>();
            if (newOrphan is not null)
            {
                _debugOrphans!.Add(newOrphan);
            }
            if (noLongerOrphan is not null)
            {
                _debugOrphans!.Remove(noLongerOrphan);
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
            if (!debugCheckingIntrinsics)
            {
                throw DartRuntimePrimitives.AsException(
                    new FlutterError(
                        new List<DiagnosticsNode>
                        {
                            new ErrorSummary(
                                $"{GetType()} does not support returning intrinsic dimensions."
                            ),
                            new ErrorDescription(
                                "Calculating the intrinsic dimensions would require instantiating every child of "
                                    + "the viewport, which defeats the point of viewports being lazy."
                            ),
                        }
                    )
                );
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

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var __child = (RenderBox)child;
        Offset paintOffsetLocal = DartRuntimePrimitives.RequireValue(
            parentDataOf(__child).paintOffset
        );
        transform.translate(paintOffsetLocal.dx, paintOffsetLocal.dy);
    }

    public override void dispose()
    {
        _clipRectLayer.layer = null;
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
        System.Diagnostics.Debug.Assert(xIndex >= -1L);
        System.Diagnostics.Debug.Assert(yIndex >= -1L);
    }

    public override bool Equals(object? other)
    {
        var __other = other as ChildVicinity;
        if (__other is null)
        {
            return false;
        }

        return (__other is ChildVicinity)
            && (__other.xIndex == xIndex)
            && (__other.yIndex == yIndex);
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(xIndex, yIndex));

    public virtual long compareTo(ChildVicinity other)
    {
        if (xIndex == other.xIndex)
        {
            return yIndex - other.yIndex;
        }
        return xIndex - other.xIndex;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"(xIndex: {xIndex}, yIndex: {yIndex})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public int CompareTo(ChildVicinity? other) => checked((int)compareTo(other!));
}
