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
    Uri? BaseUri = null)
{
    public static DorotiLaunchContext Create(
        string target,
        string runtimeIdentifier,
        IEnumerable<string>? arguments = null,
        Uri? baseUri = null) => new(
            target,
            runtimeIdentifier,
            (arguments ?? []).ToArray(),
            baseUri);
}

/// <summary>Generated registration for a target adapter such as a browser JavaScript plugin.</summary>
public sealed record DorotiApplicationPluginRegistration(
    string Id,
    string Channel,
    string Adapter,
    string Module,
    string ExportName);

/// <summary>Immutable application definition shared by every native or browser host.</summary>
public sealed record DorotiApplicationDescriptor(
    Func<IDorotiViewEntrypoint> EntrypointFactory,
    Assembly ApplicationAssembly,
    DorotiViewConfiguration ViewConfiguration,
    DorotiLaunchContext LaunchContext,
    IReadOnlyList<DorotiApplicationPluginRegistration> PluginRegistrations);

public sealed class DorotiApplicationBuilder
{
    private readonly Assembly _applicationAssembly;
    private readonly DorotiLaunchContext _launchContext;
    private readonly List<DorotiApplicationPluginRegistration> _plugins = [];
    private Func<IDorotiViewEntrypoint>? _entrypointFactory;
    private DorotiViewConfiguration? _viewConfiguration;

    public DorotiApplicationBuilder(Assembly applicationAssembly, DorotiLaunchContext launchContext)
    {
        _applicationAssembly = applicationAssembly ?? throw new ArgumentNullException(nameof(applicationAssembly));
        _launchContext = launchContext ?? throw new ArgumentNullException(nameof(launchContext));
    }

    public DorotiApplicationBuilder UseEntrypoint(Func<IDorotiViewEntrypoint> entrypointFactory)
    {
        _entrypointFactory = entrypointFactory ?? throw new ArgumentNullException(nameof(entrypointFactory));
        return this;
    }

    public DorotiApplicationBuilder UseView(DorotiViewConfiguration configuration)
    {
        _viewConfiguration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        return this;
    }

    public DorotiApplicationBuilder AddPlugin(DorotiApplicationPluginRegistration registration)
    {
        ArgumentNullException.ThrowIfNull(registration);
        if (_plugins.Any(item => item.Id == registration.Id || item.Channel == registration.Channel))
            throw new InvalidOperationException($"Doroti plugin id/channel is duplicated: {registration.Id}/{registration.Channel}.");
        _plugins.Add(registration);
        return this;
    }

    public DorotiApplicationDescriptor Build()
    {
        if (_entrypointFactory is null)
            throw new InvalidOperationException($"{nameof(UseEntrypoint)} must be called by the Doroti startup.");
        if (_viewConfiguration is null)
            throw new InvalidOperationException($"{nameof(UseView)} must be called by the Doroti startup.");
        return new(
            _entrypointFactory,
            _applicationAssembly,
            _viewConfiguration,
            _launchContext,
            _plugins.ToArray());
    }
}

/// <summary>Strongly typed factory called only from SDK-owned generated bootstrap code.</summary>
public static class DorotiApplicationFactory
{
    public static DorotiApplicationDescriptor Create<TStartup>(
        DorotiLaunchContext launchContext,
        IEnumerable<DorotiApplicationPluginRegistration>? plugins = null)
        where TStartup : IDorotiApplicationStartup, new()
    {
        ArgumentNullException.ThrowIfNull(launchContext);
        var builder = new DorotiApplicationBuilder(typeof(TStartup).Assembly, launchContext);
        new TStartup().Configure(builder);
        foreach (var plugin in plugins ?? []) builder.AddPlugin(plugin);
        return builder.Build();
    }
}
