#include "doroti_qt_quick.h"
#include "doroti_qt_webview.h"
#include <QApplication>
#include <QFile>
#include <QJsonArray>
#include <QJsonDocument>
#include <QJsonObject>
#include <QKeyEvent>
#include <QMouseEvent>
#include <QWheelEvent>
#include <QTouchEvent>
#include <QTabletEvent>
#include <QQmlComponent>
#include <QQmlEngine>
#include <QQuickItem>
#include <QQuickWindow>
#include <QSGSimpleTextureNode>
#include <QSGTexture>
#include <QSGRendererInterface>
#include <QVulkanInstance>
#include <QPointer>
#include <QThread>
#include <QScreen>
#include <QTimer>
#include <map>
#include <memory>
#include <mutex>
#include <set>
#include <cmath>
#include <stdexcept>
#include <vector>
#include <bit>

namespace {
struct TextureNode final : QSGSimpleTextureNode { std::uint64_t identity=0; };
class RasterItem final : public QQuickItem {
 public:
  std::uint64_t image=0,identity=0;
  QSize pixels;
  explicit RasterItem(QQuickItem* parent):QQuickItem(parent) { setFlag(ItemHasContents);setAcceptedMouseButtons(Qt::NoButton); }
 protected:
  QSGNode* updatePaintNode(QSGNode* previous,UpdatePaintNodeData*) override {
    auto* node=static_cast<TextureNode*>(previous);
    if(!image) { delete node;return nullptr; }
    auto* texture=node?node->texture():nullptr;
    auto* native=texture?texture->nativeInterface<QNativeInterface::QSGVulkanTexture>():nullptr;
    if(!node || node->identity!=identity || !native || reinterpret_cast<std::uint64_t>(native->nativeImage())!=image || texture->textureSize()!=pixels) {
      texture=QNativeInterface::QSGVulkanTexture::fromNative(reinterpret_cast<VkImage>(image),
          VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL,window(),pixels,QQuickWindow::TextureHasAlphaChannel);
      if(!texture) { delete node; return nullptr; }
      // Destroy the old node together with its owned wrapper. Replacing the
      // pointer alone leaves imported texture resources alive across resize.
      delete node;
      node=new TextureNode;
      node->identity=identity;
      node->setTexture(texture);node->setOwnsTexture(true);
      node->setFiltering(QSGTexture::Nearest);
    }
    node->setRect(boundingRect());return node;
  }
};
struct Control { QQuickItem *clip,*item;void(*focused)(void*,std::uint64_t);void* context; };
struct Pending { void(*callback)(void*,int);void* context; };
struct Owner {
  QQuickWindow* window;
  std::unique_ptr<QQmlEngine> engine;
  std::map<std::uint64_t,Control> controls;
  std::map<std::uint64_t,Pending> pending;
  std::vector<RasterItem*> rasters;
  std::vector<doroti_qt_quick_part> parts;
  QQuickItem* sourceGroup=nullptr;
  QQuickItem* effect=nullptr;
  bool mouseDragActive=false;
  bool nativeDrag=false;
  std::map<std::uint64_t,bool> touchOwners;
  std::map<std::uint64_t,bool> tabletOwners;
};
std::mutex gate;
std::map<std::uint64_t,std::unique_ptr<Owner>> owners;
std::uint64_t nextId=0;
bool Gui() { return qApp && QThread::currentThread()==qApp->thread(); }
Owner* Find(std::uint64_t id) { auto it=owners.find(id);return it==owners.end()?nullptr:it->second.get(); }
Owner* Find(QWindow* window) { for(auto&[id,owner]:owners)if(owner->window==window)return owner.get();return nullptr; }
int Lookup(std::uint64_t id,Owner*& owner) { if(!Gui())return DOROTI_QT_PV_WRONG_THREAD;std::lock_guard lock(gate);owner=Find(id);return owner?0:DOROTI_QT_PV_CLOSED; }
QRectF Rect(doroti_qt_pv_rect r) { return {r.x,r.y,r.width,r.height}; }
bool Valid(doroti_qt_pv_rect r) { return std::isfinite(r.x)&&std::isfinite(r.y)&&std::isfinite(r.width)&&std::isfinite(r.height)&&
  std::abs(r.x)<=1e6&&std::abs(r.y)<=1e6&&r.width>=0&&r.height>=0&&r.width<=1e6&&r.height<=1e6; }
void Place(Control& c,QRectF bounds,QRectF clip,int z,bool visible) {
  clip=clip.intersected(bounds);
  if(clip.isEmpty()||!visible) { c.clip->setVisible(false);return; }
  c.clip->setPosition(clip.topLeft());c.clip->setSize(clip.size());c.clip->setZ(z);
  c.item->setPosition(bounds.topLeft()-clip.topLeft());c.item->setSize(bounds.size());c.clip->setVisible(true);
}
int Post(std::uint64_t id,void(*callback)(void*,int),void* context) {
  if(!callback)return DOROTI_QT_ERROR_INVALID_ARGUMENT;
  std::lock_guard lock(gate);auto* owner=Find(id);if(!owner)return DOROTI_QT_PV_CLOSED;
  if(owner->pending.size()>=1024)return DOROTI_QT_PV_UNSUPPORTED;
  auto task=++nextId;owner->pending.emplace(task,Pending{callback,context});
  bool posted=QMetaObject::invokeMethod(owner->window,[id,task]{
    Pending work{};
    { std::lock_guard lock(gate);auto* owner=Find(id);if(!owner)return;auto it=owner->pending.find(task);if(it==owner->pending.end())return;work=it->second;owner->pending.erase(it); }
    work.callback(work.context,0);
  },Qt::QueuedConnection);
  if(!posted) { owner->pending.erase(task);return DOROTI_QT_PV_CLOSED; }return 0;
}
int Create(std::uint64_t id,std::uint32_t kind,doroti_qt_utf8_v2 text,void(*focused)(void*,std::uint64_t),void* context,std::uint64_t* result) {
  Owner* owner;auto status=Lookup(id,owner);if(status)return status;
  if(kind>2||!result||text.length>32*1024*1024||(text.length&&!text.data))return DOROTI_QT_ERROR_INVALID_ARGUMENT;
#ifndef DOROTI_QT_WEBENGINE
  if(kind==2)return DOROTI_QT_PV_UNSUPPORTED;
#endif
  QQmlComponent component(owner->engine.get());
  component.setData(kind==2?QByteArray("import QtQuick\nimport QtWebEngine\nWebEngineView { objectName: 'doroti-quick-webview'; property string initialHtml; Component.onCompleted: loadHtml(initialHtml) }"):
      kind==1?QByteArray("import QtQuick\nimport QtQuick.Controls\nTextField { objectName: 'doroti-quick-editor'; text: 'Edit native Qt Quick text'; selectByMouse: true; property int pressCount: 0; onPressed: pressCount++ }"):
      QByteArray("import QtQuick\nimport QtQuick.Controls\nButton { objectName: 'doroti-quick-button'; text: 'Native Qt Quick button'; property int clickCount: 0; onClicked: clickCount++ }"),QUrl());
  QVariantMap properties;
  if(kind==2)properties.insert("initialHtml",QString::fromUtf8(reinterpret_cast<const char*>(text.data),qsizetype(text.length)));
  std::unique_ptr<QObject> created;
#ifdef DOROTI_QT_WEBENGINE
  if(kind==2)created.reset(DorotiWebCreate(owner->engine.get(),QByteArray(reinterpret_cast<const char*>(text.data),qsizetype(text.length))));
  else
#endif
  created.reset(component.createWithInitialProperties(properties));
  auto* item=qobject_cast<QQuickItem*>(created.get());
  if(!item) { qWarning()<<component.errors();return DOROTI_QT_PV_UNSUPPORTED; }
  auto* clip=new QQuickItem(owner->window->contentItem());clip->setClip(true);clip->setVisible(false);
  item->setParent(clip);item->setParentItem(clip);
  std::uint64_t token;{ std::lock_guard lock(gate);token=++nextId; }
  owner->controls.emplace(token,Control{clip,item,focused,context});
  created.release();
  QObject::connect(item,&QQuickItem::activeFocusChanged,owner->window,[id,token]{
    auto* owner=Find(id);if(!owner)return;auto it=owner->controls.find(token);if(it==owner->controls.end()||!it->second.item->hasActiveFocus())return;
    QMetaObject::invokeMethod(owner->window,[id,token]{auto* o=Find(id);if(!o)return;auto c=o->controls.find(token);
      if(c!=o->controls.end()&&c->second.focused)c->second.focused(c->second.context,token);},Qt::QueuedConnection);
  });
  *result=token;return 0;
}
int Adopt(std::uint64_t,void*,void(*)(void*,std::uint64_t),void*,std::uint64_t*) { return DOROTI_QT_PV_UNSUPPORTED; }
int Commit(std::uint64_t id,const doroti_qt_pv_placement* items,std::uint64_t count,std::uint32_t apply) {
  Owner* owner;auto status=Lookup(id,owner);if(status)return status;
  if(count>256||(count&&!items))return DOROTI_QT_ERROR_INVALID_ARGUMENT;
  std::set<std::uint64_t> seen;
  for(std::uint64_t i=0;i<count;i++)if(items[i].struct_size!=sizeof(*items)||!Valid(items[i].bounds)||!Valid(items[i].clip)||
      !owner->controls.contains(items[i].id)||!seen.insert(items[i].id).second)return DOROTI_QT_PV_STALE;
  if(!apply)return 0;
  for(auto&[token,c]:owner->controls)if(!seen.contains(token))c.clip->setVisible(false);
  for(std::uint64_t i=0;i<count;i++) { auto&c=owner->controls.at(items[i].id);Place(c,Rect(items[i].bounds),Rect(items[i].clip),int(c.clip->z()),items[i].visible); }
  return 0;
}
int Focus(std::uint64_t id,std::uint64_t token,std::uint32_t focused) {
  Owner* owner;auto status=Lookup(id,owner);if(status)return status;
  auto it=owner->controls.find(token);if(it==owner->controls.end())return DOROTI_QT_PV_STALE;
  if(focused) { if(!it->second.clip->isVisible())return DOROTI_QT_PV_UNSUPPORTED;it->second.item->forceActiveFocus(Qt::OtherFocusReason); }
  else if(it->second.item->hasActiveFocus()) {
    it->second.item->setFocus(false,Qt::OtherFocusReason);
    owner->window->contentItem()->forceActiveFocus();
  }return 0;
}
int Remove(std::uint64_t id,std::uint64_t token) {
  Owner* owner;auto status=Lookup(id,owner);if(status)return status;
  auto it=owner->controls.find(token);if(it==owner->controls.end())return DOROTI_QT_PV_STALE;
  auto c=it->second;owner->controls.erase(it);
  DorotiQtQuickCancelNativeInput(owner->window);
#ifdef DOROTI_QT_WEBENGINE
  DorotiWebBind(c.item,nullptr,nullptr);
#endif
  delete c.clip;return 0;
}
#ifdef DOROTI_QT_WEBENGINE
int WebBind(std::uint64_t id,std::uint64_t token,doroti_web_callback callback,void* context) {
  Owner* owner;auto status=Lookup(id,owner);if(status)return status;
  auto it=owner->controls.find(token);if(it==owner->controls.end())return DOROTI_QT_PV_STALE;
  return DorotiWebBind(it->second.item,callback,context);
}
int WebExecute(std::uint64_t id,std::uint64_t token,doroti_qt_utf8_v2 data) {
  Owner* owner;auto status=Lookup(id,owner);if(status)return status;
  if(data.length>16*1024*1024||(data.length&&!data.data))return DOROTI_QT_ERROR_INVALID_ARGUMENT;
  auto it=owner->controls.find(token);if(it==owner->controls.end())return DOROTI_QT_PV_STALE;
  return DorotiWebExecute(it->second.item,QByteArray(reinterpret_cast<const char*>(data.data),qsizetype(data.length)));
}
#endif
template<auto>struct Boundary;
template<typename...Args,int(*Function)(Args...)>struct Boundary<Function>{static int Call(Args...args)noexcept{try{return Function(args...);}catch(...){return DOROTI_QT_ERROR_NATIVE_EXCEPTION;}}};
const doroti_qt_pv_api api{1,sizeof(doroti_qt_pv_api),27
#ifdef DOROTI_QT_WEBENGINE
  |4
#endif
  ,Boundary<Post>::Call,Boundary<Create>::Call,Boundary<Adopt>::Call,
  Boundary<Commit>::Call,Boundary<Focus>::Call,Boundary<Remove>::Call};
}
extern "C" DOROTI_QT_EXPORT int doroti_qt_get_webview_api(std::uint32_t version,std::uint32_t size,doroti_qt_webview_api* output) {
  if(version!=1)return DOROTI_QT_ERROR_ABI_VERSION;
  if(size!=sizeof(doroti_qt_webview_api))return DOROTI_QT_ERROR_ABI_SIZE;
  if(!output)return DOROTI_QT_ERROR_INVALID_ARGUMENT;
#ifdef DOROTI_QT_WEBENGINE
  *output={1,sizeof(doroti_qt_webview_api),31,Boundary<WebBind>::Call,Boundary<WebExecute>::Call};return 0;
#else
  return DOROTI_QT_PV_UNSUPPORTED;
#endif
}
void DorotiQtRegisterPlatformOwner(QWindow* window) {
  auto owner=std::make_unique<Owner>();owner->window=qobject_cast<QQuickWindow*>(window);
  if(!owner->window)throw std::invalid_argument("Quick owner requires QQuickWindow");
  owner->engine=std::make_unique<QQmlEngine>();
  std::lock_guard lock(gate);owners.emplace(++nextId,std::move(owner));
}
void DorotiQtClosePlatformOwner(QWindow* window) {
  std::unique_ptr<Owner> owner;
  { std::lock_guard lock(gate);for(auto it=owners.begin();it!=owners.end();++it)if(it->second->window==window){owner=std::move(it->second);owners.erase(it);break;} }
  if(!owner)return;
#ifdef DOROTI_QT_WEBENGINE
  // A canceled managed post may release its session/context. Disconnect before
  // those callbacks run, and before QML destruction can finish pending scripts.
  for(auto&[id,c]:owner->controls)DorotiWebBind(c.item,nullptr,nullptr);
#endif
  for(auto&[id,work]:owner->pending)work.callback(work.context,DOROTI_QT_PV_CLOSED);
  for(auto&[id,c]:owner->controls)delete c.clip;
  for(auto* item:owner->rasters)delete item;
  delete owner->effect;
  delete owner->sourceGroup;
}
extern "C" DOROTI_QT_EXPORT int doroti_qt_get_platform_views(void* window,std::uint32_t version,std::uint32_t size,std::uint64_t* result,doroti_qt_pv_api* output) {
  if(!Gui())return DOROTI_QT_PV_WRONG_THREAD;
  if(version!=1)return DOROTI_QT_ERROR_ABI_VERSION;if(size!=sizeof(api))return DOROTI_QT_ERROR_ABI_SIZE;
  if(!result||!output)return DOROTI_QT_ERROR_INVALID_ARGUMENT;
  std::lock_guard lock(gate);for(auto&[id,owner]:owners)if(owner->window==window){*result=id;*output=api;return 0;}return DOROTI_QT_PV_CLOSED;
}
extern "C" DOROTI_QT_EXPORT int doroti_qt_quick_get_gpu(void* window,doroti_qt_quick_gpu* gpu) {
  if(!Gui())return DOROTI_QT_PV_WRONG_THREAD;
  if(!gpu||gpu->version!=1||gpu->size!=sizeof(*gpu))return DOROTI_QT_ERROR_ABI_SIZE;
  auto* owner=Find(static_cast<QWindow*>(window));if(!owner)return DOROTI_QT_PV_CLOSED;
  auto* w=owner->window;auto* r=w->rendererInterface();
  if(r->graphicsApi()!=QSGRendererInterface::Vulkan)return DOROTI_QT_PV_UNSUPPORTED;
  auto resource=[&](auto key){return r->getResource(w,key);};
  auto* device=static_cast<VkDevice*>(resource(QSGRendererInterface::DeviceResource));
  auto* physical=static_cast<VkPhysicalDevice*>(resource(QSGRendererInterface::PhysicalDeviceResource));
  auto* queue=static_cast<VkQueue*>(resource(QSGRendererInterface::CommandQueueResource));
  auto* family=static_cast<std::uint32_t*>(resource(QSGRendererInterface::GraphicsQueueFamilyIndexResource));
  auto* instance=static_cast<QVulkanInstance*>(resource(QSGRendererInterface::VulkanInstanceResource));
  if(!device||!physical||!queue||!family||!instance)return DOROTI_QT_PV_UNSUPPORTED;
  gpu->device=*device;gpu->physical_device=*physical;gpu->queue=*queue;gpu->instance=instance->vkInstance();gpu->queue_family=*family;
  auto version=instance->apiVersion();gpu->api_version=VK_MAKE_VERSION(version.majorVersion(),version.minorVersion(),version.microVersion());return 0;
}
extern "C" DOROTI_QT_EXPORT int doroti_qt_quick_commit(void* window,const doroti_qt_quick_part* parts,std::uint64_t count,std::uint32_t apply) {
  try {
    if(!Gui())return DOROTI_QT_PV_WRONG_THREAD;
    auto* owner=Find(static_cast<QWindow*>(window));if(!owner)return DOROTI_QT_PV_CLOSED;
    if(count>64||(count&&!parts))return DOROTI_QT_ERROR_INVALID_ARGUMENT;
    std::set<std::uint64_t> seen;std::size_t rasters=0;const doroti_qt_quick_part* effect=nullptr;
    for(std::uint64_t i=0;i<count;i++) { const auto&p=parts[i];
      if(p.size!=sizeof(p)||p.kind>4||!Valid(p.bounds)||!Valid(p.clip))return DOROTI_QT_ERROR_INVALID_ARGUMENT;
      if(p.kind==1&&(!owner->controls.contains(p.id)||!seen.insert(p.id).second))return DOROTI_QT_PV_STALE;
      if(p.kind==0&&(!p.image||!p.pixel_width||!p.pixel_height||p.pixel_width>16384||p.pixel_height>16384||++rasters>17))return DOROTI_QT_PV_UNSUPPORTED;
      if(p.kind==3 || p.kind==4) {
        const auto saturation=p.kind==4?std::bit_cast<double>(p.image):1.0;
        if(!std::isfinite(saturation)||saturation<0||saturation>2)return DOROTI_QT_PV_UNSUPPORTED;
        const auto sigma=std::bit_cast<double>(p.id);
        const auto dpr=owner->window->devicePixelRatio();
        const auto required=Rect(p.bounds).adjusted(-3*sigma,-3*sigma,3*sigma,3*sigma);
        const auto sample=Rect(p.clip);
        const bool sameSample=std::abs(sample.x()-required.x())<1e-7&&std::abs(sample.y()-required.y())<1e-7&&
            std::abs(sample.width()-required.width())<1e-7&&std::abs(sample.height()-required.height())<1e-7;
        if(effect||!std::isfinite(sigma)||sigma<0||sigma>32||Rect(p.bounds).isEmpty()||
            !sameSample||p.clip.width*dpr>4096||p.clip.height*dpr>4096||
            std::ceil(p.clip.width*dpr)*std::ceil(p.clip.height*dpr)>4*1024*1024)
          return DOROTI_QT_PV_UNSUPPORTED;
        effect=&p;
      }
    }
    if(!apply)return 0;
    // Prepare fallible QML/resource allocations before changing the displayed batch.
    std::unique_ptr<QObject> newEffect;
    std::unique_ptr<QQuickItem> newGroup;
    if(effect&&!owner->effect) {
      QQmlComponent component(owner->engine.get(),QUrl("qrc:/doroti/qml/Backdrop.qml"));
      newEffect.reset(component.create());
      if(!qobject_cast<QQuickItem*>(newEffect.get())) { qWarning()<<component.errors();return DOROTI_QT_PV_UNSUPPORTED; }
      newGroup=std::make_unique<QQuickItem>();
    }
    std::vector<doroti_qt_quick_part> next;
    if(count)next.assign(parts,parts+count);
    while(owner->rasters.size()<rasters)owner->rasters.push_back(new RasterItem(owner->window->contentItem()));
    auto* root=owner->window->contentItem();
    if(newEffect) {
      owner->effect=qobject_cast<QQuickItem*>(newEffect.release());
      owner->effect->setParent(root);owner->effect->setParentItem(root);
      owner->sourceGroup=newGroup.release();
      owner->sourceGroup->setParent(root);owner->sourceGroup->setParentItem(root);
    }
    if(effect) {
      owner->sourceGroup->setSize(root->size());owner->sourceGroup->setZ(0);
      owner->effect->setProperty("background",QVariant::fromValue(owner->sourceGroup));
      owner->effect->setProperty("sampleRect",Rect(effect->clip));
      owner->effect->setProperty("outputRect",Rect(effect->bounds));
      owner->effect->setProperty("sigma",std::bit_cast<double>(effect->id));
      owner->effect->setProperty("saturation",effect->kind==4?std::bit_cast<double>(effect->image):1.0);
      owner->effect->setZ(effect-parts+1);
    }
    // Reparent visual items only; QObject ownership and native identity stay stable.
    QPointer<QQuickItem> focused=owner->window->activeFocusItem();
  bool hiddenControl=false;
  for(auto&[id,c]:owner->controls)if(!seen.contains(id)) {
      hiddenControl=hiddenControl||c.clip->isVisible();
      c.clip->setVisible(false);
      if(c.clip->parentItem()!=root)c.clip->setParentItem(root);
    }
    std::size_t raster=0;
    for(std::uint64_t i=0;i<count;i++) { const auto&p=parts[i];
      auto* parent=effect && &p<effect?owner->sourceGroup:root;
      if(p.kind==1) { auto& c=owner->controls.at(p.id);
        if(c.clip->parentItem()!=parent)c.clip->setParentItem(parent);
        const bool wasVisible=c.clip->isVisible();
        Place(c,Rect(p.bounds),Rect(p.clip),i+1,true);
        hiddenControl=hiddenControl||(wasVisible&&!c.clip->isVisible()); }
      else if(p.kind==0) { auto* item=owner->rasters[raster++];item->image=p.image;item->identity=p.id;item->pixels=QSize(p.pixel_width,p.pixel_height);
        item->setParentItem(parent);
        item->setPosition(Rect(p.bounds).topLeft());item->setSize(Rect(p.bounds).size());item->setZ(i+1);item->setVisible(true);item->update(); }
    }
    // Hidden QSG nodes must not retain wrappers for images that managed retirement
    // will reclaim on the next GUI/render iteration.
    while(owner->rasters.size()>rasters) { delete owner->rasters.back();owner->rasters.pop_back(); }
    if(!effect) { delete owner->effect;owner->effect=nullptr;delete owner->sourceGroup;owner->sourceGroup=nullptr; }
    if(focused&&focused->isVisible()&&!focused->hasActiveFocus())focused->forceActiveFocus(Qt::OtherFocusReason);
    owner->parts=std::move(next);
    if(hiddenControl) DorotiQtQuickCancelNativeInput(owner->window);
    return 0;
  }catch(...){return DOROTI_QT_ERROR_NATIVE_EXCEPTION;}
}
bool DorotiQtQuickHasNativeFocus(QWindow* window) {
  auto* o=Find(window);if(!o)return false;auto* focus=o->window->activeFocusItem();
  for(auto&[id,c]:o->controls)if(focus&&(focus==c.item||c.item->isAncestorOf(focus))&&c.clip->isVisible())return true;return false;
}
void DorotiQtQuickClearFocus(QWindow* window) {
  auto* o=Find(window);if(!o)return;
  if(auto* focus=o->window->activeFocusItem())focus->setFocus(false,Qt::MouseFocusReason);
  // Clear the outer WebEngine focus scope as well as its internal editor.
  // Focusing the root alone lets Qt restore that scope's remembered child.
  for(auto&[id,c]:o->controls)c.item->setFocus(false,Qt::MouseFocusReason);
  o->window->contentItem()->forceActiveFocus(Qt::MouseFocusReason);
}
bool DorotiQtQuickNativeInput(QWindow* window,QEvent* event) {
  auto* o=Find(window);if(!o)return false;
  if(event->type()==QEvent::UngrabMouse) {
    o->mouseDragActive=false;o->nativeDrag=false;
    return false;
  }
  if(event->type()==QEvent::KeyPress||event->type()==QEvent::KeyRelease||event->type()==QEvent::InputMethod||event->type()==QEvent::InputMethodQuery)
    return DorotiQtQuickHasNativeFocus(window);
  auto hit=[o](QPointF point) {
    for(auto it=o->parts.rbegin();it!=o->parts.rend();++it)
      if((it->kind==1||it->kind==2)&&Rect(it->bounds).intersected(Rect(it->clip)).contains(point))
        return it->kind==1&&o->controls.contains(it->id)&&o->controls.at(it->id).clip->isVisible();
    return false;
  };
  if(auto* touch=dynamic_cast<QTouchEvent*>(event)) {
    const auto device=static_cast<std::uint64_t>(touch->pointingDevice()->systemId());
    auto found=o->touchOwners.find(device);
    bool native=found!=o->touchOwners.end()?found->second:
        !touch->points().isEmpty()&&hit(touch->points().first().scenePosition());
    if(event->type()==QEvent::TouchEnd||event->type()==QEvent::TouchCancel)
      o->touchOwners.erase(device);
    else o->touchOwners[device]=native;
    return native;
  }
  if(auto* tablet=dynamic_cast<QTabletEvent*>(event)) {
    const auto device=static_cast<std::uint64_t>(tablet->device()->systemId());
    auto found=o->tabletOwners.find(device);
    const bool native=found!=o->tabletOwners.end()?found->second:hit(tablet->position());
    if(event->type()==QEvent::TabletRelease) o->tabletOwners.erase(device);
    else if(event->type()==QEvent::TabletPress) o->tabletOwners[device]=native;
    return native;
  }
  QPointF point;
  auto* mouse=dynamic_cast<QMouseEvent*>(event);
  if(mouse)point=mouse->position();
  else if(auto* wheel=dynamic_cast<QWheelEvent*>(event))point=wheel->position();
  else return false;
  bool native=hit(point);
  if(mouse) {
    if(event->type()==QEvent::MouseButtonPress||event->type()==QEvent::MouseButtonDblClick) {
      if(!o->mouseDragActive) {o->nativeDrag=native;o->mouseDragActive=true;}
      native=o->nativeDrag;
    } else if(event->type()==QEvent::MouseButtonRelease) {
      if(o->mouseDragActive)native=o->nativeDrag;
      if(mouse->buttons()==Qt::NoButton) {o->mouseDragActive=false;o->nativeDrag=false;}
    } else if(o->mouseDragActive) native=o->nativeDrag;
  }
  return native;
}
void DorotiQtQuickCancelNativeInput(QWindow* window) {
  auto* o=Find(window);if(!o)return;
  o->mouseDragActive=false;o->nativeDrag=false;
  o->touchOwners.clear();o->tabletOwners.clear();
}
void DorotiQtRecordPlatformOwner(QWindow* window,const char* path) {
  if(!path||!*path)return;auto* o=Find(window);if(!o)return;
  QJsonArray controls;
  for(auto&[id,c]:o->controls)controls.append(QJsonObject{{"id",qint64(id)},{"visible",c.clip->isVisible()},
    {"kind",c.item->objectName()},{"text",c.item->property("text").toString()},
    {"clickCount",c.item->property("clickCount").toInt()},{"pressCount",c.item->property("pressCount").toInt()},
    {"focused",c.item->hasActiveFocus()},
    {"x",c.clip->x()},{"y",c.clip->y()},{"width",c.clip->width()},{"height",c.clip->height()},{"z",c.clip->z()}});
  // Validation-only GPU readback; QScreen::grabWindow cannot capture Wayland clients.
  // Qt cannot read back a hidden/minimized surface; a grab may wait for a
  // scene-graph frame that the compositor will never expose.
  QImage capture;
  if(o->window->isVisible()&&o->window->isExposed())capture=o->window->grabWindow();
  bool saved=!capture.isNull()&&capture.save(QString::fromUtf8(path)+".png");
  QFile file(QString::fromUtf8(path)+".json");if(file.open(QIODevice::WriteOnly))file.write(QJsonDocument(QJsonObject{
    {"qpa",QGuiApplication::platformName()},{"composition","qt-quick-vulkan-gpu-textures"},{"cpuReadback",false},
    {"windowWidth",o->window->width()},{"windowHeight",o->window->height()},
    {"windowDpr",o->window->devicePixelRatio()},{"windowVisible",o->window->isVisible()},
    {"windowState",int(o->window->windowState())},
    {"validationCaptureReadback",saved},{"rasterItems",int(o->rasters.size())},{"effectItems",o->effect?1:0},
    {"frameworkEffectState",QString::fromUtf8(qgetenv("DOROTI_PLATFORM_EFFECT_PROBE_STATE"))},
    {"effectSourceGroup",o->sourceGroup!=nullptr},{"controls",controls},{"windowCapture",saved}}).toJson());
}
