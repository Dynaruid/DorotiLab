// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/scroll_metrics.dart
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

public interface ScrollMetrics
{
    public ScrollMetrics copyWith(double? minScrollExtent = null, double? maxScrollExtent = null, double? pixels = null, double? viewportDimension = null, global::Doroti.Generated.Framework.Painting.AxisDirection? axisDirection = null, double? devicePixelRatio = null, long? itemIndex = null, double? minRange = null, double? maxRange = null, double? correctionOffset = null, double? viewportFraction = null);
    public double minScrollExtent { get; }
    public double maxScrollExtent { get; }
    public bool hasContentDimensions { get; }
    public double pixels { get; }
    public bool hasPixels { get; }
    public double viewportDimension { get; }
    public bool hasViewportDimension { get; }
    public global::Doroti.Generated.Framework.Painting.AxisDirection axisDirection { get; }
    public global::Doroti.Generated.Framework.Painting.Axis axis { get; }
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
    public virtual global::Doroti.Generated.Framework.Painting.AxisDirection axisDirection { get; private set; } = default!;
    public virtual double devicePixelRatio { get; private set; } = default!;

    public FixedScrollMetrics(double? minScrollExtent, double? maxScrollExtent, double? pixels, double? viewportDimension, global::Doroti.Generated.Framework.Painting.AxisDirection axisDirection, double devicePixelRatio)
    {
        this.axisDirection = axisDirection;
        this.devicePixelRatio = devicePixelRatio;
        this._minScrollExtent = minScrollExtent;
        this._maxScrollExtent = maxScrollExtent;
        this._pixels = pixels;
        this._viewportDimension = viewportDimension;
    }

    public virtual double minScrollExtent => DartRuntimePrimitives.ConvertValue<double>(DartRuntimePrimitives.RequireValue(this._minScrollExtent));
    public virtual double maxScrollExtent => DartRuntimePrimitives.ConvertValue<double>(DartRuntimePrimitives.RequireValue(this._maxScrollExtent));
    public virtual bool hasContentDimensions => DartRuntimePrimitives.ConvertValue<bool>(((this._minScrollExtent is not null) && (this._maxScrollExtent is not null)));
    public virtual double pixels => DartRuntimePrimitives.ConvertValue<double>(DartRuntimePrimitives.RequireValue(this._pixels));
    public virtual bool hasPixels => DartRuntimePrimitives.ConvertValue<bool>((this._pixels is not null));
    public virtual double viewportDimension => DartRuntimePrimitives.ConvertValue<double>(DartRuntimePrimitives.RequireValue(this._viewportDimension));
    public virtual bool hasViewportDimension => DartRuntimePrimitives.ConvertValue<bool>((this._viewportDimension is not null));
    public override string ToString()
    {
        return $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "FixedScrollMetrics"))}({this.extentBefore.toStringAsFixed(1L)}..[{this.extentInside.toStringAsFixed(1L)}]..{this.extentAfter.toStringAsFixed(1L)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ScrollMetrics copyWith(double? minScrollExtent = null, double? maxScrollExtent = null, double? pixels = null, double? viewportDimension = null, global::Doroti.Generated.Framework.Painting.AxisDirection? axisDirection = null, double? devicePixelRatio = null, long? itemIndex = null, double? minRange = null, double? maxRange = null, double? correctionOffset = null, double? viewportFraction = null)
    {
        return ((ScrollMetrics)(object?)new FixedScrollMetrics(minScrollExtent: (minScrollExtent ?? ((this.hasContentDimensions ? this.minScrollExtent : null))), maxScrollExtent: (maxScrollExtent ?? ((this.hasContentDimensions ? this.maxScrollExtent : null))), pixels: (pixels ?? ((this.hasPixels ? this.pixels : null))), viewportDimension: (viewportDimension ?? ((this.hasViewportDimension ? this.viewportDimension : null))), axisDirection: (axisDirection ?? this.axisDirection), devicePixelRatio: (devicePixelRatio ?? this.devicePixelRatio)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Painting.Axis axis => global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(DartRuntimePrimitives.RequireValue(this.axisDirection));
    public virtual bool outOfRange => DartRuntimePrimitives.ConvertValue<bool>(((this.pixels < this.minScrollExtent) || (this.pixels > this.maxScrollExtent)));
    public virtual bool atEdge => DartRuntimePrimitives.ConvertValue<bool>(((this.pixels == this.minScrollExtent) || (this.pixels == this.maxScrollExtent)));
    public virtual double extentBefore => Math.Max((this.pixels - this.minScrollExtent), 0.0);
    public virtual double extentInside
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (this.minScrollExtent <= this.maxScrollExtent));
            return ((this.viewportDimension - Dart_uiLibrary.clampDouble((this.minScrollExtent - this.pixels), 0, this.viewportDimension)) - Dart_uiLibrary.clampDouble((this.pixels - this.maxScrollExtent), 0, this.viewportDimension));
            return default!;
        }
    }
    public virtual double extentAfter => Math.Max((this.maxScrollExtent - this.pixels), 0.0);
    public virtual double extentTotal => DartRuntimePrimitives.ConvertValue<double>(((this.maxScrollExtent - this.minScrollExtent) + this.viewportDimension));
}

