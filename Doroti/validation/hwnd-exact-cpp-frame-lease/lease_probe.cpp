#include "lease_probe.h"

#include <windows.h>

#include <d3d12.h>
#include <dwmapi.h>
#include <dxgi1_6.h>
#include <dxgidebug.h>
#include <wrl/client.h>

#include <array>
#include <cstddef>
#include <cstdio>
#include <stdexcept>
#include <vector>

using Microsoft::WRL::ComPtr;

namespace {

constexpr wchar_t kTopClass[] = L"Doroti.HwndExactCpp.LeaseProbe.Top.v1";
constexpr wchar_t kChildClass[] = L"Doroti.HwndExactCpp.LeaseProbe.Child.v1";

void Check(HRESULT result, const char* operation) {
  if (FAILED(result)) {
    throw std::runtime_error(operation);
  }
}

template <typename T>
bool ValidHeader(const T* value) {
  return value != nullptr && value->abi_version == DOROTI_WINDOWS_ABI_VERSION_V1 &&
         value->struct_size >= sizeof(T);
}

D3D12_HEAP_PROPERTIES DefaultHeap() {
  D3D12_HEAP_PROPERTIES properties{};
  properties.Type = D3D12_HEAP_TYPE_DEFAULT;
  properties.CreationNodeMask = 1;
  properties.VisibleNodeMask = 1;
  return properties;
}

D3D12_RESOURCE_BARRIER Transition(ID3D12Resource* resource,
                                  D3D12_RESOURCE_STATES before,
                                  D3D12_RESOURCE_STATES after) {
  D3D12_RESOURCE_BARRIER barrier{};
  barrier.Type = D3D12_RESOURCE_BARRIER_TYPE_TRANSITION;
  barrier.Transition.pResource = resource;
  barrier.Transition.StateBefore = before;
  barrier.Transition.StateAfter = after;
  barrier.Transition.Subresource = D3D12_RESOURCE_BARRIER_ALL_SUBRESOURCES;
  return barrier;
}

LRESULT CALLBACK WindowProcedure(HWND window, UINT message, WPARAM wparam,
                                 LPARAM lparam) {
  return DefWindowProcW(window, message, wparam, lparam);
}

class Probe final {
 public:
  Probe(const doroti_windows_callbacks_v1& callbacks,
        doroti_windows_lease_probe_result_v1& result)
      : callbacks_(callbacks), result_(result) {
    CreateWindows();
    CreateDevice();
  }

  ~Probe() {
    try {
      ReleaseContext();
      ReleaseNative();
    } catch (...) {
    }
  }

  void Run() {
    AcquireContext();
    constexpr std::array<std::pair<uint32_t, uint32_t>, 10> sizes{{
        {640, 480}, {720, 500}, {680, 520}, {800, 540}, {700, 490},
        {760, 560}, {660, 510}, {820, 580}, {690, 530}, {740, 550},
    }};
    ResizeChild(640, 480);
    CreateOrResizeSwapChain(640, 480);
    uint64_t generation = 0;
    for (size_t index = 0; index < sizes.size(); ++index) {
      if (index == 5) {
        ReleaseContext();
        ++context_generation_;
        AcquireContext();
      }
      ResizeChild(sizes[index].first, sizes[index].second);
      CreateOrResizeSwapChain(sizes[index].first, sizes[index].second);
      auto resource = CreateFrameResource(sizes[index].first, sizes[index].second);
      InvokeRender(*resource.Get(), sizes[index].first, sizes[index].second,
                   ++generation, generation, DOROTI_WINDOWS_FRAME_PRESENTED_V1,
                   true);
    }

    const auto width = sizes.back().first;
    const auto height = sizes.back().second;
    for (const auto [causal_id, terminal] :
         std::array<std::pair<uint64_t, uint32_t>, 3>{{
             {UINT64_MAX - 1, DOROTI_WINDOWS_FRAME_SUPERSEDED_V1},
             {UINT64_MAX, DOROTI_WINDOWS_FRAME_FAILED_V1},
             {UINT64_MAX - 2, DOROTI_WINDOWS_FRAME_FAILED_V1},
         }}) {
      auto resource = CreateFrameResource(width, height);
      InvokeRender(*resource.Get(), width, height, ++generation, causal_id,
                   terminal, false);
    }

    ReleaseContext();
    WaitIdle();
    CountDebugErrors();
    ReleaseNative();
    ReportLiveObjects();
  }

