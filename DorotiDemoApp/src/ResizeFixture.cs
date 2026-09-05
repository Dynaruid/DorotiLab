using Doroti.Framework.Widgets;
using Doroti.Framework.Painting;
using Doroti.Ui;
using TextStyle = Doroti.Framework.Painting.TextStyle;

// Matched by reference/flutter_sample_app/lib/resize_fixture.dart. Opt-in only.
internal sealed class ResizeFixture(string kind) : StatelessWidget
{
    private static readonly Widget FixedPicture = new RepaintBoundary(child: new Stack(children:
    [
        new Positioned(left: 0, top: 0, width: 120, height: 80, child: new Container(color: new Color(0xff00a878L))),
        new Positioned(left: 12, top: 12, width: 96, height: 8, child: new Container(color: new Color(0xff173f5fL))),
        new Positioned(left: 12, top: 36, width: 72, height: 8, child: new Container(color: new Color(0xff173f5fL))),
    ]));

    public override Widget build(BuildContext context)
    {
        var children = new List<Widget>
        {
        new Positioned(left: 0, top: 0, right: 0, bottom: 0, child: new Container(color: new Color(0xfff8f8f8L))),
        new Positioned(left: 2, top: 3, width: 22, height: 3, child: new Container(color: new Color(0xffff1744L))),
        new Positioned(right: 1, top: 7, width: 3, height: 19, child: new Container(color: new Color(0xffff1744L))),
        new Positioned(right: 1, bottom: 1, width: 27, height: 3, child: new Container(color: new Color(0xffff1744L))),
        new Center(child: new SizedBox(width: 12, height: 12, child: new Container(color: new Color(0xff2962ffL)))),
        };
        if (kind == "F1") children.Add(new Align(alignment: Alignment.bottomRight,
            child: new SizedBox(width: 120, height: 80, child: FixedPicture)));
        if (kind == "F2") children.Add(new Positioned(left: 32, right: 32, top: 48,
            child: new ClipRect(child: new Text(
                "Resize wrapping fixture: alpha beta gamma delta epsilon zeta eta theta. " +
                "Resize wrapping fixture: alpha beta gamma delta epsilon zeta eta theta.",
                style: new TextStyle(inherit: false, fontFamily: "NanumGothic", fontSize: 24, height: 1.2, color: new Color(0xff173f5fL))))));
        return new Stack(children: children);
    }
}
