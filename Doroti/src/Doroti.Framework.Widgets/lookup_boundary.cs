// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/lookup_boundary.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public class LookupBoundary : InheritedWidget
{
    public LookupBoundary(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!) : base(key: key, child: child)
    {
    }

    public static T? dependOnInheritedWidgetOfExactType<T>(BuildContext context, object? aspect = null) where T : InheritedWidget
    {
        context.dependOnInheritedWidgetOfExactType<LookupBoundary>();
        InheritedElement? candidate = ((InheritedElement?)getElementForInheritedWidgetOfExactType<T>(context));
        if ((candidate is null))
        {
            return default;
        }
        context.dependOnInheritedElement(candidate, aspect: aspect);
        return ((T?)candidate.widget)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static T? getInheritedWidgetOfExactType<T>(BuildContext context, object? aspect = null) where T : InheritedWidget
    {
        InheritedElement? candidate = ((InheritedElement?)getElementForInheritedWidgetOfExactType<T>(context));
        if ((candidate is null))
        {
            return default;
        }
        return ((T?)candidate.widget)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static InheritedElement? getElementForInheritedWidgetOfExactType<T>(BuildContext context) where T : InheritedWidget
    {
        InheritedElement? candidate = ((InheritedElement?)context.getElementForInheritedWidgetOfExactType<T>());
        if ((candidate is null))
        {
            return ((InheritedElement?)null);
        }
        Element? boundary = ((Element?)context.getElementForInheritedWidgetOfExactType<LookupBoundary>());
        if (((boundary is not null) && (((Element)boundary).depth > candidate.depth)))
        {
            return ((InheritedElement?)null);
        }
        return candidate;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static T? findAncestorWidgetOfExactType<T>(BuildContext context) where T : Widget
    {
        Element? target = default!;
        context.visitAncestorElements(((global::System.Func<Element, bool>)((ancestor) =>
        {
            if ((Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget), typeof(T))))
            {
                target = ancestor;
                return false;
            }
            return (!Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget), typeof(LookupBoundary)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        return ((T?)target?.widget)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static T? findAncestorStateOfType<T>(BuildContext context) where T : IState
    {
        StatefulElement? target = default!;
        context.visitAncestorElements(((global::System.Func<Element, bool>)((ancestor) =>
        {
            if (((ancestor is StatefulElement) && (((StatefulElement)((StatefulElement)ancestor)).state is T)))
            {
                target = ((StatefulElement)ancestor);
                return false;
            }
            return (!Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget), typeof(LookupBoundary)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        return ((T?)(object?)target?.state)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static T? findRootAncestorStateOfType<T>(BuildContext context) where T : IState
    {
        StatefulElement? target = default!;
        context.visitAncestorElements(((global::System.Func<Element, bool>)((ancestor) =>
        {
            if (((ancestor is StatefulElement) && (((StatefulElement)((StatefulElement)ancestor)).state is T)))
            {
                target = ((StatefulElement)ancestor);
            }
            return (!Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget), typeof(LookupBoundary)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        return ((T?)(object?)target?.state)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static T? findAncestorRenderObjectOfType<T>(BuildContext context) where T : global::Doroti.Framework.Rendering.RenderObject
    {
        Element? target = default!;
        context.visitAncestorElements(((global::System.Func<Element, bool>)((ancestor) =>
        {
            if (((ancestor is RenderObjectElement) && (((RenderObjectElement)((RenderObjectElement)ancestor)).renderObject is T)))
            {
                target = DartRuntimePrimitives.ConvertValue<Element>(((RenderObjectElement)ancestor));
                return false;
            }
            return (!Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget), typeof(LookupBoundary)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        return ((T?)target?.renderObject)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static void visitAncestorElements(BuildContext context, global::System.Func<Element, bool> visitor)
    {
        context.visitAncestorElements(((global::System.Func<Element, bool>)((ancestor) =>
        {
            return (visitor(ancestor) && (!Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget), typeof(LookupBoundary))));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
    }

    public static void visitChildElements(BuildContext context, global::System.Action<Element> visitor)
    {
        context.visitChildElements(((global::System.Action<Element>)((child) =>
        {
            if ((!Equals(DartRuntimePrimitives.RuntimeType(((Element)child).widget), typeof(LookupBoundary))))
            {
                visitor(child);
            }
        })));
    }

    public static bool debugIsHidingAncestorWidgetOfExactType<T>(BuildContext context) where T : Widget
    {
        bool? result = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                var hiddenByBoundary = false;
                var ancestorFound = false;
                context.visitAncestorElements(((global::System.Func<Element, bool>)((ancestor) =>
                {
                    if ((Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget), typeof(T))))
                    {
                        ancestorFound = true;
                        return false;
                    }
                    hiddenByBoundary = (hiddenByBoundary || (Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget), typeof(LookupBoundary))));
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                })));
                result = (ancestorFound & hiddenByBoundary);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return DartRuntimePrimitives.RequireValue(result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool debugIsHidingAncestorStateOfType<T>(BuildContext context) where T : IState
    {
        bool? result = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                var hiddenByBoundary = false;
                var ancestorFound = false;
                context.visitAncestorElements(((global::System.Func<Element, bool>)((ancestor) =>
                {
                    if (((ancestor is StatefulElement) && (((StatefulElement)((StatefulElement)ancestor)).state is T)))
                    {
                        ancestorFound = true;
                        return false;
                    }
                    hiddenByBoundary = (hiddenByBoundary || (Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget), typeof(LookupBoundary))));
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                })));
                result = (ancestorFound & hiddenByBoundary);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return DartRuntimePrimitives.RequireValue(result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool debugIsHidingAncestorRenderObjectOfType<T>(BuildContext context) where T : global::Doroti.Framework.Rendering.RenderObject
    {
        bool? result = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                var hiddenByBoundary = false;
                var ancestorFound = false;
                context.visitAncestorElements(((global::System.Func<Element, bool>)((ancestor) =>
                {
                    if (((ancestor is RenderObjectElement) && (((RenderObjectElement)((RenderObjectElement)ancestor)).renderObject is T)))
                    {
                        ancestorFound = true;
                        return false;
                    }
                    hiddenByBoundary = (hiddenByBoundary || (Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget), typeof(LookupBoundary))));
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                })));
                result = (ancestorFound & hiddenByBoundary);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return DartRuntimePrimitives.RequireValue(result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) => false;
}

