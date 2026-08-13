using Doroti.Composition;
using SkiaSharp;

namespace Doroti.Backends.Skia;

public sealed class SkiaImageDecoder : IImageDecoder
{
    public ValueTask<DecodedImage> DecodeAsync(ReadOnlyMemory<byte> encodedBytes, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (encodedBytes.IsEmpty)
        {
            throw new ArgumentException("Encoded image bytes are required.", nameof(encodedBytes));
        }
        using var source = SKBitmap.Decode(encodedBytes.ToArray())
            ?? throw new InvalidDataException("Skia could not decode the image payload.");
        using var converted = new SKBitmap(new SKImageInfo(source.Width, source.Height, SKColorType.Bgra8888, SKAlphaType.Premul));
        using (var canvas = new SKCanvas(converted))
        {
            canvas.Clear(SKColors.Transparent);
            canvas.DrawBitmap(source, 0, 0);
            canvas.Flush();
        }
        cancellationToken.ThrowIfCancellationRequested();
        var image = new DecodedImage(converted.Width, converted.Height, converted.GetPixelSpan().ToArray());
        image.Validate();
        return ValueTask.FromResult(image);
    }
}
