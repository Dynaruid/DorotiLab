// <doroti-reviewed-framework-source />
// Owner-bound replacement for the excluded Flutter web controller shim.
using Doroti.Framework.Services;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

/// <summary>Registered DOM factories share the native view asynchronous lifecycle.</summary>
internal sealed class RegisteredHtmlElementView(HtmlElementView definition) : StatefulWidget
{
    internal HtmlElementView Definition { get; } = definition;
    public override IState createState() => new RegisteredHtmlElementState();
}

internal sealed class RegisteredHtmlElementState : State<RegisteredHtmlElementView>
{
    private readonly long _id = Platform_viewsLibrary.platformViewsRegistry.getNextPlatformViewId();
    private PlatformViewRequest? _request;
    public override Widget build(BuildContext context)
    {
        var definition = widget.Definition;
        if (definition.hitTestBehavior == Rendering.PlatformViewHitTestBehavior.translucent)
            throw new NotSupportedException("DOM direct input does not support translucent framework gesture mediation.");
        _request ??= new PlatformViewRequest(_id, definition.viewType,
            PlatformViewComposition.InterleavedComposition,
            CreationParameters: definition.creationParams is null ? default : new StandardMessageCodec().encodeMessage(definition.creationParams)!.asMemory());
        return new PlatformView(View.of(context), _request,
            onCreated: handle => definition.onPlatformViewCreated?.Invoke(handle.InstanceId));
    }
    public override void didUpdateWidget(RegisteredHtmlElementView oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (oldWidget.Definition.viewType != widget.Definition.viewType ||
            !Equals(oldWidget.Definition.creationParams, widget.Definition.creationParams))
            throw new InvalidOperationException("Changing an HtmlElementView factory/creation parameters requires a new widget key.");
    }
}
