#include "doroti_windows_acrylic_composition_v1.h"

#include <windows.h>
#include <Presentation.h>
#include <d3d11_4.h>
#include <dcomp.h>
#include <dxgi1_6.h>
#include <wrl/client.h>

#include <cstdint>
#include <memory>

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
  ComPtr<ID3D11Device> device;
  ComPtr<IPresentationFactory> factory;
  ComPtr<IPresentationManager> manager;
  ComPtr<IPresentationSurface> surface;
  ComPtr<ID3D11Fence> retiring_fence;
  HANDLE retiring_event{};
  HANDLE surface_handle{};
  uint64_t last_present_id{};
  uint32_t last_present_width{};
  uint32_t last_present_height{};
  uint32_t size_change_present_count{};
  BufferSlot slots[3];

  ~AcrylicContext() {
    if (retiring_event != nullptr) CloseHandle(retiring_event);
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
    acrylic->retiring_event = CreateEventW(nullptr, FALSE, FALSE, nullptr);
    if (acrylic->retiring_event == nullptr)
      return HRESULT_FROM_WIN32(GetLastError());
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
  const auto result = IsAvailable(
      static_cast<AcrylicContext*>(context)->slots[slot_index], value);
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
  auto& slot = acrylic.slots[slot_index];
  if (!slot.buffer || width == 0 || height == 0 ||
      width > slot.width || height > slot.height)
    return E_INVALIDARG;
  bool available{};
  auto result = IsAvailable(slot, available);
  if (FAILED(result)) return result;
  if (!available) return DXGI_ERROR_WAS_STILL_DRAWING;
  // Arm the retiring fence while the three-slot size-changing pipeline warms.
  // This wait runs on Doroti's raster worker, never the WndProc/platform
  // thread. Once all three initial replacements have retired in order, buffer
  // availability plus the native exact-WM_SIZE handshake keep the steady-state
  // queue bounded without paying a retirement round trip on every frame.
  const auto previous_present_id = acrylic.last_present_id;
  const auto size_changed =
      previous_present_id != 0 &&
      (acrylic.last_present_width != width ||
       acrylic.last_present_height != height);
  bool wait_for_previous =
      size_changed && acrylic.size_change_present_count < 3 &&
      acrylic.retiring_fence &&
      acrylic.retiring_event != nullptr &&
      acrylic.retiring_fence->GetCompletedValue() < previous_present_id;
  if (wait_for_previous) {
    ResetEvent(acrylic.retiring_event);
    result = acrylic.retiring_fence->SetEventOnCompletion(
        previous_present_id, acrylic.retiring_event);
    if (FAILED(result)) return result;
  }
  result = acrylic.surface->SetBuffer(slot.buffer.Get());
  if (FAILED(result)) return result;
  result =
      acrylic.surface->SetColorSpace(DXGI_COLOR_SPACE_RGB_FULL_G22_NONE_P709);
  if (FAILED(result)) return result;
  result = acrylic.surface->SetAlphaMode(DXGI_ALPHA_MODE_PREMULTIPLIED);
  if (FAILED(result)) return result;
  RECT source{0, 0, static_cast<LONG>(width), static_cast<LONG>(height)};
  result = acrylic.surface->SetSourceRect(&source);
  if (FAILED(result)) return result;
  acrylic.surface->SetTag(static_cast<UINT_PTR>(tag));
  const auto id = acrylic.manager->GetNextPresentId();
  result = acrylic.manager->Present();
  if (FAILED(result)) return result;
  acrylic.last_present_id = id;
  acrylic.last_present_width = width;
  acrylic.last_present_height = height;
  if (size_changed) ++acrylic.size_change_present_count;
  if (wait_for_previous) {
    const auto wait = WaitForSingleObject(acrylic.retiring_event, 100);
    if (wait == WAIT_TIMEOUT) return HRESULT_FROM_WIN32(ERROR_TIMEOUT);
    if (wait != WAIT_OBJECT_0) return HRESULT_FROM_WIN32(GetLastError());
  }
  *present_id = id;
  *retiring_fence_value = acrylic.retiring_fence
                                ? acrylic.retiring_fence->GetCompletedValue()
                                : 0;
  return S_OK;
}
