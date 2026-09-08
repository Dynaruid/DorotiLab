using System.Diagnostics;
using System.Text.Json;
using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Scheduler;
using Doroti.Framework.Widgets;
using Doroti.Ui;
using M = Doroti.Framework.Material;
using TextStyle = Doroti.Framework.Painting.TextStyle;

// Explicit async preparation boundary: only immutable numeric weights enter the shared engine.
// Existing RenderBox.layout, LayoutBuilder callbacks, widget State and painting stay on the owner.
internal sealed class ParallelLayoutLab(bool embedded = false) : StatefulWidget, IDisposable
{
    internal bool Embedded => embedded;
    internal PreparedTreemapLayout Engine { get; } = new();
    internal bool IsDisposed { get; private set; }
    public void Dispose() { IsDisposed = true; Engine.Dispose(); }
    public override IState createState() => new ParallelLayoutLabState();
}

internal sealed class ParallelLayoutLabState : State<ParallelLayoutLab>
{
    private PreparedTreemapLayout.Result? _result;
    private bool _parallel = Environment.GetEnvironmentVariable("DOROTI_LAYOUT_MODE") != "serial";
    private bool _scheduled, _running;
    private int _seed, _revision, _applied;
    private double _width, _height;
    private string _status = "Preparing layout…";
    private CancellationTokenSource? _cancel;
    private const int NodesPerPartition = 8192;

    private void Change(bool? parallel = null)
    {
        setState(() =>
        {
            if (parallel.HasValue) _parallel = parallel.Value; else _seed++;
            _revision++; _status = "Preparing layout…";
            _cancel?.Cancel();
        });
        Schedule();
    }

    private void Schedule()
    {
        if (_scheduled || _running || _width <= 0 || _height <= 0) return;
        _scheduled = true;
        SchedulerBinding.instance.addPostFrameCallback(_ =>
        {
            _scheduled = false;
            if (mounted) Prepare();
        }, debugLabel: "ParallelLayoutLab.prepare");
        SchedulerBinding.instance.scheduleFrame();
    }

    private async void Prepare()
    {
        if (_running || widget.IsDisposed) return;
        _running = true;
        var revision = _revision;
        var parallel = _parallel;
        var seed = _seed;
        var owner = Environment.CurrentManagedThreadId;
        _cancel = new CancellationTokenSource();
        try
        {
            // Deterministic, identical serial/parallel input. Every computed leaf is painted once.
            var left = Enumerable.Range(0, NodesPerPartition).Select(i => 1d + ((i * 31 + seed * 17) % 101)).ToArray();
            var right = Enumerable.Range(0, NodesPerPartition).Select(i => 1d + ((i * 47 + seed * 23) % 101)).ToArray();
            var result = await widget.Engine.PrepareAsync(left, right, new(0, 0, _width, _height), parallel, _cancel.Token);
            if (Environment.CurrentManagedThreadId != owner) throw new InvalidOperationException("Layout result resumed outside the UI owner.");
            if (!mounted || widget.IsDisposed || revision != _revision) return;
            setState(() =>
            {
                _result = result; _applied = revision;
                _status = $"{(parallel ? "Parallel" : "Serial")} · {NodesPerPartition * 2:N0} tiles · {result.ElapsedMs:F2} ms preparation";
            });
            Console.WriteLine("DOROTI_PARALLEL_LAYOUT " + JsonSerializer.Serialize(new
            {
                revision, seed, parallel, owner, resumedThread = Environment.CurrentManagedThreadId,
                leftThread = result.Left.ThreadId, rightThread = result.Right.ThreadId,
                result.OverlapMs, result.ElapsedMs, nodes = NodesPerPartition * 2,
                width = _width, height = _height, checksum = Checksum(result),
                leftStart = result.Left.Start, leftEnd = result.Left.End,
                rightStart = result.Right.Start, rightEnd = result.Right.End,
                frequency = Stopwatch.Frequency,
            }));
        }
        catch (OperationCanceledException) { }
        catch (Exception error)
        {
            Console.Error.WriteLine("DOROTI_PARALLEL_LAYOUT_ERROR " + error);
            if (mounted) setState(() => _status = "Layout failed: " + error.Message);
        }
        finally
        {
            _cancel.Dispose(); _cancel = null; _running = false;
            if (mounted && revision != _revision) Schedule();
        }
    }

