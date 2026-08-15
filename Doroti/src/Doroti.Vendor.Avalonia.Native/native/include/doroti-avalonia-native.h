#pragma once
#include <stdint.h>

// Generated Doroti C ABI projection for the A0-pinned Avalonia.Native AppKit source set.
// The upstream generated COM headers remain provenance inputs; native Cocoa objects stay here.
#ifdef __cplusplus
extern "C" {
#endif

typedef void (*doroti_avn_event_callback)(
    void* context, int32_t kind, int32_t phase, uint64_t window_id,
    double a, double b, double c, double d,
    uint64_t u0, uint64_t u1, const char* text);

void doroti_avn_app_init(void);
void doroti_avn_app_wake(void);
int32_t doroti_avn_app_pump(int32_t wait);
void* doroti_avn_window_create(const char* title, double width, double height,
                               doroti_avn_event_callback callback, void* context);
void doroti_avn_window_show(void* host);
void doroti_avn_window_resize(void* host, double width, double height);
void doroti_avn_window_minimize(void* host, int32_t minimized);
void doroti_avn_window_focus(void* host, int32_t focused);
void doroti_avn_window_close(void* host);
void doroti_avn_window_destroy(void* host);
void doroti_avn_window_move_to_screen(void* host, uint64_t screen_id);
void doroti_avn_window_metrics(void* host, double* width, double* height,
                               double* pixel_width, double* pixel_height, double* scale);
int32_t doroti_avn_screen_primary(uint64_t* screen_id, double* x, double* y,
                                  double* width, double* height, double* scale);
void* doroti_avn_window_nswindow(void* host);
void doroti_avn_cursor_set(int32_t kind);
char* doroti_avn_clipboard_get(void);
int32_t doroti_avn_clipboard_set(const char* text);
void doroti_avn_string_free(char* value);
void doroti_avn_accessibility_set(void* host, int32_t node_id, const char* label, int32_t can_press);
void doroti_avn_text_caret(void* host, double x, double y, double width, double height);
void doroti_avn_test_pointer(void* host, int32_t phase, double x, double y, double dx, double dy);
void doroti_avn_test_key(void* host, int32_t phase, uint32_t key);
void doroti_avn_test_text(void* host, int32_t phase, const char* text);
void* doroti_avn_gl_create(void* host);
void* doroti_avn_gl_make_current(void* context);
void doroti_avn_gl_restore(void* context);
void doroti_avn_gl_present(void* context);
const char* doroti_avn_gl_renderer(void* context);
const char* doroti_avn_gl_version(void* context);
void doroti_avn_gl_destroy(void* context);

#ifdef __cplusplus
}
#endif
