#include "doroti_qt_webview.h"
#include <QApplication>
#include <QQuickWindow>
#include <QQuickItem>
#include <QVulkanInstance>
#include <QJsonDocument>
#include <QJsonObject>
#include <QTest>
#include <QElapsedTimer>
#include <iostream>
#include <thread>
#include <signal.h>
#include <stdexcept>

static void Check(bool ok,const char* label) {if(!ok)throw std::runtime_error(label);std::cout<<"PASS "<<label<<std::endl;}
static doroti_qt_utf8_v2 Utf8(const QByteArray& bytes) {return {reinterpret_cast<const std::uint8_t*>(bytes.constData()),std::uint64_t(bytes.size())};}
static void Callback(void* context,doroti_qt_utf8_v2 data) {
    auto* messages=static_cast<QList<QJsonObject>*>(context);
    messages->append(QJsonDocument::fromJson(QByteArray(reinterpret_cast<const char*>(data.data),qsizetype(data.length))).object());
}
int main(int argc,char** argv) {
    qputenv("QSG_RENDER_LOOP","basic");QQuickWindow::setGraphicsApi(QSGRendererInterface::Vulkan);
    if(DorotiWebInitialize()!=0)return 3;QApplication app(argc,argv);
    QVulkanInstance instance;instance.setApiVersion(QVersionNumber(1,2));if(!instance.create())return 2;
    QQuickWindow a,b;a.setVulkanInstance(&instance);b.setVulkanInstance(&instance);
    a.resize(320,240);b.resize(320,240);a.show();b.show();
    DorotiQtRegisterPlatformOwner(&a);DorotiQtRegisterPlatformOwner(&b);
    try {
        doroti_qt_pv_api pv{},pv2{};std::uint64_t owner,other,id,id2;
        Check(doroti_qt_get_platform_views(&a,1,sizeof(pv),&owner,&pv)==0,"owner");
        Check(doroti_qt_get_platform_views(&b,1,sizeof(pv2),&other,&pv2)==0,"second owner");
        doroti_qt_webview_api web{};
        Check(doroti_qt_get_webview_api(2,sizeof(web),&web)==DOROTI_QT_ERROR_ABI_VERSION,"reject incompatible version");
        Check(doroti_qt_get_webview_api(1,sizeof(web)-8,&web)==DOROTI_QT_ERROR_ABI_SIZE,"reject incompatible size");
        Check(doroti_qt_get_webview_api(1,sizeof(web),&web)==0&&web.features==31,"WebView ABI 1 / 32 bytes");
        auto malformed=QByteArray("doroti-webview:999\n{}");
        std::uint64_t rejected=0;
        Check(pv.create(owner,2,Utf8(malformed),nullptr,nullptr,&rejected)==DOROTI_QT_ERROR_NATIVE_EXCEPTION&&rejected==0,
              "native creation exception becomes status 70 without publishing an item");
        auto html=QByteArray("<!doctype html><title>owner</title><input value='retained'>");
        Check(pv.create(owner,2,Utf8(html),nullptr,nullptr,&id)==0,"live first WebView");
        Check(pv.create(other,2,Utf8(html),nullptr,nullptr,&id2)==0&&id!=id2,"live second owner WebView");
        QList<QJsonObject> messages,second;
        Check(web.bind(owner,id,Callback,&messages)==0&&web.bind(other,id2,Callback,&second)==0,"owner callbacks");
        auto state=QByteArray(R"JSON({"request":"1","operation":1})JSON");
        Check(web.execute(other,id,Utf8(state))==DOROTI_QT_PV_STALE,"reject cross-owner command");
        int wrongThread=0;std::thread worker([&]{wrongThread=web.execute(owner,id,Utf8(state));});worker.join();
        Check(wrongThread==DOROTI_QT_PV_WRONG_THREAD,"reject wrong-thread command");
        QElapsedTimer timer;timer.start();bool loaded=false;
        while(timer.elapsed()<15000) {
            messages.clear();web.execute(owner,id,Utf8(state));
            if(!messages.isEmpty()&&!messages.last()["loading"].toBool()&&messages.last()["generation"].toInteger()>0) {loaded=true;break;}
            QTest::qWait(50);
        }
        Check(loaded,"actual Chromium document completion");
        const auto generation=messages.last()["generation"].toInteger();
        messages.clear();auto js=QByteArray(R"JSON({"request":"2","operation":8,"text":"document.querySelector('input').value"})JSON");
        Check(web.execute(owner,id,Utf8(js))==0,"admit JS");
        timer.restart();while(messages.isEmpty()&&timer.elapsed()<15000)QTest::qWait(20);
        Check(!messages.isEmpty()&&messages.last()["text"].toString()=="\"retained\"","actual JS completion");
        messages.clear();auto slow=QByteArray(R"JSON({"request":"3","operation":8,"text":"(()=>{let end=Date.now()+500;while(Date.now()<end){};return 'old'})()"})JSON");
        web.execute(owner,id,Utf8(slow));
        auto load=QByteArray(R"JSON({"request":"4","operation":3,"text":"<title>replacement</title>"})JSON");web.execute(owner,id,Utf8(load));
        bool canceled=false;for(const auto& message:messages)if(message["request"]=="3"&&message["error"].toInt()==4)canceled=true;
        Check(canceled,"navigation cancels stale JS immediately");
        auto stale=QJsonDocument(QJsonObject{{"request","5"},{"operation",8},{"generation",generation},{"text","1"}}).toJson();
        web.execute(owner,id,Utf8(stale));Check(messages.last()["error"].toInt()==4,"stale generation rejected");
        Check(web.bind(owner,id,nullptr,nullptr)==0,"unbind callback before release");
        const auto count=messages.size();Check(pv.remove(owner,id)==0,"native retirement");QTest::qWait(600);
        Check(messages.size()==count,"no late callback after unbind/removal");
        Check(web.execute(owner,id,Utf8(state))==DOROTI_QT_PV_STALE,"removed identity rejected");
        for(int i=0;i<10;i++) {
            std::uint64_t next;QList<QJsonObject> cycle;
            Check(pv.create(owner,2,Utf8(html),nullptr,nullptr,&next)==0,"create cycle");
            web.bind(owner,next,Callback,&cycle);
            // Replace initial HTML before its first load callback can run.
            web.execute(owner,next,Utf8(load));timer.restart();loaded=false;
            while(timer.elapsed()<15000) {
                cycle.clear();web.execute(owner,next,Utf8(state));
                if(!cycle.isEmpty()&&!cycle.last()["loading"].toBool()&&cycle.last()["title"]=="replacement") {loaded=true;break;}
                QTest::qWait(20);
            }
            Check(loaded,"initial-load replacement settles");
            web.bind(owner,next,nullptr,nullptr);Check(pv.remove(owner,next)==0,"dispose cycle");
        }
        DorotiQtClosePlatformOwner(&a);
        second.clear();Check(web.execute(other,id2,Utf8(state))==0&&!second.isEmpty(),"second owner survives first close");
        auto* item=b.findChild<QQuickItem*>("doroti-quick-webview");
        const auto pid=item?item->property("renderProcessPid").toLongLong():0;
        Check(pid>0&&kill(pid,SIGKILL)==0,"terminate this fixture's renderer process");
        timer.restart();bool terminated=false;
        while(!terminated&&timer.elapsed()<15000) {QTest::qWait(20);for(const auto& message:second)if(message["event"].toInt(-1)==4)terminated=true;}
        Check(terminated,"native process-failure event");
        web.execute(other,id2,Utf8(state));Check(second.last()["error"].toInt()==6,"failed process is terminal");
        web.bind(other,id2,nullptr,nullptr);Check(pv.remove(other,id2)==0,"retire failed item");
        Check(pv.create(other,2,Utf8(html),nullptr,nullptr,&id2)==0,"explicit recreation after process failure");
        second.clear();web.bind(other,id2,Callback,&second);timer.restart();loaded=false;
        while(timer.elapsed()<15000) {second.clear();web.execute(other,id2,Utf8(state));if(!second.isEmpty()&&!second.last()["loading"].toBool()&&second.last()["generation"].toInteger()>0){loaded=true;break;}QTest::qWait(50);}
        Check(loaded,"replacement renderer loads a fresh document");
        web.bind(other,id2,nullptr,nullptr);
        struct CloseProbe {QQuickItem* item;bool canceled=false;int late=0;} closing{b.findChild<QQuickItem*>("doroti-quick-webview")};
        Check(closing.item!=nullptr,"closing item exists");
        web.bind(other,id2,[](void* context,doroti_qt_utf8_v2){auto* probe=static_cast<CloseProbe*>(context);if(probe->canceled)++probe->late;},&closing);
        Check(pv.post(other,[](void* context,int status){
            auto* probe=static_cast<CloseProbe*>(context);probe->canceled=status==DOROTI_QT_PV_CLOSED;
            // Simulate a late native event after canceled managed work releases
            // its context, but before the owner's QML items are destroyed.
            auto* bridge=probe->item->findChild<QObject*>("doroti",Qt::FindDirectChildrenOnly);
            if(bridge)QMetaObject::invokeMethod(bridge,"failed",Q_ARG(QString,QStringLiteral("late close probe")));
        },&closing)==0,"queue canceled work beside WebView");
        DorotiQtClosePlatformOwner(&b);
        Check(closing.canceled&&closing.late==0,"callbacks disconnected before canceled-post cleanup");
        Check(web.execute(other,id2,Utf8(state))==DOROTI_QT_PV_CLOSED,"closed owner rejected");
        return 0;
    } catch(const std::exception& error) {std::cerr<<"FAIL "<<error.what()<<std::endl;DorotiQtClosePlatformOwner(&a);DorotiQtClosePlatformOwner(&b);return 1;}
}
