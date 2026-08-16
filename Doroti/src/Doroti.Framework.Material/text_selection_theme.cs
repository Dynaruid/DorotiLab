// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/text_selection_theme.dart
using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Material;

public class TextSelectionThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual Color? cursorColor { get; private set; }
    public virtual Color? selectionColor { get; private set; }
    public virtual Color? selectionHandleColor { get; private set; }

    public TextSelectionThemeData(Color? cursorColor = null, Color? selectionColor = null, Color? selectionHandleColor = null)
    {
        this.cursorColor = cursorColor;
        this.selectionColor = selectionColor;
        this.selectionHandleColor = selectionHandleColor;
    }

    public virtual TextSelectionThemeData copyWith(Color? cursorColor = null, Color? selectionColor = null, Color? selectionHandleColor = null)
    {
        return new TextSelectionThemeData(cursorColor: (cursorColor ?? this.cursorColor), selectionColor: (selectionColor ?? this.selectionColor), selectionHandleColor: (selectionHandleColor ?? this.selectionHandleColor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TextSelectionThemeData? lerp(TextSelectionThemeData? a, TextSelectionThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new TextSelectionThemeData(cursorColor: Dart_uiLibrary.Color.lerp(a?.cursorColor, b?.cursorColor, t), selectionColor: Dart_uiLibrary.Color.lerp(a?.selectionColor, b?.selectionColor, t), selectionHandleColor: Dart_uiLibrary.Color.lerp(a?.selectionHandleColor, b?.selectionHandleColor, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.cursorColor, this.selectionColor, this.selectionHandleColor));
    public override bool Equals(object? other)
    {
        var __other = other as TextSelectionThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((__other is TextSelectionThemeData) && (object.Equals(((TextSelectionThemeData)((TextSelectionThemeData)__other)).cursorColor, this.cursorColor))) && (object.Equals(((TextSelectionThemeData)((TextSelectionThemeData)__other)).selectionColor, this.selectionColor))) && (object.Equals(((TextSelectionThemeData)((TextSelectionThemeData)__other)).selectionHandleColor, this.selectionHandleColor)));
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("cursorColor", this.cursorColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("selectionColor", this.selectionColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("selectionHandleColor", this.selectionHandleColor, defaultValue: null));
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
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

public class TextSelectionTheme : global::Doroti.Framework.Widgets.InheritedTheme
{
    public virtual TextSelectionThemeData data { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Widgets.Widget _child { get; private set; } = default!;

    public TextSelectionTheme(global::Doroti.Framework.Foundation.Key? key = null, TextSelectionThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: new _NullWidget__text_selection_theme())
    {
        this.data = data;
        this._child = child;
    }

    public override global::Doroti.Framework.Widgets.Widget child
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.DefaultSelectionStyle(selectionColor: ((TextSelectionThemeData)this.data).selectionColor, cursorColor: ((TextSelectionThemeData)this.data).cursorColor, child: this._child));
            return default!;
        }
    }
    public static TextSelectionThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        TextSelectionTheme? selectionTheme__6349 = ((TextSelectionTheme?)(object?)context.dependOnInheritedWidgetOfExactType<TextSelectionTheme>());
        return (selectionTheme__6349?.data ?? Theme.of(context).textSelectionTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new TextSelectionTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((TextSelectionTheme)oldWidget).data)));
}

internal class _NullWidget__text_selection_theme : global::Doroti.Framework.Widgets.Widget
{
    internal _NullWidget__text_selection_theme()
    {
    }

    public override global::Doroti.Framework.Widgets.Element createElement() => throw new NotImplementedException();
}
