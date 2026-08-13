// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/selection.dart
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

public enum SelectionResult
{
    next,
    previous,
    end,
    pending,
    none
}

public interface SelectionHandler : ValueListenable<SelectionGeometry>
{
    public void pushHandleLayers(LayerLink? startHandle, LayerLink? endHandle);
    public SelectedContent? getSelectedContent();
    public SelectedContentRange? getSelection();
    public SelectionResult dispatchSelectionEvent(SelectionEvent @event);
    public long contentLength { get; }
}

public class SelectedContentRange : Diagnosticable
{
    public virtual long startOffset { get; private set; } = default!;
    public virtual long endOffset { get; private set; } = default!;

    public SelectedContentRange(long startOffset, long endOffset)
    {
        this.startOffset = startOffset;
        this.endOffset = endOffset;
        System.Diagnostics.Debug.Assert((((startOffset >= 0L) && (endOffset >= 0L))));
    }

    public override bool Equals(object? other)
    {
        var __other = other as SelectedContentRange;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is SelectedContentRange) && (((SelectedContentRange)((SelectedContentRange)__other)).startOffset == this.startOffset)) && (((SelectedContentRange)((SelectedContentRange)__other)).endOffset == this.endOffset));
    }

    public override int GetHashCode()
    {
        return FoundationRuntimePorts.ObjectHash(this.startOffset, this.endOffset);
        return default!;
    }
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new IntProperty("startOffset", this.startOffset));
        properties.add(new IntProperty("endOffset", this.endOffset));
    }

}

public class SelectedContent : Diagnosticable
{
    public virtual string plainText { get; private set; } = default!;

    public SelectedContent(string plainText)
    {
        this.plainText = plainText;
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new StringProperty("plainText", this.plainText));
    }

}

public interface Selectable : SelectionHandler
{
    public Matrix4 getTransformTo(RenderObject? ancestor);
    public global::Doroti.Flutter.Ui.Size size { get; }
    public List<global::Doroti.Flutter.Ui.Rect> boundingBoxes { get; }
    public void dispose();
}

public interface SelectionRegistrant : Listenable, Selectable
{
    public SelectionRegistrar? registrar { get; set; }
}

