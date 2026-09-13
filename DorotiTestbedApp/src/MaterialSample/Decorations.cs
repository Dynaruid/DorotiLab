// Copyright 2021 The Flutter team. All rights reserved.
// Adapted from component_screen.dart; BSD license in LICENSE.flutter.
using Doroti.Framework.Gestures;
using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Ui;
using M = Doroti.Framework.Material;

namespace MaterialSample;

internal sealed class ComponentSection(string label, Widget child) : StatefulWidget
{
    internal string Label => label;
    internal Widget Child => child;
    internal string Tooltip => Label switch
    {
        "Common buttons" => "Use ElevatedButton, FilledButton, FilledButton.tonal, OutlinedButton, or TextButton",
        "Floating action buttons" => "Use FloatingActionButton or FloatingActionButton.extended",
        "Cards" => "Use Card",
        "Text fields" => "Use TextField with different InputDecoration",
        "Dialog" => "Use showDialog with Dialog.fullscreen, AlertDialog, or SimpleDialog",
        "Dividers" => "Use Divider or VerticalDivider",
        "Switches" => "Use SwitchListTile or Switch",
        "Checkboxes" => "Use CheckboxListTile or Checkbox",
        "Radio buttons" => "Use RadioListTile<T> or Radio<T>",
        "Progress indicators" => "Use CircularProgressIndicator or LinearProgressIndicator",
        "Badges" => "Use Badge or Badge.count",
        "Navigation bar" => "Use NavigationBar",
        "Icon buttons" => "Use IconButton, IconButton.filled, IconButton.filledTonal, and IconButton.outlined",
        "Chips" => "Use ActionChip, FilterChip, or InputChip. \nActionChip can also be used for suggestion chip",
        "Date picker" => "Use showDatePicker",
        "Time picker" => "Use showTimePicker",
        "Segmented buttons" => "Use SegmentedButton<T>",
        "Snackbar" => "Use ScaffoldMessenger.of(context).showSnackBar with SnackBar",
        "Bottom sheet" => "Use showModalBottomSheet<T> or showBottomSheet<T>",
        "Bottom app bar" => "Use BottomAppBar",
        "Navigation drawer" => "Use NavigationDrawer. For modal navigation drawers, see Scaffold.endDrawer",
        "Navigation rail" => "Use NavigationRail",
        "Tabs" => "Use TabBar",
        "Top app bars" => "Use AppBar, SliverAppBar, SliverAppBar.medium, or  SliverAppBar.large",
        "Menus" => "Use MenuAnchor or DropdownMenu<T>",
        "Sliders" => "Use Slider or RangeSlider",
        "Search" => "Use SearchAnchor or SearchAnchor.bar",
        "Carousel" => "Use CarouselView",
        _ => "",
    };
    public override IState createState() => new ComponentSectionState();
}
internal sealed class ComponentSectionState : State<ComponentSection>
{
    private readonly FocusNode _focus = new();
    public override void dispose() { _focus.dispose(); base.dispose(); }
    public override Widget build(BuildContext context) => new RepaintBoundary(child: new Padding(
        padding: EdgeInsets.CreateSymmetric(vertical: 10), child: new Column(children:
        [
            new Row(mainAxisAlignment: MainAxisAlignment.center, children:
            [new Text(widget.Label, style: M.Theme.of(context).textTheme.titleMedium),
             new M.Tooltip(message: widget.Tooltip, child: new Padding(padding: EdgeInsets.CreateSymmetric(horizontal: 5), child: new Icon(M.Icons.info_outline, size: 16)))]),
            new ConstrainedBox(constraints: BoxConstraints.CreateTightFor(width: 450), child: new Focus(focusNode: _focus,
                // Wait until the card wins the tap. onTapDown also fires while
                // a child is recognizing a long press and steals its focus.
                child: new GestureDetector(onTap: () => _focus.requestFocus(), behavior: HitTestBehavior.opaque,
                    child: new M.Card(elevation: 0, shape: new RoundedRectangleBorder(borderRadius: BorderRadius.CreateCircular(12),
                        side: new BorderSide(color: M.Theme.of(context).colorScheme.outlineVariant)),
                        child: new Padding(padding: EdgeInsets.CreateSymmetric(horizontal: 5, vertical: 20), child: new Center(child: widget.Child))))))
        ])));
}

internal sealed class GalleryDrawer : StatefulWidget
{
    internal static readonly string[] Labels = ["Inbox", "Outbox", "Favorites", "Trash", "Family", "School", "Work"];
    internal static readonly IconData[] Icons = [M.Icons.inbox_outlined, M.Icons.send_outlined, M.Icons.favorite_outline, M.Icons.delete_outline, M.Icons.bookmark_border, M.Icons.bookmark_border, M.Icons.bookmark_border];
    internal static readonly IconData[] SelectedIcons = [M.Icons.inbox, M.Icons.send, M.Icons.favorite, M.Icons.delete, M.Icons.bookmark, M.Icons.bookmark, M.Icons.bookmark];
    public override IState createState() => new GalleryDrawerState();
}
internal sealed class GalleryDrawerState : State<GalleryDrawer>
{
    private long _selected;
    public override Widget build(BuildContext context)
    {
        Widget Heading(string text) => new Padding(padding: EdgeInsets.CreateFromLTRB(28, 16, 16, 10), child: new Text(text, style: M.Theme.of(context).textTheme.titleSmall));
        Widget Destination(int i) => new M.NavigationDrawerDestination(label: new Text(GalleryDrawer.Labels[i]), icon: new Icon(GalleryDrawer.Icons[i]), selectedIcon: new Icon(GalleryDrawer.SelectedIcons[i]));
        return new M.NavigationDrawer(selectedIndex: _selected, onDestinationSelected: value => setState(() => _selected = value), children:
            [Heading("Mail"), .. Enumerable.Range(0, 4).Select(Destination), new M.Divider(indent: 28, endIndent: 28), Heading("Labels"), .. Enumerable.Range(4, 3).Select(Destination)]);
    }
}
