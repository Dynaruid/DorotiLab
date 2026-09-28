using System.Collections.Concurrent;
using System.Reflection.Metadata;
using System.Text.Json;
using Doroti.Runtime;
using Doroti.Ui;

[assembly: MetadataUpdateHandler(typeof(Doroti.Framework.Widgets.DorotiHotReload))]

namespace Doroti.Framework.Widgets;

/// <summary>Metadata updates are serialized onto each registered view's owning event queue.</summary>
public static class DorotiHotReload
{
    private static readonly ConcurrentDictionary<Guid, Registration> Views = new();
    public static void ClearCache(Type[]? updatedTypes) { }
    public static void UpdateApplication(Type[]? updatedTypes)
    {
        foreach (var view in Views.Values) view.RequestUpdate();
    }

    public static IDisposable Register(DorotiView view, Func<Task> reassemble)
    {
        DorotiCallbackDispatcher? owner = null;
        view.DispatchPlatformEvent(() => owner = DorotiExecutionContext.CaptureDispatcher());
        var registration = new Registration(view, owner!, reassemble);
        Views.TryAdd(registration.Id, registration);
        registration.Publish("ready", null, null);
        return registration;
    }

    private sealed class OwnerContext(DorotiCallbackDispatcher owner) : SynchronizationContext
    {
        public override void Post(SendOrPostCallback d, object? state)
        {
            owner.TryPost(() =>
            {
                var previous = Current;
                SetSynchronizationContext(this);
                try { d(state); }
                finally { SetSynchronizationContext(previous); }
            });
        }
    }

    private sealed class Registration(DorotiView view, DorotiCallbackDispatcher owner, Func<Task> reassemble) : IDisposable
    {
        private readonly SemaphoreSlim _serial = new(1);
        private readonly CancellationTokenSource _lifetime = new();
        private readonly string? _directory = OperatingSystem.IsBrowser() ? null : Environment.GetEnvironmentVariable("DOROTI_DEV_SESSION");
        private readonly string? _session = Environment.GetEnvironmentVariable("DOROTI_DEV_SESSION_ID");
        private long _revision;
        public Guid Id { get; } = Guid.NewGuid();
        public void RequestUpdate() => _ = ApplyAsync();

        private async Task ApplyAsync()
        {
            string? requestId = null;
            var entered = false;
            try
            {
                await _serial.WaitAsync(_lifetime.Token).ConfigureAwait(false);
                entered = true;
                if (OperatingSystem.IsBrowser()) requestId = DorotiDevelopmentSession.Request(Id.ToString());
                // A partial/stale editor request must not prevent a save-triggered
                // metadata update from refreshing the UI.
                try
                {
                    if (_directory is not null && File.Exists(System.IO.Path.Combine(_directory, "request.json")))
                    {
                        using var request = JsonDocument.Parse(File.ReadAllText(System.IO.Path.Combine(_directory, "request.json")));
                        var root = request.RootElement;
                        if (root.TryGetProperty("runtimeId", out var runtime) && runtime.GetString() == Id.ToString() &&
                            root.TryGetProperty("sessionId", out var session) && session.GetString() == _session &&
                            root.TryGetProperty("requestId", out var id) && id.ValueKind == JsonValueKind.String)
                            requestId = id.GetString();
                    }
                }
                catch (Exception error) when (error is IOException or JsonException or InvalidOperationException)
                { System.Diagnostics.Trace.TraceWarning($"Ignored invalid development request: {error.Message}"); }
                Publish("applying", requestId, null);
                Task? work = null;
                await owner.PostAsync(() =>
                {
                    var previous = SynchronizationContext.Current;
                    // Browser JS proxies require the runtime's JSSynchronizationContext.
                    // It already owns this render thread; replacing it breaks text/JS interop.
                    if (!OperatingSystem.IsBrowser())
                        SynchronizationContext.SetSynchronizationContext(new OwnerContext(owner));
                    try { work = ReassembleCheckedAsync(); }
                    finally { SynchronizationContext.SetSynchronizationContext(previous); }
                }).WaitAsync(_lifetime.Token).ConfigureAwait(false);
                await work!.WaitAsync(TimeSpan.FromSeconds(30), _lifetime.Token).ConfigureAwait(false);
                Interlocked.Increment(ref _revision);
                Publish("applied", requestId, null);
            }
            catch (OperationCanceledException) when (_lifetime.IsCancellationRequested) { }
            catch (Exception error)
            {
                Publish("failed", requestId, error.ToString());
            }
            finally { if (entered) _serial.Release(); }
        }

        private async Task ReassembleCheckedAsync()
        {
            var errors = new ConcurrentQueue<Exception>();
            var previous = Foundation.FlutterError.onError;
            Foundation.FlutterExceptionHandler observer = details =>
            {
                if (errors.IsEmpty) errors.Enqueue(details.exceptionThrown);
                if (previous is not null) previous(details);
                else Foundation.FlutterError.dumpErrorToConsole(details);
            };
            Foundation.FlutterError.onError = observer;
            try
            {
                await reassemble().WaitAsync(TimeSpan.FromSeconds(30), _lifetime.Token);
                if (!errors.IsEmpty) throw new AggregateException("Hot Reload frame failed.", errors);
            }
            finally
            {
                if (ReferenceEquals(Foundation.FlutterError.onError, observer)) Foundation.FlutterError.onError = previous;
            }
        }

        internal void Publish(string status, string? requestId, string? error)
        {
            if (_lifetime.IsCancellationRequested ||
                (!OperatingSystem.IsBrowser() && (_directory is null || _session is null))) return;
            try
            {
                using var stream = new MemoryStream();
                using (var writer = new Utf8JsonWriter(stream))
                {
                    writer.WriteStartObject();
                    writer.WriteString("schemaVersion", "doroti.dev/v1");
                    writer.WriteString("sessionId", _session ?? "browser");
                    writer.WriteString("runtimeId", Id.ToString());
                    writer.WriteNumber("processId", Environment.ProcessId);
                    writer.WriteBoolean("supported", status != "closed" && MetadataUpdater.IsSupported && !Foundation.ConstantsLibrary.kReleaseMode);
                    writer.WriteString("host", view.targetIdentity);
                    writer.WriteNumber("revision", Interlocked.Read(ref _revision));
                    writer.WriteString("status", status);
                    writer.WriteString("requestId", requestId);
                    writer.WriteString("error", error is { Length: > 8000 } ? error[..8000] : error);
                    writer.WriteEndObject();
                }
                var bytes = stream.ToArray();
                if (OperatingSystem.IsBrowser())
                    DorotiDevelopmentSession.Publish(Id.ToString(), System.Text.Encoding.UTF8.GetString(bytes));
                else
                {
                    Directory.CreateDirectory(_directory!);
                    var target = System.IO.Path.Combine(_directory!, "runtime.json");
                    var temporary = target + "." + Id + ".tmp";
                    File.WriteAllBytes(temporary, bytes);
                    File.Move(temporary, target, true);
                }
            }
            catch (Exception diagnosticError) { System.Diagnostics.Trace.TraceError(diagnosticError.ToString()); }
        }
        public void Dispose()
        {
            Publish("closed", null, null);
            Views.TryRemove(Id, out _);
            _lifetime.Cancel();
        }
    }
}
