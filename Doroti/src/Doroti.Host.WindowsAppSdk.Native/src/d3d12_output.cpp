#include "doroti_windows_d3d12_output_v1.h"
#include "resize_order_trace.h"
#include <windows.h>
#include <d3d12.h>
#include <d3d12sdklayers.h>
#include <dcomp.h>
#include <dwmapi.h>
#include <dxgi1_6.h>
#include <wrl/client.h>
#include <algorithm>
#include <memory>
#include <mutex>
#include <cstdio>
#include <vector>

using Microsoft::WRL::ComPtr;
extern "C" int32_t __cdecl doroti_windows_composition_raster_create_on_adapter_v1(
    uint32_t, int32_t, IUnknown**, uint64_t, void**);

namespace {
constexpr UINT kCount = 3;
constexpr DWORD kTimeoutMs = 5000;
struct Handle {
  HANDLE value{};
  ~Handle() { if (value) CloseHandle(value); }
};
struct Slot {
  ComPtr<ID3D12Resource> resource;
  Handle available;
  UINT width{}, height{};
  UINT64 ready{}, copied{};
};
struct Frame {
  ComPtr<ID3D12Resource> buffer;
  ComPtr<ID3D12CommandAllocator> allocator;
  ComPtr<ID3D12GraphicsCommandList> list;
  UINT64 copied{};
};
struct Output {
  std::mutex gate;
  ComPtr<IDXGIFactory4> factory;
  ComPtr<ID3D12Device> device;
  ComPtr<ID3D12InfoQueue> debug;
  ComPtr<IUnknown> raster_device;
  ComPtr<ID3D12CommandQueue> queue;
  ComPtr<ID3D12Fence> producer, completion;
  ComPtr<IDXGISwapChain3> swapchain;
  ComPtr<IDCompositionDevice> composition;
  ComPtr<IDCompositionTarget> target;
  ComPtr<IDCompositionVisual> visual;
  Handle completion_event;
  Slot slots[kCount];
  Frame frames[kCount];
  LUID luid{};
  UINT width{}, height{};
  UINT64 submitted{}, producer_value{};
  UINT64 allocations{}, resizes{}, debug_errors{}, debug_warnings{};
  bool alpha{}, poisoned{};

  HRESULT CheckDebug() {
    if (!debug) return S_OK;
    const auto count = debug->GetNumStoredMessages();
    for (UINT64 i = 0; i < count; ++i) {
      SIZE_T length{};
      auto hr = debug->GetMessage(i, nullptr, &length);
      if (FAILED(hr)) return hr;
      std::vector<unsigned char> storage(length);
      auto message = reinterpret_cast<D3D12_MESSAGE*>(storage.data());
      hr = debug->GetMessage(i, message, &length);
      if (FAILED(hr)) return hr;
      if (message->Severity <= D3D12_MESSAGE_SEVERITY_ERROR) ++debug_errors;
      else if (message->Severity == D3D12_MESSAGE_SEVERITY_WARNING) ++debug_warnings;
      else continue;
      std::fprintf(stderr, "D3D12 validation: id=%u severity=%u %s\n",
          static_cast<unsigned>(message->ID), static_cast<unsigned>(message->Severity), message->pDescription);
    }
    debug->ClearStoredMessages();
    return debug_errors ? E_FAIL : S_OK;
  }

  HRESULT Wait(UINT64 value) {
    if (poisoned) return DXGI_ERROR_DEVICE_REMOVED;
    const auto done = completion->GetCompletedValue();
    if (done == UINT64_MAX) return DXGI_ERROR_DEVICE_REMOVED;
    if (done >= value) return S_OK;
    auto hr = completion->SetEventOnCompletion(value, completion_event.value);
    if (FAILED(hr)) return hr;
    const auto deadline = GetTickCount64() + kTimeoutMs;
    while (true) {
      const auto current = completion->GetCompletedValue();
      if (current == UINT64_MAX) return DXGI_ERROR_DEVICE_REMOVED;
      if (current >= value) return S_OK;
      const auto now = GetTickCount64();
      if (now >= deadline) return HRESULT_FROM_WIN32(WAIT_TIMEOUT);
      const auto result = WaitForSingleObject(completion_event.value, static_cast<DWORD>(deadline - now));
      if (result == WAIT_TIMEOUT) return HRESULT_FROM_WIN32(WAIT_TIMEOUT);
      if (result != WAIT_OBJECT_0) return HRESULT_FROM_WIN32(GetLastError());
      // A signal from an earlier timed-out registration does not prove this value.
    }
  }

