using System.Reflection;
using Doroti.Ui;

namespace Doroti.Hosting;

/// <summary>Target-neutral application startup implemented by every Doroti application.</summary>
public interface IDorotiApplicationStartup
{
    void Configure(DorotiApplicationBuilder builder);
}

public sealed record DorotiLaunchContext(
    string Target,
    string RuntimeIdentifier,
    IReadOnlyList<string> Arguments,
    Uri? BaseUri = null
)
{
    public static DorotiLaunchContext Create(
        string target,
        string runtimeIdentifier,
        IEnumerable<string>? arguments = null,
        Uri? baseUri = null
    ) => new(target, runtimeIdentifier, (arguments ?? []).ToArray(), baseUri);
}

/// <summary>Generated registration for a target adapter such as a browser JavaScript plugin.</summary>
public sealed record DorotiApplicationPluginRegistration(
    string Id,
    string Channel,
    string Adapter,
    string Module,
    string ExportName
);

/// <summary>Immutable application definition shared by every native or browser host.</summary>
public sealed record DorotiApplicationDescriptor(
    Func<IDorotiViewEntrypoint> EntrypointFactory,
    Assembly ApplicationAssembly,
    Assembly ManifestAssembly,
    DorotiViewConfiguration ViewConfiguration,
    DorotiLaunchContext LaunchContext,
    IReadOnlyList<DorotiApplicationPluginRegistration> PluginRegistrations,
    IReadOnlyList<IDorotiNativePluginHandler> NativePluginHandlers,
    IReadOnlyList<DorotiManagedPluginRegistration>? ManagedPluginRegistrations = null
);

public sealed class DorotiApplicationBuilder : IDisposable
{
    private readonly Assembly _applicationAssembly;
    private readonly Assembly _manifestAssembly;
    private readonly DorotiLaunchContext _launchContext;
    private readonly List<DorotiApplicationPluginRegistration> _plugins = [];
    private readonly List<IDorotiNativePluginHandler> _nativePluginHandlers = [];
    private readonly List<DorotiManagedPluginRegistration> _managedPlugins = [];
    private Func<IDorotiViewEntrypoint>? _entrypointFactory;
    private DorotiViewConfiguration? _viewConfiguration;
    private bool _built;
    private bool _disposed;

    public DorotiApplicationBuilder(
        Assembly applicationAssembly,
        DorotiLaunchContext launchContext,
        Assembly? manifestAssembly = null
    )
    {
        _applicationAssembly =
            applicationAssembly ?? throw new ArgumentNullException(nameof(applicationAssembly));
        _manifestAssembly = manifestAssembly ?? applicationAssembly;
        _launchContext = launchContext ?? throw new ArgumentNullException(nameof(launchContext));
    }

    public DorotiApplicationBuilder UseEntrypoint(Func<IDorotiViewEntrypoint> entrypointFactory)
    {
        EnsureMutable();
        _entrypointFactory =
            entrypointFactory ?? throw new ArgumentNullException(nameof(entrypointFactory));
        return this;
    }

    public DorotiApplicationBuilder UseView(DorotiViewConfiguration configuration)
    {
        EnsureMutable();
        _viewConfiguration =
            configuration ?? throw new ArgumentNullException(nameof(configuration));
        return this;
    }

    public DorotiApplicationBuilder AddPlugin(DorotiApplicationPluginRegistration registration)
    {
        EnsureMutable();
        ArgumentNullException.ThrowIfNull(registration);
        if (
            _plugins.Any(item => item.Id == registration.Id || item.Channel == registration.Channel)
        )
        {
            throw new InvalidOperationException(
                $"Doroti plugin id/channel is duplicated: {registration.Id}/{registration.Channel}."
            );
        }

        _plugins.Add(registration);
        return this;
    }

    public DorotiApplicationBuilder AddNativePluginHandler(IDorotiNativePluginHandler handler)
    {
        EnsureMutable();
        ArgumentNullException.ThrowIfNull(handler);
        if (_nativePluginHandlers.Any(item => item.PluginId == handler.PluginId))
        {
            throw new InvalidOperationException(
                $"Doroti native plugin handler is duplicated: {handler.PluginId}."
            );
        }

        _nativePluginHandlers.Add(handler);
        return this;
    }


    public DorotiApplicationBuilder AddNativePluginHandlerFactory(Func<IDorotiNativePluginHandler> factory)
    {
        EnsureMutable();
        ArgumentNullException.ThrowIfNull(factory);
        var handler = factory() ?? throw new InvalidOperationException("A native plugin factory returned null.");
        try { return AddNativePluginHandler(handler); }
        catch
        {
            if (!_nativePluginHandlers.Any(owned => ReferenceEquals(owned, handler))) (handler as IDisposable)?.Dispose();
            throw;
        }
    }

    public DorotiApplicationBuilder AddManagedPluginFactory<TRequest, TResult>(string pluginId,
        Func<IDorotiManagedPluginHandler<TRequest, TResult>> factory)
    {
        EnsureMutable();
        if (_managedPlugins.Any(plugin => plugin.PluginId == pluginId))
            throw new InvalidOperationException($"Managed plugin is duplicated: {pluginId}.");
        _managedPlugins.Add(DorotiManagedPluginRegistration.Create(pluginId, factory));
        return this;
    }

    private void EnsureMutable()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_built) throw new InvalidOperationException("The application descriptor is already frozen.");
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (!_built)
            Doroti.Runtime.DorotiCleanup.Run(_nativePluginHandlers.OfType<IDisposable>()
                .Select<IDisposable, Action>(handler => handler.Dispose).ToArray());
        _nativePluginHandlers.Clear();
    }

    public DorotiApplicationDescriptor Build()
    {
        EnsureMutable();
        if (_entrypointFactory is null)
        {
            throw new InvalidOperationException(
                $"{nameof(UseEntrypoint)} must be called by the Doroti startup."
            );
        }

        if (_viewConfiguration is null)
        {
            throw new InvalidOperationException(
                $"{nameof(UseView)} must be called by the Doroti startup."
            );
        }

        _built = true;
        return new(
            _entrypointFactory,
            _applicationAssembly,
            _manifestAssembly,
            _viewConfiguration,
            _launchContext,
            Array.AsReadOnly(_plugins.ToArray()),
            Array.AsReadOnly(_nativePluginHandlers.ToArray()),
            Array.AsReadOnly(_managedPlugins.ToArray())
        );
    }
}

/// <summary>Strongly typed factory called only from SDK-owned generated bootstrap code.</summary>
public static class DorotiApplicationFactory
{
    public static DorotiApplicationDescriptor Create<TStartup>(
        DorotiLaunchContext launchContext,
        IEnumerable<DorotiApplicationPluginRegistration>? plugins = null,
        Assembly? manifestAssembly = null,
        IEnumerable<IDorotiNativePluginHandler>? nativePluginHandlers = null,
        IEnumerable<Func<IDorotiNativePluginHandler>>? nativePluginHandlerFactories = null
    )
        where TStartup : IDorotiApplicationStartup, new()
    {
        ArgumentNullException.ThrowIfNull(launchContext);
        using var builder = new DorotiApplicationBuilder(
            typeof(TStartup).Assembly,
            launchContext,
            manifestAssembly
        );
        new TStartup().Configure(builder);
        foreach (var plugin in plugins ?? [])
        {
            builder.AddPlugin(plugin);
        }

        foreach (var handler in nativePluginHandlers ?? [])
        {
            builder.AddNativePluginHandler(handler);
        }

        foreach (var factory in nativePluginHandlerFactories ?? []) builder.AddNativePluginHandlerFactory(factory);
        return builder.Build();
    }
}
