# Windows native ABI fixtures

## Text input regression

From the repository root:

```powershell
pwsh -NoProfile -File ./Doroti/eng/test-windows-text-input.ps1
```

The runner builds the Release native DLL, runs `--text-input` against that DLL,
then runs the mounted Material TextField `--mounted-text` contracts. Each process
has a 20-minute timeout; stdout/stderr are retained in a new `.doroti/evidence`
directory (or the supplied `-OutputDirectory`).

The native fixture posts `WM_CHAR` to hidden child and top-level host HWNDs and
checks emitted text, selection, edit counts and Enter actions. It covers the DEL
character produced after Ctrl+Backspace, repeated DEL on empty input, selection
preservation, Backspace/shortcut control filtering, ASCII/Korean insertion,
selection replacement and single/multiline Enter behavior. It does not send
global keyboard input or request a visible window.

The mounted fixture separately checks actual framework shortcut dispatch,
including repeated Ctrl+Backspace deleting one word at a time through empty text.
These are automated host-message and widget checks, not physical keyboard/IME
or visible Windows application acceptance.
