using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
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
        CancellationToken cancellationToken = default);
}

public sealed record DorotiNativePluginPackage(
    string Rid,
    string PackageId,
    string Version,
    string AbiVersion,
    string HandlerType);

public sealed record DorotiApplicationPlugin(
    string Id,
    string Channel,
    string Codec,
    string CapabilityId,
    DorotiNativePluginPackage NativePackage);

public sealed record DorotiApplicationManifest(
    string SchemaVersion,
    string ApplicationId,
    string TargetRid,
    DorotiEmbeddedResource[] Resources,
    DorotiApplicationPlugin[] Plugins);

public sealed record DorotiEmbeddedResource(
    string Key,
    string Kind,
    string? FontFamily,
    string? Locale,
    string EmbeddedResourceName,
    string Sha256,
    long Length);

/// <summary>Compiler-produced application resource and plugin boundary for one target RID.</summary>
public sealed class DorotiApplicationBoundary : IDisposable
{
    private readonly ApplicationResourceCapability _resources;
    private readonly ApplicationPluginCapability _plugins;

    private DorotiApplicationBoundary(
        DorotiApplicationManifest manifest,
        Assembly assembly,
        IEnumerable<IDorotiNativePluginHandler> handlers)
    {
        Manifest = manifest;
        _resources = new(manifest, assembly);
        _plugins = new(manifest, handlers);
    }

    public DorotiApplicationManifest Manifest { get; }

    public static DorotiApplicationBoundary Load(
        Assembly applicationAssembly,
        string targetRid,
        IEnumerable<IDorotiNativePluginHandler>? handlers = null) =>
        Load(applicationAssembly, applicationAssembly, targetRid, handlers);

