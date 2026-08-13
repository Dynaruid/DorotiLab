// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/drawer.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public enum DrawerAlignment
{
    start,
    end
}

public static partial class DrawerLibrary
{
    internal static double _kWidth = 304.0;
}

public static partial class DrawerLibrary
{
    internal static double _kEdgeDragWidth = 20.0;
}

public static partial class DrawerLibrary
{
    internal static double _kMinFlingVelocity = 365.0;
}

public static partial class DrawerLibrary
{
    internal static Duration _kBaseSettleDuration = Duration.Create(milliseconds: 246L);
}

public class Drawer : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual double? width { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? child { get; private set; }
    public virtual string? semanticLabel { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }

    public Drawer(global::Doroti.Generated.Framework.Foundation.Key? key = null, Color? backgroundColor = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, double? width = null, global::Doroti.Generated.Framework.Widgets.Widget? child = null, string? semanticLabel = null, Clip? clipBehavior = null) : base(key: key)
    {
        this.backgroundColor = backgroundColor;
        this.elevation = elevation;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.shape = shape;
        this.width = width;
        this.child = child;
        this.semanticLabel = semanticLabel;
        this.clipBehavior = clipBehavior;
        System.Diagnostics.Debug.Assert(((elevation is null) || (elevation >= 0.0)));
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        DrawerThemeData drawerTheme__9461 = DrawerTheme.of(context);
        string? label__9518 = (global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform switch { global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS => this.semanticLabel, global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS => this.semanticLabel, global::Doroti.Generated.Framework.Foundation.TargetPlatform.android or global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia or global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux => (this.semanticLabel ?? MaterialLocalizations.of(context).drawerLabel), global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows => (this.semanticLabel ?? MaterialLocalizations.of(context).drawerLabel), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        bool useMaterial3__9838 = Theme.of(context).useMaterial3;
        var isDrawerStart__9895 = (!object.Equals(DrawerController.maybeOf(context)?.alignment, DrawerAlignment.end));
        DrawerThemeData defaults__10006 = (useMaterial3__9838 ? new _DrawerDefaultsM3__drawer(context) : new _DrawerDefaultsM2__drawer(context));
        global::Doroti.Generated.Framework.Painting.ShapeBorder? effectiveShape__10128 = (this.shape ?? ((isDrawerStart__9895 ? ((drawerTheme__9461.shape ?? defaults__10006.shape)) : ((drawerTheme__9461.endShape ?? defaults__10006.endShape)))));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Semantics(scopesRoute: true, namesRoute: true, explicitChildNodes: true, label: label__9518, child: new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateExpand(width: ((this.width ?? drawerTheme__9461.width) ?? DrawerLibrary._kWidth)), child: new Material(color: ((this.backgroundColor ?? drawerTheme__9461.backgroundColor) ?? defaults__10006.backgroundColor), elevation: ((this.elevation ?? drawerTheme__9461.elevation) ?? DartRuntimePrimitives.RequireValue(defaults__10006.elevation)), shadowColor: ((this.shadowColor ?? drawerTheme__9461.shadowColor) ?? defaults__10006.shadowColor), surfaceTintColor: ((this.surfaceTintColor ?? drawerTheme__9461.surfaceTintColor) ?? defaults__10006.surfaceTintColor), shape: effectiveShape__10128, clipBehavior: ((effectiveShape__10128 is not null) ? (((this.clipBehavior ?? drawerTheme__9461.clipBehavior) ?? DartRuntimePrimitives.RequireValue(defaults__10006.clipBehavior))) : Clip.none), child: this.child))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public delegate void DrawerCallback(bool isOpened);

internal class _DrawerControllerScope__drawer : global::Doroti.Generated.Framework.Widgets.InheritedWidget
{
    public virtual DrawerController controller { get; private set; } = default!;

    internal _DrawerControllerScope__drawer(DrawerController controller, global::Doroti.Generated.Framework.Widgets.Widget child) : base(child: child)
    {
        this.controller = controller;
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget)
    {
        var __old = (_DrawerControllerScope__drawer)(object)oldWidget;
        return (!object.Equals(this.controller, ((_DrawerControllerScope__drawer)__old).controller));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DrawerController : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual DrawerAlignment alignment { get; private set; } = default!;
    public virtual global::System.Action<bool>? drawerCallback { get; private set; }
    public virtual bool drawerBarrierDismissible { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual Color? scrimColor { get; private set; }
    public virtual bool enableOpenDragGesture { get; private set; } = default!;
    public virtual double? edgeDragWidth { get; private set; }
    public virtual bool isDrawerOpen { get; private set; } = default!;

    public DrawerController(global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>? key = null, global::Doroti.Generated.Framework.Widgets.Widget child = default!, DrawerAlignment alignment = default!, bool isDrawerOpen = false, global::System.Action<bool>? drawerCallback = null, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, Color? scrimColor = null, double? edgeDragWidth = null, bool enableOpenDragGesture = true, bool drawerBarrierDismissible = true) : base(key: key)
    {
        this.child = child;
        this.alignment = alignment;
        this.isDrawerOpen = isDrawerOpen;
        this.drawerCallback = drawerCallback;
        this.dragStartBehavior = dragStartBehavior;
        this.scrimColor = scrimColor;
        this.edgeDragWidth = edgeDragWidth;
        this.enableOpenDragGesture = enableOpenDragGesture;
        this.drawerBarrierDismissible = drawerBarrierDismissible;
    }

    public static DrawerController? maybeOf(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return context.dependOnInheritedWidgetOfExactType<_DrawerControllerScope__drawer>()?.controller;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DrawerController of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DrawerController? controller__16921 = ((DrawerController?)(object?)DrawerController.maybeOf(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((controller__16921 is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Generated.Framework.Foundation.FlutterError.Create("DrawerController.of() was called with a context that does not " + "contain a DrawerController widget.\n" + "No DrawerController widget ancestor could be found starting from " + "the context that was passed to DrawerController.of(). This can " + "happen because you are using a widget that looks for a DrawerController " + "ancestor, but no such ancestor exists.\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
            });
        return controller__16921!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new DrawerControllerState());
}

public class DrawerControllerState : global::Doroti.Generated.Framework.Widgets.State<DrawerController>, global::Doroti.Generated.Framework.Widgets.SingleTickerProviderStateMixin<DrawerController>
{
    internal virtual global::Doroti.Generated.Framework.Widgets.LocalHistoryEntry? _historyEntry { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Widgets.FocusScopeNode _focusScopeNode { get; private set; } = new global::Doroti.Generated.Framework.Widgets.FocusScopeNode();
    internal virtual global::Doroti.Generated.Framework.Animation.AnimationController _controller { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> _drawerKey { get; private set; } = global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create();
    internal virtual bool _previouslyOpened { get; set; } = false;
    internal virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> _gestureDetectorKey { get; private set; } = global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create();
    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _controller = new global::Doroti.Generated.Framework.Animation.AnimationController(value: (((DrawerController)this.widget).isDrawerOpen ? 1.0 : 0.0), duration: DrawerLibrary._kBaseSettleDuration, vsync: this);
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Animation.AnimationController>)(() =>
{            var __cascade = this._controller;
            __cascade.addListener(() => this._animationChanged());
            __cascade.addStatusListener((AnimationStatusListener)this._animationStatusChanged);
            return __cascade;        }))());
    }

    public override void dispose()
    {
        this._historyEntry?.remove();
        this._controller.dispose();
        this._focusScopeNode.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if (((this._ticker is null) || !this._ticker!.isActive))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), this._ticker!.describeForError("The offending ticker was") }));
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override void didUpdateWidget(DrawerController oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (global::Doroti.Generated.Framework.Animation.AnimationStatusMembers.isAnimating(((global::Doroti.Generated.Framework.Animation.AnimationController)this._controller).status))
        {
            return;
        }
        if ((((DrawerController)this.widget).isDrawerOpen != ((DrawerController)oldWidget).isDrawerOpen))
        {
            this._controller.value = (((DrawerController)this.widget).isDrawerOpen ? 1.0 : 0.0);
        }
    }

    internal virtual void _animationChanged()
    {
        setState(((global::System.Action)(() => {
})));
    }

    internal virtual void _ensureHistoryEntry()
    {
        if ((this._historyEntry is null))
        {
            dynamic route__19067 = global::Doroti.Generated.Framework.Widgets.ModalRoute<object>.of<object>(this.context);
            if ((route__19067 is not null))
            {
                _historyEntry = new global::Doroti.Generated.Framework.Widgets.LocalHistoryEntry(onRemove: () => this._handleHistoryEntryRemoved(), impliesAppBarDismissal: false);
                ((dynamic)route__19067).addLocalHistoryEntry(this._historyEntry!);
                FocusScope.of(this.context).setFirstFocus(this._focusScopeNode);
            }
        }
    }

    internal virtual void _animationStatusChanged(global::Doroti.Generated.Framework.Animation.AnimationStatus status)
    {
        switch (status)
        {
            case global::Doroti.Generated.Framework.Animation.AnimationStatus.forward:
                {
                    _ensureHistoryEntry();
                    break;
                }
            case global::Doroti.Generated.Framework.Animation.AnimationStatus.reverse:
                {
                    this._historyEntry?.remove();
                    _historyEntry = null;
                    break;
                }
            case global::Doroti.Generated.Framework.Animation.AnimationStatus.dismissed:
            case global::Doroti.Generated.Framework.Animation.AnimationStatus.completed:
                {
                    break;
                }
        }
    }

    internal virtual void _handleHistoryEntryRemoved()
    {
        _historyEntry = null;
        close();
    }

    internal virtual void _handleDragDown(global::Doroti.Generated.Framework.Gestures.DragDownDetails details)
    {
        this._controller.stop();
        _ensureHistoryEntry();
    }

    internal virtual void _handleDragCancel()
    {
        if ((this._controller.isDismissed || ((global::Doroti.Generated.Framework.Animation.AnimationController)this._controller).isAnimating))
        {
            return;
        }
        if ((((global::Doroti.Generated.Framework.Animation.AnimationController)this._controller).value < 0.5))
        {
            close();
        }
        else
        {
            open();
        }
    }

    internal virtual double _width
    {
        get
        {
            var box__20255 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)((global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>)this._drawerKey).currentContext?.findRenderObject())!;
            return (box__20255?.size.width ?? DrawerLibrary._kWidth);
            return default!;
        }
    }
    internal virtual long _directionFactor
    {
        get
        {
            return ((Directionality.of(this.context), ((DrawerController)this.widget).alignment) switch { (TextDirection.rtl, DrawerAlignment.start) => -1L, (TextDirection.rtl, DrawerAlignment.end) => 1L, (TextDirection.ltr, DrawerAlignment.start) => 1L, (TextDirection.ltr, DrawerAlignment.end) => -1L, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            return default!;
        }
    }
    internal virtual void _move(global::Doroti.Generated.Framework.Gestures.DragUpdateDetails details)
    {
        this._controller.value += ((DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Gestures.DragUpdateDetails)details).primaryDelta) / this._width) * this._directionFactor);
        bool opened__20921 = (((global::Doroti.Generated.Framework.Animation.AnimationController)this._controller).value > 0.5);
        if (((opened__20921 != this._previouslyOpened) && (((DrawerController)this.widget).drawerCallback is not null)))
        {
            ((DrawerController)this.widget).drawerCallback!(opened__20921);
        }
        _previouslyOpened = opened__20921;
    }

    internal virtual void _settle(global::Doroti.Generated.Framework.Gestures.DragEndDetails details)
    {
        if (this._controller.isDismissed)
        {
            return;
        }
        double xVelocity__21221 = ((global::Doroti.Generated.Framework.Gestures.DragEndDetails)details).velocity.pixelsPerSecond.dx;
        if ((xVelocity__21221.abs() >= Bottom_sheetLibrary._kMinFlingVelocity))
        {
            double visualVelocity__21338 = ((xVelocity__21221 / this._width) * this._directionFactor);
            this._controller.fling(velocity: visualVelocity__21338);
            ((DrawerController)this.widget).drawerCallback?.Invoke((visualVelocity__21338 > 0.0));
        }
        else
        {
            if ((((global::Doroti.Generated.Framework.Animation.AnimationController)this._controller).value < 0.5))
            {
                close();
            }
            else
            {
                open();
            }
        }
    }

    public virtual void open()
    {
        this._controller.fling();
        ((DrawerController)this.widget).drawerCallback?.Invoke(true);
    }

    public virtual void close()
    {
        this._controller.fling(velocity: -1.0);
        ((DrawerController)this.widget).drawerCallback?.Invoke(false);
    }

    internal virtual global::Doroti.Generated.Framework.Painting.AlignmentDirectional _drawerOuterAlignment => (((DrawerController)this.widget).alignment switch { DrawerAlignment.start => global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerStart, DrawerAlignment.end => global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerEnd, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    internal virtual global::Doroti.Generated.Framework.Painting.AlignmentDirectional _drawerInnerAlignment => (((DrawerController)this.widget).alignment switch { DrawerAlignment.start => global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerEnd, DrawerAlignment.end => global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerStart, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildDrawer(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        bool isDesktop__22468 = (Theme.of(context).platform switch { global::Doroti.Generated.Framework.Foundation.TargetPlatform.android or global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS => false, global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia => false, global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS or global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux => true, global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows => true, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        double dragAreaWidth__22716 = (((DrawerController)this.widget).edgeDragWidth ?? (DrawerLibrary._kEdgeDragWidth + ((((DrawerController)this.widget).alignment, Directionality.of(context)) switch { (DrawerAlignment.start, TextDirection.ltr) => MediaQuery.paddingOf(context).left, (DrawerAlignment.start, TextDirection.rtl) => MediaQuery.paddingOf(context).right, (DrawerAlignment.end, TextDirection.rtl) => MediaQuery.paddingOf(context).left, (DrawerAlignment.end, TextDirection.ltr) => MediaQuery.paddingOf(context).right, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })));
        if (this._controller.isDismissed)
        {
            if ((((DrawerController)this.widget).enableOpenDragGesture && !isDesktop__22468))
            {
                return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Align(alignment: this._drawerOuterAlignment, child: new global::Doroti.Generated.Framework.Widgets.GestureDetector(key: this._gestureDetectorKey, onHorizontalDragUpdate: (global::System.Action<global::Doroti.Generated.Framework.Gestures.DragUpdateDetails>)this._move, onHorizontalDragEnd: (global::System.Action<global::Doroti.Generated.Framework.Gestures.DragEndDetails>)this._settle, behavior: global::Doroti.Generated.Framework.Rendering.HitTestBehavior.translucent, excludeFromSemantics: true, dragStartBehavior: ((DrawerController)this.widget).dragStartBehavior, child: new global::Doroti.Generated.Framework.Widgets.LimitedBox(maxHeight: 0.0, child: new global::Doroti.Generated.Framework.Widgets.SizedBox(width: dragAreaWidth__22716, height: double.PositiveInfinity)))));
            }
            else
            {
                return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)global::Doroti.Generated.Framework.Widgets.SizedBox.CreateShrink());
            }
        }
        else
        {
            bool platformHasBackButton__23991 = (global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform switch { global::Doroti.Generated.Framework.Foundation.TargetPlatform.android => true, global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS or global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS or global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia or global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux => false, global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows => false, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            global::Doroti.Flutter.Ui.Color scrimColor__24285 = ((global::Doroti.Flutter.Ui.Color)(object?)((((DrawerController)this.widget).scrimColor ?? DrawerTheme.of(context).scrimColor) ?? Colors.black54));
            global::Doroti.Flutter.Ui.Color effectiveScrimColor__24401 = ((global::Doroti.Flutter.Ui.Color)(object?)scrimColor__24285.withValues(alpha: (scrimColor__24285.a * ((global::Doroti.Generated.Framework.Animation.AnimationController)this._controller).value)));
            global::Doroti.Generated.Framework.Widgets.Widget drawerScrim__24523 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.ColoredBox(color: effectiveScrimColor__24401, child: new global::Doroti.Generated.Framework.Widgets.LimitedBox(maxWidth: 0.0, maxHeight: 0.0, child: global::Doroti.Generated.Framework.Widgets.SizedBox.CreateExpand())));
            global::Doroti.Generated.Framework.Widgets.Widget child__24704 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _DrawerControllerScope__drawer(controller: this.widget, child: new global::Doroti.Generated.Framework.Widgets.RepaintBoundary(child: new global::Doroti.Generated.Framework.Widgets.Stack(children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.BlockSemantics(child: new global::Doroti.Generated.Framework.Widgets.ExcludeSemantics(excluding: platformHasBackButton__23991, child: new global::Doroti.Generated.Framework.Widgets.GestureDetector(onTap: ((global::System.Action)(((DrawerController)this.widget).drawerBarrierDismissible ? this.close : null)), child: new global::Doroti.Generated.Framework.Widgets.Semantics(label: MaterialLocalizations.of(context).modalBarrierDismissLabel, child: drawerScrim__24523))))), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Align(alignment: this._drawerOuterAlignment, child: new global::Doroti.Generated.Framework.Widgets.Align(alignment: this._drawerInnerAlignment, widthFactor: ((global::Doroti.Generated.Framework.Animation.AnimationController)this._controller).value, child: new global::Doroti.Generated.Framework.Widgets.RepaintBoundary(child: new global::Doroti.Generated.Framework.Widgets.FocusScope(key: this._drawerKey, node: this._focusScopeNode, child: ((DrawerController)this.widget).child))))) }))));
            if (isDesktop__22468)
            {
                return child__24704;
            }
            return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.GestureDetector(key: this._gestureDetectorKey, onHorizontalDragDown: (global::System.Action<global::Doroti.Generated.Framework.Gestures.DragDownDetails>)this._handleDragDown, onHorizontalDragUpdate: (global::System.Action<global::Doroti.Generated.Framework.Gestures.DragUpdateDetails>)this._move, onHorizontalDragEnd: (global::System.Action<global::Doroti.Generated.Framework.Gestures.DragEndDetails>)this._settle, onHorizontalDragCancel: () => this._handleDragCancel(), excludeFromSemantics: true, dragStartBehavior: ((DrawerController)this.widget).dragStartBehavior, child: child__24704));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        return ListTileTheme.merge(style: ListTileStyle.drawer, child: _buildDrawer(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._ticker is null))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this.GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
            });
        this._ticker = new global::Doroti.Generated.Framework.Scheduler.Ticker((global::System.Action<Duration>)onTick, debugLabel: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
        _updateTickerModeNotifier();
        _updateTicker();
        return this._ticker!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTicker();
    }

