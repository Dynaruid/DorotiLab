#pragma once
#include "doroti_qt_platform_views.h"
// Independent optional ABI. Does not change host callbacks ABI 4 or Quick part size.
// All operations are GUI-thread only. UTF-8 payloads are borrowed for the callback.
// bind(null) synchronously disconnects callbacks before the managed context is freed.
using doroti_web_callback = void(*)(void*, doroti_qt_utf8_v2);
struct doroti_qt_webview_api {
  std::uint32_t version, size;
  std::uint64_t features; // 1 navigation, 2 JS, 4 profiles, 8 app content, 16 trusted app messages
  int (*bind)(std::uint64_t owner, std::uint64_t id, doroti_web_callback, void*);
  int (*execute)(std::uint64_t owner, std::uint64_t id, doroti_qt_utf8_v2);
};
static_assert(sizeof(doroti_qt_webview_api)==32);
extern "C" DOROTI_QT_EXPORT int doroti_qt_get_webview_api(std::uint32_t, std::uint32_t, doroti_qt_webview_api*);
#ifdef DOROTI_QT_WEBENGINE
class QQmlEngine;
class QQuickItem;
class QByteArray;
// Bootstrap failures: 80 runtime version, 81 helper, 82 resources, 83 locales.
int DorotiWebInitialize();
QQuickItem* DorotiWebCreate(QQmlEngine*, const QByteArray&);
int DorotiWebBind(QQuickItem*, doroti_web_callback, void*);
int DorotiWebExecute(QQuickItem*, const QByteArray&);
#endif
