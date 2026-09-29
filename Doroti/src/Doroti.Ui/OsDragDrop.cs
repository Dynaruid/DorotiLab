namespace Doroti.Ui;

[Flags]
public enum OsDropAction { None = 0, Copy = 1, Move = 2, Link = 4 }
public enum OsDropPhase { Enter, Over, Leave, Drop, Error }
public enum OsDropFailure { None, Denied, InvalidData, Failed }

public static class OsDropFormats
{
    public const string Files = "application/x-doroti-files";
    public const string Text = "text/plain";
    public const string UriList = "text/uri-list";
}

public sealed record OsDropSupport(bool CanReceive, bool CanSend, OsDropAction Actions,
    IReadOnlyList<string> Formats, bool VirtualFiles = false);

/// <summary>View-local logical coordinates; Bounds is in the same coordinate space.</summary>
public sealed record OsDropOffer(Offset Position, IReadOnlyList<string> Formats,
    OsDropAction SourceActions, OsDropAction RequestedAction = OsDropAction.None);

public sealed record OsDropOptions(OsDropAction Actions, IReadOnlyList<string> Formats, Rect? Bounds = null);

public sealed record OsDropEvent(OsDropPhase Phase, OsDropOffer Offer, OsDropAction Action,
    OsDropData? Data = null, OsDropFailure Failure = OsDropFailure.None, string? Message = null);

/// <summary>Receiver owns a delivered payload. Dispose releases every read grant; owner close also revokes it.</summary>
public sealed class OsDropData : IDisposable
{
    private readonly CancellationTokenSource _lifetime = new();
    private int _disposed;
    public OsDropData(IEnumerable<IPickedFile>? files = null, string? text = null, IEnumerable<Uri>? uris = null)
    {
        Files = Array.AsReadOnly((files ?? []).ToArray());
        Text = text;
        Uris = Array.AsReadOnly((uris ?? []).ToArray());
        Lifetime = _lifetime.Token;
    }
    public IReadOnlyList<IPickedFile> Files { get; }
    public string? Text { get; }
    public IReadOnlyList<Uri> Uris { get; }
    public CancellationToken Lifetime { get; }
    public bool IsDisposed => Volatile.Read(ref _disposed) != 0;
    public event Action? Disposed;
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
        List<Exception> errors = [];
        try { _lifetime.Cancel(); } catch (Exception error) { errors.Add(error); }
        foreach (var file in Files)
            try { file.Dispose(); } catch (Exception error) { errors.Add(error); }
        try { Disposed?.Invoke(); } catch (Exception error) { errors.Add(error); }
        Disposed = null;
        _lifetime.Dispose();
        if (errors.Count != 0) throw new AggregateException(errors);
    }
}

public interface IOsDropRegistration : IDisposable { void Update(OsDropOptions options); }

/// <summary>One explicit receiver per view. Register/dispose with the receiving screen's lifetime.</summary>
public interface IOsDragDropHostCapability
{
    OsDropSupport Support { get; }
    IOsDropRegistration Register(OsDropOptions options, Action<OsDropEvent> onEvent);
}

/// <summary>Export text and URI references. The caller keeps source data alive until completion.
/// File URIs refer to existing files; virtual file providers are a separate unsupported feature.</summary>
public sealed record OsDragSourceData(string? Text = null, IReadOnlyList<Uri>? Uris = null,
    ReadOnlyMemory<byte> ImagePng = default, Offset? ImageHotspot = null);

public sealed record OsDragResult(OsDropAction Action, bool Canceled);

/// <summary>Start from a user drag gesture. A returned Move only reports negotiation;
/// the application decides whether and how to remove its original data.</summary>
public interface IOsDragSourceHostCapability
{
    OsDropAction Actions { get; }
    ValueTask<OsDragResult> StartDragAsync(OsDragSourceData data, OsDropAction actions = OsDropAction.Copy,
        CancellationToken cancellationToken = default);
}
