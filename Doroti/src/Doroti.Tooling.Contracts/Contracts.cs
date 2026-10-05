namespace Doroti.Tooling.Contracts;

public enum ToolService { Configuration, Diagnostics, Devices, Templates, Operations }
public enum ToolMode { DotnetInproc, ProcessStdio }
public enum DiagnosticStatus { Pass, Partial, Skipped, Fail }
public enum OptionKind { Text, Boolean, Integer, Choice }
public sealed record ToolIdentity(string ProviderId, string Version, int ProtocolVersion, string CoreRange, string Runtime, IReadOnlyList<string> HostOperatingSystems, IReadOnlyList<ToolService> Services);
public sealed record ToolContext(string Workspace, string Project, string ProviderId, string TargetId, string Profile, string? DeviceId, long Generation, string? TargetFramework = null, string? RuntimeIdentifier = null, string? Backend = null, string? ProviderManifest = null, IReadOnlyList<OptionValue>? Options = null);
public sealed record ToolOption(string Name, OptionKind Kind, string? Default, bool Required, IReadOnlyList<string> Choices, long? Minimum = null, long? Maximum = null);
public sealed record OptionValue(string Name, string Value);
public sealed record ToolConfiguration(IReadOnlyList<ToolOption> Options);
public sealed record ConfigurationRequest(ToolContext Context, IReadOnlyList<OptionValue> Values);
public sealed record ConfigurationResult(IReadOnlyList<OptionValue> Values);
public sealed record Diagnostic(string Id, DiagnosticStatus Status, string Message, string Scope);
public sealed record DiagnosticResult(IReadOnlyList<Diagnostic> Diagnostics);
public sealed record ToolDevice(string Id, string Name, IReadOnlyList<string> Targets, IReadOnlyList<string> Profiles);
public sealed record DeviceResult(IReadOnlyList<ToolDevice> Devices);
public sealed record ToolTemplate(string Id, string Version, IReadOnlyList<string> Designs, IReadOnlyList<ToolOption> Inputs);
public sealed record TemplateResult(IReadOnlyList<ToolTemplate> Templates);
public sealed record TemplateRequest(ToolContext Context, string TemplateId, string Design, string Destination, IReadOnlyList<OptionValue> Values);
public sealed record GeneratedFile(string RelativePath, string Content);
public sealed record TemplateGeneration(IReadOnlyList<GeneratedFile> Files);
public sealed record OperationRequest(ToolContext Context, string Operation, string Configuration, IReadOnlyList<OptionValue> Options);
public sealed record EnvironmentValue(string Name, string Value);
public sealed record ProcessStopSignal(string Path, string SessionId, int TimeoutSeconds = 30);
public sealed record ProcessStep(string Executable, IReadOnlyList<string> Arguments, string WorkingDirectory, IReadOnlyList<EnvironmentValue> Environment, ProcessStopSignal? StopSignal = null);
public sealed record ExecutionPlan(string OperationId, IReadOnlyList<ProcessStep> Steps);

/// <summary>Extensions evaluate plans and bounded probes; the CLI owns launched app/build processes.</summary>
public interface IDorotiToolExtension : IAsyncDisposable
{
    ValueTask<ToolIdentity> GetCapabilitiesAsync(CancellationToken cancellationToken);
    ValueTask<ToolConfiguration> GetConfigurationAsync(ToolContext context, CancellationToken cancellationToken);
    ValueTask<ConfigurationResult> ConfigureAsync(ConfigurationRequest request, CancellationToken cancellationToken);
    ValueTask<DiagnosticResult> DiagnoseAsync(ToolContext context, CancellationToken cancellationToken);
    ValueTask<DeviceResult> GetDevicesAsync(ToolContext context, CancellationToken cancellationToken);
    ValueTask<TemplateResult> GetTemplatesAsync(ToolContext context, CancellationToken cancellationToken);
    ValueTask<TemplateGeneration> GenerateAsync(TemplateRequest request, CancellationToken cancellationToken);
    ValueTask<ExecutionPlan> PlanAsync(OperationRequest request, CancellationToken cancellationToken);
}

public sealed class ToolContractException(string code, string message) : InvalidOperationException(message)
{
    public string Code { get; } = code;
}

