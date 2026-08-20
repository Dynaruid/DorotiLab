[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [ValidateSet('doctor', 'build', 'run', 'publish', 'native', 'scaffold-interop', 'validate', 'audit', 'release', 'clean')]
    [string] $Command = 'doctor',

    [Parameter(Position = 1)]
    [ValidateSet('doctor', 'build', 'open', 'add')]
    [string] $NativeCommand,

    [ValidateSet('Source', 'Build', 'Targets', 'Fcr0', 'Fcr1', 'Fcr2', 'Fcr3', 'Fcr4', 'Fcr5', 'Fcr6', 'Fcr7', 'Fcr8', 'Developer', 'Release')]
    [string] $ValidationSuite = 'Developer',

    [string] $App,

    [ValidateSet('android', 'ios', 'linux', 'macos', 'maccatalyst', 'web', 'windows', 'all')]
    [string] $Platform,

    [string] $Rid,

    [string] $Device,

    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Release',

    [switch] $Launch,

    [ValidatePattern('^[A-Za-z][A-Za-z0-9_.-]*$')]
    [string] $InteropName = 'DorotiNativeInterop'
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$repositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $dorotiRoot '..'))
$solution = Join-Path $dorotiRoot 'Doroti.slnx'
$productSolution = Join-Path $dorotiRoot 'Doroti.Product.slnx'
$artifacts = Join-Path $dorotiRoot 'artifacts'

function Invoke-Checked {
    param(
        [Parameter(Mandatory)] [string] $File,
        [Parameter(Mandatory)] [AllowEmptyCollection()] [string[]] $Arguments,
        [string] $WorkingDirectory = $dorotiRoot
    )

    Push-Location $WorkingDirectory
    try {
        & $File @Arguments
        if ($LASTEXITCODE -ne 0) {
            throw "Command failed with exit code ${LASTEXITCODE}: $File $($Arguments -join ' ')"
        }
    }
    finally {
        Pop-Location
    }
}

