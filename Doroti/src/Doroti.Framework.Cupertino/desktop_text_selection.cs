// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/desktop_text_selection.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

internal class _CupertinoDesktopTextSelectionHandleControls__desktop_text_selection : CupertinoDesktopTextSelectionControls, TextSelectionHandleControls
{

    public override Widget buildToolbar(BuildContext context, Rect globalEditableRegion, double textLineHeight, Offset selectionMidpoint, List<TextSelectionPoint> endpoints, TextSelectionDelegate @delegate, ValueListenable<ClipboardStatus>? clipboardStatus, Offset? lastSecondaryTapDownPosition) => DartRuntimePrimitives.ConvertValue<Widget>(SizedBox.CreateShrink());
    public override bool canCut(TextSelectionDelegate @delegate) => false;
    public override bool canCopy(TextSelectionDelegate @delegate) => false;
    public override bool canPaste(TextSelectionDelegate @delegate) => false;
    public override bool canSelectAll(TextSelectionDelegate @delegate) => false;
    public override void handleCut(TextSelectionDelegate @delegate) { }

    public virtual void handleCut(TextSelectionDelegate @delegate, ClipboardStatusNotifier? clipboardStatus = null)
    {
    }

    public override void handleCopy(TextSelectionDelegate @delegate) { }

    public virtual void handleCopy(TextSelectionDelegate @delegate, ClipboardStatusNotifier? clipboardStatus = null)
    {
    }

    public async override Future handlePaste(TextSelectionDelegate @delegate)
    {
    }

    public override void handleSelectAll(TextSelectionDelegate @delegate)
    {
    }

}

public class CupertinoDesktopTextSelectionControls : TextSelectionControls
{
    public override Size getHandleSize(double textLineHeight)
    {
        return Size.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget buildToolbar(BuildContext context, Rect globalEditableRegion, double textLineHeight, Offset selectionMidpoint, List<TextSelectionPoint> endpoints, TextSelectionDelegate @delegate, ValueListenable<ClipboardStatus>? clipboardStatus, Offset? lastSecondaryTapDownPosition)
    {
        return new _CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection(clipboardStatus: clipboardStatus, endpoints: endpoints, globalEditableRegion: globalEditableRegion, handleCut: canCut(@delegate) ? (() => { handleCut(@delegate); }) : null, handleCopy: canCopy(@delegate) ? (() => { handleCopy(@delegate); }) : null, handlePaste: canPaste(@delegate) ? (() => { _ = handlePaste(@delegate); }) : null, handleSelectAll: canSelectAll(@delegate) ? (() => { handleSelectAll(@delegate); }) : null, selectionMidpoint: selectionMidpoint, lastSecondaryTapDownPosition: lastSecondaryTapDownPosition, textLineHeight: textLineHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget buildHandle(BuildContext context, TextSelectionHandleType type, double textLineHeight, Action? onTap = null)
    {
        return SizedBox.CreateShrink();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Offset getHandleAnchor(TextSelectionHandleType type, double textLineHeight)
    {
        return Offset.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void handleSelectAll(TextSelectionDelegate @delegate)
    {
        base.handleSelectAll(@delegate);
        @delegate.hideToolbar();
    }

}

public static partial class Desktop_text_selectionLibrary
{
    public static TextSelectionControls cupertinoDesktopTextSelectionHandleControls = new _CupertinoDesktopTextSelectionHandleControls__desktop_text_selection();
}

public static partial class Desktop_text_selectionLibrary
{
    public static TextSelectionControls cupertinoDesktopTextSelectionControls = new CupertinoDesktopTextSelectionControls();
}

public class _CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection : StatefulWidget
{
    public virtual ValueListenable<ClipboardStatus>? clipboardStatus { get; private set; }
    public virtual List<TextSelectionPoint> endpoints { get; private set; } = default!;
    public virtual Rect globalEditableRegion { get; private set; } = default!;
    public virtual Action? handleCopy { get; private set; }
    public virtual Action? handleCut { get; private set; }
    public virtual Action? handlePaste { get; private set; }
    public virtual Action? handleSelectAll { get; private set; }
    public virtual Offset? lastSecondaryTapDownPosition { get; private set; }
    public virtual Offset selectionMidpoint { get; private set; } = default!;
    public virtual double textLineHeight { get; private set; } = default!;

    internal _CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection(ValueListenable<ClipboardStatus>? clipboardStatus, List<TextSelectionPoint> endpoints, Rect globalEditableRegion, Action? handleCopy, Action? handleCut, Action? handlePaste, Action? handleSelectAll, Offset selectionMidpoint, double textLineHeight, Offset? lastSecondaryTapDownPosition)
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
        this.lastSecondaryTapDownPosition = lastSecondaryTapDownPosition;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoDesktopTextSelectionControlsToolbarState__desktop_text_selection());
}

public class _CupertinoDesktopTextSelectionControlsToolbarState__desktop_text_selection : State<_CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection>
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

    public override void didUpdateWidget(_CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection oldWidget)
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
        if ((widget.handlePaste is not null) && Equals(widget.clipboardStatus?.value, ClipboardStatus.unknown))
        {
            return SizedBox.CreateShrink();
        }
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        EdgeInsets mediaQueryPadding = MediaQuery.paddingOf(context);
        var midpointAnchor = new Offset(Dart_uiLibrary.clampDouble(widget.selectionMidpoint.dx - widget.globalEditableRegion.left, mediaQueryPadding.left, MediaQuery.widthOf(context) - mediaQueryPadding.right), widget.selectionMidpoint.dy - widget.globalEditableRegion.top);
        var items = new List<Widget>();
        CupertinoLocalizations localizations = CupertinoLocalizations.of(context);
        Widget onePhysicalPixelVerticalDivider = new SizedBox(width: 1.0 / MediaQuery.devicePixelRatioOf(context));
        void addToolbarButton(string text, Action onPressed)
        {
            if (Enumerable.Any(items))
            {
                items.Add(onePhysicalPixelVerticalDivider);
            }
            items.Add(CupertinoDesktopTextSelectionToolbarButton.CreateText(onPressed: () => onPressed(), text: text));
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
        return new CupertinoDesktopTextSelectionToolbar(anchor: widget.lastSecondaryTapDownPosition ?? midpointAnchor, children: items);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
