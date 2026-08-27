// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/inherited_model.dart
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

public abstract class InheritedModel<T> : InheritedWidget where T : notnull
{
    protected InheritedModel(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!) : base(key: key, child: child)
    {
    }

    public override InheritedModelElement<T> createElement() => new InheritedModelElement<T>(this);
    public abstract bool updateShouldNotifyDependent(InheritedModel<T> oldWidget, HashSet<T> dependencies);
    public virtual bool isSupportedAspect(object aspect) => true;
    internal static void _findModels<T>(BuildContext context, object aspect, List<InheritedElement> results) where T : InheritedWidget
    {
        InheritedElement? model = ((InheritedElement?)(object?)context.getElementForInheritedWidgetOfExactType<T>());
        if ((model is null))
        {
            return;
        }
        results.Add(model);
        DartRuntimePrimitives.Assert(() => (model.widget is T));
        var modelWidget = ((T?)(object?)model.widget)!;
        if (((bool)((dynamic)modelWidget).isSupportedAspect(aspect)))
        {
            return;
        }
        Element? modelParent = default!;
        model.visitAncestorElements(((global::System.Func<Element, bool>)((ancestor) =>
        {
            modelParent = ancestor;
            return false;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        if ((modelParent is null))
        {
            return;
        }
        InheritedModel<T>._findModels<T>(modelParent!, aspect, results);
    }

    public static T? inheritFrom<T>(BuildContext context, object? aspect = null) where T : InheritedWidget
    {
        if ((aspect is null))
        {
            return ((T?)(object?)context.dependOnInheritedWidgetOfExactType<T>());
        }
        var models = new List<InheritedElement>();
        InheritedModel<T>._findModels<T>(context, aspect, models);
        if (!System.Linq.Enumerable.Any(models))
        {
            return default;
        }
        InheritedElement lastModel = models.Last();
        foreach (var model in models)
        {
            var value = ((T?)(object?)context.dependOnInheritedElement(model, aspect: aspect))!;
            if ((object.Equals(model, lastModel)))
            {
                return value;
            }
        }
        DartRuntimePrimitives.Assert(() => false);
        return default;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class InheritedModelElement<T> : InheritedElement
{
    public InheritedModelElement(InheritedModel<T> widget) : base(widget)
    {
    }

    public override void updateDependencies(Element dependent, object? aspect)
    {
        var dependencies = ((HashSet<T>?)(object?)getDependencies(dependent))!;
        if (((dependencies is not null) && !System.Linq.Enumerable.Any(dependencies)))
        {
            return;
        }
        if ((aspect is null))
        {
            setDependencies(dependent, new HashSet<T>());
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (aspect is T));
            setDependencies(dependent, ((Func<HashSet<T>>)(() =>
{
    var __cascade = ((dependencies ?? new HashSet<T>()));
    __cascade.Add(((T?)(object?)aspect)!);
    return __cascade;
}))());
        }
    }

    public override void notifyDependent(InheritedWidget oldWidget, Element dependent)
    {
        var __oldWidget = (InheritedModel<T>)(object)oldWidget;
        var dependencies = ((HashSet<T>?)(object?)getDependencies(dependent))!;
        if ((dependencies is null))
        {
            return;
        }
        if ((!System.Linq.Enumerable.Any(dependencies) || (((InheritedModel<T>?)(object?)this.widget)!).updateShouldNotifyDependent(__oldWidget, dependencies)))
        {
            dependent.didChangeDependencies();
        }
    }

}

