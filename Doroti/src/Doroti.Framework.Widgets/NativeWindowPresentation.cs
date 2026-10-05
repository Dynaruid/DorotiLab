using System.Runtime.CompilerServices;
using Doroti.Runtime;
using Doroti.Ui;
namespace Doroti.Framework.Widgets;
public enum WindowPresentation { Auto, Native, Overlay }
/// <summary>Routes native child content through the application's existing view collection.</summary>
public static class NativeWindowPresentation
{
    internal sealed class Entry(Widget child)
    {
        internal Widget Child = child;
        internal TaskCompletionSource<(DorotiView View, long Frame)> Mounted = new(TaskCreationOptions.RunContinuationsAsynchronously);
        internal TaskCompletionSource Closed = new(TaskCreationOptions.RunContinuationsAsynchronously);
        internal GlobalKey<NavigatorState>? NavigatorKey;
        internal ulong CallerViewId;
        internal DorotiSceneOwner? NativeOwner;
    }
    internal sealed class Registry : ChangeNotifier
    {
        internal readonly Dictionary<WindowId, Entry> Entries = [];
        internal Widget? Child(DorotiView view)
        {
            var windows = view.GetCapabilityOrDefault<IWindowService>(DorotiCapabilityIds.WindowService);
            var window = windows?.GetWindows().SingleOrDefault(value => value.ViewId == view.viewId && !value.Closed);
            if (window is null || !Entries.TryGetValue(window.Id, out var entry)) return null;
            entry.NativeOwner = view.SceneOwner;
            return new PresentedChild(entry, view);
        }
        public override void dispose()
        {
            foreach (var entry in Entries.Values) { entry.Mounted.TrySetCanceled(); entry.Closed.TrySetCanceled(); }
            Entries.Clear(); base.dispose();
        }
    }
    private static readonly ConditionalWeakTable<PlatformDispatcher, Registry> Registries = new();
    internal static Registry For(PlatformDispatcher dispatcher) => Registries.GetValue(dispatcher, _ => new());
    internal static NavigatorState? NavigatorForCaller(BuildContext context)
    {
        var dispatcher = WidgetsBinding.instance.platformDispatcher;
        if (dispatcher.CurrentInvocationView is not { } current || current.InvocationLifetime.IsCancellationRequested ||
            !Registries.TryGetValue(dispatcher, out var registry)) return null;
        var entry = registry.Entries.Values.FirstOrDefault(entry => entry.NativeOwner == current.SceneOwner && !entry.Closed.Task.IsCompleted);
        if (entry is null) return null;
        var caller = View.maybeOf(context);
        if (caller is null || caller.viewId == current.viewId) return null;
        return entry.CallerViewId == caller.viewId ? entry.NavigatorKey?.currentState : null;
    }
    internal static void Shutdown(PlatformDispatcher dispatcher)
    { if (Registries.TryGetValue(dispatcher, out var registry)) { Registries.Remove(dispatcher); registry.dispose(); } }
    private sealed class PresentedChild(Entry entry, DorotiView view) : StatelessWidget
    {
        public override Widget build(BuildContext context)
        {
            var frame = view.platformDispatcher.CurrentFrameNumber;
            if (!entry.Mounted.Task.IsCompleted)
                WidgetsBinding.instance.addPostFrameCallback(_ => entry.Mounted.TrySetResult((view, frame)));
            return entry.Child;
        }
    }
    public static WindowCapabilityResult EvaluateContent(BuildContext context, WindowRequest request)
    {
        var view = View.of(context); var owner = WindowScope.maybeOf(context);
        var windows = view.GetCapabilityOrDefault<IWindowService>(DorotiCapabilityIds.WindowService);
        if (windows is null) return new(WindowAvailability.Unsupported, "This view has no native window service.");
        if (owner is null || owner.Closed || owner.Closing || owner.ViewId != view.viewId || request.Owner != owner.Id)
            throw new InvalidOperationException("Native content requires this context's live window owner.");
        if (view.GetCapabilityOrDefault<IFramePresentationHostCapability>(DorotiCapabilityIds.FramePresentation) is null)
            return new(WindowAvailability.Unsupported, "This provider does not acknowledge view presentation terminals.");
        return windows.Evaluate(request);
    }
    public sealed class ContentHandle : IAsyncDisposable
    {
        private readonly IWindowService _windows;
        private readonly Task _retirement;
        private readonly object _gate = new();
        private Task? _dispose;
        internal ContentHandle(IWindowService windows, WindowSnapshot window, Task retirement)
        { _windows = windows; Window = window; _retirement = retirement; }
        public WindowSnapshot Window { get; }
        public ValueTask DisposeAsync()
        {
            lock (_gate) return new(_dispose ??= CloseAsync());
        }
        private async Task CloseAsync()
        {
            await _windows.CloseAsync(Window.Id, CancellationToken.None);
            await _retirement;
        }
    }
    public static async ValueTask<ContentHandle> ShowContentAsync(BuildContext context, WindowRequest request, Widget child,
        CancellationToken cancellationToken = default)
    {
        EvaluateContent(context, request).RequireSupported();
        var owner = View.of(context); var windows = owner.GetCapabilityOrDefault<IWindowService>(DorotiCapabilityIds.WindowService)!;
        var registry = For(owner.platformDispatcher);
        var captured = InheritedTheme.capture(context, null).wrap(Localizations.CreateOverride(context: context,
            child: new Directionality(textDirection: Directionality.of(context), child: child)));
        var entry = new Entry(captured);
        var lifetime = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, owner.InvocationLifetime);
        WindowSnapshot? created = null;
        void Changed(WindowEvent value)
        {
            if (created is not null && value.Window.Id == created.Id && (value.Window.Closed || value.Window.Closing))
            {
                entry.Closed.TrySetResult();
                try { lifetime.Cancel(); } catch (ObjectDisposedException) { }
            }
        }
        windows.Changed += Changed;
        try
        {
            created = await owner.InvokeCapabilityAsync<IWindowService, WindowSnapshot>(DorotiCapabilityIds.WindowService,
                DorotiUiInvocation.Managed("Widgets.Content.create"), async (service, ct) =>
                {
                    // Capture the owned result before invocation rejects a late canceled result.
                    created = await service.CreateAsync(request, ct);
                    return created;
                }, lifetime.Token);
            if (!windows.GetWindows().Any(window => window.Id == created.Id && !window.Closed && !window.Closing))
                throw new OperationCanceledException("Native content closed during creation.");
            await owner.DispatchPlatformEventAsync(() => { registry.Entries.Add(created.Id, entry); registry.notifyListeners(); }, lifetime.Token);
            var mounted = await entry.Mounted.Task.WaitAsync(lifetime.Token);
            await mounted.View.InvokeCapabilityAsync<IFramePresentationHostCapability, bool>(DorotiCapabilityIds.FramePresentation,
                DorotiUiInvocation.Managed("Widgets.Content.first-present"), async (host, ct) => { await host.WaitForPresentationAsync(mounted.Frame, ct); return true; }, lifetime.Token);
            if (entry.Closed.Task.IsCompleted) throw new OperationCanceledException("Native content closed before presentation.", lifetime.Token);
            await windows.ExecuteAsync(new(created.Id, WindowAction.Show), lifetime.Token);
            var retirement = RetireAsync();
            DartRuntimePrimitives.ObserveTask(retirement, "native content retirement");
            return new ContentHandle(windows, created, retirement);
            async Task RetireAsync()
            {
                try
                {
                    await entry.Closed.Task.WaitAsync(lifetime.Token);
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                finally
                {
                    windows.Changed -= Changed;
                    try { await windows.CloseAsync(created.Id, CancellationToken.None); }
                    finally
                    {
                        try { await RemoveContentAsync(owner.platformDispatcher, registry, created.Id); }
                        finally { lifetime.Dispose(); }
                    }
                }
            }
        }
        catch
        {
            windows.Changed -= Changed;
            try
            {
                if (created is not null)
                {
                    try { await windows.CloseAsync(created.Id, CancellationToken.None); }
                    finally { await RemoveContentAsync(owner.platformDispatcher, registry, created.Id); }
                }
            }
            finally { lifetime.Dispose(); }
            throw;
        }
    }

