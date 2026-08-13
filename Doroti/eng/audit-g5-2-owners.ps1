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
    legacyCompatibility = [ordered]@{
        productCompile = $false
        removalMilestone = 'G5-3'
        projects = @('Doroti.Legacy.Rendering', 'Doroti.Legacy.Engine', 'Doroti.Widgets')
        reason = 'Historical C# Widget/render behavior remains available only to root regression and comparison projects.'
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
