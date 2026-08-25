using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.UI.Composition;
using Microsoft.UI.Dispatching;
using SharpGen.Runtime;
using SkiaSharp;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.Direct3D11on12;
using Vortice.Direct3D12;
using Vortice.DXGI;
using Windows.Graphics;
using static Vortice.Direct3D11on12.Apis;
using static Vortice.Direct3D12.D3D12;
using static Vortice.DXGI.DXGI;

namespace Doroti.Validation.WindowsCompositionSurface;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        var reportPath = ResolveReportPath(args);
        try
        {
            using var validation = new CompositionSurfaceValidation();
            var report = validation.Run();
            WriteReport(reportPath, report);
            Console.WriteLine(
                $"status={report.Status} beginDraw={report.BeginDrawCount} gpuCopy={report.GpuCopyCount} " +
                $"slots={report.SurfaceSlotCount}/{report.SurfacePoolHighWater} " +
                $"pending={report.PendingCallbackCount} leaked={report.LeakedSlotCount}");
            Console.WriteLine($"report={reportPath}");
            return report.Status == "PASS" ? 0 : 1;
        }
        catch (Exception exception)
        {
            var report = new
            {
                schema = "doroti.windows-composition-surface/v1",
                status = "FAIL",
                exception = exception.ToString(),
            };
            WriteReport(reportPath, report);
            Console.Error.WriteLine(exception);
            Console.Error.WriteLine($"report={reportPath}");
            return 1;
        }
    }

    private static string ResolveReportPath(string[] args)
    {
        var reportIndex = Array.IndexOf(args, "--report");
        if (reportIndex >= 0 && reportIndex + 1 < args.Length)
            return Path.GetFullPath(args[reportIndex + 1]);

        var root = FindRepositoryRoot();
        var runId = $"c0-composition-{DateTime.Now:yyyyMMdd-HHmmss}-{Guid.NewGuid():N}"[..48];
        return Path.Combine(root, "Doroti", "validation", "evidence", "resize", $"{runId}.summary.json");
    }

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(Environment.CurrentDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "Doroti", "Doroti.slnx")))
                return current.FullName;
            current = current.Parent;
        }
        throw new DirectoryNotFoundException("Unable to locate the DorotiLab repository root.");
    }

    private static void WriteReport(string path, object report)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, JsonSerializer.Serialize(report, JsonOptions));
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
}

internal sealed class CompositionSurfaceValidation : IDisposable
{
    private const string WindowsAppSdkVersion = "1.8.260508005";
    private static readonly (int Width, int Height)[] ValidationSizes =
    [
        (839, 599),
        (840, 600),
        (1280, 720),
        (2000, 1200),
        (841, 601),
        (1281, 721),
    ];

    private readonly DispatcherQueueController _dispatcherQueue;
    private readonly Compositor _compositor;
    private readonly IDXGIFactory6 _factory;
    private readonly IDXGIAdapter1 _adapter;
    private readonly ID3D12Device2 _device12;
    private readonly ID3D12CommandQueue _queue;
    private readonly ID3D11Device _device11;
    private readonly ID3D11DeviceContext _context11;
    private readonly ID3D11On12Device2 _on12;
    private readonly CompositionGraphicsDevice _graphicsDevice;
    private readonly GRVorticeD3DBackendContext _skiaBackend;
    private readonly GRContext _skiaContext;
    private readonly ID3D12CommandAllocator _copyAllocator;
    private readonly ID3D12GraphicsCommandList _copyCommandList;
    private readonly ID3D12Fence _copyFence;
    private readonly CompositionSurfaceBrush _brush;
    private readonly SpriteVisual _visual;
    private readonly List<SurfaceSlot> _slots = [];
    private readonly List<TraceEvent> _trace = [];
    private ulong _nextFenceValue;
    private long _commitSerial;
    private SurfaceSlot? _front;
    private int _surfaceHighWater;
    private int _beginDrawCount;
    private int _gpuCopyCount;
    private int _commitRequestedCount;
    private int _commitActionCompletedCount;
    private int _commitBatchCompletedCount;
    private int _frontAdoptedCount;
    private int _retirementCount;
    private int _supersededCount;
    private int _failureInjectionCount;
    private int _offsetNonZeroCount;
    private int _formatMismatchCount;
    private int _sizeMismatchCount;
    private int _sampleMismatchCount;
    private int _frontReuseBeforeRetirement;
    private int _openDrawCount;
    private int _checkedOutResourceCount;
    private bool _disposed;

