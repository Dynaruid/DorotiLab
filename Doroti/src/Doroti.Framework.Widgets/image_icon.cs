// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/image_icon.dart
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

namespace Doroti.Framework.Widgets;

public class ImageIcon : StatelessWidget
{
    public virtual dynamic image { get; private set; } = default!;
    public virtual double? size { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual string? semanticLabel { get; private set; }
    public virtual bool useOriginalColors { get; private set; } = default!;

    public ImageIcon(dynamic image, global::Doroti.Framework.Foundation.Key? key = null, double? size = null, Color? color = null, string? semanticLabel = null, bool useOriginalColors = false) : base(key: key)
    {
        this.image = image;
        this.size = size;
        this.color = color;
        this.semanticLabel = semanticLabel;
        this.useOriginalColors = useOriginalColors;
        System.Diagnostics.Debug.Assert(!((useOriginalColors && (color is not null))));
    }

    public override Widget build(BuildContext context)
    {
        IconThemeData iconTheme__3079 = ((IconThemeData)(object?)IconTheme.of(context));
        double? iconSize__3132 = (this.size ?? ((IconThemeData)iconTheme__3079).size);
        if ((this.image is null))
        {
            return ((Widget)(object?)new Semantics(label: this.semanticLabel, child: new SizedBox(width: iconSize__3132, height: iconSize__3132)));
        }
        double? iconOpacity__3341 = ((IconThemeData)iconTheme__3079).opacity;
        global::Doroti.Ui.Color iconColor__3384 = ((global::Doroti.Ui.Color)(object?)(this.color ?? ((IconThemeData)iconTheme__3079).color!));
        if (((iconOpacity__3341 is not null) && (DartRuntimePrimitives.RequireValue(iconOpacity__3341) != 1.0)))
        {
            double iconOpacity__3341__value3432 = DartRuntimePrimitives.RequireValue(iconOpacity__3341);
            iconColor__3384 = iconColor__3384.withOpacity((iconColor__3384.opacity * DartRuntimePrimitives.RequireValue(iconOpacity__3341__value3432)));
        }
        return ((Widget)(object?)new Semantics(label: this.semanticLabel, child: new Image(image: this.image!, width: iconSize__3132, height: iconSize__3132, color: (this.useOriginalColors ? null : iconColor__3384), fit: global::Doroti.Framework.Painting.BoxFit.scaleDown, excludeFromSemantics: true)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<object>("image", this.image, ifNull: "<empty>", showName: false));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("size", this.size, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("color", this.color, defaultValue: null));
    }

}

