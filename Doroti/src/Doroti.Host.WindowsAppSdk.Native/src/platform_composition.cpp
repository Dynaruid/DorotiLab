// Windows.UI.Composition only: compatible with WebView2 RootVisualTarget.
#include <windows.h>
#include <d3d11.h>
#include <d2d1_1.h>
#include <d2d1effects.h>
#include <windows.ui.composition.interop.h>
#include <windows.graphics.effects.interop.h>
#include <winrt/Windows.Foundation.h>
#include <winrt/Windows.Graphics.Effects.h>
#include <winrt/Windows.Graphics.DirectX.h>
#include <winrt/Windows.UI.Composition.h>
#include <memory>
#include <cmath>

namespace {
using namespace winrt;
using namespace Windows::UI::Composition;
using namespace Windows::Graphics::Effects;
using ABI::Windows::Graphics::Effects::IGraphicsEffectD2D1Interop;
using ABI::Windows::Graphics::Effects::GRAPHICS_EFFECT_PROPERTY_MAPPING;

struct Gaussian : implements<Gaussian, IGraphicsEffect, IGraphicsEffectSource, IGraphicsEffectD2D1Interop> {
  float sigma;
  IGraphicsEffectSource source;
  hstring name{L"DorotiBlur"};
  Gaussian(float value) : sigma(value), source(CompositionEffectSourceParameter(L"backdrop")) {}
  hstring Name() const { return name; }
  void Name(hstring const& value) { name = value; }
  HRESULT __stdcall GetEffectId(GUID* id) noexcept override {
    if (!id) return E_POINTER; *id = CLSID_D2D1GaussianBlur; return S_OK;
  }
  HRESULT __stdcall GetNamedPropertyMapping(LPCWSTR, UINT*, GRAPHICS_EFFECT_PROPERTY_MAPPING*) noexcept override { return E_NOTIMPL; }
  HRESULT __stdcall GetPropertyCount(UINT* count) noexcept override {
    if (!count) return E_POINTER; *count = 3; return S_OK;
  }
  HRESULT __stdcall GetProperty(UINT index, ABI::Windows::Foundation::IPropertyValue** value) noexcept override {
    if (!value) return E_POINTER; *value = nullptr;
    try {
      Windows::Foundation::IInspectable property{nullptr};
      if (index == 0) property = Windows::Foundation::PropertyValue::CreateSingle(sigma);
      else if (index == 1) property = Windows::Foundation::PropertyValue::CreateUInt32(D2D1_GAUSSIANBLUR_OPTIMIZATION_BALANCED);
      else if (index == 2) property = Windows::Foundation::PropertyValue::CreateUInt32(D2D1_BORDER_MODE_HARD);
      else return E_INVALIDARG;
      return winrt::get_unknown(property)->QueryInterface(__uuidof(**value), reinterpret_cast<void**>(value));
    } catch (...) { return to_hresult(); }
  }
  HRESULT __stdcall GetSourceCount(UINT* count) noexcept override {
    if (!count) return E_POINTER; *count = 1; return S_OK;
  }
  HRESULT __stdcall GetSource(UINT index, ABI::Windows::Graphics::Effects::IGraphicsEffectSource** value) noexcept override {
    if (!value) return E_POINTER; *value = nullptr;
    if (index != 0) return E_INVALIDARG;
    return winrt::get_unknown(source)->QueryInterface(__uuidof(**value), reinterpret_cast<void**>(value));
  }
};

struct RasterContext {
  com_ptr<ID3D11Device> device;
  com_ptr<ID2D1Device> drawing;
  CompositionGraphicsDevice graphics{nullptr};
};

}

extern "C" __declspec(dllexport) int32_t __cdecl doroti_windows_platform_effect_v1(
    IUnknown* compositor, float sigma, IUnknown** brush) noexcept {
  if (!compositor || !brush || !std::isfinite(sigma) || sigma <= 0 || sigma > 128) return E_INVALIDARG;
  *brush = nullptr;
  try {
    Compositor owner{nullptr}; copy_from_abi(owner, compositor);
    auto effect = make<Gaussian>(sigma);
    auto factory = owner.CreateEffectFactory(effect);
    auto result = factory.CreateBrush();
    result.SetSourceParameter(L"backdrop", owner.CreateBackdropBrush());
    *brush = reinterpret_cast<IUnknown*>(detach_abi(result));
    return S_OK;
  } catch (...) { return to_hresult(); }
}

