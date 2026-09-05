using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;

namespace Doroti.Graphics.DisplayList;

public static class DisplayListEncoder
{
    private const DisplayListFlags KnownFlags =
        DisplayListFlags.ChecksumPresent |
        DisplayListFlags.DiagnosticCapture;
    private const DisplayResourceFlags KnownResourceFlags = DisplayResourceFlags.Recoverable;

    public static byte[] Encode(DisplayListDocument document) => Encode(document, null);

    public static byte[] Encode(DisplayListDocument document, DisplayListEncodingCache? cache)
    {
        ArgumentNullException.ThrowIfNull(document);
        cache?.BeginFrame();
        ValidateScene(document.Scene);
        if ((document.Flags & ~KnownFlags) != 0)
        {
            throw new ArgumentException($"DisplayList flags contain unsupported bits: {document.Flags}.", nameof(document));
        }

        if (document.Commands.Count > DisplayListFormat.MaximumCommandCount)
        {
            throw new ArgumentException("The DisplayList command limit was exceeded.", nameof(document));
        }

        if (document.Resources.Count > DisplayListFormat.MaximumResourceCount)
        {
            throw new ArgumentException("The DisplayList resource limit was exceeded.", nameof(document));
        }

        var resources = CanonicalizeResources(document.Resources);
        var resourceCatalog = resources.ToDictionary(resource => resource.Reference);
        var strings = CanonicalizeStrings(document.Commands);
        var stringIds = strings
            .Select((value, index) => (value, index))
            .ToDictionary(item => item.value.Value, item => checked((uint)item.index), StringComparer.Ordinal);

        var resourceWriter = new DisplayListBinaryWriter();
        foreach (var resource in resources)
        {
            WriteResourceDescriptor(resourceWriter, resource);
        }

        var stringWriter = new DisplayListBinaryWriter();
        foreach (var value in strings)
        {
            stringWriter.WriteUInt32(checked((uint)value.Bytes.Length));
            stringWriter.WriteBytes(value.Bytes);
        }

        if (stringWriter.Length > DisplayListFormat.MaximumStringTableByteLength)
        {
            throw new ArgumentException("The DisplayList string-table byte limit was exceeded.", nameof(document));
        }

        var commandWriter = new DisplayListBinaryWriter();
        var context = new EncoderContext(resourceCatalog, stringIds);
        foreach (var command in document.Commands)
        {
            ArgumentNullException.ThrowIfNull(command);
            commandWriter.WriteUInt16((ushort)command.Opcode);
            commandWriter.WriteUInt16(0);
            var payloadLengthOffset = commandWriter.Length;
            commandWriter.WriteUInt32(0);
            var payloadOffset = commandWriter.Length;
            if (cache is not null && cache.TryGet(command, out var cachedPayload))
                commandWriter.WriteBytes(cachedPayload);
            else
            {
                WriteCommandPayload(commandWriter, command, context);
                cache?.Add(command, commandWriter.WrittenSpan[payloadOffset..]);
            }
            commandWriter.PatchUInt32(
                payloadLengthOffset,
                checked((uint)(commandWriter.Length - payloadOffset)));
        }

        var byteLength = checked(
            DisplayListFormat.HeaderSize +
            resourceWriter.Length +
            stringWriter.Length +
            commandWriter.Length);
        if (byteLength > DisplayListFormat.MaximumByteLength)
        {
            throw new ArgumentException("The DisplayList byte limit was exceeded.", nameof(document));
        }

        var buffer = new byte[byteLength];
        var header = buffer.AsSpan(0, DisplayListFormat.HeaderSize);
        BinaryPrimitives.WriteUInt32LittleEndian(header[0..], DisplayListFormat.Magic);
        BinaryPrimitives.WriteUInt16LittleEndian(header[4..], DisplayListFormat.SchemaVersion);
        BinaryPrimitives.WriteUInt16LittleEndian(header[6..], DisplayListFormat.HeaderSize);
        BinaryPrimitives.WriteUInt32LittleEndian(header[8..], checked((uint)byteLength));
        BinaryPrimitives.WriteUInt32LittleEndian(header[12..], (uint)document.Flags);
        WriteScene(header, document.Scene);
        BinaryPrimitives.WriteUInt32LittleEndian(header[84..], checked((uint)document.Commands.Count));
        BinaryPrimitives.WriteUInt32LittleEndian(header[88..], checked((uint)resources.Count));
        BinaryPrimitives.WriteUInt32LittleEndian(header[92..], checked((uint)stringWriter.Length));
        BinaryPrimitives.WriteUInt32LittleEndian(header[96..], checked((uint)commandWriter.Length));
        BinaryPrimitives.WriteUInt32LittleEndian(header[100..], checked((uint)resourceWriter.Length));
        BinaryPrimitives.WriteUInt32LittleEndian(header[104..], 0);
        BinaryPrimitives.WriteUInt32LittleEndian(header[108..], 0);

        var destinationOffset = (int)DisplayListFormat.HeaderSize;
        resourceWriter.WrittenSpan.CopyTo(buffer.AsSpan(destinationOffset));
        destinationOffset += resourceWriter.Length;
        stringWriter.WrittenSpan.CopyTo(buffer.AsSpan(destinationOffset));
        destinationOffset += stringWriter.Length;
        commandWriter.WrittenSpan.CopyTo(buffer.AsSpan(destinationOffset));

        if ((document.Flags & DisplayListFlags.ChecksumPresent) != 0)
        {
            BinaryPrimitives.WriteUInt32LittleEndian(
                header[DisplayListFormat.ChecksumOffset..],
                DisplayListChecksum.Compute(buffer));
        }

        return buffer;
    }

