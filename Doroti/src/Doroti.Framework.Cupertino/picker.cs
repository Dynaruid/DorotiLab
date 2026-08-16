// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/picker.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

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
    internal static global::Doroti.Framework.Animation.Curve _kCupertinoPickerTapToScrollCurve = ((global::Doroti.Framework.Animation.Curve)(object?)global::Doroti.Framework.Animation.Curves.easeInOut);
}

public class CupertinoPicker : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual double diameterRatio { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual double offAxisFraction { get; private set; } = default!;
    public virtual bool useMagnifier { get; private set; } = default!;
    public virtual double magnification { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.FixedExtentScrollController? scrollController { get; private set; }
    public virtual double itemExtent { get; private set; } = default!;
    public virtual double squeeze { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.ChangeReportingBehavior changeReportingBehavior { get; private set; } = default!;
    public virtual global::System.Action<long>? onSelectedItemChanged { get; private set; }
    public virtual global::Doroti.Framework.Widgets.ListWheelChildDelegate childDelegate { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? selectionOverlay { get; private set; }

    public CupertinoPicker(global::Doroti.Framework.Foundation.Key? key = null, double? diameterRatio = null, Color? backgroundColor = null, double offAxisFraction = 0.0, bool useMagnifier = false, double magnification = 1.0, global::Doroti.Framework.Widgets.FixedExtentScrollController? scrollController = null, double? squeeze = null, global::Doroti.Framework.Widgets.ChangeReportingBehavior changeReportingBehavior = global::Doroti.Framework.Widgets.ChangeReportingBehavior.onScrollUpdate, double itemExtent = default!, global::System.Action<long>? onSelectedItemChanged = default!, List<global::Doroti.Framework.Widgets.Widget> children = default!, global::Doroti.Framework.Widgets.Widget? selectionOverlay = default!, bool looping = false) : base(key: key)
    {
        double __diameterRatio = diameterRatio ?? PickerLibrary._kDefaultDiameterRatio;
        double __squeeze = squeeze ?? PickerLibrary._kSqueeze;
        global::Doroti.Framework.Widgets.Widget? __selectionOverlay = selectionOverlay ?? new CupertinoPickerDefaultSelectionOverlay();
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
        this.childDelegate = (looping ? new global::Doroti.Framework.Widgets.ListWheelChildLoopingListDelegate(children: children) : new global::Doroti.Framework.Widgets.ListWheelChildListDelegate(children: children));
        System.Diagnostics.Debug.Assert((__diameterRatio > 0.0));
        System.Diagnostics.Debug.Assert((magnification > 0L));
        System.Diagnostics.Debug.Assert((itemExtent > 0L));
        System.Diagnostics.Debug.Assert((__squeeze > 0L));
    }

    public static CupertinoPicker CreateBuilder(global::Doroti.Framework.Foundation.Key? key = null, double? diameterRatio = null, Color? backgroundColor = null, double offAxisFraction = 0.0, bool useMagnifier = false, double magnification = 1.0, global::Doroti.Framework.Widgets.FixedExtentScrollController? scrollController = null, double? squeeze = null, global::Doroti.Framework.Widgets.ChangeReportingBehavior changeReportingBehavior = global::Doroti.Framework.Widgets.ChangeReportingBehavior.onScrollUpdate, double itemExtent = default!, global::System.Action<long>? onSelectedItemChanged = default!, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget?> itemBuilder = default!, long? childCount = null, global::Doroti.Framework.Widgets.Widget? selectionOverlay = default!)
    {
        var __instance = new CupertinoPicker(key: key, diameterRatio: diameterRatio, backgroundColor: backgroundColor, offAxisFraction: offAxisFraction, useMagnifier: useMagnifier, magnification: magnification, scrollController: scrollController, squeeze: squeeze, changeReportingBehavior: changeReportingBehavior, itemExtent: itemExtent, onSelectedItemChanged: onSelectedItemChanged, children: default!, selectionOverlay: selectionOverlay);
        double __diameterRatio = diameterRatio ?? PickerLibrary._kDefaultDiameterRatio;
        double __squeeze = squeeze ?? PickerLibrary._kSqueeze;
        global::Doroti.Framework.Widgets.Widget? __selectionOverlay = selectionOverlay ?? new CupertinoPickerDefaultSelectionOverlay();
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
        __instance.childDelegate = new global::Doroti.Framework.Widgets.ListWheelChildBuilderDelegate(builder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget?>)itemBuilder, childCount: childCount);
        return __instance;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoPickerState__picker());
}

internal class _CupertinoPickerState__picker : global::Doroti.Framework.Widgets.State<CupertinoPicker>
{
    private bool __late__lastHapticIndex_initialized;
    private long __late__lastHapticIndex = default!;
    internal virtual long _lastHapticIndex
    {
        get
        {
            if (!__late__lastHapticIndex_initialized)
            {
                __late__lastHapticIndex = ((global::Doroti.Framework.Widgets.FixedExtentScrollController)this._effectiveController).initialItem;
                __late__lastHapticIndex_initialized = true;
            }
            return __late__lastHapticIndex;
        }
        set { __late__lastHapticIndex = value; __late__lastHapticIndex_initialized = true; }
    }
    internal virtual long? _lastMiddlePosition { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.FixedExtentScrollController? _controller { get; set; } = default;
    internal virtual bool _enableHapticFeedback { get; set; } = true;

    internal virtual global::Doroti.Framework.Widgets.FixedExtentScrollController _effectiveController => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.FixedExtentScrollController>((((CupertinoPicker)this.widget).scrollController ?? this._controller!));
    public override void initState()
    {
        base.initState();
        if ((((CupertinoPicker)this.widget).scrollController is null))
        {
            _controller = new global::Doroti.Framework.Widgets.FixedExtentScrollController();
        }
        this._effectiveController.addListener(() => this._handleScroll());
    }

    public override void didUpdateWidget(CupertinoPicker oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (((((CupertinoPicker)this.widget).scrollController is not null) && (((CupertinoPicker)oldWidget).scrollController is null)))
        {
            this._controller?.dispose();
            _controller = null;
            ((CupertinoPicker)this.widget).scrollController!.addListener(() => this._handleScroll());
        }
        else
        {
            if (((((CupertinoPicker)this.widget).scrollController is null) && (((CupertinoPicker)oldWidget).scrollController is not null)))
            {
                DartRuntimePrimitives.Assert(() => (this._controller is null));
                ((CupertinoPicker)oldWidget).scrollController!.removeListener(() => this._handleScroll());
                _controller = new global::Doroti.Framework.Widgets.FixedExtentScrollController();
                this._controller!.addListener(() => this._handleScroll());
            }
        }
    }

    public override void dispose()
    {
        this._controller?.dispose();
        if ((((CupertinoPicker)this.widget).scrollController is not null))
        {
            ((CupertinoPicker)this.widget).scrollController!.removeListener(() => this._handleScroll());
        }
        base.dispose();
    }

    internal virtual void _handleHapticFeedback(long index)
    {
        if (!this._enableHapticFeedback)
        {
            return;
        }
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                {
                    if ((index != this._lastHapticIndex))
                    {
                        _lastHapticIndex = index;
                        DartRuntimePrimitives.Ignore(HapticFeedback.selectionClick());
                        DartRuntimePrimitives.Ignore(SystemSound.play(global::Doroti.Framework.Services.SystemSoundType.tick));
                    }
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    return;
                }
        }
    }

    internal virtual void _handleScroll()
    {
        long index__11064 = ((global::Doroti.Framework.Widgets.FixedExtentScrollController)this._effectiveController).selectedItem;
        double fractionalOffset__11231 = (this._effectiveController.offset / ((CupertinoPicker)this.widget).itemExtent);
        long currentPosition__11313 = fractionalOffset__11231.floor();
        double currentItemOffset__11375 = (fractionalOffset__11231 - index__11064);
        if (((currentPosition__11313 != this._lastMiddlePosition) || (currentItemOffset__11375.abs() <= 0.1)))
        {
            _handleHapticFeedback(index__11064);
        }
        _lastMiddlePosition = currentPosition__11313;
    }

    internal async virtual Future _handleChildTap(long index)
    {
        _enableHapticFeedback = false;
        await this._effectiveController.animateToItem(index, duration: PickerLibrary._kCupertinoPickerTapToScrollDuration, curve: PickerLibrary._kCupertinoPickerTapToScrollCurve);
        _enableHapticFeedback = true;
        _lastHapticIndex = ((global::Doroti.Framework.Widgets.FixedExtentScrollController)this._effectiveController).selectedItem;
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildSelectionOverlay(global::Doroti.Framework.Widgets.Widget selectionOverlay)
    {
        double height__12516 = (((CupertinoPicker)this.widget).itemExtent * ((CupertinoPicker)this.widget).magnification);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.IgnorePointer(child: new global::Doroti.Framework.Widgets.Center(child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: global::Doroti.Framework.Rendering.BoxConstraints.CreateExpand(height: height__12516), child: selectionOverlay))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Painting.TextStyle textStyle__12846 = CupertinoTheme.of(context).textTheme.pickerTextStyle;
        global::Doroti.Ui.Color? resolvedBackgroundColor__12929 = ((global::Doroti.Ui.Color?)(object?)CupertinoDynamicColor.maybeResolve(((CupertinoPicker)this.widget).backgroundColor, context));
        DartRuntimePrimitives.Assert(() => (global::Doroti.Framework.Rendering.RenderListWheelViewport.defaultPerspective == PickerLibrary._kDefaultPerspective));
        global::Doroti.Framework.Widgets.Widget result__13141 = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.DefaultTextStyle(style: textStyle__12846.copyWith(color: CupertinoDynamicColor.maybeResolve(((global::Doroti.Framework.Painting.TextStyle)textStyle__12846).color, context)), child: new global::Doroti.Framework.Widgets.Stack(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection13325 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection13325.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(global::Doroti.Framework.Widgets.Positioned.CreateFill(child: new _CupertinoPickerSemantics__picker(scrollController: this._effectiveController, child: global::Doroti.Framework.Widgets.ListWheelScrollView.CreateUseDelegate(controller: this._effectiveController, physics: new global::Doroti.Framework.Widgets.FixedExtentScrollPhysics(), diameterRatio: ((CupertinoPicker)this.widget).diameterRatio, offAxisFraction: ((CupertinoPicker)this.widget).offAxisFraction, useMagnifier: ((CupertinoPicker)this.widget).useMagnifier, magnification: ((CupertinoPicker)this.widget).magnification, overAndUnderCenterOpacity: PickerLibrary._kOverAndUnderCenterOpacity, itemExtent: ((CupertinoPicker)this.widget).itemExtent, squeeze: ((CupertinoPicker)this.widget).squeeze, onSelectedItemChanged: (global::System.Action<long>?)((CupertinoPicker)this.widget).onSelectedItemChanged, dragStartBehavior: global::Doroti.Framework.Gestures.DragStartBehavior.down, changeReportingBehavior: ((CupertinoPicker)this.widget).changeReportingBehavior, childDelegate: new _CupertinoPickerListWheelChildDelegateWrapper__picker(((CupertinoPicker)this.widget).childDelegate, onTappedChild: (__arg0) => { _ = this._handleChildTap(__arg0); })))))); if ((((CupertinoPicker)this.widget).selectionOverlay is not null)) { __collection13325.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildSelectionOverlay(((CupertinoPicker)this.widget).selectionOverlay!))); } return __collection13325; }))())));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.DecoratedBox(decoration: new global::Doroti.Framework.Painting.BoxDecoration(color: resolvedBackgroundColor__12929), child: result__13141));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CupertinoPickerDefaultSelectionOverlay : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual bool capStartEdge { get; private set; } = default!;
    public virtual bool capEndEdge { get; private set; } = default!;
    public virtual Color background { get; private set; } = default!;
    internal const double _defaultSelectionOverlayHorizontalMargin = 9;
    internal const double _defaultSelectionOverlayRadius = 8;

    public CupertinoPickerDefaultSelectionOverlay(global::Doroti.Framework.Foundation.Key? key = null, Color background = default!, bool capStartEdge = true, bool capEndEdge = true) : base(key: key)
    {
        Color __background = background ?? CupertinoColors.tertiarySystemFill;
        this.background = __background;
        this.capStartEdge = capStartEdge;
        this.capEndEdge = capEndEdge;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        var radius__17194 = global::Doroti.Ui.Radius.circular(_defaultSelectionOverlayRadius);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Container(margin: global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: (this.capStartEdge ? _defaultSelectionOverlayHorizontalMargin : 0), end: (this.capEndEdge ? _defaultSelectionOverlayHorizontalMargin : 0)), decoration: new global::Doroti.Framework.Painting.ShapeDecoration(shape: new global::Doroti.Framework.Painting.RoundedSuperellipseBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadiusDirectional.CreateHorizontal(start: (this.capStartEdge ? radius__17194 : Radius.zero), end: (this.capEndEdge ? radius__17194 : Radius.zero))), color: CupertinoDynamicColor.resolve(this.background, context))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CupertinoPickerSemantics__picker : global::Doroti.Framework.Widgets.SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Framework.Widgets.FixedExtentScrollController scrollController { get; private set; } = default!;

    internal _CupertinoPickerSemantics__picker(global::Doroti.Framework.Widgets.Widget? child = null, global::Doroti.Framework.Widgets.FixedExtentScrollController scrollController = default!) : base(child: child)
    {
        this.scrollController = scrollController;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderCupertinoPickerSemantics__picker(this.scrollController, Directionality.of(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderCupertinoPickerSemantics__picker)(object)renderObject;
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        DartRuntimePrimitives.Ignore(((Func<_RenderCupertinoPickerSemantics__picker>)(() =>
{            var __cascade = __renderObject;
            __cascade.textDirection = Directionality.of(context);
            __cascade.controller = this.scrollController;
            return __cascade;        }))());
    }

}

