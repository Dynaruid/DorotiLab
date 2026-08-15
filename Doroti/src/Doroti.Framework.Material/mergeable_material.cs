// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/mergeable_material.dart
using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public abstract class MergeableMaterialItem
{
    public virtual global::Doroti.Generated.Framework.Foundation.LocalKey key { get; private set; } = default!;

    protected MergeableMaterialItem(global::Doroti.Generated.Framework.Foundation.LocalKey key)
    {
        this.key = key;
    }

}

public class MaterialSlice : MergeableMaterialItem
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual Color? color { get; private set; }

    public MaterialSlice(global::Doroti.Generated.Framework.Foundation.LocalKey key, global::Doroti.Generated.Framework.Widgets.Widget child, Color? color = null) : base(key)
    {
        this.child = child;
        this.color = color;
    }

    public override string ToString()
    {
        return $"MergeableSlice(key: {this.key}, child: {this.child}, color: {this.color})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class MaterialGap : MergeableMaterialItem
{
    public virtual double size { get; private set; } = default!;

    public MaterialGap(global::Doroti.Generated.Framework.Foundation.LocalKey key, double size = 16.0) : base(key)
    {
        this.size = size;
    }

    public override string ToString()
    {
        return $"MaterialGap(key: {this.key}, child: {this.size})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class MergeableMaterial : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual List<MergeableMaterialItem> children { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.Axis mainAxis { get; private set; } = default!;
    public virtual double elevation { get; private set; } = default!;
    public virtual bool hasDividers { get; private set; } = default!;
    public virtual Color? dividerColor { get; private set; }

    public MergeableMaterial(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.Axis mainAxis = global::Doroti.Generated.Framework.Painting.Axis.vertical, double elevation = 2, bool hasDividers = false, List<MergeableMaterialItem> children = default!, Color? dividerColor = null) : base(key: key)
    {
        List<MergeableMaterialItem> __children = children ?? new List<MergeableMaterialItem>();
        this.mainAxis = mainAxis;
        this.elevation = elevation;
        this.hasDividers = hasDividers;
        this.children = __children;
        this.dividerColor = dividerColor;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Painting.Axis>("mainAxis", this.mainAxis));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("elevation", this.elevation));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _MergeableMaterialState__mergeable_material());
}

