// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/material.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public delegate Rect RectCallback();

public enum MaterialType
{
    canvas,
    card,
    circle,
    button,
    transparency,
}

public static partial class MaterialLibrary
{
    public static DartMap<MaterialType, BorderRadius?> kMaterialEdges = new DartMap<
        MaterialType,
        BorderRadius?
    >
    {
        [MaterialType.canvas] = (BorderRadius?)(object?)null,
        [MaterialType.card] = BorderRadius.CreateAll(Radius.circular(2.0)),
        [MaterialType.circle] = (BorderRadius?)(object?)null,
        [MaterialType.button] = BorderRadius.CreateAll(Radius.circular(2.0)),
        [MaterialType.transparency] = (BorderRadius?)(object?)null,
    };
}

public interface MaterialInkController
{
    public Color? color { get; }
    public Scheduler.TickerProvider vsync { get; }
    public void addInkFeature(InkFeature feature);
    public void markNeedsPaint();
}

public class Material : StatefulWidget
{
    public virtual Widget? child { get; private set; }
    public virtual MaterialType type { get; private set; } = default!;
    public virtual bool animateColor { get; private set; } = default!;
    public virtual double elevation { get; private set; } = default!;
    public virtual Color? color { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual TextStyle? textStyle { get; private set; }
    public virtual ShapeBorder? shape { get; private set; }
    public virtual bool borderOnForeground { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual Duration animationDuration { get; private set; } = default!;
    public virtual BorderRadiusGeometry? borderRadius { get; private set; }
    public const double defaultSplashRadius = 35.0;

    public Material(
        Key? key = null,
        MaterialType type = MaterialType.canvas,
        double elevation = 0.0,
        Color? color = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        TextStyle? textStyle = null,
        BorderRadiusGeometry? borderRadius = null,
        ShapeBorder? shape = null,
        bool borderOnForeground = true,
        Clip clipBehavior = Clip.none,
        Duration? animationDuration = null,
        Widget? child = null,
        bool animateColor = false
    )
        : base(key: key)
    {
        Duration __animationDuration = animationDuration ?? ConstantsLibrary.kThemeChangeDuration;
        this.type = type;
        this.elevation = elevation;
        this.color = color;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.textStyle = textStyle;
        this.borderRadius = borderRadius;
        this.shape = shape;
        this.borderOnForeground = borderOnForeground;
        this.clipBehavior = clipBehavior;
        this.animationDuration = __animationDuration;
        this.child = child;
        this.animateColor = animateColor;
        System.Diagnostics.Debug.Assert(elevation >= 0.0);
        System.Diagnostics.Debug.Assert(!((shape is not null) && (borderRadius is not null)));
        System.Diagnostics.Debug.Assert(
            !(
                DartRuntimePrimitives.Identical(type, MaterialType.circle)
                && ((borderRadius is not null) || (shape is not null))
            )
        );
    }

    public static MaterialInkController? maybeOf(BuildContext context)
    {
        return LookupBoundary.findAncestorRenderObjectOfType<_RenderInkFeatures__material>(context);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static MaterialInkController of(BuildContext context)
    {
        MaterialInkController? controller = maybeOf(context);
        DartRuntimePrimitives.Assert(() =>
        {
            if (controller is null)
            {
                if (
                    LookupBoundary.debugIsHidingAncestorRenderObjectOfType<_RenderInkFeatures__material>(
                        context
                    )
                )
                {
                    throw DartRuntimePrimitives.AsException(
                        FlutterError.Create(
                            "Material.of() was called with a context that does not have access to a Material widget.\n"
                                + "The context provided to Material.of() does have a Material widget ancestor, but it is "
                                + "hidden by a LookupBoundary. This can happen because you are using a widget that looks "
                                + "for a Material ancestor, but no such ancestor exists within the closest LookupBoundary.\n"
                                + "The context used was:\n"
                                + $"  {context}"
                        )
                    );
                }
                throw DartRuntimePrimitives.AsException(
                    FlutterError.Create(
                        "Material.of() was called with a context that does not contain a Material widget.\n"
                            + "No Material widget ancestor could be found starting from the context that was passed to "
                            + "Material.of(). This can happen because you are using a widget that looks for a Material "
                            + "ancestor, but no such ancestor exists.\n"
                            + "The context used was:\n"
                            + $"  {context}"
                    )
                );
            }
            return true;
        });
        return controller!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _MaterialState__material());

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<MaterialType>("type", type));
        properties.add(new DoubleProperty("elevation", elevation, defaultValue: 0.0));
        properties.add(new ColorProperty("color", color, defaultValue: null));
        properties.add(new ColorProperty("shadowColor", shadowColor, defaultValue: null));
        properties.add(new ColorProperty("surfaceTintColor", surfaceTintColor, defaultValue: null));
        textStyle?.debugFillProperties(properties, prefix: "textStyle.");
        properties.add(new DiagnosticsProperty<ShapeBorder>("shape", shape, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<bool>(
                "borderOnForeground",
                borderOnForeground,
                defaultValue: true
            )
        );
        properties.add(
            new DiagnosticsProperty<BorderRadiusGeometry>(
                "borderRadius",
                borderRadius,
                defaultValue: null
            )
        );
    }
}

internal class _MaterialState__material : State<Material>, TickerProviderStateMixin<Material>
{
    internal virtual GlobalKey<IState> _inkFeatureRenderer { get; private set; } =
        GlobalKey<IState>.Create(debugLabel: "ink renderer");
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override Widget build(BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        Color? backgroundColor = (Color?)(
            widget.color
            ?? (
                widget.type switch
                {
                    MaterialType.canvas => theme.canvasColor,
                    MaterialType.card => theme.cardColor,
                    MaterialType.button or MaterialType.circle =>
                        DartRuntimePrimitives.ConvertValue<Color>(null),
                    MaterialType.transparency => DartRuntimePrimitives.ConvertValue<Color>(null),
                    _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                        throw new InvalidOperationException("Non-exhaustive Dart switch value."),
                }
            )
        );
        Color modelShadowColor = widget.shadowColor ?? theme.colorScheme.shadow;
        DartRuntimePrimitives.Assert(
            () => (backgroundColor is not null) || Equals(widget.type, MaterialType.transparency),
            () =>
                (object?)"If Material type is not MaterialType.transparency, a color must "
                + "either be passed in through the `color` property, or be defined "
                + "in the theme (ex. canvasColor != null if type is set to "
                + "MaterialType.canvas)"
        );
        Widget? contents = widget.child;
        if (contents is not null)
        {
            contents = DartRuntimePrimitives.ConvertValue<Widget>(
                new AnimatedDefaultTextStyle(
                    style: widget.textStyle ?? Theme.of(context).textTheme.bodyMedium!,
                    duration: widget.animationDuration,
                    child: contents
                )
            );
        }
        contents = DartRuntimePrimitives.ConvertValue<Widget>(
            new NotificationListener<LayoutChangedNotification>(
                onNotification: (notification) =>
                {
                    var renderer = (
                        (_RenderInkFeatures__material?)
                            _inkFeatureRenderer.currentContext!.findRenderObject()!
                    )!;
                    renderer._didChangeLayout();
                    return false;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                },
                child: new _InkFeatures__material(
                    key: _inkFeatureRenderer,
                    absorbHitTest: !Equals(widget.type, MaterialType.transparency),
                    color: backgroundColor,
                    vsync: this,
                    child: contents
                )
            )
        );
        ShapeBorder? shapeLocal =
            (widget.borderRadius is not null)
                ? new RoundedRectangleBorder(borderRadius: widget.borderRadius!)
                : widget.shape;
        if (Equals(widget.type, MaterialType.canvas) && (shapeLocal is null))
        {
            Color colorLocal = ElevationOverlay.applySurfaceTint(
                backgroundColor!,
                widget.surfaceTintColor,
                widget.elevation
            );
            return new AnimatedPhysicalModel(
                curve: Curves.fastOutSlowIn,
                duration: widget.animationDuration,
                clipBehavior: widget.clipBehavior,
                elevation: widget.elevation,
                color: colorLocal,
                shadowColor: modelShadowColor,
                animateColor: widget.animateColor,
                child: contents
            );
        }
        shapeLocal ??= (
            widget.type switch
            {
                MaterialType.circle => DartRuntimePrimitives.ConvertValue<OutlinedBorder>(
                    new CircleBorder()
                ),
                MaterialType.canvas => DartRuntimePrimitives.ConvertValue<OutlinedBorder>(
                    new RoundedRectangleBorder()
                ),
                MaterialType.transparency => DartRuntimePrimitives.ConvertValue<OutlinedBorder>(
                    new RoundedRectangleBorder()
                ),
                MaterialType.card => DartRuntimePrimitives.ConvertValue<OutlinedBorder>(
                    new RoundedRectangleBorder(
                        borderRadius: BorderRadius.CreateAll(Radius.circular(2.0))
                    )
                ),
                MaterialType.button => DartRuntimePrimitives.ConvertValue<OutlinedBorder>(
                    new RoundedRectangleBorder(
                        borderRadius: BorderRadius.CreateAll(Radius.circular(2.0))
                    )
                ),
                _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                    throw new InvalidOperationException("Non-exhaustive Dart switch value."),
            }
        );
        if (Equals(widget.type, MaterialType.transparency))
        {
            return new ClipPath(
                clipper: new ShapeBorderClipper(
                    shape: shapeLocal,
                    textDirection: Directionality.maybeOf(context)
                ),
                clipBehavior: widget.clipBehavior,
                child: new _ShapeBorderPaint__material(shape: shapeLocal, child: contents)
            );
        }
        return new _MaterialInterior__material(
            curve: Curves.fastOutSlowIn,
            duration: widget.animationDuration,
            shape: shapeLocal,
            borderOnForeground: widget.borderOnForeground,
            clipBehavior: widget.clipBehavior,
            elevation: widget.elevation,
            color: backgroundColor!,
            shadowColor: modelShadowColor,
            surfaceTintColor: widget.surfaceTintColor,
            child: contents
        );
    }

    public virtual Scheduler.Ticker createTicker(Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = (
            (Func<_WidgetTicker__ticker_provider>)(
                () =>
                {
                    var __cascade = new _WidgetTicker__ticker_provider(
                        onTick,
                        this,
                        debugLabel: Foundation.ConstantsLibrary.kDebugMode
                            ? $"created by {DiagnosticsLibrary.describeIdentity(this)}"
                            : null
                    );
                    __cascade.muted = !values.enabled;
                    __cascade.forceFrames = values.forceFrames;
                    return __cascade;
                }
            )
        )();
        _tickers!.Add(result);
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(_WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => _tickers is not null);
        DartRuntimePrimitives.Assert(() => _tickers!.Contains(ticker));
        _tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if (_tickers is not null)
        {
            TickerModeData values = _tickerModeNotifier!.value;
            bool mutedLocal = !values.enabled;
            foreach (Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (_tickers is not null)
            {
                foreach (Scheduler.Ticker ticker in _tickers!)
                {
                    if (ticker.isActive)
                    {
                        throw DartRuntimePrimitives.AsException(
                            new FlutterError(
                                new List<DiagnosticsNode>
                                {
                                    new ErrorSummary($"{this} was disposed with an active Ticker."),
                                    new ErrorDescription(
                                        $"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time "
                                            + "dispose() was called on the mixin, that Ticker was still active. All Tickers must "
                                            + "be disposed before calling super.dispose()."
                                    ),
                                    new ErrorHint(
                                        "Tickers used by AnimationControllers "
                                            + "should be disposed by calling dispose() on the AnimationController itself. "
                                            + "Otherwise, the ticker will leak."
                                    ),
                                    ticker.describeForError("The offending ticker was"),
                                }
                            )
                        );
                    }
                }
            }
            return true;
        });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new DiagnosticsProperty<HashSet<Scheduler.Ticker>>(
                "tickers",
                _tickers,
                description: (_tickers is not null)
                    ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}"
                    : null,
                defaultValue: default
            )
        );
    }
}

