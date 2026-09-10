# Material sample P0 inventory — 2026-09-06

Baseline: repository `48343b7c8511c040faba8704e59aa32a80062374`, local
`reference/flutter_sample_app/lib/main.dart` and `lib/src`. SDK resolved by the
sample's package config: `C:/Users/parti/flutter`, revision
`6b182d2c7585eba26d4edce0f97630effd256c33` (SDK `pubspec.lock` locally modified).
This is the original source inventory, not a blanket runtime acceptance. Integration results were recorded in the historical Material sample plan's `work.md` section 9 (2026-09-06); that reference does not describe the later Graphite plan.
The initial blocker table below is historical. All four screens and six component groups
now have C# implementations in `DorotiTestbedApp/src/MaterialSample`; the implementation
and validation sections below supersede the initial "blocked" and "notVerified" states.

`Doroti/eng/snapshot-material-sample.ps1 -FetchImages` records source, tests,
pubspec/lock, license, font, SDK SearchAnchor/ColorScheme and color-utilities hashes
in `.doroti/evidence/material-sample-p0/reference-inputs.json`. The six PNG inputs
are downloaded there for validation only. They are not added to the app bundle.
Actual browser CORS, font selection and image decode remain separate checks.
The captured manifest and Dart oracle output are also preserved in
[`baselines/material-sample-p0/`](baselines/material-sample-p0/reference-inputs.json).

## Screen and theme API mapping

| Source | Content/state/callback contract | Doroti API / owner |
| --- | --- | --- |
| main.dart | Initial system brightness; manual brightness toggle; selected seed/image; rebuild current theme | StatefulWidget/State, ThemeData.Create, ColorScheme.CreateFromSeed; app state owns selection revisions |
| constants.dart | 9 seeds: M3 Baseline, Indigo, Blue, Teal, Green, Yellow, Orange, Deep Orange, Pink; 6 images: Leaves, Peonies, Bubbles, Seaweed, Sea Grapes, Petals | Ui.Color, Material.Colors, Painting.NetworkImage; image themes blocked by D01–D03 |
| home.dart, navigation/bar/rail/one_two_transition.dart | Components/Color/Typography/Elevation; bar at width ≤1000, rail at >1000, extended at >1500; settings scroll at height ≤740; reversible controller and disposal | LayoutBuilder, NavigationBar/Rail, AnimationController, CurvedAnimation, AnimatedBuilder; app composition |
| component_screen.dart: First/SecondComponentList, BuildSlivers, _CacheHeight | 1/2 lists, independent scroll positions; indexed visible-section measurement, estimated unknown heights, visited State retained | CustomScrollView with SectionList/SectionExtentIndex; stable GlobalKey owners, metric invalidation, parked right column during narrow layouts |
| color_palettes_screen.dart, scheme.dart, color_box.dart | Both light/dark schemes from selected primary; content <500 role chips, otherwise width-902 preview fitted to available size; dynamic_color link | ColorScheme.CreateFromSeed, Wrap, FittedBox; URL service D05 |
| typography_screen.dart | Display/Headline/Title/Label/Body × Large/Medium/Small, onSurface | Theme.of(context).textTheme, Text and actual style |
| elevation_screen.dart | tint / tint+shadow / shadow × 0,1,3,6,8,12 dp; 0,5,8,11,12,14%; 3 columns below content 450, otherwise 6 | SliverLayoutBuilder, Material elevation/surfaceTintColor/shadowColor |
| expanded_*_action.dart, buttons.dart | Selected checkmarks; brightness/seed/image controls; update theme in both compact and expanded placements | IconButton, PopupMenuButton, menu entries and app-owned selection callbacks |

## Components: complete group inventory

All names below refer to classes in `lib/src/component_screen.dart`. The listed
callbacks are behavior requirements, not claims that widget class presence proves
the behavior. Reference empty callbacks stay display examples.

