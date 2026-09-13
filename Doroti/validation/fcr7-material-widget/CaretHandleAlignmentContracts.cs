using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Services;
using Doroti.Ui;

internal static class CaretHandleAlignmentContracts
{
    internal static void Verify()
    {
        using var environment = new ImageFixtureEnvironment();
        environment.Renderer.RegisterFontAsync(File.ReadAllBytes("DorotiTestbedApp/assets/fonts/Roboto-medium.ttf"), "Roboto").GetAwaiter().GetResult();
        VerifyEmptyFieldGeometry();
        var cases = 0;
        foreach (var platform in new[] { HostOperatingSystem.android, HostOperatingSystem.iOS, HostOperatingSystem.windows })
        using (PlatformEnvironmentContext.Enter(new PlatformConfiguration(
            [new Locale("en", "US")], Brightness.light, false, false, platform)))
        foreach (var dpr in new[] { 1.0, 1.5, 2.625, 3.0 })
        foreach (var width in new[] { 1.0, 2.0, 5.0 })
        foreach (var direction in new[] { TextDirection.ltr, TextDirection.rtl })
        foreach (var scroll in new[] { 0.0, 12.25 })
        foreach (var cursorOffset in new[] { Offset.zero, new Offset(1.25, 0) })
        {
            const string text = "iiiiWWWW variable width text";
            var style = new Doroti.Framework.Painting.TextStyle(fontFamily: "Roboto", fontSize: 18);
            var editable = new RenderEditable(
                text: new TextSpan(text: text, style: style), textDirection: direction,
                startHandleLayerLink: new LayerLink(), endHandleLayerLink: new LayerLink(),
                offset: ViewportOffset.CreateFixed(scroll), cursorWidth: width,
                cursorOffset: cursorOffset, devicePixelRatio: dpr,
                selection: TextSelection.CreateCollapsed(4));
            var parent = new RenderPadding(padding: EdgeInsets.CreateOnly(left: .375, top: .625)) { child = editable };
            var owner = new PipelineOwner { rootNode = parent };
            parent.layout(BoxConstraints.CreateTight(new Size(180, 40)));
            try
            {
                foreach (var position in new[] { 0, 4, 8, text.Length })
                {
                    var selection = TextSelection.CreateCollapsed(position);
                    var endpoint = editable.getEndpointsForSelection(selection).Single().point;
                    var caret = editable.getLocalRectForCaret(selection.extent);
                    if (platform == HostOperatingSystem.android)
                        Near(endpoint.dx, caret.center.dx, $"Android caret/handle center {dpr}/{width}/{direction}/{scroll}/{position}");
                    // Platform-specific horizontal alignment must not move the
                    // handle vertically or alter the Apple/desktop endpoint.
                    var painter = new TextPainter(text: new TextSpan(text: text, style: style), textDirection: direction);
                    painter.layout(maxWidth: double.PositiveInfinity);
                    var raw = painter.getOffsetForCaret(selection.extent, Rect.fromLTWH(0, 0, width, editable.preferredLineHeight));
                    Near(endpoint.dy, raw.dy + editable.preferredLineHeight, "line-bottom handle anchor");
                    if (platform != HostOperatingSystem.android)
                        Near(endpoint.dx, raw.dx - editable.offset.pixels, "unchanged non-Android endpoint");
                    painter.dispose();
                    cases++;
                }
                var range = new TextSelection(2, 8);
                var forward = editable.getEndpointsForSelection(range);
                var reverse = editable.getEndpointsForSelection(new TextSelection(8, 2));
                var boxes = editable.getBoxesForSelection(range);
                Near(forward[0].point.dx, boxes.First().start, "range start stays on selection edge");
                Near(forward[1].point.dx, boxes.Last().end, "range end stays on selection edge");
                for (var i = 0; i < 2; i++)
                    Near(forward[i].point.dx, reverse[i].point.dx, "reversed range endpoints");
            }
            finally
            {
                owner.rootNode = null;
                parent.child = null;
                editable.dispose();
                parent.dispose();
                owner.dispose();
            }
        }
        Console.WriteLine($"Caret/handle alignment PASS: {cases} collapsed cases, range and reversed-range edges, fractional parent origin, DPR, cursor width/offset, scrolling, LTR/RTL, Android/iOS/Windows.");
    }

    private static void VerifyEmptyFieldGeometry()
    {
        foreach (var platform in new[] { HostOperatingSystem.android, HostOperatingSystem.iOS })
        using (PlatformEnvironmentContext.Enter(new PlatformConfiguration(
            [new Locale("en", "US")], Brightness.light, false, false, platform)))
        foreach (var direction in new[] { TextDirection.ltr, TextDirection.rtl })
        foreach (var fontSize in new[] { 14.0, 24.0 })
        {
            var style = new Doroti.Framework.Painting.TextStyle(fontFamily: "Roboto", fontSize: fontSize);
            var editable = new RenderEditable(
                text: new TextSpan(text: "", style: style), textDirection: direction,
                startHandleLayerLink: new LayerLink(), endHandleLayerLink: new LayerLink(),
                offset: ViewportOffset.CreateFixed(0), selection: TextSelection.CreateCollapsed(0));
            var owner = new PipelineOwner { rootNode = editable };
            try
            {
                // InputDecorator gives its editable loose vertical constraints.
                // A tight-height parent would mask an empty paragraph's zero height.
                foreach (var text in new[] { "", "text", "" })
                {
                    editable.text = new TextSpan(text: text, style: style);
                    editable.selection = TextSelection.CreateCollapsed(text.Length);
                    editable.layout(new BoxConstraints(maxWidth: 180, maxHeight: 100));
                    var endpoint = editable.getEndpointsForSelection(editable.selection!).Single().point;
                    var lineHeight = editable.preferredLineHeight;
                    Near(editable.size.height, lineHeight, $"{platform}/{direction}/{fontSize}/{text.Length}: editable line height");
                    Near(Math.Clamp(endpoint.dy, 0, editable.size.height), lineHeight,
                        "handle leader remains at line bottom after clearing text");
                    if (text.Length == 0)
                    {
                        using var paragraph = new ParagraphBuilder(new ParagraphStyle(
                            fontFamily: "Roboto", fontSize: fontSize, textDirection: direction)).build();
                        paragraph.layout(new ParagraphConstraints(180));
                        Near(paragraph.height, lineHeight, "empty paragraph reserves a line height");
                        if (paragraph.numberOfLines != 0 || paragraph.computeLineMetrics().Count != 0 ||
                            paragraph.getLineMetricsAt(0) is not null || paragraph.getGlyphInfoAt(0) is not null ||
                            paragraph.getClosestGlyphInfoForOffset(Offset.zero) is not null)
                            throw new InvalidOperationException("Empty paragraph invented line metrics or glyphs.");
                    }
                }
            }
            finally
            {
                owner.rootNode = null;
                editable.dispose();
                owner.dispose();
            }
        }
        Console.WriteLine("Empty-field handle anchors: initial empty, populated, cleared; Android/iOS, LTR/RTL, font sizes PASS.");
    }

    private static void Near(double actual, double expected, string label)
    {
        if (Math.Abs(actual - expected) > .00001)
            throw new InvalidOperationException($"{label}: actual={actual}, expected={expected}");
    }
}
