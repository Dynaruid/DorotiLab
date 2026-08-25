#include <cstdint>
#include <winrt/base.h>
#include <winrt/Microsoft.UI.Content.h>

extern "C" __declspec(dllexport) std::int32_t
DorotiWinRtConfigureDesktopAttachedSiteBridge(
    void* value,
    std::int32_t processesKeyboardInput,
    std::int32_t processesPointerInput) noexcept
{
    if (value == nullptr)
    {
        return static_cast<std::int32_t>(0x80004003u);
    }

    try
    {
        winrt::Microsoft::UI::Content::DesktopAttachedSiteBridge bridge{ nullptr };
        winrt::copy_from_abi(bridge, value);
        bridge.ProcessesKeyboardInput(processesKeyboardInput != 0);
        bridge.ProcessesPointerInput(processesPointerInput != 0);
        return 0;
    }
    catch (...)
    {
        return winrt::to_hresult();
    }
}
