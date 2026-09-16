// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/desktop_text_selection.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

internal class _CupertinoDesktopTextSelectionHandleControls__desktop_text_selection : CupertinoDesktopTextSelectionControls, global::Doroti.Framework.Widgets.TextSelectionHandleControls
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

public class CupertinoDesktopTextSelectionControls : global::Doroti.Framework.Widgets.TextSelectionControls
{
    public override Size getHandleSize(double textLineHeight)
    {
        return Size.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget buildToolbar(global::Doroti.Framework.Widgets.BuildContext context, Rect globalEditableRegion, double textLineHeight, Offset selectionMidpoint, List<global::Doroti.Framework.Rendering.TextSelectionPoint> endpoints, global::Doroti.Framework.Services.TextSelectionDelegate @delegate, global::Doroti.Framework.Foundation.ValueListenable<global::Doroti.Framework.Widgets.ClipboardStatus>? clipboardStatus, Offset? lastSecondaryTapDownPosition)
    {
        return new _CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection(clipboardStatus: clipboardStatus, endpoints: endpoints, globalEditableRegion: globalEditableRegion, handleCut: canCut(@delegate) ? (() => { handleCut(@delegate); }) : null, handleCopy: canCopy(@delegate) ? (() => { handleCopy(@delegate); }) : null, handlePaste: canPaste(@delegate) ? (() => { _ = handlePaste(@delegate); }) : null, handleSelectAll: canSelectAll(@delegate) ? (() => { handleSelectAll(@delegate); }) : null, selectionMidpoint: selectionMidpoint, lastSecondaryTapDownPosition: lastSecondaryTapDownPosition, textLineHeight: textLineHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget buildHandle(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.TextSelectionHandleType type, double textLineHeight, global::System.Action? onTap = null)
    {
        return SizedBox.CreateShrink();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Offset getHandleAnchor(global::Doroti.Framework.Rendering.TextSelectionHandleType type, double textLineHeight)
    {
        return Offset.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void handleSelectAll(global::Doroti.Framework.Services.TextSelectionDelegate @delegate)
    {
        base.handleSelectAll(@delegate);
        @delegate.hideToolbar();
    }

}

public static partial class Desktop_text_selectionLibrary
{
    public static global::Doroti.Framework.Widgets.TextSelectionControls cupertinoDesktopTextSelectionHandleControls = new _CupertinoDesktopTextSelectionHandleControls__desktop_text_selection();
}

public static partial class Desktop_text_selectionLibrary
{
    public static global::Doroti.Framework.Widgets.TextSelectionControls cupertinoDesktopTextSelectionControls = new CupertinoDesktopTextSelectionControls();
}

public class _CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Foundation.ValueListenable<global::Doroti.Framework.Widgets.ClipboardStatus>? clipboardStatus { get; private set; }
    public virtual List<global::Doroti.Framework.Rendering.TextSelectionPoint> endpoints { get; private set; } = default!;
    public virtual Rect globalEditableRegion { get; private set; } = default!;
    public virtual global::System.Action? handleCopy { get; private set; }
    public virtual global::System.Action? handleCut { get; private set; }
    public virtual global::System.Action? handlePaste { get; private set; }
    public virtual global::System.Action? handleSelectAll { get; private set; }
    public virtual Offset? lastSecondaryTapDownPosition { get; private set; }
    public virtual Offset selectionMidpoint { get; private set; } = default!;
    public virtual double textLineHeight { get; private set; } = default!;

    internal _CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection(global::Doroti.Framework.Foundation.ValueListenable<global::Doroti.Framework.Widgets.ClipboardStatus>? clipboardStatus, List<global::Doroti.Framework.Rendering.TextSelectionPoint> endpoints, Rect globalEditableRegion, global::System.Action? handleCopy, global::System.Action? handleCut, global::System.Action? handlePaste, global::System.Action? handleSelectAll, Offset selectionMidpoint, double textLineHeight, Offset? lastSecondaryTapDownPosition)
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

public class _CupertinoDesktopTextSelectionControlsToolbarState__desktop_text_selection : global::Doroti.Framework.Widgets.State<_CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection>
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

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if ((widget.handlePaste is not null) && Equals(widget.clipboardStatus?.value, ClipboardStatus.unknown))
        {
            return SizedBox.CreateShrink();
        }
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        global::Doroti.Framework.Painting.EdgeInsets mediaQueryPadding = MediaQuery.paddingOf(context);
        var midpointAnchor = new global::Doroti.Ui.Offset(Dart_uiLibrary.clampDouble(widget.selectionMidpoint.dx - widget.globalEditableRegion.left, mediaQueryPadding.left, MediaQuery.widthOf(context) - mediaQueryPadding.right), widget.selectionMidpoint.dy - widget.globalEditableRegion.top);
        var items = new List<global::Doroti.Framework.Widgets.Widget>();
        CupertinoLocalizations localizations = CupertinoLocalizations.of(context);
        global::Doroti.Framework.Widgets.Widget onePhysicalPixelVerticalDivider = new global::Doroti.Framework.Widgets.SizedBox(width: 1.0 / MediaQuery.devicePixelRatioOf(context));
        void addToolbarButton(string text, global::System.Action onPressed)
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
