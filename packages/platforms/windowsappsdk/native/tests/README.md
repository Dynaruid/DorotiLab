# Windows cursor pointer regression

From the repository root, compile the portable production pointer-sequence
helper with a C++20 compiler, then run the regression with the required
20-minute timeout:

```powershell
New-Item -ItemType Directory -Path temp/native-tests -Force | Out-Null
clang++ -std=c++20 -Wall -Wextra -Werror packages/platforms/windowsappsdk/native/tests/pointer_sequence_test.cpp -o temp/native-tests/pointer-sequence-test.exe
python Doroti/eng/run-with-timeout.py --timeout 1200 temp/native-tests/pointer-sequence-test.exe
```

The test covers 20 alternating left/right clicks, stable identities through
drag/release, additional-button and partial-release transitions, capture
cancellation, cursor re-entry, and distinct identities across windows. Native
host rebuild and a Debug app interaction check are separate validation steps.

## Windows accessibility events regression

From an x64 Visual Studio Developer PowerShell, compile and run the test with
the Windows SDK that builds the native host.

```powershell
New-Item -ItemType Directory -Path temp/native-tests -Force | Out-Null
cl /nologo /std:c++20 /EHsc /W4 /WX /DNOMINMAX /DWIN32_LEAN_AND_MEAN /DUNICODE /D_UNICODE /Fo:temp/native-tests/ /Fe:temp/native-tests/accessibility-events-test.exe packages/platforms/windowsappsdk/native/tests/accessibility_events_test.cpp /link user32.lib ole32.lib oleaut32.lib uiautomationcore.lib uuid.lib
python Doroti/eng/run-with-timeout.py --timeout 1200 temp/native-tests/accessibility-events-test.exe
```

The test covers structure notifications for role and provider capability changes,
coordinate updates, and updates while UIA event subscriptions are inactive.
