// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/animated_size.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Rendering;

public enum RenderAnimatedSizeState
{
    start,
    stable,
    changed,
    unstable
}

public class RenderAnimatedSize : RenderAligningShiftedBox
{
    internal virtual AnimationController _controller { get; private set; } = default!;
    internal virtual CurvedAnimation _animation { get; private set; } = default!;
    internal virtual SizeTween _sizeTween { get; private set; } = new SizeTween();
    internal virtual bool _hasVisualOverflow { get; set; } = default!;
    internal virtual double? _lastValue { get; set; } = default;
    internal virtual RenderAnimatedSizeState _state { get; set; } = RenderAnimatedSizeState.start;
    internal virtual Clip _clipBehavior { get; set; } = Clip.hardEdge;
    internal virtual TickerProvider _vsync { get; set; } = default!;
    internal virtual Action? _onEnd { get; set; } = default;
    internal virtual Size _currentSize { get; set; } = default!;
    internal virtual LayerHandle<ClipRectLayer> _clipRectLayer { get; private set; } = new LayerHandle<ClipRectLayer>();

    public RenderAnimatedSize(TickerProvider vsync, Duration duration, Duration? reverseDuration = null, Curve curve = default!, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, TextDirection? textDirection = null, RenderBox? child = null, Clip clipBehavior = Clip.hardEdge, Action? onEnd = null) : base(alignment: alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center, textDirection: DartRuntimePrimitives.RequireValue(textDirection), child: child)
    {
        Curve __curve = curve ?? Curves.linear;
        this._vsync = vsync;
        this._clipBehavior = clipBehavior;
    }

