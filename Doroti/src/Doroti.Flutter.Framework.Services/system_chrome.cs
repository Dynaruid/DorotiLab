#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/system_chrome.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Services;

public enum DeviceOrientation
{
    portraitUp,
    landscapeLeft,
    portraitDown,
    landscapeRight
}

public class ApplicationSwitcherDescription
{
    public virtual string? label { get; private set; }
    public virtual long? primaryColor { get; private set; }

    public ApplicationSwitcherDescription(string? label = null, long? primaryColor = null)
    {
        this.label = label;
        this.primaryColor = primaryColor;
    }

}

public enum SystemUiOverlay
{
    top,
    bottom
}

public enum SystemUiMode
{
    leanBack,
    immersive,
    immersiveSticky,
    edgeToEdge,
    manual
}

public class SystemUiOverlayStyle : Diagnosticable
{
    public virtual Color? systemNavigationBarColor { get; private set; }
    public virtual Color? systemNavigationBarDividerColor { get; private set; }
    public virtual Brightness? systemNavigationBarIconBrightness { get; private set; }
    public virtual bool? systemNavigationBarContrastEnforced { get; private set; }
    public virtual Color? statusBarColor { get; private set; }
    public virtual Brightness? statusBarBrightness { get; private set; }
    public virtual Brightness? statusBarIconBrightness { get; private set; }
    public virtual bool? systemStatusBarContrastEnforced { get; private set; }
    public static SystemUiOverlayStyle light = new SystemUiOverlayStyle(systemNavigationBarColor: new global::Doroti.Flutter.Ui.Color(4278190080L), systemNavigationBarIconBrightness: Brightness.light, statusBarIconBrightness: Brightness.light, statusBarBrightness: Brightness.dark);
    public static SystemUiOverlayStyle dark = new SystemUiOverlayStyle(systemNavigationBarColor: new global::Doroti.Flutter.Ui.Color(4278190080L), systemNavigationBarIconBrightness: Brightness.light, statusBarIconBrightness: Brightness.dark, statusBarBrightness: Brightness.light);

    public SystemUiOverlayStyle(Color? systemNavigationBarColor = null, Color? systemNavigationBarDividerColor = null, Brightness? systemNavigationBarIconBrightness = null, bool? systemNavigationBarContrastEnforced = null, Color? statusBarColor = null, Brightness? statusBarBrightness = null, Brightness? statusBarIconBrightness = null, bool? systemStatusBarContrastEnforced = null)
    {
        this.systemNavigationBarColor = systemNavigationBarColor;
        this.systemNavigationBarDividerColor = systemNavigationBarDividerColor;
        this.systemNavigationBarIconBrightness = systemNavigationBarIconBrightness;
        this.systemNavigationBarContrastEnforced = systemNavigationBarContrastEnforced;
        this.statusBarColor = statusBarColor;
        this.statusBarBrightness = statusBarBrightness;
        this.statusBarIconBrightness = statusBarIconBrightness;
        this.systemStatusBarContrastEnforced = systemStatusBarContrastEnforced;
    }

