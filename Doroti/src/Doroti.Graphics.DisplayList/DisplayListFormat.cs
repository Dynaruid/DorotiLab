using System.Collections.ObjectModel;

namespace Doroti.Graphics.DisplayList;

public static class DisplayListFormat
{
    public const uint Magic = 0x54534C44;
    public const ushort SchemaVersion = 2;
    public const ushort HeaderSize = 112;
    public const int ResourceEntrySize = 32;
    public const int CommandEnvelopeSize = 8;
    public const int ChecksumOffset = 104;
    public const int ChecksumSize = 4;
    public const uint MaximumByteLength = 64 * 1024 * 1024;
    public const uint MaximumCommandCount = 1_000_000;
    public const uint MaximumResourceCount = 65_536;
    public const uint MaximumStringTableByteLength = 16 * 1024 * 1024;
    public const uint MaximumCollectionCount = 1_000_000;
    public const int MaximumNestingDepth = 32;
}

[Flags]
public enum DisplayListFlags : uint
{
    None = 0,
    ChecksumPresent = 1 << 0,
    DiagnosticCapture = 1 << 1,
}

[Flags]
public enum DisplayResourceFlags : ushort
{
    None = 0,
    Recoverable = 1 << 0,
}

public enum DisplayResourceKind : ushort
{
    Font = 1,
    Image = 2,
    RuntimeEffect = 3,
    RetainedScene = 4,
}

public enum DisplayListOpcode : ushort
{
    Save = 1,
    Restore = 2,
    SaveLayer = 3,
    Transform = 4,
    ClipRect = 5,
    ClipRoundedRect = 6,
    ClipPath = 7,

    DrawColor = 16,
    DrawPaint = 17,
    DrawLine = 18,
    DrawPoints = 19,
    DrawRect = 20,
    DrawRoundedRect = 21,
    DrawDoubleRoundedRect = 22,
    DrawCircle = 23,
    DrawOval = 24,
    DrawArc = 25,
    DrawPath = 26,
    DrawShadow = 27,
    DrawImage = 28,
    DrawImageRect = 29,
    DrawNinePatch = 30,
    DrawParagraph = 31,

    PushOpacity = 48,
    PushColorFilter = 49,
    PushImageFilter = 50,
    PushBackdropFilter = 51,
    PushShaderMask = 52,
    DrawRetainedScene = 53,
}

public enum DisplayListFailureCode
{
    BufferTooShort,
    InvalidMagic,
    UnsupportedVersion,
    InvalidHeader,
    LengthMismatch,
    BoundsExceeded,
    LimitExceeded,
    UnknownFlags,
    ChecksumMismatch,
    InvalidResource,
    DuplicateResource,
    MissingResource,
    InvalidString,
    NonCanonicalEncoding,
    UnknownOpcode,
    InvalidCommand,
    InvalidValue,
}

public enum DisplayListSceneTerminalKind
{
    Submitted,
    Superseded,
    Failed,
}

public readonly record struct DisplayResourceReference(
    DisplayResourceKind Kind,
    ulong Id,
    uint Version);

public readonly record struct DisplayResourceFingerprint(ulong Low, ulong High);

public sealed record DisplayResourceDescriptor(
    DisplayResourceReference Reference,
    DisplayResourceFingerprint Fingerprint,
    DisplayResourceFlags Flags = DisplayResourceFlags.Recoverable);

public readonly record struct DisplayListSceneMetadata(
    ulong ViewId,
    ulong SceneSequence,
    ulong BuildToken,
    ulong ResizeEpoch,
    ulong SurfaceGeneration,
    ulong ContextGeneration,
    float LogicalWidth,
    float LogicalHeight,
    uint PhysicalWidth,
    uint PhysicalHeight,
    float DevicePixelRatio);

public readonly record struct DisplayListWireHeader(
    uint ByteLength,
    DisplayListFlags Flags,
    DisplayListSceneMetadata Scene,
    uint CommandCount,
    uint ResourceCount,
    uint StringTableByteLength,
    uint CommandByteLength,
    uint ResourceTableByteLength,
    uint Checksum);

public sealed class DisplayListDocument
{
    public DisplayListDocument(
        DisplayListSceneMetadata scene,
        IEnumerable<DisplayResourceDescriptor>? resources,
        IEnumerable<DisplayListCommand>? commands,
        DisplayListFlags flags = DisplayListFlags.ChecksumPresent)
    {
        Scene = scene;
        Resources = new ReadOnlyCollection<DisplayResourceDescriptor>((resources ?? []).ToArray());
        Commands = new ReadOnlyCollection<DisplayListCommand>((commands ?? []).ToArray());
        Flags = flags;
    }

    public DisplayListSceneMetadata Scene { get; }

    public IReadOnlyList<DisplayResourceDescriptor> Resources { get; }

    public IReadOnlyList<DisplayListCommand> Commands { get; }

    public DisplayListFlags Flags { get; }
}

public readonly record struct DisplayListSceneTerminal(
    ulong SceneSequence,
    DisplayListSceneTerminalKind Kind,
    DisplayListFailureCode? FailureCode,
    string? Detail)
{
    public static DisplayListSceneTerminal Submitted(ulong sequence, string? detail = null) =>
        new(sequence, DisplayListSceneTerminalKind.Submitted, null, detail);

    public static DisplayListSceneTerminal Superseded(ulong sequence, string? detail = null) =>
        new(sequence, DisplayListSceneTerminalKind.Superseded, null, detail);

    public static DisplayListSceneTerminal Failed(
        ulong sequence,
        DisplayListFailureCode code,
        string detail) =>
        new(sequence, DisplayListSceneTerminalKind.Failed, code, detail);
}

public sealed record DisplayListFailure(
    DisplayListFailureCode Code,
    int Offset,
    string Message,
    DisplayListSceneTerminal Terminal);

public sealed class DisplayListDecodeResult
{
    private DisplayListDecodeResult(
        DisplayListDocument? document,
        DisplayListWireHeader? header,
        DisplayListFailure? failure)
    {
        Document = document;
        Header = header;
        Failure = failure;
    }

    public bool IsSuccess => Failure is null;

    public DisplayListDocument? Document { get; }

    public DisplayListWireHeader? Header { get; }

    public DisplayListFailure? Failure { get; }

    internal static DisplayListDecodeResult Success(
        DisplayListDocument document,
        DisplayListWireHeader header) =>
        new(document, header, null);

    internal static DisplayListDecodeResult Failed(DisplayListFailure failure) =>
        new(null, null, failure);
}
