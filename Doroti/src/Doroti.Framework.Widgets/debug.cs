// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/debug.dart
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

public static partial class DebugLibrary
{
    public static bool debugPrintRebuildDirtyWidgets = false;
}

public delegate void RebuildDirtyWidgetCallback(Element e, bool builtOnce);

public static partial class DebugLibrary
{
    public static global::System.Action<Element, bool>? debugOnRebuildDirtyWidget;
}

public static partial class DebugLibrary
{
    public static bool debugPrintBuildScope = false;
}

public static partial class DebugLibrary
{
    public static bool debugPrintScheduleBuildForStacks = false;
}

public static partial class DebugLibrary
{
    public static bool debugPrintGlobalKeyedWidgetLifecycle = false;
}

public static partial class DebugLibrary
{
    public static bool debugProfileBuildsEnabled = false;
}

public static partial class DebugLibrary
{
    public static bool debugProfileBuildsEnabledUserWidgets = false;
}

public static partial class DebugLibrary
{
    public static bool debugEnhanceBuildTimelineArguments = false;
}

public static partial class DebugLibrary
{
    public static bool debugHighlightDeprecatedWidgets = false;
}

public static partial class DebugLibrary
{
    public static bool debugPaintFocusBoxes = false;
}

public static partial class DebugLibrary
{
    internal static global::Doroti.Framework.Foundation.Key? _firstNonUniqueKey(IEnumerable<Widget> widgets)
    {
        HashSet<global::Doroti.Framework.Foundation.Key> keySet__8644 = new HashSet<global::Doroti.Framework.Foundation.Key>();
        foreach (var widget__8682 in widgets)
        {
            if ((((Widget)widget__8682).key is null))
            {
                continue;
            }
            if (!keySet__8644.Add(((Widget)widget__8682).key!))
            {
                return ((Widget)widget__8682).key;
            }
        }
        return ((global::Doroti.Framework.Foundation.Key)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class DebugLibrary
{
    public static bool debugChildrenHaveDuplicateKeys(Widget parent, IEnumerable<Widget> children, string? message = null)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                global::Doroti.Framework.Foundation.Key? nonUniqueKey__9671 = DebugLibrary._firstNonUniqueKey(children.Cast<Widget>());
                if ((nonUniqueKey__9671 is not null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create($"{(message ?? "Duplicate keys found.\n" + "If multiple keyed widgets exist as children of another widget, they must have unique keys.")}" + $"\n{parent} has multiple children with key {nonUniqueKey__9671}."));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class DebugLibrary
{
    public static bool debugItemsHaveDuplicateKeys(IEnumerable<Widget> items)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                global::Doroti.Framework.Foundation.Key? nonUniqueKey__10571 = DebugLibrary._firstNonUniqueKey(items.Cast<Widget>());
                if ((nonUniqueKey__10571 is not null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create($"Duplicate key found: {nonUniqueKey__10571}."));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class DebugLibrary
{
    public static bool debugCheckHasTable(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (((((BuildContext)context).widget is not Table) && (context.findAncestorWidgetOfExactType<Table>() is null)))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("No Table widget found."), new global::Doroti.Framework.Foundation.ErrorDescription($"{DartRuntimePrimitives.RuntimeType(((BuildContext)context).widget)} widgets require a Table widget ancestor."), context.describeWidget("The specific widget that could not find a Table ancestor was"), context.describeOwnershipChain("The ownership chain for the affected widget is") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class DebugLibrary
{
    public static bool debugCheckHasMediaQuery(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (((((BuildContext)context).widget is not MediaQuery) && (context.getElementForInheritedWidgetOfExactType<MediaQuery>() is null)))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("No MediaQuery widget ancestor found."), new global::Doroti.Framework.Foundation.ErrorDescription($"{DartRuntimePrimitives.RuntimeType(((BuildContext)context).widget)} widgets require a MediaQuery widget ancestor."), context.describeWidget("The specific widget that could not find a MediaQuery ancestor was"), context.describeOwnershipChain("The ownership chain for the affected widget is"), new global::Doroti.Framework.Foundation.ErrorHint("No MediaQuery ancestor could be found starting from the context " + "that was passed to MediaQuery.of(). This can happen because the " + "context used is not a descendant of a View widget, which introduces " + "a MediaQuery.") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class DebugLibrary
{
    public static bool debugCheckHasDirectionality(BuildContext context, string? why = null, string? hint = null, string? alternative = null)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (((((BuildContext)context).widget is not Directionality) && (context.getElementForInheritedWidgetOfExactType<Directionality>() is null)))
                {
                    why = ((why is null) ? "" : $" {why}");
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("No Directionality widget found."), new global::Doroti.Framework.Foundation.ErrorDescription($"{DartRuntimePrimitives.RuntimeType(((BuildContext)context).widget)} widgets require a Directionality widget ancestor{why}.\n"), context.describeWidget("The specific widget that could not find a Directionality ancestor was"), context.describeOwnershipChain("The ownership chain for the affected widget is"), new global::Doroti.Framework.Foundation.ErrorHint("Typically, the Directionality widget is introduced by the MaterialApp " + "or WidgetsApp widget at the top of your application widget tree. It " + "determines the ambient reading direction and is used, for example, to " + "determine how to lay out text, how to interpret \"start\" and \"end\" " + "values, and to resolve EdgeInsetsDirectional, " + "AlignmentDirectional, and other *Directional objects.") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class DebugLibrary
{
    public static void debugWidgetBuilderValue(Widget widget, Widget? built)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((built is null))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("A build function returned null."), new global::Doroti.Framework.Foundation.DiagnosticsProperty<Widget>("The offending widget is", widget, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty), new global::Doroti.Framework.Foundation.ErrorDescription("Build functions must never return null."), new global::Doroti.Framework.Foundation.ErrorHint("To return an empty space that causes the building widget to fill available room, return \"Container()\". " + "To return an empty space that takes as little room as possible, return \"Container(width: 0.0, height: 0.0)\".") }));
                }
                if ((object.Equals(widget, built)))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("A build function returned context.widget."), new global::Doroti.Framework.Foundation.DiagnosticsProperty<Widget>("The offending widget is", widget, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty), new global::Doroti.Framework.Foundation.ErrorDescription("Build functions must never return their BuildContext parameter's widget or a child that contains \"context.widget\". " + "Doing so introduces a loop in the widget tree that can cause the app to crash.") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }
}

public static partial class DebugLibrary
{
    public static bool debugCheckHasWidgetsLocalizations(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((Localizations.of<WidgetsLocalizations>(context, typeof(WidgetsLocalizations)) is null))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("No WidgetsLocalizations found."), new global::Doroti.Framework.Foundation.ErrorDescription($"{DartRuntimePrimitives.RuntimeType(((BuildContext)context).widget)} widgets require WidgetsLocalizations " + "to be provided by a Localizations widget ancestor."), new global::Doroti.Framework.Foundation.ErrorDescription("The widgets library uses Localizations to generate messages, " + "labels, and abbreviations."), new global::Doroti.Framework.Foundation.ErrorHint("To introduce a WidgetsLocalizations, either use a " + "WidgetsApp at the root of your application to include them " + "automatically, or add a Localization widget with a " + "WidgetsLocalizations delegate.") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class DebugLibrary
{
    public static bool debugCheckHasOverlay(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((LookupBoundary.findAncestorWidgetOfExactType<Overlay>(context) is null))
                {
                    bool hiddenByBoundary__20534 = LookupBoundary.debugIsHidingAncestorWidgetOfExactType<Overlay>(context);
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"No Overlay widget found{(hiddenByBoundary__20534 ? " within the closest LookupBoundary" : "")}."), new global::Doroti.Framework.Foundation.ErrorDescription($"{DartRuntimePrimitives.RuntimeType(((BuildContext)context).widget)} widgets require an Overlay " + "widget ancestor within the closest LookupBoundary.\n" + "An overlay lets widgets float on top of other widget children."), new global::Doroti.Framework.Foundation.ErrorHint("To introduce an Overlay widget, you can either directly " + "include one, or use a widget that contains an Overlay itself, " + "such as a Navigator, WidgetApp, MaterialApp, or CupertinoApp.") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class DebugLibrary
{
    public static bool debugAssertAllWidgetVarsUnset(string reason)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((((((((DebugLibrary.debugPrintRebuildDirtyWidgets || DebugLibrary.debugPrintBuildScope) || DebugLibrary.debugPrintScheduleBuildForStacks) || DebugLibrary.debugPrintGlobalKeyedWidgetLifecycle) || DebugLibrary.debugProfileBuildsEnabled) || DebugLibrary.debugHighlightDeprecatedWidgets) || DebugLibrary.debugProfileBuildsEnabledUserWidgets) || DebugLibrary.debugPaintFocusBoxes))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create(reason));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
