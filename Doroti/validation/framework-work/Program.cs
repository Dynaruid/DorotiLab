using Doroti.Framework.Widgets;
using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Ui;
using Aspect = Doroti.Framework.Widgets._MediaQueryAspect__media_query;

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
for (var i = 0; i < 2050; i++) {
    FrameworkWorkCounters.Add(FrameworkWork.NewPicture);
    trace.Record(DorotiFramePhase.build, 42, DorotiFrameClock.Now, frameworkFrameNumber: i);
}
var snapshot = FrameworkWorkCounters.Snapshot();
Require(snapshot.Samples.Length == 2048 && snapshot.Dropped == 2, "bounded ring reports overwritten samples");
Require(snapshot.Samples[0].Boundary.Frame == 2 && snapshot.Samples[^1].Boundary.Frame == 2049,
    "ring wrap preserves chronological frame identity");
Require(snapshot.Samples[^1].Totals[(int)FrameworkWork.NewPicture] - snapshot.Samples[0].Totals[(int)FrameworkWork.NewPicture] == 2047,
    "cumulative deltas survive ring wrap");

sealed class LayoutProbe : RenderBox
{
    public int LayoutCount;
    public override void performLayout() { LayoutCount++; size = new Size(10,10); }
}