 private:
  void CreateWindows() {
    const auto instance = GetModuleHandleW(nullptr);
    for (const auto* name : {kTopClass, kChildClass}) {
      WNDCLASSEXW description{};
      description.cbSize = sizeof(description);
      description.lpfnWndProc = WindowProcedure;
      description.hInstance = instance;
      description.lpszClassName = name;
      if (!RegisterClassExW(&description) &&
          GetLastError() != ERROR_CLASS_ALREADY_EXISTS) {
        throw std::runtime_error("Lease probe RegisterClassExW failed");
      }
    }
    top_ = CreateWindowExW(0, kTopClass, L"", WS_OVERLAPPEDWINDOW, 0, 0,
                           640, 480, nullptr, nullptr, instance, nullptr);
    child_ = CreateWindowExW(0, kChildClass, L"", WS_CHILD | WS_VISIBLE, 0, 0,
                             640, 480, top_, nullptr, instance, nullptr);
    if (!top_ || !child_) {
      throw std::runtime_error("Lease probe HWND creation failed");
    }
  }

  void CreateDevice() {
    ComPtr<ID3D12Debug> debug;
    Check(D3D12GetDebugInterface(IID_PPV_ARGS(&debug)),
          "Lease probe D3D12 debug layer unavailable");
    debug->EnableDebugLayer();
    Check(CreateDXGIFactory2(DXGI_CREATE_FACTORY_DEBUG,
                             IID_PPV_ARGS(&factory_)),
          "Lease probe CreateDXGIFactory2 failed");
    for (UINT index = 0;; ++index) {
      ComPtr<IDXGIAdapter1> candidate;
      if (factory_->EnumAdapterByGpuPreference(
              index, DXGI_GPU_PREFERENCE_HIGH_PERFORMANCE,
              IID_PPV_ARGS(&candidate)) == DXGI_ERROR_NOT_FOUND) {
        break;
      }
      DXGI_ADAPTER_DESC1 description{};
      candidate->GetDesc1(&description);
      if ((description.Flags & DXGI_ADAPTER_FLAG_SOFTWARE) != 0) {
        continue;
      }
      if (SUCCEEDED(D3D12CreateDevice(candidate.Get(), D3D_FEATURE_LEVEL_11_0,
                                      IID_PPV_ARGS(&device_)))) {
        adapter_ = std::move(candidate);
        adapter_luid_ = description.AdapterLuid;
        break;
      }
    }
    if (!device_) {
      throw std::runtime_error("Lease probe found no hardware D3D12 adapter");
    }
    Check(device_.As(&info_queue_), "Lease probe ID3D12InfoQueue unavailable");
    D3D12_COMMAND_QUEUE_DESC queue_description{};
    queue_description.Type = D3D12_COMMAND_LIST_TYPE_DIRECT;
    Check(device_->CreateCommandQueue(&queue_description,
                                       IID_PPV_ARGS(&queue_)),
          "Lease probe CreateCommandQueue failed");
    Check(device_->CreateCommandAllocator(D3D12_COMMAND_LIST_TYPE_DIRECT,
                                           IID_PPV_ARGS(&allocator_)),
          "Lease probe CreateCommandAllocator failed");
    Check(device_->CreateCommandList(0, D3D12_COMMAND_LIST_TYPE_DIRECT,
                                      allocator_.Get(), nullptr,
                                      IID_PPV_ARGS(&command_list_)),
          "Lease probe CreateCommandList failed");
    Check(command_list_->Close(), "Lease probe initial command-list close failed");
    Check(device_->CreateFence(0, D3D12_FENCE_FLAG_NONE,
                                IID_PPV_ARGS(&fence_)),
          "Lease probe CreateFence failed");
    fence_event_ = CreateEventW(nullptr, FALSE, FALSE, nullptr);
    if (!fence_event_) {
      throw std::runtime_error("Lease probe fence event creation failed");
    }
  }

  void AcquireContext() {
    device_->AddRef();
    queue_->AddRef();
    const uint64_t luid = static_cast<uint32_t>(adapter_luid_.LowPart) |
                          (static_cast<uint64_t>(
                               static_cast<uint32_t>(adapter_luid_.HighPart))
                           << 32);
    doroti_windows_d3d12_host_lease_v1 lease{
        DOROTI_WINDOWS_ABI_VERSION_V1,
        sizeof(doroti_windows_d3d12_host_lease_v1),
        host_generation_,
        context_generation_,
        luid,
        device_.Get(),
        queue_.Get(),
    };
    const auto status =
        callbacks_.acquire_context(callbacks_.callback_context, &lease);
    if (status != 0) {
      queue_->Release();
      device_->Release();
      throw std::runtime_error("Managed host-lifetime lease acquisition failed");
    }
    context_active_ = true;
    ++result_.context_acquire_count;
  }

