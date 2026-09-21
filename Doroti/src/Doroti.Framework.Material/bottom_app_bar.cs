// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/bottom_app_bar.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class BottomAppBar : StatefulWidget
{
    public virtual Widget? child { get; private set; }
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual NotchedShape? shape { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual double notchMargin { get; private set; } = default!;
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual double? height { get; private set; }

    public BottomAppBar(
        Key? key = null,
        Color? color = null,
        double? elevation = null,
        NotchedShape? shape = null,
        Clip clipBehavior = Clip.none,
        double notchMargin = 4.0,
        Widget? child = null,
        EdgeInsetsGeometry? padding = null,
        Color? surfaceTintColor = null,
        Color? shadowColor = null,
        double? height = null
    )
        : base(key: key)
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
        System.Diagnostics.Debug.Assert((elevation is null) || (elevation >= 0.0));
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _BottomAppBarState__bottom_app_bar());
}

internal class _BottomAppBarState__bottom_app_bar : State<BottomAppBar>
{
    public virtual ValueListenable<ScaffoldGeometry> geometryListenable { get; set; } = default!;
    public virtual GlobalKey<IState> materialKey { get; private set; } = GlobalKey<IState>.Create();

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        geometryListenable = Scaffold.geometryOf(context);
    }

    public override Widget build(BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        BottomAppBarThemeData babTheme = BottomAppBarTheme.of(context);
        BottomAppBarThemeData defaults = new _BottomAppBarDefaultsM3__bottom_app_bar(context);
        bool hasFab = Scaffold.of(context).hasFloatingActionButton;
        NotchedShape? notchedShape = (widget.shape ?? babTheme.shape) ?? defaults.shape;
        CustomClipper<Path> clipperLocal =
            ((notchedShape is not null) && hasFab)
                ? new _BottomAppBarClipper__bottom_app_bar(
                    geometry: geometryListenable,
                    shape: notchedShape,
                    materialKey: materialKey,
                    notchMargin: widget.notchMargin
                )
                : new global::Doroti.Framework.Rendering.ShapeBorderClipper(
                    shape: new global::Doroti.Framework.Painting.RoundedRectangleBorder()
                );
        double elevationLocal =
            (widget.elevation ?? babTheme.elevation)
            ?? DartRuntimePrimitives.RequireValue(defaults.elevation);
        double? heightLocal = (widget.height ?? babTheme.height) ?? defaults.height;
        Color colorLocal = (widget.color ?? babTheme.color) ?? defaults.color!;
        Color surfaceTintColorLocal =
            (widget.surfaceTintColor ?? babTheme.surfaceTintColor) ?? defaults.surfaceTintColor!;
        Color effectiveColor = ElevationOverlay.applySurfaceTint(
            colorLocal,
            surfaceTintColorLocal,
            elevationLocal
        );
        Color shadowColorLocal =
            (widget.shadowColor ?? babTheme.shadowColor) ?? defaults.shadowColor!;
        Widget childLocal = new SizedBox(
            height: heightLocal,
            child: new Padding(
                padding: (widget.padding ?? babTheme.padding)
                    ?? EdgeInsets.CreateSymmetric(vertical: 12.0, horizontal: 16.0),
                child: widget.child
            )
        );
        var material = new Material(
            key: materialKey,
            type: MaterialType.transparency,
            child: new SafeArea(child: childLocal)
        );
        return new PhysicalShape(
            clipper: clipperLocal,
            elevation: elevationLocal,
            shadowColor: shadowColorLocal,
            color: effectiveColor,
            clipBehavior: widget.clipBehavior,
            child: material
        );
    }
}

internal class _BottomAppBarClipper__bottom_app_bar : CustomClipper<Path>
{
    public virtual ValueListenable<ScaffoldGeometry> geometry { get; private set; } = default!;
    public virtual NotchedShape shape { get; private set; } = default!;
    public virtual GlobalKey<IState> materialKey { get; private set; } = default!;
    public virtual double notchMargin { get; private set; } = default!;

    internal _BottomAppBarClipper__bottom_app_bar(
        ValueListenable<ScaffoldGeometry> geometry,
        NotchedShape shape,
        GlobalKey<IState> materialKey,
        double notchMargin
    )
        : base(reclip: geometry)
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
            double? bottomNavigationBarTopLocal = geometry.value.bottomNavigationBarTop;
            if (bottomNavigationBarTopLocal is not null)
            {
                double bottomNavigationBarTop__9605__value9677 = DartRuntimePrimitives.RequireValue(
                    bottomNavigationBarTopLocal
                );
                return DartRuntimePrimitives.RequireValue(
                    DartRuntimePrimitives.RequireValue(bottomNavigationBarTop__9605__value9677)
                );
            }
            var box = ((RenderBox?)materialKey.currentContext?.findRenderObject())!;
            return box?.localToGlobal(Offset.zero).dy ?? 0;
        }
    }

    public override Path getClip(Size size)
    {
        Rect? button = geometry.value.floatingActionButtonArea?.translate(
            0.0,
            bottomNavigationBarTop * -1.0
        );
        return shape.getOuterPath(Offset.zero & size, button?.inflate(notchMargin));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldReclip(CustomClipper<Path> oldClipper)
    {
        var __oldClipper = (_BottomAppBarClipper__bottom_app_bar)oldClipper;
        return (!Equals(__oldClipper.geometry, geometry))
            || (!Equals(__oldClipper.shape, shape))
            || (__oldClipper.notchMargin != notchMargin);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _BottomAppBarDefaultsM3__bottom_app_bar : BottomAppBarThemeData
{
    public virtual BuildContext context { get; private set; } = default!;
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = Theme.of(context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _BottomAppBarDefaultsM3__bottom_app_bar(BuildContext context)
        : base(
            elevation: 3.0,
            height: 80.0,
            shape: new AutomaticNotchedShape(new RoundedRectangleBorder())
        )
    {
        this.context = context;
    }

    public override Color? color =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.surfaceContainer);
    public override Color? surfaceTintColor =>
        DartRuntimePrimitives.ConvertValue<Color>(Colors.transparent);
    public override Color? shadowColor =>
        DartRuntimePrimitives.ConvertValue<Color>(Colors.transparent);
}
