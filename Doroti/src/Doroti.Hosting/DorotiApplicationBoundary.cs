using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using Doroti.Ui;

namespace Doroti.Hosting;

public interface IDorotiNativePluginHandler
{
    string PluginId { get; }

    string AbiVersion { get; }

    ValueTask<ReadOnlyMemory<byte>?> HandleAsync(
        string channel,
        string codec,
        ReadOnlyMemory<byte>? message,
        CancellationToken cancellationToken = default
    );
}

public sealed record DorotiNativePluginPackage(
    string Rid,
    string PackageId,
    string Version,
    string AbiVersion,
    string HandlerType
);

public sealed record DorotiApplicationPlugin(
    string Id,
    string Channel,
    string Codec,
    string CapabilityId,
    DorotiNativePluginPackage NativePackage
);

public sealed record DorotiApplicationManifest(
    string SchemaVersion,
    string ApplicationId,
    string TargetRid,
    DorotiEmbeddedResource[] Resources,
    DorotiApplicationPlugin[] Plugins
)
{
    private DorotiPlatformViewRegistration[] _platformViews = [];

    // Older v1 manifests omit this optional field. The source-generated object
    // creator may explicitly assign its default null after running initializers.
    public DorotiPlatformViewRegistration[] PlatformViews
    {
        get => _platformViews;
        init => _platformViews = value ?? [];
    }
}

public sealed record DorotiPlatformViewRegistration(string ViewType, string Rid);

public sealed record DorotiEmbeddedResource(
    string Key,
    string Kind,
    string? FontFamily,
    string? Locale,
    string EmbeddedResourceName,
    string Sha256,
    long Length
);

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(DorotiApplicationManifest))]
[JsonSerializable(typeof(DorotiApplicationPlugin[]))]
internal sealed partial class DorotiApplicationManifestJsonContext : JsonSerializerContext;

/// <summary>Compiler-produced application resource and plugin boundary for one target RID.</summary>
public sealed class DorotiApplicationBoundary : IDisposable
{
    private readonly ApplicationResourceCapability _resources;
    private readonly ApplicationPluginCapability _plugins;
    private readonly PlatformViewFactoryRegistry _platformViews;
    private readonly SharedApplicationServices _shared;
    private int _disposed;

    private sealed class SharedApplicationServices(ApplicationResourceCapability resources, ApplicationPluginCapability plugins)
    {
        private int _owners = 1;
        private readonly object _gate = new();
        public ApplicationResourceCapability Resources => resources;
        public ApplicationPluginCapability Plugins => plugins;
        public void Retain()
        {
            lock (_gate)
            {
                ObjectDisposedException.ThrowIf(_owners == 0, this);
                _owners++;
            }
        }
        public void Release()
        {
            lock (_gate) { if (--_owners != 0) return; }
            try { plugins.Dispose(); }
            finally { resources.Dispose(); }
        }
    }

    private DorotiApplicationBoundary(DorotiApplicationBoundary source, PlatformViewFactoryRegistry factories)
    {
        _shared = source._shared;
        _shared.Retain();
        Manifest = source.Manifest;
        _resources = _shared.Resources;
        _plugins = _shared.Plugins;
        _platformViews = factories;
    }

