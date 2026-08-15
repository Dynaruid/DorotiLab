// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/action_icons_theme.dart
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

public class ActionIconThemeData : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>? backButtonIconBuilder { get; private set; }
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>? closeButtonIconBuilder { get; private set; }
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>? drawerButtonIconBuilder { get; private set; }
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>? endDrawerButtonIconBuilder { get; private set; }

    public ActionIconThemeData(global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>? backButtonIconBuilder = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>? closeButtonIconBuilder = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>? drawerButtonIconBuilder = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>? endDrawerButtonIconBuilder = null)
    {
        this.backButtonIconBuilder = backButtonIconBuilder;
        this.closeButtonIconBuilder = closeButtonIconBuilder;
        this.drawerButtonIconBuilder = drawerButtonIconBuilder;
        this.endDrawerButtonIconBuilder = endDrawerButtonIconBuilder;
    }

    public virtual ActionIconThemeData copyWith(global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>? backButtonIconBuilder = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>? closeButtonIconBuilder = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>? drawerButtonIconBuilder = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>? endDrawerButtonIconBuilder = null)
    {
        return new ActionIconThemeData(backButtonIconBuilder: ((backButtonIconBuilder ?? (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>)this.backButtonIconBuilder)), closeButtonIconBuilder: ((closeButtonIconBuilder ?? (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>)this.closeButtonIconBuilder)), drawerButtonIconBuilder: ((drawerButtonIconBuilder ?? (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>)this.drawerButtonIconBuilder)), endDrawerButtonIconBuilder: ((endDrawerButtonIconBuilder ?? (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>)this.endDrawerButtonIconBuilder)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ActionIconThemeData? lerp(ActionIconThemeData? a, ActionIconThemeData? b, double t)
    {
        if (((a is null) && (b is null)))
        {
            return null;
        }
        return new ActionIconThemeData(backButtonIconBuilder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>)((t < 0.5) ? a?.backButtonIconBuilder : b?.backButtonIconBuilder)), closeButtonIconBuilder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>)((t < 0.5) ? a?.closeButtonIconBuilder : b?.closeButtonIconBuilder)), drawerButtonIconBuilder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>)((t < 0.5) ? a?.drawerButtonIconBuilder : b?.drawerButtonIconBuilder)), endDrawerButtonIconBuilder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>)((t < 0.5) ? a?.endDrawerButtonIconBuilder : b?.endDrawerButtonIconBuilder)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode()
    {
        var values__3331 = new List<object?> { this.backButtonIconBuilder, this.closeButtonIconBuilder, this.drawerButtonIconBuilder, this.endDrawerButtonIconBuilder };
        return FoundationRuntimePorts.ObjectHashAll(values__3331);
        return default!;
    }
    public override bool Equals(object? other)
    {
        var __other = other as ActionIconThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((__other is ActionIconThemeData) && (object.Equals((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>?)((ActionIconThemeData)((ActionIconThemeData)__other)).backButtonIconBuilder, (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>?)this.backButtonIconBuilder))) && (object.Equals((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>?)((ActionIconThemeData)((ActionIconThemeData)__other)).closeButtonIconBuilder, (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>?)this.closeButtonIconBuilder))) && (object.Equals((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>?)((ActionIconThemeData)((ActionIconThemeData)__other)).drawerButtonIconBuilder, (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>?)this.drawerButtonIconBuilder))) && (object.Equals((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>?)((ActionIconThemeData)((ActionIconThemeData)__other)).endDrawerButtonIconBuilder, (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>?)this.endDrawerButtonIconBuilder)));
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>>("backButtonIconBuilder", this.backButtonIconBuilder, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>>("closeButtonIconBuilder", this.closeButtonIconBuilder, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>>("drawerButtonIconBuilder", this.drawerButtonIconBuilder, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>>("endDrawerButtonIconBuilder", this.endDrawerButtonIconBuilder, defaultValue: null));
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

public class ActionIconTheme : global::Doroti.Generated.Framework.Widgets.InheritedTheme
{
    public virtual ActionIconThemeData data { get; private set; } = default!;

    public ActionIconTheme(global::Doroti.Generated.Framework.Foundation.Key? key = null, ActionIconThemeData data = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static ActionIconThemeData? of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ActionIconTheme? actionIconTheme__6164 = ((ActionIconTheme?)(object?)context.dependOnInheritedWidgetOfExactType<ActionIconTheme>());
        return (actionIconTheme__6164?.data ?? Theme.of(context).actionIconTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget wrap(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new ActionIconTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((ActionIconTheme)oldWidget).data)));
}
