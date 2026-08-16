// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/ink_highlight.dart
using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Material;

public static partial class Ink_highlightLibrary
{
    internal static Duration _kDefaultHighlightFadeDuration = Duration.Create(milliseconds: 200L);
}

public class InkHighlight : InteractiveInkFeature
{
    internal virtual global::Doroti.Framework.Painting.BoxShape _shape { get; private set; } = default!;
    internal virtual double? _radius { get; private set; }
    internal virtual global::Doroti.Framework.Painting.BorderRadius _borderRadius { get; private set; } = default!;
    internal virtual global::System.Func<Rect>? _rectCallback { get; private set; }
    internal virtual TextDirection _textDirection { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<long> _alpha { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.AnimationController _alphaController { get; set; } = default!;
    internal virtual bool _active { get; set; } = true;

    public InkHighlight(MaterialInkController controller, global::Doroti.Framework.Rendering.RenderBox referenceBox, Color color, TextDirection textDirection, global::Doroti.Framework.Painting.BoxShape shape = global::Doroti.Framework.Painting.BoxShape.rectangle, double? radius = null, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Framework.Painting.ShapeBorder? customBorder = null, global::System.Func<Rect>? rectCallback = null, global::System.Action? onRemoved = null, Duration? fadeDuration = null) : base(controller: controller, referenceBox: referenceBox, color: color, customBorder: customBorder, onRemoved: onRemoved)
    {
        Duration __fadeDuration = fadeDuration ?? Ink_highlightLibrary._kDefaultHighlightFadeDuration;
        this._shape = shape;
        this._radius = radius;
        this._borderRadius = (borderRadius ?? global::Doroti.Framework.Painting.BorderRadius.zero);
        this._textDirection = textDirection;
        this._rectCallback = rectCallback;
        _alphaController = ((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{            var __cascade = new global::Doroti.Framework.Animation.AnimationController(duration: DartRuntimePrimitives.RequireValue(__fadeDuration), vsync: ((MaterialInkController)this.controller).vsync);
            __cascade.addListener(() => ((MaterialInkController)this.controller).markNeedsPaint());
            __cascade.addStatusListener((AnimationStatusListener)this._handleAlphaStatusChanged);
            __cascade.forward();
            return __cascade;        }))();
        _alpha = this._alphaController.drive(new global::Doroti.Framework.Animation.IntTween(begin: 0L, end: this.color.alpha));
        this.controller.addInkFeature(this);
    }

    public virtual bool active => this._active;
    public virtual void activate()
    {
        _active = true;
        this._alphaController.forward();
    }

    public virtual void deactivate()
    {
        _active = false;
        this._alphaController.reverse();
    }

    internal virtual void _handleAlphaStatusChanged(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        if ((global::Doroti.Framework.Animation.AnimationStatusMembers.isDismissed(status) && !this._active))
        {
            dispose();
        }
    }

    public override void dispose()
    {
        this._alphaController.dispose();
        base.dispose();
    }

    internal virtual void _paintHighlight(Canvas canvas, Rect rect, Paint paint)
    {
        canvas.save();
        if ((this.customBorder is not null))
        {
            canvas.clipPath(this.customBorder!.getOuterPath(rect, textDirection: this._textDirection));
        }
        switch (this._shape)
        {
            case global::Doroti.Framework.Painting.BoxShape.circle:
                {
                    canvas.drawCircle(((Offset)((dynamic)rect).center), (this._radius ?? Material.defaultSplashRadius), paint);
                    break;
                }
            case global::Doroti.Framework.Painting.BoxShape.rectangle:
                {
                    if ((!object.Equals(this._borderRadius, global::Doroti.Framework.Painting.BorderRadius.zero)))
                    {
                        var clipRRect__3915 = global::Doroti.Ui.RRect.fromRectAndCorners(rect, topLeft: ((global::Doroti.Framework.Painting.BorderRadius)this._borderRadius).topLeft, topRight: ((global::Doroti.Framework.Painting.BorderRadius)this._borderRadius).topRight, bottomLeft: ((global::Doroti.Framework.Painting.BorderRadius)this._borderRadius).bottomLeft, bottomRight: ((global::Doroti.Framework.Painting.BorderRadius)this._borderRadius).bottomRight);
                        canvas.drawRRect(clipRRect__3915, paint);
                    }
                    else
                    {
                        canvas.drawRect(rect, paint);
                    }
                    break;
                }
        }
        canvas.restore();
    }

    public override void paintFeature(Canvas canvas, Matrix4 transform)
    {
        var paint__4400 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = this.color.withAlpha(((global::Doroti.Framework.Animation.Animation<long>)this._alpha).value);
            return __cascade;        }))();
        global::Doroti.Ui.Offset? originOffset__4474 = ((global::Doroti.Ui.Offset?)(object?)MatrixUtils.getAsTranslation(transform));
        global::Doroti.Ui.Rect rect__4545 = ((global::Doroti.Ui.Rect)(object?)((this._rectCallback is not null) ? this._rectCallback() : (Offset.zero & ((global::Doroti.Framework.Rendering.RenderBox)this.referenceBox).size)));
        if ((originOffset__4474 is null))
        {
            canvas.save();
            canvas.transform(transform.storage);
            _paintHighlight(canvas, rect__4545, paint__4400);
            canvas.restore();
        }
        else
        {
            _paintHighlight(canvas, rect__4545.shift(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(originOffset__4474))), paint__4400);
        }
    }

}
