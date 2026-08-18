// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/text_selection.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Material;

public static partial class Text_selectionLibrary
{
    internal static double _kHandleSize = 22.0;
}

public static partial class Text_selectionLibrary
{
    internal static double _kToolbarContentDistanceBelow = (Text_selectionLibrary._kHandleSize - 2.0);
}

public static partial class Text_selectionLibrary
{
    internal static double _kToolbarContentDistance = 8.0;
}

public class MaterialTextSelectionHandleControls : MaterialTextSelectionControls
{

    public override Widget buildToolbar(BuildContext context, Rect globalEditableRegion, double textLineHeight, Offset selectionMidpoint, List<global::Doroti.Framework.Rendering.TextSelectionPoint> endpoints, global::Doroti.Framework.Services.TextSelectionDelegate @delegate, global::Doroti.Framework.Foundation.ValueListenable<ClipboardStatus>? clipboardStatus, Offset? lastSecondaryTapDownPosition) => DartRuntimePrimitives.ConvertValue<Widget>(SizedBox.CreateShrink());
    public override bool canCut(global::Doroti.Framework.Services.TextSelectionDelegate @delegate) => false;
    public override bool canCopy(global::Doroti.Framework.Services.TextSelectionDelegate @delegate) => false;
    public override bool canPaste(global::Doroti.Framework.Services.TextSelectionDelegate @delegate) => false;
    public override bool canSelectAll(global::Doroti.Framework.Services.TextSelectionDelegate @delegate) => false;
    public virtual void handleCut(global::Doroti.Framework.Services.TextSelectionDelegate @delegate, ClipboardStatusNotifier? clipboardStatus = null)
    {
    }

    public virtual void handleCopy(global::Doroti.Framework.Services.TextSelectionDelegate @delegate, ClipboardStatusNotifier? clipboardStatus = null)
    {
    }

    public async override Future handlePaste(global::Doroti.Framework.Services.TextSelectionDelegate @delegate)
    {
    }

    public override void handleSelectAll(global::Doroti.Framework.Services.TextSelectionDelegate @delegate)
    {
    }

}

