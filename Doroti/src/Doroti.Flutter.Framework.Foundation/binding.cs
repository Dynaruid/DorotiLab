// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/foundation/binding.dart
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;

namespace Doroti.Generated.Framework.Foundation;

public delegate Task<Dictionary<string, object?>> ServiceExtensionCallback(IReadOnlyDictionary<string, string> parameters);

public abstract class BindingBase : IDisposable
{
    public static Type? debugBindingType() => typeof(BindingBase);
    private readonly Dictionary<string, ServiceExtensionCallback> _serviceExtensions = new(StringComparer.Ordinal);
    private readonly Queue<Func<Task>> _lockedEvents = new();
    private int _lockCount;
    private bool _disposed;

    protected BindingBase(PlatformDispatcher? platformDispatcher = null)
    {
        this.platformDispatcher = platformDispatcher ?? new PlatformDispatcher();
        ownsPlatformDispatcher = platformDispatcher is null;
        using var initialization = ConstantsLibrary.kReleaseMode ? null : FlutterTimeline.startSync("Framework initialization");
        initInstances();
        initServiceExtensions();
    }

    public PlatformDispatcher platformDispatcher { get; }

    public bool locked => _lockCount > 0;

    protected bool ownsPlatformDispatcher { get; }

    protected virtual void initInstances()
    {
    }

    protected virtual void initServiceExtensions()
    {
        registerSignalServiceExtension("reassemble", reassembleApplication);
    }

    public static T checkInstance<T>(T? instance)
        where T : class => instance ?? throw new FlutterError(
            $"Binding has not yet been initialized. Construct the {typeof(T).Name} binding before reading instance.");

