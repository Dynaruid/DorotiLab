// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/basic.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

internal class _UbiquitousInheritedElement__basic : InheritedElement
{
    internal _UbiquitousInheritedElement__basic(InheritedWidget widget) : base(widget)
    {
    }

    public override void setDependencies(Element dependent, object? value)
    {
        DartRuntimePrimitives.Assert(() => (value is null));
    }

    public override object? getDependencies(Element dependent)
    {
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void notifyClients(ProxyWidget oldWidget)
    {
        var __oldWidget = (InheritedWidget)(object)oldWidget;
        _UbiquitousInheritedElement__basic._recurseChildren(this, ((global::System.Action<Element>)((element) => {
if (element.doesDependOnInheritedElement(this))
{
    notifyDependent(__oldWidget, element);
}
})));
    }

    internal static void _recurseChildren(Element element, global::System.Action<Element> visitor)
    {
        element.visitChildren(((global::System.Action<Element>)((child) => {
_UbiquitousInheritedElement__basic._recurseChildren(child, (global::System.Action<Element>)visitor);
})));
        visitor(element);
    }

}

public abstract class _UbiquitousInheritedWidget__basic : InheritedWidget
{
    internal _UbiquitousInheritedWidget__basic(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget child = default!) : base(key: key, child: child)
    {
    }

    public override InheritedElement createElement() => DartRuntimePrimitives.ConvertValue<InheritedElement>(new _UbiquitousInheritedElement__basic(this));
}

public class Directionality : _UbiquitousInheritedWidget__basic
{
    public virtual TextDirection textDirection { get; private set; } = default!;

    public Directionality(global::Doroti.Generated.Framework.Foundation.Key? key = null, TextDirection textDirection = default!, Widget child = default!) : base(key: key, child: child)
    {
        this.textDirection = textDirection;
    }

    public static global::Doroti.Ui.TextDirection of(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        Directionality widget__6574 = context.dependOnInheritedWidgetOfExactType<Directionality>()!;
        return ((Directionality)widget__6574).textDirection;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Ui.TextDirection? maybeOf(BuildContext context)
    {
        Directionality? widget__7263 = ((Directionality?)(object?)context.dependOnInheritedWidgetOfExactType<Directionality>());
        return ((TextDirection?)((dynamic)widget__7263)?.textDirection);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.textDirection, ((Directionality)oldWidget).textDirection)));
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this.textDirection));
    }

}

public class Opacity : SingleChildRenderObjectWidget
{
    public virtual double opacity { get; private set; } = default!;
    public virtual bool alwaysIncludeSemantics { get; private set; } = default!;

    public Opacity(global::Doroti.Generated.Framework.Foundation.Key? key = null, double opacity = default!, bool alwaysIncludeSemantics = false, Widget? child = null) : base(key: key, child: child)
    {
        this.opacity = opacity;
        this.alwaysIncludeSemantics = alwaysIncludeSemantics;
        System.Diagnostics.Debug.Assert(((opacity >= 0.0) && (opacity <= 1.0)));
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderOpacity(opacity: this.opacity, alwaysIncludeSemantics: this.alwaysIncludeSemantics));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderOpacity)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderOpacity>)(() =>
{            var __cascade = __renderObject;
            __cascade.opacity = this.opacity;
            __cascade.alwaysIncludeSemantics = this.alwaysIncludeSemantics;
            return __cascade;        }))());
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("opacity", this.opacity));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("alwaysIncludeSemantics", value: this.alwaysIncludeSemantics, ifTrue: "alwaysIncludeSemantics"));
    }

}

public class ShaderMask : SingleChildRenderObjectWidget
{
    public virtual global::System.Func<Rect, Shader> shaderCallback { get; private set; } = default!;
    public virtual BlendMode blendMode { get; private set; } = default!;

    public ShaderMask(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Func<Rect, Shader> shaderCallback = default!, BlendMode blendMode = BlendMode.modulate, Widget? child = null) : base(key: key, child: child)
    {
        this.shaderCallback = shaderCallback;
        this.blendMode = blendMode;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderShaderMask(shaderCallback: (global::System.Func<Rect, Shader>)this.shaderCallback, blendMode: this.blendMode));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderShaderMask)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderShaderMask>)(() =>
{            var __cascade = __renderObject;
            __cascade.shaderCallback = this.shaderCallback;
            __cascade.blendMode = this.blendMode;
            return __cascade;        }))());
    }

}

public class BackdropGroup : InheritedWidget
{
    public virtual global::Doroti.Generated.Framework.Rendering.BackdropKey backdropKey { get; private set; } = default!;

    public BackdropGroup(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget child = default!, global::Doroti.Generated.Framework.Rendering.BackdropKey? backdropKey = null) : base(key: key, child: child)
    {
        this.backdropKey = (backdropKey ?? new global::Doroti.Generated.Framework.Rendering.BackdropKey());
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (BackdropGroup)(object)oldWidget;
        return (!object.Equals(((BackdropGroup)__oldWidget).backdropKey, this.backdropKey));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static BackdropGroup? of(BuildContext context)
    {
        return ((BackdropGroup?)(object?)context.dependOnInheritedWidgetOfExactType<BackdropGroup>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class BackdropFilter : SingleChildRenderObjectWidget
{
    public virtual ImageFilter? filter { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.ImageFilterConfig? filterConfig { get; private set; }
    public virtual BlendMode blendMode { get; private set; } = default!;
    public virtual bool enabled { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.BackdropKey? backdropGroupKey { get; private set; }
    internal virtual bool _useSharedKey { get; private set; } = default!;

    public BackdropFilter(global::Doroti.Generated.Framework.Foundation.Key? key = null, ImageFilter? filter = null, global::Doroti.Generated.Framework.Rendering.ImageFilterConfig? filterConfig = null, Widget? child = null, BlendMode blendMode = BlendMode.srcOver, bool enabled = true, global::Doroti.Generated.Framework.Rendering.BackdropKey? backdropGroupKey = null) : base(key: key, child: child)
    {
        this.filter = filter;
        this.filterConfig = filterConfig;
        this.blendMode = blendMode;
        this.enabled = enabled;
        this.backdropGroupKey = backdropGroupKey;
        this._useSharedKey = false;
        System.Diagnostics.Debug.Assert(((filter is not null) || (filterConfig is not null)));
        System.Diagnostics.Debug.Assert(((filter is null) || (filterConfig is null)));
    }

    public static BackdropFilter CreateGrouped(global::Doroti.Generated.Framework.Foundation.Key? key = null, ImageFilter? filter = null, global::Doroti.Generated.Framework.Rendering.ImageFilterConfig? filterConfig = null, Widget? child = null, BlendMode blendMode = BlendMode.srcOver, bool enabled = true)
    {
        var __instance = new BackdropFilter(default!, default!, default!, default!, default!, default!, default!);
        __instance.filter = filter;
        __instance.filterConfig = filterConfig;
        __instance.blendMode = blendMode;
        __instance.enabled = enabled;
        __instance.backdropGroupKey = null;
        __instance._useSharedKey = true;
        return __instance;
    }

    internal virtual global::Doroti.Generated.Framework.Rendering.BackdropKey? _getBackdropGroupKey(BuildContext context)
    {
        if (this._useSharedKey)
        {
            return BackdropGroup.of(context)?.backdropKey;
        }
        return this.backdropGroupKey;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Rendering.ImageFilterConfig _effectiveFilterConfig
    {
        get
        {
            return (this.filterConfig ?? global::Doroti.Generated.Framework.Rendering.ImageFilterConfig.Create(this.filter!));
            return default!;
        }
    }
    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderBackdropFilter(filterConfig: this._effectiveFilterConfig, blendMode: this.blendMode, enabled: this.enabled, backdropKey: _getBackdropGroupKey(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderBackdropFilter)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderBackdropFilter>)(() =>
{            var __cascade = __renderObject;
            __cascade.filterConfig = this._effectiveFilterConfig;
            __cascade.enabled = this.enabled;
            __cascade.blendMode = this.blendMode;
            __cascade.backdropKey = _getBackdropGroupKey(context);
            return __cascade;        }))());
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.ImageFilter>("filter", this.filter, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Rendering.ImageFilterConfig>("filterConfig", this.filterConfig, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Ui.BlendMode>("blendMode", this.blendMode));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("enabled", value: this.enabled, ifTrue: "enabled"));
    }

}

public class CustomPaint : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Rendering.CustomPainter? painter { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.CustomPainter? foregroundPainter { get; private set; }
    public virtual Size size { get; private set; } = default!;
    public virtual bool isComplex { get; private set; } = default!;
    public virtual bool willChange { get; private set; } = default!;

    public CustomPaint(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Rendering.CustomPainter? painter = null, global::Doroti.Generated.Framework.Rendering.CustomPainter? foregroundPainter = null, Size size = default, bool isComplex = false, bool willChange = false, Widget? child = null) : base(key: key, child: child)
    {
        this.painter = painter;
        this.foregroundPainter = foregroundPainter;
        this.size = size ?? Size.zero;
        this.isComplex = isComplex;
        this.willChange = willChange;
        System.Diagnostics.Debug.Assert((((painter is not null) || (foregroundPainter is not null)) || ((!isComplex && !willChange))));
    }

    public CustomPaint(Size size, ToggleablePainter painter) : this(size: size, painter: (global::Doroti.Generated.Framework.Rendering.CustomPainter?)null)
    {
        _ = painter;
    }

    public CustomPaint(global::Doroti.Generated.Framework.Foundation.Key? key = null, object? painter = null, object? foregroundPainter = null, Size size = default, bool isComplex = false, bool willChange = false, Widget? child = null) : this(
        key: key,
        painter: painter as global::Doroti.Generated.Framework.Rendering.CustomPainter,
        foregroundPainter: foregroundPainter as global::Doroti.Generated.Framework.Rendering.CustomPainter,
        size: size,
        isComplex: isComplex,
        willChange: willChange,
        child: child)
    {
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderCustomPaint(painter: this.painter, foregroundPainter: this.foregroundPainter, preferredSize: this.size, isComplex: this.isComplex, willChange: this.willChange));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderCustomPaint)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderCustomPaint>)(() =>
{            var __cascade = __renderObject;
            __cascade.painter = this.painter;
            __cascade.foregroundPainter = this.foregroundPainter;
            __cascade.preferredSize = this.size;
            __cascade.isComplex = this.isComplex;
            __cascade.willChange = this.willChange;
            return __cascade;        }))());
    }

    public override void didUnmountRenderObject(global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderCustomPaint)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderCustomPaint>)(() =>
{            var __cascade = __renderObject;
            __cascade.painter = null;
            __cascade.foregroundPainter = null;
            return __cascade;        }))());
    }

}

public class ClipRect : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Rendering.CustomClipper<Rect>? clipper { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;

    public ClipRect(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Rendering.CustomClipper<Rect>? clipper = null, Clip clipBehavior = Clip.hardEdge, Widget? child = null) : base(key: key, child: child)
    {
        this.clipper = clipper;
        this.clipBehavior = clipBehavior;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderClipRect(clipper: this.clipper, clipBehavior: this.clipBehavior));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderClipRect)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderClipRect>)(() =>
{            var __cascade = __renderObject;
            __cascade.clipper = this.clipper;
            __cascade.clipBehavior = this.clipBehavior;
            return __cascade;        }))());
    }

    public override void didUnmountRenderObject(global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderClipRect)(object)renderObject;
        __renderObject.clipper = null;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Rendering.CustomClipper<global::Doroti.Ui.Rect>>("clipper", this.clipper, defaultValue: null));
    }

}

public class ClipRRect : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry borderRadius { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.CustomClipper<RRect>? clipper { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;

    public ClipRRect(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry borderRadius = default!, global::Doroti.Generated.Framework.Rendering.CustomClipper<RRect>? clipper = null, Clip clipBehavior = Clip.antiAlias, Widget? child = null) : base(key: key, child: child)
    {
        global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry __borderRadius = borderRadius ?? global::Doroti.Generated.Framework.Painting.BorderRadius.zero;
        this.borderRadius = __borderRadius;
        this.clipper = clipper;
        this.clipBehavior = clipBehavior;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderClipRRect(borderRadius: this.borderRadius, clipper: this.clipper, clipBehavior: this.clipBehavior, textDirection: Directionality.maybeOf(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderClipRRect)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderClipRRect>)(() =>
{            var __cascade = __renderObject;
            __cascade.borderRadius = this.borderRadius;
            __cascade.clipBehavior = this.clipBehavior;
            __cascade.clipper = this.clipper;
            __cascade.textDirection = Directionality.maybeOf(context);
            return __cascade;        }))());
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry>("borderRadius", this.borderRadius, showName: false, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Rendering.CustomClipper<global::Doroti.Ui.RRect>>("clipper", this.clipper, defaultValue: null));
    }

}

public class ClipRSuperellipse : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry borderRadius { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.CustomClipper<RSuperellipse>? clipper { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;

    public ClipRSuperellipse(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry borderRadius = default!, global::Doroti.Generated.Framework.Rendering.CustomClipper<RSuperellipse>? clipper = null, Clip clipBehavior = Clip.antiAlias, Widget? child = null) : base(key: key, child: child)
    {
        global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry __borderRadius = borderRadius ?? global::Doroti.Generated.Framework.Painting.BorderRadius.zero;
        this.borderRadius = __borderRadius;
        this.clipper = clipper;
        this.clipBehavior = clipBehavior;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderClipRSuperellipse(borderRadius: this.borderRadius, clipBehavior: this.clipBehavior, clipper: this.clipper, textDirection: Directionality.maybeOf(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderClipRSuperellipse)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderClipRSuperellipse>)(() =>
{            var __cascade = __renderObject;
            __cascade.borderRadius = this.borderRadius;
            __cascade.clipBehavior = this.clipBehavior;
            __cascade.clipper = this.clipper;
            __cascade.textDirection = Directionality.maybeOf(context);
            return __cascade;        }))());
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry>("borderRadius", this.borderRadius, showName: false, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Rendering.CustomClipper<global::Doroti.Ui.RSuperellipse>>("clipper", this.clipper, defaultValue: null));
    }

}

public class ClipOval : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Rendering.CustomClipper<Rect>? clipper { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;

    public ClipOval(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Rendering.CustomClipper<Rect>? clipper = null, Clip clipBehavior = Clip.antiAlias, Widget? child = null) : base(key: key, child: child)
    {
        this.clipper = clipper;
        this.clipBehavior = clipBehavior;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderClipOval(clipper: this.clipper, clipBehavior: this.clipBehavior));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderClipOval)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderClipOval>)(() =>
{            var __cascade = __renderObject;
            __cascade.clipper = this.clipper;
            __cascade.clipBehavior = this.clipBehavior;
            return __cascade;        }))());
    }

    public override void didUnmountRenderObject(global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderClipOval)(object)renderObject;
        __renderObject.clipper = null;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Rendering.CustomClipper<global::Doroti.Ui.Rect>>("clipper", this.clipper, defaultValue: null));
    }

}

public class ClipPath : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Rendering.CustomClipper<Path>? clipper { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;

    public ClipPath(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Rendering.CustomClipper<Path>? clipper = null, Clip clipBehavior = Clip.antiAlias, Widget? child = null) : base(key: key, child: child)
    {
        this.clipper = clipper;
        this.clipBehavior = clipBehavior;
    }

    public static Widget shape(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.ShapeBorder shape = default!, Clip clipBehavior = Clip.antiAlias, Widget? child = null)
    {
        return ((Widget)(object?)new Builder(key: key, builder: ((global::System.Func<BuildContext, Widget>)((context) => {
return ((Widget)(object?)new ClipPath(clipper: new global::Doroti.Generated.Framework.Rendering.ShapeBorderClipper(shape: shape, textDirection: Directionality.maybeOf(context)), clipBehavior: clipBehavior, child: child));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderClipPath(clipper: this.clipper, clipBehavior: this.clipBehavior));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderClipPath)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderClipPath>)(() =>
{            var __cascade = __renderObject;
            __cascade.clipper = this.clipper;
            __cascade.clipBehavior = this.clipBehavior;
            return __cascade;        }))());
    }

