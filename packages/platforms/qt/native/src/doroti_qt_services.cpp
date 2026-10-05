#include "doroti_qt_services.h"
#include <QApplication>
#include <QDialog>
#include <QDropEvent>
#include <QDrag>
#include <QPixmap>
#include <QTimer>
#include <QFileDialog>
#include <QJsonArray>
#include <QJsonDocument>
#include <QJsonObject>
#include <QMimeData>
#include <QPointer>
#include <QThread>
#include <QWindow>
#include <map>
#include <memory>
#include <sys/stat.h>
#include <fcntl.h>
#include <unistd.h>
#include <cerrno>

namespace {
using Reply = void (*)(void*, int, doroti_qt_utf8_v2);
using Drop = int (*)(void*, doroti_qt_utf8_v2);
doroti_qt_utf8_v2 Utf8(const QByteArray& bytes) {
  return {reinterpret_cast<const std::uint8_t*>(bytes.constData()), std::uint64_t(bytes.size())};
}
bool Gui() { return qApp && QThread::currentThread() == qApp->thread(); }
struct Picker : QObject {
  QPointer<QFileDialog> dialog;
  Reply reply; void* context; bool completed = false;
  void Complete(int status, const QByteArray& paths = "[]") {
    if (completed) return;
    completed = true;
    reply(context, status, Utf8(paths));
    if (dialog) { dialog->hide(); dialog->deleteLater(); }
    deleteLater();
  }
  ~Picker() override { if (!completed) reply(context, 1, Utf8("[]")); if (dialog) delete dialog.data(); }
};
class Receiver : public QObject {
 public:
  Drop callback; void* context;
  Receiver(QObject* owner, Drop cb, void* ctx) : QObject(owner), callback(cb), context(ctx) {}
 protected:
  bool eventFilter(QObject*, QEvent* event) override {
    const auto type = event->type();
    if (type != QEvent::DragEnter && type != QEvent::DragMove && type != QEvent::DragLeave && type != QEvent::Drop) return false;
    QJsonObject value{{"phase", type == QEvent::DragEnter ? 0 : type == QEvent::DragMove ? 1 : type == QEvent::DragLeave ? 2 : 3}};
    if (type == QEvent::DragLeave) { callback(context, Utf8(QJsonDocument(value).toJson(QJsonDocument::Compact))); return false; }
    auto* drop = static_cast<QDropEvent*>(event);
    auto* mime = drop->mimeData();
    QJsonArray paths, uris, formats;
    if (mime->hasUrls()) {
      for (const auto& url : mime->urls()) {
        if (url.isLocalFile()) paths.append(url.toLocalFile());
        if (url.isValid()) uris.append(url.toString(QUrl::FullyEncoded));
      }
      if (!paths.isEmpty()) formats.append("application/x-doroti-files");
      formats.append("text/uri-list");
    }
    if (mime->hasText()) formats.append("text/plain");
    value.insert("x", drop->position().x()); value.insert("y", drop->position().y());
    value.insert("copy", bool(drop->possibleActions() & Qt::CopyAction));
    value.insert("formats", formats);
    // File contents are never loaded into the event payload.
    if (type == QEvent::Drop) {
      value.insert("paths", paths); value.insert("uris", uris);
      value.insert("text", mime->hasText() ? mime->text() : QString());
    }
    const auto result = callback(context, Utf8(QJsonDocument(value).toJson(QJsonDocument::Compact)));
    if (result == 1) { drop->setDropAction(Qt::CopyAction); drop->accept(); return true; }
    drop->ignore();
    return false; // native controls retain their own drop policy
  }
};
std::map<void*, QPointer<Receiver>> receivers;
std::map<std::uint64_t, QPointer<Picker>> pickers;
}

