// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/converter.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Gestures;

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
                    return ((buttons == 0L) ? global::Doroti.Framework.Gestures.EventsLibrary.kPrimaryButton : buttons);
                }
            case PointerDeviceKind.unknown:
                {
                    return ((buttons == 0L) ? global::Doroti.Framework.Gestures.EventsLibrary.kPrimaryButton : buttons);
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
            double? devicePixelRatio = devicePixelRatioForView(checked((long)datum.viewId));
            if ((devicePixelRatio is null))
            {
                return null;
            }
            global::Doroti.Ui.Offset positionLocal = (new global::Doroti.Ui.Offset(datum.physicalX, datum.physicalY) / DartRuntimePrimitives.RequireValue(devicePixelRatio));
            global::Doroti.Ui.Offset deltaLocal = (new global::Doroti.Ui.Offset(datum.physicalDeltaX, datum.physicalDeltaY) / DartRuntimePrimitives.RequireValue(devicePixelRatio));
            double radiusMinorLocal = _toLogicalPixels(datum.radiusMinor, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(devicePixelRatio)));
            double radiusMajorLocal = _toLogicalPixels(datum.radiusMajor, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(devicePixelRatio)));
            double radiusMinLocal = _toLogicalPixels(datum.radiusMin, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(devicePixelRatio)));
            double radiusMaxLocal = _toLogicalPixels(datum.radiusMax, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(devicePixelRatio)));
            Duration timeStampLocal = datum.timeStamp;
            global::Doroti.Ui.PointerDeviceKind kindLocal = datum.kind;
            switch ((datum.signalKind ?? Dart_uiLibrary.PointerSignalKind.none))
            {
                case Dart_uiLibrary.PointerSignalKind.none:
                    {
                        switch (datum.change)
                        {
                            case Dart_uiLibrary.PointerChange.add:
                                {
                                    return new PointerAddedEvent(viewId: checked((long)datum.viewId), timeStamp: timeStampLocal, kind: kindLocal, device: checked((long)datum.device), position: positionLocal, obscured: datum.obscured, pressureMin: datum.pressureMin, pressureMax: datum.pressureMax, distance: datum.distance, distanceMax: datum.distanceMax, radiusMin: radiusMinLocal, radiusMax: radiusMaxLocal, orientation: datum.orientation, tilt: datum.tilt, embedderId: checked((long)datum.embedderId));
                                }
                            case Dart_uiLibrary.PointerChange.hover:
                                {
                                    return new PointerHoverEvent(viewId: checked((long)datum.viewId), timeStamp: timeStampLocal, kind: kindLocal, device: checked((long)datum.device), position: positionLocal, delta: deltaLocal, buttons: datum.buttons, obscured: datum.obscured, pressureMin: datum.pressureMin, pressureMax: datum.pressureMax, distance: datum.distance, distanceMax: datum.distanceMax, size: datum.size, radiusMajor: radiusMajorLocal, radiusMinor: radiusMinorLocal, radiusMin: radiusMinLocal, radiusMax: radiusMaxLocal, orientation: datum.orientation, tilt: datum.tilt, synthesized: datum.synthesized, embedderId: checked((long)datum.embedderId));
                                }
                            case Dart_uiLibrary.PointerChange.down:
                                {
                                    return new PointerDownEvent(viewId: checked((long)datum.viewId), timeStamp: timeStampLocal, pointer: checked((long)datum.pointerIdentifier), kind: kindLocal, device: checked((long)datum.device), position: positionLocal, buttons: ConverterLibrary._synthesiseDownButtons(datum.buttons, kindLocal), obscured: datum.obscured, pressure: datum.pressure, pressureMin: datum.pressureMin, pressureMax: datum.pressureMax, distanceMax: datum.distanceMax, size: datum.size, radiusMajor: radiusMajorLocal, radiusMinor: radiusMinorLocal, radiusMin: radiusMinLocal, radiusMax: radiusMaxLocal, orientation: datum.orientation, tilt: datum.tilt, embedderId: checked((long)datum.embedderId));
                                }
                            case Dart_uiLibrary.PointerChange.move:
                                {
                                    return new PointerMoveEvent(viewId: checked((long)datum.viewId), timeStamp: timeStampLocal, pointer: checked((long)datum.pointerIdentifier), kind: kindLocal, device: checked((long)datum.device), position: positionLocal, delta: deltaLocal, buttons: ConverterLibrary._synthesiseDownButtons(datum.buttons, kindLocal), obscured: datum.obscured, pressure: datum.pressure, pressureMin: datum.pressureMin, pressureMax: datum.pressureMax, distanceMax: datum.distanceMax, size: datum.size, radiusMajor: radiusMajorLocal, radiusMinor: radiusMinorLocal, radiusMin: radiusMinLocal, radiusMax: radiusMaxLocal, orientation: datum.orientation, tilt: datum.tilt, platformData: datum.platformData, synthesized: datum.synthesized, embedderId: checked((long)datum.embedderId));
                                }
                            case Dart_uiLibrary.PointerChange.up:
                                {
                                    return new PointerUpEvent(viewId: checked((long)datum.viewId), timeStamp: timeStampLocal, pointer: checked((long)datum.pointerIdentifier), kind: kindLocal, device: checked((long)datum.device), position: positionLocal, buttons: datum.buttons, obscured: datum.obscured, pressure: datum.pressure, pressureMin: datum.pressureMin, pressureMax: datum.pressureMax, distance: datum.distance, distanceMax: datum.distanceMax, size: datum.size, radiusMajor: radiusMajorLocal, radiusMinor: radiusMinorLocal, radiusMin: radiusMinLocal, radiusMax: radiusMaxLocal, orientation: datum.orientation, tilt: datum.tilt, embedderId: checked((long)datum.embedderId));
                                }
                            case Dart_uiLibrary.PointerChange.cancel:
                                {
                                    return new PointerCancelEvent(viewId: checked((long)datum.viewId), timeStamp: timeStampLocal, pointer: checked((long)datum.pointerIdentifier), kind: kindLocal, device: checked((long)datum.device), position: positionLocal, buttons: datum.buttons, obscured: datum.obscured, pressureMin: datum.pressureMin, pressureMax: datum.pressureMax, distance: datum.distance, distanceMax: datum.distanceMax, size: datum.size, radiusMajor: radiusMajorLocal, radiusMinor: radiusMinorLocal, radiusMin: radiusMinLocal, radiusMax: radiusMaxLocal, orientation: datum.orientation, tilt: datum.tilt, embedderId: checked((long)datum.embedderId));
                                }
                            case Dart_uiLibrary.PointerChange.remove:
                                {
                                    return new PointerRemovedEvent(viewId: checked((long)datum.viewId), timeStamp: timeStampLocal, kind: kindLocal, device: checked((long)datum.device), position: positionLocal, obscured: datum.obscured, pressureMin: datum.pressureMin, pressureMax: datum.pressureMax, distanceMax: datum.distanceMax, radiusMin: radiusMinLocal, radiusMax: radiusMaxLocal, embedderId: checked((long)datum.embedderId));
                                }
                            case Dart_uiLibrary.PointerChange.panZoomStart:
                                {
                                    return new PointerPanZoomStartEvent(viewId: checked((long)datum.viewId), timeStamp: timeStampLocal, pointer: checked((long)datum.pointerIdentifier), device: checked((long)datum.device), position: positionLocal, embedderId: checked((long)datum.embedderId), synthesized: datum.synthesized);
                                }
                            case Dart_uiLibrary.PointerChange.panZoomUpdate:
                                {
                                    global::Doroti.Ui.Offset panLocal = (new global::Doroti.Ui.Offset(datum.panX, datum.panY) / DartRuntimePrimitives.RequireValue(devicePixelRatio));
                                    global::Doroti.Ui.Offset panDeltaLocal = (new global::Doroti.Ui.Offset(datum.panDeltaX, datum.panDeltaY) / DartRuntimePrimitives.RequireValue(devicePixelRatio));
                                    return new PointerPanZoomUpdateEvent(viewId: checked((long)datum.viewId), timeStamp: timeStampLocal, pointer: checked((long)datum.pointerIdentifier), device: checked((long)datum.device), position: positionLocal, pan: panLocal, panDelta: panDeltaLocal, scale: datum.scale, rotation: datum.rotation, embedderId: checked((long)datum.embedderId), synthesized: datum.synthesized);
                                }
                            case Dart_uiLibrary.PointerChange.panZoomEnd:
                                {
                                    return new PointerPanZoomEndEvent(viewId: checked((long)datum.viewId), timeStamp: timeStampLocal, pointer: checked((long)datum.pointerIdentifier), device: checked((long)datum.device), position: positionLocal, embedderId: checked((long)datum.embedderId), synthesized: datum.synthesized);
                                }
                        }
                        break;
                    }
                case Dart_uiLibrary.PointerSignalKind.scroll:
                    {
                        if (((!double.IsFinite(datum.scrollDeltaX) || !double.IsFinite(datum.scrollDeltaY)) || (devicePixelRatio <= 0L)))
                        {
                            return null;
                        }
                        global::Doroti.Ui.Offset scrollDeltaLocal = (new global::Doroti.Ui.Offset(datum.scrollDeltaX, datum.scrollDeltaY) / DartRuntimePrimitives.RequireValue(devicePixelRatio));
                        return new PointerScrollEvent(viewId: checked((long)datum.viewId), timeStamp: timeStampLocal, kind: kindLocal, device: checked((long)datum.device), position: positionLocal, scrollDelta: scrollDeltaLocal, embedderId: checked((long)datum.embedderId), onRespond: datum.respond);
                    }
                case Dart_uiLibrary.PointerSignalKind.scrollInertiaCancel:
                    {
                        return new PointerScrollInertiaCancelEvent(viewId: checked((long)datum.viewId), timeStamp: timeStampLocal, kind: kindLocal, device: checked((long)datum.device), position: positionLocal, embedderId: checked((long)datum.embedderId));
                    }
                case Dart_uiLibrary.PointerSignalKind.scale:
                    {
                        return new PointerScaleEvent(viewId: checked((long)datum.viewId), timeStamp: timeStampLocal, kind: kindLocal, device: checked((long)datum.device), position: positionLocal, embedderId: checked((long)datum.embedderId), scale: datum.scale);
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