    public override void didUnmountRenderObject(global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderClipPath)(object)renderObject;
        __renderObject.clipper = null;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Rendering.CustomClipper<global::Doroti.Ui.Path>>("clipper", this.clipper, defaultValue: null));
    }

}

public class PhysicalModel : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Painting.BoxShape shape { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius { get; private set; }
    public virtual double elevation { get; private set; } = default!;
    public virtual Color color { get; private set; } = default!;
    public virtual Color shadowColor { get; private set; } = default!;

    public PhysicalModel(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.BoxShape shape = global::Doroti.Generated.Framework.Painting.BoxShape.rectangle, Clip clipBehavior = Clip.none, global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius = null, double elevation = 0.0, Color color = default!, Color shadowColor = default!, Widget? child = null) : base(key: key, child: child)
    {
        Color __shadowColor = shadowColor ?? new Color(0xFF000000);
        this.shape = shape;
        this.clipBehavior = clipBehavior;
        this.borderRadius = borderRadius;
        this.elevation = elevation;
        this.color = color;
        this.shadowColor = __shadowColor;
        System.Diagnostics.Debug.Assert((elevation >= 0.0));
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderPhysicalModel(shape: this.shape, clipBehavior: this.clipBehavior, borderRadius: this.borderRadius, elevation: this.elevation, color: this.color, shadowColor: this.shadowColor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderPhysicalModel)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderPhysicalModel>)(() =>
{            var __cascade = __renderObject;
            __cascade.shape = this.shape;
            __cascade.clipBehavior = this.clipBehavior;
            __cascade.borderRadius = this.borderRadius;
            __cascade.elevation = this.elevation;
            __cascade.color = this.color;
            __cascade.shadowColor = this.shadowColor;
            return __cascade;        }))());
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Painting.BoxShape>("shape", this.shape));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.BorderRadius>("borderRadius", this.borderRadius));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("elevation", this.elevation));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("color", this.color));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("shadowColor", this.shadowColor));
    }

}

public class PhysicalShape : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Rendering.CustomClipper<Path> clipper { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual double elevation { get; private set; } = default!;
    public virtual Color color { get; private set; } = default!;
    public virtual Color shadowColor { get; private set; } = default!;

    public PhysicalShape(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Rendering.CustomClipper<Path> clipper = default!, Clip clipBehavior = Clip.none, double elevation = 0.0, Color color = default!, Color shadowColor = default!, Widget? child = null) : base(key: key, child: child)
    {
        Color __shadowColor = shadowColor ?? new Color(0xFF000000);
        this.clipper = clipper;
        this.clipBehavior = clipBehavior;
        this.elevation = elevation;
        this.color = color;
        this.shadowColor = __shadowColor;
        System.Diagnostics.Debug.Assert((elevation >= 0.0));
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderPhysicalShape(clipper: this.clipper, clipBehavior: this.clipBehavior, elevation: this.elevation, color: this.color, shadowColor: this.shadowColor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderPhysicalShape)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderPhysicalShape>)(() =>
{            var __cascade = __renderObject;
            __cascade.clipper = this.clipper;
            __cascade.clipBehavior = this.clipBehavior;
            __cascade.elevation = this.elevation;
            __cascade.color = this.color;
            __cascade.shadowColor = this.shadowColor;
            return __cascade;        }))());
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Rendering.CustomClipper<global::Doroti.Ui.Path>>("clipper", this.clipper));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("elevation", this.elevation));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("color", this.color));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("shadowColor", this.shadowColor));
    }

}

public class Transform : SingleChildRenderObjectWidget
{
    public virtual Matrix4 transform { get; private set; } = default!;
    public virtual Offset? origin { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment { get; private set; }
    public virtual bool transformHitTests { get; private set; } = default!;
    public virtual FilterQuality? filterQuality { get; private set; }

    public Transform(global::Doroti.Generated.Framework.Foundation.Key? key = null, Matrix4 transform = default!, Offset? origin = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment = null, bool transformHitTests = true, FilterQuality? filterQuality = null, Widget? child = null) : base(key: key, child: child)
    {
        this.transform = transform;
        this.origin = origin;
        this.alignment = alignment;
        this.transformHitTests = transformHitTests;
        this.filterQuality = filterQuality;
    }

    public static Transform CreateRotate(global::Doroti.Generated.Framework.Foundation.Key? key = null, double angle = default!, Offset? origin = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment = default!, bool transformHitTests = true, FilterQuality? filterQuality = null, Widget? child = null)
    {
        var __instance = new Transform(default!, default!, default!, default!, default!, default!, default!);
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry? __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center;
        __instance.origin = origin;
        __instance.alignment = __alignment;
        __instance.transformHitTests = transformHitTests;
        __instance.filterQuality = filterQuality;
        __instance.transform = Transform._computeRotation(angle);
        return __instance;
    }

    public static Transform CreateTranslate(global::Doroti.Generated.Framework.Foundation.Key? key = null, Offset offset = default!, bool transformHitTests = true, FilterQuality? filterQuality = null, Widget? child = null)
    {
        var __instance = new Transform(default!, default!, default!, default!, default!, default!, default!);
        __instance.transformHitTests = transformHitTests;
        __instance.filterQuality = filterQuality;
        __instance.transform = Matrix4.translationValues(offset.dx, offset.dy, 0.0);
        __instance.origin = null;
        __instance.alignment = null;
        return __instance;
    }

    public static Transform CreateScale(global::Doroti.Generated.Framework.Foundation.Key? key = null, double? scale = null, double? scaleX = null, double? scaleY = null, Offset? origin = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment = default!, bool transformHitTests = true, FilterQuality? filterQuality = null, Widget? child = null)
    {
        var __instance = new Transform(default!, default!, default!, default!, default!, default!, default!);
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry? __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center;
        __instance.origin = origin;
        __instance.alignment = __alignment;
        __instance.transformHitTests = transformHitTests;
        __instance.filterQuality = filterQuality;
        __instance.transform = Matrix4.diagonal3Values(((scale ?? scaleX) ?? 1.0), ((scale ?? scaleY) ?? 1.0), 1.0);
        return __instance;
    }

    public static Transform CreateFlip(global::Doroti.Generated.Framework.Foundation.Key? key = null, bool flipX = false, bool flipY = false, Offset? origin = null, bool transformHitTests = true, FilterQuality? filterQuality = null, Widget? child = null)
    {
        var __instance = new Transform(default!, default!, default!, default!, default!, default!, default!);
        __instance.origin = origin;
        __instance.transformHitTests = transformHitTests;
        __instance.filterQuality = filterQuality;
        __instance.alignment = global::Doroti.Generated.Framework.Painting.Alignment.center;
        __instance.transform = Matrix4.diagonal3Values((flipX ? -1.0 : 1.0), (flipY ? -1.0 : 1.0), 1.0);
        return __instance;
    }

    internal static Matrix4 _computeRotation(double radians)
    {
        DartRuntimePrimitives.Assert(() => double.IsFinite(radians), () => (object?)$"Cannot compute the rotation matrix for a non-finite angle: {radians}");
        if ((radians == 0.0))
        {
            return Matrix4.identity();
        }
        double sin__59610 = global::Doroti.Runtime.Dart_mathLibrary.sin(radians);
        if ((sin__59610 == 1.0))
        {
            return ((Matrix4)(object?)Transform._createZRotation(1.0, 0.0));
        }
        if ((sin__59610 == -1.0))
        {
            return ((Matrix4)(object?)Transform._createZRotation(-1.0, 0.0));
        }
        double cos__59792 = global::Doroti.Runtime.Dart_mathLibrary.cos(radians);
        if ((cos__59792 == -1.0))
        {
            return ((Matrix4)(object?)Transform._createZRotation(0.0, -1.0));
        }
        return ((Matrix4)(object?)Transform._createZRotation(sin__59610, cos__59792));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static Matrix4 _createZRotation(double sin, double cos)
    {
        var result__60002 = Matrix4.zero();
        result__60002.storage[0L] = cos;
        result__60002.storage[1L] = sin;
        result__60002.storage[4L] = -sin;
        result__60002.storage[5L] = cos;
        result__60002.storage[10L] = 1.0;
        result__60002.storage[15L] = 1.0;
        return result__60002;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderTransform(transform: this.transform, origin: this.origin, alignment: this.alignment, textDirection: Directionality.maybeOf(context), transformHitTests: this.transformHitTests, filterQuality: this.filterQuality));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderTransform)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderTransform>)(() =>
{            var __cascade = __renderObject;
            __cascade.transform = this.transform;
            __cascade.origin = this.origin;
            __cascade.alignment = this.alignment;
            __cascade.textDirection = Directionality.maybeOf(context);
            __cascade.transformHitTests = this.transformHitTests;
            __cascade.filterQuality = this.filterQuality;
            return __cascade;        }))());
    }

}

public class CompositedTransformTarget : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Rendering.LayerLink link { get; private set; } = default!;

    public CompositedTransformTarget(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Rendering.LayerLink link = default!, Widget? child = null) : base(key: key, child: child)
    {
        this.link = link;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderLeaderLayer(link: this.link));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderLeaderLayer)(object)renderObject;
        __renderObject.link = this.link;
    }

}

public class CompositedTransformFollower : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Rendering.LayerLink link { get; private set; } = default!;
    public virtual bool showWhenUnlinked { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.Alignment targetAnchor { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.Alignment followerAnchor { get; private set; } = default!;
    public virtual Offset offset { get; private set; } = default!;

    public CompositedTransformFollower(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Rendering.LayerLink link = default!, bool showWhenUnlinked = true, Offset offset = default, global::Doroti.Generated.Framework.Painting.Alignment targetAnchor = default!, global::Doroti.Generated.Framework.Painting.Alignment followerAnchor = default!, Widget? child = null) : base(key: key, child: child)
    {
        global::Doroti.Generated.Framework.Painting.Alignment __targetAnchor = targetAnchor ?? global::Doroti.Generated.Framework.Painting.Alignment.topLeft;
        global::Doroti.Generated.Framework.Painting.Alignment __followerAnchor = followerAnchor ?? global::Doroti.Generated.Framework.Painting.Alignment.topLeft;
        this.link = link;
        this.showWhenUnlinked = showWhenUnlinked;
        this.offset = offset;
        this.targetAnchor = __targetAnchor;
        this.followerAnchor = __followerAnchor;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderFollowerLayer(link: this.link, showWhenUnlinked: this.showWhenUnlinked, offset: this.offset, leaderAnchor: this.targetAnchor, followerAnchor: this.followerAnchor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderFollowerLayer)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderFollowerLayer>)(() =>
{            var __cascade = __renderObject;
            __cascade.link = this.link;
            __cascade.showWhenUnlinked = this.showWhenUnlinked;
            __cascade.offset = this.offset;
            __cascade.leaderAnchor = this.targetAnchor;
            __cascade.followerAnchor = this.followerAnchor;
            return __cascade;        }))());
    }

}

public class FittedBox : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Painting.BoxFit fit { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;

    public FittedBox(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.BoxFit fit = global::Doroti.Generated.Framework.Painting.BoxFit.contain, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, Clip clipBehavior = Clip.none, Widget? child = null) : base(key: key, child: child)
    {
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center;
        this.fit = fit;
        this.alignment = __alignment;
        this.clipBehavior = clipBehavior;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderFittedBox(fit: this.fit, alignment: this.alignment, textDirection: Directionality.maybeOf(context), clipBehavior: this.clipBehavior));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderFittedBox)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderFittedBox>)(() =>
{            var __cascade = __renderObject;
            __cascade.fit = this.fit;
            __cascade.alignment = this.alignment;
            __cascade.textDirection = Directionality.maybeOf(context);
            __cascade.clipBehavior = this.clipBehavior;
            return __cascade;        }))());
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Painting.BoxFit>("fit", this.fit));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.AlignmentGeometry>("alignment", this.alignment));
    }

}

public class FractionalTranslation : SingleChildRenderObjectWidget
{
    public virtual Offset translation { get; private set; } = default!;
    public virtual bool transformHitTests { get; private set; } = default!;

    public FractionalTranslation(global::Doroti.Generated.Framework.Foundation.Key? key = null, Offset translation = default!, bool transformHitTests = true, Widget? child = null) : base(key: key, child: child)
    {
        this.translation = translation;
        this.transformHitTests = transformHitTests;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderFractionalTranslation(translation: this.translation, transformHitTests: this.transformHitTests));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderFractionalTranslation)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderFractionalTranslation>)(() =>
{            var __cascade = __renderObject;
            __cascade.translation = this.translation;
            __cascade.transformHitTests = this.transformHitTests;
            return __cascade;        }))());
    }

}

public class RotatedBox : SingleChildRenderObjectWidget
{
    public virtual long quarterTurns { get; private set; } = default!;

    public RotatedBox(global::Doroti.Generated.Framework.Foundation.Key? key = null, long quarterTurns = default!, Widget? child = null) : base(key: key, child: child)
    {
        this.quarterTurns = quarterTurns;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(new global::Doroti.Generated.Framework.Rendering.RenderRotatedBox(quarterTurns: this.quarterTurns));
    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderRotatedBox)(object)renderObject;
        __renderObject.quarterTurns = this.quarterTurns;
    }

}

public class Padding : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding { get; private set; } = default!;

    public Padding(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding = default!, Widget? child = null) : base(key: key, child: child)
    {
        this.padding = padding;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderPadding(padding: this.padding, textDirection: Directionality.maybeOf(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderPadding)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderPadding>)(() =>
{            var __cascade = __renderObject;
            __cascade.padding = this.padding;
            __cascade.textDirection = Directionality.maybeOf(context);
            return __cascade;        }))());
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>("padding", this.padding));
    }

}

public class Align : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;
    public virtual double? widthFactor { get; private set; }
    public virtual double? heightFactor { get; private set; }

    public Align(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, double? widthFactor = null, double? heightFactor = null, Widget? child = null) : base(key: key, child: child)
    {
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center;
        this.alignment = __alignment;
        this.widthFactor = widthFactor;
        this.heightFactor = heightFactor;
        System.Diagnostics.Debug.Assert(((widthFactor is null) || (widthFactor >= 0.0)));
        System.Diagnostics.Debug.Assert(((heightFactor is null) || (heightFactor >= 0.0)));
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderPositionedBox(alignment: this.alignment, widthFactor: this.widthFactor, heightFactor: this.heightFactor, textDirection: Directionality.maybeOf(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderPositionedBox)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderPositionedBox>)(() =>
{            var __cascade = __renderObject;
            __cascade.alignment = this.alignment;
            __cascade.widthFactor = this.widthFactor;
            __cascade.heightFactor = this.heightFactor;
            __cascade.textDirection = Directionality.maybeOf(context);
            return __cascade;        }))());
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.AlignmentGeometry>("alignment", this.alignment));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("widthFactor", this.widthFactor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("heightFactor", this.heightFactor, defaultValue: null));
    }

}

public class Center : Align
{
    public Center(global::Doroti.Generated.Framework.Foundation.Key? key = null, double? widthFactor = null, double? heightFactor = null, Widget? child = null) : base(key: key, widthFactor: widthFactor, heightFactor: heightFactor, child: child)
    {
    }

}

public class CustomSingleChildLayout : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Rendering.SingleChildLayoutDelegate @delegate { get; private set; } = default!;

    public CustomSingleChildLayout(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Rendering.SingleChildLayoutDelegate @delegate = default!, Widget? child = null) : base(key: key, child: child)
    {
        this.@delegate = @delegate;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderCustomSingleChildLayoutBox(@delegate: this.@delegate));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderCustomSingleChildLayoutBox)(object)renderObject;
        __renderObject.@delegate = this.@delegate;
    }

}

