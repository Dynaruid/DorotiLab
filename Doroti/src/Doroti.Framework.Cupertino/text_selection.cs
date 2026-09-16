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

internal class _CupertinoTextSelectionHandlePainter__text_selection : global::Doroti.Framework.Rendering.CustomPainter
{
    public virtual Color color { get; private set; } = default!;

    internal _CupertinoTextSelectionHandlePainter__text_selection(Color color)
    {
        this.color = color;
    }

    public override void paint(Canvas canvas, Size size)
    {
        var halfStrokeWidth = 1.0;
        var paintLocal = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = color;
    return __cascade;
}))();
        var circle = Rect.fromCircle(center: new global::Doroti.Ui.Offset(Text_selectionLibrary._kSelectionHandleRadius, Text_selectionLibrary._kSelectionHandleRadius), radius: Text_selectionLibrary._kSelectionHandleRadius);
        var line = Rect.fromPoints(new global::Doroti.Ui.Offset(Text_selectionLibrary._kSelectionHandleRadius - halfStrokeWidth, (2L * Text_selectionLibrary._kSelectionHandleRadius) - Text_selectionLibrary._kSelectionHandleOverlap), new global::Doroti.Ui.Offset(Text_selectionLibrary._kSelectionHandleRadius + halfStrokeWidth, size.height));
        var path = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addOval(circle);
    __cascade.addRect(line);
    return __cascade;
}))();
        canvas.drawPath(path, paintLocal);
    }

    public override bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate) => !Equals(color, ((_CupertinoTextSelectionHandlePainter__text_selection)oldDelegate).color);
}

public class CupertinoTextSelectionHandleControls : CupertinoTextSelectionControls, global::Doroti.Framework.Widgets.TextSelectionHandleControls
{

    public override Widget buildToolbar(BuildContext context, Rect globalEditableRegion, double textLineHeight, Offset selectionMidpoint, List<global::Doroti.Framework.Rendering.TextSelectionPoint> endpoints, global::Doroti.Framework.Services.TextSelectionDelegate @delegate, global::Doroti.Framework.Foundation.ValueListenable<ClipboardStatus>? clipboardStatus, Offset? lastSecondaryTapDownPosition) => DartRuntimePrimitives.ConvertValue<Widget>(SizedBox.CreateShrink());
    public override bool canCut(global::Doroti.Framework.Services.TextSelectionDelegate @delegate) => false;
    public override bool canCopy(global::Doroti.Framework.Services.TextSelectionDelegate @delegate) => false;
    public override bool canPaste(global::Doroti.Framework.Services.TextSelectionDelegate @delegate) => false;
    public override bool canSelectAll(global::Doroti.Framework.Services.TextSelectionDelegate @delegate) => false;
    public override void handleCut(global::Doroti.Framework.Services.TextSelectionDelegate @delegate) { }

    public virtual void handleCut(global::Doroti.Framework.Services.TextSelectionDelegate @delegate, ClipboardStatusNotifier? clipboardStatus = null)
    {
    }

