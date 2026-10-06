using Doroti.Ui;
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

/// <summary>
/// Wraps a file-selection button whose callback calls PickFilesAsync with Options.
/// Browser hosts open the picker inside the native tap before dispatching to managed code.
/// </summary>
public sealed class FilePickerActivation(Widget child, FilePickOptions options, Key? key = null) : StatefulWidget(key: key)
{
    public Widget Child { get; } = child;
    public FilePickOptions Options { get; } = options.Normalize();
    public override IState createState() => new FilePickerActivationState();
}

internal sealed class FilePickerActivationState : State<FilePickerActivation>
{
    private readonly string _identifier = $"doroti.file-picker.{Guid.NewGuid():N}";
    private DorotiView? _view;
    private IDisposable? _registration;

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        var view = View.of(context);
        if (_view == view) return;
        _view = view;
        Bind();
    }

    public override void didUpdateWidget(FilePickerActivation oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (oldWidget.Options.AllowMultiple != widget.Options.AllowMultiple ||
            !oldWidget.Options.Extensions!.SequenceEqual(widget.Options.Extensions!)) Bind();
    }

    private void Bind()
    {
        _registration?.Dispose();
        _registration = _view?.GetCapabilityOrDefault<IFilePickerHostCapability>(DorotiCapabilityIds.FilePicker)
            ?.RegisterActivation(_identifier, widget.Options);
    }

    public override Widget build(BuildContext context) => new Semantics(identifier: _identifier, child: widget.Child);

    public override void dispose()
    {
        _registration?.Dispose();
        base.dispose();
    }
}
