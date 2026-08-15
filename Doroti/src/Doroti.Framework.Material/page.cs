// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/page.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public class MaterialPageRoute<T> : global::Doroti.Generated.Framework.Widgets.PageRoute<T>, MaterialRouteTransitionMixin<T>
{
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget> builder { get; private set; } = default!;
    private bool __field_maintainState = default!;
    public override bool maintainState { get => __field_maintainState; }

    public MaterialPageRoute(global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget> builder, global::Doroti.Generated.Framework.Widgets.RouteSettings? settings = null, bool? requestFocus = null, bool maintainState = true, bool fullscreenDialog = false, bool allowSnapshotting = true, bool barrierDismissible = false, global::Doroti.Generated.Framework.Widgets.TraversalEdgeBehavior? traversalEdgeBehavior = null, global::Doroti.Generated.Framework.Widgets.TraversalEdgeBehavior? directionalTraversalEdgeBehavior = null) : base(settings: settings, requestFocus: requestFocus, fullscreenDialog: fullscreenDialog, allowSnapshotting: allowSnapshotting, barrierDismissible: barrierDismissible, traversalEdgeBehavior: traversalEdgeBehavior, directionalTraversalEdgeBehavior: directionalTraversalEdgeBehavior)
    {
        this.builder = builder;
        this.__field_maintainState = maintainState;
        DartRuntimePrimitives.Assert(() => this.opaque);
    }

    public virtual global::Doroti.Generated.Framework.Widgets.Widget buildContent(global::Doroti.Generated.Framework.Widgets.BuildContext context) => this.builder(context);
    public override string debugLabel => $"{base.debugLabel}({(((global::Doroti.Generated.Framework.Widgets.RouteSettings)this.settings).name)})";
    public override Duration transitionDuration => DartRuntimePrimitives.ConvertValue<Duration>((_getPageTransitionBuilder(this.navigator!.context)?.transitionDuration ?? Duration.Create(microseconds: 300L)));
    public override Duration reverseTransitionDuration => DartRuntimePrimitives.ConvertValue<Duration>((_getPageTransitionBuilder(this.navigator!.context)?.reverseTransitionDuration ?? Duration.Create(microseconds: 300L)));
    public virtual global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder? _getPageTransitionBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Generated.Framework.Foundation.TargetPlatform platform__3711 = Theme.of(context).platform;
        PageTransitionsTheme pageTransitionsTheme__3781 = Theme.of(context).pageTransitionsTheme;
        return (pageTransitionsTheme__3781.builders.GetValueOrDefault(platform__3711) ?? (platform__3711 switch { global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder>(new CupertinoPageTransitionsBuilder()), global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder>(new CupertinoPageTransitionsBuilder()), global::Doroti.Generated.Framework.Foundation.TargetPlatform.android or global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia or global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder>(new ZoomPageTransitionsBuilder()), global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder>(new ZoomPageTransitionsBuilder()), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Scheduler.TickerFuture didPush()
    {
        this.controller?.duration = this.transitionDuration;
        return ((global::Doroti.Generated.Framework.Scheduler.TickerFuture)(object?)base.didPush());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool didPop(T? result)
    {
        this.controller?.reverseDuration = this.reverseTransitionDuration;
        return base.didPop(result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Color? barrierColor => DartRuntimePrimitives.ConvertValue<Color>(null);
    public override string? barrierLabel => DartRuntimePrimitives.ConvertValue<string>(null);
    public override global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, bool, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget?>? delegatedTransition => MaterialRouteTransitionMixin<T>._delegatedTransition;
    public override bool canTransitionTo(dynamic nextRoute)
    {
        bool nextRouteIsNotFullscreen__6437 = (((nextRoute is not global::Doroti.Generated.Framework.Widgets.PageRoute<T>)) || !((global::Doroti.Generated.Framework.Widgets.PageRoute<T>)nextRoute).fullscreenDialog);
        bool nextRouteHasDelegatedTransition__6724 = ((nextRoute is global::Doroti.Generated.Framework.Widgets.ModalRoute<T>) && (((global::Doroti.Generated.Framework.Widgets.ModalRoute<T>)nextRoute).delegatedTransition is not null));
        return (nextRouteIsNotFullscreen__6437 && ((((nextRoute is MaterialRouteTransitionMixin<object>)) || nextRouteHasDelegatedTransition__6724)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool canTransitionFrom(dynamic previousRoute)
    {
        return ((previousRoute is PageRoute<object>) && !this.fullscreenDialog);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget buildPage(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation)
    {
        global::Doroti.Generated.Framework.Widgets.Widget result__7508 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)buildContent(context));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Semantics(scopesRoute: true, explicitChildNodes: true, child: result__7508));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget buildTransitions(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        PageTransitionsTheme theme__7822 = Theme.of(context).pageTransitionsTheme;
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)theme__7822.buildTransitions<T>(this, context, animation, secondaryAnimation, child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public interface MaterialRouteTransitionMixin<T>
{
    public global::Doroti.Generated.Framework.Widgets.Widget buildContent(global::Doroti.Generated.Framework.Widgets.BuildContext context);
    public Duration transitionDuration { get; }
    public Duration reverseTransitionDuration { get; }
    public global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder? _getPageTransitionBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context);
    public global::Doroti.Generated.Framework.Scheduler.TickerFuture didPush();
    public bool didPop(T? result);
    public global::Doroti.Ui.Color? barrierColor { get; }
    public string? barrierLabel { get; }
    public global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, bool, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget?>? delegatedTransition { get; }
    public static global::Doroti.Generated.Framework.Widgets.Widget? _delegatedTransition(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation, bool allowSnapshotting, global::Doroti.Generated.Framework.Widgets.Widget? child)
    {
        PageTransitionsTheme theme__5742 = Theme.of(context).pageTransitionsTheme;
        global::Doroti.Generated.Framework.Foundation.TargetPlatform platform__5815 = Theme.of(context).platform;
        global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, bool, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget?>? themeDelegatedTransition__5892 = theme__5742.delegatedTransition(platform__5815);
        return ((themeDelegatedTransition__5892 is not null) ? themeDelegatedTransition__5892(context, animation, secondaryAnimation, allowSnapshotting, child) : null);
    }
    public bool canTransitionTo(dynamic nextRoute);
    public bool canTransitionFrom(dynamic previousRoute);
    public global::Doroti.Generated.Framework.Widgets.Widget buildPage(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation);
    public global::Doroti.Generated.Framework.Widgets.Widget buildTransitions(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Generated.Framework.Widgets.Widget child);
}

public class MaterialPage<T> : global::Doroti.Generated.Framework.Widgets.Page<T>
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual bool maintainState { get; private set; } = default!;
    public virtual bool fullscreenDialog { get; private set; } = default!;
    public virtual bool allowSnapshotting { get; private set; } = default!;

    public MaterialPage(global::Doroti.Generated.Framework.Widgets.Widget child, bool maintainState = true, bool fullscreenDialog = false, bool allowSnapshotting = true, global::Doroti.Generated.Framework.Foundation.LocalKey? key = null, bool canPop = true, global::System.Action<bool, T?> onPopInvoked = default!, string? name = null, object? arguments = null, string? restorationId = null) : base(key: key, canPop: canPop, onPopInvoked: onPopInvoked ?? ((didPop, result) => Page<T>._defaultPopInvokedHandler(didPop, result)), name: name, arguments: arguments, restorationId: restorationId)
    {
        this.child = child;
        this.maintainState = maintainState;
        this.fullscreenDialog = fullscreenDialog;
        this.allowSnapshotting = allowSnapshotting;
    }

    public override global::Doroti.Generated.Framework.Widgets.Route<T> createRoute(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Route<T>)(object?)new _PageBasedMaterialPageRoute__page<T>(page: this, allowSnapshotting: this.allowSnapshotting));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _PageBasedMaterialPageRoute__page<T> : global::Doroti.Generated.Framework.Widgets.PageRoute<T>, MaterialRouteTransitionMixin<T>
{

    internal _PageBasedMaterialPageRoute__page(MaterialPage<T> page, bool allowSnapshotting = true) : base(allowSnapshotting: allowSnapshotting, settings: page)
    {
        DartRuntimePrimitives.Assert(() => this.opaque);
    }

    internal virtual MaterialPage<T> _page => ((MaterialPage<T>?)(object?)this.settings)!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget buildContent(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((MaterialPage<T>)this._page).child;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool maintainState => ((MaterialPage<T>)this._page).maintainState;
    public override bool fullscreenDialog => ((MaterialPage<T>)this._page).fullscreenDialog;
    public override string debugLabel => $"{base.debugLabel}({this._page.name})";
    public override Duration transitionDuration => DartRuntimePrimitives.ConvertValue<Duration>((_getPageTransitionBuilder(this.navigator!.context)?.transitionDuration ?? Duration.Create(microseconds: 300L)));
    public override Duration reverseTransitionDuration => DartRuntimePrimitives.ConvertValue<Duration>((_getPageTransitionBuilder(this.navigator!.context)?.reverseTransitionDuration ?? Duration.Create(microseconds: 300L)));
    public virtual global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder? _getPageTransitionBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Generated.Framework.Foundation.TargetPlatform platform__3711 = Theme.of(context).platform;
        PageTransitionsTheme pageTransitionsTheme__3781 = Theme.of(context).pageTransitionsTheme;
        return (pageTransitionsTheme__3781.builders.GetValueOrDefault(platform__3711) ?? (platform__3711 switch { global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder>(new CupertinoPageTransitionsBuilder()), global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder>(new CupertinoPageTransitionsBuilder()), global::Doroti.Generated.Framework.Foundation.TargetPlatform.android or global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia or global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder>(new ZoomPageTransitionsBuilder()), global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder>(new ZoomPageTransitionsBuilder()), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Scheduler.TickerFuture didPush()
    {
        this.controller?.duration = this.transitionDuration;
        return ((global::Doroti.Generated.Framework.Scheduler.TickerFuture)(object?)base.didPush());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool didPop(T? result)
    {
        this.controller?.reverseDuration = this.reverseTransitionDuration;
        return base.didPop(result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Color? barrierColor => DartRuntimePrimitives.ConvertValue<Color>(null);
    public override string? barrierLabel => DartRuntimePrimitives.ConvertValue<string>(null);
    public override global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, bool, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget?>? delegatedTransition => MaterialRouteTransitionMixin<T>._delegatedTransition;
    public override bool canTransitionTo(dynamic nextRoute)
    {
        bool nextRouteIsNotFullscreen__6437 = (((nextRoute is not global::Doroti.Generated.Framework.Widgets.PageRoute<T>)) || !((global::Doroti.Generated.Framework.Widgets.PageRoute<T>)nextRoute).fullscreenDialog);
        bool nextRouteHasDelegatedTransition__6724 = ((nextRoute is global::Doroti.Generated.Framework.Widgets.ModalRoute<T>) && (((global::Doroti.Generated.Framework.Widgets.ModalRoute<T>)nextRoute).delegatedTransition is not null));
        return (nextRouteIsNotFullscreen__6437 && ((((nextRoute is MaterialRouteTransitionMixin<object>)) || nextRouteHasDelegatedTransition__6724)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool canTransitionFrom(dynamic previousRoute)
    {
        return ((previousRoute is PageRoute<object>) && !this.fullscreenDialog);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget buildPage(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation)
    {
        global::Doroti.Generated.Framework.Widgets.Widget result__7508 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)buildContent(context));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Semantics(scopesRoute: true, explicitChildNodes: true, child: result__7508));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget buildTransitions(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        PageTransitionsTheme theme__7822 = Theme.of(context).pageTransitionsTheme;
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)theme__7822.buildTransitions<T>(this, context, animation, secondaryAnimation, child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
