// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/page.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

internal static class MaterialPageTransitionResolver
{
    internal static global::Doroti.Framework.Widgets.PageTransitionsBuilder Resolve(
        global::Doroti.Framework.Widgets.BuildContext context)
    {
        var theme = Theme.of(context);
        var platform = theme.platform;
        if (theme.pageTransitionsTheme.builders.TryGetValue(platform, out var builder))
        {
            return builder;
        }
        return platform switch
        {
            TargetPlatform.iOS or
            TargetPlatform.macOS => new CupertinoPageTransitionsBuilder(),
            TargetPlatform.android or
            TargetPlatform.fuchsia or
            TargetPlatform.windows or
            TargetPlatform.linux => new ZoomPageTransitionsBuilder(),
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
    }
}

public class MaterialPageRoute<T> : global::Doroti.Framework.Widgets.PageRoute<T>, MaterialRouteTransitionMixin<T>
{
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> builder { get; private set; } = default!;
    private bool __field_maintainState = default!;
    public override bool maintainState { get => __field_maintainState; }

    public MaterialPageRoute(global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> builder, global::Doroti.Framework.Widgets.RouteSettings? settings = null, bool? requestFocus = null, bool maintainState = true, bool fullscreenDialog = false, bool allowSnapshotting = true, bool barrierDismissible = false, global::Doroti.Framework.Widgets.TraversalEdgeBehavior? traversalEdgeBehavior = null, global::Doroti.Framework.Widgets.TraversalEdgeBehavior? directionalTraversalEdgeBehavior = null) : base(settings: settings, requestFocus: requestFocus, fullscreenDialog: fullscreenDialog, allowSnapshotting: allowSnapshotting, barrierDismissible: barrierDismissible, traversalEdgeBehavior: traversalEdgeBehavior, directionalTraversalEdgeBehavior: directionalTraversalEdgeBehavior)
    {
        this.builder = builder;
        __field_maintainState = maintainState;
        DartRuntimePrimitives.Assert(() => opaque);
    }

    public virtual global::Doroti.Framework.Widgets.Widget buildContent(global::Doroti.Framework.Widgets.BuildContext context) => builder(context);
    public override string debugLabel => $"{base.debugLabel}({settings.name})";
    public override Duration transitionDuration => MaterialPageTransitionResolver.Resolve(navigator!.context).transitionDuration;
    public override Duration reverseTransitionDuration => MaterialPageTransitionResolver.Resolve(navigator!.context).reverseTransitionDuration;
    public virtual global::Doroti.Framework.Widgets.PageTransitionsBuilder? _getPageTransitionBuilder(global::Doroti.Framework.Widgets.BuildContext context)
        => MaterialPageTransitionResolver.Resolve(context);

