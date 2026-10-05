#include <windows.h>
#include <d3d11_4.h>
#include <d3d10.h>
#include <dxgi1_6.h>
#include <mfapi.h>
#include <mferror.h>
#include <mfidl.h>
#include <mfreadwrite.h>
#include <winrt/base.h>
#include <memory>
#include <cstring>

namespace {
using winrt::com_ptr;
struct TextureFrame { com_ptr<ID3D11Texture2D> texture; HANDLE handle{}; int64_t luid{}; UINT width{}, height{}; };
struct FrameInfo { void* shared; uint32_t width, height; int64_t luid; };
int64_t Adapter(ID3D11Device* device) {
  com_ptr<IDXGIDevice> dxgi; winrt::check_hresult(device->QueryInterface(dxgi.put()));
  com_ptr<IDXGIAdapter> adapter; winrt::check_hresult(dxgi->GetAdapter(adapter.put()));
  DXGI_ADAPTER_DESC desc{}; winrt::check_hresult(adapter->GetDesc(&desc));
  int64_t luid{}; std::memcpy(&luid, &desc.AdapterLuid, sizeof(luid)); return luid;
}
com_ptr<ID3D11Device> Device(int64_t luid) {
  if (!luid) winrt::throw_hresult(E_INVALIDARG);
  com_ptr<IDXGIFactory4> factory; winrt::check_hresult(CreateDXGIFactory1(__uuidof(IDXGIFactory4), factory.put_void()));
  LUID id{}; std::memcpy(&id, &luid, sizeof(id));
  com_ptr<IDXGIAdapter> adapter; winrt::check_hresult(factory->EnumAdapterByLuid(id, __uuidof(IDXGIAdapter), adapter.put_void()));
  com_ptr<ID3D11Device> device;
  winrt::check_hresult(D3D11CreateDevice(adapter.get(), D3D_DRIVER_TYPE_UNKNOWN, nullptr,
      D3D11_CREATE_DEVICE_BGRA_SUPPORT | D3D11_CREATE_DEVICE_VIDEO_SUPPORT, nullptr, 0, D3D11_SDK_VERSION, device.put(), nullptr, nullptr));
  com_ptr<ID3D11DeviceContext> context; device->GetImmediateContext(context.put());
  auto multithread = context.as<ID3D10Multithread>(); multithread->SetMultithreadProtected(TRUE);
  return device;
}
void Wait(ID3D11Device* device, ID3D11DeviceContext* context) {
  com_ptr<ID3D11Query> query; D3D11_QUERY_DESC desc{D3D11_QUERY_EVENT, 0};
  winrt::check_hresult(device->CreateQuery(&desc, query.put())); context->End(query.get()); context->Flush();
  const auto deadline = GetTickCount64() + 5000;
  for (;;) {
    const auto hr = context->GetData(query.get(), nullptr, 0, 0);
    if (hr == S_OK) return;
    winrt::check_hresult(hr); winrt::check_hresult(device->GetDeviceRemovedReason());
    if (GetTickCount64() >= deadline) winrt::throw_hresult(HRESULT_FROM_WIN32(WAIT_TIMEOUT));
    Sleep(1);
  }
}
std::unique_ptr<TextureFrame> Allocate(ID3D11Device* device, UINT width, UINT height) {
  if (!width || !height || width > 8192 || height > 8192) winrt::throw_hresult(E_INVALIDARG);
  auto frame = std::make_unique<TextureFrame>(); frame->width = width; frame->height = height; frame->luid = Adapter(device);
  D3D11_TEXTURE2D_DESC desc{}; desc.Width=width; desc.Height=height; desc.MipLevels=desc.ArraySize=1;
  desc.Format=DXGI_FORMAT_B8G8R8A8_UNORM; desc.SampleDesc.Count=1; desc.Usage=D3D11_USAGE_DEFAULT;
  desc.BindFlags=D3D11_BIND_RENDER_TARGET|D3D11_BIND_SHADER_RESOURCE; desc.MiscFlags=D3D11_RESOURCE_MISC_SHARED;
  winrt::check_hresult(device->CreateTexture2D(&desc, nullptr, frame->texture.put()));
  winrt::check_hresult(frame->texture.as<IDXGIResource>()->GetSharedHandle(&frame->handle)); return frame;
}
std::unique_ptr<TextureFrame> Copy(ID3D11Texture2D* input, UINT subresource) {
  if (!input) winrt::throw_hresult(E_INVALIDARG);
  D3D11_TEXTURE2D_DESC desc{}; input->GetDesc(&desc);
  if (desc.MipLevels != 1 || subresource >= desc.ArraySize || desc.SampleDesc.Count != 1) winrt::throw_hresult(E_INVALIDARG);
  com_ptr<ID3D11Device> device; input->GetDevice(device.put());
  com_ptr<ID3D11DeviceContext> context; device->GetImmediateContext(context.put());
  auto multithread = context.as<ID3D10Multithread>(); multithread->SetMultithreadProtected(TRUE);
  auto frame = Allocate(device.get(), desc.Width, desc.Height);
  if (desc.Format == DXGI_FORMAT_B8G8R8A8_UNORM) {
    context->CopySubresourceRegion(frame->texture.get(), 0, 0, 0, 0, input, subresource, nullptr);
  } else {
    // The public conversion contract is SDR BT.709 limited-range YUV or full-range RGBA.
    // HDR/other colorimetry must be converted by the producer before publication.
    if(desc.Format!=DXGI_FORMAT_NV12 && desc.Format!=DXGI_FORMAT_YUY2 && desc.Format!=DXGI_FORMAT_R8G8B8A8_UNORM)
      winrt::throw_hresult(MF_E_UNSUPPORTED_D3D_TYPE);
    auto video = device.as<ID3D11VideoDevice>(); auto processing = context.as<ID3D11VideoContext>();
    D3D11_VIDEO_PROCESSOR_CONTENT_DESC content{}; content.InputFrameFormat=D3D11_VIDEO_FRAME_FORMAT_PROGRESSIVE;
    content.InputWidth=content.OutputWidth=desc.Width; content.InputHeight=content.OutputHeight=desc.Height;
    content.InputFrameRate={30,1}; content.OutputFrameRate={30,1}; content.Usage=D3D11_VIDEO_USAGE_PLAYBACK_NORMAL;
    com_ptr<ID3D11VideoProcessorEnumerator> enumerator; winrt::check_hresult(video->CreateVideoProcessorEnumerator(&content, enumerator.put()));
    UINT inputFlags{}, outputFlags{};
    winrt::check_hresult(enumerator->CheckVideoProcessorFormat(desc.Format, &inputFlags));
    winrt::check_hresult(enumerator->CheckVideoProcessorFormat(DXGI_FORMAT_B8G8R8A8_UNORM, &outputFlags));
    if (!(inputFlags & D3D11_VIDEO_PROCESSOR_FORMAT_SUPPORT_INPUT) || !(outputFlags & D3D11_VIDEO_PROCESSOR_FORMAT_SUPPORT_OUTPUT)) winrt::throw_hresult(MF_E_UNSUPPORTED_D3D_TYPE);
    com_ptr<ID3D11VideoProcessor> processor; winrt::check_hresult(video->CreateVideoProcessor(enumerator.get(), 0, processor.put()));
    D3D11_VIDEO_PROCESSOR_INPUT_VIEW_DESC inDesc{}; inDesc.ViewDimension=D3D11_VPIV_DIMENSION_TEXTURE2D; inDesc.Texture2D.ArraySlice=subresource;
    com_ptr<ID3D11VideoProcessorInputView> inView; winrt::check_hresult(video->CreateVideoProcessorInputView(input, enumerator.get(), &inDesc, inView.put()));
    D3D11_VIDEO_PROCESSOR_OUTPUT_VIEW_DESC outDesc{}; outDesc.ViewDimension=D3D11_VPOV_DIMENSION_TEXTURE2D;
    com_ptr<ID3D11VideoProcessorOutputView> outView; winrt::check_hresult(video->CreateVideoProcessorOutputView(frame->texture.get(), enumerator.get(), &outDesc, outView.put()));
    D3D11_VIDEO_PROCESSOR_COLOR_SPACE inputColor{}; inputColor.YCbCr_Matrix=1; inputColor.Nominal_Range=desc.Format==DXGI_FORMAT_R8G8B8A8_UNORM?2:1;
    D3D11_VIDEO_PROCESSOR_COLOR_SPACE outputColor{}; outputColor.Nominal_Range=2;
    processing->VideoProcessorSetStreamColorSpace(processor.get(), 0, &inputColor);
    processing->VideoProcessorSetOutputColorSpace(processor.get(), &outputColor);
    D3D11_VIDEO_PROCESSOR_STREAM stream{}; stream.Enable=TRUE; stream.pInputSurface=inView.get();
    winrt::check_hresult(processing->VideoProcessorBlt(processor.get(), outView.get(), 0, 1, &stream));
  }
  Wait(device.get(), context.get()); return frame;
}
struct Camera {
  com_ptr<IMFMediaSource> source;
  com_ptr<IMFSourceReader> reader;
  com_ptr<IMFDXGIDeviceManager> manager;
  com_ptr<ID3D11Device> device;
  bool com{}, mf{};
  ~Camera() { reader=nullptr; if(source) source->Shutdown(); source=nullptr; manager=nullptr; device=nullptr; if(mf) MFShutdown(); if(com) CoUninitialize(); }
};
}
#define TEXTURE_EXPORT extern "C" __declspec(dllexport) HRESULT __cdecl
TEXTURE_EXPORT doroti_texture_frame_info(void* value, FrameInfo* info) noexcept {
  if(!value||!info) return E_INVALIDARG; auto& f=*static_cast<TextureFrame*>(value); *info={f.handle,f.width,f.height,f.luid}; return S_OK;
}
extern "C" __declspec(dllexport) void __cdecl doroti_texture_frame_release(void* frame) noexcept { delete static_cast<TextureFrame*>(frame); }
TEXTURE_EXPORT doroti_texture_copy_d3d11(void* texture, UINT subresource, void** result) noexcept {
  if(!result) return E_INVALIDARG; *result=nullptr;
  try { *result=Copy(static_cast<ID3D11Texture2D*>(texture),subresource).release(); return S_OK; } catch(...) { return winrt::to_hresult(); }
}
TEXTURE_EXPORT doroti_texture_test_device(int64_t luid, void** result) noexcept {
  if(!result) return E_INVALIDARG; *result=nullptr;
  try { *result=Device(luid).detach(); return S_OK; } catch(...) { return winrt::to_hresult(); }
}
TEXTURE_EXPORT doroti_texture_test_frame(void* value, UINT step, void** result) noexcept {
  if(!value||!result) return E_INVALIDARG; *result=nullptr;
  try {
    auto* device=static_cast<ID3D11Device*>(value); auto frame=Allocate(device,320,180);
    com_ptr<ID3D11DeviceContext> context; device->GetImmediateContext(context.put()); auto context1=context.as<ID3D11DeviceContext1>();
    com_ptr<ID3D11RenderTargetView> target; winrt::check_hresult(device->CreateRenderTargetView(frame->texture.get(),nullptr,target.put()));
    const float colors[4][4]={{1,0,0,1},{0,1,0,1},{0,0,1,1},{1,1,0,1}};
    const D3D11_RECT rectangles[4]={{0,0,160,90},{160,0,320,90},{0,90,160,180},{160,90,320,180}};
    for(int i=0;i<4;i++) context1->ClearView(target.get(),colors[i],&rectangles[i],1);
    const float white[4]={1,1,1,1}; const LONG x=static_cast<LONG>(step%290); const D3D11_RECT moving{x,75,x+30,105};
    context1->ClearView(target.get(),white,&moving,1); Wait(device,context.get()); *result=frame.release(); return S_OK;
  } catch(...) { return winrt::to_hresult(); }
}
TEXTURE_EXPORT doroti_texture_camera_open(int64_t luid, UINT cameraIndex, void** result) noexcept {
  if(!result) return E_INVALIDARG; *result=nullptr;
  try {
    auto camera=std::make_unique<Camera>(); const auto com=CoInitializeEx(nullptr,COINIT_MULTITHREADED); winrt::check_hresult(com); camera->com=true;
    winrt::check_hresult(MFStartup(MF_VERSION)); camera->mf=true; camera->device=Device(luid);
    com_ptr<IMFAttributes> attributes; winrt::check_hresult(MFCreateAttributes(attributes.put(),1));
    winrt::check_hresult(attributes->SetGUID(MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE,MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID));
    IMFActivate** devices{}; UINT count{}; winrt::check_hresult(MFEnumDeviceSources(attributes.get(),&devices,&count));
    HRESULT activation=HRESULT_FROM_WIN32(ERROR_NOT_FOUND);
    if(cameraIndex<count) activation=devices[cameraIndex]->ActivateObject(__uuidof(IMFMediaSource),camera->source.put_void());
    for(UINT i=0;i<count;i++) devices[i]->Release(); CoTaskMemFree(devices); winrt::check_hresult(activation);
    UINT token{}; winrt::check_hresult(MFCreateDXGIDeviceManager(&token,camera->manager.put()));
    winrt::check_hresult(camera->manager->ResetDevice(camera->device.get(),token));
    com_ptr<IMFAttributes> options; winrt::check_hresult(MFCreateAttributes(options.put(),3));
    winrt::check_hresult(options->SetUnknown(MF_SOURCE_READER_D3D_MANAGER,camera->manager.get()));
    winrt::check_hresult(options->SetUINT32(MF_READWRITE_ENABLE_HARDWARE_TRANSFORMS,TRUE));
    winrt::check_hresult(options->SetUINT32(MF_SOURCE_READER_ENABLE_ADVANCED_VIDEO_PROCESSING,TRUE));
    winrt::check_hresult(MFCreateSourceReaderFromMediaSource(camera->source.get(),options.get(),camera->reader.put()));
    com_ptr<IMFMediaType> type; winrt::check_hresult(MFCreateMediaType(type.put()));
    winrt::check_hresult(type->SetGUID(MF_MT_MAJOR_TYPE,MFMediaType_Video)); winrt::check_hresult(type->SetGUID(MF_MT_SUBTYPE,MFVideoFormat_ARGB32));
    winrt::check_hresult(camera->reader->SetCurrentMediaType(static_cast<DWORD>(MF_SOURCE_READER_FIRST_VIDEO_STREAM),nullptr,type.get()));
    *result=camera.release(); return S_OK;
  } catch(...) { return winrt::to_hresult(); }
}
TEXTURE_EXPORT doroti_texture_camera_read(void* value, void** result) noexcept {
  if(!value||!result) return E_INVALIDARG; *result=nullptr;
  try {
    auto& camera=*static_cast<Camera*>(value); DWORD flags{}; com_ptr<IMFSample> sample;
    winrt::check_hresult(camera.reader->ReadSample(static_cast<DWORD>(MF_SOURCE_READER_FIRST_VIDEO_STREAM),0,nullptr,&flags,nullptr,sample.put()));
    if(flags & MF_SOURCE_READERF_ENDOFSTREAM) return MF_E_END_OF_STREAM;
    if(!sample) return S_FALSE;
    com_ptr<IMFMediaBuffer> buffer; winrt::check_hresult(sample->GetBufferByIndex(0,buffer.put()));
    auto dxgi=buffer.try_as<IMFDXGIBuffer>(); if(!dxgi) return MF_E_UNSUPPORTED_D3D_TYPE; // no CPU fallback
    com_ptr<ID3D11Texture2D> texture; UINT index{};
    winrt::check_hresult(dxgi->GetResource(__uuidof(ID3D11Texture2D),texture.put_void())); winrt::check_hresult(dxgi->GetSubresourceIndex(&index));
    *result=Copy(texture.get(),index).release(); return S_OK;
  } catch(...) { return winrt::to_hresult(); }
}
extern "C" __declspec(dllexport) void __cdecl doroti_texture_camera_stop(void* value) noexcept { if(value) static_cast<Camera*>(value)->source->Shutdown(); }
extern "C" __declspec(dllexport) void __cdecl doroti_texture_camera_close(void* value) noexcept { delete static_cast<Camera*>(value); }