public class LayoutId : ParentDataWidget<global::Doroti.Generated.Framework.Rendering.MultiChildLayoutParentData>
{
    public virtual object id { get; private set; } = default!;

    public LayoutId(global::Doroti.Generated.Framework.Foundation.Key? key = null, object id = default!, Widget child = default!) : base(child: child, key: (key ?? new global::Doroti.Generated.Framework.Foundation.ValueKey<object>(id)))
    {
        this.id = id;
    }

    public override void applyParentData(global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Generated.Framework.Rendering.RenderObject)renderObject).parentData is global::Doroti.Generated.Framework.Rendering.MultiChildLayoutParentData));
        var parentData__92727 = ((global::Doroti.Generated.Framework.Rendering.MultiChildLayoutParentData?)(object?)((global::Doroti.Generated.Framework.Rendering.RenderObject)renderObject).parentData!)!;
        if ((!object.Equals(((global::Doroti.Generated.Framework.Rendering.MultiChildLayoutParentData)parentData__92727).id, this.id)))
        {
            parentData__92727.id = this.id;
            ((dynamic)((global::Doroti.Generated.Framework.Rendering.RenderObject)renderObject).parent)?.markNeedsLayout();
        }
    }

    public override Type debugTypicalAncestorWidgetClass => typeof(CustomMultiChildLayout);
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<object>("id", this.id));
    }

}

public class CustomMultiChildLayout : MultiChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Rendering.MultiChildLayoutDelegate @delegate { get; private set; } = default!;

    public CustomMultiChildLayout(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Rendering.MultiChildLayoutDelegate @delegate = default!, List<Widget> children = default!) : base(key: key, children: children ?? new List<Widget>())
    {
        this.@delegate = @delegate;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderCustomMultiChildLayoutBox(@delegate: this.@delegate));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderCustomMultiChildLayoutBox)(object)renderObject;
        __renderObject.@delegate = this.@delegate;
    }

}

public class SizedBox : SingleChildRenderObjectWidget
{
    public virtual double? width { get; private set; }
    public virtual double? height { get; private set; }

    public SizedBox(global::Doroti.Generated.Framework.Foundation.Key? key = null, double? width = null, double? height = null, Widget? child = null) : base(key: key, child: child)
    {
        this.width = width;
        this.height = height;
    }

    public static SizedBox CreateExpand(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget? child = null)
    {
        var __instance = new SizedBox(default!, default!, default!, default!);
        __instance.width = double.PositiveInfinity;
        __instance.height = double.PositiveInfinity;
        return __instance;
    }

    public static SizedBox CreateShrink(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget? child = null)
    {
        var __instance = new SizedBox(default!, default!, default!, default!);
        __instance.width = 0.0;
        __instance.height = 0.0;
        return __instance;
    }

    public static SizedBox CreateFromSize(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget? child = null, Size? size = null)
    {
        var __instance = new SizedBox(default!, default!, default!, default!);
        __instance.width = size?.width;
        __instance.height = size?.height;
        return __instance;
    }

    public static SizedBox CreateSquare(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget? child = null, double? dimension = null)
    {
        var __instance = new SizedBox(default!, default!, default!, default!);
        __instance.width = dimension;
        __instance.height = dimension;
        return __instance;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderConstrainedBox(additionalConstraints: this._additionalConstraints));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints _additionalConstraints
    {
        get
        {
            return global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateTightFor(width: this.width, height: this.height);
            return default!;
        }
    }
    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderConstrainedBox)(object)renderObject;
        __renderObject.additionalConstraints = this._additionalConstraints;
    }

    public override string toStringShort()
    {
        string type__100066 = ((this.width, this.height) switch { (var __constant100107, var __constant100124) when (object.Equals(__constant100107, double.PositiveInfinity)) && (object.Equals(__constant100124, double.PositiveInfinity)) => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "SizedBox"))}.expand", (0.0, 0.0) => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "SizedBox"))}.shrink", _ => global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "SizedBox") });
        return ((this.key is null) ? type__100066 : $"{type__100066}-{this.key}");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        global::Doroti.Generated.Framework.Foundation.DiagnosticLevel level__100518 = default!;
        if (((((this.width == double.PositiveInfinity) && (this.height == double.PositiveInfinity))) || (((this.width == 0.0) && (this.height == 0.0)))))
        {
            level__100518 = global::Doroti.Generated.Framework.Foundation.DiagnosticLevel.hidden;
        }
        else
        {
            level__100518 = global::Doroti.Generated.Framework.Foundation.DiagnosticLevel.info;
        }
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("width", this.width, defaultValue: null, level: level__100518));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("height", this.height, defaultValue: null, level: level__100518));
    }

}

public class ConstrainedBox : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints { get; private set; } = default!;

    public ConstrainedBox(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints = default!, Widget? child = null) : base(key: key, child: child)
    {
        this.constraints = constraints;
        System.Diagnostics.Debug.Assert(constraints.debugAssertIsValid());
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderConstrainedBox(additionalConstraints: this.constraints));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderConstrainedBox)(object)renderObject;
        __renderObject.additionalConstraints = this.constraints;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Rendering.BoxConstraints>("constraints", this.constraints, showName: false));
    }

}

public class ConstraintsTransformBox : SingleChildRenderObjectWidget
{
    internal static DartMap<global::System.Func<global::Doroti.Generated.Framework.Rendering.BoxConstraints, global::Doroti.Generated.Framework.Rendering.BoxConstraints>, string> _debugKnownTransforms = new DartMap<global::System.Func<global::Doroti.Generated.Framework.Rendering.BoxConstraints, global::Doroti.Generated.Framework.Rendering.BoxConstraints>, string> { [unmodified] = "unmodified", [unconstrained] = "unconstrained", [widthUnconstrained] = "width constraints removed", [heightUnconstrained] = "height constraints removed", [maxWidthUnconstrained] = "maxWidth constraint removed", [maxHeightUnconstrained] = "maxHeight constraint removed", [maxUnconstrained] = "maxWidth & maxHeight constraints removed" };
    public virtual TextDirection? textDirection { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Generated.Framework.Rendering.BoxConstraints, global::Doroti.Generated.Framework.Rendering.BoxConstraints> constraintsTransform { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    internal virtual string _debugTransformLabel { get; private set; } = default!;

    public ConstraintsTransformBox(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget? child = null, TextDirection? textDirection = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, global::System.Func<global::Doroti.Generated.Framework.Rendering.BoxConstraints, global::Doroti.Generated.Framework.Rendering.BoxConstraints> constraintsTransform = default!, Clip clipBehavior = Clip.none, string debugTransformType = "") : base(key: key, child: child)
    {
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center;
        this.textDirection = textDirection;
        this.alignment = __alignment;
        this.constraintsTransform = constraintsTransform;
        this.clipBehavior = clipBehavior;
        this._debugTransformLabel = debugTransformType;
    }

    public static global::Doroti.Generated.Framework.Rendering.BoxConstraints unmodified(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints) => constraints;
    public static global::Doroti.Generated.Framework.Rendering.BoxConstraints unconstrained(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints) => new global::Doroti.Generated.Framework.Rendering.BoxConstraints();
    public static global::Doroti.Generated.Framework.Rendering.BoxConstraints widthUnconstrained(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints) => constraints.heightConstraints();
    public static global::Doroti.Generated.Framework.Rendering.BoxConstraints heightUnconstrained(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints) => constraints.widthConstraints();
    public static global::Doroti.Generated.Framework.Rendering.BoxConstraints maxHeightUnconstrained(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints) => constraints.copyWith(maxHeight: double.PositiveInfinity);
    public static global::Doroti.Generated.Framework.Rendering.BoxConstraints maxWidthUnconstrained(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints) => constraints.copyWith(maxWidth: double.PositiveInfinity);
    public static global::Doroti.Generated.Framework.Rendering.BoxConstraints maxUnconstrained(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints) => constraints.copyWith(maxWidth: double.PositiveInfinity, maxHeight: double.PositiveInfinity);
    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderConstraintsTransformBox(textDirection: ((this.textDirection ?? (TextDirection)Directionality.maybeOf(context))), alignment: this.alignment, constraintsTransform: (global::System.Func<global::Doroti.Generated.Framework.Rendering.BoxConstraints, global::Doroti.Generated.Framework.Rendering.BoxConstraints>)this.constraintsTransform, clipBehavior: this.clipBehavior));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderConstraintsTransformBox)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderConstraintsTransformBox>)(() =>
{            var __cascade = __renderObject;
            __cascade.textDirection = ((this.textDirection ?? (TextDirection)Directionality.maybeOf(context)));
            __cascade.constraintsTransform = this.constraintsTransform;
            __cascade.alignment = this.alignment;
            __cascade.clipBehavior = this.clipBehavior;
            return __cascade;        }))());
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.AlignmentGeometry>("alignment", this.alignment));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
        string? debugTransformLabel__112556 = ((this._debugTransformLabel.Length != 0) ? this._debugTransformLabel : _debugKnownTransforms.GetValueOrDefault(this.constraintsTransform));
        if ((debugTransformLabel__112556 is not null))
        {
            properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<string>("constraints transform", debugTransformLabel__112556));
        }
    }

}

public class UnconstrainedBox : StatelessWidget
{
    public virtual TextDirection? textDirection { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.Axis? constrainedAxis { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual Widget? child { get; private set; }

    public UnconstrainedBox(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget? child = null, TextDirection? textDirection = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Generated.Framework.Painting.Axis? constrainedAxis = null, Clip clipBehavior = Clip.none) : base(key: key)
    {
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center;
        this.child = child;
        this.textDirection = textDirection;
        this.alignment = __alignment;
        this.constrainedAxis = constrainedAxis;
        this.clipBehavior = clipBehavior;
    }

    internal virtual global::System.Func<global::Doroti.Generated.Framework.Rendering.BoxConstraints, global::Doroti.Generated.Framework.Rendering.BoxConstraints> _axisToTransform(global::Doroti.Generated.Framework.Painting.Axis? constrainedAxis)
    {
        return ((global::System.Func<global::Doroti.Generated.Framework.Rendering.BoxConstraints, global::Doroti.Generated.Framework.Rendering.BoxConstraints>)(constrainedAxis switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => ConstraintsTransformBox.heightUnconstrained, global::Doroti.Generated.Framework.Painting.Axis.vertical => ConstraintsTransformBox.widthUnconstrained, null => ConstraintsTransformBox.unconstrained, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new ConstraintsTransformBox(textDirection: this.textDirection, alignment: this.alignment, clipBehavior: this.clipBehavior, constraintsTransform: _axisToTransform(this.constrainedAxis), child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.AlignmentGeometry>("alignment", this.alignment));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Painting.Axis>("constrainedAxis", this.constrainedAxis, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
    }

}

public class FractionallySizedBox : SingleChildRenderObjectWidget
{
    public virtual double? widthFactor { get; private set; }
    public virtual double? heightFactor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;

    public FractionallySizedBox(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, double? widthFactor = null, double? heightFactor = null, Widget? child = null) : base(key: key, child: child)
    {
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center;
        this.alignment = __alignment;
        this.widthFactor = widthFactor;
        this.heightFactor = heightFactor;
        System.Diagnostics.Debug.Assert(((widthFactor is null) || (widthFactor >= 0.0)));
        System.Diagnostics.Debug.Assert(((heightFactor is null) || (heightFactor >= 0.0)));
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderFractionallySizedOverflowBox(alignment: this.alignment, widthFactor: this.widthFactor, heightFactor: this.heightFactor, textDirection: Directionality.maybeOf(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderFractionallySizedOverflowBox)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderFractionallySizedOverflowBox>)(() =>
{            var __cascade = __renderObject;
            __cascade.alignment = this.alignment;
            __cascade.widthFactor = this.widthFactor;
            __cascade.heightFactor = this.heightFactor;
            __cascade.textDirection = Directionality.maybeOf(context);
            return __cascade;        }))());
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.AlignmentGeometry>("alignment", this.alignment));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("widthFactor", this.widthFactor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("heightFactor", this.heightFactor, defaultValue: null));
    }

}

public class LimitedBox : SingleChildRenderObjectWidget
{
    public virtual double maxWidth { get; private set; } = default!;
    public virtual double maxHeight { get; private set; } = default!;

    public LimitedBox(global::Doroti.Generated.Framework.Foundation.Key? key = null, double maxWidth = double.PositiveInfinity, double maxHeight = double.PositiveInfinity, Widget? child = null) : base(key: key, child: child)
    {
        this.maxWidth = maxWidth;
        this.maxHeight = maxHeight;
        System.Diagnostics.Debug.Assert((maxWidth >= 0.0));
        System.Diagnostics.Debug.Assert((maxHeight >= 0.0));
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderLimitedBox(maxWidth: this.maxWidth, maxHeight: this.maxHeight));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderLimitedBox)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderLimitedBox>)(() =>
{            var __cascade = __renderObject;
            __cascade.maxWidth = this.maxWidth;
            __cascade.maxHeight = this.maxHeight;
            return __cascade;        }))());
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("maxWidth", this.maxWidth, defaultValue: double.PositiveInfinity));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("maxHeight", this.maxHeight, defaultValue: double.PositiveInfinity));
    }

}

public class OverflowBox : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;
    public virtual double? minWidth { get; private set; }
    public virtual double? maxWidth { get; private set; }
    public virtual double? minHeight { get; private set; }
    public virtual double? maxHeight { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.OverflowBoxFit fit { get; private set; } = default!;

    public OverflowBox(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, double? minWidth = null, double? maxWidth = null, double? minHeight = null, double? maxHeight = null, global::Doroti.Generated.Framework.Rendering.OverflowBoxFit fit = global::Doroti.Generated.Framework.Rendering.OverflowBoxFit.max, Widget? child = null) : base(key: key, child: child)
    {
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center;
        this.alignment = __alignment;
        this.minWidth = minWidth;
        this.maxWidth = maxWidth;
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
        this.fit = fit;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderConstrainedOverflowBox(alignment: this.alignment, minWidth: this.minWidth, maxWidth: this.maxWidth, minHeight: this.minHeight, maxHeight: this.maxHeight, fit: this.fit, textDirection: Directionality.maybeOf(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderConstrainedOverflowBox)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderConstrainedOverflowBox>)(() =>
{            var __cascade = __renderObject;
            __cascade.alignment = this.alignment;
            __cascade.minWidth = this.minWidth;
            __cascade.maxWidth = this.maxWidth;
            __cascade.minHeight = this.minHeight;
            __cascade.maxHeight = this.maxHeight;
            __cascade.fit = this.fit;
            __cascade.textDirection = Directionality.maybeOf(context);
            return __cascade;        }))());
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.AlignmentGeometry>("alignment", this.alignment));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("minWidth", this.minWidth, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("maxWidth", this.maxWidth, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("minHeight", this.minHeight, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("maxHeight", this.maxHeight, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Rendering.OverflowBoxFit>("fit", this.fit));
    }

}

public class SizedOverflowBox : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;
    public virtual Size size { get; private set; } = default!;

    public SizedOverflowBox(global::Doroti.Generated.Framework.Foundation.Key? key = null, Size size = default!, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, Widget? child = null) : base(key: key, child: child)
    {
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center;
        this.size = size;
        this.alignment = __alignment;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderSizedOverflowBox(alignment: this.alignment, requestedSize: this.size, textDirection: Directionality.of(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderSizedOverflowBox)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderSizedOverflowBox>)(() =>
{            var __cascade = __renderObject;
            __cascade.alignment = this.alignment;
            __cascade.requestedSize = this.size;
            __cascade.textDirection = Directionality.of(context);
            return __cascade;        }))());
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.AlignmentGeometry>("alignment", this.alignment));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Size>("size", this.size, defaultValue: null));
    }

}

