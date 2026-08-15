// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/expansion_tile.dart
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

public static partial class Expansion_tileLibrary
{
    internal static global::Doroti.Generated.Framework.Animation.Curve _kAnimationCurve = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)global::Doroti.Generated.Framework.Animation.Curves.easeInOut);
}

public static partial class Expansion_tileLibrary
{
    internal static Duration _kAnimationDuration = Duration.Create(milliseconds: 250L);
}

public static partial class Expansion_tileLibrary
{
    internal static double _kIconFontSize = 15.0;
}

public static partial class Expansion_tileLibrary
{
    internal static double _kHeaderHeight = 44.0;
}

public enum ExpansionTileTransitionMode
{
    fade,
    scroll
}

public class CupertinoExpansionTile : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget title { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.ExpansibleController? controller { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual ExpansionTileTransitionMode transitionMode { get; private set; } = default!;

    public CupertinoExpansionTile(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget title = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!, global::Doroti.Generated.Framework.Widgets.ExpansibleController? controller = null, ExpansionTileTransitionMode transitionMode = ExpansionTileTransitionMode.fade) : base(key: key)
    {
        this.title = title;
        this.child = child;
        this.controller = controller;
        this.transitionMode = transitionMode;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoExpansionTileState__expansion_tile());
}

internal class _CupertinoExpansionTileState__expansion_tile : global::Doroti.Generated.Framework.Widgets.State<CupertinoExpansionTile>
{
    internal virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> _headerKey { get; private set; } = global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create();
    internal virtual global::Doroti.Generated.Framework.Widgets.OverlayPortalController _fadeController { get; private set; } = new global::Doroti.Generated.Framework.Widgets.OverlayPortalController();
    internal static global::Doroti.Generated.Framework.Animation.Animatable<double> _quarterTween = ((global::Doroti.Generated.Framework.Animation.Animatable<double>)(object?)new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 0.0, end: 0.25));
    internal virtual global::Doroti.Generated.Framework.Widgets.ExpansibleController _tileController { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.Animation<double> _iconTurns { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        _tileController = (((CupertinoExpansionTile)this.widget).controller ?? new global::Doroti.Generated.Framework.Widgets.ExpansibleController());
    }

    public override void didUpdateWidget(CupertinoExpansionTile oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((CupertinoExpansionTile)oldWidget).controller, ((CupertinoExpansionTile)this.widget).controller)))
        {
            if ((((CupertinoExpansionTile)oldWidget).controller is null))
            {
                this._tileController.dispose();
            }
            _tileController = (((CupertinoExpansionTile)this.widget).controller ?? new global::Doroti.Generated.Framework.Widgets.ExpansibleController());
        }
    }

    public override void dispose()
    {
        if ((((CupertinoExpansionTile)this.widget).controller is null))
        {
            this._tileController.dispose();
        }
        base.dispose();
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget? _buildIcon(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> animation)
    {
        _iconTurns = animation.drive(_quarterTween.chain(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: Expansion_tileLibrary._kAnimationCurve)));
        return ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)new global::Doroti.Generated.Framework.Widgets.RotationTransition(turns: this._iconTurns, child: global::Doroti.Generated.Framework.Widgets.SizedBox.CreateSquare(dimension: CupertinoTheme.of(context).textTheme.textStyle.fontSize, child: new global::Doroti.Generated.Framework.Widgets.Center(child: new global::Doroti.Generated.Framework.Widgets.Icon(CupertinoIcons.right_chevron, color: CupertinoColors.activeBlue, size: Expansion_tileLibrary._kIconFontSize, fontWeight: FontWeight.w900)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _onHeaderTap()
    {
        if (((global::Doroti.Generated.Framework.Widgets.ExpansibleController)this._tileController).isExpanded)
        {
            this._tileController.collapse();
        }
        else
        {
            this._tileController.expand();
        }
        this._fadeController.show();
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildHeader(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> animation)
    {
        CupertinoLocalizations localizations__5790 = ((CupertinoLocalizations)(object?)CupertinoLocalizations.of(context));
        string onTapHint__5859 = (((global::Doroti.Generated.Framework.Widgets.ExpansibleController)this._tileController).isExpanded ? ((CupertinoLocalizations)localizations__5790).expansionTileExpandedTapHint : ((CupertinoLocalizations)localizations__5790).expansionTileCollapsedTapHint);
        string? semanticsHint__6018 = default!;
        switch (global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
                {
                    semanticsHint__6018 = (((global::Doroti.Generated.Framework.Widgets.ExpansibleController)this._tileController).isExpanded ? $"{((CupertinoLocalizations)localizations__5790).collapsedHint}\n {((CupertinoLocalizations)localizations__5790).expansionTileExpandedHint}" : $"{((CupertinoLocalizations)localizations__5790).expandedHint}\n {((CupertinoLocalizations)localizations__5790).expansionTileCollapsedHint}");
                    break;
                }
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
                {
                    break;
                }
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Semantics(hint: semanticsHint__6018, onTapHint: onTapHint__5859, child: new CupertinoListTile(key: this._headerKey, onTap: () => { this._onHeaderTap(); return null!; }, title: ((CupertinoExpansionTile)this.widget).title, trailing: _buildIcon(context, animation), backgroundColorActivated: CupertinoColors.transparent)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildExpansible(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget header, global::Doroti.Generated.Framework.Widgets.Widget body, global::Doroti.Generated.Framework.Animation.Animation<double> animation)
    {
        global::Doroti.Generated.Framework.Widgets.Widget child__7000 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Column(mainAxisSize: global::Doroti.Generated.Framework.Rendering.MainAxisSize.min, children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection7070 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); __collection7070.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(header)); if ((((global::Doroti.Generated.Framework.Animation.Animation<double>)animation).isAnimating && (object.Equals(((CupertinoExpansionTile)this.widget).transitionMode, ExpansionTileTransitionMode.fade)))) { __collection7070.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Opacity(opacity: 0.0, child: body))); } else { __collection7070.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(body)); } return __collection7070; }))()));
        if ((object.Equals(((CupertinoExpansionTile)this.widget).transitionMode, ExpansionTileTransitionMode.scroll)))
        {
            return child__7000;
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(((CupertinoExpansionTile)this.widget).transitionMode, ExpansionTileTransitionMode.fade)));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.LayoutBuilder(builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Rendering.BoxConstraints, global::Doroti.Generated.Framework.Widgets.Widget>)((context, constraints) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.OverlayPortal(controller: this._fadeController, overlayChildBuilder: ((context) => {
global::Doroti.Generated.Framework.Widgets.BuildContext headerContext__7700 = ((global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>)this._headerKey).currentContext!;
var overlay__7762 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)Overlay.of(headerContext__7700).context.findRenderObject()!)!;
var headerBox__7858 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)headerContext__7700.findRenderObject()!)!;
global::Doroti.Ui.Offset headerOffset__7943 = ((global::Doroti.Ui.Offset)(object?)((Offset)((dynamic)headerBox__7858).localToGlobal(Offset.zero, ancestor: overlay__7762)));
return new global::Doroti.Generated.Framework.Widgets.Positioned(top: (headerOffset__7943.dy + Expansion_tileLibrary._kHeaderHeight), left: headerOffset__7943.dx, child: new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: constraints, child: new global::Doroti.Generated.Framework.Widgets.Visibility(visible: ((global::Doroti.Generated.Framework.Animation.Animation<double>)animation).isAnimating, child: new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: animation, child: ((CupertinoExpansionTile)this.widget).child))));
throw new InvalidOperationException("Dart closure completed without a value.");
}), child: child__7000));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Expansible(controller: this._tileController, duration: Expansion_tileLibrary._kAnimationDuration, curve: Expansion_tileLibrary._kAnimationCurve, headerBuilder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Widgets.Widget>)this._buildHeader, bodyBuilder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Widgets.Widget>)((context, animation) => ((CupertinoExpansionTile)this.widget).child)), expansibleBuilder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Widgets.Widget>)this._buildExpansible));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
