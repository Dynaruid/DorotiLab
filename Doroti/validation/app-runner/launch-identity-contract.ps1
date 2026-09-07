param([string] $EvidenceDirectory = '.doroti/evidence/boot/launch-identity-contract')
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot '../../eng/launch-identity.ps1')
$root = [IO.Path]::GetFullPath($EvidenceDirectory)
if (Test-Path -LiteralPath $root) { throw 'Use a fresh evidence directory.' }
$appRoot = Join-Path $root 'app'
New-Item -ItemType Directory -Path $appRoot | Out-Null
$font = Join-Path $appRoot 'font.ttf'
[IO.File]::WriteAllBytes($font, [byte[]](1,2,3,4))
$props = Join-Path $root 'Directory.Build.props'
Set-Content $props '<Project />'
function Fingerprint { Get-DorotiContentFingerprint (@(Get-DorotiInputFiles @($appRoot)) + @(Get-DorotiInheritedInputs @($appRoot))) 'Release|test-rid' }
$initial = Fingerprint
$mtime = (Get-Item $font).LastWriteTimeUtc
[IO.File]::WriteAllBytes($font, [byte[]](4,3,2,1))
(Get-Item $font).LastWriteTimeUtc = $mtime
if ((Fingerprint) -eq $initial) { throw 'Same-size/mtime binary edit escaped fingerprint.' }
$afterBinary = Fingerprint
Set-Content $props '<Project><PropertyGroup /></Project>'
if ((Fingerprint) -eq $afterBinary) { throw 'Inherited props edit escaped fingerprint.' }
$afterProps = Fingerprint
New-Item -ItemType Directory (Join-Path $appRoot 'node_modules/deep') | Out-Null
Set-Content (Join-Path $appRoot 'node_modules/deep/ignored.js') 'ignored'
if ((Fingerprint) -ne $afterProps) { throw 'Dependency directory was traversed.' }
Set-Content (Join-Path $appRoot 'package-lock.json') '{"version":1}'
if ((Fingerprint) -eq $afterProps) { throw 'Dependency lock edit escaped fingerprint.' }
$expected = [ordered]@{ outputRoot=$root; targetPath=$font; rid='test'; framework='net10.0'; files=@([ordered]@{path=$font;length=4;sha256='original'}) }
Assert-DorotiArtifactIdentity $expected ($expected | ConvertTo-Json -Depth 10 | ConvertFrom-Json)
foreach ($mutation in @('missing','tampered','rid','framework','extra')) {
    $actual = $expected | ConvertTo-Json -Depth 10 | ConvertFrom-Json
    switch ($mutation) {
        missing { $actual.files = @() }
        tampered { $actual.files[0].sha256 = 'modified' }
        rid { $actual.rid = 'other' }
        framework { $actual.framework = 'other' }
        extra { $actual.files += $actual.files[0] }
    }
    $rejected = $false
    try { Assert-DorotiArtifactIdentity $expected $actual } catch { $rejected = $true }
    if (!$rejected) { throw "Artifact mutation accepted: $mutation" }
}
[ordered]@{status='PASS'; binarySameSizeMtime=$true; inheritedProps=$true; dependencyPruning=$true; lockIncluded=$true; artifactMutationsRejected=5} | ConvertTo-Json | Tee-Object -FilePath (Join-Path $root 'result.json')
