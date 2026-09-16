// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/text.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class DefaultTextStyle : InheritedTheme
{
    public virtual TextStyle style { get; private set; } = default!;
    public virtual TextAlign? textAlign { get; private set; }
    public virtual bool softWrap { get; private set; } = default!;
    public virtual TextOverflow overflow { get; private set; } = default!;
    public virtual long? maxLines { get; private set; }
    public virtual TextWidthBasis textWidthBasis { get; private set; } = default!;
    public virtual TextHeightBehavior? textHeightBehavior { get; private set; }

    public DefaultTextStyle(Key? key = null, TextStyle style = default!, TextAlign? textAlign = null, bool softWrap = true, TextOverflow overflow = TextOverflow.clip, long? maxLines = null, TextWidthBasis textWidthBasis = TextWidthBasis.parent, TextHeightBehavior? textHeightBehavior = null, Widget child = default!) : base(key: key, child: child)
    {
        this.style = style;
        this.textAlign = textAlign;
        this.softWrap = softWrap;
        this.overflow = overflow;
        this.maxLines = maxLines;
        this.textWidthBasis = textWidthBasis;
        this.textHeightBehavior = textHeightBehavior;
        System.Diagnostics.Debug.Assert((maxLines is null) || (DartRuntimePrimitives.RequireValue(maxLines) > 0L));
    }

    public static DefaultTextStyle CreateFallback(Key? key = null)
    {
        var __instance = new DefaultTextStyle(key, default!, default!, default!, default!, default!, default!, default!, default!);
        __instance.style = new TextStyle();
        __instance.textAlign = null;
        __instance.softWrap = true;
        __instance.maxLines = null;
        __instance.overflow = TextOverflow.clip;
        __instance.textWidthBasis = TextWidthBasis.parent;
        __instance.textHeightBehavior = null;
        return __instance;
    }

    public static Widget merge(Key? key = null, TextStyle? style = null, TextAlign? textAlign = null, bool? softWrap = null, TextOverflow? overflow = null, long? maxLines = null, TextWidthBasis? textWidthBasis = null, TextHeightBehavior? textHeightBehavior = null, Widget child = default!)
    {
        return new Builder(builder: (context) =>
        {
            DefaultTextStyle parent = of(context);
            return new DefaultTextStyle(key: key, style: parent.style.merge(style), textAlign: textAlign ?? parent.textAlign, softWrap: softWrap ?? parent.softWrap, overflow: overflow ?? parent.overflow, maxLines: maxLines ?? parent.maxLines, textWidthBasis: textWidthBasis ?? parent.textWidthBasis, textHeightBehavior: textHeightBehavior ?? parent.textHeightBehavior, child: child);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DefaultTextStyle of(BuildContext context)
    {
        return context.dependOnInheritedWidgetOfExactType<DefaultTextStyle>() ?? CreateFallback();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (DefaultTextStyle)oldWidget;
        return (!Equals(style, __oldWidget.style)) || (!Equals(textAlign, __oldWidget.textAlign)) || (softWrap != __oldWidget.softWrap) || (!Equals(overflow, __oldWidget.overflow)) || (maxLines != __oldWidget.maxLines) || (!Equals(textWidthBasis, __oldWidget.textWidthBasis)) || (!Equals(textHeightBehavior, __oldWidget.textHeightBehavior));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return new DefaultTextStyle(style: style, textAlign: textAlign, softWrap: DartRuntimePrimitives.RequireValue(softWrap), overflow: DartRuntimePrimitives.RequireValue(overflow), maxLines: maxLines, textWidthBasis: DartRuntimePrimitives.RequireValue(textWidthBasis), textHeightBehavior: textHeightBehavior, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        style.debugFillProperties(properties);
        properties.add(new EnumProperty<TextAlign>("textAlign", textAlign, defaultValue: null));
        properties.add(new FlagProperty("softWrap", value: softWrap, ifTrue: "wrapping at box width", ifFalse: "no wrapping except at line break characters", showName: true));
        properties.add(new EnumProperty<TextOverflow>("overflow", overflow, defaultValue: null));
        properties.add(new IntProperty("maxLines", maxLines, defaultValue: null));
        properties.add(new EnumProperty<TextWidthBasis>("textWidthBasis", textWidthBasis, defaultValue: TextWidthBasis.parent));
        properties.add(new DiagnosticsProperty<TextHeightBehavior>("textHeightBehavior", textHeightBehavior, defaultValue: null));
    }

}

internal class _NullWidget__text : StatelessWidget
{
    internal _NullWidget__text()
    {
    }

    public override Widget build(BuildContext context)
    {
        throw DartRuntimePrimitives.AsException(FlutterError.Create("A DefaultTextStyle constructed with DefaultTextStyle.fallback cannot be incorporated into the widget tree, " + "it is meant only to provide a fallback value returned by DefaultTextStyle.of() " + "when no enclosing default text style is present in a BuildContext."));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DefaultTextHeightBehavior : InheritedTheme
{
    public virtual TextHeightBehavior textHeightBehavior { get; private set; } = default!;

    public DefaultTextHeightBehavior(Key? key = null, TextHeightBehavior textHeightBehavior = default!, Widget child = default!) : base(key: key, child: child)
    {
        this.textHeightBehavior = textHeightBehavior;
    }

    public static TextHeightBehavior? maybeOf(BuildContext context)
    {
        return context.dependOnInheritedWidgetOfExactType<DefaultTextHeightBehavior>()?.textHeightBehavior;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TextHeightBehavior of(BuildContext context)
    {
        TextHeightBehavior? behavior = maybeOf(context);
        DartRuntimePrimitives.Assert(() =>
            {
                if (behavior is null)
                {
                    throw DartRuntimePrimitives.AsException(FlutterError.Create("DefaultTextHeightBehavior.of() was called with a context that does not contain a " + "DefaultTextHeightBehavior widget.\n" + "No DefaultTextHeightBehavior widget ancestor could be found starting from the " + "context that was passed to DefaultTextHeightBehavior.of(). This can happen " + "because you are using a widget that looks for a DefaultTextHeightBehavior " + "ancestor, but no such ancestor exists.\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return behavior!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (DefaultTextHeightBehavior)oldWidget;
        return !Equals(textHeightBehavior, __oldWidget.textHeightBehavior);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return new DefaultTextHeightBehavior(textHeightBehavior: textHeightBehavior, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<TextHeightBehavior>("textHeightBehavior", textHeightBehavior, defaultValue: null));
    }

}

public class Text : StatelessWidget
{
    public virtual string? data { get; private set; }
    public virtual InlineSpan? textSpan { get; private set; }
    public virtual TextStyle? style { get; private set; }
    public virtual Painting.StrutStyle? strutStyle { get; private set; }
    public virtual TextAlign? textAlign { get; private set; }
    public virtual TextDirection? textDirection { get; private set; }
    public virtual Locale? locale { get; private set; }
    public virtual bool? softWrap { get; private set; }
    public virtual TextOverflow? overflow { get; private set; }
    public virtual double? textScaleFactor { get; private set; }
    public virtual TextScaler? textScaler { get; private set; }
    public virtual long? maxLines { get; private set; }
    public virtual string? semanticsLabel { get; private set; }
    public virtual string? semanticsIdentifier { get; private set; }
    public virtual TextWidthBasis? textWidthBasis { get; private set; }
    public virtual TextHeightBehavior? textHeightBehavior { get; private set; }
    public virtual Color? selectionColor { get; private set; }

    public Text(string data, Key? key = null, TextStyle? style = null, Painting.StrutStyle? strutStyle = null, TextAlign? textAlign = null, TextDirection? textDirection = null, Locale? locale = null, bool? softWrap = null, TextOverflow? overflow = null, double? textScaleFactor = null, TextScaler? textScaler = null, long? maxLines = null, string? semanticsLabel = null, string? semanticsIdentifier = null, TextWidthBasis? textWidthBasis = null, TextHeightBehavior? textHeightBehavior = null, Color? selectionColor = null) : base(key: key)
    {
        this.data = data;
        this.style = style;
        this.strutStyle = strutStyle;
        this.textAlign = textAlign;
        this.textDirection = textDirection;
        this.locale = locale;
        this.softWrap = softWrap;
        this.overflow = overflow;
        this.textScaleFactor = textScaleFactor;
        this.textScaler = textScaler;
        this.maxLines = maxLines;
        this.semanticsLabel = semanticsLabel;
        this.semanticsIdentifier = semanticsIdentifier;
        this.textWidthBasis = textWidthBasis;
        this.textHeightBehavior = textHeightBehavior;
        this.selectionColor = selectionColor;
        textSpan = null;
        System.Diagnostics.Debug.Assert((textScaler is null) || (textScaleFactor is null));
    }

    public static Text CreateRich(InlineSpan textSpan, Key? key = null, TextStyle? style = null, Painting.StrutStyle? strutStyle = null, TextAlign? textAlign = null, TextDirection? textDirection = null, Locale? locale = null, bool? softWrap = null, TextOverflow? overflow = null, double? textScaleFactor = null, TextScaler? textScaler = null, long? maxLines = null, string? semanticsLabel = null, string? semanticsIdentifier = null, TextWidthBasis? textWidthBasis = null, TextHeightBehavior? textHeightBehavior = null, Color? selectionColor = null)
    {
        var __instance = new Text(default!, key, style, strutStyle, textAlign, textDirection, locale, softWrap, overflow, textScaleFactor, textScaler, maxLines, semanticsLabel, semanticsIdentifier, textWidthBasis, textHeightBehavior, selectionColor);
        __instance.textSpan = textSpan;
        __instance.style = style;
        __instance.strutStyle = strutStyle;
        __instance.textAlign = textAlign;
        __instance.textDirection = textDirection;
        __instance.locale = locale;
        __instance.softWrap = softWrap;
        __instance.overflow = overflow;
        __instance.textScaleFactor = textScaleFactor;
        __instance.textScaler = textScaler;
        __instance.maxLines = maxLines;
        __instance.semanticsLabel = semanticsLabel;
        __instance.semanticsIdentifier = semanticsIdentifier;
        __instance.textWidthBasis = textWidthBasis;
        __instance.textHeightBehavior = textHeightBehavior;
        __instance.selectionColor = selectionColor;
        __instance.data = null;
        return __instance;
    }

    public override Widget build(BuildContext context)
    {
        DefaultTextStyle defaultTextStyle = DefaultTextStyle.of(context);
        TextStyle? effectiveTextStyle = style;
        if ((style is null) || style!.inherit)
        {
            effectiveTextStyle = defaultTextStyle.style.merge(style);
        }
        if (MediaQuery.boldTextOf(context))
        {
            effectiveTextStyle = effectiveTextStyle!.merge(new TextStyle(fontWeight: FontWeight.bold));
        }
        double? lineHeightScaleFactorLocal = MediaQuery.maybeLineHeightScaleFactorOverrideOf(context);
        double? letterSpacingLocal = MediaQuery.maybeLetterSpacingOverrideOf(context);
        double? wordSpacingLocal = MediaQuery.maybeWordSpacingOverrideOf(context);
        TextSpan effectiveTextSpan = _OverridingTextStyleTextSpanUtils__text.applyTextSpacingOverrides(lineHeightScaleFactor: lineHeightScaleFactorLocal, letterSpacing: letterSpacingLocal, wordSpacing: wordSpacingLocal, textSpan: new TextSpan(style: effectiveTextStyle, text: data, locale: locale, children: (textSpan is not null) ? new List<InlineSpan> { textSpan! } : null));
        Painting.StrutStyle? effectiveStrutStyle = strutStyle?.merge(new Painting.StrutStyle(height: lineHeightScaleFactorLocal));
        SelectionRegistrar? registrar = SelectionContainer.maybeOf(context);
        TextScaler textScalerLocal = (textScaler, textScaleFactor) switch { (TextScaler textScalerAlternate, _) => textScalerAlternate, (null, double textScaleFactorLocal) => TextScaler.CreateLinear(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(textScaleFactorLocal))), (null, null) => MediaQuery.textScalerOf(context) };
        Widget result = default!;
        if (registrar is not null)
        {
            result = DartRuntimePrimitives.ConvertValue<Widget>(new MouseRegion(cursor: DefaultSelectionStyle.of(context).mouseCursor ?? SystemMouseCursors.text, child: new _SelectableTextContainer__text(textAlign: (textAlign ?? defaultTextStyle.textAlign) ?? TextAlign.start, textDirection: textDirection, locale: locale, softWrap: softWrap ?? defaultTextStyle.softWrap, overflow: (overflow ?? effectiveTextStyle?.overflow) ?? defaultTextStyle.overflow, textScaler: textScalerLocal, maxLines: maxLines ?? defaultTextStyle.maxLines, strutStyle: effectiveStrutStyle, textWidthBasis: textWidthBasis ?? defaultTextStyle.textWidthBasis, textHeightBehavior: (textHeightBehavior ?? defaultTextStyle.textHeightBehavior) ?? DefaultTextHeightBehavior.maybeOf(context), selectionColor: (selectionColor ?? DefaultSelectionStyle.of(context).selectionColor) ?? DefaultSelectionStyle.defaultColor, text: effectiveTextSpan)));
        }
        else
        {
            result = DartRuntimePrimitives.ConvertValue<Widget>(new RichText(textAlign: (textAlign ?? defaultTextStyle.textAlign) ?? TextAlign.start, textDirection: textDirection, locale: locale, softWrap: softWrap ?? defaultTextStyle.softWrap, overflow: (overflow ?? effectiveTextStyle?.overflow) ?? defaultTextStyle.overflow, textScaler: textScalerLocal, maxLines: maxLines ?? defaultTextStyle.maxLines, strutStyle: effectiveStrutStyle, textWidthBasis: textWidthBasis ?? defaultTextStyle.textWidthBasis, textHeightBehavior: (textHeightBehavior ?? defaultTextStyle.textHeightBehavior) ?? DefaultTextHeightBehavior.maybeOf(context), selectionColor: (selectionColor ?? DefaultSelectionStyle.of(context).selectionColor) ?? DefaultSelectionStyle.defaultColor, text: effectiveTextSpan));
        }
        if ((semanticsLabel is not null) || (semanticsIdentifier is not null))
        {
            result = DartRuntimePrimitives.ConvertValue<Widget>(new Semantics(textDirection: textDirection, label: semanticsLabel, identifier: semanticsIdentifier, child: new ExcludeSemantics(excluding: semanticsLabel is not null, child: result)));
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new StringProperty("data", data, showName: false));
        if (textSpan is not null)
        {
            properties.add(((Diagnosticable)textSpan!).toDiagnosticsNode(name: "textSpan", style: DiagnosticsTreeStyle.transition));
        }
        style?.debugFillProperties(properties);
        properties.add(new EnumProperty<TextAlign>("textAlign", textAlign, defaultValue: null));
        properties.add(new EnumProperty<TextDirection>("textDirection", textDirection, defaultValue: null));
        properties.add(new DiagnosticsProperty<Locale>("locale", locale, defaultValue: null));
        properties.add(new FlagProperty("softWrap", value: softWrap, ifTrue: "wrapping at box width", ifFalse: "no wrapping except at line break characters", showName: true));
        properties.add(new EnumProperty<TextOverflow>("overflow", overflow, defaultValue: null));
        properties.add(new DoubleProperty("textScaleFactor", textScaleFactor, defaultValue: null));
        properties.add(new IntProperty("maxLines", maxLines, defaultValue: null));
        properties.add(new EnumProperty<TextWidthBasis>("textWidthBasis", textWidthBasis, defaultValue: null));
        properties.add(new DiagnosticsProperty<TextHeightBehavior>("textHeightBehavior", textHeightBehavior, defaultValue: null));
        if (semanticsLabel is not null)
        {
            properties.add(new StringProperty("semanticsLabel", semanticsLabel));
        }
        if (semanticsIdentifier is not null)
        {
            properties.add(new StringProperty("semanticsIdentifier", semanticsIdentifier));
        }
    }

}

internal class _SelectableTextContainer__text : StatefulWidget
{
    public virtual TextSpan text { get; private set; } = default!;
    public virtual TextAlign textAlign { get; private set; } = default!;
    public virtual TextDirection? textDirection { get; private set; }
    public virtual bool softWrap { get; private set; } = default!;
    public virtual TextOverflow overflow { get; private set; } = default!;
    public virtual TextScaler textScaler { get; private set; } = default!;
    public virtual long? maxLines { get; private set; }
    public virtual Locale? locale { get; private set; }
    public virtual Painting.StrutStyle? strutStyle { get; private set; }
    public virtual TextWidthBasis textWidthBasis { get; private set; } = default!;
    public virtual TextHeightBehavior? textHeightBehavior { get; private set; }
    public virtual Color selectionColor { get; private set; } = default!;

    internal _SelectableTextContainer__text(TextSpan text, TextAlign textAlign, TextDirection? textDirection = null, bool softWrap = default!, TextOverflow overflow = default!, TextScaler textScaler = default!, long? maxLines = null, Locale? locale = null, Painting.StrutStyle? strutStyle = null, TextWidthBasis textWidthBasis = default!, TextHeightBehavior? textHeightBehavior = null, Color selectionColor = default!)
    {
        this.text = text;
        this.textAlign = textAlign;
        this.textDirection = textDirection;
        this.softWrap = softWrap;
        this.overflow = overflow;
        this.textScaler = textScaler;
        this.maxLines = maxLines;
        this.locale = locale;
        this.strutStyle = strutStyle;
        this.textWidthBasis = textWidthBasis;
        this.textHeightBehavior = textHeightBehavior;
        this.selectionColor = selectionColor;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SelectableTextContainerState__text());
}

internal class _SelectableTextContainerState__text : State<_SelectableTextContainer__text>
{
    internal virtual _SelectableTextContainerDelegate__text _selectionDelegate { get; private set; } = default!;
    internal virtual GlobalKey<IState> _textKey { get; private set; } = GlobalKey<IState>.Create();

    public override void initState()
    {
        base.initState();
        _selectionDelegate = new _SelectableTextContainerDelegate__text(_textKey);
    }

    public override void dispose()
    {
        _selectionDelegate.dispose();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return new SelectionContainer(@delegate: _selectionDelegate, child: new _RichText__text(textKey: _textKey, textAlign: widget.textAlign, textDirection: widget.textDirection, locale: widget.locale, softWrap: widget.softWrap, overflow: widget.overflow, textScaler: widget.textScaler, maxLines: widget.maxLines, strutStyle: widget.strutStyle, textWidthBasis: widget.textWidthBasis, textHeightBehavior: widget.textHeightBehavior, selectionColor: widget.selectionColor, text: widget.text));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _RichText__text : StatelessWidget
{
    public virtual GlobalKey<IState>? textKey { get; private set; }
    public virtual InlineSpan text { get; private set; } = default!;
    public virtual TextAlign textAlign { get; private set; } = default!;
    public virtual TextDirection? textDirection { get; private set; }
    public virtual bool softWrap { get; private set; } = default!;
    public virtual TextOverflow overflow { get; private set; } = default!;
    public virtual TextScaler textScaler { get; private set; } = default!;
    public virtual long? maxLines { get; private set; }
    public virtual Locale? locale { get; private set; }
    public virtual Painting.StrutStyle? strutStyle { get; private set; }
    public virtual TextWidthBasis textWidthBasis { get; private set; } = default!;
    public virtual TextHeightBehavior? textHeightBehavior { get; private set; }
    public virtual Color selectionColor { get; private set; } = default!;

    internal _RichText__text(GlobalKey<IState>? textKey = null, InlineSpan text = default!, TextAlign textAlign = default!, TextDirection? textDirection = null, bool softWrap = default!, TextOverflow overflow = default!, TextScaler textScaler = default!, long? maxLines = null, Locale? locale = null, Painting.StrutStyle? strutStyle = null, TextWidthBasis textWidthBasis = default!, TextHeightBehavior? textHeightBehavior = null, Color selectionColor = default!)
    {
        this.textKey = textKey;
        this.text = text;
        this.textAlign = textAlign;
        this.textDirection = textDirection;
        this.softWrap = softWrap;
        this.overflow = overflow;
        this.textScaler = textScaler;
        this.maxLines = maxLines;
        this.locale = locale;
        this.strutStyle = strutStyle;
        this.textWidthBasis = textWidthBasis;
        this.textHeightBehavior = textHeightBehavior;
        this.selectionColor = selectionColor;
    }

    public override Widget build(BuildContext context)
    {
        SelectionRegistrar? registrar = SelectionContainer.maybeOf(context);
        return new RichText(key: textKey, textAlign: textAlign, textDirection: textDirection, locale: locale, softWrap: softWrap, overflow: overflow, textScaler: textScaler, maxLines: maxLines, strutStyle: strutStyle, textWidthBasis: textWidthBasis, textHeightBehavior: textHeightBehavior, selectionRegistrar: registrar, selectionColor: selectionColor, text: text);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class TextLibrary
{
    internal static double _kSelectableVerticalComparingThreshold = 3.0;
}

internal class _SelectableTextContainerDelegate__text : StaticSelectionContainerDelegate
{
    internal virtual GlobalKey<IState> _textKey { get; private set; } = default!;

    internal _SelectableTextContainerDelegate__text(GlobalKey<IState> textKey)
    {
        _textKey = textKey;
    }

    public virtual RenderParagraph paragraph => ((RenderParagraph?)_textKey.currentContext!.findRenderObject()!)!;
    public override SelectionResult handleSelectParagraph(SelectParagraphSelectionEvent @event)
    {
        SelectionResult result = _handleSelectParagraph(@event);
        base.didReceiveSelectionBoundaryEvents();
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult _handleSelectParagraph(SelectParagraphSelectionEvent @event)
    {
        if (@event.absorb)
        {
            for (var index = 0L; index < checked(selectables.Count); index += 1L)
            {
                dispatchSelectionEventToChild(selectables[(int)index], @event);
            }
            currentSelectionStartIndex = 0L;
            currentSelectionEndIndex = checked(selectables.Count) - 1L;
            return SelectionResult.next;
        }
        for (var indexLocal = 0L; indexLocal < checked(selectables.Count); indexLocal += 1L)
        {
            bool selectableIsPlaceholder = !paragraph.selectableBelongsToParagraph(selectables[(int)indexLocal]);
            if (selectableIsPlaceholder && Enumerable.Any(selectables[(int)indexLocal].boundingBoxes))
            {
                foreach (Rect rect in selectables[(int)indexLocal].boundingBoxes)
                {
                    Rect globalRect = MatrixUtils.transformRect(selectables[(int)indexLocal].getTransformTo(null), rect);
                    if (globalRect.contains(@event.globalPosition))
                    {
                        currentSelectionStartIndex = currentSelectionEndIndex = indexLocal;
                        return dispatchSelectionEventToChild(selectables[(int)indexLocal], @event);
                    }
                }
            }
        }
        SelectionResult? lastSelectionResult = default!;
        var foundStart = false;
        long? lastNextIndex = default!;
        for (var indexAlternate = 0L; indexAlternate < checked(selectables.Count); indexAlternate += 1L)
        {
            if (!paragraph.selectableBelongsToParagraph(selectables[(int)indexAlternate]))
            {
                if (foundStart)
                {
                    SelectionEvent synthesizedEvent = new SelectParagraphSelectionEvent(globalPosition: @event.globalPosition, absorb: true);
                    SelectionResult result = dispatchSelectionEventToChild(selectables[(int)indexAlternate], synthesizedEvent);
                    if ((checked(selectables.Count) - 1L) == indexAlternate)
                    {
                        currentSelectionEndIndex = indexAlternate;
                        _flushInactiveSelections();
                        return result;
                    }
                }
                continue;
            }
            SelectionGeometry existingGeometry = selectables[(int)indexAlternate].value;
            lastSelectionResult = dispatchSelectionEventToChild(selectables[(int)indexAlternate], @event);
            if ((indexAlternate == (checked(selectables.Count) - 1L)) && Equals(DartRuntimePrimitives.RequireValue(lastSelectionResult), SelectionResult.next))
            {
                if (foundStart)
                {
                    currentSelectionEndIndex = indexAlternate;
                }
                else
                {
                    currentSelectionStartIndex = currentSelectionEndIndex = indexAlternate;
                }
                return SelectionResult.next;
            }
            if (Equals(DartRuntimePrimitives.RequireValue(lastSelectionResult), SelectionResult.next))
            {
                if (Equals(selectables[(int)indexAlternate].value, existingGeometry) && !foundStart)
                {
                    lastNextIndex = indexAlternate;
                }
                if ((!Equals(selectables[(int)indexAlternate].value, existingGeometry)) && !foundStart)
                {
                    DartRuntimePrimitives.Assert(() => Enumerable.Any(selectables[(int)indexAlternate].boundingBoxes));
                    DartRuntimePrimitives.Assert(() => Enumerable.Any(selectables[(int)indexAlternate].value.selectionRects));
                    bool selectionAtStartOfSelectable = selectables[(int)indexAlternate].boundingBoxes[(int)0L].overlaps(selectables[(int)indexAlternate].value.selectionRects[(int)0L]);
                    var startIndex = 0L;
                    if ((lastNextIndex is not null) && selectionAtStartOfSelectable)
                    {
                        long lastNextIndex__38240__value40009 = DartRuntimePrimitives.RequireValue(lastNextIndex);
                        startIndex = DartRuntimePrimitives.RequireValue(lastNextIndex__38240__value40009) + 1L;
                    }
                    else
                    {
                        startIndex = ((lastNextIndex is null) && selectionAtStartOfSelectable) ? 0L : indexAlternate;
                    }
                    for (var i = startIndex; i < indexAlternate; i += 1L)
                    {
                        SelectionEvent synthesizedEventLocal = new SelectParagraphSelectionEvent(globalPosition: @event.globalPosition, absorb: true);
                        dispatchSelectionEventToChild(selectables[(int)i], synthesizedEventLocal);
                    }
                    currentSelectionStartIndex = startIndex;
                    foundStart = true;
                }
                continue;
            }
            if ((indexAlternate == 0L) && Equals(DartRuntimePrimitives.RequireValue(lastSelectionResult), SelectionResult.previous))
            {
                return SelectionResult.previous;
            }
            if (!Equals(selectables[(int)indexAlternate].value, existingGeometry))
            {
                if (!foundStart && (lastNextIndex is null))
                {
                    currentSelectionStartIndex = 0L;
                    for (var iLocal = 0L; iLocal < indexAlternate; iLocal += 1L)
                    {
                        SelectionEvent synthesizedEventAlternate = new SelectParagraphSelectionEvent(globalPosition: @event.globalPosition, absorb: true);
                        dispatchSelectionEventToChild(selectables[(int)iLocal], synthesizedEventAlternate);
                    }
                }
                currentSelectionEndIndex = indexAlternate;
                _flushInactiveSelections();
            }
            return SelectionResult.end;
        }
        DartRuntimePrimitives.Assert(() => lastSelectionResult is null);
        return SelectionResult.end;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    // Dart library-private member: distinct from the same name in the base library.
    internal new virtual SelectionResult _adjustSelection(SelectionEdgeUpdateEvent @event, bool isEnd)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (isEnd)
                {
                    DartRuntimePrimitives.Assert(() => (currentSelectionEndIndex < checked(selectables.Count)) && (currentSelectionEndIndex >= 0L));
                    return true;
                }
                DartRuntimePrimitives.Assert(() => (currentSelectionStartIndex < checked(selectables.Count)) && (currentSelectionStartIndex >= 0L));
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        SelectionResult? finalResult = default!;
        var isCurrentEdgeWithinViewport = isEnd ? (value.endSelectionPoint is not null) : (value.startSelectionPoint is not null);
        var isOppositeEdgeWithinViewport = isEnd ? (value.startSelectionPoint is not null) : (value.endSelectionPoint is not null);
        long newIndex = (isEnd, isCurrentEdgeWithinViewport, isOppositeEdgeWithinViewport) switch { (true, true, true) => currentSelectionEndIndex, (true, true, false) => currentSelectionEndIndex, (true, false, true) => currentSelectionStartIndex, (true, false, false) => 0L, (false, true, true) => currentSelectionStartIndex, (false, true, false) => currentSelectionStartIndex, (false, false, true) => currentSelectionEndIndex, (false, false, false) => 0L };
        bool? forward = default!;
        SelectionResult currentSelectableResult = default!;
        while ((newIndex < checked(selectables.Count)) && (newIndex >= 0L) && (finalResult is null))
        {
            currentSelectableResult = dispatchSelectionEventToChild(selectables[(int)newIndex], @event);
            switch (currentSelectableResult)
            {
                case SelectionResult.end:
                case SelectionResult.pending:
                case SelectionResult.none:
                    {
                        finalResult = currentSelectableResult;
                        break;
                    }
                case SelectionResult.next:
                    {
                        if (forward == false)
                        {
                            newIndex += 1L;
                            finalResult = SelectionResult.end;
                        }
                        else
                        {
                            if (newIndex == (checked(selectables.Count) - 1L))
                            {
                                finalResult = currentSelectableResult;
                            }
                            else
                            {
                                forward = true;
                                newIndex += 1L;
                            }
                        }
                        break;
                    }
                case SelectionResult.previous:
                    {
                        if (forward ?? false)
                        {
                            newIndex -= 1L;
                            finalResult = SelectionResult.end;
                        }
                        else
                        {
                            if (newIndex == 0L)
                            {
                                finalResult = currentSelectableResult;
                            }
                            else
                            {
                                forward = false;
                                newIndex -= 1L;
                            }
                        }
                        break;
                    }
            }
        }
        if (isEnd)
        {
            bool forwardSelection = currentSelectionEndIndex >= currentSelectionStartIndex;
            if ((forward is not null) && (!forwardSelection && DartRuntimePrimitives.RequireValue(forward) && (newIndex >= currentSelectionStartIndex) || forwardSelection && !DartRuntimePrimitives.RequireValue(forward) && (newIndex <= currentSelectionStartIndex)))
            {
                bool forward__43403__value45352 = DartRuntimePrimitives.RequireValue(forward);
                currentSelectionStartIndex = currentSelectionEndIndex;
            }
            currentSelectionEndIndex = newIndex;
        }
        else
        {
            bool forwardSelectionLocal = currentSelectionEndIndex >= currentSelectionStartIndex;
            if ((forward is not null) && (!forwardSelectionLocal && !DartRuntimePrimitives.RequireValue(forward) && (newIndex <= currentSelectionEndIndex) || forwardSelectionLocal && DartRuntimePrimitives.RequireValue(forward) && (newIndex >= currentSelectionEndIndex)))
            {
                bool forward__43403__value45778 = DartRuntimePrimitives.RequireValue(forward);
                currentSelectionEndIndex = currentSelectionStartIndex;
            }
            currentSelectionStartIndex = newIndex;
        }
        _flushInactiveSelections();
        return DartRuntimePrimitives.RequireValue(finalResult);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Comparison<Selectable> compareOrder => new Comparison<Selectable>((left, right) => checked((int)_compareScreenOrder(left, right)));
    internal new static long _compareScreenOrder(Selectable a, Selectable b)
    {
        Rect rectA = MatrixUtils.transformRect(a.getTransformTo(null), a.boundingBoxes.First());
        Rect rectB = MatrixUtils.transformRect(b.getTransformTo(null), b.boundingBoxes.First());
        long result = _compareVertically(rectA, rectB);
        if (result != 0L)
        {
            return result;
        }
        return _compareHorizontally(rectA, rectB);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal new static long _compareVertically(Rect a, Rect b)
    {
        if (((a.top - b.top) < Selectable_regionLibrary._kSelectableVerticalComparingThreshold) && ((a.bottom - b.bottom) > -Selectable_regionLibrary._kSelectableVerticalComparingThreshold) || ((b.top - a.top) < Selectable_regionLibrary._kSelectableVerticalComparingThreshold) && ((b.bottom - a.bottom) > -Selectable_regionLibrary._kSelectableVerticalComparingThreshold))
        {
            return 0L;
        }
        if ((a.top - b.top).abs() > Selectable_regionLibrary._kSelectableVerticalComparingThreshold)
        {
            return (a.top > b.top) ? 1L : -1L;
        }
        return (a.bottom > b.bottom) ? 1L : -1L;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal new static long _compareHorizontally(Rect a, Rect b)
    {
        if (((a.left - b.left) < Foundation.ConstantsLibrary.precisionErrorTolerance) && ((a.right - b.right) > -Foundation.ConstantsLibrary.precisionErrorTolerance))
        {
            return -1L;
        }
        if (((b.left - a.left) < Foundation.ConstantsLibrary.precisionErrorTolerance) && ((b.right - a.right) > -Foundation.ConstantsLibrary.precisionErrorTolerance))
        {
            return 1L;
        }
        if ((a.left - b.left).abs() > Foundation.ConstantsLibrary.precisionErrorTolerance)
        {
            return (a.left > b.left) ? 1L : -1L;
        }
        return (a.right > b.right) ? 1L : -1L;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    // Dart library-private member: distinct from the same name in the base library.
    internal new virtual SelectedContentRange? _calculateLocalRange(List<(long contentLength, SelectedContentRange? range)> selections)
    {
        if ((currentSelectionStartIndex == -1L) || (currentSelectionEndIndex == -1L))
        {
            return null;
        }
        var startOffsetLocal = 0L;
        var endOffsetLocal = 0L;
        var foundStart = false;
        bool forwardSelection = currentSelectionEndIndex >= currentSelectionStartIndex;
        if (currentSelectionEndIndex == currentSelectionStartIndex)
        {
            SelectedContentRange rangeAtSelectableInSelection = selectables[(int)currentSelectionStartIndex].getSelection()!;
            forwardSelection = rangeAtSelectableInSelection.endOffset >= rangeAtSelectableInSelection.startOffset;
        }
        for (var index = 0L; index < checked(selections.Count); index++)
        {
            (long contentLength, SelectedContentRange? range) selection = selections[(int)index];
            if (selection.range is null)
            {
                if (foundStart)
                {
                    return new SelectedContentRange(startOffset: forwardSelection ? startOffsetLocal : endOffsetLocal, endOffset: forwardSelection ? endOffsetLocal : startOffsetLocal);
                }
                startOffsetLocal += selection.contentLength;
                endOffsetLocal = startOffsetLocal;
                continue;
            }
            long selectionStartNormalized = Math.Min(selection.range!.startOffset, selection.range!.endOffset);
            long selectionEndNormalized = Math.Max(selection.range!.startOffset, selection.range!.endOffset);
            if (!foundStart)
            {
                bool shouldConsiderContentStart = (index > 0L) && paragraph.selectableBelongsToParagraph(selectables[(int)index]);
                startOffsetLocal += (selectionStartNormalized - (shouldConsiderContentStart ? paragraph.getPositionForOffset(selectables[(int)index].boundingBoxes.First().centerLeft).offset : 0L)).abs();
                endOffsetLocal = startOffsetLocal + (selectionEndNormalized - selectionStartNormalized).abs();
                foundStart = true;
            }
            else
            {
                endOffsetLocal += (selectionEndNormalized - selectionStartNormalized).abs();
            }
        }
        DartRuntimePrimitives.Assert(() => foundStart, () => (object?)"The start of the selection has not been found despite this selection delegate having an existing currentSelectionStartIndex and currentSelectionEndIndex.");
        return new SelectedContentRange(startOffset: forwardSelection ? startOffsetLocal : endOffsetLocal, endOffset: forwardSelection ? endOffsetLocal : startOffsetLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override SelectedContentRange? getSelection()
    {
        var selections = new List<(long contentLength, SelectedContentRange? range)>();
        return _calculateLocalRange(selections);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    // Dart library-private member: distinct from the same name in the base library.
    internal new virtual void _flushInactiveSelections()
    {
        if ((currentSelectionStartIndex == -1L) && (currentSelectionEndIndex == -1L))
        {
            return;
        }
        if ((currentSelectionStartIndex == -1L) || (currentSelectionEndIndex == -1L))
        {
            long skipIndex = (currentSelectionStartIndex == -1L) ? currentSelectionEndIndex : currentSelectionStartIndex;
            for (var i = 0L; i < checked(selectables.Count); i++)
            {
                if (i == skipIndex)
                {
                    continue;
                }
                dispatchSelectionEventToChild(selectables[(int)i], new ClearSelectionEvent());
            }
            return;
        }
        long skipStart = Math.Min(currentSelectionStartIndex, currentSelectionEndIndex);
        long skipEnd = Math.Max(currentSelectionStartIndex, currentSelectionEndIndex);
        for (var index = 0L; index < checked(selectables.Count); index += 1L)
        {
            if ((index >= skipStart) && (index <= skipEnd))
            {
                continue;
            }
            dispatchSelectionEventToChild(selectables[(int)index], new ClearSelectionEvent());
        }
    }

    public override SelectionResult handleSelectionEdgeUpdate(SelectionEdgeUpdateEvent @event)
    {
        if (!Equals(@event.granularity, TextGranularity.paragraph))
        {
            return base.handleSelectionEdgeUpdate(@event);
        }
        updateLastSelectionEdgeLocation(globalSelectionEdgeLocation: @event.globalPosition, forEnd: Equals(@event.type, SelectionEventType.endEdgeUpdate));
        if (Equals(@event.type, SelectionEventType.endEdgeUpdate))
        {
            return (currentSelectionEndIndex == -1L) ? base.handleSelectionEdgeUpdate(@event) : _adjustSelection(@event, isEnd: true);
        }
        return (currentSelectionStartIndex == -1L) ? base.handleSelectionEdgeUpdate(@event) : _adjustSelection(@event, isEnd: false);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal delegate void _SelectionInfo__text();

internal class _OverridingTextStyleTextSpanUtils__text
{
    public static TextSpan applyTextSpacingOverrides(double? lineHeightScaleFactor = null, double? letterSpacing = null, double? wordSpacing = null, TextSpan textSpan = default!)
    {
        if ((lineHeightScaleFactor is null) && (letterSpacing is null) && (wordSpacing is null))
        {
            return textSpan;
        }
        return _applyTextStyleOverrides(new TextStyle(height: lineHeightScaleFactor, letterSpacing: letterSpacing, wordSpacing: wordSpacing), textSpan);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static TextSpan _applyTextStyleOverrides(TextStyle overrideTextStyle, TextSpan textSpan)
    {
        return new TextSpan(text: textSpan.text, children: textSpan.children?.map((child) =>
        {
            if ((child is TextSpan) && Equals(DartRuntimePrimitives.RuntimeType((TextSpan)child), typeof(TextSpan)))
            {
                return _applyTextStyleOverrides(overrideTextStyle, (TextSpan)child);
            }
            return child;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }).ToList(), style: textSpan.style?.merge(overrideTextStyle) ?? overrideTextStyle, recognizer: textSpan.recognizer, mouseCursor: textSpan.mouseCursor, onEnter: textSpan.onEnter, onExit: textSpan.onExit, semanticsLabel: textSpan.semanticsLabel, semanticsIdentifier: textSpan.semanticsIdentifier, locale: textSpan.locale, spellOut: textSpan.spellOut);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

