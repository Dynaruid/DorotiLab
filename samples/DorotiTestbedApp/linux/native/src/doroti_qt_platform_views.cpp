#include "doroti_qt_platform_views.h"
#include <QApplication>
#include <QFile>
#include <QJsonArray>
#include <QJsonDocument>
#include <QJsonObject>
#include <QScreen>
#include <QKeyEvent>
#include <QLineEdit>
#include <QPushButton>
#include <QPointer>
#include <QThread>
#include <QWindow>
#include <QWidget>
#include <algorithm>
#include <cmath>
#include <map>
#include <memory>
#include <mutex>
#include <set>
#include <vector>

namespace {
// Qt can recreate a top-level QWidget's native window when a child starts RHI
// rendering (e.g. WebEngine's internal Quick widget). Rebind the new window to
// its owner before it becomes visible; it must never escape as a popup.
class ClipWidget final : public QWidget {
  QPointer<QWindow> owner_;
  bool rebinding_ = false;
 public:
  explicit ClipWidget(QWindow* owner) : owner_(owner) {}
 protected:
  bool event(QEvent* event) override {
    const auto result = QWidget::event(event);
    if (event->type() == QEvent::WinIdChange && owner_ && windowHandle() && !rebinding_) {
      rebinding_ = true;
      windowHandle()->setParent(owner_);
      rebinding_ = false;
    }
    return result;
  }
};
struct Control {
  QPointer<QWidget> clip, widget;
  void (*focused)(void*, std::uint64_t);
  void* context;
};
struct Pending { void (*callback)(void*, std::int32_t); void* context; };
struct Owner {
  QWindow* window;
  std::map<std::uint64_t, Control> controls;
  std::map<std::uint64_t, Pending> pending;
  QMetaObject::Connection focus_connection;
};
std::mutex gate;
std::map<std::uint64_t, std::unique_ptr<Owner>> owners;
std::uint64_t next_id = 0;
bool Gui() { return qApp && QThread::currentThread() == qApp->thread(); }
Owner* Find(std::uint64_t id) {
  auto found = owners.find(id);
  return found == owners.end() ? nullptr : found->second.get();
}
// GUI-only calls cannot race owner teardown, which also runs on that thread.
int Lookup(std::uint64_t id, Owner*& owner) {
  if (!Gui()) return DOROTI_QT_PV_WRONG_THREAD;
  std::lock_guard lock(gate);
  owner = Find(id);
  return owner ? int(DOROTI_QT_OK) : int(DOROTI_QT_PV_CLOSED);
}
int Post(std::uint64_t id, void (*callback)(void*, int), void* context) {
  if (!callback) return DOROTI_QT_ERROR_INVALID_ARGUMENT;
  std::lock_guard lock(gate);
  auto* owner = Find(id);
  if (!owner) return DOROTI_QT_PV_CLOSED;
  auto task = ++next_id;
  owner->pending.emplace(task, Pending{callback, context});
  bool posted;
  try { posted = QMetaObject::invokeMethod(owner->window, [id, task] {
    Pending pending{};
    {
      std::lock_guard lock(gate);
      auto* owner = Find(id);
      if (!owner) return;
      auto it = owner->pending.find(task);
      if (it == owner->pending.end()) return;
      pending = it->second;
      owner->pending.erase(it);
    }
    pending.callback(pending.context, DOROTI_QT_OK);
  }, Qt::QueuedConnection); }
  catch (...) { owner->pending.erase(task); throw; }
  if (!posted) { owner->pending.erase(task); return DOROTI_QT_PV_CLOSED; }
  return DOROTI_QT_OK;
}
int Adopt(std::uint64_t id, void* pointer, void (*focused)(void*, std::uint64_t),
          void* context, std::uint64_t* result) {
  Owner* owner; auto status = Lookup(id, owner); if (status) return status;
  auto* widget = static_cast<QWidget*>(pointer);
  if (!widget || !result || widget->thread() != qApp->thread() ||
      widget->parent() || !widget->isHidden()) return DOROTI_QT_ERROR_INVALID_ARGUMENT;
  auto clip = std::make_unique<ClipWidget>(owner->window);
  clip->setObjectName("doroti-platform-clip");
  clip->setAttribute(Qt::WA_NativeWindow);
  clip->winId();
  clip->windowHandle()->setParent(owner->window);
  std::uint64_t token;
  { std::lock_guard lock(gate); token = ++next_id; }
  owner->controls.emplace(token, Control{clip.get(), widget, focused, context});
  try { widget->setParent(clip.get()); widget->show(); }
  catch (...) {
    owner->controls.erase(token);
    widget->hide(); widget->setParent(nullptr);
    throw;
  }
  clip.release();
  *result = token;
  return DOROTI_QT_OK;
}
int Create(std::uint64_t owner, std::uint32_t kind, doroti_qt_utf8_v2 text,
           void (*focused)(void*, std::uint64_t), void* context, std::uint64_t* result) {
  Owner* found; auto status = Lookup(owner, found); if (status) return status;
  if (kind > 1 || text.length > 1024 * 1024 || (text.length && !text.data))
    return DOROTI_QT_ERROR_INVALID_ARGUMENT;
  const auto label = QString::fromUtf8(reinterpret_cast<const char*>(text.data), text.length);
  std::unique_ptr<QWidget> widget(kind ? static_cast<QWidget*>(new QLineEdit(label)) : new QPushButton(label));
  status = Adopt(owner, widget.get(), focused, context, result);
  if (!status) widget.release();
  return status;
}
bool Valid(doroti_qt_pv_rect r) {
  for (auto value : {r.x, r.y, r.width, r.height})
    if (!std::isfinite(value) || std::abs(value) > 1e6 || std::floor(value) != value) return false;
  return r.width >= 0 && r.height >= 0;
}
QRect Rect(doroti_qt_pv_rect r) { return {int(r.x), int(r.y), int(r.width), int(r.height)}; }
void Hide(Owner& owner, Control& control) {
  auto* focus = QApplication::focusWidget();
  const bool had_focus = control.widget && focus &&
      (focus == control.widget || control.widget->isAncestorOf(focus));
  if (control.clip) control.clip->hide();
  if (had_focus) { focus->clearFocus(); owner.window->requestActivate(); }
}
int Commit(std::uint64_t id, const doroti_qt_pv_placement* items, std::uint64_t count, std::uint32_t apply) {
  Owner* owner; auto status = Lookup(id, owner); if (status) return status;
  if (count > 256 || (count && !items)) return DOROTI_QT_ERROR_INVALID_ARGUMENT;
  std::set<std::uint64_t> seen;
  std::vector<QRect> visible;
  for (std::uint64_t i = 0; i < count; ++i) {
    const auto& item = items[i];
    if (item.struct_size != sizeof(item) || item.visible > 1 || !Valid(item.bounds) || !Valid(item.clip))
      return DOROTI_QT_ERROR_INVALID_ARGUMENT;
    auto it = owner->controls.find(item.id);
    if (it == owner->controls.end() || !it->second.widget || !it->second.clip ||
        it->second.clip->windowHandle()->parent() != owner->window || !seen.insert(item.id).second)
      return DOROTI_QT_PV_STALE;
    auto rect = Rect(item.bounds).intersected(Rect(item.clip)).intersected(QRect(QPoint(), owner->window->size()));
    if (!item.visible || rect.isEmpty()) continue;
    for (auto other : visible) if (other.intersects(rect)) return DOROTI_QT_PV_UNSUPPORTED;
    visible.push_back(rect);
  }
  if (!apply) return DOROTI_QT_OK;
  // No callbacks/events are pumped between validation and this GUI-thread batch.
  for (auto& [token, control] : owner->controls) if (!seen.contains(token)) Hide(*owner, control);
  for (std::uint64_t i = 0; i < count; ++i) {
    const auto& item = items[i];
    auto& control = owner->controls.at(item.id);
    auto bounds = Rect(item.bounds);
    auto clip = bounds.intersected(Rect(item.clip)).intersected(QRect(QPoint(), owner->window->size()));
    if (!item.visible || clip.isEmpty()) { Hide(*owner, control); continue; }
    control.clip->setGeometry(clip);
    control.widget->setGeometry(QRect(bounds.topLeft() - clip.topLeft(), bounds.size()));
    control.clip->show();
  }
  return DOROTI_QT_OK;
}
int Focus(std::uint64_t id, std::uint64_t token, std::uint32_t focused) {
  Owner* owner; auto status = Lookup(id, owner); if (status) return status;
  auto it = owner->controls.find(token);
  if (it == owner->controls.end() || !it->second.widget) return DOROTI_QT_PV_STALE;
  auto& control = it->second;
  if (focused) {
    if (!control.clip || !control.clip->isVisible()) return DOROTI_QT_PV_UNSUPPORTED;
    control.clip->activateWindow(); control.widget->setFocus(Qt::OtherFocusReason);
  } else if (control.widget->hasFocus()) {
    control.widget->clearFocus(); owner->window->requestActivate();
  }
  return DOROTI_QT_OK;
}
int Remove(std::uint64_t id, std::uint64_t token) {
  Owner* owner; auto status = Lookup(id, owner); if (status) return status;
  auto it = owner->controls.find(token);
  if (it == owner->controls.end()) return DOROTI_QT_PV_STALE;
  auto control = it->second;
  owner->controls.erase(it); // Reject callbacks before QObject destruction.
  Hide(*owner, control);
  delete control.clip.data();
  return DOROTI_QT_OK;
}
template<auto> struct Boundary;
template<typename... Args, int (*Function)(Args...)> struct Boundary<Function> {
  static int Call(Args... args) noexcept {
    try { return Function(args...); } catch (...) { return DOROTI_QT_ERROR_NATIVE_EXCEPTION; }
  }
};
const doroti_qt_pv_api api{1, sizeof(doroti_qt_pv_api), 1,
    Boundary<Post>::Call, Boundary<Create>::Call, Boundary<Adopt>::Call,
    Boundary<Commit>::Call, Boundary<Focus>::Call, Boundary<Remove>::Call};
}

