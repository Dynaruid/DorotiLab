// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/text_selection.dart
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
        var halfStrokeWidth__1132 = 1.0;
        var paint__1165 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = this.color;
    return __cascade;
}))();
        var circle__1207 = global::Doroti.Ui.Rect.fromCircle(center: new global::Doroti.Ui.Offset(Text_selectionLibrary._kSelectionHandleRadius, Text_selectionLibrary._kSelectionHandleRadius), radius: Text_selectionLibrary._kSelectionHandleRadius);
        var line__1367 = global::Doroti.Ui.Rect.fromPoints(new global::Doroti.Ui.Offset((Text_selectionLibrary._kSelectionHandleRadius - halfStrokeWidth__1132), ((2L * Text_selectionLibrary._kSelectionHandleRadius) - Text_selectionLibrary._kSelectionHandleOverlap)), new global::Doroti.Ui.Offset((Text_selectionLibrary._kSelectionHandleRadius + halfStrokeWidth__1132), size.height));
        var path__1622 = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addOval(circle__1207);
    __cascade.addRect(line__1367);
    return __cascade;
}))();
        canvas.drawPath(path__1622, paint__1165);
    }

    public override bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate) => (!object.Equals(this.color, ((_CupertinoTextSelectionHandlePainter__text_selection)oldDelegate).color));
}

public class CupertinoTextSelectionHandleControls : CupertinoTextSelectionControls
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