public class Offstage : SingleChildRenderObjectWidget
{
    public virtual bool offstage { get; private set; } = default!;

    public Offstage(global::Doroti.Generated.Framework.Foundation.Key? key = null, bool offstage = true, Widget? child = null) : base(key: key, child: child)
    {
        this.offstage = offstage;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(new global::Doroti.Generated.Framework.Rendering.RenderOffstage(offstage: this.offstage));
    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderOffstage)(object)renderObject;
        __renderObject.offstage = this.offstage;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("offstage", this.offstage));
    }

    public override SingleChildRenderObjectElement createElement() => DartRuntimePrimitives.ConvertValue<SingleChildRenderObjectElement>(new _OffstageElement__basic(this));
}

internal class _OffstageElement__basic : SingleChildRenderObjectElement
{
    internal _OffstageElement__basic(Offstage widget) : base(widget)
    {
    }

    public override void debugVisitOnstageChildren(global::System.Action<Element> visitor)
    {
        if (!(((Offstage?)(object?)this.widget)!).offstage)
        {
            base.debugVisitOnstageChildren((global::System.Action<Element>)visitor);
        }
    }

}

public class AspectRatio : SingleChildRenderObjectWidget
{
    public virtual double aspectRatio { get; private set; } = default!;

    public AspectRatio(global::Doroti.Generated.Framework.Foundation.Key? key = null, double aspectRatio = default!, Widget? child = null) : base(key: key, child: child)
    {
        this.aspectRatio = aspectRatio;
        System.Diagnostics.Debug.Assert((aspectRatio > 0.0));
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(new global::Doroti.Generated.Framework.Rendering.RenderAspectRatio(aspectRatio: this.aspectRatio));
    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderAspectRatio)(object)renderObject;
        __renderObject.aspectRatio = this.aspectRatio;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("aspectRatio", this.aspectRatio));
    }

}

public class IntrinsicWidth : SingleChildRenderObjectWidget
{
    public virtual double? stepWidth { get; private set; }
    public virtual double? stepHeight { get; private set; }

    public IntrinsicWidth(global::Doroti.Generated.Framework.Foundation.Key? key = null, double? stepWidth = null, double? stepHeight = null, Widget? child = null) : base(key: key, child: child)
    {
        this.stepWidth = stepWidth;
        this.stepHeight = stepHeight;
        System.Diagnostics.Debug.Assert(((stepWidth is null) || (stepWidth >= 0.0)));
        System.Diagnostics.Debug.Assert(((stepHeight is null) || (stepHeight >= 0.0)));
    }

    internal virtual double? _stepWidth => ((this.stepWidth == 0.0) ? null : this.stepWidth);
    internal virtual double? _stepHeight => ((this.stepHeight == 0.0) ? null : this.stepHeight);
    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderIntrinsicWidth(stepWidth: this._stepWidth, stepHeight: this._stepHeight));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderIntrinsicWidth)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderIntrinsicWidth>)(() =>
{            var __cascade = __renderObject;
            __cascade.stepWidth = this._stepWidth;
            __cascade.stepHeight = this._stepHeight;
            return __cascade;        }))());
    }

}

public class IntrinsicHeight : SingleChildRenderObjectWidget
{
    public IntrinsicHeight(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget? child = null) : base(key: key, child: child)
    {
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(new global::Doroti.Generated.Framework.Rendering.RenderIntrinsicHeight());
}

public class Baseline : SingleChildRenderObjectWidget
{
    public virtual double baseline { get; private set; } = default!;
    public virtual TextBaseline baselineType { get; private set; } = default!;

    public Baseline(global::Doroti.Generated.Framework.Foundation.Key? key = null, double baseline = default!, TextBaseline baselineType = default!, Widget? child = null) : base(key: key, child: child)
    {
        this.baseline = baseline;
        this.baselineType = baselineType;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderBaseline(baseline: this.baseline, baselineType: this.baselineType));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderBaseline)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderBaseline>)(() =>
{            var __cascade = __renderObject;
            __cascade.baseline = this.baseline;
            __cascade.baselineType = this.baselineType;
            return __cascade;        }))());
    }

}

public class IgnoreBaseline : SingleChildRenderObjectWidget
{
    public IgnoreBaseline(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget? child = null) : base(key: key, child: child)
    {
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderIgnoreBaseline());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SliverToBoxAdapter : SingleChildRenderObjectWidget
{
    public SliverToBoxAdapter(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget? child = null) : base(key: key, child: child)
    {
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(new global::Doroti.Generated.Framework.Rendering.RenderSliverToBoxAdapter());
}

public class SliverPadding : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding { get; private set; } = default!;

    public SliverPadding(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding = default!, Widget? sliver = null) : base(key: key, child: sliver)
    {
        this.padding = padding;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderSliverPadding(padding: this.padding, textDirection: Directionality.of(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderSliverPadding)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderSliverPadding>)(() =>
{            var __cascade = __renderObject;
            __cascade.padding = this.padding;
            __cascade.textDirection = Directionality.of(context);
            return __cascade;        }))());
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>("padding", this.padding));
    }

}

public abstract class _SemanticsBase__basic : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Semantics.SemanticsProperties properties { get; private set; } = default!;
    public virtual bool container { get; private set; } = default!;
    public virtual bool explicitChildNodes { get; private set; } = default!;
    public virtual Locale? localeForSubtree { get; private set; }
    public virtual bool excludeSemantics { get; private set; } = default!;
    public virtual bool blockUserActions { get; private set; } = default!;

    internal _SemanticsBase__basic(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget? child = null, bool container = default!, bool explicitChildNodes = default!, bool excludeSemantics = default!, bool blockUserActions = default!, bool? enabled = default!, bool? @checked = default!, bool? mixed = default!, bool? selected = default!, bool? toggled = default!, bool? button = default!, bool? slider = default!, bool? keyboardKey = default!, bool? link = default!, DartUri? linkUrl = default!, bool? header = default!, long? headingLevel = default!, bool? textField = default!, bool? readOnly = default!, bool? focusable = default!, bool? focused = default!, global::Doroti.Generated.Framework.Semantics.AccessibilityFocusBlockType? accessibilityFocusBlockType = default!, bool? inMutuallyExclusiveGroup = default!, bool? obscured = default!, bool? multiline = default!, bool? scopesRoute = default!, bool? namesRoute = default!, bool? hidden = default!, bool? image = default!, bool? liveRegion = default!, bool? expanded = default!, bool? isRequired = default!, long? maxValueLength = default!, long? currentValueLength = default!, string? identifier = default!, object? traversalParentIdentifier = default!, object? traversalChildIdentifier = default!, string? label = default!, global::Doroti.Generated.Framework.Semantics.AttributedString? attributedLabel = default!, string? value = default!, global::Doroti.Generated.Framework.Semantics.AttributedString? attributedValue = default!, string? increasedValue = default!, global::Doroti.Generated.Framework.Semantics.AttributedString? attributedIncreasedValue = default!, string? decreasedValue = default!, global::Doroti.Generated.Framework.Semantics.AttributedString? attributedDecreasedValue = default!, string? hint = default!, global::Doroti.Generated.Framework.Semantics.AttributedString? attributedHint = default!, string? tooltip = default!, string? onTapHint = default!, string? onLongPressHint = default!, TextDirection? textDirection = default!, global::Doroti.Generated.Framework.Semantics.SemanticsSortKey? sortKey = default!, global::Doroti.Generated.Framework.Semantics.SemanticsTag? tagForChildren = default!, global::System.Action? onTap = default!, global::System.Action? onLongPress = default!, global::System.Action? onScrollLeft = default!, global::System.Action? onScrollRight = default!, global::System.Action? onScrollUp = default!, global::System.Action? onScrollDown = default!, global::System.Action? onIncrease = default!, global::System.Action? onDecrease = default!, global::System.Action? onCopy = default!, global::System.Action? onCut = default!, global::System.Action? onPaste = default!, global::System.Action? onDismiss = default!, global::System.Action<bool>? onMoveCursorForwardByCharacter = default!, global::System.Action<bool>? onMoveCursorBackwardByCharacter = default!, global::System.Action<global::Doroti.Generated.Framework.Services.TextSelection>? onSetSelection = default!, global::System.Action<string>? onSetText = default!, global::System.Action? onDidGainAccessibilityFocus = default!, global::System.Action? onDidLoseAccessibilityFocus = default!, global::System.Action? onFocus = default!, global::System.Action? onExpand = default!, global::System.Action? onCollapse = default!, DartMap<global::Doroti.Generated.Framework.Semantics.CustomSemanticsAction, global::System.Action>? customSemanticsActions = default!, SemanticsRole? role = default!, HashSet<string>? controlsNodes = default!, SemanticsValidationResult validationResult = default!, SemanticsHitTestBehavior? hitTestBehavior = default!, SemanticsInputType? inputType = default!, Locale? localeForSubtree = default!, string? minValue = default!, string? maxValue = default!) : this(key: key, child: child, container: container, explicitChildNodes: explicitChildNodes, excludeSemantics: excludeSemantics, blockUserActions: blockUserActions, localeForSubtree: localeForSubtree, properties: new global::Doroti.Generated.Framework.Semantics.SemanticsProperties(enabled: enabled, @checked: @checked, mixed: mixed, expanded: expanded, toggled: toggled, selected: selected, button: button, slider: slider, keyboardKey: keyboardKey, link: link, linkUrl: linkUrl, header: header, headingLevel: headingLevel, textField: textField, readOnly: readOnly, focusable: focusable, focused: focused, accessibilityFocusBlockType: accessibilityFocusBlockType, inMutuallyExclusiveGroup: inMutuallyExclusiveGroup, obscured: obscured, multiline: multiline, scopesRoute: scopesRoute, namesRoute: namesRoute, hidden: hidden, image: image, liveRegion: liveRegion, isRequired: isRequired, maxValueLength: maxValueLength, currentValueLength: currentValueLength, identifier: identifier, traversalParentIdentifier: traversalParentIdentifier, traversalChildIdentifier: traversalChildIdentifier, label: label, attributedLabel: attributedLabel, value: value, attributedValue: attributedValue, increasedValue: increasedValue, attributedIncreasedValue: attributedIncreasedValue, decreasedValue: decreasedValue, attributedDecreasedValue: attributedDecreasedValue, hint: hint, attributedHint: attributedHint, tooltip: tooltip, textDirection: textDirection, sortKey: sortKey, tagForChildren: tagForChildren, onTap: onTap, onLongPress: onLongPress, onScrollLeft: onScrollLeft, onScrollRight: onScrollRight, onScrollUp: onScrollUp, onScrollDown: onScrollDown, onIncrease: onIncrease, onDecrease: onDecrease, onCopy: onCopy, onCut: onCut, onPaste: onPaste, onMoveCursorForwardByCharacter: (global::System.Action<bool>?)onMoveCursorForwardByCharacter, onMoveCursorBackwardByCharacter: (global::System.Action<bool>?)onMoveCursorBackwardByCharacter, onDidGainAccessibilityFocus: onDidGainAccessibilityFocus, onDidLoseAccessibilityFocus: onDidLoseAccessibilityFocus, onFocus: onFocus, onDismiss: onDismiss, onSetSelection: (global::System.Action<global::Doroti.Generated.Framework.Services.TextSelection>?)onSetSelection, onSetText: (global::System.Action<string>?)onSetText, onExpand: onExpand, onCollapse: onCollapse, customSemanticsActions: (DartMap<global::Doroti.Generated.Framework.Semantics.CustomSemanticsAction, global::System.Action>?)customSemanticsActions, hintOverrides: (((onTapHint is not null) || (onLongPressHint is not null)) ? new global::Doroti.Generated.Framework.Semantics.SemanticsHintOverrides(onTapHint: onTapHint, onLongPressHint: onLongPressHint) : null), role: role, controlsNodes: controlsNodes, validationResult: validationResult, hitTestBehavior: hitTestBehavior, inputType: inputType, minValue: minValue, maxValue: maxValue))
    {
    }

    internal _SemanticsBase__basic(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget? child = null, bool container = default!, bool explicitChildNodes = default!, bool excludeSemantics = default!, bool blockUserActions = default!, Locale? localeForSubtree = default!, global::Doroti.Generated.Framework.Semantics.SemanticsProperties properties = default!) : base(key: key, child: child)
    {
        this.container = container;
        this.explicitChildNodes = explicitChildNodes;
        this.excludeSemantics = excludeSemantics;
        this.blockUserActions = blockUserActions;
        this.localeForSubtree = localeForSubtree;
        this.properties = properties;
    }

    internal virtual global::Doroti.Ui.TextDirection? _getTextDirection(BuildContext context)
    {
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this.properties).textDirection is not null))
        {
            return ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this.properties).textDirection;
        }
        bool containsText__162694 = (((((((((((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this.properties).label is not null) || (((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this.properties).attributedLabel is not null)) || (((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this.properties).value is not null)) || (((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this.properties).attributedValue is not null)) || (((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this.properties).increasedValue is not null)) || (((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this.properties).attributedIncreasedValue is not null)) || (((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this.properties).decreasedValue is not null)) || (((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this.properties).attributedDecreasedValue is not null)) || (((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this.properties).hint is not null)) || (((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this.properties).attributedHint is not null)) || (((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this.properties).tooltip is not null));
        if (!containsText__162694)
        {
            return null;
        }
        return Directionality.maybeOf(context);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SliverSemantics : _SemanticsBase__basic
{
    public SliverSemantics(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget sliver = default!, bool container = false, bool explicitChildNodes = false, bool excludeSemantics = false, bool blockUserActions = false, bool? enabled = null, bool? @checked = null, bool? mixed = null, bool? selected = null, bool? toggled = null, bool? button = null, bool? slider = null, bool? keyboardKey = null, bool? link = null, DartUri? linkUrl = null, bool? header = null, long? headingLevel = null, bool? textField = null, bool? readOnly = null, bool? focusable = null, bool? focused = null, global::Doroti.Generated.Framework.Semantics.AccessibilityFocusBlockType? accessibilityFocusBlockType = null, bool? inMutuallyExclusiveGroup = null, bool? obscured = null, bool? multiline = null, bool? scopesRoute = null, bool? namesRoute = null, bool? hidden = null, bool? image = null, bool? liveRegion = null, bool? expanded = null, bool? isRequired = null, long? maxValueLength = null, long? currentValueLength = null, string? identifier = null, object? traversalParentIdentifier = null, object? traversalChildIdentifier = null, string? label = null, global::Doroti.Generated.Framework.Semantics.AttributedString? attributedLabel = null, string? value = null, global::Doroti.Generated.Framework.Semantics.AttributedString? attributedValue = null, string? increasedValue = null, global::Doroti.Generated.Framework.Semantics.AttributedString? attributedIncreasedValue = null, string? decreasedValue = null, global::Doroti.Generated.Framework.Semantics.AttributedString? attributedDecreasedValue = null, string? hint = null, global::Doroti.Generated.Framework.Semantics.AttributedString? attributedHint = null, string? tooltip = null, string? onTapHint = null, string? onLongPressHint = null, TextDirection? textDirection = null, global::Doroti.Generated.Framework.Semantics.SemanticsSortKey? sortKey = null, global::Doroti.Generated.Framework.Semantics.SemanticsTag? tagForChildren = null, global::System.Action? onTap = null, global::System.Action? onLongPress = null, global::System.Action? onScrollLeft = null, global::System.Action? onScrollRight = null, global::System.Action? onScrollUp = null, global::System.Action? onScrollDown = null, global::System.Action? onIncrease = null, global::System.Action? onDecrease = null, global::System.Action? onCopy = null, global::System.Action? onCut = null, global::System.Action? onPaste = null, global::System.Action? onDismiss = null, global::System.Action<bool>? onMoveCursorForwardByCharacter = null, global::System.Action<bool>? onMoveCursorBackwardByCharacter = null, global::System.Action<global::Doroti.Generated.Framework.Services.TextSelection>? onSetSelection = null, global::System.Action<string>? onSetText = null, global::System.Action? onDidGainAccessibilityFocus = null, global::System.Action? onDidLoseAccessibilityFocus = null, global::System.Action? onFocus = null, global::System.Action? onExpand = null, global::System.Action? onCollapse = null, DartMap<global::Doroti.Generated.Framework.Semantics.CustomSemanticsAction, global::System.Action>? customSemanticsActions = null, SemanticsRole? role = null, HashSet<string>? controlsNodes = null, SemanticsValidationResult validationResult = SemanticsValidationResult.none, SemanticsHitTestBehavior? hitTestBehavior = null, SemanticsInputType? inputType = null, Locale? localeForSubtree = null, string? minValue = null, string? maxValue = null) : base(key: key, container: container, explicitChildNodes: explicitChildNodes, excludeSemantics: excludeSemantics, blockUserActions: blockUserActions, enabled: DartRuntimePrimitives.RequireValue(enabled), @checked: DartRuntimePrimitives.RequireValue(@checked), mixed: DartRuntimePrimitives.RequireValue(mixed), selected: DartRuntimePrimitives.RequireValue(selected), toggled: DartRuntimePrimitives.RequireValue(toggled), button: DartRuntimePrimitives.RequireValue(button), slider: DartRuntimePrimitives.RequireValue(slider), keyboardKey: DartRuntimePrimitives.RequireValue(keyboardKey), link: DartRuntimePrimitives.RequireValue(link), linkUrl: linkUrl, header: DartRuntimePrimitives.RequireValue(header), headingLevel: DartRuntimePrimitives.RequireValue(headingLevel), textField: DartRuntimePrimitives.RequireValue(textField), readOnly: DartRuntimePrimitives.RequireValue(readOnly), focusable: DartRuntimePrimitives.RequireValue(focusable), focused: DartRuntimePrimitives.RequireValue(focused), accessibilityFocusBlockType: DartRuntimePrimitives.RequireValue(accessibilityFocusBlockType), inMutuallyExclusiveGroup: DartRuntimePrimitives.RequireValue(inMutuallyExclusiveGroup), obscured: DartRuntimePrimitives.RequireValue(obscured), multiline: DartRuntimePrimitives.RequireValue(multiline), scopesRoute: DartRuntimePrimitives.RequireValue(scopesRoute), namesRoute: DartRuntimePrimitives.RequireValue(namesRoute), hidden: DartRuntimePrimitives.RequireValue(hidden), image: DartRuntimePrimitives.RequireValue(image), liveRegion: DartRuntimePrimitives.RequireValue(liveRegion), expanded: DartRuntimePrimitives.RequireValue(expanded), isRequired: DartRuntimePrimitives.RequireValue(isRequired), maxValueLength: DartRuntimePrimitives.RequireValue(maxValueLength), currentValueLength: DartRuntimePrimitives.RequireValue(currentValueLength), identifier: identifier, traversalParentIdentifier: traversalParentIdentifier, traversalChildIdentifier: traversalChildIdentifier, label: label, attributedLabel: attributedLabel, value: value, attributedValue: attributedValue, increasedValue: increasedValue, attributedIncreasedValue: attributedIncreasedValue, decreasedValue: decreasedValue, attributedDecreasedValue: attributedDecreasedValue, hint: hint, attributedHint: attributedHint, tooltip: tooltip, onTapHint: onTapHint, onLongPressHint: onLongPressHint, textDirection: DartRuntimePrimitives.RequireValue(textDirection), sortKey: sortKey, tagForChildren: tagForChildren, onTap: onTap, onLongPress: onLongPress, onScrollLeft: onScrollLeft, onScrollRight: onScrollRight, onScrollUp: onScrollUp, onScrollDown: onScrollDown, onIncrease: onIncrease, onDecrease: onDecrease, onCopy: onCopy, onCut: onCut, onPaste: onPaste, onDismiss: onDismiss, onMoveCursorForwardByCharacter: onMoveCursorForwardByCharacter, onMoveCursorBackwardByCharacter: onMoveCursorBackwardByCharacter, onSetSelection: onSetSelection, onSetText: onSetText, onDidGainAccessibilityFocus: onDidGainAccessibilityFocus, onDidLoseAccessibilityFocus: onDidLoseAccessibilityFocus, onFocus: onFocus, onExpand: onExpand, onCollapse: onCollapse, customSemanticsActions: customSemanticsActions, role: DartRuntimePrimitives.RequireValue(role), controlsNodes: controlsNodes, validationResult: validationResult, hitTestBehavior: DartRuntimePrimitives.RequireValue(hitTestBehavior), inputType: DartRuntimePrimitives.RequireValue(inputType), localeForSubtree: DartRuntimePrimitives.RequireValue(localeForSubtree), minValue: minValue, maxValue: maxValue, child: sliver)
    {
    }

    public static SliverSemantics CreateFromProperties(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget? child = null, bool container = false, bool explicitChildNodes = false, bool excludeSemantics = false, bool blockUserActions = false, Locale? localeForSubtree = null, global::Doroti.Generated.Framework.Semantics.SemanticsProperties properties = default!)
    {
        var __instance = new SliverSemantics(default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!);
        return __instance;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderSliverSemanticsAnnotations(container: this.container, explicitChildNodes: this.explicitChildNodes, excludeSemantics: this.excludeSemantics, blockUserActions: this.blockUserActions, properties: this.properties, localeForSubtree: this.localeForSubtree, textDirection: _getTextDirection(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderSliverSemanticsAnnotations)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderSliverSemanticsAnnotations>)(() =>
{            var __cascade = __renderObject;
            __cascade.container = this.container;
            __cascade.explicitChildNodes = this.explicitChildNodes;
            __cascade.excludeSemantics = this.excludeSemantics;
            __cascade.blockUserActions = this.blockUserActions;
            __cascade.properties = this.properties;
            __cascade.textDirection = _getTextDirection(context);
            __cascade.localeForSubtree = this.localeForSubtree;
            return __cascade;        }))());
    }

}

public static partial class BasicLibrary
{
    public static global::Doroti.Generated.Framework.Painting.AxisDirection getAxisDirectionFromAxisReverseAndDirectionality(BuildContext context, global::Doroti.Generated.Framework.Painting.Axis axis, bool reverse)
    {
        switch (axis)
        {
            case global::Doroti.Generated.Framework.Painting.Axis.horizontal:
                {
                    DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context));
                    global::Doroti.Ui.TextDirection textDirection__168392 = Directionality.of(context);
                    global::Doroti.Generated.Framework.Painting.AxisDirection axisDirection__168462 = global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.textDirectionToAxisDirection(textDirection__168392);
                    return (reverse ? global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.flipAxisDirection(axisDirection__168462) : axisDirection__168462);
                }
            case global::Doroti.Generated.Framework.Painting.Axis.vertical:
                {
                    return (reverse ? global::Doroti.Generated.Framework.Painting.AxisDirection.up : global::Doroti.Generated.Framework.Painting.AxisDirection.down);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class ListBody : MultiChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Painting.Axis mainAxis { get; private set; } = default!;
    public virtual bool reverse { get; private set; } = default!;

    public ListBody(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.Axis mainAxis = global::Doroti.Generated.Framework.Painting.Axis.vertical, bool reverse = false, List<Widget> children = default!) : base(key: key, children: children ?? new List<Widget>())
    {
        this.mainAxis = mainAxis;
        this.reverse = reverse;
    }

    internal virtual global::Doroti.Generated.Framework.Painting.AxisDirection _getDirection(BuildContext context)
    {
        return BasicLibrary.getAxisDirectionFromAxisReverseAndDirectionality(context, this.mainAxis, this.reverse);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderListBody(axisDirection: _getDirection(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderListBody)(object)renderObject;
        __renderObject.axisDirection = _getDirection(context);
    }

}

public class Stack : MultiChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;
    public virtual TextDirection? textDirection { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.StackFit fit { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;

    public Stack(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, TextDirection? textDirection = null, global::Doroti.Generated.Framework.Rendering.StackFit fit = global::Doroti.Generated.Framework.Rendering.StackFit.loose, Clip clipBehavior = Clip.hardEdge, List<Widget> children = default!) : base(key: key, children: children ?? new List<Widget>())
    {
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.AlignmentDirectional.topStart;
        this.alignment = __alignment;
        this.textDirection = textDirection;
        this.fit = fit;
        this.clipBehavior = clipBehavior;
    }

    public static Stack Create(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, TextDirection? textDirection = null, global::Doroti.Generated.Framework.Rendering.StackFit fit = global::Doroti.Generated.Framework.Rendering.StackFit.loose, Clip clipBehavior = Clip.hardEdge, List<Widget> children = default!) =>
        new(key, alignment, textDirection, fit, clipBehavior, children);

    internal virtual bool _debugCheckHasDirectionality(BuildContext context)
    {
        if (((this.alignment is global::Doroti.Generated.Framework.Painting.AlignmentDirectional) && (this.textDirection is null)))
        {
            global::Doroti.Generated.Framework.Painting.AlignmentDirectional alignment__as178201 = (global::Doroti.Generated.Framework.Painting.AlignmentDirectional)alignment;
            DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context, why: "to resolve the 'alignment' argument", hint: ((object.Equals(this.alignment, global::Doroti.Generated.Framework.Painting.AlignmentDirectional.topStart)) ? "The default value for 'alignment' is AlignmentDirectional.topStart, which requires a text direction." : null), alternative: $"Instead of providing a Directionality widget, another solution would be passing a non-directional 'alignment__as178201', or an explicit 'textDirection', to the {this.GetType()}."));
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => _debugCheckHasDirectionality(context));
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderStack(alignment: this.alignment, textDirection: ((this.textDirection ?? (TextDirection)Directionality.maybeOf(context))), fit: this.fit, clipBehavior: this.clipBehavior));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderStack)(object)renderObject;
        DartRuntimePrimitives.Assert(() => _debugCheckHasDirectionality(context));
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderStack>)(() =>
{            var __cascade = __renderObject;
            __cascade.alignment = this.alignment;
            __cascade.textDirection = ((this.textDirection ?? (TextDirection)Directionality.maybeOf(context)));
            __cascade.fit = this.fit;
            __cascade.clipBehavior = this.clipBehavior;
            return __cascade;        }))());
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.AlignmentGeometry>("alignment", this.alignment));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Rendering.StackFit>("fit", this.fit));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Ui.Clip>("clipBehavior", this.clipBehavior, defaultValue: Clip.hardEdge));
    }

}

