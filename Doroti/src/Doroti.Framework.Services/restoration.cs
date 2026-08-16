#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/restoration.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Services;

internal delegate void _BucketVisitor(RestorationBucket bucket);

public class RestorationManager : ChangeNotifier
{
    internal virtual RestorationBucket? _rootBucket { get; set; } = default;
    internal virtual Completer<RestorationBucket?>? _pendingRootBucket { get; set; } = default;
    internal virtual bool _rootBucketIsValid { get; set; } = false;
    internal virtual bool _isReplacing { get; set; } = false;
    internal virtual bool _debugDoingUpdate { get; set; } = false;
    internal virtual bool _serializationScheduled { get; set; } = false;
    internal virtual HashSet<RestorationBucket> _bucketsNeedingSerialization { get; private set; } = new HashSet<RestorationBucket>();

    public RestorationManager()
    {
    }

    public virtual void initChannels()
    {
        SystemChannels.restoration.setMethodCallHandler(_methodHandler);
    }

    public virtual Future<RestorationBucket?> rootBucket
    {
        get
        {
            if (_rootBucketIsValid)
            {
                return new SynchronousFuture<RestorationBucket?>(_rootBucket);
            }
            if ((_pendingRootBucket is null))
            {
                _pendingRootBucket = new Completer<RestorationBucket?>();
                _ = _getRootBucketFromEngine();
            }
            return _pendingRootBucket!.future;
        }
    }
    public virtual bool isReplacing => _isReplacing;
    internal async virtual Future _getRootBucketFromEngine()
    {
        DartMap<object?, object?>? config = await SystemChannels.restoration.invokeMethod<DartMap<object?, object?>>("get");
        if ((_pendingRootBucket is null))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => (_rootBucket is null));
        _parseAndHandleRestorationUpdateFromEngine(config);
    }

    internal virtual void _parseAndHandleRestorationUpdateFromEngine(DartMap<object?, object?>? update)
    {
        handleRestorationUpdateFromEngine(enabled: ((update is not null) && ((bool)update.GetValueOrDefault("enabled")!)), data: ((update is null) ? null : ((Uint8List?)update.GetValueOrDefault("data"))!));
    }

    public virtual void handleRestorationUpdateFromEngine(bool enabled, Uint8List? data)
    {
        DartRuntimePrimitives.Assert(() => (enabled || (data is null)));
        _isReplacing = (_rootBucketIsValid && enabled);
        if (_isReplacing)
        {
            SchedulerBinding.instance.addPostFrameCallback(((_) =>
            {
                _isReplacing = false;
            }), debugLabel: "RestorationManager.resetIsReplacing");
        }
        RestorationBucket? oldRoot = _rootBucket;
        _rootBucket = (enabled ? RestorationBucket.CreateRoot(manager: this, rawData: _decodeRestorationData(data)) : null);
        _rootBucketIsValid = true;
        DartRuntimePrimitives.Assert(() => ((_pendingRootBucket is null) || !_pendingRootBucket!.isCompleted));
        _pendingRootBucket?.complete(_rootBucket);
        _pendingRootBucket = null;
        if ((!object.Equals(_rootBucket, oldRoot)))
        {
            notifyListeners();
            oldRoot?.dispose();
        }
    }

    public virtual Future sendToEngine(Uint8List encodedData)
    {
        return SystemChannels.restoration.invokeMethod<object?>("put", encodedData);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal async virtual Future _methodHandler(MethodCall call)
    {
        switch (call.method)
        {
            case var __case15057 when object.Equals(__case15057, "push"):
                {
                    _parseAndHandleRestorationUpdateFromEngine(DartRuntimePrimitives.ConvertMap<object?, object?>((System.Collections.IDictionary)call.arguments));
                    break;
                }
            default:
                {
                    throw new NotImplementedException($"{call.method} was invoked but isn't implemented by {this.GetType()}");
                }
        }
    }

    internal virtual DartMap<object?, object?>? _decodeRestorationData(Uint8List? data)
    {
        if ((data is null))
        {
            return null;
        }
        ByteData encoded = data.buffer.asByteData(data.offsetInBytes, data.lengthInBytes);
        return DartRuntimePrimitives.ConvertMap<object?, object?>((System.Collections.IDictionary)new StandardMessageCodec().decodeMessage(encoded));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Uint8List _encodeRestorationData(DartMap<object?, object?> data)
    {
        ByteData encoded = new StandardMessageCodec().encodeMessage(data)!;
        return encoded.buffer.asUint8List(encoded.offsetInBytes, encoded.lengthInBytes);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void scheduleSerializationFor(RestorationBucket bucket)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(bucket._manager, this)));
        DartRuntimePrimitives.Assert(() => !_debugDoingUpdate);
        _bucketsNeedingSerialization.Add(bucket);
        if (!_serializationScheduled)
        {
            _serializationScheduled = true;
            SchedulerBinding.instance.addPostFrameCallback(((_) => _doSerialization()), debugLabel: "RestorationManager.doSerialization");
        }
    }

    public virtual void unscheduleSerializationFor(RestorationBucket bucket)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(bucket._manager, this)));
        DartRuntimePrimitives.Assert(() => !_debugDoingUpdate);
        _bucketsNeedingSerialization.Remove(bucket);
    }

    internal virtual void _doSerialization()
    {
        if (!_serializationScheduled)
        {
            return;
        }
        DartRuntimePrimitives.Assert(() =>
            {
                _debugDoingUpdate = true;
                return true;
            });
        _serializationScheduled = false;
        foreach (RestorationBucket bucket in _bucketsNeedingSerialization)
        {
            bucket.finalize();
        }
        _bucketsNeedingSerialization.Clear();
        _ = sendToEngine(_encodeRestorationData(_rootBucket!._rawData));
        DartRuntimePrimitives.Assert(() =>
            {
                _debugDoingUpdate = false;
                return true;
            });
    }

    public virtual void flushData()
    {
        DartRuntimePrimitives.Assert(() => !_debugDoingUpdate);
        if (SchedulerBinding.instance.hasScheduledFrame)
        {
            return;
        }
        _doSerialization();
        DartRuntimePrimitives.Assert(() => !_serializationScheduled);
    }

    public override void dispose()
    {
        _rootBucket?.dispose();
        base.dispose();
    }

}

