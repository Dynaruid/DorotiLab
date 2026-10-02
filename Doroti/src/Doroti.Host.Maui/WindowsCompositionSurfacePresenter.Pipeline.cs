#if WINDOWS
using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;
using ResourceStates = Silk.NET.Direct3D12.ResourceStates;

namespace Doroti.Host.Maui;

internal sealed partial class WindowsCompositionSurfacePresenter
{
    private readonly SourceBank[] _sourceBanks = [new(), new()];
    private int _activeSourceBank;
    private long _nextPipelineSequence;
    internal int PendingPipelineFrames => _sourceBanks.Count(bank => bank.Frame is not null);
    internal int MaximumPipelineFrames { get; private set; }
    internal long CompletedPipelineFrames { get; private set; }
    internal bool SupportsPipeline => _nativeOutput is not null && _graphite is not null;

    private sealed class SourceBank
    {
        internal WindowsD3D12BackingStore? Store;
        internal PendingFrame? Frame;
        internal ulong ConsumerValue;
    }

    private sealed class PendingFrame(long sequence, MauiPaintCompletion completion,
        DorotiResizeEpoch target, Action<MauiPaintCompletion, bool> terminal)
    {
        internal long Sequence { get; } = sequence;
        internal MauiPaintCompletion Completion { get; } = completion;
        internal DorotiResizeEpoch Target { get; } = target;
        internal Action<MauiPaintCompletion, bool> Terminal { get; } = terminal;
        internal bool TerminalCommitted;
    }

    internal bool TryBeginPipelineFrame(DorotiWindowsDxgiHost host, DorotiResizeEpoch target)
    {
        EnsureDeviceAndVisual(host);
        if (!SupportsPipeline) return false;
        _sourceBanks[_activeSourceBank].Store ??= _backingStore;
        var completed = _nativeOutput!.CompletedCopy;
        var index = Array.FindIndex(_sourceBanks, bank => bank.Frame is null && completed >= bank.ConsumerValue);
        if (index < 0) return false;
        _activeSourceBank = index;
        var bank = _sourceBanks[index];
        _graphite!.SelectD3D12Slot(index);
        bank.Store ??= new WindowsD3D12BackingStore(_device12!, null);
        _backingStore = bank.Store;
        SurfaceChanged = _backingStore.EnsureSize(target.PhysicalWidth, target.PhysicalHeight);
        if (SurfaceChanged)
        {
            var handle = _device12!.CreateSharedHandle(_backingStore.Resource, null, null!);
            try { _graphite.ImportD3D12Resource(handle, target.PhysicalWidth, target.PhysicalHeight); }
            finally { CloseGraphiteHandle(handle); }
        }
        Width = target.PhysicalWidth;
        Height = target.PhysicalHeight;
        _graphiteSurface = _graphite.BeginD3D12Frame();
        return true;
    }

    internal void QueuePipelineFrame(MauiPaintCompletion completion, DorotiResizeEpoch target,
        Action<MauiPaintCompletion, bool> terminal)
    {
        var bank = _sourceBanks[_activeSourceBank];
        if (bank.Frame is not null) throw new InvalidOperationException("A pipeline source is still GPU-owned.");
        Flush(asynchronous: true);
        bank.ConsumerValue = 0;
        bank.Frame = new(++_nextPipelineSequence, completion, target, terminal);
        MaximumPipelineFrames = Math.Max(MaximumPipelineFrames, PendingPipelineFrames);
    }

    internal void CancelPipelineRecording()
    {
        _graphite?.ReleaseD3D12Frame();
        _graphiteSurface = null;
    }

    /// <summary>Producer/copy completion and D3D consumer completion are independent.
    /// Present acceptance commits a terminal but never makes a bank reusable.</summary>
    internal void PollPipelineFrames(Func<long> latestGeneration)
    {
        if (!SupportsPipeline) return;
        foreach (var item in _sourceBanks.Select((bank, index) => (bank, index))
            .Where(item => item.bank.Frame is not null).OrderBy(item => item.bank.Frame!.Sequence))
        {
            var bank = item.bank;
            var frame = bank.Frame!;
            if (frame.TerminalCommitted)
            {
                if (_nativeOutput!.CompletedCopy >= bank.ConsumerValue) { bank.Frame = null; CompletedPipelineFrames++; }
                continue;
            }
            _graphite!.SelectD3D12Slot(item.index);
            if (!_graphite.PollD3D12Slot()) continue;
            if (latestGeneration() != frame.Target.Generation)
            {
                frame.TerminalCommitted = true;
                frame.Terminal(frame.Completion, true);
                bank.Frame = null;
                CompletedPipelineFrames++;
                continue;
            }
            if (!_nativeOutput!.Prepare(bank.Store!.Resource, frame.Target.PhysicalWidth,
                frame.Target.PhysicalHeight, ResourceStates.Common, asynchronous: true)) break;
            var presented = false;
            _presentOnUiThread(frame.Target.Generation, () =>
            {
                if (latestGeneration() == frame.Target.Generation)
                    presented = _nativeOutput.Present(_nativeContentTop(), frame.Target.Generation, asynchronous: true);
            });
            bank.ConsumerValue = _nativeOutput.SubmittedCopy;
            frame.TerminalCommitted = true;
            frame.Terminal(frame.Completion, !presented);
        }
        // Polling may switch to an older completed source. Restore the most
        // recently recorded source before any serial/native replay or resize.
        _graphite!.SelectD3D12Slot(_activeSourceBank);
        _backingStore = _sourceBanks[_activeSourceBank].Store ?? _backingStore;
    }

    private void ReleasePipelineStores()
    {
        // Output.Dispose and Graphite.Dispose have already established the
        // last consumer boundary. Never treat a wait timeout as retirement.
        foreach (var bank in _sourceBanks)
        {
            if (bank.Frame is { TerminalCommitted: false } frame)
                frame.Terminal(frame.Completion, true);
            bank.Frame = null;
            if (bank.Store is not null && !ReferenceEquals(bank.Store, _backingStore)) bank.Store.Dispose();
            bank.Store = null;
            bank.ConsumerValue = 0;
        }
        _activeSourceBank = 0;
    }
}
#endif
