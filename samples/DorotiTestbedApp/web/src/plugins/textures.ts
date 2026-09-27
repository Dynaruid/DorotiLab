// Only IDs/control text cross managed interop. The video/canvas frames stay in JS.
const moduleUrl = new URL("./_content/Doroti.Host.Web/doroti.web.textures.js", document.baseURI).href;
type Entry = { textureId: string; markFrameAvailable(): void; pushFrame(frame: VideoFrame | ImageBitmap): boolean; dispose(): Promise<void>; ownCleanup(action: () => void): void };
type Registry = { registerCanvas(canvas: HTMLCanvasElement): Promise<Entry>; registerVideo(video: HTMLVideoElement): Promise<Entry>; createFrameProducer(): Promise<Entry>; startCamera(): Promise<Entry> };
let entry: Entry | undefined;
let canvas: HTMLCanvasElement | undefined;
let video: HTMLVideoElement | undefined;
let timer = 0;
let frame = 0;
let busy = false;
let source = "canvas";
let generation = 0;
export function pattern(target: HTMLCanvasElement, tick: number): void {
  const ctx = target.getContext("2d")!; const w = target.width, h = target.height;
  ctx.clearRect(0, 0, w, h);
  for (const [color, x, y] of [["#ff0000", 0, 0], ["#00ff00", w / 2, 0], ["#0000ff", 0, h / 2], ["#ffff00", w / 2, h / 2]] as const) {
    ctx.fillStyle = color; ctx.fillRect(x, y, w / 2, h / 2);
  }
  ctx.clearRect(w * .4, h * .35, w * .2, h * .3);
  ctx.fillStyle = "rgba(255,0,255,.5)"; ctx.fillRect(w * .4, h * .35, w * .2, h * .3);
  ctx.fillStyle = "white"; ctx.fillRect(tick % Math.max(1, w - 12), h * .25, 12, 12);
}
async function update(): Promise<void> {
  if (!canvas || !entry || busy) return;
  const current = entry, epoch = generation; busy = true;
  try {
    pattern(canvas, frame++);
    if (source === "canvas") current.markFrameAvailable();
    else {
      const next = source === "frame" ? new VideoFrame(canvas, { timestamp: frame * 33333 })
        : await createImageBitmap(canvas, { premultiplyAlpha: "premultiply" });
      if (epoch !== generation || !current.pushFrame(next)) next.close();
    }
  } finally { busy = false; }
}
export async function stop(): Promise<void> {
  generation++; clearInterval(timer); timer = 0;
  const old = entry; entry = undefined; canvas = undefined;
  if (video) { video.pause(); video.removeAttribute("src"); video.load(); video = undefined; }
  await old?.dispose();
}
/** Small WebCodecs round trip; codec support is checked, never silently substituted. */
export async function webCodecsExample(): Promise<string> {
  await stop();
  const config = { codec: "vp8", width: 320, height: 180, bitrate: 200000, framerate: 30 };
  if (typeof VideoEncoder === "undefined" || typeof VideoDecoder === "undefined" ||
      !(await VideoEncoder.isConfigSupported(config)).supported || !(await VideoDecoder.isConfigSupported({ codec: "vp8" })).supported)
    throw new Error("This browser does not support the VP8 WebCodecs example.");
  const registry = (await import(moduleUrl) as { texturesForCanvas(): Registry }).texturesForCanvas();
  entry = await registry.createFrameProducer(); const target = entry;
  const canvas = document.createElement("canvas"); canvas.width = 320; canvas.height = 180; pattern(canvas, 30);
  let failure: Error | undefined;
  const decoder = new VideoDecoder({ output: frame => { if (!target.pushFrame(frame)) frame.close(); }, error: error => { failure = error; } });
  decoder.configure({ codec: "vp8" });
  const encoder = new VideoEncoder({ output: chunk => decoder.decode(chunk), error: error => { failure = error; } });
  try {
    encoder.configure(config);
    const frame = new VideoFrame(canvas, { timestamp: 0 });
    try { encoder.encode(frame, { keyFrame: true }); } finally { frame.close(); }
    await encoder.flush(); await decoder.flush();
    if (failure) throw failure;
    return target.textureId;
  } catch (error) { await stop(); throw error; }
  finally { encoder.close(); decoder.close(); }
}
export async function select(operation: string): Promise<string> {
  if (operation === "codec") return webCodecsExample();
  if (operation === "update") { await update(); return entry?.textureId ?? "0"; }
  if (operation === "resize") { if (canvas) { canvas.width = canvas.width === 320 ? 160 : 320; canvas.height = canvas.width * 9 / 16; await update(); } return entry?.textureId ?? "0"; }
  if (operation === "pause") { if (video) { if (video.paused) await video.play(); else video.pause(); } return entry?.textureId ?? "0"; }
  if (operation === "seek") { if (video && Number.isFinite(video.duration)) video.currentTime = video.duration * .6; return entry?.textureId ?? "0"; }
  if (operation === "end") { if (video && Number.isFinite(video.duration)) { video.currentTime = video.duration - .1; await video.play(); } return entry?.textureId ?? "0"; }
  if (operation === "replace-video") { if (video) { video.src = "./media/texture-pattern.mp4?replacement=1"; await video.play(); } return entry?.textureId ?? "0"; }
  await stop(); if (operation === "stop") return "0";
  const registry = (await import(moduleUrl) as { texturesForCanvas(): Registry }).texturesForCanvas();
  source = operation;
  if (operation === "camera") entry = await registry.startCamera();
  else if (operation === "video") {
    video = document.createElement("video"); video.src = "./media/texture-pattern.mp4"; video.muted = true; video.playsInline = true;
    await video.play(); entry = await registry.registerVideo(video);
    const ownedVideo = video;
    entry.ownCleanup(() => { ownedVideo.pause(); ownedVideo.removeAttribute("src"); ownedVideo.load(); });
  } else {
    canvas = document.createElement("canvas"); canvas.width = 320; canvas.height = 180;
    entry = operation === "canvas" ? await registry.registerCanvas(canvas) : await registry.createFrameProducer();
    await update(); timer = window.setInterval(() => { void update(); }, 33);
    entry.ownCleanup(() => { clearInterval(timer); timer = 0; });
  }
  return entry.textureId;
}
export function diagnostics() { return { source, paused: video?.paused, ended: video?.ended, time: video?.currentTime, duration: video?.duration }; }
export async function invoke(message: { payloadBase64: string }): Promise<string> { return btoa(await select(atob(message.payloadBase64))); }
