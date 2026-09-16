// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/error.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

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
    public static global::Doroti.Framework.Painting.EdgeInsets padding = new global::Doroti.Framework.Painting.EdgeInsets(64.0, 96.0, 64.0, 12.0);
    public static double minimumWidth = 200.0;
    public static global::Doroti.Ui.Color backgroundColor = _initBackgroundColor();
    public static global::Doroti.Ui.TextStyle textStyle = _initTextStyle();
    public static ParagraphStyle paragraphStyle = new global::Doroti.Ui.ParagraphStyle(textDirection: TextDirection.ltr, textAlign: TextAlign.left);

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
        return constraints.constrain(new global::Doroti.Ui.Size(ErrorLibrary._kMaxWidth, ErrorLibrary._kMaxHeight));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Ui.Color _initBackgroundColor()
    {
        var result = new global::Doroti.Ui.Color(4039164096L);
        DartRuntimePrimitives.Assert(() =>
            {
                result = new global::Doroti.Ui.Color(4035969024L);
                return true;
            });
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Ui.TextStyle _initTextStyle()
    {
        var result = new global::Doroti.Ui.TextStyle(color: new global::Doroti.Ui.Color(4281348144L), fontFamily: "sans-serif", fontSize: 18.0);
        DartRuntimePrimitives.Assert(() =>
            {
                result = new global::Doroti.Ui.TextStyle(color: new global::Doroti.Ui.Color(4294967142L), fontFamily: "monospace", fontSize: 14.0, fontWeight: FontWeight.bold);
                return true;
            });
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        try
        {
            context.canvas.drawRect(offset & size, ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = backgroundColor;
    return __cascade;
}))());
            if (_paragraph is not null)
            {
                double widthLocal = size.width;
                var leftLocal = 0.0;
                var topLocal = 0.0;
                if (widthLocal > (padding.left + minimumWidth + padding.right))
                {
                    widthLocal -= padding.left + padding.right;
                    leftLocal += padding.left;
                }
                _paragraph.layout(new global::Doroti.Ui.ParagraphConstraints(width: widthLocal));
                if (size.height > (padding.top + _paragraph.height + padding.bottom))
                {
                    topLocal += padding.top;
                }
                context.canvas.drawParagraph(_paragraph, offset + new global::Doroti.Ui.Offset(leftLocal, topLocal));
            }
        }
        catch (Exception)
        {
        }
    }

}

