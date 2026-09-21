// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/expansion_tile.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class Expansion_tileLibrary
{
    internal static Curve _kAnimationCurve = Curves.easeInOut;
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
    scroll,
}

public class CupertinoExpansionTile : StatefulWidget
{
    public virtual Widget title { get; private set; } = default!;
    public virtual ExpansibleController? controller { get; private set; }
    public virtual Widget child { get; private set; } = default!;
    public virtual ExpansionTileTransitionMode transitionMode { get; private set; } = default!;

    public CupertinoExpansionTile(
        Key? key = null,
        Widget title = default!,
        Widget child = default!,
        ExpansibleController? controller = null,
        ExpansionTileTransitionMode transitionMode = ExpansionTileTransitionMode.fade
    )
        : base(key: key)
    {
        this.title = title;
        this.child = child;
        this.controller = controller;
        this.transitionMode = transitionMode;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _CupertinoExpansionTileState__expansion_tile()
        );
}

internal class _CupertinoExpansionTileState__expansion_tile : State<CupertinoExpansionTile>
{
    internal virtual GlobalKey<IState> _headerKey { get; private set; } =
        GlobalKey<IState>.Create();
    internal virtual OverlayPortalController _fadeController { get; private set; } =
        new OverlayPortalController();
    internal static Animatable<double> _quarterTween = new Tween<double>(begin: 0.0, end: 0.25);
    internal virtual ExpansibleController _tileController { get; set; } = default!;
    internal virtual Animation<double> _iconTurns { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        _tileController = widget.controller ?? new ExpansibleController();
    }

    public override void didUpdateWidget(CupertinoExpansionTile oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(oldWidget.controller, widget.controller))
        {
            if (oldWidget.controller is null)
            {
                _tileController.dispose();
            }
            _tileController = widget.controller ?? new ExpansibleController();
        }
    }

    public override void dispose()
    {
        if (widget.controller is null)
        {
            _tileController.dispose();
        }
        base.dispose();
    }

    internal virtual Widget? _buildIcon(BuildContext context, Animation<double> animation)
    {
        _iconTurns = animation.drive(
            _quarterTween.chain(new CurveTween(curve: Expansion_tileLibrary._kAnimationCurve))
        );
        return (Widget?)
            new RotationTransition(
                turns: _iconTurns,
                child: SizedBox.CreateSquare(
                    dimension: CupertinoTheme.of(context).textTheme.textStyle.fontSize,
                    child: new Center(
                        child: new Icon(
                            CupertinoIcons.right_chevron,
                            color: CupertinoColors.activeBlue,
                            size: Expansion_tileLibrary._kIconFontSize,
                            fontWeight: FontWeight.w900
                        )
                    )
                )
            );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _onHeaderTap()
    {
        if (_tileController.isExpanded)
        {
            _tileController.collapse();
        }
        else
        {
            _tileController.expand();
        }
        _fadeController.show();
    }

    internal virtual Widget _buildHeader(BuildContext context, Animation<double> animation)
    {
        CupertinoLocalizations localizations = CupertinoLocalizations.of(context);
        string onTapHintLocal = _tileController.isExpanded
            ? localizations.expansionTileExpandedTapHint
            : localizations.expansionTileCollapsedTapHint;
        string? semanticsHint = default!;
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.iOS:
            case TargetPlatform.macOS:
            {
                semanticsHint = _tileController.isExpanded
                    ? $"{localizations.collapsedHint}\n {localizations.expansionTileExpandedHint}"
                    : $"{localizations.expandedHint}\n {localizations.expansionTileCollapsedHint}";
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
        return new Widgets.Semantics(
            hint: semanticsHint,
            onTapHint: onTapHintLocal,
            child: new CupertinoListTile(
                key: _headerKey,
                onTap: () =>
                {
                    _onHeaderTap();
                    return null!;
                },
                title: widget.title,
                trailing: _buildIcon(context, animation),
                backgroundColorActivated: CupertinoColors.transparent
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Widget _buildExpansible(
        BuildContext context,
        Widget header,
        Widget body,
        Animation<double> animation
    )
    {
        Widget childLocal = new Column(
            mainAxisSize: MainAxisSize.min,
            children: (
                (Func<List<Widget>>)(
                    () =>
                    {
                        var __collection7070 = new List<Widget>();
                        __collection7070.Add(DartRuntimePrimitives.ConvertValue<Widget>(header));
                        if (
                            animation.isAnimating
                            && Equals(widget.transitionMode, ExpansionTileTransitionMode.fade)
                        )
                        {
                            __collection7070.Add(
                                DartRuntimePrimitives.ConvertValue<Widget>(
                                    new Opacity(opacity: 0.0, child: body)
                                )
                            );
                        }
                        else
                        {
                            __collection7070.Add(DartRuntimePrimitives.ConvertValue<Widget>(body));
                        }
                        return __collection7070;
                    }
                )
            )()
        );
        if (Equals(widget.transitionMode, ExpansionTileTransitionMode.scroll))
        {
            return childLocal;
        }
        DartRuntimePrimitives.Assert(() =>
            Equals(widget.transitionMode, ExpansionTileTransitionMode.fade)
        );
        return new LayoutBuilder(
            builder: (context, constraints) =>
            {
                return new OverlayPortal(
                    controller: _fadeController,
                    overlayChildBuilder: (context) =>
                    {
                        BuildContext headerContext = _headerKey.currentContext!;
                        var overlay = (
                            (RenderBox?)Overlay.of(headerContext).context.findRenderObject()!
                        )!;
                        var headerBox = ((RenderBox?)headerContext.findRenderObject()!)!;
                        Offset headerOffset = headerBox.localToGlobal(
                            Offset.zero,
                            ancestor: overlay
                        );
                        return new Positioned(
                            top: headerOffset.dy + Expansion_tileLibrary._kHeaderHeight,
                            left: headerOffset.dx,
                            child: new ConstrainedBox(
                                constraints: constraints,
                                child: new Visibility(
                                    visible: animation.isAnimating,
                                    child: new FadeTransition(
                                        opacity: animation,
                                        child: widget.child
                                    )
                                )
                            )
                        );
                        throw new InvalidOperationException(
                            "Callback completed without returning a value."
                        );
                    },
                    child: childLocal
                );
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget build(BuildContext context)
    {
        return new Expansible(
            controller: _tileController,
            duration: Expansion_tileLibrary._kAnimationDuration,
            curve: Expansion_tileLibrary._kAnimationCurve,
            headerBuilder: _buildHeader,
            bodyBuilder: (context, animation) => widget.child,
            expansibleBuilder: _buildExpansible
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
