import type {
  CanvasKit,
  FontCollection,
  Paragraph,
  ParagraphStyle,
  TextStyle,
  TypefaceFontProvider,
} from "canvaskit-wasm";

interface ParagraphLayoutRequest {
  readonly text: string;
  readonly width: number;
  readonly unconstrained?: boolean;
  readonly fontFamily?: string | null;
  readonly fontSize?: number | null;
  readonly maxLines?: number | null;
  readonly color?: number | readonly number[] | null;
  readonly height?: number | null;
  readonly locale?: string | null;
  readonly direction?: string | null;
  readonly align?: string | null;
  readonly ellipsis?: string | null;
  readonly paragraphStyle?: ParagraphStyle;
  readonly textStyle?: TextStyle;
}

interface RegisteredFont {
  readonly family: string;
  readonly bytes: Uint8Array;
}

interface ParagraphGraphemeSnapshot {
  readonly start: number;
  readonly end: number;
  readonly left: number;
  readonly top: number;
  readonly right: number;
  readonly bottom: number;
  readonly direction: "ltr" | "rtl";
}

export class CanvasKitTextLayoutService {
  readonly #canvasKit: CanvasKit;
  readonly #fonts = new Map<number, RegisteredFont>();
  #provider: TypefaceFontProvider;
  #collection: FontCollection;
  #ready = false;

  constructor(canvasKit: CanvasKit) {
    this.#canvasKit = canvasKit;
    this.#provider = canvasKit.TypefaceFontProvider.Make();
    this.#collection = canvasKit.FontCollection.Make();
    this.#collection.setDefaultFontManager(this.#provider);
  }

  smoke(): Readonly<Record<string, unknown>> {
    const started = performance.now();
    const builder = this.#canvasKit.ParagraphBuilder.MakeFromFontCollection(
      new this.#canvasKit.ParagraphStyle({ textStyle: {} }), this.#collection);
    let paragraph;
    try {
      builder.addText("");
      paragraph = builder.build();
      paragraph.layout(1);
      const clusterFixtures = clusterSmokeFixtures();
      this.#ready = true;
      return {
        domAccess: 0,
        webGlContextAccess: 0,
        height: paragraph.getHeight(),
        unresolvedCodepoints: paragraph.unresolvedCodepoints(),
        clusterFixtures,
        elapsedMicroseconds: Math.round((performance.now() - started) * 1000),
      };
    } finally {
      paragraph?.delete();
      builder.delete();
    }
  }

