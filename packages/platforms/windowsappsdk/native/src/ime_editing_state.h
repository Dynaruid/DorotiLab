#pragma once

#include <algorithm>
#include <cstdint>
#include <optional>
#include <string>

namespace doroti::text_input {

// UTF-16 document offsets. The selected replacement range at START is not an
// owned preedit until the IME actually supplies a composition string.
struct ImeEditingState {
  std::wstring text;
  int32_t selection_base{};
  int32_t selection_extent{};
  int32_t composing_base{-1};
  int32_t composing_extent{-1};
  bool active{};
  bool owns_preedit{};

  void Start() {
    active = true;
    if (composing_base < 0) {
      composing_base = std::min(selection_base, selection_extent);
      composing_extent = std::max(selection_base, selection_extent);
      owns_preedit = false;
    }
  }

  void Replace(const std::wstring& replacement, bool composing) {
    const auto length = static_cast<int32_t>(text.size());
    const auto start = std::clamp(composing_base >= 0
        ? std::min(composing_base, composing_extent)
        : std::min(selection_base, selection_extent), 0, length);
    const auto end = std::clamp(composing_base >= 0
        ? std::max(composing_base, composing_extent)
        : std::max(selection_base, selection_extent), start, length);
    text.replace(static_cast<size_t>(start), static_cast<size_t>(end - start), replacement);
    selection_base = selection_extent = start + static_cast<int32_t>(replacement.size());
    composing_base = composing ? start : -1;
    composing_extent = composing ? selection_extent : -1;
    owns_preedit = composing && !replacement.empty();
  }

  void End() {
    if (owns_preedit) Replace(L"", false);
    composing_base = composing_extent = -1;
    owns_preedit = false;
    active = false;
  }

  void Apply(const std::optional<std::wstring>& result,
             const std::optional<std::wstring>& preedit,
             const std::optional<int32_t>& cursor) {
    if (result) { Replace(*result, false); active = false; }
    if (preedit) { Replace(*preedit, true); active = true; }
    if (cursor && *cursor >= 0 && composing_base >= 0) {
      const auto length = static_cast<int32_t>(text.size());
      const auto start = std::clamp(std::min(composing_base, composing_extent), 0, length);
      const auto end = std::clamp(std::max(composing_base, composing_extent), start, length);
      selection_base = selection_extent = start + std::clamp(*cursor, 0, end - start);
    }
  }
};

}  // namespace doroti::text_input