public class Positioned : ParentDataWidget<global::Doroti.Generated.Framework.Rendering.StackParentData>
{
    public virtual double? left { get; private set; }
    public virtual double? top { get; private set; }
    public virtual double? right { get; private set; }
    public virtual double? bottom { get; private set; }
    public virtual double? width { get; private set; }
    public virtual double? height { get; private set; }

    public Positioned(global::Doroti.Generated.Framework.Foundation.Key? key = null, double? left = null, double? top = null, double? right = null, double? bottom = null, double? width = null, double? height = null, Widget child = default!) : base(key: key, child: child)
    {
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
        this.width = width;
        this.height = height;
        System.Diagnostics.Debug.Assert((((left is null) || (right is null)) || (width is null)));
        System.Diagnostics.Debug.Assert((((top is null) || (bottom is null)) || (height is null)));
    }

    public static Positioned CreateFromRect(global::Doroti.Generated.Framework.Foundation.Key? key = null, Rect rect = default!, Widget child = default!)
    {
        var __instance = new Positioned(default!, default!, default!, default!, default!, default!, default!, default!);
        __instance.left = rect.left;
        __instance.top = rect.top;
        __instance.width = rect.width;
        __instance.height = rect.height;
        __instance.right = null;
        __instance.bottom = null;
        return __instance;
    }

    public static Positioned CreateFromRelativeRect(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Rendering.RelativeRect rect = default!, Widget child = default!)
    {
        var __instance = new Positioned(default!, default!, default!, default!, default!, default!, default!, default!);
        __instance.left = ((global::Doroti.Generated.Framework.Rendering.RelativeRect)rect).left;
        __instance.top = ((global::Doroti.Generated.Framework.Rendering.RelativeRect)rect).top;
        __instance.right = ((global::Doroti.Generated.Framework.Rendering.RelativeRect)rect).right;
        __instance.bottom = ((global::Doroti.Generated.Framework.Rendering.RelativeRect)rect).bottom;
        __instance.width = null;
        __instance.height = null;
        return __instance;
    }

    public static Positioned CreateFill(global::Doroti.Generated.Framework.Foundation.Key? key = null, double? left = 0.0, double? top = 0.0, double? right = 0.0, double? bottom = 0.0, Widget child = default!)
    {
        var __instance = new Positioned(default!, default!, default!, default!, default!, default!, default!, default!);
        __instance.left = left;
        __instance.top = top;
        __instance.right = right;
        __instance.bottom = bottom;
        __instance.width = null;
        __instance.height = null;
        return __instance;
    }

