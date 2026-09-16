// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/picker.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class PickerLibrary
{
    internal static double _kDefaultDiameterRatio = 1.07;
}

public static partial class PickerLibrary
{
    internal static double _kDefaultPerspective = 0.003;
}

public static partial class PickerLibrary
{
    internal static double _kSqueeze = 1.45;
}

public static partial class PickerLibrary
{
    internal static double _kOverAndUnderCenterOpacity = 0.447;
}

public static partial class PickerLibrary
{
    internal static Duration _kCupertinoPickerTapToScrollDuration = Duration.Create(milliseconds: 300L);
}

public static partial class PickerLibrary
{
    internal static Curve _kCupertinoPickerTapToScrollCurve = Curves.easeInOut;
}

public class CupertinoPicker : StatefulWidget
{
    public virtual double diameterRatio { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual double offAxisFraction { get; private set; } = default!;
    public virtual bool useMagnifier { get; private set; } = default!;
    public virtual double magnification { get; private set; } = default!;
    public virtual FixedExtentScrollController? scrollController { get; private set; }
    public virtual double itemExtent { get; private set; } = default!;
    public virtual double squeeze { get; private set; } = default!;
    public virtual ChangeReportingBehavior changeReportingBehavior { get; private set; } = default!;
    public virtual System.Action<long>? onSelectedItemChanged { get; private set; }
    public virtual ListWheelChildDelegate childDelegate { get; private set; } = default!;
    public virtual Widget? selectionOverlay { get; private set; }

    public CupertinoPicker(Key? key = null, double? diameterRatio = null, Color? backgroundColor = null, double offAxisFraction = 0.0, bool useMagnifier = false, double magnification = 1.0, FixedExtentScrollController? scrollController = null, double? squeeze = null, ChangeReportingBehavior changeReportingBehavior = ChangeReportingBehavior.onScrollUpdate, double itemExtent = default!, System.Action<long>? onSelectedItemChanged = default!, List<Widget> children = default!, Widget? selectionOverlay = default!, bool looping = false) : base(key: key)
    {
        double __diameterRatio = diameterRatio ?? PickerLibrary._kDefaultDiameterRatio;
        double __squeeze = squeeze ?? PickerLibrary._kSqueeze;
        Widget? __selectionOverlay = selectionOverlay ?? new CupertinoPickerDefaultSelectionOverlay();
        this.diameterRatio = __diameterRatio;
        this.backgroundColor = backgroundColor;
        this.offAxisFraction = offAxisFraction;
        this.useMagnifier = useMagnifier;
        this.magnification = magnification;
        this.scrollController = scrollController;
        this.squeeze = __squeeze;
        this.changeReportingBehavior = changeReportingBehavior;
        this.itemExtent = itemExtent;
        this.onSelectedItemChanged = onSelectedItemChanged;
        this.selectionOverlay = __selectionOverlay;
        childDelegate = looping ? new global::Doroti.Framework.Widgets.ListWheelChildLoopingListDelegate(children: children) : new global::Doroti.Framework.Widgets.ListWheelChildListDelegate(children: children);
        System.Diagnostics.Debug.Assert(__diameterRatio > 0.0);
        System.Diagnostics.Debug.Assert(magnification > 0L);
        System.Diagnostics.Debug.Assert(itemExtent > 0L);
        System.Diagnostics.Debug.Assert(__squeeze > 0L);
    }

    public static CupertinoPicker CreateBuilder(Key? key = null, double? diameterRatio = null, Color? backgroundColor = null, double offAxisFraction = 0.0, bool useMagnifier = false, double magnification = 1.0, FixedExtentScrollController? scrollController = null, double? squeeze = null, ChangeReportingBehavior changeReportingBehavior = ChangeReportingBehavior.onScrollUpdate, double itemExtent = default!, System.Action<long>? onSelectedItemChanged = default!, Func<BuildContext, long, Widget?> itemBuilder = default!, long? childCount = null, Widget? selectionOverlay = default!)
    {
        var __instance = new CupertinoPicker(key: key, diameterRatio: diameterRatio, backgroundColor: backgroundColor, offAxisFraction: offAxisFraction, useMagnifier: useMagnifier, magnification: magnification, scrollController: scrollController, squeeze: squeeze, changeReportingBehavior: changeReportingBehavior, itemExtent: itemExtent, onSelectedItemChanged: onSelectedItemChanged, children: default!, selectionOverlay: selectionOverlay);
        double __diameterRatio = diameterRatio ?? PickerLibrary._kDefaultDiameterRatio;
        double __squeeze = squeeze ?? PickerLibrary._kSqueeze;
        Widget? __selectionOverlay = selectionOverlay ?? new CupertinoPickerDefaultSelectionOverlay();
        __instance.diameterRatio = __diameterRatio;
        __instance.backgroundColor = backgroundColor;
        __instance.offAxisFraction = offAxisFraction;
        __instance.useMagnifier = useMagnifier;
        __instance.magnification = magnification;
        __instance.scrollController = scrollController;
        __instance.squeeze = __squeeze;
        __instance.changeReportingBehavior = changeReportingBehavior;
        __instance.itemExtent = itemExtent;
        __instance.onSelectedItemChanged = onSelectedItemChanged;
        __instance.selectionOverlay = __selectionOverlay;
        __instance.childDelegate = new ListWheelChildBuilderDelegate(builder: itemBuilder, childCount: childCount);
        return __instance;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoPickerState__picker());
}

internal class _CupertinoPickerState__picker : State<CupertinoPicker>
{
    private bool __late__lastHapticIndex_initialized;
    private long __late__lastHapticIndex = default!;
    internal virtual long _lastHapticIndex
    {
        get
        {
            if (!__late__lastHapticIndex_initialized)
            {
                __late__lastHapticIndex = _effectiveController.initialItem;
                __late__lastHapticIndex_initialized = true;
            }
            return __late__lastHapticIndex;
        }
        set { __late__lastHapticIndex = value; __late__lastHapticIndex_initialized = true; }
    }
    internal virtual long? _lastMiddlePosition { get; set; } = default;
    internal virtual FixedExtentScrollController? _controller { get; set; } = default;
    internal virtual bool _enableHapticFeedback { get; set; } = true;