public abstract class SelectionUtils
{
    public static SelectionResult getResultBasedOnRect(Rect targetRect, Offset point)
    {
        if (targetRect.contains(point))
        {
            return SelectionResult.end;
        }
        if ((point.dy < targetRect.top))
        {
            return SelectionResult.previous;
        }
        if ((point.dy > targetRect.bottom))
        {
            return SelectionResult.next;
        }
        return ((point.dx >= targetRect.right) ? SelectionResult.next : SelectionResult.previous);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Flutter.Ui.Offset adjustDragOffset(Rect targetRect, Offset point, TextDirection direction = TextDirection.ltr)
    {
        if (targetRect.contains(point))
        {
            return point;
        }
        if (((point.dy <= targetRect.top) || ((point.dy <= targetRect.bottom) && (point.dx <= targetRect.left))))
        {
            return ((object.Equals(direction, TextDirection.ltr)) ? targetRect.topLeft : targetRect.topRight);
        }
        else
        {
            return ((object.Equals(direction, TextDirection.ltr)) ? targetRect.bottomRight : targetRect.bottomLeft);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public enum SelectionEventType
{
    startEdgeUpdate,
    endEdgeUpdate,
    clear,
    selectAll,
    selectWord,
    selectParagraph,
    granularlyExtendSelection,
    directionallyExtendSelection
}

public enum TextGranularity
{
    character,
    word,
    paragraph,
    line,
    document
}

public abstract class SelectionEvent
{
    public virtual SelectionEventType type { get; private set; } = default!;

    protected SelectionEvent(SelectionEventType type)
    {
        this.type = type;
    }

}

public class SelectAllSelectionEvent : SelectionEvent
{
    public SelectAllSelectionEvent() : base(SelectionEventType.selectAll)
    {
    }

}

public class ClearSelectionEvent : SelectionEvent
{
    public ClearSelectionEvent() : base(SelectionEventType.clear)
    {
    }

}

public class SelectWordSelectionEvent : SelectionEvent
{
    public virtual Offset globalPosition { get; private set; } = default!;

    public SelectWordSelectionEvent(Offset globalPosition) : base(SelectionEventType.selectWord)
    {
        this.globalPosition = globalPosition;
    }

}

public class SelectParagraphSelectionEvent : SelectionEvent
{
    public virtual Offset globalPosition { get; private set; } = default!;
    public virtual bool absorb { get; private set; } = default!;

    public SelectParagraphSelectionEvent(Offset globalPosition, bool absorb = false) : base(SelectionEventType.selectParagraph)
    {
        this.globalPosition = globalPosition;
        this.absorb = absorb;
    }

}

public class SelectionEdgeUpdateEvent : SelectionEvent
{
    public virtual Offset globalPosition { get; private set; } = default!;
    public virtual TextGranularity granularity { get; private set; } = default!;

    public SelectionEdgeUpdateEvent(Offset globalPosition, TextGranularity? granularity = null) : base(SelectionEventType.startEdgeUpdate)
    {
        this.globalPosition = globalPosition;
        this.granularity = (granularity ?? TextGranularity.character);
    }

    public static SelectionEdgeUpdateEvent CreateForEnd(Offset globalPosition, TextGranularity? granularity = null)
    {
        var __instance = new SelectionEdgeUpdateEvent(default!, default!);
        __instance.globalPosition = globalPosition;
        __instance.granularity = (granularity ?? TextGranularity.character);
        return __instance;
    }

}

public class GranularlyExtendSelectionEvent : SelectionEvent
{
    public virtual bool forward { get; private set; } = default!;
    public virtual bool isEnd { get; private set; } = default!;
    public virtual TextGranularity granularity { get; private set; } = default!;

    public GranularlyExtendSelectionEvent(bool forward, bool isEnd, TextGranularity granularity) : base(SelectionEventType.granularlyExtendSelection)
    {
        this.forward = forward;
        this.isEnd = isEnd;
        this.granularity = granularity;
    }

}

public enum SelectionExtendDirection
{
    previousLine,
    nextLine,
    forward,
    backward
}

public class DirectionallyExtendSelectionEvent : SelectionEvent
{
    public virtual double dx { get; private set; } = default!;
    public virtual bool isEnd { get; private set; } = default!;
    public virtual SelectionExtendDirection direction { get; private set; } = default!;

    public DirectionallyExtendSelectionEvent(double dx, bool isEnd, SelectionExtendDirection direction) : base(SelectionEventType.directionallyExtendSelection)
    {
        this.dx = dx;
        this.isEnd = isEnd;
        this.direction = direction;
    }

    public virtual DirectionallyExtendSelectionEvent copyWith(double? dx = null, bool? isEnd = null, SelectionExtendDirection? direction = null)
    {
        return new DirectionallyExtendSelectionEvent(dx: (dx ?? this.dx), isEnd: (isEnd ?? this.isEnd), direction: (direction ?? this.direction));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public interface SelectionRegistrar
{
    public void add(Selectable selectable);
    public void remove(Selectable selectable);
}

public enum SelectionStatus
{
    uncollapsed,
    collapsed,
    none
}

public class SelectionGeometry : Diagnosticable
{
    public virtual SelectionPoint? startSelectionPoint { get; private set; }
    public virtual SelectionPoint? endSelectionPoint { get; private set; }
    public virtual SelectionStatus status { get; private set; } = default!;
    public virtual List<Rect> selectionRects { get; private set; } = default!;
    public virtual bool hasContent { get; private set; } = default!;

    public SelectionGeometry(SelectionPoint? startSelectionPoint = null, SelectionPoint? endSelectionPoint = null, List<Rect> selectionRects = default!, SelectionStatus status = default!, bool hasContent = default!)
    {
        List<Rect> __selectionRects = selectionRects ?? new List<Rect>();
        this.startSelectionPoint = startSelectionPoint;
        this.endSelectionPoint = endSelectionPoint;
        this.selectionRects = __selectionRects;
        this.status = status;
        this.hasContent = hasContent;
        System.Diagnostics.Debug.Assert(((((startSelectionPoint is null) && (endSelectionPoint is null))) || (!object.Equals(DartRuntimePrimitives.RequireValue(status), SelectionStatus.none))));
    }

    public virtual bool hasSelection => (!object.Equals(this.status, SelectionStatus.none));
    public virtual SelectionGeometry copyWith(SelectionPoint? startSelectionPoint = null, SelectionPoint? endSelectionPoint = null, List<Rect>? selectionRects = null, SelectionStatus? status = null, bool? hasContent = null)
    {
        return new SelectionGeometry(startSelectionPoint: (startSelectionPoint ?? this.startSelectionPoint), endSelectionPoint: (endSelectionPoint ?? this.endSelectionPoint), selectionRects: (selectionRects ?? this.selectionRects), status: (status ?? this.status), hasContent: (hasContent ?? this.hasContent));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as SelectionGeometry;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((__other is SelectionGeometry) && (object.Equals(((SelectionGeometry)((SelectionGeometry)__other)).startSelectionPoint, this.startSelectionPoint))) && (object.Equals(((SelectionGeometry)((SelectionGeometry)__other)).endSelectionPoint, this.endSelectionPoint))) && global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.listEquals(((SelectionGeometry)((SelectionGeometry)__other)).selectionRects, this.selectionRects)) && (object.Equals(((SelectionGeometry)((SelectionGeometry)__other)).status, this.status))) && (((SelectionGeometry)((SelectionGeometry)__other)).hasContent == this.hasContent));
    }

    public override int GetHashCode()
    {
        return FoundationRuntimePorts.ObjectHash(this.startSelectionPoint, this.endSelectionPoint, this.selectionRects, this.status, this.hasContent);
        return default!;
    }
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<SelectionPoint>("startSelectionPoint", this.startSelectionPoint));
        properties.add(new DiagnosticsProperty<SelectionPoint>("endSelectionPoint", this.endSelectionPoint));
        properties.add(new IterableProperty<global::Doroti.Flutter.Ui.Rect>("selectionRects", this.selectionRects));
        properties.add(new EnumProperty<SelectionStatus>("status", this.status));
        properties.add(new DiagnosticsProperty<bool>("hasContent", this.hasContent));
    }

}

