/** View-owned read grants. File bytes never pass through an unbounded JSON payload. */
type PickOptions = { multiple: boolean; accept: string };
type Selection = { status: string; files: File[]; message?: string };
type PendingPick = PickOptions & {
  identifier?: string; claimed: boolean; finished: boolean; cancelled: boolean;
  result: Promise<Selection>; cancel(): void;
};
type Owner = { files: Map<string, File>; activations: Map<string, PickOptions>; pending?: PendingPick };
const owners = new Map<number, Owner>();
export function openFileOwner(id: number): boolean {
  if (typeof document === "undefined") return false;
  if (owners.has(id)) throw new Error("File owner already attached.");
  owners.set(id, { files: new Map(), activations: new Map() });
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
export function registerFileActivation(id: number, identifier: string, multiple: boolean, accept: string): void {
  const owner = owners.get(id);
  if (!owner) throw new Error("File owner is closed.");
  if (owner.activations.has(identifier)) throw new Error("File activation already registered.");
  owner.activations.set(identifier, { multiple, accept });
}
export function unregisterFileActivation(id: number, identifier: string): void {
  const owner = owners.get(id);
  owner?.activations.delete(identifier);
  if (owner?.pending?.identifier === identifier) {
    owner.pending.cancel();
    owner.pending = undefined;
  }
}

/** Called only from trusted pointer/keyboard/accessibility activation, before Worker dispatch. */
export function reserveBrowserPicker(id: number, control: HTMLElement | null): boolean {
  const owner = owners.get(id);
  if (!owner || !control?.isConnected || control.getAttribute("role") !== "button" ||
      control.getAttribute("aria-disabled") === "true") return false;
  for (let element: HTMLElement | null = control; element; element = element.parentElement) {
    const identifier = element.dataset.dorotiSemanticsIdentifier;
    const options = identifier ? owner.activations.get(identifier) : undefined;
    if (!options) continue;
    // A completed but unclaimed gesture can be replaced by the next real tap.
    // There is at most one pending native picker and no grants until managed admission.
    if (owner.pending && (owner.pending.claimed || !owner.pending.finished)) return false;
    owner.pending = startPicker(options, identifier);
    return true;
  }
  return false;
}

export async function pickBrowserFiles(id: number, multiple: boolean, accept: string): Promise<string> {
  const owner = owners.get(id);
  if (!owner) return JSON.stringify({ status: "unsupported" });
  let pending = owner.pending;
  if (pending && (pending.claimed || pending.multiple !== multiple || pending.accept !== accept))
    return JSON.stringify({ status: "failed", message: "A different picker is already open." });
  if (!pending) {
    if (navigator.userActivation && !navigator.userActivation.isActive)
      return JSON.stringify({ status: "denied", message: "File selection requires a user gesture." });
    owner.pending = pending = startPicker({ multiple, accept });
  }
  pending.claimed = true;
  try {
    const result = await pending.result;
    if (owners.get(id) !== owner || pending.cancelled) return JSON.stringify({ status: "cancelled" });
    return JSON.stringify({ ...result, files: retainBrowserFiles(id, result.files) });
  } finally {
    if (owner.pending === pending) owner.pending = undefined;
  }
}

function startPicker(options: PickOptions, identifier?: string): PendingPick {
  const input = document.createElement("input");
  input.type = "file"; input.multiple = options.multiple; input.accept = options.accept; input.hidden = true;
  let resolve!: (result: Selection) => void;
  const pending: PendingPick = { ...options, identifier, claimed: false, finished: false, cancelled: false,
    result: new Promise(done => { resolve = done; }), cancel: () => { pending.cancelled = true; finish("cancelled"); } };
  const finish = (status: string, files: File[] = [], message?: string): void => {
    if (pending.finished) return;
    pending.finished = true;
    input.remove(); input.onchange = null; input.oncancel = null;
    resolve({ status, files, message });
  };
  input.onchange = () => {
    const files = Array.from(input.files ?? []);
    finish(files.length ? "selected" : "cancelled", files);
  };
  input.oncancel = () => finish("cancelled");
  document.body.append(input);
  try {
    // WebKit file inputs still require the synchronous gesture scope. In some
    // versions showPicker() exists but does not present a file-upload panel.
    input.click();
  } catch (error) {
    finish(error instanceof DOMException && error.name === "NotAllowedError" ? "denied" : "failed", [], String(error));
  }
  return pending;
}
export function cancelBrowserPicker(id: number): void {
  const owner = owners.get(id);
  owner?.pending?.cancel();
  if (owner?.pending && !owner.pending.claimed) owner.pending = undefined;
}
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
  owner?.pending?.cancel(); owner?.files.clear(); owner?.activations.clear(); owners.delete(id);
}
