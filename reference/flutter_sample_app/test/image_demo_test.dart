import 'dart:ui' as ui;

import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:flutter_sample_app/src/image_demo.dart';

class _FixtureImage extends ImageProvider<_FixtureImage> {
  const _FixtureImage(this.image);
  final ui.Image image;

  @override
  Future<_FixtureImage> obtainKey(ImageConfiguration configuration) =>
      SynchronousFuture(this);

  @override
  // ignore: deprecated_member_use
  ImageStreamCompleter loadImage(
    _FixtureImage key,
    ImageDecoderCallback decode,
  ) => OneFrameImageStreamCompleter(
    SynchronousFuture(ImageInfo(image: image.clone())),
  );
}

void main() {
  testWidgets('image source, fit and derived palettes work at narrow width', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(390, 1100);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.resetPhysicalSize);
    addTearDown(tester.view.resetDevicePixelRatio);
    final image = await tester.runAsync(() async {
      final recorder = ui.PictureRecorder();
      ui.Canvas(recorder).drawPaint(ui.Paint()..color = Colors.red);
      final picture = recorder.endRecording();
      final image = await picture.toImage(2, 2);
      picture.dispose();
      return image;
    });
    addTearDown(image!.dispose);
    final provider = _FixtureImage(image);
    await tester.pumpWidget(
      MaterialApp(
        home: Scaffold(
          body: SingleChildScrollView(
            child: ImageDemo(assetImage: provider, networkImage: provider),
          ),
        ),
      ),
    );
    await tester.pump();
    expect(tester.widget<Image>(find.byType(Image)).fit, BoxFit.contain);
    await tester.tap(find.text('Cover'));
    await tester.pumpAndSettle();
    expect(tester.widget<Image>(find.byType(Image)).fit, BoxFit.cover);
    await tester.runAsync(() async {
      final extract = tester
          .widget<FilledButton>(find.byType(FilledButton))
          .onPressed!;
      await (extract as Future<void> Function())();
    });
    await tester.pumpAndSettle();
    expect(find.text('Light palette'), findsOneWidget);
    expect(find.text('Dark palette'), findsOneWidget);
    expect(tester.takeException(), isNull);
    await tester.tap(find.text('Image URL'));
    await tester.pumpAndSettle();
    expect(find.text('Unsplash · remote image'), findsOneWidget);
    expect(find.text('Light palette'), findsNothing);
    expect(find.text('Extract colors'), findsOneWidget);
    expect(tester.takeException(), isNull);
  });
}
