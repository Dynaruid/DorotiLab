using Doroti.Ui;

namespace Doroti.Framework.Widgets;

/// <summary>Native direct-input view with an explicit owner and bounded layout.</summary>
public sealed class PlatformView : StatefulWidget
{
    public PlatformView(DorotiView owner, PlatformViewRequest request, System.Action<PlatformViewHandle>? onCreated = null,
        System.Action<Exception>? onError = null, Key? key = null) : base(key: key)
    { Owner = owner; Request = request; OnCreated = onCreated; OnError = onError; }
    public PlatformView(DorotiView owner, PlatformViewDescriptor descriptor, System.Action<PlatformViewHandle>? onCreated = null,
        System.Action<Exception>? onError = null, Key? key = null) : base(key: key)
    { Owner = owner; Descriptor = descriptor; OnCreated = onCreated; OnError = onError; }
    public DorotiView Owner { get; }
    public PlatformViewRequest? Request { get; }
    public PlatformViewDescriptor? Descriptor { get; }
    public System.Action<PlatformViewHandle>? OnCreated { get; }
    public System.Action<Exception>? OnError { get; }
    public override IState createState() => new PlatformViewStateImpl();
}

internal sealed class PlatformViewStateImpl : State<PlatformView>
{
    private PlatformViewClient? _client;
    private PlatformViewHandle? _handle;
    private Exception? _error;
    private readonly FocusNode _focusNode = new(debugLabel: "PlatformView");
    public override void initState() { base.initState(); Start(); }
    private void Start()
    {
        _handle = null; _error = null;
        var client = widget.Descriptor is { } descriptor ? new PlatformViewClient(widget.Owner, descriptor) :
            new PlatformViewClient(widget.Owner, widget.Request!);
        _client = client;
        client.Focused += NativeFocused;
        _ = InitializeAsync(client, widget.Owner);
    }
    private void NativeFocused()
    {
        if (mounted) widget.Owner.DispatchPlatformEvent(() => { if (mounted) _focusNode.requestFocus(); });
    }
    private void FocusChanged(bool focused)
    {
        if (_client is { } client) _ = SetNativeFocusAsync(client, focused);
    }
    private async Task SetNativeFocusAsync(PlatformViewClient client, bool focused)
    {
        try { await client.SetFocusAsync(focused); }
        catch (OperationCanceledException) { }
        catch (Exception error)
        {
            if (mounted && ReferenceEquals(_client, client))
                FlutterError.reportError(new FlutterErrorDetails(exception: error, library: "platform view focus"));
        }
    }
    private async Task InitializeAsync(PlatformViewClient client, DorotiView owner)
    {
        // Native completion can arrive from the HWND UI queue while the framework
        // is building on its render worker. Serialize the state change as a later
        // isolate event, including when a factory completes synchronously.
        await Task.Yield();
        try
        {
            var handle = await client.Ready.ConfigureAwait(false);
            owner.DispatchPlatformEvent(() =>
            {
                if (!mounted || !ReferenceEquals(_client, client)) return;
                setState(() => _handle = handle);
                widget.OnCreated?.Invoke(handle);
            });
        }
        catch (ObjectDisposedException) when (!mounted || !ReferenceEquals(_client, client)) { }
        catch (Exception exception)
        {
            try
            {
                owner.DispatchPlatformEvent(() =>
                {
                    if (!mounted || !ReferenceEquals(_client, client)) return;
                    setState(() => _error = exception);
                    widget.OnError?.Invoke(exception);
                });
            }
            catch (ObjectDisposedException) { /* The owner closed before the completion event. */ }
        }
    }
    public override void didUpdateWidget(PlatformView oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (oldWidget.Owner != widget.Owner || oldWidget.Request != widget.Request || oldWidget.Descriptor != widget.Descriptor)
            throw new InvalidOperationException("Changing PlatformView owner/request requires a new widget key so disposal completes before ID reuse.");
    }
    public override Widget build(BuildContext context)
    {
        if (_error is not null) throw new InvalidOperationException("PlatformView creation failed.", _error);
        return new Focus(focusNode: _focusNode, onFocusChange: FocusChanged,
            canRequestFocus: _handle is not null, includeSemantics: false, child: new PlatformViewLeaf(_handle));
    }
    public override void dispose()
    {
        var client = _client; _client = null;
        if (client is not null) client.Focused -= NativeFocused;
        if (client is not null) _ = DisposeClientAsync(client, widget.OnError);
        _focusNode.dispose();
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
