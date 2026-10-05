using Doroti.Ui;

namespace Doroti.Host.Maui;

internal static class AppleSemanticsState
{
    public static int? NumericValue(SemanticsNodeUpdate? node) => node?.flags?.isChecked switch
    {
        CheckedState.isTrue => 1,
        CheckedState.isFalse => 0,
        CheckedState.mixed => 2,
        _ => node?.flags?.isToggled switch { Tristate.isTrue => 1, Tristate.isFalse => 0, _ => null },
    };

    // The host's Localizable.strings can translate these fallback keys. Checked
    // state is not mapped to Selected, which has a different accessibility meaning.
    public static string? TextValue(SemanticsNodeUpdate? node, Func<string, string> localize)
    {
        if (node?.flags?.isObscured == true) return null;
        var value = NumericValue(node);
        var state = value is null ? null : localize(value == 2 ? "Mixed" : value == 1 ? "On" : "Off");
        return state is null ? node?.value : string.IsNullOrWhiteSpace(node?.value) ? state : node.value + ", " + state;
    }
}
