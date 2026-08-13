// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/paint_utilities.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Painting;

public static partial class Paint_utilitiesLibrary
{
    public static void paintZigZag(Canvas canvas, Paint paint, Offset start, Offset end, long zigs, double width)
    {
        DartRuntimePrimitives.Assert(() => true);
        DartRuntimePrimitives.Assert(() => (zigs > 0L));
        canvas.save();
        canvas.translate(start.dx, start.dy);
        end = (end - start);
        canvas.rotate(global::Doroti.Flutter.Runtime.Dart_mathLibrary.atan2(end.dy, end.dx));
        double length__1140 = end.distance;
        double spacing__1178 = (length__1140 / ((zigs * 2.0)));
        var path__1219 = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Flutter.Ui.Path();
    __cascade.moveTo(0.0, 0.0);
    return __cascade;
}))();
        for (var index__1263 = 0L; (index__1263 < zigs); index__1263 += 1L)
        {
            double x__1319 = ((((index__1263 * 2.0) + 1.0)) * spacing__1178);
            double y__1371 = (width * (((((index__1263 % 2.0)) * 2.0) - 1.0)));
            path__1219.lineTo(x__1319, y__1371);
        }
        path__1219.lineTo(length__1140, 0.0);
        canvas.drawPath(path__1219, paint);
        canvas.restore();
    }
}

