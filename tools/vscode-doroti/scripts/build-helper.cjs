const { spawnSync } = require('node:child_process');
const fs = require('node:fs');
const path = require('node:path');
const extension = path.resolve(__dirname, '..');
const project = path.resolve(extension, '../Doroti.Editor.Assist/Doroti.Editor.Assist.csproj');
const result = spawnSync('dotnet', ['publish', project, '-c', 'Release', '--no-self-contained', '-o', path.join(extension, 'helper')], { stdio: 'inherit', windowsHide: true });
if (result.status !== 0) process.exit(result.status ?? 1);
for (const file of ['Doroti.Editor.Assist.dll', 'Doroti.Editor.Assist.runtimeconfig.json', 'Microsoft.CodeAnalysis.CSharp.dll', 'Microsoft.CodeAnalysis.Workspaces.MSBuild.dll', 'BuildHost-netcore/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.dll']) {
    if (!fs.existsSync(path.join(extension, 'helper', file))) throw new Error('Missing packaged editing dependency: ' + file);
}
