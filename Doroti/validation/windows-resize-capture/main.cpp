#include <windows.h>
#include <d3d11.h>
#include <dwmapi.h>
#include <mmsystem.h>
#include <wincodec.h>
#include <windows.graphics.capture.interop.h>
#include <windows.graphics.directx.direct3d11.interop.h>

#include <winrt/base.h>
#include <winrt/Windows.Foundation.h>
#include <winrt/Windows.Graphics.h>
#include <winrt/Windows.Graphics.Capture.h>
#include <winrt/Windows.Graphics.DirectX.h>
#include <winrt/Windows.Graphics.DirectX.Direct3D11.h>

#include <algorithm>
#include <atomic>
#include <chrono>
#include <cmath>
#include <condition_variable>
#include <cstdint>
#include <cstdlib>
#include <cstring>
#include <filesystem>
#include <fstream>
#include <iomanip>
#include <iostream>
#include <mutex>
#include <optional>
#include <queue>
#include <sstream>
#include <stdexcept>
#include <string>
#include <thread>
#include <vector>

using namespace winrt;
using namespace winrt::Windows::Foundation;
using namespace winrt::Windows::Graphics;
using namespace winrt::Windows::Graphics::Capture;
using namespace winrt::Windows::Graphics::DirectX;
using namespace winrt::Windows::Graphics::DirectX::Direct3D11;

namespace {

struct Options {
    HWND hwnd{};
    HWND captureHwnd{};
    HWND visualHwnd{};
    std::wstring visualChildClass;
    std::filesystem::path output;
    std::string runId;
    std::string edge{"Left"};
    int durationSeconds{10};
    int inputHz{};
    int pngStride{1};
    int oracleStride{1};
    bool visualOracles{true};
    bool anomalyPngs{true};
};

struct WindowSample {
    long long performanceCounter{};
    int cursorX{};
    int cursorY{};
    int hitTest{};
    bool nonClientFallback{};
    RECT window{};
    RECT intendedWindow{};
};

struct FrameRecord {
    long long performanceCounter{};
    long long systemRelative100ns{};
    int width{};
    int height{};
    RECT window{};
    RECT client{};
    bool blank{};
    bool visualAnalyzed{};
    std::optional<double> appBarLogicalHeight;
    std::optional<double> circleAspect;
    std::optional<double> titleScaleRatio;
    int contentLeftGap{-1};
    int contentRightGap{-1};
    std::string png;
};

struct EncodedFrame {
    std::filesystem::path path;
    int width{};
    int height{};
    std::vector<std::uint8_t> pixels;
};

struct ActiveCallbackGuard {
    explicit ActiveCallbackGuard(std::atomic<int>& count) : count_(count) { count_.fetch_add(1); }
    ~ActiveCallbackGuard() { count_.fetch_sub(1); }
    std::atomic<int>& count_;
};

[[noreturn]] void Fail(std::string const& message) {
    throw std::runtime_error(message);
}

std::string EscapeJson(std::string const& value) {
    std::ostringstream stream;
    for (unsigned char character : value) {
        switch (character) {
        case '\\': stream << "\\\\"; break;
        case '"': stream << "\\\""; break;
        case '\n': stream << "\\n"; break;
        case '\r': stream << "\\r"; break;
        case '\t': stream << "\\t"; break;
        default:
            if (character < 0x20) {
                stream << "\\u" << std::hex << std::setw(4) << std::setfill('0') << static_cast<int>(character);
            } else {
                stream << character;
            }
        }
    }
    return stream.str();
}

std::string Narrow(std::filesystem::path const& value) {
    auto const wide = value.generic_wstring();
    if (wide.empty()) return {};
    int const size = WideCharToMultiByte(CP_UTF8, 0, wide.c_str(), static_cast<int>(wide.size()), nullptr, 0, nullptr, nullptr);
    std::string result(static_cast<std::size_t>(size), '\0');
    WideCharToMultiByte(CP_UTF8, 0, wide.c_str(), static_cast<int>(wide.size()), result.data(), size, nullptr, nullptr);
    return result;
}

std::string NarrowWide(std::wstring const& value) {
    if (value.empty()) return {};
    int const size = WideCharToMultiByte(CP_UTF8, 0, value.c_str(), static_cast<int>(value.size()), nullptr, 0, nullptr, nullptr);
    std::string result(static_cast<std::size_t>(size), '\0');
    WideCharToMultiByte(CP_UTF8, 0, value.c_str(), static_cast<int>(value.size()), result.data(), size, nullptr, nullptr);
    return result;
}

long long PerformanceCounter() {
    LARGE_INTEGER value{};
    QueryPerformanceCounter(&value);
    return value.QuadPart;
}

long long PerformanceFrequency() {
    LARGE_INTEGER value{};
    QueryPerformanceFrequency(&value);
    return value.QuadPart;
}

Options ParseOptions(int argc, wchar_t** argv) {
    Options options;
    for (int index = 1; index < argc; ++index) {
        std::wstring key = argv[index];
        auto next = [&]() -> std::wstring {
            if (++index >= argc) Fail("Missing value for a command-line option.");
            return argv[index];
        };
        if (key == L"--hwnd") options.hwnd = reinterpret_cast<HWND>(std::stoull(next(), nullptr, 0));
        else if (key == L"--output") options.output = next();
        else if (key == L"--visual-child-class") options.visualChildClass = next();
        else if (key == L"--run-id") {
            auto value = next();
            options.runId = NarrowWide(value);
        } else if (key == L"--edge") {
            auto value = next();
            options.edge = NarrowWide(value);
        } else if (key == L"--duration") options.durationSeconds = std::stoi(next());
        else if (key == L"--input-hz") options.inputHz = std::stoi(next());
        else if (key == L"--png-stride") options.pngStride = std::stoi(next());
        else if (key == L"--oracle-stride") options.oracleStride = std::stoi(next());
        else if (key == L"--capture-only") options.visualOracles = false;
        else if (key == L"--no-anomaly-png") options.anomalyPngs = false;
        else Fail("Unknown command-line option.");
    }
    if (!IsWindow(options.hwnd)) Fail("--hwnd must identify a live top-level window.");
    if (options.output.empty()) Fail("--output is required.");
    if (options.runId.empty()) Fail("--run-id is required.");
    if (options.durationSeconds < 1 || options.durationSeconds > 300) Fail("--duration must be between 1 and 300.");
    if (options.inputHz != 0 && (options.inputHz < 30 || options.inputHz > 1000)) {
        Fail("--input-hz must be 0 for display-refresh auto detection or between 30 and 1000.");
    }
    if (options.pngStride < 1 || options.pngStride > 1000) Fail("--png-stride must be between 1 and 1000.");
    if (options.oracleStride < 1 || options.oracleStride > 1000) Fail("--oracle-stride must be between 1 and 1000.");
    return options;
}

struct ChildWindowSearch {
    std::wstring className;
    HWND best{};
    long long bestArea{};
};

BOOL CALLBACK FindLargestChildByClass(HWND hwnd, LPARAM parameter) {
    auto& search = *reinterpret_cast<ChildWindowSearch*>(parameter);
    wchar_t className[256]{};
    if (GetClassNameW(hwnd, className, static_cast<int>(std::size(className))) == 0 ||
        _wcsicmp(className, search.className.c_str()) != 0 || !IsWindowVisible(hwnd)) {
        return TRUE;
    }
    RECT rect{};
    if (!GetWindowRect(hwnd, &rect)) return TRUE;
    long long const area = static_cast<long long>(std::max(0L, rect.right - rect.left)) *
        std::max(0L, rect.bottom - rect.top);
    if (area > search.bestArea) {
        search.best = hwnd;
        search.bestArea = area;
    }
    return TRUE;
}

HWND ResolveVisualWindow(Options const& options) {
    if (options.visualChildClass.empty()) return options.hwnd;
    ChildWindowSearch search{options.visualChildClass};
    EnumChildWindows(options.hwnd, FindLargestChildByClass, reinterpret_cast<LPARAM>(&search));
    if (!search.best) {
        Fail("No visible child window matched --visual-child-class.");
    }
    return search.best;
}

int DisplayRefreshRate(HWND hwnd) {
    HMONITOR monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
    MONITORINFOEX info{};
    info.cbSize = sizeof(info);
    if (!GetMonitorInfo(monitor, &info)) return 60;
    DEVMODE mode{};
    mode.dmSize = sizeof(mode);
    if (!EnumDisplaySettings(info.szDevice, ENUM_CURRENT_SETTINGS, &mode) || mode.dmDisplayFrequency <= 1) {
        return 60;
    }
    return static_cast<int>(mode.dmDisplayFrequency);
}

GraphicsCaptureItem CreateCaptureItem(HWND hwnd) {
    auto factory = get_activation_factory<GraphicsCaptureItem>();
    auto interop = factory.as<IGraphicsCaptureItemInterop>();
    GraphicsCaptureItem item{nullptr};
    check_hresult(interop->CreateForWindow(
        hwnd,
        guid_of<ABI::Windows::Graphics::Capture::IGraphicsCaptureItem>(),
        reinterpret_cast<void**>(put_abi(item))));
    return item;
}

IDirect3DDevice CreateWinRtDevice(com_ptr<ID3D11Device>& d3dDevice, com_ptr<ID3D11DeviceContext>& context) {
    UINT flags = D3D11_CREATE_DEVICE_BGRA_SUPPORT;
#if defined(_DEBUG)
    flags |= D3D11_CREATE_DEVICE_DEBUG;
#endif
    D3D_FEATURE_LEVEL featureLevel{};
    check_hresult(D3D11CreateDevice(
        nullptr,
        D3D_DRIVER_TYPE_HARDWARE,
        nullptr,
        flags,
        nullptr,
        0,
        D3D11_SDK_VERSION,
        d3dDevice.put(),
        &featureLevel,
        context.put()));
    auto dxgiDevice = d3dDevice.as<IDXGIDevice>();
    com_ptr<::IInspectable> inspectable;
    check_hresult(CreateDirect3D11DeviceFromDXGIDevice(dxgiDevice.get(), inspectable.put()));
    return inspectable.as<IDirect3DDevice>();
}

void EncodePng(std::filesystem::path const& path, int width, int height, std::vector<std::uint8_t> const& pixels) {
    com_ptr<IWICImagingFactory> factory;
    check_hresult(CoCreateInstance(CLSID_WICImagingFactory, nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(factory.put())));
    com_ptr<IWICStream> stream;
    check_hresult(factory->CreateStream(stream.put()));
    check_hresult(stream->InitializeFromFilename(path.c_str(), GENERIC_WRITE));
    com_ptr<IWICBitmapEncoder> encoder;
    check_hresult(factory->CreateEncoder(GUID_ContainerFormatPng, nullptr, encoder.put()));
    check_hresult(encoder->Initialize(stream.get(), WICBitmapEncoderNoCache));
    com_ptr<IWICBitmapFrameEncode> frame;
    check_hresult(encoder->CreateNewFrame(frame.put(), nullptr));
    check_hresult(frame->Initialize(nullptr));
    check_hresult(frame->SetSize(static_cast<UINT>(width), static_cast<UINT>(height)));
    WICPixelFormatGUID format = GUID_WICPixelFormat32bppBGRA;
    check_hresult(frame->SetPixelFormat(&format));
    UINT const stride = static_cast<UINT>(width * 4);
    check_hresult(frame->WritePixels(static_cast<UINT>(height), stride, static_cast<UINT>(pixels.size()), const_cast<BYTE*>(pixels.data())));
    check_hresult(frame->Commit());
    check_hresult(encoder->Commit());
}

class EncoderQueue {
public:
    EncoderQueue() : worker_([this] { Run(); }) {}
    ~EncoderQueue() { Stop(); }

