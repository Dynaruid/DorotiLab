#Requires -Version 5.1
param(
    [ValidateSet('All', 'N0', 'O0', 'F0', 'S0', 'R0', 'A0', 'P0')]
    [string] $Slice = 'All',
    [switch] $KeepTemporary
)

$ErrorActionPreference = 'Stop'
$env:DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER = '1'
$env:MSBUILDDISABLENODEREUSE = '1'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
. (Join-Path $PSScriptRoot 'local-storage.ps1')
$temporaryRoot = New-DorotiTemporaryDirectory -DorotiRoot $dorotiRoot -Name 'g6-app-slices'
$project = Join-Path $dorotiRoot 'validation/Doroti.Validation.G6AppSlices/Doroti.Validation.G6AppSlices.csproj'
$dll = Join-Path $dorotiRoot 'validation/Doroti.Validation.G6AppSlices/bin/Release/net10.0/Doroti.Validation.G6AppSlices.dll'
$evidenceRoot = Join-Path $dorotiRoot 'migration/flutter-framework/g6-app-slices'
$aggregatePath = Join-Path $dorotiRoot 'migration/flutter-framework/g6-app-slices.json'
$orderedSlices = @('N0', 'O0', 'F0', 'S0', 'R0', 'A0', 'P0')

function Assert-True([bool] $Condition, [string] $Name) {
    if (-not $Condition) { throw "$Name failed." }
}

function Invoke-Checked([scriptblock] $Command, [string] $Failure) {
    $global:LASTEXITCODE = 0
    & $Command
    if ($LASTEXITCODE -ne 0) { throw "$Failure (exit code $LASTEXITCODE)." }
}

function Write-Json([string] $Path, [object] $Value) {
    [IO.Directory]::CreateDirectory((Split-Path $Path -Parent)) | Out-Null
    $temporary = $Path + '.tmp-' + [Guid]::NewGuid().ToString('N')
    [IO.File]::WriteAllText(
        $temporary,
        (($Value | ConvertTo-Json -Depth 64) -replace "`r`n", "`n") + "`n",
        [Text.UTF8Encoding]::new($false))
    if ([IO.File]::Exists($Path)) {
        [IO.File]::Copy($temporary, $Path, $true)
        [IO.File]::Delete($temporary)
    } else {
        [IO.File]::Move($temporary, $Path)
    }
}

function Assert-SliceEvidence([string] $Name, [string] $Path) {
    $proof = Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json
    Assert-True ($proof.schemaVersion -eq 'doroti.g6-app-slice-evidence/v1') "$Name evidence schema"
    Assert-True ($proof.slice -eq $Name) "$Name evidence identity"
    Assert-True ($proof.status -eq 'verified-windows-x64-strict-gpu') "$Name live status"
    Assert-True ([bool]$proof.reference.pass) "$Name reference trace"
    Assert-True ($proof.live.backend -eq 'skia-wgl-opengl-gpu') "$Name strict GPU backend"
    Assert-True (-not [bool]$proof.live.softwareFallback) "$Name software fallback blocked"
    Assert-True ([long]$proof.live.presented -gt 0) "$Name native presentation"
    Assert-True ([long]$proof.live.changedPixels -ge 100) "$Name interactive raster change"
    if ($Name -eq 'S0') {
        Assert-True (@($proof.live.nativeInputTrace).Count -eq 4) 'S0 native wheel/drag/keyboard trace'
        Assert-True ([double]$proof.live.scrollOffset -gt 0) 'S0 native scroll offset'
        Assert-True ([long]$proof.live.input.pointerPackets -ge 8) 'S0 host pointer packet delivery'
        Assert-True ([long]$proof.live.input.keyPackets -ge 2) 'S0 host keyboard packet delivery'
    }
    Assert-True ([bool]$proof.live.resourceClosure.isBalanced) "$Name native resource closure"
    Assert-True ([long]$proof.unsupportedSilentSuccessCount -eq 0) "$Name silent unsupported success"
    Assert-True ($proof.boundaries.physicalIme -eq 'notVerified') "$Name physical IME boundary"
    Assert-True ($proof.boundaries.physicalAccessibility -eq 'notVerified') "$Name physical accessibility boundary"
    if ($Name -eq 'A0') {
        Assert-True (@($proof.live.externalAutomation).Count -eq 5) 'A0 external UIA action count'
    }
    if ($Name -eq 'R0') {
        Assert-True (@($proof.managed.koreanFallbackFamilies).Count -gt 0) 'R0 Korean system font fallback'
        Assert-True ($proof.managed.koreanGlyphFallback -like 'verified-*') 'R0 Korean glyph render path'
    }
    return $proof
}

