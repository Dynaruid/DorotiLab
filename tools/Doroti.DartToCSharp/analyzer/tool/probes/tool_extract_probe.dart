import 'dart:convert';
import 'dart:io';

import 'package:analyzer/dart/analysis/results.dart';
import 'package:analyzer/dart/analysis/utilities.dart';
import 'package:analyzer/dart/ast/ast.dart';
import 'package:analyzer/dart/element/element.dart';
import 'package:analyzer/dart/element/type.dart';
import 'package:analyzer/diagnostic/diagnostic.dart';
import 'package:analyzer/src/dart/analysis/analysis_context_collection.dart';
import 'package:doroti_dart_analyzer/src/local_storage.dart';

Future<void> main(List<String> args) async {
  if (args.isEmpty || args.length > 3) {
    stderr.writeln(
      'Usage: dart run entrypoints/extract.dart <input.dart> [--syntax-only | --packages <package_config.json>]',
    );
    exitCode = 64;
    return;
  }

  final path = File(args.first).absolute.resolveSymbolicLinksSync();
  final syntaxOnly = args.length == 2 && args[1] == '--syntax-only';
  final packagesPath = args.length == 3 && args[1] == '--packages'
      ? File(args[2]).absolute.resolveSymbolicLinksSync()
      : null;
  if (args.length > 1 && !syntaxOnly && packagesPath == null) {
    stderr.writeln('Invalid analyzer arguments.');
    exitCode = 64;
    return;
  }
  late final String content;
  late final CompilationUnit unit;
  late final List<Diagnostic> analysisDiagnostics;
  late final String libraryUri;
  late final List<String> imports;
  late final List<Map<String, Object?>> importDetails;
  late final List<Map<String, Object?>> libraryFragments;
  late final List<String> accessibleExtensions;
  if (syntaxOnly) {
    content = File(path).readAsStringSync();
    final parsed = parseString(
      content: content,
      path: path,
      throwIfDiagnostics: false,
    );
    unit = parsed.unit;
    analysisDiagnostics = parsed.errors;
    libraryUri = Uri.file(path).toString();
    imports =
        unit.directives
            .whereType<ImportDirective>()
            .map((item) => item.uri.stringValue)
            .whereType<String>()
            .toSet()
            .toList()
          ..sort();
    importDetails = imports
        .map((uri) => <String, Object?>{'uri': uri, 'prefix': null})
        .toList();
    libraryFragments = [
      {
        'uri': libraryUri,
        'isDefining': true,
        'ownerLibrary': libraryUri,
        'declarations': <String>[],
      },
    ];
    accessibleExtensions = <String>[];
  } else {
    final analysis = packagesPath == null
        ? await resolveFile(path: path)
        : await _resolveFileWithPackages(path, packagesPath);
    if (analysis is! ResolvedUnitResult) {
      stderr.writeln(
        'Dart analyzer could not resolve $path: ${analysis.runtimeType}',
      );
      exitCode = 65;
      return;
    }
    for (final imp in analysis.libraryFragment.libraryImports) {
      stderr.writeln(
        'IMPORT ' +
            imp.uri.toString() +
            ' -> ' +
            (imp.importedLibrary?.firstFragment.source.fullName ?? 'NULL'),
      );
    }
    content = analysis.content;
    unit = analysis.unit;
    analysisDiagnostics = analysis.diagnostics;
    libraryUri = analysis.libraryElement.uri.toString();
    imports =
        analysis.libraryFragment.libraryImports
            .map((item) => item.importedLibrary?.uri.toString())
            .whereType<String>()
            .toSet()
            .toList()
          ..sort();
    importDetails =
        analysis.libraryFragment.libraryImports
            .map(
              (item) => <String, Object?>{
                'uri': item.importedLibrary?.uri.toString(),
                'prefix': item.prefix?.element.displayName,
                'isSynthetic': item.isSynthetic,
              },
            )
            .where((item) => item['uri'] != null)
            .toList()
          ..sort((a, b) => (a['uri'] as String).compareTo(b['uri'] as String));
    final owner = analysis.libraryElement.uri.toString();
    libraryFragments =
        analysis.libraryElement.fragments
            .map(
              (fragment) => <String, Object?>{
                'uri': fragment.source.uri.toString(),
                'isDefining': identical(
                  fragment,
                  analysis.libraryElement.firstFragment,
                ),
                'ownerLibrary': owner,
                'declarations': _fragmentDeclarationIds(fragment, owner),
              },
            )
            .toList()
          ..sort((a, b) => (a['uri'] as String).compareTo(b['uri'] as String));
    accessibleExtensions =
        analysis.libraryFragment.accessibleExtensions
            .map((item) => _canonicalElementId(item))
            .toSet()
            .toList()
          ..sort();
  }
  final declarations = <Map<String, Object?>>[];
  for (final declaration in unit.declarations) {
    if (declaration is! ClassDeclaration &&
        declaration is! EnumDeclaration &&
        declaration is! MixinDeclaration &&
        declaration is! ExtensionDeclaration &&
        declaration is! GenericTypeAlias &&
        declaration is! FunctionDeclaration &&
        declaration is! TopLevelVariableDeclaration) {
      continue;
    }

    if (declaration is TopLevelVariableDeclaration) {
      for (final variable in declaration.variables.variables) {
        final element = variable.declaredFragment?.element;
        declarations.add({
          'kind': declaration.runtimeType.toString(),
          'name': variable.name.lexeme,
          'offset': declaration.offset,
          'length': declaration.length,
          'source': content.substring(declaration.offset, declaration.end),
          'element': _element(
            element,
            element?.library?.uri.toString() ?? libraryUri,
          ),
          'members': <Object>[],
          'ast': _ast(declaration),
        });
      }
      continue;
    }

    final name = switch (declaration) {
      ClassDeclaration(:final name) ||
      EnumDeclaration(:final name) ||
      MixinDeclaration(:final name) ||
      GenericTypeAlias(:final name) ||
      FunctionDeclaration(:final name) => name.lexeme,
      ExtensionDeclaration(:final name) =>
        name?.lexeme ?? '<unnamed-extension>',
      _ => throw StateError(
        'Unsupported declaration ${declaration.runtimeType}',
      ),
    };
    final element = switch (declaration) {
      ClassDeclaration(:final declaredFragment) => declaredFragment?.element,
      EnumDeclaration(:final declaredFragment) => declaredFragment?.element,
      MixinDeclaration(:final declaredFragment) => declaredFragment?.element,
      ExtensionDeclaration(:final declaredFragment) =>
        declaredFragment?.element,
      GenericTypeAlias(:final declaredFragment) => declaredFragment?.element,
      FunctionDeclaration(:final declaredFragment) => declaredFragment?.element,
      _ => null,
    };
    final members = switch (declaration) {
      ClassDeclaration(:final members) ||
      EnumDeclaration(:final members) ||
      MixinDeclaration(:final members) ||
      ExtensionDeclaration(
        :final members,
      ) => members.expand((member) => _members(member, content)).toList(),
      _ => <Map<String, Object?>>[],
    };
    declarations.add({
      'kind': declaration.runtimeType.toString(),
      'name': name,
      'offset': declaration.offset,
      'length': declaration.length,
      'source': content.substring(declaration.offset, declaration.end),
      'element': _element(element, libraryUri),
      'members': members,
      'ast': _ast(declaration),
    });
  }

  declarations.sort((a, b) {
    final byOffset = (a['offset'] as int).compareTo(b['offset'] as int);
    if (byOffset != 0) {
      return byOffset;
    }
    return (a['name'] as String).compareTo(b['name'] as String);
  });

  final diagnostics =
      analysisDiagnostics.map((Diagnostic error) {
        return <String, Object>{
          'code': error.diagnosticCode.name,
          'severity': error.severity.name,
          'offset': error.offset,
          'length': error.length,
          'message': error.message,
        };
      }).toList()..sort((a, b) {
        final byOffset = (a['offset'] as int).compareTo(b['offset'] as int);
        if (byOffset != 0) {
          return byOffset;
        }
        return (a['code'] as String).compareTo(b['code'] as String);
      });

  final output = <String, Object?>{
    'schemaVersion': 'doroti.dart-analyzer-output/v3',
    'libraryUri': libraryUri,
    'analysisMode': syntaxOnly ? 'syntax-only' : 'resolved',
    'libraryGraph': {
      'library': libraryUri,
      'fragments': libraryFragments.length == 1
          ? [
              {
                ...libraryFragments.single,
                'declarations': declarations
                    .map((item) => item['element'])
                    .whereType<Map<String, Object?>>()
                    .map((item) => item['canonicalId'])
                    .whereType<String>()
                    .toList(),
              },
            ]
          : libraryFragments,
      'imports': imports,
      'importDetails': importDetails,
      'accessibleExtensions': accessibleExtensions,
    },
    'imports': imports,
    'directives': unit.directives.map((item) => item.toSource()).toList(),
    'declarations': declarations,
    'diagnostics': diagnostics,
  };

  stdout.writeln(const JsonEncoder.withIndent('  ').convert(output));
}