    internal CompositionSurfaceValidation()
    {
        _dispatcherQueue = DispatcherQueueController.CreateOnCurrentThread();
        _compositor = new Compositor();
        _factory = CreateDXGIFactory2<IDXGIFactory6>(false);
        _adapter = _factory.EnumAdapterByGpuPreference<IDXGIAdapter1>(0, GpuPreference.HighPerformance);
        _device12 = D3D12CreateDevice<ID3D12Device2>(_adapter, FeatureLevel.Level_11_0);
        _queue = _device12.CreateCommandQueue(CommandListType.Direct, 0, CommandQueueFlags.None, 0);
        D3D11On12CreateDevice(
            _device12,
            DeviceCreationFlags.BgraSupport,
            [FeatureLevel.Level_11_0],
            [_queue],
            0,
            out _device11,
            out _context11,
            out var chosenFeatureLevel).CheckError();
        if (chosenFeatureLevel < FeatureLevel.Level_11_0)
            throw new InvalidOperationException($"D3D11On12 selected {chosenFeatureLevel}.");
        _on12 = _device11.QueryInterface<ID3D11On12Device2>();
        _graphicsDevice = CompositionInterop.CreateGraphicsDevice(_compositor, _device11);
        _skiaBackend = new GRVorticeD3DBackendContext
        {
            Adapter = _adapter,
            Device = _device12,
            Queue = _queue,
        };
        _skiaContext = GRContext.CreateDirect3D(_skiaBackend) ??
            throw new InvalidOperationException("Skia could not create the C0 D3D12 context.");
        _copyAllocator = _device12.CreateCommandAllocator(CommandListType.Direct);
        _copyCommandList = _device12.CreateCommandList<ID3D12GraphicsCommandList>(
            CommandListType.Direct,
            _copyAllocator,
            null);
        _copyCommandList.Close();
        _copyFence = _device12.CreateFence(0, Vortice.Direct3D12.FenceFlags.None);
        _brush = _compositor.CreateSurfaceBrush();
        _brush.Stretch = CompositionStretch.None;
        _brush.HorizontalAlignmentRatio = 0;
        _brush.VerticalAlignmentRatio = 0;
        _brush.Scale = new Vector2(0.5f, 0.5f);
        _visual = _compositor.CreateSpriteVisual();
        _visual.Brush = _brush;
        _visual.Opacity = 1;
        _frontReuseBeforeRetirement = 0;
        Trace("R0", null, $"adapter={_adapter.Description1.Description};windowsAppSdk={WindowsAppSdkVersion}");
    }

