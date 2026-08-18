// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/desktop_text_selection.dart
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

internal class _DesktopTextSelectionHandleControls__desktop_text_selection : DesktopTextSelectionControls
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

public class DesktopTextSelectionControls : global::Doroti.Framework.Widgets.TextSelectionControls
{
    public override Size getHandleSize(double textLineHeight)
    {
        return Size.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget buildToolbar(global::Doroti.Framework.Widgets.BuildContext context, Rect globalEditableRegion, double textLineHeight, Offset selectionMidpoint, List<global::Doroti.Framework.Rendering.TextSelectionPoint> endpoints, global::Doroti.Framework.Services.TextSelectionDelegate @delegate, global::Doroti.Framework.Foundation.ValueListenable<global::Doroti.Framework.Widgets.ClipboardStatus>? clipboardStatus, Offset? lastSecondaryTapDownPosition)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _DesktopTextSelectionControlsToolbar__desktop_text_selection(clipboardStatus: clipboardStatus, endpoints: endpoints, globalEditableRegion: globalEditableRegion, handleCut: ((global::System.Action)(canCut(@delegate) ? (() => { handleCut(@delegate); }) : null)), handleCopy: ((global::System.Action)(canCopy(@delegate) ? (() => { handleCopy(@delegate); }) : null)), handlePaste: ((global::System.Action)(canPaste(@delegate) ? (() => { _ = handlePaste(@delegate); }) : null)), handleSelectAll: ((global::System.Action)(canSelectAll(@delegate) ? (() => { handleSelectAll(@delegate); }) : null)), selectionMidpoint: selectionMidpoint, lastSecondaryTapDownPosition: lastSecondaryTapDownPosition, textLineHeight: textLineHeight));
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

    public override bool canSelectAll(global::Doroti.Framework.Services.TextSelectionDelegate @delegate)
    {
        global::Doroti.Framework.Services.TextEditingValue value__3039 = ((global::Doroti.Framework.Services.TextSelectionDelegate)@delegate).textEditingValue;
        return ((((global::Doroti.Framework.Services.TextSelectionDelegate)@delegate).selectAllEnabled && (((global::Doroti.Framework.Services.TextEditingValue)value__3039).text.Length != 0)) && !(((((global::Doroti.Framework.Services.TextEditingValue)value__3039).selection.start == 0L) && (((global::Doroti.Framework.Services.TextEditingValue)value__3039).selection.end == ((global::Doroti.Framework.Services.TextEditingValue)value__3039).text.Length))));
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
    public static global::Doroti.Framework.Widgets.TextSelectionControls desktopTextSelectionHandleControls = ((global::Doroti.Framework.Widgets.TextSelectionControls)(object?)new _DesktopTextSelectionHandleControls__desktop_text_selection());
}

public static partial class Desktop_text_selectionLibrary
{
    public static global::Doroti.Framework.Widgets.TextSelectionControls desktopTextSelectionControls = ((global::Doroti.Framework.Widgets.TextSelectionControls)(object?)new DesktopTextSelectionControls());
}

public class _DesktopTextSelectionControlsToolbar__desktop_text_selection : global::Doroti.Framework.Widgets.StatefulWidget
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

    internal _DesktopTextSelectionControlsToolbar__desktop_text_selection(global::Doroti.Framework.Foundation.ValueListenable<global::Doroti.Framework.Widgets.ClipboardStatus>? clipboardStatus, List<global::Doroti.Framework.Rendering.TextSelectionPoint> endpoints, Rect globalEditableRegion, global::System.Action? handleCopy, global::System.Action? handleCut, global::System.Action? handlePaste, global::System.Action? handleSelectAll, Offset selectionMidpoint, double textLineHeight, Offset? lastSecondaryTapDownPosition)
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

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DesktopTextSelectionControlsToolbarState__desktop_text_selection());
}

public class _DesktopTextSelectionControlsToolbarState__desktop_text_selection : global::Doroti.Framework.Widgets.State<_DesktopTextSelectionControlsToolbar__desktop_text_selection>
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
        ((_DesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).clipboardStatus?.addListener(() => this._onChangedClipboardStatus());
    }

    public override void didUpdateWidget(_DesktopTextSelectionControlsToolbar__desktop_text_selection oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((_DesktopTextSelectionControlsToolbar__desktop_text_selection)oldWidget).clipboardStatus, ((_DesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).clipboardStatus)))
        {
            ((_DesktopTextSelectionControlsToolbar__desktop_text_selection)oldWidget).clipboardStatus?.removeListener(() => this._onChangedClipboardStatus());
            ((_DesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).clipboardStatus?.addListener(() => this._onChangedClipboardStatus());
        }
    }

    public override void dispose()
    {
        ((_DesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).clipboardStatus?.removeListener(() => this._onChangedClipboardStatus());
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        if (((((_DesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).handlePaste is not null) && (object.Equals(((_DesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).clipboardStatus?.value, global::Doroti.Framework.Widgets.ClipboardStatus.unknown))))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.SizedBox.CreateShrink());
        }
        global::Doroti.Framework.Painting.EdgeInsets mediaQueryPadding__6385 = ((global::Doroti.Framework.Painting.EdgeInsets)(object?)MediaQuery.paddingOf(context));
        var midpointAnchor__6446 = new global::Doroti.Ui.Offset(Dart_uiLibrary.clampDouble((((_DesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).selectionMidpoint.dx - ((_DesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).globalEditableRegion.left), ((global::Doroti.Framework.Painting.EdgeInsets)mediaQueryPadding__6385).left, (MediaQuery.widthOf(context) - ((global::Doroti.Framework.Painting.EdgeInsets)mediaQueryPadding__6385).right)), (((_DesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).selectionMidpoint.dy - ((_DesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).globalEditableRegion.top));
        MaterialLocalizations localizations__6775 = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        var items__6836 = new List<global::Doroti.Framework.Widgets.Widget>();
        void addToolbarButton(string text, global::System.Action onPressed)
        {
            items__6836.Add(DesktopTextSelectionToolbarButton.CreateText(context: context, onPressed: () => onPressed(), text: text));
        }
        if ((((_DesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).handleCut is not null))
        {
            addToolbarButton(((MaterialLocalizations)localizations__6775).cutButtonLabel, ((_DesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).handleCut!);
        }
        if ((((_DesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).handleCopy is not null))
        {
            addToolbarButton(((MaterialLocalizations)localizations__6775).copyButtonLabel, ((_DesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).handleCopy!);
        }
        if (((((_DesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).handlePaste is not null) && (object.Equals(((_DesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).clipboardStatus?.value, global::Doroti.Framework.Widgets.ClipboardStatus.pasteable))))
        {
            addToolbarButton(((MaterialLocalizations)localizations__6775).pasteButtonLabel, ((_DesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).handlePaste!);
        }
        if ((((_DesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).handleSelectAll is not null))
        {
            addToolbarButton(((MaterialLocalizations)localizations__6775).selectAllButtonLabel, ((_DesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).handleSelectAll!);
        }
        if (!System.Linq.Enumerable.Any(items__6836))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.SizedBox.CreateShrink());
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new DesktopTextSelectionToolbar(anchor: (((_DesktopTextSelectionControlsToolbar__desktop_text_selection)this.widget).lastSecondaryTapDownPosition ?? midpointAnchor__6446), children: items__6836));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
