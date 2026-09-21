// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/drag_boundary.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public interface DragBoundaryDelegate<T>
{
    public bool isWithinBoundary(T draggedObject);
    public T nearestPositionWithinBoundary(T draggedObject);
}

internal class _DragBoundaryDelegateForRect__drag_boundary : DragBoundaryDelegate<Rect>
{
    public virtual Rect? boundary { get; private set; }

    internal _DragBoundaryDelegateForRect__drag_boundary(Rect? boundary)
    {
        this.boundary = boundary;
    }

    public virtual bool isWithinBoundary(Rect draggedObject)
    {
        if (boundary is null)
        {
            return true;
        }
        return (
                boundary
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ).contains(draggedObject.topLeft)
            && (
                boundary
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ).contains(draggedObject.bottomRight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Rect nearestPositionWithinBoundary(Rect draggedObject)
    {
        if (boundary is null)
        {
            return draggedObject;
        }
        if (
            (
                (
                    (
                        boundary
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ).right - draggedObject.width
                )
                < (
                    boundary
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ).left
            )
            || (
                (
                    (
                        boundary
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ).bottom - draggedObject.height
                )
                < (
                    boundary
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ).top
            )
        )
        {
            throw DartRuntimePrimitives.AsException(
                FlutterError.Create(
                    "The rect is larger than the boundary. "
                        + "The rect width must be less than the boundary width, and the rect height must be less than the boundary height."
                )
            );
        }
        double leftLocal = Dart_uiLibrary.clampDouble(
            draggedObject.left,
            (
                boundary
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ).left,
            (
                boundary
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ).right - draggedObject.width
        );
        double topLocal = Dart_uiLibrary.clampDouble(
            draggedObject.top,
            (
                boundary
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ).top,
            (
                boundary
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ).bottom - draggedObject.height
        );
        return Rect.fromLTWH(leftLocal, topLocal, draggedObject.width, draggedObject.height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class DragBoundary : InheritedWidget
{
    public DragBoundary(Widget child, Key? key = null)
        : base(child: child, key: key) { }

    public static DragBoundaryDelegate<Rect> forRectOf(
        BuildContext context,
        bool useGlobalPosition = true
    )
    {
        return forRectMaybeOf(context, useGlobalPosition: useGlobalPosition)
            ?? new _DragBoundaryDelegateForRect__drag_boundary(null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DragBoundaryDelegate<Rect>? forRectMaybeOf(
        BuildContext context,
        bool useGlobalPosition = true
    )
    {
        InheritedElement? element = context.getElementForInheritedWidgetOfExactType<DragBoundary>();
        if (element is null)
        {
            return null;
        }
        var rb = ((RenderBox?)element.findRenderObject())!;
        DartRuntimePrimitives.Assert(
            () => (rb is not null) && rb.hasSize,
            () => (object?)"DragBoundary is not available"
        );
        Rect boundary = useGlobalPosition
            ? Rect.fromPoints(
                rb!.localToGlobal(Offset.zero),
                rb.localToGlobal(rb.size.bottomRight(Offset.zero))
            )
            : (Offset.zero & rb!.size);
        return (DragBoundaryDelegate<Rect>?)
            new _DragBoundaryDelegateForRect__drag_boundary(boundary);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
