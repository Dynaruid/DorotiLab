using Doroti.Host.Maui;
using Doroti.Ui;
using Microsoft.Maui.Controls;
using Rect = Doroti.Ui.Rect;

internal static class SemanticsProjectionContracts
{
    internal static void Verify()
    {
        var layer = new AbsoluteLayout();
        using var bridge = new MauiSemanticsBridge(layer);
        var calls = new List<(int Id, SemanticsAction Action, object? Arguments)>();
        var generation = 0;
        var flags = new SemanticsFlags(isEnabled: Tristate.isTrue);
        var nodes = new List<SemanticsNodeUpdate>
        {
            new(1, Rect.fromLTWH(0, 0, 200, 40), "Text", "initial", SemanticsAction.setText | SemanticsAction.setSelection | SemanticsAction.focus,
                [], flags with { isTextField = true }, textSelectionBase: 1, textSelectionExtent: 4),
            new(2, Rect.fromLTWH(0, 40, 200, 40), "Check", null, SemanticsAction.tap, [], flags with { isChecked = CheckedState.isFalse }),
            new(3, Rect.fromLTWH(0, 80, 200, 40), "Switch", null, SemanticsAction.tap, [], flags with { isToggled = Tristate.isFalse }),
            new(4, Rect.fromLTWH(0, 120, 200, 40), "Slider", "0.25", SemanticsAction.increase | SemanticsAction.decrease,
                [], flags with { isSlider = true }, minValue: "0", maxValue: "1"),
            new(5, Rect.fromLTWH(0, 160, 200, 40), "Button", null, SemanticsAction.tap, [], flags),
            new(6, Rect.fromLTWH(0, 200, 200, 40), "Highlighted radio", null, SemanticsAction.tap, [],
                flags with { isInMutuallyExclusiveGroup = true, isChecked = CheckedState.isFalse, isSelected = Tristate.isTrue }),
        };
        void Require(bool condition, string message) { if (!condition) throw new Exception(message); }
        void Project()
        {
            bridge.Update(new(++generation, nodes, SemanticsUpdateUrgency.immediate), (id, action, args) => calls.Add((id, action, args)));
            Require(calls.Count == 0, $"Semantics projection echoed actions: {string.Join(',', calls.Select(c => (c.Id, c.Action)))}");
        }
        Project();
        var entry = (Entry)layer.Children[0];
        var check = (CheckBox)layer.Children[1];
        var toggle = (Switch)layer.Children[2];
        var slider = (Slider)layer.Children[3];
        var button = (Button)layer.Children[4];
        var radio = (RadioButton)layer.Children[5];
        Require(!radio.IsChecked, "Tile selection overwrote explicit radio checked state");
        Require(entry.CursorPosition == 1 && entry.SelectionLength == 3, "Text selection was not projected");

        // Native handlers may notify a different element during projection (the
        // radio incident did exactly that). Exercise the production event path.
        EventHandler<TextChangedEventArgs> crossElement = (_, _) => ((IButtonController)button).SendClicked();
        entry.TextChanged += crossElement;
        nodes[0] = nodes[0] with { value = "projected", textSelectionBase = 4, textSelectionExtent = 2 };
        nodes[1] = nodes[1] with { flags = flags with { isChecked = CheckedState.isTrue } };
        nodes[2] = nodes[2] with { flags = flags with { isToggled = Tristate.isTrue } };
        nodes[3] = nodes[3] with { value = "20", minValue = "10", maxValue = "30" };
        Project();
        entry.TextChanged -= crossElement;
        Require(entry.CursorPosition == 2 && entry.SelectionLength == 2 && check.IsChecked && toggle.IsToggled && slider.Value == 20,
            "Projection did not update native values/bounds/selection");

        // Genuine changes and activation remain available to assistive tools.
        ((IButtonController)button).SendClicked();
        check.IsChecked = false;
        toggle.IsToggled = false;
        slider.Value = 21;
        entry.Text = "edited";
        Require(calls.Count(c => c.Id == 5 && c.Action == SemanticsAction.tap) == 1 &&
            calls.Any(c => c.Id == 2 && c.Action == SemanticsAction.tap) && calls.Any(c => c.Id == 3 && c.Action == SemanticsAction.tap) &&
            calls.Any(c => c.Id == 4 && c.Action == SemanticsAction.increase) && calls.Any(c => c.Id == 1 && c.Action == SemanticsAction.setText),
            "Genuine assistive actions were blocked");
        calls.Clear();

        // A rapid second input may return to the projected value before the
        // framework has published the result of the first input.
        check.IsChecked = true;
        toggle.IsToggled = true;
        slider.Value = 20;
        entry.Text = "projected";
        Require(calls.Count(c => c.Id == 2 && c.Action == SemanticsAction.tap) == 1 &&
            calls.Count(c => c.Id == 3 && c.Action == SemanticsAction.tap) == 1 &&
            calls.Count(c => c.Id == 4 && c.Action == SemanticsAction.decrease) == 1 &&
            calls.Any(c => c.Id == 1 && c.Action == SemanticsAction.setText && Equals(c.Arguments, "projected")),
            "Rapid input returning to the projected value was lost");
        calls.Clear();
        check.IsChecked = true; toggle.IsToggled = true; slider.Value = 20; entry.Text = "projected";
        Project();
        Require(calls.Count == 0, "Unchanged native values echoed as user actions");

        nodes = nodes.Select(node => node with { flags = node.flags! with { isEnabled = Tristate.isFalse } }).ToList();
        Project();
        ((IButtonController)button).SendClicked(); check.IsChecked = false; toggle.IsToggled = false; slider.Value = 22; entry.Text = "disabled";
        Require(calls.Count == 0, "Disabled semantics accepted native changes");
        nodes[0] = nodes[0] with { flags = nodes[0].flags! with { isEnabled = Tristate.isTrue, isReadOnly = true } };
        nodes[3] = nodes[3] with { flags = nodes[3].flags! with { isEnabled = Tristate.isTrue, isReadOnly = true } };
        Project();
        entry.Text = "readonly"; slider.Value = 23;
        Require(calls.All(c => c.Action == SemanticsAction.setSelection), "Read-only semantics accepted an edit");
        calls.Clear();
        nodes = [];
        Project();
        ((IButtonController)button).SendClicked(); entry.Text = "removed"; slider.Value = 24;
        Require(calls.Count == 0, "Removed controls retained action bindings");
        bridge.Clear();
        bridge.Dispose();
        ((IButtonController)button).SendClicked();
        Require(calls.Count == 0, "Disposed bridge forwarded actions");
        Console.WriteLine("MAUI semantics projection: whole-tree suppression, text/selection/toggle/slider updates, no-op echoes, live actions, disabled/read-only/removed/disposed guards PASS");
    }
}
