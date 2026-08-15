namespace Doroti.Runtime;

/// <summary>
/// Cross-package identity for pointer events. Flutter's services and gestures
/// libraries form a Dart-level dependency cycle, so the CLR projects share
/// this narrow runtime contract instead of duplicating concrete event types.
/// </summary>
public interface IPointerEvent;

/// <summary>Marks the removal event that terminates a device cursor session.</summary>
public interface IPointerRemovedEvent : IPointerEvent;
