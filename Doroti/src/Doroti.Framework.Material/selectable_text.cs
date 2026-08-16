// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/selectable_text.dart
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

public static partial class Selectable_textLibrary
{
    public static long iOSHorizontalOffset = -2L;
}

internal class _TextSpanEditingController__selectable_text : global::Doroti.Framework.Widgets.TextEditingController
{
    internal virtual global::Doroti.Framework.Painting.TextSpan _textSpan { get; private set; } = default!;

    internal _TextSpanEditingController__selectable_text(global::Doroti.Framework.Painting.TextSpan textSpan) : base(text: textSpan.toPlainText(includeSemanticsLabels: false))
    {
        this._textSpan = textSpan;
    }

    public override global::Doroti.Framework.Painting.TextSpan buildTextSpan(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Painting.TextStyle? style = null, bool withComposing = default!)
    {
        return new global::Doroti.Framework.Painting.TextSpan(style: style, children: new List<global::Doroti.Framework.Painting.TextSpan> { this._textSpan }.Cast<global::Doroti.Framework.Painting.InlineSpan>().ToList());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string? text
    {
        set
        {
            var newText = value;
            throw new NotImplementedException();
        }
    }
}

internal class _SelectableTextSelectionGestureDetectorBuilder__selectable_text : global::Doroti.Framework.Widgets.TextSelectionGestureDetectorBuilder
{
    internal virtual _SelectableTextState__selectable_text _state { get; private set; } = default!;

    internal _SelectableTextSelectionGestureDetectorBuilder__selectable_text(_SelectableTextState__selectable_text state) : base(@delegate: state)
    {
        this._state = state;
    }

