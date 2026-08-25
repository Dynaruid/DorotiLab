#pragma once

#include <windows.h>

#include <cstdint>
#include <wrl/client.h>

#include <d3d12.h>
#include <dxgi1_6.h>

namespace doroti::validation {

struct PresenterDiagnostics {
  uint64_t present_count{};
  uint64_t resize_count{};
  uint64_t wrong_size_present_count{};
  uint64_t debug_error_count{};
  uint64_t debug_corruption_count{};
  uint64_t device_loss_injections{};
  uint64_t device_loss_observed{};
};

class D3D12Presenter final {
 public:
  D3D12Presenter(HWND child, PresenterDiagnostics& diagnostics);
  ~D3D12Presenter();

  D3D12Presenter(const D3D12Presenter&) = delete;
  D3D12Presenter& operator=(const D3D12Presenter&) = delete;

  void ResizeAndPresent(uint32_t width, uint32_t height, uint64_t generation);
  void InjectDeviceLoss();
  void Shutdown();

  [[nodiscard]] uint32_t width() const noexcept { return width_; }
  [[nodiscard]] uint32_t height() const noexcept { return height_; }
  [[nodiscard]] uint64_t generation() const noexcept { return generation_; }

 private:
  void CreateDevice();
  void CreateOrResizeSwapChain(uint32_t width, uint32_t height);
  void CreateCheckerBacking(uint32_t width, uint32_t height,
                            uint64_t generation);
  void PresentExact(uint32_t width, uint32_t height, uint64_t generation);
  void ExecuteAndWait();
  void WaitIdle();
  void CountDebugErrors();

  HWND child_{};
  PresenterDiagnostics& diagnostics_;
  Microsoft::WRL::ComPtr<IDXGIFactory6> factory_;
  Microsoft::WRL::ComPtr<IDXGIAdapter1> adapter_;
  Microsoft::WRL::ComPtr<ID3D12Device5> device_;
  Microsoft::WRL::ComPtr<ID3D12CommandQueue> queue_;
  Microsoft::WRL::ComPtr<ID3D12CommandAllocator> allocator_;
  Microsoft::WRL::ComPtr<ID3D12GraphicsCommandList> command_list_;
  Microsoft::WRL::ComPtr<ID3D12Fence> fence_;
  Microsoft::WRL::ComPtr<ID3D12InfoQueue> info_queue_;
  Microsoft::WRL::ComPtr<IDXGISwapChain3> swap_chain_;
  Microsoft::WRL::ComPtr<ID3D12Resource> backing_;
  HANDLE fence_event_{};
  uint64_t fence_value_{};
  uint32_t width_{};
  uint32_t height_{};
  uint64_t generation_{};
  bool removed_{};
  bool shutdown_{};
};

}  // namespace doroti::validation
