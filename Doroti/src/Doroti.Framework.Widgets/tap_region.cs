// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/tap_region.dart
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

public static partial class Tap_regionLibrary
{
    internal static bool _kDebugTapRegion = false;
}

public static partial class Tap_regionLibrary
{
    internal static bool _tapRegionDebug(string message, IEnumerable<string>? details = null)
    {
        if (Tap_regionLibrary._kDebugTapRegion)
        {
            global::Doroti.Framework.Foundation.PrintLibrary.debugPrint($"TAP REGION: {message}");
            if (((details is not null) && System.Linq.Enumerable.Any(details)))
            {
                foreach (string detail__927 in details)
                {
                    global::Doroti.Framework.Foundation.PrintLibrary.debugPrint($"    {detail__927}");
                }
            }
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public delegate void TapRegionCallback(global::Doroti.Framework.Gestures.PointerDownEvent @event);

public delegate void TapRegionUpCallback(global::Doroti.Framework.Gestures.PointerUpEvent @event);

public interface TapRegionRegistry
{
    public void registerTapRegion(RenderTapRegion region);
    public void unregisterTapRegion(RenderTapRegion region);
    public static TapRegionRegistry of(BuildContext context)
    {
        TapRegionRegistry? registry__2538 = ((TapRegionRegistry?)(object?)TapRegionRegistry.maybeOf(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((registry__2538 is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("TapRegionRegistry.of() was called with a context that does not contain a TapRegionSurface widget.\n" + "No TapRegionSurface widget ancestor could be found starting from the context that was passed to " + "TapRegionRegistry.of().\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return registry__2538!;
    }
    public static TapRegionRegistry? maybeOf(BuildContext context)
    {
        return ((TapRegionRegistry?)(object?)context.findAncestorRenderObjectOfType<RenderTapRegionSurface>());
    }
}

public class TapRegionSurface : SingleChildRenderObjectWidget
{
    public TapRegionSurface(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!) : base(key: key, child: child)
    {
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new RenderTapRegionSurface());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderProxyBoxWithHitTestBehavior)(object)renderObject;
    }

}

internal delegate void _ClassifiedTapRegions__tap_region();

public class RenderTapRegionSurface : global::Doroti.Framework.Rendering.RenderProxyBoxWithHitTestBehavior, TapRegionRegistry
{
    internal virtual Expando<global::Doroti.Framework.Rendering.BoxHitTestResult> _cachedResults { get; private set; } = new Expando<global::Doroti.Framework.Rendering.BoxHitTestResult>();
    internal virtual HashSet<RenderTapRegion> _registeredRegions { get; private set; } = new HashSet<RenderTapRegion>();
    internal virtual DartMap<object, HashSet<RenderTapRegion>> _groupIdToRegions { get; private set; } = new DartMap<object, HashSet<RenderTapRegion>>();

    public virtual void attach(global::Doroti.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        global::Doroti.Framework.Semantics.SemanticsBinding.instance.addSemanticsActionListener((global::System.Action<SemanticsActionEvent>)this._handleSemanticsAction);
    }

    public virtual void detach()
    {
        global::Doroti.Framework.Semantics.SemanticsBinding.instance.removeSemanticsActionListener((global::System.Action<SemanticsActionEvent>)this._handleSemanticsAction);
        base.detach();
    }

    internal virtual void _handleSemanticsAction(SemanticsActionEvent @event)
    {
        if (((!object.Equals(@event.type, SemanticsAction.tap)) && (!object.Equals(@event.type, SemanticsAction.longPress))))
        {
            return;
        }
        if (!System.Linq.Enumerable.Any(this._registeredRegions))
        {
            return;
        }
        global::Doroti.Ui.Rect? globalRect__10693 = ((global::Doroti.Ui.Rect?)(object?)global::Doroti.Framework.Semantics.SemanticsBinding.instance.getRectOfSemanticsNodeInViewCoordinates(checked((long)@event.viewId), @event.nodeId));
        if ((globalRect__10693 is null))
        {
            return;
        }
        global::Doroti.Ui.Offset globalCenter__10887 = ((global::Doroti.Ui.Offset)(object?)((Offset)((dynamic)DartRuntimePrimitives.RequireValue(globalRect__10693)).center));
        global::Doroti.Ui.Offset localPosition__10938 = ((global::Doroti.Ui.Offset)(object?)globalToLocal(globalCenter__10887));
        var hitResult__10994 = new global::Doroti.Framework.Rendering.BoxHitTestResult();
        if (!hitTest(hitResult__10994, position: localPosition__10938))
        {
            return;
        }
        var (inside__11141, outside__11176) = _classifyRegions(hitResult__10994);
        var syntheticEvent__11235 = new global::Doroti.Framework.Gestures.PointerDownEvent(viewId: checked((long)@event.viewId), position: globalCenter__10887);
        foreach (var region__11331 in outside__11176)
        {
            DartRuntimePrimitives.Assert(() => Tap_regionLibrary._tapRegionDebug($"Calling onTapOutside for {region__11331} (from semantics action)"));
            ((RenderTapRegion)region__11331).onTapOutside?.Invoke(syntheticEvent__11235);
        }
        foreach (var region__11513 in inside__11141)
        {
            DartRuntimePrimitives.Assert(() => Tap_regionLibrary._tapRegionDebug($"Calling onTapInside for {region__11513} (from semantics action)"));
            ((RenderTapRegion)region__11513).onTapInside?.Invoke(syntheticEvent__11235);
        }
    }

    public virtual void registerTapRegion(RenderTapRegion region)
    {
        DartRuntimePrimitives.Assert(() => Tap_regionLibrary._tapRegionDebug($"Region {region} registered."));
        DartRuntimePrimitives.Assert(() => !this._registeredRegions.Contains(region));
        this._registeredRegions.Add(region);
        if ((((RenderTapRegion)region).groupId is not null))
        {
            this._groupIdToRegions.putIfAbsent(((RenderTapRegion)region).groupId, () => new HashSet<RenderTapRegion>());
            this._groupIdToRegions.GetValueOrDefault(DartRuntimePrimitives.RequireReference(((RenderTapRegion)region).groupId))!.Add(region);
        }
    }

    public virtual void unregisterTapRegion(RenderTapRegion region)
    {
        DartRuntimePrimitives.Assert(() => Tap_regionLibrary._tapRegionDebug($"Region {region} unregistered."));
        DartRuntimePrimitives.Assert(() => this._registeredRegions.Contains(region));
        this._registeredRegions.Remove(region);
        if ((((RenderTapRegion)region).groupId is not null))
        {
            DartRuntimePrimitives.Assert(() => this._groupIdToRegions.ContainsKey(((RenderTapRegion)region).groupId));
            this._groupIdToRegions.GetValueOrDefault(DartRuntimePrimitives.RequireReference(((RenderTapRegion)region).groupId))!.Remove(region);
            if (!System.Linq.Enumerable.Any(this._groupIdToRegions.GetValueOrDefault(DartRuntimePrimitives.RequireReference(((RenderTapRegion)region).groupId))!))
            {
                this._groupIdToRegions.remove(((RenderTapRegion)region).groupId);
            }
        }
    }

    public override bool hitTest(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        if (!this.size.contains(position))
        {
            return false;
        }
        bool hitTarget__12705 = (hitTestChildren(result, position: position) || hitTestSelf(position));
        if (hitTarget__12705)
        {
            var entry__12821 = new global::Doroti.Framework.Rendering.BoxHitTestEntry(this, position);
            this._cachedResults[entry__12821] = result;
            result.add(entry__12821);
        }
        return hitTarget__12705;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual (IEnumerable<RenderTapRegion> inside, IEnumerable<RenderTapRegion> outside) _classifyRegions(global::Doroti.Framework.Rendering.BoxHitTestResult result)
    {
        IEnumerable<RenderTapRegion> hitRegions__13310 = ((IEnumerable<RenderTapRegion>)(object?)_getRegionsHit(this._registeredRegions, result.path.Cast<global::Doroti.Framework.Gestures.HitTestEntry<global::Doroti.Framework.Gestures.HitTestTarget>>()).cast<RenderTapRegion>());
        DartRuntimePrimitives.Assert(() => Tap_regionLibrary._tapRegionDebug($"Tap event hit {hitRegions__13310.Count()} descendants."));
        var insideRegions__13506 = new HashSet<RenderTapRegion>();
        return (inside: insideRegions__13506, outside: this._registeredRegions.where(((r) => !insideRegions__13506.Contains(r))).ToList());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void handleEvent(global::Doroti.Framework.Gestures.PointerEvent @event, global::Doroti.Framework.Gestures.HitTestEntry<global::Doroti.Framework.Gestures.HitTestTarget> entry)
    {
        DartRuntimePrimitives.Assert(() => debugHandleEvent(@event, entry));
        DartRuntimePrimitives.Assert(() =>
            {
                foreach (RenderTapRegion region__14157 in this._registeredRegions)
                {
                    if (!((RenderTapRegion)region__14157).enabled)
                    {
                        return false;
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            }, () => (object?)"A RenderTapRegion was registered when it was disabled.");
        if (((@event is not global::Doroti.Framework.Gestures.PointerDownEvent) && (@event is not global::Doroti.Framework.Gestures.PointerUpEvent)))
        {
            return;
        }
        if (!System.Linq.Enumerable.Any(this._registeredRegions))
        {
            DartRuntimePrimitives.Assert(() => Tap_regionLibrary._tapRegionDebug("Ignored tap event because no regions are registered."));
            return;
        }
        global::Doroti.Framework.Rendering.BoxHitTestResult? result__14611 = this._cachedResults[entry];
        if ((result__14611 is null))
        {
            DartRuntimePrimitives.Assert(() => Tap_regionLibrary._tapRegionDebug("Ignored tap event because no surface descendants were hit."));
            return;
        }
        var (inside__14822, outside__14857) = _classifyRegions(result__14611);
        var consumeOutsideTaps__14911 = false;
        foreach (var region__14954 in outside__14857)
        {
            if ((@event is global::Doroti.Framework.Gestures.PointerDownEvent))
            {
                global::Doroti.Framework.Gestures.PointerDownEvent @event__as14985 = (global::Doroti.Framework.Gestures.PointerDownEvent)@event;
                DartRuntimePrimitives.Assert(() => Tap_regionLibrary._tapRegionDebug($"Calling onTapOutside for {region__14954}"));
                ((RenderTapRegion)region__14954).onTapOutside?.Invoke(((global::Doroti.Framework.Gestures.PointerDownEvent)@event__as14985));
            }
            else
            {
                if ((@event is global::Doroti.Framework.Gestures.PointerUpEvent))
                {
                    global::Doroti.Framework.Gestures.PointerUpEvent @event__as15142 = (global::Doroti.Framework.Gestures.PointerUpEvent)@event;
                    DartRuntimePrimitives.Assert(() => Tap_regionLibrary._tapRegionDebug($"Calling onTapUpOutside for {region__14954}"));
                    ((RenderTapRegion)region__14954).onTapUpOutside?.Invoke(((global::Doroti.Framework.Gestures.PointerUpEvent)@event__as15142));
                }
            }
            if (((RenderTapRegion)region__14954).consumeOutsideTaps)
            {
                DartRuntimePrimitives.Assert(() => Tap_regionLibrary._tapRegionDebug($"Stopping tap propagation for {region__14954} (and all of {((RenderTapRegion)region__14954).groupId})"));
                consumeOutsideTaps__14911 = true;
            }
        }
        foreach (var region__15521 in inside__14822)
        {
            if ((@event is global::Doroti.Framework.Gestures.PointerDownEvent))
            {
                global::Doroti.Framework.Gestures.PointerDownEvent @event__as15551 = (global::Doroti.Framework.Gestures.PointerDownEvent)@event;
                DartRuntimePrimitives.Assert(() => Tap_regionLibrary._tapRegionDebug($"Calling onTapInside for {region__15521}"));
                ((RenderTapRegion)region__15521).onTapInside?.Invoke(((global::Doroti.Framework.Gestures.PointerDownEvent)@event__as15551));
            }
            else
            {
                if ((@event is global::Doroti.Framework.Gestures.PointerUpEvent))
                {
                    global::Doroti.Framework.Gestures.PointerUpEvent @event__as15706 = (global::Doroti.Framework.Gestures.PointerUpEvent)@event;
                    DartRuntimePrimitives.Assert(() => Tap_regionLibrary._tapRegionDebug($"Calling onTapUpInside for {region__15521}"));
                    ((RenderTapRegion)region__15521).onTapUpInside?.Invoke(((global::Doroti.Framework.Gestures.PointerUpEvent)@event__as15706));
                }
            }
        }
        if ((consumeOutsideTaps__14911 && (@event is global::Doroti.Framework.Gestures.PointerDownEvent)))
        {
            global::Doroti.Framework.Gestures.PointerDownEvent @event__as16104 = (global::Doroti.Framework.Gestures.PointerDownEvent)@event;
            global::Doroti.Framework.Gestures.GestureBinding.instance.gestureArena.add(((global::Doroti.Framework.Gestures.PointerDownEvent)@event__as16104).pointer, new _DummyTapRecognizer__tap_region()).resolve(global::Doroti.Framework.Gestures.GestureDisposition.accepted);
        }
    }

    internal virtual HashSet<global::Doroti.Framework.Gestures.HitTestTarget> _getRegionsHit(HashSet<RenderTapRegion> detectors, IEnumerable<global::Doroti.Framework.Gestures.HitTestEntry<global::Doroti.Framework.Gestures.HitTestTarget>> hitTestPath)
    {
        return new HashSet<global::Doroti.Framework.Gestures.HitTestTarget>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DummyTapRecognizer__tap_region : global::Doroti.Framework.Gestures.GestureArenaMember
{
    public virtual void acceptGesture(long pointer)
    {
    }

    public virtual void rejectGesture(long pointer)
    {
    }

}

public class TapRegion : SingleChildRenderObjectWidget
{
    public virtual bool enabled { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.HitTestBehavior behavior { get; private set; } = default!;
    public virtual global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>? onTapOutside { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>? onTapInside { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.PointerUpEvent>? onTapUpOutside { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.PointerUpEvent>? onTapUpInside { get; private set; }
    public virtual object? groupId { get; private set; }
    public virtual bool consumeOutsideTaps { get; private set; } = default!;
    public virtual string? debugLabel { get; private set; }

    public TapRegion(global::Doroti.Framework.Foundation.Key? key = null, Widget? child = default!, bool enabled = true, global::Doroti.Framework.Rendering.HitTestBehavior behavior = global::Doroti.Framework.Rendering.HitTestBehavior.deferToChild, global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>? onTapOutside = null, global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>? onTapInside = null, global::System.Action<global::Doroti.Framework.Gestures.PointerUpEvent>? onTapUpOutside = null, global::System.Action<global::Doroti.Framework.Gestures.PointerUpEvent>? onTapUpInside = null, object? groupId = null, bool consumeOutsideTaps = false, string? debugLabel = null) : base(key: key, child: child)
    {
        this.enabled = enabled;
        this.behavior = behavior;
        this.onTapOutside = onTapOutside;
        this.onTapInside = onTapInside;
        this.onTapUpOutside = onTapUpOutside;
        this.onTapUpInside = onTapUpInside;
        this.groupId = groupId;
        this.consumeOutsideTaps = consumeOutsideTaps;
        this.debugLabel = (global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode ? null : debugLabel);
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        bool isCurrent__23161 = (ModalRoute<object>.isCurrentOf(context) ?? true);
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new RenderTapRegion(registry: TapRegionRegistry.maybeOf(context), enabled: this.enabled, consumeOutsideTaps: (isCurrent__23161 && this.consumeOutsideTaps), behavior: this.behavior, onTapOutside: ((global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>)(isCurrent__23161 ? this.onTapOutside : null)), onTapInside: (global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>?)this.onTapInside, onTapUpOutside: ((global::System.Action<global::Doroti.Framework.Gestures.PointerUpEvent>)(isCurrent__23161 ? this.onTapUpOutside : null)), onTapUpInside: (global::System.Action<global::Doroti.Framework.Gestures.PointerUpEvent>?)this.onTapUpInside, groupId: this.groupId, debugLabel: this.debugLabel));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (RenderTapRegion)(object)renderObject;
        bool isCurrent__23765 = (ModalRoute<object>.isCurrentOf(context) ?? true);
        DartRuntimePrimitives.Ignore(((Func<RenderTapRegion>)(() =>
{            var __cascade = __renderObject;
            __cascade.registry = TapRegionRegistry.maybeOf(context);
            __cascade.enabled = this.enabled;
            __cascade.consumeOutsideTaps = (isCurrent__23765 && this.consumeOutsideTaps);
            __cascade.behavior = this.behavior;
            __cascade.groupId = this.groupId;
            __cascade.onTapOutside = ((global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>)(isCurrent__23765 ? this.onTapOutside : null));
            __cascade.onTapInside = this.onTapInside;
            __cascade.onTapUpOutside = ((global::System.Action<global::Doroti.Framework.Gestures.PointerUpEvent>)(isCurrent__23765 ? this.onTapUpOutside : null));
            __cascade.onTapUpInside = this.onTapUpInside;
            return __cascade;        }))());
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            __renderObject.debugLabel = this.debugLabel;
        }
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("enabled", value: this.enabled, ifFalse: "DISABLED", defaultValue: true));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.HitTestBehavior>("behavior", this.behavior, defaultValue: global::Doroti.Framework.Rendering.HitTestBehavior.deferToChild));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<object?>("debugLabel", this.debugLabel, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<object?>("groupId", this.groupId, defaultValue: null));
    }

}

public class RenderTapRegion : global::Doroti.Framework.Rendering.RenderProxyBoxWithHitTestBehavior
{
    internal virtual bool _isRegistered { get; set; } = false;
    public virtual global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>? onTapOutside { get; set; } = default;
    public virtual global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>? onTapInside { get; set; } = default;
    public virtual global::System.Action<global::Doroti.Framework.Gestures.PointerUpEvent>? onTapUpOutside { get; set; } = default;
    public virtual global::System.Action<global::Doroti.Framework.Gestures.PointerUpEvent>? onTapUpInside { get; set; } = default;
    public virtual string? debugLabel { get; set; } = default;
    internal virtual bool _enabled { get; set; } = default!;
    internal virtual bool _consumeOutsideTaps { get; set; } = default!;
    internal virtual object? _groupId { get; set; } = default;
    internal virtual TapRegionRegistry? _registry { get; set; } = default;

    public RenderTapRegion(TapRegionRegistry? registry = null, bool enabled = true, bool consumeOutsideTaps = false, global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>? onTapOutside = null, global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>? onTapInside = null, global::System.Action<global::Doroti.Framework.Gestures.PointerUpEvent>? onTapUpOutside = null, global::System.Action<global::Doroti.Framework.Gestures.PointerUpEvent>? onTapUpInside = null, global::Doroti.Framework.Rendering.HitTestBehavior behavior = global::Doroti.Framework.Rendering.HitTestBehavior.deferToChild, object? groupId = null, string? debugLabel = null) : base(behavior: behavior)
    {
        this.onTapOutside = onTapOutside;
        this.onTapInside = onTapInside;
        this.onTapUpOutside = onTapUpOutside;
        this.onTapUpInside = onTapUpInside;
        this._registry = registry;
        this._enabled = enabled;
        this._consumeOutsideTaps = consumeOutsideTaps;
        this._groupId = groupId;
        this.debugLabel = (global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode ? null : debugLabel);
    }

    public virtual bool enabled
    {
        get => this._enabled;
        set
        {
            var __value = value;
            if ((this._enabled != DartRuntimePrimitives.RequireValue(__value)))
            {
                _enabled = DartRuntimePrimitives.RequireValue(__value);
                markNeedsLayout();
            }
        }
    }
    public virtual bool consumeOutsideTaps
    {
        get => this._consumeOutsideTaps;
        set
        {
            var __value = value;
            if ((this._consumeOutsideTaps != DartRuntimePrimitives.RequireValue(__value)))
            {
                _consumeOutsideTaps = DartRuntimePrimitives.RequireValue(__value);
                markNeedsLayout();
            }
        }
    }
    public virtual object? groupId
    {
        get => this._groupId;
        set
        {
            var __value = value;
            if ((!object.Equals(this._groupId, __value)))
            {
                if (this._isRegistered)
                {
                    this._registry!.unregisterTapRegion(this);
                    _isRegistered = false;
                }
                _groupId = __value;
                markNeedsLayout();
            }
        }
    }
    public virtual TapRegionRegistry? registry
    {
        get => this._registry;
        set
        {
            var __value = value;
            if ((!object.Equals(this._registry, __value)))
            {
                if (this._isRegistered)
                {
                    this._registry!.unregisterTapRegion(this);
                    _isRegistered = false;
                }
                _registry = __value;
                markNeedsLayout();
            }
        }
    }
    public override void layout(global::Doroti.Framework.Rendering.Constraints constraints, bool parentUsesSize = false)
    {
        base.layout(constraints, parentUsesSize: parentUsesSize);
        if ((this._registry is null))
        {
            return;
        }
        if (this._isRegistered)
        {
            this._registry!.unregisterTapRegion(this);
        }
        bool shouldBeRegistered__31401 = (this._enabled && (this._registry is not null));
        if (shouldBeRegistered__31401)
        {
            this._registry!.registerTapRegion(this);
        }
        _isRegistered = shouldBeRegistered__31401;
    }

    public override void dispose()
    {
        if (this._isRegistered)
        {
            this._registry!.unregisterTapRegion(this);
        }
        base.dispose();
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<string?>("debugLabel", this.debugLabel, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<object?>("groupId", this.groupId, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("enabled", value: this.enabled, ifFalse: "DISABLED", defaultValue: true));
    }

}

public class TextFieldTapRegion : TapRegion
{
    public TextFieldTapRegion(global::Doroti.Framework.Foundation.Key? key = null, Widget? child = default!, bool enabled = true, global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>? onTapOutside = null, global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>? onTapInside = null, global::System.Action<global::Doroti.Framework.Gestures.PointerUpEvent>? onTapUpOutside = null, global::System.Action<global::Doroti.Framework.Gestures.PointerUpEvent>? onTapUpInside = null, bool consumeOutsideTaps = false, string? debugLabel = null, object? groupId = default!) : base(key: key, child: child, enabled: enabled, onTapOutside: onTapOutside, onTapInside: onTapInside, onTapUpOutside: onTapUpOutside, onTapUpInside: onTapUpInside, consumeOutsideTaps: consumeOutsideTaps, debugLabel: debugLabel, groupId: groupId ?? typeof(EditableText))
    {
    }

}

