// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/modal_barrier.dart
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

namespace Doroti.Generated.Framework.Widgets;

internal class _SemanticsClipper__modal_barrier : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Foundation.ValueNotifier<global::Doroti.Generated.Framework.Painting.EdgeInsets> clipDetailsNotifier { get; private set; } = default!;

    internal _SemanticsClipper__modal_barrier(Widget? child = null, global::Doroti.Generated.Framework.Foundation.ValueNotifier<global::Doroti.Generated.Framework.Painting.EdgeInsets> clipDetailsNotifier = default!) : base(child: child)
    {
        this.clipDetailsNotifier = clipDetailsNotifier;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _RenderSemanticsClipper__modal_barrier(clipDetailsNotifier: this.clipDetailsNotifier));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderSemanticsClipper__modal_barrier)(object)renderObject;
        __renderObject.clipDetailsNotifier = this.clipDetailsNotifier;
    }

}

public class _RenderSemanticsClipper__modal_barrier : global::Doroti.Generated.Framework.Rendering.RenderProxyBox
{
    internal virtual global::Doroti.Generated.Framework.Foundation.ValueNotifier<global::Doroti.Generated.Framework.Painting.EdgeInsets> _clipDetailsNotifier { get; set; } = default!;

    internal _RenderSemanticsClipper__modal_barrier(global::Doroti.Generated.Framework.Foundation.ValueNotifier<global::Doroti.Generated.Framework.Painting.EdgeInsets> clipDetailsNotifier, global::Doroti.Generated.Framework.Rendering.RenderBox? child = null) : base(child)
    {
        this._clipDetailsNotifier = clipDetailsNotifier;
    }

    public virtual global::Doroti.Generated.Framework.Foundation.ValueNotifier<global::Doroti.Generated.Framework.Painting.EdgeInsets> clipDetailsNotifier
    {
        get => this._clipDetailsNotifier;
        set
        {
            var newNotifier = value;
            if ((object.Equals(this._clipDetailsNotifier, newNotifier)))
            {
                return;
            }
            if (this.attached)
            {
                this._clipDetailsNotifier.removeListener(() => this.markNeedsSemanticsUpdate());
            }
            _clipDetailsNotifier = newNotifier;
            this._clipDetailsNotifier.addListener(() => this.markNeedsSemanticsUpdate());
            markNeedsSemanticsUpdate();
        }
    }
    public override Rect semanticBounds
    {
        get
        {
            global::Doroti.Generated.Framework.Painting.EdgeInsets clipDetails__3021 = ((global::Doroti.Generated.Framework.Painting.EdgeInsets)(object?)((global::Doroti.Generated.Framework.Foundation.ValueNotifier<global::Doroti.Generated.Framework.Painting.EdgeInsets>)this._clipDetailsNotifier).value);
            global::Doroti.Ui.Rect originalRect__3078 = ((global::Doroti.Ui.Rect)(object?)base.semanticBounds);
            var clippedRect__3125 = global::Doroti.Ui.Rect.fromLTRB((originalRect__3078.left + ((global::Doroti.Generated.Framework.Painting.EdgeInsets)clipDetails__3021).left), (originalRect__3078.top + ((global::Doroti.Generated.Framework.Painting.EdgeInsets)clipDetails__3021).top), (originalRect__3078.right - ((global::Doroti.Generated.Framework.Painting.EdgeInsets)clipDetails__3021).right), (originalRect__3078.bottom - ((global::Doroti.Generated.Framework.Painting.EdgeInsets)clipDetails__3021).bottom));
            return clippedRect__3125;
            return default!;
        }
    }
    public virtual void attach(global::Doroti.Generated.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        this.clipDetailsNotifier.addListener(() => this.markNeedsSemanticsUpdate());
    }

    public virtual void detach()
    {
        this.clipDetailsNotifier.removeListener(() => this.markNeedsSemanticsUpdate());
        base.detach();
    }

    public override void describeSemanticsConfiguration(global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.isSemanticBoundary = true;
    }

}

