# Content identities for safe -NoBuild reuse. Dot-sourced by doroti.ps1 and contract tests.
function Get-DorotiInputFiles([string[]] $Roots) {
    $excluded = @('bin', 'obj', '.doroti', 'artifacts', '.git', '.gradle', '.idea', 'node_modules', '.vs')
    $pending = [Collections.Generic.Stack[string]]::new()
    foreach ($root in $Roots) { if (Test-Path -LiteralPath $root -PathType Container) { $pending.Push($root) } }
    while ($pending.Count) {
        foreach ($entry in Get-ChildItem -LiteralPath $pending.Pop() -Force) {
            if ($entry.PSIsContainer -and $entry.Name -in $excluded) { continue }
            if ($entry.Attributes -band [IO.FileAttributes]::ReparsePoint) {
                throw "Cannot safely fingerprint a linked build input: $($entry.FullName)"
            }
            if ($entry.PSIsContainer) {
                if ($entry.Name -notin $excluded) { $pending.Push($entry.FullName) }
            } else { $entry }
        }
    }
}

function Get-DorotiInheritedInputs([string[]] $Roots) {
    $names = @('Directory.Build.props','Directory.Build.targets','Directory.Packages.props','global.json','NuGet.Config','nuget.config')
    foreach ($root in $Roots) {
        $directory = [IO.DirectoryInfo]::new([IO.Path]::GetFullPath($root))
        while ($null -ne $directory) {
            foreach ($name in $names) {
                $path = Join-Path $directory.FullName $name
                if (Test-Path -LiteralPath $path -PathType Leaf) { Get-Item -LiteralPath $path }
            }
            $directory = $directory.Parent
        }
    }
}