    internal virtual FixedExtentScrollController _effectiveController => DartRuntimePrimitives.ConvertValue<FixedExtentScrollController>(widget.scrollController ?? _controller!);
    public override void initState()
    {
        base.initState();
        if (widget.scrollController is null)
        {
            _controller = new FixedExtentScrollController();
        }
        _effectiveController.addListener(_handleScroll);
    }

    public override void didUpdateWidget(CupertinoPicker oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((widget.scrollController is not null) && (oldWidget.scrollController is null))
        {
            _controller?.dispose();
            _controller = null;
            widget.scrollController!.addListener(_handleScroll);
        }
        else
        {
            if ((widget.scrollController is null) && (oldWidget.scrollController is not null))
            {
                DartRuntimePrimitives.Assert(() => _controller is null);
                oldWidget.scrollController!.removeListener(_handleScroll);
                _controller = new FixedExtentScrollController();
                _controller!.addListener(_handleScroll);
            }
        }
    }

    public override void dispose()
    {
        _controller?.dispose();
        if (widget.scrollController is not null)
        {
            widget.scrollController!.removeListener(_handleScroll);
        }
        base.dispose();
    }

    internal virtual void _handleHapticFeedback(long index)
    {
        if (!_enableHapticFeedback)
        {
            return;
        }
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.iOS:
                {
                    if (index != _lastHapticIndex)
                    {
                        _lastHapticIndex = index;
                        DartRuntimePrimitives.Ignore(HapticFeedback.selectionClick());
                        DartRuntimePrimitives.Ignore(SystemSound.play(SystemSoundType.tick));
                    }
                    break;
                }
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.macOS:
            case TargetPlatform.windows:
                {
                    return;
                }
        }
    }

    internal virtual void _handleScroll()
    {
        long index = _effectiveController.selectedItem;
        double fractionalOffset = _effectiveController.offset / widget.itemExtent;
        long currentPosition = fractionalOffset.floor();
        double currentItemOffset = fractionalOffset - index;
        if ((currentPosition != _lastMiddlePosition) || (currentItemOffset.abs() <= 0.1))
        {
            _handleHapticFeedback(index);
        }
        _lastMiddlePosition = currentPosition;
    }

    internal async virtual Future _handleChildTap(long index)
    {
        _enableHapticFeedback = false;
        await _effectiveController.animateToItem(index, duration: PickerLibrary._kCupertinoPickerTapToScrollDuration, curve: PickerLibrary._kCupertinoPickerTapToScrollCurve);
        _enableHapticFeedback = true;
        _lastHapticIndex = _effectiveController.selectedItem;
    }