function Invoke-CheckedCapture {
    param(
        [Parameter(Mandatory)] [string] $File,
        [Parameter(Mandatory)] [AllowEmptyCollection()] [string[]] $Arguments,
        [string] $WorkingDirectory = $dorotiRoot
    )

    Push-Location $WorkingDirectory
    try {
        $output = @(& $File @Arguments 2>&1 | ForEach-Object { $_.ToString() })
        if ($LASTEXITCODE -ne 0) {
            throw "Command failed with exit code ${LASTEXITCODE}: $File $($Arguments -join ' ')`n$($output -join "`n")"
        }
        $output | ForEach-Object { Write-Host $_ }
        return $output
    }
    finally {
        Pop-Location
    }
}

function Get-CommandResult {
    param([string] $File, [string[]] $Arguments)
    try {
        $output = (& $File @Arguments 2>&1 | Out-String).Trim()
        return [ordered]@{ available = ($LASTEXITCODE -eq 0); output = $output }
    }
    catch {
        return [ordered]@{ available = $false; output = $_.Exception.Message }
    }
}

function Resolve-DorotiWorkspace {
    param(
        [Parameter(Mandatory)] [string] $AppPath,
        [switch] $AllowAllPlatforms
    )

    $resolvedInput = if ([System.IO.Path]::IsPathFullyQualified($AppPath)) {
        [System.IO.Path]::GetFullPath($AppPath)
    }
    else {
        [System.IO.Path]::GetFullPath((Join-Path (Get-Location) $AppPath))
    }
    $manifestPath = if (Test-Path -LiteralPath $resolvedInput -PathType Container) {
        Join-Path $resolvedInput 'doroti-workspace.json'
    }
    elseif ([System.IO.Path]::GetFileName($resolvedInput) -ceq 'doroti-workspace.json') {
        $resolvedInput
    }
    else {
        Join-Path ([System.IO.Path]::GetDirectoryName($resolvedInput)) 'doroti-workspace.json'
    }
    if (-not (Test-Path -LiteralPath $manifestPath -PathType Leaf)) {
        throw "Doroti workspace manifest was not found: $manifestPath"
    }

    $workspaceRoot = [System.IO.Path]::GetFullPath([System.IO.Path]::GetDirectoryName($manifestPath))
    $allowedRoot = $workspaceRoot.TrimEnd([System.IO.Path]::DirectorySeparatorChar) + [System.IO.Path]::DirectorySeparatorChar
    $document = [System.Text.Json.JsonDocument]::Parse((Get-Content -LiteralPath $manifestPath -Raw))
    try {
        $root = $document.RootElement
        if ($root.ValueKind -ne [System.Text.Json.JsonValueKind]::Object) { throw 'Doroti workspace root must be a JSON object.' }
        $schemaVersion = $root.GetProperty('schemaVersion').GetString()
        if ($schemaVersion -cne 'doroti.workspace/v1') { throw "Unsupported Doroti workspace schema: $schemaVersion" }
        $applicationRelative = $root.GetProperty('applicationProject').GetString()
        if ([string]::IsNullOrWhiteSpace($applicationRelative)) { throw 'applicationProject must be a non-empty relative path.' }
        $applicationProject = [System.IO.Path]::GetFullPath((Join-Path $workspaceRoot $applicationRelative))
        if (-not $applicationProject.StartsWith($allowedRoot, [System.StringComparison]::OrdinalIgnoreCase) -or
            -not (Test-Path -LiteralPath $applicationProject -PathType Leaf)) {
            throw "applicationProject escapes the workspace or does not exist: $applicationRelative"
        }

        $allowedAliases = @('android', 'ios', 'linux', 'macos', 'maccatalyst', 'web', 'windows')
        $platformEntries = @($root.GetProperty('platforms').EnumerateObject())
        $duplicateAliases = @($platformEntries | Group-Object { $_.Name.ToLowerInvariant() } | Where-Object Count -gt 1)
        if ($duplicateAliases.Count -gt 0) { throw "Duplicate platform aliases: $($duplicateAliases.Name -join ', ')." }
        $unexpectedAliases = @($platformEntries | Where-Object { $_.Name -cnotin $allowedAliases })
        if ($unexpectedAliases.Count -gt 0) { throw "Unknown or non-canonical platform aliases: $($unexpectedAliases.Name -join ', ')." }
        $missingAliases = @($allowedAliases | Where-Object { $_ -cnotin $platformEntries.Name })
        if ($missingAliases.Count -gt 0) { throw "Missing platform aliases: $($missingAliases -join ', ')." }

        $runners = [ordered]@{}
        foreach ($entry in $platformEntries) {
            $relative = $entry.Value.GetString()
            if ([string]::IsNullOrWhiteSpace($relative)) { throw "Runner path for '$($entry.Name)' is empty." }
            $runner = [System.IO.Path]::GetFullPath((Join-Path $workspaceRoot $relative))
            if (-not $runner.StartsWith($allowedRoot, [System.StringComparison]::OrdinalIgnoreCase) -or
                -not (Test-Path -LiteralPath $runner -PathType Leaf)) {
                throw "Runner path for '$($entry.Name)' escapes the workspace or does not exist: $relative"
            }
            $runners[$entry.Name] = $runner
        }
        return [pscustomobject]@{
            Manifest = $manifestPath
            Root = $workspaceRoot
            ApplicationProject = $applicationProject
            Runners = $runners
        }
    }
    finally {
        $document.Dispose()
    }
}

function Invoke-WorkspaceDotNet {
    param([Parameter(Mandatory)] [ValidateSet('build', 'run', 'publish')] [string] $Verb)

    if ([string]::IsNullOrWhiteSpace($App)) { throw "$Verb requires --app <path>." }
    if ([string]::IsNullOrWhiteSpace($Platform) -or $Platform -ceq 'all') { throw "$Verb requires one --platform <name>." }
    $workspace = Resolve-DorotiWorkspace $App
    $runner = $workspace.Runners[$Platform]
    $arguments = if ($Verb -ceq 'run') { @('run', '--project', $runner) } else { @($Verb, $runner) }
    if (-not [string]::IsNullOrWhiteSpace($Rid)) { $arguments += "-p:RuntimeIdentifier=$Rid" }
    if (-not [string]::IsNullOrWhiteSpace($Device)) { $arguments += "-p:DorotiDevice=$Device" }
    $arguments += '--nologo'
    Invoke-Checked 'dotnet' $arguments $workspace.Root
}

function Resolve-DorotiNativeWorkspace {
    if ([string]::IsNullOrWhiteSpace($App)) { throw 'native commands require --app <path>.' }
    if ($Platform -cnotin @('android', 'ios', 'macos', 'maccatalyst')) { throw 'native commands require --platform android, ios, macos, or maccatalyst.' }
    $workspace = Resolve-DorotiWorkspace $App
    $platformDirectory = if ($Platform -ceq 'maccatalyst') { 'macos' } else { $Platform }
    $platformRoot = Join-Path $workspace.Root $platformDirectory
    $runner = $workspace.Runners[$Platform]
    [xml] $runnerProject = Get-Content -LiteralPath $runner -Raw
    $bindingItems = @($runnerProject.Project.ItemGroup.DorotiNativeBindingProject | Where-Object { -not [string]::IsNullOrWhiteSpace($_.Include) })
    if ($bindingItems.Count -ne 1) {
        throw "The $Platform runner must declare exactly one DorotiNativeBindingProject; found $($bindingItems.Count): $runner"
    }
    $binding = [IO.Path]::GetFullPath((Join-Path ([IO.Path]::GetDirectoryName($runner)) ([string] $bindingItems[0].Include)))
    $allowedPlatformRoot = [IO.Path]::GetFullPath($platformRoot).TrimEnd([IO.Path]::DirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar
    if (-not $binding.StartsWith($allowedPlatformRoot, [StringComparison]::OrdinalIgnoreCase) -or
        -not (Test-Path -LiteralPath $binding -PathType Leaf)) {
        throw "The native binding escapes the $Platform workspace or does not exist: $binding"
    }
    $native = Join-Path $platformRoot 'native'
    if (-not (Test-Path -LiteralPath $native -PathType Container)) { throw "Native project is missing: $native" }
    return [pscustomobject]@{ Workspace = $workspace; PlatformRoot = $platformRoot; Runner = $runner; Binding = $binding; Native = $native }
}

function Resolve-AndroidJavaHome {
    $candidates = @()
    if ($IsWindows) {
        $dotnetAndroidJavaRoot = 'C:\Program Files\Android\openjdk'
        if (Test-Path -LiteralPath $dotnetAndroidJavaRoot -PathType Container) {
            $candidates += @(Get-ChildItem -LiteralPath $dotnetAndroidJavaRoot -Directory | Sort-Object Name -Descending | Select-Object -ExpandProperty FullName)
        }
    }
    if (-not [string]::IsNullOrWhiteSpace($env:JAVA_HOME)) { $candidates += $env:JAVA_HOME }
    foreach ($candidate in @($candidates | Select-Object -Unique)) {
        $java = Join-Path $candidate "bin/java$(if ($IsWindows) { '.exe' })"
        if (-not (Test-Path -LiteralPath $java -PathType Leaf)) { continue }
        $version = (& $java -version 2>&1 | Out-String).Trim()
        if ($LASTEXITCODE -eq 0 -and $version -match 'version "(?<major>\d+)' -and
            [int]$Matches.major -ge 17 -and [int]$Matches.major -le 21 -and $version -notmatch 'GraalVM') {
            return [pscustomobject]@{ Home = $candidate; Version = $version.Split("`n")[0].Trim() }
        }
    }
    throw 'A supported OpenJDK 17-21 installation was not found. Install the .NET Android OpenJDK or configure JAVA_HOME to a non-GraalVM JDK.'
}

