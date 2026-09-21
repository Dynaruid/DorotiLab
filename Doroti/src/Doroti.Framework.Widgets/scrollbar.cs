// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/scrollbar.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public static partial class ScrollbarLibrary
{
    internal static double _kMinThumbExtent = 18.0;
}

public static partial class ScrollbarLibrary
{
    internal static double _kMinInteractiveSize = 48.0;
}

public static partial class ScrollbarLibrary
{
    internal static double _kScrollbarThickness = 6.0;
}

public static partial class ScrollbarLibrary
{
    internal static Duration _kScrollbarFadeDuration = Duration.Create(milliseconds: 300L);
}

public static partial class ScrollbarLibrary
{
    internal static Duration _kScrollbarTimeToFade = Duration.Create(milliseconds: 600L);
}

public enum ScrollbarOrientation
{
    left,
    right,
    top,
    bottom,
}

public class ScrollbarPainter : ChangeNotifier
{
    internal virtual Color _color { get; set; } = default!;
    internal virtual Color _trackColor { get; set; } = default!;
    internal virtual Color _trackBorderColor { get; set; } = default!;
    internal virtual Radius? _trackRadius { get; set; } = default;
    internal virtual TextDirection? _textDirection { get; set; } = default;
    internal virtual double _thickness { get; set; } = default!;
    public virtual Animation<double> fadeoutOpacityAnimation { get; private set; } = default!;
    internal virtual double _mainAxisMargin { get; set; } = default!;
    internal virtual double _crossAxisMargin { get; set; } = default!;
    internal virtual Radius? _radius { get; set; } = default;
    internal virtual OutlinedBorder? _shape { get; set; } = default;
    internal virtual EdgeInsetsGeometry _padding { get; set; } = default!;
    internal virtual double _minLength { get; set; } = default!;
    internal virtual double _minOverscrollLength { get; set; } = default!;
    internal virtual ScrollbarOrientation? _scrollbarOrientation { get; set; } = default;
    internal virtual bool _ignorePointer { get; set; } = default!;
    internal virtual Rect? _trackRect { get; set; } = default;
    internal virtual EdgeInsets? _resolvedPadding { get; set; } = default;
    internal virtual Rect? _thumbRect { get; set; } = default;
    internal virtual double _thumbOffset { get; set; } = default!;
    internal virtual double _thumbExtent { get; set; } = default!;
    internal virtual ScrollMetrics? _lastMetrics { get; set; } = default;
    internal virtual AxisDirection? _lastAxisDirection { get; set; } = default;

    public ScrollbarPainter(
        Color color,
        Animation<double> fadeoutOpacityAnimation,
        Color trackColor = default!,
        Color trackBorderColor = default!,
        TextDirection? textDirection = null,
        double? thickness = null,
        EdgeInsetsGeometry padding = default!,
        double mainAxisMargin = 0.0,
        double crossAxisMargin = 0.0,
        Radius? radius = null,
        Radius? trackRadius = null,
        OutlinedBorder? shape = null,
        double? minLength = null,
        double? minOverscrollLength = null,
        ScrollbarOrientation? scrollbarOrientation = null,
        bool ignorePointer = false
    )
    {
        Color __trackColor = trackColor ?? new Color(0x00000000);
        Color __trackBorderColor = trackBorderColor ?? new Color(0x00000000);
        double __thickness = thickness ?? ScrollbarLibrary._kScrollbarThickness;
        EdgeInsetsGeometry __padding = padding ?? EdgeInsets.zero;
        double __minLength = minLength ?? ScrollbarLibrary._kMinThumbExtent;
        this.fadeoutOpacityAnimation = fadeoutOpacityAnimation;
        _color = color;
        _textDirection = textDirection;
        _thickness = __thickness;
        _radius = radius;
        _shape = shape;
        _padding = __padding;
        _resolvedPadding = __padding.resolve(textDirection);
        _mainAxisMargin = mainAxisMargin;
        _crossAxisMargin = crossAxisMargin;
        _minLength = __minLength;
        _trackColor = __trackColor;
        _trackBorderColor = __trackBorderColor;
        _trackRadius = trackRadius;
        _scrollbarOrientation = scrollbarOrientation;
        _minOverscrollLength = minOverscrollLength ?? __minLength;
        _ignorePointer = ignorePointer;
        this.fadeoutOpacityAnimation.addListener(notifyListeners);
        System.Diagnostics.Debug.Assert((radius is null) || (shape is null));
        System.Diagnostics.Debug.Assert(__minLength >= 0L);
        System.Diagnostics.Debug.Assert(
            (minOverscrollLength is null) || (minOverscrollLength <= __minLength)
        );
        System.Diagnostics.Debug.Assert(
            (minOverscrollLength is null) || (minOverscrollLength >= 0L)
        );
        System.Diagnostics.Debug.Assert(__padding.isNonNegative);
        System.Diagnostics.Debug.Assert(
            (__padding is not EdgeInsetsDirectional) || (textDirection is not null)
        );
    }

