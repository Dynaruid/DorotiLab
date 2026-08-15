// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/bottom_navigation_bar_theme.dart
using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public class BottomNavigationBarThemeData : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.IconThemeData? selectedIconTheme { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.IconThemeData? unselectedIconTheme { get; private set; }
    public virtual Color? selectedItemColor { get; private set; }
    public virtual Color? unselectedItemColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? selectedLabelStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? unselectedLabelStyle { get; private set; }
    public virtual bool? showSelectedLabels { get; private set; }
    public virtual bool? showUnselectedLabels { get; private set; }
    public virtual BottomNavigationBarType? type { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual BottomNavigationBarLandscapeLayout? landscapeLayout { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor { get; private set; }

    public BottomNavigationBarThemeData(Color? backgroundColor = null, double? elevation = null, global::Doroti.Generated.Framework.Widgets.IconThemeData? selectedIconTheme = null, global::Doroti.Generated.Framework.Widgets.IconThemeData? unselectedIconTheme = null, Color? selectedItemColor = null, Color? unselectedItemColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? selectedLabelStyle = null, global::Doroti.Generated.Framework.Painting.TextStyle? unselectedLabelStyle = null, bool? showSelectedLabels = null, bool? showUnselectedLabels = null, BottomNavigationBarType? type = null, bool? enableFeedback = null, BottomNavigationBarLandscapeLayout? landscapeLayout = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor = null)
    {
        this.backgroundColor = backgroundColor;
        this.elevation = elevation;
        this.selectedIconTheme = selectedIconTheme;
        this.unselectedIconTheme = unselectedIconTheme;
        this.selectedItemColor = selectedItemColor;
        this.unselectedItemColor = unselectedItemColor;
        this.selectedLabelStyle = selectedLabelStyle;
        this.unselectedLabelStyle = unselectedLabelStyle;
        this.showSelectedLabels = showSelectedLabels;
        this.showUnselectedLabels = showUnselectedLabels;
        this.type = type;
        this.enableFeedback = enableFeedback;
        this.landscapeLayout = landscapeLayout;
        this.mouseCursor = mouseCursor;
    }

    public virtual BottomNavigationBarThemeData copyWith(Color? backgroundColor = null, double? elevation = null, global::Doroti.Generated.Framework.Widgets.IconThemeData? selectedIconTheme = null, global::Doroti.Generated.Framework.Widgets.IconThemeData? unselectedIconTheme = null, Color? selectedItemColor = null, Color? unselectedItemColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? selectedLabelStyle = null, global::Doroti.Generated.Framework.Painting.TextStyle? unselectedLabelStyle = null, bool? showSelectedLabels = null, bool? showUnselectedLabels = null, BottomNavigationBarType? type = null, bool? enableFeedback = null, BottomNavigationBarLandscapeLayout? landscapeLayout = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor = null)
    {
        return new BottomNavigationBarThemeData(backgroundColor: (backgroundColor ?? this.backgroundColor), elevation: (elevation ?? this.elevation), selectedIconTheme: (selectedIconTheme ?? this.selectedIconTheme), unselectedIconTheme: (unselectedIconTheme ?? this.unselectedIconTheme), selectedItemColor: (selectedItemColor ?? this.selectedItemColor), unselectedItemColor: (unselectedItemColor ?? this.unselectedItemColor), selectedLabelStyle: (selectedLabelStyle ?? this.selectedLabelStyle), unselectedLabelStyle: (unselectedLabelStyle ?? this.unselectedLabelStyle), showSelectedLabels: (showSelectedLabels ?? this.showSelectedLabels), showUnselectedLabels: (showUnselectedLabels ?? this.showUnselectedLabels), type: (type ?? this.type), enableFeedback: (enableFeedback ?? this.enableFeedback), landscapeLayout: (landscapeLayout ?? this.landscapeLayout), mouseCursor: (mouseCursor ?? this.mouseCursor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static BottomNavigationBarThemeData lerp(BottomNavigationBarThemeData? a, BottomNavigationBarThemeData? b, double t)
    {
        if ((DartRuntimePrimitives.Identical(a, b) && (a is not null)))
        {
            return a;
        }
        return new BottomNavigationBarThemeData(backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), selectedIconTheme: IconThemeData.lerp(a?.selectedIconTheme, b?.selectedIconTheme, t), unselectedIconTheme: IconThemeData.lerp(a?.unselectedIconTheme, b?.unselectedIconTheme, t), selectedItemColor: Dart_uiLibrary.Color.lerp(a?.selectedItemColor, b?.selectedItemColor, t), unselectedItemColor: Dart_uiLibrary.Color.lerp(a?.unselectedItemColor, b?.unselectedItemColor, t), selectedLabelStyle: TextStyle.lerp(a?.selectedLabelStyle, b?.selectedLabelStyle, t), unselectedLabelStyle: TextStyle.lerp(a?.unselectedLabelStyle, b?.unselectedLabelStyle, t), showSelectedLabels: ((t < 0.5) ? a?.showSelectedLabels : b?.showSelectedLabels), showUnselectedLabels: ((t < 0.5) ? a?.showUnselectedLabels : b?.showUnselectedLabels), type: ((t < 0.5) ? a?.type : b?.type), enableFeedback: ((t < 0.5) ? a?.enableFeedback : b?.enableFeedback), landscapeLayout: ((t < 0.5) ? a?.landscapeLayout : b?.landscapeLayout), mouseCursor: ((t < 0.5) ? a?.mouseCursor : b?.mouseCursor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.backgroundColor, this.elevation, this.selectedIconTheme, this.unselectedIconTheme, this.selectedItemColor, this.unselectedItemColor, this.selectedLabelStyle, this.unselectedLabelStyle, this.showSelectedLabels, this.showUnselectedLabels, this.type, this.enableFeedback, this.landscapeLayout, this.mouseCursor));
    public override bool Equals(object? other)
    {
        var __other = other as BottomNavigationBarThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((((((((((__other is BottomNavigationBarThemeData) && (object.Equals(((BottomNavigationBarThemeData)((BottomNavigationBarThemeData)__other)).backgroundColor, this.backgroundColor))) && (((BottomNavigationBarThemeData)((BottomNavigationBarThemeData)__other)).elevation == this.elevation)) && (object.Equals(((BottomNavigationBarThemeData)((BottomNavigationBarThemeData)__other)).selectedIconTheme, this.selectedIconTheme))) && (object.Equals(((BottomNavigationBarThemeData)((BottomNavigationBarThemeData)__other)).unselectedIconTheme, this.unselectedIconTheme))) && (object.Equals(((BottomNavigationBarThemeData)((BottomNavigationBarThemeData)__other)).selectedItemColor, this.selectedItemColor))) && (object.Equals(((BottomNavigationBarThemeData)((BottomNavigationBarThemeData)__other)).unselectedItemColor, this.unselectedItemColor))) && (object.Equals(((BottomNavigationBarThemeData)((BottomNavigationBarThemeData)__other)).selectedLabelStyle, this.selectedLabelStyle))) && (object.Equals(((BottomNavigationBarThemeData)((BottomNavigationBarThemeData)__other)).unselectedLabelStyle, this.unselectedLabelStyle))) && (((BottomNavigationBarThemeData)((BottomNavigationBarThemeData)__other)).showSelectedLabels == this.showSelectedLabels)) && (((BottomNavigationBarThemeData)((BottomNavigationBarThemeData)__other)).showUnselectedLabels == this.showUnselectedLabels)) && (object.Equals(((BottomNavigationBarThemeData)((BottomNavigationBarThemeData)__other)).type, this.type))) && (((BottomNavigationBarThemeData)((BottomNavigationBarThemeData)__other)).enableFeedback == this.enableFeedback)) && (object.Equals(((BottomNavigationBarThemeData)((BottomNavigationBarThemeData)__other)).landscapeLayout, this.landscapeLayout))) && (object.Equals(((BottomNavigationBarThemeData)((BottomNavigationBarThemeData)__other)).mouseCursor, this.mouseCursor)));
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("backgroundColor", this.backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("elevation", this.elevation, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.IconThemeData>("selectedIconTheme", this.selectedIconTheme, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.IconThemeData>("unselectedIconTheme", this.unselectedIconTheme, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("selectedItemColor", this.selectedItemColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("unselectedItemColor", this.unselectedItemColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("selectedLabelStyle", this.selectedLabelStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("unselectedLabelStyle", this.unselectedLabelStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("showSelectedLabels", this.showSelectedLabels, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("showUnselectedLabels", this.showUnselectedLabels, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<BottomNavigationBarType>("type", this.type, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("enableFeedback", this.enableFeedback, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<BottomNavigationBarLandscapeLayout>("landscapeLayout", this.landscapeLayout, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>>("mouseCursor", this.mouseCursor, defaultValue: null));
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

public class BottomNavigationBarTheme : global::Doroti.Generated.Framework.Widgets.InheritedWidget
{
    public virtual BottomNavigationBarThemeData data { get; private set; } = default!;

    public BottomNavigationBarTheme(global::Doroti.Generated.Framework.Foundation.Key? key = null, BottomNavigationBarThemeData data = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static BottomNavigationBarThemeData of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        BottomNavigationBarTheme? bottomNavTheme__12363 = ((BottomNavigationBarTheme?)(object?)context.dependOnInheritedWidgetOfExactType<BottomNavigationBarTheme>());
        return (bottomNavTheme__12363?.data ?? Theme.of(context).bottomNavigationBarTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((BottomNavigationBarTheme)oldWidget).data)));
}
