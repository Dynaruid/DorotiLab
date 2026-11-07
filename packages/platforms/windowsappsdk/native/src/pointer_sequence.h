#pragma once

#include <atomic>
#include <cstdint>

namespace doroti::windows {

struct PointerSequenceUpdate {
  uint32_t change;
  uint32_t identifier;
};

// One cursor device can produce many distinct down/move/up sequences. Gesture
// arena entries from an old sequence must never address a later click's arena.
class CursorPointerSequence {
 public:
  PointerSequenceUpdate Apply(uint32_t change, int64_t buttons) noexcept {
    if (change == 4) {  // PointerChange.down
      if (!down_) {
        identifier_ = ++next_identifier_;
        if (identifier_ == 0) identifier_ = ++next_identifier_;
        down_ = true;
      } else {
        // Additional buttons belong to the current pointer, not a new down.
        change = 5;  // PointerChange.move
      }
    } else if (change == 6) {  // PointerChange.up
      if (buttons != 0) {
        change = 5;
      } else {
        down_ = false;
      }
    } else if (change == 0 || change == 2) {  // cancel / remove
      down_ = false;
    }
    return {change, identifier_};
  }

 private:
  // Arenas are shared by an application's views, so window-local counters
  // would let a newly opened window collide with an earlier window's entries.
  inline static std::atomic<uint32_t> next_identifier_{};
  uint32_t identifier_{};
  bool down_{};
};

}  // namespace doroti::windows
