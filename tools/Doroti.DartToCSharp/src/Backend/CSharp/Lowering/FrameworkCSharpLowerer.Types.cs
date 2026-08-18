using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Doroti.Tooling;

namespace Doroti.DartToCSharp;

internal sealed partial class FrameworkCSharpLowerer
{
    private string MapParameter(CoreResolvedParameter parameter, string? emittedName = null)
    {
        var isOptional = parameter.Kind is "optional-named" or "optional-positional";
        var mappedType = MapType(parameter.Type);
        if (mappedType == "void")
        {
            // A substituted Dart Never/bottom parameter can surface as void in
            // analyzer method contracts. CLR void is never a legal parameter
            // type; dynamic preserves the structural override contract.
            mappedType = "dynamic";
        }
        var name = emittedName ?? SafeIdentifier(parameter.Name);
        var parameterOwner = _session.ActiveDonorDeclaration ?? _session.ActiveDeclaration;
        if (parameterOwner?.Name == "TreeSliver" && parameter.Name == "treeRowExtentBuilder")
        {
            mappedType = "global::System.Func<TreeSliverNode<T>, global::Doroti.Framework.Rendering.SliverLayoutDimensions, double?>";
        }
        else if (parameterOwner?.Name == "ScrollAwareImageProvider" && parameter.Name == "context")
        {
            mappedType = "dynamic";
        }
        if (isOptional && !IsCompileTimeConstantDefault(parameter.DefaultValue, parameter.Type))
        {
            // C# rejects non-constant defaults; emit `= default!` and restore
            // Dart defaults at initializing-formal assignment sites when possible.
            if (IsValueType(mappedType))
            {
                return $"{MakeNullable(mappedType)} {name} = null";
            }
            return $"{mappedType} {name} = default!";
        }
        var suffix = isOptional ? $" = {MapDefault(parameter.DefaultValue, parameter.Type)}" : string.Empty;
        return $"{mappedType} {name}{suffix}";
    }