Iterable<Map<String, Object?>> _members(
  ClassMember member,
  String content,
) sync* {
  if (member is FieldDeclaration) {
    for (final variable in member.fields.variables) {
      final element = variable.declaredFragment?.element;
      yield {
        'kind': 'field',
        'name': variable.name.lexeme,
        'offset': variable.offset,
        'length': variable.length,
        'source': content.substring(variable.offset, variable.end),
        'element': _element(element, element?.library?.uri.toString() ?? ''),
        'isStatic': member.isStatic,
        'isFinal': member.fields.isFinal,
        'isConst': member.fields.isConst,
        'isLate': member.fields.isLate,
        'statements': <Object>[],
        'ast': _ast(variable),
      };
    }
    return;
  }

  final element = switch (member) {
    ConstructorDeclaration(:final declaredFragment) =>
      declaredFragment?.element,
    MethodDeclaration(:final declaredFragment) => declaredFragment?.element,
    _ => null,
  };
  final name = switch (member) {
    ConstructorDeclaration(:final name) => name?.lexeme ?? 'new',
    MethodDeclaration(:final name) => name.lexeme,
    _ => '<unsupported>',
  };
  yield {
    'kind': member is ConstructorDeclaration ? 'constructor' : 'method',
    'name': name,
    'offset': member.offset,
    'length': member.length,
    'source': content.substring(member.offset, member.end),
    'element': _element(element, element?.library.uri.toString() ?? ''),
    'isStatic': member is MethodDeclaration && member.isStatic,
    'isAbstract':
        member is MethodDeclaration && member.body is EmptyFunctionBody,
    'isGetter': member is MethodDeclaration && member.isGetter,
    'isSetter': member is MethodDeclaration && member.isSetter,
    'isOperator': member is MethodDeclaration && member.isOperator,
    'isFactory':
        member is ConstructorDeclaration && member.factoryKeyword != null,
    'isConst': member is ConstructorDeclaration && member.constKeyword != null,
    'statements': _statements(member, content),
    'ast': _ast(member),
  };
}

