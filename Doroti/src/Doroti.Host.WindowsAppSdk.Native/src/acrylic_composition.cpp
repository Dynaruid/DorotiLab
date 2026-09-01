#include "doroti_windows_acrylic_composition_v1.h"

#include <windows.h>
#include <Presentation.h>
#include <d3d11_4.h>
#include <dcomp.h>
#include <dxgi1_6.h>
#include <wrl/client.h>

#include <cstdint>
#include <cmath>
#include <memory>
#include <mutex>

using Microsoft::WRL::ComPtr;

namespace {

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

struct AcrylicContext {
  std::mutex gate;
  ComPtr<ID3D11Device> device;
  ComPtr<IPresentationFactory> factory;
  ComPtr<IPresentationManager> manager;
  ComPtr<IPresentationSurface> surface;
  ComPtr<ID3D11Fence> retiring_fence;
  HANDLE surface_handle{};
  BufferSlot slots[3];
  uint32_t bound_width{};
  uint32_t bound_height{};

  ~AcrylicContext() {
    retiring_fence.Reset();
    surface.Reset();
    manager.Reset();
    factory.Reset();
    device.Reset();
    if (surface_handle != nullptr) CloseHandle(surface_handle);
  }
};

HRESULT IsAvailable(const BufferSlot& slot, bool& available) noexcept {
  available = false;
  if (!slot.buffer) return S_FALSE;
  boolean value{};
  const auto result = slot.buffer->IsAvailable(&value);
  if (SUCCEEDED(result)) available = value != 0;
  return result;
}

HRESULT PresentBufferLocked(
    AcrylicContext& acrylic, uint32_t slot_index, uint32_t width,
    uint32_t height, uint32_t source_x, uint32_t source_y,
    float offset_x, float offset_y, uint64_t tag,
    uint64_t* present_id, uint64_t* retiring_fence_value) noexcept {
  auto& slot = acrylic.slots[slot_index];
  if (!slot.buffer || width == 0 || height == 0 ||
      source_x > slot.width || source_y > slot.height ||
      width > slot.width - source_x || height > slot.height - source_y ||
      !std::isfinite(offset_x) || !std::isfinite(offset_y))
    return E_INVALIDARG;
  bool available{};
  auto result = IsAvailable(slot, available);
  if (FAILED(result)) return result;
  if (!available) return DXGI_ERROR_WAS_STILL_DRAWING;
  // Per-buffer availability is the reuse authority. The retiring fence is
  // sampled below for diagnostics only and never blocks the raster worker.
  result = acrylic.surface->SetBuffer(slot.buffer.Get());
  if (FAILED(result)) return result;
  result =
      acrylic.surface->SetColorSpace(DXGI_COLOR_SPACE_RGB_FULL_G22_NONE_P709);
  if (FAILED(result)) return result;
  result = acrylic.surface->SetAlphaMode(DXGI_ALPHA_MODE_PREMULTIPLIED);
  if (FAILED(result)) return result;
  RECT source{
      static_cast<LONG>(source_x), static_cast<LONG>(source_y),
      static_cast<LONG>(source_x + width),
      static_cast<LONG>(source_y + height)};
  result = acrylic.surface->SetSourceRect(&source);
  if (FAILED(result)) return result;
  PresentationTransform transform{
      1.0f, 0.0f, 0.0f, 1.0f, offset_x, offset_y};
  result = acrylic.surface->SetTransform(&transform);
  if (FAILED(result)) return result;
  acrylic.surface->SetTag(static_cast<UINT_PTR>(tag));
  const auto id = acrylic.manager->GetNextPresentId();
  result = acrylic.manager->Present();
  if (FAILED(result)) return result;
  acrylic.bound_width = slot.width;
  acrylic.bound_height = slot.height;
  *present_id = id;
  *retiring_fence_value = acrylic.retiring_fence
                                ? acrylic.retiring_fence->GetCompletedValue()
                                : 0;
  return S_OK;
}

void QueryAdapter(ID3D11Device* device,
                  doroti_windows_acrylic_probe_v1& snapshot) noexcept {
  ComPtr<IDXGIDevice> dxgi_device;
  if (FAILED(device->QueryInterface(IID_PPV_ARGS(&dxgi_device)))) return;
  ComPtr<IDXGIAdapter> adapter;
  if (FAILED(dxgi_device->GetAdapter(&adapter))) return;
  DXGI_ADAPTER_DESC description{};
  if (FAILED(adapter->GetDesc(&description))) return;
  snapshot.adapter_luid_low = static_cast<int32_t>(description.AdapterLuid.LowPart);
  snapshot.adapter_luid_high = description.AdapterLuid.HighPart;
  snapshot.adapter_vendor_id = description.VendorId;
  snapshot.adapter_device_id = description.DeviceId;
}

}  // namespace

