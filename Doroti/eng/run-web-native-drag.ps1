[CmdletBinding()]
param(
    [Parameter(Mandatory)] [string] $TitleToken,
    [Parameter(Mandatory)] [string] $OutputPath,
    [ValidateSet('Left', 'Right', 'Top', 'Bottom', 'TopLeft', 'TopRight', 'BottomLeft', 'BottomRight')]
    [string] $Edge = 'TopLeft',
    [ValidateSet('expand', 'shrink', 'reverse')] [string] $Motion = 'reverse',
    [ValidateSet(150, 600, 1200)] [int] $DragMilliseconds = 150
)

$ErrorActionPreference = 'Stop'
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$driver = Join-Path $repositoryRoot '.doroti/build/windows-resize-capture-vulkan/Release/Doroti.WindowsResizeCapture.exe'
if (-not (Test-Path -LiteralPath $driver -PathType Leaf)) {
    throw "Build the existing Windows resize driver before this test: $driver"
}
Add-Type -TypeDefinition @'
using System;
using System.Text;
using System.Runtime.InteropServices;
public static class DorotiWebResizeTarget {
    public delegate bool Visitor(IntPtr hwnd, IntPtr parameter);
    [DllImport("user32.dll")] public static extern bool EnumWindows(Visitor visitor, IntPtr parameter);
    [DllImport("user32.dll", CharSet=CharSet.Unicode)] public static extern int GetWindowText(IntPtr hwnd, StringBuilder text, int capacity);
    [DllImport("user32.dll")] public static extern bool IsWindowVisible(IntPtr hwnd);
    [DllImport("user32.dll")] public static extern uint GetDpiForWindow(IntPtr hwnd);
}
'@
$targetHandles = [Collections.Generic.List[long]]::new()
$visitor = [DorotiWebResizeTarget+Visitor]{
    param([IntPtr] $candidate, [IntPtr] $parameter)
    if (-not [DorotiWebResizeTarget]::IsWindowVisible($candidate)) { return $true }
    $title = [Text.StringBuilder]::new(1024)
    [void][DorotiWebResizeTarget]::GetWindowText($candidate, $title, $title.Capacity)
    if ($title.ToString().Contains($TitleToken, [StringComparison]::Ordinal)) { $targetHandles.Add($candidate.ToInt64()) }
    return $true
}
[void][DorotiWebResizeTarget]::EnumWindows($visitor, [IntPtr]::Zero)
if ($targetHandles.Count -ne 1) { throw "Expected one owned browser with title '$TitleToken'; found $($targetHandles.Count)." }
# Chrome's minimum tracking size is larger than the native sample at high DPI.
# Keep the Windows driver's defaults and speed; give only this browser adequate room.
$dpiScale = [DorotiWebResizeTarget]::GetDpiForWindow([IntPtr]$targetHandles[0]) / 96.0
$baseWidth = [int][Math]::Ceiling(640 * $dpiScale)
$baseHeight = [int][Math]::Ceiling(400 * $dpiScale)

$output = [IO.Path]::GetFullPath($OutputPath)
[void][IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($output))
$start = [Diagnostics.ProcessStartInfo]::new($driver)
$start.UseShellExecute = $false
$start.CreateNoWindow = $true
$start.WindowStyle = [Diagnostics.ProcessWindowStyle]::Hidden
$start.RedirectStandardOutput = $true
$start.RedirectStandardError = $true
# Use the very same QPC-deadline SendInput driver as Windows/Vulkan. Log-only
# removes capture cost, without replacing the native border drag by SetWindowPos.
foreach ($argument in @('--hwnd', [string]$targetHandles[0], '--output', $output, '--run-id', $TitleToken,
    '--f6r', '--input-hz', '240', '--drag-pixels', '600', '--drag-ms', [string]$DragMilliseconds,
    '--f6r-base-width', [string]$baseWidth, '--f6r-base-height', [string]$baseHeight,
    '--edge', $Edge, '--motion', $Motion, '--duration', '1', '--log-only', '--capture-only')) {
    $start.ArgumentList.Add($argument)
}
$process = [Diagnostics.Process]::new()
$process.StartInfo = $start
function Get-ClockCalibration {
    $before = [Diagnostics.Stopwatch]::GetTimestamp()
    $unix = [DateTimeOffset]::UtcNow.ToUnixTimeMilliseconds()
    $after = [Diagnostics.Stopwatch]::GetTimestamp()
    return @{ qpc = ($before + $after) / 2.0; unixMilliseconds = $unix;
        qpcFrequency = [Diagnostics.Stopwatch]::Frequency;
        uncertaintyMilliseconds = 1.0 + ($after - $before) * 500.0 / [Diagnostics.Stopwatch]::Frequency }
}
$clockStart = Get-ClockCalibration
try {
    [void]$process.Start()
    $stdout = $process.StandardOutput.ReadToEndAsync()
    $stderr = $process.StandardError.ReadToEndAsync()
    if (-not $process.WaitForExit(20 * 60 * 1000)) {
        $process.Kill($true)
        throw 'Native drag exceeded the repository 20-minute timeout.'
    }
    [IO.File]::WriteAllText("$output.stdout.log", $stdout.GetAwaiter().GetResult())
    [IO.File]::WriteAllText("$output.stderr.log", $stderr.GetAwaiter().GetResult())
    if ($process.ExitCode -ne 0) {
        throw "Native drag failed ($($process.ExitCode)): $([IO.File]::ReadAllText("$output.stderr.log"))"
    }
    @{ start = $clockStart; end = (Get-ClockCalibration) } | ConvertTo-Json -Depth 4 |
        Set-Content -LiteralPath "$output.clock.json" -Encoding utf8
}
finally { $process.Dispose() }
