// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/raw_keyboard_listener.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public class RawKeyboardListener : StatefulWidget
{
    public virtual FocusNode focusNode { get; private set; } = default!;
    public virtual bool autofocus { get; private set; } = default!;
    public virtual bool includeSemantics { get; private set; } = default!;
    public virtual global::System.Action<global::Doroti.Generated.Framework.Services.RawKeyEvent>? onKey { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    public RawKeyboardListener(global::Doroti.Generated.Framework.Foundation.Key? key = null, FocusNode focusNode = default!, bool autofocus = false, bool includeSemantics = true, global::System.Action<global::Doroti.Generated.Framework.Services.RawKeyEvent>? onKey = null, Widget child = default!) : base(key: key)
    {
        this.focusNode = focusNode;
        this.autofocus = autofocus;
        this.includeSemantics = includeSemantics;
        this.onKey = onKey;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _RawKeyboardListenerState__raw_keyboard_listener());
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<FocusNode>("focusNode", this.focusNode));
    }

}

internal class _RawKeyboardListenerState__raw_keyboard_listener : State<RawKeyboardListener>
{
    internal virtual bool _listening { get; set; } = false;

    public override void initState()
    {
        base.initState();
        ((RawKeyboardListener)this.widget).focusNode.addListener(() => this._handleFocusChanged());
    }

    public override void didUpdateWidget(RawKeyboardListener oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((RawKeyboardListener)this.widget).focusNode, ((RawKeyboardListener)oldWidget).focusNode)))
        {
            ((RawKeyboardListener)oldWidget).focusNode.removeListener(() => this._handleFocusChanged());
            ((RawKeyboardListener)this.widget).focusNode.addListener(() => this._handleFocusChanged());
        }
    }

    public override void dispose()
    {
        ((RawKeyboardListener)this.widget).focusNode.removeListener(() => this._handleFocusChanged());
        _detachKeyboardIfAttached();
        base.dispose();
    }

    internal virtual void _handleFocusChanged()
    {
        if (((RawKeyboardListener)this.widget).focusNode.hasFocus)
        {
            _attachKeyboardIfDetached();
        }
        else
        {
            _detachKeyboardIfAttached();
        }
    }

    internal virtual void _attachKeyboardIfDetached()
    {
        if (this._listening)
        {
            return;
        }
        global::Doroti.Generated.Framework.Services.RawKeyboard.instance.addListener((global::System.Action<global::Doroti.Generated.Framework.Services.RawKeyEvent>)this._handleRawKeyEvent);
        _listening = true;
    }

    internal virtual void _detachKeyboardIfAttached()
    {
        if (!this._listening)
        {
            return;
        }
        global::Doroti.Generated.Framework.Services.RawKeyboard.instance.removeListener((global::System.Action<global::Doroti.Generated.Framework.Services.RawKeyEvent>)this._handleRawKeyEvent);
        _listening = false;
    }

    internal virtual void _handleRawKeyEvent(global::Doroti.Generated.Framework.Services.RawKeyEvent @event)
    {
        ((RawKeyboardListener)this.widget).onKey?.Invoke(@event);
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new Focus(focusNode: ((RawKeyboardListener)this.widget).focusNode, autofocus: ((RawKeyboardListener)this.widget).autofocus, includeSemantics: ((RawKeyboardListener)this.widget).includeSemantics, child: ((RawKeyboardListener)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

