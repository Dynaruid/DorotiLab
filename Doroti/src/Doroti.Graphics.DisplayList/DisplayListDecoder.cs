using System.Buffers.Binary;
using System.Text;

namespace Doroti.Graphics.DisplayList;

public static class DisplayListDecoder
{
    private const DisplayListFlags KnownFlags =
        DisplayListFlags.ChecksumPresent |
        DisplayListFlags.DiagnosticCapture;
    private const DisplayResourceFlags KnownResourceFlags = DisplayResourceFlags.Recoverable;

    public static DisplayListDecodeResult Decode(ReadOnlySpan<byte> buffer)
    {
        var sceneSequence = TryReadSceneSequence(buffer);
        try
        {
            var reader = new DisplayListBinaryReader(buffer);
            if (buffer.Length < DisplayListFormat.HeaderSize)
            {
                throw new DisplayListFormatException(
                    DisplayListFailureCode.BufferTooShort,
                    0,
                    $"A DisplayList v2 header requires {DisplayListFormat.HeaderSize} bytes.");
            }

            var magic = reader.ReadUInt32();
            if (magic != DisplayListFormat.Magic)
            {
                throw reader.Error(DisplayListFailureCode.InvalidMagic, 0, "The DisplayList magic is not 'DLST'.");
            }

            var schemaVersion = reader.ReadUInt16();
            if (schemaVersion != DisplayListFormat.SchemaVersion)
            {
                throw reader.Error(
                    DisplayListFailureCode.UnsupportedVersion,
                    4,
                    $"DisplayList schema version {schemaVersion} is unsupported; expected {DisplayListFormat.SchemaVersion}.");
            }

            var headerSize = reader.ReadUInt16();
            if (headerSize != DisplayListFormat.HeaderSize)
            {
                throw reader.Error(
                    DisplayListFailureCode.InvalidHeader,
                    6,
                    $"DisplayList header size {headerSize} is not the canonical v1 size {DisplayListFormat.HeaderSize}.");
            }

            var byteLength = reader.ReadUInt32();
            var flags = (DisplayListFlags)reader.ReadUInt32();
            if ((flags & ~KnownFlags) != 0)
            {
                throw reader.Error(DisplayListFailureCode.UnknownFlags, 12, "The DisplayList header contains unknown flag bits.");
            }

            var scene = ReadScene(ref reader);
            sceneSequence = scene.SceneSequence;
            ValidateScene(scene, ref reader);
            var commandCount = reader.ReadUInt32();
            var resourceCount = reader.ReadUInt32();
            var stringTableByteLength = reader.ReadUInt32();
            var commandByteLength = reader.ReadUInt32();
            var resourceTableByteLength = reader.ReadUInt32();
            var checksum = reader.ReadUInt32();
            var reserved = reader.ReadUInt32();
            if (reserved != 0)
            {
                throw reader.Error(DisplayListFailureCode.NonCanonicalEncoding, 108, "The DisplayList header reserved field must be zero.");
            }

            ValidateHeaderLengths(
                buffer.Length,
                byteLength,
                commandCount,
                resourceCount,
                stringTableByteLength,
                commandByteLength,
                resourceTableByteLength,
                ref reader);
            ValidateChecksum(buffer, flags, checksum, ref reader);

            var resourceReader = reader.ReadSubReader(checked((int)resourceTableByteLength));
            var resources = ReadResources(ref resourceReader, resourceCount);
            resourceReader.RequireFinished(DisplayListFailureCode.InvalidResource, "The resource table contains trailing bytes.");
            var resourceCatalog = resources.ToDictionary(resource => resource.Reference);

            var stringReader = reader.ReadSubReader(checked((int)stringTableByteLength));
            var strings = ReadStrings(ref stringReader);
            stringReader.RequireFinished(DisplayListFailureCode.InvalidString, "The string table contains trailing bytes.");

            var commandReader = reader.ReadSubReader(checked((int)commandByteLength));
            var context = new DecoderContext(resourceCatalog, strings);
            var commands = ReadCommands(ref commandReader, commandCount, context);
            commandReader.RequireFinished(DisplayListFailureCode.InvalidCommand, "The command table contains trailing bytes.");
            reader.RequireFinished(DisplayListFailureCode.LengthMismatch, "The DisplayList contains bytes beyond its declared sections.");

            var header = new DisplayListWireHeader(
                byteLength,
                flags,
                scene,
                commandCount,
                resourceCount,
                stringTableByteLength,
                commandByteLength,
                resourceTableByteLength,
                checksum);
            var document = new DisplayListDocument(scene, resources, commands, flags);
            return DisplayListDecodeResult.Success(document, header);
        }
        catch (DisplayListFormatException exception)
        {
            return Failed(sceneSequence, exception.Code, exception.Offset, exception.Message);
        }
        catch (Exception exception) when (exception is
            ArgumentException or
            InvalidOperationException or
            OverflowException or
            DecoderFallbackException)
        {
            return Failed(
                sceneSequence,
                DisplayListFailureCode.InvalidValue,
                0,
                $"The DisplayList could not be decoded: {exception.Message}");
        }
    }

    public static bool TryDecode(
        ReadOnlySpan<byte> buffer,
        out DisplayListDocument? document,
        out DisplayListFailure? failure)
    {
        var result = Decode(buffer);
        document = result.Document;
        failure = result.Failure;
        return result.IsSuccess;
    }

    private static DisplayListDecodeResult Failed(
        ulong sceneSequence,
        DisplayListFailureCode code,
        int offset,
        string message)
    {
        var terminal = DisplayListSceneTerminal.Failed(sceneSequence, code, message);
        return DisplayListDecodeResult.Failed(new DisplayListFailure(code, offset, message, terminal));
    }

    private static ulong TryReadSceneSequence(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < 32 || BinaryPrimitives.ReadUInt32LittleEndian(buffer) != DisplayListFormat.Magic)
        {
            return 0;
        }

