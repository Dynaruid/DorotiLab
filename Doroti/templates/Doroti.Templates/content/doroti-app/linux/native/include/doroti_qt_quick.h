#pragma once
#include "doroti_qt_platform_views.h"
struct doroti_qt_quick_gpu {
  std::uint32_t version, size;
  void *instance, *physical_device, *device, *queue;
  std::uint32_t queue_family, api_version;
};
// kind: raster=0, native=1, shield=2, backdrop=3, color-backdrop=4. Array order is paint order.
// PV feature bit 3 negotiates backdrop: id contains IEEE754 logical sigma bits,
// bounds is the output clip, clip is bounds expanded by 3*sigma for sampling.
// One effect, isotropic sigma <=32, <=4096 per physical sample dimension and
// <=4M sample pixels. Earlier items alone form its live source group.
// PV feature bit 4 negotiates kind 4: image carries IEEE754 saturation [0,2]; sigma zero is valid.
// PV feature bit 2 negotiates create kind 2: UTF-8 initial WebEngine Quick HTML
// or versioned WebView options. Independent WebView ABI 1 provides commands.
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
