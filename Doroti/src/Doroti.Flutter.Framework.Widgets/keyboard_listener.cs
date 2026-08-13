// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/keyboard_listener.dart
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

public class KeyboardListener : StatelessWidget
{
    public virtual FocusNode focusNode { get; private set; } = default!;
    public virtual bool autofocus { get; private set; } = default!;
    public virtual bool includeSemantics { get; private set; } = default!;
    public virtual global::System.Action<global::Doroti.Generated.Framework.Services.KeyEvent>? onKeyEvent { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    public KeyboardListener(global::Doroti.Generated.Framework.Foundation.Key? key = null, FocusNode focusNode = default!, bool autofocus = false, bool includeSemantics = true, global::System.Action<global::Doroti.Generated.Framework.Services.KeyEvent>? onKeyEvent = null, Widget child = default!) : base(key: key)
    {
        this.focusNode = focusNode;
        this.autofocus = autofocus;
        this.includeSemantics = includeSemantics;
        this.onKeyEvent = onKeyEvent;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new Focus(focusNode: this.focusNode, autofocus: this.autofocus, includeSemantics: this.includeSemantics, onKeyEvent: ((global::System.Func<FocusNode, global::Doroti.Generated.Framework.Services.KeyEvent, KeyEventResult>?)((node, @event) => {
this.onKeyEvent?.Invoke(@event);
return KeyEventResult.ignored;
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<FocusNode>("focusNode", this.focusNode));
    }

}

