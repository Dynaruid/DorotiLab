// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/bottom_navigation_bar_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class BottomNavigationBarThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual global::Doroti.Framework.Widgets.IconThemeData? selectedIconTheme { get; private set; }
    public virtual global::Doroti.Framework.Widgets.IconThemeData? unselectedIconTheme { get; private set; }
    public virtual Color? selectedItemColor { get; private set; }
    public virtual Color? unselectedItemColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? selectedLabelStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? unselectedLabelStyle { get; private set; }
    public virtual bool? showSelectedLabels { get; private set; }
    public virtual bool? showUnselectedLabels { get; private set; }
    public virtual BottomNavigationBarType? type { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual BottomNavigationBarLandscapeLayout? landscapeLayout { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor { get; private set; }

    public BottomNavigationBarThemeData(Color? backgroundColor = null, double? elevation = null, global::Doroti.Framework.Widgets.IconThemeData? selectedIconTheme = null, global::Doroti.Framework.Widgets.IconThemeData? unselectedIconTheme = null, Color? selectedItemColor = null, Color? unselectedItemColor = null, global::Doroti.Framework.Painting.TextStyle? selectedLabelStyle = null, global::Doroti.Framework.Painting.TextStyle? unselectedLabelStyle = null, bool? showSelectedLabels = null, bool? showUnselectedLabels = null, BottomNavigationBarType? type = null, bool? enableFeedback = null, BottomNavigationBarLandscapeLayout? landscapeLayout = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor = null)
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

    public virtual BottomNavigationBarThemeData copyWith(Color? backgroundColor = null, double? elevation = null, global::Doroti.Framework.Widgets.IconThemeData? selectedIconTheme = null, global::Doroti.Framework.Widgets.IconThemeData? unselectedIconTheme = null, Color? selectedItemColor = null, Color? unselectedItemColor = null, global::Doroti.Framework.Painting.TextStyle? selectedLabelStyle = null, global::Doroti.Framework.Painting.TextStyle? unselectedLabelStyle = null, bool? showSelectedLabels = null, bool? showUnselectedLabels = null, BottomNavigationBarType? type = null, bool? enableFeedback = null, BottomNavigationBarLandscapeLayout? landscapeLayout = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor = null)
    {
        return new BottomNavigationBarThemeData(backgroundColor: backgroundColor ?? this.backgroundColor, elevation: elevation ?? this.elevation, selectedIconTheme: selectedIconTheme ?? this.selectedIconTheme, unselectedIconTheme: unselectedIconTheme ?? this.unselectedIconTheme, selectedItemColor: selectedItemColor ?? this.selectedItemColor, unselectedItemColor: unselectedItemColor ?? this.unselectedItemColor, selectedLabelStyle: selectedLabelStyle ?? this.selectedLabelStyle, unselectedLabelStyle: unselectedLabelStyle ?? this.unselectedLabelStyle, showSelectedLabels: showSelectedLabels ?? this.showSelectedLabels, showUnselectedLabels: showUnselectedLabels ?? this.showUnselectedLabels, type: type ?? this.type, enableFeedback: enableFeedback ?? this.enableFeedback, landscapeLayout: landscapeLayout ?? this.landscapeLayout, mouseCursor: mouseCursor ?? this.mouseCursor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static BottomNavigationBarThemeData lerp(BottomNavigationBarThemeData? a, BottomNavigationBarThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b) && (a is not null))
        {
            return a;
        }
        return new BottomNavigationBarThemeData(backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), selectedIconTheme: IconThemeData.lerp(a?.selectedIconTheme, b?.selectedIconTheme, t), unselectedIconTheme: IconThemeData.lerp(a?.unselectedIconTheme, b?.unselectedIconTheme, t), selectedItemColor: Dart_uiLibrary.Color.lerp(a?.selectedItemColor, b?.selectedItemColor, t), unselectedItemColor: Dart_uiLibrary.Color.lerp(a?.unselectedItemColor, b?.unselectedItemColor, t), selectedLabelStyle: TextStyle.lerp(a?.selectedLabelStyle, b?.selectedLabelStyle, t), unselectedLabelStyle: TextStyle.lerp(a?.unselectedLabelStyle, b?.unselectedLabelStyle, t), showSelectedLabels: (t < 0.5) ? a?.showSelectedLabels : b?.showSelectedLabels, showUnselectedLabels: (t < 0.5) ? a?.showUnselectedLabels : b?.showUnselectedLabels, type: (t < 0.5) ? a?.type : b?.type, enableFeedback: (t < 0.5) ? a?.enableFeedback : b?.enableFeedback, landscapeLayout: (t < 0.5) ? a?.landscapeLayout : b?.landscapeLayout, mouseCursor: (t < 0.5) ? a?.mouseCursor : b?.mouseCursor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(backgroundColor, elevation, selectedIconTheme, unselectedIconTheme, selectedItemColor, unselectedItemColor, selectedLabelStyle, unselectedLabelStyle, showSelectedLabels, showUnselectedLabels, type, enableFeedback, landscapeLayout, mouseCursor));
    public override bool Equals(object? other)
    {
        var __other = other as BottomNavigationBarThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is BottomNavigationBarThemeData) && Equals(__other.backgroundColor, backgroundColor) && (__other.elevation == elevation) && Equals(__other.selectedIconTheme, selectedIconTheme) && Equals(__other.unselectedIconTheme, unselectedIconTheme) && Equals(__other.selectedItemColor, selectedItemColor) && Equals(__other.unselectedItemColor, unselectedItemColor) && Equals(__other.selectedLabelStyle, selectedLabelStyle) && Equals(__other.unselectedLabelStyle, unselectedLabelStyle) && (__other.showSelectedLabels == showSelectedLabels) && (__other.showUnselectedLabels == showUnselectedLabels) && Equals(__other.type, type) && (__other.enableFeedback == enableFeedback) && Equals(__other.landscapeLayout, landscapeLayout) && Equals(__other.mouseCursor, mouseCursor);
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("backgroundColor", backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("elevation", elevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.IconThemeData>("selectedIconTheme", selectedIconTheme, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.IconThemeData>("unselectedIconTheme", unselectedIconTheme, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("selectedItemColor", selectedItemColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("unselectedItemColor", unselectedItemColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("selectedLabelStyle", selectedLabelStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("unselectedLabelStyle", unselectedLabelStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("showSelectedLabels", showSelectedLabels, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("showUnselectedLabels", showUnselectedLabels, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<BottomNavigationBarType>("type", type, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("enableFeedback", enableFeedback, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<BottomNavigationBarLandscapeLayout>("landscapeLayout", landscapeLayout, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>>("mouseCursor", mouseCursor, defaultValue: null));
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

public class BottomNavigationBarTheme : global::Doroti.Framework.Widgets.InheritedWidget
{
    public virtual BottomNavigationBarThemeData data { get; private set; } = default!;

    public BottomNavigationBarTheme(global::Doroti.Framework.Foundation.Key? key = null, BottomNavigationBarThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static BottomNavigationBarThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        BottomNavigationBarTheme? bottomNavTheme = context.dependOnInheritedWidgetOfExactType<BottomNavigationBarTheme>();
        return bottomNavTheme?.data ?? Theme.of(context).bottomNavigationBarTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((BottomNavigationBarTheme)oldWidget).data));
}
