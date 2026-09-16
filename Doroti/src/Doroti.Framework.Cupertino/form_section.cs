// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/form_section.dart

using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class Form_sectionLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsetsDirectional _kFormDefaultInsetGroupedRowsMargin = new global::Doroti.Framework.Painting.EdgeInsetsDirectional(20.0, 0.0, 20.0, 10.0);
}

public class CupertinoFormSection : global::Doroti.Framework.Widgets.StatelessWidget
{
    internal virtual CupertinoListSectionType _type { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? header { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? footer { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry margin { get; private set; } = default!;
    public virtual List<global::Doroti.Framework.Widgets.Widget> children { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.BoxDecoration? decoration { get; private set; }
    public virtual Color backgroundColor { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;

    public CupertinoFormSection(global::Doroti.Framework.Foundation.Key? key = null, List<global::Doroti.Framework.Widgets.Widget> children = default!, global::Doroti.Framework.Widgets.Widget? header = null, global::Doroti.Framework.Widgets.Widget? footer = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry margin = default!, Color backgroundColor = default!, global::Doroti.Framework.Painting.BoxDecoration? decoration = null, Clip clipBehavior = Clip.none) : base(key: key)
    {
        global::Doroti.Framework.Painting.EdgeInsetsGeometry __margin = margin ?? EdgeInsets.zero;
        Color __backgroundColor = backgroundColor ?? CupertinoColors.systemGroupedBackground;
        this.children = children;
        this.header = header;
        this.footer = footer;
        this.margin = __margin;
        this.backgroundColor = __backgroundColor;
        this.decoration = decoration;
        this.clipBehavior = clipBehavior;
        _type = CupertinoListSectionType.@base;
        System.Diagnostics.Debug.Assert(checked(children.Count) > 0L);
    }

    public static CupertinoFormSection CreateInsetGrouped(global::Doroti.Framework.Foundation.Key? key = null, List<global::Doroti.Framework.Widgets.Widget> children = default!, global::Doroti.Framework.Widgets.Widget? header = null, global::Doroti.Framework.Widgets.Widget? footer = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry margin = default!, Color backgroundColor = default!, global::Doroti.Framework.Painting.BoxDecoration? decoration = null, Clip clipBehavior = Clip.none)
    {
        var __instance = new CupertinoFormSection(key: key, children: children, header: header, footer: footer, margin: margin, backgroundColor: backgroundColor, decoration: decoration, clipBehavior: clipBehavior);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry __margin = margin ?? Form_sectionLibrary._kFormDefaultInsetGroupedRowsMargin;
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

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Widgets.Widget? headerWidget = (header is null) ? null : new global::Doroti.Framework.Widgets.DefaultTextStyle(style: new global::Doroti.Framework.Painting.TextStyle(fontSize: 13.0, color: CupertinoColors.secondaryLabel.resolveFrom(context)), child: header!);
        global::Doroti.Framework.Widgets.Widget? footerWidget = (footer is null) ? null : new global::Doroti.Framework.Widgets.DefaultTextStyle(style: new global::Doroti.Framework.Painting.TextStyle(fontSize: 13.0, color: CupertinoColors.secondaryLabel.resolveFrom(context)), child: footer!);
        switch (_type)
        {
            case var __constant9391 when Equals(__constant9391, CupertinoListSectionType.@base):
                {
                    return new CupertinoListSection(header: headerWidget, footer: footerWidget, margin: margin, backgroundColor: backgroundColor, decoration: decoration, clipBehavior: clipBehavior, hasLeading: false, children: children);
                }
            case var __constant9746 when Equals(__constant9746, CupertinoListSectionType.insetGrouped):
                {
                    return new CupertinoListSection(header: headerWidget, footer: footerWidget, margin: margin, backgroundColor: backgroundColor, decoration: decoration, clipBehavior: clipBehavior, hasLeading: false, children: children);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
