// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/primary_scroll_controller.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public static partial class Primary_scroll_controllerLibrary
{
    internal static HashSet<global::Doroti.Framework.Foundation.TargetPlatform> _kMobilePlatforms = new HashSet<global::Doroti.Framework.Foundation.TargetPlatform> { TargetPlatform.android, TargetPlatform.iOS, TargetPlatform.fuchsia };
}

public class PrimaryScrollController : InheritedWidget
{
    public virtual ScrollController? controller { get; private set; }
    public virtual global::Doroti.Framework.Painting.Axis? scrollDirection { get; private set; }
    public virtual HashSet<global::Doroti.Framework.Foundation.TargetPlatform> automaticallyInheritForPlatforms { get; private set; } = default!;

    public PrimaryScrollController(global::Doroti.Framework.Foundation.Key? key = null, ScrollController controller = default!, HashSet<global::Doroti.Framework.Foundation.TargetPlatform> automaticallyInheritForPlatforms = default!, global::Doroti.Framework.Painting.Axis? scrollDirection = Axis.vertical, Widget child = default!) : base(key: key, child: child)
    {
        HashSet<global::Doroti.Framework.Foundation.TargetPlatform> __automaticallyInheritForPlatforms = automaticallyInheritForPlatforms ?? Primary_scroll_controllerLibrary._kMobilePlatforms;
        this.controller = controller;
        this.automaticallyInheritForPlatforms = __automaticallyInheritForPlatforms;
        this.scrollDirection = scrollDirection;
    }

    public static PrimaryScrollController CreateNone(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!)
    {
        var __instance = new PrimaryScrollController(key, default!, default!, default!, child);
        __instance.automaticallyInheritForPlatforms = new HashSet<global::Doroti.Framework.Foundation.TargetPlatform>();
        __instance.scrollDirection = null;
        __instance.controller = null;
        return __instance;
    }

    public static bool shouldInherit(BuildContext context, global::Doroti.Framework.Painting.Axis scrollDirection)
    {
        PrimaryScrollController? result = context.findAncestorWidgetOfExactType<PrimaryScrollController>();
        if (result is null)
        {
            return false;
        }
        global::Doroti.Framework.Foundation.TargetPlatform platform = ScrollConfiguration.of(context).getPlatform(context);
        if (result.automaticallyInheritForPlatforms.Contains(platform))
        {
            return Equals(result.scrollDirection, DartRuntimePrimitives.RequireValue(scrollDirection));
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ScrollController? maybeOf(BuildContext context)
    {
        PrimaryScrollController? result = context.dependOnInheritedWidgetOfExactType<PrimaryScrollController>();
        return result?.controller;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ScrollController of(BuildContext context)
    {
        ScrollController? controller = maybeOf(context);
        DartRuntimePrimitives.Assert(() =>
            {
                if (controller is null)
                {
                    throw DartRuntimePrimitives.AsException(FlutterError.Create("PrimaryScrollController.of() was called with a context that does not contain a " + "PrimaryScrollController widget.\n" + "No PrimaryScrollController widget ancestor could be found starting from the " + "context that was passed to PrimaryScrollController.of(). This can happen " + "because you are using a widget that looks for a PrimaryScrollController " + "ancestor, but no such ancestor exists.\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return controller!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(controller, ((PrimaryScrollController)oldWidget).controller));
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ScrollController>("controller", controller, ifNull: "no controller", showName: false));
    }

}

