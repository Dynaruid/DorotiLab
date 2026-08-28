#include "doroti_windows_host_v1.h"
#include "accessibility_bridge.h"

#include <windows.h>
#include <windowsx.h>
#include <imm.h>

#include <MddBootstrap.h>
#include <WindowsAppSDK-VersionInfo.h>
#include <winrt/Microsoft.UI.Interop.h>
#include <winrt/Microsoft.UI.Windowing.h>
#include <winrt/Windows.Data.Json.h>
#include <winrt/Windows.Foundation.Collections.h>
#include <winrt/base.h>

#include <algorithm>
#include <atomic>
#include <chrono>
#include <cmath>
#include <condition_variable>
#include <cstddef>
#include <cstdlib>
#include <cstring>
#include <deque>
#include <iterator>
#include <limits>
#include <memory>
#include <mutex>
#include <new>
#include <optional>
#include <stdexcept>
#include <string>
#include <thread>
#include <type_traits>
#include <unordered_set>
#include <vector>

namespace {

constexpr wchar_t kTopClass[] = L"Doroti.Product.HwndExact.Top.v1";
constexpr wchar_t kChildClass[] = L"Doroti.Product.HwndExact.Child.v1";
constexpr wchar_t kTaskClass[] = L"Doroti.Product.HwndExact.Task.v1";
constexpr UINT kRequestFrame = WM_APP + 0x401;
constexpr UINT kRequestResize = WM_APP + 0x402;
constexpr UINT kRequestClose = WM_APP + 0x403;
constexpr UINT kRequestShow = WM_APP + 0x404;
constexpr UINT kRenderCompleted = WM_APP + 0x405;
constexpr UINT kSetTextClient = WM_APP + 0x406;
constexpr UINT kUpdateTextState = WM_APP + 0x407;
constexpr UINT kSetCaretRect = WM_APP + 0x408;
constexpr UINT kClearTextClient = WM_APP + 0x409;
constexpr UINT kUpdateSemantics = WM_APP + 0x40A;
constexpr UINT kClearSemantics = WM_APP + 0x40B;
constexpr UINT_PTR kSmokeTimer = 1;
constexpr UINT_PTR kLifecycleTimer = 2;
constexpr UINT_PTR kInteractiveMoveTimer = 3;
constexpr UINT kInteractiveMoveIntervalMs = 8;

template <typename T>
bool ValidHeader(const T* value) noexcept {
  return value != nullptr &&
         value->abi_version == DOROTI_WINDOWS_ABI_VERSION_V1 &&
         value->struct_size >= sizeof(T);
}

int64_t QpcNow() noexcept {
  LARGE_INTEGER value{};
  QueryPerformanceCounter(&value);
  return value.QuadPart;
}

bool EnvironmentOne(const wchar_t* name) noexcept {
  wchar_t value[2]{};
  return GetEnvironmentVariableW(name, value, static_cast<DWORD>(std::size(value))) == 1 &&
         value[0] == L'1';
}

std::wstring Decode(const doroti_windows_utf8_v1& value) {
  if (value.data == nullptr || value.byte_length == 0) return {};
  if (value.byte_length > static_cast<uint64_t>(std::numeric_limits<int>::max()))
    throw std::bad_alloc();
  const auto length = static_cast<int>(value.byte_length);
  const auto required = MultiByteToWideChar(CP_UTF8, MB_ERR_INVALID_CHARS,
                                             reinterpret_cast<const char*>(value.data),
                                             length, nullptr, 0);
  if (required <= 0) throw std::bad_alloc();
  std::wstring decoded(static_cast<size_t>(required), L'\0');
  if (MultiByteToWideChar(CP_UTF8, MB_ERR_INVALID_CHARS,
                          reinterpret_cast<const char*>(value.data), length,
                          decoded.data(), required) != required)
    throw std::bad_alloc();
  return decoded;
}

std::string Encode(const std::wstring& value) {
  if (value.empty()) return {};
  const auto required = WideCharToMultiByte(
      CP_UTF8, WC_ERR_INVALID_CHARS, value.data(), static_cast<int>(value.size()),
      nullptr, 0, nullptr, nullptr);
  if (required <= 0) throw std::bad_alloc();
  std::string encoded(static_cast<size_t>(required), '\0');
  if (WideCharToMultiByte(CP_UTF8, WC_ERR_INVALID_CHARS, value.data(),
                          static_cast<int>(value.size()), encoded.data(), required,
                          nullptr, nullptr) != required)
    throw std::bad_alloc();
  return encoded;
}

struct ResizeCommand {
  uint32_t width;
  uint32_t height;
};

struct RenderWork {
  doroti_windows_metrics_v1 metrics;
  doroti_windows_frame_request_v1 request;
  int64_t accepted_qpc;
};

struct TextCommand {
  doroti_windows_text_configuration_v1 configuration{};
  std::wstring text;
  int32_t selection_base{};
  int32_t selection_extent{};
  int32_t composing_base{-1};
  int32_t composing_extent{-1};
};

struct CaretCommand {
  double left{};
  double top{};
  double width{};
  double height{};
};

class ProductHost;
LRESULT CALLBACK TopProcedure(HWND window, UINT message, WPARAM wparam, LPARAM lparam);
LRESULT CALLBACK ChildProcedure(HWND window, UINT message, WPARAM wparam, LPARAM lparam);
LRESULT CALLBACK TaskProcedure(HWND window, UINT message, WPARAM wparam, LPARAM lparam);

class ProductHost final {
 public:
  ProductHost(const doroti_windows_configuration_v1& configuration,
              const doroti_windows_callbacks_v1& callbacks)
      : configuration_(configuration), callbacks_(callbacks),
        platform_thread_id_(GetCurrentThreadId()) {}

  ~ProductHost() { Destroy(); }

  doroti_windows_status_v1 Run() {
    RegisterClasses();
    CreateWindows();
    ConnectAppWindow();
    doroti_windows_host_v1 host{
        DOROTI_WINDOWS_ABI_VERSION_V1,
        sizeof(doroti_windows_host_v1),
        this,
        top_,
        child_,
        task_,
        &RequestFrame,
        &RequestResize,
        &RequestClose,
        &RequestShow,
        &SetCursor,
        &SetClipboard,
        &RequestClipboard,
        &SetTextClient,
        &UpdateTextState,
        &SetCaretRect,
        &ClearTextClient,
        &UpdateSemantics,
        &ClearSemantics,
    };
    callbacks_.host_ready(callbacks_.callback_context, &host);
    StartRenderWorker();
    EmitLifecycle(1);
    PublishMetrics();
    RunInputSmoke();
    QueueRender();
    ConfigureSmokeTimer();

    MSG message{};
    while (GetMessageW(&message, nullptr, 0, 0) > 0) {
      TranslateMessage(&message);
      DispatchMessageW(&message);
    }
    StopRenderWorker();
    return fatal_ ? DOROTI_WINDOWS_STATUS_NATIVE_FAILURE_V1
                  : DOROTI_WINDOWS_STATUS_OK_V1;
  }

