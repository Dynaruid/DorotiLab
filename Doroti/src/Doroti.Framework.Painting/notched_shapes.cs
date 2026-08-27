// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/notched_shapes.dart
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

public interface NotchedShape
{
    public global::Doroti.Ui.Path getOuterPath(Rect host, Rect? guest);
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
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addRect(host);
    return __cascade;
}))();
        }
        double r = (DartRuntimePrimitives.RequireValue(guest).width / 2.0);
        var notchRadius = global::Doroti.Ui.Radius.circular(r);
        var invertMultiplier = (this.inverted ? -1.0 : 1.0);
        var s1 = 15.0;
        var s2 = 1.0;
        double a = (-r - s2);
        double b = (((this.inverted ? host.bottom : host.top)) - DartRuntimePrimitives.RequireValue(guest).center.dy);
        double n2 = global::Doroti.Runtime.Dart_mathLibrary.sqrt(((((b * b) * r) * r) * ((((a * a) + (b * b)) - (r * r)))));
        double p2xA = ((((((a * r) * r)) - n2)) / (((a * a) + (b * b))));
        double p2xB = ((((((a * r) * r)) + n2)) / (((a * a) + (b * b))));
        double p2yA = (global::Doroti.Runtime.Dart_mathLibrary.sqrt(((r * r) - (p2xA * p2xA))) * invertMultiplier);
        double p2yB = (global::Doroti.Runtime.Dart_mathLibrary.sqrt(((r * r) - (p2xB * p2xB))) * invertMultiplier);
        var p = new List<global::Doroti.Ui.Offset>(System.Linq.Enumerable.Repeat<global::Doroti.Ui.Offset>(Offset.zero, checked((int)6L)));
        p[(int)(0L)] = new global::Doroti.Ui.Offset((a - s1), b);
        p[(int)(1L)] = new global::Doroti.Ui.Offset(a, b);
        var cmp = ((b < 0L) ? -1.0 : 1.0);
        p[(int)(2L)] = (((cmp * p2yA) > (cmp * p2yB)) ? new global::Doroti.Ui.Offset(p2xA, p2yA) : new global::Doroti.Ui.Offset(p2xB, p2yB));
        p[(int)(3L)] = new global::Doroti.Ui.Offset((-1.0 * p[(int)(2L)].dx), p[(int)(2L)].dy);
        p[(int)(4L)] = new global::Doroti.Ui.Offset((-1.0 * p[(int)(1L)].dx), p[(int)(1L)].dy);
        p[(int)(5L)] = new global::Doroti.Ui.Offset((-1.0 * p[(int)(0L)].dx), p[(int)(0L)].dy);
        for (var i = 0L; (i < checked((long)(p.Count))); i += 1L)
        {
            p[(int)(i)] += DartRuntimePrimitives.RequireValue(guest).center;
        }
        var path = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.moveTo(host.left, host.top);
    return __cascade;
}))();
        if (!this.inverted)
        {
            ((Func<Path>)(() =>
{
    var __cascade = path;
    __cascade.lineTo(p[(int)(0L)].dx, p[(int)(0L)].dy);
    __cascade.quadraticBezierTo(p[(int)(1L)].dx, p[(int)(1L)].dy, p[(int)(2L)].dx, p[(int)(2L)].dy);
    __cascade.arcToPoint(p[(int)(3L)], radius: notchRadius, clockwise: false);
    __cascade.quadraticBezierTo(p[(int)(4L)].dx, p[(int)(4L)].dy, p[(int)(5L)].dx, p[(int)(5L)].dy);
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
    var __cascade = path;
    __cascade.lineTo(host.right, host.top);
    __cascade.lineTo(host.right, host.bottom);
    __cascade.lineTo(p[(int)(5L)].dx, p[(int)(5L)].dy);
    __cascade.quadraticBezierTo(p[(int)(4L)].dx, p[(int)(4L)].dy, p[(int)(3L)].dx, p[(int)(3L)].dy);
    __cascade.arcToPoint(p[(int)(2L)], radius: notchRadius, clockwise: false);
    __cascade.quadraticBezierTo(p[(int)(1L)].dx, p[(int)(1L)].dy, p[(int)(0L)].dx, p[(int)(0L)].dy);
    __cascade.lineTo(host.left, host.bottom);
    return __cascade;
}))();
        }
        return ((Func<Path>)(() =>
{
    var __cascade = path;
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
        global::Doroti.Ui.Path hostPath = this.host.getOuterPath(host);
        if (((this.guest is not null) && (guest is not null)))
        {
            Rect guestRect__value6659 = DartRuntimePrimitives.RequireValue(guest);
            global::Doroti.Ui.Path guestPath = this.guest!.getOuterPath(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(guestRect__value6659)));
            return Dart_uiLibrary.Path.combine(PathOperation.difference, hostPath, guestPath);
        }
        return hostPath;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

