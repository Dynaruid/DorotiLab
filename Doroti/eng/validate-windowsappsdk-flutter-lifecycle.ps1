#Requires -Version 7.0
[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$items = @(
    'Doroti/src/Doroti.Host.WindowsAppSdk/FlutterWindowsHostWindow.cs',
    'Doroti/src/Doroti.Host.WindowsAppSdk/FlutterWindowsLifecycleManager.cs',
    'Doroti/validation/windowsappsdk-flutter-lifecycle/Doroti.Validation.WindowsAppSdkFlutterLifecycle.csproj',
    'Doroti/validation/windowsappsdk-flutter-lifecycle/Program.cs',
    'Doroti/eng/validate-windowsappsdk-flutter-lifecycle-live.ps1'
)
$required = @{
    'Doroti/src/Doroti.Host.WindowsAppSdk/FlutterWindowsHostWindow.cs' = @('FlutterWindowsTopLevelMessage','ApplyDpiSuggestedRect','SetWindowPos')
    'Doroti/src/Doroti.Host.WindowsAppSdk/FlutterWindowsLifecycleManager.cs' = @('WmDpiChanged','SetFullscreen','MonitorFromRect','SetSuspended','BeginShutdown')
    'Doroti/validation/windowsappsdk-flutter-lifecycle/Program.cs' = @('TimeSpan.FromMinutes(20)','powerAndSessionRoundTrips','shutdownTerminalizedOnce')
}
$builder = [Text.StringBuilder]::new()
foreach($relative in ($items | Sort-Object)) {
    $path = Join-Path $repo $relative
    if(-not (Test-Path -LiteralPath $path -PathType Leaf)){throw "F8 contract input missing: $relative"}
    $text = Get-Content -LiteralPath $path -Raw
    if($required.ContainsKey($relative)){
        foreach($literal in @($required[$relative])){if(-not $text.Contains($literal,[StringComparison]::Ordinal)){throw "F8 literal missing in ${relative}: $literal"}}
    }
    $hash=(Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant()
    [void]$builder.Append($relative.Replace('\','/')).Append('=').Append($hash).Append("`n")
}
$fingerprint=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($builder.ToString()))).ToLowerInvariant()
[ordered]@{schemaVersion='doroti.windowsappsdk-flutter-lifecycle-validation/v1';status='PASS';sourceFingerprint=$fingerprint;validatedInputCount=$items.Count;scopeBoundary='F8 source contract only; live and physical acceptance remain separate.'}|ConvertTo-Json -Depth 8
