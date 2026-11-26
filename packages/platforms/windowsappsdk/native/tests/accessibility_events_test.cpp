#include "../src/accessibility_bridge.h"

#include <cstdio>
#include <cstdlib>

namespace {

bool test_subscribed = true;
unsigned structure_changes = 0;

BOOL TestClientsAreListening() { return test_subscribed ? TRUE : FALSE; }

HRESULT CaptureStructureChange(IRawElementProviderSimple*, StructureChangeType type,
                               int*, int) {
  if (type == StructureChangeType_ChildrenInvalidated) ++structure_changes;
  return S_OK;
}

void Check(bool condition, const char* message) {
  if (condition) return;
  std::fputs(message, stderr);
  std::fputc('\n', stderr);
  std::exit(1);
}

}  // namespace

// Use the native bridge with stubbed UIA subscription and structure delivery.
#define UiaClientsAreListening TestClientsAreListening
#define UiaRaiseStructureChangedEvent CaptureStructureChange
#include "../src/accessibility_bridge.cpp"
#undef UiaRaiseStructureChangedEvent
#undef UiaClientsAreListening

int main() {
  doroti::windows::AccessibilityBridge bridge;
  bridge.Attach(reinterpret_cast<HWND>(std::uintptr_t{1}), {});
  doroti::windows::AccessibilityNode node;
  node.id = 1;
  node.role = L"none";
  node.label = L"Item";
  bridge.Update(1, {node}, 1);
  Check(structure_changes == 1, "initial snapshot must expose the node");

  bridge.Patch({{node.id, false, false, false, [](auto& item) { item.left = 12; }}}, {}, 1);
  Check(structure_changes == 1, "coordinate changes must keep the UIA tree stable");

  bridge.Patch({{node.id, false, true, false, [](auto& item) { item.role = L"link"; }}}, {}, 1);
  Check(structure_changes == 2, "role-only patches must refresh UIA control types");

  node.left = 12;
  node.role = L"link";
  node.button = true;
  bridge.Update(2, {node}, 1);
  Check(structure_changes == 3, "full snapshots must refresh changed provider capabilities");

  test_subscribed = false;
  bridge.Patch({{node.id, false, true, false, [](auto& item) { item.role = L"tab"; }}}, {}, 1);
  test_subscribed = true;
  node.role = L"tab";
  bridge.Update(3, {node}, 1);
  Check(structure_changes == 3, "provider changes must apply while notifications are inactive");

  std::puts("PASS: UIA structure notifications preserve provider changes and skip coordinate updates");
}
