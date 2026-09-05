using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text.Json;
using Doroti.Graphics.DisplayList;

const string expectedGoldenSha256 = "66412CCB5E02519BBD8C11ECAB5E63CE914E2DB745F6D51110BBD03F89CCBE42";

var representative = Fixtures.Representative();
var encoded = DisplayListEncoder.Encode(representative);
var secondEncoding = DisplayListEncoder.Encode(representative);
Require(encoded.AsSpan().SequenceEqual(secondEncoding), "Repeated encoding is byte-for-byte deterministic.");
Require(encoded.AsSpan(0, 4).SequenceEqual("DLST"u8), "The wire magic is the little-endian ASCII value DLST.");
Require(BinaryPrimitives.ReadUInt16LittleEndian(encoded.AsSpan(4)) == DisplayListFormat.SchemaVersion,
    "The wire schema version is encoded little-endian.");
Require(BinaryPrimitives.ReadUInt16LittleEndian(encoded.AsSpan(6)) == DisplayListFormat.HeaderSize,
    "The fixed header size is encoded little-endian.");
Require(BinaryPrimitives.ReadUInt32LittleEndian(encoded.AsSpan(8)) == encoded.Length,
    "The header byteLength covers the exact transferable buffer.");

var goldenSha256 = Convert.ToHexString(SHA256.HashData(encoded));
if (args.Contains("--emit-golden-json", StringComparer.Ordinal))
{
    Console.WriteLine(JsonSerializer.Serialize(new
    {
        schema = "doroti.display-list/v2",
        schemaVersion = DisplayListFormat.SchemaVersion,
        byteLength = encoded.Length,
        sha256 = goldenSha256,
        base64 = Convert.ToBase64String(encoded),
    }, new JsonSerializerOptions { WriteIndented = true }));
    return;
}
Require(
    string.Equals(expectedGoldenSha256, goldenSha256, StringComparison.Ordinal),
    $"The deterministic DisplayList golden changed. Expected {expectedGoldenSha256}; actual {goldenSha256}.");
VerifyCheckedInGolden(encoded, goldenSha256);

var decoded = DisplayListDecoder.Decode(encoded);
Require(decoded.IsSuccess && decoded.Document is not null && decoded.Header is not null,
    "The representative DisplayList decodes successfully.");
Require(decoded.Header.Value.CommandCount == representative.Commands.Count,
    "The decoded header preserves command count.");
Require(decoded.Header.Value.ResourceCount == representative.Resources.Count,
    "The decoded header preserves resource count.");
var decodedParagraph = decoded.Document.Commands.OfType<DisplayDrawParagraphCommand>().Single().Paragraph;
Require(decodedParagraph.HeightMultiplier == 1.2f,
    "The decoded paragraph preserves its positive height multiplier.");
Require(decodedParagraph.TextRuns.Count == 2 &&
    decodedParagraph.TextRuns[0].Text == "Doroti " &&
    decodedParagraph.TextRuns[1].FontWeight == 700,
    "The decoded paragraph preserves normalized mixed-style text runs.");
var reencoded = DisplayListEncoder.Encode(decoded.Document);
Require(encoded.AsSpan().SequenceEqual(reencoded), "encode -> decode -> re-encode preserves canonical bytes.");
var withoutChecksum = WithoutChecksum(encoded);
var decodedWithoutChecksum = DisplayListDecoder.Decode(withoutChecksum);
Require(decodedWithoutChecksum.IsSuccess && decodedWithoutChecksum.Document is not null,
    "The explicitly optional checksum form decodes when both flag and field are zero.");
Require(withoutChecksum.AsSpan().SequenceEqual(DisplayListEncoder.Encode(decodedWithoutChecksum.Document)),
    "The checksum-absent form also round-trips canonically.");

VerifySceneTerminals();
VerifyEncoderGuards();
VerifyEncodingCache(representative, encoded);
VerifyMalformedBuffers(encoded);
VerifyResourceFailures();
VerifyStringFailures();
VerifyDeterministicFuzz(encoded);

