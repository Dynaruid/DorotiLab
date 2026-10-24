using Doroti.Framework.Widgets;
using Doroti.Ui;
using M = Doroti.Material;

namespace MaterialSample;

/// <summary>Change Message's return value while running `doroti dev` to exercise metadata updates.</summary>
public sealed class HotReloadSample : StatefulWidget
{
    public override IState createState() => new HotReloadState();
}

public sealed class HotReloadState : State<HotReloadSample>
{
    private readonly TextEditingController _text = new();
    private readonly ScrollController _scroll = new();
    private int _count;
    private static string Message() => "Before reload";

    public override Widget build(BuildContext context)
    {
        return new M.Scaffold(body: new Column(children:
        [
            new Text(Message()),
            new M.TextButton(onPressed: () => setState(() => _count++), child: new Text($"Count: {_count}")),
            new M.TextButton(onPressed: () => _scroll.jumpTo(_scroll.offset + 160), child: new Text("Scroll +160")),
            new M.TextField(controller: _text),
            new Expanded(child: ListView.CreateBuilder(controller: _scroll, itemCount: 1000,
                itemBuilder: (_, index) => new SizedBox(height: 40, child: new Text($"Row {index}")))),
        ]));
    }
    public override void dispose()
    {
        _text.dispose(); _scroll.dispose(); base.dispose();
    }
}
