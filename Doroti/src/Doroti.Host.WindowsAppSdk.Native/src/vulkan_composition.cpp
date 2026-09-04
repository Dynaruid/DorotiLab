#include "doroti_windows_vulkan_composition_v1.h"
#include "resize_order_trace.h"

#include <windows.h>
#include <Presentation.h>
#include <d3d11_4.h>
#include <dcomp.h>
#include <dwmapi.h>
#include <dxgi1_6.h>
#include <wrl/client.h>

#include <algorithm>
#include <cstdio>
#include <cstdint>
#include <memory>
#include <mutex>

using Microsoft::WRL::ComPtr;

extern "C" void DOROTI_WINDOWS_VULKAN_COMPOSITION_CALL
doroti_windows_vulkan_composition_trace_prepared_v1(void) {
  doroti::resize_trace::Record("prepare-copy-complete", doroti::resize_trace::render_key);
}

namespace {

constexpr uint32_t kBufferCount =
    DOROTI_WINDOWS_VULKAN_COMPOSITION_BUFFER_COUNT_V1;

struct BufferSlot {
  ComPtr<ID3D11Texture2D> texture;
  ComPtr<IPresentationBuffer> buffer;
  HANDLE available_event{};
  uint32_t width{};
  uint32_t height{};

  void Reset() noexcept {
    buffer.Reset();
    texture.Reset();
    if (available_event != nullptr) CloseHandle(available_event);
    available_event = nullptr;
    width = 0;
    height = 0;
  }

  ~BufferSlot() { Reset(); }
};

struct VulkanCompositionContext {
  std::mutex gate;
  ComPtr<IDXGIAdapter1> adapter;
  ComPtr<ID3D11Device> device;
  ComPtr<IPresentationFactory> factory;
  ComPtr<IPresentationManager> manager;
  ComPtr<IPresentationSurface> surface;
  ComPtr<IDCompositionDevice> composition_device;
  ComPtr<IDCompositionTarget> composition_target;
  ComPtr<IDCompositionVisual> composition_root;
  ComPtr<IUnknown> composition_surface;
  ComPtr<ID3D11Fence> retiring_fence;
  HANDLE surface_handle{};
  HANDLE present_statistics_event{};
  BufferSlot slots[kBufferCount];
  ComPtr<ID3D11Texture2D> retirement_texture;
  ComPtr<IPresentationBuffer> retirement_buffer;
  uint32_t bound_slot{kBufferCount};
  uint32_t bound_width{};
  uint32_t bound_height{};
  DXGI_ALPHA_MODE alpha_mode{DXGI_ALPHA_MODE_IGNORE};

