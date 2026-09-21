// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/dropdown_menu_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class DropdownMenuThemeData : Diagnosticable
{
    public virtual TextStyle? textStyle { get; private set; }
    internal virtual object? _inputDecorationTheme { get; private set; }
    public virtual MenuStyle? menuStyle { get; private set; }
    public virtual Color? disabledColor { get; private set; }

    public DropdownMenuThemeData(
        TextStyle? textStyle = null,
        object? inputDecorationTheme = null,
        MenuStyle? menuStyle = null,
        Color? disabledColor = null
    )
    {
        this.textStyle = textStyle;
        this.menuStyle = menuStyle;
        this.disabledColor = disabledColor;
        _inputDecorationTheme = inputDecorationTheme;
        System.Diagnostics.Debug.Assert(
            (inputDecorationTheme is null)
                || (inputDecorationTheme is InputDecorationTheme)
                || (inputDecorationTheme is InputDecorationThemeData)
        );
    }

    public virtual InputDecorationThemeData? inputDecorationTheme
    {
        get
        {
            if (_inputDecorationTheme is null)
            {
                return null;
            }
            return DartRuntimePrimitives.ConvertValue<InputDecorationThemeData>(
                _inputDecorationTheme
            );
        }
    }

    public virtual DropdownMenuThemeData copyWith(
        TextStyle? textStyle = null,
        object? inputDecorationTheme = null,
        MenuStyle? menuStyle = null,
        Color? disabledColor = null
    )
    {
        return new DropdownMenuThemeData(
            textStyle: textStyle ?? this.textStyle,
            inputDecorationTheme: inputDecorationTheme ?? this.inputDecorationTheme,
            menuStyle: menuStyle ?? this.menuStyle,
            disabledColor: disabledColor ?? this.disabledColor
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DropdownMenuThemeData lerp(
        DropdownMenuThemeData? a,
        DropdownMenuThemeData? b,
        double t
    )
    {
        if (DartRuntimePrimitives.Identical(a, b) && (a is not null))
        {
            return a;
        }
        return new DropdownMenuThemeData(
            textStyle: TextStyle.lerp(a?.textStyle, b?.textStyle, t),
            inputDecorationTheme: (t < 0.5) ? a?.inputDecorationTheme : b?.inputDecorationTheme,
            menuStyle: MenuStyle.lerp(a?.menuStyle, b?.menuStyle, t),
            disabledColor: Dart_uiLibrary.Color.lerp(a?.disabledColor, b?.disabledColor, t)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(
                textStyle,
                inputDecorationTheme,
                menuStyle,
                disabledColor
            )
        );

    public override bool Equals(object? other)
    {
        var __other = other as DropdownMenuThemeData;
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
        return (__other is DropdownMenuThemeData)
            && Equals(__other.textStyle, textStyle)
            && Equals(__other.inputDecorationTheme, inputDecorationTheme)
            && Equals(__other.menuStyle, menuStyle)
            && Equals(__other.disabledColor, disabledColor);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(
            new DiagnosticsProperty<TextStyle>("textStyle", textStyle, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<InputDecorationThemeData>(
                "inputDecorationThemeData",
                inputDecorationTheme,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<MenuStyle>("menuStyle", menuStyle, defaultValue: null)
        );
        properties.add(new ColorProperty("disabledColor", disabledColor, defaultValue: null));
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
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(
        string? name = null,
        DiagnosticsTreeStyle? style = null
    )
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class DropdownMenuTheme : InheritedTheme
{
    public virtual DropdownMenuThemeData data { get; private set; } = default!;

    public DropdownMenuTheme(
        Key? key = null,
        DropdownMenuThemeData data = default!,
        Widget child = default!
    )
        : base(key: key, child: child)
    {
        this.data = data;
    }

    public static DropdownMenuThemeData of(BuildContext context)
    {
        return maybeOf(context) ?? Theme.of(context).dropdownMenuTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DropdownMenuThemeData? maybeOf(BuildContext context)
    {
        return context.dependOnInheritedWidgetOfExactType<DropdownMenuTheme>()?.data;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return new DropdownMenuTheme(data: data, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        DartRuntimePrimitives.ConvertValue<bool>(
            !Equals(data, ((DropdownMenuTheme)oldWidget).data)
        );
}
