// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/bottom_sheet_theme.dart
using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public class BottomSheetThemeData : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? modalBackgroundColor { get; private set; }
    public virtual Color? modalBarrierColor { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual double? modalElevation { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual bool? showDragHandle { get; private set; }
    public virtual Color? dragHandleColor { get; private set; }
    public virtual Size? dragHandleSize { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints { get; private set; }

    public BottomSheetThemeData(Color? backgroundColor = null, Color? surfaceTintColor = null, double? elevation = null, Color? modalBackgroundColor = null, Color? modalBarrierColor = null, Color? shadowColor = null, double? modalElevation = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, bool? showDragHandle = null, Color? dragHandleColor = null, Size? dragHandleSize = null, Clip? clipBehavior = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null)
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

    public virtual BottomSheetThemeData copyWith(Color? backgroundColor = null, Color? surfaceTintColor = null, double? elevation = null, Color? modalBackgroundColor = null, Color? modalBarrierColor = null, Color? shadowColor = null, double? modalElevation = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, bool? showDragHandle = null, Color? dragHandleColor = null, Size? dragHandleSize = null, Clip? clipBehavior = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null)
    {
        return new BottomSheetThemeData(backgroundColor: (backgroundColor ?? this.backgroundColor), surfaceTintColor: (surfaceTintColor ?? this.surfaceTintColor), elevation: (elevation ?? this.elevation), modalBackgroundColor: (modalBackgroundColor ?? this.modalBackgroundColor), modalBarrierColor: (modalBarrierColor ?? this.modalBarrierColor), shadowColor: (shadowColor ?? this.shadowColor), modalElevation: (modalElevation ?? this.modalElevation), shape: (shape ?? this.shape), showDragHandle: (showDragHandle ?? this.showDragHandle), dragHandleColor: (dragHandleColor ?? this.dragHandleColor), dragHandleSize: (dragHandleSize ?? this.dragHandleSize), clipBehavior: (clipBehavior ?? this.clipBehavior), constraints: (constraints ?? this.constraints));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static BottomSheetThemeData? lerp(BottomSheetThemeData? a, BottomSheetThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new BottomSheetThemeData(backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), surfaceTintColor: Dart_uiLibrary.Color.lerp(a?.surfaceTintColor, b?.surfaceTintColor, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), modalBackgroundColor: Dart_uiLibrary.Color.lerp(a?.modalBackgroundColor, b?.modalBackgroundColor, t), modalBarrierColor: Dart_uiLibrary.Color.lerp(a?.modalBarrierColor, b?.modalBarrierColor, t), shadowColor: Dart_uiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t), modalElevation: Dart_uiLibrary.lerpDouble(a?.modalElevation, b?.modalElevation, t), shape: ShapeBorder.lerp(a?.shape, b?.shape, t), showDragHandle: ((t < 0.5) ? a?.showDragHandle : b?.showDragHandle), dragHandleColor: Dart_uiLibrary.Color.lerp(a?.dragHandleColor, b?.dragHandleColor, t), dragHandleSize: Dart_uiLibrary.Size.lerp(a?.dragHandleSize, b?.dragHandleSize, t), clipBehavior: ((t < 0.5) ? a?.clipBehavior : b?.clipBehavior), constraints: BoxConstraints.lerp(a?.constraints, b?.constraints, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.backgroundColor, this.surfaceTintColor, this.elevation, this.modalBackgroundColor, this.modalBarrierColor, this.shadowColor, this.modalElevation, this.shape, this.showDragHandle, this.dragHandleColor, this.dragHandleSize, this.clipBehavior, this.constraints));
    public override bool Equals(object? other)
    {
        var __other = other as BottomSheetThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((((((((__other is BottomSheetThemeData) && (object.Equals(((BottomSheetThemeData)((BottomSheetThemeData)__other)).backgroundColor, this.backgroundColor))) && (object.Equals(((BottomSheetThemeData)((BottomSheetThemeData)__other)).surfaceTintColor, this.surfaceTintColor))) && (((BottomSheetThemeData)((BottomSheetThemeData)__other)).elevation == this.elevation)) && (object.Equals(((BottomSheetThemeData)((BottomSheetThemeData)__other)).modalBackgroundColor, this.modalBackgroundColor))) && (object.Equals(((BottomSheetThemeData)((BottomSheetThemeData)__other)).shadowColor, this.shadowColor))) && (object.Equals(((BottomSheetThemeData)((BottomSheetThemeData)__other)).modalBarrierColor, this.modalBarrierColor))) && (((BottomSheetThemeData)((BottomSheetThemeData)__other)).modalElevation == this.modalElevation)) && (object.Equals(((BottomSheetThemeData)((BottomSheetThemeData)__other)).shape, this.shape))) && (((BottomSheetThemeData)((BottomSheetThemeData)__other)).showDragHandle == this.showDragHandle)) && (object.Equals(((BottomSheetThemeData)((BottomSheetThemeData)__other)).dragHandleColor, this.dragHandleColor))) && (object.Equals(((BottomSheetThemeData)((BottomSheetThemeData)__other)).dragHandleSize, this.dragHandleSize))) && (object.Equals(((BottomSheetThemeData)((BottomSheetThemeData)__other)).clipBehavior, this.clipBehavior))) && (object.Equals(((BottomSheetThemeData)((BottomSheetThemeData)__other)).constraints, this.constraints)));
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("backgroundColor", this.backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("surfaceTintColor", this.surfaceTintColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("elevation", this.elevation, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("modalBackgroundColor", this.modalBackgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("shadowColor", this.shadowColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("modalBarrierColor", this.modalBarrierColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("modalElevation", this.modalElevation, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.ShapeBorder>("shape", this.shape, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("showDragHandle", this.showDragHandle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("dragHandleColor", this.dragHandleColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Size>("dragHandleSize", this.dragHandleSize, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Clip>("clipBehavior", this.clipBehavior, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Rendering.BoxConstraints>("constraints", this.constraints, defaultValue: null));
    }

    public virtual string toStringShort() => global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return ((fullString__105654 ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
