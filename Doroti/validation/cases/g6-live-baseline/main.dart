import 'package:flutter/material.dart';

void main() => runApp(const G6LiveBaseline());

class G6LiveBaseline extends StatelessWidget {
  const G6LiveBaseline({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Doroti Material Demo',
      color: const Color(0xff6750a4),
      locale: const Locale('en', 'US'),
      debugShowCheckedModeBanner: false,
      builder: (context, child) => Scaffold(
        appBar: AppBar(title: const Text('Doroti Material Demo')),
        body: const Card(
          child: Text(
            'Reviewed Flutter Material · Windows RID · strict Skia GPU',
          ),
        ),
        floatingActionButton: FloatingActionButton(
          tooltip: 'Material action',
          onPressed: _noop,
          child: const Text('+'),
        ),
      ),
    );
  }
}

void _noop() {}
