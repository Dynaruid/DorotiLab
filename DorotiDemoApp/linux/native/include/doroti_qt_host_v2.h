#pragma once

#include <cstddef>
#include <cstdint>

#if defined(_WIN32)
#define DOROTI_QT_EXPORT __declspec(dllexport)
#else
#define DOROTI_QT_EXPORT __attribute__((visibility("default")))
#endif

extern "C" {

enum doroti_qt_result_v2 : std::int32_t {
  DOROTI_QT_OK = 0,
  DOROTI_QT_ERROR_INVALID_ARGUMENT = 64,
  DOROTI_QT_ERROR_ABI_VERSION = 65,
  DOROTI_QT_ERROR_ABI_SIZE = 66,
  DOROTI_QT_ERROR_REQUIRED_CALLBACK = 67,
  DOROTI_QT_ERROR_UNSUPPORTED_FEATURE = 68,
  DOROTI_QT_ERROR_MANAGED_FATAL = 69,
  DOROTI_QT_ERROR_NATIVE_EXCEPTION = 70,
};

enum doroti_qt_feature_v2 : std::uint64_t {
  DOROTI_QT_FEATURE_OPENGL_FBO = 1ull << 0,
  DOROTI_QT_FEATURE_SWAP_ACK = 1ull << 1,
  DOROTI_QT_FEATURE_CONTEXT_LIFETIME = 1ull << 2,
  DOROTI_QT_FEATURE_LENGTH_PREFIXED_UTF8 = 1ull << 3,
};

enum doroti_qt_terminal_state_v2 : std::uint32_t {
  DOROTI_QT_TERMINAL_PRESENTED = 1,
  DOROTI_QT_TERMINAL_REPLAYED = 2,
  DOROTI_QT_TERMINAL_SUPERSEDED = 3,
  DOROTI_QT_TERMINAL_FAILED = 4,
};

struct doroti_qt_utf8_v2 {
  const std::uint8_t* data;
  std::uint64_t length;
};

struct doroti_qt_configuration_v2 {
  std::uint32_t abi_version;
  std::uint32_t struct_size;
  std::uint64_t required_features;
  doroti_qt_utf8_v2 title;
  std::int32_t logical_width;
  std::int32_t logical_height;
};

struct doroti_qt_surface_v2 {
  std::uint32_t abi_version;
  std::uint32_t struct_size;
  std::uint64_t surface_generation;
  std::uint64_t context_identity;
  std::uint32_t framebuffer_object;
  std::int32_t pixel_width;
  std::int32_t pixel_height;
  double device_pixel_ratio;
  std::int32_t sample_count;
  std::int32_t stencil_bits;
  std::uint32_t color_format;
  std::uint32_t gl_api;
  std::uint32_t gl_profile;
  std::int32_t gl_major;
  std::int32_t gl_minor;
  std::int64_t timestamp_microseconds;
};

struct doroti_qt_host_api_v2 {
  std::uint32_t abi_version;
  std::uint32_t struct_size;
  std::uint64_t feature_bits;
  void (*request_frame)(void* view_handle, std::uint64_t frame_token);
  void (*request_close)(void* view_handle);
  void* (*get_gl_proc_address)(void* view_handle, doroti_qt_utf8_v2 name);
};

struct doroti_qt_callbacks_v2 {
  std::uint32_t abi_version;
  std::uint32_t struct_size;
  std::uint64_t required_features;
  std::uint64_t feature_bits;
  void* callback_context;
  std::int32_t (*view_created)(void* callback_context, void* view_handle,
                               const doroti_qt_host_api_v2* host_api);
  std::int32_t (*render)(void* callback_context, void* view_handle,
                         const doroti_qt_surface_v2* surface,
                         std::uint64_t frame_token);
  void (*frame_terminal)(void* callback_context, void* view_handle,
                         std::uint64_t frame_token, std::uint32_t terminal_state,
                         std::uint64_t surface_generation,
                         std::int64_t timestamp_microseconds);
  void (*surface_destroying)(void* callback_context, void* view_handle,
                             std::uint64_t surface_generation,
                             std::uint64_t context_identity);
  void (*diagnostic)(void* callback_context, doroti_qt_utf8_v2 key,
                     doroti_qt_utf8_v2 value);
  void (*fatal)(void* callback_context, std::int32_t error_code,
                doroti_qt_utf8_v2 message);
};

DOROTI_QT_EXPORT std::int32_t doroti_qt_run_v2(
    const doroti_qt_configuration_v2* configuration,
    const doroti_qt_callbacks_v2* callbacks);

}  // extern "C"

static_assert(sizeof(doroti_qt_utf8_v2) == 16);
static_assert(offsetof(doroti_qt_surface_v2, surface_generation) == 8);
static_assert(offsetof(doroti_qt_surface_v2, framebuffer_object) == 24);
static_assert(offsetof(doroti_qt_surface_v2, device_pixel_ratio) == 40);
static_assert(offsetof(doroti_qt_surface_v2, timestamp_microseconds) == 80);
static_assert(sizeof(doroti_qt_surface_v2) == 88);
static_assert(offsetof(doroti_qt_callbacks_v2, callback_context) == 24);
