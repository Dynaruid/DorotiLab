// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/curves.dart

namespace Doroti.Framework.Material;

public static partial class CurvesLibrary
{
    public static Curve standardEasing = Curves.fastOutSlowIn;
}

public static partial class CurvesLibrary
{
    public static Curve accelerateEasing = new Cubic(0.4, 0.0, 1.0, 1.0);
}

public static partial class CurvesLibrary
{
    public static Curve decelerateEasing = new Cubic(0.0, 0.0, 0.2, 1.0);
}
