// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/material.dart
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

public delegate Rect RectCallback();

public enum MaterialType
{
    canvas,
    card,
    circle,
    button,
    transparency
}

public static partial class MaterialLibrary
{
    public static DartMap<MaterialType, global::Doroti.Generated.Framework.Painting.BorderRadius?> kMaterialEdges = new DartMap<MaterialType, global::Doroti.Generated.Framework.Painting.BorderRadius?> { [MaterialType.canvas] = ((global::Doroti.Generated.Framework.Painting.BorderRadius?)(object?)null), [MaterialType.card] = global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Flutter.Ui.Radius.circular(2.0)), [MaterialType.circle] = ((global::Doroti.Generated.Framework.Painting.BorderRadius?)(object?)null), [MaterialType.button] = global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Flutter.Ui.Radius.circular(2.0)), [MaterialType.transparency] = ((global::Doroti.Generated.Framework.Painting.BorderRadius?)(object?)null) };
}

public interface MaterialInkController
{
    public global::Doroti.Flutter.Ui.Color? color { get; }
    public global::Doroti.Generated.Framework.Scheduler.TickerProvider vsync { get; }
    public void addInkFeature(InkFeature feature);
    public void markNeedsPaint();
}

public class Material : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? child { get; private set; }
    public virtual MaterialType type { get; private set; } = default!;
    public virtual bool animateColor { get; private set; } = default!;
    public virtual double elevation { get; private set; } = default!;
    public virtual Color? color { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? textStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual bool borderOnForeground { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual Duration animationDuration { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry? borderRadius { get; private set; }
    public const double defaultSplashRadius = 35.0;

    public Material(global::Doroti.Generated.Framework.Foundation.Key? key = null, MaterialType type = MaterialType.canvas, double elevation = 0.0, Color? color = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? textStyle = null, global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry? borderRadius = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, bool borderOnForeground = true, Clip clipBehavior = Clip.none, Duration? animationDuration = null, global::Doroti.Generated.Framework.Widgets.Widget? child = null, bool animateColor = false) : base(key: key)
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
        System.Diagnostics.Debug.Assert((elevation >= 0.0));
        System.Diagnostics.Debug.Assert(!(((shape is not null) && (borderRadius is not null))));
        System.Diagnostics.Debug.Assert(!((DartRuntimePrimitives.Identical(type, MaterialType.circle) && (((borderRadius is not null) || (shape is not null))))));
    }

    public static MaterialInkController? maybeOf(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((MaterialInkController?)(object?)LookupBoundary.findAncestorRenderObjectOfType<_RenderInkFeatures__material>(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static MaterialInkController of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        MaterialInkController? controller__15762 = ((MaterialInkController?)(object?)Material.maybeOf(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((controller__15762 is null))
                {
                    if (LookupBoundary.debugIsHidingAncestorRenderObjectOfType<_RenderInkFeatures__material>(context))
                    {
                        throw DartRuntimePrimitives.AsException(global::Doroti.Generated.Framework.Foundation.FlutterError.Create("Material.of() was called with a context that does not have access to a Material widget.\n" + "The context provided to Material.of() does have a Material widget ancestor, but it is " + "hidden by a LookupBoundary. This can happen because you are using a widget that looks " + "for a Material ancestor, but no such ancestor exists within the closest LookupBoundary.\n" + "The context used was:\n" + $"  {context}"));
                    }
                    throw DartRuntimePrimitives.AsException(global::Doroti.Generated.Framework.Foundation.FlutterError.Create("Material.of() was called with a context that does not contain a Material widget.\n" + "No Material widget ancestor could be found starting from the context that was passed to " + "Material.of(). This can happen because you are using a widget that looks for a Material " + "ancestor, but no such ancestor exists.\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
            });
        return controller__15762!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _MaterialState__material());
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<MaterialType>("type", this.type));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("elevation", this.elevation, defaultValue: 0.0));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("color", this.color, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("shadowColor", this.shadowColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("surfaceTintColor", this.surfaceTintColor, defaultValue: null));
        this.textStyle?.debugFillProperties(properties, prefix: "textStyle.");
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.ShapeBorder>("shape", this.shape, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("borderOnForeground", this.borderOnForeground, defaultValue: true));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry>("borderRadius", this.borderRadius, defaultValue: null));
    }

}

internal class _MaterialState__material : global::Doroti.Generated.Framework.Widgets.State<Material>, global::Doroti.Generated.Framework.Widgets.TickerProviderStateMixin<Material>
{
    internal virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> _inkFeatureRenderer { get; private set; } = global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create(debugLabel: "ink renderer");
    public virtual HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ThemeData theme__18315 = Theme.of(context);
        global::Doroti.Flutter.Ui.Color? backgroundColor__18359 = ((global::Doroti.Flutter.Ui.Color?)(object?)(((Material)this.widget).color ?? (((Material)this.widget).type switch { MaterialType.canvas => theme__18315.canvasColor, MaterialType.card => theme__18315.cardColor, MaterialType.button or MaterialType.circle => DartRuntimePrimitives.ConvertValue<Color>(null), MaterialType.transparency => DartRuntimePrimitives.ConvertValue<Color>(null), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })));
        global::Doroti.Flutter.Ui.Color modelShadowColor__18650 = ((global::Doroti.Flutter.Ui.Color)(object?)(((Material)this.widget).shadowColor ?? ((theme__18315.useMaterial3 ? theme__18315.colorScheme.shadow : theme__18315.shadowColor))));
        DartRuntimePrimitives.Assert(() => ((backgroundColor__18359 is not null) || (object.Equals(((Material)this.widget).type, MaterialType.transparency))), () => (object?)"If Material type is not MaterialType.transparency, a color must " + "either be passed in through the `color` property, or be defined " + "in the theme (ex. canvasColor != null if type is set to " + "MaterialType.canvas)");
        global::Doroti.Generated.Framework.Widgets.Widget? contents__19116 = ((Material)this.widget).child;
        if ((contents__19116 is not null))
        {
            contents__19116 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.AnimatedDefaultTextStyle(style: (((Material)this.widget).textStyle ?? Theme.of(context).textTheme.bodyMedium!), duration: ((Material)this.widget).animationDuration, child: contents__19116));
        }
        contents__19116 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.NotificationListener<global::Doroti.Generated.Framework.Widgets.LayoutChangedNotification>(onNotification: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.LayoutChangedNotification, bool>)((notification) => {
var renderer__19515 = ((_RenderInkFeatures__material?)(object?)((global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>)this._inkFeatureRenderer).currentContext!.findRenderObject()!)!;
renderer__19515._didChangeLayout();
return false;
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: new _InkFeatures__material(key: this._inkFeatureRenderer, absorbHitTest: (!object.Equals(((Material)this.widget).type, MaterialType.transparency)), color: backgroundColor__18359, vsync: this, child: contents__19116)));
        global::Doroti.Generated.Framework.Painting.ShapeBorder? shape__19923 = ((((Material)this.widget).borderRadius is not null) ? new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(borderRadius: ((Material)this.widget).borderRadius!) : ((Material)this.widget).shape);
        if (((object.Equals(((Material)this.widget).type, MaterialType.canvas)) && (shape__19923 is null)))
        {
            global::Doroti.Flutter.Ui.Color color__20810 = ((global::Doroti.Flutter.Ui.Color)(object?)(theme__18315.useMaterial3 ? ElevationOverlay.applySurfaceTint(backgroundColor__18359!, ((Material)this.widget).surfaceTintColor, ((Material)this.widget).elevation) : ElevationOverlay.applyOverlay(context, backgroundColor__18359!, ((Material)this.widget).elevation)));
            return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.AnimatedPhysicalModel(curve: global::Doroti.Generated.Framework.Animation.Curves.fastOutSlowIn, duration: ((Material)this.widget).animationDuration, clipBehavior: ((Material)this.widget).clipBehavior, elevation: ((Material)this.widget).elevation, color: color__20810, shadowColor: modelShadowColor__18650, animateColor: ((Material)this.widget).animateColor, child: contents__19116));
        }
        shape__19923 ??= (((Material)this.widget).type switch { MaterialType.circle => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.OutlinedBorder>(new global::Doroti.Generated.Framework.Painting.CircleBorder()), MaterialType.canvas => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.OutlinedBorder>(new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder()), MaterialType.transparency => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.OutlinedBorder>(new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder()), MaterialType.card => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.OutlinedBorder>(new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Flutter.Ui.Radius.circular(2.0)))), MaterialType.button => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.OutlinedBorder>(new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Flutter.Ui.Radius.circular(2.0)))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        if ((object.Equals(((Material)this.widget).type, MaterialType.transparency)))
        {
            return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.ClipPath(clipper: new global::Doroti.Generated.Framework.Rendering.ShapeBorderClipper(shape: shape__19923, textDirection: Directionality.maybeOf(context)), clipBehavior: ((Material)this.widget).clipBehavior, child: new _ShapeBorderPaint__material(shape: shape__19923, child: contents__19116)));
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _MaterialInterior__material(curve: global::Doroti.Generated.Framework.Animation.Curves.fastOutSlowIn, duration: ((Material)this.widget).animationDuration, shape: shape__19923, borderOnForeground: ((Material)this.widget).borderOnForeground, clipBehavior: ((Material)this.widget).clipBehavior, elevation: ((Material)this.widget).elevation, color: backgroundColor__18359!, shadowColor: modelShadowColor__18650, surfaceTintColor: ((Material)this.widget).surfaceTintColor, child: contents__19116));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>();
        TickerModeData values__17506 = this._tickerModeNotifier!.value;
        var result__17553 = ((Func<global::Doroti.Generated.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{            var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
            __cascade.muted = !((TickerModeData)values__17506).enabled;
            __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
            return __cascade;        }))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Generated.Framework.Scheduler.Ticker)(object?)result__17553);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Generated.Framework.Widgets._WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => (this._tickers is not null));
        DartRuntimePrimitives.Assert(() => this._tickers!.Contains(ticker));
        this._tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if ((this._tickers is not null))
        {
            TickerModeData values__18318 = this._tickerModeNotifier!.value;
            bool muted__18372 = !((TickerModeData)values__18318).enabled;
            foreach (global::Doroti.Generated.Framework.Scheduler.Ticker ticker__18421 in this._tickers!)
            {
                ticker__18421.muted = muted__18372;
                ticker__18421.forceFrames = ((TickerModeData)values__18318).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__18621 = ((global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__18621, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier__18621.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier__18621;
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Generated.Framework.Scheduler.Ticker ticker__18989 in this._tickers!)
                    {
                        if (((global::Doroti.Generated.Framework.Scheduler.Ticker)ticker__18989).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker__18989.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        this._tickerModeNotifier = null;
        base.dispose();
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

public class _RenderInkFeatures__material : global::Doroti.Generated.Framework.Rendering.RenderProxyBox, MaterialInkController
{
    public virtual global::Doroti.Generated.Framework.Scheduler.TickerProvider vsync { get; private set; } = default!;
    public virtual Color? color { get; set; } = default;
    public virtual bool absorbHitTest { get; set; } = default!;
    internal virtual List<InkFeature>? _inkFeatures { get; set; } = default;

    internal _RenderInkFeatures__material(global::Doroti.Generated.Framework.Rendering.RenderBox? child = null, global::Doroti.Generated.Framework.Scheduler.TickerProvider vsync = default!, bool absorbHitTest = default!, Color? color = null) : base(child)
    {
        this.vsync = vsync;
        this.absorbHitTest = absorbHitTest;
        this.color = color;
    }

    public virtual List<InkFeature>? debugInkFeatures
    {
        get
        {
            if (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode)
            {
                return this._inkFeatures;
            }
            return null;
            return default!;
        }
    }
    public virtual void addInkFeature(InkFeature feature)
    {
        DartRuntimePrimitives.Assert(() => !((InkFeature)feature)._debugDisposed);
        DartRuntimePrimitives.Assert(() => (object.Equals(((InkFeature)feature)._controller, this)));
        _inkFeatures ??= new List<InkFeature>();
        DartRuntimePrimitives.Assert(() => !this._inkFeatures!.Contains(feature));
        this._inkFeatures!.Add(feature);
        markNeedsPaint();
    }

    internal virtual void _removeFeature(InkFeature feature)
    {
        DartRuntimePrimitives.Assert(() => (this._inkFeatures is not null));
        this._inkFeatures!.Remove(feature);
        markNeedsPaint();
    }

    internal virtual void _didChangeLayout()
    {
        if (((this._inkFeatures is { } __items23755 ? System.Linq.Enumerable.Any(__items23755) : (bool?)null) ?? false))
        {
            markNeedsPaint();
        }
    }

    public override bool hitTestSelf(Offset position) => this.absorbHitTest;
    public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        List<InkFeature>? inkFeatures__23989 = this._inkFeatures;
        if (((inkFeatures__23989 is not null) && System.Linq.Enumerable.Any(inkFeatures__23989)))
        {
            global::Doroti.Flutter.Ui.Canvas canvas__24093 = ((global::Doroti.Flutter.Ui.Canvas)(object?)((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas);
            canvas__24093.save();
            canvas__24093.translate(offset.dx, offset.dy);
            canvas__24093.clipRect((Offset.zero & this.size));
            foreach (InkFeature inkFeature__24256 in inkFeatures__23989)
            {
                inkFeature__24256._paint(canvas__24093);
            }
            canvas__24093.restore();
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(inkFeatures__23989, this._inkFeatures)));
        base.paint(context, offset);
    }

}

internal class _InkFeatures__material : global::Doroti.Generated.Framework.Widgets.SingleChildRenderObjectWidget
{
    public virtual Color? color { get; private set; }
    public virtual global::Doroti.Generated.Framework.Scheduler.TickerProvider vsync { get; private set; } = default!;
    public virtual bool absorbHitTest { get; private set; } = default!;

    internal _InkFeatures__material(global::Doroti.Generated.Framework.Foundation.Key? key = null, Color? color = null, global::Doroti.Generated.Framework.Scheduler.TickerProvider vsync = default!, bool absorbHitTest = default!, global::Doroti.Generated.Framework.Widgets.Widget? child = null) : base(key: key, child: child)
    {
        this.color = color;
        this.vsync = vsync;
        this.absorbHitTest = absorbHitTest;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _RenderInkFeatures__material(color: this.color, absorbHitTest: this.absorbHitTest, vsync: this.vsync));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderInkFeatures__material)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderInkFeatures__material>)(() =>
{            var __cascade = __renderObject;
            __cascade.color = this.color;
            __cascade.absorbHitTest = this.absorbHitTest;
            return __cascade;        }))());
        DartRuntimePrimitives.Assert(() => (object.Equals(this.vsync, ((_RenderInkFeatures__material)__renderObject).vsync)));
    }

}

