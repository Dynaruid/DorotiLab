#include "../../src/Doroti.Host.WindowsAppSdk.Native/src/accessibility_bridge.h"
#include <iostream>
#include <stdexcept>
#include <utility>

int main() {
  CoInitializeEx(nullptr, COINIT_APARTMENTTHREADED);
  const auto window = CreateWindowExW(0, L"STATIC", L"Accessibility projection validation", WS_POPUP,
      0, 0, 400, 300, nullptr, nullptr, GetModuleHandleW(nullptr), nullptr);
  if (!window) return 1;
  int result = 0;
  {
    using namespace doroti::windows;
    AccessibilityBridge bridge;
    std::vector<std::pair<int64_t, int64_t>> actions;
    bridge.Attach(window, [&](int64_t id, int64_t action, const auto&) { actions.emplace_back(id, action); });
    AccessibilityNode button; button.id = 1; button.button = true; button.actions = 1; button.children = {2, 3, 4}; button.right = 200; button.bottom = 40;
    AccessibilityNode check; check.id = 2; check.checked = 0; check.actions = 1;
    AccessibilityNode radio; radio.id = 3; radio.selected = 0; radio.mutually_exclusive = true; radio.actions = 1;
    AccessibilityNode slider; slider.id = 4; slider.slider = true; slider.value = L"0.2"; slider.min_value = L"0"; slider.max_value = L"1"; slider.actions = (1ll << 6) | (1ll << 7);
    for (int generation = 1; generation <= 4; ++generation) {
      check.checked = generation % 2; radio.selected = generation % 2;
      bridge.Update(generation, {button, check, radio, slider}, 1);
    }
    if (!actions.empty()) throw std::runtime_error("UIA state projection emitted an input action");
    // RadioListTile highlight is independent of the radio's checked state.
    // An unchecked but highlighted tile must still accept a Select request.
    radio.checked = 0; radio.selected = 1;
    bridge.Update(5, {button, check, radio, slider}, 1);
    if (!bridge.ValidateAndInvokeForTest()) throw std::runtime_error("UIA native action patterns failed");
    const std::vector<std::pair<int64_t, int64_t>> expected{{1, 1}, {2, 1}, {3, 1}, {4, 1ll << 6}};
    if (actions != expected) throw std::runtime_error("UIA actions were lost or duplicated");
    actions.clear(); bridge.Clear();
    if (!actions.empty()) throw std::runtime_error("UIA Clear emitted an input action");
    std::cout << "Windows UIA: repeated projection emits zero actions; Invoke/Toggle/Select/RangeValue emit exactly one action each; Clear PASS\n";
  }
  DestroyWindow(window); CoUninitialize();
  return result;
}