    private static void WriteScene(Span<byte> header, DisplayListSceneMetadata scene)
    {
        BinaryPrimitives.WriteUInt64LittleEndian(header[16..], scene.ViewId);
        BinaryPrimitives.WriteUInt64LittleEndian(header[24..], scene.SceneSequence);
        BinaryPrimitives.WriteUInt64LittleEndian(header[32..], scene.BuildToken);
        BinaryPrimitives.WriteUInt64LittleEndian(header[40..], scene.ResizeEpoch);
        BinaryPrimitives.WriteUInt64LittleEndian(header[48..], scene.SurfaceGeneration);
        BinaryPrimitives.WriteUInt64LittleEndian(header[56..], scene.ContextGeneration);
        BinaryPrimitives.WriteInt32LittleEndian(header[64..], CanonicalSingleBits(scene.LogicalWidth));
        BinaryPrimitives.WriteInt32LittleEndian(header[68..], CanonicalSingleBits(scene.LogicalHeight));
        BinaryPrimitives.WriteUInt32LittleEndian(header[72..], scene.PhysicalWidth);
        BinaryPrimitives.WriteUInt32LittleEndian(header[76..], scene.PhysicalHeight);
        BinaryPrimitives.WriteInt32LittleEndian(header[80..], CanonicalSingleBits(scene.DevicePixelRatio));
    }

    private static int CanonicalSingleBits(float value) =>
        BitConverter.SingleToInt32Bits(value == 0 ? 0 : value);

    private static List<DisplayResourceDescriptor> CanonicalizeResources(
        IReadOnlyList<DisplayResourceDescriptor> resources)
    {
        var result = resources
            .OrderBy(resource => resource.Reference.Kind)
            .ThenBy(resource => resource.Reference.Id)
            .ThenBy(resource => resource.Reference.Version)
            .ToList();
        DisplayResourceReference? previous = null;
        foreach (var resource in result)
        {
            ValidateResourceReference(resource.Reference, null);
            if ((resource.Flags & ~KnownResourceFlags) != 0)
            {
                throw new ArgumentException($"Resource {resource.Reference} contains unsupported flags.", nameof(resources));
            }

            if (previous == resource.Reference)
            {
                throw new ArgumentException($"Resource {resource.Reference} is declared more than once.", nameof(resources));
            }

            previous = resource.Reference;
        }

        return result;
    }

    private static List<(string Value, byte[] Bytes)> CanonicalizeStrings(IReadOnlyList<DisplayListCommand> commands)
    {
        var values = new HashSet<string>(StringComparer.Ordinal);
        foreach (var command in commands)
        {
            if (command is not DisplayDrawParagraphCommand paragraph)
            {
                continue;
            }

            AddString(values, paragraph.Paragraph.Text);
            AddString(values, paragraph.Paragraph.FontFamily);
            AddString(values, paragraph.Paragraph.Locale);
            if (paragraph.Paragraph.Ellipsis is not null)
            {
                AddString(values, paragraph.Paragraph.Ellipsis);
            }
            foreach (var run in paragraph.Paragraph.TextRuns)
            {
                AddString(values, run.Text);
                AddString(values, run.FontFamily);
                AddString(values, run.Locale);
                foreach (var family in run.FontFamilyFallback) AddString(values, family);
                foreach (var feature in run.FontFeatures) AddString(values, feature.Name);
                foreach (var variation in run.FontVariations) AddString(values, variation.Axis);
            }
        }

        // Repeated paragraph/run strings need one strict conversion per scene.
        // Keep those bytes through sorting and writing instead of validating
        // every occurrence and encoding each unique value twice. Nothing is
        // retained across scenes and resource/command validation is unchanged.
        var encoded = new List<(string Value, byte[] Bytes)>(values.Count);
        foreach (var value in values)
        {
            try
            {
                encoded.Add((value, DisplayListUtf8.StrictEncoding.GetBytes(value)));
            }
            catch (EncoderFallbackException exception)
            {
                throw new ArgumentException("DisplayList strings must contain valid Unicode scalar values.", nameof(value), exception);
            }
        }
        encoded.Sort((left, right) => DisplayListUtf8.Compare(left.Bytes, right.Bytes));
        return encoded;
    }

    private static void AddString(HashSet<string> values, string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        values.Add(value);
    }

    private static void WriteResourceDescriptor(
        DisplayListBinaryWriter writer,
        DisplayResourceDescriptor descriptor)
    {
        writer.WriteUInt16((ushort)descriptor.Reference.Kind);
        writer.WriteUInt16((ushort)descriptor.Flags);
        writer.WriteUInt32(descriptor.Reference.Version);
        writer.WriteUInt64(descriptor.Reference.Id);
        writer.WriteUInt64(descriptor.Fingerprint.Low);
        writer.WriteUInt64(descriptor.Fingerprint.High);
    }

