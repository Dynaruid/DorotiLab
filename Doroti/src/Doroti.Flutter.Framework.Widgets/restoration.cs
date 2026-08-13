// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/restoration.dart
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

public class RestorationScope : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual string? restorationId { get; private set; }

    public RestorationScope(global::Doroti.Generated.Framework.Foundation.Key? key = null, string? restorationId = default!, Widget child = default!) : base(key: key)
    {
        this.restorationId = restorationId;
        this.child = child;
    }

    public static global::Doroti.Generated.Framework.Services.RestorationBucket? maybeOf(BuildContext context)
    {
        return context.dependOnInheritedWidgetOfExactType<UnmanagedRestorationScope>()?.bucket;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Generated.Framework.Services.RestorationBucket of(BuildContext context)
    {
        global::Doroti.Generated.Framework.Services.RestorationBucket? bucket__4772 = ((global::Doroti.Generated.Framework.Services.RestorationBucket?)(object?)RestorationScope.maybeOf(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((bucket__4772 is null))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary("RestorationScope.of() was called with a context that does not " + "contain a RestorationScope widget. "), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("No RestorationScope widget ancestor could be found starting from " + "the context that was passed to RestorationScope.of(). This can " + "happen because you are using a widget that looks for a " + "RestorationScope ancestor, but no such ancestor exists.\n" + "The context used was:\n" + $"  {context}"), new global::Doroti.Generated.Framework.Foundation.ErrorHint("State restoration must be enabled for a RestorationScope to exist. " + "This can be done by passing a restorationScopeId to MaterialApp, " + "CupertinoApp, or WidgetsApp at the root of the widget tree or by " + "wrapping the widget tree in a RootRestorationScope.") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return bucket__4772!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _RestorationScopeState__restoration());
}

internal class _RestorationScopeState__restoration : State<RestorationScope>, RestorationMixin<RestorationScope>
{
    public virtual global::Doroti.Generated.Framework.Services.RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<dynamic, global::System.Action> _properties { get; set; } = new DartMap<dynamic, global::System.Action>();
    public virtual List<dynamic>? _debugPropertiesWaitingForReregistration { get; set; } = default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual global::Doroti.Generated.Framework.Services.RestorationBucket? _currentParent { get; set; } = default;

    public virtual string? restorationId => ((RestorationScope)(object)this.widget).restorationId;
    public virtual void restoreState(global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket, bool initialRestore)
    {
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new UnmanagedRestorationScope(bucket: this.bucket, child: ((RestorationScope)(object)this.widget).child));
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
            global::Doroti.Generated.Framework.Services.RestorationBucket? potentialNewParent__40517 = ((global::Doroti.Generated.Framework.Services.RestorationBucket?)(object?)RestorationScope.maybeOf(this.context));
            return ((!object.Equals(potentialNewParent__40517, this._currentParent)) && ((potentialNewParent__40517?.isReplacing ?? false)));
            return default!;
        }
    }
    public virtual bool _debugDoingRestore => DartRuntimePrimitives.ConvertValue<bool>((this._debugPropertiesWaitingForReregistration is not null));
    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket__41020 = this._bucket;
        bool needsRestore__41056 = this.restorePending;
        this._currentParent = RestorationScope.maybeOf(this.context);
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
    }

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

    public override void dispose()
    {
        this._properties.forEach(((global::System.Action<dynamic, global::System.Action>)((property, listener) => {
if (!((dynamic)property)._disposed)
{
    property.removeListener((global::System.Action)(() => listener()));
}
})));
        this._bucket?.dispose();
        this._bucket = null;
        base.dispose();
    }

}

public class UnmanagedRestorationScope : InheritedWidget
{
    public virtual global::Doroti.Generated.Framework.Services.RestorationBucket? bucket { get; private set; }

    public UnmanagedRestorationScope(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Services.RestorationBucket? bucket = null, Widget child = default!) : base(key: key, child: child)
    {
        this.bucket = bucket;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (UnmanagedRestorationScope)(object)oldWidget;
        return (!object.Equals(((UnmanagedRestorationScope)__oldWidget).bucket, this.bucket));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RootRestorationScope : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual string? restorationId { get; private set; }

    public RootRestorationScope(global::Doroti.Generated.Framework.Foundation.Key? key = null, string? restorationId = default!, Widget child = default!) : base(key: key)
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
    internal virtual global::Doroti.Generated.Framework.Services.RestorationBucket? _rootBucket { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Services.RestorationBucket? _ancestorBucket { get; set; } = default;
    internal virtual bool _isLoadingRootBucket { get; set; } = false;

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _ancestorBucket = RestorationScope.maybeOf(this.context);
        _loadRootBucketIfNecessary();
        _okToRenderBlankContainer ??= ((((RootRestorationScope)(object)this.widget).restorationId is not null) && this._needsRootBucketInserted);
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
            return (((((RootRestorationScope)(object)this.widget).restorationId is not null) && this._needsRootBucketInserted) && !this._rootBucketValid);
            return default!;
        }
    }
    internal virtual void _loadRootBucketIfNecessary()
    {
        if ((this._isWaitingForRootBucket && !this._isLoadingRootBucket))
        {
            _isLoadingRootBucket = true;
            global::Doroti.Generated.Framework.Rendering.RendererBinding.instance.deferFirstFrame();
            DartRuntimePrimitives.Ignore(global::Doroti.Generated.Framework.Services.ServicesBinding.instance.restorationManager.rootBucket.then((global::System.Action<global::Doroti.Generated.Framework.Services.RestorationBucket?>)((bucket) => {
_isLoadingRootBucket = false;
if (this.mounted)
{
    global::Doroti.Generated.Framework.Services.ServicesBinding.instance.restorationManager.addListener(() => this._replaceRootBucket());
    setState(((global::System.Action)(() => {
_rootBucket = bucket;
_rootBucketValid = true;
_okToRenderBlankContainer = false;
})));
}
global::Doroti.Generated.Framework.Rendering.RendererBinding.instance.allowFirstFrame();
})));
        }
    }

