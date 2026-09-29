#if MACOS
using AppKit;
using Foundation;
using Doroti.Hosting;
using Doroti.Ui;

namespace Doroti.Host.Maui;

/// <summary>Copy reception on the Doroti surface. Native child views retain their own drop behavior.</summary>
internal sealed class AppKitOsDrop(Action<Action> dispatch) : IOsDragDropHostCapability, IDisposable
{
    internal const string FileType = "public.file-url", TextType = "public.utf8-plain-text", UrlType = "public.url";
    private readonly OsDropReceiver _receiver = new(new(true, false, OsDropAction.Copy,
        [OsDropFormats.Files, OsDropFormats.Text, OsDropFormats.UriList]), dispatch);
    public OsDropSupport Support => _receiver.Support;
    public IOsDropRegistration Register(OsDropOptions options, Action<OsDropEvent> onEvent) => _receiver.Register(options, onEvent);

    internal NSDragOperation Receive(NSView view, INSDraggingInfo info, OsDropPhase phase)
    {
        try
        {
            var board = info.DraggingPasteboard;
            var types = board.Types ?? [];
            List<string> formats = [];
            if (types.Contains(FileType)) formats.Add(OsDropFormats.Files);
            if (types.Contains(TextType)) formats.Add(OsDropFormats.Text);
            if (types.Contains(UrlType) || types.Contains(FileType)) formats.Add(OsDropFormats.UriList);
            var point = view.ConvertPointFromView(info.DraggingLocation, null);
            var offer = new OsDropOffer(new(point.X, view.IsFlipped ? point.Y : view.Bounds.Height - point.Y), formats,
                info.DraggingSourceOperationMask.HasFlag(NSDragOperation.Copy) ? OsDropAction.Copy : OsDropAction.None);
            var action = phase == OsDropPhase.Drop ? _receiver.Drop(offer, selected => Read(board, selected)) : _receiver.Hover(phase, offer);
            return action == OsDropAction.Copy ? NSDragOperation.Copy : NSDragOperation.None;
        }
        catch (Exception error) { System.Diagnostics.Trace.TraceWarning(error.ToString()); return NSDragOperation.None; }
    }

    internal static OsDropData Read(NSPasteboard board, IReadOnlyList<string> formats)
    {
        List<IPickedFile> files = [];
        List<Uri> uris = [];
        List<string> text = [];
        try
        {
            var items = board.PasteboardItems ?? [];
            if (items.Length > 1024) throw new InvalidDataException("Drop contains more than 1024 items.");
            foreach (var item in items)
            {
                var location = item.GetStringForType(FileType) ?? item.GetStringForType(UrlType);
                if (location is not null)
                {
                    if (!Uri.TryCreate(location, UriKind.Absolute, out var uri)) throw new InvalidDataException("Invalid drop URL.");
                    if (formats.Contains(OsDropFormats.UriList)) uris.Add(uri);
                    if (uri.IsFile && formats.Contains(OsDropFormats.Files))
                    {
                        using var url = new NSUrl(location);
                        files.Add(new AppKitReadFile(url));
                    }
                }
                if (formats.Contains(OsDropFormats.Text) && item.GetStringForType(TextType) is { } value)
                {
                    if (value.Length > 1024 * 1024 || text.Sum(s => s.Length) + value.Length > 1024 * 1024)
                        throw new InvalidDataException("Drop text exceeds 1 MiB.");
                    text.Add(value);
                }
            }
            return new(files, text.Count == 0 ? null : string.Join("\n", text), uris);
        }
        catch { foreach (var file in files) file.Dispose(); throw; }
    }
    public void Dispose() => _receiver.Dispose();
}
#endif
