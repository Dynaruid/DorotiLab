// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/sensitive_content.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

internal class _ContentSensitivitySetting__sensitive_content
{
    internal virtual long _sensitiveWidgetCount { get; set; } = 0L;
    internal virtual long _autoSensitiveWidgetCount { get; set; } = 0L;
    internal virtual long _notSensitiveWidgetCount { get; set; } = 0L;

    internal _ContentSensitivitySetting__sensitive_content()
    {
    }

    internal static void _reportUnknownContentSensitivityDetected(global::Doroti.Framework.Services.ContentSensitivity sensitivity)
    {
        FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: FlutterError.Create($"SensitiveContent widgets with ContentSensitivity {sensitivity} is unsupported by _ContentSensitivitySetting"), library: "widget library", stack: new global::System.Diagnostics.StackTrace(true)));
    }

    public virtual void addWidgetWithContentSensitivity(global::Doroti.Framework.Services.ContentSensitivity sensitivity)
    {
        switch (sensitivity)
        {
            case ContentSensitivity.sensitive:
                {
                    _sensitiveWidgetCount++;
                    break;
                }
            case ContentSensitivity.autoSensitive:
                {
                    _autoSensitiveWidgetCount++;
                    break;
                }
            case ContentSensitivity.notSensitive:
                {
                    _notSensitiveWidgetCount++;
                    break;
                }
            default:
                {
                    _reportUnknownContentSensitivityDetected(sensitivity);
                    break;
                }
        }
    }

    internal static string _getNegativeWidgetCountErrorMessage(global::Doroti.Framework.Services.ContentSensitivity sensitivity, long count)
    {
        return $"A negative amount ({count}) of {sensitivity} SensitiveContent widgets have been detected, which is not expected. Please file an issue.";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void removeWidgetWithContentSensitivity(global::Doroti.Framework.Services.ContentSensitivity sensitivity)
    {
        switch (sensitivity)
        {
            case ContentSensitivity.sensitive:
                {
                    _sensitiveWidgetCount--;
                    DartRuntimePrimitives.Assert(() => _sensitiveWidgetCount >= 0L, () => (object?)_getNegativeWidgetCountErrorMessage(sensitivity, _sensitiveWidgetCount));
                    break;
                }
            case ContentSensitivity.autoSensitive:
                {
                    _autoSensitiveWidgetCount--;
                    DartRuntimePrimitives.Assert(() => _autoSensitiveWidgetCount >= 0L, () => (object?)_getNegativeWidgetCountErrorMessage(sensitivity, _autoSensitiveWidgetCount));
                    break;
                }
            case ContentSensitivity.notSensitive:
                {
                    _notSensitiveWidgetCount--;
                    DartRuntimePrimitives.Assert(() => _notSensitiveWidgetCount >= 0L, () => (object?)_getNegativeWidgetCountErrorMessage(sensitivity, _notSensitiveWidgetCount));
                    break;
                }
            default:
                {
                    _reportUnknownContentSensitivityDetected(sensitivity);
                    break;
                }
        }
    }

    public virtual bool hasWidgets => DartRuntimePrimitives.ConvertValue<bool>((Math.Max(0L, _sensitiveWidgetCount) + Math.Max(0L, _autoSensitiveWidgetCount) + Math.Max(0L, _notSensitiveWidgetCount)) > 0L);
    public virtual global::Doroti.Framework.Services.ContentSensitivity? contentSensitivityBasedOnWidgetCounts
    {
        get
        {
            if (_sensitiveWidgetCount > 0L)
            {
                return ContentSensitivity.sensitive;
            }
            if (_autoSensitiveWidgetCount > 0L)
            {
                return ContentSensitivity.autoSensitive;
            }
            if (_notSensitiveWidgetCount > 0L)
            {
                return ContentSensitivity.notSensitive;
            }
            return null;
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
    internal virtual global::Doroti.Framework.Services.ContentSensitivity? _fallbackContentSensitivitySetting { get; set; } = default;
    internal virtual global::Doroti.Framework.Services.SensitiveContentService _sensitiveContentService { get; private set; } = new global::Doroti.Framework.Services.SensitiveContentService();
    public static SensitiveContentHost instance = new SensitiveContentHost();

    public SensitiveContentHost()
    {
    }

    public virtual global::Doroti.Framework.Services.ContentSensitivity? calculatedContentSensitivity => _contentSensitivitySetting.contentSensitivityBasedOnWidgetCounts;
    public static Future register(global::Doroti.Framework.Services.ContentSensitivity desiredSensitivity)
    {
        return instance._register(desiredSensitivity);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal async virtual Future _register(global::Doroti.Framework.Services.ContentSensitivity desiredSensitivity)
    {
        try
        {
            _contentSensitivityIsSupported ??= await _sensitiveContentService.isSupported();
        }
        catch (global::Doroti.Framework.Services.PlatformException e)
        {
            _contentSensitivityIsSupported = false;
            FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: FlutterError.Create($"Call to check if setting content sensitivity is supported on the current platform failed unexpectedly, so it is assumed to be unsupported: {e}}}"), library: "widget library", stack: (e.stacktrace is null) ? new global::System.Diagnostics.StackTrace(true) : DartRuntimePrimitives.StackTraceFrom(e.stacktrace!)));
        }
        if (!DartRuntimePrimitives.RequireValue(_contentSensitivityIsSupported))
        {
            return;
        }
        if (_fallbackContentSensitivitySetting is null)
        {
            try
            {
                _fallbackContentSensitivitySetting = await _sensitiveContentService.getContentSensitivity();
            }
            catch (NotSupportedException eLocal)
            {
                _fallbackContentSensitivitySetting = ContentSensitivity.notSensitive;
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: FlutterError.Create($"Unknown content sensitivity set in the Android embedding or by default: {eLocal}}}"), library: "widget library", stack: DartRuntimePrimitives.StackTraceFrom(eLocal)));
            }
        }
        global::Doroti.Framework.Services.ContentSensitivity? contentSensitivityBasedOnWidgetCountsBeforeRegister = _contentSensitivitySetting.contentSensitivityBasedOnWidgetCounts ?? _fallbackContentSensitivitySetting;
        _contentSensitivitySetting.addWidgetWithContentSensitivity(desiredSensitivity);
        if (Equals(contentSensitivityBasedOnWidgetCountsBeforeRegister, _contentSensitivitySetting.contentSensitivityBasedOnWidgetCounts))
        {
            return;
        }
        try
        {
            await _sensitiveContentService.setContentSensitivity(DartRuntimePrimitives.RequireValue(_contentSensitivitySetting.contentSensitivityBasedOnWidgetCounts));
        }
        catch (global::Doroti.Framework.Services.PlatformException eAlternate)
        {
            FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: FlutterError.Create($"Attempt to set {desiredSensitivity} sensitivity failed: {eAlternate}}}"), library: "widget library", stack: (eAlternate.stacktrace is null) ? new global::System.Diagnostics.StackTrace(true) : DartRuntimePrimitives.StackTraceFrom(eAlternate.stacktrace!)));
        }
    }

    public static async Future unregister(global::Doroti.Framework.Services.ContentSensitivity widgetSensitivity)
    {
        await instance._unregister(widgetSensitivity);
        return;
    }

    internal async virtual Future _unregister(global::Doroti.Framework.Services.ContentSensitivity widgetSensitivity)
    {
        if (_contentSensitivityIsSupported != true)
        {
            return;
        }
        global::Doroti.Framework.Services.ContentSensitivity contentSensitivityBasedOnWidgetCountsBeforeUnregister = DartRuntimePrimitives.RequireValue(_contentSensitivitySetting.contentSensitivityBasedOnWidgetCounts);
        _contentSensitivitySetting.removeWidgetWithContentSensitivity(widgetSensitivity);
        if (!_contentSensitivitySetting.hasWidgets)
        {
            if (Equals(contentSensitivityBasedOnWidgetCountsBeforeUnregister, _fallbackContentSensitivitySetting))
            {
                return;
            }
            try
            {
                await _sensitiveContentService.setContentSensitivity(DartRuntimePrimitives.RequireValue(_fallbackContentSensitivitySetting));
            }
            catch (global::Doroti.Framework.Services.PlatformException e)
            {
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: FlutterError.Create($"Attempted to set {_fallbackContentSensitivitySetting} sensitivity failed: {e}}}"), library: "widget library", stack: (e.stacktrace is null) ? new global::System.Diagnostics.StackTrace(true) : DartRuntimePrimitives.StackTraceFrom(e.stacktrace!)));
            }
            return;
        }
        global::Doroti.Framework.Services.ContentSensitivity contentSensitivityToRestore = DartRuntimePrimitives.RequireValue(_contentSensitivitySetting.contentSensitivityBasedOnWidgetCounts);
        if (!Equals(contentSensitivityToRestore, contentSensitivityBasedOnWidgetCountsBeforeUnregister))
        {
            try
            {
                await _sensitiveContentService.setContentSensitivity(contentSensitivityToRestore);
            }
            catch (global::Doroti.Framework.Services.PlatformException eLocal)
            {
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: FlutterError.Create($"Attempted to set {_fallbackContentSensitivitySetting} sensitivity failed: {eLocal}}}"), library: "widget library", stack: (eLocal.stacktrace is null) ? new global::System.Diagnostics.StackTrace(true) : DartRuntimePrimitives.StackTraceFrom(eLocal.stacktrace!)));
            }
        }
    }

}

