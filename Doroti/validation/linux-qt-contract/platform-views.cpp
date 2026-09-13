// Actual host attachment/lifecycle probe; render callbacks do not draw Graphite.
// Product compositor evidence is recorded separately.
#include "doroti_qt_host_v2.h"
#include <dlfcn.h>
#include "doroti_qt_platform_views.h"
#include <QApplication>
#include <QLineEdit>
#include <QPushButton>
#include <QThread>
#include <QTimer>
#include <QWindow>
#include <QScreen>
#include <QTest>
#include <QWebEngineView>
#include <QWebEngineUrlScheme>
#include <thread>
#include <atomic>
#include <source_location>
#include <cstdio>
#include <stdexcept>
#include <string>
#include <cstring>

namespace {
const doroti_qt_host_api_v2* api;
int renders, polls, releases, closed;
bool valid = true;
std::string extensions;
std::uint64_t generation, identity;
doroti_qt_pv_api pv;
std::uint64_t owner, button, editor;
void* native_view;
int prepare_count, focused_count, accepted_tasks, cancelled_tasks;
QLineEdit* editor_widget;
QWebEngineView* web;
bool web_loaded;
std::uint64_t web_id;
std::string screenshot;
using Get = decltype(&doroti_qt_get_platform_views);
Get get;
void Check(bool condition, std::source_location location = std::source_location::current());
void CheckStep(bool condition, const char* step) {
  Check(condition); std::printf("%s: %s\n", step, condition ? "PASS" : "FAIL");
}
doroti_qt_utf8_v2 Utf8(const char* s) { return {reinterpret_cast<const std::uint8_t*>(s), std::strlen(s)}; }
doroti_qt_pv_placement Place(std::uint64_t id, double x, double y, double w, double h) {
  return {sizeof(doroti_qt_pv_placement), 1, id, {x,y,w,h}, {x,y,w,h}};
}
void Finish() {
  CheckStep(web_loaded, "WebEngine live load/JS/state preservation");
  CheckStep(pv.remove(owner, web_id) == 0, "adopted WebEngine disposal");
  bool cycles_ok = true;
  for (int i=0; i<100; ++i) {
    std::uint64_t id;
    cycles_ok &= pv.create(owner, 0, Utf8("cycle"), nullptr, nullptr, &id) == 0;
    cycles_ok &= pv.remove(owner, id) == 0;
    cycles_ok &= pv.remove(owner, id) == DOROTI_QT_PV_STALE;
  }
  CheckStep(cycles_ok, "100 create/remove and stale ID rejection");
  // request_close queues first; all subsequently accepted posts must terminate
  // with CLOSED, and no callback may be dropped on QObject destruction.
  api->request_close(native_view);
  for (int i=0; i<20; ++i) {
    auto result = pv.post(owner, [](void*, int status) { if (status == 71) ++cancelled_tasks; else valid = false; }, nullptr);
    Check(result == 0); ++accepted_tasks;
  }
}
void Exercise() {
  auto focus = +[](void*, std::uint64_t) { ++focused_count; };
  CheckStep(pv.create(owner, 0, Utf8("Native Qt button"), focus, nullptr, &button) == 0 &&
      pv.create(owner, 1, Utf8("preserved"), focus, nullptr, &editor) == 0, "create built-in Widgets");
  for (auto* widget : QApplication::allWidgets()) if (auto* line = qobject_cast<QLineEdit*>(widget)) editor_widget = line;
  CheckStep(editor_widget != nullptr, "real QLineEdit");
  doroti_qt_pv_placement items[] = {Place(button, 20, 20, 220, 45), Place(editor,20,90,220,45)};
  CheckStep(pv.commit(owner, items, 2, 1) == 0, "disjoint B attachment");
  const auto old_geometry = editor_widget->parentWidget()->geometry();
  auto overlap = items[1]; overlap.bounds.y = overlap.clip.y = 20;
  doroti_qt_pv_placement bad[] = {items[0], overlap};
  CheckStep(pv.commit(owner,bad,2,1) == 74 && editor_widget->parentWidget()->geometry() == old_geometry, "overlap rejection without partial mutation");
  auto fractional = items[1]; fractional.bounds.x = 20.25;
  CheckStep(pv.commit(owner,&fractional,1,1) == 64, "fractional placement rejection");
  auto invalid = items[1]; invalid.id = ~std::uint64_t(0);
  CheckStep(pv.commit(owner,&invalid,1,1) == 73, "foreign/stale ID rejection");
  std::atomic<int> wrong_thread = 0;
  std::thread worker([&] { wrong_thread = pv.focus(owner,editor,1); }); worker.join();
  CheckStep(wrong_thread == 72, "GUI thread enforcement");
  QTest::keyClicks(editor_widget, "-edited");
  auto text = editor_widget->text();
  CheckStep(text.contains("edited"), "native key delivery");
  CheckStep(pv.commit(owner,nullptr,0,1) == 0 && !editor_widget->parentWidget()->isVisible() &&
      pv.focus(owner,editor,1) == 74, "hidden focus rejection");
  items[1].clip.x += 20; items[1].clip.width -= 20;
  CheckStep(pv.commit(owner,items,2,1) == 0 && editor_widget->text() == text &&
      editor_widget->x() == -20, "clip/restore retains native identity and text");
  web = new QWebEngineView;
  CheckStep(pv.adopt_widget(owner, web, focus, nullptr, &web_id) == 0, "adopt optional WebEngine widget");
  doroti_qt_pv_placement all[] = {items[0], items[1], Place(web_id, 280,20,320,190)};
  CheckStep(pv.commit(owner,all,3,1) == 0, "WebEngine disjoint B attachment");
  QObject::connect(web, &QWebEngineView::loadFinished, web, [](bool ok) {
    if (!ok) { Finish(); return; }
    web->page()->runJavaScript("document.querySelector('input').value='live-state'; document.title", [](const QVariant& title) {
      CheckStep(title.toString() == "Doroti live WebEngine", "real WebEngine document");
      pv.commit(owner,nullptr,0,1);
      auto placement = Place(web_id,280,20,320,190);
      auto restore_status = pv.commit(owner,&placement,1,1);
      std::printf("WebEngine restore status=%d parent=%p owner=%p\n", restore_status, (void*)web->parentWidget()->windowHandle()->parent(), native_view);
      Check(restore_status == 0);
      web->page()->runJavaScript("document.querySelector('input').value", [](const QVariant& value) {
        web_loaded = value.toString() == "live-state";
        QTimer::singleShot(500, web, [] {
          auto* window = static_cast<QWindow*>(native_view);
          auto capture = window->screen()->grabWindow(window->winId());
          if (!capture.isNull()) { capture.save(QString::fromStdString(screenshot)); std::puts("native-window capture saved"); }
          else std::puts("native-window capture unavailable on this QPA");
          Finish();
        });
      });
    });
  });
  web->setHtml("<title>Doroti live WebEngine</title><body style='background:#00aa55'><h2>Live WebEngine</h2><input value='initial'></body>");
}

void Check(bool condition, std::source_location location) { if (!condition) std::fprintf(stderr,"FAIL line %u\n", location.line()); valid &= condition; }
std::string Text(doroti_qt_utf8_v2 text) {
  return std::string(reinterpret_cast<const char*>(text.data), text.length);
}
}

