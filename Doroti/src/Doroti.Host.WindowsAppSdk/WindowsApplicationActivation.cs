using System.IO.Pipes;
using System.Security.Cryptography;
using System.Text;
using Doroti.Hosting;
using Doroti.Ui;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>Current-user single-instance delivery; protocol registration belongs to the installer.</summary>
internal sealed class WindowsApplicationActivation : IDisposable
{
    private readonly Mutex _mutex;
    private readonly bool _ownsMutex;
    private readonly CancellationTokenSource _lifetime = new();
    private readonly Queue<ApplicationActivation> _pending = new();
    private readonly object _gate = new();
    private Action<ApplicationActivation>? _deliver;
    private readonly Task? _listener;
    private readonly string _pipe;
    public bool Redirected { get; }
    public string? InitialLocation { get; }

    public WindowsApplicationActivation(string applicationId, string scheme, IReadOnlyList<string> arguments)
    {
        if (!Uri.CheckSchemeName(scheme) || scheme is "http" or "https" or "file" or "javascript" or "data")
            throw new ArgumentException("Use an application-specific protocol scheme.", nameof(scheme));
        foreach (var argument in arguments)
            if (Uri.TryCreate(argument, UriKind.Absolute, out var uri) && uri.Scheme.Equals(scheme, StringComparison.OrdinalIgnoreCase))
            {
                ApplicationNavigationHost.ValidateLocation(argument);
                InitialLocation = argument;
                break;
            }
        var identity = applicationId + ":" + Environment.UserName;
        _pipe = "doroti.activation." + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(identity)));
        _mutex = new Mutex(true, "Local\\" + _pipe, out _ownsMutex);
        try
        {
            if (!_ownsMutex)
            {
                if (InitialLocation is null) throw new InvalidOperationException("This application is already running.");
                using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                using var client = new NamedPipeClientStream(".", _pipe, PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly);
                client.ConnectAsync(timeout.Token).GetAwaiter().GetResult();
                var bytes = Encoding.UTF8.GetBytes(InitialLocation);
                client.WriteAsync(BitConverter.GetBytes(bytes.Length), timeout.Token).AsTask().GetAwaiter().GetResult();
                client.WriteAsync(bytes, timeout.Token).AsTask().GetAwaiter().GetResult();
                var ack = new byte[1];
                client.ReadExactlyAsync(ack, timeout.Token).AsTask().GetAwaiter().GetResult();
                if (ack[0] != 1) throw new InvalidOperationException("The running application rejected activation.");
                Redirected = true;
                return;
            }
            _listener = Listen(scheme);
        }
        catch { if (_ownsMutex) _mutex.ReleaseMutex(); _mutex.Dispose(); _lifetime.Dispose(); throw; }
    }

    private async Task Listen(string scheme)
    {
        while (!_lifetime.IsCancellationRequested)
        {
            try
            {
                using var server = new NamedPipeServerStream(_pipe, PipeDirection.InOut, 1,
                    PipeTransmissionMode.Byte, PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly);
                await server.WaitForConnectionAsync(_lifetime.Token).ConfigureAwait(false);
                using var timeout = CancellationTokenSource.CreateLinkedTokenSource(_lifetime.Token);
                timeout.CancelAfter(TimeSpan.FromSeconds(5));
                var header = new byte[4];
                await server.ReadExactlyAsync(header, timeout.Token).ConfigureAwait(false);
                var length = BitConverter.ToInt32(header);
                if (length is < 1 or > 32768) throw new InvalidDataException("Invalid activation length.");
                var bytes = new byte[length];
                await server.ReadExactlyAsync(bytes, timeout.Token).ConfigureAwait(false);
                var location = new UTF8Encoding(false, true).GetString(bytes);
                ApplicationNavigationHost.ValidateLocation(location);
                if (!Uri.TryCreate(location, UriKind.Absolute, out var uri) || !uri.Scheme.Equals(scheme, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException("Unregistered activation scheme.");
                lock (_gate)
                {
                    var activation = new ApplicationActivation(Guid.NewGuid().ToString("N"), location, ApplicationActivationSource.Protocol, false);
                    if (_deliver is { } deliver) deliver(activation);
                    else if (_pending.Count < 32) _pending.Enqueue(activation);
                    else throw new InvalidDataException("Activation queue is full.");
                }
                await server.WriteAsync(new byte[] { 1 }, timeout.Token).ConfigureAwait(false);
            }
            catch (Exception error) when (error is IOException or OperationCanceledException or ArgumentException)
            { if (!_lifetime.IsCancellationRequested) Console.Error.WriteLine("Activation: " + error.Message); }
        }
    }

    public void Attach(Action<ApplicationActivation> deliver)
    {
        lock (_gate)
        {
            _deliver = deliver;
            while (_pending.TryDequeue(out var activation)) deliver(activation);
        }
    }
    public void Detach() { lock (_gate) _deliver = null; }
    public void Dispose()
    {
        _lifetime.Cancel();
        lock (_gate) { _deliver = null; _pending.Clear(); }
        _listener?.GetAwaiter().GetResult();
        _lifetime.Dispose();
        if (_ownsMutex) _mutex.ReleaseMutex();
        _mutex.Dispose();
    }
}
