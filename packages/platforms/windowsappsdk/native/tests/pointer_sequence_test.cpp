#include "../src/pointer_sequence.h"

#include <cstdio>
#include <cstdlib>
#include <set>

namespace {
void Check(bool value, const char* message) {
  if (!value) {
    std::fprintf(stderr, "FAIL: %s\n", message);
    std::exit(1);
  }
}
}

int main() {
  doroti::windows::CursorPointerSequence cursor;
  Check(cursor.Apply(1, 0).identifier == 0, "device arrival is not a gesture");
  Check(cursor.Apply(3, 0).identifier == 0, "hover is not a gesture");
  std::set<uint32_t> completed;
  for (int i = 0; i < 20; ++i) {
    const auto down = cursor.Apply(4, i % 2 == 0 ? 1 : 2);
    Check(down.change == 4 && down.identifier != 0, "click starts a nonzero pointer");
    Check(!completed.contains(down.identifier), "left/right clicks cannot reuse an old arena");
    const auto move = cursor.Apply(5, i % 2 == 0 ? 1 : 2);
    const auto up = cursor.Apply(6, 0);
    Check(move.change == 5 && move.identifier == down.identifier, "drag retains its down identity");
    Check(up.change == 6 && up.identifier == down.identifier, "release retires the exact pointer");
    completed.insert(down.identifier);
    Check(cursor.Apply(3, 0).identifier == up.identifier, "hover does not allocate the next click");
  }

  const auto chord = cursor.Apply(4, 1);
  const auto second_button = cursor.Apply(4, 3);
  const auto partial_up = cursor.Apply(6, 2);
  const auto final_up = cursor.Apply(6, 0);
  Check(second_button.change == 5 && second_button.identifier == chord.identifier,
        "second button updates the active pointer instead of opening a second arena");
  Check(partial_up.change == 5 && partial_up.identifier == chord.identifier,
        "partial release keeps the remaining button in the active sequence");
  Check(final_up.change == 6 && final_up.identifier == chord.identifier,
        "only the last button ends the chord");

  const auto interrupted = cursor.Apply(4, 1);
  const auto cancel = cursor.Apply(0, 0);
  cursor.Apply(2, 0);
  cursor.Apply(1, 0);
  const auto next = cursor.Apply(4, 1);
  Check(cancel.identifier == interrupted.identifier && cancel.change == 0,
        "capture cancellation addresses the interrupted gesture");
  Check(next.identifier != interrupted.identifier && next.change == 4,
        "re-entry after cancellation starts a fresh gesture");
  doroti::windows::CursorPointerSequence other_window;
  const auto other_down = other_window.Apply(4, 1);
  Check(other_down.identifier != next.identifier && !completed.contains(other_down.identifier),
        "windows sharing framework arenas cannot allocate the same pointer");
  Check(cursor.Apply(5, 1).identifier == next.identifier,
        "another window cannot change the first window's active identity");
  std::puts("PASS: 20 distinct clicks, drag/release identity, button chords, capture cancellation, re-entry and multiple windows");
}
