// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/page_storage.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Widgets;

public class PageStorageKey<T> : global::Doroti.Framework.Foundation.ValueKey<T>
{
    public PageStorageKey(T value) : base(value)
    {
    }

}

internal class _StorageEntryIdentifier__page_storage
{
    public virtual List<PageStorageKey<object>> keys { get; private set; } = default!;

    internal _StorageEntryIdentifier__page_storage(List<PageStorageKey<object>> keys)
    {
        this.keys = keys;
    }

    public virtual bool isNotEmpty => System.Linq.Enumerable.Any(this.keys);
    public override bool Equals(object? other)
    {
        var __other = other as _StorageEntryIdentifier__page_storage;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((__other is _StorageEntryIdentifier__page_storage) && global::Doroti.Framework.Foundation.CollectionsLibrary.listEquals<PageStorageKey<object>>(((_StorageEntryIdentifier__page_storage)((_StorageEntryIdentifier__page_storage)__other)).keys, this.keys));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHashAll(this.keys));
    public override string ToString()
    {
        return $"StorageEntryIdentifier({string.Join(":", this.keys)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class PageStorageBucket
{
    internal virtual DartMap<object, object>? _storage { get; set; } = default;

    internal static bool _maybeAddKey(BuildContext context, List<PageStorageKey<object>> keys)
    {
        Widget widgetLocal = ((BuildContext)context).widget;
        global::Doroti.Framework.Foundation.Key? keyLocal = ((Widget)widgetLocal).key;
        if ((keyLocal is PageStorageKey<object>))
        {
            PageStorageKey<object> key__2231__as2257 = (PageStorageKey<object>)keyLocal;
            keys.Add(((PageStorageKey<object>)key__2231__as2257));
        }
        return (widgetLocal is not PageStorage);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<PageStorageKey<object>> _allKeys(BuildContext context)
    {
        var keys = new List<PageStorageKey<object>>();
        if (PageStorageBucket._maybeAddKey(context, keys))
        {
            context.visitAncestorElements(((global::System.Func<Element, bool>)((element) =>
            {
                return PageStorageBucket._maybeAddKey(element, keys);
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
        }
        return keys;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual _StorageEntryIdentifier__page_storage _computeIdentifier(BuildContext context)
    {
        return new _StorageEntryIdentifier__page_storage(_allKeys(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void writeState(BuildContext context, dynamic data, object? identifier = null)
    {
        _storage ??= new DartMap<object, object>();
        if ((identifier is not null))
        {
            this._storage![identifier] = data;
        }
        else
        {
            _StorageEntryIdentifier__page_storage contextIdentifier = ((_StorageEntryIdentifier__page_storage)(object?)_computeIdentifier(context));
            if (((_StorageEntryIdentifier__page_storage)contextIdentifier).isNotEmpty)
            {
                this._storage![contextIdentifier] = data;
            }
        }
    }

    public virtual dynamic readState(BuildContext context, object? identifier = null)
    {
        if ((this._storage is null))
        {
            return null;
        }
        if ((identifier is not null))
        {
            return this._storage!.GetValueOrDefault(identifier);
        }
        _StorageEntryIdentifier__page_storage contextIdentifier = ((_StorageEntryIdentifier__page_storage)(object?)_computeIdentifier(context));
        return (((_StorageEntryIdentifier__page_storage)contextIdentifier).isNotEmpty ? this._storage!.GetValueOrDefault(contextIdentifier) : null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class PageStorage : StatelessWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual PageStorageBucket bucket { get; private set; } = default!;

    public PageStorage(global::Doroti.Framework.Foundation.Key? key = null, PageStorageBucket bucket = default!, Widget child = default!) : base(key: key)
    {
        this.bucket = bucket;
        this.child = child;
    }

    public static PageStorageBucket? maybeOf(BuildContext context)
    {
        PageStorage? widget = ((PageStorage?)(object?)context.findAncestorWidgetOfExactType<PageStorage>());
        return widget?.bucket;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static PageStorageBucket of(BuildContext context)
    {
        PageStorageBucket? bucket = ((PageStorageBucket?)(object?)PageStorage.maybeOf(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((bucket is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("PageStorage.of() was called with a context that does not contain a " + "PageStorage widget.\n" + "No PageStorage widget ancestor could be found starting from the " + "context that was passed to PageStorage.of(). This can happen " + "because you are using a widget that looks for a PageStorage " + "ancestor, but no such ancestor exists.\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return bucket!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context) => this.child;
}

