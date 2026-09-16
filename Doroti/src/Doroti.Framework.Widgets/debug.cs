// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/debug.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public static partial class DebugLibrary
{
    public static bool debugPrintRebuildDirtyWidgets = false;
}

public delegate void RebuildDirtyWidgetCallback(Element e, bool builtOnce);

public static partial class DebugLibrary
{
    public static Action<Element, bool>? debugOnRebuildDirtyWidget;
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
    internal static Key? _firstNonUniqueKey(IEnumerable<Widget> widgets)
    {
        HashSet<Key> keySet = new HashSet<Key>();
        foreach (var widget in widgets)
        {
            if (widget.key is null)
            {
                continue;
            }
            if (!keySet.Add(widget.key!))
            {
                return widget.key;
            }
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class DebugLibrary
{
    public static bool debugChildrenHaveDuplicateKeys(Widget parent, IEnumerable<Widget> children, string? message = null)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                Key? nonUniqueKey = _firstNonUniqueKey(children.Cast<Widget>());
                if (nonUniqueKey is not null)
                {
                    throw DartRuntimePrimitives.AsException(FlutterError.Create($"{message ?? "Duplicate keys found.\n" + "If multiple keyed widgets exist as children of another widget, they must have unique keys."}" + $"\n{parent} has multiple children with key {nonUniqueKey}."));
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
                Key? nonUniqueKey = _firstNonUniqueKey(items.Cast<Widget>());
                if (nonUniqueKey is not null)
                {
                    throw DartRuntimePrimitives.AsException(FlutterError.Create($"Duplicate key found: {nonUniqueKey}."));
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
                if ((context.widget is not Table) && (context.findAncestorWidgetOfExactType<Table>() is null))
                {
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("No Table widget found."), new ErrorDescription($"{DartRuntimePrimitives.RuntimeType(context.widget)} widgets require a Table widget ancestor."), context.describeWidget("The specific widget that could not find a Table ancestor was"), context.describeOwnershipChain("The ownership chain for the affected widget is") }));
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
                if ((context.widget is not MediaQuery) && (context.getElementForInheritedWidgetOfExactType<MediaQuery>() is null))
                {
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("No MediaQuery widget ancestor found."), new ErrorDescription($"{DartRuntimePrimitives.RuntimeType(context.widget)} widgets require a MediaQuery widget ancestor."), context.describeWidget("The specific widget that could not find a MediaQuery ancestor was"), context.describeOwnershipChain("The ownership chain for the affected widget is"), new ErrorHint("No MediaQuery ancestor could be found starting from the context " + "that was passed to MediaQuery.of(). This can happen because the " + "context used is not a descendant of a View widget, which introduces " + "a MediaQuery.") }));
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
                if ((context.widget is not Directionality) && (context.getElementForInheritedWidgetOfExactType<Directionality>() is null))
                {
                    why = (why is null) ? "" : $" {why}";
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("No Directionality widget found."), new ErrorDescription($"{DartRuntimePrimitives.RuntimeType(context.widget)} widgets require a Directionality widget ancestor{why}.\n"), context.describeWidget("The specific widget that could not find a Directionality ancestor was"), context.describeOwnershipChain("The ownership chain for the affected widget is"), new ErrorHint("Typically, the Directionality widget is introduced by the MaterialApp " + "or WidgetsApp widget at the top of your application widget tree. It " + "determines the ambient reading direction and is used, for example, to " + "determine how to lay out text, how to interpret \"start\" and \"end\" " + "values, and to resolve EdgeInsetsDirectional, " + "AlignmentDirectional, and other *Directional objects.") }));
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
                if (built is null)
                {
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("A build function returned null."), new DiagnosticsProperty<Widget>("The offending widget is", widget, style: DiagnosticsTreeStyle.errorProperty), new ErrorDescription("Build functions must never return null."), new ErrorHint("To return an empty space that causes the building widget to fill available room, return \"Container()\". " + "To return an empty space that takes as little room as possible, return \"Container(width: 0.0, height: 0.0)\".") }));
                }
                if (Equals(widget, built))
                {
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("A build function returned context.widget."), new DiagnosticsProperty<Widget>("The offending widget is", widget, style: DiagnosticsTreeStyle.errorProperty), new ErrorDescription("Build functions must never return their BuildContext parameter's widget or a child that contains \"context.widget\". " + "Doing so introduces a loop in the widget tree that can cause the app to crash.") }));
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
                if (Localizations.of<WidgetsLocalizations>(context, typeof(WidgetsLocalizations)) is null)
                {
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("No WidgetsLocalizations found."), new ErrorDescription($"{DartRuntimePrimitives.RuntimeType(context.widget)} widgets require WidgetsLocalizations " + "to be provided by a Localizations widget ancestor."), new ErrorDescription("The widgets library uses Localizations to generate messages, " + "labels, and abbreviations."), new ErrorHint("To introduce a WidgetsLocalizations, either use a " + "WidgetsApp at the root of your application to include them " + "automatically, or add a Localization widget with a " + "WidgetsLocalizations delegate.") }));
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
                if (LookupBoundary.findAncestorWidgetOfExactType<Overlay>(context) is null)
                {
                    bool hiddenByBoundary = LookupBoundary.debugIsHidingAncestorWidgetOfExactType<Overlay>(context);
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"No Overlay widget found{(hiddenByBoundary ? " within the closest LookupBoundary" : "")}."), new ErrorDescription($"{DartRuntimePrimitives.RuntimeType(context.widget)} widgets require an Overlay " + "widget ancestor within the closest LookupBoundary.\n" + "An overlay lets widgets float on top of other widget children."), new ErrorHint("To introduce an Overlay widget, you can either directly " + "include one, or use a widget that contains an Overlay itself, " + "such as a Navigator, WidgetApp, MaterialApp, or CupertinoApp.") }));
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
                if (debugPrintRebuildDirtyWidgets || debugPrintBuildScope || debugPrintScheduleBuildForStacks || debugPrintGlobalKeyedWidgetLifecycle || debugProfileBuildsEnabled || debugHighlightDeprecatedWidgets || debugProfileBuildsEnabledUserWidgets || debugPaintFocusBoxes)
                {
                    throw DartRuntimePrimitives.AsException(FlutterError.Create(reason));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
