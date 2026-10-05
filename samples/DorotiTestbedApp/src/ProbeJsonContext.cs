using System.Text.Json.Serialization;

internal sealed record InputLifetimeProbeResult(int Created, bool EditorRecreated,
    bool WebViewRecreated, bool MixedScene, string PhysicalIme);

internal sealed record WebViewSceneProbeResult(string Status, bool WidgetScene,
    [property: JsonPropertyName("Json")] string? Json,
    [property: JsonPropertyName("DocumentGeneration")] long DocumentGeneration);

internal sealed record NativeDesignProbeResult(string Schema, string Result, string Design,
    string[] NativeViews, string OwnerView, bool CapturedTheme, bool CapturedLocalization,
    bool NativeVisibleBeforeResult, int NativeResult, int OverlayResult, int? WindowCloseResult,
    bool TooltipVisible, bool TooltipCancellation, bool TooltipEarlyCancellation,
    bool TooltipConcurrentDispose, int RemainingWindows);

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(InputLifetimeProbeResult))]
[JsonSerializable(typeof(WebViewSceneProbeResult))]
[JsonSerializable(typeof(NativeDesignProbeResult))]
internal sealed partial class TestbedProbeJsonContext : JsonSerializerContext;
