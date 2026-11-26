// Adapted from Avalonia 12.2.0 for Doroti. See paragraph-unicode-provenance.json.
using System;

namespace Doroti.Skia.Rendering.Unicode;

internal ref struct CodepointEnumerator
{
    private readonly ReadOnlySpan<char> _text;
    private int _offset;

    public CodepointEnumerator(ReadOnlySpan<char> text)
        => _text = text;

    /// <summary>
    /// Moves to the next <see cref="Codepoint"/>.
    /// </summary>
    /// <returns></returns>
    public bool MoveNext(out Codepoint codepoint)
    {
        if ((uint)_offset >= (uint)_text.Length)
        {
            codepoint = Codepoint.ReplacementCodepoint;
            return false;
        }

        codepoint = Codepoint.ReadAt(_text, _offset, out var count);

        _offset += count;

        return true;
    }
}
