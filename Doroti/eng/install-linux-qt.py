"""Install a locally qualified, unsigned Linux Qt candidate with atomic version selection.

The candidate contains candidate.json and publish/. System Qt and .NET remain external.
No OS protocol association is changed; the generated desktop entry may be installed by
an application distributor using its chosen package manager.
"""
from pathlib import Path, PurePosixPath
import argparse
import hashlib
import json
import os
import re
import shlex
import shutil
import tempfile

MARKER = 'Doroti Linux Qt portable installation v1'
VERSION = r'[0-9]+\.[0-9]+\.[0-9]+(?:[-+][A-Za-z0-9.-]+)?'

def selection(root, path):
    if not path.is_symlink(): raise ValueError('Selection is not an owned symlink: ' + str(path))
    target = Path(os.readlink(path))
    if target.is_absolute() or len(target.parts) != 2 or target.parts[0] != 'versions' or not re.fullmatch(VERSION, target.parts[1]):
        raise ValueError('Invalid selection target: ' + str(target))
    resolved = (root / target).resolve(strict=True)
    if resolved.parent != root.resolve() / 'versions' or not resolved.is_dir():
        raise ValueError('Selection escapes the owned version store.')
    return target

def recover(root):
    link = root / 'current.new'
    if link.exists() or link.is_symlink(): selection(root, link); link.unlink()
    for name, prefix in [('run.new', '#!/bin/sh\nset -eu\nbase='), ('application.desktop.new', '[Desktop Entry]\nType=Application\n')]:
        path = root / name
        if path.exists():
            if not path.is_file() or path.is_symlink() or not path.read_text().startswith(prefix):
                raise ValueError('Unowned installer temporary file: ' + str(path))
            path.unlink()
    for path in root.glob('.staging-*'):
        marker = path / '.doroti-stage'
        if marker.is_file() and not marker.is_symlink() and marker.read_text() == MARKER:
            if path.is_symlink() or any(item.is_symlink() for item in path.rglob('*')):
                raise ValueError('Linked installer staging directory.')
            shutil.rmtree(path)

def digest(path):
    with path.open('rb') as stream:
        return hashlib.file_digest(stream, 'sha256').hexdigest()

def verify(directory, payload):
    if directory.is_symlink(): raise ValueError("Payload directory cannot be a link.")
    actual = set()
    for file in directory.rglob('*'):
        if file.is_symlink(): raise ValueError('Payload links are not supported: ' + str(file))
        if file.is_file(): actual.add(file.relative_to(directory).as_posix())
    if actual != set(payload): raise ValueError('Payload file set differs from its manifest.')
    for name, expected in payload.items():
        relative = PurePosixPath(name)
        if relative.is_absolute() or '..' in relative.parts or '\\' in name:
            raise ValueError('Invalid payload path: ' + name)
        if digest(directory / name) != expected: raise ValueError('Payload hash mismatch: ' + name)

def owned(root):
    if any(path.is_symlink() for path in [root, *root.parents]) or root == Path(root.anchor): raise ValueError('Use a dedicated installation directory without linked ancestors.')
    marker = root / '.doroti-install.json'
    if root.exists() and (not marker.is_file() or marker.is_symlink() or marker.read_text() != MARKER):
        raise ValueError('Directory is not owned by this installer.')
    root.mkdir(parents=True, exist_ok=True)
    marker.write_text(MARKER)
    for file in root.rglob('*'):
        if file.is_symlink():
            if file not in (root / 'current', root / 'current.new'): raise ValueError('Unexpected installation symlink: ' + str(file))
            selection(root, file)
    recover(root)

