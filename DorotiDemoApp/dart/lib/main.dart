import 'package:flutter/material.dart';

class DorotiGeneratedDemoApp extends StatelessWidget {
  const DorotiGeneratedDemoApp({super.key});

  @override
  Widget build(BuildContext context) {
    return const MaterialApp(
      title: 'Doroti Generated Demo',
      locale: Locale('en', 'US'),
      debugShowCheckedModeBanner: false,
      home: DorotiGeneratedDemoHome(),
    );
  }
}

class DorotiGeneratedDemoHome extends StatefulWidget {
  const DorotiGeneratedDemoHome({super.key});

  @override
  State<DorotiGeneratedDemoHome> createState() =>
      _DorotiGeneratedDemoHomeState();
}

class _DorotiGeneratedDemoHomeState extends State<DorotiGeneratedDemoHome> {
  int _pressCount = 0;
  bool _checked = false;
  bool _switched = false;
  double _slider = 0.2;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Doroti Generated Demo')),
      body: Container(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          spacing: 10,
          children: [
            const Text('Dart source · package-only · strict GPU'),
            Text('Pressed $_pressCount'),
            const SizedBox(height: 48),
            ElevatedButton(
              onPressed: () => setState(() => _pressCount++),
              child: const Text('G6 generated button'),
            ),
            Row(
              spacing: 12,
              children: [
                Checkbox(
                  value: _checked,
                  onChanged: (value) =>
                      setState(() => _checked = value == true),
                ),
                const Text('Generated checkbox'),
                Switch(
                  value: _switched,
                  onChanged: (value) => setState(() => _switched = value),
                ),
              ],
            ),
            Slider(
              value: _slider,
              min: 0,
              max: 1,
              divisions: 10,
              onChanged: (value) => setState(() => _slider = value),
            ),
            const Card(
              child: ListTile(
                title: Text('Generated Material components'),
                subtitle: Text('asset · font · localization · plugin pipeline'),
              ),
            ),
          ],
        ),
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: () => setState(() => _pressCount++),
        child: const Text('+'),
      ),
    );
  }
}