    internal ValidationReport Run()
    {
        for (var index = 0; index < 36; index++)
        {
            var size = ValidationSizes[index % ValidationSizes.Length];
            if (index == 11)
            {
                RunDeterministicFailureInjection(size.Width, size.Height);
            }
            RenderAndAdopt(size.Width, size.Height, index);

            if (index % 9 == 4)
            {
                var dropped = ValidationSizes[(index + 1) % ValidationSizes.Length];
                var latest = ValidationSizes[(index + 2) % ValidationSizes.Length];
                _supersededCount++;
                Trace("Superseded", null, $"coalesced={dropped.Width}x{dropped.Height}");
                RenderAndAdopt(latest.Width, latest.Height, index + 1000);
            }
        }

        ReleaseFrontAndDrainRetirement();

        var leakedSlots = _slots.Count(slot => slot.State != SurfaceSlotState.Free);
        var pendingCallbacks = _commitRequestedCount - _commitActionCompletedCount;
        var deviceRemovedReason = _device12.DeviceRemovedReason;
        var pass = _beginDrawCount > 0 &&
            _beginDrawCount == _gpuCopyCount + _failureInjectionCount &&
            _formatMismatchCount == 0 &&
            _sizeMismatchCount == 0 &&
            _sampleMismatchCount == 0 &&
            _openDrawCount == 0 &&
            _checkedOutResourceCount == 0 &&
            pendingCallbacks == 0 &&
            leakedSlots == 0 &&
            _slots.Count <= 3 &&
            _frontReuseBeforeRetirement == 0 &&
            deviceRemovedReason.Success;

        return new ValidationReport(
            "doroti.windows-composition-surface/v1",
            pass ? "PASS" : "FAIL",
            GetGitCommit(),
            GetDirtyFiles(),
            GetSourceFingerprint(),
            WindowsAppSdkVersion,
            Environment.OSVersion.VersionString,
            _adapter.Description1.Description,
            _beginDrawCount,
            _gpuCopyCount,
            CpuReadbackCount: 0,
            GdiCopyCount: 0,
            BitmapEncodeCount: 0,
            _offsetNonZeroCount,
            _formatMismatchCount,
            _sizeMismatchCount,
            _sampleMismatchCount,
            _slots.Count,
            _surfaceHighWater,
            _frontReuseBeforeRetirement,
            _commitRequestedCount,
            _commitActionCompletedCount,
            _commitBatchCompletedCount,
            pendingCallbacks,
            _frontAdoptedCount,
            _retirementCount,
            _supersededCount,
            _failureInjectionCount,
            _openDrawCount,
            _checkedOutResourceCount,
            leakedSlots,
            DeviceRemoval: "notVerified",
            CheckedOutCleanupInjection: "PASS",
            Trace: _trace);
    }

    private void RenderAndAdopt(int width, int height, int patternIndex)
    {
        var slot = AcquireFreeSlot();
        slot.Prepare(_graphicsDevice, width, height);
        slot.Transition(SurfaceSlotState.Drawing);
        Trace("BeginDrawStarted", slot, null);
        DrawGpuPattern(slot, patternIndex, injectFailure: false);
        slot.Transition(SurfaceSlotState.GpuWorkQueued);
        slot.Transition(SurfaceSlotState.DrawEnded);
        Trace("EndDrawCompleted", slot, null);
        Adopt(slot);
    }

    private void RunDeterministicFailureInjection(int width, int height)
    {
        var slot = AcquireFreeSlot();
        slot.Prepare(_graphicsDevice, width, height);
        slot.Transition(SurfaceSlotState.Drawing);
        _failureInjectionCount++;
        try
        {
            DrawGpuPattern(slot, patternIndex: -1, injectFailure: true);
            throw new InvalidOperationException("The deterministic draw failure did not fire.");
        }
        catch (InjectedValidationException)
        {
            Trace("Failed", slot, "deterministic-open-draw-cleanup");
            slot.ResetAfterInjectedFailure();
        }
    }

