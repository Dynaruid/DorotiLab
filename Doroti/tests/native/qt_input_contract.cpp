// Standalone ABI regression driver, never linked into product/native packages.
#include "doroti_qt_host_v2.h"
#include "doroti_qt_platform_views.h"
#include <QApplication>
#include <QCryptographicHash>
#include <QDir>
#include <QInputMethodEvent>
#include <QKeyEvent>
#include <QLineEdit>
#include <QPointingDevice>
#include <QStandardPaths>
#include <QWheelEvent>
#include <QWindow>
#include <cstdio>
#include <string>
#include <vector>

namespace {
int failures = 0;
bool legacy = false;
const doroti_qt_host_api_v2* api;
QWindow* window;
QString text;
int selection_base, selection_extent, composing_base, composing_extent;
std::vector<std::uint32_t> keys;
std::vector<double> scroll;
void Check(bool value, const char* message) {
  if (!value) { std::fprintf(stderr, "FAIL: %s\n", message); ++failures; }
}
doroti_qt_utf8_v2 Utf8(const char* value) {
  return {reinterpret_cast<const std::uint8_t*>(value), std::char_traits<char>::length(value)};
}
void State(const char* value, int base, int extent, int comp_base = -1, int comp_extent = -1) {
  doroti_qt_text_configuration_v2 config{7, sizeof(config), 0, 0, 0, 0, 0, 1, 1, 0};
  doroti_qt_text_state_v2 state{7, sizeof(state), Utf8(value), base, extent, comp_base, comp_extent};
  api->set_text_client(window, &config, &state);
  QCoreApplication::processEvents();
}
void Send(QInputMethodEvent& event) { QCoreApplication::sendEvent(window, &event); }
std::int32_t Created(void*, void* handle, const doroti_qt_host_api_v2* host) {
  api = host; window = static_cast<QWindow*>(handle);
  Check(QCoreApplication::applicationName() == (legacy ? "doroti" : "org.doroti.input-contract"), "application name");
  Check(QGuiApplication::desktopFileName() == (legacy ? "" : "org.doroti.input-contract"), "desktop identity");
  Check((host->feature_bits & DOROTI_QT_FEATURE_APPLICATION_ID) != 0, "identity feature");
  // Compare public IME behavior with Qt's own editable for both selection directions.
  for (auto endpoints : {std::pair{1, 3}, std::pair{3, 1}, std::pair{4, 4}}) {
    QLineEdit reference("abcd");
    reference.setSelection(endpoints.first, endpoints.second - endpoints.first);
    State("abcd", endpoints.first, endpoints.second);
    QInputMethodEvent event("", {{QInputMethodEvent::Selection, 1, 2, QVariant()}});
    QCoreApplication::sendEvent(&reference, &event); Send(event);
    Check(text == reference.text() && selection_base == 1 && selection_extent == 3,
          "selection-only event preserves text and applies selection");
  }
  for (int replacement : {-1, 0, 1}) {
    QLineEdit reference("abcd"); reference.setCursorPosition(2);
    State("abcd", 2, 2);
    QInputMethodEvent event; event.setCommitString("X", replacement, 0);
    QCoreApplication::sendEvent(&reference, &event); Send(event);
    Check(text == reference.text() && selection_extent == reference.cursorPosition(),
          "zero-length replacement respects insertion offset");
  }
  State("abcd", 1, 3);
  QInputMethodEvent commit; commit.setCommitString("한"); Send(commit);
  Check(text == QString::fromUtf8("a한d") && selection_extent == 2, "commit replaces selection");
  State("abcd", 2, 2);
  QInputMethodEvent selection_after_commit("", {{QInputMethodEvent::Selection, 0, 1, QVariant()}});
  selection_after_commit.setCommitString("X"); Send(selection_after_commit);
  Check(text == "abXcd" && selection_base == 0 && selection_extent == 1, "selection follows commit");
  State("ab", 1, 1);
  QInputMethodEvent preedit("ㅎ", {{QInputMethodEvent::Cursor, 1, 1, QVariant()}}); Send(preedit);
  Check(text == QString::fromUtf8("aㅎb") && composing_base == 1 && composing_extent == 2,
        "preedit insertion");
  QInputMethodEvent update("하", {{QInputMethodEvent::Cursor, 1, 1, QVariant()}}); Send(update);
  Check(text == QString::fromUtf8("a하b") && composing_base == 1 && composing_extent == 2,
        "preedit replaces previous composition");
  QInputMethodEvent finish; finish.setCommitString("한"); Send(finish);
  Check(text == QString::fromUtf8("a한b") && selection_extent == 2 && composing_base == -1,
        "composition commit preserves surrounding text");
  State("ab", 1, 1);
  Send(preedit);
  QInputMethodEvent cancel; Send(cancel);
  Check(text == "ab" && selection_extent == 1 && composing_base == -1, "composition cancellation");
  State("aㅎb", 2, 2, 1, 2);
  QInputMethodEvent replace_across_preedit; replace_across_preedit.setCommitString("X", -1, 2);
  Send(replace_across_preedit);
  Check(text == "X", "replacement ignores the old preedit area");
  for (auto spec : {std::pair{QEvent::KeyPress, false}, std::pair{QEvent::KeyRelease, true},
                    std::pair{QEvent::KeyPress, true}, std::pair{QEvent::KeyRelease, false}}) {
    QKeyEvent event(spec.first, Qt::Key_A, Qt::NoModifier, 38, Qt::Key_A, 0, "a", spec.second);
    QCoreApplication::sendEvent(window, &event);
  }
  Check(keys == std::vector<std::uint32_t>{0, 2, 1}, "auto-repeat has one final release");
  // The session's primary pointing device may be a touchpad; exercise the wheel
  // path explicitly rather than depending on whichever device Qt enumerated first.
  QPointingDevice mouse("contract mouse", 42, QInputDevice::DeviceType::Mouse,
      QPointingDevice::PointerType::Generic, QInputDevice::Capability::Position, 1, 5);
  for (bool inverted : {false, true}) {
    QWheelEvent event(QPointF(40, 40), QPointF(40, 40), {}, QPoint(0, inverted ? -120 : 120),
                     Qt::NoButton, Qt::NoModifier, Qt::NoScrollPhase, inverted,
                     Qt::MouseEventNotSynthesized, &mouse);
    QCoreApplication::sendEvent(window, &event);
  }
  Check(scroll.size() == 2 && scroll[0] < 0 && scroll[1] > 0, "system scroll direction");
  doroti_qt_pv_api views{};
  std::uint64_t owner = 0;
  Check(doroti_qt_get_platform_views(window, 1, sizeof(views), &owner, &views) == 0, "platform views ABI");
  if (views.feature_bits & 4) {
    std::uint64_t id = 0;
    Check(views.create(owner, 2, Utf8("doroti-webview:1\n{\"Profile\":1,\"NativeApplicationId\":\"org.doroti.input-contract\"}"),
                       nullptr, nullptr, &id) == 0, "persistent WebView creation");
    const auto hash = QCryptographicHash::hash("org.doroti.input-contract", QCryptographicHash::Sha256).toHex();
    const auto relative = "doroti/QtWebEngine/DorotiShared-" + QString::fromLatin1(hash);
    bool found = false;
    for (auto* object : qApp->findChildren<QObject*>()) {
      if (!object->inherits("QQuickWebEngineProfile") || !object->property("storageName").toString().startsWith("DorotiShared-")) continue;
      found = true;
      Check(object->property("persistentStoragePath").toString() ==
          QDir(QStandardPaths::writableLocation(QStandardPaths::GenericDataLocation)).filePath(relative), "persistent WebView data retained");
      Check(object->property("cachePath").toString() ==
          QDir(QStandardPaths::writableLocation(QStandardPaths::GenericCacheLocation)).filePath(relative), "persistent WebView cache retained");
    }
    Check(found, "persistent WebView profile exists");
    if (id) Check(views.remove(owner, id) == 0, "persistent WebView removal");
  }
  // A deliberate callback rejection avoids showing a window or submitting GPU work.
  return DOROTI_QT_ERROR_MANAGED_FATAL;
}
}

