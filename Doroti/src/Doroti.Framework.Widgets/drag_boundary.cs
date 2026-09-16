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
        if ((this.boundary is null))
        {
            return true;
        }
        return (DartRuntimePrimitives.RequireValue(this.boundary).contains(draggedObject.topLeft) && DartRuntimePrimitives.RequireValue(this.boundary).contains(draggedObject.bottomRight));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Rect nearestPositionWithinBoundary(Rect draggedObject)
    {
        if ((this.boundary is null))
        {
            return draggedObject;
        }
        if ((((DartRuntimePrimitives.RequireValue(this.boundary).right - draggedObject.width) < DartRuntimePrimitives.RequireValue(this.boundary).left) || ((DartRuntimePrimitives.RequireValue(this.boundary).bottom - draggedObject.height) < DartRuntimePrimitives.RequireValue(this.boundary).top)))
        {
            throw DartRuntimePrimitives.AsException(FlutterError.Create("The rect is larger than the boundary. " + "The rect width must be less than the boundary width, and the rect height must be less than the boundary height."));
        }
        double leftLocal = Dart_uiLibrary.clampDouble(draggedObject.left, DartRuntimePrimitives.RequireValue(this.boundary).left, (DartRuntimePrimitives.RequireValue(this.boundary).right - draggedObject.width));
        double topLocal = Dart_uiLibrary.clampDouble(draggedObject.top, DartRuntimePrimitives.RequireValue(this.boundary).top, (DartRuntimePrimitives.RequireValue(this.boundary).bottom - draggedObject.height));
        return Rect.fromLTWH(leftLocal, topLocal, draggedObject.width, draggedObject.height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DragBoundary : InheritedWidget
{
    public DragBoundary(Widget child, global::Doroti.Framework.Foundation.Key? key = null) : base(child: child, key: key)
    {
    }

    public static DragBoundaryDelegate<global::Doroti.Ui.Rect> forRectOf(BuildContext context, bool useGlobalPosition = true)
    {
        return ((DragBoundaryDelegate<global::Doroti.Ui.Rect>)(forRectMaybeOf(context, useGlobalPosition: useGlobalPosition) ?? new _DragBoundaryDelegateForRect__drag_boundary(null)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DragBoundaryDelegate<global::Doroti.Ui.Rect>? forRectMaybeOf(BuildContext context, bool useGlobalPosition = true)
    {
        InheritedElement? element = ((InheritedElement?)context.getElementForInheritedWidgetOfExactType<DragBoundary>());
        if ((element is null))
        {
            return ((DragBoundaryDelegate<global::Doroti.Ui.Rect>?)null);
        }
        var rb = ((global::Doroti.Framework.Rendering.RenderBox?)element.findRenderObject())!;
        DartRuntimePrimitives.Assert(() => ((rb is not null) && ((global::Doroti.Framework.Rendering.RenderBox)rb).hasSize), () => (object?)"DragBoundary is not available");
        global::Doroti.Ui.Rect boundary = ((global::Doroti.Ui.Rect)(useGlobalPosition ? Rect.fromPoints(((Offset)(rb!).localToGlobal(Offset.zero)), ((Offset)(rb).localToGlobal(((global::Doroti.Framework.Rendering.RenderBox)rb).size.bottomRight(Offset.zero)))) : (Offset.zero & rb!.size)));
        return ((DragBoundaryDelegate<global::Doroti.Ui.Rect>?)new _DragBoundaryDelegateForRect__drag_boundary(boundary));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

