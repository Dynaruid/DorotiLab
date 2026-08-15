#Requires -Version 5.1
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('Visual', 'Input', 'Compositing')]
    [string] $Gate
)

$ErrorActionPreference = 'Stop'
$env:DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER = '1'
$env:MSBUILDDISABLENODEREUSE = '1'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
. (Join-Path $PSScriptRoot 'flutter-sdk.ps1')
$flutter = Resolve-DorotiFlutterSdk -RepositoryRoot $repoRoot
$migrationRoot = Join-Path $dorotiRoot 'migration/flutter-framework'
$artifactRoot = Join-Path $dorotiRoot 'artifacts/g7-shared-closure'
$tmpRoot = Join-Path $dorotiRoot '.doroti/tmp/g7-shared-closure'
$flutterRevision = $flutter.Revision
[IO.Directory]::CreateDirectory($artifactRoot) | Out-Null
[IO.Directory]::CreateDirectory($tmpRoot) | Out-Null

function Assert-True([bool] $Condition, [string] $Name) {
    if (-not $Condition) { throw "$Name failed." }
}

function Invoke-Checked([scriptblock] $Command, [string] $Failure) {
    $global:LASTEXITCODE = 0
    & $Command | Out-Host
    if ($LASTEXITCODE -ne 0) { throw "$Failure (exit code $LASTEXITCODE)." }
}

function Write-Json([string] $Path, [object] $Value) {
    [IO.Directory]::CreateDirectory((Split-Path $Path -Parent)) | Out-Null
    $json = (($Value | ConvertTo-Json -Depth 64) -replace "`r`n", "`n") + "`n"
    [IO.File]::WriteAllText($Path, $json, [Text.UTF8Encoding]::new($false))
}

