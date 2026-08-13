// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/form.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public static partial class FormLibrary
{
    internal static Duration _kIOSAnnouncementDelayDuration = Duration.Create(seconds: 1L);
}

public class Form : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual global::System.Func<Future<bool>>? onWillPop { get; private set; }
    public virtual bool? canPop { get; private set; }
    public virtual global::System.Action<bool>? onPopInvoked { get; private set; }
    public virtual global::System.Action<bool, object>? onPopInvokedWithResult { get; private set; }
    public virtual global::System.Action? onChanged { get; private set; }
    public virtual AutovalidateMode autovalidateMode { get; private set; } = default!;

    public Form(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget child = default!, bool? canPop = null, global::System.Action<bool>? onPopInvoked = null, global::System.Action<bool, object>? onPopInvokedWithResult = null, global::System.Func<Future<bool>>? onWillPop = null, global::System.Action? onChanged = null, AutovalidateMode? autovalidateMode = null) : base(key: key)
    {
        this.child = child;
        this.canPop = canPop;
        this.onPopInvoked = onPopInvoked;
        this.onPopInvokedWithResult = onPopInvokedWithResult;
        this.onWillPop = onWillPop;
        this.onChanged = onChanged;
        this.autovalidateMode = (autovalidateMode ?? AutovalidateMode.disabled);
        System.Diagnostics.Debug.Assert(((onPopInvokedWithResult is null) || (onPopInvoked is null)));
        System.Diagnostics.Debug.Assert((((((((object?)((onPopInvokedWithResult ?? (object?)onPopInvoked)) ?? (object?)canPop))) is null)) || (onWillPop is null)));
    }

    public static FormState? maybeOf(BuildContext context)
    {
        _FormScope__form? scope__3743 = ((_FormScope__form?)(object?)context.dependOnInheritedWidgetOfExactType<_FormScope__form>());
        return scope__3743?._formState;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static FormState of(BuildContext context)
    {
        FormState? formState__4508 = ((FormState?)(object?)Form.maybeOf(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((formState__4508 is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Generated.Framework.Foundation.FlutterError.Create("Form.of() was called with a context that does not contain a Form widget.\n" + "No Form widget ancestor could be found starting from the context that " + "was passed to Form.of(). This can happen because you are using a widget " + "that looks for a Form ancestor, but no such ancestor exists.\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return formState__4508!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _callPopInvoked(bool didPop, object? result)
    {
        if ((this.onPopInvokedWithResult is not null))
        {
            this.onPopInvokedWithResult!(didPop, result);
            return;
        }
        this.onPopInvoked?.Invoke(didPop);
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new FormState());
}

public class FormState : State<Form>
{
    internal virtual long _generation { get; set; } = 0L;
    internal virtual bool _hasInteractedByUser { get; set; } = false;
    internal virtual HashSet<dynamic> _fields { get; private set; } = new HashSet<dynamic>();

    public virtual IEnumerable<object> fields => DartRuntimePrimitives.ConvertValue<IEnumerable<object>>(this._fields);
    internal virtual void _fieldDidChange()
    {
        ((Form)(object)this.widget).onChanged?.Invoke();
        _hasInteractedByUser = this._fields.any(((field) => ((RestorableBool)((dynamic)field)._hasInteractedByUser).value));
        _forceRebuild();
    }

    internal virtual void _forceRebuild()
    {
        setState(((global::System.Action)(() => {
++_generation;
})));
    }

    internal virtual void _register(dynamic field)
    {
        this._fields.Add(field);
    }

    internal virtual void _unregister(dynamic field)
    {
        this._fields.Remove(field);
    }

    public override Widget build(BuildContext context)
    {
        bool hasError__9097 = this._fields.any(((field) => ((bool)((dynamic)field).hasError)));
        switch (((Form)(object)this.widget).autovalidateMode)
        {
            case AutovalidateMode.always:
                {
                    _validate(View.of(context));
                    break;
                }
            case AutovalidateMode.onUserInteraction:
                {
                    if (this._hasInteractedByUser)
                    {
                        _validate(View.of(context));
                    }
                    break;
                }
            case AutovalidateMode.onUserInteractionIfError:
                {
                    if ((this._hasInteractedByUser && hasError__9097))
                    {
                        _validate(View.of(context));
                    }
                    break;
                }
            case AutovalidateMode.onUnfocus:
            case AutovalidateMode.disabled:
                {
                    break;
                }
        }
        Widget form__9684 = default!;
        if (((((Form)(object)this.widget).canPop is not null) || ((((((Form)(object)this.widget).onPopInvokedWithResult ?? (object?)((Form)(object)this.widget).onPopInvoked))) is not null)))
        {
            form__9684 = DartRuntimePrimitives.ConvertValue<Widget>(new PopScope<object?>(canPop: (((Form)(object)this.widget).canPop ?? true), onPopInvokedWithResult: (global::System.Action<bool, object?>)((Form)(object)this.widget)._callPopInvoked, child: new _FormScope__form(formState: this, generation: this._generation, child: ((Form)(object)this.widget).child)));
        }
        else
        {
            form__9684 = DartRuntimePrimitives.ConvertValue<Widget>(new WillPopScope(onWillPop: (global::System.Func<Future<bool>>?)((Form)(object)this.widget).onWillPop, child: new _FormScope__form(formState: this, generation: this._generation, child: ((Form)(object)this.widget).child)));
        }
        return ((Widget)(object?)new Semantics(container: true, explicitChildNodes: true, role: SemanticsRole.form, child: form__9684));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void save()
    {
        foreach (dynamic field__10459 in this._fields)
        {
            ((dynamic)field__10459).save();
        }
    }

    public virtual void reset()
    {
        foreach (dynamic field__10884 in this._fields)
        {
            ((dynamic)field__10884).reset();
        }
        _hasInteractedByUser = false;
        _fieldDidChange();
    }

    public virtual void clearError()
    {
        foreach (dynamic field__11277 in this._fields)
        {
            ((dynamic)field__11277)._clearErrorInternal();
        }
        _fieldDidChange();
    }

    public virtual bool validate()
    {
        _hasInteractedByUser = true;
        _forceRebuild();
        return _validate(View.of(this.context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual HashSet<object> validateGranularly()
    {
        var invalidFields__12300 = new HashSet<dynamic>();
        _hasInteractedByUser = true;
        _forceRebuild();
        _validate(View.of(this.context), invalidFields__12300);
        return ((HashSet<object>)(object?)invalidFields__12300);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _validate(FlutterView view, HashSet<dynamic>? invalidFields = null)
    {
        var hasError__12570 = false;
        var errorMessage__12596 = "";
        var validateOnFocusChange__12625 = (object.Equals(((Form)(object)this.widget).autovalidateMode, AutovalidateMode.onUnfocus));
        foreach (dynamic field__12744 in this._fields)
        {
            bool hasFocus__12781 = ((FocusNode)((dynamic)field__12744)._focusNode).hasFocus;
            if (((!validateOnFocusChange__12625 || !hasFocus__12781) || ((validateOnFocusChange__12625 && hasFocus__12781))))
            {
                bool isFieldValid__12927 = ((bool)((dynamic)field__12744).validate());
                hasError__12570 |= !isFieldValid__12927;
                if ((errorMessage__12596.Length == 0))
                {
                    errorMessage__12596 = (((string?)((dynamic)field__12744).errorText) ?? "");
                }
                if (((invalidFields is not null) && !isFieldValid__12927))
                {
                    invalidFields.Add(field__12744);
                }
            }
        }
        if (((errorMessage__12596.Length != 0) && MediaQuery.supportsAnnounceOf(this.context)))
        {
            global::Doroti.Flutter.Ui.TextDirection directionality__13387 = Directionality.of(this.context);
            if ((object.Equals(global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS)))
            {
                global::Doroti.Flutter.Runtime.DartAsyncRuntime.unawaited(new Future((async () => {
await new Future(FormLibrary._kIOSAnnouncementDelayDuration);
try
{
    await SemanticsService.sendAnnouncement(view, errorMessage__12596, directionality__13387, assertiveness: global::Doroti.Generated.Framework.Semantics.Assertiveness.assertive);
}
catch (Exception exception__13865)
{
    var stack__13876 = new System.Diagnostics.StackTrace();
    FlutterError.reportError(new global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails(exception: exception__13865, stack: stack__13876, library: "widgets library", context: new global::Doroti.Generated.Framework.Foundation.ErrorDescription("while sending semantics announcement")));
}
throw new InvalidOperationException("Dart closure completed without a value.");
})));
            }
            else
            {
                DartRuntimePrimitives.Ignore(SemanticsService.sendAnnouncement(view, errorMessage__12596, directionality__13387, assertiveness: global::Doroti.Generated.Framework.Semantics.Assertiveness.assertive).catchError(((global::System.Action<object, global::System.Diagnostics.StackTrace>)((exception, stack) => {
FlutterError.reportError(new global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails(exception: exception, stack: stack, library: "widgets library", context: new global::Doroti.Generated.Framework.Foundation.ErrorDescription("while sending semantics announcement")));
}))));
            }
        }
        return !hasError__12570;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _FormScope__form : InheritedWidget
{
    internal virtual FormState _formState { get; private set; } = default!;
    internal virtual long _generation { get; private set; } = default!;

    internal _FormScope__form(Widget child, FormState formState, long generation) : base(child: child)
    {
        this._formState = formState;
        this._generation = generation;
    }

    public virtual Form form => this._formState.widget;
    public override bool updateShouldNotify(InheritedWidget oldWidget) => (this._generation != ((_FormScope__form)oldWidget)._generation);
}

public delegate string? FormFieldValidator<T>(T? value);

public delegate Widget FormFieldErrorBuilder(BuildContext context, string errorText);

public delegate void FormFieldSetter<T>(T? newValue);

public delegate Widget FormFieldBuilder<T>(FormFieldState<T> field);

public class FormField<T> : StatefulWidget
{
    public virtual global::System.Func<FormFieldState<T>, Widget> builder { get; private set; } = default!;
    public virtual global::System.Action<T?>? onSaved { get; private set; }
    public virtual global::System.Action? onReset { get; private set; }
    public virtual string? forceErrorText { get; private set; }
    public virtual global::System.Func<T?, string?>? validator { get; private set; }
    public virtual global::System.Func<BuildContext, string, Widget>? errorBuilder { get; private set; }
    public virtual T? initialValue { get; private set; }
    public virtual bool enabled { get; private set; } = default!;
    public virtual AutovalidateMode autovalidateMode { get; private set; } = default!;
    public virtual string? restorationId { get; private set; }

    public FormField(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Func<FormFieldState<T>, Widget> builder = default!, global::System.Action<T?>? onSaved = null, global::System.Action? onReset = null, string? forceErrorText = null, global::System.Func<T?, string?>? validator = null, global::System.Func<BuildContext, string, Widget>? errorBuilder = null, T? initialValue = default, bool enabled = true, AutovalidateMode? autovalidateMode = null, string? restorationId = null) : base(key: key)
    {
        this.builder = builder;
        this.onSaved = onSaved;
        this.onReset = onReset;
        this.forceErrorText = forceErrorText;
        this.validator = validator;
        this.errorBuilder = errorBuilder;
        this.initialValue = initialValue;
        this.enabled = enabled;
        this.restorationId = restorationId;
        this.autovalidateMode = (autovalidateMode ?? AutovalidateMode.disabled);
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new FormFieldState<T>());
}

public class FormFieldState<T> : State<FormField<T>>, RestorationMixin<FormField<T>>
{
    private bool __late__value_initialized;
    private T? __late__value = default!;
    internal virtual T? _value
    {
        get
        {
            if (!__late__value_initialized)
            {
                __late__value = ((FormField<T>)(object)this.widget).initialValue;
                __late__value_initialized = true;
            }
            return __late__value;
        }
        set { __late__value = value; __late__value_initialized = true; }
    }
    internal virtual RestorableStringN _errorText { get; private set; } = default!;
    internal virtual RestorableBool _hasInteractedByUser { get; private set; } = new RestorableBool(false);
    internal virtual FocusNode _focusNode { get; private set; } = new FocusNode();
    public virtual global::Doroti.Generated.Framework.Services.RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<dynamic, global::System.Action> _properties { get; set; } = new DartMap<dynamic, global::System.Action>();
    public virtual List<dynamic>? _debugPropertiesWaitingForReregistration { get; set; } = default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual global::Doroti.Generated.Framework.Services.RestorationBucket? _currentParent { get; set; } = default;

    public virtual T? value => this._value;
    public virtual string? errorText => this._errorText.value;
    public virtual bool hasError => DartRuntimePrimitives.ConvertValue<bool>((this._errorText.value is not null));
    public virtual bool hasInteractedByUser => this._hasInteractedByUser.value;
    public virtual bool isValid => DartRuntimePrimitives.ConvertValue<bool>(((((FormField<T>)(object)this.widget).forceErrorText is null) && (((FormField<T>)(object)this.widget).validator?.Invoke(this._value) is null)));
    public virtual void save()
    {
        ((FormField<T>)(object)this.widget).onSaved?.Invoke(this.value);
    }

    public virtual void reset()
    {
        setState(((global::System.Action)(() => {
_value = ((FormField<T>)(object)this.widget).initialValue;
_clearErrorInternal();
})));
        ((FormField<T>)(object)this.widget).onReset?.Invoke();
        Form.maybeOf(this.context)?._fieldDidChange();
    }

    public virtual void clearError()
    {
        setState(((global::System.Action)(() => {
_clearErrorInternal();
})));
        Form.maybeOf(this.context)?._fieldDidChange();
    }

    public virtual bool validate()
    {
        setState(((global::System.Action)(() => {
_validate();
})));
        return !this.hasError;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _clearErrorInternal()
    {
        this._errorText.value = null;
        this._hasInteractedByUser.value = false;
    }

    internal virtual void _validate()
    {
        if ((((FormField<T>)(object)this.widget).forceErrorText is not null))
        {
            this._errorText.value = ((FormField<T>)(object)this.widget).forceErrorText;
            return;
        }
        if ((((FormField<T>)(object)this.widget).validator is not null))
        {
            this._errorText.value = ((FormField<T>)(object)this.widget).validator!(this._value);
        }
        else
        {
            this._errorText.value = null;
        }
    }

    public virtual void didChange(T? value)
    {
        setState(((global::System.Action)(() => {
_value = value;
this._hasInteractedByUser.value = true;
})));
        Form.maybeOf(this.context)?._fieldDidChange();
    }

    public virtual void setValue(T? value)
    {
        _value = value;
    }

    public virtual string? restorationId => ((FormField<T>)(object)this.widget).restorationId;
    public virtual void restoreState(global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket, bool initialRestore)
    {
        registerForRestoration(this._errorText, "error_text");
        registerForRestoration(this._hasInteractedByUser, "has_interacted_by_user");
    }

    public override void deactivate()
    {
        Form.maybeOf(this.context)?._unregister(this);
        base.deactivate();
    }

    public override void initState()
    {
        base.initState();
        _errorText = new RestorableStringN(((FormField<T>)(object)this.widget).forceErrorText);
    }

    public override void didUpdateWidget(FormField<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        didUpdateRestorationId();
        if ((((FormField<T>)(object)this.widget).forceErrorText != ((FormField<T>)oldWidget).forceErrorText))
        {
            this._errorText.value = ((FormField<T>)(object)this.widget).forceErrorText;
        }
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket__41020 = this._bucket;
        bool needsRestore__41056 = this.restorePending;
        _currentParent = RestorationScope.maybeOf(this.context);
        bool didReplaceBucket__41159 = _updateBucketIfNecessary(parent: this._currentParent, restorePending: needsRestore__41056);
        if (needsRestore__41056)
        {
            _doRestore(oldBucket__41020);
        }
        if (didReplaceBucket__41159)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket__41020, this._bucket)));
            oldBucket__41020?.dispose();
        }
        switch (Form.maybeOf(this.context)?.widget.autovalidateMode)
        {
            case AutovalidateMode.always:
                {
                    WidgetsBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) => {
if (((((FormField<T>)(object)this.widget).enabled && !this.hasError) && !this.isValid))
{
    validate();
}
})));
                    break;
                }
            case AutovalidateMode.onUnfocus:
            case AutovalidateMode.onUserInteraction:
            case AutovalidateMode.onUserInteractionIfError:
            case AutovalidateMode.disabled:
            case null:
                {
                    break;
                }
        }
    }

    public override void dispose()
    {
        this._errorText.dispose();
        this._focusNode.dispose();
        this._hasInteractedByUser.dispose();
        this._properties.forEach(((global::System.Action<dynamic, global::System.Action>)((property, listener) => {
if (!((dynamic)property)._disposed)
{
    property.removeListener((global::System.Action)(() => listener()));
}
})));
        this._bucket?.dispose();
        _bucket = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        if (((FormField<T>)(object)this.widget).enabled)
        {
            switch (((FormField<T>)(object)this.widget).autovalidateMode)
            {
                case AutovalidateMode.always:
                    {
                        _validate();
                        break;
                    }
                case AutovalidateMode.onUserInteraction:
                    {
                        if (this._hasInteractedByUser.value)
                        {
                            _validate();
                        }
                        break;
                    }
                case AutovalidateMode.onUserInteractionIfError:
                    {
                        if ((this._hasInteractedByUser.value && this.hasError))
                        {
                            _validate();
                        }
                        break;
                    }
                case AutovalidateMode.onUnfocus:
                case AutovalidateMode.disabled:
                    {
                        break;
                    }
            }
        }
        Form.maybeOf(context)?._register(this);
        Widget child__28942 = ((Widget)(object?)new Semantics(validationResult: (this.hasError ? SemanticsValidationResult.invalid : SemanticsValidationResult.valid), child: this.widget.builder(this)));
        if ((((object.Equals(Form.maybeOf(context)?.widget.autovalidateMode, AutovalidateMode.onUnfocus)) && (!object.Equals(((FormField<T>)(object)this.widget).autovalidateMode, AutovalidateMode.always))) || (object.Equals(((FormField<T>)(object)this.widget).autovalidateMode, AutovalidateMode.onUnfocus))))
        {
            return ((Widget)(object?)new Focus(canRequestFocus: false, skipTraversal: true, onFocusChange: ((global::System.Action<bool>)((value) => {
if (!DartRuntimePrimitives.RequireValue(value))
{
    setState(((global::System.Action)(() => {
_validate();
})));
}
})), focusNode: this._focusNode, child: child__28942));
        }
        return child__28942;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Services.RestorationBucket? bucket => this._bucket;
    public virtual void didToggleBucket(global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => (this._bucket?.isReplacing != true));
    }

    public virtual void registerForRestoration(dynamic property, string restorationId)
    {
        DartRuntimePrimitives.Assert(() => ((((dynamic)property)._restorationId is null) || ((this._debugDoingRestore && (((dynamic)property)._restorationId == restorationId)))), () => (object?)$"Property is already registered under {((dynamic)property)._restorationId}.");
        DartRuntimePrimitives.Assert(() => (this._debugDoingRestore || !this._properties.Keys.map<dynamic, string?>(((r) => ((dynamic)r)._restorationId)).contains(restorationId)), () => (object?)$"\"{restorationId}\" is already registered to another property.");
        bool hasSerializedValue__36723 = (this.bucket?.contains(restorationId) ?? false);
        object? initialValue__36804 = (hasSerializedValue__36723 ? property.fromPrimitives(this.bucket!.read<object>(restorationId)) : property.createDefaultValue());
        if (!((dynamic)property).isRegistered)
        {
            property._register(restorationId, this);
            void listener()
            {
                if ((this.bucket is null))
                {
                    return;
                }
                _updateProperty(property);
            }
            property.addListener((global::System.Action)(() => listener()));
            this._properties[property] = (global::System.Action)listener;
        }
        DartRuntimePrimitives.Assert(() => (((((dynamic)property)._restorationId == restorationId) && (object.Equals(((dynamic)property)._owner, this))) && this._properties.ContainsKey(property)));
        property.initWithValue((dynamic)initialValue__36804);
        if (((!hasSerializedValue__36723 && ((dynamic)property).enabled) && (this.bucket is not null)))
        {
            _updateProperty(property);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public virtual void unregisterFromRestoration(dynamic property)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((dynamic)property)._owner, this)));
        this._bucket?.remove<object?>(((dynamic)property)._restorationId!);
        _unregister(property);
    }

    public virtual void didUpdateRestorationId()
    {
        if ((((this._currentParent is null) || (this._bucket?.restorationId == this.restorationId)) || this.restorePending))
        {
            return;
        }
        global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket__39230 = this._bucket;
        DartRuntimePrimitives.Assert(() => !this.restorePending);
        bool didReplaceBucket__39295 = _updateBucketIfNecessary(parent: this._currentParent, restorePending: false);
        if (didReplaceBucket__39295)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket__39230, this._bucket)));
            DartRuntimePrimitives.Assert(() => ((this._bucket is null) || (oldBucket__39230 is null)));
            oldBucket__39230?.dispose();
        }
    }

    public virtual bool restorePending
    {
        get
        {
            if (this._firstRestorePending)
            {
                return true;
            }
            if ((this.restorationId is null))
            {
                return false;
            }
            global::Doroti.Generated.Framework.Services.RestorationBucket? potentialNewParent__40517 = ((global::Doroti.Generated.Framework.Services.RestorationBucket?)(object?)RestorationScope.maybeOf(this.context));
            return ((!object.Equals(potentialNewParent__40517, this._currentParent)) && ((potentialNewParent__40517?.isReplacing ?? false)));
            return default!;
        }
    }
    public virtual bool _debugDoingRestore => DartRuntimePrimitives.ConvertValue<bool>((this._debugPropertiesWaitingForReregistration is not null));
    public virtual void _doRestore(global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration = this._properties.Keys.ToList();
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        restoreState(oldBucket, this._firstRestorePending);
        this._firstRestorePending = false;
        DartRuntimePrimitives.Assert(() =>
            {
                if (System.Linq.Enumerable.Any(this._debugPropertiesWaitingForReregistration!))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary("Previously registered RestorableProperties must be re-registered in \"restoreState\"."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"The RestorableProperties with the following IDs were not re-registered to {this} when " + "\"restoreState\" was called:") }));
                }
                this._debugPropertiesWaitingForReregistration = null;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public virtual bool _updateBucketIfNecessary(global::Doroti.Generated.Framework.Services.RestorationBucket? parent, bool restorePending)
    {
        if (((this.restorationId is null) || (parent is null)))
        {
            bool didReplace__42801 = _setNewBucketIfNecessary(newBucket: ((global::Doroti.Generated.Framework.Services.RestorationBucket)(object)null), restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (this._bucket is null));
            return didReplace__42801;
        }
        DartRuntimePrimitives.Assert(() => (this.restorationId is not null));
        if ((restorePending || (this._bucket is null)))
        {
            global::Doroti.Generated.Framework.Services.RestorationBucket newBucket__43086 = ((global::Doroti.Generated.Framework.Services.RestorationBucket)(object?)parent.claimChild(this.restorationId!, debugOwner: this));
            bool didReplace__43168 = _setNewBucketIfNecessary(newBucket: newBucket__43086, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (object.Equals(this._bucket, newBucket__43086)));
            return didReplace__43168;
        }
        DartRuntimePrimitives.Assert(() => (this._bucket is not null));
        DartRuntimePrimitives.Assert(() => !restorePending);
        this._bucket!.rename(this.restorationId!);
        parent.adoptChild(this._bucket!);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _setNewBucketIfNecessary(global::Doroti.Generated.Framework.Services.RestorationBucket? newBucket, bool restorePending)
    {
        if ((object.Equals(newBucket, this._bucket)))
        {
            return false;
        }
        global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket__43946 = this._bucket;
        this._bucket = newBucket;
        if (!restorePending)
        {
            if ((this._bucket is not null))
            {
                this._properties.Keys.forEach((__arg0) => ((global::System.Action<dynamic>)this._updateProperty)(__arg0));
            }
            didToggleBucket(oldBucket__43946);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _updateProperty(dynamic property)
    {
        if (((dynamic)property).enabled)
        {
            this._bucket?.write(((dynamic)property)._restorationId!, property.toPrimitives());
        }
        else
        {
            this._bucket?.remove<object>(((dynamic)property)._restorationId!);
        }
    }

    public virtual void _unregister(dynamic property)
    {
        global::System.Action listener__44576 = this._properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        property.removeListener((global::System.Action)(() => listener__44576()));
        property._unregister();
    }

}

public enum AutovalidateMode
{
    disabled,
    always,
    onUserInteraction,
    onUnfocus,
    onUserInteractionIfError
}
