// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/decorated_sliver.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Widgets;

public class DecoratedSliver : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Framework.Painting.Decoration decoration { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.DecorationPosition position { get; private set; } = default!;

    public DecoratedSliver(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.Decoration decoration = default!, global::Doroti.Framework.Rendering.DecorationPosition position = global::Doroti.Framework.Rendering.DecorationPosition.background, Widget? sliver = null) : base(key: key, child: sliver)
    {
        this.decoration = decoration;
        this.position = position;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new global::Doroti.Framework.Rendering.RenderDecoratedSliver(decoration: this.decoration, position: this.position, configuration: global::Doroti.Framework.Widgets.ImageLibrary.createLocalImageConfiguration(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderDecoratedSliver)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Rendering.RenderDecoratedSliver>)(() =>
{            var __cascade = __renderObject;
            __cascade.decoration = this.decoration;
            __cascade.position = this.position;
            __cascade.configuration = global::Doroti.Framework.Widgets.ImageLibrary.createLocalImageConfiguration(context);
            return __cascade;        }))());
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string label__3236 = (this.position switch { global::Doroti.Framework.Rendering.DecorationPosition.background => "bg", global::Doroti.Framework.Rendering.DecorationPosition.foreground => "fg", _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Rendering.DecorationPosition>("position", this.position, level: global::Doroti.Framework.Foundation.DiagnosticLevel.hidden));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.Decoration>(label__3236, this.decoration));
    }

}

