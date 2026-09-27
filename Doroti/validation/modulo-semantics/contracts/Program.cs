using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Ui;

var alignment = new Alignment(-5, -7).__(3);
Check(alignment.x == 1 && alignment.y == 2, "alignment negative coordinates");
var directional = new AlignmentDirectional(-5, -7).__(-3);
Check(directional.start == 1 && directional.y == 2, "directional alignment negative divisor");
var insets = new EdgeInsets(-5, -7, -8, -9).__(3);
Check(
    insets.left == 1 && insets.top == 2 && insets.right == 1 && insets.bottom == 0,
    "edge insets"
);
var radius = Radius.elliptical(-5, -7) % -3;
Check(radius.x == 1 && radius.y == 2, "user-defined radius operator delegates to numeric modulo");
var bounds = new BoxConstraints(minWidth: 5, maxWidth: 8, minHeight: 4, maxHeight: 7).__(-3);
Check(
    bounds.minWidth == 2 && bounds.maxWidth == 2 && bounds.minHeight == 1 && bounds.maxHeight == 1,
    "box constraints"
);
var first = new Text("first");
var last = new Text("last");
var wheel = new ListWheelChildLoopingListDelegate(
    new List<Widget> { first, new Text("middle"), last }
);
Check(
    wheel.trueIndexOf(-1) == 2 && wheel.trueIndexOf(-4) == 2 && wheel.trueIndexOf(3) == 0,
    "looping wheel negative indices"
);
Check(
    ReferenceEquals(((IndexedSemantics)wheel.build(null!, -1)!).child, last),
    "negative wheel index builds last child"
);
var hsv = HSVColor.lerp(new HSVColor(1, 10, 1, 1), new HSVColor(1, 20, 1, 1), -2)!;
Check(hsv.hue == 350, "HSV extrapolation wraps negative hue");
var hsl = HSLColor.lerp(new HSLColor(1, 10, 1, 0.5), new HSLColor(1, 20, 1, 0.5), -2)!;
Check(hsl.hue == 350, "HSL extrapolation wraps negative hue");
Console.WriteLine("PASS product modulo contracts");

static void Check(bool value, string message)
{
    if (!value)
        throw new InvalidOperationException(message);
    Console.WriteLine("PASS " + message);
}
