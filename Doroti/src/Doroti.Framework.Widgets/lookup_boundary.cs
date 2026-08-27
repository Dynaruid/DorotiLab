// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/lookup_boundary.dart
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

namespace Doroti.Framework.Widgets;

public class LookupBoundary : InheritedWidget
{
    public LookupBoundary(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!) : base(key: key, child: child)
    {
    }

    public static T? dependOnInheritedWidgetOfExactType<T>(BuildContext context, object? aspect = null) where T : InheritedWidget
    {
        context.dependOnInheritedWidgetOfExactType<LookupBoundary>();
        InheritedElement? candidate = ((InheritedElement?)(object?)LookupBoundary.getElementForInheritedWidgetOfExactType<T>(context));
        if ((candidate is null))
        {
            return default;
        }
        context.dependOnInheritedElement(candidate, aspect: aspect);
        return ((T?)(object?)candidate.widget)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static T? getInheritedWidgetOfExactType<T>(BuildContext context, object? aspect = null) where T : InheritedWidget
    {
        InheritedElement? candidate = ((InheritedElement?)(object?)LookupBoundary.getElementForInheritedWidgetOfExactType<T>(context));
        if ((candidate is null))
        {
            return default;
        }
        return ((T?)(object?)candidate.widget)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static InheritedElement? getElementForInheritedWidgetOfExactType<T>(BuildContext context) where T : InheritedWidget
    {
        InheritedElement? candidate = ((InheritedElement?)(object?)context.getElementForInheritedWidgetOfExactType<T>());
        if ((candidate is null))
        {
            return ((InheritedElement)(object)null);
        }
        Element? boundary = ((Element?)(object?)context.getElementForInheritedWidgetOfExactType<LookupBoundary>());
        if (((boundary is not null) && (((Element)boundary).depth > candidate.depth)))
        {
            return ((InheritedElement)(object)null);
        }
        return candidate;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static T? findAncestorWidgetOfExactType<T>(BuildContext context) where T : Widget
    {
        Element? target = default!;
        context.visitAncestorElements(((global::System.Func<Element, bool>)((ancestor) =>
        {
            if ((object.Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget), typeof(T))))
            {
                target = ancestor;
                return false;
            }
            return (!object.Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget), typeof(LookupBoundary)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        return ((T?)(object?)target?.widget)!;
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
            return (!object.Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget), typeof(LookupBoundary)));
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
            return (!object.Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget), typeof(LookupBoundary)));
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
            return (!object.Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget), typeof(LookupBoundary)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        return ((T?)(object?)target?.renderObject)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static void visitAncestorElements(BuildContext context, global::System.Func<Element, bool> visitor)
    {
        context.visitAncestorElements(((global::System.Func<Element, bool>)((ancestor) =>
        {
            return (visitor(ancestor) && (!object.Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget), typeof(LookupBoundary))));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
    }

    public static void visitChildElements(BuildContext context, global::System.Action<Element> visitor)
    {
        context.visitChildElements(((global::System.Action<Element>)((child) =>
        {
            if ((!object.Equals(DartRuntimePrimitives.RuntimeType(((Element)child).widget), typeof(LookupBoundary))))
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
                    if ((object.Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget), typeof(T))))
                    {
                        ancestorFound = true;
                        return false;
                    }
                    hiddenByBoundary = (hiddenByBoundary || (object.Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget), typeof(LookupBoundary))));
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
                    hiddenByBoundary = (hiddenByBoundary || (object.Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget), typeof(LookupBoundary))));
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
                    hiddenByBoundary = (hiddenByBoundary || (object.Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget), typeof(LookupBoundary))));
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

