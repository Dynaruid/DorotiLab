"""Sample2's native file picker through the real canvas tap and managed Worker.

Uses browser input automation, not physical iPhone input. Run against a frozen,
isolated Sample2 Web build. Every run closes its owned browser and tab.
"""
import argparse
import json
from pathlib import Path

from playwright.sync_api import sync_playwright

parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--url', required=True)
parser.add_argument('--browser', choices=['chromium', 'webkit'], default='chromium')
parser.add_argument('--output', type=Path, required=True)
args = parser.parse_args()
root = Path(__file__).resolve().parents[2]
run = args.output.resolve()
if not run.is_relative_to(root / 'temp/testing'):
    raise RuntimeError('Evidence must remain under temp/testing.')
run.mkdir(parents=True, exist_ok=True)
selected = run / 'selected.txt'
selected.write_text('Doroti file 한글', encoding='utf-8')

with sync_playwright() as p:
    browser = getattr(p, args.browser).launch(headless=True)
    page = browser.new_page(viewport={'width': 390, 'height': 750})
    errors = []
    page.on('pageerror', lambda error: errors.append(str(error)))
    page.set_default_timeout(30000)
    page.add_init_script('''(() => {
        let dispatching = false;
        let eventType = '';
        window.pickerCalls = [];
        for (const type of ['pointerup', 'click', 'keydown']) {
            document.addEventListener(type, e => { dispatching = e.isTrusted; eventType = type; }, true);
            document.addEventListener(type, () => { dispatching = false; });
        }
        const click = HTMLInputElement.prototype.click;
        HTMLInputElement.prototype.click = function() {
            if (this.type !== 'file') return click.call(this);
            window.pickerCalls.push({dispatching, eventType, accept: this.accept, multiple: this.multiple});
            return click.call(this);
        };
    })()''')

    def click(label):
        box = page.locator('[aria-label=' + json.dumps(label, ensure_ascii=False) + ']').bounding_box()
        assert box, label
        page.mouse.click(box['x'] + box['width'] / 2, box['y'] + box['height'] / 2, delay=80)

    def contains(text):
        page.wait_for_function('text => [...document.querySelectorAll("[aria-label]")].some(e => e.getAttribute("aria-label").includes(text))', arg=text)

    try:
        page.goto(args.url + '?dorotiRenderer=worker-direct-webgl', wait_until='networkidle')
        page.wait_for_function('document.documentElement.dataset.dorotiBootstrapStage === "started"')
        page.wait_for_timeout(500)
        click('Upload')
        contains('파일 선택')
        # A scroll/drag that starts on the button must not reserve a native picker.
        box = page.get_by_role('button', name='파일 선택', exact=True).bounding_box()
        page.mouse.move(box['x'] + box['width'] / 2, box['y'] + box['height'] / 2)
        page.mouse.down()
        page.mouse.move(box['x'] + box['width'] / 2 + 35, box['y'] + box['height'] / 2, steps=5)
        page.mouse.up()
        assert page.evaluate('pickerCalls.length') == 0

        for attempt in range(2):
            with page.expect_file_chooser() as chooser:
                click('파일 선택')
            assert chooser.value.is_multiple()
            chooser.value.set_files(str(selected))
            contains('selected.txt')
            contains('Doroti file 한글')
            contains(f'현재 {attempt + 1}개')
        with page.expect_file_chooser() as chooser:
            click('파일 선택')
        chooser.value.set_files([])
        contains('파일 선택을 취소했습니다.')

        # Keyboard/accessibility semantics activation uses the same reservation.
        button = page.get_by_role('button', name='파일 선택', exact=True)
        button.focus()
        with page.expect_file_chooser() as chooser:
            page.keyboard.press('Enter')
        chooser.value.set_files(str(selected))
        contains('현재 3개')
        calls = page.evaluate('pickerCalls')
        assert len(calls) == 4, calls
        assert all(call['dispatching'] for call in calls), calls
        assert all('.txt' in call['accept'] and call['multiple'] for call in calls), calls
        assert not errors, errors
        (run / 'result.json').write_text(json.dumps({'status': 'PASS', 'browser': args.browser,
            'version': browser.version, 'physicalInput': 'notVerified', 'calls': calls, 'errors': errors}, indent=2))
        print('PASS: synchronous trusted canvas tap/keyboard picker, filters, repeated selection, managed preview, cancellation and drag rejection', flush=True)
    except BaseException:
        page.screenshot(path=str(run / 'failure.png'))
        (run / 'failure.json').write_text(json.dumps({'url': page.url, 'errors': errors,
            'dataset': page.evaluate('({...document.documentElement.dataset})'),
            'labels': page.locator('[aria-label]').evaluate_all('(es)=>es.map(e=>e.getAttribute("aria-label"))'),
            'calls': page.evaluate('pickerCalls')}, ensure_ascii=False, indent=2))
        raise
    finally:
        browser.close()
