using Doroti.Framework.Scheduler;
using Doroti.Framework.Widgets;
using Doroti.Runtime;
using Doroti.Ui;

var bytes = new byte[] { 1, 2, 3, 4 };
var snapshot = new GpuEffectParameters(bytes);
bytes[0] = 9;
Require(snapshot.Bytes[0] == 1, "parameter bytes are copied");
var provider = new Provider();
using var controller = new GpuEffectController(snapshot, provider);
var updates = 0;
controller.addListener(() => updates++);
Require(provider.Created!.Starts == 0 && !controller.IsAnimating && updates == 0, "construction is idle");
controller.Start(); controller.Start();
Require(provider.Created.Starts == 1 && controller.IsAnimating, "start is explicit and idempotent");
provider.Created.Fire(new Duration(500_000));
Require(updates == 1 && controller.Parameters.Time == .5f && controller.Parameters.DeltaTime == .5f, "first timing snapshot");
provider.Created.Fire(new Duration(750_000));
Require(updates == 2 && controller.Parameters.Time == .75f && controller.Parameters.DeltaTime == .25f, "delta timing snapshot");
controller.Stop();
Require(!controller.IsAnimating && provider.Created.Stops == 1, "stop ends continuous scheduling");
controller.Update(snapshot);
Require(updates == 3 && provider.Created.Starts == 1, "manual update does not restart animation");
controller.Dispose();
Require(provider.Created.Disposed, "dispose closes ticker");
try { controller.Update(snapshot); throw new Exception("Disposed controller accepted update"); }
catch (ObjectDisposedException) { }
Console.WriteLine("PASS 8 immutable parameter/controller timing and idle contracts");

static void Require(bool value, string message) { if (!value) throw new Exception(message); }

sealed class Provider : TickerProvider
{
    internal FakeTicker? Created;
    public Ticker createTicker(Action<Duration> onTick) => Created = new FakeTicker(onTick);
}
sealed class FakeTicker : Ticker
{
    private readonly Action<Duration> _tick;
    internal FakeTicker(Action<Duration> tick) : base(tick) => _tick = tick;
    private bool _active;
    internal int Starts, Stops;
    internal bool Disposed;
    public override bool isActive => _active;
    public override TickerFuture start() { _active = true; Starts++; return TickerFuture.CreateComplete(); }
    public override void stop(bool canceled = false) { _active = false; Stops++; }
    public override void dispose() { _active = false; Disposed = true; }
    internal void Fire(Duration elapsed) { if (_active) _tick(elapsed); }
}
