using Doroti.Ui;

namespace Doroti.Framework.Services;

/// <summary>OS drop reception is distinct from framework-internal Draggable/DragTarget.</summary>
public static class OsDragDrop
{
    public static OsDropSupport support(DorotiView view) =>
        view.registeredCapabilityIds.Contains(DorotiCapabilityIds.OsDragDrop)
            ? Host(view).Support : new(false, false, OsDropAction.None, []);

    public static IOsDropRegistration register(DorotiView view, OsDropOptions options, Action<OsDropEvent> onEvent) =>
        Host(view).Register(options, onEvent);

    private static IOsDragDropHostCapability Host(DorotiView view) =>
        view.RequireCapability<IOsDragDropHostCapability>(DorotiCapabilityIds.OsDragDrop, DorotiUiInvocation.Managed("OsDragDrop.register"));
}