  ~VulkanCompositionContext() {
    if (composition_target) composition_target->SetRoot(nullptr);
    if (composition_device) composition_device->Commit();
    composition_surface.Reset();
    composition_root.Reset();
    composition_target.Reset();
    composition_device.Reset();
    for (auto& slot : slots) slot.Reset();
    retirement_buffer.Reset();
    retirement_texture.Reset();
    retiring_fence.Reset();
    surface.Reset();
    if (present_statistics_event != nullptr)
      CloseHandle(present_statistics_event);
    present_statistics_event = nullptr;
    manager.Reset();
    factory.Reset();
    device.Reset();
    adapter.Reset();
    if (surface_handle != nullptr) CloseHandle(surface_handle);
  }
};

int32_t Result(HRESULT result) noexcept {
  return static_cast<int32_t>(result);
}

void InitializeProbe(
    doroti_windows_vulkan_composition_probe_v1& snapshot,
    int32_t requested_luid_low, int32_t requested_luid_high) noexcept {
  snapshot = {};
  snapshot.abi_version = DOROTI_WINDOWS_VULKAN_COMPOSITION_ABI_VERSION_V1;
  snapshot.struct_size = sizeof(snapshot);
  snapshot.dxgi_factory_hresult = Result(E_PENDING);
  snapshot.adapter_enumeration_hresult = Result(E_PENDING);
  snapshot.d3d11_device_hresult = Result(E_PENDING);
  snapshot.presentation_factory_hresult = Result(E_PENDING);
  snapshot.presentation_manager_hresult = Result(E_PENDING);
  snapshot.surface_handle_hresult = Result(E_PENDING);
  snapshot.presentation_surface_hresult = Result(E_PENDING);
  snapshot.retiring_fence_hresult = Result(E_PENDING);
  snapshot.requested_adapter_luid_low = requested_luid_low;
  snapshot.requested_adapter_luid_high = requested_luid_high;
}

void InitializeBufferSnapshot(
    doroti_windows_vulkan_composition_buffer_v1& snapshot,
    uint32_t width, uint32_t height) noexcept {
  snapshot = {};
  snapshot.abi_version = DOROTI_WINDOWS_VULKAN_COMPOSITION_ABI_VERSION_V1;
  snapshot.struct_size = sizeof(snapshot);
  snapshot.texture_hresult = Result(E_PENDING);
  snapshot.dxgi_resource_hresult = Result(E_PENDING);
  snapshot.shared_handle_hresult = Result(E_PENDING);
  snapshot.add_buffer_hresult = Result(E_PENDING);
  snapshot.available_event_hresult = Result(E_PENDING);
  snapshot.width = width;
  snapshot.height = height;
}

HRESULT AttachCompositionToWindow(
    VulkanCompositionContext& composition, HWND target_window) noexcept {
  if (target_window == nullptr || !IsWindow(target_window))
    return E_INVALIDARG;
  if (composition.composition_target) return HRESULT_FROM_WIN32(ERROR_ALREADY_EXISTS);
  if (!composition.device || composition.surface_handle == nullptr)
    return E_HANDLE;

  ComPtr<IDXGIDevice> dxgi_device;
  auto result = composition.device.As(&dxgi_device);
  if (FAILED(result)) return result;
  result = DCompositionCreateDevice(
      dxgi_device.Get(), IID_PPV_ARGS(&composition.composition_device));
  if (FAILED(result)) return result;
  result = composition.composition_device->CreateTargetForHwnd(
      target_window, TRUE, &composition.composition_target);
  if (FAILED(result)) return result;
  result = composition.composition_device->CreateVisual(
      &composition.composition_root);
  if (FAILED(result)) return result;
  result = composition.composition_device->CreateSurfaceFromHandle(
      composition.surface_handle, &composition.composition_surface);
  if (FAILED(result)) return result;
  result = composition.composition_root->SetContent(
      composition.composition_surface.Get());
  if (FAILED(result)) return result;
  result = composition.composition_target->SetRoot(
      composition.composition_root.Get());
  if (FAILED(result)) return result;
  result = composition.composition_device->Commit();
  if (FAILED(result)) return result;
  return composition.composition_device->WaitForCommitCompletion();
}

HRESULT FindHardwareAdapter(
    int32_t requested_luid_low, int32_t requested_luid_high,
    VulkanCompositionContext& composition,
    doroti_windows_vulkan_composition_probe_v1& snapshot) noexcept {
  ComPtr<IDXGIFactory1> dxgi_factory;
  auto result = CreateDXGIFactory1(IID_PPV_ARGS(&dxgi_factory));
  snapshot.dxgi_factory_hresult = Result(result);
  if (FAILED(result)) return result;

  for (UINT index = 0;; ++index) {
    ComPtr<IDXGIAdapter1> candidate;
    result = dxgi_factory->EnumAdapters1(index, &candidate);
    snapshot.adapter_enumeration_hresult = Result(result);
    if (result == DXGI_ERROR_NOT_FOUND) return result;
    if (FAILED(result)) return result;

    DXGI_ADAPTER_DESC1 description{};
    result = candidate->GetDesc1(&description);
    snapshot.adapter_enumeration_hresult = Result(result);
    if (FAILED(result)) return result;
    if (static_cast<int32_t>(description.AdapterLuid.LowPart) !=
            requested_luid_low ||
        description.AdapterLuid.HighPart != requested_luid_high)
      continue;

    snapshot.actual_adapter_luid_low =
        static_cast<int32_t>(description.AdapterLuid.LowPart);
    snapshot.actual_adapter_luid_high = description.AdapterLuid.HighPart;
    snapshot.adapter_vendor_id = description.VendorId;
    snapshot.adapter_device_id = description.DeviceId;
    snapshot.adapter_flags = description.Flags;
    snapshot.adapter_luid_matched = 1;
    if ((description.Flags & DXGI_ADAPTER_FLAG_SOFTWARE) != 0)
      return DXGI_ERROR_UNSUPPORTED;
    composition.adapter = candidate;
    return S_OK;
  }
}

HRESULT CreateExactDevice(
    VulkanCompositionContext& composition,
    doroti_windows_vulkan_composition_probe_v1& snapshot) noexcept {
  constexpr D3D_FEATURE_LEVEL feature_levels[]{
      D3D_FEATURE_LEVEL_11_1,
      D3D_FEATURE_LEVEL_11_0,
  };
  D3D_FEATURE_LEVEL selected_feature_level{};
  const auto flags = static_cast<UINT>(D3D11_CREATE_DEVICE_BGRA_SUPPORT);
  auto result = D3D11CreateDevice(
      composition.adapter.Get(), D3D_DRIVER_TYPE_UNKNOWN, nullptr, flags,
      feature_levels, static_cast<UINT>(std::size(feature_levels)),
      D3D11_SDK_VERSION, &composition.device, &selected_feature_level, nullptr);
  if (result == E_INVALIDARG) {
    result = D3D11CreateDevice(
        composition.adapter.Get(), D3D_DRIVER_TYPE_UNKNOWN, nullptr, flags,
        &feature_levels[1], 1, D3D11_SDK_VERSION, &composition.device,
        &selected_feature_level, nullptr);
  }
  snapshot.d3d11_device_hresult = Result(result);
  if (FAILED(result)) return result;
  snapshot.device_creation_flags = composition.device->GetCreationFlags();
  snapshot.device_feature_level = static_cast<uint32_t>(selected_feature_level);

  // Verify the device's actual adapter rather than trusting enumeration alone.
  ComPtr<IDXGIDevice> dxgi_device;
  result = composition.device.As(&dxgi_device);
  if (FAILED(result)) {
    snapshot.d3d11_device_hresult = Result(result);
    return result;
  }
  ComPtr<IDXGIAdapter> actual_adapter;
  result = dxgi_device->GetAdapter(&actual_adapter);
  if (FAILED(result)) {
    snapshot.d3d11_device_hresult = Result(result);
    return result;
  }
  DXGI_ADAPTER_DESC actual_description{};
  result = actual_adapter->GetDesc(&actual_description);
  if (FAILED(result)) {
    snapshot.d3d11_device_hresult = Result(result);
    return result;
  }
  const auto actual_low =
      static_cast<int32_t>(actual_description.AdapterLuid.LowPart);
  const auto actual_high = actual_description.AdapterLuid.HighPart;
  snapshot.actual_adapter_luid_low = actual_low;
  snapshot.actual_adapter_luid_high = actual_high;
  snapshot.adapter_luid_matched =
      actual_low == snapshot.requested_adapter_luid_low &&
              actual_high == snapshot.requested_adapter_luid_high
          ? 1u
          : 0u;
  if (snapshot.adapter_luid_matched == 0) {
    snapshot.d3d11_device_hresult = Result(DXGI_ERROR_INVALID_CALL);
    return DXGI_ERROR_INVALID_CALL;
  }
  return S_OK;
}

HRESULT IsAvailable(const BufferSlot& slot, bool& available) noexcept {
  available = false;
  if (!slot.buffer) return S_FALSE;
  boolean value{};
  const auto result = slot.buffer->IsAvailable(&value);
  if (SUCCEEDED(result)) available = value != 0;
  return result;
}

HRESULT SetRetainedSource(
    VulkanCompositionContext& composition, uint32_t source_x,
    uint32_t source_y, uint32_t source_width,
    uint32_t source_height, uint64_t tag) noexcept {
  // The Presentation surface stays at identity on the top-level target. The
  // top-level client is the only viewport clip and managed raster replacements
  // use the same retained-capacity origin.
  RECT source{static_cast<LONG>(source_x), static_cast<LONG>(source_y),
              static_cast<LONG>(source_x + source_width),
              static_cast<LONG>(source_y + source_height)};
  auto result = composition.surface->SetSourceRect(&source);
  if (FAILED(result)) return result;
  PresentationTransform identity{1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f};
  result = composition.surface->SetTransform(&identity);
  if (FAILED(result)) return result;
  composition.surface->SetTag(static_cast<UINT_PTR>(tag));
  return S_OK;
}

HRESULT WaitForCompositionFrame(
    VulkanCompositionContext& composition, uint64_t present_id,
    uint64_t tag, uint32_t timeout_ms, bool& observed) noexcept {
  observed = false;
  if (composition.present_statistics_event == nullptr) return E_HANDLE;
  const auto deadline = GetTickCount64() + timeout_ms;
  while (true) {
    const auto now = GetTickCount64();
    if (now >= deadline) return S_OK;
    const auto remaining = static_cast<DWORD>(
        std::min<ULONGLONG>(deadline - now, MAXDWORD));
    const auto wait = WaitForSingleObject(
        composition.present_statistics_event, remaining);
    if (wait == WAIT_OBJECT_0) {
      do {
        ComPtr<IPresentStatistics> statistics;
        const auto result =
            composition.manager->GetNextPresentStatistics(&statistics);
        if (FAILED(result)) {
          std::fprintf(stderr,
                       "doroti.windows.composition_stats stage=get-next "
                       "hresult=0x%08X present=%llu tag=%llu\n",
                       static_cast<unsigned int>(result),
                       static_cast<unsigned long long>(present_id),
                       static_cast<unsigned long long>(tag));
          std::fflush(stderr);
          return result;
        }
        if (!statistics) break;
        if (statistics->GetKind() != PresentStatisticsKind_CompositionFrame)
          continue;

        ComPtr<ICompositionFramePresentStatistics> composition_frame;
        const auto query_result = statistics.As(&composition_frame);
        if (FAILED(query_result)) {
          std::fprintf(stderr,
                       "doroti.windows.composition_stats stage=query-frame "
                       "hresult=0x%08X present=%llu tag=%llu\n",
                       static_cast<unsigned int>(query_result),
                       static_cast<unsigned long long>(present_id),
                       static_cast<unsigned long long>(tag));
          std::fflush(stderr);
          return query_result;
        }
        UINT instance_count{};
        const CompositionFrameDisplayInstance* instances{};
        composition_frame->GetDisplayInstanceArray(
            &instance_count, &instances);
        if (statistics->GetPresentId() == present_id &&
            composition_frame->GetContentTag() ==
                static_cast<UINT_PTR>(tag) &&
            instance_count != 0 && instances != nullptr) {
          observed = true;
          return S_OK;
        }
      } while (WaitForSingleObject(
                   composition.present_statistics_event, 0) == WAIT_OBJECT_0);
      continue;
    }
    if (wait == WAIT_TIMEOUT) return S_OK;
    if (wait == WAIT_FAILED) return HRESULT_FROM_WIN32(GetLastError());
    return E_UNEXPECTED;
  }
}

}  // namespace

