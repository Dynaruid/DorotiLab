class Intent {}

abstract class Action<T extends Intent> {
  bool enabled = true;
  Action();
  Object? invoke(T intent);
}

class RenameIntent extends Intent {
  final int value;
  RenameIntent(this.value);
}

class RenameAction extends Action<RenameIntent> {
  @override
  Object? invoke(RenameIntent intent) => intent.value;
}

class ErasedRenameAction extends Action<Intent> {
  @override
  Object? invoke(Intent intent) => 42;
}

class IntentCallbacks {
  final void Function(RenameIntent) callback;
  IntentCallbacks(this.callback);
  void run(RenameIntent intent) => callback(intent);
}
