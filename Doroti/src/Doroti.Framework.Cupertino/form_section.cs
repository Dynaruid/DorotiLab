// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/form_section.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Cupertino;

public static partial class Form_sectionLibrary
{
    internal static global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional _kFormDefaultInsetGroupedRowsMargin = new global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional(20.0, 0.0, 20.0, 10.0);
}

public class CupertinoFormSection : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    internal virtual CupertinoListSectionType _type { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? header { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? footer { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry margin { get; private set; } = default!;
    public virtual List<global::Doroti.Generated.Framework.Widgets.Widget> children { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.BoxDecoration? decoration { get; private set; }
    public virtual Color backgroundColor { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;

    public CupertinoFormSection(global::Doroti.Generated.Framework.Foundation.Key? key = null, List<global::Doroti.Generated.Framework.Widgets.Widget> children = default!, global::Doroti.Generated.Framework.Widgets.Widget? header = null, global::Doroti.Generated.Framework.Widgets.Widget? footer = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry margin = default!, Color backgroundColor = default!, global::Doroti.Generated.Framework.Painting.BoxDecoration? decoration = null, Clip clipBehavior = Clip.none) : base(key: key)
    {
        global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry __margin = margin ?? global::Doroti.Generated.Framework.Painting.EdgeInsets.zero;
        Color __backgroundColor = backgroundColor ?? CupertinoColors.systemGroupedBackground;
        this.children = children;
        this.header = header;
        this.footer = footer;
        this.margin = __margin;
        this.backgroundColor = __backgroundColor;
        this.decoration = decoration;
        this.clipBehavior = clipBehavior;
        this._type = CupertinoListSectionType.@base;
        System.Diagnostics.Debug.Assert((checked((long)(children.Count)) > 0L));
    }

    public static CupertinoFormSection CreateInsetGrouped(global::Doroti.Generated.Framework.Foundation.Key? key = null, List<global::Doroti.Generated.Framework.Widgets.Widget> children = default!, global::Doroti.Generated.Framework.Widgets.Widget? header = null, global::Doroti.Generated.Framework.Widgets.Widget? footer = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry margin = default!, Color backgroundColor = default!, global::Doroti.Generated.Framework.Painting.BoxDecoration? decoration = null, Clip clipBehavior = Clip.none)
    {
        var __instance = new CupertinoFormSection(key: key, children: children, header: header, footer: footer, margin: margin, backgroundColor: backgroundColor, decoration: decoration, clipBehavior: clipBehavior);
        global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry __margin = margin ?? Form_sectionLibrary._kFormDefaultInsetGroupedRowsMargin;
        Color __backgroundColor = backgroundColor ?? CupertinoColors.systemGroupedBackground;
        __instance.children = children;
        __instance.header = header;
        __instance.footer = footer;
        __instance.margin = __margin;
        __instance.backgroundColor = __backgroundColor;
        __instance.decoration = decoration;
        __instance.clipBehavior = clipBehavior;
        __instance._type = CupertinoListSectionType.insetGrouped;
        return __instance;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Generated.Framework.Widgets.Widget? headerWidget__8813 = ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)((this.header is null) ? null : new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: new global::Doroti.Generated.Framework.Painting.TextStyle(fontSize: 13.0, color: CupertinoColors.secondaryLabel.resolveFrom(context)), child: this.header!)));
        global::Doroti.Generated.Framework.Widgets.Widget? footerWidget__9095 = ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)((this.footer is null) ? null : new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: new global::Doroti.Generated.Framework.Painting.TextStyle(fontSize: 13.0, color: CupertinoColors.secondaryLabel.resolveFrom(context)), child: this.footer!)));
        switch (this._type)
        {
            case var __constant9391 when (object.Equals(__constant9391, CupertinoListSectionType.@base)):
                {
                    return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new CupertinoListSection(header: headerWidget__8813, footer: footerWidget__9095, margin: this.margin, backgroundColor: this.backgroundColor, decoration: this.decoration, clipBehavior: this.clipBehavior, hasLeading: false, children: this.children));
                }
            case var __constant9746 when (object.Equals(__constant9746, CupertinoListSectionType.insetGrouped)):
                {
                    return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new CupertinoListSection(header: headerWidget__8813, footer: footerWidget__9095, margin: this.margin, backgroundColor: this.backgroundColor, decoration: this.decoration, clipBehavior: this.clipBehavior, hasLeading: false, children: this.children));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
