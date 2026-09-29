#pragma once
#include "doroti_qt_host_v2.h"

// Qt GUI thread unless documented otherwise. Payloads are borrowed UTF-8 JSON;
// callbacks copy them before return. Accepted requests complete exactly once.
extern "C" {
using doroti_qt_service_reply = void (*)(void*, int, doroti_qt_utf8_v2);
using doroti_qt_drop_callback = int (*)(void*, doroti_qt_utf8_v2);
DOROTI_QT_EXPORT int doroti_qt_pick_files_v1(void* window, std::uint64_t request,
    doroti_qt_utf8_v2 options, doroti_qt_service_reply, void* context);
DOROTI_QT_EXPORT int doroti_qt_cancel_picker_v1(std::uint64_t request);
// Bind once per view; unbind before callback context destruction. Copy receive only.
DOROTI_QT_EXPORT int doroti_qt_drop_bind_v1(void* window, doroti_qt_drop_callback, void* context);
// Returns a CLOEXEC regular-file descriptor or negative errno. Caller owns close.
DOROTI_QT_EXPORT int doroti_qt_open_read_file_v1(const char* path);
// One process drag at a time. Callback status is the negotiated Qt action (0 canceled).
DOROTI_QT_EXPORT int doroti_qt_drag_v1(void* window, std::uint64_t request,
    doroti_qt_utf8_v2 payload, doroti_qt_service_reply, void* context);
DOROTI_QT_EXPORT int doroti_qt_cancel_drag_v1(std::uint64_t request);
}
