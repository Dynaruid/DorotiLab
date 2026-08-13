using Doroti.Graphics;

namespace Doroti.Platform;

public enum SemanticsRole
{
    Generic,
    Button,
    Text,
    TextField,
    Image,
    List,
    ListItem,
    Dialog,
    CheckBox,
    Slider,
}

[Flags]
public enum SemanticsState
{
    None = 0,
    Enabled = 1,
    Focused = 2,
    Selected = 4,
    ReadOnly = 8,
    Multiline = 16,
    Hidden = 32,
    Checked = 64,
    Mixed = 128,
    Toggled = 256,
}

[Flags]
public enum SemanticsAction
{
    None = 0,
    Tap = 1,
    Focus = 2,
    SetText = 4,
    ScrollUp = 8,
    ScrollDown = 16,
    Dismiss = 32,
    Toggle = 64,
    SetSelection = 128,
}

public readonly record struct SemanticsActionRequest(int NodeId, SemanticsAction Action, object? Arguments = null);

public sealed record SemanticsNodeSnapshot(
    int Id,
    SemanticsRole Role,
    string? Label,
    string? Value,
    SemanticsState State,
    SemanticsAction Actions,
    Rect Bounds,
    IReadOnlyList<SemanticsNodeSnapshot> Children,
    int? IndexInParent = null);

public sealed record SemanticsTreeSnapshot(long Generation, SemanticsNodeSnapshot Root);

public interface IAccessibilityBridge
{
    void Update(SemanticsTreeSnapshot snapshot, Func<SemanticsActionRequest, bool> performAction);
}

public interface IAccessibilityDiagnostics
{
    SemanticsTreeSnapshot? LastSnapshot { get; }

    bool InvokeAction(int nodeId, SemanticsAction action, object? arguments = null);
}
