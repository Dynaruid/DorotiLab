using Doroti.Framework.Widgets;
using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Ui;
using Aspect = Doroti.Framework.Widgets._MediaQueryAspect__media_query;

if (args is ["--execution-cost", var costOutput])
{
    ExecutionCostContracts.Verify(costOutput);
    PersistentMapContracts.Verify();
    SameWorkContracts.Verify(costOutput + ".trace.json");
    return;
}

if (args is ["--same-work", var output])
{
    PersistentMapContracts.Verify();
    SameWorkContracts.Verify(output);
    return;
}

var baseline = new MediaQueryData(size: new Size(800, 600));
void Require(bool value, string message)
{
    if (!value) throw new InvalidOperationException(message);
    Console.WriteLine($"PASS {message}");
}
bool Notify(MediaQueryData next, params Aspect[] aspects) =>
    new MediaQuery(data: next, child: new SizedBox()).updateShouldNotifyDependent(
        new MediaQuery(data: baseline, child: new SizedBox()), new HashSet<Aspect>(aspects));

Require(!Notify(baseline, Enum.GetValues<Aspect>()), "identical metrics do not notify any aspect");
Require(Notify(baseline.copyWith(size: new Size(801,600)), Aspect.width), "width dependent notified");
Require(!Notify(baseline.copyWith(size: new Size(801,600)), Aspect.height), "width does not notify height dependent");
Require(Notify(baseline.copyWith(size: new Size(800,601)), Aspect.height), "height dependent notified");
Require(!Notify(baseline.copyWith(size: new Size(800,601)), Aspect.width), "height does not notify width dependent");
Require(Notify(baseline.copyWith(devicePixelRatio: 2), Aspect.devicePixelRatio), "same size DPR change notified");
Require(Notify(baseline.copyWith(viewInsets: EdgeInsets.CreateAll(10)), Aspect.viewInsets), "same size insets notified");
Require(Notify(baseline.copyWith(textScaleFactor: 1.5), Aspect.textScaleFactor), "same size text scale notified");
Require(Notify(baseline.copyWith(accessibleNavigation: true), Aspect.accessibleNavigation), "same size accessibility notified");
Console.WriteLine($"OBSERVATION equal-content new displayFeatures list notifies={Notify(baseline.copyWith(displayFeatures: []), Aspect.displayFeatures)}");

// A numeric-only subscription exercises the per-dependent hot path without
// value-type Equals boxing elsewhere. Inputs are all prepared outside timing.
var previousQuery = new MediaQuery(data: baseline, child: new SizedBox());
var nextQuery = new MediaQuery(data: baseline.copyWith(size: new Size(801,600)), child: new SizedBox());
var independentAspects = new HashSet<Aspect> { Aspect.height, Aspect.devicePixelRatio };
for (var i = 0; i < 1000; i++) nextQuery.updateShouldNotifyDependent(previousQuery, independentAspects);
var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
for (var i = 0; i < 10000; i++)
    if (nextQuery.updateShouldNotifyDependent(previousQuery, independentAspects)) throw new Exception("unrelated notification");
var allocated = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;
Console.WriteLine($"OBSERVATION CLR numeric-aspect loop allocatedBytes={allocated}; WASM allocation not inferred");
Require(allocated == 0, "numeric aspect checks allocate no temporary collections or boxed enums after warmup");

var child = new LayoutProbe();
child.layout(new BoxConstraints(maxWidth: 800, maxHeight: 600));
child.layout(new BoxConstraints(maxWidth: 800, maxHeight: 600));
Require(child.LayoutCount == 1, "equal constraints use existing fast path");
child.markNeedsLayout();
child.layout(new BoxConstraints(maxWidth: 800, maxHeight: 600));
Require(child.LayoutCount == 2, "explicit invalidation preserves layout with equal constraints");

Require(FrameworkWorkCounters.Enabled, "diagnostic test explicitly enabled");
var trace = new DorotiFrameTrace();
for (var i = 0; i < 8194; i++) {
    FrameworkWorkCounters.Add(FrameworkWork.NewPicture);
    trace.Record(DorotiFramePhase.build, 42, DorotiFrameClock.Now, frameworkFrameNumber: i);
}
var snapshot = FrameworkWorkCounters.Snapshot();
Require(snapshot.Samples.Length == 8192 && snapshot.Dropped == 2, "bounded ring reports overwritten samples");
Require(snapshot.Samples[0].Boundary.Frame == 2 && snapshot.Samples[^1].Boundary.Frame == 8193,
    "ring wrap preserves chronological frame identity");
Require(snapshot.Samples[^1].Totals[(int)FrameworkWork.NewPicture] - snapshot.Samples[0].Totals[(int)FrameworkWork.NewPicture] == 8191,
    "cumulative deltas survive ring wrap");

// Profile scopes must unwind on exceptions and remain owned by their UI thread.
using (FrameworkWorkProfile.Begin(typeof(LayoutProbe), 99))
{
    try { using var nested = FrameworkWorkProfile.Begin(typeof(LayoutProbe), 100); throw new InvalidOperationException(); }
    catch (InvalidOperationException) { }
}
var profile = System.Text.Json.JsonSerializer.SerializeToElement(FrameworkWorkProfile.Snapshot());
var entries = profile.GetProperty("entries").EnumerateArray().ToArray();
var outer = entries.Single(e => e.GetProperty("Kind").GetInt32() == 99);
var inner = entries.Single(e => e.GetProperty("Kind").GetInt32() == 100);
Require(outer.GetProperty("Calls").GetInt64() == 1 && inner.GetProperty("Calls").GetInt64() == 1,
    "nested profile scopes unwind through exceptions");
Require(outer.GetProperty("InclusiveMicroseconds").GetInt64() >= outer.GetProperty("SelfMicroseconds").GetInt64(),
    "profile inclusive time contains self time");
var isolated = false;
var thread = new Thread(() => {
    var other = System.Text.Json.JsonSerializer.SerializeToElement(FrameworkWorkProfile.Snapshot());
    isolated = other.GetProperty("entries").GetArrayLength() == 0;
});
thread.Start(); thread.Join();
Require(isolated, "profile never mixes worker threads");
for(var kind=1000;kind<1600;kind++) FrameworkWorkProfile.Count(typeof(LayoutProbe),kind);
var bounded = System.Text.Json.JsonSerializer.SerializeToElement(FrameworkWorkProfile.Snapshot());
Require(bounded.GetProperty("entries").GetArrayLength() == 512 && bounded.GetProperty("dropped").GetInt64() > 0,
    "profile type table is bounded and reports overflow");

sealed class LayoutProbe : RenderBox
{
    public int LayoutCount;
    public override void performLayout() { LayoutCount++; size = new Size(10,10); }
}
