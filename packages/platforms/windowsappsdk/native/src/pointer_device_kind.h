#pragma once

#include <windows.h>
#include <cstdint>

namespace doroti::windows {

// Values follow Doroti/Flutter PointerDeviceKind in the native pointer ABI.
class PointerMessageKinds {
 public:
  constexpr uint32_t Cursor(uint32_t change, uint32_t extra_info) noexcept {
    // Leave/capture cancellation carries no origin signature.
    if (change != 0 && change != 2) cursor_kind_ = PromotedKind(extra_info);
    return cursor_kind_;
  }

  constexpr uint32_t Scroll(INPUT_MESSAGE_DEVICE_TYPE source,
                            uint32_t extra_info) const noexcept {
    switch (source) {
      case IMDT_TOUCHPAD: return 4;
      case IMDT_MOUSE: return 1;
      case IMDT_TOUCH: return 0;
      case IMDT_PEN: return 2;
      default: return PromotedKind(extra_info);
    }
  }

 private:
  static constexpr uint32_t PromotedKind(uint32_t extra_info) noexcept {
    return (extra_info & 0xffffff00u) == 0xff515700u
               ? ((extra_info & 0x80u) != 0 ? 0u : 2u) : 1u;
  }

  uint32_t cursor_kind_{1};
};

}  // namespace doroti::windows
