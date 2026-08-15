// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/tweens.dart
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

public class FractionalOffsetTween : Tween<global::Doroti.Generated.Framework.Painting.FractionalOffset?>
{
    public FractionalOffsetTween(global::Doroti.Generated.Framework.Painting.FractionalOffset? begin = null, global::Doroti.Generated.Framework.Painting.FractionalOffset? end = null) : base(begin: begin, end: end)
    {
    }

    public virtual global::Doroti.Generated.Framework.Painting.FractionalOffset? lerp(double t) => FractionalOffset.lerp(begin, end, t);
}

public class AlignmentTween : Tween<global::Doroti.Generated.Framework.Painting.Alignment>
{
    public AlignmentTween(global::Doroti.Generated.Framework.Painting.Alignment? begin = null, global::Doroti.Generated.Framework.Painting.Alignment? end = null) : base(begin: begin, end: end)
    {
    }

    public virtual global::Doroti.Generated.Framework.Painting.Alignment lerp(double t) => Alignment.lerp(begin, end, t)!;
}

public class AlignmentGeometryTween : Tween<global::Doroti.Generated.Framework.Painting.AlignmentGeometry?>
{
    public AlignmentGeometryTween(global::Doroti.Generated.Framework.Painting.AlignmentGeometry? begin = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? end = null) : base(begin: begin, end: end)
    {
    }

    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry? lerp(double t) => AlignmentGeometry.lerp(begin, end, t);
}

