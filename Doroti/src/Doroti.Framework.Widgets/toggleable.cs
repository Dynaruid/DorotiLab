// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/toggleable.dart
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

public static partial class ToggleableLibrary
{
    public static Duration _kToggleDuration = Duration.Create(milliseconds: 200L);
}

public static partial class ToggleableLibrary
{
    public static Duration _kReactionFadeDuration = Duration.Create(milliseconds: 50L);
}

public interface ToggleableStateMixin<S> where S : StatefulWidget
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
    public void initState();
    public void animateToValue();
    public void dispose();
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
        get => this._position!;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._position)))
            {
                return;
            }
            this._position?.removeListener(() => this.notifyListeners());
            __value.addListener(() => this.notifyListeners());
            _position = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Animation.Animation<double> reaction
    {
        get => this._reaction!;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._reaction)))
            {
                return;
            }
            this._reaction?.removeListener(() => this.notifyListeners());
            __value.addListener(() => this.notifyListeners());
            _reaction = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Animation.Animation<double> reactionFocusFade
    {
        get => this._reactionFocusFade!;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._reactionFocusFade)))
            {
                return;
            }
            this._reactionFocusFade?.removeListener(() => this.notifyListeners());
            __value.addListener(() => this.notifyListeners());
            _reactionFocusFade = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Animation.Animation<double> reactionHoverFade
    {
        get => this._reactionHoverFade!;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._reactionHoverFade)))
            {
                return;
            }
            this._reactionHoverFade?.removeListener(() => this.notifyListeners());
            __value.addListener(() => this.notifyListeners());
            _reactionHoverFade = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color activeColor
    {
        get => this._activeColor!;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(this._activeColor, __value)))
            {
                return;
            }
            _activeColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color inactiveColor
    {
        get => this._inactiveColor!;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(this._inactiveColor, __value)))
            {
                return;
            }
            _inactiveColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color inactiveReactionColor
    {
        get => this._inactiveReactionColor!;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(__value, this._inactiveReactionColor)))
            {
                return;
            }
            _inactiveReactionColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color reactionColor
    {
        get => this._reactionColor!;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(__value, this._reactionColor)))
            {
                return;
            }
            _reactionColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color hoverColor
    {
        get => this._hoverColor!;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(__value, this._hoverColor)))
            {
                return;
            }
            _hoverColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color focusColor
    {
        get => this._focusColor!;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(__value, this._focusColor)))
            {
                return;
            }
            _focusColor = __value;
            notifyListeners();
        }
    }
    public virtual double splashRadius
    {
        get => DartRuntimePrimitives.RequireValue(this._splashRadius);
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) == this._splashRadius))
            {
                return;
            }
            _splashRadius = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Offset? downPosition
    {
        get => this._downPosition;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._downPosition)))
            {
                return;
            }
            _downPosition = __value;
            notifyListeners();
        }
    }
    public virtual bool isFocused
    {
        get => DartRuntimePrimitives.RequireValue(this._isFocused);
        set
        {
            bool? __value = value;
            if ((__value == this._isFocused))
            {
                return;
            }
            _isFocused = __value;
            notifyListeners();
        }
    }
    public virtual bool isHovered
    {
        get => DartRuntimePrimitives.RequireValue(this._isHovered);
        set
        {
            bool? __value = value;
            if ((__value == this._isHovered))
            {
                return;
            }
            _isHovered = __value;
            notifyListeners();
        }
    }
    public virtual bool isActive
    {
        get => DartRuntimePrimitives.RequireValue(this._isActive);
        set
        {
            bool? __value = value;
            if ((__value == this._isActive))
            {
                return;
            }
            _isActive = __value;
            notifyListeners();
        }
    }
    public virtual void paintRadialReaction(Canvas canvas, Offset offset = default, Offset origin = default!)
    {
        if (((!((global::Doroti.Framework.Animation.Animation<double>)this.reaction).isDismissed || !((global::Doroti.Framework.Animation.Animation<double>)this.reactionFocusFade).isDismissed) || !((global::Doroti.Framework.Animation.Animation<double>)this.reactionHoverFade).isDismissed))
        {
            var reactionPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = Dart_uiLibrary.Color.lerp(Dart_uiLibrary.Color.lerp(Dart_uiLibrary.Color.lerp(this.inactiveReactionColor, this.reactionColor, ((global::Doroti.Framework.Animation.Animation<double>)this.position).value), this.hoverColor, ((global::Doroti.Framework.Animation.Animation<double>)this.reactionHoverFade).value), this.focusColor, ((global::Doroti.Framework.Animation.Animation<double>)this.reactionFocusFade).value)!;
    return __cascade;
}))();
            global::Doroti.Framework.Animation.Animatable<double> radialReactionRadiusTween = ((global::Doroti.Framework.Animation.Animatable<double>)(object?)new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: this.splashRadius));
            double reactionRadius = ((this.isFocused || this.isHovered) ? this.splashRadius : radialReactionRadiusTween.evaluate(this.reaction));
            if ((reactionRadius > 0.0))
            {
                canvas.drawCircle((origin + offset), reactionRadius, reactionPaint);
            }
        }
    }

    public abstract void paint(Canvas canvas, Size size);

    public virtual void dispose()
    {
        this._position?.removeListener(() => this.notifyListeners());
        this._reaction?.removeListener(() => this.notifyListeners());
        this._reactionFocusFade?.removeListener(() => this.notifyListeners());
        this._reactionHoverFade?.removeListener(() => this.notifyListeners());
        base.dispose();
    }

    public virtual bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate) => true;
    public virtual bool? hitTest(Offset position) => null;
    public virtual global::System.Func<Size, List<global::Doroti.Framework.Rendering.CustomPainterSemantics>>? semanticsBuilder => DartRuntimePrimitives.ConvertValue<global::System.Func<Size, List<global::Doroti.Framework.Rendering.CustomPainterSemantics>>>(null);
    public virtual bool shouldRebuildSemantics(global::Doroti.Framework.Rendering.CustomPainter oldDelegate) => false;
    public override string ToString() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
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