public abstract class InkFeature
{
    internal virtual _RenderInkFeatures__material _controller { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.RenderBox referenceBox { get; private set; } = default!;
    public virtual global::System.Action? onRemoved { get; private set; }
    internal virtual bool _debugDisposed { get; set; } = false;

    protected InkFeature(MaterialInkController controller, global::Doroti.Generated.Framework.Rendering.RenderBox referenceBox, global::System.Action? onRemoved = null)
    {
        this.referenceBox = referenceBox;
        this.onRemoved = onRemoved;
        this._controller = ((_RenderInkFeatures__material?)(object?)controller)!;
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugMaybeDispatchCreated("material", "InkFeature", this));
    }

    public virtual MaterialInkController controller => DartRuntimePrimitives.ConvertValue<MaterialInkController>(this._controller);
    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => !this._debugDisposed);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugDisposed = true;
                return true;
            });
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        this._controller._removeFeature(this);
        this.onRemoved?.Invoke();
    }

    internal static Matrix4? _getPaintTransform(global::Doroti.Generated.Framework.Rendering.RenderObject fromRenderObject, global::Doroti.Generated.Framework.Rendering.RenderObject toRenderObject)
    {
        var fromPath__27181 = new List<global::Doroti.Generated.Framework.Rendering.RenderObject> { fromRenderObject };
        var toPath__27236 = new List<global::Doroti.Generated.Framework.Rendering.RenderObject> { toRenderObject };
        var from__27286 = fromRenderObject;
        var to__27319 = toRenderObject;
        while (!DartRuntimePrimitives.Identical(from__27286, to__27319))
        {
            long fromDepth__27392 = ((global::Doroti.Generated.Framework.Rendering.RenderObject)from__27286).depth;
            long toDepth__27432 = ((global::Doroti.Generated.Framework.Rendering.RenderObject)to__27319).depth;
            if ((fromDepth__27392 >= toDepth__27432))
            {
                global::Doroti.Generated.Framework.Rendering.RenderObject? fromParent__27515 = ((global::Doroti.Generated.Framework.Rendering.RenderObject)from__27286).parent;
                if ((false || !((bool)((dynamic)fromParent__27515).paintsChild(from__27286))))
                {
                    return null;
                }
                fromPath__27181.Add(fromParent__27515);
                from__27286 = fromParent__27515;
            }
            if ((fromDepth__27392 <= toDepth__27432))
            {
                global::Doroti.Generated.Framework.Rendering.RenderObject? toParent__27933 = ((global::Doroti.Generated.Framework.Rendering.RenderObject)to__27319).parent;
                if ((false || !((bool)((dynamic)toParent__27933).paintsChild(to__27319))))
                {
                    return null;
                }
                toPath__27236.Add(toParent__27933);
                to__27319 = toParent__27933;
            }
        }
        DartRuntimePrimitives.Assert(() => DartRuntimePrimitives.Identical(from__27286, to__27319));
        var transform__28169 = Matrix4.identity();
        var inverseTransform__28211 = Matrix4.identity();
        for (long index__28264 = (checked((long)(toPath__27236.Count)) - 1L); (index__28264 > 0L); index__28264 -= 1L)
        {
            ((dynamic)toPath__27236[(int)(index__28264)]).applyPaintTransform(toPath__27236[(int)((index__28264 - 1L))], transform__28169);
        }
        for (long index__28406 = (checked((long)(fromPath__27181.Count)) - 1L); (index__28406 > 0L); index__28406 -= 1L)
        {
            ((dynamic)fromPath__27181[(int)(index__28406)]).applyPaintTransform(fromPath__27181[(int)((index__28406 - 1L))], inverseTransform__28211);
        }
        double det__28566 = inverseTransform__28211.invert();
        return ((det__28566 != 0L) ? (((Func<Matrix4>)(() =>
{            var __cascade = inverseTransform__28211;
            __cascade.multiply(transform__28169);
            return __cascade;        }))()) : null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _paint(Canvas canvas)
    {
        DartRuntimePrimitives.Assert(() => this.referenceBox.attached);
        DartRuntimePrimitives.Assert(() => !this._debugDisposed);
        Matrix4? transform__28869 = ((Matrix4?)(object?)InkFeature._getPaintTransform(this._controller, this.referenceBox));
        if ((transform__28869 is not null))
        {
            paintFeature(canvas, transform__28869);
        }
    }

    public virtual void paintFeature(Canvas canvas, Matrix4 transform) { }
    public override string ToString() => global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
}

public class ShapeBorderTween : global::Doroti.Generated.Framework.Animation.Tween<global::Doroti.Generated.Framework.Painting.ShapeBorder?>
{
    public ShapeBorderTween(global::Doroti.Generated.Framework.Painting.ShapeBorder? begin = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? end = null) : base(begin: begin, end: end)
    {
    }

    public override global::Doroti.Generated.Framework.Painting.ShapeBorder? lerp(double t)
    {
        return ((global::Doroti.Generated.Framework.Painting.ShapeBorder?)(object?)ShapeBorder.lerp(this.begin, this.end, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MaterialInterior__material : global::Doroti.Generated.Framework.Widgets.ImplicitlyAnimatedWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder shape { get; private set; } = default!;
    public virtual bool borderOnForeground { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual double elevation { get; private set; } = default!;
    public virtual Color color { get; private set; } = default!;
    public virtual Color shadowColor { get; private set; } = default!;
    public virtual Color? surfaceTintColor { get; private set; }

    internal _MaterialInterior__material(global::Doroti.Generated.Framework.Widgets.Widget child, global::Doroti.Generated.Framework.Painting.ShapeBorder shape, bool borderOnForeground = true, Clip clipBehavior = Clip.none, double elevation = default!, Color color = default!, Color shadowColor = default!, Color? surfaceTintColor = default!, global::Doroti.Generated.Framework.Animation.Curve curve = default!, Duration duration = default!) : base(curve: curve ?? global::Doroti.Generated.Framework.Animation.Curves.linear, duration: duration)
    {
        this.child = child;
        this.shape = shape;
        this.borderOnForeground = borderOnForeground;
        this.clipBehavior = clipBehavior;
        this.elevation = elevation;
        this.color = color;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        System.Diagnostics.Debug.Assert((elevation >= 0.0));
    }

    public override _MaterialInteriorState__material createState() => new _MaterialInteriorState__material();
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.ShapeBorder>("shape", this.shape));
        description.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("elevation", this.elevation));
        description.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("color", this.color));
        description.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("shadowColor", this.shadowColor));
    }

}

