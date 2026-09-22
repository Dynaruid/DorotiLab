// Reuse existing product regression drivers with the installed host Chrome.
// Their imports are node built-ins and their URLs are explicit. No product code
// or browser capability flags are changed by this adapter.
import { readFile } from 'node:fs/promises';
const [kind, ...args] = process.argv.slice(2);
const scripts = {
  textures: 'Doroti/validation/textures/verify-web.mjs',
  webview: 'Doroti/validation/webview/verify-web-product.mjs',
};
if (!scripts[kind]) throw Error('run-contract.mjs textures|webview OUTPUT SERVER_URL BACKEND');
const chrome = process.env.DOROTI_CHROME ?? (process.platform === 'darwin'
  ? '/Applications/Google Chrome.app/Contents/MacOS/Google Chrome'
  : 'C:/Program Files/Google/Chrome/Application/chrome.exe');
let source = (await readFile(scripts[kind], 'utf8'))
  .replace("'C:/Program Files/Google/Chrome/Application/chrome.exe'", JSON.stringify(chrome));
if (process.env.DOROTI_MEMORY_MOBILE === '1') source = source.replace("await cdp('Page.enable');",
  "await cdp('Page.enable');await cdp('Emulation.setUserAgentOverride',{userAgent:'Mozilla/5.0 (iPhone; CPU iPhone OS 26_6_1 like Mac OS X) AppleWebKit/605.1.15 CriOS/150 Mobile',platform:'iPhone'});");
process.argv = [process.argv[0], scripts[kind], ...args];
await import('data:text/javascript;base64,' + Buffer.from(source).toString('base64'));
