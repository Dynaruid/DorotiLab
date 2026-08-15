// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/value_listenable_builder.dart
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

public delegate Widget ValueWidgetBuilder<T>(BuildContext context, T value, Widget? child);

public class ValueListenableBuilder<T> : StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<T> valueListenable { get; private set; } = default!;
    public virtual global::System.Func<BuildContext, T, Widget?, Widget> builder { get; private set; } = default!;
    public virtual Widget? child { get; private set; }

    public ValueListenableBuilder(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Foundation.ValueListenable<T> valueListenable = default!, global::System.Func<BuildContext, T, Widget?, Widget> builder = default!, Widget? child = null) : base(key: key)
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
        this.value = ((ValueListenableBuilder<T>)(object)this.widget).valueListenable.value;
        ((ValueListenableBuilder<T>)(object)this.widget).valueListenable.addListener(() => this._valueChanged());
    }

    public override void didUpdateWidget(ValueListenableBuilder<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((ValueListenableBuilder<T>)oldWidget).valueListenable, ((ValueListenableBuilder<T>)(object)this.widget).valueListenable)))
        {
            ((ValueListenableBuilder<T>)oldWidget).valueListenable.removeListener(() => this._valueChanged());
            this.value = ((ValueListenableBuilder<T>)(object)this.widget).valueListenable.value;
            ((ValueListenableBuilder<T>)(object)this.widget).valueListenable.addListener(() => this._valueChanged());
        }
    }

    public override void dispose()
    {
        ((ValueListenableBuilder<T>)(object)this.widget).valueListenable.removeListener(() => this._valueChanged());
        base.dispose();
    }

    internal virtual void _valueChanged()
    {
        setState(((global::System.Action)(() => {
this.value = ((ValueListenableBuilder<T>)(object)this.widget).valueListenable.value;
})));
    }

    public override Widget build(BuildContext context)
    {
        return this.widget.builder(context, this.value, ((ValueListenableBuilder<T>)(object)this.widget).child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

