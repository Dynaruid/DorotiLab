// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/text_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class Text_themeLibrary
{
    internal static TextStyle _kDefaultTextStyle = new TextStyle(
        inherit: false,
        fontFamily: "CupertinoSystemText",
        fontSize: 17.0,
        letterSpacing: -0.41,
        color: CupertinoColors.label,
        decoration: TextDecoration.none
    );
}

public static partial class Text_themeLibrary
{
    internal static TextStyle _kDefaultActionTextStyle = new TextStyle(
        inherit: false,
        fontFamily: "CupertinoSystemText",
        fontSize: 17.0,
        letterSpacing: -0.41,
        color: CupertinoColors.activeBlue,
        decoration: TextDecoration.none
    );
}

public static partial class Text_themeLibrary
{
    internal static TextStyle _kDefaultActionSmallTextStyle = new TextStyle(
        inherit: false,
        fontFamily: "CupertinoSystemText",
        fontSize: 15.0,
        letterSpacing: -0.23,
        color: CupertinoColors.activeBlue,
        decoration: TextDecoration.none
    );
}

public static partial class Text_themeLibrary
{
    internal static TextStyle _kDefaultTabLabelTextStyle = new TextStyle(
        inherit: false,
        fontFamily: "CupertinoSystemText",
        fontSize: 10.0,
        fontWeight: FontWeight.w500,
        letterSpacing: -0.24,
        color: CupertinoColors.inactiveGray
    );
}

public static partial class Text_themeLibrary
{
    internal static TextStyle _kDefaultMiddleTitleTextStyle = new TextStyle(
        inherit: false,
        fontFamily: "CupertinoSystemText",
        fontSize: 17.0,
        fontWeight: FontWeight.w600,
        letterSpacing: -0.41,
        color: CupertinoColors.label
    );
}

public static partial class Text_themeLibrary
{
    internal static TextStyle _kDefaultLargeTitleTextStyle = new TextStyle(
        inherit: false,
        fontFamily: "CupertinoSystemDisplay",
        fontSize: 34.0,
        fontWeight: FontWeight.w700,
        letterSpacing: 0.38,
        color: CupertinoColors.label
    );
}

public static partial class Text_themeLibrary
{
    internal static TextStyle _kDefaultPickerTextStyle = new TextStyle(
        inherit: false,
        fontFamily: "CupertinoSystemDisplay",
        fontSize: 21.0,
        fontWeight: FontWeight.w400,
        letterSpacing: -0.6,
        color: CupertinoColors.label
    );
}

public static partial class Text_themeLibrary
{
    internal static TextStyle _kDefaultDateTimePickerTextStyle = new TextStyle(
        inherit: false,
        fontFamily: "CupertinoSystemDisplay",
        fontSize: 21,
        letterSpacing: 0.4,
        fontWeight: FontWeight.normal,
        color: CupertinoColors.label
    );
}

