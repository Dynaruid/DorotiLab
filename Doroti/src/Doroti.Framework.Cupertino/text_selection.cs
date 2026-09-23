// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/text_selection.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class Text_selectionLibrary
{
    internal static double _kSelectionHandleOverlap = 1.5;
}

public static partial class Text_selectionLibrary
{
    internal static double _kSelectionHandleRadius = 6;
}

public static partial class Text_selectionLibrary
{
    internal static double _kArrowScreenPadding = 26.0;
}

internal class _CupertinoTextSelectionHandlePainter__text_selection : CustomPainter
{
    public virtual Color color { get; private set; } = default!;

    internal _CupertinoTextSelectionHandlePainter__text_selection(Color color)
    {
        this.color = color;
    }

    public override void paint(Canvas canvas, Size size)
    {
        var halfStrokeWidth = 1.0;
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
        var circle = Rect.fromCircle(
            center: new Offset(
                Text_selectionLibrary._kSelectionHandleRadius,
                Text_selectionLibrary._kSelectionHandleRadius
            ),
            radius: Text_selectionLibrary._kSelectionHandleRadius
        );
        var line = Rect.fromPoints(
            new Offset(
                Text_selectionLibrary._kSelectionHandleRadius - halfStrokeWidth,
                (2L * Text_selectionLibrary._kSelectionHandleRadius)
                    - Text_selectionLibrary._kSelectionHandleOverlap
            ),
            new Offset(Text_selectionLibrary._kSelectionHandleRadius + halfStrokeWidth, size.height)
        );
        var path = (
            (Func<Path>)(
                () =>
                {
                    var __cascade = new Path();
                    __cascade.addOval(circle);
                    __cascade.addRect(line);
                    return __cascade;
                }
            )
        )();
        canvas.drawPath(path, paintLocal);
    }

    public override bool shouldRepaint(CustomPainter oldDelegate) =>
        !Equals(color, ((_CupertinoTextSelectionHandlePainter__text_selection)oldDelegate).color);
}

public class CupertinoTextSelectionHandleControls
    : CupertinoTextSelectionControls,
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