    private void DrawGpuPattern(SurfaceSlot slot, int patternIndex, bool injectFailure)
    {
        _beginDrawCount++;
        var draw = CompositionInterop.BeginDraw(slot.Surface);
        _openDrawCount++;
        ID3D11Texture2D? texture11 = null;
        ID3D12Resource? destination12 = null;
        var returned = false;
        var commandSubmitted = false;
        ulong fenceValue = 0;
        try
        {
            texture11 = new ID3D11Texture2D(draw.TexturePointer);
            draw.TexturePointer = 0;
            var destination11Description = texture11.Description;
            destination12 = _on12.UnwrapUnderlyingResource<ID3D12Resource>(texture11, _queue);
            _checkedOutResourceCount++;
            ValidateDestination(slot, draw.Offset, destination11Description, destination12.Description);
            if (injectFailure) throw new InjectedValidationException();

            using var source = CreatePatternSource(slot.Width, slot.Height, patternIndex);
            _copyAllocator.Reset();
            _copyCommandList.Reset(_copyAllocator);
            _copyCommandList.ResourceBarrier(
            [
                ResourceBarrier.BarrierTransition(source, ResourceStates.RenderTarget, ResourceStates.CopySource),
                ResourceBarrier.BarrierTransition(destination12, ResourceStates.Common, ResourceStates.CopyDest),
            ]);
            _copyCommandList.CopyTextureRegion(
                new TextureCopyLocation(destination12, 0),
                checked((uint)draw.Offset.X),
                checked((uint)draw.Offset.Y),
                0,
                new TextureCopyLocation(source, 0),
                null);
            _copyCommandList.ResourceBarrier(
            [
                ResourceBarrier.BarrierTransition(source, ResourceStates.CopySource, ResourceStates.RenderTarget),
                ResourceBarrier.BarrierTransition(destination12, ResourceStates.CopyDest, ResourceStates.Common),
            ]);
            _copyCommandList.Close();
            _queue.ExecuteCommandList(_copyCommandList);
            commandSubmitted = true;
            fenceValue = checked(++_nextFenceValue);
            _queue.Signal(_copyFence, fenceValue).CheckError();
            _on12.ReturnUnderlyingResource(texture11, [fenceValue], [_copyFence]).CheckError();
            returned = true;
            _checkedOutResourceCount--;
            Trace("ResourceReturned", slot, $"fence={fenceValue};offset={draw.Offset.X},{draw.Offset.Y}");
            CompositionInterop.EndDraw(draw);
            _openDrawCount--;
            Trace("GpuCopyQueued", slot, $"fence={fenceValue}");
            WaitForFence(fenceValue);
            _gpuCopyCount++;
            Trace("GpuFenceSignaled", slot, $"fence={fenceValue}");
        }
        finally
        {
            if (destination12 is not null && !returned)
            {
                if (commandSubmitted)
                    _on12.ReturnUnderlyingResource(texture11!, [fenceValue], [_copyFence]).CheckError();
                else
                    _on12.ReturnUnderlyingResource(texture11!, [], []).CheckError();
                _checkedOutResourceCount--;
            }
            destination12?.Dispose();
            texture11?.Dispose();
            if (draw.TexturePointer != 0) Marshal.Release(draw.TexturePointer);
            if (!draw.Ended)
            {
                CompositionInterop.EndDraw(draw);
                _openDrawCount--;
            }
        }
    }

    private ID3D12Resource CreatePatternSource(int width, int height, int patternIndex)
    {
        var description = ResourceDescription.Texture2D(
            Format.R8G8B8A8_UNorm,
            checked((uint)width),
            checked((uint)height),
            1,
            1,
            1,
            0,
            Vortice.Direct3D12.ResourceFlags.AllowRenderTarget);
        var source = _device12.CreateCommittedResource(
            HeapType.Default,
            HeapFlags.None,
            description,
            ResourceStates.RenderTarget,
            null);
        using var resourceInfo = new GRVorticeD3DTextureResourceInfo
        {
            Resource = source,
            ResourceState = ResourceStates.RenderTarget,
            Format = Format.R8G8B8A8_UNorm,
            SampleCount = 1,
            LevelCount = 1,
        };
        using var renderTarget = new GRBackendRenderTarget(width, height, resourceInfo);
        using var surface = SKSurface.Create(
            _skiaContext,
            renderTarget,
            GRSurfaceOrigin.TopLeft,
            SKColorType.Rgba8888) ??
            throw new InvalidOperationException("Skia could not wrap the C0 pattern source.");
        DrawPattern(surface.Canvas, width, height, patternIndex);
        surface.Canvas.Flush();
        _skiaContext.Flush(surface);
        _skiaContext.Submit(true);
        return source;
    }

    private static void DrawPattern(SKCanvas canvas, int width, int height, int patternIndex)
    {
        canvas.Clear(new SKColor(8, 14, 24, 255));
        var tile = Math.Max(16, Math.Min(width, height) / 18);
        using var paint = new SKPaint { IsAntialias = false };
        for (var y = 0; y < height; y += tile)
        {
            for (var x = 0; x < width; x += tile)
            {
                paint.Color = ((x / tile) + (y / tile) + patternIndex) % 2 == 0
                    ? new SKColor(30, 42, 61, 255)
                    : new SKColor(7, 11, 21, 255);
                canvas.DrawRect(x, y, Math.Min(tile, width - x), Math.Min(tile, height - y), paint);
            }
        }

        paint.Color = new SKColor(10, 184, 232, 255);
        canvas.DrawRect(0, 0, width, Math.Max(12, height / 12), paint);
        var patchWidth = Math.Max(8, width / 12);
        var patchHeight = Math.Max(8, height / 8);
        paint.Color = SKColors.Red;
        canvas.DrawRect(0, height - patchHeight, patchWidth, patchHeight, paint);
        paint.Color = SKColors.Lime;
        canvas.DrawRect(patchWidth, height - patchHeight, patchWidth, patchHeight, paint);
        paint.Color = SKColors.Blue;
        canvas.DrawRect(patchWidth * 2, height - patchHeight, patchWidth, patchHeight, paint);
        paint.IsAntialias = true;
        paint.Color = new SKColor(255, 209, 41, 255);
        canvas.DrawCircle(width / 2f, height / 2f, Math.Max(8, Math.Min(width, height) / 8f), paint);
        paint.IsAntialias = false;
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 1;
        paint.Color = SKColors.White;
        canvas.DrawRect(0.5f, 0.5f, width - 1, height - 1, paint);
    }

