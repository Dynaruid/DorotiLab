// Only Doroti's interop wrapper is shipped; the versioned decoder loads from CDN.
const decoders = new Map<string, Promise<{ default(bytes: Uint8Array): Promise<Uint8Array> }>>();
export function copyDecoded(bytes: Uint8Array): Uint8Array { return bytes; }
export async function decode(bytes: Uint8Array, decoderUrl: string): Promise<Uint8Array> {
  if (bytes.length < 48) throw new Error("Truncated WOFF2 font");
  const header = new DataView(bytes.buffer, bytes.byteOffset, bytes.byteLength);
  if (header.getUint32(0) !== 0x774f4632) return bytes;
  if (header.getUint32(8) !== bytes.length || bytes.length > 30 * 1024 * 1024) throw new Error("Invalid WOFF2 length");
  if (header.getUint32(16) > 30 * 1024 * 1024) throw new Error("WOFF2 font exceeds 30 MB");
  let decoder = decoders.get(decoderUrl);
  if (!decoder) {
    decoder = import(decoderUrl);
    decoders.set(decoderUrl, decoder);
  }
  try {
    const result = await (await decoder).default(bytes);
    if (result.length > 30 * 1024 * 1024) throw new Error("Decoded font exceeds 30 MB");
    if (result.length !== header.getUint32(16)) throw new Error("WOFF2 output length mismatch");
    return result;
  } catch (error) {
    decoders.delete(decoderUrl);
    throw error;
  }
}
