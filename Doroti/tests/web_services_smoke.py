"""Actual Chrome + threaded runtime regression, separate from synthetic Node contracts.

Prerequisites: pip install playwright; installed Chrome; built Testbed Web served at --url.
Run through eng/run-with-timeout.py. Mouse/keyboard and DOM drops are automated, not physical OS input.
"""
import argparse
import json
from pathlib import Path
from playwright.sync_api import sync_playwright

parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--url', default='http://127.0.0.1:5199/')
parser.add_argument('--renderer', choices=['webgl', 'webgpu'], default='webgl')
parser.add_argument('--output', type=Path, required=True)
args = parser.parse_args()
root = Path(__file__).resolve().parents[2]
run = args.output.resolve()
if not run.is_relative_to(root / 'temp/testing'):
    raise RuntimeError('Evidence must remain under temp/testing.')
run.mkdir(parents=True, exist_ok=True)
file = run / 'selected.txt'
file.write_text('Doroti file 한글', encoding='utf-8')

with sync_playwright() as p:
    browser = p.chromium.launch(channel='chrome', headless=True, args=['--enable-unsafe-webgpu'])
    page = browser.new_page(viewport={'width': 1000, 'height': 800})
    errors = []
    page.on('pageerror', lambda error: errors.append(str(error)))
    page.on('console', lambda message: errors.append(message.text) if message.type == 'error' else None)

    def labels():
        return page.locator('[aria-label]').evaluate_all('(es)=>es.map(e=>e.getAttribute("aria-label"))')

    def open_sample(name):
        page.goto(args.url + '?dorotiSample=' + name + '&dorotiRenderer=' + args.renderer, wait_until='networkidle')
        page.wait_for_function('document.documentElement.dataset.dorotiBootstrapStage === "started"')
        page.wait_for_timeout(500)

    def click(name):
        # Semantics geometry is transparent to pointer hit testing; send input to the actual canvas.
        box = page.get_by_role('button', name=name, exact=True).bounding_box()
        assert box is not None, (name, labels())
        page.mouse.click(box['x'] + box['width'] / 2, box['y'] + box['height'] / 2, delay=100)

    def label_contains(text):
        page.wait_for_function('(text)=>Array.from(document.querySelectorAll("[aria-label]")).some(e=>e.getAttribute("aria-label").includes(text))', arg=text)

    try:
        open_sample('plugins')
        with page.expect_file_chooser() as chooser:
            click('Choose files')
        chooser.value.set_files(str(file))
        label_contains('selected.txt: 18 bytes; 446F726F74692066696C6520ED959CEA')
        with page.expect_file_chooser() as chooser:
            click('Choose files')
        chooser.value.set_files([])
        label_contains('cancelled')
        print('PASS: real browser picker, C# plugin read/release and selection cancellation', flush=True)

        open_sample('navigation')
        click('First page'); page.wait_for_url('**#/first')
        click('Second page'); page.wait_for_url('**#/second')
        page.go_back(); label_contains('#/first')
        page.go_forward(); label_contains('#/second')
        box = page.get_by_role('textbox').bounding_box()
        assert box and box['height'] < 200, 'TextField semantics absorbed the whole page.'
        page.mouse.click(box['x'] + 30, box['y'] + box['height'] / 2, delay=100)
        page.keyboard.insert_text('restore 한글')
        page.wait_for_function('document.querySelector("#doroti-ime").value === "restore 한글"')
        page.keyboard.press('Home'); page.keyboard.press('Shift+ArrowRight')
        page.wait_for_function('history.state?.doroti?.state?.includes("restore")')
        page.reload(wait_until='networkidle')
        page.wait_for_function('document.querySelector("[role=textbox]")?.value === "restore 한글"')
        assert '#/second' in page.url
        print('PASS: URL/Router, real Back/Forward, text and route reload restoration, isolated text semantics', flush=True)

        open_sample('drop')
        page.evaluate('''() => {
            const transfer = new DataTransfer();
            Object.defineProperty(transfer, 'effectAllowed', { value: 'copy' });
            transfer.items.add(new File(['drop bytes'], 'drop.txt', {type:'text/plain'}));
            transfer.setData('text/plain', '한글 drop');
            transfer.setData('text/uri-list', 'https://example.com/drop');
            const canvas = document.querySelector('canvas');
            canvas.dispatchEvent(new DragEvent('dragover', {dataTransfer:transfer,clientX:100,clientY:100,bubbles:true,cancelable:true}));
            canvas.dispatchEvent(new DragEvent('drop', {dataTransfer:transfer,clientX:100,clientY:100,bubbles:true,cancelable:true}));
        }''')
        label_contains('Completed drops: 1'); label_contains('drop.txt: 10 bytes')
        label_contains('한글 drop')
        print('PASS: synthetic DOM Copy drop through worker, managed receiver and file grant', flush=True)
        assert not errors, errors
        (run / 'result.json').write_text(json.dumps({'renderer': args.renderer, 'browser': browser.version,
            'status': 'PASS', 'physicalInput': 'notVerified', 'errors': errors}, indent=2))
    except BaseException:
        page.screenshot(path=str(run / 'failure.png'))
        (run / 'errors.json').write_text(json.dumps({'errors': errors, 'labels': labels()}, ensure_ascii=False, indent=2))
        raise
    finally:
        browser.close()