public class RestorationBucket
{
    internal const string _childrenMapKey = "c";
    internal const string _valuesMapKey = "v";
    internal virtual DartMap<object?, object?> _rawData { get; private set; } = default!;
    internal virtual object? _debugOwner { get; set; } = default;
    internal virtual RestorationManager? _manager { get; set; } = default;
    internal virtual RestorationBucket? _parent { get; set; } = default;
    internal virtual string _restorationId { get; set; } = default!;
    internal virtual DartMap<string, RestorationBucket> _claimedChildren { get; private set; } = new DartMap<string, RestorationBucket>();
    internal virtual DartMap<string, List<RestorationBucket>> _childrenToAdd { get; private set; } = new DartMap<string, List<RestorationBucket>>();
    internal virtual bool _needsSerialization { get; set; } = false;
    internal virtual bool _debugDisposed { get; set; } = false;

    public RestorationBucket(string restorationId, object? debugOwner)
    {
        this._restorationId = restorationId;
        this._rawData = new DartMap<string, object?>().cast<object?, object?>();
    }

    public static RestorationBucket CreateRoot(RestorationManager manager, DartMap<object?, object?>? rawData)
    {
        var __instance = new RestorationBucket(default!, default!);
        __instance._manager = manager;
        __instance._rawData = (rawData ?? new DartMap<object?, object?>());
        __instance._restorationId = "root";
        return __instance;
    }

    public static RestorationBucket CreateChild(string restorationId, RestorationBucket parent, object? debugOwner)
    {
        var __instance = new RestorationBucket(default!, default!);
        __instance._manager = parent._manager;
        __instance._parent = parent;
        __instance._rawData = DartRuntimePrimitives.ConvertMap<object?, object?>((System.Collections.IDictionary)parent._rawChildren.GetValueOrDefault(restorationId)!);
        __instance._restorationId = restorationId;
        return __instance;
    }

