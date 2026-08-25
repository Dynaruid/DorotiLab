using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using Doroti.Host.WindowsAppSdk;
using Doroti.Ui;

namespace Doroti.Validation.WindowsAppSdkFlutterInput;

/// <summary>
/// F7 deliberately exercises the real F2 child WndProc.  It does not create a
/// second input window procedure or a synthetic UIA tree: every native message
/// below reaches the host-owned raw child HWND, then its typed child extension
/// subscribers.  The direct router call is retained solely to prove a wrong
/// HWND is safely left unhandled.
/// </summary>
internal static partial class Program
{
    private const string EvidenceSchema = "doroti.windowsappsdk-flutter-input-evidence/v1";
    private const uint WmSetFocus = 0x0007;
    private const uint WmKillFocus = 0x0008;
    private const uint WmCancelMode = 0x001f;
    private const uint WmSetCursor = 0x0020;
    private const uint WmGetObject = 0x003d;
    private const uint WmMouseMove = 0x0200;
    private const uint WmLeftButtonDown = 0x0201;
    private const uint WmLeftButtonUp = 0x0202;
    private const uint WmMouseWheel = 0x020a;
    private const uint WmKeyDown = 0x0100;
    private const uint WmKeyUp = 0x0101;
    private const uint WmChar = 0x0102;
    private const uint WmDeadChar = 0x0103;
    private const uint WmSysKeyDown = 0x0104;
    private const uint WmSysKeyUp = 0x0105;
    private const uint WmImeStartComposition = 0x010d;
    private const uint WmImeEndComposition = 0x010e;
    private const uint WmImeNotify = 0x0282;
    private const uint HtClient = 1;
    private const uint HtLeft = 10;
    private const uint MkLeftButton = 0x0001;
    private const uint ImnOpenCandidate = 0x0005;
    private const uint VkShift = 0x0010;
    private const uint VkControl = 0x0011;
    private const uint VkMenu = 0x0012;
    private const uint VkReturn = 0x000d;
    private const uint VkBack = 0x0008;
    private const uint VkA = 0x0041;
    private const int UiaRootObjectId = -25;
    private static readonly TimeSpan TestTimeout = TimeSpan.FromMinutes(20);

