// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/thumb_painter.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Cupertino;

public static partial class Thumb_painterLibrary
{
    internal static Color _kThumbBorderColor = new global::Doroti.Ui.Color(167772160L);
}

public static partial class Thumb_painterLibrary
{
    internal static List<global::Doroti.Framework.Painting.BoxShadow> _kSwitchBoxShadows = new List<global::Doroti.Framework.Painting.BoxShadow> { new global::Doroti.Framework.Painting.BoxShadow(color: new global::Doroti.Ui.Color(637534208L), offset: new global::Doroti.Ui.Offset(0, 3), blurRadius: 8.0), new global::Doroti.Framework.Painting.BoxShadow(color: new global::Doroti.Ui.Color(251658240L), offset: new global::Doroti.Ui.Offset(0, 3), blurRadius: 1.0) };
}

public static partial class Thumb_painterLibrary
{
    internal static List<global::Doroti.Framework.Painting.BoxShadow> _kSliderBoxShadows = new List<global::Doroti.Framework.Painting.BoxShadow> { new global::Doroti.Framework.Painting.BoxShadow(color: new global::Doroti.Ui.Color(637534208L), offset: new global::Doroti.Ui.Offset(0, 3), blurRadius: 8.0), new global::Doroti.Framework.Painting.BoxShadow(color: new global::Doroti.Ui.Color(687865856L), offset: new global::Doroti.Ui.Offset(0, 1), blurRadius: 1.0), new global::Doroti.Framework.Painting.BoxShadow(color: new global::Doroti.Ui.Color(436207616L), offset: new global::Doroti.Ui.Offset(0, 3), blurRadius: 1.0) };
}

public class CupertinoThumbPainter
{
    public virtual Color color { get; private set; } = default!;
    public virtual List<global::Doroti.Framework.Painting.BoxShadow> shadows { get; private set; } = default!;
    public const double radius = 14.0;
    public const double extension = 7.0;

    public CupertinoThumbPainter(Color color = default!, List<global::Doroti.Framework.Painting.BoxShadow> shadows = default!)
    {
        Color __color = color ?? CupertinoColors.white;
        List<global::Doroti.Framework.Painting.BoxShadow> __shadows = shadows ?? Thumb_painterLibrary._kSliderBoxShadows;
        this.color = __color;
        this.shadows = __shadows;
    }

    public static CupertinoThumbPainter CreateSwitchThumb(Color color = default!, List<global::Doroti.Framework.Painting.BoxShadow> shadows = default!)
    {
        return new CupertinoThumbPainter(color: color, shadows: shadows);
    }

    public virtual void paint(Canvas canvas, Rect rect)
    {
        var thumbShape = global::Doroti.Ui.RRect.fromRectAndRadius(rect, global::Doroti.Ui.Radius.circular((rect.shortestSide / 2.0)));
        foreach (global::Doroti.Framework.Painting.BoxShadow shadow in this.shadows)
        {
            canvas.drawRRect(thumbShape.shift(shadow.offset), shadow.toPaint());
        }
        canvas.drawRRect(thumbShape.inflate(0.5), ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = Thumb_painterLibrary._kThumbBorderColor;
    return __cascade;
}))());
        canvas.drawRRect(thumbShape, ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = this.color;
    return __cascade;
}))());
    }

}
