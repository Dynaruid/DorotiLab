#Requires -Version 7.0
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$dorotiRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $dorotiRoot '..'))
$flutterRoot = Join-Path $repositoryRoot 'reference/flutter-master'
$manifestPath = Join-Path $dorotiRoot 'validation/evidence/flutter-conformance/framework-shader-manifest.json'
$contractProject = Join-Path $dorotiRoot 'validation/runtime-shader-contract/Doroti.Validation.RuntimeShaderContract.csproj'

function Assert-True([bool] $Condition, [string] $Name) {
    if (-not $Condition) { throw "$Name failed." }
}

function Resolve-RepositoryPath([string] $RelativePath) {
    [IO.Path]::GetFullPath((Join-Path $repositoryRoot ($RelativePath.Replace('/', '\'))))
}

function Resolve-FlutterPath([string] $RelativePath) {
    [IO.Path]::GetFullPath((Join-Path $flutterRoot ($RelativePath.Replace('/', '\'))))
}

function Read-Json([string] $Path) {
    Assert-True (Test-Path -LiteralPath $Path -PathType Leaf) "JSON artifact exists: $Path"
    Get-Content -Raw -LiteralPath $Path | ConvertFrom-Json
}

function Assert-Hash([string] $Path, [string] $Expected, [string] $Name) {
    Assert-True (Test-Path -LiteralPath $Path -PathType Leaf) "$Name exists"
    $actual = (Get-FileHash -Algorithm SHA256 -LiteralPath $Path).Hash.ToLowerInvariant()
    Assert-True ($actual -eq $Expected.ToLowerInvariant()) "$Name SHA-256: expected $Expected, got $actual"
}

Assert-True (Test-Path -LiteralPath $flutterRoot -PathType Container) 'pinned Flutter checkout exists'
$manifest = Read-Json $manifestPath
Assert-True ($manifest.schemaVersion -eq 'doroti.framework-shader-manifest/v1') 'framework shader manifest schema'
$revision = (& git -C $flutterRoot rev-parse HEAD).Trim()
Assert-True ($LASTEXITCODE -eq 0) 'Flutter revision lookup'
Assert-True ($revision -eq [string]$manifest.flutterRevision) "Flutter revision pin: expected $($manifest.flutterRevision), got $revision"

$assets = @($manifest.assets)
Assert-True ($assets.Count -eq 2) 'selected framework shader asset count'
Assert-True (($assets.id | Sort-Object -Unique).Count -eq $assets.Count) 'framework shader asset ids are unique'

foreach ($asset in $assets) {
    foreach ($property in @('id', 'flutterAssetKey', 'flutterSourcePath', 'flutterSourceSha256',
            'adaptedSourcePath', 'adaptedSourceSha256', 'owningAssembly', 'embeddedResourceName',
            'uniforms', 'samplers', 'license', 'targetSupport')) {
        Assert-True ($null -ne $asset.PSObject.Properties[$property]) "manifest field: $($asset.id) -> $property"
    }
    Assert-True (@($asset.targetSupport).Count -gt 0) "target support: $($asset.id)"
    Assert-True ([string]$asset.license -eq 'BSD-3-Clause') "Flutter license: $($asset.id)"

    $flutterSource = Resolve-FlutterPath ([string]$asset.flutterSourcePath)
    $adaptedSource = Resolve-RepositoryPath ([string]$asset.adaptedSourcePath)
    Assert-Hash $flutterSource ([string]$asset.flutterSourceSha256) "Flutter source: $($asset.id)"
    Assert-Hash $adaptedSource ([string]$asset.adaptedSourceSha256) "Doroti adapted source: $($asset.id)"

    $project = Join-Path $dorotiRoot "src/$($asset.owningAssembly)/$($asset.owningAssembly).csproj"
    Assert-True (Test-Path -LiteralPath $project -PathType Leaf) "owning project: $($asset.id)"
    $projectText = Get-Content -Raw -LiteralPath $project
    $expectedResource = 'LogicalName="' + [string]$asset.embeddedResourceName + '"'
    Assert-True ($projectText.Contains($expectedResource)) "embedded resource manifest: $($asset.id)"
    Assert-True (@($asset.uniforms).Count -gt 0) "uniform ABI: $($asset.id)"
}

$stretchPort = Join-Path $dorotiRoot 'src/Doroti.Framework.Widgets/stretch_effect.cs'
$stretchText = Get-Content -Raw -LiteralPath $stretchPort
Assert-True (-not $stretchText.Contains('private const string _source')) 'StretchEffect has no inline shader fork'
Assert-True ($stretchText.Contains('widgets.stretch-effect')) 'StretchEffect uses the common framework loader'

$inkPort = Join-Path $dorotiRoot 'src/Doroti.Framework.Material/ink_sparkle.cs'
$inkText = Get-Content -Raw -LiteralPath $inkPort
Assert-True (-not $inkText.Contains('GetManifestResourceStream')) 'InkSparkle has no private resource loader'
Assert-True ($inkText.Contains('material.ink-sparkle')) 'InkSparkle uses the common framework loader'

Push-Location $repositoryRoot
try {
    & dotnet run --project $contractProject -c Release --nologo
    if ($LASTEXITCODE -ne 0) { throw "FCR-1 runtime shader contract failed (exit code $LASTEXITCODE)." }
}
finally {
    Pop-Location
}

Write-Output 'Doroti FCR-1 framework shader validation: PASS'
