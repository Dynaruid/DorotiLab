// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/decorated_sliver.dart
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

public class RenderDecoratedSliver : RenderProxySliver
{
    internal virtual Decoration _decoration { get; set; } = default!;
    internal virtual DecorationPosition _position { get; set; } = default!;
    internal virtual ImageConfiguration _configuration { get; set; } = default!;
    internal virtual BoxPainter? _painter { get; set; } = default;

    public RenderDecoratedSliver(
        Decoration decoration,
        DecorationPosition position = DecorationPosition.background,
        ImageConfiguration configuration = default!
    )
    {
        ImageConfiguration __configuration = configuration ?? ImageConfiguration.empty;
        _decoration = decoration;
        _position = position;
        _configuration = __configuration;
    }

    public virtual Decoration decoration
    {
        get => _decoration;
        set
        {
            var __value = value;
            if (Equals(__value, decoration))
            {
                return;
            }
            _decoration = __value;
            _painter?.dispose();
            _painter = decoration.createBoxPainter(markNeedsPaint);
            markNeedsPaint();
        }
    }
    public virtual DecorationPosition position
    {
        get => _position;
        set
        {
            var __value = value;
            if (Equals(__value, position))
            {
                return;
            }
            _position = __value;
            markNeedsPaint();
        }
    }
    public virtual ImageConfiguration configuration
    {
        get => _configuration;
        set
        {
            var __value = value;
            if (Equals(__value, configuration))
            {
                return;
            }
            _configuration = __value;
            markNeedsPaint();
        }
    }

    public override void attach(PipelineOwner owner)
    {
        _painter = decoration.createBoxPainter(markNeedsPaint);
        base.attach(owner);
    }

    public override void detach()
    {
        _painter?.dispose();
        _painter = null;
        base.detach();
    }

    public override void dispose()
    {
        _painter?.dispose();
        _painter = null;
        base.dispose();
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if ((child is null) || !child!.geometry!.visible)
        {
            return;
        }
        Rect paintRect = getMaxPaintRect();
        void paintDecoration()
        {
            _painter!.paint(
                context.canvas,
                offset + paintRect.topLeft,
                configuration.copyWith(size: paintRect.size)
            );
        }
        switch (position)
        {
            case DecorationPosition.background:
            {
                paintDecoration();
                context.paintChild(child!, offset);
                break;
            }
            case DecorationPosition.foreground:
            {
                context.paintChild(child!, offset);
                paintDecoration();
                break;
            }
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(((Diagnosticable)_decoration).toDiagnosticsNode(name: "decoration"));
        properties.add(new DiagnosticsProperty<ImageConfiguration>("configuration", configuration));
    }
}