    public static Positioned CreateDirectional(global::Doroti.Generated.Framework.Foundation.Key? key = null, TextDirection textDirection = default!, double? start = null, double? top = null, double? end = null, double? bottom = null, double? width = null, double? height = null, Widget child = default!)
    {
        var (left__185043, right__185057) = (textDirection switch { TextDirection.rtl => (((double?, double?))((end, start))), TextDirection.ltr => (((double?, double?))((start, end))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return new Positioned(key: key, left: left__185043, top: top, right: right__185057, bottom: bottom, width: width, height: height, child: child);
    }

    public override void applyParentData(global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Generated.Framework.Rendering.RenderObject)renderObject).parentData is global::Doroti.Generated.Framework.Rendering.StackParentData));
        var parentData__187412 = ((global::Doroti.Generated.Framework.Rendering.StackParentData?)(object?)((global::Doroti.Generated.Framework.Rendering.RenderObject)renderObject).parentData!)!;
        var needsLayout__187478 = false;
        if ((((global::Doroti.Generated.Framework.Rendering.StackParentData)parentData__187412).left != this.left))
        {
            parentData__187412.left = this.left;
            needsLayout__187478 = true;
        }
        if ((((global::Doroti.Generated.Framework.Rendering.StackParentData)parentData__187412).top != this.top))
        {
            parentData__187412.top = this.top;
            needsLayout__187478 = true;
        }
        if ((((global::Doroti.Generated.Framework.Rendering.StackParentData)parentData__187412).right != this.right))
        {
            parentData__187412.right = this.right;
            needsLayout__187478 = true;
        }
        if ((((global::Doroti.Generated.Framework.Rendering.StackParentData)parentData__187412).bottom != this.bottom))
        {
            parentData__187412.bottom = this.bottom;
            needsLayout__187478 = true;
        }
        if ((((global::Doroti.Generated.Framework.Rendering.StackParentData)parentData__187412).width != this.width))
        {
            parentData__187412.width = this.width;
            needsLayout__187478 = true;
        }
        if ((((global::Doroti.Generated.Framework.Rendering.StackParentData)parentData__187412).height != this.height))
        {
            parentData__187412.height = this.height;
            needsLayout__187478 = true;
        }
        if (needsLayout__187478)
        {
            ((dynamic)((global::Doroti.Generated.Framework.Rendering.RenderObject)renderObject).parent)?.markNeedsLayout();
        }
    }

    public override Type debugTypicalAncestorWidgetClass => typeof(Stack);
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("left", this.left, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("top", this.top, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("right", this.right, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("bottom", this.bottom, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("width", this.width, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("height", this.height, defaultValue: null));
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

    public PositionedDirectional(global::Doroti.Generated.Framework.Foundation.Key? key = null, double? start = null, double? top = null, double? end = null, double? bottom = null, double? width = null, double? height = null, Widget child = default!) : base(key: key)
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
        return ((Widget)(object?)Positioned.CreateDirectional(textDirection: Directionality.of(context), start: this.start, top: this.top, end: this.end, bottom: this.bottom, width: this.width, height: this.height, child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class Flex : MultiChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Painting.Axis direction { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.MainAxisAlignment mainAxisAlignment { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.MainAxisSize mainAxisSize { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment crossAxisAlignment { get; private set; } = default!;
    public virtual TextDirection? textDirection { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.VerticalDirection verticalDirection { get; private set; } = default!;
    public virtual TextBaseline? textBaseline { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual double spacing { get; private set; } = default!;

    public Flex(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.Axis direction = default!, global::Doroti.Generated.Framework.Rendering.MainAxisAlignment mainAxisAlignment = global::Doroti.Generated.Framework.Rendering.MainAxisAlignment.start, global::Doroti.Generated.Framework.Rendering.MainAxisSize mainAxisSize = global::Doroti.Generated.Framework.Rendering.MainAxisSize.max, global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment crossAxisAlignment = global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.center, TextDirection? textDirection = null, global::Doroti.Generated.Framework.Painting.VerticalDirection verticalDirection = global::Doroti.Generated.Framework.Painting.VerticalDirection.down, TextBaseline? textBaseline = null, Clip clipBehavior = Clip.none, double spacing = 0.0, List<Widget> children = default!) : base(key: key, children: children ?? new List<Widget>())
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
        System.Diagnostics.Debug.Assert((!DartRuntimePrimitives.Identical(crossAxisAlignment, global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.baseline) || (textBaseline is not null)));
    }

    internal virtual bool _needTextDirection
    {
        get
        {
            switch (this.direction)
            {
                case global::Doroti.Generated.Framework.Painting.Axis.horizontal:
                    {
                        return true;
                    }
                case global::Doroti.Generated.Framework.Painting.Axis.vertical:
                    {
                        return ((object.Equals(this.crossAxisAlignment, global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.start)) || (object.Equals(this.crossAxisAlignment, global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.end)));
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
            return default!;
        }
    }
    public virtual global::Doroti.Ui.TextDirection? getEffectiveTextDirection(BuildContext context)
    {
        return (this.textDirection ?? ((this._needTextDirection ? Directionality.maybeOf(context) : null)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderFlex(direction: this.direction, mainAxisAlignment: this.mainAxisAlignment, mainAxisSize: this.mainAxisSize, crossAxisAlignment: this.crossAxisAlignment, textDirection: getEffectiveTextDirection(context), verticalDirection: this.verticalDirection, textBaseline: this.textBaseline, clipBehavior: this.clipBehavior, spacing: this.spacing));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderFlex)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderFlex>)(() =>
{            var __cascade = __renderObject;
            __cascade.direction = this.direction;
            __cascade.mainAxisAlignment = this.mainAxisAlignment;
            __cascade.mainAxisSize = this.mainAxisSize;
            __cascade.crossAxisAlignment = this.crossAxisAlignment;
            __cascade.textDirection = getEffectiveTextDirection(context);
            __cascade.verticalDirection = this.verticalDirection;
            __cascade.textBaseline = this.textBaseline;
            __cascade.clipBehavior = this.clipBehavior;
            __cascade.spacing = this.spacing;
            return __cascade;        }))());
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Painting.Axis>("direction", this.direction));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Rendering.MainAxisAlignment>("mainAxisAlignment", this.mainAxisAlignment));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Rendering.MainAxisSize>("mainAxisSize", this.mainAxisSize, defaultValue: global::Doroti.Generated.Framework.Rendering.MainAxisSize.max));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment>("crossAxisAlignment", this.crossAxisAlignment));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Painting.VerticalDirection>("verticalDirection", this.verticalDirection, defaultValue: global::Doroti.Generated.Framework.Painting.VerticalDirection.down));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextBaseline>("textBaseline", this.textBaseline, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Ui.Clip>("clipBehavior", this.clipBehavior, defaultValue: Clip.none));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("spacing", this.spacing, defaultValue: 0.0));
    }

}

public class Row : Flex
{
    public Row(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Rendering.MainAxisAlignment mainAxisAlignment = global::Doroti.Generated.Framework.Rendering.MainAxisAlignment.start, global::Doroti.Generated.Framework.Rendering.MainAxisSize mainAxisSize = global::Doroti.Generated.Framework.Rendering.MainAxisSize.max, global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment crossAxisAlignment = global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.center, TextDirection? textDirection = null, global::Doroti.Generated.Framework.Painting.VerticalDirection verticalDirection = global::Doroti.Generated.Framework.Painting.VerticalDirection.down, TextBaseline? textBaseline = null, double spacing = 0.0, List<Widget> children = default!) : base(key: key, mainAxisAlignment: mainAxisAlignment, mainAxisSize: mainAxisSize, crossAxisAlignment: crossAxisAlignment, textDirection: textDirection, verticalDirection: verticalDirection, textBaseline: textBaseline, spacing: spacing, children: children ?? new List<Widget>(), direction: global::Doroti.Generated.Framework.Painting.Axis.horizontal)
    {
    }

}

public class Column : Flex
{
    public Column(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Rendering.MainAxisAlignment mainAxisAlignment = global::Doroti.Generated.Framework.Rendering.MainAxisAlignment.start, global::Doroti.Generated.Framework.Rendering.MainAxisSize mainAxisSize = global::Doroti.Generated.Framework.Rendering.MainAxisSize.max, global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment crossAxisAlignment = global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.center, TextDirection? textDirection = null, global::Doroti.Generated.Framework.Painting.VerticalDirection verticalDirection = global::Doroti.Generated.Framework.Painting.VerticalDirection.down, TextBaseline? textBaseline = null, double spacing = 0.0, List<Widget> children = default!) : base(key: key, mainAxisAlignment: mainAxisAlignment, mainAxisSize: mainAxisSize, crossAxisAlignment: crossAxisAlignment, textDirection: textDirection, verticalDirection: verticalDirection, textBaseline: textBaseline, spacing: spacing, children: children ?? new List<Widget>(), direction: global::Doroti.Generated.Framework.Painting.Axis.vertical)
    {
    }

}

public class Flexible : ParentDataWidget<global::Doroti.Generated.Framework.Rendering.FlexParentData>
{
    public virtual long flex { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.FlexFit fit { get; private set; } = default!;

    public Flexible(global::Doroti.Generated.Framework.Foundation.Key? key = null, long flex = 1, global::Doroti.Generated.Framework.Rendering.FlexFit fit = global::Doroti.Generated.Framework.Rendering.FlexFit.loose, Widget child = default!) : base(key: key, child: child)
    {
        this.flex = flex;
        this.fit = fit;
    }

    public override void applyParentData(global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Generated.Framework.Rendering.RenderObject)renderObject).parentData is global::Doroti.Generated.Framework.Rendering.FlexParentData));
        var parentData__228051 = ((global::Doroti.Generated.Framework.Rendering.FlexParentData?)(object?)((global::Doroti.Generated.Framework.Rendering.RenderObject)renderObject).parentData!)!;
        var needsLayout__228116 = false;
        if ((((global::Doroti.Generated.Framework.Rendering.FlexParentData)parentData__228051).flex != this.flex))
        {
            parentData__228051.flex = this.flex;
            needsLayout__228116 = true;
        }
        if ((!object.Equals(((global::Doroti.Generated.Framework.Rendering.FlexParentData)parentData__228051).fit, this.fit)))
        {
            parentData__228051.fit = this.fit;
            needsLayout__228116 = true;
        }
        if (needsLayout__228116)
        {
            ((dynamic)((global::Doroti.Generated.Framework.Rendering.RenderObject)renderObject).parent)?.markNeedsLayout();
        }
    }

    public override Type debugTypicalAncestorWidgetClass => typeof(Flex);
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.IntProperty("flex", this.flex));
    }

}

public class Expanded : Flexible
{
    public Expanded(global::Doroti.Generated.Framework.Foundation.Key? key = null, long flex = 1, Widget child = default!) : base(key: key, flex: flex, child: child, fit: global::Doroti.Generated.Framework.Rendering.FlexFit.tight)
    {
    }

}

public class Wrap : MultiChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Painting.Axis direction { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.WrapAlignment alignment { get; private set; } = default!;
    public virtual double spacing { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.WrapAlignment runAlignment { get; private set; } = default!;
    public virtual double runSpacing { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.WrapCrossAlignment crossAxisAlignment { get; private set; } = default!;
    public virtual TextDirection? textDirection { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.VerticalDirection verticalDirection { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;

    public Wrap(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.Axis direction = global::Doroti.Generated.Framework.Painting.Axis.horizontal, global::Doroti.Generated.Framework.Rendering.WrapAlignment alignment = global::Doroti.Generated.Framework.Rendering.WrapAlignment.start, double spacing = 0.0, global::Doroti.Generated.Framework.Rendering.WrapAlignment runAlignment = global::Doroti.Generated.Framework.Rendering.WrapAlignment.start, double runSpacing = 0.0, global::Doroti.Generated.Framework.Rendering.WrapCrossAlignment crossAxisAlignment = global::Doroti.Generated.Framework.Rendering.WrapCrossAlignment.start, TextDirection? textDirection = null, global::Doroti.Generated.Framework.Painting.VerticalDirection verticalDirection = global::Doroti.Generated.Framework.Painting.VerticalDirection.down, Clip clipBehavior = Clip.none, List<Widget> children = default!) : base(key: key, children: children ?? new List<Widget>())
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

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderWrap(direction: this.direction, alignment: this.alignment, spacing: this.spacing, runAlignment: this.runAlignment, runSpacing: this.runSpacing, crossAxisAlignment: this.crossAxisAlignment, textDirection: ((this.textDirection ?? (TextDirection)Directionality.maybeOf(context))), verticalDirection: this.verticalDirection, clipBehavior: this.clipBehavior));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderWrap)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderWrap>)(() =>
{            var __cascade = __renderObject;
            __cascade.direction = this.direction;
            __cascade.alignment = this.alignment;
            __cascade.spacing = this.spacing;
            __cascade.runAlignment = this.runAlignment;
            __cascade.runSpacing = this.runSpacing;
            __cascade.crossAxisAlignment = this.crossAxisAlignment;
            __cascade.textDirection = ((this.textDirection ?? (TextDirection)Directionality.maybeOf(context)));
            __cascade.verticalDirection = this.verticalDirection;
            __cascade.clipBehavior = this.clipBehavior;
            return __cascade;        }))());
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Painting.Axis>("direction", this.direction));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Rendering.WrapAlignment>("alignment", this.alignment));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("spacing", this.spacing));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Rendering.WrapAlignment>("runAlignment", this.runAlignment));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("runSpacing", this.runSpacing));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Rendering.WrapCrossAlignment>("crossAxisAlignment", this.crossAxisAlignment));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Painting.VerticalDirection>("verticalDirection", this.verticalDirection, defaultValue: global::Doroti.Generated.Framework.Painting.VerticalDirection.down));
    }

}

public class Flow : MultiChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Rendering.FlowDelegate @delegate { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;

    public Flow(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Rendering.FlowDelegate @delegate = default!, List<Widget> children = default!, Clip clipBehavior = Clip.hardEdge) : base(key: key, children: RepaintBoundary.wrapAll(children))
    {
        List<Widget> __children = children ?? new List<Widget>();
        this.@delegate = @delegate;
        this.clipBehavior = clipBehavior;
    }

    public static Flow CreateUnwrapped(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Rendering.FlowDelegate @delegate = default!, List<Widget> children = default!, Clip clipBehavior = Clip.hardEdge)
    {
        var __instance = new Flow(default!, default!, default!, default!);
        List<Widget> __children = children ?? new List<Widget>();
        __instance.@delegate = @delegate;
        __instance.clipBehavior = clipBehavior;
        return __instance;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(new global::Doroti.Generated.Framework.Rendering.RenderFlow(@delegate: this.@delegate, clipBehavior: this.clipBehavior));
    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderFlow)(object)renderObject;
        __renderObject.@delegate = this.@delegate;
        __renderObject.clipBehavior = this.clipBehavior;
    }

}

public class RichText : MultiChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Painting.InlineSpan text { get; private set; } = default!;
    public virtual TextAlign textAlign { get; private set; } = default!;
    public virtual TextDirection? textDirection { get; private set; }
    public virtual bool softWrap { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.TextOverflow overflow { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.TextScaler textScaler { get; private set; } = default!;
    public virtual long? maxLines { get; private set; }
    public virtual Locale? locale { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.StrutStyle? strutStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextWidthBasis textWidthBasis { get; private set; } = default!;
    public virtual TextHeightBehavior? textHeightBehavior { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.SelectionRegistrar? selectionRegistrar { get; private set; }
    public virtual Color? selectionColor { get; private set; }

    public RichText(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.InlineSpan text = default!, TextAlign textAlign = TextAlign.start, TextDirection? textDirection = null, bool softWrap = true, global::Doroti.Generated.Framework.Painting.TextOverflow overflow = global::Doroti.Generated.Framework.Painting.TextOverflow.clip, double textScaleFactor = 1.0, global::Doroti.Generated.Framework.Painting.TextScaler textScaler = default!, long? maxLines = null, Locale? locale = null, global::Doroti.Generated.Framework.Painting.StrutStyle? strutStyle = null, global::Doroti.Generated.Framework.Painting.TextWidthBasis textWidthBasis = global::Doroti.Generated.Framework.Painting.TextWidthBasis.parent, TextHeightBehavior? textHeightBehavior = null, global::Doroti.Generated.Framework.Rendering.SelectionRegistrar? selectionRegistrar = null, Color? selectionColor = null) : base(key: key, children: WidgetSpan.extractFromInlineSpan(text, RichText._effectiveTextScalerFrom(textScaler, textScaleFactor)))
    {
        global::Doroti.Generated.Framework.Painting.TextScaler __textScaler = textScaler ?? global::Doroti.Generated.Framework.Painting.TextScaler.noScaling;
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
        this.textScaler = RichText._effectiveTextScalerFrom(textScaler, textScaleFactor);
        System.Diagnostics.Debug.Assert(((maxLines is null) || (DartRuntimePrimitives.RequireValue(maxLines) > 0L)));
        System.Diagnostics.Debug.Assert(((selectionRegistrar is null) || (selectionColor is not null)));
        System.Diagnostics.Debug.Assert(((textScaleFactor == 1.0) || DartRuntimePrimitives.Identical(__textScaler, global::Doroti.Generated.Framework.Painting.TextScaler.noScaling)));
    }

    internal static global::Doroti.Generated.Framework.Painting.TextScaler _effectiveTextScalerFrom(global::Doroti.Generated.Framework.Painting.TextScaler textScaler, double textScaleFactor)
    {
        textScaler ??= global::Doroti.Generated.Framework.Painting.TextScaler.noScaling;
        return ((textScaler, textScaleFactor) switch { (global::Doroti.Generated.Framework.Painting.TextScaler scaler__252109, 1.0) => scaler__252109, (var __constant252140, double textScaleFactor__252175) when (object.Equals(__constant252140, global::Doroti.Generated.Framework.Painting.TextScaler.noScaling)) => global::Doroti.Generated.Framework.Painting.TextScaler.CreateLinear(textScaleFactor__252175), (global::Doroti.Generated.Framework.Painting.TextScaler scaler__252255, _) => scaler__252255 });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double textScaleFactor => ((global::Doroti.Generated.Framework.Painting.TextScaler)this.textScaler).textScaleFactor;
    internal virtual double _getDevicePixelRatio(BuildContext context) => DartRuntimePrimitives.ConvertValue<double>(((MediaQuery.maybeDevicePixelRatioOf(context) ?? View.maybeOf(context)?.devicePixelRatio) ?? 1.0));
    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => ((this.textDirection is not null) || global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context)));
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderParagraph(this.text, textAlign: this.textAlign, textDirection: ((this.textDirection ?? (TextDirection)Directionality.of(context))), softWrap: this.softWrap, overflow: this.overflow, textScaler: this.textScaler, maxLines: this.maxLines, strutStyle: this.strutStyle, textWidthBasis: this.textWidthBasis, textHeightBehavior: this.textHeightBehavior, locale: (this.locale ?? Localizations.maybeLocaleOf(context)), registrar: this.selectionRegistrar, selectionColor: this.selectionColor, devicePixelRatio: _getDevicePixelRatio(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderParagraph)(object)renderObject;
        DartRuntimePrimitives.Assert(() => ((this.textDirection is not null) || global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context)));
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderParagraph>)(() =>
{            var __cascade = __renderObject;
            __cascade.text = this.text;
            __cascade.textAlign = this.textAlign;
            __cascade.textDirection = ((this.textDirection ?? (TextDirection)Directionality.of(context)));
            __cascade.softWrap = this.softWrap;
            __cascade.overflow = this.overflow;
            __cascade.textScaler = this.textScaler;
            __cascade.maxLines = this.maxLines;
            __cascade.strutStyle = this.strutStyle;
            __cascade.textWidthBasis = this.textWidthBasis;
            __cascade.textHeightBehavior = this.textHeightBehavior;
            __cascade.locale = (this.locale ?? Localizations.maybeLocaleOf(context));
            __cascade.registrar = this.selectionRegistrar;
            __cascade.selectionColor = this.selectionColor;
            __cascade.devicePixelRatio = _getDevicePixelRatio(context);
            return __cascade;        }))());
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextAlign>("textAlign", this.textAlign, defaultValue: global::Doroti.Ui.TextAlign.start));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("softWrap", value: this.softWrap, ifTrue: "wrapping at box width", ifFalse: "no wrapping except at line break characters", showName: true));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Painting.TextOverflow>("overflow", this.overflow, defaultValue: global::Doroti.Generated.Framework.Painting.TextOverflow.clip));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextScaler>("textScaler", this.textScaler, defaultValue: global::Doroti.Generated.Framework.Painting.TextScaler.noScaling));
        properties.add(new global::Doroti.Generated.Framework.Foundation.IntProperty("maxLines", this.maxLines, ifNull: "unlimited"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Painting.TextWidthBasis>("textWidthBasis", this.textWidthBasis, defaultValue: global::Doroti.Generated.Framework.Painting.TextWidthBasis.parent));
        properties.add(new global::Doroti.Generated.Framework.Foundation.StringProperty("text", this.text.toPlainText()));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Locale>("locale", this.locale, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.StrutStyle>("strutStyle", this.strutStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.TextHeightBehavior>("textHeightBehavior", this.textHeightBehavior, defaultValue: null));
    }

}

