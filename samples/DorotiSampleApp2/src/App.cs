using Doroti.Framework;
using Doroti.Framework.Cupertino;
using Doroti.Framework.Foundation;
using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Hosting;
using Doroti.Runtime;
using Doroti.Ui;
using RouteLibrary = Doroti.Framework.Cupertino.RouteLibrary;
using TextStyle = Doroti.Framework.Painting.TextStyle;

namespace DorotiSampleApp2;

public static class App
{
    public static Func<IDorotiViewEntrypoint> Definition =>
        () => new DorotiWidgetEntrypoint(() => new CupertinoSample(), LoadIconsAsync);

    public static DorotiViewConfiguration ViewConfiguration { get; } = new(
        "Doroti Cupertino Sample",
        new Size(720, 840),
        new Color(0xfff2f2f7L),
        new Color(0xff000000L),
        terminateAfterLastWindowClosed: true
    );

#if DOROTI_FONT_PROBE
    public static Func<Task>? ValidateFontsAsync { get; set; }
#endif

    private static async Task LoadIconsAsync()
    {
#if DOROTI_FONT_PROBE
        if (ValidateFontsAsync is not null) await ValidateFontsAsync();
#endif
        using var stream = typeof(App).Assembly.GetManifestResourceStream("CupertinoSample.icons.ttf")
            ?? throw new InvalidOperationException("Cupertino icon font is missing.");
        using var buffer = new MemoryStream();
        await stream.CopyToAsync(buffer);
        await DorotiUiLibrary.loadFontFromList(
            new Uint8List(buffer.ToArray()), fontFamily: "packages/cupertino_icons/CupertinoIcons");
    }
}

internal sealed class CupertinoSample : StatefulWidget
{
    public override IState createState() => new CupertinoSampleState();
}

internal sealed class CupertinoSampleState : State<CupertinoSample>
{
    private readonly CupertinoTabController _tabs = new(initialIndex: VariableBlurPageState.BenchmarkMode is null ? 0 : 3);
    private Brightness? _brightness;
    private bool _notifications = true;
    private double _volume = 0.5;
    private int _count;
    private string _name = "";
    private readonly TextEditingController _nameController = new();

    public override void dispose()
    {
        _tabs.dispose();
        _nameController.dispose();
        base.dispose();
    }

    public override Widget build(BuildContext context) => new CupertinoApp(
        title: "Doroti Cupertino Sample",
        debugShowCheckedModeBanner: false,
        theme: new CupertinoThemeData(brightness: _brightness, primaryColor: CupertinoColors.systemBlue),
        home: new CupertinoTabScaffold(
            controller: _tabs,
            tabBar: new CupertinoTabBar(items: new List<BottomNavigationBarItem>
            {
                new(icon: new Icon(CupertinoIcons.square_grid_2x2), label: "Components"),
                new(icon: new Icon(CupertinoIcons.person), label: "Profile"),
                new(icon: new Icon(CupertinoIcons.settings), label: "Settings"),
                new(icon: new Icon(CupertinoIcons.list_bullet), label: "Variable Blur"),
                new(icon: new Icon(CupertinoIcons.textformat), label: "Fonts"),
            }),
            tabBuilder: (tabContext, index) => index switch
            {
                1 => Profile(tabContext),
                2 => Settings(tabContext),
                3 => new VariableBlurPage(),
                4 => new FontComparisonPage(),
                _ => Components(tabContext),
            }
        )
    );

    private Widget Page(BuildContext context, string title, params Widget[] children) =>
        new CupertinoPageScaffold(
            navigationBar: new CupertinoNavigationBar(middle: new Text(title)),
            backgroundColor: CupertinoColors.systemGroupedBackground.resolveFrom(context),
            child: new SafeArea(child: new SingleChildScrollView(
                child: new Center(child: new ConstrainedBox(
                    constraints: new BoxConstraints(maxWidth: 680),
                    child: new Column(
                        crossAxisAlignment: CrossAxisAlignment.stretch,
                        children: new List<Widget>(children)
                    )
                ))
            ))
        );

