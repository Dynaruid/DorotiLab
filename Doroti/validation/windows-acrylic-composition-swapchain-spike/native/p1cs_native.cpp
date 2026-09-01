#include <windows.h>
#include <d3d11_4.h>
#include <dxgi1_6.h>
#include <dcomp.h>
#include <Presentation.h>
#include <wrl/client.h>
#include <cstdint>
#include <memory>

using Microsoft::WRL::ComPtr;

struct DorotiP1CsProbeSnapshot {
    std::uint32_t abi_version;
    std::uint32_t struct_size;
    std::int32_t factory_hresult;
    std::int32_t manager_hresult;
    std::int32_t surface_handle_hresult;
    std::int32_t presentation_surface_hresult;
    std::int32_t retiring_fence_hresult;
    std::uint32_t device_creation_flags;
    std::uint32_t presentation_supported;
    std::uint32_t independent_flip_supported;
    std::int32_t adapter_luid_low;
    std::int32_t adapter_luid_high;
    std::uint32_t adapter_vendor_id;
    std::uint32_t adapter_device_id;
    std::uint64_t retiring_fence_completed_value;
};

struct DorotiP1CsBufferSnapshot {
    std::uint32_t abi_version;
    std::uint32_t struct_size;
    std::int32_t texture_hresult;
    std::int32_t add_buffer_hresult;
    std::int32_t available_event_hresult;
    std::uint32_t width;
    std::uint32_t height;
    std::uint32_t format;
    std::uint32_t bind_flags;
    std::uint32_t misc_flags;
    std::uint32_t initially_available;
};

struct BufferSlot {
    ComPtr<ID3D11Texture2D> texture;
    ComPtr<IPresentationBuffer> buffer;
    HANDLE available_event{};
    std::uint32_t width{};
    std::uint32_t height{};
    std::uint64_t last_present_id{};

    void Reset() {
        buffer.Reset();
        texture.Reset();
        if (available_event) CloseHandle(available_event);
        available_event = nullptr;
        width = height = 0;
        last_present_id = 0;
    }

    ~BufferSlot() { Reset(); }
};

struct ProbeContext {
    ComPtr<ID3D11Device> device;
    ComPtr<IPresentationFactory> factory;
    ComPtr<IPresentationManager> manager;
    ComPtr<IPresentationSurface> surface;
    ComPtr<ID3D11Fence> retiring_fence;
    HANDLE surface_handle{};
    BufferSlot slots[3];

    ~ProbeContext() {
        retiring_fence.Reset();
        surface.Reset();
        manager.Reset();
        factory.Reset();
        device.Reset();
        if (surface_handle) CloseHandle(surface_handle);
    }
};

static HRESULT IsAvailable(BufferSlot const& slot, bool& available) {
    available = false;
    if (!slot.buffer) return S_FALSE;
    boolean value{};
    auto const result = slot.buffer->IsAvailable(&value);
    if (SUCCEEDED(result)) available = value != 0;
    return result;
}

static void QueryAdapter(ID3D11Device* device, DorotiP1CsProbeSnapshot& snapshot) {
    ComPtr<IDXGIDevice> dxgi_device;
    if (FAILED(device->QueryInterface(IID_PPV_ARGS(&dxgi_device)))) return;
    ComPtr<IDXGIAdapter> adapter;
    if (FAILED(dxgi_device->GetAdapter(&adapter))) return;
    DXGI_ADAPTER_DESC description{};
    if (FAILED(adapter->GetDesc(&description))) return;
    snapshot.adapter_luid_low = description.AdapterLuid.LowPart;
    snapshot.adapter_luid_high = description.AdapterLuid.HighPart;
    snapshot.adapter_vendor_id = description.VendorId;
    snapshot.adapter_device_id = description.DeviceId;
}

