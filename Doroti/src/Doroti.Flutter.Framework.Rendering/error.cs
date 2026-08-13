// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/error.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Rendering;

public static partial class ErrorLibrary
{
    internal static double _kMaxWidth = 100000.0;
}

public static partial class ErrorLibrary
{
    internal static double _kMaxHeight = 100000.0;
}

public class RenderErrorBox : RenderBox
{
    public virtual string message { get; private set; } = default!;
    internal virtual Paragraph? _paragraph { get; private set; }
    public static global::Doroti.Generated.Framework.Painting.EdgeInsets padding = new global::Doroti.Generated.Framework.Painting.EdgeInsets(64.0, 96.0, 64.0, 12.0);
    public static double minimumWidth = 200.0;
    public static global::Doroti.Flutter.Ui.Color backgroundColor = _initBackgroundColor();
    public static global::Doroti.Flutter.Ui.TextStyle textStyle = _initTextStyle();
    public static ParagraphStyle paragraphStyle = new global::Doroti.Flutter.Ui.ParagraphStyle(textDirection: TextDirection.ltr, textAlign: TextAlign.left);

    public RenderErrorBox(string message = "")
    {
        this.message = message;
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return ErrorLibrary._kMaxWidth;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return ErrorLibrary._kMaxHeight;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool sizedByParent => true;
    public override bool hitTestSelf(Offset position) => true;
    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return constraints.constrain(new global::Doroti.Flutter.Ui.Size(ErrorLibrary._kMaxWidth, ErrorLibrary._kMaxHeight));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Flutter.Ui.Color _initBackgroundColor()
    {
        var result__4211 = new global::Doroti.Flutter.Ui.Color(4039164096L);
        DartRuntimePrimitives.Assert(() =>
            {
                result__4211 = new global::Doroti.Flutter.Ui.Color(4035969024L);
                return true;
            });
        return result__4211;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Flutter.Ui.TextStyle _initTextStyle()
    {
        var result__4636 = new global::Doroti.Flutter.Ui.TextStyle(color: new global::Doroti.Flutter.Ui.Color(4281348144L), fontFamily: "sans-serif", fontSize: 18.0);
        DartRuntimePrimitives.Assert(() =>
            {
                result__4636 = new global::Doroti.Flutter.Ui.TextStyle(color: new global::Doroti.Flutter.Ui.Color(4294967142L), fontFamily: "monospace", fontSize: 14.0, fontWeight: FontWeight.bold);
                return true;
            });
        return result__4636;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        try
        {
            ((PaintingContext)context).canvas.drawRect((offset & size), ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Flutter.Ui.Paint();
    __cascade.color = backgroundColor;
    return __cascade;
}))());
            if ((this._paragraph is not null))
            {
                double width__5414 = size.width;
                var left__5446 = 0.0;
                var top__5470 = 0.0;
                if ((width__5414 > ((((global::Doroti.Generated.Framework.Painting.EdgeInsets)padding).left + minimumWidth) + ((global::Doroti.Generated.Framework.Painting.EdgeInsets)padding).right)))
                {
                    width__5414 -= (((global::Doroti.Generated.Framework.Painting.EdgeInsets)padding).left + ((global::Doroti.Generated.Framework.Painting.EdgeInsets)padding).right);
                    left__5446 += ((global::Doroti.Generated.Framework.Painting.EdgeInsets)padding).left;
                }
                this._paragraph.layout(new global::Doroti.Flutter.Ui.ParagraphConstraints(width: width__5414));
                if ((size.height > ((((global::Doroti.Generated.Framework.Painting.EdgeInsets)padding).top + this._paragraph.height) + ((global::Doroti.Generated.Framework.Painting.EdgeInsets)padding).bottom)))
                {
                    top__5470 += ((global::Doroti.Generated.Framework.Painting.EdgeInsets)padding).top;
                }
                ((PaintingContext)context).canvas.drawParagraph(this._paragraph, (offset + new global::Doroti.Flutter.Ui.Offset(left__5446, top__5470)));
            }
        }
        catch (Exception error__5922)
        {
        }
    }

}

