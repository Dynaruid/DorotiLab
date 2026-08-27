// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/ticker_provider.dart
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

public class TickerMode : StatefulWidget
{
    public virtual bool enabled { get; private set; } = default!;
    public virtual bool forceFrames { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public TickerMode(global::Doroti.Framework.Foundation.Key? key = null, bool enabled = default!, Widget child = default!, bool forceFrames = false) : base(key: key)
    {
        this.enabled = enabled;
        this.child = child;
        this.forceFrames = forceFrames;
    }

    public static bool of(BuildContext context)
    {
        _EffectiveTickerMode__ticker_provider? widget = ((_EffectiveTickerMode__ticker_provider?)(object?)context.dependOnInheritedWidgetOfExactType<_EffectiveTickerMode__ticker_provider>());
        return (widget?.enabled ?? true);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Framework.Foundation.ValueListenable<bool> getNotifier(BuildContext context)
    {
        _EffectiveTickerMode__ticker_provider? widget = ((_EffectiveTickerMode__ticker_provider?)(object?)context.getInheritedWidgetOfExactType<_EffectiveTickerMode__ticker_provider>());
        return DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Foundation.ValueListenable<bool>>((object?)widget?.notifier ?? (object?)new _ConstantValueListenable__ticker_provider<bool>(true));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TickerModeData valuesOf(BuildContext context)
    {
        _EffectiveTickerMode__ticker_provider? widget = ((_EffectiveTickerMode__ticker_provider?)(object?)context.dependOnInheritedWidgetOfExactType<_EffectiveTickerMode__ticker_provider>());
        return (widget?.values ?? TickerModeData.fallback);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> getValuesNotifier(BuildContext context)
    {
        var fallbackLocal = new _ConstantTickerModeDataListenable__ticker_provider(TickerModeData.fallback);
        if (!((BuildContext)context).mounted)
        {
            return ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)fallbackLocal);
        }
        _EffectiveTickerMode__ticker_provider? widget = ((_EffectiveTickerMode__ticker_provider?)(object?)context.getInheritedWidgetOfExactType<_EffectiveTickerMode__ticker_provider>());
        return DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>>((object?)widget?.valuesNotifier ?? (object?)fallbackLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Widget merge(global::Doroti.Framework.Foundation.Key? key = null, bool? enabled = null, bool? forceFrames = null, Widget child = default!)
    {
        return ((Widget)(object?)new Builder(builder: ((global::System.Func<BuildContext, Widget>)((context) =>
        {
            _EffectiveTickerMode__ticker_provider? parent = ((_EffectiveTickerMode__ticker_provider?)(object?)context.dependOnInheritedWidgetOfExactType<_EffectiveTickerMode__ticker_provider>());
            bool parentEnabled = (parent?.enabled ?? TickerModeData.fallback.enabled);
            bool parentForce = (parent?.forceFrames ?? TickerModeData.fallback.forceFrames);
            return ((Widget)(object?)new TickerMode(key: key, enabled: (enabled ?? parentEnabled), forceFrames: (forceFrames ?? parentForce), child: child));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _TickerModeState__ticker_provider());
}

internal class _TickerModeState__ticker_provider : State<TickerMode>
{
    internal virtual bool _ancestorTickerMode { get; set; } = TickerModeData.fallback.enabled;
    internal virtual bool _ancestorForceFrames { get; set; } = TickerModeData.fallback.forceFrames;
    internal virtual global::Doroti.Framework.Foundation.ValueNotifier<bool> _effectiveMode { get; private set; } = new global::Doroti.Framework.Foundation.ValueNotifier<bool>(TickerModeData.fallback.enabled);
    internal virtual global::Doroti.Framework.Foundation.ValueNotifier<TickerModeData> _effectiveValues { get; private set; } = new global::Doroti.Framework.Foundation.ValueNotifier<TickerModeData>(TickerModeData.fallback);

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _EffectiveTickerMode__ticker_provider? parent = ((_EffectiveTickerMode__ticker_provider?)(object?)this.context.dependOnInheritedWidgetOfExactType<_EffectiveTickerMode__ticker_provider>());
        _ancestorTickerMode = (parent?.enabled ?? TickerModeData.fallback.enabled);
        _ancestorForceFrames = (parent?.forceFrames ?? TickerModeData.fallback.forceFrames);
        _updateEffectiveMode();
    }

    public override void didUpdateWidget(TickerMode oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        _updateEffectiveMode();
    }

    public override void dispose()
    {
        this._effectiveMode.dispose();
        this._effectiveValues.dispose();
        base.dispose();
    }

    internal virtual void _updateEffectiveMode()
    {
        bool enabledLocal = (this._ancestorTickerMode && ((TickerMode)(object)this.widget).enabled);
        bool force = (this._ancestorForceFrames || ((TickerMode)(object)this.widget).forceFrames);
        this._effectiveMode.value = enabledLocal;
        this._effectiveValues.value = new TickerModeData(enabled: enabledLocal, forceFrames: force);
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new _EffectiveTickerMode__ticker_provider(enabled: ((global::Doroti.Framework.Foundation.ValueNotifier<bool>)this._effectiveMode).value, forceFrames: ((global::Doroti.Framework.Foundation.ValueNotifier<TickerModeData>)this._effectiveValues).value.forceFrames, notifier: this._effectiveMode, valuesNotifier: this._effectiveValues, child: ((TickerMode)(object)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("requested mode", value: ((TickerMode)(object)this.widget).enabled, ifTrue: "enabled", ifFalse: "disabled", showName: true));
    }

}

internal class _EffectiveTickerMode__ticker_provider : InheritedWidget
{
    public virtual bool enabled { get; private set; } = default!;
    public virtual bool forceFrames { get; private set; } = default!;
    public virtual global::Doroti.Framework.Foundation.ValueNotifier<bool> notifier { get; private set; } = default!;
    public virtual global::Doroti.Framework.Foundation.ValueNotifier<TickerModeData> valuesNotifier { get; private set; } = default!;

    internal _EffectiveTickerMode__ticker_provider(bool enabled, bool forceFrames, global::Doroti.Framework.Foundation.ValueNotifier<bool> notifier, global::Doroti.Framework.Foundation.ValueNotifier<TickerModeData> valuesNotifier, Widget child) : base(child: child)
    {
        this.enabled = enabled;
        this.forceFrames = forceFrames;
        this.notifier = notifier;
        this.valuesNotifier = valuesNotifier;
    }

    public virtual TickerModeData values => ((global::Doroti.Framework.Foundation.ValueNotifier<TickerModeData>)this.valuesNotifier).value;
    public override bool updateShouldNotify(InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(((this.enabled != ((_EffectiveTickerMode__ticker_provider)oldWidget).enabled) || (this.forceFrames != ((_EffectiveTickerMode__ticker_provider)oldWidget).forceFrames)));
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("effective mode", value: this.enabled, ifTrue: "enabled", ifFalse: "disabled", showName: true));
    }

}

public interface SingleTickerProviderStateMixin<T> : global::Doroti.Framework.Scheduler.TickerProvider where T : StatefulWidget
{
    global::Doroti.Framework.Scheduler.Ticker? _ticker { get; set; }
    global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; }

    public global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick);
    public void dispose();
    public void activate();
    public void _updateTicker();
    public void _updateTickerModeNotifier();
    public void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties);
}

public interface WidgetTickerCreator
{
    void _removeTicker(_WidgetTicker__ticker_provider ticker);
}

public interface TickerProviderStateMixin<T> : global::Doroti.Framework.Scheduler.TickerProvider, WidgetTickerCreator where T : StatefulWidget
{
    HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; }
    global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; }

    public global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick);
    public void _removeTicker(_WidgetTicker__ticker_provider ticker);
    public void activate();
    public void _updateTickers();
    public void _updateTickerModeNotifier();
    public void dispose();
    public void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties);
}

public class _WidgetTicker__ticker_provider : global::Doroti.Framework.Scheduler.Ticker
{
    internal virtual WidgetTickerCreator _creator { get; private set; } = default!;

