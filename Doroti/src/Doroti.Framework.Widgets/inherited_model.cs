// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/inherited_model.dart
#pragma warning disable CS0693, CS8714
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

internal interface IInheritedModelAspect
{
    bool isSupportedAspect(object aspect);
}

public abstract class InheritedModel<T> : InheritedWidget, IInheritedModelAspect where T : notnull
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
        if (((IInheritedModelAspect)modelWidget).isSupportedAspect(aspect))
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
