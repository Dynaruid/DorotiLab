#if IOS || MACCATALYST
using Doroti.Hosting;
using Doroti.Ui;
using Foundation;
using UIKit;

namespace Doroti.Host.Maui;

/// <summary>Copy text/URI reception. Native child editors keep their own drop interactions.</summary>
internal sealed class UIKitOsDrop : UIDropInteractionDelegate, IOsDragDropHostCapability
{
    private const string TextType = "public.utf8-plain-text", UrlType = "public.url";
    private readonly OsDropReceiver _receiver;
    private UIView? _view;
    private readonly VisualElement? _element;
    private readonly UIDropInteraction _interaction;
    private readonly CancellationTokenSource _lifetime = new();
    private long _registration;
    private bool _closed;

    private UIKitOsDrop(Action<Action> dispatch)
    {
        _receiver = new(new(true, false, OsDropAction.Copy, [OsDropFormats.Text, OsDropFormats.UriList]), dispatch);
        _interaction = new(this);
    }
    internal UIKitOsDrop(UIView view, Action<Action> dispatch) : this(dispatch) => Attach(view);
    internal UIKitOsDrop(VisualElement element, Action<Action> dispatch) : this(dispatch)
    {
        _element = element;
        element.HandlerChanged += HandlerChanged;
        HandlerChanged(null, EventArgs.Empty);
    }
    private void HandlerChanged(object? sender, EventArgs args) => Attach(_element?.Handler?.PlatformView as UIView);
    private void Attach(UIView? view)
    {
        if (ReferenceEquals(view, _view)) return;
        _view?.RemoveInteraction(_interaction);
        _view = view;
        _view?.AddInteraction(_interaction);
    }
    public OsDropSupport Support => _receiver.Support;
    public IOsDropRegistration Register(OsDropOptions options, Action<OsDropEvent> onEvent)
    {
        var inner = _receiver.Register(options, onEvent);
        _registration++;
        return new Registration(this, inner);
    }
    private sealed class Registration(UIKitOsDrop owner, IOsDropRegistration inner) : IOsDropRegistration
    {
        private bool _disposed;
        public void Update(OsDropOptions options) { inner.Update(options); owner._registration++; }
        public void Dispose() { if (_disposed) return; _disposed = true; owner._registration++; inner.Dispose(); }
    }
    private OsDropOffer Offer(IUIDropSession session)
    {
        var point = session.LocationInView(_view!);
        List<string> formats = [];
        if (session.Items.Any(item => item.ItemProvider.HasItemConformingTo(TextType))) formats.Add(OsDropFormats.Text);
        if (session.Items.Any(item => item.ItemProvider.HasItemConformingTo(UrlType))) formats.Add(OsDropFormats.UriList);
        return new(new(point.X, point.Y), formats, OsDropAction.Copy);
    }
    public override bool CanHandleSession(UIDropInteraction interaction, IUIDropSession session) =>
        !_closed && _view is not null && session.Items.Length <= 128 && Offer(session).Formats.Count > 0;
    public override void SessionDidEnter(UIDropInteraction interaction, IUIDropSession session) => _receiver.Hover(OsDropPhase.Enter, Offer(session));
    public override UIDropProposal SessionDidUpdate(UIDropInteraction interaction, IUIDropSession session) =>
        new(_receiver.Hover(OsDropPhase.Over, Offer(session)) == OsDropAction.Copy ? UIDropOperation.Copy : UIDropOperation.Cancel);
    public override void SessionDidExit(UIDropInteraction interaction, IUIDropSession session) => _receiver.Hover(OsDropPhase.Leave, Offer(session));
    public override void PerformDrop(UIDropInteraction interaction, IUIDropSession session)
    {
        if (!CanHandleSession(interaction, session)) return;
        var offer = Offer(session);
        if (_receiver.Hover(OsDropPhase.Over, offer) != OsDropAction.Copy) return;
        _ = ReceiveAsync(session.Items.Select(item => item.ItemProvider).ToArray(), offer, _registration);
    }
    private async Task ReceiveAsync(NSItemProvider[] providers, OsDropOffer offer, long registration)
    {
        try
        {
            List<string> text = [];
            List<Uri> uris = [];
            var length = 0;
            foreach (var provider in providers)
            {
                foreach (var type in new[] { TextType, UrlType })
                {
                    if (!provider.HasItemConformingTo(type)) continue;
                    var value = await ReadAsync(provider, type, _lifetime.Token);
                    if ((length += value.Length) > 1024 * 1024)
                        throw new InvalidDataException("Drop data exceeds 1 MiB.");
                    if (type == TextType) text.Add(value);
                    else if (Uri.TryCreate(value.TrimEnd('\0'), UriKind.Absolute, out var uri)) uris.Add(uri);
                    else throw new InvalidDataException("Invalid drop URL.");
                }
            }
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                if (_closed || registration != _registration) return;
                _receiver.Drop(offer, formats => new(null,
                    formats.Contains(OsDropFormats.Text) ? string.Join("\n", text) : null,
                    formats.Contains(OsDropFormats.UriList) ? uris : null));
            });
        }
        catch (OperationCanceledException) { }
        catch (Exception error)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                if (!_closed && registration == _registration) _receiver.Drop(offer, _ => throw error);
            });
        }
    }
    private static async Task<string> ReadAsync(NSItemProvider provider, string type, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        var completion = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        using var progress = provider.LoadDataRepresentation(type, (data, error) =>
        {
            if (token.IsCancellationRequested) { completion.TrySetCanceled(token); return; }
            try
            {
                if (error is not null) throw new NSErrorException(error);
                if (data is null || data.Length > 1024 * 1024) throw new InvalidDataException("Invalid or oversized drop data.");
                completion.TrySetResult(System.Text.Encoding.UTF8.GetString(data.ToArray()));
            }
            catch (Exception failure) { completion.TrySetException(failure); }
        });
        using var registration = token.Register(() => { progress.Cancel(); completion.TrySetCanceled(token); });
        return await completion.Task;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && !_closed)
        {
            _closed = true;
            _lifetime.Cancel();
            _receiver.Dispose();
            if (_element is not null) _element.HandlerChanged -= HandlerChanged;
            Attach(null);
            _interaction.Dispose();
        }
        base.Dispose(disposing);
    }
}
#endif
