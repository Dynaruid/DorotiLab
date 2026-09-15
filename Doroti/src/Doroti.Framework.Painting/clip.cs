// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/clip.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Painting;

public abstract class ClipContext
{
    public abstract global::Doroti.Ui.Canvas canvas { get; }
    internal virtual void _clipAndPaint(Action<bool> canvasClipCall, Clip clipBehavior, Rect bounds, Action painter)
    {
        this.canvas.save();
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
                    this.canvas.saveLayer(bounds, new global::Doroti.Ui.Paint());
                    break;
                }
        }
        painter();
        if ((object.Equals(clipBehavior, Clip.antiAliasWithSaveLayer)))
        {
            this.canvas.restore();
        }
        this.canvas.restore();
    }

    public virtual void clipPathAndPaint(Path path, Clip clipBehavior, Rect bounds, Action painter)
    {
        _clipAndPaint(((Action<bool>)((doAntiAlias) => this.canvas.clipPath(path, doAntiAlias: doAntiAlias))), clipBehavior, bounds, (Action)painter);
    }

    public virtual void clipRRectAndPaint(RRect rrect, Clip clipBehavior, Rect bounds, Action painter)
    {
        _clipAndPaint(((Action<bool>)((doAntiAlias) => this.canvas.clipRRect(rrect, doAntiAlias: doAntiAlias))), clipBehavior, bounds, (Action)painter);
    }

    public virtual void clipRSuperellipseAndPaint(RSuperellipse rse, Clip clipBehavior, Rect bounds, Action painter)
    {
        _clipAndPaint(((Action<bool>)((doAntiAlias) => this.canvas.clipRSuperellipse(rse, doAntiAlias: doAntiAlias))), clipBehavior, bounds, (Action)painter);
    }

    public virtual void clipRectAndPaint(Rect rect, Clip clipBehavior, Rect bounds, Action painter)
    {
        _clipAndPaint(((Action<bool>)((doAntiAlias) => this.canvas.clipRect(rect, doAntiAlias: doAntiAlias))), clipBehavior, bounds, (Action)painter);
    }

}