public class CupertinoTextSelectionControls : global::Doroti.Framework.Widgets.TextSelectionControls
{
    public override Size getHandleSize(double textLineHeight)
    {
        return new global::Doroti.Ui.Size((Text_selectionLibrary._kSelectionHandleRadius * 2L), ((textLineHeight + (Text_selectionLibrary._kSelectionHandleRadius * 2L)) - Text_selectionLibrary._kSelectionHandleOverlap));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget buildToolbar(global::Doroti.Framework.Widgets.BuildContext context, Rect globalEditableRegion, double textLineHeight, Offset selectionMidpoint, List<global::Doroti.Framework.Rendering.TextSelectionPoint> endpoints, global::Doroti.Framework.Services.TextSelectionDelegate @delegate, global::Doroti.Framework.Foundation.ValueListenable<global::Doroti.Framework.Widgets.ClipboardStatus>? clipboardStatus, Offset? lastSecondaryTapDownPosition)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _CupertinoTextSelectionControlsToolbar__text_selection(clipboardStatus: clipboardStatus, endpoints: endpoints, globalEditableRegion: globalEditableRegion, handleCut: ((global::System.Action)(canCut(@delegate) ? (() => { handleCut(@delegate); }) : null)), handleCopy: ((global::System.Action)(canCopy(@delegate) ? (() => { handleCopy(@delegate); }) : null)), handlePaste: ((global::System.Action)(canPaste(@delegate) ? (() => { _ = handlePaste(@delegate); }) : null)), handleSelectAll: ((global::System.Action)(canSelectAll(@delegate) ? (() => { handleSelectAll(@delegate); }) : null)), selectionMidpoint: selectionMidpoint, textLineHeight: textLineHeight));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget buildHandle(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.TextSelectionHandleType type, double textLineHeight, global::System.Action? onTap = null)
    {
        global::Doroti.Ui.Size desiredSize__4108 = default!;
        global::Doroti.Framework.Widgets.Widget handle__4138 = default!;
        global::Doroti.Framework.Widgets.Widget customPaint__4164 = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.CustomPaint(painter: new _CupertinoTextSelectionHandlePainter__text_selection(CupertinoTheme.of(context).selectionHandleColor)));
        switch (type)
        {
            case global::Doroti.Framework.Rendering.TextSelectionHandleType.left:
                {
                    desiredSize__4108 = getHandleSize(textLineHeight);
                    handle__4138 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(global::Doroti.Framework.Widgets.SizedBox.CreateFromSize(size: desiredSize__4108, child: customPaint__4164));
                    return handle__4138;
                }
            case global::Doroti.Framework.Rendering.TextSelectionHandleType.right:
                {
                    desiredSize__4108 = getHandleSize(textLineHeight);
                    handle__4138 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(global::Doroti.Framework.Widgets.SizedBox.CreateFromSize(size: desiredSize__4108, child: customPaint__4164));
                    return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Transform(transform: ((Func<Matrix4>)(() =>
{
    var __cascade = Matrix4.identity();
    __cascade.translateByDouble((desiredSize__4108.width / 2L), (desiredSize__4108.height / 2L), 0, 1);
    __cascade.rotateZ(Dart_mathLibrary.pi);
    __cascade.translateByDouble((-desiredSize__4108.width / 2L), (-desiredSize__4108.height / 2L), 0, 1);
    return __cascade;
}))(), child: handle__4138));
                }
            case global::Doroti.Framework.Rendering.TextSelectionHandleType.collapsed:
                {
                    return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.SizedBox.CreateFromSize(size: getHandleSize(textLineHeight)));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Offset getHandleAnchor(global::Doroti.Framework.Rendering.TextSelectionHandleType type, double textLineHeight)
    {
        global::Doroti.Ui.Size handleSize__5681 = ((global::Doroti.Ui.Size)(object?)getHandleSize(textLineHeight));
        switch (type)
        {
            case global::Doroti.Framework.Rendering.TextSelectionHandleType.left:
                {
                    return new global::Doroti.Ui.Offset((handleSize__5681.width / 2L), handleSize__5681.height);
                }
            case global::Doroti.Framework.Rendering.TextSelectionHandleType.right:
                {
                    return new global::Doroti.Ui.Offset((handleSize__5681.width / 2L), ((handleSize__5681.height - (2L * Text_selectionLibrary._kSelectionHandleRadius)) + Text_selectionLibrary._kSelectionHandleOverlap));
                }
            case global::Doroti.Framework.Rendering.TextSelectionHandleType.collapsed:
                {
                    return new global::Doroti.Ui.Offset((handleSize__5681.width / 2L), (textLineHeight + (((handleSize__5681.height - textLineHeight)) / 2L)));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Text_selectionLibrary
{
    public static global::Doroti.Framework.Widgets.TextSelectionControls cupertinoTextSelectionHandleControls = ((global::Doroti.Framework.Widgets.TextSelectionControls)(object?)new CupertinoTextSelectionHandleControls());
}

public static partial class Text_selectionLibrary
{
    public static global::Doroti.Framework.Widgets.TextSelectionControls cupertinoTextSelectionControls = ((global::Doroti.Framework.Widgets.TextSelectionControls)(object?)new CupertinoTextSelectionControls());
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
        setState(((global::System.Action)(() =>
        {
        })));
    }

    public override void initState()
    {
        base.initState();
        ((_CupertinoTextSelectionControlsToolbar__text_selection)this.widget).clipboardStatus?.addListener(() => this._onChangedClipboardStatus());
    }

    public override void didUpdateWidget(_CupertinoTextSelectionControlsToolbar__text_selection oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((_CupertinoTextSelectionControlsToolbar__text_selection)oldWidget).clipboardStatus, ((_CupertinoTextSelectionControlsToolbar__text_selection)this.widget).clipboardStatus)))
        {
            ((_CupertinoTextSelectionControlsToolbar__text_selection)oldWidget).clipboardStatus?.removeListener(() => this._onChangedClipboardStatus());
            ((_CupertinoTextSelectionControlsToolbar__text_selection)this.widget).clipboardStatus?.addListener(() => this._onChangedClipboardStatus());
        }
    }