public class RawImage : LeafRenderObjectWidget
{
    public virtual global::Doroti.Ui.Image? image { get; private set; }
    public virtual string? debugImageLabel { get; private set; }
    public virtual double? width { get; private set; }
    public virtual double? height { get; private set; }
    public virtual double scale { get; private set; } = default!;
    public virtual Color? color { get; private set; }
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double>? opacity { get; private set; }
    public virtual FilterQuality filterQuality { get; private set; } = default!;
    public virtual BlendMode? colorBlendMode { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.BoxFit? fit { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.ImageRepeat repeat { get; private set; } = default!;
    public virtual Rect? centerSlice { get; private set; }
    public virtual bool matchTextDirection { get; private set; } = default!;
    public virtual bool invertColors { get; private set; } = default!;
    public virtual bool isAntiAlias { get; private set; } = default!;
    public virtual BlendMode blendMode { get; private set; } = default!;

    public RawImage(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Ui.Image? image = null, string? debugImageLabel = null, double? width = null, double? height = null, double scale = 1.0, Color? color = null, global::Doroti.Generated.Framework.Animation.Animation<double>? opacity = null, BlendMode? colorBlendMode = null, global::Doroti.Generated.Framework.Painting.BoxFit? fit = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Generated.Framework.Painting.ImageRepeat repeat = global::Doroti.Generated.Framework.Painting.ImageRepeat.noRepeat, Rect? centerSlice = null, bool matchTextDirection = false, bool invertColors = false, FilterQuality filterQuality = FilterQuality.medium, bool isAntiAlias = false, BlendMode blendMode = BlendMode.srcOver) : base(key: key)
    {
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center;
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

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => (((!this.matchTextDirection && (this.alignment is global::Doroti.Generated.Framework.Painting.Alignment))) || global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context)));
        DartRuntimePrimitives.Assert(() => ((global::Doroti.Ui.Image.debugGetOpenHandleStackTraces() is { } __items265689 ? System.Linq.Enumerable.Any(__items265689) : (bool?)null) ?? true), () => (object?)"Creator of a RawImage disposed of the image when the RawImage still " + "needed it.");
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderImage(image: this.image?.clone(), debugImageLabel: this.debugImageLabel, width: this.width, height: this.height, scale: this.scale, color: this.color, opacity: this.opacity, colorBlendMode: this.colorBlendMode, fit: this.fit, alignment: this.alignment, repeat: this.repeat, centerSlice: this.centerSlice, matchTextDirection: this.matchTextDirection, textDirection: ((this.matchTextDirection || (this.alignment is not global::Doroti.Generated.Framework.Painting.Alignment)) ? Directionality.of(context) : null), invertColors: this.invertColors, isAntiAlias: this.isAntiAlias, filterQuality: this.filterQuality, blendMode: this.blendMode));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderImage)(object)renderObject;
        DartRuntimePrimitives.Assert(() => ((global::Doroti.Ui.Image.debugGetOpenHandleStackTraces() is { } __items266606 ? System.Linq.Enumerable.Any(__items266606) : (bool?)null) ?? true), () => (object?)"Creator of a RawImage disposed of the image when the RawImage still " + "needed it.");
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderImage>)(() =>
{            var __cascade = __renderObject;
            __cascade.image = this.image?.clone();
            __cascade.debugImageLabel = this.debugImageLabel;
            __cascade.width = this.width;
            __cascade.height = this.height;
            __cascade.scale = this.scale;
            __cascade.color = this.color;
            __cascade.opacity = this.opacity;
            __cascade.colorBlendMode = this.colorBlendMode;
            __cascade.fit = this.fit;
            __cascade.alignment = this.alignment;
            __cascade.repeat = this.repeat;
            __cascade.centerSlice = this.centerSlice;
            __cascade.matchTextDirection = this.matchTextDirection;
            __cascade.textDirection = ((this.matchTextDirection || (this.alignment is not global::Doroti.Generated.Framework.Painting.Alignment)) ? Directionality.of(context) : null);
            __cascade.invertColors = this.invertColors;
            __cascade.isAntiAlias = this.isAntiAlias;
            __cascade.filterQuality = this.filterQuality;
            __cascade.blendMode = this.blendMode;
            return __cascade;        }))());
    }

    public override void didUnmountRenderObject(global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderImage)(object)renderObject;
        __renderObject.image = null;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Image>("image", this.image));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("width", this.width, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("height", this.height, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("scale", this.scale, defaultValue: 1.0));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("color", this.color, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Animation.Animation<double>?>("opacity", this.opacity, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Ui.BlendMode>("colorBlendMode", this.colorBlendMode, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Painting.BoxFit>("fit", this.fit, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.AlignmentGeometry>("alignment", this.alignment, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Painting.ImageRepeat>("repeat", this.repeat, defaultValue: global::Doroti.Generated.Framework.Painting.ImageRepeat.noRepeat));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Rect>("centerSlice", this.centerSlice, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("matchTextDirection", value: this.matchTextDirection, ifTrue: "match text direction"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("invertColors", this.invertColors));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Ui.FilterQuality>("filterQuality", this.filterQuality));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Ui.BlendMode>("blendMode", this.blendMode, defaultValue: BlendMode.srcOver));
    }

}

public class DefaultAssetBundle : InheritedWidget
{
    public virtual global::Doroti.Generated.Framework.Services.AssetBundle bundle { get; private set; } = default!;

    public DefaultAssetBundle(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Services.AssetBundle bundle = default!, Widget child = default!) : base(key: key, child: child)
    {
        this.bundle = bundle;
    }

    public static global::Doroti.Generated.Framework.Services.AssetBundle of(BuildContext context)
    {
        DefaultAssetBundle? result__271303 = ((DefaultAssetBundle?)(object?)context.dependOnInheritedWidgetOfExactType<DefaultAssetBundle>());
        return (result__271303?.bundle ?? global::Doroti.Generated.Framework.Services.Asset_bundleLibrary.rootBundle);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.bundle, ((DefaultAssetBundle)oldWidget).bundle)));
}

public class WidgetToRenderBoxAdapter : LeafRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Rendering.RenderBox renderBox { get; private set; } = default!;
    public virtual global::System.Action? onBuild { get; private set; }
    public virtual global::System.Action? onUnmount { get; private set; }

    public WidgetToRenderBoxAdapter(global::Doroti.Generated.Framework.Rendering.RenderBox renderBox, global::System.Action? onBuild = null, global::System.Action? onUnmount = null) : base(key: new GlobalObjectKey<IState>(renderBox))
    {
        this.renderBox = renderBox;
        this.onBuild = onBuild;
        this.onUnmount = onUnmount;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(this.renderBox);
    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderBox)(object)renderObject;
        this.onBuild?.Invoke();
    }

    public override void didUnmountRenderObject(global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(renderObject, this.renderBox)));
        this.onUnmount?.Invoke();
    }

}

public class Listener : SingleChildRenderObjectWidget
{
    public virtual global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerDownEvent>? onPointerDown { get; private set; }
    public virtual global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerMoveEvent>? onPointerMove { get; private set; }
    public virtual global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerUpEvent>? onPointerUp { get; private set; }
    public virtual global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerHoverEvent>? onPointerHover { get; private set; }
    public virtual global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerCancelEvent>? onPointerCancel { get; private set; }
    public virtual global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerPanZoomStartEvent>? onPointerPanZoomStart { get; private set; }
    public virtual global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerPanZoomUpdateEvent>? onPointerPanZoomUpdate { get; private set; }
    public virtual global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerPanZoomEndEvent>? onPointerPanZoomEnd { get; private set; }
    public virtual global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerSignalEvent>? onPointerSignal { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.HitTestBehavior behavior { get; private set; } = default!;

    public Listener(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerDownEvent>? onPointerDown = null, global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerMoveEvent>? onPointerMove = null, global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerUpEvent>? onPointerUp = null, global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerHoverEvent>? onPointerHover = null, global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerCancelEvent>? onPointerCancel = null, global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerPanZoomStartEvent>? onPointerPanZoomStart = null, global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerPanZoomUpdateEvent>? onPointerPanZoomUpdate = null, global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerPanZoomEndEvent>? onPointerPanZoomEnd = null, global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerSignalEvent>? onPointerSignal = null, global::Doroti.Generated.Framework.Rendering.HitTestBehavior behavior = global::Doroti.Generated.Framework.Rendering.HitTestBehavior.deferToChild, Widget? child = null) : base(key: key, child: child)
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

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderPointerListener(onPointerDown: (global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerDownEvent>?)this.onPointerDown, onPointerMove: (global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerMoveEvent>?)this.onPointerMove, onPointerUp: (global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerUpEvent>?)this.onPointerUp, onPointerHover: (global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerHoverEvent>?)this.onPointerHover, onPointerCancel: (global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerCancelEvent>?)this.onPointerCancel, onPointerPanZoomStart: (global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerPanZoomStartEvent>?)this.onPointerPanZoomStart, onPointerPanZoomUpdate: (global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerPanZoomUpdateEvent>?)this.onPointerPanZoomUpdate, onPointerPanZoomEnd: (global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerPanZoomEndEvent>?)this.onPointerPanZoomEnd, onPointerSignal: (global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerSignalEvent>?)this.onPointerSignal, behavior: this.behavior));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderPointerListener)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderPointerListener>)(() =>
{            var __cascade = __renderObject;
            __cascade.onPointerDown = this.onPointerDown;
            __cascade.onPointerMove = this.onPointerMove;
            __cascade.onPointerUp = this.onPointerUp;
            __cascade.onPointerHover = this.onPointerHover;
            __cascade.onPointerCancel = this.onPointerCancel;
            __cascade.onPointerPanZoomStart = this.onPointerPanZoomStart;
            __cascade.onPointerPanZoomUpdate = this.onPointerPanZoomUpdate;
            __cascade.onPointerPanZoomEnd = this.onPointerPanZoomEnd;
            __cascade.onPointerSignal = this.onPointerSignal;
            __cascade.behavior = this.behavior;
            return __cascade;        }))());
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        var listeners__278499 = new List<string>();
        properties.add(new global::Doroti.Generated.Framework.Foundation.IterableProperty<string>("listeners", listeners__278499.Cast<string>(), ifEmpty: "<none>"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Rendering.HitTestBehavior>("behavior", this.behavior));
    }

}

public class MouseRegion : SingleChildRenderObjectWidget
{
    public virtual global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerEnterEvent>? onEnter { get; private set; }
    public virtual global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerHoverEvent>? onHover { get; private set; }
    public virtual global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerExitEvent>? onExit { get; private set; }
    public virtual global::Doroti.Generated.Framework.Services.MouseCursor cursor { get; private set; } = default!;
    public virtual bool opaque { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.HitTestBehavior? hitTestBehavior { get; private set; }

    public MouseRegion(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerEnterEvent>? onEnter = null, global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerExitEvent>? onExit = null, global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerHoverEvent>? onHover = null, global::Doroti.Generated.Framework.Services.MouseCursor cursor = default!, bool opaque = true, global::Doroti.Generated.Framework.Rendering.HitTestBehavior? hitTestBehavior = null, Widget? child = null) : base(key: key, child: child)
    {
        global::Doroti.Generated.Framework.Services.MouseCursor __cursor = cursor ?? global::Doroti.Generated.Framework.Services.MouseCursor.defer;
        this.onEnter = onEnter;
        this.onExit = onExit;
        this.onHover = onHover;
        this.cursor = __cursor;
        this.opaque = opaque;
        this.hitTestBehavior = hitTestBehavior;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderMouseRegion(onEnter: (global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerEnterEvent>?)this.onEnter, onHover: (global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerHoverEvent>?)this.onHover, onExit: (global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerExitEvent>?)this.onExit, cursor: this.cursor, opaque: this.opaque, hitTestBehavior: this.hitTestBehavior));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderMouseRegion)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderMouseRegion>)(() =>
{            var __cascade = __renderObject;
            __cascade.onEnter = this.onEnter;
            __cascade.onHover = this.onHover;
            __cascade.onExit = this.onExit;
            __cascade.cursor = this.cursor;
            __cascade.opaque = this.opaque;
            __cascade.hitTestBehavior = this.hitTestBehavior;
            return __cascade;        }))());
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        var listeners__288286 = new List<string>();
        properties.add(new global::Doroti.Generated.Framework.Foundation.IterableProperty<string>("listeners", listeners__288286.Cast<string>(), ifEmpty: "<none>"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Services.MouseCursor>("cursor", this.cursor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("opaque", this.opaque, defaultValue: true));
    }

}

public class RepaintBoundary : SingleChildRenderObjectWidget
{
    public RepaintBoundary(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget? child = null) : base(key: key, child: child)
    {
    }

    public static RepaintBoundary CreateWrap(Widget child, long childIndex)
    {
        var __instance = new RepaintBoundary(default!, default!);
        return __instance;
    }

    public static List<RepaintBoundary> wrapAll(List<Widget> widgets) => new List<RepaintBoundary>();
    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(new global::Doroti.Generated.Framework.Rendering.RenderRepaintBoundary());
}

