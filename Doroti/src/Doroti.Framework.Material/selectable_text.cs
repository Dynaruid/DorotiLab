// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/selectable_text.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class Selectable_textLibrary
{
    public static long iOSHorizontalOffset = -2L;
}

internal class _TextSpanEditingController__selectable_text : TextEditingController
{
    internal virtual TextSpan _textSpan { get; private set; } = default!;

    internal _TextSpanEditingController__selectable_text(TextSpan textSpan)
        : base(text: textSpan.toPlainText(includeSemanticsLabels: false))
    {
        _textSpan = textSpan;
    }

    public override TextSpan buildTextSpan(
        BuildContext context,
        TextStyle? style = null,
        bool withComposing = default!
    )
    {
        return new TextSpan(
            style: style,
            children: new List<TextSpan> { _textSpan }
                .Cast<InlineSpan>()
                .ToList()
        );
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

internal class _SelectableTextSelectionGestureDetectorBuilder__selectable_text
    : TextSelectionGestureDetectorBuilder
{
    internal virtual _SelectableTextState__selectable_text _state { get; private set; } = default!;

    internal _SelectableTextSelectionGestureDetectorBuilder__selectable_text(
        _SelectableTextState__selectable_text state
    )
        : base(@delegate: state)
    {
        _state = state;
    }

    public override void onSingleTapUp(Gestures.TapDragUpDetails details)
    {
        if (!@delegate.selectionEnabled)
        {
            return;
        }
        base.onSingleTapUp(details);
        _state.widget.onTap?.Invoke();
    }
}

public class SelectableText : StatefulWidget
{
    public virtual string? data { get; private set; }
    public virtual TextSpan? textSpan { get; private set; }
    public virtual FocusNode? focusNode { get; private set; }
    public virtual TextStyle? style { get; private set; }
    public virtual Painting.StrutStyle? strutStyle { get; private set; }
    public virtual TextAlign? textAlign { get; private set; }
    public virtual TextDirection? textDirection { get; private set; }
    public virtual double? textScaleFactor { get; private set; }
    public virtual TextScaler? textScaler { get; private set; }
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
    public virtual TextSelectionControls? selectionControls { get; private set; }
    public virtual Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual ToolbarOptions? toolbarOptions { get; private set; }
    public virtual Action? onTap { get; private set; }
    public virtual ScrollPhysics? scrollPhysics { get; private set; }
    public virtual ScrollBehavior? scrollBehavior { get; private set; }
    public virtual string? semanticsLabel { get; private set; }
    public virtual TextHeightBehavior? textHeightBehavior { get; private set; }
    public virtual TextWidthBasis? textWidthBasis { get; private set; }
    public virtual Action<TextSelection, SelectionChangedCause?>? onSelectionChanged
    {
        get;
        private set;
    }
    public virtual Func<BuildContext, EditableTextState, Widget>? contextMenuBuilder
    {
        get;
        private set;
    }
    public virtual TextMagnifierConfiguration? magnifierConfiguration { get; private set; }

    public SelectableText(
        string data,
        Key? key = null,
        FocusNode? focusNode = null,
        TextStyle? style = null,
        Painting.StrutStyle? strutStyle = null,
        TextAlign? textAlign = null,
        TextDirection? textDirection = null,
        double? textScaleFactor = null,
        TextScaler? textScaler = null,
        bool showCursor = false,
        bool autofocus = false,
        ToolbarOptions? toolbarOptions = null,
        long? minLines = null,
        long? maxLines = null,
        double cursorWidth = 2.0,
        double? cursorHeight = null,
        Radius? cursorRadius = null,
        Color? cursorColor = null,
        Color? selectionColor = null,
        BoxHeightStyle? selectionHeightStyle = null,
        BoxWidthStyle? selectionWidthStyle = null,
        Gestures.DragStartBehavior dragStartBehavior = Gestures.DragStartBehavior.start,
        bool enableInteractiveSelection = true,
        TextSelectionControls? selectionControls = null,
        Action? onTap = null,
        ScrollPhysics? scrollPhysics = null,
        ScrollBehavior? scrollBehavior = null,
        string? semanticsLabel = null,
        TextHeightBehavior? textHeightBehavior = null,
        TextWidthBasis? textWidthBasis = null,
        Action<TextSelection, SelectionChangedCause?>? onSelectionChanged = null,
        Func<BuildContext, EditableTextState, Widget>? contextMenuBuilder = default!,
        TextMagnifierConfiguration? magnifierConfiguration = null
    )
        : base(key: key)
    {
        Func<BuildContext, EditableTextState, Widget>? __contextMenuBuilder =
            contextMenuBuilder ?? _defaultContextMenuBuilder;
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
        System.Diagnostics.Debug.Assert(
            (maxLines is null)
                || (
                    (
                        maxLines
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ) > 0L
                )
        );
        System.Diagnostics.Debug.Assert(
            (minLines is null)
                || (
                    (
                        minLines
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ) > 0L
                )
        );
        System.Diagnostics.Debug.Assert(
            maxLines is null
                || minLines is null
                || maxLines
                    >= (
                        minLines
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    )
        );
        System.Diagnostics.Debug.Assert((textScaler is null) || (textScaleFactor is null));
    }

    public static SelectableText CreateRich(
        TextSpan textSpan,
        Key? key = null,
        FocusNode? focusNode = null,
        TextStyle? style = null,
        Painting.StrutStyle? strutStyle = null,
        TextAlign? textAlign = null,
        TextDirection? textDirection = null,
        double? textScaleFactor = null,
        TextScaler? textScaler = null,
        bool showCursor = false,
        bool autofocus = false,
        ToolbarOptions? toolbarOptions = null,
        long? minLines = null,
        long? maxLines = null,
        double cursorWidth = 2.0,
        double? cursorHeight = null,
        Radius? cursorRadius = null,
        Color? cursorColor = null,
        Color? selectionColor = null,
        BoxHeightStyle? selectionHeightStyle = null,
        BoxWidthStyle? selectionWidthStyle = null,
        Gestures.DragStartBehavior dragStartBehavior = Gestures.DragStartBehavior.start,
        bool enableInteractiveSelection = true,
        TextSelectionControls? selectionControls = null,
        Action? onTap = null,
        ScrollPhysics? scrollPhysics = null,
        ScrollBehavior? scrollBehavior = null,
        string? semanticsLabel = null,
        TextHeightBehavior? textHeightBehavior = null,
        TextWidthBasis? textWidthBasis = null,
        Action<TextSelection, SelectionChangedCause?>? onSelectionChanged = null,
        Func<BuildContext, EditableTextState, Widget>? contextMenuBuilder = default!,
        TextMagnifierConfiguration? magnifierConfiguration = null
    )
    {
        var __instance = new SelectableText(
            data: default!,
            key: key,
            focusNode: focusNode,
            style: style,
            strutStyle: strutStyle,
            textAlign: textAlign,
            textDirection: textDirection,
            textScaleFactor: textScaleFactor,
            textScaler: textScaler,
            showCursor: showCursor,
            autofocus: autofocus,
            toolbarOptions: toolbarOptions,
            minLines: minLines,
            maxLines: maxLines,
            cursorWidth: cursorWidth,
            cursorHeight: cursorHeight,
            cursorRadius: cursorRadius,
            cursorColor: cursorColor,
            selectionColor: selectionColor,
            selectionHeightStyle: selectionHeightStyle,
            selectionWidthStyle: selectionWidthStyle,
            dragStartBehavior: dragStartBehavior,
            enableInteractiveSelection: enableInteractiveSelection,
            selectionControls: selectionControls,
            onTap: onTap,
            scrollPhysics: scrollPhysics,
            scrollBehavior: scrollBehavior,
            semanticsLabel: semanticsLabel,
            textHeightBehavior: textHeightBehavior,
            textWidthBasis: textWidthBasis,
            onSelectionChanged: onSelectionChanged,
            contextMenuBuilder: contextMenuBuilder,
            magnifierConfiguration: magnifierConfiguration
        );
        Func<BuildContext, EditableTextState, Widget>? __contextMenuBuilder =
            contextMenuBuilder ?? _defaultContextMenuBuilder;
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

    internal static Widget _defaultContextMenuBuilder(
        BuildContext context,
        EditableTextState editableTextState
    )
    {
        return AdaptiveTextSelectionToolbar.CreateEditableText(
            editableTextState: editableTextState
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _SelectableTextState__selectable_text());

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<string>("data", data, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<string>("semanticsLabel", semanticsLabel, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<FocusNode>("focusNode", focusNode, defaultValue: null)
        );
        properties.add(new DiagnosticsProperty<TextStyle>("style", style, defaultValue: null));
        properties.add(new DiagnosticsProperty<bool>("autofocus", autofocus, defaultValue: false));
        properties.add(
            new DiagnosticsProperty<bool>("showCursor", showCursor, defaultValue: false)
        );
        properties.add(new IntProperty("minLines", minLines, defaultValue: null));
        properties.add(new IntProperty("maxLines", maxLines, defaultValue: null));
        properties.add(new EnumProperty<TextAlign>("textAlign", textAlign, defaultValue: null));
        properties.add(
            new EnumProperty<TextDirection>("textDirection", textDirection, defaultValue: null)
        );
        properties.add(new DoubleProperty("textScaleFactor", textScaleFactor, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<TextScaler>("textScaler", textScaler, defaultValue: null)
        );
        properties.add(new DoubleProperty("cursorWidth", cursorWidth, defaultValue: 2.0));
        properties.add(new DoubleProperty("cursorHeight", cursorHeight, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<Radius>("cursorRadius", cursorRadius, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<Color>("cursorColor", cursorColor, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<Color>("selectionColor", selectionColor, defaultValue: null)
        );
        properties.add(
            new FlagProperty(
                "selectionEnabled",
                value: selectionEnabled,
                defaultValue: true,
                ifFalse: "selection disabled"
            )
        );
        properties.add(
            new DiagnosticsProperty<TextSelectionControls>(
                "selectionControls",
                selectionControls,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<ScrollPhysics>(
                "scrollPhysics",
                scrollPhysics,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<ScrollBehavior>(
                "scrollBehavior",
                scrollBehavior,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<TextHeightBehavior>(
                "textHeightBehavior",
                textHeightBehavior,
                defaultValue: null
            )
        );
    }
}

internal class _SelectableTextState__selectable_text
    : State<SelectableText>,
        TextSelectionGestureDetectorBuilderDelegate
{
    internal virtual _TextSpanEditingController__selectable_text _controller { get; set; } =
        default!;
    internal virtual FocusNode? _focusNode { get; set; } = default;
    internal virtual bool _showSelectionHandles { get; set; } = false;
    internal virtual _SelectableTextSelectionGestureDetectorBuilder__selectable_text _selectionGestureDetectorBuilder { get; set; } =
        default!;
    public virtual bool forcePressEnabled { get; set; } = default!;
    public virtual GlobalKey<EditableTextState> editableTextKey { get; private set; } =
        GlobalKey<EditableTextState>.Create();

    internal virtual EditableTextState? _editableText => editableTextKey.currentState;
    internal virtual FocusNode _effectiveFocusNode =>
        DartRuntimePrimitives.ConvertValue<FocusNode>(
            widget.focusNode ?? (_focusNode ??= new FocusNode(skipTraversal: true))
        );
    public virtual bool selectionEnabled => widget.selectionEnabled;

    public override void initState()
    {
        base.initState();
        _selectionGestureDetectorBuilder =
            new _SelectableTextSelectionGestureDetectorBuilder__selectable_text(state: this);
        _controller = new _TextSpanEditingController__selectable_text(
            textSpan: widget.textSpan ?? new TextSpan(text: widget.data)
        );
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
            _controller = new _TextSpanEditingController__selectable_text(
                textSpan: widget.textSpan ?? new TextSpan(text: widget.data)
            );
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
        bool showSelectionHandles =
            !_effectiveFocusNode.hasFocus || !_controller.selection.isCollapsed;
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
        if (
            !_effectiveFocusNode.hasFocus
            && Equals(Scheduler.SchedulerBinding.instance.lifecycleState, AppLifecycleState.resumed)
        )
        {
            _controller.value = new TextEditingValue(text: _controller.value.text);
        }
    }

    internal virtual void _handleSelectionChanged(
        TextSelection selection,
        SelectionChangedCause? cause
    )
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

    internal virtual bool _shouldShowSelectionHandles(SelectionChangedCause? cause)
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

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        DartRuntimePrimitives.Assert(() =>
            Widgets.DebugLibrary.debugCheckHasDirectionality(context)
        );
        DartRuntimePrimitives.Assert(
            () =>
                !(
                    (widget.style is not null)
                    && !widget.style!.inherit
                    && ((widget.style!.fontSize is null) || (widget.style!.textBaseline is null))
                ),
            () => (object?)"inherit false style must supply fontSize and textBaseline"
        );
        ThemeData theme = Theme.of(context);
        DefaultSelectionStyle selectionStyle = DefaultSelectionStyle.of(context);
        FocusNode focusNodeLocal = _effectiveFocusNode;
        TextSelectionControls? textSelectionControls = widget.selectionControls;
        bool paintCursorAboveTextLocal = default!;
        bool cursorOpacityAnimatesLocal = default!;
        Offset? cursorOffsetLocal = default!;
        Color cursorColorLocal = default!;
        Color selectionColorLocal = default!;
        Radius? cursorRadiusLocal = widget.cursorRadius;
        switch (theme.platform)
        {
            case TargetPlatform.iOS:
            {
                CupertinoThemeData cupertinoTheme = CupertinoTheme.of(context);
                forcePressEnabled = true;
                textSelectionControls ??= Text_selectionLibrary.materialTextSelectionHandleControls;
                paintCursorAboveTextLocal = true;
                cursorOpacityAnimatesLocal = true;
                cursorColorLocal =
                    (widget.cursorColor ?? selectionStyle.cursorColor)
                    ?? cupertinoTheme.primaryColor;
                selectionColorLocal =
                    selectionStyle.selectionColor ?? cupertinoTheme.primaryColor.withOpacity(0.4);
                cursorRadiusLocal ??= Radius.circular(2.0);
                cursorOffsetLocal = new Offset(
                    Selectable_textLibrary.iOSHorizontalOffset
                        / MediaQuery.devicePixelRatioOf(context),
                    0
                );
                break;
            }
            case TargetPlatform.macOS:
            {
                CupertinoThemeData cupertinoThemeLocal = CupertinoTheme.of(context);
                forcePressEnabled = false;
                textSelectionControls ??=
                    Desktop_text_selectionLibrary.desktopTextSelectionHandleControls;
                paintCursorAboveTextLocal = true;
                cursorOpacityAnimatesLocal = true;
                cursorColorLocal =
                    (widget.cursorColor ?? selectionStyle.cursorColor)
                    ?? cupertinoThemeLocal.primaryColor;
                selectionColorLocal =
                    selectionStyle.selectionColor
                    ?? cupertinoThemeLocal.primaryColor.withOpacity(0.4);
                cursorRadiusLocal ??= Radius.circular(2.0);
                cursorOffsetLocal = new Offset(
                    Selectable_textLibrary.iOSHorizontalOffset
                        / MediaQuery.devicePixelRatioOf(context),
                    0
                );
                break;
            }
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            {
                forcePressEnabled = false;
                textSelectionControls ??= Text_selectionLibrary.materialTextSelectionHandleControls;
                paintCursorAboveTextLocal = false;
                cursorOpacityAnimatesLocal = false;
                cursorColorLocal =
                    (widget.cursorColor ?? selectionStyle.cursorColor) ?? theme.colorScheme.primary;
                selectionColorLocal =
                    selectionStyle.selectionColor ?? theme.colorScheme.primary.withOpacity(0.4);
                break;
            }
            case TargetPlatform.linux:
            case TargetPlatform.windows:
            {
                forcePressEnabled = false;
                textSelectionControls ??=
                    Desktop_text_selectionLibrary.desktopTextSelectionHandleControls;
                paintCursorAboveTextLocal = false;
                cursorOpacityAnimatesLocal = false;
                cursorColorLocal =
                    (widget.cursorColor ?? selectionStyle.cursorColor) ?? theme.colorScheme.primary;
                selectionColorLocal =
                    selectionStyle.selectionColor ?? theme.colorScheme.primary.withOpacity(0.4);
                break;
            }
        }
        DefaultTextStyle defaultTextStyle = DefaultTextStyle.of(context);
        TextStyle? effectiveTextStyle = widget.style;
        if ((effectiveTextStyle is null) || effectiveTextStyle.inherit)
        {
            effectiveTextStyle = defaultTextStyle.style.merge(
                widget.style ?? _controller._textSpan.style
            );
        }
        TextScaler? effectiveScaler =
            widget.textScaler
            ?? (
                widget.textScaleFactor switch
                {
                    null => DartRuntimePrimitives.ConvertValue<TextScaler>(null),
                    double textScaleFactorLocal => TextScaler.CreateLinear(textScaleFactorLocal),
                }
            );
        Widget childLocal = new RepaintBoundary(
            child: new EditableText(
                key: editableTextKey,
                style: effectiveTextStyle,
                readOnly: true,
                toolbarOptions: widget.toolbarOptions,
                textWidthBasis: widget.textWidthBasis ?? defaultTextStyle.textWidthBasis,
                textHeightBehavior: widget.textHeightBehavior
                    ?? defaultTextStyle.textHeightBehavior,
                showSelectionHandles: _showSelectionHandles,
                showCursor: widget.showCursor,
                controller: _controller,
                focusNode: focusNodeLocal,
                strutStyle: widget.strutStyle ?? new Painting.StrutStyle(),
                textAlign: (widget.textAlign ?? defaultTextStyle.textAlign) ?? TextAlign.start,
                textDirection: widget.textDirection,
                textScaler: effectiveScaler,
                autofocus: widget.autofocus,
                forceLine: false,
                minLines: widget.minLines,
                maxLines: widget.maxLines ?? defaultTextStyle.maxLines,
                selectionColor: widget.selectionColor ?? selectionColorLocal,
                selectionControls: widget.selectionEnabled ? textSelectionControls : null,
                onSelectionChanged: _handleSelectionChanged,
                onSelectionHandleTapped: _handleSelectionHandleTapped,
                rendererIgnoresPointer: true,
                cursorWidth: widget.cursorWidth,
                cursorHeight: widget.cursorHeight,
                cursorRadius: cursorRadiusLocal,
                cursorColor: cursorColorLocal,
                selectionHeightStyle: widget.selectionHeightStyle,
                selectionWidthStyle: widget.selectionWidthStyle,
                cursorOpacityAnimates: cursorOpacityAnimatesLocal,
                cursorOffset: cursorOffsetLocal,
                paintCursorAboveText: paintCursorAboveTextLocal,
                backgroundCursorColor: CupertinoColors.inactiveGray,
                enableInteractiveSelection: widget.enableInteractiveSelection,
                magnifierConfiguration: widget.magnifierConfiguration
                    ?? TextMagnifier.adaptiveMagnifierConfiguration,
                dragStartBehavior: widget.dragStartBehavior,
                scrollPhysics: widget.scrollPhysics,
                scrollBehavior: widget.scrollBehavior,
                autofillHints: null,
                contextMenuBuilder: widget.contextMenuBuilder
            )
        );
        return new Widgets.Semantics(
            label: widget.semanticsLabel,
            excludeSemantics: widget.semanticsLabel is not null,
            onLongPress: () =>
            {
                _effectiveFocusNode.requestFocus();
            },
            child: _selectionGestureDetectorBuilder.buildGestureDetector(
                behavior: HitTestBehavior.translucent,
                child: childLocal
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
