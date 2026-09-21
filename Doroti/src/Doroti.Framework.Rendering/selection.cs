// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/selection.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

public enum SelectionResult
{
    next,
    previous,
    end,
    pending,
    none,
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
        System.Diagnostics.Debug.Assert((startOffset >= 0L) && (endOffset >= 0L));
    }

    public override bool Equals(object? other)
    {
        var __other = other as SelectedContentRange;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is SelectedContentRange)
            && (__other.startOffset == startOffset)
            && (__other.endOffset == endOffset);
    }

    public override int GetHashCode()
    {
        return FoundationRuntimePorts.ObjectHash(startOffset, endOffset);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new IntProperty("startOffset", startOffset));
        properties.add(new IntProperty("endOffset", endOffset));
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
        properties.add(new StringProperty("plainText", plainText));
    }
}

public interface Selectable : SelectionHandler
{
    public Matrix4 getTransformTo(RenderObject? ancestor);
    public Size size { get; }
    public List<Rect> boundingBoxes { get; }
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
        if (point.dy < targetRect.top)
        {
            return SelectionResult.previous;
        }
        if (point.dy > targetRect.bottom)
        {
            return SelectionResult.next;
        }
        return (point.dx >= targetRect.right) ? SelectionResult.next : SelectionResult.previous;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Offset adjustDragOffset(
        Rect targetRect,
        Offset point,
        TextDirection direction = TextDirection.ltr
    )
    {
        if (targetRect.contains(point))
        {
            return point;
        }
        if (
            (point.dy <= targetRect.top)
            || ((point.dy <= targetRect.bottom) && (point.dx <= targetRect.left))
        )
        {
            return Equals(direction, TextDirection.ltr) ? targetRect.topLeft : targetRect.topRight;
        }
        else
        {
            return Equals(direction, TextDirection.ltr)
                ? targetRect.bottomRight
                : targetRect.bottomLeft;
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
    directionallyExtendSelection,
}

public enum TextGranularity
{
    character,
    word,
    paragraph,
    line,
    document,
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
    public SelectAllSelectionEvent()
        : base(SelectionEventType.selectAll) { }
}

public class ClearSelectionEvent : SelectionEvent
{
    public ClearSelectionEvent()
        : base(SelectionEventType.clear) { }
}

public class SelectWordSelectionEvent : SelectionEvent
{
    public virtual Offset globalPosition { get; private set; } = default!;

    public SelectWordSelectionEvent(Offset globalPosition)
        : base(SelectionEventType.selectWord)
    {
        this.globalPosition = globalPosition;
    }
}

public class SelectParagraphSelectionEvent : SelectionEvent
{
    public virtual Offset globalPosition { get; private set; } = default!;
    public virtual bool absorb { get; private set; } = default!;

    public SelectParagraphSelectionEvent(Offset globalPosition, bool absorb = false)
        : base(SelectionEventType.selectParagraph)
    {
        this.globalPosition = globalPosition;
        this.absorb = absorb;
    }
}

public class SelectionEdgeUpdateEvent : SelectionEvent
{
    public virtual Offset globalPosition { get; private set; } = default!;
    public virtual TextGranularity granularity { get; private set; } = default!;

    public SelectionEdgeUpdateEvent(Offset globalPosition, TextGranularity? granularity = null)
        : base(SelectionEventType.startEdgeUpdate)
    {
        this.globalPosition = globalPosition;
        this.granularity = granularity ?? TextGranularity.character;
    }

    public static SelectionEdgeUpdateEvent CreateForEnd(
        Offset globalPosition,
        TextGranularity? granularity = null
    )
    {
        var __instance = new SelectionEdgeUpdateEvent(globalPosition, granularity);
        __instance.globalPosition = globalPosition;
        __instance.granularity = granularity ?? TextGranularity.character;
        return __instance;
    }
}

public class GranularlyExtendSelectionEvent : SelectionEvent
{
    public virtual bool forward { get; private set; } = default!;
    public virtual bool isEnd { get; private set; } = default!;
    public virtual TextGranularity granularity { get; private set; } = default!;

    public GranularlyExtendSelectionEvent(bool forward, bool isEnd, TextGranularity granularity)
        : base(SelectionEventType.granularlyExtendSelection)
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
    backward,
}

public class DirectionallyExtendSelectionEvent : SelectionEvent
{
    public virtual double dx { get; private set; } = default!;
    public virtual bool isEnd { get; private set; } = default!;
    public virtual SelectionExtendDirection direction { get; private set; } = default!;

    public DirectionallyExtendSelectionEvent(
        double dx,
        bool isEnd,
        SelectionExtendDirection direction
    )
        : base(SelectionEventType.directionallyExtendSelection)
    {
        this.dx = dx;
        this.isEnd = isEnd;
        this.direction = direction;
    }

    public virtual DirectionallyExtendSelectionEvent copyWith(
        double? dx = null,
        bool? isEnd = null,
        SelectionExtendDirection? direction = null
    )
    {
        return new DirectionallyExtendSelectionEvent(
            dx: dx ?? this.dx,
            isEnd: isEnd ?? this.isEnd,
            direction: direction ?? this.direction
        );
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
    none,
}

public class SelectionGeometry : Diagnosticable
{
    public virtual SelectionPoint? startSelectionPoint { get; private set; }
    public virtual SelectionPoint? endSelectionPoint { get; private set; }
    public virtual SelectionStatus status { get; private set; } = default!;
    public virtual List<Rect> selectionRects { get; private set; } = default!;
    public virtual bool hasContent { get; private set; } = default!;

    public SelectionGeometry(
        SelectionPoint? startSelectionPoint = null,
        SelectionPoint? endSelectionPoint = null,
        List<Rect> selectionRects = default!,
        SelectionStatus status = default!,
        bool hasContent = default!
    )
    {
        List<Rect> __selectionRects = selectionRects ?? new List<Rect>();
        this.startSelectionPoint = startSelectionPoint;
        this.endSelectionPoint = endSelectionPoint;
        this.selectionRects = __selectionRects;
        this.status = status;
        this.hasContent = hasContent;
        System.Diagnostics.Debug.Assert(
            ((startSelectionPoint is null) && (endSelectionPoint is null))
                || (!Equals((status), SelectionStatus.none))
        );
    }

    public virtual bool hasSelection => !Equals(status, SelectionStatus.none);

    public virtual SelectionGeometry copyWith(
        SelectionPoint? startSelectionPoint = null,
        SelectionPoint? endSelectionPoint = null,
        List<Rect>? selectionRects = null,
        SelectionStatus? status = null,
        bool? hasContent = null
    )
    {
        return new SelectionGeometry(
            startSelectionPoint: startSelectionPoint ?? this.startSelectionPoint,
            endSelectionPoint: endSelectionPoint ?? this.endSelectionPoint,
            selectionRects: selectionRects ?? this.selectionRects,
            status: status ?? this.status,
            hasContent: hasContent ?? this.hasContent
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as SelectionGeometry;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is SelectionGeometry)
            && Equals(__other.startSelectionPoint, startSelectionPoint)
            && Equals(__other.endSelectionPoint, endSelectionPoint)
            && CollectionsLibrary.listEquals(__other.selectionRects, selectionRects)
            && Equals(__other.status, status)
            && (__other.hasContent == hasContent);
    }

    public override int GetHashCode()
    {
        return FoundationRuntimePorts.ObjectHash(
            startSelectionPoint,
            endSelectionPoint,
            selectionRects,
            status,
            hasContent
        );
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new DiagnosticsProperty<SelectionPoint>("startSelectionPoint", startSelectionPoint)
        );
        properties.add(
            new DiagnosticsProperty<SelectionPoint>("endSelectionPoint", endSelectionPoint)
        );
        properties.add(new IterableProperty<Rect>("selectionRects", selectionRects));
        properties.add(new EnumProperty<SelectionStatus>("status", status));
        properties.add(new DiagnosticsProperty<bool>("hasContent", hasContent));
    }
}

public class SelectionPoint : Diagnosticable
{
    public virtual Offset localPosition { get; private set; } = default!;
    public virtual double lineHeight { get; private set; } = default!;
    public virtual TextSelectionHandleType handleType { get; private set; } = default!;

    public SelectionPoint(
        Offset localPosition,
        double lineHeight,
        TextSelectionHandleType handleType
    )
    {
        this.localPosition = localPosition;
        this.lineHeight = lineHeight;
        this.handleType = handleType;
    }

    public override bool Equals(object? other)
    {
        var __other = other as SelectionPoint;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is SelectionPoint)
            && Equals(__other.localPosition, localPosition)
            && (__other.lineHeight == lineHeight)
            && Equals(__other.handleType, handleType);
    }

    public override int GetHashCode()
    {
        return FoundationRuntimePorts.ObjectHash(localPosition, lineHeight, handleType);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Offset>("localPosition", localPosition));
        properties.add(new DoubleProperty("lineHeight", lineHeight));
        properties.add(new EnumProperty<TextSelectionHandleType>("handleType", handleType));
    }
}

public enum TextSelectionHandleType
{
    left,
    right,
    collapsed,
}