internal class _AnimationTuple__mergeable_material
{
    public virtual global::Doroti.Generated.Framework.Animation.AnimationController controller { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation startAnimation { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation endAnimation { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation gapAnimation { get; private set; } = default!;
    public virtual double gapStart { get; set; } = 0.0;

    internal _AnimationTuple__mergeable_material(global::Doroti.Generated.Framework.Animation.AnimationController controller, global::Doroti.Generated.Framework.Animation.CurvedAnimation startAnimation, global::Doroti.Generated.Framework.Animation.CurvedAnimation endAnimation, global::Doroti.Generated.Framework.Animation.CurvedAnimation gapAnimation)
    {
        this.controller = controller;
        this.startAnimation = startAnimation;
        this.endAnimation = endAnimation;
        this.gapAnimation = gapAnimation;
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugMaybeDispatchCreated("material", "_AnimationTuple", this));
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        this.controller.dispose();
        this.startAnimation.dispose();
        this.endAnimation.dispose();
        this.gapAnimation.dispose();
    }

}

internal class _MergeableMaterialState__mergeable_material : global::Doroti.Generated.Framework.Widgets.State<MergeableMaterial>, global::Doroti.Generated.Framework.Widgets.TickerProviderStateMixin<MergeableMaterial>
{
    internal virtual List<MergeableMaterialItem> _children { get; set; } = default!;
    internal virtual DartMap<global::Doroti.Generated.Framework.Foundation.LocalKey, _AnimationTuple__mergeable_material?> _animationTuples { get; private set; } = new DartMap<global::Doroti.Generated.Framework.Foundation.LocalKey, _AnimationTuple__mergeable_material?>();
    public virtual HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _children = new List<MergeableMaterialItem>(DartRuntimePrimitives.ConvertEnumerable<MergeableMaterialItem>(((MergeableMaterial)this.widget).children));
        for (var i__5742 = 0L; (i__5742 < checked((long)(this._children.Count))); i__5742 += 1L)
        {
            MergeableMaterialItem child__5815 = this._children[(int)(i__5742)];
            if ((child__5815 is MaterialGap))
            {
                MaterialGap child__5815__as5847 = (MaterialGap)child__5815;
                _initGap(((MaterialGap)child__5815__as5847));
                this._animationTuples[child__5815__as5847.key]!.controller.value = 1.0;
            }
        }
        DartRuntimePrimitives.Assert(() => _debugGapsAreValid(this._children));
    }

    internal virtual void _initGap(MaterialGap gap)
    {
        var controller__6098 = new global::Doroti.Generated.Framework.Animation.AnimationController(duration: ThemeLibrary.kThemeAnimationDuration, vsync: this);
        var startAnimation__6191 = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: controller__6098, curve: global::Doroti.Generated.Framework.Animation.Curves.fastOutSlowIn);
        var endAnimation__6284 = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: controller__6098, curve: global::Doroti.Generated.Framework.Animation.Curves.fastOutSlowIn);
        var gapAnimation__6375 = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: controller__6098, curve: global::Doroti.Generated.Framework.Animation.Curves.fastOutSlowIn);
        controller__6098.addListener(() => this._handleTick());
        this._animationTuples[gap.key] = new _AnimationTuple__mergeable_material(controller: controller__6098, startAnimation: startAnimation__6191, endAnimation: endAnimation__6284, gapAnimation: gapAnimation__6375);
    }

    public override void dispose()
    {
        foreach (MergeableMaterialItem child__6764 in this._children)
        {
            if ((child__6764 is MaterialGap))
            {
                MaterialGap child__6764__as6796 = (MaterialGap)child__6764;
                this._animationTuples.GetValueOrDefault(((MaterialGap)child__6764__as6796).key)!.dispose();
            }
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Generated.Framework.Scheduler.Ticker ticker__18989 in this._tickers!)
                    {
                        if (((global::Doroti.Generated.Framework.Scheduler.Ticker)ticker__18989).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker__18989.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual void _handleTick()
    {
        setState(((global::System.Action)(() => {
})));
    }

    internal virtual bool _debugHasConsecutiveGaps(List<MergeableMaterialItem> children)
    {
        for (var i__7122 = 0L; (i__7122 < (checked((long)(((MergeableMaterial)this.widget).children.Count)) - 1L)); i__7122 += 1L)
        {
            if (((((MergeableMaterial)this.widget).children[(int)(i__7122)] is MaterialGap) && (((MergeableMaterial)this.widget).children[(int)((i__7122 + 1L))] is MaterialGap)))
            {
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _debugGapsAreValid(List<MergeableMaterialItem> children)
    {
        if (_debugHasConsecutiveGaps(children))
        {
            return false;
        }
        if (System.Linq.Enumerable.Any(children))
        {
            if (((children.First() is MaterialGap) || (children.Last() is MaterialGap)))
            {
                return false;
            }
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _insertChild(long index, MergeableMaterialItem child)
    {
        this._children.Insert(checked((int)index), child);
        if ((child is MaterialGap))
        {
            MaterialGap child__as7812 = (MaterialGap)child;
            _initGap(((MaterialGap)child__as7812));
        }
    }

    internal virtual void _removeChild(long index)
    {
        MergeableMaterialItem child__7935 = this._children.removeAt(index);
        if ((child__7935 is MaterialGap))
        {
            MaterialGap child__7935__as7979 = (MaterialGap)child__7935;
            this._animationTuples.GetValueOrDefault(((MaterialGap)child__7935__as7979).key)!.dispose();
            this._animationTuples[child__7935__as7979.key] = null;
        }
    }

    internal virtual bool _isClosingGap(long index)
    {
        if (((index < (checked((long)(this._children.Count)) - 1L)) && (this._children[(int)(index)] is MaterialGap)))
        {
            return (object.Equals(this._animationTuples.GetValueOrDefault(this._children[(int)(index)].key)!.controller.status, global::Doroti.Generated.Framework.Animation.AnimationStatus.reverse));
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _removeEmptyGaps()
    {
        for (long j__8381 = (checked((long)(this._children.Count)) - 1L); (j__8381 >= 0L); j__8381 -= 1L)
        {
            if (((this._children[(int)(j__8381)] is MaterialGap) && this._animationTuples.GetValueOrDefault(this._children[(int)(j__8381)].key)!.controller.isDismissed))
            {
                _removeChild(j__8381);
            }
        }
    }

    public override void didUpdateWidget(MergeableMaterial oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        HashSet<global::Doroti.Generated.Framework.Foundation.LocalKey> oldKeys__8711 = ((MergeableMaterial)oldWidget).children.map<MergeableMaterialItem, global::Doroti.Generated.Framework.Foundation.LocalKey>(((child) => ((MergeableMaterialItem)child).key)).toSet();
        HashSet<global::Doroti.Generated.Framework.Foundation.LocalKey> newKeys__8849 = ((MergeableMaterial)this.widget).children.map<MergeableMaterialItem, global::Doroti.Generated.Framework.Foundation.LocalKey>(((child) => ((MergeableMaterialItem)child).key)).toSet();
        HashSet<global::Doroti.Generated.Framework.Foundation.LocalKey> newOnly__8984 = newKeys__8849.difference<global::Doroti.Generated.Framework.Foundation.LocalKey>(oldKeys__8711);
        HashSet<global::Doroti.Generated.Framework.Foundation.LocalKey> oldOnly__9047 = oldKeys__8711.difference<global::Doroti.Generated.Framework.Foundation.LocalKey>(newKeys__8849);
        List<MergeableMaterialItem> newChildren__9125 = ((MergeableMaterial)this.widget).children.ToList();
        var i__9164 = 0L;
        var j__9179 = 0L;
        DartRuntimePrimitives.Assert(() => _debugGapsAreValid(newChildren__9125));
        _removeEmptyGaps();
        while (((i__9164 < checked((long)(newChildren__9125.Count))) && (j__9179 < checked((long)(this._children.Count)))))
        {
            if ((newOnly__8984.Contains(newChildren__9125[(int)(i__9164)].key) || oldOnly__9047.Contains(this._children[(int)(j__9179)].key)))
            {
                var startNew__9421 = i__9164;
                var startOld__9449 = j__9179;
                while (newOnly__8984.Contains(newChildren__9125[(int)(i__9164)].key))
                {
                    i__9164 += 1L;
                }
                while ((oldOnly__9047.Contains(this._children[(int)(j__9179)].key) || _isClosingGap(j__9179)))
                {
                    j__9179 += 1L;
                }
                long newLength__9720 = (i__9164 - startNew__9421);
                long oldLength__9764 = (j__9179 - startOld__9449);
                if ((newLength__9720 > 0L))
                {
                    if (((oldLength__9764 > 1L) || ((oldLength__9764 == 1L) && (this._children[(int)(startOld__9449)] is MaterialSlice))))
                    {
                        if (((newLength__9720 == 1L) && (newChildren__9125[(int)(startNew__9421)] is MaterialGap)))
                        {
                            var gapSizeSum__10064 = 0.0;
                            while ((startOld__9449 < j__9179))
                            {
                                MergeableMaterialItem child__10164 = this._children[(int)(startOld__9449)];
                                if ((child__10164 is MaterialGap))
                                {
                                    MaterialGap child__10164__as10213 = (MaterialGap)child__10164;
                                    MaterialGap gap__10273 = ((MaterialGap)child__10164__as10213);
                                    gapSizeSum__10064 += ((MaterialGap)gap__10273).size;
                                }
                                _removeChild(startOld__9449);
                                j__9179 -= 1L;
                            }
                            _insertChild(startOld__9449, newChildren__9125[(int)(startNew__9421)]);
                            DartRuntimePrimitives.Ignore(((Func<_AnimationTuple__mergeable_material>)(() =>
{            var __cascade = this._animationTuples.GetValueOrDefault(newChildren__9125[(int)(startNew__9421)].key)!;
            __cascade.gapStart = gapSizeSum__10064;
            __cascade.controller.forward();
            return __cascade;        }))());
                            j__9179 += 1L;
                        }
                        else
                        {
                            for (var k__10762 = 0L; (k__10762 < oldLength__9764); k__10762 += 1L)
                            {
                                _removeChild(startOld__9449);
                            }
                            for (var k__10873 = 0L; (k__10873 < newLength__9720); k__10873 += 1L)
                            {
                                _insertChild((startOld__9449 + k__10873), newChildren__9125[(int)((startNew__9421 + k__10873))]);
                            }
                            j__9179 += (newLength__9720 - oldLength__9764);
                        }
                    }
                    else
                    {
                        if ((oldLength__9764 == 1L))
                        {
                            if ((((newLength__9720 == 1L) && (newChildren__9125[(int)(startNew__9421)] is MaterialGap)) && (object.Equals(this._children[(int)(startOld__9449)].key, newChildren__9125[(int)(startNew__9421)].key))))
                            {
                                this._animationTuples.GetValueOrDefault(newChildren__9125[(int)(startNew__9421)].key)!.controller.forward();
                            }
                            else
                            {
                                double gapSize__11427 = _getGapSize(startOld__9449);
                                _removeChild(startOld__9449);
                                for (var k__11523 = 0L; (k__11523 < newLength__9720); k__11523 += 1L)
                                {
                                    _insertChild((startOld__9449 + k__11523), newChildren__9125[(int)((startNew__9421 + k__11523))]);
                                }
                                j__9179 += (newLength__9720 - 1L);
                                var gapSizeSum__11695 = 0.0;
                                for (var k__11737 = startNew__9421; (k__11737 < i__9164); k__11737 += 1L)
                                {
                                    MergeableMaterialItem newChild__11812 = newChildren__9125[(int)(k__11737)];
                                    if ((newChild__11812 is MaterialGap))
                                    {
                                        MaterialGap newChild__11812__as11859 = (MaterialGap)newChild__11812;
                                        gapSizeSum__11695 += ((MaterialGap)((MaterialGap)newChild__11812__as11859)).size;
                                    }
                                }
                                for (var k__12121 = startNew__9421; (k__12121 < i__9164); k__12121 += 1L)
                                {
                                    MergeableMaterialItem newChild__12196 = newChildren__9125[(int)(k__12121)];
                                    if ((newChild__12196 is MaterialGap))
                                    {
                                        MaterialGap newChild__12196__as12243 = (MaterialGap)newChild__12196;
                                        this._animationTuples[newChild__12196__as12243.key]!.gapStart = ((gapSize__11427 * ((MaterialGap)((MaterialGap)newChild__12196__as12243)).size) / gapSizeSum__11695);
                                        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Animation.AnimationController>)(() =>
{            var __cascade = this._animationTuples.GetValueOrDefault(((MaterialGap)newChild__12196__as12243).key)!.controller;
            __cascade.value = 0.0;
            __cascade.forward();
            return __cascade;        }))());
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (var k__12611 = 0L; (k__12611 < newLength__9720); k__12611 += 1L)
                            {
                                MergeableMaterialItem newChild__12685 = newChildren__9125[(int)((startNew__9421 + k__12611))];
                                _insertChild((startOld__9449 + k__12611), newChild__12685);
                                if ((newChild__12685 is MaterialGap))
                                {
                                    MaterialGap newChild__12685__as12795 = (MaterialGap)newChild__12685;
                                    this._animationTuples.GetValueOrDefault(((MaterialGap)newChild__12685__as12795).key)!.controller.forward();
                                }
                            }
                            j__9179 += newLength__9720;
                        }
                    }
                }
                else
                {
                    if (((oldLength__9764 > 1L) || ((oldLength__9764 == 1L) && (this._children[(int)(startOld__9449)] is MaterialSlice))))
                    {
                        var gapSizeSum__13166 = 0.0;
                        while ((startOld__9449 < j__9179))
                        {
                            MergeableMaterialItem child__13262 = this._children[(int)(startOld__9449)];
                            if ((child__13262 is MaterialGap))
                            {
                                MaterialGap child__13262__as13309 = (MaterialGap)child__13262;
                                gapSizeSum__13166 += ((MaterialGap)((MaterialGap)child__13262__as13309)).size;
                            }
                            _removeChild(startOld__9449);
                            j__9179 -= 1L;
                        }
                        if ((gapSizeSum__13166 != 0.0))
                        {
                            var gap__13524 = new MaterialGap(key: new global::Doroti.Generated.Framework.Foundation.UniqueKey(), size: gapSizeSum__13166);
                            _insertChild(startOld__9449, gap__13524);
                            this._animationTuples[gap__13524.key]!.gapStart = 0.0;
                            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Animation.AnimationController>)(() =>
{            var __cascade = this._animationTuples.GetValueOrDefault(gap__13524.key)!.controller;
            __cascade.value = 1.0;
            __cascade.reverse();
            return __cascade;        }))());
                            j__9179 += 1L;
                        }
                    }
                    else
                    {
                        if ((oldLength__9764 == 1L))
                        {
                            var gap__13911 = ((MaterialGap?)(object?)this._children[(int)(startOld__9449)])!;
                            this._animationTuples[gap__13911.key]!.gapStart = 0.0;
                            this._animationTuples.GetValueOrDefault(gap__13911.key)!.controller.reverse();
                        }
                    }
                }
            }
            else
            {
                if ((((this._children[(int)(j__9179)] is MaterialGap)) == ((newChildren__9125[(int)(i__9164)] is MaterialGap))))
                {
                    this._children[(int)(j__9179)] = newChildren__9125[(int)(i__9164)];
                    i__9164 += 1L;
                    j__9179 += 1L;
                }
                else
                {
                    DartRuntimePrimitives.Assert(() => (this._children[(int)(j__9179)] is MaterialGap));
                    j__9179 += 1L;
                }
            }
        }
        while ((j__9179 < checked((long)(this._children.Count))))
        {
            _removeChild(j__9179);
        }
        while ((i__9164 < checked((long)(newChildren__9125.Count))))
        {
            MergeableMaterialItem newChild__14719 = newChildren__9125[(int)(i__9164)];
            _insertChild(j__9179, newChild__14719);
            if ((newChild__14719 is MaterialGap))
            {
                MaterialGap newChild__14719__as14790 = (MaterialGap)newChild__14719;
                this._animationTuples.GetValueOrDefault(((MaterialGap)newChild__14719__as14790).key)!.controller.forward();
            }
            i__9164 += 1L;
            j__9179 += 1L;
        }
    }

    internal virtual global::Doroti.Generated.Framework.Painting.BorderRadius _borderRadius(long index, bool start, bool end)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(MaterialLibrary.kMaterialEdges.GetValueOrDefault(MaterialType.card)!.topLeft, MaterialLibrary.kMaterialEdges.GetValueOrDefault(MaterialType.card)!.topRight)));
        DartRuntimePrimitives.Assert(() => (object.Equals(MaterialLibrary.kMaterialEdges.GetValueOrDefault(MaterialType.card)!.topLeft, MaterialLibrary.kMaterialEdges.GetValueOrDefault(MaterialType.card)!.bottomLeft)));
        DartRuntimePrimitives.Assert(() => (object.Equals(MaterialLibrary.kMaterialEdges.GetValueOrDefault(MaterialType.card)!.topLeft, MaterialLibrary.kMaterialEdges.GetValueOrDefault(MaterialType.card)!.bottomRight)));
        global::Doroti.Ui.Radius cardRadius__15361 = ((global::Doroti.Ui.Radius)(object?)MaterialLibrary.kMaterialEdges.GetValueOrDefault(MaterialType.card)!.topLeft);
        global::Doroti.Ui.Radius startRadius__15430 = ((global::Doroti.Ui.Radius)(object?)Radius.zero);
        global::Doroti.Ui.Radius endRadius__15468 = ((global::Doroti.Ui.Radius)(object?)Radius.zero);
        if (((index > 0L) && (this._children[(int)((index - 1L))] is MaterialGap)))
        {
            startRadius__15430 = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Radius.lerp(Radius.zero, cardRadius__15361, this._animationTuples.GetValueOrDefault(this._children[(int)((index - 1L))].key)!.startAnimation.value));
        }
        if (((index < (checked((long)(this._children.Count)) - 2L)) && (this._children[(int)((index + 1L))] is MaterialGap)))
        {
            endRadius__15468 = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Radius.lerp(Radius.zero, cardRadius__15361, this._animationTuples.GetValueOrDefault(this._children[(int)((index + 1L))].key)!.endAnimation.value));
        }
        if ((object.Equals(((MergeableMaterial)this.widget).mainAxis, global::Doroti.Generated.Framework.Painting.Axis.vertical)))
        {
            return global::Doroti.Generated.Framework.Painting.BorderRadius.CreateVertical(top: (start ? cardRadius__15361 : startRadius__15430), bottom: (end ? cardRadius__15361 : endRadius__15468));
        }
        else
        {
            return global::Doroti.Generated.Framework.Painting.BorderRadius.CreateHorizontal(left: (start ? cardRadius__15361 : startRadius__15430), right: (end ? cardRadius__15361 : endRadius__15468));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getGapSize(long index)
    {
        var gap__16348 = ((MaterialGap?)(object?)this._children[(int)(index)])!;
        return DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this._animationTuples.GetValueOrDefault(gap__16348.key)!.gapStart, ((MaterialGap)gap__16348).size, this._animationTuples.GetValueOrDefault(gap__16348.key)!.gapAnimation.value));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _willNeedDivider(long index)
    {
        if ((index < 0L))
        {
            return false;
        }
        if ((index >= checked((long)(this._children.Count))))
        {
            return false;
        }
        return ((this._children[(int)(index)] is MaterialSlice) || _isClosingGap(index));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        _removeEmptyGaps();
        var widgets__16844 = new List<global::Doroti.Generated.Framework.Widgets.Widget>();
        var slices__16874 = new List<global::Doroti.Generated.Framework.Widgets.Widget>();
        long i__16903 = default!;
        for (i__16903 = 0L; (i__16903 < checked((long)(this._children.Count))); i__16903 += 1L)
        {
            if ((this._children[(int)(i__16903)] is MaterialGap))
            {
                DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(slices__16874));
                widgets__16844.Add(new global::Doroti.Generated.Framework.Widgets.ListBody(mainAxis: ((MergeableMaterial)this.widget).mainAxis, children: slices__16874));
                slices__16874 = new List<global::Doroti.Generated.Framework.Widgets.Widget>();
                widgets__16844.Add((((MergeableMaterial)this.widget).mainAxis switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => new global::Doroti.Generated.Framework.Widgets.SizedBox(width: _getGapSize(i__16903)), global::Doroti.Generated.Framework.Painting.Axis.vertical => new global::Doroti.Generated.Framework.Widgets.SizedBox(height: _getGapSize(i__16903)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
            }
            else
            {
                var slice__17348 = ((MaterialSlice?)(object?)this._children[(int)(i__16903)])!;
                global::Doroti.Generated.Framework.Widgets.Widget child__17402 = ((MaterialSlice)slice__17348).child;
                if (((MergeableMaterial)this.widget).hasDividers)
                {
                    bool hasTopDivider__17479 = _willNeedDivider((i__16903 - 1L));
                    bool hasBottomDivider__17541 = _willNeedDivider((i__16903 + 1L));
                    global::Doroti.Generated.Framework.Painting.BorderSide divider__17613 = Divider.createBorderSide(context, width: 0.5, color: ((MergeableMaterial)this.widget).dividerColor);
                    global::Doroti.Generated.Framework.Painting.Border border__17866 = default!;
                    if ((i__16903 == 0L))
                    {
                        border__17866 = new global::Doroti.Generated.Framework.Painting.Border(bottom: (hasBottomDivider__17541 ? divider__17613 : global::Doroti.Generated.Framework.Painting.BorderSide.none));
                    }
                    else
                    {
                        if ((i__16903 == (checked((long)(this._children.Count)) - 1L)))
                        {
                            border__17866 = new global::Doroti.Generated.Framework.Painting.Border(top: (hasTopDivider__17479 ? divider__17613 : global::Doroti.Generated.Framework.Painting.BorderSide.none));
                        }
                        else
                        {
                            border__17866 = new global::Doroti.Generated.Framework.Painting.Border(top: (hasTopDivider__17479 ? divider__17613 : global::Doroti.Generated.Framework.Painting.BorderSide.none), bottom: (hasBottomDivider__17541 ? divider__17613 : global::Doroti.Generated.Framework.Painting.BorderSide.none));
                        }
                    }
                    child__17402 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.AnimatedContainer(key: new _MergeableMaterialSliceKey__mergeable_material(this._children[(int)(i__16903)].key), decoration: new global::Doroti.Generated.Framework.Painting.BoxDecoration(border: border__17866), duration: ThemeLibrary.kThemeAnimationDuration, curve: global::Doroti.Generated.Framework.Animation.Curves.fastOutSlowIn, child: child__17402));
                }
                slices__16874.Add(new global::Doroti.Generated.Framework.Widgets.Container(decoration: new global::Doroti.Generated.Framework.Painting.BoxDecoration(color: ((((MaterialSlice?)(object?)this._children[(int)(i__16903)])!).color ?? Theme.of(context).cardColor), borderRadius: _borderRadius(i__16903, (i__16903 == 0L), (i__16903 == (checked((long)(this._children.Count)) - 1L)))), child: new Material(type: MaterialType.transparency, child: child__17402)));
            }
        }
        if (System.Linq.Enumerable.Any(slices__16874))
        {
            widgets__16844.Add(new global::Doroti.Generated.Framework.Widgets.ListBody(mainAxis: ((MergeableMaterial)this.widget).mainAxis, children: slices__16874));
            slices__16874 = new List<global::Doroti.Generated.Framework.Widgets.Widget>();
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _MergeableMaterialListBody__mergeable_material(mainAxis: ((MergeableMaterial)this.widget).mainAxis, elevation: ((MergeableMaterial)this.widget).elevation, items: this._children, children: widgets__16844));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>();
        TickerModeData values__17506 = this._tickerModeNotifier!.value;
        var result__17553 = ((Func<global::Doroti.Generated.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{            var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
            __cascade.muted = !((TickerModeData)values__17506).enabled;
            __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
            return __cascade;        }))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Generated.Framework.Scheduler.Ticker)(object?)result__17553);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Generated.Framework.Widgets._WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => (this._tickers is not null));
        DartRuntimePrimitives.Assert(() => this._tickers!.Contains(ticker));
        this._tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if ((this._tickers is not null))
        {
            TickerModeData values__18318 = this._tickerModeNotifier!.value;
            bool muted__18372 = !((TickerModeData)values__18318).enabled;
            foreach (global::Doroti.Generated.Framework.Scheduler.Ticker ticker__18421 in this._tickers!)
            {
                ticker__18421.muted = muted__18372;
                ticker__18421.forceFrames = ((TickerModeData)values__18318).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__18621 = ((global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__18621, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier__18621.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier__18621;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

internal class _MergeableMaterialSliceKey__mergeable_material : global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>
{
    public virtual global::Doroti.Generated.Framework.Foundation.LocalKey value { get; private set; } = default!;

    internal _MergeableMaterialSliceKey__mergeable_material(global::Doroti.Generated.Framework.Foundation.LocalKey value)
    {
        this.value = value;
    }

    public override bool Equals(object? other)
    {
        var __other = other as _MergeableMaterialSliceKey__mergeable_material;
        if (__other is null) return false;
        return ((__other is _MergeableMaterialSliceKey__mergeable_material) && (object.Equals(((_MergeableMaterialSliceKey__mergeable_material)((_MergeableMaterialSliceKey__mergeable_material)__other)).value, this.value)));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(this.value.GetHashCode());
    public override string ToString()
    {
        return $"_MergeableMaterialSliceKey({this.value})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MergeableMaterialListBody__mergeable_material : global::Doroti.Generated.Framework.Widgets.ListBody
{
    public virtual List<MergeableMaterialItem> items { get; private set; } = default!;
    public virtual double elevation { get; private set; } = default!;

    internal _MergeableMaterialListBody__mergeable_material(List<global::Doroti.Generated.Framework.Widgets.Widget> children, global::Doroti.Generated.Framework.Painting.Axis mainAxis = global::Doroti.Generated.Framework.Painting.Axis.vertical, List<MergeableMaterialItem> items = default!, double elevation = default!) : base(children: children, mainAxis: mainAxis)
    {
        this.items = items;
        this.elevation = elevation;
    }

    internal virtual global::Doroti.Generated.Framework.Painting.AxisDirection _getDirection(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return global::Doroti.Generated.Framework.Widgets.BasicLibrary.getAxisDirectionFromAxisReverseAndDirectionality(context, this.mainAxis, false);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _RenderMergeableMaterialListBody__mergeable_material(axisDirection: _getDirection(context), elevation: this.elevation));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderListBody)(object)renderObject;
        var materialRenderListBody__20540 = ((_RenderMergeableMaterialListBody__mergeable_material?)(object?)__renderObject)!;
        DartRuntimePrimitives.Ignore(((Func<_RenderMergeableMaterialListBody__mergeable_material>)(() =>
{            var __cascade = materialRenderListBody__20540;
            __cascade.axisDirection = _getDirection(context);
            __cascade.elevation = this.elevation;
            return __cascade;        }))());
    }

}

internal class _RenderMergeableMaterialListBody__mergeable_material : global::Doroti.Generated.Framework.Rendering.RenderListBody
{
    internal virtual double _elevation { get; set; } = default!;

    internal _RenderMergeableMaterialListBody__mergeable_material(global::Doroti.Generated.Framework.Painting.AxisDirection axisDirection = global::Doroti.Generated.Framework.Painting.AxisDirection.down, double elevation = 0.0) : base(axisDirection: axisDirection)
    {
        this._elevation = elevation;
    }

    public virtual double elevation
    {
        get => this._elevation;
        set
        {
            var __value = value;
            if ((__value == this._elevation))
            {
                return;
            }
            _elevation = __value;
            markNeedsPaint();
        }
    }
    internal virtual void _paintShadows(Canvas canvas, Rect rect)
    {
        if ((this.elevation != 0L))
        {
            canvas.drawShadow(((Func<Path>)(() =>
{            var __cascade = new global::Doroti.Ui.Path();
            __cascade.addRRect(MaterialLibrary.kMaterialEdges.GetValueOrDefault(MaterialType.card)!.toRRect(rect));
            return __cascade;        }))(), Colors.black, this.elevation, true);
        }
    }

    public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__21608 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)this.firstChild);
        var index__21636 = 0L;
        while ((child__21608 is not null))
        {
            var childParentData__21687 = ((global::Doroti.Generated.Framework.Rendering.ListBodyParentData?)(object?)child__21608.parentData!)!;
            global::Doroti.Ui.Rect rect__21763 = ((global::Doroti.Ui.Rect)(object?)(((childParentData__21687.offset + offset)) & ((global::Doroti.Generated.Framework.Rendering.RenderBox)child__21608).size));
            if (((checked((long)(index__21636)) & 1L) == 0L))
            {
                _paintShadows(((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas, rect__21763);
            }
            child__21608 = childParentData__21687.nextSibling;
            index__21636 += 1L;
        }
        defaultPaint(context, offset);
    }

}
