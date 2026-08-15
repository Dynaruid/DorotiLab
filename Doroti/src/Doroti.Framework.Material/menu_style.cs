// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/menu_style.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public class MenuStyle : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? backgroundColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? shadowColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? surfaceTintColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double?>? elevation { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?>? padding { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Size?>? minimumSize { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Size?>? fixedSize { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Size?>? maximumSize { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BorderSide?>? side { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder?>? shape { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment { get; private set; }

    public MenuStyle(global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? backgroundColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? shadowColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? surfaceTintColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double?>? elevation = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?>? padding = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Size?>? minimumSize = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Size?>? fixedSize = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Size?>? maximumSize = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BorderSide?>? side = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder?>? shape = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor = null, VisualDensity? visualDensity = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment = null)
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
        var values__7168 = new List<object?> { this.backgroundColor, this.shadowColor, this.surfaceTintColor, this.elevation, this.padding, this.minimumSize, this.fixedSize, this.maximumSize, this.side, this.shape, this.mouseCursor, this.visualDensity, this.alignment };
        return FoundationRuntimePorts.ObjectHashAll(values__7168);
        return default!;
    }
    public override bool Equals(object? other)
    {
        var __other = other as MenuStyle;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((((((((__other is MenuStyle) && (object.Equals(((MenuStyle)((MenuStyle)__other)).backgroundColor, this.backgroundColor))) && (object.Equals(((MenuStyle)((MenuStyle)__other)).shadowColor, this.shadowColor))) && (object.Equals(((MenuStyle)((MenuStyle)__other)).surfaceTintColor, this.surfaceTintColor))) && (object.Equals(((MenuStyle)((MenuStyle)__other)).elevation, this.elevation))) && (object.Equals(((MenuStyle)((MenuStyle)__other)).padding, this.padding))) && (object.Equals(((MenuStyle)((MenuStyle)__other)).minimumSize, this.minimumSize))) && (object.Equals(((MenuStyle)((MenuStyle)__other)).fixedSize, this.fixedSize))) && (object.Equals(((MenuStyle)((MenuStyle)__other)).maximumSize, this.maximumSize))) && (object.Equals(((MenuStyle)((MenuStyle)__other)).side, this.side))) && (object.Equals(((MenuStyle)((MenuStyle)__other)).shape, this.shape))) && (object.Equals(((MenuStyle)((MenuStyle)__other)).mouseCursor, this.mouseCursor))) && (object.Equals(((MenuStyle)((MenuStyle)__other)).visualDensity, this.visualDensity))) && (object.Equals(((MenuStyle)((MenuStyle)__other)).alignment, this.alignment)));
    }

    public virtual MenuStyle copyWith(global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? backgroundColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? shadowColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? surfaceTintColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double?>? elevation = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?>? padding = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Size?>? minimumSize = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Size?>? fixedSize = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Size?>? maximumSize = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BorderSide?>? side = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder?>? shape = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor = null, VisualDensity? visualDensity = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment = null)
    {
        return new MenuStyle(backgroundColor: (backgroundColor ?? this.backgroundColor), shadowColor: (shadowColor ?? this.shadowColor), surfaceTintColor: (surfaceTintColor ?? this.surfaceTintColor), elevation: (elevation ?? this.elevation), padding: (padding ?? this.padding), minimumSize: (minimumSize ?? this.minimumSize), fixedSize: (fixedSize ?? this.fixedSize), maximumSize: (maximumSize ?? this.maximumSize), side: (side ?? this.side), shape: (shape ?? this.shape), mouseCursor: (mouseCursor ?? this.mouseCursor), visualDensity: (visualDensity ?? this.visualDensity), alignment: (alignment ?? this.alignment));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual MenuStyle merge(MenuStyle? style)
    {
        if ((style is null))
        {
            return this;
        }
        return ((MenuStyle)(object?)copyWith(backgroundColor: (this.backgroundColor ?? ((MenuStyle)style).backgroundColor), shadowColor: (this.shadowColor ?? ((MenuStyle)style).shadowColor), surfaceTintColor: (this.surfaceTintColor ?? ((MenuStyle)style).surfaceTintColor), elevation: (this.elevation ?? ((MenuStyle)style).elevation), padding: (this.padding ?? ((MenuStyle)style).padding), minimumSize: (this.minimumSize ?? ((MenuStyle)style).minimumSize), fixedSize: (this.fixedSize ?? ((MenuStyle)style).fixedSize), maximumSize: (this.maximumSize ?? ((MenuStyle)style).maximumSize), side: (this.side ?? ((MenuStyle)style).side), shape: (this.shape ?? ((MenuStyle)style).shape), mouseCursor: (this.mouseCursor ?? ((MenuStyle)style).mouseCursor), visualDensity: (this.visualDensity ?? ((MenuStyle)style).visualDensity), alignment: (this.alignment ?? ((MenuStyle)style).alignment)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static MenuStyle? lerp(MenuStyle? a, MenuStyle? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new MenuStyle(backgroundColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.backgroundColor, b?.backgroundColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), shadowColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.shadowColor, b?.shadowColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), surfaceTintColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.surfaceTintColor, b?.surfaceTintColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), elevation: WidgetStateProperty.lerp<double?>(a?.elevation, b?.elevation, t, (global::System.Func<double?, double?, double, double?>)Dart_uiLibrary.lerpDouble), padding: WidgetStateProperty.lerp<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?>(a?.padding, b?.padding, t, (global::System.Func<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?, double, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?>)global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry.lerp), minimumSize: WidgetStateProperty.lerp<global::Doroti.Ui.Size?>(a?.minimumSize, b?.minimumSize, t, (global::System.Func<Size?, Size?, double, Size?>)Size.lerp), fixedSize: WidgetStateProperty.lerp<global::Doroti.Ui.Size?>(a?.fixedSize, b?.fixedSize, t, (global::System.Func<Size?, Size?, double, Size?>)Size.lerp), maximumSize: WidgetStateProperty.lerp<global::Doroti.Ui.Size?>(a?.maximumSize, b?.maximumSize, t, (global::System.Func<Size?, Size?, double, Size?>)Size.lerp), side: WidgetStateBorderSide.lerp(a?.side, b?.side, t), shape: WidgetStateProperty.lerp<global::Doroti.Generated.Framework.Painting.OutlinedBorder?>(a?.shape, b?.shape, t, (global::System.Func<global::Doroti.Generated.Framework.Painting.OutlinedBorder?, global::Doroti.Generated.Framework.Painting.OutlinedBorder?, double, global::Doroti.Generated.Framework.Painting.OutlinedBorder?>)global::Doroti.Generated.Framework.Painting.OutlinedBorder.lerp), mouseCursor: ((t < 0.5) ? a?.mouseCursor : b?.mouseCursor), visualDensity: ((t < 0.5) ? a?.visualDensity : b?.visualDensity), alignment: AlignmentGeometry.lerp(a?.alignment, b?.alignment, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("backgroundColor", this.backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("shadowColor", this.shadowColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("surfaceTintColor", this.surfaceTintColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double?>>("elevation", this.elevation, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?>>("padding", this.padding, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size?>>("minimumSize", this.minimumSize, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size?>>("fixedSize", this.fixedSize, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size?>>("maximumSize", this.maximumSize, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BorderSide?>>("side", this.side, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder?>>("shape", this.shape, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>>("mouseCursor", this.mouseCursor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<VisualDensity>("visualDensity", this.visualDensity, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.AlignmentGeometry>("alignment", this.alignment, defaultValue: null));
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
