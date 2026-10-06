param([Parameter(Position=0)][string]$Action, [string]$App, [string]$Platform, [string]$Configuration,
      [string]$SessionDirectory, [string]$SessionId, [string]$DotnetPath)
$ErrorActionPreference = 'Stop'
if ($Action -eq 'describe') {
    $editingProject = Get-ChildItem -LiteralPath $App -Filter '*.csproj' | Select-Object -First 1
    @{ schemaVersion = 'doroti.cli-workspace/v2'; root = $App; applicationProject = $editingProject.FullName;
       platforms = @{ 'editing-fixture' = $editingProject.FullName }; developmentTargets = @('editing-fixture');
       developmentSupport = @(@{ platform = 'editing-fixture'; runner = $editingProject.FullName; provider = 'editing-contract-fixture'; version = '1.0.0'; operations = @('build', 'dev');
          development = @{ transport = 'file'; debug = $true; requiresPreparedAcknowledgment = $false; usesStopSignal = $false; launchBrowser = $false } }) } | ConvertTo-Json -Depth 8 -Compress
    exit
}
if ($Action -ne 'dev') { throw 'Unsupported fixture action' }
$editingRuntime = @{ schemaVersion = 'doroti.dev/v1'; sessionId = $SessionId; runtimeId = [guid]::NewGuid().ToString(); revision = 0; supported = $true; status = 'started' }
$editingRuntimeFile = Join-Path $SessionDirectory 'runtime.json'
$editingRequestFile = Join-Path $SessionDirectory 'request.json'
$editingRuntime | ConvertTo-Json -Compress | Set-Content -LiteralPath $editingRuntimeFile -Encoding utf8
while ($true) {
    if (Test-Path -LiteralPath $editingRequestFile) {
        $editingRequest = Get-Content -LiteralPath $editingRequestFile -Raw | ConvertFrom-Json
        if ($editingRequest.sessionId -eq $SessionId -and $editingRequest.runtimeId -eq $editingRuntime.runtimeId -and $editingRequest.requestId -ne $editingRuntime.requestId) {
            $editingRuntime.requestId = $editingRequest.requestId; $editingRuntime.revision++; $editingRuntime.status = 'applied'
            $editingRuntime | ConvertTo-Json -Compress | Set-Content -LiteralPath $editingRuntimeFile -Encoding utf8
        }
    }
    Start-Sleep -Milliseconds 50
}
