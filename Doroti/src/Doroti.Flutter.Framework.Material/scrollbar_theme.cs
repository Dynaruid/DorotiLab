// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/scrollbar_theme.dart
using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public class ScrollbarThemeData : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<bool?>? thumbVisibility { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double?>? thickness { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<bool?>? trackVisibility { get; private set; }
    public virtual bool? interactive { get; private set; }
    public virtual Radius? radius { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? thumbColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? trackColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? trackBorderColor { get; private set; }
    public virtual double? crossAxisMargin { get; private set; }
    public virtual double? mainAxisMargin { get; private set; }
    public virtual double? minThumbLength { get; private set; }

    public ScrollbarThemeData(global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<bool?>? thumbVisibility = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double?>? thickness = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<bool?>? trackVisibility = null, Radius? radius = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? thumbColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? trackColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? trackBorderColor = null, double? crossAxisMargin = null, double? mainAxisMargin = null, double? minThumbLength = null, bool? interactive = null)
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

    public virtual ScrollbarThemeData copyWith(global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<bool?>? thumbVisibility = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double?>? thickness = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<bool?>? trackVisibility = null, bool? interactive = null, Radius? radius = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? thumbColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? trackColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? trackBorderColor = null, double? crossAxisMargin = null, double? mainAxisMargin = null, double? minThumbLength = null)
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
        return new ScrollbarThemeData(thumbVisibility: WidgetStateProperty.lerp<bool?>(a?.thumbVisibility, b?.thumbVisibility, t, (global::System.Func<bool?, bool?, double, bool?>)Scrollbar_themeLibrary._lerpBool), thickness: WidgetStateProperty.lerp<double?>(a?.thickness, b?.thickness, t, (global::System.Func<double?, double?, double, double?>)Dart_uiLibrary.lerpDouble), trackVisibility: WidgetStateProperty.lerp<bool?>(a?.trackVisibility, b?.trackVisibility, t, (global::System.Func<bool?, bool?, double, bool?>)Scrollbar_themeLibrary._lerpBool), interactive: Scrollbar_themeLibrary._lerpBool(a?.interactive, b?.interactive, t), radius: Dart_uiLibrary.Radius.lerp(a?.radius, b?.radius, t), thumbColor: WidgetStateProperty.lerp<global::Doroti.Flutter.Ui.Color?>(a?.thumbColor, b?.thumbColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), trackColor: WidgetStateProperty.lerp<global::Doroti.Flutter.Ui.Color?>(a?.trackColor, b?.trackColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), trackBorderColor: WidgetStateProperty.lerp<global::Doroti.Flutter.Ui.Color?>(a?.trackBorderColor, b?.trackBorderColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), crossAxisMargin: Dart_uiLibrary.lerpDouble(a?.crossAxisMargin, b?.crossAxisMargin, t), mainAxisMargin: Dart_uiLibrary.lerpDouble(a?.mainAxisMargin, b?.mainAxisMargin, t), minThumbLength: Dart_uiLibrary.lerpDouble(a?.minThumbLength, b?.minThumbLength, t));
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
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((((((__other is ScrollbarThemeData) && (object.Equals(((ScrollbarThemeData)((ScrollbarThemeData)__other)).thumbVisibility, this.thumbVisibility))) && (object.Equals(((ScrollbarThemeData)((ScrollbarThemeData)__other)).thickness, this.thickness))) && (object.Equals(((ScrollbarThemeData)((ScrollbarThemeData)__other)).trackVisibility, this.trackVisibility))) && (((ScrollbarThemeData)((ScrollbarThemeData)__other)).interactive == this.interactive)) && (object.Equals(((ScrollbarThemeData)((ScrollbarThemeData)__other)).radius, this.radius))) && (object.Equals(((ScrollbarThemeData)((ScrollbarThemeData)__other)).thumbColor, this.thumbColor))) && (object.Equals(((ScrollbarThemeData)((ScrollbarThemeData)__other)).trackColor, this.trackColor))) && (object.Equals(((ScrollbarThemeData)((ScrollbarThemeData)__other)).trackBorderColor, this.trackBorderColor))) && (((ScrollbarThemeData)((ScrollbarThemeData)__other)).crossAxisMargin == this.crossAxisMargin)) && (((ScrollbarThemeData)((ScrollbarThemeData)__other)).mainAxisMargin == this.mainAxisMargin)) && (((ScrollbarThemeData)((ScrollbarThemeData)__other)).minThumbLength == this.minThumbLength));
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<bool?>>("thumbVisibility", this.thumbVisibility, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double?>>("thickness", this.thickness, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<bool?>>("trackVisibility", this.trackVisibility, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("interactive", this.interactive, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Flutter.Ui.Radius>("radius", this.radius, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>>("thumbColor", this.thumbColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>>("trackColor", this.trackColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>>("trackBorderColor", this.trackBorderColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<double>("crossAxisMargin", this.crossAxisMargin, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<double>("mainAxisMargin", this.mainAxisMargin, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<double>("minThumbLength", this.minThumbLength, defaultValue: null));
    }

    public virtual string toStringShort() => global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
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

public static partial class Scrollbar_themeLibrary
{
    internal static bool? _lerpBool(bool? a, bool? b, double t) => ((t < 0.5) ? a : b);
}

public class ScrollbarTheme : global::Doroti.Generated.Framework.Widgets.InheritedTheme
{
    public virtual ScrollbarThemeData data { get; private set; } = default!;

    public ScrollbarTheme(global::Doroti.Generated.Framework.Foundation.Key? key = null, ScrollbarThemeData data = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static ScrollbarThemeData of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ScrollbarTheme? scrollbarTheme__10876 = ((ScrollbarTheme?)(object?)context.dependOnInheritedWidgetOfExactType<ScrollbarTheme>());
        return (scrollbarTheme__10876?.data ?? Theme.of(context).scrollbarTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget wrap(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new ScrollbarTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((ScrollbarTheme)oldWidget).data)));
}
