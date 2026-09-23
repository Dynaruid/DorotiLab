// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/text_selection.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class Text_selectionLibrary
{
    internal static double _kHandleSize = 22.0;
}

public static partial class Text_selectionLibrary
{
    internal static double _kToolbarContentDistanceBelow = _kHandleSize - 2.0;
}

public static partial class Text_selectionLibrary
{
    internal static double _kToolbarContentDistance = 8.0;
}

public class MaterialTextSelectionHandleControls
    : MaterialTextSelectionControls,
        TextSelectionHandleControls
{
    public override Widget buildToolbar(
        BuildContext context,
        Rect globalEditableRegion,
        double textLineHeight,
        Offset selectionMidpoint,
        List<TextSelectionPoint> endpoints,
        TextSelectionDelegate @delegate,
        ValueListenable<ClipboardStatus>? clipboardStatus,
        Offset? lastSecondaryTapDownPosition
    ) => DartRuntimePrimitives.ConvertValue<Widget>(SizedBox.CreateShrink());

    public override bool canCut(TextSelectionDelegate @delegate) => false;

    public override bool canCopy(TextSelectionDelegate @delegate) => false;

    public override bool canPaste(TextSelectionDelegate @delegate) => false;

    public override bool canSelectAll(TextSelectionDelegate @delegate) => false;

    public override void handleCut(TextSelectionDelegate @delegate) { }

    public virtual void handleCut(
        TextSelectionDelegate @delegate,
        ClipboardStatusNotifier? clipboardStatus = null
    ) { }

    public override void handleCopy(TextSelectionDelegate @delegate) { }

    public virtual void handleCopy(
        TextSelectionDelegate @delegate,
        ClipboardStatusNotifier? clipboardStatus = null
    ) { }

    public override async Future handlePaste(TextSelectionDelegate @delegate) { }

    public override void handleSelectAll(TextSelectionDelegate @delegate) { }
}

public class MaterialTextSelectionControls : TextSelectionControls
{
    public override Size getHandleSize(double textLineHeight) =>
        new Size(Text_selectionLibrary._kHandleSize, Text_selectionLibrary._kHandleSize);