    public override void dispose()
    {
        ((_CupertinoTextSelectionControlsToolbar__text_selection)this.widget).clipboardStatus?.removeListener(() => this._onChangedClipboardStatus());
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if (((((_CupertinoTextSelectionControlsToolbar__text_selection)this.widget).handlePaste is not null) && (object.Equals(((_CupertinoTextSelectionControlsToolbar__text_selection)this.widget).clipboardStatus?.value, global::Doroti.Framework.Widgets.ClipboardStatus.unknown))))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.SizedBox.CreateShrink());
        }
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        global::Doroti.Framework.Painting.EdgeInsets mediaQueryPadding__9280 = ((global::Doroti.Framework.Painting.EdgeInsets)(object?)MediaQuery.paddingOf(context));
        double anchorX__9544 = Dart_uiLibrary.clampDouble((((_CupertinoTextSelectionControlsToolbar__text_selection)this.widget).selectionMidpoint.dx + ((_CupertinoTextSelectionControlsToolbar__text_selection)this.widget).globalEditableRegion.left), (Text_selectionLibrary._kArrowScreenPadding + ((global::Doroti.Framework.Painting.EdgeInsets)mediaQueryPadding__9280).left), ((MediaQuery.widthOf(context) - ((global::Doroti.Framework.Painting.EdgeInsets)mediaQueryPadding__9280).right) - Text_selectionLibrary._kArrowScreenPadding));
        double topAmountInEditableRegion__9799 = (((_CupertinoTextSelectionControlsToolbar__text_selection)this.widget).endpoints.First().point.dy - ((_CupertinoTextSelectionControlsToolbar__text_selection)this.widget).textLineHeight);
        double anchorTop__9909 = (Math.Max(topAmountInEditableRegion__9799, 0L) + ((_CupertinoTextSelectionControlsToolbar__text_selection)this.widget).globalEditableRegion.top);
        var anchorAbove__10252 = new global::Doroti.Ui.Offset(anchorX__9544, anchorTop__9909);
        var anchorBelow__10304 = new global::Doroti.Ui.Offset(anchorX__9544, (((_CupertinoTextSelectionControlsToolbar__text_selection)this.widget).endpoints.Last().point.dy + ((_CupertinoTextSelectionControlsToolbar__text_selection)this.widget).globalEditableRegion.top));
        var items__10431 = new List<global::Doroti.Framework.Widgets.Widget>();
        CupertinoLocalizations localizations__10484 = CupertinoLocalizations.of(context);
        global::Doroti.Framework.Widgets.Widget onePhysicalPixelVerticalDivider__10553 = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.SizedBox(width: (1.0 / MediaQuery.devicePixelRatioOf(context))));
        void addToolbarButton(string text, global::System.Action onPressed)
        {
            if (System.Linq.Enumerable.Any(items__10431))
            {
                items__10431.Add(onePhysicalPixelVerticalDivider__10553);
            }
            items__10431.Add(CupertinoTextSelectionToolbarButton.CreateText(onPressed: () => onPressed(), text: text));
        }
        if ((((_CupertinoTextSelectionControlsToolbar__text_selection)this.widget).handleCut is not null))
        {
            addToolbarButton(localizations__10484.cutButtonLabel, ((_CupertinoTextSelectionControlsToolbar__text_selection)this.widget).handleCut!);
        }
        if ((((_CupertinoTextSelectionControlsToolbar__text_selection)this.widget).handleCopy is not null))
        {
            addToolbarButton(localizations__10484.copyButtonLabel, ((_CupertinoTextSelectionControlsToolbar__text_selection)this.widget).handleCopy!);
        }
        if (((((_CupertinoTextSelectionControlsToolbar__text_selection)this.widget).handlePaste is not null) && (object.Equals(((_CupertinoTextSelectionControlsToolbar__text_selection)this.widget).clipboardStatus?.value, global::Doroti.Framework.Widgets.ClipboardStatus.pasteable))))
        {
            addToolbarButton(localizations__10484.pasteButtonLabel, ((_CupertinoTextSelectionControlsToolbar__text_selection)this.widget).handlePaste!);
        }
        if ((((_CupertinoTextSelectionControlsToolbar__text_selection)this.widget).handleSelectAll is not null))
        {
            addToolbarButton(localizations__10484.selectAllButtonLabel, ((_CupertinoTextSelectionControlsToolbar__text_selection)this.widget).handleSelectAll!);
        }
        if (!System.Linq.Enumerable.Any(items__10431))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.SizedBox.CreateShrink());
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new CupertinoTextSelectionToolbar(anchorAbove: anchorAbove__10252, anchorBelow: anchorBelow__10304, children: items__10431));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