| Group / source class | States and callback effects | Doroti public API |
| --- | --- | --- |
| Actions / Buttons, ButtonsWithoutIcon, ButtonsWithIcon | Fixed enabled/disabled/icon columns; Elevated/Filled/tonal/Outlined/Text, with and without icon; display button callbacks empty | ElevatedButton, FilledButton/CreateTonal, OutlinedButton, TextButton, icon factories |
| Actions / FloatingActionButtons | small/default/large/extended; display callbacks | FloatingActionButton factories |
| Actions / IconToggleButtons | Four independent selected flags; standard/filled/tonal/outlined, disabled variants | IconButton, isSelected/onPressed |
| Actions / SegmentedButtons, SingleChoice | day/week/month/year single selection, default day; update selected set | SegmentedButton<T>, ButtonSegment<T> |
| Actions / MultipleChoice | XS/S/M/L/XL, initial L+XL; update multiple selected set | SegmentedButton<T>, multiSelectionEnabled |
| Communication / NavigationBars badge variant | Notification badge destinations and selected index; select updates state | Badge, NavigationBar, NavigationDestination |
| Communication / ProgressIndicators | play/stop flag; circular/linear determinate and indeterminate variants | CircularProgressIndicator, LinearProgressIndicator, IconButton |
| Communication / SnackBarSection | Show snackbar; Close action dismisses through messenger lifecycle | ScaffoldMessenger.of, SnackBar, SnackBarAction |
| Containment / BottomSheetSection | Modal and non-modal sheets; open-state flag disables duplicate persistent sheet; closed Future restores state | showModalBottomSheet, ScaffoldState.showBottomSheet, PersistentBottomSheetController |
| Containment / Cards | Elevated/filled/outlined, icon+text content, empty action callbacks | Card/CreateFilled/CreateOutlined |
| Containment / Carousels | Two 20-item lists, distinct itemExtent/shrinkExtent and snapping behavior | CarouselView |
| Containment / Dialogs | Normal and fullscreen overlays; close/cancel/confirm buttons pop route | showDialog, AlertDialog, Dialog.CreateFullscreen, Navigator |
| Containment / Dividers | Horizontal/vertical presentation | Divider, VerticalDivider |
| Navigation / BottomAppBars | Bottom app bar with FAB and display icon callbacks | BottomAppBar, Scaffold, FloatingActionButtonLocation |
| Navigation / NavigationBars | Standalone selected index and app callback when not example mode | NavigationBar, NavigationDestination |
| Navigation / NavigationDrawers, NavigationDrawerSection | Open modal end drawer; drawer index updates independently | ScaffoldState.openEndDrawer, NavigationDrawer, NavigationDrawerDestination |
| Navigation / NavigationRails, NavigationRailSection | Rail index updates; display action callback | NavigationRail, NavigationRailDestination |
| Navigation / Tabs | Three icon/text tabs; TabController initialization/disposal | TabBar, TabBarView, TabController |
| Navigation / SearchAnchors | Empty history message; case-sensitive substring color suggestions; fill-arrow sets text+collapsed selection; selection closes view and prepends history capped at 5; last selected label | SearchAnchor.CreateBar, SearchController, ListTile; D04 |
| Navigation / TopAppBars | Standard, center-aligned, medium, large top bars; display callbacks | AppBar, SliverAppBar factories |
| Selection / Checkboxes | Initial true/null/false and disabled; tri-state and list-tile callbacks update state | Checkbox, CheckboxListTile |
| Selection / Radios | Group value, enabled and disabled choices; group callback selects value | RadioGroup<T>, Radio<T> |
| Selection / Switches, SwitchRow | Two values per enabled row, thumb-icon variation; disabled rows unchanged | Switch, WidgetStateProperty |
| Selection / Chips | Assist/filter/input/suggestion and disabled variants; filter updates isFiltered | ActionChip, FilterChip, InputChip; reference InputChip onDeleted is empty, no actual deletion in this pinned app |
| Selection / DatePicker | Open picker bounded from current year−2 through current year+1; accepted result updates selectedDate, cancellation retains it | showDatePicker |
| Selection / TimePicker | Open picker; accepted result updates selectedTime, cancellation retains it | showTimePicker, TimeOfDay |
| Selection / Menus | Text and icon DropdownMenu controls update selectedColor/selectedIcon; retained controllers | DropdownMenu<T>, DropdownMenuEntry<T> |
| Selection / IconButtonAnchorExample, ButtonAnchorExample | Toggle menu controller, submenu 3.1/3.2/3.3; menu item callbacks empty | MenuAnchor, MenuController, MenuItemButton, SubmenuButton, shortcuts |
| Selection / Sliders | Initial values 30 and 20; callbacks update independent continuous/discrete controls | Slider |
| Text inputs / TextFields, _ClearButton | Six fields: filled normal/error/disabled and outlined normal/error/disabled; two shared controllers per style; clear, prefix search icon, helper/hint/error; filled error maxLength=10 without enforcement | TextField, InputDecoration, OutlineInputBorder, TextEditingController, MaxLengthEnforcement.none |
| Shared / ComponentDecoration, ComponentGroupDecoration | Label/tooltip, tap-to-focus behavior, FocusNode disposal, spacing | Focus, Tooltip, GestureDetector, Card, Padding |

Reference chip deletion callbacks are intentionally empty. `work.md` P3's deletion
example must not silently turn this into an invented reference behavior. Any
additional deletion demo needs its own acceptance condition.

