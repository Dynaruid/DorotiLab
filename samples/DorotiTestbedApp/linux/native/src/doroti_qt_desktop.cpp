#include "doroti_qt_desktop.h"
#include <QApplication>
#include <QCloseEvent>
#include <QPointer>
#include <QThread>
#include <QTimer>
#include <QWindow>
#ifdef DOROTI_QT_QUICK
#include <QQuickWindow>
#endif
#include <cmath>
#include <map>
#include <memory>
#include <stdexcept>

namespace {
bool Gui() { return qApp && QThread::currentThread() == qApp->thread(); }
class DesktopWindow final : public QObject {
 public:
  QPointer<QWindow> window;
  std::function<void()> destroy;
  void (*callback)(void*, std::uint32_t) = nullptr;
  void* context = nullptr;
  QSize minimum{0, 0}, maximum{16777215, 16777215};
  bool resizable = true, closing = false;
  Qt::WindowState observed_state = Qt::WindowNoState;
  DesktopWindow(QWindow* value, std::function<void()> action)
      : window(value), destroy(std::move(action)) {}
  void Changed() { if (callback && !closing) callback(context, 0); }
  void Limits() {
    // Clear the old pair first: restoring from fixed size must not clamp against
    // the previous pair while applying the new constraints.
    window->setMinimumSize(QSize(0, 0));
    window->setMaximumSize(QSize(16777215, 16777215));
    window->setMinimumSize(resizable ? minimum : window->size());
    window->setMaximumSize(resizable ? maximum : window->size());
#ifdef DOROTI_QT_QUICK
    // Wayland size hints take effect at a surface commit. An idle framework
    // scene must still let Qt commit changed hints before a later state request.
    if (auto* quick = qobject_cast<QQuickWindow*>(window.data())) quick->update();
#endif
  }
 protected:
  bool eventFilter(QObject*, QEvent* event) override {
#if QT_VERSION >= QT_VERSION_CHECK(6, 6, 0)
    if (event->type() == QEvent::DevicePixelRatioChange) Changed();
#endif
    if (event->type() == QEvent::WindowStateChange) {
      // QWindow setters also emit windowStateChanged for requests. Only the
      // platform's WindowStateChange event confirms the observed state.
      observed_state = window->windowState();
      Changed();
      return false;
    }
    if (event->type() != QEvent::Close || !callback) return false;
    static_cast<QCloseEvent*>(event)->ignore();
    if (!closing) callback(context, 1);
    return true;
  }
};
std::map<std::uint64_t, std::unique_ptr<DesktopWindow>> windows;
DesktopWindow* Find(std::uint64_t owner) {
  auto it = windows.find(owner);
  return it == windows.end() ? nullptr : it->second.get();
}
int Snapshot(std::uint64_t owner, doroti_qt_desktop_state* state) {
  if (!Gui()) return DOROTI_QT_PV_WRONG_THREAD;
  auto* value = Find(owner);
  if (!value || !value->window || value->closing) return DOROTI_QT_PV_CLOSED;
  if (!state || state->struct_size != sizeof(*state)) return DOROTI_QT_ERROR_INVALID_ARGUMENT;
  auto* w = value->window.data();
  *state = {sizeof(*state), (w->isVisible() ? 1u : 0u) | (w->isActive() ? 2u : 0u),
            double(w->width()), double(w->height()), w->devicePixelRatio(),
            value->observed_state == Qt::WindowFullScreen ? 3u :
            value->observed_state == Qt::WindowMaximized ? 2u :
            value->observed_state == Qt::WindowMinimized ? 1u : 0u, 0};
  return 0;
}
int Observe(std::uint64_t owner, void (*callback)(void*, std::uint32_t), void* context) {
  if (!Gui()) return DOROTI_QT_PV_WRONG_THREAD;
  auto* value = Find(owner);
  if (!value || !value->window || value->closing) return DOROTI_QT_PV_CLOSED;
  if (!callback || value->callback) return DOROTI_QT_ERROR_INVALID_ARGUMENT;
  value->callback = callback;
  value->context = context;
  value->window->installEventFilter(value);
  // The Desktop manager decides after native teardown and registry removal.
  qApp->setQuitOnLastWindowClosed(false);
  return 0;
}
bool Size(double x, double y, bool empty = false) {
  return std::isfinite(x) && std::isfinite(y) && x == std::floor(x) && y == std::floor(y)
      && x >= (empty ? 0 : 1) && y >= (empty ? 0 : 1) && x <= 16777215 && y <= 16777215;
}
int Command(std::uint64_t owner, const doroti_qt_desktop_command* command) {
  if (!Gui()) return DOROTI_QT_PV_WRONG_THREAD;
  auto* value = Find(owner);
  if (!value || !value->window || value->closing) return DOROTI_QT_PV_CLOSED;
  if (!value->callback || !command || command->struct_size != sizeof(*command))
    return DOROTI_QT_ERROR_INVALID_ARGUMENT;
  try {
    auto* w = value->window.data();
    switch (command->kind) {
      case 0: w->show(); break;
      case 1: w->hide(); break;
      case 2: w->requestActivate(); break;
      case 3:
        if (!Size(command->x, command->y) || command->x < value->minimum.width()
            || command->y < value->minimum.height() || command->x > value->maximum.width()
            || command->y > value->maximum.height()) return DOROTI_QT_ERROR_INVALID_ARGUMENT;
        if (!value->resizable) {
          w->setMinimumSize(QSize(0, 0)); w->setMaximumSize(QSize(16777215, 16777215));
        }
        w->resize(int(command->x), int(command->y)); value->Limits(); break;
      case 6:
      case 7: {
        if (!Size(command->x, command->y, true)) return DOROTI_QT_ERROR_INVALID_ARGUMENT;
        const QSize size(int(command->x), int(command->y));
        if (command->kind == 6) {
          if (size.width() > value->maximum.width() || size.height() > value->maximum.height())
            return DOROTI_QT_ERROR_INVALID_ARGUMENT;
          value->minimum = size;
        } else {
          if (size.width() < value->minimum.width() || size.height() < value->minimum.height())
            return DOROTI_QT_ERROR_INVALID_ARGUMENT;
          value->maximum = size;
        }
        value->Limits(); break;
      }
      case 8: case 9:
        return DOROTI_QT_PV_UNSUPPORTED;
      case 10:
        if (command->x != 0 && command->x != 1) return DOROTI_QT_ERROR_INVALID_ARGUMENT;
        value->resizable = command->x != 0; value->Limits(); break;
      case 11:
        // xdg-shell does not acknowledge a minimized state. Qt can report a
        // later Normal configure after this request, so do not promise an
        // observable minimize operation on native Wayland.
        if (QGuiApplication::platformName().startsWith("wayland")) return DOROTI_QT_PV_UNSUPPORTED;
        w->showMinimized(); break;
      case 12: w->showMaximized(); break;
      case 13: w->showNormal(); break;
      case 14:
        if (command->x != 0 && command->x != 1) return DOROTI_QT_ERROR_INVALID_ARGUMENT;
        if (command->x != 0) w->showFullScreen(); else w->showNormal(); break;
      case 15:
        if (command->text.length > 1048576 || (command->text.length && !command->text.data))
          return DOROTI_QT_ERROR_INVALID_ARGUMENT;
        w->setTitle(QString::fromUtf8(reinterpret_cast<const char*>(command->text.data),
                                     qsizetype(command->text.length))); break;
      case 100:
        value->closing = true;
        w->hide();
        QTimer::singleShot(0, qApp, [owner] {
          auto* item = Find(owner);
          if (!item) return;
          auto* window = item->window.data();
          // Copy: destruction completion erases the object holding this function.
          auto destroy = item->destroy;
          destroy();
          DorotiQtReleaseDesktopWindow(window);
        });
        return 0;
      default: return DOROTI_QT_PV_UNSUPPORTED;
      case 102:
        if (!std::isfinite(command->x) || command->x < 0 || command->x > 3 || command->x != std::floor(command->x))
          return DOROTI_QT_ERROR_INVALID_ARGUMENT;
        w->setWindowState(command->x == 3 ? Qt::WindowFullScreen : command->x == 2 ? Qt::WindowMaximized
            : command->x == 1 ? Qt::WindowMinimized : Qt::WindowNoState);
        break;
    }
    value->Changed();
    return 0;
  } catch (...) { return DOROTI_QT_ERROR_NATIVE_EXCEPTION; }
}
}

