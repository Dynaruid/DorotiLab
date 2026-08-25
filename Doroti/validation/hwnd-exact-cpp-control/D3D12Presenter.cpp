#include "D3D12Presenter.h"

#include <algorithm>
#include <stdexcept>
#include <vector>

#include <dwmapi.h>
#include <dxgidebug.h>

using Microsoft::WRL::ComPtr;

namespace doroti::validation {
namespace {

void Check(HRESULT result, const char* operation) {
  if (FAILED(result)) {
    throw std::runtime_error(operation);
  }
}

D3D12_HEAP_PROPERTIES HeapProperties(D3D12_HEAP_TYPE type) {
  D3D12_HEAP_PROPERTIES properties{};
  properties.Type = type;
  properties.CPUPageProperty = D3D12_CPU_PAGE_PROPERTY_UNKNOWN;
  properties.MemoryPoolPreference = D3D12_MEMORY_POOL_UNKNOWN;
  properties.CreationNodeMask = 1;
  properties.VisibleNodeMask = 1;
  return properties;
}

D3D12_RESOURCE_DESC TextureDescription(uint32_t width, uint32_t height) {
  D3D12_RESOURCE_DESC description{};
  description.Dimension = D3D12_RESOURCE_DIMENSION_TEXTURE2D;
  description.Width = width;
  description.Height = height;
  description.DepthOrArraySize = 1;
  description.MipLevels = 1;
  description.Format = DXGI_FORMAT_B8G8R8A8_UNORM;
  description.SampleDesc = {1, 0};
  description.Layout = D3D12_TEXTURE_LAYOUT_UNKNOWN;
  return description;
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

}  // namespace

D3D12Presenter::D3D12Presenter(HWND child,
                               PresenterDiagnostics& diagnostics)
    : child_(child), diagnostics_(diagnostics) {
  if (!IsWindow(child_)) {
    throw std::invalid_argument("D3D12 presenter child HWND is invalid");
  }
  CreateDevice();
}

D3D12Presenter::~D3D12Presenter() {
  try {
    Shutdown();
  } catch (...) {
  }
}

void D3D12Presenter::CreateDevice() {
  ComPtr<ID3D12Debug> debug;
  Check(D3D12GetDebugInterface(IID_PPV_ARGS(&debug)),
        "D3D12 debug layer is unavailable");
  debug->EnableDebugLayer();

  Check(CreateDXGIFactory2(DXGI_CREATE_FACTORY_DEBUG,
                           IID_PPV_ARGS(&factory_)),
        "CreateDXGIFactory2 failed");

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
      break;
    }
  }
  if (!device_) {
    throw std::runtime_error("No hardware D3D12 adapter satisfies feature level 11_0");
  }
  Check(device_.As(&info_queue_), "ID3D12InfoQueue is unavailable");

  D3D12_COMMAND_QUEUE_DESC queue_description{};
  queue_description.Type = D3D12_COMMAND_LIST_TYPE_DIRECT;
  Check(device_->CreateCommandQueue(&queue_description,
                                     IID_PPV_ARGS(&queue_)),
        "CreateCommandQueue failed");
  Check(device_->CreateCommandAllocator(D3D12_COMMAND_LIST_TYPE_DIRECT,
                                         IID_PPV_ARGS(&allocator_)),
        "CreateCommandAllocator failed");
  Check(device_->CreateCommandList(0, D3D12_COMMAND_LIST_TYPE_DIRECT,
                                    allocator_.Get(), nullptr,
                                    IID_PPV_ARGS(&command_list_)),
        "CreateCommandList failed");
  Check(command_list_->Close(), "Initial command-list close failed");
  Check(device_->CreateFence(0, D3D12_FENCE_FLAG_NONE,
                              IID_PPV_ARGS(&fence_)),
        "CreateFence failed");
  fence_event_ = CreateEventW(nullptr, FALSE, FALSE, nullptr);
  if (!fence_event_) {
    throw std::runtime_error("CreateEventW failed");
  }
}

