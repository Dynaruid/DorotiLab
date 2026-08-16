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
{            var __cascade = new global::Doroti.Framework.Animation.AnimationController(duration: Segmented_controlLibrary._kFadeDuration, vsync: this);
            __cascade.addListener(((global::System.Action)(() => {
setState(((global::System.Action)(() => {
})));
})));
            return __cascade;        }))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _updateColors()
    {
        DartRuntimePrimitives.Assert(() => this.mounted, () => (object?)"This should only be called after didUpdateDependencies");
        var changed__10908 = false;
        global::Doroti.Ui.Color disabledTextColor__10941 = ((global::Doroti.Ui.Color)(object?)(((CupertinoSegmentedControl<T>)(object)this.widget).disabledTextColor ?? Segmented_controlLibrary._kDisableTextColor));
        if ((!object.Equals(this._disabledTextColor, disabledTextColor__10941)))
        {
            changed__10908 = true;
            _disabledTextColor = disabledTextColor__10941;
        }
        global::Doroti.Ui.Color selectedColor__11150 = ((global::Doroti.Ui.Color)(object?)(((CupertinoSegmentedControl<T>)(object)this.widget).selectedColor ?? CupertinoTheme.of(this.context).primaryColor));
        if ((!object.Equals(this._selectedColor, selectedColor__11150)))
        {
            changed__10908 = true;
            _selectedColor = selectedColor__11150;
        }
        global::Doroti.Ui.Color unselectedColor__11356 = ((global::Doroti.Ui.Color)(object?)(((CupertinoSegmentedControl<T>)(object)this.widget).unselectedColor ?? CupertinoTheme.of(this.context).primaryContrastingColor));
        if ((!object.Equals(this._unselectedColor, unselectedColor__11356)))
        {
            changed__10908 = true;
            _unselectedColor = unselectedColor__11356;
        }
        global::Doroti.Ui.Color selectedDisabledColor__11593 = ((global::Doroti.Ui.Color)(object?)(((CupertinoSegmentedControl<T>)(object)this.widget).disabledColor ?? selectedColor__11150.withOpacity(0.5)));
        global::Doroti.Ui.Color unselectedDisabledColor__11689 = ((global::Doroti.Ui.Color)(object?)(((CupertinoSegmentedControl<T>)(object)this.widget).disabledColor ?? unselectedColor__11356));
        if (((!object.Equals(this._selectedDisabledColor, selectedDisabledColor__11593)) || (!object.Equals(this._unselectedDisabledColor, unselectedDisabledColor__11689))))
        {
            changed__10908 = true;
            _selectedDisabledColor = selectedDisabledColor__11593;
            _unselectedDisabledColor = unselectedDisabledColor__11689;
        }
        global::Doroti.Ui.Color borderColor__12034 = ((global::Doroti.Ui.Color)(object?)(((CupertinoSegmentedControl<T>)(object)this.widget).borderColor ?? CupertinoTheme.of(this.context).primaryColor));
        if ((!object.Equals(this._borderColor, borderColor__12034)))
        {
            changed__10908 = true;
            _borderColor = borderColor__12034;
        }
        global::Doroti.Ui.Color pressedColor__12228 = ((global::Doroti.Ui.Color)(object?)(((CupertinoSegmentedControl<T>)(object)this.widget).pressedColor ?? CupertinoTheme.of(this.context).primaryColor.withOpacity(0.2)));
        if ((!object.Equals(this._pressedColor, pressedColor__12228)))
        {
            changed__10908 = true;
            _pressedColor = pressedColor__12228;
        }
        _forwardBackgroundColorTween = new global::Doroti.Framework.Animation.ColorTween(begin: this._pressedColor, end: this._selectedColor);
        _reverseBackgroundColorTween = new global::Doroti.Framework.Animation.ColorTween(begin: this._unselectedColor, end: this._selectedColor);
        _textColorTween = new global::Doroti.Framework.Animation.ColorTween(begin: this._selectedColor, end: this._unselectedColor);
        return changed__10908;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _updateAnimationControllers()
    {
        DartRuntimePrimitives.Assert(() => this.mounted, () => (object?)"This should only be called after didUpdateDependencies");
        foreach (global::Doroti.Framework.Animation.AnimationController controller__12879 in this._selectionControllers)
        {
            controller__12879.dispose();
        }
        this._selectionControllers.Clear();
        this._childTweens.Clear();
        foreach (T key__13031 in ((CupertinoSegmentedControl<T>)(object)this.widget).children.Keys)
        {
            global::Doroti.Framework.Animation.AnimationController animationController__13094 = ((global::Doroti.Framework.Animation.AnimationController)(object?)createAnimationController());
            if (EqualityComparer<T>.Default.Equals(((CupertinoSegmentedControl<T>)(object)this.widget).groupValue, key__13031))
            {
                this._childTweens.Add(this._reverseBackgroundColorTween);
                animationController__13094.value = 1.0;
            }
            else
            {
                this._childTweens.Add(this._forwardBackgroundColorTween);
            }
            this._selectionControllers.Add(animationController__13094);
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
            var index__13885 = 0L;
            foreach (T key__13915 in ((CupertinoSegmentedControl<T>)(object)this.widget).children.Keys)
            {
                if (EqualityComparer<T>.Default.Equals(((CupertinoSegmentedControl<T>)(object)this.widget).groupValue, key__13915))
                {
                    this._childTweens[(int)(index__13885)] = this._forwardBackgroundColorTween;
                    this._selectionControllers[(int)(index__13885)].forward();
                }
                else
                {
                    this._childTweens[(int)(index__13885)] = this._reverseBackgroundColorTween;
                    this._selectionControllers[(int)(index__13885)].reverse();
                }
                index__13885 += 1L;
            }
        }
    }

    public override void dispose()
    {
        foreach (global::Doroti.Framework.Animation.AnimationController animationController__14342 in this._selectionControllers)
        {
            animationController__14342.dispose();
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker__18989 in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker__18989).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker__18989.describeForError("The offending ticker was") }));
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
            setState(((global::System.Action)(() => {
_pressedKey = currentKey;
})));
        }
    }

    internal virtual void _onTapCancel()
    {
        setState(((global::System.Action)(() => {
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
        setState(((global::System.Action)(() => {
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
        var gestureChildren__16054 = new List<global::Doroti.Framework.Widgets.Widget>();
        var backgroundColors__16094 = new List<global::Doroti.Ui.Color>();
        var index__16132 = 0L;
        long? selectedIndex__16152 = default!;
        long? pressedIndex__16176 = default!;
        foreach (T currentKey__16207 in ((CupertinoSegmentedControl<T>)(object)this.widget).children.Keys)
        {
            selectedIndex__16152 = ((EqualityComparer<T>.Default.Equals(((CupertinoSegmentedControl<T>)(object)this.widget).groupValue, currentKey__16207)) ? index__16132 : selectedIndex__16152);
            pressedIndex__16176 = ((EqualityComparer<T>.Default.Equals(this._pressedKey, currentKey__16207)) ? index__16132 : pressedIndex__16176);
            global::Doroti.Framework.Painting.TextStyle textStyle__16422 = ((global::Doroti.Framework.Painting.TextStyle)(object?)DefaultTextStyle.of(context).style.copyWith(color: getTextColor(index__16132, currentKey__16207)));
            var iconTheme__16548 = new global::Doroti.Framework.Widgets.IconThemeData(color: getTextColor(index__16132, currentKey__16207));
            global::Doroti.Framework.Widgets.Widget child__16629 = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Center(child: ((CupertinoSegmentedControl<T>)(object)this.widget).children.GetValueOrDefault(currentKey__16207)));
            bool isEnabled__16699 = !((CupertinoSegmentedControl<T>)(object)this.widget).disabledChildren.Contains(currentKey__16207);
            global::Doroti.Framework.Widgets.GlobalKey<_SegmentButtonState__segmented_control<T>> segmentKey__16805 = this._segmentKeys.putIfAbsent(currentKey__16207, (() => global::Doroti.Framework.Widgets.GlobalKey<_SegmentButtonState__segmented_control<T>>.Create()));
            child__16629 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _SegmentButton__segmented_control<T>(key: segmentKey__16805, value: currentKey__16207, enabled: isEnabled__16699, child: new global::Doroti.Framework.Widgets.MouseRegion(cursor: (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb ? global::Doroti.Framework.Services.SystemMouseCursors.click : global::Doroti.Framework.Services.MouseCursor.defer), child: new global::Doroti.Framework.Widgets.GestureDetector(behavior: global::Doroti.Framework.Rendering.HitTestBehavior.opaque, onTapDown: ((global::System.Action<global::Doroti.Framework.Gestures.TapDownDetails>)(isEnabled__16699 ? ((@event) => {
_onTapDown(currentKey__16207);
}) : null)), onTapCancel: ((global::System.Action)(isEnabled__16699 ? this._onTapCancel : null)), onTap: ((global::System.Action)(() => {
if (isEnabled__16699)
{
    DartRuntimePrimitives.NullAware(this._segmentKeys.GetValueOrDefault(currentKey__16207), __target => __target.currentState)?.requestFocus();
}
_onTap(currentKey__16207);
})), child: new global::Doroti.Framework.Widgets.IconTheme(data: iconTheme__16548, child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: textStyle__16422, child: new global::Doroti.Framework.Widgets.Semantics(button: true, inMutuallyExclusiveGroup: true, selected: EqualityComparer<T>.Default.Equals(((CupertinoSegmentedControl<T>)(object)this.widget).groupValue, currentKey__16207), child: child__16629)))))));
            backgroundColors__16094.Add(getBackgroundColor(index__16132, currentKey__16207)!);
            gestureChildren__16054.Add(child__16629);
            index__16132 += 1L;
        }
        global::Doroti.Framework.Widgets.Widget box__18205 = ((global::Doroti.Framework.Widgets.Widget)(object?)new _SegmentedControlRenderWidget__segmented_control<T>(selectedIndex: selectedIndex__16152, pressedIndex: pressedIndex__16176, backgroundColors: backgroundColors__16094, borderColor: this._borderColor!, children: gestureChildren__16054));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Actions(actions: new DartMap<Type, dynamic> { [typeof(global::Doroti.Framework.Widgets.VoidCallbackIntent)] = new global::Doroti.Framework.Widgets.VoidCallbackAction() }, child: new global::Doroti.Framework.Widgets.RadioGroup<T>(groupValue: ((CupertinoSegmentedControl<T>)(object)this.widget).groupValue, onChanged: ((global::System.Action<T?>)((value) => {
if (((value is not null) && !((CupertinoSegmentedControl<T>)(object)this.widget).disabledChildren.Contains(value)))
{
    this.widget.onValueChanged(value);
}
})), child: new global::Doroti.Framework.Widgets.Padding(padding: (((CupertinoSegmentedControl<T>)(object)this.widget).padding ?? Segmented_controlLibrary._kHorizontalItemPadding), child: new global::Doroti.Framework.Widgets.UnconstrainedBox(constrainedAxis: global::Doroti.Framework.Painting.Axis.horizontal, child: box__18205)))));
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
        TickerModeData values__17506 = this._tickerModeNotifier!.value;
        var result__17553 = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{            var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
            __cascade.muted = !((TickerModeData)values__17506).enabled;
            __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
            return __cascade;        }))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result__17553);
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
            TickerModeData values__18318 = this._tickerModeNotifier!.value;
            bool muted__18372 = !((TickerModeData)values__18318).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker__18421 in this._tickers!)
            {
                ticker__18421.muted = muted__18372;
                ticker__18421.forceFrames = ((TickerModeData)values__18318).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__18621 = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__18621, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier__18621.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier__18621;
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
{            var __cascade = __renderObject;
            __cascade.textDirection = Directionality.of(context);
            __cascade.selectedIndex = this.selectedIndex;
            __cascade.pressedIndex = this.pressedIndex;
            __cascade.backgroundColors = this.backgroundColors;
            __cascade.borderColor = this.borderColor;
            return __cascade;        }))());
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
        global::Doroti.Framework.Rendering.RenderBox? child__22039 = this.firstChild;
        var minWidth__22067 = 0.0;
        while ((child__22039 is not null))
        {
            var childParentData__22123 = ((_SegmentedControlContainerBoxParentData__segmented_control?)(object?)child__22039.parentData!)!;
            double childWidth__22222 = child__22039.getMinIntrinsicWidth(height);
            minWidth__22067 = Math.Max(minWidth__22067, childWidth__22222);
            child__22039 = childParentData__22123.nextSibling;
        }
        return (minWidth__22067 * this.childCount);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        global::Doroti.Framework.Rendering.RenderBox? child__22486 = this.firstChild;
        var maxWidth__22514 = 0.0;
        while ((child__22486 is not null))
        {
            var childParentData__22570 = ((_SegmentedControlContainerBoxParentData__segmented_control?)(object?)child__22486.parentData!)!;
            double childWidth__22669 = child__22486.getMaxIntrinsicWidth(height);
            maxWidth__22514 = Math.Max(maxWidth__22514, childWidth__22669);
            child__22486 = childParentData__22570.nextSibling;
        }
        return (maxWidth__22514 * this.childCount);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        global::Doroti.Framework.Rendering.RenderBox? child__22933 = this.firstChild;
        var minHeight__22961 = 0.0;
        while ((child__22933 is not null))
        {
            var childParentData__23018 = ((_SegmentedControlContainerBoxParentData__segmented_control?)(object?)child__22933.parentData!)!;
            double childHeight__23117 = child__22933.getMinIntrinsicHeight(width);
            minHeight__22961 = Math.Max(minHeight__22961, childHeight__23117);
            child__22933 = childParentData__23018.nextSibling;
        }
        return minHeight__22961;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        global::Doroti.Framework.Rendering.RenderBox? child__23373 = this.firstChild;
        var maxHeight__23401 = 0.0;
        while ((child__23373 is not null))
        {
            var childParentData__23458 = ((_SegmentedControlContainerBoxParentData__segmented_control?)(object?)child__23373.parentData!)!;
            double childHeight__23557 = child__23373.getMaxIntrinsicHeight(width);
            maxHeight__23401 = Math.Max(maxHeight__23401, childHeight__23557);
            child__23373 = childParentData__23458.nextSibling;
        }
        return maxHeight__23401;
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
        var child__24189 = leftChild;
        var start__24216 = 0.0;
        while ((child__24189 is not null))
        {
            var childParentData__24269 = ((_SegmentedControlContainerBoxParentData__segmented_control?)(object?)child__24189.parentData!)!;
            var childOffset__24361 = new global::Doroti.Ui.Offset(start__24216, 0.0);
            childParentData__24269.offset = childOffset__24361;
            var childRect__24451 = global::Doroti.Ui.Rect.fromLTWH(start__24216, 0.0, ((global::Doroti.Framework.Rendering.RenderBox)child__24189).size.width, ((global::Doroti.Framework.Rendering.RenderBox)child__24189).size.height);
            global::Doroti.Ui.RSuperellipse rChildRect__24553 = default!;
            if ((object.Equals(child__24189, leftChild)))
            {
                rChildRect__24553 = global::Doroti.Ui.RSuperellipse.fromRectAndCorners(childRect__24451, topLeft: global::Doroti.Ui.Radius.circular(3.0), bottomLeft: global::Doroti.Ui.Radius.circular(3.0));
            }
            else
            {
                if ((object.Equals(child__24189, rightChild)))
                {
                    rChildRect__24553 = global::Doroti.Ui.RSuperellipse.fromRectAndCorners(childRect__24451, topRight: global::Doroti.Ui.Radius.circular(3.0), bottomRight: global::Doroti.Ui.Radius.circular(3.0));
                }
                else
                {
                    rChildRect__24553 = global::Doroti.Ui.RSuperellipse.fromRectAndCorners(childRect__24451);
                }
            }
            childParentData__24269.surroundingRect = rChildRect__24553;
            start__24216 += ((global::Doroti.Framework.Rendering.RenderBox)child__24189).size.width;
            child__24189 = nextChild(child__24189);
        }
    }

    internal virtual global::Doroti.Ui.Size _calculateChildSize(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        double maxHeight__25292 = Segmented_controlLibrary._kMinSegmentedControlHeight;
        double childWidth__25344 = (((global::Doroti.Framework.Rendering.BoxConstraints)constraints).minWidth / this.childCount);
        global::Doroti.Framework.Rendering.RenderBox? child__25407 = this.firstChild;
        while ((child__25407 is not null))
        {
            childWidth__25344 = Math.Max(childWidth__25344, child__25407.getMaxIntrinsicWidth(double.PositiveInfinity));
            child__25407 = childAfter(child__25407);
        }
        childWidth__25344 = Math.Min(childWidth__25344, (((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth / this.childCount));
        child__25407 = this.firstChild;
        while ((child__25407 is not null))
        {
            double boxHeight__25725 = child__25407.getMaxIntrinsicHeight(childWidth__25344);
            maxHeight__25292 = Math.Max(maxHeight__25292, boxHeight__25725);
            child__25407 = childAfter(child__25407);
        }
        return new global::Doroti.Ui.Size(childWidth__25344, maxHeight__25292);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _computeOverallSizeFromChildSize(Size childSize)
    {
        return ((global::Doroti.Ui.Size)(object?)this.constraints.constrain(new global::Doroti.Ui.Size((childSize.width * this.childCount), childSize.height)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Ui.Size childSize__26182 = ((global::Doroti.Ui.Size)(object?)_calculateChildSize(constraints));
        var childConstraints__26238 = global::Doroti.Framework.Rendering.BoxConstraints.CreateTight(childSize__26182);
        global::Doroti.Framework.Rendering.BaselineOffset baselineOffset__26310 = global::Doroti.Framework.Rendering.BaselineOffset.noBaseline;
        for (global::Doroti.Framework.Rendering.RenderBox? child__26374 = this.firstChild; (child__26374 is not null); child__26374 = childAfter(child__26374))
        {
            baselineOffset__26310 = baselineOffset__26310.minOf(new global::Doroti.Framework.Rendering.BaselineOffset(child__26374.getDryBaseline(childConstraints__26238, baseline)));
        }
        return baselineOffset__26310.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        global::Doroti.Ui.Size childSize__26692 = ((global::Doroti.Ui.Size)(object?)_calculateChildSize(constraints));
        return _computeOverallSizeFromChildSize(childSize__26692);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        global::Doroti.Framework.Rendering.BoxConstraints constraints__26861 = this.constraints;
        global::Doroti.Ui.Size childSize__26908 = ((global::Doroti.Ui.Size)(object?)_calculateChildSize(constraints__26861));
        var childConstraints__26965 = global::Doroti.Framework.Rendering.BoxConstraints.CreateTightFor(width: childSize__26908.width, height: childSize__26908.height);
        global::Doroti.Framework.Rendering.RenderBox? child__27094 = this.firstChild;
        while ((child__27094 is not null))
        {
            child__27094.layout(childConstraints__26965, parentUsesSize: true);
            child__27094 = childAfter(child__27094);
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
        size = _computeOverallSizeFromChildSize(childSize__26908);
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        global::Doroti.Framework.Rendering.RenderBox? child__27596 = this.firstChild;
        var index__27624 = 0L;
        while ((child__27596 is not null))
        {
            _paintChild(context, offset, child__27596, index__27624);
            child__27596 = childAfter(child__27596);
            index__27624 += 1L;
        }
    }

    internal virtual void _paintChild(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset, global::Doroti.Framework.Rendering.RenderBox child, long childIndex)
    {
        var childParentData__27879 = ((_SegmentedControlContainerBoxParentData__segmented_control?)(object?)child.parentData!)!;
        ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRSuperellipse(((_SegmentedControlContainerBoxParentData__segmented_control)childParentData__27879).surroundingRect!.shift(offset), ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = this.backgroundColors[(int)(childIndex)];
            __cascade.style = PaintingStyle.fill;
            return __cascade;        }))());
        ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRSuperellipse(((_SegmentedControlContainerBoxParentData__segmented_control)childParentData__27879).surroundingRect!.shift(offset), ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = this.borderColor;
            __cascade.strokeWidth = 1.0;
            __cascade.style = PaintingStyle.stroke;
            return __cascade;        }))());
        context.paintChild(child, (childParentData__27879.offset + offset));
    }

    public override bool hitTestChildren(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        global::Doroti.Framework.Rendering.RenderBox? child__28544 = this.lastChild;
        while ((child__28544 is not null))
        {
            var childParentData__28603 = ((_SegmentedControlContainerBoxParentData__segmented_control?)(object?)child__28544.parentData!)!;
            if (((_SegmentedControlContainerBoxParentData__segmented_control)childParentData__28603).surroundingRect!.outerRect.contains(position))
            {
                return result.addWithPaintOffset(offset: childParentData__28603.offset, position: position, hitTest: ((global::System.Func<global::Doroti.Framework.Rendering.BoxHitTestResult, Offset, bool>)((result, localOffset) => {
DartRuntimePrimitives.Assert(() => (object.Equals(localOffset, (position - childParentData__28603.offset))));
return child__28544!.hitTest(result, position: localOffset);
throw new InvalidOperationException("Dart closure completed without a value.");
})));
            }
            child__28544 = childParentData__28603.previousSibling;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData__173585 = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        while ((childParentData__173585.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173585.previousSibling, child)));
            child = childParentData__173585.previousSibling!;
            childParentData__173585 = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData__173981 = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        while ((childParentData__173981.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173981.nextSibling, child)));
            child = childParentData__173981.nextSibling!;
            childParentData__173981 = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
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
        var childParentData__175971 = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData__175971.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData__175971.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData__175971.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData__176343 = ((ContainerBoxParentData<RenderBox>?)(object?)this._firstChild!.parentData!)!;
                firstChildParentData__176343.previousSibling = child;
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
            var afterParentData__176766 = ((ContainerBoxParentData<RenderBox>?)(object?)after.parentData!)!;
            if ((afterParentData__176766.nextSibling is null))
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(after, this._lastChild)));
                childParentData__175971.previousSibling = after;
                afterParentData__176766.nextSibling = child;
                this._lastChild = child;
            }
            else
            {
                childParentData__175971.nextSibling = afterParentData__176766.nextSibling;
                childParentData__175971.previousSibling = after;
                var childPreviousSiblingParentData__177424 = ((ContainerBoxParentData<RenderBox>?)(object?)childParentData__175971.previousSibling!.parentData!)!;
                var childNextSiblingParentData__177547 = ((ContainerBoxParentData<RenderBox>?)(object?)childParentData__175971.nextSibling!.parentData!)!;
                childPreviousSiblingParentData__177424.nextSibling = child;
                childNextSiblingParentData__177547.previousSibling = child;
                DartRuntimePrimitives.Assert(() => (object.Equals(afterParentData__176766.nextSibling, child)));
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
        var childParentData__179226 = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(child, equals: this._firstChild));
        DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(child, equals: this._lastChild));
        DartRuntimePrimitives.Assert(() => (this._childCount >= 0L));
        if ((childParentData__179226.previousSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._firstChild, child)));
            this._firstChild = childParentData__179226.nextSibling;
        }
        else
        {
            var childPreviousSiblingParentData__179613 = ((ContainerBoxParentData<RenderBox>?)(object?)childParentData__179226.previousSibling!.parentData!)!;
            childPreviousSiblingParentData__179613.nextSibling = childParentData__179226.nextSibling;
        }
        if ((childParentData__179226.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData__179226.previousSibling;
        }
        else
        {
            var childNextSiblingParentData__179965 = ((ContainerBoxParentData<RenderBox>?)(object?)childParentData__179226.nextSibling!.parentData!)!;
            childNextSiblingParentData__179965.previousSibling = childParentData__179226.previousSibling;
        }
        childParentData__179226.previousSibling = null;
        childParentData__179226.nextSibling = null;
        this._childCount -= 1L;
    }

    public virtual void remove(RenderBox child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderBox? child__180623 = this._firstChild;
        while ((child__180623 is not null))
        {
            var childParentData__180684 = ((ContainerBoxParentData<RenderBox>?)(object?)child__180623.parentData!)!;
            RenderBox? next__180762 = childParentData__180684.nextSibling;
            childParentData__180684.previousSibling = null;
            childParentData__180684.nextSibling = null;
            dropChild(child__180623);
            child__180623 = next__180762;
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
        var childParentData__181479 = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        if ((object.Equals(childParentData__181479.previousSibling, after)))
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
        RenderBox? child__181803 = this._firstChild;
        while ((child__181803 is not null))
        {
            ((dynamic)child__181803).attach(owner);
            var childParentData__181891 = ((ContainerBoxParentData<RenderBox>?)(object?)child__181803.parentData!)!;
            child__181803 = childParentData__181891.nextSibling;
        }
    }

    public override void detach()
    {
        base.detach();
        RenderBox? child__182065 = this._firstChild;
        while ((child__182065 is not null))
        {
            ((dynamic)child__182065).detach();
            var childParentData__182148 = ((ContainerBoxParentData<RenderBox>?)(object?)child__182065.parentData!)!;
            child__182065 = childParentData__182148.nextSibling;
        }
    }

    public override void redepthChildren()
    {
        RenderBox? child__182311 = this._firstChild;
        while ((child__182311 is not null))
        {
            redepthChild(child__182311);
            var childParentData__182399 = ((ContainerBoxParentData<RenderBox>?)(object?)child__182311.parentData!)!;
            child__182311 = childParentData__182399.nextSibling;
        }
    }

    public override void visitChildren(global::System.Action<RenderObject> visitor)
    {
        RenderBox? child__182587 = this._firstChild;
        while ((child__182587 is not null))
        {
            visitor(child__182587);
            var childParentData__182670 = ((ContainerBoxParentData<RenderBox>?)(object?)child__182587.parentData!)!;
            child__182587 = childParentData__182670.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => this._firstChild;
    public virtual RenderBox? lastChild => this._lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183103 = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        return childParentData__183103.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183356 = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        return childParentData__183356.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var children__183528 = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        if ((this.firstChild is not null))
        {
            RenderBox child__183606 = this.firstChild!;
            var count__183637 = 1L;
            while (true)
            {
                children__183528.Add(((Diagnosticable)child__183606).toDiagnosticsNode(name: $"child__183606 {count__183637}"));
                if ((object.Equals(child__183606, this.lastChild)))
                {
                    break;
                }
                count__183637 += 1L;
                var childParentData__183833 = ((ContainerBoxParentData<RenderBox>?)(object?)child__183606.parentData!)!;
                child__183606 = childParentData__183833.nextSibling!;
            }
        }
        return children__183528;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToFirstActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !this.debugNeedsLayout);
        RenderBox? child__138717 = this.firstChild;
        while ((child__138717 is not null))
        {
            var childParentData__138777 = ((ContainerBoxParentData<RenderBox>?)(object?)child__138717.parentData!)!;
            double? result__138852 = child__138717.getDistanceToActualBaseline(baseline);
            if ((result__138852 is not null))
            {
                double result__138852__value138916 = DartRuntimePrimitives.RequireValue(result__138852);
                return (DartRuntimePrimitives.RequireValue(result__138852__value138916) + childParentData__138777.offset.dy);
            }
            child__138717 = childParentData__138777.nextSibling;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToHighestActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !this.debugNeedsLayout);
        BaselineOffset minBaseline__139372 = BaselineOffset.noBaseline;
        RenderBox? child__139428 = this.firstChild;
        while ((child__139428 is not null))
        {
            var childParentData__139488 = ((ContainerBoxParentData<RenderBox>?)(object?)child__139428.parentData!)!;
            BaselineOffset candidate__139570 = (new BaselineOffset(child__139428.getDistanceToActualBaseline(baseline)).op_Add(childParentData__139488.offset.dy));
            minBaseline__139372 = minBaseline__139372.minOf(candidate__139570);
            child__139428 = childParentData__139488.nextSibling;
        }
        return minBaseline__139372.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool defaultHitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child__140279 = this.lastChild;
        while ((child__140279 is not null))
        {
            var childParentData__140418 = ((ContainerBoxParentData<RenderBox>?)(object?)child__140279.parentData!)!;
            bool isHit__140490 = result.addWithPaintOffset(offset: childParentData__140418.offset, position: position, hitTest: ((global::System.Func<BoxHitTestResult, Offset, bool>)((result, transformed) => {
DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - childParentData__140418.offset))));
return child__140279!.hitTest(result, position: transformed);
throw new InvalidOperationException("Dart closure completed without a value.");
})));
            if (isHit__140490)
            {
                return true;
            }
            child__140279 = childParentData__140418.previousSibling;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void defaultPaint(PaintingContext context, Offset offset)
    {
        RenderBox? child__141240 = this.firstChild;
        while ((child__141240 is not null))
        {
            var childParentData__141300 = ((ContainerBoxParentData<RenderBox>?)(object?)child__141240.parentData!)!;
            context.paintChild(child__141240, (childParentData__141300.offset + offset));
            child__141240 = childParentData__141300.nextSibling;
        }
    }

    public virtual List<RenderBox> getChildrenAsList()
    {
        var result__141793 = new List<RenderBox>();
        RenderBox? child__141832 = this.firstChild;
        while ((child__141832 is not null))
        {
            var childParentData__141892 = ((ContainerBoxParentData<RenderBox>?)(object?)child__141832.parentData!)!;
            result__141793.Add(((RenderBox?)(object?)child__141832)!);
            child__141832 = childParentData__141892.nextSibling;
        }
        return result__141793;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
