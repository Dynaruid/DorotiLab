param(
    [Parameter(Mandatory)][string[]]$Device,
    [string]$NativeLibrary,
    [switch]$ExtendedContext,
    [ValidateRange(1,10)][int]$ContextCycles = 1
)
$ErrorActionPreference = 'Stop'
$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../..'))
$resultRoot = Join-Path $repoRoot ('Doroti/artifacts/native-graphite/runs/' + [DateTime]::UtcNow.ToString('yyyyMMddTHHmmssfffZ'))
New-Item -ItemType Directory -Force $resultRoot | Out-Null

function Invoke-Bounded([string]$Label, [string]$Executable, [string[]]$Arguments) {
    $startInfo = [Diagnostics.ProcessStartInfo]::new($Executable)
    $startInfo.WorkingDirectory = $repoRoot
    $startInfo.UseShellExecute = $false
    $startInfo.CreateNoWindow = $true
    $startInfo.RedirectStandardOutput = $true
    $startInfo.RedirectStandardError = $true
    foreach ($argument in $Arguments) { $startInfo.ArgumentList.Add($argument) }
    $process = [Diagnostics.Process]::Start($startInfo)
    $stdout = $process.StandardOutput.ReadToEndAsync()
    $stderr = $process.StandardError.ReadToEndAsync()
    $timedOut = -not $process.WaitForExit(1200000)
    if ($timedOut) { $process.Kill($true); $process.WaitForExit() }
    $stdout.GetAwaiter().GetResult() | Set-Content (Join-Path $resultRoot "$Label.stdout.log")
    $stderr.GetAwaiter().GetResult() | Set-Content (Join-Path $resultRoot "$Label.stderr.log")
    @{ executable=$Executable; arguments=$Arguments; timeoutSeconds=1200; timedOut=$timedOut; exitCode=$process.ExitCode } |
        ConvertTo-Json -Depth 10 | Set-Content (Join-Path $resultRoot "$Label.process.json")
    $code = $process.ExitCode
    $process.Dispose()
    if ($timedOut) { throw "$Label exceeded the external 20-minute timeout. Evidence: $resultRoot" }
    return $code
}

Push-Location $repoRoot
try {
    git rev-parse HEAD | Set-Content (Join-Path $resultRoot 'commit.txt')
    git status --short | Set-Content (Join-Path $resultRoot 'status.txt')
    git diff --binary | Set-Content (Join-Path $resultRoot 'working.diff')
    Get-CimInstance Win32_OperatingSystem | Select-Object Caption,Version,BuildNumber,OSArchitecture |
        ConvertTo-Json | Set-Content (Join-Path $resultRoot 'os.json')
    Get-CimInstance Win32_VideoController | Select-Object Name,DriverVersion,PNPDeviceID |
        ConvertTo-Json | Set-Content (Join-Path $resultRoot 'gpu.json')
    $version = ([xml](Get-Content Doroti/Directory.Packages.props -Raw)).Project.ItemGroup.PackageVersion |
        Where-Object Include -EQ 'SkiaSharp' | Select-Object -ExpandProperty Version
    $packageRoot = if ($env:NUGET_PACKAGES) { $env:NUGET_PACKAGES } else { Join-Path ([Environment]::GetFolderPath('UserProfile')) '.nuget/packages' }
    $assets = foreach ($package in Get-ChildItem $packageRoot -Directory -Filter 'skiasharp*') {
        $versionRoot = Join-Path $package.FullName $version
        if (!(Test-Path $versionRoot)) { continue }
        foreach ($file in Get-ChildItem $versionRoot -Recurse -File | Where-Object { $_.Name -match '^(libSkiaSharp|SkiaSharp\.dll)' }) {
            @{ package=$package.Name; path=$file.FullName; bytes=$file.Length; sha256=(Get-FileHash $file.FullName -Algorithm SHA256).Hash.ToLowerInvariant() }
        }
    }
    $assets | ConvertTo-Json -Depth 5 | Set-Content (Join-Path $resultRoot 'package-assets.json')
    Get-ChildItem Doroti/src -Directory -Filter 'Doroti.Target.*' | ForEach-Object {
        $manifest = Join-Path $_.FullName 'doroti-target-manifest.json'
        if (Test-Path $manifest) { Get-Content $manifest -Raw | ConvertFrom-Json }
    } | ConvertTo-Json -Depth 10 | Set-Content (Join-Path $resultRoot 'current-targets.json')
    $project = Join-Path $PSScriptRoot 'Doroti.Validation.WindowsVulkanCapability.csproj'
    if ((Invoke-Bounded 'build' 'dotnet' @('build',$project,'-c','Release','--nologo')) -ne 0) {
        throw "Probe build failed. Evidence: $resultRoot"
    }
    $exe = Join-Path $PSScriptRoot 'bin/Release/net10.0-windows10.0.19041.0/win-x64/Doroti.Validation.WindowsVulkanCapability.exe'
    $index = 0
    $failed = $false
    foreach ($selector in $Device) {
        $common = @('--device',$selector)
        $baseline = Invoke-Bounded "baseline-$index" $exe ($common + @('--output',(Join-Path $resultRoot "baseline-$index.json")))
        $probeArgs = $common + @('--graphite','--output',(Join-Path $resultRoot "graphite-$index.json"))
        if ($NativeLibrary) { $probeArgs += @('--graphite-native',[IO.Path]::GetFullPath($NativeLibrary)) }
        if ($ExtendedContext) { $probeArgs += '--graphite-extended-context' }
        $probeArgs += @('--graphite-context-cycles', "$ContextCycles")
        $probe = Invoke-Bounded "graphite-$index" $exe $probeArgs
        if ($baseline -ne 0 -or $probe -ne 2) { $failed = $true }
        $index++
    }
    Write-Output "Evidence: $resultRoot"
    if ($failed) { exit 1 }
    # Exit 2 explicitly preserves PARTIAL, even if every diagnostic check passed.
    exit 2
}
finally { Pop-Location }
