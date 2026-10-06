// Browser timestamps cross a JSExport boundary into TimeSpan.FromMilliseconds.
// A single invalid/coalesced event timestamp must not abort the shared runtime.
// Leave valid timestamps untouched, including legacy epoch-based event clocks.
const maximumMilliseconds = 922_337_203_685_476;
const currentTimestamp = (): number => performance.now();

export function normalizeBrowserTimestamp(milliseconds: number, now: () => number = currentTimestamp): number {
  if (Number.isFinite(milliseconds) && milliseconds >= 0 && milliseconds <= maximumMilliseconds)
    return milliseconds;
  const replacement = now();
  return Number.isFinite(replacement) && replacement >= 0 && replacement <= maximumMilliseconds
    ? replacement : 0;
}
