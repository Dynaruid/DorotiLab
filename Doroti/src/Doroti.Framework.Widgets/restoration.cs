// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/restoration.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public class RestorationScope : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual string? restorationId { get; private set; }

    public RestorationScope(
        Key? key = null,
        string? restorationId = default!,
        Widget child = default!
    )
        : base(key: key)
    {
        this.restorationId = restorationId;
        this.child = child;
    }

    public static RestorationBucket? maybeOf(BuildContext context)
    {
        return context.dependOnInheritedWidgetOfExactType<UnmanagedRestorationScope>()?.bucket;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static RestorationBucket of(BuildContext context)
    {
        RestorationBucket? bucket = maybeOf(context);
        DartRuntimePrimitives.Assert(() =>
        {
            if (bucket is null)
            {
                throw DartRuntimePrimitives.AsException(
                    new FlutterError(
                        new List<DiagnosticsNode>
                        {
                            new ErrorSummary(
                                "RestorationScope.of() was called with a context that does not "
                                    + "contain a RestorationScope widget. "
                            ),
                            new ErrorDescription(
                                "No RestorationScope widget ancestor could be found starting from "
                                    + "the context that was passed to RestorationScope.of(). This can "
                                    + "happen because you are using a widget that looks for a "
                                    + "RestorationScope ancestor, but no such ancestor exists.\n"
                                    + "The context used was:\n"
                                    + $"  {context}"
                            ),
                            new ErrorHint(
                                "State restoration must be enabled for a RestorationScope to exist. "
                                    + "This can be done by passing a restorationScopeId to MaterialApp, "
                                    + "CupertinoApp, or WidgetsApp at the root of the widget tree or by "
                                    + "wrapping the widget tree in a RootRestorationScope."
                            ),
                        }
                    )
                );
            }
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        return bucket!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _RestorationScopeState__restoration());
}

internal class _RestorationScopeState__restoration
    : State<RestorationScope>,
        RestorationMixin<RestorationScope>
{
    public virtual RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<IRestorableProperty, Action> _properties { get; set; } =
        new DartMap<IRestorableProperty, Action>();
    public virtual List<IRestorableProperty>? _debugPropertiesWaitingForReregistration { get; set; } =
        default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual RestorationBucket? _currentParent { get; set; } = default;

    public virtual string? restorationId => widget.restorationId;

    public virtual void restoreState(RestorationBucket? oldBucket, bool initialRestore) { }

    public override Widget build(BuildContext context)
    {
        return new UnmanagedRestorationScope(bucket: bucket, child: widget.child);
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

    public override void didUpdateWidget(RestorationScope oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        didUpdateRestorationId();
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
    }

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

    public override void dispose()
    {
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
}

public class UnmanagedRestorationScope : InheritedWidget
{
    public virtual RestorationBucket? bucket { get; private set; }

    public UnmanagedRestorationScope(
        Key? key = null,
        RestorationBucket? bucket = null,
        Widget child = default!
    )
        : base(key: key, child: child)
    {
        this.bucket = bucket;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (UnmanagedRestorationScope)oldWidget;
        return !Equals(__oldWidget.bucket, bucket);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class RootRestorationScope : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual string? restorationId { get; private set; }

    public RootRestorationScope(
        Key? key = null,
        string? restorationId = default!,
        Widget child = default!
    )
        : base(key: key)
    {
        this.restorationId = restorationId;
        this.child = child;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _RootRestorationScopeState__restoration());
}

internal class _RootRestorationScopeState__restoration : State<RootRestorationScope>
{
    internal virtual bool? _okToRenderBlankContainer { get; set; } = default;
    internal virtual bool _rootBucketValid { get; set; } = false;
    internal virtual RestorationBucket? _rootBucket { get; set; } = default;
    internal virtual RestorationBucket? _ancestorBucket { get; set; } = default;
    internal virtual bool _isLoadingRootBucket { get; set; } = false;

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _ancestorBucket = RestorationScope.maybeOf(context);
        _loadRootBucketIfNecessary();
        _okToRenderBlankContainer ??= (
            (widget.restorationId is not null) && _needsRootBucketInserted
        );
    }

    public override void didUpdateWidget(RootRestorationScope oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        _loadRootBucketIfNecessary();
    }

    internal virtual bool _needsRootBucketInserted =>
        DartRuntimePrimitives.ConvertValue<bool>(_ancestorBucket is null);
    internal virtual bool _isWaitingForRootBucket
    {
        get
        {
            return (widget.restorationId is not null)
                && _needsRootBucketInserted
                && !_rootBucketValid;
        }
    }

    internal virtual void _loadRootBucketIfNecessary()
    {
        if (_isWaitingForRootBucket && !_isLoadingRootBucket)
        {
            _isLoadingRootBucket = true;
            RendererBinding.instance.deferFirstFrame();
            DartRuntimePrimitives.Ignore(
                ServicesBinding.instance.restorationManager.rootBucket.then(
                    (bucket) =>
                    {
                        _isLoadingRootBucket = false;
                        if (mounted)
                        {
                            ServicesBinding.instance.restorationManager.addListener(
                                _replaceRootBucket
                            );
                            setState(() =>
                            {
                                _rootBucket = bucket;
                                _rootBucketValid = true;
                                _okToRenderBlankContainer = false;
                            });
                        }
                        RendererBinding.instance.allowFirstFrame();
                    }
                )
            );
        }
    }

    internal virtual void _replaceRootBucket()
    {
        _rootBucketValid = false;
        _rootBucket = null;
        ServicesBinding.instance.restorationManager.removeListener(_replaceRootBucket);
        _loadRootBucketIfNecessary();
        DartRuntimePrimitives.Assert(() => !_isWaitingForRootBucket);
    }

    public override void dispose()
    {
        if (_rootBucketValid)
        {
            ServicesBinding.instance.restorationManager.removeListener(_replaceRootBucket);
        }
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        if (
            DartRuntimePrimitives.RequireValue(_okToRenderBlankContainer) && _isWaitingForRootBucket
        )
        {
            return SizedBox.CreateShrink();
        }
        return new UnmanagedRestorationScope(
            bucket: _ancestorBucket ?? _rootBucket,
            child: new RestorationScope(restorationId: widget.restorationId, child: widget.child)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

/// <summary>Restoration lifecycle and serialization independent of the property's value type.</summary>
public interface IRestorableProperty : Listenable
{
    bool _disposed { get; }
    string? _restorationId { get; }
    RestorationPropertyOwner? _owner { get; }
    bool enabled { get; }
    bool isRegistered { get; }
    object? createDefaultValueObject();
    object? fromPrimitivesObject(object? data);
    void initWithValueObject(object? value);
    object? toPrimitives();
    void _register(string restorationId, RestorationPropertyOwner owner);
    void _unregister();
}

public abstract class RestorableProperty<T> : ChangeNotifier, IRestorableProperty
{
    public virtual bool _disposed { get; set; } = false;
    public virtual string? _restorationId { get; set; } = default;
    public virtual RestorationPropertyOwner? _owner { get; set; }

    protected RestorableProperty() { }

    public abstract T createDefaultValue();
    public abstract T fromPrimitives(object? data);
    public abstract void initWithValue(T value);
    public abstract object? toPrimitives();

    object? IRestorableProperty.createDefaultValueObject() => createDefaultValue();

    object? IRestorableProperty.fromPrimitivesObject(object? data) => fromPrimitives(data);

    void IRestorableProperty.initWithValueObject(object? value) => initWithValue((T)value!);

    public virtual bool enabled => true;

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() => debugAssertNotDisposed(this));
        _owner?._unregister(this);
        base.dispose();
        _disposed = true;
    }

    public virtual void _register(string restorationId, RestorationPropertyOwner owner)
    {
        DartRuntimePrimitives.Assert(() => debugAssertNotDisposed(this));
        _restorationId = restorationId;
        _owner = owner;
    }

    public virtual void _unregister()
    {
        DartRuntimePrimitives.Assert(() => debugAssertNotDisposed(this));
        DartRuntimePrimitives.Assert(() => _restorationId is not null);
        DartRuntimePrimitives.Assert(() => _owner is not null);
        _restorationId = null;
        _owner = null;
    }

    public virtual IState state
    {
        get
        {
            DartRuntimePrimitives.Assert(() => isRegistered);
            DartRuntimePrimitives.Assert(() => debugAssertNotDisposed(this));
            return (IState)_owner!;
        }
    }
    public virtual bool isRegistered
    {
        get
        {
            DartRuntimePrimitives.Assert(() => debugAssertNotDisposed(this));
            return _restorationId is not null;
        }
    }
}

public interface RestorationPropertyOwner
{
    public void _unregister(IRestorableProperty property);
}

public interface RestorationMixin<S> : RestorationPropertyOwner
    where S : StatefulWidget
{
    RestorationBucket? _bucket { get; set; }
    DartMap<IRestorableProperty, Action> _properties { get; }
    List<IRestorableProperty>? _debugPropertiesWaitingForReregistration { get; set; }
    bool _firstRestorePending { get; set; }
    RestorationBucket? _currentParent { get; set; }

    public string? restorationId { get; }
    public RestorationBucket? bucket { get; }
    public void restoreState(RestorationBucket? oldBucket, bool initialRestore);
    public void didToggleBucket(RestorationBucket? oldBucket);
    public void didUpdateRestorationId();
    public void didUpdateWidget(S oldWidget);
    public bool restorePending { get; }
    public bool _debugDoingRestore { get; }
    public void didChangeDependencies();
    public void _doRestore(RestorationBucket? oldBucket);
    public bool _updateBucketIfNecessary(RestorationBucket? parent, bool restorePending);
    public bool _setNewBucketIfNecessary(RestorationBucket? newBucket, bool restorePending);
    public void _updateProperty(IRestorableProperty property);
    public new void _unregister(IRestorableProperty property);
    public void dispose();
}
