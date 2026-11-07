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
