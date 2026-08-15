#!/usr/bin/env bash
set -euo pipefail

doroti_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
source "$doroti_root/eng/local-storage.sh"
export DOROTI_LOCAL_ROOT="$(doroti_local_root "$doroti_root")"
temporary_root="$(new_doroti_temporary_directory "$doroti_root" "g4-3")"
trap 'remove_doroti_temporary_directory "$doroti_root" "$temporary_root"' EXIT

converter_project="$doroti_root/../tools/Doroti.DartToCSharp/Doroti.DartToCSharp.csproj"
lowering_manifest="$doroti_root/validation/cases/g4-3a-lowering.selection.json"
lowering_output="$temporary_root/g4-3a-lowering"
candidate_report="$doroti_root/migration/generated-candidates/flutter-framework/56b8e1a851a594b1a154f8ea93270807dab22b9a/g4-3/converter-report.json"
candidate_root="$doroti_root/migration/generated-candidates/flutter-framework/56b8e1a851a594b1a154f8ea93270807dab22b9a/g4-3"
candidate_solution="$candidate_root/Doroti.Generated.Framework.slnx"
candidate_coverage="$candidate_root/framework-coverage.json"
full_manifest="$doroti_root/migration/selections/g4-3-scheduler-services.json"
disposition="$doroti_root/migration/flutter-framework/g4-3-scheduler-services-disposition.json"
api_manifest="$doroti_root/migration/flutter-framework/g4-3-api-manifest.json"

dotnet build "$converter_project" --nologo
dotnet run --project "$converter_project" -- \
  --manifest "$lowering_manifest" \
  --output "$lowering_output" \
  --cache-dir "$temporary_root/analyzer-cache" \
  --parallelism 2
dotnet build "$lowering_output/Doroti.Generated.Validation.G43A.csproj" \
  -p:DorotiRepositoryRoot="$doroti_root" \
  --nologo

if ! jq -e '
  .success == true and
  (.outputs | length) == 58 and
  ([.diagnostics[] | select(.severity == "error" or .severity == "warning")] | length) == 0
' "$candidate_report" >/dev/null; then
  echo "The pinned G4-3 candidate report is not clean for all 58 libraries." >&2
  exit 2
fi

if ! jq -e '
  .declarationCount == 287 and
  .memberCount == 2426 and
  .unclassifiedAstNodeCount == 0 and
  .silentOmissionCount == 0 and
  .generatedCompileErrorCount == 0
' "$candidate_coverage" >/dev/null; then
  echo "The pinned G4-3 framework coverage census is not closed." >&2
  exit 2
fi
if ! jq -e '(.entries | length) == 287 and ([.entries[] | select(.disposition != "promoted")] | length) == 0' "$disposition" >/dev/null; then
  echo "The G4-3 declaration disposition is incomplete." >&2
  exit 2
fi
if ! jq -e '.counts.declarationOccurrences == 235 and .counts.uniqueDeclarationNames == 233 and .counts.publicMembers == 1960' "$api_manifest" >/dev/null; then
  echo "The G4-3 public API manifest census drifted." >&2
  exit 2
fi

dotnet build "$candidate_solution" -p:DorotiRepositoryRoot="$doroti_root" --nologo

determinism_a="$temporary_root/determinism-a"
determinism_b="$temporary_root/determinism-b"
full_cache="${DOROTI_G4_3_ANALYZER_CACHE:-$temporary_root/full-analyzer-cache}"
for output in "$determinism_a" "$determinism_b"; do
  dotnet run --project "$converter_project" --no-build -- \
    --manifest "$full_manifest" \
    --output "$output" \
    --cache-dir "$full_cache" \
    --parallelism 4
done
(
  cd "$determinism_a"
  find . -type f ! -path '*/bin/*' ! -path '*/obj/*' -print0 | sort -z | xargs -0 shasum -a 256
) >"$temporary_root/determinism-a.sha256"
(
  cd "$determinism_b"
  find . -type f ! -path '*/bin/*' ! -path '*/obj/*' -print0 | sort -z | xargs -0 shasum -a 256
) >"$temporary_root/determinism-b.sha256"
cmp "$temporary_root/determinism-a.sha256" "$temporary_root/determinism-b.sha256"

dotnet build "$doroti_root/src/Doroti.Host.Desktop.Framework/Doroti.Host.Desktop.Framework.csproj" --nologo
dotnet run --project "$doroti_root/validation/Doroti.Validation.G4SchedulerServices/Doroti.Validation.G4SchedulerServices.csproj" --no-restore

package_root="$temporary_root/packages"
mkdir -p "$package_root"
for project in \
  Doroti.Runtime \
  Doroti.Ui \
  Doroti.Framework.Foundation
do
  dotnet pack "$doroti_root/src/$project/$project.csproj" \
    --configuration Debug --nologo --no-build --output "$package_root"
done

dotnet restore "$candidate_solution" \
  --source "$package_root" \
  --packages "$temporary_root/candidate-nuget-cache" \
  --force-evaluate \
  --nologo
dotnet build "$candidate_solution" --no-restore --nologo

for project in Doroti.Framework.Scheduler Doroti.Framework.Services; do
  dotnet pack "$doroti_root/src/$project/$project.csproj" \
    --configuration Debug --nologo --no-build --output "$package_root"
done

consumer_root="$temporary_root/external-consumer"
cp -R "$doroti_root/validation/generated/g4-3-scheduler-services-package-consumer" "$consumer_root"
dotnet restore "$consumer_root/G4.SchedulerServices.PackageConsumer.csproj" \
  --source "$package_root" \
  --packages "$temporary_root/nuget-cache" \
  --force-evaluate \
  --nologo
consumer_output="$(dotnet run --project "$consumer_root/G4.SchedulerServices.PackageConsumer.csproj" --no-restore)"
if [[ "$consumer_output" != *"G4-3-SCHEDULER-SERVICES-PACKAGE-CONSUMER-PASS"* ]]; then
  printf '%s\n' "$consumer_output" >&2
  echo "The package-only Scheduler/Services consumer did not report PASS." >&2
  exit 2
fi

dotnet build "$doroti_root/Doroti.slnx" --nologo
"$doroti_root/eng/validate-g4-boundary.sh"
"$doroti_root/eng/validate-g4-1.sh"
"$doroti_root/eng/validate-g4-2.sh"

echo "G4-3A candidate, promotion, package, and regression validation: PASS"