extern "C" int32_t DOROTI_WINDOWS_ACRYLIC_CALL
doroti_windows_acrylic_create_v1(
    void* d3d11_device, void** context, uint64_t* composition_surface_handle,
    doroti_windows_acrylic_probe_v1* snapshot) {
  if (d3d11_device == nullptr || context == nullptr ||
      composition_surface_handle == nullptr || snapshot == nullptr ||
      snapshot->struct_size < sizeof(doroti_windows_acrylic_probe_v1))
    return E_INVALIDARG;

  *context = nullptr;
  *composition_surface_handle = 0;
  auto acrylic = std::make_unique<AcrylicContext>();
  auto* device = static_cast<ID3D11Device*>(d3d11_device);
  acrylic->device = device;
  snapshot->abi_version = 1;
  snapshot->struct_size = sizeof(doroti_windows_acrylic_probe_v1);
  snapshot->device_creation_flags = device->GetCreationFlags();
  QueryAdapter(device, *snapshot);

  snapshot->factory_hresult =
      CreatePresentationFactory(device, IID_PPV_ARGS(&acrylic->factory));
  if (FAILED(snapshot->factory_hresult)) return snapshot->factory_hresult;
  snapshot->presentation_supported =
      acrylic->factory->IsPresentationSupported() ? 1u : 0u;
  snapshot->independent_flip_supported =
      acrylic->factory->IsPresentationSupportedWithIndependentFlip() ? 1u : 0u;
  if (snapshot->presentation_supported == 0) return S_FALSE;

  snapshot->manager_hresult =
      acrylic->factory->CreatePresentationManager(&acrylic->manager);
  if (FAILED(snapshot->manager_hresult)) return snapshot->manager_hresult;
  snapshot->surface_handle_hresult = DCompositionCreateSurfaceHandle(
      COMPOSITIONOBJECT_ALL_ACCESS, nullptr, &acrylic->surface_handle);
  if (FAILED(snapshot->surface_handle_hresult))
    return snapshot->surface_handle_hresult;
  snapshot->presentation_surface_hresult =
      acrylic->manager->CreatePresentationSurface(acrylic->surface_handle,
                                                   &acrylic->surface);
  if (FAILED(snapshot->presentation_surface_hresult))
    return snapshot->presentation_surface_hresult;
  snapshot->retiring_fence_hresult = acrylic->manager->GetPresentRetiringFence(
      IID_PPV_ARGS(&acrylic->retiring_fence));
  if (SUCCEEDED(snapshot->retiring_fence_hresult) && acrylic->retiring_fence) {
    snapshot->retiring_fence_completed_value =
        acrylic->retiring_fence->GetCompletedValue();
  }

  *composition_surface_handle =
      reinterpret_cast<uint64_t>(acrylic->surface_handle);
  *context = acrylic.release();
  return S_OK;
}

extern "C" void DOROTI_WINDOWS_ACRYLIC_CALL
doroti_windows_acrylic_destroy_v1(void* context) {
  delete static_cast<AcrylicContext*>(context);
}

