using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text.Json;
using Doroti.Host.WindowsAppSdk;
using Doroti.Ui;

namespace Doroti.Validation.WindowsAppSdkFlutterInput;

internal static partial class Program
{
    private const string ManualEvidenceSchema = "doroti.windowsappsdk-flutter-input-manual-evidence/v1";
    private const uint ManualWmPaint = 0x000f;
    private const uint ManualWmKeyDown = 0x0100;
    private const uint ManualWmKeyUp = 0x0101;
    private const uint ManualWmRightButtonUp = 0x0205;
    private const uint ManualVkEscape = 0x1b;
    private const uint ManualVkF1 = 0x70;
    private const uint ManualVkF6 = 0x75;
    private const uint ManualVkF8 = 0x77;
    private const uint ManualVkBack = 0x08;
    private const uint ManualVkDelete = 0x2e;
    private const uint ManualVkLeft = 0x25;
    private const uint ManualVkRight = 0x27;
    private const uint ManualVkA = 0x41;
    private const uint ManualVkC = 0x43;
    private const uint ManualVkV = 0x56;
    private const uint ManualVkX = 0x58;
    private const long ManualModifierShift = 1L << 0;
    private const long ManualModifierControl = 1L << 1;
    private const uint ManualDtLeft = 0x0000;
    private const uint ManualDtTop = 0x0000;
    private const uint ManualDtWordBreak = 0x0010;
    private const uint ManualDtNoPrefix = 0x0800;
    private const uint ManualMfString = 0x0000;
    private const uint ManualTpmReturnCommand = 0x0100;

    private static int RunManualAcceptance(F7Options options) =>
        RunOnDedicatedPlatformStaThread(() => RunManualAcceptanceOnPlatformThread(options));