  LRESULT HandleTop(HWND window, UINT message, WPARAM wparam, LPARAM lparam) {
    switch (message) {
      case WM_ERASEBKGND:
        return 1;
      case WM_SIZE:
        if (wparam == SIZE_MINIMIZED) {
          minimized_ = true;
          EmitLifecycle(2);
          EmitLifecycle(3);
          EmitLifecycle(4);
          return 0;
        }
        if (minimized_) {
          minimized_ = false;
          EmitLifecycle(1);
        }
        if (child_ != nullptr) {
          const auto width = static_cast<uint32_t>(LOWORD(lparam));
          const auto height = static_cast<uint32_t>(HIWORD(lparam));
          if (width > 0 && height > 0) {
            // Match Flutter's host_window.cc contract: the one visible child
            // is always exactly the top-level physical client extent. The
            // child's WM_SIZE owns metrics and the bounded render transaction.
            if (!SetWindowPos(child_, nullptr, 0, 0, static_cast<int>(width),
                              static_cast<int>(height),
                              SWP_NOZORDER | SWP_NOACTIVATE)) {
              fatal_ = true;
              PostMessageW(top_, WM_CLOSE, 0, 0);
            }
          }
        }
        return 0;
      case WM_ACTIVATEAPP:
        if (!minimized_) EmitLifecycle(wparam != 0 ? 1u : 2u);
        return 0;
      case WM_ENTERSIZEMOVE:
        interactive_move_ = true;
        interactive_move_dirty_ = true;
        SetTimer(window, kInteractiveMoveTimer, kInteractiveMoveIntervalMs,
                 nullptr);
        return 0;
      case WM_WINDOWPOSCHANGED:
        if (interactive_move_) interactive_move_dirty_ = true;
        // WM_DPICHANGED can arrive while the shell window still straddles two
        // monitors. Wait until the committed window rectangle is wholly on a
        // different monitor before rebuilding the fixed-size EGL surface.
        if (interactive_move_ && EnteredDifferentMonitor() &&
            RepublishCurrentMetrics())
          QueueRender();
        break;
      case WM_EXITSIZEMOVE:
        interactive_move_ = false;
        interactive_move_dirty_ = false;
        KillTimer(window, kInteractiveMoveTimer);
        EnteredDifferentMonitor();
        // A fixed-size ANGLE window surface can retain damaged tiles after a
        // cross-DPI shell move even though the final client extent is
        // unchanged. Publish a fresh generation so managed presentation can
        // rebuild that surface once against stable HWND geometry.
        if (RepublishCurrentMetrics()) QueueRender();
        return 0;
      case WM_DPICHANGED: {
        const auto* suggested = reinterpret_cast<const RECT*>(lparam);
        if (suggested != nullptr)
          SetWindowPos(top_, nullptr, suggested->left, suggested->top,
                       suggested->right - suggested->left,
                       suggested->bottom - suggested->top,
                       SWP_NOZORDER | SWP_NOACTIVATE);
        PublishMetrics();
        return 0;
      }
      case WM_DISPLAYCHANGE:
        if (PublishMetrics()) QueueRender();
        return 0;
      case WM_CLOSE:
        StopRenderWorker();
        DestroyWindow(top_);
        return 0;
      case WM_DESTROY:
        EmitLifecycle(0);
        top_ = nullptr;
        PostQuitMessage(fatal_ ? 4 : 0);
        return 0;
      case WM_TIMER:
        if (wparam == kInteractiveMoveTimer) {
          // Coalesce geometry changes while one surface frame is in flight.
          // Each accepted tick publishes a same-size generation, which makes
          // managed ANGLE rebuild the EGL window surface before presenting.
          if (interactive_move_ && interactive_move_dirty_) {
            if (!WindowStraddlesMonitors()) {
              interactive_move_dirty_ = false;
            } else if (CanQueueInteractiveSurfaceRefresh()) {
              interactive_move_dirty_ = false;
              if (RepublishCurrentMetrics()) QueueRender();
            }
          }
          return 0;
        }
        if (wparam == kLifecycleTimer) {
          KillTimer(top_, kLifecycleTimer);
          if (lifecycle_smoke_phase_ == 0) {
            ++lifecycle_smoke_phase_;
            ShowWindow(top_, SW_MINIMIZE);
            SetTimer(top_, kLifecycleTimer, 120, nullptr);
          } else if (lifecycle_smoke_phase_ == 1) {
            ++lifecycle_smoke_phase_;
            ShowWindow(top_, SW_RESTORE);
            SendMessageW(top_, WM_DISPLAYCHANGE, 32, MAKELPARAM(1920, 1080));
          }
          return 0;
        }
        if (wparam == kSmokeTimer) {
          KillTimer(top_, kSmokeTimer);
          PostMessageW(top_, WM_CLOSE, 0, 0);
          return 0;
        }
        break;
      default:
        break;
    }
    return DefWindowProcW(window, message, wparam, lparam);
  }

  LRESULT HandleTask(HWND window, UINT message, WPARAM wparam, LPARAM lparam) {
    switch (message) {
      case kRequestFrame:
        QueueRender();
        return 0;
      case kRenderCompleted:
        DrainRenderCompletions();
        return 0;
      case kRequestResize: {
        auto* command = reinterpret_cast<ResizeCommand*>(lparam);
        if (command != nullptr) {
          ResizeTop(command->width, command->height);
          DrainRenderCompletions();
          delete command;
        }
        return 0;
      }
      case kRequestClose:
        PostMessageW(top_, WM_CLOSE, 0, 0);
        return 0;
      case kRequestShow:
        show_requested_ = true;
        if (first_exact_present_) ShowWindow(top_, ResolveShowCommand());
        return 0;
      case kSetTextClient: {
        std::unique_ptr<TextCommand> command(reinterpret_cast<TextCommand*>(lparam));
        if (command) ApplyTextCommand(*command, true);
        return 0;
      }
      case kUpdateTextState: {
        std::unique_ptr<TextCommand> command(reinterpret_cast<TextCommand*>(lparam));
        if (command) ApplyTextCommand(*command, false);
        return 0;
      }
      case kSetCaretRect: {
        std::unique_ptr<CaretCommand> command(reinterpret_cast<CaretCommand*>(lparam));
        if (command) ApplyCaretRect(*command);
        return 0;
      }
      case kClearTextClient:
        ClearTextClientOnPlatform();
        return 0;
      case kUpdateSemantics: {
        std::unique_ptr<std::wstring> json(reinterpret_cast<std::wstring*>(lparam));
        if (json) ApplySemantics(*json);
        return 0;
      }
      case kClearSemantics:
        accessibility_.Clear();
        return 0;
      default:
        return DefWindowProcW(window, message, wparam, lparam);
    }
  }

  LRESULT HandleChild(HWND window, UINT message, WPARAM wparam, LPARAM lparam) {
    switch (message) {
      case WM_ERASEBKGND:
        return 1;
      case WM_SIZE: {
        if (!render_worker_started_ || wparam == SIZE_MINIMIZED)
          return 0;
        if (PublishMetrics()) {
          const auto generation = current_generation_;
          const auto causal = QueueRender();
          WaitForExactResize(generation, causal);
        }
        return 0;
      }
      case WM_SETCURSOR:
        if (LOWORD(lparam) == HTCLIENT) {
          ::SetCursor(ResolveCursor());
          return TRUE;
        }
        break;
      case WM_SETFOCUS:
      case WM_KILLFOCUS:
        if (callbacks_.focus != nullptr)
          callbacks_.focus(callbacks_.callback_context, 1,
                           message == WM_SETFOCUS ? 1u : 0u, QpcNow());
        return 0;
      case WM_GETOBJECT:
        if (const auto result = accessibility_.HandleGetObject(wparam, lparam); result != 0)
          return result;
        break;
      case WM_IME_STARTCOMPOSITION:
        if (text_client_active_) {
          ime_composing_ = true;
          if (text_composing_base_ < 0) {
            text_composing_base_ = std::min(text_selection_base_, text_selection_extent_);
            text_composing_extent_ = std::max(text_selection_base_, text_selection_extent_);
          }
          ApplyImeWindowPosition();
          return 0;
        }
        break;
      case WM_IME_COMPOSITION:
        if (text_client_active_ && HandleImeComposition(lparam)) return 0;
        break;
      case WM_IME_ENDCOMPOSITION:
        ime_composing_ = false;
        text_composing_base_ = text_composing_extent_ = -1;
        EmitTextEditing();
        return 0;
      case WM_CHAR:
        if (text_client_active_ && !ime_composing_) {
          HandleCharacter(static_cast<wchar_t>(wparam));
          return 0;
        }
        break;
      case WM_MOUSEMOVE:
        if (!mouse_inside_) {
          mouse_inside_ = true;
          TRACKMOUSEEVENT tracking{sizeof(TRACKMOUSEEVENT), TME_LEAVE, window, 0};
          TrackMouseEvent(&tracking);
          EmitPointer(1, wparam, lparam);
        }
        EmitPointer((wparam & (MK_LBUTTON | MK_RBUTTON | MK_MBUTTON)) != 0 ? 5u : 3u,
                    wparam, lparam);
        return 0;
      case WM_MOUSELEAVE:
        mouse_inside_ = false;
        EmitPointer(2, 0, last_pointer_lparam_);
        return 0;
      case WM_LBUTTONDOWN:
      case WM_RBUTTONDOWN:
      case WM_MBUTTONDOWN:
        SetFocus(window);
        SetCapture(window);
        pointer_down_ = true;
        EmitPointer(4, wparam, lparam);
        return 0;
      case WM_LBUTTONUP:
      case WM_RBUTTONUP:
      case WM_MBUTTONUP:
        EmitPointer(6, wparam, lparam);
        if ((wparam & (MK_LBUTTON | MK_RBUTTON | MK_MBUTTON)) == 0) {
          pointer_down_ = false;
          ReleaseCapture();
        }
        return 0;
      case WM_CANCELMODE:
        if (GetCapture() == window) ReleaseCapture();
        return 0;
      case WM_CAPTURECHANGED:
        if (pointer_down_) {
          pointer_down_ = false;
          EmitPointer(0, 0, last_pointer_lparam_);
        }
        return 0;
      case WM_MOUSEWHEEL:
      case WM_MOUSEHWHEEL: {
        POINT point{GET_X_LPARAM(lparam), GET_Y_LPARAM(lparam)};
        ScreenToClient(window, &point);
        const auto client_lparam = MAKELPARAM(point.x, point.y);
        EmitPointer(3, GET_KEYSTATE_WPARAM(wparam), client_lparam,
                    message == WM_MOUSEHWHEEL
                        ? static_cast<double>(GET_WHEEL_DELTA_WPARAM(wparam))
                        : 0.0,
                    message == WM_MOUSEWHEEL
                        ? -static_cast<double>(GET_WHEEL_DELTA_WPARAM(wparam))
                        : 0.0);
        return 0;
      }
      case WM_KEYDOWN:
      case WM_SYSKEYDOWN:
      case WM_KEYUP:
      case WM_SYSKEYUP:
        EmitKey(message, wparam, lparam);
        if (text_client_active_ && (message == WM_KEYDOWN || message == WM_SYSKEYDOWN))
          HandleNavigationKey(wparam);
        return 0;
      default:
        return DefWindowProcW(window, message, wparam, lparam);
    }
    return DefWindowProcW(window, message, wparam, lparam);
  }

