// Copyright 2021 The Flutter team. All rights reserved.
// Adapted from reference/flutter_sample_app; BSD license in LICENSE.flutter.
using Doroti.Framework.Foundation;
using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Services;
using Doroti.Framework.Widgets;
using Doroti.Runtime;
using Doroti.Ui;
using M = Doroti.Framework.Material;
using TextStyle = Doroti.Framework.Painting.TextStyle;

namespace MaterialSample;

internal sealed class ComponentsScreen(bool twoColumns, double secondFraction, double secondOffset, GlobalKey<M.ScaffoldState> scaffold) : StatefulWidget
{
    internal bool TwoColumns => twoColumns;
    internal double SecondFraction => secondFraction;
    internal double SecondOffset => secondOffset;
    internal GlobalKey<M.ScaffoldState> Scaffold => scaffold;
    public override IState createState() => new ComponentsState();
}

internal sealed partial class ComponentsState : State<ComponentsScreen>
{
    private readonly ScrollController _firstScroll = new(), _secondScroll = new();
    private readonly double?[] _sectionHeights = new double?[7];
    private readonly TextEditingController _filled = new(), _outlined = new(), _colorMenu = new(), _iconMenu = new();
    private readonly List<string> _history = [];
    private string? _selectedColor;
    private bool _progress, _filtered = true, _switchA, _switchB = true;
    private readonly bool[] _icons = [false, false, false, false];
    private bool? _checkA = true, _checkB, _checkC = false;
    private string _radio = "first", _menuIcon = "Smile";
    private string? _menuColor;
    private double _sliderA = 30, _sliderB = 20;
    private HashSet<string> _single = ["Day"], _multiple = ["L", "XL"];
    private long _barIndex, _badgeIndex = 1, _railIndex;
    private M.PersistentBottomSheetController? _sheet;
    private DateTime? _date;
    private M.TimeOfDay? _time;
    private static readonly string[] SearchColors = ["red", "orange", "yellow", "green", "blue", "indigo", "violet", "purple", "pink", "silver", "gold", "beige", "brown", "grey", "black", "white"];
    private static readonly long[] SearchArgb = [0xfff44336, 0xffff9800, 0xffffeb3b, 0xff4caf50, 0xff2196f3, 0xff3f51b5, 0xff8f00ff, 0xff9c27b0, 0xffe91e63, 0xff808080, 0xffffd700, 0xfff5f5dc, 0xff795548, 0xff9e9e9e, 0xff000000, 0xffffffff];
    public override void dispose()
    {
        _firstScroll.dispose(); _secondScroll.dispose();
        _filled.dispose(); _outlined.dispose(); _colorMenu.dispose(); _iconMenu.dispose();
        base.dispose();
    }
    public override Widget build(BuildContext context)
    {
        Widget List(bool second)
        {
            // Keep estimates across selection changes. Mounted sections refresh their
            // measured height on layout; resetting unseen entries to zero clamps scrolling.
            var firstIndex = second ? 3 : 0;
            var count = second ? 4 : widget.TwoColumns ? 3 : 7;
            return new FocusTraversalGroup(child: new CustomScrollView(
                controller: second ? _secondScroll : _firstScroll, primary: false,
                slivers: [new SliverList(@delegate: new MeasuredSlivers(_sectionHeights, firstIndex, count, (ctx, index) =>
                    new CacheHeight(_sectionHeights, checked((int)index) + firstIndex, Group(ctx, checked((int)index) + firstIndex))))]));
        }
        return new Row(crossAxisAlignment: CrossAxisAlignment.stretch, children:
        [
            new Flexible(flex: 1000, child: List(false)),
            widget.SecondFraction <= 0 ? SizedBox.CreateShrink() : new Flexible(flex: Math.Max(1, (long)(widget.SecondFraction * 1000)),
                child: new FractionalTranslation(translation: new Offset(widget.SecondOffset, 0), child: List(true))),
        ]);
    }
    private Widget Group(BuildContext ctx, int index)
    {
        string[] labels = ["Actions", "Communication", "Containment", "Navigation", "Selection", "Text inputs", "Image demo"];
        Widget content = index switch { 0 => Actions(), 1 => Communication(ctx), 2 => Containment(ctx), 3 => Navigation(ctx), 4 => Selection(ctx), 5 => TextInputs(), _ => new SampleImageDemo() };
        return new Padding(padding: EdgeInsets.CreateOnly(bottom: 10, right: widget.TwoColumns ? 10 : 0), child:
            new FocusTraversalGroup(child: new M.Card(margin: EdgeInsets.zero, elevation: 0,
                color: M.Theme.of(ctx).colorScheme.surfaceContainerHighest.withAlpha(77),
                child: new Padding(padding: EdgeInsets.CreateSymmetric(vertical: 20), child: new Column(children:
                    [new Text(labels[index], style: M.Theme.of(ctx).textTheme.titleLarge), new SizedBox(height: 10), content])))));
    }
    private static Widget Section(string label, params Widget[] children) => new ComponentSection(label,
        new Column(mainAxisSize: MainAxisSize.min, crossAxisAlignment: CrossAxisAlignment.stretch, spacing: 10, children: children.ToList()));
    private static Widget Flow(params Widget[] children) => new Wrap(spacing: 10, runSpacing: 10, children: children.ToList());
    private static void DisplayAction() { } // Pinned reference display-only callbacks.
    private Widget Actions()
    {
        var icon = new Icon(M.Icons.add);
        Widget Buttons(bool disabled, bool withIcon)
        {
            Action? press = disabled ? null : DisplayAction;
            var label = new Text(withIcon ? "Icon" : "Elevated");
            return new Padding(padding: EdgeInsets.CreateSymmetric(horizontal: withIcon ? 10 : 5), child: new IntrinsicWidth(child: new Column(
                crossAxisAlignment: CrossAxisAlignment.stretch, spacing: 10, children:
                [
                    withIcon ? M.ElevatedButton.CreateIcon(onPressed: press, icon: icon, label: label) : new M.ElevatedButton(onPressed: press, child: label),
                    withIcon ? M.FilledButton.CreateIcon(onPressed: press, icon: icon, label: new Text("Icon")) : new M.FilledButton(onPressed: press, child: new Text("Filled")),
                    withIcon ? M.FilledButton.CreateTonalIcon(onPressed: press, icon: icon, label: new Text("Icon")) : M.FilledButton.CreateTonal(onPressed: press, child: new Text("Filled tonal")),
                    withIcon ? M.OutlinedButton.CreateIcon(onPressed: press, icon: icon, label: new Text("Icon")) : new M.OutlinedButton(onPressed: press, child: new Text("Outlined")),
                    withIcon ? M.TextButton.CreateIcon(onPressed: press, icon: icon, label: new Text("Icon")) : new M.TextButton(onPressed: press, child: new Text("Text")),
                ])));
        }
        Widget ToggleIcon(int i, bool enabled) => i switch
        {
            0 => new M.IconButton(icon: new Icon(M.Icons.settings_outlined), selectedIcon: new Icon(M.Icons.settings), isSelected: _icons[i], onPressed: enabled ? () => setState(() => _icons[i] = !_icons[i]) : null),
            1 => M.IconButton.CreateFilled(icon: new Icon(M.Icons.settings_outlined), selectedIcon: new Icon(M.Icons.settings), isSelected: _icons[i], onPressed: enabled ? () => setState(() => _icons[i] = !_icons[i]) : null),
            2 => M.IconButton.CreateFilledTonal(icon: new Icon(M.Icons.settings_outlined), selectedIcon: new Icon(M.Icons.settings), isSelected: _icons[i], onPressed: enabled ? () => setState(() => _icons[i] = !_icons[i]) : null),
            _ => M.IconButton.CreateOutlined(icon: new Icon(M.Icons.settings_outlined), selectedIcon: new Icon(M.Icons.settings), isSelected: _icons[i], onPressed: enabled ? () => setState(() => _icons[i] = !_icons[i]) : null),
        };
        return new Column(children:
        [
            Section("Common buttons", new Center(child: new SingleChildScrollView(scrollDirection: Axis.horizontal, child: new Row(mainAxisAlignment: MainAxisAlignment.spaceAround,
                children: [Buttons(false, false), Buttons(false, true), Buttons(true, false)])))),
            Section("Floating action buttons", new Center(child: new Wrap(crossAxisAlignment: WrapCrossAlignment.center, spacing: 10, runSpacing: 10, children:
                [M.FloatingActionButton.CreateSmall(heroTag: "sample-small", tooltip: "Small", onPressed: DisplayAction, child: icon),
                 M.FloatingActionButton.CreateExtended(heroTag: "sample-extended", tooltip: "Extended", onPressed: DisplayAction, icon: icon, label: new Text("Create")),
                 new M.FloatingActionButton(heroTag: "sample-normal", tooltip: "Standard", onPressed: DisplayAction, child: icon),
                 M.FloatingActionButton.CreateLarge(heroTag: "sample-large", tooltip: "Large", onPressed: DisplayAction, child: icon)]))),
            Section("Icon buttons", new Row(mainAxisAlignment: MainAxisAlignment.spaceAround, children: Enumerable.Range(0, 4).Select(i => (Widget)new Column(spacing: 10,
                children: [ToggleIcon(i, true), ToggleIcon(i, false)])).ToList())),
            Section("Segmented buttons", new M.SegmentedButton<string>(segments: new[] { "Day", "Week", "Month", "Year" }.Select((label, i) => new M.ButtonSegment<string>(value: label, label: new Text(label),
                icon: new Icon(new[] { M.Icons.calendar_view_day, M.Icons.calendar_view_week, M.Icons.calendar_view_month, M.Icons.calendar_today }[i]))).ToList(),
                selected: _single, onSelectionChanged: value => setState(() => _single = value)),
                new M.SegmentedButton<string>(segments: new[] { "XS", "S", "M", "L", "XL" }.Select(label => new M.ButtonSegment<string>(value: label, label: new Text(label))).ToList(),
                    selected: _multiple, multiSelectionEnabled: true, onSelectionChanged: value => setState(() => _multiple = value))),
        ]);
    }
    private Widget Communication(BuildContext ctx) => new Column(children:
    [
        Section("Badge", new M.NavigationBar(selectedIndex: _badgeIndex, onDestinationSelected: value => setState(() => _badgeIndex = value), destinations:
            [new M.NavigationDestination(icon: new M.Badge(label: new Text("999+"), child: new Icon(M.Icons.mail_outline)), selectedIcon: new M.Badge(label: new Text("999+"), child: new Icon(M.Icons.mail)), label: "Mail"),
             new M.NavigationDestination(icon: new M.Badge(label: new Text("10"), child: new Icon(M.Icons.chat_bubble_outline)), selectedIcon: new M.Badge(label: new Text("10"), child: new Icon(M.Icons.chat_bubble)), label: "Chat"),
             new M.NavigationDestination(icon: new M.Badge(child: new Icon(M.Icons.group_outlined)), selectedIcon: new M.Badge(child: new Icon(M.Icons.group_rounded)), label: "Rooms"),
             new M.NavigationDestination(icon: new M.Badge(label: new Text("3"), child: new Icon(M.Icons.videocam_outlined)), selectedIcon: new M.Badge(label: new Text("3"), child: new Icon(M.Icons.videocam)), label: "Meet") ])),
        Section("Progress indicators", new Row(spacing: 10, children:
            [new M.IconButton(tooltip: _progress ? "Stop progress" : "Start progress", isSelected: _progress, selectedIcon: new Icon(M.Icons.pause), icon: new Icon(M.Icons.play_arrow), onPressed: () => setState(() => _progress = !_progress)),
             new M.CircularProgressIndicator(value: _progress ? null : 0.7), new Expanded(child: new M.LinearProgressIndicator(value: _progress ? null : 0.7)), new SizedBox(width: 10)])),
        Section("Snackbar", new M.TextButton(child: new Text("Show snackbar"), onPressed: () => M.ScaffoldMessenger.of(ctx).showSnackBar(new M.SnackBar(
            content: new Text("This is a snackbar"), behavior: M.SnackBarBehavior.floating, action: new M.SnackBarAction(label: "Close", onPressed: DisplayAction))))),
    ]);
    private Widget Containment(BuildContext ctx)
    {
        Widget Sheet(BuildContext sheetContext) => new SizedBox(height: 200, child: new Center(child: new M.TextButton(child: new Text("Close bottom sheet"), onPressed: () => Navigator.of(sheetContext).pop<object>())));
        return new Column(children:
        [
            Section("Bottom sheets", Flow(new M.TextButton(child: new Text("Show modal bottom sheet"), onPressed: () => M.Bottom_sheetLibrary.showModalBottomSheet<object>(ctx, Sheet)),
                new M.TextButton(child: new Text("Show bottom sheet"), onPressed: _sheet is not null ? null : () => OpenSheet(Sheet)))),
            Section("Cards", new Wrap(alignment: WrapAlignment.spaceEvenly, children: [Card(0), Card(1), Card(2)])),
            Section("Carousel", new Text("Uncontained Carousel"), Carousel(false), new Text("Uncontained Carousel with snapping effect"), Carousel(true)),
            Section("Dialogs", Flow(new M.TextButton(child: new Text("Show dialog"), onPressed: () => M.DialogLibrary.showDialog<object>(ctx, dialogContext => new M.AlertDialog(
                    title: new Text("Dialog title"), content: new Text("A dialog is a type of modal window that appears in front of app content."), actions:
                    [new M.TextButton(child: new Text("Cancel"), onPressed: () => Navigator.of(dialogContext).pop<object>()), new M.TextButton(child: new Text("Confirm"), onPressed: () => Navigator.of(dialogContext).pop<object>())]))),
                new M.TextButton(child: new Text("Show fullscreen dialog"), onPressed: () => M.DialogLibrary.showDialog<object>(ctx, dialogContext => M.Dialog.CreateFullscreen(child:
                    new M.Scaffold(appBar: new M.AppBar(title: new Text("Full-screen dialog"), leading: new M.IconButton(icon: new Icon(M.Icons.close), onPressed: () => Navigator.of(dialogContext).pop<object>())),
                        body: new Center(child: new M.TextButton(child: new Text("Close"), onPressed: () => Navigator.of(dialogContext).pop<object>())))))))),
            Section("Dividers", new M.Divider(), new SizedBox(height: 40, child: new Row(children: [new Expanded(child: new Text("Before")), new M.VerticalDivider(), new Expanded(child: new Text("After"))]))),
        ]);
    }
    private Widget Card(int style)
    {
        var child = new Padding(padding: EdgeInsets.CreateFromLTRB(10, 5, 5, 10), child: new Column(children:
            [new Align(alignment: Alignment.topRight, child: new M.IconButton(icon: new Icon(M.Icons.more_vert), onPressed: DisplayAction)),
             new SizedBox(height: 20), new Align(alignment: Alignment.bottomLeft, child: new Text(new[] { "Elevated", "Filled", "Outlined" }[style]))]));
        return new SizedBox(width: 115, child: style switch {
            0 => new M.Card(child: child),
            1 => new M.Card(elevation: 0, color: M.Theme.of(context).colorScheme.surfaceContainerHighest, child: child),
            _ => new M.Card(elevation: 0, shape: new RoundedRectangleBorder(side: new BorderSide(color: M.Theme.of(context).colorScheme.outline), borderRadius: BorderRadius.CreateCircular(12)), child: child) });
    }
    private Widget Carousel(bool snapping) => new SizedBox(height: 150, child: new M.CarouselView(itemSnapping: snapping, itemExtent: 180, shrinkExtent: 100,
        shape: new RoundedRectangleBorder(borderRadius: BorderRadius.CreateCircular(10), side: new BorderSide(color: M.Theme.of(context).colorScheme.outline)), children: Enumerable.Range(0, 20).Select(i => (Widget)new Center(child: new Text($"Item {i}"))).ToList()));
    private async void OpenSheet(Func<BuildContext, Widget> builder)
    {
        var controller = widget.Scaffold.currentState!.showBottomSheet(builder);
        setState(() => _sheet = controller);
        await controller.closed;
        if (mounted) setState(() => _sheet = null);
    }
    private Widget Navigation(BuildContext ctx) => new Column(children:
    [
        Section("Bottom app bar", new SizedBox(height: 80, child: new M.Scaffold(bottomNavigationBar: new M.BottomAppBar(child: new Row(children:
            [Menu(true), new M.IconButton(icon: new Icon(M.Icons.search), tooltip: "Search", onPressed: DisplayAction), new M.IconButton(icon: new Icon(M.Icons.favorite), tooltip: "Favorite", onPressed: DisplayAction)])),
            floatingActionButtonLocation: M.FloatingActionButtonLocation.endContained,
            floatingActionButton: new M.FloatingActionButton(heroTag: "bottom-bar-fab", elevation: 0, child: new Icon(M.Icons.add), onPressed: DisplayAction)))),
        Section("Navigation bar", new M.NavigationBar(selectedIndex: _barIndex, onDestinationSelected: value => setState(() => _barIndex = value), destinations:
            [new M.NavigationDestination(icon: new Icon(M.Icons.explore_outlined), selectedIcon: new Icon(M.Icons.explore), label: "Explore"),
             new M.NavigationDestination(icon: new Icon(M.Icons.pets_outlined), selectedIcon: new Icon(M.Icons.pets), label: "Pets"),
             new M.NavigationDestination(icon: new Icon(M.Icons.account_box_outlined), selectedIcon: new Icon(M.Icons.account_box), label: "Account")])),
        Section("Navigation drawer", new M.TextButton(child: new Text("Show modal end drawer"), onPressed: () => widget.Scaffold.currentState!.openEndDrawer()),
            new SizedBox(height: 520, child: new GalleryDrawer())),
        Section("Navigation rail", new IntrinsicWidth(child: new SizedBox(height: 420, child: new M.NavigationRail(selectedIndex: _railIndex,
            onDestinationSelected: value => setState(() => _railIndex = value), elevation: 4, groupAlignment: 0, labelType: M.NavigationRailLabelType.selected,
            leading: new M.FloatingActionButton(heroTag: "sample-rail-fab", child: new Icon(M.Icons.create), onPressed: DisplayAction),
            destinations: Enumerable.Range(0, 4).Select(i => new M.NavigationRailDestination(icon: new Icon(GalleryDrawer.Icons[i]), selectedIcon: new Icon(GalleryDrawer.SelectedIcons[i]), label: new Text(GalleryDrawer.Labels[i]))).ToList())))),
        Section("Tabs", new M.DefaultTabController(length: 3, child: new SizedBox(height: 80, child: new M.Scaffold(appBar: new M.AppBar(bottom:
            new M.TabBar(tabs: [new M.Tab(text: "Video", icon: new Icon(M.Icons.videocam_outlined), iconMargin: EdgeInsets.zero),
                new M.Tab(text: "Photos", icon: new Icon(M.Icons.photo_outlined), iconMargin: EdgeInsets.zero),
                new M.Tab(text: "Audio", icon: new Icon(M.Icons.audiotrack_sharp), iconMargin: EdgeInsets.zero)])))))),
        Section("Search", M.SearchAnchor.CreateBar(barHintText: "Search colors", suggestionsBuilder: (_, controller) => Suggestions(controller)),
            new Text(_selectedColor is null ? "Select a color" : $"Last selected color is {_selectedColor}")),
        Section("Top app bars", new M.AppBar(title: new Text("Center-aligned"), leading: new M.BackButton(), centerTitle: true,
                actions: [new M.IconButton(iconSize: 32, icon: new Icon(M.Icons.account_circle_outlined), onPressed: DisplayAction)]),
            new M.AppBar(title: new Text("Small"), leading: new M.BackButton(), actions: TopBarActions(), centerTitle: false),
            new SizedBox(height: 100, child: new CustomScrollView(slivers: [M.SliverAppBar.CreateMedium(title: new Text("Medium"), leading: new M.BackButton(), actions: TopBarActions()), new SliverFillRemaining()])),
            new SizedBox(height: 130, child: new CustomScrollView(slivers: [M.SliverAppBar.CreateLarge(title: new Text("Large"), leading: new M.BackButton(), actions: TopBarActions()), new SliverFillRemaining()]))),
    ]);
    private static List<Widget> TopBarActions() => new[] { M.Icons.attach_file, M.Icons.@event, M.Icons.more_vert }
        .Select(icon => (Widget)new M.IconButton(icon: new Icon(icon), onPressed: DisplayAction)).ToList();
    private List<Widget> Suggestions(M.SearchController controller)
    {
        var history = string.IsNullOrEmpty(controller.text);
        if (history && _history.Count == 0) return [new Center(child: new Text("No search history."))];
        var values = history ? _history : SearchColors.Where(color => color.Contains(controller.text, StringComparison.Ordinal));
        return values.Select(color => (Widget)new M.ListTile(title: new Text(color),
            leading: history ? new Icon(M.Icons.history) : new M.CircleAvatar(backgroundColor: new Color(SearchArgb[Array.IndexOf(SearchColors, color)])),
            trailing: new M.IconButton(icon: new Icon(M.Icons.call_missed), onPressed: () => { controller.text = color; controller.selection = TextSelection.CreateCollapsed(offset: color.Length); }),
            onTap: () => { controller.closeView(color); setState(() => { _selectedColor = color; if (_history.Count >= 5) _history.RemoveAt(_history.Count - 1); _history.Insert(0, color); }); })).ToList();
    }
}
