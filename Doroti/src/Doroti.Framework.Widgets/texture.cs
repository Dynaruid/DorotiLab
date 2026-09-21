// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/texture.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class Texture : LeafRenderObjectWidget
{
    public virtual long textureId { get; private set; } = default!;
    public virtual bool freeze { get; private set; } = default!;
    public virtual FilterQuality filterQuality { get; private set; } = default!;

    public Texture(
        Key? key = null,
        long textureId = default!,
        bool freeze = false,
        FilterQuality filterQuality = FilterQuality.low
    )
        : base(key: key)
    {
        this.textureId = textureId;
        this.freeze = freeze;
        this.filterQuality = filterQuality;
    }

    public override RenderObject createRenderObject(BuildContext context) =>
        DartRuntimePrimitives.ConvertValue<RenderObject>(
            new TextureBox(textureId: textureId, freeze: freeze, filterQuality: filterQuality)
        );

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (TextureBox)renderObject;
        __renderObject.textureId = textureId;
        __renderObject.freeze = freeze;
        __renderObject.filterQuality = filterQuality;
    }
}
