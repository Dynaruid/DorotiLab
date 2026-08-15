import 'dart:convert';

import 'package:doroti_demo_app/main.dart';
import 'package:flutter/semantics.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  testWidgets('generated product preserves navigation state and semantics', (
    WidgetTester tester,
  ) async {
    await tester.pumpWidget(const DorotiGeneratedDemoApp());
    await tester.pumpAndSettle();

    expect(find.text('Pressed 0'), findsOneWidget);
    await tester.tap(find.text('G6 generated button'));
    await tester.pump();
    expect(find.text('Pressed 1'), findsOneWidget);

    final openDetails = find.text('Open generated details');
    final openDetailsSemantics = tester
        .getSemantics(openDetails)
        .getSemanticsData();
    expect(openDetailsSemantics.hasAction(SemanticsAction.tap), isTrue);

    await tester.ensureVisible(openDetails);
    await tester.tap(openDetails);
    await tester.pumpAndSettle();
    expect(find.text('Generated details'), findsOneWidget);
    expect(find.text('Back to generated home'), findsOneWidget);

    final backSemantics = tester
        .getSemantics(find.text('Back to generated home'))
        .getSemanticsData();
    expect(backSemantics.hasAction(SemanticsAction.tap), isTrue);
    await tester.tap(find.text('Back to generated home'));
    await tester.pumpAndSettle();

    expect(find.text('Pressed 1'), findsOneWidget);
    expect(find.text('Doroti Generated Demo'), findsWidgets);
    // ignore: avoid_print
    print(
      'G7_GENERATED_TRACE=${jsonEncode(<String, Object?>{
        'navigation': <String>['home', 'details', 'home'],
        'state': <String>['pressed=0', 'pressed=1', 'pressed=1'],
        'semantics': <String>['open-details:tap', 'back-home:tap'],
      })}',
    );
  });
}
