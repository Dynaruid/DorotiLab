# Bundled Quick Gaussian shader

Normal Debug/Release builds embed `gaussian.frag.qsb` with Qt's resource compiler.
They do not require ShaderTools development files or `qsb`. CMake checks both the
source and asset hashes in `gaussian.frag.json` to reject a stale bundled shader.

After editing `gaussian.frag`, use Qt Shader Tools to regenerate the asset:

```sh
python3 bake.py --qsb /usr/lib/qt6/bin/qsb
```

Keep the source, asset and metadata together, and synchronize the Testbed and
template copies when changing the shared host. The script emits SPIR-V for the
Vulkan-only Quick backend using `--qsbversion 64`, an older serialization format
readable by Qt 6.6+. It does not use newer multiview/tessellation features.
See the [Qt QSB manual](https://doc.qt.io/qt-6/qtshadertools-qsb.html) for the
compatibility option. The recorded tool version identifies the generator; it
does not raise the runtime Qt requirement to that tool version.
