// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/sensitive_content.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

internal class _ContentSensitivitySetting__sensitive_content
{
    internal virtual long _sensitiveWidgetCount { get; set; } = 0L;
    internal virtual long _autoSensitiveWidgetCount { get; set; } = 0L;
    internal virtual long _notSensitiveWidgetCount { get; set; } = 0L;

    internal _ContentSensitivitySetting__sensitive_content()
    {
    }

    internal static void _reportUnknownContentSensitivityDetected(global::Doroti.Generated.Framework.Services.ContentSensitivity sensitivity)
    {
        FlutterError.reportError(new global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails(exception: global::Doroti.Generated.Framework.Foundation.FlutterError.Create($"SensitiveContent widgets with ContentSensitivity {sensitivity} is unsupported by _ContentSensitivitySetting"), library: "widget library", stack: new global::System.Diagnostics.StackTrace(true)));
    }

    public virtual void addWidgetWithContentSensitivity(global::Doroti.Generated.Framework.Services.ContentSensitivity sensitivity)
    {
        switch (sensitivity)
        {
            case global::Doroti.Generated.Framework.Services.ContentSensitivity.sensitive:
                {
                    _sensitiveWidgetCount++;
                    break;
                }
            case global::Doroti.Generated.Framework.Services.ContentSensitivity.autoSensitive:
                {
                    _autoSensitiveWidgetCount++;
                    break;
                }
            case global::Doroti.Generated.Framework.Services.ContentSensitivity.notSensitive:
                {
                    _notSensitiveWidgetCount++;
                    break;
                }
            default:
                {
                    _ContentSensitivitySetting__sensitive_content._reportUnknownContentSensitivityDetected(sensitivity);
                    break;
                }
        }
    }