internal class _MaterialInteriorState__material : global::Doroti.Generated.Framework.Widgets.AnimatedWidgetBaseState<_MaterialInterior__material>
{
    internal virtual global::Doroti.Generated.Framework.Animation.Tween<double>? _elevation { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.ColorTween? _surfaceTintColor { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.ColorTween? _shadowColor { get; set; } = default;
    internal virtual ShapeBorderTween? _border { get; set; } = default;

    public override void forEachTween(global::System.Func<global::Doroti.Generated.Framework.Animation.IDartTween?, object, global::System.Func<object, global::Doroti.Generated.Framework.Animation.IDartTween>, global::Doroti.Generated.Framework.Animation.IDartTween?> visitor)
    {
        _elevation = ((global::Doroti.Generated.Framework.Animation.Tween<double>?)(object?)visitor(this._elevation, ((_MaterialInterior__material)this.widget).elevation, ((value) => new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: ((double)value)))))!;
        _shadowColor = ((global::Doroti.Generated.Framework.Animation.ColorTween?)(object?)visitor(this._shadowColor, ((_MaterialInterior__material)this.widget).shadowColor, ((value) => new global::Doroti.Generated.Framework.Animation.ColorTween(begin: ((global::Doroti.Flutter.Ui.Color?)(object?)value)!))))!;
        _surfaceTintColor = ((((_MaterialInterior__material)this.widget).surfaceTintColor is not null) ? ((global::Doroti.Generated.Framework.Animation.ColorTween?)(object?)visitor(this._surfaceTintColor, ((_MaterialInterior__material)this.widget).surfaceTintColor, ((value) => new global::Doroti.Generated.Framework.Animation.ColorTween(begin: ((global::Doroti.Flutter.Ui.Color?)(object?)value)!))))! : null);
        _border = ((ShapeBorderTween?)(object?)visitor(this._border, ((_MaterialInterior__material)this.widget).shape, ((value) => new ShapeBorderTween(begin: ((global::Doroti.Generated.Framework.Painting.ShapeBorder?)(object?)value)!))))!;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Generated.Framework.Painting.ShapeBorder shape__33252 = this._border!.evaluate(this.animation)!;
        double elevation__33308 = this._elevation!.evaluate(this.animation);
        global::Doroti.Flutter.Ui.Color color__33369 = ((global::Doroti.Flutter.Ui.Color)(object?)(Theme.of(context).useMaterial3 ? ElevationOverlay.applySurfaceTint(((_MaterialInterior__material)this.widget).color, this._surfaceTintColor?.evaluate(this.animation), elevation__33308) : ElevationOverlay.applyOverlay(context, ((_MaterialInterior__material)this.widget).color, elevation__33308)));
        global::Doroti.Flutter.Ui.Color shadowColor__33657 = ((global::Doroti.Flutter.Ui.Color)(object?)this._shadowColor!.evaluate(this.animation)!);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.PhysicalShape(clipper: new global::Doroti.Generated.Framework.Rendering.ShapeBorderClipper(shape: shape__33252, textDirection: Directionality.maybeOf(context)), clipBehavior: ((_MaterialInterior__material)this.widget).clipBehavior, elevation: elevation__33308, color: color__33369, shadowColor: shadowColor__33657, child: new _ShapeBorderPaint__material(shape: shape__33252, borderOnForeground: ((_MaterialInterior__material)this.widget).borderOnForeground, child: ((_MaterialInterior__material)this.widget).child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ShapeBorderPaint__material : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder shape { get; private set; } = default!;
    public virtual bool borderOnForeground { get; private set; } = default!;

    internal _ShapeBorderPaint__material(global::Doroti.Generated.Framework.Widgets.Widget child, global::Doroti.Generated.Framework.Painting.ShapeBorder shape, bool borderOnForeground = true)
    {
        this.child = child;
        this.shape = shape;
        this.borderOnForeground = borderOnForeground;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.CustomPaint(painter: (this.borderOnForeground ? null : new _ShapeBorderPainter__material(this.shape, Directionality.maybeOf(context))), foregroundPainter: (this.borderOnForeground ? new _ShapeBorderPainter__material(this.shape, Directionality.maybeOf(context)) : null), child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ShapeBorderPainter__material : global::Doroti.Generated.Framework.Rendering.CustomPainter
{
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder border { get; private set; } = default!;
    public virtual TextDirection? textDirection { get; private set; }

    internal _ShapeBorderPainter__material(global::Doroti.Generated.Framework.Painting.ShapeBorder border, TextDirection? textDirection)
    {
        this.border = border;
        this.textDirection = textDirection;
    }

    public override void paint(Canvas canvas, Size size)
    {
        this.border.paint(canvas, (Offset.zero & size), textDirection: this.textDirection);
    }

    public override bool shouldRepaint(global::Doroti.Generated.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __oldDelegate = (_ShapeBorderPainter__material)(object)oldDelegate;
        return (!object.Equals(((_ShapeBorderPainter__material)__oldDelegate).border, this.border));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
