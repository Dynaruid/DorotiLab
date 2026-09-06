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
             new M.Tooltip(message: "Use " + widget.Label, child: new Padding(padding: EdgeInsets.CreateSymmetric(horizontal: 5), child: new Icon(M.Icons.info_outline, size: 16)))]),
            new ConstrainedBox(constraints: BoxConstraints.CreateTightFor(width: 450), child: new Focus(focusNode: _focus,
                child: new GestureDetector(onTapDown: _ => _focus.requestFocus(), behavior: HitTestBehavior.opaque,
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