    public override void handleCopy(global::Doroti.Framework.Services.TextSelectionDelegate @delegate) { }

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

public class CupertinoTextSelectionControls : global::Doroti.Framework.Widgets.TextSelectionControls
{
    public override Size getHandleSize(double textLineHeight)
    {
        return new global::Doroti.Ui.Size(Text_selectionLibrary._kSelectionHandleRadius * 2L, textLineHeight + (Text_selectionLibrary._kSelectionHandleRadius * 2L) - Text_selectionLibrary._kSelectionHandleOverlap);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget buildToolbar(global::Doroti.Framework.Widgets.BuildContext context, Rect globalEditableRegion, double textLineHeight, Offset selectionMidpoint, List<global::Doroti.Framework.Rendering.TextSelectionPoint> endpoints, global::Doroti.Framework.Services.TextSelectionDelegate @delegate, global::Doroti.Framework.Foundation.ValueListenable<global::Doroti.Framework.Widgets.ClipboardStatus>? clipboardStatus, Offset? lastSecondaryTapDownPosition)
    {
        return new _CupertinoTextSelectionControlsToolbar__text_selection(clipboardStatus: clipboardStatus, endpoints: endpoints, globalEditableRegion: globalEditableRegion, handleCut: canCut(@delegate) ? (() => { handleCut(@delegate); }) : null, handleCopy: canCopy(@delegate) ? (() => { handleCopy(@delegate); }) : null, handlePaste: canPaste(@delegate) ? (() => { _ = handlePaste(@delegate); }) : null, handleSelectAll: canSelectAll(@delegate) ? (() => { handleSelectAll(@delegate); }) : null, selectionMidpoint: selectionMidpoint, textLineHeight: textLineHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget buildHandle(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.TextSelectionHandleType type, double textLineHeight, global::System.Action? onTap = null)
    {
        global::Doroti.Ui.Size desiredSize = default!;
        global::Doroti.Framework.Widgets.Widget handle = default!;
        global::Doroti.Framework.Widgets.Widget customPaint = new global::Doroti.Framework.Widgets.CustomPaint(painter: new _CupertinoTextSelectionHandlePainter__text_selection(CupertinoTheme.of(context).selectionHandleColor));
        switch (type)
        {
            case TextSelectionHandleType.left:
                {
                    desiredSize = getHandleSize(textLineHeight);
                    handle = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(SizedBox.CreateFromSize(size: desiredSize, child: customPaint));
                    return handle;
                }
            case TextSelectionHandleType.right:
                {
                    desiredSize = getHandleSize(textLineHeight);
                    handle = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(SizedBox.CreateFromSize(size: desiredSize, child: customPaint));
                    return new global::Doroti.Framework.Widgets.Transform(transform: ((Func<Matrix4>)(() =>
{
    var __cascade = Matrix4.identity();
    __cascade.translateByDouble(desiredSize.width / 2L, desiredSize.height / 2L, 0, 1);
    __cascade.rotateZ(Dart_mathLibrary.pi);
    __cascade.translateByDouble(-desiredSize.width / 2L, -desiredSize.height / 2L, 0, 1);
    return __cascade;
}))(), child: handle);
                }
            case TextSelectionHandleType.collapsed:
                {
                    return SizedBox.CreateFromSize(size: getHandleSize(textLineHeight));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Offset getHandleAnchor(global::Doroti.Framework.Rendering.TextSelectionHandleType type, double textLineHeight)
    {
        global::Doroti.Ui.Size handleSize = getHandleSize(textLineHeight);
        switch (type)
        {
            case TextSelectionHandleType.left:
                {
                    return new global::Doroti.Ui.Offset(handleSize.width / 2L, handleSize.height);
                }
            case TextSelectionHandleType.right:
                {
                    return new global::Doroti.Ui.Offset(handleSize.width / 2L, handleSize.height - (2L * Text_selectionLibrary._kSelectionHandleRadius) + Text_selectionLibrary._kSelectionHandleOverlap);
                }
            case TextSelectionHandleType.collapsed:
                {
                    return new global::Doroti.Ui.Offset(handleSize.width / 2L, textLineHeight + ((handleSize.height - textLineHeight) / 2L));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Text_selectionLibrary
{
    public static global::Doroti.Framework.Widgets.TextSelectionControls cupertinoTextSelectionHandleControls = new CupertinoTextSelectionHandleControls();
}

public static partial class Text_selectionLibrary
{
    public static global::Doroti.Framework.Widgets.TextSelectionControls cupertinoTextSelectionControls = new CupertinoTextSelectionControls();
}

public class _CupertinoTextSelectionControlsToolbar__text_selection : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Foundation.ValueListenable<global::Doroti.Framework.Widgets.ClipboardStatus>? clipboardStatus { get; private set; }
    public virtual List<global::Doroti.Framework.Rendering.TextSelectionPoint> endpoints { get; private set; } = default!;
    public virtual Rect globalEditableRegion { get; private set; } = default!;
    public virtual global::System.Action? handleCopy { get; private set; }
    public virtual global::System.Action? handleCut { get; private set; }
    public virtual global::System.Action? handlePaste { get; private set; }
    public virtual global::System.Action? handleSelectAll { get; private set; }
    public virtual Offset selectionMidpoint { get; private set; } = default!;
    public virtual double textLineHeight { get; private set; } = default!;

    internal _CupertinoTextSelectionControlsToolbar__text_selection(global::Doroti.Framework.Foundation.ValueListenable<global::Doroti.Framework.Widgets.ClipboardStatus>? clipboardStatus, List<global::Doroti.Framework.Rendering.TextSelectionPoint> endpoints, Rect globalEditableRegion, global::System.Action? handleCopy, global::System.Action? handleCut, global::System.Action? handlePaste, global::System.Action? handleSelectAll, Offset selectionMidpoint, double textLineHeight)
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

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoTextSelectionControlsToolbarState__text_selection());
}

public class _CupertinoTextSelectionControlsToolbarState__text_selection : global::Doroti.Framework.Widgets.State<_CupertinoTextSelectionControlsToolbar__text_selection>
{
    internal virtual void _onChangedClipboardStatus()
    {
        setState(() =>
        {
        });
    }

    public override void initState()
    {
        base.initState();
        widget.clipboardStatus?.addListener(_onChangedClipboardStatus);
    }

    public override void didUpdateWidget(_CupertinoTextSelectionControlsToolbar__text_selection oldWidget)
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

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if ((widget.handlePaste is not null) && Equals(widget.clipboardStatus?.value, ClipboardStatus.unknown))
        {
            return SizedBox.CreateShrink();
        }
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        global::Doroti.Framework.Painting.EdgeInsets mediaQueryPadding = MediaQuery.paddingOf(context);
        double anchorX = Dart_uiLibrary.clampDouble(widget.selectionMidpoint.dx + widget.globalEditableRegion.left, Text_selectionLibrary._kArrowScreenPadding + mediaQueryPadding.left, MediaQuery.widthOf(context) - mediaQueryPadding.right - Text_selectionLibrary._kArrowScreenPadding);
        double topAmountInEditableRegion = widget.endpoints.First().point.dy - widget.textLineHeight;
        double anchorTop = Math.Max(topAmountInEditableRegion, 0L) + widget.globalEditableRegion.top;
        var anchorAboveLocal = new global::Doroti.Ui.Offset(anchorX, anchorTop);
        var anchorBelowLocal = new global::Doroti.Ui.Offset(anchorX, widget.endpoints.Last().point.dy + widget.globalEditableRegion.top);
        var items = new List<global::Doroti.Framework.Widgets.Widget>();
        CupertinoLocalizations localizations = CupertinoLocalizations.of(context);
        global::Doroti.Framework.Widgets.Widget onePhysicalPixelVerticalDivider = new global::Doroti.Framework.Widgets.SizedBox(width: 1.0 / MediaQuery.devicePixelRatioOf(context));
        void addToolbarButton(string text, global::System.Action onPressed)
        {
            if (Enumerable.Any(items))
            {
                items.Add(onePhysicalPixelVerticalDivider);
            }
            items.Add(CupertinoTextSelectionToolbarButton.CreateText(onPressed: () => onPressed(), text: text));
        }
        if (widget.handleCut is not null)
        {
            addToolbarButton(localizations.cutButtonLabel, widget.handleCut!);
        }
        if (widget.handleCopy is not null)
        {
            addToolbarButton(localizations.copyButtonLabel, widget.handleCopy!);
        }
        if ((widget.handlePaste is not null) && Equals(widget.clipboardStatus?.value, ClipboardStatus.pasteable))
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
        return new CupertinoTextSelectionToolbar(anchorAbove: anchorAboveLocal, anchorBelow: anchorBelowLocal, children: items);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
