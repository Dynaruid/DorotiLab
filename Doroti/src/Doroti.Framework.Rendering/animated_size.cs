// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/animated_size.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

public enum RenderAnimatedSizeState
{
    start,
    stable,
    changed,
    unstable,
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
    internal virtual LayerHandle<ClipRectLayer> _clipRectLayer { get; private set; } =
        new LayerHandle<ClipRectLayer>();

    public RenderAnimatedSize(
        TickerProvider vsync,
        Duration duration,
        Duration? reverseDuration = null,
        Curve curve = default!,
        AlignmentGeometry alignment = default!,
        TextDirection? textDirection = null,
        RenderBox? child = null,
        Clip clipBehavior = Clip.hardEdge,
        Action? onEnd = null
    )
        : base(alignment: alignment ?? Alignment.center, textDirection: textDirection, child: child)
    {
        Curve __curve = curve ?? Curves.linear;
        _vsync = vsync;
        _clipBehavior = clipBehavior;
        _onEnd = onEnd;
        _controller = new AnimationController(
            duration: duration,
            reverseDuration: reverseDuration,
            vsync: vsync
        );
        _controller.addListener(() =>
        {
            if (_controller.value != _lastValue)
            {
                markNeedsLayout();
            }
        });
        _animation = new CurvedAnimation(parent: _controller, curve: __curve);
    }

