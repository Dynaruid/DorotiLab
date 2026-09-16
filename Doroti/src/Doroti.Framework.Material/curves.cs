// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/curves.dart

namespace Doroti.Framework.Material;

public static partial class CurvesLibrary
{
    public static global::Doroti.Framework.Animation.Curve standardEasing = Curves.fastOutSlowIn;
}

public static partial class CurvesLibrary
{
    public static global::Doroti.Framework.Animation.Curve accelerateEasing = new global::Doroti.Framework.Animation.Cubic(0.4, 0.0, 1.0, 1.0);
}

public static partial class CurvesLibrary
{
    public static global::Doroti.Framework.Animation.Curve decelerateEasing = new global::Doroti.Framework.Animation.Cubic(0.0, 0.0, 0.2, 1.0);
}