    internal virtual void _replaceRootBucket()
    {
        _rootBucketValid = false;
        _rootBucket = null;
        global::Doroti.Generated.Framework.Services.ServicesBinding.instance.restorationManager.removeListener(() => this._replaceRootBucket());
        _loadRootBucketIfNecessary();
        DartRuntimePrimitives.Assert(() => !this._isWaitingForRootBucket);
    }

    public override void dispose()
    {
        if (this._rootBucketValid)
        {
            global::Doroti.Generated.Framework.Services.ServicesBinding.instance.restorationManager.removeListener(() => this._replaceRootBucket());
        }
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        if ((DartRuntimePrimitives.RequireValue(this._okToRenderBlankContainer) && this._isWaitingForRootBucket))
        {
            return ((Widget)(object?)SizedBox.CreateShrink());
        }
        return ((Widget)(object?)new UnmanagedRestorationScope(bucket: (this._ancestorBucket ?? this._rootBucket), child: new RestorationScope(restorationId: ((RootRestorationScope)(object)this.widget).restorationId, child: ((RootRestorationScope)(object)this.widget).child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class RestorableProperty<T> : global::Doroti.Generated.Framework.Foundation.ChangeNotifier
{
    public virtual bool _disposed { get; set; } = false;
    public virtual string? _restorationId { get; set; } = default;
    public virtual dynamic _owner { get; set; } = default!;

    protected RestorableProperty()
    {
    }

    public abstract T createDefaultValue();
    public abstract T fromPrimitives(object? data);
    public abstract void initWithValue(T value);
    public abstract object? toPrimitives();
    public virtual bool enabled => true;
    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => ChangeNotifier.debugAssertNotDisposed(this));
        ((dynamic)this._owner)?._unregister(this);
        base.dispose();
        _disposed = true;
    }

    public virtual void _register(string restorationId, dynamic owner)
    {
        DartRuntimePrimitives.Assert(() => ChangeNotifier.debugAssertNotDisposed(this));
        _restorationId = restorationId;
        _owner = owner;
    }

    public virtual void _unregister()
    {
        DartRuntimePrimitives.Assert(() => ChangeNotifier.debugAssertNotDisposed(this));
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
            DartRuntimePrimitives.Assert(() => ChangeNotifier.debugAssertNotDisposed(this));
            return ((IState)(object)this._owner!);
            return default!;
        }
    }
    public virtual bool isRegistered
    {
        get
        {
            DartRuntimePrimitives.Assert(() => ChangeNotifier.debugAssertNotDisposed(this));
            return (this._restorationId is not null);
            return default!;
        }
    }
}

public interface RestorationMixin<S> where S : StatefulWidget
{
    global::Doroti.Generated.Framework.Services.RestorationBucket? _bucket { get; set; }
    DartMap<dynamic, global::System.Action> _properties { get; }
    List<dynamic>? _debugPropertiesWaitingForReregistration { get; set; }
    bool _firstRestorePending { get; set; }
    global::Doroti.Generated.Framework.Services.RestorationBucket? _currentParent { get; set; }

    public string? restorationId { get; }
    public global::Doroti.Generated.Framework.Services.RestorationBucket? bucket { get; }
    public void restoreState(global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket, bool initialRestore);
    public void didToggleBucket(global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket);
    public void didUpdateRestorationId();
    public void didUpdateWidget(S oldWidget);
    public bool restorePending { get; }
    public bool _debugDoingRestore { get; }
    public void didChangeDependencies();
    public void _doRestore(global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket);
    public bool _updateBucketIfNecessary(global::Doroti.Generated.Framework.Services.RestorationBucket? parent, bool restorePending);
    public bool _setNewBucketIfNecessary(global::Doroti.Generated.Framework.Services.RestorationBucket? newBucket, bool restorePending);
    public void _updateProperty(dynamic property);
    public void _unregister(dynamic property);
    public void dispose();
}
