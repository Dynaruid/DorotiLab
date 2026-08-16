// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/bottom_tab_bar.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Cupertino;

public static partial class Bottom_tab_barLibrary
{
    internal static double _kTabBarHeight = 50.0;
}

public static partial class Bottom_tab_barLibrary
{
    internal static Color _kDefaultTabBarBorderColor = ((Color)(object?)new CupertinoDynamicColor(color: new global::Doroti.Ui.Color(1291845632L), darkColor: new global::Doroti.Ui.Color(687865856L)));
}

public static partial class Bottom_tab_barLibrary
{
    internal static Color _kDefaultTabBarInactiveColor = ((Color)(object?)CupertinoColors.inactiveGray);
}

public class CupertinoTabBar : global::Doroti.Framework.Widgets.StatelessWidget, global::Doroti.Framework.Widgets.PreferredSizeWidget
{
    public virtual List<global::Doroti.Framework.Widgets.BottomNavigationBarItem> items { get; private set; } = default!;
    public virtual global::System.Action<long>? onTap { get; private set; }
    public virtual long currentIndex { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? activeColor { get; private set; }
    public virtual Color inactiveColor { get; private set; } = default!;
    public virtual double iconSize { get; private set; } = default!;
    public virtual double height { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.Border? border { get; private set; }

    public CupertinoTabBar(global::Doroti.Framework.Foundation.Key? key = null, List<global::Doroti.Framework.Widgets.BottomNavigationBarItem> items = default!, global::System.Action<long>? onTap = null, long currentIndex = 0, Color? backgroundColor = null, Color? activeColor = null, Color inactiveColor = default!, double iconSize = 30.0, double? height = null, global::Doroti.Framework.Painting.Border? border = default!) : base(key: key)
    {
        Color __inactiveColor = inactiveColor ?? Bottom_tab_barLibrary._kDefaultTabBarInactiveColor;
        double __height = height ?? Bottom_tab_barLibrary._kTabBarHeight;
        global::Doroti.Framework.Painting.Border? __border = border ?? new global::Doroti.Framework.Painting.Border(top: new global::Doroti.Framework.Painting.BorderSide(color: Bottom_tab_barLibrary._kDefaultTabBarBorderColor, width: 0.0));
        this.items = items;
        this.onTap = onTap;
        this.currentIndex = currentIndex;
        this.backgroundColor = backgroundColor;
        this.activeColor = activeColor;
        this.inactiveColor = __inactiveColor;
        this.iconSize = iconSize;
        this.height = __height;
        this.border = __border;
        System.Diagnostics.Debug.Assert((checked((long)(items.Count)) >= 2L));
        System.Diagnostics.Debug.Assert(((0L <= DartRuntimePrimitives.RequireValue(currentIndex)) && (DartRuntimePrimitives.RequireValue(currentIndex) < checked((long)(items.Count)))));
        System.Diagnostics.Debug.Assert((__height >= 0.0));
    }

    public virtual Size preferredSize => new global::Doroti.Ui.Size(DartRuntimePrimitives.RequireValue(this.height));
    public virtual bool opaque(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color backgroundColor__5150 = ((global::Doroti.Ui.Color)(object?)(this.backgroundColor ?? CupertinoTheme.of(context).barBackgroundColor));
        return (CupertinoDynamicColor.resolve(backgroundColor__5150, context).alpha == 255L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        double bottomPadding__5448 = MediaQuery.viewPaddingOf(context).bottom;
        global::Doroti.Ui.Color backgroundColor__5523 = ((global::Doroti.Ui.Color)(object?)CupertinoDynamicColor.resolve((this.backgroundColor ?? CupertinoTheme.of(context).barBackgroundColor), context));
        global::Doroti.Framework.Painting.BorderSide resolveBorderSide(global::Doroti.Framework.Painting.BorderSide side)
        {
            return ((object.Equals(side, global::Doroti.Framework.Painting.BorderSide.none)) ? side : side.copyWith(color: CupertinoDynamicColor.resolve(((global::Doroti.Framework.Painting.BorderSide)side).color, context)));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        global::Doroti.Framework.Painting.Border? resolvedBorder__5942 = (((this.border is null) || (!object.Equals(DartRuntimePrimitives.RuntimeType(this.border), typeof(global::Doroti.Framework.Painting.Border)))) ? this.border : new global::Doroti.Framework.Painting.Border(top: resolveBorderSide(this.border!.top), left: resolveBorderSide(this.border!.left), bottom: resolveBorderSide(this.border!.bottom), right: resolveBorderSide(this.border!.right)));
        global::Doroti.Ui.Color inactive__6279 = ((global::Doroti.Ui.Color)(object?)CupertinoDynamicColor.resolve(this.inactiveColor, context));
        global::Doroti.Framework.Widgets.Widget result__6356 = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.DecoratedBox(decoration: new global::Doroti.Framework.Painting.BoxDecoration(border: resolvedBorder__5942, color: backgroundColor__5523), child: new global::Doroti.Framework.Widgets.SizedBox(height: (this.height + bottomPadding__5448), child: IconTheme.merge(data: new global::Doroti.Framework.Widgets.IconThemeData(color: inactive__6279, size: this.iconSize), child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: CupertinoTheme.of(context).textTheme.tabLabelTextStyle.copyWith(color: inactive__6279), child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(bottom: bottomPadding__5448), child: new global::Doroti.Framework.Widgets.Semantics(explicitChildNodes: true, child: new global::Doroti.Framework.Widgets.Row(crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.end, children: _buildTabItems(context)))))))));
        if (!opaque(context))
        {
            result__6356 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ClipRect(child: new global::Doroti.Framework.Widgets.BackdropFilter(filter: new global::Doroti.Ui.ImageFilter(sigmaX: 10.0, sigmaY: 10.0), child: result__6356)));
        }
        return result__6356;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<global::Doroti.Framework.Widgets.Widget> _buildTabItems(global::Doroti.Framework.Widgets.BuildContext context)
    {
        var result__7638 = new List<global::Doroti.Framework.Widgets.Widget>();
        CupertinoLocalizations localizations__7692 = CupertinoLocalizations.of(context);
        for (var index__7758 = 0L; (index__7758 < checked((long)(this.items.Count))); index__7758 += 1L)
        {
            var active__7817 = (index__7758 == this.currentIndex);
            result__7638.Add(_wrapActiveItem(context, new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.TextFieldTapRegion(child: new global::Doroti.Framework.Widgets.Semantics(selected: active__7817, hint: localizations__7692.tabSemanticsLabel(tabIndex: (index__7758 + 1L), tabCount: checked((long)(this.items.Count))), child: new global::Doroti.Framework.Widgets.MouseRegion(cursor: (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb ? global::Doroti.Framework.Services.SystemMouseCursors.click : global::Doroti.Framework.Services.MouseCursor.defer), child: new global::Doroti.Framework.Widgets.GestureDetector(behavior: global::Doroti.Framework.Rendering.HitTestBehavior.opaque, onTap: ((global::System.Action)((this.onTap is null) ? null : (() => {
this.onTap!(index__7758);
}))), child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(bottom: 4.0), child: new global::Doroti.Framework.Widgets.Column(mainAxisAlignment: global::Doroti.Framework.Rendering.MainAxisAlignment.end, children: _buildSingleTabItem(this.items[(int)(index__7758)], active__7817)))))))), active: active__7817));
        }
        return result__7638;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<global::Doroti.Framework.Widgets.Widget> _buildSingleTabItem(global::Doroti.Framework.Widgets.BottomNavigationBarItem item, bool active)
    {
        return ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection9241 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection9241.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.Center(child: (active ? ((global::Doroti.Framework.Widgets.BottomNavigationBarItem)item).activeIcon : ((global::Doroti.Framework.Widgets.BottomNavigationBarItem)item).icon))))); if ((((global::Doroti.Framework.Widgets.BottomNavigationBarItem)item).label is not null)) { __collection9241.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Text(((global::Doroti.Framework.Widgets.BottomNavigationBarItem)item).label!, semanticsLabel: ((global::Doroti.Framework.Widgets.BottomNavigationBarItem)item).semanticsLabel))); } return __collection9241; }))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _wrapActiveItem(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget item, bool active)
    {
        if (!active)
        {
            return item;
        }
        global::Doroti.Ui.Color activeColor__9640 = ((global::Doroti.Ui.Color)(object?)CupertinoDynamicColor.resolve((this.activeColor ?? CupertinoTheme.of(context).primaryColor), context));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)IconTheme.merge(data: new global::Doroti.Framework.Widgets.IconThemeData(color: activeColor__9640), child: DefaultTextStyle.merge(style: new global::Doroti.Framework.Painting.TextStyle(color: activeColor__9640), child: item)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual CupertinoTabBar copyWith(global::Doroti.Framework.Foundation.Key? key = null, List<global::Doroti.Framework.Widgets.BottomNavigationBarItem>? items = null, Color? backgroundColor = null, Color? activeColor = null, Color? inactiveColor = null, double? iconSize = null, double? height = null, global::Doroti.Framework.Painting.Border? border = null, long? currentIndex = null, global::System.Action<long>? onTap = null)
    {
        return new CupertinoTabBar(key: (key ?? this.key), items: (items ?? this.items), backgroundColor: (backgroundColor ?? this.backgroundColor), activeColor: (activeColor ?? this.activeColor), inactiveColor: (inactiveColor ?? this.inactiveColor), iconSize: (iconSize ?? this.iconSize), height: (height ?? this.height), border: (border ?? this.border), currentIndex: (currentIndex ?? this.currentIndex), onTap: ((onTap ?? (global::System.Action<long>)this.onTap)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