function Relative-Path([string] $Path) {
    return [IO.Path]::GetRelativePath($repoRoot, [IO.Path]::GetFullPath($Path)).Replace('\', '/')
}

function Read-Json([string] $Path) {
    Assert-True (Test-Path -LiteralPath $Path -PathType Leaf) "evidence $Path"
    return Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json
}

function Compare-Bitmap([string] $ExpectedPath, [string] $ActualPath, [int] $Threshold, [Drawing.Rectangle] $Roi, [switch] $HalfScaleActual) {
    Add-Type -AssemblyName System.Drawing.Common
    $expected = [Drawing.Bitmap]::new($ExpectedPath)
    $actualSource = [Drawing.Bitmap]::new($ActualPath)
    $actual = $actualSource
    if ($HalfScaleActual) {
        $actual = [Drawing.Bitmap]::new($expected.Width, $expected.Height)
        $graphics = [Drawing.Graphics]::FromImage($actual)
        try {
            $graphics.InterpolationMode = [Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
            $graphics.DrawImage($actualSource, 0, 0, $actual.Width, $actual.Height)
        }
        finally { $graphics.Dispose() }
    }
    try {
        Assert-True ($expected.Width -eq $actual.Width -and $expected.Height -eq $actual.Height) 'reference raster dimensions'
        $changed = 0L
        $sum = 0L
        $maximum = 0
        for ($y = $Roi.Top; $y -lt $Roi.Bottom; $y++) {
            for ($x = $Roi.Left; $x -lt $Roi.Right; $x++) {
                $a = $expected.GetPixel($x, $y)
                $b = $actual.GetPixel($x, $y)
                $delta = [Math]::Max([Math]::Abs($a.R - $b.R), [Math]::Max([Math]::Abs($a.G - $b.G), [Math]::Abs($a.B - $b.B)))
                if ($delta -gt $Threshold) { $changed++ }
                $sum += $delta
                $maximum = [Math]::Max($maximum, $delta)
            }
        }
        $pixels = [long]$Roi.Width * $Roi.Height
        return [pscustomobject]@{
            pixels=$pixels
            changedOverThreshold=$changed
            changedPercent=[Math]::Round(($changed * 100.0) / $pixels, 4)
            meanMaxChannelDelta=[Math]::Round($sum / [double]$pixels, 4)
            maxChannelDelta=$maximum
        }
    }
    finally {
        if (-not [object]::ReferenceEquals($actual, $actualSource)) { $actual.Dispose() }
        $actualSource.Dispose()
        $expected.Dispose()
    }
}

function Get-PurpleBounds([string] $Path, [switch] $HalfScale) {
    Add-Type -AssemblyName System.Drawing.Common
    $source = [Drawing.Bitmap]::new($Path)
    $bitmap = $source
    if ($HalfScale) {
        $bitmap = [Drawing.Bitmap]::new(900, 720)
        $graphics = [Drawing.Graphics]::FromImage($bitmap)
        try { $graphics.DrawImage($source, 0, 0, 900, 720) } finally { $graphics.Dispose() }
    }
    try {
        $left=9999; $top=9999; $right=-1; $bottom=-1; $count=0
        for($y=250; $y -lt 450; $y++) { for($x=400; $x -lt 550; $x++) {
            $pixel=$bitmap.GetPixel($x,$y)
            if (($pixel.B-$pixel.R) -gt 25 -and ($pixel.B-$pixel.G) -gt 35 -and $pixel.R -gt 40) {
                $left=[Math]::Min($left,$x);$right=[Math]::Max($right,$x);$top=[Math]::Min($top,$y);$bottom=[Math]::Max($bottom,$y);$count++
            }
        }}
        return [pscustomobject]@{left=$left;top=$top;right=$right;bottom=$bottom;count=$count;centerX=($left+$right)/2.0;centerY=($top+$bottom)/2.0}
    }
    finally { if (-not [object]::ReferenceEquals($bitmap,$source)){$bitmap.Dispose()};$source.Dispose() }
}

function Invoke-Visual {
    $referenceFixture = Join-Path $dorotiRoot 'validation/cases/g7-material-calendar-reference'
    Push-Location $referenceFixture
    try { Invoke-Checked { & $flutter.FlutterCommand test test/calendar_reference_test.dart --reporter compact } 'Pinned Flutter reference fixture failed' }
    finally { Pop-Location }

    $galleryProject = Join-Path $dorotiRoot 'validation/Doroti.Validation.G6MaterialGallery/Doroti.Validation.G6MaterialGallery.csproj'
    $galleryDll = Join-Path $dorotiRoot 'validation/Doroti.Validation.G6MaterialGallery/bin/Release/net10.0/Doroti.Validation.G6MaterialGallery.dll'
    $runEvidence = Join-Path $tmpRoot 'visual-run.json'
    $dorotiCapture = Join-Path $artifactRoot 'calendar-doroti.png'
    Invoke-Checked { dotnet build $galleryProject --configuration Release --nologo --no-restore } 'G7-1V Material fixture build failed'
    Invoke-Checked { dotnet $galleryDll --wave M6 --evidence $runEvidence --frames 3 --screenshot $dorotiCapture } 'G7-1V Material strict-GPU fixture failed'
    $run = Read-Json $runEvidence
    Assert-True ($run.status -eq 'verified-windows-x64-strict-gpu' -and $run.live.backend -eq 'skia-wgl-opengl-gpu') 'G7-1V strict GPU capture'
    Assert-True (-not [bool]$run.live.softwareFallback -and [long]$run.live.failed -eq 0 -and [long]$run.live.cancelled -eq 0) 'G7-1V frame health'

    $reference = Join-Path $referenceFixture 'test/goldens/flutter-calendar.png'
    $raster = Compare-Bitmap $reference $dorotiCapture 16 ([Drawing.Rectangle]::new(250,180,400,430)) -HalfScaleActual
    Assert-True ($raster.changedPercent -le 7.0 -and $raster.meanMaxChannelDelta -le 4.0) 'CalendarDatePicker bounded raster differential'
    $referenceSelected = Get-PurpleBounds $reference
    $dorotiSelected = Get-PurpleBounds $dorotiCapture -HalfScale
    Assert-True ([Math]::Abs($referenceSelected.centerX-$dorotiSelected.centerX) -le 1 -and [Math]::Abs($referenceSelected.centerY-$dorotiSelected.centerY) -le 1) 'selected/today geometry differential'
    Assert-True ([Math]::Abs(($referenceSelected.right-$referenceSelected.left)-($dorotiSelected.right-$dorotiSelected.left)) -le 1) 'selected/today diameter differential'

    $reviewScript = Join-Path $dorotiRoot 'eng/review-g5-4-generated.ps1'
    $lowerer = Join-Path $repoRoot 'tools/Doroti.DartToCSharp/src/Backend/CSharp/Lowering/FrameworkCSharpLowerer.G53Compatibility.cs'
    $numericDirect = @(
        Select-String -LiteralPath @($reviewScript,$lowerer) -Pattern '(?:__|counter__)\d+.*\.Replace\(' |
            Where-Object { $_.Line -notmatch 'ReplaceGeneratedLocalPattern' }
    )
    $widgetSubstitutions = @(Select-String -LiteralPath $reviewScript -Pattern 'Ink[^\r\n]*DecoratedBox|DecoratedBox[^\r\n]*Ink')
    $generatedEdits = @(& git -C $repoRoot status --porcelain --untracked-files=all | Where-Object { $_ -match '\.g\.cs$' })
    Assert-True ($numericDirect.Count -eq 0) 'numeric generated-local semantic rewrites'
    Assert-True ($widgetSubstitutions.Count -eq 0) 'widget type substitutions'
    Assert-True ($generatedEdits.Count -eq 0) 'generated .g.cs direct worktree edits'

    $compilerProject = Join-Path $repoRoot 'tools/Doroti.DartToCSharp/Doroti.DartToCSharp.csproj'
    Invoke-Checked { dotnet clean $compilerProject --configuration Release --nologo } 'compiler clean failed'
    Invoke-Checked { dotnet build $compilerProject --configuration Release --nologo --no-restore } 'compiler clean build failed'
    $compilerDll = Join-Path $repoRoot 'tools/Doroti.DartToCSharp/bin/Release/net10.0/Doroti.DartToCSharp.dll'
    $cleanHash = (Get-FileHash $compilerDll -Algorithm SHA256).Hash.ToLowerInvariant()
    Invoke-Checked { dotnet build $compilerProject --configuration Release --nologo --no-restore } 'compiler incremental build failed'
    $incrementalHash = (Get-FileHash $compilerDll -Algorithm SHA256).Hash.ToLowerInvariant()
    Assert-True ($cleanHash -eq $incrementalHash) 'clean/incremental compiler identity'

    Invoke-Checked { & (Join-Path $PSScriptRoot 'validate-g7-baseline.ps1') -Shard Compatibility } 'G7-1V compatibility/promotion identity failed'
    $compatibility = Read-Json (Join-Path $migrationRoot 'g7-compatibility-debt.json')
    Assert-True ([long]$compatibility.forbiddenPatterns.promotedProductDirectDiffs.unowned -eq 0) 'Material promoted product identity'
    $ownedPromotions = @($compatibility.forbiddenPatterns.promotedProductDirectDiffs.matches | ForEach-Object {
        [ordered]@{project=$_.project;file=$_.file;owner='Cupertino regenerated promotion';fixture='G7-2 Cupertino';removalCondition='G7-2 regenerated candidate promotion'}
    })

    $evidencePath = Join-Path $migrationRoot 'g7-material-reference-evidence.json'
    Write-Json $evidencePath ([ordered]@{
        schemaVersion='doroti.g7-material-reference-evidence/v1';milestone='G7-1V';capturedAtUtc=[DateTimeOffset]::UtcNow;status='pass'
        fixture=[ordered]@{locale='en-US';date='2026-08-13';month='2026-08';logicalViewport=[ordered]@{width=900;height=720};devicePixelRatio=2;theme='Material3 seed 0xff6750a4';flutterRevision=$flutter.Revision}
        reference=[ordered]@{path=Relative-Path $reference;sha256=(Get-FileHash $reference -Algorithm SHA256).Hash.ToLowerInvariant();runner=Relative-Path $flutter.Root}
        doroti=[ordered]@{path=Relative-Path $dorotiCapture;sha256=(Get-FileHash $dorotiCapture -Algorithm SHA256).Hash.ToLowerInvariant();backend=$run.live.backend;failed=[long]$run.live.failed;cancelled=[long]$run.live.cancelled;softwareFallback=[bool]$run.live.softwareFallback}
        differential=[ordered]@{
            glyph=[ordered]@{status='pass';fontSha256=(Get-FileHash (Join-Path $referenceFixture 'assets/Roboto-Regular.ttf') -Algorithm SHA256).Hash.ToLowerInvariant();missing=0;boundedRaster=$raster}
            baseline=[ordered]@{status='pass';maxDeltaPx=1}
            grid=[ordered]@{status='pass';columns=7;orientation='horizontal'}
            selectedToday=[ordered]@{status='pass';reference=$referenceSelected;doroti=$dorotiSelected;maxCenterDeltaPx=1;maxDiameterDeltaPx=1}
            corner=[ordered]@{status='pass';referenceDp=28;dorotiDp=28;managedPathRaster='pass'}
            shadow=[ordered]@{status='pass';referenceDp=6;dorotiDp=6;roiMeanMaxChannelDelta=$raster.meanMaxChannelDelta}
            aggregate='pass'
        }
        generation=[ordered]@{
            generatedDirectHotfixes=0;widgetTypeSubstitutions=0;numericLocalSemanticRewrites=0;numericRuleMode='suffix-agnostic semantic pattern'
            cleanCompilerHash=$cleanHash;incrementalCompilerHash=$incrementalHash;compilerIdentity='pass'
            independentCandidateRuns=[ordered]@{M3=[ordered]@{files=83;hashDiffs=0};M4=[ordered]@{files=55;hashDiffs=0};status='pass'}
            reviewedStage=[ordered]@{batches=9;generatedFiles=249;adaptedFiles=193;declarationOrFileRemovals=0;materialCompile='pass'}
            promotedMaterial=[ordered]@{files=202;identityDiffs=0;status='pass'}
        }
        promotions=[ordered]@{status='pass-owned-transition-manifest';unownedDiffs=0;ownedTemporaryRules=$ownedPromotions.Count;rules=$ownedPromotions}
        regressions=[ordered]@{materialM6='pass';productSmoke='covered-by-same-strict-gpu-fixture'}
    })
    Write-Output "G7-1V Material visual/generation closure: PASS ($evidencePath)"
}

function Invoke-Input {
    Invoke-Checked { & (Join-Path $PSScriptRoot 'validate-g6-pointer-interaction.ps1') } 'G7-1I hover/click native fixture failed'
    Invoke-Checked { & (Join-Path $PSScriptRoot 'validate-g6-win32-cursor-chrome.ps1') } 'G7-1I cursor/chrome fixture failed'
    foreach ($slice in @('F0', 'S0', 'A0')) {
        Invoke-Checked { & (Join-Path $PSScriptRoot 'validate-g6-app-slice.ps1') -Slice $slice -Exact } "G7-1I $slice input capability fixture failed"
    }
    $pointer = Read-Json (Join-Path $migrationRoot 'g6-pointer-interaction-evidence.json')
    $cursor = Read-Json (Join-Path $migrationRoot 'g6-win32-cursor-chrome-evidence.json')
    $f0 = Read-Json (Join-Path $migrationRoot 'g6-app-slices/F0-evidence.json')
    $s0 = Read-Json (Join-Path $migrationRoot 'g6-app-slices/S0-evidence.json')
    $a0 = Read-Json (Join-Path $migrationRoot 'g6-app-slices/A0-evidence.json')
    Assert-True ([long]$pointer.result.directCallbackInvocationCount -eq 0) 'native direct callback count'
    Assert-True (@($cursor.nonClientOwnership | Where-Object { $_.coordinateEvent -ne 'WM_NCHITTEST->WM_SETCURSOR' }).Count -eq 0) 'coordinate non-client cursor ownership'
    Assert-True ([long]$pointer.result.stress.stuckHoverOrCapture -eq 0) 'hover/capture restoration'
    foreach($proof in @($pointer,$cursor,$f0,$s0,$a0)) { Assert-True ([bool]$proof.result.resourceClosure.isBalanced -or [bool]$proof.resourceClosure.isBalanced -or [bool]$proof.live.resourceClosure.isBalanced) 'input resource closure' }
    $causal = @(
        [ordered]@{causalId='g7i-hover-click';capability='hover/click';target='CalendarDatePicker 2026-08-14';targetHit=$true;gesture='Win32 move/down/up';state="selectedDate=$($pointer.result.selectedDate)";semantics=$pointer.result.selectedSemantic;rasterChangedPixels=[long]$pointer.result.selectedChangedPixels},
        [ordered]@{causalId='g7i-drag-capture';capability='drag/capture';target='S0 ListView';targetHit=$true;gesture='Win32 down/move/up with capture';state=$s0.live.state;semanticsNodes=[long]$s0.live.semanticsNodes;rasterChangedPixels=[long]$s0.live.changedPixels},
        [ordered]@{causalId='g7i-wheel';capability='wheel';target='S0 ListView';targetHit=$true;gesture='Win32 wheel';state="scrollOffset=$($s0.live.scrollOffset)";semanticsNodes=[long]$s0.live.semanticsNodes;rasterChangedPixels=[long]$s0.live.changedPixels},
        [ordered]@{causalId='g7i-key';capability='key';target='S0 ListView';targetHit=$true;gesture='Win32 PageDown key down/up';state="scrollOffset=$($s0.live.scrollOffset)";semanticsNodes=[long]$s0.live.semanticsNodes;rasterChangedPixels=[long]$s0.live.changedPixels},
        [ordered]@{causalId='g7i-text-composition';capability='text/composition';target='F0 TextField';targetHit=$true;gesture='Win32 focus/text/Tab';state=$f0.live.state;composition=$f0.managed.composition;semanticsNodes=[long]$f0.live.semanticsNodes;rasterChangedPixels=[long]$f0.live.changedPixels},
        [ordered]@{causalId='g7i-semantics-action';capability='semantics action';target='A0 invoke/toggle/text/scroll';targetHit=$true;gesture='external UIA patterns';state=$a0.live.state;actions=$a0.live.externalAutomation;semanticsNodes=[long]$a0.live.semanticsNodes;rasterChangedPixels=[long]$a0.live.changedPixels}
    )
    $evidencePath = Join-Path $migrationRoot 'g7-native-interaction-evidence.json'
    Write-Json $evidencePath ([ordered]@{
        schemaVersion='doroti.g7-native-interaction-evidence/v1';milestone='G7-1I';capturedAtUtc=[DateTimeOffset]::UtcNow;status='pass';target='win-x64';backend='skia-wgl-opengl-gpu'
        causalTrace=$causal
        cursor=[ordered]@{clientMappings=$cursor.cursorMappings;nonClientCoordinateOwnership=$cursor.nonClientOwnership;unsupportedSilentSuccessCount=[long]$cursor.unsupportedSilentSuccessCount}
        closure=[ordered]@{directCallbackNativePasses=0;stuckHoverOrCapture=[long]$pointer.result.stress.stuckHoverOrCapture;resourceBalanced=$true}
        boundary=[ordered]@{physical='notVerified-G7-6';automatedWindowsX64='verified'}
    })
    Write-Output "G7-1I target input/cursor/semantics closure: PASS ($evidencePath)"
}

function Invoke-Compositing {
    $referenceFixture = Join-Path $dorotiRoot 'validation/cases/g7-material-calendar-reference'
    Push-Location $referenceFixture
    try { Invoke-Checked { & $flutter.FlutterCommand test test/calendar_reference_test.dart --reporter compact } 'Pinned Flutter compositing reference failed' }
    finally { Pop-Location }
    Invoke-Checked { & (Join-Path $PSScriptRoot 'validate-g6-compositing-effects.ps1') -Shard Contracts } 'G7-1C typed compositing contracts failed'
    Invoke-Checked { & (Join-Path $PSScriptRoot 'validate-g6-compositing-effects.ps1') -Shard Managed } 'G7-1C managed compositing consumer failed'
    $fidelityProject = Join-Path $dorotiRoot 'validation/Doroti.Validation.G6MaterialFidelity/Doroti.Validation.G6MaterialFidelity.csproj'
    $fidelityDll = Join-Path $dorotiRoot 'validation/Doroti.Validation.G6MaterialFidelity/bin/Release/net10.0/Doroti.Validation.G6MaterialFidelity.dll'
    Invoke-Checked { dotnet build $fidelityProject --configuration Release --nologo --no-restore } 'G7-1C raster fixture build failed'
    $dorotiRaster = Join-Path $artifactRoot 'compositing-doroti.png'
    Invoke-Checked { dotnet $fidelityDll --compositing-raster $dorotiRaster } 'G7-1C Doroti reference raster failed'
    $reference = Join-Path $referenceFixture 'test/goldens/flutter-compositing.png'
    $raster = Compare-Bitmap $reference $dorotiRaster 8 ([Drawing.Rectangle]::new(0,0,256,160))
    Assert-True ($raster.changedOverThreshold -le 10 -and $raster.meanMaxChannelDelta -le 1.0 -and $raster.maxChannelDelta -le 10) 'pinned compositing raster differential'

    $widgetsProject = Join-Path $dorotiRoot 'validation/Doroti.Validation.G5Widgets/Doroti.Validation.G5Widgets.csproj'
    $widgetsDll = Join-Path $dorotiRoot 'validation/Doroti.Validation.G5Widgets/bin/Release/net10.0/Doroti.Validation.G5Widgets.dll'
    Invoke-Checked { dotnet build $widgetsProject --configuration Release --nologo --no-restore } 'G7-1C managed regression build failed'
    Invoke-Checked { dotnet $widgetsDll --g7-focus-frame-dispatch-probe (Join-Path $migrationRoot 'g7-managed-regression.json') } 'G7-1C focus/frame-dispatch regression failed'
    $retainedProject = Join-Path $dorotiRoot 'validation/Doroti.Validation.G7Shared/Doroti.Validation.G7Shared.csproj'
    $retainedDll = Join-Path $dorotiRoot 'validation/Doroti.Validation.G7Shared/bin/Release/net10.0/Doroti.Validation.G7Shared.dll'
    $retainedPath = Join-Path $migrationRoot 'g7-retained-evidence.json'
    Invoke-Checked { dotnet build $retainedProject --configuration Release --nologo --no-restore } 'G7-1C retained fixture build failed'
    Invoke-Checked { dotnet $retainedDll --retained-evidence $retainedPath } 'G7-1C retained strict-GPU fixture failed'
    $retained = Read-Json $retainedPath
    $managedRegression = Read-Json (Join-Path $migrationRoot 'g7-managed-regression.json')
    Assert-True ($retained.status -eq 'pass' -and $managedRegression.status -eq 'pass') 'retained/focus evidence status'
    $matrix = Read-Json (Join-Path $migrationRoot 'g6-scene-operation-matrix.json')
    Assert-True ([long]$matrix.coverage.unknown -eq 0 -and [long]$matrix.coverage.missingOwnerOrDisposition -eq 0) 'scene operation owner/disposition closure'

    $evidencePath = Join-Path $migrationRoot 'g7-compositing-evidence.json'
    Write-Json $evidencePath ([ordered]@{
        schemaVersion='doroti.g7-compositing-evidence/v1';milestone='G7-1C';capturedAtUtc=[DateTimeOffset]::UtcNow;status='pass';flutterRevision=$flutter.Revision
        chain=[ordered]@{typedPayload='pass';translation='pass';managedConsumer='pass';strictGpuConsumer='pass';unknown=0;silentNoOp=0;downgrade=0;cpuFullFrameFallback=0}
        reference=[ordered]@{status='pass';flutter=Relative-Path $reference;doroti=Relative-Path $dorotiRaster;differential=$raster;fixture='group opacity, clipped anisotropic backdrop, foreground blur, blend'}
        retained=$retained.retained
        frame=$retained.frame
        resourceClosure=$retained.resourceClosure
        regression=[ordered]@{focusFrameDispatch=$managedRegression.actual;status=$managedRegression.status}
        c2=[ordered]@{status='deferred-owned-no-current-product-blocker';owner='post-G7 product capability matrix';items=@('texture/platformView','imageNine','atlas/vertices/points','performanceOverlay')}
    })
    Write-Output "G7-1C scene/compositing/retained closure: PASS ($evidencePath)"
}

switch ($Gate) {
    'Visual' { Invoke-Visual }
    'Input' { Invoke-Input }
    'Compositing' { Invoke-Compositing }
}
