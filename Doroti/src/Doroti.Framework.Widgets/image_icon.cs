// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/image_icon.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class ImageIcon : StatelessWidget
{
    public virtual IImageProvider image { get; private set; } = default!;
    public virtual double? size { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual string? semanticLabel { get; private set; }
    public virtual bool useOriginalColors { get; private set; } = default!;

    public ImageIcon(
        IImageProvider image,
        Key? key = null,
        double? size = null,
        Color? color = null,
        string? semanticLabel = null,
        bool useOriginalColors = false
    )
        : base(key: key)
    {
        this.image = image;
        this.size = size;
        this.color = color;
        this.semanticLabel = semanticLabel;
        this.useOriginalColors = useOriginalColors;
        System.Diagnostics.Debug.Assert(!(useOriginalColors && (color is not null)));
    }

    public override Widget build(BuildContext context)
    {
        IconThemeData iconTheme = IconTheme.of(context);
        double? iconSize = size ?? iconTheme.size;
        if (image is null)
        {
            return new Semantics(
                label: semanticLabel,
                child: new SizedBox(width: iconSize, height: iconSize)
            );
        }
        double? iconOpacity = iconTheme.opacity;
        Color iconColor = color ?? iconTheme.color!;
        if (
            (iconOpacity is not null)
            && (
                (
                    iconOpacity
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ) != 1.0
            )
        )
        {
            double iconOpacity__3341__value3432 = (
                iconOpacity
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            iconColor = iconColor.withOpacity(iconColor.opacity * (iconOpacity__3341__value3432));
        }
        return new Semantics(
            label: semanticLabel,
            child: new Image(
                image: image!,
                width: iconSize,
                height: iconSize,
                color: useOriginalColors ? null : iconColor,
                fit: BoxFit.scaleDown,
                excludeFromSemantics: true
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new DiagnosticsProperty<object>("image", image, ifNull: "<empty>", showName: false)
        );
        properties.add(new DoubleProperty("size", size, defaultValue: null));
        properties.add(new ColorProperty("color", color, defaultValue: null));
    }
}