    public virtual AnimationController? debugController
    {
        get
        {
            AnimationController? controller = default!;
            DartRuntimePrimitives.Assert(() =>
            {
                controller = _controller;
                return true;
            });
            return controller;
        }
    }
    public virtual CurvedAnimation? debugAnimation
    {
        get
        {
            CurvedAnimation? animation = default!;
            DartRuntimePrimitives.Assert(() =>
            {
                animation = _animation;
                return true;
            });
            return animation;
        }
    }
    public virtual RenderAnimatedSizeState state => _state;
    public virtual Duration duration
    {
        get =>
            (
                _controller.duration
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        set
        {
            var __value = value;
            if (Equals((__value), _controller.duration))
            {
                return;
            }
            _controller.duration = (__value);
        }
    }
    public virtual Duration? reverseDuration
    {
        get => _controller.reverseDuration;
        set
        {
            var __value = value;
            if (Equals(__value, _controller.reverseDuration))
            {
                return;
            }
            _controller.reverseDuration = __value;
        }
    }
    public virtual Curve curve
    {
        get => _animation.curve;
        set
        {
            var __value = value;
            if (Equals(__value, _animation.curve))
            {
                return;
            }
            _animation.curve = __value;
        }
    }
    public virtual Clip clipBehavior
    {
        get => _clipBehavior;
        set
        {
            var __value = value;
            if (!Equals((__value), _clipBehavior))
            {
                _clipBehavior = (__value);
                markNeedsPaint();
                markNeedsSemanticsUpdate();
            }
        }
    }
    public virtual bool isAnimating => _controller.isAnimating;
    public virtual TickerProvider vsync
    {
        get => _vsync;
        set
        {
            var __value = value;
            if (Equals(__value, _vsync))
            {
                return;
            }
            _vsync = __value;
            _controller.resync(vsync);
        }
    }
    public virtual Action? onEnd
    {
        get => _onEnd;
        set
        {
            var __value = value;
            if (Equals(__value, _onEnd))
            {
                return;
            }
            _onEnd = __value;
        }
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        switch (state)
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
        _controller.addStatusListener(_animationStatusListener);
    }

    public override void detach()
    {
        _controller.stop();
        _controller.removeStatusListener(_animationStatusListener);
        base.detach();
    }

    internal virtual Size? _animatedSize
    {
        get { return _sizeTween.evaluate(_animation); }
    }

    public override void performLayout()
    {
        _lastValue = _controller.value;
        _hasVisualOverflow = false;
        BoxConstraints constraintsLocal = constraints;
        if ((child is null) || constraintsLocal.isTight)
        {
            _controller.stop();
            size = _currentSize = _sizeTween.begin = _sizeTween.end = constraintsLocal.smallest;
            _state = RenderAnimatedSizeState.start;
            child?.layout(constraintsLocal);
            return;
        }
        child!.layout(constraintsLocal, parentUsesSize: true);
        switch (_state)
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
        size = _currentSize = constraintsLocal.constrain(
            (
                _animatedSize
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            )
        );
        alignChild();
        if (
            (
                size.width
                < (
                    _sizeTween.end
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ).width
            )
            || (
                size.height
                < (
                    _sizeTween.end
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ).height
            )
        )
        {
            _hasVisualOverflow = true;
        }
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        if ((child is null) || constraints.isTight)
        {
            return constraints.smallest;
        }
        Size childSize = child!.getDryLayout(constraints);
        switch (_state)
        {
            case RenderAnimatedSizeState.start:
            {
                return constraints.constrain(childSize);
            }
            case RenderAnimatedSizeState.stable:
            {
                if (!Equals(_sizeTween.end, childSize))
                {
                    return constraints.constrain(_currentSize);
                }
                else
                {
                    if (_controller.value == _controller.upperBound)
                    {
                        return constraints.constrain(childSize);
                    }
                }
                break;
            }
            case RenderAnimatedSizeState.unstable:
            case RenderAnimatedSizeState.changed:
            {
                if (!Equals(_sizeTween.end, childSize))
                {
                    return constraints.constrain(childSize);
                }
                break;
            }
        }
        return constraints.constrain(
            (
                _animatedSize
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _restartAnimation()
    {
        _lastValue = 0.0;
        _controller.forward(from: 0.0);
    }

    internal virtual void _layoutStart()
    {
        _sizeTween.begin = _sizeTween.end = debugAdoptSize(child!.size);
        _state = RenderAnimatedSizeState.stable;
    }

    internal virtual void _layoutStable()
    {
        if (!Equals(_sizeTween.end, child!.size))
        {
            _sizeTween.begin = size;
            _sizeTween.end = debugAdoptSize(child!.size);
            _restartAnimation();
            _state = RenderAnimatedSizeState.changed;
        }
        else
        {
            if (_controller.value == _controller.upperBound)
            {
                _sizeTween.begin = _sizeTween.end = debugAdoptSize(child!.size);
            }
            else
            {
                if (!_controller.isAnimating)
                {
                    _controller.forward();
                }
            }
        }
    }

    internal virtual void _layoutChanged()
    {
        if (!Equals(_sizeTween.end, child!.size))
        {
            _sizeTween.begin = _sizeTween.end = debugAdoptSize(child!.size);
            _restartAnimation();
            _state = RenderAnimatedSizeState.unstable;
        }
        else
        {
            _state = RenderAnimatedSizeState.stable;
            if (!_controller.isAnimating)
            {
                _controller.forward();
            }
        }
    }

    internal virtual void _layoutUnstable()
    {
        if (!Equals(_sizeTween.end, child!.size))
        {
            _sizeTween.begin = _sizeTween.end = debugAdoptSize(child!.size);
            _restartAnimation();
        }
        else
        {
            _controller.stop();
            _state = RenderAnimatedSizeState.stable;
        }
    }

    internal virtual void _animationStatusListener(AnimationStatus status)
    {
        if (AnimationStatusMembers.isCompleted(status))
        {
            _onEnd?.Invoke();
        }
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if ((child is not null) && _hasVisualOverflow && (!Equals(clipBehavior, Clip.none)))
        {
            Rect rect = Offset.zero & size;
            _clipRectLayer.layer = context.pushClipRect(
                needsCompositing,
                offset,
                rect,
                base.paint,
                clipBehavior: clipBehavior,
                oldLayer: _clipRectLayer.layer
            );
        }
        else
        {
            _clipRectLayer.layer = null;
            base.paint(context, offset);
        }
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return null;
        }
        double? result = childLocal.getDryBaseline(constraints, baseline);
        if (result is null)
        {
            return null;
        }
        Size childSize = childLocal.getDryLayout(constraints);
        Size mySize = getDryLayout(constraints);
        Offset offset = resolvedAlignment.alongOffset(mySize - childSize);
        return (
                result
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ) + offset.dy;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        _clipRectLayer.layer = null;
        _controller.dispose();
        _animation.dispose();
        base.dispose();
    }
}
