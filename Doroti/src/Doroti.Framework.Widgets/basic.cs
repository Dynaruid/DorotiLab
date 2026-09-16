// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/basic.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

internal class _UbiquitousInheritedElement__basic : InheritedElement
{
    internal _UbiquitousInheritedElement__basic(InheritedWidget widget) : base(widget)
    {
    }

    public override void setDependencies(Element dependent, object? value)
    {
        DartRuntimePrimitives.Assert(() => value is null);
    }

    public override object? getDependencies(Element dependent)
    {
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void notifyClients(ProxyWidget oldWidget)
    {
        var __oldWidget = (InheritedWidget)oldWidget;
        _recurseChildren(this, (element) =>
        {
            if (element.doesDependOnInheritedElement(this))
            {
                notifyDependent(__oldWidget, element);
            }
        });
    }

    internal static void _recurseChildren(Element element, Action<Element> visitor)
    {
        element.visitChildren((child) =>
        {
            _recurseChildren(child, visitor);
        });
        visitor(element);
    }

}

public abstract class _UbiquitousInheritedWidget__basic : InheritedWidget
{
    internal _UbiquitousInheritedWidget__basic(Key? key = null, Widget child = default!) : base(key: key, child: child)
    {
    }

    public override InheritedElement createElement() => DartRuntimePrimitives.ConvertValue<InheritedElement>(new _UbiquitousInheritedElement__basic(this));
}

public class Directionality : _UbiquitousInheritedWidget__basic
{
    public virtual TextDirection textDirection { get; private set; } = default!;

    public Directionality(Key? key = null, TextDirection textDirection = default!, Widget child = default!) : base(key: key, child: child)
    {
        this.textDirection = textDirection;
    }

    public static TextDirection of(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasDirectionality(context));
        Directionality widget = context.dependOnInheritedWidgetOfExactType<Directionality>()!;
        return widget.textDirection;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TextDirection? maybeOf(BuildContext context)
    {
        Directionality? widget = context.dependOnInheritedWidgetOfExactType<Directionality>();
        return widget?.textDirection;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(textDirection, ((Directionality)oldWidget).textDirection));
    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<TextDirection>("textDirection", textDirection));
    }

}

public class Opacity : SingleChildRenderObjectWidget
{
    public virtual double opacity { get; private set; } = default!;
    public virtual bool alwaysIncludeSemantics { get; private set; } = default!;

    public Opacity(Key? key = null, double opacity = default!, bool alwaysIncludeSemantics = false, Widget? child = null) : base(key: key, child: child)
    {
        this.opacity = opacity;
        this.alwaysIncludeSemantics = alwaysIncludeSemantics;
        System.Diagnostics.Debug.Assert((opacity >= 0.0) && (opacity <= 1.0));
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderOpacity(opacity: opacity, alwaysIncludeSemantics: alwaysIncludeSemantics);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderOpacity)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderOpacity>)(() =>
{
    var __cascade = __renderObject;
    __cascade.opacity = opacity;
    __cascade.alwaysIncludeSemantics = alwaysIncludeSemantics;
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("opacity", opacity));
        properties.add(new FlagProperty("alwaysIncludeSemantics", value: alwaysIncludeSemantics, ifTrue: "alwaysIncludeSemantics"));
    }

}

public class ShaderMask : SingleChildRenderObjectWidget
{
    public virtual Func<Rect, Shader> shaderCallback { get; private set; } = default!;
    public virtual BlendMode blendMode { get; private set; } = default!;

    public ShaderMask(Key? key = null, Func<Rect, Shader> shaderCallback = default!, BlendMode blendMode = BlendMode.modulate, Widget? child = null) : base(key: key, child: child)
    {
        this.shaderCallback = shaderCallback;
        this.blendMode = blendMode;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderShaderMask(shaderCallback: shaderCallback, blendMode: blendMode);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderShaderMask)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderShaderMask>)(() =>
{
    var __cascade = __renderObject;
    __cascade.shaderCallback = shaderCallback;
    __cascade.blendMode = blendMode;
    return __cascade;
}))());
    }

}

public class BackdropGroup : InheritedWidget
{
    public virtual BackdropKey backdropKey { get; private set; } = default!;

    public BackdropGroup(Key? key = null, Widget child = default!, BackdropKey? backdropKey = null) : base(key: key, child: child)
    {
        this.backdropKey = backdropKey ?? new BackdropKey();
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (BackdropGroup)oldWidget;
        return !Equals(__oldWidget.backdropKey, backdropKey);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static BackdropGroup? of(BuildContext context)
    {
        return context.dependOnInheritedWidgetOfExactType<BackdropGroup>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class BackdropFilter : SingleChildRenderObjectWidget
{
    public virtual ImageFilter? filter { get; private set; }
    public virtual ImageFilterConfig? filterConfig { get; private set; }
    public virtual BlendMode blendMode { get; private set; } = default!;
    public virtual bool enabled { get; private set; } = default!;
    public virtual BackdropKey? backdropGroupKey { get; private set; }
    internal virtual bool _useSharedKey { get; private set; } = default!;

    public BackdropFilter(Key? key = null, ImageFilter? filter = null, ImageFilterConfig? filterConfig = null, Widget? child = null, BlendMode blendMode = BlendMode.srcOver, bool enabled = true, BackdropKey? backdropGroupKey = null) : base(key: key, child: child)
    {
        this.filter = filter;
        this.filterConfig = filterConfig;
        this.blendMode = blendMode;
        this.enabled = enabled;
        this.backdropGroupKey = backdropGroupKey;
        _useSharedKey = false;
        System.Diagnostics.Debug.Assert((filter is not null) || (filterConfig is not null));
        System.Diagnostics.Debug.Assert((filter is null) || (filterConfig is null));
    }

    public static BackdropFilter CreateGrouped(Key? key = null, ImageFilter? filter = null, ImageFilterConfig? filterConfig = null, Widget? child = null, BlendMode blendMode = BlendMode.srcOver, bool enabled = true)
    {
        var __instance = new BackdropFilter(key, filter, filterConfig, child, blendMode, enabled, default!);
        __instance.filter = filter;
        __instance.filterConfig = filterConfig;
        __instance.blendMode = blendMode;
        __instance.enabled = enabled;
        __instance.backdropGroupKey = null;
        __instance._useSharedKey = true;
        return __instance;
    }

    internal virtual BackdropKey? _getBackdropGroupKey(BuildContext context)
    {
        if (_useSharedKey)
        {
            return BackdropGroup.of(context)?.backdropKey;
        }
        return backdropGroupKey;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual ImageFilterConfig _effectiveFilterConfig
    {
        get
        {
            return filterConfig ?? ImageFilterConfig.Create(filter!);
        }
    }
    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderBackdropFilter(filterConfig: _effectiveFilterConfig, blendMode: blendMode, enabled: enabled, backdropKey: _getBackdropGroupKey(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderBackdropFilter)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderBackdropFilter>)(() =>
{
    var __cascade = __renderObject;
    __cascade.filterConfig = _effectiveFilterConfig;
    __cascade.enabled = enabled;
    __cascade.blendMode = blendMode;
    __cascade.backdropKey = _getBackdropGroupKey(context);
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<ImageFilter>("filter", filter, defaultValue: null));
        properties.add(new DiagnosticsProperty<ImageFilterConfig>("filterConfig", filterConfig, defaultValue: null));
        properties.add(new EnumProperty<BlendMode>("blendMode", blendMode));
        properties.add(new FlagProperty("enabled", value: enabled, ifTrue: "enabled"));
    }

}

public class CustomPaint : SingleChildRenderObjectWidget
{
    public virtual CustomPainter? painter { get; private set; }
    public virtual CustomPainter? foregroundPainter { get; private set; }
    public virtual Size size { get; private set; } = default!;
    public virtual bool isComplex { get; private set; } = default!;
    public virtual bool willChange { get; private set; } = default!;

    public CustomPaint(Key? key = null, CustomPainter? painter = null, CustomPainter? foregroundPainter = null, Size? size = null, bool isComplex = false, bool willChange = false, Widget? child = null) : base(key: key, child: child)
    {
        this.painter = painter;
        this.foregroundPainter = foregroundPainter;
        this.size = size ?? Size.zero;
        this.isComplex = isComplex;
        this.willChange = willChange;
        System.Diagnostics.Debug.Assert((painter is not null) || (foregroundPainter is not null) || !isComplex && !willChange);
    }

    public CustomPaint(Size size, ToggleablePainter painter) : this(
        size: size,
        painter: new ToggleableCustomPainterAdapter(painter))
    {
    }

    public CustomPaint(Key? key = null, object? painter = null, object? foregroundPainter = null, Size? size = null, bool isComplex = false, bool willChange = false, Widget? child = null) : this(
        key: key,
        painter: painter as CustomPainter,
        foregroundPainter: foregroundPainter as CustomPainter,
        size: size,
        isComplex: isComplex,
        willChange: willChange,
        child: child)
    {
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderCustomPaint(painter: painter, foregroundPainter: foregroundPainter, preferredSize: size, isComplex: isComplex, willChange: willChange);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderCustomPaint)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderCustomPaint>)(() =>
{
    var __cascade = __renderObject;
    __cascade.painter = painter;
    __cascade.foregroundPainter = foregroundPainter;
    __cascade.preferredSize = size;
    __cascade.isComplex = isComplex;
    __cascade.willChange = willChange;
    return __cascade;
}))());
    }

    public override void didUnmountRenderObject(RenderObject renderObject)
    {
        var __renderObject = (RenderCustomPaint)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderCustomPaint>)(() =>
{
    var __cascade = __renderObject;
    __cascade.painter = null;
    __cascade.foregroundPainter = null;
    return __cascade;
}))());
    }

}

public class ClipRect : SingleChildRenderObjectWidget
{
    public virtual CustomClipper<Rect>? clipper { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;

    public ClipRect(Key? key = null, CustomClipper<Rect>? clipper = null, Clip clipBehavior = Clip.hardEdge, Widget? child = null) : base(key: key, child: child)
    {
        this.clipper = clipper;
        this.clipBehavior = clipBehavior;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderClipRect(clipper: clipper, clipBehavior: clipBehavior);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderClipRect)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderClipRect>)(() =>
{
    var __cascade = __renderObject;
    __cascade.clipper = clipper;
    __cascade.clipBehavior = clipBehavior;
    return __cascade;
}))());
    }

    public override void didUnmountRenderObject(RenderObject renderObject)
    {
        var __renderObject = (RenderClipRect)renderObject;
        __renderObject.clipper = null;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<CustomClipper<Rect>>("clipper", clipper, defaultValue: null));
    }

}

public class ClipRRect : SingleChildRenderObjectWidget
{
    public virtual BorderRadiusGeometry borderRadius { get; private set; } = default!;
    public virtual CustomClipper<RRect>? clipper { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;

    public ClipRRect(Key? key = null, BorderRadiusGeometry borderRadius = default!, CustomClipper<RRect>? clipper = null, Clip clipBehavior = Clip.antiAlias, Widget? child = null) : base(key: key, child: child)
    {
        BorderRadiusGeometry __borderRadius = borderRadius ?? BorderRadius.zero;
        this.borderRadius = __borderRadius;
        this.clipper = clipper;
        this.clipBehavior = clipBehavior;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderClipRRect(borderRadius: borderRadius, clipper: clipper, clipBehavior: clipBehavior, textDirection: Directionality.maybeOf(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderClipRRect)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderClipRRect>)(() =>
{
    var __cascade = __renderObject;
    __cascade.borderRadius = borderRadius;
    __cascade.clipBehavior = clipBehavior;
    __cascade.clipper = clipper;
    __cascade.textDirection = Directionality.maybeOf(context);
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<BorderRadiusGeometry>("borderRadius", borderRadius, showName: false, defaultValue: null));
        properties.add(new DiagnosticsProperty<CustomClipper<RRect>>("clipper", clipper, defaultValue: null));
    }

}

public class ClipRSuperellipse : SingleChildRenderObjectWidget
{
    public virtual BorderRadiusGeometry borderRadius { get; private set; } = default!;
    public virtual CustomClipper<RSuperellipse>? clipper { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;

    public ClipRSuperellipse(Key? key = null, BorderRadiusGeometry borderRadius = default!, CustomClipper<RSuperellipse>? clipper = null, Clip clipBehavior = Clip.antiAlias, Widget? child = null) : base(key: key, child: child)
    {
        BorderRadiusGeometry __borderRadius = borderRadius ?? BorderRadius.zero;
        this.borderRadius = __borderRadius;
        this.clipper = clipper;
        this.clipBehavior = clipBehavior;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderClipRSuperellipse(borderRadius: borderRadius, clipBehavior: clipBehavior, clipper: clipper, textDirection: Directionality.maybeOf(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderClipRSuperellipse)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderClipRSuperellipse>)(() =>
{
    var __cascade = __renderObject;
    __cascade.borderRadius = borderRadius;
    __cascade.clipBehavior = clipBehavior;
    __cascade.clipper = clipper;
    __cascade.textDirection = Directionality.maybeOf(context);
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<BorderRadiusGeometry>("borderRadius", borderRadius, showName: false, defaultValue: null));
        properties.add(new DiagnosticsProperty<CustomClipper<RSuperellipse>>("clipper", clipper, defaultValue: null));
    }

}

public class ClipOval : SingleChildRenderObjectWidget
{
    public virtual CustomClipper<Rect>? clipper { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;

    public ClipOval(Key? key = null, CustomClipper<Rect>? clipper = null, Clip clipBehavior = Clip.antiAlias, Widget? child = null) : base(key: key, child: child)
    {
        this.clipper = clipper;
        this.clipBehavior = clipBehavior;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderClipOval(clipper: clipper, clipBehavior: clipBehavior);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderClipOval)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderClipOval>)(() =>
{
    var __cascade = __renderObject;
    __cascade.clipper = clipper;
    __cascade.clipBehavior = clipBehavior;
    return __cascade;
}))());
    }

    public override void didUnmountRenderObject(RenderObject renderObject)
    {
        var __renderObject = (RenderClipOval)renderObject;
        __renderObject.clipper = null;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<CustomClipper<Rect>>("clipper", clipper, defaultValue: null));
    }

}

public class ClipPath : SingleChildRenderObjectWidget
{
    public virtual CustomClipper<Path>? clipper { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;

    public ClipPath(Key? key = null, CustomClipper<Path>? clipper = null, Clip clipBehavior = Clip.antiAlias, Widget? child = null) : base(key: key, child: child)
    {
        this.clipper = clipper;
        this.clipBehavior = clipBehavior;
    }

    public static Widget shape(Key? key = null, ShapeBorder shape = default!, Clip clipBehavior = Clip.antiAlias, Widget? child = null)
    {
        return new Builder(key: key, builder: (context) =>
        {
            return new ClipPath(clipper: new ShapeBorderClipper(shape: shape, textDirection: Directionality.maybeOf(context)), clipBehavior: clipBehavior, child: child);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderClipPath(clipper: clipper, clipBehavior: clipBehavior);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderClipPath)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderClipPath>)(() =>
{
    var __cascade = __renderObject;
    __cascade.clipper = clipper;
    __cascade.clipBehavior = clipBehavior;
    return __cascade;
}))());
    }

    public override void didUnmountRenderObject(RenderObject renderObject)
    {
        var __renderObject = (RenderClipPath)renderObject;
        __renderObject.clipper = null;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<CustomClipper<Path>>("clipper", clipper, defaultValue: null));
    }

}

