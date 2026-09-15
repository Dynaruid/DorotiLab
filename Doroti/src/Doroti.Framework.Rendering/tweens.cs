// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/tweens.dart
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

public class FractionalOffsetTween : Tween<global::Doroti.Framework.Painting.FractionalOffset?>
{
    public FractionalOffsetTween(global::Doroti.Framework.Painting.FractionalOffset? begin = null, global::Doroti.Framework.Painting.FractionalOffset? end = null) : base(begin: begin, end: end)
    {
    }

    public override global::Doroti.Framework.Painting.FractionalOffset? lerp(double t) => FractionalOffset.lerp(begin, end, t);
}

public class AlignmentTween : Tween<global::Doroti.Framework.Painting.Alignment>
{
    public AlignmentTween(global::Doroti.Framework.Painting.Alignment? begin = null, global::Doroti.Framework.Painting.Alignment? end = null) : base(begin: begin, end: end)
    {
    }

    public override global::Doroti.Framework.Painting.Alignment lerp(double t) => Alignment.lerp(begin, end, t)!;
}

public class AlignmentGeometryTween : Tween<global::Doroti.Framework.Painting.AlignmentGeometry?>
{
    public AlignmentGeometryTween(global::Doroti.Framework.Painting.AlignmentGeometry? begin = null, global::Doroti.Framework.Painting.AlignmentGeometry? end = null) : base(begin: begin, end: end)
    {
    }

    public override global::Doroti.Framework.Painting.AlignmentGeometry? lerp(double t) => AlignmentGeometry.lerp(begin, end, t);
}

