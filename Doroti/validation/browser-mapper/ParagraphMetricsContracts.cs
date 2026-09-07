using Doroti.Host.Web;
using Doroti.Ui;

internal static class ParagraphMetricsContracts
{
    internal static void Verify()
    {
        double[] wire = [0x4454504d, 1, 40, 20, 15, 18, 10, 39.5, 10, 0,
            uint.MaxValue, 0xabcdef01, 1, 1, 1, 0,
            10,
            0, 1, 1, 15, 5, 20, 10, 0, 15,
            0, 1, 0, 0, 10, 20, 0, 20, 1];
        var decoded = BrowserParagraphMetrics.Decode(wire, 1, false);
        Check(decoded.MetricsHash == 0xabcdef01ffffffffUL && decoded.Graphemes[0].Direction == TextDirection.rtl &&
            decoded.Lines[0].HardBreak && decoded.CodeUnitAdvances[0] == 10 && decoded.MaxIntrinsicWidth == 39.5,
            "numeric metrics preserve exact hash, glyphs, direction, lines and intrinsics");
        Check(BrowserParagraphMetrics.Decode(wire, 1, true).MaxIntrinsicWidth == 40,
            "unconstrained layout preserves the proven finite paint width");
        foreach (var (index, invalid) in new (int, double)[] { (0, 0), (1, 2), (2, double.NaN), (3, -1),
            (9, 2), (10, (double)uint.MaxValue + 1), (12, int.MaxValue), (13, .5), (15, 1), (16, -1), (18, 2), (34, 2) })
        {
            var bad = wire.ToArray(); bad[index] = invalid;
            try { BrowserParagraphMetrics.Decode(bad, 1, false); throw new Exception($"Accepted invalid metrics at {index}"); }
            catch (InvalidDataException) { }
        }
        var cache = new BrowserParagraphLayoutCache(); var calls = 0;
        ParagraphHostLayoutSnapshot Create() { calls++; return decoded; }
        cache.GetOrCreate(40, 1, Create); cache.GetOrCreate(100, 1, Create); cache.GetOrCreate(40, 1, Create);
        Check(calls == 2, "alternate measured widths reuse their immutable snapshots");
        cache.GetOrCreate(40, 2, Create);
        Check(calls == 3, "font registration invalidates the same width");
        cache.GetOrCreate(100, 2, Create); cache.GetOrCreate(200, 2, Create); cache.GetOrCreate(40, 2, Create);
        Check(calls == 6, "only the two most recent widths are retained");
        Console.WriteLine("Paragraph metrics: PASS (numeric transport, malformed tables, exact hash, width/font invalidation)");
    }
    private static void Check(bool value, string message) { if (!value) throw new Exception(message); }
}
