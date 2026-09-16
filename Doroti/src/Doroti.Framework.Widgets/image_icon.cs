// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/image_icon.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class ImageIcon : StatelessWidget
{
    public virtual global::Doroti.Framework.Painting.IImageProvider image { get; private set; } = default!;
    public virtual double? size { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual string? semanticLabel { get; private set; }
    public virtual bool useOriginalColors { get; private set; } = default!;

    public ImageIcon(global::Doroti.Framework.Painting.IImageProvider image, global::Doroti.Framework.Foundation.Key? key = null, double? size = null, Color? color = null, string? semanticLabel = null, bool useOriginalColors = false) : base(key: key)
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
            return new Semantics(label: semanticLabel, child: new SizedBox(width: iconSize, height: iconSize));
        }
        double? iconOpacity = iconTheme.opacity;
        global::Doroti.Ui.Color iconColor = color ?? iconTheme.color!;
        if ((iconOpacity is not null) && (DartRuntimePrimitives.RequireValue(iconOpacity) != 1.0))
        {
            double iconOpacity__3341__value3432 = DartRuntimePrimitives.RequireValue(iconOpacity);
            iconColor = iconColor.withOpacity(iconColor.opacity * DartRuntimePrimitives.RequireValue(iconOpacity__3341__value3432));
        }
        return new Semantics(label: semanticLabel, child: new Image(image: image!, width: iconSize, height: iconSize, color: useOriginalColors ? null : iconColor, fit: BoxFit.scaleDown, excludeFromSemantics: true));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<object>("image", image, ifNull: "<empty>", showName: false));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("size", size, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("color", color, defaultValue: null));
    }

}

