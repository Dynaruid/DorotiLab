using Doroti.Framework.Rendering;

static void Check(bool value, string message) { if (!value) throw new Exception(message); }
await using (var engine = new PreparedTreemapLayout())
{
    var weights = new double[] { 1, 2, 3, 4 };
    var serial = await engine.PrepareAsync(weights, weights, new(7, 11, 200, 100), false);
    var parallel = await engine.PrepareAsync(weights, weights, new(7, 11, 200, 100), true);
    Check(serial.Left.Boxes.SequenceEqual(parallel.Left.Boxes) && serial.Right.Boxes.SequenceEqual(parallel.Right.Boxes), "Geometry differs");
    Check(parallel.Left.ThreadId != parallel.Right.ThreadId && parallel.Left.ThreadId != parallel.OwnerThreadId &&
        parallel.Right.ThreadId != parallel.OwnerThreadId, "Not two independent compute threads");
    foreach (var partition in new[] { parallel.Left, parallel.Right })
    {
        for (var i = 0; i < 4; i++)
        {
            var box = partition.Boxes[i];
            Check(Math.Abs(box.Width * box.Height - 10000 * weights[i] / 10) < 1e-8, "Weighted area incorrect");
            Check(box.X >= 7 && box.Y >= 11 && box.X + box.Width <= 207.000001 && box.Y + box.Height <= 111.000001, "Outside bounds");
            for (var j = 0; j < i; j++)
            {
                var other = partition.Boxes[j];
                Check(Math.Min(box.X + box.Width, other.X + other.Width) <= Math.Max(box.X, other.X) + 1e-9 ||
                    Math.Min(box.Y + box.Height, other.Y + other.Height) <= Math.Max(box.Y, other.Y) + 1e-9, "Overlapping leaves");
            }
        }
    }
    using var canceled = new CancellationTokenSource(); canceled.Cancel();
    try { await engine.PrepareAsync(weights, weights, new(0, 0, 100, 100), true, canceled.Token); throw new Exception("Cancellation ignored"); }
    catch (OperationCanceledException) { }
    try { await engine.PrepareAsync([double.NaN], weights, new(0, 0, 100, 100), true); throw new Exception("NaN accepted"); }
    catch (ArgumentOutOfRangeException) { }
    var large = Enumerable.Range(0, 8192).Select(i => 1d + i % 101).ToArray();
    var a = await engine.PrepareAsync(large, large, new(0, 0, 1240, 600), false);
    var b = await engine.PrepareAsync(large, large, new(0, 0, 1240, 600), true);
    Check(a.Left.Boxes.SequenceEqual(b.Left.Boxes) && a.Right.Boxes.SequenceEqual(b.Right.Boxes), "Large geometry differs");
    Console.WriteLine($"PASS geometry/area/nonoverlap/cancel/validation; threads {b.Left.ThreadId}/{b.Right.ThreadId}; overlap {b.OverlapMs:F3} ms");
}
var disposed = new PreparedTreemapLayout();
var pending = disposed.PrepareAsync(Enumerable.Repeat(1d, 65536).ToArray(), [1d], new(0, 0, 100, 100), true);
await disposed.DisposeAsync();
try { await pending; } catch (OperationCanceledException) { } catch (InvalidOperationException) { }
try { await disposed.PrepareAsync([1d], [1d], new(0, 0, 1, 1), true); throw new Exception("Disposed engine accepted work"); }
catch (ObjectDisposedException) { }
Console.WriteLine("PASS shutdown completes without owner Join; disposed engine rejects work");
