using Doroti.Framework;
using Doroti.Cupertino;
using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Ui;
using TextStyle = Doroti.Framework.Painting.TextStyle;

namespace DorotiSampleApp2;

internal sealed class FontComparisonPage : StatefulWidget
{
    public override IState createState() => new FontComparisonState();
}

internal sealed class FontComparisonState : State<FontComparisonPage>
{
    private string _family = "SUITE Variable";
    private double _weight = 450;
    private readonly TextEditingController _controller = new(text: "한글 입력과 선택 · Hello 0123456789");
    private readonly FocusNode _focusNode = new();

    public override void dispose() { _focusNode.dispose(); _controller.dispose(); base.dispose(); }

    public override Widget build(BuildContext context)
    {
        var children = new List<Widget>
        {
            new Text("서체와 굵기 비교", style: new TextStyle(fontSize: 26, fontWeight: FontWeight.bold)),
            new Row(children: new List<Widget>(new[] { "Roboto", "Galmuri11", "SUITE Variable" }.Select(family =>
                new Expanded(child: new CupertinoButton(child: new Text(family), onPressed: () => setState(() => _family = family)))))),
            new Text($"{_family} · wght {_weight:0}", style: new TextStyle(fontSize: 18)),
            new CupertinoSlider(value: _weight, min: 300, max: 900, onChanged: value => setState(() => _weight = value)),
            new Text("가변 글꼴 ABC abc 0123456789\n폭에 맞춰 줄바꿈되는 한글과 English text.", style: new TextStyle(
                fontFamily: _family, fontSize: 28, fontVariations: new List<FontVariation> { new("wght", _weight) })),
            new SizedBox(height: 16),
            new CupertinoTextField(controller: _controller, focusNode: _focusNode,
                onTapOutside: _ => _focusNode.unfocus(), maxLines: 3, padding: EdgeInsets.CreateAll(12),
                style: new TextStyle(fontFamily: _family, fontSize: 24, fontVariations: new List<FontVariation> { new("wght", _weight) })),
            new SizedBox(height: 20),
        };
        foreach (var weight in new[] { FontWeight.w300, FontWeight.w400, FontWeight.w500, FontWeight.w700, FontWeight.w900 })
            children.Add(new Padding(padding: EdgeInsets.CreateSymmetric(vertical: 6), child: new Text(
                $"{weight.value}  다람쥐 헌 쳇바퀴 ABC 0123", style: new TextStyle(fontFamily: _family, fontWeight: weight, fontSize: 25))));
        return new CupertinoPageScaffold(navigationBar: new CupertinoNavigationBar(middle: new Text("Fonts")),
            child: new SafeArea(child: new SingleChildScrollView(child: new Padding(padding: EdgeInsets.CreateAll(20),
                child: new Column(crossAxisAlignment: CrossAxisAlignment.stretch, children: children)))));
    }
}
