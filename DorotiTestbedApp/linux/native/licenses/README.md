# Linux native dependency notices

Doroti's own code is BSD-3-Clause. The following dependencies retain their own
licenses. These notices do not relicense those dependencies as BSD.

## Qt

The Linux host dynamically links system Qt 6. Qt Core, Gui, Widgets, OpenGL,
Network, DBus, Qml, Quick, QuickControls2 and their supporting modules are used
under LGPL-3.0 where present. Optional WebEngine, WebChannel and their supporting
modules also use the LGPL option. Copyright belongs to The Qt Company Ltd. and
the respective Qt contributors; consult the source of the installed version for
the full copyright notices. LGPL-3.0 and the incorporated GPL-3.0 text are included.

- Licensing: https://doc.qt.io/qt-6/licensing.html
- Source releases: https://download.qt.io/official_releases/qt/
- Source repositories: https://code.qt.io/
- Chromium/third-party notices: https://doc.qt.io/qt-6/qtwebengine-licensing.html

This build uses system libraries and does not redistribute Qt/Chromium. Users
can replace them with ABI-compatible modified shared libraries. Do not prevent
that replacement, or reverse engineering for debugging modifications to the
LGPL components, through application terms or technical restrictions.

If redistributing Qt, supply the applicable notices and corresponding source
(including distribution patches and build scripts) using a license-compliant
method. The general upstream links above are reference links, not a substitute
for providing the exact corresponding source of redistributed binaries. Include
the actual version's Chromium/Qt third-party notices and any required installation
information. A commercial Qt license does not waive third-party obligations.

## GStreamer (only when the optional adapter is enabled)

GStreamer core and the selected raw camera/GPU plugins are used under
LGPL-2.1-or-later. Copyright belongs to the GStreamer developers and individual
contributors as recorded in the matching source release. LGPL-2.1 is included.

- Licensing: https://gstreamer.freedesktop.org/documentation/application-development/appendix/licensing.html
- Sources and releases: https://gstreamer.freedesktop.org/src/

Review each actual plugin binary and every linked dependency. The adapter's
allowlist and metadata checks do not establish that a vendor's build excludes
GPL codec libraries. When redistributing libraries/plugins, retain their notices,
provide their corresponding source as required and permit replacement with
compatible modified libraries. Keep the reviewed plugin directory immutable
while the process runs.

## Wayland

The vendored ext-background-effect-v1 protocol and generated client code are MIT
licensed; its copyright and permission text are in Wayland-background-effect-MIT.txt.
The legacy LGPL KDE blur XML and generated code are no longer included.
The system Wayland client library is MIT licensed; preserve its own notices if
redistributing it.

## Other native dependencies

The notices supplied with SkiaSharp/native assets, graphics drivers and system
libraries remain applicable. This directory is not a complete SBOM of an OS or
of a self-contained distribution. Check the final shipped files, QML/platform
plugins and their transitive dependencies before distribution.
