#pragma once
#include "doroti_qt_platform_views.h"
struct doroti_qt_quick_gpu {
  std::uint32_t version, size;
  void *instance, *physical_device, *device, *queue;
  std::uint32_t queue_family, api_version;
};
// kind: raster=0, native=1, shield=2. Array order is the scene paint order.
struct doroti_qt_quick_part {
  std::uint32_t size, kind;
  std::uint64_t id, image;
  std::uint32_t pixel_width, pixel_height;
  doroti_qt_pv_rect bounds, clip;
};
static_assert(sizeof(doroti_qt_quick_gpu)==48);
static_assert(sizeof(doroti_qt_quick_part)==96);
extern "C" {
DOROTI_QT_EXPORT int doroti_qt_quick_get_gpu(void* window, doroti_qt_quick_gpu* gpu);
DOROTI_QT_EXPORT int doroti_qt_quick_commit(void* window,const doroti_qt_quick_part* parts,std::uint64_t count,std::uint32_t apply);
}
#ifdef DOROTI_QT_QUICK
#include <QEvent>
#include <QWindow>
bool DorotiQtQuickNativeInput(QWindow* window,QEvent* event);
bool DorotiQtQuickHasNativeFocus(QWindow* window);
void DorotiQtQuickClearFocus(QWindow* window);
#endif