    bool Enqueue(EncodedFrame&& frame) {
        std::lock_guard lock(mutex_);
        if (stopping_ || queue_.size() >= 256) {
            ++dropped_;
            return false;
        }
        queue_.push(std::move(frame));
        condition_.notify_one();
        return true;
    }

    void Stop() {
        {
            std::lock_guard lock(mutex_);
            if (stopping_) return;
            stopping_ = true;
        }
        condition_.notify_all();
        if (worker_.joinable()) worker_.join();
    }

    int Dropped() const { return dropped_.load(); }
    int Encoded() const { return encoded_.load(); }
    std::string Error() const { std::lock_guard lock(errorMutex_); return error_; }

private:
    void Run() {
        init_apartment(apartment_type::multi_threaded);
        for (;;) {
            EncodedFrame frame;
            {
                std::unique_lock lock(mutex_);
                condition_.wait(lock, [this] { return stopping_ || !queue_.empty(); });
                if (queue_.empty()) {
                    if (stopping_) return;
                    continue;
                }
                frame = std::move(queue_.front());
                queue_.pop();
            }
            try {
                EncodePng(frame.path, frame.width, frame.height, frame.pixels);
                ++encoded_;
            } catch (std::exception const& error) {
                std::lock_guard lock(errorMutex_);
                if (error_.empty()) error_ = error.what();
            }
        }
    }

