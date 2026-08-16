// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/decorated_sliver.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Rendering;

public class RenderDecoratedSliver : RenderProxySliver
{
    internal virtual global::Doroti.Framework.Painting.Decoration _decoration { get; set; } = default!;
    internal virtual DecorationPosition _position { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.ImageConfiguration _configuration { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.BoxPainter? _painter { get; set; } = default;

    public RenderDecoratedSliver(global::Doroti.Framework.Painting.Decoration decoration, DecorationPosition position = DecorationPosition.background, global::Doroti.Framework.Painting.ImageConfiguration configuration = default!)
    {
        global::Doroti.Framework.Painting.ImageConfiguration __configuration = configuration ?? global::Doroti.Framework.Painting.ImageConfiguration.empty;
        this._decoration = decoration;
        this._position = position;
        this._configuration = __configuration;
    }

    public virtual global::Doroti.Framework.Painting.Decoration decoration
    {
        get => this._decoration;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this.decoration)))
            {
                return;
            }
            _decoration = __value;
            this._painter?.dispose();
            _painter = this.decoration.createBoxPainter((Action)markNeedsPaint);
            markNeedsPaint();
        }
    }
    public virtual DecorationPosition position
    {
        get => this._position;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this.position)))
            {
                return;
            }
            _position = __value;
            markNeedsPaint();
        }
    }
    public virtual global::Doroti.Framework.Painting.ImageConfiguration configuration
    {
        get => this._configuration;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this.configuration)))
            {
                return;
            }
            _configuration = __value;
            markNeedsPaint();
        }
    }
    public override void attach(PipelineOwner owner)
    {
        _painter = this.decoration.createBoxPainter((Action)markNeedsPaint);
        base.attach(owner);
    }

    public override void detach()
    {
        this._painter?.dispose();
        _painter = null;
        base.detach();
    }

    public override void dispose()
    {
        this._painter?.dispose();
        _painter = null;
        base.dispose();
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (((child is null) || !child!.geometry!.visible))
        {
            return;
        }
        global::Doroti.Ui.Rect paintRect__2850 = getMaxPaintRect();
        void paintDecoration()
        {
            this._painter!.paint(((PaintingContext)context).canvas, (offset + paintRect__2850.topLeft), this.configuration.copyWith(size: paintRect__2850.size));
        }
        switch (this.position)
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
        properties.add(((Diagnosticable)this._decoration).toDiagnosticsNode(name: "decoration"));
        properties.add(new DiagnosticsProperty<global::Doroti.Framework.Painting.ImageConfiguration>("configuration", this.configuration));
    }

}

