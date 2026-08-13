#Requires -Version 5.1
$ErrorActionPreference = 'Stop'

$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
. (Join-Path $PSScriptRoot 'local-storage.ps1')
$temporaryRoot = New-DorotiTemporaryDirectory -DorotiRoot $dorotiRoot -Name 'g4-3'

$converterProject = "$dorotiRoot/../tools/Doroti.DartToCSharp/Doroti.DartToCSharp.csproj"
$loweringManifest = "$dorotiRoot/validation/cases/g4-3a-lowering.selection.json"
$loweringOutput = Join-Path $temporaryRoot 'g4-3a-lowering'
$candidateRoot = "$dorotiRoot/migration/generated-candidates/flutter-framework/56b8e1a851a594b1a154f8ea93270807dab22b9a/g4-3"
$candidateReport = "$candidateRoot/converter-report.json"
$candidateSolution = "$candidateRoot/Doroti.Generated.Framework.slnx"
$candidateCoverage = "$candidateRoot/framework-coverage.json"
$fullManifest = "$dorotiRoot/migration/selections/g4-3-scheduler-services.json"
$disposition = "$dorotiRoot/migration/flutter-framework/g4-3-scheduler-services-disposition.json"
$apiManifest = "$dorotiRoot/migration/flutter-framework/g4-3-api-manifest.json"

function Invoke-Checked {
    param([scriptblock]$Command)
    & $Command
    if ($LASTEXITCODE -ne 0) { throw "Command failed with exit code $LASTEXITCODE." }
}

function Get-OutputDigest {
    param([string]$Root)
    $lines = Get-ChildItem -Path $Root -Recurse -File |
        Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' } |
        Sort-Object { $_.FullName.Substring($Root.Length) } |
        ForEach-Object {
            $relative = $_.FullName.Substring($Root.Length) -replace '\\', '/'
            "$((Get-FileHash -Algorithm SHA256 $_.FullName).Hash.ToLowerInvariant())  $relative"
        }
    ($lines -join "`n").ToLowerInvariant()
}