    public override global::Doroti.Framework.Scheduler.TickerFuture didPush()
    {
        controller?.duration = transitionDuration;
        return base.didPush();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool didPop(T? result)
    {
        controller?.reverseDuration = reverseTransitionDuration;
        return base.didPop(result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Color? barrierColor => DartRuntimePrimitives.ConvertValue<Color>(null);
    public override string? barrierLabel => DartRuntimePrimitives.ConvertValue<string>(null);
    public override global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, bool, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget?>? delegatedTransition => MaterialRouteTransitionMixin<T>._delegatedTransition;
    public override bool canTransitionTo(dynamic nextRoute)
    {
        bool nextRouteIsNotFullscreen = nextRoute is not IPageRoute || !((global::Doroti.Framework.Widgets.IPageRoute)(object)nextRoute).fullscreenDialog;
        bool nextRouteHasDelegatedTransition = (nextRoute is global::Doroti.Framework.Widgets.IModalRoute) && (((global::Doroti.Framework.Widgets.IModalRoute)(object)nextRoute).delegatedTransition is not null);
        return nextRouteIsNotFullscreen && (nextRoute is IMaterialRouteTransition || nextRouteHasDelegatedTransition);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool canTransitionFrom(dynamic previousRoute)
    {
        return (previousRoute is IPageRoute) && !fullscreenDialog;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget buildPage(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation)
    {
        global::Doroti.Framework.Widgets.Widget result = buildContent(context);
        return new global::Doroti.Framework.Widgets.Semantics(scopesRoute: true, explicitChildNodes: true, child: result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget buildTransitions(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget child)
    {
        PageTransitionsTheme theme = Theme.of(context).pageTransitionsTheme;
        return theme.buildTransitions<T>(this, context, animation, secondaryAnimation, child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public interface IMaterialRouteTransition { }

public interface MaterialRouteTransitionMixin<T> : IMaterialRouteTransition
{
    public global::Doroti.Framework.Widgets.Widget buildContent(global::Doroti.Framework.Widgets.BuildContext context);
    public Duration transitionDuration { get; }
    public Duration reverseTransitionDuration { get; }
    public global::Doroti.Framework.Widgets.PageTransitionsBuilder? _getPageTransitionBuilder(global::Doroti.Framework.Widgets.BuildContext context);
    public global::Doroti.Framework.Scheduler.TickerFuture didPush();
    public bool didPop(T? result);
    public global::Doroti.Ui.Color? barrierColor { get; }
    public string? barrierLabel { get; }
    public global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, bool, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget?>? delegatedTransition { get; }
    public static global::Doroti.Framework.Widgets.Widget? _delegatedTransition(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, bool allowSnapshotting, global::Doroti.Framework.Widgets.Widget? child)
    {
        PageTransitionsTheme theme = Theme.of(context).pageTransitionsTheme;
        global::Doroti.Framework.Foundation.TargetPlatform platformLocal = Theme.of(context).platform;
        global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, bool, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget?>? themeDelegatedTransition = theme.delegatedTransition(platformLocal);
        return (themeDelegatedTransition is not null) ? themeDelegatedTransition(context, animation, secondaryAnimation, allowSnapshotting, child) : null;
    }
    public bool canTransitionTo(dynamic nextRoute);
    public bool canTransitionFrom(dynamic previousRoute);
    public global::Doroti.Framework.Widgets.Widget buildPage(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation);
    public global::Doroti.Framework.Widgets.Widget buildTransitions(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget child);
}

public class MaterialPage<T> : global::Doroti.Framework.Widgets.Page<T>
{
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual bool maintainState { get; private set; } = default!;
    public virtual bool fullscreenDialog { get; private set; } = default!;
    public virtual bool allowSnapshotting { get; private set; } = default!;

    public MaterialPage(global::Doroti.Framework.Widgets.Widget child, bool maintainState = true, bool fullscreenDialog = false, bool allowSnapshotting = true, global::Doroti.Framework.Foundation.LocalKey? key = null, bool canPop = true, global::System.Action<bool, T?> onPopInvoked = default!, string? name = null, object? arguments = null, string? restorationId = null) : base(key: key, canPop: canPop, onPopInvoked: onPopInvoked ?? ((didPop, result) => _defaultPopInvokedHandler(didPop, result)), name: name, arguments: arguments, restorationId: restorationId)
    {
        this.child = child;
        this.maintainState = maintainState;
        this.fullscreenDialog = fullscreenDialog;
        this.allowSnapshotting = allowSnapshotting;
    }

    public override global::Doroti.Framework.Widgets.Route<T> createRoute(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new _PageBasedMaterialPageRoute__page<T>(page: this, allowSnapshotting: allowSnapshotting);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _PageBasedMaterialPageRoute__page<T> : global::Doroti.Framework.Widgets.PageRoute<T>, MaterialRouteTransitionMixin<T>
{

    internal _PageBasedMaterialPageRoute__page(MaterialPage<T> page, bool allowSnapshotting = true) : base(allowSnapshotting: allowSnapshotting, settings: page)
    {
        DartRuntimePrimitives.Assert(() => opaque);
    }

    internal virtual MaterialPage<T> _page => ((MaterialPage<T>?)settings)!;
    public virtual global::Doroti.Framework.Widgets.Widget buildContent(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return _page.child;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool maintainState => _page.maintainState;
    public override bool fullscreenDialog => _page.fullscreenDialog;
    public override string debugLabel => $"{base.debugLabel}({_page.name})";
    public override Duration transitionDuration => MaterialPageTransitionResolver.Resolve(navigator!.context).transitionDuration;
    public override Duration reverseTransitionDuration => MaterialPageTransitionResolver.Resolve(navigator!.context).reverseTransitionDuration;
    public virtual global::Doroti.Framework.Widgets.PageTransitionsBuilder? _getPageTransitionBuilder(global::Doroti.Framework.Widgets.BuildContext context)
        => MaterialPageTransitionResolver.Resolve(context);

    public override global::Doroti.Framework.Scheduler.TickerFuture didPush()
    {
        controller?.duration = transitionDuration;
        return base.didPush();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool didPop(T? result)
    {
        controller?.reverseDuration = reverseTransitionDuration;
        return base.didPop(result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Color? barrierColor => DartRuntimePrimitives.ConvertValue<Color>(null);
    public override string? barrierLabel => DartRuntimePrimitives.ConvertValue<string>(null);
    public override global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, bool, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget?>? delegatedTransition => MaterialRouteTransitionMixin<T>._delegatedTransition;
    public override bool canTransitionTo(dynamic nextRoute)
    {
        bool nextRouteIsNotFullscreen = nextRoute is not IPageRoute || !((global::Doroti.Framework.Widgets.IPageRoute)(object)nextRoute).fullscreenDialog;
        bool nextRouteHasDelegatedTransition = (nextRoute is global::Doroti.Framework.Widgets.IModalRoute) && (((global::Doroti.Framework.Widgets.IModalRoute)(object)nextRoute).delegatedTransition is not null);
        return nextRouteIsNotFullscreen && (nextRoute is IMaterialRouteTransition || nextRouteHasDelegatedTransition);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool canTransitionFrom(dynamic previousRoute)
    {
        return (previousRoute is IPageRoute) && !fullscreenDialog;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget buildPage(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation)
    {
        global::Doroti.Framework.Widgets.Widget result = buildContent(context);
        return new global::Doroti.Framework.Widgets.Semantics(scopesRoute: true, explicitChildNodes: true, child: result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget buildTransitions(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget child)
    {
        PageTransitionsTheme theme = Theme.of(context).pageTransitionsTheme;
        return theme.buildTransitions<T>(this, context, animation, secondaryAnimation, child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
