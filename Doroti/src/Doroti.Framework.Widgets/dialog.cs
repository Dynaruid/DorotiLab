// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/dialog.dart
#pragma warning disable CS8600, CS8602, CS8603
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

public delegate Route<T> RawDialogRouteBuilder<T>(BuildContext context, global::System.Func<BuildContext, Widget> builder);

public static partial class DialogLibrary
{
    public static Future<T?> showRawDialog<T>(BuildContext context, global::System.Func<BuildContext, Widget> builder, global::System.Func<BuildContext, global::System.Func<BuildContext, Widget>, Route<T>>? routeBuilder = null, bool useRootNavigator = true, RouteSettings? routeSettings = null, bool fullscreenDialog = false)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasWidgetsLocalizations(context));
        NavigatorState navigator = ((NavigatorState)(object?)Navigator.of(context, rootNavigator: useRootNavigator));
        Route<T> route = ((routeBuilder is null ? new RawDialogRoute<T>(pageBuilder: ((global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, Widget>)((context, animation, secondaryAnimation) => builder(context))), settings: routeSettings, fullscreenDialog: fullscreenDialog) : routeBuilder.Invoke(context, builder)));
        return ((Future<T?>)(object?)navigator.push<T>(route));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