try {
    Invoke-Checked { dotnet build $converterProject --nologo }
    Invoke-Checked {
        dotnet run --project $converterProject -- `
            --manifest $loweringManifest `
            --output $loweringOutput `
            --cache-dir (Join-Path $temporaryRoot 'analyzer-cache') `
            --parallelism 2
    }
    Invoke-Checked {
        dotnet build "$loweringOutput/Doroti.Generated.Validation.G43A.csproj" `
            -p:DorotiRepositoryRoot="$dorotiRoot" `
            --nologo
    }
    $loweringSource = Get-Content "$loweringOutput/g4-3a-lowering.g.cs" -Raw
    foreach ($requiredLowering in @(
        'this.value = value;',
        'global::System.Action<string>? localCallback',
        '.Cast<WidgetBase>().ToList()',
        'is { } __items'
    )) {
        if ($loweringSource.IndexOf($requiredLowering, [StringComparison]::Ordinal) -lt 0) {
            throw "The focused lowering fixture is missing contextual type/member evidence: $requiredLowering"
        }
    }

    $report = Get-Content $candidateReport -Raw | ConvertFrom-Json
    $reportErrors = @($report.diagnostics | Where-Object { $_.severity -eq 'error' -or $_.severity -eq 'warning' })
    if (-not $report.success -or $report.outputs.Count -ne 58 -or $reportErrors.Count -ne 0) {
        throw "The pinned G4-3 candidate report is not clean for all 58 libraries."
    }

    $coverage = Get-Content $candidateCoverage -Raw | ConvertFrom-Json
    if ($coverage.declarationCount -ne 287 -or
        $coverage.memberCount -ne 2426 -or
        $coverage.unclassifiedAstNodeCount -ne 0 -or
        $coverage.silentOmissionCount -ne 0 -or
        $coverage.generatedCompileErrorCount -ne 0) {
        throw "The pinned G4-3 framework coverage census is not closed."
    }

    $dispositionEntries = (Get-Content $disposition -Raw | ConvertFrom-Json).entries
    $nonPromoted = @($dispositionEntries | Where-Object { $_.disposition -ne 'promoted' })
    if ($dispositionEntries.Count -ne 287 -or $nonPromoted.Count -ne 0) {
        throw "The G4-3 declaration disposition is incomplete."
    }

    $manifestCounts = (Get-Content $apiManifest -Raw | ConvertFrom-Json).counts
    if ($manifestCounts.declarationOccurrences -ne 235 -or
        $manifestCounts.uniqueDeclarationNames -ne 233 -or
        $manifestCounts.publicMembers -ne 1960) {
        throw "The G4-3 public API manifest census drifted."
    }

    Invoke-Checked { dotnet build $candidateSolution -p:DorotiRepositoryRoot="$dorotiRoot" --nologo }

    $determinismA = Join-Path $temporaryRoot 'determinism-a'
    $determinismB = Join-Path $temporaryRoot 'determinism-b'
    $fullCache = if ($env:DOROTI_G4_3_ANALYZER_CACHE) { $env:DOROTI_G4_3_ANALYZER_CACHE } else { Join-Path $temporaryRoot 'full-analyzer-cache' }
    foreach ($output in @($determinismA, $determinismB)) {
        Invoke-Checked {
            dotnet run --project $converterProject --no-build -- `
                --manifest $fullManifest `
                --output $output `
                --cache-dir $fullCache `
                --parallelism 4
        }
    }
    $digestA = Get-OutputDigest -Root $determinismA
    $digestB = Get-OutputDigest -Root $determinismB
    if ($digestA -cne $digestB) {
        throw "The full G4-3 selection regeneration is not deterministic."
    }
    $freshScheduler = Get-Content (Join-Path $determinismA 'projects/Scheduler/binding.g.cs') -Raw
    $freshAutofill = Get-Content (Join-Path $determinismA 'projects/Services/autofill.g.cs') -Raw
    $freshHardwareKeyboard = Get-Content (Join-Path $determinismA 'projects/Services/hardware_keyboard.g.cs') -Raw
    foreach ($surfaceContract in @(
        @($freshScheduler, 'public virtual SchedulingStrategy schedulingStrategy'),
        @($freshAutofill, 'public AutofillConfiguration(string uniqueIdentifier, List<string> autofillHints, TextEditingValue currentEditingValue'),
        @($freshAutofill, 'public interface AutofillScopeMixin : AutofillScope'),
        @($freshHardwareKeyboard, 'public abstract class KeyEvent : Diagnosticable')
    )) {
        if ($surfaceContract[0].IndexOf($surfaceContract[1], [StringComparison]::Ordinal) -lt 0) {
            throw "The fresh G4-3 candidate is missing reviewed surface evidence: $($surfaceContract[1])"
        }
    }
    # Compile the freshly regenerated graph as well as the pinned candidate.
    # Deterministic text alone must not mask a current-lowerer aggregate break.
    Invoke-Checked {
        dotnet build "$determinismA/Doroti.Generated.Framework.slnx" `
            -p:DorotiRepositoryRoot="$dorotiRoot" `
            --nologo
    }

    Invoke-Checked { dotnet build "$dorotiRoot/src/Doroti.Host.Desktop.Flutter/Doroti.Host.Desktop.Flutter.csproj" --nologo }
    # Promotion must follow the current compiler output. The pinned candidate is
    # retained as historical compiler evidence and is deliberately not treated
    # as the current reviewed product contract.
    $env:DOROTI_G4_3_CANDIDATE_ROOT = Join-Path $determinismA 'projects'
    Invoke-Checked { dotnet run --project "$dorotiRoot/validation/Doroti.Validation.G4SchedulerServices/Doroti.Validation.G4SchedulerServices.csproj" }

    $packageRoot = Join-Path $temporaryRoot 'packages'
    New-Item -ItemType Directory -Path $packageRoot | Out-Null
    foreach ($project in @('Doroti.Flutter.Runtime', 'Doroti.Flutter.Ui', 'Doroti.Flutter.Framework.Foundation')) {
        Invoke-Checked {
            dotnet pack "$dorotiRoot/src/$project/$project.csproj" `
                --configuration Debug --nologo --no-build --output $packageRoot
        }
    }

    Invoke-Checked {
        dotnet restore $candidateSolution `
            --source $packageRoot `
            --packages (Join-Path $temporaryRoot 'candidate-nuget-cache') `
            --force-evaluate `
            --nologo
    }
    Invoke-Checked { dotnet build $candidateSolution --no-restore --nologo }

    foreach ($project in @('Doroti.Flutter.Framework.Scheduler', 'Doroti.Flutter.Framework.Services')) {
        Invoke-Checked {
            dotnet pack "$dorotiRoot/src/$project/$project.csproj" `
                --configuration Debug --nologo --no-build --output $packageRoot
        }
    }

    $consumerRoot = Join-Path $temporaryRoot 'external-consumer'
    Copy-Item -Recurse "$dorotiRoot/validation/generated/g4-3-scheduler-services-package-consumer" $consumerRoot
    Invoke-Checked {
        dotnet restore "$consumerRoot/G4.SchedulerServices.PackageConsumer.csproj" `
            --source $packageRoot `
            --packages (Join-Path $temporaryRoot 'nuget-cache') `
            --force-evaluate `
            --nologo
    }
    $consumerOutput = dotnet run --project "$consumerRoot/G4.SchedulerServices.PackageConsumer.csproj" --no-restore | Out-String
    if ($LASTEXITCODE -ne 0 -or $consumerOutput -notlike '*G4-3-SCHEDULER-SERVICES-PACKAGE-CONSUMER-PASS*') {
        Write-Error $consumerOutput
        throw "The package-only Scheduler/Services consumer did not report PASS."
    }

    Invoke-Checked { dotnet build "$dorotiRoot/Doroti.slnx" --nologo }
    Invoke-Checked { & "$dorotiRoot/eng/validate-g4-boundary.ps1" }
    Invoke-Checked { & "$dorotiRoot/eng/validate-g4-1.ps1" }
    Invoke-Checked { & "$dorotiRoot/eng/validate-g4-2.ps1" }

    Write-Output "G4-3A candidate, promotion, package, and regression validation: PASS"
}
finally {
    Remove-DorotiTemporaryItem -DorotiRoot $dorotiRoot -Path $temporaryRoot
}
