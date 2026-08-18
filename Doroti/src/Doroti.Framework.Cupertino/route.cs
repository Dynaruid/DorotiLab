// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/route.dart
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

public static partial class RouteLibrary
{
    internal static double _kBackGestureWidth = 20.0;
}

public static partial class RouteLibrary
{
    internal static double _kMinFlingVelocity = 1.0;
}

public static partial class RouteLibrary
{
    internal static Duration _kDroppedSwipePageAnimationDuration = Duration.Create(milliseconds: 350L);
}

public static partial class RouteLibrary
{
    internal static Color _kCupertinoPageTransitionBarrierColor = new global::Doroti.Ui.Color(402653184L);
}

public static partial class RouteLibrary
{
    public static Color kCupertinoModalBarrierColor = ((Color)(object?)new CupertinoDynamicColor(color: new global::Doroti.Ui.Color(855638016L), darkColor: new global::Doroti.Ui.Color(2046820352L)));
}

public static partial class RouteLibrary
{
    internal static Duration _kModalPopupTransitionDuration = Duration.Create(milliseconds: 335L);
}

public static partial class RouteLibrary
{
    internal static global::Doroti.Framework.Animation.Animatable<Offset> _kRightMiddleTween = ((global::Doroti.Framework.Animation.Animatable<Offset>)(object?)new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: new global::Doroti.Ui.Offset(1.0, 0.0), end: Offset.zero));
}

public static partial class RouteLibrary
{
    internal static global::Doroti.Framework.Animation.Animatable<Offset> _kMiddleLeftTween = ((global::Doroti.Framework.Animation.Animatable<Offset>)(object?)new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: Offset.zero, end: new global::Doroti.Ui.Offset((-1.0 / 3.0), 0.0)));
}

public static partial class RouteLibrary
{
    internal static global::Doroti.Framework.Animation.Animatable<Offset> _kBottomUpTween = ((global::Doroti.Framework.Animation.Animatable<Offset>)(object?)new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: new global::Doroti.Ui.Offset(0.0, 1.0), end: Offset.zero));
}

public interface CupertinoRouteTransitionMixin<T>
{
    global::Doroti.Framework.Foundation.ValueNotifier<string?>? _previousTitle { get; set; }
    public static Duration kTransitionDuration = Duration.Create(milliseconds: 500L);

    public global::Doroti.Framework.Widgets.Widget buildContent(global::Doroti.Framework.Widgets.BuildContext context);
    public string? title { get; }
    public global::Doroti.Framework.Foundation.ValueListenable<string?> previousTitle { get; }
    public void dispose();
    public void didChangePrevious(dynamic previousRoute);
    public Duration transitionDuration { get; }
    public global::Doroti.Ui.Color? barrierColor { get; }
    public string? barrierLabel { get; }
    public bool canTransitionTo(dynamic nextRoute);
    public bool canTransitionFrom(dynamic previousRoute);
    public global::Doroti.Framework.Widgets.Widget buildPage(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation);
    public static _CupertinoBackGestureController__route<T> _startPopGesture<T>(global::Doroti.Framework.Widgets.PageRoute<T> route)
    {
        DartRuntimePrimitives.Assert(() => ((global::Doroti.Framework.Widgets.PageRoute<T>)route).popGestureEnabled);
        return new _CupertinoBackGestureController__route<T>(navigator: route.navigator!, getIsCurrent: ((global::System.Func<bool>)(() => route.isCurrent)), getIsActive: ((global::System.Func<bool>)(() => route.isActive)), controller: route.controller!);
    }
    public static global::Doroti.Framework.Widgets.Widget buildPageTransitions<T>(global::Doroti.Framework.Widgets.PageRoute<T> route, global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget child)
    {
        bool linearTransition__8493 = route.popGestureInProgress;
        if (((global::Doroti.Framework.Widgets.PageRoute<T>)route).fullscreenDialog)
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new CupertinoFullscreenDialogTransition(primaryRouteAnimation: animation, secondaryRouteAnimation: secondaryAnimation, linearTransition: linearTransition__8493, child: child));
        }
        else
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new CupertinoPageTransition(primaryRouteAnimation: animation, secondaryRouteAnimation: secondaryAnimation, linearTransition: linearTransition__8493, child: new _CupertinoBackGestureDetector__route<T>(enabledCallback: ((global::System.Func<bool>)(() => ((global::Doroti.Framework.Widgets.PageRoute<T>)route).popGestureEnabled)), onStartPopGesture: ((global::System.Func<_CupertinoBackGestureController__route<T>>)(() => CupertinoRouteTransitionMixin<T>._startPopGesture<T>(route))), child: child)));
        }
    }
    public global::Doroti.Framework.Widgets.Widget buildTransitions(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget child);
}

public class CupertinoPageRoute<T> : global::Doroti.Framework.Widgets.PageRoute<T>, CupertinoRouteTransitionMixin<T>
{
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> builder { get; private set; } = default!;
    public virtual string? title { get; private set; }
    private bool __field_maintainState = default!;
    public override bool maintainState { get => __field_maintainState; }
    public virtual global::Doroti.Framework.Foundation.ValueNotifier<string?>? _previousTitle { get; set; } = default;

    public CupertinoPageRoute(global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> builder, string? title = null, global::Doroti.Framework.Widgets.RouteSettings? settings = null, bool? requestFocus = null, bool maintainState = true, bool fullscreenDialog = false, bool allowSnapshotting = true, bool barrierDismissible = false) : base(settings: settings, requestFocus: requestFocus, fullscreenDialog: fullscreenDialog, allowSnapshotting: allowSnapshotting, barrierDismissible: barrierDismissible)
    {
        this.builder = builder;
        this.title = title;
        this.__field_maintainState = maintainState;
        DartRuntimePrimitives.Assert(() => this.opaque);
    }