extern "C" int32_t DOROTI_WINDOWS_ACRYLIC_CALL
doroti_windows_acrylic_replace_buffer_v1(
    void* context, uint32_t slot_index, uint32_t width, uint32_t height,
    void** texture, uint64_t* available_event,
    doroti_windows_acrylic_buffer_v1* snapshot) {
  if (context == nullptr || slot_index >= 3 || width == 0 || height == 0 ||
      texture == nullptr || available_event == nullptr || snapshot == nullptr ||
      snapshot->struct_size < sizeof(doroti_windows_acrylic_buffer_v1))
    return E_INVALIDARG;

  auto& acrylic = *static_cast<AcrylicContext*>(context);
  std::lock_guard lock(acrylic.gate);
  auto& slot = acrylic.slots[slot_index];
  if (!acrylic.manager || !acrylic.surface) return E_HANDLE;
  if (slot.buffer) {
    bool available{};
    const auto available_result = IsAvailable(slot, available);
    if (FAILED(available_result)) return available_result;
    if (!available) return DXGI_ERROR_WAS_STILL_DRAWING;
    slot.Reset();
  }

  *texture = nullptr;
  *available_event = 0;
  snapshot->abi_version = 1;
  snapshot->struct_size = sizeof(doroti_windows_acrylic_buffer_v1);
  snapshot->width = width;
  snapshot->height = height;
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
  snapshot->format = description.Format;
  snapshot->bind_flags = description.BindFlags;
  snapshot->misc_flags = description.MiscFlags;
  snapshot->texture_hresult =
      acrylic.device->CreateTexture2D(&description, nullptr, &slot.texture);
  if (FAILED(snapshot->texture_hresult)) return snapshot->texture_hresult;
  // Presentation can sample a narrow guard region while the framework catches
  // up with a moving edge. Initialize that region to transparent once; ANGLE
  // renders only into the inset 1:1 viewport and never stretches its pixels.
  ComPtr<ID3D11RenderTargetView> clear_view;
  auto clear_result = acrylic.device->CreateRenderTargetView(
      slot.texture.Get(), nullptr, &clear_view);
  if (FAILED(clear_result)) {
    slot.Reset();
    return clear_result;
  }
  ComPtr<ID3D11DeviceContext> immediate;
  acrylic.device->GetImmediateContext(&immediate);
  if (!immediate) {
    slot.Reset();
    return E_HANDLE;
  }
  constexpr float transparent[4]{0.0f, 0.0f, 0.0f, 0.0f};
  immediate->ClearRenderTargetView(clear_view.Get(), transparent);
  immediate->Flush();
  snapshot->add_buffer_hresult =
      acrylic.manager->AddBufferFromResource(slot.texture.Get(), &slot.buffer);
  if (FAILED(snapshot->add_buffer_hresult)) {
    slot.Reset();
    return snapshot->add_buffer_hresult;
  }
  snapshot->available_event_hresult =
      slot.buffer->GetAvailableEvent(&slot.available_event);
  if (FAILED(snapshot->available_event_hresult)) {
    slot.Reset();
    return snapshot->available_event_hresult;
  }
  bool available{};
  const auto available_result = IsAvailable(slot, available);
  if (FAILED(available_result)) {
    slot.Reset();
    return available_result;
  }
  snapshot->initially_available = available ? 1u : 0u;
  slot.width = width;
  slot.height = height;
  slot.texture->AddRef();
  *texture = slot.texture.Get();
  *available_event = reinterpret_cast<uint64_t>(slot.available_event);
  return S_OK;
}

extern "C" int32_t DOROTI_WINDOWS_ACRYLIC_CALL
doroti_windows_acrylic_is_available_v1(
    void* context, uint32_t slot_index, uint32_t* available) {
  if (context == nullptr || slot_index >= 3 || available == nullptr)
    return E_INVALIDARG;
  bool value{};
  auto& acrylic = *static_cast<AcrylicContext*>(context);
  std::lock_guard lock(acrylic.gate);
  const auto result = IsAvailable(acrylic.slots[slot_index], value);
  *available = value ? 1u : 0u;
  return result;
}

