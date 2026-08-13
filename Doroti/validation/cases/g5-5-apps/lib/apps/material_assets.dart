import 'package:flutter/material.dart';
import 'package:g55_apps/shared/common.dart';

class G55MaterialAssetsApp extends StatelessWidget {
  const G55MaterialAssetsApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: g55Title('material-assets'),
      home: const Text('material asset app'),
    );
  }
}
