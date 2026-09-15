// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/debug.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Cupertino;

public static partial class DebugLibrary
{
    public static bool debugCheckHasCupertinoLocalizations(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((Localizations.of<CupertinoLocalizations>(context, typeof(CupertinoLocalizations)) is null))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(((Func<List<global::Doroti.Framework.Foundation.DiagnosticsNode>>)(() => { var __collection1052 = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>(); __collection1052.Add(new global::Doroti.Framework.Foundation.ErrorSummary("No CupertinoLocalizations found.")); __collection1052.Add(new global::Doroti.Framework.Foundation.ErrorDescription($"{(DartRuntimePrimitives.RuntimeType(((global::Doroti.Framework.Widgets.BuildContext)context).widget))} widgets require CupertinoLocalizations " + "to be provided by a Localizations widget ancestor.")); __collection1052.Add(new global::Doroti.Framework.Foundation.ErrorDescription("The cupertino library uses Localizations to generate messages, " + "labels, and abbreviations.")); __collection1052.Add(new global::Doroti.Framework.Foundation.ErrorHint("To introduce a CupertinoLocalizations, either use a " + "CupertinoApp at the root of your application to include them " + "automatically, or add a Localization widget with a " + "CupertinoLocalizations delegate.")); __collection1052.AddRange(context.describeMissingAncestor(expectedAncestorType: typeof(CupertinoLocalizations))); return __collection1052; }))()));
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
