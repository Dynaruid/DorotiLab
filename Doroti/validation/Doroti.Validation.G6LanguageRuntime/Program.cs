using System.Text.Json;
using Doroti.Generated.Validation.G6LanguageRuntime;

var results = new SortedDictionary<string, string>(StringComparer.Ordinal)
{
    ["nullable-super-defaults"] = Nullable_super_defaultsLibrary.runNullableSuperDefaults(),
    ["constructors-initializers"] = Constructors_initializersLibrary.runConstructorsInitializers(),
    ["generic-variance"] = Generic_varianceLibrary.runGenericVariance(),
    ["future-typed-values"] = await Future_typed_valuesLibrary.runFutureTypedValues(),
    ["null-aware-late-required"] = Null_aware_late_requiredLibrary.runNullAwareLateRequired(upper: true),
    ["member-resolution"] = Member_resolutionLibrary.runMemberResolution(),
    ["tearoffs-callbacks"] = Tearoffs_callbacksLibrary.runTearoffsCallbacks(),
    ["collections-patterns-dynamic"] = Collections_patterns_dynamicLibrary.runCollectionsPatternsDynamic(),
};

Console.WriteLine(JsonSerializer.Serialize(results));
