#pragma once

#include <stddef.h>
#include <stdint.h>

#if defined(_WIN32)
#define DOROTI_WINDOWS_API __declspec(dllexport)
#define DOROTI_WINDOWS_CALL __cdecl
#else
#define DOROTI_WINDOWS_API
#define DOROTI_WINDOWS_CALL
#endif

#ifdef __cplusplus
extern "C" {
#endif

enum { DOROTI_WINDOWS_ABI_VERSION_V1 = 1 };

typedef enum doroti_windows_required_feature_v1 {
  DOROTI_WINDOWS_FEATURE_NONE_V1 = 0,
  DOROTI_WINDOWS_FEATURE_EXPERIMENTAL_ACRYLIC_V1 = 1ull << 0,
  DOROTI_WINDOWS_FEATURE_POST_PRESENT_DWM_FLUSH_V1 = 1ull << 1,
  DOROTI_WINDOWS_FEATURE_RETAINED_OVERSIZED_CHILD_SURFACE_V1 = 1ull << 2,
  DOROTI_WINDOWS_FEATURE_COMPOSITION_PRESENTATION_V1 = 1ull << 3,
  DOROTI_WINDOWS_FEATURE_VULKAN_ACRYLIC_V1 = 1ull << 4,
  DOROTI_WINDOWS_FEATURE_PREPARED_GEOMETRY_RECEIPT_V1 = 1ull << 5,
} doroti_windows_required_feature_v1;

typedef enum doroti_windows_status_v1 {
  DOROTI_WINDOWS_STATUS_OK_V1 = 0,
  DOROTI_WINDOWS_STATUS_INVALID_ARGUMENT_V1 = 1,
  DOROTI_WINDOWS_STATUS_ABI_MISMATCH_V1 = 2,
  DOROTI_WINDOWS_STATUS_NOT_IMPLEMENTED_V1 = 3,
  DOROTI_WINDOWS_STATUS_NATIVE_FAILURE_V1 = 4
} doroti_windows_status_v1;

typedef enum doroti_windows_frame_terminal_kind_v1 {
  DOROTI_WINDOWS_FRAME_PRESENTED_V1 = 1,
  DOROTI_WINDOWS_FRAME_SUPERSEDED_V1 = 2,
  DOROTI_WINDOWS_FRAME_FAILED_V1 = 3
} doroti_windows_frame_terminal_kind_v1;

#pragma pack(push, 8)

typedef struct doroti_windows_utf8_v1 {
  uint32_t abi_version;
  uint32_t struct_size;
  const uint8_t* data;
  uint64_t byte_length;
} doroti_windows_utf8_v1;

typedef struct doroti_windows_metrics_v1 {
  uint32_t abi_version;
  uint32_t struct_size;
  uint64_t view_id;
  uint64_t generation;
  uint32_t width_px;
  uint32_t height_px;
  double scale;
  double logical_width;
  double logical_height;
  uint64_t display_id;
  int64_t timestamp_qpc;
} doroti_windows_metrics_v1;

typedef struct doroti_windows_frame_request_v1 {
  uint32_t abi_version;
  uint32_t struct_size;
  uint64_t view_id;
  uint64_t generation;
  uint32_t width_px;
  uint32_t height_px;
  uint64_t causal_frame_id;
  int64_t timestamp_qpc;
} doroti_windows_frame_request_v1;

typedef struct doroti_windows_frame_terminal_v1 {
  uint32_t abi_version;
  uint32_t struct_size;
  uint64_t view_id;
  uint64_t generation;
  uint64_t causal_frame_id;
  uint32_t terminal_kind;
  uint32_t error_category;
  int64_t accepted_qpc;
  int64_t terminal_qpc;
  uint32_t platform_wait_timed_out;
  uint32_t reserved;
} doroti_windows_frame_terminal_v1;

typedef struct doroti_windows_pointer_v1 {
  uint32_t abi_version;
  uint32_t struct_size;
  uint64_t view_id;
  int64_t timestamp_qpc;
  uint32_t change;
  uint32_t kind;
  int64_t device;
  double physical_x;
  double physical_y;
  double physical_delta_x;
  double physical_delta_y;
  int64_t buttons;
  double scroll_delta_x;
  double scroll_delta_y;
  uint32_t signal_kind;
  uint32_t pointer_identifier;
  double pressure;
  double tilt;
  int64_t platform_data;
} doroti_windows_pointer_v1;

typedef struct doroti_windows_key_v1 {
  uint32_t abi_version;
  uint32_t struct_size;
  uint64_t view_id;
  int64_t timestamp_qpc;
  uint32_t type;
  uint32_t repeat;
  int64_t physical;
  int64_t logical;
  doroti_windows_utf8_v1 character;
} doroti_windows_key_v1;

typedef struct doroti_windows_text_configuration_v1 {
  uint32_t abi_version;
  uint32_t struct_size;
  uint32_t input_type;
  uint32_t input_action;
  uint32_t capitalization;
  uint32_t flags;
} doroti_windows_text_configuration_v1;

typedef struct doroti_windows_text_state_v1 {
  uint32_t abi_version;
  uint32_t struct_size;
  doroti_windows_utf8_v1 text;
  int32_t selection_base;
  int32_t selection_extent;
  int32_t composing_base;
  int32_t composing_extent;
} doroti_windows_text_state_v1;

typedef uint32_t(DOROTI_WINDOWS_CALL* doroti_windows_request_frame_v1)(void* host_context);
typedef uint32_t(DOROTI_WINDOWS_CALL* doroti_windows_request_resize_v1)(
    void* host_context, uint32_t width_px, uint32_t height_px);
typedef uint32_t(DOROTI_WINDOWS_CALL* doroti_windows_request_close_v1)(void* host_context);
typedef uint32_t(DOROTI_WINDOWS_CALL* doroti_windows_request_show_v1)(void* host_context);
typedef uint32_t(DOROTI_WINDOWS_CALL* doroti_windows_request_opaque_fallback_v1)(
    void* host_context);
typedef uint32_t(DOROTI_WINDOWS_CALL* doroti_windows_set_cursor_v1)(
    void* host_context, uint32_t cursor);
typedef uint32_t(DOROTI_WINDOWS_CALL* doroti_windows_set_clipboard_v1)(
    void* host_context, doroti_windows_utf8_v1 text);
typedef uint32_t(DOROTI_WINDOWS_CALL* doroti_windows_request_clipboard_v1)(
    void* host_context, uint64_t request_id);
typedef uint32_t(DOROTI_WINDOWS_CALL* doroti_windows_set_text_client_v1)(
    void* host_context, const doroti_windows_text_configuration_v1* configuration,
    const doroti_windows_text_state_v1* state);
typedef uint32_t(DOROTI_WINDOWS_CALL* doroti_windows_update_text_state_v1)(
    void* host_context, const doroti_windows_text_state_v1* state);
typedef uint32_t(DOROTI_WINDOWS_CALL* doroti_windows_set_caret_rect_v1)(
    void* host_context, double left, double top, double width, double height);
typedef uint32_t(DOROTI_WINDOWS_CALL* doroti_windows_clear_text_client_v1)(
    void* host_context);
typedef uint32_t(DOROTI_WINDOWS_CALL* doroti_windows_update_semantics_v1)(
    void* host_context, doroti_windows_utf8_v1 json);
typedef uint32_t(DOROTI_WINDOWS_CALL* doroti_windows_clear_semantics_v1)(
    void* host_context);
typedef uint32_t(DOROTI_WINDOWS_CALL* doroti_windows_set_composition_child_v1)(
    void* host_context, void* child_hwnd);

typedef enum doroti_windows_platform_brightness_v1 {
  DOROTI_WINDOWS_PLATFORM_BRIGHTNESS_DARK_V1 = 0,
  DOROTI_WINDOWS_PLATFORM_BRIGHTNESS_LIGHT_V1 = 1,
} doroti_windows_platform_brightness_v1;

typedef struct doroti_windows_host_v1 {
  uint32_t abi_version;
  uint32_t struct_size;
  void* host_context;
  void* top_level_hwnd;
  void* child_hwnd;
  void* opaque_child_hwnd;
  void* task_hwnd;
  doroti_windows_request_frame_v1 request_frame;
  doroti_windows_request_resize_v1 request_resize;
  doroti_windows_request_close_v1 request_close;
  doroti_windows_request_show_v1 request_show;
  doroti_windows_request_opaque_fallback_v1 request_opaque_fallback;
  doroti_windows_set_cursor_v1 set_cursor;
  doroti_windows_set_clipboard_v1 set_clipboard;
  doroti_windows_request_clipboard_v1 request_clipboard;
  doroti_windows_set_text_client_v1 set_text_client;
  doroti_windows_update_text_state_v1 update_text_state;
  doroti_windows_set_caret_rect_v1 set_caret_rect;
  doroti_windows_clear_text_client_v1 clear_text_client;
  doroti_windows_update_semantics_v1 update_semantics;
  doroti_windows_clear_semantics_v1 clear_semantics;
  uint32_t initial_platform_brightness;
  doroti_windows_set_composition_child_v1 set_composition_child;
} doroti_windows_host_v1;

typedef void(DOROTI_WINDOWS_CALL* doroti_windows_host_ready_callback_v1)(
    void* callback_context, const doroti_windows_host_v1* host);
typedef void(DOROTI_WINDOWS_CALL* doroti_windows_metrics_callback_v1)(
    void* callback_context, const doroti_windows_metrics_v1* metrics);
typedef uint32_t(DOROTI_WINDOWS_CALL* doroti_windows_render_callback_v1)(
    void* callback_context, const doroti_windows_frame_request_v1* request);
typedef void(DOROTI_WINDOWS_CALL* doroti_windows_frame_terminal_callback_v1)(
    void* callback_context, const doroti_windows_frame_terminal_v1* terminal);
typedef void(DOROTI_WINDOWS_CALL* doroti_windows_log_callback_v1)(
    void* callback_context, uint32_t level, doroti_windows_utf8_v1 message);
typedef void(DOROTI_WINDOWS_CALL* doroti_windows_pointer_callback_v1)(
    void* callback_context, const doroti_windows_pointer_v1* pointer);
typedef void(DOROTI_WINDOWS_CALL* doroti_windows_key_callback_v1)(
    void* callback_context, const doroti_windows_key_v1* key);
typedef void(DOROTI_WINDOWS_CALL* doroti_windows_focus_callback_v1)(
    void* callback_context, uint64_t view_id, uint32_t focused,
    int64_t timestamp_qpc);
typedef void(DOROTI_WINDOWS_CALL* doroti_windows_clipboard_callback_v1)(
    void* callback_context, uint64_t request_id, doroti_windows_utf8_v1 text);
typedef void(DOROTI_WINDOWS_CALL* doroti_windows_text_editing_callback_v1)(
    void* callback_context, const doroti_windows_text_state_v1* state);
typedef void(DOROTI_WINDOWS_CALL* doroti_windows_text_action_callback_v1)(
    void* callback_context, uint32_t action);
typedef void(DOROTI_WINDOWS_CALL* doroti_windows_semantics_action_callback_v1)(
    void* callback_context, int64_t node_id, int64_t action,
    doroti_windows_utf8_v1 arguments_json);
typedef void(DOROTI_WINDOWS_CALL* doroti_windows_lifecycle_callback_v1)(
    void* callback_context, uint64_t view_id, uint32_t state,
    int64_t timestamp_qpc);
typedef void(DOROTI_WINDOWS_CALL* doroti_windows_platform_brightness_callback_v1)(
    void* callback_context, uint64_t view_id, uint32_t brightness);
typedef void(DOROTI_WINDOWS_CALL* doroti_windows_platform_resources_shutdown_callback_v1)(
    void* callback_context);
typedef enum doroti_windows_composition_resize_phase_v1 {
  DOROTI_WINDOWS_COMPOSITION_RESIZE_POST_GEOMETRY_V1 = 0,
  DOROTI_WINDOWS_COMPOSITION_RESIZE_PRE_GEOMETRY_V1 = 1,
} doroti_windows_composition_resize_phase_v1;
typedef void(DOROTI_WINDOWS_CALL* doroti_windows_composition_resize_callback_v1)(
    void* callback_context, uint32_t width_px, uint32_t height_px, double scale,
    uint32_t sizing_edge, uint32_t resize_phase);

typedef struct doroti_windows_moving_frame_v1 {
  uint64_t resize_epoch;
  uint64_t generation;
  uint32_t sizing_edge;
  int32_t left, top, right, bottom;
  uint32_t width, height;
  double scale;
} doroti_windows_moving_frame_v1;
// action: 1 begin prepare, 2 commit after geometry, 3 cancel, 4 mismatch,
// 5 align compositor phase before geometry. Result: 0 success, 1 mismatch,
// negative failure. Prepared is an internal render result (4), never a terminal.
typedef int32_t(DOROTI_WINDOWS_CALL* doroti_windows_moving_frame_callback_v1)(
    void* callback_context, uint32_t action,
    const doroti_windows_moving_frame_v1* key);

typedef struct doroti_windows_configuration_v1 {
  uint32_t abi_version;
  uint32_t struct_size;
  uint64_t required_features;
  doroti_windows_utf8_v1 application_id;
  doroti_windows_utf8_v1 title;
  uint32_t initial_width_px;
  uint32_t initial_height_px;
  uint32_t n_cmd_show;
  uint32_t composition_background_argb;
} doroti_windows_configuration_v1;

typedef struct doroti_windows_callbacks_v1 {
  uint32_t abi_version;
  uint32_t struct_size;
  void* callback_context;
  doroti_windows_host_ready_callback_v1 host_ready;
  doroti_windows_metrics_callback_v1 metrics;
  doroti_windows_render_callback_v1 render;
  doroti_windows_frame_terminal_callback_v1 frame_terminal;
  doroti_windows_log_callback_v1 log;
  doroti_windows_pointer_callback_v1 pointer;
  doroti_windows_key_callback_v1 key;
  doroti_windows_focus_callback_v1 focus;
  doroti_windows_clipboard_callback_v1 clipboard;
  doroti_windows_text_editing_callback_v1 text_editing;
  doroti_windows_text_action_callback_v1 text_action;
  doroti_windows_semantics_action_callback_v1 semantics_action;
  doroti_windows_lifecycle_callback_v1 lifecycle;
  doroti_windows_platform_brightness_callback_v1 platform_brightness;
  doroti_windows_platform_resources_shutdown_callback_v1 platform_resources_shutdown;
  doroti_windows_composition_resize_callback_v1 composition_resize;
  doroti_windows_moving_frame_callback_v1 moving_frame;
} doroti_windows_callbacks_v1;

typedef struct doroti_windows_abi_layout_v1 {
  uint32_t abi_version;
  uint32_t struct_size;
  uint32_t pointer_packet_size;
  uint32_t packing;
  uint32_t utf8_size;
  uint32_t metrics_size;
  uint32_t frame_request_size;
  uint32_t host_size;
  uint32_t frame_terminal_size;
  uint32_t configuration_size;
  uint32_t callbacks_size;
  uint32_t metrics_generation_offset;
  uint32_t host_child_hwnd_offset;
  uint32_t terminal_kind_offset;
  uint32_t callbacks_render_offset;
  uint32_t gpu_pointer_count;
  uint32_t pointer_size;
  uint32_t key_size;
  uint32_t callbacks_pointer_offset;
  uint32_t host_set_cursor_offset;
  uint32_t text_configuration_size;
  uint32_t text_state_size;
  uint32_t host_set_text_client_offset;
  uint32_t callbacks_text_editing_offset;
  uint32_t callbacks_lifecycle_offset;
  uint32_t host_initial_platform_brightness_offset;
  uint32_t callbacks_platform_brightness_offset;
} doroti_windows_abi_layout_v1;

#pragma pack(pop)

DOROTI_WINDOWS_API uint32_t DOROTI_WINDOWS_CALL doroti_windows_get_abi_version_v1(void);
DOROTI_WINDOWS_API doroti_windows_status_v1 DOROTI_WINDOWS_CALL
doroti_windows_get_abi_layout_v1(doroti_windows_abi_layout_v1* layout);
DOROTI_WINDOWS_API doroti_windows_status_v1 DOROTI_WINDOWS_CALL
doroti_windows_run_v1(const doroti_windows_configuration_v1* configuration,
                      const doroti_windows_callbacks_v1* callbacks);

#ifdef __cplusplus
}
#endif
