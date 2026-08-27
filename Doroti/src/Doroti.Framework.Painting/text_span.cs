// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/text_span.dart
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

public class TextSpan : InlineSpan, HitTestTarget
{
    public virtual string? text { get; private set; }
    public virtual List<InlineSpan>? children { get; private set; }
    public virtual GestureRecognizer? recognizer { get; private set; }
    public virtual MouseCursor mouseCursor { get; private set; } = default!;
    public virtual Action<PointerEnterEvent>? onEnter { get; private set; }
    public virtual Action<PointerExitEvent>? onExit { get; private set; }
    public virtual string? semanticsLabel { get; private set; }
    public virtual string? semanticsIdentifier { get; private set; }
    public virtual Locale? locale { get; private set; }
    public virtual bool? spellOut { get; private set; }

    public TextSpan(string? text = null, List<InlineSpan>? children = null, TextStyle? style = null, GestureRecognizer? recognizer = null, MouseCursor? mouseCursor = null, Action<PointerEnterEvent>? onEnter = null, Action<PointerExitEvent>? onExit = null, string? semanticsLabel = null, string? semanticsIdentifier = null, Locale? locale = null, bool? spellOut = null) : base(style: style)
    {
        this.text = text;
        this.children = children;
        this.recognizer = recognizer;
        this.onEnter = onEnter;
        this.onExit = onExit;
        this.semanticsLabel = semanticsLabel;
        this.semanticsIdentifier = semanticsIdentifier;
        this.locale = locale;
        this.spellOut = spellOut;
        this.mouseCursor = (mouseCursor ?? (((recognizer is null) ? MouseCursor.defer : SystemMouseCursors.click)));
        System.Diagnostics.Debug.Assert(!(((text is null) && (semanticsLabel is not null))));
    }

    public virtual MouseCursor cursor => this.mouseCursor;
    public virtual bool validForMouseTracker => true;
    public virtual void handleEvent(PointerEvent @event, HitTestEntry<HitTestTarget> entry)
    {
        if ((@event is PointerDownEvent))
        {
            PointerDownEvent @event__as9792 = (PointerDownEvent)@event;
            this.recognizer?.addPointer((global::Doroti.Framework.Gestures.PointerDownEvent)(object)@event__as9792);
        }
    }

