// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/_web_image_io.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public class RawWebImageIo : StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Painting.WebImageInfoIo image { get; private set; } = default!;
    public virtual string? debugImageLabel { get; private set; }
    public virtual double? width { get; private set; }
    public virtual double? height { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.BoxFit? fit { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;
    public virtual bool matchTextDirection { get; private set; } = default!;

    public RawWebImageIo(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.WebImageInfoIo image = default!, string? debugImageLabel = null, double? width = null, double? height = null, global::Doroti.Generated.Framework.Painting.BoxFit? fit = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, bool matchTextDirection = false) : base(key: key)
    {
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center;
        this.image = image;
        this.debugImageLabel = debugImageLabel;
        this.width = width;
        this.height = height;
        this.fit = fit;
        this.alignment = __alignment;
        this.matchTextDirection = matchTextDirection;
    }

    public override Widget build(BuildContext context)
    {
        throw new NotSupportedException("It is impossible to instantiate a RawWebImage when not running on the web");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

