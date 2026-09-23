// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/arc.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class ArcLibrary
{
    internal static double _kOnAxisDelta = 2.0;
}

public class MaterialPointArcTween : Tween<Offset>
{
    internal virtual bool _dirty { get; set; } = true;
    internal virtual Offset? _center { get; set; } = default;
    internal virtual double? _radius { get; set; } = default;
    internal virtual double? _beginAngle { get; set; } = default;
    internal virtual double? _endAngle { get; set; } = default;

    public MaterialPointArcTween(Offset? begin = null, Offset? end = null)
        : base(
            begin: (
                begin
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ),
            end: (
                end ?? throw new global::System.NullReferenceException("A required value was null.")
            )
        ) { }

    internal virtual void _initialize()
    {
        Offset beginLocal = (begin);
        Offset endLocal = (end);
        Offset delta = (endLocal) - (beginLocal);
        double deltaX = delta.dx.abs();
        double deltaY = delta.dy.abs();
        double distanceFromAtoB = delta.distance;
        var c = new Offset((endLocal).dx, (beginLocal).dy);
        double sweepAngle()
        {
            return 2.0
                * Math.Asin(
                    distanceFromAtoB
                        / (
                            2.0
                            * (
                                _radius
                                ?? throw new global::System.NullReferenceException(
                                    "A required value was null."
                                )
                            )
                        )
                );
            throw new InvalidOperationException(
                "Control flow completed without returning a value."
            );
        }
        if ((deltaX > ArcLibrary._kOnAxisDelta) && (deltaY > ArcLibrary._kOnAxisDelta))
        {
            if (deltaX < deltaY)
            {
                _radius = distanceFromAtoB * distanceFromAtoB / (c - (beginLocal)).distance / 2.0;
                _center = new Offset(
                    (endLocal).dx
                        + (
                            (
                                _radius
                                ?? throw new global::System.NullReferenceException(
                                    "A required value was null."
                                )
                            ) * Math.Sign((beginLocal).dx - (endLocal).dx)
                        ),
                    (endLocal).dy
                );
                if ((beginLocal).dx < (endLocal).dx)
                {
                    _beginAngle = sweepAngle() * Math.Sign((beginLocal).dy - (endLocal).dy);
                    _endAngle = 0.0;
                }
                else
                {
                    _beginAngle =
                        Math.PI
                        + (sweepAngle() * Math.Sign((endLocal).dy - (beginLocal).dy));
                    _endAngle = Math.PI;
                }
            }
            else
            {
                _radius = distanceFromAtoB * distanceFromAtoB / (c - (endLocal)).distance / 2.0;
                _center = new Offset(
                    (beginLocal).dx,
                    (beginLocal).dy
                        + (
                            Math.Sign((endLocal).dy - (beginLocal).dy)
                            * (
                                _radius
                                ?? throw new global::System.NullReferenceException(
                                    "A required value was null."
                                )
                            )
                        )
                );
                if ((beginLocal).dy < (endLocal).dy)
                {
                    _beginAngle = -Math.PI / 2.0;
                    _endAngle =
                        (
                            _beginAngle
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        ) + (sweepAngle() * Math.Sign((endLocal).dx - (beginLocal).dx));
                }
                else
                {
                    _beginAngle = Math.PI / 2.0;
                    _endAngle =
                        (
                            _beginAngle
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        ) + (sweepAngle() * Math.Sign((beginLocal).dx - (endLocal).dx));
                }
            }
            DartRuntimePrimitives.Assert(() => _beginAngle is not null);
            DartRuntimePrimitives.Assert(() => _endAngle is not null);
        }
        else
        {
            _beginAngle = null;
            _endAngle = null;
        }
        _dirty = false;
    }

    public virtual Offset? center
    {
        get
        {
            if (_dirty)
            {
                _initialize();
            }
            return _center;
        }
    }
    public virtual double? radius
    {
        get
        {
            if (_dirty)
            {
                _initialize();
            }
            return _radius;
        }
    }
    public virtual double? beginAngle
    {
        get
        {
            if (_dirty)
            {
                _initialize();
            }
            return _beginAngle;
        }
    }
    public virtual double? endAngle
    {
        get
        {
            if (_dirty)
            {
                _initialize();
            }
            return _beginAngle;
        }
    }
    public override Offset begin
    {
        set
        {
            var __value = value;
            if (!Equals(__value, begin))
            {
                base.begin = __value;
                _dirty = true;
            }
        }
    }
    public override Offset end
    {
        set
        {
            var __value = value;
            if (!Equals(__value, end))
            {
                base.end = __value;
                _dirty = true;
            }
        }
    }