    public virtual bool debugCheckZone(string entryPoint)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryPoint);
        return true;
    }

    private async Task lockTaskEvents(Func<Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        ObjectDisposedException.ThrowIf(_disposed, this);
        _lockCount++;
        try
        {
            await callback().ConfigureAwait(false);
        }
        finally
        {
            _lockCount--;
            if (_lockCount == 0)
            {
                unlocked();
            }
        }
    }

    protected virtual void unlocked()
    {
        while (_lockedEvents.TryDequeue(out var callback))
        {
            callback().GetAwaiter().GetResult();
        }
    }

    public Future lockEvents(Func<Future> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        return Future.fromTask(LockFutureEvents(callback));
    }

    private async Task LockFutureEvents(Func<Future> callback)
    {
        _lockCount++;
        try
        {
            await callback();
        }
        finally
        {
            _lockCount--;
            if (_lockCount == 0)
            {
                unlocked();
            }
        }
    }

    protected void deferEvent(Func<Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        if (_lockCount == 0)
        {
            callback().GetAwaiter().GetResult();
        }
        else
        {
            _lockedEvents.Enqueue(callback);
        }
    }

    public virtual Task reassembleApplication() => lockTaskEvents(performReassemble);

    protected virtual Task performReassemble() => Task.CompletedTask;

    public void registerSignalServiceExtension(string name, AsyncCallback callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        registerServiceExtension(name, async _ =>
        {
            await callback().ConfigureAwait(false);
            return new Dictionary<string, object?> { ["type"] = "Success" };
        });
    }

    public void registerBoolServiceExtension(string name, AsyncValueGetter<bool> getter, AsyncValueSetter<bool> setter)
    {
        registerServiceExtension(name, async parameters =>
        {
            if (parameters.TryGetValue("enabled", out var raw))
            {
                await setter(string.Equals(raw, "true", StringComparison.OrdinalIgnoreCase)).ConfigureAwait(false);
            }
            return new Dictionary<string, object?> { ["enabled"] = (await getter().ConfigureAwait(false)).ToString().ToLowerInvariant() };
        });
    }

    public void registerBoolServiceExtension(string name, Func<Future<bool>> getter, Func<bool, Future> setter)
    {
        ArgumentNullException.ThrowIfNull(getter);
        ArgumentNullException.ThrowIfNull(setter);
        registerBoolServiceExtension(name, () => getter().asTask(), value => setter(value).asTask());
    }

    public void registerBoolServiceExtension(string name, Func<bool> getter, Action<bool> setter)
    {
        registerBoolServiceExtension(name, () => Task.FromResult(getter()), value => { setter(value); return Task.CompletedTask; });
    }

    public void registerNumericServiceExtension(string name, AsyncValueGetter<double> getter, AsyncValueSetter<double> setter)
    {
        registerServiceExtension(name, async parameters =>
        {
            if (parameters.TryGetValue(name, out var raw))
            {
                await setter(double.Parse(raw, System.Globalization.CultureInfo.InvariantCulture)).ConfigureAwait(false);
            }
            return new Dictionary<string, object?> { [name] = await getter().ConfigureAwait(false) };
        });
    }

    public void registerNumericServiceExtension(string name, Func<double> getter, Action<double> setter)
    {
        ArgumentNullException.ThrowIfNull(getter);
        ArgumentNullException.ThrowIfNull(setter);
        registerServiceExtension(name, parameters =>
        {
            if (parameters.TryGetValue(name, out var raw))
            {
                setter(double.Parse(raw, System.Globalization.CultureInfo.InvariantCulture));
            }
            return Task.FromResult(new Dictionary<string, object?> { [name] = getter() });
        });
    }

    public void registerStringServiceExtension(string name, AsyncValueGetter<string> getter, AsyncValueSetter<string> setter)
    {
        registerServiceExtension(name, async parameters =>
        {
            if (parameters.TryGetValue("value", out var value))
            {
                await setter(value).ConfigureAwait(false);
            }
            return new Dictionary<string, object?> { ["value"] = await getter().ConfigureAwait(false) };
        });
    }

    public void registerStringServiceExtension(string name, Func<string> getter, Action<string> setter)
    {
        registerStringServiceExtension(name, () => Task.FromResult(getter()), value => { setter(value); return Task.CompletedTask; });
    }

    public void registerServiceExtension(string name, ServiceExtensionCallback callback)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(callback);
        if (!_serviceExtensions.TryAdd(name, callback))
        {
            throw new InvalidOperationException($"Service extension '{name}' is already registered.");
        }
    }

    public void registerServiceExtension(
        string name,
        Func<DartMap<string, string>, Future<DartMap<string, object>>> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        registerServiceExtension(name, async parameters =>
        {
            var dartParameters = new DartMap<string, string>(parameters);
            var result = await callback(dartParameters);
            return result.ToDictionary(item => item.Key, item => (object?)item.Value, StringComparer.Ordinal);
        });
    }

    public async Task<IReadOnlyDictionary<string, object?>> invokeServiceExtension(string name, IReadOnlyDictionary<string, string>? parameters = null)
    {
        if (!_serviceExtensions.TryGetValue(name, out var callback))
        {
            throw new KeyNotFoundException($"Service extension '{name}' is not registered.");
        }
        var result = await callback(parameters ?? new Dictionary<string, string>()).ConfigureAwait(false);
        result["method"] = name;
        result.TryAdd("type", "_extensionType");
        return result;
    }

    public void postEvent<T>(string eventKind, IReadOnlyDictionary<string, T> eventData)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(eventKind);
        ArgumentNullException.ThrowIfNull(eventData);
        var normalized = eventData.ToDictionary(
            pair => pair.Key,
            pair => (object?)pair.Value,
            StringComparer.Ordinal);
        serviceExtensionEvent?.Invoke(eventKind, normalized);
    }

    public event Action<string, IReadOnlyDictionary<string, object?>>? serviceExtensionEvent;

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        if (ownsPlatformDispatcher)
        {
            platformDispatcher.Dispose();
        }
        GC.SuppressFinalize(this);
    }

    public override string ToString() => $"<{GetType().Name}>";
}

public static class BindingLibrary
{
    internal static Task _exitApplication(int exitCode = 0) => Task.FromException(
        new FlutterCapabilityException(
            FlutterCapabilityIds.PlatformServices,
            null,
            DartUiInvocation.Managed("dart:io#exit"),
            $"foundation requested process exit code {exitCode}; only the active host may terminate the application"));
}