Console.WriteLine($"DisplayList contract: PASS ({encoded.Length} bytes, SHA-256 {goldenSha256})");

static void VerifySceneTerminals()
{
    var submitted = DisplayListSceneTerminal.Submitted(11, "surface.flush");
    var superseded = DisplayListSceneTerminal.Superseded(12, "newer exact scene");
    var failed = DisplayListSceneTerminal.Failed(13, DisplayListFailureCode.UnknownOpcode, "opcode");
    Require(submitted.Kind == DisplayListSceneTerminalKind.Submitted && submitted.FailureCode is null,
        "Submitted is a non-failure scene terminal.");
    Require(superseded.Kind == DisplayListSceneTerminalKind.Superseded && superseded.FailureCode is null,
        "Superseded is a non-failure scene terminal.");
    Require(failed.Kind == DisplayListSceneTerminalKind.Failed && failed.FailureCode == DisplayListFailureCode.UnknownOpcode,
        "Failed carries the deterministic protocol failure code.");
}

static void VerifyCheckedInGolden(byte[] encoded, string sha256)
{
    var path = Path.Combine(AppContext.BaseDirectory, "golden", "display-list-v2-full.json");
    Require(File.Exists(path), "The cross-language DisplayList golden is copied to validation output.");
    using var document = JsonDocument.Parse(File.ReadAllBytes(path));
    var root = document.RootElement;
    Require(root.GetProperty("schema").GetString() == "doroti.display-list/v2",
        "The checked-in golden declares the v2 schema.");
    Require(root.GetProperty("schemaVersion").GetUInt16() == DisplayListFormat.SchemaVersion,
        "The checked-in golden declares the current schema version.");
    Require(root.GetProperty("byteLength").GetInt32() == encoded.Length,
        "The checked-in golden declares the exact byte length.");
    Require(string.Equals(root.GetProperty("sha256").GetString(), sha256, StringComparison.Ordinal),
        "The checked-in golden declares the exact SHA-256.");
    var checkedInBytes = Convert.FromBase64String(root.GetProperty("base64").GetString() ?? string.Empty);
    Require(encoded.AsSpan().SequenceEqual(checkedInBytes),
        "The checked-in cross-language golden is byte-for-byte current.");
}

static void VerifyEncoderGuards()
{
    ExpectException<ArgumentException>(
        () => DisplayListEncoder.Encode(Fixtures.WithMissingImageResource()),
        "The encoder rejects a referenced resource that is not declared.");
    ExpectException<ArgumentException>(
        () => DisplayListEncoder.Encode(Fixtures.WithDuplicateResource()),
        "The encoder rejects duplicate resource identities.");
    ExpectException<ArgumentOutOfRangeException>(
        () => DisplayListEncoder.Encode(Fixtures.WithInvalidOpacity()),
        "The encoder rejects out-of-range typed values.");
    ExpectException<ArgumentOutOfRangeException>(
        () => DisplayListEncoder.Encode(Fixtures.WithInvalidParagraphHeightMultiplier()),
        "The encoder rejects a nonpositive paragraph height multiplier.");
}

