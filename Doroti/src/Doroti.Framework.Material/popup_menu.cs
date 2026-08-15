// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/popup_menu.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public static partial class Popup_menuLibrary
{
    internal static Duration _kMenuDuration = Duration.Create(milliseconds: 300L);
}

public static partial class Popup_menuLibrary
{
    internal static double _kMenuCloseIntervalEnd = (2.0 / 3.0);
}

public static partial class Popup_menuLibrary
{
    internal static double _kMenuDividerHeight = 16.0;
}

public static partial class Popup_menuLibrary
{
    internal static double _kMenuMaxWidth = (5.0 * Popup_menuLibrary._kMenuWidthStep);
}

public static partial class Popup_menuLibrary
{
    internal static double _kMenuMinWidth = (2.0 * Popup_menuLibrary._kMenuWidthStep);
}

public static partial class Popup_menuLibrary
{
    internal static double _kMenuWidthStep = 56.0;
}

public static partial class Popup_menuLibrary
{
    internal static double _kMenuScreenPadding = 8.0;
}

public abstract class PopupMenuEntry<T> : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    protected PopupMenuEntry(global::Doroti.Generated.Framework.Foundation.Key? key = null) : base(key: key)
    {
    }

    public abstract double height { get; }
    public abstract bool represents(T? value);
}

public class PopupMenuDivider : PopupMenuEntry<dynamic>
{
    private double __field_height = default!;
    public override double height { get => __field_height; }
    public virtual double? thickness { get; private set; }
    public virtual double? indent { get; private set; }
    public virtual double? endIndent { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry? radius { get; private set; }
    public virtual Color? color { get; private set; }

    public PopupMenuDivider(global::Doroti.Generated.Framework.Foundation.Key? key = null, double? height = null, double? thickness = null, double? indent = null, double? endIndent = null, global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry? radius = null, Color? color = null) : base(key: key)
    {
        double __height = height ?? Popup_menuLibrary._kMenuDividerHeight;
        this.__field_height = __height;
        this.thickness = thickness;
        this.indent = indent;
        this.endIndent = endIndent;
        this.radius = radius;
        this.color = color;
    }

    public override bool represents(dynamic value) => false;
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _PopupMenuDividerState__popup_menu());
}

internal class _PopupMenuDividerState__popup_menu : global::Doroti.Generated.Framework.Widgets.State<PopupMenuDivider>
{
    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new Divider(height: ((PopupMenuDivider)(object)this.widget).height, thickness: ((PopupMenuDivider)(object)this.widget).thickness, indent: ((PopupMenuDivider)(object)this.widget).indent, color: ((PopupMenuDivider)(object)this.widget).color, endIndent: ((PopupMenuDivider)(object)this.widget).endIndent, radius: ((PopupMenuDivider)(object)this.widget).radius));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MenuItem__popup_menu : global::Doroti.Generated.Framework.Widgets.SingleChildRenderObjectWidget
{
    public virtual global::System.Action<Size> onLayout { get; private set; } = default!;

    internal _MenuItem__popup_menu(global::System.Action<Size> onLayout, global::Doroti.Generated.Framework.Widgets.Widget? child) : base(child: child)
    {
        this.onLayout = onLayout;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _RenderMenuItem__popup_menu((global::System.Action<Size>)this.onLayout));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderMenuItem__popup_menu)(object)renderObject;
        __renderObject.onLayout = (global::System.Action<Size>)this.onLayout;
    }

}

public class _RenderMenuItem__popup_menu : global::Doroti.Generated.Framework.Rendering.RenderShiftedBox
{
    public virtual global::System.Action<Size> onLayout { get; set; } = default!;

    internal _RenderMenuItem__popup_menu(global::System.Action<Size> onLayout, global::Doroti.Generated.Framework.Rendering.RenderBox? child = null) : base(child)
    {
        this.onLayout = onLayout;
    }

