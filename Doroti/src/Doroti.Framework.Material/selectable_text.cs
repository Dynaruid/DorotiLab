// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/selectable_text.dart

using Doroti.Runtime;
using Doroti.Ui;

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
        _textSpan = textSpan;
    }

    public override global::Doroti.Framework.Painting.TextSpan buildTextSpan(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Painting.TextStyle? style = null, bool withComposing = default!)
    {
        return new global::Doroti.Framework.Painting.TextSpan(style: style, children: new List<global::Doroti.Framework.Painting.TextSpan> { _textSpan }.Cast<global::Doroti.Framework.Painting.InlineSpan>().ToList());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string text
    {
        get => base.text;
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
        _state = state;
    }

    public override void onSingleTapUp(global::Doroti.Framework.Gestures.TapDragUpDetails details)
    {
        if (!@delegate.selectionEnabled)
        {
            return;
        }
        base.onSingleTapUp(details);
        _state.widget.onTap?.Invoke();
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

    public SelectableText(string data, global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::Doroti.Framework.Painting.TextStyle? style = null, global::Doroti.Framework.Painting.StrutStyle? strutStyle = null, TextAlign? textAlign = null, TextDirection? textDirection = null, double? textScaleFactor = null, global::Doroti.Framework.Painting.TextScaler? textScaler = null, bool showCursor = false, bool autofocus = false, global::Doroti.Framework.Widgets.ToolbarOptions? toolbarOptions = null, long? minLines = null, long? maxLines = null, double cursorWidth = 2.0, double? cursorHeight = null, Radius? cursorRadius = null, Color? cursorColor = null, Color? selectionColor = null, BoxHeightStyle? selectionHeightStyle = null, BoxWidthStyle? selectionWidthStyle = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = Gestures.DragStartBehavior.start, bool enableInteractiveSelection = true, global::Doroti.Framework.Widgets.TextSelectionControls? selectionControls = null, global::System.Action? onTap = null, global::Doroti.Framework.Widgets.ScrollPhysics? scrollPhysics = null, global::Doroti.Framework.Widgets.ScrollBehavior? scrollBehavior = null, string? semanticsLabel = null, TextHeightBehavior? textHeightBehavior = null, global::Doroti.Framework.Painting.TextWidthBasis? textWidthBasis = null, global::System.Action<global::Doroti.Framework.Services.TextSelection, global::Doroti.Framework.Services.SelectionChangedCause?>? onSelectionChanged = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget>? contextMenuBuilder = default!, global::Doroti.Framework.Widgets.TextMagnifierConfiguration? magnifierConfiguration = null) : base(key: key)
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
        textSpan = null;
        System.Diagnostics.Debug.Assert((maxLines is null) || (DartRuntimePrimitives.RequireValue(maxLines) > 0L));
        System.Diagnostics.Debug.Assert((minLines is null) || (DartRuntimePrimitives.RequireValue(minLines) > 0L));
        System.Diagnostics.Debug.Assert(maxLines is null || minLines is null || maxLines >= DartRuntimePrimitives.RequireValue(minLines));
        System.Diagnostics.Debug.Assert((textScaler is null) || (textScaleFactor is null));
    }

    public static SelectableText CreateRich(global::Doroti.Framework.Painting.TextSpan textSpan, global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::Doroti.Framework.Painting.TextStyle? style = null, global::Doroti.Framework.Painting.StrutStyle? strutStyle = null, TextAlign? textAlign = null, TextDirection? textDirection = null, double? textScaleFactor = null, global::Doroti.Framework.Painting.TextScaler? textScaler = null, bool showCursor = false, bool autofocus = false, global::Doroti.Framework.Widgets.ToolbarOptions? toolbarOptions = null, long? minLines = null, long? maxLines = null, double cursorWidth = 2.0, double? cursorHeight = null, Radius? cursorRadius = null, Color? cursorColor = null, Color? selectionColor = null, BoxHeightStyle? selectionHeightStyle = null, BoxWidthStyle? selectionWidthStyle = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = Gestures.DragStartBehavior.start, bool enableInteractiveSelection = true, global::Doroti.Framework.Widgets.TextSelectionControls? selectionControls = null, global::System.Action? onTap = null, global::Doroti.Framework.Widgets.ScrollPhysics? scrollPhysics = null, global::Doroti.Framework.Widgets.ScrollBehavior? scrollBehavior = null, string? semanticsLabel = null, TextHeightBehavior? textHeightBehavior = null, global::Doroti.Framework.Painting.TextWidthBasis? textWidthBasis = null, global::System.Action<global::Doroti.Framework.Services.TextSelection, global::Doroti.Framework.Services.SelectionChangedCause?>? onSelectionChanged = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget>? contextMenuBuilder = default!, global::Doroti.Framework.Widgets.TextMagnifierConfiguration? magnifierConfiguration = null)
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

    public virtual bool selectionEnabled => enableInteractiveSelection;
    internal static global::Doroti.Framework.Widgets.Widget _defaultContextMenuBuilder(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.EditableTextState editableTextState)
    {
        return AdaptiveTextSelectionToolbar.CreateEditableText(editableTextState: editableTextState);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SelectableTextState__selectable_text());
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<string>("data", data, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<string>("semanticsLabel", semanticsLabel, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.FocusNode>("focusNode", focusNode, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("style", style, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("autofocus", autofocus, defaultValue: false));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("showCursor", showCursor, defaultValue: false));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("minLines", minLines, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("maxLines", maxLines, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextAlign>("textAlign", textAlign, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", textDirection, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("textScaleFactor", textScaleFactor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextScaler>("textScaler", textScaler, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("cursorWidth", cursorWidth, defaultValue: 2.0));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("cursorHeight", cursorHeight, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Radius>("cursorRadius", cursorRadius, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Color>("cursorColor", cursorColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Color>("selectionColor", selectionColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("selectionEnabled", value: selectionEnabled, defaultValue: true, ifFalse: "selection disabled"));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.TextSelectionControls>("selectionControls", selectionControls, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.ScrollPhysics>("scrollPhysics", scrollPhysics, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.ScrollBehavior>("scrollBehavior", scrollBehavior, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.TextHeightBehavior>("textHeightBehavior", textHeightBehavior, defaultValue: null));
    }

}

internal class _SelectableTextState__selectable_text : global::Doroti.Framework.Widgets.State<SelectableText>, global::Doroti.Framework.Widgets.TextSelectionGestureDetectorBuilderDelegate
{
    internal virtual _TextSpanEditingController__selectable_text _controller { get; set; } = default!;
    internal virtual global::Doroti.Framework.Widgets.FocusNode? _focusNode { get; set; } = default;
    internal virtual bool _showSelectionHandles { get; set; } = false;
    internal virtual _SelectableTextSelectionGestureDetectorBuilder__selectable_text _selectionGestureDetectorBuilder { get; set; } = default!;
    public virtual bool forcePressEnabled { get; set; } = default!;
    public virtual global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.EditableTextState> editableTextKey { get; private set; } = GlobalKey<EditableTextState>.Create();

    internal virtual global::Doroti.Framework.Widgets.EditableTextState? _editableText => editableTextKey.currentState;
    internal virtual global::Doroti.Framework.Widgets.FocusNode _effectiveFocusNode => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.FocusNode>(widget.focusNode ?? (_focusNode ??= new global::Doroti.Framework.Widgets.FocusNode(skipTraversal: true)));
    public virtual bool selectionEnabled => widget.selectionEnabled;
    public override void initState()
    {
        base.initState();
        _selectionGestureDetectorBuilder = new _SelectableTextSelectionGestureDetectorBuilder__selectable_text(state: this);
        _controller = new _TextSpanEditingController__selectable_text(textSpan: widget.textSpan ?? new global::Doroti.Framework.Painting.TextSpan(text: widget.data));
        _controller.addListener(_onControllerChanged);
        _effectiveFocusNode.addListener(_handleFocusChanged);
    }

    public override void didUpdateWidget(SelectableText oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((widget.data != oldWidget.data) || (!Equals(widget.textSpan, oldWidget.textSpan)))
        {
            _controller.removeListener(_onControllerChanged);
            _controller.dispose();
            _controller = new _TextSpanEditingController__selectable_text(textSpan: widget.textSpan ?? new global::Doroti.Framework.Painting.TextSpan(text: widget.data));
            _controller.addListener(_onControllerChanged);
        }
        if (!Equals(widget.focusNode, oldWidget.focusNode))
        {
            (oldWidget.focusNode ?? _focusNode)?.removeListener(_handleFocusChanged);
            (widget.focusNode ?? _focusNode)?.addListener(_handleFocusChanged);
        }
        if (_effectiveFocusNode.hasFocus && _controller.selection.isCollapsed)
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
        _effectiveFocusNode.removeListener(_handleFocusChanged);
        _focusNode?.dispose();
        _controller.dispose();
        base.dispose();
    }

    internal virtual void _onControllerChanged()
    {
        bool showSelectionHandles = !_effectiveFocusNode.hasFocus || !_controller.selection.isCollapsed;
        if (showSelectionHandles == _showSelectionHandles)
        {
            return;
        }
        setState(() =>
        {
            _showSelectionHandles = showSelectionHandles;
        });
    }

    internal virtual void _handleFocusChanged()
    {
        if (!_effectiveFocusNode.hasFocus && Equals(Scheduler.SchedulerBinding.instance.lifecycleState, AppLifecycleState.resumed))
        {
            _controller.value = new global::Doroti.Framework.Services.TextEditingValue(text: _controller.value.text);
        }
    }

    internal virtual void _handleSelectionChanged(global::Doroti.Framework.Services.TextSelection selection, global::Doroti.Framework.Services.SelectionChangedCause? cause)
    {
        bool willShowSelectionHandles = _shouldShowSelectionHandles(cause);
        if (willShowSelectionHandles != _showSelectionHandles)
        {
            setState(() =>
            {
                _showSelectionHandles = willShowSelectionHandles;
            });
        }
        widget.onSelectionChanged?.Invoke(selection, cause);
        switch (Theme.of(context).platform)
        {
            case TargetPlatform.iOS:
            case TargetPlatform.macOS:
                {
                    if (Equals(cause, SelectionChangedCause.longPress))
                    {
                        _editableText?.bringIntoView(selection.@base);
                    }
                    return;
                }
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
                break;
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
    }

    internal virtual void _handleSelectionHandleTapped()
    {
        if (_controller.selection.isCollapsed)
        {
            _editableText!.toggleToolbar();
        }
    }

    internal virtual bool _shouldShowSelectionHandles(global::Doroti.Framework.Services.SelectionChangedCause? cause)
    {
        if (!_selectionGestureDetectorBuilder.shouldShowSelectionToolbar)
        {
            return false;
        }
        if (_controller.selection.isCollapsed)
        {
            return false;
        }
        if (Equals(cause, SelectionChangedCause.keyboard))
        {
            return false;
        }
        if (Equals(cause, SelectionChangedCause.longPress))
        {
            return true;
        }
        if (_controller.text.Length != 0)
        {
            return true;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        DartRuntimePrimitives.Assert(() => !((widget.style is not null) && !widget.style!.inherit && ((widget.style!.fontSize is null) || (widget.style!.textBaseline is null))), () => (object?)"inherit false style must supply fontSize and textBaseline");
        ThemeData theme = Theme.of(context);
        global::Doroti.Framework.Widgets.DefaultSelectionStyle selectionStyle = DefaultSelectionStyle.of(context);
        global::Doroti.Framework.Widgets.FocusNode focusNodeLocal = _effectiveFocusNode;
        global::Doroti.Framework.Widgets.TextSelectionControls? textSelectionControls = widget.selectionControls;
        bool paintCursorAboveTextLocal = default!;
        bool cursorOpacityAnimatesLocal = default!;
        global::Doroti.Ui.Offset? cursorOffsetLocal = default!;
        global::Doroti.Ui.Color cursorColorLocal = default!;
        global::Doroti.Ui.Color selectionColorLocal = default!;
        global::Doroti.Ui.Radius? cursorRadiusLocal = widget.cursorRadius;
        switch (theme.platform)
        {
            case TargetPlatform.iOS:
                {
                    CupertinoThemeData cupertinoTheme = CupertinoTheme.of(context);
                    forcePressEnabled = true;
                    textSelectionControls ??= Text_selectionLibrary.materialTextSelectionHandleControls;
                    paintCursorAboveTextLocal = true;
                    cursorOpacityAnimatesLocal = true;
                    cursorColorLocal = (widget.cursorColor ?? selectionStyle.cursorColor) ?? cupertinoTheme.primaryColor;
                    selectionColorLocal = selectionStyle.selectionColor ?? cupertinoTheme.primaryColor.withOpacity(0.4);
                    cursorRadiusLocal ??= Radius.circular(2.0);
                    cursorOffsetLocal = new global::Doroti.Ui.Offset(Selectable_textLibrary.iOSHorizontalOffset / MediaQuery.devicePixelRatioOf(context), 0);
                    break;
                }
            case TargetPlatform.macOS:
                {
                    CupertinoThemeData cupertinoThemeLocal = CupertinoTheme.of(context);
                    forcePressEnabled = false;
                    textSelectionControls ??= Desktop_text_selectionLibrary.desktopTextSelectionHandleControls;
                    paintCursorAboveTextLocal = true;
                    cursorOpacityAnimatesLocal = true;
                    cursorColorLocal = (widget.cursorColor ?? selectionStyle.cursorColor) ?? cupertinoThemeLocal.primaryColor;
                    selectionColorLocal = selectionStyle.selectionColor ?? cupertinoThemeLocal.primaryColor.withOpacity(0.4);
                    cursorRadiusLocal ??= Radius.circular(2.0);
                    cursorOffsetLocal = new global::Doroti.Ui.Offset(Selectable_textLibrary.iOSHorizontalOffset / MediaQuery.devicePixelRatioOf(context), 0);
                    break;
                }
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
                {
                    forcePressEnabled = false;
                    textSelectionControls ??= Text_selectionLibrary.materialTextSelectionHandleControls;
                    paintCursorAboveTextLocal = false;
                    cursorOpacityAnimatesLocal = false;
                    cursorColorLocal = (widget.cursorColor ?? selectionStyle.cursorColor) ?? theme.colorScheme.primary;
                    selectionColorLocal = selectionStyle.selectionColor ?? theme.colorScheme.primary.withOpacity(0.4);
                    break;
                }
            case TargetPlatform.linux:
            case TargetPlatform.windows:
                {
                    forcePressEnabled = false;
                    textSelectionControls ??= Desktop_text_selectionLibrary.desktopTextSelectionHandleControls;
                    paintCursorAboveTextLocal = false;
                    cursorOpacityAnimatesLocal = false;
                    cursorColorLocal = (widget.cursorColor ?? selectionStyle.cursorColor) ?? theme.colorScheme.primary;
                    selectionColorLocal = selectionStyle.selectionColor ?? theme.colorScheme.primary.withOpacity(0.4);
                    break;
                }
        }
        global::Doroti.Framework.Widgets.DefaultTextStyle defaultTextStyle = DefaultTextStyle.of(context);
        global::Doroti.Framework.Painting.TextStyle? effectiveTextStyle = widget.style;
        if ((effectiveTextStyle is null) || effectiveTextStyle.inherit)
        {
            effectiveTextStyle = defaultTextStyle.style.merge(widget.style ?? _controller._textSpan.style);
        }
        global::Doroti.Framework.Painting.TextScaler? effectiveScaler = widget.textScaler ?? (widget.textScaleFactor switch { null => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.TextScaler>(null), double textScaleFactorLocal => TextScaler.CreateLinear(textScaleFactorLocal) });
        global::Doroti.Framework.Widgets.Widget childLocal = new global::Doroti.Framework.Widgets.RepaintBoundary(child: new global::Doroti.Framework.Widgets.EditableText(key: editableTextKey, style: effectiveTextStyle, readOnly: true, toolbarOptions: widget.toolbarOptions, textWidthBasis: widget.textWidthBasis ?? defaultTextStyle.textWidthBasis, textHeightBehavior: widget.textHeightBehavior ?? defaultTextStyle.textHeightBehavior, showSelectionHandles: _showSelectionHandles, showCursor: widget.showCursor, controller: _controller, focusNode: focusNodeLocal, strutStyle: widget.strutStyle ?? new global::Doroti.Framework.Painting.StrutStyle(), textAlign: (widget.textAlign ?? defaultTextStyle.textAlign) ?? TextAlign.start, textDirection: widget.textDirection, textScaler: effectiveScaler, autofocus: widget.autofocus, forceLine: false, minLines: widget.minLines, maxLines: widget.maxLines ?? defaultTextStyle.maxLines, selectionColor: widget.selectionColor ?? selectionColorLocal, selectionControls: widget.selectionEnabled ? textSelectionControls : null, onSelectionChanged: _handleSelectionChanged, onSelectionHandleTapped: _handleSelectionHandleTapped, rendererIgnoresPointer: true, cursorWidth: widget.cursorWidth, cursorHeight: widget.cursorHeight, cursorRadius: cursorRadiusLocal, cursorColor: cursorColorLocal, selectionHeightStyle: widget.selectionHeightStyle, selectionWidthStyle: widget.selectionWidthStyle, cursorOpacityAnimates: cursorOpacityAnimatesLocal, cursorOffset: cursorOffsetLocal, paintCursorAboveText: paintCursorAboveTextLocal, backgroundCursorColor: CupertinoColors.inactiveGray, enableInteractiveSelection: widget.enableInteractiveSelection, magnifierConfiguration: widget.magnifierConfiguration ?? TextMagnifier.adaptiveMagnifierConfiguration, dragStartBehavior: widget.dragStartBehavior, scrollPhysics: widget.scrollPhysics, scrollBehavior: widget.scrollBehavior, autofillHints: null, contextMenuBuilder: widget.contextMenuBuilder));
        return new global::Doroti.Framework.Widgets.Semantics(label: widget.semanticsLabel, excludeSemantics: widget.semanticsLabel is not null, onLongPress: () =>
        {
            _effectiveFocusNode.requestFocus();
        }, child: _selectionGestureDetectorBuilder.buildGestureDetector(behavior: HitTestBehavior.translucent, child: childLocal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
