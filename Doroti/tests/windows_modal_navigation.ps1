<# Actual Windows host/UIA smoke. Input is posted to the owned HWND, not physical input.
   Run through eng/run-with-timeout.py --timeout 1200. #>
param(
    [Parameter(Mandatory = $true)][string] $Output,
    [string] $Executable = 'samples/DorotiTestbedApp/windowsappsdk/bin/Debug/net10.0-windows10.0.19041.0/win-x64/DorotiTestbedApp.WindowsAppSdk.exe'
)
$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$outputPath = [System.IO.Path]::GetFullPath((Join-Path $repoRoot $Output))
$testingRoot = [System.IO.Path]::GetFullPath((Join-Path $repoRoot 'temp/testing')) + [System.IO.Path]::DirectorySeparatorChar
if (!$outputPath.StartsWith($testingRoot, [System.StringComparison]::OrdinalIgnoreCase)) { throw 'Evidence must be under temp/testing.' }
[System.IO.Directory]::CreateDirectory($outputPath) | Out-Null
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type @'
using System;
using System.Runtime.InteropServices;
public static class ModalInput {
    [DllImport("user32.dll")] public static extern bool PostMessage(IntPtr hwnd, uint message, IntPtr wparam, IntPtr lparam);
    [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hwnd);
}
'@
$appPath = [System.IO.Path]::GetFullPath((Join-Path $repoRoot $Executable))
$start = New-Object System.Diagnostics.ProcessStartInfo
$start.FileName = $appPath
$start.UseShellExecute = $false
$start.CreateNoWindow = $true
$start.WorkingDirectory = $repoRoot
$start.EnvironmentVariables['DOROTI_SAMPLE'] = ''
$start.EnvironmentVariables.Remove('DOROTI_DESKTOP_PROBE')
$start.EnvironmentVariables.Remove('DOROTI_INPUT_PROBE')
$start.EnvironmentVariables.Remove('DOROTI_MULTIWINDOW_PROBE')
$checks = @()
$app = [System.Diagnostics.Process]::Start($start)
try {
    for ($attempt = 0; $attempt -lt 30; $attempt++) {
        Start-Sleep -Seconds 1
        $app.Refresh()
        if ($app.HasExited) { throw "Application exited: $($app.ExitCode)" }
        if ($app.MainWindowHandle -ne 0) { break }
    }
    if ($app.MainWindowHandle -eq 0) { throw 'No application window.' }
    Start-Sleep -Seconds 3
    $element = [System.Windows.Automation.AutomationElement]::FromHandle($app.MainWindowHandle)
    $all = $element.FindAll([System.Windows.Automation.TreeScope]::Subtree, [System.Windows.Automation.Condition]::TrueCondition)
    Write-Output "Window: $($app.MainWindowTitle) Responding=$($app.Responding) Elements=$($all.Count)"
    function Find-Name([string] $name) {
        return $element.FindFirst([System.Windows.Automation.TreeScope]::Subtree,
            (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, $name)))
    }
    function Wait-Name([string] $name, [bool] $present) {
        for ($attempt = 0; $attempt -lt 30; $attempt++) {
            $found = Find-Name $name
            if (($null -ne $found) -eq $present) { return }
            Start-Sleep -Milliseconds 200
        }
        throw "Expected '$name' present=$present"
    }
    [ModalInput]::SetForegroundWindow($app.MainWindowHandle) | Out-Null
    foreach ($case in @(@('Show modal bottom sheet', 'Favorite'), @('Show dialog', 'What is a dialog?'), @('Show full-screen dialog', 'Full-screen dialog'))) {
        $button = $null
        for ($attempt = 0; $attempt -lt 30; $attempt++) {
            $button = Find-Name $case[0]
            if ($null -ne $button) { break }
            # The main Components column scrolls independently of the right column.
            $rect = $element.Current.BoundingRectangle
            $position = [int]($rect.Left + 400) -bor ([int]($rect.Top + 500) -shl 16)
            [ModalInput]::PostMessage($app.MainWindowHandle, 0x20A, [IntPtr](-600 -shl 16), [IntPtr]$position) | Out-Null
            Start-Sleep -Milliseconds 300
        }
        if ($null -eq $button) { throw "No button: $($case[0])" }
        $invoke = $button.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern)
        $invoke.Invoke()
        Wait-Name $case[1] $true
        [ModalInput]::PostMessage($app.MainWindowHandle, 0x100, [IntPtr]27, [IntPtr]0x10001) | Out-Null
        [ModalInput]::PostMessage($app.MainWindowHandle, 0x101, [IntPtr]27, [IntPtr]0x10001) | Out-Null
        Wait-Name $case[1] $false
        $app.Refresh()
        if ($app.HasExited -or !$app.Responding) { throw 'Application did not survive modal Escape.' }
        $checks += $case[0]
        Write-Output "PASS: $($case[0]) closes with native Escape; PID=$($app.Id) Responding=$($app.Responding)"
    }
    @{ result = 'PASS'; executable = $Executable; processId = $app.Id; title = $app.MainWindowTitle;
        responding = $app.Responding; modals = $checks; scope = 'actual host; UIA Invoke; posted HWND wheel/Escape; no physical input or IME composition proof' } |
        ConvertTo-Json -Depth 4 | Set-Content (Join-Path $outputPath 'receipt.json') -Encoding UTF8
} finally {
    if (!$app.HasExited) { $app.Kill(); $app.WaitForExit(10000) | Out-Null }
    $app.Dispose()
}