void D3D12Presenter::CreateOrResizeSwapChain(uint32_t width,
                                             uint32_t height) {
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
    ComPtr<IDXGISwapChain1> swap_chain;
    Check(factory_->CreateSwapChainForHwnd(queue_.Get(), child_, &description,
                                            nullptr, nullptr, &swap_chain),
          "CreateSwapChainForHwnd failed");
    Check(swap_chain.As(&swap_chain_), "IDXGISwapChain3 is unavailable");
    Check(factory_->MakeWindowAssociation(child_, DXGI_MWA_NO_ALT_ENTER),
          "MakeWindowAssociation failed");
  } else {
    backing_.Reset();
    Check(swap_chain_->ResizeBuffers(2, width, height,
                                     DXGI_FORMAT_B8G8R8A8_UNORM, 0),
          "ResizeBuffers failed");
    ++diagnostics_.resize_count;
  }
}

void D3D12Presenter::CreateCheckerBacking(uint32_t width, uint32_t height,
                                          uint64_t generation) {
  const auto texture_description = TextureDescription(width, height);
  const auto default_heap = HeapProperties(D3D12_HEAP_TYPE_DEFAULT);
  Check(device_->CreateCommittedResource(
            &default_heap, D3D12_HEAP_FLAG_NONE, &texture_description,
            D3D12_RESOURCE_STATE_COPY_DEST, nullptr, IID_PPV_ARGS(&backing_)),
        "Create exact backing resource failed");

  D3D12_PLACED_SUBRESOURCE_FOOTPRINT footprint{};
  UINT rows{};
  UINT64 row_size{};
  UINT64 upload_size{};
  device_->GetCopyableFootprints(&texture_description, 0, 1, 0, &footprint,
                                 &rows, &row_size, &upload_size);
  D3D12_RESOURCE_DESC upload_description{};
  upload_description.Dimension = D3D12_RESOURCE_DIMENSION_BUFFER;
  upload_description.Width = upload_size;
  upload_description.Height = 1;
  upload_description.DepthOrArraySize = 1;
  upload_description.MipLevels = 1;
  upload_description.SampleDesc = {1, 0};
  upload_description.Layout = D3D12_TEXTURE_LAYOUT_ROW_MAJOR;
  const auto upload_heap = HeapProperties(D3D12_HEAP_TYPE_UPLOAD);
  ComPtr<ID3D12Resource> upload;
  Check(device_->CreateCommittedResource(
            &upload_heap, D3D12_HEAP_FLAG_NONE, &upload_description,
            D3D12_RESOURCE_STATE_GENERIC_READ, nullptr,
            IID_PPV_ARGS(&upload)),
        "Create checker upload resource failed");

  uint8_t* mapped{};
  const D3D12_RANGE read_range{0, 0};
  Check(upload->Map(0, &read_range, reinterpret_cast<void**>(&mapped)),
        "Map checker upload failed");
  for (uint32_t y = 0; y < height; ++y) {
    auto* row = reinterpret_cast<uint32_t*>(
        mapped + footprint.Offset + static_cast<size_t>(y) * footprint.Footprint.RowPitch);
    for (uint32_t x = 0; x < width; ++x) {
      const bool checker = ((x / 24) + (y / 24)) % 2 == 0;
      const uint8_t generation_color = static_cast<uint8_t>((generation * 37) & 0xff);
      const uint8_t red = checker ? 0x24 : generation_color;
      const uint8_t green = checker ? generation_color : 0x45;
      const uint8_t blue = checker ? 0xd8 : 0x18;
      row[x] = 0xff000000u | (static_cast<uint32_t>(red) << 16) |
               (static_cast<uint32_t>(green) << 8) | blue;
    }
  }
  upload->Unmap(0, nullptr);

  Check(allocator_->Reset(), "Command allocator reset failed");
  Check(command_list_->Reset(allocator_.Get(), nullptr),
        "Command list reset failed");
  D3D12_TEXTURE_COPY_LOCATION destination{};
  destination.pResource = backing_.Get();
  destination.Type = D3D12_TEXTURE_COPY_TYPE_SUBRESOURCE_INDEX;
  D3D12_TEXTURE_COPY_LOCATION source{};
  source.pResource = upload.Get();
  source.Type = D3D12_TEXTURE_COPY_TYPE_PLACED_FOOTPRINT;
  source.PlacedFootprint = footprint;
  command_list_->CopyTextureRegion(&destination, 0, 0, 0, &source, nullptr);
  const auto barrier = Transition(backing_.Get(), D3D12_RESOURCE_STATE_COPY_DEST,
                                  D3D12_RESOURCE_STATE_COPY_SOURCE);
  command_list_->ResourceBarrier(1, &barrier);
  ExecuteAndWait();
}

