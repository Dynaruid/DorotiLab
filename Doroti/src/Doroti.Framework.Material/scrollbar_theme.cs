// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/scrollbar_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class ScrollbarThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<bool?>? thumbVisibility { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? thickness { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<bool?>? trackVisibility { get; private set; }
    public virtual bool? interactive { get; private set; }
    public virtual Radius? radius { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? thumbColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? trackColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? trackBorderColor { get; private set; }
    public virtual double? crossAxisMargin { get; private set; }
    public virtual double? mainAxisMargin { get; private set; }
    public virtual double? minThumbLength { get; private set; }

    public ScrollbarThemeData(global::Doroti.Framework.Widgets.WidgetStateProperty<bool?>? thumbVisibility = null, global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? thickness = null, global::Doroti.Framework.Widgets.WidgetStateProperty<bool?>? trackVisibility = null, Radius? radius = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? thumbColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? trackColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? trackBorderColor = null, double? crossAxisMargin = null, double? mainAxisMargin = null, double? minThumbLength = null, bool? interactive = null)
    {
        this.thumbVisibility = thumbVisibility;
        this.thickness = thickness;
        this.trackVisibility = trackVisibility;
        this.radius = radius;
        this.thumbColor = thumbColor;
        this.trackColor = trackColor;
        this.trackBorderColor = trackBorderColor;
        this.crossAxisMargin = crossAxisMargin;
        this.mainAxisMargin = mainAxisMargin;
        this.minThumbLength = minThumbLength;
        this.interactive = interactive;
    }

    public virtual ScrollbarThemeData copyWith(global::Doroti.Framework.Widgets.WidgetStateProperty<bool?>? thumbVisibility = null, global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? thickness = null, global::Doroti.Framework.Widgets.WidgetStateProperty<bool?>? trackVisibility = null, bool? interactive = null, Radius? radius = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? thumbColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? trackColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? trackBorderColor = null, double? crossAxisMargin = null, double? mainAxisMargin = null, double? minThumbLength = null)
    {
        return new ScrollbarThemeData(thumbVisibility: (thumbVisibility ?? this.thumbVisibility), thickness: (thickness ?? this.thickness), trackVisibility: (trackVisibility ?? this.trackVisibility), interactive: (interactive ?? this.interactive), radius: (radius ?? this.radius), thumbColor: (thumbColor ?? this.thumbColor), trackColor: (trackColor ?? this.trackColor), trackBorderColor: (trackBorderColor ?? this.trackBorderColor), crossAxisMargin: (crossAxisMargin ?? this.crossAxisMargin), mainAxisMargin: (mainAxisMargin ?? this.mainAxisMargin), minThumbLength: (minThumbLength ?? this.minThumbLength));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ScrollbarThemeData lerp(ScrollbarThemeData? a, ScrollbarThemeData? b, double t)
    {
        if ((DartRuntimePrimitives.Identical(a, b) && (a is not null)))
        {
            return a;
        }
        return new ScrollbarThemeData(thumbVisibility: WidgetStateProperty.lerp<bool?>(a?.thumbVisibility, b?.thumbVisibility, t, (global::System.Func<bool?, bool?, double, bool?>)Scrollbar_themeLibrary._lerpBool), thickness: WidgetStateProperty.lerp<double?>(a?.thickness, b?.thickness, t, (global::System.Func<double?, double?, double, double?>)Dart_uiLibrary.lerpDouble), trackVisibility: WidgetStateProperty.lerp<bool?>(a?.trackVisibility, b?.trackVisibility, t, (global::System.Func<bool?, bool?, double, bool?>)Scrollbar_themeLibrary._lerpBool), interactive: Scrollbar_themeLibrary._lerpBool(a?.interactive, b?.interactive, t), radius: Dart_uiLibrary.Radius.lerp(a?.radius, b?.radius, t), thumbColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.thumbColor, b?.thumbColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), trackColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.trackColor, b?.trackColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), trackBorderColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.trackBorderColor, b?.trackBorderColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), crossAxisMargin: Dart_uiLibrary.lerpDouble(a?.crossAxisMargin, b?.crossAxisMargin, t), mainAxisMargin: Dart_uiLibrary.lerpDouble(a?.mainAxisMargin, b?.mainAxisMargin, t), minThumbLength: Dart_uiLibrary.lerpDouble(a?.minThumbLength, b?.minThumbLength, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.thumbVisibility, this.thickness, this.trackVisibility, this.interactive, this.radius, this.thumbColor, this.trackColor, this.trackBorderColor, this.crossAxisMargin, this.mainAxisMargin, this.minThumbLength));
    public override bool Equals(object? other)
    {
        var __other = other as ScrollbarThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((((((__other is ScrollbarThemeData) && (Equals(((ScrollbarThemeData)((ScrollbarThemeData)__other)).thumbVisibility, this.thumbVisibility))) && (Equals(((ScrollbarThemeData)((ScrollbarThemeData)__other)).thickness, this.thickness))) && (Equals(((ScrollbarThemeData)((ScrollbarThemeData)__other)).trackVisibility, this.trackVisibility))) && (((ScrollbarThemeData)((ScrollbarThemeData)__other)).interactive == this.interactive)) && (Equals(((ScrollbarThemeData)((ScrollbarThemeData)__other)).radius, this.radius))) && (Equals(((ScrollbarThemeData)((ScrollbarThemeData)__other)).thumbColor, this.thumbColor))) && (Equals(((ScrollbarThemeData)((ScrollbarThemeData)__other)).trackColor, this.trackColor))) && (Equals(((ScrollbarThemeData)((ScrollbarThemeData)__other)).trackBorderColor, this.trackBorderColor))) && (((ScrollbarThemeData)((ScrollbarThemeData)__other)).crossAxisMargin == this.crossAxisMargin)) && (((ScrollbarThemeData)((ScrollbarThemeData)__other)).mainAxisMargin == this.mainAxisMargin)) && (((ScrollbarThemeData)((ScrollbarThemeData)__other)).minThumbLength == this.minThumbLength));
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<bool?>>("thumbVisibility", this.thumbVisibility, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<double?>>("thickness", this.thickness, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<bool?>>("trackVisibility", this.trackVisibility, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("interactive", this.interactive, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Radius>("radius", this.radius, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("thumbColor", this.thumbColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("trackColor", this.trackColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("trackBorderColor", this.trackBorderColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<double>("crossAxisMargin", this.crossAxisMargin, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<double>("mainAxisMargin", this.mainAxisMargin, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<double>("minThumbLength", this.minThumbLength, defaultValue: null));
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);
    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return ((fullString ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Scrollbar_themeLibrary
{
    internal static bool? _lerpBool(bool? a, bool? b, double t) => ((t < 0.5) ? a : b);
}

public class ScrollbarTheme : global::Doroti.Framework.Widgets.InheritedTheme
{
    public virtual ScrollbarThemeData data { get; private set; } = default!;

    public ScrollbarTheme(global::Doroti.Framework.Foundation.Key? key = null, ScrollbarThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static ScrollbarThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ScrollbarTheme? scrollbarThemeLocal = ((ScrollbarTheme?)context.dependOnInheritedWidgetOfExactType<ScrollbarTheme>());
        return (scrollbarThemeLocal?.data ?? Theme.of(context).scrollbarTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)new ScrollbarTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!Equals(this.data, ((ScrollbarTheme)oldWidget).data)));
}