static void VerifyEncodingCache(DisplayListDocument document, byte[] expected)
{
    var cache = new DisplayListEncodingCache();
    Require(DisplayListEncoder.Encode(document, cache).AsSpan().SequenceEqual(expected),
        "A cold encoding cache preserves the entire canonical golden.");
    Require(DisplayListEncoder.Encode(Fixtures.Representative(), cache).AsSpan().SequenceEqual(expected),
        "Fresh immutable values reuse payloads without changing canonical bytes.");
    Require(cache.FrameHits > 0 && cache.EntryCount > 0, "The warm encoding cache actually reuses payloads.");
    ExpectException<ArgumentException>(() => DisplayListEncoder.Encode(Fixtures.WithMissingImageResource(), cache),
        "Warm cached payloads never bypass per-scene resource validation.");
    ExpectException<ArgumentOutOfRangeException>(() => DisplayListEncoder.Encode(Fixtures.WithInvalidOpacity(), cache),
        "A warm encoding cache preserves malformed-value rejection.");
    var changed = new DisplayListDocument(document.Scene, [], [
        new DisplayDrawRectCommand(new DisplayRect(1, 2, 31, 42), new DisplayPaint(0xff123456)),
    ]);
    Require(DisplayListEncoder.Encode(changed, cache).AsSpan().SequenceEqual(DisplayListEncoder.Encode(changed)),
        "Changed geometry and paint cannot retrieve stale encoded payloads.");
    for (var i = 0; i < 9000; i++)
        DisplayListEncoder.Encode(new DisplayListDocument(document.Scene, [], [
            new DisplayDrawColorCommand((uint)i, DisplayBlendMode.Source),
        ]), cache);
    Require(cache.EntryCount <= 8192 && cache.RetainedBytes <= 8 * 1024 * 1024,
        "Encoding-cache eviction respects entry and charged-memory bounds.");
    cache.Clear();
    Require(cache.EntryCount == 0 && cache.RetainedBytes == 0, "Producer disposal releases the encoding cache.");
}

static void VerifyMalformedBuffers(byte[] canonical)
{
    AssertFailure([], DisplayListFailureCode.BufferTooShort, 0);

    var badMagic = MutateWithoutChecksum(canonical, bytes => bytes[0] ^= 0x80);
    AssertFailure(badMagic, DisplayListFailureCode.InvalidMagic, 0);

    var badVersion = MutateWithoutChecksum(canonical, bytes =>
        BinaryPrimitives.WriteUInt16LittleEndian(bytes.AsSpan(4), DisplayListFormat.SchemaVersion + 1));
    AssertFailure(badVersion, DisplayListFailureCode.UnsupportedVersion, canonicalSequence: 42);

    var badHeaderSize = MutateWithoutChecksum(canonical, bytes =>
        BinaryPrimitives.WriteUInt16LittleEndian(bytes.AsSpan(6), DisplayListFormat.HeaderSize - 1));
    AssertFailure(badHeaderSize, DisplayListFailureCode.InvalidHeader, canonicalSequence: 42);

    var badByteLength = canonical[..^1];
    AssertFailure(badByteLength, DisplayListFailureCode.LengthMismatch, canonicalSequence: 42);

    var badSectionLength = MutateWithoutChecksum(canonical, bytes =>
        BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(96),
            BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(96)) + 1));
    AssertFailure(badSectionLength, DisplayListFailureCode.LengthMismatch, canonicalSequence: 42);

    var unknownFlags = MutateWithoutChecksum(canonical, bytes =>
        BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(12), 0x8000_0000));
    AssertFailure(unknownFlags, DisplayListFailureCode.UnknownFlags, canonicalSequence: 42);

    var badReserved = MutateWithoutChecksum(canonical, bytes => bytes[108] = 1);
    AssertFailure(badReserved, DisplayListFailureCode.NonCanonicalEncoding, canonicalSequence: 42);

    var negativeZero = MutateWithoutChecksum(canonical, bytes =>
        BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(64), int.MinValue));
    AssertFailure(negativeZero, DisplayListFailureCode.NonCanonicalEncoding, canonicalSequence: 42);

    var badChecksum = (byte[])canonical.Clone();
    badChecksum[^1] ^= 1;
    AssertFailure(badChecksum, DisplayListFailureCode.ChecksumMismatch, canonicalSequence: 42);

    var checksumWithoutFlag = WithoutChecksum(canonical);
    BinaryPrimitives.WriteUInt32LittleEndian(checksumWithoutFlag.AsSpan(DisplayListFormat.ChecksumOffset), 1);
    AssertFailure(checksumWithoutFlag, DisplayListFailureCode.NonCanonicalEncoding, canonicalSequence: 42);

    var commandOffset = CommandOffset(canonical);
    var unknownOpcode = MutateWithoutChecksum(canonical, bytes =>
        BinaryPrimitives.WriteUInt16LittleEndian(bytes.AsSpan(commandOffset), ushort.MaxValue));
    AssertFailure(unknownOpcode, DisplayListFailureCode.UnknownOpcode, canonicalSequence: 42);

    var commandOutOfBounds = MutateWithoutChecksum(canonical, bytes =>
        BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(commandOffset + 4), uint.MaxValue));
    AssertFailure(commandOutOfBounds, DisplayListFailureCode.BoundsExceeded, canonicalSequence: 42);

    var tooManyCommands = MutateWithoutChecksum(canonical, bytes =>
        BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(84), DisplayListFormat.MaximumCommandCount + 1));
    AssertFailure(tooManyCommands, DisplayListFailureCode.LimitExceeded, canonicalSequence: 42);
}

