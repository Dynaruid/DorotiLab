import 'dart:io';

const dorotiLocalRootEnvironmentVariable = 'DOROTI_LOCAL_ROOT';

Directory dorotiLocalRoot() {
  final configured = Platform.environment[dorotiLocalRootEnvironmentVariable];
  if (configured != null && configured.trim().isNotEmpty) {
    final directory = Directory(configured.trim());
    final resolved = directory.isAbsolute
        ? directory.absolute
        : Directory(_join(Directory.current.absolute.path, directory.path));
    resolved.createSync(recursive: true);
    return resolved;
  }

  var current = Directory.current.absolute;
  while (true) {
    final nestedDoroti = Directory(_join(current.path, 'Doroti'));
    if (File(_join(nestedDoroti.path, 'Doroti.slnx')).existsSync()) {
      final root = Directory(_join(current.path, '.doroti'));
      root.createSync(recursive: true);
      return root;
    }
    if (File(_join(current.path, 'Doroti.slnx')).existsSync()) {
      final parent = current.parent;
      final workspaceOwnsCompiler = Directory(
        _join(parent.path, 'tools', 'Doroti.DartToCSharp'),
      ).existsSync();
      final root = Directory(
        _join(workspaceOwnsCompiler ? parent.path : current.path, '.doroti'),
      );
      root.createSync(recursive: true);
      return root;
    }
    if (current.parent.path == current.path) {
      throw StateError(
        'Could not find the Doroti workspace from ${Directory.current.path}. '
        'Set $dorotiLocalRootEnvironmentVariable explicitly.',
      );
    }
    current = current.parent;
  }
}

Directory dorotiCacheDirectory(String name) {
  _validateName(name);
  final directory = Directory(_join(dorotiLocalRoot().path, 'cache', name));
  directory.createSync(recursive: true);
  return directory;
}

Directory createDorotiTemporaryDirectory(String name) {
  _validateName(name);
  final parent = Directory(_join(dorotiLocalRoot().path, 'tmp'))
    ..createSync(recursive: true);
  return parent.createTempSync('$name-');
}

String dorotiStableKey(String value) {
  var hash = 0xcbf29ce484222325;
  for (final byte in value.codeUnits) {
    hash ^= byte;
    hash = (hash * 0x100000001b3) & 0xffffffffffffffff;
  }
  return hash.toRadixString(16).padLeft(16, '0');
}

String _join(String first, String second, [String? third]) {
  final two = '$first${Platform.pathSeparator}$second';
  return third == null ? two : '$two${Platform.pathSeparator}$third';
}

void _validateName(String name) {
  if (!RegExp(r'^[A-Za-z0-9][A-Za-z0-9._-]*$').hasMatch(name)) {
    throw ArgumentError.value(name, 'name', 'Invalid Doroti local-state name');
  }
}