public class _RenderInkFeatures__material : RenderProxyBox, MaterialInkController
{
    public virtual Scheduler.TickerProvider vsync { get; private set; } = default!;
    public virtual Color? color { get; set; } = default;
    public virtual bool absorbHitTest { get; set; } = default!;
    internal virtual List<InkFeature>? _inkFeatures { get; set; } = default;

    internal _RenderInkFeatures__material(
        RenderBox? child = null,
        Scheduler.TickerProvider vsync = default!,
        bool absorbHitTest = default!,
        Color? color = null
    )
        : base(child)
    {
        this.vsync = vsync;
        this.absorbHitTest = absorbHitTest;
        this.color = color;
    }

    public virtual List<InkFeature>? debugInkFeatures
    {
        get
        {
            if (Foundation.ConstantsLibrary.kDebugMode)
            {
                return _inkFeatures;
            }
            return null;
        }
    }

    public virtual void addInkFeature(InkFeature feature)
    {
        DartRuntimePrimitives.Assert(() => !feature._debugDisposed);
        DartRuntimePrimitives.Assert(() => Equals(feature._controller, this));
        _inkFeatures ??= new List<InkFeature>();
        DartRuntimePrimitives.Assert(() => !_inkFeatures!.Contains(feature));
        _inkFeatures!.Add(feature);
        markNeedsPaint();
    }

