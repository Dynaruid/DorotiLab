# Dependency license policy

Doroti's own code remains BSD-3-Clause. Do not add runtime dependencies or copied
code that require licensing the application under GPL or AGPL. A permissive or
LGPL alternative must actually be available and selected for the files/build in
use; seeing `OR` in an unrelated package license is not sufficient.

LGPL dependencies are allowed with compliance: prefer replaceable shared
libraries, preserve notices/license texts, provide corresponding source when
required (including modifications), and allow debugging modifications to the
LGPL components. Do not paste LGPL implementation code into BSD source files.
Static LGPL linkage requires a separate review of relinking/source obligations;
the standard Linux Qt build rejects it. A C ABI, P/Invoke or dynamic loading does
not by itself avoid GPL obligations.

## Enforced Linux safeguards

- `linux/native/cmake/DorotiLicensePolicy.cmake` checks the linked CMake target
  graph against reviewed LGPL-capable Qt modules and rejects static Qt runtimes.
- The legacy LGPL KDE blur XML/generated code is removed from the app and
  template. Only the MIT ext-background-effect-v1 protocol is generated.
- GStreamer remains optional and disabled by default. Its adapter validates a
  dedicated plugin directory before discovery, limits plugin names/search paths,
  and rejects unexpected plugin names or licenses before accepting a pipeline.
  The list intentionally excludes automatic playback and libav/x264/x265.
- The runner copies Linux notices/license texts to build and publish output;
  CMake installation includes them too. App and template must remain identical.
- CI exercises the Qt allowlist's rejection paths and checks native/template
  parity. These checks are regression guards, not a complete license scanner.

## Dependency and release review

For new/upgraded NuGet packages, Qt modules, native libraries and plugins, record
the version, origin, selected license and transitive dependencies. Inspect the
actual archive/build, not only its top-level license. Check both linked libraries
and dynamically loaded QML/platform/media plugins. Resolve unknown licenses
before distribution. Do not add raw Qt linker flags or unreviewed runtime QML
imports to bypass the CMake policy.

Before a binary release, inventory the final shipped files (SBOM), review their
licenses, collect notices and exact corresponding source/build materials where
required, and verify users can replace LGPL libraries. Re-review distributor
builds of multimedia libraries: an LGPL plugin can link a GPL-enabled FFmpeg or
another restricted codec. System-library use is not a blanket licensing exemption.

SkiaSharp's retained native notice file includes alternative licenses and a
libmicrohttpd notice. Its presence alone does not prove that code is linked into
every platform asset. Verify each shipped native asset and its build configuration;
do not mark all SkiaSharp transitive dependencies as MIT without that evidence.

Development tools and their output must be assessed separately from runtime
libraries. A GPL tool does not automatically license its output under GPL, but
copied/generated implementation code and tool exceptions must be checked.

See the app/template's `linux/native/licenses/README.md` and
[third-party notices](../THIRD-PARTY-NOTICES.md) for distribution details.
