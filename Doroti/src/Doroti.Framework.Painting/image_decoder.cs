// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/image_decoder.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Painting;

public static partial class Image_decoderLibrary
{
    public static async Future<Image> decodeImageFromList(Uint8List bytes)
    {
        global::Doroti.Ui.ImmutableBuffer buffer = await Dart_uiLibrary.ImmutableBuffer.fromUint8List(bytes);
        global::Doroti.Ui.Codec codec = await PaintingBinding.instance.instantiateImageCodecWithSize(buffer);
        global::Doroti.Ui.FrameInfo frameInfo = default!;
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

