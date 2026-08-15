import 'dart:convert';

import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter/semantics.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  testWidgets(
    'adaptive controls preserve the pinned platform selection trace',
    (WidgetTester tester) async {
      final trace = <String, Object?>{};
      for (final platform in <TargetPlatform>[
        TargetPlatform.windows,
        TargetPlatform.macOS,
      ]) {
        var checkboxValue = false;
        var switchValue = false;
        var sliderValue = 0.25;
        var callbackCount = 0;

        await tester.pumpWidget(
          MaterialApp(
            key: ValueKey<TargetPlatform>(platform),
            theme: ThemeData(platform: platform),
            home: StatefulBuilder(
              builder: (context, setState) {
                return Material(
                  child: Column(
                    children: <Widget>[
                      Semantics(
                        label: 'adaptive checkbox',
                        child: Checkbox.adaptive(
                          key: const ValueKey<String>('adaptive-checkbox'),
                          value: checkboxValue,
                          onChanged: (value) => setState(() {
                            checkboxValue = value ?? false;
                            callbackCount += 1;
                          }),
                        ),
                      ),
                      Semantics(
                        label: 'adaptive switch',
                        child: Switch.adaptive(
                          key: const ValueKey<String>('adaptive-switch'),
                          value: switchValue,
                          onChanged: (value) => setState(() {
                            switchValue = value;
                            callbackCount += 1;
                          }),
                        ),
                      ),
                      Semantics(
                        label: 'adaptive slider',
                        child: Slider.adaptive(
                          key: const ValueKey<String>('adaptive-slider'),
                          value: sliderValue,
                          onChanged: (value) => setState(() {
                            sliderValue = value;
                            callbackCount += 1;
                          }),
                        ),
                      ),
                      Semantics(
                        label: 'adaptive progress',
                        child: CircularProgressIndicator.adaptive(value: 0.5),
                      ),
                    ],
                  ),
                );
              },
            ),
          ),
        );
        await tester.pumpAndSettle();

        final isCupertino = platform == TargetPlatform.macOS;
        expect(
          find.byType(CupertinoCheckbox),
          isCupertino ? findsOneWidget : findsNothing,
        );
        // The pinned Switch.adaptive implementation keeps the Material render
        // object on every platform and applies Cupertino adaptive colors on
        // iOS/macOS; it does not instantiate CupertinoSwitch.
        expect(find.byType(CupertinoSwitch), findsNothing);
        expect(
          find.byType(CupertinoSlider),
          isCupertino ? findsOneWidget : findsNothing,
        );
        expect(
          find.byType(CupertinoActivityIndicator),
          isCupertino ? findsOneWidget : findsNothing,
        );

        await tester.tap(
          find.byKey(const ValueKey<String>('adaptive-checkbox')),
        );
        await tester.pump();
        await tester.tap(find.byKey(const ValueKey<String>('adaptive-switch')));
        await tester.pump();

        final semantics = tester
            .getSemantics(find.byKey(const ValueKey<String>('adaptive-switch')))
            .getSemanticsData();
        expect(semantics.hasAction(SemanticsAction.tap), isTrue);
        expect(checkboxValue, isTrue);
        expect(switchValue, isTrue);
        expect(callbackCount, 2);

        trace[platform.name] = <String, Object?>{
          'selection': <String, String>{
            'checkbox': isCupertino ? 'cupertino' : 'material',
            'switch': isCupertino ? 'material-cupertino-colors' : 'material',
            'slider': isCupertino ? 'cupertino' : 'material',
            'progress': isCupertino ? 'cupertino' : 'material',
          },
          'controls': <String>['checkbox', 'switch', 'slider', 'progress'],
          'callbackCount': callbackCount,
          'checkboxValue': checkboxValue,
          'switchValue': switchValue,
          'semanticsTap': semantics.hasAction(SemanticsAction.tap),
        };
      }

      // The G7 validator extracts this stable marker and compares it with the
      // promoted Doroti managed trace.
      // ignore: avoid_print
      print('G7_ADAPTIVE_TRACE=${jsonEncode(trace)}');
    },
  );
}
