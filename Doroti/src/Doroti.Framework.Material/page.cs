// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/page.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

internal static class MaterialPageTransitionResolver
{
    internal static PageTransitionsBuilder Resolve(
        BuildContext context)
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

public class MaterialPageRoute<T> : PageRoute<T>, MaterialRouteTransitionMixin<T>
{
    public virtual Func<BuildContext, Widget> builder { get; private set; } = default!;
    private bool __field_maintainState = default!;
    public override bool maintainState { get => __field_maintainState; }

    public MaterialPageRoute(Func<BuildContext, Widget> builder, RouteSettings? settings = null, bool? requestFocus = null, bool maintainState = true, bool fullscreenDialog = false, bool allowSnapshotting = true, bool barrierDismissible = false, TraversalEdgeBehavior? traversalEdgeBehavior = null, TraversalEdgeBehavior? directionalTraversalEdgeBehavior = null) : base(settings: settings, requestFocus: requestFocus, fullscreenDialog: fullscreenDialog, allowSnapshotting: allowSnapshotting, barrierDismissible: barrierDismissible, traversalEdgeBehavior: traversalEdgeBehavior, directionalTraversalEdgeBehavior: directionalTraversalEdgeBehavior)
    {
        this.builder = builder;
        __field_maintainState = maintainState;
        DartRuntimePrimitives.Assert(() => opaque);
    }

    public virtual Widget buildContent(BuildContext context) => builder(context);
    public override string debugLabel => $"{base.debugLabel}({settings.name})";
    public override Duration transitionDuration => MaterialPageTransitionResolver.Resolve(navigator!.context).transitionDuration;
    public override Duration reverseTransitionDuration => MaterialPageTransitionResolver.Resolve(navigator!.context).reverseTransitionDuration;
    public virtual PageTransitionsBuilder? _getPageTransitionBuilder(BuildContext context)
        => MaterialPageTransitionResolver.Resolve(context);

