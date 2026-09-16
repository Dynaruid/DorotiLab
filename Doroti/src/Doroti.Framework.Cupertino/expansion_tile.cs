// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/expansion_tile.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class Expansion_tileLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _kAnimationCurve = ((global::Doroti.Framework.Animation.Curve)Curves.easeInOut);
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

public class CupertinoExpansionTile : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget title { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.ExpansibleController? controller { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual ExpansionTileTransitionMode transitionMode { get; private set; } = default!;

    public CupertinoExpansionTile(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget title = default!, global::Doroti.Framework.Widgets.Widget child = default!, global::Doroti.Framework.Widgets.ExpansibleController? controller = null, ExpansionTileTransitionMode transitionMode = ExpansionTileTransitionMode.fade) : base(key: key)
    {
        this.title = title;
        this.child = child;
        this.controller = controller;
        this.transitionMode = transitionMode;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoExpansionTileState__expansion_tile());
}

internal class _CupertinoExpansionTileState__expansion_tile : global::Doroti.Framework.Widgets.State<CupertinoExpansionTile>
{
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _headerKey { get; private set; } = GlobalKey<IState>.Create();
    internal virtual global::Doroti.Framework.Widgets.OverlayPortalController _fadeController { get; private set; } = new global::Doroti.Framework.Widgets.OverlayPortalController();
    internal static global::Doroti.Framework.Animation.Animatable<double> _quarterTween = ((global::Doroti.Framework.Animation.Animatable<double>)new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: 0.25));
    internal virtual global::Doroti.Framework.Widgets.ExpansibleController _tileController { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<double> _iconTurns { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        _tileController = (((CupertinoExpansionTile)this.widget).controller ?? new global::Doroti.Framework.Widgets.ExpansibleController());
    }

    public override void didUpdateWidget(CupertinoExpansionTile oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!Equals(((CupertinoExpansionTile)oldWidget).controller, ((CupertinoExpansionTile)this.widget).controller)))
        {
            if ((((CupertinoExpansionTile)oldWidget).controller is null))
            {
                this._tileController.dispose();
            }
            _tileController = (((CupertinoExpansionTile)this.widget).controller ?? new global::Doroti.Framework.Widgets.ExpansibleController());
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

    internal virtual global::Doroti.Framework.Widgets.Widget? _buildIcon(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation)
    {
        _iconTurns = animation.drive(_quarterTween.chain(new global::Doroti.Framework.Animation.CurveTween(curve: Expansion_tileLibrary._kAnimationCurve)));
        return ((global::Doroti.Framework.Widgets.Widget?)new global::Doroti.Framework.Widgets.RotationTransition(turns: this._iconTurns, child: SizedBox.CreateSquare(dimension: CupertinoTheme.of(context).textTheme.textStyle.fontSize, child: new global::Doroti.Framework.Widgets.Center(child: new global::Doroti.Framework.Widgets.Icon(CupertinoIcons.right_chevron, color: CupertinoColors.activeBlue, size: Expansion_tileLibrary._kIconFontSize, fontWeight: FontWeight.w900)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _onHeaderTap()
    {
        if (((global::Doroti.Framework.Widgets.ExpansibleController)this._tileController).isExpanded)
        {
            this._tileController.collapse();
        }
        else
        {
            this._tileController.expand();
        }
        this._fadeController.show();
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildHeader(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation)
    {
        CupertinoLocalizations localizations = ((CupertinoLocalizations)CupertinoLocalizations.of(context));
        string onTapHintLocal = (((global::Doroti.Framework.Widgets.ExpansibleController)this._tileController).isExpanded ? ((CupertinoLocalizations)localizations).expansionTileExpandedTapHint : ((CupertinoLocalizations)localizations).expansionTileCollapsedTapHint);
        string? semanticsHint = default!;
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.iOS:
            case TargetPlatform.macOS:
                {
                    semanticsHint = (((global::Doroti.Framework.Widgets.ExpansibleController)this._tileController).isExpanded ? $"{((CupertinoLocalizations)localizations).collapsedHint}\n {((CupertinoLocalizations)localizations).expansionTileExpandedHint}" : $"{((CupertinoLocalizations)localizations).expandedHint}\n {((CupertinoLocalizations)localizations).expansionTileCollapsedHint}");
                    break;
                }
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
                {
                    break;
                }
        }
        return ((global::Doroti.Framework.Widgets.Widget)new global::Doroti.Framework.Widgets.Semantics(hint: semanticsHint, onTapHint: onTapHintLocal, child: new CupertinoListTile(key: this._headerKey, onTap: () => { this._onHeaderTap(); return null!; }, title: ((CupertinoExpansionTile)this.widget).title, trailing: _buildIcon(context, animation), backgroundColorActivated: CupertinoColors.transparent)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildExpansible(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget header, global::Doroti.Framework.Widgets.Widget body, global::Doroti.Framework.Animation.Animation<double> animation)
    {
        global::Doroti.Framework.Widgets.Widget childLocal = ((global::Doroti.Framework.Widgets.Widget)new global::Doroti.Framework.Widgets.Column(mainAxisSize: MainAxisSize.min, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection7070 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection7070.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(header)); if ((((global::Doroti.Framework.Animation.Animation<double>)animation).isAnimating && (Equals(((CupertinoExpansionTile)this.widget).transitionMode, ExpansionTileTransitionMode.fade)))) { __collection7070.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Opacity(opacity: 0.0, child: body))); } else { __collection7070.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(body)); } return __collection7070; }))()));
        if ((Equals(((CupertinoExpansionTile)this.widget).transitionMode, ExpansionTileTransitionMode.scroll)))
        {
            return childLocal;
        }
        DartRuntimePrimitives.Assert(() => (Equals(((CupertinoExpansionTile)this.widget).transitionMode, ExpansionTileTransitionMode.fade)));
        return ((global::Doroti.Framework.Widgets.Widget)new global::Doroti.Framework.Widgets.LayoutBuilder(builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Rendering.BoxConstraints, global::Doroti.Framework.Widgets.Widget>)((context, constraints) =>
        {
            return ((global::Doroti.Framework.Widgets.Widget)new global::Doroti.Framework.Widgets.OverlayPortal(controller: this._fadeController, overlayChildBuilder: ((context) =>
            {
                global::Doroti.Framework.Widgets.BuildContext headerContext = ((global::Doroti.Framework.Widgets.GlobalKey<IState>)this._headerKey).currentContext!;
                var overlay = ((global::Doroti.Framework.Rendering.RenderBox?)Overlay.of(headerContext).context.findRenderObject()!)!;
                var headerBox = ((global::Doroti.Framework.Rendering.RenderBox?)headerContext.findRenderObject()!)!;
                global::Doroti.Ui.Offset headerOffset = ((global::Doroti.Ui.Offset)((Offset)(headerBox).localToGlobal(Offset.zero, ancestor: overlay)));
                return new global::Doroti.Framework.Widgets.Positioned(top: (headerOffset.dy + Expansion_tileLibrary._kHeaderHeight), left: headerOffset.dx, child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: constraints, child: new global::Doroti.Framework.Widgets.Visibility(visible: ((global::Doroti.Framework.Animation.Animation<double>)animation).isAnimating, child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: animation, child: ((CupertinoExpansionTile)this.widget).child))));
                throw new InvalidOperationException("Dart closure completed without a value.");
            }), child: childLocal));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)new global::Doroti.Framework.Widgets.Expansible(controller: this._tileController, duration: Expansion_tileLibrary._kAnimationDuration, curve: Expansion_tileLibrary._kAnimationCurve, headerBuilder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget>)this._buildHeader, bodyBuilder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget>)((context, animation) => ((CupertinoExpansionTile)this.widget).child)), expansibleBuilder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget>)this._buildExpansible));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
