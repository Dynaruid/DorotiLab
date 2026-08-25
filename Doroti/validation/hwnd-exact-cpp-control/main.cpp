#include "D3D12Presenter.h"

#include <windows.h>

#include <MddBootstrap.h>
#include <WindowsAppSDK-VersionInfo.h>
#include <dxgidebug.h>
#include <winrt/Microsoft.UI.Interop.h>
#include <winrt/Microsoft.UI.Windowing.h>
#include <winrt/base.h>
#include <wrl/client.h>

#include <cstdint>
#include <filesystem>
#include <fstream>
#include <iostream>
#include <stdexcept>
#include <string>

using doroti::validation::D3D12Presenter;
using doroti::validation::PresenterDiagnostics;
using Microsoft::WRL::ComPtr;

namespace {

constexpr wchar_t kTopClass[] = L"Doroti.HwndExactCpp.Control.Top.v1";
constexpr wchar_t kChildClass[] = L"Doroti.HwndExactCpp.Control.Child.v1";
constexpr wchar_t kTaskClass[] = L"Doroti.HwndExactCpp.Control.Task.v1";

struct Diagnostics {
  uint64_t warmup_cycles{};
  uint64_t requested_cycles{};
  uint64_t completed_cycles{};
  uint64_t top_level_created{};
  uint64_t child_created{};
  uint64_t task_created{};
  uint64_t app_window_connected{};
  uint64_t minimized{};
  uint64_t restored{};
  uint64_t accepted_generations{};
  uint64_t terminal_generations{};
  uint64_t stale_presents{};
  uint64_t unaccounted_generations{};
  uint64_t topology_failures{};
  PresenterDiagnostics presenter;
  DWORD gdi_start{};
  DWORD gdi_end{};
  DWORD user_start{};
  DWORD user_end{};
  HRESULT live_object_report_result{E_NOTIMPL};
};

struct Options {
  uint32_t cycles{10};
  std::filesystem::path report;
  bool inject_device_loss{true};
};

void Check(HRESULT result, const char* operation) {
  if (FAILED(result)) {
    throw std::runtime_error(operation);
  }
}

void CheckWin32(BOOL result, const char* operation) {
  if (!result) {
    throw std::runtime_error(operation);
  }
}

Options ParseOptions(int argc, wchar_t** argv) {
  Options options;
  for (int index = 1; index < argc; ++index) {
    const std::wstring argument = argv[index];
    if (argument == L"--cycles" && index + 1 < argc) {
      options.cycles = static_cast<uint32_t>(std::stoul(argv[++index]));
    } else if (argument == L"--report" && index + 1 < argc) {
      options.report = argv[++index];
    } else if (argument == L"--no-device-loss") {
      options.inject_device_loss = false;
    } else {
      throw std::invalid_argument("Unknown or incomplete control argument");
    }
  }
  if (options.cycles == 0 || options.cycles > 10) {
    throw std::invalid_argument("--cycles must be between 1 and 10");
  }
  if (options.report.empty()) {
    throw std::invalid_argument("--report is required");
  }
  return options;
}

LRESULT CALLBACK WindowProcedure(HWND window, UINT message, WPARAM wparam,
                                 LPARAM lparam) {
  if (message == WM_ERASEBKGND) {
    return 1;
  }
  return DefWindowProcW(window, message, wparam, lparam);
}

void RegisterClasses(HINSTANCE instance) {
  for (const auto* name : {kTopClass, kChildClass, kTaskClass}) {
    WNDCLASSEXW description{};
    description.cbSize = sizeof(description);
    description.lpfnWndProc = WindowProcedure;
    description.hInstance = instance;
    description.hCursor = LoadCursorW(nullptr, IDC_ARROW);
    description.lpszClassName = name;
    if (!RegisterClassExW(&description) && GetLastError() != ERROR_CLASS_ALREADY_EXISTS) {
      throw std::runtime_error("RegisterClassExW failed");
    }
  }
}

void PumpMessages() {
  MSG message{};
  while (PeekMessageW(&message, nullptr, 0, 0, PM_REMOVE)) {
    TranslateMessage(&message);
    DispatchMessageW(&message);
  }
}

void ResizeChildToClient(HWND top, HWND child) {
  RECT client{};
  CheckWin32(GetClientRect(top, &client), "GetClientRect(top) failed");
  CheckWin32(SetWindowPos(child, nullptr, 0, 0, client.right - client.left,
                          client.bottom - client.top,
                          SWP_NOZORDER | SWP_NOACTIVATE),
             "SetWindowPos(child) failed");
}

std::pair<uint32_t, uint32_t> ClientExtent(HWND window) {
  RECT client{};
  CheckWin32(GetClientRect(window, &client), "GetClientRect failed");
  return {static_cast<uint32_t>(client.right - client.left),
          static_cast<uint32_t>(client.bottom - client.top)};
}

void ValidateTopology(HWND top, HWND child, HWND task, Diagnostics& diagnostics) {
  auto fail = [&diagnostics](const char* message) {
    ++diagnostics.topology_failures;
    throw std::runtime_error(message);
  };
  if (!IsWindow(top) || !IsWindow(child) || !IsWindow(task)) {
    fail("Native control HWND creation validation failed");
  }
  if (GetParent(child) != top || GetAncestor(child, GA_ROOT) != top) {
    fail("Native control child-parent validation failed");
  }
  const auto top_style = static_cast<DWORD>(GetWindowLongPtrW(top, GWL_STYLE));
  const auto child_style = static_cast<DWORD>(GetWindowLongPtrW(child, GWL_STYLE));
  if ((top_style & WS_OVERLAPPEDWINDOW) != WS_OVERLAPPEDWINDOW) {
    fail("Native control top-level style validation failed");
  }
  if ((child_style & (WS_CHILD | WS_VISIBLE)) !=
      (WS_CHILD | WS_VISIBLE)) {
    fail("Native control child style validation failed");
  }
  if (FindWindowExW(HWND_MESSAGE, nullptr, kTaskClass, nullptr) != task) {
    fail("Native control task-window parent validation failed");
  }
}

winrt::Microsoft::UI::Windowing::AppWindow ConnectAppWindow(HWND top) {
  const auto window_id = winrt::Microsoft::UI::GetWindowIdFromWindow(top);
  auto app_window =
      winrt::Microsoft::UI::Windowing::AppWindow::GetFromWindowId(window_id);
  if (!app_window) {
    throw std::runtime_error("AppWindow::GetFromWindowId returned null");
  }
  app_window.Title(L"Doroti HwndExactCpp native control");
  return app_window;
}

void AccountPresent(D3D12Presenter& presenter, HWND child, uint64_t generation,
                    Diagnostics& diagnostics) {
  const auto [width, height] = ClientExtent(child);
  if (width == 0 || height == 0) {
    throw std::runtime_error("A present generation cannot have zero extent");
  }
  ++diagnostics.accepted_generations;
  presenter.ResizeAndPresent(width, height, generation);
  if (presenter.width() != width || presenter.height() != height ||
      presenter.generation() != generation) {
    ++diagnostics.presenter.wrong_size_present_count;
    throw std::runtime_error("Presenter did not retain exact generation identity");
  }
  ++diagnostics.terminal_generations;
}

void RunCycle(uint32_t cycle, uint32_t total_cycles, bool inject_device_loss,
              Diagnostics& diagnostics) {
  const auto instance = GetModuleHandleW(nullptr);
  HWND top = CreateWindowExW(0, kTopClass, L"Doroti native control",
                             WS_OVERLAPPEDWINDOW, 80, 80, 640, 480, nullptr,
                             nullptr, instance, nullptr);
  if (!top) {
    throw std::runtime_error("CreateWindowExW(top) failed");
  }
  ++diagnostics.top_level_created;

  HWND child = CreateWindowExW(0, kChildClass, L"", WS_CHILD | WS_VISIBLE, 0,
                               0, 1, 1, top, nullptr, instance, nullptr);
  if (!child) {
    DestroyWindow(top);
    throw std::runtime_error("CreateWindowExW(child) failed");
  }
  ++diagnostics.child_created;
  HWND task = CreateWindowExW(0, kTaskClass, L"", 0, 0, 0, 0, 0,
                              HWND_MESSAGE, nullptr, instance, nullptr);
  if (!task) {
    DestroyWindow(child);
    DestroyWindow(top);
    throw std::runtime_error("CreateWindowExW(task) failed");
  }
  ++diagnostics.task_created;

  try {
    ValidateTopology(top, child, task, diagnostics);
    auto app_window = ConnectAppWindow(top);
    ++diagnostics.app_window_connected;
    ResizeChildToClient(top, child);

    D3D12Presenter presenter(child, diagnostics.presenter);
    uint64_t generation = static_cast<uint64_t>(cycle) * 10;
    AccountPresent(presenter, child, ++generation, diagnostics);
    ShowWindow(top, SW_SHOWNOACTIVATE);
    PumpMessages();

    const int width = 680 + static_cast<int>((cycle % 7) * 13);
    const int height = 500 + static_cast<int>((cycle % 5) * 17);
    CheckWin32(SetWindowPos(top, nullptr, 80, 80, width, height,
                            SWP_NOZORDER | SWP_NOACTIVATE),
               "SetWindowPos(top resize) failed");
    ResizeChildToClient(top, child);
    AccountPresent(presenter, child, ++generation, diagnostics);

    ShowWindow(top, SW_MINIMIZE);
    PumpMessages();
    ++diagnostics.minimized;
    ShowWindow(top, SW_RESTORE);
    PumpMessages();
    ResizeChildToClient(top, child);
    ++diagnostics.restored;
    AccountPresent(presenter, child, ++generation, diagnostics);

    if (inject_device_loss && cycle + 1 == total_cycles) {
      presenter.InjectDeviceLoss();
    }
    presenter.Shutdown();
    app_window = nullptr;
    CheckWin32(DestroyWindow(task), "DestroyWindow(task) failed");
    task = nullptr;
    CheckWin32(DestroyWindow(child), "DestroyWindow(child) failed");
    child = nullptr;
    CheckWin32(DestroyWindow(top), "DestroyWindow(top) failed");
    top = nullptr;
    ++diagnostics.completed_cycles;
  } catch (...) {
    if (task) {
      DestroyWindow(task);
    }
    if (child) {
      DestroyWindow(child);
    }
    if (top) {
      DestroyWindow(top);
    }
    throw;
  }
}

void WriteReport(const Options& options, Diagnostics& diagnostics,
                 const char* status, const std::string& error) {
  diagnostics.unaccounted_generations =
      diagnostics.accepted_generations - diagnostics.terminal_generations;
  std::ofstream output(options.report, std::ios::binary | std::ios::trunc);
  if (!output) {
    throw std::runtime_error("Could not create the C2 report");
  }
  output << "{\n"
         << "  \"schemaVersion\": \"doroti.windows.hwnd-exact-cpp-control/v1\",\n"
         << "  \"gate\": \"C2\",\n"
         << "  \"status\": \"" << status << "\",\n"
         << "  \"warmupCycles\": " << diagnostics.warmup_cycles << ",\n"
         << "  \"cyclesRequested\": " << diagnostics.requested_cycles << ",\n"
         << "  \"cyclesCompleted\": " << diagnostics.completed_cycles << ",\n"
         << "  \"topLevelCreated\": " << diagnostics.top_level_created << ",\n"
         << "  \"childCreated\": " << diagnostics.child_created << ",\n"
         << "  \"taskWindowCreated\": " << diagnostics.task_created << ",\n"
         << "  \"appWindowConnected\": " << diagnostics.app_window_connected << ",\n"
         << "  \"minimizeCount\": " << diagnostics.minimized << ",\n"
         << "  \"restoreCount\": " << diagnostics.restored << ",\n"
         << "  \"acceptedGenerations\": " << diagnostics.accepted_generations << ",\n"
         << "  \"terminalGenerations\": " << diagnostics.terminal_generations << ",\n"
         << "  \"unaccountedGenerations\": " << diagnostics.unaccounted_generations << ",\n"
         << "  \"presentCount\": " << diagnostics.presenter.present_count << ",\n"
         << "  \"resizeBuffersCount\": " << diagnostics.presenter.resize_count << ",\n"
         << "  \"wrongSizePresentCount\": " << diagnostics.presenter.wrong_size_present_count << ",\n"
         << "  \"stalePresentCount\": " << diagnostics.stale_presents << ",\n"
         << "  \"topologyFailureCount\": " << diagnostics.topology_failures << ",\n"
         << "  \"d3d12DebugErrorCount\": " << diagnostics.presenter.debug_error_count << ",\n"
         << "  \"d3d12DebugCorruptionCount\": " << diagnostics.presenter.debug_corruption_count << ",\n"
         << "  \"deviceLossInjections\": " << diagnostics.presenter.device_loss_injections << ",\n"
         << "  \"deviceLossObserved\": " << diagnostics.presenter.device_loss_observed << ",\n"
         << "  \"gdiStart\": " << diagnostics.gdi_start << ",\n"
         << "  \"gdiEnd\": " << diagnostics.gdi_end << ",\n"
         << "  \"userStart\": " << diagnostics.user_start << ",\n"
         << "  \"userEnd\": " << diagnostics.user_end << ",\n"
         << "  \"dxgiLiveObjectReportHresult\": "
         << diagnostics.live_object_report_result << ",\n"
         << "  \"error\": \"" << error << "\"\n"
         << "}\n";
}

}  // namespace

