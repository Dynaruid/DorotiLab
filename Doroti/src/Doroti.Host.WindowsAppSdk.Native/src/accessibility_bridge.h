#pragma once

#include <windows.h>
#include <ole2.h>
#include <UIAutomation.h>

#include <cstdint>
#include <functional>
#include <memory>
#include <string>
#include <vector>

namespace doroti::windows {

struct AccessibilityTextRun {
  int start{}, end{}, line_start{}, line_end{};
  double left{}, top{}, right{}, bottom{};
  bool rtl{};
};
struct AccessibilityNode {
  int id{};
  double left{};
  double top{};
  double right{};
  double bottom{};
  std::wstring label;
  std::wstring value;
  std::wstring identifier;
  std::wstring hint;
  std::wstring tooltip;
  std::wstring link_url;
  std::wstring increased_value;
  std::wstring decreased_value;
  std::wstring min_value;
  std::wstring max_value;
  std::wstring role;
  int64_t actions{};
  std::vector<int> children;
  bool enabled{true};
  bool focusable{};
  bool focused{};
  bool hidden{};
  bool button{};
  bool text_field{};
  bool read_only{};
  bool slider{};
  bool mutually_exclusive{};
  bool header{};
  bool image{};
  bool live_region{};
  bool link{};
  bool obscured{};
  bool required{};
  int checked{-1};
  int selected{-1};
  int toggled{-1};
  int expanded{-1};
  int heading_level{};
  bool text_geometry{};
  uint64_t text_revision{};
  std::wstring layout_text;
  std::vector<int> text_boundaries;
  std::vector<AccessibilityTextRun> text_runs;
  int selection_base{-1}, selection_extent{-1};
};

class AccessibilityBridge final {
 public:
  struct State;
  using ActionCallback =
      std::function<void(int64_t node_id, int64_t action,
                         const std::wstring& arguments_json)>;

  AccessibilityBridge();
  ~AccessibilityBridge();
  AccessibilityBridge(const AccessibilityBridge&) = delete;
  AccessibilityBridge& operator=(const AccessibilityBridge&) = delete;

  void Attach(HWND window, ActionCallback callback);
  void Update(uint64_t generation, std::vector<AccessibilityNode> nodes,
              double scale);
  void Clear();
  void SetScale(double scale);
  bool ValidateAndInvokeForTest();
  bool ValidateTextForTest();
  LRESULT HandleGetObject(WPARAM wparam, LPARAM lparam);

 private:
  std::shared_ptr<State> state_;
};

}  // namespace doroti::windows
