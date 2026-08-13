// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/_web_image_web.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public class ImgElementPlatformViewIo : StatelessWidget
{
    internal const string _viewType = "Flutter__ImgElementImage__";
    internal static bool _registered = false;
    public virtual string? src { get; private set; }

    public ImgElementPlatformViewIo(string? src, global::Doroti.Generated.Framework.Foundation.Key? key = null) : base(key: key)
    {
        this.src = src;
    }

    internal static void _register()
    {
        DartRuntimePrimitives.Assert(() => !_registered);
        _registered = true;
        Dart_ui_webLibrary.platformViewRegistry.registerViewFactory(global::Doroti.Generated.Framework.Widgets._platform_selectable_region_context_menu_webLibrary._viewType, ((viewId, arg1) => {
var paramsMap__1130 = DartRuntimePrimitives.ConvertMap<object?, object?>((System.Collections.IDictionary)@params!);
var img__1319 = ((HTMLImageElement?)(object?)WebLibrary.document.createElement("img"))!;
img__1319.src = ((string?)(object?)paramsMap__1130.GetValueOrDefault("src")!)!;
DartRuntimePrimitives.Ignore(((Func<CSSStyleDeclaration>)(() =>
{            var __cascade = img__1319.style;
            __cascade.width = "100%";
            __cascade.height = "100%";
            __cascade.pointerEvents = "none";
            return __cascade;        }))());
return img__1319;
throw new InvalidOperationException("Dart closure completed without a value.");
}));
    }

    public override Widget build(BuildContext context)
    {
        if ((this.src is null))
        {
            return ((Widget)(object?)SizedBox.CreateExpand());
        }
        return ((Widget)(object?)new HtmlElementView(viewType: global::Doroti.Generated.Framework.Widgets._platform_selectable_region_context_menu_webLibrary._viewType, creationParams: new DartMap<string, string?> { ["src"] = this.src }, hitTestBehavior: global::Doroti.Generated.Framework.Rendering.PlatformViewHitTestBehavior.transparent));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RawWebImageIo : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Painting.WebImageInfoIo image { get; private set; } = default!;
    public virtual string? debugImageLabel { get; private set; }
    public virtual double? width { get; private set; }
    public virtual double? height { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.BoxFit? fit { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;
    public virtual bool matchTextDirection { get; private set; } = default!;

    public RawWebImageIo(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.WebImageInfoIo image = default!, string? debugImageLabel = null, double? width = null, double? height = null, global::Doroti.Generated.Framework.Painting.BoxFit? fit = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, bool matchTextDirection = false) : base(key: key, child: new ImgElementPlatformViewIo(((HTMLImageElement)((dynamic)image).htmlImage).src))
    {
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center;
        this.image = image;
        this.debugImageLabel = debugImageLabel;
        this.width = width;
        this.height = height;
        this.fit = fit;
        this.alignment = __alignment;
        this.matchTextDirection = matchTextDirection;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new RenderWebImageIo(image: ((HTMLImageElement)((dynamic)this.image).htmlImage), width: this.width, height: this.height, fit: this.fit, alignment: this.alignment, matchTextDirection: this.matchTextDirection, textDirection: ((this.matchTextDirection || (this.alignment is not global::Doroti.Generated.Framework.Painting.Alignment)) ? Directionality.of(context) : null)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (RenderWebImageIo)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderWebImageIo>)(() =>
{            var __cascade = __renderObject;
            __cascade.image = ((HTMLImageElement)((dynamic)this.image).htmlImage);
            __cascade.width = this.width;
            __cascade.height = this.height;
            __cascade.fit = this.fit;
            __cascade.alignment = this.alignment;
            __cascade.matchTextDirection = this.matchTextDirection;
            __cascade.textDirection = ((this.matchTextDirection || (this.alignment is not global::Doroti.Generated.Framework.Painting.Alignment)) ? Directionality.of(context) : null);
            return __cascade;        }))());
    }

}

public class RenderWebImageIo : global::Doroti.Generated.Framework.Rendering.RenderShiftedBox
{
    internal virtual global::Doroti.Generated.Framework.Painting.Alignment? _resolvedAlignment { get; set; } = default;
    internal virtual bool? _flipHorizontally { get; set; } = default;
    internal virtual bool _needsClip { get; set; } = false;
    internal virtual bool _matchTextDirection { get; set; } = default!;
    internal virtual TextDirection? _textDirection { get; set; } = default;
    internal virtual HTMLImageElement _image { get; set; } = default!;
    internal virtual double? _width { get; set; } = default;
    internal virtual double? _height { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Painting.BoxFit? _fit { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry _alignment { get; set; } = default!;

    public RenderWebImageIo(global::Doroti.Generated.Framework.Rendering.RenderBox? child = null, HTMLImageElement image = default!, double? width = null, double? height = null, global::Doroti.Generated.Framework.Painting.BoxFit? fit = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, bool matchTextDirection = false, TextDirection? textDirection = null) : base(child)
    {
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center;
        this._image = image;
        this._width = width;
        this._height = height;
        this._fit = fit;
        this._alignment = __alignment;
        this._matchTextDirection = matchTextDirection;
        this._textDirection = textDirection;
    }

    internal virtual void _resolve()
    {
        if ((this._resolvedAlignment is not null))
        {
            return;
        }
        _resolvedAlignment = this.alignment.resolve(this.textDirection);
        _flipHorizontally = (this.matchTextDirection && (object.Equals(this.textDirection, TextDirection.rtl)));
    }

    internal virtual void _markNeedResolution()
    {
        _resolvedAlignment = null;
        _flipHorizontally = null;
        markNeedsLayout();
    }

    public virtual bool matchTextDirection
    {
        get => this._matchTextDirection;
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) == this._matchTextDirection))
            {
                return;
            }
            _matchTextDirection = DartRuntimePrimitives.RequireValue(__value);
            _markNeedResolution();
        }
    }
    public virtual global::Doroti.Flutter.Ui.TextDirection? textDirection
    {
        get => this._textDirection;
        set
        {
            var __value = value;
            if ((object.Equals(this._textDirection, __value)))
            {
                return;
            }
            _textDirection = __value;
            _markNeedResolution();
        }
    }
    public virtual HTMLImageElement image
    {
        get => this._image;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._image)))
            {
                return;
            }
            if ((__value.src == this._image.src))
            {
                return;
            }
            bool sizeChanged__6847 = ((this._image.naturalWidth != __value.naturalWidth) || (this._image.naturalHeight != __value.naturalHeight));
            _image = __value;
            markNeedsPaint();
            if ((sizeChanged__6847 && (((this._width is null) || (this._height is null)))))
            {
                markNeedsLayout();
            }
        }
    }
    public virtual double? width
    {
        get => this._width;
        set
        {
            var __value = value;
            if ((__value == this._width))
            {
                return;
            }
            _width = __value;
            markNeedsLayout();
        }
    }
    public virtual double? height
    {
        get => this._height;
        set
        {
            var __value = value;
            if ((__value == this._height))
            {
                return;
            }
            _height = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Generated.Framework.Painting.BoxFit? fit
    {
        get => this._fit;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._fit)))
            {
                return;
            }
            _fit = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment
    {
        get => this._alignment;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._alignment)))
            {
                return;
            }
            _alignment = __value;
            _markNeedResolution();
        }
    }
    internal virtual global::Doroti.Flutter.Ui.Size _sizeForConstraints(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
        constraints = global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateTightFor(width: this._width, height: this._height).enforce(constraints);
        return ((global::Doroti.Flutter.Ui.Size)(object?)constraints.constrainSizeAndAttemptToPreserveAspectRatio(new global::Doroti.Flutter.Ui.Size(this._image.naturalWidth.toDouble(), this._image.naturalHeight.toDouble())));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        DartRuntimePrimitives.Assert(() => (height >= 0.0));
        if (((this._width is null) && (this._height is null)))
        {
            return 0.0;
        }
        return DartRuntimePrimitives.RequireValue(_sizeForConstraints(global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateTightForFinite(height: DartRuntimePrimitives.RequireValue(height))).width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        DartRuntimePrimitives.Assert(() => (height >= 0.0));
        return DartRuntimePrimitives.RequireValue(_sizeForConstraints(global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateTightForFinite(height: DartRuntimePrimitives.RequireValue(height))).width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        DartRuntimePrimitives.Assert(() => (width >= 0.0));
        if (((this._width is null) && (this._height is null)))
        {
            return 0.0;
        }
        return DartRuntimePrimitives.RequireValue(_sizeForConstraints(global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateTightForFinite(width: DartRuntimePrimitives.RequireValue(width))).height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        DartRuntimePrimitives.Assert(() => (width >= 0.0));
        return DartRuntimePrimitives.RequireValue(_sizeForConstraints(global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateTightForFinite(width: DartRuntimePrimitives.RequireValue(width))).height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestSelf(Offset position) => true;
    public override Size computeDryLayout(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
        return _sizeForConstraints(constraints);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        _resolve();
        DartRuntimePrimitives.Assert(() => (this._resolvedAlignment is not null));
        DartRuntimePrimitives.Assert(() => (this._flipHorizontally is not null));
        size = _sizeForConstraints(this.constraints);
        if ((this.child is null))
        {
            return;
        }
        var inputSize__11026 = new global::Doroti.Flutter.Ui.Size(this.image.naturalWidth.toDouble(), this.image.naturalHeight.toDouble());
        global::Doroti.Generated.Framework.Painting.BoxFit resolvedFit__11124 = (this.fit ?? global::Doroti.Generated.Framework.Painting.BoxFit.scaleDown);
        global::Doroti.Generated.Framework.Painting.FittedSizes fittedSizes__11185 = global::Doroti.Generated.Framework.Painting.Box_fitLibrary.applyBoxFit(resolvedFit__11124, inputSize__11026, this.size);
        if ((object.Equals(((global::Doroti.Generated.Framework.Painting.FittedSizes)fittedSizes__11185).source, inputSize__11026)))
        {
            global::Doroti.Flutter.Ui.Size childSize__11527 = ((global::Doroti.Flutter.Ui.Size)(object?)((global::Doroti.Generated.Framework.Painting.FittedSizes)fittedSizes__11185).destination);
            this.child!.layout(global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateTight(childSize__11527));
            double halfWidthDelta__11919 = (((this.size.width - childSize__11527.width)) / 2.0);
            double halfHeightDelta__11993 = (((this.size.height - childSize__11527.height)) / 2.0);
            double dx__12070 = (halfWidthDelta__11919 + (((DartRuntimePrimitives.RequireValue(this._flipHorizontally) ? -this._resolvedAlignment!.x : this._resolvedAlignment!.x)) * halfWidthDelta__11919));
            double dy__12219 = (halfHeightDelta__11993 + (this._resolvedAlignment!.y * halfHeightDelta__11993));
            var childParentData__12295 = ((global::Doroti.Generated.Framework.Rendering.BoxParentData?)(object?)this.child!.parentData!)!;
            childParentData__12295.offset = new global::Doroti.Flutter.Ui.Offset(dx__12070, dy__12219);
            _needsClip = false;
        }
        else
        {
            global::Doroti.Flutter.Ui.Size sourceSize__12917 = ((global::Doroti.Flutter.Ui.Size)(object?)((global::Doroti.Generated.Framework.Painting.FittedSizes)fittedSizes__11185).source);
            global::Doroti.Flutter.Ui.Size destinationSize__12967 = ((global::Doroti.Flutter.Ui.Size)(object?)((global::Doroti.Generated.Framework.Painting.FittedSizes)fittedSizes__11185).destination);
            DartRuntimePrimitives.Assert(() => (((sourceSize__12917.aspectRatio - destinationSize__12967.aspectRatio)).abs() < global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance));
            double scale__13997 = (destinationSize__12967.width / sourceSize__12917.width);
            global::Doroti.Flutter.Ui.Size childSize__14064 = ((global::Doroti.Flutter.Ui.Size)(object?)(inputSize__11026 * scale__13997));
            this.child!.layout(global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateTight(childSize__14064));
            global::Doroti.Flutter.Ui.Rect sourceRect__14845 = ((global::Doroti.Flutter.Ui.Rect)(object?)this._resolvedAlignment!.inscribe(sourceSize__12917, (Offset.zero & inputSize__11026)));
            global::Doroti.Flutter.Ui.Offset childOffset__14944 = ((global::Doroti.Flutter.Ui.Offset)(object?)(new global::Doroti.Flutter.Ui.Offset(-sourceRect__14845.left, -sourceRect__14845.top) * scale__13997));
            var childParentData__15021 = ((global::Doroti.Generated.Framework.Rendering.BoxParentData?)(object?)this.child!.parentData!)!;
            childParentData__15021.offset = childOffset__14944;
            _needsClip = true;
        }
    }

    public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        if ((this.child is null))
        {
            return;
        }
        if (this._needsClip)
        {
            global::Doroti.Flutter.Ui.Rect destinationRect__15307 = ((global::Doroti.Flutter.Ui.Rect)(object?)(Offset.zero & this.size));
            context.pushClipRect(this.needsCompositing, offset, destinationRect__15307, (global::System.Action<global::Doroti.Generated.Framework.Rendering.PaintingContext, Offset>)base.paint);
        }
        else
        {
            base.paint(context, offset);
        }
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<HTMLImageElement>("image", this.image));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("width", this.width, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("height", this.height, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Painting.BoxFit>("fit", this.fit, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.AlignmentGeometry>("alignment", this.alignment, defaultValue: null));
    }

}