  HRESULT Available(Slot& slot, uint32_t* available) {
    *available = 0;
    if (poisoned) return DXGI_ERROR_DEVICE_REMOVED;
    const auto done = completion->GetCompletedValue();
    if (done == UINT64_MAX) return DXGI_ERROR_DEVICE_REMOVED;
    if (slot.resource && done >= slot.copied) {
      *available = 1;
      if (!SetEvent(slot.available.value)) return HRESULT_FROM_WIN32(GetLastError());
    }
    return S_OK;
  }

  HRESULT EnsureSwapchain(UINT requested_width, UINT requested_height) {
    if (swapchain && width >= requested_width && height >= requested_height) return S_OK;
    auto hr = Wait(submitted);
    if (FAILED(hr)) return hr;
    const auto next_width = std::max(width, requested_width);
    const auto next_height = std::max(height, requested_height);
    for (auto& frame : frames) frame.buffer.Reset();
    if (swapchain) {
      hr = swapchain->ResizeBuffers(kCount, next_width, next_height,
          DXGI_FORMAT_B8G8R8A8_UNORM, 0);
      if (SUCCEEDED(hr)) ++resizes;
    } else {
      DXGI_SWAP_CHAIN_DESC1 desc{};
      desc.Width = next_width; desc.Height = next_height;
      desc.Format = DXGI_FORMAT_B8G8R8A8_UNORM;
      desc.SampleDesc.Count = 1;
      desc.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
      desc.BufferCount = kCount;
      desc.Scaling = DXGI_SCALING_STRETCH;
      desc.SwapEffect = DXGI_SWAP_EFFECT_FLIP_SEQUENTIAL;
      desc.AlphaMode = alpha ? DXGI_ALPHA_MODE_PREMULTIPLIED : DXGI_ALPHA_MODE_IGNORE;
      ComPtr<IDXGISwapChain1> created;
      hr = factory->CreateSwapChainForComposition(queue.Get(), &desc, nullptr, &created);
      if (SUCCEEDED(hr)) hr = created.As(&swapchain);
      if (SUCCEEDED(hr)) hr = swapchain->SetColorSpace1(DXGI_COLOR_SPACE_RGB_FULL_G22_NONE_P709);
      if (SUCCEEDED(hr)) hr = visual->SetContent(swapchain.Get());
      if (SUCCEEDED(hr)) hr = composition->Commit();
    }
    if (FAILED(hr)) { poisoned = true; return hr; }
    width = next_width; height = next_height;
    for (UINT i = 0; i < kCount; ++i) {
      hr = swapchain->GetBuffer(i, IID_PPV_ARGS(&frames[i].buffer));
      if (FAILED(hr)) { poisoned = true; return hr; }
    }
    return S_OK;
  }
};

D3D12_RESOURCE_BARRIER Transition(ID3D12Resource* resource,
    D3D12_RESOURCE_STATES before, D3D12_RESOURCE_STATES after) {
  D3D12_RESOURCE_BARRIER barrier{};
  barrier.Type = D3D12_RESOURCE_BARRIER_TYPE_TRANSITION;
  barrier.Transition = {resource, D3D12_RESOURCE_BARRIER_ALL_SUBRESOURCES, before, after};
  return barrier;
}
} // namespace

