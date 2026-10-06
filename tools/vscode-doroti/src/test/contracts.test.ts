import { test } from 'node:test';
import * as assert from 'node:assert/strict';
import * as path from 'node:path';
import { validateName, parseProject, classifyOutput } from '../contracts';
import { importPlan } from '../imports';
test('names cannot escape output folder or name Windows devices', () => {
    for (const value of ['../app', 'x/y', 'x\\y', 'con', 'NUL.txt', 'Foo.', 'a..b', '1app', 'a;echo', 'a.1b', 'class', 'A.namespace']) assert.ok(validateName(value), value);
    for (const value of ['한글앱', 'My.App', 'Widget_2']) assert.equal(validateName(value), undefined);
});
test('provider metadata is authoritative for arbitrary aliases, operations and transports', () => {
    const alias = 'laboratory-x';
    const runner = path.resolve('app.csproj');
    const support = { platform: alias, runner, provider: 'external', version: '1.0.0', operations: ['build', 'dev'], development: { transport: 'file', debug: true, requiresPreparedAcknowledgment: false, usesStopSignal: false, launchBrowser: false } };
    const project = { schemaVersion: 'doroti.cli-workspace/v2', root: path.resolve('.'), applicationProject: path.resolve('app.csproj'), platforms: { [alias]: runner }, developmentTargets: [alias], developmentSupport: [support] };
    assert.deepEqual(parseProject(JSON.stringify(project)).developmentTargets, [alias]);
    for (const bad of [{ schemaVersion: 'doroti.cli-workspace/v1' }, { developmentTargets: ['unknown'] }, { developmentSupport: [support, support] }, { developmentSupport: [{ ...support, operations: ['build'] }] }, { developmentSupport: [{ ...support, development: { ...support.development, transport: 'invalid' } }] }, { platforms: { '../escape': runner } }, { platforms: { [alias]: 'relative.csproj' } }])
        assert.throws(() => parseProject(JSON.stringify({ ...project, ...bad })));
    for (const transport of ['none', 'browser', 'device'])
        assert.equal(parseProject(JSON.stringify({ ...project, developmentSupport: [{ ...support, development: { ...support.development, transport } }] })).developmentSupport[0].development.transport, transport);
});
test('watcher errors do not masquerade as applied reloads', () => {
    assert.equal(classifyOutput('error CS1002: ; expected'), 'compile-error');
    assert.equal(classifyOutput('Do you want to restart your app?'), 'restart-required');
    assert.equal(classifyOutput("Further changes won't be applied to this process."), 'restart-required');
    assert.equal(classifyOutput('Hot reload succeeded'), undefined);
});
test('imports avoid duplicates, aliases and local type conflicts', () => {
    const ns = 'Doroti.Framework.Widgets';
    for (const source of [`using ${ns};\nclass App {}`, `global using ${ns};\nnamespace App;`]) assert.equal(importPlan(source, 'Text', ns).text, undefined);
    assert.equal(importPlan('namespace App;', 'Text', ns, [ns]).text, undefined);
    assert.ok(importPlan('namespace App;', 'Text', ns, ['Text=Other.Text']).qualified);
    assert.ok(importPlan('using Text = Other.Text;\nnamespace App;', 'Text', ns).qualified);
    assert.ok(importPlan('class Text {}', 'Text', ns).qualified);
    assert.ok(importPlan('// using Doroti.Framework.Widgets;\nnamespace App;', 'Text', ns).text);
    assert.equal(importPlan('namespace App {\nusing System;\n}', 'Text', ns).offset, 0);
    assert.ok(importPlan(`namespace Other {\nusing ${ns};\n}\nnamespace App {}`, 'Text', ns).qualified);
    const source = 'using System;\nnamespace App;\n';
    const plan = importPlan(source, 'Text', ns);
    assert.equal(source.slice(0, plan.offset) + plan.text + source.slice(plan.offset), `using System;\nusing ${ns};\nnamespace App;\n`);
});
