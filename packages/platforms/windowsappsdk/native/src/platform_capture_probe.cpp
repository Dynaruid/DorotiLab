// Opt-in, in-process renderer diagnostic. Captures this host's own client area;
// it is never part of the native-content or backdrop rendering transport.
#include <windows.h>
#include <dwmapi.h>
#include <d3d11.h>
#include <windows.graphics.capture.interop.h>
#include <windows.graphics.directx.direct3d11.interop.h>
#include <winrt/Windows.Foundation.h>
#include <winrt/Windows.Graphics.Capture.h>
#include <winrt/Windows.Graphics.DirectX.h>
#include <winrt/Windows.Graphics.DirectX.Direct3D11.h>
#include <atomic>
#include <cstdio>
#include <cstring>
#include <memory>
#include <string>
#include <vector>

extern "C" __declspec(dllexport) int32_t __cdecl doroti_windows_platform_capture_probe_v1(
    HWND window, const wchar_t* path, uint32_t frameCount) noexcept {
  using namespace winrt;
  using namespace winrt::Windows::Graphics::Capture;
  if (!IsWindow(window) || !path || frameCount == 0 || frameCount > 60) return E_INVALIDARG;
  DWORD process{};
  GetWindowThreadProcessId(window, &process);
  if (process != GetCurrentProcessId()) return E_ACCESSDENIED;
  try {
    init_apartment(apartment_type::multi_threaded);
    struct Apartment { ~Apartment() { uninit_apartment(); } } apartment;
    com_ptr<ID3D11Device> device;
    com_ptr<ID3D11DeviceContext> context;
    check_hresult(D3D11CreateDevice(nullptr, D3D_DRIVER_TYPE_HARDWARE, nullptr,
        D3D11_CREATE_DEVICE_BGRA_SUPPORT, nullptr, 0, D3D11_SDK_VERSION, device.put(), nullptr, context.put()));
    winrt::Windows::Foundation::IInspectable inspectable{nullptr};
    check_hresult(CreateDirect3D11DeviceFromDXGIDevice(device.as<IDXGIDevice>().get(), reinterpret_cast<IInspectable**>(put_abi(inspectable))));
    const auto captureDevice = inspectable.as<winrt::Windows::Graphics::DirectX::Direct3D11::IDirect3DDevice>();
    GraphicsCaptureItem item{nullptr};
    check_hresult(get_activation_factory<GraphicsCaptureItem, IGraphicsCaptureItemInterop>()->CreateForWindow(
        window, guid_of<GraphicsCaptureItem>(), put_abi(item)));
    const auto pool = Direct3D11CaptureFramePool::CreateFreeThreaded(captureDevice,
        winrt::Windows::Graphics::DirectX::DirectXPixelFormat::B8G8R8A8UIntNormalized, 2, item.Size());
    const auto session = pool.CreateCaptureSession(item);
    session.IsCursorCaptureEnabled(false);
    struct CaptureState {
      handle ready{CreateEventW(nullptr, TRUE, FALSE, nullptr)};
      std::atomic<HRESULT> result{E_PENDING};
      std::atomic<bool> captured{false};
      std::atomic<uint32_t> frames{0};
    };
    const auto state = std::make_shared<CaptureState>();
    if (!state->ready) throw_last_error();
    const auto token = pool.FrameArrived([state, device, context, window, frameCount, outputPath = std::wstring(path)](auto const& sender, auto const&) {
      auto frame = sender.TryGetNextFrame();
      if (!frame || state->captured.exchange(true)) return;
      HRESULT status = S_OK;
      try {
        const auto access = frame.Surface().template as<::Windows::Graphics::DirectX::Direct3D11::IDirect3DDxgiInterfaceAccess>();
        com_ptr<ID3D11Texture2D> texture;
        check_hresult(access->GetInterface(__uuidof(ID3D11Texture2D), texture.put_void()));
        D3D11_TEXTURE2D_DESC desc{};
        texture->GetDesc(&desc);
        desc.Usage = D3D11_USAGE_STAGING; desc.BindFlags = 0;
        desc.CPUAccessFlags = D3D11_CPU_ACCESS_READ; desc.MiscFlags = 0;
        com_ptr<ID3D11Texture2D> staging;
        check_hresult(device->CreateTexture2D(&desc, nullptr, staging.put()));
        context->CopyResource(staging.get(), texture.get());
        D3D11_MAPPED_SUBRESOURCE mapped{};
        check_hresult(context->Map(staging.get(), 0, D3D11_MAP_READ, 0, &mapped));
        RECT client{}, frameBounds{};
        POINT origin{};
        GetClientRect(window, &client); ClientToScreen(window, &origin);
        check_hresult(DwmGetWindowAttribute(window, DWMWA_EXTENDED_FRAME_BOUNDS, &frameBounds, sizeof(frameBounds)));
        const auto left = origin.x - frameBounds.left, top = origin.y - frameBounds.top;
        const auto width = client.right, height = client.bottom;
        if (left < 0 || top < 0 || width <= 0 || height <= 0 ||
            left + width > static_cast<LONG>(desc.Width) || top + height > static_cast<LONG>(desc.Height)) {
          context->Unmap(staging.get(), 0); throw hresult_invalid_argument();
        }
        std::vector<BYTE> pixels(static_cast<size_t>(width) * height * 4);
        for (LONG y = 0; y < height; ++y)
          memcpy(pixels.data() + static_cast<size_t>(y) * width * 4,
              static_cast<BYTE*>(mapped.pData) + static_cast<size_t>(top + y) * mapped.RowPitch + left * 4,
              static_cast<size_t>(width) * 4);
        context->Unmap(staging.get(), 0);
        BITMAPFILEHEADER fileHeader{};
        fileHeader.bfType = 0x4d42;
        fileHeader.bfOffBits = sizeof(BITMAPFILEHEADER) + sizeof(BITMAPINFOHEADER);
        fileHeader.bfSize = fileHeader.bfOffBits + static_cast<DWORD>(pixels.size());
        BITMAPINFOHEADER bitmap{};
        bitmap.biSize = sizeof(bitmap); bitmap.biWidth = width; bitmap.biHeight = -height;
        bitmap.biPlanes = 1; bitmap.biBitCount = 32; bitmap.biCompression = BI_RGB;
        FILE* file{};
        const auto filePath = state->frames.load() == 0 ? outputPath : outputPath + L"." + std::to_wstring(state->frames.load()) + L".bmp";
        if (_wfopen_s(&file, filePath.c_str(), L"wb") != 0 || !file) throw hresult_error(E_FAIL);
        const bool written = fwrite(&fileHeader, sizeof(fileHeader), 1, file) == 1 &&
            fwrite(&bitmap, sizeof(bitmap), 1, file) == 1 && fwrite(pixels.data(), pixels.size(), 1, file) == 1;
        fclose(file);
        if (!written) throw hresult_error(E_FAIL);
      } catch (...) { status = to_hresult(); }
      if (FAILED(status) || ++state->frames >= frameCount) {
        state->result.store(status);
        SetEvent(state->ready.get());
      } else state->captured.store(false);
    });
    session.StartCapture();
    const auto wait = WaitForSingleObject(state->ready.get(), 5000);
    pool.FrameArrived(token);
    session.Close(); pool.Close();
    return wait == WAIT_OBJECT_0 ? state->result.load() : HRESULT_FROM_WIN32(ERROR_TIMEOUT);
  } catch (...) { return to_hresult(); }
}
