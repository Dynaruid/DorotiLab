#Requires -Version 7.0
[CmdletBinding()]
param(
    [string] $EvidencePath
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$timeoutMilliseconds = 20 * 60 * 1000
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$publishRoot = Join-Path $repoRoot '.doroti\publish'
$runRoot = Join-Path $publishRoot ("c9-" + (Get-Date).ToUniversalTime().ToString('yyyyMMddTHHmmssfffZ'))
$publishDirectory = Join-Path $runRoot 'product'
$project = Join-Path $repoRoot 'Doroti\validation\hwnd-exact-cpp-product\Doroti.Validation.HwndExactCppProduct.csproj'
if ([string]::IsNullOrWhiteSpace($EvidencePath)) {
    $EvidencePath = Join-Path $repoRoot '.doroti\evidence\hwnd-exact-cpp-c9-publish.json'
}
$EvidencePath = [IO.Path]::GetFullPath($EvidencePath)

function Assert-WithinRunRoot([string] $Path) {
    $full = [IO.Path]::GetFullPath($Path)
    $root = [IO.Path]::GetFullPath($runRoot).TrimEnd([IO.Path]::DirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar
    if (-not $full.StartsWith($root, [StringComparison]::OrdinalIgnoreCase)) {
        throw "C9 probe path escaped its run root: $full"
    }
}

function New-Probe([string] $Name) {
    $directory = Join-Path $runRoot $Name
    Assert-WithinRunRoot $directory
    [void](New-Item -ItemType Directory -Path $directory)
    Get-ChildItem -LiteralPath $publishDirectory | Copy-Item -Destination $directory -Recurse
    return $directory
}

function Invoke-Probe([string] $Name, [string] $Directory, [switch] $Audit) {
    Assert-WithinRunRoot $Directory
    $executable = Join-Path $Directory 'Doroti.Validation.HwndExactCppProduct.exe'
    $report = Join-Path $Directory "$Name-report.json"
    $start = [Diagnostics.ProcessStartInfo]::new()
    $start.FileName = $executable
    $start.WorkingDirectory = $Directory
    $start.UseShellExecute = $false
    $start.CreateNoWindow = $true
    $start.RedirectStandardOutput = $true
    $start.RedirectStandardError = $true
    $start.Environment['PATH'] = ''
    if ($Audit) { $start.Environment['DOROTI_WINDOWS_NATIVE_AUDIT'] = '1' }
    [void]$start.ArgumentList.Add('--report')
    [void]$start.ArgumentList.Add($report)
    $process = [Diagnostics.Process]::new()
    $process.StartInfo = $start
    if (-not $process.Start()) { throw "C9 $Name probe failed to start." }
    $stdoutTask = $process.StandardOutput.ReadToEndAsync()
    $stderrTask = $process.StandardError.ReadToEndAsync()
    if (-not $process.WaitForExit($timeoutMilliseconds)) {
        $process.Kill($true)
        throw "C9 $Name probe exceeded the 20-minute timeout."
    }
    [pscustomobject]@{
        name = $Name
        exitCode = $process.ExitCode
        stdout = $stdoutTask.GetAwaiter().GetResult()
        stderr = $stderrTask.GetAwaiter().GetResult()
        report = $report
    }
}

[void](New-Item -ItemType Directory -Path $publishDirectory -Force)
& dotnet publish $project -c Release -r win-x64 --self-contained true -o $publishDirectory
if ($LASTEXITCODE -ne 0) { throw "C9 self-contained publish failed with exit code $LASTEXITCODE." }

$nativeFiles = @(
    'doroti_windows_appsdk_host_v1.dll',
    'Microsoft.WindowsAppRuntime.Bootstrap.dll',
    'av_libglesv2.dll'
) | ForEach-Object {
    $path = Join-Path $publishDirectory $_
    [pscustomobject]@{
        name = $_
        sha256 = (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant()
        length = (Get-Item -LiteralPath $path).Length
    }
}
$nativeByName = @{}
$nativeFiles | ForEach-Object { $nativeByName[$_.name] = $_ }
[ordered]@{
    schemaVersion = 'doroti.windows.native-provenance/v1'
    nativeHostSha256 = $nativeByName['doroti_windows_appsdk_host_v1.dll'].sha256
    bootstrapSha256 = $nativeByName['Microsoft.WindowsAppRuntime.Bootstrap.dll'].sha256
    angleRuntimeSha256 = $nativeByName['av_libglesv2.dll'].sha256
} | ConvertTo-Json -Compress | Set-Content -LiteralPath (Join-Path $publishDirectory 'doroti-native-provenance.json') -Encoding utf8

$successDirectory = New-Probe 'success'
$success = Invoke-Probe 'success' $successDirectory

$auditDirectory = New-Probe 'audit-success'
$audit = Invoke-Probe 'audit-success' $auditDirectory -Audit

$missingDirectory = New-Probe 'missing-native'
$missingTarget = Join-Path $missingDirectory 'doroti_windows_appsdk_host_v1.dll'
Assert-WithinRunRoot $missingTarget
Remove-Item -LiteralPath $missingTarget -Force
$missing = Invoke-Probe 'missing-native' $missingDirectory

$missingAngleDirectory = New-Probe 'missing-angle-runtime'
$missingAngleTarget = Join-Path $missingAngleDirectory 'av_libglesv2.dll'
Assert-WithinRunRoot $missingAngleTarget
Remove-Item -LiteralPath $missingAngleTarget -Force
$missingAngle = Invoke-Probe 'missing-angle-runtime' $missingAngleDirectory

$architectureDirectory = New-Probe 'wrong-architecture'
$architectureTarget = Join-Path $architectureDirectory 'doroti_windows_appsdk_host_v1.dll'
Assert-WithinRunRoot $architectureTarget
Copy-Item -LiteralPath (Join-Path $architectureDirectory 'Doroti.Ui.dll') -Destination $architectureTarget -Force
$architecture = Invoke-Probe 'wrong-architecture' $architectureDirectory

$versionDirectory = New-Probe 'wrong-version'
$versionTarget = Join-Path $versionDirectory 'doroti_windows_appsdk_host_v1.dll'
Assert-WithinRunRoot $versionTarget
Copy-Item -LiteralPath (Join-Path $versionDirectory 'Microsoft.WindowsAppRuntime.Bootstrap.dll') -Destination $versionTarget -Force
$version = Invoke-Probe 'wrong-version' $versionDirectory

if ($success.exitCode -ne 0) { throw 'C9 app-directory success launch failed.' }
if ($audit.exitCode -ne 0) { throw 'C9 full-hash provenance audit launch failed.' }
if ($missing.exitCode -eq 0 -or $missing.stderr -notmatch 'missing from the application directory') {
    throw 'C9 missing-native launch did not fail fast with the expected identity.'
}
if ($missingAngle.exitCode -eq 0 -or $missingAngle.stderr -notmatch 'ANGLE EGL/GLES runtime is missing') {
    throw 'C9 missing ANGLE runtime launch did not fail fast with the expected identity.'
}
if ($architecture.exitCode -eq 0 -or $architecture.stderr -notmatch 'not a win-x64 PE image') {
    throw 'C9 wrong-architecture launch did not fail fast with the expected identity.'
}
if ($version.exitCode -eq 0 -or $version.stderr -notmatch 'EntryPointNotFound|entry point') {
    throw 'C9 wrong-version launch did not fail fast at the ABI export boundary.'
}

$unexpectedAngleFiles = @(@('libEGL.dll', 'libGLESv2.dll') |
    Where-Object { Test-Path (Join-Path $publishDirectory $_) })
if ($unexpectedAngleFiles.Count -ne 0) {
    throw "C9 publish contains unexpected split ANGLE native files: $($unexpectedAngleFiles -join ', ')."
}

$report = [ordered]@{
    schemaVersion = 'doroti.windows.hwnd-exact-cpp-c9-publish/v1'
    gate = 'C9'
    status = 'PASS'
    generatedAt = (Get-Date).ToUniversalTime().ToString('o')
    publishDirectory = $publishDirectory
    searchPolicy = 'PATH empty in all probes; app-directory native resolver excludes PATH/current-directory'
    nativeFiles = $nativeFiles
    probes = @($success, $audit, $missing, $missingAngle, $architecture, $version) | ForEach-Object {
        [ordered]@{
            name = $_.name
            exitCode = $_.exitCode
            expected = if ($_.name -in @('success', 'audit-success')) { 'success' } else { 'explicit fail-fast' }
        }
    }
    boundary = 'Clean self-contained publish and install-like app-directory launch. Signed installer/MSIX is outside this gate.'
}
[void](New-Item -ItemType Directory -Path ([IO.Path]::GetDirectoryName($EvidencePath)) -Force)
$report | ConvertTo-Json -Depth 10 | Set-Content -LiteralPath $EvidencePath -Encoding utf8
$report | ConvertTo-Json -Depth 10
