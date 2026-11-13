import { dotnet } from "./_framework/dotnet.js";
const runtime = await dotnet.create();
console.info("Mono 11 runtime-only probe; not a Skia render qualification.");
document.querySelector("output").textContent = "runtime-ready";
const exports = await runtime.getAssemblyExports(runtime.getConfig().mainAssemblyName);
document.querySelector("output").textContent = "exports-ready";
const id = await exports.Probe.Start();
document.querySelector("output").textContent = `owner=${id}`;