Map<String, Object?> _ast(AstNode node) {
  final children = <Map<String, Object?>>[];
  for (final child in node.childEntities) {
    if (child is AstNode) {
      children.add(_ast(child));
    }
  }

  return {
    'kind': _nodeKind(node),
    'analyzerKind': node.runtimeType.toString(),
    'category': _nodeCategory(node),
    'offset': node.offset,
    'length': node.length,
    'staticType': node is Expression
        ? node.staticType?.getDisplayString()
        : null,
    'elementId': _nodeElementId(node),
    'properties': _nodeProperties(node),
    'children': children,
  };
}

String _nodeKind(AstNode node) {
  final value = node.runtimeType.toString();
  return value.endsWith('Impl') ? value.substring(0, value.length - 4) : value;
}

String _nodeCategory(AstNode node) => switch (node) {
  Directive() => 'directive',
  Declaration() || ClassMember() => 'declaration',
  Statement() => 'statement',
  Expression() => 'expression',
  DartPattern() => 'pattern',
  TypeAnnotation() => 'type',
  FormalParameter() => 'parameter',
  Annotation() => 'metadata',
  ConstructorInitializer() => 'constructor-initializer',
  CollectionElement() => 'collection-element',
  _ => 'structural',
};

String? _nodeElementId(AstNode node) {
  final element = switch (node) {
    SimpleIdentifier(:final element) => element,
    PrefixedIdentifier(:final identifier) => identifier.element,
    NamedType(:final element) => element,
    ConstructorName(:final element) => element,
    Annotation(:final element) => element,
    MethodInvocation(:final methodName) => methodName.element,
    VariableDeclaration(:final declaredFragment) => declaredFragment?.element,
    InstanceCreationExpression(:final constructorName) =>
      constructorName.element,
    RedirectingConstructorInvocation(:final element) => element,
    SuperConstructorInvocation(:final element) => element,
    EnumConstantDeclaration(:final declaredFragment) =>
      declaredFragment?.element,
    _ => null,
  };
  if (element == null) {
    return null;
  }
  return _canonicalElementId(element);
}

