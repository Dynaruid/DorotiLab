#Requires -Version 5.1
param(
    [Parameter(Mandatory = $true)][int] $ProcessId,
    [Parameter(Mandatory = $true)][long] $WindowHandle,
    [Parameter(Mandatory = $true)][string] $OutputPath
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName UIAutomationClient
Add-Type @'
using System;
using System.Runtime.InteropServices;
public static class G52NativeWindowLookup
{
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr FindWindow(string className, string windowName);
    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr window, out uint processId);
}
'@

$window = $null
$deadline = [DateTime]::UtcNow.AddSeconds(5)
while ($null -eq $window -and [DateTime]::UtcNow -lt $deadline) {
    $handle = [IntPtr]::new($WindowHandle)
    if ($handle -ne [IntPtr]::Zero) {
        [uint32] $owner = 0
        [void][G52NativeWindowLookup]::GetWindowThreadProcessId($handle, [ref]$owner)
        if ($owner -eq $ProcessId) {
            $window = [System.Windows.Automation.AutomationElement]::FromHandle($handle)
        }
    }
    if ($null -eq $window) { Start-Sleep -Milliseconds 50 }
}
if ($null -eq $window) { throw "UI Automation could not find the target window for process $ProcessId." }

function Find-Node {
    param([string] $AutomationId)
    $condition = [System.Windows.Automation.PropertyCondition]::new(
        [System.Windows.Automation.AutomationElement]::AutomationIdProperty,
        $AutomationId)
    $node = $window.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $condition)
    if ($null -eq $node) { throw "UI Automation could not find $AutomationId." }
    return $node
}

$button = Find-Node 'semantics-2'
$toggle = Find-Node 'semantics-3'
$text = Find-Node 'semantics-4'
$scroll = Find-Node 'semantics-5'

if ($button.Current.ControlType -ne [System.Windows.Automation.ControlType]::Button) { throw 'semantics-2 is not exposed as a native Button.' }
if ($toggle.Current.ControlType -ne [System.Windows.Automation.ControlType]::CheckBox) { throw 'semantics-3 is not exposed as a native CheckBox.' }
if ($text.Current.ControlType -ne [System.Windows.Automation.ControlType]::Edit) { throw 'semantics-4 is not exposed as a native Edit.' }
if ($scroll.Current.ControlType -ne [System.Windows.Automation.ControlType]::List) { throw 'semantics-5 is not exposed as a native List.' }
$descendants = $window.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$treeOrder = @($descendants | ForEach-Object { $_.Current.AutomationId } | Where-Object { $_ -like 'semantics-*' })
if (($treeOrder -join ',') -ne 'semantics-1,semantics-2,semantics-3,semantics-4,semantics-5') {
    throw "Native semantics traversal order drifted: $($treeOrder -join ',')."
}

$button.SetFocus()
$invokePattern = [System.Windows.Automation.InvokePattern]$button.GetCurrentPattern(
    [System.Windows.Automation.InvokePattern]::Pattern)
$invokePattern.Invoke()
$togglePattern = [System.Windows.Automation.TogglePattern]$toggle.GetCurrentPattern(
    [System.Windows.Automation.TogglePattern]::Pattern)
if ($togglePattern.Current.ToggleState -ne [System.Windows.Automation.ToggleState]::Off) { throw 'Native toggle state did not preserve Flutter isToggled=false.' }
$togglePattern.Toggle()
$valuePattern = [System.Windows.Automation.ValuePattern]$text.GetCurrentPattern(
    [System.Windows.Automation.ValuePattern]::Pattern)
$valuePattern.SetValue('external-uia-text')
$scrollPattern = [System.Windows.Automation.ScrollPattern]$scroll.GetCurrentPattern(
    [System.Windows.Automation.ScrollPattern]::Pattern)
$scrollPattern.Scroll(
    [System.Windows.Automation.ScrollAmount]::NoAmount,
    [System.Windows.Automation.ScrollAmount]::SmallIncrement)

$evidence = [ordered]@{
    schemaVersion = 'doroti.g5-2-external-uia-client/v1'
    capturedAtUtc = [DateTime]::UtcNow.ToString('O')
    clientProcessId = $PID
    targetProcessId = $ProcessId
    targetWindow = $window.Current.Name
    entrypoint = 'System.Windows.Automation external client over WM_GETOBJECT/UIAutomationCore'
    exposedTree = [ordered]@{
        traversalOrder = $treeOrder
        roles = [ordered]@{
            'semantics-2' = $button.Current.ControlType.ProgrammaticName
            'semantics-3' = $toggle.Current.ControlType.ProgrammaticName
            'semantics-4' = $text.Current.ControlType.ProgrammaticName
            'semantics-5' = $scroll.Current.ControlType.ProgrammaticName
        }
        toggleStateBeforeAction = 'Off'
        patterns = @('Invoke', 'Toggle', 'Value', 'Scroll')
    }
    actions = @(
        [ordered]@{ node = 2; action = 'focus' },
        [ordered]@{ node = 2; action = 'invoke' },
        [ordered]@{ node = 3; action = 'toggle' },
        [ordered]@{ node = 4; action = 'setText'; value = 'external-uia-text' },
        [ordered]@{ node = 5; action = 'scrollDown' }
    )
    success = $true
}
$fullOutput = [IO.Path]::GetFullPath($OutputPath)
[IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($fullOutput)) | Out-Null
$temporary = $fullOutput + '.tmp-' + [Guid]::NewGuid().ToString('N')
[IO.File]::WriteAllText($temporary, (($evidence | ConvertTo-Json -Depth 8) -replace "`r`n", "`n") + "`n", [Text.UTF8Encoding]::new($false))
if (Test-Path -LiteralPath $fullOutput) {
    [IO.File]::Replace($temporary, $fullOutput, $null)
}
else {
    [IO.File]::Move($temporary, $fullOutput)
}
Write-Output "G5-2 external UI Automation client: PASS ($fullOutput)"
