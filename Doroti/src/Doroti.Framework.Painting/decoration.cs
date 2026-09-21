// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/decoration.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public abstract class Decoration : Diagnosticable
{
    protected Decoration() { }

    public virtual string toStringShort() =>
        objectRuntimeTypeFunctions.objectRuntimeType(this, "Decoration");

    public virtual bool debugAssertIsValid() => true;

    public virtual EdgeInsetsGeometry padding => EdgeInsets.zero;
    public virtual bool isComplex => false;

    public virtual Decoration? lerpFrom(Decoration? a, double t) => null;

    public virtual Decoration? lerpTo(Decoration? b, double t) => null;

    public static Decoration? lerp(Decoration? a, Decoration? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if (a is null)
        {
            return b!.lerpFrom(null, t) ?? b;
        }
        if (b is null)
        {
            return a.lerpTo(null, t) ?? a;
        }
        if (t == 0.0)
        {
            return a;
        }
        if (t == 1.0)
        {
            return b;
        }
        return (b.lerpFrom(a, t) ?? a.lerpTo(b, t))
            ?? (
                (t < 0.5)
                    ? (a.lerpTo(null, t * 2.0) ?? a)
                    : (b.lerpFrom(null, (t - 0.5) * 2.0) ?? b)
            );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool hitTest(Size size, Offset position, TextDirection? textDirection = null) =>
        true;

    public abstract BoxPainter createBoxPainter(Action onChanged = default!);

    public virtual Path getClipPath(Rect rect, TextDirection textDirection)
    {
        throw new NotSupportedException(
            $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "This Decoration subclass")} does not expect to be used for clipping."
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public abstract class BoxPainter
{
    public virtual Action? onChanged { get; private set; }

    protected BoxPainter(Action? onChanged = null)
    {
        this.onChanged = onChanged;
    }

    public abstract void paint(Canvas canvas, Offset offset, ImageConfiguration configuration);

    public virtual void dispose() { }
}
