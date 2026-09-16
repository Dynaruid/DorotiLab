// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/value_listenable_builder.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public delegate Widget ValueWidgetBuilder<T>(BuildContext context, T value, Widget? child);

public class ValueListenableBuilder<T> : StatefulWidget
{
    public virtual global::Doroti.Framework.Foundation.ValueListenable<T> valueListenable { get; private set; } = default!;
    public virtual global::System.Func<BuildContext, T, Widget?, Widget> builder { get; private set; } = default!;
    public virtual Widget? child { get; private set; }

    public ValueListenableBuilder(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Foundation.ValueListenable<T> valueListenable = default!, global::System.Func<BuildContext, T, Widget?, Widget> builder = default!, Widget? child = null) : base(key: key)
    {
        this.valueListenable = valueListenable;
        this.builder = builder;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ValueListenableBuilderState__value_listenable_builder<T>());
}

internal class _ValueListenableBuilderState__value_listenable_builder<T> : State<ValueListenableBuilder<T>>
{
    public virtual T value { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        value = widget.valueListenable.value;
        widget.valueListenable.addListener(_valueChanged);
    }

    public override void didUpdateWidget(ValueListenableBuilder<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(oldWidget.valueListenable, widget.valueListenable))
        {
            oldWidget.valueListenable.removeListener(_valueChanged);
            value = widget.valueListenable.value;
            widget.valueListenable.addListener(_valueChanged);
        }
    }

    public override void dispose()
    {
        widget.valueListenable.removeListener(_valueChanged);
        base.dispose();
    }

    internal virtual void _valueChanged()
    {
        setState(() =>
        {
            value = widget.valueListenable.value;
        });
    }

    public override Widget build(BuildContext context)
    {
        return widget.builder(context, value, widget.child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

