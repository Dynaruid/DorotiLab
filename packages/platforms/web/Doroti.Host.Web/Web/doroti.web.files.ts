/** View-owned read grants. File bytes never pass through an unbounded JSON payload. */
type Owner = { files: Map<string, File>; cancel?: () => void };
const owners = new Map<number, Owner>();
export function openFileOwner(id: number): boolean {
  if (typeof document === "undefined") return false;
  if (owners.has(id)) throw new Error("File owner already attached.");
  owners.set(id, { files: new Map() });
  return true;
}
export function retainBrowserFiles(id: number, files: File[]): { token: string; name: string; length: number }[] {
  const owner = owners.get(id);
  if (!owner) throw new Error("File owner is closed.");
  return files.map(file => {
    const token = crypto.randomUUID();
    owner.files.set(token, file);
    return { token, name: file.name, length: file.size };
  });
}
export function pickBrowserFiles(id: number, multiple: boolean, accept: string): Promise<string> {
  const owner = owners.get(id);
  if (!owner) return Promise.resolve(JSON.stringify({ status: "unsupported" }));
  if (owner.cancel) return Promise.resolve(JSON.stringify({ status: "failed", message: "A picker is already open." }));
  if (navigator.userActivation && !navigator.userActivation.isActive)
    return Promise.resolve(JSON.stringify({ status: "denied", message: "File selection requires a user gesture." }));
  return new Promise(resolve => {
    const input = document.createElement("input");
    input.type = "file"; input.multiple = multiple; input.accept = accept; input.hidden = true;
    let finished = false;
    const finish = (status: string, files: File[] = [], message?: string): void => {
      if (finished) return;
      finished = true;
      input.remove(); input.onchange = null; input.oncancel = null; owner.cancel = undefined;
      resolve(JSON.stringify({ status, message, files: retainBrowserFiles(id, files) }));
    };
    owner.cancel = () => finish("cancelled");
    input.onchange = () => {
      const files = Array.from(input.files ?? []);
      finish(files.length ? "selected" : "cancelled", files);
    };
    input.oncancel = () => finish("cancelled");
    document.body.append(input);
    try { input.click(); } catch (error) { finish("failed", [], String(error)); }
  });
}
export function cancelBrowserPicker(id: number): void { owners.get(id)?.cancel?.(); }
export async function readBrowserFile(id: number, token: string, offset: number, count: number): Promise<Uint8Array> {
  const owner = owners.get(id);
  const file = owner?.files.get(token);
  if (!file) throw new Error("File grant is unknown or revoked.");
  if (!Number.isSafeInteger(offset) || offset < 0 || !Number.isInteger(count) || count < 0 || count > 65536)
    throw new RangeError("Invalid bounded file read.");
  const bytes = new Uint8Array(await file.slice(offset, offset + count).arrayBuffer());
  if (owners.get(id) !== owner || owner?.files.get(token) !== file) throw new Error("File grant was revoked during read.");
  return bytes;
}
export function releaseBrowserFile(id: number, token: string): void { owners.get(id)?.files.delete(token); }
export async function readBrowserFileBase64(id: number, token: string, offset: number, count: number): Promise<string> {
  const bytes = await readBrowserFile(id, token, offset, count);
  // Promise<byte[]> is not supported by the managed JS marshaller. Bound the encoded reply to 64 KiB.
  let binary = "";
  for (const byte of bytes) binary += String.fromCharCode(byte);
  return btoa(binary);
}
export function closeFileOwner(id: number): void {
  const owner = owners.get(id);
  owner?.cancel?.(); owner?.files.clear(); owners.delete(id);
}