public class _RenderCupertinoPickerSemantics__picker : global::Doroti.Framework.Rendering.RenderProxyBox
{
    internal virtual global::Doroti.Framework.Widgets.FixedExtentScrollController _controller { get; set; } = default!;
    internal virtual TextDirection _textDirection { get; set; } = default!;
    internal virtual long _currentIndex { get; set; } = 0L;

    internal _RenderCupertinoPickerSemantics__picker(global::Doroti.Framework.Widgets.FixedExtentScrollController controller, TextDirection _textDirection)
    {
        this._textDirection = _textDirection;
        _updateController(null, controller);
    }

    public virtual global::Doroti.Framework.Widgets.FixedExtentScrollController controller
    {
        get => this._controller;
        set
        {
            var __value = value;
            _updateController(this._controller, __value);
        }
    }
    internal virtual void _updateController(global::Doroti.Framework.Widgets.FixedExtentScrollController? oldValue, global::Doroti.Framework.Widgets.FixedExtentScrollController value)
    {
        if ((object.Equals(value, oldValue)))
        {
            return;
        }
        if ((oldValue is not null))
        {
            oldValue.removeListener(() => this._handleScrollUpdate());
        }
        else
        {
            _currentIndex = ((global::Doroti.Framework.Widgets.FixedExtentScrollController)value).initialItem;
        }
        value.addListener(() => this._handleScrollUpdate());
        _controller = value;
    }