function Invoke-A0([string] $EvidencePath) {
    $runRoot = Join-Path $temporaryRoot 'a0'
    [IO.Directory]::CreateDirectory($runRoot) | Out-Null
    $readyPath = Join-Path $runRoot 'ready.json'
    $actionsPath = Join-Path $runRoot 'actions.txt'
    $stdoutPath = Join-Path $runRoot 'stdout.log'
    $stderrPath = Join-Path $runRoot 'stderr.log'
    $arguments = @($dll, '--slice', 'A0', '--evidence', $EvidencePath, '--ready', $readyPath, '--external-actions', $actionsPath)
    $process = Start-Process -FilePath 'dotnet' -ArgumentList $arguments -PassThru -WindowStyle Normal -RedirectStandardOutput $stdoutPath -RedirectStandardError $stderrPath
    try {
        $deadline = [DateTime]::UtcNow.AddSeconds(30)
        while (-not (Test-Path -LiteralPath $readyPath) -and -not $process.HasExited -and [DateTime]::UtcNow -lt $deadline) {
            Start-Sleep -Milliseconds 50
            $process.Refresh()
        }
        if (-not (Test-Path -LiteralPath $readyPath)) {
            throw "A0 did not publish UIA readiness.`n$(Get-Content -LiteralPath $stderrPath -Raw -ErrorAction SilentlyContinue)"
        }
        $ready = Get-Content -LiteralPath $readyPath -Raw | ConvertFrom-Json
        & (Join-Path $PSScriptRoot 'invoke-g6-app-slice-uia.ps1') -ProcessId $ready.processId -WindowHandle $ready.hwnd -OutputPath $actionsPath
        if (-not $process.WaitForExit(60000)) {
            $process.Kill()
            throw 'A0 exceeded the 60 second native/UIA budget.'
        }
        $process.Refresh()
        if ([int]$process.ExitCode -ne 0) {
            throw "A0 failed.`n$(Get-Content -LiteralPath $stdoutPath -Raw)`n$(Get-Content -LiteralPath $stderrPath -Raw)"
        }
    }
    finally {
        if (-not $process.HasExited) { $process.Kill() }
        $process.Dispose()
    }
}

function Update-AggregateEvidence {
    $entries = @()
    foreach ($name in $orderedSlices) {
        $path = Join-Path $evidenceRoot "$name-evidence.json"
        if (Test-Path -LiteralPath $path) {
            $entries += Get-Content -LiteralPath $path -Raw | ConvertFrom-Json
        }
    }
    $complete = $entries.Count -eq $orderedSlices.Count -and @($entries | Where-Object { $_.status -ne 'verified-windows-x64-strict-gpu' }).Count -eq 0
    $referencePath = Join-Path $dorotiRoot 'validation/Doroti.Validation.G6AppSlices/Resources/g6-reference-traces.json'
    Write-Json $aggregatePath ([ordered]@{
        schemaVersion = 'doroti.g6-app-slices/v1'
        milestone = 'G6-4'
        capturedAtUtc = [DateTime]::UtcNow.ToString('O')
        status = if ($complete) { 'verified-windows-x64-strict-gpu' } else { 'partial' }
        reference = [ordered]@{
            path = 'Doroti/validation/Doroti.Validation.G6AppSlices/Resources/g6-reference-traces.json'
            sha256 = (Get-FileHash -LiteralPath $referencePath -Algorithm SHA256).Hash.ToLowerInvariant()
        }
        slices = $entries
        boundaries = [ordered]@{
            automatedNative = 'verified-windows-x64'
            physicalIme = 'notVerified'
            physicalAccessibility = 'notVerified'
            linux = 'notVerified'
            macOS = 'notVerified'
            otherRids = 'notVerified'
        }
    })
}

try {
    [IO.Directory]::CreateDirectory($evidenceRoot) | Out-Null
    Invoke-Checked {
        dotnet build $project --configuration Release --nologo --no-restore --disable-build-servers --tl:off -nodeReuse:false -p:UseSharedCompilation=false
    } 'G6-4 application slice build failed'

    $selected = if ($Slice -eq 'All') {
        $orderedSlices
    } else {
        $orderedSlices[0..[Array]::IndexOf($orderedSlices, $Slice)]
    }
    foreach ($name in $selected) {
        $evidencePath = Join-Path $evidenceRoot "$name-evidence.json"
        if ($name -eq 'A0') {
            Invoke-A0 $evidencePath
        } else {
            Invoke-Checked { dotnet $dll --slice $name --evidence $evidencePath } "G6-4 $name live slice failed"
        }
        Assert-SliceEvidence $name $evidencePath | Out-Null
        Write-Output "G6-4 $name reference/managed/live native gate: PASS"
    }

    Update-AggregateEvidence
    if ($Slice -eq 'All') {
        $aggregate = Get-Content -LiteralPath $aggregatePath -Raw | ConvertFrom-Json
        Assert-True ($aggregate.status -eq 'verified-windows-x64-strict-gpu') 'G6-4 aggregate evidence'
        Invoke-Checked { & (Join-Path $PSScriptRoot 'validate-g6-material-demo.ps1') -Shard Evidence } 'G6-3 committed evidence regression failed'
    }
    Write-Output "G6-4 $Slice application slice validation: PASS"
}
finally {
    dotnet build-server shutdown --msbuild --vbcscompiler | Out-Host
    if (-not $KeepTemporary) { Remove-DorotiTemporaryItem -DorotiRoot $dorotiRoot -Path $temporaryRoot }
}
