// Copyright 2013 The Flutter Authors. All rights reserved.
// Use of this source code is governed by a BSD-style license found in
// Doroti/LICENSES/Flutter-LICENSE.md.
// Adapted from engine/src/flutter/lib/web_ui/lib/rsuperellipse_param.dart.
using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Skia.Rendering;

// Flutter's continuous corners consist of two conics and a circular cubic per
// octant. Use one contour for painting, decoration paths and clipping so their
// edges agree, including nonuniform and elliptical corner radii.
internal static class SkiaRSuperellipsePath
{
    internal static SKPath Create(RSuperellipse shape)
    {
        using var path = new SKPathBuilder();
        var r = shape.outerRect;
        if (r.isEmpty || !r.IsFinite) return path.Detach();
        var radii = new[] { shape.tlRadius, shape.trRadius, shape.brRadius, shape.blRadius }
            .Select(radius => radius.x <= 0 || radius.y <= 0 ? Radius.zero : radius).ToArray();
        var scale = 1.0;
        Limit(r.width, radii[0].x + radii[1].x);
        Limit(r.width, radii[3].x + radii[2].x);
        Limit(r.height, radii[0].y + radii[3].y);
        Limit(r.height, radii[1].y + radii[2].y);
        for (var i = 0; i < radii.Length; i++) radii[i] = Radius.elliptical(radii[i].x * scale, radii[i].y * scale);
        var top = Split(r.left, r.right, radii[0].x, radii[1].x);
        var right = Split(r.top, r.bottom, radii[1].y, radii[2].y);
        var bottom = Split(r.left, r.right, radii[3].x, radii[2].x);
        var left = Split(r.top, r.bottom, radii[0].y, radii[3].y);
        path.MoveTo((float)top, (float)r.top);
        Quadrant(path, new(top, right), new(r.right, r.top), radii[1], new(1, -1), false);
        Quadrant(path, new(bottom, right), new(r.right, r.bottom), radii[2], new(1, 1), true);
        Quadrant(path, new(bottom, left), new(r.left, r.bottom), radii[3], new(-1, 1), false);
        Quadrant(path, new(top, left), new(r.left, r.top), radii[0], new(-1, -1), true);
        path.LineTo((float)top, (float)r.top);
        path.Close();
        return path.Detach();

        void Limit(double dimension, double sum)
        {
            if (sum > 0) scale = Math.Min(scale, dimension / sum);
        }
    }

    private static double Split(double start, double end, double first, double second) =>
        first + second == 0 ? (start + end) / 2 : (start * second + end * first) / (first + second);

    private static SKPoint Point(Offset p) => new((float)p.dx, (float)p.dy);

    private static void Quadrant(SKPathBuilder path, Offset center, Offset corner, Radius radii, Offset sign, bool reverse)
    {
        var vector = corner - center;
        var radius = Math.Min(radii.x, radii.y);
        var width = Math.Abs(vector.dx) / (radius == 0 ? 1 : radii.x / radius);
        var height = Math.Abs(vector.dy) / (radius == 0 ? 1 : radii.y / radius);
        var sx = width == 0 ? sign.dx : vector.dx / width;
        var sy = height == 0 ? sign.dy : vector.dy / height;
        Offset Transform(Offset p) => center + new Offset(p.dx * sx, p.dy * sy);
        var c = width - height;
        var topOffset = new Offset(0, -c);
        var rightOffset = new Offset(c, 0);
        if (radius <= 0)
        {
            var offset = reverse ? topOffset : rightOffset;
            var a = reverse ? width : height;
            path.LineTo(Point(Transform(offset + new Offset(a, a))));
            path.LineTo(Point(Transform(offset + (reverse ? new Offset(0, a) : new Offset(a, 0)))));
            return;
        }
        if (!reverse)
        {
            Octant(path, Transform, topOffset, width, radius, false, false);
            Octant(path, Transform, rightOffset, height, radius, true, true);
        }
        else
        {
            Octant(path, Transform, rightOffset, height, radius, false, true);
            Octant(path, Transform, topOffset, width, radius, true, false);
        }
    }

