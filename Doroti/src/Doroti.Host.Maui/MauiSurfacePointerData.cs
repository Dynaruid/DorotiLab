using Doroti.Ui;

namespace Doroti.Host.Maui;

internal readonly record struct MauiSurfacePointerData(
    TimeSpan Timestamp,
    PointerChange Change,
    PointerDeviceKind Kind,
    ulong Pointer,
    double X,
    double Y,
    int Buttons,
    double ScrollDeltaX,
    double ScrollDeltaY,
    PointerSignalKind SignalKind,
    double Pressure,
    double PanX = 0,
    double PanY = 0,
    double PanDeltaX = 0,
    double PanDeltaY = 0,
    double Scale = 1,
    double Rotation = 0,
    ulong? Device = null,
    double Orientation = 0,
    double Tilt = 0);
