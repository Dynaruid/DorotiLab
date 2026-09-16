// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/flutter_logo.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class FlutterLogo : StatelessWidget
{
    public virtual double? size { get; private set; }
    public virtual Color textColor { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.FlutterLogoStyle style { get; private set; } = default!;
    public virtual Duration duration { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Curve curve { get; private set; } = default!;

    public FlutterLogo(global::Doroti.Framework.Foundation.Key? key = null, double? size = null, Color textColor = default!, global::Doroti.Framework.Painting.FlutterLogoStyle style = FlutterLogoStyle.markOnly, Duration? duration = null, global::Doroti.Framework.Animation.Curve curve = default!) : base(key: key)
    {
        Color __textColor = textColor ?? new Color(0xFF757575);
        Duration __duration = duration ?? Duration.Create(milliseconds: 750);
        global::Doroti.Framework.Animation.Curve __curve = curve ?? Curves.fastOutSlowIn;
        this.size = size;
        this.textColor = __textColor;
        this.style = style;
        this.duration = __duration;
        this.curve = __curve;
    }

    public override Widget build(BuildContext context)
    {
        IconThemeData iconTheme = IconTheme.of(context);
        double? iconSize = size ?? iconTheme.size;
        return new AnimatedContainer(width: iconSize, height: iconSize, duration: DartRuntimePrimitives.RequireValue(duration), curve: curve, decoration: new global::Doroti.Framework.Painting.FlutterLogoDecoration(style: style, textColor: textColor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

