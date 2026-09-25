// Validation-only preload: inspect native state and deliver real QWindow close
// events. Does not represent physical keyboard/WM-button qualification.
#include "doroti_qt_desktop.h"
#include <QApplication>
#include <QFile>
#include <QJsonDocument>
#include <QJsonObject>
#include <QJsonArray>
#include <QPointer>
#include <QQuickWindow>
#include <QTimer>
#include <dlfcn.h>
#include <memory>
#include <thread>

namespace {
void Write(const QString& path, const QJsonObject& value) {
  QFile file(path);
  if (!file.open(QIODevice::WriteOnly)) qFatal("Cannot write Desktop evidence");
  file.write(QJsonDocument(value).toJson());
}
void Start() {
  const auto path = QString::fromUtf8(qgetenv("DOROTI_QT_DESKTOP_PROBE"));
  if (path.isEmpty()) return;
  QTimer::singleShot(0, qApp, [path] {
    auto* timer = new QTimer(qApp);
    auto record = std::make_shared<QJsonObject>();
    auto api = std::make_shared<doroti_qt_desktop_api>();
    auto owner = std::make_shared<std::uint64_t>(0);
    QObject::connect(qApp, &QCoreApplication::aboutToQuit, qApp, [path, record, api, owner] {
      doroti_qt_desktop_state state{sizeof(state)};
      record->insert("staleOwnerRejected", *owner && api->snapshot(*owner, &state) == DOROTI_QT_PV_CLOSED);
      record->insert("remainingWindows", QGuiApplication::topLevelWindows().size());
      Write(path + ".teardown.json", *record);
    });
    QObject::connect(timer, &QTimer::timeout, qApp, [path, timer, record, api, owner,
        window = QPointer<QQuickWindow>(), phase = 0, closedTicks = 0]() mutable {
      if (phase == 0) {
        for (auto* candidate : QGuiApplication::topLevelWindows())
          if (auto* quick = qobject_cast<QQuickWindow*>(candidate)) window = quick;
        if (!QFile::exists(path)) {
          if (window) Write(path + ".waiting.json", {{"width", window->width()}, {"height", window->height()},
            {"state", int(window->windowState())}, {"minimumWidth", window->minimumWidth()},
            {"maximumWidth", window->maximumWidth()}, {"visible", window->isVisible()}});
          return;
        }
        if (!window) qFatal("Desktop probe has no Quick window");
        using Get = int (*)(void*, std::uint32_t, std::uint32_t, std::uint64_t*, doroti_qt_desktop_api*);
        auto get = reinterpret_cast<Get>(dlsym(RTLD_DEFAULT, "doroti_qt_get_desktop"));
        // .NET uses local dlopen; find the already loaded sibling by its filename.
        if (!get) {
          auto* library = dlopen("libdoroti_qt_host.so", RTLD_NOW | RTLD_NOLOAD);
          if (library) get = reinterpret_cast<Get>(dlsym(library, "doroti_qt_get_desktop"));
        }
        if (!get || get(window, 1, sizeof(*api), owner.get(), api.get()) != 0)
          qFatal("Desktop ABI unavailable in validation driver");
        std::uint64_t unused = 0;
        doroti_qt_desktop_api invalid{};
        record->insert("invalidVersionRejected", get(window, 99, sizeof(invalid), &unused, &invalid) == 64);
        record->insert("invalidSizeRejected", get(window, 1, 0, &unused, &invalid) == 64);
        int wrongThread = 0;
        std::thread worker([&] { doroti_qt_desktop_state state{sizeof(state)};
          wrongThread = api->snapshot(*owner, &state); });
        worker.join();
        record->insert("wrongThreadRejected", wrongThread == DOROTI_QT_PV_WRONG_THREAD);
        doroti_qt_desktop_command unsupported{sizeof(unsupported), 8};
        const bool topmost = api->command(*owner, &unsupported) == DOROTI_QT_PV_UNSUPPORTED;
        unsupported.kind = 9;
        record->insert("unsupportedDefaultsRejected", topmost &&
            api->command(*owner, &unsupported) == DOROTI_QT_PV_UNSUPPORTED);
        record->insert("qpa", QGuiApplication::platformName());
        record->insert("width", window->width()); record->insert("height", window->height());
        record->insert("minimumWidth", window->minimumWidth());
        record->insert("maximumWidth", window->maximumWidth());
        record->insert("title", window->title());
        record->insert("visible", window->isVisible());
        record->insert("nativeDecorations", !window->flags().testFlag(Qt::FramelessWindowHint));
        QFile maps("/proc/self/maps");
        const auto mapped = maps.open(QIODevice::ReadOnly) ? maps.readAll() : QByteArray();
        record->insert("validationLayerLoaded", mapped.contains("libVkLayer_khronos_validation.so"));
        QJsonArray libraries;
        for (const auto& line : mapped.split('\n'))
          if (line.contains("/libdoroti_") || line.contains("/libSkiaSharp.so") || line.contains("/libQt6")) {
            const auto library = QString::fromUtf8(line.mid(line.indexOf('/')));
            if (!libraries.contains(library)) libraries.append(library);
          }
        record->insert("nativeLibraries", libraries);
        record->insert("capture", window->grabWindow().save(path + ".png"));
        Write(path + ".native.json", *record);
        phase = 1;
      } else if (phase == 1 && qEnvironmentVariableIsSet("DOROTI_QT_DESKTOP_NATIVE_CLOSE")) {
        window->close(); phase = 2;
      } else if (phase == 2) {
        QFile close(path + ".close");
        if (!close.open(QIODevice::ReadOnly) || close.readAll() != "1") return;
        record->insert("nativeCloseCanceled", window && window->isVisible());
        if (!window || !window->isVisible()) qFatal("Canceled native close destroyed the window");
        window->setTitle("Native close cancellation survived");
        window->close(); phase = 3;
      }
      if (QFile::exists(path + ".closed") && qgetenv("DOROTI_DESKTOP_LIFETIME") == "Explicit") {
        if (++closedTicks < 5) return;
        record->insert("explicitProcessSurvived", true);
        timer->stop();
        QCoreApplication::quit(); // The external validation driver ends Explicit lifetime.
      }
    });
    timer->start(100);
  });
}
}
Q_COREAPP_STARTUP_FUNCTION(Start)