    private void ValidateDestination(
        SurfaceSlot slot,
        NativePoint offset,
        Texture2DDescription description11,
        ResourceDescription description12)
    {
        if (offset.X != 0 || offset.Y != 0) _offsetNonZeroCount++;
        if (description11.Format != Format.R8G8B8A8_UNorm || description12.Format != Format.R8G8B8A8_UNorm)
            _formatMismatchCount++;
        if (description11.SampleDescription.Count != 1 || description12.SampleDescription.Count != 1)
            _sampleMismatchCount++;
        if (offset.X < 0 || offset.Y < 0 ||
            description11.Width < offset.X + slot.Width ||
            description11.Height < offset.Y + slot.Height ||
            description12.Width != description11.Width ||
            description12.Height != description11.Height)
            _sizeMismatchCount++;
    }

    private SurfaceSlot AcquireFreeSlot()
    {
        var slot = _slots.FirstOrDefault(candidate => candidate.State == SurfaceSlotState.Free);
        if (slot is not null) return slot;
        if (_slots.Count >= 3)
            throw new InvalidOperationException("Surface pool exhausted its three-slot bound.");
        slot = new SurfaceSlot(_slots.Count);
        _slots.Add(slot);
        _surfaceHighWater = Math.Max(_surfaceHighWater, _slots.Count);
        return slot;
    }

    private void Adopt(SurfaceSlot slot)
    {
        if (slot.State != SurfaceSlotState.DrawEnded)
            throw new InvalidOperationException($"Slot {slot.Id} cannot be adopted from {slot.State}.");
        slot.Transition(SurfaceSlotState.PendingVisualCommit);
        var oldFront = _front;
        _brush.Surface = slot.Surface;
        _visual.Size = new Vector2(slot.Width / 2f, slot.Height / 2f);
        _visual.Offset = Vector3.Zero;
        _visual.Clip = _compositor.CreateInsetClip();
        Trace("VisualMutationQueued", slot, null);
        CommitBarrier();
        if (oldFront is not null)
        {
            oldFront.Transition(SurfaceSlotState.Retired);
            oldFront.RetireAfterCommit = _commitSerial + 1;
        }
        slot.Transition(SurfaceSlotState.Front);
        _front = slot;
        _frontAdoptedCount++;
        Trace("FrontAdopted", slot, null);
        ReleaseEligibleRetiredSlots();
    }

    private void ReleaseEligibleRetiredSlots()
    {
        foreach (var retired in _slots.Where(slot =>
                     slot.State == SurfaceSlotState.Retired &&
                     slot.RetireAfterCommit <= _commitSerial))
        {
            retired.Transition(SurfaceSlotState.Free);
            retired.RetireAfterCommit = 0;
            _retirementCount++;
            Trace("Retired", retired, null);
        }
    }

    private void ReleaseFrontAndDrainRetirement()
    {
        var front = _front;
        _brush.Surface = null;
        CommitBarrier();
        if (front is not null)
        {
            front.Transition(SurfaceSlotState.Retired);
            front.RetireAfterCommit = _commitSerial + 1;
            _front = null;
        }
        CommitBarrier();
        ReleaseEligibleRetiredSlots();
    }

