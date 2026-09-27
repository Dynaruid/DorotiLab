// This module is loaded on the managed render Worker, never on main.
const url = new URL("../_content/Doroti.Host.Web/doroti.web.texture-worker.js", import.meta.url).href;
const textures = await import(url) as { registerLocalCanvas(canvas: OffscreenCanvas): { textureId: string; markFrameAvailable(): void; dispose(): Promise<void> } };
let entry: ReturnType<typeof textures.registerLocalCanvas> | undefined;
export async function command(operation: string): Promise<string> {
  if (operation === "create") {
    if (entry) throw new Error("Local canvas example already exists.");
    const canvas = new OffscreenCanvas(320, 180); const ctx = canvas.getContext("2d")!;
    ctx.fillStyle = "red"; ctx.fillRect(0, 0, 160, 90);
    ctx.fillStyle = "lime"; ctx.fillRect(160, 0, 160, 90);
    ctx.fillStyle = "blue"; ctx.fillRect(0, 90, 160, 90);
    ctx.fillStyle = "yellow"; ctx.fillRect(160, 90, 160, 90);
    entry = textures.registerLocalCanvas(canvas);
  } else if (operation === "update") entry?.markFrameAvailable();
  else if (operation === "stop") { await entry?.dispose(); entry = undefined; }
  else throw new Error("Unknown owner-local canvas operation.");
  return entry?.textureId ?? "0";
}