    public override Widget buildToolbar(
        BuildContext context,
        Rect globalEditableRegion,
        double textLineHeight,
        Offset selectionMidpoint,
        List<TextSelectionPoint> endpoints,
        TextSelectionDelegate @delegate,
        ValueListenable<ClipboardStatus>? clipboardStatus,
        Offset? lastSecondaryTapDownPosition
    )
    {
        return new _TextSelectionControlsToolbar__text_selection(
            globalEditableRegion: globalEditableRegion,
            textLineHeight: textLineHeight,
            selectionMidpoint: selectionMidpoint,
            endpoints: endpoints,
            @delegate: @delegate,
            clipboardStatus: clipboardStatus,
            handleCut: canCut(@delegate)
                ? (
                    () =>
                    {
                        handleCut(@delegate);
                    }
                )
                : null,
            handleCopy: canCopy(@delegate)
                ? (
                    () =>
                    {
                        handleCopy(@delegate);
                    }
                )
                : null,
            handlePaste: canPaste(@delegate)
                ? (
                    () =>
                    {
                        _ = handlePaste(@delegate);
                    }
                )
                : null,
            handleSelectAll: canSelectAll(@delegate)
                ? (
                    () =>
                    {
                        handleSelectAll(@delegate);
                    }
                )
                : null
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget buildHandle(
        BuildContext context,
        TextSelectionHandleType type,
        double textLineHeight,
        Action? onTap = null
    )
    {
        ThemeData theme = Theme.of(context);
        Color handleColor =
            TextSelectionTheme.of(context).selectionHandleColor ?? theme.colorScheme.primary;
        Widget handle = SizedBox.CreateSquare(
            dimension: Text_selectionLibrary._kHandleSize,
            child: new CustomPaint(
                painter: new _TextSelectionHandlePainter__text_selection(color: handleColor),
                child: new GestureDetector(onTap: onTap, behavior: HitTestBehavior.translucent)
            )
        );
        return type switch
        {
            TextSelectionHandleType.left => DartRuntimePrimitives.ConvertValue<Widget>(
                Transform.CreateRotate(angle: Math.PI / 2.0, child: handle)
            ),
            TextSelectionHandleType.right => handle,
            TextSelectionHandleType.collapsed => DartRuntimePrimitives.ConvertValue<Widget>(
                Transform.CreateRotate(angle: Math.PI / 4.0, child: handle)
            ),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Offset getHandleAnchor(TextSelectionHandleType type, double textLineHeight)
    {
        return type switch
        {
            TextSelectionHandleType.collapsed => new Offset(
                Text_selectionLibrary._kHandleSize / 2L,
                -4
            ),
            TextSelectionHandleType.left => new Offset(Text_selectionLibrary._kHandleSize, 0),
            TextSelectionHandleType.right => Offset.zero,
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool canSelectAll(TextSelectionDelegate @delegate)
    {
        TextEditingValue value = @delegate.textEditingValue;
        return @delegate.selectAllEnabled
            && (value.text.Length != 0)
            && !((value.selection.start == 0L) && (value.selection.end == value.text.Length));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _TextSelectionToolbarItemData__text_selection
{
    public virtual string label { get; private set; } = default!;
    public virtual Action onPressed { get; private set; } = default!;

    internal _TextSelectionToolbarItemData__text_selection(string label, Action onPressed)
    {
        this.label = label;
        this.onPressed = onPressed;
    }
}

public class _TextSelectionControlsToolbar__text_selection : StatefulWidget
{
    public virtual ValueListenable<ClipboardStatus>? clipboardStatus { get; private set; }
    public virtual TextSelectionDelegate @delegate { get; private set; } = default!;
    public virtual List<TextSelectionPoint> endpoints { get; private set; } = default!;
    public virtual Rect globalEditableRegion { get; private set; } = default!;
    public virtual Action? handleCut { get; private set; }
    public virtual Action? handleCopy { get; private set; }
    public virtual Action? handlePaste { get; private set; }
    public virtual Action? handleSelectAll { get; private set; }
    public virtual Offset selectionMidpoint { get; private set; } = default!;
    public virtual double textLineHeight { get; private set; } = default!;

    internal _TextSelectionControlsToolbar__text_selection(
        ValueListenable<ClipboardStatus>? clipboardStatus,
        TextSelectionDelegate @delegate,
        List<TextSelectionPoint> endpoints,
        Rect globalEditableRegion,
        Action? handleCut,
        Action? handleCopy,
        Action? handlePaste,
        Action? handleSelectAll,
        Offset selectionMidpoint,
        double textLineHeight
    )
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

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _TextSelectionControlsToolbarState__text_selection()
        );
}

public class _TextSelectionControlsToolbarState__text_selection
    : State<_TextSelectionControlsToolbar__text_selection>,
        TickerProviderStateMixin<_TextSelectionControlsToolbar__text_selection>
{
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual void _onChangedClipboardStatus()
    {
        setState(() => { });
    }

    public override void initState()
    {
        base.initState();
        widget.clipboardStatus?.addListener(_onChangedClipboardStatus);
    }

    public override void didUpdateWidget(_TextSelectionControlsToolbar__text_selection oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(widget.clipboardStatus, oldWidget.clipboardStatus))
        {
            widget.clipboardStatus?.addListener(_onChangedClipboardStatus);
            oldWidget.clipboardStatus?.removeListener(_onChangedClipboardStatus);
        }
    }

    public override void dispose()
    {
        widget.clipboardStatus?.removeListener(_onChangedClipboardStatus);
        DartRuntimePrimitives.Assert(() =>
        {
            if (_tickers is not null)
            {
                foreach (Scheduler.Ticker ticker in _tickers!)
                {
                    if (ticker.isActive)
                    {
                        throw DartRuntimePrimitives.AsException(
                            new FlutterError(
                                new List<DiagnosticsNode>
                                {
                                    new ErrorSummary($"{this} was disposed with an active Ticker."),
                                    new ErrorDescription(
                                        $"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time "
                                            + "dispose() was called on the mixin, that Ticker was still active. All Tickers must "
                                            + "be disposed before calling super.dispose()."
                                    ),
                                    new ErrorHint(
                                        "Tickers used by AnimationControllers "
                                            + "should be disposed by calling dispose() on the AnimationController itself. "
                                            + "Otherwise, the ticker will leak."
                                    ),
                                    ticker.describeForError("The offending ticker was"),
                                }
                            )
                        );
                    }
                }
            }
            return true;
        });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        if (
            (widget.handleCut is null)
            && (widget.handleCopy is null)
            && (widget.handlePaste is null)
            && (widget.handleSelectAll is null)
        )
        {
            return SizedBox.CreateShrink();
        }
        if (
            (widget.handlePaste is not null)
            && Equals(widget.clipboardStatus?.value, ClipboardStatus.unknown)
        )
        {
            return SizedBox.CreateShrink();
        }
        TextSelectionPoint startTextSelectionPoint = widget.endpoints[(int)0L];
        TextSelectionPoint endTextSelectionPoint =
            (checked(widget.endpoints.Count) > 1L)
                ? widget.endpoints[(int)1L]
                : widget.endpoints[(int)0L];
        double topAmountInEditableRegion = startTextSelectionPoint.point.dy - widget.textLineHeight;
        double anchorTop =
            Math.Max(topAmountInEditableRegion, 0L)
            + widget.globalEditableRegion.top
            - Text_selectionLibrary._kToolbarContentDistance;
        var anchorAboveLocal = new Offset(
            widget.globalEditableRegion.left + widget.selectionMidpoint.dx,
            anchorTop
        );
        var anchorBelowLocal = new Offset(
            widget.globalEditableRegion.left + widget.selectionMidpoint.dx,
            widget.globalEditableRegion.top
                + endTextSelectionPoint.point.dy
                + Text_selectionLibrary._kToolbarContentDistanceBelow
        );
        DartRuntimePrimitives.Assert(() =>
            DebugLibrary.debugCheckHasMaterialLocalizations(context)
        );
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        var itemDatas = (
            (Func<List<_TextSelectionToolbarItemData__text_selection>>)(
                () =>
                {
                    var __collection8792 =
                        new List<_TextSelectionToolbarItemData__text_selection>();
                    if (widget.handleCut is not null)
                    {
                        __collection8792.Add(
                            new _TextSelectionToolbarItemData__text_selection(
                                label: localizations.cutButtonLabel,
                                onPressed: widget.handleCut!
                            )
                        );
                    }
                    if (widget.handleCopy is not null)
                    {
                        __collection8792.Add(
                            new _TextSelectionToolbarItemData__text_selection(
                                label: localizations.copyButtonLabel,
                                onPressed: widget.handleCopy!
                            )
                        );
                    }
                    if (
                        (widget.handlePaste is not null)
                        && Equals(widget.clipboardStatus?.value, ClipboardStatus.pasteable)
                    )
                    {
                        __collection8792.Add(
                            new _TextSelectionToolbarItemData__text_selection(
                                label: localizations.pasteButtonLabel,
                                onPressed: widget.handlePaste!
                            )
                        );
                    }
                    if (widget.handleSelectAll is not null)
                    {
                        __collection8792.Add(
                            new _TextSelectionToolbarItemData__text_selection(
                                label: localizations.selectAllButtonLabel,
                                onPressed: widget.handleSelectAll!
                            )
                        );
                    }
                    return __collection8792;
                }
            )
        )();
        if (!Enumerable.Any(itemDatas))
        {
            return SizedBox.CreateShrink();
        }
        return new TextSelectionToolbar(
            anchorAbove: anchorAboveLocal,
            anchorBelow: anchorBelowLocal,
            children: itemDatas
                .asMap()
                .entries.map(
                    (entry) =>
                    {
                        return new TextSelectionToolbarTextButton(
                            padding: TextSelectionToolbarTextButton.getPadding(
                                entry.key,
                                checked(itemDatas.Count)
                            ),
                            alignment: AlignmentDirectional.centerStart,
                            onPressed: () => entry.value.onPressed(),
                            child: new Text(entry.value.label)
                        );
                        throw new InvalidOperationException(
                            "Callback completed without returning a value."
                        );
                    }
                )
                .ToList()
                .Cast<Widget>()
                .ToList()
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Scheduler.Ticker createTicker(Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = (
            (Func<_WidgetTicker__ticker_provider>)(
                () =>
                {
                    var __cascade = new _WidgetTicker__ticker_provider(
                        onTick,
                        this,
                        debugLabel: Foundation.ConstantsLibrary.kDebugMode
                            ? $"created by {DiagnosticsLibrary.describeIdentity(this)}"
                            : null
                    );
                    __cascade.muted = !values.enabled;
                    __cascade.forceFrames = values.forceFrames;
                    return __cascade;
                }
            )
        )();
        _tickers!.Add(result);
        return result;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void _removeTicker(_WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => _tickers is not null);
        DartRuntimePrimitives.Assert(() => _tickers!.Contains(ticker));
        _tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if (_tickers is not null)
        {
            TickerModeData values = _tickerModeNotifier!.value;
            bool mutedLocal = !values.enabled;
            foreach (Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new DiagnosticsProperty<HashSet<Scheduler.Ticker>>(
                "tickers",
                _tickers,
                description: (_tickers is not null)
                    ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}"
                    : null,
                defaultValue: default
            )
        );
    }
}

internal class _TextSelectionHandlePainter__text_selection : CustomPainter
{
    public virtual Color color { get; private set; } = default!;

    internal _TextSelectionHandlePainter__text_selection(Color color)
    {
        this.color = color;
    }

    public override void paint(Canvas canvas, Size size)
    {
        var paintLocal = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.color = color;
                    return __cascade;
                }
            )
        )();
        double radiusLocal = size.width / 2.0;
        var circle = Rect.fromCircle(
            center: new Offset(radiusLocal, radiusLocal),
            radius: radiusLocal
        );
        var point = Rect.fromLTWH(0.0, 0.0, radiusLocal, radiusLocal);
        var path = (
            (Func<Path>)(
                () =>
                {
                    var __cascade = new Path();
                    __cascade.addOval(circle);
                    __cascade.addRect(point);
                    return __cascade;
                }
            )
        )();
        canvas.drawPath(path, paintLocal);
    }

    public override bool shouldRepaint(CustomPainter oldDelegate)
    {
        var __oldPainter = (_TextSelectionHandlePainter__text_selection)oldDelegate;
        return !Equals(color, __oldPainter.color);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public static partial class Text_selectionLibrary
{
    public static TextSelectionControls materialTextSelectionHandleControls =
        new MaterialTextSelectionHandleControls();
}

public static partial class Text_selectionLibrary
{
    public static TextSelectionControls materialTextSelectionControls =
        new MaterialTextSelectionControls();
}
