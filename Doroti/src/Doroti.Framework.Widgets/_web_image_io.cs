// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/_web_image_io.dart
namespace Doroti.Framework.Widgets;

public class RawWebImageIo : StatelessWidget
{
    public virtual global::Doroti.Framework.Painting.WebImageInfoIo image { get; private set; } = default!;
    public virtual string? debugImageLabel { get; private set; }
    public virtual double? width { get; private set; }
    public virtual double? height { get; private set; }
    public virtual global::Doroti.Framework.Painting.BoxFit? fit { get; private set; }
    public virtual global::Doroti.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;
    public virtual bool matchTextDirection { get; private set; } = default!;

    public RawWebImageIo(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.WebImageInfoIo image = default!, string? debugImageLabel = null, double? width = null, double? height = null, global::Doroti.Framework.Painting.BoxFit? fit = null, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, bool matchTextDirection = false) : base(key: key)
    {
        global::Doroti.Framework.Painting.AlignmentGeometry __alignment = alignment ?? Alignment.center;
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