static void VerifyResourceFailures()
{
    var oneImage = DisplayListEncoder.Encode(Fixtures.OneImage());
    var resourceOffset = DisplayListFormat.HeaderSize;

    var unknownKind = MutateWithoutChecksum(oneImage, bytes =>
        BinaryPrimitives.WriteUInt16LittleEndian(bytes.AsSpan(resourceOffset), ushort.MaxValue));
    AssertFailure(unknownKind, DisplayListFailureCode.InvalidResource, canonicalSequence: 7);

    var zeroVersion = MutateWithoutChecksum(oneImage, bytes =>
        BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(resourceOffset + 4), 0));
    AssertFailure(zeroVersion, DisplayListFailureCode.InvalidResource, canonicalSequence: 7);

    var missingResource = MutateWithoutChecksum(oneImage, bytes =>
        BinaryPrimitives.WriteUInt64LittleEndian(bytes.AsSpan(resourceOffset + 8), 999));
    AssertFailure(missingResource, DisplayListFailureCode.MissingResource, canonicalSequence: 7);

    var twoImages = DisplayListEncoder.Encode(Fixtures.TwoImages());
    var duplicate = MutateWithoutChecksum(twoImages, bytes =>
    {
        var first = bytes.AsSpan(resourceOffset, DisplayListFormat.ResourceEntrySize).ToArray();
        first.CopyTo(bytes.AsSpan(resourceOffset + DisplayListFormat.ResourceEntrySize));
    });
    AssertFailure(duplicate, DisplayListFailureCode.DuplicateResource, canonicalSequence: 8);

    var nonCanonicalOrder = MutateWithoutChecksum(twoImages, bytes =>
    {
        var first = bytes.AsSpan(resourceOffset, DisplayListFormat.ResourceEntrySize).ToArray();
        var second = bytes.AsSpan(resourceOffset + DisplayListFormat.ResourceEntrySize, DisplayListFormat.ResourceEntrySize).ToArray();
        second.CopyTo(bytes.AsSpan(resourceOffset));
        first.CopyTo(bytes.AsSpan(resourceOffset + DisplayListFormat.ResourceEntrySize));
    });
    AssertFailure(nonCanonicalOrder, DisplayListFailureCode.NonCanonicalEncoding, canonicalSequence: 8);
}

static void VerifyStringFailures()
{
    var strings = DisplayListEncoder.Encode(Fixtures.ThreeStrings());
    var stringOffset = checked((int)DisplayListFormat.HeaderSize +
        checked((int)BinaryPrimitives.ReadUInt32LittleEndian(strings.AsSpan(100))));

    var outOfBounds = MutateWithoutChecksum(strings, bytes =>
        BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(stringOffset), uint.MaxValue));
    AssertFailure(outOfBounds, DisplayListFailureCode.BoundsExceeded, canonicalSequence: 9);

    var invalidUtf8 = MutateWithoutChecksum(strings, bytes => bytes[stringOffset + sizeof(uint)] = 0xFF);
    AssertFailure(invalidUtf8, DisplayListFailureCode.InvalidString, canonicalSequence: 9);

    var nonCanonicalOrder = MutateWithoutChecksum(strings, bytes =>
    {
        const int entryLength = sizeof(uint) + 1;
        var first = bytes.AsSpan(stringOffset, entryLength).ToArray();
        var second = bytes.AsSpan(stringOffset + entryLength, entryLength).ToArray();
        second.CopyTo(bytes.AsSpan(stringOffset));
        first.CopyTo(bytes.AsSpan(stringOffset + entryLength));
    });
    AssertFailure(nonCanonicalOrder, DisplayListFailureCode.NonCanonicalEncoding, canonicalSequence: 9);
}