DOROTI_D3D12_API doroti_windows_d3d12_output_create_v1(
    uint32_t luid_low, int32_t luid_high, void** context, uint64_t* fence_handle) {
  if (!context || !fence_handle) return E_INVALIDARG;
  *context = nullptr; *fence_handle = 0;
  auto value = std::make_unique<Output>();
  wchar_t validation[2]{};
  const bool validate = GetEnvironmentVariableW(L"DOROTI_WINDOWS_D3D12_VALIDATION", validation, 2) == 1 && validation[0] == L'1';
  if (validate) {
    ComPtr<ID3D12Debug> debug;
    const auto result = D3D12GetDebugInterface(IID_PPV_ARGS(&debug));
    if (FAILED(result)) return result;
    debug->EnableDebugLayer();
  }
  value->luid = {luid_low, luid_high};
  auto hr = CreateDXGIFactory2(0, IID_PPV_ARGS(&value->factory));
  ComPtr<IDXGIAdapter1> adapter;
  if (SUCCEEDED(hr)) hr = value->factory->EnumAdapterByLuid(value->luid, IID_PPV_ARGS(&adapter));
  DXGI_ADAPTER_DESC1 desc{};
  if (SUCCEEDED(hr)) hr = adapter->GetDesc1(&desc);
  if (SUCCEEDED(hr) && (desc.Flags & DXGI_ADAPTER_FLAG_SOFTWARE)) hr = DXGI_ERROR_UNSUPPORTED;
  if (SUCCEEDED(hr)) hr = D3D12CreateDevice(adapter.Get(), D3D_FEATURE_LEVEL_11_0, IID_PPV_ARGS(&value->device));
  if (FAILED(hr)) return hr;
  if (validate) {
    hr = value->device.As(&value->debug);
    if (FAILED(hr)) return hr;
  }
  const auto actual = value->device->GetAdapterLuid();
  if (actual.LowPart != luid_low || actual.HighPart != luid_high) return DXGI_ERROR_INVALID_CALL;
  D3D12_COMMAND_QUEUE_DESC queue{};
  queue.Type = D3D12_COMMAND_LIST_TYPE_DIRECT;
  hr = value->device->CreateCommandQueue(&queue, IID_PPV_ARGS(&value->queue));
  if (SUCCEEDED(hr)) hr = value->device->CreateFence(0, D3D12_FENCE_FLAG_SHARED, IID_PPV_ARGS(&value->producer));
  if (SUCCEEDED(hr)) hr = value->device->CreateFence(0, D3D12_FENCE_FLAG_NONE, IID_PPV_ARGS(&value->completion));
  if (FAILED(hr)) return hr;
  value->completion_event.value = CreateEventW(nullptr, FALSE, FALSE, nullptr);
  if (!value->completion_event.value) return HRESULT_FROM_WIN32(GetLastError());
  for (auto& frame : value->frames) {
    hr = value->device->CreateCommandAllocator(D3D12_COMMAND_LIST_TYPE_DIRECT, IID_PPV_ARGS(&frame.allocator));
    if (SUCCEEDED(hr)) hr = value->device->CreateCommandList(0, D3D12_COMMAND_LIST_TYPE_DIRECT,
        frame.allocator.Get(), nullptr, IID_PPV_ARGS(&frame.list));
    if (SUCCEEDED(hr)) hr = frame.list->Close();
    if (FAILED(hr)) return hr;
  }
  // DXGI swapchain content needs no D3D11 device or Presentation API manager.
  hr = DCompositionCreateDevice(nullptr, IID_PPV_ARGS(&value->composition));
  if (SUCCEEDED(hr)) hr = value->composition->CreateVisual(&value->visual);
  HANDLE shared{};
  if (SUCCEEDED(hr)) hr = value->device->CreateSharedHandle(value->producer.Get(), nullptr, GENERIC_ALL, nullptr, &shared);
  if (FAILED(hr)) return hr;
  *fence_handle = reinterpret_cast<uint64_t>(shared);
  *context = value.release();
  return S_OK;
}

DOROTI_D3D12_API doroti_windows_d3d12_output_attach_v1(void* context, uint64_t hwnd) {
  if (!context || !IsWindow(reinterpret_cast<HWND>(hwnd))) return E_INVALIDARG;
  auto& value = *static_cast<Output*>(context);
  std::lock_guard lock(value.gate);
  if (value.target) return DXGI_ERROR_INVALID_CALL;
  auto hr = value.composition->CreateTargetForHwnd(reinterpret_cast<HWND>(hwnd), TRUE, &value.target);
  if (SUCCEEDED(hr)) hr = value.target->SetRoot(value.visual.Get());
  if (SUCCEEDED(hr)) hr = value.composition->Commit();
  return hr;
}

