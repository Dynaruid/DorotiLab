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

internal sealed class ComponentsScreen(bool twoColumns, GlobalKey<M.ScaffoldState> scaffold, Key? key = null) : StatefulWidget(key: key)
{
    internal bool TwoColumns => twoColumns;
    internal GlobalKey<M.ScaffoldState> Scaffold => scaffold;
    public override IState createState() => new ComponentsState();
}

internal sealed partial class ComponentsState : State<ComponentsScreen>
{
    private readonly ScrollController _firstScroll = new(), _secondScroll = new();
    private readonly TextEditingController _filled = new(), _outlined = new(), _colorMenu = new(), _iconMenu = new();
    private readonly List<string> _history = [];
    private string? _selectedColor;
    private readonly GlobalKey<ProgressIndicatorsState> _progressKey = new();
    private static readonly bool BroadProgressScope = Environment.GetEnvironmentVariable("DOROTI_SAMPLE_PROGRESS_SCOPE") == "broad";
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
    internal bool OwnsScrollNotification(ScrollNotification notification)
    {
        // Observer depth describes the notification's route. Check its source
        // as well: inline demos and drawers have independent scroll positions.
        var source = notification.context is { } context ? Scrollable.maybeOf(context)?.position : null;
        return source is not null && (_firstScroll.positions.Contains(source) || _secondScroll.positions.Contains(source));
    }
    public override void dispose()
    {
        _sectionFocus?.Dispose();
        if (IndexedSections) PaintingBinding.instance.systemFonts.removeListener(FontsChanged);
        _firstScroll.dispose(); _secondScroll.dispose();
        _filled.dispose(); _outlined.dispose(); _colorMenu.dispose(); _iconMenu.dispose();
        base.dispose();
    }
    private SectionEntry[]? _sections;
    private static readonly string SectionViewport = ReadSectionViewport();
    private static readonly bool LazySections = SectionViewport == "sliver-list";
    private static readonly bool IndexedSections = SectionViewport == "indexed";
    private static string ReadSectionViewport() => Environment.GetEnvironmentVariable("DOROTI_SAMPLE_SECTION_VIEWPORT") switch
    {
        null or "" or "eager" => "eager",
        "sliver-list" => "sliver-list",
        "indexed" => "indexed",
        _ => throw new ArgumentException("DOROTI_SAMPLE_SECTION_VIEWPORT must be eager, sliver-list, or indexed."),
    };
    private readonly HashSet<int> _visitedSections = [];
    private readonly Dictionary<(int First, int Count), SectionExtentIndex> _indices = new();
    private SectionExtentIndex? _activeFirstIndex;
    private (int Index, double Offset)? _primaryAnchor;
    private object? _metricRevision;
    private bool _placementChanged;
    private bool _secondVisited;
    private double _lastRightWidth;
    private bool SectionOwnedBy(int id, ScrollController controller)
    {
        var context = _sectionKeys[id].currentContext;
        var scrollable = context is null ? null : Scrollable.maybeOf(context);
        return scrollable is not null && controller.positions.Contains(scrollable.position);
    }
    private long _fontGeneration;
    private SectionFocusCoordinator? _sectionFocus;
    private void MaterializeSection(int id)
    {
        var second = widget.TwoColumns && id >= _split;
        var first = second ? _split : 0;
        var count = second ? _sections!.Length - _split : widget.TwoColumns ? _split : _sections!.Length;
        SectionList.RequestItem(second ? _secondScroll : _firstScroll, _indices[(first, count)], id - first);
    }
    public override void initState()
    {
        base.initState();
        if (IndexedSections) PaintingBinding.instance.systemFonts.addListener(FontsChanged);
    }
    private void FontsChanged()
    {
        if (mounted) setState(() => { _fontGeneration++; _lists.Clear(); });
    }
    private void InvalidateSection(int id)
    {
        foreach (var (range, index) in _indices)
            if (id >= range.First && id < range.First + range.Count) index.Invalidate(id - range.First);
    }
    public override void didUpdateWidget(ComponentsScreen oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (oldWidget.TwoColumns != widget.TwoColumns)
        {
            if (_activeFirstIndex is { } index) _primaryAnchor = (index.AnchorIndex, index.AnchorOffset);
            _placementChanged = true;
            _lists.Clear();
        }
    }
    private readonly List<GlobalKey<IState>> _sectionKeys = [];
    private readonly List<GlobalKey<IState>> _sectionOwnerKeys = [];
    private readonly Dictionary<(int First, int Count), Widget> _lists = new();
    private int _split;
    internal long SectionBuildCount { get; private set; }
    internal long SectionFirstBuildCount { get; private set; }
    private SectionEntry[] CreateSections(BuildContext context)
    {
        // Descriptors hold identities and builders only. Theme/context are supplied
        // by the mounted view on every build, never captured in a factory cache.
        (int Count, Func<BuildContext, StateSetter, int, Widget> Build)[] groups =
            [(4, Actions), (3, Communication), (5, Containment), (7, Navigation),
             (8, Selection), (1, (_, _, _) => TextInputs()), (1, (_, _, _) => new SampleImageDemo())];
        if (IndexedSections) _sectionFocus ??= new SectionFocusCoordinator(Enumerable.Range(0, groups.Sum(group => group.Count)), MaterializeSection);
        var entries = new List<SectionEntry>();
        for (var group = 0; group < groups.Length; group++)
        {
            var (count, builder) = groups[group];
            for (var index = 0; index < count; index++)
            {
                var sectionIndex = index;
                // State data belongs to the screen; each mounted demo owns its
                // redraw scope. The local context keeps inherited dependencies local.
                if (_sectionKeys.Count == entries.Count)
                { _sectionKeys.Add(new GlobalKey<IState>()); _sectionOwnerKeys.Add(new GlobalKey<IState>()); }
                var sectionId = entries.Count;
                var built = false;
                Widget child = new StatefulBuilder(key: _sectionKeys[entries.Count], builder: (ctx, change) =>
                {
                    SectionBuildCount++;
                    _visitedSections.Add(sectionId);
                    if (!built) { built = true; SectionFirstBuildCount++; }
                    var content = builder(ctx, fn => { if (ctx.mounted) change(() => { fn(); InvalidateSection(sectionId); }); }, sectionIndex);
                    return _sectionFocus?.Wrap(sectionId, content) ?? content;
                });
                var entry = new SectionEntry(group, index == 0, index == count - 1, child);
                entries.Add(entry with { Child = new Builder(builder: ctx => GroupPiece(ctx, entry)) });
            }
            if (group == 2) _split = entries.Count;
        }
        return entries.ToArray();
    }
    public override Widget build(BuildContext context)
    {
        if (IndexedSections)
        {
            // Conservative metric invalidation includes every theme change.
            // Color-only cache retention can be added after metric parity tests.
            object revision = (M.Theme.of(context), MediaQuery.textScalerOf(context), Directionality.of(context), _fontGeneration);
            if (!Equals(revision, _metricRevision)) { _metricRevision = revision; _lists.Clear(); }
        }
        var sections = _sections ??= CreateSections(context);
        Widget List(bool second)
        {
            var firstIndex = second ? _split : 0;
            var count = second ? sections.Length - _split : widget.TwoColumns ? _split : sections.Length;
            if (!_lists.TryGetValue((firstIndex, count), out var list))
            {
                Widget? LazyChild(BuildContext _, long index) =>
                    IndexedSections && second && !widget.TwoColumns && !SectionOwnedBy(firstIndex + (int)index, _secondScroll)
                    ? null : new KeepAlive(key: IndexedSections ? _sectionOwnerKeys[firstIndex + (int)index] : new ValueKey<int>(firstIndex + (int)index), keepAlive: true,
                    child: new IndexedSemantics(index: index, child: new RepaintBoundary(child: sections[firstIndex + (int)index].Child)));
                var children = new SliverChildBuilderDelegate(LazyChild, childCount: count,
                    findChildIndexCallback: key =>
                    {
                        var id = IndexedSections ? _sectionOwnerKeys.FindIndex(ownerKey => ReferenceEquals(ownerKey, key))
                            : key is ValueKey<int> value ? value.value : -1;
                        return id >= firstIndex && id < firstIndex + count ? id - firstIndex : null;
                    },
                    addAutomaticKeepAlives: false, addRepaintBoundaries: false, addSemanticIndexes: false);
                if (!_indices.TryGetValue((firstIndex, count), out var extentIndex))
                    _indices[(firstIndex, count)] = extentIndex = new SectionExtentIndex(count);
                if (!second) _activeFirstIndex = extentIndex;
                if (_placementChanged)
                {
                    var anchor = _primaryAnchor;
                    if (anchor is { } primary && primary.Index >= firstIndex && primary.Index < firstIndex + count)
                        extentIndex.RequestItem(primary.Index - firstIndex, primary.Offset);
                    else extentIndex.RequestItem(extentIndex.AnchorIndex, extentIndex.AnchorOffset);
                }
                list = new FocusTraversalGroup(child: new CustomScrollView(
                    controller: second ? _secondScroll : _firstScroll, primary: false,
                    cacheExtent: IndexedSections ? 0 : null,
                    // This finite gallery has heterogeneous sections. Lay each one out
                    // before scrolling so the viewport knows the actual end immediately.
                    // Separate slivers cull offscreen paint; boundaries retain each
                    // section's drawing without rasterizing one gallery-sized picture.
                    slivers: IndexedSections
                        ? [new SectionList(children, extentIndex, metricRevision: _metricRevision,
                            retainIndices: _visitedSections.Where(id => id >= firstIndex && id < firstIndex + count &&
                                (second || widget.TwoColumns || id < _split || SectionOwnedBy(id, _firstScroll))).Select(id => id - firstIndex).ToArray(),
                            suspended: second && !widget.TwoColumns,
                            restoreRetainedChildren: second && widget.TwoColumns && _placementChanged)]
                        : LazySections ? [new SliverList(@delegate: children)]
                        : sections.Skip(firstIndex).Take(count).Select(section => (Widget)
                            new SliverToBoxAdapter(child: new RepaintBoundary(child: section.Child))).ToList()));
                _lists.Add((firstIndex, count), list);
            }
            return new Padding(padding: EdgeInsets.CreateOnly(right: widget.TwoColumns || (IndexedSections && second) ? 10 : 0), child: list);
        }
        Widget result;
        if (IndexedSections)
        {
            _secondVisited |= widget.TwoColumns;
            var firstList = List(false);
            var secondList = _secondVisited ? List(true) : SizedBox.CreateShrink();
            // Keep the right scroll owner mounted once visited. Hidden sections
            // stay there until the single-column viewport actually needs them.
            // Preserve its last finite width while its occupied width is zero.
            result = new LayoutBuilder(builder: (_, constraints) =>
            {
                if (widget.TwoColumns) _lastRightWidth = constraints.maxWidth / 2;
                return new Row(crossAxisAlignment: CrossAxisAlignment.stretch, children:
                [
                    new Expanded(child: firstList),
                    new SizedBox(width: widget.TwoColumns ? _lastRightWidth : 0,
                        child: new Offstage(offstage: !widget.TwoColumns,
                            child: new OverflowBox(minWidth: _lastRightWidth, maxWidth: _lastRightWidth,
                                alignment: Alignment.topLeft,
                                child: new TickerMode(enabled: widget.TwoColumns,
                                    child: new ExcludeFocus(excluding: !widget.TwoColumns, child: secondList))))),
                ]);
            });
        }
        else result = new Row(crossAxisAlignment: CrossAxisAlignment.stretch, children:
        [
            new Flexible(flex: 1000, child: List(false)),
            widget.TwoColumns ? new Flexible(flex: 1000, child: List(true)) : SizedBox.CreateShrink(),
        ]);
        _placementChanged = false;
        return IndexedSections ? new Listener(onPointerDown: _ => _sectionFocus?.CancelPendingTraversal(), child: result) : result;
    }

