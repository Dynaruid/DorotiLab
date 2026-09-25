#pragma once

#include <gst/gst.h>
#include <filesystem>
#include <mutex>
#include <stdexcept>
#include <string>
#include <string_view>

namespace doroti {
// Deliberately limited to raw camera/GPU transport. In particular, libav,
// x264/x265 and automatic media playback plugins are not admitted. This list
// does not certify a distributor's build or its transitive dependencies.
inline bool AllowedGStreamerPlugin(std::string_view name) {
  return name == "coreelements" || name == "app" || name == "video4linux2"
      || name == "videoconvertscale" || name == "videotestsrc"
      || name == "opengl" || name == "vulkan" || name == "va";
}

inline void InitializeGStreamer() {
  static std::once_flag initialized;
  std::call_once(initialized, [] {
    if (gst_is_initialized())
      throw std::runtime_error("Doroti must initialize GStreamer before other users so its plugin policy can be applied");
    const char* configured = g_getenv("DOROTI_GSTREAMER_PLUGIN_DIR");
    if (!configured || !*configured)
      throw std::runtime_error("Set DOROTI_GSTREAMER_PLUGIN_DIR to a dedicated directory of reviewed GStreamer plugins");
    const auto directory = std::filesystem::canonical(configured);
    if (!std::filesystem::is_directory(directory) || directory.string().find(':') != std::string::npos)
      throw std::runtime_error("DOROTI_GSTREAMER_PLUGIN_DIR must name one directory");
    bool has_core = false, has_app = false;
    for (const auto& entry : std::filesystem::directory_iterator(directory)) {
      const auto file = entry.path().filename().string();
      // Validate before gst_init_check: even registry discovery can dlopen a
      // plugin and its dependencies. Do not scan the system's full plugin set.
      if (!entry.is_regular_file() || !file.starts_with("libgst") || !file.ends_with(".so")
          || !AllowedGStreamerPlugin(std::string_view(file).substr(6, file.size() - 9)))
        throw std::runtime_error("Unreviewed GStreamer plugin directory entry: " + file);
      has_core |= file == "libgstcoreelements.so";
      has_app |= file == "libgstapp.so";
    }
    if (!has_core || !has_app)
      throw std::runtime_error("GStreamer plugin directory requires libgstcoreelements.so and libgstapp.so");
    // This adapter owns process-wide GStreamer initialization. The directory
    // must remain immutable while running; applications must not load plugins
    // through another GStreamer user in the same process.
    g_setenv("GST_PLUGIN_PATH_1_0", "", TRUE);
    g_setenv("GST_PLUGIN_SYSTEM_PATH_1_0", directory.c_str(), TRUE);
    g_setenv("GST_PLUGIN_LOADING_WHITELIST",
        "coreelements,app,video4linux2,videoconvertscale,videotestsrc,opengl,vulkan,va", TRUE);
    g_setenv("GST_REGISTRY_UPDATE", "yes", TRUE);
    // Keep discovery from replacing the desktop's ordinary registry cache.
    const auto cache = std::filesystem::path(g_get_user_cache_dir()) / "doroti";
    std::filesystem::create_directories(cache);
    const auto registry = cache / "gstreamer-registry.bin";
    g_setenv("GST_REGISTRY_1_0", registry.c_str(), TRUE);
    GError* error = nullptr;
    if (!gst_init_check(nullptr, nullptr, &error)) {
      std::string message = error ? error->message : "gst_init failed";
      if (error) g_error_free(error);
      throw std::runtime_error(message);
    }
    GList* plugins = gst_registry_get_plugin_list(gst_registry_get());
    std::string rejected;
    for (GList* item = plugins; item; item = item->next) {
      auto* plugin = GST_PLUGIN(item->data);
      const char* name = gst_plugin_get_name(plugin);
      const char* license = gst_plugin_get_license(plugin);
      const char* filename = gst_plugin_get_filename(plugin);
      // staticelements is GStreamer's built-in bin/pipeline registration.
      const bool builtin = name && std::string_view(name) == "staticelements" && !filename;
      const bool known = name && AllowedGStreamerPlugin(name) && filename
          && std::filesystem::path(filename).parent_path().lexically_normal() == directory;
      if ((!builtin && !known) || !license || std::string_view(license) != "LGPL") {
        rejected = name ? name : "unknown";
        break;
      }
    }
    gst_plugin_list_free(plugins);
    if (!rejected.empty())
      throw std::runtime_error("GStreamer plugin rejected by license policy: " + rejected);
  });
}
}  // namespace doroti
