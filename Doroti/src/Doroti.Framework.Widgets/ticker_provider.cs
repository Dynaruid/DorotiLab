// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/ticker_provider.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public class TickerMode : StatefulWidget
{
    public virtual bool enabled { get; private set; } = default!;
    public virtual bool forceFrames { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public TickerMode(
        Key? key = null,
        bool enabled = default!,
        Widget child = default!,
        bool forceFrames = false
    )
        : base(key: key)
    {
        this.enabled = enabled;
        this.child = child;
        this.forceFrames = forceFrames;
    }

    public static bool of(BuildContext context)
    {
        _EffectiveTickerMode__ticker_provider? widget =
            context.dependOnInheritedWidgetOfExactType<_EffectiveTickerMode__ticker_provider>();
        return widget?.enabled ?? true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static ValueListenable<bool> getNotifier(BuildContext context)
    {
        _EffectiveTickerMode__ticker_provider? widget =
            context.getInheritedWidgetOfExactType<_EffectiveTickerMode__ticker_provider>();
        return DartRuntimePrimitives.ConvertValue<ValueListenable<bool>>(
            (object?)widget?.notifier
                ?? (object?)new _ConstantValueListenable__ticker_provider<bool>(true)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static TickerModeData valuesOf(BuildContext context)
    {
        _EffectiveTickerMode__ticker_provider? widget =
            context.dependOnInheritedWidgetOfExactType<_EffectiveTickerMode__ticker_provider>();
        return widget?.values ?? TickerModeData.fallback;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static ValueListenable<TickerModeData> getValuesNotifier(BuildContext context)
    {
        var fallbackLocal = new _ConstantTickerModeDataListenable__ticker_provider(
            TickerModeData.fallback
        );
        if (!context.mounted)
        {
            return fallbackLocal;
        }
        _EffectiveTickerMode__ticker_provider? widget =
            context.getInheritedWidgetOfExactType<_EffectiveTickerMode__ticker_provider>();
        return DartRuntimePrimitives.ConvertValue<ValueListenable<TickerModeData>>(
            (object?)widget?.valuesNotifier ?? (object?)fallbackLocal
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static Widget merge(
        Key? key = null,
        bool? enabled = null,
        bool? forceFrames = null,
        Widget child = default!
    )
    {
        return new Builder(
            builder: (context) =>
            {
                _EffectiveTickerMode__ticker_provider? parent =
                    context.dependOnInheritedWidgetOfExactType<_EffectiveTickerMode__ticker_provider>();
                bool parentEnabled = parent?.enabled ?? TickerModeData.fallback.enabled;
                bool parentForce = parent?.forceFrames ?? TickerModeData.fallback.forceFrames;
                return new TickerMode(
                    key: key,
                    enabled: enabled ?? parentEnabled,
                    forceFrames: forceFrames ?? parentForce,
                    child: child
                );
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _TickerModeState__ticker_provider());
}

internal class _TickerModeState__ticker_provider : State<TickerMode>
{
    internal virtual bool _ancestorTickerMode { get; set; } = TickerModeData.fallback.enabled;
    internal virtual bool _ancestorForceFrames { get; set; } = TickerModeData.fallback.forceFrames;
    internal virtual ValueNotifier<bool> _effectiveMode { get; private set; } =
        new ValueNotifier<bool>(TickerModeData.fallback.enabled);
    internal virtual ValueNotifier<TickerModeData> _effectiveValues { get; private set; } =
        new ValueNotifier<TickerModeData>(TickerModeData.fallback);

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _EffectiveTickerMode__ticker_provider? parent =
            context.dependOnInheritedWidgetOfExactType<_EffectiveTickerMode__ticker_provider>();
        _ancestorTickerMode = parent?.enabled ?? TickerModeData.fallback.enabled;
        _ancestorForceFrames = parent?.forceFrames ?? TickerModeData.fallback.forceFrames;
        _updateEffectiveMode();
    }

    public override void didUpdateWidget(TickerMode oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        _updateEffectiveMode();
    }

    public override void dispose()
    {
        _effectiveMode.dispose();
        _effectiveValues.dispose();
        base.dispose();
    }

    internal virtual void _updateEffectiveMode()
    {
        bool enabledLocal = _ancestorTickerMode && widget.enabled;
        bool force = _ancestorForceFrames || widget.forceFrames;
        _effectiveMode.value = enabledLocal;
        _effectiveValues.value = new TickerModeData(enabled: enabledLocal, forceFrames: force);
    }

    public override Widget build(BuildContext context)
    {
        return new _EffectiveTickerMode__ticker_provider(
            enabled: _effectiveMode.value,
            forceFrames: _effectiveValues.value.forceFrames,
            notifier: _effectiveMode,
            valuesNotifier: _effectiveValues,
            child: widget.child
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new FlagProperty(
                "requested mode",
                value: widget.enabled,
                ifTrue: "enabled",
                ifFalse: "disabled",
                showName: true
            )
        );
    }
}

internal class _EffectiveTickerMode__ticker_provider : InheritedWidget
{
    public virtual bool enabled { get; private set; } = default!;
    public virtual bool forceFrames { get; private set; } = default!;
    public virtual ValueNotifier<bool> notifier { get; private set; } = default!;
    public virtual ValueNotifier<TickerModeData> valuesNotifier { get; private set; } = default!;

    internal _EffectiveTickerMode__ticker_provider(
        bool enabled,
        bool forceFrames,
        ValueNotifier<bool> notifier,
        ValueNotifier<TickerModeData> valuesNotifier,
        Widget child
    )
        : base(child: child)
    {
        this.enabled = enabled;
        this.forceFrames = forceFrames;
        this.notifier = notifier;
        this.valuesNotifier = valuesNotifier;
    }

    public virtual TickerModeData values => valuesNotifier.value;

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        DartRuntimePrimitives.ConvertValue<bool>(
            (enabled != ((_EffectiveTickerMode__ticker_provider)oldWidget).enabled)
                || (forceFrames != ((_EffectiveTickerMode__ticker_provider)oldWidget).forceFrames)
        );

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new FlagProperty(
                "effective mode",
                value: enabled,
                ifTrue: "enabled",
                ifFalse: "disabled",
                showName: true
            )
        );
    }
}

public interface SingleTickerProviderStateMixin<T> : Scheduler.TickerProvider
    where T : StatefulWidget
{
    Scheduler.Ticker? _ticker { get; set; }
    ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; }

    public new Scheduler.Ticker createTicker(Action<Duration> onTick);
    public void dispose();
    public void activate();
    public void _updateTicker();
    public void _updateTickerModeNotifier();
    public void debugFillProperties(DiagnosticPropertiesBuilder properties);
}

public interface WidgetTickerCreator
{
    void _removeTicker(_WidgetTicker__ticker_provider ticker);
}

public interface TickerProviderStateMixin<T> : Scheduler.TickerProvider, WidgetTickerCreator
    where T : StatefulWidget
{
    HashSet<Scheduler.Ticker>? _tickers { get; set; }
    ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; }

    public new Scheduler.Ticker createTicker(Action<Duration> onTick);
    public new void _removeTicker(_WidgetTicker__ticker_provider ticker);
    public void activate();
    public void _updateTickers();
    public void _updateTickerModeNotifier();
    public void dispose();
    public void debugFillProperties(DiagnosticPropertiesBuilder properties);
}

public class _WidgetTicker__ticker_provider : Scheduler.Ticker
{
    internal virtual WidgetTickerCreator _creator { get; private set; } = default!;

    public _WidgetTicker__ticker_provider(
        Action<Duration> onTick,
        WidgetTickerCreator _creator,
        string? debugLabel = null
    )
        : base(onTick, debugLabel: debugLabel ?? _creator.GetType().Name)
    {
        this._creator = _creator;
    }

    public override void dispose()
    {
        _creator._removeTicker(this);
        base.dispose();
    }
}

internal class _ConstantValueListenable__ticker_provider<T> : ValueListenable<T>
{
    public virtual T value { get; private set; } = default!;

    internal _ConstantValueListenable__ticker_provider(T value)
    {
        this.value = value;
    }

    public virtual void addListener(Action listener) { }

    public virtual void removeListener(Action listener) { }
}

public class TickerModeData
{
    public static TickerModeData fallback = new TickerModeData(enabled: true, forceFrames: false);
    public virtual bool enabled { get; private set; } = default!;
    public virtual bool forceFrames { get; private set; } = default!;

    public TickerModeData(bool enabled, bool forceFrames)
    {
        this.enabled = enabled;
        this.forceFrames = forceFrames;
    }

    public override bool Equals(object? other)
    {
        var __other = other as TickerModeData;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(__other, this))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is TickerModeData)
            && (__other.enabled == enabled)
            && (__other.forceFrames == forceFrames);
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(enabled, forceFrames)
        );
}

internal class _ConstantTickerModeDataListenable__ticker_provider : ValueListenable<TickerModeData>
{
    public virtual TickerModeData value { get; private set; } = default!;

    internal _ConstantTickerModeDataListenable__ticker_provider(TickerModeData value)
    {
        this.value = value;
    }

    public virtual void addListener(Action listener) { }

    public virtual void removeListener(Action listener) { }
}