public class CupertinoTextSelectionControls : TextSelectionControls
{
    public override Size getHandleSize(double textLineHeight)
    {
        return new Size(
            Text_selectionLibrary._kSelectionHandleRadius * 2L,
            textLineHeight
                + (Text_selectionLibrary._kSelectionHandleRadius * 2L)
                - Text_selectionLibrary._kSelectionHandleOverlap
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

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
        return new _CupertinoTextSelectionControlsToolbar__text_selection(
            clipboardStatus: clipboardStatus,
            endpoints: endpoints,
            globalEditableRegion: globalEditableRegion,
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
                : null,
            selectionMidpoint: selectionMidpoint,
            textLineHeight: textLineHeight
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
        Size desiredSize = default!;
        Widget handle = default!;
        Widget customPaint = new CustomPaint(
            painter: new _CupertinoTextSelectionHandlePainter__text_selection(
                CupertinoTheme.of(context).selectionHandleColor
            )
        );
        switch (type)
        {
            case TextSelectionHandleType.left:
            {
                desiredSize = getHandleSize(textLineHeight);
                handle = DartRuntimePrimitives.ConvertValue<Widget>(
                    SizedBox.CreateFromSize(size: desiredSize, child: customPaint)
                );
                return handle;
            }
            case TextSelectionHandleType.right:
            {
                desiredSize = getHandleSize(textLineHeight);
                handle = DartRuntimePrimitives.ConvertValue<Widget>(
                    SizedBox.CreateFromSize(size: desiredSize, child: customPaint)
                );
                return new Transform(
                    transform: (
                        (Func<Matrix4>)(
                            () =>
                            {
                                var __cascade = Matrix4.identity();
                                __cascade.translateByDouble(
                                    desiredSize.width / 2L,
                                    desiredSize.height / 2L,
                                    0,
                                    1
                                );
                                __cascade.rotateZ(Math.PI);
                                __cascade.translateByDouble(
                                    -desiredSize.width / 2L,
                                    -desiredSize.height / 2L,
                                    0,
                                    1
                                );
                                return __cascade;
                            }
                        )
                    )(),
                    child: handle
                );
            }
            case TextSelectionHandleType.collapsed:
            {
                return SizedBox.CreateFromSize(size: getHandleSize(textLineHeight));
            }
            default:
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                );
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Offset getHandleAnchor(TextSelectionHandleType type, double textLineHeight)
    {
        Size handleSize = getHandleSize(textLineHeight);
        switch (type)
        {
            case TextSelectionHandleType.left:
            {
                return new Offset(handleSize.width / 2L, handleSize.height);
            }
            case TextSelectionHandleType.right:
            {
                return new Offset(
                    handleSize.width / 2L,
                    handleSize.height
                        - (2L * Text_selectionLibrary._kSelectionHandleRadius)
                        + Text_selectionLibrary._kSelectionHandleOverlap
                );
            }
            case TextSelectionHandleType.collapsed:
            {
                return new Offset(
                    handleSize.width / 2L,
                    textLineHeight + ((handleSize.height - textLineHeight) / 2L)
                );
            }
            default:
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                );
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public static partial class Text_selectionLibrary
{
    public static TextSelectionControls cupertinoTextSelectionHandleControls =
        new CupertinoTextSelectionHandleControls();
}

public static partial class Text_selectionLibrary
{
    public static TextSelectionControls cupertinoTextSelectionControls =
        new CupertinoTextSelectionControls();
}

public class _CupertinoTextSelectionControlsToolbar__text_selection : StatefulWidget
{
    public virtual ValueListenable<ClipboardStatus>? clipboardStatus { get; private set; }
    public virtual List<TextSelectionPoint> endpoints { get; private set; } = default!;
    public virtual Rect globalEditableRegion { get; private set; } = default!;
    public virtual Action? handleCopy { get; private set; }
    public virtual Action? handleCut { get; private set; }
    public virtual Action? handlePaste { get; private set; }
    public virtual Action? handleSelectAll { get; private set; }
    public virtual Offset selectionMidpoint { get; private set; } = default!;
    public virtual double textLineHeight { get; private set; } = default!;

    internal _CupertinoTextSelectionControlsToolbar__text_selection(
        ValueListenable<ClipboardStatus>? clipboardStatus,
        List<TextSelectionPoint> endpoints,
        Rect globalEditableRegion,
        Action? handleCopy,
        Action? handleCut,
        Action? handlePaste,
        Action? handleSelectAll,
        Offset selectionMidpoint,
        double textLineHeight
    )
    {
        this.clipboardStatus = clipboardStatus;
        this.endpoints = endpoints;
        this.globalEditableRegion = globalEditableRegion;
        this.handleCopy = handleCopy;
        this.handleCut = handleCut;
        this.handlePaste = handlePaste;
        this.handleSelectAll = handleSelectAll;
        this.selectionMidpoint = selectionMidpoint;
        this.textLineHeight = textLineHeight;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _CupertinoTextSelectionControlsToolbarState__text_selection()
        );
}

public class _CupertinoTextSelectionControlsToolbarState__text_selection
    : State<_CupertinoTextSelectionControlsToolbar__text_selection>
{
    internal virtual void _onChangedClipboardStatus()
    {
        setState(() => { });
    }

    public override void initState()
    {
        base.initState();
        widget.clipboardStatus?.addListener(_onChangedClipboardStatus);
    }

    public override void didUpdateWidget(
        _CupertinoTextSelectionControlsToolbar__text_selection oldWidget
    )
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(oldWidget.clipboardStatus, widget.clipboardStatus))
        {
            oldWidget.clipboardStatus?.removeListener(_onChangedClipboardStatus);
            widget.clipboardStatus?.addListener(_onChangedClipboardStatus);
        }
    }

    public override void dispose()
    {
        widget.clipboardStatus?.removeListener(_onChangedClipboardStatus);
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        if (
            (widget.handlePaste is not null)
            && Equals(widget.clipboardStatus?.value, ClipboardStatus.unknown)
        )
        {
            return SizedBox.CreateShrink();
        }
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        EdgeInsets mediaQueryPadding = MediaQuery.paddingOf(context);
        double anchorX = DorotiUiLibrary.clampDouble(
            widget.selectionMidpoint.dx + widget.globalEditableRegion.left,
            Text_selectionLibrary._kArrowScreenPadding + mediaQueryPadding.left,
            MediaQuery.widthOf(context)
                - mediaQueryPadding.right
                - Text_selectionLibrary._kArrowScreenPadding
        );
        double topAmountInEditableRegion =
            widget.endpoints.First().point.dy - widget.textLineHeight;
        double anchorTop =
            Math.Max(topAmountInEditableRegion, 0L) + widget.globalEditableRegion.top;
        var anchorAboveLocal = new Offset(anchorX, anchorTop);
        var anchorBelowLocal = new Offset(
            anchorX,
            widget.endpoints.Last().point.dy + widget.globalEditableRegion.top
        );
        var items = new List<Widget>();
        CupertinoLocalizations localizations = CupertinoLocalizations.of(context);
        Widget onePhysicalPixelVerticalDivider = new SizedBox(
            width: 1.0 / MediaQuery.devicePixelRatioOf(context)
        );
        void addToolbarButton(string text, Action onPressed)
        {
            if (Enumerable.Any(items))
            {
                items.Add(onePhysicalPixelVerticalDivider);
            }
            items.Add(
                CupertinoTextSelectionToolbarButton.CreateText(
                    onPressed: () => onPressed(),
                    text: text
                )
            );
        }
        if (widget.handleCut is not null)
        {
            addToolbarButton(localizations.cutButtonLabel, widget.handleCut!);
        }
        if (widget.handleCopy is not null)
        {
            addToolbarButton(localizations.copyButtonLabel, widget.handleCopy!);
        }
        if (
            (widget.handlePaste is not null)
            && Equals(widget.clipboardStatus?.value, ClipboardStatus.pasteable)
        )
        {
            addToolbarButton(localizations.pasteButtonLabel, widget.handlePaste!);
        }
        if (widget.handleSelectAll is not null)
        {
            addToolbarButton(localizations.selectAllButtonLabel, widget.handleSelectAll!);
        }
        if (!Enumerable.Any(items))
        {
            return SizedBox.CreateShrink();
        }
        return new CupertinoTextSelectionToolbar(
            anchorAbove: anchorAboveLocal,
            anchorBelow: anchorBelowLocal,
            children: items
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