static void VerifyDeterministicFuzz(byte[] canonical)
{
    for (var length = 0; length < canonical.Length; length++)
    {
        var result = DisplayListValidator.Validate(canonical.AsSpan(0, length));
        Require(!result.IsSuccess && result.Failure is not null,
            $"Every truncated prefix fails without an out-of-bounds read (length {length}).");
        VerifySingleFailureTerminal(result.Failure, null);
    }

    var random = new Random(0xD07_01);
    for (var iteration = 0; iteration < 512; iteration++)
    {
        var mutated = WithoutChecksum(canonical);
        var index = random.Next(mutated.Length);
        mutated[index] ^= (byte)random.Next(1, 256);
        var result = DisplayListValidator.Validate(mutated);
        if (result.IsSuccess)
        {
            Require(result.Document is not null, "A successful fuzz decode returns a document.");
            Require(mutated.AsSpan().SequenceEqual(DisplayListEncoder.Encode(result.Document)),
                "Every accepted fuzz mutation is already canonical.");
        }
        else
        {
            Require(result.Failure is not null, "A rejected fuzz mutation returns one failure.");
            VerifySingleFailureTerminal(result.Failure, null);
        }
    }
}

static void AssertFailure(
    ReadOnlySpan<byte> bytes,
    DisplayListFailureCode expected,
    ulong? canonicalSequence = null)
{
    var result = DisplayListValidator.Validate(bytes);
    Require(!result.IsSuccess && result.Document is null && result.Header is null && result.Failure is not null,
        $"Malformed input returns a failure result ({expected}).");
    Require(result.Failure.Code == expected,
        $"Malformed input reports {expected}, not {result.Failure.Code}: {result.Failure.Message}");
    VerifySingleFailureTerminal(result.Failure, canonicalSequence);
}

static void VerifySingleFailureTerminal(DisplayListFailure failure, ulong? canonicalSequence)
{
    Require(failure.Terminal.Kind == DisplayListSceneTerminalKind.Failed,
        "A decode failure produces exactly the singular failed terminal carried by the failure result.");
    Require(failure.Terminal.FailureCode == failure.Code,
        "The failed terminal preserves the decode failure code.");
    if (canonicalSequence is not null)
    {
        Require(failure.Terminal.SceneSequence == canonicalSequence,
            "The failed terminal preserves sceneSequence when the DLST header is readable.");
    }
}

static byte[] MutateWithoutChecksum(byte[] source, Action<byte[]> mutation)
{
    var result = WithoutChecksum(source);
    mutation(result);
    return result;
}

static byte[] WithoutChecksum(byte[] source)
{
    var result = (byte[])source.Clone();
    BinaryPrimitives.WriteUInt32LittleEndian(result.AsSpan(12), 0);
    BinaryPrimitives.WriteUInt32LittleEndian(result.AsSpan(DisplayListFormat.ChecksumOffset), 0);
    return result;
}

static int CommandOffset(byte[] buffer) => checked(
    (int)DisplayListFormat.HeaderSize +
    (int)BinaryPrimitives.ReadUInt32LittleEndian(buffer.AsSpan(100)) +
    (int)BinaryPrimitives.ReadUInt32LittleEndian(buffer.AsSpan(92)));

static void ExpectException<TException>(Action action, string message)
    where TException : Exception
{
    try
    {
        action();
    }
    catch (TException)
    {
        return;
    }

    throw new InvalidOperationException(message);
}

static void Require([DoesNotReturnIf(false)] bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}

internal static class Fixtures
{
    private static readonly DisplayResourceReference Font = new(DisplayResourceKind.Font, 100, 1);
    private static readonly DisplayResourceReference FallbackFont = new(DisplayResourceKind.Font, 101, 3);
    private static readonly DisplayResourceReference Image = new(DisplayResourceKind.Image, 200, 2);
    private static readonly DisplayResourceReference Effect = new(DisplayResourceKind.RuntimeEffect, 300, 5);
    private static readonly DisplayResourceReference RetainedScene = new(DisplayResourceKind.RetainedScene, 400, 8);

