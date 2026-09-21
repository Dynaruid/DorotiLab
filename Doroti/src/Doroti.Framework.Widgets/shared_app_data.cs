// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/shared_app_data.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public delegate T SharedAppDataInitCallback<T>();

public class SharedAppData : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;

    public SharedAppData(Key? key = null, Widget child = default!)
        : base(key: key)
    {
        this.child = child;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _SharedAppDataState__shared_app_data());

    public static V getValue<K, V>(BuildContext context, K key, Func<V> init)
        where K : notnull
    {
        _SharedAppModel__shared_app_data? model =
            InheritedModel<object>.inheritFrom<_SharedAppModel__shared_app_data>(
                context,
                aspect: key
            );
        DartRuntimePrimitives.Assert(() => _debugHasSharedAppData(model, context, "getValue"));
        return model!.sharedAppDataState.getValue(key, init);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static void setValue<K, V>(BuildContext context, K key, V value)
        where K : notnull
    {
        _SharedAppModel__shared_app_data? model =
            context.getInheritedWidgetOfExactType<_SharedAppModel__shared_app_data>();
        DartRuntimePrimitives.Assert(() => _debugHasSharedAppData(model, context, "setValue"));
        model!.sharedAppDataState.setValue(key, value);
    }

    internal static bool _debugHasSharedAppData(
        _SharedAppModel__shared_app_data? model,
        BuildContext context,
        string methodName
    )
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (model is null)
            {
                throw DartRuntimePrimitives.AsException(
                    new FlutterError(
                        new List<DiagnosticsNode>
                        {
                            new ErrorSummary("No SharedAppData widget found."),
                            new ErrorDescription(
                                $"SharedAppData.{methodName} requires an SharedAppData widget ancestor.\n"
                            ),
                            context.describeWidget(
                                "The specific widget that could not find an SharedAppData ancestor was"
                            ),
                            context.describeOwnershipChain(
                                "The ownership chain for the affected widget is"
                            ),
                            new ErrorHint(
                                "Typically, the SharedAppData widget is introduced by the MaterialApp "
                                    + "or WidgetsApp widget at the top of your application widget tree. It "
                                    + "provides a key/value map of data that is shared with the entire "
                                    + "application."
                            ),
                        }
                    )
                );
            }
            return true;
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _SharedAppDataState__shared_app_data : State<SharedAppData>
{
    private bool __late_data_initialized;
    private DartMap<object, object?> __late_data = default!;
    public virtual DartMap<object, object?> data
    {
        get
        {
            if (!__late_data_initialized)
            {
                __late_data = new DartMap<object, object?>();
                __late_data_initialized = true;
            }
            return __late_data;
        }
        set
        {
            __late_data = value;
            __late_data_initialized = true;
        }
    }

    public override Widget build(BuildContext context)
    {
        return new _SharedAppModel__shared_app_data(sharedAppDataState: this, child: widget.child);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual V getValue<K, V>(K key, Func<V> init)
        where K : notnull
    {
        data.putIfAbsent(key, () => init());
        return ((V?)data.GetValueOrDefault(key))!;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void setValue<K, V>(K key, V value)
        where K : notnull
    {
        if (!Equals(data.GetValueOrDefault(key), value))
        {
            setState(() =>
            {
                data = new DartMap<object, object?>(data);
                data[key] = value;
            });
        }
    }
}

internal class _SharedAppModel__shared_app_data : InheritedModel<object>
{
    public virtual _SharedAppDataState__shared_app_data sharedAppDataState { get; private set; } =
        default!;
    public virtual DartMap<object, object?> data { get; private set; } = default!;

    internal _SharedAppModel__shared_app_data(
        _SharedAppDataState__shared_app_data sharedAppDataState,
        Widget child
    )
        : base(child: child)
    {
        this.sharedAppDataState = sharedAppDataState;
        data = sharedAppDataState.data;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __old = (_SharedAppModel__shared_app_data)oldWidget;
        return !Equals(data, __old.data);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool updateShouldNotifyDependent(
        InheritedModel<object> old,
        HashSet<object> keys
    )
    {
        var __old = (_SharedAppModel__shared_app_data)old;
        foreach (var key in keys)
        {
            if (!Equals(data.GetValueOrDefault(key), __old.data.GetValueOrDefault(key)))
            {
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
