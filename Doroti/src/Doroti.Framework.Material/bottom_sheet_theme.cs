// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/bottom_sheet_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class BottomSheetThemeData : Diagnosticable
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? modalBackgroundColor { get; private set; }
    public virtual Color? modalBarrierColor { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual double? modalElevation { get; private set; }
    public virtual ShapeBorder? shape { get; private set; }
    public virtual bool? showDragHandle { get; private set; }
    public virtual Color? dragHandleColor { get; private set; }
    public virtual Size? dragHandleSize { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }
    public virtual BoxConstraints? constraints { get; private set; }

    public BottomSheetThemeData(
        Color? backgroundColor = null,
        Color? surfaceTintColor = null,
        double? elevation = null,
        Color? modalBackgroundColor = null,
        Color? modalBarrierColor = null,
        Color? shadowColor = null,
        double? modalElevation = null,
        ShapeBorder? shape = null,
        bool? showDragHandle = null,
        Color? dragHandleColor = null,
        Size? dragHandleSize = null,
        Clip? clipBehavior = null,
        BoxConstraints? constraints = null
    )
    {
        this.backgroundColor = backgroundColor;
        this.surfaceTintColor = surfaceTintColor;
        this.elevation = elevation;
        this.modalBackgroundColor = modalBackgroundColor;
        this.modalBarrierColor = modalBarrierColor;
        this.shadowColor = shadowColor;
        this.modalElevation = modalElevation;
        this.shape = shape;
        this.showDragHandle = showDragHandle;
        this.dragHandleColor = dragHandleColor;
        this.dragHandleSize = dragHandleSize;
        this.clipBehavior = clipBehavior;
        this.constraints = constraints;
    }

    public virtual BottomSheetThemeData copyWith(
        Color? backgroundColor = null,
        Color? surfaceTintColor = null,
        double? elevation = null,
        Color? modalBackgroundColor = null,
        Color? modalBarrierColor = null,
        Color? shadowColor = null,
        double? modalElevation = null,
        ShapeBorder? shape = null,
        bool? showDragHandle = null,
        Color? dragHandleColor = null,
        Size? dragHandleSize = null,
        Clip? clipBehavior = null,
        BoxConstraints? constraints = null
    )
    {
        return new BottomSheetThemeData(
            backgroundColor: backgroundColor ?? this.backgroundColor,
            surfaceTintColor: surfaceTintColor ?? this.surfaceTintColor,
            elevation: elevation ?? this.elevation,
            modalBackgroundColor: modalBackgroundColor ?? this.modalBackgroundColor,
            modalBarrierColor: modalBarrierColor ?? this.modalBarrierColor,
            shadowColor: shadowColor ?? this.shadowColor,
            modalElevation: modalElevation ?? this.modalElevation,
            shape: shape ?? this.shape,
            showDragHandle: showDragHandle ?? this.showDragHandle,
            dragHandleColor: dragHandleColor ?? this.dragHandleColor,
            dragHandleSize: dragHandleSize ?? this.dragHandleSize,
            clipBehavior: clipBehavior ?? this.clipBehavior,
            constraints: constraints ?? this.constraints
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static BottomSheetThemeData? lerp(
        BottomSheetThemeData? a,
        BottomSheetThemeData? b,
        double t
    )
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new BottomSheetThemeData(
            backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t),
            surfaceTintColor: Dart_uiLibrary.Color.lerp(
                a?.surfaceTintColor,
                b?.surfaceTintColor,
                t
            ),
            elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t),
            modalBackgroundColor: Dart_uiLibrary.Color.lerp(
                a?.modalBackgroundColor,
                b?.modalBackgroundColor,
                t
            ),
            modalBarrierColor: Dart_uiLibrary.Color.lerp(
                a?.modalBarrierColor,
                b?.modalBarrierColor,
                t
            ),
            shadowColor: Dart_uiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t),
            modalElevation: Dart_uiLibrary.lerpDouble(a?.modalElevation, b?.modalElevation, t),
            shape: ShapeBorder.lerp(a?.shape, b?.shape, t),
            showDragHandle: (t < 0.5) ? a?.showDragHandle : b?.showDragHandle,
            dragHandleColor: Dart_uiLibrary.Color.lerp(a?.dragHandleColor, b?.dragHandleColor, t),
            dragHandleSize: Dart_uiLibrary.Size.lerp(a?.dragHandleSize, b?.dragHandleSize, t),
            clipBehavior: (t < 0.5) ? a?.clipBehavior : b?.clipBehavior,
            constraints: BoxConstraints.lerp(a?.constraints, b?.constraints, t)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(
                backgroundColor,
                surfaceTintColor,
                elevation,
                modalBackgroundColor,
                modalBarrierColor,
                shadowColor,
                modalElevation,
                shape,
                showDragHandle,
                dragHandleColor,
                dragHandleSize,
                clipBehavior,
                constraints
            )
        );

    public override bool Equals(object? other)
    {
        var __other = other as BottomSheetThemeData;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is BottomSheetThemeData)
            && Equals(__other.backgroundColor, backgroundColor)
            && Equals(__other.surfaceTintColor, surfaceTintColor)
            && (__other.elevation == elevation)
            && Equals(__other.modalBackgroundColor, modalBackgroundColor)
            && Equals(__other.shadowColor, shadowColor)
            && Equals(__other.modalBarrierColor, modalBarrierColor)
            && (__other.modalElevation == modalElevation)
            && Equals(__other.shape, shape)
            && (__other.showDragHandle == showDragHandle)
            && Equals(__other.dragHandleColor, dragHandleColor)
            && Equals(__other.dragHandleSize, dragHandleSize)
            && Equals(__other.clipBehavior, clipBehavior)
            && Equals(__other.constraints, constraints);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(new ColorProperty("backgroundColor", backgroundColor, defaultValue: null));
        properties.add(new ColorProperty("surfaceTintColor", surfaceTintColor, defaultValue: null));
        properties.add(new DoubleProperty("elevation", elevation, defaultValue: null));
        properties.add(
            new ColorProperty("modalBackgroundColor", modalBackgroundColor, defaultValue: null)
        );
        properties.add(new ColorProperty("shadowColor", shadowColor, defaultValue: null));
        properties.add(
            new ColorProperty("modalBarrierColor", modalBarrierColor, defaultValue: null)
        );
        properties.add(new DoubleProperty("modalElevation", modalElevation, defaultValue: null));
        properties.add(new DiagnosticsProperty<ShapeBorder>("shape", shape, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<bool>("showDragHandle", showDragHandle, defaultValue: null)
        );
        properties.add(new ColorProperty("dragHandleColor", dragHandleColor, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<Size>("dragHandleSize", dragHandleSize, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<Clip>("clipBehavior", clipBehavior, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<BoxConstraints>("constraints", constraints, defaultValue: null)
        );
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);

    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine)
                .toDiagnosticsNode()
                .toStringDeep(minLevel: minLevel);
            return true;
        });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(
        string? name = null,
        DiagnosticsTreeStyle? style = null
    )
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
