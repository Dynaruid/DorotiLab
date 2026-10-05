// CSSOM validates declarations; the small scanner only preserves rule boundaries,
// imports and source lists (including quoted commas, comments and escapes).
export interface CssFontOptions { discover: boolean; sameOriginOnly: boolean; stylesheets: string[]; }
export interface CssFontFace {
  family: string; weight: string; style: string; stretch: string; unicodeRange: string;
  variationSettings: string; stylesheet: string; sources: string[];
}
export function splitCss(text: string, separator: string): string[] {
  const parts: string[] = []; let start = 0, quote = "", depth = 0;
  for (let i = 0; i < text.length; i++) {
    const c = text[i];
    if (c === "\\") { i++; continue; }
    if (quote) { if (c === quote) quote = ""; continue; }
    if (c === '"' || c === "'") { quote = c; continue; }
    if (c === "/" && text[i + 1] === "*") { const end = text.indexOf("*/", i + 2); i = end < 0 ? text.length : end + 1; continue; }
    if (c === "(" || c === "{" || c === "[") depth++;
    if (c === ")" || c === "}" || c === "]") depth--;
    if (c === separator && depth === 0) { parts.push(text.slice(start, i)); start = i + 1; }
  }
  parts.push(text.slice(start)); return parts;
}
function unquote(value: string): string {
  value = value.trim();
  if ((value[0] === '"' || value[0] === "'") && value.at(-1) === value[0]) value = value.slice(1, -1);
  return value.replace(/\\([0-9a-f]{1,6}\s?|.)/gi, (_, escaped: string) =>
    /^[0-9a-f]/i.test(escaped) ? String.fromCodePoint(parseInt(escaped.trim(), 16) || 0xfffd) : escaped);
}
function sourceUrl(value: string): { url: string; rest: string } | null {
  const v = value.trim(); const quote = v[0];
  if (quote === '"' || quote === "'") {
    for (let i = 1; i < v.length; i++) {
      if (v[i] === "\\") { i++; continue; }
      if (v[i] === quote) return { url: unquote(v.slice(0, i + 1)), rest: v.slice(i + 1).trim() };
    }
  }
  if (/^url\(/i.test(v)) {
    let quoted = "";
    for (let i = 4; i < v.length; i++) {
      if (v[i] === "\\") { i++; continue; }
      if (quoted) { if (v[i] === quoted) quoted = ""; continue; }
      if (v[i] === '"' || v[i] === "'") { quoted = v[i]; continue; }
      if (v[i] === ")") return { url: unquote(v.slice(4, i)), rest: v.slice(i + 1).trim() };
    }
  }
  return null;
}
export async function collectCssFonts(options: CssFontOptions): Promise<string> {
  const faces: CssFontFace[] = [], visited = new Set<string>(), faceKeys = new Set<string>(); let sheetCount = 0;
  const deadline = performance.now() + 20000;
  const diagnostic = (where: string, reason: unknown): void => console.warn(`Doroti CSS fonts (${where}): ${String(reason)}`);
  const allowed = (url: string): boolean => {
    const parsed = new URL(url, document.baseURI);
    return ["http:", "https:"].includes(parsed.protocol) && (!options.sameOriginOnly || parsed.origin === location.origin);
  };
  async function visitUrl(url: string, depth: number): Promise<void> {
    if (performance.now() >= deadline || faces.length >= 128) { diagnostic(url, "startup discovery budget exceeded"); return; }
    if (!allowed(url) || visited.has(url)) return;
    if (depth > 8 || ++sheetCount > 64) { diagnostic(url, "stylesheet traversal limit"); return; }
    visited.add(url);
    try {
      const response = await fetch(url, { mode: "cors", redirect: options.sameOriginOnly ? "error" : "follow", signal: AbortSignal.timeout(Math.max(1, Math.ceil(Math.min(10000, deadline - performance.now())))) });
      if (!response.ok || !allowed(response.url)) throw new Error(`HTTP ${response.status} or disallowed redirect`);
      const css = await response.text();
      if (css.length > 2 * 1024 * 1024) throw new Error("stylesheet exceeds 2 MB");
      await visitText(css, response.url, depth);
    } catch (error) { diagnostic(url, error); }
  }
  async function visitRules(rules: CSSRuleList, base: string, depth: number): Promise<void> {
    for (const rule of Array.from(rules)) {
      if (faces.length >= 128) { diagnostic(base, "128 face preload limit; use explicit assets for larger catalogs"); return; }
      if (rule instanceof CSSFontFaceRule) {
        const s = rule.style, family = unquote(s.getPropertyValue("font-family"));
        const sources = splitCss(s.getPropertyValue("src"), ",").flatMap(candidate => {
          const source = sourceUrl(candidate); if (!source) return [];
          if (/tech\(/i.test(source.rest) && !/tech\(\s*variations\s*\)/i.test(source.rest)) { diagnostic(base, `unsupported source technology: ${source.rest}`); return []; }
          const format = /format\(\s*['"]?([^'"\s)]+)/i.exec(source.rest)?.[1];
          if (format && !["woff", "woff2", "woff2-variations", "truetype", "opentype", "truetype-variations", "opentype-variations"].includes(format.toLowerCase())) return [];
          const url = new URL(source.url, base).href; return allowed(url) ? [url] : [];
        });
        if (!family || !sources.length) { diagnostic(base, `no usable URL source for ${family}`); continue; }
        for (const property of Array.from(s)) {
          if (!["font-family", "src", "font-weight", "font-style", "font-stretch", "unicode-range", "font-variation-settings", "font-display"].includes(property)) diagnostic(base, `${family}: unsupported descriptor ${property}`);
        }
        const face = { family, sources, stylesheet: base, weight: s.getPropertyValue("font-weight") || "normal",
          style: s.getPropertyValue("font-style") || "normal", stretch: s.getPropertyValue("font-stretch") || "normal",
          unicodeRange: s.getPropertyValue("unicode-range"), variationSettings: s.getPropertyValue("font-variation-settings") };
        const key = JSON.stringify(face); if (!faceKeys.has(key)) { faces.push(face); faceKeys.add(key); }
      } else if (rule instanceof CSSImportRule) {
        if ((!rule.media.length || matchMedia(rule.media.mediaText).matches) && (!rule.supportsText || CSS.supports(rule.supportsText))) await visitUrl(new URL(rule.href, base).href, depth + 1);
      } else if (rule instanceof CSSMediaRule) {
        if (matchMedia(rule.conditionText).matches) await visitRules(rule.cssRules, base, depth);
      } else if (rule instanceof CSSSupportsRule) {
        if (CSS.supports(rule.conditionText)) await visitRules(rule.cssRules, base, depth);
      } else if (rule instanceof CSSLayerBlockRule) await visitRules(rule.cssRules, base, depth);
    }
  }
  async function visitText(css: string, base: string, depth: number): Promise<void> {
    // Constructed stylesheets discard imports. Extract only top-level imports
    // before handing the remaining complete rules to the browser parser.
    let remainder = "";
    for (const part of splitCss(css, ";")) {
      const clean = part.replace(/^\s*(?:\/\*[\s\S]*?\*\/\s*)*/, "").trim();
      if (/^@import\s/i.test(clean)) {
        const source = sourceUrl(clean.replace(/^@import\s+/i, ""));
        if (!source) { diagnostic(base, "invalid import"); continue; }
        // CSSOM handles import media/supports grammar without attaching a sheet
        // or triggering browser font loads.
        let media = source.rest;
        // Layer names do not alter face descriptors. Keep document import order.
        if (/^layer\b/.test(media)) {
          if (media.startsWith("layer(")) {
            const layer = sourceUrl("url" + media.slice(5));
            if (!layer) { diagnostic(base, "invalid import layer"); continue; }
            media = layer.rest;
          } else media = media.slice(5).trim();
        }
        if (media.startsWith("supports(")) {
          let depth = 1, end = 9;
          for (; end < media.length && depth; end++) {
            if (media[end] === "(") depth++;
            if (media[end] === ")") depth--;
          }
          const condition = media.slice(9, end - 1);
          if (depth || !(CSS.supports(condition) || CSS.supports(`(${condition})`))) continue;
          media = media.slice(end).trim();
        }
        if (!media || matchMedia(media).matches) await visitUrl(new URL(source.url, base).href, depth + 1);
      } else remainder += part + ";";
    }
    const sheet = new CSSStyleSheet(); sheet.replaceSync(remainder); await visitRules(sheet.cssRules, base, depth);
  }
  if (options.discover) {
    for (const sheet of Array.from(document.styleSheets).concat(Array.from(document.adoptedStyleSheets))) {
      if (sheet.disabled || (sheet.media.length && !matchMedia(sheet.media.mediaText).matches)) continue;
      const base = sheet.href || document.baseURI;
      if (sheet.href && !allowed(base)) continue;
      try { await visitRules(sheet.cssRules, base, 0); }
      catch { if (sheet.href) await visitUrl(sheet.href, 0); }
    }
  }
  for (const url of options.stylesheets) await visitUrl(new URL(url, document.baseURI).href, 0);
  return JSON.stringify(faces);
}
