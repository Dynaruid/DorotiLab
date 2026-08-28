// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/mergeable_material.dart
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace Doroti.Framework.Material;

public abstract class MergeableMaterialItem
{
    public virtual global::Doroti.Framework.Foundation.LocalKey key { get; private set; } = default!;

    protected MergeableMaterialItem(global::Doroti.Framework.Foundation.LocalKey key)
    {
        this.key = key;
    }

}

public class MaterialSlice : MergeableMaterialItem
{
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual Color? color { get; private set; }

    public MaterialSlice(global::Doroti.Framework.Foundation.LocalKey key, global::Doroti.Framework.Widgets.Widget child, Color? color = null) : base(key)
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

    public MaterialGap(global::Doroti.Framework.Foundation.LocalKey key, double size = 16.0) : base(key)
    {
        this.size = size;
    }

    public override string ToString()
    {
        return $"MaterialGap(key: {this.key}, child: {this.size})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class MergeableMaterial : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual List<MergeableMaterialItem> children { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.Axis mainAxis { get; private set; } = default!;
    public virtual double elevation { get; private set; } = default!;
    public virtual bool hasDividers { get; private set; } = default!;
    public virtual Color? dividerColor { get; private set; }

    public MergeableMaterial(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.Axis mainAxis = global::Doroti.Framework.Painting.Axis.vertical, double elevation = 2, bool hasDividers = false, List<MergeableMaterialItem> children = default!, Color? dividerColor = null) : base(key: key)
    {
        List<MergeableMaterialItem> __children = children ?? new List<MergeableMaterialItem>();
        this.mainAxis = mainAxis;
        this.elevation = elevation;
        this.hasDividers = hasDividers;
        this.children = __children;
        this.dividerColor = dividerColor;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Painting.Axis>("mainAxis", this.mainAxis));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("elevation", this.elevation));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _MergeableMaterialState__mergeable_material());
}

internal class _AnimationTuple__mergeable_material
{
    public virtual global::Doroti.Framework.Animation.AnimationController controller { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation startAnimation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation endAnimation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation gapAnimation { get; private set; } = default!;
    public virtual double gapStart { get; set; } = 0.0;

    internal _AnimationTuple__mergeable_material(global::Doroti.Framework.Animation.AnimationController controller, global::Doroti.Framework.Animation.CurvedAnimation startAnimation, global::Doroti.Framework.Animation.CurvedAnimation endAnimation, global::Doroti.Framework.Animation.CurvedAnimation gapAnimation)
    {
        this.controller = controller;
        this.startAnimation = startAnimation;
        this.endAnimation = endAnimation;
        this.gapAnimation = gapAnimation;
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchCreated("material", "_AnimationTuple", this));
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        this.controller.dispose();
        this.startAnimation.dispose();
        this.endAnimation.dispose();
        this.gapAnimation.dispose();
    }

}