void DorotiQtRegisterDesktopWindow(QWindow* window, std::function<void()> destroy) {
#ifdef DOROTI_QT_QUICK
  doroti_qt_pv_api pv{};
  std::uint64_t owner = 0;
  if (doroti_qt_get_platform_views(window, 1, sizeof(pv), &owner, &pv) != 0)
    throw std::runtime_error("Qt Desktop owner registration failed");
  auto item = std::make_unique<DesktopWindow>(window, std::move(destroy));
  auto* value = item.get();
  auto changed = [value] { value->Changed(); };
  QObject::connect(window, &QWindow::visibleChanged, value, changed);
  QObject::connect(window, &QWindow::activeChanged, value, changed);
  QObject::connect(window, &QWindow::widthChanged, value, changed);
  QObject::connect(window, &QWindow::heightChanged, value, changed);
  QObject::connect(window, &QWindow::windowStateChanged, value, changed);
  QObject::connect(window, &QWindow::screenChanged, value, changed);
  windows.emplace(owner, std::move(item));
#endif
}
void DorotiQtReleaseDesktopWindow(QWindow* window) {
  // QPointer is already cleared after destruction; there is one product window.
  // Non-null entries belong to a still-live window and must not be released.
  for (auto it = windows.begin(); it != windows.end();) {
    auto& item = it->second;
    if (item->window && item->window != window) { ++it; continue; }
    auto callback = item->callback;
    auto context = item->context;
    it = windows.erase(it);
    if (callback) callback(context, 2);
  }
}
extern "C" std::int32_t doroti_qt_get_desktop(void* window, std::uint32_t version,
    std::uint32_t size, std::uint64_t* owner, doroti_qt_desktop_api* api) {
  if (!Gui()) return DOROTI_QT_PV_WRONG_THREAD;
  if (!owner || !api || version != 1 || size != sizeof(*api)) return DOROTI_QT_ERROR_INVALID_ARGUMENT;
  doroti_qt_pv_api pv{};
  const auto status = doroti_qt_get_platform_views(window, 1, sizeof(pv), owner, &pv);
  if (status != 0) return status;
  if (!(pv.feature_bits & 2) || !Find(*owner)) return DOROTI_QT_PV_UNSUPPORTED;
  *api = {1, sizeof(*api), 1, pv.post, Observe, Command, Snapshot};
  return 0;
}
extern "C" void doroti_qt_desktop_quit() { QCoreApplication::quit(); }
