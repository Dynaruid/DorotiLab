// Minimal dart:ui surface for Doroti framework analyzer resolution.
library dart.ui;

typedef VoidCallback = void Function();
typedef HitTestCallback = HitTestResponse Function(HitTestRequest request);

enum Brightness { dark, light }

double clampDouble(double x, double min, double max) {
  assert(min <= max);
  if (x < min) {
    return min;
  }
  if (x > max) {
    return max;
  }
  return x;
}

class Offset {
  const Offset(this.dx, this.dy);
  final double dx;
  final double dy;
  static const Offset zero = Offset(0, 0);
}

class Size {
  const Size(this.width, this.height);
  final double width;
  final double height;
  static const Size zero = Size(0, 0);
}

class FlutterView {
  FlutterView({required this.viewId, this.devicePixelRatio = 1.0});
  final int viewId;
  final double devicePixelRatio;
}

class PointerDataPacket {
  const PointerDataPacket({this.data = const <PointerData>[]});
  final List<PointerData> data;
}

class PointerData {
  const PointerData();
}

/// Request that the framework hit-test [view] at [offset].
class HitTestRequest {
  const HitTestRequest({required this.view, required this.offset});
  final FlutterView view;
  final Offset offset;
}

/// Result of a [PlatformDispatcher.onHitTest] callback.
class HitTestResponse {
  const HitTestResponse({required this.hasPlatformView});
  static const HitTestResponse empty = HitTestResponse(hasPlatformView: false);
  final bool hasPlatformView;
}

class PlatformDispatcher {
  PlatformDispatcher._();
  static final PlatformDispatcher instance = PlatformDispatcher._();

  PointerDataPacketCallback? onPointerDataPacket;
  HitTestCallback? onHitTest;

  FlutterView? view({required int id}) => null;
}

typedef PointerDataPacketCallback = void Function(PointerDataPacket packet);
