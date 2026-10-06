# Web file picker activation

Sample2's Upload button previously dispatched to the managed Worker and then
requested an `input[type=file].click()` on the DOM owner. WebKit's file-input
activation checks the synchronous user-gesture scope, which can be lost during
that round trip even while `navigator.userActivation.isActive` remains true.
See [WebKit FileInputType](https://github.com/WebKit/WebKit/blob/main/Source/WebCore/html/FileInputType.cpp)
and its [asynchronous gesture discussion](https://bugs.webkit.org/show_bug.cgi?id=225559).
The former path can leave the sample waiting indefinitely for a change/cancel
event when the browser ignores the click.

`FilePickerActivation` registers a stable semantics identifier and normalized
options with the view's picker. The DOM host reserves the native picker in the
trusted pointer-up, keyboard or accessibility click before managed dispatch.
`PickFilesAsync` claims that result with matching options; it also handles a
selection completed before the Worker request arrives. File grants are created
only after admission, retain bounded reads, and are revoked on release/view
disposal. Cancel/unregister also discards unclaimed files. Disabled controls,
cancelled/draggable gestures and untrusted DOM events do not reserve a picker.
Native hosts use the interface's default registration and retain their own UI.

Validation on 2026-10-06, with uncommitted changes:

- Sample2 Release AOT publish passed; TypeScript strict compilation and
  `git diff --check` passed.
- `node --experimental-transform-types --experimental-vm-modules --test
  Doroti/tests/web_*.mts`: 80 passed, including six picker regressions for delayed
  admission, completed-before-claim results, disabled/unbound controls, concurrent
  requests, cancellation and ownership/grant revocation.
- `Doroti.Plugin.Tests` Release passed, including the default native binding,
  capability/cancellation, bounded reads and cleanup contracts.
- Chromium 148.0.7778.96, actual isolated Sample2 AOT application:
  `web_file_picker_smoke.py` passed two canvas selections with Korean text
  previews, cancellation, another selection by Enter, multiple/extension filters
  and rejection of a drag starting on the button. All four native picker calls
  occurred synchronously in trusted pointer-up/keydown dispatch. No page errors.
- Playwright WebKit 26.4: the production JS file service passed three native
  chooser operations with a 1.2-second Worker delay, bounded reads, cancellation
  and retry. The full Sample2 AOT canvas-input test timed out entering Upload;
  it is **not** qualified by this service-only result.
- Physical iPhone 12 / iOS Chrome: the existing test tab was reused. The changed
  AOT application started and its file activation binding reserved the filtered,
  multiple picker from an Inspector-emulated user gesture. A synthetic text File
  was passed through the existing Worker/managed preview path. This checks
  registration and data flow, **not physical finger input or native Files UI
  selection**. The original test URL was restored after verification.

Scratch logs and browser results are under `temp/testing/ios-file-picker/`.
Owned headless browser tabs close at the end of each run. The device test reused
one Chrome tab; no new device test tab was created.