def install(candidate, root, executable, scheme=None):
    manifest = json.loads((candidate / 'candidate.json').read_text())
    version = manifest['version']
    if not re.fullmatch(VERSION, version): raise ValueError('Invalid version.')
    if '/' in executable or not re.fullmatch(r'[A-Za-z0-9_.-]+', executable): raise ValueError('Invalid executable name.')
    if scheme and (not re.fullmatch(r'[a-z][a-z0-9+.-]*', scheme) or scheme in ['http','https','file','data','javascript']):
        raise ValueError('Use an application-specific URI scheme.')
    verify(candidate / 'publish', manifest['payload'])
    if executable not in manifest['payload']: raise ValueError('Executable absent from payload.')
    owned(root)
    if (root / 'current').is_symlink() and not (root / 'current' / executable).is_file():
        raise ValueError('Changing the stable executable name requires removing the old binaries first; userdata is retained.')
    versions = root / 'versions'; versions.mkdir(exist_ok=True)
    staging = Path(tempfile.mkdtemp(prefix='.staging-', dir=root))
    (staging / '.doroti-stage').write_text(MARKER)
    payload = staging / 'payload'
    target = versions / version
    try:
        shutil.copytree(candidate / 'publish', payload)
        verify(payload, manifest['payload'])
        if target.exists(): verify(target, manifest['payload'])
        else: os.replace(payload, target)
        # Stable launcher keeps application data across upgrades and uninstall.
        launcher = '#!/bin/sh\nset -eu\nbase=' + shlex.quote(str(root)) + '\n'
        launcher += 'mkdir -p "$base/userdata"\nexport XDG_DATA_HOME="$base/userdata"\nexec "$base/current/' + executable + '" "$@"\n'
        (root / 'run.new').write_text(launcher); (root / 'run.new').chmod(0o755)
        # This file is an artifact, not a global association. Quoted Exec follows
        # the desktop-entry argument escaping rules, including percent literals.
        quoted = str(root / 'run').replace('\\', '\\\\').replace('"', '\\"').replace('`', '\\`').replace('$', '\\$').replace('%', '%%')
        desktop = '[Desktop Entry]\nType=Application\nName=' + executable + '\nExec="' + quoted + '" %u\nTerminal=false\n'
        if scheme: desktop += 'MimeType=x-scheme-handler/' + scheme + ';\n'
        (root / 'application.desktop.new').write_text(desktop)
        if (root / 'current.new').exists() or (root / 'current.new').is_symlink(): (root / 'current.new').unlink()
        (root / 'current.new').symlink_to(Path('versions') / version)
        old_files = {name: (root / name).read_bytes() if (root / name).is_file() else None for name in ['run', 'application.desktop']}
        old_selection = selection(root, root / 'current') if (root / 'current').is_symlink() else None
        try:
            os.replace(root / 'run.new', root / 'run')
            os.replace(root / 'application.desktop.new', root / 'application.desktop')
            # Commit point: the version selection changes only after both stable artifacts exist.
            os.replace(root / 'current.new', root / 'current')
        except BaseException:
            for name, content in old_files.items():
                path = root / name
                if content is None: path.unlink(missing_ok=True)
                else:
                    path.write_bytes(content)
                    if name == 'run': path.chmod(0o755)
            # An exception raised just after replace still restores the old selection.
            current = root / 'current'
            if current.is_symlink(): current.unlink()
            if old_selection is not None: current.symlink_to(old_selection)
            raise
    finally:
        if staging.exists(): shutil.rmtree(staging)
    return version

def remove(root):
    if not root.exists(): return
    owned(root)
    if (root / 'versions').exists(): shutil.rmtree(root / 'versions')
    for name in ['current', 'run', 'application.desktop']:
        path = root / name
        if path.exists() or path.is_symlink(): path.unlink()
    # userdata and ownership marker intentionally survive uninstall.

if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('action', choices=['install','remove'])
    parser.add_argument('--root', required=True, type=Path)
    parser.add_argument('--candidate', type=Path)
    parser.add_argument('--executable', default='DorotiTemplateApp.Linux')
    parser.add_argument('--protocol-scheme')
    args = parser.parse_args()
    root = args.root.absolute()
    if args.action == 'install':
        if args.candidate is None: parser.error('--candidate is required for install')
        print('Installed', install(args.candidate.resolve(), root, args.executable, args.protocol_scheme))
    else: remove(root); print('Removed binaries; userdata retained.')