extern "C" int32_t DOROTI_WINDOWS_ACRYLIC_CALL
doroti_windows_acrylic_present_v1(
    void* context, uint32_t slot_index, uint32_t width, uint32_t height,
    uint64_t tag, uint64_t* present_id, uint64_t* retiring_fence_value) {
  if (context == nullptr || slot_index >= 3 || present_id == nullptr ||
      retiring_fence_value == nullptr)
    return E_INVALIDARG;
  auto& acrylic = *static_cast<AcrylicContext*>(context);
  std::lock_guard lock(acrylic.gate);
  return PresentBufferLocked(acrylic, slot_index, width, height, 0, 0,
                             0.0f, 0.0f, tag, present_id,
                             retiring_fence_value);
}

extern "C" int32_t DOROTI_WINDOWS_ACRYLIC_CALL
doroti_windows_acrylic_present_positioned_v1(
    void* context, uint32_t slot_index, uint32_t width, uint32_t height,
    float offset_x, float offset_y, uint64_t tag, uint64_t* present_id,
    uint64_t* retiring_fence_value) {
  if (context == nullptr || slot_index >= 3 || present_id == nullptr ||
      retiring_fence_value == nullptr)
    return E_INVALIDARG;
  auto& acrylic = *static_cast<AcrylicContext*>(context);
  std::lock_guard lock(acrylic.gate);
  return PresentBufferLocked(acrylic, slot_index, width, height, 0, 0,
                             offset_x, offset_y, tag, present_id,
                             retiring_fence_value);
}

extern "C" int32_t DOROTI_WINDOWS_ACRYLIC_CALL
doroti_windows_acrylic_present_cropped_v1(
    void* context, uint32_t slot_index, uint32_t source_x,
    uint32_t source_y, uint32_t width, uint32_t height, uint64_t tag,
    uint64_t* present_id, uint64_t* retiring_fence_value) {
  if (context == nullptr || slot_index >= 3 || present_id == nullptr ||
      retiring_fence_value == nullptr)
    return E_INVALIDARG;
  auto& acrylic = *static_cast<AcrylicContext*>(context);
  std::lock_guard lock(acrylic.gate);
  return PresentBufferLocked(acrylic, slot_index, width, height, source_x,
                             source_y, 0.0f, 0.0f, tag, present_id,
                             retiring_fence_value);
}

extern "C" int32_t DOROTI_WINDOWS_ACRYLIC_CALL
doroti_windows_acrylic_crop_v1(
    void* context, uint32_t source_x, uint32_t source_y,
    uint32_t width, uint32_t height, uint64_t tag) {
  if (context == nullptr || width == 0 || height == 0)
    return E_INVALIDARG;
  auto& acrylic = *static_cast<AcrylicContext*>(context);
  std::lock_guard lock(acrylic.gate);
  if (!acrylic.manager || !acrylic.surface ||
      source_x > acrylic.bound_width || source_y > acrylic.bound_height ||
      width > acrylic.bound_width - source_x ||
      height > acrylic.bound_height - source_y)
    return E_INVALIDARG;
  RECT source{
      static_cast<LONG>(source_x), static_cast<LONG>(source_y),
      static_cast<LONG>(source_x + width),
      static_cast<LONG>(source_y + height)};
  auto result = acrylic.surface->SetSourceRect(&source);
  if (FAILED(result)) return result;
  PresentationTransform identity{1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f};
  result = acrylic.surface->SetTransform(&identity);
  if (FAILED(result)) return result;
  acrylic.surface->SetTag(static_cast<UINT_PTR>(tag));
  return acrylic.manager->Present();
}

extern "C" int32_t DOROTI_WINDOWS_ACRYLIC_CALL
doroti_windows_acrylic_place_v1(
    void* context, float offset_x, float offset_y, uint64_t tag) {
  if (context == nullptr || !std::isfinite(offset_x) ||
      !std::isfinite(offset_y))
    return E_INVALIDARG;
  auto& acrylic = *static_cast<AcrylicContext*>(context);
  std::lock_guard lock(acrylic.gate);
  if (!acrylic.manager || !acrylic.surface) return E_HANDLE;
  PresentationTransform transform{
      1.0f, 0.0f, 0.0f, 1.0f, offset_x, offset_y};
  auto result = acrylic.surface->SetTransform(&transform);
  if (FAILED(result)) return result;
  acrylic.surface->SetTag(static_cast<UINT_PTR>(tag));
  return acrylic.manager->Present();
}