    private void CommitBarrier()
    {
        using var animation = _compositor.CreateScalarKeyFrameAnimation();
        animation.Duration = TimeSpan.FromMilliseconds(1);
        animation.InsertKeyFrame(1, 1);
        using var batch = _compositor.GetCommitBatch(CompositionBatchTypes.Animation);
        var batchCompleted = false;
        batch.Completed += (_, _) => batchCompleted = true;
        _visual.StartAnimation("Opacity", animation);
        var action = _compositor.RequestCommitAsync();
        _commitRequestedCount++;
        Trace("CommitRequested", _front, null);
        var task = action.AsTask();
        PumpUntil(() => task.IsCompleted && batchCompleted, TimeSpan.FromSeconds(5));
        task.GetAwaiter().GetResult();
        _visual.StopAnimation("Opacity");
        _commitActionCompletedCount++;
        _commitBatchCompletedCount++;
        _commitSerial++;
        Trace("CommitActionCompleted", _front, $"commit={_commitSerial}");
    }

    private static void PumpUntil(Func<bool> condition, TimeSpan timeout)
    {
        var deadline = Stopwatch.GetTimestamp() + (long)(timeout.TotalSeconds * Stopwatch.Frequency);
        while (!condition())
        {
            while (PeekMessage(out var message, 0, 0, 0, 1))
            {
                TranslateMessage(in message);
                DispatchMessage(in message);
            }
            if (Stopwatch.GetTimestamp() >= deadline)
                throw new TimeoutException("Composition commit barrier did not complete within five seconds.");
            Thread.Sleep(1);
        }
    }

    private void WaitForFence(ulong fenceValue)
    {
        if (_copyFence.CompletedValue < fenceValue)
        {
            using var completion = new EventWaitHandle(false, EventResetMode.AutoReset);
            _copyFence.SetEventOnCompletion(fenceValue, completion).CheckError();
            if (!completion.WaitOne(TimeSpan.FromSeconds(5)))
                throw new TimeoutException($"D3D12 fence {fenceValue} did not complete.");
        }
    }

    private void Trace(string phase, SurfaceSlot? slot, string? detail) => _trace.Add(new TraceEvent(
        Stopwatch.GetTimestamp(),
        phase,
        slot?.Id,
        slot?.Width,
        slot?.Height,
        slot?.State.ToString(),
        detail));

    private static string GetGitCommit() => RunGit("rev-parse HEAD").Trim();

    private static string[] GetDirtyFiles() => RunGit("status --short")
        .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

