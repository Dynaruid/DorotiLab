namespace Doroti.Graphics.DisplayList;

/// <summary>
/// Producer-local cache of validated, table-independent immutable command
/// payloads. It is not thread-safe. Resource/string references and variable-size
/// paths, shaders and filters always pass through the ordinary encoder.
/// </summary>
public sealed class DisplayListEncodingCache
{
    private const int MaximumEntries = 8192;
    private const int MaximumChargedBytes = 8 * 1024 * 1024;
    private const int KeyCharge = 512;
    private readonly Dictionary<DisplayListCommand, byte[]> _payloads = [];
    public int RetainedBytes { get; private set; }
    public int EntryCount => _payloads.Count;
    public int FrameHits { get; private set; }
    public int FrameMisses { get; private set; }
    internal void BeginFrame() { FrameHits = 0; FrameMisses = 0; }
    // Remember only a size, never a scratch buffer. The cap limits over-allocation
    // after a large scene; the next successful scene immediately replaces it.
    internal int CommandCapacityHint { get; private set; } = 256;
    internal void RecordCommandLength(int length) =>
        CommandCapacityHint = Math.Clamp(length, 256, 1024 * 1024);
    internal bool TryGet(DisplayListCommand command, out byte[] payload)
    {
        if (Eligible(command) && _payloads.TryGetValue(command, out payload!))
        { FrameHits++; return true; }
        FrameMisses++;
        payload = [];
        return false;
    }
    internal void Add(DisplayListCommand command, ReadOnlySpan<byte> payload)
    {
        if (!Eligible(command)) return;
        var charge = KeyCharge + payload.Length;
        if (charge > MaximumChargedBytes) return;
        while (_payloads.Count >= MaximumEntries || RetainedBytes + charge > MaximumChargedBytes)
        {
            var first = _payloads.First();
            RetainedBytes -= KeyCharge + first.Value.Length;
            _payloads.Remove(first.Key);
        }
        _payloads.Add(command, payload.ToArray());
        RetainedBytes += charge;
    }
    public void Clear() { _payloads.Clear(); RetainedBytes = 0; CommandCapacityHint = 256; BeginFrame(); }
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
        _ => false,
    };
    private static bool Simple(DisplayPaint paint) => paint is not null && paint.Shader is null &&
        paint.ColorFilter is null && paint.MaskFilter is null && paint.ImageFilter is null;
}
