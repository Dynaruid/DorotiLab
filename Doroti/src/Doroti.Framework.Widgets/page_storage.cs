// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/page_storage.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public class PageStorageKey<T> : ValueKey<T>
{
    public PageStorageKey(T value)
        : base(value) { }
}

internal class _StorageEntryIdentifier__page_storage
{
    public virtual List<PageStorageKey<object>> keys { get; private set; } = default!;

    internal _StorageEntryIdentifier__page_storage(List<PageStorageKey<object>> keys)
    {
        this.keys = keys;
    }

    public virtual bool isNotEmpty => Enumerable.Any(keys);

    public override bool Equals(object? other)
    {
        var __other = other as _StorageEntryIdentifier__page_storage;
        if (__other is null)
        {
            return false;
        }

        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is _StorageEntryIdentifier__page_storage)
            && CollectionsLibrary.listEquals(__other.keys, keys);
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHashAll(keys));

    public override string ToString()
    {
        return $"StorageEntryIdentifier({string.Join(":", keys)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class PageStorageBucket
{
    internal virtual DartMap<object, object?>? _storage { get; set; } = default;

    internal static bool _maybeAddKey(BuildContext context, List<PageStorageKey<object>> keys)
    {
        Widget widgetLocal = context.widget;
        Key? keyLocal = widgetLocal.key;
        if (keyLocal is PageStorageKey<object>)
        {
            PageStorageKey<object> key__2231__as2257 = (PageStorageKey<object>)keyLocal;
            keys.Add(key__2231__as2257);
        }
        return widgetLocal is not PageStorage;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<PageStorageKey<object>> _allKeys(BuildContext context)
    {
        var keys = new List<PageStorageKey<object>>();
        if (_maybeAddKey(context, keys))
        {
            context.visitAncestorElements(
                (element) =>
                {
                    return _maybeAddKey(element, keys);
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }
            );
        }
        return keys;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual _StorageEntryIdentifier__page_storage _computeIdentifier(BuildContext context)
    {
        return new _StorageEntryIdentifier__page_storage(_allKeys(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void writeState(BuildContext context, object? data, object? identifier = null)
    {
        _storage ??= new DartMap<object, object?>();
        if (identifier is not null)
        {
            _storage![identifier] = data;
        }
        else
        {
            _StorageEntryIdentifier__page_storage contextIdentifier = _computeIdentifier(context);
            if (contextIdentifier.isNotEmpty)
            {
                _storage![contextIdentifier] = data;
            }
        }
    }

    public virtual object? readState(BuildContext context, object? identifier = null)
    {
        if (_storage is null)
        {
            return null;
        }
        if (identifier is not null)
        {
            return _storage!.GetValueOrDefault(identifier);
        }
        _StorageEntryIdentifier__page_storage contextIdentifier = _computeIdentifier(context);
        return contextIdentifier.isNotEmpty ? _storage!.GetValueOrDefault(contextIdentifier) : null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class PageStorage : StatelessWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual PageStorageBucket bucket { get; private set; } = default!;

    public PageStorage(
        Key? key = null,
        PageStorageBucket bucket = default!,
        Widget child = default!
    )
        : base(key: key)
    {
        this.bucket = bucket;
        this.child = child;
    }

    public static PageStorageBucket? maybeOf(BuildContext context)
    {
        PageStorage? widget = context.findAncestorWidgetOfExactType<PageStorage>();
        return widget?.bucket;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static PageStorageBucket of(BuildContext context)
    {
        PageStorageBucket? bucket = maybeOf(context);
        DartRuntimePrimitives.Assert(() =>
        {
            if (bucket is null)
            {
                throw DartRuntimePrimitives.AsException(
                    FlutterError.Create(
                        "PageStorage.of() was called with a context that does not contain a "
                            + "PageStorage widget.\n"
                            + "No PageStorage widget ancestor could be found starting from the "
                            + "context that was passed to PageStorage.of(). This can happen "
                            + "because you are using a widget that looks for a PageStorage "
                            + "ancestor, but no such ancestor exists.\n"
                            + "The context used was:\n"
                            + $"  {context}"
                    )
                );
            }
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        return bucket!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context) => child;
}
