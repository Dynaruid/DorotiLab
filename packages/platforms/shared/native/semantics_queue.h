#pragma once

#include <cstdint>
#include <deque>
#include <mutex>
#include <utility>

namespace doroti {

// One FIFO per native view. Acknowledging before returning to the producer
// makes a rejected base recoverable even if there is no next framework frame.
template <typename Payload>
class SemanticsQueue {
 public:
  struct Item {
    Payload payload;
    bool patch{};
    bool clear{};
    bool versioned{};
  };

  enum Status : std::uint32_t { Accepted = 0, SnapshotRequired = 2, Stale = 3, Unavailable = 4 };

  template <typename Wake>
  Status Submit(Payload payload, std::uint64_t revision, std::uint64_t base,
                bool clear, Wake wake) {
    std::lock_guard lock(mutex_);
    if (revision != 0) {
      if (revision <= latest_revision_) return Stale;
      if (base != 0 && base != accepted_base_) return SnapshotRequired;
    }
    items_.push_back({std::move(payload), base != 0, clear, revision != 0});
    if (!scheduled_) {
      if (!wake()) {
        items_.pop_back();
        return Unavailable;
      }
      scheduled_ = true;
    }
    accepted_base_ = revision;
    if (revision != 0) latest_revision_ = revision;
    return Accepted;
  }

  template <typename Apply>
  void Drain(Apply apply) {
    for (;;) {
      Item item;
      {
        std::lock_guard lock(mutex_);
        if (items_.empty()) {
          scheduled_ = false;
          return;
        }
        item = std::move(items_.front());
        items_.pop_front();
      }
      apply(std::move(item));
    }
  }

 private:
  std::mutex mutex_;
  std::deque<Item> items_;
  std::uint64_t latest_revision_{};
  std::uint64_t accepted_base_{};
  bool scheduled_{};
};

}  // namespace doroti