    private static void WriteCommandPayload(
        DisplayListBinaryWriter writer,
        DisplayListCommand command,
        EncoderContext context)
    {
        switch (command)
        {
            case DisplaySaveCommand:
            case DisplayRestoreCommand:
                return;
            case DisplaySaveLayerCommand value:
                WriteOptionalRect(writer, value.Bounds);
                WriteOptionalPaint(writer, value.Paint, context, 0);
                return;
            case DisplayTransformCommand value:
                WriteMatrix(writer, value.Matrix);
                return;
            case DisplayClipRectCommand value:
                WriteRect(writer, value.Rect);
                WriteEnumByte(writer, value.Operation, "clip operation");
                writer.WriteBoolean(value.IsAntiAlias);
                writer.WriteUInt16(0);
                return;
            case DisplayClipRoundedRectCommand value:
                WriteRoundedRect(writer, value.RoundedRect);
                WriteEnumByte(writer, value.Operation, "clip operation");
                writer.WriteBoolean(value.IsAntiAlias);
                writer.WriteUInt16(0);
                return;
            case DisplayClipPathCommand value:
                WritePath(writer, value.Path);
                WriteEnumByte(writer, value.Operation, "clip operation");
                writer.WriteBoolean(value.IsAntiAlias);
                writer.WriteUInt16(0);
                return;
            case DisplayDrawColorCommand value:
                writer.WriteUInt32(value.Color);
                WriteEnumByte(writer, value.BlendMode, "blend mode");
                return;
            case DisplayDrawPaintCommand value:
                WritePaint(writer, value.Paint, context, 0);
                return;
            case DisplayDrawLineCommand value:
                WritePoint(writer, value.Start);
                WritePoint(writer, value.End);
                WritePaint(writer, value.Paint, context, 0);
                return;
            case DisplayDrawPointsCommand value:
                WriteEnumByte(writer, value.Mode, "point mode");
                writer.WriteUInt32(CheckedCount(value.Points.Count, "point"));
                foreach (var point in value.Points)
                {
                    WritePoint(writer, point);
                }

                WritePaint(writer, value.Paint, context, 0);
                return;
            case DisplayDrawRectCommand value:
                WriteRect(writer, value.Rect);
                WritePaint(writer, value.Paint, context, 0);
                return;
            case DisplayDrawRoundedRectCommand value:
                WriteRoundedRect(writer, value.RoundedRect);
                WritePaint(writer, value.Paint, context, 0);
                return;
            case DisplayDrawDoubleRoundedRectCommand value:
                WriteRoundedRect(writer, value.Outer);
                WriteRoundedRect(writer, value.Inner);
                WritePaint(writer, value.Paint, context, 0);
                return;
            case DisplayDrawCircleCommand value:
                WritePoint(writer, value.Center);
                WriteNonnegativeSingle(writer, value.Radius, nameof(value.Radius));
                WritePaint(writer, value.Paint, context, 0);
                return;
            case DisplayDrawOvalCommand value:
                WriteRect(writer, value.Bounds);
                WritePaint(writer, value.Paint, context, 0);
                return;
            case DisplayDrawArcCommand value:
                WriteRect(writer, value.Bounds);
                WriteFiniteSingle(writer, value.StartAngle, nameof(value.StartAngle));
                WriteFiniteSingle(writer, value.SweepAngle, nameof(value.SweepAngle));
                writer.WriteBoolean(value.UseCenter);
                WritePaint(writer, value.Paint, context, 0);
                return;
            case DisplayDrawPathCommand value:
                WritePath(writer, value.Path);
                WritePaint(writer, value.Paint, context, 0);
                return;
            case DisplayDrawShadowCommand value:
                WritePath(writer, value.Path);
                writer.WriteUInt32(value.Color);
                WriteNonnegativeSingle(writer, value.Elevation, nameof(value.Elevation));
                writer.WriteBoolean(value.TransparentOccluder);
                return;
            case DisplayDrawImageCommand value:
                context.WriteResource(writer, value.Image, DisplayResourceKind.Image);
                WritePoint(writer, value.Offset);
                WriteEnumByte(writer, value.Sampling, "sampling quality");
                WritePaint(writer, value.Paint, context, 0);
                return;
            case DisplayDrawImageRectCommand value:
                context.WriteResource(writer, value.Image, DisplayResourceKind.Image);
                WriteRect(writer, value.Source);
                WriteRect(writer, value.Destination);
                WriteEnumByte(writer, value.Sampling, "sampling quality");
                WritePaint(writer, value.Paint, context, 0);
                return;
            case DisplayDrawNinePatchCommand value:
                context.WriteResource(writer, value.Image, DisplayResourceKind.Image);
                WriteRect(writer, value.Center);
                WriteRect(writer, value.Destination);
                WriteEnumByte(writer, value.Sampling, "sampling quality");
                WritePaint(writer, value.Paint, context, 0);
                return;
            case DisplayDrawParagraphCommand value:
                WriteParagraph(writer, value.Paragraph, context);
                WritePoint(writer, value.Offset);
                return;
            case DisplayPushOpacityCommand value:
                WriteUnitSingle(writer, value.Opacity, nameof(value.Opacity));
                WritePoint(writer, value.Offset);
                return;
            case DisplayPushColorFilterCommand value:
                WriteColorFilter(writer, value.Filter, context, 0, false);
                WritePoint(writer, value.Offset);
                return;
            case DisplayPushImageFilterCommand value:
                WriteImageFilter(writer, value.Filter, context, 0, false);
                WritePoint(writer, value.Offset);
                WriteOptionalRect(writer, value.Bounds);
                return;
            case DisplayPushBackdropFilterCommand value:
                WriteImageFilter(writer, value.Filter, context, 0, false);
                WriteEnumByte(writer, value.BlendMode, "blend mode");
                writer.WriteUInt64(value.BackdropId);
                WritePoint(writer, value.Offset);
                return;
            case DisplayPushShaderMaskCommand value:
                WriteShader(writer, value.Shader, context, 0, false);
                WriteRect(writer, value.MaskRect);
                WriteEnumByte(writer, value.BlendMode, "blend mode");
                return;
            case DisplayDrawRetainedSceneCommand value:
                context.WriteResource(writer, value.Scene, DisplayResourceKind.RetainedScene);
                WritePoint(writer, value.Offset);
                if ((value.CacheHint & ~(DisplayRetainedSceneCacheHint.IsComplex | DisplayRetainedSceneCacheHint.WillChange)) != 0)
                {
                    throw new ArgumentException("Retained-scene cache hints contain unsupported bits.", nameof(command));
                }

                writer.WriteByte((byte)value.CacheHint);
                return;
            default:
                throw new ArgumentException($"Command type {command.GetType().FullName} is not supported by DisplayList v2.", nameof(command));
        }
    }

