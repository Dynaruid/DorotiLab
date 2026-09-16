// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/heroes.dart
using Doroti.Runtime;
using Doroti.Ui;

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
        global::Doroti.Framework.Animation.Curve __curve = curve ?? Curves.fastOutSlowIn;
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
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("There are multiple heroes that share the same tag within a subtree."), new global::Doroti.Framework.Foundation.ErrorDescription("Within each subtree for which heroes are to be animated (i.e. a PageRoute subtree), " + "each Hero must have a unique non-null tag.\n" + $"In this case, multiple heroes had the following tag: {tag}"), new global::Doroti.Framework.Foundation.DiagnosticsProperty<StatefulElement>("Here is the subtree for one of the offending heroes", hero, linePrefix: "# ", style: DiagnosticsTreeStyle.dense) }));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            var heroWidget = ((Hero?)hero.widget)!;
            var heroState = ((_HeroState__heroes?)hero.state)!;
            if (!isUserGestureTransition || heroWidget.transitionOnUserGestures)
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
            Widget widgetLocal = element.widget;
            if (widgetLocal is Hero)
            {
                Hero widget__13444__as13479 = (Hero)widgetLocal;
                var heroLocal = ((StatefulElement?)element)!;
                object tagLocal = widget__13444__as13479.tag;
                if (Equals(Navigator.of(heroLocal), navigator))
                {
                    inviteHero(heroLocal, tagLocal);
                }
                else
                {
                    IModalRoute? heroRoute = ModalRoute<object>.untypedOf(heroLocal);
                    if ((heroRoute is not null) && (heroRoute is IPageRoute) && heroRoute.isCurrent)
                    {
                        var heroRoute__14091__as14159 = heroRoute;
                        inviteHero(heroLocal, tagLocal);
                    }
                }
            }
            else
            {
                if ((widgetLocal is HeroMode) && !((HeroMode)widgetLocal).enabled)
                {
                    HeroMode widget__13444__as14282 = (HeroMode)widgetLocal;
                    return;
                }
            }
            element.visitChildren(visitor);
        }
        context.visitChildElements(visitor);
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _HeroState__heroes());
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<object>("tag", tag));
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
        DartRuntimePrimitives.Assert(() => mounted);
        var box = ((global::Doroti.Framework.Rendering.RenderBox?)context.findRenderObject()!)!;
        DartRuntimePrimitives.Assert(() => box.hasSize);
        setState(() =>
        {
            _placeholderSize = box.size;
        });
    }

    public virtual void endFlight(bool keepPlaceholder = false)
    {
        if (keepPlaceholder || (_placeholderSize is null))
        {
            return;
        }
        _placeholderSize = null;
        if (mounted)
        {
            setState(() =>
            {
            });
        }
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => context.findAncestorWidgetOfExactType<Hero>() is null, () => (object?)"A Hero widget cannot be the descendant of another Hero widget.");
        var showPlaceholder = _placeholderSize is not null;
        if (showPlaceholder && (widget.placeholderBuilder is not null))
        {
            return widget.placeholderBuilder!(context, DartRuntimePrimitives.RequireValue(_placeholderSize), widget.child);
        }
        if (showPlaceholder && !_shouldIncludeChild)
        {
            return new SizedBox(width: DartRuntimePrimitives.RequireValue(_placeholderSize).width, height: DartRuntimePrimitives.RequireValue(_placeholderSize).height);
        }
        return new SizedBox(width: _placeholderSize?.width, height: _placeholderSize?.height, child: new Offstage(offstage: showPlaceholder, child: new TickerMode(enabled: !showPlaceholder, child: new KeyedSubtree(key: _key, child: widget.child))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _HeroFlightManifest__heroes
{
    public virtual HeroFlightDirection type { get; private set; } = default!;
    public virtual OverlayState overlay { get; private set; } = default!;
    public virtual Size navigatorSize { get; private set; } = default!;
    public virtual IPageRoute fromRoute { get; private set; } = default!;
    public virtual IPageRoute toRoute { get; private set; } = default!;
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
                __late_fromHeroLocation = _boundingBoxFor(fromHero.context, fromRoute.subtreeContext);
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
                __late_toHeroLocation = _boundingBoxFor(toHero.context, toRoute.subtreeContext);
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
                __late_isValid = toHeroLocation.isFinite && (isDiverted || fromHeroLocation.isFinite);
                __late_isValid_initialized = true;
            }
            return __late_isValid;
        }
    }

    internal _HeroFlightManifest__heroes(HeroFlightDirection type, OverlayState overlay, Size navigatorSize, IPageRoute fromRoute, IPageRoute toRoute, _HeroState__heroes fromHero, _HeroState__heroes toHero, global::System.Func<Rect?, Rect?, global::Doroti.Framework.Animation.Tween<Rect?>>? createRectTween, global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, HeroFlightDirection, BuildContext, BuildContext, Widget> shuttleBuilder, bool isUserGestureTransition, bool isDiverted)
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
        System.Diagnostics.Debug.Assert(Equals(fromHero.widget.tag, toHero.widget.tag));
    }

    public virtual object tag => fromHero.widget.tag;
    public virtual global::Doroti.Framework.Animation.Animation<double> animation
    {
        get
        {
            global::Doroti.Framework.Animation.Curve curveLocal = default!;
            global::Doroti.Framework.Animation.Curve reverseCurveLocal = default!;
            global::Doroti.Framework.Animation.Animation<double> parentLocal = default!;
            switch (type)
            {
                case HeroFlightDirection.push:
                    {
                        parentLocal = toRoute.animation!;
                        curveLocal = toHero.widget.curve;
                        reverseCurveLocal = toHero.widget.reverseCurve ?? curveLocal.flipped;
                        break;
                    }
                case HeroFlightDirection.pop:
                    {
                        parentLocal = fromRoute.animation!;
                        curveLocal = fromHero.widget.curve;
                        reverseCurveLocal = fromHero.widget.reverseCurve ?? curveLocal.flipped;
                        break;
                    }
            }
            return _animation ??= new global::Doroti.Framework.Animation.CurvedAnimation(parent: parentLocal, curve: curveLocal, reverseCurve: isDiverted ? null : reverseCurveLocal);
        }
    }
    public virtual global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Rect?> createHeroRectTween(Rect? begin, Rect? end)
    {
        global::System.Func<Rect?, Rect?, global::Doroti.Framework.Animation.Tween<Rect?>>? createRectTweenLocal = toHero.widget.createRectTween ?? createRectTween;
        return createRectTweenLocal is null ? new global::Doroti.Framework.Animation.RectTween(begin: begin, end: end) : createRectTweenLocal.Invoke(begin, end);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Ui.Rect _boundingBoxFor(BuildContext context, BuildContext? ancestorContext)
    {
        DartRuntimePrimitives.Assert(() => ancestorContext is not null);
        var box = ((global::Doroti.Framework.Rendering.RenderBox?)context.findRenderObject()!)!;
        DartRuntimePrimitives.Assert(() => box.hasSize && box.size.isFinite);
        return MatrixUtils.transformRect(box.getTransformTo(ancestorContext?.findRenderObject()), Offset.zero & box.size);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"_HeroFlightManifest({type} tag: {tag} from route: {fromRoute.settings} " + $"to route: {toRoute.settings} with hero: {fromHero} to {toHero}){(isValid ? "" : ", INVALID")}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void dispose()
    {
        _animation?.dispose();
    }

}

internal class _HeroFlight__heroes
{
    public virtual global::System.Action<_HeroFlight__heroes> onFlightEnded { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Tween<Rect?> heroRectTween { get; set; } = default!;
    public virtual Widget? shuttle { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.Animation<double> _heroOpacity { get; set; } = AnimationsLibrary.kAlwaysCompleteAnimation;
    internal virtual global::Doroti.Framework.Animation.ProxyAnimation _proxyAnimation { get; set; } = new global::Doroti.Framework.Animation.ProxyAnimation();
    internal virtual _HeroFlightManifest__heroes? _manifest { get; set; } = default;
    public virtual OverlayEntry? overlayEntry { get; set; } = default;
    internal virtual bool _aborted { get; set; } = false;
    internal static global::Doroti.Framework.Animation.Animatable<double> _reverseTween = new global::Doroti.Framework.Animation.Tween<double>(begin: 1.0, end: 0.0);
    internal virtual bool _scheduledPerformAnimationUpdate { get; set; } = false;

    internal _HeroFlight__heroes(global::System.Action<_HeroFlight__heroes> onFlightEnded)
    {
        this.onFlightEnded = onFlightEnded;
    }

    public virtual _HeroFlightManifest__heroes manifest
    {
        get => _manifest!;
        set
        {
            var __value = value;
            _manifest?.dispose();
            _manifest = __value;
        }
    }
    internal virtual Widget _buildOverlay(BuildContext context)
    {
        shuttle ??= manifest.shuttleBuilder(context, manifest.animation, manifest.type, manifest.fromHero.context, manifest.toHero.context);
        DartRuntimePrimitives.Assert(() => shuttle is not null);
        return new AnimatedBuilder(animation: _proxyAnimation, child: shuttle, builder: (context, child) =>
        {
            global::Doroti.Ui.Rect rect = DartRuntimePrimitives.RequireValue(heroRectTween.evaluate(_proxyAnimation));
            var offsets = RelativeRect.CreateFromSize(rect, manifest.navigatorSize);
            return new Positioned(top: offsets.top, right: offsets.right, bottom: offsets.bottom, left: offsets.left, child: new IgnorePointer(child: new FadeTransition(opacity: _heroOpacity, child: child)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _performAnimationUpdate(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        if (!AnimationStatusMembers.isAnimating(status))
        {
            _proxyAnimation.parent = null;
            DartRuntimePrimitives.Assert(() => overlayEntry is not null);
            overlayEntry!.remove();
            overlayEntry!.dispose();
            overlayEntry = null;
            manifest.fromHero.endFlight(keepPlaceholder: AnimationStatusMembers.isCompleted(status));
            manifest.toHero.endFlight(keepPlaceholder: AnimationStatusMembers.isDismissed(status));
            onFlightEnded(this);
            _proxyAnimation.removeListener(onTick);
        }
    }

    internal virtual void _handleAnimationUpdate(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        if (manifest.fromRoute.navigator?.userGestureInProgress != true)
        {
            _performAnimationUpdate(status);
            return;
        }
        if (_scheduledPerformAnimationUpdate)
        {
            return;
        }
        NavigatorState navigatorLocal = manifest.fromRoute.navigator!;
        void delayedPerformAnimationUpdate()
        {
            DartRuntimePrimitives.Assert(() => !navigatorLocal.userGestureInProgress);
            DartRuntimePrimitives.Assert(() => _scheduledPerformAnimationUpdate);
            _scheduledPerformAnimationUpdate = false;
            navigatorLocal.userGestureInProgressNotifier.removeListener(delayedPerformAnimationUpdate);
            _performAnimationUpdate(_proxyAnimation.status);
        }
        DartRuntimePrimitives.Assert(() => navigatorLocal.userGestureInProgress);
        _scheduledPerformAnimationUpdate = true;
        navigatorLocal.userGestureInProgressNotifier.addListener(delayedPerformAnimationUpdate);
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        if (overlayEntry is not null)
        {
            overlayEntry!.remove();
            overlayEntry!.dispose();
            overlayEntry = null;
            _proxyAnimation.parent = null;
            _proxyAnimation.removeListener(onTick);
            _proxyAnimation.removeStatusListener(_handleAnimationUpdate);
        }
        _manifest?.dispose();
    }

    public virtual void onTick()
    {
        global::Doroti.Framework.Rendering.RenderBox? toHeroBox = (!_aborted && manifest.toHero.mounted) ? ((global::Doroti.Framework.Rendering.RenderBox?)manifest.toHero.context.findRenderObject())! : null;
        global::Doroti.Ui.Offset? toHeroOrigin = (global::Doroti.Ui.Offset?)(object?)(((toHeroBox is not null) && toHeroBox.attached && toHeroBox.hasSize) ? toHeroBox.localToGlobal(Offset.zero, ancestor: ((global::Doroti.Framework.Rendering.RenderBox?)manifest.toRoute.subtreeContext?.findRenderObject())!) : null);
        if ((toHeroOrigin is not null) && DartRuntimePrimitives.RequireValue(toHeroOrigin).isFinite)
        {
            Offset toHeroOrigin__26130__value26392 = DartRuntimePrimitives.RequireValue(toHeroOrigin);
            if (!Equals(DartRuntimePrimitives.RequireValue(toHeroOrigin__26130__value26392), DartRuntimePrimitives.RequireValue(heroRectTween.end).topLeft))
            {
                global::Doroti.Ui.Rect heroRectEnd = DartRuntimePrimitives.RequireValue(toHeroOrigin__26130__value26392) & DartRuntimePrimitives.RequireValue(heroRectTween.end).size;
                heroRectTween = manifest.createHeroRectTween(begin: heroRectTween.begin, end: heroRectEnd);
            }
        }
        else
        {
            if (_heroOpacity.isCompleted)
            {
                _heroOpacity = _proxyAnimation.drive(_reverseTween.chain(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Interval(_proxyAnimation.value, 1.0))));
            }
        }
        _aborted = (toHeroOrigin is null) || !DartRuntimePrimitives.RequireValue(toHeroOrigin).isFinite;
    }

    public virtual void start(_HeroFlightManifest__heroes initialManifest)
    {
        DartRuntimePrimitives.Assert(() => !_aborted);
        DartRuntimePrimitives.Assert(() =>
            {
                global::Doroti.Framework.Animation.Animation<double> initial = initialManifest.animation;
                HeroFlightDirection typeLocal = initialManifest.type;
                switch (typeLocal)
                {
                    case HeroFlightDirection.pop:
                        {
                            return initialManifest.isUserGestureTransition || Equals(initial.status, AnimationStatus.reverse);
                        }
                    case HeroFlightDirection.push:
                        {
                            return (initial.value == 0.0) && Equals(initial.status, AnimationStatus.forward);
                        }
                    default:
                        throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                }
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        manifest = initialManifest;
        bool shouldIncludeChildInPlaceholder = default!;
        switch (manifest.type)
        {
            case HeroFlightDirection.pop:
                {
                    _proxyAnimation.parent = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.Animation<double>>(new global::Doroti.Framework.Animation.ReverseAnimation(manifest.animation));
                    shouldIncludeChildInPlaceholder = false;
                    break;
                }
            case HeroFlightDirection.push:
                {
                    _proxyAnimation.parent = manifest.animation;
                    shouldIncludeChildInPlaceholder = true;
                    break;
                }
        }
        heroRectTween = manifest.createHeroRectTween(begin: manifest.fromHeroLocation, end: manifest.toHeroLocation);
        manifest.fromHero.startFlight(shouldIncludedChildInPlaceholder: shouldIncludeChildInPlaceholder);
        manifest.toHero.startFlight();
        manifest.overlay.insert(overlayEntry = new OverlayEntry(builder: _buildOverlay));
        _proxyAnimation.addListener(onTick);
    }

    public virtual void divert(_HeroFlightManifest__heroes newManifest)
    {
        DartRuntimePrimitives.Assert(() => Equals(manifest.tag, newManifest.tag));
        if (Equals(manifest.type, HeroFlightDirection.push) && Equals(newManifest.type, HeroFlightDirection.pop))
        {
            DartRuntimePrimitives.Assert(() => Equals(newManifest.animation.status, AnimationStatus.reverse));
            DartRuntimePrimitives.Assert(() => Equals(manifest.fromHero, newManifest.toHero));
            DartRuntimePrimitives.Assert(() => Equals(manifest.toHero, newManifest.fromHero));
            DartRuntimePrimitives.Assert(() => Equals(manifest.fromRoute, newManifest.toRoute));
            DartRuntimePrimitives.Assert(() => Equals(manifest.toRoute, newManifest.fromRoute));
            _proxyAnimation.parent = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.Animation<double>>(new global::Doroti.Framework.Animation.ReverseAnimation(newManifest.animation));
            heroRectTween = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.Tween<Rect?>>(new global::Doroti.Framework.Animation.ReverseTween<global::Doroti.Ui.Rect?>(heroRectTween));
        }
        else
        {
            if (Equals(manifest.type, HeroFlightDirection.pop) && Equals(newManifest.type, HeroFlightDirection.push))
            {
                DartRuntimePrimitives.Assert(() => Equals(newManifest.animation.status, AnimationStatus.forward));
                DartRuntimePrimitives.Assert(() => Equals(manifest.toHero, newManifest.fromHero));
                DartRuntimePrimitives.Assert(() => Equals(manifest.toRoute, newManifest.fromRoute));
                _proxyAnimation.parent = newManifest.animation.drive(new global::Doroti.Framework.Animation.Tween<double>(begin: manifest.animation.value, end: 1.0));
                if (!Equals(manifest.fromHero, newManifest.toHero))
                {
                    manifest.fromHero.endFlight(keepPlaceholder: true);
                    newManifest.toHero.startFlight();
                    heroRectTween = manifest.createHeroRectTween(begin: heroRectTween.end, end: newManifest.toHeroLocation);
                }
                else
                {
                    heroRectTween = manifest.createHeroRectTween(begin: heroRectTween.end, end: heroRectTween.begin);
                }
            }
            else
            {
                DartRuntimePrimitives.Assert(() => !Equals(manifest.fromHero, newManifest.fromHero));
                DartRuntimePrimitives.Assert(() => !Equals(manifest.toHero, newManifest.toHero));
                heroRectTween = manifest.createHeroRectTween(begin: heroRectTween.evaluate(_proxyAnimation), end: newManifest.toHeroLocation);
                shuttle = null;
                if (Equals(newManifest.type, HeroFlightDirection.pop))
                {
                    _proxyAnimation.parent = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.Animation<double>>(new global::Doroti.Framework.Animation.ReverseAnimation(newManifest.animation));
                }
                else
                {
                    _proxyAnimation.parent = newManifest.animation;
                }
                manifest.fromHero.endFlight(keepPlaceholder: true);
                manifest.toHero.endFlight(keepPlaceholder: true);
                newManifest.fromHero.startFlight(shouldIncludedChildInPlaceholder: Equals(newManifest.type, HeroFlightDirection.push));
                newManifest.toHero.startFlight();
                overlayEntry!.markNeedsBuild();
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
        RouteSettings @from = manifest.fromRoute.settings;
        RouteSettings to = manifest.toRoute.settings;
        object tagLocal = manifest.tag;
        return $"HeroFlight(for: {tagLocal}, from: {@from}, to: {to} {_proxyAnimation.parent})";
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

    public override void didChangeTop(dynamic topRoute, dynamic? previousTopRoute)
    {
        DartRuntimePrimitives.Assert(() => ((RouteBase)(object)topRoute).isCurrent);
        DartRuntimePrimitives.Assert(() => navigator is not null);
        if (previousTopRoute is null)
        {
            return;
        }
        if (!navigator!.userGestureInProgress)
        {
            _maybeStartHeroTransition(fromRouteValue: (object?)previousTopRoute, toRouteValue: (object?)topRoute, isUserGestureTransition: false);
        }
    }

    public override void didStartUserGesture(dynamic route, dynamic? previousRoute)
    {
        DartRuntimePrimitives.Assert(() => navigator is not null);
        _maybeStartHeroTransition(fromRouteValue: (object?)route, toRouteValue: (object?)previousRoute, isUserGestureTransition: true);
    }

    public override void didStopUserGesture()
    {
        if (navigator!.userGestureInProgress)
        {
            return;
        }
        bool isInvalidFlight(_HeroFlight__heroes flight)
        {
            return flight.manifest.isUserGestureTransition && Equals(flight.manifest.type, HeroFlightDirection.pop) && flight._proxyAnimation.isDismissed;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        List<_HeroFlight__heroes> invalidFlights = _flights.Values.where(isInvalidFlight).ToList().ToList();
        foreach (var flightLocal in invalidFlights)
        {
            flightLocal._handleAnimationUpdate(AnimationStatus.dismissed);
        }
    }

    internal virtual void _maybeStartHeroTransition(object? fromRouteValue, object? toRouteValue, bool isUserGestureTransition)
    {
        if (fromRouteValue is not IPageRoute fromRoute || toRouteValue is not IPageRoute toRoute || ReferenceEquals(toRoute, fromRoute))
        {
            return;
        }
        global::Doroti.Framework.Animation.Animation<double> newRouteAnimation = toRoute.animation!;
        global::Doroti.Framework.Animation.Animation<double> oldRouteAnimation = fromRoute.animation!;
        HeroFlightDirection? flightType = default!;
        switch ((isUserGestureTransition, oldRouteAnimation.status, newRouteAnimation.status))
        {
            case (true, _, _):
            case (_, AnimationStatus.reverse, _):
                {
                    flightType = HeroFlightDirection.pop;
                    break;
                }
            case (_, _, AnimationStatus.forward):
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
        if (flightType is not null)
        {
            HeroFlightDirection flightType__36140__value36599 = DartRuntimePrimitives.RequireValue(flightType);
            switch (DartRuntimePrimitives.RequireValue(flightType__36140__value36599))
            {
                case HeroFlightDirection.pop:
                    {
                        if (fromRoute.animation!.value == 0.0)
                        {
                            return;
                        }
                        break;
                    }
                case HeroFlightDirection.push:
                    {
                        if (toRoute.animation!.value == 1.0)
                        {
                            return;
                        }
                        break;
                    }
            }
        }
        var fromRouteRenderBox = ((global::Doroti.Framework.Rendering.RenderBox?)toRoute.subtreeContext?.findRenderObject())!;
        bool hasValidSize = (fromRouteRenderBox?.hasSize ?? false) && fromRouteRenderBox!.size.isFinite;
        if (isUserGestureTransition && Equals(flightType, HeroFlightDirection.pop) && toRoute.maintainState && hasValidSize)
        {
            _startHeroTransition(fromRoute, toRoute, flightType, isUserGestureTransition);
        }
        else
        {
            toRoute.offstage = toRoute.animation!.value == 0.0;
            WidgetsBinding.instance.addPostFrameCallback((value) =>
            {
                if ((fromRoute.navigator is null) || (toRoute.navigator is null))
                {
                    return;
                }
                _startHeroTransition(fromRoute, toRoute, flightType, isUserGestureTransition);
            }, debugLabel: "HeroController.startTransition");
        }
    }

    internal virtual void _startHeroTransition(IPageRoute from, IPageRoute to, HeroFlightDirection? flightType, bool isUserGestureTransition)
    {
        to.offstage = false;
        NavigatorState? navigatorLocal = navigator;
        OverlayState? overlayLocal = navigatorLocal?.overlay;
        if ((navigatorLocal is null) || (overlayLocal is null))
        {
            return;
        }
        global::Doroti.Framework.Rendering.RenderObject? navigatorRenderObject = navigatorLocal.context.findRenderObject();
        if (navigatorRenderObject is not RenderBox)
        {
            DartRuntimePrimitives.Assert(() => false, () => (object?)$"Navigator {navigatorLocal} has an invalid RenderObject type {DartRuntimePrimitives.RuntimeType(navigatorRenderObject)}.");
            return;
        }
        DartRuntimePrimitives.Assert(() => ((global::Doroti.Framework.Rendering.RenderBox)navigatorRenderObject).hasSize);
        BuildContext? fromSubtreeContext = from.subtreeContext;
        DartMap<object, _HeroState__heroes> fromHeroes = (fromSubtreeContext is not null) ? Hero._allHeroesFor(fromSubtreeContext, isUserGestureTransition, navigatorLocal) : new DartMap<object, _HeroState__heroes>();
        BuildContext? toSubtreeContext = to.subtreeContext;
        DartMap<object, _HeroState__heroes> toHeroes = (toSubtreeContext is not null) ? Hero._allHeroesFor(toSubtreeContext, isUserGestureTransition, navigatorLocal) : new DartMap<object, _HeroState__heroes>();
        foreach (MapEntry<object, _HeroState__heroes> fromHeroEntry in fromHeroes.entries)
        {
            object tag = fromHeroEntry.key;
            _HeroState__heroes fromHeroLocal = fromHeroEntry.value;
            _HeroState__heroes? toHeroLocal = toHeroes.GetValueOrDefault(tag);
            _HeroFlight__heroes? existingFlight = _flights.GetValueOrDefault(tag);
            _HeroFlightManifest__heroes? manifest = ((toHeroLocal is null) || (flightType is null)) ? null : new _HeroFlightManifest__heroes(type: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(flightType)), overlay: overlayLocal, navigatorSize: ((global::Doroti.Framework.Rendering.RenderBox)navigatorRenderObject).size, fromRoute: from, toRoute: to, fromHero: fromHeroLocal, toHero: toHeroLocal, createRectTween: createRectTween, shuttleBuilder: (toHeroLocal.widget.flightShuttleBuilder ?? fromHeroLocal.widget.flightShuttleBuilder) ?? _defaultHeroFlightShuttleBuilder, isUserGestureTransition: isUserGestureTransition, isDiverted: existingFlight is not null);
            if ((manifest is not null) && manifest.isValid)
            {
                toHeroes.remove(tag);
                if (existingFlight is not null)
                {
                    existingFlight.divert(manifest);
                }
                else
                {
                    _flights[tag] = ((Func<_HeroFlight__heroes>)(() =>
{
    var __cascade = new _HeroFlight__heroes(_handleFlightEnded);
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
        _flights.remove(flight.manifest.tag)?.dispose();
    }

    internal virtual Widget _defaultHeroFlightShuttleBuilder(BuildContext flightContext, global::Doroti.Framework.Animation.Animation<double> animation, HeroFlightDirection flightDirection, BuildContext fromHeroContext, BuildContext toHeroContext)
    {
        var toHero = ((Hero?)toHeroContext.widget)!;
        MediaQueryData? toMediaQueryData = MediaQuery.maybeOf(toHeroContext);
        MediaQueryData? fromMediaQueryData = MediaQuery.maybeOf(fromHeroContext);
        if ((toMediaQueryData is null) || (fromMediaQueryData is null))
        {
            return toHero.child;
        }
        global::Doroti.Framework.Painting.EdgeInsets fromHeroPadding = fromMediaQueryData.padding;
        global::Doroti.Framework.Painting.EdgeInsets toHeroPadding = toMediaQueryData.padding;
        return new AnimatedBuilder(animation: animation, builder: (context, child) =>
        {
            return new MediaQuery(data: toMediaQueryData.copyWith(padding: Equals(flightDirection, HeroFlightDirection.push) ? new EdgeInsetsTween(begin: fromHeroPadding, end: toHeroPadding).evaluate(animation) : new EdgeInsetsTween(begin: toHeroPadding, end: fromHeroPadding).evaluate(animation)), child: toHero.child);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        foreach (_HeroFlight__heroes flight in _flights.Values)
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

    public override Widget build(BuildContext context) => child;
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("mode", value: enabled, ifTrue: "enabled", ifFalse: "disabled", showName: true));
    }

}