extern "C" __declspec(dllexport) std::int32_t doroti_p1cs_probe_create(
    void* d3d11_device,
    void** context,
    std::uint64_t* composition_surface_handle,
    DorotiP1CsProbeSnapshot* snapshot) noexcept {
    if (!d3d11_device || !context || !composition_surface_handle || !snapshot ||
        snapshot->struct_size < sizeof(DorotiP1CsProbeSnapshot)) return E_INVALIDARG;
    *context = nullptr;
    *composition_surface_handle = 0;
    auto probe = std::make_unique<ProbeContext>();
    auto* device = static_cast<ID3D11Device*>(d3d11_device);
    probe->device = device;
    snapshot->abi_version = 1;
    snapshot->struct_size = sizeof(DorotiP1CsProbeSnapshot);
    snapshot->device_creation_flags = device->GetCreationFlags();
    QueryAdapter(device, *snapshot);

    snapshot->factory_hresult = CreatePresentationFactory(
        device, IID_PPV_ARGS(&probe->factory));
    if (FAILED(snapshot->factory_hresult)) return snapshot->factory_hresult;
    snapshot->presentation_supported = probe->factory->IsPresentationSupported() ? 1u : 0u;
    snapshot->independent_flip_supported =
        probe->factory->IsPresentationSupportedWithIndependentFlip() ? 1u : 0u;
    if (!snapshot->presentation_supported) {
        *context = probe.release();
        return S_FALSE;
    }

    snapshot->manager_hresult = probe->factory->CreatePresentationManager(&probe->manager);
    if (FAILED(snapshot->manager_hresult)) return snapshot->manager_hresult;
    snapshot->surface_handle_hresult = DCompositionCreateSurfaceHandle(
        COMPOSITIONOBJECT_ALL_ACCESS, nullptr, &probe->surface_handle);
    if (FAILED(snapshot->surface_handle_hresult)) return snapshot->surface_handle_hresult;
    snapshot->presentation_surface_hresult = probe->manager->CreatePresentationSurface(
        probe->surface_handle, &probe->surface);
    if (FAILED(snapshot->presentation_surface_hresult)) return snapshot->presentation_surface_hresult;
    snapshot->retiring_fence_hresult = probe->manager->GetPresentRetiringFence(
        IID_PPV_ARGS(&probe->retiring_fence));
    if (SUCCEEDED(snapshot->retiring_fence_hresult) && probe->retiring_fence) {
        snapshot->retiring_fence_completed_value = probe->retiring_fence->GetCompletedValue();
    }
    *composition_surface_handle = reinterpret_cast<std::uint64_t>(probe->surface_handle);
    *context = probe.release();
    return S_OK;
}

extern "C" __declspec(dllexport) void doroti_p1cs_probe_destroy(void* context) noexcept {
    delete static_cast<ProbeContext*>(context);
}

extern "C" __declspec(dllexport) std::int32_t doroti_p1cs_replace_buffer(
    void* context,
    std::uint32_t slot_index,
    std::uint32_t width,
    std::uint32_t height,
    void** texture,
    std::uint64_t* available_event,
    DorotiP1CsBufferSnapshot* snapshot) noexcept {
    if (!context || slot_index >= 3 || !width || !height || !texture ||
        !available_event || !snapshot ||
        snapshot->struct_size < sizeof(DorotiP1CsBufferSnapshot)) return E_INVALIDARG;
    auto& probe = *static_cast<ProbeContext*>(context);
    auto& slot = probe.slots[slot_index];
    if (!probe.manager || !probe.surface) return E_HANDLE;
    if (slot.buffer) {
        bool available{};
        auto const available_result = IsAvailable(slot, available);
        if (FAILED(available_result)) return available_result;
        if (!available) return DXGI_ERROR_WAS_STILL_DRAWING;
        slot.Reset();
    }
    *texture = nullptr;
    *available_event = 0;
    snapshot->abi_version = 1;
    snapshot->struct_size = sizeof(DorotiP1CsBufferSnapshot);
    snapshot->width = width;
    snapshot->height = height;
    D3D11_TEXTURE2D_DESC description{};
    description.Width = width;
    description.Height = height;
    description.MipLevels = 1;
    description.ArraySize = 1;
    description.Format = DXGI_FORMAT_B8G8R8A8_UNORM;
    description.SampleDesc.Count = 1;
    description.SampleDesc.Quality = 0;
    description.Usage = D3D11_USAGE_DEFAULT;
    description.BindFlags = D3D11_BIND_SHADER_RESOURCE | D3D11_BIND_RENDER_TARGET;
    description.CPUAccessFlags = 0;
    description.MiscFlags = D3D11_RESOURCE_MISC_SHARED | D3D11_RESOURCE_MISC_SHARED_NTHANDLE;
    snapshot->format = description.Format;
    snapshot->bind_flags = description.BindFlags;
    snapshot->misc_flags = description.MiscFlags;
    snapshot->texture_hresult = probe.device->CreateTexture2D(
        &description, nullptr, &slot.texture);
    if (FAILED(snapshot->texture_hresult)) return snapshot->texture_hresult;
    snapshot->add_buffer_hresult = probe.manager->AddBufferFromResource(
        slot.texture.Get(), &slot.buffer);
    if (FAILED(snapshot->add_buffer_hresult)) {
        slot.Reset();
        return snapshot->add_buffer_hresult;
    }
    snapshot->available_event_hresult = slot.buffer->GetAvailableEvent(&slot.available_event);
    if (FAILED(snapshot->available_event_hresult)) {
        slot.Reset();
        return snapshot->available_event_hresult;
    }
    bool is_available{};
    auto const is_available_result = IsAvailable(slot, is_available);
    if (FAILED(is_available_result)) {
        slot.Reset();
        return is_available_result;
    }
    snapshot->initially_available = is_available ? 1u : 0u;
    slot.width = width;
    slot.height = height;
    slot.texture->AddRef();
    *texture = slot.texture.Get();
    *available_event = reinterpret_cast<std::uint64_t>(slot.available_event);
    return S_OK;
}

