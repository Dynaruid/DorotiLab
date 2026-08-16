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
        return ((Widget)(object?)new Builder(builder: ((global::System.Func<BuildContext, Widget>)((context) => {
DefaultTextStyle parent__4908 = ((DefaultTextStyle)(object?)DefaultTextStyle.of(context));
return ((Widget)(object?)new DefaultTextStyle(key: key, style: ((DefaultTextStyle)parent__4908).style.merge(style), textAlign: (textAlign ?? ((DefaultTextStyle)parent__4908).textAlign), softWrap: (softWrap ?? ((DefaultTextStyle)parent__4908).softWrap), overflow: (overflow ?? ((DefaultTextStyle)parent__4908).overflow), maxLines: (maxLines ?? ((DefaultTextStyle)parent__4908).maxLines), textWidthBasis: (textWidthBasis ?? ((DefaultTextStyle)parent__4908).textWidthBasis), textHeightBehavior: (textHeightBehavior ?? ((DefaultTextStyle)parent__4908).textHeightBehavior), child: child));
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
        global::Doroti.Ui.TextHeightBehavior? behavior__12097 = ((global::Doroti.Ui.TextHeightBehavior?)(object?)DefaultTextHeightBehavior.maybeOf(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((behavior__12097 is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("DefaultTextHeightBehavior.of() was called with a context that does not contain a " + "DefaultTextHeightBehavior widget.\n" + "No DefaultTextHeightBehavior widget ancestor could be found starting from the " + "context that was passed to DefaultTextHeightBehavior.of(). This can happen " + "because you are using a widget that looks for a DefaultTextHeightBehavior " + "ancestor, but no such ancestor exists.\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((global::Doroti.Ui.TextHeightBehavior)(object?)behavior__12097!);
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
        DefaultTextStyle defaultTextStyle__26353 = ((DefaultTextStyle)(object?)DefaultTextStyle.of(context));
        global::Doroti.Framework.Painting.TextStyle? effectiveTextStyle__26417 = this.style;
        if (((this.style is null) || this.style!.inherit))
        {
            effectiveTextStyle__26417 = ((DefaultTextStyle)defaultTextStyle__26353).style.merge(this.style);
        }
        if (MediaQuery.boldTextOf(context))
        {
            effectiveTextStyle__26417 = effectiveTextStyle__26417!.merge(new global::Doroti.Framework.Painting.TextStyle(fontWeight: FontWeight.bold));
        }
        double? lineHeightScaleFactor__26998 = MediaQuery.maybeLineHeightScaleFactorOverrideOf(context);
        double? letterSpacing__27098 = MediaQuery.maybeLetterSpacingOverrideOf(context);
        double? wordSpacing__27182 = MediaQuery.maybeWordSpacingOverrideOf(context);
        global::Doroti.Framework.Painting.TextSpan effectiveTextSpan__27263 = ((global::Doroti.Framework.Painting.TextSpan)(object?)_OverridingTextStyleTextSpanUtils__text.applyTextSpacingOverrides(lineHeightScaleFactor: lineHeightScaleFactor__26998, letterSpacing: letterSpacing__27098, wordSpacing: wordSpacing__27182, textSpan: new global::Doroti.Framework.Painting.TextSpan(style: effectiveTextStyle__26417, text: this.data, locale: this.locale, children: ((this.textSpan is not null) ? new List<global::Doroti.Framework.Painting.InlineSpan> { this.textSpan! } : null))));
        global::Doroti.Framework.Painting.StrutStyle? effectiveStrutStyle__27676 = ((global::Doroti.Framework.Painting.StrutStyle?)(object?)this.strutStyle?.merge(new global::Doroti.Framework.Painting.StrutStyle(height: lineHeightScaleFactor__26998)));
        global::Doroti.Framework.Rendering.SelectionRegistrar? registrar__27803 = ((global::Doroti.Framework.Rendering.SelectionRegistrar?)(object?)SelectionContainer.maybeOf(context));
        global::Doroti.Framework.Painting.TextScaler textScaler__27873 = ((this.textScaler, this.textScaleFactor) switch { (global::Doroti.Framework.Painting.TextScaler textScaler__27956, _) => textScaler__27956, (null, double textScaleFactor__28072) => global::Doroti.Framework.Painting.TextScaler.CreateLinear(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(textScaleFactor__28072))), (null, null) => MediaQuery.textScalerOf(context) });
        Widget result__28207 = default!;
        if ((registrar__27803 is not null))
        {
            result__28207 = DartRuntimePrimitives.ConvertValue<Widget>(new MouseRegion(cursor: (DefaultSelectionStyle.of(context).mouseCursor ?? global::Doroti.Framework.Services.SystemMouseCursors.text), child: new _SelectableTextContainer__text(textAlign: ((this.textAlign ?? ((DefaultTextStyle)defaultTextStyle__26353).textAlign) ?? global::Doroti.Ui.TextAlign.start), textDirection: this.textDirection, locale: this.locale, softWrap: (this.softWrap ?? ((DefaultTextStyle)defaultTextStyle__26353).softWrap), overflow: ((this.overflow ?? effectiveTextStyle__26417?.overflow) ?? ((DefaultTextStyle)defaultTextStyle__26353).overflow), textScaler: textScaler__27873, maxLines: (this.maxLines ?? ((DefaultTextStyle)defaultTextStyle__26353).maxLines), strutStyle: effectiveStrutStyle__27676, textWidthBasis: (this.textWidthBasis ?? ((DefaultTextStyle)defaultTextStyle__26353).textWidthBasis), textHeightBehavior: (((this.textHeightBehavior ?? ((DefaultTextStyle)defaultTextStyle__26353).textHeightBehavior) ?? (TextHeightBehavior)DefaultTextHeightBehavior.maybeOf(context))), selectionColor: ((this.selectionColor ?? DefaultSelectionStyle.of(context).selectionColor) ?? DefaultSelectionStyle.defaultColor), text: effectiveTextSpan__27263)));
        }
        else
        {
            result__28207 = DartRuntimePrimitives.ConvertValue<Widget>(new RichText(textAlign: ((this.textAlign ?? ((DefaultTextStyle)defaultTextStyle__26353).textAlign) ?? global::Doroti.Ui.TextAlign.start), textDirection: this.textDirection, locale: this.locale, softWrap: (this.softWrap ?? ((DefaultTextStyle)defaultTextStyle__26353).softWrap), overflow: ((this.overflow ?? effectiveTextStyle__26417?.overflow) ?? ((DefaultTextStyle)defaultTextStyle__26353).overflow), textScaler: textScaler__27873, maxLines: (this.maxLines ?? ((DefaultTextStyle)defaultTextStyle__26353).maxLines), strutStyle: effectiveStrutStyle__27676, textWidthBasis: (this.textWidthBasis ?? ((DefaultTextStyle)defaultTextStyle__26353).textWidthBasis), textHeightBehavior: (((this.textHeightBehavior ?? ((DefaultTextStyle)defaultTextStyle__26353).textHeightBehavior) ?? (TextHeightBehavior)DefaultTextHeightBehavior.maybeOf(context))), selectionColor: ((this.selectionColor ?? DefaultSelectionStyle.of(context).selectionColor) ?? DefaultSelectionStyle.defaultColor), text: effectiveTextSpan__27263));
        }
        if (((this.semanticsLabel is not null) || (this.semanticsIdentifier is not null)))
        {
            result__28207 = DartRuntimePrimitives.ConvertValue<Widget>(new Semantics(textDirection: this.textDirection, label: this.semanticsLabel, identifier: this.semanticsIdentifier, child: new ExcludeSemantics(excluding: (this.semanticsLabel is not null), child: result__28207)));
        }
        return result__28207;
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
        global::Doroti.Framework.Rendering.SelectionRegistrar? registrar__35597 = ((global::Doroti.Framework.Rendering.SelectionRegistrar?)(object?)SelectionContainer.maybeOf(context));
        return ((Widget)(object?)new RichText(key: this.textKey, textAlign: this.textAlign, textDirection: this.textDirection, locale: this.locale, softWrap: this.softWrap, overflow: this.overflow, textScaler: this.textScaler, maxLines: this.maxLines, strutStyle: this.strutStyle, textWidthBasis: this.textWidthBasis, textHeightBehavior: this.textHeightBehavior, selectionRegistrar: registrar__35597, selectionColor: this.selectionColor, text: this.text));
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
        global::Doroti.Framework.Rendering.SelectionResult result__36788 = _handleSelectParagraph(@event);
        base.didReceiveSelectionBoundaryEvents();
        return result__36788;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Rendering.SelectionResult _handleSelectParagraph(global::Doroti.Framework.Rendering.SelectParagraphSelectionEvent @event)
    {
        if (((global::Doroti.Framework.Rendering.SelectParagraphSelectionEvent)@event).absorb)
        {
            for (var index__37018 = 0L; (index__37018 < checked((long)(this.selectables.Count))); index__37018 += 1L)
            {
                dispatchSelectionEventToChild(this.selectables[(int)(index__37018)], @event);
            }
            currentSelectionStartIndex = 0L;
            currentSelectionEndIndex = (checked((long)(this.selectables.Count)) - 1L);
            return global::Doroti.Framework.Rendering.SelectionResult.next;
        }
        for (var index__37442 = 0L; (index__37442 < checked((long)(this.selectables.Count))); index__37442 += 1L)
        {
            bool selectableIsPlaceholder__37512 = !this.paragraph.selectableBelongsToParagraph(this.selectables[(int)(index__37442)]);
            if ((selectableIsPlaceholder__37512 && System.Linq.Enumerable.Any(this.selectables[(int)(index__37442)].boundingBoxes)))
            {
                foreach (global::Doroti.Ui.Rect rect__37724 in this.selectables[(int)(index__37442)].boundingBoxes)
                {
                    global::Doroti.Ui.Rect globalRect__37789 = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(this.selectables[(int)(index__37442)].getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null)), rect__37724));
                    if (globalRect__37789.contains(((global::Doroti.Framework.Rendering.SelectParagraphSelectionEvent)@event).globalPosition))
                    {
                        currentSelectionStartIndex = currentSelectionEndIndex = index__37442;
                        return dispatchSelectionEventToChild(this.selectables[(int)(index__37442)], @event);
                    }
                }
            }
        }
        global::Doroti.Framework.Rendering.SelectionResult? lastSelectionResult__38182 = default!;
        var foundStart__38211 = false;
        long? lastNextIndex__38240 = default!;
        for (var index__38268 = 0L; (index__38268 < checked((long)(this.selectables.Count))); index__38268 += 1L)
        {
            if (!this.paragraph.selectableBelongsToParagraph(this.selectables[(int)(index__38268)]))
            {
                if (foundStart__38211)
                {
                    global::Doroti.Framework.Rendering.SelectionEvent synthesizedEvent__38451 = ((global::Doroti.Framework.Rendering.SelectionEvent)(object?)new global::Doroti.Framework.Rendering.SelectParagraphSelectionEvent(globalPosition: ((global::Doroti.Framework.Rendering.SelectParagraphSelectionEvent)@event).globalPosition, absorb: true));
                    global::Doroti.Framework.Rendering.SelectionResult result__38622 = dispatchSelectionEventToChild(this.selectables[(int)(index__38268)], synthesizedEvent__38451);
                    if (((checked((long)(this.selectables.Count)) - 1L) == index__38268))
                    {
                        currentSelectionEndIndex = index__38268;
                        _flushInactiveSelections();
                        return result__38622;
                    }
                }
                continue;
            }
            global::Doroti.Framework.Rendering.SelectionGeometry existingGeometry__38977 = this.selectables[(int)(index__38268)].value;
            lastSelectionResult__38182 = dispatchSelectionEventToChild(this.selectables[(int)(index__38268)], @event);
            if (((index__38268 == (checked((long)(this.selectables.Count)) - 1L)) && (object.Equals(DartRuntimePrimitives.RequireValue(lastSelectionResult__38182), global::Doroti.Framework.Rendering.SelectionResult.next))))
            {
                if (foundStart__38211)
                {
                    currentSelectionEndIndex = index__38268;
                }
                else
                {
                    currentSelectionStartIndex = currentSelectionEndIndex = index__38268;
                }
                return global::Doroti.Framework.Rendering.SelectionResult.next;
            }
            if ((object.Equals(DartRuntimePrimitives.RequireValue(lastSelectionResult__38182), global::Doroti.Framework.Rendering.SelectionResult.next)))
            {
                if (((object.Equals(this.selectables[(int)(index__38268)].value, existingGeometry__38977)) && !foundStart__38211))
                {
                    lastNextIndex__38240 = index__38268;
                }
                if (((!object.Equals(this.selectables[(int)(index__38268)].value, existingGeometry__38977)) && !foundStart__38211))
                {
                    DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(this.selectables[(int)(index__38268)].boundingBoxes));
                    DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(this.selectables[(int)(index__38268)].value.selectionRects));
                    bool selectionAtStartOfSelectable__39819 = this.selectables[(int)(index__38268)].boundingBoxes[(int)(0L)].overlaps(this.selectables[(int)(index__38268)].value.selectionRects[(int)(0L)]);
                    var startIndex__39979 = 0L;
                    if (((lastNextIndex__38240 is not null) && selectionAtStartOfSelectable__39819))
                    {
                        long lastNextIndex__38240__value40009 = DartRuntimePrimitives.RequireValue(lastNextIndex__38240);
                        startIndex__39979 = (DartRuntimePrimitives.RequireValue(lastNextIndex__38240__value40009) + 1L);
                    }
                    else
                    {
                        startIndex__39979 = (((lastNextIndex__38240 is null) && selectionAtStartOfSelectable__39819) ? 0L : index__38268);
                    }
                    for (var i__40252 = startIndex__39979; (i__40252 < index__38268); i__40252 += 1L)
                    {
                        global::Doroti.Framework.Rendering.SelectionEvent synthesizedEvent__40322 = ((global::Doroti.Framework.Rendering.SelectionEvent)(object?)new global::Doroti.Framework.Rendering.SelectParagraphSelectionEvent(globalPosition: ((global::Doroti.Framework.Rendering.SelectParagraphSelectionEvent)@event).globalPosition, absorb: true));
                        dispatchSelectionEventToChild(this.selectables[(int)(i__40252)], synthesizedEvent__40322);
                    }
                    currentSelectionStartIndex = startIndex__39979;
                    foundStart__38211 = true;
                }
                continue;
            }
            if (((index__38268 == 0L) && (object.Equals(DartRuntimePrimitives.RequireValue(lastSelectionResult__38182), global::Doroti.Framework.Rendering.SelectionResult.previous))))
            {
                return global::Doroti.Framework.Rendering.SelectionResult.previous;
            }
            if ((!object.Equals(this.selectables[(int)(index__38268)].value, existingGeometry__38977)))
            {
                if ((!foundStart__38211 && (lastNextIndex__38240 is null)))
                {
                    currentSelectionStartIndex = 0L;
                    for (var i__40967 = 0L; (i__40967 < index__38268); i__40967 += 1L)
                    {
                        global::Doroti.Framework.Rendering.SelectionEvent synthesizedEvent__41028 = ((global::Doroti.Framework.Rendering.SelectionEvent)(object?)new global::Doroti.Framework.Rendering.SelectParagraphSelectionEvent(globalPosition: ((global::Doroti.Framework.Rendering.SelectParagraphSelectionEvent)@event).globalPosition, absorb: true));
                        dispatchSelectionEventToChild(this.selectables[(int)(i__40967)], synthesizedEvent__41028);
                    }
                }
                currentSelectionEndIndex = index__38268;
                _flushInactiveSelections();
            }
            return global::Doroti.Framework.Rendering.SelectionResult.end;
        }
        DartRuntimePrimitives.Assert(() => (lastSelectionResult__38182 is null));
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
        global::Doroti.Framework.Rendering.SelectionResult? finalResult__42028 = default!;
        var isCurrentEdgeWithinViewport__42628 = (isEnd ? (((global::Doroti.Framework.Rendering.SelectionGeometry)this.value).endSelectionPoint is not null) : (((global::Doroti.Framework.Rendering.SelectionGeometry)this.value).startSelectionPoint is not null));
        var isOppositeEdgeWithinViewport__42761 = (isEnd ? (((global::Doroti.Framework.Rendering.SelectionGeometry)this.value).startSelectionPoint is not null) : (((global::Doroti.Framework.Rendering.SelectionGeometry)this.value).endSelectionPoint is not null));
        long newIndex__42893 = ((isEnd, isCurrentEdgeWithinViewport__42628, isOppositeEdgeWithinViewport__42761) switch { (true, true, true) => this.currentSelectionEndIndex, (true, true, false) => this.currentSelectionEndIndex, (true, false, true) => this.currentSelectionStartIndex, (true, false, false) => 0L, (false, true, true) => this.currentSelectionStartIndex, (false, true, false) => this.currentSelectionStartIndex, (false, false, true) => this.currentSelectionEndIndex, (false, false, false) => 0L });
        bool? forward__43403 = default!;
        global::Doroti.Framework.Rendering.SelectionResult currentSelectableResult__43437 = default!;
        while ((((newIndex__42893 < checked((long)(this.selectables.Count))) && (newIndex__42893 >= 0L)) && (finalResult__42028 is null)))
        {
            currentSelectableResult__43437 = dispatchSelectionEventToChild(this.selectables[(int)(newIndex__42893)], @event);
            switch (currentSelectableResult__43437)
            {
                case global::Doroti.Framework.Rendering.SelectionResult.end:
                case global::Doroti.Framework.Rendering.SelectionResult.pending:
                case global::Doroti.Framework.Rendering.SelectionResult.none:
                    {
                        finalResult__42028 = currentSelectableResult__43437;
                        break;
                    }
                case global::Doroti.Framework.Rendering.SelectionResult.next:
                    {
                        if ((forward__43403 == false))
                        {
                            newIndex__42893 += 1L;
                            finalResult__42028 = global::Doroti.Framework.Rendering.SelectionResult.end;
                        }
                        else
                        {
                            if ((newIndex__42893 == (checked((long)(this.selectables.Count)) - 1L)))
                            {
                                finalResult__42028 = currentSelectableResult__43437;
                            }
                            else
                            {
                                forward__43403 = true;
                                newIndex__42893 += 1L;
                            }
                        }
                        break;
                    }
                case global::Doroti.Framework.Rendering.SelectionResult.previous:
                    {
                        if ((forward__43403 ?? false))
                        {
                            newIndex__42893 -= 1L;
                            finalResult__42028 = global::Doroti.Framework.Rendering.SelectionResult.end;
                        }
                        else
                        {
                            if ((newIndex__42893 == 0L))
                            {
                                finalResult__42028 = currentSelectableResult__43437;
                            }
                            else
                            {
                                forward__43403 = false;
                                newIndex__42893 -= 1L;
                            }
                        }
                        break;
                    }
            }
        }
        if (isEnd)
        {
            bool forwardSelection__45267 = (this.currentSelectionEndIndex >= this.currentSelectionStartIndex);
            if (((forward__43403 is not null) && (((((!forwardSelection__45267 && DartRuntimePrimitives.RequireValue(forward__43403)) && (newIndex__42893 >= this.currentSelectionStartIndex))) || (((forwardSelection__45267 && !DartRuntimePrimitives.RequireValue(forward__43403)) && (newIndex__42893 <= this.currentSelectionStartIndex)))))))
            {
                bool forward__43403__value45352 = DartRuntimePrimitives.RequireValue(forward__43403);
                currentSelectionStartIndex = this.currentSelectionEndIndex;
            }
            currentSelectionEndIndex = newIndex__42893;
        }
        else
        {
            bool forwardSelection__45693 = (this.currentSelectionEndIndex >= this.currentSelectionStartIndex);
            if (((forward__43403 is not null) && (((((!forwardSelection__45693 && !DartRuntimePrimitives.RequireValue(forward__43403)) && (newIndex__42893 <= this.currentSelectionEndIndex))) || (((forwardSelection__45693 && DartRuntimePrimitives.RequireValue(forward__43403)) && (newIndex__42893 >= this.currentSelectionEndIndex)))))))
            {
                bool forward__43403__value45778 = DartRuntimePrimitives.RequireValue(forward__43403);
                currentSelectionEndIndex = this.currentSelectionStartIndex;
            }
            currentSelectionStartIndex = newIndex__42893;
        }
        _flushInactiveSelections();
        return DartRuntimePrimitives.RequireValue(finalResult__42028);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Comparison<global::Doroti.Framework.Rendering.Selectable> compareOrder => new Comparison<global::Doroti.Framework.Rendering.Selectable>((left, right) => checked((int)_compareScreenOrder(left, right)));
    internal static long _compareScreenOrder(global::Doroti.Framework.Rendering.Selectable a, global::Doroti.Framework.Rendering.Selectable b)
    {
        global::Doroti.Ui.Rect rectA__46595 = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(a.getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null)), ((global::Doroti.Framework.Rendering.Selectable)a).boundingBoxes.First()));
        global::Doroti.Ui.Rect rectB__46692 = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(b.getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null)), ((global::Doroti.Framework.Rendering.Selectable)b).boundingBoxes.First()));
        long result__46788 = _SelectableTextContainerDelegate__text._compareVertically(rectA__46595, rectB__46692);
        if ((result__46788 != 0L))
        {
            return result__46788;
        }
        return _SelectableTextContainerDelegate__text._compareHorizontally(rectA__46595, rectB__46692);
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
        var startOffset__49013 = 0L;
        var endOffset__49038 = 0L;
        var foundStart__49061 = false;
        bool forwardSelection__49090 = (this.currentSelectionEndIndex >= this.currentSelectionStartIndex);
        if ((this.currentSelectionEndIndex == this.currentSelectionStartIndex))
        {
            global::Doroti.Framework.Rendering.SelectedContentRange rangeAtSelectableInSelection__49490 = this.selectables[(int)(this.currentSelectionStartIndex)].getSelection()!;
            forwardSelection__49090 = (((global::Doroti.Framework.Rendering.SelectedContentRange)rangeAtSelectableInSelection__49490).endOffset >= ((global::Doroti.Framework.Rendering.SelectedContentRange)rangeAtSelectableInSelection__49490).startOffset);
        }
        for (var index__49726 = 0L; (index__49726 < checked((long)(selections.Count))); index__49726++)
        {
            (long contentLength, global::Doroti.Framework.Rendering.SelectedContentRange? range) selection__49802 = selections[(int)(index__49726)];
            if ((selection__49802.range is null))
            {
                if (foundStart__49061)
                {
                    return new global::Doroti.Framework.Rendering.SelectedContentRange(startOffset: (forwardSelection__49090 ? startOffset__49013 : endOffset__49038), endOffset: (forwardSelection__49090 ? endOffset__49038 : startOffset__49013));
                }
                startOffset__49013 += selection__49802.contentLength;
                endOffset__49038 = startOffset__49013;
                continue;
            }
            long selectionStartNormalized__50217 = Math.Min(selection__49802.range!.startOffset, selection__49802.range!.endOffset);
            long selectionEndNormalized__50348 = Math.Max(selection__49802.range!.startOffset, selection__49802.range!.endOffset);
            if (!foundStart__49061)
            {
                bool shouldConsiderContentStart__50756 = ((index__49726 > 0L) && this.paragraph.selectableBelongsToParagraph(this.selectables[(int)(index__49726)]));
                startOffset__49013 += ((selectionStartNormalized__50217 - ((shouldConsiderContentStart__50756 ? this.paragraph.getPositionForOffset(this.selectables[(int)(index__49726)].boundingBoxes.First().centerLeft).offset : 0L)))).abs();
                endOffset__49038 = (startOffset__49013 + ((selectionEndNormalized__50348 - selectionStartNormalized__50217)).abs());
                foundStart__49061 = true;
            }
            else
            {
                endOffset__49038 += ((selectionEndNormalized__50348 - selectionStartNormalized__50217)).abs();
            }
        }
        DartRuntimePrimitives.Assert(() => foundStart__49061, () => (object?)"The start of the selection has not been found despite this selection delegate having an existing currentSelectionStartIndex and currentSelectionEndIndex.");
        return new global::Doroti.Framework.Rendering.SelectedContentRange(startOffset: (forwardSelection__49090 ? startOffset__49013 : endOffset__49038), endOffset: (forwardSelection__49090 ? endOffset__49038 : startOffset__49013));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Rendering.SelectedContentRange? getSelection()
    {
        var selections__52198 = new List<(long contentLength, global::Doroti.Framework.Rendering.SelectedContentRange? range)>();
        return ((global::Doroti.Framework.Rendering.SelectedContentRange?)(object?)_calculateLocalRange(selections__52198));
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
            long skipIndex__53020 = ((this.currentSelectionStartIndex == -1L) ? this.currentSelectionEndIndex : this.currentSelectionStartIndex);
            for (var i__53157 = 0L; (i__53157 < checked((long)(this.selectables.Count))); i__53157++)
            {
                if ((i__53157 == skipIndex__53020))
                {
                    continue;
                }
                dispatchSelectionEventToChild(this.selectables[(int)(i__53157)], new global::Doroti.Framework.Rendering.ClearSelectionEvent());
            }
            return;
        }
        long skipStart__53381 = Math.Min(this.currentSelectionStartIndex, this.currentSelectionEndIndex);
        long skipEnd__53466 = Math.Max(this.currentSelectionStartIndex, this.currentSelectionEndIndex);
        for (var index__53548 = 0L; (index__53548 < checked((long)(this.selectables.Count))); index__53548 += 1L)
        {
            if (((index__53548 >= skipStart__53381) && (index__53548 <= skipEnd__53466)))
            {
                continue;
            }
            dispatchSelectionEventToChild(this.selectables[(int)(index__53548)], new global::Doroti.Framework.Rendering.ClearSelectionEvent());
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
        return new global::Doroti.Framework.Painting.TextSpan(text: ((global::Doroti.Framework.Painting.TextSpan)textSpan).text, children: ((global::Doroti.Framework.Painting.TextSpan)textSpan).children?.map<global::Doroti.Framework.Painting.InlineSpan, global::Doroti.Framework.Painting.InlineSpan>(((child) => {
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

