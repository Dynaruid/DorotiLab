#Requires -Version 5.1
param(
    [ValidateSet('All', 'LiveWindows', 'Compiler', 'Regression', 'Evidence')]
    [string] $Shard = 'All'
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
. (Join-Path $PSScriptRoot 'local-storage.ps1')
$project = Join-Path $dorotiRoot 'validation/Doroti.Validation.G6WidgetsLive/Doroti.Validation.G6WidgetsLive.csproj'
$referencePath = Join-Path $dorotiRoot 'validation/Doroti.Validation.G6WidgetsLive/g6-widgets-reference.json'
$evidencePath = Join-Path $dorotiRoot 'migration/flutter-framework/g6-widgets-live-evidence.json'
$temporaryRoot = New-DorotiTemporaryDirectory -DorotiRoot $dorotiRoot -Name 'g6-widgets-live'
$readyPath = Join-Path $temporaryRoot 'ready.json'
$uiaPath = Join-Path $temporaryRoot 'uia.json'
$stdoutPath = Join-Path $temporaryRoot 'stdout.log'
$stderrPath = Join-Path $temporaryRoot 'stderr.log'

function Test-Shard([string] $Name) { return $Shard -eq 'All' -or $Shard -eq $Name }
function Assert-True([bool] $Condition, [string] $Name) { if (-not $Condition) { throw "$Name failed." } }
function Invoke-Checked([scriptblock] $Command, [string] $Failure) {
    $global:LASTEXITCODE = 0
    & $Command
    if ($LASTEXITCODE -ne 0) { throw "$Failure (exit code $LASTEXITCODE)." }
}
function Assert-Reference([string] $ActualEvidence) {
    $actual = Get-Content -LiteralPath $ActualEvidence -Raw | ConvertFrom-Json
    $reference = Get-Content -LiteralPath $referencePath -Raw | ConvertFrom-Json
    Assert-True ($actual.status -eq 'verified-windows-x64-strict-gpu') 'strict GPU live status'
    Assert-True ($actual.flutterRevision -eq $reference.flutterRevision) 'Flutter source pin'
    Assert-True ($actual.activeDiagnostics.frame.backendIdentity -eq 'skia-wgl-opengl-gpu') 'strict backend identity'
    Assert-True (-not [bool]$actual.activeDiagnostics.frame.softwareFallbackUsed) 'software fallback blocked'
    Assert-True ($actual.activeDiagnostics.frame.presented -ge 300) '300 presented frames'
    $terminal = [long]$actual.activeDiagnostics.frame.presented + [long]$actual.activeDiagnostics.frame.superseded + [long]$actual.activeDiagnostics.frame.stale + [long]$actual.activeDiagnostics.frame.failed + [long]$actual.activeDiagnostics.frame.cancelled
    Assert-True ([long]$actual.activeDiagnostics.frame.submitted -eq $terminal) 'terminal ACK balance'
    Assert-True ([bool]$actual.resourceClosure.isBalanced) 'native resource closure'
    Assert-True ([long]$actual.firstPixels.nonTransparent -eq ([long]$actual.firstPixels.width * [long]$actual.firstPixels.height)) 'non-empty first frame'
    Assert-True ([long]$actual.firstPixels.accentCount -gt 0) 'accent paint'
    $coverage = [double]$actual.firstPixels.backgroundCount / ([double]$actual.firstPixels.width * [double]$actual.firstPixels.height)
    Assert-True ($coverage -ge [double]$reference.layoutPaintTolerance.minimumBackgroundCoverage) 'background coverage tolerance'
    $scale = [double]$actual.firstPixels.width / 640.0
    foreach ($sample in @(@($actual.firstPixels.accentBounds, $reference.layoutPaintTolerance.initialAccentBounds), @($actual.resizedPixels.accentBounds, $reference.layoutPaintTolerance.resizedAccentBounds))) {
        foreach ($property in @('x', 'y', 'width', 'height')) {
            $delta = [Math]::Abs(([double]$sample[0].$property / $scale) - [double]$sample[1].$property)
            Assert-True ($delta -le [double]$reference.layoutPaintTolerance.logicalPixelTolerance) "accent $property tolerance"
        }
    }
    $position = 0
    foreach ($expected in @($reference.requiredOrderedTrace)) {
        $found = $false
        while ($position -lt @($actual.treeTrace).Count) {
            if ([string]$actual.treeTrace[$position++] -ceq [string]$expected) { $found = $true; break }
        }
        Assert-True $found "reference trace event $expected"
    }
}

if (Test-Shard 'LiveWindows') {
    Invoke-Checked { dotnet build $project --configuration Release --nologo } 'G6-2 live validator build failed'
    $dll = Join-Path $dorotiRoot 'validation/Doroti.Validation.G6WidgetsLive/bin/Release/net10.0/Doroti.Validation.G6WidgetsLive.dll'
    $arguments = @($dll, '--duration-ms', '30000', '--frames', '300', '--evidence', $evidencePath, '--ready', $readyPath, '--require-external-uia')
    $process = Start-Process -FilePath 'dotnet' -ArgumentList $arguments -PassThru -WindowStyle Hidden -RedirectStandardOutput $stdoutPath -RedirectStandardError $stderrPath
    $deadline = [DateTime]::UtcNow.AddSeconds(30)
    while (-not (Test-Path -LiteralPath $readyPath) -and -not $process.HasExited -and [DateTime]::UtcNow -lt $deadline) { Start-Sleep -Milliseconds 50 }
    if (-not (Test-Path -LiteralPath $readyPath)) { throw "G6-2 live target did not publish readiness.`n$(Get-Content $stderrPath -Raw)" }
    $ready = Get-Content -LiteralPath $readyPath -Raw | ConvertFrom-Json
    & (Join-Path $PSScriptRoot 'invoke-g6-widgets-live-uia.ps1') -ProcessId $ready.processId -WindowHandle $ready.hwnd -SemanticsName $ready.semanticsName -OutputPath $uiaPath
    if (-not $process.WaitForExit(120000)) { $process.Kill(); throw 'G6-2 live target exceeded the 120 second shard budget.' }
    $process.Refresh()
    $exitCode = if ($null -eq $process.ExitCode) { 0 } else { [int]$process.ExitCode }
    if ($exitCode -ne 0) { throw "G6-2 live target failed with exit code $exitCode.`n$(Get-Content $stdoutPath -Raw)`n$(Get-Content $stderrPath -Raw)" }
    Assert-Reference $evidencePath
    $uia = Get-Content -LiteralPath $uiaPath -Raw | ConvertFrom-Json
    Assert-True ([bool]$uia.success) 'external UIA evidence'
    Write-Output "G6-2 30 second/300 frame live Windows gate: PASS"
}
if (Test-Shard 'Compiler') {
    Invoke-Checked { & (Join-Path $PSScriptRoot 'validate-g6-language-runtime.ps1') -Shard Fixtures } 'G6-2 constructor-body compiler regression failed'
}
if (Test-Shard 'Regression') {
    Invoke-Checked { dotnet run --project (Join-Path $dorotiRoot 'validation/Doroti.Validation.G5Widgets/Doroti.Validation.G5Widgets.csproj') --configuration Release } 'G5 Widgets managed regression failed'
    Invoke-Checked { dotnet build (Join-Path $dorotiRoot 'Doroti.Product.slnx') --configuration Release --nologo } 'Doroti product solution regression failed'
}
if (Test-Shard 'Evidence') { Assert-Reference $evidencePath; Write-Output 'G6-2 committed evidence/reference gate: PASS' }
