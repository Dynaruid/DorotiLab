using Doroti.Hosting;
using Doroti.Ui;

static void Require(bool value, string message) { if (!value) throw new Exception(message); }
var queue = new Queue<Action>();
void Drain() { while (queue.TryDequeue(out var action)) action(); }
using var receiver = new OsDropReceiver(new(true, false, OsDropAction.Copy,
    [OsDropFormats.Files, OsDropFormats.Text, OsDropFormats.UriList]), queue.Enqueue);
var events = new List<OsDropEvent>();
var options = new OsDropOptions(OsDropAction.Copy, [OsDropFormats.Files, OsDropFormats.Text], Rect.fromLTWH(10, 20, 100, 80));
using var registration = receiver.Register(options, events.Add);
var offer = new OsDropOffer(new Offset(30, 40), [OsDropFormats.Files, OsDropFormats.UriList], OsDropAction.Copy | OsDropAction.Move);
Require(receiver.Hover(OsDropPhase.Enter, offer) == OsDropAction.Copy && events.Count == 0, "Callbacks ran on native stack.");
Require(receiver.Hover(OsDropPhase.Over, offer with { RequestedAction = OsDropAction.Move }) == OsDropAction.None, "Unsupported move accepted.");
Require(receiver.Hover(OsDropPhase.Over, offer with { Position = new Offset(9, 40) }) == OsDropAction.None, "Outside region accepted.");
Require(receiver.Hover(OsDropPhase.Over, offer with { Formats = ["unknown/type"] }) == OsDropAction.None, "Unknown type accepted.");
receiver.Hover(OsDropPhase.Leave, offer);
Require(queue.Count == 3, "Over updates were not coalesced.");
Drain();
Require(events[0].Phase == OsDropPhase.Enter && events[^1].Phase == OsDropPhase.Leave, "Event ordering changed.");
events.Clear();
receiver.Hover(OsDropPhase.Over, offer);
receiver.Hover(OsDropPhase.Leave, offer);
receiver.Hover(OsDropPhase.Enter, offer with { Position = new Offset(50, 60) });
receiver.Hover(OsDropPhase.Over, offer with { Position = new Offset(55, 65) });
Drain();
Require(events.Count == 4 && events[0].Offer.Position == offer.Position && events[^1].Offer.Position.dx == 55,
    "Coalescing crossed a leave/enter boundary.");
var file = new TestFile();
var payload = new OsDropData([file]);
Require(receiver.Drop(offer, formats => { Require(formats.SequenceEqual([OsDropFormats.Files]), "Unaccepted formats exposed."); return payload; }) == OsDropAction.Copy, "Copy drop rejected.");
Drain();
Require(events[^1].Data == payload && events[^1].Offer.Position == offer.Position, "Payload/coordinates changed.");
var buffer = new byte[8];
Require(await file.ReadAsync(4_000_000_000, buffer) == 8, "64-bit offset lost.");
payload.Dispose();
Require(file.Disposed && payload.Lifetime.IsCancellationRequested, "Explicit disposal did not revoke grant.");
Require(receiver.Drop(offer, _ => throw new UnauthorizedAccessException("denied")) == OsDropAction.None, "Failed acquisition reported success.");
Drain();
Require(events[^1].Failure == OsDropFailure.Denied, "Permission failure was not reported.");
var neverAcquire = false;
receiver.Drop(offer with { RequestedAction = OsDropAction.Link }, _ => { neverAcquire = true; return new(); });
Require(!neverAcquire, "Data acquired for a rejected action.");
var late = new TestFile();
receiver.Drop(offer, _ => new([late]));
var before = events.Count;
registration.Dispose();
Drain();
Require(late.Disposed && events.Count == before, "Unmount delivered a queued payload or leaked a file.");
using var second = receiver.Register(options, events.Add);
second.Update(options with { Bounds = Rect.fromLTWH(0, 0, 10, 10) });
Require(receiver.Hover(OsDropPhase.Over, offer) == OsDropAction.None, "Updated bounds ignored.");
try { second.Update(options with { Actions = OsDropAction.Move }); throw new Exception("Move capability accepted."); }
catch (NotSupportedException) { }
receiver.Dispose();
Drain();
try { receiver.Register(options, events.Add); throw new Exception("Closed owner accepted a receiver."); }
catch (ObjectDisposedException) { }
Console.WriteLine("PASS: asynchronous enter/over/leave/drop; formats/actions/regions; 64-bit reads; denied acquisition; queued-drop/unmount/owner lifetime.");

sealed class TestFile : IPickedFile
{
    public bool Disposed;
    public string Name => "large.bin";
    public long Length => 5_000_000_000;
    public ValueTask<int> ReadAsync(long offset, Memory<byte> buffer, CancellationToken cancellationToken = default)
    { ObjectDisposedException.ThrowIf(Disposed, this); cancellationToken.ThrowIfCancellationRequested(); return ValueTask.FromResult(buffer.Length); }
    public void Dispose() => Disposed = true;
}
