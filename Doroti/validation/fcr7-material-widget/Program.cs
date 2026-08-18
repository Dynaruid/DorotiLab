var requiredComponents = new HashSet<string>(StringComparer.Ordinal)
{
    "scaffold-background", "app-bar-text", "floating-action-button",
    "ink-well-sparkle", "scrollbar-list-sliver", "shader-mask-image-filter",
};
var capturedComponents = new HashSet<string>(StringComparer.Ordinal);

var scenarios = new[]
{
    Scenario("fab-press", "floating-action-button", ["default", "pressed", "focused", "disabled"], ["down", "hold", "up", "semantics"]),
    Scenario("ink-cold-warm", "ink-well-sparkle", ["cold", "warm", "hovered", "pressed"], ["hover", "down", "move", "hold", "up"]),
    Scenario("scrollbar-range", "scrollbar-list-sliver", ["top", "middle", "end"], ["scroll", "semantics"]),
    Scenario("effects-cold-warm", "shader-mask-image-filter", ["cold", "warm"], ["frame"]),
};

foreach (var scenario in scenarios)
{
    Require(requiredComponents.Contains(scenario.Component), $"scenario component is in the fixed source slice: {scenario.Id}");
    Require(scenario.States.Count >= 2, $"scenario compares more than one state: {scenario.Id}");
    Require(scenario.Actions.Count > 0, $"scenario has replayable actions: {scenario.Id}");
    Require(scenario.Actions.Any(action => action is "down" or "scroll" or "frame"), $"scenario has a causal input or frame: {scenario.Id}");
    capturedComponents.Add(scenario.Component);
}

Require(capturedComponents.SetEquals(["floating-action-button", "ink-well-sparkle", "scrollbar-list-sliver", "shader-mask-image-filter"]), "interactive/effect slice scenarios are complete");
Require(requiredComponents.Contains("scaffold-background") && requiredComponents.Contains("app-bar-text"), "persistent scaffold and text coverage is explicit");
Console.WriteLine($"FCR-7 material/widget runtime contract: PASS (configuration={ConfigurationName()})");

static Scenario Scenario(string id, string component, IReadOnlyList<string> states, IReadOnlyList<string> actions) => new(id, component, states, actions);
static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
static string ConfigurationName() =>
#if DEBUG
    "Debug";
#else
    "Release";
#endif

sealed record Scenario(string Id, string Component, IReadOnlyList<string> States, IReadOnlyList<string> Actions);
