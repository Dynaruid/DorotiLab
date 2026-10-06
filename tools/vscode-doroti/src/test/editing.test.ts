import { test } from 'node:test';
import * as assert from 'node:assert/strict';
import * as path from 'node:path';
import { loadTemplates, instantiate, isCode, validateIdentifier, defaultName } from '../editing/catalog';
const templates = loadTemplates(path.resolve(__dirname, '../..'));
test('one catalog owns aliases and resolves C# identifiers and Unicode names', () => {
    assert.deepEqual(templates.StatelessWidget.prefix, ['stl', 'stless', 'dstateless']);
    assert.deepEqual(templates.StatefulWidget.prefix, ['stf', 'stful', 'dstateful']);
    for (const value of ['class', '1Widget', 'A.B', '../Widget']) assert.ok(validateIdentifier(value));
    for (const value of ['한글위젯', 'Widget_2']) assert.equal(validateIdentifier(value), undefined);
    assert.equal(defaultName('C:/src/한글위젯.cs'), '한글위젯');
    const prefixes = Object.values(templates).flatMap(t => Array.isArray(t.prefix) ? t.prefix : [t.prefix]);
    assert.equal(new Set(prefixes).size, prefixes.length);
});
test('class snippets keep linked names and combine imports once with CRLF/BOM', () => {
    const source = '\uFEFFnamespace App;\r\n';
    const result = instantiate(templates.StatefulWidget, source, 'Counter', []);
    assert.ok(result.body.includes('${1:Counter}') && result.body.includes('State<${1}>') && result.body.includes('new ${1}State()'));
    assert.equal(result.edits.length, 1); assert.equal(result.edits[0].start, 1);
    assert.equal(result.edits[0].text, 'using Doroti.Framework.Widgets;\r\n');
    const qualified = instantiate(templates.StatelessWidget, source, 'Counter', [], ['Text']);
    assert.ok(qualified.body.includes('global::Doroti.Framework.Widgets.Text'));
    const restricted = instantiate(templates.StatelessWidget, source, 'Counter', [], [], true);
    assert.equal(restricted.edits.length, 0); assert.ok(restricted.body.includes('global::Doroti.Framework.Widgets.StatelessWidget'));
    const restrictedState = instantiate(templates.StatefulWidget, source, 'Text', [], [], true);
    assert.ok(restrictedState.body.includes('new ${1}State()') && restrictedState.body.includes('class ${1}State') && restrictedState.body.includes('${1:Text}'));
});
test('fallback lexical guard excludes comments, raw/verbatim/interpolated strings and conditional regions', () => {
    for (const text of ['// stl', '/* stl */', '"stl"', '@"stl"', '$"stl"', '"""stl"""', '#if X\nstl\n#endif']) assert.equal(isCode(text, text.indexOf('stl') + 2), false, text);
    assert.equal(isCode('namespace App;\nstl', 18), true);
});
