#include "doroti_windows_composition_raster_v1.h"
#include "resize_order_trace.h"
#include <windows.h>
#include <d3d11_4.h>
#include <dxgi1_6.h>
#include <dcomp.h>
#include <wrl/client.h>
#include <memory>
#include <vector>
#include <cmath>
#include <unordered_map>
using Microsoft::WRL::ComPtr;
namespace {
// Retained source descriptions let a later backdrop sample an earlier backdrop
// without reparenting visuals. HWND wrappers keep following native repaints.
struct CompositionSource {
  ComPtr<IUnknown> content;
  std::vector<std::shared_ptr<CompositionSource>> children;
  float x{}, y{}, sigma{};
  D2D_RECT_F clip{};
  bool clipped{};
  uint64_t nativeGeneration{};
};
struct RetainedVisual {
  ComPtr<IDCompositionVisual2> root, group, sample;
  ComPtr<IDCompositionGaussianBlurEffect> blur;
  ComPtr<IUnknown> content;
  std::vector<std::unique_ptr<RetainedVisual>> children;
  float x{}, y{}, sigma{-1};
  D2D_RECT_F clip{};
  bool clipped{};

  HRESULT Update(IDCompositionDesktopDevice* device, const CompositionSource& source) {
    HRESULT hr = S_OK;
    if (!root) {
      hr = device->CreateVisual(&root);
      if (SUCCEEDED(hr)) hr = device->CreateVisual(&group);
      if (SUCCEEDED(hr)) hr = device->CreateVisual(&sample);
      if (SUCCEEDED(hr)) hr = group->AddVisual(sample.Get(), FALSE, nullptr);
      if (SUCCEEDED(hr)) hr = root->AddVisual(group.Get(), FALSE, nullptr);
      if (FAILED(hr)) return hr;
    }
    if (x != source.x) { hr = root->SetOffsetX(source.x); x = source.x; }
    if (SUCCEEDED(hr) && y != source.y) { hr = root->SetOffsetY(source.y); y = source.y; }
    const bool clipChanged = clipped != source.clipped || memcmp(&clip, &source.clip, sizeof(clip)) != 0;
    if (SUCCEEDED(hr) && clipChanged) {
      hr = source.clipped ? root->SetClip(source.clip) : root->SetClip(static_cast<IDCompositionClip*>(nullptr));
      clip = source.clip; clipped = source.clipped;
    }
    if (SUCCEEDED(hr) && (sigma != source.sigma || clipChanged)) {
      if (source.sigma > 0) {
        if (!blur) {
          ComPtr<IDCompositionDevice3> effects;
          hr = device->QueryInterface(IID_PPV_ARGS(&effects));
          if (SUCCEEDED(hr)) hr = effects->CreateGaussianBlurEffect(&blur);
          if (SUCCEEDED(hr)) hr = blur->SetBorderMode(D2D1_BORDER_MODE_HARD);
        }
        if (SUCCEEDED(hr)) hr = blur->SetStandardDeviation(source.sigma);
        if (SUCCEEDED(hr)) hr = group->SetEffect(blur.Get());
        // Bound the INPUT before the blur. Clipping only the output makes DWM
        // filter a window-sized subtree for a small foreground panel.
        const float halo = std::ceil(3 * source.sigma);
        const D2D_RECT_F input{source.clip.left - halo, source.clip.top - halo,
            source.clip.right + halo, source.clip.bottom + halo};
        if (SUCCEEDED(hr)) hr = sample->SetClip(input);
      } else {
        hr = group->SetEffect(nullptr);
        if (SUCCEEDED(hr)) hr = sample->SetClip(static_cast<IDCompositionClip*>(nullptr));
      }
      sigma = source.sigma;
    }
    if (SUCCEEDED(hr) && content != source.content) {
      hr = sample->SetContent(source.content.Get()); content = source.content;
    }
    if (SUCCEEDED(hr) && children.size() != source.children.size()) {
      hr = sample->RemoveAllVisuals(); children.clear();
      for (size_t i = 0; SUCCEEDED(hr) && i < source.children.size(); ++i) {
        auto child = std::make_unique<RetainedVisual>();
        hr = child->Update(device, *source.children[i]);
        if (SUCCEEDED(hr)) hr = sample->AddVisual(child->root.Get(), FALSE, nullptr);
        children.push_back(std::move(child));
      }
    } else {
      for (size_t i = 0; SUCCEEDED(hr) && i < children.size(); ++i)
        hr = children[i]->Update(device, *source.children[i]);
    }
    return hr;
  }
};
struct CompositionRaster {
  ComPtr<ID3D11Device> device;
  ComPtr<ID3D11DeviceContext> immediate;
  ComPtr<IDCompositionDesktopDevice> composition;
  ComPtr<IDCompositionTarget> target;
  ComPtr<IDCompositionVisual> visual;
  ComPtr<IDCompositionSurface> surface;
  std::shared_ptr<CompositionSource> source;
  std::unique_ptr<RetainedVisual> backdrop;
  std::unordered_map<uint64_t, std::shared_ptr<CompositionSource>> nativeSources;
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
  if (SUCCEEDED(hr)) hr = DCompositionCreateDevice3(dxgi.Get(), IID_PPV_ARGS(&value->composition));
  if (SUCCEEDED(hr)) hr = value->composition->CreateTargetForHwnd(reinterpret_cast<HWND>(hwnd), TRUE, &value->target);
  ComPtr<IDCompositionVisual2> visual;
  if (SUCCEEDED(hr)) hr = value->composition->CreateVisual(&visual);
  if (SUCCEEDED(hr)) hr = visual.As(&value->visual);
  if (SUCCEEDED(hr)) hr = value->target->SetRoot(value->visual.Get());
  if (FAILED(hr)) return hr;
  *raster = value.release();
  return S_OK;
}

extern "C" int32_t __cdecl doroti_windows_composition_raster_create_shared_v1(
    void* shared, uint64_t hwnd, void** raster) {
  if (!shared || !raster || !IsWindow(reinterpret_cast<HWND>(hwnd))) return E_INVALIDARG;
  *raster = nullptr;
  const auto& source = *static_cast<CompositionRaster*>(shared);
  auto value = std::make_unique<CompositionRaster>();
  value->device = source.device;
  value->immediate = source.immediate;
  value->composition = source.composition;
  auto hr = value->composition->CreateTargetForHwnd(reinterpret_cast<HWND>(hwnd), TRUE, &value->target);
  ComPtr<IDCompositionVisual2> visual;
  if (SUCCEEDED(hr)) hr = value->composition->CreateVisual(&visual);
  if (SUCCEEDED(hr)) hr = visual.As(&value->visual);
  if (SUCCEEDED(hr)) hr = value->target->SetRoot(value->visual.Get());
  if (FAILED(hr)) return hr;
  *raster = value.release();
  return S_OK;
}

static int32_t UpdateCompositionTree(
    void* raster, const DorotiCompositionSourceV1* sources, uint32_t count,
    float left, float top, float right, float bottom, float sigma) {
  if (!raster || !sources || count > 64 || !std::isfinite(sigma) || sigma < 0 || sigma > 128 ||
      !std::isfinite(left) || !std::isfinite(top) || !std::isfinite(right) ||
      !std::isfinite(bottom) || right < left || bottom < top) return E_INVALIDARG;
  auto& value = *static_cast<CompositionRaster*>(raster);
  auto next = std::make_shared<CompositionSource>();
  next->sigma = sigma;
  next->clipped = true;
  next->clip = {left, top, right, bottom};
  std::unordered_map<uint64_t, std::shared_ptr<CompositionSource>> nativeSources;
  for (uint32_t i = 0; i < count; ++i) {
    const auto& input = sources[i];
    if (input.raster) {
      const auto& preceding = *static_cast<CompositionRaster*>(input.raster);
      if (input.raster == raster || preceding.composition != value.composition || !preceding.source)
        return E_INVALIDARG;
      next->children.push_back(preceding.source);
    } else {
      auto found = value.nativeSources.find(input.hwnd);
      auto child = found != value.nativeSources.end() && found->second->nativeGeneration == input.generation
          ? found->second : std::make_shared<CompositionSource>();
      if (!child->content) {
        auto hr = value.composition->CreateSurfaceFromHwnd(reinterpret_cast<HWND>(input.hwnd), &child->content);
        if (FAILED(hr)) return hr;
      }
      child->x = input.x; child->y = input.y;
      child->nativeGeneration = input.generation;
      child->clipped = true;
      child->clip = {input.left, input.top, input.right, input.bottom};
      nativeSources.emplace(input.hwnd, child);
      next->children.push_back(std::move(child));
    }
  }
  const bool created = !value.backdrop;
  if (created) value.backdrop = std::make_unique<RetainedVisual>();
  auto hr = value.backdrop->Update(value.composition.Get(), *next);
  if (SUCCEEDED(hr) && created) hr = value.target->SetRoot(value.backdrop->root.Get());
  if (SUCCEEDED(hr)) {
    value.source = std::move(next); value.visual = value.backdrop->root;
    value.nativeSources = std::move(nativeSources);
  }
  // The owner publishes all sibling updates with one device Commit per frame.
  return hr;
}

extern "C" int32_t __cdecl doroti_windows_composition_scene_update_v1(
    void* raster, const DorotiCompositionSourceV1* sources, uint32_t count, float width, float height) {
  return UpdateCompositionTree(raster, sources, count, 0, 0, width, height, 0);
}

extern "C" int32_t __cdecl doroti_windows_composition_backdrop_update_v1(
    void* raster, const DorotiCompositionSourceV1* sources, uint32_t count,
    float left, float top, float right, float bottom, float sigma) {
  if (sigma <= 0) return E_INVALIDARG;
  return UpdateCompositionTree(raster, sources, count, left, top, right, bottom, sigma);
}

extern "C" void __cdecl doroti_windows_d3d12_output_trace_prepared_v1(void) {
  doroti::resize_trace::Record("prepare-vulkan-copy-complete", doroti::resize_trace::render_key);
}

static int32_t UpdateRaster(
    void* raster, const void* pixels, uint32_t width, uint32_t height, uint32_t row_bytes, int32_t x, int32_t y) {
  if (raster == nullptr || pixels == nullptr || width == 0 || height == 0 ||
      width > 16384 || height > 16384 || row_bytes < width * 4u)
    return Result(E_INVALIDARG);
  auto& value = *static_cast<CompositionRaster*>(raster);
  if (value.source && value.source->sigma > 0) {
    ComPtr<IDCompositionVisual2> visual;
    auto hr = value.composition->CreateVisual(&visual);
    if (SUCCEEDED(hr)) hr = value.target->SetRoot(visual.Get());
    if (FAILED(hr)) return hr;
    value.visual = visual;
    value.backdrop.reset(); value.nativeSources.clear();
    value.surface.Reset();
    value.width = value.height = 0;
  }
  if (value.width != width || value.height != height) {
    ComPtr<IDCompositionSurface> next;
    auto hr = value.composition->CreateSurface(width, height, DXGI_FORMAT_B8G8R8A8_UNORM,
        DXGI_ALPHA_MODE_PREMULTIPLIED, &next);
    if (FAILED(hr)) return Result(hr);
    hr = value.visual->SetContent(next.Get());
    if (SUCCEEDED(hr)) hr = value.visual->SetOffsetX(0.0f);
    if (SUCCEEDED(hr)) hr = value.visual->SetOffsetY(0.0f);
    if (FAILED(hr)) return Result(hr);
    value.surface = next;
    value.source = std::make_shared<CompositionSource>();
    value.source->content = next;
    value.width = width;
    value.height = height;
  }
  if (value.source->x != x) {
    const auto hr = value.visual->SetOffsetX(static_cast<float>(x));
    if (FAILED(hr)) return hr;
    value.source->x = static_cast<float>(x);
  }
  if (value.source->y != y) {
    const auto hr = value.visual->SetOffsetY(static_cast<float>(y));
    if (FAILED(hr)) return hr;
    value.source->y = static_cast<float>(y);
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
    hr = value.device->GetDeviceRemovedReason();
  }
  texture.Reset();
  drawing2.Reset();
  drawing.Reset();
  const auto end = value.surface->EndDraw();
  if (FAILED(hr)) return Result(hr);
  if (FAILED(end)) return Result(end);
  return S_OK;
}

extern "C" int32_t __cdecl doroti_windows_composition_commit_v1(void* raster) {
  if (!raster) return E_INVALIDARG;
  auto& value = *static_cast<CompositionRaster*>(raster);
  value.immediate->Flush();
  return value.composition->Commit();
}

extern "C" int32_t __cdecl doroti_windows_composition_wait_commit_v1(void* raster) {
  if (!raster) return E_INVALIDARG;
  return static_cast<CompositionRaster*>(raster)->composition->WaitForCommitCompletion();
}

extern "C" int32_t __cdecl doroti_windows_composition_raster_update_region_v1(
    void* raster, const void* pixels, uint32_t width, uint32_t height, uint32_t row_bytes, int32_t x, int32_t y) {
  return UpdateRaster(raster, pixels, width, height, row_bytes, x, y);
}

extern "C" int32_t __cdecl doroti_windows_composition_raster_update_v1(
    void* raster, const void* pixels, uint32_t width, uint32_t height, uint32_t row_bytes) {
  const auto hr = UpdateRaster(raster, pixels, width, height, row_bytes, 0, 0);
  return FAILED(hr) ? hr : doroti_windows_composition_commit_v1(raster);
}

extern "C" void __cdecl
doroti_windows_composition_raster_destroy_v1(void* raster) {
  delete static_cast<CompositionRaster*>(raster);
}

