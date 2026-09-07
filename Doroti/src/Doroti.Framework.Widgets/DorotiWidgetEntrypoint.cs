using Doroti.Framework.Widgets;
using Doroti.Framework.Foundation;
using Doroti.Hosting;
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework;

/// <summary>
/// Hosts a widget root behind the stable Doroti application-entrypoint contract.
/// </summary>
public sealed class DorotiWidgetEntrypoint : IDorotiViewEntrypoint
{
    private readonly Func<Widget> _rootFactory;
    private readonly Func<Task>? _initialize;
    private WidgetsFlutterBinding? _binding;
    private DorotiView? _view;
    private long _attachmentGeneration;

    public DorotiWidgetEntrypoint(Func<Widget> rootFactory) =>
        _rootFactory = rootFactory ?? throw new ArgumentNullException(nameof(rootFactory));

    /// <summary>Prepares resources in the attached view before creating its root widget.</summary>
    public DorotiWidgetEntrypoint(Func<Widget> rootFactory, Func<Task> initialize) : this(rootFactory) =>
        _initialize = initialize ?? throw new ArgumentNullException(nameof(initialize));

    public void Bootstrap(PlatformDispatcher dispatcher) => _binding = new WidgetsFlutterBinding(dispatcher);

    public void AttachView(DorotiView view)
    {
        ArgumentNullException.ThrowIfNull(view);
        if (_binding is null)
            throw new InvalidOperationException("The Doroti widget runtime is not bootstrapped.");
        if (_view is not null)
            throw new InvalidOperationException("This Doroti widget entrypoint already owns a view.");

        _view = view;
        var binding = _binding;
        var generation = ++_attachmentGeneration;
        bool IsAttached() => ReferenceEquals(_view, view) &&
            ReferenceEquals(_binding, binding) && _attachmentGeneration == generation;
        void AttachRoot()
        {
            if (IsAttached()) binding.attachRootWidget(binding.wrapWithDefaultView(_rootFactory()));
        }
        async Task InitializeAsync()
        {
            try
            {
                await _initialize!();
                // Async resource completion must return to the view's event
                // loop, including when there is no root producing frames yet.
                DartAsyncRuntime.scheduleMicrotask(AttachRoot);
            }
            catch (Exception error)
            {
                DartAsyncRuntime.scheduleMicrotask(() =>
                {
                    if (IsAttached()) FlutterError.reportError(new FlutterErrorDetails(
                        error, library: "Doroti widget bootstrap"));
                });
            }
        }
        binding.scheduleFrameCallback(timestamp =>
        {
            if (!IsAttached()) return;
            if (_initialize is null) AttachRoot();
            else _ = InitializeAsync();
        }, scheduleNewFrame: false);
        // Ordinary frames are disabled until a root exists. Bootstrap is the
        // entrypoint's responsibility, just as runApp owns Flutter's warm-up.
        binding.scheduleForcedFrame();
    }

    public void DetachView(DorotiView view)
    {
        if (ReferenceEquals(_view, view))
        {
            _view = null;
            _attachmentGeneration++;
        }
    }

    public void Shutdown()
    {
        _attachmentGeneration++;
        _binding?.Dispose();
        _binding = null;
        _view = null;
    }
}