public class PhysicalModel : SingleChildRenderObjectWidget
{
    public virtual BoxShape shape { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual BorderRadius? borderRadius { get; private set; }
    public virtual double elevation { get; private set; } = default!;
    public virtual Color color { get; private set; } = default!;
    public virtual Color shadowColor { get; private set; } = default!;

    public PhysicalModel(Key? key = null, BoxShape shape = BoxShape.rectangle, Clip clipBehavior = Clip.none, BorderRadius? borderRadius = null, double elevation = 0.0, Color color = default!, Color shadowColor = default!, Widget? child = null) : base(key: key, child: child)
    {
        Color __shadowColor = shadowColor ?? new Color(0xFF000000);
        this.shape = shape;
        this.clipBehavior = clipBehavior;
        this.borderRadius = borderRadius;
        this.elevation = elevation;
        this.color = color;
        this.shadowColor = __shadowColor;
        System.Diagnostics.Debug.Assert(elevation >= 0.0);
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderPhysicalModel(shape: shape, clipBehavior: clipBehavior, borderRadius: borderRadius, elevation: elevation, color: color, shadowColor: shadowColor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderPhysicalModel)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderPhysicalModel>)(() =>
{
    var __cascade = __renderObject;
    __cascade.shape = shape;
    __cascade.clipBehavior = clipBehavior;
    __cascade.borderRadius = borderRadius;
    __cascade.elevation = elevation;
    __cascade.color = color;
    __cascade.shadowColor = shadowColor;
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<BoxShape>("shape", shape));
        properties.add(new DiagnosticsProperty<BorderRadius>("borderRadius", borderRadius));
        properties.add(new DoubleProperty("elevation", elevation));
        properties.add(new ColorProperty("color", color));
        properties.add(new ColorProperty("shadowColor", shadowColor));
    }

}

public class PhysicalShape : SingleChildRenderObjectWidget
{
    public virtual CustomClipper<Path> clipper { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual double elevation { get; private set; } = default!;
    public virtual Color color { get; private set; } = default!;
    public virtual Color shadowColor { get; private set; } = default!;

    public PhysicalShape(Key? key = null, CustomClipper<Path> clipper = default!, Clip clipBehavior = Clip.none, double elevation = 0.0, Color color = default!, Color shadowColor = default!, Widget? child = null) : base(key: key, child: child)
    {
        Color __shadowColor = shadowColor ?? new Color(0xFF000000);
        this.clipper = clipper;
        this.clipBehavior = clipBehavior;
        this.elevation = elevation;
        this.color = color;
        this.shadowColor = __shadowColor;
        System.Diagnostics.Debug.Assert(elevation >= 0.0);
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderPhysicalShape(clipper: clipper, clipBehavior: clipBehavior, elevation: elevation, color: color, shadowColor: shadowColor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderPhysicalShape)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderPhysicalShape>)(() =>
{
    var __cascade = __renderObject;
    __cascade.clipper = clipper;
    __cascade.clipBehavior = clipBehavior;
    __cascade.elevation = elevation;
    __cascade.color = color;
    __cascade.shadowColor = shadowColor;
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<CustomClipper<Path>>("clipper", clipper));
        properties.add(new DoubleProperty("elevation", elevation));
        properties.add(new ColorProperty("color", color));
        properties.add(new ColorProperty("shadowColor", shadowColor));
    }

}

public class Transform : SingleChildRenderObjectWidget
{
    public virtual Matrix4 transform { get; private set; } = default!;
    public virtual Offset? origin { get; private set; }
    public virtual AlignmentGeometry? alignment { get; private set; }
    public virtual bool transformHitTests { get; private set; } = default!;
    public virtual FilterQuality? filterQuality { get; private set; }

    public Transform(Key? key = null, Matrix4 transform = default!, Offset? origin = null, AlignmentGeometry? alignment = null, bool transformHitTests = true, FilterQuality? filterQuality = null, Widget? child = null) : base(key: key, child: child)
    {
        this.transform = transform;
        this.origin = origin;
        this.alignment = alignment;
        this.transformHitTests = transformHitTests;
        this.filterQuality = filterQuality;
    }

    public static Transform CreateRotate(Key? key = null, double angle = default!, Offset? origin = null, AlignmentGeometry? alignment = default!, bool transformHitTests = true, FilterQuality? filterQuality = null, Widget? child = null)
    {
        var __instance = new Transform(key: key, child: child);
        AlignmentGeometry? __alignment = alignment ?? Alignment.center;
        __instance.origin = origin;
        __instance.alignment = __alignment;
        __instance.transformHitTests = transformHitTests;
        __instance.filterQuality = filterQuality;
        __instance.transform = _computeRotation(angle);
        return __instance;
    }

    public static Transform CreateTranslate(Key? key = null, Offset offset = default!, bool transformHitTests = true, FilterQuality? filterQuality = null, Widget? child = null)
    {
        var __instance = new Transform(key: key, child: child);
        __instance.transformHitTests = transformHitTests;
        __instance.filterQuality = filterQuality;
        __instance.transform = Matrix4.translationValues(offset.dx, offset.dy, 0.0);
        __instance.origin = null;
        __instance.alignment = null;
        return __instance;
    }

    public static Transform CreateScale(Key? key = null, double? scale = null, double? scaleX = null, double? scaleY = null, Offset? origin = null, AlignmentGeometry? alignment = default!, bool transformHitTests = true, FilterQuality? filterQuality = null, Widget? child = null)
    {
        var __instance = new Transform(key: key, child: child);
        AlignmentGeometry? __alignment = alignment ?? Alignment.center;
        __instance.origin = origin;
        __instance.alignment = __alignment;
        __instance.transformHitTests = transformHitTests;
        __instance.filterQuality = filterQuality;
        __instance.transform = Matrix4.diagonal3Values((scale ?? scaleX) ?? 1.0, (scale ?? scaleY) ?? 1.0, 1.0);
        return __instance;
    }

    public static Transform CreateFlip(Key? key = null, bool flipX = false, bool flipY = false, Offset? origin = null, bool transformHitTests = true, FilterQuality? filterQuality = null, Widget? child = null)
    {
        var __instance = new Transform(key: key, child: child);
        __instance.origin = origin;
        __instance.transformHitTests = transformHitTests;
        __instance.filterQuality = filterQuality;
        __instance.alignment = Alignment.center;
        __instance.transform = Matrix4.diagonal3Values(flipX ? -1.0 : 1.0, flipY ? -1.0 : 1.0, 1.0);
        return __instance;
    }

    internal static Matrix4 _computeRotation(double radians)
    {
        DartRuntimePrimitives.Assert(() => double.IsFinite(radians), () => (object?)$"Cannot compute the rotation matrix for a non-finite angle: {radians}");
        if (radians == 0.0)
        {
            return Matrix4.identity();
        }
        double sinLocal = Dart_mathLibrary.sin(radians);
        if (sinLocal == 1.0)
        {
            return _createZRotation(1.0, 0.0);
        }
        if (sinLocal == -1.0)
        {
            return _createZRotation(-1.0, 0.0);
        }
        double cosLocal = Dart_mathLibrary.cos(radians);
        if (cosLocal == -1.0)
        {
            return _createZRotation(0.0, -1.0);
        }
        return _createZRotation(sinLocal, cosLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static Matrix4 _createZRotation(double sin, double cos)
    {
        var result = Matrix4.zero();
        result.storage[0L] = cos;
        result.storage[1L] = sin;
        result.storage[4L] = -sin;
        result.storage[5L] = cos;
        result.storage[10L] = 1.0;
        result.storage[15L] = 1.0;
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderTransform(transform: transform, origin: origin, alignment: alignment, textDirection: Directionality.maybeOf(context), transformHitTests: transformHitTests, filterQuality: filterQuality);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderTransform)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderTransform>)(() =>
{
    var __cascade = __renderObject;
    __cascade.transform = transform;
    __cascade.origin = origin;
    __cascade.alignment = alignment;
    __cascade.textDirection = Directionality.maybeOf(context);
    __cascade.transformHitTests = transformHitTests;
    __cascade.filterQuality = filterQuality;
    return __cascade;
}))());
    }

}

public class CompositedTransformTarget : SingleChildRenderObjectWidget
{
    public virtual LayerLink link { get; private set; } = default!;

    public CompositedTransformTarget(Key? key = null, LayerLink link = default!, Widget? child = null) : base(key: key, child: child)
    {
        this.link = link;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderLeaderLayer(link: link);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderLeaderLayer)renderObject;
        __renderObject.link = link;
    }

}

public class CompositedTransformFollower : SingleChildRenderObjectWidget
{
    public virtual LayerLink link { get; private set; } = default!;
    public virtual bool showWhenUnlinked { get; private set; } = default!;
    public virtual Alignment targetAnchor { get; private set; } = default!;
    public virtual Alignment followerAnchor { get; private set; } = default!;
    public virtual Offset offset { get; private set; } = default!;

    public CompositedTransformFollower(Key? key = null, LayerLink link = default!, bool showWhenUnlinked = true, Offset offset = default, Alignment targetAnchor = default!, Alignment followerAnchor = default!, Widget? child = null) : base(key: key, child: child)
    {
        Alignment __targetAnchor = targetAnchor ?? Alignment.topLeft;
        Alignment __followerAnchor = followerAnchor ?? Alignment.topLeft;
        this.link = link;
        this.showWhenUnlinked = showWhenUnlinked;
        this.offset = offset;
        this.targetAnchor = __targetAnchor;
        this.followerAnchor = __followerAnchor;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderFollowerLayer(link: link, showWhenUnlinked: showWhenUnlinked, offset: offset, leaderAnchor: targetAnchor, followerAnchor: followerAnchor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderFollowerLayer)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderFollowerLayer>)(() =>
{
    var __cascade = __renderObject;
    __cascade.link = link;
    __cascade.showWhenUnlinked = showWhenUnlinked;
    __cascade.offset = offset;
    __cascade.leaderAnchor = targetAnchor;
    __cascade.followerAnchor = followerAnchor;
    return __cascade;
}))());
    }

}

public class FittedBox : SingleChildRenderObjectWidget
{
    public virtual BoxFit fit { get; private set; } = default!;
    public virtual AlignmentGeometry alignment { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;

    public FittedBox(Key? key = null, BoxFit fit = BoxFit.contain, AlignmentGeometry alignment = default!, Clip clipBehavior = Clip.none, Widget? child = null) : base(key: key, child: child)
    {
        AlignmentGeometry __alignment = alignment ?? Alignment.center;
        this.fit = fit;
        this.alignment = __alignment;
        this.clipBehavior = clipBehavior;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderFittedBox(fit: fit, alignment: alignment, textDirection: Directionality.maybeOf(context), clipBehavior: clipBehavior);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderFittedBox)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderFittedBox>)(() =>
{
    var __cascade = __renderObject;
    __cascade.fit = fit;
    __cascade.alignment = alignment;
    __cascade.textDirection = Directionality.maybeOf(context);
    __cascade.clipBehavior = clipBehavior;
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<BoxFit>("fit", fit));
        properties.add(new DiagnosticsProperty<AlignmentGeometry>("alignment", alignment));
    }

}

public class FractionalTranslation : SingleChildRenderObjectWidget
{
    public virtual Offset translation { get; private set; } = default!;
    public virtual bool transformHitTests { get; private set; } = default!;

    public FractionalTranslation(Key? key = null, Offset translation = default!, bool transformHitTests = true, Widget? child = null) : base(key: key, child: child)
    {
        this.translation = translation;
        this.transformHitTests = transformHitTests;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderFractionalTranslation(translation: translation, transformHitTests: transformHitTests);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderFractionalTranslation)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderFractionalTranslation>)(() =>
{
    var __cascade = __renderObject;
    __cascade.translation = translation;
    __cascade.transformHitTests = transformHitTests;
    return __cascade;
}))());
    }

}

public class RotatedBox : SingleChildRenderObjectWidget
{
    public virtual long quarterTurns { get; private set; } = default!;

    public RotatedBox(Key? key = null, long quarterTurns = default!, Widget? child = null) : base(key: key, child: child)
    {
        this.quarterTurns = quarterTurns;
    }

    public override RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<RenderObject>(new RenderRotatedBox(quarterTurns: quarterTurns));
    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderRotatedBox)renderObject;
        __renderObject.quarterTurns = quarterTurns;
    }

}

public class Padding : SingleChildRenderObjectWidget
{
    public virtual EdgeInsetsGeometry padding { get; private set; } = default!;

    public Padding(Key? key = null, EdgeInsetsGeometry padding = default!, Widget? child = null) : base(key: key, child: child)
    {
        this.padding = padding;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderPadding(padding: padding, textDirection: Directionality.maybeOf(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderPadding)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderPadding>)(() =>
{
    var __cascade = __renderObject;
    __cascade.padding = padding;
    __cascade.textDirection = Directionality.maybeOf(context);
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<EdgeInsetsGeometry>("padding", padding));
    }

}

public class Align : SingleChildRenderObjectWidget
{
    public virtual AlignmentGeometry alignment { get; private set; } = default!;
    public virtual double? widthFactor { get; private set; }
    public virtual double? heightFactor { get; private set; }

    public Align(Key? key = null, AlignmentGeometry alignment = default!, double? widthFactor = null, double? heightFactor = null, Widget? child = null) : base(key: key, child: child)
    {
        AlignmentGeometry __alignment = alignment ?? Alignment.center;
        this.alignment = __alignment;
        this.widthFactor = widthFactor;
        this.heightFactor = heightFactor;
        System.Diagnostics.Debug.Assert((widthFactor is null) || (widthFactor >= 0.0));
        System.Diagnostics.Debug.Assert((heightFactor is null) || (heightFactor >= 0.0));
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderPositionedBox(alignment: alignment, widthFactor: widthFactor, heightFactor: heightFactor, textDirection: Directionality.maybeOf(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderPositionedBox)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderPositionedBox>)(() =>
{
    var __cascade = __renderObject;
    __cascade.alignment = alignment;
    __cascade.widthFactor = widthFactor;
    __cascade.heightFactor = heightFactor;
    __cascade.textDirection = Directionality.maybeOf(context);
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<AlignmentGeometry>("alignment", alignment));
        properties.add(new DoubleProperty("widthFactor", widthFactor, defaultValue: null));
        properties.add(new DoubleProperty("heightFactor", heightFactor, defaultValue: null));
    }

}

public class Center : Align
{
    public Center(Key? key = null, double? widthFactor = null, double? heightFactor = null, Widget? child = null) : base(key: key, widthFactor: widthFactor, heightFactor: heightFactor, child: child)
    {
    }

}

public class CustomSingleChildLayout : SingleChildRenderObjectWidget
{
    public virtual SingleChildLayoutDelegate @delegate { get; private set; } = default!;

