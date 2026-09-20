#pragma once
#include <d3d11_1.h>
#include <dxgi1_6.h>
#include <d2d1_1.h>
#include <winrt/base.h>

// All producer/consumer devices select the Vulkan owner's adapter. Each source
// is immutable and admitted only after its Vulkan fence. No keyed-mutex reuse.
inline winrt::com_ptr<ID3D11Device> DorotiRasterDevice(uint32_t low, int32_t high) {
  winrt::com_ptr<IDXGIFactory4> factory;
  winrt::com_ptr<IDXGIAdapter1> adapter;
  winrt::check_hresult(CreateDXGIFactory1(__uuidof(IDXGIFactory4), factory.put_void()));
  winrt::check_hresult(factory->EnumAdapterByLuid({low, high}, __uuidof(IDXGIAdapter1), adapter.put_void()));
  winrt::com_ptr<ID3D11Device> device;
  winrt::check_hresult(D3D11CreateDevice(adapter.get(), D3D_DRIVER_TYPE_UNKNOWN, nullptr,
      D3D11_CREATE_DEVICE_BGRA_SUPPORT, nullptr, 0, D3D11_SDK_VERSION, device.put(), nullptr, nullptr));
  return device;
}
inline winrt::com_ptr<ID3D11Texture2D> DorotiOpenRaster(ID3D11Device* device, HANDLE handle) {
  winrt::com_ptr<ID3D11Texture2D> texture;
  winrt::check_hresult(device->OpenSharedResource(handle, __uuidof(ID3D11Texture2D), texture.put_void()));
  return texture;
}
inline void DorotiDrawSharedRaster(ID3D11Device* device, ID2D1DeviceContext* drawing,
    HANDLE handle, uint32_t source_y, uint32_t width, uint32_t height, POINT offset) {
  auto texture = DorotiOpenRaster(device, handle);
  D3D11_TEXTURE2D_DESC desc{}; texture->GetDesc(&desc);
  if (width > desc.Width || uint64_t(source_y) + height > desc.Height) winrt::throw_hresult(E_INVALIDARG);
  winrt::com_ptr<ID2D1Bitmap1> bitmap;
  const auto props = D2D1::BitmapProperties1(D2D1_BITMAP_OPTIONS_NONE,
      D2D1::PixelFormat(DXGI_FORMAT_B8G8R8A8_UNORM, D2D1_ALPHA_MODE_PREMULTIPLIED));
  winrt::check_hresult(drawing->CreateBitmapFromDxgiSurface(texture.as<IDXGISurface>().get(), &props, bitmap.put()));
  drawing->SetDpi(96, 96);
  drawing->SetTransform(D2D1::Matrix3x2F::Translation(float(offset.x), float(offset.y)));
  drawing->Clear(D2D1::ColorF(0, 0));
  drawing->DrawBitmap(bitmap.get(), D2D1::RectF(0, 0, float(width), float(height)), 1,
      D2D1_INTERPOLATION_MODE_NEAREST_NEIGHBOR,
      D2D1::RectF(0, float(source_y), float(width), float(source_y + height)));
}
