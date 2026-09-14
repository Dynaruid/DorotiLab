# Qt managed and Widgets contract

The restored `Contract.csproj` checks the actual managed C ABI sizes/offsets and
translation/clip conversion. Quick preserves fractional logical geometry;
Widgets keep integer rounding and inward clipping. Disjoint clipping disables
input, and unsupported transforms fail explicitly.

```sh
python3 Doroti/validation/run-with-timeout.py dotnet run \
  --project Doroti/validation/linux-qt-contract/Contract.csproj \
  --artifacts-path Doroti/artifacts/platform-views/2026-09-14/linux-qt/rearchitecture/contract-build
```

Pass the separately built Quick validation driver path after `--` to exercise
GPU bank retirement and rejected-frame preservation on a Qt-owned Vulkan queue.
See [Quick validation](../linux-qt-quick/README.md) for build/run commands.

The Widgets product backend is selected using `-p:DorotiQtQuick=false` (and
`-p:DorotiQtWebEngine=false` if explicitly configured). Its contract remains
disjoint native child QPushButton/QLineEdit, B only. WebEngine Quick, effects,
input shields and interleaving are not supported by Widgets. Quick runtime
results must not be used as Widgets runtime evidence. Missing historical
`record-platform-views.py` and Widgets artifacts have not been reconstructed.
