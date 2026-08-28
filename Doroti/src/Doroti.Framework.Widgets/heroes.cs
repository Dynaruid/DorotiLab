// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/heroes.dart
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

public delegate global::Doroti.Framework.Animation.Tween<Rect?> CreateRectTween(Rect? begin, Rect? end);

public delegate Widget HeroPlaceholderBuilder(BuildContext context, Size heroSize, Widget child);

public delegate Widget HeroFlightShuttleBuilder(BuildContext flightContext, global::Doroti.Framework.Animation.Animation<double> animation, HeroFlightDirection flightDirection, BuildContext fromHeroContext, BuildContext toHeroContext);

internal delegate void _OnFlightEnded__heroes(_HeroFlight__heroes flight);

public enum HeroFlightDirection
{
    push,
    pop
}

public class Hero : StatefulWidget
{
    public virtual object tag { get; private set; } = default!;
    public virtual global::System.Func<Rect?, Rect?, global::Doroti.Framework.Animation.Tween<Rect?>>? createRectTween { get; private set; }
    public virtual Widget child { get; private set; } = default!;
    public virtual global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, HeroFlightDirection, BuildContext, BuildContext, Widget>? flightShuttleBuilder { get; private set; }
    public virtual global::System.Func<BuildContext, Size, Widget, Widget>? placeholderBuilder { get; private set; }
    public virtual bool transitionOnUserGestures { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Curve curve { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Curve? reverseCurve { get; private set; }

    public Hero(global::Doroti.Framework.Foundation.Key? key = null, object tag = default!, global::System.Func<Rect?, Rect?, global::Doroti.Framework.Animation.Tween<Rect?>>? createRectTween = null, global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, HeroFlightDirection, BuildContext, BuildContext, Widget>? flightShuttleBuilder = null, global::System.Func<BuildContext, Size, Widget, Widget>? placeholderBuilder = null, bool transitionOnUserGestures = false, global::Doroti.Framework.Animation.Curve curve = default!, global::Doroti.Framework.Animation.Curve? reverseCurve = null, Widget child = default!) : base(key: key)
    {
        global::Doroti.Framework.Animation.Curve __curve = curve ?? global::Doroti.Framework.Animation.Curves.fastOutSlowIn;
        this.tag = tag;
        this.createRectTween = createRectTween;
        this.flightShuttleBuilder = flightShuttleBuilder;
        this.placeholderBuilder = placeholderBuilder;
        this.transitionOnUserGestures = transitionOnUserGestures;
        this.curve = __curve;
        this.reverseCurve = reverseCurve;
        this.child = child;
    }

    internal static DartMap<object, _HeroState__heroes> _allHeroesFor(BuildContext context, bool isUserGestureTransition, NavigatorState navigator)
    {
        var result = new DartMap<object, _HeroState__heroes>();
        void inviteHero(StatefulElement hero, object tag)
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    if (result.ContainsKey(tag))
                    {
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("There are multiple heroes that share the same tag within a subtree."), new global::Doroti.Framework.Foundation.ErrorDescription("Within each subtree for which heroes are to be animated (i.e. a PageRoute subtree), " + "each Hero must have a unique non-null tag.\n" + $"In this case, multiple heroes had the following tag: {tag}"), new global::Doroti.Framework.Foundation.DiagnosticsProperty<StatefulElement>("Here is the subtree for one of the offending heroes", hero, linePrefix: "# ", style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.dense) }));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            var heroWidget = ((Hero?)(object?)hero.widget)!;
            var heroState = ((_HeroState__heroes?)(object?)((StatefulElement)hero).state)!;
            if ((!isUserGestureTransition || ((Hero)heroWidget).transitionOnUserGestures))
            {
                result[tag] = heroState;
            }
            else
            {
                heroState.endFlight();
            }
        }
        void visitor(Element element)
        {
            Widget widgetLocal = ((Element)element).widget;
            if ((widgetLocal is Hero))
            {
                Hero widget__13444__as13479 = (Hero)widgetLocal;
                var heroLocal = ((StatefulElement?)(object?)element)!;
                object tagLocal = ((Hero)((Hero)widget__13444__as13479)).tag;
                if ((object.Equals(Navigator.of(heroLocal), navigator)))
                {
                    inviteHero(heroLocal, tagLocal);
                }
                else
                {
                    dynamic heroRoute = ModalRoute<object>.of<object>(heroLocal);
                    if ((((heroRoute is not null) && (heroRoute is PageRoute<object>)) && ((bool)((dynamic)heroRoute).isCurrent)))
                    {
                        dynamic heroRoute__14091__as14159 = (dynamic)heroRoute;
                        inviteHero(heroLocal, tagLocal);
                    }
                }
            }
            else
            {
                if (((widgetLocal is HeroMode) && !((HeroMode)((HeroMode)widgetLocal)).enabled))
                {
                    HeroMode widget__13444__as14282 = (HeroMode)widgetLocal;
                    return;
                }
            }
            element.visitChildren((global::System.Action<Element>)visitor);
        }
        context.visitChildElements((global::System.Action<Element>)visitor);
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _HeroState__heroes());
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<object>("tag", this.tag));
    }

}

public class _HeroState__heroes : State<Hero>
{
    internal virtual GlobalKey<IState> _key { get; private set; } = GlobalKey<IState>.Create();
    internal virtual Size? _placeholderSize { get; set; } = default;
    internal virtual bool _shouldIncludeChild { get; set; } = true;