    private static void WriteParagraph(
        DisplayListBinaryWriter writer,
        DisplayParagraphRecipe paragraph,
        EncoderContext context)
    {
        ArgumentNullException.ThrowIfNull(paragraph);
        writer.WriteUInt32(context.StringId(paragraph.Text));
        context.WriteResource(writer, paragraph.Font, DisplayResourceKind.Font);
        writer.WriteUInt32(context.StringId(paragraph.FontFamily));
        writer.WriteUInt32(context.StringId(paragraph.Locale));
        writer.WriteUInt32(paragraph.Ellipsis is null ? uint.MaxValue : context.StringId(paragraph.Ellipsis));
        WritePositiveSingle(writer, paragraph.FontSize, nameof(paragraph.FontSize));
        WritePositiveSingle(writer, paragraph.HeightMultiplier, nameof(paragraph.HeightMultiplier));
        writer.WriteUInt32(paragraph.Color);
        if (paragraph.FontWeight is < 1 or > 1000)
        {
            throw new ArgumentOutOfRangeException(nameof(paragraph), "Font weight must be between 1 and 1000.");
        }

        writer.WriteInt32(paragraph.FontWeight);
        WriteEnumByte(writer, paragraph.FontSlant, "font slant");
        WriteEnumByte(writer, paragraph.Direction, "text direction");
        WriteEnumByte(writer, paragraph.Align, "text alignment");
        writer.WriteByte(0);
        writer.WriteUInt32(paragraph.MaxLines);
        WriteNonnegativeSingle(writer, paragraph.LayoutWidth, nameof(paragraph.LayoutWidth));
        WriteNonnegativeSingle(writer, paragraph.MeasuredWidth, nameof(paragraph.MeasuredWidth));
        WriteNonnegativeSingle(writer, paragraph.MeasuredHeight, nameof(paragraph.MeasuredHeight));
        writer.WriteUInt64(paragraph.MetricsHash);
        writer.WriteUInt32(CheckedCount(paragraph.FallbackFonts.Count, "fallback font"));
        foreach (var fallback in paragraph.FallbackFonts)
        {
            context.WriteResource(writer, fallback, DisplayResourceKind.Font);
        }
        if (paragraph.TextRuns.Count != 0 &&
            !string.Equals(string.Concat(paragraph.TextRuns.Select(run => run?.Text)), paragraph.Text, StringComparison.Ordinal))
            throw new ArgumentException("Paragraph text runs must concatenate to the paragraph text.", nameof(paragraph));
        writer.WriteUInt32(CheckedCount(paragraph.TextRuns.Count, "paragraph text run"));
        foreach (var run in paragraph.TextRuns)
        {
            ArgumentNullException.ThrowIfNull(run);
            if (run.Text.Length == 0)
                throw new ArgumentException("Paragraph text runs must be nonempty.", nameof(paragraph));
            writer.WriteUInt32(context.StringId(run.Text));
            writer.WriteUInt32(context.StringId(run.FontFamily));
            writer.WriteUInt32(context.StringId(run.Locale));
            WritePositiveSingle(writer, run.FontSize, nameof(run.FontSize));
            WritePositiveSingle(writer, run.HeightMultiplier, nameof(run.HeightMultiplier));
            writer.WriteUInt32(run.Color);
            if (run.FontWeight is < 1 or > 1000)
                throw new ArgumentOutOfRangeException(nameof(paragraph), "Run font weight must be between 1 and 1000.");
            writer.WriteInt32(run.FontWeight);
            WriteEnumByte(writer, run.FontSlant, "run font slant");
            if ((run.Decoration & ~7u) != 0)
                throw new ArgumentOutOfRangeException(nameof(paragraph), "Run decoration contains unknown bits.");
            writer.WriteUInt32(run.Decoration);
            WriteOptionalUInt32(writer, run.BackgroundColor);
            WriteOptionalUInt32(writer, run.DecorationColor);
            WriteOptionalEnumByte(writer, run.DecorationStyle, "run decoration style");
            WriteOptionalNonnegativeSingle(writer, run.DecorationThickness, "run decoration thickness");
            WriteOptionalEnumByte(writer, run.TextBaseline, "run text baseline");
            WriteOptionalFiniteSingle(writer, run.LetterSpacing, "run letter spacing");
            WriteOptionalFiniteSingle(writer, run.WordSpacing, "run word spacing");
            writer.WriteByte(run.HalfLeading switch { null => 0, false => 1, true => 2 });
            writer.WriteUInt32(CheckedCount(run.FontFamilyFallback.Count, "run fallback font family"));
            foreach (var family in run.FontFamilyFallback)
                writer.WriteUInt32(context.StringId(family));
            writer.WriteUInt32(CheckedCount(run.Shadows.Count, "run shadow"));
            foreach (var shadow in run.Shadows)
            {
                ArgumentNullException.ThrowIfNull(shadow);
                writer.WriteUInt32(shadow.Color);
                WriteFiniteSingle(writer, shadow.DeltaX, "run shadow x");
                WriteFiniteSingle(writer, shadow.DeltaY, "run shadow y");
                WriteNonnegativeSingle(writer, shadow.BlurRadius, "run shadow blur radius");
            }
            writer.WriteUInt32(CheckedCount(run.FontFeatures.Count, "run font feature"));
            foreach (var feature in run.FontFeatures)
            {
                ArgumentNullException.ThrowIfNull(feature);
                writer.WriteUInt32(context.StringId(feature.Name));
                writer.WriteInt32(feature.Value);
            }
            writer.WriteUInt32(CheckedCount(run.FontVariations.Count, "run font variation"));
            foreach (var variation in run.FontVariations)
            {
                ArgumentNullException.ThrowIfNull(variation);
                writer.WriteUInt32(context.StringId(variation.Axis));
                WriteFiniteSingle(writer, variation.Value, "run font variation");
            }
        }
    }

