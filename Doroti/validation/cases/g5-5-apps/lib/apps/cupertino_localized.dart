import 'package:flutter/cupertino.dart';
import 'package:g55_apps/shared/common.dart';

class G55CupertinoLocalizedApp extends StatelessWidget {
  const G55CupertinoLocalizedApp({super.key});

  @override
  Widget build(BuildContext context) {
    return CupertinoApp(
      title: g55Title('cupertino-localized'),
      home: const Text('cupertino localized app'),
    );
  }
}
