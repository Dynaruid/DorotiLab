# FCR-6 semantics physical checklist

Run separately on Android with TalkBack enabled and on Windows with a UI Automation client. Record the target, OS, backend, app revision, Flutter pin, raw event log, and MAUI semantics diagnostics.

- During continuous scroll, verify visible bounds advance in order while native semantics UI-thread time remains at or below the recorded 10% frame-budget threshold.
- Move accessibility focus through the first visible item, a text field, FAB, and Scrollbar. Each focus/action/value/selection change must appear on the next UI dispatch, not after the 15 fps scroll timer.
- Invoke scroll and button actions from TalkBack/UIA and verify the returned framework state and focus order.
- Verify a removed item is absent, a reused id has the new label/actions, and route changes replace the old tree.
- With ordinary touch/mouse, tap and drag beneath the accessibility overlay; FAB and canvas hit tests must still receive the input.
- Stop a drag and ballistic scroll, then verify the final bounds/action state is flushed. Dispose or navigate away during a pending scroll update and verify `StaleCallbacksSuppressed` rises without a callback to the old surface.

Do not mark this checklist passed from structural, Windows-live, browser, or another target's evidence.
