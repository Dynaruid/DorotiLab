// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/desktop_text_selection.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Cupertino;

internal class _CupertinoDesktopTextSelectionHandleControls__desktop_text_selection : CupertinoDesktopTextSelectionControls, global::Doroti.Framework.Widgets.TextSelectionHandleControls
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

public class CupertinoDesktopTextSelectionControls : global::Doroti.Framework.Widgets.TextSelectionControls
{
    public override Size getHandleSize(double textLineHeight)
    {
        return Size.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget buildToolbar(global::Doroti.Framework.Widgets.BuildContext context, Rect globalEditableRegion, double textLineHeight, Offset selectionMidpoint, List<global::Doroti.Framework.Rendering.TextSelectionPoint> endpoints, global::Doroti.Framework.Services.TextSelectionDelegate @delegate, global::Doroti.Framework.Foundation.ValueListenable<global::Doroti.Framework.Widgets.ClipboardStatus>? clipboardStatus, Offset? lastSecondaryTapDownPosition)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection(clipboardStatus: clipboardStatus, endpoints: endpoints, globalEditableRegion: globalEditableRegion, handleCut: ((global::System.Action)(canCut(@delegate) ? (() => { handleCut(@delegate); }) : null)), handleCopy: ((global::System.Action)(canCopy(@delegate) ? (() => { handleCopy(@delegate); }) : null)), handlePaste: ((global::System.Action)(canPaste(@delegate) ? (() => { _ = handlePaste(@delegate); }) : null)), handleSelectAll: ((global::System.Action)(canSelectAll(@delegate) ? (() => { handleSelectAll(@delegate); }) : null)), selectionMidpoint: selectionMidpoint, lastSecondaryTapDownPosition: lastSecondaryTapDownPosition, textLineHeight: textLineHeight));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget buildHandle(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.TextSelectionHandleType type, double textLineHeight, global::System.Action? onTap = null)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.SizedBox.CreateShrink());
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
    public static global::Doroti.Framework.Widgets.TextSelectionControls cupertinoDesktopTextSelectionHandleControls = ((global::Doroti.Framework.Widgets.TextSelectionControls)(object?)new _CupertinoDesktopTextSelectionHandleControls__desktop_text_selection());
}

public static partial class Desktop_text_selectionLibrary
{
    public static global::Doroti.Framework.Widgets.TextSelectionControls cupertinoDesktopTextSelectionControls = ((global::Doroti.Framework.Widgets.TextSelectionControls)(object?)new CupertinoDesktopTextSelectionControls());
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
        setState(((global::System.Action)(() =>
        {
        })));
    }

    public override void initState()
    {
        base.initState();
        ((_CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).clipboardStatus?.addListener(() => this._onChangedClipboardStatus());
    }

    public override void didUpdateWidget(_CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((_CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection)oldWidget).clipboardStatus, ((_CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).clipboardStatus)))
        {
            ((_CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection)oldWidget).clipboardStatus?.removeListener(() => this._onChangedClipboardStatus());
            ((_CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).clipboardStatus?.addListener(() => this._onChangedClipboardStatus());
        }
    }

    public override void dispose()
    {
        ((_CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).clipboardStatus?.removeListener(() => this._onChangedClipboardStatus());
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if (((((_CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).handlePaste is not null) && (object.Equals(((_CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).clipboardStatus?.value, global::Doroti.Framework.Widgets.ClipboardStatus.unknown))))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.SizedBox.CreateShrink());
        }
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        global::Doroti.Framework.Painting.EdgeInsets mediaQueryPadding = ((global::Doroti.Framework.Painting.EdgeInsets)(object?)MediaQuery.paddingOf(context));
        var midpointAnchor = new global::Doroti.Ui.Offset(Dart_uiLibrary.clampDouble((((_CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).selectionMidpoint.dx - ((_CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).globalEditableRegion.left), ((global::Doroti.Framework.Painting.EdgeInsets)mediaQueryPadding).left, (MediaQuery.widthOf(context) - ((global::Doroti.Framework.Painting.EdgeInsets)mediaQueryPadding).right)), (((_CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).selectionMidpoint.dy - ((_CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).globalEditableRegion.top));
        var items = new List<global::Doroti.Framework.Widgets.Widget>();
        CupertinoLocalizations localizations = CupertinoLocalizations.of(context);
        global::Doroti.Framework.Widgets.Widget onePhysicalPixelVerticalDivider = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.SizedBox(width: (1.0 / MediaQuery.devicePixelRatioOf(context))));
        void addToolbarButton(string text, global::System.Action onPressed)
        {
            if (System.Linq.Enumerable.Any(items))
            {
                items.Add(onePhysicalPixelVerticalDivider);
            }
            items.Add(CupertinoDesktopTextSelectionToolbarButton.CreateText(onPressed: () => onPressed(), text: text));
        }
        if ((((_CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).handleCut is not null))
        {
            addToolbarButton(localizations.cutButtonLabel, ((_CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).handleCut!);
        }
        if ((((_CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).handleCopy is not null))
        {
            addToolbarButton(localizations.copyButtonLabel, ((_CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).handleCopy!);
        }
        if (((((_CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).handlePaste is not null) && (object.Equals(((_CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).clipboardStatus?.value, global::Doroti.Framework.Widgets.ClipboardStatus.pasteable))))
        {
            addToolbarButton(localizations.pasteButtonLabel, ((_CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).handlePaste!);
        }
        if ((((_CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).handleSelectAll is not null))
        {
            addToolbarButton(localizations.selectAllButtonLabel, ((_CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).handleSelectAll!);
        }
        if (!System.Linq.Enumerable.Any(items))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.SizedBox.CreateShrink());
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new CupertinoDesktopTextSelectionToolbar(anchor: (((_CupertinoDesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).lastSecondaryTapDownPosition ?? midpointAnchor), children: items));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
