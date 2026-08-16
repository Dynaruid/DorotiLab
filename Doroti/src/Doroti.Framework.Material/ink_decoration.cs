// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/ink_decoration.dart
using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

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
        this.decoration = (decoration ?? (((color is not null) ? new global::Doroti.Framework.Painting.BoxDecoration(color: color) : null)));
        System.Diagnostics.Debug.Assert(((padding is null) || ((global::Doroti.Framework.Painting.EdgeInsetsGeometry)padding).isNonNegative));
        System.Diagnostics.Debug.Assert(((decoration is null) || decoration.debugAssertIsValid()));
        System.Diagnostics.Debug.Assert(((color is null) || (decoration is null)));
    }

    public static Ink CreateImage(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, dynamic image = default!, global::System.Action<object, global::System.Diagnostics.StackTrace?>? onImageError = null, ColorFilter? colorFilter = null, global::Doroti.Framework.Painting.BoxFit? fit = null, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, Rect? centerSlice = null, global::Doroti.Framework.Painting.ImageRepeat repeat = global::Doroti.Framework.Painting.ImageRepeat.noRepeat, bool matchTextDirection = false, double? width = null, double? height = null, global::Doroti.Framework.Widgets.Widget? child = null)
    {
        var __instance = new Ink(key: key, padding: padding, width: width, height: height, child: child);
        global::Doroti.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Framework.Painting.Alignment.center;
        __instance.padding = padding;
        __instance.width = width;
        __instance.height = height;
        __instance.child = child;
        __instance.decoration = new global::Doroti.Framework.Painting.BoxDecoration(image: new global::Doroti.Framework.Painting.DecorationImage(image: image, onError: (global::System.Action<object, global::System.Diagnostics.StackTrace?>?)onImageError, colorFilter: colorFilter, fit: fit, alignment: alignment, centerSlice: centerSlice, repeat: repeat, matchTextDirection: matchTextDirection));
        return __instance;
    }

    internal virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry _paddingIncludingDecoration
    {
        get
        {
            return ((this.padding, this.decoration?.padding) switch { (null, null) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Framework.Painting.EdgeInsets.zero), (null, global::Doroti.Framework.Painting.EdgeInsetsGeometry padding__9101) => padding__9101, (global::Doroti.Framework.Painting.EdgeInsetsGeometry padding__9154, null) => padding__9154, _ => this.padding!.add(this.decoration!.padding) });
            return default!;
        }
    }
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("padding", this.padding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.Decoration>("bg", this.decoration, defaultValue: null));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _InkState__ink_decoration());
}

internal class _InkState__ink_decoration : global::Doroti.Framework.Widgets.State<Ink>
{
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _boxKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create();
    internal virtual InkDecoration? _ink { get; set; } = default;

    internal virtual void _handleRemoved()
    {
        _ink = null;
    }

    public override void deactivate()
    {
        this._ink?.dispose();
        DartRuntimePrimitives.Assert(() => (this._ink is null));
        base.deactivate();
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if ((this._ink is null))
        {
            _ink = new InkDecoration(decoration: ((Ink)this.widget).decoration, isVisible: Visibility.of(context), configuration: global::Doroti.Framework.Widgets.ImageLibrary.createLocalImageConfiguration(context), controller: Material.of(context), referenceBox: ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((global::Doroti.Framework.Widgets.GlobalKey<IState>)this._boxKey).currentContext!.findRenderObject()!)!, onRemoved: () => this._handleRemoved());
        }
        else
        {
            this._ink!.decoration = ((Ink)this.widget).decoration;
            this._ink!.isVisible = Visibility.of(context);
            this._ink!.configuration = global::Doroti.Framework.Widgets.ImageLibrary.createLocalImageConfiguration(context);
        }
        return (((Ink)this.widget).child ?? new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: global::Doroti.Framework.Rendering.BoxConstraints.CreateExpand()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        global::Doroti.Framework.Widgets.Widget result__10780 = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Padding(key: this._boxKey, padding: ((Ink)this.widget)._paddingIncludingDecoration, child: new global::Doroti.Framework.Widgets.Builder(builder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)this._build)));
        if (((((Ink)this.widget).width is not null) || (((Ink)this.widget).height is not null)))
        {
            result__10780 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(width: ((Ink)this.widget).width, height: ((Ink)this.widget).height, child: result__10780));
        }
        return result__10780;
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
        this._configuration = configuration;
        this.decoration = decoration;
        this.isVisible = isVisible;
        this.controller.addInkFeature(this);
    }

    public virtual global::Doroti.Framework.Painting.Decoration? decoration
    {
        get => this._decoration;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._decoration)))
            {
                return;
            }
            _decoration = __value;
            this._painter?.dispose();
            _painter = this._decoration?.createBoxPainter(() => this._handleChanged());
            this.controller.markNeedsPaint();
        }
    }
    public virtual bool isVisible
    {
        get => this._isVisible;
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) == this._isVisible))
            {
                return;
            }
            _isVisible = DartRuntimePrimitives.RequireValue(__value);
            this.controller.markNeedsPaint();
        }
    }
    public virtual global::Doroti.Framework.Painting.ImageConfiguration configuration
    {
        get => this._configuration;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._configuration)))
            {
                return;
            }
            _configuration = __value;
            this.controller.markNeedsPaint();
        }
    }
    internal virtual void _handleChanged()
    {
        this.controller.markNeedsPaint();
    }

    public override void dispose()
    {
        this._painter?.dispose();
        base.dispose();
    }

    public override void paintFeature(Canvas canvas, Matrix4 transform)
    {
        if (((this._painter is null) || !this.isVisible))
        {
            return;
        }
        global::Doroti.Ui.Offset? originOffset__13767 = ((global::Doroti.Ui.Offset?)(object?)MatrixUtils.getAsTranslation(transform));
        global::Doroti.Framework.Painting.ImageConfiguration sizedConfiguration__13852 = ((global::Doroti.Framework.Painting.ImageConfiguration)(object?)this.configuration.copyWith(size: ((global::Doroti.Framework.Rendering.RenderBox)this.referenceBox).size));
        if ((originOffset__13767 is null))
        {
            canvas.save();
            canvas.transform(transform.storage);
            this._painter!.paint(canvas, Offset.zero, sizedConfiguration__13852);
            canvas.restore();
        }
        else
        {
            this._painter!.paint(canvas, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(originOffset__13767)), sizedConfiguration__13852);
        }
    }

}
