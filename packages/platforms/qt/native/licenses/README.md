# Linux native dependency notices

Doroti's own code is BSD-3-Clause. The following dependencies retain their own
licenses. These notices do not relicense those dependencies as BSD.

## Qt

Development builds dynamically link distribution Qt 6. Publication uses a reviewed
application-private Qt 6.12 shared-library bundle; see ../../qt-runtime.md. Qt Core, Gui, Widgets, OpenGL,
Network, DBus, Qml, Quick, QuickControls2 and their supporting modules are used
under LGPL-3.0 where present. Optional WebEngine, WebChannel and their supporting
modules also use the LGPL option. Copyright belongs to The Qt Company Ltd. and
the respective Qt contributors; consult the source of the installed version for
the full copyright notices. LGPL-3.0 and the incorporated GPL-3.0 text are included.

Debian 13's Qt 6.8.2 also links the shared QmlMeta support module. Its
qtdeclarative source is covered by the LGPL-3 option in the distribution's
`/usr/share/doc/libqt6qml6/copyright`; it is included in the reviewed shared-module
allowlist. That old system profile is historical; the Qt 6.12 collector supplies its own exact binary/source/notice inventory.

- Licensing: https://doc.qt.io/qt-6/licensing.html
- Source releases: https://download.qt.io/official_releases/qt/
- Source repositories: https://code.qt.io/
- Chromium/third-party notices: https://doc.qt.io/qt-6/qtwebengine-licensing.html

Published bundles redistribute reviewed Qt/Chromium runtimes with exact corresponding
source, build information and third-party notices supplied by the SDK inventory.
Users can select ABI-compatible modified shared libraries with
DOROTI_QT_FRAMEWORK_PATH and DOROTI_QT_WEBENGINE_PATH. Do not prevent
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