    public CustomSingleChildLayout(Key? key = null, SingleChildLayoutDelegate @delegate = default!, Widget? child = null) : base(key: key, child: child)
    {
        this.@delegate = @delegate;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderCustomSingleChildLayoutBox(@delegate: @delegate);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderCustomSingleChildLayoutBox)renderObject;
        __renderObject.@delegate = @delegate;
    }

}

public class LayoutId : ParentDataWidget<MultiChildLayoutParentData>
{
    public virtual object id { get; private set; } = default!;

    public LayoutId(Key? key = null, object id = default!, Widget child = default!) : base(child: child, key: key ?? new ValueKey<object>(id))
    {
        this.id = id;
    }

    public override void applyParentData(RenderObject renderObject)
    {
        DartRuntimePrimitives.Assert(() => renderObject.parentData is MultiChildLayoutParentData);
        var parentDataLocal = ((MultiChildLayoutParentData?)renderObject.parentData!)!;
        if (!Equals(parentDataLocal.id, id))
        {
            parentDataLocal.id = id;
            renderObject.parent?.markNeedsLayout();
        }
    }

    public override Type debugTypicalAncestorWidgetClass => typeof(CustomMultiChildLayout);
    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<object>("id", id));
    }

}

public class CustomMultiChildLayout : MultiChildRenderObjectWidget
{
    public virtual MultiChildLayoutDelegate @delegate { get; private set; } = default!;

    public CustomMultiChildLayout(Key? key = null, MultiChildLayoutDelegate @delegate = default!, List<Widget> children = default!) : base(key: key, children: children ?? new List<Widget>())
    {
        this.@delegate = @delegate;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderCustomMultiChildLayoutBox(@delegate: @delegate);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderCustomMultiChildLayoutBox)renderObject;
        __renderObject.@delegate = @delegate;
    }

}

public class SizedBox : SingleChildRenderObjectWidget
{
    public virtual double? width { get; private set; }
    public virtual double? height { get; private set; }

    public SizedBox(Key? key = null, double? width = null, double? height = null, Widget? child = null) : base(key: key, child: child)
    {
        this.width = width;
        this.height = height;
    }

    public static SizedBox CreateExpand(Key? key = null, Widget? child = null)
    {
        var __instance = new SizedBox(key: key, child: child);
        __instance.width = double.PositiveInfinity;
        __instance.height = double.PositiveInfinity;
        return __instance;
    }

    public static SizedBox CreateShrink(Key? key = null, Widget? child = null)
    {
        var __instance = new SizedBox(key: key, child: child);
        __instance.width = 0.0;
        __instance.height = 0.0;
        return __instance;
    }

    public static SizedBox CreateFromSize(Key? key = null, Widget? child = null, Size? size = null)
    {
        var __instance = new SizedBox(key: key, child: child);
        __instance.width = size?.width;
        __instance.height = size?.height;
        return __instance;
    }

    public static SizedBox CreateSquare(Key? key = null, Widget? child = null, double? dimension = null)
    {
        var __instance = new SizedBox(key: key, child: child);
        __instance.width = dimension;
        __instance.height = dimension;
        return __instance;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderConstrainedBox(additionalConstraints: _additionalConstraints);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual BoxConstraints _additionalConstraints
    {
        get
        {
            return BoxConstraints.CreateTightFor(width: width, height: height);
        }
    }
    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderConstrainedBox)renderObject;
        __renderObject.additionalConstraints = _additionalConstraints;
    }

    public override string toStringShort()
    {
        string @type = (width, height) switch { (var __constant100107, var __constant100124) when Equals(__constant100107, double.PositiveInfinity) && Equals(__constant100124, double.PositiveInfinity) => $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "SizedBox")}.expand", (0.0, 0.0) => $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "SizedBox")}.shrink", _ => objectRuntimeTypeFunctions.objectRuntimeType(this, "SizedBox") };
        return (key is null) ? @type : $"{@type}-{key}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        DiagnosticLevel levelLocal = default!;
        if ((width == double.PositiveInfinity) && (height == double.PositiveInfinity) || (width == 0.0) && (height == 0.0))
        {
            levelLocal = DiagnosticLevel.hidden;
        }
        else
        {
            levelLocal = DiagnosticLevel.info;
        }
        properties.add(new DoubleProperty("width", width, defaultValue: null, level: levelLocal));
        properties.add(new DoubleProperty("height", height, defaultValue: null, level: levelLocal));
    }

}

public class ConstrainedBox : SingleChildRenderObjectWidget
{
    public virtual BoxConstraints constraints { get; private set; } = default!;

    public ConstrainedBox(Key? key = null, BoxConstraints constraints = default!, Widget? child = null) : base(key: key, child: child)
    {
        this.constraints = constraints;
        System.Diagnostics.Debug.Assert(constraints.debugAssertIsValid());
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderConstrainedBox(additionalConstraints: constraints);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderConstrainedBox)renderObject;
        __renderObject.additionalConstraints = constraints;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<BoxConstraints>("constraints", constraints, showName: false));
    }

}

public class ConstraintsTransformBox : SingleChildRenderObjectWidget
{
    internal static DartMap<Func<BoxConstraints, BoxConstraints>, string> _debugKnownTransforms = new DartMap<Func<BoxConstraints, BoxConstraints>, string> { [unmodified] = "unmodified", [unconstrained] = "unconstrained", [widthUnconstrained] = "width constraints removed", [heightUnconstrained] = "height constraints removed", [maxWidthUnconstrained] = "maxWidth constraint removed", [maxHeightUnconstrained] = "maxHeight constraint removed", [maxUnconstrained] = "maxWidth & maxHeight constraints removed" };
    public virtual TextDirection? textDirection { get; private set; }
    public virtual AlignmentGeometry alignment { get; private set; } = default!;
    public virtual Func<BoxConstraints, BoxConstraints> constraintsTransform { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    internal virtual string _debugTransformLabel { get; private set; } = default!;

    public ConstraintsTransformBox(Key? key = null, Widget? child = null, TextDirection? textDirection = null, AlignmentGeometry alignment = default!, Func<BoxConstraints, BoxConstraints> constraintsTransform = default!, Clip clipBehavior = Clip.none, string debugTransformType = "") : base(key: key, child: child)
    {
        AlignmentGeometry __alignment = alignment ?? Alignment.center;
        this.textDirection = textDirection;
        this.alignment = __alignment;
        this.constraintsTransform = constraintsTransform;
        this.clipBehavior = clipBehavior;
        _debugTransformLabel = debugTransformType;
    }

    public static BoxConstraints unmodified(BoxConstraints constraints) => constraints;
    public static BoxConstraints unconstrained(BoxConstraints constraints) => new BoxConstraints();
    public static BoxConstraints widthUnconstrained(BoxConstraints constraints) => constraints.heightConstraints();
    public static BoxConstraints heightUnconstrained(BoxConstraints constraints) => constraints.widthConstraints();
    public static BoxConstraints maxHeightUnconstrained(BoxConstraints constraints) => constraints.copyWith(maxHeight: double.PositiveInfinity);
    public static BoxConstraints maxWidthUnconstrained(BoxConstraints constraints) => constraints.copyWith(maxWidth: double.PositiveInfinity);
    public static BoxConstraints maxUnconstrained(BoxConstraints constraints) => constraints.copyWith(maxWidth: double.PositiveInfinity, maxHeight: double.PositiveInfinity);
    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderConstraintsTransformBox(textDirection: textDirection ?? Directionality.maybeOf(context), alignment: alignment, constraintsTransform: constraintsTransform, clipBehavior: clipBehavior);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderConstraintsTransformBox)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderConstraintsTransformBox>)(() =>
{
    var __cascade = __renderObject;
    __cascade.textDirection = textDirection ?? Directionality.maybeOf(context);
    __cascade.constraintsTransform = constraintsTransform;
    __cascade.alignment = alignment;
    __cascade.clipBehavior = clipBehavior;
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<AlignmentGeometry>("alignment", alignment));
        properties.add(new EnumProperty<TextDirection>("textDirection", textDirection, defaultValue: null));
        string? debugTransformLabel = (_debugTransformLabel.Length != 0) ? _debugTransformLabel : _debugKnownTransforms.GetValueOrDefault(constraintsTransform);
        if (debugTransformLabel is not null)
        {
            properties.add(new DiagnosticsProperty<string>("constraints transform", debugTransformLabel));
        }
    }

}

public class UnconstrainedBox : StatelessWidget
{
    public virtual TextDirection? textDirection { get; private set; }
    public virtual AlignmentGeometry alignment { get; private set; } = default!;
    public virtual Axis? constrainedAxis { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual Widget? child { get; private set; }

    public UnconstrainedBox(Key? key = null, Widget? child = null, TextDirection? textDirection = null, AlignmentGeometry alignment = default!, Axis? constrainedAxis = null, Clip clipBehavior = Clip.none) : base(key: key)
    {
        AlignmentGeometry __alignment = alignment ?? Alignment.center;
        this.child = child;
        this.textDirection = textDirection;
        this.alignment = __alignment;
        this.constrainedAxis = constrainedAxis;
        this.clipBehavior = clipBehavior;
    }

    internal virtual Func<BoxConstraints, BoxConstraints> _axisToTransform(Axis? constrainedAxis)
    {
        return constrainedAxis switch { Axis.horizontal => ConstraintsTransformBox.heightUnconstrained, Axis.vertical => ConstraintsTransformBox.widthUnconstrained, null => ConstraintsTransformBox.unconstrained, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        return new ConstraintsTransformBox(textDirection: textDirection, alignment: alignment, clipBehavior: clipBehavior, constraintsTransform: _axisToTransform(constrainedAxis), child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<AlignmentGeometry>("alignment", alignment));
        properties.add(new EnumProperty<Axis>("constrainedAxis", constrainedAxis, defaultValue: null));
        properties.add(new EnumProperty<TextDirection>("textDirection", textDirection, defaultValue: null));
    }

}

public class FractionallySizedBox : SingleChildRenderObjectWidget
{
    public virtual double? widthFactor { get; private set; }
    public virtual double? heightFactor { get; private set; }
    public virtual AlignmentGeometry alignment { get; private set; } = default!;

    public FractionallySizedBox(Key? key = null, AlignmentGeometry alignment = default!, double? widthFactor = null, double? heightFactor = null, Widget? child = null) : base(key: key, child: child)
    {
        AlignmentGeometry __alignment = alignment ?? Alignment.center;
        this.alignment = __alignment;
        this.widthFactor = widthFactor;
        this.heightFactor = heightFactor;
        System.Diagnostics.Debug.Assert((widthFactor is null) || (widthFactor >= 0.0));
        System.Diagnostics.Debug.Assert((heightFactor is null) || (heightFactor >= 0.0));
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderFractionallySizedOverflowBox(alignment: alignment, widthFactor: widthFactor, heightFactor: heightFactor, textDirection: Directionality.maybeOf(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderFractionallySizedOverflowBox)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderFractionallySizedOverflowBox>)(() =>
{
    var __cascade = __renderObject;
    __cascade.alignment = alignment;
    __cascade.widthFactor = widthFactor;
    __cascade.heightFactor = heightFactor;
    __cascade.textDirection = Directionality.maybeOf(context);
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<AlignmentGeometry>("alignment", alignment));
        properties.add(new DoubleProperty("widthFactor", widthFactor, defaultValue: null));
        properties.add(new DoubleProperty("heightFactor", heightFactor, defaultValue: null));
    }

}

public class LimitedBox : SingleChildRenderObjectWidget
{
    public virtual double maxWidth { get; private set; } = default!;
    public virtual double maxHeight { get; private set; } = default!;

    public LimitedBox(Key? key = null, double maxWidth = double.PositiveInfinity, double maxHeight = double.PositiveInfinity, Widget? child = null) : base(key: key, child: child)
    {
        this.maxWidth = maxWidth;
        this.maxHeight = maxHeight;
        System.Diagnostics.Debug.Assert(maxWidth >= 0.0);
        System.Diagnostics.Debug.Assert(maxHeight >= 0.0);
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderLimitedBox(maxWidth: maxWidth, maxHeight: maxHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderLimitedBox)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderLimitedBox>)(() =>
{
    var __cascade = __renderObject;
    __cascade.maxWidth = maxWidth;
    __cascade.maxHeight = maxHeight;
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("maxWidth", maxWidth, defaultValue: double.PositiveInfinity));
        properties.add(new DoubleProperty("maxHeight", maxHeight, defaultValue: double.PositiveInfinity));
    }

}

public class OverflowBox : SingleChildRenderObjectWidget
{
    public virtual AlignmentGeometry alignment { get; private set; } = default!;
    public virtual double? minWidth { get; private set; }
    public virtual double? maxWidth { get; private set; }
    public virtual double? minHeight { get; private set; }
    public virtual double? maxHeight { get; private set; }
    public virtual OverflowBoxFit fit { get; private set; } = default!;

    public OverflowBox(Key? key = null, AlignmentGeometry alignment = default!, double? minWidth = null, double? maxWidth = null, double? minHeight = null, double? maxHeight = null, OverflowBoxFit fit = OverflowBoxFit.max, Widget? child = null) : base(key: key, child: child)
    {
        AlignmentGeometry __alignment = alignment ?? Alignment.center;
        this.alignment = __alignment;
        this.minWidth = minWidth;
        this.maxWidth = maxWidth;
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
        this.fit = fit;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderConstrainedOverflowBox(alignment: alignment, minWidth: minWidth, maxWidth: maxWidth, minHeight: minHeight, maxHeight: maxHeight, fit: fit, textDirection: Directionality.maybeOf(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderConstrainedOverflowBox)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderConstrainedOverflowBox>)(() =>
{
    var __cascade = __renderObject;
    __cascade.alignment = alignment;
    __cascade.minWidth = minWidth;
    __cascade.maxWidth = maxWidth;
    __cascade.minHeight = minHeight;
    __cascade.maxHeight = maxHeight;
    __cascade.fit = fit;
    __cascade.textDirection = Directionality.maybeOf(context);
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<AlignmentGeometry>("alignment", alignment));
        properties.add(new DoubleProperty("minWidth", minWidth, defaultValue: null));
        properties.add(new DoubleProperty("maxWidth", maxWidth, defaultValue: null));
        properties.add(new DoubleProperty("minHeight", minHeight, defaultValue: null));
        properties.add(new DoubleProperty("maxHeight", maxHeight, defaultValue: null));
        properties.add(new EnumProperty<OverflowBoxFit>("fit", fit));
    }

}

public class SizedOverflowBox : SingleChildRenderObjectWidget
{
    public virtual AlignmentGeometry alignment { get; private set; } = default!;
    public virtual Size size { get; private set; } = default!;

    public SizedOverflowBox(Key? key = null, Size size = default!, AlignmentGeometry alignment = default!, Widget? child = null) : base(key: key, child: child)
    {
        AlignmentGeometry __alignment = alignment ?? Alignment.center;
        this.size = size;
        this.alignment = __alignment;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderSizedOverflowBox(alignment: alignment, requestedSize: size, textDirection: Directionality.of(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderSizedOverflowBox)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderSizedOverflowBox>)(() =>
{
    var __cascade = __renderObject;
    __cascade.alignment = alignment;
    __cascade.requestedSize = size;
    __cascade.textDirection = Directionality.of(context);
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<AlignmentGeometry>("alignment", alignment));
        properties.add(new DiagnosticsProperty<Size>("size", size, defaultValue: null));
    }

}

public class Offstage : SingleChildRenderObjectWidget
{
    public virtual bool offstage { get; private set; } = default!;

    public Offstage(Key? key = null, bool offstage = true, Widget? child = null) : base(key: key, child: child)
    {
        this.offstage = offstage;
    }

    public override RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<RenderObject>(new RenderOffstage(offstage: offstage));
    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderOffstage)renderObject;
        __renderObject.offstage = offstage;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<bool>("offstage", offstage));
    }

    public override SingleChildRenderObjectElement createElement() => DartRuntimePrimitives.ConvertValue<SingleChildRenderObjectElement>(new _OffstageElement__basic(this));
}

internal class _OffstageElement__basic : SingleChildRenderObjectElement
{
    internal _OffstageElement__basic(Offstage widget) : base(widget)
    {
    }

    public override void debugVisitOnstageChildren(Action<Element> visitor)
    {
        if (!((Offstage?)widget)!.offstage)
        {
            base.debugVisitOnstageChildren(visitor);
        }
    }

}

public class AspectRatio : SingleChildRenderObjectWidget
{
    public virtual double aspectRatio { get; private set; } = default!;

    public AspectRatio(Key? key = null, double aspectRatio = default!, Widget? child = null) : base(key: key, child: child)
    {
        this.aspectRatio = aspectRatio;
        System.Diagnostics.Debug.Assert(aspectRatio > 0.0);
    }

    public override RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<RenderObject>(new RenderAspectRatio(aspectRatio: aspectRatio));
    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderAspectRatio)renderObject;
        __renderObject.aspectRatio = aspectRatio;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("aspectRatio", aspectRatio));
    }

}

