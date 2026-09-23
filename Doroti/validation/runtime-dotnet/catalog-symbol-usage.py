"""Count lexical Runtime type mentions outside Runtime in one checkout.

This is an impact hint, not a Roslyn symbol-reference result. Names in comments,
strings, or unrelated namespaces can count; aliases and extension calls may not.
"""

import argparse
import csv
from collections import defaultdict
from pathlib import Path
import re
import subprocess


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--repo-root', type=Path, required=True)
    parser.add_argument('--api-snapshot', type=Path, required=True)
    parser.add_argument('--output', type=Path, required=True)
    args = parser.parse_args()
    root = args.repo_root.resolve()
    snapshot = args.api_snapshot.resolve().read_text(encoding='utf-8').splitlines()
    types = [line.removeprefix('TYPE Doroti.Runtime.') for line in snapshot if line.startswith('TYPE ')]
    names = {item: item.split('+')[-1].split('`')[0] for item in types}
    types_by_name = defaultdict(list)
    for item, name in names.items():
        types_by_name[name].append(item)
    pattern = re.compile(
        r'(?<![\w])(?:' + '|'.join(re.escape(name) for name in sorted(
            types_by_name, key=len, reverse=True
        )) + r')(?![\w])'
    )

    listed = subprocess.check_output(
        ['rg', '--files', '-g', '*.cs', 'Doroti', 'DorotiTestbedApp', 'tools'],
        cwd=root, text=True
    ).splitlines()
    counts = {item: dict.fromkeys(
        ['product', 'validation', 'tool', 'template', 'sample', 'other'], 0
    ) for item in types}
    projects = {item: set() for item in types}
    for name in listed:
        path = Path(name)
        if any(part in {'bin', 'obj', 'artifacts'} for part in path.parts):
            continue
        if len(path.parts) >= 3 and path.parts[:3] == ('Doroti', 'src', 'Doroti.Runtime'):
            continue
        content = (root / path).read_text(encoding='utf-8-sig')
        matched = set(pattern.findall(content))
        parts = path.parts
        if len(parts) > 2 and parts[:2] == ('Doroti', 'src'):
            group = 'product'
        elif len(parts) > 1 and parts[:2] == ('Doroti', 'validation'):
            group = 'validation'
        elif len(parts) > 1 and parts[:2] == ('Doroti', 'templates'):
            group = 'template'
        elif parts[0] == 'DorotiTestbedApp':
            group = 'sample'
        elif parts[0] == 'tools':
            group = 'tool'
        else:
            group = 'other'
        for matched_name in matched:
            for item in types_by_name[matched_name]:
                counts[item][group] += 1
                if group == 'product':
                    projects[item].add(parts[2])

    args.output.parent.mkdir(parents=True, exist_ok=True)
    with args.output.open('w', newline='', encoding='utf-8') as stream:
        writer = csv.writer(stream, delimiter='\t')
        writer.writerow(['type', 'product_files', 'validation_files', 'tool_files',
                         'template_files', 'sample_files', 'other_files', 'product_projects'])
        for item in sorted(types):
            row = counts[item]
            writer.writerow([item, row['product'], row['validation'], row['tool'],
                             row['template'], row['sample'], row['other'],
                             ','.join(sorted(projects[item]))])
    print(f'{len(types)} type rows written to {args.output}')


if __name__ == '__main__':
    main()