    /// <summary>Retains application-owned handlers while a new native window is being initialized.</summary>
    public DorotiApplicationBoundary Retain()
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        return new(this, _platformViews);
    }

    /// <summary>Shares application services, but binds native view factories to the new window.</summary>
    public DorotiApplicationBoundary CreateWindowBoundary(IEnumerable<IPlatformViewFactory> factories)
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        return new(this, CreatePlatformViewRegistry(Manifest, factories));
    }

    private DorotiApplicationBoundary(
        DorotiApplicationManifest manifest,
        Assembly assembly,
        IEnumerable<IDorotiNativePluginHandler> handlers,
        IEnumerable<IPlatformViewFactory> platformViewFactories
    )
    {
        Manifest = manifest;
        _resources = new(manifest, assembly);
        _plugins = new(manifest, handlers);
        _platformViews = CreatePlatformViewRegistry(manifest, platformViewFactories);
        _shared = new(_resources, _plugins);
    }

    public DorotiApplicationManifest Manifest { get; }
    public IApplicationResourceHostCapability ApplicationResources => _resources;

    /// <summary>Strongly typed alternative for generated hosts without an embedded manifest.</summary>
    public static DorotiApplicationBoundary Create(DorotiApplicationManifest manifest, Assembly applicationAssembly,
        IEnumerable<IDorotiNativePluginHandler>? handlers = null, IEnumerable<IPlatformViewFactory>? platformViewFactories = null)
    {
        ArgumentNullException.ThrowIfNull(manifest);
        ArgumentNullException.ThrowIfNull(applicationAssembly);
        if (manifest.SchemaVersion != "doroti.application-capabilities/v1")
            throw new InvalidDataException($"Unsupported generated application manifest: {manifest.SchemaVersion}");
        return new(manifest, applicationAssembly, handlers ?? [], platformViewFactories ?? []);
    }

    public static DorotiApplicationBoundary Load(
        Assembly applicationAssembly,
        string targetRid,
        IEnumerable<IDorotiNativePluginHandler>? handlers = null,
        IEnumerable<IPlatformViewFactory>? platformViewFactories = null
    ) => Load(applicationAssembly, applicationAssembly, targetRid, handlers, platformViewFactories);

    public static DorotiApplicationBoundary Load(
        Assembly manifestAssembly,
        Assembly applicationAssembly,
        string targetRid,
        IEnumerable<IDorotiNativePluginHandler>? handlers = null,
        IEnumerable<IPlatformViewFactory>? platformViewFactories = null
    )
    {
        ArgumentNullException.ThrowIfNull(manifestAssembly);
        ArgumentNullException.ThrowIfNull(applicationAssembly);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetRid);
        using var stream =
            manifestAssembly.GetManifestResourceStream("Doroti.Application.Manifest")
            ?? throw new InvalidDataException(
                "Doroti runner assembly is missing Doroti.Application.Manifest."
            );
        var manifest =
            JsonSerializer.Deserialize(
                stream,
                DorotiApplicationManifestJsonContext.Default.DorotiApplicationManifest
            ) ?? throw new InvalidDataException("Generated application manifest is empty.");
        if (manifest.SchemaVersion != "doroti.application-capabilities/v1")
        {
            throw new InvalidDataException(
                $"Unsupported generated application manifest: {manifest.SchemaVersion}"
            );
        }

        if (manifest.TargetRid != targetRid)
        {
            throw new DorotiCapabilityException(
                DorotiCapabilityIds.PlatformPlugins,
                null,
                DorotiUiInvocation.Managed("Doroti.Hosting#LoadApplication"),
                $"application targets RID '{manifest.TargetRid}', not '{targetRid}'",
                targetRid
            );
        }

        using var packagePlugins = manifestAssembly.GetManifestResourceStream("Doroti.Application.PackagePlugins");
        if (packagePlugins is not null)
            manifest = manifest with { Plugins = [..manifest.Plugins, ..JsonSerializer.Deserialize(packagePlugins,
                DorotiApplicationManifestJsonContext.Default.DorotiApplicationPluginArray) ?? []] };

        return new(manifest, applicationAssembly, handlers ?? [], platformViewFactories ?? []);
    }

    public static PlatformViewFactoryRegistry CreatePlatformViewRegistry(
        DorotiApplicationManifest manifest,
        IEnumerable<IPlatformViewFactory> factories
    )
    {
        var supplied = new PlatformViewFactoryRegistry(factories);
        var registered = new List<IPlatformViewFactory>();
        foreach (var registration in manifest.PlatformViews)
        {
            if (
                registration.Rid != manifest.TargetRid
                || supplied.Find(registration.ViewType) is not { } factory
            )
            {
                throw new DorotiCapabilityException(
                    DorotiCapabilityIds.PlatformViews,
                    null,
                    DorotiUiInvocation.Managed("application-platform-view:register"),
                    $"factory '{registration.ViewType}' is missing or targets a different RID",
                    manifest.TargetRid
                );
            }

            registered.Add(factory);
        }
        return new(registered);
    }

    /// <summary>Explicit optional registration. A coordinator must never be shared across owners.</summary>
    public PlatformViewCoordinator ConfigurePlatformViews(
        DorotiViewCapabilities capabilities,
        ulong ownerViewId,
        IPlatformViewDispatcher dispatcher
    )
    {
        var coordinator = new PlatformViewCoordinator(
            ownerViewId,
            capabilities.TargetIdentity,
            _platformViews,
            dispatcher
        );
        capabilities.Register<IPlatformViewHostCapability>(
            DorotiCapabilityIds.PlatformViews,
            coordinator
        );
        return coordinator;
    }

    public void Configure(DorotiViewCapabilities capabilities)
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        ArgumentNullException.ThrowIfNull(capabilities);
        var plugins = _plugins.CreateScope(capabilities);
        capabilities
            .Register<IApplicationResourceHostCapability>(
                DorotiCapabilityIds.ApplicationResources,
                _resources
            )
            .Register<IPlatformMessageHostCapability>(
                DorotiCapabilityIds.PlatformMessaging,
                plugins
            )
            .Register<IPlatformPluginHostCapability>(DorotiCapabilityIds.PlatformPlugins, plugins)
            .Register<IPlatformPluginEventsHostCapability>(DorotiCapabilityIds.PlatformPluginEvents, plugins);
    }

    public void Configure(
        DorotiViewCapabilities capabilities,
        IPlatformMessageHostCapability frameworkChannels
    )
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        ArgumentNullException.ThrowIfNull(capabilities);
        ArgumentNullException.ThrowIfNull(frameworkChannels);
        var plugins = _plugins.CreateScope(capabilities);
        capabilities
            .Register<IApplicationResourceHostCapability>(
                DorotiCapabilityIds.ApplicationResources,
                _resources
            )
            .Register<IPlatformMessageHostCapability>(
                DorotiCapabilityIds.PlatformMessaging,
                new RoutedPlatformMessageCapability(frameworkChannels, plugins)
            )
            .Register<IPlatformPluginHostCapability>(DorotiCapabilityIds.PlatformPlugins, plugins)
            .Register<IPlatformPluginEventsHostCapability>(DorotiCapabilityIds.PlatformPluginEvents, plugins);
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 0) _shared.Release();
    }

    private sealed class ApplicationResourceCapability
        : IApplicationResourceHostCapability,
            IDisposable
    {
        private readonly Assembly _assembly;
        private readonly Dictionary<string, DorotiEmbeddedResource> _resources;

        public ApplicationResourceCapability(DorotiApplicationManifest manifest, Assembly assembly)
        {
            _assembly = assembly;
            _resources = manifest.Resources.ToDictionary(item => item.Key, StringComparer.Ordinal);
            Resources = manifest
                .Resources.Select(item => new DorotiApplicationResource(
                    item.Key,
                    item.Kind,
                    item.FontFamily,
                    item.Locale,
                    item.Sha256,
                    item.Length
                ))
                .ToArray();
        }

        public IReadOnlyList<DorotiApplicationResource> Resources { get; }

        public async ValueTask<ReadOnlyMemory<byte>> LoadAsync(
            string key,
            CancellationToken cancellationToken = default
        )
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            if (!_resources.TryGetValue(key, out var resource))
            {
                throw new DorotiCapabilityException(
                    DorotiCapabilityIds.ApplicationResources,
                    null,
                    DorotiUiInvocation.Managed($"application-resource:{key}"),
                    "the generated resource manifest does not register this key"
                );
            }

            await using var stream =
                _assembly.GetManifestResourceStream(resource.EmbeddedResourceName)
                ?? throw new InvalidDataException(
                    $"Embedded application resource is missing: {resource.EmbeddedResourceName}"
                );
            using var buffer = new MemoryStream(checked((int)resource.Length));
            await stream.CopyToAsync(buffer, cancellationToken).ConfigureAwait(false);
            var bytes = buffer.ToArray();
            var hash = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
            if (bytes.LongLength != resource.Length || hash != resource.Sha256)
            {
                throw new InvalidDataException(
                    $"Embedded application resource integrity failed: {key}"
                );
            }

            return bytes;
        }

        public DorotiApplicationResource ResolveFont(string family) =>
            Resolve(
                item => item.Kind == "font" && item.FontFamily == family,
                $"font family '{family}'"
            );

        public DorotiApplicationResource ResolveLocalization(string locale) =>
            Resolve(
                item => item.Kind == "localization" && item.Locale == locale,
                $"locale '{locale}'"
            );

        private DorotiApplicationResource Resolve(
            Func<DorotiApplicationResource, bool> predicate,
            string description
        ) =>
            Resources.SingleOrDefault(predicate)
            ?? throw new DorotiCapabilityException(
                DorotiCapabilityIds.ApplicationResources,
                null,
                DorotiUiInvocation.Managed("application-resource:resolve"),
                $"the generated resource manifest does not register {description}"
            );

        public void Dispose() { }
    }

    private sealed class ApplicationPluginCapability
        : IPlatformMessageHostCapability,
            IPlatformPluginHostCapability,
            IDisposable
    {
        private readonly string _targetRid;
        private readonly List<PluginScope> _scopes = [];
        private bool _disposed;
        private int _activeCalls;
        private bool _handlersDisposed;
        private readonly Dictionary<
            string,
            (DorotiApplicationPlugin Descriptor, IDorotiNativePluginHandler Handler)
        > _handlers;

        public ApplicationPluginCapability(
            DorotiApplicationManifest manifest,
            IEnumerable<IDorotiNativePluginHandler> handlers
        )
        {
            _targetRid = manifest.TargetRid;
            var handlersById = new Dictionary<string, IDorotiNativePluginHandler>(StringComparer.Ordinal);
            foreach (var handler in handlers)
                if (!handlersById.TryAdd(handler.PluginId, handler))
                    throw Missing(handler.PluginId, $"duplicate native handler id '{handler.PluginId}'");
            _handlers = new(StringComparer.Ordinal);
            var ids = new HashSet<string>(StringComparer.Ordinal);
            foreach (var plugin in manifest.Plugins)
            {
                if (string.IsNullOrWhiteSpace(plugin.Id) || string.IsNullOrWhiteSpace(plugin.Channel) || string.IsNullOrWhiteSpace(plugin.Codec))
                    throw Missing(plugin.Channel ?? "<missing>", "plugin id, channel and codec must be nonempty");
                if (!ids.Add(plugin.Id) || _handlers.ContainsKey(plugin.Channel))
                    throw Missing(plugin.Channel, $"duplicate plugin id/channel '{plugin.Id}/{plugin.Channel}'");
                if (plugin.Channel.StartsWith("flutter/", StringComparison.Ordinal))
                    throw Missing(plugin.Channel, "the flutter/ channel prefix is reserved");
                if (plugin.NativePackage is null)
                {
                    throw Missing(
                        plugin.Channel,
                        $"plugin '{plugin.Id}' has no native package for RID '{manifest.TargetRid}'"
                    );
                }

                if (plugin.NativePackage.Rid != manifest.TargetRid)
                    throw Missing(plugin.Channel, $"plugin '{plugin.Id}' RID '{plugin.NativePackage.Rid}' does not match '{manifest.TargetRid}'");

                if (!handlersById.TryGetValue(plugin.Id, out var handler))
                {
                    throw Missing(
                        plugin.Channel,
                        $"native handler for plugin '{plugin.Id}' was not supplied"
                    );
                }

                if (handler.AbiVersion != plugin.NativePackage.AbiVersion)
                {
                    throw Missing(
                        plugin.Channel,
                        $"plugin '{plugin.Id}' ABI '{handler.AbiVersion}' does not match '{plugin.NativePackage.AbiVersion}'"
                    );
                }

                if (!(manifest.TargetRid == "browser-wasm" && plugin.NativePackage.HandlerType == "generated-js-registration")
                    && handler.GetType().FullName != plugin.NativePackage.HandlerType)
                    throw Missing(plugin.Channel, $"handler type '{handler.GetType().FullName}' does not match '{plugin.NativePackage.HandlerType}'");

                _handlers.Add(plugin.Channel, (plugin, handler));
            }
            foreach (var id in handlersById.Keys)
                if (!ids.Contains(id)) throw Missing(id, $"native handler '{id}' has no manifest descriptor");
        }

        public PluginScope CreateScope(DorotiViewCapabilities capabilities)
        {
            lock (_scopes)
            {
                ObjectDisposedException.ThrowIf(_disposed, this);
                var scope = new PluginScope(this, new(capabilities));
                _scopes.Add(scope);
                return scope;
            }
        }

        public IReadOnlyCollection<string> RegisteredChannels =>
            _handlers.Keys.Order(StringComparer.Ordinal).ToArray();

        public ValueTask<ReadOnlyMemory<byte>?> SendAsync(
            string channel,
            ReadOnlyMemory<byte>? data,
            CancellationToken cancellationToken = default
        )
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(channel);
            if (!_handlers.TryGetValue(channel, out var binding))
            {
                throw Missing(
                    channel,
                    "no generated plugin descriptor and native handler are registered"
                );
            }

            return binding.Handler.HandleAsync(
                channel,
                binding.Descriptor.Codec,
                data,
                cancellationToken
            );
        }

        public void SetMessageHandler(string channel, PlatformMessageHandler? handler) =>
            throw new NotSupportedException(
                "Native application plugin registrations are immutable."
            );

        public void Dispose()
        {
            PluginScope[] scopes;
            lock (_scopes)
            {
                if (_disposed) return;
                _disposed = true;
                scopes = _scopes.ToArray();
                _scopes.Clear();
            }
            List<Exception> errors = [];
            foreach (var scope in scopes)
                try { scope.Dispose(); } catch (Exception error) { errors.Add(error); }
            try { TryDisposeHandlers(); } catch (Exception error) { errors.Add(error); }
            if (errors.Count != 0) throw new AggregateException(errors);
        }

        private void TryDisposeHandlers()
        {
            lock (_scopes)
            {
                if (!_disposed || _activeCalls != 0 || _handlersDisposed) return;
                _handlersDisposed = true;
            }
            List<Exception> errors = [];
            foreach (
                var handler in _handlers
                    .Values.Select(item => item.Handler)
                    .Distinct(ReferenceEqualityComparer.Instance)
                    .OfType<IDisposable>()
            )
            {
                try { handler.Dispose(); } catch (Exception error) { errors.Add(error); }
            }

            // Keep immutable bindings available to already-running continuations.
            if (errors.Count != 0) throw new AggregateException(errors);
        }

        public sealed class PluginScope(ApplicationPluginCapability owner, DorotiPluginContext context)
            : IPlatformMessageHostCapability, IPlatformPluginHostCapability, IPlatformPluginEventsHostCapability, IDisposable
        {
            private readonly CancellationTokenSource _lifetime = new();
            private readonly object _gate = new();
            private bool _closed;
            public IReadOnlyCollection<string> RegisteredChannels => owner.RegisteredChannels;
            public IReadOnlyCollection<string> EventChannels => owner._handlers
                .Where(binding => binding.Value.Handler is IDorotiPluginEventHandler)
                .Select(binding => binding.Key).Order(StringComparer.Ordinal).ToArray();

            public async IAsyncEnumerable<ReadOnlyMemory<byte>> SubscribeAsync(string channel,
                ReadOnlyMemory<byte>? arguments = null, int capacity = 16,
                [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
            {
                if (capacity is < 1 or > 1024) throw new ArgumentOutOfRangeException(nameof(capacity));
                if (!owner._handlers.TryGetValue(channel, out var binding) || binding.Handler is not IDorotiPluginEventHandler events)
                    throw new NotSupportedException($"Plugin channel '{channel}' does not provide events.");
                var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _lifetime.Token);
                lock (_gate)
                {
                    if (_closed) { linked.Dispose(); throw new ObjectDisposedException(nameof(PluginScope)); }
                    lock (owner._scopes)
                    {
                        if (owner._disposed) { linked.Dispose(); throw new ObjectDisposedException(nameof(ApplicationPluginCapability)); }
                        owner._activeCalls++;
                    }
                }
                var queue = System.Threading.Channels.Channel.CreateBounded<ReadOnlyMemory<byte>>(new System.Threading.Channels.BoundedChannelOptions(capacity)
                {
                    FullMode = System.Threading.Channels.BoundedChannelFullMode.Wait,
                    SingleReader = true, SingleWriter = true, AllowSynchronousContinuations = false,
                });
                var token = linked.Token;
                // Producer owns the handler lease and CTS until native enumeration actually exits.
                _ = Produce();
                try
                {
                    await foreach (var item in queue.Reader.ReadAllAsync(token).ConfigureAwait(false))
                    {
                        token.ThrowIfCancellationRequested();
                        yield return item;
                    }
                }
                finally
                {
                    try { linked.Cancel(); } catch (ObjectDisposedException) { }
                }

                async Task Produce()
                {
                    Exception? failure = null;
                    try
                    {
                        token.ThrowIfCancellationRequested();
                        await foreach (var item in events.SubscribeAsync(context, channel, binding.Descriptor.Codec, arguments, token)
                            .WithCancellation(token).ConfigureAwait(false))
                        {
                            token.ThrowIfCancellationRequested();
                            if (item.Length > 65536) throw new InvalidDataException("Plugin event exceeds 64 KiB.");
                            await queue.Writer.WriteAsync(item.ToArray(), token).ConfigureAwait(false);
                        }
                    }
                    catch (Exception error) { failure = error; }
                    finally
                    {
                        lock (owner._scopes) { owner._activeCalls--; }
                        try { owner.TryDisposeHandlers(); }
                        catch (Exception error) { failure ??= error; }
                        finally { linked.Dispose(); }
                        queue.Writer.TryComplete(failure);
                    }
                }
            }

            public async ValueTask<ReadOnlyMemory<byte>?> SendAsync(string channel, ReadOnlyMemory<byte>? data,
                CancellationToken cancellationToken = default)
            {
                using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _lifetime.Token);
                lock (_gate)
                {
                    ObjectDisposedException.ThrowIf(_closed, this);
                    lock (owner._scopes)
                    {
                        ObjectDisposedException.ThrowIf(owner._disposed, owner);
                        owner._activeCalls++;
                    }
                }
                // Stop waiting even if an external handler ignores cancellation. Invoke still
                // owns the handler until it exits; the view context rejects late retained grants.
                return await Invoke(channel, data, linked.Token).WaitAsync(linked.Token).ConfigureAwait(false);
            }

            private async Task<ReadOnlyMemory<byte>?> Invoke(string channel, ReadOnlyMemory<byte>? data, CancellationToken token)
            {
                try
                {
                    token.ThrowIfCancellationRequested();
                    if (!owner._handlers.TryGetValue(channel, out var binding))
                        throw owner.Missing(channel, "no generated plugin descriptor and native handler are registered");
                    var result = binding.Handler is IDorotiViewPluginHandler viewHandler
                        ? await viewHandler.HandleAsync(context, channel, binding.Descriptor.Codec, data, token).ConfigureAwait(false)
                        : await binding.Handler.HandleAsync(channel, binding.Descriptor.Codec, data, token).ConfigureAwait(false);
                    token.ThrowIfCancellationRequested();
                    return result;
                }
                finally
                {
                    lock (owner._scopes) { owner._activeCalls--; }
                    owner.TryDisposeHandlers();
                }
            }

            public void SetMessageHandler(string channel, PlatformMessageHandler? handler) => owner.SetMessageHandler(channel, handler);

            public void Dispose()
            {
                lock (_gate) { if (_closed) return; _closed = true; }
                try { _lifetime.Cancel(); }
                finally
                {
                    try { context.Dispose(); }
                    finally { lock (owner._scopes) { owner._scopes.Remove(this); } }
                }
            }
        }

        private DorotiCapabilityException Missing(string channel, string reason) =>
            new(
                DorotiCapabilityIds.PlatformPlugins,
                null,
                DorotiUiInvocation.Managed($"platform-channel:{channel}"),
                reason,
                _targetRid
            );
    }

    private sealed class RoutedPlatformMessageCapability(
        IPlatformMessageHostCapability frameworkChannels,
        IPlatformMessageHostCapability applicationChannels
    ) : IPlatformMessageHostCapability
    {
        public ValueTask<ReadOnlyMemory<byte>?> SendAsync(
            string channel,
            ReadOnlyMemory<byte>? data,
            CancellationToken cancellationToken = default
        ) =>
            channel.StartsWith("flutter/", StringComparison.Ordinal)
                ? frameworkChannels.SendAsync(channel, data, cancellationToken)
                : applicationChannels.SendAsync(channel, data, cancellationToken);

        public void SetMessageHandler(string channel, PlatformMessageHandler? handler)
        {
            if (!channel.StartsWith("flutter/", StringComparison.Ordinal))
            {
                throw new NotSupportedException(
                    "Native application plugin registrations are immutable."
                );
            }
            frameworkChannels.SetMessageHandler(channel, handler);
        }
    }
}
