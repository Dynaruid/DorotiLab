// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/lookup_boundary.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public class LookupBoundary : InheritedWidget
{
    public LookupBoundary(Key? key = null, Widget child = default!)
        : base(key: key, child: child) { }

    public static T? dependOnInheritedWidgetOfExactType<T>(
        BuildContext context,
        object? aspect = null
    )
        where T : InheritedWidget
    {
        context.dependOnInheritedWidgetOfExactType<LookupBoundary>();
        InheritedElement? candidate = getElementForInheritedWidgetOfExactType<T>(context);
        if (candidate is null)
        {
            return default;
        }
        context.dependOnInheritedElement(candidate, aspect: aspect);
        return ((T?)candidate.widget)!;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static T? getInheritedWidgetOfExactType<T>(BuildContext context, object? aspect = null)
        where T : InheritedWidget
    {
        InheritedElement? candidate = getElementForInheritedWidgetOfExactType<T>(context);
        if (candidate is null)
        {
            return default;
        }
        return ((T?)candidate.widget)!;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static InheritedElement? getElementForInheritedWidgetOfExactType<T>(BuildContext context)
        where T : InheritedWidget
    {
        InheritedElement? candidate = context.getElementForInheritedWidgetOfExactType<T>();
        if (candidate is null)
        {
            return null;
        }
        Element? boundary = context.getElementForInheritedWidgetOfExactType<LookupBoundary>();
        if ((boundary is not null) && (boundary.depth > candidate.depth))
        {
            return null;
        }
        return candidate;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static T? findAncestorWidgetOfExactType<T>(BuildContext context)
        where T : Widget
    {
        Element? target = default!;
        context.visitAncestorElements(
            (ancestor) =>
            {
                if (Equals(DartRuntimePrimitives.RuntimeType(ancestor.widget), typeof(T)))
                {
                    target = ancestor;
                    return false;
                }
                return !Equals(
                    DartRuntimePrimitives.RuntimeType(ancestor.widget),
                    typeof(LookupBoundary)
                );
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        return ((T?)target?.widget)!;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static T? findAncestorStateOfType<T>(BuildContext context)
        where T : IState
    {
        StatefulElement? target = default!;
        context.visitAncestorElements(
            (ancestor) =>
            {
                if ((ancestor is StatefulElement) && (((StatefulElement)ancestor).state is T))
                {
                    target = (StatefulElement)ancestor;
                    return false;
                }
                return !Equals(
                    DartRuntimePrimitives.RuntimeType(ancestor.widget),
                    typeof(LookupBoundary)
                );
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        return ((T?)(object?)target?.state)!;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static T? findRootAncestorStateOfType<T>(BuildContext context)
        where T : IState
    {
        StatefulElement? target = default!;
        context.visitAncestorElements(
            (ancestor) =>
            {
                if ((ancestor is StatefulElement) && (((StatefulElement)ancestor).state is T))
                {
                    target = (StatefulElement)ancestor;
                }
                return !Equals(
                    DartRuntimePrimitives.RuntimeType(ancestor.widget),
                    typeof(LookupBoundary)
                );
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        return ((T?)(object?)target?.state)!;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static T? findAncestorRenderObjectOfType<T>(BuildContext context)
        where T : RenderObject
    {
        Element? target = default!;
        context.visitAncestorElements(
            (ancestor) =>
            {
                if (
                    (ancestor is RenderObjectElement)
                    && (((RenderObjectElement)ancestor).renderObject is T)
                )
                {
                    target = DartRuntimePrimitives.ConvertValue<Element>(
                        (RenderObjectElement)ancestor
                    );
                    return false;
                }
                return !Equals(
                    DartRuntimePrimitives.RuntimeType(ancestor.widget),
                    typeof(LookupBoundary)
                );
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        return ((T?)target?.renderObject)!;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static void visitAncestorElements(BuildContext context, Func<Element, bool> visitor)
    {
        context.visitAncestorElements(
            (ancestor) =>
            {
                return visitor(ancestor)
                    && (
                        !Equals(
                            DartRuntimePrimitives.RuntimeType(ancestor.widget),
                            typeof(LookupBoundary)
                        )
                    );
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
    }

    public static void visitChildElements(BuildContext context, Action<Element> visitor)
    {
        context.visitChildElements(
            (child) =>
            {
                if (
                    !Equals(DartRuntimePrimitives.RuntimeType(child.widget), typeof(LookupBoundary))
                )
                {
                    visitor(child);
                }
            }
        );
    }

    public static bool debugIsHidingAncestorWidgetOfExactType<T>(BuildContext context)
        where T : Widget
    {
        bool? result = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            var hiddenByBoundary = false;
            var ancestorFound = false;
            context.visitAncestorElements(
                (ancestor) =>
                {
                    if (Equals(DartRuntimePrimitives.RuntimeType(ancestor.widget), typeof(T)))
                    {
                        ancestorFound = true;
                        return false;
                    }
                    hiddenByBoundary =
                        hiddenByBoundary
                        || Equals(
                            DartRuntimePrimitives.RuntimeType(ancestor.widget),
                            typeof(LookupBoundary)
                        );
                    return true;
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                }
            );
            result = ancestorFound & hiddenByBoundary;
            return true;
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        return (
            result ?? throw new global::System.NullReferenceException("A required value was null.")
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static bool debugIsHidingAncestorStateOfType<T>(BuildContext context)
        where T : IState
    {
        bool? result = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            var hiddenByBoundary = false;
            var ancestorFound = false;
            context.visitAncestorElements(
                (ancestor) =>
                {
                    if ((ancestor is StatefulElement) && (((StatefulElement)ancestor).state is T))
                    {
                        ancestorFound = true;
                        return false;
                    }
                    hiddenByBoundary =
                        hiddenByBoundary
                        || Equals(
                            DartRuntimePrimitives.RuntimeType(ancestor.widget),
                            typeof(LookupBoundary)
                        );
                    return true;
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                }
            );
            result = ancestorFound & hiddenByBoundary;
            return true;
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        return (
            result ?? throw new global::System.NullReferenceException("A required value was null.")
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static bool debugIsHidingAncestorRenderObjectOfType<T>(BuildContext context)
        where T : RenderObject
    {
        bool? result = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            var hiddenByBoundary = false;
            var ancestorFound = false;
            context.visitAncestorElements(
                (ancestor) =>
                {
                    if (
                        (ancestor is RenderObjectElement)
                        && (((RenderObjectElement)ancestor).renderObject is T)
                    )
                    {
                        ancestorFound = true;
                        return false;
                    }
                    hiddenByBoundary =
                        hiddenByBoundary
                        || Equals(
                            DartRuntimePrimitives.RuntimeType(ancestor.widget),
                            typeof(LookupBoundary)
                        );
                    return true;
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                }
            );
            result = ancestorFound & hiddenByBoundary;
            return true;
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        return (
            result ?? throw new global::System.NullReferenceException("A required value was null.")
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) => false;
}
