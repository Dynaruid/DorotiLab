using Doroti.Framework.Widgets;
using Doroti.Hosting;
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework;

/// <summary>One framework binding and logical root with independently owned View branches.</summary>
public sealed class DorotiWidgetEntrypoint : IDorotiViewEntrypoint
{
    private readonly Func<DorotiApplicationViews, Widget> _rootFactory;
    private readonly Func<Task>? _initialize;
    private readonly DorotiApplicationViews _applicationViews = new();
    private WidgetsFlutterBinding? _binding;
    private bool _rootScheduled;
    private long _generation;

    public DorotiWidgetEntrypoint(Func<Widget> rootFactory)
    {
        ArgumentNullException.ThrowIfNull(rootFactory);
        _rootFactory = views => new DorotiApplicationViewCollection(views, (_, _) => rootFactory());
    }

    /// <summary>Creates a shared application root. State above the collection survives primary close.</summary>
    public DorotiWidgetEntrypoint(Func<DorotiApplicationViews, Widget> applicationRootFactory) =>
        _rootFactory = applicationRootFactory ?? throw new ArgumentNullException(nameof(applicationRootFactory));

    public DorotiWidgetEntrypoint(Func<Widget> rootFactory, Func<Task> initialize) : this(rootFactory) =>
        _initialize = initialize ?? throw new ArgumentNullException(nameof(initialize));

    public DorotiWidgetEntrypoint(Func<DorotiApplicationViews, Widget> rootFactory, Func<Task> initialize) : this(rootFactory) =>
        _initialize = initialize ?? throw new ArgumentNullException(nameof(initialize));

    public void Bootstrap(PlatformDispatcher dispatcher)
    {
        if (_binding is not null) throw new InvalidOperationException("The application binding is already bootstrapped.");
        _binding = new WidgetsFlutterBinding(dispatcher);
        _generation++;
    }

    public void AttachView(DorotiView view)
    {
        ArgumentNullException.ThrowIfNull(view);
        var binding = _binding ?? throw new InvalidOperationException("The widget application is not bootstrapped.");
        _applicationViews.Attach(view);
        if (_rootScheduled)
        {
            binding.scheduleForcedFrame();
            return;
        }
        _rootScheduled = true;
        var generation = _generation;
        bool IsAlive() => ReferenceEquals(_binding, binding) && generation == _generation && _applicationViews.Views.Count != 0;
        void AttachRoot()
        {
            if (IsAlive()) binding.attachRootWidget(_rootFactory(_applicationViews));
        }
        async Task InitializeAsync()
        {
            try
            {
                await _initialize!();
                DartAsyncRuntime.scheduleMicrotask(() =>
                {
                    if (!IsAlive()) return;
                    binding.scheduleFrameCallback(_ => AttachRoot(), scheduleNewFrame: false);
                    binding.scheduleForcedFrame();
                });
            }
            catch (Exception error)
            {
                DartAsyncRuntime.scheduleMicrotask(() =>
                {
                    if (IsAlive()) FlutterError.reportError(new FlutterErrorDetails(error, library: "Doroti application bootstrap"));
                });
            }
        }
        binding.scheduleFrameCallback(timestamp =>
        {
            if (!IsAlive()) return;
            if (_initialize is null) AttachRoot();
            else _ = InitializeAsync();
        }, scheduleNewFrame: false);
        binding.scheduleForcedFrame();
    }

    public void DetachView(DorotiView view)
    {
        ArgumentNullException.ThrowIfNull(view);
        _applicationViews.Detach(view);
        // Unmount the removed branch while its capabilities still exist, before native drain/disposal.
        if (_binding?.rootElement is { } root)
        {
            _binding.buildOwner!.buildScope(root);
            _binding.buildOwner.finalizeTree();
        }
        _binding?.ReleaseViewImageCache(view);
    }

    public void Shutdown()
    {
        _generation++;
        if (_binding is { } binding) NativeWindowPresentation.Shutdown(binding.platformDispatcher);
        _binding?.Dispose();
        _binding = null;
        _applicationViews.Dispose();
    }
}
