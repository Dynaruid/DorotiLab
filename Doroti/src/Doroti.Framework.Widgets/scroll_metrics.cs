// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/scroll_metrics.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public interface ScrollMetrics
{
    public ScrollMetrics copyWith(double? minScrollExtent = null, double? maxScrollExtent = null, double? pixels = null, double? viewportDimension = null, global::Doroti.Framework.Painting.AxisDirection? axisDirection = null, double? devicePixelRatio = null, long? itemIndex = null, double? minRange = null, double? maxRange = null, double? correctionOffset = null, double? viewportFraction = null);
    public double minScrollExtent { get; }
    public double maxScrollExtent { get; }
    public bool hasContentDimensions { get; }
    public double pixels { get; }
    public bool hasPixels { get; }
    public double viewportDimension { get; }
    public bool hasViewportDimension { get; }
    public global::Doroti.Framework.Painting.AxisDirection axisDirection { get; }
    public global::Doroti.Framework.Painting.Axis axis { get; }
    public bool outOfRange { get; }
    public bool atEdge { get; }
    public double extentBefore { get; }
    public double extentInside { get; }
    public double extentAfter { get; }
    public double extentTotal { get; }
    public double devicePixelRatio { get; }
}

public class FixedScrollMetrics : ScrollMetrics
{
    internal virtual double? _minScrollExtent { get; private set; }
    internal virtual double? _maxScrollExtent { get; private set; }
    internal virtual double? _pixels { get; private set; }
    internal virtual double? _viewportDimension { get; private set; }
    public virtual global::Doroti.Framework.Painting.AxisDirection axisDirection { get; private set; } = default!;
    public virtual double devicePixelRatio { get; private set; } = default!;

    public FixedScrollMetrics(double? minScrollExtent, double? maxScrollExtent, double? pixels, double? viewportDimension, global::Doroti.Framework.Painting.AxisDirection axisDirection, double devicePixelRatio)
    {
        this.axisDirection = axisDirection;
        this.devicePixelRatio = devicePixelRatio;
        _minScrollExtent = minScrollExtent;
        _maxScrollExtent = maxScrollExtent;
        _pixels = pixels;
        _viewportDimension = viewportDimension;
    }

    public virtual double minScrollExtent => DartRuntimePrimitives.ConvertValue<double>(DartRuntimePrimitives.RequireValue(_minScrollExtent));
    public virtual double maxScrollExtent => DartRuntimePrimitives.ConvertValue<double>(DartRuntimePrimitives.RequireValue(_maxScrollExtent));
    public virtual bool hasContentDimensions => DartRuntimePrimitives.ConvertValue<bool>((_minScrollExtent is not null) && (_maxScrollExtent is not null));
    public virtual double pixels => DartRuntimePrimitives.ConvertValue<double>(DartRuntimePrimitives.RequireValue(_pixels));
    public virtual bool hasPixels => DartRuntimePrimitives.ConvertValue<bool>(_pixels is not null);
    public virtual double viewportDimension => DartRuntimePrimitives.ConvertValue<double>(DartRuntimePrimitives.RequireValue(_viewportDimension));
    public virtual bool hasViewportDimension => DartRuntimePrimitives.ConvertValue<bool>(_viewportDimension is not null);
    public override string ToString()
    {
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "FixedScrollMetrics")}({extentBefore.toStringAsFixed(1L)}..[{extentInside.toStringAsFixed(1L)}]..{extentAfter.toStringAsFixed(1L)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ScrollMetrics copyWith(double? minScrollExtent = null, double? maxScrollExtent = null, double? pixels = null, double? viewportDimension = null, global::Doroti.Framework.Painting.AxisDirection? axisDirection = null, double? devicePixelRatio = null, long? itemIndex = null, double? minRange = null, double? maxRange = null, double? correctionOffset = null, double? viewportFraction = null)
    {
        return new FixedScrollMetrics(minScrollExtent: minScrollExtent ?? (hasContentDimensions ? this.minScrollExtent : null), maxScrollExtent: maxScrollExtent ?? (hasContentDimensions ? this.maxScrollExtent : null), pixels: pixels ?? (hasPixels ? this.pixels : null), viewportDimension: viewportDimension ?? (hasViewportDimension ? this.viewportDimension : null), axisDirection: axisDirection ?? this.axisDirection, devicePixelRatio: devicePixelRatio ?? this.devicePixelRatio);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Painting.Axis axis => Basic_typesLibrary.axisDirectionToAxis(DartRuntimePrimitives.RequireValue(axisDirection));
    public virtual bool outOfRange => DartRuntimePrimitives.ConvertValue<bool>((pixels < minScrollExtent) || (pixels > maxScrollExtent));
    public virtual bool atEdge => DartRuntimePrimitives.ConvertValue<bool>((pixels == minScrollExtent) || (pixels == maxScrollExtent));
    public virtual double extentBefore => Math.Max(pixels - minScrollExtent, 0.0);
    public virtual double extentInside
    {
        get
        {
            DartRuntimePrimitives.Assert(() => minScrollExtent <= maxScrollExtent);
            return viewportDimension - Dart_uiLibrary.clampDouble(minScrollExtent - pixels, 0, viewportDimension) - Dart_uiLibrary.clampDouble(pixels - maxScrollExtent, 0, viewportDimension);
        }
    }
    public virtual double extentAfter => Math.Max(maxScrollExtent - pixels, 0.0);
    public virtual double extentTotal => DartRuntimePrimitives.ConvertValue<double>(maxScrollExtent - minScrollExtent + viewportDimension);
}