    internal static DisplayListDocument Representative()
    {
        var path = new DisplayPath(
            DisplayPathFillType.EvenOdd,
            [DisplayPathVerb.MoveTo, DisplayPathVerb.LineTo, DisplayPathVerb.CubicTo, DisplayPathVerb.Close],
            [10, 10, 90, 10, 90, 20, 80, 90, 10, 90]);
        var roundedRect = new DisplayRoundedRect(new DisplayRect(5, 6, 205, 106), 4, 5, 6, 7, 8, 9, 10, 11);
        var matrix = new DisplayMatrix(
            [
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                3, 4, 0, 1,
            ]);
        var runtimeShader = new DisplayRuntimeEffectShader(Effect, [1, 2, 3, 4, 5, 6, 7, 8], [Image]);
        var paint = new DisplayPaint(
            0xFF123456,
            DisplayPaintStyle.Stroke,
            2.5f,
            7,
            DisplayStrokeCap.Round,
            DisplayStrokeJoin.Bevel,
            true,
            DisplayBlendMode.SourceOver,
            DisplaySamplingQuality.Medium,
            false,
            new DisplayLinearGradientShader(
                new DisplayPoint(0, 0),
                new DisplayPoint(100, 50),
                [0xFF000000, 0xFF00FF00, 0xFFFFFFFF],
                [0, 0.4f, 1],
                DisplayTileMode.Mirror,
                matrix),
            new DisplayMatrixColorFilter(
                [
                    1, 0, 0, 0, 0,
                    0, 1, 0, 0, 0,
                    0, 0, 1, 0, 0,
                    0, 0, 0, 1, 0,
                ]),
            new DisplayMaskFilter(DisplayBlurStyle.Normal, 2),
            new DisplayComposeImageFilter(
                new DisplayBlurImageFilter(1.5f, 2.5f, DisplayTileMode.Decal, new DisplayRect(0, 0, 300, 200)),
                new DisplayRuntimeEffectImageFilter(runtimeShader, DisplaySamplingQuality.High)));
        var paragraph = new DisplayParagraphRecipe(
            "Doroti 한글 😀 e\u0301",
            Font,
            "Doroti Sans",
            18,
            1.2f,
            0xFF112233,
            600,
            DisplayFontSlant.Normal,
            DisplayTextDirection.LeftToRight,
            DisplayTextAlign.Start,
            "ko-KR",
            2,
            "…",
            260,
            251.25f,
            44,
            0x1234_5678_9ABC_DEF0,
            [FallbackFont],
            [
                new DisplayParagraphTextRun(
                    "Doroti ", "Doroti Sans", "ko-KR", 18, 1.2f, 0xFF112233, 600,
                    DisplayFontSlant.Normal,
                    fontFamilyFallback: ["Doroti Fallback"],
                    letterSpacing: 0.25f,
                    fontFeatures: [new DisplayFontFeature("kern", 1)]),
                new DisplayParagraphTextRun(
                    "한글 😀 e\u0301", "Doroti Sans", "ko-KR", 20, 1.3f, 0xFF445566, 700,
                    DisplayFontSlant.Italic,
                    decoration: 1,
                    backgroundColor: 0x1100FF00,
                    decorationColor: 0xFFFF0000,
                    decorationStyle: DisplayTextDecorationStyle.Wavy,
                    decorationThickness: 1.5f,
                    textBaseline: DisplayTextBaseline.Alphabetic,
                    wordSpacing: 0.5f,
                    halfLeading: true,
                    shadows: [new DisplayTextShadow(0x88000000, 1, 2, 3)],
                    fontVariations: [new DisplayFontVariation("wght", 700)]),
            ]);

        DisplayListCommand[] commands =
        [
            new DisplaySaveCommand(),
            new DisplayTransformCommand(matrix),
            new DisplayClipRectCommand(new DisplayRect(0, 0, 640, 360)),
            new DisplayClipRoundedRectCommand(roundedRect, DisplayClipOperation.Intersect, true),
            new DisplayClipPathCommand(path, DisplayClipOperation.Difference, false),
            new DisplaySaveLayerCommand(new DisplayRect(0, 0, 640, 360), paint),
            new DisplayDrawColorCommand(0xFF202124, DisplayBlendMode.Source),
            new DisplayDrawPaintCommand(paint),
            new DisplayDrawLineCommand(new DisplayPoint(1, 2), new DisplayPoint(3, 4), paint),
            new DisplayDrawPointsCommand(DisplayPointMode.Polygon, [new(1, 1), new(2, 3), new(5, 8)], paint),
            new DisplayDrawRectCommand(new DisplayRect(1, 2, 20, 30), paint),
            new DisplayDrawRoundedRectCommand(roundedRect, paint),
            new DisplayDrawDoubleRoundedRectCommand(roundedRect, new DisplayRoundedRect(new DisplayRect(20, 20, 100, 80), 2, 2), paint),
            new DisplayDrawCircleCommand(new DisplayPoint(50, 60), 25, paint),
            new DisplayDrawOvalCommand(new DisplayRect(1, 2, 80, 40), paint),
            new DisplayDrawArcCommand(new DisplayRect(1, 2, 80, 40), 0.25f, 1.5f, true, paint),
            new DisplayDrawPathCommand(path, paint),
            new DisplayDrawShadowCommand(path, 0x80000000, 6, true),
            new DisplayDrawImageCommand(Image, new DisplayPoint(9, 10), DisplaySamplingQuality.Low, paint),
            new DisplayDrawImageRectCommand(Image, new DisplayRect(0, 0, 64, 64), new DisplayRect(10, 10, 74, 74), DisplaySamplingQuality.High, paint),
            new DisplayDrawNinePatchCommand(Image, new DisplayRect(8, 8, 56, 56), new DisplayRect(0, 0, 120, 90), DisplaySamplingQuality.Medium, paint),
            new DisplayDrawParagraphCommand(paragraph, new DisplayPoint(12, 34)),
            new DisplayPushOpacityCommand(0.75f, new DisplayPoint(2, 3)),
            new DisplayPushColorFilterCommand(new DisplayBlendColorFilter(0x80FF0000, DisplayBlendMode.Modulate), new DisplayPoint(4, 5)),
            new DisplayPushImageFilterCommand(
                new DisplayDropShadowImageFilter(2, 3, 4, 5, 0x88000000, false),
                new DisplayPoint(6, 7),
                new DisplayRect(8, 9, 108, 59)),
            new DisplayPushBackdropFilterCommand(new DisplayColorImageFilter(new DisplayLinearToSrgbColorFilter()), DisplayBlendMode.SourceOver, 77, new DisplayPoint(8, 9)),
            new DisplayPushShaderMaskCommand(
                new DisplayImageShader(Image, DisplayTileMode.Clamp, DisplayTileMode.Repeat, DisplaySamplingQuality.Low, matrix),
                new DisplayRect(0, 0, 50, 50),
                DisplayBlendMode.SourceIn),
            new DisplayDrawPaintCommand(new DisplayPaint(
                0xFF010203,
                Shader: new DisplayRadialGradientShader(
                    new DisplayPoint(20, 30),
                    40,
                    [0xFF102030, 0xFF405060],
                    [0, 1],
                    DisplayTileMode.Clamp,
                    new DisplayPoint(18, 28),
                    2))),
            new DisplayDrawPaintCommand(new DisplayPaint(
                0xFF040506,
                Shader: new DisplaySweepGradientShader(
                    new DisplayPoint(30, 40),
                    0.5f,
                    5.5f,
                    [0xFF112233, 0xFF445566],
                    [0, 1],
                    DisplayTileMode.Repeat),
                ColorFilter: new DisplaySrgbToLinearColorFilter(),
                ImageFilter: new DisplayMatrixImageFilter(matrix, DisplaySamplingQuality.Low))),
            new DisplayDrawPaintCommand(new DisplayPaint(0xFF070809, Shader: runtimeShader)),
            new DisplayDrawPaintCommand(new DisplayPaint(0xFF0A0B0C)),
            new DisplayDrawRetainedSceneCommand(RetainedScene, new DisplayPoint(11, 12), DisplayRetainedSceneCacheHint.IsComplex),
            new DisplayRestoreCommand(),
        ];

        return new DisplayListDocument(
            Scene(42),
            [
                Resource(RetainedScene, 8),
                Resource(Image, 2),
                Resource(FallbackFont, 3),
                Resource(Effect, 5),
                Resource(Font, 1),
            ],
            commands,
            DisplayListFlags.ChecksumPresent | DisplayListFlags.DiagnosticCapture);
    }

