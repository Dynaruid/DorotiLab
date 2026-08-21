#Requires -Version 7.0
param(
    [ValidateSet('Toolchain', 'Hosting', 'Graph', 'Template', 'Compile', 'Publish')]
    [string] $Shard
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
$webProject = Join-Path $repoRoot 'DorotiDemoApp/web/DorotiDemoApp.Web.csproj'
$evidencePath = Join-Path $dorotiRoot 'validation/evidence/web/web-product-evidence.json'

function Assert-True([bool] $Condition, [string] $Name) { if (-not $Condition) { throw "$Name failed." } }
function Invoke-Checked([scriptblock] $Command, [string] $Failure) {
    $global:LASTEXITCODE = 0
    & $Command | Out-Host
    if ($LASTEXITCODE -ne 0) { throw "$Failure (exit code $LASTEXITCODE)." }
}

switch ($Shard) {
    'Toolchain' {
        $sdk = (& dotnet --version).Trim()
        Assert-True ($LASTEXITCODE -eq 0 -and $sdk -match '^10\.') '.NET 10 SDK'
        $workloads = (& dotnet workload list | Out-String)
        Assert-True ($workloads -match 'wasm-tools') 'wasm-tools workload'
    }
    'Hosting' {
        Invoke-Checked { dotnet build (Join-Path $dorotiRoot 'src/Doroti.Host.Web/Doroti.Host.Web.csproj') -c Release --nologo } 'Web host build failed'
        Invoke-Checked { dotnet build (Join-Path $dorotiRoot 'src/Doroti.Target.Web.browser-wasm/Doroti.Target.Web.browser-wasm.csproj') -c Release --nologo } 'Web target build failed'
    }
    'Graph' {
        Invoke-Checked { & (Join-Path $PSScriptRoot 'validate-app-targets.ps1') -Shard Graph } 'workspace Web graph failed'
    }
    'Template' {
        Invoke-Checked { & (Join-Path $PSScriptRoot 'validate-app-targets.ps1') -Shard Template } 'workspace template gate failed'
    }
    'Compile' {
        Invoke-Checked { dotnet build $webProject -c Release --nologo } 'DemoApp Web runner build failed'
    }
    'Publish' {
        $publishRoot = Join-Path $dorotiRoot ".doroti/tmp/web-publish-$([Guid]::NewGuid().ToString('N'))"
        Invoke-Checked { dotnet publish $webProject -c Release -o $publishRoot --nologo } 'DemoApp Web publish failed'
        $wwwroot = Join-Path $publishRoot 'wwwroot'
        Assert-True (Test-Path -LiteralPath (Join-Path $wwwroot 'index.html') -PathType Leaf) 'published Web index'
        Assert-True (Test-Path -LiteralPath (Join-Path $wwwroot '_framework/blazor.webassembly.js') -PathType Leaf) 'published Blazor WebAssembly boot loader'
        Assert-True (Test-Path -LiteralPath (Join-Path $wwwroot 'doroti_bootstrap.js') -PathType Leaf) 'published TypeScript bootstrap'
        Assert-True (Test-Path -LiteralPath (Join-Path $wwwroot 'plugins/echo.js') -PathType Leaf) 'published TypeScript plugin'
        Assert-True (@(Get-ChildItem -LiteralPath $wwwroot -Recurse -File -Include '*.ts','tsconfig.json').Count -eq 0) 'published TypeScript source absence'
        $evidence = [ordered]@{
            schemaVersion = 'doroti.web-product-evidence/v2'
            capturedAt = [DateTimeOffset]::UtcNow.ToString('O')
            status = 'pass-build-publish'
            runnerProject = 'DorotiDemoApp/web/DorotiDemoApp.Web.csproj'
            targetFramework = 'net10.0'
            runtimeIdentifier = 'browser-wasm'
            host = 'BlazorWebAssembly'
            publish = [ordered]@{ index = 'pass'; blazorBootLoader = 'pass'; bootstrap = 'pass'; plugin = 'pass'; typeScriptSourceFiles = 0 }
            browserLive = 'notVerified'
            note = 'Build and static publish do not prove browser interaction, GPU presentation, accessibility, or clipboard/IME behavior.'
        }
        [IO.Directory]::CreateDirectory((Split-Path $evidencePath -Parent)) | Out-Null
        [IO.File]::WriteAllText($evidencePath, (($evidence | ConvertTo-Json -Depth 16) -replace "`r`n", "`n") + "`n", [Text.UTF8Encoding]::new($false))
    }
}

Write-Output "Doroti Web product shard '$Shard': PASS"
