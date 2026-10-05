using System.Runtime.InteropServices;
using System.Text.Json;
using Doroti.Desktop;
using Doroti.Ui;
namespace DorotiTestbedApp.Desktop;
internal static class NativeMenuProbe
{
    internal static async Task RunAsync(DesktopWindowContext context, string output, CancellationToken token)
    {
        Console.Error.WriteLine("doroti.menu.probe begin");
        var view = context.Window.View ?? throw new InvalidOperationException("Native owner has no attached view.");
        var host = view.RequireCapability<IPlatformMenuHostCapability>(DorotiCapabilityIds.PlatformMenu, DorotiUiInvocation.Managed("NativeMenuProbe"));
        var request = new PlatformMenuRequest(context.Window.Id, [new("first", "첫 항목"), new("separator", "", Separator: true), new("disabled", "Disabled", false), new("nested", "Nested", Children: [new("checked", "Checked", Checked: true)])], Rect.fromLTWH(25, 30, 10, 10), PlatformMenuPresentation.Native);
        host.Evaluate(request).RequireSupported();
        try { host.Evaluate(request with { Window = new WindowId(Guid.NewGuid()) }); throw new InvalidOperationException("Invalid owner was accepted."); }
        catch (InvalidOperationException error) when (error.Message.Contains("requested menu owner")) { }
        if (host.Evaluate(request with { Presentation = PlatformMenuPresentation.Overlay }).Availability != WindowAvailability.Unsupported) throw new InvalidOperationException("Overlay was advertised as native.");
        foreach (var item in new PlatformMenuItem[] { new("shortcut", "Run", Shortcut: new("R")), new("role", "Quit", PlatformRole: "quit") })
            if (host.Evaluate(request with { Items = [item] }).Availability != WindowAvailability.Unsupported)
                throw new InvalidOperationException("Unimplemented popup shortcut or role was advertised as native.");
        async Task<nint> WaitMenuAsync()
        {
            for (var attempt = 0; attempt < 30; attempt++)
            {
                nint menu = 0;
                while ((menu = FindWindowExW(0, menu, "#32768", null)) != 0)
                {
                    GetWindowThreadProcessId(menu, out var process);
                    if (process == Environment.ProcessId) { Console.Error.WriteLine($"doroti.menu.probe observed={menu}"); return menu; }
                }
                await Task.Delay(25, token);
            }
            throw new TimeoutException("The actual OS popup menu was not observed.");
        }
        using var canceled = CancellationTokenSource.CreateLinkedTokenSource(token);
        var cancelObserver = Task.Run(async () => { var menu = await WaitMenuAsync(); Console.Error.WriteLine("doroti.menu.probe cancel"); canceled.Cancel(); Console.Error.WriteLine("doroti.menu.probe canceled callback"); return menu; }, token);
        var cancelObserved = false;
        try
        {
            await view.InvokeCapabilityAsync<IPlatformMenuHostCapability, PlatformMenuResult>(DorotiCapabilityIds.PlatformMenu, DorotiUiInvocation.Managed("native-menu-cancel"), (capability, ct) => capability.ShowAsync(request, ct), canceled.Token);
        }
        catch (OperationCanceledException) when (canceled.IsCancellationRequested) { cancelObserved = true; }
        await cancelObserver; Console.Error.WriteLine("doroti.menu.probe cancellation completed");
        if (!cancelObserved) throw new InvalidOperationException("Cancellation was hidden as menu success.");
        var owner = FindWindowW(null, "Doroti Desktop · Sudoku");
        Console.Error.WriteLine($"doroti.menu.probe owner={owner}");
        if (owner == 0) throw new InvalidOperationException("Native probe owner HWND was not found.");
        var dismissObserver = Task.Run(async () => { await WaitMenuAsync(); if (!PostMessageW(owner, 0x001F, 0, 0)) throw new InvalidOperationException("Cannot cancel native menu mode."); }, token);
        var result = await view.InvokeCapabilityAsync<IPlatformMenuHostCapability, PlatformMenuResult>(DorotiCapabilityIds.PlatformMenu, DorotiUiInvocation.Managed("native-menu-dismiss"), (capability, ct) => capability.ShowAsync(request, ct), token);
        await dismissObserver; Console.Error.WriteLine("doroti.menu.probe dismissal completed");
        if (result.Dismissal != PlatformMenuDismissal.Canceled || result.ItemId is not null) throw new InvalidOperationException("Native dismissal result was incorrect.");
        var bar = view.RequireCapability<IPlatformMenuBarHostCapability>(DorotiCapabilityIds.PlatformMenuBar, DorotiUiInvocation.Managed("NativeMenuBarProbe"));
        var barRequest = new PlatformMenuBarRequest(context.Window.Id, view.viewId, 1,
            [new("root", "File", Children: [new("run", "Run", Shortcut: new("R", Modifiers: PlatformMenuModifiers.Control)), new("separator", "", Separator: true), new("off", "Disabled", false)])]);
        bar.Evaluate(barRequest).RequireSupported();
        if (bar.Evaluate(barRequest with { Items = [new("role", "", PlatformRole: "quit")] }).Availability != WindowAvailability.Unsupported)
            throw new InvalidOperationException("Unsupported OS menu role was advertised.");
        var oldEvents = 0;
        var firstBar = await view.InvokeCapabilityAsync<IPlatformMenuBarHostCapability, IPlatformMenuBarRegistration>(DorotiCapabilityIds.PlatformMenuBar,
            DorotiUiInvocation.Managed("native-menubar-first"), (capability, ct) => capability.SetAsync(barRequest, _ => Interlocked.Increment(ref oldEvents), ct), token);
        if (GetMenu(owner) == 0) throw new InvalidOperationException("The actual HWND menu bar is missing.");
        var selected = new TaskCompletionSource<PlatformMenuEvent>(TaskCreationOptions.RunContinuationsAsynchronously);
        var replacement = barRequest with { Generation = 2 };
        var secondBar = await view.InvokeCapabilityAsync<IPlatformMenuBarHostCapability, IPlatformMenuBarRegistration>(DorotiCapabilityIds.PlatformMenuBar,
            DorotiUiInvocation.Managed("native-menubar-replacement"), (capability, ct) => capability.SetAsync(replacement, value => selected.TrySetResult(value), ct), token);
        await firstBar.DisposeAsync();
        if (GetMenu(owner) == 0 || !PostMessageW(owner, 0x0111, 1, 0)) throw new InvalidOperationException("Replacement menu bar was removed or native command failed.");
        var selection = await selected.Task.WaitAsync(TimeSpan.FromSeconds(5), token);
        if (selection.Window != context.Window.Id || selection.ViewId != view.viewId || selection.Generation != 2 || selection.ItemId != "run" || oldEvents != 0)
            throw new InvalidOperationException("Menu callback identity or closed-generation suppression failed.");
        await secondBar.DisposeAsync();
        if (GetMenu(owner) != 0) throw new InvalidOperationException("Native menu bar was not detached.");
        File.WriteAllText(output, JsonSerializer.Serialize(new { schemaVersion = "doroti.native-menu-probe/v1", result = "PASS", owner = context.Window.Id.Value, viewId = view.viewId.ToString(), actualOsMenuObserved = true, canceledInvocation = true, dismissedResult = result.Dismissal.ToString(), invalidOwnerRejected = true, actualMenuBarObserved = true, nativeCommandGeneration = selection.Generation, staleMenuCallbacks = oldEvents, menuBarDetached = true }));
        await context.Window.CloseAsync(CancellationToken.None);
    }
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern nint FindWindowExW(nint parent, nint after, string name, string? title);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern nint FindWindowW(string? name, string title);
    [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(nint window, out int process);
    [DllImport("user32.dll")] private static extern nint GetMenu(nint window);
    [DllImport("user32.dll")] private static extern bool PostMessageW(nint window, uint message, nuint wparam, nint lparam);
}