extern "C" int32_t DOROTI_WINDOWS_VULKAN_COMPOSITION_CALL
doroti_windows_vulkan_composition_create_v1(
    int32_t adapter_luid_low, int32_t adapter_luid_high, void** context,
    uint64_t* composition_surface_handle,
    doroti_windows_vulkan_composition_probe_v1* snapshot) {
  if (context == nullptr || composition_surface_handle == nullptr ||
      snapshot == nullptr ||
      snapshot->struct_size <
          sizeof(doroti_windows_vulkan_composition_probe_v1))
    return Result(E_INVALIDARG);

  *context = nullptr;
  *composition_surface_handle = 0;
  InitializeProbe(*snapshot, adapter_luid_low, adapter_luid_high);
  auto composition = std::make_unique<VulkanCompositionContext>();

  auto result = FindHardwareAdapter(adapter_luid_low, adapter_luid_high,
                                    *composition, *snapshot);
  if (FAILED(result)) return Result(result);
  result = CreateExactDevice(*composition, *snapshot);
  if (FAILED(result)) return Result(result);

  result = CreatePresentationFactory(
      composition->device.Get(), IID_PPV_ARGS(&composition->factory));
  snapshot->presentation_factory_hresult = Result(result);
  if (FAILED(result)) return Result(result);
  snapshot->presentation_supported =
      composition->factory->IsPresentationSupported() ? 1u : 0u;
  snapshot->independent_flip_supported =
      composition->factory->IsPresentationSupportedWithIndependentFlip()
          ? 1u
          : 0u;
  if (snapshot->presentation_supported == 0) return Result(S_FALSE);

  result = composition->factory->CreatePresentationManager(
      &composition->manager);
  snapshot->presentation_manager_hresult = Result(result);
  if (FAILED(result)) return Result(result);
  result = composition->manager->EnablePresentStatisticsKind(
      PresentStatisticsKind_CompositionFrame, TRUE);
  if (FAILED(result)) {
    snapshot->presentation_manager_hresult = Result(result);
    return Result(result);
  }
  result = composition->manager->GetPresentStatisticsAvailableEvent(
      &composition->present_statistics_event);
  if (FAILED(result)) {
    snapshot->presentation_manager_hresult = Result(result);
    return Result(result);
  }
  result = DCompositionCreateSurfaceHandle(
      COMPOSITIONOBJECT_ALL_ACCESS, nullptr, &composition->surface_handle);
  snapshot->surface_handle_hresult = Result(result);
  if (FAILED(result)) return Result(result);
  result = composition->manager->CreatePresentationSurface(
      composition->surface_handle, &composition->surface);
  snapshot->presentation_surface_hresult = Result(result);
  if (FAILED(result)) return Result(result);
  result = composition->manager->GetPresentRetiringFence(
      IID_PPV_ARGS(&composition->retiring_fence));
  snapshot->retiring_fence_hresult = Result(result);
  if (SUCCEEDED(result) && composition->retiring_fence) {
    snapshot->retiring_fence_completed_value =
        composition->retiring_fence->GetCompletedValue();
  }

  *composition_surface_handle =
      reinterpret_cast<uint64_t>(composition->surface_handle);
  *context = composition.release();
  return Result(S_OK);
}

