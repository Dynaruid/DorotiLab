// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/text_form_field.dart
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

public class TextFormField : global::Doroti.Framework.Widgets.FormField<string>
{
    public virtual global::Doroti.Framework.Widgets.TextEditingController? controller { get; private set; }
    public virtual object groupId { get; private set; } = default!;
    public virtual global::System.Action<string>? onChanged { get; private set; }

    public TextFormField(global::Doroti.Framework.Foundation.Key? key = null, object groupId = default!, global::Doroti.Framework.Widgets.TextEditingController? controller = null, string? initialValue = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, string? forceErrorText = null, InputDecoration? decoration = default!, global::Doroti.Framework.Services.TextInputType? keyboardType = null, global::Doroti.Framework.Services.TextCapitalization textCapitalization = global::Doroti.Framework.Services.TextCapitalization.none, global::Doroti.Framework.Services.TextInputAction? textInputAction = null, global::Doroti.Framework.Painting.TextStyle? style = null, global::Doroti.Framework.Painting.StrutStyle? strutStyle = null, TextDirection? textDirection = null, TextAlign textAlign = TextAlign.start, global::Doroti.Framework.Painting.TextAlignVertical? textAlignVertical = null, bool autofocus = false, bool readOnly = false, global::Doroti.Framework.Widgets.ToolbarOptions? toolbarOptions = null, bool? showCursor = null, string obscuringCharacter = "•", bool obscureText = false, bool autocorrect = true, global::Doroti.Framework.Services.SmartDashesType? smartDashesType = null, global::Doroti.Framework.Services.SmartQuotesType? smartQuotesType = null, bool enableSuggestions = true, global::Doroti.Framework.Services.MaxLengthEnforcement? maxLengthEnforcement = null, long? maxLines = 1, long? minLines = null, bool expands = false, long? maxLength = null, global::System.Action<string>? onChanged = null, global::System.Action? onTap = null, bool onTapAlwaysCalled = false, global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>? onTapOutside = null, global::System.Action<global::Doroti.Framework.Gestures.PointerUpEvent>? onTapUpOutside = null, global::System.Action? onEditingComplete = null, global::System.Action<string>? onFieldSubmitted = null, global::System.Action<string?>? onSaved = null, global::System.Func<string?, string?>? validator = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, string, global::Doroti.Framework.Widgets.Widget>? errorBuilder = null, List<global::Doroti.Framework.Services.TextInputFormatter>? inputFormatters = null, bool? enabled = null, bool? ignorePointers = null, double cursorWidth = 2.0, double? cursorHeight = null, Radius? cursorRadius = null, Color? cursorColor = null, Color? cursorErrorColor = null, Brightness? keyboardAppearance = null, global::Doroti.Framework.Painting.EdgeInsets scrollPadding = default!, bool? enableInteractiveSelection = null, bool? selectAllOnFocus = null, global::Doroti.Framework.Widgets.TextSelectionControls? selectionControls = null, InputCounterWidgetBuilder? buildCounter = null, global::Doroti.Framework.Widgets.ScrollPhysics? scrollPhysics = null, IEnumerable<string>? autofillHints = null, global::Doroti.Framework.Widgets.AutovalidateMode? autovalidateMode = null, global::Doroti.Framework.Widgets.ScrollController? scrollController = null, string? restorationId = null, bool enableIMEPersonalizedLearning = true, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget>? contextMenuBuilder = default!, global::Doroti.Framework.Widgets.SpellCheckConfiguration? spellCheckConfiguration = null, global::Doroti.Framework.Widgets.TextMagnifierConfiguration? magnifierConfiguration = null, global::Doroti.Framework.Widgets.UndoHistoryController? undoController = null, global::System.Action<string, DartMap<string, object>>? onAppPrivateCommand = null, bool? cursorOpacityAnimates = null, BoxHeightStyle? selectionHeightStyle = null, BoxWidthStyle? selectionWidthStyle = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.start, global::Doroti.Framework.Widgets.ContentInsertionConfiguration? contentInsertionConfiguration = null, global::Doroti.Framework.Widgets.WidgetStatesController? statesController = null, Clip clipBehavior = Clip.hardEdge, bool scribbleEnabled = true, bool? stylusHandwritingEnabled = null, bool canRequestFocus = true, List<Locale>? hintLocales = null) : base(key: key, forceErrorText: forceErrorText, onSaved: onSaved, validator: validator, errorBuilder: errorBuilder, restorationId: restorationId, initialValue: ((controller is not null) ? ((global::Doroti.Framework.Widgets.TextEditingController)controller).text : ((initialValue ?? ""))), enabled: ((enabled ?? decoration?.enabled) ?? true), autovalidateMode: (autovalidateMode ?? global::Doroti.Framework.Widgets.AutovalidateMode.disabled), builder: ((global::System.Func<global::Doroti.Framework.Widgets.FormFieldState<string>, global::Doroti.Framework.Widgets.Widget>)((field) => {
var state__8775 = ((_TextFormFieldState__text_form_field?)(object?)field)!;
InputDecoration effectiveDecoration__8840 = ((decoration ?? new InputDecoration())).applyDefaults(InputDecorationTheme.of(((_TextFormFieldState__text_form_field)field).context));
string? errorText__8999 = ((_TextFormFieldState__text_form_field)field).errorText;
if ((errorText__8999 is not null))
{
    effectiveDecoration__8840 = ((errorBuilder is not null) ? effectiveDecoration__8840.copyWith(error: errorBuilder(state__8775.context, errorText__8999)) : effectiveDecoration__8840.copyWith(errorText: errorText__8999));
}
void onChangedHandler(string value)
{
    ((_TextFormFieldState__text_form_field)field).didChange(value);
    onChanged?.Invoke(value);
}
return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.UnmanagedRestorationScope(bucket: ((_TextFormFieldState__text_form_field)field).bucket, child: new TextField(groupId: groupId ?? typeof(global::Doroti.Framework.Widgets.EditableText), restorationId: restorationId, controller: ((_TextFormFieldState__text_form_field)state__8775)._effectiveController, focusNode: focusNode, decoration: effectiveDecoration__8840, keyboardType: keyboardType, textInputAction: textInputAction, style: style, strutStyle: strutStyle, textAlign: textAlign, textAlignVertical: textAlignVertical, textDirection: textDirection, textCapitalization: textCapitalization, autofocus: autofocus, statesController: statesController, toolbarOptions: toolbarOptions, readOnly: readOnly, showCursor: showCursor, obscuringCharacter: obscuringCharacter, obscureText: obscureText, autocorrect: autocorrect, smartDashesType: (smartDashesType ?? ((obscureText ? global::Doroti.Framework.Services.SmartDashesType.disabled : global::Doroti.Framework.Services.SmartDashesType.enabled))), smartQuotesType: (smartQuotesType ?? ((obscureText ? global::Doroti.Framework.Services.SmartQuotesType.disabled : global::Doroti.Framework.Services.SmartQuotesType.enabled))), enableSuggestions: enableSuggestions, maxLengthEnforcement: maxLengthEnforcement, maxLines: maxLines, minLines: minLines, expands: expands, maxLength: maxLength, onChanged: (global::System.Action<string>)onChangedHandler, onTap: onTap, onTapAlwaysCalled: onTapAlwaysCalled, onTapOutside: (global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>?)onTapOutside, onTapUpOutside: (global::System.Action<global::Doroti.Framework.Gestures.PointerUpEvent>?)onTapUpOutside, onEditingComplete: onEditingComplete, onSubmitted: (global::System.Action<string>?)onFieldSubmitted, inputFormatters: inputFormatters, enabled: ((enabled ?? decoration?.enabled) ?? true), ignorePointers: ignorePointers, cursorWidth: cursorWidth, cursorHeight: cursorHeight, cursorRadius: cursorRadius, cursorColor: cursorColor, cursorErrorColor: cursorErrorColor, scrollPadding: scrollPadding ?? global::Doroti.Framework.Painting.EdgeInsets.CreateAll(20.0), scrollPhysics: scrollPhysics, keyboardAppearance: keyboardAppearance, enableInteractiveSelection: (enableInteractiveSelection ?? ((!obscureText || !readOnly))), selectAllOnFocus: selectAllOnFocus, selectionControls: selectionControls, buildCounter: (InputCounterWidgetBuilder?)buildCounter, autofillHints: autofillHints, scrollController: scrollController, enableIMEPersonalizedLearning: enableIMEPersonalizedLearning, mouseCursor: mouseCursor, contextMenuBuilder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget>?)contextMenuBuilder, spellCheckConfiguration: spellCheckConfiguration, magnifierConfiguration: magnifierConfiguration, undoController: undoController, onAppPrivateCommand: (global::System.Action<string, DartMap<string, object>>?)onAppPrivateCommand, cursorOpacityAnimates: cursorOpacityAnimates, selectionHeightStyle: ((selectionHeightStyle ?? (BoxHeightStyle)global::Doroti.Framework.Widgets.EditableText.defaultSelectionHeightStyle)), selectionWidthStyle: ((selectionWidthStyle ?? (BoxWidthStyle)global::Doroti.Framework.Widgets.EditableText.defaultSelectionWidthStyle)), dragStartBehavior: dragStartBehavior, contentInsertionConfiguration: contentInsertionConfiguration, clipBehavior: clipBehavior, scribbleEnabled: scribbleEnabled, stylusHandwritingEnabled: DartRuntimePrimitives.RequireValue(stylusHandwritingEnabled), canRequestFocus: canRequestFocus, hintLocales: hintLocales)));
throw new InvalidOperationException("Dart closure completed without a value.");
})))
    {
        object __groupId = groupId ?? typeof(global::Doroti.Framework.Widgets.EditableText);
        InputDecoration? __decoration = decoration ?? new InputDecoration();
        global::Doroti.Framework.Painting.EdgeInsets __scrollPadding = scrollPadding ?? global::Doroti.Framework.Painting.EdgeInsets.CreateAll(20.0);
        global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget>? __contextMenuBuilder = contextMenuBuilder ?? _defaultContextMenuBuilder;
        bool __stylusHandwritingEnabled = stylusHandwritingEnabled ?? global::Doroti.Framework.Widgets.EditableText.defaultStylusHandwritingEnabled;
        this.groupId = __groupId;
        this.controller = controller;
        this.onChanged = onChanged;
        System.Diagnostics.Debug.Assert(((initialValue is null) || (controller is null)));
        System.Diagnostics.Debug.Assert((obscuringCharacter.Length == 1L));
        System.Diagnostics.Debug.Assert(((maxLines is null) || (DartRuntimePrimitives.RequireValue(maxLines) > 0L)));
        System.Diagnostics.Debug.Assert(((minLines is null) || (DartRuntimePrimitives.RequireValue(minLines) > 0L)));
        System.Diagnostics.Debug.Assert(((((maxLines is null)) || ((minLines is null))) || ((maxLines >= DartRuntimePrimitives.RequireValue(minLines)))));
        System.Diagnostics.Debug.Assert((!expands || (((maxLines is null) && (minLines is null)))));
        System.Diagnostics.Debug.Assert((!obscureText || (maxLines == 1L)));
        System.Diagnostics.Debug.Assert((((maxLength is null) || (DartRuntimePrimitives.RequireValue(maxLength) == TextField.noMaxLength)) || (DartRuntimePrimitives.RequireValue(maxLength) > 0L)));
        System.Diagnostics.Debug.Assert(((errorBuilder is null) || (__decoration?.errorText is null)));
    }

    internal static global::Doroti.Framework.Widgets.Widget _defaultContextMenuBuilder(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.EditableTextState editableTextState)
    {
        if (SystemContextMenu.isSupportedByField(editableTextState))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.SystemContextMenu.CreateEditableText(editableTextState: editableTextState));
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)AdaptiveTextSelectionToolbar.CreateEditableText(editableTextState: editableTextState));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _TextFormFieldState__text_form_field());
}

