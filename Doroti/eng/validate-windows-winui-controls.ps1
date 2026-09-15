param(
    [string]$OutputDirectory = (Join-Path $PSScriptRoot '../artifacts/windows-winui-controls'),
    [switch]$NoBuild
)

$ErrorActionPreference = 'Stop'
$workspace = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$project = Join-Path $workspace 'DorotiTestbedApp/windowsappsdk/DorotiTestbedApp.WindowsAppSdk.csproj'
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null
$resultPath = Join-Path $OutputDirectory 'result.json'
if (Test-Path -LiteralPath $resultPath) { Remove-Item -LiteralPath $resultPath }

function Invoke-BoundedTool([string]$Name, [string[]]$Arguments, [hashtable]$EnvironmentOverrides, [string]$Executable = 'dotnet') {
    $start = [Diagnostics.ProcessStartInfo]::new($Executable)
    $start.WorkingDirectory = $workspace
    $start.UseShellExecute = $false
    $start.CreateNoWindow = $true
    $start.RedirectStandardOutput = $true
    $start.RedirectStandardError = $true
    foreach ($argument in $Arguments) { $start.ArgumentList.Add($argument) }
    foreach ($key in $EnvironmentOverrides.Keys) {
        if ($null -eq $EnvironmentOverrides[$key]) { [void]$start.Environment.Remove($key) }
        else { $start.Environment[$key] = $EnvironmentOverrides[$key] }
    }
    $process = [Diagnostics.Process]::Start($start)
    $stdout = $process.StandardOutput.ReadToEndAsync()
    $stderr = $process.StandardError.ReadToEndAsync()
    if (-not $process.WaitForExit(1200000)) {
        $process.Kill($true)
        throw "$Name exceeded the 20-minute timeout."
    }
    $output = $stdout.GetAwaiter().GetResult() + $stderr.GetAwaiter().GetResult()
    [IO.File]::WriteAllText((Join-Path $OutputDirectory "$Name.log"), $output)
    if ($process.ExitCode -ne 0) { throw "$Name failed with exit code $($process.ExitCode); see $OutputDirectory/$Name.log" }
    if ($output -match 'PlatformView creation failed|WinUI:|error [A-Z]+\d+|Unhandled exception') {
        throw "$Name reported an error; see $OutputDirectory/$Name.log"
    }
    return $output
}

if (-not $NoBuild) {
    $null = Invoke-BoundedTool 'build' @('build', $project, '-c', 'Release', '--nologo') @{}
}

