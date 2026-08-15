// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/debug.dart
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

public static partial class DebugLibrary
{
    public static bool debugCheckHasMaterial(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((LookupBoundary.findAncestorWidgetOfExactType<Material>(context) is null))
                {
                    bool hiddenByBoundary__1276 = LookupBoundary.debugIsHidingAncestorWidgetOfExactType<Material>(context);
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(((Func<List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>>)(() => { var __collection1421 = new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>(); __collection1421.Add(new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"No Material widget found{(hiddenByBoundary__1276 ? " within the closest LookupBoundary" : "")}.")); if (hiddenByBoundary__1276) { __collection1421.Add(new global::Doroti.Generated.Framework.Foundation.ErrorDescription("There is an ancestor Material widget, but it is hidden by a LookupBoundary.")); } __collection1421.Add(new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{(DartRuntimePrimitives.RuntimeType(((global::Doroti.Generated.Framework.Widgets.BuildContext)context).widget))} widgets require a Material " + "widget ancestor within the closest LookupBoundary.\n" + "In Material Design, most widgets are conceptually \"printed\" on " + "a sheet of material. In Flutter's material library, that " + "material is represented by the Material widget. It is the " + "Material widget that renders ink splashes, for instance. " + "Because of this, many material library widgets require that " + "there be a Material widget in the tree above them.")); __collection1421.Add(new global::Doroti.Generated.Framework.Foundation.ErrorHint("To introduce a Material widget, you can either directly " + "include one, or use a widget that contains Material itself, " + "such as a Card, Dialog, Drawer, or Scaffold.")); __collection1421.AddRange(context.describeMissingAncestor(expectedAncestorType: typeof(Material))); return __collection1421; }))()));
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class DebugLibrary
{
    public static bool debugCheckHasMaterialLocalizations(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((Localizations.of<MaterialLocalizations>(context, typeof(MaterialLocalizations)) is null))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(((Func<List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>>)(() => { var __collection3880 = new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>(); __collection3880.Add(new global::Doroti.Generated.Framework.Foundation.ErrorSummary("No MaterialLocalizations found.")); __collection3880.Add(new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{(DartRuntimePrimitives.RuntimeType(((global::Doroti.Generated.Framework.Widgets.BuildContext)context).widget))} widgets require MaterialLocalizations " + "to be provided by a Localizations widget ancestor.")); __collection3880.Add(new global::Doroti.Generated.Framework.Foundation.ErrorDescription("The material library uses Localizations to generate messages, " + "labels, and abbreviations.")); __collection3880.Add(new global::Doroti.Generated.Framework.Foundation.ErrorHint("To introduce a MaterialLocalizations, either use a " + "MaterialApp at the root of your application to include them " + "automatically, or add a Localization widget with a " + "MaterialLocalizations delegate.")); __collection3880.AddRange(context.describeMissingAncestor(expectedAncestorType: typeof(MaterialLocalizations))); return __collection3880; }))()));
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class DebugLibrary
{
    public static bool debugCheckHasScaffold(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (((((global::Doroti.Generated.Framework.Widgets.BuildContext)context).widget is not Scaffold) && (context.findAncestorWidgetOfExactType<Scaffold>() is null)))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(((Func<List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>>)(() => { var __collection5564 = new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>(); __collection5564.Add(new global::Doroti.Generated.Framework.Foundation.ErrorSummary("No Scaffold widget found.")); __collection5564.Add(new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{(DartRuntimePrimitives.RuntimeType(((global::Doroti.Generated.Framework.Widgets.BuildContext)context).widget))} widgets require a Scaffold widget ancestor.")); __collection5564.AddRange(context.describeMissingAncestor(expectedAncestorType: typeof(Scaffold))); __collection5564.Add(new global::Doroti.Generated.Framework.Foundation.ErrorHint("Typically, the Scaffold widget is introduced by the MaterialApp or " + "WidgetsApp widget at the top of your application widget tree.")); return __collection5564; }))()));
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class DebugLibrary
{
    public static bool debugCheckHasScaffoldMessenger(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((context.findAncestorWidgetOfExactType<ScaffoldMessenger>() is null))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(((Func<List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>>)(() => { var __collection6933 = new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>(); __collection6933.Add(new global::Doroti.Generated.Framework.Foundation.ErrorSummary("No ScaffoldMessenger widget found.")); __collection6933.Add(new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{(DartRuntimePrimitives.RuntimeType(((global::Doroti.Generated.Framework.Widgets.BuildContext)context).widget))} widgets require a ScaffoldMessenger widget ancestor.")); __collection6933.AddRange(context.describeMissingAncestor(expectedAncestorType: typeof(ScaffoldMessenger))); __collection6933.Add(new global::Doroti.Generated.Framework.Foundation.ErrorHint("Typically, the ScaffoldMessenger widget is introduced by the MaterialApp " + "at the top of your application widget tree.")); return __collection6933; }))()));
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
