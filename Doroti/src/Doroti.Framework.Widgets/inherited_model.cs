// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/inherited_model.dart

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
    internal static void _findModels<TModel>(BuildContext context, object aspect, List<InheritedElement> results) where TModel : InheritedWidget
    {
        InheritedElement? model = ((InheritedElement?)context.getElementForInheritedWidgetOfExactType<TModel>());
        if ((model is null))
        {
            return;
        }
        results.Add(model);
        DartRuntimePrimitives.Assert(() => (model.widget is TModel));
        var modelWidget = ((TModel?)model.widget)!;
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
        _findModels<TModel>(modelParent, aspect, results);
    }

    public static TModel? inheritFrom<TModel>(BuildContext context, object? aspect = null) where TModel : InheritedWidget
    {
        if ((aspect is null))
        {
            return ((TModel?)context.dependOnInheritedWidgetOfExactType<TModel>());
        }
        var models = new List<InheritedElement>();
        _findModels<TModel>(context, aspect, models);
        if (!Enumerable.Any(models))
        {
            return default;
        }
        InheritedElement lastModel = models.Last();
        foreach (var model in models)
        {
            var value = ((TModel?)context.dependOnInheritedElement(model, aspect: aspect))!;
            if ((Equals(model, lastModel)))
            {
                return value;
            }
        }
        DartRuntimePrimitives.Assert(() => false);
        return default;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class InheritedModelElement<T> : InheritedElement where T : notnull
{
    public InheritedModelElement(InheritedModel<T> widget) : base(widget)
    {
    }

    public override void updateDependencies(Element dependent, object? aspect)
    {
        var dependencies = ((HashSet<T>?)getDependencies(dependent))!;
        if (((dependencies is not null) && !Enumerable.Any(dependencies)))
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
        var __oldWidget = (InheritedModel<T>)oldWidget;
        var dependencies = ((HashSet<T>?)getDependencies(dependent))!;
        if ((dependencies is null))
        {
            return;
        }
        if ((!Enumerable.Any(dependencies) || (((InheritedModel<T>?)this.widget)!).updateShouldNotifyDependent(__oldWidget, dependencies)))
        {
            dependent.didChangeDependencies();
        }
    }

}