## Initial blocker ownership and independent fixtures (historical P0)

| ID | Reproduction / current result | Required shared work and next acceptance |
| --- | --- | --- |
| D01/D02 | `--material-sample`: record red pixel next to transparent pixel, Picture.toImage(2,1), rawRgba; capability exception | Ui Picture snapshot + image resource ownership; rendering/host rasterization/readback. Specify byte order, premultiplication, row stride, size validation, dispose/clone and Worker request/response lifetime. Then run fixture with actual Windows and Web capabilities. Current fixture is unhosted and cannot establish per-host support. |
| D03 | Internal quantizer invoked in isolation with 3 opaque colors and maxColors=1; returns 3. Gray-majority and empty-score fixtures also FAIL against the executed Dart oracle. | Material/color runtime Celebi and Score; compare pinned material_color_utilities 0.13.0. Reflection is only a temporary validation bridge while public image-provider extraction is blocked; do not expose internals solely for this test. |
| D04 | `--material-sample-search`: default enabled, omitted trailing, optional callbacks, explicit options | Shared search_anchor.cs factory and route forwarding corrected. Four construction/callback checks PASS; mounted open/close, search/history and focus restoration remain notVerified. |
| D05 | No URL launcher API found in product C#/TS; existing IPlatformServicesHostCapability provides clipboard/cursor only | Reuse view-scoped capability lookup pattern from Services/clipboard.cs; add a separate optional URL capability and typed outcome so existing host implementers remain compatible. Services owns validation/policy; Windows shell and Web main own launch. Main must handle activation synchronously or acknowledge popup blocking; a delayed Worker round trip cannot be assumed to preserve activation. OS targets report separately. |

Dependencies: P3 Search uses D04; P4 image theme uses D01 → D02 → D03 and
async latest-selection/error/dispose handling; P4 Color link uses D05. P1/P2
composition does not prove any blocker complete. No renderer default changes.

## C# integration and current acceptance

`SampleApp.cs` owns theme revisions, navigation and reversible responsive transitions;
`Components.cs`/`Selection.cs` contain all six groups; `Screens.cs` and `SchemePreview.cs`
contain the other destinations; `Components.cs` measures the finite gallery through separate box slivers;
`Decorations.cs` contains section focus/card wrappers and the drawer; `ImageDemo.cs`
uses the real image pipeline. Source and font provenance is in `src/MaterialSample/source-provenance.json`.

Search minimal-argument construction and the mounted Web route both pass, including
selection, history, reopen and Escape. All nine seed choices and six real image themes
pass; intercepted corrupt/late responses verify retry and last-success/latest-selection
behavior. URL service contracts and Web opening/blocked results pass. Public font, default
theme dispatch, typed routes, nullable restoration, calendar normalization, private
InheritedModel lookup, overlay layout, zero-duration animation completion and DPR semantics
have independent regressions. See work section 9 for exact artifact labels and newer tests.

The source comparison also corrected FAB order, selected destination icons, badge counts,
navigation example destinations, checkbox/radio list tiles, chip enabled/disabled states,
menu selection preview, tabs and app bars. Reference InputChip delete callbacks remain empty.
The final `material-sample-final-v27` integration run passes all 13 scenarios. Section
height estimates now survive selection rebuilds, and Web radio semantics keeps checked
state separate from tile highlighting. Light/dark body measurements are recorded in
`visual-comparison-v2.json`. These results do not establish full pixel parity, every
focus transition, carousel snapping accuracy or physical IME acceptance.

## Image repair and reference extension — 2026-09-06

The initial failures above remain historical evidence. D01/D02 now pass with an
attached native Skia capability and the live CanvasKit Worker; D03 passes exact
MCU 0.13.0 palette/seed/role differential on each host's own bytes. The public
MemoryImage/NetworkImageIo path is exercised in Web. D06 restores paintImage's
default center alignment; D07 connects codec completion and stream error callbacks.
See [image-pipeline contracts and limitations](../image-pipeline/README.md).

User-requested reference-only expansion: Components gains **Image demo**, after
Text inputs in the narrow single list or the wider second list. It adds asset/URL
selection, Contain/Cover, image retry and light/dark palette extraction. The asset
is `mae-mu-9002s2VnOAY-unsplash.webp`; the original six theme images are unchanged.
This is a seventh local section, not an upstream section present in the P0 snapshot.
The old hashes must not be silently replaced; future snapshots must include
`lib/src/image_demo.dart`, its test and `assets/images/`.
Flutter widget/browser acceptance does not mean the section has been ported to Doroti.
