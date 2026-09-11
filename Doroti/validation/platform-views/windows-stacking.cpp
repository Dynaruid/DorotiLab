// Independent PV-3 feasibility probe: generic HWND below/above a DComp target.
#include <windows.h>
#include <d3d11.h>
#include <dxgi1_2.h>
#include <dcomp.h>
#include <dwmapi.h>
#include <wrl/client.h>
#include <cstdio>
#include <stdexcept>
using Microsoft::WRL::ComPtr;
static int clicks = 0;
static LRESULT CALLBACK WindowProc(HWND hwnd, UINT message, WPARAM wparam, LPARAM lparam) {
  if (message == WM_COMMAND && HIWORD(wparam) == BN_CLICKED) ++clicks;
  return DefWindowProcW(hwnd, message, wparam, lparam);
}
static void Check(HRESULT hr) { if (FAILED(hr)) { std::printf("HRESULT=%08lx\n", hr); throw std::runtime_error("Windows API failed"); } }
static void Pump() {
  for (int i=0;i<30;++i) { MSG message; while (PeekMessageW(&message,nullptr,0,0,PM_REMOVE)) { TranslateMessage(&message); DispatchMessageW(&message); } Sleep(10); }
}
static COLORREF Pixel(HWND hwnd, int x, int y) {
  POINT point{x,y}; ClientToScreen(hwnd,&point);
  auto dc=GetDC(nullptr); auto color=GetPixel(dc,point.x,point.y); ReleaseDC(nullptr,dc); return color;
}
int main() {
 try {
  SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
  WNDCLASSW wc{}; wc.lpfnWndProc=WindowProc; wc.hInstance=GetModuleHandleW(nullptr); wc.lpszClassName=L"DorotiPlatformViewStackingProbe";
  RegisterClassW(&wc);
  HWND window=CreateWindowExW(0,wc.lpszClassName,L"Doroti PV-3 HWND stacking probe",WS_OVERLAPPEDWINDOW|WS_VISIBLE,100,100,500,350,nullptr,nullptr,wc.hInstance,nullptr);
  if (!window) throw std::runtime_error("CreateWindow failed");
  HWND button=CreateWindowExW(0,L"BUTTON",L"Native button",WS_CHILD|WS_VISIBLE|WS_TABSTOP|BS_PUSHBUTTON,40,40,160,60,window,(HMENU)1,wc.hInstance,nullptr);
  if (!button) throw std::runtime_error("Create button failed");
  ShowWindow(window,SW_SHOW); SetForegroundWindow(window); Pump();
  const auto baseline=Pixel(window,50,50);
  ComPtr<ID3D11Device> device; ComPtr<ID3D11DeviceContext> context;
  Check(D3D11CreateDevice(nullptr,D3D_DRIVER_TYPE_HARDWARE,nullptr,D3D11_CREATE_DEVICE_BGRA_SUPPORT,nullptr,0,D3D11_SDK_VERSION,&device,nullptr,&context));
  ComPtr<IDXGIDevice> dxgi; Check(device.As(&dxgi));
  ComPtr<IDXGIAdapter> adapter; Check(dxgi->GetAdapter(&adapter));
  ComPtr<IDXGIFactory2> factory; Check(adapter->GetParent(IID_PPV_ARGS(&factory)));
  DXGI_SWAP_CHAIN_DESC1 desc{}; desc.Width=500; desc.Height=350; desc.Format=DXGI_FORMAT_B8G8R8A8_UNORM; desc.SampleDesc.Count=1;
  desc.BufferUsage=DXGI_USAGE_RENDER_TARGET_OUTPUT; desc.BufferCount=2; desc.SwapEffect=DXGI_SWAP_EFFECT_FLIP_SEQUENTIAL; desc.AlphaMode=DXGI_ALPHA_MODE_IGNORE;
  ComPtr<IDXGISwapChain1> swapchain; Check(factory->CreateSwapChainForComposition(device.Get(),&desc,nullptr,&swapchain));
  ComPtr<ID3D11Texture2D> backbuffer; Check(swapchain->GetBuffer(0,IID_PPV_ARGS(&backbuffer)));
  ComPtr<ID3D11RenderTargetView> rtv; Check(device->CreateRenderTargetView(backbuffer.Get(),nullptr,&rtv));
  const float blue[]{0,0,1,1}; context->ClearRenderTargetView(rtv.Get(),blue); Check(swapchain->Present(1,0));
  COLORREF samples[2]{};
  for (int arm=0;arm<2;++arm) {
    if (arm==1) {
      DestroyWindow(window);
      window=CreateWindowExW(0,wc.lpszClassName,L"Doroti PV-3 lower DComp target probe",WS_OVERLAPPEDWINDOW|WS_VISIBLE|WS_CLIPCHILDREN,100,100,500,350,nullptr,nullptr,wc.hInstance,nullptr);
      button=CreateWindowExW(0,L"BUTTON",L"Native button",WS_CHILD|WS_VISIBLE|WS_TABSTOP|BS_PUSHBUTTON,40,40,160,60,window,(HMENU)1,wc.hInstance,nullptr);
      ShowWindow(window,SW_SHOW); SetForegroundWindow(window); Pump();
    }
    ComPtr<IDCompositionDevice> composition; Check(DCompositionCreateDevice(dxgi.Get(),IID_PPV_ARGS(&composition)));
    ComPtr<IDCompositionTarget> target; Check(composition->CreateTargetForHwnd(window,arm==0?TRUE:FALSE,&target));
    ComPtr<IDCompositionVisual> root; Check(composition->CreateVisual(&root)); Check(root->SetContent(swapchain.Get())); Check(target->SetRoot(root.Get()));
    Check(composition->Commit()); Check(composition->WaitForCommitCompletion());
    RedrawWindow(window,nullptr,nullptr,RDW_INVALIDATE|RDW_ALLCHILDREN|RDW_UPDATENOW);
    DwmFlush(); Pump();
    samples[arm]=Pixel(window,50,50);
    POINT point{80,65}; ClientToScreen(window,&point);
    const auto hit=WindowFromPoint(point);
    // BM_CLICK checks native command wiring only, not pointer visibility or real input latency.
    SendMessageW(button,BM_CLICK,0,0);
    std::printf("{\"arm\":\"%s\",\"baselinePixel\":%lu,\"samplePixel\":%lu,\"blue\":%lu,\"hitIsButton\":%s,\"nativeCommands\":%d,\"physical\":\"notVerified\"}\n",
      arm==0?"topmost":"below-child-hwnd",baseline,samples[arm],RGB(0,0,255),hit==button?"true":"false",clicks);
    Check(target->SetRoot(nullptr)); Check(composition->Commit()); Check(composition->WaitForCommitCompletion());
  }
  DestroyWindow(window);
  if (samples[0]!=RGB(0,0,255) || samples[1]==RGB(0,0,255) || samples[1]==CLR_INVALID || clicks!=2) return 2;
  return 0;
 } catch (...) { return 1; }
}