    private static int RunManualAcceptanceOnPlatformThread(F7Options options)
    {
        if (string.IsNullOrWhiteSpace(options.EvidencePath))
            throw new ArgumentException("F7 --manual requires --evidence <path>.");

        var bootstrap = new FlutterWindowsAppSdkBootstrap();
        FlutterWindowsHostWindow? host = null;
        FlutterWindowsViewMetricsCoordinator? metrics = null;
        FlutterWindowsInputHost? input = null;
        FlutterWindowsUiaBridge? uia = null;
        var engineTasks = new QueueEngineTaskRunner();
        var state = new ManualAcceptanceState();
        try
        {
            host = FlutterWindowsHostWindow.CreateOnCurrentThread(
                bootstrap,
                new FlutterWindowsHostWindowOptions(
                    "Doroti F7 physical input, IME, and accessibility acceptance",
                    InitialClientWidth: 760,
                    InitialClientHeight: 620,
                    MinimumClientWidth: 520,
                    MinimumClientHeight: 420,
                    MaximumClientWidth: 1400,
                    MaximumClientHeight: 1000,
                    InitialX: 120,
                    InitialY: 90));
            state.Host = host;
            host.ChildMessageReceived += state.HandleChildMessage;
            host.CloseRequested += state.CancelAndQuit;

            metrics = FlutterWindowsViewMetricsCoordinator.AttachToHostWindow(
                host,
                viewId: 70_002,
                new FlutterWindowsPhysicalConstraints(520, 420, 1400, 1000));
            input = new FlutterWindowsInputHost(host, 70_002, () => metrics.Current);
            state.Input = input;
            input.PointerData += packet =>
            {
                state.PointerEventCount += packet.data.Count;
                state.Invalidate();
            };
            input.FocusData += data =>
            {
                if (data.isFocused) state.FocusGainCount++;
                else state.FocusLossCount++;
                state.Invalidate();
            };
            input.EditingStateChanged += editingState =>
            {
                state.EditingState = editingState;
                state.UpdateSemantics();
                state.Invalidate();
            };
            input.KeyData += state.HandleKeyData;
            input.SetClient(
                new DorotiTextInputConfiguration(
                    DorotiTextInputType.text,
                    DorotiTextInputAction.done,
                    DorotiTextCapitalization.none,
                    readOnly: false,
                    obscureText: false,
                    autocorrect: true,
                    enableSuggestions: true),
                state.EditingState);
            input.SetCaretRect(new Rect(24, 118, 12, 32));

            uia = FlutterWindowsUiaBridge.AttachToHostWindow(
                host,
                metrics,
                engineTasks,
                (nodeId, action, arguments) => state.HandleSemanticsAction(nodeId, action, arguments),
                () => input.RequestFocus(ViewFocusState.focused, ViewFocusDirection.undefined));
            state.Uia = uia;
            state.UpdateSemantics();

            host.NotifyFirstFrameSwapped();
            _ = ManualSetForegroundWindow(host.TopLevelHwnd);
            _ = ManualSetFocus(host.ViewHwnd);
            state.Invalidate();

            Console.WriteLine("F7 physical/manual window is ready. Complete the six checks shown in the window.");
            Console.WriteLine("Use F1-F6 to attest each observed result, then F8 to finish. Esc cancels.");
            while (ManualGetMessageW(out var message, 0, 0, 0) > 0)
            {
                _ = ManualTranslateMessage(in message);
                _ = ManualDispatchMessageW(in message);
                while (engineTasks.TryRunOneTask()) { }
            }

            var inputSnapshot = input.Snapshot;
            var textSnapshot = input.TextInputSnapshot;
            var uiaSnapshot = uia.Snapshot;
            var evidence = new
            {
                schemaVersion = ManualEvidenceSchema,
                status = state.Completed ? "PASS" : "notVerified",
                runId = Guid.NewGuid().ToString("N"),
                recordedAt = DateTimeOffset.UtcNow,
                observer = Environment.UserName,
                sourceFingerprint = options.SourceFingerprint ?? "unbound-local-run",
                selfAttestation = new
                {
                    realPointerCaptureAndBorderCursor = state.Checks[0],
                    focusAltTabMinimizeRestoreAndPopup = state.Checks[1],
                    koreanImeCandidateCaretSelectionClipboard = state.Checks[2],
                    narratorRootFragmentsAndActions = state.Checks[3],
                    accessibilityInsightsTreeAndPatterns = state.Checks[4],
                    resizeDpiInputAndUiaBounds = state.Checks[5],
                    allRequiredChecks = state.Checks.All(static value => value),
                    completedExplicitlyWithF8 = state.Completed,
                },
                runtime = new
                {
                    topLevelHwnd = host.TopLevelHwnd.ToInt64(),
                    childHwnd = host.ViewHwnd.ToInt64(),
                    pointerEventCount = state.PointerEventCount,
                    focusGainCount = state.FocusGainCount,
                    focusLossCount = state.FocusLossCount,
                    popupRoundTripCount = state.PopupRoundTripCount,
                    text = state.EditingState.text,
                    selectionBase = state.EditingState.selection.baseOffset,
                    selectionExtent = state.EditingState.selection.extentOffset,
                    compositionStartCount = textSnapshot.CompositionStartCount,
                    compositionUpdateCount = textSnapshot.CompositionUpdateCount,
                    compositionEndCount = textSnapshot.CompositionEndCount,
                    clipboardReadCount = inputSnapshot.ClipboardReadCount,
                    clipboardWriteCount = inputSnapshot.ClipboardWriteCount,
                    uiaRootProviderCount = uiaSnapshot.RootProviderCreationCount,
                    uiaFragmentCount = uiaSnapshot.FragmentProviderCount,
                    uiaDuplicateRootCount = uiaSnapshot.DuplicateRootProviderCount,
                    uiaBrokenFragmentCount = uiaSnapshot.BrokenFragmentCount,
                    uiaQueuedActionCount = uiaSnapshot.QueuedActionCount,
                },
                boundaries = new
                {
                    auxiliaryIslandCreated = false,
                    visibleProductAcceptanceVerified = false,
                    note = "This is an explicit human-observation F7 fixture. FG product/compositor acceptance remains separate.",
                },
            };
            var evidencePath = System.IO.Path.GetFullPath(options.EvidencePath);
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(evidencePath) ?? throw new InvalidOperationException(
                "The F7 manual evidence path has no parent directory."));
            File.WriteAllText(
                evidencePath,
                JsonSerializer.Serialize(evidence, new JsonSerializerOptions { WriteIndented = true }) + Environment.NewLine);
            Console.WriteLine($"F7 physical/manual evidence: {evidencePath}");
            return state.Completed ? 0 : 2;
        }
        finally
        {
            uia?.Dispose();
            input?.Dispose();
            metrics?.Dispose();
            if (host is not null) host.Dispose();
            else if (bootstrap.Snapshot.Phase != FlutterWindowsAppSdkBootstrapPhase.Disposed)
                bootstrap.DisposeOnCurrentThread();
        }
    }

    private sealed class ManualAcceptanceState
    {
        private static readonly string[] CheckLabels =
        [
            "F1 pointer capture outside child and standard resize-border cursor",
            "F2 Alt+Tab, minimize/restore, and right-click popup focus return",
            "F3 Korean two-beolsik composition/candidate/caret plus selection/copy/paste",
            "F4 Narrator sees one root and Invoke/Value/Scroll children and can act",
            "F5 Accessibility Insights sees one unbroken tree and expected patterns",
            "F6 resize/DPI change keeps pointer, caret, and UIA bounds aligned",
        ];

        internal FlutterWindowsHostWindow? Host { get; set; }
        internal FlutterWindowsInputHost? Input { get; set; }
        internal FlutterWindowsUiaBridge? Uia { get; set; }
        internal DorotiTextEditingState EditingState { get; set; } =
            new("Korean IME: ", new DorotiTextSelection(12, 12), null);
        internal bool[] Checks { get; } = new bool[CheckLabels.Length];
        internal bool Completed { get; private set; }
        internal int PointerEventCount { get; set; }
        internal int FocusGainCount { get; set; }
        internal int FocusLossCount { get; set; }
        internal int PopupRoundTripCount { get; private set; }

        internal FlutterWindowsChildMessageResult HandleChildMessage(FlutterWindowsChildMessage message)
        {
            if (message.Message == ManualWmPaint)
            {
                Paint(message.Hwnd);
                return FlutterWindowsChildMessageResult.HandledResult();
            }
            if (message.Message is ManualWmKeyDown or ManualWmKeyUp)
            {
                var key = unchecked((uint)message.WParam);
                if (key is >= ManualVkF1 and <= ManualVkF6)
                {
                    if (message.Message == ManualWmKeyDown)
                    {
                        Checks[key - ManualVkF1] = !Checks[key - ManualVkF1];
                        Invalidate();
                    }
                    return FlutterWindowsChildMessageResult.HandledResult();
                }
                if (key is ManualVkF8 or ManualVkEscape)
                {
                    if (message.Message == ManualWmKeyDown)
                    {
                        if (key == ManualVkF8 && Checks.All(static value => value)) Completed = true;
                        ManualPostQuitMessage(0);
                    }
                    return FlutterWindowsChildMessageResult.HandledResult();
                }
            }
            if (message.Message == ManualWmRightButtonUp)
            {
                ShowPopup(message.Hwnd);
                return FlutterWindowsChildMessageResult.HandledResult();
            }
            return FlutterWindowsChildMessageResult.Unhandled;
        }

        internal void HandleKeyData(KeyData data)
        {
            if (data.type is not (KeyEventType.down or KeyEventType.repeat) || Input is null) return;
            var key = unchecked((uint)data.logical);
            var control = (data.modifiers & ManualModifierControl) != 0;
            var shift = (data.modifiers & ManualModifierShift) != 0;
            var current = EditingState;
            if (control && key == ManualVkA)
            {
                ApplyState(new(current.text, new DorotiTextSelection(0, current.text.Length), null));
                return;
            }
            if (control && key is ManualVkC or ManualVkX)
            {
                var (start, length) = SelectionRange(current);
                if (length > 0)
                    Input.SetClipboardTextAsync(current.text.Substring(start, length)).AsTask().GetAwaiter().GetResult();
                if (key == ManualVkX && length > 0) ApplyState(ReplaceSelection(current, string.Empty));
                return;
            }
            if (control && key == ManualVkV)
            {
                var clipboard = Input.GetClipboardTextAsync().AsTask().GetAwaiter().GetResult() ?? string.Empty;
                ApplyState(ReplaceSelection(current, clipboard));
                return;
            }
            if (key is ManualVkLeft or ManualVkRight)
            {
                var next = Math.Clamp(current.selection.extentOffset + (key == ManualVkLeft ? -1 : 1), 0, current.text.Length);
                ApplyState(new(current.text,
                    new DorotiTextSelection(shift ? current.selection.baseOffset : next, next), null));
                return;
            }
            if (key is ManualVkBack or ManualVkDelete)
            {
                var (start, length) = SelectionRange(current);
                if (length == 0)
                {
                    if (key == ManualVkBack && start > 0) { start--; length = 1; }
                    else if (key == ManualVkDelete && start < current.text.Length) length = 1;
                }
                if (length > 0)
                {
                    var nextText = current.text.Remove(start, length);
                    ApplyState(new(nextText, new DorotiTextSelection(start, start), null));
                }
            }
        }

        internal void HandleSemanticsAction(int nodeId, SemanticsAction action, object? arguments)
        {
            if (action == SemanticsAction.setText && arguments is string value)
                ApplyState(new(value, new DorotiTextSelection(value.Length, value.Length), null));
            _ = nodeId;
            Invalidate();
        }

        internal void UpdateSemantics()
        {
            _ = Uia?.UpdateSemantics(new SemanticsUpdate(
                generation: DateTime.UtcNow.Ticks,
                nodes:
                [
                    new SemanticsNodeUpdate(0, new Rect(0, 0, 760, 620), "Doroti F7 acceptance", null,
                        SemanticsAction.none, [1, 2, 3], new SemanticsFlags(isEnabled: Tristate.isTrue)),
                    new SemanticsNodeUpdate(1, new Rect(24, 70, 220, 110), "Invoke acceptance action", null,
                        SemanticsAction.tap, [], new SemanticsFlags(isEnabled: Tristate.isTrue, isButton: true)),
                    new SemanticsNodeUpdate(2, new Rect(24, 112, 710, 158), "Korean IME text field", EditingState.text,
                        SemanticsAction.setText, [], new SemanticsFlags(isEnabled: Tristate.isTrue, isTextField: true)),
                    new SemanticsNodeUpdate(3, new Rect(24, 166, 710, 260), "Scrollable acceptance region", null,
                        SemanticsAction.scrollDown | SemanticsAction.scrollUp | SemanticsAction.scrollToOffset,
                        [], new SemanticsFlags(isEnabled: Tristate.isTrue, hasImplicitScrolling: true),
                        scrollPosition: 40, scrollExtentMax: 100, scrollExtentMin: 0),
                ]));
        }

        internal void CancelAndQuit()
        {
            Completed = false;
            ManualPostQuitMessage(0);
        }

        internal void Invalidate()
        {
            if (Host?.ViewHwnd is { } hwnd and not 0) _ = ManualInvalidateRect(hwnd, 0, false);
        }

        private void ApplyState(DorotiTextEditingState next)
        {
            EditingState = next;
            Input?.UpdateState(next);
            UpdateSemantics();
            Invalidate();
        }

        private void ShowPopup(nint hwnd)
        {
            var menu = ManualCreatePopupMenu();
            if (menu == 0) return;
            try
            {
                _ = ManualAppendMenuW(menu, ManualMfString, 1, "Return focus to Doroti child");
                if (!ManualGetCursorPos(out var point)) return;
                if (ManualTrackPopupMenu(menu, ManualTpmReturnCommand, point.X, point.Y, 0, hwnd, 0) == 1)
                {
                    PopupRoundTripCount++;
                    _ = ManualSetFocus(hwnd);
                    Invalidate();
                }
            }
            finally
            {
                _ = ManualDestroyMenu(menu);
            }
        }

        private void Paint(nint hwnd)
        {
            var hdc = ManualBeginPaint(hwnd, out var paint);
            if (hdc == 0) return;
            try
            {
                if (!ManualGetClientRect(hwnd, out var bounds)) return;
                var brush = ManualCreateSolidBrush(0x00f7f7f7);
                if (brush != 0)
                {
                    _ = ManualFillRect(hdc, in bounds, brush);
                    _ = ManualDeleteObject(brush);
                }
                _ = ManualSetBkMode(hdc, 1);
                _ = ManualSetTextColor(hdc, 0x00202020);
                var checklist = string.Join(Environment.NewLine,
                    CheckLabels.Select((label, index) => $"{(Checks[index] ? "[PASS]" : "[    ]")} {label}"));
                var selection = $"{EditingState.selection.baseOffset}..{EditingState.selection.extentOffset}";
                var text =
                    "Doroti F7 physical/manual acceptance\r\n" +
                    "Type in the child with Korean IME. Right-click tests popup focus. Use Narrator and Accessibility Insights.\r\n" +
                    $"Text: {EditingState.text}\r\nSelection: {selection}  Pointer events: {PointerEventCount}  Focus: +{FocusGainCount}/-{FocusLossCount}  Popup: {PopupRoundTripCount}\r\n\r\n" +
                    checklist +
                    "\r\n\r\nOnly after directly observing each item: F1-F6 toggle PASS. F8 completes. Esc/close records notVerified.";
                var textBounds = new ManualRect(20, 18, Math.Max(40, bounds.Right - 20), Math.Max(40, bounds.Bottom - 18));
                _ = ManualDrawTextW(hdc, text, text.Length, ref textBounds,
                    ManualDtLeft | ManualDtTop | ManualDtWordBreak | ManualDtNoPrefix);
            }
            finally
            {
                _ = ManualEndPaint(hwnd, in paint);
            }
        }

        private static (int Start, int Length) SelectionRange(DorotiTextEditingState state)
        {
            var start = Math.Min(state.selection.baseOffset, state.selection.extentOffset);
            var end = Math.Max(state.selection.baseOffset, state.selection.extentOffset);
            return (Math.Clamp(start, 0, state.text.Length), Math.Clamp(end - start, 0, state.text.Length - start));
        }

        private static DorotiTextEditingState ReplaceSelection(DorotiTextEditingState state, string replacement)
        {
            var (start, length) = SelectionRange(state);
            var text = state.text.Remove(start, length).Insert(start, replacement);
            var caret = start + replacement.Length;
            return new(text, new DorotiTextSelection(caret, caret), null);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct ManualPoint { internal int X; internal int Y; }

    [StructLayout(LayoutKind.Sequential)]
    private struct ManualRect
    {
        internal int Left;
        internal int Top;
        internal int Right;
        internal int Bottom;
        internal ManualRect(int left, int top, int right, int bottom) =>
            (Left, Top, Right, Bottom) = (left, top, right, bottom);
    }

    [StructLayout(LayoutKind.Sequential)]
    private unsafe struct ManualPaintStruct
    {
        internal nint Hdc;
        internal int Erase;
        internal ManualRect Paint;
        internal int Restore;
        internal int IncUpdate;
        internal fixed byte Reserved[32];
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct ManualMessage
    {
        internal nint Hwnd;
        internal uint Message;
        internal nuint WParam;
        internal nint LParam;
        internal uint Time;
        internal ManualPoint Point;
        internal uint Private;
    }

    [LibraryImport("user32.dll", EntryPoint = "GetMessageW", SetLastError = true)]
    private static partial int ManualGetMessageW(out ManualMessage message, nint hwnd, uint min, uint max);
    [LibraryImport("user32.dll", EntryPoint = "TranslateMessage")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ManualTranslateMessage(in ManualMessage message);
    [LibraryImport("user32.dll", EntryPoint = "DispatchMessageW")]
    private static partial nint ManualDispatchMessageW(in ManualMessage message);
    [LibraryImport("user32.dll", EntryPoint = "PostQuitMessage")]
    private static partial void ManualPostQuitMessage(int exitCode);
    [LibraryImport("user32.dll", EntryPoint = "InvalidateRect")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ManualInvalidateRect(nint hwnd, nint rect, [MarshalAs(UnmanagedType.Bool)] bool erase);
    [LibraryImport("user32.dll", EntryPoint = "BeginPaint")]
    private static partial nint ManualBeginPaint(nint hwnd, out ManualPaintStruct paint);
    [LibraryImport("user32.dll", EntryPoint = "EndPaint")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ManualEndPaint(nint hwnd, in ManualPaintStruct paint);
    [LibraryImport("user32.dll", EntryPoint = "GetClientRect")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ManualGetClientRect(nint hwnd, out ManualRect rect);
    [LibraryImport("gdi32.dll", EntryPoint = "CreateSolidBrush")]
    private static partial nint ManualCreateSolidBrush(uint color);
    [LibraryImport("user32.dll", EntryPoint = "FillRect")]
    private static partial int ManualFillRect(nint hdc, in ManualRect rect, nint brush);
    [LibraryImport("gdi32.dll", EntryPoint = "DeleteObject")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ManualDeleteObject(nint value);
    [LibraryImport("gdi32.dll", EntryPoint = "SetBkMode")]
    private static partial int ManualSetBkMode(nint hdc, int mode);
    [LibraryImport("gdi32.dll", EntryPoint = "SetTextColor")]
    private static partial uint ManualSetTextColor(nint hdc, uint color);
    [LibraryImport("user32.dll", EntryPoint = "DrawTextW", StringMarshalling = StringMarshalling.Utf16)]
    private static partial int ManualDrawTextW(nint hdc, string text, int count, ref ManualRect rect, uint format);
    [LibraryImport("user32.dll", EntryPoint = "SetForegroundWindow")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ManualSetForegroundWindow(nint hwnd);
    [LibraryImport("user32.dll", EntryPoint = "SetFocus")]
    private static partial nint ManualSetFocus(nint hwnd);
    [LibraryImport("user32.dll", EntryPoint = "CreatePopupMenu")]
    private static partial nint ManualCreatePopupMenu();
    [LibraryImport("user32.dll", EntryPoint = "AppendMenuW", StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ManualAppendMenuW(nint menu, uint flags, nuint identifier, string text);
    [LibraryImport("user32.dll", EntryPoint = "TrackPopupMenu")]
    private static partial uint ManualTrackPopupMenu(nint menu, uint flags, int x, int y, int reserved, nint hwnd, nint rect);
    [LibraryImport("user32.dll", EntryPoint = "DestroyMenu")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ManualDestroyMenu(nint menu);
    [LibraryImport("user32.dll", EntryPoint = "GetCursorPos")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ManualGetCursorPos(out ManualPoint point);
}
