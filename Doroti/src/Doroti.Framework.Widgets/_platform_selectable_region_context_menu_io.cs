// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/_platform_selectable_region_context_menu_io.dart
namespace Doroti.Framework.Widgets;

public delegate void RegisterViewFactoryIo(
    string __unused0,
    Func<long, object> __unused1,
    bool isVisible = default!
);

public class PlatformSelectableRegionContextMenuIo : StatelessWidget
{
    public static RegisterViewFactoryIo? debugOverrideRegisterViewFactory = default;

    public PlatformSelectableRegionContextMenuIo(Widget child, Key? key = null)
        : base(key: key) { }

    public static void attach(SelectionContainerDelegate client) =>
        throw new NotImplementedException();

    public static void detach(SelectionContainerDelegate client) =>
        throw new NotImplementedException();

    public static SelectionContainerDelegate? debugActiveClient =>
        throw new NotImplementedException();

    public static void debugResetRegistry()
    {
        throw new NotImplementedException();
    }

    public override Widget build(BuildContext context) => throw new NotImplementedException();
}
