using Doroti.Framework.Painting;
using Doroti.Framework.Widgets;
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

internal sealed class _WidgetStateInputBorder__material_state : WidgetStateInputBorder
{
    private readonly Func<HashSet<WidgetState>, InputBorder> resolver;
    private readonly OutlineInputBorder shape = new();
    internal _WidgetStateInputBorder__material_state(Func<HashSet<WidgetState>, InputBorder> resolver) => this.resolver = resolver;
    public override InputBorder resolve(HashSet<WidgetState> states) => resolver(states);
    public override BorderSide borderSide => shape.borderSide;
    public override bool isOutline => shape.isOutline;
    public override EdgeInsetsGeometry dimensions => shape.dimensions;
    public override global::Doroti.Ui.Path getOuterPath(Rect rect, TextDirection? textDirection = null) => shape.getOuterPath(rect, textDirection);
    public override global::Doroti.Ui.Path getInnerPath(Rect rect, TextDirection? textDirection = null) => shape.getInnerPath(rect, textDirection);
    public override ShapeBorder scale(double t) => shape.scale(t);
    public override InputBorder copyWith(BorderSide? borderSide = null, BorderRadius? borderRadius = null, double? gapPadding = null, ShapeBorder? shape = null) => this.shape.copyWith(borderSide, borderRadius, gapPadding, shape);
    public override void paint(Canvas canvas, Rect rect, TextDirection? textDirection = null, double? gapStart = 0.0, double gapExtent = 0.0, double gapPercentage = 0.0, BoxShape shape = BoxShape.rectangle, BorderRadius? borderRadius = null) => this.shape.paint(canvas, rect, textDirection, gapStart, gapExtent, gapPercentage, shape, borderRadius);
    public override void paintInterior(Canvas canvas, Rect rect, Paint paint, TextDirection? textDirection = null) => shape.paintInterior(canvas, rect, paint, textDirection);
    public override bool preferPaintInterior => shape.preferPaintInterior;
}

internal sealed class _WidgetInputBorderMapper__material_state : WidgetStateInputBorder, IWidgetStateMapping<InputBorder>
{
    private readonly WidgetStateMapper<InputBorder> mapper;
    DartMap<WidgetStatesConstraint, InputBorder> IWidgetStateMapping<InputBorder>.Mapping => ((IWidgetStateMapping<InputBorder>)mapper).Mapping;
    internal _WidgetInputBorderMapper__material_state(DartMap<WidgetStatesConstraint, InputBorder> map) => mapper = new(map);
    public override InputBorder resolve(HashSet<WidgetState> states) => WidgetStateMapping.ResolveRequired(mapper, states);
    public override bool Equals(object? other) => mapper.Equals(other);
    public override int GetHashCode() => mapper.GetHashCode();
    public override string ToString() => mapper.ToString();
    public override EdgeInsetsGeometry dimensions => throw WidgetStateMapping.Unresolved(nameof(dimensions));
    public override ShapeBorder? add(ShapeBorder other, bool reversed = false) => throw WidgetStateMapping.Unresolved(nameof(add));
    public override ShapeBorder op_Add(ShapeBorder other) => throw WidgetStateMapping.Unresolved(nameof(op_Add));
    public override ShapeBorder scale(double t) => throw WidgetStateMapping.Unresolved(nameof(scale));
    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t) => throw WidgetStateMapping.Unresolved(nameof(lerpFrom));
    public override ShapeBorder? lerpTo(ShapeBorder? b, double t) => throw WidgetStateMapping.Unresolved(nameof(lerpTo));
    public override global::Doroti.Ui.Path getOuterPath(Rect rect, TextDirection? textDirection = null) => throw WidgetStateMapping.Unresolved(nameof(getOuterPath));
    public override global::Doroti.Ui.Path getInnerPath(Rect rect, TextDirection? textDirection = null) => throw WidgetStateMapping.Unresolved(nameof(getInnerPath));
    public override bool hitTest(Rect rect, Offset position, TextDirection? textDirection = null) => throw WidgetStateMapping.Unresolved(nameof(hitTest));
    public override void paintInterior(Canvas canvas, Rect rect, Paint paint, TextDirection? textDirection = null) => throw WidgetStateMapping.Unresolved(nameof(paintInterior));
    public override bool preferPaintInterior => throw WidgetStateMapping.Unresolved(nameof(preferPaintInterior));
    public override global::Doroti.Framework.Painting.BorderSide borderSide => throw WidgetStateMapping.Unresolved(nameof(borderSide));
    public override InputBorder copyWith(global::Doroti.Framework.Painting.BorderSide? borderSide = null, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, double? gapPadding = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null) => throw WidgetStateMapping.Unresolved(nameof(copyWith));
    public override bool isOutline => throw WidgetStateMapping.Unresolved(nameof(isOutline));
    public override void paint(Canvas canvas, Rect rect, TextDirection? textDirection = null, global::Doroti.Framework.Painting.BoxShape shape = BoxShape.rectangle, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null) => throw WidgetStateMapping.Unresolved(nameof(paint));
    public override void paint(Canvas canvas, Rect rect, TextDirection? textDirection = null, double? gapStart = 0.0, double gapExtent = 0.0, double gapPercentage = default!, global::Doroti.Framework.Painting.BoxShape shape = BoxShape.rectangle, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null) => throw WidgetStateMapping.Unresolved(nameof(paint));
}
