import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:g55_apps/shared/common.dart';

class G55MaterialPluginApp extends StatelessWidget {
  const G55MaterialPluginApp({super.key});

  MethodChannel get echoChannel => const MethodChannel('g55/echo', StandardMethodCodec());

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: g55Title('material-plugin'),
      home: const Text('material plugin app'),
    );
  }
}
