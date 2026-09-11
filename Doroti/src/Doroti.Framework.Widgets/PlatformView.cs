using Doroti.Framework.Foundation;
using Doroti.Framework.Rendering;
using Doroti.Framework.Services;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

/// <summary>Native direct-input view with an explicit owner and bounded layout.</summary>
public sealed class PlatformView : StatefulWidget
{
    public PlatformView(DorotiView owner, PlatformViewRequest request, System.Action<PlatformViewHandle>? onCreated = null,
        System.Action<Exception>? onError = null, Key? key = null) : base(key: key)
    { Owner = owner; Request = request; OnCreated = onCreated; OnError = onError; }
    public DorotiView Owner { get; }
    public PlatformViewRequest Request { get; }
    public System.Action<PlatformViewHandle>? OnCreated { get; }
    public System.Action<Exception>? OnError { get; }
    public override IState createState() => new PlatformViewStateImpl();
}

internal sealed class PlatformViewStateImpl : State<PlatformView>
{
    private PlatformViewClient? _client;
    private PlatformViewHandle? _handle;
    private Exception? _error;
    public override void initState() { base.initState(); Start(); }
    private void Start()
    {
        _handle = null; _error = null;
        var client = new PlatformViewClient(widget.Owner, widget.Request);
        _client = client;
        _ = InitializeAsync(client);
    }
    private async Task InitializeAsync(PlatformViewClient client)
    {
        try
        {
            var handle = await client.Ready;
            if (!mounted || !ReferenceEquals(_client, client)) return;
            setState(() => _handle = handle);
            widget.OnCreated?.Invoke(handle);
        }
        catch (Exception exception)
        {
            if (!mounted || !ReferenceEquals(_client, client)) return;
            setState(() => _error = exception);
            widget.OnError?.Invoke(exception);
        }
    }
    public override void didUpdateWidget(PlatformView oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (oldWidget.Owner != widget.Owner || oldWidget.Request != widget.Request)
            throw new InvalidOperationException("Changing PlatformView owner/request requires a new widget key so disposal completes before ID reuse.");
    }
    public override Widget build(BuildContext context)
    {
        if (_error is not null) throw new InvalidOperationException("PlatformView creation failed.", _error);
        return new PlatformViewLeaf(_handle);
    }
    public override void dispose()
    {
        var client = _client; _client = null;
        if (client is not null) _ = DisposeClientAsync(client, widget.OnError);
        base.dispose();
    }
    private static async Task DisposeClientAsync(PlatformViewClient client, System.Action<Exception>? onError)
    {
        try { await client.DisposeAsync(); }
        catch (Exception exception)
        {
            if (onError is not null) onError(exception);
            else FlutterError.reportError(new FlutterErrorDetails(exception: exception, library: "platform views"));
        }
    }
}

internal sealed class PlatformViewLeaf(PlatformViewHandle? handle) : LeafRenderObjectWidget
{
    public override RenderObject createRenderObject(BuildContext context) => new RenderPlatformView { Handle = handle };
    public override void updateRenderObject(BuildContext context, RenderObject renderObject) => ((RenderPlatformView)renderObject).Handle = handle;
}

public sealed class PointerInterceptor : SingleChildRenderObjectWidget
{
    public PointerInterceptor(Widget child, bool intercepting = true, bool debug = false, Key? key = null) : base(key: key, child: child)
    { Intercepting = intercepting; Debug = debug; }
    public bool Intercepting { get; }
    public bool Debug { get; }
    public override RenderObject createRenderObject(BuildContext context) => new RenderPointerInterceptor { Intercepting = Intercepting, Debug = Debug };
    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var interceptor = (RenderPointerInterceptor)renderObject;
        interceptor.Intercepting = Intercepting;
        interceptor.Debug = Debug;
    }
}