DOROTI_D3D12_API doroti_windows_d3d12_output_alpha_v1(void* context, uint32_t enabled) {
  if (!context || enabled > 1) return E_INVALIDARG;
  auto& value = *static_cast<Output*>(context);
  std::lock_guard lock(value.gate);
  if (value.swapchain) return DXGI_ERROR_INVALID_CALL;
  value.alpha = enabled != 0;
  return S_OK;
}

DOROTI_D3D12_API doroti_windows_d3d12_output_replace_v1(
    void* context, uint32_t index, uint32_t width, uint32_t height,
    uint64_t* resource_handle, uint64_t* available_event) {
  if (!context || index >= kCount || !width || !height || width > 16384 || height > 16384 ||
      !resource_handle || !available_event) return E_INVALIDARG;
  *resource_handle = 0; *available_event = 0;
  auto& value = *static_cast<Output*>(context);
  std::lock_guard lock(value.gate);
  auto& slot = value.slots[index];
  auto hr = value.Wait(slot.copied);
  if (FAILED(hr)) return hr;
  D3D12_HEAP_PROPERTIES heap{};
  heap.Type = D3D12_HEAP_TYPE_DEFAULT; heap.CreationNodeMask = heap.VisibleNodeMask = 1;
  D3D12_RESOURCE_DESC desc{};
  desc.Dimension = D3D12_RESOURCE_DIMENSION_TEXTURE2D;
  desc.Width = width; desc.Height = height;
  desc.DepthOrArraySize = desc.MipLevels = 1;
  desc.Format = DXGI_FORMAT_B8G8R8A8_UNORM; desc.SampleDesc.Count = 1;
  desc.Layout = D3D12_TEXTURE_LAYOUT_UNKNOWN;
  const auto allocation = value.device->GetResourceAllocationInfo(0, 1, &desc);
  if (allocation.SizeInBytes > 512ULL * 1024 * 1024) return E_OUTOFMEMORY;
  ComPtr<ID3D12Resource> resource;
  hr = value.device->CreateCommittedResource(&heap, D3D12_HEAP_FLAG_SHARED, &desc,
      D3D12_RESOURCE_STATE_COMMON, nullptr, IID_PPV_ARGS(&resource));
  if (FAILED(hr)) return hr;
  if (!slot.available.value) slot.available.value = CreateEventW(nullptr, TRUE, TRUE, nullptr);
  if (!slot.available.value) return HRESULT_FROM_WIN32(GetLastError());
  HANDLE shared{};
  hr = value.device->CreateSharedHandle(resource.Get(), nullptr, GENERIC_ALL, nullptr, &shared);
  if (FAILED(hr)) return hr;
  slot.resource = resource; slot.width = width; slot.height = height; slot.ready = 0; slot.copied = 0;
  ++value.allocations;
  SetEvent(slot.available.value);
  *resource_handle = reinterpret_cast<uint64_t>(shared);
  *available_event = reinterpret_cast<uint64_t>(slot.available.value);
  return S_OK;
}

DOROTI_D3D12_API doroti_windows_d3d12_output_available_v1(void* context, uint32_t index, uint32_t* available) {
  if (!context || index >= kCount || !available) return E_INVALIDARG;
  auto& value = *static_cast<Output*>(context);
  std::lock_guard lock(value.gate);
  return value.Available(value.slots[index], available);
}

DOROTI_D3D12_API doroti_windows_d3d12_output_ready_v1(void* context, uint32_t index, uint64_t producer_value) {
  if (!context || index >= kCount || !producer_value) return E_INVALIDARG;
  auto& value = *static_cast<Output*>(context);
  std::lock_guard lock(value.gate);
  uint32_t available{};
  auto hr = value.Available(value.slots[index], &available);
  if (FAILED(hr)) return hr;
  if (!available || producer_value <= value.producer_value) return DXGI_ERROR_INVALID_CALL;
  value.producer_value = producer_value;
  value.slots[index].ready = producer_value;
  return S_OK;
}