  void ReleaseContext() {
    if (!context_active_) {
      return;
    }
    const auto status = callbacks_.release_context(
        callbacks_.callback_context, host_generation_, context_generation_);
    if (status != 0) {
      throw std::runtime_error("Managed host-lifetime lease release ACK failed");
    }
    context_active_ = false;
    ++result_.context_release_count;
  }

  void ResizeChild(uint32_t width, uint32_t height) {
    if (!SetWindowPos(child_, nullptr, 0, 0, static_cast<int>(width),
                      static_cast<int>(height),
                      SWP_NOZORDER | SWP_NOACTIVATE)) {
      throw std::runtime_error("Lease probe child resize failed");
    }
  }

  void CreateOrResizeSwapChain(uint32_t width, uint32_t height) {
    WaitIdle();
    if (!swap_chain_) {
      DXGI_SWAP_CHAIN_DESC1 description{};
      description.Width = width;
      description.Height = height;
      description.Format = DXGI_FORMAT_B8G8R8A8_UNORM;
      description.SampleDesc = {1, 0};
      description.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
      description.BufferCount = 2;
      description.Scaling = DXGI_SCALING_NONE;
      description.SwapEffect = DXGI_SWAP_EFFECT_FLIP_DISCARD;
      description.AlphaMode = DXGI_ALPHA_MODE_IGNORE;
      ComPtr<IDXGISwapChain1> created;
      Check(factory_->CreateSwapChainForHwnd(queue_.Get(), child_, &description,
                                              nullptr, nullptr, &created),
            "Lease probe CreateSwapChainForHwnd failed");
      Check(created.As(&swap_chain_), "Lease probe IDXGISwapChain3 unavailable");
    } else {
      const auto resized = swap_chain_->ResizeBuffers(
          2, width, height, DXGI_FORMAT_B8G8R8A8_UNORM, 0);
      if (resized == DXGI_ERROR_INVALID_CALL) {
        ++result_.resize_invalid_call_count;
      }
      Check(resized, "Lease probe ResizeBuffers failed");
      ++result_.resize_buffers_count;
    }
  }

  ComPtr<ID3D12Resource> CreateFrameResource(uint32_t width, uint32_t height) {
    D3D12_RESOURCE_DESC description{};
    description.Dimension = D3D12_RESOURCE_DIMENSION_TEXTURE2D;
    description.Width = width;
    description.Height = height;
    description.DepthOrArraySize = 1;
    description.MipLevels = 1;
    description.Format = DXGI_FORMAT_B8G8R8A8_UNORM;
    description.SampleDesc = {1, 0};
    description.Flags = D3D12_RESOURCE_FLAG_ALLOW_RENDER_TARGET;
    const auto heap = DefaultHeap();
    D3D12_CLEAR_VALUE clear{};
    clear.Format = description.Format;
    ComPtr<ID3D12Resource> resource;
    Check(device_->CreateCommittedResource(
              &heap, D3D12_HEAP_FLAG_NONE, &description,
              D3D12_RESOURCE_STATE_RENDER_TARGET, &clear,
              IID_PPV_ARGS(&resource)),
          "Lease probe exact frame resource creation failed");
    return resource;
  }

  void InvokeRender(ID3D12Resource& resource, uint32_t width, uint32_t height,
                    uint64_t generation, uint64_t causal_frame_id,
                    uint32_t expected_terminal, bool should_present) {
    const uint64_t lease_id = ++lease_id_;
    doroti_windows_frame_request_v1 request{
        DOROTI_WINDOWS_ABI_VERSION_V1,
        sizeof(doroti_windows_frame_request_v1),
        1,
        generation,
        width,
        height,
        causal_frame_id,
        lease_id,
    };
    resource.AddRef();
    doroti_windows_d3d12_lease_v1 lease{
        DOROTI_WINDOWS_ABI_VERSION_V1,
        sizeof(doroti_windows_d3d12_lease_v1),
        lease_id,
        host_generation_,
        context_generation_,
        static_cast<uint32_t>(adapter_luid_.LowPart) |
            (static_cast<uint64_t>(
                 static_cast<uint32_t>(adapter_luid_.HighPart))
             << 32),
        device_.Get(),
        queue_.Get(),
        &resource,
        width,
        height,
        DXGI_FORMAT_B8G8R8A8_UNORM,
        DOROTI_WINDOWS_RESOURCE_RENDER_TARGET_V1,
        1,
        0,
    };
    ++result_.render_callback_count;
    const auto callback_status = callbacks_.render(
        callbacks_.callback_context, &request, &lease);

    const auto reference_probe = resource.AddRef();
    resource.Release();
    if (reference_probe != 2) {
      ++result_.per_frame_reference_leak_count;
    }

    uint32_t terminal = expected_terminal;
    if (should_present && callback_status == 0) {
      SignalManagedSubmitFence();
      CopyAndPresent(resource);
      terminal = DOROTI_WINDOWS_FRAME_PRESENTED_V1;
    } else if (should_present) {
      terminal = DOROTI_WINDOWS_FRAME_FAILED_V1;
    }
    if (!should_present && callback_status == 0) {
      throw std::runtime_error("Managed failure probe unexpectedly succeeded");
    }
    doroti_windows_frame_terminal_v1 receipt{
        DOROTI_WINDOWS_ABI_VERSION_V1,
        sizeof(doroti_windows_frame_terminal_v1),
        1,
        generation,
        causal_frame_id,
        lease_id,
        terminal,
        callback_status,
        0,
        0,
        callback_status == 3 ? 1u : 0u,
        0,
    };
    callbacks_.frame_terminal(callbacks_.callback_context, &receipt);
    if (terminal == DOROTI_WINDOWS_FRAME_PRESENTED_V1) {
      ++result_.presented_count;
    } else if (terminal == DOROTI_WINDOWS_FRAME_SUPERSEDED_V1) {
      ++result_.superseded_count;
    } else {
      ++result_.failed_count;
    }
  }