int wmain(int argc, wchar_t** argv) {
  Options options;
  Diagnostics diagnostics;
  bool bootstrap_initialized = false;
  try {
    options = ParseOptions(argc, argv);
    diagnostics.requested_cycles = options.cycles;
    PACKAGE_VERSION minimum_version{};
    minimum_version.Version = WINDOWSAPPSDK_RUNTIME_VERSION_UINT64;
    Check(MddBootstrapInitialize2(WINDOWSAPPSDK_RELEASE_MAJORMINOR,
                                  WINDOWSAPPSDK_RELEASE_VERSION_TAG_W,
                                  minimum_version,
                                  MddBootstrapInitializeOptions_None),
          "Windows App SDK 2.4 bootstrap failed");
    bootstrap_initialized = true;
    winrt::init_apartment(winrt::apartment_type::single_threaded);
    RegisterClasses(GetModuleHandleW(nullptr));
    Diagnostics warmup;
    RunCycle(0, 1, false, warmup);
    if (warmup.presenter.debug_error_count != 0 ||
        warmup.presenter.debug_corruption_count != 0) {
      throw std::runtime_error("C2 warmup emitted a D3D12 debug error");
    }
    PumpMessages();
    diagnostics.warmup_cycles = warmup.completed_cycles;
    const auto process = GetCurrentProcess();
    diagnostics.gdi_start = GetGuiResources(process, GR_GDIOBJECTS);
    diagnostics.user_start = GetGuiResources(process, GR_USEROBJECTS);

    for (uint32_t cycle = 0; cycle < options.cycles; ++cycle) {
      RunCycle(cycle, options.cycles, options.inject_device_loss, diagnostics);
    }

    diagnostics.gdi_end = GetGuiResources(process, GR_GDIOBJECTS);
    diagnostics.user_end = GetGuiResources(process, GR_USEROBJECTS);
    ComPtr<IDXGIDebug1> dxgi_debug;
    diagnostics.live_object_report_result =
        DXGIGetDebugInterface1(0, IID_PPV_ARGS(&dxgi_debug));
    if (SUCCEEDED(diagnostics.live_object_report_result)) {
      diagnostics.live_object_report_result = dxgi_debug->ReportLiveObjects(
          DXGI_DEBUG_ALL,
          static_cast<DXGI_DEBUG_RLO_FLAGS>(DXGI_DEBUG_RLO_SUMMARY |
                                            DXGI_DEBUG_RLO_IGNORE_INTERNAL));
    }

    const bool passed = diagnostics.completed_cycles == options.cycles &&
                        diagnostics.top_level_created == options.cycles &&
                        diagnostics.child_created == options.cycles &&
                        diagnostics.task_created == options.cycles &&
                        diagnostics.app_window_connected == options.cycles &&
                        diagnostics.minimized == options.cycles &&
                        diagnostics.restored == options.cycles &&
                        diagnostics.accepted_generations ==
                            diagnostics.terminal_generations &&
                        diagnostics.presenter.wrong_size_present_count == 0 &&
                        diagnostics.stale_presents == 0 &&
                        diagnostics.topology_failures == 0 &&
                        diagnostics.presenter.debug_error_count == 0 &&
                        diagnostics.presenter.debug_corruption_count == 0 &&
                        (!options.inject_device_loss ||
                         (diagnostics.presenter.device_loss_injections == 1 &&
                          diagnostics.presenter.device_loss_observed == 1)) &&
                        diagnostics.gdi_start == diagnostics.gdi_end &&
                        diagnostics.user_start == diagnostics.user_end &&
                        SUCCEEDED(diagnostics.live_object_report_result);
    WriteReport(options, diagnostics, passed ? "PASS" : "FAIL", "");
    winrt::uninit_apartment();
    MddBootstrapShutdown();
    return passed ? 0 : 2;
  } catch (const std::exception& exception) {
    try {
      if (!options.report.empty()) {
        WriteReport(options, diagnostics, "FAIL", exception.what());
      }
    } catch (...) {
    }
    if (bootstrap_initialized) {
      MddBootstrapShutdown();
    }
    std::cerr << exception.what() << std::endl;
    return 1;
  }
}
