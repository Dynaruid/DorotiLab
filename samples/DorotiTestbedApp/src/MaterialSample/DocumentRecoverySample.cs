using Doroti.Material;
using Doroti.Framework.Services;
using Doroti.Framework.Widgets;
using Doroti.Ui;
namespace MaterialSample;

internal sealed class DocumentRecoverySample : StatefulWidget
{
    public override IState createState() => new RecoveryState();
}
internal sealed class RecoveryState : State<DocumentRecoverySample>
{
    private readonly TextEditingController _text = new();
    private string _status = "Draft checkpoint unavailable";
    private string? _path;
    private Doroti.Hosting.DocumentRecoveryStore? _recovery;
    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        if (_path is not null) return;
        _path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "DorotiTestbedApp", "recovery", $"view-{View.of(context).viewId}.txt");
        _recovery = new(_path);
        if (_recovery.TryRead(out var recovered)) _text.text = recovered;
        _status = "Each edit saves an explicit text draft; native close cancellation is a separate capability.";
    }
    public override Widget build(BuildContext context) => new Scaffold(
        appBar: new AppBar(title: new Text("Document recovery")),
        body: new Column(children: [new Text(_status), new TextField(controller: _text, maxLines: 6,
            onChanged: value => {
                try {
                    _recovery!.Save(value);
                    setState(() => _status = "Draft saved. Reopen this owner to recover its text.");
                } catch (Exception error) { setState(() => _status = "Draft save failed: " + error.Message); }
            })]));
    public override void dispose() { _text.dispose(); base.dispose(); }
}