    public virtual global::Doroti.Ui.TextDirection textDirection
    {
        get => this._textDirection;
        set
        {
            var __value = value;
            if ((object.Equals(this.textDirection, __value)))
            {
                return;
            }
            _textDirection = __value;
            markNeedsSemanticsUpdate();
        }
    }
    internal virtual void _handleIncrease()
    {
        this.controller.jumpToItem((this._currentIndex + 1L));
    }

    internal virtual void _handleDecrease()
    {
        this.controller.jumpToItem((this._currentIndex - 1L));
    }

    internal virtual void _handleScrollUpdate()
    {
        if ((((global::Doroti.Framework.Widgets.FixedExtentScrollController)this.controller).selectedItem == this._currentIndex))
        {
            return;
        }
        _currentIndex = ((global::Doroti.Framework.Widgets.FixedExtentScrollController)this.controller).selectedItem;
        markNeedsSemanticsUpdate();
    }

    public override void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.isSemanticBoundary = true;
        ((dynamic)config).textDirection = this.textDirection;
    }

    public override void assembleSemanticsNode(global::Doroti.Framework.Semantics.SemanticsNode node, global::Doroti.Framework.Semantics.SemanticsConfiguration config, IEnumerable<global::Doroti.Framework.Semantics.SemanticsNode> children)
    {
        if (!System.Linq.Enumerable.Any(children))
        {
            base.assembleSemanticsNode(node, config, children.Cast<global::Doroti.Framework.Semantics.SemanticsNode>());
            return;
        }
        global::Doroti.Framework.Semantics.SemanticsNode scrollable__20946 = children.First();
        var indexedChildren__20985 = new DartMap<long, global::Doroti.Framework.Semantics.SemanticsNode>();
        scrollable__20946.visitChildren(((global::System.Func<global::Doroti.Framework.Semantics.SemanticsNode, bool>)((child) => {
DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Semantics.SemanticsNode)child).indexInParent is not null));
indexedChildren__20985[DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsNode)child).indexInParent)] = child;
return true;
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        if ((!indexedChildren__20985.ContainsKey(this._currentIndex)))
        {
            node.updateWith(config: config);
            return;
        }
        string currentLabel__21322 = indexedChildren__20985.GetValueOrDefault(this._currentIndex)!.label;
        if ((currentLabel__21322.Length == 0))
        {
            node.updateWith(config: config);
            return;
        }
        config.value = currentLabel__21322;
        global::Doroti.Framework.Semantics.SemanticsNode? previousChild__21858 = indexedChildren__20985.GetValueOrDefault((this._currentIndex - 1L));
        global::Doroti.Framework.Semantics.SemanticsNode? nextChild__21935 = indexedChildren__20985.GetValueOrDefault((this._currentIndex + 1L));
        if (((nextChild__21935 is not null) && (((global::Doroti.Framework.Semantics.SemanticsNode)nextChild__21935).label.Length != 0)))
        {
            config.increasedValue = ((global::Doroti.Framework.Semantics.SemanticsNode)nextChild__21935).label;
            config.onIncrease = (global::System.Action)this._handleIncrease;
        }
        if (((previousChild__21858 is not null) && (((global::Doroti.Framework.Semantics.SemanticsNode)previousChild__21858).label.Length != 0)))
        {
            config.decreasedValue = ((global::Doroti.Framework.Semantics.SemanticsNode)previousChild__21858).label;
            config.onDecrease = (global::System.Action)this._handleDecrease;
        }
        node.updateWith(config: config);
    }

    public override void dispose()
    {
        base.dispose();
        this.controller.removeListener(() => this._handleScrollUpdate());
    }

}

