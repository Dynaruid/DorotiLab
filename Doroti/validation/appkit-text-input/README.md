# AppKit text input regression

Run from the repository root on Apple Silicon with the macOS workload installed:

```powershell
dotnet build ./Doroti/validation/appkit-text-input -c Release
& "./Doroti/artifacts/validation/build/appkit-text-input/bin/Release/net10.0-macos/osx-arm64/Doroti.Validation.AppKitTextInput.app/Contents/MacOS/Doroti.Validation.AppKitTextInput"
```

The executable uses real AppKit controls and the pinned MAUI backend in an
unshown window. It links the product text bridge and layout adapter sources and
checks dynamic native attachment, initial selection, repeated focus requests,
native insertion, layout versus Return editing-end notifications, switching to
multiline input, and clearing a client with a pending focus request.

The notification checks distinguish submission causes; physical IME candidate
interaction is outside this fixture. Use the AppKit sample for interactive input.
