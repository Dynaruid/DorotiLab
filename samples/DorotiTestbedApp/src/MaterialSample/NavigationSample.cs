using Doroti.Framework.Widgets;
using Doroti.Framework.Foundation;
using Doroti.Framework.Services;
using Doroti.Framework.Painting;
using Doroti.Runtime;
using M = Doroti.Framework.Material;

namespace MaterialSample;

public sealed class NavigationSample : StatefulWidget
{
    public override IState createState() => new NavigationSampleState();
}

internal sealed class NavigationSampleState : State<NavigationSample>
{
    private readonly SampleRouter _router = new();
    public override Widget build(BuildContext context) => M.MaterialApp.CreateRouter(
        routerDelegate: _router, routeInformationParser: new SampleRouteParser(), restorationScopeId: "navigation-sample");
    public override void dispose() { _router.Dispose(); base.dispose(); }
}

internal sealed record SampleRoute(string Location, string Text, long Base, long Extent);

internal sealed class SampleRouteParser : RouteInformationParser<SampleRoute>
{
    public override Future<SampleRoute> parseRouteInformation(RouteInformation information)
    {
        var state = information.state as System.Collections.IDictionary;
        return new SynchronousFuture<SampleRoute>(new(information.uri.ToString(), state?["text"] as string ?? "",
            state?["base"] is { } b ? Convert.ToInt64(b) : 0, state?["extent"] is { } e ? Convert.ToInt64(e) : 0));
    }
    public override RouteInformation restoreRouteInformation(SampleRoute configuration) => new(
        uri: DartUri.parse(configuration.Location), state: new DartMap<string, object?>
        { ["text"] = configuration.Text, ["base"] = configuration.Base, ["extent"] = configuration.Extent });
}

internal sealed class SampleRouter : RouterDelegate<SampleRoute>, IDisposable
{
    private readonly HashSet<Action> _listeners = [];
    private readonly TextEditingController _text = new();
    private string _location = "/";
    private bool _restoring;
    public SampleRouter() => _text.addListener(Changed);
    public override void addListener(Action listener) => _listeners.Add(listener);
    public override void removeListener(Action listener) => _listeners.Remove(listener);
    private void Changed() { if (!_restoring) foreach (var listener in _listeners.ToArray()) listener(); }
    private void Go(string page)
    {
        // Hash routes keep the sample deployable on a static file server.
        var current = _location.Split('#')[0];
        _location = current + "#/" + page;
        Changed();
    }
    public override SampleRoute currentConfiguration => new(_location, _text.text,
        _text.selection.baseOffset, _text.selection.extentOffset);
    public override Future setNewRoutePath(SampleRoute configuration)
    {
        _restoring = true;
        try
        {
            _location = configuration.Location;
            _text.value = new(text: configuration.Text, selection: new TextSelection(
                baseOffset: Math.Clamp(configuration.Base, 0, configuration.Text.Length),
                extentOffset: Math.Clamp(configuration.Extent, 0, configuration.Text.Length)));
        }
        finally { _restoring = false; }
        return Future.value();
    }
    public override Future<bool> popRoute() { Go("home"); return Future<bool>.value(true); }
    public override Widget build(BuildContext context)
    {
        if (!OperatingSystem.IsBrowser() && Environment.GetEnvironmentVariable("DOROTI_NAVIGATION_PROBE") is { Length: > 0 } file)
            File.WriteAllText(file, _location + "\n" + _text.text);
        return new M.Scaffold(body: new SafeArea(child: new Padding(padding: EdgeInsets.CreateAll(16), child: new Column(children:
        [
            new Text("Navigation and restoration"),
            new Text("Route: " + _location),
            new M.TextButton(onPressed: () => Go("first"), child: new Text("First page")),
            new M.TextButton(onPressed: () => Go("second"), child: new Text("Second page")),
            new M.TextField(controller: _text),
            new Text("Type text, navigate, use Back/Forward, then reload or restart."),
        ]))));
    }
    public void Dispose() { _text.removeListener(Changed); _text.dispose(); _listeners.Clear(); }
}
