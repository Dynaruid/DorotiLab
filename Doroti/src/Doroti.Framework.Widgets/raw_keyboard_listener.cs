// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/raw_keyboard_listener.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public class RawKeyboardListener : StatefulWidget
{
    public virtual FocusNode focusNode { get; private set; } = default!;
    public virtual bool autofocus { get; private set; } = default!;
    public virtual bool includeSemantics { get; private set; } = default!;
    public virtual Action<RawKeyEvent>? onKey { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    public RawKeyboardListener(
        Key? key = null,
        FocusNode focusNode = default!,
        bool autofocus = false,
        bool includeSemantics = true,
        Action<RawKeyEvent>? onKey = null,
        Widget child = default!
    )
        : base(key: key)
    {
        this.focusNode = focusNode;
        this.autofocus = autofocus;
        this.includeSemantics = includeSemantics;
        this.onKey = onKey;
        this.child = child;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _RawKeyboardListenerState__raw_keyboard_listener()
        );

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<FocusNode>("focusNode", focusNode));
    }
}

internal class _RawKeyboardListenerState__raw_keyboard_listener : State<RawKeyboardListener>
{
    internal virtual bool _listening { get; set; } = false;

    public override void initState()
    {
        base.initState();
        widget.focusNode.addListener(_handleFocusChanged);
    }

    public override void didUpdateWidget(RawKeyboardListener oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(widget.focusNode, oldWidget.focusNode))
        {
            oldWidget.focusNode.removeListener(_handleFocusChanged);
            widget.focusNode.addListener(_handleFocusChanged);
        }
    }

    public override void dispose()
    {
        widget.focusNode.removeListener(_handleFocusChanged);
        _detachKeyboardIfAttached();
        base.dispose();
    }

    internal virtual void _handleFocusChanged()
    {
        if (widget.focusNode.hasFocus)
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
        if (_listening)
        {
            return;
        }
        RawKeyboard.instance.addListener(_handleRawKeyEvent);
        _listening = true;
    }

    internal virtual void _detachKeyboardIfAttached()
    {
        if (!_listening)
        {
            return;
        }
        RawKeyboard.instance.removeListener(_handleRawKeyEvent);
        _listening = false;
    }

    internal virtual void _handleRawKeyEvent(RawKeyEvent @event)
    {
        widget.onKey?.Invoke(@event);
    }

    public override Widget build(BuildContext context)
    {
        return new Focus(
            focusNode: widget.focusNode,
            autofocus: widget.autofocus,
            includeSemantics: widget.includeSemantics,
            child: widget.child
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