Map<String, String?> _nodeProperties(AstNode node) {
  final properties = <String, String?>{};
  switch (node) {
    case SimpleIdentifier(:final name):
      properties['name'] = name;
    case BooleanLiteral(:final value):
      properties['value'] = value.toString();
    case IntegerLiteral(:final value):
      properties['value'] = value?.toString();
    case DoubleLiteral(:final value):
      properties['value'] = value.toString();
    case SimpleStringLiteral(:final value):
      properties['value'] = value;
    case InterpolationString(:final value):
      properties['value'] = value;
    case BinaryExpression(
      :final leftOperand,
      :final operator,
      :final rightOperand,
    ):
      properties['operator'] = operator.lexeme;
      properties['leftOffset'] = leftOperand.offset.toString();
      properties['rightOffset'] = rightOperand.offset.toString();
    case AssignmentExpression(
      :final leftHandSide,
      :final operator,
      :final rightHandSide,
    ):
      properties['operator'] = operator.lexeme;
      properties['leftOffset'] = leftHandSide.offset.toString();
      properties['rightOffset'] = rightHandSide.offset.toString();
    case PrefixExpression(:final operator, :final operand):
      properties['operator'] = operator.lexeme;
      properties['operandOffset'] = operand.offset.toString();
    case PostfixExpression(:final operator, :final operand):
      properties['operator'] = operator.lexeme;
      properties['operandOffset'] = operand.offset.toString();
    case PropertyAccess(:final target, :final operator, :final propertyName):
      properties['name'] = propertyName.name;
      properties['targetOffset'] = target?.offset.toString();
      properties['operator'] = operator.lexeme;
    case PrefixedIdentifier(:final prefix, :final identifier):
      properties['prefix'] = prefix.name;
      properties['name'] = identifier.name;
    case EnumConstantDeclaration(:final name):
      properties['name'] = name.lexeme;
    case MethodInvocation(
      :final target,
      :final operator,
      :final methodName,
      :final argumentList,
    ):
      properties['name'] = methodName.name;
      properties['targetOffset'] = target?.offset.toString();
      properties['argumentsOffset'] = argumentList.offset.toString();
      properties['operator'] = operator?.lexeme;
    case FunctionExpressionInvocation(:final function, :final argumentList):
      properties['functionOffset'] = function.offset.toString();
      properties['argumentsOffset'] = argumentList.offset.toString();
    case FunctionExpression(:final parameters, :final body):
      properties['parametersOffset'] = parameters?.offset.toString();
      properties['bodyOffset'] = body.offset.toString();
    case ReturnStatement(:final expression):
      properties['expressionOffset'] = expression?.offset.toString();
    case AssertStatement(:final condition, :final message):
      properties['conditionOffset'] = condition.offset.toString();
      properties['messageOffset'] = message?.offset.toString();
    case IfStatement(
      :final expression,
      :final thenStatement,
      :final elseStatement,
    ):
      properties['conditionOffset'] = expression.offset.toString();
      properties['thenOffset'] = thenStatement.offset.toString();
      properties['elseOffset'] = elseStatement?.offset.toString();
    case ExpressionFunctionBody(:final expression):
      properties['expressionOffset'] = expression.offset.toString();
    case BlockFunctionBody(:final block):
      properties['blockOffset'] = block.offset.toString();
    case FunctionDeclaration(:final name, :final isGetter, :final isSetter):
      properties['name'] = name.lexeme;
      properties['isGetter'] = isGetter.toString();
      properties['isSetter'] = isSetter.toString();
    case MethodDeclaration(
      :final name,
      :final isGetter,
      :final isSetter,
      :final isOperator,
    ):
      properties['name'] = name.lexeme;
      properties['isGetter'] = isGetter.toString();
      properties['isSetter'] = isSetter.toString();
      properties['isOperator'] = isOperator.toString();
    case ConstructorDeclaration(
      :final name,
      :final factoryKeyword,
      :final constKeyword,
    ):
      properties['name'] = name?.lexeme ?? 'new';
      properties['isFactory'] = (factoryKeyword != null).toString();
      properties['isConst'] = (constKeyword != null).toString();
    case VariableDeclaration(:final name, :final initializer):
      properties['name'] = name.lexeme;
      properties['initializerOffset'] = initializer?.offset.toString();
    case FieldDeclaration(:final fields, :final isStatic):
      properties['isStatic'] = isStatic.toString();
      properties['isFinal'] = fields.isFinal.toString();
      properties['isConst'] = fields.isConst.toString();
      properties['isLate'] = fields.isLate.toString();
    case ClassDeclaration(
      :final abstractKeyword,
      :final baseKeyword,
      :final finalKeyword,
      :final interfaceKeyword,
      :final sealedKeyword,
    ):
      properties['isAbstract'] = (abstractKeyword != null).toString();
      properties['isBase'] = (baseKeyword != null).toString();
      properties['isFinal'] = (finalKeyword != null).toString();
      properties['isInterface'] = (interfaceKeyword != null).toString();
      properties['isSealed'] = (sealedKeyword != null).toString();
    case InstanceCreationExpression(
      :final constructorName,
      :final argumentList,
    ):
      properties['constructor'] = constructorName.name?.name ?? 'new';
      properties['argumentsOffset'] = argumentList.offset.toString();
      properties['isConst'] = node.keyword?.lexeme == 'const'
          ? 'true'
          : 'false';
    case AsExpression(:final expression, :final type):
      properties['expressionOffset'] = expression.offset.toString();
      properties['typeOffset'] = type.offset.toString();
    case ParenthesizedExpression(:final expression):
      properties['expressionOffset'] = expression.offset.toString();
    case ConstructorFieldInitializer(:final fieldName, :final expression):
      properties['fieldName'] = fieldName.name;
      properties['expressionOffset'] = expression.offset.toString();
    case AssertInitializer(:final condition, :final message):
      properties['conditionOffset'] = condition.offset.toString();
      properties['messageOffset'] = message?.offset.toString();
    case DeclaredVariablePattern(:final name):
      properties['name'] = name.lexeme;
    case DeclaredIdentifier(:final name):
      properties['name'] = name.lexeme;
    case NamedType(:final name, :final question):
      properties['name'] = name.lexeme;
      properties['isNullable'] = (question != null).toString();
    case SimpleFormalParameter(:final name):
      properties['name'] = name?.lexeme;
    case CatchClauseParameter(:final name):
      properties['name'] = name.lexeme;
    case FunctionTypedFormalParameter(:final name):
      properties['name'] = name.lexeme;
    case FieldFormalParameter(:final name):
      properties['name'] = name.lexeme;
    case NamedExpression(:final name, :final expression):
      properties['name'] = name.label.name;
      properties['expressionOffset'] = expression.offset.toString();
    case StringInterpolation(:final elements):
      properties['partCount'] = elements.length.toString();
    case InterpolationExpression(:final expression):
      properties['expressionOffset'] = expression.offset.toString();
    case IsExpression(:final expression, :final notOperator, :final type):
      properties['expressionOffset'] = expression.offset.toString();
      properties['isNot'] = (notOperator != null).toString();
      properties['typeOffset'] = type.offset.toString();
    case ConditionalExpression(
      :final condition,
      :final thenExpression,
      :final elseExpression,
    ):
      properties['conditionOffset'] = condition.offset.toString();
      properties['thenOffset'] = thenExpression.offset.toString();
      properties['elseOffset'] = elseExpression.offset.toString();
    case AwaitExpression(:final expression):
      properties['expressionOffset'] = expression.offset.toString();
    case SwitchStatement(:final expression):
      properties['expressionOffset'] = expression.offset.toString();
    case SwitchExpression(:final expression):
      properties['expressionOffset'] = expression.offset.toString();
    case SwitchExpressionCase(:final expression):
      properties['expressionOffset'] = expression.offset.toString();
    case FunctionDeclarationStatement(:final functionDeclaration):
      properties['functionOffset'] = functionDeclaration.offset.toString();
      final element = functionDeclaration.declaredFragment?.element;
      if (element != null) {
        properties['returnType'] = element.returnType.getDisplayString();
        properties['parameterCount'] = element.formalParameters.length
            .toString();
        for (var index = 0; index < element.formalParameters.length; index++) {
          final parameter = element.formalParameters[index];
          properties['parameter${index}Name'] = parameter.displayName;
          properties['parameter${index}Type'] = parameter.type
              .getDisplayString();
        }
      }
    case ConstructorReference(:final constructorName):
      properties['constructorOffset'] = constructorName.offset.toString();
    default:
      break;
  }
  return properties;
}