function Invoke-DorotiNative {
    if ([string]::IsNullOrWhiteSpace($NativeCommand)) { throw 'native requires doctor, build, open, or add.' }
    $nativeWorkspace = Resolve-DorotiNativeWorkspace
    switch ($NativeCommand) {
        'doctor' {
            $dotnet = Get-CommandResult 'dotnet' @('--version')
            if ($Platform -ceq 'android') {
                $java = Resolve-AndroidJavaHome
                $wrapper = Join-Path $nativeWorkspace.Native 'gradlew.bat'
                if (-not $dotnet.available -or -not (Test-Path -LiteralPath $wrapper -PathType Leaf)) {
                    throw "Android native prerequisites are incomplete. dotnet=$($dotnet.available), wrapper=$(Test-Path -LiteralPath $wrapper)."
                }
                Write-Host "Native doctor: PASS (Android; dotnet=$($dotnet.output); java=$($java.Version); javaHome=$($java.Home); wrapper=$wrapper)"
            }
            else {
                $xcode = Get-CommandResult 'xcode-select' @('-p')
                if (-not $dotnet.available -or -not $xcode.available) {
                    throw "Apple native prerequisites are incomplete. dotnet=$($dotnet.available), xcode-select=$($xcode.available). Run this command on a Mac with Xcode."
                }
                Write-Host "Native doctor: PASS ($Platform; dotnet=$($dotnet.output); xcode=$($xcode.output))"
            }
        }
        'build' {
            $arguments = @('build', $nativeWorkspace.Binding, '--configuration', $Configuration, '--nologo')
            if (-not [string]::IsNullOrWhiteSpace($Rid)) { $arguments += "-p:RuntimeIdentifier=$Rid" }
            Invoke-Checked 'dotnet' $arguments $nativeWorkspace.PlatformRoot
            Write-Host "Native binding build: PASS ($Platform, $Configuration, $($nativeWorkspace.Binding))"
        }
        'open' {
            $project = if ($Platform -ceq 'android') { $nativeWorkspace.Native } else {
                @(Get-ChildItem -LiteralPath $nativeWorkspace.Native -Directory -Filter '*.xcodeproj' | Select-Object -ExpandProperty FullName -First 1)
            }
            if ($null -eq $project -or @($project).Count -eq 0) { throw "Native IDE project was not found under $($nativeWorkspace.Native)." }
            $project = [string] @($project)[0]
            Write-Host $project
            if ($Launch) {
                if ($Platform -ceq 'android') {
                    $studio = Get-Command 'studio64.exe' -ErrorAction SilentlyContinue
                    if ($null -eq $studio) { throw 'Android Studio (studio64.exe) was not found on PATH.' }
                    Start-Process -FilePath $studio.Source -ArgumentList @($project)
                }
                else { Invoke-Checked 'xed' @($project) $nativeWorkspace.PlatformRoot }
            }
        }
        'add' {
            $bridgeRoot = Join-Path $nativeWorkspace.Native 'bridges'
            $bridgePath = Join-Path $bridgeRoot "$InteropName.bridge.json"
            if (Test-Path -LiteralPath $bridgePath) { throw "Named bridge already exists: $bridgePath" }
            [IO.Directory]::CreateDirectory($bridgeRoot) | Out-Null
            $bridge = [ordered]@{ schemaVersion='doroti.native-named-bridge/v1'; name=$InteropName; platform=$Platform; owner='native-project'; status='declared' }
            [IO.File]::WriteAllText($bridgePath, (($bridge | ConvertTo-Json) -replace "`r`n", "`n") + "`n", [Text.UTF8Encoding]::new($false))
            Write-Host "Created named native bridge declaration: $bridgePath"
        }
    }
}

