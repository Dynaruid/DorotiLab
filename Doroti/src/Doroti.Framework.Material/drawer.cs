// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/drawer.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Material;

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

public class Drawer : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual double? width { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }
    public virtual string? semanticLabel { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }

    public Drawer(global::Doroti.Framework.Foundation.Key? key = null, Color? backgroundColor = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, double? width = null, global::Doroti.Framework.Widgets.Widget? child = null, string? semanticLabel = null, Clip? clipBehavior = null) : base(key: key)
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

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        DrawerThemeData drawerTheme = DrawerTheme.of(context);
        string? labelLocal = (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform switch { global::Doroti.Framework.Foundation.TargetPlatform.iOS => this.semanticLabel, global::Doroti.Framework.Foundation.TargetPlatform.macOS => this.semanticLabel, global::Doroti.Framework.Foundation.TargetPlatform.android or global::Doroti.Framework.Foundation.TargetPlatform.fuchsia or global::Doroti.Framework.Foundation.TargetPlatform.linux => (this.semanticLabel ?? MaterialLocalizations.of(context).drawerLabel), global::Doroti.Framework.Foundation.TargetPlatform.windows => (this.semanticLabel ?? MaterialLocalizations.of(context).drawerLabel), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        bool useMaterial3Local = Theme.of(context).useMaterial3;
        var isDrawerStart = (!object.Equals(DrawerController.maybeOf(context)?.alignment, DrawerAlignment.end));
        DrawerThemeData defaults = (useMaterial3Local ? new _DrawerDefaultsM3__drawer(context) : new _DrawerDefaultsM2__drawer(context));
        global::Doroti.Framework.Painting.ShapeBorder? effectiveShape = (this.shape ?? ((isDrawerStart ? ((drawerTheme.shape ?? defaults.shape)) : ((drawerTheme.endShape ?? defaults.endShape)))));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(scopesRoute: true, namesRoute: true, explicitChildNodes: true, label: labelLocal, child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: global::Doroti.Framework.Rendering.BoxConstraints.CreateExpand(width: ((this.width ?? drawerTheme.width) ?? DrawerLibrary._kWidth)), child: new Material(color: ((this.backgroundColor ?? drawerTheme.backgroundColor) ?? defaults.backgroundColor), elevation: ((this.elevation ?? drawerTheme.elevation) ?? DartRuntimePrimitives.RequireValue(defaults.elevation)), shadowColor: ((this.shadowColor ?? drawerTheme.shadowColor) ?? defaults.shadowColor), surfaceTintColor: ((this.surfaceTintColor ?? drawerTheme.surfaceTintColor) ?? defaults.surfaceTintColor), shape: effectiveShape, clipBehavior: ((effectiveShape is not null) ? (((this.clipBehavior ?? drawerTheme.clipBehavior) ?? DartRuntimePrimitives.RequireValue(defaults.clipBehavior))) : Clip.none), child: this.child))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public delegate void DrawerCallback(bool isOpened);

internal class _DrawerControllerScope__drawer : global::Doroti.Framework.Widgets.InheritedWidget
{
    public virtual DrawerController controller { get; private set; } = default!;

    internal _DrawerControllerScope__drawer(DrawerController controller, global::Doroti.Framework.Widgets.Widget child) : base(child: child)
    {
        this.controller = controller;
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget)
    {
        var __old = (_DrawerControllerScope__drawer)(object)oldWidget;
        return (!object.Equals(this.controller, ((_DrawerControllerScope__drawer)__old).controller));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DrawerController : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual DrawerAlignment alignment { get; private set; } = default!;
    public virtual global::System.Action<bool>? drawerCallback { get; private set; }
    public virtual bool drawerBarrierDismissible { get; private set; } = default!;
    public virtual global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual Color? scrimColor { get; private set; }
    public virtual bool enableOpenDragGesture { get; private set; } = default!;
    public virtual double? edgeDragWidth { get; private set; }
    public virtual bool isDrawerOpen { get; private set; } = default!;

    public DrawerController(global::Doroti.Framework.Widgets.GlobalKey<IState>? key = null, global::Doroti.Framework.Widgets.Widget child = default!, DrawerAlignment alignment = default!, bool isDrawerOpen = false, global::System.Action<bool>? drawerCallback = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.start, Color? scrimColor = null, double? edgeDragWidth = null, bool enableOpenDragGesture = true, bool drawerBarrierDismissible = true) : base(key: key)
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

    public static DrawerController? maybeOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return context.dependOnInheritedWidgetOfExactType<_DrawerControllerScope__drawer>()?.controller;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DrawerController of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DrawerController? controller = ((DrawerController?)(object?)DrawerController.maybeOf(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((controller is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("DrawerController.of() was called with a context that does not " + "contain a DrawerController widget.\n" + "No DrawerController widget ancestor could be found starting from " + "the context that was passed to DrawerController.of(). This can " + "happen because you are using a widget that looks for a DrawerController " + "ancestor, but no such ancestor exists.\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
            });
        return controller!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new DrawerControllerState());
}

public class DrawerControllerState : global::Doroti.Framework.Widgets.State<DrawerController>, global::Doroti.Framework.Widgets.SingleTickerProviderStateMixin<DrawerController>
{
    internal virtual global::Doroti.Framework.Widgets.LocalHistoryEntry? _historyEntry { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.FocusScopeNode _focusScopeNode { get; private set; } = new global::Doroti.Framework.Widgets.FocusScopeNode();
    internal virtual global::Doroti.Framework.Animation.AnimationController _controller { get; set; } = default!;
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _drawerKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create();
    internal virtual bool _previouslyOpened { get; set; } = false;
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _gestureDetectorKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create();
    public virtual global::Doroti.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _controller = new global::Doroti.Framework.Animation.AnimationController(value: (((DrawerController)this.widget).isDrawerOpen ? 1.0 : 0.0), duration: DrawerLibrary._kBaseSettleDuration, vsync: this);
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{
    var __cascade = this._controller;
    __cascade.addListener(() => this._animationChanged());
    __cascade.addStatusListener((AnimationStatusListener)this._animationStatusChanged);
    return __cascade;
}))());
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
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), this._ticker!.describeForError("The offending ticker was") }));
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override void didUpdateWidget(DrawerController oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (global::Doroti.Framework.Animation.AnimationStatusMembers.isAnimating(((global::Doroti.Framework.Animation.AnimationController)this._controller).status))
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
        setState(((global::System.Action)(() =>
        {
        })));
    }

    internal virtual void _ensureHistoryEntry()
    {
        if ((this._historyEntry is null))
        {
            dynamic route = global::Doroti.Framework.Widgets.ModalRoute<object>.of<object>(this.context);
            if ((route is not null))
            {
                _historyEntry = new global::Doroti.Framework.Widgets.LocalHistoryEntry(onRemove: () => this._handleHistoryEntryRemoved(), impliesAppBarDismissal: false);
                ((dynamic)route).addLocalHistoryEntry(this._historyEntry!);
                FocusScope.of(this.context).setFirstFocus(this._focusScopeNode);
            }
        }
    }

    internal virtual void _animationStatusChanged(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        switch (status)
        {
            case global::Doroti.Framework.Animation.AnimationStatus.forward:
                {
                    _ensureHistoryEntry();
                    break;
                }
            case global::Doroti.Framework.Animation.AnimationStatus.reverse:
                {
                    this._historyEntry?.remove();
                    _historyEntry = null;
                    break;
                }
            case global::Doroti.Framework.Animation.AnimationStatus.dismissed:
            case global::Doroti.Framework.Animation.AnimationStatus.completed:
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

    internal virtual void _handleDragDown(global::Doroti.Framework.Gestures.DragDownDetails details)
    {
        this._controller.stop();
        _ensureHistoryEntry();
    }

    internal virtual void _handleDragCancel()
    {
        if ((this._controller.isDismissed || ((global::Doroti.Framework.Animation.AnimationController)this._controller).isAnimating))
        {
            return;
        }
        if ((((global::Doroti.Framework.Animation.AnimationController)this._controller).value < 0.5))
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
            var box = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((global::Doroti.Framework.Widgets.GlobalKey<IState>)this._drawerKey).currentContext?.findRenderObject())!;
            return (box?.size.width ?? DrawerLibrary._kWidth);
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
    internal virtual void _move(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        this._controller.value += ((DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Gestures.DragUpdateDetails)details).primaryDelta) / this._width) * this._directionFactor);
        bool opened = (((global::Doroti.Framework.Animation.AnimationController)this._controller).value > 0.5);
        if (((opened != this._previouslyOpened) && (((DrawerController)this.widget).drawerCallback is not null)))
        {
            ((DrawerController)this.widget).drawerCallback!(opened);
        }
        _previouslyOpened = opened;
    }

    internal virtual void _settle(global::Doroti.Framework.Gestures.DragEndDetails details)
    {
        if (this._controller.isDismissed)
        {
            return;
        }
        double xVelocity = ((global::Doroti.Framework.Gestures.DragEndDetails)details).velocity.pixelsPerSecond.dx;
        if ((xVelocity.abs() >= Bottom_sheetLibrary._kMinFlingVelocity))
        {
            double visualVelocity = ((xVelocity / this._width) * this._directionFactor);
            this._controller.fling(velocity: visualVelocity);
            ((DrawerController)this.widget).drawerCallback?.Invoke((visualVelocity > 0.0));
        }
        else
        {
            if ((((global::Doroti.Framework.Animation.AnimationController)this._controller).value < 0.5))
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

    internal virtual global::Doroti.Framework.Painting.AlignmentDirectional _drawerOuterAlignment => (((DrawerController)this.widget).alignment switch { DrawerAlignment.start => global::Doroti.Framework.Painting.AlignmentDirectional.centerStart, DrawerAlignment.end => global::Doroti.Framework.Painting.AlignmentDirectional.centerEnd, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    internal virtual global::Doroti.Framework.Painting.AlignmentDirectional _drawerInnerAlignment => (((DrawerController)this.widget).alignment switch { DrawerAlignment.start => global::Doroti.Framework.Painting.AlignmentDirectional.centerEnd, DrawerAlignment.end => global::Doroti.Framework.Painting.AlignmentDirectional.centerStart, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    internal virtual global::Doroti.Framework.Widgets.Widget _buildDrawer(global::Doroti.Framework.Widgets.BuildContext context)
    {
        bool isDesktop = (Theme.of(context).platform switch { global::Doroti.Framework.Foundation.TargetPlatform.android or global::Doroti.Framework.Foundation.TargetPlatform.iOS => false, global::Doroti.Framework.Foundation.TargetPlatform.fuchsia => false, global::Doroti.Framework.Foundation.TargetPlatform.macOS or global::Doroti.Framework.Foundation.TargetPlatform.linux => true, global::Doroti.Framework.Foundation.TargetPlatform.windows => true, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        double dragAreaWidth = (((DrawerController)this.widget).edgeDragWidth ?? (DrawerLibrary._kEdgeDragWidth + ((((DrawerController)this.widget).alignment, Directionality.of(context)) switch { (DrawerAlignment.start, TextDirection.ltr) => MediaQuery.paddingOf(context).left, (DrawerAlignment.start, TextDirection.rtl) => MediaQuery.paddingOf(context).right, (DrawerAlignment.end, TextDirection.rtl) => MediaQuery.paddingOf(context).left, (DrawerAlignment.end, TextDirection.ltr) => MediaQuery.paddingOf(context).right, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })));
        if (this._controller.isDismissed)
        {
            if ((((DrawerController)this.widget).enableOpenDragGesture && !isDesktop))
            {
                return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Align(alignment: this._drawerOuterAlignment, child: new global::Doroti.Framework.Widgets.GestureDetector(key: this._gestureDetectorKey, onHorizontalDragUpdate: (global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>)this._move, onHorizontalDragEnd: (global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>)this._settle, behavior: global::Doroti.Framework.Rendering.HitTestBehavior.translucent, excludeFromSemantics: true, dragStartBehavior: ((DrawerController)this.widget).dragStartBehavior, child: new global::Doroti.Framework.Widgets.LimitedBox(maxHeight: 0.0, child: new global::Doroti.Framework.Widgets.SizedBox(width: dragAreaWidth, height: double.PositiveInfinity)))));
            }
            else
            {
                return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.SizedBox.CreateShrink());
            }
        }
        else
        {
            bool platformHasBackButton = (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform switch { global::Doroti.Framework.Foundation.TargetPlatform.android => true, global::Doroti.Framework.Foundation.TargetPlatform.iOS or global::Doroti.Framework.Foundation.TargetPlatform.macOS or global::Doroti.Framework.Foundation.TargetPlatform.fuchsia or global::Doroti.Framework.Foundation.TargetPlatform.linux => false, global::Doroti.Framework.Foundation.TargetPlatform.windows => false, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            global::Doroti.Ui.Color scrimColorLocal = ((global::Doroti.Ui.Color)(object?)((((DrawerController)this.widget).scrimColor ?? DrawerTheme.of(context).scrimColor) ?? Colors.black54));
            global::Doroti.Ui.Color effectiveScrimColor = ((global::Doroti.Ui.Color)(object?)scrimColorLocal.withValues(alpha: (scrimColorLocal.a * ((global::Doroti.Framework.Animation.AnimationController)this._controller).value)));
            global::Doroti.Framework.Widgets.Widget drawerScrim = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.ColoredBox(color: effectiveScrimColor, child: new global::Doroti.Framework.Widgets.LimitedBox(maxWidth: 0.0, maxHeight: 0.0, child: global::Doroti.Framework.Widgets.SizedBox.CreateExpand())));
            global::Doroti.Framework.Widgets.Widget childLocal = ((global::Doroti.Framework.Widgets.Widget)(object?)new _DrawerControllerScope__drawer(controller: this.widget, child: new global::Doroti.Framework.Widgets.RepaintBoundary(child: new global::Doroti.Framework.Widgets.Stack(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.BlockSemantics(child: new global::Doroti.Framework.Widgets.ExcludeSemantics(excluding: platformHasBackButton, child: new global::Doroti.Framework.Widgets.GestureDetector(onTap: ((global::System.Action)(((DrawerController)this.widget).drawerBarrierDismissible ? this.close : null)), child: new global::Doroti.Framework.Widgets.Semantics(label: MaterialLocalizations.of(context).modalBarrierDismissLabel, child: drawerScrim))))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Align(alignment: this._drawerOuterAlignment, child: new global::Doroti.Framework.Widgets.Align(alignment: this._drawerInnerAlignment, widthFactor: ((global::Doroti.Framework.Animation.AnimationController)this._controller).value, child: new global::Doroti.Framework.Widgets.RepaintBoundary(child: new global::Doroti.Framework.Widgets.FocusScope(key: this._drawerKey, node: this._focusScopeNode, child: ((DrawerController)this.widget).child))))) }))));
            if (isDesktop)
            {
                return childLocal;
            }
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.GestureDetector(key: this._gestureDetectorKey, onHorizontalDragDown: (global::System.Action<global::Doroti.Framework.Gestures.DragDownDetails>)this._handleDragDown, onHorizontalDragUpdate: (global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>)this._move, onHorizontalDragEnd: (global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>)this._settle, onHorizontalDragCancel: () => this._handleDragCancel(), excludeFromSemantics: true, dragStartBehavior: ((DrawerController)this.widget).dragStartBehavior, child: childLocal));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        return ListTileTheme.merge(style: ListTileStyle.drawer, child: _buildDrawer(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._ticker is null))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this.GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new global::Doroti.Framework.Foundation.ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new global::Doroti.Framework.Foundation.ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
            });
        this._ticker = new global::Doroti.Framework.Scheduler.Ticker((global::System.Action<Duration>)onTick, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
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
        TickerModeData values = this._tickerModeNotifier!.value;
        if ((this._ticker is not null))
        {
            this._ticker!.muted = !((TickerModeData)values).enabled;
            this._ticker!.forceFrames = ((TickerModeData)values).forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        newNotifier.addListener(() => this._updateTicker());
        this._tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription = ((this._ticker?.isActive, this._ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) });
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Scheduler.Ticker>("ticker", this._ticker, description: tickerDescription, showSeparator: false, defaultValue: default));
    }

}

internal class _DrawerDefaultsM2__drawer : DrawerThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;

    internal _DrawerDefaultsM2__drawer(global::Doroti.Framework.Widgets.BuildContext context) : base(elevation: 16.0, clipBehavior: Clip.hardEdge)
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Theme.of(this.context).shadowColor);
}

internal class _DrawerDefaultsM3__drawer : DrawerThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late_direction_initialized;
    private global::Doroti.Ui.TextDirection __late_direction = default!;
    public virtual global::Doroti.Ui.TextDirection direction
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

    internal _DrawerDefaultsM3__drawer(global::Doroti.Framework.Widgets.BuildContext context) : base(elevation: 1.0, clipBehavior: Clip.hardEdge)
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Theme.of(this.context).colorScheme.surfaceContainerLow);
    public virtual global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public virtual global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public override global::Doroti.Framework.Painting.ShapeBorder? shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.ShapeBorder>(new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadiusDirectional.CreateHorizontal(end: global::Doroti.Ui.Radius.circular(16.0)).resolve(this.direction)));
    public override global::Doroti.Framework.Painting.ShapeBorder? endShape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.ShapeBorder>(new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadiusDirectional.CreateHorizontal(start: global::Doroti.Ui.Radius.circular(16.0)).resolve(this.direction)));
}