    public _WidgetTicker__ticker_provider(global::System.Action<Duration> onTick, WidgetTickerCreator _creator, string? debugLabel = null) : base(onTick, debugLabel: debugLabel ?? _creator.GetType().Name)
    {
        this._creator = _creator;
    }

    public override void dispose()
    {
        this._creator._removeTicker(this);
        base.dispose();
    }

}

internal class _ConstantValueListenable__ticker_provider<T> : global::Doroti.Framework.Foundation.ValueListenable<T>
{
    public virtual T value { get; private set; } = default!;

    internal _ConstantValueListenable__ticker_provider(T value)
    {
        this.value = value;
    }

    public virtual void addListener(global::System.Action listener)
    {
    }

    public virtual void removeListener(global::System.Action listener)
    {
    }

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
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(__other, this))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is TickerModeData) && (((TickerModeData)((TickerModeData)__other)).enabled == this.enabled)) && (((TickerModeData)((TickerModeData)__other)).forceFrames == this.forceFrames));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.enabled, this.forceFrames));
}

internal class _ConstantTickerModeDataListenable__ticker_provider : global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>
{
    public virtual TickerModeData value { get; private set; } = default!;

    internal _ConstantTickerModeDataListenable__ticker_provider(TickerModeData value)
    {
        this.value = value;
    }

    public virtual void addListener(global::System.Action listener)
    {
    }

    public virtual void removeListener(global::System.Action listener)
    {
    }

}