    private static void Octant(SKPathBuilder path, Func<Offset, Offset> external, Offset offset,
        double a, double radius, bool reverse, bool flip)
    {
        SKPoint Transform(Offset p) => Point(external(offset + (flip ? new Offset(p.dy, p.dx) : p)));
        var (n, xRatio) = Parameters(2 * a / radius);
        var gap = 0.29289321881 * radius;
        var j = new Offset(xRatio * a, Math.Pow(1 - Math.Pow(xRatio, n), 1 / n) * a);
        var tangent = Math.Pow(j.dx / j.dy, n - 1);
        var d = (j.dx - tangent * j.dy) / (1 - tangent);
        var circleRadius = (a - d - gap) * Math.Sqrt(2);
        var m = new Offset(a - gap, a - gap);
        var chord = m - j;
        var center = (j + m) / 2 - new Offset(-chord.dy, chord.dx) / chord.distance *
            Math.Sqrt(Math.Max(0, circleRadius * circleRadius - chord.distanceSquared / 4));
        var start = j - center;
        var mid = m - center;
        var angle = Math.Atan2(mid.dx * start.dy - mid.dy * start.dx, mid.dx * start.dx + mid.dy * start.dy);
        var end = new Offset(start.dx * Math.Cos(angle) + start.dy * Math.Sin(angle),
            -start.dx * Math.Sin(angle) + start.dy * Math.Cos(angle));
        var factor = Math.Tan(angle / 4) * 4 / 3;
        var c1 = j + new Offset(start.dy, -start.dx) * factor;
        var c2 = center + end + new Offset(-end.dy, end.dx) * factor;
        var (w1, w2, yRatio) = BezierFactors(n, xRatio, j.dy / a);
        var h = new Offset(Math.Pow(1 - Math.Pow(yRatio, n), 1 / n) * a, yRatio * a);
        var initial = new Offset(0, a);
        var kh = -Math.Pow(h.dx / h.dy, n - 1);
        var cp1 = Intersection(initial, 0, h, kh);
        var cp2 = Intersection(h, kh, j, -tangent);
        if (!reverse)
        {
            path.ConicTo(Transform(cp1), Transform(h), (float)w1);
            path.ConicTo(Transform(cp2), Transform(j), (float)w2);
            path.CubicTo(Transform(c1), Transform(c2), Transform(center + end));
        }
        else
        {
            path.CubicTo(Transform(c2), Transform(c1), Transform(j));
            path.ConicTo(Transform(cp2), Transform(h), (float)w2);
            path.ConicTo(Transform(cp1), Transform(initial), (float)w1);
        }
    }

    private static Offset Intersection(Offset p, double slope, Offset q, double otherSlope)
    {
        if (Math.Abs(slope - otherSlope) < 1e-5) return (p + q) / 2;
        var x = (slope * p.dx - otherSlope * q.dx + q.dy - p.dy) / (slope - otherSlope);
        return new Offset(x, slope * (x - p.dx) + p.dy);
    }

    private static readonly (double N, double Factor)[] ParameterTable =
    [
        (2.00000000, 1.13276676), (2.18349805, 1.20311921), (2.33888662, 1.28698796),
        (2.48660575, 1.36351941), (2.62226596, 1.44717976), (2.75148990, 1.53385819),
        (3.36298265, 1.98288283), (4.08649929, 2.23811846), (4.85481134, 2.47563463),
        (5.62945551, 2.72948597), (6.43023796, 2.98020421),
    ];

    private static (double N, double XRatio) Parameters(double ratio)
    {
        if (ratio > 5) return (1.559599389 * (ratio - 5) + 6.43023796,
            1 - 1 / (0.522807185 * (ratio - 5) + 2.98020421));
        ratio = Math.Clamp(ratio, 2, 5);
        var steps = ratio < 2.5 ? (ratio - 2) * 10 : (ratio - 2.5) * 2 + 5;
        var index = Math.Clamp((int)steps, 0, ParameterTable.Length - 2);
        var t = steps - index;
        var first = ParameterTable[index];
        var second = ParameterTable[index + 1];
        return (Lerp(first.N, second.N, t), 1 - 1 / Lerp(first.Factor, second.Factor, t));
    }

    private static readonly (double First, double Second)[] BezierTable =
    [
        (0.7078, 8.3194), (0.7895, 2.4523), (0.8379, 1.8528), (0.8701, 1.6891),
        (0.8932, 1.5806), (0.9107, 1.5043), (0.9244, 1.4470), (0.9355, 1.4037),
        (0.9448, 1.3701), (0.9526, 1.3431), (0.9594, 1.3212), (0.9653, 1.3032), (0.9705, 1.2880),
    ];

    private static (double W1, double W2, double YRatio) BezierFactors(double n, double xRatio, double yRatio)
    {
        n = Math.Clamp(n, 2, 14);
        var steps = n - 2;
        var index = Math.Clamp((int)steps, 0, BezierTable.Length - 2);
        var first = BezierTable[index];
        var second = BezierTable[index + 1];
        var root = Math.Sqrt(n);
        return (Lerp(first.First, second.First, steps - index) * root,
            Lerp(first.Second, second.Second, steps - index) * xRatio, (root + yRatio) / (root + 1));
    }

    private static double Lerp(double start, double end, double t) => start + (end - start) * t;
}