extern "C" void DOROTI_WINDOWS_VULKAN_COMPOSITION_CALL
doroti_windows_vulkan_composition_destroy_v1(void* context) {
  delete static_cast<VulkanCompositionContext*>(context);
}

extern "C" int32_t DOROTI_WINDOWS_VULKAN_COMPOSITION_CALL
doroti_windows_vulkan_composition_attach_window_v1(
    void* context, uint64_t target_window) {
  if (context == nullptr || target_window == 0) return Result(E_INVALIDARG);
  auto& composition = *static_cast<VulkanCompositionContext*>(context);
  std::lock_guard lock(composition.gate);
  return Result(AttachCompositionToWindow(
      composition, reinterpret_cast<HWND>(target_window)));
}

extern "C" int32_t DOROTI_WINDOWS_VULKAN_COMPOSITION_CALL
doroti_windows_vulkan_composition_set_premultiplied_alpha_v1(
    void* context, uint32_t enabled) {
  if (context == nullptr || enabled > 1) return Result(E_INVALIDARG);
  auto& composition = *static_cast<VulkanCompositionContext*>(context);
  std::lock_guard lock(composition.gate);
  if (composition.bound_slot < kBufferCount)
    return Result(HRESULT_FROM_WIN32(ERROR_INVALID_STATE));
  composition.alpha_mode = enabled != 0
                               ? DXGI_ALPHA_MODE_PREMULTIPLIED
                               : DXGI_ALPHA_MODE_IGNORE;
  return Result(S_OK);
}