public class IntrinsicWidth : SingleChildRenderObjectWidget
{
    public virtual double? stepWidth { get; private set; }
    public virtual double? stepHeight { get; private set; }

    public IntrinsicWidth(Key? key = null, double? stepWidth = null, double? stepHeight = null, Widget? child = null) : base(key: key, child: child)
    {
        this.stepWidth = stepWidth;
        this.stepHeight = stepHeight;
        System.Diagnostics.Debug.Assert((stepWidth is null) || (stepWidth >= 0.0));
        System.Diagnostics.Debug.Assert((stepHeight is null) || (stepHeight >= 0.0));
    }

    internal virtual double? _stepWidth => (stepWidth == 0.0) ? null : stepWidth;
    internal virtual double? _stepHeight => (stepHeight == 0.0) ? null : stepHeight;
    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderIntrinsicWidth(stepWidth: _stepWidth, stepHeight: _stepHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderIntrinsicWidth)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderIntrinsicWidth>)(() =>
{
    var __cascade = __renderObject;
    __cascade.stepWidth = _stepWidth;
    __cascade.stepHeight = _stepHeight;
    return __cascade;
}))());
    }

}

public class IntrinsicHeight : SingleChildRenderObjectWidget
{
    public IntrinsicHeight(Key? key = null, Widget? child = null) : base(key: key, child: child)
    {
    }

    public override RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<RenderObject>(new RenderIntrinsicHeight());
}

public class Baseline : SingleChildRenderObjectWidget
{
    public virtual double baseline { get; private set; } = default!;
    public virtual TextBaseline baselineType { get; private set; } = default!;

    public Baseline(Key? key = null, double baseline = default!, TextBaseline baselineType = default!, Widget? child = null) : base(key: key, child: child)
    {
        this.baseline = baseline;
        this.baselineType = baselineType;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderBaseline(baseline: baseline, baselineType: baselineType);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderBaseline)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderBaseline>)(() =>
{
    var __cascade = __renderObject;
    __cascade.baseline = baseline;
    __cascade.baselineType = baselineType;
    return __cascade;
}))());
    }

}

