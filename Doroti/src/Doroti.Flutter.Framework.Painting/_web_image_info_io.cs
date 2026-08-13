// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/_web_image_info_io.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Painting;

public class WebImageInfoIo : ImageInfo
{
    public override ImageInfo clone() => _unsupported();
    public override string? debugLabel => _unsupported();
    public override void dispose() => _unsupported();
    public override Image image => _unsupported();
    public override bool isCloneOf(ImageInfo other) => _unsupported();
    public override double scale => _unsupported();
    public override long sizeBytes => _unsupported();
    internal virtual dynamic _unsupported() => throw new NotSupportedException("WebImageInfo should never be instantiated in a non-web context.");
}

