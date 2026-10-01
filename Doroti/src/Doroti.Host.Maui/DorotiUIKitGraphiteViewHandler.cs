#if IOS || MACCATALYST
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
            handler.PlatformView.SetNeedsDisplay(),
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
#endif

    public DorotiUIKitGraphiteView()
        : base(
            CGRect.Empty,
            MTLDevice.SystemDefault
                ?? throw new PlatformNotSupportedException(
                    "Doroti Graphite requires a Metal-capable device."
                )
        )
    {
        _queue =
            Device!.CreateCommandQueue()
            ?? throw new InvalidOperationException("Metal queue creation failed.");
        ColorPixelFormat = MTLPixelFormat.BGRA8Unorm;
        FramebufferOnly = false;
        AutoResizeDrawable = false;
#if IOS && !MACCATALYST
        _animatedViewport = new(this, RenderViewport, CanRenderViewport);
        _animatedViewport.AnimationChanged += active => EnableSetNeedsDisplay = !active;
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
        SetNeedsDisplay();
    }

#if IOS || MACCATALYST
    internal UIKitPlatformRasterSurface CreatePlatformRasterSurface() => new(Device!, _queue);
#endif

#if MACCATALYST
    private readonly TaskCompletionSource _retired = new(
        TaskCreationOptions.RunContinuationsAsynchronously
    );

    internal Task RetireAsync()
    {
        Disconnect();
        return _retired.Task;
    }