public class IgnoreBaseline : SingleChildRenderObjectWidget
{
    public IgnoreBaseline(Key? key = null, Widget? child = null) : base(key: key, child: child)
    {
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderIgnoreBaseline();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SliverToBoxAdapter : SingleChildRenderObjectWidget
{
    public SliverToBoxAdapter(Key? key = null, Widget? child = null) : base(key: key, child: child)
    {
    }

    public override RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<RenderObject>(new RenderSliverToBoxAdapter());
}

public class SliverPadding : SingleChildRenderObjectWidget
{
    public virtual EdgeInsetsGeometry padding { get; private set; } = default!;

    public SliverPadding(Key? key = null, EdgeInsetsGeometry padding = default!, Widget? sliver = null) : base(key: key, child: sliver)
    {
        this.padding = padding;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderSliverPadding(padding: padding, textDirection: Directionality.of(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderSliverPadding)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderSliverPadding>)(() =>
{
    var __cascade = __renderObject;
    __cascade.padding = padding;
    __cascade.textDirection = Directionality.of(context);
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<EdgeInsetsGeometry>("padding", padding));
    }

}

public abstract class _SemanticsBase__basic : SingleChildRenderObjectWidget
{
    public virtual SemanticsProperties properties { get; private set; } = default!;
    public virtual bool container { get; private set; } = default!;
    public virtual bool explicitChildNodes { get; private set; } = default!;
    public virtual Locale? localeForSubtree { get; private set; }
    public virtual bool excludeSemantics { get; private set; } = default!;
    public virtual bool blockUserActions { get; private set; } = default!;

    internal _SemanticsBase__basic(Key? key = null, Widget? child = null, bool container = default!, bool explicitChildNodes = default!, bool excludeSemantics = default!, bool blockUserActions = default!, bool? enabled = default!, bool? @checked = default!, bool? mixed = default!, bool? selected = default!, bool? toggled = default!, bool? button = default!, bool? slider = default!, bool? keyboardKey = default!, bool? link = default!, DartUri? linkUrl = default!, bool? header = default!, long? headingLevel = default!, bool? textField = default!, bool? readOnly = default!, bool? focusable = default!, bool? focused = default!, AccessibilityFocusBlockType? accessibilityFocusBlockType = default!, bool? inMutuallyExclusiveGroup = default!, bool? obscured = default!, bool? multiline = default!, bool? scopesRoute = default!, bool? namesRoute = default!, bool? hidden = default!, bool? image = default!, bool? liveRegion = default!, bool? expanded = default!, bool? isRequired = default!, long? maxValueLength = default!, long? currentValueLength = default!, string? identifier = default!, object? traversalParentIdentifier = default!, object? traversalChildIdentifier = default!, string? label = default!, AttributedString? attributedLabel = default!, string? value = default!, AttributedString? attributedValue = default!, string? increasedValue = default!, AttributedString? attributedIncreasedValue = default!, string? decreasedValue = default!, AttributedString? attributedDecreasedValue = default!, string? hint = default!, AttributedString? attributedHint = default!, string? tooltip = default!, string? onTapHint = default!, string? onLongPressHint = default!, TextDirection? textDirection = default!, SemanticsSortKey? sortKey = default!, SemanticsTag? tagForChildren = default!, Action? onTap = default!, Action? onLongPress = default!, Action? onScrollLeft = default!, Action? onScrollRight = default!, Action? onScrollUp = default!, Action? onScrollDown = default!, Action? onIncrease = default!, Action? onDecrease = default!, Action? onCopy = default!, Action? onCut = default!, Action? onPaste = default!, Action? onDismiss = default!, Action<bool>? onMoveCursorForwardByCharacter = default!, Action<bool>? onMoveCursorBackwardByCharacter = default!, Action<TextSelection>? onSetSelection = default!, Action<string>? onSetText = default!, Action? onDidGainAccessibilityFocus = default!, Action? onDidLoseAccessibilityFocus = default!, Action? onFocus = default!, Action? onExpand = default!, Action? onCollapse = default!, DartMap<CustomSemanticsAction, Action>? customSemanticsActions = default!, SemanticsRole? role = default!, HashSet<string>? controlsNodes = default!, SemanticsValidationResult validationResult = default!, SemanticsHitTestBehavior? hitTestBehavior = default!, SemanticsInputType? inputType = default!, Locale? localeForSubtree = default!, string? minValue = default!, string? maxValue = default!) : this(key: key, child: child, container: container, explicitChildNodes: explicitChildNodes, excludeSemantics: excludeSemantics, blockUserActions: blockUserActions, localeForSubtree: localeForSubtree, properties: new SemanticsProperties(enabled: enabled, @checked: @checked, mixed: mixed, expanded: expanded, toggled: toggled, selected: selected, button: button, slider: slider, keyboardKey: keyboardKey, link: link, linkUrl: linkUrl, header: header, headingLevel: headingLevel, textField: textField, readOnly: readOnly, focusable: focusable, focused: focused, accessibilityFocusBlockType: accessibilityFocusBlockType, inMutuallyExclusiveGroup: inMutuallyExclusiveGroup, obscured: obscured, multiline: multiline, scopesRoute: scopesRoute, namesRoute: namesRoute, hidden: hidden, image: image, liveRegion: liveRegion, isRequired: isRequired, maxValueLength: maxValueLength, currentValueLength: currentValueLength, identifier: identifier, traversalParentIdentifier: traversalParentIdentifier, traversalChildIdentifier: traversalChildIdentifier, label: label, attributedLabel: attributedLabel, value: value, attributedValue: attributedValue, increasedValue: increasedValue, attributedIncreasedValue: attributedIncreasedValue, decreasedValue: decreasedValue, attributedDecreasedValue: attributedDecreasedValue, hint: hint, attributedHint: attributedHint, tooltip: tooltip, textDirection: textDirection, sortKey: sortKey, tagForChildren: tagForChildren, onTap: onTap, onLongPress: onLongPress, onScrollLeft: onScrollLeft, onScrollRight: onScrollRight, onScrollUp: onScrollUp, onScrollDown: onScrollDown, onIncrease: onIncrease, onDecrease: onDecrease, onCopy: onCopy, onCut: onCut, onPaste: onPaste, onMoveCursorForwardByCharacter: onMoveCursorForwardByCharacter, onMoveCursorBackwardByCharacter: onMoveCursorBackwardByCharacter, onDidGainAccessibilityFocus: onDidGainAccessibilityFocus, onDidLoseAccessibilityFocus: onDidLoseAccessibilityFocus, onFocus: onFocus, onDismiss: onDismiss, onSetSelection: onSetSelection, onSetText: onSetText, onExpand: onExpand, onCollapse: onCollapse, customSemanticsActions: customSemanticsActions, hintOverrides: ((onTapHint is not null) || (onLongPressHint is not null)) ? new SemanticsHintOverrides(onTapHint: onTapHint, onLongPressHint: onLongPressHint) : null, role: role, controlsNodes: controlsNodes, validationResult: validationResult, hitTestBehavior: hitTestBehavior, inputType: inputType, minValue: minValue, maxValue: maxValue))
    {
    }

    internal _SemanticsBase__basic(Key? key = null, Widget? child = null, bool container = default!, bool explicitChildNodes = default!, bool excludeSemantics = default!, bool blockUserActions = default!, Locale? localeForSubtree = default!, SemanticsProperties properties = default!) : base(key: key, child: child)
    {
        this.container = container;
        this.explicitChildNodes = explicitChildNodes;
        this.excludeSemantics = excludeSemantics;
        this.blockUserActions = blockUserActions;
        this.localeForSubtree = localeForSubtree;
        this.properties = properties;
    }

    internal virtual TextDirection? _getTextDirection(BuildContext context)
    {
        if (properties.textDirection is not null)
        {
            return properties.textDirection;
        }
        bool containsText = (properties.label is not null) || (properties.attributedLabel is not null) || (properties.value is not null) || (properties.attributedValue is not null) || (properties.increasedValue is not null) || (properties.attributedIncreasedValue is not null) || (properties.decreasedValue is not null) || (properties.attributedDecreasedValue is not null) || (properties.hint is not null) || (properties.attributedHint is not null) || (properties.tooltip is not null);
        if (!containsText)
        {
            return null;
        }
        return Directionality.maybeOf(context);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SliverSemantics : _SemanticsBase__basic
{
    private SliverSemantics(Key? key, Widget? child, bool container, bool explicitChildNodes, bool excludeSemantics, bool blockUserActions, Locale? localeForSubtree, SemanticsProperties properties) : base(key: key, child: child, container: container, explicitChildNodes: explicitChildNodes, excludeSemantics: excludeSemantics, blockUserActions: blockUserActions, localeForSubtree: localeForSubtree, properties: properties)
    {
    }

    public SliverSemantics(Key? key = null, Widget sliver = default!, bool container = false, bool explicitChildNodes = false, bool excludeSemantics = false, bool blockUserActions = false, bool? enabled = null, bool? @checked = null, bool? mixed = null, bool? selected = null, bool? toggled = null, bool? button = null, bool? slider = null, bool? keyboardKey = null, bool? link = null, DartUri? linkUrl = null, bool? header = null, long? headingLevel = null, bool? textField = null, bool? readOnly = null, bool? focusable = null, bool? focused = null, AccessibilityFocusBlockType? accessibilityFocusBlockType = null, bool? inMutuallyExclusiveGroup = null, bool? obscured = null, bool? multiline = null, bool? scopesRoute = null, bool? namesRoute = null, bool? hidden = null, bool? image = null, bool? liveRegion = null, bool? expanded = null, bool? isRequired = null, long? maxValueLength = null, long? currentValueLength = null, string? identifier = null, object? traversalParentIdentifier = null, object? traversalChildIdentifier = null, string? label = null, AttributedString? attributedLabel = null, string? value = null, AttributedString? attributedValue = null, string? increasedValue = null, AttributedString? attributedIncreasedValue = null, string? decreasedValue = null, AttributedString? attributedDecreasedValue = null, string? hint = null, AttributedString? attributedHint = null, string? tooltip = null, string? onTapHint = null, string? onLongPressHint = null, TextDirection? textDirection = null, SemanticsSortKey? sortKey = null, SemanticsTag? tagForChildren = null, Action? onTap = null, Action? onLongPress = null, Action? onScrollLeft = null, Action? onScrollRight = null, Action? onScrollUp = null, Action? onScrollDown = null, Action? onIncrease = null, Action? onDecrease = null, Action? onCopy = null, Action? onCut = null, Action? onPaste = null, Action? onDismiss = null, Action<bool>? onMoveCursorForwardByCharacter = null, Action<bool>? onMoveCursorBackwardByCharacter = null, Action<TextSelection>? onSetSelection = null, Action<string>? onSetText = null, Action? onDidGainAccessibilityFocus = null, Action? onDidLoseAccessibilityFocus = null, Action? onFocus = null, Action? onExpand = null, Action? onCollapse = null, DartMap<CustomSemanticsAction, Action>? customSemanticsActions = null, SemanticsRole? role = null, HashSet<string>? controlsNodes = null, SemanticsValidationResult validationResult = SemanticsValidationResult.none, SemanticsHitTestBehavior? hitTestBehavior = null, SemanticsInputType? inputType = null, Locale? localeForSubtree = null, string? minValue = null, string? maxValue = null) : base(key: key, container: container, explicitChildNodes: explicitChildNodes, excludeSemantics: excludeSemantics, blockUserActions: blockUserActions, enabled: DartRuntimePrimitives.RequireValue(enabled), @checked: DartRuntimePrimitives.RequireValue(@checked), mixed: DartRuntimePrimitives.RequireValue(mixed), selected: DartRuntimePrimitives.RequireValue(selected), toggled: DartRuntimePrimitives.RequireValue(toggled), button: DartRuntimePrimitives.RequireValue(button), slider: DartRuntimePrimitives.RequireValue(slider), keyboardKey: DartRuntimePrimitives.RequireValue(keyboardKey), link: DartRuntimePrimitives.RequireValue(link), linkUrl: linkUrl, header: DartRuntimePrimitives.RequireValue(header), headingLevel: DartRuntimePrimitives.RequireValue(headingLevel), textField: DartRuntimePrimitives.RequireValue(textField), readOnly: DartRuntimePrimitives.RequireValue(readOnly), focusable: DartRuntimePrimitives.RequireValue(focusable), focused: DartRuntimePrimitives.RequireValue(focused), accessibilityFocusBlockType: DartRuntimePrimitives.RequireValue(accessibilityFocusBlockType), inMutuallyExclusiveGroup: DartRuntimePrimitives.RequireValue(inMutuallyExclusiveGroup), obscured: DartRuntimePrimitives.RequireValue(obscured), multiline: DartRuntimePrimitives.RequireValue(multiline), scopesRoute: DartRuntimePrimitives.RequireValue(scopesRoute), namesRoute: DartRuntimePrimitives.RequireValue(namesRoute), hidden: DartRuntimePrimitives.RequireValue(hidden), image: DartRuntimePrimitives.RequireValue(image), liveRegion: DartRuntimePrimitives.RequireValue(liveRegion), expanded: DartRuntimePrimitives.RequireValue(expanded), isRequired: DartRuntimePrimitives.RequireValue(isRequired), maxValueLength: DartRuntimePrimitives.RequireValue(maxValueLength), currentValueLength: DartRuntimePrimitives.RequireValue(currentValueLength), identifier: identifier, traversalParentIdentifier: traversalParentIdentifier, traversalChildIdentifier: traversalChildIdentifier, label: label, attributedLabel: attributedLabel, value: value, attributedValue: attributedValue, increasedValue: increasedValue, attributedIncreasedValue: attributedIncreasedValue, decreasedValue: decreasedValue, attributedDecreasedValue: attributedDecreasedValue, hint: hint, attributedHint: attributedHint, tooltip: tooltip, onTapHint: onTapHint, onLongPressHint: onLongPressHint, textDirection: DartRuntimePrimitives.RequireValue(textDirection), sortKey: sortKey, tagForChildren: tagForChildren, onTap: onTap, onLongPress: onLongPress, onScrollLeft: onScrollLeft, onScrollRight: onScrollRight, onScrollUp: onScrollUp, onScrollDown: onScrollDown, onIncrease: onIncrease, onDecrease: onDecrease, onCopy: onCopy, onCut: onCut, onPaste: onPaste, onDismiss: onDismiss, onMoveCursorForwardByCharacter: onMoveCursorForwardByCharacter, onMoveCursorBackwardByCharacter: onMoveCursorBackwardByCharacter, onSetSelection: onSetSelection, onSetText: onSetText, onDidGainAccessibilityFocus: onDidGainAccessibilityFocus, onDidLoseAccessibilityFocus: onDidLoseAccessibilityFocus, onFocus: onFocus, onExpand: onExpand, onCollapse: onCollapse, customSemanticsActions: customSemanticsActions, role: DartRuntimePrimitives.RequireValue(role), controlsNodes: controlsNodes, validationResult: validationResult, hitTestBehavior: DartRuntimePrimitives.RequireValue(hitTestBehavior), inputType: DartRuntimePrimitives.RequireValue(inputType), localeForSubtree: DartRuntimePrimitives.RequireValue(localeForSubtree), minValue: minValue, maxValue: maxValue, child: sliver)
    {
    }

    public static SliverSemantics CreateFromProperties(Key? key = null, Widget? child = null, bool container = false, bool explicitChildNodes = false, bool excludeSemantics = false, bool blockUserActions = false, Locale? localeForSubtree = null, SemanticsProperties properties = default!)
    {
        return new SliverSemantics(key, child, container, explicitChildNodes, excludeSemantics, blockUserActions, localeForSubtree, properties);
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderSliverSemanticsAnnotations(container: container, explicitChildNodes: explicitChildNodes, excludeSemantics: excludeSemantics, blockUserActions: blockUserActions, properties: properties, localeForSubtree: localeForSubtree, textDirection: _getTextDirection(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderSliverSemanticsAnnotations)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderSliverSemanticsAnnotations>)(() =>
{
    var __cascade = __renderObject;
    __cascade.container = container;
    __cascade.explicitChildNodes = explicitChildNodes;
    __cascade.excludeSemantics = excludeSemantics;
    __cascade.blockUserActions = blockUserActions;
    __cascade.properties = properties;
    __cascade.textDirection = _getTextDirection(context);
    __cascade.localeForSubtree = localeForSubtree;
    return __cascade;
}))());
    }

}

public static partial class BasicLibrary
{
    public static AxisDirection getAxisDirectionFromAxisReverseAndDirectionality(BuildContext context, Axis axis, bool reverse)
    {
        switch (axis)
        {
            case Axis.horizontal:
                {
                    DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasDirectionality(context));
                    TextDirection textDirection = Directionality.of(context);
                    AxisDirection axisDirection = Basic_typesLibrary.textDirectionToAxisDirection(textDirection);
                    return reverse ? Basic_typesLibrary.flipAxisDirection(axisDirection) : axisDirection;
                }
            case Axis.vertical:
                {
                    return reverse ? AxisDirection.up : AxisDirection.down;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class ListBody : MultiChildRenderObjectWidget
{
    public virtual Axis mainAxis { get; private set; } = default!;
    public virtual bool reverse { get; private set; } = default!;

    public ListBody(Key? key = null, Axis mainAxis = Axis.vertical, bool reverse = false, List<Widget> children = default!) : base(key: key, children: children ?? new List<Widget>())
    {
        this.mainAxis = mainAxis;
        this.reverse = reverse;
    }

    internal virtual AxisDirection _getDirection(BuildContext context)
    {
        return BasicLibrary.getAxisDirectionFromAxisReverseAndDirectionality(context, mainAxis, reverse);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderListBody(axisDirection: _getDirection(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderListBody)renderObject;
        __renderObject.axisDirection = _getDirection(context);
    }

}

public class Stack : MultiChildRenderObjectWidget
{
    public virtual AlignmentGeometry alignment { get; private set; } = default!;
    public virtual TextDirection? textDirection { get; private set; }
    public virtual StackFit fit { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;

    public Stack(Key? key = null, AlignmentGeometry alignment = default!, TextDirection? textDirection = null, StackFit fit = StackFit.loose, Clip clipBehavior = Clip.hardEdge, List<Widget> children = default!) : base(key: key, children: children ?? new List<Widget>())
    {
        AlignmentGeometry __alignment = alignment ?? AlignmentDirectional.topStart;
        this.alignment = __alignment;
        this.textDirection = textDirection;
        this.fit = fit;
        this.clipBehavior = clipBehavior;
    }

    public static Stack Create(Key? key = null, AlignmentGeometry alignment = default!, TextDirection? textDirection = null, StackFit fit = StackFit.loose, Clip clipBehavior = Clip.hardEdge, List<Widget> children = default!) =>
        new(key, alignment, textDirection, fit, clipBehavior, children);

    internal virtual bool _debugCheckHasDirectionality(BuildContext context)
    {
        if ((alignment is AlignmentDirectional) && (textDirection is null))
        {
            AlignmentDirectional alignment__as178201 = (AlignmentDirectional)alignment;
            DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasDirectionality(context, why: "to resolve the 'alignment' argument", hint: Equals(alignment, AlignmentDirectional.topStart) ? "The default value for 'alignment' is AlignmentDirectional.topStart, which requires a text direction." : null, alternative: $"Instead of providing a Directionality widget, another solution would be passing a non-directional 'alignment__as178201', or an explicit 'textDirection', to the {GetType()}."));
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => _debugCheckHasDirectionality(context));
        return new RenderStack(alignment: alignment, textDirection: textDirection ?? Directionality.maybeOf(context), fit: fit, clipBehavior: clipBehavior);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderStack)renderObject;
        DartRuntimePrimitives.Assert(() => _debugCheckHasDirectionality(context));
        DartRuntimePrimitives.Ignore(((Func<RenderStack>)(() =>
{
    var __cascade = __renderObject;
    __cascade.alignment = alignment;
    __cascade.textDirection = textDirection ?? Directionality.maybeOf(context);
    __cascade.fit = fit;
    __cascade.clipBehavior = clipBehavior;
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<AlignmentGeometry>("alignment", alignment));
        properties.add(new EnumProperty<TextDirection>("textDirection", textDirection, defaultValue: null));
        properties.add(new EnumProperty<StackFit>("fit", fit));
        properties.add(new EnumProperty<Clip>("clipBehavior", clipBehavior, defaultValue: Clip.hardEdge));
    }

}

public class Positioned : ParentDataWidget<StackParentData>
{
    public virtual double? left { get; private set; }
    public virtual double? top { get; private set; }
    public virtual double? right { get; private set; }
    public virtual double? bottom { get; private set; }
    public virtual double? width { get; private set; }
    public virtual double? height { get; private set; }

    public Positioned(Key? key = null, double? left = null, double? top = null, double? right = null, double? bottom = null, double? width = null, double? height = null, Widget child = default!) : base(key: key, child: child)
    {
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
        this.width = width;
        this.height = height;
        System.Diagnostics.Debug.Assert((left is null) || (right is null) || (width is null));
        System.Diagnostics.Debug.Assert((top is null) || (bottom is null) || (height is null));
    }

    public static Positioned CreateFromRect(Key? key = null, Rect rect = default!, Widget child = default!)
    {
        var __instance = new Positioned(key, default!, default!, default!, default!, default!, default!, child);
        __instance.left = rect.left;
        __instance.top = rect.top;
        __instance.width = rect.width;
        __instance.height = rect.height;
        __instance.right = null;
        __instance.bottom = null;
        return __instance;
    }

    public static Positioned CreateFromRelativeRect(Key? key = null, RelativeRect rect = default!, Widget child = default!)
    {
        var __instance = new Positioned(key, default!, default!, default!, default!, default!, default!, child);
        __instance.left = rect.left;
        __instance.top = rect.top;
        __instance.right = rect.right;
        __instance.bottom = rect.bottom;
        __instance.width = null;
        __instance.height = null;
        return __instance;
    }

    public static Positioned CreateFill(Key? key = null, double? left = 0.0, double? top = 0.0, double? right = 0.0, double? bottom = 0.0, Widget child = default!)
    {
        var __instance = new Positioned(key, left, top, right, bottom, default!, default!, child);
        __instance.left = left;
        __instance.top = top;
        __instance.right = right;
        __instance.bottom = bottom;
        __instance.width = null;
        __instance.height = null;
        return __instance;
    }

    public static Positioned CreateDirectional(Key? key = null, TextDirection textDirection = default!, double? start = null, double? top = null, double? end = null, double? bottom = null, double? width = null, double? height = null, Widget child = default!)
    {
        var (leftLocal, rightLocal) = textDirection switch { TextDirection.rtl => (end, start), TextDirection.ltr => (start, end), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        return new Positioned(key: key, left: leftLocal, top: top, right: rightLocal, bottom: bottom, width: width, height: height, child: child);
    }

    public override void applyParentData(RenderObject renderObject)
    {
        DartRuntimePrimitives.Assert(() => renderObject.parentData is StackParentData);
        var parentDataLocal = ((StackParentData?)renderObject.parentData!)!;
        var needsLayout = false;
        if (parentDataLocal.left != left)
        {
            parentDataLocal.left = left;
            needsLayout = true;
        }
        if (parentDataLocal.top != top)
        {
            parentDataLocal.top = top;
            needsLayout = true;
        }
        if (parentDataLocal.right != right)
        {
            parentDataLocal.right = right;
            needsLayout = true;
        }
        if (parentDataLocal.bottom != bottom)
        {
            parentDataLocal.bottom = bottom;
            needsLayout = true;
        }
        if (parentDataLocal.width != width)
        {
            parentDataLocal.width = width;
            needsLayout = true;
        }
        if (parentDataLocal.height != height)
        {
            parentDataLocal.height = height;
            needsLayout = true;
        }
        if (needsLayout)
        {
            renderObject.parent?.markNeedsLayout();
        }
    }

    public override Type debugTypicalAncestorWidgetClass => typeof(Stack);
    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("left", left, defaultValue: null));
        properties.add(new DoubleProperty("top", top, defaultValue: null));
        properties.add(new DoubleProperty("right", right, defaultValue: null));
        properties.add(new DoubleProperty("bottom", bottom, defaultValue: null));
        properties.add(new DoubleProperty("width", width, defaultValue: null));
        properties.add(new DoubleProperty("height", height, defaultValue: null));
    }

}

public class PositionedDirectional : StatelessWidget
{
    public virtual double? start { get; private set; }
    public virtual double? top { get; private set; }
    public virtual double? end { get; private set; }
    public virtual double? bottom { get; private set; }
    public virtual double? width { get; private set; }
    public virtual double? height { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    public PositionedDirectional(Key? key = null, double? start = null, double? top = null, double? end = null, double? bottom = null, double? width = null, double? height = null, Widget child = default!) : base(key: key)
    {
        this.start = start;
        this.top = top;
        this.end = end;
        this.bottom = bottom;
        this.width = width;
        this.height = height;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        return Positioned.CreateDirectional(textDirection: Directionality.of(context), start: start, top: top, end: end, bottom: bottom, width: width, height: height, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class Flex : MultiChildRenderObjectWidget
{
    public virtual Axis direction { get; private set; } = default!;
    public virtual MainAxisAlignment mainAxisAlignment { get; private set; } = default!;
    public virtual MainAxisSize mainAxisSize { get; private set; } = default!;
    public virtual CrossAxisAlignment crossAxisAlignment { get; private set; } = default!;
    public virtual TextDirection? textDirection { get; private set; }
    public virtual VerticalDirection verticalDirection { get; private set; } = default!;
    public virtual TextBaseline? textBaseline { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual double spacing { get; private set; } = default!;

    public Flex(Key? key = null, Axis direction = default!, MainAxisAlignment mainAxisAlignment = MainAxisAlignment.start, MainAxisSize mainAxisSize = MainAxisSize.max, CrossAxisAlignment crossAxisAlignment = CrossAxisAlignment.center, TextDirection? textDirection = null, VerticalDirection verticalDirection = VerticalDirection.down, TextBaseline? textBaseline = null, Clip clipBehavior = Clip.none, double spacing = 0.0, List<Widget> children = default!) : base(key: key, children: children ?? new List<Widget>())
    {
        this.direction = direction;
        this.mainAxisAlignment = mainAxisAlignment;
        this.mainAxisSize = mainAxisSize;
        this.crossAxisAlignment = crossAxisAlignment;
        this.textDirection = textDirection;
        this.verticalDirection = verticalDirection;
        this.textBaseline = textBaseline;
        this.clipBehavior = clipBehavior;
        this.spacing = spacing;
        System.Diagnostics.Debug.Assert(!DartRuntimePrimitives.Identical(crossAxisAlignment, CrossAxisAlignment.baseline) || (textBaseline is not null));
    }

    internal virtual bool _needTextDirection
    {
        get
        {
            switch (direction)
            {
                case Axis.horizontal:
                    {
                        return true;
                    }
                case Axis.vertical:
                    {
                        return Equals(crossAxisAlignment, CrossAxisAlignment.start) || Equals(crossAxisAlignment, CrossAxisAlignment.end);
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
        }
    }
    public virtual TextDirection? getEffectiveTextDirection(BuildContext context)
    {
        return textDirection ?? (_needTextDirection ? Directionality.maybeOf(context) : null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderFlex(direction: direction, mainAxisAlignment: mainAxisAlignment, mainAxisSize: mainAxisSize, crossAxisAlignment: crossAxisAlignment, textDirection: getEffectiveTextDirection(context), verticalDirection: verticalDirection, textBaseline: textBaseline, clipBehavior: clipBehavior, spacing: spacing);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderFlex)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderFlex>)(() =>
{
    var __cascade = __renderObject;
    __cascade.direction = direction;
    __cascade.mainAxisAlignment = mainAxisAlignment;
    __cascade.mainAxisSize = mainAxisSize;
    __cascade.crossAxisAlignment = crossAxisAlignment;
    __cascade.textDirection = getEffectiveTextDirection(context);
    __cascade.verticalDirection = verticalDirection;
    __cascade.textBaseline = textBaseline;
    __cascade.clipBehavior = clipBehavior;
    __cascade.spacing = spacing;
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<Axis>("direction", direction));
        properties.add(new EnumProperty<MainAxisAlignment>("mainAxisAlignment", mainAxisAlignment));
        properties.add(new EnumProperty<MainAxisSize>("mainAxisSize", mainAxisSize, defaultValue: MainAxisSize.max));
        properties.add(new EnumProperty<CrossAxisAlignment>("crossAxisAlignment", crossAxisAlignment));
        properties.add(new EnumProperty<TextDirection>("textDirection", textDirection, defaultValue: null));
        properties.add(new EnumProperty<VerticalDirection>("verticalDirection", verticalDirection, defaultValue: VerticalDirection.down));
        properties.add(new EnumProperty<TextBaseline>("textBaseline", textBaseline, defaultValue: null));
        properties.add(new EnumProperty<Clip>("clipBehavior", clipBehavior, defaultValue: Clip.none));
        properties.add(new DoubleProperty("spacing", spacing, defaultValue: 0.0));
    }

}

public class Row : Flex
{
    public Row(Key? key = null, MainAxisAlignment mainAxisAlignment = MainAxisAlignment.start, MainAxisSize mainAxisSize = MainAxisSize.max, CrossAxisAlignment crossAxisAlignment = CrossAxisAlignment.center, TextDirection? textDirection = null, VerticalDirection verticalDirection = VerticalDirection.down, TextBaseline? textBaseline = null, double spacing = 0.0, List<Widget> children = default!) : base(key: key, mainAxisAlignment: mainAxisAlignment, mainAxisSize: mainAxisSize, crossAxisAlignment: crossAxisAlignment, textDirection: textDirection, verticalDirection: verticalDirection, textBaseline: textBaseline, spacing: spacing, children: children ?? new List<Widget>(), direction: Axis.horizontal)
    {
    }

}

public class Column : Flex
{
    public Column(Key? key = null, MainAxisAlignment mainAxisAlignment = MainAxisAlignment.start, MainAxisSize mainAxisSize = MainAxisSize.max, CrossAxisAlignment crossAxisAlignment = CrossAxisAlignment.center, TextDirection? textDirection = null, VerticalDirection verticalDirection = VerticalDirection.down, TextBaseline? textBaseline = null, double spacing = 0.0, List<Widget> children = default!) : base(key: key, mainAxisAlignment: mainAxisAlignment, mainAxisSize: mainAxisSize, crossAxisAlignment: crossAxisAlignment, textDirection: textDirection, verticalDirection: verticalDirection, textBaseline: textBaseline, spacing: spacing, children: children ?? new List<Widget>(), direction: Axis.vertical)
    {
    }

}

public class Flexible : ParentDataWidget<FlexParentData>
{
    public virtual long flex { get; private set; } = default!;
    public virtual FlexFit fit { get; private set; } = default!;

    public Flexible(Key? key = null, long flex = 1, FlexFit fit = FlexFit.loose, Widget child = default!) : base(key: key, child: child)
    {
        this.flex = flex;
        this.fit = fit;
    }

    public override void applyParentData(RenderObject renderObject)
    {
        DartRuntimePrimitives.Assert(() => renderObject.parentData is FlexParentData);
        var parentDataLocal = ((FlexParentData?)renderObject.parentData!)!;
        var needsLayout = false;
        if (parentDataLocal.flex != flex)
        {
            parentDataLocal.flex = flex;
            needsLayout = true;
        }
        if (!Equals(parentDataLocal.fit, fit))
        {
            parentDataLocal.fit = fit;
            needsLayout = true;
        }
        if (needsLayout)
        {
            renderObject.parent?.markNeedsLayout();
        }
    }

    public override Type debugTypicalAncestorWidgetClass => typeof(Flex);
    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new IntProperty("flex", flex));
    }

}

public class Expanded : Flexible
{
    public Expanded(Key? key = null, long flex = 1, Widget child = default!) : base(key: key, flex: flex, child: child, fit: FlexFit.tight)
    {
    }

}

public class Wrap : MultiChildRenderObjectWidget
{
    public virtual Axis direction { get; private set; } = default!;
    public virtual WrapAlignment alignment { get; private set; } = default!;
    public virtual double spacing { get; private set; } = default!;
    public virtual WrapAlignment runAlignment { get; private set; } = default!;
    public virtual double runSpacing { get; private set; } = default!;
    public virtual WrapCrossAlignment crossAxisAlignment { get; private set; } = default!;
    public virtual TextDirection? textDirection { get; private set; }
    public virtual VerticalDirection verticalDirection { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;

    public Wrap(Key? key = null, Axis direction = Axis.horizontal, WrapAlignment alignment = WrapAlignment.start, double spacing = 0.0, WrapAlignment runAlignment = WrapAlignment.start, double runSpacing = 0.0, WrapCrossAlignment crossAxisAlignment = WrapCrossAlignment.start, TextDirection? textDirection = null, VerticalDirection verticalDirection = VerticalDirection.down, Clip clipBehavior = Clip.none, List<Widget> children = default!) : base(key: key, children: children ?? new List<Widget>())
    {
        this.direction = direction;
        this.alignment = alignment;
        this.spacing = spacing;
        this.runAlignment = runAlignment;
        this.runSpacing = runSpacing;
        this.crossAxisAlignment = crossAxisAlignment;
        this.textDirection = textDirection;
        this.verticalDirection = verticalDirection;
        this.clipBehavior = clipBehavior;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderWrap(direction: direction, alignment: alignment, spacing: spacing, runAlignment: runAlignment, runSpacing: runSpacing, crossAxisAlignment: crossAxisAlignment, textDirection: textDirection ?? Directionality.maybeOf(context), verticalDirection: verticalDirection, clipBehavior: clipBehavior);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderWrap)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderWrap>)(() =>
{
    var __cascade = __renderObject;
    __cascade.direction = direction;
    __cascade.alignment = alignment;
    __cascade.spacing = spacing;
    __cascade.runAlignment = runAlignment;
    __cascade.runSpacing = runSpacing;
    __cascade.crossAxisAlignment = crossAxisAlignment;
    __cascade.textDirection = textDirection ?? Directionality.maybeOf(context);
    __cascade.verticalDirection = verticalDirection;
    __cascade.clipBehavior = clipBehavior;
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<Axis>("direction", direction));
        properties.add(new EnumProperty<WrapAlignment>("alignment", alignment));
        properties.add(new DoubleProperty("spacing", spacing));
        properties.add(new EnumProperty<WrapAlignment>("runAlignment", runAlignment));
        properties.add(new DoubleProperty("runSpacing", runSpacing));
        properties.add(new EnumProperty<WrapCrossAlignment>("crossAxisAlignment", crossAxisAlignment));
        properties.add(new EnumProperty<TextDirection>("textDirection", textDirection, defaultValue: null));
        properties.add(new EnumProperty<VerticalDirection>("verticalDirection", verticalDirection, defaultValue: VerticalDirection.down));
    }

}

public class Flow : MultiChildRenderObjectWidget
{
    public virtual FlowDelegate @delegate { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;

    public Flow(Key? key = null, FlowDelegate @delegate = default!, List<Widget> children = default!, Clip clipBehavior = Clip.hardEdge) : base(key: key, children: RepaintBoundary.wrapAll(children))
    {
        List<Widget> __children = children ?? new List<Widget>();
        this.@delegate = @delegate;
        this.clipBehavior = clipBehavior;
    }

    public static Flow CreateUnwrapped(Key? key = null, FlowDelegate @delegate = default!, List<Widget> children = default!, Clip clipBehavior = Clip.hardEdge)
    {
        var __instance = new Flow(key, @delegate, children, clipBehavior);
        List<Widget> __children = children ?? new List<Widget>();
        __instance.@delegate = @delegate;
        __instance.clipBehavior = clipBehavior;
        return __instance;
    }

    public override RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<RenderObject>(new RenderFlow(@delegate: @delegate, clipBehavior: clipBehavior));
    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderFlow)renderObject;
        __renderObject.@delegate = @delegate;
        __renderObject.clipBehavior = clipBehavior;
    }

}

public class RichText : MultiChildRenderObjectWidget
{
    public virtual InlineSpan text { get; private set; } = default!;
    public virtual TextAlign textAlign { get; private set; } = default!;
    public virtual TextDirection? textDirection { get; private set; }
    public virtual bool softWrap { get; private set; } = default!;
    public virtual TextOverflow overflow { get; private set; } = default!;
    public virtual TextScaler textScaler { get; private set; } = default!;
    public virtual long? maxLines { get; private set; }
    public virtual Locale? locale { get; private set; }
    public virtual Painting.StrutStyle? strutStyle { get; private set; }
    public virtual TextWidthBasis textWidthBasis { get; private set; } = default!;
    public virtual TextHeightBehavior? textHeightBehavior { get; private set; }
    public virtual SelectionRegistrar? selectionRegistrar { get; private set; }
    public virtual Color? selectionColor { get; private set; }

    public RichText(Key? key = null, InlineSpan text = default!, TextAlign textAlign = TextAlign.start, TextDirection? textDirection = null, bool softWrap = true, TextOverflow overflow = TextOverflow.clip, double textScaleFactor = 1.0, TextScaler? textScaler = null, long? maxLines = null, Locale? locale = null, Painting.StrutStyle? strutStyle = null, TextWidthBasis textWidthBasis = TextWidthBasis.parent, TextHeightBehavior? textHeightBehavior = null, SelectionRegistrar? selectionRegistrar = null, Color? selectionColor = null) : base(key: key, children: WidgetSpan.extractFromInlineSpan(text, _effectiveTextScalerFrom(textScaler, textScaleFactor)))
    {
        TextScaler __textScaler = textScaler ?? TextScaler.noScaling;
        this.text = text;
        this.textAlign = textAlign;
        this.textDirection = textDirection;
        this.softWrap = softWrap;
        this.overflow = overflow;
        this.maxLines = maxLines;
        this.locale = locale;
        this.strutStyle = strutStyle;
        this.textWidthBasis = textWidthBasis;
        this.textHeightBehavior = textHeightBehavior;
        this.selectionRegistrar = selectionRegistrar;
        this.selectionColor = selectionColor;
        this.textScaler = _effectiveTextScalerFrom(textScaler, textScaleFactor);
        System.Diagnostics.Debug.Assert((maxLines is null) || (DartRuntimePrimitives.RequireValue(maxLines) > 0L));
        System.Diagnostics.Debug.Assert((selectionRegistrar is null) || (selectionColor is not null));
        System.Diagnostics.Debug.Assert((textScaleFactor == 1.0) || DartRuntimePrimitives.Identical(__textScaler, TextScaler.noScaling));
    }

    internal static TextScaler _effectiveTextScalerFrom(TextScaler? textScaler, double textScaleFactor)
    {
        textScaler ??= TextScaler.noScaling;
        textScaler ??= TextScaler.noScaling;
        return (textScaler, textScaleFactor) switch { (TextScaler scaler, 1.0) => scaler, (var __constant252140, double textScaleFactorLocal) when Equals(__constant252140, TextScaler.noScaling) => TextScaler.CreateLinear(textScaleFactorLocal), (TextScaler scalerLocal, _) => scalerLocal };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double textScaleFactor => textScaler.textScaleFactor;
    internal virtual double _getDevicePixelRatio(BuildContext context) => DartRuntimePrimitives.ConvertValue<double>((MediaQuery.maybeDevicePixelRatioOf(context) ?? View.maybeOf(context)?.devicePixelRatio) ?? 1.0);
    public override RenderObject createRenderObject(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => (textDirection is not null) || DebugLibrary.debugCheckHasDirectionality(context));
        return new RenderParagraph(text, textAlign: textAlign, textDirection: textDirection ?? Directionality.of(context), softWrap: softWrap, overflow: overflow, textScaler: textScaler, maxLines: maxLines, strutStyle: strutStyle, textWidthBasis: textWidthBasis, textHeightBehavior: textHeightBehavior, locale: locale ?? Localizations.maybeLocaleOf(context), registrar: selectionRegistrar, selectionColor: selectionColor, devicePixelRatio: _getDevicePixelRatio(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderParagraph)renderObject;
        DartRuntimePrimitives.Assert(() => (textDirection is not null) || DebugLibrary.debugCheckHasDirectionality(context));
        DartRuntimePrimitives.Ignore(((Func<RenderParagraph>)(() =>
{
    var __cascade = __renderObject;
    __cascade.text = text;
    __cascade.textAlign = textAlign;
    __cascade.textDirection = textDirection ?? Directionality.of(context);
    __cascade.softWrap = softWrap;
    __cascade.overflow = overflow;
    __cascade.textScaler = textScaler;
    __cascade.maxLines = maxLines;
    __cascade.strutStyle = strutStyle;
    __cascade.textWidthBasis = textWidthBasis;
    __cascade.textHeightBehavior = textHeightBehavior;
    __cascade.locale = locale ?? Localizations.maybeLocaleOf(context);
    __cascade.registrar = selectionRegistrar;
    __cascade.selectionColor = selectionColor;
    __cascade.devicePixelRatio = _getDevicePixelRatio(context);
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<TextAlign>("textAlign", textAlign, defaultValue: TextAlign.start));
        properties.add(new EnumProperty<TextDirection>("textDirection", textDirection, defaultValue: null));
        properties.add(new FlagProperty("softWrap", value: softWrap, ifTrue: "wrapping at box width", ifFalse: "no wrapping except at line break characters", showName: true));
        properties.add(new EnumProperty<TextOverflow>("overflow", overflow, defaultValue: TextOverflow.clip));
        properties.add(new DiagnosticsProperty<TextScaler>("textScaler", textScaler, defaultValue: TextScaler.noScaling));
        properties.add(new IntProperty("maxLines", maxLines, ifNull: "unlimited"));
        properties.add(new EnumProperty<TextWidthBasis>("textWidthBasis", textWidthBasis, defaultValue: TextWidthBasis.parent));
        properties.add(new StringProperty("text", text.toPlainText()));
        properties.add(new DiagnosticsProperty<Locale>("locale", locale, defaultValue: null));
        properties.add(new DiagnosticsProperty<Painting.StrutStyle>("strutStyle", strutStyle, defaultValue: null));
        properties.add(new DiagnosticsProperty<TextHeightBehavior>("textHeightBehavior", textHeightBehavior, defaultValue: null));
    }

}

public class RawImage : LeafRenderObjectWidget
{
    public virtual Ui.Image? image { get; private set; }
    public virtual string? debugImageLabel { get; private set; }
    public virtual double? width { get; private set; }
    public virtual double? height { get; private set; }
    public virtual double scale { get; private set; } = default!;
    public virtual Color? color { get; private set; }
    public virtual Animation<double>? opacity { get; private set; }
    public virtual FilterQuality filterQuality { get; private set; } = default!;
    public virtual BlendMode? colorBlendMode { get; private set; }
    public virtual BoxFit? fit { get; private set; }
    public virtual AlignmentGeometry alignment { get; private set; } = default!;
    public virtual ImageRepeat repeat { get; private set; } = default!;
    public virtual Rect? centerSlice { get; private set; }
    public virtual bool matchTextDirection { get; private set; } = default!;
    public virtual bool invertColors { get; private set; } = default!;
    public virtual bool isAntiAlias { get; private set; } = default!;
    public virtual BlendMode blendMode { get; private set; } = default!;

    public RawImage(Key? key = null, Ui.Image? image = null, string? debugImageLabel = null, double? width = null, double? height = null, double scale = 1.0, Color? color = null, Animation<double>? opacity = null, BlendMode? colorBlendMode = null, BoxFit? fit = null, AlignmentGeometry alignment = default!, ImageRepeat repeat = ImageRepeat.noRepeat, Rect? centerSlice = null, bool matchTextDirection = false, bool invertColors = false, FilterQuality filterQuality = FilterQuality.medium, bool isAntiAlias = false, BlendMode blendMode = BlendMode.srcOver) : base(key: key)
    {
        AlignmentGeometry __alignment = alignment ?? Alignment.center;
        this.image = image;
        this.debugImageLabel = debugImageLabel;
        this.width = width;
        this.height = height;
        this.scale = scale;
        this.color = color;
        this.opacity = opacity;
        this.colorBlendMode = colorBlendMode;
        this.fit = fit;
        this.alignment = __alignment;
        this.repeat = repeat;
        this.centerSlice = centerSlice;
        this.matchTextDirection = matchTextDirection;
        this.invertColors = invertColors;
        this.filterQuality = filterQuality;
        this.isAntiAlias = isAntiAlias;
        this.blendMode = blendMode;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => !matchTextDirection && (alignment is Alignment) || DebugLibrary.debugCheckHasDirectionality(context));
        DartRuntimePrimitives.Assert(() => image?.debugDisposed != true, () => (object?)"Creator of a RawImage disposed of the image when the RawImage still " + "needed it.");
        return new RenderImage(image: image?.clone(), debugImageLabel: debugImageLabel, width: width, height: height, scale: scale, color: color, opacity: opacity, colorBlendMode: colorBlendMode, fit: fit, alignment: alignment, repeat: repeat, centerSlice: centerSlice, matchTextDirection: matchTextDirection, textDirection: (matchTextDirection || (alignment is not Alignment)) ? Directionality.of(context) : null, invertColors: invertColors, isAntiAlias: isAntiAlias, filterQuality: filterQuality, blendMode: blendMode);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderImage)renderObject;
        DartRuntimePrimitives.Assert(() => image?.debugDisposed != true, () => (object?)"Creator of a RawImage disposed of the image when the RawImage still " + "needed it.");
        DartRuntimePrimitives.Ignore(((Func<RenderImage>)(() =>
{
    var __cascade = __renderObject;
    __cascade.image = image?.clone();
    __cascade.debugImageLabel = debugImageLabel;
    __cascade.width = width;
    __cascade.height = height;
    __cascade.scale = scale;
    __cascade.color = color;
    __cascade.opacity = opacity;
    __cascade.colorBlendMode = colorBlendMode;
    __cascade.fit = fit;
    __cascade.alignment = alignment;
    __cascade.repeat = repeat;
    __cascade.centerSlice = centerSlice;
    __cascade.matchTextDirection = matchTextDirection;
    __cascade.textDirection = (matchTextDirection || (alignment is not Alignment)) ? Directionality.of(context) : null;
    __cascade.invertColors = invertColors;
    __cascade.isAntiAlias = isAntiAlias;
    __cascade.filterQuality = filterQuality;
    __cascade.blendMode = blendMode;
    return __cascade;
}))());
    }

    public override void didUnmountRenderObject(RenderObject renderObject)
    {
        var __renderObject = (RenderImage)renderObject;
        __renderObject.image = null;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Ui.Image>("image", image));
        properties.add(new DoubleProperty("width", width, defaultValue: null));
        properties.add(new DoubleProperty("height", height, defaultValue: null));
        properties.add(new DoubleProperty("scale", scale, defaultValue: 1.0));
        properties.add(new ColorProperty("color", color, defaultValue: null));
        properties.add(new DiagnosticsProperty<Animation<double>?>("opacity", opacity, defaultValue: null));
        properties.add(new EnumProperty<BlendMode>("colorBlendMode", colorBlendMode, defaultValue: null));
        properties.add(new EnumProperty<BoxFit>("fit", fit, defaultValue: null));
        properties.add(new DiagnosticsProperty<AlignmentGeometry>("alignment", alignment, defaultValue: null));
        properties.add(new EnumProperty<ImageRepeat>("repeat", repeat, defaultValue: ImageRepeat.noRepeat));
        properties.add(new DiagnosticsProperty<Rect>("centerSlice", centerSlice, defaultValue: null));
        properties.add(new FlagProperty("matchTextDirection", value: matchTextDirection, ifTrue: "match text direction"));
        properties.add(new DiagnosticsProperty<bool>("invertColors", invertColors));
        properties.add(new EnumProperty<FilterQuality>("filterQuality", filterQuality));
        properties.add(new EnumProperty<BlendMode>("blendMode", blendMode, defaultValue: BlendMode.srcOver));
    }

}

public class DefaultAssetBundle : InheritedWidget
{
    public virtual AssetBundle bundle { get; private set; } = default!;

    public DefaultAssetBundle(Key? key = null, AssetBundle bundle = default!, Widget child = default!) : base(key: key, child: child)
    {
        this.bundle = bundle;
    }

    public static AssetBundle of(BuildContext context)
    {
        DefaultAssetBundle? result = context.dependOnInheritedWidgetOfExactType<DefaultAssetBundle>();
        return result?.bundle ?? Asset_bundleLibrary.rootBundle;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(bundle, ((DefaultAssetBundle)oldWidget).bundle));
}

public class WidgetToRenderBoxAdapter : LeafRenderObjectWidget
{
    public virtual RenderBox renderBox { get; private set; } = default!;
    public virtual Action? onBuild { get; private set; }
    public virtual Action? onUnmount { get; private set; }

    public WidgetToRenderBoxAdapter(RenderBox renderBox, Action? onBuild = null, Action? onUnmount = null) : base(key: new GlobalObjectKey<IState>(renderBox))
    {
        this.renderBox = renderBox;
        this.onBuild = onBuild;
        this.onUnmount = onUnmount;
    }

    public override RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<RenderObject>(renderBox);
    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderBox)renderObject;
        onBuild?.Invoke();
    }

    public override void didUnmountRenderObject(RenderObject renderObject)
    {
        DartRuntimePrimitives.Assert(() => Equals(renderObject, renderBox));
        onUnmount?.Invoke();
    }

}

public class Listener : SingleChildRenderObjectWidget
{
    public virtual Action<Gestures.PointerDownEvent>? onPointerDown { get; private set; }
    public virtual Action<Gestures.PointerMoveEvent>? onPointerMove { get; private set; }
    public virtual Action<Gestures.PointerUpEvent>? onPointerUp { get; private set; }
    public virtual Action<Gestures.PointerHoverEvent>? onPointerHover { get; private set; }
    public virtual Action<Gestures.PointerCancelEvent>? onPointerCancel { get; private set; }
    public virtual Action<PointerPanZoomStartEvent>? onPointerPanZoomStart { get; private set; }
    public virtual Action<PointerPanZoomUpdateEvent>? onPointerPanZoomUpdate { get; private set; }
    public virtual Action<PointerPanZoomEndEvent>? onPointerPanZoomEnd { get; private set; }
    public virtual Action<PointerSignalEvent>? onPointerSignal { get; private set; }
    public virtual HitTestBehavior behavior { get; private set; } = default!;

    public Listener(Key? key = null, Action<Gestures.PointerDownEvent>? onPointerDown = null, Action<Gestures.PointerMoveEvent>? onPointerMove = null, Action<Gestures.PointerUpEvent>? onPointerUp = null, Action<Gestures.PointerHoverEvent>? onPointerHover = null, Action<Gestures.PointerCancelEvent>? onPointerCancel = null, Action<PointerPanZoomStartEvent>? onPointerPanZoomStart = null, Action<PointerPanZoomUpdateEvent>? onPointerPanZoomUpdate = null, Action<PointerPanZoomEndEvent>? onPointerPanZoomEnd = null, Action<PointerSignalEvent>? onPointerSignal = null, HitTestBehavior behavior = HitTestBehavior.deferToChild, Widget? child = null) : base(key: key, child: child)
    {
        this.onPointerDown = onPointerDown;
        this.onPointerMove = onPointerMove;
        this.onPointerUp = onPointerUp;
        this.onPointerHover = onPointerHover;
        this.onPointerCancel = onPointerCancel;
        this.onPointerPanZoomStart = onPointerPanZoomStart;
        this.onPointerPanZoomUpdate = onPointerPanZoomUpdate;
        this.onPointerPanZoomEnd = onPointerPanZoomEnd;
        this.onPointerSignal = onPointerSignal;
        this.behavior = behavior;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderPointerListener(onPointerDown: onPointerDown, onPointerMove: onPointerMove, onPointerUp: onPointerUp, onPointerHover: onPointerHover, onPointerCancel: onPointerCancel, onPointerPanZoomStart: onPointerPanZoomStart, onPointerPanZoomUpdate: onPointerPanZoomUpdate, onPointerPanZoomEnd: onPointerPanZoomEnd, onPointerSignal: onPointerSignal, behavior: behavior);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderPointerListener)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderPointerListener>)(() =>
{
    var __cascade = __renderObject;
    __cascade.onPointerDown = onPointerDown;
    __cascade.onPointerMove = onPointerMove;
    __cascade.onPointerUp = onPointerUp;
    __cascade.onPointerHover = onPointerHover;
    __cascade.onPointerCancel = onPointerCancel;
    __cascade.onPointerPanZoomStart = onPointerPanZoomStart;
    __cascade.onPointerPanZoomUpdate = onPointerPanZoomUpdate;
    __cascade.onPointerPanZoomEnd = onPointerPanZoomEnd;
    __cascade.onPointerSignal = onPointerSignal;
    __cascade.behavior = behavior;
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        var listeners = new List<string>();
        properties.add(new IterableProperty<string>("listeners", listeners.Cast<string>(), ifEmpty: "<none>"));
        properties.add(new EnumProperty<HitTestBehavior>("behavior", behavior));
    }

}

public class MouseRegion : SingleChildRenderObjectWidget
{
    public virtual Action<Gestures.PointerEnterEvent>? onEnter { get; private set; }
    public virtual Action<Gestures.PointerHoverEvent>? onHover { get; private set; }
    public virtual Action<Gestures.PointerExitEvent>? onExit { get; private set; }
    public virtual MouseCursor cursor { get; private set; } = default!;
    public virtual bool opaque { get; private set; } = default!;
    public virtual HitTestBehavior? hitTestBehavior { get; private set; }

    public MouseRegion(Key? key = null, Action<Gestures.PointerEnterEvent>? onEnter = null, Action<Gestures.PointerExitEvent>? onExit = null, Action<Gestures.PointerHoverEvent>? onHover = null, MouseCursor cursor = default!, bool opaque = true, HitTestBehavior? hitTestBehavior = null, Widget? child = null) : base(key: key, child: child)
    {
        MouseCursor __cursor = cursor ?? MouseCursor.defer;
        this.onEnter = onEnter;
        this.onExit = onExit;
        this.onHover = onHover;
        this.cursor = __cursor;
        this.opaque = opaque;
        this.hitTestBehavior = hitTestBehavior;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderMouseRegion(onEnter: onEnter, onHover: onHover, onExit: onExit, cursor: cursor, opaque: opaque, hitTestBehavior: hitTestBehavior);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderMouseRegion)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderMouseRegion>)(() =>
{
    var __cascade = __renderObject;
    __cascade.onEnter = onEnter;
    __cascade.onHover = onHover;
    __cascade.onExit = onExit;
    __cascade.cursor = cursor;
    __cascade.opaque = opaque;
    __cascade.hitTestBehavior = hitTestBehavior;
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        var listeners = new List<string>();
        properties.add(new IterableProperty<string>("listeners", listeners.Cast<string>(), ifEmpty: "<none>"));
        properties.add(new DiagnosticsProperty<MouseCursor>("cursor", cursor, defaultValue: null));
        properties.add(new DiagnosticsProperty<bool>("opaque", opaque, defaultValue: true));
    }

}

public class RepaintBoundary : SingleChildRenderObjectWidget
{
    public RepaintBoundary(Key? key = null, Widget? child = null) : base(key: key, child: child)
    {
    }

    public static RepaintBoundary CreateWrap(Widget child, long childIndex)
    {
        return new RepaintBoundary(
            new ValueKey<object>(child.key ?? (object)childIndex),
            child);
    }

    public static List<RepaintBoundary> wrapAll(List<Widget> widgets) => new List<RepaintBoundary>();
    public override RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<RenderObject>(new RenderRepaintBoundary());
}

public class IgnorePointer : SingleChildRenderObjectWidget
{
    public virtual bool ignoring { get; private set; } = default!;
    public virtual bool? ignoringSemantics { get; private set; }

    public IgnorePointer(Key? key = null, bool ignoring = true, bool? ignoringSemantics = null, Widget? child = null) : base(key: key, child: child)
    {
        this.ignoring = ignoring;
        this.ignoringSemantics = ignoringSemantics;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderIgnorePointer(ignoring: ignoring, ignoringSemantics: ignoringSemantics);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderIgnorePointer)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderIgnorePointer>)(() =>
{
    var __cascade = __renderObject;
    __cascade.ignoring = ignoring;
    __cascade.ignoringSemantics = ignoringSemantics;
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<bool>("ignoring", ignoring));
        properties.add(new DiagnosticsProperty<bool>("ignoringSemantics", ignoringSemantics, defaultValue: null));
    }

}

public class AbsorbPointer : SingleChildRenderObjectWidget
{
    public virtual bool absorbing { get; private set; } = default!;
    public virtual bool? ignoringSemantics { get; private set; }

    public AbsorbPointer(Key? key = null, bool absorbing = true, bool? ignoringSemantics = null, Widget? child = null) : base(key: key, child: child)
    {
        this.absorbing = absorbing;
        this.ignoringSemantics = ignoringSemantics;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderAbsorbPointer(absorbing: absorbing, ignoringSemantics: ignoringSemantics);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderAbsorbPointer)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderAbsorbPointer>)(() =>
{
    var __cascade = __renderObject;
    __cascade.absorbing = absorbing;
    __cascade.ignoringSemantics = ignoringSemantics;
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<bool>("absorbing", absorbing));
        properties.add(new DiagnosticsProperty<bool>("ignoringSemantics", ignoringSemantics, defaultValue: null));
    }

}

public class MetaData : SingleChildRenderObjectWidget
{
    public virtual object? metaData { get; private set; } = default!;
    public virtual HitTestBehavior behavior { get; private set; } = default!;

    public MetaData(Key? key = null, object? metaData = default!, HitTestBehavior behavior = HitTestBehavior.deferToChild, Widget? child = null) : base(key: key, child: child)
    {
        this.metaData = metaData;
        this.behavior = behavior;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderMetaData(metaData: metaData, behavior: behavior);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderMetaData)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderMetaData>)(() =>
{
    var __cascade = __renderObject;
    __cascade.metaData = metaData;
    __cascade.behavior = behavior;
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<HitTestBehavior>("behavior", behavior));
        properties.add(new DiagnosticsProperty<object>("metaData", metaData));
    }

}

public class Semantics : _SemanticsBase__basic
{
    private Semantics(Key? key, Widget? child, bool container, bool explicitChildNodes, bool excludeSemantics, bool blockUserActions, Locale? localeForSubtree, SemanticsProperties properties) : base(key: key, child: child, container: container, explicitChildNodes: explicitChildNodes, excludeSemantics: excludeSemantics, blockUserActions: blockUserActions, localeForSubtree: localeForSubtree, properties: properties)
    {
    }

    public Semantics(Key? key = null, Widget? child = null, bool container = false, bool explicitChildNodes = false, bool excludeSemantics = false, bool blockUserActions = false, bool? enabled = null, bool? @checked = null, bool? mixed = null, bool? selected = null, bool? toggled = null, bool? button = null, bool? slider = null, bool? keyboardKey = null, bool? link = null, DartUri? linkUrl = null, bool? header = null, long? headingLevel = null, bool? textField = null, bool? readOnly = null, bool? focusable = null, bool? focused = null, AccessibilityFocusBlockType? accessibilityFocusBlockType = null, bool? inMutuallyExclusiveGroup = null, bool? obscured = null, bool? multiline = null, bool? scopesRoute = null, bool? namesRoute = null, bool? hidden = null, bool? image = null, bool? liveRegion = null, bool? expanded = null, bool? isRequired = null, long? maxValueLength = null, long? currentValueLength = null, string? identifier = null, object? traversalParentIdentifier = null, object? traversalChildIdentifier = null, string? label = null, AttributedString? attributedLabel = null, string? value = null, AttributedString? attributedValue = null, string? increasedValue = null, AttributedString? attributedIncreasedValue = null, string? decreasedValue = null, AttributedString? attributedDecreasedValue = null, string? hint = null, AttributedString? attributedHint = null, string? tooltip = null, string? onTapHint = null, string? onLongPressHint = null, TextDirection? textDirection = null, SemanticsSortKey? sortKey = null, SemanticsTag? tagForChildren = null, Action? onTap = null, Action? onLongPress = null, Action? onScrollLeft = null, Action? onScrollRight = null, Action? onScrollUp = null, Action? onScrollDown = null, Action? onIncrease = null, Action? onDecrease = null, Action? onCopy = null, Action? onCut = null, Action? onPaste = null, Action? onDismiss = null, Action<bool>? onMoveCursorForwardByCharacter = null, Action<bool>? onMoveCursorBackwardByCharacter = null, Action<TextSelection>? onSetSelection = null, Action<string>? onSetText = null, Action? onDidGainAccessibilityFocus = null, Action? onDidLoseAccessibilityFocus = null, Action? onFocus = null, Action? onExpand = null, Action? onCollapse = null, DartMap<CustomSemanticsAction, Action>? customSemanticsActions = null, SemanticsRole? role = null, HashSet<string>? controlsNodes = null, SemanticsValidationResult validationResult = SemanticsValidationResult.none, SemanticsHitTestBehavior? hitTestBehavior = null, SemanticsInputType? inputType = null, Locale? localeForSubtree = null, string? minValue = null, string? maxValue = null) : base(key: key, child: child, container: container, explicitChildNodes: explicitChildNodes, excludeSemantics: excludeSemantics, blockUserActions: blockUserActions, enabled: enabled, @checked: @checked, mixed: mixed, selected: selected, toggled: toggled, button: button, slider: slider, keyboardKey: keyboardKey, link: link, linkUrl: linkUrl, header: header, headingLevel: headingLevel, textField: textField, readOnly: readOnly, focusable: focusable, focused: focused, accessibilityFocusBlockType: accessibilityFocusBlockType, inMutuallyExclusiveGroup: inMutuallyExclusiveGroup, obscured: obscured, multiline: multiline, scopesRoute: scopesRoute, namesRoute: namesRoute, hidden: hidden, image: image, liveRegion: liveRegion, expanded: expanded, isRequired: isRequired, maxValueLength: maxValueLength, currentValueLength: currentValueLength, identifier: identifier, traversalParentIdentifier: traversalParentIdentifier, traversalChildIdentifier: traversalChildIdentifier, label: label, attributedLabel: attributedLabel, value: value, attributedValue: attributedValue, increasedValue: increasedValue, attributedIncreasedValue: attributedIncreasedValue, decreasedValue: decreasedValue, attributedDecreasedValue: attributedDecreasedValue, hint: hint, attributedHint: attributedHint, tooltip: tooltip, onTapHint: onTapHint, onLongPressHint: onLongPressHint, textDirection: textDirection, sortKey: sortKey, tagForChildren: tagForChildren, onTap: onTap, onLongPress: onLongPress, onScrollLeft: onScrollLeft, onScrollRight: onScrollRight, onScrollUp: onScrollUp, onScrollDown: onScrollDown, onIncrease: onIncrease, onDecrease: onDecrease, onCopy: onCopy, onCut: onCut, onPaste: onPaste, onDismiss: onDismiss, onMoveCursorForwardByCharacter: onMoveCursorForwardByCharacter, onMoveCursorBackwardByCharacter: onMoveCursorBackwardByCharacter, onSetSelection: onSetSelection, onSetText: onSetText, onDidGainAccessibilityFocus: onDidGainAccessibilityFocus, onDidLoseAccessibilityFocus: onDidLoseAccessibilityFocus, onFocus: onFocus, onExpand: onExpand, onCollapse: onCollapse, customSemanticsActions: customSemanticsActions, role: role, controlsNodes: controlsNodes, validationResult: validationResult, hitTestBehavior: hitTestBehavior, inputType: inputType, localeForSubtree: localeForSubtree, minValue: minValue, maxValue: maxValue)
    {
    }

    public static Semantics CreateFromProperties(Key? key = null, Widget? child = null, bool container = false, bool explicitChildNodes = false, bool excludeSemantics = false, bool blockUserActions = false, Locale? localeForSubtree = null, SemanticsProperties properties = default!)
    {
        return new Semantics(key, child, container, explicitChildNodes, excludeSemantics, blockUserActions, localeForSubtree, properties);
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderSemanticsAnnotations(container: container, explicitChildNodes: explicitChildNodes, excludeSemantics: excludeSemantics, blockUserActions: blockUserActions, properties: properties, localeForSubtree: localeForSubtree, textDirection: _getTextDirection(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderSemanticsAnnotations)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderSemanticsAnnotations>)(() =>
{
    var __cascade = __renderObject;
    __cascade.container = container;
    __cascade.explicitChildNodes = explicitChildNodes;
    __cascade.excludeSemantics = excludeSemantics;
    __cascade.blockUserActions = blockUserActions;
    __cascade.properties = properties;
    __cascade.textDirection = _getTextDirection(context);
    __cascade.localeForSubtree = localeForSubtree;
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<bool>("container", container));
        properties.add(new DiagnosticsProperty<SemanticsProperties>("properties", this.properties));
        this.properties.debugFillProperties(properties);
    }

}

public class MergeSemantics : SingleChildRenderObjectWidget
{
    public MergeSemantics(Key? key = null, Widget? child = null) : base(key: key, child: child)
    {
    }

    public override RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<RenderObject>(new RenderMergeSemantics());
}

public class BlockSemantics : SingleChildRenderObjectWidget
{
    public virtual bool blocking { get; private set; } = default!;

    public BlockSemantics(Key? key = null, bool blocking = true, Widget? child = null) : base(key: key, child: child)
    {
        this.blocking = blocking;
    }

    public override RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<RenderObject>(new RenderBlockSemantics(blocking: blocking));
    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderBlockSemantics)renderObject;
        __renderObject.blocking = blocking;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<bool>("blocking", blocking));
    }

}

public class ExcludeSemantics : SingleChildRenderObjectWidget
{
    public virtual bool excluding { get; private set; } = default!;

    public ExcludeSemantics(Key? key = null, bool excluding = true, Widget? child = null) : base(key: key, child: child)
    {
        this.excluding = excluding;
    }

    public override RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<RenderObject>(new RenderExcludeSemantics(excluding: excluding));
    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderExcludeSemantics)renderObject;
        __renderObject.excluding = excluding;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<bool>("excluding", excluding));
    }

}

public class IndexedSemantics : SingleChildRenderObjectWidget
{
    public virtual long index { get; private set; } = default!;

    public IndexedSemantics(Key? key = null, long index = default!, Widget? child = null) : base(key: key, child: child)
    {
        this.index = index;
    }

    public override RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<RenderObject>(new RenderIndexedSemantics(index: index));
    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderIndexedSemantics)renderObject;
        __renderObject.index = index;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<long>("index", index));
    }

}

