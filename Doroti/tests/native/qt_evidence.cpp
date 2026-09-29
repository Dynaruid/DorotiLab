// Loaded only by linux_qt_smoke.py, never linked into product shims.
#include <QApplication>
#include <QFile>
#include <QJsonArray>
#include <QJsonDocument>
#include <QJsonObject>
#include <QQuickWindow>
#include <QTimer>
#include <QWindow>
#include <QFileDialog>
#include <QMimeData>
#include <QDragEnterEvent>
#include <QDropEvent>
#include <QUrl>

static void Start() {
  auto* timer = new QTimer(qApp);
  QObject::connect(timer, &QTimer::timeout, qApp, [timer] {
    const auto services = qgetenv("DOROTI_QT_SERVICES_PROBE");
    static bool picked = false, dropped = false;
    if (!services.isEmpty()) {
      if (!picked) for (auto* widget : QApplication::topLevelWidgets()) {
        auto* dialog = qobject_cast<QFileDialog*>(widget);
        if (!dialog || !dialog->isVisible()) continue;
        dialog->selectFile(QString::fromUtf8(qgetenv("DOROTI_QT_PICK_FILE")));
        QMetaObject::invokeMethod(dialog, "accept", Qt::QueuedConnection);
        picked = true; break;
      }
      if (!dropped && QFile::exists(QString::fromUtf8(services + ".drop-ready"))) {
        for (auto* window : QGuiApplication::topLevelWindows()) {
          if (!qobject_cast<QQuickWindow*>(window) || !window->isVisible()) continue;
          QMimeData mime;
          mime.setText(QString::fromUtf8("Qt drop 한글"));
          mime.setUrls({QUrl::fromLocalFile(QString::fromUtf8(qgetenv("DOROTI_QT_PICK_FILE"))),
            QUrl::fromLocalFile(QString::fromUtf8(qgetenv("DOROTI_QT_LARGE_FILE"))), QUrl("https://example.com/qt")});
          QDragEnterEvent enter(QPoint(40,40),Qt::CopyAction,&mime,Qt::LeftButton,Qt::NoModifier);
          QCoreApplication::sendEvent(window,&enter);
          QDropEvent drop(QPointF(40,40),Qt::CopyAction,&mime,Qt::LeftButton,Qt::NoModifier);
          QCoreApplication::sendEvent(window,&drop);
          dropped = drop.isAccepted(); break;
        }
      }
    }
    auto path = qgetenv("DOROTI_MULTIWINDOW_PROBE");
    const bool multi = !path.isEmpty();
    if (!multi) path = qgetenv("DOROTI_QT_DESKTOP_PROBE");
    if (path.isEmpty() || !QFile::exists(QString::fromUtf8(path + (multi ? ".ready" : ""))) ||
        QFile::exists(QString::fromUtf8(path + ".native.json"))) return;
    QJsonArray snapshots;
    for (auto* window : QGuiApplication::topLevelWindows()) {
      auto* quick = qobject_cast<QQuickWindow*>(window);
      if (!quick || !quick->isVisible()) continue;
      const auto capture = quick->grabWindow();
      if (capture.isNull()) return;
      const auto filename = QString::fromUtf8(path) + "." + QString::number(snapshots.size()) + ".png";
      if (!capture.save(filename)) return;
      snapshots.append(QJsonObject{{"title",window->title()}, {"width",window->width()},
        {"height",window->height()}, {"dpr",window->devicePixelRatio()},
        {"pixelsWidth",capture.width()}, {"pixelsHeight",capture.height()}, {"capture",filename}});
    }
    if (snapshots.size() < (multi ? 2 : 1)) return;
    QFile output(QString::fromUtf8(path + ".native.json"));
    if (output.open(QIODevice::WriteOnly)) output.write(QJsonDocument(snapshots).toJson());
    timer->stop();
  });
  timer->start(200);
}
Q_COREAPP_STARTUP_FUNCTION(Start)