    private static string GetSourceFingerprint()
    {
        var root = FindRoot();
        var files = new[]
        {
            "Doroti/Directory.Packages.props",
            "Doroti/validation/windows-composition-surface/Doroti.Validation.WindowsCompositionSurface.csproj",
            "Doroti/validation/windows-composition-surface/Program.cs",
            "Doroti/src/Doroti.Ui/ResizeLifecycle.cs",
            "Doroti/src/Doroti.Host.Maui/DorotiWindowsDxgiSurface.cs",
        };
        var builder = new StringBuilder();
        foreach (var relative in files.Order(StringComparer.Ordinal))
        {
            var hash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Path.Combine(root, relative))))
                .ToLowerInvariant();
            builder.Append(relative).Append('=').Append(hash).Append('\n');
        }
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();
    }

    private static string RunGit(string arguments)
    {
        var startInfo = new ProcessStartInfo("git", arguments)
        {
            WorkingDirectory = FindRoot(),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        using var process = Process.Start(startInfo) ?? throw new InvalidOperationException("Unable to start git.");
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();
        if (process.ExitCode != 0) throw new InvalidOperationException(error);
        return output;
    }

    private static string FindRoot()
    {
        var directory = new DirectoryInfo(Environment.CurrentDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Doroti", "Doroti.slnx")))
                return directory.FullName;
            directory = directory.Parent;
        }
        throw new DirectoryNotFoundException("Unable to locate repository root.");
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        foreach (var slot in _slots) slot.Dispose();
        _visual.Dispose();
        _brush.Dispose();
        _copyFence.Dispose();
        _copyCommandList.Dispose();
        _copyAllocator.Dispose();
        _skiaContext.Dispose();
        _skiaBackend.Dispose();
        _graphicsDevice.Dispose();
        _on12.Dispose();
        _context11.Dispose();
        _device11.Dispose();
        _queue.Dispose();
        _device12.Dispose();
        _adapter.Dispose();
        _factory.Dispose();
        _compositor.Dispose();
        _dispatcherQueue.ShutdownQueue();
    }

    [DllImport("user32.dll", EntryPoint = "PeekMessageW", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool PeekMessage(out NativeMessage message, nint window, uint minimum, uint maximum, uint remove);

    [DllImport("user32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool TranslateMessage(in NativeMessage message);

    [DllImport("user32.dll", EntryPoint = "DispatchMessageW", ExactSpelling = true)]
    private static extern nint DispatchMessage(in NativeMessage message);
}

internal sealed class SurfaceSlot(int id) : IDisposable
{
    internal int Id { get; } = id;
    internal int Width { get; private set; }
    internal int Height { get; private set; }
    internal SurfaceSlotState State { get; private set; } = SurfaceSlotState.Free;
    internal long RetireAfterCommit { get; set; }
    internal CompositionDrawingSurface Surface { get; private set; } = null!;

    internal void Prepare(CompositionGraphicsDevice graphicsDevice, int width, int height)
    {
        if (State != SurfaceSlotState.Free)
            throw new InvalidOperationException($"Slot {Id} is not free.");
        if (Surface is null)
        {
            Surface = graphicsDevice.CreateDrawingSurface2(
                new SizeInt32(width, height),
                Microsoft.Graphics.DirectX.DirectXPixelFormat.R8G8B8A8UIntNormalized,
                Microsoft.Graphics.DirectX.DirectXAlphaMode.Premultiplied);
        }
        else if (Width != width || Height != height)
        {
            CompositionInterop.Resize(Surface, width, height);
        }
        Width = width;
        Height = height;
    }

    internal void Transition(SurfaceSlotState target)
    {
        var valid = (State, target) switch
        {
            (SurfaceSlotState.Free, SurfaceSlotState.Drawing) => true,
            (SurfaceSlotState.Drawing, SurfaceSlotState.GpuWorkQueued) => true,
            (SurfaceSlotState.GpuWorkQueued, SurfaceSlotState.DrawEnded) => true,
            (SurfaceSlotState.DrawEnded, SurfaceSlotState.PendingVisualCommit) => true,
            (SurfaceSlotState.PendingVisualCommit, SurfaceSlotState.Front) => true,
            (SurfaceSlotState.Front, SurfaceSlotState.Retired) => true,
            (SurfaceSlotState.Retired, SurfaceSlotState.Free) => true,
            _ => false,
        };
        if (!valid) throw new InvalidOperationException($"Illegal slot transition {State} -> {target} for {Id}.");
        State = target;
    }

    internal void ResetAfterInjectedFailure()
    {
        if (State != SurfaceSlotState.Drawing)
            throw new InvalidOperationException($"Failure cleanup expected Drawing, got {State}.");
        State = SurfaceSlotState.Free;
    }

    public void Dispose()
    {
        Surface?.Dispose();
        Surface = null!;
    }
}

internal static class CompositionInterop
{
    private static readonly Guid CompositorInteropIid = new("FAB19398-6D19-4D8A-B752-8F096C396069");
    private static readonly Guid DrawingSurfaceInteropIid = new("2D6355C2-AD57-4EAE-92E4-4C3EFF65D578");
    private static readonly Guid Texture2DIid = new("6F15AAF2-D208-4E89-9AB4-489535D34F9C");

    internal static unsafe CompositionGraphicsDevice CreateGraphicsDevice(
        Compositor compositor,
        ID3D11Device renderingDevice)
    {
        using var interop = ((WinRT.IWinRTObject)compositor).NativeObject.As(CompositorInteropIid);
        var thisPointer = interop.ThisPtr;
        var vtable = *(nint**)thisPointer;
        var create = (delegate* unmanaged[Stdcall]<nint, nint, nint*, int>)vtable[3];
        nint result = 0;
        Check(create(thisPointer, renderingDevice.NativePointer, &result));
        try
        {
            return WinRT.MarshalInterface<CompositionGraphicsDevice>.FromAbi(result);
        }
        finally
        {
            Marshal.Release(result);
        }
    }

    internal static unsafe ActiveDraw BeginDraw(CompositionDrawingSurface surface)
    {
        var interop = ((WinRT.IWinRTObject)surface).NativeObject.As(DrawingSurfaceInteropIid);
        var thisPointer = interop.ThisPtr;
        var vtable = *(nint**)thisPointer;
        var begin = (delegate* unmanaged[Stdcall]<nint, void*, Guid*, nint*, NativePoint*, int>)vtable[3];
        nint texture = 0;
        NativePoint offset = default;
        var textureIid = Texture2DIid;
        Check(begin(thisPointer, null, &textureIid, &texture, &offset));
        return new ActiveDraw(interop, texture, offset);
    }

    internal static unsafe void EndDraw(ActiveDraw draw)
    {
        if (draw.Ended) return;
        var thisPointer = draw.Interop.ThisPtr;
        var vtable = *(nint**)thisPointer;
        var end = (delegate* unmanaged[Stdcall]<nint, int>)vtable[4];
        Check(end(thisPointer));
        draw.Ended = true;
        draw.Interop.Dispose();
    }

    internal static unsafe void Resize(CompositionDrawingSurface surface, int width, int height)
    {
        using var interop = ((WinRT.IWinRTObject)surface).NativeObject.As(DrawingSurfaceInteropIid);
        var thisPointer = interop.ThisPtr;
        var vtable = *(nint**)thisPointer;
        var resize = (delegate* unmanaged[Stdcall]<nint, NativeSize, int>)vtable[5];
        Check(resize(thisPointer, new NativeSize(width, height)));
    }

    private static void Check(int hresult)
    {
        if (hresult < 0) Marshal.ThrowExceptionForHR(hresult);
    }
}

internal sealed class ActiveDraw(WinRT.IObjectReference interop, nint texturePointer, NativePoint offset)
{
    internal WinRT.IObjectReference Interop { get; } = interop;
    internal nint TexturePointer { get; set; } = texturePointer;
    internal NativePoint Offset { get; } = offset;
    internal bool Ended { get; set; }
}

internal enum SurfaceSlotState
{
    Free,
    Drawing,
    GpuWorkQueued,
    DrawEnded,
    PendingVisualCommit,
    Front,
    Retired,
}

internal sealed class InjectedValidationException : Exception;

[StructLayout(LayoutKind.Sequential)]
internal readonly record struct NativePoint(int X, int Y);

[StructLayout(LayoutKind.Sequential)]
internal readonly record struct NativeSize(int Width, int Height);

[StructLayout(LayoutKind.Sequential)]
internal readonly record struct NativeMessage(
    nint Window,
    uint Message,
    nuint WParam,
    nint LParam,
    uint Time,
    NativePoint Point,
    uint Private);

internal sealed record TraceEvent(
    long Timestamp,
    string Phase,
    int? Slot,
    int? Width,
    int? Height,
    string? State,
    string? Detail);

internal sealed record ValidationReport(
    string Schema,
    string Status,
    string GitCommit,
    string[] DirtyFiles,
    string SourceFingerprint,
    string WindowsAppSdkVersion,
    string OperatingSystem,
    string Adapter,
    int BeginDrawCount,
    int GpuCopyCount,
    int CpuReadbackCount,
    int GdiCopyCount,
    int BitmapEncodeCount,
    int NonZeroBeginDrawOffsetCount,
    int FormatMismatchCount,
    int SizeMismatchCount,
    int SampleMismatchCount,
    int SurfaceSlotCount,
    int SurfacePoolHighWater,
    int FrontReuseBeforeRetirement,
    int CommitRequestedCount,
    int CommitActionCompletedCount,
    int CommitBatchCompletedCount,
    int PendingCallbackCount,
    int FrontAdoptedCount,
    int RetirementCount,
    int SupersededCount,
    int FailureInjectionCount,
    int OpenDrawCount,
    int CheckedOutResourceCount,
    int LeakedSlotCount,
    string DeviceRemoval,
    string CheckedOutCleanupInjection,
    IReadOnlyList<TraceEvent> Trace);