        return BinaryPrimitives.ReadUInt64LittleEndian(buffer[24..]);
    }

    private static DisplayListSceneMetadata ReadScene(ref DisplayListBinaryReader reader) =>
        new(
            reader.ReadUInt64(),
            reader.ReadUInt64(),
            reader.ReadUInt64(),
            reader.ReadUInt64(),
            reader.ReadUInt64(),
            reader.ReadUInt64(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadUInt32(),
            reader.ReadUInt32(),
            reader.ReadSingle());

    private static void ValidateScene(
        DisplayListSceneMetadata scene,
        ref DisplayListBinaryReader reader)
    {
        if (scene.ViewId == 0 || scene.SceneSequence == 0 || scene.BuildToken == 0 ||
            scene.ResizeEpoch == 0 || scene.SurfaceGeneration == 0 || scene.ContextGeneration == 0)
        {
            throw reader.Error(
                DisplayListFailureCode.InvalidHeader,
                16,
                "DisplayList scene identities and generations must be nonzero.");
        }

        if (scene.LogicalWidth <= 0 || scene.LogicalHeight <= 0 ||
            scene.PhysicalWidth == 0 || scene.PhysicalHeight == 0 || scene.DevicePixelRatio <= 0)
        {
            throw reader.Error(
                DisplayListFailureCode.InvalidHeader,
                64,
                "DisplayList scene geometry must be finite and positive.");
        }
    }

    private static void ValidateHeaderLengths(
        int actualLength,
        uint byteLength,
        uint commandCount,
        uint resourceCount,
        uint stringTableByteLength,
        uint commandByteLength,
        uint resourceTableByteLength,
        ref DisplayListBinaryReader reader)
    {
        if (byteLength != actualLength)
        {
            throw reader.Error(
                DisplayListFailureCode.LengthMismatch,
                8,
                $"DisplayList byteLength {byteLength} does not match the received length {actualLength}.");
        }

        if (byteLength > DisplayListFormat.MaximumByteLength)
        {
            throw reader.Error(DisplayListFailureCode.LimitExceeded, 8, "The DisplayList byte limit was exceeded.");
        }

        if (commandCount > DisplayListFormat.MaximumCommandCount)
        {
            throw reader.Error(DisplayListFailureCode.LimitExceeded, 84, "The DisplayList command limit was exceeded.");
        }

        if (resourceCount > DisplayListFormat.MaximumResourceCount)
        {
            throw reader.Error(DisplayListFailureCode.LimitExceeded, 88, "The DisplayList resource limit was exceeded.");
        }

        if (stringTableByteLength > DisplayListFormat.MaximumStringTableByteLength)
        {
            throw reader.Error(DisplayListFailureCode.LimitExceeded, 92, "The DisplayList string-table limit was exceeded.");
        }

        var expectedResourceBytes = (ulong)resourceCount * DisplayListFormat.ResourceEntrySize;
        if (resourceTableByteLength != expectedResourceBytes)
        {
            throw reader.Error(
                DisplayListFailureCode.InvalidHeader,
                100,
                "The DisplayList resource byte length does not match its fixed-width entry count.");
        }

        if ((ulong)commandCount * DisplayListFormat.CommandEnvelopeSize > commandByteLength)
        {
            throw reader.Error(
                DisplayListFailureCode.InvalidHeader,
                96,
                "The DisplayList command byte length cannot contain its declared command envelopes.");
        }

        var expectedLength = (ulong)DisplayListFormat.HeaderSize +
            resourceTableByteLength +
            stringTableByteLength +
            commandByteLength;
        if (expectedLength != byteLength)
        {
            throw reader.Error(
                DisplayListFailureCode.LengthMismatch,
                8,
                "The DisplayList section lengths do not add up to byteLength.");
        }
    }

    private static void ValidateChecksum(
        ReadOnlySpan<byte> buffer,
        DisplayListFlags flags,
        uint checksum,
        ref DisplayListBinaryReader reader)
    {
        if ((flags & DisplayListFlags.ChecksumPresent) == 0)
        {
            if (checksum != 0)
            {
                throw reader.Error(
                    DisplayListFailureCode.NonCanonicalEncoding,
                    DisplayListFormat.ChecksumOffset,
                    "A DisplayList without the checksum flag must store zero in the checksum field.");
            }

            return;
        }

        var actual = DisplayListChecksum.Compute(buffer);
        if (actual != checksum)
        {
            throw reader.Error(
                DisplayListFailureCode.ChecksumMismatch,
                DisplayListFormat.ChecksumOffset,
                $"DisplayList checksum mismatch: expected 0x{checksum:X8}, calculated 0x{actual:X8}.");
        }
    }

    private static List<DisplayResourceDescriptor> ReadResources(
        ref DisplayListBinaryReader reader,
        uint count)
    {
        var result = new List<DisplayResourceDescriptor>(checked((int)count));
        DisplayResourceReference? previous = null;
        for (var index = 0u; index < count; index++)
        {
            var offset = reader.AbsoluteOffset;
            var reference = new DisplayResourceReference(
                ReadResourceKind(ref reader),
                0,
                0);
            var flags = (DisplayResourceFlags)reader.ReadUInt16();
            reference = reference with
            {
                Version = reader.ReadUInt32(),
                Id = reader.ReadUInt64(),
            };
            var fingerprint = new DisplayResourceFingerprint(reader.ReadUInt64(), reader.ReadUInt64());
            ValidateResourceReference(reference, null, offset, ref reader);
            if ((flags & ~KnownResourceFlags) != 0)
            {
                throw reader.Error(DisplayListFailureCode.InvalidResource, offset + 2, "A resource contains unknown flag bits.");
            }

            if (previous is { } previousReference)
            {
                var comparison = CompareResource(previousReference, reference);
                if (comparison == 0)
                {
                    throw reader.Error(DisplayListFailureCode.DuplicateResource, offset, "A resource is declared more than once.");
                }

                if (comparison > 0)
                {
                    throw reader.Error(
                        DisplayListFailureCode.NonCanonicalEncoding,
                        offset,
                        "Resource entries must be sorted by kind, id, and version.");
                }
            }

            previous = reference;
            result.Add(new DisplayResourceDescriptor(reference, fingerprint, flags));
        }

        return result;
    }

    private static int CompareResource(DisplayResourceReference left, DisplayResourceReference right)
    {
        var comparison = left.Kind.CompareTo(right.Kind);
        if (comparison != 0)
        {
            return comparison;
        }

        comparison = left.Id.CompareTo(right.Id);
        return comparison != 0 ? comparison : left.Version.CompareTo(right.Version);
    }

    private static List<string> ReadStrings(ref DisplayListBinaryReader reader)
    {
        var result = new List<string>();
        byte[]? previous = null;
        while (reader.Remaining != 0)
        {
            if (result.Count >= DisplayListFormat.MaximumCollectionCount)
            {
                throw reader.Error(DisplayListFailureCode.LimitExceeded, reader.AbsoluteOffset, "The DisplayList string count limit was exceeded.");
            }

            var lengthOffset = reader.AbsoluteOffset;
            var length = reader.ReadUInt32();
            if (length > reader.Remaining)
            {
                throw reader.Error(DisplayListFailureCode.BoundsExceeded, lengthOffset, "A string extends beyond the string-table bounds.");
            }

            var bytes = reader.ReadBytes(checked((int)length));
            if (previous is not null && DisplayListUtf8.Compare(previous, bytes) >= 0)
            {
                throw reader.Error(
                    DisplayListFailureCode.NonCanonicalEncoding,
                    lengthOffset,
                    "DisplayList strings must be unique and strictly sorted by UTF-8 bytes.");
            }

            string value;
            try
            {
                value = DisplayListUtf8.StrictEncoding.GetString(bytes);
            }
            catch (DecoderFallbackException exception)
            {
                throw new DisplayListFormatException(
                    DisplayListFailureCode.InvalidString,
                    lengthOffset + sizeof(uint),
                    $"The DisplayList string is not valid UTF-8: {exception.Message}");
            }

            previous = bytes.ToArray();
            result.Add(value);
        }

        return result;
    }

    private static List<DisplayListCommand> ReadCommands(
        ref DisplayListBinaryReader reader,
        uint count,
        DecoderContext context)
    {
        var result = new List<DisplayListCommand>(checked((int)count));
        for (var index = 0u; index < count; index++)
        {
            var envelopeOffset = reader.AbsoluteOffset;
            var rawOpcode = reader.ReadUInt16();
            if (!Enum.IsDefined((DisplayListOpcode)rawOpcode))
            {
                throw reader.Error(
                    DisplayListFailureCode.UnknownOpcode,
                    envelopeOffset,
                    $"DisplayList opcode {rawOpcode} is unknown in schema v2.");
            }

            var commandFlags = reader.ReadUInt16();
            if (commandFlags != 0)
            {
                throw reader.Error(
                    DisplayListFailureCode.NonCanonicalEncoding,
                    envelopeOffset + 2,
                    "DisplayList v2 command flags must be zero.");
            }

            var payloadLength = reader.ReadUInt32();
            if (payloadLength > reader.Remaining)
            {
                throw reader.Error(
                    DisplayListFailureCode.BoundsExceeded,
                    envelopeOffset + 4,
                    "A command payload extends beyond the command section.");
            }

            var payloadReader = reader.ReadSubReader(checked((int)payloadLength));
            var command = ReadCommandPayload(ref payloadReader, (DisplayListOpcode)rawOpcode, context);
            payloadReader.RequireFinished(
                DisplayListFailureCode.InvalidCommand,
                $"Command {(DisplayListOpcode)rawOpcode} did not consume its exact payload length.");
            result.Add(command);
        }

        return result;
    }

    private static DisplayListCommand ReadCommandPayload(
        ref DisplayListBinaryReader reader,
        DisplayListOpcode opcode,
        DecoderContext context) => opcode switch
        {
            DisplayListOpcode.Save => new DisplaySaveCommand(),
            DisplayListOpcode.Restore => new DisplayRestoreCommand(),
            DisplayListOpcode.SaveLayer => new DisplaySaveLayerCommand(
                ReadOptionalRect(ref reader),
                ReadOptionalPaint(ref reader, context, 0)),
            DisplayListOpcode.Transform => new DisplayTransformCommand(ReadMatrix(ref reader)),
            DisplayListOpcode.ClipRect => ReadClipRect(ref reader),
            DisplayListOpcode.ClipRoundedRect => ReadClipRoundedRect(ref reader),
            DisplayListOpcode.ClipPath => ReadClipPath(ref reader),
            DisplayListOpcode.DrawColor => new DisplayDrawColorCommand(
                reader.ReadUInt32(),
                ReadEnumByte<DisplayBlendMode>(ref reader, "blend mode")),
            DisplayListOpcode.DrawPaint => new DisplayDrawPaintCommand(ReadPaint(ref reader, context, 0)),
            DisplayListOpcode.DrawLine => new DisplayDrawLineCommand(
                ReadPoint(ref reader),
                ReadPoint(ref reader),
                ReadPaint(ref reader, context, 0)),
            DisplayListOpcode.DrawPoints => ReadDrawPoints(ref reader, context),
            DisplayListOpcode.DrawRect => new DisplayDrawRectCommand(
                ReadRect(ref reader),
                ReadPaint(ref reader, context, 0)),
            DisplayListOpcode.DrawRoundedRect => new DisplayDrawRoundedRectCommand(
                ReadRoundedRect(ref reader),
                ReadPaint(ref reader, context, 0)),
            DisplayListOpcode.DrawDoubleRoundedRect => new DisplayDrawDoubleRoundedRectCommand(
                ReadRoundedRect(ref reader),
                ReadRoundedRect(ref reader),
                ReadPaint(ref reader, context, 0)),
            DisplayListOpcode.DrawCircle => new DisplayDrawCircleCommand(
                ReadPoint(ref reader),
                ReadNonnegativeSingle(ref reader, "circle radius"),
                ReadPaint(ref reader, context, 0)),
            DisplayListOpcode.DrawOval => new DisplayDrawOvalCommand(
                ReadRect(ref reader),
                ReadPaint(ref reader, context, 0)),
            DisplayListOpcode.DrawArc => new DisplayDrawArcCommand(
                ReadRect(ref reader),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadBoolean(),
                ReadPaint(ref reader, context, 0)),
            DisplayListOpcode.DrawPath => new DisplayDrawPathCommand(
                ReadPath(ref reader),
                ReadPaint(ref reader, context, 0)),
            DisplayListOpcode.DrawShadow => new DisplayDrawShadowCommand(
                ReadPath(ref reader),
                reader.ReadUInt32(),
                ReadNonnegativeSingle(ref reader, "shadow elevation"),
                reader.ReadBoolean()),
            DisplayListOpcode.DrawImage => new DisplayDrawImageCommand(
                context.ReadResource(ref reader, DisplayResourceKind.Image),
                ReadPoint(ref reader),
                ReadEnumByte<DisplaySamplingQuality>(ref reader, "sampling quality"),
                ReadPaint(ref reader, context, 0)),
            DisplayListOpcode.DrawImageRect => new DisplayDrawImageRectCommand(
                context.ReadResource(ref reader, DisplayResourceKind.Image),
                ReadRect(ref reader),
                ReadRect(ref reader),
                ReadEnumByte<DisplaySamplingQuality>(ref reader, "sampling quality"),
                ReadPaint(ref reader, context, 0)),
            DisplayListOpcode.DrawNinePatch => new DisplayDrawNinePatchCommand(
                context.ReadResource(ref reader, DisplayResourceKind.Image),
                ReadRect(ref reader),
                ReadRect(ref reader),
                ReadEnumByte<DisplaySamplingQuality>(ref reader, "sampling quality"),
                ReadPaint(ref reader, context, 0)),
            DisplayListOpcode.DrawParagraph => new DisplayDrawParagraphCommand(
                ReadParagraph(ref reader, context),
                ReadPoint(ref reader)),
            DisplayListOpcode.PushOpacity => ReadPushOpacity(ref reader),
            DisplayListOpcode.PushColorFilter => new DisplayPushColorFilterCommand(
                ReadRequiredColorFilter(ref reader, context, 0),
                ReadPoint(ref reader)),
            DisplayListOpcode.PushImageFilter => new DisplayPushImageFilterCommand(
                ReadRequiredImageFilter(ref reader, context, 0),
                ReadPoint(ref reader),
                ReadOptionalRect(ref reader)),
            DisplayListOpcode.PushBackdropFilter => new DisplayPushBackdropFilterCommand(
                ReadRequiredImageFilter(ref reader, context, 0),
                ReadEnumByte<DisplayBlendMode>(ref reader, "blend mode"),
                reader.ReadUInt64(),
                ReadPoint(ref reader)),
            DisplayListOpcode.PushShaderMask => new DisplayPushShaderMaskCommand(
                ReadRequiredShader(ref reader, context, 0),
                ReadRect(ref reader),
                ReadEnumByte<DisplayBlendMode>(ref reader, "blend mode")),
            DisplayListOpcode.DrawRetainedScene => ReadRetainedScene(ref reader, context),
            _ => throw reader.Error(DisplayListFailureCode.UnknownOpcode, reader.AbsoluteOffset, $"DisplayList opcode {opcode} is unknown."),
        };

    private static DisplayClipRectCommand ReadClipRect(ref DisplayListBinaryReader reader)
    {
        var result = new DisplayClipRectCommand(
            ReadRect(ref reader),
            ReadEnumByte<DisplayClipOperation>(ref reader, "clip operation"),
            reader.ReadBoolean());
        RequireZero(reader.ReadUInt16(), reader.AbsoluteOffset - 2, "clip-rect reserved field", ref reader);
        return result;
    }

    private static DisplayClipRoundedRectCommand ReadClipRoundedRect(ref DisplayListBinaryReader reader)
    {
        var result = new DisplayClipRoundedRectCommand(
            ReadRoundedRect(ref reader),
            ReadEnumByte<DisplayClipOperation>(ref reader, "clip operation"),
            reader.ReadBoolean());
        RequireZero(reader.ReadUInt16(), reader.AbsoluteOffset - 2, "clip-rounded-rect reserved field", ref reader);
        return result;
    }

    private static DisplayClipPathCommand ReadClipPath(ref DisplayListBinaryReader reader)
    {
        var result = new DisplayClipPathCommand(
            ReadPath(ref reader),
            ReadEnumByte<DisplayClipOperation>(ref reader, "clip operation"),
            reader.ReadBoolean());
        RequireZero(reader.ReadUInt16(), reader.AbsoluteOffset - 2, "clip-path reserved field", ref reader);
        return result;
    }

    private static DisplayDrawPointsCommand ReadDrawPoints(
        ref DisplayListBinaryReader reader,
        DecoderContext context)
    {
        var mode = ReadEnumByte<DisplayPointMode>(ref reader, "point mode");
        var count = ReadCount(ref reader, "point");
        if ((long)count * 8 > reader.Remaining)
        {
            throw reader.Error(DisplayListFailureCode.BoundsExceeded, reader.AbsoluteOffset, "The point array exceeds the command payload.");
        }

        var points = new DisplayPoint[count];
        for (var index = 0; index < count; index++)
        {
            points[index] = ReadPoint(ref reader);
        }

        return new DisplayDrawPointsCommand(mode, points, ReadPaint(ref reader, context, 0));
    }

    private static DisplayPushOpacityCommand ReadPushOpacity(ref DisplayListBinaryReader reader)
    {
        var offset = reader.AbsoluteOffset;
        var opacity = reader.ReadSingle();
        if (opacity is < 0 or > 1)
        {
            throw reader.Error(DisplayListFailureCode.InvalidValue, offset, "Opacity must be between zero and one.");
        }

        return new DisplayPushOpacityCommand(opacity, ReadPoint(ref reader));
    }

    private static DisplayDrawRetainedSceneCommand ReadRetainedScene(
        ref DisplayListBinaryReader reader,
        DecoderContext context)
    {
        var scene = context.ReadResource(ref reader, DisplayResourceKind.RetainedScene);
        var offset = ReadPoint(ref reader);
        var hintOffset = reader.AbsoluteOffset;
        var cacheHint = (DisplayRetainedSceneCacheHint)reader.ReadByte();
        if ((cacheHint & ~(DisplayRetainedSceneCacheHint.IsComplex | DisplayRetainedSceneCacheHint.WillChange)) != 0)
        {
            throw reader.Error(DisplayListFailureCode.InvalidValue, hintOffset, "The retained-scene cache hint contains unknown bits.");
        }

        return new DisplayDrawRetainedSceneCommand(scene, offset, cacheHint);
    }

    private static DisplayParagraphRecipe ReadParagraph(
        ref DisplayListBinaryReader reader,
        DecoderContext context)
    {
        var text = context.ReadString(ref reader, false)!;
        var font = context.ReadResource(ref reader, DisplayResourceKind.Font);
        var family = context.ReadString(ref reader, false)!;
        var locale = context.ReadString(ref reader, false)!;
        var ellipsis = context.ReadString(ref reader, true);
        var fontSize = ReadPositiveSingle(ref reader, "font size");
        var heightMultiplier = ReadPositiveSingle(ref reader, "height multiplier");
        var color = reader.ReadUInt32();
        var weightOffset = reader.AbsoluteOffset;
        var fontWeight = reader.ReadInt32();
        if (fontWeight is < 1 or > 1000)
        {
            throw reader.Error(DisplayListFailureCode.InvalidValue, weightOffset, "Font weight must be between 1 and 1000.");
        }

        var slant = ReadEnumByte<DisplayFontSlant>(ref reader, "font slant");
        var direction = ReadEnumByte<DisplayTextDirection>(ref reader, "text direction");
        var align = ReadEnumByte<DisplayTextAlign>(ref reader, "text alignment");
        RequireZero(reader.ReadByte(), reader.AbsoluteOffset - 1, "paragraph reserved field", ref reader);
        var maxLines = reader.ReadUInt32();
        var layoutWidth = ReadNonnegativeSingle(ref reader, "paragraph layout width");
        var measuredWidth = ReadNonnegativeSingle(ref reader, "paragraph measured width");
        var measuredHeight = ReadNonnegativeSingle(ref reader, "paragraph measured height");
        var metricsHash = reader.ReadUInt64();
        var fallbackCount = ReadCount(ref reader, "fallback font");
        if ((long)fallbackCount * 16 > reader.Remaining)
        {
            throw reader.Error(DisplayListFailureCode.BoundsExceeded, reader.AbsoluteOffset, "The fallback-font array exceeds the command payload.");
        }

        var fallbackFonts = new DisplayResourceReference[fallbackCount];
        for (var index = 0; index < fallbackCount; index++)
        {
            fallbackFonts[index] = context.ReadResource(ref reader, DisplayResourceKind.Font);
        }

        var runCount = ReadCount(ref reader, "paragraph text run");
        var textRuns = new DisplayParagraphTextRun[runCount];
        for (var index = 0; index < runCount; index++)
        {
            var runText = context.ReadString(ref reader, false)!;
            if (runText.Length == 0)
                throw reader.Error(DisplayListFailureCode.InvalidValue, reader.AbsoluteOffset,
                    "Paragraph text runs must be nonempty.");
            var runFamily = context.ReadString(ref reader, false)!;
            var runLocale = context.ReadString(ref reader, false)!;
            var runFontSize = ReadPositiveSingle(ref reader, "run font size");
            var runHeight = ReadPositiveSingle(ref reader, "run height multiplier");
            var runColor = reader.ReadUInt32();
            var runWeightOffset = reader.AbsoluteOffset;
            var runWeight = reader.ReadInt32();
            if (runWeight is < 1 or > 1000)
                throw reader.Error(DisplayListFailureCode.InvalidValue, runWeightOffset,
                    "Run font weight must be between 1 and 1000.");
            var runSlant = ReadEnumByte<DisplayFontSlant>(ref reader, "run font slant");
            var decorationOffset = reader.AbsoluteOffset;
            var decoration = reader.ReadUInt32();
            if ((decoration & ~7u) != 0)
                throw reader.Error(DisplayListFailureCode.InvalidValue, decorationOffset,
                    "Run decoration contains unknown bits.");
            uint? backgroundColor = reader.ReadBoolean() ? reader.ReadUInt32() : null;
            uint? decorationColor = reader.ReadBoolean() ? reader.ReadUInt32() : null;
            DisplayTextDecorationStyle? decorationStyle = reader.ReadBoolean()
                ? ReadEnumByte<DisplayTextDecorationStyle>(ref reader, "run decoration style")
                : null;
            float? decorationThickness = reader.ReadBoolean()
                ? ReadNonnegativeSingle(ref reader, "run decoration thickness")
                : null;
            DisplayTextBaseline? textBaseline = reader.ReadBoolean()
                ? ReadEnumByte<DisplayTextBaseline>(ref reader, "run text baseline")
                : null;
            float? letterSpacing = reader.ReadBoolean() ? reader.ReadSingle() : null;
            float? wordSpacing = reader.ReadBoolean() ? reader.ReadSingle() : null;
            bool? halfLeading = reader.ReadByte() switch
            {
                0 => null,
                1 => false,
                2 => true,
                var value => throw reader.Error(DisplayListFailureCode.InvalidValue,
                    reader.AbsoluteOffset - 1, $"Run half-leading state {value} is invalid."),
            };
            var fallbackFamilyCount = ReadCount(ref reader, "run fallback font family");
            var fallbackFamilies = new string[fallbackFamilyCount];
            for (var fallbackIndex = 0; fallbackIndex < fallbackFamilyCount; fallbackIndex++)
                fallbackFamilies[fallbackIndex] = context.ReadString(ref reader, false)!;
            var shadowCount = ReadCount(ref reader, "run shadow");
            var shadows = new DisplayTextShadow[shadowCount];
            for (var shadowIndex = 0; shadowIndex < shadowCount; shadowIndex++)
            {
                shadows[shadowIndex] = new DisplayTextShadow(
                    reader.ReadUInt32(),
                    reader.ReadSingle(),
                    reader.ReadSingle(),
                    ReadNonnegativeSingle(ref reader, "run shadow blur radius"));
            }
            var featureCount = ReadCount(ref reader, "run font feature");
            var features = new DisplayFontFeature[featureCount];
            for (var featureIndex = 0; featureIndex < featureCount; featureIndex++)
                features[featureIndex] = new DisplayFontFeature(
                    context.ReadString(ref reader, false)!, reader.ReadInt32());
            var variationCount = ReadCount(ref reader, "run font variation");
            var variations = new DisplayFontVariation[variationCount];
            for (var variationIndex = 0; variationIndex < variationCount; variationIndex++)
                variations[variationIndex] = new DisplayFontVariation(
                    context.ReadString(ref reader, false)!, reader.ReadSingle());

            textRuns[index] = new DisplayParagraphTextRun(
                runText,
                runFamily,
                runLocale,
                runFontSize,
                runHeight,
                runColor,
                runWeight,
                runSlant,
                decoration,
                backgroundColor,
                decorationColor,
                decorationStyle,
                decorationThickness,
                textBaseline,
                letterSpacing,
                wordSpacing,
                halfLeading,
                fallbackFamilies,
                shadows,
                features,
                variations);
        }
        if (textRuns.Length != 0 &&
            !string.Equals(string.Concat(textRuns.Select(run => run.Text)), text, StringComparison.Ordinal))
            throw reader.Error(DisplayListFailureCode.InvalidValue, reader.AbsoluteOffset,
                "Paragraph text runs do not concatenate to paragraph text.");

        return new DisplayParagraphRecipe(
            text,
            font,
            family,
            fontSize,
            heightMultiplier,
            color,
            fontWeight,
            slant,
            direction,
            align,
            locale,
            maxLines,
            ellipsis,
            layoutWidth,
            measuredWidth,
            measuredHeight,
            metricsHash,
            fallbackFonts,
            textRuns);
    }

    private static DisplayPaint? ReadOptionalPaint(
        ref DisplayListBinaryReader reader,
        DecoderContext context,
        int depth) =>
        reader.ReadBoolean() ? ReadPaint(ref reader, context, depth) : null;

    private static DisplayPaint ReadPaint(
        ref DisplayListBinaryReader reader,
        DecoderContext context,
        int depth)
    {
        RequireDepth(depth, ref reader);
        var color = reader.ReadUInt32();
        var style = ReadEnumByte<DisplayPaintStyle>(ref reader, "paint style");
        var cap = ReadEnumByte<DisplayStrokeCap>(ref reader, "stroke cap");
        var join = ReadEnumByte<DisplayStrokeJoin>(ref reader, "stroke join");
        var antialias = reader.ReadBoolean();
        var blend = ReadEnumByte<DisplayBlendMode>(ref reader, "blend mode");
        var sampling = ReadEnumByte<DisplaySamplingQuality>(ref reader, "sampling quality");
        var invert = reader.ReadBoolean();
        RequireZero(reader.ReadByte(), reader.AbsoluteOffset - 1, "paint reserved field", ref reader);
        var strokeWidth = ReadNonnegativeSingle(ref reader, "stroke width");
        var strokeMiter = ReadNonnegativeSingle(ref reader, "stroke miter limit");
        var shader = ReadShader(ref reader, context, depth + 1, true);
        var colorFilter = ReadColorFilter(ref reader, context, depth + 1, true);
        var maskFilter = ReadMaskFilter(ref reader);
        var imageFilter = ReadImageFilter(ref reader, context, depth + 1, true);
        return new DisplayPaint(
            color,
            style,
            strokeWidth,
            strokeMiter,
            cap,
            join,
            antialias,
            blend,
            sampling,
            invert,
            shader,
            colorFilter,
            maskFilter,
            imageFilter);
    }

    private static DisplayShader ReadRequiredShader(
        ref DisplayListBinaryReader reader,
        DecoderContext context,
        int depth) =>
        ReadShader(ref reader, context, depth, false)!;

    private static DisplayShader? ReadShader(
        ref DisplayListBinaryReader reader,
        DecoderContext context,
        int depth,
        bool allowNull)
    {
        RequireDepth(depth, ref reader);
        var tagOffset = reader.AbsoluteOffset;
        var tag = reader.ReadByte();
        return tag switch
        {
            0 when allowNull => null,
            0 => throw reader.Error(DisplayListFailureCode.InvalidValue, tagOffset, "A required shader cannot use the null tag."),
            1 => ReadLinearGradient(ref reader),
            2 => ReadRadialGradient(ref reader),
            3 => ReadSweepGradient(ref reader),
            4 => ReadImageShader(ref reader, context),
            5 => ReadRuntimeEffectShader(ref reader, context),
            _ => throw reader.Error(DisplayListFailureCode.InvalidValue, tagOffset, $"Shader tag {tag} is unknown."),
        };
    }

    private static DisplayLinearGradientShader ReadLinearGradient(ref DisplayListBinaryReader reader)
    {
        var start = ReadPoint(ref reader);
        var end = ReadPoint(ref reader);
        var tile = ReadEnumByte<DisplayTileMode>(ref reader, "tile mode");
        var (colors, stops) = ReadGradient(ref reader);
        return new DisplayLinearGradientShader(start, end, colors, stops, tile, ReadOptionalMatrix(ref reader));
    }

    private static DisplayRadialGradientShader ReadRadialGradient(ref DisplayListBinaryReader reader)
    {
        var center = ReadPoint(ref reader);
        var radius = ReadNonnegativeSingle(ref reader, "gradient radius");
        var tile = ReadEnumByte<DisplayTileMode>(ref reader, "tile mode");
        var hasFocal = reader.ReadBoolean();
        DisplayPoint? focal = hasFocal ? ReadPoint(ref reader) : null;
        var focalRadius = ReadNonnegativeSingle(ref reader, "gradient focal radius");
        var (colors, stops) = ReadGradient(ref reader);
        return new DisplayRadialGradientShader(
            center,
            radius,
            colors,
            stops,
            tile,
            focal,
            focalRadius,
            ReadOptionalMatrix(ref reader));
    }

    private static DisplaySweepGradientShader ReadSweepGradient(ref DisplayListBinaryReader reader)
    {
        var center = ReadPoint(ref reader);
        var start = reader.ReadSingle();
        var end = reader.ReadSingle();
        var tile = ReadEnumByte<DisplayTileMode>(ref reader, "tile mode");
        var (colors, stops) = ReadGradient(ref reader);
        return new DisplaySweepGradientShader(center, start, end, colors, stops, tile, ReadOptionalMatrix(ref reader));
    }

    private static DisplayImageShader ReadImageShader(
        ref DisplayListBinaryReader reader,
        DecoderContext context)
    {
        var image = context.ReadResource(ref reader, DisplayResourceKind.Image);
        var tileX = ReadEnumByte<DisplayTileMode>(ref reader, "horizontal tile mode");
        var tileY = ReadEnumByte<DisplayTileMode>(ref reader, "vertical tile mode");
        var sampling = ReadEnumByte<DisplaySamplingQuality>(ref reader, "sampling quality");
        RequireZero(reader.ReadByte(), reader.AbsoluteOffset - 1, "image-shader reserved field", ref reader);
        return new DisplayImageShader(image, tileX, tileY, sampling, ReadMatrix(ref reader));
    }

    private static DisplayRuntimeEffectShader ReadRuntimeEffectShader(
        ref DisplayListBinaryReader reader,
        DecoderContext context)
    {
        var effect = context.ReadResource(ref reader, DisplayResourceKind.RuntimeEffect);
        var uniformCount = ReadCount(ref reader, "runtime-effect uniform byte");
        if (uniformCount > reader.Remaining)
        {
            throw reader.Error(DisplayListFailureCode.BoundsExceeded, reader.AbsoluteOffset, "The uniform byte array exceeds the command payload.");
        }

        var uniforms = reader.ReadBytes(uniformCount).ToArray();
        var childCount = ReadCount(ref reader, "runtime-effect child");
        if ((long)childCount * 16 > reader.Remaining)
        {
            throw reader.Error(DisplayListFailureCode.BoundsExceeded, reader.AbsoluteOffset, "The runtime-effect child array exceeds the command payload.");
        }

        var children = new DisplayResourceReference[childCount];
        for (var index = 0; index < childCount; index++)
        {
            children[index] = context.ReadResource(ref reader, null);
        }

        return new DisplayRuntimeEffectShader(effect, uniforms, children);
    }

    private static (uint[] Colors, float[] Stops) ReadGradient(ref DisplayListBinaryReader reader)
    {
        var count = ReadCount(ref reader, "gradient stop");
        if (count < 2)
        {
            throw reader.Error(DisplayListFailureCode.InvalidValue, reader.AbsoluteOffset - sizeof(uint), "A gradient requires at least two stops.");
        }

        if ((long)count * 8 > reader.Remaining)
        {
            throw reader.Error(DisplayListFailureCode.BoundsExceeded, reader.AbsoluteOffset, "The gradient array exceeds the command payload.");
        }

        var colors = new uint[count];
        var stops = new float[count];
        var previous = float.NegativeInfinity;
        for (var index = 0; index < count; index++)
        {
            colors[index] = reader.ReadUInt32();
            var stopOffset = reader.AbsoluteOffset;
            stops[index] = reader.ReadSingle();
            if (stops[index] < previous)
            {
                throw reader.Error(DisplayListFailureCode.InvalidValue, stopOffset, "Gradient stops must be nondecreasing.");
            }

            previous = stops[index];
        }

        return (colors, stops);
    }

    private static DisplayColorFilter ReadRequiredColorFilter(
        ref DisplayListBinaryReader reader,
        DecoderContext context,
        int depth) =>
        ReadColorFilter(ref reader, context, depth, false)!;

    private static DisplayColorFilter? ReadColorFilter(
        ref DisplayListBinaryReader reader,
        DecoderContext context,
        int depth,
        bool allowNull)
    {
        _ = context;
        RequireDepth(depth, ref reader);
        var tagOffset = reader.AbsoluteOffset;
        var tag = reader.ReadByte();
        return tag switch
        {
            0 when allowNull => null,
            0 => throw reader.Error(DisplayListFailureCode.InvalidValue, tagOffset, "A required color filter cannot use the null tag."),
            1 => new DisplayBlendColorFilter(
                reader.ReadUInt32(),
                ReadEnumByte<DisplayBlendMode>(ref reader, "blend mode")),
            2 => new DisplayMatrixColorFilter(ReadSingles(ref reader, 20)),
            3 => new DisplayLinearToSrgbColorFilter(),
            4 => new DisplaySrgbToLinearColorFilter(),
            _ => throw reader.Error(DisplayListFailureCode.InvalidValue, tagOffset, $"Color-filter tag {tag} is unknown."),
        };
    }

    private static DisplayMaskFilter? ReadMaskFilter(ref DisplayListBinaryReader reader)
    {
        if (!reader.ReadBoolean())
        {
            return null;
        }

        return new DisplayMaskFilter(
            ReadEnumByte<DisplayBlurStyle>(ref reader, "blur style"),
            ReadNonnegativeSingle(ref reader, "mask-filter sigma"));
    }

    private static DisplayImageFilter ReadRequiredImageFilter(
        ref DisplayListBinaryReader reader,
        DecoderContext context,
        int depth) =>
        ReadImageFilter(ref reader, context, depth, false)!;

    private static DisplayImageFilter? ReadImageFilter(
        ref DisplayListBinaryReader reader,
        DecoderContext context,
        int depth,
        bool allowNull)
    {
        RequireDepth(depth, ref reader);
        var tagOffset = reader.AbsoluteOffset;
        var tag = reader.ReadByte();
        return tag switch
        {
            0 when allowNull => null,
            0 => throw reader.Error(DisplayListFailureCode.InvalidValue, tagOffset, "A required image filter cannot use the null tag."),
            1 => new DisplayBlurImageFilter(
                ReadNonnegativeSingle(ref reader, "horizontal blur sigma"),
                ReadNonnegativeSingle(ref reader, "vertical blur sigma"),
                ReadEnumByte<DisplayTileMode>(ref reader, "tile mode"),
                ReadOptionalRect(ref reader)),
            2 => new DisplayColorImageFilter(ReadRequiredColorFilter(ref reader, context, depth + 1)),
            3 => new DisplayMatrixImageFilter(
                ReadMatrix(ref reader),
                ReadEnumByte<DisplaySamplingQuality>(ref reader, "sampling quality")),
            4 => ReadRuntimeEffectImageFilter(ref reader, context, depth),
            5 => new DisplayComposeImageFilter(
                ReadRequiredImageFilter(ref reader, context, depth + 1),
                ReadRequiredImageFilter(ref reader, context, depth + 1)),
            6 => new DisplayDropShadowImageFilter(
                reader.ReadSingle(),
                reader.ReadSingle(),
                ReadNonnegativeSingle(ref reader, "horizontal shadow sigma"),
                ReadNonnegativeSingle(ref reader, "vertical shadow sigma"),
                reader.ReadUInt32(),
                reader.ReadBoolean()),
            _ => throw reader.Error(DisplayListFailureCode.InvalidValue, tagOffset, $"Image-filter tag {tag} is unknown."),
        };
    }

    private static DisplayRuntimeEffectImageFilter ReadRuntimeEffectImageFilter(
        ref DisplayListBinaryReader reader,
        DecoderContext context,
        int depth)
    {
        var shader = ReadRequiredShader(ref reader, context, depth + 1);
        if (shader is not DisplayRuntimeEffectShader runtimeEffect)
        {
            throw reader.Error(
                DisplayListFailureCode.InvalidValue,
                reader.AbsoluteOffset,
                "A runtime-effect image filter requires a runtime-effect shader payload.");
        }

        return new DisplayRuntimeEffectImageFilter(
            runtimeEffect,
            ReadEnumByte<DisplaySamplingQuality>(ref reader, "sampling quality"));
    }

    private static DisplayPath ReadPath(ref DisplayListBinaryReader reader)
    {
        var fillType = ReadEnumByte<DisplayPathFillType>(ref reader, "path fill type");
        RequireZero(reader.ReadByte(), reader.AbsoluteOffset - 1, "path reserved byte", ref reader);
        RequireZero(reader.ReadUInt16(), reader.AbsoluteOffset - 2, "path reserved field", ref reader);
        var verbCount = ReadCount(ref reader, "path verb");
        var valueCount = ReadCount(ref reader, "path value");
        if (verbCount > reader.Remaining)
        {
            throw reader.Error(DisplayListFailureCode.BoundsExceeded, reader.AbsoluteOffset, "The path verb stream exceeds the command payload.");
        }

        var verbs = new DisplayPathVerb[verbCount];
        long expectedValueCount = 0;
        for (var index = 0; index < verbCount; index++)
        {
            verbs[index] = ReadEnumByte<DisplayPathVerb>(ref reader, "path verb");
            expectedValueCount += ValuesForVerb(verbs[index]);
        }

        if (expectedValueCount != valueCount)
        {
            throw reader.Error(
                DisplayListFailureCode.InvalidValue,
                reader.AbsoluteOffset,
                $"The path verb stream requires {expectedValueCount} values but declares {valueCount}.");
        }

        if ((long)valueCount * sizeof(float) > reader.Remaining)
        {
            throw reader.Error(DisplayListFailureCode.BoundsExceeded, reader.AbsoluteOffset, "The path value stream exceeds the command payload.");
        }

        return new DisplayPath(fillType, verbs, ReadSingles(ref reader, valueCount));
    }

    private static int ValuesForVerb(DisplayPathVerb verb) => verb switch
    {
        DisplayPathVerb.MoveTo or
        DisplayPathVerb.LineTo or
        DisplayPathVerb.RelativeMoveTo or
        DisplayPathVerb.RelativeLineTo => 2,
        DisplayPathVerb.QuadraticTo => 4,
        DisplayPathVerb.ConicTo => 5,
        DisplayPathVerb.CubicTo => 6,
        DisplayPathVerb.AddRect or DisplayPathVerb.AddOval => 4,
        DisplayPathVerb.AddArc => 6,
        DisplayPathVerb.AddRoundedRect or DisplayPathVerb.AddSuperellipse => 12,
        DisplayPathVerb.ArcToPoint or DisplayPathVerb.ArcTo => 7,
        DisplayPathVerb.Close => 0,
        _ => throw new ArgumentOutOfRangeException(nameof(verb)),
    };

    private static DisplayPoint ReadPoint(ref DisplayListBinaryReader reader) =>
        new(reader.ReadSingle(), reader.ReadSingle());

    private static DisplayRect ReadRect(ref DisplayListBinaryReader reader) =>
        new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

    private static DisplayRect? ReadOptionalRect(ref DisplayListBinaryReader reader) =>
        reader.ReadBoolean() ? ReadRect(ref reader) : null;

    private static DisplayRoundedRect ReadRoundedRect(ref DisplayListBinaryReader reader) =>
        new(
            ReadRect(ref reader),
            ReadNonnegativeSingle(ref reader, "top-left x radius"),
            ReadNonnegativeSingle(ref reader, "top-left y radius"),
            ReadNonnegativeSingle(ref reader, "top-right x radius"),
            ReadNonnegativeSingle(ref reader, "top-right y radius"),
            ReadNonnegativeSingle(ref reader, "bottom-right x radius"),
            ReadNonnegativeSingle(ref reader, "bottom-right y radius"),
            ReadNonnegativeSingle(ref reader, "bottom-left x radius"),
            ReadNonnegativeSingle(ref reader, "bottom-left y radius"));

    private static DisplayMatrix ReadMatrix(ref DisplayListBinaryReader reader) =>
        new(ReadSingles(ref reader, 16));

    private static DisplayMatrix? ReadOptionalMatrix(ref DisplayListBinaryReader reader) =>
        reader.ReadBoolean() ? ReadMatrix(ref reader) : null;

    private static float[] ReadSingles(ref DisplayListBinaryReader reader, int count)
    {
        if ((long)count * sizeof(float) > reader.Remaining)
        {
            throw reader.Error(DisplayListFailureCode.BoundsExceeded, reader.AbsoluteOffset, "A float array exceeds the command payload.");
        }

        var result = new float[count];
        for (var index = 0; index < count; index++)
        {
            result[index] = reader.ReadSingle();
        }

        return result;
    }

    private static float ReadPositiveSingle(ref DisplayListBinaryReader reader, string name)
    {
        var offset = reader.AbsoluteOffset;
        var value = reader.ReadSingle();
        if (value <= 0)
        {
            throw reader.Error(DisplayListFailureCode.InvalidValue, offset, $"The {name} must be positive.");
        }

        return value;
    }

    private static float ReadNonnegativeSingle(ref DisplayListBinaryReader reader, string name)
    {
        var offset = reader.AbsoluteOffset;
        var value = reader.ReadSingle();
        if (value < 0)
        {
            throw reader.Error(DisplayListFailureCode.InvalidValue, offset, $"The {name} cannot be negative.");
        }

        return value;
    }

    private static int ReadCount(ref DisplayListBinaryReader reader, string name)
    {
        var offset = reader.AbsoluteOffset;
        var count = reader.ReadUInt32();
        if (count > DisplayListFormat.MaximumCollectionCount)
        {
            throw reader.Error(DisplayListFailureCode.LimitExceeded, offset, $"The DisplayList {name} count limit was exceeded.");
        }

        return checked((int)count);
    }

    private static TEnum ReadEnumByte<TEnum>(
        ref DisplayListBinaryReader reader,
        string name)
        where TEnum : struct, Enum
    {
        var offset = reader.AbsoluteOffset;
        var raw = reader.ReadByte();
        if (!Enum.IsDefined(typeof(TEnum), raw))
        {
            throw reader.Error(DisplayListFailureCode.InvalidValue, offset, $"The {name} value {raw} is unknown.");
        }

        return (TEnum)Enum.ToObject(typeof(TEnum), raw);
    }

    private static DisplayResourceKind ReadResourceKind(ref DisplayListBinaryReader reader)
    {
        var offset = reader.AbsoluteOffset;
        var raw = reader.ReadUInt16();
        var kind = (DisplayResourceKind)raw;
        if (!Enum.IsDefined(kind))
        {
            throw reader.Error(DisplayListFailureCode.InvalidResource, offset, $"Resource kind {raw} is unknown.");
        }

        return kind;
    }

    private static void ValidateResourceReference(
        DisplayResourceReference reference,
        DisplayResourceKind? expectedKind,
        int offset,
        ref DisplayListBinaryReader reader)
    {
        if (reference.Id == 0 || reference.Version == 0)
        {
            throw reader.Error(DisplayListFailureCode.InvalidResource, offset, "Resource id and version must be nonzero.");
        }

        if (expectedKind is not null && reference.Kind != expectedKind)
        {
            throw reader.Error(
                DisplayListFailureCode.InvalidResource,
                offset,
                $"Resource kind {reference.Kind} does not match required kind {expectedKind}.");
        }
    }

    private static void RequireDepth(int depth, ref DisplayListBinaryReader reader)
    {
        if (depth > DisplayListFormat.MaximumNestingDepth)
        {
            throw reader.Error(DisplayListFailureCode.LimitExceeded, reader.AbsoluteOffset, "The tagged-value nesting limit was exceeded.");
        }
    }

    private static void RequireZero(
        ulong value,
        int offset,
        string name,
        ref DisplayListBinaryReader reader)
    {
        if (value != 0)
        {
            throw reader.Error(DisplayListFailureCode.NonCanonicalEncoding, offset, $"The {name} must be zero.");
        }
    }

    private sealed class DecoderContext(
        IReadOnlyDictionary<DisplayResourceReference, DisplayResourceDescriptor> resources,
        IReadOnlyList<string> strings)
    {
        internal DisplayResourceReference ReadResource(
            ref DisplayListBinaryReader reader,
            DisplayResourceKind? expectedKind)
        {
            var offset = reader.AbsoluteOffset;
            var kind = ReadResourceKind(ref reader);
            RequireZero(reader.ReadUInt16(), reader.AbsoluteOffset - 2, "resource-reference reserved field", ref reader);
            var version = reader.ReadUInt32();
            var id = reader.ReadUInt64();
            var reference = new DisplayResourceReference(kind, id, version);
            ValidateResourceReference(reference, expectedKind, offset, ref reader);
            if (!resources.ContainsKey(reference))
            {
                throw reader.Error(
                    DisplayListFailureCode.MissingResource,
                    offset,
                    $"Resource {reference} is referenced but not declared by this scene.");
            }

            return reference;
        }

        internal string? ReadString(ref DisplayListBinaryReader reader, bool allowNull)
        {
            var offset = reader.AbsoluteOffset;
            var id = reader.ReadUInt32();
            if (allowNull && id == uint.MaxValue)
            {
                return null;
            }

            if (id >= strings.Count)
            {
                throw reader.Error(DisplayListFailureCode.InvalidString, offset, $"String-table id {id} is not declared.");
            }

            return strings[checked((int)id)];
        }
    }
}

public static class DisplayListValidator
{
    public static DisplayListDecodeResult Validate(ReadOnlySpan<byte> buffer) =>
        DisplayListDecoder.Decode(buffer);
}