    internal virtual Widget _buildSelectionOverlay(Widget selectionOverlay)
    {
        double heightLocal = widget.itemExtent * widget.magnification;
        return new IgnorePointer(child: new Center(child: new ConstrainedBox(constraints: BoxConstraints.CreateExpand(height: heightLocal), child: selectionOverlay)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        TextStyle textStyle = CupertinoTheme.of(context).textTheme.pickerTextStyle;
        Color? resolvedBackgroundColor = CupertinoDynamicColor.maybeResolve(widget.backgroundColor, context);
        DartRuntimePrimitives.Assert(() => RenderListWheelViewport.defaultPerspective == PickerLibrary._kDefaultPerspective);
        Widget result = new DefaultTextStyle(style: textStyle.copyWith(color: CupertinoDynamicColor.maybeResolve(textStyle.color, context)), child: new Stack(children: ((Func<List<Widget>>)(() => { var __collection13325 = new List<Widget>(); __collection13325.Add(DartRuntimePrimitives.ConvertValue<Widget>(Positioned.CreateFill(child: new _CupertinoPickerSemantics__picker(scrollController: _effectiveController, child: ListWheelScrollView.CreateUseDelegate(controller: _effectiveController, physics: new FixedExtentScrollPhysics(), diameterRatio: widget.diameterRatio, offAxisFraction: widget.offAxisFraction, useMagnifier: widget.useMagnifier, magnification: widget.magnification, overAndUnderCenterOpacity: PickerLibrary._kOverAndUnderCenterOpacity, itemExtent: widget.itemExtent, squeeze: widget.squeeze, onSelectedItemChanged: widget.onSelectedItemChanged, dragStartBehavior: Gestures.DragStartBehavior.down, changeReportingBehavior: widget.changeReportingBehavior, childDelegate: new _CupertinoPickerListWheelChildDelegateWrapper__picker(widget.childDelegate, onTappedChild: (__arg0) => { _ = _handleChildTap(__arg0); })))))); if (widget.selectionOverlay is not null) { __collection13325.Add(DartRuntimePrimitives.ConvertValue<Widget>(_buildSelectionOverlay(widget.selectionOverlay!))); } return __collection13325; }))()));
        return new DecoratedBox(decoration: new BoxDecoration(color: resolvedBackgroundColor), child: result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CupertinoPickerDefaultSelectionOverlay : StatelessWidget
{
    public virtual bool capStartEdge { get; private set; } = default!;
    public virtual bool capEndEdge { get; private set; } = default!;
    public virtual Color background { get; private set; } = default!;
    internal const double _defaultSelectionOverlayHorizontalMargin = 9;
    internal const double _defaultSelectionOverlayRadius = 8;

    public CupertinoPickerDefaultSelectionOverlay(Key? key = null, Color background = default!, bool capStartEdge = true, bool capEndEdge = true) : base(key: key)
    {
        Color __background = background ?? CupertinoColors.tertiarySystemFill;
        this.background = __background;
        this.capStartEdge = capStartEdge;
        this.capEndEdge = capEndEdge;
    }

    public override Widget build(BuildContext context)
    {
        var radius = Radius.circular(_defaultSelectionOverlayRadius);
        return new Container(margin: EdgeInsetsDirectional.CreateOnly(start: capStartEdge ? _defaultSelectionOverlayHorizontalMargin : 0, end: capEndEdge ? _defaultSelectionOverlayHorizontalMargin : 0), decoration: new ShapeDecoration(shape: new RoundedSuperellipseBorder(borderRadius: BorderRadiusDirectional.CreateHorizontal(start: capStartEdge ? radius : Radius.zero, end: capEndEdge ? radius : Radius.zero)), color: CupertinoDynamicColor.resolve(background, context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CupertinoPickerSemantics__picker : SingleChildRenderObjectWidget
{
    public virtual FixedExtentScrollController scrollController { get; private set; } = default!;

    internal _CupertinoPickerSemantics__picker(Widget? child = null, FixedExtentScrollController scrollController = default!) : base(child: child)
    {
        this.scrollController = scrollController;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        return new _RenderCupertinoPickerSemantics__picker(scrollController, Directionality.of(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderCupertinoPickerSemantics__picker)renderObject;
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        DartRuntimePrimitives.Ignore(((Func<_RenderCupertinoPickerSemantics__picker>)(() =>
{
    var __cascade = __renderObject;
    __cascade.textDirection = Directionality.of(context);
    __cascade.controller = scrollController;
    return __cascade;
}))());
    }

}

public class _RenderCupertinoPickerSemantics__picker : RenderProxyBox
{
    internal virtual FixedExtentScrollController _controller { get; set; } = default!;
    internal virtual TextDirection _textDirection { get; set; } = default!;
    internal virtual long _currentIndex { get; set; } = 0L;

    internal _RenderCupertinoPickerSemantics__picker(FixedExtentScrollController controller, TextDirection _textDirection)
    {
        this._textDirection = _textDirection;
        _updateController(null, controller);
    }

    public virtual FixedExtentScrollController controller
    {
        get => _controller;
        set
        {
            var __value = value;
            _updateController(_controller, __value);
        }
    }
    internal virtual void _updateController(FixedExtentScrollController? oldValue, FixedExtentScrollController value)
    {
        if (Equals(value, oldValue))
        {
            return;
        }
        if (oldValue is not null)
        {
            oldValue.removeListener(_handleScrollUpdate);
        }
        else
        {
            _currentIndex = value.initialItem;
        }
        value.addListener(_handleScrollUpdate);
        _controller = value;
    }

    public virtual TextDirection textDirection
    {
        get => _textDirection;
        set
        {
            var __value = value;
            if (Equals(textDirection, __value))
            {
                return;
            }
            _textDirection = __value;
            markNeedsSemanticsUpdate();
        }
    }
    internal virtual void _handleIncrease()
    {
        controller.jumpToItem(_currentIndex + 1L);
    }

    internal virtual void _handleDecrease()
    {
        controller.jumpToItem(_currentIndex - 1L);
    }

    internal virtual void _handleScrollUpdate()
    {
        if (controller.selectedItem == _currentIndex)
        {
            return;
        }
        _currentIndex = controller.selectedItem;
        markNeedsSemanticsUpdate();
    }

    public override void describeSemanticsConfiguration(Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.isSemanticBoundary = true;
        config.textDirection = textDirection;
    }

    public override void assembleSemanticsNode(Semantics.SemanticsNode node, Semantics.SemanticsConfiguration config, IEnumerable<Semantics.SemanticsNode> children)
    {
        if (!Enumerable.Any(children))
        {
            base.assembleSemanticsNode(node, config, children.Cast<Semantics.SemanticsNode>());
            return;
        }
        Semantics.SemanticsNode scrollable = children.First();
        var indexedChildren = new DartMap<long, Semantics.SemanticsNode>();
        scrollable.visitChildren((child) =>
        {
            DartRuntimePrimitives.Assert(() => child.indexInParent is not null);
            indexedChildren[DartRuntimePrimitives.RequireValue(child.indexInParent)] = child;
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        if (!indexedChildren.ContainsKey(_currentIndex))
        {
            node.updateWith(config: config);
            return;
        }
        string currentLabel = indexedChildren.GetValueOrDefault(_currentIndex)!.label;
        if (currentLabel.Length == 0)
        {
            node.updateWith(config: config);
            return;
        }
        config.value = currentLabel;
        Semantics.SemanticsNode? previousChild = indexedChildren.GetValueOrDefault(_currentIndex - 1L);
        Semantics.SemanticsNode? nextChild = indexedChildren.GetValueOrDefault(_currentIndex + 1L);
        if ((nextChild is not null) && (nextChild.label.Length != 0))
        {
            config.increasedValue = nextChild.label;
            config.onIncrease = _handleIncrease;
        }
        if ((previousChild is not null) && (previousChild.label.Length != 0))
        {
            config.decreasedValue = previousChild.label;
            config.onDecrease = _handleDecrease;
        }
        node.updateWith(config: config);
    }

    public override void dispose()
    {
        base.dispose();
        controller.removeListener(_handleScrollUpdate);
    }

}

internal class _CupertinoPickerListWheelChildDelegateWrapper__picker : ListWheelChildDelegate
{
    internal virtual ListWheelChildDelegate _wrapped { get; private set; } = default!;
    public virtual System.Action<long> onTappedChild { get; private set; } = default!;

    internal _CupertinoPickerListWheelChildDelegateWrapper__picker(ListWheelChildDelegate _wrapped, System.Action<long> onTappedChild)
    {
        this._wrapped = _wrapped;
        this.onTappedChild = onTappedChild;
    }

    public override Widget? build(BuildContext context, long index)
    {
        Widget? childLocal = _wrapped.build(context, index);
        if (childLocal is null)
        {
            return childLocal;
        }
        return (Widget?)new GestureDetector(behavior: HitTestBehavior.translucent, excludeFromSemantics: true, onTap: () => { onTappedChild(index); }, child: childLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long? estimatedChildCount => _wrapped.estimatedChildCount;
    public override bool shouldRebuild(ListWheelChildDelegate oldDelegate) => _wrapped.shouldRebuild(((_CupertinoPickerListWheelChildDelegateWrapper__picker)oldDelegate)._wrapped);
    public override long trueIndexOf(long index) => _wrapped.trueIndexOf(index);
}