 private:
  static uint32_t DOROTI_WINDOWS_CALL RequestFrame(void* context) {
    auto* host = static_cast<ProductHost*>(context);
    return host != nullptr && PostMessageW(host->task_, kRequestFrame, 0, 0)
               ? 0u : 4u;
  }

  static uint32_t DOROTI_WINDOWS_CALL RequestResize(
      void* context, uint32_t width, uint32_t height) {
    auto* host = static_cast<ProductHost*>(context);
    if (host == nullptr || width == 0 || height == 0 ||
        width > static_cast<uint32_t>(std::numeric_limits<int>::max()) ||
        height > static_cast<uint32_t>(std::numeric_limits<int>::max()))
      return 1;
    auto* command = new (std::nothrow) ResizeCommand{width, height};
    if (command == nullptr) return 4;
    if (!PostMessageW(host->task_, kRequestResize, 0,
                      reinterpret_cast<LPARAM>(command))) {
      delete command;
      return 4;
    }
    return 0;
  }

  static uint32_t DOROTI_WINDOWS_CALL RequestClose(void* context) {
    auto* host = static_cast<ProductHost*>(context);
    return host != nullptr && PostMessageW(host->task_, kRequestClose, 0, 0)
               ? 0u : 4u;
  }

  static uint32_t DOROTI_WINDOWS_CALL RequestShow(void* context) {
    auto* host = static_cast<ProductHost*>(context);
    if (host == nullptr) return 4;
    if (GetCurrentThreadId() == host->platform_thread_id_) {
      host->show_requested_ = true;
      if (host->first_exact_present_)
        ShowWindow(host->top_, host->ResolveShowCommand());
      return 0;
    }
    return PostMessageW(host->task_, kRequestShow, 0, 0) ? 0u : 4u;
  }

  static uint32_t DOROTI_WINDOWS_CALL SetCursor(void* context, uint32_t cursor) {
    auto* host = static_cast<ProductHost*>(context);
    if (host == nullptr || cursor > 35) return 1;
    host->cursor_kind_.store(cursor);
    PostMessageW(host->child_, WM_SETCURSOR,
                 reinterpret_cast<WPARAM>(host->child_),
                 MAKELPARAM(HTCLIENT, WM_MOUSEMOVE));
    return 0;
  }

  static uint32_t DOROTI_WINDOWS_CALL SetClipboard(
      void* context, doroti_windows_utf8_v1 text) {
    auto* host = static_cast<ProductHost*>(context);
    if (host == nullptr || !ValidHeader(&text)) return 1;
    try {
      const auto wide = Decode(text);
      if (!OpenClipboard(nullptr)) return 4;
      if (!EmptyClipboard()) {
        CloseClipboard();
        return 4;
      }
      const auto bytes = (wide.size() + 1) * sizeof(wchar_t);
      auto memory = GlobalAlloc(GMEM_MOVEABLE, bytes);
      if (memory == nullptr) {
        CloseClipboard();
        return 4;
      }
      auto* destination = static_cast<wchar_t*>(GlobalLock(memory));
      if (destination == nullptr) {
        GlobalFree(memory);
        CloseClipboard();
        return 4;
      }
      memcpy(destination, wide.c_str(), bytes);
      GlobalUnlock(memory);
      if (SetClipboardData(CF_UNICODETEXT, memory) == nullptr) {
        GlobalFree(memory);
        CloseClipboard();
        return 4;
      }
      CloseClipboard();
      return 0;
    } catch (...) {
      return 4;
    }
  }

  static uint32_t DOROTI_WINDOWS_CALL RequestClipboard(
      void* context, uint64_t request_id) {
    auto* host = static_cast<ProductHost*>(context);
    if (host == nullptr || request_id == 0 || callbacks_missing(host->callbacks_.clipboard))
      return 1;
    std::string utf8;
    if (!OpenClipboard(nullptr)) return 4;
    const auto memory = GetClipboardData(CF_UNICODETEXT);
    if (memory != nullptr) {
      const auto* text = static_cast<const wchar_t*>(GlobalLock(memory));
      if (text != nullptr) {
        const auto wide_length = lstrlenW(text);
        const auto required = WideCharToMultiByte(CP_UTF8, 0, text, wide_length, nullptr, 0,
                                                   nullptr, nullptr);
        if (required > 0) {
          utf8.resize(static_cast<size_t>(required));
          WideCharToMultiByte(CP_UTF8, 0, text, wide_length, utf8.data(), required,
                              nullptr, nullptr);
        }
        GlobalUnlock(memory);
      }
    }
    CloseClipboard();
    doroti_windows_utf8_v1 value{
        DOROTI_WINDOWS_ABI_VERSION_V1, sizeof(doroti_windows_utf8_v1),
        reinterpret_cast<const uint8_t*>(utf8.data()), utf8.size()};
    host->callbacks_.clipboard(host->callbacks_.callback_context, request_id, value);
    return 0;
  }

  static uint32_t DOROTI_WINDOWS_CALL SetTextClient(
      void* context, const doroti_windows_text_configuration_v1* configuration,
      const doroti_windows_text_state_v1* state) {
    auto* host = static_cast<ProductHost*>(context);
    if (host == nullptr || !ValidHeader(configuration) || !ValidHeader(state)) return 1;
    try {
      auto command = std::make_unique<TextCommand>();
      command->configuration = *configuration;
      CopyTextState(*state, *command);
      return host->PostOwned(kSetTextClient, std::move(command));
    } catch (...) {
      return 4;
    }
  }

  static uint32_t DOROTI_WINDOWS_CALL UpdateTextState(
      void* context, const doroti_windows_text_state_v1* state) {
    auto* host = static_cast<ProductHost*>(context);
    if (host == nullptr || !ValidHeader(state)) return 1;
    try {
      auto command = std::make_unique<TextCommand>();
      CopyTextState(*state, *command);
      return host->PostOwned(kUpdateTextState, std::move(command));
    } catch (...) {
      return 4;
    }
  }

  static uint32_t DOROTI_WINDOWS_CALL SetCaretRect(
      void* context, double left, double top, double width, double height) {
    auto* host = static_cast<ProductHost*>(context);
    if (host == nullptr || !std::isfinite(left) || !std::isfinite(top) ||
        !std::isfinite(width) || !std::isfinite(height) || width < 0 || height < 0)
      return 1;
    auto command = std::make_unique<CaretCommand>();
    command->left = left;
    command->top = top;
    command->width = width;
    command->height = height;
    return host->PostOwned(kSetCaretRect, std::move(command));
  }

