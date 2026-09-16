// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/keyboard_listener.dart
namespace Doroti.Framework.Widgets;

public class KeyboardListener : StatelessWidget
{
    public virtual FocusNode focusNode { get; private set; } = default!;
    public virtual bool autofocus { get; private set; } = default!;
    public virtual bool includeSemantics { get; private set; } = default!;
    public virtual Action<KeyEvent>? onKeyEvent { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    public KeyboardListener(Key? key = null, FocusNode focusNode = default!, bool autofocus = false, bool includeSemantics = true, Action<KeyEvent>? onKeyEvent = null, Widget child = default!) : base(key: key)
    {
        this.focusNode = focusNode;
        this.autofocus = autofocus;
        this.includeSemantics = includeSemantics;
        this.onKeyEvent = onKeyEvent;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        return new Focus(focusNode: focusNode, autofocus: autofocus, includeSemantics: includeSemantics, onKeyEvent: (node, @event) =>
        {
            onKeyEvent?.Invoke(@event);
            return KeyEventResult.ignored;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<FocusNode>("focusNode", focusNode));
    }

}