public class SelectionPoint : Diagnosticable
{
    public virtual Offset localPosition { get; private set; } = default!;
    public virtual double lineHeight { get; private set; } = default!;
    public virtual TextSelectionHandleType handleType { get; private set; } = default!;

    public SelectionPoint(Offset localPosition, double lineHeight, TextSelectionHandleType handleType)
    {
        this.localPosition = localPosition;
        this.lineHeight = lineHeight;
        this.handleType = handleType;
    }

    public override bool Equals(object? other)
    {
        var __other = other as SelectionPoint;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((__other is SelectionPoint) && (object.Equals(((SelectionPoint)((SelectionPoint)__other)).localPosition, this.localPosition))) && (((SelectionPoint)((SelectionPoint)__other)).lineHeight == this.lineHeight)) && (object.Equals(((SelectionPoint)((SelectionPoint)__other)).handleType, this.handleType)));
    }

    public override int GetHashCode()
    {
        return FoundationRuntimePorts.ObjectHash(this.localPosition, this.lineHeight, this.handleType);
        return default!;
    }
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Flutter.Ui.Offset>("localPosition", this.localPosition));
        properties.add(new DoubleProperty("lineHeight", this.lineHeight));
        properties.add(new EnumProperty<TextSelectionHandleType>("handleType", this.handleType));
    }

}

public enum TextSelectionHandleType
{
    left,
    right,
    collapsed
}