function Get-DorotiContentFingerprint([object[]] $Files, [string] $Selection) {
    $hash = [Security.Cryptography.IncrementalHash]::CreateHash([Security.Cryptography.HashAlgorithmName]::SHA256)
    try {
        foreach ($file in @($Files | Sort-Object FullName -Unique)) {
            # Fixed-size content digests and explicit lengths prevent ambiguous concatenation.
            $digest = (Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash
            $hash.AppendData([Text.Encoding]::UTF8.GetBytes("$($file.FullName)|$($file.Length)|$digest`n"))
        }
        $hash.AppendData([Text.Encoding]::UTF8.GetBytes($Selection))
        [Convert]::ToHexString($hash.GetHashAndReset()).ToLowerInvariant()
    } finally { $hash.Dispose() }
}

function Get-DorotiEvaluatedInputFiles([string[]] $Projects, [string] $Configuration, [string] $Rid, [string] $WorkingDirectory) {
    $collector = Join-Path $PSScriptRoot 'launch-inputs.targets'
    Push-Location $WorkingDirectory
    try {
        foreach ($project in $Projects) {
            $arguments = @('msbuild', $project, '-nologo', "-p:Configuration=$Configuration",
                "-p:CustomAfterMicrosoftCommonTargets=$collector", '-getTargetResult:DorotiCollectLaunchInputs')
            if ($Rid) { $arguments += "-p:RuntimeIdentifier=$Rid" }
            $json = & dotnet @arguments
            if ($LASTEXITCODE -ne 0) { throw "Cannot establish evaluated build inputs: $project" }
            $result = ($json -join "`n" | ConvertFrom-Json -Depth 100).TargetResults.DorotiCollectLaunchInputs
            if ($result.Result -ne 'Success') { throw "Build input discovery failed: $project" }
            foreach ($item in $result.Items) {
                $path = $item.Identity
                if ($path -match '[\\/](bin|obj|node_modules|\.doroti|artifacts)[\\/]') { continue }
                if (Test-Path -LiteralPath $path -PathType Leaf) { Get-Item -LiteralPath $path }
            }
        }
    } finally { Pop-Location }
}

function Get-DorotiDependencyIdentity([string] $Runner, [string] $Configuration, [string] $Rid, [string] $WorkingDirectory) {
    $collector = Join-Path $PSScriptRoot 'launch-inputs.targets'
    $arguments = @('msbuild', $Runner, '-nologo', "-p:Configuration=$Configuration",
        "-p:CustomAfterMicrosoftCommonTargets=$collector", '-getTargetResult:DorotiCollectLaunchDependencies')
    if ($Rid) { $arguments += "-p:RuntimeIdentifier=$Rid" }
    Push-Location $WorkingDirectory
    try {
        $json = & dotnet @arguments
        if ($LASTEXITCODE -ne 0) { throw 'Cannot establish restored dependency identity. Restore/build before using -NoBuild.' }
        $result = ($json -join "`n" | ConvertFrom-Json -Depth 100).TargetResults.DorotiCollectLaunchDependencies
        if ($result.Result -ne 'Success') { throw 'Dependency discovery failed.' }
        $files = foreach ($item in $result.Items) {
            # obj/project.assets.json is deliberately included: it selects the
            # resolved graph. Do not apply the source-tree obj/bin exclusion.
            if (!(Test-Path -LiteralPath $item.Identity -PathType Leaf)) {
                throw "Resolved dependency is missing: $($item.Identity)"
            }
            Get-Item -LiteralPath $item.Identity
        }
        Get-DorotiContentFingerprint @($files) "$Runner|$Configuration|$Rid"
    } finally { Pop-Location }
}

function Test-DorotiDependencyRebuild($State, [string] $Dependencies, [string] $Toolchain) {
    !$State -or $State.schemaVersion -cne 'doroti.launch-state/v3' -or
        $State.dependencies -cne $Dependencies -or $State.toolchain -cne $Toolchain
}

function Get-DorotiToolchainIdentity([string] $WorkingDirectory) {
    Push-Location $WorkingDirectory
    try {
        $info = (& dotnet --info 2>&1 | Out-String)
        if ($LASTEXITCODE -ne 0) { throw 'Cannot establish dotnet SDK/workload identity.' }
        # Tool paths and bytes catch replacement at the same SDK/native-tool version.
        $tools = foreach ($name in @('dotnet','node','cmake','ninja','java','clang','cl','msbuild')) {
            $command = Get-Command $name -ErrorAction SilentlyContinue | Select-Object -First 1
            if ($command -and (Test-Path -LiteralPath $command.Source -PathType Leaf)) { Get-Item -LiteralPath $command.Source }
        }
        Get-DorotiContentFingerprint @($tools) $info
    } finally { Pop-Location }
}

function Get-DorotiArtifactIdentity([string] $Runner, [string] $Configuration, [string] $Rid, [string] $WorkingDirectory = (Get-Location).Path) {
    $arguments = @('msbuild', $Runner, '-nologo', "-p:Configuration=$Configuration",
        '-getProperty:TargetDir,TargetPath,RuntimeIdentifier,TargetFramework,IntermediateOutputPath')
    if ($Rid) { $arguments += "-p:RuntimeIdentifier=$Rid" }
    Push-Location $WorkingDirectory
    try {
        $json = & dotnet @arguments
        if ($LASTEXITCODE -ne 0) { throw 'Cannot evaluate artifact identity.' }
    } finally { Pop-Location }
    $properties = ($json -join "`n" | ConvertFrom-Json).Properties
    if (!$properties.TargetDir -or !(Test-Path -LiteralPath $properties.TargetPath -PathType Leaf)) {
        throw 'The evaluated runner artifact is missing. Build again without -NoBuild.'
    }
    $outputRoot = [IO.Path]::GetFullPath($properties.TargetDir)
    $files = @(Get-ChildItem -LiteralPath $outputRoot -Recurse -File | Sort-Object FullName | ForEach-Object {
        [ordered]@{ path=$_.FullName; length=$_.Length; sha256=(Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash }
    })
    if (!$files.Count) { throw 'An empty output directory cannot be reused.' }
    # Static web asset manifests are consumed from obj by dotnet run as well.
    $intermediate = [IO.Path]::GetFullPath($properties.IntermediateOutputPath, [IO.Path]::GetDirectoryName($Runner))
    $manifests = @(Get-ChildItem -LiteralPath $intermediate -Filter 'staticwebassets*.json' -File -ErrorAction SilentlyContinue | ForEach-Object {
        [ordered]@{ path=$_.FullName; length=$_.Length; sha256=(Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash }
    })
    # dotnet run serves generated assets from other projects' obj directories.
    # Bind those evaluated identities too; hashing only the manifest misses a
    # removed or tampered TypeScript payload behind an unchanged path.
    $assets = @()
    $buildManifest = Join-Path $intermediate 'staticwebassets.build.json'
    if (Test-Path -LiteralPath $buildManifest -PathType Leaf) {
        $assets = @((Get-Content -LiteralPath $buildManifest -Raw | ConvertFrom-Json).Assets | ForEach-Object {
            $assetPath = [IO.Path]::GetFullPath($_.Identity)
            if (!(Test-Path -LiteralPath $assetPath -PathType Leaf)) { throw "Evaluated static web asset is missing: $assetPath" }
            $assetFile = Get-Item -LiteralPath $assetPath
            [ordered]@{path=$assetPath;length=$assetFile.Length;sha256=(Get-FileHash -LiteralPath $assetPath -Algorithm SHA256).Hash}
        })
    }
    $allFiles = @(@($files) + @($manifests) + @($assets) | Sort-Object { $_.path } -Unique)
    [ordered]@{ outputRoot=$outputRoot; targetPath=$properties.TargetPath; rid=$properties.RuntimeIdentifier; framework=$properties.TargetFramework; files=$allFiles }
}

function Assert-DorotiArtifactIdentity($Expected, $Actual) {
    if (!$Expected -or !$Expected.files -or
        (ConvertTo-Json $Expected -Depth 10 -Compress) -cne (ConvertTo-Json $Actual -Depth 10 -Compress)) {
        throw 'The successful artifact is missing, modified, or stale (output/RID/framework mismatch). Build again without -NoBuild.'
    }
}
