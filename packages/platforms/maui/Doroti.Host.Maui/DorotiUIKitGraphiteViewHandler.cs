#if IOS || MACCATALYST
using System.Diagnostics;
using CoreAnimation;
using CoreGraphics;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using Foundation;
using Metal;
using MetalKit;
using Microsoft.Maui;
using Microsoft.Maui.Handlers;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using UIKit;

namespace Doroti.Host.Maui;

public sealed class DorotiUIKitGraphiteViewHandler
    : ViewHandler<DorotiGraphiteView, DorotiUIKitGraphiteView>
{
    private static readonly CommandMapper<
        DorotiGraphiteView,
        DorotiUIKitGraphiteViewHandler
    > Commands = new(ViewCommandMapper)
    {
        [nameof(ISKGLView.InvalidateSurface)] = (handler, _, _) =>
            handler.PlatformView.RequestFramePulse(),
    };

    public DorotiUIKitGraphiteViewHandler()
        : base(ViewMapper, Commands) { }

    protected override DorotiUIKitGraphiteView CreatePlatformView() => new();

    protected override void ConnectHandler(DorotiUIKitGraphiteView platformView)
    {
        base.ConnectHandler(platformView);
        platformView.Connect(VirtualView);
    }

    protected override void DisconnectHandler(DorotiUIKitGraphiteView platformView)
    {
        platformView.Disconnect();
        base.DisconnectHandler(platformView);
    }
}

/// <summary>UIKit owns input/layout; this view owns the Metal queue and Graphite recorder.</summary>
public sealed class DorotiUIKitGraphiteView : MTKView, IMTKViewDelegate
#if IOS && !MACCATALYST
        , IUIKitAnimatedViewport
