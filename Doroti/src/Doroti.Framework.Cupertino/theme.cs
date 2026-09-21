// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class ThemeLibrary
{
    internal static _CupertinoThemeDefaults__theme _kDefaultTheme =
        new _CupertinoThemeDefaults__theme(
            null,
            CupertinoColors.systemBlue,
            CupertinoColors.white,
            CupertinoDynamicColor.CreateWithBrightness(
                color: new Color(4042914297L),
                darkColor: new Color(4028439837L)
            ),
            CupertinoColors.systemBackground,
            CupertinoColors.systemBlue,
            false,
            new _CupertinoTextThemeDefaults__theme(
                CupertinoColors.label,
                CupertinoColors.inactiveGray
            )
        );
}

public class CupertinoTheme : StatelessWidget
{
    public virtual CupertinoThemeData data { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public CupertinoTheme(
        Key? key = null,
        CupertinoThemeData data = default!,
        Widget child = default!
    )
        : base(key: key)
    {
        this.data = data;
        this.child = child;
    }

    public static CupertinoThemeData of(BuildContext context)
    {
        InheritedCupertinoTheme? inheritedTheme =
            context.dependOnInheritedWidgetOfExactType<InheritedCupertinoTheme>();
        return (inheritedTheme?.theme.data ?? new CupertinoThemeData()).resolveFrom(context);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static Brightness brightnessOf(BuildContext context)
    {
        InheritedCupertinoTheme? inheritedTheme =
            context.dependOnInheritedWidgetOfExactType<InheritedCupertinoTheme>();
        return inheritedTheme?.theme.data.brightness ?? MediaQuery.platformBrightnessOf(context);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static Brightness? maybeBrightnessOf(BuildContext context)
    {
        InheritedCupertinoTheme? inheritedTheme =
            context.dependOnInheritedWidgetOfExactType<InheritedCupertinoTheme>();
        return inheritedTheme?.theme.data.brightness
            ?? MediaQuery.maybePlatformBrightnessOf(context);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget build(BuildContext context)
    {
        return new InheritedCupertinoTheme(
            theme: this,
            child: new IconTheme(
                data: new CupertinoIconThemeData(color: data.primaryColor),
                child: child
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        data.debugFillProperties(properties);
    }
}

public class InheritedCupertinoTheme : InheritedTheme
{
    public virtual CupertinoTheme theme { get; private set; } = default!;

    public InheritedCupertinoTheme(
        Key? key = null,
        CupertinoTheme theme = default!,
        Widget child = default!
    )
        : base(key: key, child: child)
    {
        this.theme = theme;
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return new CupertinoTheme(data: theme.data, child: child);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        DartRuntimePrimitives.ConvertValue<bool>(
            !Equals(theme.data, ((InheritedCupertinoTheme)oldWidget).theme.data)
        );
}

public class CupertinoThemeData : NoDefaultCupertinoThemeData, Diagnosticable
{
    internal virtual _CupertinoThemeDefaults__theme _defaults { get; private set; } = default!;

    public CupertinoThemeData(
        Brightness? brightness = null,
        Color? primaryColor = null,
        Color? primaryContrastingColor = null,
        CupertinoTextThemeData? textTheme = null,
        Color? barBackgroundColor = null,
        Color? scaffoldBackgroundColor = null,
        Color? selectionHandleColor = null,
        bool? applyThemeToAll = null
    )
        : this(
            brightness,
            primaryColor,
            primaryContrastingColor,
            textTheme,
            barBackgroundColor,
            scaffoldBackgroundColor,
            selectionHandleColor,
            applyThemeToAll,
            ThemeLibrary._kDefaultTheme
        ) { }

    public static CupertinoThemeData CreateRaw(
        Brightness? brightness,
        Color? primaryColor,
        Color? primaryContrastingColor,
        CupertinoTextThemeData? textTheme,
        Color? barBackgroundColor,
        Color? scaffoldBackgroundColor,
        Color? selectionHandleColor,
        bool? applyThemeToAll
    )
    {
        return new CupertinoThemeData(
            brightness,
            primaryColor,
            primaryContrastingColor,
            textTheme,
            barBackgroundColor,
            scaffoldBackgroundColor,
            selectionHandleColor,
            applyThemeToAll,
            ThemeLibrary._kDefaultTheme
        );
    }

    internal CupertinoThemeData(
        Brightness? brightness,
        Color? primaryColor,
        Color? primaryContrastingColor,
        CupertinoTextThemeData? textTheme,
        Color? barBackgroundColor,
        Color? scaffoldBackgroundColor,
        Color? selectionHandleColor,
        bool? applyThemeToAll,
        _CupertinoThemeDefaults__theme _defaults
    )
        : base(
            brightness: brightness,
            primaryColor: primaryColor,
            primaryContrastingColor: primaryContrastingColor,
            textTheme: textTheme,
            barBackgroundColor: barBackgroundColor,
            scaffoldBackgroundColor: scaffoldBackgroundColor,
            selectionHandleColor: selectionHandleColor,
            applyThemeToAll: applyThemeToAll
        )
    {
        this._defaults = _defaults;
    }

    public override Color primaryColor =>
        DartRuntimePrimitives.ConvertValue<Color>(base.primaryColor ?? _defaults.primaryColor);
    public override Color primaryContrastingColor =>
        DartRuntimePrimitives.ConvertValue<Color>(
            base.primaryContrastingColor ?? _defaults.primaryContrastingColor
        );
    public override CupertinoTextThemeData textTheme
    {
        get
        {
            return base.textTheme
                ?? _defaults.textThemeDefaults.createDefaults(primaryColor: primaryColor);
        }
    }
    public override Color barBackgroundColor =>
        DartRuntimePrimitives.ConvertValue<Color>(
            base.barBackgroundColor ?? _defaults.barBackgroundColor
        );
    public override Color scaffoldBackgroundColor =>
        DartRuntimePrimitives.ConvertValue<Color>(
            base.scaffoldBackgroundColor ?? _defaults.scaffoldBackgroundColor
        );
    public override Color selectionHandleColor =>
        DartRuntimePrimitives.ConvertValue<Color>(
            base.selectionHandleColor ?? _defaults.selectionHandleColor
        );
    public override bool? applyThemeToAll =>
        DartRuntimePrimitives.ConvertValue<bool>(base.applyThemeToAll ?? _defaults.applyThemeToAll);

    public override NoDefaultCupertinoThemeData noDefault()
    {
        return new NoDefaultCupertinoThemeData(
            brightness: base.brightness,
            primaryColor: base.primaryColor,
            primaryContrastingColor: base.primaryContrastingColor,
            textTheme: base.textTheme,
            barBackgroundColor: base.barBackgroundColor,
            scaffoldBackgroundColor: base.scaffoldBackgroundColor,
            selectionHandleColor: base.selectionHandleColor,
            applyThemeToAll: base.applyThemeToAll
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override CupertinoThemeData resolveFrom(BuildContext context)
    {
        Color? convertColor(Color? color)
        {
            return CupertinoDynamicColor.maybeResolve(color, context);
            throw new InvalidOperationException(
                "Control flow completed without returning a value."
            );
        }
        return new CupertinoThemeData(
            brightness,
            convertColor(base.primaryColor),
            convertColor(base.primaryContrastingColor),
            base.textTheme?.resolveFrom(context),
            convertColor(base.barBackgroundColor),
            convertColor(base.scaffoldBackgroundColor),
            convertColor(base.selectionHandleColor),
            applyThemeToAll,
            _defaults.resolveFrom(context, base.textTheme is null)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override CupertinoThemeData copyWith(
        Brightness? brightness = null,
        Color? primaryColor = null,
        Color? primaryContrastingColor = null,
        CupertinoTextThemeData? textTheme = null,
        Color? barBackgroundColor = null,
        Color? scaffoldBackgroundColor = null,
        Color? selectionHandleColor = null,
        bool? applyThemeToAll = null
    )
    {
        return new CupertinoThemeData(
            brightness ?? base.brightness,
            primaryColor ?? base.primaryColor,
            primaryContrastingColor ?? base.primaryContrastingColor,
            textTheme ?? base.textTheme,
            barBackgroundColor ?? base.barBackgroundColor,
            scaffoldBackgroundColor ?? base.scaffoldBackgroundColor,
            selectionHandleColor ?? base.selectionHandleColor,
            applyThemeToAll ?? base.applyThemeToAll,
            _defaults
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        var defaultData = new CupertinoThemeData();
        properties.add(new EnumProperty<Brightness>("brightness", brightness, defaultValue: null));
        properties.add(
            ColorsLibrary.createCupertinoColorProperty(
                "primaryColor",
                primaryColor,
                defaultValue: defaultData.primaryColor
            )
        );
        properties.add(
            ColorsLibrary.createCupertinoColorProperty(
                "primaryContrastingColor",
                primaryContrastingColor,
                defaultValue: defaultData.primaryContrastingColor
            )
        );
        properties.add(
            ColorsLibrary.createCupertinoColorProperty(
                "barBackgroundColor",
                barBackgroundColor,
                defaultValue: defaultData.barBackgroundColor
            )
        );
        properties.add(
            ColorsLibrary.createCupertinoColorProperty(
                "scaffoldBackgroundColor",
                scaffoldBackgroundColor,
                defaultValue: defaultData.scaffoldBackgroundColor
            )
        );
        properties.add(
            ColorsLibrary.createCupertinoColorProperty(
                "selectionHandleColor",
                selectionHandleColor,
                defaultValue: defaultData.selectionHandleColor
            )
        );
        properties.add(
            new DiagnosticsProperty<bool>(
                "applyThemeToAll",
                applyThemeToAll,
                defaultValue: defaultData.applyThemeToAll
            )
        );
        textTheme.debugFillProperties(properties);
    }

    public override bool Equals(object? other)
    {
        var __other = other as CupertinoThemeData;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is CupertinoThemeData)
            && Equals(__other.brightness, brightness)
            && Equals(__other.primaryColor, primaryColor)
            && Equals(__other.primaryContrastingColor, primaryContrastingColor)
            && Equals(__other.textTheme, textTheme)
            && Equals(__other.barBackgroundColor, barBackgroundColor)
            && Equals(__other.scaffoldBackgroundColor, scaffoldBackgroundColor)
            && Equals(__other.selectionHandleColor, selectionHandleColor)
            && (__other.applyThemeToAll == applyThemeToAll);
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(
                brightness,
                primaryColor,
                primaryContrastingColor,
                textTheme,
                barBackgroundColor,
                scaffoldBackgroundColor,
                selectionHandleColor,
                applyThemeToAll
            )
        );

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);

    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine)
                .toDiagnosticsNode()
                .toStringDeep(minLevel: minLevel);
            return true;
        });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(
        string? name = null,
        DiagnosticsTreeStyle? style = null
    )
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class NoDefaultCupertinoThemeData
{
    public virtual Brightness? brightness { get; private set; }
    public virtual Color? primaryColor { get; private set; }
    public virtual Color? primaryContrastingColor { get; private set; }
    public virtual CupertinoTextThemeData? textTheme { get; private set; }
    public virtual Color? barBackgroundColor { get; private set; }
    public virtual Color? scaffoldBackgroundColor { get; private set; }
    public virtual Color? selectionHandleColor { get; private set; }
    public virtual bool? applyThemeToAll { get; private set; }

    public NoDefaultCupertinoThemeData(
        Brightness? brightness = null,
        Color? primaryColor = null,
        Color? primaryContrastingColor = null,
        CupertinoTextThemeData? textTheme = null,
        Color? barBackgroundColor = null,
        Color? scaffoldBackgroundColor = null,
        Color? selectionHandleColor = null,
        bool? applyThemeToAll = null
    )
    {
        this.brightness = brightness;
        this.primaryColor = primaryColor;
        this.primaryContrastingColor = primaryContrastingColor;
        this.textTheme = textTheme;
        this.barBackgroundColor = barBackgroundColor;
        this.scaffoldBackgroundColor = scaffoldBackgroundColor;
        this.selectionHandleColor = selectionHandleColor;
        this.applyThemeToAll = applyThemeToAll;
    }

    public virtual NoDefaultCupertinoThemeData noDefault() => this;

    public virtual NoDefaultCupertinoThemeData resolveFrom(BuildContext context)
    {
        Color? convertColor(Color? color)
        {
            return CupertinoDynamicColor.maybeResolve(color, context);
            throw new InvalidOperationException(
                "Control flow completed without returning a value."
            );
        }
        return new NoDefaultCupertinoThemeData(
            brightness: brightness,
            primaryColor: convertColor(primaryColor),
            primaryContrastingColor: convertColor(primaryContrastingColor),
            textTheme: textTheme?.resolveFrom(context),
            barBackgroundColor: convertColor(barBackgroundColor),
            scaffoldBackgroundColor: convertColor(scaffoldBackgroundColor),
            selectionHandleColor: convertColor(selectionHandleColor),
            applyThemeToAll: applyThemeToAll
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual NoDefaultCupertinoThemeData copyWith(
        Brightness? brightness = null,
        Color? primaryColor = null,
        Color? primaryContrastingColor = null,
        CupertinoTextThemeData? textTheme = null,
        Color? barBackgroundColor = null,
        Color? scaffoldBackgroundColor = null,
        Color? selectionHandleColor = null,
        bool? applyThemeToAll = null
    )
    {
        return new NoDefaultCupertinoThemeData(
            brightness: brightness ?? this.brightness,
            primaryColor: primaryColor ?? this.primaryColor,
            primaryContrastingColor: primaryContrastingColor ?? this.primaryContrastingColor,
            textTheme: textTheme ?? this.textTheme,
            barBackgroundColor: barBackgroundColor ?? this.barBackgroundColor,
            scaffoldBackgroundColor: scaffoldBackgroundColor ?? this.scaffoldBackgroundColor,
            selectionHandleColor: selectionHandleColor ?? this.selectionHandleColor,
            applyThemeToAll: applyThemeToAll ?? this.applyThemeToAll
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as NoDefaultCupertinoThemeData;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is NoDefaultCupertinoThemeData)
            && Equals(__other.brightness, brightness)
            && Equals(__other.primaryColor, primaryColor)
            && Equals(__other.primaryContrastingColor, primaryContrastingColor)
            && Equals(__other.textTheme, textTheme)
            && Equals(__other.barBackgroundColor, barBackgroundColor)
            && Equals(__other.scaffoldBackgroundColor, scaffoldBackgroundColor)
            && (__other.applyThemeToAll == applyThemeToAll);
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(
                brightness,
                primaryColor,
                primaryContrastingColor,
                textTheme,
                barBackgroundColor,
                scaffoldBackgroundColor,
                applyThemeToAll
            )
        );
}

internal class _CupertinoThemeDefaults__theme
{
    public virtual Brightness? brightness { get; private set; }
    public virtual Color primaryColor { get; private set; } = default!;
    public virtual Color primaryContrastingColor { get; private set; } = default!;
    public virtual Color barBackgroundColor { get; private set; } = default!;
    public virtual Color scaffoldBackgroundColor { get; private set; } = default!;
    public virtual Color selectionHandleColor { get; private set; } = default!;
    public virtual bool applyThemeToAll { get; private set; } = default!;
    public virtual _CupertinoTextThemeDefaults__theme textThemeDefaults { get; private set; } =
        default!;

    internal _CupertinoThemeDefaults__theme(
        Brightness? brightness,
        Color primaryColor,
        Color primaryContrastingColor,
        Color barBackgroundColor,
        Color scaffoldBackgroundColor,
        Color selectionHandleColor,
        bool applyThemeToAll,
        _CupertinoTextThemeDefaults__theme textThemeDefaults
    )
    {
        this.brightness = brightness;
        this.primaryColor = primaryColor;
        this.primaryContrastingColor = primaryContrastingColor;
        this.barBackgroundColor = barBackgroundColor;
        this.scaffoldBackgroundColor = scaffoldBackgroundColor;
        this.selectionHandleColor = selectionHandleColor;
        this.applyThemeToAll = applyThemeToAll;
        this.textThemeDefaults = textThemeDefaults;
    }

    public virtual _CupertinoThemeDefaults__theme resolveFrom(
        BuildContext context,
        bool resolveTextTheme
    )
    {
        Color convertColor(Color color)
        {
            return CupertinoDynamicColor.resolve(color, context);
            throw new InvalidOperationException(
                "Control flow completed without returning a value."
            );
        }
        return new _CupertinoThemeDefaults__theme(
            brightness,
            convertColor(primaryColor),
            convertColor(primaryContrastingColor),
            convertColor(barBackgroundColor),
            convertColor(scaffoldBackgroundColor),
            convertColor(selectionHandleColor),
            applyThemeToAll,
            resolveTextTheme ? textThemeDefaults.resolveFrom(context) : textThemeDefaults
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class _CupertinoTextThemeDefaults__theme
{
    public virtual Color labelColor { get; private set; } = default!;
    public virtual Color inactiveGray { get; private set; } = default!;

    internal _CupertinoTextThemeDefaults__theme(Color labelColor, Color inactiveGray)
    {
        this.labelColor = labelColor;
        this.inactiveGray = inactiveGray;
    }

    public virtual _CupertinoTextThemeDefaults__theme resolveFrom(BuildContext context)
    {
        return new _CupertinoTextThemeDefaults__theme(
            CupertinoDynamicColor.resolve(labelColor, context),
            CupertinoDynamicColor.resolve(inactiveGray, context)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual CupertinoTextThemeData createDefaults(Color primaryColor)
    {
        return new _DefaultCupertinoTextThemeData__theme(
            primaryColor: primaryColor,
            labelColor: labelColor,
            inactiveGray: inactiveGray
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _DefaultCupertinoTextThemeData__theme : CupertinoTextThemeData
{
    public virtual Color labelColor { get; private set; } = default!;
    public virtual Color inactiveGray { get; private set; } = default!;

    internal _DefaultCupertinoTextThemeData__theme(
        Color labelColor,
        Color inactiveGray,
        Color primaryColor
    )
        : base(primaryColor: primaryColor)
    {
        this.labelColor = labelColor;
        this.inactiveGray = inactiveGray;
    }

    public override TextStyle textStyle => base.textStyle.copyWith(color: labelColor);
    public override TextStyle tabLabelTextStyle =>
        base.tabLabelTextStyle.copyWith(color: inactiveGray);
    public override TextStyle navTitleTextStyle =>
        base.navTitleTextStyle.copyWith(color: labelColor);
    public override TextStyle navLargeTitleTextStyle =>
        base.navLargeTitleTextStyle.copyWith(color: labelColor);
    public override TextStyle pickerTextStyle => base.pickerTextStyle.copyWith(color: labelColor);
    public override TextStyle dateTimePickerTextStyle =>
        base.dateTimePickerTextStyle.copyWith(color: labelColor);
}
