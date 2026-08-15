using Doroti.Ui;

namespace Doroti.Hosting;

public interface IDorotiFrameworkEntrypoint
{
    void Bootstrap(PlatformDispatcher dispatcher);

    void Shutdown();
}

/// <summary>Per-view framework attachment implemented by generated/reviewed Widgets entrypoints.</summary>
public interface IDorotiViewEntrypoint : IDorotiFrameworkEntrypoint
{
    void AttachView(DorotiView view);

    void DetachView(DorotiView view);
}

public enum DorotiHostSessionState
{
    created,
    running,
    shutDown,
}

/// <summary>Host-neutral, exactly-once framework bootstrap and multi-view shutdown protocol.</summary>
public sealed class DorotiHostSession : IDisposable
{
    private readonly IDorotiFrameworkEntrypoint _entrypoint;
    private readonly Dictionary<ulong, DorotiView> _views = [];
    private bool _bootstrapped;
    private bool _deferFrameworkBootstrap;

    public DorotiHostSession(
        IDorotiFrameworkEntrypoint entrypoint,
        IDartPerformanceModeCapability? performanceModeCapability = null)
    {
        _entrypoint = entrypoint ?? throw new ArgumentNullException(nameof(entrypoint));
        dispatcher = new(performanceModeCapability);
    }

    public PlatformDispatcher dispatcher { get; }

    public DorotiHostSessionState state { get; private set; }

    public void Start(bool deferFrameworkBootstrap = false)
    {
        if (state != DorotiHostSessionState.created)
        {
            throw new InvalidOperationException($"Flutter host session cannot start from {state}.");
        }
        _deferFrameworkBootstrap = deferFrameworkBootstrap;
        state = DorotiHostSessionState.running;
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
            state = DorotiHostSessionState.shutDown;
            dispatcher.Dispose();
            throw;
        }
    }

    public void AttachView(DorotiView view)
    {
        ArgumentNullException.ThrowIfNull(view);
        if (state != DorotiHostSessionState.running)
        {
            throw new InvalidOperationException("The Flutter host session must be running before a view is attached.");
        }
        if (_entrypoint is not IDorotiViewEntrypoint viewEntrypoint)
        {
            throw new DorotiCapabilityException(
                DorotiCapabilityIds.FrameworkViewAttachment,
                view.viewId,
                DartUiInvocation.Managed("package:flutter/widgets.dart#runApp"),
                $"entrypoint {_entrypoint.GetType().FullName} does not implement {nameof(IDorotiViewEntrypoint)}",
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
                state = DorotiHostSessionState.shutDown;
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

    public void DetachView(DorotiView view)
    {
        ArgumentNullException.ThrowIfNull(view);
        if (_views.Remove(view.viewId) && _entrypoint is IDorotiViewEntrypoint viewEntrypoint)
        {
            viewEntrypoint.DetachView(view);
        }
    }

    public void Shutdown()
    {
        if (state == DorotiHostSessionState.shutDown)
        {
            return;
        }
        var wasRunning = state == DorotiHostSessionState.running && _bootstrapped;
        state = DorotiHostSessionState.shutDown;
        try
        {
            if (wasRunning)
            {
                if (_entrypoint is IDorotiViewEntrypoint viewEntrypoint)
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
