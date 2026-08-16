// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/orientation_builder.dart
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

public delegate Widget OrientationWidgetBuilder(BuildContext context, Orientation orientation);

public class OrientationBuilder : StatelessWidget
{
    public virtual global::System.Func<BuildContext, Orientation, Widget> builder { get; private set; } = default!;

    public OrientationBuilder(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<BuildContext, Orientation, Widget> builder = default!) : base(key: key)
    {
        this.builder = builder;
    }

    internal virtual Widget _buildWithConstraints(BuildContext context, global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        Orientation orientation__1887 = ((((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth > ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxHeight) ? Orientation.landscape : Orientation.portrait);
        return this.builder(context, orientation__1887);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new LayoutBuilder(builder: (global::System.Func<BuildContext, global::Doroti.Framework.Rendering.BoxConstraints, Widget>)this._buildWithConstraints));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DeviceOrientationBuilder : StatelessWidget
{
    public virtual global::System.Func<BuildContext, Orientation, Widget> builder { get; private set; } = default!;

    public DeviceOrientationBuilder(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<BuildContext, Orientation, Widget> builder = default!) : base(key: key)
    {
        this.builder = builder;
    }

    public override Widget build(BuildContext context)
    {
        Orientation orientation__4208 = MediaQuery.orientationOf(context);
        return this.builder(context, orientation__4208);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