    private static int Main(string[] args)
    {
        var options = ParseOptions(args);
        try
        {
            if (options.Manual)
                return RunManualAcceptance(options);

            // One complete warm-up includes native child registration and UIA
            // provider allocation before the resource envelope is sampled.
            _ = RunOnDedicatedPlatformStaThread(RunF7Exercise);
            var gdiBefore = GetGuiResources(GuiResourceType.Gdi);
            var userBefore = GetGuiResources(GuiResourceType.User);
            var result = RunOnDedicatedPlatformStaThread(RunF7Exercise);
            var gdiAfter = GetGuiResources(GuiResourceType.Gdi);
            var userAfter = GetGuiResources(GuiResourceType.User);
            var resourcesBounded = gdiAfter <= gdiBefore + 3 && userAfter <= userBefore + 3;
            Assert(resourcesBounded,
                $"F7 GUI resources grew after warmup: GDI {gdiBefore}->{gdiAfter}, USER {userBefore}->{userAfter}.");

            var executablePath = Environment.ProcessPath ?? throw new InvalidOperationException(
                "The F7 validator executable path is unavailable.");
            var executableHash = ComputeFileHash(executablePath);
            if (!string.IsNullOrWhiteSpace(options.PublishedExecutableSha256))
            {
                Assert(string.Equals(executableHash, options.PublishedExecutableSha256,
                        StringComparison.OrdinalIgnoreCase),
                    "The F7 validator executable hash does not match the publish-gate input.");
            }

            var input = result.Input;
            var keyboard = result.Keyboard;
            var text = result.Text;
            var router = result.Router;
            var uia = result.Uia;
            var evidence = new
            {
                schemaVersion = EvidenceSchema,
                runId = Guid.NewGuid().ToString("N"),
                adapter = "FlutterEmbedder",
                runtime = new
                {
                    packageContractVersion = FlutterWindowsAppSdkBootstrap.ExpectedWindowsAppSdkVersion,
                    deployment = "self-contained-unpackaged",
                },
                platformThread = new
                {
                    apartment = "STA",
                    executionMode = "same-sta-thread",
                    managedThreadId = result.PlatformManagedThreadId,
                    nativeThreadId = result.PlatformNativeThreadId,
                },
                automatedScope = new
                {
                    actualChildHwnd = result.Host.ViewHwnd != 0,
                    hostWndProcMessageInjection = result.ActualChildWndProcMessageInjection,
                    typedChildRouter = router.IsAttached && router.ChildHwnd == result.Host.ViewHwnd,
                    uiaChildWmGetObject = uia.WmGetObjectRequestCount > 0 && uia.AttachedToChildHost,
                },
                child = new
                {
                    primaryInputRoot = "raw-child-hwnd",
                    topLevelHwnd = result.Host.TopLevelHwnd.ToInt64(),
                    viewHwnd = result.Host.ViewHwnd.ToInt64(),
                    singleTopLevelAndChild = result.Host.TopLevelHwnd != 0 && result.Host.ViewHwnd != 0 &&
                        result.Host.RawWindowAssociationCount == 1 && result.Host.AppWindowAssociated,
                    topLevelNonClientDefWindowProcPreserved = input.TopLevelNonClientCursorHandledCount == 0,
                },
                router = new
                {
                    receivedMessageCount = router.ReceivedMessageCount,
                    handledMessageCount = router.HandledMessageCount,
                    unhandledMessageCount = router.UnhandledMessageCount,
                    mismatchedHwndMessageCount = router.MismatchedHwndMessageCount,
                },
                pointer = new
                {
                    addCount = input.PointerAddCount,
                    hoverCount = input.PointerHoverCount,
                    downCount = input.PointerDownCount,
                    moveCount = input.PointerMoveCount,
                    upCount = input.PointerUpCount,
                    removeCount = input.PointerRemoveCount,
                    cancelCount = input.PointerCancelCount,
                    wheelCount = input.WheelCount,
                    touchRegistered = input.TouchRegistered,
                    touchRegistrationCount = input.TouchRegistrationCount,
                    touchUnregistrationCount = result.DisposedInput.TouchUnregistrationCount,
                    eventOrderValid = result.OutsideUp.OrderValid && result.Cancel.OrderValid,
                    sequences = new[]
                    {
                        ToPointerSequenceEvidence(result.OutsideUp),
                        ToPointerSequenceEvidence(result.Cancel),
                    },
                },
                capture = new
                {
                    acquireCount = input.MouseCaptureAcquireCount,
                    releaseCount = input.MouseCaptureReleaseCount,
                    outsideBoundsUpCount = input.OutsideBoundsUpCount,
                    outsideChildUpHandled = result.OutsideUp.CaptureHeldOutsideChild,
                    cancelHandled = result.Cancel.CaptureReleased,
                    releasedAfterTerminal = !input.HasMouseCapture && result.OutsideUp.CaptureReleased &&
                        result.Cancel.CaptureReleased,
                },
                focus = new
                {
                    gainedCount = input.FocusGainedCount,
                    lostCount = input.FocusLostCount,
                    childFocusOwner = input.ChildHwnd == result.Host.ViewHwnd,
                    automatedFocusTransitions = input.FocusGainedCount >= 1 && input.FocusLostCount >= 1,
                },
                cursor = new
                {
                    clientHandledCount = input.ClientCursorHandledCount,
                    topLevelNonClientHandledCount = input.TopLevelNonClientCursorHandledCount,
                    clientCursorHandled = result.ClientCursorHandled,
                    nonClientDelegatedToDefWindowProc = result.NonClientCursorDelegated &&
                        input.TopLevelNonClientCursorHandledCount == 0,
                },
                keyboard = new
                {
                    keyDownCount = keyboard.KeyDownCount,
                    keyUpCount = keyboard.KeyUpCount,
                    deadKeyCount = keyboard.DeadKeyCount,
                    surrogatePairCount = keyboard.SurrogatePairCount,
                    modifierMaskAfterSequence = keyboard.ModifierMask,
                    unhandledSystemMessageCount = keyboard.UnhandledSystemMessageCount,
                    suppressedShortcutCharacterCount = keyboard.SuppressedShortcutCharacterCount,
                    suppressedActionCharacterCount = keyboard.SuppressedActionCharacterCount,
                    suppressedControlCharacterCount = keyboard.SuppressedControlCharacterCount,
                    singleManager = result.KeyMessagesObserved == result.KeyDataCount,
                    deadKeySerialized = keyboard.DeadKeyCount >= 1 && !keyboard.HasPendingDeadKey,
                    surrogatePairSerialized = keyboard.SurrogatePairCount >= 1 && !keyboard.HasPendingHighSurrogate,
                    systemKeyDelegated = result.SystemKeyDelegated && keyboard.UnhandledSystemMessageCount >= 1,
                    shortcutControlCharactersSuppressed = result.ShortcutControlCharactersSuppressed &&
                        keyboard.SuppressedShortcutCharacterCount >= 1,
                    actionCharactersSuppressed = result.ActionCharactersSuppressed &&
                        keyboard.SuppressedActionCharacterCount >= 1 && !keyboard.SuppressNextReturnCharacter,
                    nonTextControlCharactersSuppressed = result.NonTextControlCharactersSuppressed &&
                        keyboard.SuppressedControlCharacterCount >= 1,
                },
                ime = new
                {
                    owner = "IMM32",
                    childTextOwner = text.IsImm32Owner && text.HasClient && text.ChildHwnd == result.Host.ViewHwnd,
                    clientSetCount = text.ClientSetCount,
                    compositionStartCount = text.CompositionStartCount,
                    compositionEndCount = text.CompositionEndCount,
                    candidatePositionUpdateCount = text.CandidatePositionUpdateCount,
                    caretPositionUpdateCount = text.CaretPositionUpdateCount,
                    immContextAcquireCount = text.ImmContextAcquireCount,
                    immContextReleaseCount = text.ImmContextReleaseCount,
                    candidateAndCaretBoundsCommitted = text.CommittedCaretClientRect is not null &&
                        text.CommittedCaretScreenRect is not null && text.CandidatePositionUpdateCount >= 1 &&
                        text.CaretPositionUpdateCount >= 1,
                },
                clipboard = new
                {
                    readCount = input.ClipboardReadCount,
                    writeCount = input.ClipboardWriteCount,
                    automatedRoundTrip = result.ClipboardRoundTrip,
                },
                uia = new
                {
                    actionDispatch = "engine-task-runner",
                    wmGetObjectRequestCount = uia.WmGetObjectRequestCount,
                    rootProviderReturnCount = uia.UiaRootProviderReturnCount,
                    rootProviderCreationCount = uia.RootProviderCreationCount,
                    duplicateRootProviderCount = uia.DuplicateRootProviderCount,
                    fragmentProviderCount = uia.FragmentProviderCount,
                    brokenFragmentCount = uia.BrokenFragmentCount,
                    queuedActionCount = uia.QueuedActionCount,
                    rejectedActionCount = uia.RejectedActionCount,
                    engineTaskRunCount = result.EngineTaskRunCount,
                    dispatchedInvokeCount = result.DispatchedInvokeCount,
                    dispatchedValueCount = result.DispatchedValueCount,
                    dispatchedScrollCount = result.DispatchedScrollCount,
                    childRootProviderReturned = uia.WmGetObjectRequestCount > 0 &&
                        uia.UiaRootProviderReturnCount > 0 && uia.AttachedToChildHost,
                    immutableSemanticsSnapshot = result.ImmutableSemanticsSnapshot,
                    invokeValueScrollQueued = result.QueuedUiaActions,
                    actionsRunOnEngineTaskRunner = result.EngineTaskRunCount == 3 &&
                        result.DispatchedInvokeCount == 1 && result.DispatchedValueCount == 1 &&
                        result.DispatchedScrollCount == 1,
                },
                resources = new
                {
                    gdiBefore,
                    gdiAfter,
                    userBefore,
                    userAfter,
                    boundedAfterWarmup = resourcesBounded,
                },
                physicalAcceptance = new
                {
                    realUserInteractionVerified = false,
                    koreanImeCandidateVerified = false,
                    koreanImeCaretSelectionClipboardVerified = false,
                    narratorVerified = false,
                    accessibilityInsightsVerified = false,
                    visibleProductAcceptanceVerified = false,
                },
                startup = new
                {
                    mauiOrXamlAssemblyCount = result.MauiOrXamlAssemblyNames.Length,
                    mauiOrXamlAssemblyNames = result.MauiOrXamlAssemblyNames,
                    noMauiOrXamlWindowStartup = result.MauiOrXamlAssemblyNames.Length == 0,
                },
                validation = new
                {
                    sourceFingerprint = options.SourceFingerprint ?? "unbound-local-run",
                    executablePath,
                    executableSha256 = executableHash,
                },
                scopeBoundary = "F7 proves real raw-child-HWND WndProc message injection and the typed input, IMM32, clipboard, and child-root UIA structural path. It does not prove physical Korean IME candidate UI, Narrator or Accessibility Insights inspection, real-user interaction, or visible product acceptance.",
                notVerified = new[]
                {
                    "real-user mouse, touch, pen, wheel, keyboard, focus, capture, Alt+Tab, minimize/restore, and popup interaction acceptance",
                    "Korean two-beolsik IME composition, candidate-window, caret, selection, and clipboard physical acceptance",
                    "Narrator and Accessibility Insights physical inspection of the UIA tree",
                    "auxiliary DesktopChildSiteBridge behavior (no auxiliary island is created by this fixture)",
                    "visible Windows product acceptance (FG), F8 lifecycle/recovery, and F9 product-runner selection",
                },
            };
            var json = JsonSerializer.Serialize(evidence, new JsonSerializerOptions { WriteIndented = true });
            if (!string.IsNullOrWhiteSpace(options.EvidencePath))
            {
                var evidencePath = System.IO.Path.GetFullPath(options.EvidencePath);
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(evidencePath) ?? throw new InvalidOperationException(
                    "The F7 evidence path has no parent directory."));
                File.WriteAllText(evidencePath, json + Environment.NewLine);
            }
            Console.WriteLine(json);
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"windowsappsdk-flutter-input FAIL: {exception}");
            return 1;
        }
    }

    private static F7ExerciseResult RunF7Exercise()
    {
        var bootstrap = new FlutterWindowsAppSdkBootstrap();
        FlutterWindowsHostWindow? host = null;
        FlutterWindowsViewMetricsCoordinator? metrics = null;
        FlutterWindowsInputHost? input = null;
        FlutterWindowsUiaBridge? uia = null;
        string? previousClipboard = null;
        var clipboardChanged = false;
        try
        {
            host = FlutterWindowsHostWindow.CreateOnCurrentThread(
                bootstrap,
                new FlutterWindowsHostWindowOptions(
                    "Doroti F7 child input validation",
                    InitialClientWidth: 640,
                    InitialClientHeight: 360,
                    MinimumClientWidth: 320,
                    MinimumClientHeight: 240,
                    MaximumClientWidth: 960,
                    MaximumClientHeight: 720,
                    InitialX: -32000,
                    InitialY: -32000));
            metrics = FlutterWindowsViewMetricsCoordinator.AttachToHostWindow(
                host,
                viewId: 70_001,
                new FlutterWindowsPhysicalConstraints(320, 240, 960, 720));
            var engineTaskRunner = new QueueEngineTaskRunner();
            var dispatchedActions = new List<SemanticsAction>();
            input = new FlutterWindowsInputHost(host, 70_001, () => metrics.Current);
            var pointerEvents = new List<string>();
            var keyDataCount = 0;
            var keyMessagesObserved = 0;
            var textActionCount = 0;
            input.PointerData += packet => pointerEvents.AddRange(packet.data.Select(pointer => pointer.change.ToString()));
            input.KeyData += _ => keyDataCount++;
            input.ActionPerformed += _ => textActionCount++;
            input.SetClient(
                new DorotiTextInputConfiguration(
                    DorotiTextInputType.text,
                    DorotiTextInputAction.done,
                    DorotiTextCapitalization.none,
                    readOnly: false,
                    obscureText: false,
                    autocorrect: true,
                    enableSuggestions: true),
                new DorotiTextEditingState("seed", new DorotiTextSelection(4, 4), null));
            input.SetCaretRect(new Rect(8, 12, 10, 30));

            uia = FlutterWindowsUiaBridge.AttachToHostWindow(
                host,
                metrics,
                engineTaskRunner,
                (nodeId, action, arguments) =>
                {
                    _ = nodeId;
                    _ = arguments;
                    dispatchedActions.Add(action);
                },
                () => input.RequestFocus(ViewFocusState.focused, ViewFocusDirection.undefined));
            var submittedSemantics = CreateSemantics(out var submittedRootChildren);
            Assert(uia.UpdateSemantics(submittedSemantics), "F7 did not retain its initial immutable semantics snapshot.");
            var immutableSnapshot = uia.SemanticsSnapshot;
            submittedRootChildren[0] = 99;
            var immutableSemanticsSnapshot = immutableSnapshot.Nodes.TryGetValue(0, out var retainedRoot) &&
                retainedRoot.children.SequenceEqual(new[] { 1, 2, 3 }) &&
                SnapshotDictionaryRejectsMutation(immutableSnapshot.Nodes);
            Assert(immutableSnapshot.Nodes.Count == 4 && immutableSnapshot.RootNodeIds.SequenceEqual(new[] { 0 }) &&
                uia.GetFragment(1) is not null && uia.GetFragment(2) is not null && uia.GetFragment(3) is not null,
                "F7 child UIA fragments were not retained as one root plus three reachable semantic nodes.");
            Assert(immutableSemanticsSnapshot,
                "F7 retained semantics snapshot was mutable or shared the framework input collection.");

            var child = host.ViewHwnd;
            var topLevel = host.TopLevelHwnd;
            var receivedBeforeNativeInjection = input.Router.Snapshot.ReceivedMessageCount;

            // The first move supplies the required explicit mouse add/hover prior to down.
            pointerEvents.Clear();
            _ = SendMessageW(child, WmMouseMove, 0, PackPoint(12, 14));
            _ = SendMessageW(child, WmLeftButtonDown, MkLeftButton, PackPoint(12, 14));
            Assert(GetCapture() == child, "F7 did not acquire raw child mouse capture on mouse down.");
            _ = SendMessageW(child, WmMouseMove, MkLeftButton, PackPoint(4095, 4095));
            var captureHeldOutside = GetCapture() == child;
            _ = SendMessageW(child, WmLeftButtonUp, 0, PackPoint(4095, 4095));
            var captureReleasedOutside = GetCapture() != child;
            var outsideUp = new PointerSequence(
                "outside-up",
                pointerEvents.ToArray(),
                Matches(pointerEvents, "add", "hover", "down", "move", "up", "remove"),
                captureHeldOutside,
                captureReleasedOutside);

            pointerEvents.Clear();
            _ = SendMessageW(child, WmLeftButtonDown, MkLeftButton, PackPoint(16, 18));
            Assert(GetCapture() == child, "F7 did not acquire raw child mouse capture before cancellation.");
            _ = SendMessageW(child, WmCancelMode, 0, 0);
            var cancelReleased = GetCapture() != child;
            var cancel = new PointerSequence(
                "cancel",
                pointerEvents.ToArray(),
                Matches(pointerEvents, "add", "down", "cancel", "remove"),
                CaptureHeldOutsideChild: true,
                CaptureReleased: cancelReleased);

            _ = SendMessageW(child, WmMouseWheel, MakeWheelWParam(120), PackPoint(32, 32));
            _ = SendMessageW(child, WmKillFocus, 0, 0);
            _ = SendMessageW(child, WmSetFocus, 0, 0);
            input.SetCursor(DorotiMouseCursorKind.text);
            var clientCursorHandled = SendMessageW(child, WmSetCursor, unchecked((nuint)child), PackSetCursor(HtClient)) == 1;
            _ = SendMessageW(topLevel, WmSetCursor, unchecked((nuint)topLevel), PackSetCursor(HtLeft));

            SendKey(child, WmKeyDown, VkShift, ref keyMessagesObserved);
            SendKey(child, WmKeyDown, VkA, ref keyMessagesObserved);
            SendKey(child, WmKeyUp, VkA, ref keyMessagesObserved);
            SendKey(child, WmKeyUp, VkShift, ref keyMessagesObserved);
            _ = SendMessageW(child, WmDeadChar, '^', 0);
            _ = SendMessageW(child, WmChar, 0xd83d, 0);
            _ = SendMessageW(child, WmChar, 0xde03, 0);
            var systemDown = input.Router.Route(new FlutterWindowsChildMessage(child, WmSysKeyDown, VkMenu, 0, DorotiFrameClock.Now));
            var systemUp = input.Router.Route(new FlutterWindowsChildMessage(child, WmSysKeyUp, VkMenu, 0, DorotiFrameClock.Now));
            keyMessagesObserved += 2;
            var systemKeyDelegated = !systemDown.Handled && !systemUp.Handled;

            var stateBeforeControlShortcut = input.TextInputSnapshot.EditingState;
            SendKey(child, WmKeyDown, VkControl, ref keyMessagesObserved);
            SendKey(child, WmKeyDown, VkA, ref keyMessagesObserved);
            _ = SendMessageW(child, WmChar, 0x0001, 0); // Ctrl+A C0 character.
            SendKey(child, WmKeyUp, VkA, ref keyMessagesObserved);
            SendKey(child, WmKeyUp, VkControl, ref keyMessagesObserved);
            var shortcutControlCharactersSuppressed =
                input.TextInputSnapshot.EditingState == stateBeforeControlShortcut;
            Assert(shortcutControlCharactersSuppressed,
                "F7 committed a Ctrl+A WM_CHAR control character into the editing state.");

            var stateBeforeAction = input.TextInputSnapshot.EditingState;
            SendKey(child, WmKeyDown, VkReturn, ref keyMessagesObserved);
            _ = SendMessageW(child, WmChar, '\r', 0);
            SendKey(child, WmKeyUp, VkReturn, ref keyMessagesObserved);
            var actionCharactersSuppressed = textActionCount == 1 &&
                input.TextInputSnapshot.EditingState == stateBeforeAction;
            Assert(actionCharactersSuppressed,
                "F7 duplicated a handled Enter action as literal editing-state text.");

            var stateBeforeBackspace = input.TextInputSnapshot.EditingState;
            SendKey(child, WmKeyDown, VkBack, ref keyMessagesObserved);
            _ = SendMessageW(child, WmChar, '\b', 0);
            SendKey(child, WmKeyUp, VkBack, ref keyMessagesObserved);
            var nonTextControlCharactersSuppressed =
                input.TextInputSnapshot.EditingState == stateBeforeBackspace;
            Assert(nonTextControlCharactersSuppressed,
                "F7 committed a Backspace WM_CHAR control character into the editing state.");

            _ = SendMessageW(child, WmImeStartComposition, 0, 0);
            _ = SendMessageW(child, WmImeNotify, ImnOpenCandidate, 0);
            _ = SendMessageW(child, WmImeEndComposition, 0, 0);

            previousClipboard = input.GetClipboardTextAsync().AsTask().GetAwaiter().GetResult();
            var clipboardMarker = "doroti-f7-" + Guid.NewGuid().ToString("N");
            input.SetClipboardTextAsync(clipboardMarker).AsTask().GetAwaiter().GetResult();
            clipboardChanged = true;
            var clipboardRoundTrip = string.Equals(
                input.GetClipboardTextAsync().AsTask().GetAwaiter().GetResult(),
                clipboardMarker,
                StringComparison.Ordinal);
            Assert(clipboardRoundTrip, "F7 child clipboard did not round-trip its automation marker.");
            input.SetClipboardTextAsync(previousClipboard ?? string.Empty).AsTask().GetAwaiter().GetResult();
            clipboardChanged = false;

            _ = SendMessageW(child, WmGetObject, 0, new nint(UiaRootObjectId));
            Assert(uia.TryDispatchActionForValidation(1, SemanticsAction.tap),
                "F7 UIA Invoke action was not admitted to the engine queue.");
            Assert(uia.TryDispatchActionForValidation(2, SemanticsAction.setText, "updated"),
                "F7 UIA Value action was not admitted to the engine queue.");
            Assert(uia.TryDispatchActionForValidation(3, SemanticsAction.scrollDown),
                "F7 UIA Scroll action was not admitted to the engine queue.");
            Assert(engineTaskRunner.PendingCount == 3, "F7 UIA actions did not stay queued before engine task execution.");
            while (engineTaskRunner.TryRunOneTask()) { }
            Assert(dispatchedActions.Count == 3 && dispatchedActions.Count(action => action == SemanticsAction.tap) == 1 &&
                dispatchedActions.Count(action => action == SemanticsAction.setText) == 1 &&
                dispatchedActions.Count(action => action == SemanticsAction.scrollDown) == 1,
                "F7 UIA queued actions did not execute exactly once on the engine task runner.");

            var mismatch = input.Router.Route(new FlutterWindowsChildMessage(
                new nint(child.ToInt64() + 1), WmMouseMove, 0, PackPoint(1, 1), DorotiFrameClock.Now));
            Assert(!mismatch.Handled, "F7 child router handled a mismatched HWND.");

            var inputSnapshot = input.Snapshot;
            var keyboardSnapshot = input.KeyboardSnapshot;
            var textSnapshot = input.TextInputSnapshot;
            var routerSnapshot = input.Router.Snapshot;
            var uiaSnapshot = uia.Snapshot;
            Assert(inputSnapshot.PointerAddCount >= 2 && inputSnapshot.PointerDownCount >= 2 &&
                inputSnapshot.PointerRemoveCount >= 2 && inputSnapshot.PointerCancelCount >= 1 &&
                inputSnapshot.WheelCount >= 1 && inputSnapshot.MouseCaptureAcquireCount >= 2 &&
                inputSnapshot.MouseCaptureReleaseCount >= 2 && inputSnapshot.OutsideBoundsUpCount >= 1 &&
                keyboardSnapshot.DeadKeyCount >= 1 && keyboardSnapshot.SurrogatePairCount >= 1 &&
                textSnapshot.CompositionStartCount >= 1 && textSnapshot.CompositionEndCount >= 1 &&
                uiaSnapshot.WmGetObjectRequestCount >= 1 && uiaSnapshot.UiaRootProviderReturnCount >= 1 &&
                uiaSnapshot.FragmentProviderCount >= 3 && uiaSnapshot.DuplicateRootProviderCount == 0 &&
                uiaSnapshot.BrokenFragmentCount == 0 && uiaSnapshot.RejectedActionCount == 0,
                "F7 child input/UIA snapshots violate the structural contract.");
            Assert(input.Router.Snapshot.ReceivedMessageCount > receivedBeforeNativeInjection,
                "F7 native SendMessage injection did not reach the host-attached typed child router.");

            var hostSnapshot = host.Snapshot;
            uia.Dispose();
            uia = null;
            input.Dispose();
            var disposedInput = input.Snapshot;
            input = null;
            metrics.Dispose();
            metrics = null;
            host.Dispose();
            host = null;
            return new F7ExerciseResult(
                hostSnapshot,
                inputSnapshot,
                disposedInput,
                keyboardSnapshot,
                textSnapshot,
                routerSnapshot,
                uiaSnapshot,
                outsideUp,
                cancel,
                ActualChildWndProcMessageInjection: true,
                ClientCursorHandled: clientCursorHandled,
                NonClientCursorDelegated: inputSnapshot.TopLevelNonClientCursorHandledCount == 0,
                SystemKeyDelegated: systemKeyDelegated,
                ShortcutControlCharactersSuppressed: shortcutControlCharactersSuppressed,
                ActionCharactersSuppressed: actionCharactersSuppressed,
                NonTextControlCharactersSuppressed: nonTextControlCharactersSuppressed,
                ClipboardRoundTrip: clipboardRoundTrip,
                ImmutableSemanticsSnapshot: immutableSemanticsSnapshot,
                QueuedUiaActions: true,
                EngineTaskRunCount: engineTaskRunner.RunCount,
                DispatchedInvokeCount: dispatchedActions.Count(action => action == SemanticsAction.tap),
                DispatchedValueCount: dispatchedActions.Count(action => action == SemanticsAction.setText),
                DispatchedScrollCount: dispatchedActions.Count(action => action == SemanticsAction.scrollDown),
                KeyDataCount: keyDataCount,
                KeyMessagesObserved: keyMessagesObserved,
                PlatformManagedThreadId: Environment.CurrentManagedThreadId,
                PlatformNativeThreadId: GetCurrentThreadId(),
                MauiOrXamlAssemblyNames: CaptureMauiOrXamlAssemblyNames());
        }
        finally
        {
            if (clipboardChanged && input is not null)
            {
                try { input.SetClipboardTextAsync(previousClipboard ?? string.Empty).AsTask().GetAwaiter().GetResult(); }
                catch { /* Preserve the original exercise error; normal completion restores the clipboard above. */ }
            }
            uia?.Dispose();
            input?.Dispose();
            metrics?.Dispose();
            if (host is not null) host.Dispose();
            else if (bootstrap.Snapshot.Phase != FlutterWindowsAppSdkBootstrapPhase.Disposed)
                bootstrap.DisposeOnCurrentThread();
        }
    }

    private static SemanticsUpdate CreateSemantics(out int[] rootChildren)
    {
        rootChildren = [1, 2, 3];
        return new SemanticsUpdate(
            generation: 1,
            nodes:
            [
            new SemanticsNodeUpdate(
                0, new Rect(0, 0, 640, 360), "Doroti root", null,
                SemanticsAction.none, rootChildren, new SemanticsFlags(isEnabled: Tristate.isTrue)),
            new SemanticsNodeUpdate(
                1, new Rect(8, 8, 120, 52), "Invoke", null,
                SemanticsAction.tap, [], new SemanticsFlags(isEnabled: Tristate.isTrue, isButton: true)),
            new SemanticsNodeUpdate(
                2, new Rect(8, 64, 220, 108), "Value", "seed",
                SemanticsAction.setText, [], new SemanticsFlags(isEnabled: Tristate.isTrue, isTextField: true)),
            new SemanticsNodeUpdate(
                3, new Rect(8, 120, 420, 320), "Scroll", null,
                SemanticsAction.scrollDown | SemanticsAction.scrollUp | SemanticsAction.scrollToOffset,
                [], new SemanticsFlags(isEnabled: Tristate.isTrue, hasImplicitScrolling: true),
                scrollPosition: 40, scrollExtentMax: 100, scrollExtentMin: 0),
            ]);
    }

    private static bool SnapshotDictionaryRejectsMutation(
        IReadOnlyDictionary<int, SemanticsNodeUpdate> snapshot)
    {
        if (snapshot is not ICollection<KeyValuePair<int, SemanticsNodeUpdate>> collection) return false;
        try
        {
            collection.Add(new KeyValuePair<int, SemanticsNodeUpdate>(
                -1,
                new SemanticsNodeUpdate(-1, new Rect(0, 0, 1, 1), null, null,
                    SemanticsAction.none, [])));
            return false;
        }
        catch (NotSupportedException)
        {
            return true;
        }
    }

    private static object ToPointerSequenceEvidence(PointerSequence sequence) => new
    {
        name = sequence.Name,
        events = sequence.Events,
        orderValid = sequence.OrderValid,
        captureHeldOutsideChild = sequence.CaptureHeldOutsideChild,
        captureReleased = sequence.CaptureReleased,
    };

    private static bool Matches(IReadOnlyList<string> actual, params string[] expected) =>
        actual.Count == expected.Length && actual.SequenceEqual(expected, StringComparer.Ordinal);

    private static void SendKey(nint child, uint message, uint key, ref int observed)
    {
        _ = SendMessageW(child, message, key, new nint((long)key << 16));
        observed++;
    }

    private static nint PackPoint(short x, short y) => new(unchecked((ushort)x | ((long)(ushort)y << 16)));

    private static nint PackSetCursor(uint hitTest) => new(hitTest);

    private static nuint MakeWheelWParam(short delta) => unchecked((nuint)((ushort)delta << 16));

    private static T RunOnDedicatedPlatformStaThread<T>(Func<T> action)
    {
        T? result = default;
        Exception? failure = null;
        using var completed = new ManualResetEventSlim();
        var thread = new Thread(() =>
        {
            try { result = action(); }
            catch (Exception exception) { failure = exception; }
            finally { completed.Set(); }
        })
        {
            IsBackground = true,
            Name = "Doroti F7 platform STA validation",
        };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        if (!completed.Wait(TestTimeout) || !thread.Join(TestTimeout))
            throw new TimeoutException("F7 platform validation did not terminate within 20 minutes.");
        if (failure is not null)
            throw new InvalidOperationException("F7 platform validation failed.", failure);
        return result ?? throw new InvalidOperationException("F7 validation returned no result.");
    }

    private static string[] CaptureMauiOrXamlAssemblyNames() =>
        AppDomain.CurrentDomain.GetAssemblies()
            .Select(assembly => assembly.GetName().Name ?? assembly.FullName ?? "unknown")
            .Where(name => name.StartsWith(string.Concat("Microsoft.", "Maui"), StringComparison.OrdinalIgnoreCase) ||
                           name.StartsWith(string.Concat("Microsoft.UI.", "Xaml"), StringComparison.OrdinalIgnoreCase))
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToArray();

    private static int GetGuiResources(GuiResourceType type) =>
        GetGuiResourcesNative(GetCurrentProcess(), (uint)type);

    private static F7Options ParseOptions(string[] args)
    {
        string? evidencePath = null;
        string? sourceFingerprint = null;
        string? publishedExecutableSha256 = null;
        var manual = false;
        for (var index = 0; index < args.Length; index++)
        {
            switch (args[index])
            {
                case "--manual": manual = true; break;
                case "--evidence": evidencePath = RequireOptionValue(args, ref index); break;
                case "--source-fingerprint": sourceFingerprint = RequireOptionValue(args, ref index); break;
                case "--published-executable-sha256": publishedExecutableSha256 = RequireOptionValue(args, ref index); break;
                default:
                    throw new ArgumentException(
                        "Usage: Doroti.Validation.WindowsAppSdkFlutterInput " +
                        "[--manual] " +
                        "[--evidence <path>] [--source-fingerprint <sha256>] " +
                        "[--published-executable-sha256 <sha256>]");
            }
        }
        return new F7Options(manual, evidencePath, sourceFingerprint, publishedExecutableSha256);
    }

    private static string RequireOptionValue(string[] args, ref int index)
    {
        if (++index >= args.Length)
            throw new ArgumentException("The preceding F7 validator option requires a value.");
        return args[index];
    }

    private static string ComputeFileHash(string path) =>
        Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant();

    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    [LibraryImport("user32.dll", EntryPoint = "SendMessageW")]
    private static partial nint SendMessageW(nint hwnd, uint message, nuint wParam, nint lParam);

    [LibraryImport("user32.dll")]
    private static partial nint GetCapture();

    [LibraryImport("kernel32.dll")]
    private static partial uint GetCurrentThreadId();

    [LibraryImport("kernel32.dll")]
    private static partial nint GetCurrentProcess();

    [LibraryImport("user32.dll", EntryPoint = "GetGuiResources")]
    private static partial int GetGuiResourcesNative(nint process, uint flags);

    private enum GuiResourceType : uint
    {
        Gdi = 0,
        User = 1,
    }

    private sealed class QueueEngineTaskRunner : IFlutterWindowsEngineTaskRunner
    {
        private readonly Queue<Action> _tasks = new();

        internal int PendingCount
        {
            get
            {
                lock (_tasks) return _tasks.Count;
            }
        }

        internal int RunCount { get; private set; }

        public void PostEngineTask(Action action)
        {
            ArgumentNullException.ThrowIfNull(action);
            lock (_tasks) _tasks.Enqueue(action);
        }

        public bool TryRunOneTask()
        {
            Action? task;
            lock (_tasks)
            {
                task = _tasks.Count == 0 ? null : _tasks.Dequeue();
            }
            if (task is null) return false;
            task();
            RunCount++;
            return true;
        }
    }

    private sealed record F7Options(
        bool Manual,
        string? EvidencePath,
        string? SourceFingerprint,
        string? PublishedExecutableSha256);

    private sealed record PointerSequence(
        string Name,
        string[] Events,
        bool OrderValid,
        bool CaptureHeldOutsideChild,
        bool CaptureReleased);

    private sealed record F7ExerciseResult(
        FlutterWindowsHostWindowSnapshot Host,
        FlutterWindowsInputHostSnapshot Input,
        FlutterWindowsInputHostSnapshot DisposedInput,
        FlutterWindowsKeyboardManagerSnapshot Keyboard,
        FlutterWindowsImm32TextInputManagerSnapshot Text,
        FlutterWindowsChildMessageRouterSnapshot Router,
        FlutterWindowsUiaBridgeSnapshot Uia,
        PointerSequence OutsideUp,
        PointerSequence Cancel,
        bool ActualChildWndProcMessageInjection,
        bool ClientCursorHandled,
        bool NonClientCursorDelegated,
        bool SystemKeyDelegated,
        bool ShortcutControlCharactersSuppressed,
        bool ActionCharactersSuppressed,
        bool NonTextControlCharactersSuppressed,
        bool ClipboardRoundTrip,
        bool ImmutableSemanticsSnapshot,
        bool QueuedUiaActions,
        int EngineTaskRunCount,
        int DispatchedInvokeCount,
        int DispatchedValueCount,
        int DispatchedScrollCount,
        int KeyDataCount,
        int KeyMessagesObserved,
        int PlatformManagedThreadId,
        uint PlatformNativeThreadId,
        string[] MauiOrXamlAssemblyNames);
}