extern "C" __declspec(dllexport) std::int32_t doroti_p1cs_is_available(
    void* context, std::uint32_t slot_index, std::uint32_t* available) noexcept {
    if (!context || slot_index >= 3 || !available) return E_INVALIDARG;
    bool value{};
    auto const result = IsAvailable(
        static_cast<ProbeContext*>(context)->slots[slot_index], value);
    *available = value ? 1u : 0u;
    return result;
}

extern "C" __declspec(dllexport) std::int32_t doroti_p1cs_present(
    void* context,
    std::uint32_t slot_index,
    std::uint32_t width,
    std::uint32_t height,
    std::uint64_t tag,
    std::uint64_t* present_id,
    std::uint64_t* retiring_fence_value) noexcept {
    if (!context || slot_index >= 3 || !present_id || !retiring_fence_value) return E_INVALIDARG;
    auto& probe = *static_cast<ProbeContext*>(context);
    auto& slot = probe.slots[slot_index];
    if (!slot.buffer || slot.width != width || slot.height != height) return E_INVALIDARG;
    bool available{};
    auto result = IsAvailable(slot, available);
    if (FAILED(result)) return result;
    if (!available) return DXGI_ERROR_WAS_STILL_DRAWING;
    result = probe.surface->SetBuffer(slot.buffer.Get());
    if (FAILED(result)) return result;
    result = probe.surface->SetColorSpace(DXGI_COLOR_SPACE_RGB_FULL_G22_NONE_P709);
    if (FAILED(result)) return result;
    result = probe.surface->SetAlphaMode(DXGI_ALPHA_MODE_PREMULTIPLIED);
    if (FAILED(result)) return result;
    RECT source{0, 0, static_cast<LONG>(width), static_cast<LONG>(height)};
    result = probe.surface->SetSourceRect(&source);
    if (FAILED(result)) return result;
    probe.surface->SetTag(static_cast<UINT_PTR>(tag));
    auto const id = probe.manager->GetNextPresentId();
    result = probe.manager->Present();
    if (FAILED(result)) return result;
    slot.last_present_id = id;
    *present_id = id;
    *retiring_fence_value = probe.retiring_fence
        ? probe.retiring_fence->GetCompletedValue()
        : 0;
    return S_OK;
}

extern "C" __declspec(dllexport) std::int32_t doroti_p1cs_wait_for_available(
    void* context, std::uint32_t timeout_milliseconds, std::uint32_t* slot_index) noexcept {
    if (!context || !slot_index) return E_INVALIDARG;
    auto& probe = *static_cast<ProbeContext*>(context);
    HANDLE events[3]{};
    DWORD count{};
    std::uint32_t indices[3]{};
    for (std::uint32_t index = 0; index < 3; ++index) {
        if (probe.slots[index].available_event) {
            indices[count] = index;
            events[count++] = probe.slots[index].available_event;
        }
    }
    if (!count) return E_HANDLE;
    auto const wait = WaitForMultipleObjects(count, events, FALSE, timeout_milliseconds);
    if (wait >= WAIT_OBJECT_0 && wait < WAIT_OBJECT_0 + count) {
        *slot_index = indices[wait - WAIT_OBJECT_0];
        return S_OK;
    }
    if (wait == WAIT_TIMEOUT) return HRESULT_FROM_WIN32(WAIT_TIMEOUT);
    return HRESULT_FROM_WIN32(GetLastError());
}