extern "C" DOROTI_QT_EXPORT int doroti_qt_pick_files_v1(void* owner, std::uint64_t id,
    doroti_qt_utf8_v2 options, Reply callback, void* context) {
  if (!Gui() || !owner || !callback || options.length > 65536) return 66;
  auto* window = static_cast<QWindow*>(owner);
  if (!QGuiApplication::allWindows().contains(window) || pickers.contains(id)) return 71;
  const auto json = QJsonDocument::fromJson(QByteArray(reinterpret_cast<const char*>(options.data), qsizetype(options.length)));
  if (!json.isObject()) return 66;
  auto* picker = new Picker; picker->setParent(window); picker->reply = callback; picker->context = context;
  auto* dialog = new QFileDialog; picker->dialog = dialog;
  dialog->setAttribute(Qt::WA_DeleteOnClose, false);
  dialog->setFileMode(json.object().value("multiple").toBool() ? QFileDialog::ExistingFiles : QFileDialog::ExistingFile);
  dialog->setAcceptMode(QFileDialog::AcceptOpen);
  QStringList patterns;
  for (const auto& extension : json.object().value("extensions").toArray())
    patterns << (extension.toString() == "*" ? "*" : "*" + extension.toString());
  if (!patterns.isEmpty()) dialog->setNameFilter("Files (" + patterns.join(' ') + ")");
  dialog->winId(); dialog->windowHandle()->setTransientParent(window);
  dialog->setWindowModality(Qt::WindowModal);
  pickers.emplace(id, picker);
  QObject::connect(picker, &QObject::destroyed, qApp, [id] { pickers.erase(id); });
  QObject::connect(dialog, &QDialog::finished, picker, [picker, dialog](int result) {
    QJsonArray paths;
    if (result == QDialog::Accepted) for (const auto& path : dialog->selectedFiles()) paths.append(path);
    picker->Complete(result == QDialog::Accepted ? 0 : 1, QJsonDocument(paths).toJson(QJsonDocument::Compact));
  });
  dialog->open();
  return 0;
}
extern "C" DOROTI_QT_EXPORT int doroti_qt_cancel_picker_v1(std::uint64_t id) {
  if (!Gui()) return 72;
  const auto it = pickers.find(id);
  if (it != pickers.end() && it->second) it->second->Complete(1);
  return 0;
}
extern "C" DOROTI_QT_EXPORT int doroti_qt_drop_bind_v1(void* owner, Drop callback, void* context) {
  if (!Gui() || !owner) return 72;
  auto found = receivers.find(owner);
  if (found != receivers.end()) { delete found->second.data(); receivers.erase(found); }
  if (callback) {
    auto* window = static_cast<QWindow*>(owner);
    if (!QGuiApplication::allWindows().contains(window)) return 71;
    auto* receiver = new Receiver(window, callback, context);
    receivers.emplace(owner, receiver); window->installEventFilter(receiver);
  }
  return 0;
}

// O_NONBLOCK prevents a selected FIFO/device from blocking the GUI thread.
extern "C" DOROTI_QT_EXPORT int doroti_qt_open_read_file_v1(const char* path) {
  const int fd = ::open(path, O_RDONLY | O_CLOEXEC | O_NONBLOCK);
  if (fd < 0) return -errno;
  struct stat info{};
  if (::fstat(fd, &info) != 0) { const int error = errno; ::close(fd); return -error; }
  if (!S_ISREG(info.st_mode)) { ::close(fd); return -EINVAL; }
  return fd;
}

namespace {
std::uint64_t active_drag = 0;
}
extern "C" DOROTI_QT_EXPORT int doroti_qt_drag_v1(void* owner, std::uint64_t id,
    doroti_qt_utf8_v2 payload, Reply reply, void* context) {
  if (!Gui() || !owner || !reply || payload.length > 8*1024*1024) return 66;
  auto* window = static_cast<QWindow*>(owner);
  if (!QGuiApplication::allWindows().contains(window) || active_drag) return 71;
  auto document = QJsonDocument::fromJson(QByteArray(reinterpret_cast<const char*>(payload.data), qsizetype(payload.length)));
  if (!document.isObject()) return 66;
  auto value = document.object();
  int actions = value.value("actions").toInt();
  if (!actions || (actions & ~7)) return 66;
  auto* drag = new QDrag(qApp);
  auto* mime = new QMimeData;
  if (value.contains("text") && !value.value("text").isNull()) mime->setText(value.value("text").toString());
  QList<QUrl> urls;
  for (auto uri : value.value("uris").toArray()) {
    QUrl url(uri.toString());
    if (!url.isValid() || url.isRelative()) { delete mime; delete drag; return 66; }
    urls.append(url);
  }
  if (!urls.isEmpty()) mime->setUrls(urls);
  drag->setMimeData(mime);
  if (!value.value("image").toString().isEmpty()) {
    QPixmap image;
    if (!image.loadFromData(QByteArray::fromBase64(value.value("image").toString().toUtf8()), "PNG")) { delete drag; return 66; }
    drag->setPixmap(image); drag->setHotSpot(QPoint(value.value("hotX").toInt(),value.value("hotY").toInt()));
  }
  active_drag = id;
  auto owner_closed = QObject::connect(window, &QObject::destroyed, drag, [] { QDrag::cancel(); });
  // A queued invocation leaves the ABI caller before the nested Qt drag loop.
  struct Completion {
    QPointer<QDrag> drag; Reply reply; void* context; std::uint64_t id; bool completed = false;
    ~Completion() {
      if (!completed) reply(context, 0, Utf8("{}"));
      if (active_drag == id) active_drag = 0;
      delete drag.data();
    }
  };
  auto completion = std::make_shared<Completion>();
  completion->drag = drag; completion->reply = reply; completion->context = context; completion->id = id;
  QTimer::singleShot(0, qApp, [completion, id, actions, owner_closed] {
    Qt::DropAction action = Qt::IgnoreAction;
    if (active_drag == id && completion->drag) action = completion->drag->exec(Qt::DropActions(actions));
    QObject::disconnect(owner_closed);
    if (active_drag == id) active_drag = 0;
    completion->completed = true;
    completion->reply(completion->context, int(action), Utf8("{}"));
  });
  return 0;
}
extern "C" DOROTI_QT_EXPORT int doroti_qt_cancel_drag_v1(std::uint64_t id) {
  if (!Gui()) return 72;
  if (active_drag == id) { active_drag = 0; QDrag::cancel(); }
  return 0;
}
