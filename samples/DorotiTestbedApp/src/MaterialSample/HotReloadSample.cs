using System.Text.Json;
using Doroti.Framework.Widgets;
using Doroti.Ui;
using M = Doroti.Framework.Material;

namespace MaterialSample;

/// <summary>Change Message's return value while running `doroti dev` to exercise metadata updates.</summary>
public sealed class HotReloadSample : StatefulWidget
{
    public override IState createState() => new HotReloadState();
}

public sealed class HotReloadState : State<HotReloadSample>
{
    private readonly string _identity = Guid.NewGuid().ToString();
    private readonly TextEditingController _text = new();
    private readonly ScrollController _scroll = new();
    private int _count;
    private int _reloads;
    private static string Message() => "Before reload";

    public override void initState()
    {
        base.initState();
        if (!OperatingSystem.IsBrowser() && Environment.GetEnvironmentVariable("DOROTI_RELOAD_PROBE") is { Length: > 0 })
        {
            // Automated state seeding, not a claim of physical keyboard/pointer testing.
            _count = 5;
            _text.text = "한글 유지";
            Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(_ =>
            {
                _scroll.jumpTo(160);
                setState(() => { });
            });
        }
    }
    public override void reassemble() { base.reassemble(); _reloads++; }
    public override Widget build(BuildContext context)
    {
        if (!OperatingSystem.IsBrowser() && Environment.GetEnvironmentVariable("DOROTI_RELOAD_PROBE") is { Length: > 0 } file)
        {
            if (OperatingSystem.IsIOS() && !System.IO.Path.IsPathRooted(file))
                file = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), file);
            if (OperatingSystem.IsAndroid() && !System.IO.Path.IsPathRooted(file))
                file = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), file);
            using var output = File.Create(file);
            using var writer = new Utf8JsonWriter(output);
            writer.WriteStartObject();
            writer.WriteString("stateId", _identity);
            writer.WriteString("message", Message());
            writer.WriteString("text", _text.text);
            writer.WriteNumber("count", _count);
            writer.WriteNumber("scroll", _scroll.hasClients ? _scroll.offset : 0);
            writer.WriteNumber("reassembles", _reloads);
            writer.WriteNumber("processId", Environment.ProcessId);
            writer.WriteEndObject();
        }
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
