// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/raw_keyboard_listener.dart
#pragma warning disable CS8600, CS8603
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

public class RawKeyboardListener : StatefulWidget
{
    public virtual FocusNode focusNode { get; private set; } = default!;
    public virtual bool autofocus { get; private set; } = default!;
    public virtual bool includeSemantics { get; private set; } = default!;
    public virtual global::System.Action<global::Doroti.Framework.Services.RawKeyEvent>? onKey { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    public RawKeyboardListener(global::Doroti.Framework.Foundation.Key? key = null, FocusNode focusNode = default!, bool autofocus = false, bool includeSemantics = true, global::System.Action<global::Doroti.Framework.Services.RawKeyEvent>? onKey = null, Widget child = default!) : base(key: key)
    {
        this.focusNode = focusNode;
        this.autofocus = autofocus;
        this.includeSemantics = includeSemantics;
        this.onKey = onKey;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _RawKeyboardListenerState__raw_keyboard_listener());
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<FocusNode>("focusNode", this.focusNode));
    }

}

internal class _RawKeyboardListenerState__raw_keyboard_listener : State<RawKeyboardListener>
{
    internal virtual bool _listening { get; set; } = false;

    public override void initState()
    {
        base.initState();
        ((RawKeyboardListener)this.widget).focusNode.addListener(this._handleFocusChanged);
    }

    public override void didUpdateWidget(RawKeyboardListener oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((RawKeyboardListener)this.widget).focusNode, ((RawKeyboardListener)oldWidget).focusNode)))
        {
            ((RawKeyboardListener)oldWidget).focusNode.removeListener(this._handleFocusChanged);
            ((RawKeyboardListener)this.widget).focusNode.addListener(this._handleFocusChanged);
        }
    }

    public override void dispose()
    {
        ((RawKeyboardListener)this.widget).focusNode.removeListener(this._handleFocusChanged);
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
        global::Doroti.Framework.Services.RawKeyboard.instance.addListener((global::System.Action<global::Doroti.Framework.Services.RawKeyEvent>)this._handleRawKeyEvent);
        _listening = true;
    }

    internal virtual void _detachKeyboardIfAttached()
    {
        if (!this._listening)
        {
            return;
        }
        global::Doroti.Framework.Services.RawKeyboard.instance.removeListener((global::System.Action<global::Doroti.Framework.Services.RawKeyEvent>)this._handleRawKeyEvent);
        _listening = false;
    }

    internal virtual void _handleRawKeyEvent(global::Doroti.Framework.Services.RawKeyEvent @event)
    {
        ((RawKeyboardListener)this.widget).onKey?.Invoke(@event);
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new Focus(focusNode: ((RawKeyboardListener)this.widget).focusNode, autofocus: ((RawKeyboardListener)this.widget).autofocus, includeSemantics: ((RawKeyboardListener)this.widget).includeSemantics, child: ((RawKeyboardListener)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