List<Map<String, Object>> _statements(AstNode root, String content) {
  final result = <Map<String, Object>>[];
  void visit(AstNode node) {
    if (node is Statement) {
      result.add({
        'kind': node.runtimeType.toString(),
        'offset': node.offset,
        'length': node.length,
        'source': content.substring(node.offset, node.end),
      });
    }
    for (final child in node.childEntities) {
      if (child is AstNode) {
        visit(child);
      }
    }
  }

  visit(root);
  result.sort((a, b) {
    final byOffset = (a['offset'] as int).compareTo(b['offset'] as int);
    if (byOffset != 0) {
      return byOffset;
    }
    return (a['kind'] as String).compareTo(b['kind'] as String);
  });
  return result;
}

Map<String, Object?>? _element(Element? element, String libraryUri) {
  if (element == null) {
    return null;
  }
  final result = <String, Object?>{
    'kind': element.kind.toString(),
    'name': element.displayName,
    'canonicalId': _canonicalElementId(element, fallbackLibraryUri: libraryUri),
    'isDeprecated': element.metadata.hasDeprecated,
  };
  if (element is InterfaceElement) {
    result['type'] = element.thisType.getDisplayString();
    result['supertype'] = element.supertype?.getDisplayString();
    result['mixins'] = element.mixins.map(_typeName).toList();
    result['interfaces'] = element.interfaces.map(_typeName).toList();
    result['typeParameters'] = element.typeParameters
        .map(
          (item) => {
            'name': item.displayName,
            'bound': item.bound?.getDisplayString(),
          },
        )
        .toList();
    result['isAbstract'] = element is ClassElement && element.isAbstract;
    result['isPrivate'] = element.isPrivate;
  } else if (element is TypeAliasElement) {
    final aliasedType = element.aliasedType;
    result['type'] = aliasedType.getDisplayString();
    result['isPrivate'] = element.isPrivate;
    result['typeParameters'] = element.typeParameters
        .map(
          (item) => {
            'name': item.displayName,
            'bound': item.bound?.getDisplayString(),
          },
        )
        .toList();
    if (aliasedType is FunctionType) {
      result['returnType'] = aliasedType.returnType.getDisplayString();
      result['parameters'] = aliasedType.formalParameters.map((item) {
        return {
          'name': item.displayName,
          'type': item.type.getDisplayString(),
          'kind': item.isRequiredNamed
              ? 'required-named'
              : item.isOptionalNamed
              ? 'optional-named'
              : item.isOptionalPositional
              ? 'optional-positional'
              : 'required-positional',
          'defaultValue': item.defaultValueCode,
          'isInitializingFormal': item.isInitializingFormal,
          'isSuperFormal': item.isSuperFormal,
        };
      }).toList();
    }
  } else if (element is ExecutableElement) {
    result['type'] = element.type.getDisplayString();
    result['returnType'] = element.returnType.getDisplayString();
    result['isPrivate'] = element.isPrivate;
    result['typeParameters'] = element.typeParameters
        .map(
          (item) => {
            'name': item.displayName,
            'bound': item.bound?.getDisplayString(),
          },
        )
        .toList();
    result['parameters'] = element.formalParameters.map((item) {
      return {
        'name': item.displayName,
        'type': item.type.getDisplayString(),
        'kind': item.isRequiredNamed
            ? 'required-named'
            : item.isOptionalNamed
            ? 'optional-named'
            : item.isOptionalPositional
            ? 'optional-positional'
            : 'required-positional',
        'defaultValue': item.defaultValueCode,
        'isInitializingFormal': item.isInitializingFormal,
        'isSuperFormal': item.isSuperFormal,
      };
    }).toList();
  } else if (element is VariableElement) {
    result['type'] = element.type.getDisplayString();
    result['isPrivate'] = element.isPrivate;
  }
  return result;
}

