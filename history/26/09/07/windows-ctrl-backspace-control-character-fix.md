# Windows Ctrl+Backspace control character repair

## Report and cause

The user observed a square glyph after Ctrl+Backspace in the Windows Release
DorotiTestbedApp TextField. The native `ProductHost::HandleCharacter` filtered C0
control characters (including Backspace), but accepted DEL (`U+007F`). After the
framework deleted a word, the trailing `WM_CHAR(DEL)` inserted that control
character into the text. The shared controller then displayed it in the other
sample field variants as well.

## Change

The shared Windows native host now consumes DEL in the same character filter as
the other shortcut control characters. Existing newline/action processing and
printable Unicode input retain their behavior. There is no sample-only filtering.

## Evidence

All test/build processes use a 20-minute timeout.

- `.doroti/evidence/ctrl-backspace-before`: reproduced FAIL against the original
  Release DLL: `one ` became `one \u007F`, selection `5:5`, one edit callback.
- `.doroti/evidence/ctrl-backspace-after`: Release native build PASS; ten real
  `WM_CHAR` cases each on hidden child and top-level HWNDs PASS. Cases cover DEL,
  empty/repeated input, selection, Backspace, shortcut controls, ASCII/Korean,
  printable input after DEL, replacement and single/multiline Enter.
- The same directory retains the mounted TextField PASS: Ctrl+A/C/X/V/Z,
  Ctrl+Shift+Z, Shift+Left, Ctrl+Home/Right, ordinary Backspace, repeated
  Ctrl+Backspace (`one two three` -> `one two ` -> `one ` -> empty -> empty),
  native text updates, caret scrolling and raster clipping.
- `app-build.stdout.log` in the same directory: the user's Windows Release target
  built successfully with zero warnings/errors using `doroti.ps1 build -App
  ./DorotiTestbedApp -Platform windows -Configuration Release`.

Reproduce with `pwsh -NoProfile -File ./Doroti/eng/test-windows-text-input.ps1`.
Native-message and mounted-widget checks are separate; physical Windows keyboard
and IME interaction / visible application acceptance remain notVerified.
The localhost 5088 server remains stopped as requested.
