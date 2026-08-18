// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/annotated_region.dart
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

public class AnnotatedRegion<T> : SingleChildRenderObjectWidget
{
    public virtual T value { get; private set; } = default!;
    public virtual bool sized { get; private set; } = default!;

    public AnnotatedRegion(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!, T value = default!, bool sized = true) : base(key: key, child: child)
    {
        this.value = value;
        this.sized = sized;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new global::Doroti.Framework.Rendering.RenderAnnotatedRegion<T>(value: this.value, sized: this.sized));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderAnnotatedRegion<T>)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Rendering.RenderAnnotatedRegion<T>>)(() =>
{
    var __cascade = __renderObject;
    __cascade.value = this.value;
    __cascade.sized = this.sized;
    return __cascade;
}))());
    }

}