void DorotiQtRegisterPlatformOwner(QWindow* window) {
  auto owner = std::make_unique<Owner>(); owner->window = window;
  std::uint64_t id;
  { std::lock_guard lock(gate); id = ++next_id; owners.emplace(id, std::move(owner)); }
  Find(id)->focus_connection = QObject::connect(qApp, &QApplication::focusChanged, window,
      [id](QWidget*, QWidget* focused) {
    auto* owner = Find(id); if (!owner || !focused) return;
    for (const auto& [token, control] : owner->controls)
      if (control.widget && (focused == control.widget || control.widget->isAncestorOf(focused))) {
        // Never reenter managed framework code while applying a widget batch.
        QMetaObject::invokeMethod(owner->window, [id, token] {
          auto* current = Find(id); if (!current) return;
          auto it = current->controls.find(token);
          if (it != current->controls.end() && it->second.focused)
            it->second.focused(it->second.context, token);
        }, Qt::QueuedConnection);
        break;
      }
  });
}
void DorotiQtClosePlatformOwner(QWindow* window) {
  std::unique_ptr<Owner> owner;
  {
    std::lock_guard lock(gate);
    for (auto it = owners.begin(); it != owners.end(); ++it)
      if (it->second->window == window) { owner = std::move(it->second); owners.erase(it); break; }
  }
  if (!owner) return;
  QObject::disconnect(owner->focus_connection);
  for (auto& [id, pending] : owner->pending) pending.callback(pending.context, DOROTI_QT_PV_CLOSED);
  for (auto& [id, control] : owner->controls) delete control.clip.data();
}
extern "C" DOROTI_QT_EXPORT int doroti_qt_get_platform_views(void* window, std::uint32_t version,
    std::uint32_t size, std::uint64_t* result, doroti_qt_pv_api* output) {
  if (!Gui()) return DOROTI_QT_PV_WRONG_THREAD;
  if (version != 1) return DOROTI_QT_ERROR_ABI_VERSION;
  if (size != sizeof(api)) return DOROTI_QT_ERROR_ABI_SIZE;
  if (!result || !output) return DOROTI_QT_ERROR_INVALID_ARGUMENT;
  std::lock_guard lock(gate);
  for (const auto& [id, owner] : owners) if (owner->window == window) {
    *result = id; *output = api; return DOROTI_QT_OK;
  }
  return DOROTI_QT_PV_CLOSED;
}

