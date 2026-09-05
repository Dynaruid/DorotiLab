import { test, expect } from "@playwright/test";
import { measureResizeFollowing, measureNativeObserver, type ResizeObservation } from "./helpers/resize-following.js";

const sample = (time: number, generation: number, width = 100, dpr = 1): ResizeObservation =>
  ({ time, generation, width, height: 100, dpr });

test("native matching reports reverse ambiguity instead of inventing latency", () => {
  const result = measureNativeObserver([
    { time: 0, width: 110, height: 120 }, { time: 10, width: 210, height: 120 },
    { time: 20, width: 110, height: 120 },
  ], [sample(-1, 1), sample(12, 2, 200), sample(22, 3)], 0, 1);
  expect(result.matched).toBe(1);
  expect(result.ambiguous).toBe(1);
  expect(result.latency?.p95).toBe(2);
});

test("resize metrics retain missing targets and zero/one-front boundary gaps", () => {
  const targets = [sample(0, 1), sample(50, 2)];
  const zero = measureResizeFollowing(targets, [], 0, 150);
  expect(zero.unreachedTargetCount).toBe(2);
  expect(zero.boundaryInclusiveGaps.max).toBe(150);
  expect(zero.intervalStatus).toBe("FAIL");
  expect(zero.geometry.uncoveredMilliseconds).toBe(150);
  const one = measureResizeFollowing(targets, [sample(110, 2)], 0, 150);
  expect(one.firstFrontMilliseconds).toBe(110);
  expect(one.boundaryInclusiveGaps.max).toBe(110);
  expect(one.over100msGapCount).toBe(1);
  expect(one.intervalStatus).toBe("insufficientSamples");
  expect(one.supersededTargetCount).toBe(1);
});

test("reverse geometry uses completed frame and repeated submission preserves content age", () => {
  const result = measureResizeFollowing([sample(-10, 1), sample(0, 2, 200), sample(50, 3)],
    [sample(-5, 1), sample(25, 2, 200), sample(75, 2, 200), sample(100, 3)], 0, 150);
  expect(result.geometry.width.integral).toBe(7500);
  expect(result.geometry.width.p95).toBe(100);
  expect(result.geometry.uncoveredMilliseconds).toBe(0);
  expect(result.activeFrontCount).toBe(2);
  expect(result.contentAge.maxMilliseconds).toBe(100);
  expect(result.contentAge.p95Milliseconds).toBeCloseTo(96.25, 5);
  expect(result.settleFromObserverMilliseconds).toBe(50);
  expect(result.settleFromNativeEndMilliseconds).toBe(-50);
});

test("settle requires exact epoch, size and DPR and counts late observer delivery", () => {
  const result = measureResizeFollowing([sample(-1, 1), sample(160, 2, 200, 2)],
    [sample(-1, 1), sample(170, 2, 200, 1), sample(180, 3, 200, 2)], 0, 150);
  expect(result.targetCount).toBe(1);
  expect(result.activeTargetCount).toBe(0);
  expect(result.perTarget[0].caughtUpMilliseconds).toBe(10);
  expect(result.perTarget[0].exactEpochMilliseconds).toBeNull();
  expect(result.settleFromObserverMilliseconds).toBeNull();
  expect(result.activeFrontCount).toBe(0);
});

test("pre-motion idle age stays separate and old resubmission does not reset active age", () => {
  const result = measureResizeFollowing([sample(-500, 1), sample(10, 2, 200)],
    [sample(-250, 1), sample(50, 1)], 0, 100);
  expect(result.idleBaselineAgeMilliseconds).toBe(500);
  expect(result.contentAge.maxMilliseconds).toBe(600);
  expect(result.activeMotionContentAge.meanMilliseconds).toBe(50);
  expect(result.activeMotionContentAge.p95Milliseconds).toBeCloseTo(95, 5);
  expect(result.activeMotionContentAge.maxMilliseconds).toBe(100);
  expect(result.unreachedTargetCount).toBe(1);
});
