// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/scrollbar.dart
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
    bottom
}

public class ScrollbarPainter : global::Doroti.Framework.Foundation.ChangeNotifier
{
    internal virtual Color _color { get; set; } = default!;
    internal virtual Color _trackColor { get; set; } = default!;
    internal virtual Color _trackBorderColor { get; set; } = default!;
    internal virtual Radius? _trackRadius { get; set; } = default;
    internal virtual TextDirection? _textDirection { get; set; } = default;
    internal virtual double _thickness { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> fadeoutOpacityAnimation { get; private set; } = default!;
    internal virtual double _mainAxisMargin { get; set; } = default!;
    internal virtual double _crossAxisMargin { get; set; } = default!;
    internal virtual Radius? _radius { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.OutlinedBorder? _shape { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry _padding { get; set; } = default!;
    internal virtual double _minLength { get; set; } = default!;
    internal virtual double _minOverscrollLength { get; set; } = default!;
    internal virtual ScrollbarOrientation? _scrollbarOrientation { get; set; } = default;
    internal virtual bool _ignorePointer { get; set; } = default!;
    internal virtual Rect? _trackRect { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.EdgeInsets? _resolvedPadding { get; set; } = default;
    internal virtual Rect? _thumbRect { get; set; } = default;
    internal virtual double _thumbOffset { get; set; } = default!;
    internal virtual double _thumbExtent { get; set; } = default!;
    internal virtual ScrollMetrics? _lastMetrics { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.AxisDirection? _lastAxisDirection { get; set; } = default;

    public ScrollbarPainter(Color color, global::Doroti.Framework.Animation.Animation<double> fadeoutOpacityAnimation, Color trackColor = default!, Color trackBorderColor = default!, TextDirection? textDirection = null, double? thickness = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry padding = default!, double mainAxisMargin = 0.0, double crossAxisMargin = 0.0, Radius? radius = null, Radius? trackRadius = null, global::Doroti.Framework.Painting.OutlinedBorder? shape = null, double? minLength = null, double? minOverscrollLength = null, ScrollbarOrientation? scrollbarOrientation = null, bool ignorePointer = false)
    {
        Color __trackColor = trackColor ?? new Color(0x00000000);
        Color __trackBorderColor = trackBorderColor ?? new Color(0x00000000);
        double __thickness = thickness ?? ScrollbarLibrary._kScrollbarThickness;
        global::Doroti.Framework.Painting.EdgeInsetsGeometry __padding = padding ?? global::Doroti.Framework.Painting.EdgeInsets.zero;
        double __minLength = minLength ?? ScrollbarLibrary._kMinThumbExtent;
        this.fadeoutOpacityAnimation = fadeoutOpacityAnimation;
        this._color = color;
        this._textDirection = textDirection;
        this._thickness = __thickness;
        this._radius = radius;
        this._shape = shape;
        this._padding = __padding;
        this._resolvedPadding = __padding.resolve(textDirection);
        this._mainAxisMargin = mainAxisMargin;
        this._crossAxisMargin = crossAxisMargin;
        this._minLength = __minLength;
        this._trackColor = __trackColor;
        this._trackBorderColor = __trackBorderColor;
        this._trackRadius = trackRadius;
        this._scrollbarOrientation = scrollbarOrientation;
        this._minOverscrollLength = minOverscrollLength ?? __minLength;
        this._ignorePointer = ignorePointer;
        this.fadeoutOpacityAnimation.addListener(this.notifyListeners);
        System.Diagnostics.Debug.Assert(((radius is null) || (shape is null)));
        System.Diagnostics.Debug.Assert((__minLength >= 0L));
        System.Diagnostics.Debug.Assert(((minOverscrollLength is null) || (minOverscrollLength <= __minLength)));
        System.Diagnostics.Debug.Assert(((minOverscrollLength is null) || (minOverscrollLength >= 0L)));
        System.Diagnostics.Debug.Assert(((global::Doroti.Framework.Painting.EdgeInsetsGeometry)__padding).isNonNegative);
        System.Diagnostics.Debug.Assert(((__padding is not global::Doroti.Framework.Painting.EdgeInsetsDirectional) || (textDirection is not null)));
    }

    public virtual global::Doroti.Ui.Color color
    {
        get => this._color;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(this.color, __value)))
            {
                return;
            }
            _color = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color trackColor
    {
        get => this._trackColor;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(this.trackColor, __value)))
            {
                return;
            }
            _trackColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color trackBorderColor
    {
        get => this._trackBorderColor;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(this.trackBorderColor, __value)))
            {
                return;
            }
            _trackBorderColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Radius? trackRadius
    {
        get => this._trackRadius;
        set
        {
            var __value = value;
            if ((object.Equals(this.trackRadius, __value)))
            {
                return;
            }
            _trackRadius = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.TextDirection? textDirection
    {
        get => this._textDirection;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value is not null));
            if ((object.Equals(this.textDirection, __value)))
            {
                return;
            }
            _textDirection = __value;
            _resolvedPadding = this._padding.resolve(this._textDirection);
            notifyListeners();
        }
    }
    public virtual double thickness
    {
        get => this._thickness;
        set
        {
            var __value = value;
            if ((this.thickness == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _thickness = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual double mainAxisMargin
    {
        get => this._mainAxisMargin;
        set
        {
            var __value = value;
            if ((this.mainAxisMargin == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _mainAxisMargin = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual double crossAxisMargin
    {
        get => this._crossAxisMargin;
        set
        {
            var __value = value;
            if ((this.crossAxisMargin == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _crossAxisMargin = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Radius? radius
    {
        get => this._radius;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => ((this.shape is null) || (__value is null)));
            if ((object.Equals(this.radius, __value)))
            {
                return;
            }
            _radius = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Painting.OutlinedBorder? shape
    {
        get => this._shape;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => ((this.radius is null) || (__value is null)));
            if ((object.Equals(this.shape, __value)))
            {
                return;
            }
            _shape = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry padding
    {
        get => this._padding;
        set
        {
            var __value = value;
            if ((object.Equals(this.padding, __value)))
            {
                return;
            }
            _padding = __value;
            _resolvedPadding = this._padding.resolve(this._textDirection);
            notifyListeners();
        }
    }
    public virtual double minLength
    {
        get => this._minLength;
        set
        {
            var __value = value;
            if ((this.minLength == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _minLength = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual double minOverscrollLength
    {
        get => this._minOverscrollLength;
        set
        {
            var __value = value;
            if ((this.minOverscrollLength == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _minOverscrollLength = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual ScrollbarOrientation? scrollbarOrientation
    {
        get => this._scrollbarOrientation;
        set
        {
            var __value = value;
            if ((object.Equals(this.scrollbarOrientation, __value)))
            {
                return;
            }
            _scrollbarOrientation = __value;
            notifyListeners();
        }
    }
    public virtual bool ignorePointer
    {
        get => this._ignorePointer;
        set
        {
            var __value = value;
            if ((this.ignorePointer == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _ignorePointer = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    internal virtual double _trackExtent => DartRuntimePrimitives.ConvertValue<double>((this._lastMetrics!.viewportDimension - this._totalTrackMainAxisOffsets));
    internal virtual double _traversableTrackExtent => DartRuntimePrimitives.ConvertValue<double>((this._trackExtent - ((2L * this.mainAxisMargin))));
    internal virtual double _totalTrackMainAxisOffsets => (this._isVertical ? this._resolvedPadding!.vertical : this._resolvedPadding!.horizontal);
    internal virtual double _leadingTrackMainAxisOffset => (this._resolvedOrientation switch { ScrollbarOrientation.left => this._resolvedPadding!.top, ScrollbarOrientation.right => this._resolvedPadding!.top, ScrollbarOrientation.top => this._resolvedPadding!.left, ScrollbarOrientation.bottom => this._resolvedPadding!.left, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    internal virtual double _leadingThumbMainAxisOffset => DartRuntimePrimitives.ConvertValue<double>((this._leadingTrackMainAxisOffset + this.mainAxisMargin));
    internal virtual void _setThumbExtent()
    {
        double fractionVisible = Dart_uiLibrary.clampDouble((((this._lastMetrics!.extentInside - this._totalTrackMainAxisOffsets)) / ((this._totalContentExtent - this._totalTrackMainAxisOffsets))), 0.0, 1.0);
        double thumbExtent = Math.Max(Math.Min(this._traversableTrackExtent, this.minOverscrollLength), (this._traversableTrackExtent * fractionVisible));
        double fractionOverscrolled = (1.0 - (this._lastMetrics!.extentInside / this._lastMetrics!.viewportDimension));
        double safeMinLength = Math.Min(DartRuntimePrimitives.RequireValue(this.minLength), this._traversableTrackExtent);
        double newMinLength = ((((this._beforeExtent > 0L) && (this._afterExtent > 0L))) ? safeMinLength : (safeMinLength * ((1.0 - (Dart_uiLibrary.clampDouble(fractionOverscrolled, 0.0, 0.2) / 0.2)))));
        _thumbExtent = Dart_uiLibrary.clampDouble(thumbExtent, newMinLength, this._traversableTrackExtent);
    }

    internal virtual bool _lastMetricsAreScrollable => DartRuntimePrimitives.ConvertValue<bool>((this._lastMetrics!.minScrollExtent != this._lastMetrics!.maxScrollExtent));
    internal virtual bool _isVertical => DartRuntimePrimitives.ConvertValue<bool>(((object.Equals(this._lastAxisDirection, global::Doroti.Framework.Painting.AxisDirection.down)) || (object.Equals(this._lastAxisDirection, global::Doroti.Framework.Painting.AxisDirection.up))));
    internal virtual bool _isReversed => DartRuntimePrimitives.ConvertValue<bool>(((object.Equals(this._lastAxisDirection, global::Doroti.Framework.Painting.AxisDirection.up)) || (object.Equals(this._lastAxisDirection, global::Doroti.Framework.Painting.AxisDirection.left))));
    internal virtual double _beforeExtent => (this._isReversed ? this._lastMetrics!.extentAfter : this._lastMetrics!.extentBefore);
    internal virtual double _afterExtent => (this._isReversed ? this._lastMetrics!.extentBefore : this._lastMetrics!.extentAfter);
    internal virtual double _totalContentExtent
    {
        get
        {
            return ((this._lastMetrics!.maxScrollExtent - this._lastMetrics!.minScrollExtent) + this._lastMetrics!.viewportDimension);
            return default!;
        }
    }
    internal virtual ScrollbarOrientation _resolvedOrientation
    {
        get
        {
            if ((this.scrollbarOrientation is null))
            {
                if (this._isVertical)
                {
                    return ((object.Equals(this.textDirection, TextDirection.ltr)) ? ScrollbarOrientation.right : ScrollbarOrientation.left);
                }
                return ScrollbarOrientation.bottom;
            }
            return DartRuntimePrimitives.RequireValue(this.scrollbarOrientation);
            return default!;
        }
    }
    internal virtual void _debugAssertIsValidOrientation(ScrollbarOrientation orientation)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                bool isVerticalOrientation(ScrollbarOrientation orientation)
                {
                    return ((object.Equals(orientation, ScrollbarOrientation.left)) || (object.Equals(orientation, ScrollbarOrientation.right)));
                    throw new InvalidOperationException("Dart control flow completed without a value.");
                }
                return (((this._isVertical && isVerticalOrientation(orientation))) || ((!this._isVertical && !isVerticalOrientation(orientation))));
                throw new InvalidOperationException("Dart closure completed without a value.");
            }, () => (object?)$"The given ScrollbarOrientation: {orientation} is incompatible with the " + $"current AxisDirection: {this._lastAxisDirection}.");
    }

    public virtual void update(ScrollMetrics metrics, global::Doroti.Framework.Painting.AxisDirection axisDirection)
    {
        if ((((((this._lastMetrics is not null) && (this._lastMetrics!.extentBefore == ((ScrollMetrics)metrics).extentBefore)) && (this._lastMetrics!.extentInside == ((ScrollMetrics)metrics).extentInside)) && (this._lastMetrics!.extentAfter == ((ScrollMetrics)metrics).extentAfter)) && (object.Equals(this._lastAxisDirection, axisDirection))))
        {
            return;
        }
        ScrollMetrics? oldMetrics = this._lastMetrics;
        _lastMetrics = metrics;
        _lastAxisDirection = axisDirection;
        if ((!_needPaint(oldMetrics) && !_needPaint(metrics)))
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

    internal virtual global::Doroti.Ui.Paint _paintThumb
    {
        get
        {
            return ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = this.color.withOpacity((this.color.opacity * ((global::Doroti.Framework.Animation.Animation<double>)this.fadeoutOpacityAnimation).value));
    return __cascade;
}))();
            return default!;
        }
    }
    internal virtual bool _needPaint(ScrollMetrics? metrics)
    {
        return ((metrics is not null) && ((((ScrollMetrics)metrics).maxScrollExtent - ((ScrollMetrics)metrics).minScrollExtent) > global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Paint _paintTrack(bool isBorder = false)
    {
        if (isBorder)
        {
            return ((global::Doroti.Ui.Paint)(object?)((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = this.trackBorderColor.withOpacity((this.trackBorderColor.opacity * ((global::Doroti.Framework.Animation.Animation<double>)this.fadeoutOpacityAnimation).value));
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = 1.0;
    return __cascade;
}))());
        }
        return ((global::Doroti.Ui.Paint)(object?)((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = this.trackColor.withOpacity((this.trackColor.opacity * ((global::Doroti.Framework.Animation.Animation<double>)this.fadeoutOpacityAnimation).value));
    return __cascade;
}))());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _paintScrollbar(Canvas canvas, Size size)
    {
        DartRuntimePrimitives.Assert(() => (this.textDirection is not null), () => (object?)"A TextDirection must be provided before a Scrollbar can be painted.");
        double x = default!;
        double y = default!;
        global::Doroti.Ui.Size thumbSize = default!;
        global::Doroti.Ui.Size trackSize = default!;
        global::Doroti.Ui.Offset trackOffset = default!;
        global::Doroti.Ui.Offset borderStart = default!;
        global::Doroti.Ui.Offset borderEnd = default!;
        _debugAssertIsValidOrientation(this._resolvedOrientation);
        switch (this._resolvedOrientation)
        {
            case ScrollbarOrientation.left:
                {
                    thumbSize = new global::Doroti.Ui.Size(DartRuntimePrimitives.RequireValue(this.thickness), this._thumbExtent);
                    trackSize = new global::Doroti.Ui.Size((this.thickness + (2L * this.crossAxisMargin)), this._trackExtent);
                    x = (this.crossAxisMargin + this._resolvedPadding!.left);
                    y = this._thumbOffset;
                    trackOffset = new global::Doroti.Ui.Offset((x - this.crossAxisMargin), this._leadingTrackMainAxisOffset);
                    borderStart = (trackOffset + new global::Doroti.Ui.Offset(trackSize.width, 0.0));
                    borderEnd = new global::Doroti.Ui.Offset((trackOffset.dx + trackSize.width), (trackOffset.dy + this._trackExtent));
                    break;
                }
            case ScrollbarOrientation.right:
                {
                    thumbSize = new global::Doroti.Ui.Size(DartRuntimePrimitives.RequireValue(this.thickness), this._thumbExtent);
                    trackSize = new global::Doroti.Ui.Size((this.thickness + (2L * this.crossAxisMargin)), this._trackExtent);
                    x = (((size.width - this.thickness) - this.crossAxisMargin) - this._resolvedPadding!.right);
                    y = this._thumbOffset;
                    trackOffset = new global::Doroti.Ui.Offset((x - this.crossAxisMargin), this._leadingTrackMainAxisOffset);
                    borderStart = trackOffset;
                    borderEnd = new global::Doroti.Ui.Offset(trackOffset.dx, (trackOffset.dy + this._trackExtent));
                    break;
                }
            case ScrollbarOrientation.top:
                {
                    thumbSize = new global::Doroti.Ui.Size(this._thumbExtent, DartRuntimePrimitives.RequireValue(this.thickness));
                    trackSize = new global::Doroti.Ui.Size(this._trackExtent, (this.thickness + (2L * this.crossAxisMargin)));
                    x = this._thumbOffset;
                    y = (this.crossAxisMargin + this._resolvedPadding!.top);
                    trackOffset = new global::Doroti.Ui.Offset(this._leadingTrackMainAxisOffset, (y - this.crossAxisMargin));
                    borderStart = (trackOffset + new global::Doroti.Ui.Offset(0.0, trackSize.height));
                    borderEnd = new global::Doroti.Ui.Offset((trackOffset.dx + this._trackExtent), (trackOffset.dy + trackSize.height));
                    break;
                }
            case ScrollbarOrientation.bottom:
                {
                    thumbSize = new global::Doroti.Ui.Size(this._thumbExtent, DartRuntimePrimitives.RequireValue(this.thickness));
                    trackSize = new global::Doroti.Ui.Size(this._trackExtent, (this.thickness + (2L * this.crossAxisMargin)));
                    x = this._thumbOffset;
                    y = (((size.height - this.thickness) - this.crossAxisMargin) - this._resolvedPadding!.bottom);
                    trackOffset = new global::Doroti.Ui.Offset(this._leadingTrackMainAxisOffset, (y - this.crossAxisMargin));
                    borderStart = trackOffset;
                    borderEnd = new global::Doroti.Ui.Offset((trackOffset.dx + this._trackExtent), trackOffset.dy);
                    break;
                }
        }
        _trackRect = (trackOffset & trackSize);
        _thumbRect = (new global::Doroti.Ui.Offset(x, y) & thumbSize);
        if ((((global::Doroti.Framework.Animation.Animation<double>)this.fadeoutOpacityAnimation).value != 0.0))
        {
            if ((this.trackRadius is null))
            {
                canvas.drawRect(DartRuntimePrimitives.RequireValue(this._trackRect), _paintTrack());
            }
            else
            {
                canvas.drawRRect(global::Doroti.Ui.RRect.fromRectAndRadius(DartRuntimePrimitives.RequireValue(this._trackRect), DartRuntimePrimitives.RequireValue(this.trackRadius)), _paintTrack());
            }
            canvas.drawLine(borderStart, borderEnd, _paintTrack(isBorder: true));
            if ((this.radius is not null))
            {
                Radius radius__value22874 = DartRuntimePrimitives.RequireValue(radius);
                canvas.drawRRect(global::Doroti.Ui.RRect.fromRectAndRadius(DartRuntimePrimitives.RequireValue(this._thumbRect), DartRuntimePrimitives.RequireValue(this.radius)), this._paintThumb);
                return;
            }
            if ((this.shape is null))
            {
                canvas.drawRect(DartRuntimePrimitives.RequireValue(this._thumbRect), this._paintThumb);
                return;
            }
            if (this.shape!.preferPaintInterior)
            {
                this.shape!.paintInterior(canvas, DartRuntimePrimitives.RequireValue(this._thumbRect), this._paintThumb);
            }
            else
            {
                global::Doroti.Ui.Path outerPath = ((global::Doroti.Ui.Path)(object?)this.shape!.getOuterPath(DartRuntimePrimitives.RequireValue(this._thumbRect)));
                canvas.drawPath(outerPath, this._paintThumb);
            }
            this.shape!.paint(canvas, DartRuntimePrimitives.RequireValue(this._thumbRect));
        }
    }

    public virtual void paint(Canvas canvas, Size size)
    {
        if (((this._lastAxisDirection is null) || !_needPaint(this._lastMetrics)))
        {
            return;
        }
        if ((this._traversableTrackExtent <= 0L))
        {
            return;
        }
        if (double.IsInfinity(this._lastMetrics!.maxScrollExtent))
        {
            return;
        }
        _setThumbExtent();
        double thumbPositionOffset = _getScrollToTrack(this._lastMetrics!, this._thumbExtent);
        _thumbOffset = (thumbPositionOffset + this._leadingThumbMainAxisOffset);
        _paintScrollbar(canvas, size);
        return;
    }

    public virtual double getTrackToScroll(double thumbOffsetLocal)
    {
        double scrollableExtent = (this._lastMetrics!.maxScrollExtent - this._lastMetrics!.minScrollExtent);
        double thumbMovableExtent = (this._traversableTrackExtent - this._thumbExtent);
        return ((scrollableExtent * thumbOffsetLocal) / thumbMovableExtent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double getThumbScrollOffset()
    {
        DartRuntimePrimitives.Assert(() => (double.IsFinite(this._lastMetrics!.maxScrollExtent) && double.IsFinite(this._lastMetrics!.minScrollExtent)));
        double scrollableExtent = (this._lastMetrics!.maxScrollExtent - this._lastMetrics!.minScrollExtent);
        double maxFraction = (this._lastMetrics!.maxScrollExtent / scrollableExtent);
        double minFraction = (this._lastMetrics!.minScrollExtent / scrollableExtent);
        double fractionPast = (((scrollableExtent > 0L)) ? Dart_uiLibrary.clampDouble((this._lastMetrics!.pixels / scrollableExtent), minFraction, maxFraction) : 0);
        return (fractionPast * ((this._traversableTrackExtent - this._thumbExtent)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getScrollToTrack(ScrollMetrics metrics, double thumbExtent)
    {
        double scrollableExtent = (((ScrollMetrics)metrics).maxScrollExtent - ((ScrollMetrics)metrics).minScrollExtent);
        double fractionPast = (((scrollableExtent > 0L)) ? Dart_uiLibrary.clampDouble((((((ScrollMetrics)metrics).pixels - ((ScrollMetrics)metrics).minScrollExtent)) / scrollableExtent), 0.0, 1.0) : 0);
        return (((this._isReversed ? (1L - fractionPast) : fractionPast)) * ((this._traversableTrackExtent - thumbExtent)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool? hitTest(Offset? position)
    {
        if ((this._thumbRect is null))
        {
            return null;
        }
        if (((this.ignorePointer || (((global::Doroti.Framework.Animation.Animation<double>)this.fadeoutOpacityAnimation).value == 0.0)) || !this._lastMetricsAreScrollable))
        {
            return false;
        }
        return DartRuntimePrimitives.RequireValue(this._trackRect).contains(DartRuntimePrimitives.RequireValue(position));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool hitTestInteractive(Offset position, PointerDeviceKind kind, bool forHover = false)
    {
        if ((this._trackRect is null))
        {
            return false;
        }
        if (this.ignorePointer)
        {
            return false;
        }
        if (!this._lastMetricsAreScrollable)
        {
            return false;
        }
        global::Doroti.Ui.Rect interactiveRect = ((global::Doroti.Ui.Rect)(object?)DartRuntimePrimitives.RequireValue(this._trackRect));
        global::Doroti.Ui.Rect paddedRect = ((global::Doroti.Ui.Rect)(object?)interactiveRect.expandToInclude(global::Doroti.Ui.Rect.fromCircle(center: ((Offset)((dynamic)DartRuntimePrimitives.RequireValue(this._thumbRect)).center), radius: (ScrollbarLibrary._kMinInteractiveSize / 2L))));
        if ((((global::Doroti.Framework.Animation.Animation<double>)this.fadeoutOpacityAnimation).value == 0.0))
        {
            if ((forHover && (object.Equals(kind, PointerDeviceKind.mouse))))
            {
                return paddedRect.contains(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(position)));
            }
            return false;
        }
        switch (kind)
        {
            case PointerDeviceKind.touch:
            case PointerDeviceKind.trackpad:
                {
                    return paddedRect.contains(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(position)));
                }
            case PointerDeviceKind.mouse:
            case PointerDeviceKind.stylus:
            case PointerDeviceKind.invertedStylus:
            case PointerDeviceKind.unknown:
                {
                    return interactiveRect.contains(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(position)));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool hitTestOnlyThumbInteractive(Offset position, PointerDeviceKind kind)
    {
        if ((this._thumbRect is null))
        {
            return false;
        }
        if (this.ignorePointer)
        {
            return false;
        }
        if ((((global::Doroti.Framework.Animation.Animation<double>)this.fadeoutOpacityAnimation).value == 0.0))
        {
            return false;
        }
        if (!this._lastMetricsAreScrollable)
        {
            return false;
        }
        switch (kind)
        {
            case PointerDeviceKind.touch:
            case PointerDeviceKind.trackpad:
                {
                    global::Doroti.Ui.Rect touchThumbRect = ((global::Doroti.Ui.Rect)(object?)DartRuntimePrimitives.RequireValue(this._thumbRect).expandToInclude(global::Doroti.Ui.Rect.fromCircle(center: ((Offset)((dynamic)DartRuntimePrimitives.RequireValue(this._thumbRect)).center), radius: (ScrollbarLibrary._kMinInteractiveSize / 2L))));
                    return touchThumbRect.contains(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(position)));
                }
            case PointerDeviceKind.mouse:
            case PointerDeviceKind.stylus:
            case PointerDeviceKind.invertedStylus:
            case PointerDeviceKind.unknown:
                {
                    return DartRuntimePrimitives.RequireValue(this._thumbRect).contains(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(position)));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool shouldRepaint(ScrollbarPainter oldDelegate)
    {
        return ((((((((((((((((!object.Equals(this.color, ((ScrollbarPainter)oldDelegate).color)) || (!object.Equals(this.trackColor, ((ScrollbarPainter)oldDelegate).trackColor))) || (!object.Equals(this.trackBorderColor, ((ScrollbarPainter)oldDelegate).trackBorderColor))) || (!object.Equals(this.textDirection, ((ScrollbarPainter)oldDelegate).textDirection))) || (this.thickness != ((ScrollbarPainter)oldDelegate).thickness)) || (!object.Equals(this.fadeoutOpacityAnimation, ((ScrollbarPainter)oldDelegate).fadeoutOpacityAnimation))) || (this.mainAxisMargin != ((ScrollbarPainter)oldDelegate).mainAxisMargin)) || (this.crossAxisMargin != ((ScrollbarPainter)oldDelegate).crossAxisMargin)) || (!object.Equals(this.radius, ((ScrollbarPainter)oldDelegate).radius))) || (!object.Equals(this.trackRadius, ((ScrollbarPainter)oldDelegate).trackRadius))) || (!object.Equals(this.shape, ((ScrollbarPainter)oldDelegate).shape))) || (!object.Equals(this.padding, ((ScrollbarPainter)oldDelegate).padding))) || (this.minLength != ((ScrollbarPainter)oldDelegate).minLength)) || (this.minOverscrollLength != ((ScrollbarPainter)oldDelegate).minOverscrollLength)) || (!object.Equals(this.scrollbarOrientation, ((ScrollbarPainter)oldDelegate).scrollbarOrientation))) || (this.ignorePointer != ((ScrollbarPainter)oldDelegate).ignorePointer));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool shouldRebuildSemantics(global::Doroti.Framework.Rendering.CustomPainter oldDelegate) => false;
    public virtual global::System.Func<Size, List<global::Doroti.Framework.Rendering.CustomPainterSemantics>>? semanticsBuilder => DartRuntimePrimitives.ConvertValue<global::System.Func<Size, List<global::Doroti.Framework.Rendering.CustomPainterSemantics>>>(null);
    public override string ToString() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual void dispose()
    {
        this.fadeoutOpacityAnimation.removeListener(this.notifyListeners);
        base.dispose();
    }

}

public class RawScrollbar : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual ScrollController? controller { get; private set; }
    public virtual bool? thumbVisibility { get; private set; }
    public virtual global::Doroti.Framework.Painting.OutlinedBorder? shape { get; private set; }
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
    public virtual global::System.Func<ScrollNotification, bool> notificationPredicate { get; private set; } = default!;
    public virtual bool? interactive { get; private set; }
    public virtual ScrollbarOrientation? scrollbarOrientation { get; private set; }
    public virtual double mainAxisMargin { get; private set; } = default!;
    public virtual double crossAxisMargin { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }

    public RawScrollbar(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!, ScrollController? controller = null, bool? thumbVisibility = null, global::Doroti.Framework.Painting.OutlinedBorder? shape = null, Radius? radius = null, double? thickness = null, Color? thumbColor = null, double? minThumbLength = null, double? minOverscrollLength = null, bool? trackVisibility = null, Radius? trackRadius = null, Color? trackColor = null, Color? trackBorderColor = null, Duration? fadeDuration = null, Duration? timeToFade = null, Duration pressDuration = default, global::System.Func<ScrollNotification, bool> notificationPredicate = default!, bool? interactive = null, ScrollbarOrientation? scrollbarOrientation = null, double mainAxisMargin = 0.0, double crossAxisMargin = 0.0, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null) : base(key: key)
    {
        double __minThumbLength = minThumbLength ?? ScrollbarLibrary._kMinThumbExtent;
        Duration __fadeDuration = fadeDuration ?? ScrollbarLibrary._kScrollbarFadeDuration;
        Duration __timeToFade = timeToFade ?? ScrollbarLibrary._kScrollbarTimeToFade;
        global::System.Func<ScrollNotification, bool> __notificationPredicate = notificationPredicate ?? Scroll_notificationLibrary.defaultScrollNotificationPredicate;
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
        System.Diagnostics.Debug.Assert(!(((thumbVisibility == false) && ((trackVisibility ?? false)))));
        System.Diagnostics.Debug.Assert((__minThumbLength >= 0L));
        System.Diagnostics.Debug.Assert(((minOverscrollLength is null) || (minOverscrollLength <= __minThumbLength)));
        System.Diagnostics.Debug.Assert(((minOverscrollLength is null) || (minOverscrollLength >= 0L)));
        System.Diagnostics.Debug.Assert(((radius is null) || (shape is null)));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new RawScrollbarState<RawScrollbar>());
}

public class RawScrollbarState<T> : State<T>, TickerProviderStateMixin<T> where T : RawScrollbar
{
    internal virtual Offset? _startDragScrollbarAxisOffset { get; set; } = default;
    internal virtual Offset? _lastDragUpdateOffset { get; set; } = default;
    internal virtual double? _startDragThumbOffset { get; set; } = default;
    internal virtual ScrollController? _cachedController { get; set; } = default;
    internal virtual Timer? _fadeoutTimer { get; set; } = default;
    internal virtual bool _isDisposed { get; set; }
    internal virtual global::Doroti.Framework.Animation.AnimationController _fadeoutAnimationController { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation _fadeoutOpacityAnimation { get; set; } = default!;
    internal virtual GlobalKey<IState> _scrollbarPainterKey { get; private set; } = GlobalKey<IState>.Create();
    internal virtual bool _hoverIsActive { get; set; } = false;
    internal virtual global::Doroti.Framework.Gestures.Drag? _thumbDrag { get; set; } = default;
    internal virtual bool _maxScrollExtentPermitsScrolling { get; set; } = false;
    internal virtual ScrollHoldController? _thumbHold { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.Axis? _axis { get; set; } = default;
    internal virtual GlobalKey<RawGestureDetectorState> _gestureDetectorKey { get; private set; } = GlobalKey<RawGestureDetectorState>.Create();
    public virtual ScrollbarPainter scrollbarPainter { get; private set; } = default!;
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual ScrollController? _effectiveScrollController => DartRuntimePrimitives.ConvertValue<ScrollController>(((((RawScrollbar)(object)this.widget).controller ?? (ScrollController)PrimaryScrollController.maybeOf(this.context))));
    public virtual bool showScrollbar => DartRuntimePrimitives.ConvertValue<bool>((((RawScrollbar)(object)this.widget).thumbVisibility ?? false));
    internal virtual bool _showTrack => DartRuntimePrimitives.ConvertValue<bool>((this.showScrollbar && ((((RawScrollbar)(object)this.widget).trackVisibility ?? false))));
    public virtual bool enableGestures => DartRuntimePrimitives.ConvertValue<bool>((((RawScrollbar)(object)this.widget).interactive ?? true));
    public override void initState()
    {
        base.initState();
        _fadeoutAnimationController = ((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{
    var __cascade = new global::Doroti.Framework.Animation.AnimationController(vsync: this, duration: ((RawScrollbar)(object)this.widget).fadeDuration);
    __cascade.addStatusListener((AnimationStatusListener)this._validateInteractions);
    return __cascade;
}))();
        _fadeoutOpacityAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._fadeoutAnimationController, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
        scrollbarPainter = new ScrollbarPainter(color: (((RawScrollbar)(object)this.widget).thumbColor ?? new global::Doroti.Ui.Color(1723645116L)), fadeoutOpacityAnimation: this._fadeoutOpacityAnimation, thickness: (((RawScrollbar)(object)this.widget).thickness ?? ScrollbarLibrary._kScrollbarThickness), radius: ((RawScrollbar)(object)this.widget).radius, trackRadius: ((RawScrollbar)(object)this.widget).trackRadius, scrollbarOrientation: ((RawScrollbar)(object)this.widget).scrollbarOrientation, mainAxisMargin: ((RawScrollbar)(object)this.widget).mainAxisMargin, shape: ((RawScrollbar)(object)this.widget).shape, crossAxisMargin: ((RawScrollbar)(object)this.widget).crossAxisMargin, minLength: ((RawScrollbar)(object)this.widget).minThumbLength, minOverscrollLength: (((RawScrollbar)(object)this.widget).minOverscrollLength ?? ((RawScrollbar)(object)this.widget).minThumbLength));
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        DartRuntimePrimitives.Assert(() => _debugScheduleCheckHasValidScrollPosition());
    }

    internal virtual bool _debugScheduleCheckHasValidScrollPosition()
    {
        if (!this.showScrollbar)
        {
            return true;
        }
        WidgetsBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((duration) =>
        {
            DartRuntimePrimitives.Assert(() => _debugCheckHasValidScrollPosition());
        })), debugLabel: "RawScrollbar.checkScrollPosition");
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _validateInteractions(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        if (global::Doroti.Framework.Animation.AnimationStatusMembers.isDismissed(status))
        {
            DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Animation.CurvedAnimation)this._fadeoutOpacityAnimation).value == 0.0));
        }
        else
        {
            if (((this._effectiveScrollController is not null) && this.enableGestures))
            {
                if (((object.Equals(((global::Doroti.Framework.Animation.AnimationController)this._fadeoutAnimationController).status, global::Doroti.Framework.Animation.AnimationStatus.forward)) && ((((RawScrollbar)(object)this.widget).thumbVisibility ?? false))))
                {
                    return;
                }
                DartRuntimePrimitives.Assert(() => _debugCheckHasValidScrollPosition());
            }
        }
    }

    internal virtual bool _debugCheckHasValidScrollPosition()
    {
        if (!this.mounted)
        {
            return true;
        }
        ScrollController? scrollController = this._effectiveScrollController;
        var tryPrimary = (((RawScrollbar)(object)this.widget).controller is null);
        var controllerForError = (tryPrimary ? "PrimaryScrollController" : "provided ScrollController");
        var @when = "";
        if ((((RawScrollbar)(object)this.widget).thumbVisibility ?? false))
        {
            @when = "Scrollbar.thumbVisibility is true";
        }
        else
        {
            if (this.enableGestures)
            {
                @when = "the scrollbar is interactive";
            }
            else
            {
                @when = "using the Scrollbar";
            }
        }
        DartRuntimePrimitives.Assert(() => (scrollController is not null), () => (object?)$"A ScrollController is required when {@when}. " + $"{(tryPrimary ? "The Scrollbar was not provided a ScrollController, " + "and attempted to use the PrimaryScrollController, but none was found." : "")}");
        DartRuntimePrimitives.Assert(() =>
            {
                if (!scrollController!.hasClients)
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("The Scrollbar's ScrollController has no ScrollPosition attached."), new global::Doroti.Framework.Foundation.ErrorDescription("A Scrollbar cannot be painted without a ScrollPosition. "), new global::Doroti.Framework.Foundation.ErrorHint($"The Scrollbar attempted to use the {controllerForError}. This " + "ScrollController should be associated with the ScrollView that " + "the Scrollbar is being applied to.") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() =>
            {
                try
                {
                    DartRuntimePrimitives.Ignore(scrollController!.position);
                }
                catch (Exception error)
                {
                    if (((scrollController is null) || (((ScrollController)scrollController).positions.Count() <= 1L)))
                    {
                        throw;
                    }
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"The {controllerForError} is attached to more than one ScrollPosition."), new global::Doroti.Framework.Foundation.ErrorDescription("The Scrollbar requires a single ScrollPosition in order to be painted."), new global::Doroti.Framework.Foundation.ErrorHint($"When {@when}, the associated ScrollController must only have one " + "ScrollPosition attached.") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void updateScrollbarPainter()
    {
        global::Doroti.Ui.TextDirection textDirectionLocal = Directionality.of(this.context);
        DartRuntimePrimitives.Ignore(((Func<ScrollbarPainter>)(() =>
{
    var __cascade = this.scrollbarPainter;
    __cascade.color = (((RawScrollbar)(object)this.widget).thumbColor ?? new global::Doroti.Ui.Color(1723645116L));
    __cascade.trackRadius = ((RawScrollbar)(object)this.widget).trackRadius;
    __cascade.trackColor = (this._showTrack ? (((RawScrollbar)(object)this.widget).trackColor ?? new global::Doroti.Ui.Color(134217728L)) : new global::Doroti.Ui.Color(0L));
    __cascade.trackBorderColor = (this._showTrack ? (((RawScrollbar)(object)this.widget).trackBorderColor ?? new global::Doroti.Ui.Color(436207616L)) : new global::Doroti.Ui.Color(0L));
    __cascade.textDirection = textDirectionLocal;
    __cascade.thickness = (((RawScrollbar)(object)this.widget).thickness ?? ScrollbarLibrary._kScrollbarThickness);
    __cascade.radius = ((RawScrollbar)(object)this.widget).radius;
    __cascade.padding = (((((RawScrollbar)(object)this.widget).padding ?? (global::Doroti.Framework.Painting.EdgeInsetsGeometry)MediaQuery.paddingOf(this.context)))).resolve(textDirectionLocal);
    __cascade.scrollbarOrientation = ((RawScrollbar)(object)this.widget).scrollbarOrientation;
    __cascade.mainAxisMargin = ((RawScrollbar)(object)this.widget).mainAxisMargin;
    __cascade.shape = ((RawScrollbar)(object)this.widget).shape;
    __cascade.crossAxisMargin = ((RawScrollbar)(object)this.widget).crossAxisMargin;
    __cascade.minLength = ((RawScrollbar)(object)this.widget).minThumbLength;
    __cascade.minOverscrollLength = (((RawScrollbar)(object)this.widget).minOverscrollLength ?? ((RawScrollbar)(object)this.widget).minThumbLength);
    __cascade.ignorePointer = !this.enableGestures;
    return __cascade;
}))());
    }

    public override void didUpdateWidget(T oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((((RawScrollbar)(object)this.widget).thumbVisibility != ((RawScrollbar)(object)oldWidget).thumbVisibility))
        {
            if ((((RawScrollbar)(object)this.widget).thumbVisibility ?? false))
            {
                DartRuntimePrimitives.Assert(() => _debugScheduleCheckHasValidScrollPosition());
                this._fadeoutTimer?.cancel();
                this._fadeoutAnimationController.animateTo(1.0);
            }
            else
            {
                this._fadeoutAnimationController.reverse();
            }
        }
    }

    internal virtual void _maybeStartFadeoutTimer()
    {
        if (!this.showScrollbar)
        {
            this._fadeoutTimer?.cancel();
            _fadeoutTimer = new Timer(((RawScrollbar)(object)this.widget).timeToFade, (() =>
            {
                if (!this._isDisposed)
                {
                    try
                    {
                        this._fadeoutAnimationController.reverse();
                    }
                    catch (ObjectDisposedException)
                    {
                        // The host dispatcher can finish before a delayed scrollbar fade.
                    }
                }
                _fadeoutTimer = null;
            }));
        }
    }

    public virtual global::Doroti.Framework.Painting.Axis? getScrollbarDirection() => this._axis;
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
        DartRuntimePrimitives.Assert(() => (this._cachedController is not null));
        DartRuntimePrimitives.Assert(() => (this._startDragScrollbarAxisOffset is not null));
        DartRuntimePrimitives.Assert(() => (this._lastDragUpdateOffset is not null));
        DartRuntimePrimitives.Assert(() => (this._startDragThumbOffset is not null));
        ScrollPosition positionLocal = this._cachedController!.position;
        double primaryDeltaFromDragStart = default!;
        double primaryDeltaFromLastDragUpdate = default!;
        switch (positionLocal.axisDirection)
        {
            case global::Doroti.Framework.Painting.AxisDirection.up:
                {
                    primaryDeltaFromDragStart = (DartRuntimePrimitives.RequireValue(this._startDragScrollbarAxisOffset).dy - localPosition.dy);
                    primaryDeltaFromLastDragUpdate = (DartRuntimePrimitives.RequireValue(this._lastDragUpdateOffset).dy - localPosition.dy);
                    break;
                }
            case global::Doroti.Framework.Painting.AxisDirection.right:
                {
                    primaryDeltaFromDragStart = (localPosition.dx - DartRuntimePrimitives.RequireValue(this._startDragScrollbarAxisOffset).dx);
                    primaryDeltaFromLastDragUpdate = (localPosition.dx - DartRuntimePrimitives.RequireValue(this._lastDragUpdateOffset).dx);
                    break;
                }
            case global::Doroti.Framework.Painting.AxisDirection.down:
                {
                    primaryDeltaFromDragStart = (localPosition.dy - DartRuntimePrimitives.RequireValue(this._startDragScrollbarAxisOffset).dy);
                    primaryDeltaFromLastDragUpdate = (localPosition.dy - DartRuntimePrimitives.RequireValue(this._lastDragUpdateOffset).dy);
                    break;
                }
            case global::Doroti.Framework.Painting.AxisDirection.left:
                {
                    primaryDeltaFromDragStart = (DartRuntimePrimitives.RequireValue(this._startDragScrollbarAxisOffset).dx - localPosition.dx);
                    primaryDeltaFromLastDragUpdate = (DartRuntimePrimitives.RequireValue(this._lastDragUpdateOffset).dx - localPosition.dx);
                    break;
                }
        }
        double scrollOffsetGlobal = this.scrollbarPainter.getTrackToScroll((DartRuntimePrimitives.RequireValue(this._startDragThumbOffset) + primaryDeltaFromDragStart));
        if ((((primaryDeltaFromDragStart > 0L) && (scrollOffsetGlobal < ((ScrollPosition)positionLocal).pixels)) || ((primaryDeltaFromDragStart < 0L) && (scrollOffsetGlobal > ((ScrollPosition)positionLocal).pixels))))
        {
            scrollOffsetGlobal = (((ScrollPosition)positionLocal).pixels + this.scrollbarPainter.getTrackToScroll(primaryDeltaFromLastDragUpdate));
        }
        if ((scrollOffsetGlobal != ((ScrollPosition)positionLocal).pixels))
        {
            double physicsAdjustment = ((ScrollPosition)positionLocal).physics.applyBoundaryConditions(positionLocal, scrollOffsetGlobal);
            double newPosition = (scrollOffsetGlobal - physicsAdjustment);
            switch (ScrollConfiguration.of(this.context).getPlatform(this.context))
            {
                case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                    {
                        newPosition = Dart_uiLibrary.clampDouble(newPosition, ((ScrollPosition)positionLocal).minScrollExtent, ((ScrollPosition)positionLocal).maxScrollExtent);
                        break;
                    }
                case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                case global::Doroti.Framework.Foundation.TargetPlatform.android:
                    break;
            }
            bool isReversed = global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionIsReversed(positionLocal.axisDirection);
            return (isReversed ? (newPosition - ((ScrollPosition)positionLocal).pixels) : (((ScrollPosition)positionLocal).pixels - newPosition));
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void handleThumbPress()
    {
        DartRuntimePrimitives.Assert(() => _debugCheckHasValidScrollPosition());
        _cachedController = this._effectiveScrollController;
        if ((getScrollbarDirection() is null))
        {
            return;
        }
        this._fadeoutTimer?.cancel();
        _thumbHold = this._cachedController!.position.hold(() => this._disposeThumbHold());
    }

    public virtual void handleThumbPressStart(Offset localPosition)
    {
        DartRuntimePrimitives.Assert(() => _debugCheckHasValidScrollPosition());
        global::Doroti.Framework.Painting.Axis? direction = getScrollbarDirection();
        if ((direction is null))
        {
            return;
        }
        this._fadeoutTimer?.cancel();
        this._fadeoutAnimationController.forward();
        DartRuntimePrimitives.Assert(() => (this._thumbDrag is null));
        ScrollPosition positionLocal = this._cachedController!.position;
        var renderBox = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((GlobalKey<IState>)this._scrollbarPainterKey).currentContext!.findRenderObject()!)!;
        var details = new global::Doroti.Framework.Gestures.DragStartDetails(localPosition: localPosition, globalPosition: ((Offset)((dynamic)renderBox).localToGlobal(localPosition)));
        _thumbDrag = positionLocal.drag(details, () => this._disposeThumbDrag());
        DartRuntimePrimitives.Assert(() => (this._thumbDrag is not null));
        DartRuntimePrimitives.Assert(() => (this._thumbHold is null));
        _startDragScrollbarAxisOffset = localPosition;
        _lastDragUpdateOffset = localPosition;
        _startDragThumbOffset = this.scrollbarPainter.getThumbScrollOffset();
    }

    public virtual void handleThumbPressUpdate(Offset localPosition)
    {
        DartRuntimePrimitives.Assert(() => _debugCheckHasValidScrollPosition());
        if ((object.Equals(this._lastDragUpdateOffset, localPosition)))
        {
            return;
        }
        ScrollPosition positionLocal = this._cachedController!.position;
        if (!((ScrollPosition)positionLocal).physics.shouldAcceptUserOffset(positionLocal))
        {
            return;
        }
        global::Doroti.Framework.Painting.Axis? direction = getScrollbarDirection();
        if ((direction is null))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => ((this._thumbHold is null) || (this._thumbDrag is null)));
        if ((this._thumbDrag is null))
        {
            return;
        }
        double? primaryDeltaLocal = _getPrimaryDelta(localPosition);
        if ((primaryDeltaLocal is null))
        {
            return;
        }
        global::Doroti.Ui.Offset deltaLocal = ((global::Doroti.Ui.Offset)(object?)(DartRuntimePrimitives.RequireValue(direction) switch { global::Doroti.Framework.Painting.Axis.horizontal => new global::Doroti.Ui.Offset(DartRuntimePrimitives.RequireValue(primaryDeltaLocal), 0), global::Doroti.Framework.Painting.Axis.vertical => new global::Doroti.Ui.Offset(0, DartRuntimePrimitives.RequireValue(primaryDeltaLocal)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        var renderBox = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((GlobalKey<IState>)this._scrollbarPainterKey).currentContext!.findRenderObject()!)!;
        var scrollDetails = new global::Doroti.Framework.Gestures.DragUpdateDetails(delta: deltaLocal, primaryDelta: DartRuntimePrimitives.RequireValue(primaryDeltaLocal), globalPosition: ((Offset)((dynamic)renderBox).localToGlobal(localPosition)), localPosition: localPosition);
        this._thumbDrag!.update(scrollDetails);
        _lastDragUpdateOffset = localPosition;
    }

    public virtual void handleThumbPressEnd(Offset localPosition, global::Doroti.Framework.Gestures.Velocity velocity)
    {
        DartRuntimePrimitives.Assert(() => _debugCheckHasValidScrollPosition());
        global::Doroti.Framework.Painting.Axis? direction = getScrollbarDirection();
        if ((direction is null))
        {
            return;
        }
        _maybeStartFadeoutTimer();
        _cachedController = null;
        _lastDragUpdateOffset = null;
        DartRuntimePrimitives.Assert(() => ((this._thumbHold is null) || (this._thumbDrag is null)));
        if ((this._thumbDrag is null))
        {
            return;
        }
        global::Doroti.Framework.Foundation.TargetPlatform platform = ScrollConfiguration.of(this.context).getPlatform(this.context);
        global::Doroti.Framework.Gestures.Velocity adjustedVelocity = (platform switch { global::Doroti.Framework.Foundation.TargetPlatform.iOS => -velocity, global::Doroti.Framework.Foundation.TargetPlatform.android => -velocity, _ => global::Doroti.Framework.Gestures.Velocity.zero });
        var renderBox = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((GlobalKey<IState>)this._scrollbarPainterKey).currentContext!.findRenderObject()!)!;
        var details = new global::Doroti.Framework.Gestures.DragEndDetails(localPosition: localPosition, globalPosition: ((Offset)((dynamic)renderBox).localToGlobal(localPosition)), velocity: adjustedVelocity, primaryVelocity: (DartRuntimePrimitives.RequireValue(direction) switch { global::Doroti.Framework.Painting.Axis.horizontal => ((global::Doroti.Framework.Gestures.Velocity)adjustedVelocity).pixelsPerSecond.dx, global::Doroti.Framework.Painting.Axis.vertical => ((global::Doroti.Framework.Gestures.Velocity)adjustedVelocity).pixelsPerSecond.dy, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        this._thumbDrag?.end(details);
        DartRuntimePrimitives.Assert(() => (this._thumbDrag is null));
        _startDragScrollbarAxisOffset = null;
        _lastDragUpdateOffset = null;
        _startDragThumbOffset = null;
        _cachedController = null;
    }

    public virtual void handleTrackTapDown(global::Doroti.Framework.Gestures.TapDownDetails details)
    {
        DartRuntimePrimitives.Assert(() => _debugCheckHasValidScrollPosition());
        _cachedController = this._effectiveScrollController;
        ScrollPosition positionLocal = this._cachedController!.position;
        if (!((ScrollPosition)positionLocal).physics.shouldAcceptUserOffset(positionLocal))
        {
            return;
        }
        global::Doroti.Framework.Painting.AxisDirection scrollDirection = default!;
        switch (global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(positionLocal.axisDirection))
        {
            case global::Doroti.Framework.Painting.Axis.vertical:
                {
                    if ((((global::Doroti.Framework.Gestures.TapDownDetails)details).localPosition.dy > ((ScrollbarPainter)this.scrollbarPainter)._thumbOffset))
                    {
                        scrollDirection = global::Doroti.Framework.Painting.AxisDirection.down;
                    }
                    else
                    {
                        scrollDirection = global::Doroti.Framework.Painting.AxisDirection.up;
                    }
                    break;
                }
            case global::Doroti.Framework.Painting.Axis.horizontal:
                {
                    if ((((global::Doroti.Framework.Gestures.TapDownDetails)details).localPosition.dx > ((ScrollbarPainter)this.scrollbarPainter)._thumbOffset))
                    {
                        scrollDirection = global::Doroti.Framework.Painting.AxisDirection.right;
                    }
                    else
                    {
                        scrollDirection = global::Doroti.Framework.Painting.AxisDirection.left;
                    }
                    break;
                }
        }
        ScrollableState? state = ((ScrollableState?)(object?)Scrollable.maybeOf(((ScrollPosition)positionLocal).context.notificationContext!));
        var intent = new ScrollIntent(direction: scrollDirection, type: ScrollIncrementType.page);
        DartRuntimePrimitives.Assert(() => (state is not null));
        double scrollIncrement = ScrollAction.getDirectionalIncrement(DartRuntimePrimitives.RequireValue(state), intent);
        DartRuntimePrimitives.Ignore(this._cachedController!.position.moveTo((this._cachedController!.position.pixels + scrollIncrement), duration: Duration.Create(milliseconds: 100L), curve: global::Doroti.Framework.Animation.Curves.easeInOut));
    }

    internal virtual bool _shouldUpdatePainter(global::Doroti.Framework.Painting.Axis notificationAxis)
    {
        ScrollController? scrollController = this._effectiveScrollController;
        if ((scrollController is null))
        {
            return true;
        }
        if ((((ScrollController)scrollController).positions.Count() > 1L))
        {
            return false;
        }
        return (!((ScrollController)scrollController).hasClients || (object.Equals(((ScrollController)scrollController).position.axis, notificationAxis)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _handleScrollMetricsNotification(ScrollMetricsNotification notification)
    {
        if (!this.widget.notificationPredicate(notification.asScrollUpdate()))
        {
            return false;
        }
        if ((this.showScrollbar && !this._fadeoutAnimationController.isForwardOrCompleted))
        {
            this._fadeoutAnimationController.forward();
        }
        ScrollMetrics metricsLocal = ((ScrollMetricsNotification)notification).metrics;
        if (_shouldUpdatePainter(((ScrollMetrics)metricsLocal).axis))
        {
            this.scrollbarPainter.update(metricsLocal, ((ScrollMetrics)metricsLocal).axisDirection);
        }
        if ((!object.Equals(((ScrollMetrics)metricsLocal).axis, this._axis)))
        {
            setState(((global::System.Action)(() =>
            {
                _axis = ((ScrollMetrics)metricsLocal).axis;
            })));
        }
        if ((this._maxScrollExtentPermitsScrolling != (((ScrollMetricsNotification)notification).metrics.maxScrollExtent > 0.0)))
        {
            setState(((global::System.Action)(() =>
            {
                _maxScrollExtentPermitsScrolling = !this._maxScrollExtentPermitsScrolling;
            })));
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _handleScrollNotification(ScrollNotification notification)
    {
        if (!this.widget.notificationPredicate(notification))
        {
            return false;
        }
        ScrollMetrics metricsLocal = ((ScrollNotification)notification).metrics;
        if ((((ScrollMetrics)metricsLocal).maxScrollExtent <= ((ScrollMetrics)metricsLocal).minScrollExtent))
        {
            if (this._fadeoutAnimationController.isForwardOrCompleted)
            {
                this._fadeoutAnimationController.reverse();
            }
            if (_shouldUpdatePainter(((ScrollMetrics)metricsLocal).axis))
            {
                this.scrollbarPainter.update(metricsLocal, ((ScrollMetrics)metricsLocal).axisDirection);
            }
            return false;
        }
        if (((notification is ScrollUpdateNotification) || (notification is OverscrollNotification)))
        {
            if (!this._fadeoutAnimationController.isForwardOrCompleted)
            {
                this._fadeoutAnimationController.forward();
            }
            this._fadeoutTimer?.cancel();
            if (_shouldUpdatePainter(((ScrollMetrics)metricsLocal).axis))
            {
                this.scrollbarPainter.update(metricsLocal, ((ScrollMetrics)metricsLocal).axisDirection);
            }
        }
        else
        {
            if ((notification is ScrollEndNotification))
            {
                ScrollEndNotification notification__as73302 = (ScrollEndNotification)notification;
                if ((this._thumbDrag is null))
                {
                    _maybeStartFadeoutTimer();
                }
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleThumbDragDown(global::Doroti.Framework.Gestures.DragDownDetails details)
    {
        handleThumbPress();
    }

    internal virtual global::Doroti.Ui.Offset _globalToScrollbar(Offset offset)
    {
        var renderBox = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((GlobalKey<IState>)this._scrollbarPainterKey).currentContext!.findRenderObject()!)!;
        return ((global::Doroti.Ui.Offset)(object?)((Offset)((dynamic)renderBox).globalToLocal(offset)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleThumbDragStart(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        handleThumbPressStart(_globalToScrollbar(((global::Doroti.Framework.Gestures.DragStartDetails)details).globalPosition));
    }

    internal virtual void _handleThumbDragUpdate(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        handleThumbPressUpdate(_globalToScrollbar(((global::Doroti.Framework.Gestures.DragUpdateDetails)details).globalPosition));
    }

    internal virtual void _handleThumbDragEnd(global::Doroti.Framework.Gestures.DragEndDetails details)
    {
        handleThumbPressEnd(_globalToScrollbar(((global::Doroti.Framework.Gestures.DragEndDetails)details).globalPosition), ((global::Doroti.Framework.Gestures.DragEndDetails)details).velocity);
    }

    internal virtual void _handleThumbDragCancel()
    {
        if ((((GlobalKey<RawGestureDetectorState>)this._gestureDetectorKey).currentContext is null))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => ((this._thumbHold is null) || (this._thumbDrag is null)));
        this._thumbHold?.cancel();
        this._thumbDrag?.cancel();
        DartRuntimePrimitives.Assert(() => (this._thumbHold is null));
        DartRuntimePrimitives.Assert(() => (this._thumbDrag is null));
    }

    internal virtual void _initThumbDragGestureRecognizer(global::Doroti.Framework.Gestures.DragGestureRecognizer instance)
    {
        instance.onDown = (global::System.Action<global::Doroti.Framework.Gestures.DragDownDetails>)this._handleThumbDragDown;
        instance.onStart = (global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>)this._handleThumbDragStart;
        instance.onUpdate = (global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>)this._handleThumbDragUpdate;
        instance.onEnd = (global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>)this._handleThumbDragEnd;
        instance.onCancel = (global::System.Action)this._handleThumbDragCancel;
        instance.gestureSettings = new global::Doroti.Framework.Gestures.DeviceGestureSettings(touchSlop: 0);
        instance.dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.down;
    }

    internal virtual bool _canHandleScrollGestures()
    {
        return ((((this.enableGestures && (this._effectiveScrollController is not null)) && (this._effectiveScrollController!.positions.Count() == 1L)) && this._effectiveScrollController!.position.hasContentDimensions) && ((this._effectiveScrollController!.position.maxScrollExtent - this._effectiveScrollController!.position.minScrollExtent) > global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance));
        throw new InvalidOperationException("Dart control flow completed without a value.");
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
            switch (this._effectiveScrollController!.position.axis)
            {
                case global::Doroti.Framework.Painting.Axis.horizontal:
                    {
                        gestures[typeof(_HorizontalThumbDragGestureRecognizer__scrollbar)] = new GestureRecognizerFactoryWithHandlers<_HorizontalThumbDragGestureRecognizer__scrollbar>(((global::System.Func<_HorizontalThumbDragGestureRecognizer__scrollbar>)(() => new _HorizontalThumbDragGestureRecognizer__scrollbar(debugOwner: this, customPaintKey: this._scrollbarPainterKey))), (__arg0) => ((global::System.Action<global::Doroti.Framework.Gestures.DragGestureRecognizer>)this._initThumbDragGestureRecognizer)(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Gestures.DragGestureRecognizer>(__arg0)));
                        break;
                    }
                case global::Doroti.Framework.Painting.Axis.vertical:
                    {
                        gestures[typeof(_VerticalThumbDragGestureRecognizer__scrollbar)] = new GestureRecognizerFactoryWithHandlers<_VerticalThumbDragGestureRecognizer__scrollbar>(((global::System.Func<_VerticalThumbDragGestureRecognizer__scrollbar>)(() => new _VerticalThumbDragGestureRecognizer__scrollbar(debugOwner: this, customPaintKey: this._scrollbarPainterKey))), (__arg0) => ((global::System.Action<global::Doroti.Framework.Gestures.DragGestureRecognizer>)this._initThumbDragGestureRecognizer)(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Gestures.DragGestureRecognizer>(__arg0)));
                        break;
                    }
            }
            gestures[typeof(_TrackTapGestureRecognizer__scrollbar)] = new GestureRecognizerFactoryWithHandlers<_TrackTapGestureRecognizer__scrollbar>(((global::System.Func<_TrackTapGestureRecognizer__scrollbar>)(() => new _TrackTapGestureRecognizer__scrollbar(debugOwner: this, customPaintKey: this._scrollbarPainterKey))), ((global::System.Action<_TrackTapGestureRecognizer__scrollbar>)((instance) =>
            {
                instance.onTapDown = this.handleTrackTapDown;
            })));
            return gestures;
            return default!;
        }
    }
    public virtual bool isPointerOverTrack(Offset position, PointerDeviceKind kind)
    {
        if ((((GlobalKey<IState>)this._scrollbarPainterKey).currentContext is null))
        {
            return false;
        }
        global::Doroti.Ui.Offset localOffset = ((global::Doroti.Ui.Offset)(object?)ScrollbarLibrary._getLocalOffset(this._scrollbarPainterKey, position));
        return (this.scrollbarPainter.hitTestInteractive(localOffset, kind) && !this.scrollbarPainter.hitTestOnlyThumbInteractive(localOffset, kind));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool isPointerOverThumb(Offset position, PointerDeviceKind kind)
    {
        if ((((GlobalKey<IState>)this._scrollbarPainterKey).currentContext is null))
        {
            return false;
        }
        global::Doroti.Ui.Offset localOffset = ((global::Doroti.Ui.Offset)(object?)ScrollbarLibrary._getLocalOffset(this._scrollbarPainterKey, position));
        return this.scrollbarPainter.hitTestOnlyThumbInteractive(localOffset, kind);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool isPointerOverScrollbar(Offset position, PointerDeviceKind kind, bool forHover = false)
    {
        if ((((GlobalKey<IState>)this._scrollbarPainterKey).currentContext is null))
        {
            return false;
        }
        global::Doroti.Ui.Offset localOffset = ((global::Doroti.Ui.Offset)(object?)ScrollbarLibrary._getLocalOffset(this._scrollbarPainterKey, position));
        return this.scrollbarPainter.hitTestInteractive(localOffset, kind, forHover: true);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void handleHover(global::Doroti.Framework.Gestures.PointerHoverEvent @event)
    {
        if (isPointerOverScrollbar(@event.position, @event.kind, forHover: true))
        {
            _hoverIsActive = true;
            this._fadeoutAnimationController.forward();
            this._fadeoutTimer?.cancel();
        }
        else
        {
            if (this._hoverIsActive)
            {
                _hoverIsActive = false;
                _maybeStartFadeoutTimer();
            }
        }
    }

    public virtual void handleHoverExit(global::Doroti.Framework.Gestures.PointerExitEvent @event)
    {
        _hoverIsActive = false;
        _maybeStartFadeoutTimer();
    }

    internal virtual double _pointerSignalEventDelta(global::Doroti.Framework.Gestures.PointerScrollEvent @event)
    {
        DartRuntimePrimitives.Assert(() => (this._cachedController is not null));
        double delta = ((object.Equals(this._cachedController!.position.axis, global::Doroti.Framework.Painting.Axis.horizontal)) ? ((global::Doroti.Framework.Gestures.PointerScrollEvent)@event).scrollDelta.dx : ((global::Doroti.Framework.Gestures.PointerScrollEvent)@event).scrollDelta.dy);
        if (global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionIsReversed(this._cachedController!.position.axisDirection))
        {
            delta *= -1L;
        }
        return delta;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _targetScrollOffsetForPointerScroll(double delta)
    {
        DartRuntimePrimitives.Assert(() => (this._cachedController is not null));
        return Math.Min(Math.Max((this._cachedController!.position.pixels + delta), this._cachedController!.position.minScrollExtent), this._cachedController!.position.maxScrollExtent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handlePointerScroll(global::Doroti.Framework.Gestures.PointerEvent @event)
    {
        DartRuntimePrimitives.Assert(() => (@event is global::Doroti.Framework.Gestures.PointerScrollEvent));
        _cachedController = this._effectiveScrollController;
        double delta = _pointerSignalEventDelta(((global::Doroti.Framework.Gestures.PointerScrollEvent?)(object?)@event)!);
        double targetScrollOffset = _targetScrollOffsetForPointerScroll(delta);
        if (((delta != 0.0) && (targetScrollOffset != this._cachedController!.position.pixels)))
        {
            this._cachedController!.position.pointerScroll(delta);
        }
    }

    internal virtual void _receivedPointerSignal(global::Doroti.Framework.Gestures.PointerSignalEvent @event)
    {
        _cachedController = this._effectiveScrollController;
        if ((((((this.scrollbarPainter.hitTest(@event.localPosition) ?? false)) && (this._cachedController is not null)) && this._cachedController!.hasClients) && (((this._thumbDrag is null) || global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb))))
        {
            ScrollPosition positionLocal = this._cachedController!.position;
            if ((@event is global::Doroti.Framework.Gestures.PointerScrollEvent))
            {
                global::Doroti.Framework.Gestures.PointerScrollEvent @event__as82127 = (global::Doroti.Framework.Gestures.PointerScrollEvent)@event;
                if (!((ScrollPosition)positionLocal).physics.shouldAcceptUserOffset(positionLocal))
                {
                    return;
                }
                double delta = _pointerSignalEventDelta(((global::Doroti.Framework.Gestures.PointerScrollEvent)@event__as82127));
                double targetScrollOffset = _targetScrollOffsetForPointerScroll(delta);
                if (((delta != 0.0) && (targetScrollOffset != ((ScrollPosition)positionLocal).pixels)))
                {
                    global::Doroti.Framework.Gestures.GestureBinding.instance.pointerSignalResolver.register(((global::Doroti.Framework.Gestures.PointerScrollEvent)@event__as82127), (__arg0) => ((global::System.Action<global::Doroti.Framework.Gestures.PointerEvent>)this._handlePointerScroll)(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Gestures.PointerEvent>(__arg0)));
                }
            }
            else
            {
                if ((@event is global::Doroti.Framework.Gestures.PointerScrollInertiaCancelEvent))
                {
                    global::Doroti.Framework.Gestures.PointerScrollInertiaCancelEvent @event__as82591 = (global::Doroti.Framework.Gestures.PointerScrollInertiaCancelEvent)@event;
                    positionLocal.jumpTo(((ScrollPosition)positionLocal).pixels);
                }
            }
        }
    }

    public override void dispose()
    {
        _isDisposed = true;
        this._fadeoutTimer?.cancel();
        _fadeoutTimer = null;
        this._fadeoutAnimationController.dispose();
        this.scrollbarPainter.dispose();
        this._fadeoutOpacityAnimation.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._tickerModeNotifier?.removeListener(this._updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        updateScrollbarPainter();
        return ((Widget)(object?)new NotificationListener<ScrollMetricsNotification>(onNotification: (global::System.Func<ScrollMetricsNotification, bool>)this._handleScrollMetricsNotification, child: new NotificationListener<ScrollNotification>(onNotification: (global::System.Func<ScrollNotification, bool>)this._handleScrollNotification, child: new RepaintBoundary(child: new Listener(onPointerSignal: (global::System.Action<global::Doroti.Framework.Gestures.PointerSignalEvent>)this._receivedPointerSignal, child: new RawGestureDetector(key: this._gestureDetectorKey, gestures: this._gestures, child: new MouseRegion(onExit: ((global::System.Action<global::Doroti.Framework.Gestures.PointerExitEvent>)((@event) =>
        {
            switch (@event.kind)
            {
                case PointerDeviceKind.mouse:
                case PointerDeviceKind.trackpad:
                    {
                        if (this.enableGestures)
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
        })), onHover: ((global::System.Action<global::Doroti.Framework.Gestures.PointerHoverEvent>)((@event) =>
        {
            switch (@event.kind)
            {
                case PointerDeviceKind.mouse:
                case PointerDeviceKind.trackpad:
                    {
                        if (this.enableGestures)
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
        })), child: new CustomPaint(key: this._scrollbarPainterKey, foregroundPainter: new _ScrollbarCustomPainterAdapter(this.scrollbarPainter), child: new RepaintBoundary(child: ((RawScrollbar)(object)this.widget).child)))))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = this._tickerModeNotifier!.value;
        var result = ((Func<_WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
    __cascade.muted = !((TickerModeData)values).enabled;
    __cascade.forceFrames = ((TickerModeData)values).forceFrames;
    return __cascade;
}))();
        this._tickers!.Add(result);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(_WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => (this._tickers is not null));
        DartRuntimePrimitives.Assert(() => this._tickers!.Contains(ticker));
        this._tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if ((this._tickers is not null))
        {
            TickerModeData values = this._tickerModeNotifier!.value;
            bool mutedLocal = !((TickerModeData)values).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = ((TickerModeData)values).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(this._updateTickers);
        newNotifier.addListener(this._updateTickers);
        this._tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

public static partial class ScrollbarLibrary
{
    internal static Offset _getLocalOffset(GlobalKey<IState> scrollbarPainterKey, Offset position)
    {
        var renderBox = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((GlobalKey<IState>)scrollbarPainterKey).currentContext!.findRenderObject()!)!;
        return ((Offset)((dynamic)renderBox).globalToLocal(position));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class ScrollbarLibrary
{
    internal static bool _isThumbEvent(GlobalKey<IState> customPaintKey, global::Doroti.Framework.Gestures.PointerEvent @event)
    {
        if ((((GlobalKey<IState>)customPaintKey).currentContext is null))
        {
            return false;
        }
        var customPaint = ((CustomPaint?)(object?)((GlobalKey<IState>)customPaintKey).currentContext!.widget)!;
        var painter = _ScrollbarCustomPainterAdapter.Unwrap(((CustomPaint)customPaint).foregroundPainter!);
        global::Doroti.Ui.Offset localOffset = ((global::Doroti.Ui.Offset)(object?)ScrollbarLibrary._getLocalOffset(customPaintKey, ((global::Doroti.Framework.Gestures.PointerEvent)@event).position));
        return painter.hitTestOnlyThumbInteractive(localOffset, ((global::Doroti.Framework.Gestures.PointerEvent)@event).kind);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class ScrollbarLibrary
{
    internal static bool _isTrackEvent(GlobalKey<IState> customPaintKey, global::Doroti.Framework.Gestures.PointerEvent @event)
    {
        if ((((GlobalKey<IState>)customPaintKey).currentContext is null))
        {
            return false;
        }
        var customPaint = ((CustomPaint?)(object?)((GlobalKey<IState>)customPaintKey).currentContext!.widget)!;
        var painter = _ScrollbarCustomPainterAdapter.Unwrap(((CustomPaint)customPaint).foregroundPainter!);
        global::Doroti.Ui.Offset localOffset = ((global::Doroti.Ui.Offset)(object?)ScrollbarLibrary._getLocalOffset(customPaintKey, ((global::Doroti.Framework.Gestures.PointerEvent)@event).position));
        global::Doroti.Ui.PointerDeviceKind kindLocal = ((global::Doroti.Framework.Gestures.PointerEvent)@event).kind;
        return (painter.hitTestInteractive(localOffset, kindLocal) && !painter.hitTestOnlyThumbInteractive(localOffset, kindLocal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _TrackTapGestureRecognizer__scrollbar : global::Doroti.Framework.Gestures.TapGestureRecognizer
{
    internal virtual GlobalKey<IState> _customPaintKey { get; private set; } = default!;

    internal _TrackTapGestureRecognizer__scrollbar(object? debugOwner, GlobalKey<IState> customPaintKey) : base(debugOwner: debugOwner)
    {
        this._customPaintKey = customPaintKey;
    }

    public override bool isPointerAllowed(global::Doroti.Framework.Gestures.PointerDownEvent @event)
    {
        return (ScrollbarLibrary._isTrackEvent(this._customPaintKey, @event) && base.isPointerAllowed(@event));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _VerticalThumbDragGestureRecognizer__scrollbar : global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer
{
    internal virtual GlobalKey<IState> _customPaintKey { get; private set; } = default!;

    internal _VerticalThumbDragGestureRecognizer__scrollbar(object debugOwner, GlobalKey<IState> customPaintKey) : base(debugOwner: debugOwner)
    {
        this._customPaintKey = customPaintKey;
    }

    public override bool isPointerPanZoomAllowed(global::Doroti.Framework.Gestures.PointerPanZoomStartEvent @event)
    {
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isPointerAllowed(global::Doroti.Framework.Gestures.PointerDownEvent @event)
    {
        return (ScrollbarLibrary._isThumbEvent(this._customPaintKey, @event) && base.isPointerAllowed(@event));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _HorizontalThumbDragGestureRecognizer__scrollbar : global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer
{
    internal virtual GlobalKey<IState> _customPaintKey { get; private set; } = default!;

    internal _HorizontalThumbDragGestureRecognizer__scrollbar(object debugOwner, GlobalKey<IState> customPaintKey) : base(debugOwner: debugOwner)
    {
        this._customPaintKey = customPaintKey;
    }

    public override bool isPointerPanZoomAllowed(global::Doroti.Framework.Gestures.PointerPanZoomStartEvent @event)
    {
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isPointerAllowed(global::Doroti.Framework.Gestures.PointerDownEvent @event)
    {
        return (ScrollbarLibrary._isThumbEvent(this._customPaintKey, @event) && base.isPointerAllowed(@event));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}


internal sealed class _ScrollbarCustomPainterAdapter : global::Doroti.Framework.Rendering.CustomPainter
{
    private readonly ScrollbarPainter _owner;
    internal _ScrollbarCustomPainterAdapter(ScrollbarPainter owner) : base(owner) => _owner = owner;
    internal static ScrollbarPainter Unwrap(global::Doroti.Framework.Rendering.CustomPainter painter) => painter is _ScrollbarCustomPainterAdapter adapter ? adapter._owner : (ScrollbarPainter)(object)painter;
    public override void paint(Canvas canvas, Size size) => _owner.paint(canvas, size);
    public override bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate) => oldDelegate is not _ScrollbarCustomPainterAdapter other || _owner.shouldRepaint(other._owner);
    public override bool? hitTest(Offset position) => _owner.hitTest(position);
}