    mutable std::mutex mutex_;
    std::condition_variable condition_;
    std::queue<EncodedFrame> queue_;
    bool stopping_{};
    std::thread worker_;
    std::atomic<int> dropped_{};
    std::atomic<int> encoded_{};
    mutable std::mutex errorMutex_;
    std::string error_;
};

bool IsPurple(std::uint8_t const* pixel) {
    int const b = pixel[0], g = pixel[1], r = pixel[2];
    return b - r > 25 && b - g > 35 && r > 45 && b > 100;
}

bool IsLavender(std::uint8_t const* pixel) {
    int const b = pixel[0], g = pixel[1], r = pixel[2];
    return b >= 100 && r >= 55 && b - r >= 20 && b - g >= 25;
}

std::uint8_t const* Pixel(std::vector<std::uint8_t> const& pixels, int width, int x, int y) {
    return pixels.data() + (static_cast<std::size_t>(y) * width + x) * 4;
}

RECT CaptureClientRect(HWND hwnd, RECT const& frameBounds, int width, int height) {
    RECT client{};
    GetClientRect(hwnd, &client);
    POINT topLeft{client.left, client.top};
    POINT bottomRight{client.right, client.bottom};
    ClientToScreen(hwnd, &topLeft);
    ClientToScreen(hwnd, &bottomRight);
    RECT mapped{
        std::clamp(topLeft.x - frameBounds.left, 0L, static_cast<LONG>(width)),
        std::clamp(topLeft.y - frameBounds.top, 0L, static_cast<LONG>(height)),
        std::clamp(bottomRight.x - frameBounds.left, 0L, static_cast<LONG>(width)),
        std::clamp(bottomRight.y - frameBounds.top, 0L, static_cast<LONG>(height))};
    return mapped;
}

bool IsBlank(std::vector<std::uint8_t> const& pixels, int width, RECT const& client) {
    int different = 0;
    int const clientWidth = std::max(1L, client.right - client.left);
    int const clientHeight = std::max(1L, client.bottom - client.top);
    for (int row = 1; row < 20; ++row) {
        for (int column = 1; column < 20; ++column) {
            int const x = std::min(client.right - 1, client.left + clientWidth * column / 20);
            int const y = std::min(client.bottom - 1, client.top + clientHeight * row / 20);
            auto pixel = Pixel(pixels, width, x, y);
            if (std::abs(static_cast<int>(pixel[2]) - 20) > 10 ||
                std::abs(static_cast<int>(pixel[1]) - 18) > 10 ||
                std::abs(static_cast<int>(pixel[0]) - 24) > 10) ++different;
        }
    }
    return different < 4;
}

std::optional<std::pair<int, int>> AppBarRows(
    std::vector<std::uint8_t> const& pixels, int width, int height, RECT const& client, double scale) {
    int first = -1, last = -1, gaps = 0;
    int const maxRow = std::min(client.bottom - 1, client.top + static_cast<int>(std::ceil(240 * scale)));
    int const clientWidth = std::max(1L, client.right - client.left);
    for (int row = client.top; row <= maxRow; ++row) {
        int purple = 0;
        for (int sample = 1; sample <= 48; ++sample) {
            int const column = std::min(client.right - 1, client.left + clientWidth * sample / 49);
            if (IsPurple(Pixel(pixels, width, column, row))) ++purple;
        }
        if (purple >= 24) {
            if (first < 0) first = row;
            last = row;
            gaps = 0;
        } else if (first >= 0 && ++gaps > 2) {
            break;
        }
    }
    if (first < 0 || last < first || first >= height) return std::nullopt;
    return std::pair{first, last};
}

std::optional<double> CircleAspect(
    std::vector<std::uint8_t> const& pixels, int width, RECT const& client, double scale) {
    struct Run { int row, left, right, width; };
    struct Component { int firstRow, lastRow, left, right; long long pixels; int maxRun; };
    int const minimumRun = std::max(3, static_cast<int>(std::floor(4 * scale)));
    int const maximumRun = std::max(minimumRun, static_cast<int>(std::ceil(90 * scale)));
    int const firstRow = std::min(client.bottom - 1, client.top + static_cast<int>(std::floor(80 * scale)));
    int const lastRow = client.bottom - 1;
    int const lastColumn = client.right - 1;
    std::vector<Run> runs;
    for (int row = firstRow; row <= lastRow; ++row) {
        int start = -1;
        for (int column = client.left; column <= lastColumn + 1; ++column) {
            bool const lavender = column <= lastColumn && IsLavender(Pixel(pixels, width, column, row));
            if (lavender && start < 0) start = column;
            else if (!lavender && start >= 0) {
                int const runWidth = column - start;
                if (runWidth >= minimumRun && runWidth <= maximumRun) runs.push_back({row, start, column - 1, runWidth});
                start = -1;
            }
        }
    }
    std::vector<Component> components;
    for (auto const& run : runs) {
        auto component = std::find_if(components.begin(), components.end(), [&](Component const& value) {
            return run.row - value.lastRow <= 2 && run.left <= value.right + 2 && run.right >= value.left - 2;
        });
        if (component == components.end()) {
            components.push_back({run.row, run.row, run.left, run.right, run.width, run.width});
        } else {
            component->lastRow = run.row;
            component->left = std::min(component->left, run.left);
            component->right = std::max(component->right, run.right);
            component->pixels += run.width;
            component->maxRun = std::max(component->maxRun, run.width);
        }
    }
    double bestArea = 0;
    std::optional<double> best;
    for (auto const& component : components) {
        int const componentWidth = component.right - component.left + 1;
        int const componentHeight = component.lastRow - component.firstRow + 1;
        double const logicalHeight = componentHeight / std::max(0.01, scale);
        double const fill = component.pixels / static_cast<double>(componentWidth * componentHeight);
        double const aspect = componentWidth / static_cast<double>(componentHeight);
        double const area = componentWidth * componentHeight;
        int const edgeTolerance = static_cast<int>(std::ceil(scale));
        bool const fullyVisible = component.left > client.left + edgeTolerance &&
            component.right < client.right - 1 - edgeTolerance &&
            component.firstRow > client.top + edgeTolerance &&
            component.lastRow < client.bottom - 1 - edgeTolerance;
        if (logicalHeight >= 18 && logicalHeight <= 72 && fill >= 0.55 && fill <= 0.99 &&
            std::abs(aspect - 1.0) <= 0.15 && fullyVisible && area > bestArea) {
            bestArea = area;
            best = aspect;
        }
    }
    return best;
}

std::optional<std::pair<int, int>> TitleBounds(
    std::vector<std::uint8_t> const& pixels, int width, RECT const& client, std::pair<int, int> appBar, double scale) {
    int minX = client.right, minY = appBar.second, maxX = -1, maxY = -1;
    int const left = std::min(client.right - 1, client.left + static_cast<int>(8 * scale));
    // Keep the sample inside the app-owned title. At narrow widths the native
    // caption buttons move left; the previous 420 logical-pixel ROI included
    // the minimize glyph and reported a false horizontal title stretch.
    int const right = std::min(client.right - 1, client.left + static_cast<int>(300 * scale));
    for (int y = appBar.first; y <= appBar.second; ++y) {
        for (int x = left; x <= right; ++x) {
            auto pixel = Pixel(pixels, width, x, y);
            int const b = pixel[0], g = pixel[1], r = pixel[2];
            if (r > 145 && g > 135 && b > 155) {
                minX = std::min(minX, x); maxX = std::max(maxX, x);
                minY = std::min(minY, y); maxY = std::max(maxY, y);
            }
        }
    }
    if (maxX < minX || maxY < minY) return std::nullopt;
    return std::pair{maxX - minX + 1, maxY - minY + 1};
}

class CaptureRunner {
public:
    explicit CaptureRunner(Options const& options)
        : options_(options), framesDirectory_(options.output.parent_path() / (options.runId + ".frames")) {
        std::filesystem::create_directories(framesDirectory_);
        device_ = CreateWinRtDevice(d3dDevice_, context_);
        item_ = CreateCaptureItem(options_.captureHwnd);
        lastSize_ = item_.Size();
        framePool_ = Direct3D11CaptureFramePool::CreateFreeThreaded(
            device_, DirectXPixelFormat::B8G8R8A8UIntNormalized, 3, lastSize_);
        session_ = framePool_.CreateCaptureSession(item_);
        try { session_.IsCursorCaptureEnabled(false); } catch (...) {}
        try { session_.IsBorderRequired(false); } catch (...) {}
        frameToken_ = framePool_.FrameArrived({this, &CaptureRunner::OnFrame});
    }