int main(int argc, char** argv) {
  legacy = argc == 2 && std::string(argv[1]) == "--legacy";
  doroti_qt_callbacks_v2 cb{};
  cb.abi_version = 7; cb.struct_size = sizeof(cb);
  cb.feature_bits = DOROTI_QT_FEATURE_QUICK_COMPOSITION | DOROTI_QT_FEATURE_CONSUMER_COMPLETION |
      DOROTI_QT_FEATURE_FRAME_PREPARATION | DOROTI_QT_FEATURE_GPU_POLL | DOROTI_QT_FEATURE_PRESENT_HOOK;
  cb.view_created = Created;
  cb.render = [](void*, void*, const doroti_qt_surface_v2*, std::uint64_t)->std::int32_t { return 0; };
  cb.frame_terminal = [](void*, void*, std::uint64_t, std::uint32_t, std::uint64_t, std::int64_t) {};
  cb.surface_destroying = [](void*, void*, std::uint64_t, std::uint64_t) {};
  cb.diagnostic = [](void*, doroti_qt_utf8_v2, doroti_qt_utf8_v2) {};
  cb.fatal = [](void*, std::int32_t, doroti_qt_utf8_v2) {};
  cb.metrics_changed = [](void*, void*, const doroti_qt_metrics_v2*) {};
  cb.lifecycle_changed = [](void*, void*, std::uint32_t, std::int64_t) {};
  cb.close_requested = [](void*, void*) {};
  cb.closed = [](void*, void*) {};
  cb.pointer = [](void*, void*, const doroti_qt_pointer_v2* p) { if (p->signal_kind == 1) scroll.push_back(p->scroll_delta_y); };
  cb.key = [](void*, void*, const doroti_qt_key_v2* k) { keys.push_back(k->type); };
  cb.focus = [](void*, void*, std::uint32_t, std::int64_t) {};
  cb.text_editing = [](void*, void*, const doroti_qt_text_state_v2* t) {
    text = QString::fromUtf8(reinterpret_cast<const char*>(t->text.data), qsizetype(t->text.length));
    selection_base = t->selection_base; selection_extent = t->selection_extent;
    composing_base = t->composing_base; composing_extent = t->composing_extent;
  };
  cb.text_action = [](void*, void*, std::uint32_t) {};
  cb.clipboard_text = [](void*, void*, std::uint64_t, doroti_qt_utf8_v2) {};
  cb.configuration_changed = [](void*, void*, doroti_qt_utf8_v2, std::uint32_t, std::uint32_t, std::uint32_t) {};
  cb.semantics_action = [](void*, void*, std::int64_t, std::int64_t, doroti_qt_utf8_v2) {};
  cb.poll_gpu_work = [](void*, void*)->std::int32_t { return 0; };
  cb.qt_consumer_submitted = [](void*, void*)->std::int32_t { return 0; };
  cb.prepare_frame = [](void*, void*, const doroti_qt_surface_v2*, std::uint64_t)->std::int32_t { return 0; };
  doroti_qt_configuration_v2 config{7, sizeof(config), DOROTI_QT_FEATURE_APPLICATION_ID,
      Utf8("Input contract"), 400, 300, 1, 0, 1, Utf8("org.doroti.input-contract")};
  auto invalid = config; invalid.struct_size = 56;
  Check(doroti_qt_run_v2(&invalid, &cb) == DOROTI_QT_ERROR_ABI_SIZE, "truncated identity rejected");
  invalid = config; invalid.application_id = Utf8("invalid/id");
  Check(doroti_qt_run_v2(&invalid, &cb) == DOROTI_QT_ERROR_INVALID_ARGUMENT, "invalid identity rejected");
  if (legacy) { config.struct_size = 56; config.required_features = 0; }
  Check(doroti_qt_run_v2(&config, &cb) == DOROTI_QT_ERROR_MANAGED_FATAL, "native event driver completed");
  if (failures) return 1;
  std::puts("PASS: Qt native IME selection/offset/composition, key repeat, scroll direction and application identity.");
}
