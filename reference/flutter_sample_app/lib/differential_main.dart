import 'dart:ui';
import 'dart:js_interop';
import 'dart:js_interop_unsafe';

import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter/semantics.dart';
import 'resize_fixture.dart';

void main() {
  final fixture = Uri.base.queryParameters['dorotiResizeFixture'];
  runApp(['F0', 'F1', 'F2'].contains(fixture) ? ResizeFixture(fixture!) : const DifferentialApp());
}

class DifferentialApp extends StatelessWidget {
  const DifferentialApp({super.key});

  @override
  Widget build(BuildContext context) => MaterialApp(
    debugShowCheckedModeBanner: false,
    title: 'Doroti Web differential fixture',
    theme: ThemeData(
      colorScheme: ColorScheme.fromSeed(seedColor: const Color(0xff6750a4)),
    ),
    home: const DifferentialPage(),
  );
}

class DifferentialPage extends StatefulWidget {
  const DifferentialPage({super.key});

  @override
  State<DifferentialPage> createState() => _DifferentialPageState();
}

class _DifferentialPageState extends State<DifferentialPage> {
  int counter = 0;
  double slider = .35;
  bool blur = true;
  int interactionSequence = 0;
  SemanticsHandle? semanticsHandle;
  int resizeFrameSequence = 0;

  @override
  void initState() {
    super.initState();
    semanticsHandle = SemanticsBinding.instance.ensureSemantics();
    _publishAfterFrame();
  }

  @override
  void dispose() {
    semanticsHandle?.dispose();
    super.dispose();
  }

  void _publishAfterFrame() {
    final sequence = ++interactionSequence;
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (!kIsWeb) return;
      globalContext.setProperty(
        '__flutterDifferentialFrame'.toJS,
        <String, Object>{
          'sequence': sequence,
          'counter': counter,
          'slider': slider,
          'blur': blur,
        }.jsify(),
      );
    });
  }

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.sizeOf(context);
    final dpr = MediaQuery.devicePixelRatioOf(context);
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (!kIsWeb) return;
      globalContext.setProperty('__flutterResizeFrame'.toJS, <String, Object>{
        'sequence': ++resizeFrameSequence,
        'width': size.width,
        'height': size.height,
        'dpr': dpr,
        'endpoint': 'framework-post-frame; not GPU submit or scan-out',
      }.jsify());
    });
    return _buildPage(context);
  }

  Widget _buildPage(BuildContext context) => Scaffold(
    appBar: AppBar(
      title: const Text('Doroti / Flutter frame pipeline fixture'),
    ),
    floatingActionButton: FloatingActionButton(
      key: const ValueKey('counter-fab'),
      onPressed: () {
        setState(() => counter++);
        _publishAfterFrame();
      },
      child: const Icon(Icons.add),
    ),
    body: Stack(
      children: [
        const Positioned.fill(
          child: CustomPaint(painter: FixtureGridPainter()),
        ),
        SingleChildScrollView(
          key: const ValueKey('fixture-scroll'),
          padding: const EdgeInsets.all(20),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Text(
                'Counter $counter',
                key: const ValueKey('counter-text'),
                style: Theme.of(context).textTheme.headlineMedium,
              ),
              const SizedBox(height: 12),
              Row(
                children: [
                  ElevatedButton(
                    key: const ValueKey('increment-button'),
                    onPressed: () {
                      setState(() => counter++);
                      _publishAfterFrame();
                    },
                    child: const Text('Increment'),
                  ),
                  const SizedBox(width: 12),
                  OutlinedButton(
                    key: const ValueKey('effect-button'),
                    onPressed: () {
                      setState(() => blur = !blur);
                      _publishAfterFrame();
                    },
                    child: Text(blur ? 'Disable effect' : 'Enable effect'),
                  ),
                ],
              ),
              Slider(
                key: const ValueKey('fixture-slider'),
                value: slider,
                onChanged: (value) {
                  setState(() => slider = value);
                  _publishAfterFrame();
                },
              ),
              const TextField(
                key: ValueKey('fixture-text-field'),
                decoration: InputDecoration(
                  labelText: 'Korean / IME text input',
                  border: OutlineInputBorder(),
                ),
              ),
              const SizedBox(height: 16),
              ShaderMask(
                shaderCallback: (bounds) => LinearGradient(
                  colors: const [
                    Color(0xff6750a4),
                    Color(0xff00a6a6),
                    Color(0xffffb300),
                  ],
                  stops: [0, slider, 1],
                ).createShader(bounds),
                child: const Text(
                  'Shader and clipping workload',
                  style: TextStyle(fontSize: 30, color: Colors.white),
                ),
              ),
              const SizedBox(height: 12),
              ClipRRect(
                borderRadius: BorderRadius.circular(24),
                child: BackdropFilter(
                  filter: ImageFilter.blur(
                    sigmaX: blur ? 5 : 0,
                    sigmaY: blur ? 5 : 0,
                  ),
                  child: Container(
                    height: 170,
                    color: Theme.of(
                      context,
                    ).colorScheme.surface.withValues(alpha: .82),
                    child: ListView.builder(
                      key: const ValueKey('fixture-list'),
                      itemCount: 40,
                      itemExtent: 44,
                      itemBuilder: (context, index) => ListTile(
                        leading: const Icon(Icons.grid_view),
                        title: Text('Grid row ${index + 1}'),
                        trailing: Text('${(index + 1) * 17}'),
                      ),
                    ),
                  ),
                ),
              ),
              const SizedBox(height: 640),
            ],
          ),
        ),
      ],
    ),
  );
}

class FixtureGridPainter extends CustomPainter {
  const FixtureGridPainter();

  @override
  void paint(Canvas canvas, Size size) {
    final minor = Paint()
      ..color = const Color(0x126750a4)
      ..strokeWidth = 1;
    final major = Paint()
      ..color = const Color(0x336750a4)
      ..strokeWidth = 1;
    for (double x = 0; x <= size.width; x += 16) {
      canvas.drawLine(
        Offset(x, 0),
        Offset(x, size.height),
        x % 64 == 0 ? major : minor,
      );
    }
    for (double y = 0; y <= size.height; y += 16) {
      canvas.drawLine(
        Offset(0, y),
        Offset(size.width, y),
        y % 64 == 0 ? major : minor,
      );
    }
    final edge = Paint()
      ..color = const Color(0xffff006e)
      ..strokeWidth = 4
      ..style = PaintingStyle.stroke;
    canvas.drawRect(Offset.zero & size, edge);
  }

  @override
  bool shouldRepaint(covariant FixtureGridPainter oldDelegate) => false;
}
