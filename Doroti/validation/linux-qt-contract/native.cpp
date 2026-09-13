// ABI/Qt event-loop probe only: no Graphite drawing or GPU submission.
#include "doroti_qt_host_v2.h"
#include <dlfcn.h>
#include <cstdio>
#include <stdexcept>
#include <string>

namespace {
const doroti_qt_host_api_v2* api;
int renders, polls, releases, closed;
bool valid = true;
std::string extensions;
std::uint64_t generation, identity;

void Check(bool condition) { valid &= condition; }
std::string Text(doroti_qt_utf8_v2 text) {
  return std::string(reinterpret_cast<const char*>(text.data), text.length);
}
}

int main(int argc, char** argv) {
  if (argc != 2) return 2;
  auto* library = dlopen(argv[1], RTLD_NOW | RTLD_LOCAL);
  if (!library) { std::fprintf(stderr, "%s\n", dlerror()); return 2; }
  auto run = reinterpret_cast<decltype(&doroti_qt_run_v2)>(dlsym(library, "doroti_qt_run_v2"));
  if (!run) return 2;
  doroti_qt_configuration_v2 config{};
  config.abi_version = 3;
  config.struct_size = sizeof(config);
  config.required_features = 0x7ffe;
  const std::string title = "Doroti Qt ABI probe (no rendering)";
  config.title = {reinterpret_cast<const std::uint8_t*>(title.data()), title.size()};
  config.logical_width = 320; config.logical_height = 240;
  doroti_qt_callbacks_v2 cb{};
  cb.abi_version = 3; cb.struct_size = sizeof(cb);
  cb.required_features = cb.feature_bits = config.required_features;
  cb.view_created = [](void*, void*, const doroti_qt_host_api_v2* host) {
    Check(host->struct_size == 128 && host->prepare_present != nullptr);
    api = host; return 0;
  };
  cb.render = [](void*, void*, const doroti_qt_surface_v2* surface, std::uint64_t token) {
    ++renders;
    Check(token != 0 && surface->struct_size == 144 && surface->abi_version == 3);
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
    if (polls < 3) return 1;
    api->request_close(view);
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
  auto old = cb; old.struct_size = 176;
  Check(run(&config, &old) == DOROTI_QT_ERROR_ABI_SIZE);
  auto missing = cb; missing.poll_gpu_work = nullptr;
  Check(run(&config, &missing) == DOROTI_QT_ERROR_REQUIRED_CALLBACK);
  auto legacy_present = cb; legacy_present.feature_bits &= ~DOROTI_QT_FEATURE_PRESENT_HOOK;
  Check(run(&config, &legacy_present) == DOROTI_QT_ERROR_UNSUPPORTED_FEATURE);
  auto unsupported = config; unsupported.required_features |= 1ull << 63;
  Check(run(&unsupported, &cb) == DOROTI_QT_ERROR_UNSUPPORTED_FEATURE);
  const int result = run(&config, &cb);
  Check(result == 0 && renders >= 1 && polls == 3 && releases == 1 && closed == 1);
  std::printf("Qt native contract: %s; nativeExit=%d renderCallbacks=%d idlePolls=%d releases=%d closed=%d extensions=%s\n",
              valid ? "PASS" : "FAIL", result, renders, polls, releases, closed, extensions.c_str());
  // Qt platform plugins may retain process-lifetime globals: leave the module loaded.
  return valid ? 0 : 1;
}
