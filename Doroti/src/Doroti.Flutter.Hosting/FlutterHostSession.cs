using Doroti.Flutter.Ui;

namespace Doroti.Flutter.Hosting;

public interface IFlutterFrameworkEntrypoint
{
    void Bootstrap(PlatformDispatcher dispatcher);

    void Shutdown();
}

/// <summary>Per-view framework attachment implemented by generated/reviewed Widgets entrypoints.</summary>
public interface IFlutterViewEntrypoint : IFlutterFrameworkEntrypoint
{
    void AttachView(FlutterView view);

    void DetachView(FlutterView view);
}

public enum FlutterHostSessionState
{
    created,
    running,
    shutDown,
}

/// <summary>Host-neutral, exactly-once framework bootstrap and multi-view shutdown protocol.</summary>
public sealed class FlutterHostSession : IDisposable
{
    private readonly IFlutterFrameworkEntrypoint _entrypoint;
    private readonly Dictionary<ulong, FlutterView> _views = [];
    private bool _bootstrapped;
    private bool _deferFrameworkBootstrap;

    public FlutterHostSession(
        IFlutterFrameworkEntrypoint entrypoint,
        IDartPerformanceModeCapability? performanceModeCapability = null)
    {
        _entrypoint = entrypoint ?? throw new ArgumentNullException(nameof(entrypoint));
        dispatcher = new(performanceModeCapability);
    }

    public PlatformDispatcher dispatcher { get; }

    public FlutterHostSessionState state { get; private set; }

    public void Start(bool deferFrameworkBootstrap = false)
    {
        if (state != FlutterHostSessionState.created)
        {
            throw new InvalidOperationException($"Flutter host session cannot start from {state}.");
        }
        _deferFrameworkBootstrap = deferFrameworkBootstrap;
        state = FlutterHostSessionState.running;
        if (deferFrameworkBootstrap)
        {
            return;
        }
        try
        {
            _entrypoint.Bootstrap(dispatcher);
            _bootstrapped = true;
        }
        catch
        {
            state = FlutterHostSessionState.shutDown;
            dispatcher.Dispose();
            throw;
        }
    }

    public void AttachView(FlutterView view)
    {
        ArgumentNullException.ThrowIfNull(view);
        if (state != FlutterHostSessionState.running)
        {
            throw new InvalidOperationException("The Flutter host session must be running before a view is attached.");
        }
        if (_entrypoint is not IFlutterViewEntrypoint viewEntrypoint)
        {
            throw new FlutterCapabilityException(
                FlutterCapabilityIds.FrameworkViewAttachment,
                view.viewId,
                DartUiInvocation.Managed("package:flutter/widgets.dart#runApp"),
                $"entrypoint {_entrypoint.GetType().FullName} does not implement {nameof(IFlutterViewEntrypoint)}",
                view.targetIdentity);
        }
        if (_deferFrameworkBootstrap && !_bootstrapped)
        {
            try
            {
                _entrypoint.Bootstrap(dispatcher);
                _bootstrapped = true;
            }
            catch
            {
                state = FlutterHostSessionState.shutDown;
                dispatcher.Dispose();
                throw;
            }
        }
        if (!_views.TryAdd(view.viewId, view))
        {
            throw new InvalidOperationException($"Flutter view {view.viewId} is already attached to this session.");
        }
        try
        {
            viewEntrypoint.AttachView(view);
        }
        catch
        {
            _views.Remove(view.viewId);
            throw;
        }
    }

    public void DetachView(FlutterView view)
    {
        ArgumentNullException.ThrowIfNull(view);
        if (_views.Remove(view.viewId) && _entrypoint is IFlutterViewEntrypoint viewEntrypoint)
        {
            viewEntrypoint.DetachView(view);
        }
    }

    public void Shutdown()
    {
        if (state == FlutterHostSessionState.shutDown)
        {
            return;
        }
        var wasRunning = state == FlutterHostSessionState.running && _bootstrapped;
        state = FlutterHostSessionState.shutDown;
        try
        {
            if (wasRunning)
            {
                if (_entrypoint is IFlutterViewEntrypoint viewEntrypoint)
                {
                    foreach (var view in _views.Values.Reverse().ToArray())
                    {
                        viewEntrypoint.DetachView(view);
                    }
                }
                _views.Clear();
                _entrypoint.Shutdown();
            }
        }
        finally
        {
            dispatcher.Dispose();
        }
    }

    public void Dispose() => Shutdown();
}
