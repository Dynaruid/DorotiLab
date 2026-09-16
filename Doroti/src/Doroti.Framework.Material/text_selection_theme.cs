// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/text_selection_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

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
        return new TextSelectionThemeData(cursorColor: cursorColor ?? this.cursorColor, selectionColor: selectionColor ?? this.selectionColor, selectionHandleColor: selectionHandleColor ?? this.selectionHandleColor);
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

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(cursorColor, selectionColor, selectionHandleColor));
    public override bool Equals(object? other)
    {
        var __other = other as TextSelectionThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is TextSelectionThemeData) && Equals(__other.cursorColor, cursorColor) && Equals(__other.selectionColor, selectionColor) && Equals(__other.selectionHandleColor, selectionHandleColor);
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("cursorColor", cursorColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("selectionColor", selectionColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("selectionHandleColor", selectionHandleColor, defaultValue: null));
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

public class TextSelectionTheme : global::Doroti.Framework.Widgets.InheritedTheme
{
    public virtual TextSelectionThemeData data { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Widgets.Widget _child { get; private set; } = default!;

    public TextSelectionTheme(global::Doroti.Framework.Foundation.Key? key = null, TextSelectionThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: new _NullWidget__text_selection_theme())
    {
        this.data = data;
        _child = child;
    }

    public override global::Doroti.Framework.Widgets.Widget child
    {
        get
        {
            return new global::Doroti.Framework.Widgets.DefaultSelectionStyle(selectionColor: data.selectionColor, cursorColor: data.cursorColor, child: _child);
        }
    }
    public static TextSelectionThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        TextSelectionTheme? selectionTheme = context.dependOnInheritedWidgetOfExactType<TextSelectionTheme>();
        return selectionTheme?.data ?? Theme.of(context).textSelectionTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return new TextSelectionTheme(data: data, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((TextSelectionTheme)oldWidget).data));
}

internal class _NullWidget__text_selection_theme : global::Doroti.Framework.Widgets.Widget
{
    internal _NullWidget__text_selection_theme()
    {
    }

    public override global::Doroti.Framework.Widgets.Element createElement() => throw new NotImplementedException();
}