    public virtual void _updateTicker()
    {
        TickerModeData values__15157 = this._tickerModeNotifier!.value;
        if ((this._ticker is not null))
        {
            this._ticker!.muted = !((TickerModeData)values__15157).enabled;
            this._ticker!.forceFrames = ((TickerModeData)values__15157).forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__15400 = ((global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__15400, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        newNotifier__15400.addListener(() => this._updateTicker());
        this._tickerModeNotifier = newNotifier__15400;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription__15805 = ((this._ticker?.isActive, this._ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) });
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Scheduler.Ticker>("ticker", this._ticker, description: tickerDescription__15805, showSeparator: false, defaultValue: default));
    }

}

internal class _DrawerDefaultsM2__drawer : DrawerThemeData
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;

    internal _DrawerDefaultsM2__drawer(global::Doroti.Generated.Framework.Widgets.BuildContext context) : base(elevation: 16.0, clipBehavior: Clip.hardEdge)
    {
        this.context = context;
    }

    public virtual global::Doroti.Flutter.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Color>(Theme.of(this.context).shadowColor);
}

internal class _DrawerDefaultsM3__drawer : DrawerThemeData
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late_direction_initialized;
    private global::Doroti.Flutter.Ui.TextDirection __late_direction = default!;
    public virtual global::Doroti.Flutter.Ui.TextDirection direction
    {
        get
        {
            if (!__late_direction_initialized)
            {
                __late_direction = Directionality.of(this.context);
                __late_direction_initialized = true;
            }
            return __late_direction;
        }
    }

    internal _DrawerDefaultsM3__drawer(global::Doroti.Generated.Framework.Widgets.BuildContext context) : base(elevation: 1.0, clipBehavior: Clip.hardEdge)
    {
        this.context = context;
    }

    public virtual global::Doroti.Flutter.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Color>(Theme.of(this.context).colorScheme.surfaceContainerLow);
    public virtual global::Doroti.Flutter.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Color>(Colors.transparent);
    public virtual global::Doroti.Flutter.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Color>(Colors.transparent);
    public override global::Doroti.Generated.Framework.Painting.ShapeBorder? shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.ShapeBorder>(new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadiusDirectional.CreateHorizontal(end: global::Doroti.Flutter.Ui.Radius.circular(16.0)).resolve(this.direction)));
    public override global::Doroti.Generated.Framework.Painting.ShapeBorder? endShape => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.ShapeBorder>(new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadiusDirectional.CreateHorizontal(start: global::Doroti.Flutter.Ui.Radius.circular(16.0)).resolve(this.direction)));
}
