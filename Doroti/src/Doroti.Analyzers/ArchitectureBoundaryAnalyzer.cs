using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Doroti.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ArchitectureBoundaryAnalyzer : DiagnosticAnalyzer
{
    public const string ForbiddenReferenceId = "DOTARCH001";
    public const string PublicLeakId = "DOTARCH002";
    public const string MigrationReferenceId = "DOTARCH003";
    public const string BackendWidgetsReferenceId = "DOTARCH004";
    public const string ForbiddenJsonDependencyId = "DOTARCH005";
    public const string UnauthorizedVendorReferenceId = "DOTARCH006";
    public const string VendorLayerReferenceId = "DOTARCH007";
    public const string UnauthorizedAvaloniaReferenceId = "DOTARCH008";
    public const string ForbiddenLayerReferenceId = "DOTARCH009";

    private static readonly DiagnosticDescriptor ForbiddenReference = new(
        ForbiddenReferenceId,
        "Framework layer uses a backend dependency",
        "Layer '{0}' must not reference backend namespace '{1}'",
        "Architecture",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor PublicLeak = new(
        PublicLeakId,
        "Public API exposes a backend type",
        "Public API '{0}' exposes backend type '{1}'",
        "Architecture",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor MigrationReference = new(
        MigrationReferenceId,
        "Production assembly references migration tooling",
        "Production assembly '{0}' must not reference migration/tool assembly '{1}'",
        "Architecture",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: WellKnownDiagnosticTags.CompilationEnd);

    private static readonly DiagnosticDescriptor BackendWidgetsReference = new(
        BackendWidgetsReferenceId,
        "Backend references Widgets",
        "Backend assembly '{0}' must not reference Doroti.Widgets",
        "Architecture",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: WellKnownDiagnosticTags.CompilationEnd);

    private static readonly DiagnosticDescriptor ForbiddenJsonDependency = new(
        ForbiddenJsonDependencyId,
        "Production uses an unsupported JSON library",
        "Production assembly '{0}' must use System.Text.Json instead of '{1}'",
        "Architecture",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor UnauthorizedVendorReference = new(
        UnauthorizedVendorReferenceId,
        "Assembly references an internal vendor implementation",
        "Assembly '{0}' is not allowed to reference vendor assembly '{1}'",
        "Architecture",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: WellKnownDiagnosticTags.CompilationEnd);

    private static readonly DiagnosticDescriptor VendorLayerReference = new(
        VendorLayerReferenceId,
        "Vendor assembly references an owned framework layer",
        "Vendor assembly '{0}' must not reference framework assembly '{1}'",
        "Architecture",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: WellKnownDiagnosticTags.CompilationEnd);

    private static readonly DiagnosticDescriptor UnauthorizedAvaloniaReference = new(
        UnauthorizedAvaloniaReferenceId,
        "Assembly references an Avalonia binary after product cutover",
        "Assembly '{0}' must not reference Avalonia binary assembly '{1}' after C0 product cutover",
        "Architecture",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: WellKnownDiagnosticTags.CompilationEnd);

    private static readonly DiagnosticDescriptor ForbiddenLayerReference = new(
        ForbiddenLayerReferenceId,
        "Assembly crosses the Flutter/Avalonia ownership boundary",
        "Assembly '{0}' must not reference layer assembly '{1}' under the G4 boundary",
        "Architecture",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: WellKnownDiagnosticTags.CompilationEnd);

    private static readonly string[] ProtectedLayers =
    {
        "Doroti.Core",
        "Doroti.Platform",
        "Doroti.Graphics",
        "Doroti.Composition",
        "Doroti.Rendering",
        "Doroti.Widgets",
        "Doroti.Runtime",
        "Doroti.Ui",
        "Doroti.Framework",
        "Doroti.Hosting",
        "Doroti.Engine",
        "Doroti",
    };

    private static readonly string[] BackendNamespaces =
    {
        "Avalonia",
        "Doroti.Vendor.Avalonia",
        "SkiaSharp",
        "System.Runtime.InteropServices",
        "Windows.Win32",
    };

    private static readonly string[] MigrationAssemblies =
    {
        "Doroti.SourceTools",
        "Doroti.DartToCSharp",
        "Doroti.SceneLab",
        "Doroti.Migration",
    };

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(
            ForbiddenReference,
            PublicLeak,
            MigrationReference,
            BackendWidgetsReference,
            ForbiddenJsonDependency,
            UnauthorizedVendorReference,
            VendorLayerReference,
            UnauthorizedAvaloniaReference,
            ForbiddenLayerReference);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(RegisterCompilation);
    }

    private static void RegisterCompilation(CompilationStartAnalysisContext context)
    {
        var assemblyName = context.Compilation.AssemblyName ?? string.Empty;
        var isProtected = ProtectedLayers.Any(layer =>
            string.Equals(assemblyName, layer, StringComparison.Ordinal) ||
            (!string.Equals(layer, "Doroti", StringComparison.Ordinal) && assemblyName.StartsWith(layer + ".", StringComparison.Ordinal)));
        var isBackend = assemblyName.StartsWith("Doroti.Backends.", StringComparison.Ordinal);
        var isVendor = assemblyName.StartsWith("Doroti.Vendor.Avalonia.", StringComparison.Ordinal);
        var isPlatformHost = string.Equals(assemblyName, "Doroti.Host.Desktop", StringComparison.Ordinal);
        var isProduction = string.Equals(assemblyName, "Doroti", StringComparison.Ordinal) ||
            assemblyName.StartsWith("Doroti.", StringComparison.Ordinal);

        if (isProtected)
        {
            context.RegisterSyntaxNodeAction(
                syntaxContext => AnalyzeUsing(syntaxContext, assemblyName),
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.UsingDirective);
            context.RegisterSyntaxNodeAction(
                syntaxContext => AnalyzeTypeUse(syntaxContext, assemblyName),
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.IdentifierName,
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.GenericName);
            context.RegisterSymbolAction(
                symbolContext => AnalyzeDeclaredSymbol(symbolContext, assemblyName),
                SymbolKind.NamedType,
                SymbolKind.Method,
                SymbolKind.Property,
                SymbolKind.Field,
                SymbolKind.Event);
        }

        if (isPlatformHost)
        {
            context.RegisterSymbolAction(
                symbolContext => AnalyzeHostDeclaredSymbol(symbolContext),
                SymbolKind.NamedType,
                SymbolKind.Method,
                SymbolKind.Property,
                SymbolKind.Field,
                SymbolKind.Event);
        }

        if (isProduction)
        {
            context.RegisterCompilationEndAction(endContext => AnalyzeReferences(endContext, assemblyName, isBackend, isVendor));
        }
    }

    private static void AnalyzeUsing(SyntaxNodeAnalysisContext context, string assemblyName)
    {
        var directive = (UsingDirectiveSyntax)context.Node;
        var namespaceName = directive.Name?.ToString() ?? string.Empty;
        if (string.Equals(namespaceName, "Newtonsoft.Json", StringComparison.Ordinal) ||
            namespaceName.StartsWith("Newtonsoft.Json.", StringComparison.Ordinal))
        {
            context.ReportDiagnostic(Diagnostic.Create(ForbiddenJsonDependency, directive.GetLocation(), assemblyName, namespaceName));
        }
        var forbidden = BackendNamespaces.FirstOrDefault(prefix =>
            string.Equals(namespaceName, prefix, StringComparison.Ordinal) ||
            namespaceName.StartsWith(prefix + ".", StringComparison.Ordinal));
        if (forbidden is not null)
        {
            context.ReportDiagnostic(Diagnostic.Create(ForbiddenReference, directive.GetLocation(), assemblyName, namespaceName));
        }
    }

    private static void AnalyzeTypeUse(SyntaxNodeAnalysisContext context, string assemblyName)
    {
        if (context.Node.AncestorsAndSelf().Any(node => node is UsingDirectiveSyntax))
        {
            return;
        }

        // Protected Flutter assemblies contain very large generated/reviewed
        // expression trees. Asking the semantic model about every identifier
        // made a cold Material compile take many minutes even though Material
        // does not reference a backend assembly. Imported backend namespaces
        // are rejected by AnalyzeUsing; this action only needs semantic work
        // for an explicitly qualified backend namespace.
        if (!CouldBeQualifiedBackendTypeUse(context.Node))
        {
            return;
        }

        if (context.Node.Parent is QualifiedNameSyntax qualified && qualified.Left == context.Node)
        {
            return;
        }

        var symbol = context.SemanticModel.GetSymbolInfo(context.Node, context.CancellationToken).Symbol;
        var type = symbol switch
        {
            INamedTypeSymbol namedType => namedType,
            IMethodSymbol method => method.ContainingType,
            IPropertySymbol property => property.Type,
            IFieldSymbol field => field.Type,
            IEventSymbol eventSymbol => eventSymbol.Type,
            ILocalSymbol local => local.Type,
            IParameterSymbol parameter => parameter.Type,
            IAliasSymbol alias => alias.Target as ITypeSymbol,
            _ => context.SemanticModel.GetTypeInfo(context.Node, context.CancellationToken).Type,
        };
        if (type is not null && IsBackendType(type))
        {
            context.ReportDiagnostic(Diagnostic.Create(ForbiddenReference, context.Node.GetLocation(), assemblyName, type.ToDisplayString()));
        }

        var namespaceName = GetNamespace(symbol);
        if (IsNewtonsoftNamespace(namespaceName))
        {
            context.ReportDiagnostic(Diagnostic.Create(ForbiddenJsonDependency, context.Node.GetLocation(), assemblyName, namespaceName));
        }
    }

    private static bool CouldBeQualifiedBackendTypeUse(SyntaxNode node)
    {
        var name = node;
        while (name.Parent is NameSyntax)
        {
            name = name.Parent;
        }

        var qualifiedName = name.ToString();
        const string globalPrefix = "global::";
        if (qualifiedName.StartsWith(globalPrefix, StringComparison.Ordinal))
        {
            qualifiedName = qualifiedName.Substring(globalPrefix.Length);
        }

        return BackendNamespaces.Any(prefix =>
            string.Equals(qualifiedName, prefix, StringComparison.Ordinal) ||
            qualifiedName.StartsWith(prefix + ".", StringComparison.Ordinal));
    }

    private static void AnalyzeDeclaredSymbol(SymbolAnalysisContext context, string assemblyName)
    {
        var symbol = context.Symbol;
        foreach (var type in GetExposedTypes(symbol))
        {
            if (type is null || !IsBackendType(type))
            {
                continue;
            }

            var location = symbol.Locations.FirstOrDefault() ?? Location.None;
            context.ReportDiagnostic(Diagnostic.Create(ForbiddenReference, location, assemblyName, type.ToDisplayString()));
            if (IsPublicApi(symbol))
            {
                context.ReportDiagnostic(Diagnostic.Create(PublicLeak, location, symbol.ToDisplayString(), type.ToDisplayString()));
            }
        }
    }

    private static void AnalyzeHostDeclaredSymbol(SymbolAnalysisContext context)
    {
        var symbol = context.Symbol;
        if (!IsPublicApi(symbol))
        {
            return;
        }

        foreach (var type in GetExposedTypes(symbol))
        {
            if (type is null || !IsBackendType(type))
            {
                continue;
            }

            var location = symbol.Locations.FirstOrDefault() ?? Location.None;
            context.ReportDiagnostic(Diagnostic.Create(PublicLeak, location, symbol.ToDisplayString(), type.ToDisplayString()));
        }
    }

    private static void AnalyzeReferences(
        CompilationAnalysisContext context,
        string assemblyName,
        bool isBackend,
        bool isVendor)
    {
        foreach (var reference in context.Compilation.ReferencedAssemblyNames)
        {
            if (MigrationAssemblies.Any(prefix =>
                    string.Equals(reference.Name, prefix, StringComparison.Ordinal) ||
                    reference.Name.StartsWith(prefix + ".", StringComparison.Ordinal)))
            {
                context.ReportDiagnostic(Diagnostic.Create(MigrationReference, Location.None, assemblyName, reference.Name));
            }

            if (isBackend && string.Equals(reference.Name, "Doroti.Widgets", StringComparison.Ordinal))
            {
                context.ReportDiagnostic(Diagnostic.Create(BackendWidgetsReference, Location.None, assemblyName));
            }

            if (reference.Name.StartsWith("Doroti.Vendor.Avalonia.", StringComparison.Ordinal) &&
                !IsAllowedVendorReference(assemblyName, reference.Name))
            {
                context.ReportDiagnostic(Diagnostic.Create(UnauthorizedVendorReference, Location.None, assemblyName, reference.Name));
            }

            if (isVendor && IsForbiddenVendorLayer(reference.Name))
            {
                context.ReportDiagnostic(Diagnostic.Create(VendorLayerReference, Location.None, assemblyName, reference.Name));
            }

            if (IsOfficialAvaloniaAssembly(reference.Name))
            {
                context.ReportDiagnostic(Diagnostic.Create(UnauthorizedAvaloniaReference, Location.None, assemblyName, reference.Name));
            }

            if (IsForbiddenG4LayerReference(assemblyName, reference.Name))
            {
                context.ReportDiagnostic(Diagnostic.Create(ForbiddenLayerReference, Location.None, assemblyName, reference.Name));
            }
        }
    }

    private static bool IsForbiddenG4LayerReference(string assemblyName, string referenceName)
    {
        if (string.Equals(assemblyName, "Doroti.Runtime", StringComparison.Ordinal))
        {
            return IsDorotiHostLayer(referenceName);
        }

        if (assemblyName.StartsWith("Doroti.Runtime.", StringComparison.Ordinal))
        {
            return IsDorotiHostLayer(referenceName);
        }

        if (string.Equals(assemblyName, "Doroti.Ui", StringComparison.Ordinal) ||
            assemblyName.StartsWith("Doroti.Ui.", StringComparison.Ordinal))
        {
            return IsDorotiHostLayer(referenceName) && !string.Equals(referenceName, "Doroti.Runtime", StringComparison.Ordinal);
        }

        if (assemblyName.StartsWith("Doroti.Framework", StringComparison.Ordinal))
        {
            return IsDorotiHostLayer(referenceName) &&
                !referenceName.StartsWith("Doroti.Framework", StringComparison.Ordinal) &&
                !string.Equals(referenceName, "Doroti.Runtime", StringComparison.Ordinal) &&
                !string.Equals(referenceName, "Doroti.Ui", StringComparison.Ordinal);
        }

        if (string.Equals(assemblyName, "Doroti.Hosting", StringComparison.Ordinal) ||
            assemblyName.StartsWith("Doroti.Hosting.", StringComparison.Ordinal))
        {
            return referenceName.StartsWith("Doroti.Host.", StringComparison.Ordinal) ||
                referenceName.StartsWith("Doroti.Shell.", StringComparison.Ordinal) ||
                referenceName.StartsWith("Doroti.Vendor.", StringComparison.Ordinal) ||
                referenceName.StartsWith("Doroti.Backends.", StringComparison.Ordinal) ||
                referenceName is "Doroti.Platform" or "Doroti.Graphics" or "Doroti.Composition" or "Doroti.Engine";
        }

        return false;
    }

    private static bool IsDorotiHostLayer(string assemblyName) =>
        assemblyName.StartsWith("Doroti.Host.", StringComparison.Ordinal) ||
        assemblyName.StartsWith("Doroti.Shell.", StringComparison.Ordinal) ||
        assemblyName.StartsWith("Doroti.Vendor.", StringComparison.Ordinal) ||
        assemblyName.StartsWith("Doroti.Backends.", StringComparison.Ordinal) ||
        string.Equals(assemblyName, "SkiaSharp", StringComparison.Ordinal) ||
        assemblyName.StartsWith("SkiaSharp.", StringComparison.Ordinal) ||
        string.Equals(assemblyName, "Windows.Win32", StringComparison.Ordinal) ||
        assemblyName is
            "Doroti.Core" or
            "Doroti.Platform" or
            "Doroti.Graphics" or
            "Doroti.Composition" or
            "Doroti.Rendering" or
            "Doroti.Widgets" or
            "Doroti.Engine";

    private static bool IsOfficialAvaloniaAssembly(string assemblyName) =>
        string.Equals(assemblyName, "Avalonia", StringComparison.Ordinal) ||
        assemblyName.StartsWith("Avalonia.", StringComparison.Ordinal);

    private static bool IsAllowedVendorReference(string assemblyName, string referenceName) =>
        (string.Equals(assemblyName, "Doroti.Backends.Skia", StringComparison.Ordinal) &&
         string.Equals(referenceName, "Doroti.Vendor.Avalonia.Skia", StringComparison.Ordinal)) ||
        (string.Equals(assemblyName, "Doroti.Host.Desktop", StringComparison.Ordinal) &&
         referenceName is "Doroti.Vendor.Avalonia.Base" or "Doroti.Vendor.Avalonia.Skia") ||
        (string.Equals(assemblyName, "Doroti.Host.Windows", StringComparison.Ordinal) &&
         string.Equals(referenceName, "Doroti.Vendor.Avalonia.Win32", StringComparison.Ordinal)) ||
        (string.Equals(assemblyName, "Doroti.Host.macOS", StringComparison.Ordinal) &&
         string.Equals(referenceName, "Doroti.Vendor.Avalonia.Native", StringComparison.Ordinal)) ||
        (assemblyName.StartsWith("Doroti.Validation.", StringComparison.Ordinal) &&
         referenceName is "Doroti.Vendor.Avalonia.Win32" or "Doroti.Vendor.Avalonia.Native");

    private static bool IsForbiddenVendorLayer(string assemblyName) =>
        assemblyName is
            "Doroti.Composition" or
            "Doroti.Rendering" or
            "Doroti.Widgets" or
            "Doroti.Runtime" or
            "Doroti.Engine" or
            "Avalonia.Base" or
            "Avalonia.Controls";

    private static bool IsPublicApi(ISymbol symbol)
    {
        if (symbol.DeclaredAccessibility != Accessibility.Public && symbol.DeclaredAccessibility != Accessibility.Protected && symbol.DeclaredAccessibility != Accessibility.ProtectedOrInternal)
        {
            return false;
        }

        for (var containing = symbol.ContainingType; containing is not null; containing = containing.ContainingType)
        {
            if (containing.DeclaredAccessibility != Accessibility.Public && containing.DeclaredAccessibility != Accessibility.Protected && containing.DeclaredAccessibility != Accessibility.ProtectedOrInternal)
            {
                return false;
            }
        }

        return true;
    }

    private static IEnumerable<ITypeSymbol?> GetExposedTypes(ISymbol symbol)
    {
        switch (symbol)
        {
            case INamedTypeSymbol namedType:
                yield return namedType.BaseType;
                foreach (var interfaceType in namedType.Interfaces)
                {
                    yield return interfaceType;
                }
                break;
            case IMethodSymbol method:
                yield return method.ReturnType;
                foreach (var parameter in method.Parameters)
                {
                    yield return parameter.Type;
                }
                break;
            case IPropertySymbol property:
                yield return property.Type;
                foreach (var parameter in property.Parameters)
                {
                    yield return parameter.Type;
                }
                break;
            case IFieldSymbol field:
                yield return field.Type;
                break;
            case IEventSymbol eventSymbol:
                yield return eventSymbol.Type;
                break;
        }
    }

    private static bool IsBackendType(ITypeSymbol type)
    {
        if (type is IArrayTypeSymbol array)
        {
            return IsBackendType(array.ElementType);
        }

        if (type is IPointerTypeSymbol)
        {
            return true;
        }

        if (type is INamedTypeSymbol named)
        {
            var namespaceName = named.ContainingNamespace?.ToDisplayString() ?? string.Empty;
            if (BackendNamespaces.Any(prefix =>
                    string.Equals(namespaceName, prefix, StringComparison.Ordinal) ||
                    namespaceName.StartsWith(prefix + ".", StringComparison.Ordinal)))
            {
                return true;
            }

            return named.TypeArguments.Any(IsBackendType);
        }

        return false;
    }

    private static string GetNamespace(ISymbol? symbol) => symbol switch
    {
        INamespaceSymbol namespaceSymbol => namespaceSymbol.ToDisplayString(),
        ITypeSymbol type => type.ContainingNamespace?.ToDisplayString() ?? string.Empty,
        IMethodSymbol method => method.ContainingType?.ContainingNamespace?.ToDisplayString() ?? string.Empty,
        IPropertySymbol property => property.Type.ContainingNamespace?.ToDisplayString() ?? string.Empty,
        IFieldSymbol field => field.Type.ContainingNamespace?.ToDisplayString() ?? string.Empty,
        IEventSymbol eventSymbol => eventSymbol.Type.ContainingNamespace?.ToDisplayString() ?? string.Empty,
        ILocalSymbol local => local.Type.ContainingNamespace?.ToDisplayString() ?? string.Empty,
        IParameterSymbol parameter => parameter.Type.ContainingNamespace?.ToDisplayString() ?? string.Empty,
        IAliasSymbol alias => GetNamespace(alias.Target),
        _ => string.Empty,
    };

    private static bool IsNewtonsoftNamespace(string namespaceName) =>
        string.Equals(namespaceName, "Newtonsoft.Json", StringComparison.Ordinal) ||
        namespaceName.StartsWith("Newtonsoft.Json.", StringComparison.Ordinal);
}