function Write-InteropMigrationDiagnostic {
    throw 'scaffold-interop is obsolete because Android, iOS, native macOS, and Mac Catalyst bridges are generated by default. Use: doroti native doctor|build|open|add --app <path> --platform <name>.'
}

function Invoke-Doctor {
    if (-not [string]::IsNullOrWhiteSpace($App)) {
        $workspace = Resolve-DorotiWorkspace $App
        $selected = if ([string]::IsNullOrWhiteSpace($Platform) -or $Platform -ceq 'all') { @($workspace.Runners.Keys) } else { @($Platform) }
        foreach ($alias in $selected) {
            Write-Host "$alias=$($workspace.Runners[$alias])"
        }
    }
    $dotnet = Get-CommandResult 'dotnet' @('--version')
    $workloads = Get-CommandResult 'dotnet' @('workload', 'list')
    $xcode = if ($IsMacOS) { Get-CommandResult 'xcodebuild' @('-version') } else { [ordered]@{ available=$false; output='notAvailable' } }
    $macosSdk = if ($IsMacOS) { Get-CommandResult 'xcrun' @('--sdk', 'macosx', '--show-sdk-version') } else { [ordered]@{ available=$false; output='notAvailable' } }
    $powerShell = [ordered]@{ available = $PSVersionTable.PSVersion.Major -ge 7; output = $PSVersionTable.PSVersion.ToString() }
    $flutterCheckout = Test-Path -LiteralPath (Join-Path $repositoryRoot 'reference/flutter-master') -PathType Container
    $success = $dotnet.available -and $powerShell.available
    $report = [ordered]@{
        schemaVersion = 'doroti.doctor/v3'
        success = $success
        dotnet = $dotnet
        workloads = $workloads
        xcode = $xcode
        macosSdk = $macosSdk
        hostArchitecture = [Runtime.InteropServices.RuntimeInformation]::OSArchitecture.ToString()
        appKitBackend = [ordered]@{ minimumOS='14.0'; runtimeIdentifier='osx-arm64'; packageVersion='0.1.0-preview.12.26368.2'; experimental=$true }
        powerShell = $powerShell
        referenceTools = [ordered]@{
            requiredForProductDevelopment = $false
            flutterCheckout = $flutterCheckout
            note = 'The pinned Flutter source is optional for reference comparison and migration work.'
        }
    }

    $outputDirectory = Join-Path $artifacts 'doctor'
    New-Item -ItemType Directory -Force -Path $outputDirectory | Out-Null
    $json = $report | ConvertTo-Json -Depth 8
    [System.IO.File]::WriteAllText((Join-Path $outputDirectory 'doctor.json'), ($json -replace "`r`n", "`n") + "`n", [System.Text.UTF8Encoding]::new($false))
    $markdown = @(
        '# Doroti doctor',
        '',
        "Status: **$(if ($success) { 'PASS' } else { 'FAIL' })**",
        '',
        "- .NET SDK: $($dotnet.output)",
        "- PowerShell: $($powerShell.output)",
        "- Flutter reference checkout (optional): $flutterCheckout"
    ) -join "`n"
    [System.IO.File]::WriteAllText((Join-Path $outputDirectory 'doctor.md'), $markdown + "`n", [System.Text.UTF8Encoding]::new($false))
    Write-Host "Doctor: $(if ($success) { 'PASS' } else { 'FAIL' })"
    if (-not $success) { throw 'Development environment is missing a required product-development tool. See artifacts/doctor/doctor.json.' }
}