    internal static DisplayListDocument OneImage()
    {
        var paint = new DisplayPaint(0xFFFFFFFF);
        return new DisplayListDocument(
            Scene(7),
            [Resource(new DisplayResourceReference(DisplayResourceKind.Image, 1, 1), 1)],
            [new DisplayDrawImageCommand(new(DisplayResourceKind.Image, 1, 1), new(0, 0), DisplaySamplingQuality.None, paint)]);
    }

    internal static DisplayListDocument TwoImages() => new(
        Scene(8),
        [
            Resource(new DisplayResourceReference(DisplayResourceKind.Image, 2, 1), 2),
            Resource(new DisplayResourceReference(DisplayResourceKind.Image, 1, 1), 1),
        ],
        []);

    internal static DisplayListDocument ThreeStrings()
    {
        var font = new DisplayResourceReference(DisplayResourceKind.Font, 1, 1);
        var paragraph = new DisplayParagraphRecipe(
            "a", font, "b", 12, 1.2f, 0xFF000000, 400,
            DisplayFontSlant.Normal, DisplayTextDirection.LeftToRight, DisplayTextAlign.Start,
            "c", 1, null, 10, 10, 10, 1);
        return new DisplayListDocument(
            Scene(9),
            [Resource(font, 1)],
            [new DisplayDrawParagraphCommand(paragraph, new DisplayPoint(0, 0))]);
    }