internal class _MergeableMaterialState__mergeable_material : global::Doroti.Framework.Widgets.State<MergeableMaterial>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<MergeableMaterial>
{
    internal virtual List<MergeableMaterialItem> _children { get; set; } = default!;
    internal virtual DartMap<global::Doroti.Framework.Foundation.LocalKey, _AnimationTuple__mergeable_material?> _animationTuples { get; private set; } = new DartMap<global::Doroti.Framework.Foundation.LocalKey, _AnimationTuple__mergeable_material?>();
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _children = new List<MergeableMaterialItem>(DartRuntimePrimitives.ConvertEnumerable<MergeableMaterialItem>(((MergeableMaterial)this.widget).children));
        for (var i = 0L; (i < checked((long)(this._children.Count))); i += 1L)
        {
            MergeableMaterialItem child = this._children[(int)(i)];
            if ((child is MaterialGap))
            {
                MaterialGap child__5815__as5847 = (MaterialGap)child;
                _initGap(((MaterialGap)child__5815__as5847));
                this._animationTuples[child__5815__as5847.key]!.controller.value = 1.0;
            }
        }
        DartRuntimePrimitives.Assert(() => _debugGapsAreValid(this._children));
    }

    internal virtual void _initGap(MaterialGap gap)
    {
        var controllerLocal = new global::Doroti.Framework.Animation.AnimationController(duration: ThemeLibrary.kThemeAnimationDuration, vsync: this);
        var startAnimationLocal = new global::Doroti.Framework.Animation.CurvedAnimation(parent: controllerLocal, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
        var endAnimationLocal = new global::Doroti.Framework.Animation.CurvedAnimation(parent: controllerLocal, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
        var gapAnimationLocal = new global::Doroti.Framework.Animation.CurvedAnimation(parent: controllerLocal, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
        controllerLocal.addListener(this._handleTick);
        this._animationTuples[gap.key] = new _AnimationTuple__mergeable_material(controller: controllerLocal, startAnimation: startAnimationLocal, endAnimation: endAnimationLocal, gapAnimation: gapAnimationLocal);
    }

    public override void dispose()
    {
        foreach (MergeableMaterialItem child in this._children)
        {
            if ((child is MaterialGap))
            {
                MaterialGap child__6764__as6796 = (MaterialGap)child;
                this._animationTuples.GetValueOrDefault(((MaterialGap)child__6764__as6796).key)!.dispose();
            }
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        this._tickerModeNotifier?.removeListener(this._updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual void _handleTick()
    {
        setState(((global::System.Action)(() =>
        {
        })));
    }

    internal virtual bool _debugHasConsecutiveGaps(List<MergeableMaterialItem> children)
    {
        for (var i = 0L; (i < (checked((long)(((MergeableMaterial)this.widget).children.Count)) - 1L)); i += 1L)
        {
            if (((((MergeableMaterial)this.widget).children[(int)(i)] is MaterialGap) && (((MergeableMaterial)this.widget).children[(int)((i + 1L))] is MaterialGap)))
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
        MergeableMaterialItem child = this._children.removeAt(index);
        if ((child is MaterialGap))
        {
            MaterialGap child__7935__as7979 = (MaterialGap)child;
            this._animationTuples.GetValueOrDefault(((MaterialGap)child__7935__as7979).key)!.dispose();
            this._animationTuples[child__7935__as7979.key] = null;
        }
    }

    internal virtual bool _isClosingGap(long index)
    {
        if (((index < (checked((long)(this._children.Count)) - 1L)) && (this._children[(int)(index)] is MaterialGap)))
        {
            return (object.Equals(this._animationTuples.GetValueOrDefault(this._children[(int)(index)].key)!.controller.status, global::Doroti.Framework.Animation.AnimationStatus.reverse));
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _removeEmptyGaps()
    {
        for (long j = (checked((long)(this._children.Count)) - 1L); (j >= 0L); j -= 1L)
        {
            if (((this._children[(int)(j)] is MaterialGap) && this._animationTuples.GetValueOrDefault(this._children[(int)(j)].key)!.controller.isDismissed))
            {
                _removeChild(j);
            }
        }
    }

    public override void didUpdateWidget(MergeableMaterial oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        HashSet<global::Doroti.Framework.Foundation.LocalKey> oldKeys = ((MergeableMaterial)oldWidget).children.map<MergeableMaterialItem, global::Doroti.Framework.Foundation.LocalKey>(((child) => ((MergeableMaterialItem)child).key)).toSet();
        HashSet<global::Doroti.Framework.Foundation.LocalKey> newKeys = ((MergeableMaterial)this.widget).children.map<MergeableMaterialItem, global::Doroti.Framework.Foundation.LocalKey>(((child) => ((MergeableMaterialItem)child).key)).toSet();
        HashSet<global::Doroti.Framework.Foundation.LocalKey> newOnly = newKeys.difference<global::Doroti.Framework.Foundation.LocalKey>(oldKeys);
        HashSet<global::Doroti.Framework.Foundation.LocalKey> oldOnly = oldKeys.difference<global::Doroti.Framework.Foundation.LocalKey>(newKeys);
        List<MergeableMaterialItem> newChildren = ((MergeableMaterial)this.widget).children.ToList();
        var i = 0L;
        var j = 0L;
        DartRuntimePrimitives.Assert(() => _debugGapsAreValid(newChildren));
        _removeEmptyGaps();
        while (((i < checked((long)(newChildren.Count))) && (j < checked((long)(this._children.Count)))))
        {
            if ((newOnly.Contains(newChildren[(int)(i)].key) || oldOnly.Contains(this._children[(int)(j)].key)))
            {
                var startNew = i;
                var startOld = j;
                while (newOnly.Contains(newChildren[(int)(i)].key))
                {
                    i += 1L;
                }
                while ((oldOnly.Contains(this._children[(int)(j)].key) || _isClosingGap(j)))
                {
                    j += 1L;
                }
                long newLength = (i - startNew);
                long oldLength = (j - startOld);
                if ((newLength > 0L))
                {
                    if (((oldLength > 1L) || ((oldLength == 1L) && (this._children[(int)(startOld)] is MaterialSlice))))
                    {
                        if (((newLength == 1L) && (newChildren[(int)(startNew)] is MaterialGap)))
                        {
                            var gapSizeSum = 0.0;
                            while ((startOld < j))
                            {
                                MergeableMaterialItem childLocal = this._children[(int)(startOld)];
                                if ((childLocal is MaterialGap))
                                {
                                    MaterialGap child__10164__as10213 = (MaterialGap)childLocal;
                                    MaterialGap gap = ((MaterialGap)child__10164__as10213);
                                    gapSizeSum += ((MaterialGap)gap).size;
                                }
                                _removeChild(startOld);
                                j -= 1L;
                            }
                            _insertChild(startOld, newChildren[(int)(startNew)]);
                            DartRuntimePrimitives.Ignore(((Func<_AnimationTuple__mergeable_material>)(() =>
{
    var __cascade = this._animationTuples.GetValueOrDefault(newChildren[(int)(startNew)].key)!;
    __cascade.gapStart = gapSizeSum;
    __cascade.controller.forward();
    return __cascade;
}))());
                            j += 1L;
                        }
                        else
                        {
                            for (var k = 0L; (k < oldLength); k += 1L)
                            {
                                _removeChild(startOld);
                            }
                            for (var kLocal = 0L; (kLocal < newLength); kLocal += 1L)
                            {
                                _insertChild((startOld + kLocal), newChildren[(int)((startNew + kLocal))]);
                            }
                            j += (newLength - oldLength);
                        }
                    }
                    else
                    {
                        if ((oldLength == 1L))
                        {
                            if ((((newLength == 1L) && (newChildren[(int)(startNew)] is MaterialGap)) && (object.Equals(this._children[(int)(startOld)].key, newChildren[(int)(startNew)].key))))
                            {
                                this._animationTuples.GetValueOrDefault(newChildren[(int)(startNew)].key)!.controller.forward();
                            }
                            else
                            {
                                double gapSize = _getGapSize(startOld);
                                _removeChild(startOld);
                                for (var kAlternate = 0L; (kAlternate < newLength); kAlternate += 1L)
                                {
                                    _insertChild((startOld + kAlternate), newChildren[(int)((startNew + kAlternate))]);
                                }
                                j += (newLength - 1L);
                                var gapSizeSumLocal = 0.0;
                                for (var kNested = startNew; (kNested < i); kNested += 1L)
                                {
                                    MergeableMaterialItem newChild = newChildren[(int)(kNested)];
                                    if ((newChild is MaterialGap))
                                    {
                                        MaterialGap newChild__11812__as11859 = (MaterialGap)newChild;
                                        gapSizeSumLocal += ((MaterialGap)((MaterialGap)newChild__11812__as11859)).size;
                                    }
                                }
                                for (var kCurrent = startNew; (kCurrent < i); kCurrent += 1L)
                                {
                                    MergeableMaterialItem newChildLocal = newChildren[(int)(kCurrent)];
                                    if ((newChildLocal is MaterialGap))
                                    {
                                        MaterialGap newChild__12196__as12243 = (MaterialGap)newChildLocal;
                                        this._animationTuples[newChild__12196__as12243.key]!.gapStart = ((gapSize * ((MaterialGap)((MaterialGap)newChild__12196__as12243)).size) / gapSizeSumLocal);
                                        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{
    var __cascade = this._animationTuples.GetValueOrDefault(((MaterialGap)newChild__12196__as12243).key)!.controller;
    __cascade.value = 0.0;
    __cascade.forward();
    return __cascade;
}))());
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (var kNext = 0L; (kNext < newLength); kNext += 1L)
                            {
                                MergeableMaterialItem newChildAlternate = newChildren[(int)((startNew + kNext))];
                                _insertChild((startOld + kNext), newChildAlternate);
                                if ((newChildAlternate is MaterialGap))
                                {
                                    MaterialGap newChild__12685__as12795 = (MaterialGap)newChildAlternate;
                                    this._animationTuples.GetValueOrDefault(((MaterialGap)newChild__12685__as12795).key)!.controller.forward();
                                }
                            }
                            j += newLength;
                        }
                    }
                }
                else
                {
                    if (((oldLength > 1L) || ((oldLength == 1L) && (this._children[(int)(startOld)] is MaterialSlice))))
                    {
                        var gapSizeSumAlternate = 0.0;
                        while ((startOld < j))
                        {
                            MergeableMaterialItem childAlternate = this._children[(int)(startOld)];
                            if ((childAlternate is MaterialGap))
                            {
                                MaterialGap child__13262__as13309 = (MaterialGap)childAlternate;
                                gapSizeSumAlternate += ((MaterialGap)((MaterialGap)child__13262__as13309)).size;
                            }
                            _removeChild(startOld);
                            j -= 1L;
                        }
                        if ((gapSizeSumAlternate != 0.0))
                        {
                            var gapLocal = new MaterialGap(key: new global::Doroti.Framework.Foundation.UniqueKey(), size: gapSizeSumAlternate);
                            _insertChild(startOld, gapLocal);
                            this._animationTuples[gapLocal.key]!.gapStart = 0.0;
                            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{
    var __cascade = this._animationTuples.GetValueOrDefault(gapLocal.key)!.controller;
    __cascade.value = 1.0;
    __cascade.reverse();
    return __cascade;
}))());
                            j += 1L;
                        }
                    }
                    else
                    {
                        if ((oldLength == 1L))
                        {
                            var gapAlternate = ((MaterialGap?)(object?)this._children[(int)(startOld)])!;
                            this._animationTuples[gapAlternate.key]!.gapStart = 0.0;
                            this._animationTuples.GetValueOrDefault(gapAlternate.key)!.controller.reverse();
                        }
                    }
                }
            }
            else
            {
                if ((((this._children[(int)(j)] is MaterialGap)) == ((newChildren[(int)(i)] is MaterialGap))))
                {
                    this._children[(int)(j)] = newChildren[(int)(i)];
                    i += 1L;
                    j += 1L;
                }
                else
                {
                    DartRuntimePrimitives.Assert(() => (this._children[(int)(j)] is MaterialGap));
                    j += 1L;
                }
            }
        }
        while ((j < checked((long)(this._children.Count))))
        {
            _removeChild(j);
        }
        while ((i < checked((long)(newChildren.Count))))
        {
            MergeableMaterialItem newChildNested = newChildren[(int)(i)];
            _insertChild(j, newChildNested);
            if ((newChildNested is MaterialGap))
            {
                MaterialGap newChild__14719__as14790 = (MaterialGap)newChildNested;
                this._animationTuples.GetValueOrDefault(((MaterialGap)newChild__14719__as14790).key)!.controller.forward();
            }
            i += 1L;
            j += 1L;
        }
    }

    internal virtual global::Doroti.Framework.Painting.BorderRadius _borderRadius(long index, bool start, bool end)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(MaterialLibrary.kMaterialEdges.GetValueOrDefault(MaterialType.card)!.topLeft, MaterialLibrary.kMaterialEdges.GetValueOrDefault(MaterialType.card)!.topRight)));
        DartRuntimePrimitives.Assert(() => (object.Equals(MaterialLibrary.kMaterialEdges.GetValueOrDefault(MaterialType.card)!.topLeft, MaterialLibrary.kMaterialEdges.GetValueOrDefault(MaterialType.card)!.bottomLeft)));
        DartRuntimePrimitives.Assert(() => (object.Equals(MaterialLibrary.kMaterialEdges.GetValueOrDefault(MaterialType.card)!.topLeft, MaterialLibrary.kMaterialEdges.GetValueOrDefault(MaterialType.card)!.bottomRight)));
        global::Doroti.Ui.Radius cardRadius = ((global::Doroti.Ui.Radius)(object?)MaterialLibrary.kMaterialEdges.GetValueOrDefault(MaterialType.card)!.topLeft);
        global::Doroti.Ui.Radius startRadius = ((global::Doroti.Ui.Radius)(object?)Radius.zero);
        global::Doroti.Ui.Radius endRadius = ((global::Doroti.Ui.Radius)(object?)Radius.zero);
        if (((index > 0L) && (this._children[(int)((index - 1L))] is MaterialGap)))
        {
            startRadius = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Radius.lerp(Radius.zero, cardRadius, this._animationTuples.GetValueOrDefault(this._children[(int)((index - 1L))].key)!.startAnimation.value));
        }
        if (((index < (checked((long)(this._children.Count)) - 2L)) && (this._children[(int)((index + 1L))] is MaterialGap)))
        {
            endRadius = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Radius.lerp(Radius.zero, cardRadius, this._animationTuples.GetValueOrDefault(this._children[(int)((index + 1L))].key)!.endAnimation.value));
        }
        if ((object.Equals(((MergeableMaterial)this.widget).mainAxis, global::Doroti.Framework.Painting.Axis.vertical)))
        {
            return global::Doroti.Framework.Painting.BorderRadius.CreateVertical(top: (start ? cardRadius : startRadius), bottom: (end ? cardRadius : endRadius));
        }
        else
        {
            return global::Doroti.Framework.Painting.BorderRadius.CreateHorizontal(left: (start ? cardRadius : startRadius), right: (end ? cardRadius : endRadius));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getGapSize(long index)
    {
        var gap = ((MaterialGap?)(object?)this._children[(int)(index)])!;
        return DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this._animationTuples.GetValueOrDefault(gap.key)!.gapStart, ((MaterialGap)gap).size, this._animationTuples.GetValueOrDefault(gap.key)!.gapAnimation.value));
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

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        _removeEmptyGaps();
        var widgets = new List<global::Doroti.Framework.Widgets.Widget>();
        var slices = new List<global::Doroti.Framework.Widgets.Widget>();
        long i = default!;
        for (i = 0L; (i < checked((long)(this._children.Count))); i += 1L)
        {
            if ((this._children[(int)(i)] is MaterialGap))
            {
                DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(slices));
                widgets.Add(new global::Doroti.Framework.Widgets.ListBody(mainAxis: ((MergeableMaterial)this.widget).mainAxis, children: slices));
                slices = new List<global::Doroti.Framework.Widgets.Widget>();
                widgets.Add((((MergeableMaterial)this.widget).mainAxis switch { global::Doroti.Framework.Painting.Axis.horizontal => new global::Doroti.Framework.Widgets.SizedBox(width: _getGapSize(i)), global::Doroti.Framework.Painting.Axis.vertical => new global::Doroti.Framework.Widgets.SizedBox(height: _getGapSize(i)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
            }
            else
            {
                var slice = ((MaterialSlice?)(object?)this._children[(int)(i)])!;
                global::Doroti.Framework.Widgets.Widget childLocal = ((MaterialSlice)slice).child;
                if (((MergeableMaterial)this.widget).hasDividers)
                {
                    bool hasTopDivider = _willNeedDivider((i - 1L));
                    bool hasBottomDivider = _willNeedDivider((i + 1L));
                    global::Doroti.Framework.Painting.BorderSide divider = Divider.createBorderSide(context, width: 0.5, color: ((MergeableMaterial)this.widget).dividerColor);
                    global::Doroti.Framework.Painting.Border borderLocal = default!;
                    if ((i == 0L))
                    {
                        borderLocal = new global::Doroti.Framework.Painting.Border(bottom: (hasBottomDivider ? divider : global::Doroti.Framework.Painting.BorderSide.none));
                    }
                    else
                    {
                        if ((i == (checked((long)(this._children.Count)) - 1L)))
                        {
                            borderLocal = new global::Doroti.Framework.Painting.Border(top: (hasTopDivider ? divider : global::Doroti.Framework.Painting.BorderSide.none));
                        }
                        else
                        {
                            borderLocal = new global::Doroti.Framework.Painting.Border(top: (hasTopDivider ? divider : global::Doroti.Framework.Painting.BorderSide.none), bottom: (hasBottomDivider ? divider : global::Doroti.Framework.Painting.BorderSide.none));
                        }
                    }
                    childLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.AnimatedContainer(key: new _MergeableMaterialSliceKey__mergeable_material(this._children[(int)(i)].key), decoration: new global::Doroti.Framework.Painting.BoxDecoration(border: borderLocal), duration: ThemeLibrary.kThemeAnimationDuration, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn, child: childLocal));
                }
                slices.Add(new global::Doroti.Framework.Widgets.Container(decoration: new global::Doroti.Framework.Painting.BoxDecoration(color: ((((MaterialSlice?)(object?)this._children[(int)(i)])!).color ?? Theme.of(context).cardColor), borderRadius: _borderRadius(i, (i == 0L), (i == (checked((long)(this._children.Count)) - 1L)))), child: new Material(type: MaterialType.transparency, child: childLocal)));
            }
        }
        if (System.Linq.Enumerable.Any(slices))
        {
            widgets.Add(new global::Doroti.Framework.Widgets.ListBody(mainAxis: ((MergeableMaterial)this.widget).mainAxis, children: slices));
            slices = new List<global::Doroti.Framework.Widgets.Widget>();
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _MergeableMaterialListBody__mergeable_material(mainAxis: ((MergeableMaterial)this.widget).mainAxis, elevation: ((MergeableMaterial)this.widget).elevation, items: this._children, children: widgets));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = this._tickerModeNotifier!.value;
        var result = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
    __cascade.muted = !((TickerModeData)values).enabled;
    __cascade.forceFrames = ((TickerModeData)values).forceFrames;
    return __cascade;
}))();
        this._tickers!.Add(result);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider ticker)
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
            TickerModeData values = this._tickerModeNotifier!.value;
            bool mutedLocal = !((TickerModeData)values).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = ((TickerModeData)values).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(this._updateTickers);
        newNotifier.addListener(this._updateTickers);
        this._tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

