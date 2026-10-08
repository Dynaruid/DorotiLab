#if ANDROID
using System.Collections;
using System.Collections.Concurrent;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Doroti.Framework.Services;
using Doroti.Runtime;
using Doroti.Ui;
using Microsoft.Maui.ApplicationModel;

namespace Doroti.Host.Maui;

internal sealed class AndroidProcessTextChannel(IPlatformMessageHostCapability fallback)
    : IPlatformMessageHostCapability
{
    private readonly StandardMethodCodec _codec = new(new StandardMessageCodec());

    public async ValueTask<ReadOnlyMemory<byte>?> SendAsync(string channel,
        ReadOnlyMemory<byte>? data, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (channel != "flutter/processtext" || data is null)
            return await fallback.SendAsync(channel, data, cancellationToken);
        var call = _codec.decodeMethodCall((ByteData)data.Value);
        try
        {
            if (call.method == "ProcessText.queryTextActions")
            {
                var actions = await MainThread.InvokeOnMainThreadAsync(() => QueryActions()
                    .ToDictionary(entry => entry.Key, entry => entry.Value.Label));
                return _codec.encodeSuccessEnvelope(actions).asMemory();
            }
            if (call.method == "ProcessText.processTextAction")
            {
                if (call.arguments is not IList args || args.Count != 3
                    || args[0] is not string id || args[1] is not string text || args[2] is not bool readOnly)
                    throw new ArgumentException("Expected text action ID, text and readOnly.");
                var component = await MainThread.InvokeOnMainThreadAsync(() =>
                    QueryActions().TryGetValue(id, out var action) ? action.Component
                        : throw new InvalidOperationException("Text processing activity is unavailable."));
                var result = await AndroidProcessTextActivity.Process(component, text, readOnly, cancellationToken);
                return _codec.encodeSuccessEnvelope(result).asMemory();
            }
            return await fallback.SendAsync(channel, data, cancellationToken);
        }
        catch (System.OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
        catch (Exception error)
        {
            return _codec.encodeErrorEnvelope("process-text-failed", error.Message, null).asMemory();
        }
    }

    private static Dictionary<string, (ComponentName Component, string Label)> QueryActions()
    {
        var manager = Android.App.Application.Context.PackageManager!;
        using var intent = new Intent(Intent.ActionProcessText);
        intent.SetType("text/plain");
        var result = new Dictionary<string, (ComponentName, string)>();
        foreach (var info in manager.QueryIntentActivities(intent, (PackageInfoFlags)0) ?? [])
        {
            var activity = info.ActivityInfo;
            if (activity is not { Exported: true, Enabled: true } || activity.PackageName is null || activity.Name is null)
                continue;
            if (activity.Permission is { Length: > 0 } permission
                && Android.App.Application.Context.CheckSelfPermission(permission) != Permission.Granted)
                continue;
            var component = new ComponentName(activity.PackageName, activity.Name);
            result[component.FlattenToString()!] = (component, info.LoadLabel(manager)?.ToString() ?? activity.Name);
        }
        return result;
    }

    public void SetMessageHandler(string channel, PlatformMessageHandler? handler) =>
        fallback.SetMessageHandler(channel, handler);
}

// A private relay keeps result delivery independent of the app's Activity subclass.
[Activity(Exported = false, Theme = "@android:style/Theme.Translucent.NoTitleBar",
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
public sealed class AndroidProcessTextActivity : Activity
{
    private const int RequestCode = 4702;
    private sealed class Request(ComponentName component, string text, bool readOnly)
    {
        public ComponentName Component { get; } = component;
        public string Text { get; } = text;
        public bool ReadOnly { get; } = readOnly;
        public TaskCompletionSource<string?> Completion { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public AndroidProcessTextActivity? Activity;
    }
    private static readonly ConcurrentDictionary<string, Request> Requests = new();
    private string? _id;

    internal static async Task<string?> Process(ComponentName component, string text, bool readOnly, CancellationToken token)
    {
        var id = Guid.NewGuid().ToString("N");
        var request = new Request(component, text, readOnly);
        Requests[id] = request;
        using var cancellation = token.Register(() => MainThread.BeginInvokeOnMainThread(() =>
        {
            request.Completion.TrySetCanceled(token);
            request.Activity?.FinishActivity(RequestCode);
            request.Activity?.Finish();
        }));
        try
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                token.ThrowIfCancellationRequested();
                var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity
                    ?? throw new InvalidOperationException("No foreground Android activity.");
                using var intent = new Intent(activity, typeof(AndroidProcessTextActivity));
                intent.PutExtra("doroti.text.request", id);
                activity.StartActivity(intent);
            });
            return await request.Completion.Task;
        }
        finally { Requests.TryRemove(id, out _); }
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        _id = Intent?.GetStringExtra("doroti.text.request");
        if (_id is null || !Requests.TryGetValue(_id, out var request) || request.Completion.Task.IsCompleted)
        { Finish(); return; }
        request.Activity = this;
        if (savedInstanceState is not null) return;
        try
        {
            using var intent = new Intent(Intent.ActionProcessText);
            intent.SetType("text/plain");
            intent.SetComponent(request.Component);
            intent.PutExtra(Intent.ExtraProcessText, request.Text);
            intent.PutExtra(Intent.ExtraProcessTextReadonly, request.ReadOnly);
            StartActivityForResult(intent, RequestCode);
        }
        catch (Exception error) { request.Completion.TrySetException(error); Finish(); }
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        if (requestCode != RequestCode) return;
        if (_id is not null && Requests.TryGetValue(_id, out var request))
            request.Completion.TrySetResult(resultCode == Result.Ok ? data?.GetStringExtra(Intent.ExtraProcessText) : null);
        Finish();
    }

    protected override void OnDestroy()
    {
        if (_id is not null && Requests.TryGetValue(_id, out var request) && ReferenceEquals(request.Activity, this))
        {
            request.Activity = null;
            if (!IsChangingConfigurations) request.Completion.TrySetResult(null);
        }
        base.OnDestroy();
    }
}
#endif
