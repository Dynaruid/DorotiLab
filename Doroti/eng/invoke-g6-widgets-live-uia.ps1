#Requires -Version 5.1
param(
    [Parameter(Mandatory = $true)][int] $ProcessId,
    [Parameter(Mandatory = $true)][long] $WindowHandle,
    [Parameter(Mandatory = $true)][string] $SemanticsName,
    [Parameter(Mandatory = $true)][string] $OutputPath
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName UIAutomationClient
$window = [System.Windows.Automation.AutomationElement]::FromHandle([IntPtr]::new($WindowHandle))
if ($null -eq $window) { throw "UI Automation could not open HWND $WindowHandle." }
$condition = [System.Windows.Automation.PropertyCondition]::new(
    [System.Windows.Automation.AutomationElement]::NameProperty, $SemanticsName)
$node = $window.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $condition)
if ($null -eq $node) { throw "UI Automation could not find semantics label '$SemanticsName'." }
if ($node.Current.ControlType -ne [System.Windows.Automation.ControlType]::Button) {
    throw "The G6-2 semantics action is not exposed as a native Button."
}
$node.SetFocus()
$pattern = [System.Windows.Automation.InvokePattern]$node.GetCurrentPattern(
    [System.Windows.Automation.InvokePattern]::Pattern)
$pattern.Invoke()

$evidence = [ordered]@{
    schemaVersion = 'doroti.g6-2-widgets-live-uia/v1'
    capturedAtUtc = [DateTime]::UtcNow.ToString('O')
    clientProcessId = $PID
    targetProcessId = $ProcessId
    hwnd = $WindowHandle
    entrypoint = 'external System.Windows.Automation client over WM_GETOBJECT/UIAutomationCore'
    automationId = $node.Current.AutomationId
    name = $node.Current.Name
    controlType = $node.Current.ControlType.ProgrammaticName
    actions = @('SetFocus', 'Invoke')
    success = $true
}
$fullOutput = [IO.Path]::GetFullPath($OutputPath)
[IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($fullOutput)) | Out-Null
[IO.File]::WriteAllText($fullOutput, (($evidence | ConvertTo-Json -Depth 8) -replace "`r`n", "`n") + "`n", [Text.UTF8Encoding]::new($false))
Write-Output "G6-2 external UI Automation roundtrip: PASS ($fullOutput)"