    public override global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, bool, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget?>? delegatedTransition => CupertinoPageTransition.delegatedTransition;
    public virtual global::Doroti.Framework.Widgets.Widget buildContent(global::Doroti.Framework.Widgets.BuildContext context) => this.builder(context);
    public override string debugLabel => $"{base.debugLabel}({(((global::Doroti.Framework.Widgets.RouteSettings)this.settings).name)})";
    public virtual global::Doroti.Framework.Foundation.ValueListenable<string?> previousTitle
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (this._previousTitle is not null), () => (object?)"Cannot read the previousTitle for a route that has not yet been installed");
            return ((global::Doroti.Framework.Foundation.ValueListenable<string?>)(object?)this._previousTitle!);
            return default!;
        }
    }
    public override void dispose()
    {
        this._previousTitle?.dispose();
        base.dispose();
    }

    public override void didChangePrevious(dynamic previousRoute)
    {
        string? previousTitleString__4812 = ((previousRoute is CupertinoRouteTransitionMixin<object>) ? ((CupertinoRouteTransitionMixin<object>)previousRoute).title : null);
        if ((this._previousTitle is null))
        {
            this._previousTitle = new global::Doroti.Framework.Foundation.ValueNotifier<string?>(previousTitleString__4812);
        }
        else
        {
            this._previousTitle!.value = previousTitleString__4812;
        }
        base.didChangePrevious((object?)previousRoute);
    }

    public override Duration transitionDuration => CupertinoRouteTransitionMixin<object>.kTransitionDuration;
    public override Color? barrierColor => (this.fullscreenDialog ? null : RouteLibrary._kCupertinoPageTransitionBarrierColor);
    public override string? barrierLabel => DartRuntimePrimitives.ConvertValue<string>(null);
    public override bool canTransitionTo(dynamic nextRoute)
    {
        bool nextRouteIsNotFullscreen__5718 = (((nextRoute is not global::Doroti.Framework.Widgets.PageRoute<T>)) || !((global::Doroti.Framework.Widgets.PageRoute<T>)nextRoute).fullscreenDialog);
        bool nextRouteHasDelegatedTransition__6005 = ((nextRoute is global::Doroti.Framework.Widgets.ModalRoute<T>) && (((global::Doroti.Framework.Widgets.ModalRoute<T>)nextRoute).delegatedTransition is not null));
        return (nextRouteIsNotFullscreen__5718 && ((((nextRoute is CupertinoRouteTransitionMixin<object>)) || nextRouteHasDelegatedTransition__6005)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool canTransitionFrom(dynamic previousRoute)
    {
        return ((previousRoute is PageRoute<object>) && !this.fullscreenDialog);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget buildPage(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation)
    {
        global::Doroti.Framework.Widgets.Widget child__6790 = ((global::Doroti.Framework.Widgets.Widget)(object?)buildContent(context));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(scopesRoute: true, explicitChildNodes: true, child: child__6790));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget buildTransitions(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)CupertinoRouteTransitionMixin<object>.buildPageTransitions<T>(this, context, animation, secondaryAnimation, child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _PageBasedCupertinoPageRoute__route<T> : global::Doroti.Framework.Widgets.PageRoute<T>, CupertinoRouteTransitionMixin<T>
{
    public virtual global::Doroti.Framework.Foundation.ValueNotifier<string?>? _previousTitle { get; set; } = default;

    internal _PageBasedCupertinoPageRoute__route(CupertinoPage<T> page, bool allowSnapshotting = true) : base(allowSnapshotting: allowSnapshotting, settings: page)
    {
        DartRuntimePrimitives.Assert(() => this.opaque);
    }

    public override global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, bool, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget?>? delegatedTransition => ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, bool, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget?>)(this.fullscreenDialog ? null : CupertinoPageTransition.delegatedTransition));
    internal virtual CupertinoPage<T> _page => ((CupertinoPage<T>?)(object?)this.settings)!;
    public virtual global::Doroti.Framework.Widgets.Widget buildContent(global::Doroti.Framework.Widgets.BuildContext context) => ((CupertinoPage<T>)this._page).child;
    public virtual string? title => ((CupertinoPage<T>)this._page).title;
    public override bool maintainState => ((CupertinoPage<T>)this._page).maintainState;
    public override bool fullscreenDialog => ((CupertinoPage<T>)this._page).fullscreenDialog;
    public override string debugLabel => $"{base.debugLabel}({this._page.name})";
    public virtual global::Doroti.Framework.Foundation.ValueListenable<string?> previousTitle
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (this._previousTitle is not null), () => (object?)"Cannot read the previousTitle for a route that has not yet been installed");
            return ((global::Doroti.Framework.Foundation.ValueListenable<string?>)(object?)this._previousTitle!);
            return default!;
        }
    }
    public override void dispose()
    {
        this._previousTitle?.dispose();
        base.dispose();
    }

    public override void didChangePrevious(dynamic previousRoute)
    {
        string? previousTitleString__4812 = ((previousRoute is CupertinoRouteTransitionMixin<object>) ? ((CupertinoRouteTransitionMixin<object>)previousRoute).title : null);
        if ((this._previousTitle is null))
        {
            this._previousTitle = new global::Doroti.Framework.Foundation.ValueNotifier<string?>(previousTitleString__4812);
        }
        else
        {
            this._previousTitle!.value = previousTitleString__4812;
        }
        base.didChangePrevious((object?)previousRoute);
    }

    public override Duration transitionDuration => CupertinoRouteTransitionMixin<object>.kTransitionDuration;
    public override Color? barrierColor => (this.fullscreenDialog ? null : RouteLibrary._kCupertinoPageTransitionBarrierColor);
    public override string? barrierLabel => DartRuntimePrimitives.ConvertValue<string>(null);
    public override bool canTransitionTo(dynamic nextRoute)
    {
        bool nextRouteIsNotFullscreen__5718 = (((nextRoute is not global::Doroti.Framework.Widgets.PageRoute<T>)) || !((global::Doroti.Framework.Widgets.PageRoute<T>)nextRoute).fullscreenDialog);
        bool nextRouteHasDelegatedTransition__6005 = ((nextRoute is global::Doroti.Framework.Widgets.ModalRoute<T>) && (((global::Doroti.Framework.Widgets.ModalRoute<T>)nextRoute).delegatedTransition is not null));
        return (nextRouteIsNotFullscreen__5718 && ((((nextRoute is CupertinoRouteTransitionMixin<object>)) || nextRouteHasDelegatedTransition__6005)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool canTransitionFrom(dynamic previousRoute)
    {
        return ((previousRoute is PageRoute<object>) && !this.fullscreenDialog);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget buildPage(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation)
    {
        global::Doroti.Framework.Widgets.Widget child__6790 = ((global::Doroti.Framework.Widgets.Widget)(object?)buildContent(context));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(scopesRoute: true, explicitChildNodes: true, child: child__6790));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget buildTransitions(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)CupertinoRouteTransitionMixin<object>.buildPageTransitions<T>(this, context, animation, secondaryAnimation, child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CupertinoPage<T> : global::Doroti.Framework.Widgets.Page<T>
{
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual string? title { get; private set; }
    public virtual bool maintainState { get; private set; } = default!;
    public virtual bool fullscreenDialog { get; private set; } = default!;
    public virtual bool allowSnapshotting { get; private set; } = default!;

    public CupertinoPage(global::Doroti.Framework.Widgets.Widget child, bool maintainState = true, string? title = null, bool fullscreenDialog = false, bool allowSnapshotting = true, bool canPop = true, global::System.Action<bool, T?> onPopInvoked = default!, global::Doroti.Framework.Foundation.LocalKey? key = null, string? name = null, object? arguments = null, string? restorationId = null) : base(canPop: canPop, onPopInvoked: onPopInvoked ?? ((didPop, result) => Page<object>._defaultPopInvokedHandler(didPop, result)), key: key, name: name, arguments: arguments, restorationId: restorationId)
    {
        this.child = child;
        this.maintainState = maintainState;
        this.title = title;
        this.fullscreenDialog = fullscreenDialog;
        this.allowSnapshotting = allowSnapshotting;
    }

    public override global::Doroti.Framework.Widgets.Route<T> createRoute(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Route<T>)(object?)new _PageBasedCupertinoPageRoute__route<T>(page: this, allowSnapshotting: this.allowSnapshotting));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CupertinoPageTransition : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> primaryRouteAnimation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> secondaryRouteAnimation { get; private set; } = default!;
    public virtual bool linearTransition { get; private set; } = default!;

    public CupertinoPageTransition(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Animation.Animation<double> primaryRouteAnimation = default!, global::Doroti.Framework.Animation.Animation<double> secondaryRouteAnimation = default!, global::Doroti.Framework.Widgets.Widget child = default!, bool linearTransition = default!) : base(key: key)
    {
        this.primaryRouteAnimation = primaryRouteAnimation;
        this.secondaryRouteAnimation = secondaryRouteAnimation;
        this.child = child;
        this.linearTransition = linearTransition;
    }

    public static global::Doroti.Framework.Widgets.Widget? delegatedTransition(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, bool allowSnapshotting, global::Doroti.Framework.Widgets.Widget? child)
    {
        var animation__15752 = new global::Doroti.Framework.Animation.CurvedAnimation(parent: secondaryAnimation, curve: global::Doroti.Framework.Animation.Curves.linearToEaseOut, reverseCurve: global::Doroti.Framework.Animation.Curves.easeInToLinear);
        global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Offset> delegatedPositionAnimation__15930 = ((global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Offset>)(object?)animation__15752.drive(RouteLibrary._kMiddleLeftTween));
        animation__15752.dispose();
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        global::Doroti.Ui.TextDirection textDirection__16095 = Directionality.of(context);
        return ((global::Doroti.Framework.Widgets.Widget?)(object?)new global::Doroti.Framework.Widgets.SlideTransition(position: delegatedPositionAnimation__15930, textDirection: textDirection__16095, transformHitTests: false, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoPageTransitionState__route());
}

internal class _CupertinoPageTransitionState__route : global::Doroti.Framework.Widgets.State<CupertinoPageTransition>
{
    internal virtual global::Doroti.Framework.Animation.Animation<Offset> _primaryPositionAnimation { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<Offset> _secondaryPositionAnimation { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Painting.Decoration> _primaryShadowAnimation { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation? _primaryPositionCurve { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation? _secondaryPositionCurve { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation? _primaryShadowCurve { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _setupAnimation();
    }

    public override void didUpdateWidget(CupertinoPageTransition oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((((!object.Equals(((CupertinoPageTransition)oldWidget).primaryRouteAnimation, ((CupertinoPageTransition)(object)this.widget).primaryRouteAnimation)) || (!object.Equals(((CupertinoPageTransition)oldWidget).secondaryRouteAnimation, ((CupertinoPageTransition)(object)this.widget).secondaryRouteAnimation))) || (((CupertinoPageTransition)oldWidget).linearTransition != ((CupertinoPageTransition)(object)this.widget).linearTransition)))
        {
            _disposeCurve();
            _setupAnimation();
        }
    }

    public override void dispose()
    {
        _disposeCurve();
        base.dispose();
    }

    internal virtual void _disposeCurve()
    {
        this._primaryPositionCurve?.dispose();
        this._secondaryPositionCurve?.dispose();
        this._primaryShadowCurve?.dispose();
        _primaryPositionCurve = null;
        _secondaryPositionCurve = null;
        _primaryShadowCurve = null;
    }

    internal virtual void _setupAnimation()
    {
        if (!((CupertinoPageTransition)(object)this.widget).linearTransition)
        {
            _primaryPositionCurve = new global::Doroti.Framework.Animation.CurvedAnimation(parent: ((CupertinoPageTransition)(object)this.widget).primaryRouteAnimation, curve: global::Doroti.Framework.Animation.Curves.fastEaseInToSlowEaseOut, reverseCurve: global::Doroti.Framework.Animation.Curves.fastEaseInToSlowEaseOut.flipped);
            _secondaryPositionCurve = new global::Doroti.Framework.Animation.CurvedAnimation(parent: ((CupertinoPageTransition)(object)this.widget).secondaryRouteAnimation, curve: global::Doroti.Framework.Animation.Curves.linearToEaseOut, reverseCurve: global::Doroti.Framework.Animation.Curves.easeInToLinear);
            _primaryShadowCurve = new global::Doroti.Framework.Animation.CurvedAnimation(parent: ((CupertinoPageTransition)(object)this.widget).primaryRouteAnimation, curve: global::Doroti.Framework.Animation.Curves.linearToEaseOut);
        }
        _primaryPositionAnimation = ((this._primaryPositionCurve ?? ((CupertinoPageTransition)(object)this.widget).primaryRouteAnimation)).drive(RouteLibrary._kRightMiddleTween);
        _secondaryPositionAnimation = ((this._secondaryPositionCurve ?? ((CupertinoPageTransition)(object)this.widget).secondaryRouteAnimation)).drive(RouteLibrary._kMiddleLeftTween);
        _primaryShadowAnimation = ((this._primaryShadowCurve ?? ((CupertinoPageTransition)(object)this.widget).primaryRouteAnimation)).drive(_CupertinoEdgeShadowDecoration__route.kTween);
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        global::Doroti.Ui.TextDirection textDirection__19075 = Directionality.of(context);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.SlideTransition(position: this._secondaryPositionAnimation, textDirection: textDirection__19075, transformHitTests: false, child: new global::Doroti.Framework.Widgets.SlideTransition(position: this._primaryPositionAnimation, textDirection: textDirection__19075, child: new global::Doroti.Framework.Widgets.DecoratedBoxTransition(decoration: this._primaryShadowAnimation, child: ((CupertinoPageTransition)(object)this.widget).child))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CupertinoFullscreenDialogTransition : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Animation.Animation<double> primaryRouteAnimation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> secondaryRouteAnimation { get; private set; } = default!;
    public virtual bool linearTransition { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;

    public CupertinoFullscreenDialogTransition(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Animation.Animation<double> primaryRouteAnimation = default!, global::Doroti.Framework.Animation.Animation<double> secondaryRouteAnimation = default!, global::Doroti.Framework.Widgets.Widget child = default!, bool linearTransition = default!) : base(key: key)
    {
        this.primaryRouteAnimation = primaryRouteAnimation;
        this.secondaryRouteAnimation = secondaryRouteAnimation;
        this.child = child;
        this.linearTransition = linearTransition;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoFullscreenDialogTransitionState__route());
}

internal class _CupertinoFullscreenDialogTransitionState__route : global::Doroti.Framework.Widgets.State<CupertinoFullscreenDialogTransition>
{
    internal virtual global::Doroti.Framework.Animation.Animation<Offset> _primaryPositionAnimation { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<Offset> _secondaryPositionAnimation { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation? _primaryPositionCurve { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation? _secondaryPositionCurve { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _setupAnimation();
    }

    public override void didUpdateWidget(CupertinoFullscreenDialogTransition oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((((!object.Equals(((CupertinoFullscreenDialogTransition)oldWidget).primaryRouteAnimation, ((CupertinoFullscreenDialogTransition)(object)this.widget).primaryRouteAnimation)) || (!object.Equals(((CupertinoFullscreenDialogTransition)oldWidget).secondaryRouteAnimation, ((CupertinoFullscreenDialogTransition)(object)this.widget).secondaryRouteAnimation))) || (((CupertinoFullscreenDialogTransition)oldWidget).linearTransition != ((CupertinoFullscreenDialogTransition)(object)this.widget).linearTransition)))
        {
            _disposeCurve();
            _setupAnimation();
        }
    }

    public override void dispose()
    {
        _disposeCurve();
        base.dispose();
    }

    internal virtual void _disposeCurve()
    {
        this._primaryPositionCurve?.dispose();
        this._secondaryPositionCurve?.dispose();
        _primaryPositionCurve = null;
        _secondaryPositionCurve = null;
    }

    internal virtual void _setupAnimation()
    {
        _primaryPositionAnimation = (_primaryPositionCurve = new global::Doroti.Framework.Animation.CurvedAnimation(parent: ((CupertinoFullscreenDialogTransition)(object)this.widget).primaryRouteAnimation, curve: global::Doroti.Framework.Animation.Curves.linearToEaseOut, reverseCurve: global::Doroti.Framework.Animation.Curves.linearToEaseOut.flipped)).drive(RouteLibrary._kBottomUpTween);
        _secondaryPositionAnimation = ((((CupertinoFullscreenDialogTransition)(object)this.widget).linearTransition ? ((CupertinoFullscreenDialogTransition)(object)this.widget).secondaryRouteAnimation : _secondaryPositionCurve = new global::Doroti.Framework.Animation.CurvedAnimation(parent: ((CupertinoFullscreenDialogTransition)(object)this.widget).secondaryRouteAnimation, curve: global::Doroti.Framework.Animation.Curves.linearToEaseOut, reverseCurve: global::Doroti.Framework.Animation.Curves.easeInToLinear))).drive(RouteLibrary._kMiddleLeftTween);
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        global::Doroti.Ui.TextDirection textDirection__22996 = Directionality.of(context);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.SlideTransition(position: this._secondaryPositionAnimation, textDirection: textDirection__22996, transformHitTests: false, child: new global::Doroti.Framework.Widgets.SlideTransition(position: this._primaryPositionAnimation, child: ((CupertinoFullscreenDialogTransition)(object)this.widget).child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CupertinoBackGestureDetector__route<T> : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual global::System.Func<bool> enabledCallback { get; private set; } = default!;
    public virtual global::System.Func<_CupertinoBackGestureController__route<T>> onStartPopGesture { get; private set; } = default!;

    internal _CupertinoBackGestureDetector__route(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<bool> enabledCallback = default!, global::System.Func<_CupertinoBackGestureController__route<T>> onStartPopGesture = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key)
    {
        this.enabledCallback = enabledCallback;
        this.onStartPopGesture = onStartPopGesture;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoBackGestureDetectorState__route<T>());
}

internal class _CupertinoBackGestureDetectorState__route<T> : global::Doroti.Framework.Widgets.State<_CupertinoBackGestureDetector__route<T>>
{
    internal virtual _CupertinoBackGestureController__route<T>? _backGestureController { get; set; } = default;
    internal virtual global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer _recognizer { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        _recognizer = ((Func<global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer>)(() =>
{
    var __cascade = new global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer(debugOwner: this);
    __cascade.onStart = this._handleDragStart;
    __cascade.onUpdate = this._handleDragUpdate;
    __cascade.onEnd = this._handleDragEnd;
    __cascade.onCancel = this._handleDragCancel;
    return __cascade;
}))();
    }

    public override void dispose()
    {
        this._recognizer.dispose();
        if ((this._backGestureController is not null))
        {
            global::Doroti.Framework.Widgets.WidgetsBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) =>
            {
                if ((this._backGestureController?.navigator.mounted ?? false))
                {
                    this._backGestureController?.navigator.didStopUserGesture();
                }
                _backGestureController = null;
            })));
        }
        base.dispose();
    }

    internal virtual void _handleDragStart(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        DartRuntimePrimitives.Assert(() => this.mounted);
        DartRuntimePrimitives.Assert(() => (this._backGestureController is null));
        _backGestureController = this.widget.onStartPopGesture();
    }

    internal virtual void _handleDragUpdate(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        DartRuntimePrimitives.Assert(() => this.mounted);
        DartRuntimePrimitives.Assert(() => (this._backGestureController is not null));
        this._backGestureController!.dragUpdate(_convertToLogical((DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Gestures.DragUpdateDetails)details).primaryDelta) / DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Widgets.BuildContext)this.context).size).width)));
    }

    internal virtual void _handleDragEnd(global::Doroti.Framework.Gestures.DragEndDetails details)
    {
        DartRuntimePrimitives.Assert(() => this.mounted);
        DartRuntimePrimitives.Assert(() => (this._backGestureController is not null));
        this._backGestureController!.dragEnd(_convertToLogical((((global::Doroti.Framework.Gestures.DragEndDetails)details).velocity.pixelsPerSecond.dx / DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Widgets.BuildContext)this.context).size).width)));
        _backGestureController = null;
    }

    internal virtual void _handleDragCancel()
    {
        DartRuntimePrimitives.Assert(() => this.mounted);
        this._backGestureController?.dragEnd(0.0);
        _backGestureController = null;
    }

    internal virtual void _handlePointerDown(global::Doroti.Framework.Gestures.PointerDownEvent @event)
    {
        if (this.widget.enabledCallback())
        {
            this._recognizer.addPointer((global::Doroti.Framework.Gestures.PointerDownEvent)(object)@event);
        }
    }

    internal virtual double _convertToLogical(double value)
    {
        return (Directionality.of(this.context) switch { TextDirection.rtl => -value, TextDirection.ltr => value, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        double dragAreaWidth__26674 = (Directionality.of(context) switch { TextDirection.rtl => MediaQuery.paddingOf(context).right, TextDirection.ltr => MediaQuery.paddingOf(context).left, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Stack(fit: global::Doroti.Framework.Rendering.StackFit.passthrough, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(((_CupertinoBackGestureDetector__route<T>)(object)this.widget).child), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.PositionedDirectional(start: 0.0, width: Math.Max(dragAreaWidth__26674, RouteLibrary._kBackGestureWidth), top: 0.0, bottom: 0.0, child: new global::Doroti.Framework.Widgets.Listener(onPointerDown: (global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>)this._handlePointerDown, behavior: global::Doroti.Framework.Rendering.HitTestBehavior.translucent))) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _CupertinoBackGestureController__route<T>
{
    public virtual global::Doroti.Framework.Animation.AnimationController controller { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.NavigatorState navigator { get; private set; } = default!;
    public virtual global::System.Func<bool> getIsActive { get; private set; } = default!;
    public virtual global::System.Func<bool> getIsCurrent { get; private set; } = default!;

    internal _CupertinoBackGestureController__route(global::Doroti.Framework.Widgets.NavigatorState navigator, global::Doroti.Framework.Animation.AnimationController controller, global::System.Func<bool> getIsActive, global::System.Func<bool> getIsCurrent)
    {
        this.navigator = navigator;
        this.controller = controller;
        this.getIsActive = getIsActive;
        this.getIsCurrent = getIsCurrent;
        this.navigator.didStartUserGesture();
    }

    public virtual void dragUpdate(double delta)
    {
        this.controller.value -= delta;
    }

    public virtual void dragEnd(double velocity)
    {
        global::Doroti.Framework.Animation.Curve animationCurve__28787 = ((global::Doroti.Framework.Animation.Curve)(object?)global::Doroti.Framework.Animation.Curves.fastEaseInToSlowEaseOut);
        bool isCurrent__28851 = this.getIsCurrent();
        bool animateForward__28894 = default!;
        if (!isCurrent__28851)
        {
            animateForward__28894 = this.getIsActive();
        }
        else
        {
            if ((velocity.abs() >= RouteLibrary._kMinFlingVelocity))
            {
                animateForward__28894 = (velocity <= 0L);
            }
            else
            {
                animateForward__28894 = (((global::Doroti.Framework.Animation.AnimationController)this.controller).value > 0.5);
            }
        }
        if (animateForward__28894)
        {
            this.controller.animateTo(1.0, duration: RouteLibrary._kDroppedSwipePageAnimationDuration, curve: animationCurve__28787);
        }
        else
        {
            if (isCurrent__28851)
            {
                this.navigator.pop<object>();
            }
            if (((global::Doroti.Framework.Animation.AnimationController)this.controller).isAnimating)
            {
                this.controller.animateBack(0.0, duration: RouteLibrary._kDroppedSwipePageAnimationDuration, curve: animationCurve__28787);
            }
        }
        if (((global::Doroti.Framework.Animation.AnimationController)this.controller).isAnimating)
        {
            AnimationStatusListener animationStatusCallback__30647 = default!;
            animationStatusCallback__30647 = ((status) =>
            {
                this.navigator.didStopUserGesture();
                this.controller.removeStatusListener((AnimationStatusListener)animationStatusCallback__30647);
            });
            this.controller.addStatusListener((AnimationStatusListener)animationStatusCallback__30647);
        }
        else
        {
            this.navigator.didStopUserGesture();
        }
    }

}

public class _CupertinoEdgeShadowDecoration__route : global::Doroti.Framework.Painting.Decoration
{
    public static global::Doroti.Framework.Widgets.DecorationTween kTween = new global::Doroti.Framework.Widgets.DecorationTween(begin: new _CupertinoEdgeShadowDecoration__route(), end: new _CupertinoEdgeShadowDecoration__route(new List<global::Doroti.Ui.Color> { new global::Doroti.Ui.Color(67108864L), CupertinoColors.transparent }));
    internal virtual List<Color>? _colors { get; private set; }

    internal _CupertinoEdgeShadowDecoration__route(List<Color>? _colors = null)
    {
        this._colors = _colors;
    }

    public static _CupertinoEdgeShadowDecoration__route? lerp(_CupertinoEdgeShadowDecoration__route? a, _CupertinoEdgeShadowDecoration__route? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if ((a is null))
        {
            return ((b!._colors is null) ? b : new _CupertinoEdgeShadowDecoration__route(((_CupertinoEdgeShadowDecoration__route)b)._colors!.map<Color, Color>(((color) => Dart_uiLibrary.Color.lerp(null, color, t)!)).ToList()));
        }
        if ((b is null))
        {
            return ((((_CupertinoEdgeShadowDecoration__route)a)._colors is null) ? a : new _CupertinoEdgeShadowDecoration__route(((_CupertinoEdgeShadowDecoration__route)a)._colors.map<Color, Color>(((color) => Dart_uiLibrary.Color.lerp(null, color, (1.0 - t))!)).ToList()));
        }
        DartRuntimePrimitives.Assert(() => ((((_CupertinoEdgeShadowDecoration__route)b)._colors is not null) || (((_CupertinoEdgeShadowDecoration__route)a)._colors is not null)));
        DartRuntimePrimitives.Assert(() => (((((_CupertinoEdgeShadowDecoration__route)b)._colors is null) || (((_CupertinoEdgeShadowDecoration__route)a)._colors is null)) || (checked((long)(((_CupertinoEdgeShadowDecoration__route)a)._colors.Count)) == checked((long)(((_CupertinoEdgeShadowDecoration__route)b)._colors.Count)))));
        return new _CupertinoEdgeShadowDecoration__route(((Func<List<global::Doroti.Ui.Color>>)(() => { var __collection33897 = new List<global::Doroti.Ui.Color>(); for (long i__33921 = 0L; (i__33921 < checked((long)(((_CupertinoEdgeShadowDecoration__route)b)._colors!.Count))); i__33921 += 1L) { __collection33897.Add(Dart_uiLibrary.Color.lerp(((_CupertinoEdgeShadowDecoration__route)a)._colors[(int)(i__33921)], ((_CupertinoEdgeShadowDecoration__route)b)._colors[(int)(i__33921)], t)!); } return __collection33897; }))());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _CupertinoEdgeShadowDecoration__route lerpFrom(global::Doroti.Framework.Painting.Decoration? a, double t)
    {
        if ((a is _CupertinoEdgeShadowDecoration__route))
        {
            _CupertinoEdgeShadowDecoration__route a__as34106 = (_CupertinoEdgeShadowDecoration__route)a;
            return _CupertinoEdgeShadowDecoration__route.lerp(((_CupertinoEdgeShadowDecoration__route)a__as34106), this, t)!;
        }
        return _CupertinoEdgeShadowDecoration__route.lerp(null, this, t)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _CupertinoEdgeShadowDecoration__route lerpTo(global::Doroti.Framework.Painting.Decoration? b, double t)
    {
        if ((b is _CupertinoEdgeShadowDecoration__route))
        {
            _CupertinoEdgeShadowDecoration__route b__as34370 = (_CupertinoEdgeShadowDecoration__route)b;
            return _CupertinoEdgeShadowDecoration__route.lerp(this, ((_CupertinoEdgeShadowDecoration__route)b__as34370), t)!;
        }
        return _CupertinoEdgeShadowDecoration__route.lerp(this, null, t)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _CupertinoEdgeShadowPainter__route createBoxPainter(global::System.Action onChanged = default!)
    {
        return new _CupertinoEdgeShadowPainter__route(this, () => onChanged());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as _CupertinoEdgeShadowDecoration__route;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((__other is _CupertinoEdgeShadowDecoration__route) && (object.Equals(((_CupertinoEdgeShadowDecoration__route)((_CupertinoEdgeShadowDecoration__route)__other))._colors, this._colors)));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(this._colors.GetHashCode());
    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.IterableProperty<global::Doroti.Ui.Color>("colors", this._colors.Cast<global::Doroti.Ui.Color>()));
    }

}

public class _CupertinoEdgeShadowPainter__route : global::Doroti.Framework.Painting.BoxPainter
{
    internal virtual _CupertinoEdgeShadowDecoration__route _decoration { get; private set; } = default!;

    internal _CupertinoEdgeShadowPainter__route(_CupertinoEdgeShadowDecoration__route _decoration, global::System.Action? onChanged) : base(onChanged)
    {
        this._decoration = _decoration;
        System.Diagnostics.Debug.Assert(((((_CupertinoEdgeShadowDecoration__route)_decoration)._colors is null) || (checked((long)(((_CupertinoEdgeShadowDecoration__route)_decoration)._colors.Count)) > 1L)));
    }

    public override void paint(Canvas canvas, Offset offset, global::Doroti.Framework.Painting.ImageConfiguration configuration)
    {
        List<global::Doroti.Ui.Color>? colors__35588 = ((_CupertinoEdgeShadowDecoration__route)this._decoration)._colors.ToList();
        if ((colors__35588 is null))
        {
            return;
        }
        double shadowWidth__36919 = (0.05 * DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Painting.ImageConfiguration)configuration).size).width);
        double shadowHeight__36984 = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Painting.ImageConfiguration)configuration).size).height;
        double bandWidth__37044 = (shadowWidth__36919 / ((checked((long)(colors__35588.Count)) - 1L)));
        global::Doroti.Ui.TextDirection? textDirection__37117 = ((global::Doroti.Framework.Painting.ImageConfiguration)configuration).textDirection;
        DartRuntimePrimitives.Assert(() => (textDirection__37117 is not null));
        var (shadowDirection__37215, start__37239) = (DartRuntimePrimitives.RequireValue(textDirection__37117) switch { TextDirection.rtl => (((double, double))((1, (offset.dx + DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Painting.ImageConfiguration)configuration).size).width)))), TextDirection.ltr => (((double, double))((-1, offset.dx))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        var bandColorIndex__37405 = 0L;
        for (var dx__37438 = 0L; (dx__37438 < shadowWidth__36919); dx__37438 += 1L)
        {
            if (((checked((long)(dx__37438 / bandWidth__37044))) != bandColorIndex__37405))
            {
                bandColorIndex__37405 += 1L;
            }
            var paint__37571 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = Dart_uiLibrary.Color.lerp(colors__35588[(int)(bandColorIndex__37405)], colors__35588[(int)((bandColorIndex__37405 + 1L))], (((dx__37438 % bandWidth__37044)) / bandWidth__37044))!;
    return __cascade;
}))();
            double x__37760 = (start__37239 + (shadowDirection__37215 * dx__37438));
            canvas.drawRect(global::Doroti.Ui.Rect.fromLTWH((x__37760 - 1.0), offset.dy, 1.0, shadowHeight__36984), paint__37571);
        }
    }

}

public static partial class RouteLibrary
{
    internal static double _kStandardStiffness = 522.35;
}

public static partial class RouteLibrary
{
    internal static double _kStandardDamping = 45.7099552;
}

public static partial class RouteLibrary
{
    internal static global::Doroti.Framework.Physics.SpringDescription _kStandardSpring = new global::Doroti.Framework.Physics.SpringDescription(mass: 1, stiffness: RouteLibrary._kStandardStiffness, damping: RouteLibrary._kStandardDamping);
}

public static partial class RouteLibrary
{
    internal static global::Doroti.Framework.Physics.Tolerance _kStandardTolerance = new global::Doroti.Framework.Physics.Tolerance(velocity: 0.03);
}

public class CupertinoModalPopupRoute<T> : global::Doroti.Framework.Widgets.PopupRoute<T>
{
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> builder { get; private set; } = default!;
    internal virtual bool _barrierDismissible { get; private set; } = default!;
    internal virtual bool _semanticsDismissible { get; private set; } = default!;
    private string? __field_barrierLabel = default!;
    public override string? barrierLabel { get => __field_barrierLabel; }
    private Color? __field_barrierColor = default!;
    public override Color? barrierColor { get => __field_barrierColor; }
    public virtual Offset? anchorPoint { get; private set; }
    internal static global::Doroti.Framework.Animation.Tween<Offset> _offsetTween = new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: new global::Doroti.Ui.Offset(0.0, 1.0), end: Offset.zero);

    public CupertinoModalPopupRoute(global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> builder, string barrierLabel = "Dismiss", Color? barrierColor = default!, bool barrierDismissible = true, bool semanticsDismissible = false, ImageFilter? filter = null, global::Doroti.Framework.Widgets.RouteSettings? settings = null, bool? requestFocus = null, Offset? anchorPoint = null) : base(filter: filter, settings: settings, requestFocus: requestFocus)
    {
        Color? __barrierColor = barrierColor ?? RouteLibrary.kCupertinoModalBarrierColor;
        this.builder = builder;
        this.__field_barrierLabel = barrierLabel;
        this.__field_barrierColor = __barrierColor;
        this.anchorPoint = anchorPoint;
        this._barrierDismissible = barrierDismissible;
        this._semanticsDismissible = semanticsDismissible;
    }

    public override bool barrierDismissible => this._barrierDismissible;
    public override bool semanticsDismissible => this._semanticsDismissible;
    public override Duration transitionDuration => RouteLibrary._kModalPopupTransitionDuration;
    public override global::Doroti.Framework.Physics.Simulation? createSimulation(bool forward)
    {
        DartRuntimePrimitives.Assert(() => !debugTransitionCompleted(), () => (object?)$"Cannot reuse a {this.GetType()} after disposing it.");
        var end__42370 = (forward ? 1.0 : 0.0);
        return ((global::Doroti.Framework.Physics.Simulation?)(object?)new global::Doroti.Framework.Physics.SpringSimulation(RouteLibrary._kStandardSpring, this.controller!.value, end__42370, 0, tolerance: RouteLibrary._kStandardTolerance, snapToEnd: true));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget buildPage(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new CupertinoUserInterfaceLevel(data: CupertinoUserInterfaceLevelData.elevated, child: new global::Doroti.Framework.Widgets.DisplayFeatureSubScreen(anchorPoint: this.anchorPoint, child: new global::Doroti.Framework.Widgets.Builder(builder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)this.builder))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget buildTransitions(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.Alignment.bottomCenter, child: new global::Doroti.Framework.Widgets.FractionalTranslation(translation: _offsetTween.evaluate(animation), child: child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class RouteLibrary
{
    public static Future<T?> showCupertinoModalPopup<T>(global::Doroti.Framework.Widgets.BuildContext context, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> builder, ImageFilter? filter = null, Color barrierColor = default!, bool barrierDismissible = true, bool useRootNavigator = true, bool semanticsDismissible = false, global::Doroti.Framework.Widgets.RouteSettings? routeSettings = null, Offset? anchorPoint = null, bool? requestFocus = null)
    {
        return ((Future<T?>)(object?)Navigator.of(context, rootNavigator: useRootNavigator).push(new CupertinoModalPopupRoute<T>(builder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)builder, filter: filter, barrierColor: CupertinoDynamicColor.resolve(barrierColor, context), barrierDismissible: barrierDismissible, semanticsDismissible: semanticsDismissible, settings: routeSettings, anchorPoint: anchorPoint, requestFocus: requestFocus)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class RouteLibrary
{
    internal static global::Doroti.Framework.Widgets.Widget _buildCupertinoDialogTransitions(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget child)
    {
        return child;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class RouteLibrary
{
    public static Future<T?> showCupertinoDialog<T>(global::Doroti.Framework.Widgets.BuildContext context, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> builder, string? barrierLabel = null, Color? barrierColor = null, bool useRootNavigator = true, bool barrierDismissible = false, global::Doroti.Framework.Widgets.RouteSettings? routeSettings = null, Offset? anchorPoint = null, bool? requestFocus = null)
    {
        return ((Future<T?>)(object?)Navigator.of(context, rootNavigator: useRootNavigator).push<T>(new CupertinoDialogRoute<T>(builder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)builder, context: context, barrierDismissible: barrierDismissible, barrierLabel: barrierLabel, barrierColor: barrierColor, settings: routeSettings, anchorPoint: anchorPoint, requestFocus: requestFocus)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class CupertinoDialogRoute<T> : global::Doroti.Framework.Widgets.RawDialogRoute<T>
{
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Widgets.Widget>? transitionBuilder { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation? _fadeAnimation { get; set; } = default;
    internal static global::Doroti.Framework.Animation.Tween<double> _dialogScaleTween = new global::Doroti.Framework.Animation.Tween<double>(begin: 1.3, end: 1.0);

    public CupertinoDialogRoute(global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> builder, global::Doroti.Framework.Widgets.BuildContext context, bool barrierDismissible = true, Color? barrierColor = null, string? barrierLabel = null, Duration? transitionDuration = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Widgets.Widget>? transitionBuilder = null, global::Doroti.Framework.Widgets.RouteSettings? settings = null, bool? requestFocus = null, Offset? anchorPoint = null) : base(barrierDismissible: barrierDismissible, transitionDuration: transitionDuration ?? Duration.Create(milliseconds: 250), settings: settings, requestFocus: requestFocus, anchorPoint: anchorPoint, pageBuilder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget>)((context, animation, secondaryAnimation) =>
    {
        return builder(context);
        throw new InvalidOperationException("Dart closure completed without a value.");
    })), transitionBuilder: ((transitionBuilder ?? (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Widgets.Widget>)RouteLibrary._buildCupertinoDialogTransitions)), barrierLabel: (barrierLabel ?? CupertinoLocalizations.of(context).modalBarrierDismissLabel), barrierColor: (barrierColor ?? CupertinoDynamicColor.resolve(RouteLibrary.kCupertinoModalBarrierColor, context)))
    {
        this.transitionBuilder = transitionBuilder;
    }

    public override global::Doroti.Framework.Physics.Simulation? createSimulation(bool forward)
    {
        DartRuntimePrimitives.Assert(() => !debugTransitionCompleted(), () => (object?)$"Cannot reuse a {this.GetType()} after disposing it.");
        var end__54336 = (forward ? 1.0 : 0.0);
        return ((global::Doroti.Framework.Physics.Simulation?)(object?)new global::Doroti.Framework.Physics.SpringSimulation(RouteLibrary._kStandardSpring, this.controller!.value, end__54336, 0, tolerance: RouteLibrary._kStandardTolerance, snapToEnd: true));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget buildTransitions(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget child)
    {
        if ((this.transitionBuilder is not null))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)base.buildTransitions(context, animation, secondaryAnimation, child));
        }
        if ((object.Equals(((global::Doroti.Framework.Animation.Animation<double>)animation).status, global::Doroti.Framework.Animation.AnimationStatus.reverse)))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.FadeTransition(opacity: animation, child: child));
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.FadeTransition(opacity: animation, child: new global::Doroti.Framework.Widgets.ScaleTransition(scale: animation.drive(_dialogScaleTween), child: child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        this._fadeAnimation?.dispose();
        base.dispose();
    }

}

public class CupertinoPageTransitionsBuilder : global::Doroti.Framework.Widgets.PageTransitionsBuilder
{
    public CupertinoPageTransitionsBuilder()
    {
    }

    public override Duration transitionDuration => CupertinoRouteTransitionMixin<object>.kTransitionDuration;
    public override global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, bool, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget?>? delegatedTransition => CupertinoPageTransition.delegatedTransition;
    public override global::Doroti.Framework.Widgets.Widget buildTransitions<T>(global::Doroti.Framework.Widgets.PageRoute<T> route, global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)CupertinoRouteTransitionMixin<object>.buildPageTransitions<T>(route, context, animation, secondaryAnimation, child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
