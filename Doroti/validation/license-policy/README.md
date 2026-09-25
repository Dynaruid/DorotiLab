# Linux dependency license guards

Run without Qt or GStreamer development packages (CMake, Python and .NET required):

```sh
python3 Doroti/validation/license-policy/verify.py
```

On Linux x64 with GStreamer 1.24+ development packages and the core/app plugins:

```sh
python3 Doroti/validation/license-policy/verify.py --build-gstreamer
```

For an adapter built by CMake, pass `--gst-library /path/to/libdoroti_texture_gstreamer.so`
instead. Each runtime case uses a fresh process and temporary plugin/cache directory.
No display, camera or GPU is accessed.

Checks cover permitted shared Qt targets, rejected static/unreviewed modules,
aliases, transitive generator-expression links, raw Qt linker inputs, cycles,
build/publish notice copies and native/template byte parity. Runtime checks cover
missing/empty plugin directories, unreviewed files and nested directories,
pre-existing initialization, an allowed core/app pipeline and a blocked libav
element. CI runs these alongside the managed Qt contracts.

The tests do not establish the license provenance of every binary in a deployment
or qualify camera/GPU DMA-BUF transport. See [dependency policy](../../docs/license-policy.md).

## Local verification (2026-09-26)

- Full Linux Testbed Release build passed with zero warnings/errors.
- Qt 6.10.2 Quick + WebEngine and Widgets native Release builds passed.
- GStreamer 1.28.2 adapter compiled both directly and through CMake; seven runtime
  policy cases passed without accessing a camera.
- Nine Qt policy cases, notice copying/missing-text rejection, and 29-file
  app/template parity passed.
- Managed Qt ABI and Quick/Widgets geometry contracts passed.
- Native symbols retain the MIT background-effect protocol and contain no KDE
  blur protocol symbols. Qt runtime linkage uses shared libraries.

Local GStreamer development packages were extracted into a temporary directory;
no system package installation was required. Camera rendering and compositor
visual behavior were not exercised by this validation.
