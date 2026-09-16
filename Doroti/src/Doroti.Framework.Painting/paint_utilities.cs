// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/paint_utilities.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public static partial class Paint_utilitiesLibrary
{
    public static void paintZigZag(Canvas canvas, Paint paint, Offset start, Offset end, long zigs, double width)
    {
        DartRuntimePrimitives.Assert(() => true);
        DartRuntimePrimitives.Assert(() => (zigs > 0L));
        canvas.save();
        canvas.translate(start.dx, start.dy);
        end = (end - start);
        canvas.rotate(Dart_mathLibrary.atan2(end.dy, end.dx));
        double length = end.distance;
        double spacing = (length / ((zigs * 2.0)));
        var path = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.moveTo(0.0, 0.0);
    return __cascade;
}))();
        for (var index = 0L; (index < zigs); index += 1L)
        {
            double x = ((((index * 2.0) + 1.0)) * spacing);
            double y = (width * (((((index % 2.0)) * 2.0) - 1.0)));
            path.lineTo(x, y);
        }
        path.lineTo(length, 0.0);
        canvas.drawPath(path, paint);
        canvas.restore();
    }
}

