#!/usr/bin/env bash
set -euo pipefail

doroti_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
source "$doroti_root/eng/local-storage.sh"
export DOROTI_LOCAL_ROOT="$(doroti_local_root "$doroti_root")"
temporary_root="$(new_doroti_temporary_directory "$doroti_root" "g4-2")"
trap 'remove_doroti_temporary_directory "$doroti_root" "$temporary_root"' EXIT

dotnet build "$doroti_root/src/Doroti.Flutter.Framework.Foundation/Doroti.Flutter.Framework.Foundation.csproj" --nologo
dotnet run --project "$doroti_root/validation/Doroti.Validation.G4Foundation/Doroti.Validation.G4Foundation.csproj" --no-restore

package_root="$temporary_root/packages"
mkdir -p "$package_root"
dotnet pack "$doroti_root/src/Doroti.Flutter.Runtime/Doroti.Flutter.Runtime.csproj" --configuration Debug --nologo --no-build --output "$package_root"
dotnet pack "$doroti_root/src/Doroti.Flutter.Ui/Doroti.Flutter.Ui.csproj" --configuration Debug --nologo --no-build --output "$package_root"
dotnet pack "$doroti_root/src/Doroti.Flutter.Framework.Foundation/Doroti.Flutter.Framework.Foundation.csproj" --configuration Debug --nologo --no-build --output "$package_root"

consumer_root="$temporary_root/external-consumer"
cp -R "$doroti_root/validation/generated/g4-2-foundation-package-consumer" "$consumer_root"
dotnet restore "$consumer_root/G4.Foundation.PackageConsumer.csproj" \
  --source "$package_root" \
  --packages "$temporary_root/nuget-cache" \
  --force-evaluate \
  --nologo
consumer_output="$(dotnet run --project "$consumer_root/G4.Foundation.PackageConsumer.csproj" --no-restore)"
if [[ "$consumer_output" != *"G4-2-FOUNDATION-PACKAGE-CONSUMER-PASS"* ]]; then
  printf '%s\n' "$consumer_output" >&2
  echo "The package-only Foundation consumer did not report PASS." >&2
  exit 2
fi

DOROTI_G4_2_PACKAGE_CONSUMER=pass \
  dotnet run --project "$doroti_root/validation/Doroti.Validation.G4Foundation/Doroti.Validation.G4Foundation.csproj" --no-restore

echo "G4-2 validation: PASS"
