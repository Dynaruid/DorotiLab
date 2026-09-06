// This is a basic Flutter widget test.
//
// To perform an interaction with a widget in your test, use the WidgetTester
// utility in the flutter_test package. For example, you can send tap and scroll
// gestures. You can also use WidgetTester to find child widgets in the widget
// tree, read text, and verify that the values of widget properties are correct.

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'package:flutter_sample_app/main.dart';

void main() {
  testWidgets('Material demo starts and switches theme', (
    WidgetTester tester,
  ) async {
    // Build our app and trigger a frame.
    await tester.pumpWidget(const App());

    // The demo starts with the system theme.
    expect(find.text('Material 3'), findsOneWidget);
    expect(
      tester.widget<MaterialApp>(find.byType(MaterialApp)).themeMode,
      ThemeMode.system,
    );

    // Switch to the dark theme using the app bar action.
    await tester.tap(
      find.descendant(
        of: find.byType(AppBar),
        matching: find.byIcon(Icons.dark_mode_outlined),
      ),
    );
    await tester.pump();
    await tester.pump(const Duration(seconds: 1));

    // The selected theme is applied to the app.
    expect(
      tester.widget<MaterialApp>(find.byType(MaterialApp)).themeMode,
      ThemeMode.dark,
    );
  });
}