extern "C" int32_t DOROTI_WINDOWS_VULKAN_COMPOSITION_CALL
doroti_windows_vulkan_composition_replace_buffer_v1(
    void* context, uint32_t slot_index, uint32_t width, uint32_t height,
    uint64_t* shared_texture_handle, uint64_t* available_event,
    doroti_windows_vulkan_composition_buffer_v1* snapshot) {
  if (context == nullptr || slot_index >= kBufferCount || width == 0 ||
      height == 0 || shared_texture_handle == nullptr ||
      available_event == nullptr || snapshot == nullptr ||
      snapshot->struct_size <
          sizeof(doroti_windows_vulkan_composition_buffer_v1))
    return Result(E_INVALIDARG);

  *shared_texture_handle = 0;
  *available_event = 0;
  InitializeBufferSnapshot(*snapshot, width, height);
  auto& composition = *static_cast<VulkanCompositionContext*>(context);
  std::lock_guard lock(composition.gate);
  if (!composition.device || !composition.manager || !composition.surface)
    return Result(E_HANDLE);

  auto& slot = composition.slots[slot_index];
  if (slot.buffer) {
    bool available{};
    const auto available_result = IsAvailable(slot, available);
    if (FAILED(available_result)) return Result(available_result);
    if (!available) return Result(DXGI_ERROR_WAS_STILL_DRAWING);
    slot.Reset();
    if (composition.bound_slot == slot_index) {
      composition.bound_slot = kBufferCount;
      composition.bound_width = 0;
      composition.bound_height = 0;
    }
  }

  D3D11_TEXTURE2D_DESC description{};
  description.Width = width;
  description.Height = height;
  description.MipLevels = 1;
  description.ArraySize = 1;
  description.Format = DXGI_FORMAT_B8G8R8A8_UNORM;
  description.SampleDesc.Count = 1;
  description.Usage = D3D11_USAGE_DEFAULT;
  description.BindFlags =
      D3D11_BIND_SHADER_RESOURCE | D3D11_BIND_RENDER_TARGET;
  description.MiscFlags =
      D3D11_RESOURCE_MISC_SHARED | D3D11_RESOURCE_MISC_SHARED_NTHANDLE;
  snapshot->format = static_cast<uint32_t>(description.Format);
  snapshot->bind_flags = description.BindFlags;
  snapshot->misc_flags = description.MiscFlags;

  auto result = composition.device->CreateTexture2D(
      &description, nullptr, &slot.texture);
  snapshot->texture_hresult = Result(result);
  if (FAILED(result)) return Result(result);

  ComPtr<IDXGIResource1> shared_resource;
  result = slot.texture.As(&shared_resource);
  snapshot->dxgi_resource_hresult = Result(result);
  if (FAILED(result)) {
    slot.Reset();
    return Result(result);
  }
  HANDLE exported_handle{};
  result = shared_resource->CreateSharedHandle(
      nullptr, DXGI_SHARED_RESOURCE_READ | DXGI_SHARED_RESOURCE_WRITE,
      nullptr, &exported_handle);
  snapshot->shared_handle_hresult = Result(result);
  if (FAILED(result)) {
    slot.Reset();
    return Result(result);
  }

  result = composition.manager->AddBufferFromResource(
      slot.texture.Get(), &slot.buffer);
  snapshot->add_buffer_hresult = Result(result);
  if (FAILED(result)) {
    CloseHandle(exported_handle);
    slot.Reset();
    return Result(result);
  }
  result = slot.buffer->GetAvailableEvent(&slot.available_event);
  snapshot->available_event_hresult = Result(result);
  if (FAILED(result)) {
    CloseHandle(exported_handle);
    slot.Reset();
    return Result(result);
  }
  bool available{};
  result = IsAvailable(slot, available);
  if (FAILED(result)) {
    CloseHandle(exported_handle);
    slot.Reset();
    return Result(result);
  }
  snapshot->initially_available = available ? 1u : 0u;
  slot.width = width;
  slot.height = height;
  *shared_texture_handle = reinterpret_cast<uint64_t>(exported_handle);
  *available_event = reinterpret_cast<uint64_t>(slot.available_event);
  return Result(S_OK);
}