    private static async Task RemoveContentAsync(PlatformDispatcher dispatcher, Registry registry, WindowId id)
    {
        try
        {
            await dispatcher.DispatchApplicationEventAsync(() =>
            {
                if (registry.Entries.Remove(id)) registry.notifyListeners();
            });
        }
        catch (OperationCanceledException) { }
        catch (ObjectDisposedException) { }
    }
    public static Future<T?> ShowDialog<T>(BuildContext context, Route<T> route, WindowPresentation presentation,
        bool barrierDismissible = false, bool useRootNavigator = true, Size? nativeSize = null)
    {
        if (!Enum.IsDefined(presentation)) throw new ArgumentException("Unknown presentation policy.");
        if (presentation == WindowPresentation.Overlay) return Navigator.of(context, rootNavigator: useRootNavigator).push(route);
        var view = View.of(context); var owner = WindowScope.maybeOf(context);
        var windows = view.GetCapabilityOrDefault<IWindowService>(DorotiCapabilityIds.WindowService);
        WindowCapabilityResult evaluation;
        WindowRequest? request = null;
        if (windows is null) evaluation = new(WindowAvailability.Unsupported, "This view does not support native windows.");
        else
        {
            if (owner is null || owner.Closed || owner.ViewId != view.viewId) throw new InvalidOperationException("Native dialog requires this context's live window owner.");
            // Native modal ownership cannot reproduce a barrier covering other OS windows.
            if (barrierDismissible) evaluation = new(WindowAvailability.Unsupported, "Cross-window dismissible barriers require Overlay presentation.");
            else if (view.GetCapabilityOrDefault<IFramePresentationHostCapability>(DorotiCapabilityIds.FramePresentation) is null)
                evaluation = new(WindowAvailability.Unsupported, "This provider does not acknowledge view presentation terminals.");
            else
            {
                request = new(WindowKind.Dialog, "", nativeSize ?? new Size(Math.Min(480, owner.Size.width), Math.Min(360, owner.Size.height)), owner.Id, Modal: true);
                evaluation = windows.Evaluate(request);
            }
        }
        if (evaluation.Availability == WindowAvailability.Unsupported && presentation == WindowPresentation.Auto)
            return Navigator.of(context, rootNavigator: useRootNavigator).push(route);
        evaluation.RequireSupported();
        var themes = InheritedTheme.capture(context, null);
        var navigatorKey = GlobalKey<NavigatorState>.Create();
        Widget child = new Navigator(key: navigatorKey, onGenerateInitialRoutes: (_, _) => [
            new RawDialogRoute<object>(pageBuilder: (_, _, _) => SizedBox.CreateExpand(), barrierDismissible: false), route]);
        child = themes.wrap(Localizations.CreateOverride(context: context,
            child: new Directionality(textDirection: Directionality.of(context), child: child)));
        return Future<T?>.fromTask(RunAsync(view, windows!, request!, child, route, navigatorKey: navigatorKey));
    }
    /// <summary>Presents a preview/action route in an owned popup using the caller view's logical viewport.</summary>
    public static Future<T?> ShowPopupRoute<T>(BuildContext context, Route<T> route, WindowPresentation presentation,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.IsDefined(presentation)) throw new ArgumentException("Unknown presentation policy.");
        if (presentation == WindowPresentation.Overlay) return Navigator.of(context, rootNavigator: true).push(route);
        var view = View.of(context);
        var owner = WindowScope.maybeOf(context);
        var windows = view.GetCapabilityOrDefault<IWindowService>(DorotiCapabilityIds.WindowService);
        WindowCapabilityResult evaluation;
        WindowRequest? request = null;
        if (windows is null) evaluation = new(WindowAvailability.Unsupported, "This view has no native window service.");
        else
        {
            if (owner is null || owner.Closed || owner.Closing || owner.ViewId != view.viewId)
                throw new InvalidOperationException("Native popup requires this context's live window owner.");
            // Evaluate ownership before an optional-feature rejection can select Overlay.
            var size = owner.Size;
            request = new(WindowKind.Popup, "", size, owner.Id,
                new(owner.Id, new Rect(0, -size.height, size.width, 0)));
            evaluation = windows.Evaluate(request);
            if (evaluation.Availability == WindowAvailability.Supported)
            {
                if (route is ModalRoute<T> { filter: not null })
                    evaluation = new(WindowAvailability.Unsupported, "Native popup Windowing does not sample the caller view's backdrop; filtered previews require Overlay.");
                else if (view.GetCapabilityOrDefault<IFramePresentationHostCapability>(DorotiCapabilityIds.FramePresentation) is null)
                    evaluation = new(WindowAvailability.Unsupported, "This provider does not acknowledge view presentation terminals.");
            }
        }
        if (evaluation.Availability == WindowAvailability.Unsupported && presentation == WindowPresentation.Auto)
            return Navigator.of(context, rootNavigator: true).push(route);
        evaluation.RequireSupported();
        var navigatorKey = GlobalKey<NavigatorState>.Create();
        Widget child = new Navigator(key: navigatorKey, onGenerateInitialRoutes: (_, _) => [
            new RawDialogRoute<object>(pageBuilder: (_, _, _) => SizedBox.CreateExpand(), barrierDismissible: false), route]);
        child = InheritedTheme.capture(context, null).wrap(Localizations.CreateOverride(context: context,
            child: new Directionality(textDirection: Directionality.of(context), child: child)));
        return Future<T?>.fromTask(RunAsync(view, windows!, request!, child, route, cancellationToken, navigatorKey));
    }

    private static async Task<T?> RunAsync<T>(DorotiView owner, IWindowService windows, WindowRequest request, Widget child, Route<T> route,
        CancellationToken cancellationToken = default, GlobalKey<NavigatorState>? navigatorKey = null)
    {
        using var lifetime = CancellationTokenSource.CreateLinkedTokenSource(owner.InvocationLifetime, cancellationToken);
        var entry = new Entry(child) { NavigatorKey = navigatorKey, CallerViewId = owner.viewId }; var registry = For(owner.platformDispatcher);
        WindowSnapshot? created = null;
        void Changed(WindowEvent value)
        { if (created is not null && value.Window.Id == created.Id && (value.Window.Closed || value.Window.Closing)) { entry.Closed.TrySetResult(); try { lifetime.Cancel(); } catch (ObjectDisposedException) { } } }
        windows.Changed += Changed;
        try
        {
            created = await owner.InvokeCapabilityAsync<IWindowService, WindowSnapshot>(DorotiCapabilityIds.WindowService,
                DorotiUiInvocation.Managed("Widgets.Dialog.create"), async (service, ct) =>
                {
                    created = await service.CreateAsync(request, ct);
                    return created;
                }, lifetime.Token);
            await owner.DispatchPlatformEventAsync(() => { registry.Entries.Add(created.Id, entry); registry.notifyListeners(); }, lifetime.Token);
            var mounted = await entry.Mounted.Task.WaitAsync(lifetime.Token);
            if (mounted.View.GetCapabilityOrDefault<IFramePresentationHostCapability>(DorotiCapabilityIds.FramePresentation) is null)
                throw new NotSupportedException("The child provider does not acknowledge presentation.");
            await mounted.View.InvokeCapabilityAsync<IFramePresentationHostCapability, bool>(DorotiCapabilityIds.FramePresentation,
                DorotiUiInvocation.Managed("Widgets.Dialog.first-present"), async (host, ct) => { await host.WaitForPresentationAsync(mounted.Frame, ct); return true; }, lifetime.Token);
            await windows.ExecuteAsync(new(created.Id, WindowAction.Show), lifetime.Token);
            var popped = route.popped.asTask();
            var completed = await Task.WhenAny(popped, entry.Closed.Task).WaitAsync(lifetime.Token);
            return ReferenceEquals(completed, popped) ? await popped : default;
        }
        catch (OperationCanceledException) when (entry.Closed.Task.IsCompletedSuccessfully) { return default; }
        catch (ObjectDisposedException) when (entry.Closed.Task.IsCompletedSuccessfully) { return default; }
        finally
        {
            windows.Changed -= Changed;
            if (created is not null)
            {
                await windows.CloseAsync(created.Id, CancellationToken.None);
                await RemoveContentAsync(owner.platformDispatcher, registry, created.Id);
            }
        }
    }
}
