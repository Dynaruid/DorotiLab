// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/text_span.dart
using Doroti.Runtime;
using Doroti.Ui;

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

    public TextSpan(
        string? text = null,
        List<InlineSpan>? children = null,
        TextStyle? style = null,
        GestureRecognizer? recognizer = null,
        MouseCursor? mouseCursor = null,
        Action<PointerEnterEvent>? onEnter = null,
        Action<PointerExitEvent>? onExit = null,
        string? semanticsLabel = null,
        string? semanticsIdentifier = null,
        Locale? locale = null,
        bool? spellOut = null
    )
        : base(style: style)
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
        this.mouseCursor =
            mouseCursor ?? ((recognizer is null) ? MouseCursor.defer : SystemMouseCursors.click);
        System.Diagnostics.Debug.Assert(!((text is null) && (semanticsLabel is not null)));
    }

    public virtual MouseCursor cursor => mouseCursor;
    public virtual bool validForMouseTracker => true;

    public virtual void handleEvent(PointerEvent @event, HitTestEntry<HitTestTarget> entry)
    {
        if (@event is PointerDownEvent)
        {
            PointerDownEvent @event__as9792 = (PointerDownEvent)@event;
            recognizer?.addPointer((PointerDownEvent)(object)@event__as9792);
        }
    }

    public override void build(
        ParagraphBuilder builder,
        TextScaler textScaler = default!,
        List<PlaceholderDimensions>? dimensions = null
    )
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        var hasStyle = style is not null;
        if (hasStyle)
        {
            builder.pushStyle(style!.getTextStyle(textScaler: textScaler));
        }
        if (text is not null)
        {
            try
            {
                builder.addText(text!);
            }
            catch (DartArgumentError exceptionLocal)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                FlutterError.reportError(
                    new FlutterErrorDetails(
                        exception: exceptionLocal,
                        stack: stackLocal,
                        library: "painting library",
                        context: new ErrorDescription("while building a TextSpan"),
                        silent: true
                    )
                );
                builder.addText("�");
            }
        }
        List<InlineSpan>? childrenLocal = children;
        if (childrenLocal is not null)
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
        if ((text is not null) && !visitor(this))
        {
            return false;
        }
        List<InlineSpan>? childrenLocal = children;
        if (childrenLocal is not null)
        {
            foreach (InlineSpan child in childrenLocal)
            {
                if (!child.visitChildren(visitor))
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
        List<InlineSpan>? childrenLocal = children;
        if (childrenLocal is not null)
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
        string? textLocal = text;
        if ((textLocal is null) || (textLocal.Length == 0))
        {
            return null;
        }
        TextAffinity affinityLocal = position.affinity;
        long targetOffset = position.offset;
        long endOffset = offset.value + textLocal.Length;
        if (
            ((offset.value == targetOffset) && Equals(affinityLocal, TextAffinity.downstream))
            || ((offset.value < targetOffset) && (targetOffset < endOffset))
            || ((endOffset == targetOffset) && Equals(affinityLocal, TextAffinity.upstream))
        )
        {
            return this;
        }
        offset.increment(textLocal.Length);
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void computeToPlainText(
        StringBuffer buffer,
        bool includeSemanticsLabels = true,
        bool includePlaceholders = true
    )
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        if ((semanticsLabel is not null) && includeSemanticsLabels)
        {
            buffer.write(semanticsLabel);
        }
        else
        {
            if (text is not null)
            {
                buffer.write(text);
            }
        }
        if (children is not null)
        {
            foreach (InlineSpan child in children!)
            {
                child.computeToPlainText(
                    buffer,
                    includeSemanticsLabels: includeSemanticsLabels,
                    includePlaceholders: includePlaceholders
                );
            }
        }
    }

    public override void computeSemanticsInformation(
        List<InlineSpanSemanticsInformation> collector,
        Locale? inheritedLocale = null,
        bool inheritedSpellOut = false
    )
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        Locale? effectiveLocale = locale ?? inheritedLocale;
        bool effectiveSpellOut = spellOut ?? inheritedSpellOut;
        if (text is not null)
        {
            long textLength = semanticsLabel?.Length ?? text!.Length;
            collector.Add(
                new InlineSpanSemanticsInformation(
                    text!,
                    stringAttributes: new List<StringAttribute>(),
                    semanticsLabel: semanticsLabel,
                    semanticsIdentifier: semanticsIdentifier,
                    recognizer: recognizer
                )
            );
        }
        List<InlineSpan>? childrenLocal = children;
        if (childrenLocal is not null)
        {
            foreach (InlineSpan child in childrenLocal)
            {
                if (child is TextSpan)
                {
                    TextSpan child__14821__as14854 = (TextSpan)child;
                    child__14821__as14854.computeSemanticsInformation(
                        collector,
                        inheritedLocale: effectiveLocale,
                        inheritedSpellOut: effectiveSpellOut
                    );
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
        string? textLocal = text;
        if (textLocal is null)
        {
            return null;
        }
        long localOffset = index - offset.value;
        DartRuntimePrimitives.Assert(() => localOffset >= 0L);
        offset.increment(textLocal.Length);
        return (localOffset < textLocal.Length) ? textLocal.codeUnitAt(localOffset) : null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool debugAssertIsValid()
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (children is not null)
            {
                foreach (InlineSpan child in children!)
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
        if (!Equals(DartRuntimePrimitives.RuntimeType(other), GetType()))
        {
            return RenderComparison.layout;
        }
        var textSpan = ((TextSpan?)(object?)other)!;
        if (
            (textSpan.text != text)
            || ((children?.Count) != ((long?)(textSpan.children?.Count)))
            || ((style is null) != (textSpan.style is null))
        )
        {
            return RenderComparison.layout;
        }
        RenderComparison result = Equals(recognizer, textSpan.recognizer)
            ? RenderComparison.identical
            : RenderComparison.metadata;
        if (style is not null)
        {
            RenderComparison candidate = style!.compareTo(textSpan.style!);
            if (
                FoundationRuntimePorts.EnumIndex(candidate)
                > FoundationRuntimePorts.EnumIndex(result)
            )
            {
                result = candidate;
            }
            if (Equals(result, RenderComparison.layout))
            {
                return result;
            }
        }
        if (children is not null)
        {
            for (var index = 0L; index < checked(children!.Count); index += 1L)
            {
                RenderComparison candidateLocal = children!
                    [(int)index]
                    .compareTo(textSpan.children![(int)index]);
                if (
                    FoundationRuntimePorts.EnumIndex(candidateLocal)
                    > FoundationRuntimePorts.EnumIndex(result)
                )
                {
                    result = candidateLocal;
                }
                if (Equals(result, RenderComparison.layout))
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
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        if (!base.Equals(__other))
        {
            return false;
        }
        return (__other is TextSpan)
            && (__other.text == text)
            && Equals(__other.recognizer, recognizer)
            && (__other.semanticsLabel == semanticsLabel)
            && (__other.semanticsIdentifier == semanticsIdentifier)
            && Equals(onEnter, __other.onEnter)
            && Equals(onExit, __other.onExit)
            && Equals(mouseCursor, __other.mouseCursor)
            && CollectionsLibrary.listEquals(__other.children, children);
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(
            base.GetHashCode(),
            text,
            recognizer,
            semanticsLabel,
            semanticsIdentifier,
            onEnter,
            onExit,
            mouseCursor,
            (children is null) ? null : FoundationRuntimePorts.ObjectHashAll(children!)
        );

    public virtual string toStringShort() =>
        objectRuntimeTypeFunctions.objectRuntimeType(this, "TextSpan");

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new StringProperty("text", text, showName: false, defaultValue: null));
        if ((style is null) && (text is null) && (children is null))
        {
            properties.add(new DiagnosticsNode("(empty)"));
        }
        properties.add(
            new DiagnosticsProperty<GestureRecognizer>(
                "recognizer",
                recognizer,
                description: DartRuntimePrimitives.RuntimeTypeName(recognizer),
                defaultValue: null
            )
        );
        properties.add(
            new FlagsSummary<Delegate?>(
                "callbacks",
                new DartMap<string, Delegate?> { ["enter"] = onEnter, ["exit"] = onExit }
            )
        );
        properties.add(
            new DiagnosticsProperty<MouseCursor>(
                "mouseCursor",
                cursor,
                defaultValue: MouseCursor.defer
            )
        );
        if (semanticsLabel is not null)
        {
            properties.add(new StringProperty("semanticsLabel", semanticsLabel));
        }
        if (semanticsIdentifier is not null)
        {
            properties.add(new StringProperty("semanticsIdentifier", semanticsIdentifier));
        }
    }

    public virtual List<DiagnosticsNode> debugDescribeChildren()
    {
        return children
                ?.map(
                    (child) =>
                    {
                        return ((Diagnosticable)child).toDiagnosticsNode();
                    }
                )
                .ToList()
            ?? new List<DiagnosticsNode>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
