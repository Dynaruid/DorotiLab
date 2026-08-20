#!/usr/bin/env bash
set -euo pipefail

script_dir=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)
repo_root=$(cd -- "$script_dir/../.." && pwd)
configuration=${1:-Release}
workspace=$(mktemp -d)
trap 'rm -rf -- "$workspace"' EXIT

cd "$repo_root"
dotnet run --project Doroti/validation/linux-qt-contract/Doroti.Validation.LinuxQtContract.csproj \
  -c "$configuration"

cmake -S Doroti/templates/Doroti.Templates/content/doroti-app/linux/native \
  -B "$workspace/template-native" -DCMAKE_BUILD_TYPE="$configuration"
cmake --build "$workspace/template-native" --parallel
test -f "$workspace/template-native/libdoroti_qt_host.so"

dotnet build DorotiDemoApp/linux/DorotiDemoApp.Linux.csproj \
  -c "$configuration" -r linux-x64
dotnet publish DorotiDemoApp/linux/DorotiDemoApp.Linux.csproj \
  -c "$configuration" -r linux-x64 --self-contained false -o "$workspace/fdd" --no-restore
dotnet publish DorotiDemoApp/linux/DorotiDemoApp.Linux.csproj \
  -c "$configuration" -r linux-x64 --self-contained true -o "$workspace/sc" --no-restore

for output in "$workspace/fdd" "$workspace/sc"; do
  test -f "$output/libdoroti_qt_host.so"
  test -f "$output/libSkiaSharp.so"
  test -f "$output/Doroti.Host.Qt.dll"
  test -f "$output/Doroti.Skia.Rendering.dll"
  if readelf -d "$output/libdoroti_qt_host.so" | grep -Eq 'RPATH|RUNPATH'; then
    echo "Linux Qt validation rejected an embedded RPATH/RUNPATH: $output" >&2
    exit 1
  fi
done

if [[ ${DOROTI_QT_LIVE:-0} == 1 ]]; then
  for qpa in wayland xcb; do
    (
      cd "$workspace/fdd"
      QT_QPA_PLATFORM=$qpa \
      DOROTI_QT_VALIDATION_RESIZE_CYCLES=${DOROTI_QT_RESIZE_CYCLES:-20} \
      timeout 120s dotnet DorotiDemoApp.Linux.dll
    )
  done
fi

echo "Doroti Linux Qt validation: PASS ($configuration)"
