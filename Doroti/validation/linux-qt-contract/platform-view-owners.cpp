// Owner separation/queue lifecycle without a renderer. Link the production
// attachment implementation; no WebEngine dependency in this contract gate.
#include "doroti_qt_platform_views.h"
#include <QApplication>
#include <QWindow>
#include <QWidget>
#include <QPointer>
#include <QTimer>
#include <thread>
#include <cstdio>

int main(int argc, char** argv) {
  QApplication app(argc, argv);
  QWindow first, second;
  first.resize(300, 200); second.resize(300, 200);
  DorotiQtRegisterPlatformOwner(&first); DorotiQtRegisterPlatformOwner(&second);
  doroti_qt_pv_api api{};
  std::uint64_t a, b, control;
  bool ok = doroti_qt_get_platform_views(&first, 1, sizeof(api), &a, &api) == 0 &&
      doroti_qt_get_platform_views(&second, 1, sizeof(api), &b, &api) == 0 && a != b;
  ok &= api.create(a, 1, {}, nullptr, nullptr, &control) == 0;
  ok &= api.focus(b, control, 1) == DOROTI_QT_PV_STALE;
  ok &= api.remove(b, control) == DOROTI_QT_PV_STALE;
  auto* adopted = new QWidget;
  QPointer<QWidget> lifetime(adopted);
  std::uint64_t adopted_id;
  ok &= api.adopt_widget(b, adopted, nullptr, nullptr, &adopted_id) == 0;
  int rejected_callbacks = 0, executed_callbacks = 0;
  std::thread producer([&] {
    for (int i = 0; i < 100; ++i) {
      ok &= api.post(a, [](void* count, int status) { if (status == 71) ++*static_cast<int*>(count); }, &rejected_callbacks) == 0;
      ok &= api.post(b, [](void* count, int status) { if (status == 0) ++*static_cast<int*>(count); }, &executed_callbacks) == 0;
    }
  });
  producer.join();
  DorotiQtClosePlatformOwner(&first);
  ok &= rejected_callbacks == 100 && lifetime;
  DorotiQtRegisterPlatformOwner(&first);
  std::uint64_t reopened;
  ok &= doroti_qt_get_platform_views(&first, 1, sizeof(api), &reopened, &api) == 0 && reopened != a;
  ok &= api.post(a, [](void*, int) {}, nullptr) == DOROTI_QT_PV_CLOSED;
  QTimer::singleShot(100, &app, [&] {
    ok &= executed_callbacks == 100;
    DorotiQtClosePlatformOwner(&second);
    ok &= !lifetime;
    DorotiQtClosePlatformOwner(&first);
    std::printf("Qt owner separation, queue cancellation, reopen identity, adopted widget lifetime: %s\n", ok ? "PASS" : "FAIL");
    app.exit(ok ? 0 : 1);
  });
  return app.exec();
}
