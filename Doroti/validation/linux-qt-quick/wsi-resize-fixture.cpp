#include <QGuiApplication>
#include <QFile>
#include <QQuickWindow>
#include <QSGRendererInterface>
#include <QTimer>
#include <QThread>
#include <QVulkanInstance>
#include <QVersionNumber>
#include <iostream>

// Qt-only WSI probe: no Doroti shim, Graphite, or platform views.
int main(int argc, char** argv) {
    qputenv("QSG_RENDER_LOOP", "basic");
    QQuickWindow::setGraphicsApi(QSGRendererInterface::Vulkan);
    QGuiApplication app(argc, argv);
    QVulkanInstance instance;
    instance.setApiVersion(QVersionNumber(1, 2));
    if (!instance.create()) return 2;
    QQuickWindow window;
    window.setVulkanInstance(&instance);
    const int syncDelayMs = qEnvironmentVariableIntValue("DOROTI_QT_WSI_SYNC_DELAY_MS");
    QObject::connect(&window, &QQuickWindow::beforeSynchronizing, &window,
                     [syncDelayMs] { if (syncDelayMs > 0) QThread::msleep(syncDelayMs); },
                     Qt::DirectConnection);
    window.resize(720, 640);
    int swaps = 0;
    QObject::connect(&window, &QQuickWindow::frameSwapped, &window, [&] { ++swaps; });
    window.show();
    int cycles = 0;
    QTimer timer;
    QObject::connect(&timer, &QTimer::timeout, &window, [&] {
        const int offset = (cycles % 2 == 0) ? 37 : 0;
        window.resize(720 + offset, 640 + offset);
        window.update();
        if (++cycles == 10) {
            timer.stop();
            QTimer::singleShot(500, &app, [&] {
                QFile maps("/proc/self/maps");
                const bool loaded = maps.open(QIODevice::ReadOnly) &&
                    maps.readAll().contains("libVkLayer_khronos_validation.so");
                std::cout << "cycles=10 syncDelayMs=" << syncDelayMs << " frameSwaps=" << swaps
                          << " validationLayerLoaded=" << loaded << '\n';
                app.quit();
            });
        }
    });
    timer.start(80);
    return app.exec();
}
