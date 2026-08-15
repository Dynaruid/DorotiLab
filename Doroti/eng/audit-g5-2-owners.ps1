#Requires -Version 5.1
param([string] $OutputPath)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
if ([string]::IsNullOrWhiteSpace($OutputPath)) {
    $OutputPath = Join-Path $dorotiRoot 'migration/flutter-framework/g5-2-owner-audit.json'
}
elseif (-not [IO.Path]::IsPathRooted($OutputPath)) {
    $OutputPath = Join-Path $dorotiRoot $OutputPath
}
$OutputPath = [IO.Path]::GetFullPath($OutputPath)
$solutionPath = Join-Path $dorotiRoot 'Doroti.Product.slnx'
[xml] $solution = Get-Content -LiteralPath $solutionPath -Raw
$projectPaths = @($solution.SelectNodes('//Project') | ForEach-Object {
    [IO.Path]::GetFullPath((Join-Path $dorotiRoot $_.Path))
})
$forbiddenProductProjects = @('Doroti.Legacy.Rendering', 'Doroti.Legacy.Engine', 'Doroti.Widgets', 'Doroti\Doroti.csproj')
$legacyPaths = @(
    'src/Doroti.Legacy.Rendering/Doroti.Legacy.Rendering.csproj',
    'src/Doroti.Legacy.Engine/Doroti.Legacy.Engine.csproj',
    'src/Doroti.Widgets/Doroti.Widgets.csproj',
    'src/Doroti/Doroti.csproj',
    'src/Doroti/DorotiApp.cs',
    'src/Doroti.Engine/EngineContracts.cs',
    'src/Doroti.Engine/InteractiveApplication.cs',
    'src/Doroti.Engine/ManagedBgraRenderSurface.cs',
    'src/Doroti.Rendering/BoxConstraints.cs',
    'src/Doroti.Rendering/PaintingContext.cs',
    'src/Doroti.Rendering/RenderBoxes.cs',
    'src/Doroti.Rendering/RenderTree.cs',
    'src/Doroti.Rendering/Semantics.cs',
    'src/Doroti.Host.Avalonia/Doroti.Host.Avalonia.csproj',
    'samples/AvaloniaHostCounter/AvaloniaHostCounter.csproj',
    'samples/ShellHostComparison/ShellHostComparison.csproj',
    'tools/Doroti.BehaviorRunner/Doroti.BehaviorRunner.csproj',
    'tools/Doroti.SceneLab/Doroti.SceneLab.csproj',
    'templates/Doroti.Templates/Doroti.Templates.csproj'
)
$remainingLegacyPaths = @($legacyPaths | Where-Object { Test-Path -LiteralPath (Join-Path $dorotiRoot $_) })
$forbiddenReferences = @()
$compileInputs = [ordered]@{}

foreach ($projectPath in $projectPaths) {
    $raw = & dotnet msbuild $projectPath -getItem:Compile -getItem:ProjectReference -nologo | Out-String
    if ($LASTEXITCODE -ne 0) { throw "MSBuild item audit failed for $projectPath." }
    $items = $raw | ConvertFrom-Json
    $name = [IO.Path]::GetFileNameWithoutExtension($projectPath)
    $compileInputs[$name] = @($items.Items.Compile | ForEach-Object { $_.FullPath })
    foreach ($reference in @($items.Items.ProjectReference)) {
        $fullPath = [string]$reference.FullPath
        if ($forbiddenProductProjects | Where-Object { $fullPath -like "*$_*" }) {
            $forbiddenReferences += [ordered]@{ project = $name; reference = $fullPath }
        }
    }
}

$renderingProject = 'Doroti.Rendering'
$renderingFiles = @($compileInputs[$renderingProject] | ForEach-Object { [IO.Path]::GetFileName($_) } | Sort-Object)
$allowedRenderingFiles = @('DisplayList.cs', 'FrameContracts.cs', 'LayerTree.cs', 'RenderingContracts.cs', 'SceneCommitter.cs')
$renderingOwnerInputs = @($renderingFiles | Where-Object { $_ -notin $allowedRenderingFiles })
$engineFiles = @($compileInputs['Doroti.Engine'] | ForEach-Object { [IO.Path]::GetFileName($_) } | Sort-Object)
$forbiddenEngineInputs = @($engineFiles | Where-Object { $_ -in @('EngineContracts.cs', 'InteractiveApplication.cs', 'ManagedBgraRenderSurface.cs') })
$generatedSources = @('Painting', 'Rendering', 'Semantics') | ForEach-Object {
    Get-ChildItem -LiteralPath (Join-Path $dorotiRoot "src/Doroti.Flutter.Framework.$_") -Filter '*.g.cs' -File -ErrorAction SilentlyContinue
}
$productSolutionText = Get-Content -LiteralPath $solutionPath -Raw
$forbiddenSolutionEntries = @($forbiddenProductProjects | Where-Object { $productSolutionText -like "*$_*" })
$success = $renderingOwnerInputs.Count -eq 0 -and
    $forbiddenEngineInputs.Count -eq 0 -and
    $forbiddenReferences.Count -eq 0 -and
    $forbiddenSolutionEntries.Count -eq 0 -and
    $remainingLegacyPaths.Count -eq 0 -and
    @($generatedSources).Count -eq 0

$evidence = [ordered]@{
    schemaVersion = 'doroti.g5-2-owner-audit/v1'
    capturedAtUtc = [DateTime]::UtcNow.ToString('O')
    productSolution = 'Doroti.Product.slnx'
    success = $success
    productProjectCount = $projectPaths.Count
    renderingBoundaryCompileInputs = $renderingFiles
    duplicateHandwrittenRenderingOwnerInputs = $renderingOwnerInputs
    forbiddenEngineInputs = $forbiddenEngineInputs
    forbiddenProductReferences = $forbiddenReferences
    forbiddenProductSolutionEntries = $forbiddenSolutionEntries
    generatedProductSources = @($generatedSources | ForEach-Object { $_.FullName })
    legacyRemoval = [ordered]@{
        completed = ($remainingLegacyPaths.Count -eq 0)
        removalMilestone = 'G5-3'
        removedPaths = $legacyPaths
        remainingPaths = $remainingLegacyPaths
        reason = 'Reviewed framework and host packages replaced the handwritten Widget/render compatibility island.'
    }
}
[IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($OutputPath)) | Out-Null
$temporary = $OutputPath + '.tmp-' + [Guid]::NewGuid().ToString('N')
[IO.File]::WriteAllText($temporary, (($evidence | ConvertTo-Json -Depth 12) -replace "`r`n", "`n") + "`n", [Text.UTF8Encoding]::new($false))
if (Test-Path -LiteralPath $OutputPath) {
    $backup = $OutputPath + '.bak-' + [Guid]::NewGuid().ToString('N')
    [IO.File]::Replace($temporary, $OutputPath, $backup, $true)
    Remove-Item -LiteralPath $backup -Force
}
else {
    [IO.File]::Move($temporary, $OutputPath)
}
if (-not $success) { throw "G5-2 owner audit failed. See $OutputPath." }
Write-Output "G5-2 product owner audit: PASS ($($projectPaths.Count) product projects)"
