// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/text.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Widgets;

public class DefaultTextStyle : InheritedTheme
{
    public virtual global::Doroti.Framework.Painting.TextStyle style { get; private set; } = default!;
    public virtual TextAlign? textAlign { get; private set; }
    public virtual bool softWrap { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextOverflow overflow { get; private set; } = default!;
    public virtual long? maxLines { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextWidthBasis textWidthBasis { get; private set; } = default!;
    public virtual TextHeightBehavior? textHeightBehavior { get; private set; }

    public DefaultTextStyle(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.TextStyle style = default!, TextAlign? textAlign = null, bool softWrap = true, global::Doroti.Framework.Painting.TextOverflow overflow = global::Doroti.Framework.Painting.TextOverflow.clip, long? maxLines = null, global::Doroti.Framework.Painting.TextWidthBasis textWidthBasis = global::Doroti.Framework.Painting.TextWidthBasis.parent, TextHeightBehavior? textHeightBehavior = null, Widget child = default!) : base(key: key, child: child)
    {
        this.style = style;
        this.textAlign = textAlign;
        this.softWrap = softWrap;
        this.overflow = overflow;
        this.maxLines = maxLines;
        this.textWidthBasis = textWidthBasis;
        this.textHeightBehavior = textHeightBehavior;
        System.Diagnostics.Debug.Assert(((maxLines is null) || (DartRuntimePrimitives.RequireValue(maxLines) > 0L)));
    }

    public static DefaultTextStyle CreateFallback(global::Doroti.Framework.Foundation.Key? key = null)
    {
        var __instance = new DefaultTextStyle(default!, default!, default!, default!, default!, default!, default!, default!, default!);
        __instance.style = new global::Doroti.Framework.Painting.TextStyle();
        __instance.textAlign = null;
        __instance.softWrap = true;
        __instance.maxLines = null;
        __instance.overflow = global::Doroti.Framework.Painting.TextOverflow.clip;
        __instance.textWidthBasis = global::Doroti.Framework.Painting.TextWidthBasis.parent;
        __instance.textHeightBehavior = null;
        return __instance;
    }

    public static Widget merge(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.TextStyle? style = null, TextAlign? textAlign = null, bool? softWrap = null, global::Doroti.Framework.Painting.TextOverflow? overflow = null, long? maxLines = null, global::Doroti.Framework.Painting.TextWidthBasis? textWidthBasis = null, TextHeightBehavior? textHeightBehavior = null, Widget child = default!)
    {
        return ((Widget)(object?)new Builder(builder: ((global::System.Func<BuildContext, Widget>)((context) =>
        {
            DefaultTextStyle parent = ((DefaultTextStyle)(object?)DefaultTextStyle.of(context));
            return ((Widget)(object?)new DefaultTextStyle(key: key, style: ((DefaultTextStyle)parent).style.merge(style), textAlign: (textAlign ?? ((DefaultTextStyle)parent).textAlign), softWrap: (softWrap ?? ((DefaultTextStyle)parent).softWrap), overflow: (overflow ?? ((DefaultTextStyle)parent).overflow), maxLines: (maxLines ?? ((DefaultTextStyle)parent).maxLines), textWidthBasis: (textWidthBasis ?? ((DefaultTextStyle)parent).textWidthBasis), textHeightBehavior: (textHeightBehavior ?? ((DefaultTextStyle)parent).textHeightBehavior), child: child));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DefaultTextStyle of(BuildContext context)
    {
        return (context.dependOnInheritedWidgetOfExactType<DefaultTextStyle>() ?? DefaultTextStyle.CreateFallback());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (DefaultTextStyle)(object)oldWidget;
        return (((((((!object.Equals(this.style, ((DefaultTextStyle)__oldWidget).style)) || (!object.Equals(this.textAlign, ((DefaultTextStyle)__oldWidget).textAlign))) || (this.softWrap != ((DefaultTextStyle)__oldWidget).softWrap)) || (!object.Equals(this.overflow, ((DefaultTextStyle)__oldWidget).overflow))) || (this.maxLines != ((DefaultTextStyle)__oldWidget).maxLines)) || (!object.Equals(this.textWidthBasis, ((DefaultTextStyle)__oldWidget).textWidthBasis))) || (!object.Equals(this.textHeightBehavior, ((DefaultTextStyle)__oldWidget).textHeightBehavior)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return ((Widget)(object?)new DefaultTextStyle(style: this.style, textAlign: this.textAlign, softWrap: DartRuntimePrimitives.RequireValue(this.softWrap), overflow: DartRuntimePrimitives.RequireValue(this.overflow), maxLines: this.maxLines, textWidthBasis: DartRuntimePrimitives.RequireValue(this.textWidthBasis), textHeightBehavior: this.textHeightBehavior, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        this.style.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextAlign>("textAlign", this.textAlign, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("softWrap", value: this.softWrap, ifTrue: "wrapping at box width", ifFalse: "no wrapping except at line break characters", showName: true));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Painting.TextOverflow>("overflow", this.overflow, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("maxLines", this.maxLines, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Painting.TextWidthBasis>("textWidthBasis", this.textWidthBasis, defaultValue: global::Doroti.Framework.Painting.TextWidthBasis.parent));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.TextHeightBehavior>("textHeightBehavior", this.textHeightBehavior, defaultValue: null));
    }

}

internal class _NullWidget__text : StatelessWidget
{
    internal _NullWidget__text()
    {
    }

    public override Widget build(BuildContext context)
    {
        throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("A DefaultTextStyle constructed with DefaultTextStyle.fallback cannot be incorporated into the widget tree, " + "it is meant only to provide a fallback value returned by DefaultTextStyle.of() " + "when no enclosing default text style is present in a BuildContext."));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DefaultTextHeightBehavior : InheritedTheme
{
    public virtual TextHeightBehavior textHeightBehavior { get; private set; } = default!;

    public DefaultTextHeightBehavior(global::Doroti.Framework.Foundation.Key? key = null, TextHeightBehavior textHeightBehavior = default!, Widget child = default!) : base(key: key, child: child)
    {
        this.textHeightBehavior = textHeightBehavior;
    }

    public static global::Doroti.Ui.TextHeightBehavior? maybeOf(BuildContext context)
    {
        return ((global::Doroti.Ui.TextHeightBehavior?)(object?)context.dependOnInheritedWidgetOfExactType<DefaultTextHeightBehavior>()?.textHeightBehavior);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Ui.TextHeightBehavior of(BuildContext context)
    {
        global::Doroti.Ui.TextHeightBehavior? behavior = ((global::Doroti.Ui.TextHeightBehavior?)(object?)DefaultTextHeightBehavior.maybeOf(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((behavior is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("DefaultTextHeightBehavior.of() was called with a context that does not contain a " + "DefaultTextHeightBehavior widget.\n" + "No DefaultTextHeightBehavior widget ancestor could be found starting from the " + "context that was passed to DefaultTextHeightBehavior.of(). This can happen " + "because you are using a widget that looks for a DefaultTextHeightBehavior " + "ancestor, but no such ancestor exists.\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((global::Doroti.Ui.TextHeightBehavior)(object?)behavior!);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (DefaultTextHeightBehavior)(object)oldWidget;
        return (!object.Equals(this.textHeightBehavior, ((DefaultTextHeightBehavior)__oldWidget).textHeightBehavior));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return ((Widget)(object?)new DefaultTextHeightBehavior(textHeightBehavior: this.textHeightBehavior, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.TextHeightBehavior>("textHeightBehavior", this.textHeightBehavior, defaultValue: null));
    }

}

public class Text : StatelessWidget
{
    public virtual string? data { get; private set; }
    public virtual global::Doroti.Framework.Painting.InlineSpan? textSpan { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? style { get; private set; }
    public virtual global::Doroti.Framework.Painting.StrutStyle? strutStyle { get; private set; }
    public virtual TextAlign? textAlign { get; private set; }
    public virtual TextDirection? textDirection { get; private set; }
    public virtual Locale? locale { get; private set; }
    public virtual bool? softWrap { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextOverflow? overflow { get; private set; }
    public virtual double? textScaleFactor { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextScaler? textScaler { get; private set; }
    public virtual long? maxLines { get; private set; }
    public virtual string? semanticsLabel { get; private set; }
    public virtual string? semanticsIdentifier { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextWidthBasis? textWidthBasis { get; private set; }
    public virtual TextHeightBehavior? textHeightBehavior { get; private set; }
    public virtual Color? selectionColor { get; private set; }

    public Text(string data, global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.TextStyle? style = null, global::Doroti.Framework.Painting.StrutStyle? strutStyle = null, TextAlign? textAlign = null, TextDirection? textDirection = null, Locale? locale = null, bool? softWrap = null, global::Doroti.Framework.Painting.TextOverflow? overflow = null, double? textScaleFactor = null, global::Doroti.Framework.Painting.TextScaler? textScaler = null, long? maxLines = null, string? semanticsLabel = null, string? semanticsIdentifier = null, global::Doroti.Framework.Painting.TextWidthBasis? textWidthBasis = null, TextHeightBehavior? textHeightBehavior = null, Color? selectionColor = null) : base(key: key)
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
        this.textSpan = null;
        System.Diagnostics.Debug.Assert(((textScaler is null) || (textScaleFactor is null)));
    }

    public static Text CreateRich(global::Doroti.Framework.Painting.InlineSpan textSpan, global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.TextStyle? style = null, global::Doroti.Framework.Painting.StrutStyle? strutStyle = null, TextAlign? textAlign = null, TextDirection? textDirection = null, Locale? locale = null, bool? softWrap = null, global::Doroti.Framework.Painting.TextOverflow? overflow = null, double? textScaleFactor = null, global::Doroti.Framework.Painting.TextScaler? textScaler = null, long? maxLines = null, string? semanticsLabel = null, string? semanticsIdentifier = null, global::Doroti.Framework.Painting.TextWidthBasis? textWidthBasis = null, TextHeightBehavior? textHeightBehavior = null, Color? selectionColor = null)
    {
        var __instance = new Text(default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!);
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
        DefaultTextStyle defaultTextStyle = ((DefaultTextStyle)(object?)DefaultTextStyle.of(context));
        global::Doroti.Framework.Painting.TextStyle? effectiveTextStyle = this.style;
        if (((this.style is null) || this.style!.inherit))
        {
            effectiveTextStyle = ((DefaultTextStyle)defaultTextStyle).style.merge(this.style);
        }
        if (MediaQuery.boldTextOf(context))
        {
            effectiveTextStyle = effectiveTextStyle!.merge(new global::Doroti.Framework.Painting.TextStyle(fontWeight: FontWeight.bold));
        }
        double? lineHeightScaleFactorLocal = MediaQuery.maybeLineHeightScaleFactorOverrideOf(context);
        double? letterSpacingLocal = MediaQuery.maybeLetterSpacingOverrideOf(context);
        double? wordSpacingLocal = MediaQuery.maybeWordSpacingOverrideOf(context);
        global::Doroti.Framework.Painting.TextSpan effectiveTextSpan = ((global::Doroti.Framework.Painting.TextSpan)(object?)_OverridingTextStyleTextSpanUtils__text.applyTextSpacingOverrides(lineHeightScaleFactor: lineHeightScaleFactorLocal, letterSpacing: letterSpacingLocal, wordSpacing: wordSpacingLocal, textSpan: new global::Doroti.Framework.Painting.TextSpan(style: effectiveTextStyle, text: this.data, locale: this.locale, children: ((this.textSpan is not null) ? new List<global::Doroti.Framework.Painting.InlineSpan> { this.textSpan! } : null))));
        global::Doroti.Framework.Painting.StrutStyle? effectiveStrutStyle = ((global::Doroti.Framework.Painting.StrutStyle?)(object?)this.strutStyle?.merge(new global::Doroti.Framework.Painting.StrutStyle(height: lineHeightScaleFactorLocal)));
        global::Doroti.Framework.Rendering.SelectionRegistrar? registrar = ((global::Doroti.Framework.Rendering.SelectionRegistrar?)(object?)SelectionContainer.maybeOf(context));
        global::Doroti.Framework.Painting.TextScaler textScalerLocal = ((this.textScaler, this.textScaleFactor) switch { (global::Doroti.Framework.Painting.TextScaler textScalerAlternate, _) => textScalerAlternate, (null, double textScaleFactorLocal) => global::Doroti.Framework.Painting.TextScaler.CreateLinear(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(textScaleFactorLocal))), (null, null) => MediaQuery.textScalerOf(context) });
        Widget result = default!;
        if ((registrar is not null))
        {
            result = DartRuntimePrimitives.ConvertValue<Widget>(new MouseRegion(cursor: (DefaultSelectionStyle.of(context).mouseCursor ?? global::Doroti.Framework.Services.SystemMouseCursors.text), child: new _SelectableTextContainer__text(textAlign: ((this.textAlign ?? ((DefaultTextStyle)defaultTextStyle).textAlign) ?? global::Doroti.Ui.TextAlign.start), textDirection: this.textDirection, locale: this.locale, softWrap: (this.softWrap ?? ((DefaultTextStyle)defaultTextStyle).softWrap), overflow: ((this.overflow ?? effectiveTextStyle?.overflow) ?? ((DefaultTextStyle)defaultTextStyle).overflow), textScaler: textScalerLocal, maxLines: (this.maxLines ?? ((DefaultTextStyle)defaultTextStyle).maxLines), strutStyle: effectiveStrutStyle, textWidthBasis: (this.textWidthBasis ?? ((DefaultTextStyle)defaultTextStyle).textWidthBasis), textHeightBehavior: (((this.textHeightBehavior ?? ((DefaultTextStyle)defaultTextStyle).textHeightBehavior) ?? (TextHeightBehavior)DefaultTextHeightBehavior.maybeOf(context))), selectionColor: ((this.selectionColor ?? DefaultSelectionStyle.of(context).selectionColor) ?? DefaultSelectionStyle.defaultColor), text: effectiveTextSpan)));
        }
        else
        {
            result = DartRuntimePrimitives.ConvertValue<Widget>(new RichText(textAlign: ((this.textAlign ?? ((DefaultTextStyle)defaultTextStyle).textAlign) ?? global::Doroti.Ui.TextAlign.start), textDirection: this.textDirection, locale: this.locale, softWrap: (this.softWrap ?? ((DefaultTextStyle)defaultTextStyle).softWrap), overflow: ((this.overflow ?? effectiveTextStyle?.overflow) ?? ((DefaultTextStyle)defaultTextStyle).overflow), textScaler: textScalerLocal, maxLines: (this.maxLines ?? ((DefaultTextStyle)defaultTextStyle).maxLines), strutStyle: effectiveStrutStyle, textWidthBasis: (this.textWidthBasis ?? ((DefaultTextStyle)defaultTextStyle).textWidthBasis), textHeightBehavior: (((this.textHeightBehavior ?? ((DefaultTextStyle)defaultTextStyle).textHeightBehavior) ?? (TextHeightBehavior)DefaultTextHeightBehavior.maybeOf(context))), selectionColor: ((this.selectionColor ?? DefaultSelectionStyle.of(context).selectionColor) ?? DefaultSelectionStyle.defaultColor), text: effectiveTextSpan));
        }
        if (((this.semanticsLabel is not null) || (this.semanticsIdentifier is not null)))
        {
            result = DartRuntimePrimitives.ConvertValue<Widget>(new Semantics(textDirection: this.textDirection, label: this.semanticsLabel, identifier: this.semanticsIdentifier, child: new ExcludeSemantics(excluding: (this.semanticsLabel is not null), child: result)));
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.StringProperty("data", this.data, showName: false));
        if ((this.textSpan is not null))
        {
            properties.add(((Diagnosticable)this.textSpan!).toDiagnosticsNode(name: "textSpan", style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.transition));
        }
        this.style?.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextAlign>("textAlign", this.textAlign, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Locale>("locale", this.locale, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("softWrap", value: this.softWrap, ifTrue: "wrapping at box width", ifFalse: "no wrapping except at line break characters", showName: true));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Painting.TextOverflow>("overflow", this.overflow, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("textScaleFactor", this.textScaleFactor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("maxLines", this.maxLines, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Painting.TextWidthBasis>("textWidthBasis", this.textWidthBasis, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.TextHeightBehavior>("textHeightBehavior", this.textHeightBehavior, defaultValue: null));
        if ((this.semanticsLabel is not null))
        {
            properties.add(new global::Doroti.Framework.Foundation.StringProperty("semanticsLabel", this.semanticsLabel));
        }
        if ((this.semanticsIdentifier is not null))
        {
            properties.add(new global::Doroti.Framework.Foundation.StringProperty("semanticsIdentifier", this.semanticsIdentifier));
        }
    }

}

internal class _SelectableTextContainer__text : StatefulWidget
{
    public virtual global::Doroti.Framework.Painting.TextSpan text { get; private set; } = default!;
    public virtual TextAlign textAlign { get; private set; } = default!;
    public virtual TextDirection? textDirection { get; private set; }
    public virtual bool softWrap { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextOverflow overflow { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextScaler textScaler { get; private set; } = default!;
    public virtual long? maxLines { get; private set; }
    public virtual Locale? locale { get; private set; }
    public virtual global::Doroti.Framework.Painting.StrutStyle? strutStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextWidthBasis textWidthBasis { get; private set; } = default!;
    public virtual TextHeightBehavior? textHeightBehavior { get; private set; }
    public virtual Color selectionColor { get; private set; } = default!;

    internal _SelectableTextContainer__text(global::Doroti.Framework.Painting.TextSpan text, TextAlign textAlign, TextDirection? textDirection = null, bool softWrap = default!, global::Doroti.Framework.Painting.TextOverflow overflow = default!, global::Doroti.Framework.Painting.TextScaler textScaler = default!, long? maxLines = null, Locale? locale = null, global::Doroti.Framework.Painting.StrutStyle? strutStyle = null, global::Doroti.Framework.Painting.TextWidthBasis textWidthBasis = default!, TextHeightBehavior? textHeightBehavior = null, Color selectionColor = default!)
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
        _selectionDelegate = new _SelectableTextContainerDelegate__text(this._textKey);
    }

    public override void dispose()
    {
        this._selectionDelegate.dispose();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new SelectionContainer(@delegate: this._selectionDelegate, child: new _RichText__text(textKey: this._textKey, textAlign: ((_SelectableTextContainer__text)this.widget).textAlign, textDirection: ((_SelectableTextContainer__text)this.widget).textDirection, locale: ((_SelectableTextContainer__text)this.widget).locale, softWrap: ((_SelectableTextContainer__text)this.widget).softWrap, overflow: ((_SelectableTextContainer__text)this.widget).overflow, textScaler: ((_SelectableTextContainer__text)this.widget).textScaler, maxLines: ((_SelectableTextContainer__text)this.widget).maxLines, strutStyle: ((_SelectableTextContainer__text)this.widget).strutStyle, textWidthBasis: ((_SelectableTextContainer__text)this.widget).textWidthBasis, textHeightBehavior: ((_SelectableTextContainer__text)this.widget).textHeightBehavior, selectionColor: ((_SelectableTextContainer__text)this.widget).selectionColor, text: ((_SelectableTextContainer__text)this.widget).text)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _RichText__text : StatelessWidget
{
    public virtual GlobalKey<IState>? textKey { get; private set; }
    public virtual global::Doroti.Framework.Painting.InlineSpan text { get; private set; } = default!;
    public virtual TextAlign textAlign { get; private set; } = default!;
    public virtual TextDirection? textDirection { get; private set; }
    public virtual bool softWrap { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextOverflow overflow { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextScaler textScaler { get; private set; } = default!;
    public virtual long? maxLines { get; private set; }
    public virtual Locale? locale { get; private set; }
    public virtual global::Doroti.Framework.Painting.StrutStyle? strutStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextWidthBasis textWidthBasis { get; private set; } = default!;
    public virtual TextHeightBehavior? textHeightBehavior { get; private set; }
    public virtual Color selectionColor { get; private set; } = default!;

    internal _RichText__text(GlobalKey<IState>? textKey = null, global::Doroti.Framework.Painting.InlineSpan text = default!, TextAlign textAlign = default!, TextDirection? textDirection = null, bool softWrap = default!, global::Doroti.Framework.Painting.TextOverflow overflow = default!, global::Doroti.Framework.Painting.TextScaler textScaler = default!, long? maxLines = null, Locale? locale = null, global::Doroti.Framework.Painting.StrutStyle? strutStyle = null, global::Doroti.Framework.Painting.TextWidthBasis textWidthBasis = default!, TextHeightBehavior? textHeightBehavior = null, Color selectionColor = default!)
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
        global::Doroti.Framework.Rendering.SelectionRegistrar? registrar = ((global::Doroti.Framework.Rendering.SelectionRegistrar?)(object?)SelectionContainer.maybeOf(context));
        return ((Widget)(object?)new RichText(key: this.textKey, textAlign: this.textAlign, textDirection: this.textDirection, locale: this.locale, softWrap: this.softWrap, overflow: this.overflow, textScaler: this.textScaler, maxLines: this.maxLines, strutStyle: this.strutStyle, textWidthBasis: this.textWidthBasis, textHeightBehavior: this.textHeightBehavior, selectionRegistrar: registrar, selectionColor: this.selectionColor, text: this.text));
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
        this._textKey = textKey;
    }

    public virtual global::Doroti.Framework.Rendering.RenderParagraph paragraph => ((global::Doroti.Framework.Rendering.RenderParagraph?)(object?)((GlobalKey<IState>)this._textKey).currentContext!.findRenderObject()!)!;
    public override global::Doroti.Framework.Rendering.SelectionResult handleSelectParagraph(global::Doroti.Framework.Rendering.SelectParagraphSelectionEvent @event)
    {
        global::Doroti.Framework.Rendering.SelectionResult result = _handleSelectParagraph(@event);
        base.didReceiveSelectionBoundaryEvents();
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Rendering.SelectionResult _handleSelectParagraph(global::Doroti.Framework.Rendering.SelectParagraphSelectionEvent @event)
    {
        if (((global::Doroti.Framework.Rendering.SelectParagraphSelectionEvent)@event).absorb)
        {
            for (var index = 0L; (index < checked((long)(this.selectables.Count))); index += 1L)
            {
                dispatchSelectionEventToChild(this.selectables[(int)(index)], @event);
            }
            currentSelectionStartIndex = 0L;
            currentSelectionEndIndex = (checked((long)(this.selectables.Count)) - 1L);
            return global::Doroti.Framework.Rendering.SelectionResult.next;
        }
        for (var indexLocal = 0L; (indexLocal < checked((long)(this.selectables.Count))); indexLocal += 1L)
        {
            bool selectableIsPlaceholder = !this.paragraph.selectableBelongsToParagraph(this.selectables[(int)(indexLocal)]);
            if ((selectableIsPlaceholder && System.Linq.Enumerable.Any(this.selectables[(int)(indexLocal)].boundingBoxes)))
            {
                foreach (global::Doroti.Ui.Rect rect in this.selectables[(int)(indexLocal)].boundingBoxes)
                {
                    global::Doroti.Ui.Rect globalRect = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(this.selectables[(int)(indexLocal)].getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null)), rect));
                    if (globalRect.contains(((global::Doroti.Framework.Rendering.SelectParagraphSelectionEvent)@event).globalPosition))
                    {
                        currentSelectionStartIndex = currentSelectionEndIndex = indexLocal;
                        return dispatchSelectionEventToChild(this.selectables[(int)(indexLocal)], @event);
                    }
                }
            }
        }
        global::Doroti.Framework.Rendering.SelectionResult? lastSelectionResult = default!;
        var foundStart = false;
        long? lastNextIndex = default!;
        for (var indexAlternate = 0L; (indexAlternate < checked((long)(this.selectables.Count))); indexAlternate += 1L)
        {
            if (!this.paragraph.selectableBelongsToParagraph(this.selectables[(int)(indexAlternate)]))
            {
                if (foundStart)
                {
                    global::Doroti.Framework.Rendering.SelectionEvent synthesizedEvent = ((global::Doroti.Framework.Rendering.SelectionEvent)(object?)new global::Doroti.Framework.Rendering.SelectParagraphSelectionEvent(globalPosition: ((global::Doroti.Framework.Rendering.SelectParagraphSelectionEvent)@event).globalPosition, absorb: true));
                    global::Doroti.Framework.Rendering.SelectionResult result = dispatchSelectionEventToChild(this.selectables[(int)(indexAlternate)], synthesizedEvent);
                    if (((checked((long)(this.selectables.Count)) - 1L) == indexAlternate))
                    {
                        currentSelectionEndIndex = indexAlternate;
                        _flushInactiveSelections();
                        return result;
                    }
                }
                continue;
            }
            global::Doroti.Framework.Rendering.SelectionGeometry existingGeometry = this.selectables[(int)(indexAlternate)].value;
            lastSelectionResult = dispatchSelectionEventToChild(this.selectables[(int)(indexAlternate)], @event);
            if (((indexAlternate == (checked((long)(this.selectables.Count)) - 1L)) && (object.Equals(DartRuntimePrimitives.RequireValue(lastSelectionResult), global::Doroti.Framework.Rendering.SelectionResult.next))))
            {
                if (foundStart)
                {
                    currentSelectionEndIndex = indexAlternate;
                }
                else
                {
                    currentSelectionStartIndex = currentSelectionEndIndex = indexAlternate;
                }
                return global::Doroti.Framework.Rendering.SelectionResult.next;
            }
            if ((object.Equals(DartRuntimePrimitives.RequireValue(lastSelectionResult), global::Doroti.Framework.Rendering.SelectionResult.next)))
            {
                if (((object.Equals(this.selectables[(int)(indexAlternate)].value, existingGeometry)) && !foundStart))
                {
                    lastNextIndex = indexAlternate;
                }
                if (((!object.Equals(this.selectables[(int)(indexAlternate)].value, existingGeometry)) && !foundStart))
                {
                    DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(this.selectables[(int)(indexAlternate)].boundingBoxes));
                    DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(this.selectables[(int)(indexAlternate)].value.selectionRects));
                    bool selectionAtStartOfSelectable = this.selectables[(int)(indexAlternate)].boundingBoxes[(int)(0L)].overlaps(this.selectables[(int)(indexAlternate)].value.selectionRects[(int)(0L)]);
                    var startIndex = 0L;
                    if (((lastNextIndex is not null) && selectionAtStartOfSelectable))
                    {
                        long lastNextIndex__38240__value40009 = DartRuntimePrimitives.RequireValue(lastNextIndex);
                        startIndex = (DartRuntimePrimitives.RequireValue(lastNextIndex__38240__value40009) + 1L);
                    }
                    else
                    {
                        startIndex = (((lastNextIndex is null) && selectionAtStartOfSelectable) ? 0L : indexAlternate);
                    }
                    for (var i = startIndex; (i < indexAlternate); i += 1L)
                    {
                        global::Doroti.Framework.Rendering.SelectionEvent synthesizedEventLocal = ((global::Doroti.Framework.Rendering.SelectionEvent)(object?)new global::Doroti.Framework.Rendering.SelectParagraphSelectionEvent(globalPosition: ((global::Doroti.Framework.Rendering.SelectParagraphSelectionEvent)@event).globalPosition, absorb: true));
                        dispatchSelectionEventToChild(this.selectables[(int)(i)], synthesizedEventLocal);
                    }
                    currentSelectionStartIndex = startIndex;
                    foundStart = true;
                }
                continue;
            }
            if (((indexAlternate == 0L) && (object.Equals(DartRuntimePrimitives.RequireValue(lastSelectionResult), global::Doroti.Framework.Rendering.SelectionResult.previous))))
            {
                return global::Doroti.Framework.Rendering.SelectionResult.previous;
            }
            if ((!object.Equals(this.selectables[(int)(indexAlternate)].value, existingGeometry)))
            {
                if ((!foundStart && (lastNextIndex is null)))
                {
                    currentSelectionStartIndex = 0L;
                    for (var iLocal = 0L; (iLocal < indexAlternate); iLocal += 1L)
                    {
                        global::Doroti.Framework.Rendering.SelectionEvent synthesizedEventAlternate = ((global::Doroti.Framework.Rendering.SelectionEvent)(object?)new global::Doroti.Framework.Rendering.SelectParagraphSelectionEvent(globalPosition: ((global::Doroti.Framework.Rendering.SelectParagraphSelectionEvent)@event).globalPosition, absorb: true));
                        dispatchSelectionEventToChild(this.selectables[(int)(iLocal)], synthesizedEventAlternate);
                    }
                }
                currentSelectionEndIndex = indexAlternate;
                _flushInactiveSelections();
            }
            return global::Doroti.Framework.Rendering.SelectionResult.end;
        }
        DartRuntimePrimitives.Assert(() => (lastSelectionResult is null));
        return global::Doroti.Framework.Rendering.SelectionResult.end;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Rendering.SelectionResult _adjustSelection(global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent @event, bool isEnd)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (isEnd)
                {
                    DartRuntimePrimitives.Assert(() => ((this.currentSelectionEndIndex < checked((long)(this.selectables.Count))) && (this.currentSelectionEndIndex >= 0L)));
                    return true;
                }
                DartRuntimePrimitives.Assert(() => ((this.currentSelectionStartIndex < checked((long)(this.selectables.Count))) && (this.currentSelectionStartIndex >= 0L)));
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        global::Doroti.Framework.Rendering.SelectionResult? finalResult = default!;
        var isCurrentEdgeWithinViewport = (isEnd ? (((global::Doroti.Framework.Rendering.SelectionGeometry)this.value).endSelectionPoint is not null) : (((global::Doroti.Framework.Rendering.SelectionGeometry)this.value).startSelectionPoint is not null));
        var isOppositeEdgeWithinViewport = (isEnd ? (((global::Doroti.Framework.Rendering.SelectionGeometry)this.value).startSelectionPoint is not null) : (((global::Doroti.Framework.Rendering.SelectionGeometry)this.value).endSelectionPoint is not null));
        long newIndex = ((isEnd, isCurrentEdgeWithinViewport, isOppositeEdgeWithinViewport) switch { (true, true, true) => this.currentSelectionEndIndex, (true, true, false) => this.currentSelectionEndIndex, (true, false, true) => this.currentSelectionStartIndex, (true, false, false) => 0L, (false, true, true) => this.currentSelectionStartIndex, (false, true, false) => this.currentSelectionStartIndex, (false, false, true) => this.currentSelectionEndIndex, (false, false, false) => 0L });
        bool? forward = default!;
        global::Doroti.Framework.Rendering.SelectionResult currentSelectableResult = default!;
        while ((((newIndex < checked((long)(this.selectables.Count))) && (newIndex >= 0L)) && (finalResult is null)))
        {
            currentSelectableResult = dispatchSelectionEventToChild(this.selectables[(int)(newIndex)], @event);
            switch (currentSelectableResult)
            {
                case global::Doroti.Framework.Rendering.SelectionResult.end:
                case global::Doroti.Framework.Rendering.SelectionResult.pending:
                case global::Doroti.Framework.Rendering.SelectionResult.none:
                    {
                        finalResult = currentSelectableResult;
                        break;
                    }
                case global::Doroti.Framework.Rendering.SelectionResult.next:
                    {
                        if ((forward == false))
                        {
                            newIndex += 1L;
                            finalResult = global::Doroti.Framework.Rendering.SelectionResult.end;
                        }
                        else
                        {
                            if ((newIndex == (checked((long)(this.selectables.Count)) - 1L)))
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
                case global::Doroti.Framework.Rendering.SelectionResult.previous:
                    {
                        if ((forward ?? false))
                        {
                            newIndex -= 1L;
                            finalResult = global::Doroti.Framework.Rendering.SelectionResult.end;
                        }
                        else
                        {
                            if ((newIndex == 0L))
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
            bool forwardSelection = (this.currentSelectionEndIndex >= this.currentSelectionStartIndex);
            if (((forward is not null) && (((((!forwardSelection && DartRuntimePrimitives.RequireValue(forward)) && (newIndex >= this.currentSelectionStartIndex))) || (((forwardSelection && !DartRuntimePrimitives.RequireValue(forward)) && (newIndex <= this.currentSelectionStartIndex)))))))
            {
                bool forward__43403__value45352 = DartRuntimePrimitives.RequireValue(forward);
                currentSelectionStartIndex = this.currentSelectionEndIndex;
            }
            currentSelectionEndIndex = newIndex;
        }
        else
        {
            bool forwardSelectionLocal = (this.currentSelectionEndIndex >= this.currentSelectionStartIndex);
            if (((forward is not null) && (((((!forwardSelectionLocal && !DartRuntimePrimitives.RequireValue(forward)) && (newIndex <= this.currentSelectionEndIndex))) || (((forwardSelectionLocal && DartRuntimePrimitives.RequireValue(forward)) && (newIndex >= this.currentSelectionEndIndex)))))))
            {
                bool forward__43403__value45778 = DartRuntimePrimitives.RequireValue(forward);
                currentSelectionEndIndex = this.currentSelectionStartIndex;
            }
            currentSelectionStartIndex = newIndex;
        }
        _flushInactiveSelections();
        return DartRuntimePrimitives.RequireValue(finalResult);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Comparison<global::Doroti.Framework.Rendering.Selectable> compareOrder => new Comparison<global::Doroti.Framework.Rendering.Selectable>((left, right) => checked((int)_compareScreenOrder(left, right)));
    internal static long _compareScreenOrder(global::Doroti.Framework.Rendering.Selectable a, global::Doroti.Framework.Rendering.Selectable b)
    {
        global::Doroti.Ui.Rect rectA = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(a.getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null)), ((global::Doroti.Framework.Rendering.Selectable)a).boundingBoxes.First()));
        global::Doroti.Ui.Rect rectB = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(b.getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null)), ((global::Doroti.Framework.Rendering.Selectable)b).boundingBoxes.First()));
        long result = _SelectableTextContainerDelegate__text._compareVertically(rectA, rectB);
        if ((result != 0L))
        {
            return result;
        }
        return _SelectableTextContainerDelegate__text._compareHorizontally(rectA, rectB);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static long _compareVertically(Rect a, Rect b)
    {
        if ((((((a.top - b.top) < global::Doroti.Framework.Widgets.Selectable_regionLibrary._kSelectableVerticalComparingThreshold) && ((a.bottom - b.bottom) > -global::Doroti.Framework.Widgets.Selectable_regionLibrary._kSelectableVerticalComparingThreshold))) || ((((b.top - a.top) < global::Doroti.Framework.Widgets.Selectable_regionLibrary._kSelectableVerticalComparingThreshold) && ((b.bottom - a.bottom) > -global::Doroti.Framework.Widgets.Selectable_regionLibrary._kSelectableVerticalComparingThreshold)))))
        {
            return 0L;
        }
        if ((((a.top - b.top)).abs() > global::Doroti.Framework.Widgets.Selectable_regionLibrary._kSelectableVerticalComparingThreshold))
        {
            return ((a.top > b.top) ? 1L : -1L);
        }
        return ((a.bottom > b.bottom) ? 1L : -1L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static long _compareHorizontally(Rect a, Rect b)
    {
        if ((((a.left - b.left) < global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance) && ((a.right - b.right) > -global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance)))
        {
            return -1L;
        }
        if ((((b.left - a.left) < global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance) && ((b.right - a.right) > -global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance)))
        {
            return 1L;
        }
        if ((((a.left - b.left)).abs() > global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
        {
            return ((a.left > b.left) ? 1L : -1L);
        }
        return ((a.right > b.right) ? 1L : -1L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Rendering.SelectedContentRange? _calculateLocalRange(List<(long contentLength, global::Doroti.Framework.Rendering.SelectedContentRange? range)> selections)
    {
        if (((this.currentSelectionStartIndex == -1L) || (this.currentSelectionEndIndex == -1L)))
        {
            return ((global::Doroti.Framework.Rendering.SelectedContentRange)(object)null);
        }
        var startOffsetLocal = 0L;
        var endOffsetLocal = 0L;
        var foundStart = false;
        bool forwardSelection = (this.currentSelectionEndIndex >= this.currentSelectionStartIndex);
        if ((this.currentSelectionEndIndex == this.currentSelectionStartIndex))
        {
            global::Doroti.Framework.Rendering.SelectedContentRange rangeAtSelectableInSelection = this.selectables[(int)(this.currentSelectionStartIndex)].getSelection()!;
            forwardSelection = (((global::Doroti.Framework.Rendering.SelectedContentRange)rangeAtSelectableInSelection).endOffset >= ((global::Doroti.Framework.Rendering.SelectedContentRange)rangeAtSelectableInSelection).startOffset);
        }
        for (var index = 0L; (index < checked((long)(selections.Count))); index++)
        {
            (long contentLength, global::Doroti.Framework.Rendering.SelectedContentRange? range) selection = selections[(int)(index)];
            if ((selection.range is null))
            {
                if (foundStart)
                {
                    return new global::Doroti.Framework.Rendering.SelectedContentRange(startOffset: (forwardSelection ? startOffsetLocal : endOffsetLocal), endOffset: (forwardSelection ? endOffsetLocal : startOffsetLocal));
                }
                startOffsetLocal += selection.contentLength;
                endOffsetLocal = startOffsetLocal;
                continue;
            }
            long selectionStartNormalized = Math.Min(selection.range!.startOffset, selection.range!.endOffset);
            long selectionEndNormalized = Math.Max(selection.range!.startOffset, selection.range!.endOffset);
            if (!foundStart)
            {
                bool shouldConsiderContentStart = ((index > 0L) && this.paragraph.selectableBelongsToParagraph(this.selectables[(int)(index)]));
                startOffsetLocal += ((selectionStartNormalized - ((shouldConsiderContentStart ? this.paragraph.getPositionForOffset(this.selectables[(int)(index)].boundingBoxes.First().centerLeft).offset : 0L)))).abs();
                endOffsetLocal = (startOffsetLocal + ((selectionEndNormalized - selectionStartNormalized)).abs());
                foundStart = true;
            }
            else
            {
                endOffsetLocal += ((selectionEndNormalized - selectionStartNormalized)).abs();
            }
        }
        DartRuntimePrimitives.Assert(() => foundStart, () => (object?)"The start of the selection has not been found despite this selection delegate having an existing currentSelectionStartIndex and currentSelectionEndIndex.");
        return new global::Doroti.Framework.Rendering.SelectedContentRange(startOffset: (forwardSelection ? startOffsetLocal : endOffsetLocal), endOffset: (forwardSelection ? endOffsetLocal : startOffsetLocal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Rendering.SelectedContentRange? getSelection()
    {
        var selections = new List<(long contentLength, global::Doroti.Framework.Rendering.SelectedContentRange? range)>();
        return ((global::Doroti.Framework.Rendering.SelectedContentRange?)(object?)_calculateLocalRange(selections));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _flushInactiveSelections()
    {
        if (((this.currentSelectionStartIndex == -1L) && (this.currentSelectionEndIndex == -1L)))
        {
            return;
        }
        if (((this.currentSelectionStartIndex == -1L) || (this.currentSelectionEndIndex == -1L)))
        {
            long skipIndex = ((this.currentSelectionStartIndex == -1L) ? this.currentSelectionEndIndex : this.currentSelectionStartIndex);
            for (var i = 0L; (i < checked((long)(this.selectables.Count))); i++)
            {
                if ((i == skipIndex))
                {
                    continue;
                }
                dispatchSelectionEventToChild(this.selectables[(int)(i)], new global::Doroti.Framework.Rendering.ClearSelectionEvent());
            }
            return;
        }
        long skipStart = Math.Min(this.currentSelectionStartIndex, this.currentSelectionEndIndex);
        long skipEnd = Math.Max(this.currentSelectionStartIndex, this.currentSelectionEndIndex);
        for (var index = 0L; (index < checked((long)(this.selectables.Count))); index += 1L)
        {
            if (((index >= skipStart) && (index <= skipEnd)))
            {
                continue;
            }
            dispatchSelectionEventToChild(this.selectables[(int)(index)], new global::Doroti.Framework.Rendering.ClearSelectionEvent());
        }
    }

    public override global::Doroti.Framework.Rendering.SelectionResult handleSelectionEdgeUpdate(global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent @event)
    {
        if ((!object.Equals(((global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent)@event).granularity, global::Doroti.Framework.Rendering.TextGranularity.paragraph)))
        {
            return base.handleSelectionEdgeUpdate(@event);
        }
        updateLastSelectionEdgeLocation(globalSelectionEdgeLocation: ((global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent)@event).globalPosition, forEnd: (object.Equals(@event.type, global::Doroti.Framework.Rendering.SelectionEventType.endEdgeUpdate)));
        if ((object.Equals(@event.type, global::Doroti.Framework.Rendering.SelectionEventType.endEdgeUpdate)))
        {
            return ((this.currentSelectionEndIndex == -1L) ? base.handleSelectionEdgeUpdate(@event) : _adjustSelection(@event, isEnd: true));
        }
        return ((this.currentSelectionStartIndex == -1L) ? base.handleSelectionEdgeUpdate(@event) : _adjustSelection(@event, isEnd: false));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal delegate void _SelectionInfo__text();

internal class _OverridingTextStyleTextSpanUtils__text
{
    public static global::Doroti.Framework.Painting.TextSpan applyTextSpacingOverrides(double? lineHeightScaleFactor = null, double? letterSpacing = null, double? wordSpacing = null, global::Doroti.Framework.Painting.TextSpan textSpan = default!)
    {
        if ((((lineHeightScaleFactor is null) && (letterSpacing is null)) && (wordSpacing is null)))
        {
            return textSpan;
        }
        return ((global::Doroti.Framework.Painting.TextSpan)(object?)_OverridingTextStyleTextSpanUtils__text._applyTextStyleOverrides(new global::Doroti.Framework.Painting.TextStyle(height: lineHeightScaleFactor, letterSpacing: letterSpacing, wordSpacing: wordSpacing), textSpan));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Framework.Painting.TextSpan _applyTextStyleOverrides(global::Doroti.Framework.Painting.TextStyle overrideTextStyle, global::Doroti.Framework.Painting.TextSpan textSpan)
    {
        return new global::Doroti.Framework.Painting.TextSpan(text: ((global::Doroti.Framework.Painting.TextSpan)textSpan).text, children: ((global::Doroti.Framework.Painting.TextSpan)textSpan).children?.map<global::Doroti.Framework.Painting.InlineSpan, global::Doroti.Framework.Painting.InlineSpan>(((child) =>
        {
            if (((child is global::Doroti.Framework.Painting.TextSpan) && (object.Equals(DartRuntimePrimitives.RuntimeType(((global::Doroti.Framework.Painting.TextSpan)child)), typeof(global::Doroti.Framework.Painting.TextSpan)))))
            {
                return ((global::Doroti.Framework.Painting.InlineSpan)(object?)_OverridingTextStyleTextSpanUtils__text._applyTextStyleOverrides(overrideTextStyle, ((global::Doroti.Framework.Painting.TextSpan)child)));
            }
            return child;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })).ToList(), style: (textSpan.style?.merge(overrideTextStyle) ?? overrideTextStyle), recognizer: ((global::Doroti.Framework.Painting.TextSpan)textSpan).recognizer, mouseCursor: ((global::Doroti.Framework.Painting.TextSpan)textSpan).mouseCursor, onEnter: (global::System.Action<global::Doroti.Framework.Gestures.PointerEnterEvent>?)((global::Doroti.Framework.Painting.TextSpan)textSpan).onEnter, onExit: (global::System.Action<global::Doroti.Framework.Gestures.PointerExitEvent>?)((global::Doroti.Framework.Painting.TextSpan)textSpan).onExit, semanticsLabel: ((global::Doroti.Framework.Painting.TextSpan)textSpan).semanticsLabel, semanticsIdentifier: ((global::Doroti.Framework.Painting.TextSpan)textSpan).semanticsIdentifier, locale: ((global::Doroti.Framework.Painting.TextSpan)textSpan).locale, spellOut: ((global::Doroti.Framework.Painting.TextSpan)textSpan).spellOut);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

