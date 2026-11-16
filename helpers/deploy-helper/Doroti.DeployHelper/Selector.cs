namespace Doroti.DeployHelper;

public sealed class Selector(IUserInterface ui)
{
    public async Task<T> SelectAsync<T>(string title, IReadOnlyList<T> items, Func<T, string> label, string option,
        CancellationToken ct, string? requested = null, Func<T, string, bool>? matches = null, int? defaultIndex = null)
    {
        if (requested is not null)
        {
            var selected = items.Where(item => matches!(item, requested)).ToArray();
            if (selected.Length != 1)
                throw new DeployException($"{title}: '{requested}' matched {selected.Length} entries. Use {option} with an exact ID.");
            ui.WriteLine($"{title}: {label(selected[0])}");
            return selected[0];
        }
        if (items.Count == 0) throw new DeployException($"No entries for {title}.");
        if (defaultIndex is { } index && (index < 0 || index >= items.Count))
            throw new DeployException($"Invalid default selection for {title}.");
        ui.WriteLine("\n" + title);
        for (var itemIndex = 0; itemIndex < items.Count; itemIndex++)
            ui.WriteLine($"  {itemIndex + 1}. {label(items[itemIndex])}" + (defaultIndex == itemIndex ? " (default)" : ""));
        if (items.Count == 1) { ui.WriteLine("Using the only entry."); return items[0]; }
        if (!ui.IsInteractive) throw new DeployException($"Selection needs an interactive terminal. Specify {option}.");
        while (true)
        {
            ui.WriteLine(defaultIndex is { } defaultItem
                ? $"Select 1-{items.Count} (Enter for {defaultItem + 1}, q to cancel):"
                : $"Select 1-{items.Count} (q to cancel):");
            var value = (await ui.ReadLineAsync(ct))?.Trim();
            if (value is null || value.Equals("q", StringComparison.OrdinalIgnoreCase)) throw new OperationCanceledException();
            if (value.Length == 0 && defaultIndex is { } chosenDefault) return items[chosenDefault];
            if (int.TryParse(value, out var number) && number >= 1 && number <= items.Count) return items[number - 1];
            ui.WriteLine("Enter a number from the list.");
        }
    }
}
