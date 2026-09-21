// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/converter.dart
using Doroti.Runtime;
using Doroti.Ui;

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
                return (buttons == 0L) ? EventsLibrary.kPrimaryButton : buttons;
            }
            case PointerDeviceKind.unknown:
            {
                return (buttons == 0L) ? EventsLibrary.kPrimaryButton : buttons;
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public delegate double? DevicePixelRatioGetter(long viewId);

public abstract class PointerEventConverter
{
    public static IEnumerable<PointerEvent> expand(
        IEnumerable<PointerData> data,
        Func<long, double?> devicePixelRatioForView
    )
    {
        return data.where(
                (datum) => !Equals(datum.signalKind, Dart_uiLibrary.PointerSignalKind.unknown)
            )
            .map<PointerData, PointerEvent?>(
                (datum) =>
                {
                    double? devicePixelRatio = devicePixelRatioForView(checked((long)datum.viewId));
                    if (devicePixelRatio is null)
                    {
                        return null;
                    }
                    Offset positionLocal =
                        new Offset(datum.physicalX, datum.physicalY)
                        / DartRuntimePrimitives.RequireValue(devicePixelRatio);
                    Offset deltaLocal =
                        new Offset(datum.physicalDeltaX, datum.physicalDeltaY)
                        / DartRuntimePrimitives.RequireValue(devicePixelRatio);
                    double radiusMinorLocal = _toLogicalPixels(
                        datum.radiusMinor,
                        DartRuntimePrimitives.RequireValue(
                            DartRuntimePrimitives.RequireValue(devicePixelRatio)
                        )
                    );
                    double radiusMajorLocal = _toLogicalPixels(
                        datum.radiusMajor,
                        DartRuntimePrimitives.RequireValue(
                            DartRuntimePrimitives.RequireValue(devicePixelRatio)
                        )
                    );
                    double radiusMinLocal = _toLogicalPixels(
                        datum.radiusMin,
                        DartRuntimePrimitives.RequireValue(
                            DartRuntimePrimitives.RequireValue(devicePixelRatio)
                        )
                    );
                    double radiusMaxLocal = _toLogicalPixels(
                        datum.radiusMax,
                        DartRuntimePrimitives.RequireValue(
                            DartRuntimePrimitives.RequireValue(devicePixelRatio)
                        )
                    );
                    Duration timeStampLocal = datum.timeStamp;
                    PointerDeviceKind kindLocal = datum.kind;
                    switch (datum.signalKind ?? Dart_uiLibrary.PointerSignalKind.none)
                    {
                        case Dart_uiLibrary.PointerSignalKind.none:
                        {
                            switch (datum.change)
                            {
                                case Dart_uiLibrary.PointerChange.add:
                                {
                                    return new PointerAddedEvent(
                                        viewId: checked((long)datum.viewId),
                                        timeStamp: timeStampLocal,
                                        kind: kindLocal,
                                        device: checked((long)datum.device),
                                        position: positionLocal,
                                        obscured: datum.obscured,
                                        pressureMin: datum.pressureMin,
                                        pressureMax: datum.pressureMax,
                                        distance: datum.distance,
                                        distanceMax: datum.distanceMax,
                                        radiusMin: radiusMinLocal,
                                        radiusMax: radiusMaxLocal,
                                        orientation: datum.orientation,
                                        tilt: datum.tilt,
                                        embedderId: checked((long)datum.embedderId)
                                    );
                                }
                                case Dart_uiLibrary.PointerChange.hover:
                                {
                                    return new PointerHoverEvent(
                                        viewId: checked((long)datum.viewId),
                                        timeStamp: timeStampLocal,
                                        kind: kindLocal,
                                        device: checked((long)datum.device),
                                        position: positionLocal,
                                        delta: deltaLocal,
                                        buttons: datum.buttons,
                                        obscured: datum.obscured,
                                        pressureMin: datum.pressureMin,
                                        pressureMax: datum.pressureMax,
                                        distance: datum.distance,
                                        distanceMax: datum.distanceMax,
                                        size: datum.size,
                                        radiusMajor: radiusMajorLocal,
                                        radiusMinor: radiusMinorLocal,
                                        radiusMin: radiusMinLocal,
                                        radiusMax: radiusMaxLocal,
                                        orientation: datum.orientation,
                                        tilt: datum.tilt,
                                        synthesized: datum.synthesized,
                                        embedderId: checked((long)datum.embedderId)
                                    );
                                }
                                case Dart_uiLibrary.PointerChange.down:
                                {
                                    return new PointerDownEvent(
                                        viewId: checked((long)datum.viewId),
                                        timeStamp: timeStampLocal,
                                        pointer: checked((long)datum.pointerIdentifier),
                                        kind: kindLocal,
                                        device: checked((long)datum.device),
                                        position: positionLocal,
                                        buttons: ConverterLibrary._synthesiseDownButtons(
                                            datum.buttons,
                                            kindLocal
                                        ),
                                        obscured: datum.obscured,
                                        pressure: datum.pressure,
                                        pressureMin: datum.pressureMin,
                                        pressureMax: datum.pressureMax,
                                        distanceMax: datum.distanceMax,
                                        size: datum.size,
                                        radiusMajor: radiusMajorLocal,
                                        radiusMinor: radiusMinorLocal,
                                        radiusMin: radiusMinLocal,
                                        radiusMax: radiusMaxLocal,
                                        orientation: datum.orientation,
                                        tilt: datum.tilt,
                                        embedderId: checked((long)datum.embedderId)
                                    );
                                }
                                case Dart_uiLibrary.PointerChange.move:
                                {
                                    return new PointerMoveEvent(
                                        viewId: checked((long)datum.viewId),
                                        timeStamp: timeStampLocal,
                                        pointer: checked((long)datum.pointerIdentifier),
                                        kind: kindLocal,
                                        device: checked((long)datum.device),
                                        position: positionLocal,
                                        delta: deltaLocal,
                                        buttons: ConverterLibrary._synthesiseDownButtons(
                                            datum.buttons,
                                            kindLocal
                                        ),
                                        obscured: datum.obscured,
                                        pressure: datum.pressure,
                                        pressureMin: datum.pressureMin,
                                        pressureMax: datum.pressureMax,
                                        distanceMax: datum.distanceMax,
                                        size: datum.size,
                                        radiusMajor: radiusMajorLocal,
                                        radiusMinor: radiusMinorLocal,
                                        radiusMin: radiusMinLocal,
                                        radiusMax: radiusMaxLocal,
                                        orientation: datum.orientation,
                                        tilt: datum.tilt,
                                        platformData: datum.platformData,
                                        synthesized: datum.synthesized,
                                        embedderId: checked((long)datum.embedderId)
                                    );
                                }
                                case Dart_uiLibrary.PointerChange.up:
                                {
                                    return new PointerUpEvent(
                                        viewId: checked((long)datum.viewId),
                                        timeStamp: timeStampLocal,
                                        pointer: checked((long)datum.pointerIdentifier),
                                        kind: kindLocal,
                                        device: checked((long)datum.device),
                                        position: positionLocal,
                                        buttons: datum.buttons,
                                        obscured: datum.obscured,
                                        pressure: datum.pressure,
                                        pressureMin: datum.pressureMin,
                                        pressureMax: datum.pressureMax,
                                        distance: datum.distance,
                                        distanceMax: datum.distanceMax,
                                        size: datum.size,
                                        radiusMajor: radiusMajorLocal,
                                        radiusMinor: radiusMinorLocal,
                                        radiusMin: radiusMinLocal,
                                        radiusMax: radiusMaxLocal,
                                        orientation: datum.orientation,
                                        tilt: datum.tilt,
                                        embedderId: checked((long)datum.embedderId)
                                    );
                                }
                                case Dart_uiLibrary.PointerChange.cancel:
                                {
                                    return new PointerCancelEvent(
                                        viewId: checked((long)datum.viewId),
                                        timeStamp: timeStampLocal,
                                        pointer: checked((long)datum.pointerIdentifier),
                                        kind: kindLocal,
                                        device: checked((long)datum.device),
                                        position: positionLocal,
                                        buttons: datum.buttons,
                                        obscured: datum.obscured,
                                        pressureMin: datum.pressureMin,
                                        pressureMax: datum.pressureMax,
                                        distance: datum.distance,
                                        distanceMax: datum.distanceMax,
                                        size: datum.size,
                                        radiusMajor: radiusMajorLocal,
                                        radiusMinor: radiusMinorLocal,
                                        radiusMin: radiusMinLocal,
                                        radiusMax: radiusMaxLocal,
                                        orientation: datum.orientation,
                                        tilt: datum.tilt,
                                        embedderId: checked((long)datum.embedderId)
                                    );
                                }
                                case Dart_uiLibrary.PointerChange.remove:
                                {
                                    return new PointerRemovedEvent(
                                        viewId: checked((long)datum.viewId),
                                        timeStamp: timeStampLocal,
                                        kind: kindLocal,
                                        device: checked((long)datum.device),
                                        position: positionLocal,
                                        obscured: datum.obscured,
                                        pressureMin: datum.pressureMin,
                                        pressureMax: datum.pressureMax,
                                        distanceMax: datum.distanceMax,
                                        radiusMin: radiusMinLocal,
                                        radiusMax: radiusMaxLocal,
                                        embedderId: checked((long)datum.embedderId)
                                    );
                                }
                                case Dart_uiLibrary.PointerChange.panZoomStart:
                                {
                                    return new PointerPanZoomStartEvent(
                                        viewId: checked((long)datum.viewId),
                                        timeStamp: timeStampLocal,
                                        pointer: checked((long)datum.pointerIdentifier),
                                        device: checked((long)datum.device),
                                        position: positionLocal,
                                        embedderId: checked((long)datum.embedderId),
                                        synthesized: datum.synthesized
                                    );
                                }
                                case Dart_uiLibrary.PointerChange.panZoomUpdate:
                                {
                                    Offset panLocal =
                                        new Offset(datum.panX, datum.panY)
                                        / DartRuntimePrimitives.RequireValue(devicePixelRatio);
                                    Offset panDeltaLocal =
                                        new Offset(datum.panDeltaX, datum.panDeltaY)
                                        / DartRuntimePrimitives.RequireValue(devicePixelRatio);
                                    return new PointerPanZoomUpdateEvent(
                                        viewId: checked((long)datum.viewId),
                                        timeStamp: timeStampLocal,
                                        pointer: checked((long)datum.pointerIdentifier),
                                        device: checked((long)datum.device),
                                        position: positionLocal,
                                        pan: panLocal,
                                        panDelta: panDeltaLocal,
                                        scale: datum.scale,
                                        rotation: datum.rotation,
                                        embedderId: checked((long)datum.embedderId),
                                        synthesized: datum.synthesized
                                    );
                                }
                                case Dart_uiLibrary.PointerChange.panZoomEnd:
                                {
                                    return new PointerPanZoomEndEvent(
                                        viewId: checked((long)datum.viewId),
                                        timeStamp: timeStampLocal,
                                        pointer: checked((long)datum.pointerIdentifier),
                                        device: checked((long)datum.device),
                                        position: positionLocal,
                                        embedderId: checked((long)datum.embedderId),
                                        synthesized: datum.synthesized
                                    );
                                }
                            }
                            break;
                        }
                        case Dart_uiLibrary.PointerSignalKind.scroll:
                        {
                            if (
                                !double.IsFinite(datum.scrollDeltaX)
                                || !double.IsFinite(datum.scrollDeltaY)
                                || (devicePixelRatio <= 0L)
                            )
                            {
                                return null;
                            }
                            Offset scrollDeltaLocal =
                                new Offset(datum.scrollDeltaX, datum.scrollDeltaY)
                                / DartRuntimePrimitives.RequireValue(devicePixelRatio);
                            return new PointerScrollEvent(
                                viewId: checked((long)datum.viewId),
                                timeStamp: timeStampLocal,
                                kind: kindLocal,
                                device: checked((long)datum.device),
                                position: positionLocal,
                                scrollDelta: scrollDeltaLocal,
                                embedderId: checked((long)datum.embedderId),
                                onRespond: datum.respond
                            );
                        }
                        case Dart_uiLibrary.PointerSignalKind.scrollInertiaCancel:
                        {
                            return new PointerScrollInertiaCancelEvent(
                                viewId: checked((long)datum.viewId),
                                timeStamp: timeStampLocal,
                                kind: kindLocal,
                                device: checked((long)datum.device),
                                position: positionLocal,
                                embedderId: checked((long)datum.embedderId)
                            );
                        }
                        case Dart_uiLibrary.PointerSignalKind.scale:
                        {
                            return new PointerScaleEvent(
                                viewId: checked((long)datum.viewId),
                                timeStamp: timeStampLocal,
                                kind: kindLocal,
                                device: checked((long)datum.device),
                                position: positionLocal,
                                embedderId: checked((long)datum.embedderId),
                                scale: datum.scale
                            );
                        }
                        case Dart_uiLibrary.PointerSignalKind.unknown:
                        {
                            throw new InvalidOperationException("Unreachable");
                        }
                    }
                    return default;
                }
            )
            .OfType<PointerEvent>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _toLogicalPixels(double physicalPixels, double devicePixelRatio) =>
        physicalPixels / devicePixelRatio;
}
