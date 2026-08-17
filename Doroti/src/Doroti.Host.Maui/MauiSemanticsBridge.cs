using System.Text.Json;
using Doroti.Ui;
using Microsoft.Maui.Controls;

namespace Doroti.Host.Maui;

internal sealed class MauiSemanticsBridge(AbsoluteLayout layer) : IMauiSemanticsBridge
{
    private readonly AbsoluteLayout _layer = layer;

    public void Update(string serializedTree, Action<int, SemanticsAction, object?> performAction)
    {
        ArgumentNullException.ThrowIfNull(performAction);
        var update = JsonSerializer.Deserialize<NativeSemanticsUpdate>(serializedTree, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });
        _layer.Dispatcher.Dispatch(() => Apply(update, performAction));
    }

    private void Apply(NativeSemanticsUpdate? update, Action<int, SemanticsAction, object?> performAction)
    {
        _layer.Children.Clear();
        foreach (var node in update?.Nodes ?? [])
        {
            if (node.Flags?.Hidden == true) continue;
            var actions = (SemanticsAction)node.Actions;
            View element;
            if (node.Flags?.TextField == true)
            {
                var entry = new Entry
                {
                    Text = node.Value ?? string.Empty,
                    IsReadOnly = node.Flags.ReadOnly,
                    Opacity = 0.01,
                };
                var initializing = true;
                entry.TextChanged += (_, args) =>
                {
                    if (!initializing && actions.HasFlag(SemanticsAction.setText))
                        performAction(node.Id, SemanticsAction.setText, args.NewTextValue ?? string.Empty);
                };
                entry.PropertyChanged += (_, args) =>
                {
                    if (!initializing && actions.HasFlag(SemanticsAction.setSelection) &&
                        args.PropertyName is nameof(InputView.CursorPosition) or nameof(InputView.SelectionLength))
                    {
                        performAction(node.Id, SemanticsAction.setSelection, new Dictionary<string, long>
                        {
                            ["base"] = entry.CursorPosition,
                            ["extent"] = entry.CursorPosition + entry.SelectionLength,
                        });
                    }
                };
                initializing = false;
                element = entry;
            }
            else if (actions.HasFlag(SemanticsAction.tap))
            {
                var button = new Button { Text = node.Label ?? node.Value ?? string.Empty, Opacity = 0.01 };
                button.Clicked += (_, _) => performAction(node.Id, SemanticsAction.tap, null);
                element = button;
            }
            else
            {
                element = new Label { Text = node.Label ?? node.Value ?? string.Empty, Opacity = 0.01, InputTransparent = true };
            }

            SemanticProperties.SetDescription(element, string.Join(" ", new[] { node.Label, node.Value }.Where(value => !string.IsNullOrWhiteSpace(value))));
            if (node.Flags?.Header == true) SemanticProperties.SetHeadingLevel(element, SemanticHeadingLevel.Level1);
            if (actions.HasFlag(SemanticsAction.focus))
                element.Focused += (_, _) => performAction(node.Id, SemanticsAction.focus, null);
            var rect = node.Rect is { Length: 4 } ? node.Rect : [0, 0, 0, 0];
            AbsoluteLayout.SetLayoutBounds(element, new Microsoft.Maui.Graphics.Rect(
                rect[0], rect[1], Math.Max(0, rect[2] - rect[0]), Math.Max(0, rect[3] - rect[1])));
            AbsoluteLayout.SetLayoutFlags(element, Microsoft.Maui.Layouts.AbsoluteLayoutFlags.None);
            _layer.Children.Add(element);
        }
        _layer.SetValue(SemanticProperties.DescriptionProperty, $"Doroti semantics generation {update?.Generation ?? 0}");
    }

    private sealed record NativeSemanticsUpdate(long Generation, NativeSemanticsNode[]? Nodes);
    private sealed record NativeSemanticsNode(
        int Id, string? Label, string? Value, string? Role, long Actions,
        int[]? Children, NativeSemanticsFlags? Flags, double[]? Rect);
    private sealed record NativeSemanticsFlags(bool TextField, bool ReadOnly, bool Header, bool Hidden);
}
