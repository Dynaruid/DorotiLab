// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/clip.dart
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public abstract class ClipContext
{
    public abstract Canvas canvas { get; }

    internal virtual void _clipAndPaint(
        Action<bool> canvasClipCall,
        Clip clipBehavior,
        Rect bounds,
        Action painter
    )
    {
        canvas.save();
        switch (clipBehavior)
        {
            case Clip.none:
            {
                break;
            }
            case Clip.hardEdge:
            {
                canvasClipCall(false);
                break;
            }
            case Clip.antiAlias:
            {
                canvasClipCall(true);
                break;
            }
            case Clip.antiAliasWithSaveLayer:
            {
                canvasClipCall(true);
                canvas.saveLayer(bounds, new Paint());
                break;
            }
        }
        painter();
        if (Equals(clipBehavior, Clip.antiAliasWithSaveLayer))
        {
            canvas.restore();
        }
        canvas.restore();
    }

    public virtual void clipPathAndPaint(Path path, Clip clipBehavior, Rect bounds, Action painter)
    {
        _clipAndPaint(
            (doAntiAlias) => canvas.clipPath(path, doAntiAlias: doAntiAlias),
            clipBehavior,
            bounds,
            painter
        );
    }

    public virtual void clipRRectAndPaint(
        RRect rrect,
        Clip clipBehavior,
        Rect bounds,
        Action painter
    )
    {
        _clipAndPaint(
            (doAntiAlias) => canvas.clipRRect(rrect, doAntiAlias: doAntiAlias),
            clipBehavior,
            bounds,
            painter
        );
    }

    public virtual void clipRSuperellipseAndPaint(
        RSuperellipse rse,
        Clip clipBehavior,
        Rect bounds,
        Action painter
    )
    {
        _clipAndPaint(
            (doAntiAlias) => canvas.clipRSuperellipse(rse, doAntiAlias: doAntiAlias),
            clipBehavior,
            bounds,
            painter
        );
    }

    public virtual void clipRectAndPaint(Rect rect, Clip clipBehavior, Rect bounds, Action painter)
    {
        _clipAndPaint(
            (doAntiAlias) => canvas.clipRect(rect, doAntiAlias: doAntiAlias),
            clipBehavior,
            bounds,
            painter
        );
    }
}
