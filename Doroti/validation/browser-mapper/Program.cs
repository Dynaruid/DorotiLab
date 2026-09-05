using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using Doroti.Graphics.DisplayList;
using UiPath = Doroti.Ui.Path;

// Exercise the real internal mapper without widening the product API.
var type = Assembly.Load("Doroti.Host.Web").GetType("Doroti.Host.Web.BrowserDisplayListMapper", true)!;
var map = type.GetMethod("ToPath", BindingFlags.NonPublic | BindingFlags.Static)!
    .CreateDelegate<Func<UiPath, DisplayPath>>();
var empty = map(new UiPath());
if (empty.Values.Count != 0 || empty.Verbs.Count != 0) throw new Exception("Empty path changed");
foreach (var lines in new[] { 4, 128 })
{
    var path = new UiPath();
    path.moveTo(1.25, -2.5);
    for (var i = 0; i < lines; i++) path.lineTo(i + .5, i * 2);
    path.quadraticBezierTo(1, 2, 3, 4);
    path.cubicTo(1, 2, 3, 4, 5, 6);
    path.close();
    var expected = path.Commands.SelectMany(c => c.Arguments).Select(x => checked((float)x)).ToArray();
    var mapped = map(path);
    if (!mapped.Values.SequenceEqual(expected) || mapped.Verbs.Count != path.Commands.Count)
        throw new Exception("Path argument order or verb count changed");
    var expectedVerbs = new[] { DisplayPathVerb.MoveTo }
        .Concat(Enumerable.Repeat(DisplayPathVerb.LineTo, lines))
        .Concat(new[] { DisplayPathVerb.QuadraticTo, DisplayPathVerb.CubicTo, DisplayPathVerb.Close });
    if (!mapped.Verbs.SequenceEqual(expectedVerbs)) throw new Exception("Path verb order changed");
    var identity = Convert.ToHexString(SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(mapped)));
    for (var i = 0; i < 1000; i++) map(path);
    var batches = new List<object>();
    for (var batch = 0; batch < 5; batch++)
    {
        var before = GC.GetAllocatedBytesForCurrentThread();
        var started = System.Diagnostics.Stopwatch.GetTimestamp();
        for (var i = 0; i < 10000; i++) map(path);
        var milliseconds = System.Diagnostics.Stopwatch.GetElapsedTime(started).TotalMilliseconds;
        var allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - before;
        batches.Add(new { iterations = 10000, allocatedBytes, milliseconds });
    }
    path.reset();
    if (!mapped.Values.SequenceEqual(expected)) throw new Exception("Source reset mutated mapped snapshot");
    Console.WriteLine("MAPPER_MEASURE " + JsonSerializer.Serialize(new { lines, identity, batches }));
}
Console.WriteLine("Browser path mapper: PASS; CLR allocation only, not WASM GC");
