using Doroti.Graphics.DisplayList;

namespace Doroti.Host.Web;

// Retain the mapped body of an immutable Picture, not merely individual paths.
// Font rebinding invalidates blocks. Live dependencies are checked on every hit.
// Images, shaders and filters continue through their normal lifetime-aware path.
internal sealed class BrowserPictureBlockCache(bool enabled)
{
    internal sealed record Block(DisplayListCommand[] Commands, DisplayResourceReference[] Dependencies, long Charge);
    private const long MaximumBytes = 16 * 1024 * 1024;
    private readonly Dictionary<long, Block> _blocks = [];
    private DisplayResourceReference[] _fonts = [];
    private long _bytes;
    internal bool Enabled => enabled;
    internal int Pictures { get; private set; }
    internal int Hits { get; private set; }
    internal int MappedCommands { get; set; }
    internal void BeginFrame(IReadOnlyList<DisplayResourceReference> fonts)
    {
        Pictures = Hits = MappedCommands = 0;
        if (!enabled) return;
        if (!_fonts.SequenceEqual(fonts))
        { _blocks.Clear(); _bytes = 0; _fonts = fonts.ToArray(); }
    }
    internal void Clear() { _blocks.Clear(); _fonts = []; _bytes = 0; Pictures = Hits = MappedCommands = 0; }
    internal bool TryGet(long id, out Block block)
    {
        Pictures++;
        if (enabled && _blocks.TryGetValue(id, out block!)) { Hits++; return true; }
        block = null!; return false;
    }
    internal void Add(long id, DisplayListCommand[] commands, DisplayResourceReference[] dependencies)
    {
        if (!enabled || commands.Length > 4096 || dependencies.Length > 32 ||
            dependencies.Any(d => d.Kind != DisplayResourceKind.Font)) return;
        long charge = 256 + dependencies.Length * 64L;
        foreach (var command in commands)
        {
            var size = Charge(command);
            if (size < 0 || (charge += size) > MaximumBytes / 4) return;
        }
        while (_blocks.Count >= 256 || _bytes + charge > MaximumBytes)
        {
            var first = _blocks.First(); _bytes -= first.Value.Charge; _blocks.Remove(first.Key);
        }
        _blocks.Add(id, new(commands, dependencies, charge)); _bytes += charge;
    }
    private static long Charge(DisplayListCommand command) => command switch
    {
        DisplaySaveCommand or DisplayRestoreCommand or DisplayTransformCommand or DisplayClipRectCommand or
            DisplayClipRoundedRectCommand or DisplayDrawColorCommand => 512,
        DisplayDrawRectCommand c => Simple(c.Paint) ? 512 : -1,
        DisplayDrawRoundedRectCommand c => Simple(c.Paint) ? 512 : -1,
        DisplayDrawDoubleRoundedRectCommand c => Simple(c.Paint) ? 512 : -1,
        DisplayDrawCircleCommand c => Simple(c.Paint) ? 512 : -1,
        DisplayDrawOvalCommand c => Simple(c.Paint) ? 512 : -1,
        DisplayDrawLineCommand c => Simple(c.Paint) ? 512 : -1,
        DisplayDrawArcCommand c => Simple(c.Paint) ? 512 : -1,
        DisplayDrawPaintCommand c => Simple(c.Paint) ? 512 : -1,
        DisplayClipPathCommand c => PathCharge(c.Path),
        DisplayDrawPathCommand c => Simple(c.Paint) ? PathCharge(c.Path) : -1,
        DisplayDrawShadowCommand c => PathCharge(c.Path),
        DisplayDrawParagraphCommand c => ParagraphCharge(c.Paragraph),
        _ => -1,
    };
    private static long PathCharge(DisplayPath path) => 512 + path.Values.Count * 16L + path.Verbs.Count * 8L;
    private static long ParagraphCharge(DisplayParagraphRecipe p)
    {
        long size = 512 + 2L * ((long)p.Text.Length + p.FontFamily.Length + p.Locale.Length + (p.Ellipsis?.Length ?? 0)) + p.FallbackFonts.Count * 64L;
        foreach (var run in p.TextRuns)
        {
            size += 512 + 2L * ((long)run.Text.Length + run.FontFamily.Length + run.Locale.Length) + run.Shadows.Count * 128L;
            foreach (var value in run.FontFamilyFallback) size += 64 + value.Length * 2L;
            foreach (var value in run.FontFeatures) size += 64 + value.Name.Length * 2L;
            foreach (var value in run.FontVariations) size += 64 + value.Axis.Length * 2L;
        }
        return size;
    }
    private static bool Simple(DisplayPaint paint) => paint.Shader is null && paint.ColorFilter is null &&
        paint.MaskFilter is null && paint.ImageFilter is null;
}