public class MaterialTextSelectionControls : global::Doroti.Framework.Widgets.TextSelectionControls
{
    public override Size getHandleSize(double textLineHeight) => new global::Doroti.Ui.Size(Text_selectionLibrary._kHandleSize, Text_selectionLibrary._kHandleSize);
    public override global::Doroti.Framework.Widgets.Widget buildToolbar(global::Doroti.Framework.Widgets.BuildContext context, Rect globalEditableRegion, double textLineHeight, Offset selectionMidpoint, List<global::Doroti.Framework.Rendering.TextSelectionPoint> endpoints, global::Doroti.Framework.Services.TextSelectionDelegate @delegate, global::Doroti.Framework.Foundation.ValueListenable<global::Doroti.Framework.Widgets.ClipboardStatus>? clipboardStatus, Offset? lastSecondaryTapDownPosition)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _TextSelectionControlsToolbar__text_selection(globalEditableRegion: globalEditableRegion, textLineHeight: textLineHeight, selectionMidpoint: selectionMidpoint, endpoints: endpoints, @delegate: @delegate, clipboardStatus: clipboardStatus, handleCut: ((global::System.Action)(canCut(@delegate) ? (() => { handleCut(@delegate); }) : null)), handleCopy: ((global::System.Action)(canCopy(@delegate) ? (() => { handleCopy(@delegate); }) : null)), handlePaste: ((global::System.Action)(canPaste(@delegate) ? (() => { _ = handlePaste(@delegate); }) : null)), handleSelectAll: ((global::System.Action)(canSelectAll(@delegate) ? (() => { handleSelectAll(@delegate); }) : null))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget buildHandle(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.TextSelectionHandleType type, double textLineHeight, global::System.Action? onTap = null)
    {
        ThemeData theme__2772 = Theme.of(context);
        global::Doroti.Ui.Color handleColor__2815 = ((global::Doroti.Ui.Color)(object?)(TextSelectionTheme.of(context).selectionHandleColor ?? theme__2772.colorScheme.primary));
        global::Doroti.Framework.Widgets.Widget handle__2936 = ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.SizedBox.CreateSquare(dimension: Text_selectionLibrary._kHandleSize, child: new global::Doroti.Framework.Widgets.CustomPaint(painter: new _TextSelectionHandlePainter__text_selection(color: handleColor__2815), child: new global::Doroti.Framework.Widgets.GestureDetector(onTap: onTap, behavior: global::Doroti.Framework.Rendering.HitTestBehavior.translucent))));
        return (type switch { global::Doroti.Framework.Rendering.TextSelectionHandleType.left => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(global::Doroti.Framework.Widgets.Transform.CreateRotate(angle: (Dart_mathLibrary.pi / 2.0), child: handle__2936)), global::Doroti.Framework.Rendering.TextSelectionHandleType.right => handle__2936, global::Doroti.Framework.Rendering.TextSelectionHandleType.collapsed => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(global::Doroti.Framework.Widgets.Transform.CreateRotate(angle: (Dart_mathLibrary.pi / 4.0), child: handle__2936)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Offset getHandleAnchor(global::Doroti.Framework.Rendering.TextSelectionHandleType type, double textLineHeight)
    {
        return (type switch { global::Doroti.Framework.Rendering.TextSelectionHandleType.collapsed => new global::Doroti.Ui.Offset((Text_selectionLibrary._kHandleSize / 2L), -4), global::Doroti.Framework.Rendering.TextSelectionHandleType.left => new global::Doroti.Ui.Offset(Text_selectionLibrary._kHandleSize, 0), global::Doroti.Framework.Rendering.TextSelectionHandleType.right => Offset.zero, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool canSelectAll(global::Doroti.Framework.Services.TextSelectionDelegate @delegate)
    {
        global::Doroti.Framework.Services.TextEditingValue value__4552 = ((global::Doroti.Framework.Services.TextSelectionDelegate)@delegate).textEditingValue;
        return ((((global::Doroti.Framework.Services.TextSelectionDelegate)@delegate).selectAllEnabled && (((global::Doroti.Framework.Services.TextEditingValue)value__4552).text.Length != 0)) && !(((((global::Doroti.Framework.Services.TextEditingValue)value__4552).selection.start == 0L) && (((global::Doroti.Framework.Services.TextEditingValue)value__4552).selection.end == ((global::Doroti.Framework.Services.TextEditingValue)value__4552).text.Length))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TextSelectionToolbarItemData__text_selection
{
    public virtual string label { get; private set; } = default!;
    public virtual global::System.Action onPressed { get; private set; } = default!;

    internal _TextSelectionToolbarItemData__text_selection(string label, global::System.Action onPressed)
    {
        this.label = label;
        this.onPressed = onPressed;
    }

}

public class _TextSelectionControlsToolbar__text_selection : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Foundation.ValueListenable<global::Doroti.Framework.Widgets.ClipboardStatus>? clipboardStatus { get; private set; }
    public virtual global::Doroti.Framework.Services.TextSelectionDelegate @delegate { get; private set; } = default!;
    public virtual List<global::Doroti.Framework.Rendering.TextSelectionPoint> endpoints { get; private set; } = default!;
    public virtual Rect globalEditableRegion { get; private set; } = default!;
    public virtual global::System.Action? handleCut { get; private set; }
    public virtual global::System.Action? handleCopy { get; private set; }
    public virtual global::System.Action? handlePaste { get; private set; }
    public virtual global::System.Action? handleSelectAll { get; private set; }
    public virtual Offset selectionMidpoint { get; private set; } = default!;
    public virtual double textLineHeight { get; private set; } = default!;

    internal _TextSelectionControlsToolbar__text_selection(global::Doroti.Framework.Foundation.ValueListenable<global::Doroti.Framework.Widgets.ClipboardStatus>? clipboardStatus, global::Doroti.Framework.Services.TextSelectionDelegate @delegate, List<global::Doroti.Framework.Rendering.TextSelectionPoint> endpoints, Rect globalEditableRegion, global::System.Action? handleCut, global::System.Action? handleCopy, global::System.Action? handlePaste, global::System.Action? handleSelectAll, Offset selectionMidpoint, double textLineHeight)
    {
        this.clipboardStatus = clipboardStatus;
        this.@delegate = @delegate;
        this.endpoints = endpoints;
        this.globalEditableRegion = globalEditableRegion;
        this.handleCut = handleCut;
        this.handleCopy = handleCopy;
        this.handlePaste = handlePaste;
        this.handleSelectAll = handleSelectAll;
        this.selectionMidpoint = selectionMidpoint;
        this.textLineHeight = textLineHeight;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _TextSelectionControlsToolbarState__text_selection());
}

public class _TextSelectionControlsToolbarState__text_selection : global::Doroti.Framework.Widgets.State<_TextSelectionControlsToolbar__text_selection>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<_TextSelectionControlsToolbar__text_selection>
{
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual void _onChangedClipboardStatus()
    {
        setState(((global::System.Action)(() =>
        {
        })));
    }

    public override void initState()
    {
        base.initState();
        ((_TextSelectionControlsToolbar__text_selection)this.widget).clipboardStatus?.addListener(() => this._onChangedClipboardStatus());
    }

    public override void didUpdateWidget(_TextSelectionControlsToolbar__text_selection oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((_TextSelectionControlsToolbar__text_selection)this.widget).clipboardStatus, ((_TextSelectionControlsToolbar__text_selection)oldWidget).clipboardStatus)))
        {
            ((_TextSelectionControlsToolbar__text_selection)this.widget).clipboardStatus?.addListener(() => this._onChangedClipboardStatus());
            ((_TextSelectionControlsToolbar__text_selection)oldWidget).clipboardStatus?.removeListener(() => this._onChangedClipboardStatus());
        }
    }

    public override void dispose()
    {
        ((_TextSelectionControlsToolbar__text_selection)this.widget).clipboardStatus?.removeListener(() => this._onChangedClipboardStatus());
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker__18989 in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker__18989).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker__18989.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if (((((((_TextSelectionControlsToolbar__text_selection)this.widget).handleCut is null) && (((_TextSelectionControlsToolbar__text_selection)this.widget).handleCopy is null)) && (((_TextSelectionControlsToolbar__text_selection)this.widget).handlePaste is null)) && (((_TextSelectionControlsToolbar__text_selection)this.widget).handleSelectAll is null)))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.SizedBox.CreateShrink());
        }
        if (((((_TextSelectionControlsToolbar__text_selection)this.widget).handlePaste is not null) && (object.Equals(((_TextSelectionControlsToolbar__text_selection)this.widget).clipboardStatus?.value, global::Doroti.Framework.Widgets.ClipboardStatus.unknown))))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.SizedBox.CreateShrink());
        }
        global::Doroti.Framework.Rendering.TextSelectionPoint startTextSelectionPoint__7645 = ((_TextSelectionControlsToolbar__text_selection)this.widget).endpoints[(int)(0L)];
        global::Doroti.Framework.Rendering.TextSelectionPoint endTextSelectionPoint__7721 = ((checked((long)(((_TextSelectionControlsToolbar__text_selection)this.widget).endpoints.Count)) > 1L) ? ((_TextSelectionControlsToolbar__text_selection)this.widget).endpoints[(int)(1L)] : ((_TextSelectionControlsToolbar__text_selection)this.widget).endpoints[(int)(0L)]);
        double topAmountInEditableRegion__7851 = (((global::Doroti.Framework.Rendering.TextSelectionPoint)startTextSelectionPoint__7645).point.dy - ((_TextSelectionControlsToolbar__text_selection)this.widget).textLineHeight);
        double anchorTop__7962 = ((Math.Max(topAmountInEditableRegion__7851, 0L) + ((_TextSelectionControlsToolbar__text_selection)this.widget).globalEditableRegion.top) - Text_selectionLibrary._kToolbarContentDistance);
        var anchorAbove__8110 = new global::Doroti.Ui.Offset((((_TextSelectionControlsToolbar__text_selection)this.widget).globalEditableRegion.left + ((_TextSelectionControlsToolbar__text_selection)this.widget).selectionMidpoint.dx), anchorTop__7962);
        var anchorBelow__8236 = new global::Doroti.Ui.Offset((((_TextSelectionControlsToolbar__text_selection)this.widget).globalEditableRegion.left + ((_TextSelectionControlsToolbar__text_selection)this.widget).selectionMidpoint.dx), ((((_TextSelectionControlsToolbar__text_selection)this.widget).globalEditableRegion.top + ((global::Doroti.Framework.Rendering.TextSelectionPoint)endTextSelectionPoint__7721).point.dy) + Text_selectionLibrary._kToolbarContentDistanceBelow));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        MaterialLocalizations localizations__8719 = MaterialLocalizations.of(context);
        var itemDatas__8780 = ((Func<List<_TextSelectionToolbarItemData__text_selection>>)(() => { var __collection8792 = new List<_TextSelectionToolbarItemData__text_selection>(); if ((((_TextSelectionControlsToolbar__text_selection)this.widget).handleCut is not null)) { __collection8792.Add(new _TextSelectionToolbarItemData__text_selection(label: localizations__8719.cutButtonLabel, onPressed: ((_TextSelectionControlsToolbar__text_selection)this.widget).handleCut!)); } if ((((_TextSelectionControlsToolbar__text_selection)this.widget).handleCopy is not null)) { __collection8792.Add(new _TextSelectionToolbarItemData__text_selection(label: localizations__8719.copyButtonLabel, onPressed: ((_TextSelectionControlsToolbar__text_selection)this.widget).handleCopy!)); } if (((((_TextSelectionControlsToolbar__text_selection)this.widget).handlePaste is not null) && (object.Equals(((_TextSelectionControlsToolbar__text_selection)this.widget).clipboardStatus?.value, global::Doroti.Framework.Widgets.ClipboardStatus.pasteable)))) { __collection8792.Add(new _TextSelectionToolbarItemData__text_selection(label: localizations__8719.pasteButtonLabel, onPressed: ((_TextSelectionControlsToolbar__text_selection)this.widget).handlePaste!)); } if ((((_TextSelectionControlsToolbar__text_selection)this.widget).handleSelectAll is not null)) { __collection8792.Add(new _TextSelectionToolbarItemData__text_selection(label: localizations__8719.selectAllButtonLabel, onPressed: ((_TextSelectionControlsToolbar__text_selection)this.widget).handleSelectAll!)); } return __collection8792; }))();
        if (!System.Linq.Enumerable.Any(itemDatas__8780))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.SizedBox.CreateShrink());
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new TextSelectionToolbar(anchorAbove: anchorAbove__8110, anchorBelow: anchorBelow__8236, children: itemDatas__8780.asMap().entries.map<MapEntry<long, _TextSelectionToolbarItemData__text_selection>, TextSelectionToolbarTextButton>(((entry) =>
        {
            return new TextSelectionToolbarTextButton(padding: TextSelectionToolbarTextButton.getPadding(entry.key, checked((long)(itemDatas__8780.Count))), alignment: global::Doroti.Framework.Painting.AlignmentDirectional.centerStart, onPressed: () => entry.value.onPressed(), child: new global::Doroti.Framework.Widgets.Text(entry.value.label));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })).ToList().Cast<global::Doroti.Framework.Widgets.Widget>().ToList()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values__17506 = this._tickerModeNotifier!.value;
        var result__17553 = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
    __cascade.muted = !((TickerModeData)values__17506).enabled;
    __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
    return __cascade;
}))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result__17553);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => (this._tickers is not null));
        DartRuntimePrimitives.Assert(() => this._tickers!.Contains(ticker));
        this._tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if ((this._tickers is not null))
        {
            TickerModeData values__18318 = this._tickerModeNotifier!.value;
            bool muted__18372 = !((TickerModeData)values__18318).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker__18421 in this._tickers!)
            {
                ticker__18421.muted = muted__18372;
                ticker__18421.forceFrames = ((TickerModeData)values__18318).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__18621 = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__18621, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier__18621.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier__18621;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

internal class _TextSelectionHandlePainter__text_selection : global::Doroti.Framework.Rendering.CustomPainter
{
    public virtual Color color { get; private set; } = default!;

    internal _TextSelectionHandlePainter__text_selection(Color color)
    {
        this.color = color;
    }

    public override void paint(Canvas canvas, Size size)
    {
        var paint__10544 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = this.color;
    return __cascade;
}))();
        double radius__10593 = (size.width / 2.0);
        var circle__10630 = global::Doroti.Ui.Rect.fromCircle(center: new global::Doroti.Ui.Offset(radius__10593, radius__10593), radius: radius__10593);
        var point__10714 = global::Doroti.Ui.Rect.fromLTWH(0.0, 0.0, radius__10593, radius__10593);
        var path__10773 = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addOval(circle__10630);
    __cascade.addRect(point__10714);
    return __cascade;
}))();
        canvas.drawPath(path__10773, paint__10544);
    }

    public override bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __oldPainter = (_TextSelectionHandlePainter__text_selection)(object)oldDelegate;
        return (!object.Equals(this.color, ((_TextSelectionHandlePainter__text_selection)__oldPainter).color));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Text_selectionLibrary
{
    public static global::Doroti.Framework.Widgets.TextSelectionControls materialTextSelectionHandleControls = ((global::Doroti.Framework.Widgets.TextSelectionControls)(object?)new MaterialTextSelectionHandleControls());
}

public static partial class Text_selectionLibrary
{
    public static global::Doroti.Framework.Widgets.TextSelectionControls materialTextSelectionControls = ((global::Doroti.Framework.Widgets.TextSelectionControls)(object?)new MaterialTextSelectionControls());
}