#endif
{
#if IOS && !MACCATALYST
    private readonly UIKitAnimatedViewport _animatedViewport;
    private bool _renderingViewport;
    private bool _rotationRequiresExactBacking;
    private SKSizeI _viewportPixels;
    private readonly System.Collections.Concurrent.ConcurrentQueue<(
        PendingFrame Frame,
        MTLCommandBufferStatus Status,
        string? Error
    )> _completedFrames = new();
    UIKitAnimatedViewport IUIKitAnimatedViewport.AnimatedViewport => _animatedViewport;
#endif
    private readonly IMTLCommandQueue _queue;
    private readonly bool _profileBlur =
        Environment.GetEnvironmentVariable("DOROTI_VARIABLE_BLUR_PROFILE") == "1";
#if IOS && !MACCATALYST
    internal event Action<double, MauiPaintCompletion>? ViewportPresented;
#endif
    private IosFrameLoopDiagnostics? _frameLoop;
    private long _pulseId;
    private long _activePulse;
    private long _preparedGeneration = -1;
    private long _lastPreparedScene;
#if IOS && !MACCATALYST
    private bool _pipelineDisplayLink;
    private bool _replayRequested;
#endif
    private readonly object _presentationGate = new();
    private readonly Queue<double> _presentationIntervals = new();
    private double _lastPresentationTime;
    private long _presentedDrawables;
    private long _commandBuffersCommitted;
    private long _commandBuffersCompleted;
    private long _commandBuffersErrored;
    private SkiaGraphiteSession? _session;
    private DorotiGraphiteView? _owner;

    // All mutations, including retirement, run on the UIKit/recorder owner thread.
    // A failed terminal-marker submission must keep its native resources rooted.
    private static readonly HashSet<DorotiUIKitGraphiteView> RetiringViews = new(
        ReferenceEqualityComparer.Instance
    );

    // Native NSObject hashes can change when their handles are disposed.
    // Retirement identity must remain stable until the pending entry is removed.
    private readonly HashSet<PendingFrame> _pending = new(ReferenceEqualityComparer.Instance);
    private DorotiGraphiteView? _resourceOwner;
    private bool _releaseRequested;
    private bool _resourcesReleased;
    private bool _faulted;
    private bool _frameBackpressure;
    private readonly MauiFrameWakeQueue _frameWakes = new();
    private long _preparedFrameworkPulses;
    private long _preparedWhileGpuFull;
    private long _rejectedAdmissions;
    private long _completedGpuFrames;
    private int _maximumGpuFrames;
    private string? _frameFallback;
#if MACCATALYST
    private nfloat _lastScale;
    private CGSize _lastSize;
#endif
    private long _generation;
    private bool _drawing;
    private NSTimer? _retirementTimer;
#if IOS || MACCATALYST
    private NSObject? _inactiveObserver;
    private NSObject? _activeObserver;
    private NSObject? _sceneInactiveObserver;
    private NSObject? _sceneActiveObserver;
    private bool _suspended;
    private volatile bool _ownerActiveSnapshot;
#endif

    private static IMTLDevice RequireMetalDevice()
    {
        NativeFrameConfiguration.ValidateEnvironment();
        return MTLDevice.SystemDefault
            ?? throw new PlatformNotSupportedException("Doroti Graphite requires a Metal-capable device.");
    }

    public DorotiUIKitGraphiteView()
        : base(
            CGRect.Empty,
            RequireMetalDevice()
        )
    {
        _queue =
            Device!.CreateCommandQueue()
            ?? throw new InvalidOperationException("Metal queue creation failed.");
        ColorPixelFormat = MTLPixelFormat.BGRA8Unorm;
        FramebufferOnly = false;
        AutoResizeDrawable = false;
#if IOS && !MACCATALYST
        _animatedViewport = new(this, RenderViewport, CanRenderViewport, CanFinishViewport);
        _animatedViewport.AnimationChanged += active =>
        {
            if (active)
            {
                _rotationRequiresExactBacking = false;
                StopPipelineDisplayLink();
            }
            EnableSetNeedsDisplay = !active;
            if (!active && _owner?.FrameworkFrameRequested?.Invoke() == true) RequestFramePulse();
        };
        // Keep text and controls at their rendered size as UIKit interpolates
        // rotation bounds. Center the image without stretching either axis;
        // the viewport follows presentation bounds for the entire transition.
        ContentMode = UIViewContentMode.Center;
        Layer.ContentsGravity = CALayer.GravityCenter;
#else
        ContentMode = UIViewContentMode.Redraw;
        Layer.ContentsGravity = CALayer.GravityTopLeft;
#endif
        Layer.MasksToBounds = true;
        Paused = true;
        EnableSetNeedsDisplay = true;
        Opaque = false;
        BackgroundColor = UIColor.Clear;
        MultipleTouchEnabled = true;
        Delegate = this;
#if IOS || MACCATALYST
        _suspended = UIApplication.SharedApplication.ApplicationState != UIApplicationState.Active;
        _inactiveObserver = NSNotificationCenter.DefaultCenter.AddObserver(
            UIApplication.WillResignActiveNotification,
            _ =>
            {
                if (Window?.WindowScene is null)
                {
                    SuspendRendering();
                }
            }
        );
        _activeObserver = NSNotificationCenter.DefaultCenter.AddObserver(
            UIApplication.DidBecomeActiveNotification,
            _ =>
            {
                if (Window?.WindowScene is null)
                {
                    ResumeRendering();
                }
            }
        );
        _sceneInactiveObserver = NSNotificationCenter.DefaultCenter.AddObserver(
#if MACCATALYST
            UIScene.DidEnterBackgroundNotification,
#else
            UIScene.WillDeactivateNotification,
#endif
            notification =>
            {
                if (Window?.WindowScene == notification.Object)
                {
                    SuspendRendering();
                }
            }
        );
        _sceneActiveObserver = NSNotificationCenter.DefaultCenter.AddObserver(
            UIScene.DidActivateNotification,
            notification =>
            {
                if (Window?.WindowScene == notification.Object)
                {
                    ResumeRendering();
                }
            }
        );
#endif
    }

#if IOS && !MACCATALYST
    private void StopPipelineDisplayLink()
    {
        if (!_pipelineDisplayLink) return;
        _pipelineDisplayLink = false;
        Paused = true;
        EnableSetNeedsDisplay = true;
    }
#endif

    internal void RequestFramePulse()
    {
        _frameWakes.Request(prepareFramework: true);
#if IOS && !MACCATALYST
        if (!_renderingViewport && !_animatedViewport.IsAnimating
            && _owner?.PlatformViews?.HasComposition != true
            && !_suspended && OwnerIsActive && !_releaseRequested && !_faulted)
        {
            _replayRequested |= _owner?.FrameworkFrameRequested?.Invoke() != true
                && _owner?.PreparedScene?.Invoke() is null;
            _pipelineDisplayLink = true;
            EnableSetNeedsDisplay = false;
            Paused = false;
            return;
        }
#endif
        #if IOS && !MACCATALYST
        StopPipelineDisplayLink();
        EnableSetNeedsDisplay = !_animatedViewport.IsAnimating;
        #endif
        SetNeedsDisplay();
    }

    internal void Connect(DorotiGraphiteView owner)
    {
        if (_releaseRequested || _resourcesReleased)
        {
            throw new InvalidOperationException(
                "A disconnected Metal view requires a fresh handler/native view."
            );
        }

        // Other windows may still drain their own queues. Their retained resources
        // do not belong to this new surface and must not block its creation.
        _resourceOwner = _owner = owner;
        RequestFramePulse();
    }

#if IOS || MACCATALYST
    internal UIKitPlatformRasterSurface CreatePlatformRasterSurface() => new(Device!, _queue);
#endif

    private readonly TaskCompletionSource _retired = new(
        TaskCreationOptions.RunContinuationsAsynchronously
    );

    internal Task RetireAsync()
    {
        Disconnect();
        return _retired.Task;
    }

    internal void Disconnect()
    {
        if (_releaseRequested)
        {
            return;
        }

        _releaseRequested = true;
#if IOS && !MACCATALYST
        StopPipelineDisplayLink();
        _animatedViewport.Dispose();
#endif
        _generation++;
        _owner = null;
#if IOS || MACCATALYST
        _resourceOwner?.PlatformViews?.DetachSurface();
#endif
#if IOS || MACCATALYST
        RemoveLifecycleObserver(ref _inactiveObserver);
        RemoveLifecycleObserver(ref _activeObserver);
        RemoveLifecycleObserver(ref _sceneInactiveObserver);
        RemoveLifecycleObserver(ref _sceneActiveObserver);
#endif
        _session?.StopAcceptingFrames();
        RetiringViews.Add(this);
        if (!_drawing && _pending.Count == 0)
        {
            ReleaseGpuResources();
        }

        if (!_resourcesReleased)
        {
            _retirementTimer = NSTimer.CreateScheduledTimer(
                TimeSpan.FromSeconds(5),
                _ =>
                {
                    if (_resourcesReleased)
                    {
                        return;
                    }

                    _faulted = true;
                    // A deadline is diagnostic, never evidence of device loss.
                    // Keep this generation and its drawable leases until completion.
                    var error = new TimeoutException(
                        "UIKit Metal retirement exceeded five seconds; retaining GPU resources and rejecting new generations."
                    );
                    System.Diagnostics.Trace.TraceError(error.ToString());
                    try
                    {
                        _resourceOwner?.FailGraphite(null, error);
                    }
                    catch (Exception callbackError)
                    {
                        System.Diagnostics.Trace.TraceError(callbackError.ToString());
                    }
                }
            );
        }
    }

#if IOS || MACCATALYST
    private bool OwnerIsActive =>
        _ownerActiveSnapshot = Window?.WindowScene is { } scene
#if MACCATALYST
            ? scene.ActivationState
                is UISceneActivationState.ForegroundActive
                    or UISceneActivationState.ForegroundInactive
#else
            ? scene.ActivationState == UISceneActivationState.ForegroundActive
#endif
            : UIApplication.SharedApplication.ApplicationState == UIApplicationState.Active;

    private void SuspendRendering()
    {
        if (!_suspended)
        {
            _generation++;
        }

        _suspended = true;
        _ownerActiveSnapshot = false;
#if IOS && !MACCATALYST
        StopPipelineDisplayLink();
#endif
#if IOS && !MACCATALYST
        _animatedViewport.Stop();
#endif
        _frameLoop?.Record("lifecycle-suspend", _activePulse, _generation, _pending.Count);
    }

    private void ResumeRendering()
    {
        _suspended = !OwnerIsActive;
        _frameLoop?.Record("lifecycle-resume", _activePulse, _generation, _pending.Count,
            reason: _suspended ? "inactive" : "active");
        if (!_suspended && !_releaseRequested && !_faulted)
        {
#if IOS && !MACCATALYST
            _animatedViewport.LayoutChanged();
#endif
            RequestFramePulse();
        }
    }

    private static void RemoveLifecycleObserver(ref NSObject? observer)
    {
        if (observer is null)
        {
            return;
        }

        NSNotificationCenter.DefaultCenter.RemoveObserver(observer);
        observer.Dispose();
        observer = null;
    }
#endif

    public override void MovedToWindow()
    {
        base.MovedToWindow();
#if IOS && !MACCATALYST
        if (Window is null)
        {
            StopPipelineDisplayLink();
            _animatedViewport.Stop();
        }
#endif
#if IOS || MACCATALYST
        _suspended = !OwnerIsActive;
#endif
        if (!_releaseRequested && Window is not null)
        {
#if IOS && !MACCATALYST
            _animatedViewport.LayoutChanged();
#endif
            RequestFramePulse();
        }
    }

    public override void LayoutSubviews()
    {
        base.LayoutSubviews();
#if IOS && !MACCATALYST
        if (!_releaseRequested)
        {
            _animatedViewport.LayoutChanged();
        }
    }

    public override void SafeAreaInsetsDidChange()
    {
        base.SafeAreaInsetsDidChange();
        if (!_releaseRequested)
        {
            _animatedViewport?.LayoutChanged();
        }
    }

    private void RenderViewport(CGSize size)
    {
        if (_releaseRequested || Window is null)
        {
            return;
        }
#else
        if (
            _releaseRequested
            || Window is null
            || Bounds.Width <= 0
            || Bounds.Height <= 0
            || (Bounds.Size.Equals(_lastSize) && ContentScaleFactor == _lastScale)
        )
        {
            return;
        }

        _lastSize = Bounds.Size;
        _lastScale = ContentScaleFactor;
        var size = Bounds.Size;
#endif
#if IOS || MACCATALYST
        var previousPresentation = PresentsWithTransaction;
#endif
        CATransaction.Begin();
        try
        {
            CATransaction.DisableActions = true;
#if IOS && !MACCATALYST
            Layer.ContentsGravity = CALayer.GravityCenter;
#else
            Layer.ContentsGravity = CALayer.GravityTopLeft;
#endif
#if IOS && !MACCATALYST
            var scale = _animatedViewport.Scale;
            _viewportPixels = new SKSizeI(
                Math.Max(1, (int)Math.Round(size.Width * scale)),
                Math.Max(1, (int)Math.Round(size.Height * scale))
            );
            // Reallocating the drawable pool every pulse can stall nextDrawable
            // past the next display. Keep capacity through rotation and draw
            // an unscaled, centered viewport; UIKit clips the animated bounds.
            // Native overlay composition retains its exact-size surface contract.
            var backingSize =
                _animatedViewport.IsAnimating && !_rotationRequiresExactBacking
                    && _owner?.PlatformViews?.HasComposition != true
                    ? _animatedViewport.AnimationExtent
                    : size;
#else
            var scale = ContentScaleFactor;
            var backingSize = size;
#endif
            var drawableSize = new CGSize(
                Math.Max(1, Math.Round(backingSize.Width * scale)),
                Math.Max(1, Math.Round(backingSize.Height * scale))
            );
            if (!DrawableSize.Equals(drawableSize))
                DrawableSize = drawableSize;
            // MTKView derives a contents scale from drawable/model bounds.
            // During rotation those sizes intentionally differ. Restore the
            // owning screen's scale after resizing so it cannot feed back into
            // the next drawable size, text density, or pointer conversion.
            Layer.ContentsScale = scale;
#if IOS || MACCATALYST
            // The new drawable must accompany UIKit's rotation geometry. An
            // independently queued present can otherwise replace the old image
            // partway through the rotation, making the layout visibly jump.
            PresentsWithTransaction = true;
#endif
#if IOS && !MACCATALYST
            _renderingViewport = true;
#endif
            _frameWakes.Request(prepareFramework: true);
            Draw();
        }
        finally
        {
#if IOS && !MACCATALYST
            _renderingViewport = false;
#endif
            CATransaction.Commit();
#if IOS || MACCATALYST
            PresentsWithTransaction = previousPresentation;
#endif
        }
        RequestFramePulse();
    }

#if IOS && !MACCATALYST
    private bool CanRenderViewport()
    {
        DrainCompletedFrames();
        return !_drawing && (_pending.Count == 0 || _faulted || _releaseRequested
            || HasStableRotationBacking && _pending.Count < NativeFrameAdmissionPolicy.ShaderFrameLimit);
    }

    private bool CanFinishViewport()
    {
        DrainCompletedFrames();
        return !_drawing && (_pending.Count == 0 || _faulted || _releaseRequested);
    }

    // The drawable generation stays fixed while only the clipped shader viewport
    // changes. Each recording retains its own scene/GPU leases, so the next CPU
    // frame can prepare while the previous GPU frame finishes. Pool resizing,
    // native composition and the final exact-size frame still drain completely.
    private bool HasStableRotationBacking
    {
        get
        {
            if (!_animatedViewport.IsAnimating || _rotationRequiresExactBacking
                || _owner?.PlatformViews?.HasComposition == true
                || _pending.Any(p => p.Generation != _generation || p.PlatformFrame is not null))
                return false;
            var extent = _animatedViewport.AnimationExtent;
            var scale = _animatedViewport.Scale;
            return DrawableSize.Equals(new CGSize(
                Math.Max(1, Math.Round(extent.Width * scale)),
                Math.Max(1, Math.Round(extent.Height * scale))));
        }
    }

    private void DrainCompletedFrames() => DrainCompletedFramesCore(allowDuringDraw: false);

    private void DrainCompletedFramesCore(bool allowDuringDraw)
    {
        if (_drawing && !allowDuringDraw) return;
        while (_completedFrames.TryDequeue(out var completed))
            Retire(completed.Frame, completed.Status, completed.Error);
    }
#endif

    public void DrawableSizeWillChange(MTKView view, CGSize size)
    {
        _generation++;
    }

    public void Draw(MTKView view)
    {
        if (!_drawing) DrawFrame(_frameWakes.Take());
    }

    private void DrawFrame(bool prepareFramework)
    {
        if (_drawing) return;
        _drawing = true;
        try { DrawFrameCore(prepareFramework); }
        finally
        {
            _drawing = false;
#if IOS && !MACCATALYST
            if (_pipelineDisplayLink && (_owner?.PlatformViews?.HasComposition == true
                || _renderingViewport || _animatedViewport.IsAnimating || _suspended || _faulted || _releaseRequested
                || _owner?.FrameworkFrameRequested?.Invoke() != true
                    && _owner?.PreparedScene?.Invoke() is null && _pending.Count == 0))
            {
                StopPipelineDisplayLink();
                EnableSetNeedsDisplay = !_animatedViewport.IsAnimating;
                if (!_suspended && !_releaseRequested && !_faulted
                    && _owner?.FrameworkFrameRequested?.Invoke() == true) SetNeedsDisplay();
            }
#endif
            if (_releaseRequested && _pending.Count == 0) ReleaseGpuResources();
        }
    }

    private void DrawFrameCore(bool prepareFramework)
    {
#if IOS && !MACCATALYST
        // One display link owns rotation frames. A queued SetNeedsDisplay must
        // not add an old-size replay between geometry updates.
        if (_animatedViewport.IsAnimating && !_renderingViewport)
            return;
        DrainCompletedFramesCore(allowDuringDraw: true);
#endif
        var owner = _owner;
        if (_releaseRequested || _faulted || Window is null || owner is null)
        {
            return;
        }
#if IOS || MACCATALYST
        // Invalidation/layout can still arrive while UIKit is moving to the
        // background. Metal must not receive new work until activation.
        if (_suspended || !OwnerIsActive)
        {
            return;
        }
#endif
        if (!prepareFramework && _preparedGeneration != _generation)
        {
            prepareFramework = true;
        }
        var pulse = _activePulse = ++_pulseId;
        _frameLoop ??= _profileBlur ? new(
            NativeFrameConfiguration.Mode,
            CAAnimation.CurrentMediaTime()) : null;
        _frameLoop?.Record(prepareFramework ? "pulse" : "raster-wake", pulse, _generation, _pending.Count);
        var shaderFramePipeline = false;
        var frameworkPrepared = !prepareFramework;
        var needsTransaction = owner.PlatformViews?.IsConfigured == true;
#if IOS && !MACCATALYST
        // Preserve the first frame's drawable/transaction ordering. Preparing
        // ahead is useful only when a previous shader frame is actually in flight.
        var retainedRotationBacking = HasStableRotationBacking && _pending.Count > 0;
#endif
        try
        {
            var admission = NativeFrameAdmissionPolicy.PrepareAndDecide(
                () =>
                {
                    _preparedFrameworkPulses++;
                    if (_pending.Count >= NativeFrameAdmissionPolicy.ShaderFrameLimit) _preparedWhileGpuFull++;
                    EnsureSession();
#if IOS && !MACCATALYST
                    var pixels = _viewportPixels.Width > 0 ? _viewportPixels
                        : new SKSizeI((int)DrawableSize.Width, (int)DrawableSize.Height);
#else
                    var pixels = new SKSizeI((int)DrawableSize.Width, (int)DrawableSize.Height);
#endif
                    var start = Stopwatch.GetTimestamp();
                    _frameLoop?.Record("framework-start", pulse, _generation, _pending.Count);
                    owner.PrepareFrameworkFrame?.Invoke(new(_session, pixels.Width, pixels.Height,
#if IOS && !MACCATALYST
                        Math.Max(1, (double)_animatedViewport.Scale),
#else
                        Math.Max(1, (double)ContentScaleFactor),
#endif
                        _generation,
                        GetType().FullName!, "UIKit/MTKView/Graphite-Metal", DorotiFrameClock.Now,
                        _frameLoop is null ? null : (stage, duration) =>
                            _frameLoop.Record(stage, pulse, _generation, _pending.Count, duration: duration)));
                    frameworkPrepared = true;
                    _preparedGeneration = _generation;
                    if (owner.PreparedScene?.Invoke() is { } scene && scene.SceneSequence != _lastPreparedScene)
                    {
                        _lastPreparedScene = scene.SceneSequence;
                        _frameLoop?.Record("scene-created", pulse, _generation, _pending.Count, scene);
                    }
                    _frameLoop?.Record("framework-end", pulse, _generation, _pending.Count,
                        duration: Stopwatch.GetElapsedTime(start).TotalMilliseconds);
                },
                () => owner.ShaderSceneAdmission?.Invoke() ?? SkiaShaderSceneAdmission.noNewScene,
                () =>
                {
#if IOS && !MACCATALYST
                    DrainCompletedFramesCore(allowDuringDraw: true);
#endif
                    return _pending.Count;
                }, owner.PlatformViews?.HasComposition == true,
                _pending.Any(p => p.PlatformFrame is not null),

#if IOS && !MACCATALYST
                (_renderingViewport || _animatedViewport.IsAnimating) && !retainedRotationBacking,
#else
                _preparedGeneration >= 0 && _preparedGeneration != _generation,
#endif
                prepareFramework);
            shaderFramePipeline = admission.FreshOnly;
            needsTransaction = admission.SynchronizePresentation;
#if IOS && !MACCATALYST
            // CPU/GPU overlap does not change UIKit's presentation contract:
            // every rotation drawable is presented in the geometry transaction.
            needsTransaction |= _renderingViewport || _animatedViewport.IsAnimating;
#endif
            _frameFallback = admission.FreshOnly ? null : admission.Reason;
            _frameLoop?.Record("admission", pulse, _generation, _pending.Count, reason: admission.Reason);
#if IOS && !MACCATALYST
            if (retainedRotationBacking
                && owner.ShaderSceneAdmission?.Invoke() == SkiaShaderSceneAdmission.nativeScene)
            {
                // A callback introduced native content. Drain the retained pool
                // and retry with an exact-size drawable before composing it.
                _rotationRequiresExactBacking = true;
                _frameBackpressure = true;
                return;
            }
#endif
            if (!admission.Admitted)
            {
                _rejectedAdmissions++;
                _frameBackpressure = true;
                return;
            }
        }
        catch (Exception exception)
        {
            _faulted = true;
            _session?.StopAcceptingFrames();
            owner.FailGraphite(null, exception);
            return;
        }
#if IOS && !MACCATALYST
        if (_pipelineDisplayLink && owner.PreparedScene?.Invoke() is null
            && owner.FrameworkFrameRequested?.Invoke() != true)
        {
            if (_pending.Count != 0) return;
            if (!_replayRequested)
            {
                StopPipelineDisplayLink();
                EnableSetNeedsDisplay = true;
                return;
            }
            _replayRequested = false;
        }
#endif
        _frameBackpressure = false;
        var recordingAhead = _pending.Count > 0;
        SkiaGraphiteSession.Frame? frame = null;
        ICAMetalDrawable? drawable = null;
        MauiSkiaPaintContext? paint = null;
        var submitted = false;
#if IOS || MACCATALYST
        UIKitPlatformViewHost.PreparedFrame? platformFrame = null;
        var compositionTransaction = needsTransaction;
        var previousPresentation = PresentsWithTransaction;
        if (compositionTransaction)
        {
            CATransaction.Begin();
            CATransaction.DisableActions = true;
            // A configured coordinator alone does not mean native overlays are
            // changing. Shader-only frames use normal asynchronous presentation;
            // their atomic paint admission rejects a newly inserted native scene.
            PresentsWithTransaction = needsTransaction;
        }
        #if IOS && !MACCATALYST
        else PresentsWithTransaction = false;
        #endif
#endif
        try
        {
            var drawableStart = Stopwatch.GetTimestamp();
            drawable = CurrentDrawable;
            _frameLoop?.Record("drawable", pulse, _generation, _pending.Count,
                duration: Stopwatch.GetElapsedTime(drawableStart).TotalMilliseconds);
            if (drawable is null)
            {
                _frameWakes.Request(prepareFramework: false);
                SetNeedsDisplay();
                return;
            }
            EnsureSession();
            var width = checked((int)drawable.Texture.Width);
            var height = checked((int)drawable.Texture.Height);
            frame = _session!.BeginMetalFrame(width, height, drawable.Texture.Handle);
            frame.Surface.Canvas.Clear(SKColors.Transparent);
#if IOS && !MACCATALYST
            if (_viewportPixels.Width > 0 && _viewportPixels.Height > 0)
            {
                var viewportWidth = Math.Min(width, _viewportPixels.Width);
                var viewportHeight = Math.Min(height, _viewportPixels.Height);
                frame.Surface.Canvas.Translate(
                    (width - viewportWidth) / 2f,
                    (height - viewportHeight) / 2f
                );
                frame.Surface.Canvas.ClipRect(SKRect.Create(viewportWidth, viewportHeight));
                width = viewportWidth;
                height = viewportHeight;
            }
#endif
            var generation = _generation;
            paint = new(
                frame.Surface,
                _session,
                width,
                height,
#if IOS && !MACCATALYST
                Math.Max(1, (double)_animatedViewport.Scale),
#else
                Math.Max(1, (double)ContentScaleFactor),
#endif
                generation,
                GetType().FullName!,
                "UIKit/MTKView/Graphite-Metal"
            );
            paint.RequireNewShaderScene = shaderFramePipeline;
            paint.FrameworkPrepared = frameworkPrepared;
            if (_frameLoop is not null)
            {
                paint.CpuStageMeasured = (stage, duration) =>
                    _frameLoop.Record(stage, pulse, generation, _pending.Count, duration: duration);
                paint.ScenePrepared = scene =>
                {
                    if (scene.SceneSequence == _lastPreparedScene) return;
                    _lastPreparedScene = scene.SceneSequence;
                    _frameLoop.Record("scene-created", pulse, generation, _pending.Count, scene);
                };
            }
            var rasterStart = Stopwatch.GetTimestamp();
            _frameLoop?.Record("raster-start", pulse, generation, _pending.Count);
            owner.PaintGraphite(paint);
            _preparedGeneration = generation;
            _frameLoop?.Record("raster-end", pulse, generation, _pending.Count, paint.Completion,
                duration: Stopwatch.GetElapsedTime(rasterStart).TotalMilliseconds);
#if IOS || MACCATALYST
            platformFrame = owner.PlatformViews?.TakePending();
#if IOS && !MACCATALYST
            // Recheck the recorded scene before selecting native/rotation synchronization.
            PresentsWithTransaction = _renderingViewport
                || _animatedViewport.IsAnimating || !paint.ShaderOnly || platformFrame is not null
                || owner.PlatformViews?.HasComposition == true;
#endif
#endif
            if (paint.SkipPresent || _releaseRequested || generation != _generation
                || recordingAhead && paint.Completion?.IsNewFrame != true)
            {
                _frameBackpressure |= recordingAhead;
                if (shaderFramePipeline && _pending.Count == 0) SetNeedsDisplay();
                frame.CancelRecording();
                frame = null;
                if (paint.Completion is { } stale)
                {
                    owner.CompleteGraphite(stale, true);
                }

                return;
            }
#if IOS || MACCATALYST
            platformFrame?.Submit();
#endif
            submitted = true;
            frame.Submit();
            // Transfer ownership before attempting the terminal marker. Even a
            // failed commit must retain textures; it is not GPU completion.
            var pending = new PendingFrame(
                frame,
                drawable,
                owner,
                paint.Completion,
                generation,
                pulse
#if IOS || MACCATALYST
                ,
                platformFrame
#endif
            );
#if IOS || MACCATALYST
            platformFrame = null;
#endif
            _pending.Add(pending);
            _maximumGpuFrames = Math.Max(_maximumGpuFrames, _pending.Count);
            _frameLoop?.Record("submitted", pulse, generation, _pending.Count, pending.Completion,
                reason: pending.Completion?.IsNewFrame == true ? "new" : "replay");
            frame = null;
            drawable = null;
            CommitTerminal(pending, present: true);
        }
        catch (Exception exception)
        {
            _faulted = true;
            _session?.StopAcceptingFrames();
#if IOS || MACCATALYST
            owner.PlatformViews?.CancelPending();
            try
            {
                platformFrame?.Abort();
            }
            catch (Exception rollbackError)
            {
                System.Diagnostics.Trace.TraceError(rollbackError.ToString());
            }
            if ((frame is not null && submitted) || platformFrame?.HasSubmitted == true)
#else
            if (frame is not null && submitted)
#endif
            {
                if (!submitted)
                {
                    frame?.CancelRecording();
                    frame = null;
                }
                var pending = new PendingFrame(
                    frame,
                    drawable!,
                    owner,
                    null,
                    _generation,
                    pulse
#if IOS || MACCATALYST
                    ,
                    platformFrame
#endif
                );
#if IOS || MACCATALYST
                platformFrame = null;
#endif
                _pending.Add(pending);
                _maximumGpuFrames = Math.Max(_maximumGpuFrames, _pending.Count);
                frame = null;
                drawable = null;
                try
                {
                    CommitTerminal(pending, present: false);
                }
                catch (Exception markerError)
                {
                    System.Diagnostics.Trace.TraceError(markerError.ToString());
                }
            }
            owner.FailGraphite(paint?.Completion, exception);
        }
        finally
        {
            frame?.CancelRecording();
            drawable?.Dispose();
#if IOS || MACCATALYST
            platformFrame?.Dispose();
            if (compositionTransaction)
            {
                CATransaction.Commit();
            }
            PresentsWithTransaction = previousPresentation;
#endif
        }
    }

    private void WakePendingAdmission()
    {
        if (!_frameBackpressure || _releaseRequested || _faulted) return;
#if IOS && !MACCATALYST
        if (_pipelineDisplayLink)
        {
            RequestFramePulse();
            return;
        }
#endif
        _frameWakes.Request(prepareFramework: false);
        SetNeedsDisplay();
    }

    private void EnsureSession()
    {
        _session ??= SkiaGraphiteSession.CreateMetal(Device!.Handle, _queue.Handle, Math.Max(1, ++_generation));
        _session.NativeTextureImporter ??= new AppleNativeTextureImporter(_session, Device!.Handle);
        _session.GpuEffects ??= new AppleGpuEffects(Device!, _queue, _session);
        if (_frameLoop is not null)
            _session.CpuStageMeasured = (stage, duration) =>
                _frameLoop.Record(stage, _activePulse, _generation, _pending.Count, duration: duration);
    }

    private sealed record PendingFrame(
        SkiaGraphiteSession.Frame? Frame,
        ICAMetalDrawable Drawable,
        DorotiGraphiteView Owner,
        MauiPaintCompletion? Completion,
        long Generation,
        long PulseId
#if IOS || MACCATALYST
        ,
        UIKitPlatformViewHost.PreparedFrame? PlatformFrame
#endif
    )
    {
        internal IosFrameLifetime Lifetime { get; } = new();
        private bool _drawableReleased;
        internal void ReleaseDrawableIfSafe()
        {
            if (_drawableReleased || !Lifetime.Retired) return;
            Drawable.Dispose();
            _drawableReleased = true;
        }
    }

    private void CommitTerminal(PendingFrame pending, bool present)
    {
        if (pending.Lifetime.TerminalCommitted) return;
        try
        {
            using var command =
                _queue.CommandBuffer()
                ?? throw new InvalidOperationException(
                    "Metal terminal buffer creation failed; retaining GPU resources."
                );
            var transactionPresentation = false;
            if (present && AppleMetalPresentation.CanObserve(pending.Drawable))
            {
                pending.Drawable.AddPresentedHandler(drawable =>
                {
                    var timestamp = drawable.PresentedTime;
                    if (timestamp <= 0)
                    {
                        _frameLoop?.Record("presentation-dropped", pending.PulseId, pending.Generation,
                            completion: pending.Completion);
                        return;
                    }
                    _frameLoop?.Record("displayed", pending.PulseId, pending.Generation,
                        completion: pending.Completion, presentedTime: timestamp);
#if IOS && !MACCATALYST
                    if (pending.Completion is { } completion)
                        ViewportPresented?.Invoke(timestamp, completion);
#endif
                    if (_profileBlur) lock (_presentationGate)
                    {
                        _presentedDrawables++;
                        if (_lastPresentationTime > 0 && timestamp > _lastPresentationTime)
                        {
                            if (_presentationIntervals.Count == 4096)
                                _presentationIntervals.Dequeue();
                            _presentationIntervals.Enqueue(
                                (timestamp - _lastPresentationTime) * 1000
                            );
                        }
                        _lastPresentationTime = Math.Max(_lastPresentationTime, timestamp);
                    }
                });
            }
#if IOS || MACCATALYST
            transactionPresentation = present && PresentsWithTransaction;
#endif
            if (present && !transactionPresentation)
            {
                command.PresentDrawable(pending.Drawable);
            }

            command.AddCompletedHandler(completed =>
            {
                _frameLoop?.Record("gpu-arrived", pending.PulseId, pending.Generation, completion: pending.Completion);
                if (_frameLoop is not null && AppleMetalPresentation.CanReadGpuEndTime(completed) && completed.GpuEndTime > 0)
                    _frameLoop?.Record("gpu-ended", pending.PulseId, pending.Generation,
                        completion: pending.Completion, presentedTime: completed.GpuEndTime);
                var status = completed.Status;
                var error = completed.Error?.LocalizedDescription;
                if (_profileBlur)
                {
                    Interlocked.Increment(ref _commandBuffersCompleted);
                    if (status == MTLCommandBufferStatus.Error)
                        Interlocked.Increment(ref _commandBuffersErrored);
                }
#if IOS && !MACCATALYST
                // A display pulse can run before the posted main-thread callback.
                // Publish completion now, then drain on the owner thread before
                // changing drawable generation for that pulse.
                _completedFrames.Enqueue((pending, status, error));
                UIApplication.SharedApplication.BeginInvokeOnMainThread(DrainCompletedFrames);
#else
                // Dispatch via the application, since the native view may have
                // been disposed while its borrowed drawable remains in flight.
                UIApplication.SharedApplication.BeginInvokeOnMainThread(() =>
                    Retire(pending, status, error)
                );
#endif
            });
#if IOS || MACCATALYST
            if (present)
            {
                pending.PlatformFrame?.Commit();
            }
#endif
            var commitStart = Stopwatch.GetTimestamp();
            command.Commit();
            pending.Lifetime.MarkTerminalCommitted();
            _frameLoop?.Record("terminal-commit", pending.PulseId, pending.Generation, _pending.Count,
                pending.Completion, transactionPresentation ? "transaction" : "async",
                Stopwatch.GetElapsedTime(commitStart).TotalMilliseconds);
            if (_profileBlur)
                Interlocked.Increment(ref _commandBuffersCommitted);
            if (transactionPresentation)
            {
                // Apple's transaction presentation contract requires scheduling
                // first, then presenting the drawable directly in this layout
                // transaction. This waits for scheduling, not GPU completion;
                // resource retirement still belongs to the completion callback.
                var scheduledStart = Stopwatch.GetTimestamp();
                command.WaitUntilScheduled();
                _frameLoop?.Record("wait-scheduled", pending.PulseId, pending.Generation, _pending.Count,
                    pending.Completion, duration: Stopwatch.GetElapsedTime(scheduledStart).TotalMilliseconds);
                pending.Drawable.Present();
#if IOS || MACCATALYST
                pending.PlatformFrame?.Present();
#endif
            }
        }
        catch
        {
            _faulted = true;
            RetiringViews.Add(this);
#if IOS || MACCATALYST
            try
            {
                pending.PlatformFrame?.Abort();
            }
            catch (Exception rollbackError)
            {
                System.Diagnostics.Trace.TraceError(rollbackError.ToString());
            }
#endif
            if (present && !pending.Lifetime.TerminalCommitted)
            {
                try
                {
                    CommitTerminal(pending, present: false);
                }
                catch (Exception markerError)
                {
                    System.Diagnostics.Trace.TraceError(markerError.ToString());
                }
            }
            throw;
        }
    }

    internal MauiSurfaceSnapshot CaptureSnapshot(MauiSurfaceSnapshot current)
    {
        current = current with
        {
            NativeFramePipeline = new(NativeFrameConfiguration.Mode, _preparedFrameworkPulses,
                _pending.Count, _maximumGpuFrames, _completedGpuFrames,
                "Metal same-queue terminal completion; drawable presentation is a separate receipt",
                _frameFallback, _preparedWhileGpuFull, _rejectedAdmissions),
        };
        if (!_profileBlur)
            return current;
        lock (_presentationGate)
            return current with
            {
                IosFrameLoop = _frameLoop?.Snapshot(),
                IosFrameLoopState = $"drawing={_drawing}; suspended={_suspended}; faulted={_faulted}; release={_releaseRequested}; gpuPending={_pending.Count}; generation={_generation}; paused={Paused}; needsDisplay={EnableSetNeedsDisplay}; ownerActive={_ownerActiveSnapshot}"
#if IOS && !MACCATALYST
                    + $"; pulseRunning={_pipelineDisplayLink}; frameRequested={_owner?.FrameworkFrameRequested?.Invoke() == true}"
#endif
                    ,
                MetalDevice = Device?.Name,
                PixelFormat = ColorPixelFormat.ToString(),
                PresentedDrawables = _presentedDrawables,
                PresentationIntervalsMilliseconds = _presentationIntervals.ToArray(),
                // These are terminal markers only; Graphite's internal submissions are separate.
                CommandBuffersCommitted = Interlocked.Read(ref _commandBuffersCommitted),
                CommandBuffersCompleted = Interlocked.Read(ref _commandBuffersCompleted),
                CommandBuffersErrored = Interlocked.Read(ref _commandBuffersErrored),
                MetalAllocatedBytes = _resourcesReleased
                    ? null
                    : checked((long)(Device?.CurrentAllocatedSize ?? 0)),
            };
    }

    private void Retire(PendingFrame pending, MTLCommandBufferStatus status, string? error)
    {
        if (!_pending.Contains(pending))
        {
            return; // A recovery marker may follow an accepted terminal.
        }

        try
        {
            // An empty later marker reporting Error does not prove earlier
            // queue work completed. Hold unless the context confirms loss.
            if (!pending.Lifetime.CanRetire(status == MTLCommandBufferStatus.Completed, _session?.IsDeviceLost == true))
            {
                throw new InvalidOperationException(
                    $"Metal terminal failed; retaining GPU resources: {status}: {error}"
                );
            }

            _frameLoop?.Record("gpu-owner", pending.PulseId, pending.Generation, _pending.Count, pending.Completion);
            pending.Frame?.CompleteGpuWork();
#if IOS || MACCATALYST
            pending.PlatformFrame?.Dispose();
#endif
            pending.Lifetime.MarkRetired(status == MTLCommandBufferStatus.Completed, _session?.IsDeviceLost == true);
            pending.ReleaseDrawableIfSafe();
            _pending.Remove(pending);
            if (status == MTLCommandBufferStatus.Completed) _completedGpuFrames++;
            if (status != MTLCommandBufferStatus.Completed)
            {
                throw new InvalidOperationException(
                    $"Metal presentation failed: {status}: {error}"
                );
            }

            if (pending.Completion is { } completion)
            {
                pending.Owner.CompleteGraphite(
                    completion,
                    _releaseRequested
                        || pending.Generation != _generation
                        || !ReferenceEquals(_owner, pending.Owner)
                );
            }
        }
        catch (Exception exception)
        {
            _faulted = true;
            _session?.StopAcceptingFrames();
            RetiringViews.Add(this);
            pending.Owner.FailGraphite(pending.Completion, exception);
        }
        finally
        {
            if (_releaseRequested && _pending.Count == 0)
            {
                ReleaseGpuResources();
            }

            WakePendingAdmission();
        }
    }

    private void ReleaseGpuResources()
    {
        if (_resourcesReleased)
        {
            return;
        }

        _retirementTimer?.Invalidate();
        _retirementTimer?.Dispose();
        _retirementTimer = null;
        _resourceOwner?.ReleaseGraphiteResources();
        _resourceOwner = null;
        _session?.Dispose();
        _session = null;
        _queue.Dispose();
        _resourcesReleased = true;
        RetiringViews.Remove(this);
        _retired.TrySetResult();
    }

    private void DispatchTouches(NSSet touches, UIEvent? evt, PointerChange change)
    {
        if (_owner?.EnableTouchEvents != true)
        {
            return;
        }

        foreach (UITouch touch in touches.Cast<UITouch>())
        {
            var point = touch.LocationInView(this);
            var kind = touch.Type switch
            {
                UITouchType.Direct or UITouchType.Indirect => PointerDeviceKind.touch,
                UITouchType.Stylus => PointerDeviceKind.stylus,
                UITouchType.IndirectPointer => PointerDeviceKind.mouse,
                _ => PointerDeviceKind.touch,
            };
            var buttons =
                change is PointerChange.up or PointerChange.cancel ? 0
                : kind == PointerDeviceKind.mouse ? (int)(evt?.ButtonMask ?? 0)
                : 1;
            _owner.DispatchNativePointer(
                new(
                    TimeSpan.FromSeconds(touch.Timestamp),
                    change,
                    kind,
                    checked((ulong)((IntPtr)touch.Handle).ToInt64()),
                    point.X * ContentScaleFactor,
                    point.Y * ContentScaleFactor,
                    buttons,
                    0,
                    0,
                    PointerSignalKind.none,
                    touch.MaximumPossibleForce > 0 ? PenMeasurements.Pressure(touch.Force / touch.MaximumPossibleForce) : 1,
                    Orientation: kind == PointerDeviceKind.stylus ? PenMeasurements.Orientation(touch.GetAzimuthAngle(this)) : 0,
                    Tilt: kind == PointerDeviceKind.stylus ? PenMeasurements.FromAltitude(touch.AltitudeAngle) : 0,
                    PenSupport: kind == PointerDeviceKind.stylus ? new(
                        touch.MaximumPossibleForce > 0 ? PenFieldSupport.Supported : PenFieldSupport.Unknown,
                        PenFieldSupport.Supported, PenFieldSupport.Supported,
                        PenOrientationReference.ScreenAzimuth) : null
                )
            );
        }
    }

    public override void TouchesBegan(NSSet touches, UIEvent? evt)
    {
        base.TouchesBegan(touches, evt);
        DispatchTouches(touches, evt, PointerChange.down);
    }

    public override void TouchesMoved(NSSet touches, UIEvent? evt)
    {
        base.TouchesMoved(touches, evt);
        DispatchTouches(touches, evt, PointerChange.move);
    }

    public override void TouchesEnded(NSSet touches, UIEvent? evt)
    {
        base.TouchesEnded(touches, evt);
        DispatchTouches(touches, evt, PointerChange.up);
    }

    public override void TouchesCancelled(NSSet touches, UIEvent? evt)
    {
        base.TouchesCancelled(touches, evt);
        DispatchTouches(touches, evt, PointerChange.cancel);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Disconnect();
            Delegate = null;
        }
        base.Dispose(disposing);
    }
}
#endif
