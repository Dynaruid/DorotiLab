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

    public Texture(global::Doroti.Framework.Foundation.Key? key = null, long textureId = default!, bool freeze = false, FilterQuality filterQuality = FilterQuality.low) : base(key: key)
    {
        this.textureId = textureId;
        this.freeze = freeze;
        this.filterQuality = filterQuality;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(new global::Doroti.Framework.Rendering.TextureBox(textureId: this.textureId, freeze: this.freeze, filterQuality: this.filterQuality));
    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.TextureBox)renderObject;
        __renderObject.textureId = this.textureId;
        __renderObject.freeze = this.freeze;
        __renderObject.filterQuality = this.filterQuality;
    }

}

