// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/texture.dart
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

namespace Doroti.Generated.Framework.Rendering;

public class TextureBox : RenderBox
{
    internal virtual long _textureId { get; set; } = default!;
    internal virtual bool _freeze { get; set; } = default!;
    internal virtual FilterQuality _filterQuality { get; set; } = default!;

    public TextureBox(long textureId, bool freeze = false, FilterQuality filterQuality = FilterQuality.low)
    {
        this._textureId = textureId;
        this._freeze = freeze;
        this._filterQuality = filterQuality;
    }

    public virtual long textureId
    {
        get => this._textureId;
        set
        {
            var __value = value;
            if ((__value != this._textureId))
            {
                _textureId = __value;
                markNeedsPaint();
            }
        }
    }
    public virtual bool freeze
    {
        get => this._freeze;
        set
        {
            var __value = value;
            if ((__value != this._freeze))
            {
                _freeze = __value;
                markNeedsPaint();
            }
        }
    }
    public virtual global::Doroti.Ui.FilterQuality filterQuality
    {
        get => this._filterQuality;
        set
        {
            var __value = value;
            if ((!object.Equals(__value, this._filterQuality)))
            {
                _filterQuality = __value;
                markNeedsPaint();
            }
        }
    }
    public override bool sizedByParent => true;
    public override bool alwaysNeedsCompositing => true;
    public override bool isRepaintBoundary => true;
    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return ((BoxConstraints)constraints).biggest;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestSelf(Offset position) => true;
    public override void paint(PaintingContext context, Offset offset)
    {
        context.addLayer(new TextureLayer(rect: global::Doroti.Ui.Rect.fromLTWH(offset.dx, offset.dy, size.width, size.height), textureId: this._textureId, freeze: this.freeze, filterQuality: this._filterQuality));
    }

}