DOROTI_D3D12_API doroti_windows_d3d12_output_present_v1(
    void* context, uint32_t index, uint32_t source_x, uint32_t source_y,
    uint32_t width, uint32_t height, uint64_t, uint32_t wait_for_frame,
    uint32_t timeout_ms, uint32_t* observed, uint64_t* present_id, uint64_t* completed) {
  if (!context || index >= kCount || !observed || !present_id || !completed || !width || !height || wait_for_frame > 2)
    return E_INVALIDARG;
  *observed = 0; *present_id = 0; *completed = 0;
  auto& value = *static_cast<Output*>(context);
  std::lock_guard lock(value.gate);
  auto& slot = value.slots[index];
  if (!value.target || !slot.resource || !slot.ready || source_x > slot.width || source_y > slot.height ||
      width > slot.width - source_x || height > slot.height - source_y) return E_INVALIDARG;
  uint32_t available{};
  auto hr = value.Available(slot, &available);
  if (FAILED(hr)) return hr;
  if (!available) return DXGI_ERROR_WAS_STILL_DRAWING;
  hr = value.EnsureSwapchain(width, height);
  if (FAILED(hr)) return hr;
  auto& frame = value.frames[value.swapchain->GetCurrentBackBufferIndex()];
  hr = value.Wait(frame.copied);
  if (SUCCEEDED(hr)) hr = frame.allocator->Reset();
  if (SUCCEEDED(hr)) hr = frame.list->Reset(frame.allocator.Get(), nullptr);
  if (FAILED(hr)) return hr;
  D3D12_RESOURCE_BARRIER barriers[]{
      Transition(slot.resource.Get(), D3D12_RESOURCE_STATE_COMMON, D3D12_RESOURCE_STATE_COPY_SOURCE),
      Transition(frame.buffer.Get(), D3D12_RESOURCE_STATE_PRESENT, D3D12_RESOURCE_STATE_COPY_DEST)};
  frame.list->ResourceBarrier(2, barriers);
  D3D12_TEXTURE_COPY_LOCATION src{}, dst{};
  src.pResource = slot.resource.Get(); dst.pResource = frame.buffer.Get();
  src.Type = dst.Type = D3D12_TEXTURE_COPY_TYPE_SUBRESOURCE_INDEX;
  D3D12_BOX box{source_x, source_y, 0, source_x + width, source_y + height, 1};
  frame.list->CopyTextureRegion(&dst, 0, 0, 0, &src, &box);
  for (auto& barrier : barriers) std::swap(barrier.Transition.StateBefore, barrier.Transition.StateAfter);
  frame.list->ResourceBarrier(2, barriers);
  hr = frame.list->Close();
  if (SUCCEEDED(hr)) hr = value.queue->Wait(value.producer.Get(), slot.ready);
  if (FAILED(hr)) return hr;
  ID3D12CommandList* lists[]{frame.list.Get()};
  value.queue->ExecuteCommandLists(1, lists);
  // Until a completion signal is queued, failures must retain this owner.
  value.poisoned = true;
  const auto serial = ++value.submitted;
  slot.copied = frame.copied = serial;
  slot.ready = 0;
  ResetEvent(slot.available.value);
  // Clip retained capacity without scaling pixels to the client viewport.
  const D2D_RECT_F clip{0, 0, static_cast<float>(width), static_cast<float>(height)};
  hr = value.visual->SetClip(clip);
  if (SUCCEEDED(hr)) hr = value.composition->Commit();
  if (SUCCEEDED(hr)) {
    doroti::resize_trace::Record("d3d12-present-call", doroti::resize_trace::render_key);
    hr = value.swapchain->Present(0, 0);
  }
  // Signal after Present so resize/drain also cover queue work inserted by DXGI.
  const auto signal = value.queue->Signal(value.completion.Get(), serial);
  if (FAILED(signal)) return signal;
  value.poisoned = false;
  const auto event = value.completion->SetEventOnCompletion(serial, slot.available.value);
  if (FAILED(event)) return event;
  if (FAILED(hr)) return hr;
  hr = value.CheckDebug();
  if (FAILED(hr)) return hr;
  UINT count{};
  hr = value.swapchain->GetLastPresentCount(&count);
  if (FAILED(hr)) return hr;
  *present_id = count;
  *completed = value.completion->GetCompletedValue();
  if (*completed == UINT64_MAX) return DXGI_ERROR_DEVICE_REMOVED;
  if (wait_for_frame == 1) {
    // Startup occurs before the HWND is visible, so a displayed-frame receipt
    // cannot arrive yet. Preserve the ordinary DWM boundary used by this host.
    hr = DwmFlush();
    if (FAILED(hr)) return hr;
    *observed = 1;
  } else if (wait_for_frame == 2) {
    // A DXGI present-count receipt, not a Presentation API content-tag receipt.
    // No DwmFlush success is promoted to a matching frame statistic.
    const auto deadline = GetTickCount64() + std::min<DWORD>(timeout_ms, kTimeoutMs);
    do {
      DXGI_FRAME_STATISTICS statistics{};
      hr = value.swapchain->GetFrameStatistics(&statistics);
      if (SUCCEEDED(hr) && statistics.PresentCount == count) { *observed = 1; break; }
      if (FAILED(hr) && hr != DXGI_ERROR_FRAME_STATISTICS_DISJOINT) return hr;
      Sleep(1);
    } while (GetTickCount64() < deadline);
  }
  return S_OK;
}

