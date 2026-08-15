import { dotnet } from "./_framework/dotnet.js";
import { configureManagedCallbacks } from "./_content/Doroti.Host.Web/doroti.web.js";

const runtime = await dotnet.create();
const exports = await runtime.getAssemblyExports("Doroti.Host.Web.dll");
const interop = exports.Doroti.Host.Web.BrowserInterop;
configureManagedCallbacks({
  dispatchAnimationFrame: interop.DispatchAnimationFrame,
  dispatchSnapshot: interop.DispatchSnapshot,
});
await runtime.run();