    public override Size computeDryLayout(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
        return (this.child?.getDryLayout(constraints) ?? Size.zero);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        return this.child?.getDryBaseline(constraints, baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        if ((this.child is null))
        {
            size = Size.zero;
        }
        else
        {
            this.child!.layout(this.constraints, parentUsesSize: true);
            size = this.constraints.constrain(this.child!.size);
            var childParentData__7444 = ((global::Doroti.Generated.Framework.Rendering.BoxParentData?)(object?)this.child!.parentData!)!;
            childParentData__7444.offset = Offset.zero;
        }
        this.onLayout(this.size);
    }

}

public class PopupMenuItem<T> : PopupMenuEntry<T>
{
    public virtual T? value { get; private set; }
    public virtual global::System.Action? onTap { get; private set; }
    public virtual bool enabled { get; private set; } = default!;
    private double __field_height = default!;
    public override double height { get => __field_height; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsets? padding { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? textStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?>? labelTextStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? child { get; private set; }

    public PopupMenuItem(global::Doroti.Generated.Framework.Foundation.Key? key = null, T? value = default, global::System.Action? onTap = null, bool enabled = true, double? height = null, global::Doroti.Generated.Framework.Painting.EdgeInsets? padding = null, global::Doroti.Generated.Framework.Painting.TextStyle? textStyle = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?>? labelTextStyle = null, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null, global::Doroti.Generated.Framework.Widgets.Widget? child = default!) : base(key: key)
    {
        double __height = height ?? ConstantsLibrary.kMinInteractiveDimension;
        this.value = value;
        this.onTap = onTap;
        this.enabled = enabled;
        this.__field_height = __height;
        this.padding = padding;
        this.textStyle = textStyle;
        this.labelTextStyle = labelTextStyle;
        this.mouseCursor = mouseCursor;
        this.child = child;
    }

    public override bool represents(T? value) => DartRuntimePrimitives.ConvertValue<bool>(EqualityComparer<T>.Default.Equals(value, this.value));
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new PopupMenuItemState<T, PopupMenuItem<T>>());
}

public class PopupMenuItemState<T, W> : global::Doroti.Generated.Framework.Widgets.State<W> where W : PopupMenuItem<T>
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? buildChild() => ((PopupMenuItem<T>)(object)this.widget).child;
    public virtual void handleTap()
    {
        Navigator.pop<T>(this.context, ((PopupMenuItem<T>)(object)this.widget).value);
        ((PopupMenuItem<T>)(object)this.widget).onTap?.Invoke();
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ThemeData theme__14174 = Theme.of(context);
        PopupMenuThemeData popupMenuTheme__14230 = PopupMenuTheme.of(context);
        PopupMenuThemeData defaults__14304 = (theme__14174.useMaterial3 ? new _PopupMenuDefaultsM3__popup_menu(context) : new _PopupMenuDefaultsM2__popup_menu(context));
        var states__14425 = ((Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>>)(() => { var __collection14434 = new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>(); if (!((PopupMenuItem<T>)(object)this.widget).enabled) { __collection14434.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled); } return __collection14434; }))();
        global::Doroti.Generated.Framework.Painting.TextStyle style__14507 = (theme__14174.useMaterial3 ? (((((PopupMenuItem<T>)(object)this.widget).labelTextStyle?.resolve(states__14425) ?? popupMenuTheme__14230.labelTextStyle?.resolve(states__14425)!) ?? defaults__14304.labelTextStyle!.resolve(states__14425)!)) : (((((PopupMenuItem<T>)(object)this.widget).textStyle ?? popupMenuTheme__14230.textStyle) ?? defaults__14304.textStyle!)));
        if ((!((PopupMenuItem<T>)(object)this.widget).enabled && !theme__14174.useMaterial3))
        {
            style__14507 = style__14507.copyWith(color: theme__14174.disabledColor);
        }
        global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding__14934 = ((global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry)(object?)(((PopupMenuItem<T>)(object)this.widget).padding ?? ((theme__14174.useMaterial3 ? _PopupMenuDefaultsM3__popup_menu.menuItemPadding : _PopupMenuDefaultsM2__popup_menu.menuItemPadding))));
        global::Doroti.Generated.Framework.Widgets.Widget item__15114 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.AnimatedDefaultTextStyle(style: style__14507, duration: ConstantsLibrary.kThemeChangeDuration, child: new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Generated.Framework.Rendering.BoxConstraints(minHeight: ((PopupMenuItem<T>)(object)this.widget).height), child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: padding__14934, child: new global::Doroti.Generated.Framework.Widgets.Align(alignment: global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerStart, child: buildChild())))));
        if (!((PopupMenuItem<T>)(object)this.widget).enabled)
        {
            var isDark__15506 = (object.Equals(theme__14174.brightness, Brightness.dark));
            item__15114 = IconTheme.merge(data: new global::Doroti.Generated.Framework.Widgets.IconThemeData(opacity: (isDark__15506 ? 0.5 : 0.38)), child: item__15114);
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.MergeSemantics(child: buildSemantics(child: new InkWell(onTap: ((global::System.Action)(((PopupMenuItem<T>)(object)this.widget).enabled ? this.handleTap : null)), canRequestFocus: ((PopupMenuItem<T>)(object)this.widget).enabled, mouseCursor: new _EffectiveMouseCursor__popup_menu(((PopupMenuItem<T>)(object)this.widget).mouseCursor, popupMenuTheme__14230.mouseCursor), child: ListTileTheme.merge(contentPadding: global::Doroti.Generated.Framework.Painting.EdgeInsets.zero, titleTextStyle: style__14507, child: item__15114)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Widgets.Widget buildSemantics(global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Semantics(role: SemanticsRole.menuItem, enabled: ((PopupMenuItem<T>)(object)this.widget).enabled, button: true, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CheckedPopupMenuItem<T> : PopupMenuItem<T>
{
    public virtual bool @checked { get; private set; } = default!;

    public CheckedPopupMenuItem(global::Doroti.Generated.Framework.Foundation.Key? key = null, T? value = default, bool @checked = false, bool enabled = true, global::Doroti.Generated.Framework.Painting.EdgeInsets? padding = null, double? height = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?>? labelTextStyle = null, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null, global::Doroti.Generated.Framework.Widgets.Widget? child = null, global::System.Action? onTap = null) : base(key: key, value: value, enabled: enabled, padding: padding, height: height ?? ConstantsLibrary.kMinInteractiveDimension, labelTextStyle: labelTextStyle, mouseCursor: mouseCursor, child: child, onTap: onTap)
    {
        this.@checked = @checked;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget? child => ((global::Doroti.Generated.Framework.Widgets.Widget?)base.child);
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CheckedPopupMenuItemState__popup_menu<T>());
}

internal class _CheckedPopupMenuItemState__popup_menu<T> : PopupMenuItemState<T, CheckedPopupMenuItem<T>>, global::Doroti.Generated.Framework.Widgets.SingleTickerProviderStateMixin<CheckedPopupMenuItem<T>>
{
    internal static Duration _fadeDuration = Duration.Create(milliseconds: 150L);
    internal virtual global::Doroti.Generated.Framework.Animation.AnimationController _controller { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual global::Doroti.Generated.Framework.Animation.Animation<double> _opacity => ((global::Doroti.Generated.Framework.Animation.AnimationController)this._controller).view;
    public override void initState()
    {
        base.initState();
        _controller = ((Func<global::Doroti.Generated.Framework.Animation.AnimationController>)(() =>
{            var __cascade = new global::Doroti.Generated.Framework.Animation.AnimationController(duration: _fadeDuration, vsync: this);
            __cascade.value = (((CheckedPopupMenuItem<T>)(object)this.widget).@checked ? 1.0 : 0.0);
            __cascade.addListener(((global::System.Action)(() => { setState(((global::System.Action)(() => {
}))); })));
            return __cascade;        }))();
    }

    public override void dispose()
    {
        this._controller.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if (((this._ticker is null) || !this._ticker!.isActive))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), this._ticker!.describeForError("The offending ticker was") }));
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override void handleTap()
    {
        if (((CheckedPopupMenuItem<T>)(object)this.widget).@checked)
        {
            this._controller.reverse();
        }
        else
        {
            this._controller.forward();
        }
        base.handleTap();
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget buildSemantics(global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Semantics(role: SemanticsRole.menuItemCheckbox, enabled: ((PopupMenuItem<T>)(object)this.widget).enabled, @checked: ((CheckedPopupMenuItem<T>)(object)this.widget).@checked, button: true, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget? buildChild()
    {
        ThemeData theme__21841 = Theme.of(this.context);
        PopupMenuThemeData popupMenuTheme__21897 = PopupMenuTheme.of(this.context);
        PopupMenuThemeData defaults__21971 = (theme__21841.useMaterial3 ? new _PopupMenuDefaultsM3__popup_menu(this.context) : new _PopupMenuDefaultsM2__popup_menu(this.context));
        var states__22092 = ((Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>>)(() => { var __collection22101 = new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>(); if (((CheckedPopupMenuItem<T>)(object)this.widget).@checked) { __collection22101.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.selected); } return __collection22101; }))();
        global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?>? effectiveLabelTextStyle__22201 = ((((PopupMenuItem<T>)(object)this.widget).labelTextStyle ?? popupMenuTheme__21897.labelTextStyle) ?? defaults__21971.labelTextStyle);
        return ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)new global::Doroti.Generated.Framework.Widgets.IgnorePointer(child: ListTileTheme.merge(contentPadding: global::Doroti.Generated.Framework.Painting.EdgeInsets.zero, child: new ListTile(enabled: ((PopupMenuItem<T>)(object)this.widget).enabled, titleTextStyle: effectiveLabelTextStyle__22201?.resolve(states__22092), leading: new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: this._opacity, child: new global::Doroti.Generated.Framework.Widgets.Icon((this._controller.isDismissed ? null : Icons.done))), title: ((CheckedPopupMenuItem<T>)(object)this.widget).child))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._ticker is null))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this.GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
            });
        this._ticker = new global::Doroti.Generated.Framework.Scheduler.Ticker((global::System.Action<Duration>)onTick, debugLabel: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
        _updateTickerModeNotifier();
        _updateTicker();
        return this._ticker!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTicker();
    }

    public virtual void _updateTicker()
    {
        TickerModeData values__15157 = this._tickerModeNotifier!.value;
        if ((this._ticker is not null))
        {
            this._ticker!.muted = !((TickerModeData)values__15157).enabled;
            this._ticker!.forceFrames = ((TickerModeData)values__15157).forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__15400 = ((global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__15400, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        newNotifier__15400.addListener(() => this._updateTicker());
        this._tickerModeNotifier = newNotifier__15400;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription__15805 = ((this._ticker?.isActive, this._ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) });
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Scheduler.Ticker>("ticker", this._ticker, description: tickerDescription__15805, showSeparator: false, defaultValue: default));
    }

}

public class _PopupMenu__popup_menu<T> : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual List<global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>> itemKeys { get; private set; } = default!;
    public virtual _PopupMenuRoute__popup_menu<T> route { get; private set; } = default!;
    public virtual string? semanticLabel { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;

    internal _PopupMenu__popup_menu(global::Doroti.Generated.Framework.Foundation.Key? key = null, List<global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>> itemKeys = default!, _PopupMenuRoute__popup_menu<T> route = default!, string? semanticLabel = default!, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, Clip clipBehavior = default!) : base(key: key)
    {
        this.itemKeys = itemKeys;
        this.route = route;
        this.semanticLabel = semanticLabel;
        this.constraints = constraints;
        this.clipBehavior = clipBehavior;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _PopupMenuState__popup_menu<T>());
}

internal class _PopupMenuState__popup_menu<T> : global::Doroti.Generated.Framework.Widgets.State<_PopupMenu__popup_menu<T>>
{
    internal virtual List<global::Doroti.Generated.Framework.Animation.CurvedAnimation> _opacities { get; set; } = new List<global::Doroti.Generated.Framework.Animation.CurvedAnimation>();

    public override void initState()
    {
        base.initState();
        _setOpacities();
    }

    public override void didUpdateWidget(_PopupMenu__popup_menu<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (((checked((long)(((_PopupMenu__popup_menu<T>)oldWidget).route.items.Count)) != checked((long)(((_PopupMenu__popup_menu<T>)(object)this.widget).route.items.Count))) || (!object.Equals(((_PopupMenu__popup_menu<T>)oldWidget).route.animation, ((_PopupMenu__popup_menu<T>)(object)this.widget).route.animation))))
        {
            _setOpacities();
        }
    }

    internal virtual void _setOpacities()
    {
        foreach (global::Doroti.Generated.Framework.Animation.CurvedAnimation opacity__23765 in this._opacities)
        {
            opacity__23765.dispose();
        }
        var newOpacities__23831 = new List<global::Doroti.Generated.Framework.Animation.CurvedAnimation>();
        double unit__23884 = (1.0 / ((checked((long)(((_PopupMenu__popup_menu<T>)(object)this.widget).route.items.Count)) + 1.5)));
        for (var i__24016 = 0L; (i__24016 < checked((long)(((_PopupMenu__popup_menu<T>)(object)this.widget).route.items.Count))); i__24016 += 1L)
        {
            double start__24083 = (((i__24016 + 1L)) * unit__23884);
            double end__24126 = Dart_uiLibrary.clampDouble((start__24083 + (1.5 * unit__23884)), 0.0, 1.0);
            var opacity__24187 = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: ((_PopupMenu__popup_menu<T>)(object)this.widget).route.animation!, curve: new global::Doroti.Generated.Framework.Animation.Interval(start__24083, end__24126));
            newOpacities__23831.Add(opacity__24187);
        }
        _opacities = newOpacities__23831;
    }

    public override void dispose()
    {
        foreach (global::Doroti.Generated.Framework.Animation.CurvedAnimation opacity__24413 in this._opacities)
        {
            opacity__24413.dispose();
        }
        base.dispose();
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        double unit__24563 = (1.0 / ((checked((long)(((_PopupMenu__popup_menu<T>)(object)this.widget).route.items.Count)) + 1.5)));
        var children__24692 = new List<global::Doroti.Generated.Framework.Widgets.Widget>();
        ThemeData theme__24735 = Theme.of(context);
        PopupMenuThemeData popupMenuTheme__24791 = PopupMenuTheme.of(context);
        PopupMenuThemeData defaults__24865 = (theme__24735.useMaterial3 ? new _PopupMenuDefaultsM3__popup_menu(context) : new _PopupMenuDefaultsM2__popup_menu(context));
        for (var i__24990 = 0L; (i__24990 < checked((long)(((_PopupMenu__popup_menu<T>)(object)this.widget).route.items.Count))); i__24990 += 1L)
        {
            global::Doroti.Generated.Framework.Animation.CurvedAnimation opacity__25066 = this._opacities[(int)(i__24990)];
            global::Doroti.Generated.Framework.Widgets.Widget item__25104 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)((_PopupMenu__popup_menu<T>)(object)this.widget).route.items[(int)(i__24990)]);
            if (((((_PopupMenu__popup_menu<T>)(object)this.widget).route.initialValue is not null) && ((_PopupMenu__popup_menu<T>)(object)this.widget).route.items[(int)(i__24990)].represents(((_PopupMenu__popup_menu<T>)(object)this.widget).route.initialValue)))
            {
                item__25104 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.ColoredBox(color: Theme.of(context).highlightColor, child: item__25104));
            }
            children__24692.Add(new _MenuItem__popup_menu(onLayout: ((global::System.Action<Size>)((size) => {
((_PopupMenu__popup_menu<T>)(object)this.widget).route.itemSizes[(int)(i__24990)] = size;
})), child: new global::Doroti.Generated.Framework.Widgets.FadeTransition(key: ((_PopupMenu__popup_menu<T>)(object)this.widget).itemKeys[(int)(i__24990)], opacity: opacity__25066, child: item__25104)));
        }
        var opacity__25601 = new global::Doroti.Generated.Framework.Animation.CurveTween(curve: new global::Doroti.Generated.Framework.Animation.Interval(0.0, (1.0 / 3.0)));
        var width__25672 = new global::Doroti.Generated.Framework.Animation.CurveTween(curve: new global::Doroti.Generated.Framework.Animation.Interval(0.0, unit__24563));
        var height__25730 = new global::Doroti.Generated.Framework.Animation.CurveTween(curve: new global::Doroti.Generated.Framework.Animation.Interval(0.0, (unit__24563 * checked((long)(((_PopupMenu__popup_menu<T>)(object)this.widget).route.items.Count)))));
        global::Doroti.Generated.Framework.Widgets.Widget child__25825 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: (((_PopupMenu__popup_menu<T>)(object)this.widget).constraints ?? new global::Doroti.Generated.Framework.Rendering.BoxConstraints(minWidth: Popup_menuLibrary._kMenuMinWidth, maxWidth: Popup_menuLibrary._kMenuMaxWidth)), child: new global::Doroti.Generated.Framework.Widgets.IntrinsicWidth(stepWidth: Popup_menuLibrary._kMenuWidthStep, child: new global::Doroti.Generated.Framework.Widgets.Semantics(role: SemanticsRole.menu, scopesRoute: true, namesRoute: true, explicitChildNodes: true, label: ((_PopupMenu__popup_menu<T>)(object)this.widget).semanticLabel, child: new global::Doroti.Generated.Framework.Widgets.SingleChildScrollView(padding: ((((_PopupMenu__popup_menu<T>)(object)this.widget).route.menuPadding ?? popupMenuTheme__24791.menuPadding) ?? defaults__24865.menuPadding), child: new global::Doroti.Generated.Framework.Widgets.ListBody(children: children__24692))))));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.AnimatedBuilder(animation: ((_PopupMenu__popup_menu<T>)(object)this.widget).route.animation!, builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, child) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: opacity__25601.animate(((_PopupMenu__popup_menu<T>)(object)this.widget).route.animation!), child: new Material(shape: ((((_PopupMenu__popup_menu<T>)(object)this.widget).route.shape ?? popupMenuTheme__24791.shape) ?? defaults__24865.shape), color: ((((_PopupMenu__popup_menu<T>)(object)this.widget).route.color ?? popupMenuTheme__24791.color) ?? defaults__24865.color), clipBehavior: ((_PopupMenu__popup_menu<T>)(object)this.widget).clipBehavior, type: MaterialType.card, elevation: ((((_PopupMenu__popup_menu<T>)(object)this.widget).route.elevation ?? popupMenuTheme__24791.elevation) ?? DartRuntimePrimitives.RequireValue(defaults__24865.elevation)), shadowColor: ((((_PopupMenu__popup_menu<T>)(object)this.widget).route.shadowColor ?? popupMenuTheme__24791.shadowColor) ?? defaults__24865.shadowColor), surfaceTintColor: ((((_PopupMenu__popup_menu<T>)(object)this.widget).route.surfaceTintColor ?? popupMenuTheme__24791.surfaceTintColor) ?? defaults__24865.surfaceTintColor), child: new global::Doroti.Generated.Framework.Widgets.Align(alignment: global::Doroti.Generated.Framework.Painting.AlignmentDirectional.topEnd, widthFactor: width__25672.evaluate(((_PopupMenu__popup_menu<T>)(object)this.widget).route.animation!), heightFactor: height__25730.evaluate(((_PopupMenu__popup_menu<T>)(object)this.widget).route.animation!), child: child))));
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: child__25825));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _PopupMenuRouteLayout__popup_menu : global::Doroti.Generated.Framework.Rendering.SingleChildLayoutDelegate
{
    public virtual global::Doroti.Generated.Framework.Rendering.RelativeRect position { get; private set; } = default!;
    public virtual List<Size?> itemSizes { get; set; } = default!;
    public virtual long? selectedItemIndex { get; private set; }
    public virtual TextDirection textDirection { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsets padding { get; set; } = default!;
    public virtual HashSet<Rect> avoidBounds { get; private set; } = default!;

    internal _PopupMenuRouteLayout__popup_menu(global::Doroti.Generated.Framework.Rendering.RelativeRect position, List<Size?> itemSizes, long? selectedItemIndex, TextDirection textDirection, global::Doroti.Generated.Framework.Painting.EdgeInsets padding, HashSet<Rect> avoidBounds)
    {
        this.position = position;
        this.itemSizes = itemSizes;
        this.selectedItemIndex = selectedItemIndex;
        this.textDirection = textDirection;
        this.padding = padding;
        this.avoidBounds = avoidBounds;
    }

    public override global::Doroti.Generated.Framework.Rendering.BoxConstraints getConstraintsForChild(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
        return ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)(object?)global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateLoose(((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).biggest).deflate((global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll(Popup_menuLibrary._kMenuScreenPadding).op_Add(this.padding))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Offset getPositionForChild(Size size, Size childSize)
    {
        double y__29191 = ((global::Doroti.Generated.Framework.Rendering.RelativeRect)this.position).top;
        double x__29407 = default!;
        if ((((global::Doroti.Generated.Framework.Rendering.RelativeRect)this.position).left > ((global::Doroti.Generated.Framework.Rendering.RelativeRect)this.position).right))
        {
            x__29407 = ((size.width - ((global::Doroti.Generated.Framework.Rendering.RelativeRect)this.position).right) - childSize.width);
        }
        else
        {
            if ((((global::Doroti.Generated.Framework.Rendering.RelativeRect)this.position).left < ((global::Doroti.Generated.Framework.Rendering.RelativeRect)this.position).right))
            {
                x__29407 = ((global::Doroti.Generated.Framework.Rendering.RelativeRect)this.position).left;
            }
            else
            {
                x__29407 = (this.textDirection switch { TextDirection.rtl => ((size.width - ((global::Doroti.Generated.Framework.Rendering.RelativeRect)this.position).right) - childSize.width), TextDirection.ltr => ((global::Doroti.Generated.Framework.Rendering.RelativeRect)this.position).left, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            }
        }
        var wantedPosition__30054 = new global::Doroti.Ui.Offset(x__29407, y__29191);
        global::Doroti.Ui.Offset originCenter__30102 = ((global::Doroti.Ui.Offset)(object?)((Offset)((dynamic)this.position.toRect((Offset.zero & size))).center));
        IEnumerable<global::Doroti.Ui.Rect> subScreens__30186 = ((IEnumerable<global::Doroti.Ui.Rect>)(object?)DisplayFeatureSubScreen.subScreensInBounds((Offset.zero & size), this.avoidBounds));
        global::Doroti.Ui.Rect subScreen__30310 = ((global::Doroti.Ui.Rect)(object?)_closestScreen(subScreens__30186.Cast<Rect>(), originCenter__30102));
        return _fitInsideScreen(subScreen__30310, childSize, wantedPosition__30054);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Rect _closestScreen(IEnumerable<Rect> screens, Offset point)
    {
        global::Doroti.Ui.Rect closest__30507 = ((global::Doroti.Ui.Rect)(object?)screens.First());
        foreach (var screen__30547 in screens)
        {
            if ((((((Offset)((dynamic)screen__30547).center) - point)).distance < ((((Offset)((dynamic)closest__30507).center) - point)).distance))
            {
                closest__30507 = screen__30547;
            }
        }
        return closest__30507;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Offset _fitInsideScreen(Rect screen, Size childSize, Offset wantedPosition)
    {
        double x__30806 = wantedPosition.dx;
        double y__30840 = wantedPosition.dy;
        if ((x__30806 < ((screen.left + Popup_menuLibrary._kMenuScreenPadding) + ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.padding).left)))
        {
            x__30806 = ((screen.left + Popup_menuLibrary._kMenuScreenPadding) + ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.padding).left);
        }
        else
        {
            if (((x__30806 + childSize.width) > ((screen.right - Popup_menuLibrary._kMenuScreenPadding) - ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.padding).right)))
            {
                x__30806 = (((screen.right - childSize.width) - Popup_menuLibrary._kMenuScreenPadding) - ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.padding).right);
            }
        }
        if ((y__30840 < ((screen.top + Popup_menuLibrary._kMenuScreenPadding) + ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.padding).top)))
        {
            y__30840 = (Popup_menuLibrary._kMenuScreenPadding + ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.padding).top);
        }
        else
        {
            if (((y__30840 + childSize.height) > ((screen.bottom - Popup_menuLibrary._kMenuScreenPadding) - ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.padding).bottom)))
            {
                y__30840 = (((screen.bottom - childSize.height) - Popup_menuLibrary._kMenuScreenPadding) - ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.padding).bottom);
            }
        }
        return new global::Doroti.Ui.Offset(x__30806, y__30840);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRelayout(global::Doroti.Generated.Framework.Rendering.SingleChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (_PopupMenuRouteLayout__popup_menu)(object)oldDelegate;
        DartRuntimePrimitives.Assert(() => (checked((long)(this.itemSizes.Count)) == checked((long)(((_PopupMenuRouteLayout__popup_menu)__oldDelegate).itemSizes.Count))));
        return ((((((!object.Equals(this.position, ((_PopupMenuRouteLayout__popup_menu)__oldDelegate).position)) || (this.selectedItemIndex != ((_PopupMenuRouteLayout__popup_menu)__oldDelegate).selectedItemIndex)) || (!object.Equals(this.textDirection, ((_PopupMenuRouteLayout__popup_menu)__oldDelegate).textDirection))) || !global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.listEquals(this.itemSizes, ((_PopupMenuRouteLayout__popup_menu)__oldDelegate).itemSizes)) || (!object.Equals(this.padding, ((_PopupMenuRouteLayout__popup_menu)__oldDelegate).padding))) || !global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.setEquals(this.avoidBounds, ((_PopupMenuRouteLayout__popup_menu)__oldDelegate).avoidBounds));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _PopupMenuRoute__popup_menu<T> : global::Doroti.Generated.Framework.Widgets.PopupRoute<T>
{
    public virtual global::Doroti.Generated.Framework.Rendering.RelativeRect? position { get; private set; }
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Rendering.BoxConstraints, global::Doroti.Generated.Framework.Rendering.RelativeRect>? positionBuilder { get; private set; }
    public virtual List<PopupMenuEntry<T>> items { get; private set; } = default!;
    public virtual List<global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>> itemKeys { get; private set; } = default!;
    public virtual List<Size?> itemSizes { get; private set; } = default!;
    public virtual T? initialValue { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual string? semanticLabel { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? menuPadding { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.CapturedThemes capturedThemes { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.AnimationStyle? popUpAnimationStyle { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation? _animation { get; set; } = default;
    private string? __field_barrierLabel = default!;
    public override string? barrierLabel { get => __field_barrierLabel; }

    internal _PopupMenuRoute__popup_menu(global::Doroti.Generated.Framework.Rendering.RelativeRect? position = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Rendering.BoxConstraints, global::Doroti.Generated.Framework.Rendering.RelativeRect>? positionBuilder = null, List<PopupMenuEntry<T>> items = default!, List<global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>> itemKeys = default!, T? initialValue = default, double? elevation = null, Color? surfaceTintColor = null, Color? shadowColor = null, string barrierLabel = default!, string? semanticLabel = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? menuPadding = null, Color? color = null, global::Doroti.Generated.Framework.Widgets.CapturedThemes capturedThemes = default!, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, Clip clipBehavior = default!, global::Doroti.Generated.Framework.Widgets.RouteSettings? settings = null, bool? requestFocus = null, global::Doroti.Generated.Framework.Animation.AnimationStyle? popUpAnimationStyle = null) : base(settings: settings, requestFocus: requestFocus, traversalEdgeBehavior: global::Doroti.Generated.Framework.Widgets.TraversalEdgeBehavior.closedLoop)
    {
        this.position = position;
        this.positionBuilder = positionBuilder;
        this.items = items;
        this.itemKeys = itemKeys;
        this.initialValue = initialValue;
        this.elevation = elevation;
        this.surfaceTintColor = surfaceTintColor;
        this.shadowColor = shadowColor;
        this.__field_barrierLabel = barrierLabel;
        this.semanticLabel = semanticLabel;
        this.shape = shape;
        this.menuPadding = menuPadding;
        this.color = color;
        this.capturedThemes = capturedThemes;
        this.constraints = constraints;
        this.clipBehavior = clipBehavior;
        this.popUpAnimationStyle = popUpAnimationStyle;
        this.itemSizes = new List<global::Doroti.Ui.Size?>(System.Linq.Enumerable.Repeat<global::Doroti.Ui.Size?>(null, checked((int)checked((long)(items.Count)))));
        System.Diagnostics.Debug.Assert((((position is not null)) != ((positionBuilder is not null))));
    }

    public override global::Doroti.Generated.Framework.Animation.Animation<double> createAnimation()
    {
        if ((!object.Equals(this.popUpAnimationStyle, global::Doroti.Generated.Framework.Animation.AnimationStyle.noAnimation)))
        {
            return _animation ??= new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: base.createAnimation(), curve: (this.popUpAnimationStyle?.curve ?? global::Doroti.Generated.Framework.Animation.Curves.linear), reverseCurve: (this.popUpAnimationStyle?.reverseCurve ?? new global::Doroti.Generated.Framework.Animation.Interval(0.0, Popup_menuLibrary._kMenuCloseIntervalEnd)));
        }
        return ((global::Doroti.Generated.Framework.Animation.Animation<double>)(object?)base.createAnimation());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void scrollTo(long selectedItemIndex)
    {
        global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) => {
if ((this.itemKeys[(int)(DartRuntimePrimitives.RequireValue(selectedItemIndex))].currentContext is not null))
{
    DartRuntimePrimitives.Ignore(Scrollable.ensureVisible(this.itemKeys[(int)(DartRuntimePrimitives.RequireValue(selectedItemIndex))].currentContext!));
}
})));
    }

    public override Duration transitionDuration => DartRuntimePrimitives.ConvertValue<Duration>((this.popUpAnimationStyle?.duration ?? Popup_menuLibrary._kMenuDuration));
    public override bool barrierDismissible => true;
    public override Color? barrierColor => DartRuntimePrimitives.ConvertValue<Color>(null);
    public override global::Doroti.Generated.Framework.Widgets.Widget buildPage(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation)
    {
        long? selectedItemIndex__34930 = default!;
        if ((this.initialValue is not null))
        {
            for (var index__34996 = 0L; ((selectedItemIndex__34930 is null) && (index__34996 < checked((long)(this.items.Count)))); index__34996 += 1L)
            {
                if (this.items[(int)(index__34996)].represents(this.initialValue))
                {
                    selectedItemIndex__34930 = index__34996;
                }
            }
        }
        if ((selectedItemIndex__34930 is not null))
        {
            long selectedItemIndex__34930__value35194 = DartRuntimePrimitives.RequireValue(selectedItemIndex__34930);
            scrollTo(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(selectedItemIndex__34930__value35194)));
        }
        global::Doroti.Generated.Framework.Widgets.Widget menu__35282 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _PopupMenu__popup_menu<T>(route: this, itemKeys: this.itemKeys, semanticLabel: this.semanticLabel, constraints: this.constraints, clipBehavior: this.clipBehavior));
        global::Doroti.Generated.Framework.Widgets.MediaQueryData mediaQuery__35483 = ((global::Doroti.Generated.Framework.Widgets.MediaQueryData)(object?)MediaQuery.of(context));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)global::Doroti.Generated.Framework.Widgets.MediaQuery.CreateRemovePadding(context: context, removeTop: true, removeBottom: true, removeLeft: true, removeRight: true, child: new global::Doroti.Generated.Framework.Widgets.LayoutBuilder(builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Rendering.BoxConstraints, global::Doroti.Generated.Framework.Widgets.Widget>)((context, constraints) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.CustomSingleChildLayout(@delegate: new _PopupMenuRouteLayout__popup_menu(((this.positionBuilder is null ? this.position! : this.positionBuilder.Invoke(context, constraints))), this.itemSizes, selectedItemIndex__34930, Directionality.of(context), ((global::Doroti.Generated.Framework.Widgets.MediaQueryData)mediaQuery__35483).padding, _avoidBounds(mediaQuery__35483)), child: this.capturedThemes.wrap(menu__35282)));
throw new InvalidOperationException("Dart closure completed without a value.");
})))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual HashSet<global::Doroti.Ui.Rect> _avoidBounds(global::Doroti.Generated.Framework.Widgets.MediaQueryData mediaQuery)
    {
        return ((HashSet<global::Doroti.Ui.Rect>)(object?)DisplayFeatureSubScreen.avoidBounds(mediaQuery).toSet());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        this._animation?.dispose();
        base.dispose();
    }

}

