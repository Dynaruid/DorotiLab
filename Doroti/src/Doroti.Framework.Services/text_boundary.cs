#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/text_boundary.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Services;

public delegate bool UntilPredicate(long offset, bool forward);

public abstract class TextBoundary
{
    protected TextBoundary()
    {
    }

    public virtual long? getLeadingTextBoundaryAt(long position)
    {
        if ((position < 0L))
        {
            return null;
        }
        long start = getTextBoundaryAt(position).start;
        return ((start >= 0L) ? start : null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long? getTrailingTextBoundaryAt(long position)
    {
        long end = getTextBoundaryAt(Math.Max(0L, position)).end;
        return ((end >= 0L) ? end : null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TextRange getTextBoundaryAt(long position)
    {
        long start = (getLeadingTextBoundaryAt(position) ?? -1L);
        long end = (getTrailingTextBoundaryAt(position) ?? -1L);
        return new global::Doroti.Ui.TextRange(start: start, end: end);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CharacterBoundary : TextBoundary
{
    internal virtual string _text { get; private set; } = default!;

    public CharacterBoundary(string _text)
    {
        this._text = _text;
    }

    public override long? getLeadingTextBoundaryAt(long position)
    {
        if ((position < 0L))
        {
            return null;
        }
        long graphemeStart = new CharacterRange(_text, Math.Min(position, _text.Length)).stringBeforeLength;
        DartRuntimePrimitives.Assert(() => (new CharacterRange(_text, graphemeStart).Count == 0));
        return graphemeStart;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long? getTrailingTextBoundaryAt(long position)
    {
        if ((position >= _text.Length))
        {
            return null;
        }
        var rangeAtPosition = new CharacterRange(_text, Math.Max(0L, (position + 1L)));
        long nextBoundary = (rangeAtPosition.stringBeforeLength + rangeAtPosition.Current.Length);
        DartRuntimePrimitives.Assert(() => ((nextBoundary == _text.Length) || (new CharacterRange(_text, nextBoundary).Count == 0)));
        return nextBoundary;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override TextRange getTextBoundaryAt(long position)
    {
        if ((position < 0L))
        {
            return new global::Doroti.Ui.TextRange(start: -1L, end: (getTrailingTextBoundaryAt(position) ?? -1L));
        }
        else
        {
            if ((position >= _text.Length))
            {
                return new global::Doroti.Ui.TextRange(start: (getLeadingTextBoundaryAt(position) ?? -1L), end: -1L);
            }
        }
        var rangeAtPosition = new CharacterRange(_text, position);
        return ((rangeAtPosition.Count != 0) ? new global::Doroti.Ui.TextRange(start: rangeAtPosition.stringBeforeLength, end: (rangeAtPosition.stringBeforeLength + rangeAtPosition.Current.Length)) : new global::Doroti.Ui.TextRange(start: rangeAtPosition.stringBeforeLength, end: (getTrailingTextBoundaryAt(position) ?? -1L)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class LineBoundary : TextBoundary
{
    internal virtual TextLayoutMetrics _textLayout { get; private set; } = default!;

    public LineBoundary(TextLayoutMetrics _textLayout)
    {
        this._textLayout = _textLayout;
    }

    public override TextRange getTextBoundaryAt(long position) => _textLayout.getLineAtOffset(new global::Doroti.Ui.TextPosition(offset: Math.Max(position, 0L)));
}

public class ParagraphBoundary : TextBoundary
{
    internal virtual string _text { get; private set; } = default!;

    public ParagraphBoundary(string _text)
    {
        this._text = _text;
    }

    public override long? getLeadingTextBoundaryAt(long position)
    {
        if (((position < 0L) || (_text.Length == 0)))
        {
            return null;
        }
        if ((position >= _text.Length))
        {
            return _text.Length;
        }
        if ((position == 0L))
        {
            return 0L;
        }
        var index = position;
        if ((((index > 1L) && (_text.codeUnitAt(index) == 10L)) && (_text.codeUnitAt((index - 1L)) == 13L)))
        {
            index -= 2L;
        }
        else
        {
            if (TextLayoutMetrics.isLineTerminator(_text.codeUnitAt(index)))
            {
                index -= 1L;
            }
        }
        while ((index > 0L))
        {
            if (TextLayoutMetrics.isLineTerminator(_text.codeUnitAt(index)))
            {
                return (index + 1L);
            }
            index -= 1L;
        }
        return Math.Max(index, 0L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long? getTrailingTextBoundaryAt(long position)
    {
        if (((position >= _text.Length) || (_text.Length == 0)))
        {
            return null;
        }
        if ((position < 0L))
        {
            return 0L;
        }
        var index = position;
        while (!TextLayoutMetrics.isLineTerminator(_text.codeUnitAt(index)))
        {
            index += 1L;
            if ((index == _text.Length))
            {
                return index;
            }
        }
        return ((((index < (_text.Length - 1L)) && (_text.codeUnitAt(index) == 13L)) && (_text.codeUnitAt((index + 1L)) == 10L)) ? (index + 2L) : (index + 1L));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DocumentBoundary : TextBoundary
{
    internal virtual string _text { get; private set; } = default!;

    public DocumentBoundary(string _text)
    {
        this._text = _text;
    }

    public override long? getLeadingTextBoundaryAt(long position) => ((position < 0L) ? null : 0L);
    public override long? getTrailingTextBoundaryAt(long position) => ((position >= _text.Length) ? null : _text.Length);
}

