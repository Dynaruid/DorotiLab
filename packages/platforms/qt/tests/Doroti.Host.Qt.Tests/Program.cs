using System.IO.Pipes;
using System.Security.Cryptography;
using System.Text;
using Doroti.Host.Qt;

static void Check(bool value, string message)
{
    if (!value) throw new InvalidOperationException(message);
}

// Keep mutex creation/disposal on this thread, as in the actual runner.
var applicationId = "org.doroti.activation-test." + Guid.NewGuid().ToString("N");
var pipe = "doroti.activation." + Convert.ToHexString(
    SHA256.HashData(Encoding.UTF8.GetBytes(applicationId + ":" + Environment.UserName)));
using var primary = new QtApplicationActivation(applicationId, "doroti-test", []);
void Send(byte[] bytes, bool accepted = false)
{
    using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
    using var client = new NamedPipeClientStream(".", pipe, PipeDirection.InOut,
        PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly);
    client.ConnectAsync(timeout.Token).GetAwaiter().GetResult();
    client.WriteAsync(bytes, timeout.Token).AsTask().GetAwaiter().GetResult();
    var ack = new byte[1];
    var count = client.ReadAsync(ack, timeout.Token).AsTask().GetAwaiter().GetResult();
    Check(accepted ? count == 1 && ack[0] == 1 : count == 0, "Unexpected activation acknowledgment.");
}
static byte[] Packet(string location)
{
    var bytes = Encoding.UTF8.GetBytes(location);
    return [.. BitConverter.GetBytes(bytes.Length), .. bytes];
}
Send(BitConverter.GetBytes(0));
Send(BitConverter.GetBytes(32769));
Send([.. BitConverter.GetBytes(1), 0xff]);
Send(Packet("other-scheme://invalid"));
Send(Packet("doroti-test://valid/한글"), accepted: true);
string? received = null;
primary.Attach(value => received = value.Location);
Check(received == "doroti-test://valid/한글", "Malformed input stopped later activation delivery.");
primary.Detach();
for (var i = 0; i < 32; i++) Send(Packet($"doroti-test://queued/{i}"), accepted: true);
Send(Packet("doroti-test://overflow"));
var delivered = 0;
primary.Attach(_ => delivered++);
Send(Packet("doroti-test://after-overflow"), accepted: true);
Check(delivered == 33, "Queue overflow stopped the activation listener.");
unsafe
{
    QtNativeV2.ValidateLayout();
    var bytes = Encoding.UTF8.GetBytes(applicationId);
    fixed (byte* data = bytes)
    {
        var configuration = new QtNativeV2.Configuration(new(data, (ulong)bytes.Length),
            new(data, (ulong)bytes.Length), 400, 300, 1, 0);
        Check(configuration.ApplicationId.Length == (ulong)bytes.Length
            && (configuration.RequiredFeatures & (1UL << 25)) != 0,
            "Application identity was omitted from the negotiated configuration.");
    }
    var api = default(QtNativeV2.HostApi);
    *((ulong*)((byte*)&api + 8)) = 1UL << 19; // Native activation is otherwise required.
    var host = new QtHostAdapter(0, in api, 400, 300, 1);
    host.ApplyLifecycle((uint)Doroti.Ui.AppLifecycleState.detached);
    // No native module is loaded: an activation of the destroyed view would fail.
    host.RequestFocus(Doroti.Ui.ViewFocusState.focused, Doroti.Ui.ViewFocusDirection.forward);
    host.Dispose();
}
Console.WriteLine("PASS: activation rejects malformed input/overflow, recovers for later requests, disposes cleanly, negotiates application identity, and skips focus on detached views.");
