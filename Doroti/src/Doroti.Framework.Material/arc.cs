// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/arc.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class ArcLibrary
{
    internal static double _kOnAxisDelta = 2.0;
}

public class MaterialPointArcTween : global::Doroti.Framework.Animation.Tween<Offset>
{
    internal virtual bool _dirty { get; set; } = true;
    internal virtual Offset? _center { get; set; } = default;
    internal virtual double? _radius { get; set; } = default;
    internal virtual double? _beginAngle { get; set; } = default;
    internal virtual double? _endAngle { get; set; } = default;

    public MaterialPointArcTween(Offset? begin = null, Offset? end = null) : base(begin: DartRuntimePrimitives.RequireValue(begin), end: DartRuntimePrimitives.RequireValue(end))
    {
    }

    internal virtual void _initialize()
    {


        global::Doroti.Ui.Offset beginLocal = ((global::Doroti.Ui.Offset)DartRuntimePrimitives.RequireValue(this.begin));
        global::Doroti.Ui.Offset endLocal = ((global::Doroti.Ui.Offset)DartRuntimePrimitives.RequireValue(this.end));
        global::Doroti.Ui.Offset delta = ((global::Doroti.Ui.Offset)(DartRuntimePrimitives.RequireValue(endLocal) - DartRuntimePrimitives.RequireValue(beginLocal)));
        double deltaX = delta.dx.abs();
        double deltaY = delta.dy.abs();
        double distanceFromAtoB = delta.distance;
        var c = new global::Doroti.Ui.Offset(DartRuntimePrimitives.RequireValue(endLocal).dx, DartRuntimePrimitives.RequireValue(beginLocal).dy);
        double sweepAngle()
        {
            return (2.0 * Dart_mathLibrary.asin((distanceFromAtoB / ((2.0 * DartRuntimePrimitives.RequireValue(this._radius))))));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        if (((deltaX > ArcLibrary._kOnAxisDelta) && (deltaY > ArcLibrary._kOnAxisDelta)))
        {
            if ((deltaX < deltaY))
            {
                _radius = (((distanceFromAtoB * distanceFromAtoB) / ((c - DartRuntimePrimitives.RequireValue(beginLocal))).distance) / 2.0);
                _center = new global::Doroti.Ui.Offset((DartRuntimePrimitives.RequireValue(endLocal).dx + (DartRuntimePrimitives.RequireValue(this._radius) * Math.Sign(((DartRuntimePrimitives.RequireValue(beginLocal).dx - DartRuntimePrimitives.RequireValue(endLocal).dx))))), DartRuntimePrimitives.RequireValue(endLocal).dy);
                if ((DartRuntimePrimitives.RequireValue(beginLocal).dx < DartRuntimePrimitives.RequireValue(endLocal).dx))
                {
                    _beginAngle = (sweepAngle() * Math.Sign(((DartRuntimePrimitives.RequireValue(beginLocal).dy - DartRuntimePrimitives.RequireValue(endLocal).dy))));
                    _endAngle = 0.0;
                }
                else
                {
                    _beginAngle = (Dart_mathLibrary.pi + (sweepAngle() * Math.Sign(((DartRuntimePrimitives.RequireValue(endLocal).dy - DartRuntimePrimitives.RequireValue(beginLocal).dy)))));
                    _endAngle = Dart_mathLibrary.pi;
                }
            }
            else
            {
                _radius = (((distanceFromAtoB * distanceFromAtoB) / ((c - DartRuntimePrimitives.RequireValue(endLocal))).distance) / 2.0);
                _center = new global::Doroti.Ui.Offset(DartRuntimePrimitives.RequireValue(beginLocal).dx, (DartRuntimePrimitives.RequireValue(beginLocal).dy + (Math.Sign(((DartRuntimePrimitives.RequireValue(endLocal).dy - DartRuntimePrimitives.RequireValue(beginLocal).dy))) * DartRuntimePrimitives.RequireValue(this._radius))));
                if ((DartRuntimePrimitives.RequireValue(beginLocal).dy < DartRuntimePrimitives.RequireValue(endLocal).dy))
                {
                    _beginAngle = (-Dart_mathLibrary.pi / 2.0);
                    _endAngle = (DartRuntimePrimitives.RequireValue(this._beginAngle) + (sweepAngle() * Math.Sign(((DartRuntimePrimitives.RequireValue(endLocal).dx - DartRuntimePrimitives.RequireValue(beginLocal).dx)))));
                }
                else
                {
                    _beginAngle = (Dart_mathLibrary.pi / 2.0);
                    _endAngle = (DartRuntimePrimitives.RequireValue(this._beginAngle) + (sweepAngle() * Math.Sign(((DartRuntimePrimitives.RequireValue(beginLocal).dx - DartRuntimePrimitives.RequireValue(endLocal).dx)))));
                }
            }
            DartRuntimePrimitives.Assert(() => (this._beginAngle is not null));
            DartRuntimePrimitives.Assert(() => (this._endAngle is not null));
        }
        else
        {
            _beginAngle = null;
            _endAngle = null;
        }
        _dirty = false;
    }

    public virtual global::Doroti.Ui.Offset? center
    {
        get
        {
            if (this._dirty)
            {
                _initialize();
            }
            return this._center;
        }
    }
    public virtual double? radius
    {
        get
        {
            if (this._dirty)
            {
                _initialize();
            }
            return this._radius;
        }
    }
    public virtual double? beginAngle
    {
        get
        {
            if (this._dirty)
            {
                _initialize();
            }
            return this._beginAngle;
        }
    }
    public virtual double? endAngle
    {
        get
        {
            if (this._dirty)
            {
                _initialize();
            }
            return this._beginAngle;
        }
    }
    public override Offset begin
    {
        set
        {
            var __value = value;
            if ((!Equals(__value, this.begin)))
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
            if ((!Equals(__value, this.end)))
            {
                base.end = __value;
                _dirty = true;
            }
        }
    }
    public override Offset lerp(double t)
    {
        if (this._dirty)
        {
            _initialize();
        }
        if ((t == 0.0))
        {
            return DartRuntimePrimitives.RequireValue(this.begin);
        }
        if ((t == 1.0))
        {
            return DartRuntimePrimitives.RequireValue(this.end);
        }
        if (((this._beginAngle is null) || (this._endAngle is null)))
        {
            return DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Offset.lerp(this.begin, this.end, t));
        }
        double angle = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this._beginAngle, this._endAngle, t));
        double x = (Dart_mathLibrary.cos(angle) * DartRuntimePrimitives.RequireValue(this._radius));
        double y = (Dart_mathLibrary.sin(angle) * DartRuntimePrimitives.RequireValue(this._radius));
        return (DartRuntimePrimitives.RequireValue(this._center) + new global::Doroti.Ui.Offset(x, y));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"{(objectRuntimeTypeFunctions.objectRuntimeType(this, "MaterialPointArcTween"))}({this.begin} → {this.end}; center={this.center}, radius={this.radius}, beginAngle={this.beginAngle}, endAngle={this.endAngle})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal enum _CornerId__arc
{
    topLeft,
    topRight,
    bottomLeft,
    bottomRight
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
    internal static List<_Diagonal__arc> _allDiagonals = new List<_Diagonal__arc> { new _Diagonal__arc(_CornerId__arc.topLeft, _CornerId__arc.bottomRight), new _Diagonal__arc(_CornerId__arc.bottomRight, _CornerId__arc.topLeft), new _Diagonal__arc(_CornerId__arc.topRight, _CornerId__arc.bottomLeft), new _Diagonal__arc(_CornerId__arc.bottomLeft, _CornerId__arc.topRight) };
}

