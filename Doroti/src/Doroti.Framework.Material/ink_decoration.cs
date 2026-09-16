// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/ink_decoration.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class Ink : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual global::Doroti.Framework.Painting.Decoration? decoration { get; private set; }
    public virtual double? width { get; private set; }
    public virtual double? height { get; private set; }

    public Ink(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, Color? color = null, global::Doroti.Framework.Painting.Decoration? decoration = null, double? width = null, double? height = null, global::Doroti.Framework.Widgets.Widget? child = null) : base(key: key)
    {
        this.padding = padding;
        this.width = width;
        this.height = height;
        this.child = child;
        this.decoration = decoration ?? ((color is not null) ? new global::Doroti.Framework.Painting.BoxDecoration(color: color) : null);
        System.Diagnostics.Debug.Assert((padding is null) || padding.isNonNegative);
        System.Diagnostics.Debug.Assert((decoration is null) || decoration.debugAssertIsValid());
        System.Diagnostics.Debug.Assert((color is null) || (decoration is null));
    }

    public static Ink CreateImage(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Painting.IImageProvider image = default!, global::System.Action<object, global::System.Diagnostics.StackTrace?>? onImageError = null, ColorFilter? colorFilter = null, global::Doroti.Framework.Painting.BoxFit? fit = null, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, Rect? centerSlice = null, global::Doroti.Framework.Painting.ImageRepeat repeat = ImageRepeat.noRepeat, bool matchTextDirection = false, double? width = null, double? height = null, global::Doroti.Framework.Widgets.Widget? child = null)
    {
        var __instance = new Ink(key: key, padding: padding, width: width, height: height, child: child);
        global::Doroti.Framework.Painting.AlignmentGeometry __alignment = alignment ?? Alignment.center;
        __instance.padding = padding;
        __instance.width = width;
        __instance.height = height;
        __instance.child = child;
        __instance.decoration = new global::Doroti.Framework.Painting.BoxDecoration(image: new global::Doroti.Framework.Painting.DecorationImage(image: image, onError: onImageError, colorFilter: colorFilter, fit: fit, alignment: __alignment, centerSlice: centerSlice, repeat: repeat, matchTextDirection: matchTextDirection));
        return __instance;
    }

    internal virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry _paddingIncludingDecoration
    {
        get
        {
            return (padding, decoration?.padding) switch { (null, null) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(EdgeInsets.zero), (null, global::Doroti.Framework.Painting.EdgeInsetsGeometry paddingLocal) => paddingLocal, (global::Doroti.Framework.Painting.EdgeInsetsGeometry paddingAlternate, null) => paddingAlternate, _ => padding!.add(decoration!.padding) };
        }
    }
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("padding", padding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.Decoration>("bg", decoration, defaultValue: null));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _InkState__ink_decoration());
}

internal class _InkState__ink_decoration : global::Doroti.Framework.Widgets.State<Ink>
{
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _boxKey { get; private set; } = GlobalKey<IState>.Create();
    internal virtual InkDecoration? _ink { get; set; } = default;

    internal virtual void _handleRemoved()
    {
        _ink = null;
    }

    public override void deactivate()
    {
        _ink?.dispose();
        DartRuntimePrimitives.Assert(() => _ink is null);
        base.deactivate();
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if (_ink is null)
        {
            _ink = new InkDecoration(decoration: widget.decoration, isVisible: Visibility.of(context), configuration: ImageLibrary.createLocalImageConfiguration(context), controller: Material.of(context), referenceBox: ((global::Doroti.Framework.Rendering.RenderBox?)_boxKey.currentContext!.findRenderObject()!)!, onRemoved: () => _handleRemoved());
        }
        else
        {
            _ink!.decoration = widget.decoration;
            _ink!.isVisible = Visibility.of(context);
            _ink!.configuration = ImageLibrary.createLocalImageConfiguration(context);
        }
        return widget.child ?? new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: BoxConstraints.CreateExpand());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        global::Doroti.Framework.Widgets.Widget result = new global::Doroti.Framework.Widgets.Padding(key: _boxKey, padding: widget._paddingIncludingDecoration, child: new global::Doroti.Framework.Widgets.Builder(builder: _build));
        if ((widget.width is not null) || (widget.height is not null))
        {
            result = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(width: widget.width, height: widget.height, child: result));
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class InkDecoration : InkFeature
{
    internal virtual global::Doroti.Framework.Painting.BoxPainter? _painter { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.Decoration? _decoration { get; set; } = default;
    internal virtual bool _isVisible { get; set; } = true;
    internal virtual global::Doroti.Framework.Painting.ImageConfiguration _configuration { get; set; } = default!;

    public InkDecoration(global::Doroti.Framework.Painting.Decoration? decoration, bool isVisible = true, global::Doroti.Framework.Painting.ImageConfiguration configuration = default!, MaterialInkController controller = default!, global::Doroti.Framework.Rendering.RenderBox referenceBox = default!, global::System.Action? onRemoved = null) : base(controller: controller, referenceBox: referenceBox, onRemoved: onRemoved)
    {
        _configuration = configuration;
        this.decoration = decoration;
        this.isVisible = isVisible;
        this.controller.addInkFeature(this);
    }

    public virtual global::Doroti.Framework.Painting.Decoration? decoration
    {
        get => _decoration;
        set
        {
            var __value = value;
            if (Equals(__value, _decoration))
            {
                return;
            }
            _decoration = __value;
            _painter?.dispose();
            _painter = _decoration?.createBoxPainter(() => _handleChanged());
            controller.markNeedsPaint();
        }
    }
    public virtual bool isVisible
    {
        get => _isVisible;
        set
        {
            var __value = value;
            if (DartRuntimePrimitives.RequireValue(__value) == _isVisible)
            {
                return;
            }
            _isVisible = DartRuntimePrimitives.RequireValue(__value);
            controller.markNeedsPaint();
        }
    }
    public virtual global::Doroti.Framework.Painting.ImageConfiguration configuration
    {
        get => _configuration;
        set
        {
            var __value = value;
            if (Equals(__value, _configuration))
            {
                return;
            }
            _configuration = __value;
            controller.markNeedsPaint();
        }
    }
    internal virtual void _handleChanged()
    {
        controller.markNeedsPaint();
    }

    public override void dispose()
    {
        _painter?.dispose();
        base.dispose();
    }

    public override void paintFeature(Canvas canvas, Matrix4 transform)
    {
        if ((_painter is null) || !isVisible)
        {
            return;
        }
        global::Doroti.Ui.Offset? originOffset = MatrixUtils.getAsTranslation(transform);
        global::Doroti.Framework.Painting.ImageConfiguration sizedConfiguration = configuration.copyWith(size: referenceBox.size);
        if (originOffset is null)
        {
            canvas.save();
            canvas.transform(transform.storage);
            _painter!.paint(canvas, Offset.zero, sizedConfiguration);
            canvas.restore();
        }
        else
        {
            _painter!.paint(canvas, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(originOffset)), sizedConfiguration);
        }
    }

}
