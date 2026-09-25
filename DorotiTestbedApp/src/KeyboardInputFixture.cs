using System.Text.Json;
using Doroti.Framework.Painting;
using Doroti.Framework.Services;
using Doroti.Framework.Widgets;
using M = Doroti.Framework.Material;

// Explicit validation mode only; captures the framework end of native ingress.
internal sealed class KeyboardInputFixture : StatefulWidget
{
    public override IState createState() => new KeyboardInputState();
}

internal sealed class KeyboardInputState : State<KeyboardInputFixture>
{
    private readonly FocusNode _focus = new();
    private readonly TextEditingController _text = new();

    private static void Record(object data)
    {
        var json = JsonSerializer.Serialize(data);
        Console.WriteLine("DOROTI_KEYBOARD:" + json);
        if (Environment.GetEnvironmentVariable("DOROTI_KEYBOARD_REPORT") is { Length: > 0 } path)
            File.AppendAllText(path, json + Environment.NewLine);
    }

    public override void initState()
    {
        base.initState();
        HardwareKeyboard.instance.addHandler(OnKey);
        Record(new { kind = "ready" });
    }

    private bool OnKey(KeyEvent key)
    {
        Record(new { kind = "key", type = key.GetType().Name,
            physical = key.physicalKey.usbHidUsage, logical = key.logicalKey.keyId,
            key.synthesized, key.character });
        return false;
    }

    public override Widget build(BuildContext context) => new M.Scaffold(
        body: new Padding(padding: EdgeInsets.CreateAll(24), child: new Column(children:
        [
            new KeyboardListener(focusNode: _focus, autofocus: true,
                child: new M.TextButton(onPressed: () => _focus.requestFocus(),
                    child: new Text("Keyboard focus"))),
            new M.TextField(controller: _text,
                decoration: new M.InputDecoration(labelText: "Input"),
                onChanged: value => Record(new { kind = "text", value })),
        ])));

    public override void dispose()
    {
        HardwareKeyboard.instance.removeHandler(OnKey);
        _focus.dispose();
        _text.dispose();
        base.dispose();
    }
}