internal class _MergeableMaterialSliceKey__mergeable_material : global::Doroti.Framework.Widgets.GlobalKey<IState>
{
    public virtual global::Doroti.Framework.Foundation.LocalKey value { get; private set; } = default!;

    internal _MergeableMaterialSliceKey__mergeable_material(global::Doroti.Framework.Foundation.LocalKey value)
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

internal class _MergeableMaterialListBody__mergeable_material : global::Doroti.Framework.Widgets.ListBody
{
    public virtual List<MergeableMaterialItem> items { get; private set; } = default!;
    public virtual double elevation { get; private set; } = default!;

    internal _MergeableMaterialListBody__mergeable_material(List<global::Doroti.Framework.Widgets.Widget> children, global::Doroti.Framework.Painting.Axis mainAxis = global::Doroti.Framework.Painting.Axis.vertical, List<MergeableMaterialItem> items = default!, double elevation = default!) : base(children: children, mainAxis: mainAxis)
    {
        this.items = items;
        this.elevation = elevation;
    }

    internal virtual global::Doroti.Framework.Painting.AxisDirection _getDirection(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return global::Doroti.Framework.Widgets.BasicLibrary.getAxisDirectionFromAxisReverseAndDirectionality(context, this.mainAxis, false);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderMergeableMaterialListBody__mergeable_material(axisDirection: _getDirection(context), elevation: this.elevation));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderListBody)(object)renderObject;
        var materialRenderListBody = ((_RenderMergeableMaterialListBody__mergeable_material?)(object?)__renderObject)!;
        DartRuntimePrimitives.Ignore(((Func<_RenderMergeableMaterialListBody__mergeable_material>)(() =>
{
    var __cascade = materialRenderListBody;
    __cascade.axisDirection = _getDirection(context);
    __cascade.elevation = this.elevation;
    return __cascade;
}))());
    }

}

