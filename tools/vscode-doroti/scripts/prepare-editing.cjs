const fs = require('node:fs');
const path = require('node:path');
const root = path.resolve(__dirname, '../../..');
const output = path.resolve(process.argv[2]);
fs.mkdirSync(path.join(output, 'src'), { recursive: true });
fs.mkdirSync(path.join(output, 'lib'), { recursive: true });
const libraries = [path.join(root, 'tools/Doroti.Editor.Assist/tests/bin/Release/net10.0'), path.join(root, 'packages/Doroti.Material/bin/Release/net10.0')];
for (const directory of libraries) {
    for (const file of fs.readdirSync(directory)) {
        if (/^Doroti\..*\.(dll|xml)$/.test(file) && !file.startsWith('Doroti.Editor.')) fs.copyFileSync(path.join(directory, file), path.join(output, 'lib', file));
    }
}
const references = fs.readdirSync(path.join(output, 'lib')).filter(f => f.endsWith('.dll')).map(f => `    <Reference Include="${f.slice(0, -4)}"><HintPath>lib/${f}</HintPath></Reference>`).join('\n');
fs.writeFileSync(path.join(output, 'EditorFixture.csproj'), `<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net10.0</TargetFramework><LangVersion>14.0</LangVersion><Nullable>enable</Nullable><ImplicitUsings>enable</ImplicitUsings><EnableDefaultCompileItems>false</EnableDefaultCompileItems></PropertyGroup><ItemGroup><Compile Include="src/**/*.cs" />\n${references}\n</ItemGroup></Project>\n`);
fs.writeFileSync(path.join(output, 'doroti-workspace.json'), JSON.stringify({ schemaVersion: 'doroti.workspace/v2', applicationProject: 'EditorFixture.csproj', platforms: {} }, null, 2));
fs.writeFileSync(path.join(output, 'src/EditorProbe.cs'), 'namespace Probe;\n');
fs.mkdirSync(path.join(output, 'unrelated'), { recursive: true });
fs.writeFileSync(path.join(output, 'unrelated/Plain.csproj'), '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>');
fs.writeFileSync(path.join(output, 'unrelated/Plain.cs'), 'class Plain { }');
console.log(output);