internal delegate double _KeyFunc__arc<T>(T input);

public static partial class ArcLibrary
{
    internal static T _maxBy<T>(IEnumerable<T> input, global::System.Func<T, double> keyFunc)
    {
        T maxValue = default!;
        double? maxKey = default!;
        foreach (var value in input)
        {
            double key = keyFunc(value);
            if (((maxKey is null) || (key > DartRuntimePrimitives.RequireValue(maxKey))))
            {
                maxValue = value;
                maxKey = key;
            }
        }
        return maxValue;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class MaterialRectArcTween : global::Doroti.Framework.Animation.RectTween
{
    internal virtual bool _dirty { get; set; } = true;
    internal virtual MaterialPointArcTween _beginArc { get; set; } = default!;
    internal virtual MaterialPointArcTween _endArc { get; set; } = default!;

    public MaterialRectArcTween(Rect? begin = null, Rect? end = null) : base(begin: DartRuntimePrimitives.RequireValue(begin), end: DartRuntimePrimitives.RequireValue(end))
    {
    }

    internal virtual void _initialize()
    {


        global::Doroti.Ui.Offset centersVector = ((global::Doroti.Ui.Offset)(((Offset)(DartRuntimePrimitives.RequireValue(this.end)).center) - ((Offset)(DartRuntimePrimitives.RequireValue(this.begin)).center)));
        _Diagonal__arc diagonal = ArcLibrary._maxBy<_Diagonal__arc>(ArcLibrary._allDiagonals.Cast<_Diagonal__arc>(), ((d) => _diagonalSupport(centersVector, d)));
        _beginArc = new MaterialPointArcTween(begin: _cornerFor(DartRuntimePrimitives.RequireValue(this.begin), ((_Diagonal__arc)diagonal).beginId), end: _cornerFor(DartRuntimePrimitives.RequireValue(this.end), ((_Diagonal__arc)diagonal).beginId));
        _endArc = new MaterialPointArcTween(begin: _cornerFor(DartRuntimePrimitives.RequireValue(this.begin), ((_Diagonal__arc)diagonal).endId), end: _cornerFor(DartRuntimePrimitives.RequireValue(this.end), ((_Diagonal__arc)diagonal).endId));
        _dirty = false;
    }

    internal virtual double _diagonalSupport(Offset centersVector, _Diagonal__arc diagonal)
    {
        global::Doroti.Ui.Offset delta = ((global::Doroti.Ui.Offset)(_cornerFor(DartRuntimePrimitives.RequireValue(this.begin), ((_Diagonal__arc)diagonal).endId) - _cornerFor(DartRuntimePrimitives.RequireValue(this.begin), ((_Diagonal__arc)diagonal).beginId)));
        double length = delta.distance;
        return (((centersVector.dx * delta.dx) / length) + ((centersVector.dy * delta.dy) / length));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Offset _cornerFor(Rect rect, _CornerId__arc id)
    {
        return (id switch { _CornerId__arc.topLeft => rect.topLeft, _CornerId__arc.topRight => rect.topRight, _CornerId__arc.bottomLeft => rect.bottomLeft, _CornerId__arc.bottomRight => rect.bottomRight, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual MaterialPointArcTween? beginArc
    {
        get
        {
            if ((this.begin is null))
            {
                return null;
            }
            if (this._dirty)
            {
                _initialize();
            }
            return this._beginArc;
        }
    }
    public virtual MaterialPointArcTween? endArc
    {
        get
        {
            if ((this.end is null))
            {
                return null;
            }
            if (this._dirty)
            {
                _initialize();
            }
            return this._endArc;
        }
    }
    public override Rect? begin
    {
        set
        {
            var __value = value;
            if ((!Equals(__value, this.begin)))
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
            if ((!Equals(__value, this.end)))
            {
                base.end = __value;
                _dirty = true;
            }
        }
    }
    public override Rect? lerp(double t)
    {
        if (this._dirty)
        {
            _initialize();
        }
        if ((t == 0.0))
        {
            return DartRuntimePrimitives.RequireValue(this.begin);
        }
        if ((t == 1.0))
        {
            return DartRuntimePrimitives.RequireValue(this.end);
        }
        return Rect.fromPoints(this._beginArc.lerp(t), this._endArc.lerp(t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"{(objectRuntimeTypeFunctions.objectRuntimeType(this, "MaterialRectArcTween"))}({this.begin} → {this.end}; beginArc={this.beginArc}, endArc={this.endArc})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class MaterialRectCenterArcTween : global::Doroti.Framework.Animation.RectTween
{
    internal virtual bool _dirty { get; set; } = true;
    internal virtual MaterialPointArcTween _centerArc { get; set; } = default!;

    public MaterialRectCenterArcTween(Rect? begin = null, Rect? end = null) : base(begin: DartRuntimePrimitives.RequireValue(begin), end: DartRuntimePrimitives.RequireValue(end))
    {
    }

    internal virtual void _initialize()
    {


        _centerArc = new MaterialPointArcTween(begin: ((Offset)(DartRuntimePrimitives.RequireValue(this.begin)).center), end: ((Offset)(DartRuntimePrimitives.RequireValue(this.end)).center));
        _dirty = false;
    }

    public virtual MaterialPointArcTween? centerArc
    {
        get
        {
            if (this._dirty)
            {
                _initialize();
            }
            return this._centerArc;
        }
    }
    public override Rect? begin
    {
        set
        {
            var __value = value;
            if ((!Equals(__value, this.begin)))
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
            if ((!Equals(__value, this.end)))
            {
                base.end = __value;
                _dirty = true;
            }
        }
    }
    public override Rect? lerp(double t)
    {
        if (this._dirty)
        {
            _initialize();
        }
        if ((t == 0.0))
        {
            return DartRuntimePrimitives.RequireValue(this.begin);
        }
        if ((t == 1.0))
        {
            return DartRuntimePrimitives.RequireValue(this.end);
        }
        global::Doroti.Ui.Offset center = ((global::Doroti.Ui.Offset)this._centerArc.lerp(t));
        double widthLocal = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(DartRuntimePrimitives.RequireValue(this.begin).width, DartRuntimePrimitives.RequireValue(this.end).width, t));
        double heightLocal = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(DartRuntimePrimitives.RequireValue(this.begin).height, DartRuntimePrimitives.RequireValue(this.end).height, t));
        return Rect.fromLTWH((center.dx - (widthLocal / 2.0)), (center.dy - (heightLocal / 2.0)), widthLocal, heightLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"{(objectRuntimeTypeFunctions.objectRuntimeType(this, "MaterialRectCenterArcTween"))}({this.begin} → {this.end}; centerArc={this.centerArc})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