    internal virtual DartMap<string, object> _toMap()
    {
        return new DartMap<string, object> { ["systemNavigationBarColor"] = systemNavigationBarColor?.value, ["systemNavigationBarDividerColor"] = systemNavigationBarDividerColor?.value, ["systemStatusBarContrastEnforced"] = systemStatusBarContrastEnforced, ["statusBarColor"] = statusBarColor?.value, ["statusBarBrightness"] = statusBarBrightness?.ToString(), ["statusBarIconBrightness"] = statusBarIconBrightness?.ToString(), ["systemNavigationBarIconBrightness"] = systemNavigationBarIconBrightness?.ToString(), ["systemNavigationBarContrastEnforced"] = systemNavigationBarContrastEnforced };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SystemUiOverlayStyle copyWith(Color? systemNavigationBarColor = null, Color? systemNavigationBarDividerColor = null, bool? systemNavigationBarContrastEnforced = null, Color? statusBarColor = null, Brightness? statusBarBrightness = null, Brightness? statusBarIconBrightness = null, bool? systemStatusBarContrastEnforced = null, Brightness? systemNavigationBarIconBrightness = null)
    {
        return new SystemUiOverlayStyle(systemNavigationBarColor: (systemNavigationBarColor ?? this.systemNavigationBarColor), systemNavigationBarDividerColor: (systemNavigationBarDividerColor ?? this.systemNavigationBarDividerColor), systemNavigationBarContrastEnforced: (systemNavigationBarContrastEnforced ?? this.systemNavigationBarContrastEnforced), statusBarColor: (statusBarColor ?? this.statusBarColor), statusBarIconBrightness: (statusBarIconBrightness ?? this.statusBarIconBrightness), statusBarBrightness: (statusBarBrightness ?? this.statusBarBrightness), systemStatusBarContrastEnforced: (systemStatusBarContrastEnforced ?? this.systemStatusBarContrastEnforced), systemNavigationBarIconBrightness: (systemNavigationBarIconBrightness ?? this.systemNavigationBarIconBrightness));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(systemNavigationBarColor, systemNavigationBarDividerColor, systemNavigationBarContrastEnforced, statusBarColor, statusBarBrightness, statusBarIconBrightness, systemStatusBarContrastEnforced, systemNavigationBarIconBrightness);
    public override bool Equals(object? other)
    {
        var __other = other as SystemUiOverlayStyle;
        if (__other is null) return false;
        if ((!object.Equals(__other.GetType(), this.GetType())))
        {
            return false;
        }
        return (((((((((__other is SystemUiOverlayStyle) && (object.Equals(((SystemUiOverlayStyle)__other).systemNavigationBarColor, systemNavigationBarColor))) && (object.Equals(((SystemUiOverlayStyle)__other).systemNavigationBarDividerColor, systemNavigationBarDividerColor))) && (((SystemUiOverlayStyle)__other).systemNavigationBarContrastEnforced == systemNavigationBarContrastEnforced)) && (object.Equals(((SystemUiOverlayStyle)__other).statusBarColor, statusBarColor))) && (object.Equals(((SystemUiOverlayStyle)__other).statusBarIconBrightness, statusBarIconBrightness))) && (object.Equals(((SystemUiOverlayStyle)__other).statusBarBrightness, statusBarBrightness))) && (((SystemUiOverlayStyle)__other).systemStatusBarContrastEnforced == systemStatusBarContrastEnforced)) && (object.Equals(((SystemUiOverlayStyle)__other).systemNavigationBarIconBrightness, systemNavigationBarIconBrightness)));
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.Add(new DiagnosticsProperty<global::Doroti.Flutter.Ui.Color>("systemNavigationBarColor", systemNavigationBarColor));
        properties.Add(new DiagnosticsProperty<global::Doroti.Flutter.Ui.Color>("systemNavigationBarDividerColor", systemNavigationBarDividerColor));
        properties.Add(new DiagnosticsProperty<global::Doroti.Flutter.Ui.Brightness>("systemNavigationBarIconBrightness", systemNavigationBarIconBrightness));
        properties.Add(new DiagnosticsProperty<bool>("systemNavigationBarContrastEnforced", systemNavigationBarContrastEnforced));
        properties.Add(new DiagnosticsProperty<global::Doroti.Flutter.Ui.Color>("statusBarColor", statusBarColor));
        properties.Add(new DiagnosticsProperty<global::Doroti.Flutter.Ui.Brightness>("statusBarBrightness", statusBarBrightness));
        properties.Add(new DiagnosticsProperty<global::Doroti.Flutter.Ui.Brightness>("statusBarIconBrightness", statusBarIconBrightness));
        properties.Add(new DiagnosticsProperty<bool>("systemStatusBarContrastEnforced", systemStatusBarContrastEnforced));
    }

}

public static partial class System_chromeLibrary
{
    internal static List<string> _stringify<T>(List<T> list) => new List<string>();
}

public abstract class SystemChrome
{
    internal static SystemUiOverlayStyle? _pendingStyle = default;
    internal static SystemUiOverlayStyle? _latestStyle = default;

    public static async Future setPreferredOrientations(List<DeviceOrientation> orientations)
    {
        await SystemChannels.platform.invokeMethod<object?>("SystemChrome.setPreferredOrientations", System_chromeLibrary._stringify(orientations));
    }

    public static async Future setApplicationSwitcherDescription(ApplicationSwitcherDescription description)
    {
        await SystemChannels.platform.invokeMethod<object?>("SystemChrome.setApplicationSwitcherDescription", new DartMap<string, object> { ["label"] = description.label, ["primaryColor"] = description.primaryColor });
    }

    public static async Future setEnabledSystemUIMode(SystemUiMode mode, List<SystemUiOverlay>? overlays = null)
    {
        if ((!object.Equals(mode, SystemUiMode.manual)))
        {
            await SystemChannels.platform.invokeMethod<object?>("SystemChrome.setEnabledSystemUIMode", mode.ToString());
        }
        else
        {
            DartRuntimePrimitives.Assert(() => ((object.Equals(mode, SystemUiMode.manual)) && (overlays is not null)));
            await SystemChannels.platform.invokeMethod<object?>("SystemChrome.setEnabledSystemUIOverlays", System_chromeLibrary._stringify(overlays!));
        }
    }

    public static async Future setSystemUIChangeCallback(Func<bool, Future>? callback)
    {
        ServicesBinding.instance.setSystemUiChangeCallback(callback);
        if ((callback is not null))
        {
            await SystemChannels.platform.invokeMethod<object?>("SystemChrome.setSystemUIChangeListener");
        }
    }

    public static async Future restoreSystemUIOverlays()
    {
        await SystemChannels.platform.invokeMethod<object?>("SystemChrome.restoreSystemUIOverlays");
    }

    public static void setSystemUIOverlayStyle(SystemUiOverlayStyle style)
    {
        if ((_pendingStyle is not null))
        {
            _pendingStyle = style;
            return;
        }
        if ((object.Equals(style, _latestStyle)))
        {
            return;
        }
        _pendingStyle = style;
        DartAsyncRuntime.scheduleMicrotask((() =>
        {
            DartRuntimePrimitives.Assert(() => (_pendingStyle is not null));
            if ((!object.Equals(_pendingStyle, _latestStyle)))
            {
                _ = SystemChannels.platform.invokeMethod<object?>("SystemChrome.setSystemUIOverlayStyle", _pendingStyle!._toMap()).then(((_) =>
                {
                }), onError: ((error, stack) =>
                {
                    FlutterError.reportError(new FlutterErrorDetails(exception: error, stack: stack, library: "services library", context: new ErrorDescription("while setting the system UI overlay style")));
                }));
                _latestStyle = _pendingStyle;
            }
            _pendingStyle = null;
        }));
    }

    public static void handleAppLifecycleStateChanged(AppLifecycleState state)
    {
        if ((object.Equals(state, AppLifecycleState.detached)))
        {
            DartAsyncRuntime.scheduleMicrotask((() =>
            {
                _latestStyle = null;
            }));
        }
    }

    public static SystemUiOverlayStyle? latestStyle => _latestStyle;
}