  static uint32_t DOROTI_WINDOWS_CALL ClearTextClient(void* context) {
    auto* host = static_cast<ProductHost*>(context);
    if (host == nullptr || host->task_ == nullptr) return 1;
    return PostMessageW(host->task_, kClearTextClient, 0, 0) ? 0u : 4u;
  }

  static uint32_t DOROTI_WINDOWS_CALL UpdateSemantics(
      void* context, doroti_windows_utf8_v1 json) {
    auto* host = static_cast<ProductHost*>(context);
    if (host == nullptr || !ValidHeader(&json)) return 1;
    try {
      auto value = std::make_unique<std::wstring>(Decode(json));
      return host->PostOwned(kUpdateSemantics, std::move(value));
    } catch (...) {
      return 4;
    }
  }

  static uint32_t DOROTI_WINDOWS_CALL ClearSemantics(void* context) {
    auto* host = static_cast<ProductHost*>(context);
    if (host == nullptr || host->task_ == nullptr) return 1;
    return PostMessageW(host->task_, kClearSemantics, 0, 0) ? 0u : 4u;
  }

  template <typename T>
  static bool callbacks_missing(T callback) noexcept { return callback == nullptr; }

  template <typename T>
  uint32_t PostOwned(UINT message, std::unique_ptr<T> command) {
    if (task_ == nullptr || command == nullptr) return 1;
    auto* raw = command.release();
    if (PostMessageW(task_, message, 0, reinterpret_cast<LPARAM>(raw))) return 0;
    delete raw;
    return 4;
  }

  static void CopyTextState(const doroti_windows_text_state_v1& state,
                            TextCommand& command) {
    command.text = Decode(state.text);
    const auto length = static_cast<int32_t>(std::min<size_t>(
        command.text.size(), static_cast<size_t>(std::numeric_limits<int32_t>::max())));
    const auto valid_offset = [length](int32_t value) {
      return value >= 0 && value <= length;
    };
    // Flutter uses (-1, -1) when the controller has no current selection.
    const auto valid_selection =
        (state.selection_base == -1 && state.selection_extent == -1) ||
        (valid_offset(state.selection_base) && valid_offset(state.selection_extent));
    if (!valid_selection ||
        !((state.composing_base == -1 && state.composing_extent == -1) ||
          (valid_offset(state.composing_base) && valid_offset(state.composing_extent))))
      throw std::invalid_argument("invalid text range");
    command.selection_base = state.selection_base;
    command.selection_extent = state.selection_extent;
    command.composing_base = state.composing_base;
    command.composing_extent = state.composing_extent;
  }

  void RegisterClasses() {
    const auto instance = GetModuleHandleW(nullptr);
    const struct Entry {
      const wchar_t* name;
      WNDPROC procedure;
    } entries[]{{kTopClass, TopProcedure}, {kChildClass, ChildProcedure},
                {kTaskClass, TaskProcedure}};
    for (const auto& entry : entries) {
      WNDCLASSEXW value{};
      value.cbSize = sizeof(value);
      value.lpfnWndProc = entry.procedure;
      value.hInstance = instance;
      value.hCursor = LoadCursorW(nullptr, IDC_ARROW);
      value.lpszClassName = entry.name;
      if (!RegisterClassExW(&value) &&
          GetLastError() != ERROR_CLASS_ALREADY_EXISTS)
        throw std::bad_alloc();
    }
  }

  void CreateWindows() {
    const auto instance = GetModuleHandleW(nullptr);
    const auto dpi = GetDpiForSystem();
    RECT bounds{0, 0,
                static_cast<LONG>(std::max(1u, configuration_.initial_width_px)),
                static_cast<LONG>(std::max(1u, configuration_.initial_height_px))};
    if (!AdjustWindowRectExForDpi(&bounds, WS_OVERLAPPEDWINDOW, FALSE, 0, dpi))
      throw std::bad_alloc();
    auto title = Decode(configuration_.title);
    if (title.empty()) title = L"Doroti";
    top_ = CreateWindowExW(0, kTopClass, title.c_str(), WS_OVERLAPPEDWINDOW,
                           CW_USEDEFAULT, CW_USEDEFAULT, bounds.right - bounds.left,
                           bounds.bottom - bounds.top, nullptr, nullptr, instance, this);
    if (top_ == nullptr) throw std::bad_alloc();
    stable_monitor_ = MonitorFromWindow(top_, MONITOR_DEFAULTTONEAREST);
    child_ = CreateWindowExW(0, kChildClass, L"", WS_CHILD | WS_VISIBLE,
                             0, 0, 1, 1, top_, nullptr, instance, this);
    task_ = CreateWindowExW(0, kTaskClass, L"", 0, 0, 0, 0, 0, HWND_MESSAGE,
                            nullptr, instance, this);
    if (child_ == nullptr || task_ == nullptr) throw std::bad_alloc();
    RECT client{};
    if (!GetClientRect(top_, &client) ||
        !SetWindowPos(child_, nullptr, 0, 0, client.right - client.left,
                      client.bottom - client.top,
                      SWP_NOZORDER | SWP_NOACTIVATE))
      throw std::bad_alloc();
    accessibility_.Attach(child_, [this](int64_t node_id, int64_t action,
                                         const std::wstring& arguments) {
      if (callbacks_.semantics_action == nullptr) return;
      const auto utf8 = Encode(arguments.empty() ? L"null" : arguments);
      doroti_windows_utf8_v1 value{
          DOROTI_WINDOWS_ABI_VERSION_V1, sizeof(doroti_windows_utf8_v1),
          reinterpret_cast<const uint8_t*>(utf8.data()), utf8.size()};
      callbacks_.semantics_action(callbacks_.callback_context, node_id, action, value);
    });
  }

  void ConnectAppWindow() {
    const auto id = winrt::Microsoft::UI::GetWindowIdFromWindow(top_);
    app_window_ = winrt::Microsoft::UI::Windowing::AppWindow::GetFromWindowId(id);
    if (!app_window_) throw std::bad_alloc();
  }

  void ApplyTextCommand(const TextCommand& command, bool set_client) {
    if (set_client) {
      text_configuration_ = command.configuration;
      text_client_active_ = true;
      ImmAssociateContextEx(child_, nullptr, IACE_DEFAULT);
    } else if (!text_client_active_) {
      return;
    }
    text_ = command.text;
    text_selection_base_ = command.selection_base;
    text_selection_extent_ = command.selection_extent;
    text_composing_base_ = command.composing_base;
    text_composing_extent_ = command.composing_extent;
    ime_composing_ = text_composing_base_ >= 0;
    ApplyImeWindowPosition();
    if (set_client && !text_smoke_emitted_ &&
        EnvironmentOne(L"DOROTI_WINDOWS_APPSDK_C7_SMOKE")) {
      text_smoke_emitted_ = true;
      text_ = L"한";
      text_selection_base_ = text_selection_extent_ = 1;
      text_composing_base_ = 0;
      text_composing_extent_ = 1;
      EmitTextEditing();
      text_ = L"한글";
      text_selection_base_ = text_selection_extent_ = 2;
      text_composing_base_ = text_composing_extent_ = -1;
      EmitTextEditing();
      if (callbacks_.text_action != nullptr)
        callbacks_.text_action(callbacks_.callback_context,
                               text_configuration_.input_action);
    }
  }

  void ApplyCaretRect(const CaretCommand& command) {
    caret_ = command;
    ApplyImeWindowPosition();
  }

  void ClearTextClientOnPlatform() {
    text_client_active_ = false;
    ime_composing_ = false;
    text_.clear();
    text_selection_base_ = text_selection_extent_ = 0;
    text_composing_base_ = text_composing_extent_ = -1;
    if (child_ != nullptr) ImmAssociateContextEx(child_, nullptr, 0);
  }

  void ApplyImeWindowPosition() {
    if (!text_client_active_ || child_ == nullptr) return;
    const auto context = ImmGetContext(child_);
    if (context == nullptr) return;
    const auto scale = current_scale_ > 0 ? current_scale_ : 1.0;
    const POINT point{static_cast<LONG>(std::lround(caret_.left * scale)),
                      static_cast<LONG>(std::lround((caret_.top + caret_.height) * scale))};
    COMPOSITIONFORM composition{};
    composition.dwStyle = CFS_POINT;
    composition.ptCurrentPos = point;
    ImmSetCompositionWindow(context, &composition);
    CANDIDATEFORM candidate{};
    candidate.dwIndex = 0;
    candidate.dwStyle = CFS_CANDIDATEPOS;
    candidate.ptCurrentPos = point;
    ImmSetCandidateWindow(context, &candidate);
    ImmReleaseContext(child_, context);
  }

