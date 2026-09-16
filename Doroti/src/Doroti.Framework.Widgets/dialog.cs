// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/dialog.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public delegate Route<T> RawDialogRouteBuilder<T>(BuildContext context, Func<BuildContext, Widget> builder);

public static partial class DialogLibrary
{
    public static Future<T?> showRawDialog<T>(BuildContext context, Func<BuildContext, Widget> builder, Func<BuildContext, Func<BuildContext, Widget>, Route<T>>? routeBuilder = null, bool useRootNavigator = true, RouteSettings? routeSettings = null, bool fullscreenDialog = false)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasWidgetsLocalizations(context));
        NavigatorState navigator = Navigator.of(context, rootNavigator: useRootNavigator);
        Route<T> route = routeBuilder is null ? new RawDialogRoute<T>(pageBuilder: (context, animation, secondaryAnimation) => builder(context), settings: routeSettings, fullscreenDialog: fullscreenDialog) : routeBuilder.Invoke(context, builder);
        return navigator.push(route);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

