[CmdletBinding()]
param(
    [Parameter(Mandatory)] [string] $InstallRoot,
    [Parameter(Mandatory)] [ValidatePattern('^[a-z][a-z0-9+.-]{1,63}$')] [string] $Scheme,
    [string] $Executable,
    [ValidateSet('Register', 'Remove')] [string] $Action = 'Register'
)
$ErrorActionPreference = 'Stop'
if (-not $IsWindows) { throw 'Protocol registration requires Windows.' }
if ($Scheme -in @('http', 'https', 'file', 'mailto', 'data', 'javascript', 'ftp', 'ms-settings')) {
    throw 'Use an application-specific protocol scheme.'
}
$rootPath = [IO.Path]::GetFullPath($InstallRoot)
$marker = Join-Path $rootPath '.doroti-candidate-install.json'
if (-not (Test-Path -LiteralPath $marker) -or (Get-Content -LiteralPath $marker -Raw).Trim() -ne 'Doroti local candidate installer v1') {
    throw 'Expected an installation owned by the candidate installer.'
}
$registryPath = 'Software\Classes\' + $Scheme
$existing = [Microsoft.Win32.Registry]::CurrentUser.OpenSubKey($registryPath)
try {
    if ($existing -and $existing.GetValue('DorotiInstallRoot') -ne $rootPath) {
        throw 'This protocol is already owned by another application.'
    }
} finally { if ($existing) { $existing.Dispose() } }
if ($Action -eq 'Remove') {
    [Microsoft.Win32.Registry]::CurrentUser.DeleteSubKeyTree($registryPath, $false)
    return
}
$current = Get-Content -LiteralPath (Join-Path $rootPath 'current.json') -Raw | ConvertFrom-Json
if ($current.platform -ne 'windows') { throw 'Only a Windows installation can register a protocol.' }
if (-not $Executable -or [IO.Path]::IsPathRooted($Executable) -or $Executable.Contains('"')) { throw 'Provide a relative executable path.' }
$versionRoot = [IO.Path]::GetFullPath($current.directory)
$installPrefix = $rootPath.TrimEnd('\') + '\versions\'
if (-not $versionRoot.StartsWith($installPrefix, [StringComparison]::OrdinalIgnoreCase)) { throw 'Version directory leaves the installation.' }
$program = [IO.Path]::GetFullPath((Join-Path $versionRoot $Executable))
if (-not $program.StartsWith($versionRoot.TrimEnd('\') + '\', [StringComparison]::OrdinalIgnoreCase) -or
    [IO.Path]::GetExtension($program) -ne '.exe' -or -not (Test-Path -LiteralPath $program -PathType Leaf)) {
    throw 'Executable must exist inside the current installed payload.'
}
$ancestor = $program
while ($ancestor) {
    if ((Get-Item -LiteralPath $ancestor -Force).Attributes -band [IO.FileAttributes]::ReparsePoint) { throw 'Linked installation paths are not supported.' }
    $ancestor = [IO.Path]::GetDirectoryName($ancestor)
}
$key = [Microsoft.Win32.Registry]::CurrentUser.CreateSubKey($registryPath)
try {
    $key.SetValue('', 'URL:' + $Scheme)
    $key.SetValue('URL Protocol', '')
    $key.SetValue('DorotiInstallRoot', $rootPath)
    $command = $key.CreateSubKey('shell\open\command')
    try { $command.SetValue('', '"' + $program + '" "%1"') } finally { $command.Dispose() }
} finally { $key.Dispose() }
Write-Output ('Registered current-user protocol: ' + $Scheme)
