// Recompute only sidecars. Historical reports and their verdicts stay intact.
import { globSync, readFileSync, writeFileSync } from 'node:fs';
import { basename, resolve } from 'node:path';
import { measureResizeFollowing } from './tests/helpers/resize-following.ts';

const [root, output] = process.argv.slice(2);
const rows = [...globSync('**/fast-resize-*.json', { cwd: root })]
  .filter(file => /^fast-resize-\d+\.json$/.test(basename(file))).map(file => {
  const report = JSON.parse(readFileSync(resolve(root, file), 'utf8'));
  const origin = report.stages.main.timeOrigin;
  const trace = report.diagnostics.trace;
  const targets = trace.filter(e => e.phase === 'target-observed').map(e => ({
    time: origin + e.timestampMicroseconds / 1000, generation: e.epoch.generation,
    width: e.epoch.logicalWidth, height: e.epoch.logicalHeight, dpr: e.epoch.devicePixelRatio,
  }));
  const fronts = trace.filter(e => e.phase === 'front-commit').map(e => {
    const frame = JSON.parse(e.detail);
    return { time: origin + e.timestampMicroseconds / 1000, generation: frame.generation,
      width: frame.logicalWidth, height: frame.logicalHeight, dpr: frame.devicePixelRatio };
  });
  if (fronts.some(f => !Object.values(f).every(Number.isFinite))) throw new Error(`Incomplete frame: ${file}`);
  const metrics = measureResizeFollowing(targets, fronts,
    report.stimulus.motionStartEpochMilliseconds, report.stimulus.motionEndEpochMilliseconds);
  return { file, condition: [report.stimulus.edge, report.stimulus.requestedMilliseconds, report.stimulus.motion],
    originalStatus: report.followingV2.status, originalContentAge: report.followingV2.contentAge,
    recomputedContentAge: metrics.contentAge, idleBaselineAgeMilliseconds: metrics.idleBaselineAgeMilliseconds,
    activeMotionContentAge: metrics.activeMotionContentAge,
    caughtUp: metrics.caughtUp, firstFrontMilliseconds: metrics.firstFrontMilliseconds,
    geometry: metrics.geometry, boundaryInclusiveGaps: metrics.boundaryInclusiveGaps };
});
writeFileSync(output, JSON.stringify({ schema: 'doroti.resize-age-sidecar/v1', root, rows,
  limitation: 'New active age is separate from historical absolute age; no new native trials or reclassification' }, null, 2));
console.log(JSON.stringify({ reports: rows.length, originalFailures: rows.filter(r => r.originalStatus === 'FAIL').length }));
