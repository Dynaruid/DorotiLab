#include "doroti_qt_quick.h"
#include <QApplication>
#include <QQuickWindow>
#include <QTimer>
#include <QVulkanInstance>

// Synthetic GPU bank/failure contract, on an actual Qt-owned Vulkan queue.
extern "C" int doroti_test_gpu(int(*callback)(const doroti_qt_quick_gpu*)) {
    qputenv("QSG_RENDER_LOOP","basic");
    QQuickWindow::setGraphicsApi(QSGRendererInterface::Vulkan);
    int argc=1;char name[]="doroti-gpu-contract";char* argv[]={name,nullptr};
    QApplication app(argc,argv);
    QVulkanInstance instance;instance.setApiVersion(QVersionNumber(1,2));
    if(!instance.create())return 2;
    QQuickWindow window;window.setVulkanInstance(&instance);window.resize(128,128);
    DorotiQtRegisterPlatformOwner(&window);
    int result=3;bool ran=false;
    QObject::connect(&window,&QQuickWindow::beforeSynchronizing,&window,[&] {
        if(ran)return;ran=true;
        doroti_qt_quick_gpu gpu{1,sizeof(gpu)};
        result=doroti_qt_quick_get_gpu(&window,&gpu);
        if(result==0)result=callback(&gpu);
        QTimer::singleShot(0,&app,&QCoreApplication::quit);
    },Qt::DirectConnection);
    QTimer::singleShot(30000,&app,&QCoreApplication::quit);
    window.show();app.exec();
    DorotiQtClosePlatformOwner(&window);
    return result;
}
