using System.Reflection;
using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Ui;

internal static class CaretGeometryRegression
{
    internal static void Run()
    {
        var capture = typeof(RenderBox).Assembly.GetType("Doroti.Framework.Rendering.RenderSemanticsTextGeometry")!
            .GetMethod("Capture", BindingFlags.Static | BindingFlags.NonPublic)!
            .CreateDelegate<Func<TextPainter, Offset, bool, SemanticsTextGeometry?>>();
        var painter = new TextPainter(text: new TextSpan(text: "ab😀e\u0301 한글 cursor movement"), textDirection: TextDirection.ltr);
        painter.layout(maxWidth: 120);
        var first = capture(painter, Offset.zero, false)!;
        Check(first.Runs.Count > 0 && first.CharacterBoundaries.Contains(4), "actual grapheme layout is retained");
        // Selection-only rebuilds call layout again and describe/assemble
        // semantics independently. Neither operation changes glyph positions.
        painter.layout(maxWidth: 120);
        Check(ReferenceEquals(first, capture(painter, Offset.zero, false)), "unchanged layout reuses immutable geometry");
        var bytes = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 20; i++) capture(painter, Offset.zero, false);
        Check(GC.GetAllocatedBytesForCurrentThread() == bytes, "repeated caret semantics captures allocate no geometry");
        var shifted = capture(painter, new Offset(3, 7), false)!;
        Check(shifted.Runs[0].Bounds == first.Runs[0].Bounds.shift(new Offset(3, 7)), "scroll offset invalidates geometry");
        Check(first.Runs[0].Bounds != shifted.Runs[0].Bounds, "old snapshots remain immutable");
        painter.text = new TextSpan(text: "zz😀e\u0301 한글 cursor movement");
        Check(capture(painter, Offset.zero, false) is null, "invalidated layout cannot expose stale text geometry");
        painter.layout(maxWidth: 120);
        var edited = capture(painter, Offset.zero, false)!;
        Check(edited.Text.StartsWith("zz") && edited.Revision != first.Revision, "same-length text edits refresh geometry");
        painter.layout(maxWidth: 65);
        var wrapped = capture(painter, Offset.zero, false)!;
        Check(wrapped.Revision != edited.Revision && wrapped.Runs.Max(r => r.Bounds.bottom) > edited.Runs.Max(r => r.Bounds.bottom), "wrapping refreshes line geometry");
        painter.textScaler = TextScaler.CreateLinear(1.5);
        painter.layout(maxWidth: 65);
        Check(capture(painter, Offset.zero, false)!.Revision != wrapped.Revision, "text scale refreshes geometry");
        Check(capture(painter, Offset.zero, true) is null, "obscured text publishes no geometry");
        var revealed = capture(painter, Offset.zero, false)!;
        Check(!ReferenceEquals(revealed, wrapped), "obscured state clears the old cache");
        painter.dispose();
        Check(capture(painter, Offset.zero, false) is null, "disposed painter cannot expose cached geometry");

        // _resizeToFit can reuse a paragraph while changing centered/RTL paint
        // positions. Its content width is part of the geometry revision.
        var centered = new TextPainter(text: new TextSpan(text: "short"), textAlign: TextAlign.center, textDirection: TextDirection.rtl);
        centered.layout(minWidth: 100, maxWidth: 200);
        var narrow = capture(centered, Offset.zero, false)!;
        centered.layout(minWidth: 180, maxWidth: 200);
        var wide = capture(centered, Offset.zero, false)!;
        Check(narrow.Revision != wide.Revision && narrow.Runs[0].Bounds != wide.Runs[0].Bounds, "reused RTL/center paragraph tracks changed paint alignment");
        centered.dispose();
        Console.WriteLine("PASS: caret semantics cache, graphemes, edit/wrap/scale/offset/obscure/dispose invalidation");
    }

    private static void Check(bool value, string message)
    {
        if (!value) throw new InvalidOperationException(message);
    }
}
