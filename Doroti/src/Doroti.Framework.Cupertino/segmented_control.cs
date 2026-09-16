// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/segmented_control.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class Segmented_controlLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsetsGeometry _kHorizontalItemPadding = EdgeInsets.CreateSymmetric(horizontal: 16.0);
}

public static partial class Segmented_controlLibrary
{
    internal static double _kMinSegmentedControlHeight = 28.0;
}

public static partial class Segmented_controlLibrary
{
    internal static Color _kDisableTextColor = Color.fromARGB(115L, 122L, 122L, 122L);
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
        System.Diagnostics.Debug.Assert(checked(children.Count) >= 2L);
        System.Diagnostics.Debug.Assert((groupValue is null) || children.Keys.any((child) => EqualityComparer<T>.Default.Equals(child, groupValue)));
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
        _focusNode = new global::Doroti.Framework.Widgets.FocusNode(debugLabel: $"CupertinoSegmentedControl<{typeof(T)}>[{widget.value}]");
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        registry = widget.enabled ? RadioGroup.maybeOf<T>(context) : null;
    }

    public override void didUpdateWidget(_SegmentButton__segmented_control<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (oldWidget.enabled != widget.enabled)
        {
            registry = widget.enabled ? RadioGroup.maybeOf<T>(context) : null;
        }
    }

    public override void dispose()
    {
        registry = null;
        _focusNode.dispose();
        base.dispose();
    }

    public virtual T radioValue => widget.value;
    public virtual global::Doroti.Framework.Widgets.FocusNode focusNode => _focusNode;
    public virtual bool tristate => false;
    public virtual bool enabled => widget.enabled;
    public virtual void requestFocus()
    {
        if (widget.enabled)
        {
            _focusNode.requestFocus();
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.Focus(focusNode: _focusNode, canRequestFocus: widget.enabled, onKeyEvent: (node, @event) => KeyEventResult.ignored, child: widget.child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RadioGroupRegistry<T>? registry
    {
        get => _registry;
        set
        {
            var newRegistry = value;
            if (!Equals(_registry, newRegistry))
            {
                _registry?.unregisterClient(this);
            }
            _registry = newRegistry;
            _registry?.registerClient(this);
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
    __cascade.addListener(() =>
    {
        setState(() =>
        {
        });
    });
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _updateColors()
    {
        DartRuntimePrimitives.Assert(() => mounted, () => (object?)"This should only be called after didUpdateDependencies");
        var changed = false;
        global::Doroti.Ui.Color disabledTextColorLocal = widget.disabledTextColor ?? Segmented_controlLibrary._kDisableTextColor;
        if (!Equals(_disabledTextColor, disabledTextColorLocal))
        {
            changed = true;
            _disabledTextColor = disabledTextColorLocal;
        }
        global::Doroti.Ui.Color selectedColorLocal = widget.selectedColor ?? CupertinoTheme.of(context).primaryColor;
        if (!Equals(_selectedColor, selectedColorLocal))
        {
            changed = true;
            _selectedColor = selectedColorLocal;
        }
        global::Doroti.Ui.Color unselectedColorLocal = widget.unselectedColor ?? CupertinoTheme.of(context).primaryContrastingColor;
        if (!Equals(_unselectedColor, unselectedColorLocal))
        {
            changed = true;
            _unselectedColor = unselectedColorLocal;
        }
        global::Doroti.Ui.Color selectedDisabledColor = widget.disabledColor ?? selectedColorLocal.withOpacity(0.5);
        global::Doroti.Ui.Color unselectedDisabledColor = widget.disabledColor ?? unselectedColorLocal;
        if ((!Equals(_selectedDisabledColor, selectedDisabledColor)) || (!Equals(_unselectedDisabledColor, unselectedDisabledColor)))
        {
            changed = true;
            _selectedDisabledColor = selectedDisabledColor;
            _unselectedDisabledColor = unselectedDisabledColor;
        }
        global::Doroti.Ui.Color borderColorLocal = widget.borderColor ?? CupertinoTheme.of(context).primaryColor;
        if (!Equals(_borderColor, borderColorLocal))
        {
            changed = true;
            _borderColor = borderColorLocal;
        }
        global::Doroti.Ui.Color pressedColorLocal = widget.pressedColor ?? CupertinoTheme.of(context).primaryColor.withOpacity(0.2);
        if (!Equals(_pressedColor, pressedColorLocal))
        {
            changed = true;
            _pressedColor = pressedColorLocal;
        }
        _forwardBackgroundColorTween = new global::Doroti.Framework.Animation.ColorTween(begin: _pressedColor, end: _selectedColor);
        _reverseBackgroundColorTween = new global::Doroti.Framework.Animation.ColorTween(begin: _unselectedColor, end: _selectedColor);
        _textColorTween = new global::Doroti.Framework.Animation.ColorTween(begin: _selectedColor, end: _unselectedColor);
        return changed;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _updateAnimationControllers()
    {
        DartRuntimePrimitives.Assert(() => mounted, () => (object?)"This should only be called after didUpdateDependencies");
        foreach (global::Doroti.Framework.Animation.AnimationController controller in _selectionControllers)
        {
            controller.dispose();
        }
        _selectionControllers.Clear();
        _childTweens.Clear();
        foreach (T key in widget.children.Keys)
        {
            global::Doroti.Framework.Animation.AnimationController animationController = createAnimationController();
            if (EqualityComparer<T>.Default.Equals(widget.groupValue, key))
            {
                _childTweens.Add(_reverseBackgroundColorTween);
                animationController.value = 1.0;
            }
            else
            {
                _childTweens.Add(_forwardBackgroundColorTween);
            }
            _selectionControllers.Add(animationController);
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
        if (_updateColors() || (checked(oldWidget.children.Count) != checked((long)widget.children.Count)))
        {
            _updateAnimationControllers();
        }
        if (!EqualityComparer<T>.Default.Equals(oldWidget.groupValue, widget.groupValue))
        {
            var index = 0L;
            foreach (T key in widget.children.Keys)
            {
                if (EqualityComparer<T>.Default.Equals(widget.groupValue, key))
                {
                    _childTweens[(int)index] = _forwardBackgroundColorTween;
                    _selectionControllers[(int)index].forward();
                }
                else
                {
                    _childTweens[(int)index] = _reverseBackgroundColorTween;
                    _selectionControllers[(int)index].reverse();
                }
                index += 1L;
            }
        }
    }

    public override void dispose()
    {
        foreach (global::Doroti.Framework.Animation.AnimationController animationController in _selectionControllers)
        {
            animationController.dispose();
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if (_tickers is not null)
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in _tickers!)
                    {
                        if (ticker.isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual void _onTapDown(T currentKey)
    {
        if ((_pressedKey is null) && !EqualityComparer<T>.Default.Equals(currentKey, widget.groupValue))
        {
            setState(() =>
            {
                _pressedKey = currentKey;
            });
        }
    }

    internal virtual void _onTapCancel()
    {
        setState(() =>
        {
            _pressedKey = default(T);
        });
    }

    internal virtual void _onTap(T currentKey)
    {
        if (!EqualityComparer<T>.Default.Equals(currentKey, _pressedKey))
        {
            return;
        }
        if (!widget.disabledChildren.Contains(currentKey))
        {
            DartRuntimePrimitives.NullAware(_segmentKeys.GetValueOrDefault(currentKey), __target => __target.currentState)?.requestFocus();
            if (!EqualityComparer<T>.Default.Equals(currentKey, widget.groupValue))
            {
                widget.onValueChanged(currentKey);
            }
        }
        setState(() =>
        {
            _pressedKey = default(T);
        });
    }

    public virtual global::Doroti.Ui.Color? getTextColor(long index, T currentKey)
    {
        if (widget.disabledChildren.Contains(currentKey))
        {
            return _disabledTextColor;
        }
        if (_selectionControllers[(int)index].isAnimating)
        {
            return _textColorTween.evaluate(_selectionControllers[(int)index]);
        }
        if (EqualityComparer<T>.Default.Equals(widget.groupValue, currentKey))
        {
            return _unselectedColor;
        }
        return _selectedColor;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Color? getBackgroundColor(long index, T currentKey)
    {
        if (widget.disabledChildren.Contains(currentKey))
        {
            return EqualityComparer<T>.Default.Equals(widget.groupValue, currentKey) ? _selectedDisabledColor : _unselectedDisabledColor;
        }
        if (_selectionControllers[(int)index].isAnimating)
        {
            return _childTweens[(int)index].evaluate(_selectionControllers[(int)index]);
        }
        if (EqualityComparer<T>.Default.Equals(widget.groupValue, currentKey))
        {
            return _selectedColor;
        }
        if (EqualityComparer<T>.Default.Equals(_pressedKey, currentKey))
        {
            return _pressedColor;
        }
        return _unselectedColor;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        var gestureChildren = new List<global::Doroti.Framework.Widgets.Widget>();
        var backgroundColorsLocal = new List<global::Doroti.Ui.Color>();
        var index = 0L;
        long? selectedIndexLocal = default!;
        long? pressedIndexLocal = default!;
        foreach (T currentKey in widget.children.Keys)
        {
            selectedIndexLocal = EqualityComparer<T>.Default.Equals(widget.groupValue, currentKey) ? index : selectedIndexLocal;
            pressedIndexLocal = EqualityComparer<T>.Default.Equals(_pressedKey, currentKey) ? index : pressedIndexLocal;
            global::Doroti.Framework.Painting.TextStyle textStyle = DefaultTextStyle.of(context).style.copyWith(color: getTextColor(index, currentKey));
            var iconTheme = new global::Doroti.Framework.Widgets.IconThemeData(color: getTextColor(index, currentKey));
            global::Doroti.Framework.Widgets.Widget childLocal = new global::Doroti.Framework.Widgets.Center(child: widget.children.GetValueOrDefault(currentKey));
            bool isEnabled = !widget.disabledChildren.Contains(currentKey);
            global::Doroti.Framework.Widgets.GlobalKey<_SegmentButtonState__segmented_control<T>> segmentKey = _segmentKeys.putIfAbsent(currentKey, () => GlobalKey<_SegmentButtonState__segmented_control<T>>.Create());
            childLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _SegmentButton__segmented_control<T>(key: segmentKey, value: currentKey, enabled: isEnabled, child: new global::Doroti.Framework.Widgets.MouseRegion(cursor: Foundation.ConstantsLibrary.kIsWeb ? SystemMouseCursors.click : MouseCursor.defer, child: new global::Doroti.Framework.Widgets.GestureDetector(behavior: HitTestBehavior.opaque, onTapDown: isEnabled ? ((@event) =>
            {
                _onTapDown(currentKey);
            }) : null, onTapCancel: isEnabled ? _onTapCancel : null, onTap: () =>
            {
                if (isEnabled)
                {
                    DartRuntimePrimitives.NullAware(_segmentKeys.GetValueOrDefault(currentKey), __target => __target.currentState)?.requestFocus();
                }
                _onTap(currentKey);
            }, child: new global::Doroti.Framework.Widgets.IconTheme(data: iconTheme, child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: textStyle, child: new global::Doroti.Framework.Widgets.Semantics(button: true, inMutuallyExclusiveGroup: true, selected: EqualityComparer<T>.Default.Equals(widget.groupValue, currentKey), child: childLocal)))))));
            backgroundColorsLocal.Add(getBackgroundColor(index, currentKey)!);
            gestureChildren.Add(childLocal);
            index += 1L;
        }
        global::Doroti.Framework.Widgets.Widget box = new _SegmentedControlRenderWidget__segmented_control<T>(selectedIndex: selectedIndexLocal, pressedIndex: pressedIndexLocal, backgroundColors: backgroundColorsLocal, borderColor: _borderColor!, children: gestureChildren);
        return new global::Doroti.Framework.Widgets.Actions(actions: new DartMap<Type, dynamic> { [typeof(global::Doroti.Framework.Widgets.VoidCallbackIntent)] = new global::Doroti.Framework.Widgets.VoidCallbackAction() }, child: new global::Doroti.Framework.Widgets.RadioGroup<T>(groupValue: widget.groupValue, onChanged: (value) =>
        {
            if ((value is not null) && !widget.disabledChildren.Contains(value))
            {
                widget.onValueChanged(value);
            }
        }, child: new global::Doroti.Framework.Widgets.Padding(padding: widget.padding ?? Segmented_controlLibrary._kHorizontalItemPadding, child: new global::Doroti.Framework.Widgets.UnconstrainedBox(constrainedAxis: Axis.horizontal, child: box))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider(onTick, this, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
    __cascade.muted = !values.enabled;
    __cascade.forceFrames = values.forceFrames;
    return __cascade;
}))();
        _tickers!.Add(result);
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => _tickers is not null);
        DartRuntimePrimitives.Assert(() => _tickers!.Contains(ticker));
        _tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if (_tickers is not null)
        {
            TickerModeData values = _tickerModeNotifier!.value;
            bool mutedLocal = !values.enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", _tickers, description: (_tickers is not null) ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}" : null, defaultValue: default));
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
        return new _RenderSegmentedControl__segmented_control<T>(textDirection: Directionality.of(context), selectedIndex: selectedIndex, pressedIndex: pressedIndex, backgroundColors: backgroundColors, borderColor: borderColor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderSegmentedControl__segmented_control<T>)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderSegmentedControl__segmented_control<T>>)(() =>
{
    var __cascade = __renderObject;
    __cascade.textDirection = Directionality.of(context);
    __cascade.selectedIndex = selectedIndex;
    __cascade.pressedIndex = pressedIndex;
    __cascade.backgroundColors = backgroundColors;
    __cascade.borderColor = borderColor;
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
        _textDirection = textDirection;
        _selectedIndex = selectedIndex;
        _pressedIndex = pressedIndex;
        _backgroundColors = backgroundColors;
        _borderColor = borderColor;
    }

    public virtual long? selectedIndex
    {
        get => _selectedIndex;
        set
        {
            var __value = value;
            if (_selectedIndex == __value)
            {
                return;
            }
            _selectedIndex = __value;
            markNeedsPaint();
        }
    }
    public virtual long? pressedIndex
    {
        get => _pressedIndex;
        set
        {
            var __value = value;
            if (_pressedIndex == __value)
            {
                return;
            }
            _pressedIndex = __value;
            markNeedsPaint();
        }
    }
    public virtual global::Doroti.Ui.TextDirection textDirection
    {
        get => _textDirection;
        set
        {
            var __value = value;
            if (Equals(_textDirection, DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _textDirection = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual List<global::Doroti.Ui.Color> backgroundColors
    {
        get => _backgroundColors;
        set
        {
            var __value = value;
            if (Equals(_backgroundColors, __value))
            {
                return;
            }
            _backgroundColors = __value;
            markNeedsPaint();
        }
    }
    public virtual global::Doroti.Ui.Color borderColor
    {
        get => _borderColor;
        set
        {
            var __value = value;
            if (Equals(_borderColor, __value))
            {
                return;
            }
            _borderColor = __value;
            markNeedsPaint();
        }
    }
    public override double computeMinIntrinsicWidth(double height)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = firstChild;
        var minWidth = 0.0;
        while (child is not null)
        {
            var childParentData = ((_SegmentedControlContainerBoxParentData__segmented_control?)child.parentData!)!;
            double childWidth = child.getMinIntrinsicWidth(height);
            minWidth = Math.Max(minWidth, childWidth);
            child = childParentData.nextSibling;
        }
        return minWidth * childCount;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = firstChild;
        var maxWidth = 0.0;
        while (child is not null)
        {
            var childParentData = ((_SegmentedControlContainerBoxParentData__segmented_control?)child.parentData!)!;
            double childWidth = child.getMaxIntrinsicWidth(height);
            maxWidth = Math.Max(maxWidth, childWidth);
            child = childParentData.nextSibling;
        }
        return maxWidth * childCount;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = firstChild;
        var minHeight = 0.0;
        while (child is not null)
        {
            var childParentData = ((_SegmentedControlContainerBoxParentData__segmented_control?)child.parentData!)!;
            double childHeight = child.getMinIntrinsicHeight(width);
            minHeight = Math.Max(minHeight, childHeight);
            child = childParentData.nextSibling;
        }
        return minHeight;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = firstChild;
        var maxHeight = 0.0;
        while (child is not null)
        {
            var childParentData = ((_SegmentedControlContainerBoxParentData__segmented_control?)child.parentData!)!;
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
        var __child = (global::Doroti.Framework.Rendering.RenderBox)child;
        if (__child.parentData is not _SegmentedControlContainerBoxParentData__segmented_control)
        {
            __child.parentData = new _SegmentedControlContainerBoxParentData__segmented_control();
        }
    }

    internal virtual void _layoutRects(global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.RenderBox?> nextChild, global::Doroti.Framework.Rendering.RenderBox? leftChild, global::Doroti.Framework.Rendering.RenderBox? rightChild)
    {
        var child = leftChild;
        var start = 0.0;
        while (child is not null)
        {
            var childParentData = ((_SegmentedControlContainerBoxParentData__segmented_control?)child.parentData!)!;
            var childOffset = new global::Doroti.Ui.Offset(start, 0.0);
            childParentData.offset = childOffset;
            var childRect = Rect.fromLTWH(start, 0.0, child.size.width, child.size.height);
            global::Doroti.Ui.RSuperellipse rChildRect = default!;
            if (Equals(child, leftChild))
            {
                rChildRect = RSuperellipse.fromRectAndCorners(childRect, topLeft: Radius.circular(3.0), bottomLeft: Radius.circular(3.0));
            }
            else
            {
                if (Equals(child, rightChild))
                {
                    rChildRect = RSuperellipse.fromRectAndCorners(childRect, topRight: Radius.circular(3.0), bottomRight: Radius.circular(3.0));
                }
                else
                {
                    rChildRect = RSuperellipse.fromRectAndCorners(childRect);
                }
            }
            childParentData.surroundingRect = rChildRect;
            start += child.size.width;
            child = nextChild(child);
        }
    }

    internal virtual global::Doroti.Ui.Size _calculateChildSize(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        double maxHeight = Segmented_controlLibrary._kMinSegmentedControlHeight;
        double childWidth = constraints.minWidth / childCount;
        global::Doroti.Framework.Rendering.RenderBox? child = firstChild;
        while (child is not null)
        {
            childWidth = Math.Max(childWidth, child.getMaxIntrinsicWidth(double.PositiveInfinity));
            child = childAfter(child);
        }
        childWidth = Math.Min(childWidth, constraints.maxWidth / childCount);
        child = firstChild;
        while (child is not null)
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
        return constraints.constrain(new global::Doroti.Ui.Size(childSize.width * childCount, childSize.height));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Ui.Size childSize = _calculateChildSize(constraints);
        var childConstraints = BoxConstraints.CreateTight(childSize);
        global::Doroti.Framework.Rendering.BaselineOffset baselineOffset = BaselineOffset.noBaseline;
        for (global::Doroti.Framework.Rendering.RenderBox? child = firstChild; child is not null; child = childAfter(child))
        {
            baselineOffset = baselineOffset.minOf(new global::Doroti.Framework.Rendering.BaselineOffset(child.getDryBaseline(childConstraints, baseline)));
        }
        return baselineOffset.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        global::Doroti.Ui.Size childSize = _calculateChildSize(constraints);
        return _computeOverallSizeFromChildSize(childSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        global::Doroti.Framework.Rendering.BoxConstraints constraintsLocal = constraints;
        global::Doroti.Ui.Size childSize = _calculateChildSize(constraintsLocal);
        var childConstraints = BoxConstraints.CreateTightFor(width: childSize.width, height: childSize.height);
        global::Doroti.Framework.Rendering.RenderBox? child = firstChild;
        while (child is not null)
        {
            child.layout(childConstraints, parentUsesSize: true);
            child = childAfter(child);
        }
        switch (textDirection)
        {
            case TextDirection.rtl:
                {
                    _layoutRects(childBefore, lastChild, firstChild);
                    break;
                }
            case TextDirection.ltr:
                {
                    _layoutRects(childAfter, firstChild, lastChild);
                    break;
                }
        }
        size = _computeOverallSizeFromChildSize(childSize);
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = firstChild;
        var index = 0L;
        while (child is not null)
        {
            _paintChild(context, offset, child, index);
            child = childAfter(child);
            index += 1L;
        }
    }

    internal virtual void _paintChild(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset, global::Doroti.Framework.Rendering.RenderBox child, long childIndex)
    {
        var childParentData = ((_SegmentedControlContainerBoxParentData__segmented_control?)child.parentData!)!;
        context.canvas.drawRSuperellipse(childParentData.surroundingRect!.shift(offset), ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = backgroundColors[(int)childIndex];
    __cascade.style = PaintingStyle.fill;
    return __cascade;
}))());
        context.canvas.drawRSuperellipse(childParentData.surroundingRect!.shift(offset), ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = borderColor;
    __cascade.strokeWidth = 1.0;
    __cascade.style = PaintingStyle.stroke;
    return __cascade;
}))());
        context.paintChild(child, childParentData.offset + offset);
    }

    public override bool hitTestChildren(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = lastChild;
        while (child is not null)
        {
            var childParentData = ((_SegmentedControlContainerBoxParentData__segmented_control?)child.parentData!)!;
            if (childParentData.surroundingRect!.outerRect.contains(position))
            {
                return result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: (result, localOffset) =>
                {
                    DartRuntimePrimitives.Assert(() => Equals(localOffset, position - childParentData.offset));
                    return child!.hitTest(result, position: localOffset);
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            }
            child = childParentData.previousSibling;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
        while (childParentData.previousSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.previousSibling, child));
            child = childParentData.previousSibling!;
            childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
        }
        return Equals(child, equals);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
        while (childParentData.nextSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.nextSibling, child));
            child = childParentData.nextSibling!;
            childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
        }
        return Equals(child, equals);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long childCount => _childCount;
    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (child is not RenderBox)
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"A {GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new global::Doroti.Framework.Foundation.ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new global::Doroti.Framework.Foundation.ErrorSpacer(), new global::Doroti.Framework.Foundation.DiagnosticsProperty<object?>($"The {GetType()} that expected a {typeof(RenderBox)} child was created by", debugCreator, style: DiagnosticsTreeStyle.errorProperty), new global::Doroti.Framework.Foundation.ErrorSpacer(), new global::Doroti.Framework.Foundation.DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", child.debugCreator, style: DiagnosticsTreeStyle.errorProperty) }));
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _insertIntoChildList(RenderBox child, RenderBox? after = null)
    {
        var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => childParentData.nextSibling is null);
        DartRuntimePrimitives.Assert(() => childParentData.previousSibling is null);
        _childCount += 1L;
        DartRuntimePrimitives.Assert(() => _childCount > 0L);
        if (after is null)
        {
            childParentData.nextSibling = _firstChild;
            if (_firstChild is not null)
            {
                var firstChildParentData = ((ContainerBoxParentData<RenderBox>?)_firstChild!.parentData!)!;
                firstChildParentData.previousSibling = child;
            }
            _firstChild = child;
            _lastChild ??= child;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => _firstChild is not null);
            DartRuntimePrimitives.Assert(() => _lastChild is not null);
            DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(after, equals: _firstChild));
            DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(after, equals: _lastChild));
            var afterParentData = ((ContainerBoxParentData<RenderBox>?)after.parentData!)!;
            if (afterParentData.nextSibling is null)
            {
                DartRuntimePrimitives.Assert(() => Equals(after, _lastChild));
                childParentData.previousSibling = after;
                afterParentData.nextSibling = child;
                _lastChild = child;
            }
            else
            {
                childParentData.nextSibling = afterParentData.nextSibling;
                childParentData.previousSibling = after;
                var childPreviousSiblingParentData = ((ContainerBoxParentData<RenderBox>?)childParentData.previousSibling!.parentData!)!;
                var childNextSiblingParentData = ((ContainerBoxParentData<RenderBox>?)childParentData.nextSibling!.parentData!)!;
                childPreviousSiblingParentData.nextSibling = child;
                childNextSiblingParentData.previousSibling = child;
                DartRuntimePrimitives.Assert(() => Equals(afterParentData.nextSibling, child));
            }
        }
    }

    public virtual void insert(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => !Equals(child, this), () => (object?)"A RenderObject cannot be inserted into itself.");
        DartRuntimePrimitives.Assert(() => !Equals(after, this), () => (object?)"A RenderObject cannot simultaneously be both the parent and the sibling of another RenderObject.");
        DartRuntimePrimitives.Assert(() => !Equals(child, after), () => (object?)"A RenderObject cannot be inserted after itself.");
        DartRuntimePrimitives.Assert(() => !Equals(child, _firstChild));
        DartRuntimePrimitives.Assert(() => !Equals(child, _lastChild));
        adoptChild(child);
        DartRuntimePrimitives.Assert(() => child.parentData is ContainerBoxParentData<RenderBox>, () => (object?)$"A child of {GetType()} has parentData of type {DartRuntimePrimitives.RuntimeType(child.parentData)}, " + $"which does not conform to {typeof(ContainerBoxParentData<RenderBox>)}. Class using ContainerRenderObjectMixin " + $"should override setupParentData() to set parentData to type {typeof(ContainerBoxParentData<RenderBox>)}.");
        _insertIntoChildList(child, after: after);
    }

    public virtual void add(RenderBox child)
    {
        insert(child, after: _lastChild);
    }

    public virtual void addAll(List<RenderBox>? children)
    {
        children?.forEach((__arg0) => ((global::System.Action<RenderBox>)add)(__arg0));
    }

    public virtual void _removeFromChildList(RenderBox child)
    {
        var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(child, equals: _firstChild));
        DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(child, equals: _lastChild));
        DartRuntimePrimitives.Assert(() => _childCount >= 0L);
        if (childParentData.previousSibling is null)
        {
            DartRuntimePrimitives.Assert(() => Equals(_firstChild, child));
            _firstChild = childParentData.nextSibling;
        }
        else
        {
            var childPreviousSiblingParentData = ((ContainerBoxParentData<RenderBox>?)childParentData.previousSibling!.parentData!)!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if (childParentData.nextSibling is null)
        {
            DartRuntimePrimitives.Assert(() => Equals(_lastChild, child));
            _lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = ((ContainerBoxParentData<RenderBox>?)childParentData.nextSibling!.parentData!)!;
            childNextSiblingParentData.previousSibling = childParentData.previousSibling;
        }
        childParentData.previousSibling = null;
        childParentData.nextSibling = null;
        _childCount -= 1L;
    }

    public virtual void remove(RenderBox child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
            RenderBox? next = childParentData.nextSibling;
            childParentData.previousSibling = null;
            childParentData.nextSibling = null;
            dropChild(child);
            child = next;
        }
        _firstChild = null;
        _lastChild = null;
        _childCount = 0L;
    }

    public virtual void move(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => !Equals(child, this));
        DartRuntimePrimitives.Assert(() => !Equals(after, this));
        DartRuntimePrimitives.Assert(() => !Equals(child, after));
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
        if (Equals(childParentData.previousSibling, after))
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
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            child.attach(owner);
            var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void detach()
    {
        base.detach();
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            child.detach();
            var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void redepthChildren()
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            redepthChild(child);
            var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildren(global::System.Action<RenderObject> visitor)
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            visitor(child);
            var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => _firstChild;
    public virtual RenderBox? lastChild => _lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
        return childParentData.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var children = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        if (firstChild is not null)
        {
            RenderBox child = firstChild!;
            var count = 1L;
            while (true)
            {
                children.Add(((Diagnosticable)child).toDiagnosticsNode(name: $"child__183606 {count}"));
                if (Equals(child, lastChild))
                {
                    break;
                }
                count += 1L;
                var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
                child = childParentData.nextSibling!;
            }
        }
        return children;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToFirstActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
            double? result = child.getDistanceToActualBaseline(baseline);
            if (result is not null)
            {
                double result__138852__value138916 = DartRuntimePrimitives.RequireValue(result);
                return DartRuntimePrimitives.RequireValue(result__138852__value138916) + childParentData.offset.dy;
            }
            child = childParentData.nextSibling;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToHighestActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        BaselineOffset minBaseline = BaselineOffset.noBaseline;
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
            BaselineOffset candidate = new BaselineOffset(child.getDistanceToActualBaseline(baseline)).op_Add(childParentData.offset.dy);
            minBaseline = minBaseline.minOf(candidate);
            child = childParentData.nextSibling;
        }
        return minBaseline.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool defaultHitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child = lastChild;
        while (child is not null)
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
            bool isHit = result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: (result, transformed) =>
            {
                DartRuntimePrimitives.Assert(() => Equals(transformed, position - childParentData.offset));
                return child!.hitTest(result, position: transformed);
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
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
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
            context.paintChild(child, childParentData.offset + offset);
            child = childParentData.nextSibling;
        }
    }

    public virtual List<RenderBox> getChildrenAsList()
    {
        var result = new List<RenderBox>();
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
            result.Add(child!);
            child = childParentData.nextSibling;
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
