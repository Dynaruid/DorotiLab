#Requires -Version 5.1
param(
    [Parameter(Mandatory = $true)][int] $ProcessId,
    [Parameter(Mandatory = $true)][long] $WindowHandle,
    [Parameter(Mandatory = $true)][string] $OutputPath,
    [ValidateRange(1, 5)][int] $ActionLimit = 5
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName UIAutomationClient
Add-Type @'
using System;
using System.Runtime.InteropServices;
public static class G64NativeWindowLookup
{
    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr window, out uint processId);
}
'@

$window = $null
$deadline = [DateTime]::UtcNow.AddSeconds(10)
while ($null -eq $window -and [DateTime]::UtcNow -lt $deadline) {
    $handle = [IntPtr]::new($WindowHandle)
    if ($handle -ne [IntPtr]::Zero) {
        $window = [System.Windows.Automation.AutomationElement]::FromHandle($handle)
    }
    if ($null -eq $window) { Start-Sleep -Milliseconds 50 }
}
if ($null -eq $window) { throw "UI Automation could not find the G6-4 window for process $ProcessId." }

function Find-NamedNode {
    param([string] $Name)
    $node = $window.FindAll(
        [System.Windows.Automation.TreeScope]::Descendants,
        [System.Windows.Automation.Condition]::TrueCondition) |
        Where-Object { $_.Current.Name.StartsWith($Name, [StringComparison]::Ordinal) } |
        Select-Object -First 1
    if ($null -eq $node) { throw "UI Automation could not find '$Name'." }
    return $node
}

$button = Find-NamedNode 'G6 A0 invoke'
$button.SetFocus()
if ($ActionLimit -ge 2) {
    Start-Sleep -Milliseconds 150
    $button = Find-NamedNode 'G6 A0 invoke'
    ([System.Windows.Automation.InvokePattern]$button.GetCurrentPattern(
        [System.Windows.Automation.InvokePattern]::Pattern)).Invoke()
}
if ($ActionLimit -ge 3) {
    Start-Sleep -Milliseconds 150
    $toggle = Find-NamedNode 'G6 A0 toggle'
    ([System.Windows.Automation.TogglePattern]$toggle.GetCurrentPattern(
        [System.Windows.Automation.TogglePattern]::Pattern)).Toggle()
}
if ($ActionLimit -ge 4) {
    Start-Sleep -Milliseconds 150
    $text = Find-NamedNode 'G6 A0 text'
    ([System.Windows.Automation.ValuePattern]$text.GetCurrentPattern(
        [System.Windows.Automation.ValuePattern]::Pattern)).SetValue('external-uia-text')
}
if ($ActionLimit -ge 5) {
    Start-Sleep -Milliseconds 150
    $scroll = Find-NamedNode 'G6 A0 scroll'
    ([System.Windows.Automation.ScrollPattern]$scroll.GetCurrentPattern(
        [System.Windows.Automation.ScrollPattern]::Pattern)).Scroll(
            [System.Windows.Automation.ScrollAmount]::NoAmount,
            [System.Windows.Automation.ScrollAmount]::SmallIncrement)
}

$allActions = @('focus:G6 A0 invoke', 'invoke:G6 A0 invoke', 'toggle:G6 A0 toggle', 'setText:G6 A0 text', 'scroll:G6 A0 scroll')
$actions = $allActions[0..($ActionLimit - 1)]
$fullOutput = [IO.Path]::GetFullPath($OutputPath)
[IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($fullOutput)) | Out-Null
$temporaryOutput = "$fullOutput.tmp-$([Guid]::NewGuid().ToString('N'))"
[IO.File]::WriteAllLines($temporaryOutput, $actions, [Text.UTF8Encoding]::new($false))
[IO.File]::Move($temporaryOutput, $fullOutput)
Write-Output "G6-4 external UI Automation client: PASS ($fullOutput)"
