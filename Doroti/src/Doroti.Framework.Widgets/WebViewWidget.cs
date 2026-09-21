using Doroti.Ui;

namespace Doroti.Framework.Widgets;

/// <summary>Bounded native WebView. The caller owns the controller and its disposal.</summary>
public sealed class WebViewWidget(WebViewController controller, Key? key = null)
    : StatefulWidget(key: key)
{
    public WebViewController Controller { get; } = controller;

    public override IState createState() => new WebViewWidgetState();
}

internal sealed class WebViewWidgetState : State<WebViewWidget>
{
    private PlatformViewHandle? _handle;
    private Exception? _error;
    private readonly FocusNode _focus = new(debugLabel: "WebView");

    public override void initState()
    {
        base.initState();
        widget.Controller.AttachWidget();
        widget.Controller.Focused += Focused;
        _ = InitializeAsync(widget.Controller);
    }

    private async Task InitializeAsync(WebViewController controller)
    {
        await Task.Yield();
        try
        {
            var handle = await controller.Ready.ConfigureAwait(false);
            controller.Owner.DispatchPlatformEvent(() =>
            {
                if (mounted)
                {
                    setState(() => _handle = handle);
                }
            });
        }
        catch (ObjectDisposedException) when (!mounted) { }
        catch (Exception error)
        {
            try
            {
                controller.Owner.DispatchPlatformEvent(() =>
                {
                    if (mounted)
                    {
                        setState(() => _error = error);
                    }
                });
            }
            catch (ObjectDisposedException)
            { /* Owner closed before native creation completed. */
            }
        }
    }

    private void Focused() =>
        widget.Controller.Owner.DispatchPlatformEvent(() =>
        {
            if (mounted)
            {
                _focus.requestFocus();
            }
        });

    private async void FocusChanged(bool focused)
    {
        try
        {
            await widget.Controller.SetFocusAsync(focused);
        }
        catch (Exception error)
        {
            FlutterError.reportError(
                new FlutterErrorDetails(exception: error, library: "webview focus")
            );
        }
    }

    public override void didUpdateWidget(WebViewWidget oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (oldWidget.Controller != widget.Controller)
        {
            throw new InvalidOperationException("Changing WebViewController requires a new key.");
        }
    }

    public override Widget build(BuildContext context)
    {
        if (View.of(context) != widget.Controller.Owner)
        {
            throw new InvalidOperationException("WebViewWidget belongs to another owner.");
        }

        if (_error is not null)
        {
            throw new InvalidOperationException("WebView creation failed.", _error);
        }

        return new Focus(
            focusNode: _focus,
            onFocusChange: FocusChanged,
            includeSemantics: false,
            canRequestFocus: _handle is not null,
            child: new PlatformViewLeaf(_handle)
        );
    }

    public override void dispose()
    {
        widget.Controller.Focused -= Focused;
        widget.Controller.DetachWidget();
        _focus.dispose();
        base.dispose();
    }
}
