// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/inline_span.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public class Accumulator
{
    internal virtual long _value { get; set; } = default!;

    public Accumulator(long _value = 0)
    {
        this._value = _value;
    }

    public virtual long value => _value;
    public virtual void increment(long addend)
    {
        DartRuntimePrimitives.Assert(() => addend >= 0L);
        _value += addend;
    }

}

public delegate bool InlineSpanVisitor(InlineSpan span);

public class InlineSpanSemanticsInformation
{
    public static InlineSpanSemanticsInformation placeholder = new InlineSpanSemanticsInformation("￼", isPlaceholder: true);
    public virtual string text { get; private set; } = default!;
    public virtual string? semanticsLabel { get; private set; }
    public virtual string? semanticsIdentifier { get; private set; }
    public virtual GestureRecognizer? recognizer { get; private set; }
    public virtual bool isPlaceholder { get; private set; } = default!;
    public virtual bool requiresOwnNode { get; private set; } = default!;
    public virtual List<StringAttribute> stringAttributes { get; private set; } = default!;

    public InlineSpanSemanticsInformation(string text, bool isPlaceholder = false, string? semanticsLabel = null, string? semanticsIdentifier = null, List<StringAttribute> stringAttributes = default!, GestureRecognizer? recognizer = null)
    {
        List<StringAttribute> __stringAttributes = stringAttributes ?? new List<global::Doroti.Ui.StringAttribute>();
        this.text = text;
        this.isPlaceholder = isPlaceholder;
        this.semanticsLabel = semanticsLabel;
        this.semanticsIdentifier = semanticsIdentifier;
        this.stringAttributes = __stringAttributes;
        this.recognizer = recognizer;
        requiresOwnNode = isPlaceholder || (recognizer is not null) || (semanticsIdentifier is not null);
        System.Diagnostics.Debug.Assert(!isPlaceholder || (text == "￼") && (semanticsLabel is null) && (recognizer is null));
    }

    public override bool Equals(object? other)
    {
        var __other = other as InlineSpanSemanticsInformation;
        if (__other is null) return false;
        return (__other is InlineSpanSemanticsInformation) && (__other.text == text) && (__other.semanticsLabel == semanticsLabel) && (__other.semanticsIdentifier == semanticsIdentifier) && Equals(__other.recognizer, recognizer) && (__other.isPlaceholder == isPlaceholder) && CollectionsLibrary.listEquals<global::Doroti.Ui.StringAttribute>(__other.stringAttributes, stringAttributes);
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(text, semanticsLabel, semanticsIdentifier, recognizer, isPlaceholder);
    public override string ToString() => $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "InlineSpanSemanticsInformation")}{{text: {text}, semanticsLabel: {semanticsLabel}, semanticsIdentifier: {semanticsIdentifier}, recognizer: {recognizer}}}";
}

public static partial class Inline_spanLibrary
{
    public static List<InlineSpanSemanticsInformation> combineSemanticsInfo(List<InlineSpanSemanticsInformation> infoList)
    {
        var combined = new List<InlineSpanSemanticsInformation>();
        var workingText = "";
        var workingLabel = "";
        var workingAttributes = new List<global::Doroti.Ui.StringAttribute>();
        foreach (var info in infoList)
        {
            if (info.requiresOwnNode)
            {
                combined.Add(new InlineSpanSemanticsInformation(workingText, semanticsLabel: workingLabel, stringAttributes: workingAttributes));
                workingText = "";
                workingLabel = "";
                workingAttributes = new List<global::Doroti.Ui.StringAttribute>();
                combined.Add(info);
            }
            else
            {
                workingText += info.text;
                string effectiveLabel = info.semanticsLabel ?? info.text;
                foreach (global::Doroti.Ui.StringAttribute infoAttribute in info.stringAttributes)
                {
                    workingAttributes.Add(infoAttribute.copy(range: new global::Doroti.Ui.TextRange(start: infoAttribute.range.start + workingLabel.Length, end: infoAttribute.range.end + workingLabel.Length)));
                }
                workingLabel += effectiveLabel;
            }
        }
        combined.Add(new InlineSpanSemanticsInformation(workingText, semanticsLabel: workingLabel, stringAttributes: workingAttributes));
        return combined;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public abstract class InlineSpan : DiagnosticableTree
{
    public virtual TextStyle? style { get; private set; }

    protected InlineSpan(TextStyle? style = null)
    {
        this.style = style;
    }

    public abstract void build(ParagraphBuilder builder, TextScaler textScaler = default!, List<PlaceholderDimensions>? dimensions = null);
    public abstract bool visitChildren(Func<InlineSpan, bool> visitor);
    public abstract bool visitDirectChildren(Func<InlineSpan, bool> visitor);
    public virtual InlineSpan? getSpanForPosition(TextPosition position)
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        var offset = new Accumulator();
        InlineSpan? result = default!;
        visitChildren((span) =>
        {
            result = span.getSpanForPositionVisitor(position, offset);
            return result is null;
        });
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract InlineSpan? getSpanForPositionVisitor(TextPosition position, Accumulator offset);
    public virtual string toPlainText(bool includeSemanticsLabels = true, bool includePlaceholders = true)
    {
        var buffer = new StringBuffer();
        computeToPlainText(buffer, includeSemanticsLabels: includeSemanticsLabels, includePlaceholders: includePlaceholders);
        return buffer.ToString();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<InlineSpanSemanticsInformation> getSemanticsInformation()
    {
        var collector = new List<InlineSpanSemanticsInformation>();
        computeSemanticsInformation(collector);
        return collector;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract void computeSemanticsInformation(List<InlineSpanSemanticsInformation> collector, Locale? inheritedLocale = null, bool inheritedSpellOut = false);
    public abstract void computeToPlainText(StringBuffer buffer, bool includeSemanticsLabels = true, bool includePlaceholders = true);
    public virtual long? codeUnitAt(long index)
    {
        if (index < 0L)
        {
            return null;
        }
        var offset = new Accumulator();
        long? result = default!;
        visitChildren((span) =>
        {
            result = span.codeUnitAtVisitor(index, offset);
            return result is null;
        });
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract long? codeUnitAtVisitor(long index, Accumulator offset);
    public virtual bool debugAssertIsValid() => true;
    public abstract RenderComparison compareTo(InlineSpan other);
    public override bool Equals(object? other)
    {
        var __other = other as InlineSpan;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is InlineSpan) && Equals(__other.style, style);
    }

    public override int GetHashCode() => style?.GetHashCode() ?? 0;
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.defaultDiagnosticsTreeStyle = DiagnosticsTreeStyle.whitespace;
        style?.debugFillProperties(properties);
    }

    public virtual string toStringDeep(string prefixLineOne = "", string? prefixOtherLines = null, DiagnosticLevel minLevel = DiagnosticLevel.debug, long? wrapWidth = null) =>
        ((DiagnosticableTree)this).toStringDeep(prefixLineOne, prefixOtherLines, minLevel, wrapWidth);
}