// Explicit validation hook. Window pixels include native children on xcb; a null
// Wayland grab is recorded as unavailable, never replaced by QWidget::render().
void DorotiQtRecordPlatformOwner(QWindow* window, const char* path) {
  if (!path || !*path) return;
  QJsonArray controls;
  for (const auto& [owner_id, owner] : owners) if (owner->window == window) {
    for (const auto& [id, control] : owner->controls) {
      if (!control.widget || !control.clip) continue;
      auto* editor = qobject_cast<QLineEdit*>(control.widget);
      if (editor) {
        QKeyEvent press(QEvent::KeyPress, Qt::Key_End, Qt::NoModifier);
        QCoreApplication::sendEvent(editor, &press);
        QKeyEvent text(QEvent::KeyPress, Qt::Key_Exclam, Qt::NoModifier, "!");
        QCoreApplication::sendEvent(editor, &text);
      }
      const auto rect = control.clip->geometry();
      controls.append(QJsonObject{{"id", qint64(id)}, {"visible", control.clip->isVisible()},
          {"parentMatches", control.clip->windowHandle()->parent() == window},
          {"x", rect.x()}, {"y", rect.y()}, {"width", rect.width()}, {"height", rect.height()},
          {"kind", editor ? "QLineEdit" : "QPushButton"}, {"text", editor ? editor->text() : QString()}});
    }
  }
  const auto capture = window->screen()->grabWindow(window->winId());
  bool saved = !capture.isNull() && capture.save(QString::fromUtf8(path) + ".png");
  QJsonObject report{{"schemaVersion", "doroti.qt-platform-view-product/v1"},
      {"qpa", QGuiApplication::platformName()}, {"qt", qVersion()},
      {"dpr", window->devicePixelRatio()}, {"windowCapture", saved}, {"controls", controls},
      {"composition", "disjoint-native-child-widgets-B"}, {"synchronizedPlacement", false}};
  QFile output(QString::fromUtf8(path) + ".json");
  if (output.open(QIODevice::WriteOnly)) output.write(QJsonDocument(report).toJson());
}
