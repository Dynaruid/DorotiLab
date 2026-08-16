// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/text_theme.dart
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

public static partial class Text_themeLibrary
{
    internal static global::Doroti.Framework.Painting.TextStyle _kDefaultTextStyle = new global::Doroti.Framework.Painting.TextStyle(inherit: false, fontFamily: "CupertinoSystemText", fontSize: 17.0, letterSpacing: -0.41, color: CupertinoColors.label, decoration: TextDecoration.none);
}

public static partial class Text_themeLibrary
{
    internal static global::Doroti.Framework.Painting.TextStyle _kDefaultActionTextStyle = new global::Doroti.Framework.Painting.TextStyle(inherit: false, fontFamily: "CupertinoSystemText", fontSize: 17.0, letterSpacing: -0.41, color: CupertinoColors.activeBlue, decoration: TextDecoration.none);
}

public static partial class Text_themeLibrary
{
    internal static global::Doroti.Framework.Painting.TextStyle _kDefaultActionSmallTextStyle = new global::Doroti.Framework.Painting.TextStyle(inherit: false, fontFamily: "CupertinoSystemText", fontSize: 15.0, letterSpacing: -0.23, color: CupertinoColors.activeBlue, decoration: TextDecoration.none);
}

public static partial class Text_themeLibrary
{
    internal static global::Doroti.Framework.Painting.TextStyle _kDefaultTabLabelTextStyle = new global::Doroti.Framework.Painting.TextStyle(inherit: false, fontFamily: "CupertinoSystemText", fontSize: 10.0, fontWeight: FontWeight.w500, letterSpacing: -0.24, color: CupertinoColors.inactiveGray);
}

public static partial class Text_themeLibrary
{
    internal static global::Doroti.Framework.Painting.TextStyle _kDefaultMiddleTitleTextStyle = new global::Doroti.Framework.Painting.TextStyle(inherit: false, fontFamily: "CupertinoSystemText", fontSize: 17.0, fontWeight: FontWeight.w600, letterSpacing: -0.41, color: CupertinoColors.label);
}

public static partial class Text_themeLibrary
{
    internal static global::Doroti.Framework.Painting.TextStyle _kDefaultLargeTitleTextStyle = new global::Doroti.Framework.Painting.TextStyle(inherit: false, fontFamily: "CupertinoSystemDisplay", fontSize: 34.0, fontWeight: FontWeight.w700, letterSpacing: 0.38, color: CupertinoColors.label);
}

public static partial class Text_themeLibrary
{
    internal static global::Doroti.Framework.Painting.TextStyle _kDefaultPickerTextStyle = new global::Doroti.Framework.Painting.TextStyle(inherit: false, fontFamily: "CupertinoSystemDisplay", fontSize: 21.0, fontWeight: FontWeight.w400, letterSpacing: -0.6, color: CupertinoColors.label);
}

public static partial class Text_themeLibrary
{
    internal static global::Doroti.Framework.Painting.TextStyle _kDefaultDateTimePickerTextStyle = new global::Doroti.Framework.Painting.TextStyle(inherit: false, fontFamily: "CupertinoSystemDisplay", fontSize: 21, letterSpacing: 0.4, fontWeight: FontWeight.normal, color: CupertinoColors.label);
}