public class IgnorePointer : SingleChildRenderObjectWidget
{
    public virtual bool ignoring { get; private set; } = default!;
    public virtual bool? ignoringSemantics { get; private set; }

    public IgnorePointer(global::Doroti.Generated.Framework.Foundation.Key? key = null, bool ignoring = true, bool? ignoringSemantics = null, Widget? child = null) : base(key: key, child: child)
    {
        this.ignoring = ignoring;
        this.ignoringSemantics = ignoringSemantics;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderIgnorePointer(ignoring: this.ignoring, ignoringSemantics: this.ignoringSemantics));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderIgnorePointer)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderIgnorePointer>)(() =>
{            var __cascade = __renderObject;
            __cascade.ignoring = this.ignoring;
            __cascade.ignoringSemantics = this.ignoringSemantics;
            return __cascade;        }))());
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("ignoring", this.ignoring));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("ignoringSemantics", this.ignoringSemantics, defaultValue: null));
    }

}

public class AbsorbPointer : SingleChildRenderObjectWidget
{
    public virtual bool absorbing { get; private set; } = default!;
    public virtual bool? ignoringSemantics { get; private set; }

    public AbsorbPointer(global::Doroti.Generated.Framework.Foundation.Key? key = null, bool absorbing = true, bool? ignoringSemantics = null, Widget? child = null) : base(key: key, child: child)
    {
        this.absorbing = absorbing;
        this.ignoringSemantics = ignoringSemantics;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderAbsorbPointer(absorbing: this.absorbing, ignoringSemantics: this.ignoringSemantics));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderAbsorbPointer)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderAbsorbPointer>)(() =>
{            var __cascade = __renderObject;
            __cascade.absorbing = this.absorbing;
            __cascade.ignoringSemantics = this.ignoringSemantics;
            return __cascade;        }))());
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("absorbing", this.absorbing));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("ignoringSemantics", this.ignoringSemantics, defaultValue: null));
    }

}

public class MetaData : SingleChildRenderObjectWidget
{
    public virtual dynamic metaData { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.HitTestBehavior behavior { get; private set; } = default!;

    public MetaData(global::Doroti.Generated.Framework.Foundation.Key? key = null, dynamic metaData = default!, global::Doroti.Generated.Framework.Rendering.HitTestBehavior behavior = global::Doroti.Generated.Framework.Rendering.HitTestBehavior.deferToChild, Widget? child = null) : base(key: key, child: child)
    {
        this.metaData = metaData;
        this.behavior = behavior;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderMetaData(metaData: this.metaData, behavior: this.behavior));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderMetaData)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderMetaData>)(() =>
{            var __cascade = __renderObject;
            __cascade.metaData = this.metaData;
            __cascade.behavior = this.behavior;
            return __cascade;        }))());
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Rendering.HitTestBehavior>("behavior", this.behavior));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<object>("metaData", this.metaData));
    }

}

public class Semantics : _SemanticsBase__basic
{
    public Semantics(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget? child = null, bool container = false, bool explicitChildNodes = false, bool excludeSemantics = false, bool blockUserActions = false, bool? enabled = null, bool? @checked = null, bool? mixed = null, bool? selected = null, bool? toggled = null, bool? button = null, bool? slider = null, bool? keyboardKey = null, bool? link = null, DartUri? linkUrl = null, bool? header = null, long? headingLevel = null, bool? textField = null, bool? readOnly = null, bool? focusable = null, bool? focused = null, global::Doroti.Generated.Framework.Semantics.AccessibilityFocusBlockType? accessibilityFocusBlockType = null, bool? inMutuallyExclusiveGroup = null, bool? obscured = null, bool? multiline = null, bool? scopesRoute = null, bool? namesRoute = null, bool? hidden = null, bool? image = null, bool? liveRegion = null, bool? expanded = null, bool? isRequired = null, long? maxValueLength = null, long? currentValueLength = null, string? identifier = null, object? traversalParentIdentifier = null, object? traversalChildIdentifier = null, string? label = null, global::Doroti.Generated.Framework.Semantics.AttributedString? attributedLabel = null, string? value = null, global::Doroti.Generated.Framework.Semantics.AttributedString? attributedValue = null, string? increasedValue = null, global::Doroti.Generated.Framework.Semantics.AttributedString? attributedIncreasedValue = null, string? decreasedValue = null, global::Doroti.Generated.Framework.Semantics.AttributedString? attributedDecreasedValue = null, string? hint = null, global::Doroti.Generated.Framework.Semantics.AttributedString? attributedHint = null, string? tooltip = null, string? onTapHint = null, string? onLongPressHint = null, TextDirection? textDirection = null, global::Doroti.Generated.Framework.Semantics.SemanticsSortKey? sortKey = null, global::Doroti.Generated.Framework.Semantics.SemanticsTag? tagForChildren = null, global::System.Action? onTap = null, global::System.Action? onLongPress = null, global::System.Action? onScrollLeft = null, global::System.Action? onScrollRight = null, global::System.Action? onScrollUp = null, global::System.Action? onScrollDown = null, global::System.Action? onIncrease = null, global::System.Action? onDecrease = null, global::System.Action? onCopy = null, global::System.Action? onCut = null, global::System.Action? onPaste = null, global::System.Action? onDismiss = null, global::System.Action<bool>? onMoveCursorForwardByCharacter = null, global::System.Action<bool>? onMoveCursorBackwardByCharacter = null, global::System.Action<global::Doroti.Generated.Framework.Services.TextSelection>? onSetSelection = null, global::System.Action<string>? onSetText = null, global::System.Action? onDidGainAccessibilityFocus = null, global::System.Action? onDidLoseAccessibilityFocus = null, global::System.Action? onFocus = null, global::System.Action? onExpand = null, global::System.Action? onCollapse = null, DartMap<global::Doroti.Generated.Framework.Semantics.CustomSemanticsAction, global::System.Action>? customSemanticsActions = null, SemanticsRole? role = null, HashSet<string>? controlsNodes = null, SemanticsValidationResult validationResult = SemanticsValidationResult.none, SemanticsHitTestBehavior? hitTestBehavior = null, SemanticsInputType? inputType = null, Locale? localeForSubtree = null, string? minValue = null, string? maxValue = null) : base(key: key, child: child, container: container, explicitChildNodes: explicitChildNodes, excludeSemantics: excludeSemantics, blockUserActions: blockUserActions, enabled: enabled, @checked: @checked, mixed: mixed, selected: selected, toggled: toggled, button: button, slider: slider, keyboardKey: keyboardKey, link: link, linkUrl: linkUrl, header: header, headingLevel: headingLevel, textField: textField, readOnly: readOnly, focusable: focusable, focused: focused, accessibilityFocusBlockType: accessibilityFocusBlockType, inMutuallyExclusiveGroup: inMutuallyExclusiveGroup, obscured: obscured, multiline: multiline, scopesRoute: scopesRoute, namesRoute: namesRoute, hidden: hidden, image: image, liveRegion: liveRegion, expanded: expanded, isRequired: isRequired, maxValueLength: maxValueLength, currentValueLength: currentValueLength, identifier: identifier, traversalParentIdentifier: traversalParentIdentifier, traversalChildIdentifier: traversalChildIdentifier, label: label, attributedLabel: attributedLabel, value: value, attributedValue: attributedValue, increasedValue: increasedValue, attributedIncreasedValue: attributedIncreasedValue, decreasedValue: decreasedValue, attributedDecreasedValue: attributedDecreasedValue, hint: hint, attributedHint: attributedHint, tooltip: tooltip, onTapHint: onTapHint, onLongPressHint: onLongPressHint, textDirection: textDirection, sortKey: sortKey, tagForChildren: tagForChildren, onTap: onTap, onLongPress: onLongPress, onScrollLeft: onScrollLeft, onScrollRight: onScrollRight, onScrollUp: onScrollUp, onScrollDown: onScrollDown, onIncrease: onIncrease, onDecrease: onDecrease, onCopy: onCopy, onCut: onCut, onPaste: onPaste, onDismiss: onDismiss, onMoveCursorForwardByCharacter: onMoveCursorForwardByCharacter, onMoveCursorBackwardByCharacter: onMoveCursorBackwardByCharacter, onSetSelection: onSetSelection, onSetText: onSetText, onDidGainAccessibilityFocus: onDidGainAccessibilityFocus, onDidLoseAccessibilityFocus: onDidLoseAccessibilityFocus, onFocus: onFocus, onExpand: onExpand, onCollapse: onCollapse, customSemanticsActions: customSemanticsActions, role: role, controlsNodes: controlsNodes, validationResult: validationResult, hitTestBehavior: hitTestBehavior, inputType: inputType, localeForSubtree: localeForSubtree, minValue: minValue, maxValue: maxValue)
    {
    }

    public static Semantics CreateFromProperties(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget? child = null, bool container = false, bool explicitChildNodes = false, bool excludeSemantics = false, bool blockUserActions = false, Locale? localeForSubtree = null, global::Doroti.Generated.Framework.Semantics.SemanticsProperties properties = default!)
    {
        var __instance = new Semantics(default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!);
        return __instance;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderSemanticsAnnotations(container: this.container, explicitChildNodes: this.explicitChildNodes, excludeSemantics: this.excludeSemantics, blockUserActions: this.blockUserActions, properties: this.properties, localeForSubtree: this.localeForSubtree, textDirection: _getTextDirection(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderSemanticsAnnotations)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderSemanticsAnnotations>)(() =>
{            var __cascade = __renderObject;
            __cascade.container = this.container;
            __cascade.explicitChildNodes = this.explicitChildNodes;
            __cascade.excludeSemantics = this.excludeSemantics;
            __cascade.blockUserActions = this.blockUserActions;
            __cascade.properties = this.properties;
            __cascade.textDirection = _getTextDirection(context);
            __cascade.localeForSubtree = this.localeForSubtree;
            return __cascade;        }))());
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("container", this.container));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Semantics.SemanticsProperties>("properties", this.properties));
        this.properties.debugFillProperties(properties);
    }

}

public class MergeSemantics : SingleChildRenderObjectWidget
{
    public MergeSemantics(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget? child = null) : base(key: key, child: child)
    {
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(new global::Doroti.Generated.Framework.Rendering.RenderMergeSemantics());
}

public class BlockSemantics : SingleChildRenderObjectWidget
{
    public virtual bool blocking { get; private set; } = default!;

    public BlockSemantics(global::Doroti.Generated.Framework.Foundation.Key? key = null, bool blocking = true, Widget? child = null) : base(key: key, child: child)
    {
        this.blocking = blocking;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(new global::Doroti.Generated.Framework.Rendering.RenderBlockSemantics(blocking: this.blocking));
    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderBlockSemantics)(object)renderObject;
        __renderObject.blocking = this.blocking;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("blocking", this.blocking));
    }

}

public class ExcludeSemantics : SingleChildRenderObjectWidget
{
    public virtual bool excluding { get; private set; } = default!;

    public ExcludeSemantics(global::Doroti.Generated.Framework.Foundation.Key? key = null, bool excluding = true, Widget? child = null) : base(key: key, child: child)
    {
        this.excluding = excluding;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(new global::Doroti.Generated.Framework.Rendering.RenderExcludeSemantics(excluding: this.excluding));
    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderExcludeSemantics)(object)renderObject;
        __renderObject.excluding = this.excluding;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("excluding", this.excluding));
    }

}

public class IndexedSemantics : SingleChildRenderObjectWidget
{
    public virtual long index { get; private set; } = default!;

    public IndexedSemantics(global::Doroti.Generated.Framework.Foundation.Key? key = null, long index = default!, Widget? child = null) : base(key: key, child: child)
    {
        this.index = index;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(new global::Doroti.Generated.Framework.Rendering.RenderIndexedSemantics(index: this.index));
    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderIndexedSemantics)(object)renderObject;
        __renderObject.index = this.index;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<long>("index", this.index));
    }

}

public class KeyedSubtree : StatelessWidget
{
    public virtual Widget child { get; private set; } = default!;

    public KeyedSubtree(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget child = default!) : base(key: key)
    {
        this.child = child;
    }

    public static KeyedSubtree CreateWrap(Widget child, long childIndex)
    {
        var __instance = new KeyedSubtree(default!, default!);
        __instance.child = child;
        return __instance;
    }

    public static List<Widget> ensureUniqueKeysForList(List<Widget> items, long baseIndex = 0)
    {
        if (!System.Linq.Enumerable.Any(items))
        {
            return items;
        }
        var itemsWithUniqueKeys__314095 = new List<Widget>();
        DartRuntimePrimitives.Assert(() => !global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugItemsHaveDuplicateKeys(itemsWithUniqueKeys__314095.Cast<Widget>()));
        return itemsWithUniqueKeys__314095;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context) => this.child;
}

public class Builder : StatelessWidget
{
    public virtual global::System.Func<BuildContext, Widget> builder { get; private set; } = default!;

    public Builder(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Func<BuildContext, Widget> builder = default!) : base(key: key)
    {
        this.builder = builder;
    }

    public override Widget build(BuildContext context) => this.builder(context);
}

public delegate Widget StatefulWidgetBuilder(BuildContext context, global::System.Action<global::System.Action> setState);

public class StatefulBuilder : StatefulWidget
{
    public virtual global::System.Func<BuildContext, global::System.Action<global::System.Action>, Widget> builder { get; private set; } = default!;

    public StatefulBuilder(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Func<BuildContext, global::System.Action<global::System.Action>, Widget> builder = default!) : base(key: key)
    {
        this.builder = builder;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _StatefulBuilderState__basic());
}

internal class _StatefulBuilderState__basic : State<StatefulBuilder>
{
    public override Widget build(BuildContext context) => this.widget.builder(context, this.setState);
}

public class ColoredBox : SingleChildRenderObjectWidget
{
    public virtual Color color { get; private set; } = default!;
    public virtual bool isAntiAlias { get; private set; } = default!;

    public ColoredBox(Color color, bool isAntiAlias = true, Widget? child = null, global::Doroti.Generated.Framework.Foundation.Key? key = null) : base(child: child, key: key)
    {
        this.color = color;
        this.isAntiAlias = isAntiAlias;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _RenderColoredBox__basic(color: this.color, isAntiAlias: this.isAntiAlias));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        DartRuntimePrimitives.Ignore(((Func<_RenderColoredBox__basic>)(() =>
{            var __cascade = (((_RenderColoredBox__basic?)(object?)renderObject)!);
            __cascade.color = this.color;
            __cascade.isAntiAlias = this.isAntiAlias;
            return __cascade;        }))());
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Color>("color", this.color));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("isAntiAlias", this.isAntiAlias, defaultValue: true));
    }

}

internal class _RenderColoredBox__basic : global::Doroti.Generated.Framework.Rendering.RenderProxyBoxWithHitTestBehavior
{
    internal virtual Color _color { get; set; } = default!;
    internal virtual bool _isAntiAlias { get; set; } = default!;

    internal _RenderColoredBox__basic(Color color, bool isAntiAlias) : base(behavior: global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque)
    {
        this._color = color;
        this._isAntiAlias = isAntiAlias;
    }

    public virtual global::Doroti.Ui.Color color
    {
        get => this._color;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(__value, this._color)))
            {
                return;
            }
            _color = __value;
            markNeedsPaint();
        }
    }
    public virtual bool isAntiAlias
    {
        get => this._isAntiAlias;
        set
        {
            var __value = value;
            if ((__value == this._isAntiAlias))
            {
                return;
            }
            _isAntiAlias = __value;
            markNeedsPaint();
        }
    }
    public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        if ((this.size > Size.zero))
        {
            ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawRect((offset & this.size), ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.isAntiAlias = this.isAntiAlias;
            __cascade.color = this.color;
            return __cascade;        }))());
        }
        if ((this.child is not null))
        {
            context.paintChild(this.child!, offset);
        }
    }

}
