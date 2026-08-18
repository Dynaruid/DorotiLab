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
        InheritedElement? candidate__5191 = ((InheritedElement?)(object?)LookupBoundary.getElementForInheritedWidgetOfExactType<T>(context));
        if ((candidate__5191 is null))
        {
            return default;
        }
        context.dependOnInheritedElement(candidate__5191, aspect: aspect);
        return ((T?)(object?)candidate__5191.widget)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static T? getInheritedWidgetOfExactType<T>(BuildContext context, object? aspect = null) where T : InheritedWidget
    {
        InheritedElement? candidate__6239 = ((InheritedElement?)(object?)LookupBoundary.getElementForInheritedWidgetOfExactType<T>(context));
        if ((candidate__6239 is null))
        {
            return default;
        }
        return ((T?)(object?)candidate__6239.widget)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static InheritedElement? getElementForInheritedWidgetOfExactType<T>(BuildContext context) where T : InheritedWidget
    {
        InheritedElement? candidate__7325 = ((InheritedElement?)(object?)context.getElementForInheritedWidgetOfExactType<T>());
        if ((candidate__7325 is null))
        {
            return ((InheritedElement)(object)null);
        }
        Element? boundary__7464 = ((Element?)(object?)context.getElementForInheritedWidgetOfExactType<LookupBoundary>());
        if (((boundary__7464 is not null) && (((Element)boundary__7464).depth > candidate__7325.depth)))
        {
            return ((InheritedElement)(object)null);
        }
        return candidate__7325;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static T? findAncestorWidgetOfExactType<T>(BuildContext context) where T : Widget
    {
        Element? target__8413 = default!;
        context.visitAncestorElements(((global::System.Func<Element, bool>)((ancestor) =>
        {
            if ((object.Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget), typeof(T))))
            {
                target__8413 = ancestor;
                return false;
            }
            return (!object.Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget), typeof(LookupBoundary)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        return ((T?)(object?)target__8413?.widget)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static T? findAncestorStateOfType<T>(BuildContext context) where T : IState
    {
        StatefulElement? target__9446 = default!;
        context.visitAncestorElements(((global::System.Func<Element, bool>)((ancestor) =>
        {
            if (((ancestor is StatefulElement) && (((StatefulElement)((StatefulElement)ancestor)).state is T)))
            {
                target__9446 = ((StatefulElement)ancestor);
                return false;
            }
            return (!object.Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget), typeof(LookupBoundary)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        return ((T?)(object?)target__9446?.state)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static T? findRootAncestorStateOfType<T>(BuildContext context) where T : IState
    {
        StatefulElement? target__10438 = default!;
        context.visitAncestorElements(((global::System.Func<Element, bool>)((ancestor) =>
        {
            if (((ancestor is StatefulElement) && (((StatefulElement)((StatefulElement)ancestor)).state is T)))
            {
                target__10438 = ((StatefulElement)ancestor);
            }
            return (!object.Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget), typeof(LookupBoundary)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        return ((T?)(object?)target__10438?.state)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static T? findAncestorRenderObjectOfType<T>(BuildContext context) where T : global::Doroti.Framework.Rendering.RenderObject
    {
        Element? target__11491 = default!;
        context.visitAncestorElements(((global::System.Func<Element, bool>)((ancestor) =>
        {
            if (((ancestor is RenderObjectElement) && (((RenderObjectElement)((RenderObjectElement)ancestor)).renderObject is T)))
            {
                target__11491 = DartRuntimePrimitives.ConvertValue<Element>(((RenderObjectElement)ancestor));
                return false;
            }
            return (!object.Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget), typeof(LookupBoundary)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        return ((T?)(object?)target__11491?.renderObject)!;
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
        bool? result__13405 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                var hiddenByBoundary__13439 = false;
                var ancestorFound__13475 = false;
                context.visitAncestorElements(((global::System.Func<Element, bool>)((ancestor) =>
                {
                    if ((object.Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget), typeof(T))))
                    {
                        ancestorFound__13475 = true;
                        return false;
                    }
                    hiddenByBoundary__13439 = (hiddenByBoundary__13439 || (object.Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget), typeof(LookupBoundary))));
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                })));
                result__13405 = (ancestorFound__13475 & hiddenByBoundary__13439);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return DartRuntimePrimitives.RequireValue(result__13405);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool debugIsHidingAncestorStateOfType<T>(BuildContext context) where T : IState
    {
        bool? result__14214 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                var hiddenByBoundary__14248 = false;
                var ancestorFound__14284 = false;
                context.visitAncestorElements(((global::System.Func<Element, bool>)((ancestor) =>
                {
                    if (((ancestor is StatefulElement) && (((StatefulElement)((StatefulElement)ancestor)).state is T)))
                    {
                        ancestorFound__14284 = true;
                        return false;
                    }
                    hiddenByBoundary__14248 = (hiddenByBoundary__14248 || (object.Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget), typeof(LookupBoundary))));
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                })));
                result__14214 = (ancestorFound__14284 & hiddenByBoundary__14248);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return DartRuntimePrimitives.RequireValue(result__14214);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool debugIsHidingAncestorRenderObjectOfType<T>(BuildContext context) where T : global::Doroti.Framework.Rendering.RenderObject
    {
        bool? result__15081 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                var hiddenByBoundary__15115 = false;
                var ancestorFound__15151 = false;
                context.visitAncestorElements(((global::System.Func<Element, bool>)((ancestor) =>
                {
                    if (((ancestor is RenderObjectElement) && (((RenderObjectElement)((RenderObjectElement)ancestor)).renderObject is T)))
                    {
                        ancestorFound__15151 = true;
                        return false;
                    }
                    hiddenByBoundary__15115 = (hiddenByBoundary__15115 || (object.Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget), typeof(LookupBoundary))));
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                })));
                result__15081 = (ancestorFound__15151 & hiddenByBoundary__15115);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return DartRuntimePrimitives.RequireValue(result__15081);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) => false;
}