public class KeyedSubtree : StatelessWidget
{
    public virtual Widget child { get; private set; } = default!;

    public KeyedSubtree(Key? key = null, Widget child = default!) : base(key: key)
    {
        this.child = child;
    }

    public static KeyedSubtree CreateWrap(Widget child, long childIndex)
    {
        var __instance = new KeyedSubtree(default!, child);
        __instance.child = child;
        return __instance;
    }

    public static List<Widget> ensureUniqueKeysForList(List<Widget> items, long baseIndex = 0)
    {
        if (!Enumerable.Any(items))
        {
            return items;
        }
        var itemsWithUniqueKeys = new List<Widget>();
        DartRuntimePrimitives.Assert(() => !DebugLibrary.debugItemsHaveDuplicateKeys(itemsWithUniqueKeys.Cast<Widget>()));
        return itemsWithUniqueKeys;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context) => child;
}

public class Builder : StatelessWidget
{
    public virtual Func<BuildContext, Widget> builder { get; private set; } = default!;

    public Builder(Key? key = null, Func<BuildContext, Widget> builder = default!) : base(key: key)
    {
        this.builder = builder;
    }

    public override Widget build(BuildContext context) => builder(context);
}

public delegate Widget StatefulWidgetBuilder(BuildContext context, Action<Action> setState);

