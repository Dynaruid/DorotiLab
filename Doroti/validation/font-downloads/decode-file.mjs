import fs from 'node:fs/promises';
import { decode } from '../../src/Doroti.Host.Web/obj/Release/net10.0/Doroti.Web/wwwroot/doroti.web.fonts.js';
const decoder = new URL('../../artifacts/font-downloads/decoder.mjs', import.meta.url);
try { await fs.access(decoder); } catch {
  const response = await fetch('https://cdn.jsdelivr.net/npm/woff2-encoder@2.0.0/dist/decompress.js');
  if (!response.ok) throw new Error(`Decoder CDN: HTTP ${response.status}`);
  await fs.writeFile(decoder, new Uint8Array(await response.arrayBuffer()));
}
await fs.writeFile(process.argv[3], await decode(new Uint8Array(await fs.readFile(process.argv[2])), decoder.href));
