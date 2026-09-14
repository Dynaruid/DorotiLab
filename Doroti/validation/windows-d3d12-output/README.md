# Windows Vulkan/D3D12 output gate

Build `DorotiTestbedApp/windowsappsdk/DorotiTestbedApp.WindowsAppSdk.csproj` in
Release, then run from the repository root with the required 20-minute timeout:

```powershell
python -c "import subprocess; subprocess.run(['python','Doroti/validation/windows-d3d12-output/verify.py'],timeout=1200,check=True)"
python -c "import subprocess; subprocess.run(['python','Doroti/validation/windows-d3d12-output/verify.py','--gpu','HighPerformancePreference'],timeout=1200,check=True)"
```

Requires Windows, a hardware Vulkan 1.2/D3D12 adapter, the D3D12 debug layer,
Python, and Pillow. The script temporarily raises its own Testbed window and
uses synthetic HWND resize/lifecycle messages. It verifies visible nonuniform
content, shared-source copy completion, DXGI capacity growth, three prepared
moving-origin commits with matching present-count receipts, minimize/restore,
one device reset, and two successfully drained/destroyed output owners.

`--vulkan-validation` additionally enables the Khronos validation layer and
requires zero Vulkan validation errors and evidence that the writable
depth/stencil barrier correction was exercised. Strict runs pass on AMD Radeon
780M and NVIDIA RTX 4060 Laptop after the destination-stage correction; the
earlier failing reports remain preserved in the artifacts directory.

Outputs go to `Doroti/artifacts/validation/windows-d3d12-output/`. The product
report is taken before presenter disposal, so its active DXGI swapchain count
is one; the debug teardown records and process exit verify subsequent disposal.
The Acrylic/PlatformView alpha and interaction gate remains
`../windows-acrylic-composition/verify.py`.

Physical input/IME, smoothness, mixed DPI, exact scan-out atomicity, and before/
after CPU/input performance remain `notVerified`. Vulkan frame completion still
uses its existing CPU fence wait, and DXGI adds an output GPU copy; this change
does not establish a performance improvement.
