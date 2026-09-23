param(
    [Parameter(Mandatory = $true)][string]$AssemblyPath,
    [Parameter(Mandatory = $true)][string]$OutputPath,
    [string]$Revision = '<working-tree>'
)

$ErrorActionPreference = 'Stop'
$assemblyFile = (Resolve-Path -LiteralPath $AssemblyPath).Path
$packageRoot = Join-Path $env:USERPROFILE '.nuget/packages/materialcolorutilities'
$materialDependency = Get-ChildItem -LiteralPath $packageRoot -Filter MaterialColorUtilities.dll -Recurse |
    Where-Object { $_.FullName -match '[\\/]lib[\\/]net6\.0[\\/]' } |
    Sort-Object FullName -Descending |
    Select-Object -First 1
if ($null -eq $materialDependency) {
    throw 'MaterialColorUtilities runtime dependency was not found in the NuGet cache.'
}
[System.Reflection.Assembly]::LoadFrom($materialDependency.FullName) | Out-Null
$assembly = [System.Reflection.Assembly]::LoadFrom($assemblyFile)
$flags = [System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static,DeclaredOnly'

function Test-VisibleType([type]$type) {
    return $type.IsPublic -or $type.IsNotPublic -or $type.IsNestedPublic -or $type.IsNestedAssembly
}

function Test-VisibleMember([System.Reflection.MemberInfo]$member) {
    if ($member -is [System.Reflection.MethodBase]) {
        return $member.IsPublic -or $member.IsAssembly -or $member.IsFamilyOrAssembly
    }
    if ($member -is [System.Reflection.FieldInfo]) {
        return $member.IsPublic -or $member.IsAssembly -or $member.IsFamilyOrAssembly
    }
    if ($member -is [System.Reflection.PropertyInfo]) {
        return @($member.GetAccessors($true) | Where-Object { Test-VisibleMember $_ }).Count -gt 0
    }
    if ($member -is [System.Reflection.EventInfo]) {
        return Test-VisibleMember $member.GetAddMethod($true)
    }
    return $false
}

$lines = [System.Collections.Generic.List[string]]::new()
$lines.Add("# Doroti.Runtime API snapshot: $Revision")
$lines.Add('# Includes public and internal declared types and members; compiler-generated private types are excluded.')
foreach ($type in @($assembly.GetTypes() | Where-Object {
    $_.Namespace -eq 'Doroti.Runtime' -and (Test-VisibleType $_)
} | Sort-Object FullName)) {
    $lines.Add("TYPE $($type.FullName)")
    foreach ($member in @($type.GetMembers($flags) | Where-Object {
        Test-VisibleMember $_
    } | Sort-Object { $_.ToString() })) {
        $lines.Add("  $($member.MemberType) $member")
    }
}
$outputFile = [System.IO.Path]::GetFullPath($OutputPath)
[System.IO.Directory]::CreateDirectory([System.IO.Path]::GetDirectoryName($outputFile)) | Out-Null
[System.IO.File]::WriteAllLines($outputFile, $lines)
Write-Output "$($lines.Count) API lines written to $outputFile"
