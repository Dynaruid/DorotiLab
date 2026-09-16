// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/debug.dart
using Doroti.Runtime;

namespace Doroti.Framework.Cupertino;

public static partial class DebugLibrary
{
    public static bool debugCheckHasCupertinoLocalizations(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (Localizations.of<CupertinoLocalizations>(context, typeof(CupertinoLocalizations)) is null)
                {
                    throw DartRuntimePrimitives.AsException(new FlutterError(((Func<List<DiagnosticsNode>>)(() => { var __collection1052 = new List<DiagnosticsNode>(); __collection1052.Add(new ErrorSummary("No CupertinoLocalizations found.")); __collection1052.Add(new ErrorDescription($"{DartRuntimePrimitives.RuntimeType(context.widget)} widgets require CupertinoLocalizations " + "to be provided by a Localizations widget ancestor.")); __collection1052.Add(new ErrorDescription("The cupertino library uses Localizations to generate messages, " + "labels, and abbreviations.")); __collection1052.Add(new ErrorHint("To introduce a CupertinoLocalizations, either use a " + "CupertinoApp at the root of your application to include them " + "automatically, or add a Localization widget with a " + "CupertinoLocalizations delegate.")); __collection1052.AddRange(context.describeMissingAncestor(expectedAncestorType: typeof(CupertinoLocalizations))); return __collection1052; }))()));
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
