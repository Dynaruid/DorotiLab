#include "managed_presenter_probe.h"

#include <windows.h>

#include <atomic>
#include <cstddef>
#include <thread>

namespace {

constexpr wchar_t kTopClass[] = L"Doroti.ManagedPresenterProbe.Top.v1";
constexpr wchar_t kChildClass[] = L"Doroti.ManagedPresenterProbe.Child.v1";
constexpr wchar_t kTaskClass[] = L"Doroti.ManagedPresenterProbe.Task.v1";
constexpr UINT kResizeMessage = WM_APP + 0x311;
constexpr UINT kWorkerCompletedMessage = WM_APP + 0x312;
constexpr UINT kBoundedCompletionMessage = WM_APP + 0x313;
constexpr UINT kRecursiveProbeMessage = WM_APP + 0x314;

struct ResizeCommand {
  uint32_t width;
  uint32_t height;
  uint64_t generation;
  HANDLE completed;
  uint32_t status;
};

class Probe;

LRESULT CALLBACK TopProcedure(HWND window, UINT message, WPARAM wparam,
                              LPARAM lparam) {
  return DefWindowProcW(window, message, wparam, lparam);
}

LRESULT CALLBACK TaskProcedure(HWND window, UINT message, WPARAM wparam,
                               LPARAM lparam);

class Probe final {
 public:
  explicit Probe(doroti_managed_presenter_probe_result_v1& result)
      : result_(result), platform_thread_id_(GetCurrentThreadId()) {
    result_.platform_thread_id = platform_thread_id_;
  }

  ~Probe() { DestroyWindows(); }

  uint32_t Run(const doroti_managed_presenter_callbacks_v1& callbacks) {
    CreateWindows();
    doroti_managed_presenter_host_v1 host{
        DOROTI_MANAGED_PRESENTER_PROBE_ABI_V1,
        sizeof(doroti_managed_presenter_host_v1),
        this,
        top_,
        child_,
        task_,
        &RequestResize,
    };

    std::thread presenter([&]() {
      result_.presenter_thread_id = GetCurrentThreadId();
      result_.callback_status =
          callbacks.run_presenter(callbacks.callback_context, &host);
      PostMessageW(task_, kWorkerCompletedMessage, 0, 0);
    });

    MSG message{};
    while (!worker_completed_) {
      const auto status = GetMessageW(&message, nullptr, 0, 0);
      if (status <= 0) {
        result_.callback_status = 0xFFFFFFFFu;
        break;
      }
      TranslateMessage(&message);
      DispatchMessageW(&message);
    }
    presenter.join();
    return result_.callback_status == 0 ? 0u : 4u;
  }

  LRESULT HandleTask(UINT message, LPARAM lparam) {
    if (message == kWorkerCompletedMessage) {
      worker_completed_ = true;
      return 0;
    }
    if (message != kResizeMessage) {
      return DefWindowProcW(task_, message, 0, lparam);
    }
    ++result_.task_dispatch_count;
    auto* command = reinterpret_cast<ResizeCommand*>(lparam);
    if (command == nullptr) {
      return 0;
    }
    command->status = 0;
    if (!SetWindowPos(child_, nullptr, 0, 0,
                      static_cast<int>(command->width),
                      static_cast<int>(command->height),
                      SWP_NOZORDER | SWP_NOACTIVATE)) {
      command->status = 4;
    } else {
      RECT client{};
      if (!GetClientRect(child_, &client) ||
          static_cast<uint32_t>(client.right - client.left) != command->width ||
          static_cast<uint32_t>(client.bottom - client.top) != command->height) {
        ++result_.child_extent_mismatch_count;
        command->status = 4;
      }
    }
    ++result_.resize_command_count;
    SetEvent(command->completed);
    return 0;
  }

 private:
  static uint32_t DOROTI_MANAGED_PRESENTER_PROBE_CALL RequestResize(
      void* host_context, uint32_t width, uint32_t height,
      uint64_t generation) {
    auto* probe = static_cast<Probe*>(host_context);
    if (probe == nullptr || width == 0 || height == 0 ||
        GetCurrentThreadId() == probe->platform_thread_id_) {
      return 1;
    }
    ResizeCommand command{width, height, generation,
                          CreateEventW(nullptr, FALSE, FALSE, nullptr), 4};
    if (command.completed == nullptr) {
      return 4;
    }
    if (!PostMessageW(probe->task_, kResizeMessage, 0,
                      reinterpret_cast<LPARAM>(&command))) {
      CloseHandle(command.completed);
      return 4;
    }
    const auto wait = WaitForSingleObject(command.completed, 5000);
    CloseHandle(command.completed);
    return wait == WAIT_OBJECT_0 ? command.status : 4u;
  }

