// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/clip.dart
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

namespace Doroti.Generated.Framework.Painting;

public abstract class ClipContext
{
    public abstract global::Doroti.Flutter.Ui.Canvas canvas { get; }
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
                    this.canvas.saveLayer(bounds, new global::Doroti.Flutter.Ui.Paint());
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

