#include "doroti_qt_quick.h"
#include <QApplication>
#include <QQuickItem>
#include <QQuickWindow>
#include <QTouchEvent>
#include <QTabletEvent>
#include <QTest>
#include <QVulkanInstance>
#include <bit>
#include <iostream>
#include <stdexcept>
#include <thread>

static void Check(bool value,const char* message) { if(!value)throw std::runtime_error(message); }
static void Completed(void* state,int result) {
    auto* values=static_cast<int*>(state);++values[0];values[1]=result;
}
int main(int argc,char** argv) {
    qputenv("QSG_RENDER_LOOP","basic");
    QQuickWindow::setGraphicsApi(QSGRendererInterface::Vulkan);
    QApplication app(argc,argv);
    QVulkanInstance instance;instance.setApiVersion(QVersionNumber(1,2));
    if(!instance.create())return 2;
    QQuickWindow a,b;a.setVulkanInstance(&instance);b.setVulkanInstance(&instance);
    a.resize(200,200);b.resize(200,200);
    DorotiQtRegisterPlatformOwner(&a);DorotiQtRegisterPlatformOwner(&b);
    try {
        doroti_qt_pv_api api{},other{};std::uint64_t owner,owner2,id,editor;
        Check(doroti_qt_get_platform_views(&a,1,sizeof(api),&owner,&api)==0,"owner ABI");
        Check(doroti_qt_get_platform_views(&b,1,sizeof(other),&owner2,&other)==0&&owner!=owner2,"two owner isolation");
        Check(api.create(owner,0,{},nullptr,nullptr,&id)==0,"create button");
        Check(api.create(owner,1,{},nullptr,nullptr,&editor)==0,"create editor");
        doroti_qt_quick_part part{sizeof(part),1,id,0,0,0,{10.25,20.75,100.5,50.25},{10.25,20.75,100.5,50.25}};
        Check(doroti_qt_quick_commit(&a,&part,1,1)==0,"fractional placement");
        auto* button=a.findChild<QQuickItem*>("doroti-quick-button");
        Check(button&&button->parentItem()->x()==10.25&&button->parentItem()->y()==20.75,"Quick must preserve subpixel geometry");
        Check(doroti_qt_quick_commit(&b,&part,1,1)==DOROTI_QT_PV_STALE,"cross owner rejected");
        auto bad=part;bad.id=999999;
        Check(doroti_qt_quick_commit(&a,&bad,1,1)==DOROTI_QT_PV_STALE&&button->parentItem()->x()==10.25,"stale frame preserves placement");
        auto move=part;move.bounds.x=30.5;move.clip=move.bounds;
        Check(doroti_qt_quick_commit(&a,&move,1,0)==0&&button->parentItem()->x()==10.25,"prepare is non-mutating");
        int threadResult=0;
        std::thread worker([&]{threadResult=doroti_qt_quick_commit(&a,&part,1,1);});worker.join();
        Check(threadResult==DOROTI_QT_PV_WRONG_THREAD,"GUI thread ownership");
        QMouseEvent press(QEvent::MouseButtonPress,{25,35},{25,35},Qt::LeftButton,Qt::LeftButton,Qt::NoModifier);
        Check(DorotiQtQuickNativeInput(&a,&press),"exposed native input");
        QMouseEvent release(QEvent::MouseButtonRelease,{25,35},{25,35},Qt::LeftButton,Qt::NoButton,Qt::NoModifier);
        DorotiQtQuickNativeInput(&a,&release);
        doroti_qt_quick_part shield{sizeof(shield),2,0,0,0,0,part.bounds,part.clip};
        doroti_qt_quick_part blocked[]{part,shield};
        Check(doroti_qt_quick_commit(&a,blocked,2,1)==0&&!DorotiQtQuickNativeInput(&a,&press),"committed shield blocks native");
        DorotiQtQuickNativeInput(&a,&release);
        doroti_qt_quick_part effect{sizeof(effect),3,std::bit_cast<std::uint64_t>(6.0),0,0,0,{20,30,60,30},{2,12,96,66}};
        doroti_qt_quick_part withEffect[]{part,effect};
        Check(doroti_qt_quick_commit(&a,withEffect,2,1)==0,"effect source group");
        Check(DorotiQtQuickNativeInput(&a,&press),"effect passes native input");
        DorotiQtQuickNativeInput(&a,&release);
        auto* parent=button->parentItem()->parentItem();
        Check(parent!=a.contentItem(),"native enters preceding source group");
        doroti_qt_quick_part duplicate[]{part,effect,effect};
        Check(doroti_qt_quick_commit(&a,duplicate,3,1)==DOROTI_QT_PV_UNSUPPORTED&&button->parentItem()->parentItem()==parent,"multiple effects explicitly rejected without mutation");
        bad=effect;bad.clip.width+=1;
        Check(doroti_qt_quick_commit(&a,&bad,1,1)==DOROTI_QT_PV_UNSUPPORTED,"sample expansion validated");
        Check(doroti_qt_quick_commit(&a,&part,1,1)==0&&button->parentItem()->parentItem()==a.contentItem(),"last effect removed and native identity retained");
        QMouseEvent secondPress(QEvent::MouseButtonPress,{180,180},{180,180},Qt::RightButton,
                                Qt::LeftButton|Qt::RightButton,Qt::NoModifier);
        QMouseEvent firstRelease(QEvent::MouseButtonRelease,{180,180},{180,180},Qt::LeftButton,
                                  Qt::RightButton,Qt::NoModifier);
        QMouseEvent secondRelease(QEvent::MouseButtonRelease,{180,180},{180,180},Qt::RightButton,
                                   Qt::NoButton,Qt::NoModifier);
        Check(DorotiQtQuickNativeInput(&a,&press),"native drag start");
        Check(DorotiQtQuickNativeInput(&a,&secondPress),"second button preserves first owner");
        Check(DorotiQtQuickNativeInput(&a,&firstRelease),"first button release preserves drag owner");
        Check(DorotiQtQuickNativeInput(&a,&secondRelease),"final button release reaches owner");
        QMouseEvent outsideMove(QEvent::MouseMove,{180,180},{180,180},Qt::NoButton,Qt::NoButton,Qt::NoModifier);
        Check(!DorotiQtQuickNativeInput(&a,&outsideMove),"final mouse release clears capture");
        QPointingDevice touchDevice("contract-touch",7,QInputDevice::DeviceType::TouchScreen,
                                    QPointingDevice::PointerType::Finger,QInputDevice::Capability::Position,2,0);
        QTouchEvent touchDown(QEvent::TouchBegin,&touchDevice,Qt::NoModifier,
            {QEventPoint(1,QEventPoint::Pressed,{25,35},{25,35})});
        QTouchEvent touchMove(QEvent::TouchUpdate,&touchDevice,Qt::NoModifier,
            {QEventPoint(1,QEventPoint::Updated,{180,180},{180,180})});
        QTouchEvent touchEnd(QEvent::TouchEnd,&touchDevice,Qt::NoModifier,
            {QEventPoint(1,QEventPoint::Released,{180,180},{180,180})});
        Check(DorotiQtQuickNativeInput(&a,&touchDown),"native touch start");
        Check(DorotiQtQuickNativeInput(&a,&touchMove),"touch move retains owner");
        Check(DorotiQtQuickNativeInput(&a,&touchEnd),"touch release retains owner");
        Check(!DorotiQtQuickNativeInput(&a,&touchMove),"touch release clears owner");
        QPointingDevice penDevice("contract-pen",8,QInputDevice::DeviceType::Stylus,
                                  QPointingDevice::PointerType::Pen,QInputDevice::Capability::Position,1,1);
        QTabletEvent penDown(QEvent::TabletPress,&penDevice,{25,35},{25,35},1,0,0,0,0,0,
                             Qt::NoModifier,Qt::LeftButton,Qt::LeftButton);
        QTabletEvent penMove(QEvent::TabletMove,&penDevice,{180,180},{180,180},1,0,0,0,0,0,
                             Qt::NoModifier,Qt::NoButton,Qt::LeftButton);
        QTabletEvent penUp(QEvent::TabletRelease,&penDevice,{180,180},{180,180},0,0,0,0,0,0,
                           Qt::NoModifier,Qt::LeftButton,Qt::NoButton);
        Check(DorotiQtQuickNativeInput(&a,&penDown),"native tablet start");
        Check(DorotiQtQuickNativeInput(&a,&penMove),"tablet move retains owner");
        Check(DorotiQtQuickNativeInput(&a,&penUp),"tablet release retains owner");
        Check(!DorotiQtQuickNativeInput(&a,&penMove),"tablet release clears owner");
        Check(doroti_qt_quick_commit(&a,blocked,2,1)==0,"touch/tablet shield setup");
        Check(!DorotiQtQuickNativeInput(&a,&touchDown),"shield blocks native touch");
        Check(!DorotiQtQuickNativeInput(&a,&penDown),"shield blocks native tablet");
        DorotiQtQuickNativeInput(&a,&touchEnd);
        DorotiQtQuickNativeInput(&a,&penUp);
        Check(doroti_qt_quick_commit(&a,&part,1,1)==0,"touch/tablet shield cleanup");
        Check(DorotiQtQuickNativeInput(&a,&press),"drag before cancel");
        DorotiQtQuickCancelNativeInput(&a);
        Check(!DorotiQtQuickNativeInput(&a,&outsideMove),"window deactivation clears native capture");
        Check(DorotiQtQuickNativeInput(&a,&press),"drag before lost grab");
        QEvent ungrab(QEvent::UngrabMouse);
        DorotiQtQuickNativeInput(&a,&ungrab);
        Check(!DorotiQtQuickNativeInput(&a,&outsideMove),"lost mouse grab clears capture");
        Check(DorotiQtQuickNativeInput(&a,&press),"drag before hide");
        Check(doroti_qt_quick_commit(&a,nullptr,0,1)==0&&!button->isVisible(),"empty frame hides native and shields");
        Check(!DorotiQtQuickNativeInput(&a,&outsideMove),"hidden native control clears capture");
        for(int i=0;i<10;i++) {
            std::uint64_t transient;
            Check(api.create(owner,0,{},nullptr,nullptr,&transient)==0,"repeated create");
            Check(api.remove(owner,transient)==0,"repeated dispose");
            Check(api.remove(owner,transient)==DOROTI_QT_PV_STALE,"old generation removal rejected");
        }
        int completion[2]{};
        for(int i=0;i<1024;i++)Check(api.post(owner,Completed,completion)==0,"bounded post accepted");
        Check(api.post(owner,Completed,completion)==DOROTI_QT_PV_UNSUPPORTED&&completion[0]==0,"queue backpressure invokes no rejected callback");
        DorotiQtClosePlatformOwner(&a);
        QCoreApplication::processEvents();
        Check(completion[0]==1024&&completion[1]==DOROTI_QT_PV_CLOSED,"owner close exactly-once cancellation");
        Check(api.post(owner,Completed,completion)==DOROTI_QT_PV_CLOSED&&completion[0]==1024,"rejected late post has no callback");
        Check(api.remove(owner,id)==DOROTI_QT_PV_CLOSED,"late native removal");
        Check(other.create(owner2,0,{},nullptr,nullptr,&id)==0,"second owner survives first close");
        DorotiQtClosePlatformOwner(&b);
        std::cout<<"PASS: owner/generation/prepare/failure/fractional/input/effect/10-lifecycles/late-close contracts\n";
        return 0;
    } catch(const std::exception& e) {
        DorotiQtClosePlatformOwner(&a);DorotiQtClosePlatformOwner(&b);
        std::cerr<<e.what()<<'\n';return 1;
    }
}