extern "C" int32_t DOROTI_WINDOWS_VULKAN_COMPOSITION_CALL
doroti_windows_vulkan_composition_is_available_v1(
    void* context, uint32_t slot_index, uint32_t* available) {
  if (context == nullptr || slot_index >= kBufferCount || available == nullptr)
    return Result(E_INVALIDARG);
  *available = 0;
  auto& composition = *static_cast<VulkanCompositionContext*>(context);
  std::lock_guard lock(composition.gate);
  bool value{};
  const auto result = IsAvailable(composition.slots[slot_index], value);
  *available = value ? 1u : 0u;
  return Result(result);
}

extern "C" int32_t DOROTI_WINDOWS_VULKAN_COMPOSITION_CALL
doroti_windows_vulkan_composition_present_cropped_v1(
    void* context, uint32_t slot_index, uint32_t source_x,
    uint32_t source_y, uint32_t width, uint32_t height, uint64_t tag,
    uint32_t wait_for_composition_frame, uint32_t wait_timeout_ms,
    uint32_t* composition_frame_observed,
    uint64_t* present_id, uint64_t* retiring_fence_value) {
  // Managed caller enters only after exact raster and synchronous copy finish.
  doroti::resize_trace::Record("render-copy-complete-handoff",
                               doroti::resize_trace::render_key);
  if (context == nullptr || slot_index >= kBufferCount || width == 0 ||
      height == 0 || composition_frame_observed == nullptr ||
      present_id == nullptr || retiring_fence_value == nullptr)
    return Result(E_INVALIDARG);
  *composition_frame_observed = 0;
  *present_id = 0;
  *retiring_fence_value = 0;
  auto& composition = *static_cast<VulkanCompositionContext*>(context);
  std::lock_guard lock(composition.gate);
  if (!composition.manager || !composition.surface) return Result(E_HANDLE);
  auto& slot = composition.slots[slot_index];
  if (!slot.buffer || source_x > slot.width || source_y > slot.height ||
      width > slot.width - source_x || height > slot.height - source_y)
    return Result(E_INVALIDARG);

  bool available{};
  auto result = IsAvailable(slot, available);
  if (FAILED(result)) return Result(result);
  if (!available) return Result(DXGI_ERROR_WAS_STILL_DRAWING);
  result = composition.surface->SetBuffer(slot.buffer.Get());
  if (FAILED(result)) return Result(result);
  result = composition.surface->SetColorSpace(
      DXGI_COLOR_SPACE_RGB_FULL_G22_NONE_P709);
  if (FAILED(result)) return Result(result);
  result = composition.surface->SetAlphaMode(composition.alpha_mode);
  if (FAILED(result)) return Result(result);
  result = SetRetainedSource(
      composition, source_x, source_y, width, height, tag);
  if (FAILED(result)) return Result(result);

  const auto id = composition.manager->GetNextPresentId();
  doroti::resize_trace::Record("present-call", doroti::resize_trace::render_key);
  result = composition.manager->Present();
  doroti::resize_trace::Record("present-return", doroti::resize_trace::render_key);
  if (FAILED(result)) return Result(result);
  composition.bound_slot = slot_index;
  composition.bound_width = slot.width;
  composition.bound_height = slot.height;
  *present_id = id;
  *retiring_fence_value = composition.retiring_fence
                               ? composition.retiring_fence->GetCompletedValue()
                               : 0;
  if (wait_for_composition_frame != 0) {
    if (composition.composition_target && wait_for_composition_frame == 1) {
      // The product path attaches the Presentation surface to a native topmost
      // target on the top-level HWND. DwmFlush observes its commit at a DWM boundary.
      result = DwmFlush();
      if (FAILED(result)) return Result(result);
      *composition_frame_observed = 1;
    } else {
      bool observed{};
      result = WaitForCompositionFrame(
          composition, id, tag, wait_timeout_ms, observed);
      if (FAILED(result)) return Result(result);
      *composition_frame_observed = observed ? 1u : 0u;
      if (wait_for_composition_frame == 2)
        doroti::resize_trace::Record("present-receipt", doroti::resize_trace::render_key,
                                     observed ? 1u : 0u);
    }
  }
  return Result(S_OK);
}

