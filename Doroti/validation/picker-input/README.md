# Picker input regression

Run from the repository root (each command has the required 20-minute deadline):

```powershell
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/picker-input/PickerInput.csproj -c Release
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/picker-input/PickerInput.csproj -c Release --no-build -- --portrait
```

The executable mounts the production Material widgets and renders with Skia using
the Testbed fonts. It checks default/explicit handwriting options, date and 24-hour
time entry mode round trips, edited values, invalid input, and input-only modes.
Any framework error or ErrorWidget fails the run. PNGs are written to `snapshots`
under the executable output directory for visual inspection.

The original regression threw the former null-assertion error while building a
TextFormField with an omitted `stylusHandwritingEnabled` argument. Its builder
must apply the EditableText default while preserving explicit true/false values.

The host supplies deterministic frame, clipboard, and input services. This checks
framework build/layout/raster behavior; it does not exercise native OS keyboard,
mouse, IME, or physical device presentation.