    public virtual void startFlight(bool shouldIncludedChildInPlaceholder = false)
    {
        _shouldIncludeChild = shouldIncludedChildInPlaceholder;
        DartRuntimePrimitives.Assert(() => this.mounted);
        var box = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)this.context.findRenderObject()!)!;
        DartRuntimePrimitives.Assert(() => ((global::Doroti.Framework.Rendering.RenderBox)box).hasSize);
        setState(((global::System.Action)(() =>
        {
            _placeholderSize = ((global::Doroti.Framework.Rendering.RenderBox)box).size;
        })));
    }

    public virtual void endFlight(bool keepPlaceholder = false)
    {
        if ((keepPlaceholder || (this._placeholderSize is null)))
        {
            return;
        }
        _placeholderSize = null;
        if (this.mounted)
        {
            setState(((global::System.Action)(() =>
            {
            })));
        }
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => (context.findAncestorWidgetOfExactType<Hero>() is null), () => (object?)"A Hero widget cannot be the descendant of another Hero widget.");
        var showPlaceholder = (this._placeholderSize is not null);
        if ((showPlaceholder && (((Hero)this.widget).placeholderBuilder is not null)))
        {
            return ((Hero)this.widget).placeholderBuilder!(context, DartRuntimePrimitives.RequireValue(this._placeholderSize), ((Hero)this.widget).child);
        }
        if ((showPlaceholder && !this._shouldIncludeChild))
        {
            return ((Widget)(object?)new SizedBox(width: DartRuntimePrimitives.RequireValue(this._placeholderSize).width, height: DartRuntimePrimitives.RequireValue(this._placeholderSize).height));
        }
        return ((Widget)(object?)new SizedBox(width: this._placeholderSize?.width, height: this._placeholderSize?.height, child: new Offstage(offstage: showPlaceholder, child: new TickerMode(enabled: !showPlaceholder, child: new KeyedSubtree(key: this._key, child: ((Hero)this.widget).child)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _HeroFlightManifest__heroes
{
    public virtual HeroFlightDirection type { get; private set; } = default!;
    public virtual OverlayState overlay { get; private set; } = default!;
    public virtual Size navigatorSize { get; private set; } = default!;
    public virtual dynamic fromRoute { get; private set; } = default!;
    public virtual dynamic toRoute { get; private set; } = default!;
    public virtual _HeroState__heroes fromHero { get; private set; } = default!;
    public virtual _HeroState__heroes toHero { get; private set; } = default!;
    public virtual global::System.Func<Rect?, Rect?, global::Doroti.Framework.Animation.Tween<Rect?>>? createRectTween { get; private set; }
    public virtual global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, HeroFlightDirection, BuildContext, BuildContext, Widget> shuttleBuilder { get; private set; } = default!;
    public virtual bool isUserGestureTransition { get; private set; } = default!;
    public virtual bool isDiverted { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation? _animation { get; set; } = default;
    private bool __late_fromHeroLocation_initialized;
    private global::Doroti.Ui.Rect __late_fromHeroLocation = default!;
    public virtual global::Doroti.Ui.Rect fromHeroLocation
    {
        get
        {
            if (!__late_fromHeroLocation_initialized)
            {
                __late_fromHeroLocation = ((global::Doroti.Ui.Rect)(object?)_HeroFlightManifest__heroes._boundingBoxFor(this.fromHero.context, ((BuildContext?)((dynamic)this.fromRoute).subtreeContext)));
                __late_fromHeroLocation_initialized = true;
            }
            return __late_fromHeroLocation;
        }
    }
    private bool __late_toHeroLocation_initialized;
    private global::Doroti.Ui.Rect __late_toHeroLocation = default!;
    public virtual global::Doroti.Ui.Rect toHeroLocation
    {
        get
        {
            if (!__late_toHeroLocation_initialized)
            {
                __late_toHeroLocation = ((global::Doroti.Ui.Rect)(object?)_HeroFlightManifest__heroes._boundingBoxFor(this.toHero.context, ((BuildContext?)((dynamic)this.toRoute).subtreeContext)));
                __late_toHeroLocation_initialized = true;
            }
            return __late_toHeroLocation;
        }
    }
    private bool __late_isValid_initialized;
    private bool __late_isValid = default!;
    public virtual bool isValid
    {
        get
        {
            if (!__late_isValid_initialized)
            {
                __late_isValid = (this.toHeroLocation.isFinite && ((this.isDiverted || this.fromHeroLocation.isFinite)));
                __late_isValid_initialized = true;
            }
            return __late_isValid;
        }
    }

    internal _HeroFlightManifest__heroes(HeroFlightDirection type, OverlayState overlay, Size navigatorSize, dynamic fromRoute, dynamic toRoute, _HeroState__heroes fromHero, _HeroState__heroes toHero, global::System.Func<Rect?, Rect?, global::Doroti.Framework.Animation.Tween<Rect?>>? createRectTween, global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, HeroFlightDirection, BuildContext, BuildContext, Widget> shuttleBuilder, bool isUserGestureTransition, bool isDiverted)
    {
        this.type = type;
        this.overlay = overlay;
        this.navigatorSize = navigatorSize;
        this.fromRoute = fromRoute;
        this.toRoute = toRoute;
        this.fromHero = fromHero;
        this.toHero = toHero;
        this.createRectTween = createRectTween;
        this.shuttleBuilder = shuttleBuilder;
        this.isUserGestureTransition = isUserGestureTransition;
        this.isDiverted = isDiverted;
        System.Diagnostics.Debug.Assert((object.Equals(fromHero.widget.tag, toHero.widget.tag)));
    }

    public virtual object tag => this.fromHero.widget.tag;
    public virtual global::Doroti.Framework.Animation.Animation<double> animation
    {
        get
        {
            global::Doroti.Framework.Animation.Curve curveLocal = default!;
            global::Doroti.Framework.Animation.Curve reverseCurveLocal = default!;
            global::Doroti.Framework.Animation.Animation<double> parentLocal = default!;
            switch (this.type)
            {
                case HeroFlightDirection.push:
                    {
                        parentLocal = ((global::Doroti.Framework.Animation.Animation<double>?)((dynamic)this.toRoute).animation)!;
                        curveLocal = this.toHero.widget.curve;
                        reverseCurveLocal = ((this.toHero.widget.reverseCurve ?? (global::Doroti.Framework.Animation.Curve)((global::Doroti.Framework.Animation.Curve)curveLocal).flipped));
                        break;
                    }
                case HeroFlightDirection.pop:
                    {
                        parentLocal = ((global::Doroti.Framework.Animation.Animation<double>?)((dynamic)this.fromRoute).animation)!;
                        curveLocal = this.fromHero.widget.curve;
                        reverseCurveLocal = ((this.fromHero.widget.reverseCurve ?? (global::Doroti.Framework.Animation.Curve)((global::Doroti.Framework.Animation.Curve)curveLocal).flipped));
                        break;
                    }
            }
            return _animation ??= new global::Doroti.Framework.Animation.CurvedAnimation(parent: parentLocal, curve: curveLocal, reverseCurve: (this.isDiverted ? null : reverseCurveLocal));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Rect?> createHeroRectTween(Rect? begin, Rect? end)
    {
        global::System.Func<Rect?, Rect?, global::Doroti.Framework.Animation.Tween<Rect?>>? createRectTweenLocal = ((this.toHero.widget.createRectTween ?? (global::System.Func<Rect?, Rect?, global::Doroti.Framework.Animation.Tween<Rect?>>)this.createRectTween));
        return ((global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Rect?>)(object?)((createRectTweenLocal is null ? new global::Doroti.Framework.Animation.RectTween(begin: begin, end: end) : createRectTweenLocal.Invoke(begin, end))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Ui.Rect _boundingBoxFor(BuildContext context, BuildContext? ancestorContext)
    {
        DartRuntimePrimitives.Assert(() => (ancestorContext is not null));
        var box = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)context.findRenderObject()!)!;
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Rendering.RenderBox)box).hasSize && ((global::Doroti.Framework.Rendering.RenderBox)box).size.isFinite));
        return ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(box.getTransformTo(ancestorContext?.findRenderObject()), (Offset.zero & ((global::Doroti.Framework.Rendering.RenderBox)box).size)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"_HeroFlightManifest({this.type} tag: {this.tag} from route: {((RouteSettings)((dynamic)this.fromRoute).settings)} " + $"to route: {((RouteSettings)((dynamic)this.toRoute).settings)} with hero: {this.fromHero} to {this.toHero}){(this.isValid ? "" : ", INVALID")}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void dispose()
    {
        this._animation?.dispose();
    }

}

internal class _HeroFlight__heroes
{
    public virtual global::System.Action<_HeroFlight__heroes> onFlightEnded { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Tween<Rect?> heroRectTween { get; set; } = default!;
    public virtual Widget? shuttle { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.Animation<double> _heroOpacity { get; set; } = global::Doroti.Framework.Animation.AnimationsLibrary.kAlwaysCompleteAnimation;
    internal virtual global::Doroti.Framework.Animation.ProxyAnimation _proxyAnimation { get; set; } = default!;
    internal virtual _HeroFlightManifest__heroes? _manifest { get; set; } = default;
    public virtual OverlayEntry? overlayEntry { get; set; } = default;
    internal virtual bool _aborted { get; set; } = false;
    internal static global::Doroti.Framework.Animation.Animatable<double> _reverseTween = ((global::Doroti.Framework.Animation.Animatable<double>)(object?)new global::Doroti.Framework.Animation.Tween<double>(begin: 1.0, end: 0.0));
    internal virtual bool _scheduledPerformAnimationUpdate { get; set; } = false;

    internal _HeroFlight__heroes(global::System.Action<_HeroFlight__heroes> onFlightEnded)
    {
        this.onFlightEnded = onFlightEnded;
    }

    public virtual _HeroFlightManifest__heroes manifest
    {
        get => this._manifest!;
        set
        {
            var __value = value;
            this._manifest?.dispose();
            _manifest = __value;
        }
    }
    internal virtual Widget _buildOverlay(BuildContext context)
    {
        shuttle ??= this.manifest.shuttleBuilder(context, ((_HeroFlightManifest__heroes)this.manifest).animation, ((_HeroFlightManifest__heroes)this.manifest).type, ((_HeroFlightManifest__heroes)this.manifest).fromHero.context, ((_HeroFlightManifest__heroes)this.manifest).toHero.context);
        DartRuntimePrimitives.Assert(() => (this.shuttle is not null));
        return ((Widget)(object?)new AnimatedBuilder(animation: this._proxyAnimation, child: this.shuttle, builder: ((global::System.Func<BuildContext, Widget?, Widget>)((context, child) =>
        {
            global::Doroti.Ui.Rect rect = ((global::Doroti.Ui.Rect)(object?)DartRuntimePrimitives.RequireValue(this.heroRectTween.evaluate(this._proxyAnimation)));
            var offsets = global::Doroti.Framework.Rendering.RelativeRect.CreateFromSize(rect, ((_HeroFlightManifest__heroes)this.manifest).navigatorSize);
            return ((Widget)(object?)new Positioned(top: ((global::Doroti.Framework.Rendering.RelativeRect)offsets).top, right: ((global::Doroti.Framework.Rendering.RelativeRect)offsets).right, bottom: ((global::Doroti.Framework.Rendering.RelativeRect)offsets).bottom, left: ((global::Doroti.Framework.Rendering.RelativeRect)offsets).left, child: new IgnorePointer(child: new FadeTransition(opacity: this._heroOpacity, child: child))));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _performAnimationUpdate(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        if (!global::Doroti.Framework.Animation.AnimationStatusMembers.isAnimating(status))
        {
            this._proxyAnimation.parent = null;
            DartRuntimePrimitives.Assert(() => (this.overlayEntry is not null));
            this.overlayEntry!.remove();
            this.overlayEntry!.dispose();
            overlayEntry = null;
            ((_HeroFlightManifest__heroes)this.manifest).fromHero.endFlight(keepPlaceholder: global::Doroti.Framework.Animation.AnimationStatusMembers.isCompleted(status));
            ((_HeroFlightManifest__heroes)this.manifest).toHero.endFlight(keepPlaceholder: global::Doroti.Framework.Animation.AnimationStatusMembers.isDismissed(status));
            this.onFlightEnded(this);
            this._proxyAnimation.removeListener(this.onTick);
        }
    }

    internal virtual void _handleAnimationUpdate(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        if ((((NavigatorState?)((dynamic)((_HeroFlightManifest__heroes)this.manifest).fromRoute).navigator)?.userGestureInProgress != true))
        {
            _performAnimationUpdate(status);
            return;
        }
        if (this._scheduledPerformAnimationUpdate)
        {
            return;
        }
        NavigatorState navigatorLocal = ((NavigatorState?)((dynamic)((_HeroFlightManifest__heroes)this.manifest).fromRoute).navigator)!;
        void delayedPerformAnimationUpdate()
        {
            DartRuntimePrimitives.Assert(() => !((NavigatorState)navigatorLocal).userGestureInProgress);
            DartRuntimePrimitives.Assert(() => this._scheduledPerformAnimationUpdate);
            _scheduledPerformAnimationUpdate = false;
            ((NavigatorState)navigatorLocal).userGestureInProgressNotifier.removeListener(delayedPerformAnimationUpdate);
            _performAnimationUpdate(((global::Doroti.Framework.Animation.ProxyAnimation)this._proxyAnimation).status);
        }
        DartRuntimePrimitives.Assert(() => ((NavigatorState)navigatorLocal).userGestureInProgress);
        _scheduledPerformAnimationUpdate = true;
        ((NavigatorState)navigatorLocal).userGestureInProgressNotifier.addListener(delayedPerformAnimationUpdate);
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        if ((this.overlayEntry is not null))
        {
            this.overlayEntry!.remove();
            this.overlayEntry!.dispose();
            overlayEntry = null;
            this._proxyAnimation.parent = null;
            this._proxyAnimation.removeListener(this.onTick);
            this._proxyAnimation.removeStatusListener((AnimationStatusListener)this._handleAnimationUpdate);
        }
        this._manifest?.dispose();
    }

    public virtual void onTick()
    {
        global::Doroti.Framework.Rendering.RenderBox? toHeroBox = (((!this._aborted && ((_HeroFlightManifest__heroes)this.manifest).toHero.mounted)) ? ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((_HeroFlightManifest__heroes)this.manifest).toHero.context.findRenderObject())! : null);
        global::Doroti.Ui.Offset? toHeroOrigin = ((global::Doroti.Ui.Offset?)(object?)((((toHeroBox is not null) && toHeroBox.attached) && ((global::Doroti.Framework.Rendering.RenderBox)toHeroBox).hasSize) ? ((Offset)((dynamic)toHeroBox).localToGlobal(Offset.zero, ancestor: ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((BuildContext?)((dynamic)((_HeroFlightManifest__heroes)this.manifest).toRoute).subtreeContext)?.findRenderObject())!)) : null));
        if (((toHeroOrigin is not null) && DartRuntimePrimitives.RequireValue(toHeroOrigin).isFinite))
        {
            Offset toHeroOrigin__26130__value26392 = DartRuntimePrimitives.RequireValue(toHeroOrigin);
            if ((!object.Equals(DartRuntimePrimitives.RequireValue(toHeroOrigin__26130__value26392), DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Animation.Tween<Rect?>)this.heroRectTween).end).topLeft)))
            {
                global::Doroti.Ui.Rect heroRectEnd = ((global::Doroti.Ui.Rect)(object?)(DartRuntimePrimitives.RequireValue(toHeroOrigin__26130__value26392) & DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Animation.Tween<Rect?>)this.heroRectTween).end).size));
                heroRectTween = this.manifest.createHeroRectTween(begin: ((global::Doroti.Framework.Animation.Tween<Rect?>)this.heroRectTween).begin, end: heroRectEnd);
            }
        }
        else
        {
            if (((global::Doroti.Framework.Animation.Animation<double>)this._heroOpacity).isCompleted)
            {
                _heroOpacity = this._proxyAnimation.drive(_reverseTween.chain(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Interval(((global::Doroti.Framework.Animation.ProxyAnimation)this._proxyAnimation).value, 1.0))));
            }
        }
        _aborted = ((toHeroOrigin is null) || !DartRuntimePrimitives.RequireValue(toHeroOrigin).isFinite);
    }

    public virtual void start(_HeroFlightManifest__heroes initialManifest)
    {
        DartRuntimePrimitives.Assert(() => !this._aborted);
        DartRuntimePrimitives.Assert(() =>
            {
                global::Doroti.Framework.Animation.Animation<double> initial = ((_HeroFlightManifest__heroes)initialManifest).animation;
                HeroFlightDirection typeLocal = ((_HeroFlightManifest__heroes)initialManifest).type;
                switch (typeLocal)
                {
                    case HeroFlightDirection.pop:
                        {
                            return (((_HeroFlightManifest__heroes)initialManifest).isUserGestureTransition || (object.Equals(((global::Doroti.Framework.Animation.Animation<double>)initial).status, global::Doroti.Framework.Animation.AnimationStatus.reverse)));
                        }
                    case HeroFlightDirection.push:
                        {
                            return ((((global::Doroti.Framework.Animation.Animation<double>)initial).value == 0.0) && (object.Equals(((global::Doroti.Framework.Animation.Animation<double>)initial).status, global::Doroti.Framework.Animation.AnimationStatus.forward)));
                        }
                    default:
                        throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                }
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        manifest = initialManifest;
        bool shouldIncludeChildInPlaceholder = default!;
        switch (((_HeroFlightManifest__heroes)this.manifest).type)
        {
            case HeroFlightDirection.pop:
                {
                    this._proxyAnimation.parent = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.Animation<double>>(new global::Doroti.Framework.Animation.ReverseAnimation(((_HeroFlightManifest__heroes)this.manifest).animation));
                    shouldIncludeChildInPlaceholder = false;
                    break;
                }
            case HeroFlightDirection.push:
                {
                    this._proxyAnimation.parent = ((_HeroFlightManifest__heroes)this.manifest).animation;
                    shouldIncludeChildInPlaceholder = true;
                    break;
                }
        }
        heroRectTween = this.manifest.createHeroRectTween(begin: ((_HeroFlightManifest__heroes)this.manifest).fromHeroLocation, end: ((_HeroFlightManifest__heroes)this.manifest).toHeroLocation);
        ((_HeroFlightManifest__heroes)this.manifest).fromHero.startFlight(shouldIncludedChildInPlaceholder: shouldIncludeChildInPlaceholder);
        ((_HeroFlightManifest__heroes)this.manifest).toHero.startFlight();
        ((_HeroFlightManifest__heroes)this.manifest).overlay.insert(overlayEntry = new OverlayEntry(builder: (global::System.Func<BuildContext, Widget>)this._buildOverlay));
        this._proxyAnimation.addListener(this.onTick);
    }

    public virtual void divert(_HeroFlightManifest__heroes newManifest)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((_HeroFlightManifest__heroes)this.manifest).tag, ((_HeroFlightManifest__heroes)newManifest).tag)));
        if (((object.Equals(((_HeroFlightManifest__heroes)this.manifest).type, HeroFlightDirection.push)) && (object.Equals(((_HeroFlightManifest__heroes)newManifest).type, HeroFlightDirection.pop))))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(((_HeroFlightManifest__heroes)newManifest).animation.status, global::Doroti.Framework.Animation.AnimationStatus.reverse)));
            DartRuntimePrimitives.Assert(() => (object.Equals(((_HeroFlightManifest__heroes)this.manifest).fromHero, ((_HeroFlightManifest__heroes)newManifest).toHero)));
            DartRuntimePrimitives.Assert(() => (object.Equals(((_HeroFlightManifest__heroes)this.manifest).toHero, ((_HeroFlightManifest__heroes)newManifest).fromHero)));
            DartRuntimePrimitives.Assert(() => (object.Equals(((_HeroFlightManifest__heroes)this.manifest).fromRoute, ((_HeroFlightManifest__heroes)newManifest).toRoute)));
            DartRuntimePrimitives.Assert(() => (object.Equals(((_HeroFlightManifest__heroes)this.manifest).toRoute, ((_HeroFlightManifest__heroes)newManifest).fromRoute)));
            this._proxyAnimation.parent = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.Animation<double>>(new global::Doroti.Framework.Animation.ReverseAnimation(((_HeroFlightManifest__heroes)newManifest).animation));
            heroRectTween = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.Tween<Rect?>>(new global::Doroti.Framework.Animation.ReverseTween<global::Doroti.Ui.Rect?>(this.heroRectTween));
        }
        else
        {
            if (((object.Equals(((_HeroFlightManifest__heroes)this.manifest).type, HeroFlightDirection.pop)) && (object.Equals(((_HeroFlightManifest__heroes)newManifest).type, HeroFlightDirection.push))))
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(((_HeroFlightManifest__heroes)newManifest).animation.status, global::Doroti.Framework.Animation.AnimationStatus.forward)));
                DartRuntimePrimitives.Assert(() => (object.Equals(((_HeroFlightManifest__heroes)this.manifest).toHero, ((_HeroFlightManifest__heroes)newManifest).fromHero)));
                DartRuntimePrimitives.Assert(() => (object.Equals(((_HeroFlightManifest__heroes)this.manifest).toRoute, ((_HeroFlightManifest__heroes)newManifest).fromRoute)));
                this._proxyAnimation.parent = ((_HeroFlightManifest__heroes)newManifest).animation.drive(new global::Doroti.Framework.Animation.Tween<double>(begin: ((_HeroFlightManifest__heroes)this.manifest).animation.value, end: 1.0));
                if ((!object.Equals(((_HeroFlightManifest__heroes)this.manifest).fromHero, ((_HeroFlightManifest__heroes)newManifest).toHero)))
                {
                    ((_HeroFlightManifest__heroes)this.manifest).fromHero.endFlight(keepPlaceholder: true);
                    ((_HeroFlightManifest__heroes)newManifest).toHero.startFlight();
                    heroRectTween = this.manifest.createHeroRectTween(begin: ((global::Doroti.Framework.Animation.Tween<Rect?>)this.heroRectTween).end, end: ((_HeroFlightManifest__heroes)newManifest).toHeroLocation);
                }
                else
                {
                    heroRectTween = this.manifest.createHeroRectTween(begin: ((global::Doroti.Framework.Animation.Tween<Rect?>)this.heroRectTween).end, end: ((global::Doroti.Framework.Animation.Tween<Rect?>)this.heroRectTween).begin);
                }
            }
            else
            {
                DartRuntimePrimitives.Assert(() => (!object.Equals(((_HeroFlightManifest__heroes)this.manifest).fromHero, ((_HeroFlightManifest__heroes)newManifest).fromHero)));
                DartRuntimePrimitives.Assert(() => (!object.Equals(((_HeroFlightManifest__heroes)this.manifest).toHero, ((_HeroFlightManifest__heroes)newManifest).toHero)));
                heroRectTween = this.manifest.createHeroRectTween(begin: this.heroRectTween.evaluate(this._proxyAnimation), end: ((_HeroFlightManifest__heroes)newManifest).toHeroLocation);
                shuttle = null;
                if ((object.Equals(((_HeroFlightManifest__heroes)newManifest).type, HeroFlightDirection.pop)))
                {
                    this._proxyAnimation.parent = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.Animation<double>>(new global::Doroti.Framework.Animation.ReverseAnimation(((_HeroFlightManifest__heroes)newManifest).animation));
                }
                else
                {
                    this._proxyAnimation.parent = ((_HeroFlightManifest__heroes)newManifest).animation;
                }
                ((_HeroFlightManifest__heroes)this.manifest).fromHero.endFlight(keepPlaceholder: true);
                ((_HeroFlightManifest__heroes)this.manifest).toHero.endFlight(keepPlaceholder: true);
                ((_HeroFlightManifest__heroes)newManifest).fromHero.startFlight(shouldIncludedChildInPlaceholder: (object.Equals(((_HeroFlightManifest__heroes)newManifest).type, HeroFlightDirection.push)));
                ((_HeroFlightManifest__heroes)newManifest).toHero.startFlight();
                this.overlayEntry!.markNeedsBuild();
            }
        }
        manifest = newManifest;
    }

    public virtual void abort()
    {
        _aborted = true;
    }

    public override string ToString()
    {
        RouteSettings @from = ((RouteSettings)((dynamic)((_HeroFlightManifest__heroes)this.manifest).fromRoute).settings);
        RouteSettings to = ((RouteSettings)((dynamic)((_HeroFlightManifest__heroes)this.manifest).toRoute).settings);
        object tagLocal = ((_HeroFlightManifest__heroes)this.manifest).tag;
        return $"HeroFlight(for: {tagLocal}, from: {@from}, to: {to} {(((global::Doroti.Framework.Animation.ProxyAnimation)this._proxyAnimation).parent)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class HeroController : NavigatorObserver
{
    public virtual global::System.Func<Rect?, Rect?, global::Doroti.Framework.Animation.Tween<Rect?>>? createRectTween { get; private set; }
    internal virtual DartMap<object, _HeroFlight__heroes> _flights { get; private set; } = new DartMap<object, _HeroFlight__heroes>();

    public HeroController(global::System.Func<Rect?, Rect?, global::Doroti.Framework.Animation.Tween<Rect?>>? createRectTween = null)
    {
        this.createRectTween = createRectTween;
    }

    public override void didChangeTop(dynamic topRoute, dynamic previousTopRoute)
    {
        DartRuntimePrimitives.Assert(() => ((bool)((dynamic)topRoute).isCurrent));
        DartRuntimePrimitives.Assert(() => (this.navigator is not null));
        if ((previousTopRoute is null))
        {
            return;
        }
        if (!this.navigator!.userGestureInProgress)
        {
            _maybeStartHeroTransition(fromRoute: previousTopRoute, toRoute: topRoute, isUserGestureTransition: false);
        }
    }

    public override void didStartUserGesture(dynamic route, dynamic previousRoute)
    {
        DartRuntimePrimitives.Assert(() => (this.navigator is not null));
        _maybeStartHeroTransition(fromRoute: route, toRoute: previousRoute, isUserGestureTransition: true);
    }

    public override void didStopUserGesture()
    {
        if (this.navigator!.userGestureInProgress)
        {
            return;
        }
        bool isInvalidFlight(_HeroFlight__heroes flight)
        {
            return ((((_HeroFlight__heroes)flight).manifest.isUserGestureTransition && (object.Equals(((_HeroFlight__heroes)flight).manifest.type, HeroFlightDirection.pop))) && ((_HeroFlight__heroes)flight)._proxyAnimation.isDismissed);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        List<_HeroFlight__heroes> invalidFlights = this._flights.Values.where(isInvalidFlight).ToList().ToList();
        foreach (var flightLocal in invalidFlights)
        {
            flightLocal._handleAnimationUpdate(global::Doroti.Framework.Animation.AnimationStatus.dismissed);
        }
    }

    internal virtual void _maybeStartHeroTransition(dynamic fromRoute, dynamic toRoute, bool isUserGestureTransition)
    {
        if ((((object.Equals(toRoute, fromRoute)) || (toRoute is not PageRoute<dynamic>)) || (fromRoute is not PageRoute<dynamic>)))
        {
            return;
        }
        global::Doroti.Framework.Animation.Animation<double> newRouteAnimation = ((global::Doroti.Framework.Animation.Animation<double>?)((dynamic)toRoute).animation)!;
        global::Doroti.Framework.Animation.Animation<double> oldRouteAnimation = ((global::Doroti.Framework.Animation.Animation<double>?)((dynamic)fromRoute).animation)!;
        HeroFlightDirection? flightType = default!;
        switch ((isUserGestureTransition, ((global::Doroti.Framework.Animation.Animation<double>)oldRouteAnimation).status, ((global::Doroti.Framework.Animation.Animation<double>)newRouteAnimation).status))
        {
            case (true, _, _):
            case (_, global::Doroti.Framework.Animation.AnimationStatus.reverse, _):
                {
                    flightType = HeroFlightDirection.pop;
                    break;
                }
            case (_, _, global::Doroti.Framework.Animation.AnimationStatus.forward):
                {
                    flightType = HeroFlightDirection.push;
                    break;
                }
            default:
                {
                    flightType = null;
                    break;
                }
        }
        if ((flightType is not null))
        {
            HeroFlightDirection flightType__36140__value36599 = DartRuntimePrimitives.RequireValue(flightType);
            switch (DartRuntimePrimitives.RequireValue(flightType__36140__value36599))
            {
                case HeroFlightDirection.pop:
                    {
                        if ((((global::Doroti.Framework.Animation.Animation<double>?)((dynamic)fromRoute).animation)!.value == 0.0))
                        {
                            return;
                        }
                        break;
                    }
                case HeroFlightDirection.push:
                    {
                        if ((((global::Doroti.Framework.Animation.Animation<double>?)((dynamic)toRoute).animation)!.value == 1.0))
                        {
                            return;
                        }
                        break;
                    }
            }
        }
        var fromRouteRenderBox = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((BuildContext?)((dynamic)toRoute).subtreeContext)?.findRenderObject())!;
        bool hasValidSize = (((fromRouteRenderBox?.hasSize ?? false)) && fromRouteRenderBox!.size.isFinite);
        if ((((isUserGestureTransition && (object.Equals(flightType, HeroFlightDirection.pop))) && ((bool)((dynamic)toRoute).maintainState)) && hasValidSize))
        {
            _startHeroTransition(fromRoute, toRoute, flightType, isUserGestureTransition);
        }
        else
        {
            ((dynamic)toRoute).offstage = (((global::Doroti.Framework.Animation.Animation<double>?)((dynamic)toRoute).animation)!.value == 0.0);
            WidgetsBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((value) =>
            {
                if (((((NavigatorState?)((dynamic)fromRoute).navigator) is null) || (((NavigatorState?)((dynamic)toRoute).navigator) is null)))
                {
                    return;
                }
                _startHeroTransition(fromRoute, toRoute, flightType, isUserGestureTransition);
            })), debugLabel: "HeroController.startTransition");
        }
    }

    internal virtual void _startHeroTransition(dynamic from, dynamic to, HeroFlightDirection? flightType, bool isUserGestureTransition)
    {
        ((dynamic)to).offstage = false;
        NavigatorState? navigatorLocal = this.navigator;
        OverlayState? overlayLocal = navigatorLocal?.overlay;
        if (((navigatorLocal is null) || (overlayLocal is null)))
        {
            return;
        }
        global::Doroti.Framework.Rendering.RenderObject? navigatorRenderObject = ((global::Doroti.Framework.Rendering.RenderObject?)(object?)navigatorLocal.context.findRenderObject());
        if ((navigatorRenderObject is not global::Doroti.Framework.Rendering.RenderBox))
        {
            DartRuntimePrimitives.Assert(() => false, () => (object?)$"Navigator {navigatorLocal} has an invalid RenderObject type {DartRuntimePrimitives.RuntimeType(navigatorRenderObject)}.");
            return;
        }
        DartRuntimePrimitives.Assert(() => ((global::Doroti.Framework.Rendering.RenderBox)((global::Doroti.Framework.Rendering.RenderBox)navigatorRenderObject)).hasSize);
        BuildContext? fromSubtreeContext = ((BuildContext?)((dynamic)from).subtreeContext);
        DartMap<object, _HeroState__heroes> fromHeroes = ((fromSubtreeContext is not null) ? Hero._allHeroesFor(fromSubtreeContext, isUserGestureTransition, navigatorLocal) : new DartMap<object, _HeroState__heroes>());
        BuildContext? toSubtreeContext = ((BuildContext?)((dynamic)to).subtreeContext);
        DartMap<object, _HeroState__heroes> toHeroes = ((toSubtreeContext is not null) ? Hero._allHeroesFor(toSubtreeContext, isUserGestureTransition, navigatorLocal) : new DartMap<object, _HeroState__heroes>());
        foreach (MapEntry<object, _HeroState__heroes> fromHeroEntry in fromHeroes.entries)
        {
            object tag = fromHeroEntry.key;
            _HeroState__heroes fromHeroLocal = fromHeroEntry.value;
            _HeroState__heroes? toHeroLocal = toHeroes.GetValueOrDefault(tag);
            _HeroFlight__heroes? existingFlight = this._flights.GetValueOrDefault(tag);
            _HeroFlightManifest__heroes? manifest = (((toHeroLocal is null) || (flightType is null)) ? null : new _HeroFlightManifest__heroes(type: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(flightType)), overlay: overlayLocal, navigatorSize: ((global::Doroti.Framework.Rendering.RenderBox)((global::Doroti.Framework.Rendering.RenderBox)navigatorRenderObject)).size, fromRoute: from, toRoute: to, fromHero: fromHeroLocal, toHero: toHeroLocal, createRectTween: (global::System.Func<Rect?, Rect?, global::Doroti.Framework.Animation.Tween<Rect?>>?)this.createRectTween, shuttleBuilder: ((((toHeroLocal.widget.flightShuttleBuilder ?? (global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, HeroFlightDirection, BuildContext, BuildContext, Widget>)fromHeroLocal.widget.flightShuttleBuilder)) ?? (global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, HeroFlightDirection, BuildContext, BuildContext, Widget>)this._defaultHeroFlightShuttleBuilder)), isUserGestureTransition: isUserGestureTransition, isDiverted: (existingFlight is not null)));
            if (((manifest is not null) && ((_HeroFlightManifest__heroes)manifest).isValid))
            {
                toHeroes.remove(tag);
                if ((existingFlight is not null))
                {
                    existingFlight.divert(manifest);
                }
                else
                {
                    this._flights[tag] = ((Func<_HeroFlight__heroes>)(() =>
{
    var __cascade = new _HeroFlight__heroes((global::System.Action<_HeroFlight__heroes>)this._handleFlightEnded);
    __cascade.start(manifest);
    return __cascade;
}))();
                }
            }
            else
            {
                existingFlight?.abort();
            }
        }
        foreach (_HeroState__heroes toHeroAlternate in toHeroes.Values)
        {
            toHeroAlternate.endFlight();
        }
    }

    internal virtual void _handleFlightEnded(_HeroFlight__heroes flight)
    {
        this._flights.remove(((_HeroFlight__heroes)flight).manifest.tag)?.dispose();
    }

    internal virtual Widget _defaultHeroFlightShuttleBuilder(BuildContext flightContext, global::Doroti.Framework.Animation.Animation<double> animation, HeroFlightDirection flightDirection, BuildContext fromHeroContext, BuildContext toHeroContext)
    {
        var toHero = ((Hero?)(object?)((BuildContext)toHeroContext).widget)!;
        MediaQueryData? toMediaQueryData = ((MediaQueryData?)(object?)MediaQuery.maybeOf(toHeroContext));
        MediaQueryData? fromMediaQueryData = ((MediaQueryData?)(object?)MediaQuery.maybeOf(fromHeroContext));
        if (((toMediaQueryData is null) || (fromMediaQueryData is null)))
        {
            return ((Hero)toHero).child;
        }
        global::Doroti.Framework.Painting.EdgeInsets fromHeroPadding = ((MediaQueryData)fromMediaQueryData).padding;
        global::Doroti.Framework.Painting.EdgeInsets toHeroPadding = ((MediaQueryData)toMediaQueryData).padding;
        return ((Widget)(object?)new AnimatedBuilder(animation: animation, builder: ((global::System.Func<BuildContext, Widget?, Widget>)((context, child) =>
        {
            return ((Widget)(object?)new MediaQuery(data: toMediaQueryData.copyWith(padding: (((object.Equals(flightDirection, HeroFlightDirection.push))) ? new EdgeInsetsTween(begin: fromHeroPadding, end: toHeroPadding).evaluate(animation) : new EdgeInsetsTween(begin: toHeroPadding, end: fromHeroPadding).evaluate(animation))), child: ((Hero)toHero).child));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        foreach (_HeroFlight__heroes flight in this._flights.Values)
        {
            flight.dispose();
        }
    }

}

public class HeroMode : StatelessWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual bool enabled { get; private set; } = default!;

    public HeroMode(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!, bool enabled = true) : base(key: key)
    {
        this.child = child;
        this.enabled = enabled;
    }

    public override Widget build(BuildContext context) => this.child;
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("mode", value: this.enabled, ifTrue: "enabled", ifFalse: "disabled", showName: true));
    }

}

