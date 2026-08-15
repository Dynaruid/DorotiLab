namespace Doroti.DartToCSharp;

internal enum FlutterBinding
{
    PlatformDispatcher,
    Window,
    Locale,
    SchedulerBinding,
    ServicesBinding,
}

internal sealed record FlutterBindingRule(SymbolId Source, FlutterBinding Binding);

internal static class FlutterBindingRegistry
{
    private static readonly IReadOnlyDictionary<SymbolId, FlutterBinding> Bindings =
        new Dictionary<SymbolId, FlutterBinding>
        {
            [SymbolId.Parse("dart:ui#PlatformDispatcher")] = FlutterBinding.PlatformDispatcher,
            [SymbolId.Parse("dart:ui#DorotiView")] = FlutterBinding.Window,
            [SymbolId.Parse("dart:ui#Locale")] = FlutterBinding.Locale,
            [SymbolId.Parse("package:flutter/scheduler.dart#SchedulerBinding")] = FlutterBinding.SchedulerBinding,
            [SymbolId.Parse("package:flutter/services.dart#ServicesBinding")] = FlutterBinding.ServicesBinding,
        };

    public static FlutterBinding? Resolve(SymbolId source) => Bindings.TryGetValue(source, out var binding)
        ? binding
        : null;
}
