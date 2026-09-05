#include "doroti_windows_gpu_selection_v1.h"

#include <windows.h>
#include <d3d11.h>
#include <dxgi1_6.h>
#include <wrl/client.h>

using Microsoft::WRL::ComPtr;

extern "C" int32_t DOROTI_WINDOWS_CALL
doroti_windows_gpu_select_adapter_v1(
    uint32_t preference, const uint64_t* eligible_luids,
    uint32_t eligible_count, uint64_t* selected_luid) {
  if (preference > 2 || selected_luid == nullptr ||
      (eligible_count != 0 && eligible_luids == nullptr))
    return E_INVALIDARG;
  *selected_luid = 0;

  const auto try_adapter = [&](IDXGIAdapter1* adapter) -> HRESULT {
    DXGI_ADAPTER_DESC1 description{};
    const auto result = adapter->GetDesc1(&description);
    if (FAILED(result)) return result;
    if ((description.Flags & DXGI_ADAPTER_FLAG_SOFTWARE) != 0) return S_FALSE;
    const auto luid = static_cast<uint64_t>(description.AdapterLuid.LowPart) |
        (static_cast<uint64_t>(static_cast<uint32_t>(description.AdapterLuid.HighPart)) << 32);
    bool eligible = eligible_count == 0;
    for (uint32_t index = 0; index < eligible_count; ++index)
      eligible = eligible || eligible_luids[index] == luid;
    if (!eligible) return S_FALSE;
    *selected_luid = luid;
    return S_OK;
  };

  if (preference == 0) {
    // Match the default hardware D3D11 selection used by ANGLE, including
    // system/driver policy, rather than ranking discrete vs integrated GPUs.
    ComPtr<ID3D11Device> device;
    auto result = D3D11CreateDevice(
        nullptr, D3D_DRIVER_TYPE_HARDWARE, nullptr, 0, nullptr, 0,
        D3D11_SDK_VERSION, &device, nullptr, nullptr);
    if (SUCCEEDED(result)) {
      ComPtr<IDXGIDevice> dxgi_device;
      ComPtr<IDXGIAdapter> adapter;
      ComPtr<IDXGIAdapter1> adapter1;
      result = device.As(&dxgi_device);
      if (SUCCEEDED(result)) result = dxgi_device->GetAdapter(&adapter);
      if (SUCCEEDED(result)) result = adapter.As(&adapter1);
      if (FAILED(result)) return result;
      result = try_adapter(adapter1.Get());
      if (result != S_FALSE) return result;
    }
  }

  ComPtr<IDXGIFactory1> factory;
  auto result = CreateDXGIFactory1(IID_PPV_ARGS(&factory));
  if (FAILED(result)) return result;
  ComPtr<IDXGIFactory6> factory6;
  if (preference != 0) {
    result = factory.As(&factory6);
    // Like Flutter, use the default enumeration when GPU preference is unavailable.
    if (FAILED(result) && result != E_NOINTERFACE) return result;
  }
  for (UINT index = 0;; ++index) {
    ComPtr<IDXGIAdapter1> adapter;
    result = factory6
        ? factory6->EnumAdapterByGpuPreference(
              index, static_cast<DXGI_GPU_PREFERENCE>(preference), IID_PPV_ARGS(&adapter))
        : factory->EnumAdapters1(index, &adapter);
    if (FAILED(result)) return result;
    result = try_adapter(adapter.Get());
    if (result != S_FALSE) return result;
  }
}
