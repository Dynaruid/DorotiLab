// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/notched_shapes.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public interface NotchedShape
{
    public Path getOuterPath(Rect host, Rect? guest);
}

public class CircularNotchedRectangle : NotchedShape
{
    public virtual bool inverted { get; private set; } = default!;

    public CircularNotchedRectangle(bool inverted = false)
    {
        this.inverted = inverted;
    }

    public virtual Path getOuterPath(Rect host, Rect? guest)
    {
        if (
            (guest is null)
            || !host.overlaps(
                (
                    (
                        guest
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    )
                )
            )
        )
        {
            return (
                (Func<Path>)(
                    () =>
                    {
                        var __cascade = new Path();
                        __cascade.addRect(host);
                        return __cascade;
                    }
                )
            )();
        }
        double r =
            (
                guest
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ).width / 2.0;
        var notchRadius = Radius.circular(r);
        var invertMultiplier = inverted ? -1.0 : 1.0;
        var s1 = 15.0;
        var s2 = 1.0;
        double a = -r - s2;
        double b =
            (inverted ? host.bottom : host.top)
            - (
                guest
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            )
                .center
                .dy;
        double n2 = Dart_mathLibrary.sqrt(b * b * r * r * ((a * a) + (b * b) - (r * r)));
        double p2xA = ((a * r * r) - n2) / ((a * a) + (b * b));
        double p2xB = ((a * r * r) + n2) / ((a * a) + (b * b));
        double p2yA = Dart_mathLibrary.sqrt((r * r) - (p2xA * p2xA)) * invertMultiplier;
        double p2yB = Dart_mathLibrary.sqrt((r * r) - (p2xB * p2xB)) * invertMultiplier;
        var p = new List<Offset>(Enumerable.Repeat(Offset.zero, checked((int)6L)));
        p[(int)0L] = new Offset(a - s1, b);
        p[(int)1L] = new Offset(a, b);
        var cmp = (b < 0L) ? -1.0 : 1.0;
        p[(int)2L] =
            ((cmp * p2yA) > (cmp * p2yB)) ? new Offset(p2xA, p2yA) : new Offset(p2xB, p2yB);
        p[(int)3L] = new Offset(-1.0 * p[(int)2L].dx, p[(int)2L].dy);
        p[(int)4L] = new Offset(-1.0 * p[(int)1L].dx, p[(int)1L].dy);
        p[(int)5L] = new Offset(-1.0 * p[(int)0L].dx, p[(int)0L].dy);
        for (var i = 0L; i < checked(p.Count); i += 1L)
        {
            p[(int)i] += (
                guest
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ).center;
        }
        var path = (
            (Func<Path>)(
                () =>
                {
                    var __cascade = new Path();
                    __cascade.moveTo(host.left, host.top);
                    return __cascade;
                }
            )
        )();
        if (!inverted)
        {
            (
                (Func<Path>)(
                    () =>
                    {
                        var __cascade = path;
                        __cascade.lineTo(p[(int)0L].dx, p[(int)0L].dy);
                        __cascade.quadraticBezierTo(
                            p[(int)1L].dx,
                            p[(int)1L].dy,
                            p[(int)2L].dx,
                            p[(int)2L].dy
                        );
                        __cascade.arcToPoint(p[(int)3L], radius: notchRadius, clockwise: false);
                        __cascade.quadraticBezierTo(
                            p[(int)4L].dx,
                            p[(int)4L].dy,
                            p[(int)5L].dx,
                            p[(int)5L].dy
                        );
                        __cascade.lineTo(host.right, host.top);
                        __cascade.lineTo(host.right, host.bottom);
                        __cascade.lineTo(host.left, host.bottom);
                        return __cascade;
                    }
                )
            )();
        }
        else
        {
            (
                (Func<Path>)(
                    () =>
                    {
                        var __cascade = path;
                        __cascade.lineTo(host.right, host.top);
                        __cascade.lineTo(host.right, host.bottom);
                        __cascade.lineTo(p[(int)5L].dx, p[(int)5L].dy);
                        __cascade.quadraticBezierTo(
                            p[(int)4L].dx,
                            p[(int)4L].dy,
                            p[(int)3L].dx,
                            p[(int)3L].dy
                        );
                        __cascade.arcToPoint(p[(int)2L], radius: notchRadius, clockwise: false);
                        __cascade.quadraticBezierTo(
                            p[(int)1L].dx,
                            p[(int)1L].dy,
                            p[(int)0L].dx,
                            p[(int)0L].dy
                        );
                        __cascade.lineTo(host.left, host.bottom);
                        return __cascade;
                    }
                )
            )();
        }
        return (
            (Func<Path>)(
                () =>
                {
                    var __cascade = path;
                    __cascade.close();
                    return __cascade;
                }
            )
        )();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class AutomaticNotchedShape : NotchedShape
{
    public virtual ShapeBorder host { get; private set; } = default!;
    public virtual ShapeBorder? guest { get; private set; }

    public AutomaticNotchedShape(ShapeBorder host, ShapeBorder? guest = null)
    {
        this.host = host;
        this.guest = guest;
    }

    public virtual Path getOuterPath(Rect host, Rect? guest)
    {
        Path hostPath = this.host.getOuterPath(host);
        if ((this.guest is not null) && (guest is not null))
        {
            Rect guestRect__value6659 = (
                guest
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
            Path guestPath = this.guest!.getOuterPath(((guestRect__value6659)));
            return Dart_uiLibrary.Path.combine(PathOperation.difference, hostPath, guestPath);
        }
        return hostPath;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
