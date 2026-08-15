import 'dart:ui' as ui;

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_test/flutter_test.dart';

class G7ReferenceBinding extends AutomatedTestWidgetsFlutterBinding {
  @override
  bool get disableShadows => false;
}

void main() {
  G7ReferenceBinding();

  testWidgets('captures the pinned G7 CalendarDatePicker fixture', (tester) async {
    tester.view.physicalSize = const Size(1800, 1440);
    tester.view.devicePixelRatio = 2;
    addTearDown(tester.view.resetPhysicalSize);
    addTearDown(tester.view.resetDevicePixelRatio);

    final roboto = FontLoader('RobotoG7')..addFont(rootBundle.load('assets/Roboto-Regular.ttf'));
    final materialIcons = FontLoader('MaterialIcons')
      ..addFont(rootBundle.load('assets/MaterialIcons-Regular.otf'));
    await Future.wait([roboto.load(), materialIcons.load()]);

    final boundaryKey = GlobalKey();
    final theme = ThemeData(
      useMaterial3: true,
      colorSchemeSeed: const Color(0xff6750a4),
      scaffoldBackgroundColor: const Color(0xfffffbfe),
      fontFamily: 'RobotoG7',
    );
    await tester.pumpWidget(RepaintBoundary(
      key: boundaryKey,
      child: MaterialApp(
        title: 'Flutter G7-1V M6',
        locale: const Locale('en', 'US'),
        debugShowCheckedModeBanner: false,
        theme: theme,
        home: Theme(
          data: theme,
          child: Scaffold(
            backgroundColor: const Color(0xfffffbfe),
            appBar: AppBar(title: const Text('G6-5 M6 · exercise 0 · revision 0')),
            body: SingleChildScrollView(
              primary: false,
              child: Padding(
                padding: const EdgeInsets.all(16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    DataTable(
                      columns: const [
                        DataColumn(label: Text('Name')),
                        DataColumn(label: Text('Value')),
                      ],
                      rows: const [
                        DataRow(cells: [DataCell(Text('Doroti')), DataCell(Text('0'))]),
                      ],
                    ),
                    const SizedBox(height: 12),
                    Center(
                      child: SizedBox(
                        width: 384,
                        height: 420,
                        child: Card(
                          color: const Color(0xfffffbfe),
                          shadowColor: const Color(0xff000000),
                          surfaceTintColor: const Color(0x00000000),
                          elevation: 6,
                          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(28)),
                          clipBehavior: Clip.antiAlias,
                          margin: const EdgeInsets.all(12),
                          child: CalendarDatePicker(
                            initialDate: DateTime(2026, 8, 13),
                            firstDate: DateTime(2026, 1, 1),
                            lastDate: DateTime(2026, 12, 31),
                            currentDate: DateTime(2026, 8, 13),
                            onDateChanged: (_) {},
                            selectableDayPredicate: (date) => date.day != 15,
                          ),
                        ),
                      ),
                    ),
                    const SizedBox(height: 12),
                    const Text('Material 3 calendar surface · 28 dp rounded shape · 6 dp elevation'),
                  ],
                ),
              ),
            ),
          ),
        ),
      ),
    ));
    await tester.pumpAndSettle();

    await expectLater(find.byKey(boundaryKey), matchesGoldenFile('goldens/flutter-calendar.png'));
  });

  testWidgets('captures the pinned G7 compositing fixture', (tester) async {
    tester.view.physicalSize = const Size(512, 320);
    tester.view.devicePixelRatio = 2;
    addTearDown(tester.view.resetPhysicalSize);
    addTearDown(tester.view.resetDevicePixelRatio);
    final key = GlobalKey();
    await tester.pumpWidget(Directionality(
      textDirection: TextDirection.ltr,
      child: RepaintBoundary(
        key: key,
        child: SizedBox(
          width: 256,
          height: 160,
          child: Stack(children: [
            const Positioned.fill(child: CustomPaint(painter: _CheckerPainter())),
            Positioned(
              left: 32,
              top: 32,
              width: 192,
              height: 96,
              child: ClipRect(
                child: BackdropFilter(
                  filter: ui.ImageFilter.blur(sigmaX: 6, sigmaY: 2),
                  child: Container(color: const Color(0x6bffffff)),
                ),
              ),
            ),
            Positioned(
              left: 52,
              top: 52,
              child: ImageFiltered(
                imageFilter: ui.ImageFilter.blur(sigmaX: 2, sigmaY: 2),
                child: Container(width: 44, height: 32, color: const Color(0xffb3261e)),
              ),
            ),
            Positioned(
              right: 48,
              bottom: 44,
              child: Opacity(
                opacity: 0.65,
                child: Container(width: 40, height: 36, color: const Color(0xff6750a4)),
              ),
            ),
          ]),
        ),
      ),
    ));
    await tester.pump();
    await expectLater(find.byKey(key), matchesGoldenFile('goldens/flutter-compositing.png'));
  });
}

class _CheckerPainter extends CustomPainter {
  const _CheckerPainter();

  @override
  void paint(Canvas canvas, Size size) {
    const cell = 16.0;
    final light = Paint()..color = const Color(0xfff5f1f7);
    final dark = Paint()..color = const Color(0xff49454f);
    for (var y = 0.0; y < size.height; y += cell) {
      for (var x = 0.0; x < size.width; x += cell) {
        canvas.drawRect(Rect.fromLTWH(x, y, cell, cell), ((x / cell + y / cell).floor()).isEven ? light : dark);
      }
    }
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}
