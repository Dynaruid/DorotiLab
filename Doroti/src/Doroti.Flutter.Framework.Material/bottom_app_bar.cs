// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/bottom_app_bar.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public class BottomAppBar : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? child { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.NotchedShape? shape { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual double notchMargin { get; private set; } = default!;
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual double? height { get; private set; }

    public BottomAppBar(global::Doroti.Generated.Framework.Foundation.Key? key = null, Color? color = null, double? elevation = null, global::Doroti.Generated.Framework.Painting.NotchedShape? shape = null, Clip clipBehavior = Clip.none, double notchMargin = 4.0, global::Doroti.Generated.Framework.Widgets.Widget? child = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, Color? surfaceTintColor = null, Color? shadowColor = null, double? height = null) : base(key: key)
    {
        this.color = color;
        this.elevation = elevation;
        this.shape = shape;
        this.clipBehavior = clipBehavior;
        this.notchMargin = notchMargin;
        this.child = child;
        this.padding = padding;
        this.surfaceTintColor = surfaceTintColor;
        this.shadowColor = shadowColor;
        this.height = height;
        System.Diagnostics.Debug.Assert(((elevation is null) || (elevation >= 0.0)));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _BottomAppBarState__bottom_app_bar());
}

internal class _BottomAppBarState__bottom_app_bar : global::Doroti.Generated.Framework.Widgets.State<BottomAppBar>
{
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<ScaffoldGeometry> geometryListenable { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> materialKey { get; private set; } = global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create();

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        geometryListenable = Scaffold.geometryOf(this.context);
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ThemeData theme__6766 = Theme.of(context);
        bool isMaterial3__6808 = theme__6766.useMaterial3;
        BottomAppBarThemeData babTheme__6874 = BottomAppBarTheme.of(context);
        BottomAppBarThemeData defaults__6948 = (isMaterial3__6808 ? new _BottomAppBarDefaultsM3__bottom_app_bar(context) : new _BottomAppBarDefaultsM2__bottom_app_bar(context));
        bool hasFab__7074 = Scaffold.of(context).hasFloatingActionButton;
        global::Doroti.Generated.Framework.Painting.NotchedShape? notchedShape__7153 = ((((BottomAppBar)this.widget).shape ?? babTheme__6874.shape) ?? defaults__6948.shape);
        global::Doroti.Generated.Framework.Rendering.CustomClipper<global::Doroti.Flutter.Ui.Path> clipper__7248 = ((global::Doroti.Generated.Framework.Rendering.CustomClipper<global::Doroti.Flutter.Ui.Path>)(object?)(((notchedShape__7153 is not null) && hasFab__7074) ? new _BottomAppBarClipper__bottom_app_bar(geometry: this.geometryListenable, shape: notchedShape__7153, materialKey: this.materialKey, notchMargin: ((BottomAppBar)this.widget).notchMargin) : new global::Doroti.Generated.Framework.Rendering.ShapeBorderClipper(shape: new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder())));
        double elevation__7577 = ((((BottomAppBar)this.widget).elevation ?? babTheme__6874.elevation) ?? DartRuntimePrimitives.RequireValue(defaults__6948.elevation));
        double? height__7670 = ((((BottomAppBar)this.widget).height ?? babTheme__6874.height) ?? defaults__6948.height);
        global::Doroti.Flutter.Ui.Color color__7748 = ((global::Doroti.Flutter.Ui.Color)(object?)((((BottomAppBar)this.widget).color ?? babTheme__6874.color) ?? defaults__6948.color!));
        global::Doroti.Flutter.Ui.Color surfaceTintColor__7823 = ((global::Doroti.Flutter.Ui.Color)(object?)((((BottomAppBar)this.widget).surfaceTintColor ?? babTheme__6874.surfaceTintColor) ?? defaults__6948.surfaceTintColor!));
        global::Doroti.Flutter.Ui.Color effectiveColor__7950 = ((global::Doroti.Flutter.Ui.Color)(object?)(isMaterial3__6808 ? ElevationOverlay.applySurfaceTint(color__7748, surfaceTintColor__7823, elevation__7577) : ElevationOverlay.applyOverlay(context, color__7748, elevation__7577)));
        global::Doroti.Flutter.Ui.Color shadowColor__8143 = ((global::Doroti.Flutter.Ui.Color)(object?)((((BottomAppBar)this.widget).shadowColor ?? babTheme__6874.shadowColor) ?? defaults__6948.shadowColor!));
        global::Doroti.Generated.Framework.Widgets.Widget child__8244 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.SizedBox(height: height__7670, child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: ((((BottomAppBar)this.widget).padding ?? babTheme__6874.padding) ?? ((isMaterial3__6808 ? global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(vertical: 12.0, horizontal: 16.0) : global::Doroti.Generated.Framework.Painting.EdgeInsets.zero))), child: ((BottomAppBar)this.widget).child)));
        var material__8581 = new Material(key: this.materialKey, type: MaterialType.transparency, child: new global::Doroti.Generated.Framework.Widgets.SafeArea(child: child__8244));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.PhysicalShape(clipper: clipper__7248, elevation: elevation__7577, shadowColor: shadowColor__8143, color: effectiveColor__7950, clipBehavior: ((BottomAppBar)this.widget).clipBehavior, child: material__8581));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _BottomAppBarClipper__bottom_app_bar : global::Doroti.Generated.Framework.Rendering.CustomClipper<Path>
{
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<ScaffoldGeometry> geometry { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.NotchedShape shape { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> materialKey { get; private set; } = default!;
    public virtual double notchMargin { get; private set; } = default!;

    internal _BottomAppBarClipper__bottom_app_bar(global::Doroti.Generated.Framework.Foundation.ValueListenable<ScaffoldGeometry> geometry, global::Doroti.Generated.Framework.Painting.NotchedShape shape, global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> materialKey, double notchMargin) : base(reclip: geometry)
    {
        this.geometry = geometry;
        this.shape = shape;
        this.materialKey = materialKey;
        this.notchMargin = notchMargin;
    }

    public virtual double bottomNavigationBarTop
    {
        get
        {
            double? bottomNavigationBarTop__9605 = ((global::Doroti.Generated.Framework.Foundation.ValueListenable<ScaffoldGeometry>)this.geometry).value.bottomNavigationBarTop;
            if ((bottomNavigationBarTop__9605 is not null))
            {
                double bottomNavigationBarTop__9605__value9677 = DartRuntimePrimitives.RequireValue(bottomNavigationBarTop__9605);
                return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(bottomNavigationBarTop__9605__value9677));
            }
            var box__9764 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)((global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>)this.materialKey).currentContext?.findRenderObject())!;
            return ((Offset)((dynamic)box__9764)?.localToGlobal(Offset.zero)).dy;
            return default!;
        }
    }
    public override Path getClip(Size size)
    {
        global::Doroti.Flutter.Ui.Rect? button__10145 = ((global::Doroti.Flutter.Ui.Rect?)(object?)((global::Doroti.Generated.Framework.Foundation.ValueListenable<ScaffoldGeometry>)this.geometry).value.floatingActionButtonArea?.translate(0.0, (this.bottomNavigationBarTop * -1.0)));
        return ((Path)(object?)this.shape.getOuterPath((Offset.zero & size), button__10145?.inflate(this.notchMargin)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldReclip(global::Doroti.Generated.Framework.Rendering.CustomClipper<Path> oldClipper)
    {
        var __oldClipper = (_BottomAppBarClipper__bottom_app_bar)(object)oldClipper;
        return (((!object.Equals(((_BottomAppBarClipper__bottom_app_bar)__oldClipper).geometry, this.geometry)) || (!object.Equals(((_BottomAppBarClipper__bottom_app_bar)__oldClipper).shape, this.shape))) || (((_BottomAppBarClipper__bottom_app_bar)__oldClipper).notchMargin != this.notchMargin));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _BottomAppBarDefaultsM2__bottom_app_bar : BottomAppBarThemeData
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;

    internal _BottomAppBarDefaultsM2__bottom_app_bar(global::Doroti.Generated.Framework.Widgets.BuildContext context) : base(elevation: 8.0)
    {
        this.context = context;
    }

    public virtual global::Doroti.Flutter.Ui.Color? color => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Color>(((object.Equals(Theme.brightnessOf(this.context), Brightness.dark)) ? Colors.grey[800L]! : Colors.white));
    public virtual global::Doroti.Flutter.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Color>(Theme.of(this.context).colorScheme.surfaceTint);
    public virtual global::Doroti.Flutter.Ui.Color shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Color>(new global::Doroti.Flutter.Ui.Color(4278190080L));
}

internal class _BottomAppBarDefaultsM3__bottom_app_bar : BottomAppBarThemeData
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = Theme.of(this.context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _BottomAppBarDefaultsM3__bottom_app_bar(global::Doroti.Generated.Framework.Widgets.BuildContext context) : base(elevation: 3.0, height: 80.0, shape: new global::Doroti.Generated.Framework.Painting.AutomaticNotchedShape(new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder()))
    {
        this.context = context;
    }

    public virtual global::Doroti.Flutter.Ui.Color? color => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Color>(this._colors.surfaceContainer);
    public virtual global::Doroti.Flutter.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Color>(Colors.transparent);
    public virtual global::Doroti.Flutter.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Color>(Colors.transparent);
}
