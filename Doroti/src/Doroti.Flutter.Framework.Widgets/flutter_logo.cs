// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/flutter_logo.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public class FlutterLogo : StatelessWidget
{
    public virtual double? size { get; private set; }
    public virtual Color textColor { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.FlutterLogoStyle style { get; private set; } = default!;
    public virtual Duration duration { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Curve curve { get; private set; } = default!;

    public FlutterLogo(global::Doroti.Generated.Framework.Foundation.Key? key = null, double? size = null, Color textColor = default!, global::Doroti.Generated.Framework.Painting.FlutterLogoStyle style = global::Doroti.Generated.Framework.Painting.FlutterLogoStyle.markOnly, Duration? duration = null, global::Doroti.Generated.Framework.Animation.Curve curve = default!) : base(key: key)
    {
        Color __textColor = textColor ?? new Color(0xFF757575);
        Duration __duration = duration ?? Duration.Create(milliseconds: 750);
        global::Doroti.Generated.Framework.Animation.Curve __curve = curve ?? global::Doroti.Generated.Framework.Animation.Curves.fastOutSlowIn;
        this.size = size;
        this.textColor = __textColor;
        this.style = style;
        this.duration = __duration;
        this.curve = __curve;
    }

    public override Widget build(BuildContext context)
    {
        IconThemeData iconTheme__2364 = ((IconThemeData)(object?)IconTheme.of(context));
        double? iconSize__2417 = (this.size ?? ((IconThemeData)iconTheme__2364).size);
        return ((Widget)(object?)new AnimatedContainer(width: iconSize__2417, height: iconSize__2417, duration: DartRuntimePrimitives.RequireValue(this.duration), curve: this.curve, decoration: new global::Doroti.Generated.Framework.Painting.FlutterLogoDecoration(style: this.style, textColor: this.textColor)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