internal class _CupertinoPickerListWheelChildDelegateWrapper__picker : global::Doroti.Framework.Widgets.ListWheelChildDelegate
{
    internal virtual global::Doroti.Framework.Widgets.ListWheelChildDelegate _wrapped { get; private set; } = default!;
    public virtual global::System.Action<long> onTappedChild { get; private set; } = default!;

    internal _CupertinoPickerListWheelChildDelegateWrapper__picker(global::Doroti.Framework.Widgets.ListWheelChildDelegate _wrapped, global::System.Action<long> onTappedChild)
    {
        this._wrapped = _wrapped;
        this.onTappedChild = onTappedChild;
    }

    public override global::Doroti.Framework.Widgets.Widget? build(global::Doroti.Framework.Widgets.BuildContext context, long index)
    {
        global::Doroti.Framework.Widgets.Widget? child__23026 = ((global::Doroti.Framework.Widgets.Widget?)(object?)this._wrapped.build(context, index));
        if ((child__23026 is null))
        {
            return child__23026;
        }
        return ((global::Doroti.Framework.Widgets.Widget?)(object?)new global::Doroti.Framework.Widgets.GestureDetector(behavior: global::Doroti.Framework.Rendering.HitTestBehavior.translucent, excludeFromSemantics: true, onTap: ((global::System.Action)(() => { this.onTappedChild(index); })), child: child__23026));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long? estimatedChildCount => ((global::Doroti.Framework.Widgets.ListWheelChildDelegate)this._wrapped).estimatedChildCount;
    public override bool shouldRebuild(global::Doroti.Framework.Widgets.ListWheelChildDelegate oldDelegate) => this._wrapped.shouldRebuild(((_CupertinoPickerListWheelChildDelegateWrapper__picker)oldDelegate)._wrapped);
    public override long trueIndexOf(long index) => this._wrapped.trueIndexOf(index);
}
