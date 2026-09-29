#include "doroti_qt_webview.h"
#include <QtWebEngineQuick/qtwebenginequickglobal.h>
#include <QtWebEngineQuick/QQuickWebEngineProfile>
#include <QtWebEngineQuick/QQuickWebEngineDownloadRequest>
#include <QWebEngineUrlScheme>
#include <QWebEngineUrlSchemeHandler>
#include <QWebEngineUrlRequestJob>
#include <QWebEngineUrlRequestInterceptor>
#include <QApplication>
#include <QBuffer>
#include <QFile>
#include <QFileInfo>
#include <QLibraryInfo>
#include <QDir>
#include <QCryptographicHash>
#include <QVersionNumber>
#include <QJsonArray>
#include <QJsonDocument>
#include <QJsonObject>
#include <QQmlComponent>
#include <QQmlEngine>
#include <QQuickItem>
#include <QPointer>
#include <QTimer>
#include <QWebChannel>
#include <map>
#include <algorithm>
#include <functional>
#include <stdexcept>

namespace {
constexpr int Limit=2*1024*1024;
QString JsonString(const QString& value) {
  auto bytes=QJsonDocument(QJsonArray{value}).toJson(QJsonDocument::Compact);
  return QString::fromUtf8(bytes.mid(1,bytes.size()-2));
}
class Content final : public QWebEngineUrlSchemeHandler {
 public:
  QJsonObject routes;
  explicit Content(QObject* parent):QWebEngineUrlSchemeHandler(parent) {}
  void requestStarted(QWebEngineUrlRequestJob* job) override {
    const auto url=job->requestUrl(); const auto path=url.path(QUrl::FullyEncoded);
    const auto headers=job->requestHeaders();
    const bool range=std::any_of(headers.keyBegin(),headers.keyEnd(),[](const QByteArray& name){return name.compare("Range",Qt::CaseInsensitive)==0;});
    if(job->requestMethod()!="GET"||url.host()!="content"||!url.userInfo().isEmpty()||url.port()!=-1||
       path.contains('%')||path.contains("..")||!routes.contains(path)||range) {
      job->fail(QWebEngineUrlRequestJob::RequestDenied);return;
    }
    const auto route=routes[path].toObject();
    auto* data=new QBuffer(job);data->setData(QByteArray::fromBase64(route["Data"].toString().toLatin1()));data->open(QIODevice::ReadOnly);
    job->reply(route["MimeType"].toString().toUtf8(),data);
  }
};
// app content is a closed manifest: no remote/file subresources or untrusted frames
// are allowed to acquire the trusted transport's authority.
class ContentPolicy final : public QWebEngineUrlRequestInterceptor {
 public:
  explicit ContentPolicy(QObject* parent):QWebEngineUrlRequestInterceptor(parent) {}
  void interceptRequest(QWebEngineUrlRequestInfo& info) override {
    if(info.firstPartyUrl().scheme()=="doroti-app") {
      const auto headers=info.httpHeaders();
      if(std::any_of(headers.keyBegin(),headers.keyEnd(),[](const QByteArray& name){return name.compare("Range",Qt::CaseInsensitive)==0;}))info.block(true);
      const auto scheme=info.requestUrl().scheme();
      if(scheme!="doroti-app"&&scheme!="data"&&scheme!="about")info.block(true);
      if(info.resourceType()==QWebEngineUrlRequestInfo::ResourceTypeSubFrame)info.block(true);
    }
  }
};
QPointer<QQuickWebEngineProfile> sharedProfile;
class Transport final : public QObject {
 Q_OBJECT
 public:
  std::function<void(const QString&)> deliver;
  explicit Transport(QObject* parent):QObject(parent) {}
  Q_INVOKABLE void receive(const QString& text) { if(deliver)deliver(text); }
};
class Bridge final : public QObject {
 Q_OBJECT
 Q_PROPERTY(QQuickWebEngineProfile* profile READ profile CONSTANT)
 Q_PROPERTY(QObject* transport READ transport CONSTANT)
 Q_PROPERTY(QString initialHtml READ initialHtml CONSTANT)
 Q_PROPERTY(QString transportScript READ transportScript CONSTANT)
 public:
  Transport* endpoint=nullptr;
  QObject* transport() const { return endpoint; }
  QQuickWebEngineProfile* profile() const { return store; }
  QString initialHtml() const { return options["Html"].toString("<!doctype html><meta charset=utf-8>"); }
  QString transportScript() const {
    if(!messages)return {};
    QFile source(":/qtwebchannel/qwebchannel.js");if(!source.open(QIODevice::ReadOnly))return {};
    return QString::fromUtf8(source.readAll())+QStringLiteral(R"JS(
if(location.origin==='doroti-app://content')new QWebChannel(qt.webChannelTransport,function(channel){
 window.addEventListener('doroti-message',function(event){if(typeof event.detail==='string'&&event.detail.length<=65536)channel.objects.doroti.receive(event.detail)});
});)JS");
  }
  QQuickWebEngineProfile* store=nullptr;
  QPointer<QQuickItem> view;
  QJsonObject options;
  doroti_web_callback callback=nullptr;void* context=nullptr;
  qint64 navigation=0,generation=0;
  bool expectingStart=false,terminal=false,messages=false;
  std::map<QString,qint64> pending;
  explicit Bridge(const QJsonObject& config):options(config) {
    setObjectName("doroti");
    endpoint=new Transport(this);endpoint->deliver=[this](const QString& text){receive(text);};
    const bool shared=options["Profile"].toInt()==1;
    const auto routes=options["NativeResources"].toObject();
    messages=!options["MessageOrigins"].toArray().isEmpty();
    // Per-view route maps must never overwrite a shared profile's scheme handler.
    if(shared&&!routes.isEmpty())throw std::invalid_argument("App content requires an ephemeral profile.");
    if(messages&&(routes.isEmpty()||options["MessageOrigins"].toArray()!=QJsonArray{QStringLiteral("doroti-app://content")}))
      throw std::invalid_argument("Messages require trusted app content and only doroti-app://content.");
    if(shared) {
      if(!sharedProfile) {
        const auto scope=QCryptographicHash::hash(options["NativeApplicationId"].toString("doroti").toUtf8(),QCryptographicHash::Sha256).toHex();
        sharedProfile=new QQuickWebEngineProfile("DorotiShared-"+QString::fromLatin1(scope),qApp);
      }
      store=sharedProfile;
    } else store=new QQuickWebEngineProfile(this);
    if(!shared)store->setOffTheRecord(true);
    store->setPersistentPermissionsPolicy(QQuickWebEngineProfile::PersistentPermissionsPolicy::AskEveryTime);
    QObject::connect(store,&QQuickWebEngineProfile::downloadRequested,this,[](QQuickWebEngineDownloadRequest* download){download->cancel();});
    if(!routes.isEmpty()) {
      auto* content=new Content(store);content->routes=routes;store->installUrlSchemeHandler("doroti-app",content);
      auto* policy=new ContentPolicy(store);store->setUrlRequestInterceptor(policy);
    }
  }
  void send(QJsonObject data) {
    if(!callback)return;
    auto bytes=QJsonDocument(data).toJson(QJsonDocument::Compact);
    callback(context,{reinterpret_cast<const std::uint8_t*>(bytes.constData()),std::uint64_t(bytes.size())});
  }
  QJsonObject snapshot() const {
    return {{"navigation",navigation},{"generation",generation},{"url",view?view->property("url").toUrl().toString():QString()},
      {"title",view?view->property("title").toString():QString()},{"loading",expectingStart||(view&&view->property("loading").toBool())},
      {"back",view&&view->property("canGoBack").toBool()},{"forward",view&&view->property("canGoForward").toBool()}};
  }
  void result(const QString& id,int error=-1,const QString& text={},bool undefined=false) {
    auto value=snapshot();value["request"]=id;value["error"]=error;value["text"]=text;value["undefined"]=undefined;send(value);
  }
  void event(int kind,const QString& error={}) { auto value=snapshot();value["event"]=kind;value["text"]=error;send(value); }
  void cancel(int code,const QString& message) { auto copy=std::move(pending);pending.clear();for(auto&[id,g]:copy)result(id,code,message); }
  void advance() { ++navigation;++generation;cancel(4,"Document changed."); }
  Q_INVOKABLE bool allows(const QString& text) const {
    const QUrl url(text);
    if(text=="about:blank"||text.startsWith("data:text/html"))return true;
    if(!url.isValid()||!url.userInfo().isEmpty())return false;
    if(url.scheme()=="doroti-app")return !options["NativeResources"].toObject().isEmpty()&&url.host()=="content"&&url.port()==-1;
    if(url.scheme()!="http"&&url.scheme()!="https")return false;
    if(options["AllowedOrigins"].isNull()||options["AllowedOrigins"].isUndefined())return true;
    auto origin=url.adjusted(QUrl::RemovePath|QUrl::RemoveQuery|QUrl::RemoveFragment).toString();
    if((url.scheme()=="https"&&url.port()==443)||(url.scheme()=="http"&&url.port()==80)) { auto canonical=url;canonical.setPort(-1);origin=canonical.adjusted(QUrl::RemovePath|QUrl::RemoveQuery|QUrl::RemoveFragment).toString(); }
    return options["AllowedOrigins"].toArray().contains(origin);
  }
  Q_INVOKABLE void loading(int status,const QString& error) {
    if(terminal)return;
    if(status==0) { if(!expectingStart)advance();expectingStart=false;event(0); }
    else if(status==2) {
      // Replacing the initial HTML while it is loading may complete without
      // another LoadStarted signal. Do not leave the admission latch set forever.
      expectingStart=false;
      if(messages&&view)QMetaObject::invokeMethod(view,"installFacade",Q_ARG(QVariant,QVariant(generation)));
      event(2);
    } else if(status==3) {expectingStart=false;event(3,error);}
  }
  Q_INVOKABLE void failed(const QString& reason) { terminal=true;cancel(6,reason);event(4,reason); }
  Q_INVOKABLE void receive(const QString& text) {
    if(!messages||terminal||expectingStart||!view||view->property("loading").toBool()||text.toUtf8().size()>65536)return;
    auto url=view->property("url").toUrl();if(url.scheme()!="doroti-app"||url.host()!="content")return;
    auto message=QJsonDocument::fromJson(text.toUtf8()).object();
    if(message["generation"].toInteger()!=generation||message["requestId"].toInteger()<=0||message["name"].toString().isEmpty()||message["name"].toString().size()>128)return;
    auto value=snapshot();value["event"]=5;value["message"]=message;send(value);
  }
  Q_INVOKABLE void scriptResult(const QString& id,const QVariant& value) {
    const auto it=pending.find(id);if(it==pending.end())return;
    auto prior=it->second;pending.erase(it);
    if(prior!=generation) {result(id,4,"Document changed.");return;}
    auto bytes=value.toString().toUtf8();auto json=QJsonDocument::fromJson(bytes).object();
    if(bytes.size()>Limit||!json.contains("ok")||!json["ok"].toBool()) {result(id,5,json["error"].toString("Unsupported JavaScript result."));return;}
    result(id,-1,json["json"].toString(),json["undefined"].toBool());
  }
  int execute(const QByteArray& data) {
    auto command=QJsonDocument::fromJson(data).object();auto id=command["request"].toString();
    if(id.isEmpty())return DOROTI_QT_ERROR_INVALID_ARGUMENT;
    const auto op=command["operation"].toInt(-1);const auto text=command["text"].toString();
    if(terminal) {result(id,6,"Recreate the failed WebView.");return 0;}
    if(command["generation"].toInteger()!=0&&command["generation"].toInteger()!=generation) {result(id,4,"Stale document.");return 0;}
    if(op<0||op>9||op==9) {result(id,2,"Full profile storage deletion is not available in the public Quick API.");return 0;}
    if(text.toUtf8().size()>Limit) {result(id,3,"Command exceeds 2 MiB.");return 0;}
    if(op==0) {
      auto value=snapshot();value["request"]=id;value["error"]=-1;value["features"]=QJsonObject{{"messages",messages},{"content",!options["NativeResources"].toObject().isEmpty()}};send(value);return 0;
    }
    if(op==1) {result(id);return 0;}
    if((op==2&&(!allows(text)||(!text.startsWith("http://")&&!text.startsWith("https://")&&!text.startsWith("doroti-app://"))))||
       (op==6&&!view->property("canGoBack").toBool())||(op==7&&!view->property("canGoForward").toBool())) {result(id,3,"Navigation rejected.");return 0;}
    QString script=text;
    if(op==8) {
      if(expectingStart||view->property("loading").toBool()) {result(id,0,"Document is loading.");return 0;}
      if(pending.size()>=32) {result(id,7,"32 script operations are already pending.");return 0;}
      script="(()=>{try{let v=(0,eval)("+JsonString(text)+");if(v&&typeof v.then==='function')throw Error('Promise results are unsupported');let j=JSON.stringify(v);if(v!==undefined&&j===undefined)throw Error('Non-JSON result');return JSON.stringify({ok:true,undefined:v===undefined,json:j});}catch(e){return JSON.stringify({ok:false,error:String(e)});}})()";
      pending[id]=generation;
      QTimer::singleShot(15000,this,[this,id]{if(pending.erase(id))result(id,7,"JavaScript timed out.");});
    } else if(op!=5) {advance();expectingStart=true;}
    if(!QMetaObject::invokeMethod(view,"dispatch",Q_ARG(QVariant,QVariant(op)),Q_ARG(QVariant,QVariant(script)),Q_ARG(QVariant,QVariant(id)))) {
      pending.erase(id);result(id,2,"QML command adapter missing.");return 0;
    }
    if(op!=8)result(id);return 0;
  }
};
Bridge* Get(QQuickItem* view) { return view?view->findChild<Bridge*>("doroti",Qt::FindDirectChildrenOnly):nullptr; }
}
int DorotiWebInitialize() {
  if(QVersionNumber::fromString(qVersion())<QVersionNumber(6,8,0))return 80;
  const auto helper=qEnvironmentVariable("QTWEBENGINEPROCESS_PATH",QLibraryInfo::path(QLibraryInfo::LibraryExecutablesPath)+"/QtWebEngineProcess");
  if(!QFileInfo(helper).isExecutable())return 81;
  const auto resources=qEnvironmentVariable("QTWEBENGINE_RESOURCES_PATH",QLibraryInfo::path(QLibraryInfo::DataPath)+"/resources");
  for(const auto* name:{"qtwebengine_resources.pak","qtwebengine_resources_100p.pak","qtwebengine_resources_200p.pak"})
    if(!QFileInfo(QDir(resources).filePath(name)).isReadable())return 82;
  const auto locales=qEnvironmentVariable("QTWEBENGINE_LOCALES_PATH",QLibraryInfo::path(QLibraryInfo::TranslationsPath)+"/qtwebengine_locales");
  if(!QFileInfo(QDir(locales).filePath("en-US.pak")).isReadable())return 83;
  QCoreApplication::setAttribute(Qt::AA_ShareOpenGLContexts);
  QWebEngineUrlScheme scheme("doroti-app");scheme.setSyntax(QWebEngineUrlScheme::Syntax::Host);
  scheme.setFlags(QWebEngineUrlScheme::SecureScheme|QWebEngineUrlScheme::LocalScheme|QWebEngineUrlScheme::LocalAccessAllowed|QWebEngineUrlScheme::FetchApiAllowed);
  QWebEngineUrlScheme::registerScheme(scheme);QtWebEngineQuick::initialize();return 0;
}
QQuickItem* DorotiWebCreate(QQmlEngine* engine,const QByteArray& parameters) {
  constexpr auto prefix="doroti-webview:1\n";
  QJsonObject options;
  if(parameters.startsWith(prefix)) {
    QJsonParseError error;auto document=QJsonDocument::fromJson(parameters.mid(int(strlen(prefix))),&error);
    if(error.error!=QJsonParseError::NoError||!document.isObject())throw std::invalid_argument("Malformed WebView options.");
    options=document.object();
  } else if(parameters.startsWith("doroti-webview:"))throw std::invalid_argument("Unsupported WebView creation protocol.");
  else options["Html"]=QString::fromUtf8(parameters);
  auto bridge=std::make_unique<Bridge>(options);
  QQmlEngine::setObjectOwnership(bridge.get(),QQmlEngine::CppOwnership);
  QQmlComponent component(engine,QUrl("qrc:/doroti/qml/WebView.qml"));
  auto* item=qobject_cast<QQuickItem*>(component.createWithInitialProperties({{"bridge",QVariant::fromValue(bridge.get())}}));
  if(!item) {qWarning()<<component.errors();throw std::runtime_error("QtWebEngine QML module could not create the WebView.");}
  bridge->view=item;bridge->setParent(item);bridge.release();return item;
}
int DorotiWebBind(QQuickItem* item,doroti_web_callback callback,void* context) {
  auto* bridge=Get(item);if(!bridge)return DOROTI_QT_PV_UNSUPPORTED;
  bridge->callback=callback;bridge->context=context;return 0;
}
int DorotiWebExecute(QQuickItem* item,const QByteArray& command) {
  auto* bridge=Get(item);return bridge?bridge->execute(command):DOROTI_QT_PV_UNSUPPORTED;
}
#include "doroti_webview_qt.moc"
