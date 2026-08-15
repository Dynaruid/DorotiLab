// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/curves.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public static partial class CurvesLibrary
{
    public static global::Doroti.Generated.Framework.Animation.Curve standardEasing = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)global::Doroti.Generated.Framework.Animation.Curves.fastOutSlowIn);
}

public static partial class CurvesLibrary
{
    public static global::Doroti.Generated.Framework.Animation.Curve accelerateEasing = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)new global::Doroti.Generated.Framework.Animation.Cubic(0.4, 0.0, 1.0, 1.0));
}

public static partial class CurvesLibrary
{
    public static global::Doroti.Generated.Framework.Animation.Curve decelerateEasing = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)new global::Doroti.Generated.Framework.Animation.Cubic(0.0, 0.0, 0.2, 1.0));
}