function Invoke-Build {
    if ([string]::IsNullOrWhiteSpace($App)) {
        Invoke-Checked 'dotnet' @('build', $productSolution, '--nologo')
    }
    else {
        Invoke-WorkspaceDotNet 'build'
    }
}

function Invoke-Validation {
    & (Join-Path $PSScriptRoot 'validate.ps1') -Suite $ValidationSuite
}

function Invoke-Audit {
    & (Join-Path $PSScriptRoot 'validate-local-storage.ps1')
    & (Join-Path $PSScriptRoot 'validate.ps1') -Suite Source
}

function Invoke-Release {
    & (Join-Path $PSScriptRoot 'validate.ps1') -Suite Release
    Invoke-Audit

    $packageDirectory = Join-Path $artifacts 'packages'
    New-Item -ItemType Directory -Force -Path $packageDirectory | Out-Null
    Get-ChildItem -LiteralPath $packageDirectory -File |
        Where-Object { $_.Extension -in @('.nupkg', '.snupkg') } |
        Remove-Item -Force
    Invoke-Checked 'dotnet' @('pack', $productSolution, '--configuration', 'Release', '--output', $packageDirectory, '--nologo')
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    foreach ($packagePath in (Get-ChildItem -LiteralPath $packageDirectory -Filter '*.nupkg' -File)) {
        $archive = [System.IO.Compression.ZipFile]::OpenRead($packagePath.FullName)
        try {
            $entries = @($archive.Entries | ForEach-Object { $_.FullName })
            if ($entries | Where-Object { $_ -match '\.(dart|yaml|lock)$' }) {
                throw "Package contains compiler/source snapshot content: $($packagePath.Name)"
            }
            $nuspec = $archive.Entries | Where-Object { $_.FullName -like '*.nuspec' } | Select-Object -First 1
            $reader = [System.IO.StreamReader]::new($nuspec.Open())
            try { [xml] $packageXml = $reader.ReadToEnd() } finally { $reader.Dispose() }
            $packageId = [string] $packageXml.package.metadata.id
            $dependencies = @($packageXml.package.metadata.dependencies.group.dependency | ForEach-Object { $_.id })
            $avaloniaDependencies = @($dependencies | Where-Object { $_ -eq 'Avalonia' -or $_ -like 'Avalonia.*' })
            if ($avaloniaDependencies.Count -gt 0) { throw "C0 package retains an Avalonia binary dependency: $packageId" }
        }
        finally {
            $archive.Dispose()
        }
    }
    Write-Host 'Release: PASS'
}

