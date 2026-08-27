// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/segmented_control.dart
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

public static partial class Segmented_controlLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsetsGeometry _kHorizontalItemPadding = ((global::Doroti.Framework.Painting.EdgeInsetsGeometry)(object?)global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 16.0));
}

public static partial class Segmented_controlLibrary
{
    internal static double _kMinSegmentedControlHeight = 28.0;
}

public static partial class Segmented_controlLibrary
{
    internal static Color _kDisableTextColor = global::Doroti.Ui.Color.fromARGB(115L, 122L, 122L, 122L);
}

public static partial class Segmented_controlLibrary
{
    internal static Duration _kFadeDuration = Duration.Create(milliseconds: 165L);
}

public class CupertinoSegmentedControl<T> : global::Doroti.Framework.Widgets.StatefulWidget where T : notnull
{
    public virtual DartMap<T, global::Doroti.Framework.Widgets.Widget> children { get; private set; } = default!;
    public virtual T? groupValue { get; private set; }
    public virtual global::System.Action<T> onValueChanged { get; private set; } = default!;
    public virtual Color? unselectedColor { get; private set; }
    public virtual Color? selectedColor { get; private set; }
    public virtual Color? borderColor { get; private set; }
    public virtual Color? pressedColor { get; private set; }
    public virtual Color? disabledColor { get; private set; }
    public virtual Color? disabledTextColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual HashSet<T> disabledChildren { get; private set; } = default!;

    public CupertinoSegmentedControl(global::Doroti.Framework.Foundation.Key? key = null, DartMap<T, global::Doroti.Framework.Widgets.Widget> children = default!, global::System.Action<T> onValueChanged = default!, T? groupValue = default, Color? unselectedColor = null, Color? selectedColor = null, Color? borderColor = null, Color? pressedColor = null, Color? disabledColor = null, Color? disabledTextColor = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, HashSet<T> disabledChildren = default!) : base(key: key)
    {
        HashSet<T> __disabledChildren = disabledChildren ?? new HashSet<T>();
        this.children = children;
        this.onValueChanged = onValueChanged;
        this.groupValue = groupValue;
        this.unselectedColor = unselectedColor;
        this.selectedColor = selectedColor;
        this.borderColor = borderColor;
        this.pressedColor = pressedColor;
        this.disabledColor = disabledColor;
        this.disabledTextColor = disabledTextColor;
        this.padding = padding;
        this.disabledChildren = __disabledChildren;
        System.Diagnostics.Debug.Assert((checked((long)(children.Count)) >= 2L));
        System.Diagnostics.Debug.Assert(((groupValue is null) || children.Keys.any(((child) => EqualityComparer<T>.Default.Equals(child, groupValue)))));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SegmentedControlState__segmented_control<T>());
}

public class _SegmentButton__segmented_control<T> : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual T value { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual bool enabled { get; private set; } = default!;

    internal _SegmentButton__segmented_control(global::Doroti.Framework.Foundation.Key? key = null, T value = default!, global::Doroti.Framework.Widgets.Widget child = default!, bool enabled = default!) : base(key: key)
    {
        this.value = value;
        this.child = child;
        this.enabled = enabled;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SegmentButtonState__segmented_control<T>());
}

internal class _SegmentButtonState__segmented_control<T> : global::Doroti.Framework.Widgets.State<_SegmentButton__segmented_control<T>>, global::Doroti.Framework.Widgets.RadioClient<T>
{
    internal virtual global::Doroti.Framework.Widgets.FocusNode _focusNode { get; private set; } = default!;
    public virtual RadioGroupRegistry<T>? _registry { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _focusNode = new global::Doroti.Framework.Widgets.FocusNode(debugLabel: $"CupertinoSegmentedControl<{typeof(T)}>[{((_SegmentButton__segmented_control<T>)(object)this.widget).value}]");
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        registry = (((_SegmentButton__segmented_control<T>)(object)this.widget).enabled ? RadioGroup.maybeOf<T>(this.context) : null);
    }

    public override void didUpdateWidget(_SegmentButton__segmented_control<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((((_SegmentButton__segmented_control<T>)oldWidget).enabled != ((_SegmentButton__segmented_control<T>)(object)this.widget).enabled))
        {
            registry = (((_SegmentButton__segmented_control<T>)(object)this.widget).enabled ? RadioGroup.maybeOf<T>(this.context) : null);
        }
    }

    public override void dispose()
    {
        registry = null;
        this._focusNode.dispose();
        base.dispose();
    }