    public override void onSingleTapUp(global::Doroti.Framework.Gestures.TapDragUpDetails details)
    {
        if (!((global::Doroti.Framework.Widgets.TextSelectionGestureDetectorBuilderDelegate)this.@delegate).selectionEnabled)
        {
            return;
        }
        base.onSingleTapUp(details);
        this._state.widget.onTap?.Invoke();
    }

}

public class SelectableText : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual string? data { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextSpan? textSpan { get; private set; }
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? style { get; private set; }
    public virtual global::Doroti.Framework.Painting.StrutStyle? strutStyle { get; private set; }
    public virtual TextAlign? textAlign { get; private set; }
    public virtual TextDirection? textDirection { get; private set; }
    public virtual double? textScaleFactor { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextScaler? textScaler { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual long? minLines { get; private set; }
    public virtual long? maxLines { get; private set; }
    public virtual bool showCursor { get; private set; } = default!;
    public virtual double cursorWidth { get; private set; } = default!;
    public virtual double? cursorHeight { get; private set; }
    public virtual Radius? cursorRadius { get; private set; }
    public virtual Color? cursorColor { get; private set; }
    public virtual Color? selectionColor { get; private set; }
    public virtual BoxHeightStyle? selectionHeightStyle { get; private set; }
    public virtual BoxWidthStyle? selectionWidthStyle { get; private set; }
    public virtual bool enableInteractiveSelection { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.TextSelectionControls? selectionControls { get; private set; }
    public virtual global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.ToolbarOptions? toolbarOptions { get; private set; }
    public virtual global::System.Action? onTap { get; private set; }
    public virtual global::Doroti.Framework.Widgets.ScrollPhysics? scrollPhysics { get; private set; }
    public virtual global::Doroti.Framework.Widgets.ScrollBehavior? scrollBehavior { get; private set; }
    public virtual string? semanticsLabel { get; private set; }
    public virtual TextHeightBehavior? textHeightBehavior { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextWidthBasis? textWidthBasis { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Services.TextSelection, global::Doroti.Framework.Services.SelectionChangedCause?>? onSelectionChanged { get; private set; }
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget>? contextMenuBuilder { get; private set; }
    public virtual global::Doroti.Framework.Widgets.TextMagnifierConfiguration? magnifierConfiguration { get; private set; }

    public SelectableText(string data, global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::Doroti.Framework.Painting.TextStyle? style = null, global::Doroti.Framework.Painting.StrutStyle? strutStyle = null, TextAlign? textAlign = null, TextDirection? textDirection = null, double? textScaleFactor = null, global::Doroti.Framework.Painting.TextScaler? textScaler = null, bool showCursor = false, bool autofocus = false, global::Doroti.Framework.Widgets.ToolbarOptions? toolbarOptions = null, long? minLines = null, long? maxLines = null, double cursorWidth = 2.0, double? cursorHeight = null, Radius? cursorRadius = null, Color? cursorColor = null, Color? selectionColor = null, BoxHeightStyle? selectionHeightStyle = null, BoxWidthStyle? selectionWidthStyle = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.start, bool enableInteractiveSelection = true, global::Doroti.Framework.Widgets.TextSelectionControls? selectionControls = null, global::System.Action? onTap = null, global::Doroti.Framework.Widgets.ScrollPhysics? scrollPhysics = null, global::Doroti.Framework.Widgets.ScrollBehavior? scrollBehavior = null, string? semanticsLabel = null, TextHeightBehavior? textHeightBehavior = null, global::Doroti.Framework.Painting.TextWidthBasis? textWidthBasis = null, global::System.Action<global::Doroti.Framework.Services.TextSelection, global::Doroti.Framework.Services.SelectionChangedCause?>? onSelectionChanged = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget>? contextMenuBuilder = default!, global::Doroti.Framework.Widgets.TextMagnifierConfiguration? magnifierConfiguration = null) : base(key: key)
    {
        global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget>? __contextMenuBuilder = contextMenuBuilder ?? _defaultContextMenuBuilder;
        this.data = data;
        this.focusNode = focusNode;
        this.style = style;
        this.strutStyle = strutStyle;
        this.textAlign = textAlign;
        this.textDirection = textDirection;
        this.textScaleFactor = textScaleFactor;
        this.textScaler = textScaler;
        this.showCursor = showCursor;
        this.autofocus = autofocus;
        this.toolbarOptions = toolbarOptions;
        this.minLines = minLines;
        this.maxLines = maxLines;
        this.cursorWidth = cursorWidth;
        this.cursorHeight = cursorHeight;
        this.cursorRadius = cursorRadius;
        this.cursorColor = cursorColor;
        this.selectionColor = selectionColor;
        this.selectionHeightStyle = selectionHeightStyle;
        this.selectionWidthStyle = selectionWidthStyle;
        this.dragStartBehavior = dragStartBehavior;
        this.enableInteractiveSelection = enableInteractiveSelection;
        this.selectionControls = selectionControls;
        this.onTap = onTap;
        this.scrollPhysics = scrollPhysics;
        this.scrollBehavior = scrollBehavior;
        this.semanticsLabel = semanticsLabel;
        this.textHeightBehavior = textHeightBehavior;
        this.textWidthBasis = textWidthBasis;
        this.onSelectionChanged = onSelectionChanged;
        this.contextMenuBuilder = __contextMenuBuilder;
        this.magnifierConfiguration = magnifierConfiguration;
        this.textSpan = null;
        System.Diagnostics.Debug.Assert(((maxLines is null) || (DartRuntimePrimitives.RequireValue(maxLines) > 0L)));
        System.Diagnostics.Debug.Assert(((minLines is null) || (DartRuntimePrimitives.RequireValue(minLines) > 0L)));
        System.Diagnostics.Debug.Assert(((((maxLines is null)) || ((minLines is null))) || ((maxLines >= DartRuntimePrimitives.RequireValue(minLines)))));
        System.Diagnostics.Debug.Assert(((textScaler is null) || (textScaleFactor is null)));
    }

    public static SelectableText CreateRich(global::Doroti.Framework.Painting.TextSpan textSpan, global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::Doroti.Framework.Painting.TextStyle? style = null, global::Doroti.Framework.Painting.StrutStyle? strutStyle = null, TextAlign? textAlign = null, TextDirection? textDirection = null, double? textScaleFactor = null, global::Doroti.Framework.Painting.TextScaler? textScaler = null, bool showCursor = false, bool autofocus = false, global::Doroti.Framework.Widgets.ToolbarOptions? toolbarOptions = null, long? minLines = null, long? maxLines = null, double cursorWidth = 2.0, double? cursorHeight = null, Radius? cursorRadius = null, Color? cursorColor = null, Color? selectionColor = null, BoxHeightStyle? selectionHeightStyle = null, BoxWidthStyle? selectionWidthStyle = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.start, bool enableInteractiveSelection = true, global::Doroti.Framework.Widgets.TextSelectionControls? selectionControls = null, global::System.Action? onTap = null, global::Doroti.Framework.Widgets.ScrollPhysics? scrollPhysics = null, global::Doroti.Framework.Widgets.ScrollBehavior? scrollBehavior = null, string? semanticsLabel = null, TextHeightBehavior? textHeightBehavior = null, global::Doroti.Framework.Painting.TextWidthBasis? textWidthBasis = null, global::System.Action<global::Doroti.Framework.Services.TextSelection, global::Doroti.Framework.Services.SelectionChangedCause?>? onSelectionChanged = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget>? contextMenuBuilder = default!, global::Doroti.Framework.Widgets.TextMagnifierConfiguration? magnifierConfiguration = null)
    {
        var __instance = new SelectableText(data: default!, key: key, focusNode: focusNode, style: style, strutStyle: strutStyle, textAlign: textAlign, textDirection: textDirection, textScaleFactor: textScaleFactor, textScaler: textScaler, showCursor: showCursor, autofocus: autofocus, toolbarOptions: toolbarOptions, minLines: minLines, maxLines: maxLines, cursorWidth: cursorWidth, cursorHeight: cursorHeight, cursorRadius: cursorRadius, cursorColor: cursorColor, selectionColor: selectionColor, selectionHeightStyle: selectionHeightStyle, selectionWidthStyle: selectionWidthStyle, dragStartBehavior: dragStartBehavior, enableInteractiveSelection: enableInteractiveSelection, selectionControls: selectionControls, onTap: onTap, scrollPhysics: scrollPhysics, scrollBehavior: scrollBehavior, semanticsLabel: semanticsLabel, textHeightBehavior: textHeightBehavior, textWidthBasis: textWidthBasis, onSelectionChanged: onSelectionChanged, contextMenuBuilder: contextMenuBuilder, magnifierConfiguration: magnifierConfiguration);
        global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget>? __contextMenuBuilder = contextMenuBuilder ?? _defaultContextMenuBuilder;
        __instance.textSpan = textSpan;
        __instance.focusNode = focusNode;
        __instance.style = style;
        __instance.strutStyle = strutStyle;
        __instance.textAlign = textAlign;
        __instance.textDirection = textDirection;
        __instance.textScaleFactor = textScaleFactor;
        __instance.textScaler = textScaler;
        __instance.showCursor = showCursor;
        __instance.autofocus = autofocus;
        __instance.toolbarOptions = toolbarOptions;
        __instance.minLines = minLines;
        __instance.maxLines = maxLines;
        __instance.cursorWidth = cursorWidth;
        __instance.cursorHeight = cursorHeight;
        __instance.cursorRadius = cursorRadius;
        __instance.cursorColor = cursorColor;
        __instance.selectionColor = selectionColor;
        __instance.selectionHeightStyle = selectionHeightStyle;
        __instance.selectionWidthStyle = selectionWidthStyle;
        __instance.dragStartBehavior = dragStartBehavior;
        __instance.enableInteractiveSelection = enableInteractiveSelection;
        __instance.selectionControls = selectionControls;
        __instance.onTap = onTap;
        __instance.scrollPhysics = scrollPhysics;
        __instance.scrollBehavior = scrollBehavior;
        __instance.semanticsLabel = semanticsLabel;
        __instance.textHeightBehavior = textHeightBehavior;
        __instance.textWidthBasis = textWidthBasis;
        __instance.onSelectionChanged = onSelectionChanged;
        __instance.contextMenuBuilder = __contextMenuBuilder;
        __instance.magnifierConfiguration = magnifierConfiguration;
        __instance.data = null;
        return __instance;
    }

    public virtual bool selectionEnabled => this.enableInteractiveSelection;
    internal static global::Doroti.Framework.Widgets.Widget _defaultContextMenuBuilder(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.EditableTextState editableTextState)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)AdaptiveTextSelectionToolbar.CreateEditableText(editableTextState: editableTextState));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SelectableTextState__selectable_text());
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<string>("data", this.data, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<string>("semanticsLabel", this.semanticsLabel, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.FocusNode>("focusNode", this.focusNode, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("style", this.style, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("autofocus", this.autofocus, defaultValue: false));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("showCursor", this.showCursor, defaultValue: false));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("minLines", this.minLines, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("maxLines", this.maxLines, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextAlign>("textAlign", this.textAlign, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("textScaleFactor", this.textScaleFactor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextScaler>("textScaler", this.textScaler, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("cursorWidth", this.cursorWidth, defaultValue: 2.0));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("cursorHeight", this.cursorHeight, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Radius>("cursorRadius", this.cursorRadius, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Color>("cursorColor", this.cursorColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Color>("selectionColor", this.selectionColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("selectionEnabled", value: this.selectionEnabled, defaultValue: true, ifFalse: "selection disabled"));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.TextSelectionControls>("selectionControls", this.selectionControls, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.ScrollPhysics>("scrollPhysics", this.scrollPhysics, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.ScrollBehavior>("scrollBehavior", this.scrollBehavior, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.TextHeightBehavior>("textHeightBehavior", this.textHeightBehavior, defaultValue: null));
    }

}

internal class _SelectableTextState__selectable_text : global::Doroti.Framework.Widgets.State<SelectableText>, global::Doroti.Framework.Widgets.TextSelectionGestureDetectorBuilderDelegate
{
    internal virtual _TextSpanEditingController__selectable_text _controller { get; set; } = default!;
    internal virtual global::Doroti.Framework.Widgets.FocusNode? _focusNode { get; set; } = default;
    internal virtual bool _showSelectionHandles { get; set; } = false;
    internal virtual _SelectableTextSelectionGestureDetectorBuilder__selectable_text _selectionGestureDetectorBuilder { get; set; } = default!;
    public virtual bool forcePressEnabled { get; set; } = default!;
    public virtual global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.EditableTextState> editableTextKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.EditableTextState>.Create();

    internal virtual global::Doroti.Framework.Widgets.EditableTextState? _editableText => ((global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.EditableTextState>)this.editableTextKey).currentState;
    internal virtual global::Doroti.Framework.Widgets.FocusNode _effectiveFocusNode => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.FocusNode>((((SelectableText)this.widget).focusNode ?? (_focusNode ??= new global::Doroti.Framework.Widgets.FocusNode(skipTraversal: true))));
    public virtual bool selectionEnabled => ((SelectableText)this.widget).selectionEnabled;
    public override void initState()
    {
        base.initState();
        _selectionGestureDetectorBuilder = new _SelectableTextSelectionGestureDetectorBuilder__selectable_text(state: this);
        _controller = new _TextSpanEditingController__selectable_text(textSpan: (((SelectableText)this.widget).textSpan ?? new global::Doroti.Framework.Painting.TextSpan(text: ((SelectableText)this.widget).data)));
        this._controller.addListener(() => this._onControllerChanged());
        this._effectiveFocusNode.addListener(() => this._handleFocusChanged());
    }

    public override void didUpdateWidget(SelectableText oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (((((SelectableText)this.widget).data != ((SelectableText)oldWidget).data) || (!object.Equals(((SelectableText)this.widget).textSpan, ((SelectableText)oldWidget).textSpan))))
        {
            this._controller.removeListener(() => this._onControllerChanged());
            this._controller.dispose();
            _controller = new _TextSpanEditingController__selectable_text(textSpan: (((SelectableText)this.widget).textSpan ?? new global::Doroti.Framework.Painting.TextSpan(text: ((SelectableText)this.widget).data)));
            this._controller.addListener(() => this._onControllerChanged());
        }
        if ((!object.Equals(((SelectableText)this.widget).focusNode, ((SelectableText)oldWidget).focusNode)))
        {
            ((((SelectableText)oldWidget).focusNode ?? this._focusNode))?.removeListener(() => this._handleFocusChanged());
            ((((SelectableText)this.widget).focusNode ?? this._focusNode))?.addListener(() => this._handleFocusChanged());
        }
        if ((((global::Doroti.Framework.Widgets.FocusNode)this._effectiveFocusNode).hasFocus && this._controller.selection.isCollapsed))
        {
            _showSelectionHandles = false;
        }
        else
        {
            _showSelectionHandles = true;
        }
    }

    public override void dispose()
    {
        this._effectiveFocusNode.removeListener(() => this._handleFocusChanged());
        this._focusNode?.dispose();
        this._controller.dispose();
        base.dispose();
    }

    internal virtual void _onControllerChanged()
    {
        bool showSelectionHandles__20544 = (!((global::Doroti.Framework.Widgets.FocusNode)this._effectiveFocusNode).hasFocus || !this._controller.selection.isCollapsed);
        if ((showSelectionHandles__20544 == this._showSelectionHandles))
        {
            return;
        }
        setState(((global::System.Action)(() => {
_showSelectionHandles = showSelectionHandles__20544;
})));
    }

    internal virtual void _handleFocusChanged()
    {
        if ((!((global::Doroti.Framework.Widgets.FocusNode)this._effectiveFocusNode).hasFocus && (object.Equals(global::Doroti.Framework.Scheduler.SchedulerBinding.instance.lifecycleState, AppLifecycleState.resumed))))
        {
            this._controller.value = new global::Doroti.Framework.Services.TextEditingValue(text: this._controller.value.text);
        }
    }

    internal virtual void _handleSelectionChanged(global::Doroti.Framework.Services.TextSelection selection, global::Doroti.Framework.Services.SelectionChangedCause? cause)
    {
        bool willShowSelectionHandles__21638 = _shouldShowSelectionHandles(cause);
        if ((willShowSelectionHandles__21638 != this._showSelectionHandles))
        {
            setState(((global::System.Action)(() => {
_showSelectionHandles = willShowSelectionHandles__21638;
})));
        }
        ((SelectableText)this.widget).onSelectionChanged?.Invoke(selection, cause);
        switch (Theme.of(this.context).platform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                {
                    if ((object.Equals(cause, global::Doroti.Framework.Services.SelectionChangedCause.longPress)))
                    {
                        this._editableText?.bringIntoView(((global::Doroti.Framework.Services.TextSelection)selection).@base);
                    }
                    return;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                break;
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
    }

    internal virtual void _handleSelectionHandleTapped()
    {
        if (this._controller.selection.isCollapsed)
        {
            this._editableText!.toggleToolbar();
        }
    }

    internal virtual bool _shouldShowSelectionHandles(global::Doroti.Framework.Services.SelectionChangedCause? cause)
    {
        if (!this._selectionGestureDetectorBuilder.shouldShowSelectionToolbar)
        {
            return false;
        }
        if (this._controller.selection.isCollapsed)
        {
            return false;
        }
        if ((object.Equals(cause, global::Doroti.Framework.Services.SelectionChangedCause.keyboard)))
        {
            return false;
        }
        if ((object.Equals(cause, global::Doroti.Framework.Services.SelectionChangedCause.longPress)))
        {
            return true;
        }
        if ((this._controller.text.Length != 0))
        {
            return true;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        DartRuntimePrimitives.Assert(() => !((((((SelectableText)this.widget).style is not null) && !((SelectableText)this.widget).style!.inherit) && (((((SelectableText)this.widget).style!.fontSize is null) || (((SelectableText)this.widget).style!.textBaseline is null))))), () => (object?)"inherit false style must supply fontSize and textBaseline");
        ThemeData theme__23957 = Theme.of(context);
        global::Doroti.Framework.Widgets.DefaultSelectionStyle selectionStyle__24016 = ((global::Doroti.Framework.Widgets.DefaultSelectionStyle)(object?)DefaultSelectionStyle.of(context));
        global::Doroti.Framework.Widgets.FocusNode focusNode__24088 = this._effectiveFocusNode;
        global::Doroti.Framework.Widgets.TextSelectionControls? textSelectionControls__24149 = ((SelectableText)this.widget).selectionControls;
        bool paintCursorAboveText__24214 = default!;
        bool cursorOpacityAnimates__24251 = default!;
        global::Doroti.Ui.Offset? cursorOffset__24286 = default!;
        global::Doroti.Ui.Color cursorColor__24316 = default!;
        global::Doroti.Ui.Color selectionColor__24345 = default!;
        global::Doroti.Ui.Radius? cursorRadius__24373 = ((global::Doroti.Ui.Radius?)(object?)((SelectableText)this.widget).cursorRadius);
        switch (theme__23957.platform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                {
                    CupertinoThemeData cupertinoTheme__24504 = CupertinoTheme.of(context);
                    forcePressEnabled = true;
                    textSelectionControls__24149 ??= Text_selectionLibrary.materialTextSelectionHandleControls;
                    paintCursorAboveText__24214 = true;
                    cursorOpacityAnimates__24251 = true;
                    cursorColor__24316 = ((((SelectableText)this.widget).cursorColor ?? ((global::Doroti.Framework.Widgets.DefaultSelectionStyle)selectionStyle__24016).cursorColor) ?? cupertinoTheme__24504.primaryColor);
                    selectionColor__24345 = (((global::Doroti.Framework.Widgets.DefaultSelectionStyle)selectionStyle__24016).selectionColor ?? cupertinoTheme__24504.primaryColor.withOpacity(0.4));
                    cursorRadius__24373 ??= global::Doroti.Ui.Radius.circular(2.0);
                    cursorOffset__24286 = new global::Doroti.Ui.Offset((Selectable_textLibrary.iOSHorizontalOffset / MediaQuery.devicePixelRatioOf(context)), 0);
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                {
                    CupertinoThemeData cupertinoTheme__25178 = CupertinoTheme.of(context);
                    forcePressEnabled = false;
                    textSelectionControls__24149 ??= Desktop_text_selectionLibrary.desktopTextSelectionHandleControls;
                    paintCursorAboveText__24214 = true;
                    cursorOpacityAnimates__24251 = true;
                    cursorColor__24316 = ((((SelectableText)this.widget).cursorColor ?? ((global::Doroti.Framework.Widgets.DefaultSelectionStyle)selectionStyle__24016).cursorColor) ?? cupertinoTheme__25178.primaryColor);
                    selectionColor__24345 = (((global::Doroti.Framework.Widgets.DefaultSelectionStyle)selectionStyle__24016).selectionColor ?? cupertinoTheme__25178.primaryColor.withOpacity(0.4));
                    cursorRadius__24373 ??= global::Doroti.Ui.Radius.circular(2.0);
                    cursorOffset__24286 = new global::Doroti.Ui.Offset((Selectable_textLibrary.iOSHorizontalOffset / MediaQuery.devicePixelRatioOf(context)), 0);
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                {
                    forcePressEnabled = false;
                    textSelectionControls__24149 ??= Text_selectionLibrary.materialTextSelectionHandleControls;
                    paintCursorAboveText__24214 = false;
                    cursorOpacityAnimates__24251 = false;
                    cursorColor__24316 = ((((SelectableText)this.widget).cursorColor ?? ((global::Doroti.Framework.Widgets.DefaultSelectionStyle)selectionStyle__24016).cursorColor) ?? theme__23957.colorScheme.primary);
                    selectionColor__24345 = (((global::Doroti.Framework.Widgets.DefaultSelectionStyle)selectionStyle__24016).selectionColor ?? theme__23957.colorScheme.primary.withOpacity(0.4));
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    forcePressEnabled = false;
                    textSelectionControls__24149 ??= Desktop_text_selectionLibrary.desktopTextSelectionHandleControls;
                    paintCursorAboveText__24214 = false;
                    cursorOpacityAnimates__24251 = false;
                    cursorColor__24316 = ((((SelectableText)this.widget).cursorColor ?? ((global::Doroti.Framework.Widgets.DefaultSelectionStyle)selectionStyle__24016).cursorColor) ?? theme__23957.colorScheme.primary);
                    selectionColor__24345 = (((global::Doroti.Framework.Widgets.DefaultSelectionStyle)selectionStyle__24016).selectionColor ?? theme__23957.colorScheme.primary.withOpacity(0.4));
                    break;
                }
        }
        global::Doroti.Framework.Widgets.DefaultTextStyle defaultTextStyle__26764 = ((global::Doroti.Framework.Widgets.DefaultTextStyle)(object?)DefaultTextStyle.of(context));
        global::Doroti.Framework.Painting.TextStyle? effectiveTextStyle__26828 = ((SelectableText)this.widget).style;
        if (((effectiveTextStyle__26828 is null) || ((global::Doroti.Framework.Painting.TextStyle)effectiveTextStyle__26828).inherit))
        {
            effectiveTextStyle__26828 = ((global::Doroti.Framework.Widgets.DefaultTextStyle)defaultTextStyle__26764).style.merge((((SelectableText)this.widget).style ?? ((_TextSpanEditingController__selectable_text)this._controller)._textSpan.style));
        }
        global::Doroti.Framework.Painting.TextScaler? effectiveScaler__27078 = (((SelectableText)this.widget).textScaler ?? (((SelectableText)this.widget).textScaleFactor switch { null => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.TextScaler>(null), double textScaleFactor__27214 => global::Doroti.Framework.Painting.TextScaler.CreateLinear(textScaleFactor__27214) }));
        global::Doroti.Framework.Widgets.Widget child__27297 = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.RepaintBoundary(child: new global::Doroti.Framework.Widgets.EditableText(key: this.editableTextKey, style: effectiveTextStyle__26828, readOnly: true, toolbarOptions: ((SelectableText)this.widget).toolbarOptions, textWidthBasis: (((SelectableText)this.widget).textWidthBasis ?? ((global::Doroti.Framework.Widgets.DefaultTextStyle)defaultTextStyle__26764).textWidthBasis), textHeightBehavior: (((SelectableText)this.widget).textHeightBehavior ?? ((global::Doroti.Framework.Widgets.DefaultTextStyle)defaultTextStyle__26764).textHeightBehavior), showSelectionHandles: this._showSelectionHandles, showCursor: ((SelectableText)this.widget).showCursor, controller: this._controller, focusNode: focusNode__24088, strutStyle: (((SelectableText)this.widget).strutStyle ?? new global::Doroti.Framework.Painting.StrutStyle()), textAlign: ((((SelectableText)this.widget).textAlign ?? ((global::Doroti.Framework.Widgets.DefaultTextStyle)defaultTextStyle__26764).textAlign) ?? global::Doroti.Ui.TextAlign.start), textDirection: ((SelectableText)this.widget).textDirection, textScaler: effectiveScaler__27078, autofocus: ((SelectableText)this.widget).autofocus, forceLine: false, minLines: ((SelectableText)this.widget).minLines, maxLines: (((SelectableText)this.widget).maxLines ?? ((global::Doroti.Framework.Widgets.DefaultTextStyle)defaultTextStyle__26764).maxLines), selectionColor: (((SelectableText)this.widget).selectionColor ?? selectionColor__24345), selectionControls: (((SelectableText)this.widget).selectionEnabled ? textSelectionControls__24149 : null), onSelectionChanged: (global::System.Action<global::Doroti.Framework.Services.TextSelection, global::Doroti.Framework.Services.SelectionChangedCause?>)this._handleSelectionChanged, onSelectionHandleTapped: () => this._handleSelectionHandleTapped(), rendererIgnoresPointer: true, cursorWidth: ((SelectableText)this.widget).cursorWidth, cursorHeight: ((SelectableText)this.widget).cursorHeight, cursorRadius: cursorRadius__24373, cursorColor: cursorColor__24316, selectionHeightStyle: ((SelectableText)this.widget).selectionHeightStyle, selectionWidthStyle: ((SelectableText)this.widget).selectionWidthStyle, cursorOpacityAnimates: cursorOpacityAnimates__24251, cursorOffset: cursorOffset__24286, paintCursorAboveText: paintCursorAboveText__24214, backgroundCursorColor: CupertinoColors.inactiveGray, enableInteractiveSelection: ((SelectableText)this.widget).enableInteractiveSelection, magnifierConfiguration: (((SelectableText)this.widget).magnifierConfiguration ?? TextMagnifier.adaptiveMagnifierConfiguration), dragStartBehavior: ((SelectableText)this.widget).dragStartBehavior, scrollPhysics: ((SelectableText)this.widget).scrollPhysics, scrollBehavior: ((SelectableText)this.widget).scrollBehavior, autofillHints: null, contextMenuBuilder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget>?)((SelectableText)this.widget).contextMenuBuilder)));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(label: ((SelectableText)this.widget).semanticsLabel, excludeSemantics: (((SelectableText)this.widget).semanticsLabel is not null), onLongPress: ((global::System.Action)(() => {
this._effectiveFocusNode.requestFocus();
})), child: this._selectionGestureDetectorBuilder.buildGestureDetector(behavior: global::Doroti.Framework.Rendering.HitTestBehavior.translucent, child: child__27297)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