    internal virtual void _removeFeature(InkFeature feature)
    {
        DartRuntimePrimitives.Assert(() => _inkFeatures is not null);
        _inkFeatures!.Remove(feature);
        markNeedsPaint();
    }

    internal virtual void _didChangeLayout()
    {
        if (
            (
                _inkFeatures is { } __items23755
                    ? System.Linq.Enumerable.Any(__items23755)
                    : (bool?)null
            ) ?? false
        )
        {
            markNeedsPaint();
        }
    }

    public override bool hitTestSelf(Offset position) => absorbHitTest;

    public override void paint(PaintingContext context, Offset offset)
    {
        List<InkFeature>? inkFeatures = _inkFeatures;
        if ((inkFeatures is not null) && Enumerable.Any(inkFeatures))
        {
            Canvas canvasLocal = context.canvas;
            canvasLocal.save();
            canvasLocal.translate(offset.dx, offset.dy);
            canvasLocal.clipRect(Offset.zero & size);
            foreach (InkFeature inkFeature in inkFeatures)
            {
                inkFeature._paint(canvasLocal);
            }
            canvasLocal.restore();
        }
        DartRuntimePrimitives.Assert(() => Equals(inkFeatures, _inkFeatures));
        base.paint(context, offset);
    }
}

internal class _InkFeatures__material : SingleChildRenderObjectWidget
{
    public virtual Color? color { get; private set; }
    public virtual Scheduler.TickerProvider vsync { get; private set; } = default!;
    public virtual bool absorbHitTest { get; private set; } = default!;