    private bool IsCompileTimeConstantDefault(string? value, string dartType)
    {
        if (value is null or "null" or "true" or "false")
        {
            return true;
        }
        if (double.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out _))
        {
            return true;
        }
        if (long.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out _))
        {
            return true;
        }
        if (value.StartsWith(".", StringComparison.Ordinal) ||
            value.StartsWith("double.", StringComparison.Ordinal))
        {
            return true;
        }
        if (value.EndsWith(".zero", StringComparison.Ordinal) &&
            IsValueType(MapType(dartType).TrimEnd('?')))
        {
            return true;
        }
        if (Regex.IsMatch(value, @"^(?:ui\.)?[A-Za-z_]\w*\.[A-Za-z_]\w*$", RegexOptions.CultureInvariant) &&
            IsEnumType(MapType(dartType).TrimEnd('?')))
        {
            return true;
        }
        if (value.Length >= 2 &&
            ((value[0] == '\'' && value[^1] == '\'') || (value[0] == '"' && value[^1] == '"')))
        {
            return true;
        }
        _ = dartType;
        return false;
    }

    private bool IsNonConstReferenceDefault(string? value, string dartType) =>
        !IsCompileTimeConstantDefault(value, dartType) &&
        !IsValueType(MapType(dartType).TrimEnd('?')) &&
        MapType(dartType).TrimEnd('?') is not "string" and not "object" &&
        !string.IsNullOrEmpty(value);

    private bool NeedsNonConstValueDefault(CoreResolvedParameter parameter) =>
        parameter.Kind is "optional-named" or "optional-positional" &&
        !IsCompileTimeConstantDefault(parameter.DefaultValue, parameter.Type) &&
        !string.IsNullOrEmpty(parameter.DefaultValue) &&
        IsValueType(MapType(parameter.Type));

    private bool NeedsRuntimeDefaultRestore(CoreResolvedParameter parameter) =>
        parameter.Kind is "optional-named" or "optional-positional" &&
        !IsCompileTimeConstantDefault(parameter.DefaultValue, parameter.Type) &&
        !string.IsNullOrEmpty(parameter.DefaultValue);

    private string MapNonConstDefaultExpression(string value, string? library = null)
    {
        var expression = value.StartsWith("const ", StringComparison.Ordinal)
            ? value["const ".Length..].Trim()
            : value.Trim();
        expression = expression.Replace("double.infinity", "double.PositiveInfinity", StringComparison.Ordinal)
            .Replace("double.negativeInfinity", "double.NegativeInfinity", StringComparison.Ordinal);
        if (Regex.Match(expression, @"^<(?<type>.+)>\[\]$", RegexOptions.CultureInvariant) is { Success: true } list)
        {
            return $"new List<{MapType(list.Groups["type"].Value)}>()";
        }
        if (Regex.Match(expression, @"^<(?<types>.+)>\{\}$", RegexOptions.CultureInvariant) is { Success: true } map)
        {
            var types = SplitGenericArguments(map.Groups["types"].Value);
            return types.Length == 2
                ? $"new DartMap<{MapType(types[0])}, {MapType(types[1])}>()"
                : "new DartMap<object, object>()";
        }
        if (Regex.Match(expression, @"^<(?<type>.+)>\[(?<items>.*)\]$", RegexOptions.CultureInvariant) is { Success: true } populatedList)
        {
            var itemExpressions = SplitGenericArguments(populatedList.Groups["items"].Value)
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Select(item => MapNonConstDefaultExpression(item, library));
            return $"new List<{MapType(populatedList.Groups["type"].Value)}> {{ {string.Join(", ", itemExpressions)} }}";
        }
        if (Regex.Match(expression, @"^(?<type>[A-Za-z_]\w*(?:<.*>)?)\((?<arguments>.*)\)$", RegexOptions.CultureInvariant) is { Success: true } constructor)
        {
            var type = MapType(constructor.Groups["type"].Value);
            var arguments = constructor.Groups["arguments"].Value.Replace('\'', '"');
            arguments = Regex.Replace(
                arguments,
                @"(?<!new\s)(?<![\w.])Color\(",
                "new global::Doroti.Ui.Color(",
                RegexOptions.CultureInvariant);
            arguments = Regex.Replace(
                arguments,
                @"\bkTouchSlop\b",
                "global::Doroti.Framework.Gestures.ConstantsLibrary.kTouchSlop",
                RegexOptions.CultureInvariant);
            if (type is "Color" or "global::Doroti.Ui.Color" &&
                arguments.Trim() == "_kColorDefault")
            {
                return $"new {type}(0xFF000000L)";
            }
            var factory = type is "Duration" or "global::Doroti.Runtime.Duration" ? ".Create" : string.Empty;
            return factory.Length > 0
                ? $"{type}{factory}({arguments})"
                : $"new {type}({arguments})";
        }
        if (Regex.Match(expression, @"^(?<type>[A-Za-z_]\w*)\.(?<name>[A-Za-z_]\w*)\((?<arguments>.*)\)$", RegexOptions.CultureInvariant) is { Success: true } named)
        {
            var type = MapType(named.Groups["type"].Value);
            if (type.EndsWith("EdgeInsets", StringComparison.Ordinal) && named.Groups["name"].Value == "fromLTRB")
            {
                return $"new {type}({named.Groups["arguments"].Value})";
            }
            var method = NamedConstructorMethodName(named.Groups["name"].Value);
            return $"{type}.{method}({named.Groups["arguments"].Value})";
        }
        if (expression.StartsWith("ui.", StringComparison.Ordinal))
        {
            return "Dart_uiLibrary." + expression[3..];
        }
        if (Regex.Match(expression, @"^(?:[A-Za-z_]\w*\.)?(?<type>[A-Za-z_]\w*)\.(?<name>[A-Za-z_]\w*)$", RegexOptions.CultureInvariant) is { Success: true } member)
        {
            return $"{MapType(member.Groups["type"].Value)}.{SafeIdentifier(member.Groups["name"].Value)}";
        }
        expression = expression.Replace("math.pi", "Dart_mathLibrary.pi", StringComparison.Ordinal);
        if (expression == "kNoDefaultValue")
        {
            return "global::Doroti.Framework.Foundation.DiagnosticsLibrary.kNoDefaultValue";
        }
        if (expression == "kTouchSlop")
        {
            return "global::Doroti.Framework.Gestures.ConstantsLibrary.kTouchSlop";
        }
        if (expression == "kLongPressTimeout")
        {
            return "global::Doroti.Framework.Gestures.ConstantsLibrary.kLongPressTimeout";
        }
        if (expression == "defaultTargetPlatform")
        {
            return "global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform";
        }
        if (Regex.IsMatch(expression, @"^[A-Za-z_]\w*$", RegexOptions.CultureInvariant) &&
            FindGlobalDeclaration(expression) is
            {
                Ast.Kind: CoreNodeKind.ClassDeclaration or
                CoreNodeKind.MixinDeclaration or CoreNodeKind.ExtensionTypeDeclaration
            })
        {
            return $"typeof({MapType(expression)})";
        }
        if (!string.IsNullOrEmpty(library) &&
            Regex.IsMatch(expression, @"^[A-Za-z_]\w*$", RegexOptions.CultureInvariant))
        {
            var currentOwner = _currentDeclarations?.FirstOrDefault(candidate =>
                candidate.Ast.Kind is CoreNodeKind.TopLevelVariableDeclaration or CoreNodeKind.FunctionDeclaration &&
                candidate.Name == expression);
            var requestedOwner = _semanticIndex.FindDeclaration(library, expression);
            var matchingOwners = _semanticIndex.FindDeclarations(expression)
                .Where(candidate => candidate.Ast.Kind is CoreNodeKind.TopLevelVariableDeclaration or CoreNodeKind.FunctionDeclaration)
                .Take(2)
                .ToArray();
            var uniqueOwner = matchingOwners.Length == 1 ? matchingOwners[0] : null;
            var owner = currentOwner ?? requestedOwner ?? uniqueOwner;
            if (owner is not null)
            {
                var ownerLibrary = LibraryUriFromElementId(owner.Element.CanonicalId);
                return LibraryStaticClassName(ownerLibrary) + "." + SafeIdentifier(expression);
            }
        }
        return expression.Replace('\'', '"');
    }

    private string MapParameterRuntimeDefault(CoreResolvedParameter parameter, string? library = null)
    {
        if (parameter.Name == "displayStringForOption" && parameter.DefaultValue == "defaultStringForOption")
        {
            var delegateType = MapType(parameter.Type).TrimEnd('?');
            return $"new {delegateType}((__option) => defaultStringForOption(__option))";
        }
        return MapNonConstDefaultExpression(parameter.DefaultValue!, library);
    }

    private IEnumerable<string> MapParameters(IEnumerable<CoreResolvedParameter> parameters)
    {
        var optionalSeen = false;
        var wildcardIndex = 0;
        var usedNames = new HashSet<string>(StringComparer.Ordinal);
        foreach (var parameter in parameters)
        {
            var optional = parameter.Kind is "optional-named" or "optional-positional";
            var emittedName = parameter.Name is "_" or "<unnamed>"
                ? $"__unused{wildcardIndex++}"
                : SafeIdentifier(parameter.Name);
            if (!usedNames.Add(emittedName))
            {
                var baseName = emittedName.TrimStart('@');
                do
                {
                    emittedName = $"__{baseName}{wildcardIndex++}";
                }
                while (!usedNames.Add(emittedName));
            }
            if (!optional && optionalSeen)
            {
                yield return $"{MapType(parameter.Type)} {emittedName} = default!";
            }
            else
            {
                yield return MapParameter(parameter, emittedName);
            }
            optionalSeen |= optional;
        }
    }

    private string MapDefault(string? value, string dartType)
    {
        if (value is null)
        {
            var mappedDefaultType = MapType(dartType).TrimEnd('?');
            if (IsUnboundTypeParameterName(mappedDefaultType) ||
                _session.ActiveMethodTypeParameters.Contains(mappedDefaultType))
            {
                return "default";
            }
            return dartType.EndsWith("?", StringComparison.Ordinal) ? "null" : "default!";
        }
        if (value == "null" &&
            (IsUnboundTypeParameterName(MapType(dartType).TrimEnd('?')) ||
             _session.ActiveMethodTypeParameters.Contains(MapType(dartType).TrimEnd('?'))))
        {
            return "default";
        }
        if (value is "null" or "true" or "false")
        {
            return value;
        }
        if (value.StartsWith(".", StringComparison.Ordinal))
        {
            return MapType(dartType).TrimEnd('?') + "." + SafeIdentifier(value[1..]);
        }
        if (value.StartsWith("ui.", StringComparison.Ordinal))
        {
            if (IsEnumType(MapType(dartType).TrimEnd('?')))
            {
                return value[3..];
            }
            return "Dart_uiLibrary." + value[3..];
        }
        if (value.EndsWith(".zero", StringComparison.Ordinal) &&
            IsValueType(MapType(dartType).TrimEnd('?')))
        {
            return "default";
        }
        if (value.StartsWith("double.", StringComparison.Ordinal))
        {
            return MapDoubleConstant(value["double.".Length..]);
        }
        if (Regex.Match(value, @"^(?<type>[A-Za-z_]\w*)\.(?<member>[A-Za-z_]\w*)$", RegexOptions.CultureInvariant) is { Success: true } staticMember)
        {
            return $"{MapType(staticMember.Groups["type"].Value)}.{SafeIdentifier(staticMember.Groups["member"].Value)}";
        }
        if (dartType.TrimEnd('?') is "MouseCursor" or "TextInputType" or "AutofillConfiguration" or "TextRange")
        {
            return "default!";
        }
        if (value.StartsWith("const ", StringComparison.Ordinal) ||
            value.StartsWith("<", StringComparison.Ordinal) || value is "[]" or "{}" ||
            value.Contains(":", StringComparison.Ordinal))
        {
            return "default!";
        }
        if (IsNonConstReferenceDefault(value, dartType))
        {
            return "null";
        }
        return value.Replace('\'', '"');
    }

    private string MapDoubleConstant(string name) => name switch
    {
        "infinity" => "double.PositiveInfinity",
        "negativeInfinity" => "double.NegativeInfinity",
        "nan" => "double.NaN",
        "maxFinite" => "double.MaxValue",
        "minPositive" => "double.Epsilon",
        _ => "double." + SafeIdentifier(name),
    };

    private string FormatTypeParameters(CoreResolvedTypeParameter[]? typeParameters)
    {
        if (typeParameters is null or { Length: 0 })
        {
            return string.Empty;
        }
        return $"<{string.Join(", ", typeParameters.Select(item => SafeIdentifier(item.Name)))}>";
    }

    private string FormatTypeParameterConstraints(CoreResolvedTypeParameter[]? typeParameters, CoreResolvedDeclaration declaration)
    {
        if (declaration.Name is "ParentDataWidget" or "ParentDataElement" && typeParameters is { Length: > 0 })
        {
            typeParameters = typeParameters.Select(parameter => parameter with { Bound = null }).ToArray();
        }
        if (declaration.Name == "RestorableNumN" && typeParameters is { Length: > 0 })
        {
            typeParameters = typeParameters.Select(parameter => parameter with { Bound = null }).ToArray();
        }
        var relevantTypes = new List<string>();
        if (declaration.Element.Supertype is { } supertype && supertype != "Object")
        {
            relevantTypes.Add(MapType(supertype));
        }
        relevantTypes.AddRange((declaration.Element.Mixins ?? []).Select(MapType));
        relevantTypes.AddRange((declaration.Element.Interfaces ?? []).Select(MapType));
        foreach (var member in declaration.Members)
        {
            if (member.Element.Type is not null)
            {
                relevantTypes.Add(MapType(member.Element.Type));
            }
            if (member.Element.ReturnType is not null)
            {
                relevantTypes.Add(MapType(member.Element.ReturnType));
            }
            if (member.Element.Parameters is not null)
            {
                relevantTypes.AddRange(member.Element.Parameters.Select(parameter => MapType(parameter.Type)));
            }
        }
        return FormatTypeParameterConstraints(typeParameters, relevantTypes);
    }

    private string FormatTypeParameterConstraints(CoreResolvedTypeParameter[]? typeParameters, IEnumerable<string> relevantTypes)
    {
        if (typeParameters is null or { Length: 0 })
        {
            return string.Empty;
        }
        var constraints = typeParameters
            .Select(item =>
            {
                var parts = new List<string>();
                if (item.Bound is not null)
                {
                    var bound = MapType(item.Bound);
                    // C# forbids `where T : object` / dynamic-equivalent bounds.
                    if (bound.TrimEnd('?') == "double")
                    {
                        parts.Add("struct");
                    }
                    else if (bound.TrimEnd('?') is not ("object" or "dynamic"))
                    {
                        parts.Add(bound);
                    }
                }
                if (parts.Count == 0 && RequiresNotNullConstraint(item.Name, relevantTypes))
                {
                    parts.Add("notnull");
                }
                return parts.Count > 0 ? $"where {SafeIdentifier(item.Name)} : {string.Join(", ", parts)}" : null;
            })
            .Where(item => item is not null)
            .ToArray();
        return constraints.Length > 0 ? " " + string.Join(" ", constraints) : string.Empty;
    }

    private bool RequiresNotNullConstraint(string name, IEnumerable<string> relevantTypes)
    {
        var pattern = $"\\b(Dictionary|DartMap|HashSet)<{Regex.Escape(name)}\\b";
        return relevantTypes.Any(type => Regex.IsMatch(type, pattern));
    }

    private string MakeNullable(string type) => type.EndsWith("?", StringComparison.Ordinal) ? type : type + "?";

    private bool IsValueType(string type)
    {
        var baseType = type.TrimEnd('?');
        var simpleType = baseType[(baseType.LastIndexOf('.') + 1)..];
        return baseType.StartsWith('(') && baseType.EndsWith(')') ||
            baseType.StartsWith("MapEntry<", StringComparison.Ordinal) ||
            baseType is "int" or "long" or "short" or "byte" or "uint" or "ulong" or "ushort" or "sbyte" or "bool" or "double" or "float" or "decimal" or "char" or "DateTime" or "TimeSpan" or "Guid" or "Enum" or "IntPtr" or "UIntPtr" or "Duration" or "Offset" or "Size" or "Rect" or "Radius" or "FrameTiming" ||
            simpleType is "Brightness" or "Locale" or "TextAffinity" or "TextDirection" or "KeyEventDeviceType" or
                "PointerDeviceKind" or "PointerChange" or "PointerSignalKind" ||
            IsEnumType(baseType) || FindGlobalDeclaration(simpleType)?.Ast.Kind == CoreNodeKind.EnumDeclaration;
    }

    private bool IsEnumType(string type)
    {
        var baseType = type.TrimEnd('?');
        var simpleType = baseType[(baseType.LastIndexOf('.') + 1)..];
        return simpleType is "Brightness" or "TextAffinity" or "TextDirection" or "TextAlign" or "FontWeight" or "KeyEventDeviceType" or
                "PointerDeviceKind" or "PointerChange" or "PointerSignalKind" or "BlendMode" or
                "BlurStyle" or "BoxHeightStyle" or "BoxWidthStyle" or "Clip" or "FilterQuality" or
                "FontStyle" or "PlaceholderAlignment" or "TextBaseline" or "TextLeadingDistribution" or
                "TileMode" or "PathFillType" or "PathOperation" or "TextDecorationStyle" or "PaintingStyle" or
                "SemanticsAction" or "SemanticsFlag" or "SemanticsRole" or "SemanticsInputType" or
                "SemanticsValidationResult" or "SemanticsHitTestBehavior" or "CheckedState" or "Tristate" or
                "DiagnosticsTreeStyle" or "DiagnosticLevel" or "AppExitType" ||
            FindGlobalDeclaration(simpleType)?.Ast.Kind == CoreNodeKind.EnumDeclaration;
    }

    private static bool IsExternalStaticFactoryType(string typeName)
    {
        var mapped = typeName.TrimEnd('?');
        var simple = mapped[(mapped.LastIndexOf('.') + 1)..];
        return simple is "Size" or "Radius" or "Rect" or "RRect" or "RSuperellipse" or "Color" or "ColorFilter" or "MaskFilter" or
            "Gradient" or "ImageShader" or "Matrix4" or "ParagraphStyle" or "TextStyle" or "StrutStyle";
    }

    private string MethodSignatureKey(CoreResolvedMember method) =>
        $"{method.Name}({string.Join(",", (method.Element.Parameters ?? []).Select(parameter => MapType(parameter.Type)))})";

    private CoreResolvedParameter[] CanonicalOverrideParameters(
        CoreResolvedDeclaration declaration,
        CoreResolvedMember method,
        CoreResolvedParameter[] sourceParameters,
        CoreResolvedMember? contractMember)
    {
        var root = FindOverrideFamilyRoot(declaration, method, contractMember);
        if (root is null)
        {
            return sourceParameters;
        }
        if (method.Name.StartsWith('_') &&
            !string.Equals(
                LibraryUriFromElementId(root.Value.Declaration.Element.CanonicalId),
                LibraryUriFromElementId(declaration.Element.CanonicalId),
                StringComparison.Ordinal))
        {
            // Dart private members are library-scoped. Identically named private
            // methods from a base class in another library are not an override family.
            return sourceParameters;
        }

        var familyDeclarations = _semanticIndex.Descendants(root.Value.Declaration.Name)
            .Append(root.Value.Declaration)
            .Where(candidate => candidate.Element.CanonicalId == root.Value.Declaration.Element.CanonicalId ||
                IsDescendantOf(candidate, root.Value.Declaration))
            .OrderBy(candidate => candidate.Element.CanonicalId, StringComparer.Ordinal)
            .ToArray();
        if (contractMember is null &&
            declaration.Ast.Kind != CoreNodeKind.MixinDeclaration &&
            familyDeclarations.All(candidate => candidate.Element.CanonicalId == declaration.Element.CanonicalId))
        {
            return sourceParameters;
        }

        var parameters = new List<CoreResolvedParameter>();
        var names = new HashSet<string>(StringComparer.Ordinal);
        void AddParameters(IEnumerable<CoreResolvedParameter> candidates)
        {
            var positionalIndex = 0;
            foreach (var candidate in candidates)
            {
                if (!names.Add(candidate.Name))
                {
                    if (candidate.Kind.Contains("positional", StringComparison.OrdinalIgnoreCase)) positionalIndex++;
                    continue;
                }
                if (candidate.Kind.Contains("positional", StringComparison.OrdinalIgnoreCase))
                {
                    var currentPositionals = parameters.Count(parameter =>
                        parameter.Kind.Contains("positional", StringComparison.OrdinalIgnoreCase));
                    if (positionalIndex >= currentPositionals)
                    {
                        parameters.Insert(positionalIndex, candidate);
                    }
                    positionalIndex++;
                    continue;
                }
                parameters.Add(candidate);
            }
        }

        AddParameters(root.Value.Member.Element.Parameters ?? []);
        foreach (var candidate in familyDeclarations)
        {
            AddParameters(candidate.Members
                .Where(member => member.Kind == "method" && SameMemberShape(member, method))
                .OrderBy(member => member.Offset)
                .SelectMany(member => member.Element.Parameters ?? []));
            AddParameters(AppliedMixinDeclarations(candidate)
                .SelectMany(mixin => mixin.Members)
                .Where(member => member.Kind == "method" && SameMemberShape(member, method))
                .OrderBy(member => member.Offset)
                .SelectMany(member => member.Element.Parameters ?? []));
        }

        AddParameters(sourceParameters);
        for (var index = 0; index < sourceParameters.Length && index < parameters.Count; index++)
        {
            if (sourceParameters[index].Kind is "optional-named" or "optional-positional")
            {
                // Parameter optionality belongs to the concrete Dart method.
                // An override may widen a required contract parameter to an
                // optional one (SelectableRegionState.selectAll is one such
                // case); keep the canonical family type/name, but retain the
                // implementation's omission contract and default value.
                parameters[index] = parameters[index] with
                {
                    Kind = sourceParameters[index].Kind,
                    DefaultValue = sourceParameters[index].DefaultValue
                };
            }
            if (ContainsUnboundTypeParameter(parameters[index].Type) &&
                !ContainsUnboundTypeParameter(sourceParameters[index].Type))
            {
                parameters[index] = sourceParameters[index];
            }
        }
        return parameters.ToArray();
    }

    private (CoreResolvedDeclaration Declaration, CoreResolvedMember Member)? FindOverrideFamilyRoot(
        CoreResolvedDeclaration declaration,
        CoreResolvedMember method,
        CoreResolvedMember? contractMember)
    {
        var owner = declaration;
        var current = contractMember ?? method;
        if (contractMember is not null)
        {
            owner = FindDeclaringDeclaration(contractMember) ?? declaration;
        }
        else if (declaration.Ast.Kind == CoreNodeKind.MixinDeclaration)
        {
            var application = _semanticIndex.TypeUsers(declaration.Name)
                .Where(candidate => AppliedMixinDeclarations(candidate).Any(mixin =>
                    mixin.Element.CanonicalId == declaration.Element.CanonicalId))
                .OrderBy(candidate => candidate.Element.CanonicalId, StringComparer.Ordinal)
                .FirstOrDefault(candidate => FindBaseContractMember(candidate, method) is not null);
            if (application is not null && FindBaseContractMember(application, method) is { } appliedContract)
            {
                owner = FindDeclaringDeclaration(appliedContract) ?? application;
                current = appliedContract;
            }
        }

        while (FindBaseContractMember(owner, current) is { } parent)
        {
            var parentOwner = FindDeclaringDeclaration(parent);
            if (parentOwner is null || parentOwner.Element.CanonicalId == owner.Element.CanonicalId)
            {
                break;
            }
            owner = parentOwner;
            current = parent;
        }
        return (owner, current);
    }

    private CoreResolvedDeclaration? FindDeclaringDeclaration(CoreResolvedMember member) =>
        _semanticIndex.FindMemberOwner(member);

    private IReadOnlyDictionary<string, string> ContractTypeParameterSubstitutions(
        CoreResolvedDeclaration declaration,
        CoreResolvedMember? contractMember)
    {
        var owner = contractMember is null ? null : FindDeclaringDeclaration(contractMember);
        if (owner?.Element.TypeParameters is not { Length: > 0 } parameters)
        {
            return new Dictionary<string, string>(StringComparer.Ordinal);
        }
        var application = new[] { declaration.Element.Supertype }
            .Concat(declaration.Element.Mixins ?? [])
            .Concat(declaration.Element.Interfaces ?? [])
            .Where(type => !string.IsNullOrWhiteSpace(type))
            .Select(type => StripLibraryPrefix(type!))
            .FirstOrDefault(type => string.Equals(type.Split('<')[0], owner.Name, StringComparison.Ordinal));
        if (application is null)
        {
            return new Dictionary<string, string>(StringComparer.Ordinal);
        }
        var genericStart = application.IndexOf('<');
        if (genericStart < 0 || !application.EndsWith('>'))
        {
            return new Dictionary<string, string>(StringComparer.Ordinal);
        }
        var arguments = SplitGenericArguments(application[(genericStart + 1)..^1]);
        return parameters
            .Take(Math.Min(parameters.Length, arguments.Length))
            .Select((parameter, index) => new KeyValuePair<string, string>(parameter.Name, arguments[index]))
            .ToDictionary(item => item.Key, item => item.Value, StringComparer.Ordinal);
    }

    private static string ApplyTypeParameterSubstitutions(
        string type,
        IReadOnlyDictionary<string, string> substitutions)
    {
        var result = type;
        foreach (var substitution in substitutions.OrderByDescending(item => item.Key.Length))
        {
            result = Regex.Replace(
                result,
                $@"(?<![A-Za-z0-9_]){Regex.Escape(substitution.Key)}(?![A-Za-z0-9_])",
                substitution.Value,
                RegexOptions.CultureInvariant);
        }
        return result;
    }

    private bool TryGenericTypeApplication(string type, string expectedOuter, out string[] arguments)
    {
        var normalized = StripLibraryPrefix(type).TrimEnd('?');
        var genericStart = normalized.IndexOf('<');
        if (genericStart <= 0 || !normalized.EndsWith('>') ||
            !string.Equals(normalized[..genericStart], expectedOuter, StringComparison.Ordinal))
        {
            arguments = [];
            return false;
        }
        arguments = SplitGenericArguments(normalized[(genericStart + 1)..^1]);
        return true;
    }

    private bool IsDescendantOf(CoreResolvedDeclaration declaration, CoreResolvedDeclaration ancestor)
    {
        var pending = new Queue<string>(DirectBaseNames(declaration));
        var visited = new HashSet<string>(StringComparer.Ordinal);
        while (pending.Count > 0)
        {
            var baseName = pending.Dequeue();
            var baseDeclaration = FindGlobalDeclaration(baseName);
            if (baseDeclaration is null || !visited.Add(baseDeclaration.Element.CanonicalId))
            {
                continue;
            }
            if (baseDeclaration.Element.CanonicalId == ancestor.Element.CanonicalId)
            {
                return true;
            }
            foreach (var parent in DirectBaseNames(baseDeclaration))
            {
                pending.Enqueue(parent);
            }
        }
        return false;
    }

    private string MapInheritanceType(string rawType)
    {
        var type = StripLibraryPrefix(rawType);
        return type switch
        {
            "Action<Intent>" => "Action<Intent>",
            "GestureRecognizerFactory<GestureRecognizer>" => "GestureRecognizerFactory<GestureRecognizer>",
            _ => MapType(rawType),
        };
    }

    private string MapType(string dartType)
    {
        var mappingKey = dartType.Trim();
        if (!_activeTypeMappings.Add(mappingKey))
        {
            // A raw Dart generic can have an F-bounded parameter such as
            // `T extends Comparable<T>`. CLR has no raw generic form, so the
            // recursively re-entered argument is represented by object while
            // the outer nominal type and its arity remain explicit.
            return "object";
        }
        try
        {
            return MapTypeCore(mappingKey);
        }
        finally
        {
            _activeTypeMappings.Remove(mappingKey);
        }
    }

    private string MapTypeCore(string dartType)
    {
        var nullable = dartType.EndsWith("?", StringComparison.Ordinal);
        var type = nullable ? dartType[..^1] : dartType;
        var visitedSubstitutions = new HashSet<string>(StringComparer.Ordinal);
        while (!_session.ActiveMethodTypeParameters.Contains(type) &&
               _session.TypeParameterSubstitutions.TryGetValue(type, out var substitutedType) &&
               !string.Equals(type, substitutedType, StringComparison.Ordinal) &&
               visitedSubstitutions.Add(type))
        {
            nullable |= substitutedType.EndsWith("?", StringComparison.Ordinal);
            type = substitutedType.TrimEnd('?');
        }
        if (type is "FlutterView" or "dart:ui.FlutterView" or "ui.FlutterView")
        {
            const string dorotiView = "global::Doroti.Ui.DorotiView";
            return nullable ? dorotiView + "?" : dorotiView;
        }
        if (type.StartsWith("dart:ui.", StringComparison.Ordinal))
        {
            var uiType = "global::Doroti.Ui." + type["dart:ui.".Length..];
            return nullable ? uiType + "?" : uiType;
        }
        if (type.StartsWith("ui.", StringComparison.Ordinal))
        {
            var uiType = "global::Doroti.Ui." + type["ui.".Length..];
            return nullable ? uiType + "?" : uiType;
        }
        if (type is "developer.CreationLocation" or "dart:developer.CreationLocation")
        {
            const string creationLocation = "global::Doroti.Runtime.CreationLocation";
            return nullable ? creationLocation + "?" : creationLocation;
        }
        if (type.StartsWith("Doroti.Ui.", StringComparison.Ordinal))
        {
            var uiType = "global::" + type;
            return nullable ? uiType + "?" : uiType;
        }
        if (type.StartsWith("global::", StringComparison.Ordinal))
        {
            return nullable ? type + "?" : type;
        }
        if (type.StartsWith("InvalidType", StringComparison.Ordinal))
        {
            type = "object";
        }
        if (type == "dynamic")
        {
            return "dynamic";
        }
        if (type == "Object")
        {
            return nullable ? "object?" : "object";
        }
        var sourceLibrary = _session.ActiveSourceLibrary ?? _currentLibrary;
        if (type == "Image" && sourceLibrary is { } imageLibrary &&
            (imageLibrary.EndsWith("/basic.dart", StringComparison.Ordinal) ||
             imageLibrary.EndsWith("/snapshot_widget.dart", StringComparison.Ordinal) ||
             imageLibrary.EndsWith("/widget_inspector.dart", StringComparison.Ordinal) ||
             imageLibrary.EndsWith("/_accessibility_evaluations.dart", StringComparison.Ordinal)))
        {
            const string uiImage = "global::Doroti.Ui.Image";
            return nullable ? uiImage + "?" : uiImage;
        }
        type = StripLibraryPrefix(type);
        if (type is "TickerProviderStateMixin<StatefulWidget>" or
            "RestorationMixin<StatefulWidget>" or
            "ToggleableStateMixin<StatefulWidget>" or
            "_RawMenuAnchorBaseMixin<StatefulWidget>")
        {
            // These are Dart's covariant, erased mixin-owner views. Their CLR
            // interfaces necessarily consume and produce the generic state
            // parameter, so use checked dynamic dispatch only at the broad
            // owner view while keeping concrete mixin implementations typed.
            return "dynamic";
        }
        if (type == "PageRouteFactory")
        {
            return nullable ? "PageRouteFactory?" : "PageRouteFactory";
        }
        if (type == "State")
        {
            return nullable ? "IState?" : "IState";
        }
        if (type == "DisposableBuildContext" &&
            (_session.ActiveDonorDeclaration ?? _session.ActiveDeclaration)?.Name != "DisposableBuildContext")
        {
            return "dynamic";
        }
        if (type == "DiagnosticableTreeNode")
        {
            var diagnosticNode = "global::Doroti.Framework.Foundation.DiagnosticableTreeNode<global::Doroti.Framework.Foundation.DiagnosticableTree>";
            return nullable ? diagnosticNode + "?" : diagnosticNode;
        }
        if (type == "SemanticsBinding")
        {
            var semanticsBinding = "global::Doroti.Framework.Semantics.SemanticsBinding";
            return nullable ? semanticsBinding + "?" : semanticsBinding;
        }
        if (type == "HitTestEntry")
        {
            return nullable ? "HitTestEntry<HitTestTarget>?" : "HitTestEntry<HitTestTarget>";
        }
        if (type.Contains(" Function", StringComparison.Ordinal) &&
            TryMapNamedParameterFunctionAlias(type) is { } sourceOrderedFunctionAlias)
        {
            return nullable ? MakeNullable(sourceOrderedFunctionAlias) : sourceOrderedFunctionAlias;
        }
        // Dart typedefs are transparent aliases. Emitting a distinct CLR delegate
        // for a named callback while analyzer-expanded occurrences become
        // Action/Func makes otherwise identical assignments incompatible. Keep the
        // declaration for API inventory, but lower references to its structural
        // function type.
        if (FindGlobalDeclaration(type) is { Ast.Kind: CoreNodeKind.GenericTypeAlias } aliasDeclaration &&
            aliasDeclaration.Element.Type is { } aliasType &&
            aliasType.Contains(" Function", StringComparison.Ordinal) &&
            !string.Equals(aliasType, type, StringComparison.Ordinal))
        {
            var previousSubstitutions = _session.TypeParameterSubstitutions;
            var substitutions = previousSubstitutions.ToDictionary(item => item.Key, item => item.Value, StringComparer.Ordinal);
            foreach (var parameter in aliasDeclaration.Element.TypeParameters ?? [])
            {
                substitutions.TryAdd(parameter.Name, "object");
            }
            _session.TypeParameterSubstitutions = substitutions;
            try
            {
                var mappedAlias = MapType(aliasType);
                return nullable ? MakeNullable(mappedAlias) : mappedAlias;
            }
            finally
            {
                _session.TypeParameterSubstitutions = previousSubstitutions;
            }
        }
        if (type.Contains("DiagnosticsNode", StringComparison.Ordinal) &&
            type.Contains(" Function()", StringComparison.Ordinal))
        {
            return nullable ? "InformationCollector?" : "InformationCollector";
        }
        if (type == "void Function(Object, StackTrace?)")
        {
            return nullable
                ? "global::System.Action<object, global::System.Diagnostics.StackTrace?>?"
                : "global::System.Action<object, global::System.Diagnostics.StackTrace?>";
        }
        if (type == "void Function(T, void Function(Object, StackTrace?))")
        {
            return "global::System.Action<T, global::System.Action<object, global::System.Diagnostics.StackTrace?>>";
        }
        if (type == "void Function(AnimationStatus)" &&
            FindGlobalDeclaration("AnimationStatusListener") is { Ast.Kind: CoreNodeKind.GenericTypeAlias } statusListenerAlias &&
            string.Equals(statusListenerAlias.Element.Type, type, StringComparison.Ordinal))
        {
            var namedFunctionAlias = EmittedTypeName(
                LibraryUriFromElementId(statusListenerAlias.Element.CanonicalId),
                statusListenerAlias.Name);
            return nullable ? MakeNullable(namedFunctionAlias) : namedFunctionAlias;
        }
        if (type.Contains("SliverLayoutDimensions", StringComparison.Ordinal) &&
            type.Contains(" Function", StringComparison.Ordinal))
        {
            return nullable ? "ItemExtentBuilder?" : "ItemExtentBuilder";
        }
        if (type.Contains("PageRoute<T> Function<T>", StringComparison.Ordinal))
        {
            return nullable ? "PageRouteFactory?" : "PageRouteFactory";
        }
        if (type.Contains(" Function", StringComparison.Ordinal) && TryMapFunctionType(type) is { } mappedFunction)
        {
            return nullable ? MakeNullable(mappedFunction) : mappedFunction;
        }
        if (type.StartsWith('(') && type.EndsWith(')'))
        {
            var tuple = MapRecordType(type[1..^1]);
            return nullable ? tuple + "?" : tuple;
        }
        string mapped;
        var genericStart = type.IndexOf('<');
        if (genericStart > 0 && type.EndsWith('>'))
        {
            var outer = type[..genericStart];
            var arguments = SplitGenericArguments(type[(genericStart + 1)..^1]);
            if (outer is "Set" or "HashSet" or "LinkedHashSet" && arguments.Length == 1 &&
                StripLibraryPrefix(arguments[0]).TrimEnd('?').StartsWith("_WidgetTicker", StringComparison.Ordinal))
            {
                const string tickerSet = "HashSet<global::Doroti.Framework.Scheduler.Ticker>";
                return nullable ? tickerSet + "?" : tickerSet;
            }
            if (outer == "Tween" && arguments.Length == 1 &&
                arguments[0].TrimEnd('?') is "dynamic" or "Object" or "object")
            {
                const string dartTween = "global::Doroti.Framework.Animation.IDartTween";
                return nullable ? dartTween + "?" : dartTween;
            }
            if ((outer == "Action" && arguments.Length == 1 &&
                 StripLibraryPrefix(arguments[0]).TrimEnd('?') == "Intent") ||
                (outer == "GestureRecognizerFactory" && arguments.Length == 1 &&
                 StripLibraryPrefix(arguments[0]).TrimEnd('?') == "GestureRecognizer"))
            {
                // Dart generic classes are covariant at use sites, while these
                // CLR classes consume T and cannot be declared variant. Preserve
                // Dart's checked runtime dispatch at the erased supertype only;
                // concrete Action<T>/factory declarations remain strongly typed.
                return "dynamic";
            }
            if (arguments.Any(argument => argument.TrimEnd('?') is "Object" or "object" or "dynamic") &&
                outer is "Route" or "TransitionRoute" or "PageRoute" or "ModalRoute" or
                    "_DragAvatar" or "PopEntry" or "FormFieldState" or "Router" or
                    "RouteInformationParser" or "RouterDelegate" or "_RouterState" or
                    "TreeSliverNode" or "LocalizationsDelegate")
            {
                // These Dart declarations are covariant at their public use
                // sites. Their CLR members consume T, so the erased Object view
                // must preserve Dart's checked runtime dispatch.
                return "dynamic";
            }
            if (outer == "State" && arguments.Length == 1 &&
                (arguments[0].TrimEnd('?') is "StatefulWidget" or "dynamic" or "Object" or "object" ||
                 _currentLibrary.Contains("disposable_build_context.dart", StringComparison.Ordinal) ||
                 (_session.ActiveDonorDeclaration ?? _session.ActiveDeclaration)?.Name == "DisposableBuildContext" ||
                 (_session.ActiveDonorDeclaration ?? _session.ActiveDeclaration)?.Element.TypeParameters?
                    .Any(parameter => parameter.Name == arguments[0].TrimEnd('?') &&
                        parameter.Bound?.StartsWith("State", StringComparison.Ordinal) == true) == true))
            {
                return nullable ? "IState?" : "IState";
            }
            if (outer is "ValueChanged" or "ValueSetter" && arguments.Length == 1)
            {
                var callback = $"global::System.Action<{MapGenericArgument(arguments[0])}>";
                return nullable ? callback + "?" : callback;
            }
            if (FindGlobalDeclaration(outer) is { Ast.Kind: CoreNodeKind.GenericTypeAlias } genericAlias &&
                genericAlias.Element.Type is { } genericAliasType &&
                genericAliasType.Contains(" Function", StringComparison.Ordinal))
            {
                var previousSubstitutions = _session.TypeParameterSubstitutions;
                var substitutions = previousSubstitutions.ToDictionary(item => item.Key, item => item.Value, StringComparer.Ordinal);
                var parameters = genericAlias.Element.TypeParameters ?? [];
                for (var index = 0; index < Math.Min(parameters.Length, arguments.Length); index++)
                {
                    substitutions[parameters[index].Name] = arguments[index];
                }
                _session.TypeParameterSubstitutions = substitutions;
                try
                {
                    var mappedAlias = MapType(genericAliasType);
                    return nullable ? MakeNullable(mappedAlias) : mappedAlias;
                }
                finally
                {
                    _session.TypeParameterSubstitutions = previousSubstitutions;
                }
            }
            if (outer == "ImageProvider" && arguments.Length == 1 &&
                arguments[0].TrimEnd('?') is "Object" or "object" or "dynamic")
            {
                return "dynamic";
            }
            if (outer == "Map")
            {
                mapped = $"DartMap<{string.Join(", ", arguments.Select(MapGenericArgument))}>";
            }
            else if (outer == "Future")
            {
                var argument = arguments.Length == 0 ? "object" : arguments[0];
                var mappedArgument = MapGenericArgument(argument);
                mapped = argument == "void" ? "Future" : $"Future<{mappedArgument}>";
            }
            else if (outer == "FutureOr")
            {
                mapped = "object";
            }
            else if (outer == "AsyncValueGetter" && arguments.Length == 1)
            {
                mapped = $"global::System.Func<Future<{MapGenericArgument(arguments[0])}>>";
            }
            else if (outer == "AsyncValueSetter" && arguments.Length == 1)
            {
                mapped = $"global::System.Func<{MapGenericArgument(arguments[0])}, Future>";
            }
            else
            {
                var mappedOuter = outer switch
                {
                    "List" => "List",
                    "Set" => "HashSet",
                    "HashSet" => "HashSet",
                    "LinkedHashSet" => "HashSet",
                    "LinkedList" => "DartLinkedList",
                    "LinkedListEntry" => "DartLinkedListEntry",
                    "HashMap" => "DartMap",
                    "SplayTreeMap" => "SortedDictionary",
                    "SplayTreeSet" => "SortedSet",
                    "Iterable" => "IEnumerable",
                    "Iterator" => "IEnumerator",
                    "Comparable" => "IComparable",
                    "Comparator" => "Comparison",
                    "ValueChanged" or "ValueSetter" => "global::System.Action",
                    "ValueGetter" => "global::System.Func",
                    _ => MapNamedType(outer),
                };
                var mappedArguments = outer == "EnumProperty"
                    ? arguments.Select(argument => MapGenericArgument(argument).TrimEnd('?'))
                    : arguments.Select(MapGenericArgument);
                mapped = $"{mappedOuter}<{string.Join(", ", mappedArguments)}>";
            }
        }
        else
        {
            mapped = type switch
            {
                "String" or "string" => "string",
                "Object" or "object" => "object",
                "dynamic" => "object",
                "bool" => "bool",
                "long" => "long",
                "float" => "float",
                "decimal" => "decimal",
                // Dart VM int is arbitrary-precision / 64-bit; map to long so SMI-sized
                // constants (e.g. kMaxUnsignedSMI) and bit shifts stay representable.
                "int" or "Integer" => "long",
                "double" or "num" => "double",
                "void" => "void",
                "Type" => "Type",
                "Vector4" => "global::System.Numerics.Vector4",
                "Quad" => "global::Doroti.Ui.Quad",
                "ClipOp" => "global::Doroti.Ui.ClipOp",
                "RSTransform" => "global::Doroti.Ui.RSTransform",
                "PointMode" => "global::Doroti.Ui.PointMode",
                "Vertices" => "global::Doroti.Ui.Vertices",
                "FragmentShader" => "global::Doroti.Ui.FragmentShader",
                "FragmentProgram" => "global::Doroti.Ui.FragmentProgram",
                "DisplayFeature" => "global::Doroti.Ui.DisplayFeature",
                "ViewConfiguration" => "global::Doroti.Framework.Rendering.ViewConfiguration",
                "Map" => "System.Collections.IDictionary",
                "List" => "System.Collections.IList",
                "Uri" => "DartUri",
                "HttpClient" => "global::Doroti.Runtime.HttpClient",
                "HttpClientRequest" => "global::Doroti.Runtime.HttpClientRequest",
                "HttpClientResponse" => "global::Doroti.Runtime.HttpClientResponse",
                "File" => "global::Doroti.Runtime.DartFile",
                // Dart Never is the bottom type. `dynamic` preserves its ability
                // to inhabit every expression context while the emitted body
                // still terminates by throwing.
                "Never" => "dynamic",
                "Null" => "object",
                "StackTrace" => "global::System.Diagnostics.StackTrace",
                "VoidCallback" => "global::System.Action",
                "GestureTapCallback" or "GestureTapCancelCallback" or
                    "GestureLongPressCallback" or "GestureLongPressCancelCallback" or
                    "GestureLongPressUpCallback" or "GestureDragCancelCallback" => "global::System.Action",
                "GestureDragUpdateCallback" =>
                    "global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>",
                "Function" => "Delegate",
                "pragma" => "object",
                "Invocation" => "global::Doroti.Runtime.Invocation",
                "CreationLocation" => "global::Doroti.Runtime.CreationLocation",
                "Comparator" or "Comparison" => "Comparison",
                "UnimplementedError" => "NotImplementedException",
                "UnsupportedError" => "NotSupportedException",
                "ArgumentError" => "DartArgumentError",
                "Random" => "DartRandom",
                "StateError" => "InvalidOperationException",
                _ => TryMapFunctionType(type) ?? MapNamedType(type),
            };
        }
        if (genericStart < 0 && !IsUnboundTypeParameterName(type) &&
            FindGlobalDeclaration(type) is { } rawGenericDeclaration &&
            rawGenericDeclaration.Element.TypeParameters is { Length: > 0 } rawTypeParameters)
        {
            var rawArguments = rawTypeParameters.Select(parameter =>
                _session.ActiveDeclaration?.Element.CanonicalId == rawGenericDeclaration.Element.CanonicalId
                    ? SafeIdentifier(parameter.Name)
                    : string.IsNullOrWhiteSpace(parameter.Bound) ? "object" : MapType(parameter.Bound));
            mapped += $"<{string.Join(", ", rawArguments)}>";
        }
        return nullable ? mapped + "?" : mapped;
    }

    private string MapNamedType(string type)
    {
        var mappingLibrary = _session.ActiveSourceLibrary ?? _currentLibrary;
        if (type == "MouseTrackerAnnotation" &&
            mappingLibrary.Contains("/rendering/", StringComparison.Ordinal))
        {
            return "global::Doroti.Framework.Services.IMouseTrackerAnnotation";
        }
        // Flutter framework pointer events are owned by package:flutter/gestures.
        // The Services assembly precedes Gestures in the reviewed CLR graph,
        // so its exported callback aliases retain the dart:ui boundary while
        // downstream framework libraries bind the concrete Gestures events.
        if (!mappingLibrary.Contains("/services/", StringComparison.Ordinal) &&
            type is "PointerEvent" or "PointerAddedEvent" or "PointerRemovedEvent" or
                "PointerDownEvent" or "PointerMoveEvent" or "PointerUpEvent" or
                "PointerHoverEvent" or "PointerCancelEvent" or "PointerEnterEvent" or "PointerExitEvent" or
                "PointerSignalEvent" or "PointerScrollEvent" or "PointerScrollInertiaCancelEvent" or
                "PointerScaleEvent" or "PointerPanZoomStartEvent" or "PointerPanZoomUpdateEvent" or
                "PointerPanZoomEndEvent")
        {
            return "global::Doroti.Framework.Gestures." + type;
        }
        if (!_semanticIndex.DeclarationsBySimpleName.TryGetValue(type, out var matches) || matches.Length == 0)
        {
            return EmittedTypeName(mappingLibrary, type);
        }
        var declaration = matches.FirstOrDefault(candidate =>
            string.Equals(LibraryUriFromElementId(candidate.Element.CanonicalId), mappingLibrary, StringComparison.Ordinal))
            ?? matches[0];
        var declarationLibrary = LibraryUriFromElementId(declaration.Element.CanonicalId);
        var emitted = EmittedTypeName(declarationLibrary, declaration.Name);
        var declarationNamespace = FrameworkNamespaceForLibrary(declarationLibrary);
        var currentNamespace = emitted.StartsWith('_')
            ? FrameworkNamespaceForLibrary(_currentLibrary)
            : FrameworkNamespaceForLibrary(mappingLibrary);
        return declarationNamespace is not null && declarationNamespace != currentNamespace
            ? $"global::Doroti.Framework.{declarationNamespace}.{emitted}"
            : emitted;
    }

    private string MapStaticOwnerType(string dartType, CoreResolvedDeclaration activeDeclaration)
    {
        var mapped = MapType(dartType).TrimEnd('?');
        if (mapped.Contains('<', StringComparison.Ordinal)) return mapped;

        var raw = StripLibraryPrefix(dartType).TrimEnd('?');
        var generic = raw.IndexOf('<');
        if (generic >= 0) raw = raw[..generic];
        var owner = FindGlobalDeclaration(raw);
        if (owner?.Element.TypeParameters is not { Length: > 0 } parameters) return mapped;

        var arguments = owner.Element.CanonicalId == activeDeclaration.Element.CanonicalId
            ? parameters.Select(parameter => SafeIdentifier(parameter.Name))
            : parameters.Select(parameter => string.IsNullOrWhiteSpace(parameter.Bound)
                ? "object"
                : MapType(parameter.Bound));
        return mapped + "<" + string.Join(", ", arguments) + ">";
    }

    private string? FrameworkNamespaceForLibrary(string library) => library switch
    {
        var value when value.Contains("/foundation/", StringComparison.Ordinal) => "Foundation",
        var value when value.Contains("/scheduler/", StringComparison.Ordinal) => "Scheduler",
        var value when value.Contains("/services/", StringComparison.Ordinal) => "Services",
        var value when value.Contains("/physics/", StringComparison.Ordinal) => "Physics",
        var value when value.Contains("/animation/", StringComparison.Ordinal) => "Animation",
        var value when value.Contains("/gestures/", StringComparison.Ordinal) => "Gestures",
        var value when value.Contains("/painting/", StringComparison.Ordinal) => "Painting",
        var value when value.Contains("/rendering/", StringComparison.Ordinal) => "Rendering",
        var value when value.Contains("/semantics/", StringComparison.Ordinal) => "Semantics",
        var value when value.Contains("/widgets/", StringComparison.Ordinal) => "Widgets",
        _ => null,
    };

    private string MapGenericArgument(string dartType)
    {
        if (dartType is "Future<dynamic>" or "Future<Object>" or "Future<Object?>") return "Future";
        if (dartType is "dynamic" or "Object" or "Object?") return "object";
        var mapped = MapType(dartType);
        return mapped == "void" ? "object?" : mapped;
    }

    private string MapRecordType(string fieldsText)
    {
        var fields = SplitGenericArguments(fieldsText);
        if (fields.Length == 1 && fields[0].StartsWith('{') && fields[0].EndsWith('}'))
        {
            fields = SplitGenericArguments(fields[0][1..^1]);
        }
        return $"({string.Join(", ", fields.Select(MapRecordTypeField))})";
    }

    private string MapRecordTypeField(string field)
    {
        var value = field.Trim();
        var split = FindLastTopLevelTypeSpace(value);
        if (split < 0) return MapType(value);
        var name = value[(split + 1)..].Trim();
        if (name.Length == 0 || !(char.IsLetter(name[0]) || name[0] == '_') ||
            !name.Skip(1).All(character => char.IsLetterOrDigit(character) || character == '_'))
        {
            return MapType(value);
        }
        return $"{MapType(value[..split].Trim())} {SafeIdentifier(name)}";
    }

    private static int FindLastTopLevelTypeSpace(string value)
    {
        var angle = 0;
        var round = 0;
        var square = 0;
        var curly = 0;
        for (var index = value.Length - 1; index >= 0; index--)
        {
            switch (value[index])
            {
                case '>': angle++; break;
                case '<': angle--; break;
                case ')': round++; break;
                case '(': round--; break;
                case ']': square++; break;
                case '[': square--; break;
                case '}': curly++; break;
                case '{': curly--; break;
            }
            if (char.IsWhiteSpace(value[index]) && angle == 0 && round == 0 && square == 0 && curly == 0)
            {
                return index;
            }
        }
        return -1;
    }

    private string? TryMapFunctionType(string type)
    {
        var functionIndex = FindTopLevelFunctionIndex(type);
        if (functionIndex < 0)
        {
            return null;
        }
        var returnType = type[..functionIndex].Trim();
        var parameterStart = type.IndexOf('(', functionIndex);
        var parameterEnd = type.LastIndexOf(')');
        if (parameterStart < 0 || parameterEnd < 0 || parameterEnd < parameterStart)
        {
            return null;
        }
        var parameterString = type[(parameterStart + 1)..parameterEnd];
        var parameters = string.IsNullOrWhiteSpace(parameterString)
            ? Array.Empty<string>()
            : SplitFunctionParameters(parameterString).Select(NormalizeFunctionParameterType).ToArray();
        if (returnType is "void" or "Null")
        {
            return parameters.Length == 0
                ? "global::System.Action"
                : $"global::System.Action<{string.Join(", ", parameters.Select(MapGenericArgument))}>";
        }
        return parameters.Length == 0
            ? $"global::System.Func<{MapType(returnType)}>"
            : $"global::System.Func<{string.Join(", ", parameters.Select(MapGenericArgument))}, {MapType(returnType)}>";
    }

    private string? TryMapNamedParameterFunctionAlias(string functionType)
    {
        var alias = _semanticIndex.AllDeclarations
            .Where(declaration =>
                declaration.Ast.Kind == CoreNodeKind.GenericTypeAlias &&
                declaration.Element.TypeParameters is not { Length: > 0 } &&
                string.Equals(declaration.Element.Type, functionType, StringComparison.Ordinal) &&
                declaration.Element.Parameters?.Any(parameter =>
                    parameter.Kind.Contains("named", StringComparison.Ordinal)) == true)
            .OrderBy(declaration => declaration.Element.CanonicalId, StringComparer.Ordinal)
            .FirstOrDefault();
        return alias is null
            ? null
            : EmittedTypeName(LibraryUriFromElementId(alias.Element.CanonicalId), alias.Name);
    }

    private static int FindTopLevelFunctionIndex(string value)
    {
        var angle = 0;
        var round = 0;
        var square = 0;
        var curly = 0;
        var result = -1;
        for (var index = 0; index <= value.Length - " Function".Length; index++)
        {
            if (angle == 0 && round == 0 && square == 0 && curly == 0 &&
                value.AsSpan(index).StartsWith(" Function", StringComparison.Ordinal))
            {
                result = index;
            }
            switch (value[index])
            {
                case '<': angle++; break;
                case '>': angle--; break;
                case '(': round++; break;
                case ')': round--; break;
                case '[': square++; break;
                case ']': square--; break;
                case '{': curly++; break;
                case '}': curly--; break;
            }
        }
        return result;
    }

    private IEnumerable<string> SplitFunctionParameters(string value)
    {
        foreach (var parameter in SplitGenericArguments(value))
        {
            var trimmed = parameter.Trim();
            if ((trimmed.StartsWith('{') && trimmed.EndsWith('}')) ||
                (trimmed.StartsWith('[') && trimmed.EndsWith(']')))
            {
                foreach (var grouped in SplitGenericArguments(trimmed[1..^1]))
                {
                    yield return grouped;
                }
            }
            else
            {
                yield return trimmed;
            }
        }
    }

    private string NormalizeFunctionParameterType(string parameter)
    {
        var value = parameter.Trim().TrimStart('{', '[').TrimEnd('}', ']');
        if (value.StartsWith("required ", StringComparison.Ordinal))
        {
            value = value["required ".Length..];
        }
        var equals = value.IndexOf('=');
        if (equals >= 0)
        {
            value = value[..equals].Trim();
        }
        if (value.EndsWith(')') || value.EndsWith(")?", StringComparison.Ordinal))
        {
            return value;
        }
        var depth = 0;
        for (var index = value.Length - 1; index >= 0; index--)
        {
            depth += value[index] switch { '>' or ')' => 1, '<' or '(' => -1, _ => 0 };
            if (depth == 0 && char.IsWhiteSpace(value[index]))
            {
                return value[..index].Trim();
            }
        }
        return value;
    }

    private string StripLibraryPrefix(string type)
    {
        if (type.TrimStart().StartsWith('('))
        {
            // Record fields may each carry their own analyzer library prefix.
            // Stripping at the whole-record level would cut at the last field
            // prefix and turn the field label into a synthetic nominal type.
            return type;
        }
        var lastColon = type.LastIndexOf("::", StringComparison.Ordinal);
        var genericStart = type.IndexOf('<');
        if (lastColon >= 0 && (genericStart < 0 || lastColon < genericStart))
        {
            return type[(lastColon + 2)..].Trim();
        }
        return type;
    }

    private string MapTypeFromAst(CoreAstNode typeNode)
    {
        if (typeNode.Kind == CoreNodeKind.RecordTypeAnnotation)
        {
            var fields = typeNode.Children
                .SelectMany(item => item.Kind switch
                {
                    CoreNodeKind.RecordTypeAnnotationPositionalField or CoreNodeKind.RecordTypeAnnotationNamedField => [item],
                    CoreNodeKind.RecordTypeAnnotationNamedFields => item.Children
                        .Where(child => child.Kind == CoreNodeKind.RecordTypeAnnotationNamedField),
                    _ => [],
                })
                .Select(item =>
                {
                    var fieldType = item.Children.FirstOrDefault(child => child.Category == "type");
                    var mappedType = fieldType is null ? "object" : MapTypeFromAst(fieldType);
                    var fieldName = item.Text(CoreProperty.name);
                    return string.IsNullOrWhiteSpace(fieldName)
                        ? mappedType
                        : $"{mappedType} {SafeIdentifier(fieldName)}";
                })
                .ToArray();
            var tuple = $"({string.Join(", ", fields)})";
            return typeNode.Text(CoreProperty.isNullable) == "true" ? tuple + "?" : tuple;
        }
        if (typeNode.Kind == CoreNodeKind.GenericFunctionType)
        {
            var returnTypeNode = typeNode.Children.FirstOrDefault(item => item.Category == "type");
            var parameterList = typeNode.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.FormalParameterList);
            var parameterTypes = parameterList?.Children
                .Where(item => item.Category == "parameter")
                .Select(item => item.Children.FirstOrDefault(child => child.Category == "type"))
                .Select(item => item is null ? "object" : MapTypeFromAst(item))
                .ToArray() ?? [];
            var returnType = returnTypeNode is null ? "void" : MapTypeFromAst(returnTypeNode);
            if (returnType == "void")
            {
                return parameterTypes.Length == 0
                    ? "global::System.Action"
                    : $"global::System.Action<{string.Join(", ", parameterTypes.Select(type => type == "void" ? "object?" : type))}>";
            }
            return parameterTypes.Length == 0
                ? $"global::System.Func<{returnType}>"
                : $"global::System.Func<{string.Join(", ", parameterTypes.Select(MapGenericArgument))}, {returnType}>";
        }
        var namedType = typeNode.Kind switch
        {
            CoreNodeKind.NamedType => typeNode,
            CoreNodeKind.ConstructorName => typeNode.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.NamedType),
            _ => null
        };
        if (namedType is null)
        {
            return MapType(typeNode.StaticType ?? "object");
        }
        var fallbackName = namedType.Text(CoreProperty.name) ?? "object";
        var typeArguments = namedType.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.TypeArgumentList);
        if (typeArguments is not null && fallbackName is "ValueChanged" or "ValueSetter")
        {
            var callbackArguments = typeArguments.Children
                .Where(item => item.Category == "type")
                .Select(MapTypeFromAst)
                .ToArray();
            if (callbackArguments.Length == 1)
            {
                var callback = $"global::System.Action<{callbackArguments[0]}>";
                return namedType.Text(CoreProperty.isNullable) == "true" ? callback + "?" : callback;
            }
        }
        if (typeArguments is not null &&
            !string.IsNullOrEmpty(namedType.ElementId) &&
            FindGlobalDeclaration(fallbackName) is { Ast.Kind: CoreNodeKind.GenericTypeAlias })
        {
            var aliasArguments = typeArguments.Children
                .Where(item => item.Category == "type")
                .Select(item => item.StaticType ?? item.Text(CoreProperty.name) ?? "object")
                .ToArray();
            var aliasType = $"{fallbackName}<{string.Join(", ", aliasArguments)}>";
            if (namedType.Text(CoreProperty.isNullable) == "true")
            {
                aliasType += "?";
            }
            return MapType(aliasType);
        }
        var name = ResolveEmittedTypeName(namedType, fallbackName);
        if (typeArguments is null)
        {
            if (FindGlobalDeclaration(fallbackName) is { Element.TypeParameters.Length: > 0 } genericDeclaration)
            {
                var ownDeclaration = _session.ActiveDeclaration is { } activeDeclaration &&
                    activeDeclaration.Element.CanonicalId == genericDeclaration.Element.CanonicalId
                    ? activeDeclaration
                    : null;
                var rawArguments = genericDeclaration.Element.TypeParameters.Select(parameter =>
                    ownDeclaration is not null
                        ? SafeIdentifier(parameter.Name)
                        : string.IsNullOrWhiteSpace(parameter.Bound) ? "object" : MapType(parameter.Bound));
                var rawType = $"{fallbackName}<{string.Join(", ", rawArguments)}>";
                if (namedType.Text(CoreProperty.isNullable) == "true")
                {
                    rawType += "?";
                }
                // A raw generic typedef must still be expanded through MapType.
                // Appending CLR type arguments to the already-expanded Func/Action
                // result produces invalid constructs such as Func<...><T>.
                if (genericDeclaration.Ast.Kind == CoreNodeKind.GenericTypeAlias)
                {
                    return MapType(rawType);
                }
                name = ResolveEmittedTypeName(namedType, fallbackName) +
                    $"<{string.Join(", ", rawArguments)}>";
            }
            var resolvedType = namedType.Text(CoreProperty.isNullable) == "true" ? name + "?" : name;
            return name.StartsWith("global::", StringComparison.Ordinal) ? resolvedType : MapType(resolvedType);
        }
        var arguments = typeArguments.Children
            .Where(item => item.Category == "type")
            .Select(MapTypeFromAst)
            .ToArray();
        var type = $"{name}<{string.Join(", ", arguments)}>";
        if (namedType.Text(CoreProperty.isNullable) == "true")
        {
            type += "?";
        }
        return name.StartsWith("global::", StringComparison.Ordinal) ? type : MapType(type);
    }

    private string MapRedirectTargetType(CoreAstNode redirectName, CoreResolvedDeclaration declaration)
    {
        var mapped = MapTypeFromAst(redirectName);
        var typeParameters = FormatTypeParameters(declaration.Element.TypeParameters);
        if (string.IsNullOrEmpty(typeParameters))
        {
            return mapped;
        }
        if (mapped.Contains('<', StringComparison.Ordinal))
        {
            return mapped;
        }
        return mapped + typeParameters;
    }

    private string ResolveEmittedTypeName(CoreAstNode namedType, string fallbackName)
    {
        var elementId = namedType.ElementId;
        if (string.IsNullOrEmpty(elementId))
        {
            return fallbackName;
        }
        var library = LibraryUriFromElementId(elementId);
        var alias = _currentDeclarations?.FirstOrDefault(item =>
            item.Ast.Kind == CoreNodeKind.GenericTypeAlias &&
            string.Equals(item.Element.CanonicalId, elementId, StringComparison.Ordinal));
        if (alias?.Element.Type is { } aliasType)
        {
            return MapType(aliasType);
        }
        if (string.Equals(library, "dart:ui", StringComparison.Ordinal))
        {
            var uiMarker = elementId.LastIndexOf('#');
            var uiSymbol = uiMarker >= 0 ? elementId[(uiMarker + 1)..] : fallbackName;
            var uiTypeName = uiSymbol.Contains('.', StringComparison.Ordinal)
                ? uiSymbol[(uiSymbol.LastIndexOf('.') + 1)..]
                : uiSymbol;
            if (uiTypeName == "TimingsCallback")
            {
                return "global::System.Action<List<FrameTiming>>";
            }
            if (uiTypeName == "VoidCallback")
            {
                return "global::System.Action";
            }
            if (uiTypeName == "FlutterView")
            {
                return "global::Doroti.Ui.DorotiView";
            }
            return "global::Doroti.Ui." + SafeIdentifier(uiTypeName);
        }
        if (library.StartsWith("dart:", StringComparison.Ordinal))
        {
            return fallbackName;
        }
        var marker = elementId.LastIndexOf('#');
        var symbol = marker >= 0 ? elementId[(marker + 1)..] : fallbackName;
        // Type parameters appear as Owner.T — keep the final segment.
        var typeName = symbol.Contains('.', StringComparison.Ordinal)
            ? symbol[(symbol.LastIndexOf('.') + 1)..]
            : symbol;
        return EmittedTypeName(library, typeName);
    }

    private string EmittedTypeName(string libraryUri, string name)
    {
        var safe = SafeIdentifier(name);
        if (safe.StartsWith('_'))
        {
            if (_semanticIndex.FindEmittedDeclaration(safe) is not null)
            {
                // A substituted generic argument can already carry its Dart
                // library suffix (for example _Slot__sliver_resizing_header).
                // Do not append the generic owner's library a second time.
                return safe;
            }
            var file = libraryUri[(libraryUri.LastIndexOf('/') + 1)..];
            var stem = Path.GetFileNameWithoutExtension(file);
            // A private Dart type is disambiguated with its library stem.  The
            // stem itself can be a C# keyword (rendering/object.dart), but '@'
            // is only valid at the beginning of a complete C# identifier.
            var suffix = "__" + SafeIdentifier(stem).TrimStart('@');
            return safe.EndsWith(suffix, StringComparison.Ordinal) ? safe : safe + suffix;
        }
        if (IsPrivateCompanionLibrary(libraryUri) &&
            !libraryUri.EndsWith("/_background_isolate_binary_messenger_io.dart", StringComparison.Ordinal) &&
            !safe.StartsWith('_') &&
            _semanticIndex.HasDeclaration(libraryUri, name))
        {
            return safe + "Io";
        }
        return safe;
    }

}
