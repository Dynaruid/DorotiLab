// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/toggleable.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public static partial class ToggleableLibrary
{
    public static Duration _kToggleDuration = Duration.Create(milliseconds: 200L);
}

public static partial class ToggleableLibrary
{
    public static Duration _kReactionFadeDuration = Duration.Create(milliseconds: 50L);
}

public interface ToggleableStateMixin<S> : IToggleableState where S : StatefulWidget
{
}

/// <summary>State and animation contract shared by toggle controls of any value type.</summary>
public interface IToggleableState : IState
{
    global::Doroti.Framework.Animation.AnimationController _positionController { get; set; }
    global::Doroti.Framework.Animation.CurvedAnimation _position { get; set; }
    global::Doroti.Framework.Animation.AnimationController _reactionController { get; set; }
    global::Doroti.Framework.Animation.CurvedAnimation _reaction { get; set; }
    global::Doroti.Framework.Animation.CurvedAnimation _reactionHoverFade { get; set; }
    global::Doroti.Framework.Animation.AnimationController _reactionHoverFadeController { get; set; }
    global::Doroti.Framework.Animation.CurvedAnimation _reactionFocusFade { get; set; }
    global::Doroti.Framework.Animation.AnimationController _reactionFocusFadeController { get; set; }
    Duration _reactionAnimationDuration { get; }
    DartMap<Type, dynamic> _actionMap { get; }
    Offset? _downPosition { get; set; }
    bool _focused { get; set; }
    bool _hovering { get; set; }

    public global::Doroti.Framework.Animation.AnimationController positionController { get; }
    public global::Doroti.Framework.Animation.CurvedAnimation position { get; }
    public global::Doroti.Framework.Animation.AnimationController reactionController { get; }
    public global::Doroti.Framework.Animation.CurvedAnimation reaction { get; }
    public global::Doroti.Framework.Animation.CurvedAnimation reactionHoverFade { get; }
    public global::Doroti.Framework.Animation.CurvedAnimation reactionFocusFade { get; }
    public Duration? reactionAnimationDuration { get; }
    public bool isInteractive { get; }
    public global::System.Action<bool?>? onChanged { get; }
    public bool? value { get; }
    public bool tristate { get; }
    public new void initState();
    public void animateToValue();
    public new void dispose();
    public global::Doroti.Ui.Offset? downPosition { get; }
    public void _handleTapDown(global::Doroti.Framework.Gestures.TapDownDetails details);
    public void _handleTap(Intent? __unused0 = null);
    public void _handleTapEnd(global::Doroti.Framework.Gestures.TapUpDetails? __unused0 = null);
    public void _handleFocusHighlightChanged(bool focused);
    public void _handleHoverChanged(bool hovering);
    public HashSet<WidgetState> states { get; }
    public Widget buildToggleableWithChild(FocusNode? focusNode = null, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor>? mouseCursor = null, Widget child = default!);
}

