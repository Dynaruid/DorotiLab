// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/converter.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Gestures;

public static partial class ConverterLibrary
{
    internal static long _synthesiseDownButtons(long buttons, PointerDeviceKind kind)
    {
        switch (kind)
        {
            case PointerDeviceKind.mouse:
            case PointerDeviceKind.trackpad:
                {
                    return buttons;
                }
            case PointerDeviceKind.touch:
            case PointerDeviceKind.stylus:
            case PointerDeviceKind.invertedStylus:
                {
                    return ((buttons == 0L) ? global::Doroti.Generated.Framework.Gestures.EventsLibrary.kPrimaryButton : buttons);
                }
            case PointerDeviceKind.unknown:
                {
                    return ((buttons == 0L) ? global::Doroti.Generated.Framework.Gestures.EventsLibrary.kPrimaryButton : buttons);
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public delegate double? DevicePixelRatioGetter(long viewId);

public abstract class PointerEventConverter
{
    public static IEnumerable<PointerEvent> expand(IEnumerable<PointerData> data, Func<long, double?> devicePixelRatioForView)
    {
        return data.where(((datum) => (!object.Equals(datum.signalKind, Dart_uiLibrary.PointerSignalKind.unknown)))).map<PointerData, PointerEvent?>(((datum) =>
        {
            double? devicePixelRatio__2653 = devicePixelRatioForView(checked((long)datum.viewId));
            if ((devicePixelRatio__2653 is null))
            {
                return null;
            }
            global::Doroti.Flutter.Ui.Offset position__2856 = (new global::Doroti.Flutter.Ui.Offset(datum.physicalX, datum.physicalY) / DartRuntimePrimitives.RequireValue(devicePixelRatio__2653));
            global::Doroti.Flutter.Ui.Offset delta__2951 = (new global::Doroti.Flutter.Ui.Offset(datum.physicalDeltaX, datum.physicalDeltaY) / DartRuntimePrimitives.RequireValue(devicePixelRatio__2653));
            double radiusMinor__3067 = _toLogicalPixels(datum.radiusMinor, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(devicePixelRatio__2653)));
            double radiusMajor__3159 = _toLogicalPixels(datum.radiusMajor, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(devicePixelRatio__2653)));
            double radiusMin__3251 = _toLogicalPixels(datum.radiusMin, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(devicePixelRatio__2653)));
            double radiusMax__3339 = _toLogicalPixels(datum.radiusMax, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(devicePixelRatio__2653)));
            Duration timeStamp__3429 = datum.timeStamp;
            global::Doroti.Flutter.Ui.PointerDeviceKind kind__3492 = datum.kind;
            switch ((datum.signalKind ?? Dart_uiLibrary.PointerSignalKind.none))
            {
                case Dart_uiLibrary.PointerSignalKind.none:
                    {
                        switch (datum.change)
                        {
                            case Dart_uiLibrary.PointerChange.add:
                                {
                                    return new PointerAddedEvent(viewId: checked((long)datum.viewId), timeStamp: timeStamp__3429, kind: kind__3492, device: checked((long)datum.device), position: position__2856, obscured: datum.obscured, pressureMin: datum.pressureMin, pressureMax: datum.pressureMax, distance: datum.distance, distanceMax: datum.distanceMax, radiusMin: radiusMin__3251, radiusMax: radiusMax__3339, orientation: datum.orientation, tilt: datum.tilt, embedderId: checked((long)datum.embedderId));
                                }
                            case Dart_uiLibrary.PointerChange.hover:
                                {
                                    return new PointerHoverEvent(viewId: checked((long)datum.viewId), timeStamp: timeStamp__3429, kind: kind__3492, device: checked((long)datum.device), position: position__2856, delta: delta__2951, buttons: datum.buttons, obscured: datum.obscured, pressureMin: datum.pressureMin, pressureMax: datum.pressureMax, distance: datum.distance, distanceMax: datum.distanceMax, size: datum.size, radiusMajor: radiusMajor__3159, radiusMinor: radiusMinor__3067, radiusMin: radiusMin__3251, radiusMax: radiusMax__3339, orientation: datum.orientation, tilt: datum.tilt, synthesized: datum.synthesized, embedderId: checked((long)datum.embedderId));
                                }
                            case Dart_uiLibrary.PointerChange.down:
                                {
                                    return new PointerDownEvent(viewId: checked((long)datum.viewId), timeStamp: timeStamp__3429, pointer: checked((long)datum.pointerIdentifier), kind: kind__3492, device: checked((long)datum.device), position: position__2856, buttons: ConverterLibrary._synthesiseDownButtons(datum.buttons, kind__3492), obscured: datum.obscured, pressure: datum.pressure, pressureMin: datum.pressureMin, pressureMax: datum.pressureMax, distanceMax: datum.distanceMax, size: datum.size, radiusMajor: radiusMajor__3159, radiusMinor: radiusMinor__3067, radiusMin: radiusMin__3251, radiusMax: radiusMax__3339, orientation: datum.orientation, tilt: datum.tilt, embedderId: checked((long)datum.embedderId));
                                }
                            case Dart_uiLibrary.PointerChange.move:
                                {
                                    return new PointerMoveEvent(viewId: checked((long)datum.viewId), timeStamp: timeStamp__3429, pointer: checked((long)datum.pointerIdentifier), kind: kind__3492, device: checked((long)datum.device), position: position__2856, delta: delta__2951, buttons: ConverterLibrary._synthesiseDownButtons(datum.buttons, kind__3492), obscured: datum.obscured, pressure: datum.pressure, pressureMin: datum.pressureMin, pressureMax: datum.pressureMax, distanceMax: datum.distanceMax, size: datum.size, radiusMajor: radiusMajor__3159, radiusMinor: radiusMinor__3067, radiusMin: radiusMin__3251, radiusMax: radiusMax__3339, orientation: datum.orientation, tilt: datum.tilt, platformData: datum.platformData, synthesized: datum.synthesized, embedderId: checked((long)datum.embedderId));
                                }
                            case Dart_uiLibrary.PointerChange.up:
                                {
                                    return new PointerUpEvent(viewId: checked((long)datum.viewId), timeStamp: timeStamp__3429, pointer: checked((long)datum.pointerIdentifier), kind: kind__3492, device: checked((long)datum.device), position: position__2856, buttons: datum.buttons, obscured: datum.obscured, pressure: datum.pressure, pressureMin: datum.pressureMin, pressureMax: datum.pressureMax, distance: datum.distance, distanceMax: datum.distanceMax, size: datum.size, radiusMajor: radiusMajor__3159, radiusMinor: radiusMinor__3067, radiusMin: radiusMin__3251, radiusMax: radiusMax__3339, orientation: datum.orientation, tilt: datum.tilt, embedderId: checked((long)datum.embedderId));
                                }
                            case Dart_uiLibrary.PointerChange.cancel:
                                {
                                    return new PointerCancelEvent(viewId: checked((long)datum.viewId), timeStamp: timeStamp__3429, pointer: checked((long)datum.pointerIdentifier), kind: kind__3492, device: checked((long)datum.device), position: position__2856, buttons: datum.buttons, obscured: datum.obscured, pressureMin: datum.pressureMin, pressureMax: datum.pressureMax, distance: datum.distance, distanceMax: datum.distanceMax, size: datum.size, radiusMajor: radiusMajor__3159, radiusMinor: radiusMinor__3067, radiusMin: radiusMin__3251, radiusMax: radiusMax__3339, orientation: datum.orientation, tilt: datum.tilt, embedderId: checked((long)datum.embedderId));
                                }
                            case Dart_uiLibrary.PointerChange.remove:
                                {
                                    return new PointerRemovedEvent(viewId: checked((long)datum.viewId), timeStamp: timeStamp__3429, kind: kind__3492, device: checked((long)datum.device), position: position__2856, obscured: datum.obscured, pressureMin: datum.pressureMin, pressureMax: datum.pressureMax, distanceMax: datum.distanceMax, radiusMin: radiusMin__3251, radiusMax: radiusMax__3339, embedderId: checked((long)datum.embedderId));
                                }
                            case Dart_uiLibrary.PointerChange.panZoomStart:
                                {
                                    return new PointerPanZoomStartEvent(viewId: checked((long)datum.viewId), timeStamp: timeStamp__3429, pointer: checked((long)datum.pointerIdentifier), device: checked((long)datum.device), position: position__2856, embedderId: checked((long)datum.embedderId), synthesized: datum.synthesized);
                                }
                            case Dart_uiLibrary.PointerChange.panZoomUpdate:
                                {
                                    global::Doroti.Flutter.Ui.Offset pan__10925 = (new global::Doroti.Flutter.Ui.Offset(datum.panX, datum.panY) / DartRuntimePrimitives.RequireValue(devicePixelRatio__2653));
                                    global::Doroti.Flutter.Ui.Offset panDelta__11013 = (new global::Doroti.Flutter.Ui.Offset(datum.panDeltaX, datum.panDeltaY) / DartRuntimePrimitives.RequireValue(devicePixelRatio__2653));
                                    return new PointerPanZoomUpdateEvent(viewId: checked((long)datum.viewId), timeStamp: timeStamp__3429, pointer: checked((long)datum.pointerIdentifier), device: checked((long)datum.device), position: position__2856, pan: pan__10925, panDelta: panDelta__11013, scale: datum.scale, rotation: datum.rotation, embedderId: checked((long)datum.embedderId), synthesized: datum.synthesized);
                                }
                            case Dart_uiLibrary.PointerChange.panZoomEnd:
                                {
                                    return new PointerPanZoomEndEvent(viewId: checked((long)datum.viewId), timeStamp: timeStamp__3429, pointer: checked((long)datum.pointerIdentifier), device: checked((long)datum.device), position: position__2856, embedderId: checked((long)datum.embedderId), synthesized: datum.synthesized);
                                }
                        }
                        break;
                    }
                case Dart_uiLibrary.PointerSignalKind.scroll:
                    {
                        if (((!double.IsFinite(datum.scrollDeltaX) || !double.IsFinite(datum.scrollDeltaY)) || (devicePixelRatio__2653 <= 0L)))
                        {
                            return null;
                        }
                        global::Doroti.Flutter.Ui.Offset scrollDelta__12377 = (new global::Doroti.Flutter.Ui.Offset(datum.scrollDeltaX, datum.scrollDeltaY) / DartRuntimePrimitives.RequireValue(devicePixelRatio__2653));
                        return new PointerScrollEvent(viewId: checked((long)datum.viewId), timeStamp: timeStamp__3429, kind: kind__3492, device: checked((long)datum.device), position: position__2856, scrollDelta: scrollDelta__12377, embedderId: checked((long)datum.embedderId), onRespond: datum.respond);
                    }
                case Dart_uiLibrary.PointerSignalKind.scrollInertiaCancel:
                    {
                        return new PointerScrollInertiaCancelEvent(viewId: checked((long)datum.viewId), timeStamp: timeStamp__3429, kind: kind__3492, device: checked((long)datum.device), position: position__2856, embedderId: checked((long)datum.embedderId));
                    }
                case Dart_uiLibrary.PointerSignalKind.scale:
                    {
                        return new PointerScaleEvent(viewId: checked((long)datum.viewId), timeStamp: timeStamp__3429, kind: kind__3492, device: checked((long)datum.device), position: position__2856, embedderId: checked((long)datum.embedderId), scale: datum.scale);
                    }
                case Dart_uiLibrary.PointerSignalKind.unknown:
                    {
                        throw new InvalidOperationException("Unreachable");
                    }
            }
            return default;
        })).OfType<PointerEvent>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _toLogicalPixels(double physicalPixels, double devicePixelRatio) => (physicalPixels / devicePixelRatio);
}

