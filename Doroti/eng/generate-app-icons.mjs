// Run with a path to an installed Playwright package; no browser dependency is
// needed when building applications. See docs/branding/README.md.
import { createRequire } from 'node:module';
import { readFile, writeFile, mkdir } from 'node:fs/promises';
import { dirname, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';

const root = resolve(dirname(fileURLToPath(import.meta.url)), '../..');
const require = createRequire(import.meta.url);
const { chromium } = require(process.argv[2] || 'playwright');
const output = resolve(root, 'Doroti/src/Doroti.Runner.Sdk/Sdk/Icons');
const svg = await readFile(resolve(root, 'Doroti/docs/branding/doroti-app-icon.svg'), 'utf8');
const brandSvg = await readFile(resolve(root, 'Doroti/docs/branding/doroti-symbol-color.svg'), 'utf8');
await mkdir(output, { recursive: true });
const browser = await chromium.launch(process.argv[3] ? { channel: process.argv[3] } : {});
try {
  const page = await browser.newPage({ deviceScaleFactor: 1 });
  await page.setContent(svg);
  const { foreground, background } = await page.evaluate(() => {
    const source = document.querySelector('svg');
    const backdrop = source.querySelector(':scope > rect');
    if (!backdrop || backdrop.getAttribute('fill') !== '#512BD4') {
      throw new Error('The app icon must have a background rectangle filled with #512BD4.');
    }
    const background = backdrop.getAttribute('fill');
    backdrop.remove();
    return { foreground: new XMLSerializer().serializeToString(source), background };
  });
  async function render(source, size) {
    await page.setViewportSize({ width: size, height: size });
    await page.setContent(`<style>html,body{margin:0;background:transparent}svg{display:block;width:${size}px;height:${size}px}</style>${source}`);
    return page.screenshot({ omitBackground: true });
  }
  const images = new Map();
  for (const size of [16, 24, 32, 48, 64, 128, 256, 512, 1024]) {
    images.set(size, await render(svg, size));
  }
  // Keep the adaptive foreground transparent so platform scaling does not shrink
  // the purple background into a second square inside the launcher mask.
  await writeFile(resolve(output, 'appiconfg.png'), await render(foreground, 1024));
  await writeFile(resolve(output, 'appicon.svg'), `<svg xmlns="http://www.w3.org/2000/svg" width="1024" height="1024" viewBox="0 0 1024 1024"><rect width="1024" height="1024" fill="${background}"/></svg>\n`);
  await writeFile(resolve(output, 'appicon.png'), images.get(512));
  await writeFile(resolve(output, 'favicon.svg'), svg);
  await writeFile(resolve(root, 'Doroti/tools/vscode-doroti/images/icon.png'), await render(brandSvg, 256));
  // ICO directory entries point at individual PNG frames; Windows selects the
  // frame appropriate for Explorer, the taskbar and the window's current DPI.
  const sizes = [16, 24, 32, 48, 64, 128, 256];
  const header = Buffer.alloc(6 + sizes.length * 16);
  header.writeUInt16LE(1, 2);
  header.writeUInt16LE(sizes.length, 4);
  let offset = header.length;
  sizes.forEach((size, index) => {
    const entry = 6 + index * 16;
    header[entry] = header[entry + 1] = size % 256;
    header.writeUInt16LE(1, entry + 4);
    header.writeUInt16LE(32, entry + 6);
    header.writeUInt32LE(images.get(size).length, entry + 8);
    header.writeUInt32LE(offset, entry + 12);
    offset += images.get(size).length;
  });
  await writeFile(resolve(output, 'appicon.ico'), Buffer.concat([header, ...sizes.map(size => images.get(size))]));
  // Modern macOS ICNS PNG representations, including Retina sizes.
  const chunks = [['icp4',16], ['icp5',32], ['icp6',64], ['ic07',128], ['ic08',256], ['ic09',512], ['ic10',1024], ['ic11',32], ['ic12',64], ['ic13',256], ['ic14',512]].map(([kind, size]) => {
    const chunk = Buffer.alloc(8);
    chunk.write(kind);
    chunk.writeUInt32BE(8 + images.get(size).length, 4);
    return Buffer.concat([chunk, images.get(size)]);
  });
  const icns = Buffer.alloc(8);
  icns.write('icns');
  icns.writeUInt32BE(8 + chunks.reduce((sum, chunk) => sum + chunk.length, 0), 4);
  await writeFile(resolve(output, 'appicon.icns'), Buffer.concat([icns, ...chunks]));
  console.log(`Generated Doroti app icons in ${output}`);
} finally {
  await browser.close();
}
