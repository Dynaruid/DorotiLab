// Exercise the production event handler without a renderer or visible window.
#include "../../../DorotiTestbedApp/linux/native/src/doroti_qt_host.cpp"
#include <cstdio>
#include <vector>

int main(int argc, char** argv) {
  QApplication application(argc, argv);
  std::vector<doroti_qt_pointer_v2> packets;
  doroti_qt_callbacks_v2 cb{};
  cb.pointer = [](void* state, void*, const doroti_qt_pointer_v2* data) {
    static_cast<std::vector<doroti_qt_pointer_v2>*>(state)->push_back(*data);
  };
  cb.focus = [](void*, void*, std::uint32_t, std::int64_t) {};
  cb.closed = [](void*, void*) {};
  cb.lifecycle_changed = [](void*, void*, std::uint32_t, std::int64_t) {};
  cb.surface_destroying = [](void*, void*, std::uint64_t, std::uint64_t) {};
  cb.metrics_changed = [](void*, void*, const doroti_qt_metrics_v2*) {};
  DorotiSurface window(&packets, cb, 0, 0, 0);
  const QPointingDevice pad("fixture trackpad", 77, QInputDevice::DeviceType::TouchPad,
      QPointingDevice::PointerType::Finger, QInputDevice::Capability::Position, 5, 0);
  const auto check = [](bool condition, const char* message) {
    if (!condition) throw std::runtime_error(message);
  };
  auto wheel = [&](QPoint delta, Qt::ScrollPhase phase) {
    QWheelEvent event({40, 60}, {40, 60}, delta, {}, Qt::NoButton, Qt::NoModifier,
        phase, false, Qt::MouseEventNotSynthesized, &pad);
    QCoreApplication::sendEvent(&window, &event);
  };
  auto native = [&](Qt::NativeGestureType kind, double value, QPointF delta = {}) {
    QNativeGestureEvent event(kind, &pad, 2, {40, 60}, {40, 60}, {40, 60}, value, delta);
    QCoreApplication::sendEvent(&window, &event);
  };
  wheel({}, Qt::ScrollBegin);
  wheel({6, 12}, Qt::ScrollUpdate);
  const auto first = packets.back().pointer_identifier;
  const auto ratio = window.devicePixelRatio();
  check(packets.back().kind == 4 && packets.back().change == 8 &&
      packets.back().pan_y == 12 * ratio && packets.back().pan_delta_y == 12 * ratio,
      "Qt continuous scroll lost kind or physical pan");
  wheel({}, Qt::ScrollEnd);
  check(packets.back().change == 9, "Qt scroll did not end");
  const auto count = packets.size();
  wheel({3, 4}, Qt::ScrollMomentum);
  check(packets.size() == count, "Qt OS inertia duplicated framework physics");
  native(Qt::BeginNativeGesture, 0);
  check(packets.back().change == 7 && packets.back().pointer_identifier != first,
      "Qt next gesture did not get a new pointer");
  check(packets[count].signal_kind == 2, "Qt interrupted momentum did not cancel inertia");
  native(Qt::ZoomNativeGesture, .25);
  native(Qt::RotateNativeGesture, 90);
  native(Qt::PanNativeGesture, 0, {2, 3});
  check(packets.back().struct_size == 168 && packets.back().scale == 1.25 &&
      std::abs(packets.back().rotation - 3.141592653589793 / 2) < 1e-9 &&
      packets.back().pan_x == 2 * ratio, "Qt native gesture transform was lost");
  native(Qt::EndNativeGesture, 0);
  wheel({1, 2}, Qt::NoScrollPhase);
  QEvent deactivate(QEvent::WindowDeactivate);
  QCoreApplication::sendEvent(&window, &deactivate);
  check(packets[packets.size() - 2].change == 9 && packets.back().change == 2,
      "Qt deactivation did not terminate and remove the trackpad");
  std::printf("PASS: production Qt wheel/native gesture input, pan/zoom/rotation ABI, inertia suppression/cancel and deactivation; packets=%zu\n", packets.size());
  return 0;
}
