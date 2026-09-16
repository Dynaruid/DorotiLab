// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/image_decoder.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public static partial class Image_decoderLibrary
{
    public static async Future<Image> decodeImageFromList(Uint8List bytes)
    {
        ImmutableBuffer buffer = await Dart_uiLibrary.ImmutableBuffer.fromUint8List(bytes);
        Codec codec = await PaintingBinding.instance.instantiateImageCodecWithSize(buffer);
        FrameInfo frameInfo = default!;
        try
        {
            frameInfo = await codec.getNextFrame();
        }
        finally
        {
            codec.dispose();
        }
        return frameInfo.image;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