internal class _TextFormFieldState__text_form_field : global::Doroti.Framework.Widgets.FormFieldState<string>
{
    internal virtual global::Doroti.Framework.Widgets.RestorableTextEditingController? _controller { get; set; } = default;
    internal virtual string? _initialValue { get; private set; }

    internal virtual global::Doroti.Framework.Widgets.TextEditingController _effectiveController => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.TextEditingController>((((TextFormField)this._textFormField).controller ?? this._controller!.value));
    internal virtual TextFormField _textFormField => ((TextFormField?)(object?)base.widget)!;
    public override void restoreState(global::Doroti.Framework.Services.RestorationBucket? oldBucket, bool initialRestore)
    {
        base.restoreState(oldBucket, initialRestore);
        if ((this._controller is not null))
        {
            _registerController();
        }
        setValue(((global::Doroti.Framework.Widgets.TextEditingController)this._effectiveController).text);
    }

    internal virtual void _registerController()
    {
        DartRuntimePrimitives.Assert(() => (this._controller is not null));
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this._controller!), "controller");
    }

    internal virtual void _createLocalController(global::Doroti.Framework.Services.TextEditingValue? value = null)
    {
        DartRuntimePrimitives.Assert(() => (this._controller is null));
        _controller = ((value is null) ? global::Doroti.Framework.Widgets.RestorableTextEditingController.Create() : new global::Doroti.Framework.Widgets.RestorableTextEditingController(value));
        if (!this.restorePending)
        {
            _registerController();
        }
    }

    public override void initState()
    {
        base.initState();
        if ((((TextFormField)this._textFormField).controller is null))
        {
            _createLocalController(((this.widget.initialValue is not null) ? new global::Doroti.Framework.Services.TextEditingValue(text: this.widget.initialValue!) : null));
        }
        else
        {
            ((TextFormField)this._textFormField).controller!.addListener(() => this._handleControllerChanged());
        }
        _initialValue = (this._textFormField.initialValue ?? ((TextFormField)this._textFormField).controller?.text);
    }

    public override void didUpdateWidget(global::Doroti.Framework.Widgets.FormField<string> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((TextFormField)this._textFormField).controller, ((TextFormField)oldWidget).controller)))
        {
            ((TextFormField)oldWidget).controller?.removeListener(() => this._handleControllerChanged());
            ((TextFormField)this._textFormField).controller?.addListener(() => this._handleControllerChanged());
            if (((((TextFormField)oldWidget).controller is not null) && (((TextFormField)this._textFormField).controller is null)))
            {
                _createLocalController(((TextFormField)oldWidget).controller!.value);
            }
            if ((((TextFormField)this._textFormField).controller is not null))
            {
                setValue(((TextFormField)this._textFormField).controller!.text);
                if ((((TextFormField)oldWidget).controller is null))
                {
                    unregisterFromRestoration(this._controller!);
                    this._controller!.dispose();
                    _controller = null;
                }
            }
        }
    }

    public override void dispose()
    {
        ((TextFormField)this._textFormField).controller?.removeListener(() => this._handleControllerChanged());
        this._controller?.dispose();
        base.dispose();
    }

    public override void didChange(string? value)
    {
        base.didChange(value);
        if ((((global::Doroti.Framework.Widgets.TextEditingController)this._effectiveController).text != value))
        {
            this._effectiveController.value = new global::Doroti.Framework.Services.TextEditingValue(text: (value ?? ""));
        }
    }

    public override void reset()
    {
        this._effectiveController.value = new global::Doroti.Framework.Services.TextEditingValue(text: (this._initialValue ?? ""));
        base.reset();
        ((TextFormField)this._textFormField).onChanged?.Invoke(((global::Doroti.Framework.Widgets.TextEditingController)this._effectiveController).text);
    }

    internal virtual void _handleControllerChanged()
    {
        if ((((global::Doroti.Framework.Widgets.TextEditingController)this._effectiveController).text != this.value))
        {
            didChange(((global::Doroti.Framework.Widgets.TextEditingController)this._effectiveController).text);
        }
    }

}
