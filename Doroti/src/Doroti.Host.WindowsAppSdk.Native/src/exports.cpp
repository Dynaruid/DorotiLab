#include "doroti_windows_host_v1.h"

#include <windows.h>
#include <windowsx.h>

#include <MddBootstrap.h>
#include <WindowsAppSDK-VersionInfo.h>
#include <winrt/Microsoft.UI.Interop.h>
#include <winrt/Microsoft.UI.Windowing.h>
#include <winrt/base.h>

#include <algorithm>
#include <atomic>
#include <condition_variable>
#include <cstddef>
#include <cstdlib>
#include <cstring>
#include <deque>
#include <iterator>
#include <limits>
#include <mutex>
#include <new>
#include <optional>
#include <string>
#include <thread>
#include <type_traits>

namespace {

constexpr wchar_t kTopClass[] = L"Doroti.Product.HwndExact.Top.v1";
constexpr wchar_t kChildClass[] = L"Doroti.Product.HwndExact.Child.v1";
constexpr wchar_t kTaskClass[] = L"Doroti.Product.HwndExact.Task.v1";
constexpr UINT kRequestFrame = WM_APP + 0x401;
constexpr UINT kRequestResize = WM_APP + 0x402;
constexpr UINT kRequestClose = WM_APP + 0x403;
constexpr UINT kRequestShow = WM_APP + 0x404;
constexpr UINT kRenderCompleted = WM_APP + 0x405;
constexpr UINT_PTR kSmokeTimer = 1;

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

struct ResizeCommand {
  uint32_t width;
  uint32_t height;
};

struct RenderWork {
  doroti_windows_metrics_v1 metrics;
  doroti_windows_frame_request_v1 request;
  int64_t accepted_qpc;
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
    };
    callbacks_.host_ready(callbacks_.callback_context, &host);
    StartRenderWorker();
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
        if (child_ != nullptr && wparam != SIZE_MINIMIZED) {
          const auto width = static_cast<uint32_t>(LOWORD(lparam));
          const auto height = static_cast<uint32_t>(HIWORD(lparam));
          if (width > 0 && height > 0) {
            if (!SetWindowPos(child_, nullptr, 0, 0, static_cast<int>(width),
                              static_cast<int>(height),
                              SWP_NOZORDER | SWP_NOACTIVATE)) {
              fatal_ = true;
              PostMessageW(top_, WM_CLOSE, 0, 0);
            } else {
              if (PublishMetrics()) QueueRender();
            }
          }
        }
        return 0;
      case WM_CLOSE:
        StopRenderWorker();
        DestroyWindow(top_);
        return 0;
      case WM_DESTROY:
        top_ = nullptr;
        PostQuitMessage(fatal_ ? 4 : 0);
        return 0;
      case WM_TIMER:
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
      default:
        return DefWindowProcW(window, message, wparam, lparam);
    }
  }

  LRESULT HandleChild(HWND window, UINT message, WPARAM wparam, LPARAM lparam) {
    switch (message) {
      case WM_ERASEBKGND:
        return 1;
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

  template <typename T>
  static bool callbacks_missing(T callback) noexcept { return callback == nullptr; }

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
    child_ = CreateWindowExW(0, kChildClass, L"", WS_CHILD | WS_VISIBLE,
                             0, 0, 1, 1, top_, nullptr, instance, this);
    task_ = CreateWindowExW(0, kTaskClass, L"", 0, 0, 0, 0, 0, HWND_MESSAGE,
                            nullptr, instance, this);
    if (child_ == nullptr || task_ == nullptr) throw std::bad_alloc();
    RECT client{};
    if (!GetClientRect(top_, &client) ||
        !SetWindowPos(child_, nullptr, 0, 0, client.right, client.bottom,
                      SWP_NOZORDER | SWP_NOACTIVATE))
      throw std::bad_alloc();
  }

  void ConnectAppWindow() {
    const auto id = winrt::Microsoft::UI::GetWindowIdFromWindow(top_);
    app_window_ = winrt::Microsoft::UI::Windowing::AppWindow::GetFromWindowId(id);
    if (!app_window_) throw std::bad_alloc();
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
    current_generation_++;
    current_metrics_qpc_ = QpcNow();
    return true;
  }

  void QueueRender() {
    if (current_generation_ == 0 || current_width_ == 0 || current_height_ == 0)
      return;
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
      if (render_stopping_) return;
      render_pending_ = work;
    }
    render_condition_.notify_one();
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
        render_completions_.push_back(MakeTerminal(work, terminal, error));
      }
      PostMessageW(task_, kRenderCompleted, 0, 0);
    }
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
    render_thread_.join();
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
  std::mutex render_mutex_;
  std::condition_variable render_condition_;
  std::optional<RenderWork> render_pending_;
  std::deque<doroti_windows_frame_terminal_v1> render_completions_;
  std::thread render_thread_;
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
