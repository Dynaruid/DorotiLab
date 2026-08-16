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
        InheritedElement? model__6012 = ((InheritedElement?)(object?)context.getElementForInheritedWidgetOfExactType<T>());
        if ((model__6012 is null))
        {
            return;
        }
        results.Add(model__6012);
        DartRuntimePrimitives.Assert(() => (model__6012.widget is T));
        var modelWidget__6186 = ((T?)(object?)model__6012.widget)!;
        if (((bool)((dynamic)modelWidget__6186).isSupportedAspect(aspect)))
        {
            return;
        }
        Element? modelParent__6302 = default!;
        model__6012.visitAncestorElements(((global::System.Func<Element, bool>)((ancestor) => {
modelParent__6302 = ancestor;
return false;
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        if ((modelParent__6302 is null))
        {
            return;
        }
        InheritedModel<T>._findModels<T>(modelParent__6302!, aspect, results);
    }

    public static T? inheritFrom<T>(BuildContext context, object? aspect = null) where T : InheritedWidget
    {
        if ((aspect is null))
        {
            return ((T?)(object?)context.dependOnInheritedWidgetOfExactType<T>());
        }
        var models__7571 = new List<InheritedElement>();
        InheritedModel<T>._findModels<T>(context, aspect, models__7571);
        if (!System.Linq.Enumerable.Any(models__7571))
        {
            return default;
        }
        InheritedElement lastModel__7726 = models__7571.Last();
        foreach (var model__7766 in models__7571)
        {
            var value__7797 = ((T?)(object?)context.dependOnInheritedElement(model__7766, aspect: aspect))!;
            if ((object.Equals(model__7766, lastModel__7726)))
            {
                return value__7797;
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
        var dependencies__8322 = ((HashSet<T>?)(object?)getDependencies(dependent))!;
        if (((dependencies__8322 is not null) && !System.Linq.Enumerable.Any(dependencies__8322)))
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
{            var __cascade = ((dependencies__8322 ?? new HashSet<T>()));
            __cascade.Add(((T?)(object?)aspect)!);
            return __cascade;        }))());
        }
    }

    public override void notifyDependent(InheritedWidget oldWidget, Element dependent)
    {
        var __oldWidget = (InheritedModel<T>)(object)oldWidget;
        var dependencies__8757 = ((HashSet<T>?)(object?)getDependencies(dependent))!;
        if ((dependencies__8757 is null))
        {
            return;
        }
        if ((!System.Linq.Enumerable.Any(dependencies__8757) || (((InheritedModel<T>?)(object?)this.widget)!).updateShouldNotifyDependent(__oldWidget, dependencies__8757)))
        {
            dependent.didChangeDependencies();
        }
    }

}

