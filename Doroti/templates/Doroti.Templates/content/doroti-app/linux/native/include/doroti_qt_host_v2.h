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
  DOROTI_QT_FEATURE_METRICS_LIFECYCLE = 1ull << 4,
  DOROTI_QT_FEATURE_POINTER_INPUT = 1ull << 5,
  DOROTI_QT_FEATURE_KEY_FOCUS_INPUT = 1ull << 6,
  DOROTI_QT_FEATURE_TEXT_INPUT = 1ull << 7,
  DOROTI_QT_FEATURE_PLATFORM_SERVICES = 1ull << 8,
  DOROTI_QT_FEATURE_SEMANTICS = 1ull << 9,
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

struct doroti_qt_metrics_v2 {
  std::uint32_t abi_version;
  std::uint32_t struct_size;
  std::uint64_t surface_generation;
  std::int32_t pixel_width;
  std::int32_t pixel_height;
  double device_pixel_ratio;
  std::uint32_t lifecycle_state;
  std::uint32_t reserved;
  std::uint64_t metrics_generation;
  std::int64_t timestamp_microseconds;
};

struct doroti_qt_pointer_v2 {
  std::uint32_t abi_version;
  std::uint32_t struct_size;
  std::uint64_t device;
  std::uint64_t pointer_identifier;
  std::uint32_t change;
  std::uint32_t kind;
  std::int64_t buttons;
  double physical_x;
  double physical_y;
  double physical_delta_x;
  double physical_delta_y;
  double pressure;
  double tilt;
  std::uint32_t signal_kind;
  std::uint32_t platform_data;
  double scroll_delta_x;
  double scroll_delta_y;
  std::int64_t timestamp_microseconds;
};

struct doroti_qt_key_v2 {
  std::uint32_t abi_version;
  std::uint32_t struct_size;
  std::int64_t physical;
  std::int64_t logical;
  std::uint32_t type;
  std::uint32_t modifiers;
  doroti_qt_utf8_v2 character;
  std::int64_t timestamp_microseconds;
};

struct doroti_qt_text_configuration_v2 {
  std::uint32_t abi_version;
  std::uint32_t struct_size;
  std::uint32_t input_type;
  std::uint32_t input_action;
  std::uint32_t capitalization;
  std::uint32_t read_only;
  std::uint32_t obscure_text;
  std::uint32_t autocorrect;
  std::uint32_t enable_suggestions;
  std::uint32_t reserved;
};

struct doroti_qt_text_state_v2 {
  std::uint32_t abi_version;
  std::uint32_t struct_size;
  doroti_qt_utf8_v2 text;
  std::int32_t selection_base;
  std::int32_t selection_extent;
  std::int32_t composing_base;
  std::int32_t composing_extent;
};

struct doroti_qt_host_api_v2 {
  std::uint32_t abi_version;
  std::uint32_t struct_size;
  std::uint64_t feature_bits;
  void (*request_frame)(void* view_handle, std::uint64_t frame_token);
  void (*request_close)(void* view_handle);
  void* (*get_gl_proc_address)(void* view_handle, doroti_qt_utf8_v2 name);
  void (*resize)(void* view_handle, double logical_width, double logical_height);
  void (*set_clipboard_text)(void* view_handle, doroti_qt_utf8_v2 text);
  void (*request_clipboard_text)(void* view_handle, std::uint64_t request_id);
  void (*set_cursor)(void* view_handle, std::uint32_t cursor);
  void (*set_text_client)(void* view_handle,
                          const doroti_qt_text_configuration_v2* configuration,
                          const doroti_qt_text_state_v2* state);
  void (*update_text_state)(void* view_handle,
                            const doroti_qt_text_state_v2* state);
  void (*set_caret_rect)(void* view_handle, double left, double top,
                         double width, double height);
  void (*clear_text_client)(void* view_handle);
  void (*update_semantics)(void* view_handle, doroti_qt_utf8_v2 json);
  void (*clear_semantics)(void* view_handle);
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
  void (*metrics_changed)(void* callback_context, void* view_handle,
                          const doroti_qt_metrics_v2* metrics);
  void (*lifecycle_changed)(void* callback_context, void* view_handle,
                            std::uint32_t lifecycle_state,
                            std::int64_t timestamp_microseconds);
  void (*close_requested)(void* callback_context, void* view_handle);
  void (*closed)(void* callback_context, void* view_handle);
  void (*pointer)(void* callback_context, void* view_handle,
                  const doroti_qt_pointer_v2* pointer);
  void (*key)(void* callback_context, void* view_handle,
              const doroti_qt_key_v2* key);
  void (*focus)(void* callback_context, void* view_handle,
                std::uint32_t focused, std::int64_t timestamp_microseconds);
  void (*text_editing)(void* callback_context, void* view_handle,
                       const doroti_qt_text_state_v2* state);
  void (*text_action)(void* callback_context, void* view_handle,
                      std::uint32_t action);
  void (*clipboard_text)(void* callback_context, void* view_handle,
                         std::uint64_t request_id, doroti_qt_utf8_v2 text);
  void (*configuration_changed)(void* callback_context, void* view_handle,
                                doroti_qt_utf8_v2 ui_languages,
                                std::uint32_t brightness,
                                std::uint32_t always_use_24_hour_format);
  void (*semantics_action)(void* callback_context, void* view_handle,
                           std::int64_t node_id, std::int64_t action,
                           doroti_qt_utf8_v2 arguments_json);
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
static_assert(sizeof(doroti_qt_metrics_v2) == 56);
static_assert(sizeof(doroti_qt_pointer_v2) == 120);
static_assert(sizeof(doroti_qt_key_v2) == 56);
static_assert(sizeof(doroti_qt_text_configuration_v2) == 40);
static_assert(sizeof(doroti_qt_text_state_v2) == 40);
static_assert(offsetof(doroti_qt_callbacks_v2, callback_context) == 24);
static_assert(sizeof(doroti_qt_host_api_v2) == 120);
static_assert(sizeof(doroti_qt_callbacks_v2) == 176);