extern "C" int32_t DOROTI_WINDOWS_VULKAN_COMPOSITION_CALL
doroti_windows_vulkan_composition_crop_v1(
    void* context, uint32_t source_x, uint32_t source_y,
    uint32_t width, uint32_t height, uint64_t tag) {
  if (context == nullptr || width == 0 || height == 0)
    return Result(E_INVALIDARG);
  auto& composition = *static_cast<VulkanCompositionContext*>(context);
  std::lock_guard lock(composition.gate);
  if (!composition.manager || !composition.surface ||
      composition.bound_slot >= kBufferCount ||
      source_x > composition.bound_width ||
      source_y > composition.bound_height ||
      width > composition.bound_width - source_x ||
      height > composition.bound_height - source_y)
    return Result(E_INVALIDARG);
  auto result = SetRetainedSource(
      composition, source_x, source_y, width, height, tag);
  if (FAILED(result)) return Result(result);
  return Result(composition.manager->Present());
}

extern "C" int32_t DOROTI_WINDOWS_VULKAN_COMPOSITION_CALL
doroti_windows_vulkan_composition_retire_buffers_v1(
    void* context, uint64_t tag, uint64_t* present_id) {
  if (context == nullptr || present_id == nullptr) return Result(E_INVALIDARG);
  *present_id = 0;
  auto& composition = *static_cast<VulkanCompositionContext*>(context);
  std::lock_guard lock(composition.gate);
  if (!composition.manager || !composition.surface) return Result(E_HANDLE);

  // A displayed Presentation buffer remains unavailable until a later present
  // replaces it. Use a native-only 1x1 buffer for that successor so all three
  // Vulkan-imported buffers can become available before their VkDeviceMemory
  // is released. The drain buffer itself lives until context destruction.
  auto result = S_OK;
  if (!composition.retirement_buffer) {
    D3D11_TEXTURE2D_DESC description{};
    description.Width = 1;
    description.Height = 1;
    description.MipLevels = 1;
    description.ArraySize = 1;
    description.Format = DXGI_FORMAT_B8G8R8A8_UNORM;
    description.SampleDesc.Count = 1;
    description.Usage = D3D11_USAGE_DEFAULT;
    description.BindFlags =
        D3D11_BIND_SHADER_RESOURCE | D3D11_BIND_RENDER_TARGET;
    description.MiscFlags =
        D3D11_RESOURCE_MISC_SHARED | D3D11_RESOURCE_MISC_SHARED_NTHANDLE;
    ComPtr<ID3D11Texture2D> retirement_texture;
    result = composition.device->CreateTexture2D(
        &description, nullptr, &retirement_texture);
    if (FAILED(result)) return Result(result);
    ComPtr<ID3D11RenderTargetView> view;
    result = composition.device->CreateRenderTargetView(
        retirement_texture.Get(), nullptr, &view);
    if (FAILED(result)) return Result(result);
    ComPtr<ID3D11DeviceContext> device_context;
    composition.device->GetImmediateContext(&device_context);
    constexpr float transparent[4]{0.0f, 0.0f, 0.0f, 0.0f};
    constexpr float opaque_black[4]{0.0f, 0.0f, 0.0f, 1.0f};
    device_context->ClearRenderTargetView(
        view.Get(), composition.alpha_mode == DXGI_ALPHA_MODE_PREMULTIPLIED
                        ? transparent
                        : opaque_black);
    device_context->Flush();
    ComPtr<IPresentationBuffer> retirement_buffer;
    result = composition.manager->AddBufferFromResource(
        retirement_texture.Get(), &retirement_buffer);
    if (FAILED(result)) return Result(result);
    composition.retirement_texture = retirement_texture;
    composition.retirement_buffer = retirement_buffer;
  }
  result = composition.surface->SetBuffer(composition.retirement_buffer.Get());
  if (FAILED(result)) return Result(result);
  result = composition.surface->SetColorSpace(
      DXGI_COLOR_SPACE_RGB_FULL_G22_NONE_P709);
  if (FAILED(result)) return Result(result);
  result = composition.surface->SetAlphaMode(composition.alpha_mode);
  if (FAILED(result)) return Result(result);
  RECT source{0, 0, 1, 1};
  result = composition.surface->SetSourceRect(&source);
  if (FAILED(result)) return Result(result);
  PresentationTransform identity{1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f};
  result = composition.surface->SetTransform(&identity);
  if (FAILED(result)) return Result(result);
  composition.surface->SetTag(static_cast<UINT_PTR>(tag));
  const auto id = composition.manager->GetNextPresentId();
  result = composition.manager->Present();
  if (FAILED(result)) return Result(result);
  composition.bound_slot = kBufferCount;
  composition.bound_width = 0;
  composition.bound_height = 0;
  *present_id = id;
  return Result(S_OK);
}
