import 'dart:convert';
import 'dart:io';
import 'package:flutter/rendering.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_test/flutter_test.dart';

final calls = <String>[];
var tick = 0;

class ScopeWidget extends InheritedWidget {
  const ScopeWidget(this.value, {required super.child});
  final int value;
  @override
  bool updateShouldNotify(ScopeWidget oldWidget) => oldWidget.value != value;
}
class Probe extends StatefulWidget {
  const Probe({super.key});
  @override
  ProbeState createState() => ProbeState();
}
class ProbeState extends State<Probe> {
  void log(String name) => calls.add('$tick:probe:$name');
  @override
  void initState() { super.initState(); log('init'); }
  @override
  void didChangeDependencies() { super.didChangeDependencies(); log('dependencies'); }
  @override
  void didUpdateWidget(Probe oldWidget) { super.didUpdateWidget(oldWidget); log('update'); }
  @override
  void activate() { super.activate(); log('activate'); }
  @override
  void deactivate() { log('deactivate'); super.deactivate(); }
  @override
  void dispose() { log('dispose'); super.dispose(); }
  @override
  Widget build(BuildContext context) {
    final value = context.dependOnInheritedWidgetOfExactType<ScopeWidget>()!.value;
    log('build:$value');
    return SizedBox(width: value.toDouble(), height: 10);
  }
}
void main() {
  TestWidgetsFlutterBinding.ensureInitialized();
  test('same inherited target and GlobalKey lifetime in five logical ticks', () {
    final owner = WidgetsBinding.instance.buildOwner!;
    final container = RenderPositionedBox(alignment: Alignment.topLeft, textDirection: TextDirection.ltr);
    final key = GlobalKey<ProbeState>();
    RenderObjectToWidgetElement<RenderBox>? root;
    final states = <ProbeState?>[];
    for (tick = 0; tick < 5; tick++) {
      final right = tick == 2;
      Widget child(bool side) => tick < 4 && side == right ? Probe(key: key) : const SizedBox(width: 10, height: 10);
      final content = ScopeWidget(1, child: Row(textDirection: TextDirection.ltr, children: [
        ScopeWidget(10, child: child(false)), ScopeWidget(20, child: child(true)),
      ]));
      root = RenderObjectToWidgetAdapter<RenderBox>(container: container, child: content).attachToRenderTree(owner, root);
      owner.buildScope(root);
      container.layout(BoxConstraints.tight(const Size(200, 100)));
      owner.finalizeTree();
      states.add(key.currentState);
    }
    expect(states.take(4).every((state) => state != null && identical(state, states.first)), isTrue);
    expect(states.last, isNull);
    final output = Platform.environment['DOROTI_SAME_WORK_OUTPUT']!;
    File(output).writeAsStringSync(jsonEncode({'calls': calls, 'runtime': 'Flutter debug test; internal wrapper trace not captured'}));
  });
}
