#include "doroti_qt_quick.h"
#include <QCoreApplication>
#include <QFile>
#include <QJsonArray>
#include <QJsonDocument>
#include <QJsonObject>
#include <QQmlEngine>
#include <QQuickItem>
#include <QQuickWindow>
#include <QTest>
#include <QTimer>
#include <QAccessible>

class ProbeWriter : public QObject {
    Q_OBJECT
    QString path;
public:
    ProbeWriter(QString path,QObject* parent):QObject(parent),path(std::move(path)){}
public slots:
    void save(QJSValue value) {
        QFile output(path);
        if(!output.open(QIODevice::WriteOnly))qFatal("Cannot write JavaScript evidence");
        // WebEngine converts only plain data; JSON strings make that contract explicit.
        output.write(value.toString().toUtf8());
        deleteLater();
    }
};

// Loaded only by the validation command via LD_PRELOAD. All injected input enters
// the real DorotiSurface event handler. This is automated Qt input, not physical.
static void Start() {
    const auto script=qgetenv("DOROTI_QT_TEST_SCRIPT");
    if(script.isEmpty())return;
    QTimer::singleShot(0,qApp,[script] {
        QFile file(QString::fromUtf8(script));
        if(!file.open(QIODevice::ReadOnly))qFatal("Cannot open validation script");
        auto steps=QJsonDocument::fromJson(file.readAll()).array();
        auto* timer=new QTimer(qApp);
        QObject::connect(timer,&QTimer::timeout,qApp,[timer,steps,index=0,retries=0]() mutable {
            QQuickWindow* window=nullptr;
            for(auto* w:QGuiApplication::topLevelWindows())
                if(w->title().contains("Doroti")) { window=qobject_cast<QQuickWindow*>(w);break; }
            if(!window||!window->isExposed())return;
            if(index>=steps.size()) { timer->stop();window->close();return; }
            const auto step=steps[index++].toObject();
            const auto action=step["action"].toString();
            if(step.contains("expectedNative")) {
                int count=0;
                for(auto* item:window->findChildren<QQuickItem*>())
                    if(item->objectName()=="doroti-quick-button"||item->objectName()=="doroti-quick-editor"||item->objectName()=="doroti-quick-webview")++count;
                if(count!=step["expectedNative"].toInt()) {
                    if(++retries>100)qFatal("Native lifecycle did not settle within 5 seconds");
                    --index;timer->setInterval(50);window->update();return;
                }
                retries=0;
            }
            const QPoint point(step["x"].toInt(),step["y"].toInt());
            if(action=="click")QTest::mouseClick(window,Qt::LeftButton,Qt::NoModifier,point);
            else if(action=="text") {
                QTest::keyClick(window,Qt::Key_A,Qt::ControlModifier);
                for(auto ch:step["text"].toString()) {
                    QKeyEvent press(QEvent::KeyPress,0,Qt::NoModifier,QString(ch));
                    QKeyEvent release(QEvent::KeyRelease,0,Qt::NoModifier,QString(ch));
                    QCoreApplication::sendEvent(window,&press);QCoreApplication::sendEvent(window,&release);
                }
            } else if(action=="wheel") {
                QWheelEvent event(point,window->mapToGlobal(point),QPoint(),QPoint(0,step["delta"].toInt()),
                    Qt::NoButton,Qt::NoModifier,Qt::NoScrollPhase,false);
                QCoreApplication::sendEvent(window,&event);
            } else if(action=="resize")window->resize(step["width"].toInt(),step["height"].toInt());
            else if(action=="capture") {
                const auto path=step["path"].toString();
                DorotiQtRecordPlatformOwner(window,path.toUtf8().constData());
                QJsonArray names;
                std::function<void(QAccessibleInterface*,int)> visit=[&](QAccessibleInterface* item,int depth) {
                    if(!item||depth>32||names.size()>512)return;
                    names.append(item->text(QAccessible::Name));
                    for(int i=0;i<item->childCount();i++)visit(item->child(i),depth+1);
                };
                visit(QAccessible::queryAccessibleInterface(window),0);
                QFile file(path+".json");
                if(file.open(QIODevice::ReadOnly)) {
                    auto record=QJsonDocument::fromJson(file.readAll()).object();file.close();
                    record.insert("accessibleNames",names);
                    QFile maps("/proc/self/maps");
                    record.insert("validationLayerLoaded",maps.open(QIODevice::ReadOnly)&&maps.readAll().contains("libVkLayer_khronos_validation.so"));
                    if(file.open(QIODevice::WriteOnly))file.write(QJsonDocument(record).toJson());
                }
            }
            else if(action=="js") {
                int number=0;
                for(auto* item:window->findChildren<QQuickItem*>())if(item->objectName()=="doroti-quick-webview") {
                    auto* engine=qmlEngine(item);
                    const auto path=step["path"].toString()+QString::number(++number)+".json";
                    engine->globalObject().setProperty("dorotiWriter",engine->newQObject(new ProbeWriter(path,engine)));
                    auto callback=engine->evaluate("(function(writer) { return function(value) { writer.save(value); }; })(dorotiWriter)");
                    if(callback.isError())qFatal("Cannot create JavaScript probe callback");
                    const auto code="JSON.stringify((()=>{"+step["code"].toString()+"})())";
                    if(!QMetaObject::invokeMethod(item,"runJavaScript",Q_ARG(QString,code),Q_ARG(QJSValue,callback)))
                        qFatal("WebEngine JavaScript probe invocation failed");
                }
            } else if(action!="wait")qFatal("Unknown validation action");
            timer->setInterval(step["wait"].toInt(500));
        });
        timer->start(1500);
    });
}
Q_COREAPP_STARTUP_FUNCTION(Start)
#include "product-driver.moc"