    internal _InkFeatures__material(
        Key? key = null,
        Color? color = null,
        Scheduler.TickerProvider vsync = default!,
        bool absorbHitTest = default!,
        Widget? child = null
    )
        : base(key: key, child: child)
    {
        this.color = color;
        this.vsync = vsync;
        this.absorbHitTest = absorbHitTest;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderInkFeatures__material(
            color: color,
            absorbHitTest: absorbHitTest,
            vsync: vsync
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderInkFeatures__material)renderObject;
        DartRuntimePrimitives.Ignore(
            (
                (Func<_RenderInkFeatures__material>)(
                    () =>
                    {
                        var __cascade = __renderObject;
                        __cascade.color = color;
                        __cascade.absorbHitTest = absorbHitTest;
                        return __cascade;
                    }
                )
            )()
        );
        DartRuntimePrimitives.Assert(() => Equals(vsync, __renderObject.vsync));
    }
}

public abstract class InkFeature
{
    internal virtual _RenderInkFeatures__material _controller { get; private set; } = default!;
    public virtual RenderBox referenceBox { get; private set; } = default!;
    public virtual Action? onRemoved { get; private set; }
    internal virtual bool _debugDisposed { get; set; } = false;

    protected InkFeature(
        MaterialInkController controller,
        RenderBox referenceBox,
        Action? onRemoved = null
    )
    {
        this.referenceBox = referenceBox;
        this.onRemoved = onRemoved;
        _controller = ((_RenderInkFeatures__material?)controller)!;
        DartRuntimePrimitives.Assert(() =>
            Foundation.DebugLibrary.debugMaybeDispatchCreated("material", "InkFeature", this)
        );
    }