    public virtual object? debugOwner
    {
        get
        {
            DartRuntimePrimitives.Assert(() => _debugAssertNotDisposed());
            return _debugOwner;
        }
    }
    public virtual bool isReplacing => (_manager?.isReplacing ?? false);
    public virtual string restorationId
    {
        get
        {
            DartRuntimePrimitives.Assert(() => _debugAssertNotDisposed());
            return _restorationId;
        }
    }
    internal virtual DartMap<object?, object?> _rawChildren => DartRuntimePrimitives.ConvertMap<object?, object?>((System.Collections.IDictionary)_rawData.putIfAbsent(_childrenMapKey, (() => new DartMap<object?, object?>()))!);
    internal virtual DartMap<object?, object?> _rawValues => DartRuntimePrimitives.ConvertMap<object?, object?>((System.Collections.IDictionary)_rawData.putIfAbsent(_valuesMapKey, (() => new DartMap<object?, object?>()))!);
    public virtual P? read<P>(string restorationId)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertNotDisposed());
        return ((P?)_rawValues.GetValueOrDefault(restorationId))!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void write<P>(string restorationId, P value)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertNotDisposed());
        DartRuntimePrimitives.Assert(() => RestorationLibrary.debugIsSerializableForRestoration(value));
        if (((!object.Equals(_rawValues.GetValueOrDefault(restorationId), value)) || !_rawValues.ContainsKey(restorationId)))
        {
            _rawValues[restorationId] = value;
            _markNeedsSerialization();
        }
    }

    public virtual P? remove<P>(string restorationId)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertNotDisposed());
        bool needsUpdate = _rawValues.ContainsKey(restorationId);
        var result = ((P?)_rawValues.remove(restorationId))!;
        if ((_rawValues.Count == 0))
        {
            _rawData.remove(_valuesMapKey);
        }
        if (needsUpdate)
        {
            _markNeedsSerialization();
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool contains(string restorationId)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertNotDisposed());
        return _rawValues.ContainsKey(restorationId);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RestorationBucket claimChild(string restorationId, object? debugOwner)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertNotDisposed());
        if ((_claimedChildren.ContainsKey(restorationId) || !_rawChildren.ContainsKey(restorationId)))
        {
            var child__33838 = new RestorationBucket(debugOwner: debugOwner, restorationId: restorationId);
            adoptChild(child__33838);
            return child__33838;
        }
        DartRuntimePrimitives.Assert(() => (_rawChildren.GetValueOrDefault(restorationId) is not null));
        var child = RestorationBucket.CreateChild(restorationId: restorationId, parent: this, debugOwner: debugOwner);
        _claimedChildren[restorationId] = child;
        return child;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void adoptChild(RestorationBucket child)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertNotDisposed());
        if ((!object.Equals(child._parent, this)))
        {
            child._parent?._removeChildData(child);
            child._parent = this;
            _addChildData(child);
            if ((!object.Equals(child._manager, _manager)))
            {
                _recursivelyUpdateManager(child);
            }
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(child._parent, this)));
        DartRuntimePrimitives.Assert(() => (object.Equals(child._manager, _manager)));
    }

    internal virtual void _dropChild(RestorationBucket child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child._parent, this)));
        _removeChildData(child);
        child._parent = null;
        if ((child._manager is not null))
        {
            child._updateManager(null);
            child._visitChildren(_recursivelyUpdateManager);
        }
    }

    internal virtual void _markNeedsSerialization()
    {
        if (!_needsSerialization)
        {
            _needsSerialization = true;
            _manager?.scheduleSerializationFor(this);
        }
    }

    public virtual void finalize()
    {
        DartRuntimePrimitives.Assert(() => _debugAssertNotDisposed());
        DartRuntimePrimitives.Assert(() => _needsSerialization);
        _needsSerialization = false;
        DartRuntimePrimitives.Assert(() => _debugAssertIntegrity());
    }

    internal virtual void _recursivelyUpdateManager(RestorationBucket bucket)
    {
        bucket._updateManager(_manager);
        bucket._visitChildren(_recursivelyUpdateManager);
    }

    internal virtual void _updateManager(RestorationManager? newManager)
    {
        if ((object.Equals(_manager, newManager)))
        {
            return;
        }
        if (_needsSerialization)
        {
            _manager?.unscheduleSerializationFor(this);
        }
        _manager = newManager;
        if ((_needsSerialization && (_manager is not null)))
        {
            _needsSerialization = false;
            _markNeedsSerialization();
        }
    }

    internal virtual bool _debugAssertIntegrity()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((_childrenToAdd.Count == 0))
                {
                    return true;
                }
                var error = new List<DiagnosticsNode> { new ErrorSummary("Multiple owners claimed child RestorationBuckets with the same IDs."), new ErrorDescription($"The following IDs were claimed multiple times from the parent {this}:") };
                foreach (MapEntry<string, List<RestorationBucket>> child in _childrenToAdd.entries)
                {
                    string id__37151 = child.key;
                    List<RestorationBucket> buckets__37205 = child.value;
                    DartRuntimePrimitives.Assert(() => (buckets__37205.Count != 0));
                    DartRuntimePrimitives.Assert(() => _claimedChildren.ContainsKey(id__37151));
                    error.AddRange(new List<DiagnosticsNode> { new ErrorDescription($" * \"{id__37151}\" was claimed by:"), new ErrorDescription($"   * {_claimedChildren.GetValueOrDefault(id__37151)!.debugOwner} (current owner)") });
                }
                throw new FlutterError(error);
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _removeChildData(RestorationBucket child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child._parent, this)));
        if ((object.Equals(_claimedChildren.remove(child.restorationId), child)))
        {
            _rawChildren.remove(child.restorationId);
            List<RestorationBucket>? pendingChildren__37957 = _childrenToAdd.GetValueOrDefault(child.restorationId);
            if ((pendingChildren__37957 is not null))
            {
                RestorationBucket toAdd__38081 = pendingChildren__37957.removeLast();
                _finalizeAddChildData(toAdd__38081);
                if ((pendingChildren__37957.Count == 0))
                {
                    _childrenToAdd.remove(child.restorationId);
                }
            }
            if ((_rawChildren.Count == 0))
            {
                _rawData.remove(_childrenMapKey);
            }
            _markNeedsSerialization();
            return;
        }
        _childrenToAdd.GetValueOrDefault(child.restorationId)?.Remove(child);
        if ((((bool?)((_childrenToAdd.GetValueOrDefault(child.restorationId)?.Count == 0))) ?? false))
        {
            _childrenToAdd.remove(child.restorationId);
        }
    }

    internal virtual void _addChildData(RestorationBucket child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child._parent, this)));
        if (_claimedChildren.ContainsKey(child.restorationId))
        {
            _childrenToAdd.putIfAbsent(child.restorationId, (() => new List<RestorationBucket>())).Add(child);
            _markNeedsSerialization();
            return;
        }
        _finalizeAddChildData(child);
        _markNeedsSerialization();
    }

    internal virtual void _finalizeAddChildData(RestorationBucket child)
    {
        DartRuntimePrimitives.Assert(() => (_claimedChildren.GetValueOrDefault(child.restorationId) is null));
        DartRuntimePrimitives.Assert(() => (_rawChildren.GetValueOrDefault(child.restorationId) is null));
        _claimedChildren[child.restorationId] = child;
        _rawChildren[child.restorationId] = child._rawData;
    }

    internal virtual void _visitChildren(Action<RestorationBucket> visitor, bool concurrentModification = false)
    {
        IEnumerable<RestorationBucket> children = _claimedChildren.Values.followedBy(_childrenToAdd.Values.expand(((buckets) => buckets)));
        if (concurrentModification)
        {
            children = children.ToList();
        }
        children.forEach(visitor);
    }

    public virtual void rename(string newRestorationId)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertNotDisposed());
        if ((newRestorationId == restorationId))
        {
            return;
        }
        _parent?._removeChildData(this);
        _restorationId = newRestorationId;
        _parent?._addChildData(this);
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => _debugAssertNotDisposed());
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        _visitChildren(_dropChild, concurrentModification: true);
        _claimedChildren.Clear();
        _childrenToAdd.Clear();
        _parent?._removeChildData(this);
        _parent = null;
        _updateManager(null);
        _debugDisposed = true;
    }

    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "RestorationBucket"))}(restorationId: {restorationId}, owner: {debugOwner})";
    internal virtual bool _debugAssertNotDisposed()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (_debugDisposed)
                {
                    throw new FlutterError($"A {this.GetType()} was used after being disposed.\n" + $"Once you have called dispose() on a {this.GetType()}, it can no longer be used.");
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class RestorationLibrary
{
    public static bool debugIsSerializableForRestoration(object? @object)
    {
        var result = false;
        DartRuntimePrimitives.Assert(() =>
            {
                try
                {
                    new StandardMessageCodec().encodeMessage(@object);
                    result = true;
                }
                catch (Exception error)
                {
                    result = false;
                }
                return true;
            });
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