  void SignalManagedSubmitFence() {
    const auto value = ++fence_value_;
    Check(queue_->Signal(fence_.Get(), value),
          "Lease probe managed-submit fence signal failed");
    if (fence_->GetCompletedValue() < value) {
      Check(fence_->SetEventOnCompletion(value, fence_event_),
            "Lease probe managed-submit fence registration failed");
      if (WaitForSingleObject(fence_event_, 5000) != WAIT_OBJECT_0) {
        throw std::runtime_error("Lease probe managed-submit fence timed out");
      }
    }
    ++result_.fence_after_submit_count;
  }

  void CopyAndPresent(ID3D12Resource& resource) {
    ComPtr<ID3D12Resource> back_buffer;
    Check(swap_chain_->GetBuffer(swap_chain_->GetCurrentBackBufferIndex(),
                                 IID_PPV_ARGS(&back_buffer)),
          "Lease probe GetBuffer failed");
    Check(allocator_->Reset(), "Lease probe allocator reset failed");
    Check(command_list_->Reset(allocator_.Get(), nullptr),
          "Lease probe command-list reset failed");
    std::array barriers{
        Transition(&resource, D3D12_RESOURCE_STATE_RENDER_TARGET,
                   D3D12_RESOURCE_STATE_COPY_SOURCE),
        Transition(back_buffer.Get(), D3D12_RESOURCE_STATE_PRESENT,
                   D3D12_RESOURCE_STATE_COPY_DEST),
    };
    command_list_->ResourceBarrier(static_cast<UINT>(barriers.size()),
                                   barriers.data());
    command_list_->CopyResource(back_buffer.Get(), &resource);
    const auto present =
        Transition(back_buffer.Get(), D3D12_RESOURCE_STATE_COPY_DEST,
                   D3D12_RESOURCE_STATE_PRESENT);
    command_list_->ResourceBarrier(1, &present);
    Check(command_list_->Close(), "Lease probe command-list close failed");
    ID3D12CommandList* lists[]{command_list_.Get()};
    queue_->ExecuteCommandLists(1, lists);
    WaitIdle();
    back_buffer.Reset();
    Check(swap_chain_->Present(0, 0), "Lease probe Present failed");
    Check(DwmFlush(), "Lease probe DwmFlush failed");
  }

  void WaitIdle() {
    if (!queue_) {
      return;
    }
    const auto value = ++fence_value_;
    Check(queue_->Signal(fence_.Get(), value), "Lease probe idle signal failed");
    if (fence_->GetCompletedValue() < value) {
      Check(fence_->SetEventOnCompletion(value, fence_event_),
            "Lease probe idle fence registration failed");
      if (WaitForSingleObject(fence_event_, 5000) != WAIT_OBJECT_0) {
        throw std::runtime_error("Lease probe idle fence timed out");
      }
    }
  }

