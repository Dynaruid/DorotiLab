// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/oval_border.dart
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

namespace Doroti.Framework.Painting;

public class OvalBorder : CircleBorder
{
    public OvalBorder(BorderSide side = default!, double eccentricity = 1.0) : base(side: side ?? BorderSide.none, eccentricity: eccentricity)
    {
    }

    public override ShapeBorder scale(double t) => new OvalBorder(side: side.scale(t), eccentricity: DartRuntimePrimitives.RequireValue(eccentricity));
    public override OvalBorder copyWith(BorderSide? side = null, BorderRadiusGeometry? borderRadius = null, double? eccentricity = null, LinearBorderEdge? start = null, LinearBorderEdge? end = null, LinearBorderEdge? top = null, LinearBorderEdge? bottom = null, double? circularity = null, double? rectilinearity = null, double? points = null, double? innerRadiusRatio = null, double? pointRounding = null, double? valleyRounding = null, double? rotation = null, double? squash = null)
    {
        return new OvalBorder(side: (side ?? this.side), eccentricity: (eccentricity ?? this.eccentricity));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        if ((a is OvalBorder))
        {
            OvalBorder a__as1519 = (OvalBorder)a;
            return new OvalBorder(side: BorderSide.lerp(((OvalBorder)a__as1519).side, side, t), eccentricity: Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((OvalBorder)a__as1519).eccentricity, DartRuntimePrimitives.RequireValue(eccentricity), t)), 0.0, 1.0));
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        if ((b is OvalBorder))
        {
            OvalBorder b__as1828 = (OvalBorder)b;
            return new OvalBorder(side: BorderSide.lerp(side, ((OvalBorder)b__as1828).side, t), eccentricity: Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(DartRuntimePrimitives.RequireValue(eccentricity), ((OvalBorder)b__as1828).eccentricity, t)), 0.0, 1.0));
        }
        return base.lerpTo(b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        if ((DartRuntimePrimitives.RequireValue(eccentricity) != 1.0))
        {
            return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "OvalBorder"))}({side}, eccentricity: {DartRuntimePrimitives.RequireValue(eccentricity)})";
        }
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "OvalBorder"))}({side})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

