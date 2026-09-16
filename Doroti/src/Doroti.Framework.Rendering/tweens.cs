// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/tweens.dart
namespace Doroti.Framework.Rendering;

public class FractionalOffsetTween : Tween<FractionalOffset?>
{
    public FractionalOffsetTween(FractionalOffset? begin = null, FractionalOffset? end = null) : base(begin: begin, end: end)
    {
    }

    public override FractionalOffset? lerp(double t) => FractionalOffset.lerp(begin, end, t);
}

public class AlignmentTween : Tween<Alignment>
{
    public AlignmentTween(Alignment? begin = null, Alignment? end = null) : base(begin: begin, end: end)
    {
    }

    public override Alignment lerp(double t) => Alignment.lerp(begin, end, t)!;
}

public class AlignmentGeometryTween : Tween<AlignmentGeometry?>
{
    public AlignmentGeometryTween(AlignmentGeometry? begin = null, AlignmentGeometry? end = null) : base(begin: begin, end: end)
    {
    }

    public override AlignmentGeometry? lerp(double t) => AlignmentGeometry.lerp(begin, end, t);
}

