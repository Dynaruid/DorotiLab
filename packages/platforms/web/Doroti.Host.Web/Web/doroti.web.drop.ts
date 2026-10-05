import { retainBrowserFiles, releaseBrowserFile } from "./doroti.web.files.js";
type Options = { formats: string[]; bounds?: number[] };
const owners = new Map<number, () => void>();
let dispatch: ((id: number, json: string) => void) | undefined;
export function configureDrop(callback: (id: number, json: string) => void): void { dispatch = callback; }
export function closeBrowserDrop(id: number): void { owners.get(id)?.(); owners.delete(id); }
export function openBrowserDrop(id: number, canvasId: string, optionsJson: string): void {
  closeBrowserDrop(id);
  const canvas = document.getElementById(canvasId);
  if (!canvas) throw new Error("Drop canvas is missing.");
  const options = JSON.parse(optionsJson) as Options;
  const receive = (event: DragEvent): void => {
    const transfer = event.dataTransfer;
    if (!transfer || !dispatch) return;
    const rect = canvas.getBoundingClientRect();
    const x = event.clientX - rect.left;
    const y = event.clientY - rect.top;
    const formats = Array.from(transfer.types).map(t => t === "Files" ? "application/x-doroti-files" : t);
    const copy = ["all", "uninitialized", "copy", "copyLink", "copyMove"].includes(transfer.effectAllowed);
    const b = options.bounds;
    const accepted = copy && formats.some(t => options.formats.includes(t)) && (!b || (x >= b[0]! && y >= b[1]! && x < b[2]! && y < b[3]!));
    const phase = event.type === "drop" ? "Drop" : event.type === "dragenter" ? "Enter" : event.type === "dragleave" ? "Leave" : "Over";
    if (accepted && phase !== "Leave") { event.preventDefault(); transfer.dropEffect = "copy"; }
    else transfer.dropEffect = "none";
    if (phase === "Drop" && !accepted) return;
    const files = phase === "Drop" && options.formats.includes("application/x-doroti-files") ? retainBrowserFiles(id, Array.from(transfer.files)) : [];
    const text = phase === "Drop" && options.formats.includes("text/plain") ? transfer.getData("text/plain") : null;
    const uris = phase === "Drop" && options.formats.includes("text/uri-list") ? transfer.getData("text/uri-list") : null;
    if ((text?.length ?? 0) + (uris?.length ?? 0) > 1048576) { files.forEach(f => releaseBrowserFile(id, f.token)); return; }
    try { dispatch(id, JSON.stringify({ phase, x, y, formats, copy, files, text, uris })); }
    catch (error) { files.forEach(f => releaseBrowserFile(id, f.token)); throw error; }
  };
  const types = ["dragenter", "dragover", "dragleave", "drop"];
  types.forEach(type => canvas.addEventListener(type, receive as EventListener));
  owners.set(id, () => types.forEach(type => canvas.removeEventListener(type, receive as EventListener)));
}
