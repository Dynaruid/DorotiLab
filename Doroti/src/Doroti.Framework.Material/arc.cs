// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/arc.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

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


        global::Doroti.Ui.Offset begin__1824 = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(this.begin));
        global::Doroti.Ui.Offset end__1862 = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(this.end));
        global::Doroti.Ui.Offset delta__2031 = ((global::Doroti.Ui.Offset)(object?)(DartRuntimePrimitives.RequireValue(end__1862) - DartRuntimePrimitives.RequireValue(begin__1824)));
        double deltaX__2069 = delta__2031.dx.abs();
        double deltaY__2111 = delta__2031.dy.abs();
        double distanceFromAtoB__2153 = delta__2031.distance;
        var c__2198 = new global::Doroti.Ui.Offset(DartRuntimePrimitives.RequireValue(end__1862).dx, DartRuntimePrimitives.RequireValue(begin__1824).dy);
        double sweepAngle()
        {
            return (2.0 * global::Doroti.Runtime.Dart_mathLibrary.asin((distanceFromAtoB__2153 / ((2.0 * DartRuntimePrimitives.RequireValue(this._radius))))));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        if (((deltaX__2069 > ArcLibrary._kOnAxisDelta) && (deltaY__2111 > ArcLibrary._kOnAxisDelta)))
        {
            if ((deltaX__2069 < deltaY__2111))
            {
                _radius = (((distanceFromAtoB__2153 * distanceFromAtoB__2153) / ((c__2198 - DartRuntimePrimitives.RequireValue(begin__1824))).distance) / 2.0);
                _center = new global::Doroti.Ui.Offset((DartRuntimePrimitives.RequireValue(end__1862).dx + (DartRuntimePrimitives.RequireValue(this._radius) * Math.Sign(((DartRuntimePrimitives.RequireValue(begin__1824).dx - DartRuntimePrimitives.RequireValue(end__1862).dx))))), DartRuntimePrimitives.RequireValue(end__1862).dy);
                if ((DartRuntimePrimitives.RequireValue(begin__1824).dx < DartRuntimePrimitives.RequireValue(end__1862).dx))
                {
                    _beginAngle = (sweepAngle() * Math.Sign(((DartRuntimePrimitives.RequireValue(begin__1824).dy - DartRuntimePrimitives.RequireValue(end__1862).dy))));
                    _endAngle = 0.0;
                }
                else
                {
                    _beginAngle = (Dart_mathLibrary.pi + (sweepAngle() * Math.Sign(((DartRuntimePrimitives.RequireValue(end__1862).dy - DartRuntimePrimitives.RequireValue(begin__1824).dy)))));
                    _endAngle = Dart_mathLibrary.pi;
                }
            }
            else
            {
                _radius = (((distanceFromAtoB__2153 * distanceFromAtoB__2153) / ((c__2198 - DartRuntimePrimitives.RequireValue(end__1862))).distance) / 2.0);
                _center = new global::Doroti.Ui.Offset(DartRuntimePrimitives.RequireValue(begin__1824).dx, (DartRuntimePrimitives.RequireValue(begin__1824).dy + (Math.Sign(((DartRuntimePrimitives.RequireValue(end__1862).dy - DartRuntimePrimitives.RequireValue(begin__1824).dy))) * DartRuntimePrimitives.RequireValue(this._radius))));
                if ((DartRuntimePrimitives.RequireValue(begin__1824).dy < DartRuntimePrimitives.RequireValue(end__1862).dy))
                {
                    _beginAngle = (-Dart_mathLibrary.pi / 2.0);
                    _endAngle = (DartRuntimePrimitives.RequireValue(this._beginAngle) + (sweepAngle() * Math.Sign(((DartRuntimePrimitives.RequireValue(end__1862).dx - DartRuntimePrimitives.RequireValue(begin__1824).dx)))));
                }
                else
                {
                    _beginAngle = (Dart_mathLibrary.pi / 2.0);
                    _endAngle = (DartRuntimePrimitives.RequireValue(this._beginAngle) + (sweepAngle() * Math.Sign(((DartRuntimePrimitives.RequireValue(begin__1824).dx - DartRuntimePrimitives.RequireValue(end__1862).dx)))));
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
            if (false)
            {
                return null;
            }
            if (this._dirty)
            {
                _initialize();
            }
            return this._center;
            return default!;
        }
    }
    public virtual double? radius
    {
        get
        {
            if (false)
            {
                return null;
            }
            if (this._dirty)
            {
                _initialize();
            }
            return this._radius;
            return default!;
        }
    }
    public virtual double? beginAngle
    {
        get
        {
            if (false)
            {
                return null;
            }
            if (this._dirty)
            {
                _initialize();
            }
            return this._beginAngle;
            return default!;
        }
    }
    public virtual double? endAngle
    {
        get
        {
            if (false)
            {
                return null;
            }
            if (this._dirty)
            {
                _initialize();
            }
            return this._beginAngle;
            return default!;
        }
    }
    public override Offset begin
    {
        set
        {
            var __value = value;
            if ((!object.Equals(__value, this.begin)))
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
            if ((!object.Equals(__value, this.end)))
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
        double angle__5482 = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this._beginAngle, this._endAngle, t));
        double x__5547 = (global::Doroti.Runtime.Dart_mathLibrary.cos(angle__5482) * DartRuntimePrimitives.RequireValue(this._radius));
        double y__5596 = (global::Doroti.Runtime.Dart_mathLibrary.sin(angle__5482) * DartRuntimePrimitives.RequireValue(this._radius));
        return (DartRuntimePrimitives.RequireValue(this._center) + new global::Doroti.Ui.Offset(x__5547, y__5596));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "MaterialPointArcTween"))}({this.begin} → {this.end}; center={this.center}, radius={this.radius}, beginAngle={this.beginAngle}, endAngle={this.endAngle})";
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
        T maxValue__6516 = default!;
        double? maxKey__6536 = default!;
        foreach (var value__6557 in input)
        {
            double key__6592 = keyFunc(value__6557);
            if (((maxKey__6536 is null) || (key__6592 > DartRuntimePrimitives.RequireValue(maxKey__6536))))
            {
                maxValue__6516 = value__6557;
                maxKey__6536 = key__6592;
            }
        }
        return maxValue__6516;
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


        global::Doroti.Ui.Offset centersVector__8156 = ((global::Doroti.Ui.Offset)(object?)(((Offset)((dynamic)DartRuntimePrimitives.RequireValue(this.end)).center) - ((Offset)((dynamic)DartRuntimePrimitives.RequireValue(this.begin)).center)));
        _Diagonal__arc diagonal__8221 = ArcLibrary._maxBy<_Diagonal__arc>(ArcLibrary._allDiagonals.Cast<_Diagonal__arc>(), ((d) => _diagonalSupport(centersVector__8156, d)));
        _beginArc = new MaterialPointArcTween(begin: _cornerFor(DartRuntimePrimitives.RequireValue(this.begin), ((_Diagonal__arc)diagonal__8221).beginId), end: _cornerFor(DartRuntimePrimitives.RequireValue(this.end), ((_Diagonal__arc)diagonal__8221).beginId));
        _endArc = new MaterialPointArcTween(begin: _cornerFor(DartRuntimePrimitives.RequireValue(this.begin), ((_Diagonal__arc)diagonal__8221).endId), end: _cornerFor(DartRuntimePrimitives.RequireValue(this.end), ((_Diagonal__arc)diagonal__8221).endId));
        _dirty = false;
    }

    internal virtual double _diagonalSupport(Offset centersVector, _Diagonal__arc diagonal)
    {
        global::Doroti.Ui.Offset delta__8732 = ((global::Doroti.Ui.Offset)(object?)(_cornerFor(DartRuntimePrimitives.RequireValue(this.begin), ((_Diagonal__arc)diagonal).endId) - _cornerFor(DartRuntimePrimitives.RequireValue(this.begin), ((_Diagonal__arc)diagonal).beginId)));
        double length__8832 = delta__8732.distance;
        return (((centersVector.dx * delta__8732.dx) / length__8832) + ((centersVector.dy * delta__8732.dy) / length__8832));
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
            return default!;
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
            return default!;
        }
    }
    public override Rect? begin
    {
        set
        {
            var __value = value;
            if ((!object.Equals(__value, this.begin)))
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
            if ((!object.Equals(__value, this.end)))
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
        return global::Doroti.Ui.Rect.fromPoints(this._beginArc.lerp(t), this._endArc.lerp(t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "MaterialRectArcTween"))}({this.begin} → {this.end}; beginArc={this.beginArc}, endArc={this.endArc})";
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


        _centerArc = new MaterialPointArcTween(begin: ((Offset)((dynamic)DartRuntimePrimitives.RequireValue(this.begin)).center), end: ((Offset)((dynamic)DartRuntimePrimitives.RequireValue(this.end)).center));
        _dirty = false;
    }

    public virtual MaterialPointArcTween? centerArc
    {
        get
        {
            if (false)
            {
                return null;
            }
            if (this._dirty)
            {
                _initialize();
            }
            return this._centerArc;
            return default!;
        }
    }
    public override Rect? begin
    {
        set
        {
            var __value = value;
            if ((!object.Equals(__value, this.begin)))
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
            if ((!object.Equals(__value, this.end)))
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
        global::Doroti.Ui.Offset center__12629 = ((global::Doroti.Ui.Offset)(object?)this._centerArc.lerp(t));
        double width__12675 = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(DartRuntimePrimitives.RequireValue(this.begin).width, DartRuntimePrimitives.RequireValue(this.end).width, t));
        double height__12742 = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(DartRuntimePrimitives.RequireValue(this.begin).height, DartRuntimePrimitives.RequireValue(this.end).height, t));
        return global::Doroti.Ui.Rect.fromLTWH((center__12629.dx - (width__12675 / 2.0)), (center__12629.dy - (height__12742 / 2.0)), width__12675, height__12742);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "MaterialRectCenterArcTween"))}({this.begin} → {this.end}; centerArc={this.centerArc})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
