[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string] $TitleToken,

    [Parameter(Mandatory)]
    [string] $BoundsBase64,

    [ValidateRange(1, 1000)]
    [int] $IntervalMilliseconds = 12,

    [ValidateRange(0, 10000)]
    [int] $StartDelayMilliseconds = 250
)

$ErrorActionPreference = 'Stop'

Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;
using System.Text;

public static class DorotiNativeWindowResize
{
    public delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr parameter);

    [DllImport("user32.dll")]
    public static extern bool EnumWindows(EnumWindowsProc callback, IntPtr parameter);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int GetWindowText(IntPtr hwnd, StringBuilder text, int maximumCount);

    [DllImport("user32.dll")]
    public static extern bool IsWindowVisible(IntPtr hwnd);

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hwnd, int command);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool SetWindowPos(
        IntPtr hwnd, IntPtr insertAfter, int x, int y, int width, int height, uint flags);
}
'@

$decoded = [Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($BoundsBase64))
$bounds = @($decoded | ConvertFrom-Json)
if ($bounds.Count -eq 0) { throw 'Native resize sequence is empty.' }

$window = [IntPtr]::Zero
$callback = [DorotiNativeWindowResize+EnumWindowsProc]{
    param([IntPtr] $candidate, [IntPtr] $parameter)
    if (-not [DorotiNativeWindowResize]::IsWindowVisible($candidate)) { return $true }
    $text = [Text.StringBuilder]::new(512)
    [void][DorotiNativeWindowResize]::GetWindowText($candidate, $text, $text.Capacity)
    if ($text.ToString().Contains($TitleToken, [StringComparison]::Ordinal)) {
        $script:window = $candidate
        return $false
    }
    return $true
}
[void][DorotiNativeWindowResize]::EnumWindows($callback, [IntPtr]::Zero)
if ($window -eq [IntPtr]::Zero) { throw "Chrome window containing '$TitleToken' was not found." }

if ($StartDelayMilliseconds -gt 0) { Start-Sleep -Milliseconds $StartDelayMilliseconds }
[void][DorotiNativeWindowResize]::ShowWindow($window, 9)
$flags = 0x0004 -bor 0x0010 # SWP_NOZORDER | SWP_NOACTIVATE
foreach ($value in $bounds) {
    $ok = [DorotiNativeWindowResize]::SetWindowPos(
        $window, [IntPtr]::Zero,
        [int]$value.x, [int]$value.y, [int]$value.width, [int]$value.height,
        [uint32]$flags)
    if (-not $ok) {
        throw "SetWindowPos failed with Win32 error $([Runtime.InteropServices.Marshal]::GetLastWin32Error())."
    }
    Start-Sleep -Milliseconds $IntervalMilliseconds
}