    ~CaptureRunner() { Stop(); }

    void Start() { session_.StartCapture(); }

    void Stop() {
        if (stopped_.exchange(true)) return;
        for (int wait = 0; wait < 5000 && activeCallbacks_.load() != 0; ++wait) Sleep(1);
        encoder_.Stop();
    }

    std::vector<FrameRecord> Frames() const { std::lock_guard lock(mutex_); return frames_; }
    int EncoderDropped() const { return encoder_.Dropped(); }
    int Encoded() const { return encoder_.Encoded(); }
    std::string EncoderError() const { return encoder_.Error(); }
    int CaptureErrors() const { return captureErrors_.load(); }

private:
    void OnFrame(
        Direct3D11CaptureFramePool const& sender,
        winrt::Windows::Foundation::IInspectable const&) noexcept {
        if (stopped_) return;
        ActiveCallbackGuard callbackGuard(activeCallbacks_);
        if (stopped_) return;
        try {
            auto frame = sender.TryGetNextFrame();
            if (!frame) return;
            auto const size = frame.ContentSize();
            if (size.Width <= 0 || size.Height <= 0) return;
            int const frameIndex = frameCount_.fetch_add(1);
            long long const captureCounter = PerformanceCounter();
            bool const encodeStrideFrame = frameIndex % options_.pngStride == 0;
            bool const analyzeFrame = options_.visualOracles;
            bool const analyzeShapeFrame = analyzeFrame && frameIndex % options_.oracleStride == 0;
            bool const needsPixels = analyzeFrame || encodeStrideFrame;
            std::vector<std::uint8_t> pixels;
            if (needsPixels) {
                auto access = frame.Surface().as<
                    ::Windows::Graphics::DirectX::Direct3D11::IDirect3DDxgiInterfaceAccess>();
                com_ptr<ID3D11Texture2D> source;
                check_hresult(access->GetInterface(__uuidof(ID3D11Texture2D), source.put_void()));
                D3D11_TEXTURE2D_DESC description{};
                source->GetDesc(&description);
                description.Width = static_cast<UINT>(size.Width);
                description.Height = static_cast<UINT>(size.Height);
                description.Usage = D3D11_USAGE_STAGING;
                description.BindFlags = 0;
                description.CPUAccessFlags = D3D11_CPU_ACCESS_READ;
                description.MiscFlags = 0;
                description.ArraySize = 1;
                description.MipLevels = 1;
                description.SampleDesc = {1, 0};
                com_ptr<ID3D11Texture2D> staging;
                check_hresult(d3dDevice_->CreateTexture2D(&description, nullptr, staging.put()));
                D3D11_BOX box{0, 0, 0, static_cast<UINT>(size.Width), static_cast<UINT>(size.Height), 1};
                context_->CopySubresourceRegion(staging.get(), 0, 0, 0, 0, source.get(), 0, &box);
                D3D11_MAPPED_SUBRESOURCE mapped{};
                check_hresult(context_->Map(staging.get(), 0, D3D11_MAP_READ, 0, &mapped));
                pixels.resize(static_cast<std::size_t>(size.Width) * size.Height * 4);
                for (int row = 0; row < size.Height; ++row) {
                    std::memcpy(
                        pixels.data() + static_cast<std::size_t>(row) * size.Width * 4,
                        static_cast<std::uint8_t const*>(mapped.pData) + static_cast<std::size_t>(row) * mapped.RowPitch,
                        static_cast<std::size_t>(size.Width) * 4);
                }
                context_->Unmap(staging.get(), 0);
            }

            RECT captureBounds{};
            if (FAILED(DwmGetWindowAttribute(options_.captureHwnd, DWMWA_EXTENDED_FRAME_BOUNDS,
                &captureBounds, sizeof(captureBounds)))) {
                GetWindowRect(options_.captureHwnd, &captureBounds);
            }
            // DriveResize records GetWindowRect. Keep the frame-side timing
            // sample in that same coordinate space; extended DWM bounds are
            // used only to map the captured bitmap's client inset.
            RECT window{};
            if (!GetWindowRect(options_.captureHwnd, &window)) window = captureBounds;
            RECT client{};
            if (!clientInsets_) {
                RECT initialClient{};
                if (options_.visualHwnd == options_.captureHwnd) {
                    initialClient = CaptureClientRect(
                        options_.captureHwnd, captureBounds, size.Width, size.Height);
                } else {
                    RECT visualBounds{};
                    if (!GetWindowRect(options_.visualHwnd, &visualBounds)) {
                        Fail("Could not resolve the visual child bounds.");
                    }
                    initialClient = RECT{
                        visualBounds.left - captureBounds.left,
                        visualBounds.top - captureBounds.top,
                        visualBounds.right - captureBounds.left,
                        visualBounds.bottom - captureBounds.top};
                }
                clientInsets_ = RECT{
                    initialClient.left,
                    initialClient.top,
                    size.Width - initialClient.right,
                    size.Height - initialClient.bottom};
            }
            client = RECT{
                std::clamp(clientInsets_->left, 0L, static_cast<LONG>(size.Width)),
                std::clamp(clientInsets_->top, 0L, static_cast<LONG>(size.Height)),
                std::clamp(static_cast<LONG>(size.Width) - clientInsets_->right, 0L, static_cast<LONG>(size.Width)),
                std::clamp(static_cast<LONG>(size.Height) - clientInsets_->bottom, 0L, static_cast<LONG>(size.Height))};
            UINT const dpi = GetDpiForWindow(options_.visualHwnd);
            double const scale = std::max(1.0, dpi / 96.0);
            std::optional<std::pair<int, int>> appBar;
            std::optional<double> circle;
            std::optional<double> titleScale;
            int leftGap = -1, rightGap = -1;
            if (analyzeFrame) {
                appBar = AppBarRows(pixels, size.Width, size.Height, client, scale);
            }
            if (analyzeShapeFrame) {
                circle = CircleAspect(pixels, size.Width, client, scale);
            }
            if (analyzeFrame && appBar) {
                auto title = TitleBounds(pixels, size.Width, client, *appBar, scale);
                if (title && !baselineTitle_) baselineTitle_ = title;
                if (title && baselineTitle_ && title->second > 0 && baselineTitle_->first > 0 && baselineTitle_->second > 0) {
                    double const xScale = title->first / static_cast<double>(baselineTitle_->first);
                    double const yScale = title->second / static_cast<double>(baselineTitle_->second);
                    titleScale = xScale / std::max(0.001, yScale);
                }
                int const middle = (appBar->first + appBar->second) / 2;
                for (int x = client.left; x < client.right; ++x) {
                    if (IsPurple(Pixel(pixels, size.Width, x, middle))) { leftGap = x - client.left; break; }
                }
                for (int x = client.right - 1; x >= client.left; --x) {
                    if (IsPurple(Pixel(pixels, size.Width, x, middle))) { rightGap = client.right - 1 - x; break; }
                }
            }

            FrameRecord record;
            record.performanceCounter = captureCounter;
            record.systemRelative100ns = frame.SystemRelativeTime().count();
            record.width = size.Width;
            record.height = size.Height;
            record.window = window;
            record.client = client;
            record.visualAnalyzed = analyzeFrame;
            record.blank = analyzeFrame && IsBlank(pixels, size.Width, client);
            if (appBar) record.appBarLogicalHeight = (appBar->second - appBar->first + 1) / scale;
            record.circleAspect = circle;
            record.titleScaleRatio = titleScale;
            record.contentLeftGap = leftGap;
            record.contentRightGap = rightGap;

            if (record.appBarLogicalHeight && !baselineAppBarHeight_) {
                baselineAppBarHeight_ = record.appBarLogicalHeight;
            }
            bool const oracleFailure = analyzeFrame && (record.blank || !record.appBarLogicalHeight ||
                (baselineAppBarHeight_ && record.appBarLogicalHeight &&
                    std::abs(*record.appBarLogicalHeight - *baselineAppBarHeight_) > 1.1) ||
                (record.circleAspect && std::abs(*record.circleAspect - 1.0) > std::max(1.0, std::ceil(scale)) / 18.0) ||
                (record.titleScaleRatio && std::abs(*record.titleScaleRatio - 1.0) > 0.04) ||
                leftGap > static_cast<int>(std::ceil(scale)) || rightGap > static_cast<int>(std::ceil(scale)));
            if (encodeStrideFrame || (oracleFailure && options_.anomalyPngs)) {
                std::ostringstream filename;
                filename << "frame-" << std::setw(6) << std::setfill('0') << frameIndex << ".png";
                auto const path = framesDirectory_ / filename.str();
                record.png = Narrow(std::filesystem::relative(path, options_.output.parent_path()));
                encoder_.Enqueue({path, size.Width, size.Height, std::move(pixels)});
            }
            {
                std::lock_guard lock(mutex_);
                frames_.push_back(std::move(record));
            }

            if (size.Width != lastSize_.Width || size.Height != lastSize_.Height) {
                lastSize_ = size;
                sender.Recreate(device_, DirectXPixelFormat::B8G8R8A8UIntNormalized, 3, lastSize_);
            }
        } catch (...) {
            ++captureErrors_;
        }
    }