    public override Offset lerp(double t)
    {
        if (_dirty)
        {
            _initialize();
        }
        if (t == 0.0)
        {
            return (begin);
        }
        if (t == 1.0)
        {
            return (end);
        }
        if ((_beginAngle is null) || (_endAngle is null))
        {
            return (
                DorotiUiLibrary.Offset.lerp(begin, end, t)
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        }
        double angle = (
            DorotiUiLibrary.lerpDouble(_beginAngle, _endAngle, t)
            ?? throw new global::System.NullReferenceException("A required value was null.")
        );
        double x =
            Math.Cos(angle)
            * (
                _radius
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        double y =
            Math.Sin(angle)
            * (
                _radius
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        return (
                _center
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ) + new Offset(x, y);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override string ToString()
    {
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "MaterialPointArcTween")}({begin} → {end}; center={center}, radius={radius}, beginAngle={beginAngle}, endAngle={endAngle})";
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal enum _CornerId__arc
{
    topLeft,
    topRight,
    bottomLeft,
    bottomRight,
}

internal class _Diagonal__arc
{
    public virtual _CornerId__arc beginId { get; private set; } = default!;
    public virtual _CornerId__arc endId { get; private set; } = default!;

    internal _Diagonal__arc(_CornerId__arc beginId, _CornerId__arc endId)
    {
        this.beginId = beginId;
        this.endId = endId;
    }
}

public static partial class ArcLibrary
{
    internal static List<_Diagonal__arc> _allDiagonals = new List<_Diagonal__arc>
    {
        new _Diagonal__arc(_CornerId__arc.topLeft, _CornerId__arc.bottomRight),
        new _Diagonal__arc(_CornerId__arc.bottomRight, _CornerId__arc.topLeft),
        new _Diagonal__arc(_CornerId__arc.topRight, _CornerId__arc.bottomLeft),
        new _Diagonal__arc(_CornerId__arc.bottomLeft, _CornerId__arc.topRight),
    };
}

internal delegate double _KeyFunc__arc<T>(T input);

public static partial class ArcLibrary
{
    internal static T _maxBy<T>(IEnumerable<T> input, Func<T, double> keyFunc)
    {
        T maxValue = default!;
        double? maxKey = default!;
        foreach (var value in input)
        {
            double key = keyFunc(value);
            if (
                (maxKey is null)
                || (
                    key
                    > (
                        maxKey
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                )
            )
            {
                maxValue = value;
                maxKey = key;
            }
        }
        return maxValue;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class MaterialRectArcTween : RectTween
{
    internal virtual bool _dirty { get; set; } = true;
    internal virtual MaterialPointArcTween _beginArc { get; set; } = default!;
    internal virtual MaterialPointArcTween _endArc { get; set; } = default!;

    public MaterialRectArcTween(Rect? begin = null, Rect? end = null)
        : base(
            begin: (
                begin
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ),
            end: (
                end ?? throw new global::System.NullReferenceException("A required value was null.")
            )
        ) { }

    internal virtual void _initialize()
    {
        Offset centersVector =
            (
                end ?? throw new global::System.NullReferenceException("A required value was null.")
            ).center
            - (
                begin
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ).center;
        _Diagonal__arc diagonal = ArcLibrary._maxBy(
            ArcLibrary._allDiagonals.Cast<_Diagonal__arc>(),
            (d) => _diagonalSupport(centersVector, d)
        );
        _beginArc = new MaterialPointArcTween(
            begin: _cornerFor(
                (
                    begin
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                diagonal.beginId
            ),
            end: _cornerFor(
                (
                    end
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                diagonal.beginId
            )
        );
        _endArc = new MaterialPointArcTween(
            begin: _cornerFor(
                (
                    begin
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                diagonal.endId
            ),
            end: _cornerFor(
                (
                    end
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                diagonal.endId
            )
        );
        _dirty = false;
    }

    internal virtual double _diagonalSupport(Offset centersVector, _Diagonal__arc diagonal)
    {
        Offset delta =
            _cornerFor(
                (
                    begin
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                diagonal.endId
            )
            - _cornerFor(
                (
                    begin
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                diagonal.beginId
            );
        double length = delta.distance;
        return (centersVector.dx * delta.dx / length) + (centersVector.dy * delta.dy / length);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Offset _cornerFor(Rect rect, _CornerId__arc id)
    {
        return id switch
        {
            _CornerId__arc.topLeft => rect.topLeft,
            _CornerId__arc.topRight => rect.topRight,
            _CornerId__arc.bottomLeft => rect.bottomLeft,
            _CornerId__arc.bottomRight => rect.bottomRight,
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual MaterialPointArcTween? beginArc
    {
        get
        {
            if (begin is null)
            {
                return null;
            }
            if (_dirty)
            {
                _initialize();
            }
            return _beginArc;
        }
    }
    public virtual MaterialPointArcTween? endArc
    {
        get
        {
            if (end is null)
            {
                return null;
            }
            if (_dirty)
            {
                _initialize();
            }
            return _endArc;
        }
    }
    public override Rect? begin
    {
        set
        {
            var __value = value;
            if (!Equals(__value, begin))
            {
                base.begin = __value;
                _dirty = true;
            }
        }
    }
    public override Rect? end
    {
        set
        {
            var __value = value;
            if (!Equals(__value, end))
            {
                base.end = __value;
                _dirty = true;
            }
        }
    }

    public override Rect? lerp(double t)
    {
        if (_dirty)
        {
            _initialize();
        }
        if (t == 0.0)
        {
            return (
                begin
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        }
        if (t == 1.0)
        {
            return (
                end ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        }
        return Rect.fromPoints(_beginArc.lerp(t), _endArc.lerp(t));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override string ToString()
    {
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "MaterialRectArcTween")}({begin} → {end}; beginArc={beginArc}, endArc={endArc})";
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class MaterialRectCenterArcTween : RectTween
{
    internal virtual bool _dirty { get; set; } = true;
    internal virtual MaterialPointArcTween _centerArc { get; set; } = default!;

    public MaterialRectCenterArcTween(Rect? begin = null, Rect? end = null)
        : base(
            begin: (
                begin
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ),
            end: (
                end ?? throw new global::System.NullReferenceException("A required value was null.")
            )
        ) { }

    internal virtual void _initialize()
    {
        _centerArc = new MaterialPointArcTween(
            begin: (
                begin
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ).center,
            end: (
                end ?? throw new global::System.NullReferenceException("A required value was null.")
            ).center
        );
        _dirty = false;
    }

    public virtual MaterialPointArcTween? centerArc
    {
        get
        {
            if (_dirty)
            {
                _initialize();
            }
            return _centerArc;
        }
    }
    public override Rect? begin
    {
        set
        {
            var __value = value;
            if (!Equals(__value, begin))
            {
                base.begin = __value;
                _dirty = true;
            }
        }
    }
    public override Rect? end
    {
        set
        {
            var __value = value;
            if (!Equals(__value, end))
            {
                base.end = __value;
                _dirty = true;
            }
        }
    }

    public override Rect? lerp(double t)
    {
        if (_dirty)
        {
            _initialize();
        }
        if (t == 0.0)
        {
            return (
                begin
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        }
        if (t == 1.0)
        {
            return (
                end ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        }
        Offset center = _centerArc.lerp(t);
        double widthLocal = (
            DorotiUiLibrary.lerpDouble(
                (
                    begin
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ).width,
                (
                    end
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ).width,
                t
            ) ?? throw new global::System.NullReferenceException("A required value was null.")
        );
        double heightLocal = (
            DorotiUiLibrary.lerpDouble(
                (
                    begin
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ).height,
                (
                    end
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ).height,
                t
            ) ?? throw new global::System.NullReferenceException("A required value was null.")
        );
        return Rect.fromLTWH(
            center.dx - (widthLocal / 2.0),
            center.dy - (heightLocal / 2.0),
            widthLocal,
            heightLocal
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override string ToString()
    {
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "MaterialRectCenterArcTween")}({begin} → {end}; centerArc={centerArc})";
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
