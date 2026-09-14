#pragma once
#include "doroti_qt_host_v2.h"

// Optional owner attachment ABI for Widgets or Quick. All calls except post require the Qt GUI
// thread. IDs are process-monotonic, owner-scoped tokens, never QObject pointers.
// A post accepted with OK invokes its callback exactly once, including on close.
// A rejected post invokes no callback. Keep callback code/context loaded until
// the callback returns and keep adopted widget modules loaded until owner close.
extern "C" {
enum doroti_qt_pv_result : std::int32_t {
  DOROTI_QT_PV_CLOSED = 71,
  DOROTI_QT_PV_WRONG_THREAD = 72,
  DOROTI_QT_PV_STALE = 73,
  DOROTI_QT_PV_UNSUPPORTED = 74,
};
struct doroti_qt_pv_rect { double x, y, width, height; };
struct doroti_qt_pv_placement {
  std::uint32_t struct_size, visible;
  std::uint64_t id;
  doroti_qt_pv_rect bounds, clip;
};
struct doroti_qt_pv_api {
  std::uint32_t abi_version, struct_size;
  // bit 0: owner operations; bit 1: Quick interleaving; bit 2: WebEngine Quick;
  // bit 3: Quick live backdrop. Without bit 1 this is Widgets B only.
  std::uint64_t feature_bits;
  std::int32_t (*post)(std::uint64_t owner, void (*callback)(void*, std::int32_t), void* context);
  std::int32_t (*create)(std::uint64_t owner, std::uint32_t kind, doroti_qt_utf8_v2 text,
      void (*focused)(void*, std::uint64_t), void* context, std::uint64_t* id);
  // Transfers QWidget ownership only on success; widget must be parentless and
  // hidden. The shim creates it after QApplication on the GUI thread.
  std::int32_t (*adopt_widget)(std::uint64_t owner, void* widget,
      void (*focused)(void*, std::uint64_t), void* context, std::uint64_t* id);
  // Validates the ENTIRE owner scene before changing any widget. Omitted IDs hide.
  // Logical coordinates; only integer-aligned translation/rect clip is supported.
  std::int32_t (*commit)(std::uint64_t owner, const doroti_qt_pv_placement*, std::uint64_t count, std::uint32_t apply);
  std::int32_t (*focus)(std::uint64_t owner, std::uint64_t id, std::uint32_t focused);
  std::int32_t (*remove)(std::uint64_t owner, std::uint64_t id);
};
DOROTI_QT_EXPORT std::int32_t doroti_qt_get_platform_views(
    void* view, std::uint32_t version, std::uint32_t size,
    std::uint64_t* owner, doroti_qt_pv_api* api);
}
static_assert(sizeof(doroti_qt_pv_placement) == 80);
static_assert(sizeof(doroti_qt_pv_api) == 64);

#ifdef DOROTI_QT_HOST_BUILD
class QWindow;
void DorotiQtRegisterPlatformOwner(QWindow*);
void DorotiQtClosePlatformOwner(QWindow*);
void DorotiQtRecordPlatformOwner(QWindow*, const char* path);
#endif
