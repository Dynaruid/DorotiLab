// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/curves.dart
#pragma warning disable CS8600, CS8601
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Material;

public static partial class CurvesLibrary
{
    public static global::Doroti.Framework.Animation.Curve standardEasing = ((global::Doroti.Framework.Animation.Curve)(object?)global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
}

public static partial class CurvesLibrary
{
    public static global::Doroti.Framework.Animation.Curve accelerateEasing = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Cubic(0.4, 0.0, 1.0, 1.0));
}

public static partial class CurvesLibrary
{
    public static global::Doroti.Framework.Animation.Curve decelerateEasing = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Cubic(0.0, 0.0, 0.2, 1.0));
}
