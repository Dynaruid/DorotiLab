import 'package:flutter/widgets.dart';

class G53ExternalApp extends StatelessWidget {
  const G53ExternalApp({super.key});

  @override
  Widget build(BuildContext context) {
    return Directionality(
      textDirection: TextDirection.ltr,
      child: Navigator(
        onGenerateRoute: (_) => PageRouteBuilder<void>(
          pageBuilder: (_, __, ___) => EditableText(
            controller: TextEditingController(text: 'doroti'),
            focusNode: FocusNode(),
            style: const TextStyle(),
            cursorColor: const Color(0xFFFFFFFF),
            backgroundCursorColor: const Color(0xFF000000),
          ),
        ),
      ),
    );
  }
}

void main() => runApp(const G53ExternalApp());