    public override Scheduler.TickerFuture didPush()
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
    public override Func<BuildContext, Animation<double>, Animation<double>, bool, Widget?, Widget?>? delegatedTransition => MaterialRouteTransitionMixin<T>._delegatedTransition;
    public override bool canTransitionTo(dynamic nextRoute)
    {
        bool nextRouteIsNotFullscreen = nextRoute is not IPageRoute || !((IPageRoute)(object)nextRoute).fullscreenDialog;
        bool nextRouteHasDelegatedTransition = (nextRoute is IModalRoute) && (((IModalRoute)(object)nextRoute).delegatedTransition is not null);
        return nextRouteIsNotFullscreen && (nextRoute is IMaterialRouteTransition || nextRouteHasDelegatedTransition);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool canTransitionFrom(dynamic previousRoute)
    {
        return (previousRoute is IPageRoute) && !fullscreenDialog;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget buildPage(BuildContext context, Animation<double> animation, Animation<double> secondaryAnimation)
    {
        Widget result = buildContent(context);
        return new Widgets.Semantics(scopesRoute: true, explicitChildNodes: true, child: result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget buildTransitions(BuildContext context, Animation<double> animation, Animation<double> secondaryAnimation, Widget child)
    {
        PageTransitionsTheme theme = Theme.of(context).pageTransitionsTheme;
        return theme.buildTransitions(this, context, animation, secondaryAnimation, child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public interface IMaterialRouteTransition { }

public interface MaterialRouteTransitionMixin<T> : IMaterialRouteTransition
{
    public Widget buildContent(BuildContext context);
    public Duration transitionDuration { get; }
    public Duration reverseTransitionDuration { get; }
    public PageTransitionsBuilder? _getPageTransitionBuilder(BuildContext context);
    public Scheduler.TickerFuture didPush();
    public bool didPop(T? result);
    public Color? barrierColor { get; }
    public string? barrierLabel { get; }
    public Func<BuildContext, Animation<double>, Animation<double>, bool, Widget?, Widget?>? delegatedTransition { get; }
    public static Widget? _delegatedTransition(BuildContext context, Animation<double> animation, Animation<double> secondaryAnimation, bool allowSnapshotting, Widget? child)
    {
        PageTransitionsTheme theme = Theme.of(context).pageTransitionsTheme;
        TargetPlatform platformLocal = Theme.of(context).platform;
        Func<BuildContext, Animation<double>, Animation<double>, bool, Widget?, Widget?>? themeDelegatedTransition = theme.delegatedTransition(platformLocal);
        return (themeDelegatedTransition is not null) ? themeDelegatedTransition(context, animation, secondaryAnimation, allowSnapshotting, child) : null;
    }
    public bool canTransitionTo(dynamic nextRoute);
    public bool canTransitionFrom(dynamic previousRoute);
    public Widget buildPage(BuildContext context, Animation<double> animation, Animation<double> secondaryAnimation);
    public Widget buildTransitions(BuildContext context, Animation<double> animation, Animation<double> secondaryAnimation, Widget child);
}

public class MaterialPage<T> : Page<T>
{
    public virtual Widget child { get; private set; } = default!;
    public virtual bool maintainState { get; private set; } = default!;
    public virtual bool fullscreenDialog { get; private set; } = default!;
    public virtual bool allowSnapshotting { get; private set; } = default!;

    public MaterialPage(Widget child, bool maintainState = true, bool fullscreenDialog = false, bool allowSnapshotting = true, LocalKey? key = null, bool canPop = true, Action<bool, T?> onPopInvoked = default!, string? name = null, object? arguments = null, string? restorationId = null) : base(key: key, canPop: canPop, onPopInvoked: onPopInvoked ?? ((didPop, result) => _defaultPopInvokedHandler(didPop, result)), name: name, arguments: arguments, restorationId: restorationId)
    {
        this.child = child;
        this.maintainState = maintainState;
        this.fullscreenDialog = fullscreenDialog;
        this.allowSnapshotting = allowSnapshotting;
    }

    public override Route<T> createRoute(BuildContext context)
    {
        return new _PageBasedMaterialPageRoute__page<T>(page: this, allowSnapshotting: allowSnapshotting);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _PageBasedMaterialPageRoute__page<T> : PageRoute<T>, MaterialRouteTransitionMixin<T>
{

    internal _PageBasedMaterialPageRoute__page(MaterialPage<T> page, bool allowSnapshotting = true) : base(allowSnapshotting: allowSnapshotting, settings: page)
    {
        DartRuntimePrimitives.Assert(() => opaque);
    }

    internal virtual MaterialPage<T> _page => ((MaterialPage<T>?)settings)!;
    public virtual Widget buildContent(BuildContext context)
    {
        return _page.child;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool maintainState => _page.maintainState;
    public override bool fullscreenDialog => _page.fullscreenDialog;
    public override string debugLabel => $"{base.debugLabel}({_page.name})";
    public override Duration transitionDuration => MaterialPageTransitionResolver.Resolve(navigator!.context).transitionDuration;
    public override Duration reverseTransitionDuration => MaterialPageTransitionResolver.Resolve(navigator!.context).reverseTransitionDuration;
    public virtual PageTransitionsBuilder? _getPageTransitionBuilder(BuildContext context)
        => MaterialPageTransitionResolver.Resolve(context);

    public override Scheduler.TickerFuture didPush()
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
    public override Func<BuildContext, Animation<double>, Animation<double>, bool, Widget?, Widget?>? delegatedTransition => MaterialRouteTransitionMixin<T>._delegatedTransition;
    public override bool canTransitionTo(dynamic nextRoute)
    {
        bool nextRouteIsNotFullscreen = nextRoute is not IPageRoute || !((IPageRoute)(object)nextRoute).fullscreenDialog;
        bool nextRouteHasDelegatedTransition = (nextRoute is IModalRoute) && (((IModalRoute)(object)nextRoute).delegatedTransition is not null);
        return nextRouteIsNotFullscreen && (nextRoute is IMaterialRouteTransition || nextRouteHasDelegatedTransition);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool canTransitionFrom(dynamic previousRoute)
    {
        return (previousRoute is IPageRoute) && !fullscreenDialog;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget buildPage(BuildContext context, Animation<double> animation, Animation<double> secondaryAnimation)
    {
        Widget result = buildContent(context);
        return new Widgets.Semantics(scopesRoute: true, explicitChildNodes: true, child: result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget buildTransitions(BuildContext context, Animation<double> animation, Animation<double> secondaryAnimation, Widget child)
    {
        PageTransitionsTheme theme = Theme.of(context).pageTransitionsTheme;
        return theme.buildTransitions(this, context, animation, secondaryAnimation, child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
