namespace Doroti.Graphics.DisplayList;

/// <summary>
/// Producer-local cache of validated immutable command
/// payloads. It is not thread-safe. Table-dependent text is invalidated whenever
/// the canonical string/resource tables change. Shaders and filters remain inline.
/// </summary>
public sealed class DisplayListEncodingCache
{
    private const int MaximumEntries = 8192;
    private const int MaximumChargedBytes = 8 * 1024 * 1024;
    private const int KeyCharge = 512;
    private readonly Dictionary<DisplayListCommand, (byte[] Payload, int Charge)> _payloads = [];
    // Retained picture bodies return the same immutable command instances.
    // Avoid recursively hashing their paint/paragraph records on every frame.
    // This index contains exactly the canonical keys in _payloads, no aliases.
    private readonly Dictionary<DisplayListCommand, byte[]> _identities = new(ReferenceEqualityComparer.Instance);
    private sealed record CommandBlock(DisplayListCommand[] Commands, byte[] Bytes, int Charge);
    private readonly Dictionary<int, CommandBlock> _blocks = [];
    private int _blockBytes;
    private DisplayParagraphRecipe[]? _stringRecipes;
    private List<(string Value, byte[] Bytes)>? _stringTable;
    private Dictionary<string, uint>? _stringIds;
    internal const int BlockLength = 16;
    public int FrameBlockHits { get; private set; }
    public bool FrameStringTableHit { get; private set; }
    public int RetainedBlockBytes => _blockBytes;
    public int BlockCount => _blocks.Count;
    internal bool TryGetStringTable(IReadOnlyList<DisplayListCommand> commands,
        out List<(string Value, byte[] Bytes)> strings, out Dictionary<string, uint> ids)
    {
        strings = null!; ids = null!;
        if (_stringRecipes is null) return false;
        var index = 0;
        foreach (var command in commands)
            if (command is DisplayDrawParagraphCommand paragraph &&
                (index >= _stringRecipes.Length || !SameStrings(_stringRecipes[index++], paragraph.Paragraph))) return false;
        if (index != _stringRecipes.Length) return false;
        strings = _stringTable!; ids = _stringIds!; FrameStringTableHit = true; return true;
    }
    private static bool SameStrings(DisplayParagraphRecipe a, DisplayParagraphRecipe b)
    {
        if (ReferenceEquals(a, b)) return true;
        // A repaint can replace geometry/color while retaining every string.
        // Compare only the fields that participate in the canonical table.
        if (a.Text != b.Text || a.FontFamily != b.FontFamily || a.Locale != b.Locale || a.Ellipsis != b.Ellipsis ||
            a.TextRuns.Count != b.TextRuns.Count) return false;
        for (var i = 0; i < a.TextRuns.Count; i++)
        {
            var x = a.TextRuns[i]; var y = b.TextRuns[i];
            if (ReferenceEquals(x, y)) continue;
            if (x.Text != y.Text || x.FontFamily != y.FontFamily || x.Locale != y.Locale ||
                !x.FontFamilyFallback.SequenceEqual(y.FontFamilyFallback) ||
                x.FontFeatures.Count != y.FontFeatures.Count || x.FontVariations.Count != y.FontVariations.Count) return false;
            for (var j = 0; j < x.FontFeatures.Count; j++) if (x.FontFeatures[j].Name != y.FontFeatures[j].Name) return false;
            for (var j = 0; j < x.FontVariations.Count; j++) if (x.FontVariations[j].Axis != y.FontVariations[j].Axis) return false;
        }
        return true;
    }
    internal void RememberStringTable(IReadOnlyList<DisplayListCommand> commands,
        List<(string Value, byte[] Bytes)> strings, Dictionary<string, uint> ids)
    {
        _stringRecipes = null; _stringTable = null; _stringIds = null;
        if (strings.Sum(s => 64L + s.Value.Length * 2L + s.Bytes.Length) > 2 * 1024 * 1024) return;
        var recipes = new List<DisplayParagraphRecipe>();
        long charge = 0;
        foreach (var command in commands)
        {
            if (command is not DisplayDrawParagraphCommand paragraph) continue;
            var recipe = paragraph.Paragraph;
            charge += KeyCharge + TextCharge(command) + recipe.FallbackFonts.Count * 64L;
            foreach (var run in recipe.TextRuns)
                charge += 512 + 128L * ((long)run.Shadows.Count + run.FontFeatures.Count + run.FontVariations.Count + run.FontFamilyFallback.Count);
            if (charge > 2 * 1024 * 1024 || recipes.Count >= 512) return;
            recipes.Add(recipe);
        }
        _stringRecipes = recipes.ToArray(); _stringTable = strings; _stringIds = ids;
    }
    private string[] _strings = [];
    private DisplayResourceDescriptor[] _resources = [];
    private bool _tablesCacheable = true;
    public int RetainedBytes { get; private set; }
    public int EntryCount => _payloads.Count;
    public int FrameHits { get; private set; }
    public int FrameMisses { get; private set; }
    internal void BeginFrame() { FrameHits = 0; FrameMisses = 0; FrameBlockHits = 0; FrameStringTableHit = false; }
    internal void SetTables(IReadOnlyList<(string Value, byte[] Bytes)> strings,
        IReadOnlyList<DisplayResourceDescriptor> resources)
    {
        // Canonical tables are validated before this call. Reuse cannot bypass
        // missing-resource validation or emit a previous scene's string indices.
        _tablesCacheable = strings.Sum(s => (long)s.Value.Length * 2 + 32) <= 2 * 1024 * 1024 && resources.Count <= 1024;
        var same = _tablesCacheable && _strings.Length == strings.Count && _resources.Length == resources.Count;
        for (var i = 0; same && i < strings.Count; i++) same = _strings[i] == strings[i].Value;
        for (var i = 0; same && i < resources.Count; i++) same = _resources[i] == resources[i];
        if (same) return;
        _blocks.Clear(); _blockBytes = 0;
        // A newly visible label changes canonical text indices, but does not
        // change any geometry-only instruction. Invalidating the entire cache
        // here made every first section entry re-encode the whole scene.
        if (!_tablesCacheable) { _payloads.Clear(); _identities.Clear(); RetainedBytes = 0; }
        else foreach (var entry in _payloads.Where(p => p.Key is DisplayDrawParagraphCommand).ToArray())
        { _payloads.Remove(entry.Key); _identities.Remove(entry.Key); RetainedBytes -= entry.Value.Charge; }
        _strings = _tablesCacheable ? strings.Select(s => s.Value).ToArray() : [];
        _resources = _tablesCacheable ? resources.ToArray() : [];
    }
    // Remember only a size, never a scratch buffer. The cap limits over-allocation
    // after a large scene; the next successful scene immediately replaces it.
    internal int CommandCapacityHint { get; private set; } = 256;
    internal bool TryGetBlock(IReadOnlyList<DisplayListCommand> commands, int start, int count, out byte[] bytes)
    {
        bytes = [];
        if (!_tablesCacheable || !_blocks.TryGetValue(start, out var block) || block.Commands.Length != count) return false;
        for (var i = 0; i < count; i++)
            if (!ReferenceEquals(block.Commands[i], commands[start + i]) && !block.Commands[i].Equals(commands[start + i])) return false;
        bytes = block.Bytes; FrameBlockHits++; FrameHits += count; return true;
    }
    internal void AddBlock(IReadOnlyList<DisplayListCommand> commands, int start, int count, ReadOnlySpan<byte> bytes)
    {
        if (!_tablesCacheable || count < 8 || start >= 8192) return;
        if (_blocks.Remove(start, out var old)) _blockBytes -= old.Charge;
        long charge = 128 + count * (long)KeyCharge + bytes.Length * 8L;
        for (var i = 0; i < count; i++)
        {
            if (!Eligible(commands[start + i])) return;
            charge += TextCharge(commands[start + i]);
        }
        // Blocks copy validated instructions in bulk. Bound their independent
        // retained keys/payloads as well as the individual instruction cache.
        if (charge > 4 * 1024 * 1024) return;
        while (_blockBytes + charge > 4 * 1024 * 1024)
        { var first = _blocks.First(); _blocks.Remove(first.Key); _blockBytes -= first.Value.Charge; }
        var keys = new DisplayListCommand[count];
        for (var i = 0; i < count; i++) keys[i] = commands[start + i];
        _blocks.Add(start, new(keys, bytes.ToArray(), (int)charge)); _blockBytes += (int)charge;
    }
    internal void RecordCommandLength(int length) =>
        CommandCapacityHint = Math.Clamp(length, 256, 1024 * 1024);
    internal bool TryGet(DisplayListCommand command, out byte[] payload)
    {
        if (_tablesCacheable && _identities.TryGetValue(command, out payload!))
        { FrameHits++; return true; }
        if (_tablesCacheable && Eligible(command) && _payloads.TryGetValue(command, out var entry))
        { FrameHits++; payload = entry.Payload; return true; }
        FrameMisses++;
        payload = [];
        return false;
    }
    internal void Add(DisplayListCommand command, ReadOnlySpan<byte> payload)
    {
        if (!_tablesCacheable || !Eligible(command)) return;
        if (payload.Length > (MaximumChargedBytes - KeyCharge) / 8) return;
        var fullCharge = KeyCharge + (long)payload.Length * 8 + TextCharge(command);
        if (fullCharge > MaximumChargedBytes) return;
        var charge = (int)fullCharge;
        while (_payloads.Count >= MaximumEntries || RetainedBytes + charge > MaximumChargedBytes)
        {
            var first = _payloads.First();
            RetainedBytes -= first.Value.Charge;
            _payloads.Remove(first.Key);
            _identities.Remove(first.Key);
        }
        var owned = payload.ToArray();
        _payloads.Add(command, (owned, charge));
        _identities.Add(command, owned);
        RetainedBytes += charge;
    }
    // Equal strings need not share storage with the canonical table. Charge
    // every retained text/style string, including large text with a tiny wire ID.
    private static long TextCharge(DisplayListCommand command)
    {
        if (command is not DisplayDrawParagraphCommand c) return 0;
        var p = c.Paragraph;
        long chars = (long)p.Text.Length + p.FontFamily.Length + p.Locale.Length + (p.Ellipsis?.Length ?? 0);
        foreach (var r in p.TextRuns)
        {
            chars += (long)r.Text.Length + r.FontFamily.Length + r.Locale.Length;
            foreach (var f in r.FontFamilyFallback) chars += f.Length;
            foreach (var f in r.FontFeatures) chars += f.Name.Length;
            foreach (var v in r.FontVariations) chars += v.Axis.Length;
        }
        return chars * 2;
    }
    public void Clear() { _payloads.Clear(); _identities.Clear(); _blocks.Clear(); _blockBytes = 0; _stringRecipes = null; _stringTable = null; _stringIds = null; _strings = []; _resources = []; _tablesCacheable = true; RetainedBytes = 0; CommandCapacityHint = 256; BeginFrame(); }
    private static bool Eligible(DisplayListCommand command) => command switch
    {
        DisplaySaveCommand or DisplayRestoreCommand or DisplayTransformCommand or
            DisplayClipRectCommand or DisplayClipRoundedRectCommand or DisplayDrawColorCommand => true,
        DisplayDrawRectCommand c => Simple(c.Paint),
        DisplayDrawRoundedRectCommand c => Simple(c.Paint),
        DisplayDrawDoubleRoundedRectCommand c => Simple(c.Paint),
        DisplayDrawCircleCommand c => Simple(c.Paint),
        DisplayDrawOvalCommand c => Simple(c.Paint),
        DisplayDrawLineCommand c => Simple(c.Paint),
        DisplayDrawArcCommand c => Simple(c.Paint),
        DisplayDrawPaintCommand c => Simple(c.Paint),
        DisplayClipPathCommand or DisplayDrawShadowCommand or DisplayDrawParagraphCommand => true,
        DisplayDrawPathCommand c => Simple(c.Paint),
        DisplayDrawPointsCommand c => Simple(c.Paint),
        DisplaySaveLayerCommand c => c.Paint is null || Simple(c.Paint),
        DisplayPushOpacityCommand => true,
        _ => false,
    };
    private static bool Simple(DisplayPaint paint) => paint is not null && paint.Shader is null &&
        paint.ColorFilter is null && paint.MaskFilter is null && paint.ImageFilter is null;
}
