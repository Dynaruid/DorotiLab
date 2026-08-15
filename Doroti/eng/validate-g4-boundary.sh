#!/usr/bin/env bash
set -euo pipefail

doroti_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
workspace_root="$(cd "$doroti_root/.." && pwd)"
source "$doroti_root/eng/local-storage.sh"
export DOROTI_LOCAL_ROOT="$(doroti_local_root "$doroti_root")"
temporary_root="$(new_doroti_temporary_directory "$doroti_root" "g4-boundary")"
trap 'remove_doroti_temporary_directory "$doroti_root" "$temporary_root"' EXIT

cd "$workspace_root/tools/Doroti.DartToCSharp/analyzer"
dart run tool/boundary/extract_g4_boundary.dart \
  "$workspace_root/reference/flutter-master/packages/flutter/lib" \
  "$doroti_root/migration/flutter-compat/flutter-api.json" \
  "$temporary_root/source-boundary.json"
cmp "$temporary_root/source-boundary.json" "$doroti_root/migration/flutter-avalonia/source-boundary.json"

cd "$doroti_root"
dotnet build tools/Doroti.SourceTools/Doroti.SourceTools.csproj --nologo
dotnet tools/Doroti.SourceTools/bin/Debug/net10.0/Doroti.SourceTools.dll \
  flutter-avalonia-boundary-audit \
  --output "$doroti_root/artifacts/flutter-avalonia/g4-0-boundary-audit.json"

fixture_log="$temporary_root/forbidden-fixture.log"
if dotnet build validation/architecture/forbidden-framework-platform/Doroti.Framework.ForbiddenFixture.csproj \
  --nologo >"$fixture_log" 2>&1; then
  echo "The forbidden Flutter -> Platform fixture unexpectedly built successfully." >&2
  exit 2
fi
if ! grep -q 'DOTARCH009' "$fixture_log"; then
  cat "$fixture_log" >&2
  echo "The forbidden fixture failed without DOTARCH009." >&2
  exit 2
fi

echo "G4-0 boundary validation: PASS"
