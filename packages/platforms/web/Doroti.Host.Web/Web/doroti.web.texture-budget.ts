/** RGBA estimates use backing pixels (including DPR), never CSS dimensions. */
export function textureSourceBytes(width: number, height: number, maxDimension: number): number {
  const bytes = width * height * 4;
  if (!Number.isSafeInteger(maxDimension) || maxDimension <= 0 ||
      !Number.isSafeInteger(width) || !Number.isSafeInteger(height) || width <= 0 || height <= 0 ||
      width > maxDimension || height > maxDimension || !Number.isSafeInteger(bytes * 4))
    throw new Error(`Texture source dimensions must be positive integers within the device limit (${maxDimension}).`);
  return bytes;
}

/** Preserve the small-source floor and room for current, replacement and retiring frames.
 * This is an admission ceiling, not an allocation. Owners retain a high-water ceiling
 * so downsizing cannot strand outstanding allocations or pending input.
 */
export function textureViewBudget(sourceBytes: number): number {
  return Math.max(64 * 1024 * 1024, sourceBytes * 4);
}
