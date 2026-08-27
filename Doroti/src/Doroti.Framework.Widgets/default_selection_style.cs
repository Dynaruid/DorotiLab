// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/default_selection_style.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Widgets;

public class DefaultSelectionStyle : InheritedTheme
{
    public static Color defaultColor = new global::Doroti.Ui.Color(2155905152L);
    public virtual Color? cursorColor { get; private set; }
    public virtual Color? selectionColor { get; private set; }
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }

    public DefaultSelectionStyle(global::Doroti.Framework.Foundation.Key? key = null, Color? cursorColor = null, Color? selectionColor = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, Widget child = default!) : base(key: key, child: child)
    {
        this.cursorColor = cursorColor;
        this.selectionColor = selectionColor;
        this.mouseCursor = mouseCursor;
    }

    public static DefaultSelectionStyle CreateFallback(global::Doroti.Framework.Foundation.Key? key = null)
    {
        var __instance = new DefaultSelectionStyle(default!, default!, default!, default!, default!);
        __instance.cursorColor = null;
        __instance.selectionColor = null;
        __instance.mouseCursor = null;
        return __instance;
    }

    public static Widget merge(global::Doroti.Framework.Foundation.Key? key = null, Color? cursorColor = null, Color? selectionColor = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, Widget child = default!)
    {
        return ((Widget)(object?)new Builder(builder: ((global::System.Func<BuildContext, Widget>)((context) =>
        {
            DefaultSelectionStyle parent = ((DefaultSelectionStyle)(object?)DefaultSelectionStyle.of(context));
            return ((Widget)(object?)new DefaultSelectionStyle(key: key, cursorColor: (cursorColor ?? ((DefaultSelectionStyle)parent).cursorColor), selectionColor: (selectionColor ?? ((DefaultSelectionStyle)parent).selectionColor), mouseCursor: (mouseCursor ?? ((DefaultSelectionStyle)parent).mouseCursor), child: child));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DefaultSelectionStyle of(BuildContext context)
    {
        return (context.dependOnInheritedWidgetOfExactType<DefaultSelectionStyle>() ?? DefaultSelectionStyle.CreateFallback());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return ((Widget)(object?)new DefaultSelectionStyle(cursorColor: this.cursorColor, selectionColor: this.selectionColor, mouseCursor: this.mouseCursor, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (DefaultSelectionStyle)(object)oldWidget;
        return (((!object.Equals(this.cursorColor, ((DefaultSelectionStyle)__oldWidget).cursorColor)) || (!object.Equals(this.selectionColor, ((DefaultSelectionStyle)__oldWidget).selectionColor))) || (!object.Equals(this.mouseCursor, ((DefaultSelectionStyle)__oldWidget).mouseCursor)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _NullWidget__default_selection_style : StatelessWidget
{
    internal _NullWidget__default_selection_style()
    {
    }

    public override Widget build(BuildContext context)
    {
        throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("A DefaultSelectionStyle constructed with DefaultSelectionStyle.fallback cannot be incorporated into the widget tree, " + "it is meant only to provide a fallback value returned by DefaultSelectionStyle.of() " + "when no enclosing default selection style is present in a BuildContext."));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