  static std::wstring ReadCompositionString(HIMC context, DWORD index) {
    const auto byte_count = ImmGetCompositionStringW(context, index, nullptr, 0);
    if (byte_count <= 0) return {};
    std::wstring value(static_cast<size_t>(byte_count) / sizeof(wchar_t), L'\0');
    if (ImmGetCompositionStringW(context, index, value.data(),
                                 static_cast<DWORD>(byte_count)) != byte_count)
      return {};
    return value;
  }

  bool HandleImeComposition(LPARAM flags) {
    const auto context = ImmGetContext(child_);
    if (context == nullptr) return false;
    const auto release = [this, context] { ImmReleaseContext(child_, context); };
    bool handled = false;
    if ((flags & GCS_RESULTSTR) != 0) {
      const auto result = ReadCompositionString(context, GCS_RESULTSTR);
      ReplaceActiveRange(result, false);
      ime_composing_ = false;
      handled = true;
    }
    if ((flags & GCS_COMPSTR) != 0) {
      const auto composition = ReadCompositionString(context, GCS_COMPSTR);
      ReplaceActiveRange(composition, true);
      ime_composing_ = true;
      handled = true;
    }
    release();
    if (handled) EmitTextEditing();
    ApplyImeWindowPosition();
    return handled;
  }

  void ReplaceActiveRange(const std::wstring& replacement, bool composing) {
    auto start = text_composing_base_ >= 0
                     ? std::min(text_composing_base_, text_composing_extent_)
                     : std::min(text_selection_base_, text_selection_extent_);
    auto end = text_composing_base_ >= 0
                   ? std::max(text_composing_base_, text_composing_extent_)
                   : std::max(text_selection_base_, text_selection_extent_);
    const auto length = static_cast<int32_t>(text_.size());
    start = std::clamp(start, 0, length);
    end = std::clamp(end, start, length);
    text_.replace(static_cast<size_t>(start), static_cast<size_t>(end - start), replacement);
    const auto next = start + static_cast<int32_t>(replacement.size());
    text_selection_base_ = text_selection_extent_ = next;
    if (composing) {
      text_composing_base_ = start;
      text_composing_extent_ = next;
    } else {
      text_composing_base_ = text_composing_extent_ = -1;
    }
  }

  void HandleCharacter(wchar_t character) {
    if (character == L'\r' || character == L'\n') {
      const auto action = text_configuration_.input_action;
      if (text_configuration_.input_type != 1 || action != 12) {
        if (callbacks_.text_action != nullptr)
          callbacks_.text_action(callbacks_.callback_context, action);
        return;
      }
      character = L'\n';
    }
    if (character == L'\b') {
      if (text_selection_base_ == text_selection_extent_ && text_selection_base_ > 0) {
        auto start = text_selection_base_ - 1;
        if (start > 0 && text_[static_cast<size_t>(start)] >= 0xDC00 &&
            text_[static_cast<size_t>(start)] <= 0xDFFF &&
            text_[static_cast<size_t>(start - 1)] >= 0xD800 &&
            text_[static_cast<size_t>(start - 1)] <= 0xDBFF)
          --start;
        text_selection_base_ = start;
      }
      ReplaceActiveRange(L"", false);
    } else if (character >= L' ') {
      ReplaceActiveRange(std::wstring(1, character), false);
    } else {
      return;
    }
    EmitTextEditing();
  }

  void HandleNavigationKey(WPARAM key) {
    const auto length = static_cast<int32_t>(text_.size());
    if (key == VK_DELETE) {
      if (text_selection_base_ == text_selection_extent_ && text_selection_extent_ < length)
        ++text_selection_extent_;
      ReplaceActiveRange(L"", false);
      EmitTextEditing();
    } else if (key == VK_LEFT || key == VK_RIGHT) {
      const auto offset = key == VK_LEFT ? -1 : 1;
      const auto next = std::clamp(text_selection_extent_ + offset, 0, length);
      text_selection_base_ = text_selection_extent_ = next;
      text_composing_base_ = text_composing_extent_ = -1;
      EmitTextEditing();
    }
  }

  void EmitTextEditing() {
    if (callbacks_.text_editing == nullptr) return;
    const auto utf8 = Encode(text_);
    doroti_windows_text_state_v1 state{
        DOROTI_WINDOWS_ABI_VERSION_V1,
        sizeof(doroti_windows_text_state_v1),
        {DOROTI_WINDOWS_ABI_VERSION_V1, sizeof(doroti_windows_utf8_v1),
         reinterpret_cast<const uint8_t*>(utf8.data()), utf8.size()},
        text_selection_base_, text_selection_extent_, text_composing_base_,
        text_composing_extent_};
    callbacks_.text_editing(callbacks_.callback_context, &state);
  }

  static bool JsonBool(const winrt::Windows::Data::Json::JsonObject& value,
                       const wchar_t* name, bool fallback = false) {
    if (!value.HasKey(name)) return fallback;
    const auto item = value.GetNamedValue(name);
    return item.ValueType() == winrt::Windows::Data::Json::JsonValueType::Boolean
               ? item.GetBoolean()
               : fallback;
  }

  void ApplySemantics(const std::wstring& json) {
    try {
      const auto root = winrt::Windows::Data::Json::JsonObject::Parse(json);
      const auto generation = static_cast<uint64_t>(root.GetNamedNumber(L"generation"));
      const auto values = root.GetNamedArray(L"nodes");
      std::vector<doroti::windows::AccessibilityNode> nodes;
      nodes.reserve(values.Size());
      for (const auto& value : values) {
        const auto source = value.GetObject();
        doroti::windows::AccessibilityNode node;
        node.id = static_cast<int>(source.GetNamedNumber(L"id"));
        node.label = source.GetNamedString(L"label", L"");
        node.value = source.GetNamedString(L"value", L"");
        node.role = source.GetNamedString(L"role", L"none");
        node.actions = static_cast<int64_t>(source.GetNamedNumber(L"actions", 0));
        const auto rect = source.GetNamedArray(L"rect");
        if (rect.Size() != 4) throw std::invalid_argument("semantics rect");
        node.left = rect.GetNumberAt(0);
        node.top = rect.GetNumberAt(1);
        node.right = rect.GetNumberAt(2);
        node.bottom = rect.GetNumberAt(3);
        const auto children = source.GetNamedArray(L"children");
        for (uint32_t index = 0; index < children.Size(); ++index)
          node.children.push_back(static_cast<int>(children.GetNumberAt(index)));
        if (source.HasKey(L"flags") &&
            source.GetNamedValue(L"flags").ValueType() ==
                winrt::Windows::Data::Json::JsonValueType::Object) {
          const auto flags = source.GetNamedObject(L"flags");
          node.enabled = JsonBool(flags, L"enabled", true);
          node.focused = JsonBool(flags, L"focused");
          node.hidden = JsonBool(flags, L"hidden");
          node.button = JsonBool(flags, L"button");
          node.text_field = JsonBool(flags, L"textField");
          node.read_only = JsonBool(flags, L"readOnly");
          node.slider = JsonBool(flags, L"slider");
        }
        nodes.push_back(std::move(node));
      }
      accessibility_.Update(generation, std::move(nodes), current_scale_);
      if (!semantics_smoke_emitted_ &&
          EnvironmentOne(L"DOROTI_WINDOWS_APPSDK_C7_SMOKE")) {
        semantics_smoke_emitted_ = true;
        if (!accessibility_.ValidateAndInvokeForTest())
          throw std::runtime_error("UIA provider smoke failed");
      }
    } catch (...) {
      fatal_ = true;
      PostMessageW(top_, WM_CLOSE, 0, 0);
    }
  }

  void EmitLifecycle(uint32_t state) {
    if (state == lifecycle_state_ || callbacks_.lifecycle == nullptr) return;
    lifecycle_state_ = state;
    callbacks_.lifecycle(callbacks_.callback_context, 1, state, QpcNow());
  }