internal class _RenderMergeableMaterialListBody__mergeable_material : global::Doroti.Framework.Rendering.RenderListBody
{
    internal virtual double _elevation { get; set; } = default!;

    internal _RenderMergeableMaterialListBody__mergeable_material(global::Doroti.Framework.Painting.AxisDirection axisDirection = global::Doroti.Framework.Painting.AxisDirection.down, double elevation = 0.0) : base(axisDirection: axisDirection)
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
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addRRect(MaterialLibrary.kMaterialEdges.GetValueOrDefault(MaterialType.card)!.toRRect(rect));
    return __cascade;
}))(), Colors.black, this.elevation, true);
        }
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)this.firstChild);
        var index = 0L;
        while ((child is not null))
        {
            var childParentData = ((global::Doroti.Framework.Rendering.ListBodyParentData?)(object?)child.parentData!)!;
            global::Doroti.Ui.Rect rect = ((global::Doroti.Ui.Rect)(object?)(((childParentData.offset + offset)) & ((global::Doroti.Framework.Rendering.RenderBox)child).size));
            if (((checked((long)(index)) & 1L) == 0L))
            {
                _paintShadows(((global::Doroti.Framework.Rendering.PaintingContext)context).canvas, rect);
            }
            child = childParentData.nextSibling;
            index += 1L;
        }
        defaultPaint(context, offset);
    }

}
