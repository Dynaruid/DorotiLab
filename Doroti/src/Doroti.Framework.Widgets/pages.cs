// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/pages.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Widgets;

public abstract class PageRoute<T> : ModalRoute<T>
{
    private bool __field_fullscreenDialog = default!;
    public override bool fullscreenDialog { get => __field_fullscreenDialog; }
    private bool __field_allowSnapshotting = default!;
    public override bool allowSnapshotting { get => __field_allowSnapshotting; }
    internal virtual bool _barrierDismissible { get; private set; } = default!;

    protected PageRoute(RouteSettings? settings = null, bool? requestFocus = null, TraversalEdgeBehavior? traversalEdgeBehavior = null, TraversalEdgeBehavior? directionalTraversalEdgeBehavior = null, bool fullscreenDialog = false, bool allowSnapshotting = true, bool barrierDismissible = false) : base(settings: settings, requestFocus: requestFocus, traversalEdgeBehavior: traversalEdgeBehavior, directionalTraversalEdgeBehavior: directionalTraversalEdgeBehavior)
    {
        this.__field_fullscreenDialog = fullscreenDialog;
        this.__field_allowSnapshotting = allowSnapshotting;
        this._barrierDismissible = barrierDismissible;
    }

    public override bool opaque => true;
    public override bool barrierDismissible => this._barrierDismissible;
    public override bool canTransitionTo(dynamic nextRoute) => (nextRoute is PageRoute<T>);
    public override bool canTransitionFrom(dynamic previousRoute) => (previousRoute is PageRoute<T>);
    public override bool popGestureEnabled
    {
        get
        {
            return (!this.fullscreenDialog && base.popGestureEnabled);
            return default!;
        }
    }
}

public static partial class PagesLibrary
{
    internal static Widget _defaultTransitionsBuilder(BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, Widget child)
    {
        return child;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class PageRouteBuilder<T> : PageRoute<T>
{
    public virtual global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, Widget> pageBuilder { get; private set; } = default!;
    public virtual global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, Widget, Widget> transitionsBuilder { get; private set; } = default!;
    private Duration __field_transitionDuration = default!;
    public override Duration transitionDuration { get => __field_transitionDuration; }
    private Duration __field_reverseTransitionDuration = default!;
    public override Duration reverseTransitionDuration { get => __field_reverseTransitionDuration; }
    private bool __field_opaque = default!;
    public override bool opaque { get => __field_opaque; }
    private bool __field_barrierDismissible = default!;
    public override bool barrierDismissible { get => __field_barrierDismissible; }
    private Color? __field_barrierColor = default!;
    public override Color? barrierColor { get => __field_barrierColor; }
    private string? __field_barrierLabel = default!;
    public override string? barrierLabel { get => __field_barrierLabel; }
    private bool __field_maintainState = default!;
    public override bool maintainState { get => __field_maintainState; }

    public PageRouteBuilder(RouteSettings? settings = null, bool? requestFocus = null, global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, Widget> pageBuilder = default!, global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, Widget, Widget> transitionsBuilder = default!, Duration? transitionDuration = null, Duration? reverseTransitionDuration = null, bool opaque = true, bool barrierDismissible = false, Color? barrierColor = null, string? barrierLabel = null, bool maintainState = true, bool fullscreenDialog = false, bool allowSnapshotting = true) : base(settings: settings, requestFocus: requestFocus, fullscreenDialog: fullscreenDialog, allowSnapshotting: allowSnapshotting)
    {
        global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, Widget, Widget> __transitionsBuilder = transitionsBuilder ?? PagesLibrary._defaultTransitionsBuilder;
        Duration __transitionDuration = transitionDuration ?? Duration.Create(milliseconds: 300);
        Duration __reverseTransitionDuration = reverseTransitionDuration ?? Duration.Create(milliseconds: 300);
        this.pageBuilder = pageBuilder;
        this.transitionsBuilder = __transitionsBuilder;
        this.__field_transitionDuration = __transitionDuration;
        this.__field_reverseTransitionDuration = __reverseTransitionDuration;
        this.__field_opaque = opaque;
        this.__field_barrierDismissible = barrierDismissible;
        this.__field_barrierColor = barrierColor;
        this.__field_barrierLabel = barrierLabel;
        this.__field_maintainState = maintainState;
    }

    public override Widget buildPage(BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation)
    {
        return this.pageBuilder(context, animation, secondaryAnimation);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget buildTransitions(BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, Widget child)
    {
        return this.transitionsBuilder(context, animation, secondaryAnimation, child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