    public virtual Color color
    {
        get => _color;
        set
        {
            var __value = value;
            if (Equals(color, __value))
            {
                return;
            }
            _color = __value;
            notifyListeners();
        }
    }
    public virtual Color trackColor
    {
        get => _trackColor;
        set
        {
            var __value = value;
            if (Equals(trackColor, __value))
            {
                return;
            }
            _trackColor = __value;
            notifyListeners();
        }
    }
    public virtual Color trackBorderColor
    {
        get => _trackBorderColor;
        set
        {
            var __value = value;
            if (Equals(trackBorderColor, __value))
            {
                return;
            }
            _trackBorderColor = __value;
            notifyListeners();
        }
    }
    public virtual Radius? trackRadius
    {
        get => _trackRadius;
        set
        {
            var __value = value;
            if (Equals(trackRadius, __value))
            {
                return;
            }
            _trackRadius = __value;
            notifyListeners();
        }
    }
    public virtual TextDirection? textDirection
    {
        get => _textDirection;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value is not null);
            if (Equals(textDirection, __value))
            {
                return;
            }
            _textDirection = __value;
            _resolvedPadding = _padding.resolve(_textDirection);
            notifyListeners();
        }
    }
    public virtual double thickness
    {
        get => _thickness;
        set
        {
            var __value = value;
            if (thickness == (__value))
            {
                return;
            }
            _thickness = (__value);
            notifyListeners();
        }
    }
    public virtual double mainAxisMargin
    {
        get => _mainAxisMargin;
        set
        {
            var __value = value;
            if (mainAxisMargin == (__value))
            {
                return;
            }
            _mainAxisMargin = (__value);
            notifyListeners();
        }
    }
    public virtual double crossAxisMargin
    {
        get => _crossAxisMargin;
        set
        {
            var __value = value;
            if (crossAxisMargin == (__value))
            {
                return;
            }
            _crossAxisMargin = (__value);
            notifyListeners();
        }
    }
    public virtual Radius? radius
    {
        get => _radius;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (shape is null) || (__value is null));
            if (Equals(radius, __value))
            {
                return;
            }
            _radius = __value;
            notifyListeners();
        }
    }
    public virtual OutlinedBorder? shape
    {
        get => _shape;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (radius is null) || (__value is null));
            if (Equals(shape, __value))
            {
                return;
            }
            _shape = __value;
            notifyListeners();
        }
    }
    public virtual EdgeInsetsGeometry padding
    {
        get => _padding;
        set
        {
            var __value = value;
            if (Equals(padding, __value))
            {
                return;
            }
            _padding = __value;
            _resolvedPadding = _padding.resolve(_textDirection);
            notifyListeners();
        }
    }
    public virtual double minLength
    {
        get => _minLength;
        set
        {
            var __value = value;
            if (minLength == (__value))
            {
                return;
            }
            _minLength = (__value);
            notifyListeners();
        }
    }
    public virtual double minOverscrollLength
    {
        get => _minOverscrollLength;
        set
        {
            var __value = value;
            if (minOverscrollLength == (__value))
            {
                return;
            }
            _minOverscrollLength = (__value);
            notifyListeners();
        }
    }
    public virtual ScrollbarOrientation? scrollbarOrientation
    {
        get => _scrollbarOrientation;
        set
        {
            var __value = value;
            if (Equals(scrollbarOrientation, __value))
            {
                return;
            }
            _scrollbarOrientation = __value;
            notifyListeners();
        }
    }
    public virtual bool ignorePointer
    {
        get => _ignorePointer;
        set
        {
            var __value = value;
            if (ignorePointer == (__value))
            {
                return;
            }
            _ignorePointer = (__value);
            notifyListeners();
        }
    }
    internal virtual double _trackExtent =>
        DartRuntimePrimitives.ConvertValue<double>(
            _lastMetrics!.viewportDimension - _totalTrackMainAxisOffsets
        );
    internal virtual double _traversableTrackExtent =>
        DartRuntimePrimitives.ConvertValue<double>(_trackExtent - (2L * mainAxisMargin));
    internal virtual double _totalTrackMainAxisOffsets =>
        _isVertical ? _resolvedPadding!.vertical : _resolvedPadding!.horizontal;
    internal virtual double _leadingTrackMainAxisOffset =>
        _resolvedOrientation switch
        {
            ScrollbarOrientation.left => _resolvedPadding!.top,
            ScrollbarOrientation.right => _resolvedPadding!.top,
            ScrollbarOrientation.top => _resolvedPadding!.left,
            ScrollbarOrientation.bottom => _resolvedPadding!.left,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
    internal virtual double _leadingThumbMainAxisOffset =>
        DartRuntimePrimitives.ConvertValue<double>(_leadingTrackMainAxisOffset + mainAxisMargin);

    internal virtual void _setThumbExtent()
    {
        double fractionVisible = DorotiUiLibrary.clampDouble(
            (_lastMetrics!.extentInside - _totalTrackMainAxisOffsets)
                / (_totalContentExtent - _totalTrackMainAxisOffsets),
            0.0,
            1.0
        );
        double thumbExtent = Math.Max(
            Math.Min(_traversableTrackExtent, minOverscrollLength),
            _traversableTrackExtent * fractionVisible
        );
        double fractionOverscrolled =
            1.0 - (_lastMetrics!.extentInside / _lastMetrics!.viewportDimension);
        double safeMinLength = Math.Min((minLength), _traversableTrackExtent);
        double newMinLength =
            ((_beforeExtent > 0L) && (_afterExtent > 0L))
                ? safeMinLength
                : (
                    safeMinLength
                    * (1.0 - (DorotiUiLibrary.clampDouble(fractionOverscrolled, 0.0, 0.2) / 0.2))
                );
        _thumbExtent = DorotiUiLibrary.clampDouble(
            thumbExtent,
            newMinLength,
            _traversableTrackExtent
        );
    }

    internal virtual bool _lastMetricsAreScrollable =>
        DartRuntimePrimitives.ConvertValue<bool>(
            _lastMetrics!.minScrollExtent != _lastMetrics!.maxScrollExtent
        );
    internal virtual bool _isVertical =>
        DartRuntimePrimitives.ConvertValue<bool>(
            Equals(_lastAxisDirection, AxisDirection.down)
                || Equals(_lastAxisDirection, AxisDirection.up)
        );
    internal virtual bool _isReversed =>
        DartRuntimePrimitives.ConvertValue<bool>(
            Equals(_lastAxisDirection, AxisDirection.up)
                || Equals(_lastAxisDirection, AxisDirection.left)
        );
    internal virtual double _beforeExtent =>
        _isReversed ? _lastMetrics!.extentAfter : _lastMetrics!.extentBefore;
    internal virtual double _afterExtent =>
        _isReversed ? _lastMetrics!.extentBefore : _lastMetrics!.extentAfter;
    internal virtual double _totalContentExtent
    {
        get
        {
            return _lastMetrics!.maxScrollExtent
                - _lastMetrics!.minScrollExtent
                + _lastMetrics!.viewportDimension;
        }
    }
    internal virtual ScrollbarOrientation _resolvedOrientation
    {
        get
        {
            if (scrollbarOrientation is null)
            {
                if (_isVertical)
                {
                    return Equals(textDirection, TextDirection.ltr)
                        ? ScrollbarOrientation.right
                        : ScrollbarOrientation.left;
                }
                return ScrollbarOrientation.bottom;
            }
            return (
                scrollbarOrientation
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        }
    }

    internal virtual void _debugAssertIsValidOrientation(ScrollbarOrientation orientation)
    {
        DartRuntimePrimitives.Assert(
            () =>
            {
                bool isVerticalOrientation(ScrollbarOrientation orientation)
                {
                    return Equals(orientation, ScrollbarOrientation.left)
                        || Equals(orientation, ScrollbarOrientation.right);
                    throw new InvalidOperationException(
                        "Control flow completed without returning a value."
                    );
                }
                return (_isVertical && isVerticalOrientation(orientation))
                    || (!_isVertical && !isVerticalOrientation(orientation));
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            },
            () =>
                (object?)$"The given ScrollbarOrientation: {orientation} is incompatible with the "
                + $"current AxisDirection: {_lastAxisDirection}."
        );
    }

    public virtual void update(ScrollMetrics metrics, AxisDirection axisDirection)
    {
        if (
            (_lastMetrics is not null)
            && (_lastMetrics!.extentBefore == metrics.extentBefore)
            && (_lastMetrics!.extentInside == metrics.extentInside)
            && (_lastMetrics!.extentAfter == metrics.extentAfter)
            && Equals(_lastAxisDirection, axisDirection)
        )
        {
            return;
        }
        ScrollMetrics? oldMetrics = _lastMetrics;
        _lastMetrics = metrics;
        _lastAxisDirection = axisDirection;
        if (!_needPaint(oldMetrics) && !_needPaint(metrics))
        {
            return;
        }
        notifyListeners();
    }

    public virtual void updateThickness(double nextThickness, Radius nextRadius)
    {
        thickness = nextThickness;
        radius = nextRadius;
    }

    internal virtual Paint _paintThumb
    {
        get
        {
            return (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = new Paint();
                        __cascade.color = color.withOpacity(
                            color.opacity * fadeoutOpacityAnimation.value
                        );
                        return __cascade;
                    }
                )
            )();
        }
    }

    internal virtual bool _needPaint(ScrollMetrics? metrics)
    {
        return (metrics is not null)
            && (
                (metrics.maxScrollExtent - metrics.minScrollExtent)
                > Foundation.ConstantsLibrary.precisionErrorTolerance
            );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Paint _paintTrack(bool isBorder = false)
    {
        if (isBorder)
        {
            return (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = new Paint();
                        __cascade.color = trackBorderColor.withOpacity(
                            trackBorderColor.opacity * fadeoutOpacityAnimation.value
                        );
                        __cascade.style = PaintingStyle.stroke;
                        __cascade.strokeWidth = 1.0;
                        return __cascade;
                    }
                )
            )();
        }
        return (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.color = trackColor.withOpacity(
                        trackColor.opacity * fadeoutOpacityAnimation.value
                    );
                    return __cascade;
                }
            )
        )();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _paintScrollbar(Canvas canvas, Size size)
    {
        DartRuntimePrimitives.Assert(
            () => textDirection is not null,
            () => (object?)"A TextDirection must be provided before a Scrollbar can be painted."
        );
        double x = default!;
        double y = default!;
        Size thumbSize = default!;
        Size trackSize = default!;
        Offset trackOffset = default!;
        Offset borderStart = default!;
        Offset borderEnd = default!;
        _debugAssertIsValidOrientation(_resolvedOrientation);
        switch (_resolvedOrientation)
        {
            case ScrollbarOrientation.left:
            {
                thumbSize = new Size((thickness), _thumbExtent);
                trackSize = new Size(thickness + (2L * crossAxisMargin), _trackExtent);
                x = crossAxisMargin + _resolvedPadding!.left;
                y = _thumbOffset;
                trackOffset = new Offset(x - crossAxisMargin, _leadingTrackMainAxisOffset);
                borderStart = trackOffset + new Offset(trackSize.width, 0.0);
                borderEnd = new Offset(
                    trackOffset.dx + trackSize.width,
                    trackOffset.dy + _trackExtent
                );
                break;
            }
            case ScrollbarOrientation.right:
            {
                thumbSize = new Size((thickness), _thumbExtent);
                trackSize = new Size(thickness + (2L * crossAxisMargin), _trackExtent);
                x = size.width - thickness - crossAxisMargin - _resolvedPadding!.right;
                y = _thumbOffset;
                trackOffset = new Offset(x - crossAxisMargin, _leadingTrackMainAxisOffset);
                borderStart = trackOffset;
                borderEnd = new Offset(trackOffset.dx, trackOffset.dy + _trackExtent);
                break;
            }
            case ScrollbarOrientation.top:
            {
                thumbSize = new Size(_thumbExtent, (thickness));
                trackSize = new Size(_trackExtent, thickness + (2L * crossAxisMargin));
                x = _thumbOffset;
                y = crossAxisMargin + _resolvedPadding!.top;
                trackOffset = new Offset(_leadingTrackMainAxisOffset, y - crossAxisMargin);
                borderStart = trackOffset + new Offset(0.0, trackSize.height);
                borderEnd = new Offset(
                    trackOffset.dx + _trackExtent,
                    trackOffset.dy + trackSize.height
                );
                break;
            }
            case ScrollbarOrientation.bottom:
            {
                thumbSize = new Size(_thumbExtent, (thickness));
                trackSize = new Size(_trackExtent, thickness + (2L * crossAxisMargin));
                x = _thumbOffset;
                y = size.height - thickness - crossAxisMargin - _resolvedPadding!.bottom;
                trackOffset = new Offset(_leadingTrackMainAxisOffset, y - crossAxisMargin);
                borderStart = trackOffset;
                borderEnd = new Offset(trackOffset.dx + _trackExtent, trackOffset.dy);
                break;
            }
        }
        _trackRect = trackOffset & trackSize;
        _thumbRect = new Offset(x, y) & thumbSize;
        if (fadeoutOpacityAnimation.value != 0.0)
        {
            if (trackRadius is null)
            {
                canvas.drawRect(
                    (
                        _trackRect
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ),
                    _paintTrack()
                );
            }
            else
            {
                canvas.drawRRect(
                    RRect.fromRectAndRadius(
                        (
                            _trackRect
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        ),
                        (
                            trackRadius
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        )
                    ),
                    _paintTrack()
                );
            }
            canvas.drawLine(borderStart, borderEnd, _paintTrack(isBorder: true));
            if (radius is not null)
            {
                Radius radius__value22874 = (
                    radius
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
                canvas.drawRRect(
                    RRect.fromRectAndRadius(
                        (
                            _thumbRect
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        ),
                        (
                            radius
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        )
                    ),
                    _paintThumb
                );
                return;
            }
            if (shape is null)
            {
                canvas.drawRect(
                    (
                        _thumbRect
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ),
                    _paintThumb
                );
                return;
            }
            if (shape!.preferPaintInterior)
            {
                shape!.paintInterior(
                    canvas,
                    (
                        _thumbRect
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ),
                    _paintThumb
                );
            }
            else
            {
                Path outerPath = shape!.getOuterPath(
                    (
                        _thumbRect
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                );
                canvas.drawPath(outerPath, _paintThumb);
            }
            shape!.paint(
                canvas,
                (
                    _thumbRect
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
            );
        }
    }

    public virtual void paint(Canvas canvas, Size size)
    {
        if ((_lastAxisDirection is null) || !_needPaint(_lastMetrics))
        {
            return;
        }
        if (_traversableTrackExtent <= 0L)
        {
            return;
        }
        if (double.IsInfinity(_lastMetrics!.maxScrollExtent))
        {
            return;
        }
        _setThumbExtent();
        double thumbPositionOffset = _getScrollToTrack(_lastMetrics!, _thumbExtent);
        _thumbOffset = thumbPositionOffset + _leadingThumbMainAxisOffset;
        _paintScrollbar(canvas, size);
        return;
    }

    public virtual double getTrackToScroll(double thumbOffsetLocal)
    {
        double scrollableExtent = _lastMetrics!.maxScrollExtent - _lastMetrics!.minScrollExtent;
        double thumbMovableExtent = _traversableTrackExtent - _thumbExtent;
        return scrollableExtent * thumbOffsetLocal / thumbMovableExtent;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual double getThumbScrollOffset()
    {
        DartRuntimePrimitives.Assert(() =>
            double.IsFinite(_lastMetrics!.maxScrollExtent)
            && double.IsFinite(_lastMetrics!.minScrollExtent)
        );
        double scrollableExtent = _lastMetrics!.maxScrollExtent - _lastMetrics!.minScrollExtent;
        double maxFraction = _lastMetrics!.maxScrollExtent / scrollableExtent;
        double minFraction = _lastMetrics!.minScrollExtent / scrollableExtent;
        double fractionPast =
            (scrollableExtent > 0L)
                ? DorotiUiLibrary.clampDouble(
                    _lastMetrics!.pixels / scrollableExtent,
                    minFraction,
                    maxFraction
                )
                : 0;
        return fractionPast * (_traversableTrackExtent - _thumbExtent);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual double _getScrollToTrack(ScrollMetrics metrics, double thumbExtent)
    {
        double scrollableExtent = metrics.maxScrollExtent - metrics.minScrollExtent;
        double fractionPast =
            (scrollableExtent > 0L)
                ? DorotiUiLibrary.clampDouble(
                    (metrics.pixels - metrics.minScrollExtent) / scrollableExtent,
                    0.0,
                    1.0
                )
                : 0;
        return (_isReversed ? (1L - fractionPast) : fractionPast)
            * (_traversableTrackExtent - thumbExtent);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool? hitTest(Offset? position)
    {
        if (_thumbRect is null)
        {
            return null;
        }
        if (ignorePointer || (fadeoutOpacityAnimation.value == 0.0) || !_lastMetricsAreScrollable)
        {
            return false;
        }
        return (
            _trackRect
            ?? throw new global::System.NullReferenceException("A required value was null.")
        ).contains(
            (
                position
                ?? throw new global::System.NullReferenceException("A required value was null.")
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool hitTestInteractive(
        Offset position,
        PointerDeviceKind kind,
        bool forHover = false
    )
    {
        if (_trackRect is null)
        {
            return false;
        }
        if (ignorePointer)
        {
            return false;
        }
        if (!_lastMetricsAreScrollable)
        {
            return false;
        }
        Rect interactiveRect = (
            _trackRect
            ?? throw new global::System.NullReferenceException("A required value was null.")
        );
        Rect paddedRect = interactiveRect.expandToInclude(
            Rect.fromCircle(
                center: (
                    _thumbRect
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ).center,
                radius: ScrollbarLibrary._kMinInteractiveSize / 2L
            )
        );
        if (fadeoutOpacityAnimation.value == 0.0)
        {
            if (forHover && Equals(kind, PointerDeviceKind.mouse))
            {
                return paddedRect.contains(((position)));
            }
            return false;
        }
        switch (kind)
        {
            case PointerDeviceKind.touch:
            case PointerDeviceKind.trackpad:
            {
                return paddedRect.contains(((position)));
            }
            case PointerDeviceKind.mouse:
            case PointerDeviceKind.stylus:
            case PointerDeviceKind.invertedStylus:
            case PointerDeviceKind.unknown:
            {
                return interactiveRect.contains(((position)));
            }
            default:
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                );
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool hitTestOnlyThumbInteractive(Offset position, PointerDeviceKind kind)
    {
        if (_thumbRect is null)
        {
            return false;
        }
        if (ignorePointer)
        {
            return false;
        }
        if (fadeoutOpacityAnimation.value == 0.0)
        {
            return false;
        }
        if (!_lastMetricsAreScrollable)
        {
            return false;
        }
        switch (kind)
        {
            case PointerDeviceKind.touch:
            case PointerDeviceKind.trackpad:
            {
                Rect touchThumbRect = (
                    _thumbRect
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ).expandToInclude(
                    Rect.fromCircle(
                        center: (
                            _thumbRect
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        ).center,
                        radius: ScrollbarLibrary._kMinInteractiveSize / 2L
                    )
                );
                return touchThumbRect.contains(((position)));
            }
            case PointerDeviceKind.mouse:
            case PointerDeviceKind.stylus:
            case PointerDeviceKind.invertedStylus:
            case PointerDeviceKind.unknown:
            {
                return (
                    _thumbRect
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ).contains(((position)));
            }
            default:
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                );
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool shouldRepaint(ScrollbarPainter oldDelegate)
    {
        return (!Equals(color, oldDelegate.color))
            || (!Equals(trackColor, oldDelegate.trackColor))
            || (!Equals(trackBorderColor, oldDelegate.trackBorderColor))
            || (!Equals(textDirection, oldDelegate.textDirection))
            || (thickness != oldDelegate.thickness)
            || (!Equals(fadeoutOpacityAnimation, oldDelegate.fadeoutOpacityAnimation))
            || (mainAxisMargin != oldDelegate.mainAxisMargin)
            || (crossAxisMargin != oldDelegate.crossAxisMargin)
            || (!Equals(radius, oldDelegate.radius))
            || (!Equals(trackRadius, oldDelegate.trackRadius))
            || (!Equals(shape, oldDelegate.shape))
            || (!Equals(padding, oldDelegate.padding))
            || (minLength != oldDelegate.minLength)
            || (minOverscrollLength != oldDelegate.minOverscrollLength)
            || (!Equals(scrollbarOrientation, oldDelegate.scrollbarOrientation))
            || (ignorePointer != oldDelegate.ignorePointer);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool shouldRebuildSemantics(CustomPainter oldDelegate) => false;

    public virtual Func<Size, List<CustomPainterSemantics>>? semanticsBuilder =>
        DartRuntimePrimitives.ConvertValue<Func<Size, List<CustomPainterSemantics>>>(null);

    public override string ToString() => DiagnosticsLibrary.describeIdentity(this);

    public override void dispose()
    {
        fadeoutOpacityAnimation.removeListener(notifyListeners);
        base.dispose();
    }
}

public class RawScrollbar : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual ScrollController? controller { get; private set; }
    public virtual bool? thumbVisibility { get; private set; }
    public virtual OutlinedBorder? shape { get; private set; }
    public virtual Radius? radius { get; private set; }
    public virtual double? thickness { get; private set; }
    public virtual Color? thumbColor { get; private set; }
    public virtual double minThumbLength { get; private set; } = default!;
    public virtual double? minOverscrollLength { get; private set; }
    public virtual bool? trackVisibility { get; private set; }
    public virtual Radius? trackRadius { get; private set; }
    public virtual Color? trackColor { get; private set; }
    public virtual Color? trackBorderColor { get; private set; }
    public virtual Duration fadeDuration { get; private set; } = default!;
    public virtual Duration timeToFade { get; private set; } = default!;
    public virtual Duration pressDuration { get; private set; } = default!;
    public virtual Func<ScrollNotification, bool> notificationPredicate { get; private set; } =
        default!;
    public virtual bool? interactive { get; private set; }
    public virtual ScrollbarOrientation? scrollbarOrientation { get; private set; }
    public virtual double mainAxisMargin { get; private set; } = default!;
    public virtual double crossAxisMargin { get; private set; } = default!;
    public virtual EdgeInsetsGeometry? padding { get; private set; }

    public RawScrollbar(
        Key? key = null,
        Widget child = default!,
        ScrollController? controller = null,
        bool? thumbVisibility = null,
        OutlinedBorder? shape = null,
        Radius? radius = null,
        double? thickness = null,
        Color? thumbColor = null,
        double? minThumbLength = null,
        double? minOverscrollLength = null,
        bool? trackVisibility = null,
        Radius? trackRadius = null,
        Color? trackColor = null,
        Color? trackBorderColor = null,
        Duration? fadeDuration = null,
        Duration? timeToFade = null,
        Duration pressDuration = default,
        Func<ScrollNotification, bool> notificationPredicate = default!,
        bool? interactive = null,
        ScrollbarOrientation? scrollbarOrientation = null,
        double mainAxisMargin = 0.0,
        double crossAxisMargin = 0.0,
        EdgeInsetsGeometry? padding = null
    )
        : base(key: key)
    {
        double __minThumbLength = minThumbLength ?? ScrollbarLibrary._kMinThumbExtent;
        Duration __fadeDuration = fadeDuration ?? ScrollbarLibrary._kScrollbarFadeDuration;
        Duration __timeToFade = timeToFade ?? ScrollbarLibrary._kScrollbarTimeToFade;
        Func<ScrollNotification, bool> __notificationPredicate =
            notificationPredicate ?? Scroll_notificationLibrary.defaultScrollNotificationPredicate;
        this.child = child;
        this.controller = controller;
        this.thumbVisibility = thumbVisibility;
        this.shape = shape;
        this.radius = radius;
        this.thickness = thickness;
        this.thumbColor = thumbColor;
        this.minThumbLength = __minThumbLength;
        this.minOverscrollLength = minOverscrollLength;
        this.trackVisibility = trackVisibility;
        this.trackRadius = trackRadius;
        this.trackColor = trackColor;
        this.trackBorderColor = trackBorderColor;
        this.fadeDuration = __fadeDuration;
        this.timeToFade = __timeToFade;
        this.pressDuration = pressDuration;
        this.notificationPredicate = __notificationPredicate;
        this.interactive = interactive;
        this.scrollbarOrientation = scrollbarOrientation;
        this.mainAxisMargin = mainAxisMargin;
        this.crossAxisMargin = crossAxisMargin;
        this.padding = padding;
        System.Diagnostics.Debug.Assert(
            !((thumbVisibility == false) && (trackVisibility ?? false))
        );
        System.Diagnostics.Debug.Assert(__minThumbLength >= 0L);
        System.Diagnostics.Debug.Assert(
            (minOverscrollLength is null) || (minOverscrollLength <= __minThumbLength)
        );
        System.Diagnostics.Debug.Assert(
            (minOverscrollLength is null) || (minOverscrollLength >= 0L)
        );
        System.Diagnostics.Debug.Assert((radius is null) || (shape is null));
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new RawScrollbarState<RawScrollbar>());
}

public class RawScrollbarState<T> : State<T>, TickerProviderStateMixin<T>
    where T : RawScrollbar
{
    internal virtual Offset? _startDragScrollbarAxisOffset { get; set; } = default;
    internal virtual Offset? _lastDragUpdateOffset { get; set; } = default;
    internal virtual double? _startDragThumbOffset { get; set; } = default;
    internal virtual ScrollController? _cachedController { get; set; } = default;
    internal virtual Timer? _fadeoutTimer { get; set; } = default;
    internal virtual bool _isDisposed { get; set; }
    internal virtual AnimationController _fadeoutAnimationController { get; set; } = default!;
    internal virtual CurvedAnimation _fadeoutOpacityAnimation { get; set; } = default!;
    internal virtual GlobalKey<IState> _scrollbarPainterKey { get; private set; } =
        GlobalKey<IState>.Create();
    internal virtual bool _hoverIsActive { get; set; } = false;
    internal virtual Drag? _thumbDrag { get; set; } = default;
    internal virtual bool _maxScrollExtentPermitsScrolling { get; set; } = false;
    internal virtual ScrollHoldController? _thumbHold { get; set; } = default;
    internal virtual Axis? _axis { get; set; } = default;
    internal virtual GlobalKey<RawGestureDetectorState> _gestureDetectorKey { get; private set; } =
        GlobalKey<RawGestureDetectorState>.Create();
    public virtual ScrollbarPainter scrollbarPainter { get; private set; } = default!;
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual ScrollController? _effectiveScrollController =>
        DartRuntimePrimitives.ConvertValue<ScrollController>(
            widget.controller ?? PrimaryScrollController.maybeOf(context)
        );
    public virtual bool showScrollbar =>
        DartRuntimePrimitives.ConvertValue<bool>(widget.thumbVisibility ?? false);
    internal virtual bool _showTrack =>
        DartRuntimePrimitives.ConvertValue<bool>(
            showScrollbar && (widget.trackVisibility ?? false)
        );
    public virtual bool enableGestures =>
        DartRuntimePrimitives.ConvertValue<bool>(widget.interactive ?? true);

    public override void initState()
    {
        base.initState();
        _fadeoutAnimationController = (
            (Func<AnimationController>)(
                () =>
                {
                    var __cascade = new AnimationController(
                        vsync: this,
                        duration: widget.fadeDuration
                    );
                    __cascade.addStatusListener(_validateInteractions);
                    return __cascade;
                }
            )
        )();
        _fadeoutOpacityAnimation = new CurvedAnimation(
            parent: _fadeoutAnimationController,
            curve: Curves.fastOutSlowIn
        );
        scrollbarPainter = new ScrollbarPainter(
            color: widget.thumbColor ?? new Color(1723645116L),
            fadeoutOpacityAnimation: _fadeoutOpacityAnimation,
            thickness: widget.thickness ?? ScrollbarLibrary._kScrollbarThickness,
            radius: widget.radius,
            trackRadius: widget.trackRadius,
            scrollbarOrientation: widget.scrollbarOrientation,
            mainAxisMargin: widget.mainAxisMargin,
            shape: widget.shape,
            crossAxisMargin: widget.crossAxisMargin,
            minLength: widget.minThumbLength,
            minOverscrollLength: widget.minOverscrollLength ?? widget.minThumbLength
        );
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        DartRuntimePrimitives.Assert(() => _debugScheduleCheckHasValidScrollPosition());
    }

    internal virtual bool _debugScheduleCheckHasValidScrollPosition()
    {
        if (!showScrollbar)
        {
            return true;
        }
        WidgetsBinding.instance.addPostFrameCallback(
            (duration) =>
            {
                DartRuntimePrimitives.Assert(() => _debugCheckHasValidScrollPosition());
            },
            debugLabel: "RawScrollbar.checkScrollPosition"
        );
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _validateInteractions(AnimationStatus status)
    {
        if (AnimationStatusMembers.isDismissed(status))
        {
            DartRuntimePrimitives.Assert(() => _fadeoutOpacityAnimation.value == 0.0);
        }
        else
        {
            if ((_effectiveScrollController is not null) && enableGestures)
            {
                if (
                    Equals(_fadeoutAnimationController.status, AnimationStatus.forward)
                    && (widget.thumbVisibility ?? false)
                )
                {
                    return;
                }
                DartRuntimePrimitives.Assert(() => _debugCheckHasValidScrollPosition());
            }
        }
    }

    internal virtual bool _debugCheckHasValidScrollPosition()
    {
        if (!mounted)
        {
            return true;
        }
        ScrollController? scrollController = _effectiveScrollController;
        var tryPrimary = widget.controller is null;
        var controllerForError = tryPrimary
            ? "PrimaryScrollController"
            : "provided ScrollController";
        var @when = "";
        if (widget.thumbVisibility ?? false)
        {
            @when = "Scrollbar.thumbVisibility is true";
        }
        else
        {
            if (enableGestures)
            {
                @when = "the scrollbar is interactive";
            }
            else
            {
                @when = "using the Scrollbar";
            }
        }
        DartRuntimePrimitives.Assert(
            () => scrollController is not null,
            () =>
                (object?)$"A ScrollController is required when {@when}. "
                + $"{(tryPrimary ? "The Scrollbar was not provided a ScrollController, " + "and attempted to use the PrimaryScrollController, but none was found." : "")}"
        );
        DartRuntimePrimitives.Assert(() =>
        {
            if (!scrollController!.hasClients)
            {
                throw DartRuntimePrimitives.AsException(
                    new FlutterError(
                        new List<DiagnosticsNode>
                        {
                            new ErrorSummary(
                                "The Scrollbar's ScrollController has no ScrollPosition attached."
                            ),
                            new ErrorDescription(
                                "A Scrollbar cannot be painted without a ScrollPosition. "
                            ),
                            new ErrorHint(
                                $"The Scrollbar attempted to use the {controllerForError}. This "
                                    + "ScrollController should be associated with the ScrollView that "
                                    + "the Scrollbar is being applied to."
                            ),
                        }
                    )
                );
            }
            return true;
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        DartRuntimePrimitives.Assert(() =>
        {
            try
            {
                DartRuntimePrimitives.Ignore(scrollController!.position);
            }
            catch (Exception)
            {
                if ((scrollController is null) || (scrollController.positions.Count() <= 1L))
                {
                    throw;
                }
                throw DartRuntimePrimitives.AsException(
                    new FlutterError(
                        new List<DiagnosticsNode>
                        {
                            new ErrorSummary(
                                $"The {controllerForError} is attached to more than one ScrollPosition."
                            ),
                            new ErrorDescription(
                                "The Scrollbar requires a single ScrollPosition in order to be painted."
                            ),
                            new ErrorHint(
                                $"When {@when}, the associated ScrollController must only have one "
                                    + "ScrollPosition attached."
                            ),
                        }
                    )
                );
            }
            return true;
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void updateScrollbarPainter()
    {
        TextDirection textDirectionLocal = Directionality.of(context);
        DartRuntimePrimitives.Ignore(
            (
                (Func<ScrollbarPainter>)(
                    () =>
                    {
                        var __cascade = scrollbarPainter;
                        __cascade.color = widget.thumbColor ?? new Color(1723645116L);
                        __cascade.trackRadius = widget.trackRadius;
                        __cascade.trackColor = _showTrack
                            ? (widget.trackColor ?? new Color(134217728L))
                            : new Color(0L);
                        __cascade.trackBorderColor = _showTrack
                            ? (widget.trackBorderColor ?? new Color(436207616L))
                            : new Color(0L);
                        __cascade.textDirection = textDirectionLocal;
                        __cascade.thickness =
                            widget.thickness ?? ScrollbarLibrary._kScrollbarThickness;
                        __cascade.radius = widget.radius;
                        __cascade.padding = (
                            widget.padding ?? MediaQuery.paddingOf(context)
                        ).resolve(textDirectionLocal);
                        __cascade.scrollbarOrientation = widget.scrollbarOrientation;
                        __cascade.mainAxisMargin = widget.mainAxisMargin;
                        __cascade.shape = widget.shape;
                        __cascade.crossAxisMargin = widget.crossAxisMargin;
                        __cascade.minLength = widget.minThumbLength;
                        __cascade.minOverscrollLength =
                            widget.minOverscrollLength ?? widget.minThumbLength;
                        __cascade.ignorePointer = !enableGestures;
                        return __cascade;
                    }
                )
            )()
        );
    }

    public override void didUpdateWidget(T oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (widget.thumbVisibility != oldWidget.thumbVisibility)
        {
            if (widget.thumbVisibility ?? false)
            {
                DartRuntimePrimitives.Assert(() => _debugScheduleCheckHasValidScrollPosition());
                _fadeoutTimer?.cancel();
                _fadeoutAnimationController.animateTo(1.0);
            }
            else
            {
                _fadeoutAnimationController.reverse();
            }
        }
    }

    internal virtual void _maybeStartFadeoutTimer()
    {
        if (!showScrollbar)
        {
            _fadeoutTimer?.cancel();
            _fadeoutTimer = new Timer(
                widget.timeToFade,
                () =>
                {
                    if (!_isDisposed)
                    {
                        try
                        {
                            _fadeoutAnimationController.reverse();
                        }
                        catch (ObjectDisposedException)
                        {
                            // The host dispatcher can finish before a delayed scrollbar fade.
                        }
                    }
                    _fadeoutTimer = null;
                }
            );
        }
    }

    public virtual Axis? getScrollbarDirection() => _axis;

    internal virtual void _disposeThumbDrag()
    {
        _thumbDrag = null;
    }

    internal virtual void _disposeThumbHold()
    {
        _thumbHold = null;
    }

    internal virtual double? _getPrimaryDelta(Offset localPosition)
    {
        DartRuntimePrimitives.Assert(() => _cachedController is not null);
        DartRuntimePrimitives.Assert(() => _startDragScrollbarAxisOffset is not null);
        DartRuntimePrimitives.Assert(() => _lastDragUpdateOffset is not null);
        DartRuntimePrimitives.Assert(() => _startDragThumbOffset is not null);
        ScrollPosition positionLocal = _cachedController!.position;
        double primaryDeltaFromDragStart = default!;
        double primaryDeltaFromLastDragUpdate = default!;
        switch (positionLocal.axisDirection)
        {
            case AxisDirection.up:
            {
                primaryDeltaFromDragStart =
                    (
                        _startDragScrollbarAxisOffset
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ).dy - localPosition.dy;
                primaryDeltaFromLastDragUpdate =
                    (
                        _lastDragUpdateOffset
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ).dy - localPosition.dy;
                break;
            }
            case AxisDirection.right:
            {
                primaryDeltaFromDragStart =
                    localPosition.dx
                    - (
                        _startDragScrollbarAxisOffset
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ).dx;
                primaryDeltaFromLastDragUpdate =
                    localPosition.dx
                    - (
                        _lastDragUpdateOffset
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ).dx;
                break;
            }
            case AxisDirection.down:
            {
                primaryDeltaFromDragStart =
                    localPosition.dy
                    - (
                        _startDragScrollbarAxisOffset
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ).dy;
                primaryDeltaFromLastDragUpdate =
                    localPosition.dy
                    - (
                        _lastDragUpdateOffset
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ).dy;
                break;
            }
            case AxisDirection.left:
            {
                primaryDeltaFromDragStart =
                    (
                        _startDragScrollbarAxisOffset
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ).dx - localPosition.dx;
                primaryDeltaFromLastDragUpdate =
                    (
                        _lastDragUpdateOffset
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ).dx - localPosition.dx;
                break;
            }
        }
        double scrollOffsetGlobal = scrollbarPainter.getTrackToScroll(
            (
                _startDragThumbOffset
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ) + primaryDeltaFromDragStart
        );
        if (
            ((primaryDeltaFromDragStart > 0L) && (scrollOffsetGlobal < positionLocal.pixels))
            || ((primaryDeltaFromDragStart < 0L) && (scrollOffsetGlobal > positionLocal.pixels))
        )
        {
            scrollOffsetGlobal =
                positionLocal.pixels
                + scrollbarPainter.getTrackToScroll(primaryDeltaFromLastDragUpdate);
        }
        if (scrollOffsetGlobal != positionLocal.pixels)
        {
            double physicsAdjustment = positionLocal.physics.applyBoundaryConditions(
                positionLocal,
                scrollOffsetGlobal
            );
            double newPosition = scrollOffsetGlobal - physicsAdjustment;
            switch (ScrollConfiguration.of(context).getPlatform(context))
            {
                case TargetPlatform.fuchsia:
                case TargetPlatform.linux:
                case TargetPlatform.macOS:
                case TargetPlatform.windows:
                {
                    newPosition = DorotiUiLibrary.clampDouble(
                        newPosition,
                        positionLocal.minScrollExtent,
                        positionLocal.maxScrollExtent
                    );
                    break;
                }
                case TargetPlatform.iOS:
                case TargetPlatform.android:
                    break;
            }
            bool isReversed = Basic_typesLibrary.axisDirectionIsReversed(
                positionLocal.axisDirection
            );
            return isReversed
                ? (newPosition - positionLocal.pixels)
                : (positionLocal.pixels - newPosition);
        }
        return null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void handleThumbPress()
    {
        DartRuntimePrimitives.Assert(() => _debugCheckHasValidScrollPosition());
        _cachedController = _effectiveScrollController;
        if (getScrollbarDirection() is null)
        {
            return;
        }
        _fadeoutTimer?.cancel();
        _thumbHold = _cachedController!.position.hold(() => _disposeThumbHold());
    }

    public virtual void handleThumbPressStart(Offset localPosition)
    {
        DartRuntimePrimitives.Assert(() => _debugCheckHasValidScrollPosition());
        Axis? direction = getScrollbarDirection();
        if (direction is null)
        {
            return;
        }
        _fadeoutTimer?.cancel();
        _fadeoutAnimationController.forward();
        DartRuntimePrimitives.Assert(() => _thumbDrag is null);
        ScrollPosition positionLocal = _cachedController!.position;
        var renderBox = ((RenderBox?)_scrollbarPainterKey.currentContext!.findRenderObject()!)!;
        var details = new DragStartDetails(
            localPosition: localPosition,
            globalPosition: renderBox.localToGlobal(localPosition)
        );
        _thumbDrag = positionLocal.drag(details, () => _disposeThumbDrag());
        DartRuntimePrimitives.Assert(() => _thumbDrag is not null);
        DartRuntimePrimitives.Assert(() => _thumbHold is null);
        _startDragScrollbarAxisOffset = localPosition;
        _lastDragUpdateOffset = localPosition;
        _startDragThumbOffset = scrollbarPainter.getThumbScrollOffset();
    }

    public virtual void handleThumbPressUpdate(Offset localPosition)
    {
        DartRuntimePrimitives.Assert(() => _debugCheckHasValidScrollPosition());
        if (Equals(_lastDragUpdateOffset, localPosition))
        {
            return;
        }
        ScrollPosition positionLocal = _cachedController!.position;
        if (!positionLocal.physics.shouldAcceptUserOffset(positionLocal))
        {
            return;
        }
        Axis? direction = getScrollbarDirection();
        if (direction is null)
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => (_thumbHold is null) || (_thumbDrag is null));
        if (_thumbDrag is null)
        {
            return;
        }
        double? primaryDeltaLocal = _getPrimaryDelta(localPosition);
        if (primaryDeltaLocal is null)
        {
            return;
        }
        Offset deltaLocal = (
            direction
            ?? throw new global::System.NullReferenceException("A required value was null.")
        ) switch
        {
            Axis.horizontal => new Offset(
                (
                    primaryDeltaLocal
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                0
            ),
            Axis.vertical => new Offset(
                0,
                (
                    primaryDeltaLocal
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
            ),
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        var renderBox = ((RenderBox?)_scrollbarPainterKey.currentContext!.findRenderObject()!)!;
        var scrollDetails = new DragUpdateDetails(
            delta: deltaLocal,
            primaryDelta: (
                primaryDeltaLocal
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ),
            globalPosition: renderBox.localToGlobal(localPosition),
            localPosition: localPosition
        );
        _thumbDrag!.update(scrollDetails);
        _lastDragUpdateOffset = localPosition;
    }

    public virtual void handleThumbPressEnd(Offset localPosition, Velocity velocity)
    {
        DartRuntimePrimitives.Assert(() => _debugCheckHasValidScrollPosition());
        Axis? direction = getScrollbarDirection();
        if (direction is null)
        {
            return;
        }
        _maybeStartFadeoutTimer();
        _cachedController = null;
        _lastDragUpdateOffset = null;
        DartRuntimePrimitives.Assert(() => (_thumbHold is null) || (_thumbDrag is null));
        if (_thumbDrag is null)
        {
            return;
        }
        TargetPlatform platform = ScrollConfiguration.of(context).getPlatform(context);
        Velocity adjustedVelocity = platform switch
        {
            TargetPlatform.iOS => -velocity,
            TargetPlatform.android => -velocity,
            _ => Velocity.zero,
        };
        var renderBox = ((RenderBox?)_scrollbarPainterKey.currentContext!.findRenderObject()!)!;
        var details = new DragEndDetails(
            localPosition: localPosition,
            globalPosition: renderBox.localToGlobal(localPosition),
            velocity: adjustedVelocity,
            primaryVelocity: (
                direction
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ) switch
            {
                Axis.horizontal => adjustedVelocity.pixelsPerSecond.dx,
                Axis.vertical => adjustedVelocity.pixelsPerSecond.dy,
                _ => throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
            }
        );
        _thumbDrag?.end(details);
        DartRuntimePrimitives.Assert(() => _thumbDrag is null);
        _startDragScrollbarAxisOffset = null;
        _lastDragUpdateOffset = null;
        _startDragThumbOffset = null;
        _cachedController = null;
    }

    public virtual void handleTrackTapDown(TapDownDetails details)
    {
        DartRuntimePrimitives.Assert(() => _debugCheckHasValidScrollPosition());
        _cachedController = _effectiveScrollController;
        ScrollPosition positionLocal = _cachedController!.position;
        if (!positionLocal.physics.shouldAcceptUserOffset(positionLocal))
        {
            return;
        }
        AxisDirection scrollDirection = default!;
        switch (Basic_typesLibrary.axisDirectionToAxis(positionLocal.axisDirection))
        {
            case Axis.vertical:
            {
                if (details.localPosition.dy > scrollbarPainter._thumbOffset)
                {
                    scrollDirection = AxisDirection.down;
                }
                else
                {
                    scrollDirection = AxisDirection.up;
                }
                break;
            }
            case Axis.horizontal:
            {
                if (details.localPosition.dx > scrollbarPainter._thumbOffset)
                {
                    scrollDirection = AxisDirection.right;
                }
                else
                {
                    scrollDirection = AxisDirection.left;
                }
                break;
            }
        }
        ScrollableState? state = Scrollable.maybeOf(positionLocal.context.notificationContext!);
        var intent = new ScrollIntent(direction: scrollDirection, type: ScrollIncrementType.page);
        DartRuntimePrimitives.Assert(() => state is not null);
        double scrollIncrement = ScrollAction.getDirectionalIncrement(
            (
                state
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ),
            intent
        );
        DartRuntimePrimitives.Ignore(
            _cachedController!.position.moveTo(
                _cachedController!.position.pixels + scrollIncrement,
                duration: Duration.Create(milliseconds: 100L),
                curve: Curves.easeInOut
            )
        );
    }

    internal virtual bool _shouldUpdatePainter(Axis notificationAxis)
    {
        ScrollController? scrollController = _effectiveScrollController;
        if (scrollController is null)
        {
            return true;
        }
        if (scrollController.positions.Count() > 1L)
        {
            return false;
        }
        return !scrollController.hasClients
            || Equals(scrollController.position.axis, notificationAxis);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual bool _handleScrollMetricsNotification(ScrollMetricsNotification notification)
    {
        if (!widget.notificationPredicate(notification.asScrollUpdate()))
        {
            return false;
        }
        if (showScrollbar && !_fadeoutAnimationController.isForwardOrCompleted)
        {
            _fadeoutAnimationController.forward();
        }
        ScrollMetrics metricsLocal = notification.metrics;
        if (_shouldUpdatePainter(metricsLocal.axis))
        {
            scrollbarPainter.update(metricsLocal, metricsLocal.axisDirection);
        }
        if (!Equals(metricsLocal.axis, _axis))
        {
            setState(() =>
            {
                _axis = metricsLocal.axis;
            });
        }
        if (_maxScrollExtentPermitsScrolling != (notification.metrics.maxScrollExtent > 0.0))
        {
            setState(() =>
            {
                _maxScrollExtentPermitsScrolling = !_maxScrollExtentPermitsScrolling;
            });
        }
        return false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual bool _handleScrollNotification(ScrollNotification notification)
    {
        if (!widget.notificationPredicate(notification))
        {
            return false;
        }
        ScrollMetrics metricsLocal = notification.metrics;
        if (metricsLocal.maxScrollExtent <= metricsLocal.minScrollExtent)
        {
            if (_fadeoutAnimationController.isForwardOrCompleted)
            {
                _fadeoutAnimationController.reverse();
            }
            if (_shouldUpdatePainter(metricsLocal.axis))
            {
                scrollbarPainter.update(metricsLocal, metricsLocal.axisDirection);
            }
            return false;
        }
        if ((notification is ScrollUpdateNotification) || (notification is OverscrollNotification))
        {
            if (!_fadeoutAnimationController.isForwardOrCompleted)
            {
                _fadeoutAnimationController.forward();
            }
            _fadeoutTimer?.cancel();
            if (_shouldUpdatePainter(metricsLocal.axis))
            {
                scrollbarPainter.update(metricsLocal, metricsLocal.axisDirection);
            }
        }
        else
        {
            if (notification is ScrollEndNotification)
            {
                ScrollEndNotification notification__as73302 = (ScrollEndNotification)notification;
                if (_thumbDrag is null)
                {
                    _maybeStartFadeoutTimer();
                }
            }
        }
        return false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _handleThumbDragDown(DragDownDetails details)
    {
        handleThumbPress();
    }

    internal virtual Offset _globalToScrollbar(Offset offset)
    {
        var renderBox = ((RenderBox?)_scrollbarPainterKey.currentContext!.findRenderObject()!)!;
        return renderBox.globalToLocal(offset);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _handleThumbDragStart(DragStartDetails details)
    {
        handleThumbPressStart(_globalToScrollbar(details.globalPosition));
    }

    internal virtual void _handleThumbDragUpdate(DragUpdateDetails details)
    {
        handleThumbPressUpdate(_globalToScrollbar(details.globalPosition));
    }

    internal virtual void _handleThumbDragEnd(DragEndDetails details)
    {
        handleThumbPressEnd(_globalToScrollbar(details.globalPosition), details.velocity);
    }

    internal virtual void _handleThumbDragCancel()
    {
        if (_gestureDetectorKey.currentContext is null)
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => (_thumbHold is null) || (_thumbDrag is null));
        _thumbHold?.cancel();
        _thumbDrag?.cancel();
        DartRuntimePrimitives.Assert(() => _thumbHold is null);
        DartRuntimePrimitives.Assert(() => _thumbDrag is null);
    }

    internal virtual void _initThumbDragGestureRecognizer(DragGestureRecognizer instance)
    {
        instance.onDown = _handleThumbDragDown;
        instance.onStart = _handleThumbDragStart;
        instance.onUpdate = _handleThumbDragUpdate;
        instance.onEnd = _handleThumbDragEnd;
        instance.onCancel = _handleThumbDragCancel;
        instance.gestureSettings = new DeviceGestureSettings(touchSlop: 0);
        instance.dragStartBehavior = DragStartBehavior.down;
    }

    internal virtual bool _canHandleScrollGestures()
    {
        return enableGestures
            && (_effectiveScrollController is not null)
            && (_effectiveScrollController!.positions.Count() == 1L)
            && _effectiveScrollController!.position.hasContentDimensions
            && (
                (
                    _effectiveScrollController!.position.maxScrollExtent
                    - _effectiveScrollController!.position.minScrollExtent
                ) > Foundation.ConstantsLibrary.precisionErrorTolerance
            );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual DartMap<Type, dynamic> _gestures
    {
        get
        {
            var gestures = new DartMap<Type, dynamic>();
            if (!_canHandleScrollGestures())
            {
                return gestures;
            }
            switch (_effectiveScrollController!.position.axis)
            {
                case Axis.horizontal:
                {
                    gestures[typeof(_HorizontalThumbDragGestureRecognizer__scrollbar)] =
                        new GestureRecognizerFactoryWithHandlers<_HorizontalThumbDragGestureRecognizer__scrollbar>(
                            () =>
                                new _HorizontalThumbDragGestureRecognizer__scrollbar(
                                    debugOwner: this,
                                    customPaintKey: _scrollbarPainterKey
                                ),
                            (__arg0) =>
                                ((Action<DragGestureRecognizer>)_initThumbDragGestureRecognizer)(
                                    DartRuntimePrimitives.ConvertValue<DragGestureRecognizer>(
                                        __arg0
                                    )
                                )
                        );
                    break;
                }
                case Axis.vertical:
                {
                    gestures[typeof(_VerticalThumbDragGestureRecognizer__scrollbar)] =
                        new GestureRecognizerFactoryWithHandlers<_VerticalThumbDragGestureRecognizer__scrollbar>(
                            () =>
                                new _VerticalThumbDragGestureRecognizer__scrollbar(
                                    debugOwner: this,
                                    customPaintKey: _scrollbarPainterKey
                                ),
                            (__arg0) =>
                                ((Action<DragGestureRecognizer>)_initThumbDragGestureRecognizer)(
                                    DartRuntimePrimitives.ConvertValue<DragGestureRecognizer>(
                                        __arg0
                                    )
                                )
                        );
                    break;
                }
            }
            gestures[typeof(_TrackTapGestureRecognizer__scrollbar)] =
                new GestureRecognizerFactoryWithHandlers<_TrackTapGestureRecognizer__scrollbar>(
                    () =>
                        new _TrackTapGestureRecognizer__scrollbar(
                            debugOwner: this,
                            customPaintKey: _scrollbarPainterKey
                        ),
                    (instance) =>
                    {
                        instance.onTapDown = handleTrackTapDown;
                    }
                );
            return gestures;
        }
    }

    public virtual bool isPointerOverTrack(Offset position, PointerDeviceKind kind)
    {
        if (_scrollbarPainterKey.currentContext is null)
        {
            return false;
        }
        Offset localOffset = ScrollbarLibrary._getLocalOffset(_scrollbarPainterKey, position);
        return scrollbarPainter.hitTestInteractive(localOffset, kind)
            && !scrollbarPainter.hitTestOnlyThumbInteractive(localOffset, kind);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool isPointerOverThumb(Offset position, PointerDeviceKind kind)
    {
        if (_scrollbarPainterKey.currentContext is null)
        {
            return false;
        }
        Offset localOffset = ScrollbarLibrary._getLocalOffset(_scrollbarPainterKey, position);
        return scrollbarPainter.hitTestOnlyThumbInteractive(localOffset, kind);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool isPointerOverScrollbar(
        Offset position,
        PointerDeviceKind kind,
        bool forHover = false
    )
    {
        if (_scrollbarPainterKey.currentContext is null)
        {
            return false;
        }
        Offset localOffset = ScrollbarLibrary._getLocalOffset(_scrollbarPainterKey, position);
        return scrollbarPainter.hitTestInteractive(localOffset, kind, forHover: true);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void handleHover(Gestures.PointerHoverEvent @event)
    {
        if (isPointerOverScrollbar(@event.position, @event.kind, forHover: true))
        {
            _hoverIsActive = true;
            _fadeoutAnimationController.forward();
            _fadeoutTimer?.cancel();
        }
        else
        {
            if (_hoverIsActive)
            {
                _hoverIsActive = false;
                _maybeStartFadeoutTimer();
            }
        }
    }

    public virtual void handleHoverExit(Gestures.PointerExitEvent @event)
    {
        _hoverIsActive = false;
        _maybeStartFadeoutTimer();
    }

    internal virtual double _pointerSignalEventDelta(PointerScrollEvent @event)
    {
        DartRuntimePrimitives.Assert(() => _cachedController is not null);
        double delta = Equals(_cachedController!.position.axis, Axis.horizontal)
            ? @event.scrollDelta.dx
            : @event.scrollDelta.dy;
        if (Basic_typesLibrary.axisDirectionIsReversed(_cachedController!.position.axisDirection))
        {
            delta *= -1L;
        }
        return delta;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual double _targetScrollOffsetForPointerScroll(double delta)
    {
        DartRuntimePrimitives.Assert(() => _cachedController is not null);
        return Math.Min(
            Math.Max(
                _cachedController!.position.pixels + delta,
                _cachedController!.position.minScrollExtent
            ),
            _cachedController!.position.maxScrollExtent
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _handlePointerScroll(PointerEvent @event)
    {
        DartRuntimePrimitives.Assert(() => @event is PointerScrollEvent);
        _cachedController = _effectiveScrollController;
        double delta = _pointerSignalEventDelta(((PointerScrollEvent?)@event)!);
        double targetScrollOffset = _targetScrollOffsetForPointerScroll(delta);
        if ((delta != 0.0) && (targetScrollOffset != _cachedController!.position.pixels))
        {
            _cachedController!.position.pointerScroll(delta);
        }
    }

    internal virtual void _receivedPointerSignal(PointerSignalEvent @event)
    {
        _cachedController = _effectiveScrollController;
        if (
            (scrollbarPainter.hitTest(@event.localPosition) ?? false)
            && (_cachedController is not null)
            && _cachedController!.hasClients
            && ((_thumbDrag is null) || Foundation.ConstantsLibrary.kIsWeb)
        )
        {
            ScrollPosition positionLocal = _cachedController!.position;
            if (@event is PointerScrollEvent)
            {
                PointerScrollEvent @event__as82127 = (PointerScrollEvent)@event;
                if (!positionLocal.physics.shouldAcceptUserOffset(positionLocal))
                {
                    return;
                }
                double delta = _pointerSignalEventDelta(@event__as82127);
                double targetScrollOffset = _targetScrollOffsetForPointerScroll(delta);
                if ((delta != 0.0) && (targetScrollOffset != positionLocal.pixels))
                {
                    GestureBinding.instance.pointerSignalResolver.register(
                        @event__as82127,
                        (__arg0) =>
                            ((Action<PointerEvent>)_handlePointerScroll)(
                                DartRuntimePrimitives.ConvertValue<PointerEvent>(__arg0)
                            )
                    );
                }
            }
            else
            {
                if (@event is PointerScrollInertiaCancelEvent)
                {
                    PointerScrollInertiaCancelEvent @event__as82591 =
                        (PointerScrollInertiaCancelEvent)@event;
                    positionLocal.jumpTo(positionLocal.pixels);
                }
            }
        }
    }

    public override void dispose()
    {
        _isDisposed = true;
        _fadeoutTimer?.cancel();
        _fadeoutTimer = null;
        _fadeoutAnimationController.dispose();
        scrollbarPainter.dispose();
        _fadeoutOpacityAnimation.dispose();
        DartRuntimePrimitives.Assert(() =>
        {
            if (_tickers is not null)
            {
                foreach (Scheduler.Ticker ticker in _tickers!)
                {
                    if (ticker.isActive)
                    {
                        throw DartRuntimePrimitives.AsException(
                            new FlutterError(
                                new List<DiagnosticsNode>
                                {
                                    new ErrorSummary($"{this} was disposed with an active Ticker."),
                                    new ErrorDescription(
                                        $"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time "
                                            + "dispose() was called on the mixin, that Ticker was still active. All Tickers must "
                                            + "be disposed before calling super.dispose()."
                                    ),
                                    new ErrorHint(
                                        "Tickers used by AnimationControllers "
                                            + "should be disposed by calling dispose() on the AnimationController itself. "
                                            + "Otherwise, the ticker will leak."
                                    ),
                                    ticker.describeForError("The offending ticker was"),
                                }
                            )
                        );
                    }
                }
            }
            return true;
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        updateScrollbarPainter();
        return new NotificationListener<ScrollMetricsNotification>(
            onNotification: _handleScrollMetricsNotification,
            child: new NotificationListener<ScrollNotification>(
                onNotification: _handleScrollNotification,
                child: new RepaintBoundary(
                    child: new Listener(
                        onPointerSignal: _receivedPointerSignal,
                        child: new RawGestureDetector(
                            key: _gestureDetectorKey,
                            gestures: _gestures,
                            child: new MouseRegion(
                                onExit: (@event) =>
                                {
                                    switch (@event.kind)
                                    {
                                        case PointerDeviceKind.mouse:
                                        case PointerDeviceKind.trackpad:
                                        {
                                            if (enableGestures)
                                            {
                                                handleHoverExit(@event);
                                            }
                                            break;
                                        }
                                        case PointerDeviceKind.stylus:
                                        case PointerDeviceKind.invertedStylus:
                                        case PointerDeviceKind.unknown:
                                        case PointerDeviceKind.touch:
                                        {
                                            break;
                                        }
                                    }
                                },
                                onHover: (@event) =>
                                {
                                    switch (@event.kind)
                                    {
                                        case PointerDeviceKind.mouse:
                                        case PointerDeviceKind.trackpad:
                                        {
                                            if (enableGestures)
                                            {
                                                handleHover(@event);
                                            }
                                            break;
                                        }
                                        case PointerDeviceKind.stylus:
                                        case PointerDeviceKind.invertedStylus:
                                        case PointerDeviceKind.unknown:
                                        case PointerDeviceKind.touch:
                                        {
                                            break;
                                        }
                                    }
                                },
                                child: new CustomPaint(
                                    key: _scrollbarPainterKey,
                                    foregroundPainter: new _ScrollbarCustomPainterAdapter(
                                        scrollbarPainter
                                    ),
                                    child: new RepaintBoundary(child: widget.child)
                                )
                            )
                        )
                    )
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Scheduler.Ticker createTicker(Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = (
            (Func<_WidgetTicker__ticker_provider>)(
                () =>
                {
                    var __cascade = new _WidgetTicker__ticker_provider(
                        onTick,
                        this,
                        debugLabel: Foundation.ConstantsLibrary.kDebugMode
                            ? $"created by {DiagnosticsLibrary.describeIdentity(this)}"
                            : null
                    );
                    __cascade.muted = !values.enabled;
                    __cascade.forceFrames = values.forceFrames;
                    return __cascade;
                }
            )
        )();
        _tickers!.Add(result);
        return result;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void _removeTicker(_WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => _tickers is not null);
        DartRuntimePrimitives.Assert(() => _tickers!.Contains(ticker));
        _tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if (_tickers is not null)
        {
            TickerModeData values = _tickerModeNotifier!.value;
            bool mutedLocal = !values.enabled;
            foreach (Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new DiagnosticsProperty<HashSet<Scheduler.Ticker>>(
                "tickers",
                _tickers,
                description: (_tickers is not null)
                    ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}"
                    : null,
                defaultValue: default
            )
        );
    }
}

public static partial class ScrollbarLibrary
{
    internal static Offset _getLocalOffset(GlobalKey<IState> scrollbarPainterKey, Offset position)
    {
        var renderBox = ((RenderBox?)scrollbarPainterKey.currentContext!.findRenderObject()!)!;
        return renderBox.globalToLocal(position);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public static partial class ScrollbarLibrary
{
    internal static bool _isThumbEvent(GlobalKey<IState> customPaintKey, PointerEvent @event)
    {
        if (customPaintKey.currentContext is null)
        {
            return false;
        }
        var customPaint = ((CustomPaint?)customPaintKey.currentContext!.widget)!;
        var painter = _ScrollbarCustomPainterAdapter.Unwrap(customPaint.foregroundPainter!);
        Offset localOffset = _getLocalOffset(customPaintKey, @event.position);
        return painter.hitTestOnlyThumbInteractive(localOffset, @event.kind);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public static partial class ScrollbarLibrary
{
    internal static bool _isTrackEvent(GlobalKey<IState> customPaintKey, PointerEvent @event)
    {
        if (customPaintKey.currentContext is null)
        {
            return false;
        }
        var customPaint = ((CustomPaint?)customPaintKey.currentContext!.widget)!;
        var painter = _ScrollbarCustomPainterAdapter.Unwrap(customPaint.foregroundPainter!);
        Offset localOffset = _getLocalOffset(customPaintKey, @event.position);
        PointerDeviceKind kindLocal = @event.kind;
        return painter.hitTestInteractive(localOffset, kindLocal)
            && !painter.hitTestOnlyThumbInteractive(localOffset, kindLocal);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _TrackTapGestureRecognizer__scrollbar : TapGestureRecognizer
{
    internal virtual GlobalKey<IState> _customPaintKey { get; private set; } = default!;

    internal _TrackTapGestureRecognizer__scrollbar(
        object? debugOwner,
        GlobalKey<IState> customPaintKey
    )
        : base(debugOwner: debugOwner)
    {
        _customPaintKey = customPaintKey;
    }

    public override bool isPointerAllowed(Gestures.PointerDownEvent @event)
    {
        return ScrollbarLibrary._isTrackEvent(_customPaintKey, @event)
            && base.isPointerAllowed(@event);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _VerticalThumbDragGestureRecognizer__scrollbar : VerticalDragGestureRecognizer
{
    internal virtual GlobalKey<IState> _customPaintKey { get; private set; } = default!;

    internal _VerticalThumbDragGestureRecognizer__scrollbar(
        object debugOwner,
        GlobalKey<IState> customPaintKey
    )
        : base(debugOwner: debugOwner)
    {
        _customPaintKey = customPaintKey;
    }

    public override bool isPointerPanZoomAllowed(PointerPanZoomStartEvent @event)
    {
        return false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool isPointerAllowed(Gestures.PointerDownEvent @event)
    {
        return ScrollbarLibrary._isThumbEvent(_customPaintKey, @event)
            && base.isPointerAllowed(@event);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _HorizontalThumbDragGestureRecognizer__scrollbar : HorizontalDragGestureRecognizer
{
    internal virtual GlobalKey<IState> _customPaintKey { get; private set; } = default!;

    internal _HorizontalThumbDragGestureRecognizer__scrollbar(
        object debugOwner,
        GlobalKey<IState> customPaintKey
    )
        : base(debugOwner: debugOwner)
    {
        _customPaintKey = customPaintKey;
    }

    public override bool isPointerPanZoomAllowed(PointerPanZoomStartEvent @event)
    {
        return false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool isPointerAllowed(Gestures.PointerDownEvent @event)
    {
        return ScrollbarLibrary._isThumbEvent(_customPaintKey, @event)
            && base.isPointerAllowed(@event);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal sealed class _ScrollbarCustomPainterAdapter : CustomPainter
{
    private readonly ScrollbarPainter _owner;

    internal _ScrollbarCustomPainterAdapter(ScrollbarPainter owner)
        : base(owner) => _owner = owner;

    internal static ScrollbarPainter Unwrap(CustomPainter painter) =>
        painter is _ScrollbarCustomPainterAdapter adapter
            ? adapter._owner
            : (ScrollbarPainter)(object)painter;

    public override void paint(Canvas canvas, Size size) => _owner.paint(canvas, size);

    public override bool shouldRepaint(CustomPainter oldDelegate) =>
        oldDelegate is not _ScrollbarCustomPainterAdapter other
        || _owner.shouldRepaint(other._owner);

    public override bool? hitTest(Offset position) => _owner.hitTest(position);
}