    Options options_;
    std::filesystem::path framesDirectory_;
    com_ptr<ID3D11Device> d3dDevice_;
    com_ptr<ID3D11DeviceContext> context_;
    IDirect3DDevice device_{nullptr};
    GraphicsCaptureItem item_{nullptr};
    Direct3D11CaptureFramePool framePool_{nullptr};
    GraphicsCaptureSession session_{nullptr};
    event_token frameToken_{};
    SizeInt32 lastSize_{};
    std::atomic<bool> stopped_{};
    std::atomic<int> frameCount_{};
    std::atomic<int> captureErrors_{};
    std::atomic<int> activeCallbacks_{};
    mutable std::mutex mutex_;
    std::vector<FrameRecord> frames_;
    std::optional<double> baselineAppBarHeight_;
    std::optional<std::pair<int, int>> baselineTitle_;
    std::optional<RECT> clientInsets_;
    EncoderQueue encoder_;
};

struct MotionDefinition {
    bool left{}, right{}, top{}, bottom{};
};

MotionDefinition MotionForEdge(std::string const& edge) {
    if (edge == "Left") return {true, false, false, false};
    if (edge == "Right") return {false, true, false, false};
    if (edge == "Top") return {false, false, true, false};
    if (edge == "Bottom") return {false, false, false, true};
    if (edge == "TopLeft") return {true, false, true, false};
    if (edge == "TopRight") return {false, true, true, false};
    if (edge == "BottomLeft") return {true, false, false, true};
    if (edge == "BottomRight") return {false, true, false, true};
    Fail("Unsupported edge.");
}

void MovePointer(int x, int y) {
    int const virtualLeft = GetSystemMetrics(SM_XVIRTUALSCREEN);
    int const virtualTop = GetSystemMetrics(SM_YVIRTUALSCREEN);
    int const virtualWidth = std::max(2, GetSystemMetrics(SM_CXVIRTUALSCREEN));
    int const virtualHeight = std::max(2, GetSystemMetrics(SM_CYVIRTUALSCREEN));
    INPUT input{};
    input.type = INPUT_MOUSE;
    input.mi.dx = static_cast<LONG>(std::llround((x - virtualLeft) * 65535.0 / (virtualWidth - 1)));
    input.mi.dy = static_cast<LONG>(std::llround((y - virtualTop) * 65535.0 / (virtualHeight - 1)));
    input.mi.dwFlags = MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_VIRTUALDESK;
    if (SendInput(1, &input, sizeof(input)) != 1) Fail("Could not send native absolute pointer motion.");
}

class ResizeInputGuard {
public:
    explicit ResizeInputGuard(HWND hwnd) : hwnd_(hwnd) {
        timeBeginPeriod(1);
        previousThreadPriority_ = GetThreadPriority(GetCurrentThread());
        SetPriorityClass(GetCurrentProcess(), HIGH_PRIORITY_CLASS);
        SetThreadPriority(GetCurrentThread(), THREAD_PRIORITY_TIME_CRITICAL);
    }

    ~ResizeInputGuard() {
        INPUT input{};
        input.type = INPUT_MOUSE;
        input.mi.dwFlags = MOUSEEVENTF_LEFTUP;
        SendInput(1, &input, sizeof(input));
        // A capture/GetWindowRect failure must not strand the target inside its
        // modal non-client sizing loop. LEFTUP handles native SendInput drag;
        // WM_CANCELMODE also terminates the WM_NCLBUTTONDOWN fallback.
        if (hwnd_) PostMessageW(hwnd_, WM_CANCELMODE, 0, 0);
        SetThreadPriority(GetCurrentThread(), previousThreadPriority_);
        timeEndPeriod(1);
    }