  registerFont(resourceId: number, descriptorJson: string, bytes: Uint8Array): void {
    const descriptor = JSON.parse(descriptorJson || "{}") as Record<string, unknown>;
    const family = String(descriptor.family ?? descriptor.fontFamily ?? "").trim();
    if (!family) throw new Error("Doroti CanvasKit font descriptor requires 'family'.");
    const owned = bytes.slice();
    const probe = this.#canvasKit.Typeface.MakeFreeTypeFaceFromData(
      owned.buffer.slice(owned.byteOffset, owned.byteOffset + owned.byteLength));
    if (!probe) throw new Error(`Doroti CanvasKit could not decode font resource ${resourceId}.`);
    probe.delete();
    this.#fonts.set(resourceId, { family, bytes: owned });
    this.#rebuildProvider();
  }

  releaseFont(resourceId: number): void {
    if (!this.#fonts.delete(resourceId)) return;
    this.#rebuildProvider();
  }

  layout(requestJson: string): string {
    if (!this.#ready) throw new Error("Doroti CanvasKit UI text service is not ready.");
    const request = JSON.parse(requestJson) as Partial<ParagraphLayoutRequest>;
    const text = String(request.text ?? "");
    const rawRequestedWidth = Number(request.width);
    const requestedWidth = Math.fround(rawRequestedWidth);
    if (!Number.isFinite(rawRequestedWidth) || rawRequestedWidth < 0 ||
        !Number.isFinite(requestedWidth))
      throw new Error("Doroti CanvasKit paragraph width must be finite and nonnegative.");
    if (text.length > 0 && this.#fonts.size === 0)
      throw new Error("Doroti CanvasKit paragraph layout requires a registered font.");
    const paragraphStyle = normalizeParagraphStyle(this.#canvasKit, request);
    const builder = this.#canvasKit.ParagraphBuilder.MakeFromFontCollection(
      new this.#canvasKit.ParagraphStyle(paragraphStyle), this.#collection);
    let paragraph;
    try {
      builder.addText(text);
      paragraph = builder.build();
      paragraph.layout(requestedWidth);
      let layoutWidth = requestedWidth;
      if (request.unconstrained) {
        const naturalWidth = paragraph.getMaxIntrinsicWidth();
        if (!Number.isFinite(naturalWidth) || naturalWidth < 0)
          throw new Error("Doroti CanvasKit returned an invalid intrinsic paragraph width.");
        layoutWidth = Math.fround(naturalWidth);
        if (!Number.isFinite(layoutWidth) || layoutWidth < 0)
          throw new Error("Doroti CanvasKit intrinsic paragraph width exceeds f32 range.");
        paragraph.layout(layoutWidth);
      }
      const lines = paragraph.getLineMetrics().map((line) => ({
        start: line.startIndex,
        end: line.endIndex,
        hardBreak: line.isHardBreak,
        ascent: line.ascent,
        descent: line.descent,
        height: line.height,
        width: line.width,
        left: line.left,
        baseline: line.baseline,
      }));
      const graphemes = paragraphGraphemeSnapshots(this.#canvasKit, paragraph, text);
      const codeUnitAdvances = new Array<number>(text.length).fill(0);
      for (const grapheme of graphemes)
        codeUnitAdvances[grapheme.start] = grapheme.right - grapheme.left;
      const unresolvedCodepoints = paragraph.unresolvedCodepoints();
      const result = {
        width: layoutWidth,
        height: paragraph.getHeight(),
        alphabeticBaseline: paragraph.getAlphabeticBaseline(),
        ideographicBaseline: paragraph.getIdeographicBaseline(),
        minIntrinsicWidth: paragraph.getMinIntrinsicWidth(),
        maxIntrinsicWidth: paragraph.getMaxIntrinsicWidth(),
        // CanvasKit/SkParagraph reports -FLT_MAX for an empty paragraph.
        // Doroti's public paragraph contract uses the canonical empty width 0.
        longestLine: Math.max(0, paragraph.getLongestLine()),
        didExceedMaxLines: paragraph.didExceedMaxLines(),
        numberOfLines: lines.length,
        metricsHash: "",
        codeUnitAdvances,
        graphemes,
        lines,
        unresolvedCodepoints,
      };
      result.metricsHash = fnv1a64(JSON.stringify(result)).toString(10);
      return JSON.stringify(result);
    } finally {
      paragraph?.delete();
      builder.delete();
    }
  }

  diagnostics(): Readonly<Record<string, number | boolean>> {
    return { ready: this.#ready, fontCount: this.#fonts.size };
  }

  dispose(): void {
    this.#ready = false;
    this.#fonts.clear();
    this.#collection.delete();
    this.#provider.delete();
  }

  #rebuildProvider(): void {
    const nextProvider = this.#canvasKit.TypefaceFontProvider.Make();
    try {
      for (const font of this.#fonts.values()) nextProvider.registerFont(font.bytes, font.family);
      const nextCollection = this.#canvasKit.FontCollection.Make();
      nextCollection.setDefaultFontManager(nextProvider);
      this.#collection.delete();
      this.#provider.delete();
      this.#collection = nextCollection;
      this.#provider = nextProvider;
    } catch (error) {
      nextProvider.delete();
      throw error;
    }
  }
}

function paragraphGraphemeSnapshots(
  canvasKit: CanvasKit,
  paragraph: Paragraph,
  text: string,
): ParagraphGraphemeSnapshot[] {
  const result: ParagraphGraphemeSnapshot[] = [];
  const seen = new Set<string>();
  for (let index = 0; index < text.length; index++) {
    const info = paragraph.getGlyphInfoAt(index);
    if (!info) continue;
    const start = Number(info.graphemeClusterTextRange.start);
    const end = Number(info.graphemeClusterTextRange.end);
    if (!Number.isSafeInteger(start) || !Number.isSafeInteger(end) ||
        start < 0 || end <= start || end > text.length)
      throw new Error(`Doroti CanvasKit returned invalid grapheme range [${start}, ${end}).`);
    const key = `${start}:${end}`;
    if (seen.has(key)) continue;
    seen.add(key);
    const [left, top, right, bottom] = [...info.graphemeLayoutBounds].map(Number);
    if (![left, top, right, bottom].every(Number.isFinite) || right < left || bottom < top)
      throw new Error(`Doroti CanvasKit returned invalid grapheme bounds for [${start}, ${end}).`);
    const direction = info.dir === canvasKit.TextDirection.LTR
      ? "ltr"
      : info.dir === canvasKit.TextDirection.RTL
        ? "rtl"
        : null;
    if (!direction)
      throw new Error(`Doroti CanvasKit returned invalid grapheme direction for [${start}, ${end}).`);
    result.push({ start, end, left, top, right, bottom, direction });
  }
  result.sort((left, right) => left.start - right.start || left.end - right.end);
  for (let index = 1; index < result.length; index++) {
    if (result[index - 1].end > result[index].start)
      throw new Error("Doroti CanvasKit returned overlapping grapheme ranges.");
  }
  return result;
}

function normalizeTextStyle(canvasKit: CanvasKit, value: TextStyle): TextStyle {
  const style = { ...value } as TextStyle;
  const rawColor = (value as TextStyle & { color?: unknown }).color;
  if (Array.isArray(rawColor) && rawColor.length === 4)
    style.color = canvasKit.Color4f(...rawColor.map(Number) as [number, number, number, number]);
  return style;
}

function normalizeParagraphStyle(
  canvasKit: CanvasKit,
  request: Partial<ParagraphLayoutRequest>,
): ParagraphStyle {
  const paragraphStyle = { ...(request.paragraphStyle ?? {}) } as ParagraphStyle;
  const textStyle = normalizeTextStyle(
    canvasKit, request.textStyle ?? paragraphStyle.textStyle ?? {});
  const family = String(request.fontFamily ?? "").trim();
  if (family) textStyle.fontFamilies = [family];
  if (request.fontSize !== undefined && request.fontSize !== null) {
    const fontSize = Number(request.fontSize);
    if (!Number.isFinite(fontSize) || fontSize <= 0)
      throw new Error("Doroti CanvasKit paragraph fontSize must be finite and positive.");
    textStyle.fontSize = fontSize;
  }
  if (request.color !== undefined && request.color !== null)
    textStyle.color = normalizeColor(canvasKit, request.color);
  if (request.height !== undefined && request.height !== null) {
    const height = Number(request.height);
    if (!Number.isFinite(height) || height <= 0)
      throw new Error("Doroti CanvasKit paragraph height must be finite and positive.");
    const fontSize = Number(textStyle.fontSize);
    if (!Number.isFinite(fontSize) || fontSize <= 0)
      throw new Error(
        "Doroti CanvasKit paragraph height requires a finite positive fontSize.");
    const heightMultiplier = height / fontSize;
    if (!Number.isFinite(heightMultiplier) || heightMultiplier <= 0)
      throw new Error("Doroti CanvasKit paragraph height multiplier must be positive.");
    textStyle.heightMultiplier = heightMultiplier;
  } else if (request.fontSize !== undefined && request.fontSize !== null) {
    // The managed flat v1 ABI freezes the absent-height value as an explicit
    // multiplier so UI measurement and Raster replay construct identical styles.
    textStyle.heightMultiplier = 1;
  }
  const locale = String(request.locale ?? "").trim();
  if (locale) textStyle.locale = locale;
  if (request.maxLines !== undefined && request.maxLines !== null) {
    const maxLines = Number(request.maxLines);
    if (!Number.isSafeInteger(maxLines) || maxLines <= 0 || maxLines > 0xffff_ffff)
      throw new Error("Doroti CanvasKit paragraph maxLines must be a positive uint32.");
    paragraphStyle.maxLines = maxLines;
  }
  if (request.direction !== undefined && request.direction !== null) {
    const direction = String(request.direction).toLowerCase();
    if (direction !== "ltr" && direction !== "rtl")
      throw new Error(`Doroti CanvasKit paragraph direction '${direction}' is unsupported.`);
    paragraphStyle.textDirection = direction === "rtl"
      ? canvasKit.TextDirection.RTL
      : canvasKit.TextDirection.LTR;
  }
  if (request.align !== undefined && request.align !== null) {
    const align = String(request.align).toLowerCase();
    switch (align) {
      case "start": paragraphStyle.textAlign = canvasKit.TextAlign.Start; break;
      case "end": paragraphStyle.textAlign = canvasKit.TextAlign.End; break;
      case "left": paragraphStyle.textAlign = canvasKit.TextAlign.Left; break;
      case "right": paragraphStyle.textAlign = canvasKit.TextAlign.Right; break;
      case "center": paragraphStyle.textAlign = canvasKit.TextAlign.Center; break;
      case "justify": paragraphStyle.textAlign = canvasKit.TextAlign.Justify; break;
      default:
        throw new Error(`Doroti CanvasKit paragraph alignment '${align}' is unsupported.`);
    }
  }
  if (request.ellipsis !== undefined && request.ellipsis !== null)
    paragraphStyle.ellipsis = String(request.ellipsis);
  paragraphStyle.textStyle = textStyle;
  return paragraphStyle;
}

function normalizeColor(canvasKit: CanvasKit, value: number | readonly number[]) {
  if (typeof value === "number") {
    if (!Number.isSafeInteger(value) || value < 0 || value > 0xffff_ffff)
      throw new Error("Doroti CanvasKit paragraph color must be an ARGB uint32.");
    return canvasKit.Color(
      (value >>> 16) & 0xff,
      (value >>> 8) & 0xff,
      value & 0xff,
      ((value >>> 24) & 0xff) / 255);
  }
  if (value.length !== 4 || value.some((component) => !Number.isFinite(Number(component))))
    throw new Error("Doroti CanvasKit paragraph color array must contain four finite components.");
  return canvasKit.Color4f(...value.map(Number) as [number, number, number, number]);
}

function graphemeRanges(text: string): readonly { start: number; end: number }[] {
  const Segmenter = (Intl as typeof Intl & {
    Segmenter?: new (
      locale?: string | string[],
      options?: { granularity: "grapheme" },
    ) => { segment(value: string): Iterable<{ index: number; segment: string }> };
  }).Segmenter;
  if (Segmenter) {
    return [...new Segmenter(undefined, { granularity: "grapheme" }).segment(text)]
      .map((entry) => ({ start: entry.index, end: entry.index + entry.segment.length }));
  }
  const ranges: { start: number; end: number }[] = [];
  for (let start = 0; start < text.length;) {
    const codePoint = text.codePointAt(start)!;
    let end = start + (codePoint > 0xffff ? 2 : 1);
    while (end < text.length) {
      const next = text.codePointAt(end)!;
      const nextText = String.fromCodePoint(next);
      if (!/^\p{Mark}$/u.test(nextText) && next !== 0xfe0f && next !== 0x200d) break;
      end += next > 0xffff ? 2 : 1;
      if (next === 0x200d && end < text.length) {
        const joined = text.codePointAt(end)!;
        end += joined > 0xffff ? 2 : 1;
      }
    }
    ranges.push({ start, end });
    start = end;
  }
  return ranges;
}

function clusterSmokeFixtures(): Readonly<Record<string, readonly number[]>> {
  const fixtures = {
    surrogate: graphemeRanges("😀").map((range) => range.end - range.start),
    combining: graphemeRanges("e\u0301").map((range) => range.end - range.start),
    hangul: graphemeRanges("한글").map((range) => range.end - range.start),
    rtl: graphemeRanges("אב").map((range) => range.end - range.start),
  };
  if (fixtures.surrogate.join() !== "2" || fixtures.combining.join() !== "2" ||
      fixtures.hangul.join() !== "1,1" || fixtures.rtl.join() !== "1,1")
    throw new Error("Doroti CanvasKit UTF-16 grapheme smoke fixture failed.");
  return fixtures;
}

function fnv1a64(value: string): bigint {
  const bytes = new TextEncoder().encode(value);
  let hash = 0xcbf29ce484222325n;
  for (const byte of bytes) {
    hash ^= BigInt(byte);
    hash = BigInt.asUintN(64, hash * 0x100000001b3n);
  }
  return hash;
}
