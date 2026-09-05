import { PNG } from "pngjs";
import { test, expect } from "@playwright/test";
import { decodeResizePixelMarker } from "./helpers/resize-pixel-marker.js";

test("captured marker decodes frame pixels and rejects a corrupt payload", () => {
  const png = new PNG({ width: 400, height: 100 });
  const bits = [123456, 1920, 1080].flatMap((value, index) =>
    Array.from({ length: index ? 16 : 32 }, (_, bit) => (value >>> bit) & 1));
  const colors = [[0, 255, 255], [255, 0, 255], [0, 255, 255], [255, 0, 255],
    ...bits.map(bit => bit ? [0, 255, 0] : [0, 0, 0])];
  colors.forEach((rgb, cell) => {
    for (let y = 32; y < 36; y++) for (let x = 32 + cell * 4; x < 36 + cell * 4; x++) {
      const offset = (y * png.width + x) * 4;
      png.data.set([...rgb, 255], offset);
    }
  });
  expect(decodeResizePixelMarker(PNG.sync.write(png))).toMatchObject({
    generation: 123456, physicalWidth: 1920, physicalHeight: 1080, rootX: 0, rootY: 0,
  });
  for (let y = 32; y < 36; y++) for (let x = 48; x < 52; x++)
    png.data.set([127, 127, 127, 255], (y * png.width + x) * 4);
  expect(decodeResizePixelMarker(PNG.sync.write(png))).toBeNull();
});