$runs = @()
foreach ($mode in @('default', 'platform-views', 'platform-views-no-blur', 'platform-views-raster-only-probe', 'platform-effects', 'embedded-native')) {
    $evidencePath = Join-Path $OutputDirectory "$mode.json"
    if (Test-Path -LiteralPath $evidencePath) { Remove-Item -LiteralPath $evidencePath }
    if (Test-Path -LiteralPath ($evidencePath + '.bmp')) { Remove-Item -LiteralPath ($evidencePath + '.bmp') }
    $output = Invoke-BoundedTool $mode @('run', '--project', $project, '-c', 'Release', '--no-build') @{
        DOROTI_TESTBED_MODE = $(if ($mode -in @('default', 'embedded-native')) { $null } elseif ($mode.StartsWith('platform-views')) { 'platform-views' } else { $mode })
        DOROTI_WINDOWS_APPSDK_SMOKE_MS = '6000'
        DOROTI_PLATFORM_VIEW_EVIDENCE = $evidencePath
        DOROTI_PLATFORM_VIEW_COMPOSITION = $null
        DOROTI_PLATFORM_VIEW_BACKDROP = $(if ($mode -eq 'platform-views-no-blur') { '0' } else { '1' })
        DOROTI_WINDOWS_PLATFORM_CAPTURE = $(if ($mode.StartsWith('platform-views') -or $mode -eq 'embedded-native') { '1' } else { $null })
        DOROTI_WINDOWS_PLATFORM_CAPTURE_FRAMES = $(if ($mode -eq 'embedded-native') { '30' } else { '1' })
        DOROTI_TESTBED_NATIVE_PAGE_PROBE = $(if ($mode -eq 'embedded-native') { '1' } else { $null })
        DOROTI_WINUI_BACKDROP_OMIT_NATIVE_PROBE = $(if ($mode -eq 'platform-views-raster-only-probe') { '1' } else { $null })
    }
    $summaryLine = @($output -split '\r?\n' | Where-Object { $_.StartsWith('doroti.windows.summary=') })[-1]
    if (-not $summaryLine) { throw "$mode did not emit a runtime summary." }
    $summary = $summaryLine.Substring('doroti.windows.summary='.Length) | ConvertFrom-Json
    if ($summary.FailedTerminals -ne 0 -or $summary.OperationalDebugErrors -ne 0 -or
        $summary.Vulkan.ValidationErrors -ne 0 -or -not $summary.VisibleAfterExactPresent) {
        throw "$mode failed runtime presentation checks."
    }
    if ($mode -eq 'platform-views') {
        if ($output -notmatch 'doroti.windows.winui=shutdown-islands=0') { throw 'WinUI islands did not retire cleanly.' }
        $evidence = Get-Content -LiteralPath $evidencePath -Raw | ConvertFrom-Json
        if ($evidence.winUi.Count -ne 2 -or $evidence.commits -lt 1) { throw 'Expected two live WinUI controls and a committed scene.' }
        if ($evidence.winUiBackdrop.effects -lt 1 -or $evidence.winUiBackdrop.nativeSources -ne 2 -or
            $evidence.winUiBackdrop.nativeReadbackBytes -ne 0 -or $evidence.winUiBackdrop.commits -lt 2) {
            throw 'WinUI backdrop did not connect both live native sources without native readback.'
        }
        foreach ($type in @('Microsoft.UI.Xaml.Controls.Button', 'Microsoft.UI.Xaml.Controls.TextBox')) {
            $control = @($evidence.winUi | Where-Object { $_.type -eq $type })
            if ($control.Count -ne 1 -or -not $control[0].loaded -or $control[0].templateChildren -lt 1 -or
                $control[0].width -le 0 -or $control[0].height -le 0 -or $control[0].cornerRadius -le 0) {
                throw "$type did not load its native Fluent template and layout."
            }
        }
    }
    if ($mode -eq 'platform-effects') {
        $evidence = Get-Content -LiteralPath $evidencePath -Raw | ConvertFrom-Json
        if ($evidence.webView.loadedViews -lt 1 -or $evidence.webView.effects -lt 1) {
            throw 'WebView2 composition/backdrop did not load.'
        }
    }
    if ($mode.StartsWith('platform-views') -or $mode -eq 'embedded-native') {
        $evidence = Get-Content -LiteralPath $evidencePath -Raw | ConvertFrom-Json
        if ($null -eq $evidence.captureProbeStatus -or $evidence.captureProbeStatus -ne 0 -or
            -not (Test-Path -LiteralPath ($evidencePath + '.bmp'))) { throw "$mode did not capture its own rendered client area." }
        if ($mode -eq 'platform-views-no-blur' -and $evidence.winUiBackdrop.effects -ne 0) { throw 'Blur-off control still contains native effects.' }
    }
    $runs += [ordered]@{ mode = $mode; exitCode = 0; presented = $summary.PresentedTerminals }
}
$null = Invoke-BoundedTool 'pixels' @((Join-Path $PSScriptRoot '../validation/platform-views/verify-windows-winui-backdrop.py'), $OutputDirectory) @{} 'python'
$null = Invoke-BoundedTool 'continuity' @((Join-Path $PSScriptRoot '../validation/platform-views/verify-windows-winui-continuity.py'), $OutputDirectory) @{} 'python'
$inputDirectory = Join-Path $OutputDirectory 'input'
$null = Invoke-BoundedTool 'input' @((Join-Path $PSScriptRoot '../validation/platform-views/verify-windows-winui-input.py'), $inputDirectory) @{} 'python'
$result = [ordered]@{
    result = 'PASS'
    runs = $runs
    controlInput = Get-Content -LiteralPath (Join-Path $inputDirectory 'result.json') -Raw | ConvertFrom-Json
    physicalDisplay = 'notVerified'
    winUiBackdropSampling = 'live CompositionVisualSurface; two native sources; bounded Gaussian effect'
    visualBlurPixels = Get-Content -LiteralPath (Join-Path $OutputDirectory 'pixels.json') -Raw | ConvertFrom-Json
    nativePageContinuity = Get-Content -LiteralPath (Join-Path $OutputDirectory 'continuity.json') -Raw | ConvertFrom-Json
}
$result | ConvertTo-Json -Depth 5 | Tee-Object -FilePath (Join-Path $OutputDirectory 'result.json')
