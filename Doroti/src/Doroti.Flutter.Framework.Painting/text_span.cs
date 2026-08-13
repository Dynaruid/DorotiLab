// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/text_span.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Painting;

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
            this.recognizer?.addPointer((global::Doroti.Generated.Framework.Gestures.PointerDownEvent)(object)@event__as9792);
        }
    }

    public override void build(ParagraphBuilder builder, TextScaler textScaler = default!, List<PlaceholderDimensions>? dimensions = null)
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        var hasStyle__10432 = (style is not null);
        if (hasStyle__10432)
        {
            builder.pushStyle(style!.getTextStyle(textScaler: textScaler));
        }
        if ((this.text is not null))
        {
            try
            {
                builder.addText(this.text!);
            }
            catch (DartArgumentError exception__10654)
            {
                var stack__10665 = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new FlutterErrorDetails(exception: exception__10654, stack: stack__10665, library: "painting library", context: new ErrorDescription("while building a TextSpan"), silent: true));
                builder.addText("�");
            }
        }
        List<InlineSpan>? children__11116 = this.children;
        if ((children__11116 is not null))
        {
            foreach (InlineSpan child__11198 in children__11116)
            {
                child__11198.build(builder, textScaler: textScaler, dimensions: dimensions);
            }
        }
        if (hasStyle__10432)
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
        List<InlineSpan>? children__11762 = this.children;
        if ((children__11762 is not null))
        {
            foreach (InlineSpan child__11844 in children__11762)
            {
                if (!child__11844.visitChildren((Func<InlineSpan, bool>)visitor))
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
        List<InlineSpan>? children__12076 = this.children;
        if ((children__12076 is not null))
        {
            foreach (InlineSpan child__12158 in children__12076)
            {
                if (!visitor(child__12158))
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
        string? text__12469 = this.text;
        if (((text__12469 is null) || (text__12469.Length == 0)))
        {
            return null;
        }
        global::Doroti.Flutter.Ui.TextAffinity affinity__12575 = position.affinity;
        long targetOffset__12619 = position.offset;
        long endOffset__12665 = (((Accumulator)offset).value + text__12469.Length);
        if (((((((Accumulator)offset).value == targetOffset__12619) && (object.Equals(affinity__12575, TextAffinity.downstream))) || ((((Accumulator)offset).value < targetOffset__12619) && (targetOffset__12619 < endOffset__12665))) || ((endOffset__12665 == targetOffset__12619) && (object.Equals(affinity__12575, TextAffinity.upstream)))))
        {
            return this;
        }
        offset.increment(text__12469.Length);
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
            foreach (InlineSpan child__13407 in this.children!)
            {
                child__13407.computeToPlainText(buffer, includeSemanticsLabels: includeSemanticsLabels, includePlaceholders: includePlaceholders);
            }
        }
    }

    public override void computeSemanticsInformation(List<InlineSpanSemanticsInformation> collector, Locale? inheritedLocale = null, bool inheritedSpellOut = false)
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        global::Doroti.Flutter.Ui.Locale? effectiveLocale__13853 = (this.locale ?? inheritedLocale);
        bool effectiveSpellOut__13913 = (this.spellOut ?? inheritedSpellOut);
        if ((this.text is not null))
        {
            long textLength__14005 = (this.semanticsLabel?.Length ?? this.text!.Length);
            collector.Add(new InlineSpanSemanticsInformation(this.text!, stringAttributes: new List<global::Doroti.Flutter.Ui.StringAttribute>(), semanticsLabel: this.semanticsLabel, semanticsIdentifier: this.semanticsIdentifier, recognizer: this.recognizer));
        }
        List<InlineSpan>? children__14739 = this.children;
        if ((children__14739 is not null))
        {
            foreach (InlineSpan child__14821 in children__14739)
            {
                if ((child__14821 is TextSpan))
                {
                    TextSpan child__14821__as14854 = (TextSpan)child__14821;
                    ((TextSpan)child__14821__as14854).computeSemanticsInformation(collector, inheritedLocale: effectiveLocale__13853, inheritedSpellOut: effectiveSpellOut__13913);
                }
                else
                {
                    child__14821.computeSemanticsInformation(collector);
                }
            }
        }
    }

    public override long? codeUnitAtVisitor(long index, Accumulator offset)
    {
        string? text__15242 = this.text;
        if ((text__15242 is null))
        {
            return null;
        }
        long localOffset__15323 = (index - ((Accumulator)offset).value);
        DartRuntimePrimitives.Assert(() => (localOffset__15323 >= 0L));
        offset.increment(text__15242.Length);
        return ((localOffset__15323 < text__15242.Length) ? text__15242.codeUnitAt(localOffset__15323) : null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool debugAssertIsValid()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this.children is not null))
                {
                    foreach (InlineSpan child__15871 in this.children!)
                    {
                        DartRuntimePrimitives.Assert(() => child__15871.debugAssertIsValid());
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
        var textSpan__16270 = ((TextSpan?)(object?)other)!;
        if ((((((TextSpan)textSpan__16270).text != this.text) || (((long?)(this.children?.Count)) != ((long?)(((TextSpan)textSpan__16270).children?.Count)))) || (((style is null)) != ((textSpan__16270.style is null)))))
        {
            return RenderComparison.layout;
        }
        RenderComparison result__16510 = ((object.Equals(this.recognizer, ((TextSpan)textSpan__16270).recognizer)) ? RenderComparison.identical : RenderComparison.metadata);
        if ((style is not null))
        {
            RenderComparison candidate__16681 = style!.compareTo(textSpan__16270.style!);
            if ((FoundationRuntimePorts.EnumIndex(candidate__16681) > FoundationRuntimePorts.EnumIndex(result__16510)))
            {
                result__16510 = candidate__16681;
            }
            if ((object.Equals(result__16510, RenderComparison.layout)))
            {
                return result__16510;
            }
        }
        if ((this.children is not null))
        {
            for (var index__16935 = 0L; (index__16935 < checked((long)(this.children!.Count))); index__16935 += 1L)
            {
                RenderComparison candidate__17017 = this.children![(int)(index__16935)].compareTo(((TextSpan)textSpan__16270).children![(int)(index__16935)]);
                if ((FoundationRuntimePorts.EnumIndex(candidate__17017) > FoundationRuntimePorts.EnumIndex(result__16510)))
                {
                    result__16510 = candidate__17017;
                }
                if ((object.Equals(result__16510, RenderComparison.layout)))
                {
                    return result__16510;
                }
            }
        }
        return result__16510;
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
        return (((((((((__other is TextSpan) && (((TextSpan)((TextSpan)__other)).text == this.text)) && (object.Equals(((TextSpan)((TextSpan)__other)).recognizer, this.recognizer))) && (((TextSpan)((TextSpan)__other)).semanticsLabel == this.semanticsLabel)) && (((TextSpan)((TextSpan)__other)).semanticsIdentifier == this.semanticsIdentifier)) && (object.Equals((Action<PointerEnterEvent>?)this.onEnter, (Action<PointerEnterEvent>?)((TextSpan)((TextSpan)__other)).onEnter))) && (object.Equals((Action<PointerExitEvent>?)this.onExit, (Action<PointerExitEvent>?)((TextSpan)((TextSpan)__other)).onExit))) && (object.Equals(this.mouseCursor, ((TextSpan)((TextSpan)__other)).mouseCursor))) && global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.listEquals<InlineSpan>(((TextSpan)((TextSpan)__other)).children, this.children));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(base.GetHashCode(), this.text, this.recognizer, this.semanticsLabel, this.semanticsIdentifier, this.onEnter, this.onExit, this.mouseCursor, ((this.children is null) ? null : FoundationRuntimePorts.ObjectHashAll(this.children!)));
    public virtual string toStringShort() => global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "TextSpan");
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