    internal static string _getNegativeWidgetCountErrorMessage(global::Doroti.Generated.Framework.Services.ContentSensitivity sensitivity, long count)
    {
        return $"A negative amount ({count}) of {sensitivity} SensitiveContent widgets have been detected, which is not expected. Please file an issue.";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void removeWidgetWithContentSensitivity(global::Doroti.Generated.Framework.Services.ContentSensitivity sensitivity)
    {
        switch (sensitivity)
        {
            case global::Doroti.Generated.Framework.Services.ContentSensitivity.sensitive:
                {
                    _sensitiveWidgetCount--;
                    DartRuntimePrimitives.Assert(() => (this._sensitiveWidgetCount >= 0L), () => (object?)_ContentSensitivitySetting__sensitive_content._getNegativeWidgetCountErrorMessage(sensitivity, this._sensitiveWidgetCount));
                    break;
                }
            case global::Doroti.Generated.Framework.Services.ContentSensitivity.autoSensitive:
                {
                    _autoSensitiveWidgetCount--;
                    DartRuntimePrimitives.Assert(() => (this._autoSensitiveWidgetCount >= 0L), () => (object?)_ContentSensitivitySetting__sensitive_content._getNegativeWidgetCountErrorMessage(sensitivity, this._autoSensitiveWidgetCount));
                    break;
                }
            case global::Doroti.Generated.Framework.Services.ContentSensitivity.notSensitive:
                {
                    _notSensitiveWidgetCount--;
                    DartRuntimePrimitives.Assert(() => (this._notSensitiveWidgetCount >= 0L), () => (object?)_ContentSensitivitySetting__sensitive_content._getNegativeWidgetCountErrorMessage(sensitivity, this._notSensitiveWidgetCount));
                    break;
                }
            default:
                {
                    _ContentSensitivitySetting__sensitive_content._reportUnknownContentSensitivityDetected(sensitivity);
                    break;
                }
        }
    }

    public virtual bool hasWidgets => DartRuntimePrimitives.ConvertValue<bool>((((Math.Max(0L, this._sensitiveWidgetCount) + Math.Max(0L, this._autoSensitiveWidgetCount)) + Math.Max(0L, this._notSensitiveWidgetCount)) > 0L));
    public virtual global::Doroti.Generated.Framework.Services.ContentSensitivity? contentSensitivityBasedOnWidgetCounts
    {
        get
        {
            if ((this._sensitiveWidgetCount > 0L))
            {
                return global::Doroti.Generated.Framework.Services.ContentSensitivity.sensitive;
            }
            if ((this._autoSensitiveWidgetCount > 0L))
            {
                return global::Doroti.Generated.Framework.Services.ContentSensitivity.autoSensitive;
            }
            if ((this._notSensitiveWidgetCount > 0L))
            {
                return global::Doroti.Generated.Framework.Services.ContentSensitivity.notSensitive;
            }
            return ((global::Doroti.Generated.Framework.Services.ContentSensitivity)(object)null);
            return default!;
        }
    }
}

public class SensitiveContentHost
{
    internal virtual bool? _contentSensitivityIsSupported { get; set; } = default;
    private bool __late__contentSensitivitySetting_initialized;
    private _ContentSensitivitySetting__sensitive_content __late__contentSensitivitySetting = default!;
    internal virtual _ContentSensitivitySetting__sensitive_content _contentSensitivitySetting
    {
        get
        {
            if (!__late__contentSensitivitySetting_initialized)
            {
                __late__contentSensitivitySetting = new _ContentSensitivitySetting__sensitive_content();
                __late__contentSensitivitySetting_initialized = true;
            }
            return __late__contentSensitivitySetting;
        }
    }
    internal virtual global::Doroti.Generated.Framework.Services.ContentSensitivity? _fallbackContentSensitivitySetting { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Services.SensitiveContentService _sensitiveContentService { get; private set; } = new global::Doroti.Generated.Framework.Services.SensitiveContentService();
    public static SensitiveContentHost instance = new SensitiveContentHost();

    public SensitiveContentHost()
    {
    }

    public virtual global::Doroti.Generated.Framework.Services.ContentSensitivity? calculatedContentSensitivity => ((_ContentSensitivitySetting__sensitive_content)this._contentSensitivitySetting).contentSensitivityBasedOnWidgetCounts;
    public static Future register(global::Doroti.Generated.Framework.Services.ContentSensitivity desiredSensitivity)
    {
        return ((Future)(object?)instance._register(desiredSensitivity));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal async virtual Future _register(global::Doroti.Generated.Framework.Services.ContentSensitivity desiredSensitivity)
    {
        try
        {
            _contentSensitivityIsSupported ??= await this._sensitiveContentService.isSupported();
        }
        catch (global::Doroti.Generated.Framework.Services.PlatformException e__6181)
        {
            _contentSensitivityIsSupported = false;
            FlutterError.reportError(new global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails(exception: global::Doroti.Generated.Framework.Foundation.FlutterError.Create($"Call to check if setting content sensitivity is supported on the current platform failed unexpectedly, so it is assumed to be unsupported: {e__6181}}}"), library: "widget library", stack: ((((global::Doroti.Generated.Framework.Services.PlatformException)e__6181).stacktrace is null) ? new global::System.Diagnostics.StackTrace(true) : DartRuntimePrimitives.StackTraceFrom(((global::Doroti.Generated.Framework.Services.PlatformException)e__6181).stacktrace!))));
        }
        if (!DartRuntimePrimitives.RequireValue(this._contentSensitivityIsSupported))
        {
            return;
        }
        if ((this._fallbackContentSensitivitySetting is null))
        {
            try
            {
                _fallbackContentSensitivitySetting = await this._sensitiveContentService.getContentSensitivity();
            }
            catch (NotSupportedException e__7279)
            {
                _fallbackContentSensitivitySetting = global::Doroti.Generated.Framework.Services.ContentSensitivity.notSensitive;
                FlutterError.reportError(new global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails(exception: global::Doroti.Generated.Framework.Foundation.FlutterError.Create($"Unknown content sensitivity set in the Android embedding or by default: {e__7279}}}"), library: "widget library", stack: DartRuntimePrimitives.StackTraceFrom(e__7279)));
            }
        }
        global::Doroti.Generated.Framework.Services.ContentSensitivity? contentSensitivityBasedOnWidgetCountsBeforeRegister__8160 = (((_ContentSensitivitySetting__sensitive_content)this._contentSensitivitySetting).contentSensitivityBasedOnWidgetCounts ?? this._fallbackContentSensitivitySetting);
        this._contentSensitivitySetting.addWidgetWithContentSensitivity(desiredSensitivity);
        if ((object.Equals(contentSensitivityBasedOnWidgetCountsBeforeRegister__8160, ((_ContentSensitivitySetting__sensitive_content)this._contentSensitivitySetting).contentSensitivityBasedOnWidgetCounts)))
        {
            return;
        }
        try
        {
            await this._sensitiveContentService.setContentSensitivity(DartRuntimePrimitives.RequireValue(((_ContentSensitivitySetting__sensitive_content)this._contentSensitivitySetting).contentSensitivityBasedOnWidgetCounts));
        }
        catch (global::Doroti.Generated.Framework.Services.PlatformException e__9063)
        {
            FlutterError.reportError(new global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails(exception: global::Doroti.Generated.Framework.Foundation.FlutterError.Create($"Attempt to set {desiredSensitivity} sensitivity failed: {e__9063}}}"), library: "widget library", stack: ((((global::Doroti.Generated.Framework.Services.PlatformException)e__9063).stacktrace is null) ? new global::System.Diagnostics.StackTrace(true) : DartRuntimePrimitives.StackTraceFrom(((global::Doroti.Generated.Framework.Services.PlatformException)e__9063).stacktrace!))));
        }
    }

    public static async Future unregister(global::Doroti.Generated.Framework.Services.ContentSensitivity widgetSensitivity)
    {
        await instance._unregister(widgetSensitivity);
        return;
    }

    internal async virtual Future _unregister(global::Doroti.Generated.Framework.Services.ContentSensitivity widgetSensitivity)
    {
        if ((this._contentSensitivityIsSupported != true))
        {
            return;
        }
        global::Doroti.Generated.Framework.Services.ContentSensitivity contentSensitivityBasedOnWidgetCountsBeforeUnregister__10227 = DartRuntimePrimitives.RequireValue(((_ContentSensitivitySetting__sensitive_content)this._contentSensitivitySetting).contentSensitivityBasedOnWidgetCounts);
        this._contentSensitivitySetting.removeWidgetWithContentSensitivity(widgetSensitivity);
        if (!((_ContentSensitivitySetting__sensitive_content)this._contentSensitivitySetting).hasWidgets)
        {
            if ((object.Equals(contentSensitivityBasedOnWidgetCountsBeforeUnregister__10227, this._fallbackContentSensitivitySetting)))
            {
                return;
            }
            try
            {
                await this._sensitiveContentService.setContentSensitivity(DartRuntimePrimitives.RequireValue(this._fallbackContentSensitivitySetting));
            }
            catch (global::Doroti.Generated.Framework.Services.PlatformException e__11058)
            {
                FlutterError.reportError(new global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails(exception: global::Doroti.Generated.Framework.Foundation.FlutterError.Create($"Attempted to set {this._fallbackContentSensitivitySetting} sensitivity failed: {e__11058}}}"), library: "widget library", stack: ((((global::Doroti.Generated.Framework.Services.PlatformException)e__11058).stacktrace is null) ? new global::System.Diagnostics.StackTrace(true) : DartRuntimePrimitives.StackTraceFrom(((global::Doroti.Generated.Framework.Services.PlatformException)e__11058).stacktrace!))));
            }
            return;
        }
        global::Doroti.Generated.Framework.Services.ContentSensitivity contentSensitivityToRestore__11739 = DartRuntimePrimitives.RequireValue(((_ContentSensitivitySetting__sensitive_content)this._contentSensitivitySetting).contentSensitivityBasedOnWidgetCounts);
        if ((!object.Equals(contentSensitivityToRestore__11739, contentSensitivityBasedOnWidgetCountsBeforeUnregister__10227)))
        {
            try
            {
                await this._sensitiveContentService.setContentSensitivity(contentSensitivityToRestore__11739);
            }
            catch (global::Doroti.Generated.Framework.Services.PlatformException e__12144)
            {
                FlutterError.reportError(new global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails(exception: global::Doroti.Generated.Framework.Foundation.FlutterError.Create($"Attempted to set {this._fallbackContentSensitivitySetting} sensitivity failed: {e__12144}}}"), library: "widget library", stack: ((((global::Doroti.Generated.Framework.Services.PlatformException)e__12144).stacktrace is null) ? new global::System.Diagnostics.StackTrace(true) : DartRuntimePrimitives.StackTraceFrom(((global::Doroti.Generated.Framework.Services.PlatformException)e__12144).stacktrace!))));
            }
        }
    }

}

public class SensitiveContent : StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Services.ContentSensitivity sensitivity { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public SensitiveContent(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Services.ContentSensitivity sensitivity = default!, Widget child = default!) : base(key: key)
    {
        this.sensitivity = sensitivity;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SensitiveContentState__sensitive_content());
}

internal class _SensitiveContentState__sensitive_content : State<SensitiveContent>
{
    internal virtual Future _sensitiveContentRegistrationFuture { get; set; } = Future.value();

    public override void initState()
    {
        base.initState();
        DartRuntimePrimitives.Ignore(_sensitiveContentRegistrationFuture = SensitiveContentHost.register(((SensitiveContent)this.widget).sensitivity));
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Ignore(SensitiveContentHost.unregister(((SensitiveContent)this.widget).sensitivity).catchError(((global::System.Action<object, global::System.Diagnostics.StackTrace>)((exception, stack) => {
FlutterError.reportError(new global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails(exception: exception, stack: stack, library: "widgets library", context: new global::Doroti.Generated.Framework.Foundation.ErrorDescription("while unregistering sensitive content")));
}))));
        base.dispose();
    }

    internal async virtual Future _reregisterWidget(global::Doroti.Generated.Framework.Services.ContentSensitivity oldSensitivity, global::Doroti.Generated.Framework.Services.ContentSensitivity newSensitivity)
    {
        await SensitiveContentHost.register(newSensitivity);
        await SensitiveContentHost.unregister(oldSensitivity);
    }

    public override void didUpdateWidget(SensitiveContent oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((object.Equals(((SensitiveContent)this.widget).sensitivity, ((SensitiveContent)oldWidget).sensitivity)))
        {
            return;
        }
        DartRuntimePrimitives.Ignore(_sensitiveContentRegistrationFuture = _reregisterWidget(((SensitiveContent)oldWidget).sensitivity, ((SensitiveContent)this.widget).sensitivity));
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new FutureBuilder<object?>(future: DartRuntimePrimitives.ConvertValue<Future<object?>>(this._sensitiveContentRegistrationFuture), builder: ((global::System.Func<BuildContext, AsyncSnapshot<object?>, Widget>)((context, snapshot) => {
if ((object.Equals(((AsyncSnapshot<object?>)snapshot).connectionState, ConnectionState.done)))
{
    return ((SensitiveContent)this.widget).child;
}
return ((Widget)(object?)SizedBox.CreateShrink());
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

