// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/thumb_painter.dart
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class Thumb_painterLibrary
{
    internal static Color _kThumbBorderColor = new Color(167772160L);
}

public static partial class Thumb_painterLibrary
{
    internal static List<BoxShadow> _kSwitchBoxShadows = new List<BoxShadow>
    {
        new BoxShadow(color: new Color(637534208L), offset: new Offset(0, 3), blurRadius: 8.0),
        new BoxShadow(color: new Color(251658240L), offset: new Offset(0, 3), blurRadius: 1.0),
    };
}

public static partial class Thumb_painterLibrary
{
    internal static List<BoxShadow> _kSliderBoxShadows = new List<BoxShadow>
    {
        new BoxShadow(color: new Color(637534208L), offset: new Offset(0, 3), blurRadius: 8.0),
        new BoxShadow(color: new Color(687865856L), offset: new Offset(0, 1), blurRadius: 1.0),
        new BoxShadow(color: new Color(436207616L), offset: new Offset(0, 3), blurRadius: 1.0),
    };
}

public class CupertinoThumbPainter
{
    public virtual Color color { get; private set; } = default!;
    public virtual List<BoxShadow> shadows { get; private set; } = default!;
    public const double radius = 14.0;
    public const double extension = 7.0;

    public CupertinoThumbPainter(Color color = default!, List<BoxShadow> shadows = default!)
    {
        Color __color = color ?? CupertinoColors.white;
        List<BoxShadow> __shadows = shadows ?? Thumb_painterLibrary._kSliderBoxShadows;
        this.color = __color;
        this.shadows = __shadows;
    }

    public static CupertinoThumbPainter CreateSwitchThumb(
        Color color = default!,
        List<BoxShadow> shadows = default!
    )
    {
        return new CupertinoThumbPainter(color: color, shadows: shadows);
    }

    public virtual void paint(Canvas canvas, Rect rect)
    {
        var thumbShape = RRect.fromRectAndRadius(rect, Radius.circular(rect.shortestSide / 2.0));
        foreach (BoxShadow shadow in shadows)
        {
            canvas.drawRRect(thumbShape.shift(shadow.offset), shadow.toPaint());
        }
        canvas.drawRRect(
            thumbShape.inflate(0.5),
            (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = new Paint();
                        __cascade.color = Thumb_painterLibrary._kThumbBorderColor;
                        return __cascade;
                    }
                )
            )()
        );
        canvas.drawRRect(
            thumbShape,
            (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = new Paint();
                        __cascade.color = color;
                        return __cascade;
                    }
                )
            )()
        );
    }
}
