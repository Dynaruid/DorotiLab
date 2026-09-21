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
    public virtual Func<Future<bool>>? onWillPop { get; private set; }
    public virtual bool? canPop { get; private set; }
    public virtual Action<bool>? onPopInvoked { get; private set; }
    public virtual Action<bool, object?>? onPopInvokedWithResult { get; private set; }
    public virtual Action? onChanged { get; private set; }
    public virtual AutovalidateMode autovalidateMode { get; private set; } = default!;

    public Form(
        Key? key = null,
        Widget child = default!,
        bool? canPop = null,
        Action<bool>? onPopInvoked = null,
        Action<bool, object?>? onPopInvokedWithResult = null,
        Func<Future<bool>>? onWillPop = null,
        Action? onChanged = null,
        AutovalidateMode? autovalidateMode = null
    )
        : base(key: key)
    {
        this.child = child;
        this.canPop = canPop;
        this.onPopInvoked = onPopInvoked;
        this.onPopInvokedWithResult = onPopInvokedWithResult;
        this.onWillPop = onWillPop;
        this.onChanged = onChanged;
        this.autovalidateMode = autovalidateMode ?? AutovalidateMode.disabled;
        System.Diagnostics.Debug.Assert((onPopInvokedWithResult is null) || (onPopInvoked is null));
        System.Diagnostics.Debug.Assert(
            ((onPopInvokedWithResult ?? (object?)onPopInvoked) ?? canPop) is null
                || (onWillPop is null)
        );
    }

    public static FormState? maybeOf(BuildContext context)
    {
        _FormScope__form? scope = context.dependOnInheritedWidgetOfExactType<_FormScope__form>();
        return scope?._formState;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static FormState of(BuildContext context)
    {
        FormState? formState = maybeOf(context);
        DartRuntimePrimitives.Assert(() =>
        {
            if (formState is null)
            {
                throw DartRuntimePrimitives.AsException(
                    FlutterError.Create(
                        "Form.of() was called with a context that does not contain a Form widget.\n"
                            + "No Form widget ancestor could be found starting from the context that "
                            + "was passed to Form.of(). This can happen because you are using a widget "
                            + "that looks for a Form ancestor, but no such ancestor exists.\n"
                            + "The context used was:\n"
                            + $"  {context}"
                    )
                );
            }
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        return formState!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _callPopInvoked(bool didPop, object? result)
    {
        if (onPopInvokedWithResult is not null)
        {
            onPopInvokedWithResult!(didPop, result);
            return;
        }
        onPopInvoked?.Invoke(didPop);
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new FormState());
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
    internal virtual HashSet<IFormFieldState> _fields { get; private set; } =
        new HashSet<IFormFieldState>();

    public virtual IEnumerable<object> fields => _fields;

    internal virtual void _fieldDidChange()
    {
        widget.onChanged?.Invoke();
        _hasInteractedByUser = _fields.any(field => field.hasInteractedByUser);
        _forceRebuild();
    }

    internal virtual void _forceRebuild()
    {
        setState(() =>
        {
            ++_generation;
        });
    }

    internal virtual void _register(IFormFieldState field)
    {
        _fields.Add(field);
    }

    internal virtual void _unregister(IFormFieldState field)
    {
        _fields.Remove(field);
    }

    public override Widget build(BuildContext context)
    {
        bool hasErrorLocal = _fields.any(field => field.hasError);
        switch (widget.autovalidateMode)
        {
            case AutovalidateMode.always:
            {
                _validate(View.of(context));
                break;
            }
            case AutovalidateMode.onUserInteraction:
            {
                if (_hasInteractedByUser)
                {
                    _validate(View.of(context));
                }
                break;
            }
            case AutovalidateMode.onUserInteractionIfError:
            {
                if (_hasInteractedByUser && hasErrorLocal)
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
        if (
            (widget.canPop is not null)
            || ((widget.onPopInvokedWithResult ?? (object?)widget.onPopInvoked) is not null)
        )
        {
            formLocal = DartRuntimePrimitives.ConvertValue<Widget>(
                new PopScope<object?>(
                    canPop: widget.canPop ?? true,
                    onPopInvokedWithResult: widget._callPopInvoked,
                    child: new _FormScope__form(
                        formState: this,
                        generation: _generation,
                        child: widget.child
                    )
                )
            );
        }
        else
        {
            formLocal = DartRuntimePrimitives.ConvertValue<Widget>(
                new WillPopScope(
                    onWillPop: widget.onWillPop,
                    child: new _FormScope__form(
                        formState: this,
                        generation: _generation,
                        child: widget.child
                    )
                )
            );
        }
        return new Semantics(
            container: true,
            explicitChildNodes: true,
            role: SemanticsRole.form,
            child: formLocal
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void save()
    {
        foreach (IFormFieldState @field in _fields)
        {
            @field.save();
        }
    }

    public virtual void reset()
    {
        foreach (IFormFieldState @field in _fields)
        {
            @field.reset();
        }
        _hasInteractedByUser = false;
        _fieldDidChange();
    }

    public virtual void clearError()
    {
        foreach (IFormFieldState @field in _fields)
        {
            @field.clearErrorInternal();
        }
        _fieldDidChange();
    }

    public virtual bool validate()
    {
        _hasInteractedByUser = true;
        _forceRebuild();
        return _validate(View.of(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual HashSet<object> validateGranularly()
    {
        var invalidFields = new HashSet<object>();
        _hasInteractedByUser = true;
        _forceRebuild();
        _validate(View.of(context), invalidFields);
        return invalidFields;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _validate(DorotiView view, HashSet<object>? invalidFields = null)
    {
        var hasError = false;
        var errorMessage = "";
        var validateOnFocusChange = Equals(widget.autovalidateMode, AutovalidateMode.onUnfocus);
        foreach (IFormFieldState @field in _fields)
        {
            bool hasFocusLocal = @field.focusNode.hasFocus;
            if (
                !validateOnFocusChange
                || !hasFocusLocal
                || (validateOnFocusChange && hasFocusLocal)
            )
            {
                bool isFieldValid = @field.validate();
                hasError |= !isFieldValid;
                if (errorMessage.Length == 0)
                {
                    errorMessage = @field.errorText ?? "";
                }
                if ((invalidFields is not null) && !isFieldValid)
                {
                    invalidFields.Add(@field);
                }
            }
        }
        if ((errorMessage.Length != 0) && MediaQuery.supportsAnnounceOf(context))
        {
            TextDirection directionality = Directionality.of(context);
            if (Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.iOS))
            {
                DartAsyncRuntime.unawaited(
                    new Future(async () =>
                    {
                        await new Future(FormLibrary._kIOSAnnouncementDelayDuration);
                        try
                        {
                            await SemanticsService.sendAnnouncement(
                                view,
                                errorMessage,
                                directionality,
                                assertiveness: Assertiveness.assertive
                            );
                        }
                        catch (Exception exceptionLocal)
                        {
                            var stackLocal = new System.Diagnostics.StackTrace();
                            FlutterError.reportError(
                                new FlutterErrorDetails(
                                    exception: exceptionLocal,
                                    stack: stackLocal,
                                    library: "widgets library",
                                    context: new ErrorDescription(
                                        "while sending semantics announcement"
                                    )
                                )
                            );
                        }
                        throw new InvalidOperationException(
                            "Dart closure completed without a value."
                        );
                    })
                );
            }
            else
            {
                DartRuntimePrimitives.Ignore(
                    SemanticsService
                        .sendAnnouncement(
                            view,
                            errorMessage,
                            directionality,
                            assertiveness: Assertiveness.assertive
                        )
                        .catchError(
                            (exception, stack) =>
                            {
                                FlutterError.reportError(
                                    new FlutterErrorDetails(
                                        exception: exception,
                                        stack: stack,
                                        library: "widgets library",
                                        context: new ErrorDescription(
                                            "while sending semantics announcement"
                                        )
                                    )
                                );
                            }
                        )
                );
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

    internal _FormScope__form(Widget child, FormState formState, long generation)
        : base(child: child)
    {
        _formState = formState;
        _generation = generation;
    }

    public virtual Form form => _formState.widget;

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        _generation != ((_FormScope__form)oldWidget)._generation;
}

public delegate string? FormFieldValidator<T>(T? value);

public delegate Widget FormFieldErrorBuilder(BuildContext context, string errorText);

public delegate void FormFieldSetter<T>(T? newValue);

public delegate Widget FormFieldBuilder<T>(FormFieldState<T> field);

public class FormField<T> : StatefulWidget
{
    public virtual Func<FormFieldState<T>, Widget> builder { get; private set; } = default!;
    public virtual Action<T?>? onSaved { get; private set; }
    public virtual Action? onReset { get; private set; }
    public virtual string? forceErrorText { get; private set; }
    public virtual Func<T?, string?>? validator { get; private set; }
    public virtual Func<BuildContext, string, Widget>? errorBuilder { get; private set; }
    public virtual T? initialValue { get; private set; }
    public virtual bool enabled { get; private set; } = default!;
    public virtual AutovalidateMode autovalidateMode { get; private set; } = default!;
    public virtual string? restorationId { get; private set; }

    public FormField(
        Key? key = null,
        Func<FormFieldState<T>, Widget> builder = default!,
        Action<T?>? onSaved = null,
        Action? onReset = null,
        string? forceErrorText = null,
        Func<T?, string?>? validator = null,
        Func<BuildContext, string, Widget>? errorBuilder = null,
        T? initialValue = default,
        bool enabled = true,
        AutovalidateMode? autovalidateMode = null,
        string? restorationId = null
    )
        : base(key: key)
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
        this.autovalidateMode = autovalidateMode ?? AutovalidateMode.disabled;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new FormFieldState<T>());
}

public class FormFieldState<T>
    : State<FormField<T>>,
        RestorationMixin<FormField<T>>,
        IFormFieldState
{
    FocusNode IFormFieldState.focusNode => _focusNode;

    void IFormFieldState.clearErrorInternal() => _clearErrorInternal();

    private bool __late__value_initialized;
    private T? __late__value = default!;
    internal virtual T? _value
    {
        get
        {
            if (!__late__value_initialized)
            {
                __late__value = widget.initialValue;
                __late__value_initialized = true;
            }
            return __late__value;
        }
        set
        {
            __late__value = value;
            __late__value_initialized = true;
        }
    }
    internal virtual RestorableStringN _errorText { get; private set; } = default!;
    internal virtual RestorableBool _hasInteractedByUser { get; private set; } =
        new RestorableBool(false);
    internal virtual FocusNode _focusNode { get; private set; } = new FocusNode();
    public virtual RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<IRestorableProperty, Action> _properties { get; set; } =
        new DartMap<IRestorableProperty, Action>();
    public virtual List<IRestorableProperty>? _debugPropertiesWaitingForReregistration { get; set; } =
        default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual RestorationBucket? _currentParent { get; set; } = default;

    public virtual T? value => _value;
    public virtual string? errorText => _errorText.value;
    public virtual bool hasError =>
        DartRuntimePrimitives.ConvertValue<bool>(_errorText.value is not null);
    public virtual bool hasInteractedByUser => _hasInteractedByUser.value;
    public virtual bool isValid =>
        DartRuntimePrimitives.ConvertValue<bool>(
            (widget.forceErrorText is null) && (widget.validator?.Invoke(_value) is null)
        );

    public virtual void save()
    {
        widget.onSaved?.Invoke(value);
    }

    public virtual void reset()
    {
        setState(() =>
        {
            _value = widget.initialValue;
            _clearErrorInternal();
        });
        widget.onReset?.Invoke();
        Form.maybeOf(context)?._fieldDidChange();
    }

    public virtual void clearError()
    {
        setState(() =>
        {
            _clearErrorInternal();
        });
        Form.maybeOf(context)?._fieldDidChange();
    }

    public virtual bool validate()
    {
        setState(() =>
        {
            _validate();
        });
        return !hasError;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _clearErrorInternal()
    {
        _errorText.value = null;
        _hasInteractedByUser.value = false;
    }

    internal virtual void _validate()
    {
        if (widget.forceErrorText is not null)
        {
            _errorText.value = widget.forceErrorText;
            return;
        }
        if (widget.validator is not null)
        {
            _errorText.value = widget.validator!(_value);
        }
        else
        {
            _errorText.value = null;
        }
    }

    public virtual void didChange(T? value)
    {
        setState(() =>
        {
            _value = value;
            _hasInteractedByUser.value = true;
        });
        Form.maybeOf(context)?._fieldDidChange();
    }

    public virtual void setValue(T? value)
    {
        _value = value;
    }

    public virtual string? restorationId => widget.restorationId;

    public virtual void restoreState(RestorationBucket? oldBucket, bool initialRestore)
    {
        registerForRestoration(_errorText, "error_text");
        registerForRestoration(_hasInteractedByUser, "has_interacted_by_user");
    }

    public override void deactivate()
    {
        Form.maybeOf(context)?._unregister(this);
        base.deactivate();
    }

    public override void initState()
    {
        base.initState();
        _errorText = new RestorableStringN(widget.forceErrorText);
    }

    public override void didUpdateWidget(FormField<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        didUpdateRestorationId();
        if (widget.forceErrorText != oldWidget.forceErrorText)
        {
            _errorText.value = widget.forceErrorText;
        }
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        RestorationBucket? oldBucket = _bucket;
        bool needsRestore = restorePending;
        _currentParent = RestorationScope.maybeOf(context);
        bool didReplaceBucket = _updateBucketIfNecessary(
            parent: _currentParent,
            restorePending: needsRestore
        );
        if (needsRestore)
        {
            _doRestore(oldBucket);
        }
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => !Equals(oldBucket, _bucket));
            oldBucket?.dispose();
        }
        switch (Form.maybeOf(context)?.widget.autovalidateMode)
        {
            case AutovalidateMode.always:
            {
                WidgetsBinding.instance.addPostFrameCallback(
                    (_) =>
                    {
                        if (widget.enabled && !hasError && !isValid)
                        {
                            validate();
                        }
                    }
                );
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
        _errorText.dispose();
        _focusNode.dispose();
        _hasInteractedByUser.dispose();
        _properties.forEach(
            (property, listener) =>
            {
                if (!property._disposed)
                {
                    property.removeListener(listener);
                }
            }
        );
        _bucket?.dispose();
        _bucket = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        if (widget.enabled)
        {
            switch (widget.autovalidateMode)
            {
                case AutovalidateMode.always:
                {
                    _validate();
                    break;
                }
                case AutovalidateMode.onUserInteraction:
                {
                    if (_hasInteractedByUser.value)
                    {
                        _validate();
                    }
                    break;
                }
                case AutovalidateMode.onUserInteractionIfError:
                {
                    if (_hasInteractedByUser.value && hasError)
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
        Widget childLocal = new Semantics(
            validationResult: hasError
                ? SemanticsValidationResult.invalid
                : SemanticsValidationResult.valid,
            child: widget.builder(this)
        );
        if (
            (
                Equals(Form.maybeOf(context)?.widget.autovalidateMode, AutovalidateMode.onUnfocus)
                && (!Equals(widget.autovalidateMode, AutovalidateMode.always))
            ) || Equals(widget.autovalidateMode, AutovalidateMode.onUnfocus)
        )
        {
            return new Focus(
                canRequestFocus: false,
                skipTraversal: true,
                onFocusChange: (value) =>
                {
                    if (!DartRuntimePrimitives.RequireValue(value))
                    {
                        setState(() =>
                        {
                            _validate();
                        });
                    }
                },
                focusNode: _focusNode,
                child: childLocal
            );
        }
        return childLocal;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RestorationBucket? bucket => _bucket;

    public virtual void didToggleBucket(RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => _bucket?.isReplacing != true);
    }

    public virtual void registerForRestoration(IRestorableProperty property, string restorationId)
    {
        DartRuntimePrimitives.Assert(
            () =>
                (property._restorationId is null)
                || (_debugDoingRestore && (property._restorationId == restorationId)),
            () => (object?)$"Property is already registered under {property._restorationId}."
        );
        DartRuntimePrimitives.Assert(
            () =>
                _debugDoingRestore
                || !_properties.Keys.map((r) => r._restorationId).contains(restorationId),
            () => (object?)$"\"{restorationId}\" is already registered to another property."
        );
        bool hasSerializedValue = bucket?.contains(restorationId) ?? false;
        object? initialValue = hasSerializedValue
            ? property.fromPrimitivesObject(bucket!.read<object>(restorationId))
            : property.createDefaultValueObject();
        if (!property.isRegistered)
        {
            property._register(restorationId, this);
            void listener()
            {
                if (bucket is null)
                {
                    return;
                }
                _updateProperty(property);
            }
            property.addListener(listener);
            _properties[property] = listener;
        }
        DartRuntimePrimitives.Assert(() =>
            (property._restorationId == restorationId)
            && Equals(property._owner, this)
            && _properties.ContainsKey(property)
        );
        property.initWithValueObject(initialValue);
        if (!hasSerializedValue && property.enabled && (bucket is not null))
        {
            _updateProperty(property);
        }
        DartRuntimePrimitives.Assert(() =>
        {
            _debugPropertiesWaitingForReregistration?.Remove(property);
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
    }

    public virtual void unregisterFromRestoration(IRestorableProperty property)
    {
        DartRuntimePrimitives.Assert(() => Equals(property._owner, this));
        _bucket?.remove<object?>(property._restorationId!);
        _unregister(property);
    }

    public virtual void didUpdateRestorationId()
    {
        if ((_currentParent is null) || (_bucket?.restorationId == restorationId) || restorePending)
        {
            return;
        }
        RestorationBucket? oldBucket = _bucket;
        DartRuntimePrimitives.Assert(() => !restorePending);
        bool didReplaceBucket = _updateBucketIfNecessary(
            parent: _currentParent,
            restorePending: false
        );
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => !Equals(oldBucket, _bucket));
            DartRuntimePrimitives.Assert(() => (_bucket is null) || (oldBucket is null));
            oldBucket?.dispose();
        }
    }

    public virtual bool restorePending
    {
        get
        {
            if (_firstRestorePending)
            {
                return true;
            }
            if (restorationId is null)
            {
                return false;
            }
            RestorationBucket? potentialNewParent = RestorationScope.maybeOf(context);
            return (!Equals(potentialNewParent, _currentParent))
                && (potentialNewParent?.isReplacing ?? false);
        }
    }
    public virtual bool _debugDoingRestore =>
        DartRuntimePrimitives.ConvertValue<bool>(
            _debugPropertiesWaitingForReregistration is not null
        );

    public virtual void _doRestore(RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            _debugPropertiesWaitingForReregistration = _properties.Keys.ToList();
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        restoreState(oldBucket, _firstRestorePending);
        _firstRestorePending = false;
        DartRuntimePrimitives.Assert(() =>
        {
            if (Enumerable.Any(_debugPropertiesWaitingForReregistration!))
            {
                throw DartRuntimePrimitives.AsException(
                    new FlutterError(
                        new List<DiagnosticsNode>
                        {
                            new ErrorSummary(
                                "Previously registered RestorableProperties must be re-registered in \"restoreState\"."
                            ),
                            new ErrorDescription(
                                $"The RestorableProperties with the following IDs were not re-registered to {this} when "
                                    + "\"restoreState\" was called:"
                            ),
                        }
                    )
                );
            }
            _debugPropertiesWaitingForReregistration = null;
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
    }

    public virtual bool _updateBucketIfNecessary(RestorationBucket? parent, bool restorePending)
    {
        if ((restorationId is null) || (parent is null))
        {
            bool didReplace = _setNewBucketIfNecessary(
                newBucket: null,
                restorePending: restorePending
            );
            DartRuntimePrimitives.Assert(() => _bucket is null);
            return didReplace;
        }
        DartRuntimePrimitives.Assert(() => restorationId is not null);
        if (restorePending || (_bucket is null))
        {
            RestorationBucket newBucketLocal = parent.claimChild(restorationId!, debugOwner: this);
            bool didReplaceLocal = _setNewBucketIfNecessary(
                newBucket: newBucketLocal,
                restorePending: restorePending
            );
            DartRuntimePrimitives.Assert(() => Equals(_bucket, newBucketLocal));
            return didReplaceLocal;
        }
        DartRuntimePrimitives.Assert(() => _bucket is not null);
        DartRuntimePrimitives.Assert(() => !restorePending);
        _bucket!.rename(restorationId!);
        parent.adoptChild(_bucket!);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _setNewBucketIfNecessary(RestorationBucket? newBucket, bool restorePending)
    {
        if (Equals(newBucket, _bucket))
        {
            return false;
        }
        RestorationBucket? oldBucket = _bucket;
        _bucket = newBucket;
        if (!restorePending)
        {
            if (_bucket is not null)
            {
                _properties.Keys.forEach(
                    (__arg0) => ((Action<IRestorableProperty>)_updateProperty)(__arg0)
                );
            }
            didToggleBucket(oldBucket);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _updateProperty(IRestorableProperty property)
    {
        if (property.enabled)
        {
            _bucket?.write(property._restorationId!, property.toPrimitives());
        }
        else
        {
            _bucket?.remove<object>(property._restorationId!);
        }
    }

    public virtual void _unregister(IRestorableProperty property)
    {
        Action listener = _properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
        {
            _debugPropertiesWaitingForReregistration?.Remove(property);
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
    onUserInteractionIfError,
}