    public virtual MaterialInkController controller =>
        DartRuntimePrimitives.ConvertValue<MaterialInkController>(_controller);

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => !_debugDisposed);
        DartRuntimePrimitives.Assert(() =>
        {
            _debugDisposed = true;
            return true;
        });
        DartRuntimePrimitives.Assert(() =>
            Foundation.DebugLibrary.debugMaybeDispatchDisposed(this)
        );
        _controller._removeFeature(this);
        onRemoved?.Invoke();
    }

    internal static Matrix4? _getPaintTransform(
        RenderObject fromRenderObject,
        RenderObject toRenderObject
    )
    {
        var fromPath = new List<RenderObject> { fromRenderObject };
        var toPath = new List<RenderObject> { toRenderObject };
        var @from = fromRenderObject;
        var to = toRenderObject;
        while (!DartRuntimePrimitives.Identical(@from, to))
        {
            long fromDepth = @from.depth;
            long toDepth = to.depth;
            if (fromDepth >= toDepth)
            {
                RenderObject? fromParent = @from.parent;
                if (fromParent is null || !fromParent.paintsChild(@from))
                {
                    return null;
                }
                fromPath.Add(fromParent);
                @from = fromParent;
            }
            if (fromDepth <= toDepth)
            {
                RenderObject? toParent = to.parent;
                if (toParent is null || !toParent.paintsChild(to))
                {
                    return null;
                }
                toPath.Add(toParent);
                to = toParent;
            }
        }
        DartRuntimePrimitives.Assert(() => DartRuntimePrimitives.Identical(@from, to));
        var transform = Matrix4.identity();
        var inverseTransform = Matrix4.identity();
        for (long index = checked(toPath.Count) - 1L; index > 0L; index -= 1L)
        {
            toPath[(int)index].applyPaintTransform(toPath[(int)(index - 1L)], transform);
        }
        for (long indexLocal = checked(fromPath.Count) - 1L; indexLocal > 0L; indexLocal -= 1L)
        {
            fromPath[(int)indexLocal]
                .applyPaintTransform(fromPath[(int)(indexLocal - 1L)], inverseTransform);
        }
        double det = inverseTransform.invert();
        return (det != 0L)
            ? (
                (Func<Matrix4>)(
                    () =>
                    {
                        var __cascade = inverseTransform;
                        __cascade.multiply(transform);
                        return __cascade;
                    }
                )
            )()
            : null;
    }

    internal virtual void _paint(Canvas canvas)
    {
        DartRuntimePrimitives.Assert(() => referenceBox.attached);
        DartRuntimePrimitives.Assert(() => !_debugDisposed);
        Matrix4? transform = _getPaintTransform(_controller, referenceBox);
        if (transform is not null)
        {
            paintFeature(canvas, transform);
        }
    }

    public virtual void paintFeature(Canvas canvas, Matrix4 transform) { }

    public override string ToString() => DiagnosticsLibrary.describeIdentity(this);
}

public class ShapeBorderTween : Tween<ShapeBorder?>
{
    public ShapeBorderTween(ShapeBorder? begin = null, ShapeBorder? end = null)
        : base(begin: begin, end: end) { }

