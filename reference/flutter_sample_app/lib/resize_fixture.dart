import 'package:flutter/widgets.dart';
import 'dart:js_interop';
import 'dart:js_interop_unsafe';

class ResizeFixture extends StatelessWidget {
  static int sequence = 0;
  final String kind;
  const ResizeFixture(this.kind, {super.key});
  static const fixedPicture = RepaintBoundary(child: Stack(children: [
    Positioned(left: 0, top: 0, width: 120, height: 80, child: ColoredBox(color: Color(0xff00a878))),
    Positioned(left: 12, top: 12, width: 96, height: 8, child: ColoredBox(color: Color(0xff173f5f))),
    Positioned(left: 12, top: 36, width: 72, height: 8, child: ColoredBox(color: Color(0xff173f5f))),
  ]));
  @override
  Widget build(BuildContext context) {
    final view = View.of(context);
    final size = MediaQuery.sizeOf(context);
    final dpr = MediaQuery.devicePixelRatioOf(context);
    final id = ++sequence;
    WidgetsBinding.instance.addPostFrameCallback((_) {
      globalContext.setProperty('__flutterResizeFrame'.toJS, <String, Object>{
        'sequence': id, 'width': size.width,
        'height': size.height, 'dpr': dpr,
        'endpoint': 'framework-post-frame; not captured presentation',
      }.jsify());
    });
    return Directionality(textDirection: TextDirection.ltr, child: Stack(children: [
    const Positioned.fill(child: ColoredBox(color: Color(0xfff8f8f8))),
    const Positioned(left: 2, top: 3, width: 22, height: 3, child: ColoredBox(color: Color(0xffff1744))),
    const Positioned(right: 1, top: 7, width: 3, height: 19, child: ColoredBox(color: Color(0xffff1744))),
    const Positioned(right: 1, bottom: 1, width: 27, height: 3, child: ColoredBox(color: Color(0xffff1744))),
    const Center(child: SizedBox(width: 12, height: 12, child: ColoredBox(color: Color(0xff2962ff)))),
    if (kind == 'F1') const Align(alignment: Alignment.bottomRight, child: SizedBox(width: 120, height: 80, child: fixedPicture)),
    if (kind == 'F2') const Positioned(left: 32, right: 32, top: 48, child: ClipRect(child: Text(
      'Resize wrapping fixture: alpha beta gamma delta epsilon zeta eta theta. '
      'Resize wrapping fixture: alpha beta gamma delta epsilon zeta eta theta.',
      style: TextStyle(inherit: false, fontFamily: 'NanumGothic', fontSize: 24, height: 1.2, color: Color(0xff173f5f)),
    ))),
    if (Uri.base.queryParameters['dorotiFrameMarker'] == '1') Positioned.fill(
      child: IgnorePointer(child: CustomPaint(painter: ResizeMarkerPainter(id, view.devicePixelRatio)))),
  ]));
  }
}

class ResizeMarkerPainter extends CustomPainter {
  final int sequence;
  final double dpr;
  ResizeMarkerPainter(this.sequence, this.dpr);
  @override
  void paint(Canvas canvas, Size size) {
    final values = [sequence, (size.width * dpr).round(), (size.height * dpr).round()];
    final colors = [const Color(0xff00ffff), const Color(0xffff00ff), const Color(0xff00ffff), const Color(0xffff00ff)];
    for (var field = 0; field < 3; field++) {
      for (var bit = 0; bit < (field == 0 ? 32 : 16); bit++) {
        colors.add((values[field] >> bit) & 1 == 1 ? const Color(0xff00ff00) : const Color(0xff000000));
      }
    }
    final paint = Paint()..isAntiAlias = false;
    for (var cell = 0; cell < colors.length; cell++) {
      paint.color = colors[cell];
      canvas.drawRect(Rect.fromLTWH((32 + cell * 4) / dpr, 32 / dpr, 4 / dpr, 4 / dpr), paint);
    }
  }
  @override
  bool shouldRepaint(ResizeMarkerPainter oldDelegate) => sequence != oldDelegate.sequence || dpr != oldDelegate.dpr;
}
