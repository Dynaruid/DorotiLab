// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/action_icons_theme.dart

using Doroti.Runtime;

namespace Doroti.Framework.Material;

public class ActionIconThemeData : Diagnosticable
{
    public virtual Func<BuildContext, Widget>? backButtonIconBuilder { get; private set; }
    public virtual Func<BuildContext, Widget>? closeButtonIconBuilder { get; private set; }
    public virtual Func<BuildContext, Widget>? drawerButtonIconBuilder { get; private set; }
    public virtual Func<BuildContext, Widget>? endDrawerButtonIconBuilder { get; private set; }

    public ActionIconThemeData(
        Func<BuildContext, Widget>? backButtonIconBuilder = null,
        Func<BuildContext, Widget>? closeButtonIconBuilder = null,
        Func<BuildContext, Widget>? drawerButtonIconBuilder = null,
        Func<BuildContext, Widget>? endDrawerButtonIconBuilder = null
    )
    {
        this.backButtonIconBuilder = backButtonIconBuilder;
        this.closeButtonIconBuilder = closeButtonIconBuilder;
        this.drawerButtonIconBuilder = drawerButtonIconBuilder;
        this.endDrawerButtonIconBuilder = endDrawerButtonIconBuilder;
    }

    public virtual ActionIconThemeData copyWith(
        Func<BuildContext, Widget>? backButtonIconBuilder = null,
        Func<BuildContext, Widget>? closeButtonIconBuilder = null,
        Func<BuildContext, Widget>? drawerButtonIconBuilder = null,
        Func<BuildContext, Widget>? endDrawerButtonIconBuilder = null
    )
    {
        return new ActionIconThemeData(
            backButtonIconBuilder: backButtonIconBuilder ?? this.backButtonIconBuilder,
            closeButtonIconBuilder: closeButtonIconBuilder ?? this.closeButtonIconBuilder,
            drawerButtonIconBuilder: drawerButtonIconBuilder ?? this.drawerButtonIconBuilder,
            endDrawerButtonIconBuilder: endDrawerButtonIconBuilder
                ?? this.endDrawerButtonIconBuilder
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ActionIconThemeData? lerp(
        ActionIconThemeData? a,
        ActionIconThemeData? b,
        double t
    )
    {
        if ((a is null) && (b is null))
        {
            return null;
        }
        return new ActionIconThemeData(
            backButtonIconBuilder: (t < 0.5) ? a?.backButtonIconBuilder : b?.backButtonIconBuilder,
            closeButtonIconBuilder: (t < 0.5)
                ? a?.closeButtonIconBuilder
                : b?.closeButtonIconBuilder,
            drawerButtonIconBuilder: (t < 0.5)
                ? a?.drawerButtonIconBuilder
                : b?.drawerButtonIconBuilder,
            endDrawerButtonIconBuilder: (t < 0.5)
                ? a?.endDrawerButtonIconBuilder
                : b?.endDrawerButtonIconBuilder
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode()
    {
        var values = new List<object?>
        {
            backButtonIconBuilder,
            closeButtonIconBuilder,
            drawerButtonIconBuilder,
            endDrawerButtonIconBuilder,
        };
        return FoundationRuntimePorts.ObjectHashAll(values);
    }

    public override bool Equals(object? other)
    {
        var __other = other as ActionIconThemeData;
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
        return (__other is ActionIconThemeData)
            && Equals(__other.backButtonIconBuilder, backButtonIconBuilder)
            && Equals(__other.closeButtonIconBuilder, closeButtonIconBuilder)
            && Equals(__other.drawerButtonIconBuilder, drawerButtonIconBuilder)
            && Equals(__other.endDrawerButtonIconBuilder, endDrawerButtonIconBuilder);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(
            new DiagnosticsProperty<Func<BuildContext, Widget>>(
                "backButtonIconBuilder",
                backButtonIconBuilder,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<Func<BuildContext, Widget>>(
                "closeButtonIconBuilder",
                closeButtonIconBuilder,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<Func<BuildContext, Widget>>(
                "drawerButtonIconBuilder",
                drawerButtonIconBuilder,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<Func<BuildContext, Widget>>(
                "endDrawerButtonIconBuilder",
                endDrawerButtonIconBuilder,
                defaultValue: null
            )
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

public class ActionIconTheme : InheritedTheme
{
    public virtual ActionIconThemeData data { get; private set; } = default!;

    public ActionIconTheme(
        Key? key = null,
        ActionIconThemeData data = default!,
        Widget child = default!
    )
        : base(key: key, child: child)
    {
        this.data = data;
    }

    public static ActionIconThemeData? of(BuildContext context)
    {
        ActionIconTheme? actionIconThemeLocal =
            context.dependOnInheritedWidgetOfExactType<ActionIconTheme>();
        return actionIconThemeLocal?.data ?? Theme.of(context).actionIconTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return new ActionIconTheme(data: data, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((ActionIconTheme)oldWidget).data));
}
