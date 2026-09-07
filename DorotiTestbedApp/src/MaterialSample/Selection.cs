// Copyright 2021 The Flutter team. All rights reserved.
// Adapted from reference/flutter_sample_app; BSD license in LICENSE.flutter.
using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Services;
using Doroti.Framework.Widgets;
using Doroti.Ui;
using M = Doroti.Framework.Material;
using TextStyle = Doroti.Framework.Painting.TextStyle;

namespace MaterialSample;

internal sealed partial class ComponentsState
{
    private IReadOnlyList<Func<Widget>> Selection(BuildContext ctx) =>
    [
        () => Section("Checkboxes",
            new M.CheckboxListTile(title: new Text("Option 1"), tristate: true, value: _checkA, onChanged: value => setState(() => _checkA = value)),
            new M.CheckboxListTile(title: new Text("Option 2"), tristate: true, value: _checkB, onChanged: value => setState(() => _checkB = value)),
            new M.CheckboxListTile(title: new Text("Option 3"), tristate: true, value: _checkC, onChanged: value => setState(() => _checkC = value)),
            new M.CheckboxListTile(title: new Text("Option 4"), tristate: true, value: true, onChanged: null)),
        () => Section("Chips", Flow(new M.ActionChip(avatar: new Icon(M.Icons.@event), label: new Text("Assist"), onPressed: DisplayAction),
            new M.FilterChip(label: new Text("Filter"), selected: _filtered, onSelected: value => setState(() => _filtered = value)),
            new M.InputChip(label: new Text("Input"), onPressed: DisplayAction, onDeleted: DisplayAction), new M.ActionChip(label: new Text("Suggestion"), onPressed: DisplayAction)),
            Flow(new M.ActionChip(avatar: new Icon(M.Icons.@event), label: new Text("Assist")), new M.FilterChip(label: new Text("Filter"), selected: _filtered, onSelected: null),
                new M.InputChip(label: new Text("Input"), onDeleted: DisplayAction, isEnabled: false), new M.ActionChip(label: new Text("Suggestion")))),
        () => Section("Date picker", M.TextButton.CreateIcon(icon: new Icon(M.Icons.calendar_month), label: new Text("Show date picker", style: new TextStyle(fontWeight: FontWeight.bold)), onPressed: () => PickDate(ctx))),
        () => Section("Time picker", M.TextButton.CreateIcon(icon: new Icon(M.Icons.schedule), label: new Text("Show time picker", style: new TextStyle(fontWeight: FontWeight.bold)), onPressed: () => PickTime(ctx))),
        () => Section("Menus", new Row(mainAxisAlignment: MainAxisAlignment.center, spacing: 10, children: [Menu(false), Menu(true)]),
            Flow(new M.DropdownMenu<string>(controller: _colorMenu, initialSelection: _menuColor, enableFilter: true, label: new Text("Color"), inputDecorationTheme: new M.InputDecorationTheme(filled: true),
                dropdownMenuEntries: new[] { "Blue", "Pink", "Green", "Yellow", "Grey" }.Select(label => new M.DropdownMenuEntry<string>(value: label, label: label, enabled: label != "Grey")).ToList(), onSelected: value => { if (value is not null) setState(() => _menuColor = value); }),
            new M.DropdownMenu<string>(controller: _iconMenu, initialSelection: _menuIcon, leadingIcon: new Icon(M.Icons.search), label: new Text("Icon"),
                dropdownMenuEntries: new[] { "Smile", "Cloud", "Brush", "Heart" }.Select(label => new M.DropdownMenuEntry<string>(value: label, label: label)).ToList(), onSelected: value => { if (value is not null) setState(() => _menuIcon = value); }),
            new Icon(MenuIcon(_menuIcon), color: _menuColor switch { "Blue" => M.Colors.blue, "Pink" => M.Colors.pink, "Green" => M.Colors.green, "Yellow" => M.Colors.yellow, _ => M.Colors.grey.withAlpha(128) }))),
        () => Section("Radio buttons", new RadioGroup<string>(groupValue: _radio, onChanged: value => setState(() => _radio = value!), child: new Column(children:
            [new M.RadioListTile<string>(value: "first", title: new Text("Option 1")), new M.RadioListTile<string>(value: "second", title: new Text("Option 2")), new M.RadioListTile<string>(value: "disabled", title: new Text("Option 3"), enabled: false)]))),
        () => Section("Sliders", new M.Slider(value: _sliderA, max: 100, label: $"{_sliderA:0}", onChanged: value => setState(() => _sliderA = value)),
            new SizedBox(height: 10), new M.Slider(value: _sliderB, max: 100, divisions: 5, label: $"{_sliderB:0}", onChanged: value => setState(() => _sliderB = value))),
        () => Section("Switches", new Row(mainAxisAlignment: MainAxisAlignment.spaceEvenly, children:
            [new M.Switch(value: _switchA, onChanged: value => setState(() => _switchA = value)), new M.Switch(value: _switchB, onChanged: value => setState(() => _switchB = value), thumbIcon: SwitchIcon())]),
            new Row(mainAxisAlignment: MainAxisAlignment.spaceEvenly, children: [new M.Switch(value: false, onChanged: null), new M.Switch(value: true, onChanged: null, thumbIcon: SwitchIcon())])),
    ];
    private static WidgetStateProperty<Icon?> SwitchIcon() => WidgetStateProperty<Icon?>.resolveWith<Icon?>(states => new Icon(states.Contains(WidgetState.selected) ? M.Icons.check : M.Icons.close));
    private static IconData MenuIcon(string label) => label switch { "Smile" => M.Icons.sentiment_satisfied_outlined, "Cloud" => M.Icons.cloud_outlined, "Brush" => M.Icons.brush_outlined, _ => M.Icons.favorite };
    private Widget Menu(bool icon) => new M.MenuAnchor(menuChildren: icon
        ? [new M.MenuItemButton(child: new Text("Menu 1"), onPressed: DisplayAction), new M.MenuItemButton(child: new Text("Menu 2"), onPressed: DisplayAction),
           new M.SubmenuButton(child: new Text("Menu 3"), menuChildren: new[] { "Menu 3.1", "Menu 3.2", "Menu 3.3" }.Select(label => (Widget)new M.MenuItemButton(child: new Text(label), onPressed: DisplayAction)).ToList())]
        : [new M.MenuItemButton(leadingIcon: new Icon(M.Icons.people_alt_outlined), child: new Text("Item 1"), onPressed: DisplayAction),
           new M.MenuItemButton(leadingIcon: new Icon(M.Icons.remove_red_eye_outlined), child: new Text("Item 2"), onPressed: DisplayAction),
           new M.MenuItemButton(leadingIcon: new Icon(M.Icons.refresh), child: new Text("Item 3"), onPressed: DisplayAction)],
        builder: (_, controller, _) => icon ? new M.IconButton(icon: new Icon(M.Icons.more_vert), tooltip: "Open menu", onPressed: () => { if (controller.isOpen) controller.close(); else controller.open(); })
            : M.FilledButton.CreateTonal(child: new Text("Show menu"), onPressed: () => { if (controller.isOpen) controller.close(); else controller.open(); }));    private async void PickDate(BuildContext ctx)
    {
        var now = DateTime.Now;
        var result = await M.Date_pickerLibrary.showDatePicker(ctx, initialDate: _date ?? now, firstDate: new DateTime(now.Year - 2, 1, 1), lastDate: new DateTime(now.Year + 1, 1, 1));
        if (!mounted) return;
        setState(() => _date = result);
        if (result is { } date) M.ScaffoldMessenger.of(ctx).showSnackBar(new M.SnackBar(content: new Text($"Selected Date: {date.Day}/{date.Month}/{date.Year}")));
    }
    private async void PickTime(BuildContext ctx)
    {
        var result = await M.Time_pickerLibrary.showTimePicker(ctx, initialTime: _time ?? M.TimeOfDay.CreateNow(),
            builder: (dialogContext, child) => new MediaQuery(data: MediaQuery.of(dialogContext).copyWith(alwaysUse24HourFormat: true), child: child!));
        if (!mounted) return;
        setState(() => _time = result);
        if (result is { } time) M.ScaffoldMessenger.of(ctx).showSnackBar(new M.SnackBar(content: new Text($"Selected time: {time.format(ctx)}")));
    }
    private Widget TextInputs()
    {
        Widget Field(bool outlined, int state)
        {
            var controller = outlined ? _outlined : _filled;
            return new M.TextField(controller: controller, enabled: state != 2, maxLength: !outlined && state == 1 ? 10 : null,
                maxLengthEnforcement: MaxLengthEnforcement.none,
                decoration: new M.InputDecoration(labelText: state == 2 ? "Disabled" : outlined ? "Outlined" : "Filled", hintText: "hint text", helperText: "supporting text",
                    errorText: state == 1 ? "error text" : null, filled: !outlined || state != 0,
                    border: outlined ? new M.OutlineInputBorder() : null, prefixIcon: new Icon(M.Icons.search),
                    suffixIcon: new M.IconButton(tooltip: "Clear text", icon: new Icon(M.Icons.close), onPressed: state == 2 ? null : () => controller.clear())));
        }
        Widget Pair(bool outlined) => new Row(spacing: 10, children: [new Expanded(child: Field(outlined, 1)), new Expanded(child: Field(outlined, 2))]);
        return Section("Text fields", Field(false, 0), Pair(false), Field(true, 0), Pair(true));
    }
}