public delegate global::Doroti.Generated.Framework.Rendering.RelativeRect PopupMenuPositionBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints);

public static partial class Popup_menuLibrary
{
    public static Future<T?> showMenu<T>(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Rendering.RelativeRect? position = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Rendering.BoxConstraints, global::Doroti.Generated.Framework.Rendering.RelativeRect>? positionBuilder = null, List<PopupMenuEntry<T>> items = default!, T? initialValue = default, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, string? semanticLabel = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? menuPadding = null, Color? color = null, bool useRootNavigator = false, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, Clip clipBehavior = Clip.none, global::Doroti.Generated.Framework.Widgets.RouteSettings? routeSettings = null, global::Doroti.Generated.Framework.Animation.AnimationStyle? popUpAnimationStyle = null, bool? requestFocus = null)
    {
        DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(items));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        DartRuntimePrimitives.Assert(() => (((position is not null)) != ((positionBuilder is not null))), () => (object?)"Either position or positionBuilder must be provided.");
        switch (global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
                {
                    break;
                }
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
                {
                    semanticLabel ??= MaterialLocalizations.of(context).popupMenuLabel;
                    break;
                }
        }
        var menuItemKeys__41973 = DartRuntimePrimitives.CreateList<global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>>(checked((long)(items.Count)), ((index) => global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create()));
        global::Doroti.Generated.Framework.Widgets.NavigatorState navigator__42079 = ((global::Doroti.Generated.Framework.Widgets.NavigatorState)(object?)Navigator.of(context, rootNavigator: useRootNavigator));
        return ((Future<T?>)(object?)navigator__42079.push(new _PopupMenuRoute__popup_menu<T>(position: position, positionBuilder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Rendering.BoxConstraints, global::Doroti.Generated.Framework.Rendering.RelativeRect>?)positionBuilder, items: items, itemKeys: menuItemKeys__41973, initialValue: initialValue, elevation: elevation, shadowColor: shadowColor, surfaceTintColor: surfaceTintColor, semanticLabel: semanticLabel, barrierLabel: MaterialLocalizations.of(context).menuDismissLabel, shape: shape, menuPadding: menuPadding, color: color, capturedThemes: InheritedTheme.capture(from: context, to: navigator__42079.context), constraints: constraints, clipBehavior: clipBehavior, settings: routeSettings, popUpAnimationStyle: popUpAnimationStyle, requestFocus: requestFocus)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public delegate void PopupMenuItemSelected<T>(T value);

public delegate void PopupMenuCanceled();

public delegate List<PopupMenuEntry<T>> PopupMenuItemBuilder<T>(global::Doroti.Generated.Framework.Widgets.BuildContext context);

public class PopupMenuButton<T> : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, List<PopupMenuEntry<T>>> itemBuilder { get; private set; } = default!;
    public virtual T? initialValue { get; private set; }
    public virtual global::System.Action? onOpened { get; private set; }
    public virtual global::System.Action<T>? onSelected { get; private set; }
    public virtual global::System.Action? onCanceled { get; private set; }
    public virtual string? tooltip { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? menuPadding { get; private set; }
    public virtual double? splashRadius { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? child { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? icon { get; private set; }
    public virtual Offset offset { get; private set; } = default!;
    public virtual bool enabled { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual Color? iconColor { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual double? iconSize { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints { get; private set; }
    public virtual PopupMenuPosition? position { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual bool useRootNavigator { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.AnimationStyle? popUpAnimationStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.RouteSettings? routeSettings { get; private set; }
    public virtual ButtonStyle? style { get; private set; }
    public virtual bool? requestFocus { get; private set; }

    public PopupMenuButton(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, List<PopupMenuEntry<T>>> itemBuilder = default!, T? initialValue = default, global::System.Action? onOpened = null, global::System.Action<T>? onSelected = null, global::System.Action? onCanceled = null, string? tooltip = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding = default!, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? menuPadding = null, global::Doroti.Generated.Framework.Widgets.Widget? child = null, global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius = null, double? splashRadius = null, global::Doroti.Generated.Framework.Widgets.Widget? icon = null, double? iconSize = null, Offset offset = default, bool enabled = true, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, Color? color = null, Color? iconColor = null, bool? enableFeedback = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, PopupMenuPosition? position = null, Clip clipBehavior = Clip.none, bool useRootNavigator = false, global::Doroti.Generated.Framework.Animation.AnimationStyle? popUpAnimationStyle = null, global::Doroti.Generated.Framework.Widgets.RouteSettings? routeSettings = null, ButtonStyle? style = null, bool? requestFocus = null) : base(key: key)
    {
        global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry __padding = padding ?? global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll(8.0);
        this.itemBuilder = itemBuilder;
        this.initialValue = initialValue;
        this.onOpened = onOpened;
        this.onSelected = onSelected;
        this.onCanceled = onCanceled;
        this.tooltip = tooltip;
        this.elevation = elevation;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.padding = __padding;
        this.menuPadding = menuPadding;
        this.child = child;
        this.borderRadius = borderRadius;
        this.splashRadius = splashRadius;
        this.icon = icon;
        this.iconSize = iconSize;
        this.offset = offset;
        this.enabled = enabled;
        this.shape = shape;
        this.color = color;
        this.iconColor = iconColor;
        this.enableFeedback = enableFeedback;
        this.constraints = constraints;
        this.position = position;
        this.clipBehavior = clipBehavior;
        this.useRootNavigator = useRootNavigator;
        this.popUpAnimationStyle = popUpAnimationStyle;
        this.routeSettings = routeSettings;
        this.style = style;
        this.requestFocus = requestFocus;
        System.Diagnostics.Debug.Assert(!(((child is not null) && (icon is not null))));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new PopupMenuButtonState<T>());
}

public class PopupMenuButtonState<T> : global::Doroti.Generated.Framework.Widgets.State<PopupMenuButton<T>>
{
    internal virtual bool _isMenuExpanded { get; set; } = false;
    internal virtual global::Doroti.Generated.Framework.Rendering.RelativeRect? _lastPosition { get; set; } = default;
    internal virtual PopupMenuThemeData _popupMenuTheme { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Rendering.RenderBox? _cachedButtonRenderBox { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Rendering.RenderBox? _cachedOverlayRenderBox { get; set; } = default;

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _updateCachedObjects();
    }

    internal virtual void _updateCachedObjects()
    {
        if (this.mounted)
        {
            _popupMenuTheme = PopupMenuTheme.of(this.context);
            global::Doroti.Generated.Framework.Rendering.RenderObject? buttonRenderObject__57178 = ((global::Doroti.Generated.Framework.Rendering.RenderObject?)(object?)this.context.findRenderObject());
            if ((buttonRenderObject__57178 is global::Doroti.Generated.Framework.Rendering.RenderBox))
            {
                global::Doroti.Generated.Framework.Rendering.RenderBox buttonRenderObject__57178__as57237 = (global::Doroti.Generated.Framework.Rendering.RenderBox)buttonRenderObject__57178;
                _cachedButtonRenderBox = ((global::Doroti.Generated.Framework.Rendering.RenderBox)buttonRenderObject__57178__as57237);
            }
            try
            {
                global::Doroti.Generated.Framework.Widgets.NavigatorState navigator__57374 = ((global::Doroti.Generated.Framework.Widgets.NavigatorState)(object?)Navigator.of(this.context, rootNavigator: ((PopupMenuButton<T>)(object)this.widget).useRootNavigator));
                global::Doroti.Generated.Framework.Rendering.RenderObject? overlayRenderObject__57508 = ((global::Doroti.Generated.Framework.Rendering.RenderObject?)(object?)((global::Doroti.Generated.Framework.Widgets.NavigatorState)navigator__57374).overlay?.context.findRenderObject());
                if ((overlayRenderObject__57508 is global::Doroti.Generated.Framework.Rendering.RenderBox))
                {
                    global::Doroti.Generated.Framework.Rendering.RenderBox overlayRenderObject__57508__as57589 = (global::Doroti.Generated.Framework.Rendering.RenderBox)overlayRenderObject__57508;
                    _cachedOverlayRenderBox = ((global::Doroti.Generated.Framework.Rendering.RenderBox)overlayRenderObject__57508__as57589);
                }
            }
            catch (Exception e__57707)
            {
                _cachedButtonRenderBox = null;
                _cachedOverlayRenderBox = null;
            }
        }
    }

    internal virtual global::Doroti.Generated.Framework.Rendering.RelativeRect _getDefaultPosition(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
        return (this._lastPosition ?? global::Doroti.Generated.Framework.Rendering.RelativeRect.CreateFromSize(Rect.zero, ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).biggest));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Rendering.RelativeRect _positionBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext __unused0, global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
        if (!this.mounted)
        {
            return ((global::Doroti.Generated.Framework.Rendering.RelativeRect)(object?)_getDefaultPosition(constraints));
        }
        PopupMenuThemeData popupMenuTheme__58417 = this._popupMenuTheme;
        global::Doroti.Generated.Framework.Rendering.RenderBox? button__58520 = this._cachedButtonRenderBox;
        global::Doroti.Generated.Framework.Rendering.RenderBox? overlay__58574 = this._cachedOverlayRenderBox;
        if (((((button__58520 is null) || (overlay__58574 is null)) || !button__58520.attached) || !overlay__58574.attached))
        {
            return ((global::Doroti.Generated.Framework.Rendering.RelativeRect)(object?)_getDefaultPosition(constraints));
        }
        PopupMenuPosition popupMenuPosition__58939 = ((((PopupMenuButton<T>)(object)this.widget).position ?? popupMenuTheme__58417.position) ?? PopupMenuPosition.over);
        global::Doroti.Ui.Offset offset__59053 = default!;
        switch (popupMenuPosition__58939)
        {
            case var __constant59105 when (object.Equals(__constant59105, PopupMenuPosition.over)):
                {
                    offset__59053 = ((PopupMenuButton<T>)(object)this.widget).offset;
                    break;
                }
            case var __constant59172 when (object.Equals(__constant59172, PopupMenuPosition.under)):
                {
                    offset__59053 = (new global::Doroti.Ui.Offset(0.0, ((global::Doroti.Generated.Framework.Rendering.RenderBox)button__58520).size.height) + ((PopupMenuButton<T>)(object)this.widget).offset);
                    if ((((PopupMenuButton<T>)(object)this.widget).child is null))
                    {
                        offset__59053 -= new global::Doroti.Ui.Offset(0.0, (((PopupMenuButton<T>)(object)this.widget).padding.vertical / 2L));
                    }
                    break;
                }
        }
        var position__59439 = global::Doroti.Generated.Framework.Rendering.RelativeRect.CreateFromRect(global::Doroti.Ui.Rect.fromPoints(((Offset)((dynamic)button__58520).localToGlobal(offset__59053, ancestor: overlay__58574)), ((Offset)((dynamic)button__58520).localToGlobal((((global::Doroti.Generated.Framework.Rendering.RenderBox)button__58520).size.bottomRight(Offset.zero) + offset__59053), ancestor: overlay__58574))), (Offset.zero & ((global::Doroti.Generated.Framework.Rendering.RenderBox)overlay__58574).size));
        return _lastPosition = position__59439;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void showButtonMenu()
    {
        _updateCachedObjects();
        List<PopupMenuEntry<T>> items__60372 = this.widget.itemBuilder(this.context).ToList();
        if (System.Linq.Enumerable.Any(items__60372))
        {
            ((PopupMenuButton<T>)(object)this.widget).onOpened?.Invoke();
            setState(((global::System.Action)(() => {
_isMenuExpanded = true;
})));
            DartRuntimePrimitives.Ignore(Popup_menuLibrary.showMenu<T?>(context: this.context, elevation: ((PopupMenuButton<T>)(object)this.widget).elevation, shadowColor: ((PopupMenuButton<T>)(object)this.widget).shadowColor, surfaceTintColor: ((PopupMenuButton<T>)(object)this.widget).surfaceTintColor, items: items__60372, initialValue: ((PopupMenuButton<T>)(object)this.widget).initialValue, positionBuilder: this._positionBuilder, shape: ((PopupMenuButton<T>)(object)this.widget).shape, menuPadding: ((PopupMenuButton<T>)(object)this.widget).menuPadding, color: ((PopupMenuButton<T>)(object)this.widget).color, constraints: ((PopupMenuButton<T>)(object)this.widget).constraints, clipBehavior: ((PopupMenuButton<T>)(object)this.widget).clipBehavior, useRootNavigator: ((PopupMenuButton<T>)(object)this.widget).useRootNavigator, popUpAnimationStyle: ((PopupMenuButton<T>)(object)this.widget).popUpAnimationStyle, routeSettings: ((PopupMenuButton<T>)(object)this.widget).routeSettings, requestFocus: ((PopupMenuButton<T>)(object)this.widget).requestFocus).then((global::System.Action<T?>)((newValue) => {
if (!this.mounted)
{
    _ = (object?)null;
    return;
}
setState(((global::System.Action)(() => {
_isMenuExpanded = false;
})));
if ((newValue is null))
{
    ((PopupMenuButton<T>)(object)this.widget).onCanceled?.Invoke();
    _ = (object?)null;
    return;
}
((PopupMenuButton<T>)(object)this.widget).onSelected?.Invoke(newValue);
})));
        }
    }

    internal virtual bool _canRequestFocus
    {
        get
        {
            global::Doroti.Generated.Framework.Widgets.NavigationMode mode__61630 = (MediaQuery.maybeNavigationModeOf(this.context) ?? global::Doroti.Generated.Framework.Widgets.NavigationMode.traditional);
            return (mode__61630 switch { global::Doroti.Generated.Framework.Widgets.NavigationMode.traditional => ((PopupMenuButton<T>)(object)this.widget).enabled, global::Doroti.Generated.Framework.Widgets.NavigationMode.directional => true, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            return default!;
        }
    }
    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Generated.Framework.Widgets.IconThemeData iconTheme__61939 = ((global::Doroti.Generated.Framework.Widgets.IconThemeData)(object?)IconTheme.of(context));
        PopupMenuThemeData popupMenuTheme__62003 = PopupMenuTheme.of(context);
        bool enableFeedback__62063 = ((((PopupMenuButton<T>)(object)this.widget).enableFeedback ?? PopupMenuTheme.of(context).enableFeedback) ?? true);
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        if ((((PopupMenuButton<T>)(object)this.widget).child is not null))
        {
            global::Doroti.Generated.Framework.Widgets.Widget child__62274 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new Tooltip(message: (((PopupMenuButton<T>)(object)this.widget).tooltip ?? MaterialLocalizations.of(context).showMenuTooltip), child: new InkWell(borderRadius: ((PopupMenuButton<T>)(object)this.widget).borderRadius, onTap: ((global::System.Action)(((PopupMenuButton<T>)(object)this.widget).enabled ? this.showButtonMenu : null)), canRequestFocus: this._canRequestFocus, radius: ((PopupMenuButton<T>)(object)this.widget).splashRadius, enableFeedback: enableFeedback__62063, child: ((PopupMenuButton<T>)(object)this.widget).child)));
            MaterialTapTargetSize tapTargetSize__62714 = (((PopupMenuButton<T>)(object)this.widget).style?.tapTargetSize ?? MaterialTapTargetSize.shrinkWrap);
            if ((object.Equals(tapTargetSize__62714, MaterialTapTargetSize.padded)))
            {
                return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Generated.Framework.Rendering.BoxConstraints(minWidth: global::Doroti.Generated.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension, minHeight: global::Doroti.Generated.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension), child: child__62274));
            }
            return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Semantics(expanded: this._isMenuExpanded, child: child__62274));
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Semantics(child: new IconButton(key: StandardComponentTypeMembers.key(global::Doroti.Generated.Framework.Widgets.StandardComponentType.moreButton), icon: new global::Doroti.Generated.Framework.Widgets.Semantics(expanded: this._isMenuExpanded, child: (((PopupMenuButton<T>)(object)this.widget).icon ?? new global::Doroti.Generated.Framework.Widgets.Icon(Icons.adaptive.more))), padding: ((PopupMenuButton<T>)(object)this.widget).padding, splashRadius: ((PopupMenuButton<T>)(object)this.widget).splashRadius, iconSize: ((((PopupMenuButton<T>)(object)this.widget).iconSize ?? popupMenuTheme__62003.iconSize) ?? ((global::Doroti.Generated.Framework.Widgets.IconThemeData)iconTheme__61939).size), color: ((((PopupMenuButton<T>)(object)this.widget).iconColor ?? popupMenuTheme__62003.iconColor) ?? ((global::Doroti.Generated.Framework.Widgets.IconThemeData)iconTheme__61939).color), tooltip: (((PopupMenuButton<T>)(object)this.widget).tooltip ?? MaterialLocalizations.of(context).showMenuTooltip), onPressed: ((global::System.Action)(((PopupMenuButton<T>)(object)this.widget).enabled ? this.showButtonMenu : null)), enableFeedback: enableFeedback__62063, style: ((PopupMenuButton<T>)(object)this.widget).style)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _EffectiveMouseCursor__popup_menu : global::Doroti.Generated.Framework.Widgets.WidgetStateMouseCursor
{
    public virtual global::Doroti.Generated.Framework.Services.MouseCursor? widgetCursor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? themeCursor { get; private set; }

    internal _EffectiveMouseCursor__popup_menu(global::Doroti.Generated.Framework.Services.MouseCursor? widgetCursor, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? themeCursor)
    {
        this.widgetCursor = widgetCursor;
        this.themeCursor = themeCursor;
    }

    public override global::Doroti.Generated.Framework.Services.MouseCursor resolve(HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> states)
    {
        return ((((WidgetStateProperty.resolveAs<global::Doroti.Generated.Framework.Services.MouseCursor?>(this.widgetCursor, states) ?? (global::Doroti.Generated.Framework.Services.MouseCursor)this.themeCursor?.resolve(states))) ?? (global::Doroti.Generated.Framework.Services.MouseCursor)global::Doroti.Generated.Framework.Widgets.WidgetStateMouseCursor.adaptiveClickable.resolve(states)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string debugDescription => "WidgetStateMouseCursor(PopupMenuItemState)";
}

internal class _PopupMenuDefaultsM2__popup_menu : PopupMenuThemeData
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__theme_initialized;
    private ThemeData __late__theme = default!;
    internal virtual ThemeData _theme
    {
        get
        {
            if (!__late__theme_initialized)
            {
                __late__theme = Theme.of(this.context);
                __late__theme_initialized = true;
            }
            return __late__theme;
        }
    }
    private bool __late__textTheme_initialized;
    private TextTheme __late__textTheme = default!;
    internal virtual TextTheme _textTheme
    {
        get
        {
            if (!__late__textTheme_initialized)
            {
                __late__textTheme = this._theme.textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }
    public static global::Doroti.Generated.Framework.Painting.EdgeInsets menuItemPadding = global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 16.0);

    internal _PopupMenuDefaultsM2__popup_menu(global::Doroti.Generated.Framework.Widgets.BuildContext context) : base(elevation: 8.0)
    {
        this.context = context;
    }

    public override global::Doroti.Generated.Framework.Painting.TextStyle? textStyle => this._textTheme.titleMedium;
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsets? menuPadding => global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(vertical: 8.0);
}

internal class _PopupMenuDefaultsM3__popup_menu : PopupMenuThemeData
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__theme_initialized;
    private ThemeData __late__theme = default!;
    internal virtual ThemeData _theme
    {
        get
        {
            if (!__late__theme_initialized)
            {
                __late__theme = Theme.of(this.context);
                __late__theme_initialized = true;
            }
            return __late__theme;
        }
    }
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = this._theme.colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }
    private bool __late__textTheme_initialized;
    private TextTheme __late__textTheme = default!;
    internal virtual TextTheme _textTheme
    {
        get
        {
            if (!__late__textTheme_initialized)
            {
                __late__textTheme = this._theme.textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }
    public static global::Doroti.Generated.Framework.Painting.EdgeInsets menuItemPadding = global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 12.0);

    internal _PopupMenuDefaultsM3__popup_menu(global::Doroti.Generated.Framework.Widgets.BuildContext context) : base(elevation: 3.0)
    {
        this.context = context;
    }

    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?>? labelTextStyle
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?>?)(object?)WidgetStateProperty.resolveWith((states) => {
global::Doroti.Generated.Framework.Painting.TextStyle style__65924 = this._textTheme.labelLarge!;
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return (style__65924.apply(color: this._colors.onSurface.withOpacity(0.38)));
}
return (style__65924.apply(color: this._colors.onSurface));
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            return default!;
        }
    }
    public virtual global::Doroti.Ui.Color? color => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.surfaceContainer);
    public virtual global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.shadow);
    public virtual global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public override global::Doroti.Generated.Framework.Painting.ShapeBorder? shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.ShapeBorder>(new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(4.0))));
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsets? menuPadding => global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(vertical: 8.0);
}