  void CreateWindows() {
    const auto instance = GetModuleHandleW(nullptr);
    const struct ClassEntry {
      const wchar_t* name;
      WNDPROC procedure;
    } classes[]{{kTopClass, TopProcedure}, {kChildClass, TopProcedure},
                {kTaskClass, TaskProcedure}};
    for (const auto& entry : classes) {
      WNDCLASSEXW description{};
      description.cbSize = sizeof(description);
      description.lpfnWndProc = entry.procedure;
      description.hInstance = instance;
      description.lpszClassName = entry.name;
      if (!RegisterClassExW(&description) &&
          GetLastError() != ERROR_CLASS_ALREADY_EXISTS) {
        throw 4u;
      }
    }
    top_ = CreateWindowExW(0, kTopClass, L"", WS_OVERLAPPEDWINDOW, 0, 0,
                           640, 480, nullptr, nullptr, instance, nullptr);
    child_ = CreateWindowExW(0, kChildClass, L"", WS_CHILD | WS_VISIBLE, 0, 0,
                             640, 480, top_, nullptr, instance, nullptr);
    task_ = CreateWindowExW(0, kTaskClass, L"", 0, 0, 0, 0, 0, HWND_MESSAGE,
                            nullptr, instance, this);
    if (!top_ || !child_ || !task_) {
      throw 4u;
    }
    ++result_.top_level_created_count;
    ++result_.child_created_count;
    ++result_.task_window_created_count;
  }

  void DestroyWindows() {
    if (task_) DestroyWindow(task_);
    if (child_) DestroyWindow(child_);
    if (top_) DestroyWindow(top_);
    task_ = child_ = top_ = nullptr;
  }

