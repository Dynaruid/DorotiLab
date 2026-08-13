// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/notched_shapes.dart
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

public interface NotchedShape
{
    public global::Doroti.Flutter.Ui.Path getOuterPath(Rect host, Rect? guest);
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
        if (((guest is null) || !host.overlaps(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(guest)))))
        {
            return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Flutter.Ui.Path();
    __cascade.addRect(host);
    return __cascade;
}))();
        }
        double r__2546 = (DartRuntimePrimitives.RequireValue(guest).width / 2.0);
        var notchRadius__2579 = global::Doroti.Flutter.Ui.Radius.circular(r__2546);
        var invertMultiplier__2739 = (this.inverted ? -1.0 : 1.0);
        var s1__3279 = 15.0;
        var s2__3300 = 1.0;
        double a__3328 = (-r__2546 - s2__3300);
        double b__3358 = (((this.inverted ? host.bottom : host.top)) - DartRuntimePrimitives.RequireValue(guest).center.dy);
        double n2__3435 = global::Doroti.Flutter.Runtime.Dart_mathLibrary.sqrt(((((b__3358 * b__3358) * r__2546) * r__2546) * ((((a__3328 * a__3328) + (b__3358 * b__3358)) - (r__2546 * r__2546)))));
        double p2xA__3509 = ((((((a__3328 * r__2546) * r__2546)) - n2__3435)) / (((a__3328 * a__3328) + (b__3358 * b__3358))));
        double p2xB__3571 = ((((((a__3328 * r__2546) * r__2546)) + n2__3435)) / (((a__3328 * a__3328) + (b__3358 * b__3358))));
        double p2yA__3633 = (global::Doroti.Flutter.Runtime.Dart_mathLibrary.sqrt(((r__2546 * r__2546) - (p2xA__3509 * p2xA__3509))) * invertMultiplier__2739);
        double p2yB__3708 = (global::Doroti.Flutter.Runtime.Dart_mathLibrary.sqrt(((r__2546 * r__2546) - (p2xB__3571 * p2xB__3571))) * invertMultiplier__2739);
        var p__3777 = new List<global::Doroti.Flutter.Ui.Offset>(System.Linq.Enumerable.Repeat<global::Doroti.Flutter.Ui.Offset>(Offset.zero, checked((int)6L)));
        p__3777[(int)(0L)] = new global::Doroti.Flutter.Ui.Offset((a__3328 - s1__3279), b__3358);
        p__3777[(int)(1L)] = new global::Doroti.Flutter.Ui.Offset(a__3328, b__3358);
        var cmp__3944 = ((b__3358 < 0L) ? -1.0 : 1.0);
        p__3777[(int)(2L)] = (((cmp__3944 * p2yA__3633) > (cmp__3944 * p2yB__3708)) ? new global::Doroti.Flutter.Ui.Offset(p2xA__3509, p2yA__3633) : new global::Doroti.Flutter.Ui.Offset(p2xB__3571, p2yB__3708));
        p__3777[(int)(3L)] = new global::Doroti.Flutter.Ui.Offset((-1.0 * p__3777[(int)(2L)].dx), p__3777[(int)(2L)].dy);
        p__3777[(int)(4L)] = new global::Doroti.Flutter.Ui.Offset((-1.0 * p__3777[(int)(1L)].dx), p__3777[(int)(1L)].dy);
        p__3777[(int)(5L)] = new global::Doroti.Flutter.Ui.Offset((-1.0 * p__3777[(int)(0L)].dx), p__3777[(int)(0L)].dy);
        for (var i__4380 = 0L; (i__4380 < checked((long)(p__3777.Count))); i__4380 += 1L)
        {
            p__3777[(int)(i__4380)] += DartRuntimePrimitives.RequireValue(guest).center;
        }
        var path__4516 = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Flutter.Ui.Path();
    __cascade.moveTo(host.left, host.top);
    return __cascade;
}))();
        if (!this.inverted)
        {
            ((Func<Path>)(() =>
{
    var __cascade = path__4516;
    __cascade.lineTo(p__3777[(int)(0L)].dx, p__3777[(int)(0L)].dy);
    __cascade.quadraticBezierTo(p__3777[(int)(1L)].dx, p__3777[(int)(1L)].dy, p__3777[(int)(2L)].dx, p__3777[(int)(2L)].dy);
    __cascade.arcToPoint(p__3777[(int)(3L)], radius: notchRadius__2579, clockwise: false);
    __cascade.quadraticBezierTo(p__3777[(int)(4L)].dx, p__3777[(int)(4L)].dy, p__3777[(int)(5L)].dx, p__3777[(int)(5L)].dy);
    __cascade.lineTo(host.right, host.top);
    __cascade.lineTo(host.right, host.bottom);
    __cascade.lineTo(host.left, host.bottom);
    return __cascade;
}))();
        }
        else
        {
            ((Func<Path>)(() =>
{
    var __cascade = path__4516;
    __cascade.lineTo(host.right, host.top);
    __cascade.lineTo(host.right, host.bottom);
    __cascade.lineTo(p__3777[(int)(5L)].dx, p__3777[(int)(5L)].dy);
    __cascade.quadraticBezierTo(p__3777[(int)(4L)].dx, p__3777[(int)(4L)].dy, p__3777[(int)(3L)].dx, p__3777[(int)(3L)].dy);
    __cascade.arcToPoint(p__3777[(int)(2L)], radius: notchRadius__2579, clockwise: false);
    __cascade.quadraticBezierTo(p__3777[(int)(1L)].dx, p__3777[(int)(1L)].dy, p__3777[(int)(0L)].dx, p__3777[(int)(0L)].dy);
    __cascade.lineTo(host.left, host.bottom);
    return __cascade;
}))();
        }
        return ((Func<Path>)(() =>
{
    var __cascade = path__4516;
    __cascade.close();
    return __cascade;
}))();
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
        global::Doroti.Flutter.Ui.Path hostPath__6594 = this.host.getOuterPath(host);
        if (((this.guest is not null) && (guest is not null)))
        {
            Rect guestRect__value6659 = DartRuntimePrimitives.RequireValue(guest);
            global::Doroti.Flutter.Ui.Path guestPath__6697 = this.guest!.getOuterPath(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(guestRect__value6659)));
            return Dart_uiLibrary.Path.combine(PathOperation.difference, hostPath__6594, guestPath__6697);
        }
        return hostPath__6594;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

