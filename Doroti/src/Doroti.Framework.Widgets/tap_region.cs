// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/tap_region.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public static partial class Tap_regionLibrary
{
    internal static bool _kDebugTapRegion = false;
}

public static partial class Tap_regionLibrary
{
    internal static bool _tapRegionDebug(string message, IEnumerable<string>? details = null)
    {
        if (_kDebugTapRegion)
        {
            PrintLibrary.debugPrint($"TAP REGION: {message}");
            if ((details is not null) && Enumerable.Any(details))
            {
                foreach (string detail in details)
                {
                    PrintLibrary.debugPrint($"    {detail}");
                }
            }
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public delegate void TapRegionCallback(Gestures.PointerDownEvent @event);

public delegate void TapRegionUpCallback(Gestures.PointerUpEvent @event);

public interface TapRegionRegistry
{
    public void registerTapRegion(RenderTapRegion region);
    public void unregisterTapRegion(RenderTapRegion region);
    public static TapRegionRegistry of(BuildContext context)
    {
        TapRegionRegistry? registry = maybeOf(context);
        DartRuntimePrimitives.Assert(() =>
        {
            if (registry is null)
            {
                throw DartRuntimePrimitives.AsException(
                    FlutterError.Create(
                        "TapRegionRegistry.of() was called with a context that does not contain a TapRegionSurface widget.\n"
                            + "No TapRegionSurface widget ancestor could be found starting from the context that was passed to "
                            + "TapRegionRegistry.of().\n"
                            + "The context used was:\n"
                            + $"  {context}"
                    )
                );
            }
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        return registry!;
    }
    public static TapRegionRegistry? maybeOf(BuildContext context)
    {
        return context.findAncestorRenderObjectOfType<RenderTapRegionSurface>();
    }
}

public class TapRegionSurface : SingleChildRenderObjectWidget
{
    public TapRegionSurface(Key? key = null, Widget child = default!)
        : base(key: key, child: child) { }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderTapRegionSurface();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderProxyBoxWithHitTestBehavior)renderObject;
    }
}

internal delegate void _ClassifiedTapRegions__tap_region();

public class RenderTapRegionSurface : RenderProxyBoxWithHitTestBehavior, TapRegionRegistry
{
    internal virtual Expando<BoxHitTestResult> _cachedResults { get; private set; } =
        new Expando<BoxHitTestResult>();
    internal virtual HashSet<RenderTapRegion> _registeredRegions { get; private set; } =
        new HashSet<RenderTapRegion>();
    internal virtual DartMap<object, HashSet<RenderTapRegion>> _groupIdToRegions
    {
        get;
        private set;
    } = new DartMap<object, HashSet<RenderTapRegion>>();

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        Framework.Semantics.SemanticsBinding.instance.addSemanticsActionListener(
            _handleSemanticsAction
        );
    }

    public override void detach()
    {
        Framework.Semantics.SemanticsBinding.instance.removeSemanticsActionListener(
            _handleSemanticsAction
        );
        base.detach();
    }

    internal virtual void _handleSemanticsAction(SemanticsActionEvent @event)
    {
        if (
            (!Equals(@event.type, SemanticsAction.tap))
            && (!Equals(@event.type, SemanticsAction.longPress))
        )
        {
            return;
        }
        if (!Enumerable.Any(_registeredRegions))
        {
            return;
        }
        Rect? globalRect =
            Framework.Semantics.SemanticsBinding.instance.getRectOfSemanticsNodeInViewCoordinates(
                checked((long)@event.viewId),
                @event.nodeId
            );
        if (globalRect is null)
        {
            return;
        }
        Offset globalCenter = DartRuntimePrimitives.RequireValue(globalRect).center;
        Offset localPosition = globalToLocal(globalCenter);
        var hitResult = new BoxHitTestResult();
        if (!hitTest(hitResult, position: localPosition))
        {
            return;
        }
        var (inside, outside) = _classifyRegions(hitResult);
        var syntheticEvent = new Gestures.PointerDownEvent(
            viewId: checked((long)@event.viewId),
            position: globalCenter
        );
        foreach (var region in outside)
        {
            DartRuntimePrimitives.Assert(() =>
                Tap_regionLibrary._tapRegionDebug(
                    $"Calling onTapOutside for {region} (from semantics action)"
                )
            );
            region.onTapOutside?.Invoke(syntheticEvent);
        }
        foreach (var regionLocal in inside)
        {
            DartRuntimePrimitives.Assert(() =>
                Tap_regionLibrary._tapRegionDebug(
                    $"Calling onTapInside for {regionLocal} (from semantics action)"
                )
            );
            regionLocal.onTapInside?.Invoke(syntheticEvent);
        }
    }

    public virtual void registerTapRegion(RenderTapRegion region)
    {
        DartRuntimePrimitives.Assert(() =>
            Tap_regionLibrary._tapRegionDebug($"Region {region} registered.")
        );
        DartRuntimePrimitives.Assert(() => !_registeredRegions.Contains(region));
        _registeredRegions.Add(region);
        if (region.groupId is not null)
        {
            _groupIdToRegions.putIfAbsent(region.groupId, () => new HashSet<RenderTapRegion>());
            _groupIdToRegions
                .GetValueOrDefault(DartRuntimePrimitives.RequireReference(region.groupId))!
                .Add(region);
        }
    }

    public virtual void unregisterTapRegion(RenderTapRegion region)
    {
        DartRuntimePrimitives.Assert(() =>
            Tap_regionLibrary._tapRegionDebug($"Region {region} unregistered.")
        );
        DartRuntimePrimitives.Assert(() => _registeredRegions.Contains(region));
        _registeredRegions.Remove(region);
        if (region.groupId is not null)
        {
            DartRuntimePrimitives.Assert(() => _groupIdToRegions.ContainsKey(region.groupId));
            _groupIdToRegions
                .GetValueOrDefault(DartRuntimePrimitives.RequireReference(region.groupId))!
                .Remove(region);
            if (
                !Enumerable.Any(
                    _groupIdToRegions.GetValueOrDefault(
                        DartRuntimePrimitives.RequireReference(region.groupId)
                    )!
                )
            )
            {
                _groupIdToRegions.remove(region.groupId);
            }
        }
    }

    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        if (!size.contains(position))
        {
            return false;
        }
        bool hitTarget = hitTestChildren(result, position: position) || hitTestSelf(position);
        if (hitTarget)
        {
            var entry = new BoxHitTestEntry(this, position);
            _cachedResults[entry.identity] = result;
            result.add(entry);
        }
        return hitTarget;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual (
        IEnumerable<RenderTapRegion> inside,
        IEnumerable<RenderTapRegion> outside
    ) _classifyRegions(BoxHitTestResult result)
    {
        IEnumerable<RenderTapRegion> hitRegions = _getRegionsHit(
                _registeredRegions,
                result.path.Cast<HitTestEntry<HitTestTarget>>()
            )
            .cast<RenderTapRegion>();
        DartRuntimePrimitives.Assert(() =>
            Tap_regionLibrary._tapRegionDebug($"Tap event hit {hitRegions.Count()} descendants.")
        );
        var insideRegions = new HashSet<RenderTapRegion>();
        foreach (RenderTapRegion region in hitRegions)
        {
            if (region.groupId is null)
            {
                insideRegions.Add(region);
                continue;
            }
            HashSet<RenderTapRegion>? groupedRegions = _groupIdToRegions.GetValueOrDefault(
                region.groupId
            );
            if (groupedRegions is not null)
            {
                insideRegions.UnionWith(groupedRegions);
            }
        }
        return (
            inside: insideRegions,
            outside: _registeredRegions.where((r) => !insideRegions.Contains(r)).ToList()
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void handleEvent(PointerEvent @event, HitTestEntry<HitTestTarget> entry)
    {
        DartRuntimePrimitives.Assert(() => debugHandleEvent(@event, entry));
        DartRuntimePrimitives.Assert(
            () =>
            {
                foreach (RenderTapRegion region in _registeredRegions)
                {
                    if (!region.enabled)
                    {
                        return false;
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            },
            () => (object?)"A RenderTapRegion was registered when it was disabled."
        );
        if ((@event is not Gestures.PointerDownEvent) && (@event is not Gestures.PointerUpEvent))
        {
            return;
        }
        if (!Enumerable.Any(_registeredRegions))
        {
            DartRuntimePrimitives.Assert(() =>
                Tap_regionLibrary._tapRegionDebug(
                    "Ignored tap event because no regions are registered."
                )
            );
            return;
        }
        BoxHitTestResult? result = _cachedResults[entry.identity];
        if (result is null)
        {
            DartRuntimePrimitives.Assert(() =>
                Tap_regionLibrary._tapRegionDebug(
                    "Ignored tap event because no surface descendants were hit."
                )
            );
            return;
        }
        var (inside, outside) = _classifyRegions(result);
        var consumeOutsideTapsLocal = false;
        foreach (var regionLocal in outside)
        {
            if (@event is Gestures.PointerDownEvent)
            {
                Gestures.PointerDownEvent @event__as14985 = (Gestures.PointerDownEvent)@event;
                DartRuntimePrimitives.Assert(() =>
                    Tap_regionLibrary._tapRegionDebug($"Calling onTapOutside for {regionLocal}")
                );
                regionLocal.onTapOutside?.Invoke(@event__as14985);
            }
            else
            {
                if (@event is Gestures.PointerUpEvent)
                {
                    Gestures.PointerUpEvent @event__as15142 = (Gestures.PointerUpEvent)@event;
                    DartRuntimePrimitives.Assert(() =>
                        Tap_regionLibrary._tapRegionDebug(
                            $"Calling onTapUpOutside for {regionLocal}"
                        )
                    );
                    regionLocal.onTapUpOutside?.Invoke(@event__as15142);
                }
            }
            if (regionLocal.consumeOutsideTaps)
            {
                DartRuntimePrimitives.Assert(() =>
                    Tap_regionLibrary._tapRegionDebug(
                        $"Stopping tap propagation for {regionLocal} (and all of {regionLocal.groupId})"
                    )
                );
                consumeOutsideTapsLocal = true;
            }
        }
        foreach (var regionAlternate in inside)
        {
            if (@event is Gestures.PointerDownEvent)
            {
                Gestures.PointerDownEvent @event__as15551 = (Gestures.PointerDownEvent)@event;
                DartRuntimePrimitives.Assert(() =>
                    Tap_regionLibrary._tapRegionDebug($"Calling onTapInside for {regionAlternate}")
                );
                regionAlternate.onTapInside?.Invoke(@event__as15551);
            }
            else
            {
                if (@event is Gestures.PointerUpEvent)
                {
                    Gestures.PointerUpEvent @event__as15706 = (Gestures.PointerUpEvent)@event;
                    DartRuntimePrimitives.Assert(() =>
                        Tap_regionLibrary._tapRegionDebug(
                            $"Calling onTapUpInside for {regionAlternate}"
                        )
                    );
                    regionAlternate.onTapUpInside?.Invoke(@event__as15706);
                }
            }
        }
        if (consumeOutsideTapsLocal && (@event is Gestures.PointerDownEvent))
        {
            Gestures.PointerDownEvent @event__as16104 = (Gestures.PointerDownEvent)@event;
            GestureBinding
                .instance.gestureArena.add(
                    @event__as16104.pointer,
                    new _DummyTapRecognizer__tap_region()
                )
                .resolve(GestureDisposition.accepted);
        }
    }

    internal virtual HashSet<HitTestTarget> _getRegionsHit(
        HashSet<RenderTapRegion> detectors,
        IEnumerable<HitTestEntry<HitTestTarget>> hitTestPath
    )
    {
        var regions = new HashSet<HitTestTarget>();
        foreach (HitTestEntry<HitTestTarget> entry in hitTestPath)
        {
            if (entry.target is RenderTapRegion region && detectors.Contains(region))
            {
                regions.Add(region);
            }
        }
        return regions;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _DummyTapRecognizer__tap_region : GestureArenaMember
{
    public virtual void acceptGesture(long pointer) { }

    public virtual void rejectGesture(long pointer) { }
}

public class TapRegion : SingleChildRenderObjectWidget
{
    public virtual bool enabled { get; private set; } = default!;
    public virtual HitTestBehavior behavior { get; private set; } = default!;
    public virtual Action<Gestures.PointerDownEvent>? onTapOutside { get; private set; }
    public virtual Action<Gestures.PointerDownEvent>? onTapInside { get; private set; }
    public virtual Action<Gestures.PointerUpEvent>? onTapUpOutside { get; private set; }
    public virtual Action<Gestures.PointerUpEvent>? onTapUpInside { get; private set; }
    public virtual object? groupId { get; private set; }
    public virtual bool consumeOutsideTaps { get; private set; } = default!;
    public virtual string? debugLabel { get; private set; }

    public TapRegion(
        Key? key = null,
        Widget? child = default!,
        bool enabled = true,
        HitTestBehavior behavior = HitTestBehavior.deferToChild,
        Action<Gestures.PointerDownEvent>? onTapOutside = null,
        Action<Gestures.PointerDownEvent>? onTapInside = null,
        Action<Gestures.PointerUpEvent>? onTapUpOutside = null,
        Action<Gestures.PointerUpEvent>? onTapUpInside = null,
        object? groupId = null,
        bool consumeOutsideTaps = false,
        string? debugLabel = null
    )
        : base(key: key, child: child)
    {
        this.enabled = enabled;
        this.behavior = behavior;
        this.onTapOutside = onTapOutside;
        this.onTapInside = onTapInside;
        this.onTapUpOutside = onTapUpOutside;
        this.onTapUpInside = onTapUpInside;
        this.groupId = groupId;
        this.consumeOutsideTaps = consumeOutsideTaps;
        this.debugLabel = Foundation.ConstantsLibrary.kReleaseMode ? null : debugLabel;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        bool isCurrent = ModalRoute<object>.isCurrentOf(context) ?? true;
        return new RenderTapRegion(
            registry: TapRegionRegistry.maybeOf(context),
            enabled: enabled,
            consumeOutsideTaps: isCurrent && consumeOutsideTaps,
            behavior: behavior,
            onTapOutside: isCurrent ? onTapOutside : null,
            onTapInside: onTapInside,
            onTapUpOutside: isCurrent ? onTapUpOutside : null,
            onTapUpInside: onTapUpInside,
            groupId: groupId,
            debugLabel: debugLabel
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderTapRegion)renderObject;
        bool isCurrent = ModalRoute<object>.isCurrentOf(context) ?? true;
        DartRuntimePrimitives.Ignore(
            (
                (Func<RenderTapRegion>)(
                    () =>
                    {
                        var __cascade = __renderObject;
                        __cascade.registry = TapRegionRegistry.maybeOf(context);
                        __cascade.enabled = enabled;
                        __cascade.consumeOutsideTaps = isCurrent && consumeOutsideTaps;
                        __cascade.behavior = behavior;
                        __cascade.groupId = groupId;
                        __cascade.onTapOutside = isCurrent ? onTapOutside : null;
                        __cascade.onTapInside = onTapInside;
                        __cascade.onTapUpOutside = isCurrent ? onTapUpOutside : null;
                        __cascade.onTapUpInside = onTapUpInside;
                        return __cascade;
                    }
                )
            )()
        );
        if (!Foundation.ConstantsLibrary.kReleaseMode)
        {
            __renderObject.debugLabel = debugLabel;
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new FlagProperty("enabled", value: enabled, ifFalse: "DISABLED", defaultValue: true)
        );
        properties.add(
            new DiagnosticsProperty<HitTestBehavior>(
                "behavior",
                behavior,
                defaultValue: HitTestBehavior.deferToChild
            )
        );
        properties.add(
            new DiagnosticsProperty<object?>("debugLabel", debugLabel, defaultValue: null)
        );
        properties.add(new DiagnosticsProperty<object?>("groupId", groupId, defaultValue: null));
    }
}

public class RenderTapRegion : RenderProxyBoxWithHitTestBehavior
{
    internal virtual bool _isRegistered { get; set; } = false;
    public virtual Action<Gestures.PointerDownEvent>? onTapOutside { get; set; } = default;
    public virtual Action<Gestures.PointerDownEvent>? onTapInside { get; set; } = default;
    public virtual Action<Gestures.PointerUpEvent>? onTapUpOutside { get; set; } = default;
    public virtual Action<Gestures.PointerUpEvent>? onTapUpInside { get; set; } = default;
    public virtual string? debugLabel { get; set; } = default;
    internal virtual bool _enabled { get; set; } = default!;
    internal virtual bool _consumeOutsideTaps { get; set; } = default!;
    internal virtual object? _groupId { get; set; } = default;
    internal virtual TapRegionRegistry? _registry { get; set; } = default;

    public RenderTapRegion(
        TapRegionRegistry? registry = null,
        bool enabled = true,
        bool consumeOutsideTaps = false,
        Action<Gestures.PointerDownEvent>? onTapOutside = null,
        Action<Gestures.PointerDownEvent>? onTapInside = null,
        Action<Gestures.PointerUpEvent>? onTapUpOutside = null,
        Action<Gestures.PointerUpEvent>? onTapUpInside = null,
        HitTestBehavior behavior = HitTestBehavior.deferToChild,
        object? groupId = null,
        string? debugLabel = null
    )
        : base(behavior: behavior)
    {
        this.onTapOutside = onTapOutside;
        this.onTapInside = onTapInside;
        this.onTapUpOutside = onTapUpOutside;
        this.onTapUpInside = onTapUpInside;
        _registry = registry;
        _enabled = enabled;
        _consumeOutsideTaps = consumeOutsideTaps;
        _groupId = groupId;
        this.debugLabel = Foundation.ConstantsLibrary.kReleaseMode ? null : debugLabel;
    }

    public virtual bool enabled
    {
        get => _enabled;
        set
        {
            var __value = value;
            if (_enabled != DartRuntimePrimitives.RequireValue(__value))
            {
                _enabled = DartRuntimePrimitives.RequireValue(__value);
                markNeedsLayout();
            }
        }
    }
    public virtual bool consumeOutsideTaps
    {
        get => _consumeOutsideTaps;
        set
        {
            var __value = value;
            if (_consumeOutsideTaps != DartRuntimePrimitives.RequireValue(__value))
            {
                _consumeOutsideTaps = DartRuntimePrimitives.RequireValue(__value);
                markNeedsLayout();
            }
        }
    }
    public virtual object? groupId
    {
        get => _groupId;
        set
        {
            var __value = value;
            if (!Equals(_groupId, __value))
            {
                if (_isRegistered)
                {
                    _registry!.unregisterTapRegion(this);
                    _isRegistered = false;
                }
                _groupId = __value;
                markNeedsLayout();
            }
        }
    }
    public virtual TapRegionRegistry? registry
    {
        get => _registry;
        set
        {
            var __value = value;
            if (!Equals(_registry, __value))
            {
                if (_isRegistered)
                {
                    _registry!.unregisterTapRegion(this);
                    _isRegistered = false;
                }
                _registry = __value;
                markNeedsLayout();
            }
        }
    }

    public override void layout(Constraints constraints, bool parentUsesSize = false)
    {
        base.layout(constraints, parentUsesSize: parentUsesSize);
        if (_registry is null)
        {
            return;
        }
        if (_isRegistered)
        {
            _registry!.unregisterTapRegion(this);
        }
        bool shouldBeRegistered = _enabled && (_registry is not null);
        if (shouldBeRegistered)
        {
            _registry!.registerTapRegion(this);
        }
        _isRegistered = shouldBeRegistered;
    }

    public override void dispose()
    {
        if (_isRegistered)
        {
            _registry!.unregisterTapRegion(this);
        }
        base.dispose();
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new DiagnosticsProperty<string?>("debugLabel", debugLabel, defaultValue: null)
        );
        properties.add(new DiagnosticsProperty<object?>("groupId", groupId, defaultValue: null));
        properties.add(
            new FlagProperty("enabled", value: enabled, ifFalse: "DISABLED", defaultValue: true)
        );
    }
}

public class TextFieldTapRegion : TapRegion
{
    public TextFieldTapRegion(
        Key? key = null,
        Widget? child = default!,
        bool enabled = true,
        Action<Gestures.PointerDownEvent>? onTapOutside = null,
        Action<Gestures.PointerDownEvent>? onTapInside = null,
        Action<Gestures.PointerUpEvent>? onTapUpOutside = null,
        Action<Gestures.PointerUpEvent>? onTapUpInside = null,
        bool consumeOutsideTaps = false,
        string? debugLabel = null,
        object? groupId = default!
    )
        : base(
            key: key,
            child: child,
            enabled: enabled,
            onTapOutside: onTapOutside,
            onTapInside: onTapInside,
            onTapUpOutside: onTapUpOutside,
            onTapUpInside: onTapUpInside,
            consumeOutsideTaps: consumeOutsideTaps,
            debugLabel: debugLabel,
            groupId: groupId ?? typeof(EditableText)
        ) { }
}
