import 'package:flutter/widgets.dart';
import 'package:g55_apps/shared/common.dart';

class G55WidgetsBaseApp extends StatelessWidget {
  const G55WidgetsBaseApp({super.key});

  @override
  Widget build(BuildContext context) {
    return Directionality(
      textDirection: TextDirection.ltr,
      child: Text(g55Title('widgets-base')),
    );
  }
}
