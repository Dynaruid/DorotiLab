// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/menu_style.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class MenuStyle : Diagnosticable
{
    public virtual WidgetStateProperty<Color?>? backgroundColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? shadowColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? surfaceTintColor { get; private set; }
    public virtual WidgetStateProperty<double?>? elevation { get; private set; }
    public virtual WidgetStateProperty<EdgeInsetsGeometry?>? padding { get; private set; }
    public virtual WidgetStateProperty<Size?>? minimumSize { get; private set; }
    public virtual WidgetStateProperty<Size?>? fixedSize { get; private set; }
    public virtual WidgetStateProperty<Size?>? maximumSize { get; private set; }
    public virtual WidgetStateProperty<BorderSide?>? side { get; private set; }
    public virtual WidgetStateProperty<OutlinedBorder?>? shape { get; private set; }
    public virtual WidgetStateProperty<MouseCursor?>? mouseCursor { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual AlignmentGeometry? alignment { get; private set; }

    public MenuStyle(WidgetStateProperty<Color?>? backgroundColor = null, WidgetStateProperty<Color?>? shadowColor = null, WidgetStateProperty<Color?>? surfaceTintColor = null, WidgetStateProperty<double?>? elevation = null, WidgetStateProperty<EdgeInsetsGeometry?>? padding = null, WidgetStateProperty<Size?>? minimumSize = null, WidgetStateProperty<Size?>? fixedSize = null, WidgetStateProperty<Size?>? maximumSize = null, WidgetStateProperty<BorderSide?>? side = null, WidgetStateProperty<OutlinedBorder?>? shape = null, WidgetStateProperty<MouseCursor?>? mouseCursor = null, VisualDensity? visualDensity = null, AlignmentGeometry? alignment = null)
    {
        this.backgroundColor = backgroundColor;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.elevation = elevation;
        this.padding = padding;
        this.minimumSize = minimumSize;
        this.fixedSize = fixedSize;
        this.maximumSize = maximumSize;
        this.side = side;
        this.shape = shape;
        this.mouseCursor = mouseCursor;
        this.visualDensity = visualDensity;
        this.alignment = alignment;
    }

    public override int GetHashCode()
    {
        var values = new List<object?> { backgroundColor, shadowColor, surfaceTintColor, elevation, padding, minimumSize, fixedSize, maximumSize, side, shape, mouseCursor, visualDensity, alignment };
        return FoundationRuntimePorts.ObjectHashAll(values);
    }
    public override bool Equals(object? other)
    {
        var __other = other as MenuStyle;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is MenuStyle) && Equals(__other.backgroundColor, backgroundColor) && Equals(__other.shadowColor, shadowColor) && Equals(__other.surfaceTintColor, surfaceTintColor) && Equals(__other.elevation, elevation) && Equals(__other.padding, padding) && Equals(__other.minimumSize, minimumSize) && Equals(__other.fixedSize, fixedSize) && Equals(__other.maximumSize, maximumSize) && Equals(__other.side, side) && Equals(__other.shape, shape) && Equals(__other.mouseCursor, mouseCursor) && Equals(__other.visualDensity, visualDensity) && Equals(__other.alignment, alignment);
    }

    public virtual MenuStyle copyWith(WidgetStateProperty<Color?>? backgroundColor = null, WidgetStateProperty<Color?>? shadowColor = null, WidgetStateProperty<Color?>? surfaceTintColor = null, WidgetStateProperty<double?>? elevation = null, WidgetStateProperty<EdgeInsetsGeometry?>? padding = null, WidgetStateProperty<Size?>? minimumSize = null, WidgetStateProperty<Size?>? fixedSize = null, WidgetStateProperty<Size?>? maximumSize = null, WidgetStateProperty<BorderSide?>? side = null, WidgetStateProperty<OutlinedBorder?>? shape = null, WidgetStateProperty<MouseCursor?>? mouseCursor = null, VisualDensity? visualDensity = null, AlignmentGeometry? alignment = null)
    {
        return new MenuStyle(backgroundColor: backgroundColor ?? this.backgroundColor, shadowColor: shadowColor ?? this.shadowColor, surfaceTintColor: surfaceTintColor ?? this.surfaceTintColor, elevation: elevation ?? this.elevation, padding: padding ?? this.padding, minimumSize: minimumSize ?? this.minimumSize, fixedSize: fixedSize ?? this.fixedSize, maximumSize: maximumSize ?? this.maximumSize, side: side ?? this.side, shape: shape ?? this.shape, mouseCursor: mouseCursor ?? this.mouseCursor, visualDensity: visualDensity ?? this.visualDensity, alignment: alignment ?? this.alignment);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual MenuStyle merge(MenuStyle? style)
    {
        if (style is null)
        {
            return this;
        }
        return copyWith(backgroundColor: backgroundColor ?? style.backgroundColor, shadowColor: shadowColor ?? style.shadowColor, surfaceTintColor: surfaceTintColor ?? style.surfaceTintColor, elevation: elevation ?? style.elevation, padding: padding ?? style.padding, minimumSize: minimumSize ?? style.minimumSize, fixedSize: fixedSize ?? style.fixedSize, maximumSize: maximumSize ?? style.maximumSize, side: side ?? style.side, shape: shape ?? style.shape, mouseCursor: mouseCursor ?? style.mouseCursor, visualDensity: visualDensity ?? style.visualDensity, alignment: alignment ?? style.alignment);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static MenuStyle? lerp(MenuStyle? a, MenuStyle? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new MenuStyle(backgroundColor: WidgetStateProperty.lerp(a?.backgroundColor, b?.backgroundColor, t, Color.lerp), shadowColor: WidgetStateProperty.lerp(a?.shadowColor, b?.shadowColor, t, Color.lerp), surfaceTintColor: WidgetStateProperty.lerp(a?.surfaceTintColor, b?.surfaceTintColor, t, Color.lerp), elevation: WidgetStateProperty.lerp(a?.elevation, b?.elevation, t, Dart_uiLibrary.lerpDouble), padding: WidgetStateProperty.lerp(a?.padding, b?.padding, t, EdgeInsetsGeometry.lerp), minimumSize: WidgetStateProperty.lerp(a?.minimumSize, b?.minimumSize, t, Size.lerp), fixedSize: WidgetStateProperty.lerp(a?.fixedSize, b?.fixedSize, t, Size.lerp), maximumSize: WidgetStateProperty.lerp(a?.maximumSize, b?.maximumSize, t, Size.lerp), side: WidgetStateBorderSide.lerp(a?.side, b?.side, t), shape: WidgetStateProperty.lerp(a?.shape, b?.shape, t, OutlinedBorder.lerp), mouseCursor: (t < 0.5) ? a?.mouseCursor : b?.mouseCursor, visualDensity: (t < 0.5) ? a?.visualDensity : b?.visualDensity, alignment: AlignmentGeometry.lerp(a?.alignment, b?.alignment, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(new DiagnosticsProperty<WidgetStateProperty<Color?>>("backgroundColor", backgroundColor, defaultValue: null));
        properties.add(new DiagnosticsProperty<WidgetStateProperty<Color?>>("shadowColor", shadowColor, defaultValue: null));
        properties.add(new DiagnosticsProperty<WidgetStateProperty<Color?>>("surfaceTintColor", surfaceTintColor, defaultValue: null));
        properties.add(new DiagnosticsProperty<WidgetStateProperty<double?>>("elevation", elevation, defaultValue: null));
        properties.add(new DiagnosticsProperty<WidgetStateProperty<EdgeInsetsGeometry?>>("padding", padding, defaultValue: null));
        properties.add(new DiagnosticsProperty<WidgetStateProperty<Size?>>("minimumSize", minimumSize, defaultValue: null));
        properties.add(new DiagnosticsProperty<WidgetStateProperty<Size?>>("fixedSize", fixedSize, defaultValue: null));
        properties.add(new DiagnosticsProperty<WidgetStateProperty<Size?>>("maximumSize", maximumSize, defaultValue: null));
        properties.add(new DiagnosticsProperty<WidgetStateProperty<BorderSide?>>("side", side, defaultValue: null));
        properties.add(new DiagnosticsProperty<WidgetStateProperty<OutlinedBorder?>>("shape", shape, defaultValue: null));
        properties.add(new DiagnosticsProperty<WidgetStateProperty<MouseCursor?>>("mouseCursor", mouseCursor, defaultValue: null));
        properties.add(new DiagnosticsProperty<VisualDensity>("visualDensity", visualDensity, defaultValue: null));
        properties.add(new DiagnosticsProperty<AlignmentGeometry>("alignment", alignment, defaultValue: null));
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);
    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
