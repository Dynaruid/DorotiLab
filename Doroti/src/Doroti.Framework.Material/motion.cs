// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/motion.dart
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

public abstract class Durations
{
    public static Duration short1 = Duration.Create(milliseconds: 50L);
    public static Duration short2 = Duration.Create(milliseconds: 100L);
    public static Duration short3 = Duration.Create(milliseconds: 150L);
    public static Duration short4 = Duration.Create(milliseconds: 200L);
    public static Duration medium1 = Duration.Create(milliseconds: 250L);
    public static Duration medium2 = Duration.Create(milliseconds: 300L);
    public static Duration medium3 = Duration.Create(milliseconds: 350L);
    public static Duration medium4 = Duration.Create(milliseconds: 400L);
    public static Duration long1 = Duration.Create(milliseconds: 450L);
    public static Duration long2 = Duration.Create(milliseconds: 500L);
    public static Duration long3 = Duration.Create(milliseconds: 550L);
    public static Duration long4 = Duration.Create(milliseconds: 600L);
    public static Duration extralong1 = Duration.Create(milliseconds: 700L);
    public static Duration extralong2 = Duration.Create(milliseconds: 800L);
    public static Duration extralong3 = Duration.Create(milliseconds: 900L);
    public static Duration extralong4 = Duration.Create(milliseconds: 1000L);

}

public abstract class Easing
{
    public static global::Doroti.Framework.Animation.Curve emphasizedAccelerate = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Cubic(0.3, 0.0, 0.8, 0.15));
    public static global::Doroti.Framework.Animation.Curve emphasizedDecelerate = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Cubic(0.05, 0.7, 0.1, 1.0));
    public static global::Doroti.Framework.Animation.Curve linear = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Cubic(0.0, 0.0, 1.0, 1.0));
    public static global::Doroti.Framework.Animation.Curve standard = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Cubic(0.2, 0.0, 0.0, 1.0));
    public static global::Doroti.Framework.Animation.Curve standardAccelerate = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Cubic(0.3, 0.0, 1.0, 1.0));
    public static global::Doroti.Framework.Animation.Curve standardDecelerate = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Cubic(0.0, 0.0, 0.0, 1.0));
    public static global::Doroti.Framework.Animation.Curve legacyDecelerate = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Cubic(0.0, 0.0, 0.2, 1.0));
    public static global::Doroti.Framework.Animation.Curve legacyAccelerate = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Cubic(0.4, 0.0, 1.0, 1.0));
    public static global::Doroti.Framework.Animation.Curve legacy = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Cubic(0.4, 0.0, 0.2, 1.0));

}