    private static void WriteOptionalUInt32(DisplayListBinaryWriter writer, uint? value)
    {
        writer.WriteBoolean(value.HasValue);
        if (value.HasValue) writer.WriteUInt32(value.Value);
    }

    private static void WriteOptionalEnumByte<T>(
        DisplayListBinaryWriter writer,
        T? value,
        string name) where T : struct, Enum
    {
        writer.WriteBoolean(value.HasValue);
        if (value.HasValue) WriteEnumByte(writer, value.Value, name);
    }

    private static void WriteOptionalFiniteSingle(
        DisplayListBinaryWriter writer,
        float? value,
        string name)
    {
        writer.WriteBoolean(value.HasValue);
        if (value.HasValue) WriteFiniteSingle(writer, value.Value, name);
    }

    private static void WriteOptionalNonnegativeSingle(
        DisplayListBinaryWriter writer,
        float? value,
        string name)
    {
        writer.WriteBoolean(value.HasValue);
        if (value.HasValue) WriteNonnegativeSingle(writer, value.Value, name);
    }

    private static void WriteOptionalPaint(
        DisplayListBinaryWriter writer,
        DisplayPaint? paint,
        EncoderContext context,
        int depth)
    {
        writer.WriteBoolean(paint is not null);
        if (paint is not null)
        {
            WritePaint(writer, paint, context, depth);
        }
    }

    private static void WritePaint(
        DisplayListBinaryWriter writer,
        DisplayPaint paint,
        EncoderContext context,
        int depth)
    {
        ArgumentNullException.ThrowIfNull(paint);
        RequireDepth(depth);
        writer.WriteUInt32(paint.Color);
        WriteEnumByte(writer, paint.Style, "paint style");
        WriteEnumByte(writer, paint.StrokeCap, "stroke cap");
        WriteEnumByte(writer, paint.StrokeJoin, "stroke join");
        writer.WriteBoolean(paint.IsAntiAlias);
        WriteEnumByte(writer, paint.BlendMode, "blend mode");
        WriteEnumByte(writer, paint.Sampling, "sampling quality");
        writer.WriteBoolean(paint.InvertColors);
        writer.WriteByte(0);
        WriteNonnegativeSingle(writer, paint.StrokeWidth, nameof(paint.StrokeWidth));
        WriteNonnegativeSingle(writer, paint.StrokeMiterLimit, nameof(paint.StrokeMiterLimit));
        WriteShader(writer, paint.Shader, context, depth + 1, true);
        WriteColorFilter(writer, paint.ColorFilter, context, depth + 1, true);
        WriteMaskFilter(writer, paint.MaskFilter);
        WriteImageFilter(writer, paint.ImageFilter, context, depth + 1, true);
    }

