// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/decoration.dart
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

public abstract class Decoration : Diagnosticable
{

    protected Decoration()
    {
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "Decoration");
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
        if ((a is null))
        {
            return (b!.lerpFrom(null, t) ?? b);
        }
        if ((b is null))
        {
            return (a.lerpTo(null, t) ?? a);
        }
        if ((t == 0.0))
        {
            return a;
        }
        if ((t == 1.0))
        {
            return b;
        }
        return ((b.lerpFrom(a, t) ?? a.lerpTo(b, t)) ?? (((t < 0.5) ? ((a.lerpTo(null, (t * 2.0)) ?? a)) : ((b.lerpFrom(null, (((t - 0.5)) * 2.0)) ?? b)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool hitTest(Size size, Offset position, TextDirection? textDirection = null) => true;
    public abstract BoxPainter createBoxPainter(Action onChanged = default!);
    public virtual global::Doroti.Ui.Path getClipPath(Rect rect, TextDirection textDirection)
    {
        throw new NotSupportedException($"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "This Decoration subclass"))} does not expect to be used for clipping.");
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
    public virtual void dispose()
    {
    }

}

