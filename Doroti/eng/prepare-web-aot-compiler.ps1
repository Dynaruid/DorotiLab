[CmdletBinding()]
param(
    [Parameter(Mandatory)] [string] $SourcePath,
    [ValidateRange(16,128)] [int] $StackMegabytes = 64
)
$ErrorActionPreference = 'Stop'
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$outputRoot = Join-Path $repositoryRoot '.doroti/tools'
[IO.Directory]::CreateDirectory($outputRoot) | Out-Null
$source = [IO.Path]::GetFullPath($SourcePath)
$bytes = [IO.File]::ReadAllBytes($source)
$pe = [BitConverter]::ToInt32($bytes, 0x3c)
if ($pe -lt 0 -or $pe + 112 -gt $bytes.Length -or [BitConverter]::ToUInt32($bytes,$pe) -ne 0x4550 -or [BitConverter]::ToUInt16($bytes,$pe+24) -ne 0x20b) {
    throw 'Expected a valid PE32+ compiler executable.'
}
$offset = $pe + 24 + 72
$previous = [BitConverter]::ToUInt64($bytes,$offset)
$next = [uint64]$StackMegabytes * 1024 * 1024
if ($next -le $previous) { throw 'Requested stack reserve must exceed the source reserve.' }
[BitConverter]::GetBytes($next).CopyTo($bytes,$offset)
$output = Join-Path $outputRoot "mono-aot-cross-stack$StackMegabytes.exe"
if ([IO.Path]::GetFullPath($output) -eq $source) { throw 'Never modify the installed compiler.' }
[IO.File]::WriteAllBytes($output,$bytes)
$manifest = @{ source=$source; sourceSha256=(Get-FileHash -LiteralPath $source).Hash;
    compilerPath=$output; compilerSha256=(Get-FileHash -LiteralPath $output).Hash;
    originalStackReserveBytes=$previous; stackReserveBytes=$next;
    modification='Only PE32+ SizeOfStackReserve; installed SDK unchanged' }
$manifest | ConvertTo-Json | Set-Content -LiteralPath "$output.json"
$manifest
