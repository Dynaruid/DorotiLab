// Analyze saved full-monitor PNGs; never use PNG size as the browser client size.
import { readFileSync, writeFileSync } from 'node:fs';
import { dirname, resolve } from 'node:path';
import { PNG } from 'pngjs';
import { decodeResizePixelMarker } from './tests/helpers/resize-pixel-marker.ts';

const [input, output] = process.argv.slice(2);
const report = JSON.parse(readFileSync(input, 'utf8'));
const native = report.native ?? report;
const fixture = report.fixture ?? report.manifest?.fixture ?? 'F3';
const frequency = native.clockCalibration.qpcFrequency;
const start = native.dragTiming.dragStartCounter;
const end = native.windowSamples.at(-1).performanceCounter;
const quantiles = values => {
  const sorted = values.filter(Number.isFinite).sort((a, b) => a - b);
  return { count: sorted.length, p95: sorted.length ? sorted[Math.ceil(sorted.length * .95) - 1] : null,
    max: sorted.length ? sorted.at(-1) : null };
};
// Active frames plus boundary references suffice; post-motion duplicates do
// not add independent geometry samples and need not be decoded again.
const lastPreMotion = native.frames.filter(frame => frame.callbackEntryCounter < start).at(-1);
const frames = native.frames.filter((frame, index) => index === 0 || frame === lastPreMotion || index === native.frames.length - 1 ||
  (frame.callbackEntryCounter >= start && frame.callbackEntryCounter <= end)).map(frame => {
  const bytes = readFileSync(resolve(dirname(input), frame.png));
  const marker = decodeResizePixelMarker(bytes, fixture === 'F2');
  let textRows = null;
  if (marker && fixture === 'F2') {
    const png = PNG.sync.read(bytes);
    textRows = [];
    // Fixture text alone uses this exact RGB; antialiasing edge pixels need
    // not match. Missing/covered text remains incomplete evidence.
    for (let y = Math.max(0, marker.rootY); y < Math.min(png.height, marker.rootY + marker.physicalHeight); y++) {
      let left = Infinity, right = -1;
      for (let x = Math.max(0, marker.rootX); x < Math.min(png.width, marker.rootX + marker.physicalWidth); x++) {
        const i = (y * png.width + x) * 4;
        if (png.data[i] === 23 && png.data[i + 1] === 63 && png.data[i + 2] === 95) {
          left = Math.min(left, x); right = Math.max(right, x);
        }
      }
      if (right < left) continue;
      const last = textRows.at(-1);
      if (last && y - marker.rootY <= last.bottom + 1) {
        last.bottom = y - marker.rootY;
        last.left = Math.min(last.left, left - marker.rootX);
        last.right = Math.max(last.right, right - marker.rootX);
      } else textRows.push({ top: y - marker.rootY, bottom: y - marker.rootY,
        left: left - marker.rootX, right: right - marker.rootX });
    }
  }
  return { png: frame.png, counter: frame.callbackEntryCounter, window: frame.window, marker, textRows };
});
// The first setup capture may still contain a prior resize epoch. Calibrate
// chrome against the last pre-motion pixels, retaining the first for auditing.
const initial = frames.filter(f => f.counter < start && f.marker).at(-1);
if (!initial) throw new Error('No pre-motion pixel calibration');
const chromeWidth = initial.window.right - initial.window.left - initial.marker.physicalWidth;
const chromeHeight = initial.window.bottom - initial.window.top - initial.marker.physicalHeight;
const active = frames.filter(f => f.counter >= start && f.counter <= end);
const seen = new Set(frames.filter(f => f.counter < start && f.marker).map(f => f.marker.generation));
const changed = active.filter(f => {
  if (!f.marker || f.marker.generation <= initial.marker.generation || seen.has(f.marker.generation)) return false;
  seen.add(f.marker.generation); return true;
});
const times = [start, ...changed.map(f => f.counter), end];
const rows = active.map(f => {
  if (!f.marker) return { ...f, status: 'undecoded' };
  const m = f.marker;
  const width = f.window.right - f.window.left - chromeWidth;
  const height = f.window.bottom - f.window.top - chromeHeight;
  const originX = m.rootX - initial.marker.rootX - (f.window.left - initial.window.left);
  const originY = m.rootY - initial.marker.rootY - (f.window.top - initial.window.top);
  return { ...f, widthError: Math.abs(m.physicalWidth - width), heightError: Math.abs(m.physicalHeight - height),
    originX, originY,
    inferredRasterCenterXError: Math.abs(originX + (m.physicalWidth - width) / 2),
    inferredRasterCenterYError: Math.abs(originY + (m.physicalHeight - height) / 2),
    // Independent blue center landmark, available only in F2. This residual
    // separates an internally displaced center from a correctly centered old frame.
    centerWithinRasterX: m.center ? m.center.x - m.rootX - m.physicalWidth / 2 : null,
    centerWithinRasterY: m.center ? m.center.y - m.rootY - m.physicalHeight / 2 : null };
});
const summary = Object.fromEntries(['widthError', 'heightError', 'originX', 'originY',
  'inferredRasterCenterXError', 'inferredRasterCenterYError', 'centerWithinRasterX', 'centerWithinRasterY']
  .map(key => [key, quantiles(rows.map(r => r[key] === null || r[key] === undefined ? null : Math.abs(r[key])))]));
summary.boundaryGapMilliseconds = quantiles(times.slice(1).map((t, i) => (t - times[i]) / frequency * 1000));
const result = { schema: 'doroti.capture-sidecar/v1', input, fixture, refreshHz: native.displayRefreshHz,
  captureIntegrity: ['captureRingDroppedFrames', 'encoderDroppedFrames', 'poolCapacityExceededFrames', 'captureErrors']
    .every(key => native[key] === 0) && native.encoderError === null ? 'PASS' : 'FAIL',
  activeFrames: active.length, decodedFrames: active.filter(f => f.marker).length, newFrameCount: changed.length,
  summary, rows, initial, firstSetupCapture: frames[0], final: frames.at(-1),
  limitations: ['WGC callback rect is sampled, not exact scan-out geometry',
    'Raster center is inferred from marker geometry; F3 has no independent center marker',
    'F2 text row bounds are measured pixels, not a dynamic wrap equivalence gate',
    'Browser popups and occlusion require visual review; missing landmarks never imply PASS'] };
writeFileSync(output, JSON.stringify(result, null, 2));
console.log(JSON.stringify({ fixture, activeFrames: result.activeFrames, decodedFrames: result.decodedFrames, summary }));