  bool PublishMetrics() {
    if (child_ == nullptr || callbacks_.metrics == nullptr) return false;
    RECT client{};
    if (!GetClientRect(child_, &client)) return false;
    const auto width = static_cast<uint32_t>(std::max(0L, client.right - client.left));
    const auto height = static_cast<uint32_t>(std::max(0L, client.bottom - client.top));
    if (width == 0 || height == 0) return false;
    const auto scale = static_cast<double>(GetDpiForWindow(top_)) / 96.0;
    return UpdateMetrics(width, height, scale);
  }

  bool UpdateMetrics(uint32_t width, uint32_t height, double scale) {
    if (current_generation_ != 0 && current_width_ == width &&
        current_height_ == height && current_scale_ == scale)
      return false;
    current_width_ = width;
    current_height_ = height;
    current_scale_ = scale;
    accessibility_.SetScale(scale);
    ApplyImeWindowPosition();
    current_generation_++;
    current_metrics_qpc_ = QpcNow();
    return true;
  }

  bool RepublishCurrentMetrics() {
    if (current_generation_ == 0 || current_width_ == 0 ||
        current_height_ == 0 || current_scale_ <= 0)
      return false;
    current_generation_++;
    current_metrics_qpc_ = QpcNow();
    return true;
  }

  bool EnteredDifferentMonitor() {
    if (top_ == nullptr) return false;
    RECT window_rect{};
    if (!GetWindowRect(top_, &window_rect)) return false;
    const auto monitor = MonitorFromRect(&window_rect, MONITOR_DEFAULTTONEAREST);
    if (monitor == nullptr || monitor == stable_monitor_) return false;
    MONITORINFO info{};
    info.cbSize = sizeof(info);
    if (!GetMonitorInfoW(monitor, &info)) return false;
    const auto& bounds = info.rcMonitor;
    if (window_rect.left < bounds.left || window_rect.top < bounds.top ||
        window_rect.right > bounds.right || window_rect.bottom > bounds.bottom)
      return false;
    stable_monitor_ = monitor;
    return true;
  }

  static BOOL CALLBACK CountIntersectingMonitors(HMONITOR, HDC, LPRECT,
                                                   LPARAM context) {
    auto* count = reinterpret_cast<uint32_t*>(context);
    ++(*count);
    return *count < 2;
  }

  bool WindowStraddlesMonitors() {
    if (top_ == nullptr) return false;
    RECT window_rect{};
    if (!GetWindowRect(top_, &window_rect)) return false;
    uint32_t count = 0;
    EnumDisplayMonitors(nullptr, &window_rect, &CountIntersectingMonitors,
                        reinterpret_cast<LPARAM>(&count));
    return count >= 2;
  }

  bool CanQueueInteractiveSurfaceRefresh() {
    std::lock_guard lock(render_mutex_);
    return !render_pending_.has_value() &&
           last_render_terminal_generation_ >= current_generation_;
  }

  uint64_t QueueRender() {
    if (current_generation_ == 0 || current_width_ == 0 || current_height_ == 0)
      return 0;
    const auto causal = ++causal_frame_id_;
    const auto accepted = QpcNow();
    RenderWork work{{
                         DOROTI_WINDOWS_ABI_VERSION_V1,
                         sizeof(doroti_windows_metrics_v1),
                         1,
                         current_generation_,
                         current_width_,
                         current_height_,
                         current_scale_,
                         static_cast<double>(current_width_) / current_scale_,
                         static_cast<double>(current_height_) / current_scale_,
                         1,
                         current_metrics_qpc_,
                     },
                    {
                         DOROTI_WINDOWS_ABI_VERSION_V1,
                         sizeof(doroti_windows_frame_request_v1),
                         1,
                         current_generation_,
                         current_width_,
                         current_height_,
                         causal,
                         accepted,
                     },
                    accepted};
    {
      std::lock_guard lock(render_mutex_);
      if (render_stopping_) return 0;
      render_pending_ = work;
    }
    render_condition_.notify_one();
    return causal;
  }

  static doroti_windows_frame_terminal_v1 MakeTerminal(
      const RenderWork& work, uint32_t terminal, uint32_t error_category) {
    return {
        DOROTI_WINDOWS_ABI_VERSION_V1,
        sizeof(doroti_windows_frame_terminal_v1),
        1,
        work.request.generation,
        work.request.causal_frame_id,
        terminal,
        error_category,
        work.accepted_qpc,
        QpcNow(),
        0,
        0,
    };
  }

  void StartRenderWorker() {
    render_worker_started_ = true;
    render_thread_ = std::thread([this] { RenderWorkerMain(); });
  }

  void RenderWorkerMain() {
    while (true) {
      RenderWork work{};
      {
        std::unique_lock lock(render_mutex_);
        render_condition_.wait(lock, [this] {
          return render_stopping_ || render_pending_.has_value();
        });
        if (render_stopping_ && !render_pending_.has_value()) break;
        work = *render_pending_;
        render_pending_.reset();
      }

      if (work.metrics.generation != delivered_metrics_generation_) {
        callbacks_.metrics(callbacks_.callback_context, &work.metrics);
        delivered_metrics_generation_ = work.metrics.generation;
      }
      auto terminal = callbacks_.render(callbacks_.callback_context, &work.request);
      if (terminal != DOROTI_WINDOWS_FRAME_PRESENTED_V1 &&
          terminal != DOROTI_WINDOWS_FRAME_SUPERSEDED_V1 &&
          terminal != DOROTI_WINDOWS_FRAME_FAILED_V1)
        terminal = DOROTI_WINDOWS_FRAME_FAILED_V1;
      const auto error =
          terminal == DOROTI_WINDOWS_FRAME_FAILED_V1 ? 1u : 0u;
      {
        std::lock_guard lock(render_mutex_);
        auto receipt = MakeTerminal(work, terminal, error);
        if (resize_wait_timeouts_.erase(work.request.generation) != 0)
          receipt.platform_wait_timed_out = 1;
        render_completions_.push_back(receipt);
        last_render_terminal_generation_ = work.request.generation;
        last_render_terminal_causal_frame_id_ = work.request.causal_frame_id;
        last_render_terminal_kind_ = terminal;
      }
      resize_condition_.notify_all();
      PostMessageW(task_, kRenderCompleted, 0, 0);
    }
  }

  bool WaitForExactResize(uint64_t generation, uint64_t causal_frame_id) {
    constexpr auto timeout = std::chrono::milliseconds(100);
    const auto deadline = std::chrono::steady_clock::now() + timeout;
    while (causal_frame_id != 0) {
      std::unique_lock lock(render_mutex_);
      const auto completed = resize_condition_.wait_until(
          lock, deadline, [this, causal_frame_id] {
            return render_stopping_ ||
                   last_render_terminal_causal_frame_id_ >= causal_frame_id;
          });
      if (!completed) {
        resize_wait_timeouts_.insert(generation);
        return false;
      }
      if (render_stopping_) return false;
      if (last_render_terminal_causal_frame_id_ == causal_frame_id &&
          last_render_terminal_generation_ == generation) {
        if (last_render_terminal_kind_ == DOROTI_WINDOWS_FRAME_PRESENTED_V1)
          return true;
        if (last_render_terminal_kind_ == DOROTI_WINDOWS_FRAME_FAILED_V1)
          return false;
      }
      lock.unlock();
      if (std::chrono::steady_clock::now() >= deadline) {
        std::lock_guard timeout_lock(render_mutex_);
        resize_wait_timeouts_.insert(generation);
        return false;
      }
      causal_frame_id = QueueRender();
    }
    return false;
  }

  void DrainRenderCompletions() {
    std::deque<doroti_windows_frame_terminal_v1> completions;
    {
      std::lock_guard lock(render_mutex_);
      completions.swap(render_completions_);
    }
    for (const auto& receipt : completions) {
      if (receipt.terminal_kind == DOROTI_WINDOWS_FRAME_PRESENTED_V1 &&
          !first_exact_present_) {
        first_exact_present_ = true;
        if (show_requested_) ShowWindow(top_, ResolveShowCommand());
        if (EnvironmentOne(L"DOROTI_WINDOWS_APPSDK_C8_SMOKE"))
          SetTimer(top_, kLifecycleTimer, 120, nullptr);
      }
      callbacks_.frame_terminal(callbacks_.callback_context, &receipt);
    }
  }

