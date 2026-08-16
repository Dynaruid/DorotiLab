using Doroti.Framework.Widgets;
using Doroti.Hosting;
using Doroti.Ui;

namespace Doroti.Framework;

/// <summary>
/// Hosts a widget root behind the stable Doroti application-entrypoint contract.
/// </summary>
public sealed class DorotiWidgetEntrypoint(Func<Widget> rootFactory) : IDorotiViewEntrypoint
{
    private readonly Func<Widget> _rootFactory = rootFactory ?? throw new ArgumentNullException(nameof(rootFactory));
    private WidgetsFlutterBinding? _binding;
    private DorotiView? _view;

    public void Bootstrap(PlatformDispatcher dispatcher) => _binding = new WidgetsFlutterBinding(dispatcher);

    public void AttachView(DorotiView view)
    {
        if (_binding is null)
            throw new InvalidOperationException("The Doroti widget runtime is not bootstrapped.");
        if (_view is not null)
            throw new InvalidOperationException("This Doroti widget entrypoint already owns a view.");

        _view = view;
        _binding.scheduleFrameCallback(_ => _binding.attachRootWidget(
            _binding.wrapWithDefaultView(_rootFactory())));
    }

    public void DetachView(DorotiView view)
    {
        if (ReferenceEquals(_view, view))
            _view = null;
    }

    public void Shutdown()
    {
        _binding?.Dispose();
        _binding = null;
        _view = null;
    }
}