    public static DorotiApplicationBoundary Load(
        Assembly manifestAssembly,
        Assembly applicationAssembly,
        string targetRid,
        IEnumerable<IDorotiNativePluginHandler>? handlers = null)
    {
        ArgumentNullException.ThrowIfNull(manifestAssembly);
        ArgumentNullException.ThrowIfNull(applicationAssembly);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetRid);
        using var stream = manifestAssembly.GetManifestResourceStream("Doroti.Application.Manifest")
            ?? throw new InvalidDataException("Doroti runner assembly is missing Doroti.Application.Manifest.");
        var manifest = JsonSerializer.Deserialize<DorotiApplicationManifest>(stream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        }) ?? throw new InvalidDataException("Generated application manifest is empty.");
        if (manifest.SchemaVersion != "doroti.application-capabilities/v1")
            throw new InvalidDataException($"Unsupported generated application manifest: {manifest.SchemaVersion}");
        if (manifest.TargetRid != targetRid)
            throw new DorotiCapabilityException(
                DorotiCapabilityIds.PlatformPlugins,
                null,
                DartUiInvocation.Managed("Doroti.Hosting#LoadApplication"),
                $"application targets RID '{manifest.TargetRid}', not '{targetRid}'",
                targetRid);
        return new(manifest, applicationAssembly, handlers ?? []);
    }

    public void Configure(DorotiViewCapabilities capabilities)
    {
        ArgumentNullException.ThrowIfNull(capabilities);
        capabilities
            .Register<IApplicationResourceHostCapability>(DorotiCapabilityIds.ApplicationResources, _resources)
            .Register<IPlatformMessageHostCapability>(DorotiCapabilityIds.PlatformMessaging, _plugins)
            .Register<IPlatformPluginHostCapability>(DorotiCapabilityIds.PlatformPlugins, _plugins);
    }

    public void Configure(DorotiViewCapabilities capabilities, IPlatformMessageHostCapability frameworkChannels)
    {
        ArgumentNullException.ThrowIfNull(capabilities);
        ArgumentNullException.ThrowIfNull(frameworkChannels);
        capabilities
            .Register<IApplicationResourceHostCapability>(DorotiCapabilityIds.ApplicationResources, _resources)
            .Register<IPlatformMessageHostCapability>(DorotiCapabilityIds.PlatformMessaging,
                new RoutedPlatformMessageCapability(frameworkChannels, _plugins))
            .Register<IPlatformPluginHostCapability>(DorotiCapabilityIds.PlatformPlugins, _plugins);
    }

    public void Dispose()
    {
        _resources.Dispose();
        _plugins.Dispose();
    }

    private sealed class ApplicationResourceCapability : IApplicationResourceHostCapability, IDisposable
    {
        private readonly Assembly _assembly;
        private readonly Dictionary<string, DorotiEmbeddedResource> _resources;

        public ApplicationResourceCapability(DorotiApplicationManifest manifest, Assembly assembly)
        {
            _assembly = assembly;
            _resources = manifest.Resources.ToDictionary(item => item.Key, StringComparer.Ordinal);
            Resources = manifest.Resources.Select(item => new DorotiApplicationResource(
                item.Key, item.Kind, item.FontFamily, item.Locale, item.Sha256, item.Length)).ToArray();
        }

        public IReadOnlyList<DorotiApplicationResource> Resources { get; }

        public async ValueTask<ReadOnlyMemory<byte>> LoadAsync(string key, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            if (!_resources.TryGetValue(key, out var resource))
                throw new DorotiCapabilityException(
                    DorotiCapabilityIds.ApplicationResources,
                    null,
                    DartUiInvocation.Managed($"application-resource:{key}"),
                    "the generated resource manifest does not register this key");
            await using var stream = _assembly.GetManifestResourceStream(resource.EmbeddedResourceName)
                ?? throw new InvalidDataException($"Embedded application resource is missing: {resource.EmbeddedResourceName}");
            using var buffer = new MemoryStream(checked((int)resource.Length));
            await stream.CopyToAsync(buffer, cancellationToken).ConfigureAwait(false);
            var bytes = buffer.ToArray();
            var hash = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
            if (bytes.LongLength != resource.Length || hash != resource.Sha256)
                throw new InvalidDataException($"Embedded application resource integrity failed: {key}");
            return bytes;
        }

        public DorotiApplicationResource ResolveFont(string family) => Resolve(
            item => item.Kind == "font" && item.FontFamily == family,
            $"font family '{family}'");

        public DorotiApplicationResource ResolveLocalization(string locale) => Resolve(
            item => item.Kind == "localization" && item.Locale == locale,
            $"locale '{locale}'");

        private DorotiApplicationResource Resolve(Func<DorotiApplicationResource, bool> predicate, string description) =>
            Resources.SingleOrDefault(predicate) ?? throw new DorotiCapabilityException(
                DorotiCapabilityIds.ApplicationResources,
                null,
                DartUiInvocation.Managed("application-resource:resolve"),
                $"the generated resource manifest does not register {description}");

        public void Dispose() { }
    }

    private sealed class ApplicationPluginCapability : IPlatformMessageHostCapability, IPlatformPluginHostCapability, IDisposable
    {
        private readonly string _targetRid;
        private readonly Dictionary<string, (DorotiApplicationPlugin Descriptor, IDorotiNativePluginHandler Handler)> _handlers;

        public ApplicationPluginCapability(DorotiApplicationManifest manifest, IEnumerable<IDorotiNativePluginHandler> handlers)
        {
            _targetRid = manifest.TargetRid;
            var handlersById = handlers.ToDictionary(item => item.PluginId, StringComparer.Ordinal);
            _handlers = new(StringComparer.Ordinal);
            foreach (var plugin in manifest.Plugins)
            {
                if (plugin.NativePackage is null)
                    throw Missing(plugin.Channel, $"plugin '{plugin.Id}' has no native package for RID '{manifest.TargetRid}'");
                if (!handlersById.TryGetValue(plugin.Id, out var handler))
                    throw Missing(plugin.Channel, $"native handler for plugin '{plugin.Id}' was not supplied");
                if (handler.AbiVersion != plugin.NativePackage.AbiVersion)
                    throw Missing(plugin.Channel, $"plugin '{plugin.Id}' ABI '{handler.AbiVersion}' does not match '{plugin.NativePackage.AbiVersion}'");
                _handlers.Add(plugin.Channel, (plugin, handler));
            }
        }

        public IReadOnlyCollection<string> RegisteredChannels => _handlers.Keys.Order(StringComparer.Ordinal).ToArray();

        public ValueTask<ReadOnlyMemory<byte>?> SendAsync(
            string channel,
            ReadOnlyMemory<byte>? data,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(channel);
            if (!_handlers.TryGetValue(channel, out var binding))
                throw Missing(channel, "no generated plugin descriptor and native handler are registered");
            return binding.Handler.HandleAsync(channel, binding.Descriptor.Codec, data, cancellationToken);
        }

        public void SetMessageHandler(string channel, PlatformMessageHandler? handler) =>
            throw new NotSupportedException("Native application plugin registrations are immutable.");

        public void Dispose()
        {
            foreach (var handler in _handlers.Values.Select(item => item.Handler).Distinct(ReferenceEqualityComparer.Instance).OfType<IDisposable>())
                handler.Dispose();
            _handlers.Clear();
        }

        private DorotiCapabilityException Missing(string channel, string reason) => new(
            DorotiCapabilityIds.PlatformPlugins,
            null,
            DartUiInvocation.Managed($"platform-channel:{channel}"),
            reason,
            _targetRid);
    }

    private sealed class RoutedPlatformMessageCapability(
        IPlatformMessageHostCapability frameworkChannels,
        IPlatformMessageHostCapability applicationChannels) : IPlatformMessageHostCapability
    {
        public ValueTask<ReadOnlyMemory<byte>?> SendAsync(
            string channel,
            ReadOnlyMemory<byte>? data,
            CancellationToken cancellationToken = default) =>
            channel.StartsWith("flutter/", StringComparison.Ordinal)
                ? frameworkChannels.SendAsync(channel, data, cancellationToken)
                : applicationChannels.SendAsync(channel, data, cancellationToken);

        public void SetMessageHandler(string channel, PlatformMessageHandler? handler)
        {
            if (!channel.StartsWith("flutter/", StringComparison.Ordinal))
            {
                throw new NotSupportedException("Native application plugin registrations are immutable.");
            }
            frameworkChannels.SetMessageHandler(channel, handler);
        }
    }
}
