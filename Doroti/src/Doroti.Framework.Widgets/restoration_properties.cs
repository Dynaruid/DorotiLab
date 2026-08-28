// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/restoration_properties.dart
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

public abstract class RestorableValue<T> : RestorableProperty<T>
{
    internal virtual T? _value { get; set; } = default;

    public virtual T value
    {
        get
        {
            DartRuntimePrimitives.Assert(() => this.isRegistered);
            return ((T?)(object?)this._value)!;
            return default!;
        }
        set
        {
            var newValue = value;
            DartRuntimePrimitives.Assert(() => this.isRegistered);
            if (!EqualityComparer<T>.Default.Equals(newValue, this._value))
            {
                T? oldValue = this._value;
                _value = newValue;
                didUpdateValue(oldValue);
            }
        }
    }
    public override void initWithValue(T value)
    {
        _value = value;
    }

    public abstract void didUpdateValue(T? oldValue);
}

public class _RestorablePrimitiveValueN__restoration_properties<T> : RestorableValue<T>
{
    internal virtual T _defaultValue { get; private set; } = default!;

    internal _RestorablePrimitiveValueN__restoration_properties(T _defaultValue)
    {
        this._defaultValue = _defaultValue;
        System.Diagnostics.Debug.Assert(global::Doroti.Framework.Services.RestorationLibrary.debugIsSerializableForRestoration(_defaultValue));
    }

    public override T createDefaultValue() => this._defaultValue;
    public override void didUpdateValue(T oldValue)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Services.RestorationLibrary.debugIsSerializableForRestoration(this.value));
        notifyListeners();
    }

    public override T fromPrimitives(object? data) => ((T?)(object?)data)!;
    public override object? toPrimitives() => this.value;
}

public class _RestorablePrimitiveValue__restoration_properties<T> : _RestorablePrimitiveValueN__restoration_properties<T>
{
    internal _RestorablePrimitiveValue__restoration_properties(T defaultValue) : base(defaultValue)
    {
        System.Diagnostics.Debug.Assert(global::Doroti.Framework.Services.RestorationLibrary.debugIsSerializableForRestoration(defaultValue));
    }

    public override T value
    {
        set
        {
            var __value = value;
            base.value = __value;
        }
    }
    public override T fromPrimitives(object? data)
    {
        DartRuntimePrimitives.Assert(() => (data is not null));
        return ((T)(object?)base.fromPrimitives(data));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override object? toPrimitives()
    {
        return base.toPrimitives()!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RestorableNum<T> : _RestorablePrimitiveValue__restoration_properties<T> where T : struct
{
    public RestorableNum(T defaultValue) : base(defaultValue)
    {
    }

}

public class RestorableDouble : RestorableNum<double>
{
    public RestorableDouble(double defaultValue) : base(defaultValue)
    {
    }

}

public class RestorableInt : RestorableNum<long>
{
    public RestorableInt(long defaultValue) : base(defaultValue)
    {
    }

}

public class RestorableString : _RestorablePrimitiveValue__restoration_properties<string>
{
    public RestorableString(string defaultValue) : base(defaultValue)
    {
    }

}

public class RestorableBool : _RestorablePrimitiveValue__restoration_properties<bool>
{
    public RestorableBool(bool defaultValue) : base(defaultValue)
    {
    }

}

public class RestorableBoolN : _RestorablePrimitiveValueN__restoration_properties<bool?>
{
    public RestorableBoolN(bool? defaultValue) : base(DartRuntimePrimitives.RequireValue(defaultValue))
    {
    }

}

public class RestorableNumN<T> : _RestorablePrimitiveValueN__restoration_properties<T>
{
    public RestorableNumN(T defaultValue) : base(defaultValue)
    {
    }

}

public class RestorableDoubleN : RestorableNumN<double?>
{
    public RestorableDoubleN(double? defaultValue) : base(DartRuntimePrimitives.RequireValue(defaultValue))
    {
    }

}

public class RestorableIntN : RestorableNumN<long?>
{
    public RestorableIntN(long? defaultValue) : base(DartRuntimePrimitives.RequireValue(defaultValue))
    {
    }

}

public class RestorableStringN : _RestorablePrimitiveValueN__restoration_properties<string?>
{
    public RestorableStringN(string? defaultValue) : base(defaultValue)
    {
    }

}

public class RestorableDateTime : RestorableValue<DateTime>
{
    internal virtual DateTime _defaultValue { get; private set; } = default!;

    public RestorableDateTime(DateTime defaultValue)
    {
        this._defaultValue = defaultValue;
    }

    public override DateTime createDefaultValue() => this._defaultValue;
    public override void didUpdateValue(DateTime oldValue)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Services.RestorationLibrary.debugIsSerializableForRestoration(DartRuntimePrimitives.MillisecondsSinceEpoch(this.value)));
        notifyListeners();
    }

    public override DateTime fromPrimitives(object? data) => new DateTime(((long)data!));
    public override object? toPrimitives() => DartRuntimePrimitives.MillisecondsSinceEpoch(this.value);
}

public class RestorableDateTimeN : RestorableValue<DateTime?>
{
    internal virtual DateTime? _defaultValue { get; private set; }

    public RestorableDateTimeN(DateTime? defaultValue)
    {
        this._defaultValue = defaultValue;
    }

    public override DateTime? createDefaultValue() => this._defaultValue;
    public override void didUpdateValue(DateTime? oldValue)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Services.RestorationLibrary.debugIsSerializableForRestoration(DartRuntimePrimitives.MillisecondsSinceEpoch(this.value)));
        notifyListeners();
    }

    public override DateTime? fromPrimitives(object? data) => ((data is not null) ? new DateTime(((long)data)) : null);
    public override object? toPrimitives() => DartRuntimePrimitives.MillisecondsSinceEpoch(this.value);
}

