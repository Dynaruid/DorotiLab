// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/motion.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

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
    public static global::Doroti.Generated.Framework.Animation.Curve emphasizedAccelerate = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)new global::Doroti.Generated.Framework.Animation.Cubic(0.3, 0.0, 0.8, 0.15));
    public static global::Doroti.Generated.Framework.Animation.Curve emphasizedDecelerate = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)new global::Doroti.Generated.Framework.Animation.Cubic(0.05, 0.7, 0.1, 1.0));
    public static global::Doroti.Generated.Framework.Animation.Curve linear = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)new global::Doroti.Generated.Framework.Animation.Cubic(0.0, 0.0, 1.0, 1.0));
    public static global::Doroti.Generated.Framework.Animation.Curve standard = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)new global::Doroti.Generated.Framework.Animation.Cubic(0.2, 0.0, 0.0, 1.0));
    public static global::Doroti.Generated.Framework.Animation.Curve standardAccelerate = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)new global::Doroti.Generated.Framework.Animation.Cubic(0.3, 0.0, 1.0, 1.0));
    public static global::Doroti.Generated.Framework.Animation.Curve standardDecelerate = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)new global::Doroti.Generated.Framework.Animation.Cubic(0.0, 0.0, 0.0, 1.0));
    public static global::Doroti.Generated.Framework.Animation.Curve legacyDecelerate = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)new global::Doroti.Generated.Framework.Animation.Cubic(0.0, 0.0, 0.2, 1.0));
    public static global::Doroti.Generated.Framework.Animation.Curve legacyAccelerate = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)new global::Doroti.Generated.Framework.Animation.Cubic(0.4, 0.0, 1.0, 1.0));
    public static global::Doroti.Generated.Framework.Animation.Curve legacy = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)new global::Doroti.Generated.Framework.Animation.Cubic(0.4, 0.0, 0.2, 1.0));

}
