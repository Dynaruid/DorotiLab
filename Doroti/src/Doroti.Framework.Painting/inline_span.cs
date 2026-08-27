// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/inline_span.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Painting;

public class Accumulator
{
    internal virtual long _value { get; set; } = default!;

    public Accumulator(long _value = 0)
    {
        this._value = _value;
    }

    public virtual long value => this._value;
    public virtual void increment(long addend)
    {
        DartRuntimePrimitives.Assert(() => (addend >= 0L));
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
        this.requiresOwnNode = ((isPlaceholder || (recognizer is not null)) || (semanticsIdentifier is not null));
        System.Diagnostics.Debug.Assert((!isPlaceholder || ((((text == "￼") && (semanticsLabel is null)) && (recognizer is null)))));
    }

    public override bool Equals(object? other)
    {
        var __other = other as InlineSpanSemanticsInformation;
        if (__other is null) return false;
        return (((((((__other is InlineSpanSemanticsInformation) && (((InlineSpanSemanticsInformation)((InlineSpanSemanticsInformation)__other)).text == this.text)) && (((InlineSpanSemanticsInformation)((InlineSpanSemanticsInformation)__other)).semanticsLabel == this.semanticsLabel)) && (((InlineSpanSemanticsInformation)((InlineSpanSemanticsInformation)__other)).semanticsIdentifier == this.semanticsIdentifier)) && (object.Equals(((InlineSpanSemanticsInformation)((InlineSpanSemanticsInformation)__other)).recognizer, this.recognizer))) && (((InlineSpanSemanticsInformation)((InlineSpanSemanticsInformation)__other)).isPlaceholder == this.isPlaceholder)) && global::Doroti.Framework.Foundation.CollectionsLibrary.listEquals<global::Doroti.Ui.StringAttribute>(((InlineSpanSemanticsInformation)((InlineSpanSemanticsInformation)__other)).stringAttributes, this.stringAttributes));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.text, this.semanticsLabel, this.semanticsIdentifier, this.recognizer, this.isPlaceholder);
    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "InlineSpanSemanticsInformation"))}{{text: {this.text}, semanticsLabel: {this.semanticsLabel}, semanticsIdentifier: {this.semanticsIdentifier}, recognizer: {this.recognizer}}}";
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
            if (((InlineSpanSemanticsInformation)info).requiresOwnNode)
            {
                combined.Add(new InlineSpanSemanticsInformation(workingText, semanticsLabel: workingLabel, stringAttributes: workingAttributes));
                workingText = "";
                workingLabel = "";
                workingAttributes = new List<global::Doroti.Ui.StringAttribute>();
                combined.Add(info);
            }
            else
            {
                workingText += ((InlineSpanSemanticsInformation)info).text;
                string effectiveLabel = (((InlineSpanSemanticsInformation)info).semanticsLabel ?? ((InlineSpanSemanticsInformation)info).text);
                foreach (global::Doroti.Ui.StringAttribute infoAttribute in ((InlineSpanSemanticsInformation)info).stringAttributes)
                {
                    workingAttributes.Add(infoAttribute.copy(range: new global::Doroti.Ui.TextRange(start: (infoAttribute.range.start + workingLabel.Length), end: (infoAttribute.range.end + workingLabel.Length))));
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
        visitChildren(((Func<InlineSpan, bool>)((span) =>
        {
            result = span.getSpanForPositionVisitor(position, offset);
            return (result is null);
            return default;
        })));
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
        if ((index < 0L))
        {
            return null;
        }
        var offset = new Accumulator();
        long? result = default!;
        visitChildren(((Func<InlineSpan, bool>)((span) =>
        {
            result = span.codeUnitAtVisitor(index, offset);
            return (result is null);
            return default;
        })));
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
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((__other is InlineSpan) && (object.Equals(((InlineSpan)((InlineSpan)__other)).style, this.style)));
    }

    public override int GetHashCode() => this.style.GetHashCode();
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.defaultDiagnosticsTreeStyle = DiagnosticsTreeStyle.whitespace;
        this.style?.debugFillProperties(properties);
    }

    public virtual string toStringDeep(string prefixLineOne = "", string? prefixOtherLines = null, DiagnosticLevel minLevel = DiagnosticLevel.debug, long? wrapWidth = null) =>
        ((DiagnosticableTree)this).toStringDeep(prefixLineOne, prefixOtherLines, minLevel, wrapWidth);
}

