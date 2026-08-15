// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/dialog.dart
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

namespace Doroti.Generated.Framework.Widgets;

public delegate Route<T> RawDialogRouteBuilder<T>(BuildContext context, global::System.Func<BuildContext, Widget> builder);

public static partial class DialogLibrary
{
    public static Future<T?> showRawDialog<T>(BuildContext context, global::System.Func<BuildContext, Widget> builder, global::System.Func<BuildContext, global::System.Func<BuildContext, Widget>, Route<T>>? routeBuilder = null, bool useRootNavigator = true, RouteSettings? routeSettings = null, bool fullscreenDialog = false)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasWidgetsLocalizations(context));
        NavigatorState navigator__2668 = ((NavigatorState)(object?)Navigator.of(context, rootNavigator: useRootNavigator));
        Route<T> route__2754 = ((routeBuilder is null ? new RawDialogRoute<T>(pageBuilder: ((global::System.Func<BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, Widget>)((context, animation, secondaryAnimation) => builder(context))), settings: routeSettings, fullscreenDialog: fullscreenDialog) : routeBuilder.Invoke(context, builder)));
        return ((Future<T?>)(object?)navigator__2668.push<T>(route__2754));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