List<String> _fragmentDeclarationIds(
  LibraryFragment fragment,
  String ownerLibrary,
) {
  final elements = <Element>[
    ...fragment.classes.map((item) => item.element),
    ...fragment.enums.map((item) => item.element),
    ...fragment.mixins.map((item) => item.element),
    ...fragment.extensions.map((item) => item.element),
    ...fragment.extensionTypes.map((item) => item.element),
    ...fragment.typeAliases.map((item) => item.element),
    ...fragment.functions.map((item) => item.element),
    ...fragment.topLevelVariables.map((item) => item.element),
  ];
  return elements
      .map(
        (item) => _canonicalElementId(item, fallbackLibraryUri: ownerLibrary),
      )
      .toSet()
      .toList()
    ..sort();
}

String _canonicalElementId(Element element, {String? fallbackLibraryUri}) {
  final names = <String>[];
  Element? current = element;
  while (current != null && current is! LibraryElement) {
    if (current.displayName.isNotEmpty) {
      names.add(current.displayName);
    }
    current = current.enclosingElement;
  }
  final libraryUri =
      element.library?.uri.toString() ?? fallbackLibraryUri ?? 'unresolved:';
  return '$libraryUri#${names.reversed.join('.')}';
}

String _typeName(InterfaceType type) => type.getDisplayString();

