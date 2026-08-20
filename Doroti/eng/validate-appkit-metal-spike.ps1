param(
    [string]$Configuration = "Release",
    [string]$EvidencePath = "/tmp/doroti-appkit-metal-spike.json"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$project = Join-Path $root "validation/appkit-metal-spike/Doroti.Validation.AppKitMetalSpike.csproj"

if (-not $IsMacOS) {
    throw "The AppKit Metal live gate requires macOS. Non-macOS CI must report notVerified."
}

$workloads = & dotnet workload list 2>&1 | Out-String
if ($LASTEXITCODE -ne 0 -or $workloads -notmatch "(?m)^macos\s") {
    throw "The macos workload is missing. Run: sudo dotnet workload install macos"
}

& dotnet restore $project --locked-mode
if ($LASTEXITCODE -ne 0) { throw "AppKit spike locked restore failed." }

foreach ($buildConfiguration in @("Debug", "Release")) {
    & dotnet build $project -c $buildConfiguration --no-restore
    if ($LASTEXITCODE -ne 0) { throw "AppKit spike $buildConfiguration build failed." }
}

$executable = Join-Path $root "validation/appkit-metal-spike/bin/$Configuration/net10.0-macos/osx-arm64/Doroti AppKit Metal Spike.app/Contents/MacOS/Doroti.Validation.AppKitMetalSpike"
if (-not (Test-Path -LiteralPath $executable)) {
    throw "AppKit spike executable was not produced: $executable"
}

$env:DOROTI_APPKIT_SPIKE_AUTOMATE = "1"
$env:DOROTI_APPKIT_SPIKE_EVIDENCE = $EvidencePath
try {
    & $executable
    if ($LASTEXITCODE -ne 0) { throw "AppKit spike live process exited with $LASTEXITCODE." }
}
finally {
    Remove-Item Env:DOROTI_APPKIT_SPIKE_AUTOMATE -ErrorAction SilentlyContinue
    Remove-Item Env:DOROTI_APPKIT_SPIKE_EVIDENCE -ErrorAction SilentlyContinue
}

$evidence = Get-Content -LiteralPath $EvidencePath -Raw | ConvertFrom-Json
if ($evidence.identity -ne "macOS | net10.0-macos | osx-arm64 | AppKit-Main") {
    throw "Unexpected AppKit identity: $($evidence.identity)"
}
if ($evidence.frame.submitted -ne 1 -or $evidence.frame.presented -ne 1) {
    throw "The submitted Doroti scene did not receive exactly one presented ACK."
}
if ($evidence.native.commandBuffersCommitted -ne $evidence.native.commandBuffersCompleted) {
    throw "Metal command buffer commit/completion counts differ."
}
if ($evidence.native.commandBuffersErrored -ne 0 -or $evidence.frame.failed -ne 0) {
    throw "The AppKit Metal live gate recorded a failed frame or command buffer."
}
if ($evidence.softwareFallbackFrames -ne 0 -or $evidence.cpuReadbacks -ne 0 -or $evidence.fullFrameCopies -ne 0) {
    throw "The strict GPU-only AppKit boundary was violated."
}
if ($evidence.native.metricsGeneration -lt 21 -or $evidence.frame.replayed -lt 1) {
    throw "Resize/replay automation did not complete."
}

Write-Host "Doroti AppKit Metal spike: PASS ($EvidencePath)"
