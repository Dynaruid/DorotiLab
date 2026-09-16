// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/restoration.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public class RestorationScope : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual string? restorationId { get; private set; }

    public RestorationScope(global::Doroti.Framework.Foundation.Key? key = null, string? restorationId = default!, Widget child = default!) : base(key: key)
    {
        this.restorationId = restorationId;
        this.child = child;
    }

    public static global::Doroti.Framework.Services.RestorationBucket? maybeOf(BuildContext context)
    {
        return context.dependOnInheritedWidgetOfExactType<UnmanagedRestorationScope>()?.bucket;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Framework.Services.RestorationBucket of(BuildContext context)
    {
        global::Doroti.Framework.Services.RestorationBucket? bucket = ((global::Doroti.Framework.Services.RestorationBucket?)maybeOf(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((bucket is null))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("RestorationScope.of() was called with a context that does not " + "contain a RestorationScope widget. "), new global::Doroti.Framework.Foundation.ErrorDescription("No RestorationScope widget ancestor could be found starting from " + "the context that was passed to RestorationScope.of(). This can " + "happen because you are using a widget that looks for a " + "RestorationScope ancestor, but no such ancestor exists.\n" + "The context used was:\n" + $"  {context}"), new global::Doroti.Framework.Foundation.ErrorHint("State restoration must be enabled for a RestorationScope to exist. " + "This can be done by passing a restorationScopeId to MaterialApp, " + "CupertinoApp, or WidgetsApp at the root of the widget tree or by " + "wrapping the widget tree in a RootRestorationScope.") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return bucket!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _RestorationScopeState__restoration());
}

internal class _RestorationScopeState__restoration : State<RestorationScope>, RestorationMixin<RestorationScope>
{
    public virtual global::Doroti.Framework.Services.RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<global::Doroti.Framework.Widgets.IRestorableProperty, global::System.Action> _properties { get; set; } = new DartMap<global::Doroti.Framework.Widgets.IRestorableProperty, global::System.Action>();
    public virtual List<global::Doroti.Framework.Widgets.IRestorableProperty>? _debugPropertiesWaitingForReregistration { get; set; } = default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual global::Doroti.Framework.Services.RestorationBucket? _currentParent { get; set; } = default;

    public virtual string? restorationId => ((RestorationScope)this.widget).restorationId;
    public virtual void restoreState(global::Doroti.Framework.Services.RestorationBucket? oldBucket, bool initialRestore)
    {
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)new UnmanagedRestorationScope(bucket: this.bucket, child: ((RestorationScope)this.widget).child));
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

    public override void didUpdateWidget(RestorationScope oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        didUpdateRestorationId();
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
    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        bool needsRestore = this.restorePending;
        this._currentParent = RestorationScope.maybeOf(this.context);
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
    }

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

    public override void dispose()
    {
        this._properties.forEach(((global::System.Action<global::Doroti.Framework.Widgets.IRestorableProperty, global::System.Action>)((property, listener) =>
        {
            if (!property._disposed)
            {
                property.removeListener(listener);
            }
        })));
        this._bucket?.dispose();
        this._bucket = null;
        base.dispose();
    }

}

public class UnmanagedRestorationScope : InheritedWidget
{
    public virtual global::Doroti.Framework.Services.RestorationBucket? bucket { get; private set; }

    public UnmanagedRestorationScope(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Services.RestorationBucket? bucket = null, Widget child = default!) : base(key: key, child: child)
    {
        this.bucket = bucket;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (UnmanagedRestorationScope)oldWidget;
        return (!Equals(((UnmanagedRestorationScope)__oldWidget).bucket, this.bucket));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RootRestorationScope : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual string? restorationId { get; private set; }

    public RootRestorationScope(global::Doroti.Framework.Foundation.Key? key = null, string? restorationId = default!, Widget child = default!) : base(key: key)
    {
        this.restorationId = restorationId;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _RootRestorationScopeState__restoration());
}

internal class _RootRestorationScopeState__restoration : State<RootRestorationScope>
{
    internal virtual bool? _okToRenderBlankContainer { get; set; } = default;
    internal virtual bool _rootBucketValid { get; set; } = false;
    internal virtual global::Doroti.Framework.Services.RestorationBucket? _rootBucket { get; set; } = default;
    internal virtual global::Doroti.Framework.Services.RestorationBucket? _ancestorBucket { get; set; } = default;
    internal virtual bool _isLoadingRootBucket { get; set; } = false;

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _ancestorBucket = RestorationScope.maybeOf(this.context);
        _loadRootBucketIfNecessary();
        _okToRenderBlankContainer ??= ((((RootRestorationScope)this.widget).restorationId is not null) && this._needsRootBucketInserted);
    }

    public override void didUpdateWidget(RootRestorationScope oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        _loadRootBucketIfNecessary();
    }

    internal virtual bool _needsRootBucketInserted => DartRuntimePrimitives.ConvertValue<bool>((this._ancestorBucket is null));
    internal virtual bool _isWaitingForRootBucket
    {
        get
        {
            return (((((RootRestorationScope)this.widget).restorationId is not null) && this._needsRootBucketInserted) && !this._rootBucketValid);
        }
    }
    internal virtual void _loadRootBucketIfNecessary()
    {
        if ((this._isWaitingForRootBucket && !this._isLoadingRootBucket))
        {
            _isLoadingRootBucket = true;
            RendererBinding.instance.deferFirstFrame();
            DartRuntimePrimitives.Ignore(ServicesBinding.instance.restorationManager.rootBucket.then((global::System.Action<global::Doroti.Framework.Services.RestorationBucket?>)((bucket) =>
            {
                _isLoadingRootBucket = false;
                if (this.mounted)
                {
                    ServicesBinding.instance.restorationManager.addListener(this._replaceRootBucket);
                    setState(((global::System.Action)(() =>
                    {
                        _rootBucket = bucket;
                        _rootBucketValid = true;
                        _okToRenderBlankContainer = false;
                    })));
                }
                RendererBinding.instance.allowFirstFrame();
            })));
        }
    }

    internal virtual void _replaceRootBucket()
    {
        _rootBucketValid = false;
        _rootBucket = null;
        ServicesBinding.instance.restorationManager.removeListener(this._replaceRootBucket);
        _loadRootBucketIfNecessary();
        DartRuntimePrimitives.Assert(() => !this._isWaitingForRootBucket);
    }

    public override void dispose()
    {
        if (this._rootBucketValid)
        {
            ServicesBinding.instance.restorationManager.removeListener(this._replaceRootBucket);
        }
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        if ((DartRuntimePrimitives.RequireValue(this._okToRenderBlankContainer) && this._isWaitingForRootBucket))
        {
            return ((Widget)SizedBox.CreateShrink());
        }
        return ((Widget)new UnmanagedRestorationScope(bucket: (this._ancestorBucket ?? this._rootBucket), child: new RestorationScope(restorationId: ((RootRestorationScope)this.widget).restorationId, child: ((RootRestorationScope)this.widget).child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

/// <summary>Restoration lifecycle and serialization independent of the property's value type.</summary>
public interface IRestorableProperty : global::Doroti.Framework.Foundation.Listenable
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

public abstract class RestorableProperty<T> : global::Doroti.Framework.Foundation.ChangeNotifier, IRestorableProperty
{
    public virtual bool _disposed { get; set; } = false;
    public virtual string? _restorationId { get; set; } = default;
    public virtual RestorationPropertyOwner? _owner { get; set; }

    protected RestorableProperty()
    {
    }

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
        this._owner?._unregister(this);
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
        DartRuntimePrimitives.Assert(() => (this._restorationId is not null));
        DartRuntimePrimitives.Assert(() => (this._owner is not null));
        _restorationId = null;
        _owner = null;
    }

    public virtual IState state
    {
        get
        {
            DartRuntimePrimitives.Assert(() => this.isRegistered);
            DartRuntimePrimitives.Assert(() => debugAssertNotDisposed(this));
            return ((IState)this._owner!);
        }
    }
    public virtual bool isRegistered
    {
        get
        {
            DartRuntimePrimitives.Assert(() => debugAssertNotDisposed(this));
            return (this._restorationId is not null);
        }
    }
}

public interface RestorationPropertyOwner
{
    public void _unregister(global::Doroti.Framework.Widgets.IRestorableProperty property);
}

public interface RestorationMixin<S> : RestorationPropertyOwner where S : StatefulWidget
{
    global::Doroti.Framework.Services.RestorationBucket? _bucket { get; set; }
    DartMap<global::Doroti.Framework.Widgets.IRestorableProperty, global::System.Action> _properties { get; }
    List<global::Doroti.Framework.Widgets.IRestorableProperty>? _debugPropertiesWaitingForReregistration { get; set; }
    bool _firstRestorePending { get; set; }
    global::Doroti.Framework.Services.RestorationBucket? _currentParent { get; set; }

    public string? restorationId { get; }
    public global::Doroti.Framework.Services.RestorationBucket? bucket { get; }
    public void restoreState(global::Doroti.Framework.Services.RestorationBucket? oldBucket, bool initialRestore);
    public void didToggleBucket(global::Doroti.Framework.Services.RestorationBucket? oldBucket);
    public void didUpdateRestorationId();
    public void didUpdateWidget(S oldWidget);
    public bool restorePending { get; }
    public bool _debugDoingRestore { get; }
    public void didChangeDependencies();
    public void _doRestore(global::Doroti.Framework.Services.RestorationBucket? oldBucket);
    public bool _updateBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? parent, bool restorePending);
    public bool _setNewBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? newBucket, bool restorePending);
    public void _updateProperty(global::Doroti.Framework.Widgets.IRestorableProperty property);
    public new void _unregister(global::Doroti.Framework.Widgets.IRestorableProperty property);
    public void dispose();
}
