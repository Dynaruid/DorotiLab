#include "doroti_windows_host_v1.h"
#include "accessibility_bridge.h"
#include "resize_order_trace.h"

#include <windows.h>
#include <windowsx.h>
#include <dwmapi.h>
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
#include <cstdio>
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
constexpr auto kExactResizeWait = std::chrono::milliseconds(100);
constexpr uint32_t kFramePrepared = 4;

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

bool HasSelfContainedWindowsAppRuntime() noexcept {
  std::wstring executable_path(32768, L'\0');
  const auto length = GetModuleFileNameW(
      nullptr, executable_path.data(), static_cast<DWORD>(executable_path.size()));
  if (length == 0 || length >= executable_path.size()) return false;
  executable_path.resize(length);
  const auto separator = executable_path.find_last_of(L"\\/");
  if (separator == std::wstring::npos) return false;
  executable_path.resize(separator + 1);
  executable_path.append(L"Microsoft.WindowsAppRuntime.dll");
  const auto attributes = GetFileAttributesW(executable_path.c_str());
  return attributes != INVALID_FILE_ATTRIBUTES &&
         (attributes & FILE_ATTRIBUTE_DIRECTORY) == 0;
}

uint32_t ResolvePlatformBrightness() noexcept {
  DWORD apps_use_light_theme = 1;
  DWORD byte_length = sizeof(apps_use_light_theme);
  const auto status = RegGetValueW(
      HKEY_CURRENT_USER,
      L"Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize",
      L"AppsUseLightTheme", RRF_RT_REG_DWORD, nullptr, &apps_use_light_theme,
      &byte_length);
  return status == ERROR_SUCCESS && apps_use_light_theme == 0
             ? DOROTI_WINDOWS_PLATFORM_BRIGHTNESS_DARK_V1
             : DOROTI_WINDOWS_PLATFORM_BRIGHTNESS_LIGHT_V1;
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
  doroti::resize_trace::Key trace_key{};
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
        platform_thread_id_(GetCurrentThreadId()),
        post_present_dwm_flush_(
            (configuration.required_features &
             DOROTI_WINDOWS_FEATURE_POST_PRESENT_DWM_FLUSH_V1) != 0),
        retained_oversized_child_surface_(
            (configuration.required_features &
             DOROTI_WINDOWS_FEATURE_RETAINED_OVERSIZED_CHILD_SURFACE_V1) != 0),
        composition_presentation_requested_(
            (configuration.required_features &
             DOROTI_WINDOWS_FEATURE_COMPOSITION_PRESENTATION_V1) != 0),
        opaque_composition_background_(
            composition_presentation_requested_ &&
            (configuration.required_features &
             DOROTI_WINDOWS_FEATURE_EXPERIMENTAL_ACRYLIC_V1) == 0),
        composition_requested_(
            (configuration.required_features &
             (DOROTI_WINDOWS_FEATURE_EXPERIMENTAL_ACRYLIC_V1 |
              DOROTI_WINDOWS_FEATURE_COMPOSITION_PRESENTATION_V1)) != 0),
        composition_active_(composition_requested_),
        platform_brightness_(ResolvePlatformBrightness()) {}

  ~ProductHost() { Destroy(); }

  doroti_windows_status_v1 Run() {
    doroti::resize_trace::Initialize();
    CreateCompositionBackgroundBrush();
    RegisterClasses();
    CreateWindows();
    // The selected managed Composition topology owns any AppWindow association
    // and dispatcher setup it needs. Holding a second native AppWindow
    // projection before that setup can make activation fail fast.
    if (!composition_active_) ConnectAppWindow();
    doroti_windows_host_v1 host{
        DOROTI_WINDOWS_ABI_VERSION_V1,
        sizeof(doroti_windows_host_v1),
        this,
        top_,
        composition_active_ ? top_ : child_,
        child_,
        task_,
        &RequestFrame,
        &RequestResize,
        &RequestClose,
        &RequestShow,
        &RequestOpaqueFallback,
        &SetCursor,
        &SetClipboard,
        &RequestClipboard,
        &SetTextClient,
        &UpdateTextState,
        &SetCaretRect,
        &ClearTextClient,
        &UpdateSemantics,
        &ClearSemantics,
        platform_brightness_,
        &SetCompositionChild,
    };
    callbacks_.host_ready(callbacks_.callback_context, &host);
    // Host-ready attaches the managed Composition tree synchronously. Publish
    // its initial viewport after that attachment and before the first raster
    // request.
    if (composition_active_) {
      RECT client{};
      if (!GetClientRect(top_, &client)) throw std::bad_alloc();
      const auto width =
          static_cast<uint32_t>(std::max(0L, client.right - client.left));
      const auto height =
          static_cast<uint32_t>(std::max(0L, client.bottom - client.top));
      if (width > 0 && height > 0) ResizeCompositionViewport(width, height);
    }
    AttachInputServices();
    StartRenderWorker();
    EmitLifecycle(1);
    if (PublishMetrics() && composition_active_)
      composition_flush_generation_.store(current_generation_,
                                            std::memory_order_release);
    ConfigureSmokeTimer();
    QueueRender();

    MSG message{};
    while (GetMessageW(&message, nullptr, 0, 0) > 0) {
      TranslateMessage(&message);
      DispatchMessageW(&message);
    }
    StopRenderWorker();
    doroti::resize_trace::Flush();
    ReleasePlatformResources();
    return fatal_ ? DOROTI_WINDOWS_STATUS_NATIVE_FAILURE_V1
                  : DOROTI_WINDOWS_STATUS_OK_V1;
  }

  LRESULT HandleTop(HWND window, UINT message, WPARAM wparam, LPARAM lparam) {
    doroti::resize_trace::MessageScope trace_scope{
        nullptr, {}, current_generation_};
    if (doroti::resize_trace::enabled && composition_active_) {
      const char* entry = nullptr;
      if (message == WM_SIZING && lparam != 0) {
        trace_key_ = {++trace_epoch_, current_generation_,
                      static_cast<uint32_t>(wparam),
                      *reinterpret_cast<const RECT*>(lparam)};
        entry = "sizing-entry";
        trace_scope.end = "sizing-return";
      } else if (message == WM_WINDOWPOSCHANGING) {
        entry = "windowposchanging-entry";
        trace_scope.end = "windowposchanging-return";
      } else if (message == WM_WINDOWPOSCHANGED) {
        entry = "windowposchanged-entry";
        trace_scope.end = "windowposchanged-return";
      } else if (message == WM_SIZE) {
        entry = "size-entry";
        trace_scope.end = "size-return";
      }
      trace_scope.key = trace_key_;
      trace_scope.key.generation = current_generation_;
      if ((message == WM_WINDOWPOSCHANGING || message == WM_WINDOWPOSCHANGED) &&
          lparam != 0) {
        const auto& pos = *reinterpret_cast<const WINDOWPOS*>(lparam);
        RECT actual{};
        GetWindowRect(window, &actual);
        const auto x = (pos.flags & SWP_NOMOVE) ? actual.left : pos.x;
        const auto y = (pos.flags & SWP_NOMOVE) ? actual.top : pos.y;
        const auto w = (pos.flags & SWP_NOSIZE) ? actual.right - actual.left : pos.cx;
        const auto h = (pos.flags & SWP_NOSIZE) ? actual.bottom - actual.top : pos.cy;
        trace_scope.key.outer = {x, y, x + w, y + h};
        trace_scope.flags = pos.flags;
      }
      if (entry != nullptr)
        doroti::resize_trace::Record(entry, trace_scope.key, trace_scope.flags,
                                    message == WM_WINDOWPOSCHANGING);
    }
    if (composition_active_ && IsClientInputMessage(message))
      return HandleChild(window, message, wparam, lparam);
    switch (message) {
      case WM_ERASEBKGND:
        if (opaque_composition_background_ && wparam != 0) {
          PaintCompositionBackground(reinterpret_cast<HDC>(wparam));
        }
        return 1;
      case WM_PAINT:
        if (opaque_composition_background_) {
          PAINTSTRUCT paint{};
          const auto dc = BeginPaint(window, &paint);
          if (dc != nullptr) PaintCompositionBackground(dc);
          EndPaint(window, &paint);
          return 0;
        }
        break;
      case WM_SIZING:
        if (composition_active_ && lparam != 0) {
          const auto* proposed = reinterpret_cast<const RECT*>(lparam);
          if (!PrepareCompositionSizingFrame(
                  *proposed, static_cast<uint32_t>(wparam))) {
            fatal_ = true;
            PostMessageW(top_, WM_CLOSE, 0, 0);
          }
          // USER32 still owns the proposed rectangle. Fixed-origin edges have
          // submitted and observed the raster; moving-origin edges have only
          // prepared it, for the following WINDOWPOS commit.
          return TRUE;
        }
        break;
      case WM_WINDOWPOSCHANGING:
        if (composition_active_ && lparam != 0 && moving_key_) {
          // Align the prepared transaction before geometry, but keep its pixels
          // non-visible until WM_WINDOWPOSCHANGED reports the actual new origin.
          const auto result = DefWindowProcW(window, message, wparam, lparam);
          CommitMovingFrame(*reinterpret_cast<const WINDOWPOS*>(lparam), true);
          return result;
        }
        break;
      case WM_SIZE:
        if (wparam == SIZE_MINIMIZED) {
          CancelMovingFrame();
          composition_sizing_edge_ = 0;
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
            if (composition_active_) {
              // The top-level HWND is both shell geometry and raster clip.
              // Admit the actual extent and hold this WM_SIZE through the
              // matching Presentation plus DWM commit boundary.
              ApplyCompositionResize(width, height);
              return 0;
            }
            if (retained_oversized_child_surface_) {
              // Vulkan Win32 WSI couples swapchain extent to the surface HWND.
              // Keep that child at a retained capacity and let the parent clip
              // it to the exact visible client. Logical metrics still advance
              // for every top-level size, without forcing a WSI reallocation.
              if (!EnsureRetainedChildSurfaceCapacity(width, height)) {
                fatal_ = true;
                PostMessageW(top_, WM_CLOSE, 0, 0);
                return 0;
              }
              const auto scale =
                  static_cast<double>(GetDpiForWindow(top_)) / 96.0;
              auto exact_presented = false;
              if (UpdateMetrics(width, height, scale)) {
                const auto generation = current_generation_;
                const auto causal = QueueRender();
                // Match Flutter's Windows protocol at the actual-size
                // authority: keep this WM_SIZE transaction open until the
                // raster worker presents the same generation or the shared
                // 100 ms fail-safe expires. The HWND thread never renders.
                exact_presented = WaitForExactResize(generation, causal);
              }
              if (post_present_dwm_flush_ && interactive_move_ &&
                  exact_presented)
                FlushPresentedResizeToDwm();
            } else if (!SetWindowPos(
                           child_, nullptr, 0, 0, static_cast<int>(width),
                           static_cast<int>(height),
                           SWP_NOZORDER | SWP_NOACTIVATE)) {
              // Match Flutter's host_window.cc contract for ANGLE: the one
              // visible child is exactly the top-level physical client extent,
              // and its WM_SIZE owns the bounded render transaction.
              fatal_ = true;
              PostMessageW(top_, WM_CLOSE, 0, 0);
            }
          }
        }
        return 0;
      case WM_ACTIVATEAPP:
        if (!minimized_) EmitLifecycle(wparam != 0 ? 1u : 2u);
        return 0;
      case WM_ACTIVATE:
        if (composition_active_) {
          EmitFocus(LOWORD(wparam) != WA_INACTIVE);
        }
        break;
      case WM_ENTERSIZEMOVE:
        composition_sizing_edge_ = 0;
        interactive_move_ = true;
        composition_interactive_.store(true, std::memory_order_release);
        // A timed-out or failed final-settle request must not leak a DwmFlush
        // into the next interactive sizing loop. WM_EXITSIZEMOVE publishes a
        // fresh generation after clearing this state.
        composition_flush_generation_.store(0, std::memory_order_release);
        opaque_flush_generation_.store(0, std::memory_order_release);
        interactive_move_dirty_ = true;
        if (!composition_active_)
          SetTimer(window, kInteractiveMoveTimer, kInteractiveMoveIntervalMs,
                   nullptr);
        return 0;
      case WM_WINDOWPOSCHANGED:
        if (composition_active_ && lparam != 0 && moving_key_)
          CommitMovingFrame(*reinterpret_cast<const WINDOWPOS*>(lparam), false);
        if (interactive_move_) interactive_move_dirty_ = true;
        if (composition_active_ && child_ != nullptr) {
          RECT client{};
          bool capacity_changed = false;
          if (GetClientRect(top_, &client)) {
            const auto width = static_cast<uint32_t>(
                std::max(0L, client.right - client.left));
            const auto height = static_cast<uint32_t>(
                std::max(0L, client.bottom - client.top));
            if (width > 0 && height > 0 &&
                !EnsureRetainedChildSurfaceCapacity(
                    width, height, &capacity_changed)) {
              fatal_ = true;
              PostMessageW(top_, WM_CLOSE, 0, 0);
              return 0;
            }
          }
          // The hidden child is only a retained-capacity probe. A monitor
          // transition can grow it without changing the top-level extent, so
          // republish the current metrics once when its capacity changes.
          if (capacity_changed && RepublishCurrentMetrics()) QueueRender();
        }
        // WM_DPICHANGED can arrive while the shell window still straddles two
        // monitors. Wait until the committed window rectangle is wholly on a
        // different monitor before rebuilding the fixed-size EGL surface.
        if (!composition_active_ && interactive_move_ &&
            EnteredDifferentMonitor() &&
            RepublishCurrentMetrics())
          QueueRender();
        break;
      case WM_EXITSIZEMOVE:
        CancelMovingFrame();
        trace_key_ = {};
        KillTimer(window, kInteractiveMoveTimer);
        interactive_move_ = false;
        composition_interactive_.store(false, std::memory_order_release);
        EnteredDifferentMonitor();
        // Finish native sizing at the exact actual geometry. A matching
        // pre-submitted frame does not need a second raster, but after the
        // modal loop ends it is safe to observe one DWM boundary at the final
        // origin. Programmatic/mismatched geometry still renders exactly in
        // ApplyCompositionResize before this final settle.
        if (composition_active_) {
          RECT client{};
          if (GetClientRect(top_, &client)) {
            const auto width = static_cast<uint32_t>(
                std::max(0L, client.right - client.left));
            const auto height = static_cast<uint32_t>(
                std::max(0L, client.bottom - client.top));
            if (width > 0 && height > 0) {
              ApplyCompositionResize(width, height);
              FlushPresentedResizeToDwm();
            }
          }
        } else if (RepublishCurrentMetrics()) {
          const auto generation = current_generation_;
          if (post_present_dwm_flush_ && !interactive_move_)
            opaque_flush_generation_.store(generation,
                                           std::memory_order_release);
          const auto causal = QueueRender();
          if (post_present_dwm_flush_)
            WaitForExactResize(generation, causal);
        }
        interactive_move_dirty_ = false;
        return 0;
      case WM_DPICHANGED: {
        CancelMovingFrame();
        composition_sizing_edge_ = 0;
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
      case WM_SETTINGCHANGE:
      case WM_THEMECHANGED:
        RefreshPlatformBrightness();
        break;
      case WM_CLOSE:
        CancelMovingFrame();
        // Close the managed posting gate before joining the raster worker.
        // A final in-flight scene submit may request another frame while the
        // worker is draining; that invalidation is obsolete once close has
        // begun and must not race the task HWND teardown.
        EmitLifecycle(0);
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
          if ((lifecycle_smoke_phase_ % 2u) == 0u &&
              lifecycle_smoke_phase_ / 2u < lifecycle_smoke_cycles_) {
            ShowWindow(top_, SW_MINIMIZE);
            if (!IsIconic(top_) || !minimized_) {
              // The first smoke transition can race the initial ShowWindow
              // and a managed device-reset request. Advance only after USER32
              // has actually delivered SIZE_MINIMIZED; otherwise the next
              // timer would issue restore for a transition that never began.
              SetTimer(top_, kLifecycleTimer, 120, nullptr);
              return 0;
            }
            ++lifecycle_smoke_phase_;
            SetTimer(top_, kLifecycleTimer, 120, nullptr);
          } else if ((lifecycle_smoke_phase_ % 2u) == 1u) {
            ShowWindow(top_, SW_RESTORE);
            if (IsIconic(top_) || minimized_) {
              SetTimer(top_, kLifecycleTimer, 120, nullptr);
              return 0;
            }
            ++lifecycle_smoke_phase_;
            if (lifecycle_smoke_phase_ / 2u < lifecycle_smoke_cycles_)
              SetTimer(top_, kLifecycleTimer, 120, nullptr);
            else
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
        if (opaque_composition_background_ && wparam != 0)
          PaintCompositionChildBackground(reinterpret_cast<HDC>(wparam));
        return 1;
      case WM_PAINT:
        if (opaque_composition_background_) {
          PAINTSTRUCT paint{};
          const auto dc = BeginPaint(window, &paint);
          if (dc != nullptr) PaintCompositionChildBackground(dc);
          EndPaint(window, &paint);
          return 0;
        }
        break;
      case WM_SIZE: {
        if (composition_active_) return 0;
        if (retained_oversized_child_surface_) return 0;
        if (!render_worker_started_ || wparam == SIZE_MINIMIZED)
          return 0;
        if (PublishMetrics()) {
          const auto generation = current_generation_;
          if (post_present_dwm_flush_ && !interactive_move_)
            opaque_flush_generation_.store(generation,
                                           std::memory_order_release);
          const auto causal = QueueRender();
          // Match Flutter's bounded exact-size handshake: the modal sizing
          // loop advances after this generation crosses QueuePresent. GPU
          // completion and the final-settle DwmFlush stay outside this wait.
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
      case WM_NCHITTEST:
        // Composition keeps both visible pixels and input/focus/IME/UIA on the
        // top-level HWND; the hidden capacity child must never intercept input.
        if (composition_active_) return HTTRANSPARENT;
        break;
      case WM_SETFOCUS:
      case WM_KILLFOCUS:
        EmitFocus(message == WM_SETFOCUS);
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
      case WM_SYSKEYUP: {
        // The custom HWND has no native edit control, so it owns the editing
        // keys below while a text client is active. Do not also route them
        // through the framework shortcut map or one press can edit twice.
        const auto native_editing_key = text_client_active_ &&
            (wparam == VK_BACK || wparam == VK_DELETE || wparam == VK_LEFT ||
             wparam == VK_RIGHT || wparam == VK_HOME || wparam == VK_END);
        if (!native_editing_key) EmitKey(message, wparam, lparam);
        if (native_editing_key && (message == WM_KEYDOWN || message == WM_SYSKEYDOWN) &&
            wparam != VK_BACK)
          HandleNavigationKey(wparam);
        return 0;
      }
      default:
        return DefWindowProcW(window, message, wparam, lparam);
    }
    return DefWindowProcW(window, message, wparam, lparam);
  }

 private:
  static uint32_t DOROTI_WINDOWS_CALL RequestFrame(void* context) {
    auto* host = static_cast<ProductHost*>(context);
    if (host == nullptr) return 4u;
    SetLastError(ERROR_SUCCESS);
    if (PostMessageW(host->task_, kRequestFrame, 0, 0)) return 0u;
    const auto error = GetLastError();
    std::fprintf(stderr,
                 "doroti.windows.request_frame_failure error=%lu task=%p top=%p\n",
                 static_cast<unsigned long>(error), host->task_, host->top_);
    std::fflush(stderr);
    // A managed metrics/delayed-frame callback can overlap WM_DESTROY after
    // the final native terminal was drained. Once the top-level window is
    // gone, rejecting that obsolete invalidation would turn an orderly close
    // into a false fatal error; disposal clears the managed pending frame.
    return host->top_ == nullptr ? 0u : 4u;
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

  static uint32_t DOROTI_WINDOWS_CALL RequestOpaqueFallback(void* context) {
    auto* host = static_cast<ProductHost*>(context);
    if (host == nullptr || host->top_ == nullptr || host->child_ == nullptr)
      return 1;
    if (GetCurrentThreadId() != host->platform_thread_id_) return 2;
    if (!host->composition_requested_ || !host->composition_active_) return 3;
    if (host->first_exact_present_) return 4;
    host->composition_active_ = false;
    RECT client{};
    if (!GetClientRect(host->top_, &client) ||
        !SetWindowPos(host->child_, nullptr, 0, 0,
                      client.right - client.left, client.bottom - client.top,
                      SWP_NOZORDER | SWP_NOACTIVATE | SWP_SHOWWINDOW))
      return 5;
    return 0;
  }

  static uint32_t DOROTI_WINDOWS_CALL SetCompositionChild(
      void* context, void* child_hwnd) {
    auto* host = static_cast<ProductHost*>(context);
    (void)child_hwnd;
    // ABI v1 compatibility slot. The hidden retained-capacity child remains
    // native-owned; the top-level HWND is the Composition raster authority.
    return host != nullptr && GetCurrentThreadId() == host->platform_thread_id_
               ? 0u
               : 1u;
  }

  void ResizeCompositionViewport(uint32_t width, uint32_t height,
                                 uint32_t sizing_edge = 0,
                                 uint32_t resize_phase =
                                     DOROTI_WINDOWS_COMPOSITION_RESIZE_POST_GEOMETRY_V1) {
    if (callbacks_.composition_resize == nullptr || width == 0 || height == 0)
      return;
    const auto scale = static_cast<double>(GetDpiForWindow(top_)) / 96.0;
    callbacks_.composition_resize(
        callbacks_.callback_context, width, height, scale, sizing_edge,
        resize_phase);
  }

  static uint32_t DOROTI_WINDOWS_CALL SetCursor(void* context, uint32_t cursor) {
    auto* host = static_cast<ProductHost*>(context);
    if (host == nullptr || cursor > 35) return 1;
    host->cursor_kind_.store(cursor);
    const auto target = host->InputWindow();
    PostMessageW(target, WM_SETCURSOR,
                 reinterpret_cast<WPARAM>(target),
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
      UINT style;
      HBRUSH background;
    } entries[]{
        {kTopClass, TopProcedure, CS_HREDRAW | CS_VREDRAW,
         composition_background_brush_},
        {kChildClass, ChildProcedure, 0, composition_background_brush_},
        {kTaskClass, TaskProcedure, 0, nullptr}};
    for (const auto& entry : entries) {
      WNDCLASSEXW value{};
      value.cbSize = sizeof(value);
      value.style = entry.style;
      value.lpfnWndProc = entry.procedure;
      value.hInstance = instance;
      value.hCursor = LoadCursorW(nullptr, IDC_ARROW);
      value.hbrBackground = entry.background;
      value.lpszClassName = entry.name;
      if (!RegisterClassExW(&value) &&
          GetLastError() != ERROR_CLASS_ALREADY_EXISTS)
        throw std::bad_alloc();
    }
  }

  DWORD TopWindowStyle() const noexcept {
    // The Composition path has no visible child. Non-Composition retained WSI
    // keeps its oversized child clipped by the top-level client.
    return WS_OVERLAPPEDWINDOW |
           (retained_oversized_child_surface_ && !composition_active_
                ? WS_CLIPCHILDREN
                : 0u);
  }

  DWORD TopWindowExtendedStyle() const noexcept {
    // Composition owns every client pixel through HWND-attached visual trees.
    // A separate USER32 redirection bitmap can expose its default white erase
    // between the shell geometry commit and the next target-size commit during
    // top/left resize. Remove that third plane; opaque Composition supplies its
    // own background visual and Acrylic supplies the non-topmost backdrop
    // target beneath the native topmost Vulkan target.
    return composition_active_
               ? static_cast<DWORD>(WS_EX_NOREDIRECTIONBITMAP)
               : 0u;
  }

  void CreateCompositionBackgroundBrush() {
    if (!opaque_composition_background_ ||
        composition_background_brush_ != nullptr)
      return;
    const auto argb = configuration_.composition_background_argb != 0
                          ? configuration_.composition_background_argb
                          : 0xff000000u;
    composition_background_brush_ = CreateSolidBrush(RGB(
        (argb >> 16) & 0xffu, (argb >> 8) & 0xffu, argb & 0xffu));
    if (composition_background_brush_ == nullptr) throw std::bad_alloc();
  }

  void PaintCompositionBackground(HDC dc) noexcept {
    if (!opaque_composition_background_ || dc == nullptr ||
        composition_background_brush_ == nullptr || top_ == nullptr)
      return;
    RECT client{};
    if (GetClientRect(top_, &client))
      FillRect(dc, &client, composition_background_brush_);
  }

  void PaintCompositionBackgroundNow() noexcept {
    if (!opaque_composition_background_ || top_ == nullptr) return;
    const auto dc = GetDC(top_);
    if (dc == nullptr) return;
    PaintCompositionBackground(dc);
    ReleaseDC(top_, dc);
  }

  void PaintCompositionChildBackground(HDC dc) noexcept {
    if (!opaque_composition_background_ || dc == nullptr ||
        composition_background_brush_ == nullptr || child_ == nullptr)
      return;
    RECT client{};
    if (GetClientRect(child_, &client))
      FillRect(dc, &client, composition_background_brush_);
  }

  void PaintCompositionChildBackgroundNow() noexcept {
    if (!opaque_composition_background_ || child_ == nullptr) return;
    const auto dc = GetDC(child_);
    if (dc == nullptr) return;
    PaintCompositionChildBackground(dc);
    ReleaseDC(child_, dc);
  }

  void FlushPresentedResizeToDwm() noexcept {
    // The exact logical frame has completed its presenter-specific GPU and
    // presentation commit while the actual WM_SIZE transaction is still open.
    // Hold the HWND thread through one DWM boundary so USER32 cannot immediately
    // outrun that presented generation.
    const auto result = DwmFlush();
    if (SUCCEEDED(result)) {
      ++presented_resize_dwm_flush_count_;
      return;
    }
    ++dwm_flush_failure_count_;
    wchar_t message[160]{};
    swprintf_s(message,
               L"Doroti presented-resize DwmFlush failed: 0x%08X\n",
               static_cast<unsigned int>(result));
    OutputDebugStringW(message);
  }

  bool PrepareCompositionSizingFrame(
      const RECT& proposed_window, uint32_t sizing_edge) {
    CancelMovingFrame();
    if (top_ == nullptr || child_ == nullptr || !render_worker_started_)
      return false;
    RECT current_window{};
    RECT current_client{};
    if (!GetWindowRect(top_, &current_window) ||
        !GetClientRect(top_, &current_client))
      return false;

    const auto frame_width = std::max(
        0L, (current_window.right - current_window.left) -
                (current_client.right - current_client.left));
    const auto frame_height = std::max(
        0L, (current_window.bottom - current_window.top) -
                (current_client.bottom - current_client.top));
    const auto proposed_width = proposed_window.right - proposed_window.left;
    const auto proposed_height = proposed_window.bottom - proposed_window.top;
    if (proposed_width <= frame_width || proposed_height <= frame_height)
      return true;

    const auto width = static_cast<uint32_t>(proposed_width - frame_width);
    const auto height = static_cast<uint32_t>(proposed_height - frame_height);
    composition_sizing_edge_ = sizing_edge;
    bool capacity_changed = false;
    if (!EnsureRetainedChildSurfaceCapacity(
            width, height, &capacity_changed))
      return false;

    ResizeCompositionViewport(
        width, height, sizing_edge,
        DOROTI_WINDOWS_COMPOSITION_RESIZE_PRE_GEOMETRY_V1);
    const auto scale = static_cast<double>(GetDpiForWindow(top_)) / 96.0;
    if (!UpdateMetrics(width, height, scale) && !capacity_changed)
      return true;
    if (capacity_changed && current_width_ == width && current_height_ == height)
      RepublishCurrentMetrics();

    const auto generation = current_generation_;
    const auto moving = composition_presentation_requested_ &&
        callbacks_.moving_frame != nullptr &&
        (sizing_edge == WMSZ_LEFT || sizing_edge == WMSZ_TOP ||
         sizing_edge == WMSZ_TOPLEFT || sizing_edge == WMSZ_TOPRIGHT ||
         sizing_edge == WMSZ_BOTTOMLEFT);
    if (moving) {
      moving_key_ = doroti_windows_moving_frame_v1{
          ++moving_epoch_, generation, sizing_edge,
          proposed_window.left, proposed_window.top,
          proposed_window.right, proposed_window.bottom, width, height, scale};
      if (callbacks_.moving_frame(callbacks_.callback_context, 1, &*moving_key_) != 0) {
        CancelMovingFrame();
        return false;
      }
    }
    const auto causal = QueueRender();
    // Moving-origin completion means a non-visible copied slot is reserved.
    // Fixed-origin completion still includes Present and its display wait.
    // The platform never performs raster work or waits on a Vulkan fence.
    return WaitForExactResize(generation, causal, kExactResizeWait, true, moving);
  }

  void ResolvePreparedFrame(uint32_t terminal) {
    {
      std::lock_guard lock(render_mutex_);
      if (!prepared_work_) return;
      render_completions_.push_back(MakeTerminal(*prepared_work_, terminal,
          terminal == DOROTI_WINDOWS_FRAME_FAILED_V1 ? 1u : 0u));
      prepared_work_.reset();
    }
    render_condition_.notify_one();
    PostMessageW(task_, kRenderCompleted, 0, 0);
  }

  void CancelMovingFrame() {
    moving_phase_aligned_ = false;
    if (moving_key_) {
      callbacks_.moving_frame(callbacks_.callback_context, 3, &*moving_key_);
      moving_key_.reset();
      composition_force_exact_ = true;
    }
    ResolvePreparedFrame(DOROTI_WINDOWS_FRAME_SUPERSEDED_V1);
  }

  void CommitMovingFrame(const WINDOWPOS& pos, bool align_only) {
    if (!moving_key_) return;
    const auto key = *moving_key_;
    RECT outer{}, client{};
    const auto rectangles = GetWindowRect(top_, &outer) && GetClientRect(top_, &client);
    const auto frame_width = outer.right - outer.left - (client.right - client.left);
    const auto frame_height = outer.bottom - outer.top - (client.bottom - client.top);
    const auto match = rectangles && (pos.flags & (SWP_NOMOVE | SWP_NOSIZE)) == 0 &&
        pos.x == key.left && pos.y == key.top && pos.x + pos.cx == key.right &&
        pos.y + pos.cy == key.bottom && pos.cx - frame_width == static_cast<int>(key.width) &&
        pos.cy - frame_height == static_cast<int>(key.height) &&
        key.generation == current_generation_ && key.sizing_edge == composition_sizing_edge_ &&
        key.scale == static_cast<double>(GetDpiForWindow(top_)) / 96.0;
    bool ready = false;
    {
      std::lock_guard lock(render_mutex_);
      ready = prepared_work_ && prepared_work_->request.generation == key.generation;
    }
    const auto actual_matches = align_only ||
        (moving_phase_aligned_ && outer.left == key.left && outer.top == key.top &&
         outer.right == key.right && outer.bottom == key.bottom &&
         client.right - client.left == static_cast<LONG>(key.width) &&
         client.bottom - client.top == static_cast<LONG>(key.height));
    if (!match || !ready || !actual_matches) {
      doroti::resize_trace::Record("prepared-windowpos-mismatch", trace_key_);
      callbacks_.moving_frame(callbacks_.callback_context, 4, &key);
      CancelMovingFrame();
      return;
    }
    doroti::resize_trace::render_key = {
        trace_key_.epoch, key.generation, key.sizing_edge,
        {key.left, key.top, key.right, key.bottom}};
    const auto result = callbacks_.moving_frame(callbacks_.callback_context, align_only ? 5u : 2u, &key);
    if (align_only) {
      moving_phase_aligned_ = result == 0;
      doroti::resize_trace::Record("prepared-clock-ready", doroti::resize_trace::render_key);
      if (result != 0) CancelMovingFrame();
      return;
    }
    moving_phase_aligned_ = false;
    moving_key_.reset();
    if (result == 0) {
      composition_force_exact_ = false;
      ResolvePreparedFrame(DOROTI_WINDOWS_FRAME_PRESENTED_V1);
    } else {
      callbacks_.moving_frame(callbacks_.callback_context, 3, &key);
      composition_force_exact_ = true;
      ResolvePreparedFrame(result < 0 ? DOROTI_WINDOWS_FRAME_FAILED_V1 :
                                      DOROTI_WINDOWS_FRAME_SUPERSEDED_V1);
    }
  }

  void ApplyCompositionResize(uint32_t width, uint32_t height) {
    const auto scale = static_cast<double>(GetDpiForWindow(top_)) / 96.0;
    // An interactive WM_SIZING already published and submitted this exact
    // viewport. Preserve that revision so the next ordinary app frame cannot
    // accidentally inherit a post-geometry display wait. Programmatic, DPI,
    // and otherwise mismatched geometry still take the exact fallback below.
    if (!composition_force_exact_ && current_generation_ != 0 && current_width_ == width &&
        current_height_ == height && current_scale_ == scale)
      return;
    ResizeCompositionViewport(
        width, height, composition_sizing_edge_,
        DOROTI_WINDOWS_COMPOSITION_RESIZE_POST_GEOMETRY_V1);
    const auto changed = UpdateMetrics(width, height, scale);
    const auto forced = composition_force_exact_;
    composition_force_exact_ = false;
    if (forced && !changed) RepublishCurrentMetrics();
    if (changed || forced) {
      const auto generation = current_generation_;
      const auto causal = QueueRender();
      // Programmatic and mismatched resizes that did not pass WM_SIZING still
      // fall back to an exact post-geometry render here. Interactive border
      // resizes normally arrive with the same pre-submitted metrics and do not
      // create a second frame.
      WaitForExactResize(generation, causal);
    }
  }

  SIZE ResolveRetainedChildSurfaceCapacity(uint32_t width,
                                           uint32_t height) const noexcept {
    auto desired_width = static_cast<LONG>(std::max(1u, width));
    auto desired_height = static_cast<LONG>(std::max(1u, height));
    const auto monitor = MonitorFromWindow(top_, MONITOR_DEFAULTTONEAREST);
    MONITORINFO info{};
    info.cbSize = sizeof(info);
    if (monitor != nullptr && GetMonitorInfoW(monitor, &info)) {
      // WM_SIZE/client coordinates in this host are logical while monitor
      // rectangles are physical on a scaled display. Keep the retained child
      // and managed Presentation capacity in the same coordinate space.
      const auto dpi = std::max(96u, GetDpiForWindow(top_));
      const auto monitor_width = std::max<LONG>(
          1, static_cast<LONG>(MulDiv(
                 info.rcWork.right - info.rcWork.left, 96,
                 static_cast<int>(dpi))));
      const auto monitor_height = std::max<LONG>(
          1, static_cast<LONG>(MulDiv(
                 info.rcWork.bottom - info.rcWork.top, 96,
                 static_cast<int>(dpi))));
      desired_width = std::max(desired_width, monitor_width);
      desired_height = std::max(desired_height, monitor_height);
    }

    // A window spanning beyond its monitor grows in bounded chunks. Ordinary
    // maximize/restore and border drags within the work area require no child
    // resize and therefore no Vulkan swapchain recreation.
    if (retained_surface_width_ > 0 && desired_width > retained_surface_width_)
      desired_width = std::max(
          desired_width,
          retained_surface_width_ + std::max(256L, retained_surface_width_ / 4));
    if (retained_surface_height_ > 0 && desired_height > retained_surface_height_)
      desired_height = std::max(
          desired_height,
          retained_surface_height_ + std::max(256L, retained_surface_height_ / 4));
    constexpr LONG kPresentationCapacityQuantum = 256;
    desired_width =
        ((desired_width + kPresentationCapacityQuantum - 1) /
         kPresentationCapacityQuantum) * kPresentationCapacityQuantum;
    desired_height =
        ((desired_height + kPresentationCapacityQuantum - 1) /
         kPresentationCapacityQuantum) * kPresentationCapacityQuantum;
    return {desired_width, desired_height};
  }

  bool EnsureRetainedChildSurfaceCapacity(
      uint32_t width, uint32_t height,
      bool* capacity_changed = nullptr) {
    if (capacity_changed != nullptr) *capacity_changed = false;
    if (!retained_oversized_child_surface_) return true;
    const auto needs_capacity =
        width > static_cast<uint32_t>(retained_surface_width_) ||
        height > static_cast<uint32_t>(retained_surface_height_);
    if (needs_capacity) {
      const auto capacity = ResolveRetainedChildSurfaceCapacity(width, height);
      if (!SetWindowPos(child_, nullptr, 0, 0, capacity.cx, capacity.cy,
                        SWP_NOZORDER | SWP_NOACTIVATE))
        return false;
      PaintCompositionChildBackgroundNow();
      RECT actual{};
      if (!GetClientRect(child_, &actual)) return false;
      retained_surface_width_ = actual.right - actual.left;
      retained_surface_height_ = actual.bottom - actual.top;
      if (capacity_changed != nullptr) *capacity_changed = true;
    }
    return true;
  }

  void CreateWindows() {
    const auto instance = GetModuleHandleW(nullptr);
    const auto dpi = GetDpiForSystem();
    RECT bounds{0, 0,
                static_cast<LONG>(std::max(1u, configuration_.initial_width_px)),
                static_cast<LONG>(std::max(1u, configuration_.initial_height_px))};
    const auto top_style = TopWindowStyle();
    if (!AdjustWindowRectExForDpi(&bounds, top_style, FALSE, 0, dpi))
      throw std::bad_alloc();
    auto title = Decode(configuration_.title);
    if (title.empty()) title = L"Doroti";
    top_ = CreateWindowExW(TopWindowExtendedStyle(), kTopClass, title.c_str(), top_style,
                           CW_USEDEFAULT, CW_USEDEFAULT, bounds.right - bounds.left,
                           bounds.bottom - bounds.top, nullptr, nullptr, instance, this);
    if (top_ == nullptr) throw std::bad_alloc();
    if (opaque_composition_background_) PaintCompositionBackgroundNow();
    ApplyTopLevelTheme();
    stable_monitor_ = MonitorFromWindow(top_, MONITOR_DEFAULTTONEAREST);
    // Composition uses the top-level HWND as the sole visible raster clip. Its
    // child remains hidden and only carries retained monitor capacity for the
    // managed backing allocation. Non-Composition presenters keep the normal
    // visible child HWND.
    const auto child_style = WS_CHILD |
        (!composition_active_ ? static_cast<uint32_t>(WS_VISIBLE) : 0u);
    const auto child_extended_style =
        composition_active_ && !opaque_composition_background_
            ? WS_EX_NOREDIRECTIONBITMAP
            : 0u;
    RECT client{};
    if (!GetClientRect(top_, &client)) throw std::bad_alloc();
    auto child_width = std::max(1L, client.right - client.left);
    auto child_height = std::max(1L, client.bottom - client.top);
    if (retained_oversized_child_surface_) {
      const auto capacity = ResolveRetainedChildSurfaceCapacity(
          static_cast<uint32_t>(child_width), static_cast<uint32_t>(child_height));
      child_width = retained_surface_width_ = capacity.cx;
      child_height = retained_surface_height_ = capacity.cy;
    }
    child_ = CreateWindowExW(child_extended_style, kChildClass, L"", child_style,
                             0, 0, child_width, child_height,
                             top_, nullptr, instance, this);
    task_ = CreateWindowExW(0, kTaskClass, L"", 0, 0, 0, 0, 0, HWND_MESSAGE,
                            nullptr, instance, this);
    if (child_ == nullptr || task_ == nullptr) throw std::bad_alloc();
    if (!SetWindowPos(child_, nullptr, 0, 0, child_width, child_height,
                      SWP_NOZORDER | SWP_NOACTIVATE))
      throw std::bad_alloc();
    PaintCompositionChildBackgroundNow();
    if (retained_oversized_child_surface_) {
      RECT actual{};
      if (!GetClientRect(child_, &actual)) throw std::bad_alloc();
      retained_surface_width_ = actual.right - actual.left;
      retained_surface_height_ = actual.bottom - actual.top;
    }
  }

  void AttachInputServices() {
    accessibility_.Attach(InputWindow(), [this](int64_t node_id, int64_t action,
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
      ImmAssociateContextEx(InputWindow(), nullptr, IACE_DEFAULT);
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
    if (InputWindow() != nullptr) ImmAssociateContextEx(InputWindow(), nullptr, 0);
  }

  void ApplyImeWindowPosition() {
    const auto target = InputWindow();
    if (!text_client_active_ || target == nullptr) return;
    const auto context = ImmGetContext(target);
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
    ImmReleaseContext(target, context);
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
    const auto target = InputWindow();
    const auto context = ImmGetContext(target);
    if (context == nullptr) return false;
    const auto release = [target, context] { ImmReleaseContext(target, context); };
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
    } else {
      const auto next = key == VK_HOME ? 0 : key == VK_END ? length :
          std::clamp(text_selection_extent_ + (key == VK_LEFT ? -1 : 1), 0, length);
      text_selection_base_ = text_selection_extent_ = next;
      text_composing_base_ = text_composing_extent_ = -1;
    }
    EmitTextEditing();
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

  static std::wstring JsonString(
      const winrt::Windows::Data::Json::JsonObject& value,
      const wchar_t* name) {
    if (!value.HasKey(name)) return {};
    const auto item = value.GetNamedValue(name);
    return item.ValueType() == winrt::Windows::Data::Json::JsonValueType::String
               ? std::wstring(item.GetString())
               : std::wstring{};
  }

  static double JsonNumber(const winrt::Windows::Data::Json::JsonObject& value,
                           const wchar_t* name, double fallback = 0) {
    if (!value.HasKey(name)) return fallback;
    const auto item = value.GetNamedValue(name);
    return item.ValueType() == winrt::Windows::Data::Json::JsonValueType::Number
               ? item.GetNumber()
               : fallback;
  }

  static int JsonState(const winrt::Windows::Data::Json::JsonObject& value,
                       const wchar_t* name) {
    if (!value.HasKey(name)) return -1;
    const auto item = value.GetNamedValue(name);
    if (item.ValueType() == winrt::Windows::Data::Json::JsonValueType::Boolean)
      return item.GetBoolean() ? 1 : 0;
    if (item.ValueType() == winrt::Windows::Data::Json::JsonValueType::String) {
      const auto state = item.GetString();
      if (state == L"isTrue") return 1;
      if (state == L"isFalse") return 0;
      if (state == L"mixed") return 2;
    }
    return -1;
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
        node.label = JsonString(source, L"label");
        node.value = JsonString(source, L"value");
        node.identifier = JsonString(source, L"identifier");
        node.hint = JsonString(source, L"hint");
        node.tooltip = JsonString(source, L"tooltip");
        node.link_url = JsonString(source, L"linkUrl");
        node.increased_value = JsonString(source, L"increasedValue");
        node.decreased_value = JsonString(source, L"decreasedValue");
        node.min_value = JsonString(source, L"minValue");
        node.max_value = JsonString(source, L"maxValue");
        node.heading_level = static_cast<int>(JsonNumber(source, L"headingLevel"));
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
          node.focusable = JsonBool(flags, L"focusable");
          node.focused = JsonBool(flags, L"focused");
          node.hidden = JsonBool(flags, L"hidden");
          node.button = JsonBool(flags, L"button");
          node.text_field = JsonBool(flags, L"textField");
          node.read_only = JsonBool(flags, L"readOnly");
          node.slider = JsonBool(flags, L"slider");
          node.mutually_exclusive = JsonBool(flags, L"mutuallyExclusive");
          node.header = JsonBool(flags, L"header");
          node.image = JsonBool(flags, L"image");
          node.live_region = JsonBool(flags, L"liveRegion");
          node.link = JsonBool(flags, L"link");
          node.obscured = JsonBool(flags, L"obscured");
          node.required = JsonBool(flags, L"required");
          node.checked = JsonState(flags, L"checked");
          node.selected = JsonState(flags, L"selected");
          node.toggled = JsonState(flags, L"toggled");
          node.expanded = JsonState(flags, L"expanded");
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
    const auto authority = composition_active_ || retained_oversized_child_surface_
                               ? top_
                               : child_;
    if (authority == nullptr || callbacks_.metrics == nullptr) return false;
    RECT client{};
    if (!GetClientRect(authority, &client)) return false;
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
                    accepted,
                    trace_key_};
    work.trace_key.generation = current_generation_;
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
          return render_stopping_ || (render_pending_.has_value() && !prepared_work_);
        });
        if (render_stopping_ && !render_pending_.has_value()) break;
        work = *render_pending_;
        render_pending_.reset();
      }

      if (work.metrics.generation != delivered_metrics_generation_) {
        callbacks_.metrics(callbacks_.callback_context, &work.metrics);
        delivered_metrics_generation_ = work.metrics.generation;
      }
      doroti::resize_trace::render_key = work.trace_key;
      auto terminal = callbacks_.render(callbacks_.callback_context, &work.request);
      if (terminal != DOROTI_WINDOWS_FRAME_PRESENTED_V1 &&
          terminal != kFramePrepared &&
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
        if (terminal == kFramePrepared) prepared_work_ = work;
        else render_completions_.push_back(receipt);
        last_render_terminal_generation_ = work.request.generation;
        last_render_terminal_causal_frame_id_ = work.request.causal_frame_id;
        last_render_terminal_kind_ = terminal;
      }
      doroti::resize_trace::Record("worker-terminal-notify", work.trace_key);
      resize_condition_.notify_all();
      if (terminal == DOROTI_WINDOWS_FRAME_PRESENTED_V1) {
        if (composition_active_)
          TryFinalSettleDwmFlush(composition_flush_generation_, work);
        if (post_present_dwm_flush_)
          TryFinalSettleDwmFlush(opaque_flush_generation_, work);
      }
      PostMessageW(task_, kRenderCompleted, 0, 0);
    }
  }

  void TryFinalSettleDwmFlush(std::atomic<uint64_t>& requested_generation,
                              const RenderWork& work) {
    if (composition_interactive_.load(std::memory_order_acquire)) return;
    {
      std::lock_guard lock(render_mutex_);
      if (render_stopping_) return;
    }
    auto generation = requested_generation.load(std::memory_order_acquire);
    if (generation == 0 || work.request.generation < generation) return;

    // Claim this settle request once. A failure is diagnostic rather than an
    // instruction to stall every later frame with an unbounded retry loop.
    if (!requested_generation.compare_exchange_strong(
            generation, 0, std::memory_order_acq_rel))
      return;
    if (composition_interactive_.load(std::memory_order_acquire)) return;

    // Terminal waiters are already awake. This one-shot compositor
    // acknowledgement therefore stays outside the platform-thread wait.
    const auto result = DwmFlush();
    if (SUCCEEDED(result)) return;

    // Make the failure observable without converting it into an unbounded
    // retry on every later non-interactive present.
    ++dwm_flush_failure_count_;
    wchar_t message[128]{};
    swprintf_s(message, L"Doroti final-settle DwmFlush failed: 0x%08X\n",
               static_cast<unsigned int>(result));
    OutputDebugStringW(message);
  }

  bool WaitForExactResize(
      uint64_t generation, uint64_t causal_frame_id,
      std::chrono::milliseconds timeout = kExactResizeWait,
      bool record_timeout = true, bool allow_prepared = false) {
    const auto deadline = std::chrono::steady_clock::now() + timeout;
    while (causal_frame_id != 0) {
      std::unique_lock lock(render_mutex_);
      const auto completed = resize_condition_.wait_until(
          lock, deadline, [this, causal_frame_id] {
            return render_stopping_ ||
                   last_render_terminal_causal_frame_id_ >= causal_frame_id;
          });
      if (!completed) {
        if (record_timeout) resize_wait_timeouts_.insert(generation);
        return false;
      }
      if (render_stopping_) return false;
      if (last_render_terminal_causal_frame_id_ == causal_frame_id &&
          last_render_terminal_generation_ == generation) {
        if (last_render_terminal_kind_ == DOROTI_WINDOWS_FRAME_PRESENTED_V1)
          return true;
        if (allow_prepared && last_render_terminal_kind_ == kFramePrepared)
          return true;
        if (last_render_terminal_kind_ == DOROTI_WINDOWS_FRAME_FAILED_V1)
          return false;
      }
      lock.unlock();
      if (std::chrono::steady_clock::now() >= deadline) {
        if (record_timeout) {
          std::lock_guard timeout_lock(render_mutex_);
          resize_wait_timeouts_.insert(generation);
        }
        return false;
      }
      // A superseded/out-of-date frame can complete almost immediately.
      // Back off before retrying so the bounded exact-settle loop cannot fill
      // the task queue with thousands of render-completion messages.
      std::this_thread::sleep_for(std::chrono::milliseconds(1));
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
      auto run_input_smoke = false;
      if (receipt.terminal_kind == DOROTI_WINDOWS_FRAME_PRESENTED_V1 &&
          !first_exact_present_) {
        first_exact_present_ = true;
        run_input_smoke = true;
        if (show_requested_) ShowWindow(top_, ResolveShowCommand());
        if (EnvironmentOne(L"DOROTI_WINDOWS_APPSDK_C8_SMOKE"))
          SetTimer(top_, kLifecycleTimer, 120, nullptr);
      }
      callbacks_.frame_terminal(callbacks_.callback_context, &receipt);
      // The input smoke must run after the first framework frame has attached
      // its input listeners. Running it during native bootstrap races a slower
      // presenter initialization and can drop the synthetic focus-gained edge.
      if (run_input_smoke) RunInputSmoke();
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
    if (composition_presentation_requested_ && callbacks_.moving_frame != nullptr) {
      const doroti_windows_moving_frame_v1 cancelled{};
      callbacks_.moving_frame(callbacks_.callback_context, 3, &cancelled);
    }
    CancelMovingFrame();
    render_worker_started_ = false;
    DrainRenderCompletions();
  }

  void ReleasePlatformResources() noexcept {
    if (platform_resources_released_) return;
    platform_resources_released_ = true;
    callbacks_.platform_resources_shutdown(callbacks_.callback_context);
  }

  void ResizeTop(uint32_t width, uint32_t height) {
    RECT bounds{0, 0, static_cast<LONG>(width), static_cast<LONG>(height)};
    if (!AdjustWindowRectExForDpi(&bounds, TopWindowStyle(), FALSE, 0,
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

  void RefreshPlatformBrightness() {
    const auto brightness = ResolvePlatformBrightness();
    if (brightness == platform_brightness_) return;
    platform_brightness_ = brightness;
    ApplyTopLevelTheme();
    callbacks_.platform_brightness(callbacks_.callback_context, 1, brightness);
  }

  void ApplyTopLevelTheme() const noexcept {
    if (top_ == nullptr) return;
    const BOOL use_dark_mode =
        platform_brightness_ == DOROTI_WINDOWS_PLATFORM_BRIGHTNESS_DARK_V1;
    DwmSetWindowAttribute(top_, DWMWA_USE_IMMERSIVE_DARK_MODE, &use_dark_mode,
                          sizeof(use_dark_mode));
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
    wchar_t cycles_value[16]{};
    const auto cycles_length = GetEnvironmentVariableW(
        L"DOROTI_WINDOWS_APPSDK_LIFECYCLE_CYCLES", cycles_value,
        static_cast<DWORD>(std::size(cycles_value)));
    if (cycles_length != 0 && cycles_length < std::size(cycles_value)) {
      wchar_t* cycles_end{};
      const auto cycles = wcstoul(cycles_value, &cycles_end, 10);
      if (cycles_end != cycles_value && *cycles_end == L'\0' &&
          cycles >= 1 && cycles <= 100)
        lifecycle_smoke_cycles_ = static_cast<uint32_t>(cycles);
    }
  }

  void RunInputSmoke() {
    wchar_t value[8]{};
    if (GetEnvironmentVariableW(L"DOROTI_WINDOWS_APPSDK_INPUT_SMOKE", value,
                                static_cast<DWORD>(std::size(value))) == 0 ||
        value[0] != L'1')
      return;
    const auto target = InputWindow();
    if (composition_active_)
      SendMessageW(target, WM_ACTIVATE, WA_ACTIVE, 0);
    else
      SendMessageW(target, WM_SETFOCUS, 0, 0);
    SendMessageW(target, WM_MOUSEMOVE, 0, MAKELPARAM(10, 20));
    SendMessageW(target, WM_LBUTTONDOWN, MK_LBUTTON, MAKELPARAM(10, 20));
    SendMessageW(target, WM_MOUSEMOVE, MK_LBUTTON, MAKELPARAM(18, 25));
    SendMessageW(target, WM_LBUTTONUP, 0, MAKELPARAM(18, 25));
    RECT client{};
    if (GetClientRect(target, &client)) {
      POINT wheel_point{(client.right - client.left) / 2,
                        (client.bottom - client.top) / 2};
      if (ClientToScreen(target, &wheel_point)) {
        SendMessageW(target, WM_MOUSEWHEEL,
                     MAKEWPARAM(0, static_cast<WORD>(-WHEEL_DELTA)),
                     MAKELPARAM(wheel_point.x, wheel_point.y));
      }
    }
    SendMessageW(target, WM_KEYDOWN, 'A', 1 | (0x1Eu << 16));
    SendMessageW(target, WM_KEYUP, 'A', 1 | (0x1Eu << 16) | (1u << 30) | (1u << 31));
    if (composition_active_)
      SendMessageW(target, WM_ACTIVATE, WA_INACTIVE, 0);
    else
      SendMessageW(target, WM_KILLFOCUS, 0, 0);
  }

  void EmitFocus(bool focused) {
    if (callbacks_.focus != nullptr)
      callbacks_.focus(callbacks_.callback_context, 1, focused ? 1u : 0u,
                       QpcNow());
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

  HWND InputWindow() const noexcept {
    return composition_active_ ? top_ : child_;
  }

  static bool IsClientInputMessage(UINT message) noexcept {
    switch (message) {
      case WM_SETCURSOR:
      case WM_SETFOCUS:
      case WM_KILLFOCUS:
      case WM_GETOBJECT:
      case WM_IME_STARTCOMPOSITION:
      case WM_IME_COMPOSITION:
      case WM_IME_ENDCOMPOSITION:
      case WM_CHAR:
      case WM_MOUSEMOVE:
      case WM_MOUSELEAVE:
      case WM_LBUTTONDOWN:
      case WM_RBUTTONDOWN:
      case WM_MBUTTONDOWN:
      case WM_LBUTTONUP:
      case WM_RBUTTONUP:
      case WM_MBUTTONUP:
      case WM_CANCELMODE:
      case WM_CAPTURECHANGED:
      case WM_MOUSEWHEEL:
      case WM_MOUSEHWHEEL:
      case WM_KEYDOWN:
      case WM_SYSKEYDOWN:
      case WM_KEYUP:
      case WM_SYSKEYUP:
        return true;
      default:
        return false;
    }
  }

  void Destroy() noexcept {
    StopRenderWorker();
    if (task_ != nullptr) DestroyWindow(task_);
    if (child_ != nullptr) DestroyWindow(child_);
    if (top_ != nullptr) DestroyWindow(top_);
    task_ = child_ = top_ = nullptr;
    if (composition_background_brush_ != nullptr)
      DeleteObject(composition_background_brush_);
    composition_background_brush_ = nullptr;
    app_window_ = nullptr;
  }

  doroti_windows_configuration_v1 configuration_{};
  doroti_windows_callbacks_v1 callbacks_{};
  HWND top_{};
  HWND child_{};
  HWND task_{};
  HBRUSH composition_background_brush_{};
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
  uint64_t trace_epoch_{};
  uint64_t moving_epoch_{};
  bool composition_force_exact_{};
  bool moving_phase_aligned_{};
  std::optional<doroti_windows_moving_frame_v1> moving_key_;
  std::optional<RenderWork> prepared_work_;
  doroti::resize_trace::Key trace_key_{};
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
  bool post_present_dwm_flush_{};
  bool retained_oversized_child_surface_{};
  bool composition_presentation_requested_{};
  bool opaque_composition_background_{};
  LONG retained_surface_width_{};
  LONG retained_surface_height_{};
  uint32_t composition_sizing_edge_{};
  std::atomic_bool composition_interactive_{};
  std::atomic<uint64_t> composition_flush_generation_{};
  std::atomic<uint64_t> opaque_flush_generation_{};
  std::atomic<uint64_t> dwm_flush_failure_count_{};
  uint64_t presented_resize_dwm_flush_count_{};
  bool interactive_move_dirty_{};
  bool composition_requested_{};
  bool composition_active_{};
  bool platform_resources_released_{};
  uint32_t lifecycle_state_{std::numeric_limits<uint32_t>::max()};
  uint32_t lifecycle_smoke_phase_{};
  uint32_t lifecycle_smoke_cycles_{1};
  uint32_t platform_brightness_{};
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
static_assert(sizeof(doroti_windows_moving_frame_v1) == 56);

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
      offsetof(doroti_windows_host_v1, initial_platform_brightness),
      offsetof(doroti_windows_callbacks_v1, platform_brightness),
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
      callbacks->platform_brightness == nullptr ||
      callbacks->platform_resources_shutdown == nullptr ||
      callbacks->composition_resize == nullptr ||
      configuration->initial_width_px == 0 ||
      configuration->initial_height_px == 0)
    return DOROTI_WINDOWS_STATUS_INVALID_ARGUMENT_V1;
  if ((configuration->required_features &
       ~static_cast<uint64_t>(
           DOROTI_WINDOWS_FEATURE_EXPERIMENTAL_ACRYLIC_V1 |
           DOROTI_WINDOWS_FEATURE_POST_PRESENT_DWM_FLUSH_V1 |
           DOROTI_WINDOWS_FEATURE_RETAINED_OVERSIZED_CHILD_SURFACE_V1 |
           DOROTI_WINDOWS_FEATURE_COMPOSITION_PRESENTATION_V1 |
           DOROTI_WINDOWS_FEATURE_VULKAN_ACRYLIC_V1 |
           DOROTI_WINDOWS_FEATURE_PREPARED_GEOMETRY_RECEIPT_V1)) != 0)
    return DOROTI_WINDOWS_STATUS_NOT_IMPLEMENTED_V1;
  if ((configuration->required_features &
       DOROTI_WINDOWS_FEATURE_PREPARED_GEOMETRY_RECEIPT_V1) != 0 &&
      (callbacks->moving_frame == nullptr ||
       (configuration->required_features &
        DOROTI_WINDOWS_FEATURE_COMPOSITION_PRESENTATION_V1) == 0))
    return DOROTI_WINDOWS_STATUS_INVALID_ARGUMENT_V1;

  bool bootstrap_initialized = false;
  try {
    if (!HasSelfContainedWindowsAppRuntime()) {
      PACKAGE_VERSION minimum_version{};
      minimum_version.Version = WINDOWSAPPSDK_RUNTIME_VERSION_UINT64;
      const auto result = MddBootstrapInitialize2(
          WINDOWSAPPSDK_RELEASE_MAJORMINOR,
          WINDOWSAPPSDK_RELEASE_VERSION_TAG_W,
          minimum_version,
          MddBootstrapInitializeOptions_None);
      if (FAILED(result)) return DOROTI_WINDOWS_STATUS_NATIVE_FAILURE_V1;
      bootstrap_initialized = true;
    }
    doroti_windows_status_v1 status{};
    {
      ProductHost host(*configuration, *callbacks);
      status = host.Run();
    }
    if (bootstrap_initialized) MddBootstrapShutdown();
    return status;
  } catch (...) {
    if (bootstrap_initialized) MddBootstrapShutdown();
    return DOROTI_WINDOWS_STATUS_NATIVE_FAILURE_V1;
  }
}
