// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/_web_image_info_io.dart
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

public class WebImageInfoIo : ImageInfo
{
    public override ImageInfo clone() => throw _unsupported();
    public override string? debugLabel => throw _unsupported();
    public override void dispose() => throw _unsupported();
    public override Image image => throw _unsupported();
    public override bool isCloneOf(ImageInfo other) => throw _unsupported();
    public override double scale => throw _unsupported();
    public override long sizeBytes => throw _unsupported();
    internal virtual NotSupportedException _unsupported() => new NotSupportedException("WebImageInfo should never be instantiated in a non-web context.");
}