  void StopRenderWorker() noexcept {
    {
      std::lock_guard lock(render_mutex_);
      if (!render_thread_.joinable()) return;
      render_stopping_ = true;
      render_pending_.reset();
    }
    render_condition_.notify_one();
    resize_condition_.notify_all();
    render_thread_.join();
    render_worker_started_ = false;
    DrainRenderCompletions();
  }

  void ResizeTop(uint32_t width, uint32_t height) {
    RECT bounds{0, 0, static_cast<LONG>(width), static_cast<LONG>(height)};
    if (!AdjustWindowRectExForDpi(&bounds, WS_OVERLAPPEDWINDOW, FALSE, 0,
                                  GetDpiForWindow(top_)) ||
        !SetWindowPos(top_, nullptr, 0, 0, bounds.right - bounds.left,
                      bounds.bottom - bounds.top,
                      SWP_NOMOVE | SWP_NOZORDER | SWP_NOACTIVATE)) {
      fatal_ = true;
      PostMessageW(top_, WM_CLOSE, 0, 0);
    }
  }

  int ResolveShowCommand() const noexcept {
    const auto command = static_cast<int>(configuration_.n_cmd_show);
    return command == 0 ? SW_SHOWNORMAL : command;
  }

  void ConfigureSmokeTimer() {
    wchar_t value[32]{};
    const auto length = GetEnvironmentVariableW(
        L"DOROTI_WINDOWS_APPSDK_SMOKE_MS", value,
        static_cast<DWORD>(std::size(value)));
    if (length == 0 || length >= std::size(value)) return;
    wchar_t* end{};
    const auto milliseconds = wcstoul(value, &end, 10);
    if (end == value || *end != L'\0' || milliseconds < 250 ||
        milliseconds > 60000)
      return;
    SetTimer(top_, kSmokeTimer, static_cast<UINT>(milliseconds), nullptr);
  }

  void RunInputSmoke() {
    wchar_t value[8]{};
    if (GetEnvironmentVariableW(L"DOROTI_WINDOWS_APPSDK_INPUT_SMOKE", value,
                                static_cast<DWORD>(std::size(value))) == 0 ||
        value[0] != L'1')
      return;
    SendMessageW(child_, WM_SETFOCUS, 0, 0);
    SendMessageW(child_, WM_MOUSEMOVE, 0, MAKELPARAM(10, 20));
    SendMessageW(child_, WM_LBUTTONDOWN, MK_LBUTTON, MAKELPARAM(10, 20));
    SendMessageW(child_, WM_MOUSEMOVE, MK_LBUTTON, MAKELPARAM(18, 25));
    SendMessageW(child_, WM_LBUTTONUP, 0, MAKELPARAM(18, 25));
    RECT client{};
    if (GetClientRect(child_, &client)) {
      POINT wheel_point{(client.right - client.left) / 2,
                        (client.bottom - client.top) / 2};
      if (ClientToScreen(child_, &wheel_point)) {
        SendMessageW(child_, WM_MOUSEWHEEL,
                     MAKEWPARAM(0, static_cast<WORD>(-WHEEL_DELTA)),
                     MAKELPARAM(wheel_point.x, wheel_point.y));
      }
    }
    SendMessageW(child_, WM_KEYDOWN, 'A', 1 | (0x1Eu << 16));
    SendMessageW(child_, WM_KEYUP, 'A', 1 | (0x1Eu << 16) | (1u << 30) | (1u << 31));
    SendMessageW(child_, WM_KILLFOCUS, 0, 0);
  }

  void EmitPointer(uint32_t change, WPARAM wparam, LPARAM lparam,
                   double scroll_x = 0, double scroll_y = 0) {
    if (callbacks_.pointer == nullptr) return;
    const auto x = static_cast<double>(GET_X_LPARAM(lparam));
    const auto y = static_cast<double>(GET_Y_LPARAM(lparam));
    const auto previous_x = static_cast<double>(GET_X_LPARAM(last_pointer_lparam_));
    const auto previous_y = static_cast<double>(GET_Y_LPARAM(last_pointer_lparam_));
    int64_t buttons = 0;
    if ((wparam & MK_LBUTTON) != 0) buttons |= 1;
    if ((wparam & MK_RBUTTON) != 0) buttons |= 2;
    if ((wparam & MK_MBUTTON) != 0) buttons |= 4;
    doroti_windows_pointer_v1 pointer{
        DOROTI_WINDOWS_ABI_VERSION_V1, sizeof(doroti_windows_pointer_v1),
        1, QpcNow(), change, 1, 1, x, y,
        pointer_sequence_ == 0 ? 0.0 : x - previous_x,
        pointer_sequence_ == 0 ? 0.0 : y - previous_y,
        buttons, scroll_x, scroll_y,
        (scroll_x != 0 || scroll_y != 0) ? 1u : 0u,
        1, 1.0, 0.0, 0};
    last_pointer_lparam_ = lparam;
    ++pointer_sequence_;
    callbacks_.pointer(callbacks_.callback_context, &pointer);
  }

  void EmitKey(UINT message, WPARAM wparam, LPARAM lparam) {
    if (callbacks_.key == nullptr) return;
    std::wstring character;
    if (message == WM_KEYDOWN || message == WM_SYSKEYDOWN) {
      BYTE keyboard_state[256]{};
      if (GetKeyboardState(keyboard_state)) {
        wchar_t buffer[8]{};
        const auto scan = static_cast<UINT>((lparam >> 16) & 0xFF);
        const auto count = ToUnicodeEx(static_cast<UINT>(wparam), scan,
                                       keyboard_state, buffer,
                                       static_cast<int>(std::size(buffer)), 0,
                                       GetKeyboardLayout(0));
        if (count > 0) character.assign(buffer, buffer + count);
      }
    }
    std::string utf8;
    if (!character.empty()) {
      const auto required = WideCharToMultiByte(CP_UTF8, 0, character.data(),
                                                 static_cast<int>(character.size()),
                                                 nullptr, 0, nullptr, nullptr);
      utf8.resize(static_cast<size_t>(required));
      WideCharToMultiByte(CP_UTF8, 0, character.data(),
                          static_cast<int>(character.size()), utf8.data(),
                          required, nullptr, nullptr);
    }
    doroti_windows_utf8_v1 text{
        DOROTI_WINDOWS_ABI_VERSION_V1, sizeof(doroti_windows_utf8_v1),
        reinterpret_cast<const uint8_t*>(utf8.data()), utf8.size()};
    const bool up = message == WM_KEYUP || message == WM_SYSKEYUP;
    const bool repeat = !up && (lparam & (1u << 30)) != 0;
    const auto scan = static_cast<int64_t>((lparam >> 16) & 0x1FF);
    doroti_windows_key_v1 key{
        DOROTI_WINDOWS_ABI_VERSION_V1, sizeof(doroti_windows_key_v1),
        1, QpcNow(), up ? 1u : (repeat ? 2u : 0u), repeat ? 1u : 0u,
        scan, static_cast<int64_t>(wparam), text};
    callbacks_.key(callbacks_.callback_context, &key);
  }

  HCURSOR ResolveCursor() const noexcept {
    switch (cursor_kind_.load()) {
      case 1: return LoadCursorW(nullptr, IDC_HAND);
      case 2: return LoadCursorW(nullptr, IDC_NO);
      case 3: return LoadCursorW(nullptr, IDC_WAIT);
      case 7: return LoadCursorW(nullptr, IDC_IBEAM);
      case 10: return LoadCursorW(nullptr, IDC_CROSS);
      case 11: return LoadCursorW(nullptr, IDC_SIZEALL);
      case 19: return LoadCursorW(nullptr, IDC_SIZEWE);
      case 20: return LoadCursorW(nullptr, IDC_SIZENS);
      case 21: return LoadCursorW(nullptr, IDC_SIZENWSE);
      case 22: return LoadCursorW(nullptr, IDC_SIZENESW);
      case 35: return nullptr;
      default: return LoadCursorW(nullptr, IDC_ARROW);
    }
  }

  void Destroy() noexcept {
    StopRenderWorker();
    if (task_ != nullptr) DestroyWindow(task_);
    if (child_ != nullptr) DestroyWindow(child_);
    if (top_ != nullptr) DestroyWindow(top_);
    task_ = child_ = top_ = nullptr;
    app_window_ = nullptr;
  }

