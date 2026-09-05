import { PNG } from "pngjs";

// Raster marker: four 4px sentinel cells, then little-endian generation32,
// physical width16 and height16. It is painted into the GPU frame, never DOM.
export function decodeResizePixelMarker(bytes: Buffer, includeFixtureLandmarks = false) {
  const png = PNG.sync.read(bytes);
  const is = (x: number, y: number, r: number, g: number, b: number) => {
    const i = (y * png.width + x) * 4;
    return png.data[i] === r && png.data[i + 1] === g && png.data[i + 2] === b;
  };
  for (let y = 0; y < png.height; y++) for (let x = 0; x + 68 * 4 <= png.width; x++) {
    if (!is(x, y, 0, 255, 255) || !is(x + 4, y, 255, 0, 255) ||
        !is(x + 8, y, 0, 255, 255) || !is(x + 12, y, 255, 0, 255)) continue;
    const bits: number[] = [];
    for (let bit = 0; bit < 64; bit++) {
      const px = x + 16 + bit * 4;
      if (is(px, y, 0, 255, 0)) bits.push(1);
      else if (is(px, y, 0, 0, 0)) bits.push(0);
      else break;
    }
    if (bits.length !== 64) continue;
    const value = (offset: number, count: number) => bits.slice(offset, offset + count).reduce((n, bit, index) => n + bit * 2 ** index, 0);
    let center: { x: number; y: number } | null = null;
    if (includeFixtureLandmarks) {
      let left = png.width, right = -1, top = png.height, bottom = -1;
      for (let py = Math.max(0, y - 32); py < Math.min(png.height, y - 32 + value(48, 16)); py++)
        for (let px = Math.max(0, x - 32); px < Math.min(png.width, x - 32 + value(32, 16)); px++) {
          if (!is(px, py, 41, 98, 255)) continue;
          left = Math.min(left, px); right = Math.max(right, px);
          top = Math.min(top, py); bottom = Math.max(bottom, py);
        }
      if (right >= left) center = { x: (left + right + 1) / 2, y: (top + bottom + 1) / 2 };
    }
    return { generation: value(0, 32), physicalWidth: value(32, 16), physicalHeight: value(48, 16), center,
      rootX: x - 32, rootY: y - 32, captureWidth: png.width, captureHeight: png.height };
  }
  return null;
}