    private static void WriteShader(
        DisplayListBinaryWriter writer,
        DisplayShader? shader,
        EncoderContext context,
        int depth,
        bool allowNull)
    {
        RequireDepth(depth);
        switch (shader)
        {
            case null when allowNull:
                writer.WriteByte(0);
                return;
            case null:
                throw new ArgumentNullException(nameof(shader));
            case DisplayLinearGradientShader value:
                writer.WriteByte(1);
                WritePoint(writer, value.Start);
                WritePoint(writer, value.End);
                WriteEnumByte(writer, value.TileMode, "tile mode");
                WriteGradient(writer, value.Colors, value.Stops);
                WriteOptionalMatrix(writer, value.Transform);
                return;
            case DisplayRadialGradientShader value:
                writer.WriteByte(2);
                WritePoint(writer, value.Center);
                WriteNonnegativeSingle(writer, value.Radius, nameof(value.Radius));
                WriteEnumByte(writer, value.TileMode, "tile mode");
                writer.WriteBoolean(value.Focal is not null);
                if (value.Focal is { } focal)
                {
                    WritePoint(writer, focal);
                }

                WriteNonnegativeSingle(writer, value.FocalRadius, nameof(value.FocalRadius));
                WriteGradient(writer, value.Colors, value.Stops);
                WriteOptionalMatrix(writer, value.Transform);
                return;
            case DisplaySweepGradientShader value:
                writer.WriteByte(3);
                WritePoint(writer, value.Center);
                WriteFiniteSingle(writer, value.StartAngle, nameof(value.StartAngle));
                WriteFiniteSingle(writer, value.EndAngle, nameof(value.EndAngle));
                WriteEnumByte(writer, value.TileMode, "tile mode");
                WriteGradient(writer, value.Colors, value.Stops);
                WriteOptionalMatrix(writer, value.Transform);
                return;
            case DisplayImageShader value:
                writer.WriteByte(4);
                context.WriteResource(writer, value.Image, DisplayResourceKind.Image);
                WriteEnumByte(writer, value.TileModeX, "horizontal tile mode");
                WriteEnumByte(writer, value.TileModeY, "vertical tile mode");
                WriteEnumByte(writer, value.Sampling, "sampling quality");
                writer.WriteByte(0);
                WriteMatrix(writer, value.Transform);
                return;
            case DisplayRuntimeEffectShader value:
                writer.WriteByte(5);
                context.WriteResource(writer, value.Effect, DisplayResourceKind.RuntimeEffect);
                writer.WriteUInt32(CheckedCount(value.Uniforms.Count, "runtime-effect uniform byte"));
                writer.WriteBytes(value.Uniforms.ToArray());
                writer.WriteUInt32(CheckedCount(value.Children.Count, "runtime-effect child"));
                foreach (var child in value.Children)
                {
                    context.WriteResource(writer, child, null);
                }

                return;
            default:
                throw new ArgumentException($"Shader type {shader.GetType().FullName} is not supported by DisplayList v2.", nameof(shader));
        }
    }

    private static void WriteGradient(
        DisplayListBinaryWriter writer,
        IReadOnlyList<uint> colors,
        IReadOnlyList<float> stops)
    {
        if (colors.Count < 2 || colors.Count != stops.Count)
        {
            throw new ArgumentException("A gradient requires matching color and stop arrays with at least two entries.", nameof(colors));
        }

        writer.WriteUInt32(CheckedCount(colors.Count, "gradient stop"));
        var previousStop = float.NegativeInfinity;
        for (var index = 0; index < colors.Count; index++)
        {
            var stop = stops[index];
            if (!float.IsFinite(stop) || stop < previousStop)
            {
                throw new ArgumentException("Gradient stops must be finite and nondecreasing.", nameof(stops));
            }

            writer.WriteUInt32(colors[index]);
            writer.WriteSingle(stop);
            previousStop = stop;
        }
    }

    private static void WriteColorFilter(
        DisplayListBinaryWriter writer,
        DisplayColorFilter? filter,
        EncoderContext context,
        int depth,
        bool allowNull)
    {
        _ = context;
        RequireDepth(depth);
        switch (filter)
        {
            case null when allowNull:
                writer.WriteByte(0);
                return;
            case null:
                throw new ArgumentNullException(nameof(filter));
            case DisplayBlendColorFilter value:
                writer.WriteByte(1);
                writer.WriteUInt32(value.Color);
                WriteEnumByte(writer, value.BlendMode, "blend mode");
                return;
            case DisplayMatrixColorFilter value:
                writer.WriteByte(2);
                foreach (var item in value.Values)
                {
                    WriteFiniteSingle(writer, item, "color matrix");
                }

                return;
            case DisplayLinearToSrgbColorFilter:
                writer.WriteByte(3);
                return;
            case DisplaySrgbToLinearColorFilter:
                writer.WriteByte(4);
                return;
            default:
                throw new ArgumentException($"Color-filter type {filter.GetType().FullName} is not supported by DisplayList v2.", nameof(filter));
        }
    }

    private static void WriteMaskFilter(DisplayListBinaryWriter writer, DisplayMaskFilter? filter)
    {
        writer.WriteBoolean(filter is not null);
        if (filter is null)
        {
            return;
        }

        WriteEnumByte(writer, filter.Style, "blur style");
        WriteNonnegativeSingle(writer, filter.Sigma, nameof(filter.Sigma));
    }