    public virtual AnimationController? debugController
    {
        get
        {
            AnimationController? controller__3791 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    controller__3791 = this._controller;
                    return true;
                });
            return controller__3791;
            return default!;
        }
    }
    public virtual CurvedAnimation? debugAnimation
    {
        get
        {
            CurvedAnimation? animation__4225 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    animation__4225 = this._animation;
                    return true;
                });
            return animation__4225;
            return default!;
        }
    }
    public virtual RenderAnimatedSizeState state => this._state;
    public virtual Duration duration
    {
        get => DartRuntimePrimitives.RequireValue(this._controller.duration);
        set
        {
            var __value = value;
            if ((object.Equals(DartRuntimePrimitives.RequireValue(__value), this._controller.duration)))
            {
                return;
            }
            this._controller.duration = DartRuntimePrimitives.RequireValue(__value);
        }
    }
    public virtual Duration? reverseDuration
    {
        get => this._controller.reverseDuration;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._controller.reverseDuration)))
            {
                return;
            }
            this._controller.reverseDuration = __value;
        }
    }
    public virtual Curve curve
    {
        get => this._animation.curve;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._animation.curve)))
            {
                return;
            }
            this._animation.curve = __value;
        }
    }
    public virtual global::Doroti.Flutter.Ui.Clip clipBehavior
    {
        get => this._clipBehavior;
        set
        {
            var __value = value;
            if ((!object.Equals(DartRuntimePrimitives.RequireValue(__value), this._clipBehavior)))
            {
                _clipBehavior = DartRuntimePrimitives.RequireValue(__value);
                markNeedsPaint();
                markNeedsSemanticsUpdate();
            }
        }
    }
    public virtual bool isAnimating => this._controller.isAnimating;
    public virtual TickerProvider vsync
    {
        get => this._vsync;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._vsync)))
            {
                return;
            }
            _vsync = __value;
            this._controller.resync(this.vsync);
        }
    }
    public virtual Action? onEnd
    {
        get => this._onEnd;
        set
        {
            var __value = value;
            if ((object.Equals((Action?)__value, (Action?)this._onEnd)))
            {
                return;
            }
            _onEnd = __value;
        }
    }
    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        switch (this.state)
        {
            case RenderAnimatedSizeState.start:
            case RenderAnimatedSizeState.stable:
                {
                    break;
                }
            case RenderAnimatedSizeState.changed:
            case RenderAnimatedSizeState.unstable:
                {
                    markNeedsLayout();
                    break;
                }
        }
        this._controller.addStatusListener(this._animationStatusListener);
    }

    public override void detach()
    {
        this._controller.stop();
        this._controller.removeStatusListener(this._animationStatusListener);
        base.detach();
    }

    internal virtual global::Doroti.Flutter.Ui.Size? _animatedSize
    {
        get
        {
            return this._sizeTween.evaluate(this._animation);
            return default!;
        }
    }
    public override void performLayout()
    {
        _lastValue = this._controller.value;
        _hasVisualOverflow = false;
        BoxConstraints constraints__7556 = this.constraints;
        if (((child is null) || ((BoxConstraints)constraints__7556).isTight))
        {
            this._controller.stop();
            size = _currentSize = this._sizeTween.begin = this._sizeTween.end = ((BoxConstraints)constraints__7556).smallest;
            _state = RenderAnimatedSizeState.start;
            child?.layout(constraints__7556);
            return;
        }
        child!.layout(constraints__7556, parentUsesSize: true);
        switch (this._state)
        {
            case RenderAnimatedSizeState.start:
                {
                    _layoutStart();
                    break;
                }
            case RenderAnimatedSizeState.stable:
                {
                    _layoutStable();
                    break;
                }
            case RenderAnimatedSizeState.changed:
                {
                    _layoutChanged();
                    break;
                }
            case RenderAnimatedSizeState.unstable:
                {
                    _layoutUnstable();
                    break;
                }
        }
        size = _currentSize = constraints__7556.constrain(DartRuntimePrimitives.RequireValue(this._animatedSize));
        alignChild();
        if (((size.width < DartRuntimePrimitives.RequireValue(this._sizeTween.end).width) || (size.height < DartRuntimePrimitives.RequireValue(this._sizeTween.end).height)))
        {
            _hasVisualOverflow = true;
        }
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        if (((child is null) || ((BoxConstraints)constraints).isTight))
        {
            return ((BoxConstraints)constraints).smallest;
        }
        global::Doroti.Flutter.Ui.Size childSize__8826 = child!.getDryLayout(constraints);
        switch (this._state)
        {
            case RenderAnimatedSizeState.start:
                {
                    return constraints.constrain(childSize__8826);
                }
            case RenderAnimatedSizeState.stable:
                {
                    if ((!object.Equals(this._sizeTween.end, childSize__8826)))
                    {
                        return constraints.constrain(this._currentSize);
                    }
                    else
                    {
                        if ((this._controller.value == this._controller.upperBound))
                        {
                            return constraints.constrain(childSize__8826);
                        }
                    }
                    break;
                }
            case RenderAnimatedSizeState.unstable:
            case RenderAnimatedSizeState.changed:
                {
                    if ((!object.Equals(this._sizeTween.end, childSize__8826)))
                    {
                        return constraints.constrain(childSize__8826);
                    }
                    break;
                }
        }
        return constraints.constrain(DartRuntimePrimitives.RequireValue(this._animatedSize));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _restartAnimation()
    {
        _lastValue = 0.0;
        this._controller.forward(from: 0.0);
    }

    internal virtual void _layoutStart()
    {
        this._sizeTween.begin = this._sizeTween.end = debugAdoptSize(child!.size);
        _state = RenderAnimatedSizeState.stable;
    }

    internal virtual void _layoutStable()
    {
        if ((!object.Equals(this._sizeTween.end, child!.size)))
        {
            this._sizeTween.begin = size;
            this._sizeTween.end = debugAdoptSize(child!.size);
            _restartAnimation();
            _state = RenderAnimatedSizeState.changed;
        }
        else
        {
            if ((this._controller.value == this._controller.upperBound))
            {
                this._sizeTween.begin = this._sizeTween.end = debugAdoptSize(child!.size);
            }
            else
            {
                if (!this._controller.isAnimating)
                {
                    this._controller.forward();
                }
            }
        }
    }

    internal virtual void _layoutChanged()
    {
        if ((!object.Equals(this._sizeTween.end, child!.size)))
        {
            this._sizeTween.begin = this._sizeTween.end = debugAdoptSize(child!.size);
            _restartAnimation();
            _state = RenderAnimatedSizeState.unstable;
        }
        else
        {
            _state = RenderAnimatedSizeState.stable;
            if (!this._controller.isAnimating)
            {
                this._controller.forward();
            }
        }
    }

    internal virtual void _layoutUnstable()
    {
        if ((!object.Equals(this._sizeTween.end, child!.size)))
        {
            this._sizeTween.begin = this._sizeTween.end = debugAdoptSize(child!.size);
            _restartAnimation();
        }
        else
        {
            this._controller.stop();
            _state = RenderAnimatedSizeState.stable;
        }
    }

    internal virtual void _animationStatusListener(AnimationStatus status)
    {
        if (global::Doroti.Generated.Framework.Animation.AnimationStatusMembers.isCompleted(status))
        {
            this._onEnd?.Invoke();
        }
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if ((((child is not null) && this._hasVisualOverflow) && (!object.Equals(this.clipBehavior, Clip.none))))
        {
            global::Doroti.Flutter.Ui.Rect rect__12222 = (Offset.zero & size);
            this._clipRectLayer.layer = context.pushClipRect(needsCompositing, offset, rect__12222, (Action<PaintingContext, Offset>)base.paint, clipBehavior: this.clipBehavior, oldLayer: ((LayerHandle<ClipRectLayer>)this._clipRectLayer).layer);
        }
        else
        {
            this._clipRectLayer.layer = null;
            base.paint(context, offset);
        }
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? child__12682 = this.child;
        if ((child__12682 is null))
        {
            return null;
        }
        double? result__12770 = child__12682.getDryBaseline(constraints, baseline);
        if ((result__12770 is null))
        {
            return null;
        }
        global::Doroti.Flutter.Ui.Size childSize__13044 = child__12682.getDryLayout(constraints);
        global::Doroti.Flutter.Ui.Size mySize__13104 = getDryLayout(constraints);
        global::Doroti.Flutter.Ui.Offset offset__13157 = resolvedAlignment.alongOffset(((mySize__13104 - childSize__13044)));
        return (DartRuntimePrimitives.RequireValue(result__12770) + offset__13157.dy);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        this._clipRectLayer.layer = null;
        this._controller.dispose();
        this._animation.dispose();
        base.dispose();
    }

}

