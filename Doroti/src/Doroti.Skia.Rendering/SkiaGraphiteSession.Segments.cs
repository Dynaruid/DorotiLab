using Doroti.Skia.RuntimeEffects;
using SkiaSharp;

namespace Doroti.Skia.Rendering;

public sealed partial class SkiaGraphiteSession
{
    /// <summary>The host owns the Metal texture until this frame completes.</summary>
    public SKSurface WrapMetalEffectSurface(int width, int height, nint texture)
    {
        CheckOwner();
        if (_vulkanOwner is not null) throw new InvalidOperationException("Metal surface requires a Metal session.");
        var frame = RecordingFrame;
        var backend = SKGraphiteBackendTexture.CreateMetal(width, height, texture)
            ?? throw new InvalidOperationException("Cannot wrap Metal effect texture.");
        frame.RetainUntilGpuCompletion(backend);
        var surface = SKSurface.Create(_recorder, backend, SKColorType.Rgba8888)
            ?? throw new InvalidOperationException("Cannot create Metal effect surface.");
        frame.RetainUntilGpuCompletion(surface);
        return SkiaGpuSurfaces.Register(surface, _recorder);
    }

    public Frame RecordingFrame
    {
        get
        {
            CheckOwner();
            return _recordingFrame ?? throw new InvalidOperationException("No active Graphite frame.");
        }
    }

    public sealed partial class Frame
    {
        private readonly List<SKGraphiteRecording> _segments = [];
        private readonly List<IDisposable> _segmentResources = [];
        private Action? _completeSegmentsOnCancellation;
        public int SubmittedSegments { get; private set; }

        /// <summary>Transfers ownership through the final host completion fence,
        /// including cancellation after an earlier segment was submitted.</summary>
        public void RetainUntilGpuCompletion(IDisposable resource)
        {
            CheckRecording();
            ArgumentNullException.ThrowIfNull(resource);
            _segmentResources.Add(resource);
        }

        /// <summary>Submit without ending this logical frame. The host callback
        /// must fence this same queue on cancellation or failed segment submission;
        /// it is never called on the normal submission path. External passes must
        /// restore borrowed image layouts before subsequent Skia recording.</summary>
        public void SubmitSegment(Action completeGpuWorkOnCancellation)
        {
            CheckRecording();
            ArgumentNullException.ThrowIfNull(completeGpuWorkOnCancellation);
            if (_completeSegmentsOnCancellation is not null &&
                _completeSegmentsOnCancellation != completeGpuWorkOnCancellation)
                throw new InvalidOperationException("All segments must share their queue completion owner.");
            _completeSegmentsOnCancellation = completeGpuWorkOnCancellation;
            var recording = _session._recorder.Snap()
                ?? throw new InvalidOperationException("Graphite segment Snap failed.");
            _segments.Add(recording);
            try
            {
                var inserted = _session._vulkanOwner is { } owner
                    ? owner.Insert(_session._context.Handle, recording.Handle, default, default)
                    : _session._context.InsertRecording(recording) == SKGraphiteInsertStatus.Success;
                if (!inserted || !_session._context.Submit(new SKGraphiteSubmitInfo { Sync = false }))
                    throw new InvalidOperationException("Graphite segment submission failed.");
                _session._vulkanOwner?.CheckHostState();
                _session._images.Commit();
                SkiaGpuSurfaces.CompleteRecording(_session._recorder, discarded: false);
                SubmittedSegments++;
            }
            catch
            {
                _session._faulted = true;
                throw;
            }
        }

        private void CheckRecording()
        {
            _session.CheckOwner();
            if (_returned || _submissionAttempted || _session._faulted || !ReferenceEquals(_session._recordingFrame, this))
                throw new InvalidOperationException("Frame is not recording.");
        }
    }
}
