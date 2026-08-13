// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/texture.dart
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

public class Texture : LeafRenderObjectWidget
{
    public virtual long textureId { get; private set; } = default!;
    public virtual bool freeze { get; private set; } = default!;
    public virtual FilterQuality filterQuality { get; private set; } = default!;

    public Texture(global::Doroti.Generated.Framework.Foundation.Key? key = null, long textureId = default!, bool freeze = false, FilterQuality filterQuality = FilterQuality.low) : base(key: key)
    {
        this.textureId = textureId;
        this.freeze = freeze;
        this.filterQuality = filterQuality;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(new global::Doroti.Generated.Framework.Rendering.TextureBox(textureId: this.textureId, freeze: this.freeze, filterQuality: this.filterQuality));
    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.TextureBox)(object)renderObject;
        __renderObject.textureId = this.textureId;
        __renderObject.freeze = this.freeze;
        __renderObject.filterQuality = this.filterQuality;
    }

}

