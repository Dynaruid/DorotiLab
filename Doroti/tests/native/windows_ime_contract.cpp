#include "../../../packages/platforms/windowsappsdk/native/src/ime_editing_state.h"
#include <cstdlib>
#include <iostream>

using doroti::text_input::ImeEditingState;

void Check(bool condition, const char* message) {
  if (!condition) { std::cerr << "FAIL: " << message << '\n'; std::exit(1); }
}

int main() {
  ImeEditingState state;
  auto reset = [&] { state = {}; state.text = L"ab"; state.selection_base = state.selection_extent = 1; state.Start(); };
  auto preedit = [&] { state.Apply(std::nullopt, L"XY", 1); };
  auto committed = [&] { return state.text == L"aZb" && state.selection_extent == 2 && state.composing_base == -1; };
  reset(); preedit();
  Check(state.text == L"aXYb" && state.selection_extent == 2 && state.composing_base == 1 && state.composing_extent == 3,
        "preedit cursor is independent of full composition range");
  state.Apply(std::nullopt, std::nullopt, 0);
  Check(state.text == L"aXYb" && state.selection_extent == 1 && state.composing_extent == 3, "cursor-only beginning");
  state.Apply(std::nullopt, std::nullopt, 2);
  Check(state.selection_extent == 3, "cursor-only end");
  state.Apply(std::nullopt, std::nullopt, 99);
  Check(state.selection_extent == 3, "cursor upper bound");
  state.Apply(std::nullopt, std::nullopt, -1);
  Check(state.selection_extent == 3, "failed cursor query ignored");
  state.End();
  Check(state.text == L"ab" && state.selection_extent == 1 && !state.active && state.composing_base == -1, "cancel owned preedit");
  state.End();
  Check(state.text == L"ab" && state.selection_extent == 1, "END after cancel is idempotent");
  reset(); preedit(); state.Apply(L"Z", std::nullopt, std::nullopt); state.End();
  Check(committed(), "END after result preserves committed text");
  reset(); preedit(); state.Apply(std::nullopt, L"", 0); state.End();
  Check(state.text == L"ab" && state.selection_extent == 1, "empty preedit cancellation");
  reset(); state.selection_base = 0; state.selection_extent = 2;
  state.composing_base = state.composing_extent = -1; state.Start(); state.End();
  Check(state.text == L"ab" && state.selection_base == 0 && state.selection_extent == 2, "START/END without preedit preserves selected document");
  reset(); preedit(); state.Apply(L"Z", L"12", 1);
  Check(state.text == L"aZ12b" && state.selection_extent == 3 && state.composing_base == 2 && state.composing_extent == 4,
        "result and next preedit in one message");
  state.End(); Check(committed(), "cancel next preedit preserves preceding result");
  reset(); state.Apply(std::nullopt, L"\U0001F600\U0001F600", 2);
  Check(state.composing_extent == 5 && state.selection_extent == 3, "UTF-16 supplementary character cursor");
  state.Apply(L"\U0001F600", std::nullopt, std::nullopt); state.End();
  Check(state.text == L"a\U0001F600b" && state.selection_extent == 3, "UTF-16 commit");
  reset(); preedit(); state.Apply(std::nullopt, std::nullopt, std::nullopt);
  Check(state.text == L"aXYb" && state.composing_extent == 3, "failed string queries do not delete preedit");
  state.composing_base = 3; state.composing_extent = 1;
  state.Apply(std::nullopt, std::nullopt, 1);
  Check(state.selection_extent == 2, "reversed managed composition range cursor");
  std::cout << "PASS: 16 native IME state checks (synthetic, physical IME notVerified)\n";
}