public sealed record ToolEntry(ToolMode Mode, string Assembly, string EntryType, string? Executable, IReadOnlyList<string> Arguments, string? Project = null);
public sealed record DevelopmentCapability(string Transport, bool Debug, bool RequiresPreparedAcknowledgment, bool UsesStopSignal, bool LaunchBrowser);
public sealed record ProviderProfile(IReadOnlyList<string> HostOperatingSystems, IReadOnlyList<string> Operations, DevelopmentCapability Development);
public sealed record ProviderDescriptor(string SchemaVersion, string Id, string Version, string CoreRange, int ProtocolVersion, string RegistrationType, IReadOnlyList<string> HostOperatingSystems, IReadOnlyList<string> Operations, IReadOnlyList<ToolService> Services, ToolEntry Tool, IReadOnlyDictionary<string, ProviderProfile>? Profiles = null);
public sealed record WorkspacePlatform(string Provider, string ProviderManifest, string Target, string TargetPackage, string Runner, string Backend, string Profile, string TargetFramework, string RuntimeIdentifier);
public sealed record WorkspaceDescriptor(string SchemaVersion, string ApplicationProject, IReadOnlyDictionary<string, WorkspacePlatform> Platforms);

public static class ToolContract
{
    public const int ProtocolVersion = 1;
    public static readonly string HostCoreVersion = typeof(ToolContract).Assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyInformationalVersionAttribute), false).Cast<System.Reflection.AssemblyInformationalVersionAttribute>().Single().InformationalVersion.Split('+')[0];
    public static void Validate(ToolIdentity actual, ProviderDescriptor expected)
    {
        if (actual.ProviderId != expected.Id || actual.Version != expected.Version || actual.ProtocolVersion != ProtocolVersion || expected.ProtocolVersion != ProtocolVersion || actual.CoreRange != expected.CoreRange)
            throw new ToolContractException("identity-mismatch", "Tool handshake does not match the resolved provider descriptor.");
        if (!ToolVersionRange.Contains(expected.CoreRange, HostCoreVersion)) throw new ToolContractException("unsupported-core", $"Provider range {expected.CoreRange} excludes host core {HostCoreVersion}.");
        if (actual.Services is null || expected.Services is null || actual.Services.Any(service => !Enum.IsDefined(service))) throw new ToolContractException("invalid-services", "Handshake services must be declared typed values.");
        if (actual.Services.Distinct().Count() != actual.Services.Count || !actual.Services.Order().SequenceEqual(expected.Services.Order()))
            throw new ToolContractException("services-mismatch", "Handshake services must match the declared services without duplicates.");
        var os = OperatingSystem.IsWindows() ? "windows" : OperatingSystem.IsMacOS() ? "macos" : OperatingSystem.IsLinux() ? "linux" : "unknown";
        if (!actual.HostOperatingSystems.Contains(os, StringComparer.Ordinal) || !expected.HostOperatingSystems.Contains(os, StringComparer.Ordinal))
            throw new ToolContractException("unsupported-host", "The provider tool does not support this host.");
        if (actual.Runtime != "net10.0") throw new ToolContractException("runtime-mismatch", "This host supports the net10.0 tool contract runtime.");
    }
    public static void ValidateAlias(string alias)
    {
        if (string.IsNullOrEmpty(alias) || alias.Length > 64 || !char.IsAsciiLetter(alias[0]) || alias.Any(c => !char.IsAsciiLetterOrDigit(c) && c is not '-' and not '_'))
            throw new ToolContractException("invalid-alias", "Aliases use an ASCII letter followed by letters, digits, hyphens or underscores.");
    }
    public static void ValidateConfiguration(ToolConfiguration schema, IReadOnlyList<OptionValue> values)
    {
        if (schema.Options.Select(x => x.Name).Distinct(StringComparer.Ordinal).Count() != schema.Options.Count || values.Select(x => x.Name).Distinct(StringComparer.Ordinal).Count() != values.Count)
            throw new ToolContractException("duplicate-option", "Option names must be unique.");
        foreach (var value in values)
        {
            var option = schema.Options.SingleOrDefault(x => x.Name == value.Name) ?? throw new ToolContractException("unknown-option", value.Name);
            if (option.Kind == OptionKind.Boolean && value.Value is not ("true" or "false") || option.Kind == OptionKind.Choice && !option.Choices.Contains(value.Value, StringComparer.Ordinal))
                throw new ToolContractException("invalid-option", value.Name);
            if (option.Kind == OptionKind.Integer && (!long.TryParse(value.Value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var number) || option.Minimum is { } min && number < min || option.Maximum is { } max && number > max))
                throw new ToolContractException("invalid-option", value.Name);
        }
        foreach (var option in schema.Options)
            if (option.Required && option.Default is null && !values.Any(x => x.Name == option.Name)) throw new ToolContractException("missing-option", option.Name);
    }
}