    ResizeInputGuard(ResizeInputGuard const&) = delete;
    ResizeInputGuard& operator=(ResizeInputGuard const&) = delete;

private:
    HWND hwnd_{};
    int previousThreadPriority_{THREAD_PRIORITY_NORMAL};
};

std::vector<WindowSample> DriveResize(Options const& options) {
    ResizeInputGuard inputGuard(options.hwnd);
    auto const motion = MotionForEdge(options.edge);
    RECT rect{};
    if (!GetWindowRect(options.hwnd, &rect)) Fail("GetWindowRect failed.");
    int startX = (rect.left + rect.right) / 2;
    int startY = (rect.top + rect.bottom) / 2;
    int const expectedHitTest = motion.left && motion.top ? HTTOPLEFT :
        motion.right && motion.top ? HTTOPRIGHT :
        motion.left && motion.bottom ? HTBOTTOMLEFT :
        motion.right && motion.bottom ? HTBOTTOMRIGHT :
        motion.left ? HTLEFT : motion.right ? HTRIGHT : motion.top ? HTTOP : HTBOTTOM;
    DWORD const targetThread = GetWindowThreadProcessId(options.hwnd, nullptr);
    DWORD const currentThread = GetCurrentThreadId();
    for (int attempt = 0; attempt < 3 && GetAncestor(GetForegroundWindow(), GA_ROOT) != options.hwnd; ++attempt) {
        HWND const foreground = GetForegroundWindow();
        DWORD const foregroundThread = foreground ? GetWindowThreadProcessId(foreground, nullptr) : 0;
        bool const targetAttached = targetThread != 0 && targetThread != currentThread &&
            AttachThreadInput(currentThread, targetThread, TRUE);
        bool const foregroundAttached = foregroundThread != 0 && foregroundThread != currentThread &&
            foregroundThread != targetThread && AttachThreadInput(currentThread, foregroundThread, TRUE);
        ShowWindow(options.hwnd, SW_RESTORE);
        BringWindowToTop(options.hwnd);
        SetForegroundWindow(options.hwnd);
        SetActiveWindow(options.hwnd);
        SetFocus(options.hwnd);
        if (foregroundAttached) AttachThreadInput(currentThread, foregroundThread, FALSE);
        if (targetAttached) AttachThreadInput(currentThread, targetThread, FALSE);
        Sleep(100);
    }
    if (GetAncestor(GetForegroundWindow(), GA_ROOT) != options.hwnd) Fail("Could not activate the resize target window.");
    int hitTest = HTNOWHERE;
    for (int inset = -8; inset <= 16; ++inset) {
        int const candidateX = motion.left ? rect.left + inset :
            motion.right ? rect.right - 1 - inset : (rect.left + rect.right) / 2;
        int const candidateY = motion.top ? rect.top + inset :
            motion.bottom ? rect.bottom - 1 - inset : (rect.top + rect.bottom) / 2;
        DWORD_PTR result = HTNOWHERE;
        if (SendMessageTimeoutW(options.hwnd, WM_NCHITTEST, 0,
            MAKELPARAM(candidateX, candidateY), SMTO_ABORTIFHUNG, 1000, &result) &&
            static_cast<int>(result) == expectedHitTest) {
            startX = candidateX;
            startY = candidateY;
            hitTest = static_cast<int>(result);
            break;
        }
    }
    if (hitTest != expectedHitTest) Fail("Could not locate the requested native resize hit-test zone.");
    MovePointer(startX, startY);
    Sleep(150);
    POINT cursor{startX, startY};
    HWND cursorWindow = GetAncestor(WindowFromPoint(cursor), GA_ROOT);
    if (cursorWindow != options.hwnd) Fail("The resize cursor is not over the target window border.");
    INPUT down{};
    down.type = INPUT_MOUSE;
    down.mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
    if (SendInput(1, &down, sizeof(down)) != 1) Fail("Could not press the native pointer button.");
    Sleep(150);

    int const probeX = startX + (motion.left ? 8 : motion.right ? -8 : 0);
    int const probeY = startY + (motion.top ? 8 : motion.bottom ? -8 : 0);
    MovePointer(probeX, probeY);
    Sleep(100);
    RECT probeRect{};
    if (!GetWindowRect(options.hwnd, &probeRect)) Fail("GetWindowRect failed during native input probe.");
    bool const nonClientFallback = probeRect.left == rect.left && probeRect.top == rect.top &&
        probeRect.right == rect.right && probeRect.bottom == rect.bottom;
    if (nonClientFallback) {
        INPUT release{};
        release.type = INPUT_MOUSE;
        release.mi.dwFlags = MOUSEEVENTF_LEFTUP;
        SendInput(1, &release, sizeof(release));
        MovePointer(startX, startY);
        if (!PostMessageW(options.hwnd, WM_NCLBUTTONDOWN, static_cast<WPARAM>(expectedHitTest),
            MAKELPARAM(startX, startY))) {
            Fail("Could not initiate the native non-client resize fallback.");
        }
        Sleep(150);
    } else {
        MovePointer(startX, startY);
        Sleep(100);
    }

    long long const frequency = PerformanceFrequency();
    long long const interval = std::max(1LL, frequency / options.inputHz);
    long long const start = PerformanceCounter();
    long long const end = start + frequency * options.durationSeconds;
    long long deadline = start;
    std::vector<WindowSample> samples;
    samples.reserve(static_cast<std::size_t>(options.durationSeconds * options.inputHz + 8));
    while (deadline <= end) {
        for (;;) {
            long long const now = PerformanceCounter();
            long long const remaining = deadline - now;
            if (remaining <= 0) break;
            if (remaining > frequency / 500) Sleep(1);
            else YieldProcessor();
        }
        long long const now = PerformanceCounter();
        double const elapsed = (now - start) / static_cast<double>(frequency);
        double const cycle = std::fmod(elapsed, 2.0) / 2.0;
        double const wave = cycle < 0.5 ? cycle * 2.0 : 2.0 - cycle * 2.0;
        int const horizontal = motion.left ? static_cast<int>(std::lround(260 * wave)) :
            motion.right ? -static_cast<int>(std::lround(260 * wave)) : 0;
        int const vertical = motion.top ? static_cast<int>(std::lround(140 * wave)) :
            motion.bottom ? -static_cast<int>(std::lround(140 * wave)) : 0;
        int const cursorX = startX + horizontal;
        int const cursorY = startY + vertical;
        MovePointer(cursorX, cursorY);
        RECT expected = rect;
        if (motion.left) expected.left += horizontal;
        if (motion.right) expected.right += horizontal;
        if (motion.top) expected.top += vertical;
        if (motion.bottom) expected.bottom += vertical;
        RECT actual{};
        if (!GetWindowRect(options.hwnd, &actual)) Fail("GetWindowRect failed while resizing.");
        samples.push_back({now, cursorX, cursorY, hitTest, nonClientFallback, actual, expected});
        deadline += interval;
    }
    bool resized = false;
    for (auto const& sample : samples) {
        if (sample.window.left != rect.left || sample.window.top != rect.top ||
            sample.window.right != rect.right || sample.window.bottom != rect.bottom) {
            resized = true;
            break;
        }
    }
    if (!resized) Fail("Native pointer input did not resize the target window.");
    return samples;
}

template<typename T>
std::optional<double> Percentile(std::vector<T> values, double percentile) {
    if (values.empty()) return std::nullopt;
    std::sort(values.begin(), values.end());
    std::size_t const index = static_cast<std::size_t>(std::max(0.0, std::ceil(values.size() * percentile) - 1.0));
    return static_cast<double>(values[std::min(index, values.size() - 1)]);
}

void WriteOptional(std::ostream& stream, std::optional<double> value) {
    if (value) stream << std::fixed << std::setprecision(6) << *value;
    else stream << "null";
}

void WriteRect(std::ostream& stream, RECT const& value) {
    stream << "{\"left\":" << value.left << ",\"top\":" << value.top
        << ",\"right\":" << value.right << ",\"bottom\":" << value.bottom
        << ",\"width\":" << value.right - value.left << ",\"height\":" << value.bottom - value.top << "}";
}

void WriteEvidence(
    Options const& options,
    std::vector<WindowSample> const& samples,
    std::vector<FrameRecord> const& frames,
    CaptureRunner const& capture) {
    std::ofstream output(options.output, std::ios::binary);
    if (!output) Fail("Could not create the capture evidence file.");
    UINT const dpi = GetDpiForWindow(options.hwnd);
    std::vector<long long> inputIntervals;
    for (std::size_t index = 1; index < samples.size(); ++index) {
        inputIntervals.push_back(samples[index].performanceCounter - samples[index - 1].performanceCounter);
    }
    std::vector<long long> frameIntervals;
    std::vector<long long> callbackDeliveryIntervals;
    std::vector<long long> callbackBacklogs;
    long long const frequency = PerformanceFrequency();
    for (std::size_t index = 1; index < frames.size(); ++index) {
        long long const currentTimestamp = static_cast<long long>(std::llround(
            static_cast<double>(frames[index].systemRelative100ns) * frequency / 10'000'000.0));
        long long const previousTimestamp = static_cast<long long>(std::llround(
            static_cast<double>(frames[index - 1].systemRelative100ns) * frequency / 10'000'000.0));
        frameIntervals.push_back(currentTimestamp - previousTimestamp);
        callbackDeliveryIntervals.push_back(
            frames[index].performanceCounter - frames[index - 1].performanceCounter);
    }
    for (auto const& frame : frames) {
        long long const timestamp = static_cast<long long>(std::llround(
            static_cast<double>(frame.systemRelative100ns) * frequency / 10'000'000.0));
        callbackBacklogs.push_back(std::max(0LL, frame.performanceCounter - timestamp));
    }
    auto toMicroseconds = [&](std::optional<double> ticks) -> std::optional<double> {
        if (!ticks) return std::nullopt;
        return *ticks * 1'000'000.0 / frequency;
    };
    int blank = 0, appBarFailure = 0, circleFailure = 0, circleObserved = 0;
    int titleFailure = 0, titleObserved = 0, edgeGapFrames = 0;
    int maximumLeftGap = 0, maximumRightGap = 0;
    int currentGapFrames = 0, maximumConsecutiveGapFrames = 0;
    long long currentGapStart = 0, maximumGapDurationTicks = 0;
    double const scale = std::max(1.0, dpi / 96.0);
    std::optional<double> baselineAppBar;
    std::optional<int> baselineLeftGap;
    std::optional<int> baselineRightGap;
    bool activeVisualInterval = false;
    int visualAnalyzedFrames = 0;
    int finalLeftGap = -1, finalRightGap = -1;
    for (auto const& frame : frames) {
        long long const frameTimestamp = static_cast<long long>(std::llround(
            static_cast<double>(frame.systemRelative100ns) * frequency / 10'000'000.0));
        if (!frame.visualAnalyzed) continue;
        ++visualAnalyzedFrames;
        if (!baselineAppBar && frame.appBarLogicalHeight) {
            baselineAppBar = frame.appBarLogicalHeight;
            baselineLeftGap = frame.contentLeftGap;
            baselineRightGap = frame.contentRightGap;
            activeVisualInterval = true;
        }
        if (!activeVisualInterval) continue;
        if (frame.blank) ++blank;
        if (!frame.appBarLogicalHeight || (baselineAppBar &&
            std::abs(*frame.appBarLogicalHeight - *baselineAppBar) > 1.1)) ++appBarFailure;
        if (frame.circleAspect) {
            ++circleObserved;
            if (std::abs(*frame.circleAspect - 1.0) > std::max(1.0, std::ceil(scale)) / 18.0) ++circleFailure;
        }
        if (frame.titleScaleRatio) {
            ++titleObserved;
            if (std::abs(*frame.titleScaleRatio - 1.0) > 0.04) ++titleFailure;
        }
        int const leftGapDelta = baselineLeftGap ? std::max(0, frame.contentLeftGap - *baselineLeftGap) : frame.contentLeftGap;
        int const rightGapDelta = baselineRightGap ? std::max(0, frame.contentRightGap - *baselineRightGap) : frame.contentRightGap;
        maximumLeftGap = std::max(maximumLeftGap, leftGapDelta);
        maximumRightGap = std::max(maximumRightGap, rightGapDelta);
        finalLeftGap = leftGapDelta;
        finalRightGap = rightGapDelta;
        bool const hasContentGap = leftGapDelta > static_cast<int>(std::ceil(scale)) ||
            rightGapDelta > static_cast<int>(std::ceil(scale)) ||
            frame.contentLeftGap < 0 || frame.contentRightGap < 0;
        if (hasContentGap) {
            ++edgeGapFrames;
            if (currentGapFrames++ == 0) currentGapStart = frameTimestamp;
            maximumConsecutiveGapFrames = std::max(maximumConsecutiveGapFrames, currentGapFrames);
            maximumGapDurationTicks = std::max(maximumGapDurationTicks, frameTimestamp - currentGapStart);
        } else {
            currentGapFrames = 0;
            currentGapStart = 0;
        }
    }
    output << "{\n  \"schemaVersion\": \"doroti.windows-graphics-capture/v1\",\n";
    output << "  \"runId\": \"" << EscapeJson(options.runId) << "\",\n";
    output << "  \"captureApi\": \"Windows.Graphics.Capture/CreateForWindow + Direct3D11CaptureFramePool.CreateFreeThreaded\",\n";
    output << "  \"captureTarget\": \"top-level-window\",\n";
    output << "  \"visualRegion\": \"" <<
        (options.visualChildClass.empty() ? "top-level-client" : "largest-visible-child-class:") <<
        EscapeJson(NarrowWide(options.visualChildClass)) << "\",\n";
    output << "  \"visualOraclesEnabled\": " << (options.visualOracles ? "true" : "false") << ",\n";
    output << "  \"visualOracleStride\": " << options.oracleStride << ",\n";
    output << "  \"anomalyPngsEnabled\": " << (options.anomalyPngs ? "true" : "false") << ",\n";
    output << "  \"edge\": \"" << EscapeJson(options.edge) << "\",\n";
    output << "  \"durationSeconds\": " << options.durationSeconds << ",\n";
    output << "  \"displayRefreshHz\": " << DisplayRefreshRate(options.hwnd) << ",\n";
    output << "  \"inputHzRequested\": " << options.inputHz << ",\n";
    bool const nonClientFallback = !samples.empty() && samples.front().nonClientFallback;
    output << "  \"inputMethod\": \"native QPC-deadline absolute SendInput pointer motion; "
        << (nonClientFallback ? "WM_NCLBUTTONDOWN fallback + SendInput release" : "SendInput left-button down/up")
        << "\",\n";
    output << "  \"nonClientInitiationFallback\": " << (nonClientFallback ? "true" : "false") << ",\n";
    output << "  \"windowDpi\": " << dpi << ",\n";
    output << "  \"inputSamples\": " << samples.size() << ",\n";
    output << "  \"capturedFrames\": " << frames.size() << ",\n";
    output << "  \"encodedPngFrames\": " << capture.Encoded() << ",\n";
    output << "  \"encoderDroppedFrames\": " << capture.EncoderDropped() << ",\n";
    output << "  \"captureErrors\": " << capture.CaptureErrors() << ",\n";
    output << "  \"encoderError\": " << (capture.EncoderError().empty() ? "null" : "\"" + EscapeJson(capture.EncoderError()) + "\"") << ",\n";
    output << "  \"inputIntervalMicroseconds\": {\"p50\":"; WriteOptional(output, toMicroseconds(Percentile(inputIntervals, .50)));
    output << ",\"p95\":"; WriteOptional(output, toMicroseconds(Percentile(inputIntervals, .95)));
    output << ",\"p99\":"; WriteOptional(output, toMicroseconds(Percentile(inputIntervals, .99))); output << "},\n";
    output << "  \"captureIntervalMicroseconds\": {\"p50\":"; WriteOptional(output, toMicroseconds(Percentile(frameIntervals, .50)));
    output << ",\"p95\":"; WriteOptional(output, toMicroseconds(Percentile(frameIntervals, .95)));
    output << ",\"p99\":"; WriteOptional(output, toMicroseconds(Percentile(frameIntervals, .99))); output << "},\n";
    output << "  \"callbackDeliveryIntervalMicroseconds\": {\"p50\":"; WriteOptional(output, toMicroseconds(Percentile(callbackDeliveryIntervals, .50)));
    output << ",\"p95\":"; WriteOptional(output, toMicroseconds(Percentile(callbackDeliveryIntervals, .95)));
    output << ",\"p99\":"; WriteOptional(output, toMicroseconds(Percentile(callbackDeliveryIntervals, .99))); output << "},\n";
    output << "  \"callbackBacklogMicroseconds\": {\"p50\":"; WriteOptional(output, toMicroseconds(Percentile(callbackBacklogs, .50)));
    output << ",\"p95\":"; WriteOptional(output, toMicroseconds(Percentile(callbackBacklogs, .95)));
    output << ",\"p99\":"; WriteOptional(output, toMicroseconds(Percentile(callbackBacklogs, .99))); output << "},\n";
    output << "  \"visualOracle\": {\"analyzedFrames\":" << visualAnalyzedFrames
        << ",\"appBarBaselineLogicalHeight\":";
    WriteOptional(output, baselineAppBar);
    output << ",\"baselineContentLeftGapPixels\":" << (baselineLeftGap ? *baselineLeftGap : -1)
        << ",\"baselineContentRightGapPixels\":" << (baselineRightGap ? *baselineRightGap : -1);
    output << ",\"blankFrames\":" << blank
        << ",\"appBarHeightFailures\":" << appBarFailure
        << ",\"circleObservedFrames\":" << circleObserved
        << ",\"circleAspectFailures\":" << circleFailure
        << ",\"titleObservedFrames\":" << titleObserved
        << ",\"titleNonUniformScaleFailures\":" << titleFailure
        << ",\"contentEdgeGapFrames\":" << edgeGapFrames
        << ",\"maximumContentLeftGapPixels\":" << maximumLeftGap
        << ",\"maximumContentRightGapPixels\":" << maximumRightGap
        << ",\"maximumConsecutiveContentGapFrames\":" << maximumConsecutiveGapFrames
        << ",\"maximumContentGapDurationMicroseconds\":"
        << static_cast<long long>(std::llround(maximumGapDurationTicks * 1'000'000.0 / frequency))
        << ",\"finalContentLeftGapPixels\":" << finalLeftGap
        << ",\"finalContentRightGapPixels\":" << finalRightGap
        << "},\n";
    output << "  \"windowSamples\": [\n";
    for (std::size_t index = 0; index < samples.size(); ++index) {
        output << "    {\"performanceCounter\":" << samples[index].performanceCounter
            << ",\"cursorX\":" << samples[index].cursorX << ",\"cursorY\":" << samples[index].cursorY
            << ",\"hitTest\":" << samples[index].hitTest
            << ",\"nonClientFallback\":" << (samples[index].nonClientFallback ? "true" : "false")
            << ",\"window\":";
        WriteRect(output, samples[index].window);
        output << ",\"intendedWindow\":";
        WriteRect(output, samples[index].intendedWindow);
        output << "}" << (index + 1 == samples.size() ? "\n" : ",\n");
    }
    output << "  ],\n  \"frames\": [\n";
    for (std::size_t index = 0; index < frames.size(); ++index) {
        auto const& frame = frames[index];
        output << "    {\"performanceCounter\":" << frame.performanceCounter
            << ",\"systemRelative100ns\":" << frame.systemRelative100ns
            << ",\"width\":" << frame.width << ",\"height\":" << frame.height
            << ",\"window\":"; WriteRect(output, frame.window);
        output << ",\"client\":"; WriteRect(output, frame.client);
        output << ",\"blank\":" << (frame.blank ? "true" : "false")
            << ",\"visualAnalyzed\":" << (frame.visualAnalyzed ? "true" : "false")
            << ",\"appBarLogicalHeight\":"; WriteOptional(output, frame.appBarLogicalHeight);
        output << ",\"circleAspect\":"; WriteOptional(output, frame.circleAspect);
        output << ",\"titleScaleRatio\":"; WriteOptional(output, frame.titleScaleRatio);
        output << ",\"contentLeftGap\":" << frame.contentLeftGap
            << ",\"contentRightGap\":" << frame.contentRightGap
            << ",\"png\":" << (frame.png.empty() ? "null" : "\"" + EscapeJson(frame.png) + "\"")
            << "}" << (index + 1 == frames.size() ? "\n" : ",\n");
    }
    output << "  ]\n}\n";
}

} // namespace

