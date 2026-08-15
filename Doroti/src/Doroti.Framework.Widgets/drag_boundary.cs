// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/drag_boundary.dart
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
            throw DartRuntimePrimitives.AsException(global::Doroti.Generated.Framework.Foundation.FlutterError.Create("The rect is larger than the boundary. " + "The rect width must be less than the boundary width, and the rect height must be less than the boundary height."));
        }
        double left__1867 = Dart_uiLibrary.clampDouble(draggedObject.left, DartRuntimePrimitives.RequireValue(this.boundary).left, (DartRuntimePrimitives.RequireValue(this.boundary).right - draggedObject.width));
        double top__2004 = Dart_uiLibrary.clampDouble(draggedObject.top, DartRuntimePrimitives.RequireValue(this.boundary).top, (DartRuntimePrimitives.RequireValue(this.boundary).bottom - draggedObject.height));
        return global::Doroti.Ui.Rect.fromLTWH(left__1867, top__2004, draggedObject.width, draggedObject.height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DragBoundary : InheritedWidget
{
    public DragBoundary(Widget child, global::Doroti.Generated.Framework.Foundation.Key? key = null) : base(child: child, key: key)
    {
    }

    public static DragBoundaryDelegate<global::Doroti.Ui.Rect> forRectOf(BuildContext context, bool useGlobalPosition = true)
    {
        return ((DragBoundaryDelegate<global::Doroti.Ui.Rect>)(object?)(DragBoundary.forRectMaybeOf(context, useGlobalPosition: useGlobalPosition) ?? new _DragBoundaryDelegateForRect__drag_boundary(((Rect)(object)null))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DragBoundaryDelegate<global::Doroti.Ui.Rect>? forRectMaybeOf(BuildContext context, bool useGlobalPosition = true)
    {
        InheritedElement? element__3844 = ((InheritedElement?)(object?)context.getElementForInheritedWidgetOfExactType<DragBoundary>());
        if ((element__3844 is null))
        {
            return ((DragBoundaryDelegate<global::Doroti.Ui.Rect>)(object)null);
        }
        var rb__3990 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)element__3844.findRenderObject())!;
        DartRuntimePrimitives.Assert(() => ((rb__3990 is not null) && ((global::Doroti.Generated.Framework.Rendering.RenderBox)rb__3990).hasSize), () => (object?)"DragBoundary is not available");
        global::Doroti.Ui.Rect boundary__4123 = ((global::Doroti.Ui.Rect)(object?)(useGlobalPosition ? global::Doroti.Ui.Rect.fromPoints(((Offset)((dynamic)rb__3990!).localToGlobal(Offset.zero)), ((Offset)((dynamic)rb__3990).localToGlobal(((global::Doroti.Generated.Framework.Rendering.RenderBox)rb__3990).size.bottomRight(Offset.zero)))) : (Offset.zero & rb__3990!.size)));
        return ((DragBoundaryDelegate<global::Doroti.Ui.Rect>?)(object?)new _DragBoundaryDelegateForRect__drag_boundary(boundary__4123));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