int main(int argc, char** argv) {
  setvbuf(stdout, nullptr, _IONBF, 0);
  if (argc != 3) return 2;
  screenshot = argv[2];
  auto* library = dlopen(argv[1], RTLD_NOW | RTLD_LOCAL);
  if (!library) { std::fprintf(stderr, "%s\n", dlerror()); return 2; }
  auto run = reinterpret_cast<decltype(&doroti_qt_run_v2)>(dlsym(library, "doroti_qt_run_v2"));
  get = reinterpret_cast<Get>(dlsym(library,"doroti_qt_get_platform_views"));
  if (!run || !get) return 2;
  doroti_qt_configuration_v2 config{};
  config.abi_version = 4;
  config.struct_size = sizeof(config);
  config.required_features = 0x1fffe;
  const std::string title = "Doroti Qt ABI probe (no rendering)";
  config.title = {reinterpret_cast<const std::uint8_t*>(title.data()), title.size()};
  config.logical_width = 640; config.logical_height = 300;
  doroti_qt_callbacks_v2 cb{};
  cb.abi_version = 4; cb.struct_size = sizeof(cb);
  cb.required_features = cb.feature_bits = config.required_features;
  cb.prepare_application = [](void*) {
    CheckStep(QCoreApplication::instance() == nullptr, "pre-QApplication hook"); ++prepare_count;
    QWebEngineUrlScheme scheme("doroti-probe");
    scheme.setSyntax(QWebEngineUrlScheme::Syntax::Path);
    QWebEngineUrlScheme::registerScheme(scheme);
    return 0;
  };
  cb.view_created = [](void*, void* view, const doroti_qt_host_api_v2* host) {
    native_view = view;
    CheckStep(get(view,1,sizeof(pv),&owner,&pv) == 0 && owner != 0 && pv.feature_bits == 1, "optional attachment ABI");
    Check(get(view,2,sizeof(pv),&owner,&pv) == 65);
    Check(get(view,1,sizeof(pv)-8,&owner,&pv) == 66);
    QTimer::singleShot(250, static_cast<QWindow*>(view), Exercise);
    QTimer::singleShot(30000, static_cast<QWindow*>(view), [] { valid = false; api->request_close(native_view); });
    Check(host->struct_size == 128 && host->prepare_present != nullptr);
    api = host; return 0;
  };
  cb.render = [](void*, void*, const doroti_qt_surface_v2* surface, std::uint64_t token) {
    ++renders;
    Check(token != 0 && surface->struct_size == 144 && surface->abi_version == 4);
    Check(surface->vulkan_surface != 0 && surface->vulkan_instance != nullptr);
    Check(surface->vulkan_instance_api_version >= ((1u << 22) | (2u << 12)));
    Check(surface->pixel_width > 0 && surface->pixel_height > 0);
    extensions = Text(surface->vulkan_instance_extensions);
    Check(extensions.find("VK_KHR_surface") != std::string::npos);
    generation = surface->surface_generation; identity = surface->context_identity;
    // The first attempt deliberately produces no image. The host must retry
    // without relying on a compositor callback from a presentation.
    if (renders == 1) return 1;
    return 0; // Deliberately fake acceptance; this probe does not claim presentation.
  };
  cb.poll_gpu_work = [](void*, void* view) {
    ++polls;
    return 0;
  };
  cb.surface_destroying = [](void*, void*, std::uint64_t g, std::uint64_t i) {
    ++releases; Check(g == generation && i == identity);
  };
  cb.closed = [](void*, void*) { ++closed; };
  cb.frame_terminal = [](void*, void*, std::uint64_t, std::uint32_t, std::uint64_t, std::int64_t) {};
  cb.diagnostic = [](void*, doroti_qt_utf8_v2, doroti_qt_utf8_v2) {};
  cb.fatal = [](void*, std::int32_t, doroti_qt_utf8_v2 text) {
    valid = false; std::fprintf(stderr, "%s\n", Text(text).c_str());
  };
  cb.metrics_changed = [](void*, void*, const doroti_qt_metrics_v2*) {};
  cb.lifecycle_changed = [](void*, void*, std::uint32_t, std::int64_t) {};
  cb.close_requested = [](void*, void*) {};
  cb.pointer = [](void*, void*, const doroti_qt_pointer_v2*) {};
  cb.key = [](void*, void*, const doroti_qt_key_v2*) {};
  cb.focus = [](void*, void*, std::uint32_t, std::int64_t) {};
  cb.text_editing = [](void*, void*, const doroti_qt_text_state_v2*) {};
  cb.text_action = [](void*, void*, std::uint32_t) {};
  cb.clipboard_text = [](void*, void*, std::uint64_t, doroti_qt_utf8_v2) {};
  cb.configuration_changed = [](void*, void*, doroti_qt_utf8_v2, std::uint32_t, std::uint32_t, std::uint32_t) {};
  cb.semantics_action = [](void*, void*, std::int64_t, std::int64_t, doroti_qt_utf8_v2) {};
  auto old_version = config; old_version.abi_version = 3;
  Check(run(&old_version, &cb) == DOROTI_QT_ERROR_ABI_VERSION);
  auto missing_prepare = cb; missing_prepare.prepare_application = nullptr;
  Check(run(&config, &missing_prepare) == DOROTI_QT_ERROR_REQUIRED_CALLBACK);
  auto failed_prepare = cb; failed_prepare.prepare_application = [](void*) { return 74; };
  Check(run(&config, &failed_prepare) == 74 && QCoreApplication::instance() == nullptr);
  auto old = cb; old.struct_size = 176;
  Check(run(&config, &old) == DOROTI_QT_ERROR_ABI_SIZE);
  auto missing = cb; missing.poll_gpu_work = nullptr;
  Check(run(&config, &missing) == DOROTI_QT_ERROR_REQUIRED_CALLBACK);
  auto legacy_present = cb; legacy_present.feature_bits &= ~DOROTI_QT_FEATURE_PRESENT_HOOK;
  Check(run(&config, &legacy_present) == DOROTI_QT_ERROR_UNSUPPORTED_FEATURE);
  auto unsupported = config; unsupported.required_features |= 1ull << 63;
  Check(run(&unsupported, &cb) == DOROTI_QT_ERROR_UNSUPPORTED_FEATURE);
  const int result = run(&config, &cb);
  Check(result == 0 && renders >= 1 && polls >= 1 && releases == 1 && closed == 1);
  CheckStep(prepare_count == 1 && cancelled_tasks == accepted_tasks && accepted_tasks == 20, "preparation once / close terminates accepted queue");
  CheckStep(pv.post(owner, [](void*,int) { valid=false; }, nullptr) == 71, "late owner callback rejection");
  std::printf("Qt native contract: %s; nativeExit=%d renderCallbacks=%d idlePolls=%d releases=%d closed=%d extensions=%s\n",
              valid ? "PASS" : "FAIL", result, renders, polls, releases, closed, extensions.c_str());
  // Qt platform plugins may retain process-lifetime globals: leave the module loaded.
  return valid ? 0 : 1;
}
