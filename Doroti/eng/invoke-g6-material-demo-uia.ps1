#Requires -Version 5.1
param(
    [Parameter(Mandatory = $true)][int] $ProcessId,
    [Parameter(Mandatory = $true)][long] $WindowHandle,
    [Parameter(Mandatory = $true)][string[]] $SemanticsNames,
    [Parameter(Mandatory = $true)][string] $OutputPath
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName UIAutomationClient
$window = [System.Windows.Automation.AutomationElement]::FromHandle([IntPtr]::new($WindowHandle))
if ($null -eq $window) { throw "UI Automation could not open HWND $WindowHandle." }

$results = @()
foreach ($name in $SemanticsNames) {
    $condition = [System.Windows.Automation.PropertyCondition]::new(
        [System.Windows.Automation.AutomationElement]::NameProperty, $name)
    $node = $window.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $condition)
    if ($null -eq $node) { throw "UI Automation could not find Material semantics label '$name'." }
    $before = [ordered]@{
        automationId = $node.Current.AutomationId
        controlType = $node.Current.ControlType.ProgrammaticName
        value = $node.Current.HelpText
    }
    $pattern = [System.Windows.Automation.InvokePattern]$node.GetCurrentPattern(
        [System.Windows.Automation.InvokePattern]::Pattern)
    $pattern.Invoke()
    Start-Sleep -Milliseconds 100
    $updated = $window.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $condition)
    if ($null -eq $updated) { throw "Material semantics label '$name' disappeared after Invoke." }
    $results += [ordered]@{
        name = $name
        before = $before
        after = [ordered]@{
            automationId = $updated.Current.AutomationId
            controlType = $updated.Current.ControlType.ProgrammaticName
            value = $updated.Current.HelpText
        }
        action = 'Invoke'
        success = $true
    }
}

$evidence = [ordered]@{
    schemaVersion = 'doroti.g6-material-demo-uia/v1'
    capturedAtUtc = [DateTime]::UtcNow.ToString('O')
    clientProcessId = $PID
    targetProcessId = $ProcessId
    hwnd = $WindowHandle
    entrypoint = 'external System.Windows.Automation client over WM_GETOBJECT/UIAutomationCore'
    controls = $results
    success = $results.Count -eq $SemanticsNames.Count
}
$fullOutput = [IO.Path]::GetFullPath($OutputPath)
[IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($fullOutput)) | Out-Null
[IO.File]::WriteAllText($fullOutput, (($evidence | ConvertTo-Json -Depth 12) -replace "`r`n", "`n") + "`n", [Text.UTF8Encoding]::new($false))
Write-Output "G6-3 Material external UI Automation: PASS ($fullOutput)"
