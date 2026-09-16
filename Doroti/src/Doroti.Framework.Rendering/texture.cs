// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/texture.dart
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

public class TextureBox : RenderBox
{
    internal virtual long _textureId { get; set; } = default!;
    internal virtual bool _freeze { get; set; } = default!;
    internal virtual FilterQuality _filterQuality { get; set; } = default!;

    public TextureBox(long textureId, bool freeze = false, FilterQuality filterQuality = FilterQuality.low)
    {
        _textureId = textureId;
        _freeze = freeze;
        _filterQuality = filterQuality;
    }

    public virtual long textureId
    {
        get => _textureId;
        set
        {
            var __value = value;
            if (__value != _textureId)
            {
                _textureId = __value;
                markNeedsPaint();
            }
        }
    }
    public virtual bool freeze
    {
        get => _freeze;
        set
        {
            var __value = value;
            if (__value != _freeze)
            {
                _freeze = __value;
                markNeedsPaint();
            }
        }
    }
    public virtual FilterQuality filterQuality
    {
        get => _filterQuality;
        set
        {
            var __value = value;
            if (!Equals(__value, _filterQuality))
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
        return constraints.biggest;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestSelf(Offset position) => true;
    public override void paint(PaintingContext context, Offset offset)
    {
        context.addLayer(new TextureLayer(rect: Rect.fromLTWH(offset.dx, offset.dy, size.width, size.height), textureId: _textureId, freeze: freeze, filterQuality: _filterQuality));
    }

}