public class SensitiveContent : StatefulWidget
{
    public virtual global::Doroti.Framework.Services.ContentSensitivity sensitivity { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public SensitiveContent(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Services.ContentSensitivity sensitivity = default!, Widget child = default!) : base(key: key)
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
        DartRuntimePrimitives.Ignore(_sensitiveContentRegistrationFuture = SensitiveContentHost.register(widget.sensitivity));
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Ignore(SensitiveContentHost.unregister(widget.sensitivity).catchError((exception, stack) =>
        {
            FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exception, stack: stack, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while unregistering sensitive content")));
        }));
        base.dispose();
    }

    internal async virtual Future _reregisterWidget(global::Doroti.Framework.Services.ContentSensitivity oldSensitivity, global::Doroti.Framework.Services.ContentSensitivity newSensitivity)
    {
        await SensitiveContentHost.register(newSensitivity);
        await SensitiveContentHost.unregister(oldSensitivity);
    }

    public override void didUpdateWidget(SensitiveContent oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (Equals(widget.sensitivity, oldWidget.sensitivity))
        {
            return;
        }
        DartRuntimePrimitives.Ignore(_sensitiveContentRegistrationFuture = _reregisterWidget(oldWidget.sensitivity, widget.sensitivity));
    }

    public override Widget build(BuildContext context)
    {
        return new FutureBuilder<object?>(future: DartRuntimePrimitives.ConvertValue<Future<object?>>(_sensitiveContentRegistrationFuture), builder: (context, snapshot) =>
        {
            if (Equals(snapshot.connectionState, ConnectionState.done))
            {
                return widget.child;
            }
            return SizedBox.CreateShrink();
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