    private static void WriteImageFilter(
        DisplayListBinaryWriter writer,
        DisplayImageFilter? filter,
        EncoderContext context,
        int depth,
        bool allowNull)
    {
        RequireDepth(depth);
        switch (filter)
        {
            case null when allowNull:
                writer.WriteByte(0);
                return;
            case null:
                throw new ArgumentNullException(nameof(filter));
            case DisplayBlurImageFilter value:
                writer.WriteByte(1);
                WriteNonnegativeSingle(writer, value.SigmaX, nameof(value.SigmaX));
                WriteNonnegativeSingle(writer, value.SigmaY, nameof(value.SigmaY));
                WriteEnumByte(writer, value.TileMode, "tile mode");
                WriteOptionalRect(writer, value.Bounds);
                return;
            case DisplayColorImageFilter value:
                writer.WriteByte(2);
                WriteColorFilter(writer, value.Filter, context, depth + 1, false);
                return;
            case DisplayMatrixImageFilter value:
                writer.WriteByte(3);
                WriteMatrix(writer, value.Matrix);
                WriteEnumByte(writer, value.Sampling, "sampling quality");
                return;
            case DisplayRuntimeEffectImageFilter value:
                writer.WriteByte(4);
                WriteShader(writer, value.Shader, context, depth + 1, false);
                WriteEnumByte(writer, value.Sampling, "sampling quality");
                return;
            case DisplayComposeImageFilter value:
                writer.WriteByte(5);
                WriteImageFilter(writer, value.Outer, context, depth + 1, false);
                WriteImageFilter(writer, value.Inner, context, depth + 1, false);
                return;
            case DisplayDropShadowImageFilter value:
                writer.WriteByte(6);
                WriteFiniteSingle(writer, value.DeltaX, nameof(value.DeltaX));
                WriteFiniteSingle(writer, value.DeltaY, nameof(value.DeltaY));
                WriteNonnegativeSingle(writer, value.SigmaX, nameof(value.SigmaX));
                WriteNonnegativeSingle(writer, value.SigmaY, nameof(value.SigmaY));
                writer.WriteUInt32(value.Color);
                writer.WriteBoolean(value.ShadowOnly);
                return;
            default:
                throw new ArgumentException($"Image-filter type {filter.GetType().FullName} is not supported by DisplayList v2.", nameof(filter));
        }
    }

    private static void WritePath(DisplayListBinaryWriter writer, DisplayPath path)
    {
        ArgumentNullException.ThrowIfNull(path);
        if (!Enum.IsDefined(path.FillType))
        {
            throw new ArgumentOutOfRangeException(nameof(path), "The path fill type is not defined.");
        }

        writer.WriteByte((byte)path.FillType);
        writer.WriteByte(0);
        writer.WriteUInt16(0);
        writer.WriteUInt32(CheckedCount(path.Verbs.Count, "path verb"));
        writer.WriteUInt32(CheckedCount(path.Values.Count, "path value"));
        var expectedValueCount = 0L;
        foreach (var verb in path.Verbs)
        {
            if (!Enum.IsDefined(verb))
            {
                throw new ArgumentOutOfRangeException(nameof(path), "The path contains an unknown verb.");
            }

            writer.WriteByte((byte)verb);
            expectedValueCount += ValuesForVerb(verb);
        }

        if (expectedValueCount != path.Values.Count)
        {
            throw new ArgumentException(
                $"The path verb stream requires {expectedValueCount} values but contains {path.Values.Count}.",
                nameof(path));
        }

        foreach (var value in path.Values)
        {
            WriteFiniteSingle(writer, value, "path value");
        }
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

    private static void WritePoint(DisplayListBinaryWriter writer, DisplayPoint point)
    {
        WriteFiniteSingle(writer, point.X, nameof(point.X));
        WriteFiniteSingle(writer, point.Y, nameof(point.Y));
    }

    private static void WriteRect(DisplayListBinaryWriter writer, DisplayRect rect)
    {
        WriteFiniteSingle(writer, rect.Left, nameof(rect.Left));
        WriteFiniteSingle(writer, rect.Top, nameof(rect.Top));
        WriteFiniteSingle(writer, rect.Right, nameof(rect.Right));
        WriteFiniteSingle(writer, rect.Bottom, nameof(rect.Bottom));
    }

    private static void WriteOptionalRect(DisplayListBinaryWriter writer, DisplayRect? rect)
    {
        writer.WriteBoolean(rect is not null);
        if (rect is { } value)
        {
            WriteRect(writer, value);
        }
    }

    private static void WriteRoundedRect(DisplayListBinaryWriter writer, DisplayRoundedRect roundedRect)
    {
        WriteRect(writer, roundedRect.Bounds);
        WriteNonnegativeSingle(writer, roundedRect.TopLeftX, nameof(roundedRect.TopLeftX));
        WriteNonnegativeSingle(writer, roundedRect.TopLeftY, nameof(roundedRect.TopLeftY));
        WriteNonnegativeSingle(writer, roundedRect.TopRightX, nameof(roundedRect.TopRightX));
        WriteNonnegativeSingle(writer, roundedRect.TopRightY, nameof(roundedRect.TopRightY));
        WriteNonnegativeSingle(writer, roundedRect.BottomRightX, nameof(roundedRect.BottomRightX));
        WriteNonnegativeSingle(writer, roundedRect.BottomRightY, nameof(roundedRect.BottomRightY));
        WriteNonnegativeSingle(writer, roundedRect.BottomLeftX, nameof(roundedRect.BottomLeftX));
        WriteNonnegativeSingle(writer, roundedRect.BottomLeftY, nameof(roundedRect.BottomLeftY));
    }

    private static void WriteMatrix(DisplayListBinaryWriter writer, DisplayMatrix matrix)
    {
        ArgumentNullException.ThrowIfNull(matrix);
        foreach (var value in matrix.Values)
        {
            WriteFiniteSingle(writer, value, "matrix value");
        }
    }

    private static void WriteOptionalMatrix(DisplayListBinaryWriter writer, DisplayMatrix? matrix)
    {
        writer.WriteBoolean(matrix is not null);
        if (matrix is not null)
        {
            WriteMatrix(writer, matrix);
        }
    }

    private static void WriteFiniteSingle(DisplayListBinaryWriter writer, float value, string name)
    {
        if (!float.IsFinite(value))
        {
            throw new ArgumentOutOfRangeException(name, "DisplayList floats must be finite.");
        }

        writer.WriteSingle(value);
    }

    private static void WritePositiveSingle(DisplayListBinaryWriter writer, float value, string name)
    {
        if (!float.IsFinite(value) || value <= 0)
        {
            throw new ArgumentOutOfRangeException(name, "The value must be finite and positive.");
        }

        writer.WriteSingle(value);
    }

    private static void WriteNonnegativeSingle(DisplayListBinaryWriter writer, float value, string name)
    {
        if (!float.IsFinite(value) || value < 0)
        {
            throw new ArgumentOutOfRangeException(name, "The value must be finite and nonnegative.");
        }

        writer.WriteSingle(value);
    }

    private static void WriteUnitSingle(DisplayListBinaryWriter writer, float value, string name)
    {
        if (!float.IsFinite(value) || value is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(name, "The value must be between zero and one.");
        }

        writer.WriteSingle(value);
    }

    private static void WriteEnumByte<TEnum>(
        DisplayListBinaryWriter writer,
        TEnum value,
        string name)
        where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(value))
        {
            throw new ArgumentOutOfRangeException(name, $"The {name} value is not defined.");
        }