Future<SomeResolvedUnitResult> _resolveFileWithPackages(
  String path,
  String packagesPath,
) async {
  final resolvedPackagesPath = await _materializeFrameworkPackageConfig(
    packagesPath,
  );
  final collection = AnalysisContextCollectionImpl(
    includedPaths: [path],
    packagesFile: resolvedPackagesPath,
  );
  try {
    return await collection
        .contextFor(path)
        .currentSession
        .getResolvedUnit(path);
  } finally {
    await collection.dispose();
  }
}

/// Resolves relative package roots and optionally swaps in the host Flutter
/// `sky_engine` so `dart:ui` embedder mappings resolve during framework runs.
Future<String> _materializeFrameworkPackageConfig(String packagesPath) async {
  final source = File(packagesPath);
  final decoded =
      jsonDecode(await source.readAsString()) as Map<String, Object?>;
  final packages = (decoded['packages'] as List<Object?>? ?? const <Object?>[])
      .whereType<Map>()
      .map((item) => Map<String, Object?>.from(item))
      .toList();
  final configDirectory = source.parent;
  final flutterRoot = Platform.environment['FLUTTER_ROOT'];
  final hostSkyEngine = flutterRoot == null
      ? null
      : Directory(
          '$flutterRoot${Platform.pathSeparator}bin'
          '${Platform.pathSeparator}cache'
          '${Platform.pathSeparator}pkg'
          '${Platform.pathSeparator}sky_engine',
        );
  final useHostSkyEngine =
      hostSkyEngine != null &&
      File(
        '${hostSkyEngine.path}${Platform.pathSeparator}lib${Platform.pathSeparator}_embedder.yaml',
      ).existsSync();

  for (final package in packages) {
    final name = package['name'] as String?;
    final rootUri = package['rootUri'] as String?;
    if (name == null || rootUri == null) {
      continue;
    }
    if (name == 'sky_engine' && useHostSkyEngine) {
      package['rootUri'] = hostSkyEngine.uri.toString();
      continue;
    }
    final parsed = Uri.parse(rootUri);
    if (parsed.scheme.isEmpty || parsed.scheme == 'file') {
      final resolved = parsed.scheme == 'file'
          ? parsed
          : configDirectory.uri.resolve(rootUri);
      package['rootUri'] = resolved.toString();
    }
  }

  // The checked-in Flutter package configuration deliberately owns only the
  // Flutter/meta/sky_engine roots. Analyzer-only pub dependencies (collection,
  // characters and vector_math for the Scheduler/Services closure) come from
  // this tool's locked package graph so the configuration stays portable and
  // does not embed a developer-specific pub-cache path.
  final toolPackageConfig = File(
    '${configDirectory.path}${Platform.pathSeparator}.dart_tool'
    '${Platform.pathSeparator}package_config.json',
  );
  if (await toolPackageConfig.exists()) {
    final toolDecoded =
        jsonDecode(await toolPackageConfig.readAsString())
            as Map<String, Object?>;
    final existingNames = packages
        .map((item) => item['name'])
        .whereType<String>()
        .toSet();
    for (final rawPackage
        in (toolDecoded['packages'] as List<Object?>? ?? const <Object?>[])
            .whereType<Map>()) {
      final package = Map<String, Object?>.from(rawPackage);
      final name = package['name'] as String?;
      if (name == null || existingNames.contains(name)) {
        continue;
      }
      final rootUri = package['rootUri'] as String?;
      if (rootUri != null) {
        final parsed = Uri.parse(rootUri);
        if (parsed.scheme.isEmpty) {
          package['rootUri'] = toolPackageConfig.parent.uri
              .resolve(rootUri)
              .toString();
        }
      }
      packages.add(package);
      existingNames.add(name);
    }
  }

  final content = const JsonEncoder.withIndent('  ').convert({
    'configVersion': decoded['configVersion'] ?? 2,
    'packages': packages,
  });
  final cache = dorotiCacheDirectory('flutter-package-config-probe');
  final materialized = File(
    '${cache.path}${Platform.pathSeparator}'
    '${dorotiStableKey('${source.absolute.path}\n$content')}.json',
  );
  await materialized.writeAsString(content);
  return materialized.path;
}
