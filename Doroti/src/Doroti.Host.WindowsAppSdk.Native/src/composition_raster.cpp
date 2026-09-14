#include "doroti_windows_composition_raster_v1.h"
#include "resize_order_trace.h"
#include <windows.h>
#include <d3d11_4.h>
#include <dxgi1_6.h>
#include <dcomp.h>
#include <wrl/client.h>
#include <memory>
using Microsoft::WRL::ComPtr;
namespace {
struct CompositionRaster {
  ComPtr<ID3D11Device> device;
  ComPtr<ID3D11DeviceContext> immediate;
  ComPtr<IDCompositionDevice> composition;
  ComPtr<IDCompositionTarget> target;
  ComPtr<IDCompositionVisual> visual;
  ComPtr<IDCompositionSurface> surface;
  uint32_t width{}, height{};
  ~CompositionRaster() {
    if (target) target->SetRoot(nullptr);
    if (composition) composition->Commit();
  }
};

int32_t Result(HRESULT hr) noexcept { return static_cast<int32_t>(hr); }
}

// Platform raster slices use D3D11 BeginDraw/UpdateSubresource independently
// of the main Vulkan/D3D12 output. Allocate this device only for native views.
extern "C" int32_t __cdecl doroti_windows_composition_raster_create_on_adapter_v1(
    uint32_t low, int32_t high, IUnknown** shared_device, uint64_t hwnd, void** raster) {
  if (!raster || !shared_device || !IsWindow(reinterpret_cast<HWND>(hwnd))) return E_INVALIDARG;
  *raster = nullptr;
  auto value = std::make_unique<CompositionRaster>();
  auto hr = S_OK;
  if (*shared_device) {
    hr = (*shared_device)->QueryInterface(IID_PPV_ARGS(&value->device));
    if (SUCCEEDED(hr)) value->device->GetImmediateContext(&value->immediate);
  } else {
    ComPtr<IDXGIFactory4> factory;
    ComPtr<IDXGIAdapter1> adapter;
    hr = CreateDXGIFactory1(IID_PPV_ARGS(&factory));
    const LUID luid{low, high};
    if (SUCCEEDED(hr)) hr = factory->EnumAdapterByLuid(luid, IID_PPV_ARGS(&adapter));
    if (SUCCEEDED(hr)) hr = D3D11CreateDevice(adapter.Get(), D3D_DRIVER_TYPE_UNKNOWN, nullptr,
        D3D11_CREATE_DEVICE_BGRA_SUPPORT, nullptr, 0, D3D11_SDK_VERSION,
        &value->device, nullptr, &value->immediate);
    if (SUCCEEDED(hr)) hr = value->device->QueryInterface(IID_PPV_ARGS(shared_device));
  }
  if (FAILED(hr)) return hr;
  ComPtr<ID3D11Multithread> multithread;
  hr = value->immediate.As(&multithread);
  if (FAILED(hr)) return hr;
  multithread->SetMultithreadProtected(TRUE);
  ComPtr<IDXGIDevice> dxgi;
  hr = value->device.As(&dxgi);
  if (SUCCEEDED(hr)) hr = DCompositionCreateDevice(dxgi.Get(), IID_PPV_ARGS(&value->composition));
  if (SUCCEEDED(hr)) hr = value->composition->CreateTargetForHwnd(reinterpret_cast<HWND>(hwnd), TRUE, &value->target);
  if (SUCCEEDED(hr)) hr = value->composition->CreateVisual(&value->visual);
  if (SUCCEEDED(hr)) hr = value->target->SetRoot(value->visual.Get());
  if (FAILED(hr)) return hr;
  *raster = value.release();
  return S_OK;
}

extern "C" void __cdecl doroti_windows_d3d12_output_trace_prepared_v1(void) {
  doroti::resize_trace::Record("prepare-vulkan-copy-complete", doroti::resize_trace::render_key);
}

extern "C" int32_t __cdecl
doroti_windows_composition_raster_update_v1(
    void* raster, const void* pixels, uint32_t width, uint32_t height, uint32_t row_bytes) {
  if (raster == nullptr || pixels == nullptr || width == 0 || height == 0 ||
      width > 16384 || height > 16384 || row_bytes < width * 4u)
    return Result(E_INVALIDARG);
  auto& value = *static_cast<CompositionRaster*>(raster);
  if (value.width != width || value.height != height) {
    ComPtr<IDCompositionSurface> next;
    auto hr = value.composition->CreateSurface(width, height, DXGI_FORMAT_B8G8R8A8_UNORM,
        DXGI_ALPHA_MODE_PREMULTIPLIED, &next);
    if (FAILED(hr)) return Result(hr);
    hr = value.visual->SetContent(next.Get());
    if (FAILED(hr)) return Result(hr);
    value.surface = next;
    value.width = width;
    value.height = height;
  }
  ComPtr<IDXGISurface> drawing;
  POINT offset{};
  auto hr = value.surface->BeginDraw(nullptr, IID_PPV_ARGS(&drawing), &offset);
  if (FAILED(hr)) return Result(hr);
  ComPtr<IDXGISurface2> drawing2;
  ComPtr<ID3D11Texture2D> texture;
  UINT subresource{};
  hr = drawing.As(&drawing2);
  if (SUCCEEDED(hr)) hr = drawing2->GetResource(IID_PPV_ARGS(&texture), &subresource);
  if (SUCCEEDED(hr)) {
    // BeginDraw may return an atlas offset, even for a full-surface update.
    const D3D11_BOX box{static_cast<UINT>(offset.x), static_cast<UINT>(offset.y), 0,
        static_cast<UINT>(offset.x) + width, static_cast<UINT>(offset.y) + height, 1};
    value.immediate->UpdateSubresource(texture.Get(), subresource, &box, pixels, row_bytes, 0);
    value.immediate->Flush();
    hr = value.device->GetDeviceRemovedReason();
  }
  texture.Reset();
  drawing2.Reset();
  drawing.Reset();
  const auto end = value.surface->EndDraw();
  if (FAILED(hr)) return Result(hr);
  if (FAILED(end)) return Result(end);
  // EndDraw queues the upload and Commit publishes it in order. Waiting for
  // the compositor here blocks the HWND input thread once per raster slice
  // on every animation frame. It is not a physical presentation receipt.
  return Result(value.composition->Commit());
}

extern "C" void __cdecl
doroti_windows_composition_raster_destroy_v1(void* raster) {
  delete static_cast<CompositionRaster*>(raster);
}

