using Doroti.Ui;

internal static class PlatformPolicyRegression
{
    internal static void Run()
    {
        static void Require(bool value, string message) { if (!value) throw new Exception(message); }
        foreach (var filters in new string[]?[] { null, [], ["*"], [".txt", "*"], ["invalid/filter", "*"] })
            Require(FilePickFilters.Normalize(filters).Length == 0, "All-file filters must agree.");
        var normalized = FilePickFilters.Normalize(["TXT", ".txt", ".CSV"]);
        Require(normalized.SequenceEqual(new[] { ".csv", ".txt" }), "Extension normalization/deduplication failed.");
        Require(FilePickFilters.Matches("한글.TXT", normalized) && !FilePickFilters.Matches("text.exe", normalized), "Suffix contract failed.");
        foreach (var bad in new[] { "", ".", "*.txt", "../txt", "txt,pdf", " txt" })
        {
            try { FilePickFilters.Normalize([bad]); throw new Exception("Invalid filter admitted: " + bad); }
            catch (ArgumentException) { }
        }
        Require(UrlLaunchPolicy.Validate("mailto:user@example.com", out _) is null, "mailto policy failed.");
        Require(UrlLaunchPolicy.Validate("relative/path", out _)?.Status == UrlLaunchStatus.invalidUrl, "Invalid URL status lost.");
        Require(UrlLaunchPolicy.Validate("file:///private", out _)?.Status == UrlLaunchStatus.unsupported, "External file scheme admitted.");
        foreach (var status in Enum.GetValues<UrlLaunchStatus>())
            Require(UrlLaunchPolicy.FromStatus(status.ToString()).Status == status, "URL status collapsed.");
        Require(UrlLaunchPolicy.FromStatus("future-result").Status == UrlLaunchStatus.failed, "Unknown status must fail closed.");
        var bounds = Rect.fromLTWH(-10, -5, 100, 50);
        var dpr = EffectAllocationPreflight.Evaluate(bounds, PlatformViewTransform.Identity, 2, 2, null, 4, 2, 160000);
        Require(dpr.Supported && dpr.RequiredBytes == 160000, "DPR/negative-origin bounds allocation failed.");
        var clipped = EffectAllocationPreflight.Evaluate(bounds, PlatformViewTransform.Identity, 2, 2,
            Rect.fromLTWH(0, 0, 20, 10), 4, 2, 1600);
        Require(clipped.Supported && clipped.RequiredBytes == 1600, "Final physical clip was ignored.");
        Require(!EffectAllocationPreflight.Evaluate(bounds, PlatformViewTransform.Identity, 2, 2, null, 4, 2, 159999).Supported,
            "Budget boundary was admitted.");
        Require(!EffectAllocationPreflight.Evaluate(Rect.fromLTWH(0, 0, 1e100, 1e100),
            PlatformViewTransform.Identity, 2, 2, null, 4, 2, long.MaxValue).Supported, "Overflow must fail before allocation.");
        Require(!new WebViewFeatures(true, false, false, false, false).Supports(WebViewOperation.Back),
            "Navigation does not imply browser history support.");
        var sceneFactory = new OverlayOnlyFactory();
        using var sceneOwner = new Doroti.Hosting.PlatformViewCoordinator(901, "policy",
            new Doroti.Hosting.PlatformViewFactoryRegistry([sceneFactory]), new ImmediateDispatcher());
        var overlay = new PlatformViewRequest(1, sceneFactory.ViewType);
        Require(sceneOwner.QuerySceneSupport(new([overlay])).Supported, "A standalone native overlay should remain supported.");
        Require(!sceneOwner.QuerySceneSupport(new([overlay], HasBackdrop: true)).Supported,
            "Scene query advertised backdrop sampling on content without a live source.");
        Require(!sceneOwner.QuerySceneSupport(new([overlay], HasForegroundRaster: true)).Supported,
            "Scene query advertised raster foreground ordering on an overlay.");
        Require(sceneFactory.Allocations == 0, "Scene rejection must precede native allocation.");
        var parent = new SemanticsNodeUpdate(1, Rect.fromLTWH(0, 0, 100, 100), "dialog", null,
            SemanticsAction.none, [2], role: SemanticsRole.dialog);
        var child = new SemanticsNodeUpdate(2, Rect.fromLTWH(10, 10, 50, 50), "list", null,
            SemanticsAction.none, [3], role: SemanticsRole.list);
        var item = new SemanticsNodeUpdate(3, Rect.fromLTWH(20, 20, 10, 10), "item", null, SemanticsAction.tap, []);
        var tree = new SemanticsTreeSnapshot([parent, child, item]);
        Require(tree.Parents[3] == 2 && tree.Parents[2] == 1 && tree.Roots.SequenceEqual(new[] { 1 }), "Semantic hierarchy flattened.");
        var reparented = new SemanticsTreeSnapshot([parent with { children = [2, 3] }, child with { children = [] }, item]);
        Require(reparented.Parents[3] == 1, "Semantic reparent left an old parent.");
        try { _ = new SemanticsTreeSnapshot([parent, child with { children = [1] }]); throw new Exception("Semantic cycle admitted."); }
        catch (ArgumentException) { }
        Require(!SemanticsActionPolicy.Allows(item with { flags = new(isEnabled: Tristate.isFalse) }, SemanticsAction.tap),
            "Disabled AT action admitted.");
        var geometry = new SemanticsTextGeometry("한😀e\u0301", 1,
            [new(0, 1, Rect.fromLTWH(0, 0, 10, 20), 0, 5), new(1, 3, Rect.fromLTWH(10, 0, 20, 20), 0, 5),
             new(3, 5, Rect.fromLTWH(30, 0, 10, 20), 0, 5)]);
        Require(geometry.Snap(2) == 1 && geometry.Snap(2, true) == 3 && geometry.Snap(4) == 3, "UTF-16 grapheme boundaries split.");
        Require(geometry.OffsetAtPoint(new(12, 10)) == 1 && geometry.Bounds(1, 3).Single().width == 20,
            "Layout run geometry was replaced with node bounds.");
        Require(Math.Abs(PenMeasurements.FromAxes(60, 60) - Math.Atan(Math.Sqrt(6))) < 1e-10, "Two pen tilt axes must describe tilt from the normal.");
        Require(Math.Abs(PenMeasurements.FromAltitude(Math.PI / 6) - Math.PI / 3) < 1e-10, "UIKit altitude conversion failed.");
        Require(PenMeasurements.Pressure(2) == 1 && PenMeasurements.Pressure(double.NaN) == 0, "Pressure range failed.");
        Require(Math.Abs(PenMeasurements.Orientation(PenMeasurements.Radians(-90)) - 3 * Math.PI / 2) < 1e-10, "Pen rotation units/wrap failed.");
        Console.WriteLine("Platform filter/URL/effect/semantics/pen policy: PASS");
    }
    private sealed class ImmediateDispatcher : Doroti.Hosting.IPlatformViewDispatcher
    {
        public ValueTask InvokeAsync(Func<ValueTask> action) => action();
    }
    private sealed class OverlayOnlyFactory : Doroti.Hosting.IPlatformViewFactory
    {
        public string ViewType => "policy/overlay";
        public int Allocations { get; private set; }
        public PlatformViewSupport QuerySupport(PlatformViewRequest request) => new("policy", "cpu", ViewType,
            true, PlatformViewComposition.NativeOverlay, PlatformViewEffects.RectClip, MixedScene: true);
        public ValueTask<Doroti.Hosting.IPlatformViewInstance> CreateAsync(PlatformViewHandle handle,
            ReadOnlyMemory<byte> parameters, Action<PlatformViewHandle> focused, CancellationToken cancellationToken)
        {
            Allocations++;
            return ValueTask.FromException<Doroti.Hosting.IPlatformViewInstance>(new InvalidOperationException("No native allocation in a preflight test."));
        }
    }
}
