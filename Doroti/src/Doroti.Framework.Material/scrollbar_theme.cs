// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/scrollbar_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class ScrollbarThemeData : Diagnosticable
{
    public virtual WidgetStateProperty<bool?>? thumbVisibility { get; private set; }
    public virtual WidgetStateProperty<double?>? thickness { get; private set; }
    public virtual WidgetStateProperty<bool?>? trackVisibility { get; private set; }
    public virtual bool? interactive { get; private set; }
    public virtual Radius? radius { get; private set; }
    public virtual WidgetStateProperty<Color?>? thumbColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? trackColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? trackBorderColor { get; private set; }
    public virtual double? crossAxisMargin { get; private set; }
    public virtual double? mainAxisMargin { get; private set; }
    public virtual double? minThumbLength { get; private set; }

    public ScrollbarThemeData(WidgetStateProperty<bool?>? thumbVisibility = null, WidgetStateProperty<double?>? thickness = null, WidgetStateProperty<bool?>? trackVisibility = null, Radius? radius = null, WidgetStateProperty<Color?>? thumbColor = null, WidgetStateProperty<Color?>? trackColor = null, WidgetStateProperty<Color?>? trackBorderColor = null, double? crossAxisMargin = null, double? mainAxisMargin = null, double? minThumbLength = null, bool? interactive = null)
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

    public virtual ScrollbarThemeData copyWith(WidgetStateProperty<bool?>? thumbVisibility = null, WidgetStateProperty<double?>? thickness = null, WidgetStateProperty<bool?>? trackVisibility = null, bool? interactive = null, Radius? radius = null, WidgetStateProperty<Color?>? thumbColor = null, WidgetStateProperty<Color?>? trackColor = null, WidgetStateProperty<Color?>? trackBorderColor = null, double? crossAxisMargin = null, double? mainAxisMargin = null, double? minThumbLength = null)
    {
        return new ScrollbarThemeData(thumbVisibility: thumbVisibility ?? this.thumbVisibility, thickness: thickness ?? this.thickness, trackVisibility: trackVisibility ?? this.trackVisibility, interactive: interactive ?? this.interactive, radius: radius ?? this.radius, thumbColor: thumbColor ?? this.thumbColor, trackColor: trackColor ?? this.trackColor, trackBorderColor: trackBorderColor ?? this.trackBorderColor, crossAxisMargin: crossAxisMargin ?? this.crossAxisMargin, mainAxisMargin: mainAxisMargin ?? this.mainAxisMargin, minThumbLength: minThumbLength ?? this.minThumbLength);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ScrollbarThemeData lerp(ScrollbarThemeData? a, ScrollbarThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b) && (a is not null))
        {
            return a;
        }
        return new ScrollbarThemeData(thumbVisibility: WidgetStateProperty.lerp(a?.thumbVisibility, b?.thumbVisibility, t, Scrollbar_themeLibrary._lerpBool), thickness: WidgetStateProperty.lerp(a?.thickness, b?.thickness, t, Dart_uiLibrary.lerpDouble), trackVisibility: WidgetStateProperty.lerp(a?.trackVisibility, b?.trackVisibility, t, Scrollbar_themeLibrary._lerpBool), interactive: Scrollbar_themeLibrary._lerpBool(a?.interactive, b?.interactive, t), radius: Dart_uiLibrary.Radius.lerp(a?.radius, b?.radius, t), thumbColor: WidgetStateProperty.lerp(a?.thumbColor, b?.thumbColor, t, Color.lerp), trackColor: WidgetStateProperty.lerp(a?.trackColor, b?.trackColor, t, Color.lerp), trackBorderColor: WidgetStateProperty.lerp(a?.trackBorderColor, b?.trackBorderColor, t, Color.lerp), crossAxisMargin: Dart_uiLibrary.lerpDouble(a?.crossAxisMargin, b?.crossAxisMargin, t), mainAxisMargin: Dart_uiLibrary.lerpDouble(a?.mainAxisMargin, b?.mainAxisMargin, t), minThumbLength: Dart_uiLibrary.lerpDouble(a?.minThumbLength, b?.minThumbLength, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(thumbVisibility, thickness, trackVisibility, interactive, radius, thumbColor, trackColor, trackBorderColor, crossAxisMargin, mainAxisMargin, minThumbLength));
    public override bool Equals(object? other)
    {
        var __other = other as ScrollbarThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is ScrollbarThemeData) && Equals(__other.thumbVisibility, thumbVisibility) && Equals(__other.thickness, thickness) && Equals(__other.trackVisibility, trackVisibility) && (__other.interactive == interactive) && Equals(__other.radius, radius) && Equals(__other.thumbColor, thumbColor) && Equals(__other.trackColor, trackColor) && Equals(__other.trackBorderColor, trackBorderColor) && (__other.crossAxisMargin == crossAxisMargin) && (__other.mainAxisMargin == mainAxisMargin) && (__other.minThumbLength == minThumbLength);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(new DiagnosticsProperty<WidgetStateProperty<bool?>>("thumbVisibility", thumbVisibility, defaultValue: null));
        properties.add(new DiagnosticsProperty<WidgetStateProperty<double?>>("thickness", thickness, defaultValue: null));
        properties.add(new DiagnosticsProperty<WidgetStateProperty<bool?>>("trackVisibility", trackVisibility, defaultValue: null));
        properties.add(new DiagnosticsProperty<bool>("interactive", interactive, defaultValue: null));
        properties.add(new DiagnosticsProperty<Radius>("radius", radius, defaultValue: null));
        properties.add(new DiagnosticsProperty<WidgetStateProperty<Color?>>("thumbColor", thumbColor, defaultValue: null));
        properties.add(new DiagnosticsProperty<WidgetStateProperty<Color?>>("trackColor", trackColor, defaultValue: null));
        properties.add(new DiagnosticsProperty<WidgetStateProperty<Color?>>("trackBorderColor", trackBorderColor, defaultValue: null));
        properties.add(new DiagnosticsProperty<double>("crossAxisMargin", crossAxisMargin, defaultValue: null));
        properties.add(new DiagnosticsProperty<double>("mainAxisMargin", mainAxisMargin, defaultValue: null));
        properties.add(new DiagnosticsProperty<double>("minThumbLength", minThumbLength, defaultValue: null));
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
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Scrollbar_themeLibrary
{
    internal static bool? _lerpBool(bool? a, bool? b, double t) => (t < 0.5) ? a : b;
}

public class ScrollbarTheme : InheritedTheme
{
    public virtual ScrollbarThemeData data { get; private set; } = default!;

    public ScrollbarTheme(Key? key = null, ScrollbarThemeData data = default!, Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static ScrollbarThemeData of(BuildContext context)
    {
        ScrollbarTheme? scrollbarThemeLocal = context.dependOnInheritedWidgetOfExactType<ScrollbarTheme>();
        return scrollbarThemeLocal?.data ?? Theme.of(context).scrollbarTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return new ScrollbarTheme(data: data, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((ScrollbarTheme)oldWidget).data));
}