        // All byte-valued wire enums declare ': byte'. Avoid boxing each paint,
        // clip and paragraph enum through IConvertible on the WASM hot path.
        // BitCast checks equal sizes; the IsDefined guard above still rejects
        // unknown values, and the encoded byte remains identical.
        writer.WriteByte(Unsafe.BitCast<TEnum, byte>(value));
    }

    private static uint CheckedCount(int count, string name)
    {
        if (count < 0 || count > DisplayListFormat.MaximumCollectionCount)
        {
            throw new ArgumentOutOfRangeException(name, $"The DisplayList {name} count exceeds the format limit.");
        }

        return checked((uint)count);
    }

    private static void RequireDepth(int depth)
    {
        if (depth > DisplayListFormat.MaximumNestingDepth)
        {
            throw new ArgumentException("The DisplayList tagged-value nesting limit was exceeded.");
        }
    }

    private static void ValidateScene(DisplayListSceneMetadata scene)
    {
        if (scene.ViewId == 0 || scene.SceneSequence == 0 || scene.BuildToken == 0 ||
            scene.ResizeEpoch == 0 || scene.SurfaceGeneration == 0 || scene.ContextGeneration == 0)
        {
            throw new ArgumentException("DisplayList scene identities and generations must be nonzero.", nameof(scene));
        }

        if (!float.IsFinite(scene.LogicalWidth) || scene.LogicalWidth <= 0 ||
            !float.IsFinite(scene.LogicalHeight) || scene.LogicalHeight <= 0 ||
            scene.PhysicalWidth == 0 || scene.PhysicalHeight == 0 ||
            !float.IsFinite(scene.DevicePixelRatio) || scene.DevicePixelRatio <= 0)
        {
            throw new ArgumentException("DisplayList scene geometry must be finite and positive.", nameof(scene));
        }
    }

    private static void ValidateResourceReference(
        DisplayResourceReference reference,
        DisplayResourceKind? expectedKind)
    {
        if (!Enum.IsDefined(reference.Kind) || reference.Id == 0 || reference.Version == 0)
        {
            throw new ArgumentException($"DisplayList resource reference {reference} is invalid.", nameof(reference));
        }

        if (expectedKind is not null && reference.Kind != expectedKind)
        {
            throw new ArgumentException(
                $"DisplayList resource {reference} must have kind {expectedKind}.",
                nameof(reference));
        }
    }

    private sealed class EncoderContext(
        IReadOnlyDictionary<DisplayResourceReference, DisplayResourceDescriptor> resources,
        IReadOnlyDictionary<string, uint> stringIds)
    {
        internal void WriteResource(
            DisplayListBinaryWriter writer,
            DisplayResourceReference reference,
            DisplayResourceKind? expectedKind)
        {
            ValidateResourceReference(reference, expectedKind);
            if (!resources.ContainsKey(reference))
            {
                throw new ArgumentException($"DisplayList resource {reference} is referenced but not declared.", nameof(reference));
            }

            writer.WriteUInt16((ushort)reference.Kind);
            writer.WriteUInt16(0);
            writer.WriteUInt32(reference.Version);
            writer.WriteUInt64(reference.Id);
        }

        internal uint StringId(string value)
        {
            if (!stringIds.TryGetValue(value, out var id))
            {
                throw new ArgumentException("DisplayList string was not collected into the canonical string table.", nameof(value));
            }

            return id;
        }
    }
}
