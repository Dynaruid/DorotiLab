// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/keyboard_listener.dart
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

public class KeyboardListener : StatelessWidget
{
    public virtual FocusNode focusNode { get; private set; } = default!;
    public virtual bool autofocus { get; private set; } = default!;
    public virtual bool includeSemantics { get; private set; } = default!;
    public virtual global::System.Action<global::Doroti.Framework.Services.KeyEvent>? onKeyEvent { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    public KeyboardListener(global::Doroti.Framework.Foundation.Key? key = null, FocusNode focusNode = default!, bool autofocus = false, bool includeSemantics = true, global::System.Action<global::Doroti.Framework.Services.KeyEvent>? onKeyEvent = null, Widget child = default!) : base(key: key)
    {
        this.focusNode = focusNode;
        this.autofocus = autofocus;
        this.includeSemantics = includeSemantics;
        this.onKeyEvent = onKeyEvent;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new Focus(focusNode: this.focusNode, autofocus: this.autofocus, includeSemantics: this.includeSemantics, onKeyEvent: ((global::System.Func<FocusNode, global::Doroti.Framework.Services.KeyEvent, KeyEventResult>?)((node, @event) =>
        {
            this.onKeyEvent?.Invoke(@event);
            return KeyEventResult.ignored;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<FocusNode>("focusNode", this.focusNode));
    }

}