    private static string Checksum(PreparedTreemapLayout.Result result)
    {
        ulong hash = 14695981039346656037;
        foreach (var box in result.Left.Boxes.Concat(result.Right.Boxes))
            foreach (var value in new[] { box.X, box.Y, box.Width, box.Height })
                hash = unchecked((hash ^ (ulong)BitConverter.DoubleToInt64Bits(value)) * 1099511628211);
        return hash.ToString("x16");
    }

    public override void dispose()
    {
        _revision++; _cancel?.Cancel(); widget.Dispose(); base.dispose();
    }

    public override Widget build(BuildContext context) => widget.Embedded ? Content() : new M.MaterialApp(
        title: "Doroti Parallel Layout Lab", debugShowCheckedModeBanner: false,
        locale: new Doroti.Ui.Locale("en", "US"),
        themeFactory: () => M.ThemeData.Create(colorSchemeSeed: new Color(0xff6750a4), fontFamilyFallback: ["Roboto"]),
        home: new M.Scaffold(body: new SafeArea(child: Content())));

    private Widget Content() => new Padding(padding: EdgeInsets.CreateAll(20), child: new Column(
            crossAxisAlignment: CrossAxisAlignment.start, children:
            [
                new Text("Parallel layout lab", style: new TextStyle(fontSize: 28, fontWeight: FontWeight.bold)),
                new Text("같은 타일 배치를 직렬 / 두 계산 스레드로 비교합니다. 기존 위젯 트리 전체의 병렬화는 아닙니다."),
                new SizedBox(height: 12),
                new Wrap(spacing: 8, runSpacing: 8, children:
                [
                    new M.ChoiceChip(label: new Text("Serial"), selected: !_parallel, onSelected: _ => Change(false)),
                    new M.ChoiceChip(label: new Text("Parallel (2 threads)"), selected: _parallel, onSelected: _ => Change(true)),
                    new M.OutlinedButton(onPressed: () => Change(), child: new Text("New layout")),
                ]),
                new SizedBox(height: 8),
                new Text(_status),
                new Text(_result is null ? "Waiting for the first result" :
                    $"Owner {_result.OwnerThreadId} · compute {_result.Left.ThreadId} / {_result.Right.ThreadId} · overlap {_result.OverlapMs:F3} ms · seed {_seed}"),
                new Text("실험용 treemap 레이아웃 · 크기 변경 중에는 이전 배치를 유지 · 표시 시간은 FPS가 아닙니다."),
                new SizedBox(height: 12),
                new Expanded(child: new ClipRect(child: new LayoutBuilder(builder: (_, constraints) =>
                {
                    var width = Math.Max(1, constraints.maxWidth);
                    var height = Math.Max(1, constraints.maxHeight);
                    if (width != _width || height != _height)
                    {
                        _width = width; _height = height; _revision++;
                        _cancel?.Cancel(); Schedule();
                    }
                    return new CustomPaint(size: new Size(width, height), painter: new TreemapPainter(_result, _applied));
                }))),
            ]));

    private sealed class TreemapPainter(PreparedTreemapLayout.Result? result, int revision) : CustomPainter
    {
        public override void paint(Canvas canvas, Size size)
        {
            var paint = new Paint();
            paint.color = new Color(0xff171824);
            canvas.drawRect(Rect.fromLTWH(0, 0, size.width, size.height), paint);
            if (result is null) return;
            long[] colors = [0xff8b80d9, 0xffb0a4ef, 0xff68bcb1, 0xffafd8c9, 0xffe6b970, 0xffd68ca3];
            var index = 0;
            foreach (var box in result.Left.Boxes.Concat(result.Right.Boxes))
            {
                paint.color = new Color(colors[index++ % colors.Length]);
                canvas.drawRect(Rect.fromLTWH(box.X + .3, box.Y + .3, Math.Max(0, box.Width - .6), Math.Max(0, box.Height - .6)), paint);
            }
        }
        public override bool shouldRepaint(CustomPainter oldDelegate) => oldDelegate is not TreemapPainter old || old.Revision != revision;
        private int Revision => revision;
    }
}