DOROTI_D3D12_API doroti_windows_d3d12_output_drain_v1(void* context, uint64_t, uint64_t* completed) {
  if (!context || !completed) return E_INVALIDARG;
  auto& value = *static_cast<Output*>(context);
  std::lock_guard lock(value.gate);
  auto hr = value.Wait(value.submitted);
  if (SUCCEEDED(hr)) *completed = value.completion->GetCompletedValue();
  if (SUCCEEDED(hr)) hr = value.CheckDebug();
  return hr;
}

DOROTI_D3D12_API doroti_windows_d3d12_output_destroy_v1(void* context) {
  if (!context) return S_OK;
  auto value = static_cast<Output*>(context);
  // Failure retains the owner; managed shutdown quarantines it rather than
  // releasing resources still referenced by an unproven GPU submission.
  const auto hr = value->Wait(value->submitted);
  if (FAILED(hr)) return hr;
  if (value->target) value->target->SetRoot(nullptr);
  value->composition->Commit();
  const auto copies = value->submitted;
  const auto completed = value->completion->GetCompletedValue();
  const auto validated = value->debug != nullptr;
  delete value;
  if (validated) {
    std::fprintf(stderr, "doroti.d3d12.destroyed submitted=%llu completed=%llu\n",
        static_cast<unsigned long long>(copies), static_cast<unsigned long long>(completed));
    std::fflush(stderr);
  }
  return S_OK;
}

DOROTI_D3D12_API doroti_windows_d3d12_output_raster_create_v1(void* context, uint64_t hwnd, void** raster) {
  if (!context) return E_INVALIDARG;
  auto& value = *static_cast<Output*>(context);
  std::lock_guard lock(value.gate);
  return doroti_windows_composition_raster_create_on_adapter_v1(value.luid.LowPart, value.luid.HighPart,
      value.raster_device.GetAddressOf(), hwnd, raster);
}

DOROTI_D3D12_API doroti_windows_d3d12_output_snapshot_v1(
    void* context, struct doroti_windows_d3d12_output_snapshot_v1* snapshot) {
  if (!context || !snapshot) return E_INVALIDARG;
  auto& value = *static_cast<Output*>(context);
  std::lock_guard lock(value.gate);
  *snapshot = {value.submitted, value.completion->GetCompletedValue(), value.allocations, value.resizes,
      value.debug ? 1ULL : 0ULL, value.debug_errors, value.debug_warnings, value.swapchain ? 1ULL : 0ULL};
  return S_OK;
}