    private sealed record SectionEntry(int Group, bool First, bool Last, Widget Child);
    private Widget GroupPiece(BuildContext ctx, SectionEntry entry)
    {
        if (entry.Group == 6) return entry.Child; // ImageDemo owns its card, as in Flutter.
        string[] labels = ["Actions", "Communication", "Containment", "Navigation", "Selection", "Text inputs", "Image demo"];
        var content = entry.Child;
        // Keep the continuous group card while retaining an independent layout,
        // state and paint boundary for each component section.
        var radius = new Radius(12, 12);
        var shape = new RoundedRectangleBorder(borderRadius: BorderRadius.CreateOnly(
            topLeft: entry.First ? radius : default, topRight: entry.First ? radius : default,
            bottomLeft: entry.Last ? radius : default, bottomRight: entry.Last ? radius : default));
        return new Padding(padding: EdgeInsets.CreateOnly(bottom: entry.Last ? 10 : 0), child:
            new FocusTraversalGroup(child: new M.Card(margin: EdgeInsets.zero, elevation: 0,
                shape: shape,
                color: M.Theme.of(ctx).colorScheme.surfaceContainerHighest.withAlpha(77),
                child: new Padding(padding: EdgeInsets.CreateOnly(top: entry.First ? 20 : 0, bottom: entry.Last ? 20 : 0),
                    child: entry.First ? new Column(children:
                        [new Text(labels[entry.Group], style: M.Theme.of(ctx).textTheme.titleLarge), new SizedBox(height: 10), content]) : content))));
    }
    // Preserve each demo's preferred width, as in the reference. Stretching a
    // Drawer makes its InkWell wider than the centered selection indicator.
    private static Widget Section(string label, params Widget[] children) => new ComponentSection(label,
        new Column(mainAxisSize: MainAxisSize.min, children: children.ToList()));
    private static Widget SpacedSection(string label, params Widget[] children) => new ComponentSection(label,
        new Column(mainAxisSize: MainAxisSize.min, spacing: 10, children: children.ToList()));
    private static Widget Flow(params Widget[] children) => new Wrap(spacing: 10, runSpacing: 10, children: children.ToList());
    private static Widget Bold(string label) => new Text(label, style: new TextStyle(fontWeight: FontWeight.bold));
    private static void DisplayAction() { } // Pinned reference display-only callbacks.
    private Widget Actions(BuildContext ctx, StateSetter setState, int sectionIndex)
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
        return sectionIndex switch
        {
            0 => Section("Common buttons", new Center(child: new SingleChildScrollView(scrollDirection: Axis.horizontal, child: new Row(mainAxisAlignment: MainAxisAlignment.spaceAround,
                children: [Buttons(false, false), Buttons(false, true), Buttons(true, false)])))),
            1 => Section("Floating action buttons", new Center(child: new Wrap(crossAxisAlignment: WrapCrossAlignment.center, spacing: 10, runSpacing: 10, children:
                [M.FloatingActionButton.CreateSmall(heroTag: "sample-small", tooltip: "Small", onPressed: DisplayAction, child: icon),
                 M.FloatingActionButton.CreateExtended(heroTag: "sample-extended", tooltip: "Extended", onPressed: DisplayAction, icon: icon, label: new Text("Create")),
                 new M.FloatingActionButton(heroTag: "sample-normal", tooltip: "Standard", onPressed: DisplayAction, child: icon),
                 M.FloatingActionButton.CreateLarge(heroTag: "sample-large", tooltip: "Large", onPressed: DisplayAction, child: icon)]))),
            2 => Section("Icon buttons", new Row(mainAxisAlignment: MainAxisAlignment.spaceAround, children: Enumerable.Range(0, 4).Select(i => (Widget)new Column(spacing: 10,
                children: [ToggleIcon(i, true), ToggleIcon(i, false)])).ToList())),
            3 => SpacedSection("Segmented buttons", new StatefulBuilder(builder: (_, change) => new M.SegmentedButton<string>(segments: new[] { "Day", "Week", "Month", "Year" }.Select((label, i) => new M.ButtonSegment<string>(value: label, label: new Text(label),
                icon: new Icon(new[] { M.Icons.calendar_view_day, M.Icons.calendar_view_week, M.Icons.calendar_view_month, M.Icons.calendar_today }[i]))).ToList(),
                selected: _single, onSelectionChanged: value => change(() => _single = value))),
                new StatefulBuilder(builder: (_, change) => new M.SegmentedButton<string>(segments: new[] { "XS", "S", "M", "L", "XL" }.Select(label => new M.ButtonSegment<string>(value: label, label: new Text(label))).ToList(),
                    selected: _multiple, multiSelectionEnabled: true, onSelectionChanged: value => change(() => _multiple = value)))),
            _ => throw new ArgumentOutOfRangeException(nameof(sectionIndex)),
        };
    }
    private Widget Communication(BuildContext ctx, StateSetter setState, int sectionIndex) => sectionIndex switch
    {
        0 => Section("Badges", new M.NavigationBar(selectedIndex: _badgeIndex, onDestinationSelected: value => setState(() => _badgeIndex = value), destinations:
            [new M.NavigationDestination(icon: new M.Badge(label: new Text("999+"), child: new Icon(M.Icons.mail_outline)), selectedIcon: new M.Badge(label: new Text("999+"), child: new Icon(M.Icons.mail)), label: "Mail"),
             new M.NavigationDestination(icon: new M.Badge(label: new Text("10"), child: new Icon(M.Icons.chat_bubble_outline)), selectedIcon: new M.Badge(label: new Text("10"), child: new Icon(M.Icons.chat_bubble)), label: "Chat"),
             new M.NavigationDestination(icon: new M.Badge(child: new Icon(M.Icons.group_outlined)), selectedIcon: new M.Badge(child: new Icon(M.Icons.group_rounded)), label: "Rooms"),
             new M.NavigationDestination(icon: new M.Badge(label: new Text("3"), child: new Icon(M.Icons.videocam_outlined)), selectedIcon: new M.Badge(label: new Text("3"), child: new Icon(M.Icons.videocam)), label: "Meet") ])),
        1 => BroadProgressScope ? Section("Progress indicators", new Row(children:
            [new M.IconButton(tooltip: _progress ? "Stop progress" : "Start progress", isSelected: _progress, selectedIcon: new Icon(M.Icons.pause), icon: new Icon(M.Icons.play_arrow), onPressed: () => this.setState(() => { _progress = !_progress; _sections = null; _lists.Clear(); })),
             new SizedBox(width: 20), new M.CircularProgressIndicator(value: _progress ? null : 0.7), new SizedBox(width: 20), new Expanded(child: new M.LinearProgressIndicator(value: _progress ? null : 0.7)), new SizedBox(width: 20)])) : new ProgressIndicators(key: _progressKey),
        2 => Section("Snackbar", new M.TextButton(child: Bold("Show snackbar"), onPressed: () => M.ScaffoldMessenger.of(ctx).showSnackBar(new M.SnackBar(
            content: new Text("This is a snackbar"), width: 400, behavior: M.SnackBarBehavior.floating, action: new M.SnackBarAction(label: "Close", onPressed: DisplayAction))))),
        _ => throw new ArgumentOutOfRangeException(nameof(sectionIndex)),
    };
    private Widget Containment(BuildContext ctx, StateSetter setState, int sectionIndex)
    {
        Widget Sheet(BuildContext sheetContext) => new SizedBox(height: 150, child: new Padding(padding: EdgeInsets.CreateSymmetric(horizontal: 32),
            child: new ListView(shrinkWrap: true, scrollDirection: Axis.horizontal, children:
                new[] { M.Icons.share_outlined, M.Icons.add, M.Icons.delete_outline, M.Icons.archive_outlined, M.Icons.settings_outlined, M.Icons.favorite_border }
                    .Select((icon, i) => (Widget)new Padding(padding: EdgeInsets.CreateFromLTRB(20, 30, 20, 20), child: new Column(children:
                        [new M.IconButton(icon: new Icon(icon), onPressed: DisplayAction), new Text(new[] { "Share", "Add to", "Trash", "Archive", "Settings", "Favorite" }[i])]))).ToList())));
        return sectionIndex switch
        {
            0 => Section("Bottom sheet", new Wrap(alignment: WrapAlignment.spaceEvenly, children: [
                new M.TextButton(child: Bold("Show modal bottom sheet"), onPressed: () => M.Bottom_sheetLibrary.showModalBottomSheet<object>(ctx, Sheet, showDragHandle: true, constraints: new BoxConstraints(maxWidth: 640))),
                new M.TextButton(child: Bold(_sheet is null ? "Show bottom sheet" : "Hide bottom sheet"), onPressed: () => { if (_sheet is not null) _sheet.close(); else OpenSheet(Sheet, setState); })])),
            1 => Section("Cards", new Wrap(alignment: WrapAlignment.spaceEvenly, children: [Card(ctx, 0), Card(ctx, 1), Card(ctx, 2)])),
            2 => Section("Carousel", new Column(crossAxisAlignment: CrossAxisAlignment.start, children: [
                new Padding(padding: EdgeInsets.CreateOnly(left: 8), child: new Text("Uncontained Carousel")), Carousel(ctx, false), new SizedBox(height: 10),
                new Padding(padding: EdgeInsets.CreateOnly(left: 8), child: new Text("Uncontained Carousel with snapping effect")), Carousel(ctx, true)])),
            3 => Section("Dialog", new Wrap(alignment: WrapAlignment.spaceBetween, children: [
                new M.TextButton(child: Bold("Show dialog"), onPressed: () => M.DialogLibrary.showDialog<object>(ctx, dialogContext => new M.AlertDialog(
                    title: new Text("What is a dialog?"), content: new Text("A dialog is a type of modal window that appears in front of app content to provide critical information, or prompt for a decision to be made."), actions:
                    [new M.TextButton(child: new Text("Dismiss"), onPressed: () => Navigator.of(dialogContext).pop<object>()), new M.FilledButton(child: new Text("Okay"), onPressed: () => Navigator.of(dialogContext).pop<object>())]))),
                new M.TextButton(child: Bold("Show full-screen dialog"), onPressed: () => M.DialogLibrary.showDialog<object>(ctx, dialogContext => M.Dialog.CreateFullscreen(child:
                    new Padding(padding: EdgeInsets.CreateAll(20), child: new M.Scaffold(appBar: new M.AppBar(title: new Text("Full-screen dialog"), centerTitle: false,
                        leading: new M.IconButton(icon: new Icon(M.Icons.close), onPressed: () => Navigator.of(dialogContext).pop<object>()),
                        actions: [new M.TextButton(child: new Text("Close"), onPressed: () => Navigator.of(dialogContext).pop<object>())]))))))])),
            4 => Section("Dividers", new M.Divider()),
            _ => throw new ArgumentOutOfRangeException(nameof(sectionIndex)),
        };
    }
    private Widget Card(BuildContext context, int style)
    {
        var child = new Padding(padding: EdgeInsets.CreateFromLTRB(10, 5, 5, 10), child: new Column(children:
            [new Align(alignment: Alignment.topRight, child: new M.IconButton(icon: new Icon(M.Icons.more_vert), onPressed: DisplayAction)),
             new SizedBox(height: 20), new Align(alignment: Alignment.bottomLeft, child: new Text(new[] { "Elevated", "Filled", "Outlined" }[style]))]));
        return new SizedBox(width: 115, child: style switch {
            0 => new M.Card(child: child),
            1 => new M.Card(elevation: 0, color: M.Theme.of(context).colorScheme.surfaceContainerHighest, child: child),
            _ => new M.Card(elevation: 0, shape: new RoundedRectangleBorder(side: new BorderSide(color: M.Theme.of(context).colorScheme.outline), borderRadius: BorderRadius.CreateCircular(12)), child: child) });
    }
    private Widget Carousel(BuildContext context, bool snapping) => new SizedBox(height: 150, child: new M.CarouselView(itemSnapping: snapping, itemExtent: 180, shrinkExtent: 100,
        shape: new RoundedRectangleBorder(borderRadius: BorderRadius.CreateCircular(10), side: new BorderSide(color: M.Theme.of(context).colorScheme.outline)), children: Enumerable.Range(0, 20).Select(i => (Widget)new Center(child: new Text($"Item {i}"))).ToList()));
    private async void OpenSheet(Func<BuildContext, Widget> builder, StateSetter setState)
    {
        var controller = widget.Scaffold.currentState!.showBottomSheet(builder, elevation: 8, constraints: new BoxConstraints(maxWidth: 640));
        setState(() => _sheet = controller);
        await controller.closed;
        if (mounted) setState(() => _sheet = null);
    }
    private Widget Navigation(BuildContext ctx, StateSetter setState, int sectionIndex) => sectionIndex switch
    {
        0 => Section("Bottom app bar", new SizedBox(height: 80, child: new M.Scaffold(bottomNavigationBar: new M.BottomAppBar(child: new Row(children:
            [Menu(true), new M.IconButton(icon: new Icon(M.Icons.search), tooltip: "Search", onPressed: DisplayAction), new M.IconButton(icon: new Icon(M.Icons.favorite), tooltip: "Favorite", onPressed: DisplayAction)])),
            floatingActionButtonLocation: M.FloatingActionButtonLocation.endContained,
            floatingActionButton: new M.FloatingActionButton(heroTag: "bottom-bar-fab", elevation: 0, child: new Icon(M.Icons.add), onPressed: DisplayAction)))),
        1 => Section("Navigation bar", new M.NavigationBar(selectedIndex: _barIndex, onDestinationSelected: value => setState(() => _barIndex = value), destinations:
            [new M.NavigationDestination(icon: new Icon(M.Icons.explore_outlined), selectedIcon: new Icon(M.Icons.explore), label: "Explore"),
             new M.NavigationDestination(icon: new Icon(M.Icons.pets_outlined), selectedIcon: new Icon(M.Icons.pets), label: "Pets"),
             new M.NavigationDestination(icon: new Icon(M.Icons.account_box_outlined), selectedIcon: new Icon(M.Icons.account_box), label: "Account")])),
        2 => Section("Navigation drawer", new SizedBox(height: 520, child: new GalleryDrawer()), new SizedBox(height: 20),
            new M.TextButton(child: Bold("Show modal navigation drawer"), onPressed: () => widget.Scaffold.currentState!.openEndDrawer())),
        3 => Section("Navigation rail", new IntrinsicWidth(child: new SizedBox(height: 420, child: new M.NavigationRail(selectedIndex: _railIndex,
            onDestinationSelected: value => setState(() => _railIndex = value), elevation: 4, groupAlignment: 0, labelType: M.NavigationRailLabelType.selected,
            leading: new M.FloatingActionButton(heroTag: "sample-rail-fab", child: new Icon(M.Icons.create), onPressed: DisplayAction),
            destinations: Enumerable.Range(0, 4).Select(i => new M.NavigationRailDestination(icon: new Icon(GalleryDrawer.Icons[i]), selectedIcon: new Icon(GalleryDrawer.SelectedIcons[i]), label: new Text(GalleryDrawer.Labels[i]))).ToList())))),
        4 => Section("Tabs", new M.DefaultTabController(length: 3, child: new SizedBox(height: 80, child: new M.Scaffold(appBar: new M.AppBar(bottom:
            new M.TabBar(tabs: [new M.Tab(text: "Video", icon: new Icon(M.Icons.videocam_outlined), iconMargin: EdgeInsets.zero),
                new M.Tab(text: "Photos", icon: new Icon(M.Icons.photo_outlined), iconMargin: EdgeInsets.zero),
                new M.Tab(text: "Audio", icon: new Icon(M.Icons.audiotrack_sharp), iconMargin: EdgeInsets.zero)])))))),
        5 => SpacedSection("Search", M.SearchAnchor.CreateBar(barHintText: "Search colors", suggestionsBuilder: (_, controller) => Suggestions(controller, setState)),
            new Text(_selectedColor is null ? "Select a color" : $"Last selected color is {_selectedColor}")),
        6 => SpacedSection("Top app bars", new M.AppBar(title: new Text("Center-aligned"), leading: new M.BackButton(), centerTitle: true,
                actions: [new M.IconButton(iconSize: 32, icon: new Icon(M.Icons.account_circle_outlined), onPressed: DisplayAction)]),
            new M.AppBar(title: new Text("Small"), leading: new M.BackButton(), actions: TopBarActions(), centerTitle: false),
            new SizedBox(height: 100, child: new CustomScrollView(slivers: [M.SliverAppBar.CreateMedium(title: new Text("Medium"), leading: new M.BackButton(), actions: TopBarActions()), new SliverFillRemaining()])),
            new SizedBox(height: 130, child: new CustomScrollView(slivers: [M.SliverAppBar.CreateLarge(title: new Text("Large"), leading: new M.BackButton(), actions: TopBarActions()), new SliverFillRemaining()]))),
        _ => throw new ArgumentOutOfRangeException(nameof(sectionIndex)),
    };
    private static List<Widget> TopBarActions() => new[] { M.Icons.attach_file, M.Icons.@event, M.Icons.more_vert }
        .Select(icon => (Widget)new M.IconButton(icon: new Icon(icon), onPressed: DisplayAction)).ToList();
    private List<Widget> Suggestions(M.SearchController controller, StateSetter setState)
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

// Match the reference's progress-only State. A stable key preserves the local
// state when the responsive list changes its column configuration.
internal sealed class ProgressIndicators(Key? key = null) : StatefulWidget(key: key)
{
    public override IState createState() => new ProgressIndicatorsState();
}
internal sealed class ProgressIndicatorsState : State<ProgressIndicators>
{
    private bool _progress;
    public override Widget build(BuildContext context) => new ComponentSection("Progress indicators",
        new Column(mainAxisSize: MainAxisSize.min, spacing: 10, children: [new Row(children:
            [new M.IconButton(tooltip: _progress ? "Stop progress" : "Start progress", isSelected: _progress,
                selectedIcon: new Icon(M.Icons.pause), icon: new Icon(M.Icons.play_arrow),
                onPressed: () => setState(() => _progress = !_progress)),
             new SizedBox(width: 20), new M.CircularProgressIndicator(value: _progress ? null : 0.7), new SizedBox(width: 20),
             new Expanded(child: new M.LinearProgressIndicator(value: _progress ? null : 0.7)), new SizedBox(width: 20)])]));
}
