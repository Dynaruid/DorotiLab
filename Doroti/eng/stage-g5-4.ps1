#Requires -Version 5.1
param(
    [string] $OutputRoot
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
if ([string]::IsNullOrWhiteSpace($OutputRoot)) {
    $OutputRoot = Join-Path $dorotiRoot 'migration/generated-candidates/g5-4-reviewed'
}
elseif (-not [IO.Path]::IsPathRooted($OutputRoot)) {
    $OutputRoot = Join-Path $dorotiRoot $OutputRoot
}
$OutputRoot = [IO.Path]::GetFullPath($OutputRoot)
$candidateBase = [IO.Path]::GetFullPath((Join-Path $dorotiRoot 'migration/generated-candidates'))
if (-not $OutputRoot.StartsWith($candidateBase, [StringComparison]::OrdinalIgnoreCase)) {
    throw "G5-4 staging output must remain under $candidateBase."
}
if (Test-Path -LiteralPath $OutputRoot) {
    [IO.Directory]::Delete($OutputRoot, $true)
}
[IO.Directory]::CreateDirectory($OutputRoot) | Out-Null

$templateRoots = [ordered]@{
    Material = Join-Path $candidateBase 'g5-4-m1-theme'
    Cupertino = Join-Path $candidateBase 'g5-4-c0-theme'
    WidgetPreviews = Join-Path $candidateBase 'g5-4-p0-widget-previews'
}
foreach ($name in @('Directory.Build.props', 'Directory.Packages.props')) {
    Copy-Item -LiteralPath (Join-Path $templateRoots.Material $name) -Destination (Join-Path $OutputRoot $name)
}
$solution = @'
<Solution>
  <Project Path="projects/Cupertino/Doroti.Generated.Framework.G54.Cupertino.csproj" />
  <Project Path="projects/Material/Doroti.Generated.Framework.G54.Material.csproj" />
  <Project Path="projects/WidgetPreviews/Doroti.Generated.Framework.G54.WidgetPreviews.csproj" />
</Solution>
'@
[IO.File]::WriteAllText((Join-Path $OutputRoot 'Doroti.Generated.Framework.slnx'), $solution.Replace("`r`n", "`n"), [Text.UTF8Encoding]::new($false))
$buildPropsPath = Join-Path $OutputRoot 'Directory.Build.props'
$buildProps = [IO.File]::ReadAllText($buildPropsPath)
$buildProps = $buildProps.Replace(
    '</PropertyGroup>',
    "  <NoWarn>`$(NoWarn);CS0219;CS8524;CS8846</NoWarn>`n" +
    "    <DorotiRepositoryRoot`n" +
    "      Condition=`"'`$(DorotiRepositoryRoot)' == '' and Exists('`$(MSBuildThisFileDirectory)..\..\..\src\Doroti.Runtime\Doroti.Runtime.csproj')`">" +
    "`$([System.IO.Path]::GetFullPath('`$(MSBuildThisFileDirectory)..\..\..'))</DorotiRepositoryRoot>`n" +
    '  </PropertyGroup>')
[IO.File]::WriteAllText($buildPropsPath, $buildProps, [Text.UTF8Encoding]::new($false))
foreach ($partition in @('Material', 'Cupertino', 'WidgetPreviews')) {
    $sourceDirectory = Join-Path $templateRoots[$partition] "projects/$partition"
    $sourceProjects = @(Get-ChildItem -LiteralPath $sourceDirectory -File -Filter '*.csproj' -ErrorAction Stop)
    if ($sourceProjects.Count -ne 1) { throw "Expected one batch project template for $partition, got $($sourceProjects.Count)." }
    $sourceProject = $sourceProjects[0]
    $targetProject = Join-Path $OutputRoot "projects/$partition"
    [IO.Directory]::CreateDirectory($targetProject) | Out-Null
    $projectText = [IO.File]::ReadAllText($sourceProject.FullName).Replace("`r`n", "`n")
    $projectText = [Text.RegularExpressions.Regex]::Replace(
        $projectText,
        '<AssemblyName>[^<]+</AssemblyName>',
        "<AssemblyName>Doroti.Generated.Framework.G54.$partition</AssemblyName>")
    $packageId = "Doroti.Framework.$partition"
    $projectText = $projectText.Replace(
        "<AssemblyName>Doroti.Generated.Framework.G54.$partition</AssemblyName>",
        "<AssemblyName>Doroti.Generated.Framework.G54.$partition</AssemblyName>`n    <PackageId>$packageId</PackageId>`n    <Version>0.2.0-beta</Version>`n    <IsPackable>true</IsPackable>")
    if ($partition -eq 'Material') {
        $projectText = $projectText.Replace(
            "`n  </ItemGroup>",
            "    <ProjectReference Include=`"..\Cupertino\Doroti.Generated.Framework.G54.Cupertino.csproj`" />`n`n  </ItemGroup>")
    }
    $projectPath = Join-Path $targetProject "Doroti.Generated.Framework.G54.$partition.csproj"
    [IO.File]::WriteAllText($projectPath, $projectText.Replace("`r`n", "`n"), [Text.UTF8Encoding]::new($false))
    Get-ChildItem -LiteralPath $sourceProject.DirectoryName -File -Filter '*.GlobalUsings.g.cs' |
        Copy-Item -Destination $targetProject
    if ($partition -eq 'Material') {
        [IO.File]::WriteAllText(
            (Join-Path $targetProject 'ProjectReferences.GlobalUsings.g.cs'),
            "global using Doroti.Generated.Framework.Cupertino;`n",
            [Text.UTF8Encoding]::new($false))
    }
}

$index = Get-Content -LiteralPath (Join-Path $dorotiRoot 'migration/flutter-framework/g5-4-batches.json') -Raw | ConvertFrom-Json
$written = [Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
$batchEvidence = [Collections.Generic.List[object]]::new()
foreach ($batch in @($index.batches)) {
    $candidate = Join-Path $candidateBase ("g5-4-" + ([string]$batch.id).ToLowerInvariant())
    $reportPath = Join-Path $candidate 'converter-report.json'
    $coveragePath = Join-Path $candidate 'framework-coverage.json'
    if (-not (Test-Path -LiteralPath $reportPath) -or -not (Test-Path -LiteralPath $coveragePath)) {
        throw "G5-4 batch candidate is missing: $($batch.id)"
    }
    $report = Get-Content -LiteralPath $reportPath -Raw | ConvertFrom-Json
    $coverage = Get-Content -LiteralPath $coveragePath -Raw | ConvertFrom-Json
    $errors = @($report.diagnostics | Where-Object severity -eq 'error')
    if ($errors.Count -ne 0 -or $coverage.unclassifiedAstNodeCount -ne 0 -or $coverage.silentOmissionCount -ne 0) {
        throw "G5-4 batch is not promotable: $($batch.id) errors=$($errors.Count) unclassified=$($coverage.unclassifiedAstNodeCount) omissions=$($coverage.silentOmissionCount)."
    }
    foreach ($output in @($report.outputs)) {
        $relative = ([string]$output.output).Replace('/', [IO.Path]::DirectorySeparatorChar)
        if (-not $relative.EndsWith('.g.cs', [StringComparison]::Ordinal)) { continue }
        $source = Join-Path $candidate $relative
        $target = Join-Path $OutputRoot $relative
        if (-not $written.Add($relative)) { throw "Duplicate G5-4 staged output: $relative" }
        [IO.Directory]::CreateDirectory((Split-Path $target -Parent)) | Out-Null
        Copy-Item -LiteralPath $source -Destination $target
    }
    $batchEvidence.Add([ordered]@{
        id = [string]$batch.id
        compilerIdentity = $report.identity
        outputs = @($report.outputs).Count
        diagnostics = @($report.diagnostics).Count
        errors = $errors.Count
        unclassified = [int]$coverage.unclassifiedAstNodeCount
        silentOmissions = [int]$coverage.silentOmissionCount
    })
}

if ($written.Count -ne 249) {
    throw "G5-4 staged output drifted: expected 249 files, got $($written.Count)."
}
& (Join-Path $PSScriptRoot 'review-g5-4-generated.ps1') -OutputRoot $OutputRoot
$stageReport = [ordered]@{
    schemaVersion = 'doroti.g5-4-staged-candidate/v1'
    milestone = 'G5-4'
    generatedFiles = $written.Count
    batches = @($batchEvidence)
}
$stageJson = ($stageReport | ConvertTo-Json -Depth 16) -replace "`r`n", "`n"
[IO.File]::WriteAllText((Join-Path $OutputRoot 'g5-4-stage.json'), $stageJson + "`n", [Text.UTF8Encoding]::new($false))
Write-Output "G5-4 staged candidate: PASS ($($index.counts.batches) batches, $($written.Count) generated files)"
Write-Output "Stage: $OutputRoot"