extern "C" __declspec(dllexport) int32_t __cdecl doroti_windows_platform_graphics_create_v1(
    IUnknown* compositor, void** context) noexcept {
  if (!compositor || !context) return E_INVALIDARG;
  *context = nullptr;
  try {
    auto value = std::make_unique<RasterContext>();
    check_hresult(D3D11CreateDevice(nullptr, D3D_DRIVER_TYPE_HARDWARE, nullptr,
        D3D11_CREATE_DEVICE_BGRA_SUPPORT, nullptr, 0, D3D11_SDK_VERSION, value->device.put(), nullptr, nullptr));
    com_ptr<ID2D1Factory1> factory;
    const D2D1_FACTORY_OPTIONS options{};
    check_hresult(D2D1CreateFactory(D2D1_FACTORY_TYPE_SINGLE_THREADED, __uuidof(ID2D1Factory1), &options, factory.put_void()));
    check_hresult(factory->CreateDevice(value->device.as<IDXGIDevice>().get(), value->drawing.put()));
    Compositor owner{nullptr}; copy_from_abi(owner, compositor);
    check_hresult(owner.as<ABI::Windows::UI::Composition::ICompositorInterop>()->CreateGraphicsDevice(
        value->drawing.get(), reinterpret_cast<ABI::Windows::UI::Composition::ICompositionGraphicsDevice**>(put_abi(value->graphics))));
    *context = value.release(); return S_OK;
  } catch (...) { return to_hresult(); }
}

extern "C" __declspec(dllexport) int32_t __cdecl doroti_windows_platform_surface_v1(
    void* context, const void* pixels, uint32_t width, uint32_t height, uint32_t stride, IUnknown** surface) noexcept {
  if (!context || !pixels || !surface || width == 0 || height == 0 || width > 16384 || height > 16384 || stride < width * 4u)
    return E_INVALIDARG;
  *surface = nullptr;
  try {
    auto& value = *static_cast<RasterContext*>(context);
    auto result = value.graphics.CreateDrawingSurface({static_cast<float>(width), static_cast<float>(height)},
        Windows::Graphics::DirectX::DirectXPixelFormat::B8G8R8A8UIntNormalized,
        Windows::Graphics::DirectX::DirectXAlphaMode::Premultiplied);
    auto interop = result.as<ABI::Windows::UI::Composition::ICompositionDrawingSurfaceInterop>();
    com_ptr<ID2D1DeviceContext> drawing;
    POINT offset{};
    check_hresult(interop->BeginDraw(nullptr, __uuidof(ID2D1DeviceContext), drawing.put_void(), &offset));
    HRESULT status = S_OK;
    try {
      com_ptr<ID2D1Bitmap1> bitmap;
      const auto props = D2D1::BitmapProperties1(D2D1_BITMAP_OPTIONS_NONE,
          D2D1::PixelFormat(DXGI_FORMAT_B8G8R8A8_UNORM, D2D1_ALPHA_MODE_PREMULTIPLIED));
      check_hresult(drawing->CreateBitmap(D2D1::SizeU(width, height), pixels, stride, props, bitmap.put()));
      drawing->SetDpi(96, 96);
      drawing->SetTransform(D2D1::Matrix3x2F::Translation(static_cast<float>(offset.x), static_cast<float>(offset.y)));
      drawing->Clear(D2D1::ColorF(0, 0));
      drawing->DrawBitmap(bitmap.get(), D2D1::RectF(0, 0, static_cast<float>(width), static_cast<float>(height)),
          1, D2D1_INTERPOLATION_MODE_NEAREST_NEIGHBOR);
    } catch (...) { status = to_hresult(); }
    const auto end = interop->EndDraw();
    check_hresult(status); check_hresult(end);
    *surface = reinterpret_cast<IUnknown*>(detach_abi(result)); return S_OK;
  } catch (...) { return to_hresult(); }
}

extern "C" __declspec(dllexport) void __cdecl doroti_windows_platform_graphics_destroy_v1(void* context) noexcept {
  delete static_cast<RasterContext*>(context);
}