  doroti_windows_configuration_v1 configuration_{};
  doroti_windows_callbacks_v1 callbacks_{};
  HWND top_{};
  HWND child_{};
  HWND task_{};
  HMONITOR stable_monitor_{};
  winrt::Microsoft::UI::Windowing::AppWindow app_window_{nullptr};
  DWORD platform_thread_id_{};
  uint64_t current_generation_{};
  uint64_t causal_frame_id_{};
  uint32_t current_width_{};
  uint32_t current_height_{};
  double current_scale_{};
  int64_t current_metrics_qpc_{};
  uint64_t delivered_metrics_generation_{};
  bool show_requested_{};
  bool first_exact_present_{};
  bool fatal_{};
  bool mouse_inside_{};
  bool pointer_down_{};
  LPARAM last_pointer_lparam_{};
  uint64_t pointer_sequence_{};
  std::atomic<uint32_t> cursor_kind_{};
  doroti_windows_text_configuration_v1 text_configuration_{};
  std::wstring text_;
  int32_t text_selection_base_{};
  int32_t text_selection_extent_{};
  int32_t text_composing_base_{-1};
  int32_t text_composing_extent_{-1};
  CaretCommand caret_{};
  bool text_client_active_{};
  bool ime_composing_{};
  bool text_smoke_emitted_{};
  bool semantics_smoke_emitted_{};
  bool minimized_{};
  bool interactive_move_{};
  bool interactive_move_dirty_{};
  uint32_t lifecycle_state_{std::numeric_limits<uint32_t>::max()};
  uint32_t lifecycle_smoke_phase_{};
  doroti::windows::AccessibilityBridge accessibility_;
  std::mutex render_mutex_;
  std::condition_variable render_condition_;
  std::condition_variable resize_condition_;
  std::optional<RenderWork> render_pending_;
  std::deque<doroti_windows_frame_terminal_v1> render_completions_;
  std::unordered_set<uint64_t> resize_wait_timeouts_;
  std::thread render_thread_;
  uint64_t last_render_terminal_generation_{};
  uint64_t last_render_terminal_causal_frame_id_{};
  uint32_t last_render_terminal_kind_{};
  bool render_worker_started_{};
  bool render_stopping_{};
};

ProductHost* GetHost(HWND window, UINT message, LPARAM lparam) {
  if (message == WM_NCCREATE) {
    const auto* create = reinterpret_cast<CREATESTRUCTW*>(lparam);
    SetWindowLongPtrW(window, GWLP_USERDATA,
                      reinterpret_cast<LONG_PTR>(create->lpCreateParams));
  }
  return reinterpret_cast<ProductHost*>(GetWindowLongPtrW(window, GWLP_USERDATA));
}

LRESULT CALLBACK TopProcedure(HWND window, UINT message, WPARAM wparam,
                              LPARAM lparam) {
  auto* host = GetHost(window, message, lparam);
  return host != nullptr ? host->HandleTop(window, message, wparam, lparam)
                         : DefWindowProcW(window, message, wparam, lparam);
}

LRESULT CALLBACK ChildProcedure(HWND window, UINT message, WPARAM wparam,
                                LPARAM lparam) {
  auto* host = GetHost(window, message, lparam);
  return host != nullptr ? host->HandleChild(window, message, wparam, lparam)
                         : DefWindowProcW(window, message, wparam, lparam);
}

LRESULT CALLBACK TaskProcedure(HWND window, UINT message, WPARAM wparam,
                               LPARAM lparam) {
  auto* host = GetHost(window, message, lparam);
  return host != nullptr ? host->HandleTask(window, message, wparam, lparam)
                         : DefWindowProcW(window, message, wparam, lparam);
}

static_assert(sizeof(void*) == 8, "The v1 product binary is win-x64 only.");
static_assert(std::is_standard_layout_v<doroti_windows_metrics_v1>);
static_assert(std::is_standard_layout_v<doroti_windows_frame_request_v1>);
static_assert(std::is_standard_layout_v<doroti_windows_host_v1>);
static_assert(std::is_standard_layout_v<doroti_windows_frame_terminal_v1>);
static_assert(std::is_standard_layout_v<doroti_windows_pointer_v1>);
static_assert(std::is_standard_layout_v<doroti_windows_key_v1>);
static_assert(std::is_standard_layout_v<doroti_windows_text_configuration_v1>);
static_assert(std::is_standard_layout_v<doroti_windows_text_state_v1>);
static_assert(std::is_standard_layout_v<doroti_windows_configuration_v1>);
static_assert(std::is_standard_layout_v<doroti_windows_callbacks_v1>);

}  // namespace

uint32_t DOROTI_WINDOWS_CALL doroti_windows_get_abi_version_v1(void) {
  return DOROTI_WINDOWS_ABI_VERSION_V1;
}

doroti_windows_status_v1 DOROTI_WINDOWS_CALL
doroti_windows_get_abi_layout_v1(doroti_windows_abi_layout_v1* layout) {
  if (!ValidHeader(layout)) return DOROTI_WINDOWS_STATUS_ABI_MISMATCH_V1;
  *layout = {
      DOROTI_WINDOWS_ABI_VERSION_V1,
      sizeof(doroti_windows_abi_layout_v1),
      sizeof(void*),
      8,
      sizeof(doroti_windows_utf8_v1),
      sizeof(doroti_windows_metrics_v1),
      sizeof(doroti_windows_frame_request_v1),
      sizeof(doroti_windows_host_v1),
      sizeof(doroti_windows_frame_terminal_v1),
      sizeof(doroti_windows_configuration_v1),
      sizeof(doroti_windows_callbacks_v1),
      offsetof(doroti_windows_metrics_v1, generation),
      offsetof(doroti_windows_host_v1, child_hwnd),
      offsetof(doroti_windows_frame_terminal_v1, terminal_kind),
      offsetof(doroti_windows_callbacks_v1, render),
      0,
      sizeof(doroti_windows_pointer_v1),
      sizeof(doroti_windows_key_v1),
      offsetof(doroti_windows_callbacks_v1, pointer),
      offsetof(doroti_windows_host_v1, set_cursor),
      sizeof(doroti_windows_text_configuration_v1),
      sizeof(doroti_windows_text_state_v1),
      offsetof(doroti_windows_host_v1, set_text_client),
      offsetof(doroti_windows_callbacks_v1, text_editing),
      offsetof(doroti_windows_callbacks_v1, lifecycle),
  };
  return DOROTI_WINDOWS_STATUS_OK_V1;
}

doroti_windows_status_v1 DOROTI_WINDOWS_CALL doroti_windows_run_v1(
    const doroti_windows_configuration_v1* configuration,
    const doroti_windows_callbacks_v1* callbacks) {
  if (!ValidHeader(configuration) || !ValidHeader(callbacks))
    return DOROTI_WINDOWS_STATUS_ABI_MISMATCH_V1;
  if (callbacks->host_ready == nullptr || callbacks->metrics == nullptr ||
      callbacks->render == nullptr || callbacks->frame_terminal == nullptr ||
      callbacks->text_editing == nullptr || callbacks->text_action == nullptr ||
      callbacks->semantics_action == nullptr ||
      callbacks->lifecycle == nullptr ||
      configuration->initial_width_px == 0 ||
      configuration->initial_height_px == 0)
    return DOROTI_WINDOWS_STATUS_INVALID_ARGUMENT_V1;

  bool bootstrap_initialized = false;
  try {
    const auto bootstrap = MddBootstrapInitialize2(
        WINDOWSAPPSDK_RELEASE_MAJORMINOR,
        WINDOWSAPPSDK_RELEASE_VERSION_TAG_W,
        PACKAGE_VERSION{}, MddBootstrapInitializeOptions_None);
    if (FAILED(bootstrap)) return DOROTI_WINDOWS_STATUS_NATIVE_FAILURE_V1;
    bootstrap_initialized = true;
    ProductHost host(*configuration, *callbacks);
    const auto status = host.Run();
    MddBootstrapShutdown();
    return status;
  } catch (...) {
    if (bootstrap_initialized) MddBootstrapShutdown();
    return DOROTI_WINDOWS_STATUS_NATIVE_FAILURE_V1;
  }
}