#endif

    internal void Disconnect()
    {
        if (_releaseRequested)
        {
            return;
        }

        _releaseRequested = true;
#if IOS && !MACCATALYST
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
        Window?.WindowScene is { } scene
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
#if IOS && !MACCATALYST
        _animatedViewport.Stop();
#endif
        if (!_suspended)
        {
            _generation++;
        }

        _suspended = true;
    }

    private void ResumeRendering()
    {
        _suspended = !OwnerIsActive;
        if (!_suspended && !_releaseRequested && !_faulted)
        {
#if IOS && !MACCATALYST
            _animatedViewport.LayoutChanged();
#endif
            SetNeedsDisplay();
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
            SetNeedsDisplay();
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
                _animatedViewport.IsAnimating && _owner?.PlatformViews?.HasComposition != true
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
        SetNeedsDisplay();
    }

#if IOS && !MACCATALYST
    private bool CanRenderViewport()
    {
        DrainCompletedFrames();
        return !_drawing && (_pending.Count == 0 || _faulted || _releaseRequested);
    }

    private void DrainCompletedFrames()
    {
        if (_drawing)
            return;
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
#if IOS && !MACCATALYST
        // One display link owns rotation frames. A queued SetNeedsDisplay must
        // not add an old-size replay between geometry updates.
        if (_animatedViewport.IsAnimating && !_renderingViewport)
            return;
        DrainCompletedFrames();
#endif
        var owner = _owner;
        if (_drawing || _releaseRequested || _faulted || Window is null || owner is null)
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
        // The renderer promotes a scene to its replay source at GPU completion. A
        // second native draw before then could replay the older scene and
        // overwrite a newly submitted update (including selection handles).
        // UIKit's system loupe can request those redraws without a PlatformView.
        // Defer invalidations, without blocking UIKit, until that promotion occurs.
        const int maximumPending = 1;
        if (_pending.Count >= maximumPending)
        {
            _frameBackpressure = true;
            return;
        }
        _drawing = true;
        SkiaGraphiteSession.Frame? frame = null;
        ICAMetalDrawable? drawable = null;
        MauiSkiaPaintContext? paint = null;
        var submitted = false;
#if IOS || MACCATALYST
        UIKitPlatformViewHost.PreparedFrame? platformFrame = null;
        var compositionTransaction = owner.PlatformViews?.IsConfigured == true;
        var previousPresentation = PresentsWithTransaction;
        if (compositionTransaction)
        {
            CATransaction.Begin();
            CATransaction.DisableActions = true;
            PresentsWithTransaction = true;
        }
#endif
        try
        {
            drawable = CurrentDrawable;
            if (drawable is null)
            {
                SetNeedsDisplay();
                return;
            }
            _session ??= SkiaGraphiteSession.CreateMetal(
                Device!.Handle,
                _queue.Handle,
                Math.Max(1, ++_generation)
            );
            _session.NativeTextureImporter ??= new AppleNativeTextureImporter(
                _session,
                Device!.Handle
            );
            _session.GpuEffects ??= new AppleGpuEffects(Device!, _queue, _session);
            var width = checked((int)drawable.Texture.Width);
            var height = checked((int)drawable.Texture.Height);
            frame = _session.BeginMetalFrame(width, height, drawable.Texture.Handle);
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
            owner.PaintGraphite(paint);
#if IOS || MACCATALYST
            platformFrame = owner.PlatformViews?.TakePending();
#endif
            if (paint.SkipPresent || _releaseRequested || generation != _generation)
            {
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
                generation
#if IOS || MACCATALYST
                ,
                platformFrame
#endif
            );
#if IOS || MACCATALYST
            platformFrame = null;
#endif
            _pending.Add(pending);
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
            if (submitted || platformFrame?.HasSubmitted == true)
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
                    _generation
#if IOS || MACCATALYST
                    ,
                    platformFrame
#endif
                );
#if IOS || MACCATALYST
                platformFrame = null;
#endif
                _pending.Add(pending);
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
                PresentsWithTransaction = previousPresentation;
            }
#endif
            _drawing = false;
            if (_releaseRequested && _pending.Count == 0)
            {
                ReleaseGpuResources();
            }
        }
    }

    private sealed record PendingFrame(
        SkiaGraphiteSession.Frame? Frame,
        ICAMetalDrawable Drawable,
        DorotiGraphiteView Owner,
        MauiPaintCompletion? Completion,
        long Generation
#if IOS || MACCATALYST
        ,
        UIKitPlatformViewHost.PreparedFrame? PlatformFrame
#endif
    );

    private void CommitTerminal(PendingFrame pending, bool present)
    {
        try
        {
            using var command =
                _queue.CommandBuffer()
                ?? throw new InvalidOperationException(
                    "Metal terminal buffer creation failed; retaining GPU resources."
                );
            var transactionPresentation = false;
            if (present && _profileBlur)
            {
                pending.Drawable.AddPresentedHandler(drawable =>
                {
                    var timestamp = drawable.PresentedTime;
                    if (timestamp <= 0)
                        return;
                    lock (_presentationGate)
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
            command.Commit();
            if (_profileBlur)
                Interlocked.Increment(ref _commandBuffersCommitted);
            if (transactionPresentation)
            {
                // Apple's transaction presentation contract requires scheduling
                // first, then presenting the drawable directly in this layout
                // transaction. This waits for scheduling, not GPU completion;
                // resource retirement still belongs to the completion callback.
                command.WaitUntilScheduled();
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
            if (present)
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
        if (!_profileBlur)
            return current;
        lock (_presentationGate)
            return current with
            {
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
            if (status != MTLCommandBufferStatus.Completed && _session?.IsDeviceLost != true)
            {
                throw new InvalidOperationException(
                    $"Metal terminal failed; retaining GPU resources: {status}: {error}"
                );
            }

            pending.Frame?.CompleteGpuWork();
#if IOS || MACCATALYST
            pending.PlatformFrame?.Dispose();
#endif
            pending.Drawable.Dispose();
            _pending.Remove(pending);
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

            if (_frameBackpressure && !_releaseRequested && !_faulted)
            {
                _frameBackpressure = false;
                SetNeedsDisplay();
            }
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
#if MACCATALYST
        _retired.TrySetResult();
#endif
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
                    touch.MaximumPossibleForce > 0 ? touch.Force / touch.MaximumPossibleForce : 1
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
