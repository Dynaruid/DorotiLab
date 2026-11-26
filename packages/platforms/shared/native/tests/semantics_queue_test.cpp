#include "../semantics_queue.h"

#include <cassert>
#include <vector>

int main() {
  doroti::SemanticsQueue<int> queue, second_view;
  using Status = decltype(queue)::Status;
  int wake_count = 0;
  const auto wake = [&] { ++wake_count; return true; };
  assert(queue.Submit(1, 1, 0, false, wake) == Status::Accepted);
  assert(queue.Submit(2, 2, 1, false, wake) == Status::Accepted);
  assert(queue.Submit(3, 3, 1, false, wake) == Status::SnapshotRequired);
  assert(queue.Submit(30, 3, 0, false, wake) == Status::Accepted);
  assert(queue.Submit(4, 4, 3, false, wake) == Status::Accepted);
  assert(queue.Submit(0, 0, 0, true, wake) == Status::Accepted);
  assert(queue.Submit(11, 5, 4, false, wake) == Status::SnapshotRequired);
  assert(queue.Submit(99, 3, 0, false, wake) == Status::Stale);
  assert(queue.Submit(5, 5, 0, false, wake) == Status::Accepted);
  assert(wake_count == 1);
  assert(second_view.Submit(10, 1, 0, false, wake) == Status::Accepted);
  assert(wake_count == 2);
  std::vector<int> values;
  queue.Drain([&](auto item) {
    if (item.payload == 1) {
      assert(item.versioned && !item.patch);
      // Reentrant submission belongs behind the existing packets.
      assert(queue.Submit(6, 6, 5, false, wake) == Status::Accepted);
    }
    if (item.payload == 0) assert(item.clear);
    if (item.payload == 2 || item.payload == 4 || item.payload == 6) assert(item.patch);
    values.push_back(item.payload);
  });
  assert(values == std::vector<int>({1, 2, 30, 4, 0, 5, 6}));
  assert(wake_count == 2);
  second_view.Drain([](auto item) { assert(item.payload == 10); });
  assert(queue.Submit(7, 7, 6, false, [] { return false; }) == Status::Unavailable);
  assert(queue.Submit(7, 7, 6, false, wake) == Status::Accepted);
  queue.Drain([](auto item) { assert(item.payload == 7); });
}