public abstract class RestorableListenable<T> : RestorableProperty<T> where T : global::Doroti.Framework.Foundation.Listenable
{
    internal virtual T? _value { get; set; } = default;

    public virtual T value
    {
        get
        {
            DartRuntimePrimitives.Assert(() => this.isRegistered);
            return this._value!;
            return default!;
        }
    }
    public override void initWithValue(T value)
    {
        this._value?.removeListener(this.notifyListeners);
        _value = value;
        this._value!.addListener(this.notifyListeners);
    }

    public override void dispose()
    {
        base.dispose();
        this._value?.removeListener(this.notifyListeners);
    }

}

public abstract class RestorableChangeNotifier<T> : RestorableListenable<T> where T : global::Doroti.Framework.Foundation.ChangeNotifier
{
    public override void initWithValue(T value)
    {
        _disposeOldValue();
        base.initWithValue(value);
    }

    public override void dispose()
    {
        _disposeOldValue();
        base.dispose();
    }

    internal virtual void _disposeOldValue()
    {
        if ((this._value is not null))
        {
            DartAsyncRuntime.scheduleMicrotask(this._value!.dispose);
        }
    }

}

public class RestorableTextEditingController : RestorableChangeNotifier<TextEditingController>
{
    internal virtual global::Doroti.Framework.Services.TextEditingValue _initialValue { get; private set; } = default!;

    public static RestorableTextEditingController Create(string? text = null) => new RestorableTextEditingController(((text is null) ? global::Doroti.Framework.Services.TextEditingValue.empty : new global::Doroti.Framework.Services.TextEditingValue(text: text)));

    public RestorableTextEditingController(global::Doroti.Framework.Services.TextEditingValue value)
    {
        this._initialValue = value;
    }

    public override TextEditingController createDefaultValue()
    {
        return TextEditingController.CreateFromValue(this._initialValue);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override TextEditingController fromPrimitives(object? data)
    {
        return new TextEditingController(text: ((string?)(object?)data!)!);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override object? toPrimitives()
    {
        return ((TextEditingController)(object)this.value).text;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RestorableEnumN<T> : RestorableValue<T?> where T : struct, Enum
{
    internal virtual T? _defaultValue { get; private set; }
    public virtual HashSet<T> values { get; set; } = default!;

    public RestorableEnumN(T? defaultValue, IEnumerable<T> values)
    {
        this._defaultValue = defaultValue;
        this.values = values.toSet();
        System.Diagnostics.Debug.Assert(defaultValue is null || values.Contains(defaultValue.Value));
    }

    public override T? createDefaultValue() => this._defaultValue;
    public override T? value
    {
        set
        {
            var newValue = value;
            DartRuntimePrimitives.Assert(() => newValue is null || this.values.Contains(newValue.Value), () => (object?)$"Attempted to set an unknown enum value \"{newValue}\" that is not null, or " + $"in the valid set of enum values for the {typeof(T)} type: " + $"{this.values.map<T, string>(((value) => value.ToString())).toSet()}");
            base.value = newValue;
        }
    }
    public override void didUpdateValue(T? oldValue)
    {
        notifyListeners();
    }

    public override T? fromPrimitives(object? data)
    {
        if ((data is null))
        {
            return default;
        }
        if ((data is string))
        {
            string data__as18369 = (string)data;
            foreach (T allowed in this.values)
            {
                if ((allowed.ToString() == ((string)data__as18369)))
                {
                    return allowed;
                }
            }
            DartRuntimePrimitives.Assert(() => false, () => (object?)$"Attempted to set an unknown enum value \"{((string)data__as18369)}\" that is not null, or " + $"in the valid set of enum values for the {typeof(T)} type: " + $"{this.values.map<T, string>(((value) => value.ToString())).toSet()}");
        }
        return this._defaultValue;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override object? toPrimitives() => this.value.ToString();
}

public class RestorableEnum<T> : RestorableValue<T> where T : Enum
{
    internal virtual T _defaultValue { get; private set; } = default!;
    public virtual HashSet<T> values { get; set; } = default!;

    public RestorableEnum(T defaultValue, IEnumerable<T> values)
    {
        this._defaultValue = defaultValue;
        this.values = values.toSet();
        System.Diagnostics.Debug.Assert(values.contains(defaultValue));
    }

    public override T createDefaultValue() => this._defaultValue;
    public override T value
    {
        set
        {
            var newValue = value;
            DartRuntimePrimitives.Assert(() => this.values.Contains(newValue), () => (object?)$"Attempted to set an unknown enum value \"{newValue}\" that is not in the " + $"valid set of enum values for the {typeof(T)} type: " + $"{this.values.map<T, string>(((value) => value.ToString())).toSet()}");
            base.value = newValue;
        }
    }
    public override void didUpdateValue(T oldValue)
    {
        notifyListeners();
    }

    public override T fromPrimitives(object? data)
    {
        if (((data is not null) && (data is string)))
        {
            string data__as21037 = (string)data;
            foreach (T allowed in this.values)
            {
                if ((allowed.ToString() == ((string)data__as21037)))
                {
                    return allowed;
                }
            }
            DartRuntimePrimitives.Assert(() => false, () => (object?)$"Attempted to restore an unknown enum value \"{((string)data__as21037)}\" that is not in the " + $"valid set of enum values for the {typeof(T)} type: " + $"{this.values.map<T, string>(((value) => value.ToString())).toSet()}");
        }
        return this._defaultValue;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override object? toPrimitives() => this.value.ToString();
}