int wmain(int argc, wchar_t** argv) {
    try {
        if (!SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2) &&
            GetLastError() != ERROR_ACCESS_DENIED) {
            Fail("Could not enable per-monitor-v2 DPI awareness.");
        }
        init_apartment(apartment_type::multi_threaded);
        Options options = ParseOptions(argc, argv);
        options.captureHwnd = options.hwnd;
        options.visualHwnd = ResolveVisualWindow(options);
        if (options.inputHz == 0) options.inputHz = DisplayRefreshRate(options.hwnd);
        std::filesystem::create_directories(options.output.parent_path());
        CaptureRunner capture(options);
        capture.Start();
        Sleep(750);
        auto samples = DriveResize(options);
        Sleep(750);
        capture.Stop();
        auto frames = capture.Frames();
        WriteEvidence(options, samples, frames, capture);
        std::cout << "EVIDENCE=" << Narrow(options.output) << "\n";
        std::cout << "INPUT_SAMPLES=" << samples.size() << "\n";
        std::cout << "CAPTURED_FRAMES=" << frames.size() << "\n";
        std::cout << "ENCODED_FRAMES=" << capture.Encoded() << "\n";
        int const exitCode = capture.CaptureErrors() != 0 || capture.EncoderDropped() != 0 ||
            !capture.EncoderError().empty() ? 2 : 0;
        std::cout.flush();
        std::cerr.flush();
        std::_Exit(exitCode);
    } catch (hresult_error const& error) {
        std::cerr << "HRESULT=0x" << std::hex << static_cast<unsigned int>(error.code().value)
            << " " << Narrow(std::filesystem::path(error.message().c_str())) << "\n";
        return 1;
    } catch (std::exception const& error) {
        std::cerr << error.what() << "\n";
        return 1;
    }
}
