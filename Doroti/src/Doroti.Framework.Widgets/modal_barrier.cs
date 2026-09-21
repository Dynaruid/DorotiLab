// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/modal_barrier.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

internal class _SemanticsClipper__modal_barrier : SingleChildRenderObjectWidget
{
    public virtual ValueNotifier<EdgeInsets> clipDetailsNotifier { get; private set; } = default!;

    internal _SemanticsClipper__modal_barrier(
        Widget? child = null,
        ValueNotifier<EdgeInsets> clipDetailsNotifier = default!
    )
        : base(child: child)
    {
        this.clipDetailsNotifier = clipDetailsNotifier;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderSemanticsClipper__modal_barrier(clipDetailsNotifier: clipDetailsNotifier);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderSemanticsClipper__modal_barrier)renderObject;
        __renderObject.clipDetailsNotifier = clipDetailsNotifier;
    }
}

public class _RenderSemanticsClipper__modal_barrier : RenderProxyBox
{
    internal virtual ValueNotifier<EdgeInsets> _clipDetailsNotifier { get; set; } = default!;

    internal _RenderSemanticsClipper__modal_barrier(
        ValueNotifier<EdgeInsets> clipDetailsNotifier,
        RenderBox? child = null
    )
        : base(child)
    {
        _clipDetailsNotifier = clipDetailsNotifier;
    }

    public virtual ValueNotifier<EdgeInsets> clipDetailsNotifier
    {
        get => _clipDetailsNotifier;
        set
        {
            var newNotifier = value;
            if (Equals(_clipDetailsNotifier, newNotifier))
            {
                return;
            }
            if (attached)
            {
                _clipDetailsNotifier.removeListener(markNeedsSemanticsUpdate);
            }
            _clipDetailsNotifier = newNotifier;
            _clipDetailsNotifier.addListener(markNeedsSemanticsUpdate);
            markNeedsSemanticsUpdate();
        }
    }
    public override Rect semanticBounds
    {
        get
        {
            EdgeInsets clipDetails = _clipDetailsNotifier.value;
            Rect originalRect = base.semanticBounds;
            var clippedRect = Rect.fromLTRB(
                originalRect.left + clipDetails.left,
                originalRect.top + clipDetails.top,
                originalRect.right - clipDetails.right,
                originalRect.bottom - clipDetails.bottom
            );
            return clippedRect;
        }
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        clipDetailsNotifier.addListener(markNeedsSemanticsUpdate);
    }

    public override void detach()
    {
        clipDetailsNotifier.removeListener(markNeedsSemanticsUpdate);
        base.detach();
    }

    public override void describeSemanticsConfiguration(SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.isSemanticBoundary = true;
    }
}

public class ModalBarrier : StatelessWidget
{
    public virtual Color? color { get; private set; }
    public virtual bool dismissible { get; private set; } = default!;
    public virtual Action? onDismiss { get; private set; }
    public virtual bool? barrierSemanticsDismissible { get; private set; }
    public virtual string? semanticsLabel { get; private set; }
    public virtual ValueNotifier<EdgeInsets>? clipDetailsNotifier { get; private set; }
    public virtual string? semanticsOnTapHint { get; private set; }