public class StatefulBuilder : StatefulWidget
{
    public virtual Func<BuildContext, Action<Action>, Widget> builder { get; private set; } = default!;

    public StatefulBuilder(Key? key = null, Func<BuildContext, Action<Action>, Widget> builder = default!) : base(key: key)
    {
        this.builder = builder;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _StatefulBuilderState__basic());
}

internal class _StatefulBuilderState__basic : State<StatefulBuilder>
{
    public override Widget build(BuildContext context) => widget.builder(context, setState);
}

public class ColoredBox : SingleChildRenderObjectWidget
{
    public virtual Color color { get; private set; } = default!;
    public virtual bool isAntiAlias { get; private set; } = default!;

    public ColoredBox(Color color, bool isAntiAlias = true, Widget? child = null, Key? key = null) : base(child: child, key: key)
    {
        this.color = color;
        this.isAntiAlias = isAntiAlias;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderColoredBox__basic(color: color, isAntiAlias: isAntiAlias);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        DartRuntimePrimitives.Ignore(((Func<_RenderColoredBox__basic>)(() =>
{
    var __cascade = ((_RenderColoredBox__basic?)renderObject)!;
    __cascade.color = color;
    __cascade.isAntiAlias = isAntiAlias;
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Color>("color", color));
        properties.add(new DiagnosticsProperty<bool>("isAntiAlias", isAntiAlias, defaultValue: true));
    }

}

internal class _RenderColoredBox__basic : RenderProxyBoxWithHitTestBehavior
{
    internal virtual Color _color { get; set; } = default!;
    internal virtual bool _isAntiAlias { get; set; } = default!;

    internal _RenderColoredBox__basic(Color color, bool isAntiAlias) : base(behavior: HitTestBehavior.opaque)
    {
        _color = color;
        _isAntiAlias = isAntiAlias;
    }

    public virtual Color color
    {
        get => _color;
        set
        {
            var __value = value;
            if (Equals(__value, _color))
            {
                return;
            }
            _color = __value;
            markNeedsPaint();
        }
    }
    public virtual bool isAntiAlias
    {
        get => _isAntiAlias;
        set
        {
            var __value = value;
            if (__value == _isAntiAlias)
            {
                return;
            }
            _isAntiAlias = __value;
            markNeedsPaint();
        }
    }
    public override void paint(PaintingContext context, Offset offset)
    {
        if (size > Size.zero)
        {
            context.canvas.drawRect(offset & size, ((Func<Paint>)(() =>
{
    var __cascade = new Paint();
    __cascade.isAntiAlias = isAntiAlias;
    __cascade.color = color;
    return __cascade;
}))());
        }
        if (child is not null)
        {
            context.paintChild(child!, offset);
        }
    }

}
