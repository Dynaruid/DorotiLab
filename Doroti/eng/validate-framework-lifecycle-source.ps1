[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$dorotiRoot = Split-Path -Parent $PSScriptRoot
$sourceRoot = Join-Path $dorotiRoot 'src'
$frameworkFiles = Get-ChildItem -LiteralPath $sourceRoot -Directory -Filter 'Doroti.Framework.*' |
    ForEach-Object { Get-ChildItem -LiteralPath $_.FullName -File -Filter '*.cs' }

$rules = @(
    [pscustomobject]@{
        Name = 'direct removeListener lambda'
        Pattern = '\.removeListener\s*\(\s*(?:\(global::System\.Action\)\s*)?(?:\(\s*\)\s*=>|\(\s*\(\s*\)\s*=>)'
        PathPattern = $null
    },
    [pscustomobject]@{
        Name = 'nullable callback made non-null'
        Pattern = '\b(?<callback>on[A-Za-z0-9_]+)\s*:\s*\(\s*\)\s*=>\s*\k<callback>\s*\('
        PathPattern = 'adaptive_text_selection_toolbar\.cs$'
    }
)

$failures = [System.Collections.Generic.List[string]]::new()
foreach ($file in $frameworkFiles) {
    $source = [System.IO.File]::ReadAllText($file.FullName)
    foreach ($rule in $rules) {
        if ($null -ne $rule.PathPattern -and $file.FullName -notmatch $rule.PathPattern) {
            continue
        }

        foreach ($match in [regex]::Matches($source, $rule.Pattern, [Text.RegularExpressions.RegexOptions]::Singleline)) {
            $line = 1 + ($source.Substring(0, $match.Index) -split "`n").Count - 1
            $relativePath = [IO.Path]::GetRelativePath($dorotiRoot, $file.FullName)
            $failures.Add("$relativePath`:$line`: $($rule.Name)")
        }
    }
}

if ($failures.Count -gt 0) {
    $failures | Sort-Object | ForEach-Object { [Console]::Error.WriteLine($_) }
    throw "Framework lifecycle source validation failed with $($failures.Count) finding(s)."
}

Write-Output "Framework lifecycle source validation: PASS ($($frameworkFiles.Count) files)"