    internal static DisplayListDocument WithMissingImageResource() => new(
        Scene(10),
        [],
        [new DisplayDrawImageCommand(new(DisplayResourceKind.Image, 99, 1), new(0, 0), DisplaySamplingQuality.None, new(0xFFFFFFFF))]);

    internal static DisplayListDocument WithDuplicateResource()
    {
        var resource = Resource(new DisplayResourceReference(DisplayResourceKind.Image, 1, 1), 1);
        return new DisplayListDocument(Scene(11), [resource, resource], []);
    }

    internal static DisplayListDocument WithInvalidOpacity() => new(
        Scene(12),
        [],
        [new DisplayPushOpacityCommand(1.5f, new DisplayPoint(0, 0))]);

    internal static DisplayListDocument WithInvalidParagraphHeightMultiplier()
    {
        var font = new DisplayResourceReference(DisplayResourceKind.Font, 1, 1);
        var paragraph = new DisplayParagraphRecipe(
            "invalid", font, "Doroti Sans", 14, 0, 0xFF000000, 400,
            DisplayFontSlant.Normal, DisplayTextDirection.LeftToRight, DisplayTextAlign.Start,
            "en-US", 0, null, 10, 10, 10, 1);
        return new DisplayListDocument(
            Scene(13),
            [Resource(font, 1)],
            [new DisplayDrawParagraphCommand(paragraph, new DisplayPoint(0, 0))]);
    }

    private static DisplayResourceDescriptor Resource(DisplayResourceReference reference, ulong seed) =>
        new(reference, new DisplayResourceFingerprint(seed * 101, seed * 1009));

    private static DisplayListSceneMetadata Scene(ulong sequence) => new(
        5,
        sequence,
        sequence + 1000,
        4,
        3,
        2,
        640,
        360,
        1280,
        720,
        2);
}