  void CountDebugErrors() {
    const auto count = info_queue_->GetNumStoredMessagesAllowedByRetrievalFilter();
    for (uint64_t index = 0; index < count; ++index) {
      SIZE_T length{};
      if (FAILED(info_queue_->GetMessage(index, nullptr, &length)) || length == 0) {
        continue;
      }
      std::vector<uint8_t> storage(length);
      auto* message = reinterpret_cast<D3D12_MESSAGE*>(storage.data());
      if (FAILED(info_queue_->GetMessage(index, message, &length))) {
        continue;
      }
      if (message->Severity == D3D12_MESSAGE_SEVERITY_ERROR) {
        ++result_.debug_error_count;
        std::fprintf(stderr, "D3D12 error id=%u: %s\n",
                     static_cast<unsigned>(message->ID),
                     message->pDescription ? message->pDescription : "");
      } else if (message->Severity == D3D12_MESSAGE_SEVERITY_CORRUPTION) {
        ++result_.debug_corruption_count;
        std::fprintf(stderr, "D3D12 corruption id=%u: %s\n",
                     static_cast<unsigned>(message->ID),
                     message->pDescription ? message->pDescription : "");
      }
    }
  }

  void ReleaseNative() {
    swap_chain_.Reset();
    info_queue_.Reset();
    command_list_.Reset();
    allocator_.Reset();
    fence_.Reset();
    queue_.Reset();
    device_.Reset();
    adapter_.Reset();
    factory_.Reset();
    if (fence_event_) {
      CloseHandle(fence_event_);
      fence_event_ = nullptr;
    }
    if (child_) {
      DestroyWindow(child_);
      child_ = nullptr;
    }
    if (top_) {
      DestroyWindow(top_);
      top_ = nullptr;
    }
  }

  void ReportLiveObjects() {
    ComPtr<IDXGIDebug1> debug;
    result_.live_object_report_hresult =
        DXGIGetDebugInterface1(0, IID_PPV_ARGS(&debug));
    if (SUCCEEDED(result_.live_object_report_hresult)) {
      result_.live_object_report_hresult = debug->ReportLiveObjects(
          DXGI_DEBUG_ALL,
          static_cast<DXGI_DEBUG_RLO_FLAGS>(DXGI_DEBUG_RLO_SUMMARY |
                                            DXGI_DEBUG_RLO_IGNORE_INTERNAL));
    }
  }

  const doroti_windows_callbacks_v1& callbacks_;
  doroti_windows_lease_probe_result_v1& result_;
  HWND top_{};
  HWND child_{};
  ComPtr<IDXGIFactory6> factory_;
  ComPtr<IDXGIAdapter1> adapter_;
  ComPtr<ID3D12Device5> device_;
  ComPtr<ID3D12CommandQueue> queue_;
  ComPtr<ID3D12CommandAllocator> allocator_;
  ComPtr<ID3D12GraphicsCommandList> command_list_;
  ComPtr<ID3D12Fence> fence_;
  ComPtr<ID3D12InfoQueue> info_queue_;
  ComPtr<IDXGISwapChain3> swap_chain_;
  HANDLE fence_event_{};
  LUID adapter_luid_{};
  uint64_t host_generation_{1};
  uint64_t context_generation_{1};
  uint64_t lease_id_{};
  uint64_t fence_value_{};
  bool context_active_{};
};

}  // namespace

uint32_t DOROTI_WINDOWS_CALL doroti_windows_run_lease_probe_v1(
    const doroti_windows_callbacks_v1* callbacks,
    doroti_windows_lease_probe_result_v1* result) {
  if (!ValidHeader(callbacks) || !ValidHeader(result) ||
      callbacks->acquire_context == nullptr ||
      callbacks->release_context == nullptr || callbacks->render == nullptr ||
      callbacks->frame_terminal == nullptr) {
    return DOROTI_WINDOWS_STATUS_ABI_MISMATCH_V1;
  }
  *result = {};
  result->abi_version = DOROTI_WINDOWS_ABI_VERSION_V1;
  result->struct_size = sizeof(doroti_windows_lease_probe_result_v1);
  try {
    Probe probe(*callbacks, *result);
    probe.Run();
    const bool passed = result->context_acquire_count == 2 &&
                        result->context_release_count == 2 &&
                        result->render_callback_count == 13 &&
                        result->presented_count == 10 &&
                        result->superseded_count == 1 &&
                        result->failed_count == 2 &&
                        result->fence_after_submit_count == 10 &&
                        result->resize_buffers_count == 10 &&
                        result->resize_invalid_call_count == 0 &&
                        result->per_frame_reference_leak_count == 0 &&
                        result->debug_error_count == 0 &&
                        result->debug_corruption_count == 0 &&
                        SUCCEEDED(result->live_object_report_hresult);
    result->status = passed ? DOROTI_WINDOWS_STATUS_OK_V1
                            : DOROTI_WINDOWS_STATUS_NATIVE_FAILURE_V1;
  } catch (...) {
    result->status = DOROTI_WINDOWS_STATUS_NATIVE_FAILURE_V1;
  }
  return result->status;
}
