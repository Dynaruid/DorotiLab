// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/shared_app_data.dart
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

public delegate T SharedAppDataInitCallback<T>();

public class SharedAppData : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;

    public SharedAppData(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget child = default!) : base(key: key)
    {
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SharedAppDataState__shared_app_data());
    public static V getValue<K, V>(BuildContext context, K key, global::System.Func<V> init)
    {
        _SharedAppModel__shared_app_data? model__4680 = ((_SharedAppModel__shared_app_data?)(object?)InheritedModel<object>.inheritFrom<_SharedAppModel__shared_app_data>(context, aspect: key));
        DartRuntimePrimitives.Assert(() => SharedAppData._debugHasSharedAppData(model__4680, context, "getValue"));
        return ((V)(object?)model__4680!.sharedAppDataState.getValue<K, V>(key, (global::System.Func<V>)init));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static void setValue<K, V>(BuildContext context, K key, V value)
    {
        _SharedAppModel__shared_app_data? model__5644 = ((_SharedAppModel__shared_app_data?)(object?)context.getInheritedWidgetOfExactType<_SharedAppModel__shared_app_data>());
        DartRuntimePrimitives.Assert(() => SharedAppData._debugHasSharedAppData(model__5644, context, "setValue"));
        model__5644!.sharedAppDataState.setValue<K, V>(key, value);
    }

    internal static bool _debugHasSharedAppData(_SharedAppModel__shared_app_data? model, BuildContext context, string methodName)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((model is null))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary("No SharedAppData widget found."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"SharedAppData.{methodName} requires an SharedAppData widget ancestor.\n"), context.describeWidget("The specific widget that could not find an SharedAppData ancestor was"), context.describeOwnershipChain("The ownership chain for the affected widget is"), new global::Doroti.Generated.Framework.Foundation.ErrorHint("Typically, the SharedAppData widget is introduced by the MaterialApp " + "or WidgetsApp widget at the top of your application widget tree. It " + "provides a key/value map of data that is shared with the entire " + "application.") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SharedAppDataState__shared_app_data : State<SharedAppData>
{
    private bool __late_data_initialized;
    private DartMap<object, object> __late_data = default!;
    public virtual DartMap<object, object> data
    {
        get
        {
            if (!__late_data_initialized)
            {
                __late_data = new DartMap<object, object>();
                __late_data_initialized = true;
            }
            return __late_data;
        }
        set { __late_data = value; __late_data_initialized = true; }
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new _SharedAppModel__shared_app_data(sharedAppDataState: this, child: ((SharedAppData)(object)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual V getValue<K, V>(K key, global::System.Func<V> init)
    {
        this.data.putIfAbsent(key, () => init());
        return ((V?)(object?)this.data.GetValueOrDefault(key))!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void setValue<K, V>(K key, V value)
    {
        if ((!object.Equals(this.data.GetValueOrDefault(key), value)))
        {
            setState(((global::System.Action)(() => {
data = new DartMap<object, object?>(this.data);
this.data[key] = value;
})));
        }
    }

}

internal class _SharedAppModel__shared_app_data : InheritedModel<object>
{
    public virtual _SharedAppDataState__shared_app_data sharedAppDataState { get; private set; } = default!;
    public virtual DartMap<object, object> data { get; private set; } = default!;

    internal _SharedAppModel__shared_app_data(_SharedAppDataState__shared_app_data sharedAppDataState, Widget child) : base(child: child)
    {
        this.sharedAppDataState = sharedAppDataState;
        this.data = ((_SharedAppDataState__shared_app_data)sharedAppDataState).data;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __old = (_SharedAppModel__shared_app_data)(object)oldWidget;
        return (!object.Equals(this.data, ((_SharedAppModel__shared_app_data)__old).data));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotifyDependent(InheritedModel<object> old, HashSet<object> keys)
    {
        var __old = (_SharedAppModel__shared_app_data)(object)old;
        foreach (var key__7881 in keys)
        {
            if ((!object.Equals(this.data.GetValueOrDefault(key__7881), ((_SharedAppModel__shared_app_data)__old).data.GetValueOrDefault(key__7881))))
            {
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

