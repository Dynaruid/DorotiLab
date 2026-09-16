// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/form_section.dart

using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class Form_sectionLibrary
{
    internal static EdgeInsetsDirectional _kFormDefaultInsetGroupedRowsMargin = new EdgeInsetsDirectional(20.0, 0.0, 20.0, 10.0);
}

public class CupertinoFormSection : StatelessWidget
{
    internal virtual CupertinoListSectionType _type { get; private set; } = default!;
    public virtual Widget? header { get; private set; }
    public virtual Widget? footer { get; private set; }
    public virtual EdgeInsetsGeometry margin { get; private set; } = default!;
    public virtual List<Widget> children { get; private set; } = default!;
    public virtual BoxDecoration? decoration { get; private set; }
    public virtual Color backgroundColor { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;

    public CupertinoFormSection(Key? key = null, List<Widget> children = default!, Widget? header = null, Widget? footer = null, EdgeInsetsGeometry margin = default!, Color backgroundColor = default!, BoxDecoration? decoration = null, Clip clipBehavior = Clip.none) : base(key: key)
    {
        EdgeInsetsGeometry __margin = margin ?? EdgeInsets.zero;
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

    public static CupertinoFormSection CreateInsetGrouped(Key? key = null, List<Widget> children = default!, Widget? header = null, Widget? footer = null, EdgeInsetsGeometry margin = default!, Color backgroundColor = default!, BoxDecoration? decoration = null, Clip clipBehavior = Clip.none)
    {
        var __instance = new CupertinoFormSection(key: key, children: children, header: header, footer: footer, margin: margin, backgroundColor: backgroundColor, decoration: decoration, clipBehavior: clipBehavior);
        EdgeInsetsGeometry __margin = margin ?? Form_sectionLibrary._kFormDefaultInsetGroupedRowsMargin;
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

    public override Widget build(BuildContext context)
    {
        Widget? headerWidget = (header is null) ? null : new DefaultTextStyle(style: new TextStyle(fontSize: 13.0, color: CupertinoColors.secondaryLabel.resolveFrom(context)), child: header!);
        Widget? footerWidget = (footer is null) ? null : new DefaultTextStyle(style: new TextStyle(fontSize: 13.0, color: CupertinoColors.secondaryLabel.resolveFrom(context)), child: footer!);
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