void D3D12Presenter::PresentExact(uint32_t width, uint32_t height,
                                  uint64_t generation) {
  RECT client{};
  if (!GetClientRect(child_, &client)) {
    throw std::runtime_error("GetClientRect failed before Present");
  }
  const auto client_width = static_cast<uint32_t>(client.right - client.left);
  const auto client_height = static_cast<uint32_t>(client.bottom - client.top);
  DXGI_SWAP_CHAIN_DESC1 description{};
  Check(swap_chain_->GetDesc1(&description), "GetDesc1 failed");
  if (client_width != width || client_height != height ||
      description.Width != width || description.Height != height) {
    ++diagnostics_.wrong_size_present_count;
    throw std::runtime_error("Wrong-size present was rejected");
  }

  ComPtr<ID3D12Resource> back_buffer;
  Check(swap_chain_->GetBuffer(swap_chain_->GetCurrentBackBufferIndex(),
                               IID_PPV_ARGS(&back_buffer)),
        "GetBuffer failed");
  Check(allocator_->Reset(), "Command allocator reset failed");
  Check(command_list_->Reset(allocator_.Get(), nullptr),
        "Command list reset failed");
  auto to_copy = Transition(back_buffer.Get(), D3D12_RESOURCE_STATE_PRESENT,
                            D3D12_RESOURCE_STATE_COPY_DEST);
  command_list_->ResourceBarrier(1, &to_copy);
  command_list_->CopyResource(back_buffer.Get(), backing_.Get());
  auto to_present = Transition(back_buffer.Get(), D3D12_RESOURCE_STATE_COPY_DEST,
                               D3D12_RESOURCE_STATE_PRESENT);
  command_list_->ResourceBarrier(1, &to_present);
  ExecuteAndWait();
  back_buffer.Reset();
  Check(swap_chain_->Present(0, 0), "Present failed");
  Check(DwmFlush(), "DwmFlush failed");
  width_ = width;
  height_ = height;
  generation_ = generation;
  ++diagnostics_.present_count;
}

void D3D12Presenter::ResizeAndPresent(uint32_t width, uint32_t height,
                                      uint64_t generation) {
  if (shutdown_ || removed_) {
    throw std::runtime_error("Presenter is unavailable");
  }
  if (width == 0 || height == 0) {
    return;
  }
  CreateOrResizeSwapChain(width, height);
  CreateCheckerBacking(width, height, generation);
  PresentExact(width, height, generation);
}

void D3D12Presenter::ExecuteAndWait() {
  Check(command_list_->Close(), "Command list close failed");
  ID3D12CommandList* lists[]{command_list_.Get()};
  queue_->ExecuteCommandLists(1, lists);
  WaitIdle();
}

void D3D12Presenter::WaitIdle() {
  if (!queue_ || removed_) {
    return;
  }
  const auto value = ++fence_value_;
  Check(queue_->Signal(fence_.Get(), value), "Queue fence signal failed");
  if (fence_->GetCompletedValue() < value) {
    Check(fence_->SetEventOnCompletion(value, fence_event_),
          "Fence event registration failed");
    if (WaitForSingleObject(fence_event_, 5000) != WAIT_OBJECT_0) {
      throw std::runtime_error("D3D12 fence wait exceeded five seconds");
    }
  }
}

void D3D12Presenter::InjectDeviceLoss() {
  if (removed_) {
    return;
  }
  WaitIdle();
  ++diagnostics_.device_loss_injections;
  device_->RemoveDevice();
  removed_ = true;
  if (FAILED(device_->GetDeviceRemovedReason())) {
    ++diagnostics_.device_loss_observed;
  }
}

void D3D12Presenter::CountDebugErrors() {
  if (!info_queue_) {
    return;
  }
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
      ++diagnostics_.debug_error_count;
    } else if (message->Severity == D3D12_MESSAGE_SEVERITY_CORRUPTION) {
      ++diagnostics_.debug_corruption_count;
    }
  }
}

void D3D12Presenter::Shutdown() {
  if (shutdown_) {
    return;
  }
  shutdown_ = true;
  if (!removed_) {
    WaitIdle();
  }
  CountDebugErrors();
  backing_.Reset();
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
}

}  // namespace doroti::validation
