// Native event/ABI probe; callbacks accept frames without rendering a scene.
#include "doroti_qt_host_v2.h"
#include <QAccessible>
#include <QCoreApplication>
#include <QKeyEvent>
#include <QMouseEvent>
#include <QTimer>
#include <QWindow>
#include <dlfcn.h>
#include <cstdio>
#include <string>

namespace {
bool valid = true, unified;
int renders, pointers, closes;
double padding;
const doroti_qt_host_api_v2* api;
void* handle;
void Check(bool condition, const char* message) {
  if (!condition) { valid = false; std::fprintf(stderr, "%s\n", message); }
}
void Mouse(QWindow* window, QEvent::Type type, QPointF point, Qt::MouseButton button, Qt::MouseButtons buttons) {
  QMouseEvent event(type, point, window->mapToGlobal(point.toPoint()), button, buttons, Qt::NoModifier);
  QCoreApplication::sendEvent(window, &event);
}
void Click(QWindow* window, QPointF point) {
  Mouse(window, QEvent::MouseButtonPress, point, Qt::LeftButton, Qt::LeftButton);
  Mouse(window, QEvent::MouseButtonRelease, point, Qt::LeftButton, Qt::NoButton);
}
}

int main(int argc, char** argv) {
  if (argc != 3) return 2;
  unified = std::string(argv[2]) == "unified";
  auto* library = dlopen(argv[1], RTLD_NOW | RTLD_LOCAL);
  if (!library) { std::fprintf(stderr, "%s\n", dlerror()); return 2; }
  auto run = reinterpret_cast<decltype(&doroti_qt_run_v2)>(dlsym(library, "doroti_qt_run_v2"));
  if (!run) return 2;
  doroti_qt_configuration_v2 config{};
  config.abi_version = 3; config.struct_size = sizeof(config);
  config.required_features = 0x43ff;
  const std::string title = "Doroti titlebar input probe";
  config.title = {reinterpret_cast<const std::uint8_t*>(title.data()), title.size()};
  config.logical_width = 480; config.logical_height = 320;
  config.backdrop_mode = DOROTI_QT_BACKDROP_ACRYLIC;
  config.titlebar_style = unified ? DOROTI_QT_TITLEBAR_UNIFIED : DOROTI_QT_TITLEBAR_SOLID;
  doroti_qt_callbacks_v2 cb{};
  cb.abi_version = 3; cb.struct_size = sizeof(cb);
  cb.required_features = cb.feature_bits = config.required_features;
  cb.view_created = [](void*, void* view, const doroti_qt_host_api_v2* host) {
    api = host; handle = view;
    auto* window = static_cast<QWindow*>(view);
    Check(window->flags().testFlag(Qt::FramelessWindowHint) == unified, "Wrong window decorations");
    QTimer::singleShot(200, window, [window] {
      Check(renders > 0, "No caption surface descriptor received");
      Check(padding == (unified ? 32 : 0) * window->devicePixelRatio(), "Wrong caption safe area");
      const auto before = pointers;
      Click(window, QPointF(40, 100));
      Check(pointers == before + 2, "Client pointer input was lost");
      if (unified) {
        auto* captionRoot = QAccessible::queryAccessibleInterface(window);
        Check(captionRoot && captionRoot->childCount() == 3, "Caption accessibility depends on app semantics");
        const auto clientPointers = pointers;
        Click(window, QPointF(window->width() - 69, 16));
        Check(window->windowState() == Qt::WindowMaximized, "Maximize caption button failed");
        Click(window, QPointF(window->width() - 69, 16));
        Check(window->windowState() == Qt::WindowNoState, "Restore caption button failed");
        Check(pointers == clientPointers, "Caption input reached the application");
        window->showFullScreen();
        Check(padding == 0, "Fullscreen retained the caption inset");
        window->showNormal();
      }
      const std::string semantics = R"({"nodes":[{"id":0,"label":"Root","children":[],"rect":[0,0,480,320]}]})";
      api->update_semantics(handle, {reinterpret_cast<const std::uint8_t*>(semantics.data()), semantics.size()});
      if (unified) api->clear_semantics(handle);
      QTimer::singleShot(100, window, [window] {
        if (unified) {
          auto* root = QAccessible::queryAccessibleInterface(window);
          Check(root != nullptr && root->childCount() == 3, "Caption accessibility buttons are missing");
          QAccessibleInterface* close = nullptr;
          if (root) for (int i = 0; i < root->childCount(); ++i)
            if (root->child(i)->text(QAccessible::Name) == "Close") close = root->child(i);
          Check(close && close->actionInterface(), "Close accessibility action is missing");
          if (close && close->actionInterface()) close->actionInterface()->doAction(QAccessibleActionInterface::pressAction());
          else window->close();
        } else {
          window->close();
        }
      });
    });
    QTimer::singleShot(5000, window, [window] { Check(false, "Caption probe timed out"); window->close(); });
    return 0;
  };
  cb.render = [](void*, void*, const doroti_qt_surface_v2* surface, std::uint64_t) {
    ++renders;
    Check(surface->struct_size == 144, "Wrong surface ABI size");
    Check(surface->titlebar_height == (unified ? 32u : 0u), "Wrong caption height in render descriptor");
    return 0;
  };
  cb.metrics_changed = [](void*, void*, const doroti_qt_metrics_v2* metrics) { padding = metrics->view_padding.top; };
  cb.pointer = [](void*, void*, const doroti_qt_pointer_v2*) { ++pointers; };
  cb.closed = [](void*, void*) { ++closes; };
  cb.surface_destroying = [](void*, void*, std::uint64_t, std::uint64_t) {};
  cb.frame_terminal = [](void*, void*, std::uint64_t, std::uint32_t, std::uint64_t, std::int64_t) {};
  cb.diagnostic = [](void*, doroti_qt_utf8_v2, doroti_qt_utf8_v2) {};
  cb.fatal = [](void*, std::int32_t, doroti_qt_utf8_v2) { Check(false, "Native fatal error"); };
  cb.lifecycle_changed = [](void*, void*, std::uint32_t, std::int64_t) {};
  cb.close_requested = [](void*, void*) {};
  cb.key = [](void*, void*, const doroti_qt_key_v2*) {};
  cb.focus = [](void*, void*, std::uint32_t, std::int64_t) {};
  cb.text_editing = [](void*, void*, const doroti_qt_text_state_v2*) {};
  cb.text_action = [](void*, void*, std::uint32_t) {};
  cb.clipboard_text = [](void*, void*, std::uint64_t, doroti_qt_utf8_v2) {};
  cb.configuration_changed = [](void*, void*, doroti_qt_utf8_v2, std::uint32_t, std::uint32_t, std::uint32_t) {};
  cb.semantics_action = [](void*, void*, std::int64_t, std::int64_t, doroti_qt_utf8_v2) { Check(false, "Caption action leaked to app semantics"); };
  const int result = run(&config, &cb);
  Check(result == 0 && closes == 1, "Window lifecycle failed");
  std::printf("Qt %s caption input, safe area, accessibility: %s\n", unified ? "unified" : "solid", valid ? "PASS" : "FAIL");
  return valid ? 0 : 1;
}