    public override ShapeBorder? lerp(double t)
    {
        return ShapeBorder.lerp(begin, end, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _MaterialInterior__material : ImplicitlyAnimatedWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual ShapeBorder shape { get; private set; } = default!;
    public virtual bool borderOnForeground { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual double elevation { get; private set; } = default!;
    public virtual Color color { get; private set; } = default!;
    public virtual Color shadowColor { get; private set; } = default!;
    public virtual Color? surfaceTintColor { get; private set; }

    internal _MaterialInterior__material(
        Widget child,
        ShapeBorder shape,
        bool borderOnForeground = true,
        Clip clipBehavior = Clip.none,
        double elevation = default!,
        Color color = default!,
        Color shadowColor = default!,
        Color? surfaceTintColor = default!,
        Curve curve = default!,
        Duration duration = default!
    )
        : base(curve: curve ?? Curves.linear, duration: duration)
    {
        this.child = child;
        this.shape = shape;
        this.borderOnForeground = borderOnForeground;
        this.clipBehavior = clipBehavior;
        this.elevation = elevation;
        this.color = color;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        System.Diagnostics.Debug.Assert(elevation >= 0.0);
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _MaterialInteriorState__material());

    public override void debugFillProperties(DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(new DiagnosticsProperty<ShapeBorder>("shape", shape));
        description.add(new DoubleProperty("elevation", elevation));
        description.add(new ColorProperty("color", color));
        description.add(new ColorProperty("shadowColor", shadowColor));
    }
}

internal class _MaterialInteriorState__material
    : AnimatedWidgetBaseState<_MaterialInterior__material>
{
    internal virtual Tween<double>? _elevation { get; set; } = default;
    internal virtual ColorTween? _surfaceTintColor { get; set; } = default;
    internal virtual ColorTween? _shadowColor { get; set; } = default;
    internal virtual ShapeBorderTween? _border { get; set; } = default;

    public override void forEachTween(
        Func<IDartTween?, object?, Func<object, IDartTween>, IDartTween?> visitor
    )
    {
        _elevation = (
            (Tween<double>?)visitor(
                _elevation,
                widget.elevation,
                (value) => new Tween<double>(begin: (double)value)
            )
        )!;
        _shadowColor = (
            (ColorTween?)visitor(
                _shadowColor,
                widget.shadowColor,
                (value) => new ColorTween(begin: ((Color?)value)!)
            )
        )!;
        _surfaceTintColor =
            (widget.surfaceTintColor is not null)
                ? (
                    (ColorTween?)visitor(
                        _surfaceTintColor,
                        widget.surfaceTintColor,
                        (value) => new ColorTween(begin: ((Color?)value)!)
                    )
                )!
                : null;
        _border = (
            (ShapeBorderTween?)visitor(
                _border,
                widget.shape,
                (value) => new ShapeBorderTween(begin: ((ShapeBorder?)value)!)
            )
        )!;
    }

    public override Widget build(BuildContext context)
    {
        ShapeBorder shapeLocal = _border!.evaluate(animation)!;
        double elevationLocal = _elevation!.evaluate(animation);
        Color colorLocal = ElevationOverlay.applySurfaceTint(
            widget.color,
            _surfaceTintColor?.evaluate(animation),
            elevationLocal
        );
        Color shadowColorLocal = _shadowColor!.evaluate(animation)!;
        return new PhysicalShape(
            clipper: new ShapeBorderClipper(
                shape: shapeLocal,
                textDirection: Directionality.maybeOf(context)
            ),
            clipBehavior: widget.clipBehavior,
            elevation: elevationLocal,
            color: colorLocal,
            shadowColor: shadowColorLocal,
            child: new _ShapeBorderPaint__material(
                shape: shapeLocal,
                borderOnForeground: widget.borderOnForeground,
                child: widget.child
            )
        );
    }
}

internal class _ShapeBorderPaint__material : StatelessWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual ShapeBorder shape { get; private set; } = default!;
    public virtual bool borderOnForeground { get; private set; } = default!;

    internal _ShapeBorderPaint__material(
        Widget child,
        ShapeBorder shape,
        bool borderOnForeground = true
    )
    {
        this.child = child;
        this.shape = shape;
        this.borderOnForeground = borderOnForeground;
    }

    public override Widget build(BuildContext context)
    {
        return new CustomPaint(
            painter: borderOnForeground
                ? null
                : new _ShapeBorderPainter__material(shape, Directionality.maybeOf(context)),
            foregroundPainter: borderOnForeground
                ? new _ShapeBorderPainter__material(shape, Directionality.maybeOf(context))
                : null,
            child: child
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _ShapeBorderPainter__material : CustomPainter
{
    public virtual ShapeBorder border { get; private set; } = default!;
    public virtual TextDirection? textDirection { get; private set; }

    internal _ShapeBorderPainter__material(ShapeBorder border, TextDirection? textDirection)
    {
        this.border = border;
        this.textDirection = textDirection;
    }

    public override void paint(Canvas canvas, Size size)
    {
        border.paint(canvas, Offset.zero & size, textDirection: textDirection);
    }

    public override bool shouldRepaint(CustomPainter oldDelegate)
    {
        var __oldDelegate = (_ShapeBorderPainter__material)oldDelegate;
        return !Equals(__oldDelegate.border, border);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