public abstract class ToggleablePainter : global::Doroti.Framework.Foundation.ChangeNotifier
{
    internal virtual global::Doroti.Framework.Animation.Animation<double>? _position { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.Animation<double>? _reaction { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.Animation<double>? _reactionFocusFade { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.Animation<double>? _reactionHoverFade { get; set; } = default;
    internal virtual Color? _activeColor { get; set; } = default;
    internal virtual Color? _inactiveColor { get; set; } = default;
    internal virtual Color? _inactiveReactionColor { get; set; } = default;
    internal virtual Color? _reactionColor { get; set; } = default;
    internal virtual Color? _hoverColor { get; set; } = default;
    internal virtual Color? _focusColor { get; set; } = default;
    internal virtual double? _splashRadius { get; set; } = default;
    internal virtual Offset? _downPosition { get; set; } = default;
    internal virtual bool? _isFocused { get; set; } = default;
    internal virtual bool? _isHovered { get; set; } = default;
    internal virtual bool? _isActive { get; set; } = default;

    public virtual global::Doroti.Framework.Animation.Animation<double> position
    {
        get => _position!;
        set
        {
            var __value = value;
            if (Equals(__value, _position))
            {
                return;
            }
            _position?.removeListener(notifyListeners);
            __value.addListener(notifyListeners);
            _position = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Animation.Animation<double> reaction
    {
        get => _reaction!;
        set
        {
            var __value = value;
            if (Equals(__value, _reaction))
            {
                return;
            }
            _reaction?.removeListener(notifyListeners);
            __value.addListener(notifyListeners);
            _reaction = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Animation.Animation<double> reactionFocusFade
    {
        get => _reactionFocusFade!;
        set
        {
            var __value = value;
            if (Equals(__value, _reactionFocusFade))
            {
                return;
            }
            _reactionFocusFade?.removeListener(notifyListeners);
            __value.addListener(notifyListeners);
            _reactionFocusFade = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Animation.Animation<double> reactionHoverFade
    {
        get => _reactionHoverFade!;
        set
        {
            var __value = value;
            if (Equals(__value, _reactionHoverFade))
            {
                return;
            }
            _reactionHoverFade?.removeListener(notifyListeners);
            __value.addListener(notifyListeners);
            _reactionHoverFade = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color activeColor
    {
        get => _activeColor!;
        set
        {
            var __value = value;
            if (Equals(_activeColor, __value))
            {
                return;
            }
            _activeColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color inactiveColor
    {
        get => _inactiveColor!;
        set
        {
            var __value = value;
            if (Equals(_inactiveColor, __value))
            {
                return;
            }
            _inactiveColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color inactiveReactionColor
    {
        get => _inactiveReactionColor!;
        set
        {
            var __value = value;
            if (Equals(__value, _inactiveReactionColor))
            {
                return;
            }
            _inactiveReactionColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color reactionColor
    {
        get => _reactionColor!;
        set
        {
            var __value = value;
            if (Equals(__value, _reactionColor))
            {
                return;
            }
            _reactionColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color hoverColor
    {
        get => _hoverColor!;
        set
        {
            var __value = value;
            if (Equals(__value, _hoverColor))
            {
                return;
            }
            _hoverColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color focusColor
    {
        get => _focusColor!;
        set
        {
            var __value = value;
            if (Equals(__value, _focusColor))
            {
                return;
            }
            _focusColor = __value;
            notifyListeners();
        }
    }
    public virtual double splashRadius
    {
        get => DartRuntimePrimitives.RequireValue(_splashRadius);
        set
        {
            var __value = value;
            if (DartRuntimePrimitives.RequireValue(__value) == _splashRadius)
            {
                return;
            }
            _splashRadius = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Offset? downPosition
    {
        get => _downPosition;
        set
        {
            var __value = value;
            if (Equals(__value, _downPosition))
            {
                return;
            }
            _downPosition = __value;
            notifyListeners();
        }
    }
    public virtual bool isFocused
    {
        get => DartRuntimePrimitives.RequireValue(_isFocused);
        set
        {
            bool? __value = value;
            if (__value == _isFocused)
            {
                return;
            }
            _isFocused = __value;
            notifyListeners();
        }
    }
    public virtual bool isHovered
    {
        get => DartRuntimePrimitives.RequireValue(_isHovered);
        set
        {
            bool? __value = value;
            if (__value == _isHovered)
            {
                return;
            }
            _isHovered = __value;
            notifyListeners();
        }
    }
    public virtual bool isActive
    {
        get => DartRuntimePrimitives.RequireValue(_isActive);
        set
        {
            bool? __value = value;
            if (__value == _isActive)
            {
                return;
            }
            _isActive = __value;
            notifyListeners();
        }
    }
    public virtual void paintRadialReaction(Canvas canvas, Offset offset = default, Offset origin = default!)
    {
        if (!reaction.isDismissed || !reactionFocusFade.isDismissed || !reactionHoverFade.isDismissed)
        {
            var reactionPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = Dart_uiLibrary.Color.lerp(Dart_uiLibrary.Color.lerp(Dart_uiLibrary.Color.lerp(inactiveReactionColor, reactionColor, position.value), hoverColor, reactionHoverFade.value), focusColor, reactionFocusFade.value)!;
    return __cascade;
}))();
            global::Doroti.Framework.Animation.Animatable<double> radialReactionRadiusTween = new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: splashRadius);
            double reactionRadius = (isFocused || isHovered) ? splashRadius : radialReactionRadiusTween.evaluate(reaction);
            if (reactionRadius > 0.0)
            {
                canvas.drawCircle(origin + offset, reactionRadius, reactionPaint);
            }
        }
    }

    public abstract void paint(Canvas canvas, Size size);

    public override void dispose()
    {
        _position?.removeListener(notifyListeners);
        _reaction?.removeListener(notifyListeners);
        _reactionFocusFade?.removeListener(notifyListeners);
        _reactionHoverFade?.removeListener(notifyListeners);
        base.dispose();
    }

    public virtual bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate) => true;
    public virtual bool? hitTest(Offset position) => null;
    public virtual global::System.Func<Size, List<global::Doroti.Framework.Rendering.CustomPainterSemantics>>? semanticsBuilder => DartRuntimePrimitives.ConvertValue<global::System.Func<Size, List<global::Doroti.Framework.Rendering.CustomPainterSemantics>>>(null);
    public virtual bool shouldRebuildSemantics(global::Doroti.Framework.Rendering.CustomPainter oldDelegate) => false;
    public override string ToString() => DiagnosticsLibrary.describeIdentity(this);
}

internal sealed class ToggleableCustomPainterAdapter : global::Doroti.Framework.Rendering.CustomPainter
{
    private readonly ToggleablePainter _owner;

    internal ToggleableCustomPainterAdapter(ToggleablePainter owner) : base(owner) =>
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));

    public override void paint(Canvas canvas, Size size) => _owner.paint(canvas, size);

    public override bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate) =>
        oldDelegate is not ToggleableCustomPainterAdapter other || !ReferenceEquals(_owner, other._owner);

    public override bool? hitTest(Offset position) => _owner.hitTest(position);

    public override global::System.Func<Size, List<global::Doroti.Framework.Rendering.CustomPainterSemantics>>? semanticsBuilder =>
        _owner.semanticsBuilder;

    public override bool shouldRebuildSemantics(global::Doroti.Framework.Rendering.CustomPainter oldDelegate) =>
        oldDelegate is not ToggleableCustomPainterAdapter other ||
        !ReferenceEquals(_owner, other._owner) ||
        _owner.shouldRebuildSemantics(oldDelegate);
}