    public ModalBarrier(
        Key? key = null,
        Color? color = null,
        bool dismissible = true,
        Action? onDismiss = null,
        string? semanticsLabel = null,
        bool? barrierSemanticsDismissible = true,
        ValueNotifier<EdgeInsets>? clipDetailsNotifier = null,
        string? semanticsOnTapHint = null
    )
        : base(key: key)
    {
        this.color = color;
        this.dismissible = dismissible;
        this.onDismiss = onDismiss;
        this.semanticsLabel = semanticsLabel;
        this.barrierSemanticsDismissible = barrierSemanticsDismissible;
        this.clipDetailsNotifier = clipDetailsNotifier;
        this.semanticsOnTapHint = semanticsOnTapHint;
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            !dismissible
            || (semanticsLabel is null)
            || DebugLibrary.debugCheckHasDirectionality(context)
        );
        bool platformSupportsDismissingBarrier = default!;
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
            {
                platformSupportsDismissingBarrier = false;
                break;
            }
            case TargetPlatform.android:
            case TargetPlatform.iOS:
            case TargetPlatform.macOS:
            {
                platformSupportsDismissingBarrier = true;
                break;
            }
        }
        bool semanticsDismissible = dismissible && platformSupportsDismissingBarrier;
        bool modalBarrierSemanticsDismissible = barrierSemanticsDismissible ?? semanticsDismissible;
        void handleDismiss()
        {
            if (dismissible)
            {
                if (onDismiss is not null)
                {
                    onDismiss!();
                }
                else
                {
                    DartRuntimePrimitives.Ignore(Navigator.maybePop<object>(context));
                }
            }
            else
            {
                DartRuntimePrimitives.Ignore(SystemSound.play(SystemSoundType.alert));
            }
        }
        Widget barrier = new Semantics(
            onTapHint: semanticsOnTapHint,
            onTap: (semanticsDismissible && (semanticsLabel is not null)) ? handleDismiss : null,
            onDismiss: (semanticsDismissible && (semanticsLabel is not null))
                ? handleDismiss
                : null,
            label: semanticsDismissible ? semanticsLabel : null,
            textDirection: (semanticsDismissible && (semanticsLabel is not null))
                ? Directionality.of(context)
                : null,
            child: new MouseRegion(
                cursor: SystemMouseCursors.basic,
                child: new ConstrainedBox(
                    constraints: BoxConstraints.CreateExpand(),
                    child: (color is null) ? null : new ColoredBox(color: color!)
                )
            )
        );
        bool excludingLocal = !semanticsDismissible || !modalBarrierSemanticsDismissible;
        if (!excludingLocal && (clipDetailsNotifier is not null))
        {
            barrier = DartRuntimePrimitives.ConvertValue<Widget>(
                new _SemanticsClipper__modal_barrier(
                    clipDetailsNotifier: clipDetailsNotifier!,
                    child: barrier
                )
            );
        }
        return new BlockSemantics(
            child: new ExcludeSemantics(
                excluding: excludingLocal,
                child: new _ModalBarrierGestureDetector__modal_barrier(
                    onDismiss: () => handleDismiss(),
                    child: barrier
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class AnimatedModalBarrier : AnimatedWidget
{
    public virtual bool dismissible { get; private set; } = default!;
    public virtual string? semanticsLabel { get; private set; }
    public virtual bool? barrierSemanticsDismissible { get; private set; }
    public virtual Action? onDismiss { get; private set; }
    public virtual ValueNotifier<EdgeInsets>? clipDetailsNotifier { get; private set; }
    public virtual string? semanticsOnTapHint { get; private set; }

    public AnimatedModalBarrier(
        Key? key = null,
        Animation<Color?> color = default!,
        bool dismissible = true,
        string? semanticsLabel = null,
        bool? barrierSemanticsDismissible = null,
        Action? onDismiss = null,
        ValueNotifier<EdgeInsets>? clipDetailsNotifier = null,
        string? semanticsOnTapHint = null
    )
        : base(key: key, listenable: color)
    {
        this.dismissible = dismissible;
        this.semanticsLabel = semanticsLabel;
        this.barrierSemanticsDismissible = barrierSemanticsDismissible;
        this.onDismiss = onDismiss;
        this.clipDetailsNotifier = clipDetailsNotifier;
        this.semanticsOnTapHint = semanticsOnTapHint;
    }

    public virtual Animation<Color?> color =>
        DartRuntimePrimitives.ConvertValue<Animation<Color?>>(((Animation<Color?>?)listenable)!);

    public override Widget build(BuildContext context)
    {
        return new ModalBarrier(
            color: color.value,
            dismissible: dismissible,
            semanticsLabel: semanticsLabel,
            barrierSemanticsDismissible: barrierSemanticsDismissible,
            onDismiss: onDismiss,
            clipDetailsNotifier: clipDetailsNotifier,
            semanticsOnTapHint: semanticsOnTapHint
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class _AnyTapGestureRecognizer__modal_barrier : BaseTapGestureRecognizer
{
    public virtual Action? onAnyTapUp { get; set; } = default;

    internal _AnyTapGestureRecognizer__modal_barrier() { }

    public override bool isPointerAllowed(Gestures.PointerDownEvent @event)
    {
        if (onAnyTapUp is null)
        {
            return false;
        }
        return base.isPointerAllowed(@event);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void handleTapDown(Gestures.PointerDownEvent down = default!) { }

    public override void handleTapUp(
        Gestures.PointerDownEvent down = default!,
        Gestures.PointerUpEvent up = default!
    )
    {
        if (onAnyTapUp is not null)
        {
            invokeCallback<object?>(
                "onAnyTapUp",
                () =>
                {
                    onAnyTapUp!();
                    return null;
                }
            );
        }
    }

    public override void handleTapCancel(
        Gestures.PointerDownEvent down = default!,
        Gestures.PointerCancelEvent? cancel = null,
        string reason = default!
    ) { }

    public override string debugDescription => "any tap";
}

internal class _AnyTapGestureRecognizerFactory__modal_barrier
    : GestureRecognizerFactory<_AnyTapGestureRecognizer__modal_barrier>
{
    public virtual Action? onAnyTapUp { get; private set; }

    internal _AnyTapGestureRecognizerFactory__modal_barrier(Action? onAnyTapUp = null)
    {
        this.onAnyTapUp = onAnyTapUp;
    }

    public override _AnyTapGestureRecognizer__modal_barrier constructor() =>
        new _AnyTapGestureRecognizer__modal_barrier();

    public override void initializer(_AnyTapGestureRecognizer__modal_barrier instance)
    {
        instance.onAnyTapUp = onAnyTapUp;
    }
}

internal class _ModalBarrierGestureDetector__modal_barrier : StatelessWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual Action onDismiss { get; private set; } = default!;

    internal _ModalBarrierGestureDetector__modal_barrier(Widget child, Action onDismiss)
    {
        this.child = child;
        this.onDismiss = onDismiss;
    }

    public override Widget build(BuildContext context)
    {
        var gesturesLocal = new DartMap<Type, dynamic>
        {
            [typeof(_AnyTapGestureRecognizer__modal_barrier)] =
                new _AnyTapGestureRecognizerFactory__modal_barrier(onAnyTapUp: () => onDismiss()),
        };
        return new RawGestureDetector(
            gestures: gesturesLocal,
            behavior: HitTestBehavior.opaque,
            child: child
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