function Invoke-Clean {
    Invoke-Checked 'dotnet' @('clean', $solution, '--nologo')
    $allowedRoot = $dorotiRoot.TrimEnd([System.IO.Path]::DirectorySeparatorChar) + [System.IO.Path]::DirectorySeparatorChar
    $targets = @()
    foreach ($project in (Get-ChildItem -LiteralPath $dorotiRoot -Recurse -Filter '*.csproj' -File)) {
        $projectDirectory = $project.Directory.FullName
        foreach ($outputName in @('bin', 'obj')) {
            $outputPath = Join-Path $projectDirectory $outputName
            if (Test-Path -LiteralPath $outputPath -PathType Container) {
                $targets += Get-Item -LiteralPath $outputPath
            }
        }
    }
    if (Test-Path -LiteralPath $artifacts) { $targets += Get-Item -LiteralPath $artifacts }
    foreach ($target in $targets) {
        $resolved = [System.IO.Path]::GetFullPath($target.FullName)
        if (-not $resolved.StartsWith($allowedRoot, [System.StringComparison]::OrdinalIgnoreCase)) {
            throw "Refusing to remove path outside Doroti root: $resolved"
        }
        Remove-Item -LiteralPath $resolved -Recurse -Force
    }
    & (Join-Path $PSScriptRoot 'clean-local-state.ps1') -Action temporary -Force
    Write-Host 'Clean: PASS'
}

switch ($Command) {
    'doctor' { Invoke-Doctor }
    'build' { Invoke-Build }
    'run' { Invoke-WorkspaceDotNet 'run' }
    'publish' { Invoke-WorkspaceDotNet 'publish' }
    'native' { Invoke-DorotiNative }
    'scaffold-interop' { Write-InteropMigrationDiagnostic }
    'validate' { Invoke-Validation }
    'audit' { Invoke-Audit }
    'release' { Invoke-Release }
    'clean' { Invoke-Clean }
}
