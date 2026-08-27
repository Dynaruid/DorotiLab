// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/bottom_app_bar.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Material;

public class BottomAppBar : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual global::Doroti.Framework.Painting.NotchedShape? shape { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual double notchMargin { get; private set; } = default!;
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual double? height { get; private set; }

    public BottomAppBar(global::Doroti.Framework.Foundation.Key? key = null, Color? color = null, double? elevation = null, global::Doroti.Framework.Painting.NotchedShape? shape = null, Clip clipBehavior = Clip.none, double notchMargin = 4.0, global::Doroti.Framework.Widgets.Widget? child = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, Color? surfaceTintColor = null, Color? shadowColor = null, double? height = null) : base(key: key)
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

internal class _BottomAppBarState__bottom_app_bar : global::Doroti.Framework.Widgets.State<BottomAppBar>
{
    public virtual global::Doroti.Framework.Foundation.ValueListenable<ScaffoldGeometry> geometryListenable { get; set; } = default!;
    public virtual global::Doroti.Framework.Widgets.GlobalKey<IState> materialKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create();

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        geometryListenable = Scaffold.geometryOf(this.context);
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        bool isMaterial3 = theme.useMaterial3;
        BottomAppBarThemeData babTheme = BottomAppBarTheme.of(context);
        BottomAppBarThemeData defaults = (isMaterial3 ? new _BottomAppBarDefaultsM3__bottom_app_bar(context) : new _BottomAppBarDefaultsM2__bottom_app_bar(context));
        bool hasFab = Scaffold.of(context).hasFloatingActionButton;
        global::Doroti.Framework.Painting.NotchedShape? notchedShape = ((((BottomAppBar)this.widget).shape ?? babTheme.shape) ?? defaults.shape);
        global::Doroti.Framework.Rendering.CustomClipper<global::Doroti.Ui.Path> clipperLocal = ((global::Doroti.Framework.Rendering.CustomClipper<global::Doroti.Ui.Path>)(object?)(((notchedShape is not null) && hasFab) ? new _BottomAppBarClipper__bottom_app_bar(geometry: this.geometryListenable, shape: notchedShape, materialKey: this.materialKey, notchMargin: ((BottomAppBar)this.widget).notchMargin) : new global::Doroti.Framework.Rendering.ShapeBorderClipper(shape: new global::Doroti.Framework.Painting.RoundedRectangleBorder())));
        double elevationLocal = ((((BottomAppBar)this.widget).elevation ?? babTheme.elevation) ?? DartRuntimePrimitives.RequireValue(defaults.elevation));
        double? heightLocal = ((((BottomAppBar)this.widget).height ?? babTheme.height) ?? defaults.height);
        global::Doroti.Ui.Color colorLocal = ((global::Doroti.Ui.Color)(object?)((((BottomAppBar)this.widget).color ?? babTheme.color) ?? defaults.color!));
        global::Doroti.Ui.Color surfaceTintColorLocal = ((global::Doroti.Ui.Color)(object?)((((BottomAppBar)this.widget).surfaceTintColor ?? babTheme.surfaceTintColor) ?? defaults.surfaceTintColor!));
        global::Doroti.Ui.Color effectiveColor = ((global::Doroti.Ui.Color)(object?)(isMaterial3 ? ElevationOverlay.applySurfaceTint(colorLocal, surfaceTintColorLocal, elevationLocal) : ElevationOverlay.applyOverlay(context, colorLocal, elevationLocal)));
        global::Doroti.Ui.Color shadowColorLocal = ((global::Doroti.Ui.Color)(object?)((((BottomAppBar)this.widget).shadowColor ?? babTheme.shadowColor) ?? defaults.shadowColor!));
        global::Doroti.Framework.Widgets.Widget childLocal = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.SizedBox(height: heightLocal, child: new global::Doroti.Framework.Widgets.Padding(padding: ((((BottomAppBar)this.widget).padding ?? babTheme.padding) ?? ((isMaterial3 ? global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(vertical: 12.0, horizontal: 16.0) : global::Doroti.Framework.Painting.EdgeInsets.zero))), child: ((BottomAppBar)this.widget).child)));
        var material = new Material(key: this.materialKey, type: MaterialType.transparency, child: new global::Doroti.Framework.Widgets.SafeArea(child: childLocal));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.PhysicalShape(clipper: clipperLocal, elevation: elevationLocal, shadowColor: shadowColorLocal, color: effectiveColor, clipBehavior: ((BottomAppBar)this.widget).clipBehavior, child: material));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _BottomAppBarClipper__bottom_app_bar : global::Doroti.Framework.Rendering.CustomClipper<Path>
{
    public virtual global::Doroti.Framework.Foundation.ValueListenable<ScaffoldGeometry> geometry { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.NotchedShape shape { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.GlobalKey<IState> materialKey { get; private set; } = default!;
    public virtual double notchMargin { get; private set; } = default!;

    internal _BottomAppBarClipper__bottom_app_bar(global::Doroti.Framework.Foundation.ValueListenable<ScaffoldGeometry> geometry, global::Doroti.Framework.Painting.NotchedShape shape, global::Doroti.Framework.Widgets.GlobalKey<IState> materialKey, double notchMargin) : base(reclip: geometry)
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
            double? bottomNavigationBarTopLocal = ((global::Doroti.Framework.Foundation.ValueListenable<ScaffoldGeometry>)this.geometry).value.bottomNavigationBarTop;
            if ((bottomNavigationBarTopLocal is not null))
            {
                double bottomNavigationBarTop__9605__value9677 = DartRuntimePrimitives.RequireValue(bottomNavigationBarTopLocal);
                return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(bottomNavigationBarTop__9605__value9677));
            }
            var box = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((global::Doroti.Framework.Widgets.GlobalKey<IState>)this.materialKey).currentContext?.findRenderObject())!;
            return ((Offset)((dynamic)box)?.localToGlobal(Offset.zero)).dy;
            return default!;
        }
    }
    public override Path getClip(Size size)
    {
        global::Doroti.Ui.Rect? button = ((global::Doroti.Ui.Rect?)(object?)((global::Doroti.Framework.Foundation.ValueListenable<ScaffoldGeometry>)this.geometry).value.floatingActionButtonArea?.translate(0.0, (this.bottomNavigationBarTop * -1.0)));
        return ((Path)(object?)this.shape.getOuterPath((Offset.zero & size), button?.inflate(this.notchMargin)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldReclip(global::Doroti.Framework.Rendering.CustomClipper<Path> oldClipper)
    {
        var __oldClipper = (_BottomAppBarClipper__bottom_app_bar)(object)oldClipper;
        return (((!object.Equals(((_BottomAppBarClipper__bottom_app_bar)__oldClipper).geometry, this.geometry)) || (!object.Equals(((_BottomAppBarClipper__bottom_app_bar)__oldClipper).shape, this.shape))) || (((_BottomAppBarClipper__bottom_app_bar)__oldClipper).notchMargin != this.notchMargin));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _BottomAppBarDefaultsM2__bottom_app_bar : BottomAppBarThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;

    internal _BottomAppBarDefaultsM2__bottom_app_bar(global::Doroti.Framework.Widgets.BuildContext context) : base(elevation: 8.0)
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? color => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((object.Equals(Theme.brightnessOf(this.context), Brightness.dark)) ? Colors.grey[800L]! : Colors.white));
    public virtual global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Theme.of(this.context).colorScheme.surfaceTint);
    public virtual global::Doroti.Ui.Color shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(new global::Doroti.Ui.Color(4278190080L));
}

internal class _BottomAppBarDefaultsM3__bottom_app_bar : BottomAppBarThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
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

    internal _BottomAppBarDefaultsM3__bottom_app_bar(global::Doroti.Framework.Widgets.BuildContext context) : base(elevation: 3.0, height: 80.0, shape: new global::Doroti.Framework.Painting.AutomaticNotchedShape(new global::Doroti.Framework.Painting.RoundedRectangleBorder()))
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? color => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.surfaceContainer);
    public virtual global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public virtual global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
}
