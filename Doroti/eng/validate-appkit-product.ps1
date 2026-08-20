#Requires -Version 7.0
param(
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Debug',
    [switch] $NoBuild
)

$ErrorActionPreference = 'Stop'
if (-not $IsMacOS) { throw 'The AppKit product live gate requires macOS.' }

$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
$project = Join-Path $repoRoot 'DorotiDemoApp/macos/DorotiDemoApp.MacOS.csproj'
$outputRoot = Join-Path $dorotiRoot "artifacts/appkit-product/$Configuration"
[IO.Directory]::CreateDirectory($outputRoot) | Out-Null
$surfaceEvidence = Join-Path $outputRoot 'surface.json'
$bridgeEvidence = Join-Path $outputRoot 'native-bridge.json'
Remove-Item -LiteralPath $surfaceEvidence, $bridgeEvidence -Force -ErrorAction SilentlyContinue

if (-not $NoBuild) {
    dotnet build $project -c $Configuration -r osx-arm64 --nologo
    if ($LASTEXITCODE -ne 0) { throw 'AppKit product build failed.' }
}

$binRoot = Join-Path $repoRoot "DorotiDemoApp/macos/bin/$Configuration/net10.0-macos/osx-arm64"
$app = @(Get-ChildItem -LiteralPath $binRoot -Directory -Filter '*.app' | Select-Object -First 1)
if ($app.Count -ne 1) { throw "Exactly one AppKit bundle was expected under $binRoot." }
$plist = Join-Path $app[0].FullName 'Contents/Info.plist'
$executableName = (& plutil -extract CFBundleExecutable raw $plist).Trim()
$executable = Join-Path $app[0].FullName "Contents/MacOS/$executableName"

$startInfo = [Diagnostics.ProcessStartInfo]::new($executable)
$startInfo.WorkingDirectory = $repoRoot
$startInfo.UseShellExecute = $false
$startInfo.Environment['DOROTI_MAUI_EVIDENCE'] = $surfaceEvidence
$startInfo.Environment['DOROTI_NATIVE_BRIDGE_EVIDENCE'] = $bridgeEvidence
$startInfo.Environment['DOROTI_EXIT_AFTER_EVIDENCE'] = '1'
$process = [Diagnostics.Process]::Start($startInfo)
if ($null -eq $process) { throw 'The AppKit process did not start.' }
try {
    $deadline = [DateTimeOffset]::UtcNow.AddSeconds(30)
    while ([DateTimeOffset]::UtcNow -lt $deadline -and
           (-not (Test-Path -LiteralPath $surfaceEvidence) -or -not (Test-Path -LiteralPath $bridgeEvidence))) {
        Start-Sleep -Milliseconds 200
    }
    if (-not (Test-Path -LiteralPath $surfaceEvidence) -or -not (Test-Path -LiteralPath $bridgeEvidence)) {
        throw 'Timed out waiting for AppKit surface/native bridge evidence.'
    }
    if (-not $process.WaitForExit(10000)) { throw 'The AppKit evidence run did not terminate normally.' }
    if ($process.ExitCode -ne 0) { throw "The AppKit evidence run exited with $($process.ExitCode)." }
}
finally {
    if (-not $process.HasExited) { $process.Kill($true) }
    $process.Dispose()
}

$surface = Get-Content -LiteralPath $surfaceEvidence -Raw | ConvertFrom-Json
$bridge = Get-Content -LiteralPath $bridgeEvidence -Raw | ConvertFrom-Json
if ([string]$surface.rid -cne 'osx-arm64' -or [string]$surface.surface.graphicsBackend -cne 'AppKit/MTKView/Metal-Skia') { throw 'AppKit runtime identity mismatch.' }
if ([long]$surface.frame.presented -lt 1 -or [long]$surface.frame.replayed -lt 1 -or [long]$surface.frame.failed -ne 0) { throw 'AppKit frame terminal invariant failed.' }
if ([long]$surface.surface.commandBuffersCommitted -ne [long]$surface.surface.commandBuffersCompleted -or
    [long]$surface.surface.commandBuffersErrored -ne 0 -or [long]$surface.surface.cpuReadbacks -ne 0 -or
    [long]$surface.surface.fullFrameCopies -ne 0) { throw 'AppKit Metal completion/copy invariant failed.' }
if ([string]$bridge.platform.Platform -cne 'macOS' -or [bool]$bridge.callbackOnMainThread -ne $true) { throw 'AppKit native bridge invariant failed.' }

$fileOutput = (& file $executable | Out-String)
$links = (& otool -L $executable | Out-String)
$plistText = (& plutil -p $plist | Out-String)
$frameworks = @(Get-ChildItem -LiteralPath (Join-Path $app[0].FullName 'Contents/Frameworks') -Directory -Filter '*.framework' | Select-Object -ExpandProperty Name)
if ($fileOutput -notmatch 'Mach-O 64-bit executable arm64') { throw 'The AppKit executable is not arm64 Mach-O.' }
if ($links -notmatch '/AppKit\.framework/' -or $links -match '/UIKit\.framework/|iOSSupport') { throw 'The AppKit executable linkage is contaminated.' }
if ($plistText -match 'UIDeviceFamily|UISupportedInterfaceOrientations|UIRequiredDeviceCapabilities') { throw 'The AppKit Info.plist contains UIKit-only keys.' }
if ($frameworks -ccontains 'DorotiDemoAppNative.framework' -or $frameworks -cnotcontains 'DorotiDemoAppNative-macOS.framework') { throw 'The AppKit native framework set is contaminated.' }
& codesign --verify --deep --strict $app[0].FullName
if ($LASTEXITCODE -ne 0) { throw 'The AppKit bundle code-sign verification failed.' }

Write-Output "Doroti AppKit product live gate ($Configuration): PASS"