    public override void build(ParagraphBuilder builder, TextScaler textScaler = default!, List<PlaceholderDimensions>? dimensions = null)
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        var hasStyle = (style is not null);
        if (hasStyle)
        {
            builder.pushStyle(style!.getTextStyle(textScaler: textScaler));
        }
        if ((this.text is not null))
        {
            try
            {
                builder.addText(this.text!);
            }
            catch (DartArgumentError exceptionLocal)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new FlutterErrorDetails(exception: exceptionLocal, stack: stackLocal, library: "painting library", context: new ErrorDescription("while building a TextSpan"), silent: true));
                builder.addText("�");
            }
        }
        List<InlineSpan>? childrenLocal = this.children;
        if ((childrenLocal is not null))
        {
            foreach (InlineSpan child in childrenLocal)
            {
                child.build(builder, textScaler: textScaler, dimensions: dimensions);
            }
        }
        if (hasStyle)
        {
            builder.pop();
        }
    }

    public override bool visitChildren(Func<InlineSpan, bool> visitor)
    {
        if (((this.text is not null) && !visitor(this)))
        {
            return false;
        }
        List<InlineSpan>? childrenLocal = this.children;
        if ((childrenLocal is not null))
        {
            foreach (InlineSpan child in childrenLocal)
            {
                if (!child.visitChildren((Func<InlineSpan, bool>)visitor))
                {
                    return false;
                }
            }
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool visitDirectChildren(Func<InlineSpan, bool> visitor)
    {
        List<InlineSpan>? childrenLocal = this.children;
        if ((childrenLocal is not null))
        {
            foreach (InlineSpan child in childrenLocal)
            {
                if (!visitor(child))
                {
                    return false;
                }
            }
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override InlineSpan? getSpanForPositionVisitor(TextPosition position, Accumulator offset)
    {
        string? textLocal = this.text;
        if (((textLocal is null) || (textLocal.Length == 0)))
        {
            return null;
        }
        global::Doroti.Ui.TextAffinity affinityLocal = position.affinity;
        long targetOffset = position.offset;
        long endOffset = (((Accumulator)offset).value + textLocal.Length);
        if (((((((Accumulator)offset).value == targetOffset) && (object.Equals(affinityLocal, TextAffinity.downstream))) || ((((Accumulator)offset).value < targetOffset) && (targetOffset < endOffset))) || ((endOffset == targetOffset) && (object.Equals(affinityLocal, TextAffinity.upstream)))))
        {
            return this;
        }
        offset.increment(textLocal.Length);
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void computeToPlainText(StringBuffer buffer, bool includeSemanticsLabels = true, bool includePlaceholders = true)
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        if (((this.semanticsLabel is not null) && includeSemanticsLabels))
        {
            buffer.write(this.semanticsLabel);
        }
        else
        {
            if ((this.text is not null))
            {
                buffer.write(this.text);
            }
        }
        if ((this.children is not null))
        {
            foreach (InlineSpan child in this.children!)
            {
                child.computeToPlainText(buffer, includeSemanticsLabels: includeSemanticsLabels, includePlaceholders: includePlaceholders);
            }
        }
    }

    public override void computeSemanticsInformation(List<InlineSpanSemanticsInformation> collector, Locale? inheritedLocale = null, bool inheritedSpellOut = false)
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        global::Doroti.Ui.Locale? effectiveLocale = (this.locale ?? inheritedLocale);
        bool effectiveSpellOut = (this.spellOut ?? inheritedSpellOut);
        if ((this.text is not null))
        {
            long textLength = (this.semanticsLabel?.Length ?? this.text!.Length);
            collector.Add(new InlineSpanSemanticsInformation(this.text!, stringAttributes: new List<global::Doroti.Ui.StringAttribute>(), semanticsLabel: this.semanticsLabel, semanticsIdentifier: this.semanticsIdentifier, recognizer: this.recognizer));
        }
        List<InlineSpan>? childrenLocal = this.children;
        if ((childrenLocal is not null))
        {
            foreach (InlineSpan child in childrenLocal)
            {
                if ((child is TextSpan))
                {
                    TextSpan child__14821__as14854 = (TextSpan)child;
                    ((TextSpan)child__14821__as14854).computeSemanticsInformation(collector, inheritedLocale: effectiveLocale, inheritedSpellOut: effectiveSpellOut);
                }
                else
                {
                    child.computeSemanticsInformation(collector);
                }
            }
        }
    }

    public override long? codeUnitAtVisitor(long index, Accumulator offset)
    {
        string? textLocal = this.text;
        if ((textLocal is null))
        {
            return null;
        }
        long localOffset = (index - ((Accumulator)offset).value);
        DartRuntimePrimitives.Assert(() => (localOffset >= 0L));
        offset.increment(textLocal.Length);
        return ((localOffset < textLocal.Length) ? textLocal.codeUnitAt(localOffset) : null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool debugAssertIsValid()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this.children is not null))
                {
                    foreach (InlineSpan child in this.children!)
                    {
                        DartRuntimePrimitives.Assert(() => child.debugAssertIsValid());
                    }
                }
                return true;
            });
        return base.debugAssertIsValid();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override RenderComparison compareTo(InlineSpan other)
    {
        if (DartRuntimePrimitives.Identical(this, other))
        {
            return RenderComparison.identical;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(other), this.GetType())))
        {
            return RenderComparison.layout;
        }
        var textSpan = ((TextSpan?)(object?)other)!;
        if ((((((TextSpan)textSpan).text != this.text) || (((long?)(this.children?.Count)) != ((long?)(((TextSpan)textSpan).children?.Count)))) || (((style is null)) != ((textSpan.style is null)))))
        {
            return RenderComparison.layout;
        }
        RenderComparison result = ((object.Equals(this.recognizer, ((TextSpan)textSpan).recognizer)) ? RenderComparison.identical : RenderComparison.metadata);
        if ((style is not null))
        {
            RenderComparison candidate = style!.compareTo(textSpan.style!);
            if ((FoundationRuntimePorts.EnumIndex(candidate) > FoundationRuntimePorts.EnumIndex(result)))
            {
                result = candidate;
            }
            if ((object.Equals(result, RenderComparison.layout)))
            {
                return result;
            }
        }
        if ((this.children is not null))
        {
            for (var index = 0L; (index < checked((long)(this.children!.Count))); index += 1L)
            {
                RenderComparison candidateLocal = this.children![(int)(index)].compareTo(((TextSpan)textSpan).children![(int)(index)]);
                if ((FoundationRuntimePorts.EnumIndex(candidateLocal) > FoundationRuntimePorts.EnumIndex(result)))
                {
                    result = candidateLocal;
                }
                if ((object.Equals(result, RenderComparison.layout)))
                {
                    return result;
                }
            }
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as TextSpan;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        if (!base.Equals(__other))
        {
            return false;
        }
        return (((((((((__other is TextSpan) && (((TextSpan)((TextSpan)__other)).text == this.text)) && (object.Equals(((TextSpan)((TextSpan)__other)).recognizer, this.recognizer))) && (((TextSpan)((TextSpan)__other)).semanticsLabel == this.semanticsLabel)) && (((TextSpan)((TextSpan)__other)).semanticsIdentifier == this.semanticsIdentifier)) && (object.Equals((Action<PointerEnterEvent>?)this.onEnter, (Action<PointerEnterEvent>?)((TextSpan)((TextSpan)__other)).onEnter))) && (object.Equals((Action<PointerExitEvent>?)this.onExit, (Action<PointerExitEvent>?)((TextSpan)((TextSpan)__other)).onExit))) && (object.Equals(this.mouseCursor, ((TextSpan)((TextSpan)__other)).mouseCursor))) && global::Doroti.Framework.Foundation.CollectionsLibrary.listEquals<InlineSpan>(((TextSpan)((TextSpan)__other)).children, this.children));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(base.GetHashCode(), this.text, this.recognizer, this.semanticsLabel, this.semanticsIdentifier, this.onEnter, this.onExit, this.mouseCursor, ((this.children is null) ? null : FoundationRuntimePorts.ObjectHashAll(this.children!)));
    public virtual string toStringShort() => global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "TextSpan");
    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new StringProperty("text", this.text, showName: false, defaultValue: null));
        if ((((style is null) && (this.text is null)) && (this.children is null)))
        {
            properties.add(new DiagnosticsNode("(empty)"));
        }
        properties.add(new DiagnosticsProperty<GestureRecognizer>("recognizer", this.recognizer, description: DartRuntimePrimitives.RuntimeTypeName(this.recognizer), defaultValue: null));
        properties.add(new FlagsSummary<Delegate?>("callbacks", new DartMap<string, Delegate?> { ["enter"] = this.onEnter, ["exit"] = this.onExit }));
        properties.add(new DiagnosticsProperty<MouseCursor>("mouseCursor", this.cursor, defaultValue: MouseCursor.defer));
        if ((this.semanticsLabel is not null))
        {
            properties.add(new StringProperty("semanticsLabel", this.semanticsLabel));
        }
        if ((this.semanticsIdentifier is not null))
        {
            properties.add(new StringProperty("semanticsIdentifier", this.semanticsIdentifier));
        }
    }

    public virtual List<DiagnosticsNode> debugDescribeChildren()
    {
        return (this.children?.map<InlineSpan, DiagnosticsNode>(((child) =>
        {
            return ((Diagnosticable)child).toDiagnosticsNode();
            return default;
        })).ToList() ?? new List<DiagnosticsNode>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