  doroti_managed_presenter_probe_result_v1& result_;
  DWORD platform_thread_id_{};
  HWND top_{};
  HWND child_{};
  HWND task_{};
  bool worker_completed_{};
};

LRESULT CALLBACK TaskProcedure(HWND window, UINT message, WPARAM wparam,
                               LPARAM lparam) {
  if (message == WM_NCCREATE) {
    const auto* create = reinterpret_cast<CREATESTRUCTW*>(lparam);
    SetWindowLongPtrW(window, GWLP_USERDATA,
                      reinterpret_cast<LONG_PTR>(create->lpCreateParams));
    return DefWindowProcW(window, message, wparam, lparam);
  }
  auto* probe = reinterpret_cast<Probe*>(
      GetWindowLongPtrW(window, GWLP_USERDATA));
  return probe ? probe->HandleTask(message, lparam)
               : DefWindowProcW(window, message, wparam, lparam);
}

template <typename T>
bool Valid(const T* value) {
  return value != nullptr &&
         value->abi_version == DOROTI_MANAGED_PRESENTER_PROBE_ABI_V1 &&
         value->struct_size >= sizeof(T);
}

struct FilteredWaitState {
  uint64_t task_completion_dispatch_count{};
  uint64_t top_level_recursive_dispatch_count{};
  uint64_t child_recursive_dispatch_count{};
};

LRESULT CALLBACK FilteredTopProcedure(HWND window, UINT message, WPARAM wparam,
                                      LPARAM lparam) {
  if (message == WM_NCCREATE) {
    const auto* create = reinterpret_cast<CREATESTRUCTW*>(lparam);
    SetWindowLongPtrW(window, GWLP_USERDATA,
                      reinterpret_cast<LONG_PTR>(create->lpCreateParams));
  }
  auto* state = reinterpret_cast<FilteredWaitState*>(
      GetWindowLongPtrW(window, GWLP_USERDATA));
  if (message == kRecursiveProbeMessage && state != nullptr) {
    ++state->top_level_recursive_dispatch_count;
    return 0;
  }
  return DefWindowProcW(window, message, wparam, lparam);
}

LRESULT CALLBACK FilteredChildProcedure(HWND window, UINT message,
                                        WPARAM wparam, LPARAM lparam) {
  if (message == WM_NCCREATE) {
    const auto* create = reinterpret_cast<CREATESTRUCTW*>(lparam);
    SetWindowLongPtrW(window, GWLP_USERDATA,
                      reinterpret_cast<LONG_PTR>(create->lpCreateParams));
  }
  auto* state = reinterpret_cast<FilteredWaitState*>(
      GetWindowLongPtrW(window, GWLP_USERDATA));
  if (message == kRecursiveProbeMessage && state != nullptr) {
    ++state->child_recursive_dispatch_count;
    return 0;
  }
  return DefWindowProcW(window, message, wparam, lparam);
}

LRESULT CALLBACK FilteredTaskProcedure(HWND window, UINT message,
                                       WPARAM wparam, LPARAM lparam) {
  if (message == WM_NCCREATE) {
    const auto* create = reinterpret_cast<CREATESTRUCTW*>(lparam);
    SetWindowLongPtrW(window, GWLP_USERDATA,
                      reinterpret_cast<LONG_PTR>(create->lpCreateParams));
  }
  auto* state = reinterpret_cast<FilteredWaitState*>(
      GetWindowLongPtrW(window, GWLP_USERDATA));
  if (message == kBoundedCompletionMessage && state != nullptr) {
    ++state->task_completion_dispatch_count;
    return 0;
  }
  return DefWindowProcW(window, message, wparam, lparam);
}

bool RunFilteredWait(HWND task, DWORD deadline_ms, uint64_t& elapsed_ms) {
  const auto started = GetTickCount64();
  for (;;) {
    MSG message{};
    if (PeekMessageW(&message, task, kBoundedCompletionMessage,
                     kBoundedCompletionMessage, PM_REMOVE)) {
      DispatchMessageW(&message);
      elapsed_ms = GetTickCount64() - started;
      return true;
    }
    const auto elapsed = GetTickCount64() - started;
    if (elapsed >= deadline_ms) {
      elapsed_ms = elapsed;
      return false;
    }
    const auto remaining = static_cast<DWORD>(deadline_ms - elapsed);
    MsgWaitForMultipleObjectsEx(0, nullptr, remaining, QS_POSTMESSAGE,
                                MWMO_INPUTAVAILABLE);
  }
}

}  // namespace

extern "C" uint32_t DOROTI_MANAGED_PRESENTER_PROBE_CALL
doroti_run_managed_presenter_probe_v1(
    const doroti_managed_presenter_callbacks_v1* callbacks,
    doroti_managed_presenter_probe_result_v1* result) {
  if (!Valid(callbacks) || !Valid(result) ||
      callbacks->run_presenter == nullptr) {
    return 1;
  }
  *result = {DOROTI_MANAGED_PRESENTER_PROBE_ABI_V1,
             sizeof(doroti_managed_presenter_probe_result_v1)};
  result->gdi_start = GetGuiResources(GetCurrentProcess(), GR_GDIOBJECTS);
  result->user_start = GetGuiResources(GetCurrentProcess(), GR_USEROBJECTS);
  try {
    Probe probe(*result);
    result->status = probe.Run(*callbacks);
  } catch (...) {
    result->status = 4;
  }
  result->gdi_end = GetGuiResources(GetCurrentProcess(), GR_GDIOBJECTS);
  result->user_end = GetGuiResources(GetCurrentProcess(), GR_USEROBJECTS);
  const bool passed = result->status == 0 &&
                      result->platform_thread_id != 0 &&
                      result->presenter_thread_id != 0 &&
                      result->platform_thread_id != result->presenter_thread_id &&
                      result->top_level_created_count == 1 &&
                      result->child_created_count == 1 &&
                      result->task_window_created_count == 1 &&
                      result->resize_command_count == 10 &&
                      result->task_dispatch_count == 10 &&
                      result->child_extent_mismatch_count == 0 &&
                      result->gdi_start == result->gdi_end &&
                      result->user_start == result->user_end;
  result->status = passed ? 0u : 4u;
  return result->status;
}