    private static Widget Section(string title, params Widget[] children) =>
        CupertinoListSection.CreateInsetGrouped(
            header: new Text(title), hasLeading: false, children: new List<Widget>(children));

    private static Widget Inset(Widget child) =>
        new Padding(padding: EdgeInsets.CreateAll(16), child: child);

    private Widget Components(BuildContext context) => Page(context, "Components",
        Inset(new Text("Cupertino playground", style: new TextStyle(fontSize: 28, fontWeight: FontWeight.bold))),
        Section("BUTTONS",
            Inset(new Column(crossAxisAlignment: CrossAxisAlignment.stretch, children: new List<Widget>
            {
                new Text($"Button taps: {_count}", textAlign: TextAlign.center),
                new SizedBox(height: 12),
                CupertinoButton.CreateFilled(child: new Text("Tap me"), onPressed: () => setState(() => _count++)),
                new CupertinoButton(child: new Text("Reset counter"), onPressed: () => setState(() => _count = 0)),
            }))
        ),
        Section("CONTROLS",
            new CupertinoListTile(title: new Text("Notifications"), trailing: new CupertinoSwitch(
                value: _notifications, onChanged: value => setState(() => _notifications = value))),
            Inset(new Column(crossAxisAlignment: CrossAxisAlignment.stretch, children: new List<Widget>
            {
                new Text($"Volume: {_volume:P0}"),
                new CupertinoSlider(value: _volume, onChanged: value => setState(() => _volume = value)),
            }))
        ),
        Section("FEEDBACK",
            new CupertinoListTile(title: new Text("Activity indicator"), trailing: new CupertinoActivityIndicator()),
            Inset(CupertinoButton.CreateTinted(child: new Text("Show dialog"), onPressed: () => ShowDialog(context)))
        ),
        new SizedBox(height: 24)
    );

    private Widget Profile(BuildContext context) => Page(context, "Profile",
        Inset(new Icon(CupertinoIcons.person_crop_circle, size: 72, color: CupertinoColors.systemBlue.resolveFrom(context))),
        Inset(new Text(string.IsNullOrWhiteSpace(_name) ? "Welcome to Doroti" : $"Hello, {_name.Trim()}!",
            textAlign: TextAlign.center, style: new TextStyle(fontSize: 26, fontWeight: FontWeight.bold))),
        Section("YOUR PROFILE",
            Inset(new CupertinoTextField(
                controller: _nameController,
                placeholder: "Your name",
                padding: EdgeInsets.CreateAll(12),
                onChanged: value => setState(() => _name = value)
            ))
        ),
        Inset(new Text("Try typing a name, then switch tabs. Your input and control values stay here during this session.",
            textAlign: TextAlign.center))
    );

    private Widget Settings(BuildContext context) => Page(context, "Settings",
        Section("APPEARANCE",
            AppearanceRow("System", null),
            AppearanceRow("Light", Brightness.light),
            AppearanceRow("Dark", Brightness.dark)
        ),
        Section("ABOUT",
            new CupertinoListTile(title: new Text("Application"), additionalInfo: new Text("DorotiSampleApp2")),
            new CupertinoListTile(title: new Text("Design"), additionalInfo: new Text("Cupertino"))
        )
    );

    private Widget AppearanceRow(string label, Brightness? brightness) => new CupertinoListTile(
        title: new Text(label),
        trailing: _brightness == brightness ? new Icon(CupertinoIcons.check_mark, color: CupertinoColors.systemBlue) : null,
        onTap: () => { setState(() => _brightness = brightness); return Task.CompletedTask; }
    );

    private static void ShowDialog(BuildContext context)
    {
        _ = RouteLibrary.showCupertinoDialog<object>(context,
            dialogContext => new CupertinoAlertDialog(
                title: new Text("Hello, Cupertino"),
                content: new Text("A Cupertino dialog in Doroti."),
                actions: new List<Widget>
                {
                    new CupertinoDialogAction(child: new Text("Close"),
                        onPressed: () => Navigator.pop<object>(dialogContext))
                }
            ));
    }
}
