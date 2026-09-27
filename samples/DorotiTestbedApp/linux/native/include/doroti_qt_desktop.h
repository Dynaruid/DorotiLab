#pragma once
#include "doroti_qt_platform_views.h"

// Optional Desktop ABI, independent of the unchanged host ABI 4. GUI-thread
// commands use a generation token; post has the platform-owner exactly-once contract.
extern "C" {
struct doroti_qt_desktop_state {
  std::uint32_t struct_size, flags; // visible=1, active=2
  double width, height, scale;
  std::uint32_t presentation, reserved; // normal/minimized/maximized/fullscreen
};
struct doroti_qt_desktop_command {
  std::uint32_t struct_size, kind;
  double x, y;
  doroti_qt_utf8_v2 text;
};
struct doroti_qt_desktop_api {
  std::uint32_t abi_version, struct_size;
  std::uint64_t feature_bits; // bit 0: basic Quick window and cancellable native close
  std::int32_t (*post)(std::uint64_t, void (*)(void*, std::int32_t), void*);
  std::int32_t (*observe)(std::uint64_t, void (*)(void*, std::uint32_t), void*);
  std::int32_t (*command)(std::uint64_t, const doroti_qt_desktop_command*);
  std::int32_t (*snapshot)(std::uint64_t, doroti_qt_desktop_state*);
};
// Events: 0 state changed, 1 close requested (deferred), 2 destruction finished.
// Commands 0..17 match WindowCommandKind; 100 destroys an approved window,
// 102 sets the initial presentation before showing it.
// Observe only once, before first show. Context survives through event 2.
DOROTI_QT_EXPORT std::int32_t doroti_qt_get_desktop(
    void*, std::uint32_t, std::uint32_t, std::uint64_t*, doroti_qt_desktop_api*);
DOROTI_QT_EXPORT void doroti_qt_desktop_quit();
}
static_assert(sizeof(doroti_qt_desktop_state) == 40);
static_assert(sizeof(doroti_qt_desktop_command) == 40);
static_assert(sizeof(doroti_qt_desktop_api) == 48);

#ifdef DOROTI_QT_HOST_BUILD
#include <functional>
class QWindow;
void DorotiQtRegisterDesktopWindow(QWindow*, std::function<void()> destroy);
void DorotiQtReleaseDesktopWindow(QWindow*);
#endif
