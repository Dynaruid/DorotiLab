# R9 Win32 text input

- `SimpleWindow.cs` buffers `WM_CHAR` UTF-16 high surrogates and emits one scalar string when the low surrogate arrives.
- Invalid or interrupted surrogate input becomes one replacement character instead of throwing across the HWND callback.
- Selection, composition replacement/cancellation and IMM32 caret/candidate positioning remain in the Doroti-owned Win32 backend adapter; no Avalonia input client or public native type is imported.
