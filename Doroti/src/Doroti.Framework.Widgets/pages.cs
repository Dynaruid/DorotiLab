// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/pages.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public abstract class PageRoute<T> : ModalRoute<T>, IPageRoute
{
    private bool __field_fullscreenDialog = default!;
    public override bool fullscreenDialog
    {
        get => __field_fullscreenDialog;
    }
    private bool __field_allowSnapshotting = default!;
    public override bool allowSnapshotting
    {
        get => __field_allowSnapshotting;
    }
    internal virtual bool _barrierDismissible { get; private set; } = default!;

    protected PageRoute(
        RouteSettings? settings = null,
        bool? requestFocus = null,
        TraversalEdgeBehavior? traversalEdgeBehavior = null,
        TraversalEdgeBehavior? directionalTraversalEdgeBehavior = null,
        bool fullscreenDialog = false,
        bool allowSnapshotting = true,
        bool barrierDismissible = false
    )
        : base(
            settings: settings,
            requestFocus: requestFocus,
            traversalEdgeBehavior: traversalEdgeBehavior,
            directionalTraversalEdgeBehavior: directionalTraversalEdgeBehavior
        )
    {
        __field_fullscreenDialog = fullscreenDialog;
        __field_allowSnapshotting = allowSnapshotting;
        _barrierDismissible = barrierDismissible;
    }

    public override bool opaque => true;
    public override bool barrierDismissible => _barrierDismissible;

    public override bool canTransitionTo(dynamic nextRoute) => nextRoute is IPageRoute;

    public override bool canTransitionFrom(dynamic previousRoute) => previousRoute is IPageRoute;

    public override bool popGestureEnabled
    {
        get { return !fullscreenDialog && base.popGestureEnabled; }
    }
}

public static partial class PagesLibrary
{
    internal static Widget _defaultTransitionsBuilder(
        BuildContext context,
        Animation<double> animation,
        Animation<double> secondaryAnimation,
        Widget child
    )
    {
        return child;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class PageRouteBuilder<T> : PageRoute<T>
{
    public virtual Func<BuildContext, Animation<double>, Animation<double>, Widget> pageBuilder
    {
        get;
        private set;
    } = default!;
    public virtual Func<
        BuildContext,
        Animation<double>,
        Animation<double>,
        Widget,
        Widget
    > transitionsBuilder { get; private set; } = default!;
    private Duration __field_transitionDuration = default!;
    public override Duration transitionDuration
    {
        get => __field_transitionDuration;
    }
    private Duration __field_reverseTransitionDuration = default!;
    public override Duration reverseTransitionDuration
    {
        get => __field_reverseTransitionDuration;
    }
    private bool __field_opaque = default!;
    public override bool opaque
    {
        get => __field_opaque;
    }
    private bool __field_barrierDismissible = default!;
    public override bool barrierDismissible
    {
        get => __field_barrierDismissible;
    }
    private Color? __field_barrierColor = default!;
    public override Color? barrierColor
    {
        get => __field_barrierColor;
    }
    private string? __field_barrierLabel = default!;
    public override string? barrierLabel
    {
        get => __field_barrierLabel;
    }
    private bool __field_maintainState = default!;
    public override bool maintainState
    {
        get => __field_maintainState;
    }

    public PageRouteBuilder(
        RouteSettings? settings = null,
        bool? requestFocus = null,
        Func<BuildContext, Animation<double>, Animation<double>, Widget> pageBuilder = default!,
        Func<
            BuildContext,
            Animation<double>,
            Animation<double>,
            Widget,
            Widget
        > transitionsBuilder = default!,
        Duration? transitionDuration = null,
        Duration? reverseTransitionDuration = null,
        bool opaque = true,
        bool barrierDismissible = false,
        Color? barrierColor = null,
        string? barrierLabel = null,
        bool maintainState = true,
        bool fullscreenDialog = false,
        bool allowSnapshotting = true
    )
        : base(
            settings: settings,
            requestFocus: requestFocus,
            fullscreenDialog: fullscreenDialog,
            allowSnapshotting: allowSnapshotting
        )
    {
        Func<
            BuildContext,
            Animation<double>,
            Animation<double>,
            Widget,
            Widget
        > __transitionsBuilder = transitionsBuilder ?? PagesLibrary._defaultTransitionsBuilder;
        Duration __transitionDuration = transitionDuration ?? Duration.Create(milliseconds: 300);
        Duration __reverseTransitionDuration =
            reverseTransitionDuration ?? Duration.Create(milliseconds: 300);
        this.pageBuilder = pageBuilder;
        this.transitionsBuilder = __transitionsBuilder;
        __field_transitionDuration = __transitionDuration;
        __field_reverseTransitionDuration = __reverseTransitionDuration;
        __field_opaque = opaque;
        __field_barrierDismissible = barrierDismissible;
        __field_barrierColor = barrierColor;
        __field_barrierLabel = barrierLabel;
        __field_maintainState = maintainState;
    }

    public override Widget buildPage(
        BuildContext context,
        Animation<double> animation,
        Animation<double> secondaryAnimation
    )
    {
        return pageBuilder(context, animation, secondaryAnimation);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget buildTransitions(
        BuildContext context,
        Animation<double> animation,
        Animation<double> secondaryAnimation,
        Widget child
    )
    {
        return transitionsBuilder(context, animation, secondaryAnimation, child);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
