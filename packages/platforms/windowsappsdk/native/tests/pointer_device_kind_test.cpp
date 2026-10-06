#include "../src/pointer_device_kind.h"

using doroti::windows::PointerMessageKinds;

constexpr PointerMessageKinds kinds;
static_assert(kinds.Scroll(IMDT_TOUCHPAD, 0) == 4);
static_assert(kinds.Scroll(IMDT_MOUSE, 0) == 1);
static_assert(kinds.Scroll(IMDT_TOUCH, 0) == 0);
static_assert(kinds.Scroll(IMDT_PEN, 0) == 2);
// OS source wins over legacy metadata. No inference from wheel magnitude.
static_assert(kinds.Scroll(IMDT_TOUCHPAD, 0xff515780u) == 4);
static_assert(kinds.Scroll(IMDT_MOUSE, 0xff515780u) == 1);
static_assert(kinds.Scroll(IMDT_UNAVAILABLE, 0) == 1);
static_assert(kinds.Scroll(IMDT_UNAVAILABLE, 0xff515780u) == 0);
static_assert(kinds.Scroll(IMDT_UNAVAILABLE, 0xff515700u) == 2);
static_assert(kinds.Scroll(IMDT_UNAVAILABLE, 0xfe515780u) == 1);

static_assert([] {
  PointerMessageKinds session;
  if (session.Cursor(4, 0xff515700u) != 2) return false;
  if (session.Scroll(IMDT_TOUCHPAD, 0) != 4) return false;
  if (session.Cursor(0, 0) != 2) return false;  // pen cancellation
  if (session.Cursor(1, 0) != 1) return false;
  if (session.Scroll(IMDT_TOUCHPAD, 0) != 4) return false;
  if (session.Cursor(2, 0) != 1) return false;  // mouse leave
  if (session.Cursor(5, 0) != 1) return false;  // cursor drag
  return session.Scroll(IMDT_MOUSE, 0) == 1;
}());