public static partial class Text_themeLibrary
{
    internal static TextStyle? _resolveTextStyle(TextStyle? style, BuildContext context)
    {
        return style?.copyWith(
            color: CupertinoDynamicColor.maybeResolve(style.color, context),
            backgroundColor: CupertinoDynamicColor.maybeResolve(style.backgroundColor, context),
            decorationColor: CupertinoDynamicColor.maybeResolve(style.decorationColor, context)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class CupertinoTextThemeData : Diagnosticable
{
    internal virtual _TextThemeDefaultsBuilder__text_theme _defaults { get; private set; } =
        default!;
    internal virtual Color? _primaryColor { get; private set; }
    internal virtual TextStyle? _textStyle { get; private set; }
    internal virtual TextStyle? _actionTextStyle { get; private set; }
    internal virtual TextStyle? _actionSmallTextStyle { get; private set; }
    internal virtual TextStyle? _tabLabelTextStyle { get; private set; }
    internal virtual TextStyle? _navTitleTextStyle { get; private set; }
    internal virtual TextStyle? _navLargeTitleTextStyle { get; private set; }
    internal virtual TextStyle? _navActionTextStyle { get; private set; }
    internal virtual TextStyle? _pickerTextStyle { get; private set; }
    internal virtual TextStyle? _dateTimePickerTextStyle { get; private set; }

    public CupertinoTextThemeData(
        Color primaryColor = default!,
        TextStyle? textStyle = null,
        TextStyle? actionTextStyle = null,
        TextStyle? actionSmallTextStyle = null,
        TextStyle? tabLabelTextStyle = null,
        TextStyle? navTitleTextStyle = null,
        TextStyle? navLargeTitleTextStyle = null,
        TextStyle? navActionTextStyle = null,
        TextStyle? pickerTextStyle = null,
        TextStyle? dateTimePickerTextStyle = null
    )
        : this(
            new _TextThemeDefaultsBuilder__text_theme(
                CupertinoColors.label,
                CupertinoColors.inactiveGray
            ),
            primaryColor,
            textStyle,
            actionTextStyle,
            actionSmallTextStyle,
            tabLabelTextStyle,
            navTitleTextStyle,
            navLargeTitleTextStyle,
            navActionTextStyle,
            pickerTextStyle,
            dateTimePickerTextStyle
        ) { }

    internal CupertinoTextThemeData(
        _TextThemeDefaultsBuilder__text_theme _defaults,
        Color? _primaryColor,
        TextStyle? _textStyle,
        TextStyle? _actionTextStyle,
        TextStyle? _actionSmallTextStyle,
        TextStyle? _tabLabelTextStyle,
        TextStyle? _navTitleTextStyle,
        TextStyle? _navLargeTitleTextStyle,
        TextStyle? _navActionTextStyle,
        TextStyle? _pickerTextStyle,
        TextStyle? _dateTimePickerTextStyle
    )
    {
        this._defaults = _defaults;
        this._primaryColor = _primaryColor;
        this._textStyle = _textStyle;
        this._actionTextStyle = _actionTextStyle;
        this._actionSmallTextStyle = _actionSmallTextStyle;
        this._tabLabelTextStyle = _tabLabelTextStyle;
        this._navTitleTextStyle = _navTitleTextStyle;
        this._navLargeTitleTextStyle = _navLargeTitleTextStyle;
        this._navActionTextStyle = _navActionTextStyle;
        this._pickerTextStyle = _pickerTextStyle;
        this._dateTimePickerTextStyle = _dateTimePickerTextStyle;
        System.Diagnostics.Debug.Assert(
            ((_navActionTextStyle is not null) && (_actionTextStyle is not null))
                || (_primaryColor is not null)
        );
    }

    public virtual TextStyle textStyle =>
        DartRuntimePrimitives.ConvertValue<TextStyle>(_textStyle ?? _defaults.textStyle);
    public virtual TextStyle actionTextStyle
    {
        get { return _actionTextStyle ?? _defaults.actionTextStyle(primaryColor: _primaryColor); }
    }
    public virtual TextStyle actionSmallTextStyle
    {
        get
        {
            return _actionSmallTextStyle
                ?? _defaults.actionSmallTextStyle(primaryColor: _primaryColor);
        }
    }
    public virtual TextStyle tabLabelTextStyle =>
        DartRuntimePrimitives.ConvertValue<TextStyle>(
            _tabLabelTextStyle ?? _defaults.tabLabelTextStyle
        );
    public virtual TextStyle navTitleTextStyle =>
        DartRuntimePrimitives.ConvertValue<TextStyle>(
            _navTitleTextStyle ?? _defaults.navTitleTextStyle
        );
    public virtual TextStyle navLargeTitleTextStyle =>
        DartRuntimePrimitives.ConvertValue<TextStyle>(
            _navLargeTitleTextStyle ?? _defaults.navLargeTitleTextStyle
        );
    public virtual TextStyle navActionTextStyle
    {
        get
        {
            return _navActionTextStyle ?? _defaults.navActionTextStyle(primaryColor: _primaryColor);
        }
    }
    public virtual TextStyle pickerTextStyle =>
        DartRuntimePrimitives.ConvertValue<TextStyle>(
            _pickerTextStyle ?? _defaults.pickerTextStyle
        );
    public virtual TextStyle dateTimePickerTextStyle =>
        DartRuntimePrimitives.ConvertValue<TextStyle>(
            _dateTimePickerTextStyle ?? _defaults.dateTimePickerTextStyle
        );

    public virtual CupertinoTextThemeData resolveFrom(BuildContext context)
    {
        return new CupertinoTextThemeData(
            _defaults.resolveFrom(context),
            CupertinoDynamicColor.maybeResolve(_primaryColor, context),
            Text_themeLibrary._resolveTextStyle(_textStyle, context),
            Text_themeLibrary._resolveTextStyle(_actionTextStyle, context),
            Text_themeLibrary._resolveTextStyle(_actionSmallTextStyle, context),
            Text_themeLibrary._resolveTextStyle(_tabLabelTextStyle, context),
            Text_themeLibrary._resolveTextStyle(_navTitleTextStyle, context),
            Text_themeLibrary._resolveTextStyle(_navLargeTitleTextStyle, context),
            Text_themeLibrary._resolveTextStyle(_navActionTextStyle, context),
            Text_themeLibrary._resolveTextStyle(_pickerTextStyle, context),
            Text_themeLibrary._resolveTextStyle(_dateTimePickerTextStyle, context)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual CupertinoTextThemeData copyWith(
        Color? primaryColor = null,
        TextStyle? textStyle = null,
        TextStyle? actionTextStyle = null,
        TextStyle? actionSmallTextStyle = null,
        TextStyle? tabLabelTextStyle = null,
        TextStyle? navTitleTextStyle = null,
        TextStyle? navLargeTitleTextStyle = null,
        TextStyle? navActionTextStyle = null,
        TextStyle? pickerTextStyle = null,
        TextStyle? dateTimePickerTextStyle = null
    )
    {
        return new CupertinoTextThemeData(
            _defaults,
            primaryColor ?? _primaryColor,
            textStyle ?? _textStyle,
            actionTextStyle ?? _actionTextStyle,
            actionSmallTextStyle ?? _actionSmallTextStyle,
            tabLabelTextStyle ?? _tabLabelTextStyle,
            navTitleTextStyle ?? _navTitleTextStyle,
            navLargeTitleTextStyle ?? _navLargeTitleTextStyle,
            navActionTextStyle ?? _navActionTextStyle,
            pickerTextStyle ?? _pickerTextStyle,
            dateTimePickerTextStyle ?? _dateTimePickerTextStyle
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        var defaultData = new CupertinoTextThemeData();
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "textStyle",
                textStyle,
                defaultValue: defaultData.textStyle
            )
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "actionTextStyle",
                actionTextStyle,
                defaultValue: defaultData.actionTextStyle
            )
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "actionSmallTextStyle",
                actionSmallTextStyle,
                defaultValue: defaultData.actionSmallTextStyle
            )
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "tabLabelTextStyle",
                tabLabelTextStyle,
                defaultValue: defaultData.tabLabelTextStyle
            )
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "navTitleTextStyle",
                navTitleTextStyle,
                defaultValue: defaultData.navTitleTextStyle
            )
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "navLargeTitleTextStyle",
                navLargeTitleTextStyle,
                defaultValue: defaultData.navLargeTitleTextStyle
            )
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "navActionTextStyle",
                navActionTextStyle,
                defaultValue: defaultData.navActionTextStyle
            )
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "pickerTextStyle",
                pickerTextStyle,
                defaultValue: defaultData.pickerTextStyle
            )
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "dateTimePickerTextStyle",
                dateTimePickerTextStyle,
                defaultValue: defaultData.dateTimePickerTextStyle
            )
        );
    }

    public override bool Equals(object? other)
    {
        var __other = other as CupertinoTextThemeData;
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
        return (__other is CupertinoTextThemeData)
            && Equals(__other._defaults, _defaults)
            && Equals(__other._primaryColor, _primaryColor)
            && Equals(__other._textStyle, _textStyle)
            && Equals(__other._actionTextStyle, _actionTextStyle)
            && Equals(__other._actionSmallTextStyle, _actionSmallTextStyle)
            && Equals(__other._tabLabelTextStyle, _tabLabelTextStyle)
            && Equals(__other._navTitleTextStyle, _navTitleTextStyle)
            && Equals(__other._navLargeTitleTextStyle, _navLargeTitleTextStyle)
            && Equals(__other._navActionTextStyle, _navActionTextStyle)
            && Equals(__other._pickerTextStyle, _pickerTextStyle)
            && Equals(__other._dateTimePickerTextStyle, _dateTimePickerTextStyle);
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(
                _defaults,
                _primaryColor,
                _textStyle,
                _actionTextStyle,
                _actionSmallTextStyle,
                _tabLabelTextStyle,
                _navTitleTextStyle,
                _navLargeTitleTextStyle,
                _navActionTextStyle,
                _pickerTextStyle,
                _dateTimePickerTextStyle
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

internal class _TextThemeDefaultsBuilder__text_theme
{
    public virtual Color labelColor { get; private set; } = default!;
    public virtual Color inactiveGrayColor { get; private set; } = default!;

    internal _TextThemeDefaultsBuilder__text_theme(Color labelColor, Color inactiveGrayColor)
    {
        this.labelColor = labelColor;
        this.inactiveGrayColor = inactiveGrayColor;
    }

    internal static TextStyle _applyLabelColor(TextStyle original, Color color)
    {
        return Equals(original.color, color) ? original : original.copyWith(color: color);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual TextStyle textStyle =>
        _applyLabelColor(Text_themeLibrary._kDefaultTextStyle, labelColor);
    public virtual TextStyle tabLabelTextStyle =>
        _applyLabelColor(Text_themeLibrary._kDefaultTabLabelTextStyle, inactiveGrayColor);
    public virtual TextStyle navTitleTextStyle =>
        _applyLabelColor(Text_themeLibrary._kDefaultMiddleTitleTextStyle, labelColor);
    public virtual TextStyle navLargeTitleTextStyle =>
        _applyLabelColor(Text_themeLibrary._kDefaultLargeTitleTextStyle, labelColor);
    public virtual TextStyle pickerTextStyle =>
        _applyLabelColor(Text_themeLibrary._kDefaultPickerTextStyle, labelColor);
    public virtual TextStyle dateTimePickerTextStyle =>
        _applyLabelColor(Text_themeLibrary._kDefaultDateTimePickerTextStyle, labelColor);

    public virtual TextStyle actionTextStyle(Color? primaryColor = null) =>
        Text_themeLibrary._kDefaultActionTextStyle.copyWith(color: primaryColor);

    public virtual TextStyle actionSmallTextStyle(Color? primaryColor = null) =>
        Text_themeLibrary._kDefaultActionSmallTextStyle.copyWith(color: primaryColor);

    public virtual TextStyle navActionTextStyle(Color? primaryColor = null) =>
        actionTextStyle(primaryColor: primaryColor);

    public virtual _TextThemeDefaultsBuilder__text_theme resolveFrom(BuildContext context)
    {
        Color resolvedLabelColor = CupertinoDynamicColor.resolve(labelColor, context);
        Color resolvedInactiveGray = CupertinoDynamicColor.resolve(inactiveGrayColor, context);
        return (
            Equals(resolvedLabelColor, labelColor)
            && Equals(resolvedInactiveGray, CupertinoColors.inactiveGray)
        )
            ? this
            : new _TextThemeDefaultsBuilder__text_theme(resolvedLabelColor, resolvedInactiveGray);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as _TextThemeDefaultsBuilder__text_theme;
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
        return (__other is _TextThemeDefaultsBuilder__text_theme)
            && Equals(__other.labelColor, labelColor)
            && Equals(__other.inactiveGrayColor, inactiveGrayColor);
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(labelColor, inactiveGrayColor)
        );
}