    public virtual T radioValue => ((_SegmentButton__segmented_control<T>)(object)this.widget).value;
    public virtual global::Doroti.Framework.Widgets.FocusNode focusNode => this._focusNode;
    public virtual bool tristate => false;
    public virtual bool enabled => ((_SegmentButton__segmented_control<T>)(object)this.widget).enabled;
    public virtual void requestFocus()
    {
        if (((_SegmentButton__segmented_control<T>)(object)this.widget).enabled)
        {
            this._focusNode.requestFocus();
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Focus(focusNode: this._focusNode, canRequestFocus: ((_SegmentButton__segmented_control<T>)(object)this.widget).enabled, onKeyEvent: ((global::System.Func<global::Doroti.Framework.Widgets.FocusNode, global::Doroti.Framework.Services.KeyEvent, global::Doroti.Framework.Widgets.KeyEventResult>?)((node, @event) => global::Doroti.Framework.Widgets.KeyEventResult.ignored)), child: ((_SegmentButton__segmented_control<T>)(object)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RadioGroupRegistry<T>? registry
    {
        get => this._registry;
        set
        {
            var newRegistry = value;
            if ((!object.Equals(this._registry, newRegistry)))
            {
                this._registry?.unregisterClient(this);
            }
            this._registry = newRegistry;
            this._registry?.registerClient(this);
        }
    }
}

public class _SegmentedControlState__segmented_control<T> : global::Doroti.Framework.Widgets.State<CupertinoSegmentedControl<T>>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<CupertinoSegmentedControl<T>> where T : notnull
{
    internal virtual T? _pressedKey { get; set; } = default;
    internal virtual List<global::Doroti.Framework.Animation.AnimationController> _selectionControllers { get; private set; } = new List<global::Doroti.Framework.Animation.AnimationController>();
    internal virtual List<global::Doroti.Framework.Animation.ColorTween> _childTweens { get; private set; } = new List<global::Doroti.Framework.Animation.ColorTween>();
    internal virtual DartMap<T, global::Doroti.Framework.Widgets.GlobalKey<_SegmentButtonState__segmented_control<T>>> _segmentKeys { get; private set; } = new DartMap<T, global::Doroti.Framework.Widgets.GlobalKey<_SegmentButtonState__segmented_control<T>>>();
    internal virtual global::Doroti.Framework.Animation.ColorTween _forwardBackgroundColorTween { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.ColorTween _reverseBackgroundColorTween { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.ColorTween _textColorTween { get; set; } = default!;
    internal virtual Color? _selectedColor { get; set; } = default;
    internal virtual Color? _unselectedColor { get; set; } = default;
    internal virtual Color? _borderColor { get; set; } = default;
    internal virtual Color? _pressedColor { get; set; } = default;
    internal virtual Color? _selectedDisabledColor { get; set; } = default;
    internal virtual Color? _unselectedDisabledColor { get; set; } = default;
    internal virtual Color? _disabledTextColor { get; set; } = default;
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public virtual global::Doroti.Framework.Animation.AnimationController createAnimationController()
    {
        return ((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{
    var __cascade = new global::Doroti.Framework.Animation.AnimationController(duration: Segmented_controlLibrary._kFadeDuration, vsync: this);
    __cascade.addListener(((global::System.Action)(() =>
    {
        setState(((global::System.Action)(() =>
        {
        })));
    })));
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _updateColors()
    {
        DartRuntimePrimitives.Assert(() => this.mounted, () => (object?)"This should only be called after didUpdateDependencies");
        var changed = false;
        global::Doroti.Ui.Color disabledTextColorLocal = ((global::Doroti.Ui.Color)(object?)(((CupertinoSegmentedControl<T>)(object)this.widget).disabledTextColor ?? Segmented_controlLibrary._kDisableTextColor));
        if ((!object.Equals(this._disabledTextColor, disabledTextColorLocal)))
        {
            changed = true;
            _disabledTextColor = disabledTextColorLocal;
        }
        global::Doroti.Ui.Color selectedColorLocal = ((global::Doroti.Ui.Color)(object?)(((CupertinoSegmentedControl<T>)(object)this.widget).selectedColor ?? CupertinoTheme.of(this.context).primaryColor));
        if ((!object.Equals(this._selectedColor, selectedColorLocal)))
        {
            changed = true;
            _selectedColor = selectedColorLocal;
        }
        global::Doroti.Ui.Color unselectedColorLocal = ((global::Doroti.Ui.Color)(object?)(((CupertinoSegmentedControl<T>)(object)this.widget).unselectedColor ?? CupertinoTheme.of(this.context).primaryContrastingColor));
        if ((!object.Equals(this._unselectedColor, unselectedColorLocal)))
        {
            changed = true;
            _unselectedColor = unselectedColorLocal;
        }
        global::Doroti.Ui.Color selectedDisabledColor = ((global::Doroti.Ui.Color)(object?)(((CupertinoSegmentedControl<T>)(object)this.widget).disabledColor ?? selectedColorLocal.withOpacity(0.5)));
        global::Doroti.Ui.Color unselectedDisabledColor = ((global::Doroti.Ui.Color)(object?)(((CupertinoSegmentedControl<T>)(object)this.widget).disabledColor ?? unselectedColorLocal));
        if (((!object.Equals(this._selectedDisabledColor, selectedDisabledColor)) || (!object.Equals(this._unselectedDisabledColor, unselectedDisabledColor))))
        {
            changed = true;
            _selectedDisabledColor = selectedDisabledColor;
            _unselectedDisabledColor = unselectedDisabledColor;
        }
        global::Doroti.Ui.Color borderColorLocal = ((global::Doroti.Ui.Color)(object?)(((CupertinoSegmentedControl<T>)(object)this.widget).borderColor ?? CupertinoTheme.of(this.context).primaryColor));
        if ((!object.Equals(this._borderColor, borderColorLocal)))
        {
            changed = true;
            _borderColor = borderColorLocal;
        }
        global::Doroti.Ui.Color pressedColorLocal = ((global::Doroti.Ui.Color)(object?)(((CupertinoSegmentedControl<T>)(object)this.widget).pressedColor ?? CupertinoTheme.of(this.context).primaryColor.withOpacity(0.2)));
        if ((!object.Equals(this._pressedColor, pressedColorLocal)))
        {
            changed = true;
            _pressedColor = pressedColorLocal;
        }
        _forwardBackgroundColorTween = new global::Doroti.Framework.Animation.ColorTween(begin: this._pressedColor, end: this._selectedColor);
        _reverseBackgroundColorTween = new global::Doroti.Framework.Animation.ColorTween(begin: this._unselectedColor, end: this._selectedColor);
        _textColorTween = new global::Doroti.Framework.Animation.ColorTween(begin: this._selectedColor, end: this._unselectedColor);
        return changed;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _updateAnimationControllers()
    {
        DartRuntimePrimitives.Assert(() => this.mounted, () => (object?)"This should only be called after didUpdateDependencies");
        foreach (global::Doroti.Framework.Animation.AnimationController controller in this._selectionControllers)
        {
            controller.dispose();
        }
        this._selectionControllers.Clear();
        this._childTweens.Clear();
        foreach (T key in ((CupertinoSegmentedControl<T>)(object)this.widget).children.Keys)
        {
            global::Doroti.Framework.Animation.AnimationController animationController = ((global::Doroti.Framework.Animation.AnimationController)(object?)createAnimationController());
            if (EqualityComparer<T>.Default.Equals(((CupertinoSegmentedControl<T>)(object)this.widget).groupValue, key))
            {
                this._childTweens.Add(this._reverseBackgroundColorTween);
                animationController.value = 1.0;
            }
            else
            {
                this._childTweens.Add(this._forwardBackgroundColorTween);
            }
            this._selectionControllers.Add(animationController);
        }
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        if (_updateColors())
        {
            _updateAnimationControllers();
        }
    }

    public override void didUpdateWidget(CupertinoSegmentedControl<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((_updateColors() || (checked((long)(((CupertinoSegmentedControl<T>)oldWidget).children.Count)) != checked((long)(((CupertinoSegmentedControl<T>)(object)this.widget).children.Count)))))
        {
            _updateAnimationControllers();
        }
        if (!EqualityComparer<T>.Default.Equals(((CupertinoSegmentedControl<T>)oldWidget).groupValue, ((CupertinoSegmentedControl<T>)(object)this.widget).groupValue))
        {
            var index = 0L;
            foreach (T key in ((CupertinoSegmentedControl<T>)(object)this.widget).children.Keys)
            {
                if (EqualityComparer<T>.Default.Equals(((CupertinoSegmentedControl<T>)(object)this.widget).groupValue, key))
                {
                    this._childTweens[(int)(index)] = this._forwardBackgroundColorTween;
                    this._selectionControllers[(int)(index)].forward();
                }
                else
                {
                    this._childTweens[(int)(index)] = this._reverseBackgroundColorTween;
                    this._selectionControllers[(int)(index)].reverse();
                }
                index += 1L;
            }
        }
    }

    public override void dispose()
    {
        foreach (global::Doroti.Framework.Animation.AnimationController animationController in this._selectionControllers)
        {
            animationController.dispose();
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual void _onTapDown(T currentKey)
    {
        if (((this._pressedKey is null) && !EqualityComparer<T>.Default.Equals(currentKey, ((CupertinoSegmentedControl<T>)(object)this.widget).groupValue)))
        {
            setState(((global::System.Action)(() =>
            {
                _pressedKey = currentKey;
            })));
        }
    }

    internal virtual void _onTapCancel()
    {
        setState(((global::System.Action)(() =>
        {
            _pressedKey = default(T);
        })));
    }

    internal virtual void _onTap(T currentKey)
    {
        if (!EqualityComparer<T>.Default.Equals(currentKey, this._pressedKey))
        {
            return;
        }
        if (!((CupertinoSegmentedControl<T>)(object)this.widget).disabledChildren.Contains(currentKey))
        {
            DartRuntimePrimitives.NullAware(this._segmentKeys.GetValueOrDefault(currentKey), __target => __target.currentState)?.requestFocus();
            if (!EqualityComparer<T>.Default.Equals(currentKey, ((CupertinoSegmentedControl<T>)(object)this.widget).groupValue))
            {
                this.widget.onValueChanged(currentKey);
            }
        }
        setState(((global::System.Action)(() =>
        {
            _pressedKey = default(T);
        })));
    }

    public virtual global::Doroti.Ui.Color? getTextColor(long index, T currentKey)
    {
        if (((CupertinoSegmentedControl<T>)(object)this.widget).disabledChildren.Contains(currentKey))
        {
            return ((global::Doroti.Ui.Color?)(object?)this._disabledTextColor);
        }
        if (this._selectionControllers[(int)(index)].isAnimating)
        {
            return ((global::Doroti.Ui.Color?)(object?)this._textColorTween.evaluate(this._selectionControllers[(int)(index)]));
        }
        if (EqualityComparer<T>.Default.Equals(((CupertinoSegmentedControl<T>)(object)this.widget).groupValue, currentKey))
        {
            return ((global::Doroti.Ui.Color?)(object?)this._unselectedColor);
        }
        return ((global::Doroti.Ui.Color?)(object?)this._selectedColor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Color? getBackgroundColor(long index, T currentKey)
    {
        if (((CupertinoSegmentedControl<T>)(object)this.widget).disabledChildren.Contains(currentKey))
        {
            return ((global::Doroti.Ui.Color?)(object?)(EqualityComparer<T>.Default.Equals(((CupertinoSegmentedControl<T>)(object)this.widget).groupValue, currentKey) ? this._selectedDisabledColor : this._unselectedDisabledColor));
        }
        if (this._selectionControllers[(int)(index)].isAnimating)
        {
            return ((global::Doroti.Ui.Color?)(object?)this._childTweens[(int)(index)].evaluate(this._selectionControllers[(int)(index)]));
        }
        if (EqualityComparer<T>.Default.Equals(((CupertinoSegmentedControl<T>)(object)this.widget).groupValue, currentKey))
        {
            return ((global::Doroti.Ui.Color?)(object?)this._selectedColor);
        }
        if (EqualityComparer<T>.Default.Equals(this._pressedKey, currentKey))
        {
            return ((global::Doroti.Ui.Color?)(object?)this._pressedColor);
        }
        return ((global::Doroti.Ui.Color?)(object?)this._unselectedColor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        var gestureChildren = new List<global::Doroti.Framework.Widgets.Widget>();
        var backgroundColorsLocal = new List<global::Doroti.Ui.Color>();
        var index = 0L;
        long? selectedIndexLocal = default!;
        long? pressedIndexLocal = default!;
        foreach (T currentKey in ((CupertinoSegmentedControl<T>)(object)this.widget).children.Keys)
        {
            selectedIndexLocal = ((EqualityComparer<T>.Default.Equals(((CupertinoSegmentedControl<T>)(object)this.widget).groupValue, currentKey)) ? index : selectedIndexLocal);
            pressedIndexLocal = ((EqualityComparer<T>.Default.Equals(this._pressedKey, currentKey)) ? index : pressedIndexLocal);
            global::Doroti.Framework.Painting.TextStyle textStyle = ((global::Doroti.Framework.Painting.TextStyle)(object?)DefaultTextStyle.of(context).style.copyWith(color: getTextColor(index, currentKey)));
            var iconTheme = new global::Doroti.Framework.Widgets.IconThemeData(color: getTextColor(index, currentKey));
            global::Doroti.Framework.Widgets.Widget childLocal = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Center(child: ((CupertinoSegmentedControl<T>)(object)this.widget).children.GetValueOrDefault(currentKey)));
            bool isEnabled = !((CupertinoSegmentedControl<T>)(object)this.widget).disabledChildren.Contains(currentKey);
            global::Doroti.Framework.Widgets.GlobalKey<_SegmentButtonState__segmented_control<T>> segmentKey = this._segmentKeys.putIfAbsent(currentKey, (() => global::Doroti.Framework.Widgets.GlobalKey<_SegmentButtonState__segmented_control<T>>.Create()));
            childLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _SegmentButton__segmented_control<T>(key: segmentKey, value: currentKey, enabled: isEnabled, child: new global::Doroti.Framework.Widgets.MouseRegion(cursor: (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb ? global::Doroti.Framework.Services.SystemMouseCursors.click : global::Doroti.Framework.Services.MouseCursor.defer), child: new global::Doroti.Framework.Widgets.GestureDetector(behavior: global::Doroti.Framework.Rendering.HitTestBehavior.opaque, onTapDown: ((global::System.Action<global::Doroti.Framework.Gestures.TapDownDetails>)(isEnabled ? ((@event) =>
            {
                _onTapDown(currentKey);
            }) : null)), onTapCancel: ((global::System.Action)(isEnabled ? this._onTapCancel : null)), onTap: ((global::System.Action)(() =>
            {
                if (isEnabled)
                {
                    DartRuntimePrimitives.NullAware(this._segmentKeys.GetValueOrDefault(currentKey), __target => __target.currentState)?.requestFocus();
                }
                _onTap(currentKey);
            })), child: new global::Doroti.Framework.Widgets.IconTheme(data: iconTheme, child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: textStyle, child: new global::Doroti.Framework.Widgets.Semantics(button: true, inMutuallyExclusiveGroup: true, selected: EqualityComparer<T>.Default.Equals(((CupertinoSegmentedControl<T>)(object)this.widget).groupValue, currentKey), child: childLocal)))))));
            backgroundColorsLocal.Add(getBackgroundColor(index, currentKey)!);
            gestureChildren.Add(childLocal);
            index += 1L;
        }
        global::Doroti.Framework.Widgets.Widget box = ((global::Doroti.Framework.Widgets.Widget)(object?)new _SegmentedControlRenderWidget__segmented_control<T>(selectedIndex: selectedIndexLocal, pressedIndex: pressedIndexLocal, backgroundColors: backgroundColorsLocal, borderColor: this._borderColor!, children: gestureChildren));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Actions(actions: new DartMap<Type, dynamic> { [typeof(global::Doroti.Framework.Widgets.VoidCallbackIntent)] = new global::Doroti.Framework.Widgets.VoidCallbackAction() }, child: new global::Doroti.Framework.Widgets.RadioGroup<T>(groupValue: ((CupertinoSegmentedControl<T>)(object)this.widget).groupValue, onChanged: ((global::System.Action<T?>)((value) =>
        {
            if (((value is not null) && !((CupertinoSegmentedControl<T>)(object)this.widget).disabledChildren.Contains(value)))
            {
                this.widget.onValueChanged(value);
            }
        })), child: new global::Doroti.Framework.Widgets.Padding(padding: (((CupertinoSegmentedControl<T>)(object)this.widget).padding ?? Segmented_controlLibrary._kHorizontalItemPadding), child: new global::Doroti.Framework.Widgets.UnconstrainedBox(constrainedAxis: global::Doroti.Framework.Painting.Axis.horizontal, child: box)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = this._tickerModeNotifier!.value;
        var result = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
    __cascade.muted = !((TickerModeData)values).enabled;
    __cascade.forceFrames = ((TickerModeData)values).forceFrames;
    return __cascade;
}))();
        this._tickers!.Add(result);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => (this._tickers is not null));
        DartRuntimePrimitives.Assert(() => this._tickers!.Contains(ticker));
        this._tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if ((this._tickers is not null))
        {
            TickerModeData values = this._tickerModeNotifier!.value;
            bool mutedLocal = !((TickerModeData)values).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = ((TickerModeData)values).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

internal class _SegmentedControlRenderWidget__segmented_control<T> : global::Doroti.Framework.Widgets.MultiChildRenderObjectWidget
{
    public virtual long? selectedIndex { get; private set; }
    public virtual long? pressedIndex { get; private set; }
    public virtual List<Color> backgroundColors { get; private set; } = default!;
    public virtual Color borderColor { get; private set; } = default!;

    internal _SegmentedControlRenderWidget__segmented_control(global::Doroti.Framework.Foundation.Key? key = null, List<global::Doroti.Framework.Widgets.Widget> children = default!, long? selectedIndex = default!, long? pressedIndex = default!, List<Color> backgroundColors = default!, Color borderColor = default!) : base(key: key, children: children ?? new List<global::Doroti.Framework.Widgets.Widget>())
    {
        this.selectedIndex = selectedIndex;
        this.pressedIndex = pressedIndex;
        this.backgroundColors = backgroundColors;
        this.borderColor = borderColor;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderSegmentedControl__segmented_control<T>(textDirection: Directionality.of(context), selectedIndex: this.selectedIndex, pressedIndex: this.pressedIndex, backgroundColors: this.backgroundColors, borderColor: this.borderColor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderSegmentedControl__segmented_control<T>)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderSegmentedControl__segmented_control<T>>)(() =>
{
    var __cascade = __renderObject;
    __cascade.textDirection = Directionality.of(context);
    __cascade.selectedIndex = this.selectedIndex;
    __cascade.pressedIndex = this.pressedIndex;
    __cascade.backgroundColors = this.backgroundColors;
    __cascade.borderColor = this.borderColor;
    return __cascade;
}))());
    }

}

internal class _SegmentedControlContainerBoxParentData__segmented_control : global::Doroti.Framework.Rendering.ContainerBoxParentData<global::Doroti.Framework.Rendering.RenderBox>
{
    public virtual RSuperellipse? surroundingRect { get; set; } = default;

}

internal delegate global::Doroti.Framework.Rendering.RenderBox? _NextChild__segmented_control(global::Doroti.Framework.Rendering.RenderBox child);

public class _RenderSegmentedControl__segmented_control<T> : global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.ContainerRenderObjectMixin<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.ContainerBoxParentData<global::Doroti.Framework.Rendering.RenderBox>>, global::Doroti.Framework.Rendering.RenderBoxContainerDefaultsMixin<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.ContainerBoxParentData<global::Doroti.Framework.Rendering.RenderBox>>
{
    internal virtual long? _selectedIndex { get; set; } = default;
    internal virtual long? _pressedIndex { get; set; } = default;
    internal virtual TextDirection _textDirection { get; set; } = default!;
    internal virtual List<Color> _backgroundColors { get; set; } = default!;
    internal virtual Color _borderColor { get; set; } = default!;
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;

    internal _RenderSegmentedControl__segmented_control(long? selectedIndex, long? pressedIndex, TextDirection textDirection, List<Color> backgroundColors, Color borderColor)
    {
        this._textDirection = textDirection;
        this._selectedIndex = selectedIndex;
        this._pressedIndex = pressedIndex;
        this._backgroundColors = backgroundColors;
        this._borderColor = borderColor;
    }

    public virtual long? selectedIndex
    {
        get => this._selectedIndex;
        set
        {
            var __value = value;
            if ((this._selectedIndex == __value))
            {
                return;
            }
            _selectedIndex = __value;
            markNeedsPaint();
        }
    }
    public virtual long? pressedIndex
    {
        get => this._pressedIndex;
        set
        {
            var __value = value;
            if ((this._pressedIndex == __value))
            {
                return;
            }
            _pressedIndex = __value;
            markNeedsPaint();
        }
    }
    public virtual global::Doroti.Ui.TextDirection textDirection
    {
        get => this._textDirection;
        set
        {
            var __value = value;
            if ((object.Equals(this._textDirection, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _textDirection = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual List<global::Doroti.Ui.Color> backgroundColors
    {
        get => this._backgroundColors;
        set
        {
            var __value = (List<Color>)(object)value;
            if ((object.Equals(this._backgroundColors, __value)))
            {
                return;
            }
            _backgroundColors = __value;
            markNeedsPaint();
        }
    }
    public virtual global::Doroti.Ui.Color borderColor
    {
        get => this._borderColor;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(this._borderColor, __value)))
            {
                return;
            }
            _borderColor = __value;
            markNeedsPaint();
        }
    }
    public override double computeMinIntrinsicWidth(double height)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        var minWidth = 0.0;
        while ((child is not null))
        {
            var childParentData = ((_SegmentedControlContainerBoxParentData__segmented_control?)(object?)child.parentData!)!;
            double childWidth = child.getMinIntrinsicWidth(height);
            minWidth = Math.Max(minWidth, childWidth);
            child = childParentData.nextSibling;
        }
        return (minWidth * this.childCount);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        var maxWidth = 0.0;
        while ((child is not null))
        {
            var childParentData = ((_SegmentedControlContainerBoxParentData__segmented_control?)(object?)child.parentData!)!;
            double childWidth = child.getMaxIntrinsicWidth(height);
            maxWidth = Math.Max(maxWidth, childWidth);
            child = childParentData.nextSibling;
        }
        return (maxWidth * this.childCount);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        var minHeight = 0.0;
        while ((child is not null))
        {
            var childParentData = ((_SegmentedControlContainerBoxParentData__segmented_control?)(object?)child.parentData!)!;
            double childHeight = child.getMinIntrinsicHeight(width);
            minHeight = Math.Max(minHeight, childHeight);
            child = childParentData.nextSibling;
        }
        return minHeight;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        var maxHeight = 0.0;
        while ((child is not null))
        {
            var childParentData = ((_SegmentedControlContainerBoxParentData__segmented_control?)(object?)child.parentData!)!;
            double childHeight = child.getMaxIntrinsicHeight(width);
            maxHeight = Math.Max(maxHeight, childHeight);
            child = childParentData.nextSibling;
        }
        return maxHeight;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        return defaultComputeDistanceToHighestActualBaseline(baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void setupParentData(global::Doroti.Framework.Rendering.RenderObject child)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)(object)child;
        if ((__child.parentData is not _SegmentedControlContainerBoxParentData__segmented_control))
        {
            __child.parentData = new _SegmentedControlContainerBoxParentData__segmented_control();
        }
    }

    internal virtual void _layoutRects(global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.RenderBox?> nextChild, global::Doroti.Framework.Rendering.RenderBox? leftChild, global::Doroti.Framework.Rendering.RenderBox? rightChild)
    {
        var child = leftChild;
        var start = 0.0;
        while ((child is not null))
        {
            var childParentData = ((_SegmentedControlContainerBoxParentData__segmented_control?)(object?)child.parentData!)!;
            var childOffset = new global::Doroti.Ui.Offset(start, 0.0);
            childParentData.offset = childOffset;
            var childRect = global::Doroti.Ui.Rect.fromLTWH(start, 0.0, ((global::Doroti.Framework.Rendering.RenderBox)child).size.width, ((global::Doroti.Framework.Rendering.RenderBox)child).size.height);
            global::Doroti.Ui.RSuperellipse rChildRect = default!;
            if ((object.Equals(child, leftChild)))
            {
                rChildRect = global::Doroti.Ui.RSuperellipse.fromRectAndCorners(childRect, topLeft: global::Doroti.Ui.Radius.circular(3.0), bottomLeft: global::Doroti.Ui.Radius.circular(3.0));
            }
            else
            {
                if ((object.Equals(child, rightChild)))
                {
                    rChildRect = global::Doroti.Ui.RSuperellipse.fromRectAndCorners(childRect, topRight: global::Doroti.Ui.Radius.circular(3.0), bottomRight: global::Doroti.Ui.Radius.circular(3.0));
                }
                else
                {
                    rChildRect = global::Doroti.Ui.RSuperellipse.fromRectAndCorners(childRect);
                }
            }
            childParentData.surroundingRect = rChildRect;
            start += ((global::Doroti.Framework.Rendering.RenderBox)child).size.width;
            child = nextChild(child);
        }
    }

    internal virtual global::Doroti.Ui.Size _calculateChildSize(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        double maxHeight = Segmented_controlLibrary._kMinSegmentedControlHeight;
        double childWidth = (((global::Doroti.Framework.Rendering.BoxConstraints)constraints).minWidth / this.childCount);
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        while ((child is not null))
        {
            childWidth = Math.Max(childWidth, child.getMaxIntrinsicWidth(double.PositiveInfinity));
            child = childAfter(child);
        }
        childWidth = Math.Min(childWidth, (((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth / this.childCount));
        child = this.firstChild;
        while ((child is not null))
        {
            double boxHeight = child.getMaxIntrinsicHeight(childWidth);
            maxHeight = Math.Max(maxHeight, boxHeight);
            child = childAfter(child);
        }
        return new global::Doroti.Ui.Size(childWidth, maxHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _computeOverallSizeFromChildSize(Size childSize)
    {
        return ((global::Doroti.Ui.Size)(object?)this.constraints.constrain(new global::Doroti.Ui.Size((childSize.width * this.childCount), childSize.height)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Ui.Size childSize = ((global::Doroti.Ui.Size)(object?)_calculateChildSize(constraints));
        var childConstraints = global::Doroti.Framework.Rendering.BoxConstraints.CreateTight(childSize);
        global::Doroti.Framework.Rendering.BaselineOffset baselineOffset = global::Doroti.Framework.Rendering.BaselineOffset.noBaseline;
        for (global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild; (child is not null); child = childAfter(child))
        {
            baselineOffset = baselineOffset.minOf(new global::Doroti.Framework.Rendering.BaselineOffset(child.getDryBaseline(childConstraints, baseline)));
        }
        return baselineOffset.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        global::Doroti.Ui.Size childSize = ((global::Doroti.Ui.Size)(object?)_calculateChildSize(constraints));
        return _computeOverallSizeFromChildSize(childSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        global::Doroti.Framework.Rendering.BoxConstraints constraintsLocal = this.constraints;
        global::Doroti.Ui.Size childSize = ((global::Doroti.Ui.Size)(object?)_calculateChildSize(constraintsLocal));
        var childConstraints = global::Doroti.Framework.Rendering.BoxConstraints.CreateTightFor(width: childSize.width, height: childSize.height);
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        while ((child is not null))
        {
            child.layout(childConstraints, parentUsesSize: true);
            child = childAfter(child);
        }
        switch (this.textDirection)
        {
            case TextDirection.rtl:
                {
                    _layoutRects((global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.RenderBox?>)this.childBefore, this.lastChild, this.firstChild);
                    break;
                }
            case TextDirection.ltr:
                {
                    _layoutRects((global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.RenderBox?>)this.childAfter, this.firstChild, this.lastChild);
                    break;
                }
        }
        size = _computeOverallSizeFromChildSize(childSize);
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        var index = 0L;
        while ((child is not null))
        {
            _paintChild(context, offset, child, index);
            child = childAfter(child);
            index += 1L;
        }
    }

    internal virtual void _paintChild(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset, global::Doroti.Framework.Rendering.RenderBox child, long childIndex)
    {
        var childParentData = ((_SegmentedControlContainerBoxParentData__segmented_control?)(object?)child.parentData!)!;
        ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRSuperellipse(((_SegmentedControlContainerBoxParentData__segmented_control)childParentData).surroundingRect!.shift(offset), ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = this.backgroundColors[(int)(childIndex)];
    __cascade.style = PaintingStyle.fill;
    return __cascade;
}))());
        ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRSuperellipse(((_SegmentedControlContainerBoxParentData__segmented_control)childParentData).surroundingRect!.shift(offset), ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = this.borderColor;
    __cascade.strokeWidth = 1.0;
    __cascade.style = PaintingStyle.stroke;
    return __cascade;
}))());
        context.paintChild(child, (childParentData.offset + offset));
    }

    public override bool hitTestChildren(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = this.lastChild;
        while ((child is not null))
        {
            var childParentData = ((_SegmentedControlContainerBoxParentData__segmented_control?)(object?)child.parentData!)!;
            if (((_SegmentedControlContainerBoxParentData__segmented_control)childParentData).surroundingRect!.outerRect.contains(position))
            {
                return result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: ((global::System.Func<global::Doroti.Framework.Rendering.BoxHitTestResult, Offset, bool>)((result, localOffset) =>
                {
                    DartRuntimePrimitives.Assert(() => (object.Equals(localOffset, (position - childParentData.offset))));
                    return child!.hitTest(result, position: localOffset);
                    throw new InvalidOperationException("Dart closure completed without a value.");
                })));
            }
            child = childParentData.previousSibling;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        while ((childParentData.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData.previousSibling, child)));
            child = childParentData.previousSibling!;
            childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        while ((childParentData.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData.nextSibling, child)));
            child = childParentData.nextSibling!;
            childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long childCount => this._childCount;
    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((child is not RenderBox))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"A {this.GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new global::Doroti.Framework.Foundation.ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new global::Doroti.Framework.Foundation.ErrorSpacer(), new global::Doroti.Framework.Foundation.DiagnosticsProperty<object?>($"The {this.GetType()} that expected a {typeof(RenderBox)} child was created by", this.debugCreator, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty), new global::Doroti.Framework.Foundation.ErrorSpacer(), new global::Doroti.Framework.Foundation.DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", ((RenderObject)child).debugCreator, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty) }));
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _insertIntoChildList(RenderBox child, RenderBox? after = null)
    {
        var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData = ((ContainerBoxParentData<RenderBox>?)(object?)this._firstChild!.parentData!)!;
                firstChildParentData.previousSibling = child;
            }
            this._firstChild = child;
            this._lastChild ??= child;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (this._firstChild is not null));
            DartRuntimePrimitives.Assert(() => (this._lastChild is not null));
            DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(after, equals: this._firstChild));
            DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(after, equals: this._lastChild));
            var afterParentData = ((ContainerBoxParentData<RenderBox>?)(object?)after.parentData!)!;
            if ((afterParentData.nextSibling is null))
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(after, this._lastChild)));
                childParentData.previousSibling = after;
                afterParentData.nextSibling = child;
                this._lastChild = child;
            }
            else
            {
                childParentData.nextSibling = afterParentData.nextSibling;
                childParentData.previousSibling = after;
                var childPreviousSiblingParentData = ((ContainerBoxParentData<RenderBox>?)(object?)childParentData.previousSibling!.parentData!)!;
                var childNextSiblingParentData = ((ContainerBoxParentData<RenderBox>?)(object?)childParentData.nextSibling!.parentData!)!;
                childPreviousSiblingParentData.nextSibling = child;
                childNextSiblingParentData.previousSibling = child;
                DartRuntimePrimitives.Assert(() => (object.Equals(afterParentData.nextSibling, child)));
            }
        }
    }

    public virtual void insert(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this)), () => (object?)"A RenderObject cannot be inserted into itself.");
        DartRuntimePrimitives.Assert(() => (!object.Equals(after, this)), () => (object?)"A RenderObject cannot simultaneously be both the parent and the sibling of another RenderObject.");
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, after)), () => (object?)"A RenderObject cannot be inserted after itself.");
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this._firstChild)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this._lastChild)));
        adoptChild(child);
        DartRuntimePrimitives.Assert(() => (child.parentData is ContainerBoxParentData<RenderBox>), () => (object?)$"A child of {this.GetType()} has parentData of type {DartRuntimePrimitives.RuntimeType(child.parentData)}, " + $"which does not conform to {typeof(ContainerBoxParentData<RenderBox>)}. Class using ContainerRenderObjectMixin " + $"should override setupParentData() to set parentData to type {typeof(ContainerBoxParentData<RenderBox>)}.");
        _insertIntoChildList(child, after: after);
    }

    public virtual void add(RenderBox child)
    {
        insert(child, after: this._lastChild);
    }

    public virtual void addAll(List<RenderBox>? children)
    {
        children?.forEach((__arg0) => ((global::System.Action<RenderBox>)this.add)(__arg0));
    }

    public virtual void _removeFromChildList(RenderBox child)
    {
        var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(child, equals: this._firstChild));
        DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(child, equals: this._lastChild));
        DartRuntimePrimitives.Assert(() => (this._childCount >= 0L));
        if ((childParentData.previousSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._firstChild, child)));
            this._firstChild = childParentData.nextSibling;
        }
        else
        {
            var childPreviousSiblingParentData = ((ContainerBoxParentData<RenderBox>?)(object?)childParentData.previousSibling!.parentData!)!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if ((childParentData.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = ((ContainerBoxParentData<RenderBox>?)(object?)childParentData.nextSibling!.parentData!)!;
            childNextSiblingParentData.previousSibling = childParentData.previousSibling;
        }
        childParentData.previousSibling = null;
        childParentData.nextSibling = null;
        this._childCount -= 1L;
    }

    public virtual void remove(RenderBox child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
            RenderBox? next = childParentData.nextSibling;
            childParentData.previousSibling = null;
            childParentData.nextSibling = null;
            dropChild(child);
            child = next;
        }
        this._firstChild = null;
        this._lastChild = null;
        this._childCount = 0L;
    }

    public virtual void move(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(after, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, after)));
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        if ((object.Equals(childParentData.previousSibling, after)))
        {
            return;
        }
        _removeFromChildList(child);
        _insertIntoChildList(child, after: after);
        markNeedsLayout();
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            ((dynamic)child).attach(owner);
            var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void detach()
    {
        base.detach();
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            ((dynamic)child).detach();
            var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void redepthChildren()
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            redepthChild(child);
            var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildren(global::System.Action<RenderObject> visitor)
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            visitor(child);
            var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => this._firstChild;
    public virtual RenderBox? lastChild => this._lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        return childParentData.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var children = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        if ((this.firstChild is not null))
        {
            RenderBox child = this.firstChild!;
            var count = 1L;
            while (true)
            {
                children.Add(((Diagnosticable)child).toDiagnosticsNode(name: $"child__183606 {count}"));
                if ((object.Equals(child, this.lastChild)))
                {
                    break;
                }
                count += 1L;
                var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
                child = childParentData.nextSibling!;
            }
        }
        return children;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToFirstActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !this.debugNeedsLayout);
        RenderBox? child = this.firstChild;
        while ((child is not null))
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
            double? result = child.getDistanceToActualBaseline(baseline);
            if ((result is not null))
            {
                double result__138852__value138916 = DartRuntimePrimitives.RequireValue(result);
                return (DartRuntimePrimitives.RequireValue(result__138852__value138916) + childParentData.offset.dy);
            }
            child = childParentData.nextSibling;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToHighestActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !this.debugNeedsLayout);
        BaselineOffset minBaseline = BaselineOffset.noBaseline;
        RenderBox? child = this.firstChild;
        while ((child is not null))
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
            BaselineOffset candidate = (new BaselineOffset(child.getDistanceToActualBaseline(baseline)).op_Add(childParentData.offset.dy));
            minBaseline = minBaseline.minOf(candidate);
            child = childParentData.nextSibling;
        }
        return minBaseline.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool defaultHitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child = this.lastChild;
        while ((child is not null))
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
            bool isHit = result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: ((global::System.Func<BoxHitTestResult, Offset, bool>)((result, transformed) =>
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - childParentData.offset))));
                return child!.hitTest(result, position: transformed);
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
            if (isHit)
            {
                return true;
            }
            child = childParentData.previousSibling;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void defaultPaint(PaintingContext context, Offset offset)
    {
        RenderBox? child = this.firstChild;
        while ((child is not null))
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
            context.paintChild(child, (childParentData.offset + offset));
            child = childParentData.nextSibling;
        }
    }

    public virtual List<RenderBox> getChildrenAsList()
    {
        var result = new List<RenderBox>();
        RenderBox? child = this.firstChild;
        while ((child is not null))
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
            result.Add(((RenderBox?)(object?)child)!);
            child = childParentData.nextSibling;
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
