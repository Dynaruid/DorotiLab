// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/form.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

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
    public virtual global::System.Action<bool, object?>? onPopInvokedWithResult { get; private set; }
    public virtual global::System.Action? onChanged { get; private set; }
    public virtual AutovalidateMode autovalidateMode { get; private set; } = default!;

    public Form(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!, bool? canPop = null, global::System.Action<bool>? onPopInvoked = null, global::System.Action<bool, object?>? onPopInvokedWithResult = null, global::System.Func<Future<bool>>? onWillPop = null, global::System.Action? onChanged = null, AutovalidateMode? autovalidateMode = null) : base(key: key)
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
        _FormScope__form? scope = ((_FormScope__form?)context.dependOnInheritedWidgetOfExactType<_FormScope__form>());
        return scope?._formState;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static FormState of(BuildContext context)
    {
        FormState? formState = ((FormState?)maybeOf(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((formState is null))
                {
                    throw DartRuntimePrimitives.AsException(FlutterError.Create("Form.of() was called with a context that does not contain a Form widget.\n" + "No Form widget ancestor could be found starting from the context that " + "was passed to Form.of(). This can happen because you are using a widget " + "that looks for a Form ancestor, but no such ancestor exists.\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return formState!;
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

/// <summary>Value-independent operations used by a form to manage its fields.</summary>
public interface IFormFieldState : IState
{
    bool hasError { get; }
    bool hasInteractedByUser { get; }
    string? errorText { get; }
    FocusNode focusNode { get; }
    void save();
    void reset();
    bool validate();
    void clearErrorInternal();
}

public class FormState : State<Form>
{
    internal virtual long _generation { get; set; } = 0L;
    internal virtual bool _hasInteractedByUser { get; set; } = false;
    internal virtual HashSet<IFormFieldState> _fields { get; private set; } = new HashSet<IFormFieldState>();

    public virtual IEnumerable<object> fields => this._fields;
    internal virtual void _fieldDidChange()
    {
        ((Form)this.widget).onChanged?.Invoke();
        _hasInteractedByUser = this._fields.any(field => field.hasInteractedByUser);
        _forceRebuild();
    }

    internal virtual void _forceRebuild()
    {
        setState(((global::System.Action)(() =>
        {
            ++_generation;
        })));
    }

    internal virtual void _register(IFormFieldState field)
    {
        this._fields.Add(field);
    }

    internal virtual void _unregister(IFormFieldState field)
    {
        this._fields.Remove(field);
    }

    public override Widget build(BuildContext context)
    {
        bool hasErrorLocal = this._fields.any(field => field.hasError);
        switch (((Form)this.widget).autovalidateMode)
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
                    if ((this._hasInteractedByUser && hasErrorLocal))
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
        Widget formLocal = default!;
        if (((((Form)this.widget).canPop is not null) || ((((((Form)this.widget).onPopInvokedWithResult ?? (object?)((Form)this.widget).onPopInvoked))) is not null)))
        {
            formLocal = DartRuntimePrimitives.ConvertValue<Widget>(new PopScope<object?>(canPop: (((Form)this.widget).canPop ?? true), onPopInvokedWithResult: (global::System.Action<bool, object?>)((Form)this.widget)._callPopInvoked, child: new _FormScope__form(formState: this, generation: this._generation, child: ((Form)this.widget).child)));
        }
        else
        {
            formLocal = DartRuntimePrimitives.ConvertValue<Widget>(new WillPopScope(onWillPop: (global::System.Func<Future<bool>>?)((Form)this.widget).onWillPop, child: new _FormScope__form(formState: this, generation: this._generation, child: ((Form)this.widget).child)));
        }
        return ((Widget)new Semantics(container: true, explicitChildNodes: true, role: SemanticsRole.form, child: formLocal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void save()
    {
        foreach (IFormFieldState @field in this._fields)
        {
            @field.save();
        }
    }

    public virtual void reset()
    {
        foreach (IFormFieldState @field in this._fields)
        {
            @field.reset();
        }
        _hasInteractedByUser = false;
        _fieldDidChange();
    }

    public virtual void clearError()
    {
        foreach (IFormFieldState @field in this._fields)
        {
            @field.clearErrorInternal();
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
        var invalidFields = new HashSet<object>();
        _hasInteractedByUser = true;
        _forceRebuild();
        _validate(View.of(this.context), invalidFields);
        return ((HashSet<object>)invalidFields);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _validate(DorotiView view, HashSet<object>? invalidFields = null)
    {
        var hasError = false;
        var errorMessage = "";
        var validateOnFocusChange = (Equals(((Form)this.widget).autovalidateMode, AutovalidateMode.onUnfocus));
        foreach (IFormFieldState @field in this._fields)
        {
            bool hasFocusLocal = ((FocusNode)@field.focusNode).hasFocus;
            if (((!validateOnFocusChange || !hasFocusLocal) || ((validateOnFocusChange && hasFocusLocal))))
            {
                bool isFieldValid = ((bool)@field.validate());
                hasError |= !isFieldValid;
                if ((errorMessage.Length == 0))
                {
                    errorMessage = (((string?)@field.errorText) ?? "");
                }
                if (((invalidFields is not null) && !isFieldValid))
                {
                    invalidFields.Add(@field);
                }
            }
        }
        if (((errorMessage.Length != 0) && MediaQuery.supportsAnnounceOf(this.context)))
        {
            global::Doroti.Ui.TextDirection directionality = Directionality.of(this.context);
            if ((Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.iOS)))
            {
                DartAsyncRuntime.unawaited(new Future((async () =>
                {
                    await new Future(FormLibrary._kIOSAnnouncementDelayDuration);
                    try
                    {
                        await SemanticsService.sendAnnouncement(view, errorMessage, directionality, assertiveness: Assertiveness.assertive);
                    }
                    catch (Exception exceptionLocal)
                    {
                        var stackLocal = new System.Diagnostics.StackTrace();
                        FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exceptionLocal, stack: stackLocal, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while sending semantics announcement")));
                    }
                    throw new InvalidOperationException("Dart closure completed without a value.");
                })));
            }
            else
            {
                DartRuntimePrimitives.Ignore(SemanticsService.sendAnnouncement(view, errorMessage, directionality, assertiveness: Assertiveness.assertive).catchError(((global::System.Action<object, global::System.Diagnostics.StackTrace?>)((exception, stack) =>
                {
                    FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exception, stack: stack, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while sending semantics announcement")));
                }))));
            }
        }
        return !hasError;
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

    public FormField(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<FormFieldState<T>, Widget> builder = default!, global::System.Action<T?>? onSaved = null, global::System.Action? onReset = null, string? forceErrorText = null, global::System.Func<T?, string?>? validator = null, global::System.Func<BuildContext, string, Widget>? errorBuilder = null, T? initialValue = default, bool enabled = true, AutovalidateMode? autovalidateMode = null, string? restorationId = null) : base(key: key)
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

public class FormFieldState<T> : State<FormField<T>>, RestorationMixin<FormField<T>>, IFormFieldState
{
    FocusNode IFormFieldState.focusNode => this._focusNode;
    void IFormFieldState.clearErrorInternal() => _clearErrorInternal();
    private bool __late__value_initialized;
    private T? __late__value = default!;
    internal virtual T? _value
    {
        get
        {
            if (!__late__value_initialized)
            {
                __late__value = ((FormField<T>)this.widget).initialValue;
                __late__value_initialized = true;
            }
            return __late__value;
        }
        set { __late__value = value; __late__value_initialized = true; }
    }
    internal virtual RestorableStringN _errorText { get; private set; } = default!;
    internal virtual RestorableBool _hasInteractedByUser { get; private set; } = new RestorableBool(false);
    internal virtual FocusNode _focusNode { get; private set; } = new FocusNode();
    public virtual global::Doroti.Framework.Services.RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<global::Doroti.Framework.Widgets.IRestorableProperty, global::System.Action> _properties { get; set; } = new DartMap<global::Doroti.Framework.Widgets.IRestorableProperty, global::System.Action>();
    public virtual List<global::Doroti.Framework.Widgets.IRestorableProperty>? _debugPropertiesWaitingForReregistration { get; set; } = default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual global::Doroti.Framework.Services.RestorationBucket? _currentParent { get; set; } = default;

    public virtual T? value => this._value;
    public virtual string? errorText => this._errorText.value;
    public virtual bool hasError => DartRuntimePrimitives.ConvertValue<bool>((this._errorText.value is not null));
    public virtual bool hasInteractedByUser => this._hasInteractedByUser.value;
    public virtual bool isValid => DartRuntimePrimitives.ConvertValue<bool>(((((FormField<T>)this.widget).forceErrorText is null) && (((FormField<T>)this.widget).validator?.Invoke(this._value) is null)));
    public virtual void save()
    {
        ((FormField<T>)this.widget).onSaved?.Invoke(this.value);
    }

    public virtual void reset()
    {
        setState(((global::System.Action)(() =>
        {
            _value = ((FormField<T>)this.widget).initialValue;
            _clearErrorInternal();
        })));
        ((FormField<T>)this.widget).onReset?.Invoke();
        Form.maybeOf(this.context)?._fieldDidChange();
    }

    public virtual void clearError()
    {
        setState(((global::System.Action)(() =>
        {
            _clearErrorInternal();
        })));
        Form.maybeOf(this.context)?._fieldDidChange();
    }

    public virtual bool validate()
    {
        setState(((global::System.Action)(() =>
        {
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
        if ((((FormField<T>)this.widget).forceErrorText is not null))
        {
            this._errorText.value = ((FormField<T>)this.widget).forceErrorText;
            return;
        }
        if ((((FormField<T>)this.widget).validator is not null))
        {
            this._errorText.value = ((FormField<T>)this.widget).validator!(this._value);
        }
        else
        {
            this._errorText.value = null;
        }
    }

    public virtual void didChange(T? value)
    {
        setState(((global::System.Action)(() =>
        {
            _value = value;
            this._hasInteractedByUser.value = true;
        })));
        Form.maybeOf(this.context)?._fieldDidChange();
    }

    public virtual void setValue(T? value)
    {
        _value = value;
    }

    public virtual string? restorationId => ((FormField<T>)this.widget).restorationId;
    public virtual void restoreState(global::Doroti.Framework.Services.RestorationBucket? oldBucket, bool initialRestore)
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
        _errorText = new RestorableStringN(((FormField<T>)this.widget).forceErrorText);
    }

    public override void didUpdateWidget(FormField<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        didUpdateRestorationId();
        if ((((FormField<T>)this.widget).forceErrorText != ((FormField<T>)oldWidget).forceErrorText))
        {
            this._errorText.value = ((FormField<T>)this.widget).forceErrorText;
        }
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        bool needsRestore = this.restorePending;
        _currentParent = RestorationScope.maybeOf(this.context);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: this._currentParent, restorePending: needsRestore);
        if (needsRestore)
        {
            _doRestore(oldBucket);
        }
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => (!Equals(oldBucket, this._bucket)));
            oldBucket?.dispose();
        }
        switch (Form.maybeOf(this.context)?.widget.autovalidateMode)
        {
            case AutovalidateMode.always:
                {
                    WidgetsBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) =>
                    {
                        if (((((FormField<T>)this.widget).enabled && !this.hasError) && !this.isValid))
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
        this._properties.forEach(((global::System.Action<global::Doroti.Framework.Widgets.IRestorableProperty, global::System.Action>)((property, listener) =>
        {
            if (!property._disposed)
            {
                property.removeListener(listener);
            }
        })));
        this._bucket?.dispose();
        _bucket = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        if (((FormField<T>)this.widget).enabled)
        {
            switch (((FormField<T>)this.widget).autovalidateMode)
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
        Widget childLocal = ((Widget)new Semantics(validationResult: (this.hasError ? SemanticsValidationResult.invalid : SemanticsValidationResult.valid), child: this.widget.builder(this)));
        if ((((Equals(Form.maybeOf(context)?.widget.autovalidateMode, AutovalidateMode.onUnfocus)) && (!Equals(((FormField<T>)this.widget).autovalidateMode, AutovalidateMode.always))) || (Equals(((FormField<T>)this.widget).autovalidateMode, AutovalidateMode.onUnfocus))))
        {
            return ((Widget)new Focus(canRequestFocus: false, skipTraversal: true, onFocusChange: ((global::System.Action<bool>)((value) =>
            {
                if (!DartRuntimePrimitives.RequireValue(value))
                {
                    setState(((global::System.Action)(() =>
                    {
                        _validate();
                    })));
                }
            })), focusNode: this._focusNode, child: childLocal));
        }
        return childLocal;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Services.RestorationBucket? bucket => this._bucket;
    public virtual void didToggleBucket(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => (this._bucket?.isReplacing != true));
    }

    public virtual void registerForRestoration(global::Doroti.Framework.Widgets.IRestorableProperty property, string restorationId)
    {
        DartRuntimePrimitives.Assert(() => ((property._restorationId is null) || ((this._debugDoingRestore && (property._restorationId == restorationId)))), () => (object?)$"Property is already registered under {property._restorationId}.");
        DartRuntimePrimitives.Assert(() => (this._debugDoingRestore || !this._properties.Keys.map<global::Doroti.Framework.Widgets.IRestorableProperty, string?>(((r) => r._restorationId)).contains(restorationId)), () => (object?)$"\"{restorationId}\" is already registered to another property.");
        bool hasSerializedValue = (this.bucket?.contains(restorationId) ?? false);
        object? initialValue = (hasSerializedValue ? property.fromPrimitivesObject(this.bucket!.read<object>(restorationId)) : property.createDefaultValueObject());
        if (!property.isRegistered)
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
            property.addListener((global::System.Action)listener);
            this._properties[property] = (global::System.Action)listener;
        }
        DartRuntimePrimitives.Assert(() => (((property._restorationId == restorationId) && (Equals(property._owner, this))) && this._properties.ContainsKey(property)));
        property.initWithValueObject(initialValue);
        if (((!hasSerializedValue && property.enabled) && (this.bucket is not null)))
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

    public virtual void unregisterFromRestoration(global::Doroti.Framework.Widgets.IRestorableProperty property)
    {
        DartRuntimePrimitives.Assert(() => (Equals(property._owner, this)));
        this._bucket?.remove<object?>(property._restorationId!);
        _unregister(property);
    }

    public virtual void didUpdateRestorationId()
    {
        if ((((this._currentParent is null) || (this._bucket?.restorationId == this.restorationId)) || this.restorePending))
        {
            return;
        }
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        DartRuntimePrimitives.Assert(() => !this.restorePending);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: this._currentParent, restorePending: false);
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => (!Equals(oldBucket, this._bucket)));
            DartRuntimePrimitives.Assert(() => ((this._bucket is null) || (oldBucket is null)));
            oldBucket?.dispose();
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
            global::Doroti.Framework.Services.RestorationBucket? potentialNewParent = ((global::Doroti.Framework.Services.RestorationBucket?)RestorationScope.maybeOf(this.context));
            return ((!Equals(potentialNewParent, this._currentParent)) && ((potentialNewParent?.isReplacing ?? false)));
        }
    }
    public virtual bool _debugDoingRestore => DartRuntimePrimitives.ConvertValue<bool>((this._debugPropertiesWaitingForReregistration is not null));
    public virtual void _doRestore(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
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
                if (Enumerable.Any(this._debugPropertiesWaitingForReregistration!))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Previously registered RestorableProperties must be re-registered in \"restoreState\"."), new global::Doroti.Framework.Foundation.ErrorDescription($"The RestorableProperties with the following IDs were not re-registered to {this} when " + "\"restoreState\" was called:") }));
                }
                this._debugPropertiesWaitingForReregistration = null;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public virtual bool _updateBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? parent, bool restorePending)
    {
        if (((this.restorationId is null) || (parent is null)))
        {
            bool didReplace = _setNewBucketIfNecessary(newBucket: ((global::Doroti.Framework.Services.RestorationBucket?)null), restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (this._bucket is null));
            return didReplace;
        }
        DartRuntimePrimitives.Assert(() => (this.restorationId is not null));
        if ((restorePending || (this._bucket is null)))
        {
            global::Doroti.Framework.Services.RestorationBucket newBucketLocal = ((global::Doroti.Framework.Services.RestorationBucket)parent.claimChild(this.restorationId!, debugOwner: this));
            bool didReplaceLocal = _setNewBucketIfNecessary(newBucket: newBucketLocal, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (Equals(this._bucket, newBucketLocal)));
            return didReplaceLocal;
        }
        DartRuntimePrimitives.Assert(() => (this._bucket is not null));
        DartRuntimePrimitives.Assert(() => !restorePending);
        this._bucket!.rename(this.restorationId!);
        parent.adoptChild(this._bucket!);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _setNewBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? newBucket, bool restorePending)
    {
        if ((Equals(newBucket, this._bucket)))
        {
            return false;
        }
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        this._bucket = newBucket;
        if (!restorePending)
        {
            if ((this._bucket is not null))
            {
                this._properties.Keys.forEach((__arg0) => ((global::System.Action<global::Doroti.Framework.Widgets.IRestorableProperty>)this._updateProperty)(__arg0));
            }
            didToggleBucket(oldBucket);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _updateProperty(global::Doroti.Framework.Widgets.IRestorableProperty property)
    {
        if (property.enabled)
        {
            this._bucket?.write(property._restorationId!, property.toPrimitives());
        }
        else
        {
            this._bucket?.remove<object>(property._restorationId!);
        }
    }

    public virtual void _unregister(global::Doroti.Framework.Widgets.IRestorableProperty property)
    {
        global::System.Action listener = this._properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        property.removeListener(listener);
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
