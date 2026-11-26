using System.Text;
using System.Text.Json;
using Doroti.Runtime;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Doroti.Localization.Generator;

[Generator(LanguageNames.CSharp)]
public sealed class DorotiLocalizationGenerator : IIncrementalGenerator
{
    private static readonly DiagnosticDescriptor InvalidArb = new(
        "DLOC001", "Invalid Doroti localization resource", "{0}", "Localization", DiagnosticSeverity.Error, true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var files = context.AdditionalTextsProvider
            .Where(file => Path.GetExtension(file.Path).Equals(".arb", StringComparison.OrdinalIgnoreCase))
            .Select(ArbSource.Read).Collect();
        var settings = context.AnalyzerConfigOptionsProvider.Select((options, _) => Settings.Read(options.GlobalOptions));
        context.RegisterSourceOutput(files.Combine(settings), static (output, input) =>
            Generate(output, input.Left, input.Right));
    }

    private static void Generate(SourceProductionContext output, IReadOnlyList<ArbSource> parsed, Settings settings)
    {
        if (parsed.Count == 0) return;
        var sources = parsed.Select(source => source.ForValidation()).ToArray();
        if (!ArbBundle.IsIdentifier(settings.ClassName) || settings.ClassName.StartsWith("__Doroti", StringComparison.Ordinal)
            || !settings.Namespace.Split('.').All(ArbBundle.IsIdentifier))
        {
            Report(sources[0].At(null), "Use valid C# identifiers for DorotiLocalizationClass and DorotiLocalizationNamespace.");
            return;
        }
        if (!LocaleTag.TryParse(settings.TemplateLocale, out var templateLocale))
        {
            Report(sources[0].At(null), "DorotiLocalizationTemplate must be a supported language tag.");
            return;
        }

        var requested = new HashSet<LocaleTag> { templateLocale };
        if (settings.LocaleFilter is { } filter)
        {
            foreach (var value in filter.Split([',', ';'], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            {
                if (!LocaleTag.TryParse(value, out var selected))
                {
                    Report(sources[0].At(null), $"Invalid tag '{value}' in DorotiLocalizationLocales.");
                    return;
                }
                requested.Add(selected);
            }
        }

        var bundles = new Dictionary<LocaleTag, ArbBundle>();
        foreach (var source in sources.OrderBy(source => source.Path, StringComparer.Ordinal))
        {
            output.CancellationToken.ThrowIfCancellationRequested();
            if (source.Problems.Count != 0) continue;
            if (!TryGetLocale(source, out var locale))
            {
                source.Error("@@locale", "Declare a language[-Script][-Region] tag in '@@locale'.");
                continue;
            }
            if (!bundles.TryAdd(locale, new(source, locale)))
                source.Error("@@locale", $"Duplicate normalized locale '{locale}'.");
        }

        if (settings.LocaleFilter is not null)
        {
            foreach (var missing in requested.Where(locale => !bundles.ContainsKey(locale)))
                sources[0].Error(null, $"No ARB declares the selected locale '{missing}'.");
            var excluded = bundles.Keys.Where(locale => !requested.Contains(locale)).ToArray();
            foreach (var locale in excluded) bundles.Remove(locale);
        }

        if (bundles.TryGetValue(templateLocale, out var template))
        {
            template.ReadMessages(template, settings.ClassName);
            if (template.Messages.Count == 0 && template.Source.Problems.Count == 0)
                template.Source.Error("@@locale", "The template must contain at least one message.");
            foreach (var bundle in bundles.Values.Where(bundle => !ReferenceEquals(bundle, template)))
            {
                output.CancellationToken.ThrowIfCancellationRequested();
                bundle.ReadMessages(template, settings.ClassName);
            }
        }
        else if (sources.All(source => source.Problems.Count == 0))
        {
            Report(sources[0].At(null), $"No ARB declares the template locale '{templateLocale}'.");
        }

        var errors = sources.SelectMany(source => source.Problems).ToArray();
        foreach (var error in errors) Report(error.Location, error.Message);
        if (template is null || errors.Length != 0) return;

        var ordered = bundles.Values.OrderBy(bundle => !ReferenceEquals(bundle, template))
            .ThenBy(bundle => bundle.Locale.ToString(), StringComparer.Ordinal).ToArray();
        output.AddSource(settings.ClassName + ".Localization.g.cs", SourceText.From(
            LocalizationEmitter.Emit(settings.Namespace, settings.ClassName, template, ordered), Encoding.UTF8));

        void Report(Location location, string message) =>
            output.ReportDiagnostic(Diagnostic.Create(InvalidArb, location, message));
    }

    private static bool TryGetLocale(ArbSource source, out LocaleTag locale)
    {
        locale = default;
        try
        {
            return source.Properties.TryGetValue("@@locale", out var value) && value.ValueKind == JsonValueKind.String
                && LocaleTag.TryParse(value.GetString(), out locale);
        }
        catch (InvalidOperationException) { return false; }
    }

    private sealed record Settings(string Namespace, string ClassName, string TemplateLocale, string? LocaleFilter)
    {
        internal static Settings Read(AnalyzerConfigOptions options) => new(
            Get(options, "DorotiLocalizationNamespace") ?? Get(options, "RootNamespace") ?? "Doroti.Generated",
            Get(options, "DorotiLocalizationClass") ?? "AppLocalizations",
            Get(options, "DorotiLocalizationTemplate") ?? "en",
            Get(options, "DorotiLocalizationLocales"));

        private static string? Get(AnalyzerConfigOptions options, string name) =>
            options.TryGetValue("build_property." + name, out var option) && !string.IsNullOrWhiteSpace(option)
                ? option.Trim() : null;

    }
}