public static partial class Text_themeLibrary
{
    internal static global::Doroti.Framework.Painting.TextStyle? _resolveTextStyle(global::Doroti.Framework.Painting.TextStyle? style, global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Painting.TextStyle?)(object?)style?.copyWith(color: CupertinoDynamicColor.maybeResolve(((global::Doroti.Framework.Painting.TextStyle)style).color, context), backgroundColor: CupertinoDynamicColor.maybeResolve(((global::Doroti.Framework.Painting.TextStyle)style).backgroundColor, context), decorationColor: CupertinoDynamicColor.maybeResolve(((global::Doroti.Framework.Painting.TextStyle)style).decorationColor, context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class CupertinoTextThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    internal virtual _TextThemeDefaultsBuilder__text_theme _defaults { get; private set; } = default!;
    internal virtual Color? _primaryColor { get; private set; }
    internal virtual global::Doroti.Framework.Painting.TextStyle? _textStyle { get; private set; }
    internal virtual global::Doroti.Framework.Painting.TextStyle? _actionTextStyle { get; private set; }
    internal virtual global::Doroti.Framework.Painting.TextStyle? _actionSmallTextStyle { get; private set; }
    internal virtual global::Doroti.Framework.Painting.TextStyle? _tabLabelTextStyle { get; private set; }
    internal virtual global::Doroti.Framework.Painting.TextStyle? _navTitleTextStyle { get; private set; }
    internal virtual global::Doroti.Framework.Painting.TextStyle? _navLargeTitleTextStyle { get; private set; }
    internal virtual global::Doroti.Framework.Painting.TextStyle? _navActionTextStyle { get; private set; }
    internal virtual global::Doroti.Framework.Painting.TextStyle? _pickerTextStyle { get; private set; }
    internal virtual global::Doroti.Framework.Painting.TextStyle? _dateTimePickerTextStyle { get; private set; }

    public CupertinoTextThemeData(Color primaryColor = default!, global::Doroti.Framework.Painting.TextStyle? textStyle = null, global::Doroti.Framework.Painting.TextStyle? actionTextStyle = null, global::Doroti.Framework.Painting.TextStyle? actionSmallTextStyle = null, global::Doroti.Framework.Painting.TextStyle? tabLabelTextStyle = null, global::Doroti.Framework.Painting.TextStyle? navTitleTextStyle = null, global::Doroti.Framework.Painting.TextStyle? navLargeTitleTextStyle = null, global::Doroti.Framework.Painting.TextStyle? navActionTextStyle = null, global::Doroti.Framework.Painting.TextStyle? pickerTextStyle = null, global::Doroti.Framework.Painting.TextStyle? dateTimePickerTextStyle = null) : this(new _TextThemeDefaultsBuilder__text_theme(CupertinoColors.label, CupertinoColors.inactiveGray), primaryColor, textStyle, actionTextStyle, actionSmallTextStyle, tabLabelTextStyle, navTitleTextStyle, navLargeTitleTextStyle, navActionTextStyle, pickerTextStyle, dateTimePickerTextStyle)
    {
    }

    internal CupertinoTextThemeData(_TextThemeDefaultsBuilder__text_theme _defaults, Color? _primaryColor, global::Doroti.Framework.Painting.TextStyle? _textStyle, global::Doroti.Framework.Painting.TextStyle? _actionTextStyle, global::Doroti.Framework.Painting.TextStyle? _actionSmallTextStyle, global::Doroti.Framework.Painting.TextStyle? _tabLabelTextStyle, global::Doroti.Framework.Painting.TextStyle? _navTitleTextStyle, global::Doroti.Framework.Painting.TextStyle? _navLargeTitleTextStyle, global::Doroti.Framework.Painting.TextStyle? _navActionTextStyle, global::Doroti.Framework.Painting.TextStyle? _pickerTextStyle, global::Doroti.Framework.Painting.TextStyle? _dateTimePickerTextStyle)
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
        System.Diagnostics.Debug.Assert(((((_navActionTextStyle is not null) && (_actionTextStyle is not null))) || (_primaryColor is not null)));
    }

    public virtual global::Doroti.Framework.Painting.TextStyle textStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.TextStyle>(((this._textStyle ?? (global::Doroti.Framework.Painting.TextStyle)((_TextThemeDefaultsBuilder__text_theme)this._defaults).textStyle)));
    public virtual global::Doroti.Framework.Painting.TextStyle actionTextStyle
    {
        get
        {
            return ((this._actionTextStyle ?? (global::Doroti.Framework.Painting.TextStyle)this._defaults.actionTextStyle(primaryColor: this._primaryColor)));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Painting.TextStyle actionSmallTextStyle
    {
        get
        {
            return ((this._actionSmallTextStyle ?? (global::Doroti.Framework.Painting.TextStyle)this._defaults.actionSmallTextStyle(primaryColor: this._primaryColor)));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Painting.TextStyle tabLabelTextStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.TextStyle>(((this._tabLabelTextStyle ?? (global::Doroti.Framework.Painting.TextStyle)((_TextThemeDefaultsBuilder__text_theme)this._defaults).tabLabelTextStyle)));
    public virtual global::Doroti.Framework.Painting.TextStyle navTitleTextStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.TextStyle>(((this._navTitleTextStyle ?? (global::Doroti.Framework.Painting.TextStyle)((_TextThemeDefaultsBuilder__text_theme)this._defaults).navTitleTextStyle)));
    public virtual global::Doroti.Framework.Painting.TextStyle navLargeTitleTextStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.TextStyle>(((this._navLargeTitleTextStyle ?? (global::Doroti.Framework.Painting.TextStyle)((_TextThemeDefaultsBuilder__text_theme)this._defaults).navLargeTitleTextStyle)));
    public virtual global::Doroti.Framework.Painting.TextStyle navActionTextStyle
    {
        get
        {
            return ((this._navActionTextStyle ?? (global::Doroti.Framework.Painting.TextStyle)this._defaults.navActionTextStyle(primaryColor: this._primaryColor)));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Painting.TextStyle pickerTextStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.TextStyle>(((this._pickerTextStyle ?? (global::Doroti.Framework.Painting.TextStyle)((_TextThemeDefaultsBuilder__text_theme)this._defaults).pickerTextStyle)));
    public virtual global::Doroti.Framework.Painting.TextStyle dateTimePickerTextStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.TextStyle>(((this._dateTimePickerTextStyle ?? (global::Doroti.Framework.Painting.TextStyle)((_TextThemeDefaultsBuilder__text_theme)this._defaults).dateTimePickerTextStyle)));
    public virtual CupertinoTextThemeData resolveFrom(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new CupertinoTextThemeData(this._defaults.resolveFrom(context), CupertinoDynamicColor.maybeResolve(this._primaryColor, context), Text_themeLibrary._resolveTextStyle(this._textStyle, context), Text_themeLibrary._resolveTextStyle(this._actionTextStyle, context), Text_themeLibrary._resolveTextStyle(this._actionSmallTextStyle, context), Text_themeLibrary._resolveTextStyle(this._tabLabelTextStyle, context), Text_themeLibrary._resolveTextStyle(this._navTitleTextStyle, context), Text_themeLibrary._resolveTextStyle(this._navLargeTitleTextStyle, context), Text_themeLibrary._resolveTextStyle(this._navActionTextStyle, context), Text_themeLibrary._resolveTextStyle(this._pickerTextStyle, context), Text_themeLibrary._resolveTextStyle(this._dateTimePickerTextStyle, context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual CupertinoTextThemeData copyWith(Color? primaryColor = null, global::Doroti.Framework.Painting.TextStyle? textStyle = null, global::Doroti.Framework.Painting.TextStyle? actionTextStyle = null, global::Doroti.Framework.Painting.TextStyle? actionSmallTextStyle = null, global::Doroti.Framework.Painting.TextStyle? tabLabelTextStyle = null, global::Doroti.Framework.Painting.TextStyle? navTitleTextStyle = null, global::Doroti.Framework.Painting.TextStyle? navLargeTitleTextStyle = null, global::Doroti.Framework.Painting.TextStyle? navActionTextStyle = null, global::Doroti.Framework.Painting.TextStyle? pickerTextStyle = null, global::Doroti.Framework.Painting.TextStyle? dateTimePickerTextStyle = null)
    {
        return new CupertinoTextThemeData(this._defaults, (primaryColor ?? this._primaryColor), (textStyle ?? this._textStyle), (actionTextStyle ?? this._actionTextStyle), (actionSmallTextStyle ?? this._actionSmallTextStyle), (tabLabelTextStyle ?? this._tabLabelTextStyle), (navTitleTextStyle ?? this._navTitleTextStyle), (navLargeTitleTextStyle ?? this._navLargeTitleTextStyle), (navActionTextStyle ?? this._navActionTextStyle), (pickerTextStyle ?? this._pickerTextStyle), (dateTimePickerTextStyle ?? this._dateTimePickerTextStyle));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        var defaultData__10638 = new CupertinoTextThemeData();
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("textStyle", this.textStyle, defaultValue: ((CupertinoTextThemeData)defaultData__10638).textStyle));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("actionTextStyle", this.actionTextStyle, defaultValue: ((CupertinoTextThemeData)defaultData__10638).actionTextStyle));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("actionSmallTextStyle", this.actionSmallTextStyle, defaultValue: ((CupertinoTextThemeData)defaultData__10638).actionSmallTextStyle));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("tabLabelTextStyle", this.tabLabelTextStyle, defaultValue: ((CupertinoTextThemeData)defaultData__10638).tabLabelTextStyle));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("navTitleTextStyle", this.navTitleTextStyle, defaultValue: ((CupertinoTextThemeData)defaultData__10638).navTitleTextStyle));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("navLargeTitleTextStyle", this.navLargeTitleTextStyle, defaultValue: ((CupertinoTextThemeData)defaultData__10638).navLargeTitleTextStyle));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("navActionTextStyle", this.navActionTextStyle, defaultValue: ((CupertinoTextThemeData)defaultData__10638).navActionTextStyle));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("pickerTextStyle", this.pickerTextStyle, defaultValue: ((CupertinoTextThemeData)defaultData__10638).pickerTextStyle));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("dateTimePickerTextStyle", this.dateTimePickerTextStyle, defaultValue: ((CupertinoTextThemeData)defaultData__10638).dateTimePickerTextStyle));
    }

    public override bool Equals(object? other)
    {
        var __other = other as CupertinoTextThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((((((__other is CupertinoTextThemeData) && (object.Equals(((CupertinoTextThemeData)((CupertinoTextThemeData)__other))._defaults, this._defaults))) && (object.Equals(((CupertinoTextThemeData)((CupertinoTextThemeData)__other))._primaryColor, this._primaryColor))) && (object.Equals(((CupertinoTextThemeData)((CupertinoTextThemeData)__other))._textStyle, this._textStyle))) && (object.Equals(((CupertinoTextThemeData)((CupertinoTextThemeData)__other))._actionTextStyle, this._actionTextStyle))) && (object.Equals(((CupertinoTextThemeData)((CupertinoTextThemeData)__other))._actionSmallTextStyle, this._actionSmallTextStyle))) && (object.Equals(((CupertinoTextThemeData)((CupertinoTextThemeData)__other))._tabLabelTextStyle, this._tabLabelTextStyle))) && (object.Equals(((CupertinoTextThemeData)((CupertinoTextThemeData)__other))._navTitleTextStyle, this._navTitleTextStyle))) && (object.Equals(((CupertinoTextThemeData)((CupertinoTextThemeData)__other))._navLargeTitleTextStyle, this._navLargeTitleTextStyle))) && (object.Equals(((CupertinoTextThemeData)((CupertinoTextThemeData)__other))._navActionTextStyle, this._navActionTextStyle))) && (object.Equals(((CupertinoTextThemeData)((CupertinoTextThemeData)__other))._pickerTextStyle, this._pickerTextStyle))) && (object.Equals(((CupertinoTextThemeData)((CupertinoTextThemeData)__other))._dateTimePickerTextStyle, this._dateTimePickerTextStyle)));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this._defaults, this._primaryColor, this._textStyle, this._actionTextStyle, this._actionSmallTextStyle, this._tabLabelTextStyle, this._navTitleTextStyle, this._navLargeTitleTextStyle, this._navActionTextStyle, this._pickerTextStyle, this._dateTimePickerTextStyle));
    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return ((fullString__105654 ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
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

    internal static global::Doroti.Framework.Painting.TextStyle _applyLabelColor(global::Doroti.Framework.Painting.TextStyle original, Color color)
    {
        return ((object.Equals(((global::Doroti.Framework.Painting.TextStyle)original).color, color)) ? original : original.copyWith(color: color));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Painting.TextStyle textStyle => _TextThemeDefaultsBuilder__text_theme._applyLabelColor(Text_themeLibrary._kDefaultTextStyle, this.labelColor);
    public virtual global::Doroti.Framework.Painting.TextStyle tabLabelTextStyle => _TextThemeDefaultsBuilder__text_theme._applyLabelColor(Text_themeLibrary._kDefaultTabLabelTextStyle, this.inactiveGrayColor);
    public virtual global::Doroti.Framework.Painting.TextStyle navTitleTextStyle => _TextThemeDefaultsBuilder__text_theme._applyLabelColor(Text_themeLibrary._kDefaultMiddleTitleTextStyle, this.labelColor);
    public virtual global::Doroti.Framework.Painting.TextStyle navLargeTitleTextStyle => _TextThemeDefaultsBuilder__text_theme._applyLabelColor(Text_themeLibrary._kDefaultLargeTitleTextStyle, this.labelColor);
    public virtual global::Doroti.Framework.Painting.TextStyle pickerTextStyle => _TextThemeDefaultsBuilder__text_theme._applyLabelColor(Text_themeLibrary._kDefaultPickerTextStyle, this.labelColor);
    public virtual global::Doroti.Framework.Painting.TextStyle dateTimePickerTextStyle => _TextThemeDefaultsBuilder__text_theme._applyLabelColor(Text_themeLibrary._kDefaultDateTimePickerTextStyle, this.labelColor);
    public virtual global::Doroti.Framework.Painting.TextStyle actionTextStyle(Color? primaryColor = null) => Text_themeLibrary._kDefaultActionTextStyle.copyWith(color: primaryColor);
    public virtual global::Doroti.Framework.Painting.TextStyle actionSmallTextStyle(Color? primaryColor = null) => Text_themeLibrary._kDefaultActionSmallTextStyle.copyWith(color: primaryColor);
    public virtual global::Doroti.Framework.Painting.TextStyle navActionTextStyle(Color? primaryColor = null) => actionTextStyle(primaryColor: primaryColor);
    public virtual _TextThemeDefaultsBuilder__text_theme resolveFrom(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color resolvedLabelColor__14823 = ((global::Doroti.Ui.Color)(object?)CupertinoDynamicColor.resolve(this.labelColor, context));
        global::Doroti.Ui.Color resolvedInactiveGray__14912 = ((global::Doroti.Ui.Color)(object?)CupertinoDynamicColor.resolve(this.inactiveGrayColor, context));
        return (((object.Equals(resolvedLabelColor__14823, this.labelColor)) && (object.Equals(resolvedInactiveGray__14912, CupertinoColors.inactiveGray))) ? this : new _TextThemeDefaultsBuilder__text_theme(resolvedLabelColor__14823, resolvedInactiveGray__14912));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as _TextThemeDefaultsBuilder__text_theme;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is _TextThemeDefaultsBuilder__text_theme) && (object.Equals(((_TextThemeDefaultsBuilder__text_theme)((_TextThemeDefaultsBuilder__text_theme)__other)).labelColor, this.labelColor))) && (object.Equals(((_TextThemeDefaultsBuilder__text_theme)((_TextThemeDefaultsBuilder__text_theme)__other)).inactiveGrayColor, this.inactiveGrayColor)));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.labelColor, this.inactiveGrayColor));
}