extern "C" uint32_t DOROTI_MANAGED_PRESENTER_PROBE_CALL
doroti_run_filtered_wait_probe_v1(
    doroti_filtered_wait_probe_result_v1* result) {
  if (!Valid(result)) {
    return 1;
  }
  *result = {DOROTI_MANAGED_PRESENTER_PROBE_ABI_V1,
             sizeof(doroti_filtered_wait_probe_result_v1)};
  result->gdi_start = GetGuiResources(GetCurrentProcess(), GR_GDIOBJECTS);
  result->user_start = GetGuiResources(GetCurrentProcess(), GR_USEROBJECTS);

  constexpr wchar_t top_class[] = L"Doroti.FilteredWait.Top.v1";
  constexpr wchar_t child_class[] = L"Doroti.FilteredWait.Child.v1";
  constexpr wchar_t task_class[] = L"Doroti.FilteredWait.Task.v1";
  const auto instance = GetModuleHandleW(nullptr);
  const struct ClassEntry {
    const wchar_t* name;
    WNDPROC procedure;
  } classes[]{{top_class, FilteredTopProcedure},
              {child_class, FilteredChildProcedure},
              {task_class, FilteredTaskProcedure}};
  for (const auto& entry : classes) {
    WNDCLASSEXW description{};
    description.cbSize = sizeof(description);
    description.lpfnWndProc = entry.procedure;
    description.hInstance = instance;
    description.lpszClassName = entry.name;
    if (!RegisterClassExW(&description) &&
        GetLastError() != ERROR_CLASS_ALREADY_EXISTS) {
      result->status = 4;
      return result->status;
    }
  }

  FilteredWaitState state{};
  HWND top = CreateWindowExW(0, top_class, L"", WS_OVERLAPPEDWINDOW, 0, 0,
                             640, 480, nullptr, nullptr, instance, &state);
  HWND child = CreateWindowExW(0, child_class, L"", WS_CHILD, 0, 0, 640, 480,
                               top, nullptr, instance, &state);
  HWND task = CreateWindowExW(0, task_class, L"", 0, 0, 0, 0, 0, HWND_MESSAGE,
                              nullptr, instance, &state);
  if (top == nullptr || child == nullptr || task == nullptr) {
    if (task) DestroyWindow(task);
    if (child) DestroyWindow(child);
    if (top) DestroyWindow(top);
    result->status = 4;
    return result->status;
  }

  PostMessageW(top, kRecursiveProbeMessage, 0, 0);
  PostMessageW(child, kRecursiveProbeMessage, 0, 0);
  std::thread completion([task]() {
    Sleep(10);
    PostMessageW(task, kBoundedCompletionMessage, 0, 0);
  });
  uint64_t success_elapsed{};
  if (RunFilteredWait(task, 100, success_elapsed)) {
    ++result->successful_wait_count;
  }
  completion.join();
  uint64_t timeout_elapsed{};
  if (!RunFilteredWait(task, 100, timeout_elapsed)) {
    ++result->timeout_wait_count;
  }
  result->maximum_wait_elapsed_ms =
      success_elapsed > timeout_elapsed ? success_elapsed : timeout_elapsed;
  result->task_completion_dispatch_count =
      state.task_completion_dispatch_count;
  result->top_level_recursive_dispatch_count =
      state.top_level_recursive_dispatch_count;
  result->child_recursive_dispatch_count = state.child_recursive_dispatch_count;

  MSG discarded{};
  PeekMessageW(&discarded, top, kRecursiveProbeMessage,
               kRecursiveProbeMessage, PM_REMOVE);
  PeekMessageW(&discarded, child, kRecursiveProbeMessage,
               kRecursiveProbeMessage, PM_REMOVE);
  DestroyWindow(task);
  DestroyWindow(child);
  DestroyWindow(top);
  result->gdi_end = GetGuiResources(GetCurrentProcess(), GR_GDIOBJECTS);
  result->user_end = GetGuiResources(GetCurrentProcess(), GR_USEROBJECTS);
  const bool passed = result->successful_wait_count == 1 &&
                      result->timeout_wait_count == 1 &&
                      result->task_completion_dispatch_count == 1 &&
                      result->top_level_recursive_dispatch_count == 0 &&
                      result->child_recursive_dispatch_count == 0 &&
                      result->maximum_wait_elapsed_ms >= 100 &&
                      result->maximum_wait_elapsed_ms <= 250 &&
                      result->gdi_start == result->gdi_end &&
                      result->user_start == result->user_end;
  result->status = passed ? 0u : 4u;
  return result->status;
}
