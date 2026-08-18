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
        var result__12097 = new DartMap<object, _HeroState__heroes>();
        void inviteHero(StatefulElement hero, object tag)
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    if (result__12097.ContainsKey(tag))
                    {
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("There are multiple heroes that share the same tag within a subtree."), new global::Doroti.Framework.Foundation.ErrorDescription("Within each subtree for which heroes are to be animated (i.e. a PageRoute subtree), " + "each Hero must have a unique non-null tag.\n" + $"In this case, multiple heroes had the following tag: {tag}"), new global::Doroti.Framework.Foundation.DiagnosticsProperty<StatefulElement>("Here is the subtree for one of the offending heroes", hero, linePrefix: "# ", style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.dense) }));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            var heroWidget__12985 = ((Hero?)(object?)hero.widget)!;
            var heroState__13031 = ((_HeroState__heroes?)(object?)((StatefulElement)hero).state)!;
            if ((!isUserGestureTransition || ((Hero)heroWidget__12985).transitionOnUserGestures))
            {
                result__12097[tag] = heroState__13031;
            }
            else
            {
                heroState__13031.endFlight();
            }
        }
        void visitor(Element element)
        {
            Widget widget__13444 = ((Element)element).widget;
            if ((widget__13444 is Hero))
            {
                Hero widget__13444__as13479 = (Hero)widget__13444;
                var hero__13511 = ((StatefulElement?)(object?)element)!;
                object tag__13567 = ((Hero)((Hero)widget__13444__as13479)).tag;
                if ((object.Equals(Navigator.of(hero__13511), navigator)))
                {
                    inviteHero(hero__13511, tag__13567);
                }
                else
                {
                    dynamic heroRoute__14091 = ModalRoute<object>.of<object>(hero__13511);
                    if ((((heroRoute__14091 is not null) && (heroRoute__14091 is PageRoute<object>)) && ((bool)((dynamic)heroRoute__14091).isCurrent)))
                    {
                        dynamic heroRoute__14091__as14159 = (dynamic)heroRoute__14091;
                        inviteHero(hero__13511, tag__13567);
                    }
                }
            }
            else
            {
                if (((widget__13444 is HeroMode) && !((HeroMode)((HeroMode)widget__13444)).enabled))
                {
                    HeroMode widget__13444__as14282 = (HeroMode)widget__13444;
                    return;
                }
            }
            element.visitChildren((global::System.Action<Element>)visitor);
        }
        context.visitChildElements((global::System.Action<Element>)visitor);
        return result__12097;
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
        var box__16348 = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)this.context.findRenderObject()!)!;
        DartRuntimePrimitives.Assert(() => ((global::Doroti.Framework.Rendering.RenderBox)box__16348).hasSize);
        setState(((global::System.Action)(() =>
        {
            _placeholderSize = ((global::Doroti.Framework.Rendering.RenderBox)box__16348).size;
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
        var showPlaceholder__17318 = (this._placeholderSize is not null);
        if ((showPlaceholder__17318 && (((Hero)this.widget).placeholderBuilder is not null)))
        {
            return ((Hero)this.widget).placeholderBuilder!(context, DartRuntimePrimitives.RequireValue(this._placeholderSize), ((Hero)this.widget).child);
        }
        if ((showPlaceholder__17318 && !this._shouldIncludeChild))
        {
            return ((Widget)(object?)new SizedBox(width: DartRuntimePrimitives.RequireValue(this._placeholderSize).width, height: DartRuntimePrimitives.RequireValue(this._placeholderSize).height));
        }
        return ((Widget)(object?)new SizedBox(width: this._placeholderSize?.width, height: this._placeholderSize?.height, child: new Offstage(offstage: showPlaceholder__17318, child: new TickerMode(enabled: !showPlaceholder__17318, child: new KeyedSubtree(key: this._key, child: ((Hero)this.widget).child)))));
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
            global::Doroti.Framework.Animation.Curve curve__19005 = default!;
            global::Doroti.Framework.Animation.Curve reverseCurve__19012 = default!;
            global::Doroti.Framework.Animation.Animation<double> parent__19054 = default!;
            switch (this.type)
            {
                case HeroFlightDirection.push:
                    {
                        parent__19054 = ((global::Doroti.Framework.Animation.Animation<double>?)((dynamic)this.toRoute).animation)!;
                        curve__19005 = this.toHero.widget.curve;
                        reverseCurve__19012 = ((this.toHero.widget.reverseCurve ?? (global::Doroti.Framework.Animation.Curve)((global::Doroti.Framework.Animation.Curve)curve__19005).flipped));
                        break;
                    }
                case HeroFlightDirection.pop:
                    {
                        parent__19054 = ((global::Doroti.Framework.Animation.Animation<double>?)((dynamic)this.fromRoute).animation)!;
                        curve__19005 = this.fromHero.widget.curve;
                        reverseCurve__19012 = ((this.fromHero.widget.reverseCurve ?? (global::Doroti.Framework.Animation.Curve)((global::Doroti.Framework.Animation.Curve)curve__19005).flipped));
                        break;
                    }
            }
            return _animation ??= new global::Doroti.Framework.Animation.CurvedAnimation(parent: parent__19054, curve: curve__19005, reverseCurve: (this.isDiverted ? null : reverseCurve__19012));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Rect?> createHeroRectTween(Rect? begin, Rect? end)
    {
        global::System.Func<Rect?, Rect?, global::Doroti.Framework.Animation.Tween<Rect?>>? createRectTween__19711 = ((this.toHero.widget.createRectTween ?? (global::System.Func<Rect?, Rect?, global::Doroti.Framework.Animation.Tween<Rect?>>)this.createRectTween));
        return ((global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Rect?>)(object?)((createRectTween__19711 is null ? new global::Doroti.Framework.Animation.RectTween(begin: begin, end: end) : createRectTween__19711.Invoke(begin, end))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Ui.Rect _boundingBoxFor(BuildContext context, BuildContext? ancestorContext)
    {
        DartRuntimePrimitives.Assert(() => (ancestorContext is not null));
        var box__20120 = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)context.findRenderObject()!)!;
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Rendering.RenderBox)box__20120).hasSize && ((global::Doroti.Framework.Rendering.RenderBox)box__20120).size.isFinite));
        return ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(box__20120.getTransformTo(ancestorContext?.findRenderObject()), (Offset.zero & ((global::Doroti.Framework.Rendering.RenderBox)box__20120).size)));
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
            global::Doroti.Ui.Rect rect__22996 = ((global::Doroti.Ui.Rect)(object?)DartRuntimePrimitives.RequireValue(this.heroRectTween.evaluate(this._proxyAnimation)));
            var offsets__23059 = global::Doroti.Framework.Rendering.RelativeRect.CreateFromSize(rect__22996, ((_HeroFlightManifest__heroes)this.manifest).navigatorSize);
            return ((Widget)(object?)new Positioned(top: ((global::Doroti.Framework.Rendering.RelativeRect)offsets__23059).top, right: ((global::Doroti.Framework.Rendering.RelativeRect)offsets__23059).right, bottom: ((global::Doroti.Framework.Rendering.RelativeRect)offsets__23059).bottom, left: ((global::Doroti.Framework.Rendering.RelativeRect)offsets__23059).left, child: new IgnorePointer(child: new FadeTransition(opacity: this._heroOpacity, child: child))));
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
            this._proxyAnimation.removeListener(() => this.onTick());
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
        NavigatorState navigator__24876 = ((NavigatorState?)((dynamic)((_HeroFlightManifest__heroes)this.manifest).fromRoute).navigator)!;
        void delayedPerformAnimationUpdate()
        {
            DartRuntimePrimitives.Assert(() => !((NavigatorState)navigator__24876).userGestureInProgress);
            DartRuntimePrimitives.Assert(() => this._scheduledPerformAnimationUpdate);
            _scheduledPerformAnimationUpdate = false;
            ((NavigatorState)navigator__24876).userGestureInProgressNotifier.removeListener(() => delayedPerformAnimationUpdate());
            _performAnimationUpdate(((global::Doroti.Framework.Animation.ProxyAnimation)this._proxyAnimation).status);
        }
        DartRuntimePrimitives.Assert(() => ((NavigatorState)navigator__24876).userGestureInProgress);
        _scheduledPerformAnimationUpdate = true;
        ((NavigatorState)navigator__24876).userGestureInProgressNotifier.addListener(() => delayedPerformAnimationUpdate());
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
            this._proxyAnimation.removeListener(() => this.onTick());
            this._proxyAnimation.removeStatusListener((AnimationStatusListener)this._handleAnimationUpdate);
        }
        this._manifest?.dispose();
    }

    public virtual void onTick()
    {
        global::Doroti.Framework.Rendering.RenderBox? toHeroBox__25900 = (((!this._aborted && ((_HeroFlightManifest__heroes)this.manifest).toHero.mounted)) ? ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((_HeroFlightManifest__heroes)this.manifest).toHero.context.findRenderObject())! : null);
        global::Doroti.Ui.Offset? toHeroOrigin__26130 = ((global::Doroti.Ui.Offset?)(object?)((((toHeroBox__25900 is not null) && toHeroBox__25900.attached) && ((global::Doroti.Framework.Rendering.RenderBox)toHeroBox__25900).hasSize) ? ((Offset)((dynamic)toHeroBox__25900).localToGlobal(Offset.zero, ancestor: ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((BuildContext?)((dynamic)((_HeroFlightManifest__heroes)this.manifest).toRoute).subtreeContext)?.findRenderObject())!)) : null));
        if (((toHeroOrigin__26130 is not null) && DartRuntimePrimitives.RequireValue(toHeroOrigin__26130).isFinite))
        {
            Offset toHeroOrigin__26130__value26392 = DartRuntimePrimitives.RequireValue(toHeroOrigin__26130);
            if ((!object.Equals(DartRuntimePrimitives.RequireValue(toHeroOrigin__26130__value26392), DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Animation.Tween<Rect?>)this.heroRectTween).end).topLeft)))
            {
                global::Doroti.Ui.Rect heroRectEnd__26632 = ((global::Doroti.Ui.Rect)(object?)(DartRuntimePrimitives.RequireValue(toHeroOrigin__26130__value26392) & DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Animation.Tween<Rect?>)this.heroRectTween).end).size));
                heroRectTween = this.manifest.createHeroRectTween(begin: ((global::Doroti.Framework.Animation.Tween<Rect?>)this.heroRectTween).begin, end: heroRectEnd__26632);
            }
        }
        else
        {
            if (((global::Doroti.Framework.Animation.Animation<double>)this._heroOpacity).isCompleted)
            {
                _heroOpacity = this._proxyAnimation.drive(_reverseTween.chain(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Interval(((global::Doroti.Framework.Animation.ProxyAnimation)this._proxyAnimation).value, 1.0))));
            }
        }
        _aborted = ((toHeroOrigin__26130 is null) || !DartRuntimePrimitives.RequireValue(toHeroOrigin__26130).isFinite);
    }

    public virtual void start(_HeroFlightManifest__heroes initialManifest)
    {
        DartRuntimePrimitives.Assert(() => !this._aborted);
        DartRuntimePrimitives.Assert(() =>
            {
                global::Doroti.Framework.Animation.Animation<double> initial__27418 = ((_HeroFlightManifest__heroes)initialManifest).animation;
                HeroFlightDirection type__27487 = ((_HeroFlightManifest__heroes)initialManifest).type;
                switch (type__27487)
                {
                    case HeroFlightDirection.pop:
                        {
                            return (((_HeroFlightManifest__heroes)initialManifest).isUserGestureTransition || (object.Equals(((global::Doroti.Framework.Animation.Animation<double>)initial__27418).status, global::Doroti.Framework.Animation.AnimationStatus.reverse)));
                        }
                    case HeroFlightDirection.push:
                        {
                            return ((((global::Doroti.Framework.Animation.Animation<double>)initial__27418).value == 0.0) && (object.Equals(((global::Doroti.Framework.Animation.Animation<double>)initial__27418).status, global::Doroti.Framework.Animation.AnimationStatus.forward)));
                        }
                    default:
                        throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                }
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        manifest = initialManifest;
        bool shouldIncludeChildInPlaceholder__28059 = default!;
        switch (((_HeroFlightManifest__heroes)this.manifest).type)
        {
            case HeroFlightDirection.pop:
                {
                    this._proxyAnimation.parent = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.Animation<double>>(new global::Doroti.Framework.Animation.ReverseAnimation(((_HeroFlightManifest__heroes)this.manifest).animation));
                    shouldIncludeChildInPlaceholder__28059 = false;
                    break;
                }
            case HeroFlightDirection.push:
                {
                    this._proxyAnimation.parent = ((_HeroFlightManifest__heroes)this.manifest).animation;
                    shouldIncludeChildInPlaceholder__28059 = true;
                    break;
                }
        }
        heroRectTween = this.manifest.createHeroRectTween(begin: ((_HeroFlightManifest__heroes)this.manifest).fromHeroLocation, end: ((_HeroFlightManifest__heroes)this.manifest).toHeroLocation);
        ((_HeroFlightManifest__heroes)this.manifest).fromHero.startFlight(shouldIncludedChildInPlaceholder: shouldIncludeChildInPlaceholder__28059);
        ((_HeroFlightManifest__heroes)this.manifest).toHero.startFlight();
        ((_HeroFlightManifest__heroes)this.manifest).overlay.insert(overlayEntry = new OverlayEntry(builder: (global::System.Func<BuildContext, Widget>)this._buildOverlay));
        this._proxyAnimation.addListener(() => this.onTick());
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
        RouteSettings from__32497 = ((RouteSettings)((dynamic)((_HeroFlightManifest__heroes)this.manifest).fromRoute).settings);
        RouteSettings to__32557 = ((RouteSettings)((dynamic)((_HeroFlightManifest__heroes)this.manifest).toRoute).settings);
        object tag__32606 = ((_HeroFlightManifest__heroes)this.manifest).tag;
        return $"HeroFlight(for: {tag__32606}, from: {from__32497}, to: {to__32557} {(((global::Doroti.Framework.Animation.ProxyAnimation)this._proxyAnimation).parent)})";
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
        List<_HeroFlight__heroes> invalidFlights__35159 = this._flights.Values.where(isInvalidFlight).ToList().ToList();
        foreach (var flight__35408 in invalidFlights__35159)
        {
            flight__35408._handleAnimationUpdate(global::Doroti.Framework.Animation.AnimationStatus.dismissed);
        }
    }

    internal virtual void _maybeStartHeroTransition(dynamic fromRoute, dynamic toRoute, bool isUserGestureTransition)
    {
        if ((((object.Equals(toRoute, fromRoute)) || (toRoute is not PageRoute<dynamic>)) || (fromRoute is not PageRoute<dynamic>)))
        {
            return;
        }
        global::Doroti.Framework.Animation.Animation<double> newRouteAnimation__35999 = ((global::Doroti.Framework.Animation.Animation<double>?)((dynamic)toRoute).animation)!;
        global::Doroti.Framework.Animation.Animation<double> oldRouteAnimation__36067 = ((global::Doroti.Framework.Animation.Animation<double>?)((dynamic)fromRoute).animation)!;
        HeroFlightDirection? flightType__36140 = default!;
        switch ((isUserGestureTransition, ((global::Doroti.Framework.Animation.Animation<double>)oldRouteAnimation__36067).status, ((global::Doroti.Framework.Animation.Animation<double>)newRouteAnimation__35999).status))
        {
            case (true, _, _):
            case (_, global::Doroti.Framework.Animation.AnimationStatus.reverse, _):
                {
                    flightType__36140 = HeroFlightDirection.pop;
                    break;
                }
            case (_, _, global::Doroti.Framework.Animation.AnimationStatus.forward):
                {
                    flightType__36140 = HeroFlightDirection.push;
                    break;
                }
            default:
                {
                    flightType__36140 = null;
                    break;
                }
        }
        if ((flightType__36140 is not null))
        {
            HeroFlightDirection flightType__36140__value36599 = DartRuntimePrimitives.RequireValue(flightType__36140);
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
        var fromRouteRenderBox__37234 = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((BuildContext?)((dynamic)toRoute).subtreeContext)?.findRenderObject())!;
        bool hasValidSize__37328 = (((fromRouteRenderBox__37234?.hasSize ?? false)) && fromRouteRenderBox__37234!.size.isFinite);
        if ((((isUserGestureTransition && (object.Equals(flightType__36140, HeroFlightDirection.pop))) && ((bool)((dynamic)toRoute).maintainState)) && hasValidSize__37328))
        {
            _startHeroTransition(fromRoute, toRoute, flightType__36140, isUserGestureTransition);
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
                _startHeroTransition(fromRoute, toRoute, flightType__36140, isUserGestureTransition);
            })), debugLabel: "HeroController.startTransition");
        }
    }

    internal virtual void _startHeroTransition(dynamic from, dynamic to, HeroFlightDirection? flightType, bool isUserGestureTransition)
    {
        ((dynamic)to).offstage = false;
        NavigatorState? navigator__38884 = this.navigator;
        OverlayState? overlay__38936 = navigator__38884?.overlay;
        if (((navigator__38884 is null) || (overlay__38936 is null)))
        {
            return;
        }
        global::Doroti.Framework.Rendering.RenderObject? navigatorRenderObject__39329 = ((global::Doroti.Framework.Rendering.RenderObject?)(object?)navigator__38884.context.findRenderObject());
        if ((navigatorRenderObject__39329 is not global::Doroti.Framework.Rendering.RenderBox))
        {
            DartRuntimePrimitives.Assert(() => false, () => (object?)$"Navigator {navigator__38884} has an invalid RenderObject type {DartRuntimePrimitives.RuntimeType(navigatorRenderObject__39329)}.");
            return;
        }
        DartRuntimePrimitives.Assert(() => ((global::Doroti.Framework.Rendering.RenderBox)((global::Doroti.Framework.Rendering.RenderBox)navigatorRenderObject__39329)).hasSize);
        BuildContext? fromSubtreeContext__39913 = ((BuildContext?)((dynamic)from).subtreeContext);
        DartMap<object, _HeroState__heroes> fromHeroes__39989 = ((fromSubtreeContext__39913 is not null) ? Hero._allHeroesFor(fromSubtreeContext__39913, isUserGestureTransition, navigator__38884) : new DartMap<object, _HeroState__heroes>());
        BuildContext? toSubtreeContext__40178 = ((BuildContext?)((dynamic)to).subtreeContext);
        DartMap<object, _HeroState__heroes> toHeroes__40250 = ((toSubtreeContext__40178 is not null) ? Hero._allHeroesFor(toSubtreeContext__40178, isUserGestureTransition, navigator__38884) : new DartMap<object, _HeroState__heroes>());
        foreach (MapEntry<object, _HeroState__heroes> fromHeroEntry__40454 in fromHeroes__39989.entries)
        {
            object tag__40512 = fromHeroEntry__40454.key;
            _HeroState__heroes fromHero__40560 = fromHeroEntry__40454.value;
            _HeroState__heroes? toHero__40616 = toHeroes__40250.GetValueOrDefault(tag__40512);
            _HeroFlight__heroes? existingFlight__40665 = this._flights.GetValueOrDefault(tag__40512);
            _HeroFlightManifest__heroes? manifest__40730 = (((toHero__40616 is null) || (flightType is null)) ? null : new _HeroFlightManifest__heroes(type: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(flightType)), overlay: overlay__38936, navigatorSize: ((global::Doroti.Framework.Rendering.RenderBox)((global::Doroti.Framework.Rendering.RenderBox)navigatorRenderObject__39329)).size, fromRoute: from, toRoute: to, fromHero: fromHero__40560, toHero: toHero__40616, createRectTween: (global::System.Func<Rect?, Rect?, global::Doroti.Framework.Animation.Tween<Rect?>>?)this.createRectTween, shuttleBuilder: ((((toHero__40616.widget.flightShuttleBuilder ?? (global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, HeroFlightDirection, BuildContext, BuildContext, Widget>)fromHero__40560.widget.flightShuttleBuilder)) ?? (global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, HeroFlightDirection, BuildContext, BuildContext, Widget>)this._defaultHeroFlightShuttleBuilder)), isUserGestureTransition: isUserGestureTransition, isDiverted: (existingFlight__40665 is not null)));
            if (((manifest__40730 is not null) && ((_HeroFlightManifest__heroes)manifest__40730).isValid))
            {
                toHeroes__40250.remove(tag__40512);
                if ((existingFlight__40665 is not null))
                {
                    existingFlight__40665.divert(manifest__40730);
                }
                else
                {
                    this._flights[tag__40512] = ((Func<_HeroFlight__heroes>)(() =>
{
    var __cascade = new _HeroFlight__heroes((global::System.Action<_HeroFlight__heroes>)this._handleFlightEnded);
    __cascade.start(manifest__40730);
    return __cascade;
}))();
                }
            }
            else
            {
                existingFlight__40665?.abort();
            }
        }
        foreach (_HeroState__heroes toHero__42337 in toHeroes__40250.Values)
        {
            toHero__42337.endFlight();
        }
    }

    internal virtual void _handleFlightEnded(_HeroFlight__heroes flight)
    {
        this._flights.remove(((_HeroFlight__heroes)flight).manifest.tag)?.dispose();
    }

    internal virtual Widget _defaultHeroFlightShuttleBuilder(BuildContext flightContext, global::Doroti.Framework.Animation.Animation<double> animation, HeroFlightDirection flightDirection, BuildContext fromHeroContext, BuildContext toHeroContext)
    {
        var toHero__42740 = ((Hero?)(object?)((BuildContext)toHeroContext).widget)!;
        MediaQueryData? toMediaQueryData__42806 = ((MediaQueryData?)(object?)MediaQuery.maybeOf(toHeroContext));
        MediaQueryData? fromMediaQueryData__42886 = ((MediaQueryData?)(object?)MediaQuery.maybeOf(fromHeroContext));
        if (((toMediaQueryData__42806 is null) || (fromMediaQueryData__42886 is null)))
        {
            return ((Hero)toHero__42740).child;
        }
        global::Doroti.Framework.Painting.EdgeInsets fromHeroPadding__43066 = ((MediaQueryData)fromMediaQueryData__42886).padding;
        global::Doroti.Framework.Painting.EdgeInsets toHeroPadding__43133 = ((MediaQueryData)toMediaQueryData__42806).padding;
        return ((Widget)(object?)new AnimatedBuilder(animation: animation, builder: ((global::System.Func<BuildContext, Widget?, Widget>)((context, child) =>
        {
            return ((Widget)(object?)new MediaQuery(data: toMediaQueryData__42806.copyWith(padding: (((object.Equals(flightDirection, HeroFlightDirection.push))) ? new EdgeInsetsTween(begin: fromHeroPadding__43066, end: toHeroPadding__43133).evaluate(animation) : new EdgeInsetsTween(begin: toHeroPadding__43133, end: fromHeroPadding__43066).evaluate(animation))), child: ((Hero)toHero__42740).child));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        foreach (_HeroFlight__heroes flight__43832 in this._flights.Values)
        {
            flight__43832.dispose();
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