public class ModalBarrier : StatelessWidget
{
    public virtual Color? color { get; private set; }
    public virtual bool dismissible { get; private set; } = default!;
    public virtual global::System.Action? onDismiss { get; private set; }
    public virtual bool? barrierSemanticsDismissible { get; private set; }
    public virtual string? semanticsLabel { get; private set; }
    public virtual global::Doroti.Generated.Framework.Foundation.ValueNotifier<global::Doroti.Generated.Framework.Painting.EdgeInsets>? clipDetailsNotifier { get; private set; }
    public virtual string? semanticsOnTapHint { get; private set; }

    public ModalBarrier(global::Doroti.Generated.Framework.Foundation.Key? key = null, Color? color = null, bool dismissible = true, global::System.Action? onDismiss = null, string? semanticsLabel = null, bool? barrierSemanticsDismissible = true, global::Doroti.Generated.Framework.Foundation.ValueNotifier<global::Doroti.Generated.Framework.Painting.EdgeInsets>? clipDetailsNotifier = null, string? semanticsOnTapHint = null) : base(key: key)
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
        DartRuntimePrimitives.Assert(() => ((!this.dismissible || (this.semanticsLabel is null)) || global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context)));
        bool platformSupportsDismissingBarrier__7307 = default!;
        switch (global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
                {
                    platformSupportsDismissingBarrier__7307 = false;
                    break;
                }
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
                {
                    platformSupportsDismissingBarrier__7307 = true;
                    break;
                }
        }
        bool semanticsDismissible__7703 = (this.dismissible && platformSupportsDismissingBarrier__7307);
        bool modalBarrierSemanticsDismissible__7791 = (this.barrierSemanticsDismissible ?? semanticsDismissible__7703);
        void handleDismiss()
        {
            if (this.dismissible)
            {
                if ((this.onDismiss is not null))
                {
                    this.onDismiss!();
                }
                else
                {
                    DartRuntimePrimitives.Ignore(Navigator.maybePop<object>(context));
                }
            }
            else
            {
                DartRuntimePrimitives.Ignore(SystemSound.play(global::Doroti.Generated.Framework.Services.SystemSoundType.alert));
            }
        }
        Widget barrier__8153 = ((Widget)(object?)new Semantics(onTapHint: this.semanticsOnTapHint, onTap: ((global::System.Action)((semanticsDismissible__7703 && (this.semanticsLabel is not null)) ? handleDismiss : null)), onDismiss: ((global::System.Action)((semanticsDismissible__7703 && (this.semanticsLabel is not null)) ? handleDismiss : null)), label: (semanticsDismissible__7703 ? this.semanticsLabel : null), textDirection: ((semanticsDismissible__7703 && (this.semanticsLabel is not null)) ? Directionality.of(context) : null), child: new MouseRegion(cursor: global::Doroti.Generated.Framework.Services.SystemMouseCursors.basic, child: new ConstrainedBox(constraints: global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateExpand(), child: ((this.color is null) ? null : new ColoredBox(color: this.color!))))));
        bool excluding__9106 = (!semanticsDismissible__7703 || !modalBarrierSemanticsDismissible__7791);
        if ((!excluding__9106 && (this.clipDetailsNotifier is not null)))
        {
            barrier__8153 = DartRuntimePrimitives.ConvertValue<Widget>(new _SemanticsClipper__modal_barrier(clipDetailsNotifier: this.clipDetailsNotifier!, child: barrier__8153));
        }
        return ((Widget)(object?)new BlockSemantics(child: new ExcludeSemantics(excluding: excluding__9106, child: new _ModalBarrierGestureDetector__modal_barrier(onDismiss: () => handleDismiss(), child: barrier__8153))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class AnimatedModalBarrier : AnimatedWidget
{
    public virtual bool dismissible { get; private set; } = default!;
    public virtual string? semanticsLabel { get; private set; }
    public virtual bool? barrierSemanticsDismissible { get; private set; }
    public virtual global::System.Action? onDismiss { get; private set; }
    public virtual global::Doroti.Generated.Framework.Foundation.ValueNotifier<global::Doroti.Generated.Framework.Painting.EdgeInsets>? clipDetailsNotifier { get; private set; }
    public virtual string? semanticsOnTapHint { get; private set; }

    public AnimatedModalBarrier(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Animation.Animation<Color?> color = default!, bool dismissible = true, string? semanticsLabel = null, bool? barrierSemanticsDismissible = null, global::System.Action? onDismiss = null, global::Doroti.Generated.Framework.Foundation.ValueNotifier<global::Doroti.Generated.Framework.Painting.EdgeInsets>? clipDetailsNotifier = null, string? semanticsOnTapHint = null) : base(key: key, listenable: color)
    {
        this.dismissible = dismissible;
        this.semanticsLabel = semanticsLabel;
        this.barrierSemanticsDismissible = barrierSemanticsDismissible;
        this.onDismiss = onDismiss;
        this.clipDetailsNotifier = clipDetailsNotifier;
        this.semanticsOnTapHint = semanticsOnTapHint;
    }

    public virtual global::Doroti.Generated.Framework.Animation.Animation<global::Doroti.Ui.Color?> color => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Animation.Animation<global::Doroti.Ui.Color?>>(((global::Doroti.Generated.Framework.Animation.Animation<global::Doroti.Ui.Color?>?)(object?)this.listenable)!);
    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new ModalBarrier(color: ((global::Doroti.Generated.Framework.Animation.Animation<Color?>)this.color).value, dismissible: this.dismissible, semanticsLabel: this.semanticsLabel, barrierSemanticsDismissible: this.barrierSemanticsDismissible, onDismiss: () => this.onDismiss(), clipDetailsNotifier: this.clipDetailsNotifier, semanticsOnTapHint: this.semanticsOnTapHint));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _AnyTapGestureRecognizer__modal_barrier : global::Doroti.Generated.Framework.Gestures.BaseTapGestureRecognizer
{
    public virtual global::System.Action? onAnyTapUp { get; set; } = default;

    internal _AnyTapGestureRecognizer__modal_barrier()
    {
    }

    public override bool isPointerAllowed(global::Doroti.Generated.Framework.Gestures.PointerDownEvent @event)
    {
        if ((this.onAnyTapUp is null))
        {
            return false;
        }
        return base.isPointerAllowed(@event);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void handleTapDown(global::Doroti.Generated.Framework.Gestures.PointerDownEvent down = default!)
    {
    }

    public override void handleTapUp(global::Doroti.Generated.Framework.Gestures.PointerDownEvent down = default!, global::Doroti.Generated.Framework.Gestures.PointerUpEvent up = default!)
    {
        if ((this.onAnyTapUp is not null))
        {
            invokeCallback<object?>("onAnyTapUp", () => { ((Action)(this.onAnyTapUp!))(); return null; });
        }
    }

    public override void handleTapCancel(global::Doroti.Generated.Framework.Gestures.PointerDownEvent down = default!, global::Doroti.Generated.Framework.Gestures.PointerCancelEvent? cancel = null, string reason = default!)
    {
    }

    public override string debugDescription => "any tap";
}

internal class _AnyTapGestureRecognizerFactory__modal_barrier : GestureRecognizerFactory<_AnyTapGestureRecognizer__modal_barrier>
{
    public virtual global::System.Action? onAnyTapUp { get; private set; }

    internal _AnyTapGestureRecognizerFactory__modal_barrier(global::System.Action? onAnyTapUp = null)
    {
        this.onAnyTapUp = onAnyTapUp;
    }

    public override _AnyTapGestureRecognizer__modal_barrier constructor() => new _AnyTapGestureRecognizer__modal_barrier();
    public override void initializer(_AnyTapGestureRecognizer__modal_barrier instance)
    {
        instance.onAnyTapUp = (global::System.Action)this.onAnyTapUp;
    }

}

internal class _ModalBarrierGestureDetector__modal_barrier : StatelessWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual global::System.Action onDismiss { get; private set; } = default!;

    internal _ModalBarrierGestureDetector__modal_barrier(Widget child, global::System.Action onDismiss)
    {
        this.child = child;
        this.onDismiss = onDismiss;
    }

    public override Widget build(BuildContext context)
    {
        var gestures__14778 = new DartMap<Type, dynamic> { [typeof(_AnyTapGestureRecognizer__modal_barrier)] = new _AnyTapGestureRecognizerFactory__modal_barrier(onAnyTapUp: () => this.onDismiss()) };
        return ((Widget)(object?)new RawGestureDetector(gestures: gestures__14778, behavior: global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque, child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

