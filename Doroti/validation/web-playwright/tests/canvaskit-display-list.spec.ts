import { readFile } from "node:fs/promises";
import { test, expect } from "./helpers/fixtures.js";

interface DisplayListGolden {
  readonly byteLength: number;
  readonly sha256: string;
  readonly base64: string;
}

const goldenPath = new URL(
  "../../display-list-contract/golden/display-list-v2-full.json",
  import.meta.url);

test("CanvasKit DisplayList v2 validates the all-opcode managed golden", async ({ page }) => {
  const golden = JSON.parse(await readFile(goldenPath, "utf8")) as DisplayListGolden;
  expect(golden.byteLength).toBe(6330);
  expect(golden.sha256).toBe("66412CCB5E02519BBD8C11ECAB5E63CE914E2DB745F6D51110BBD03F89CCBE42");
  await page.goto("/", { waitUntil: "domcontentloaded" });
  const result = await page.evaluate(async (base64) => {
    interface CommandEnvelope { readonly opcode: number; readonly payloadOffset: number; readonly payloadLength: number }
    interface ValidatedDocument {
      readonly metadata: { readonly byteLength: number };
      readonly resources: readonly unknown[];
      readonly strings: readonly string[];
      readonly commands: readonly CommandEnvelope[];
    }
    const moduleUrl = "/_content/Doroti.Host.Web/doroti.web.protocol.js";
    const protocol = await import(moduleUrl) as {
      validateDorotiDisplayList(value: Uint8Array): ValidatedDocument;
      canvasKitSurfaceGeneration(sessionId: number, resizeGeneration: number): number;
    };
    const rasterModuleUrl = "/_content/Doroti.Host.Web/doroti.canvaskit.worker.js";
    const raster = await import(rasterModuleUrl) as {
      dorotiCanvasKitImplementedOpcodes: readonly number[];
      dorotiCanvasKitDisplayMatrix(wire: ArrayLike<number>): Float32Array;
    };
    const bytes = Uint8Array.from(atob(base64), (value) => value.charCodeAt(0));
    const document = protocol.validateDorotiDisplayList(bytes);
    const failures: string[] = [];
    const reject = (mutate: (copy: Uint8Array, view: DataView) => void): void => {
      const copy = bytes.slice();
      const view = new DataView(copy.buffer);
      mutate(copy, view);
      view.setUint32(12, view.getUint32(12, true) & ~1, true);
      view.setUint32(104, 0, true);
      try {
        protocol.validateDorotiDisplayList(copy);
        failures.push("accepted");
      } catch (error) {
        failures.push(String(error));
      }
    };

    reject((_copy, view) => view.setBigUint64(56, 0n, true));
    reject((_copy, view) => view.setUint32(64, 0x8000_0000, true));
    reject((copy, view) => {
      const stringOffset = 112 + view.getUint32(100, true);
      copy[stringOffset + 4] = 0xff;
    });
    const clipRect = document.commands.find((command) => command.opcode === 5)!;
    reject((_copy, view) => view.setUint16(clipRect.payloadOffset + 18, 1, true));
    const drawImage = document.commands.find((command) => command.opcode === 28)!;
    reject((_copy, view) => view.setBigUint64(drawImage.payloadOffset + 8, 0xffffn, true));
    const shaderMask = document.commands.find((command) => command.opcode === 52)!;
    reject((copy) => { copy[shaderMask.payloadOffset] = 0; });
    const retained = document.commands.find((command) => command.opcode === 53)!;
    reject((copy) => { copy[retained.payloadOffset + retained.payloadLength - 1] = 4; });

    return {
      byteLength: document.metadata.byteLength,
      resourceCount: document.resources.length,
      stringCount: document.strings.length,
      opcodes: document.commands.map((command) => command.opcode),
      implementedOpcodes: raster.dorotiCanvasKitImplementedOpcodes,
      surfaceIdentities: [
        protocol.canvasKitSurfaceGeneration(1, 7),
        protocol.canvasKitSurfaceGeneration(2, 7),
      ],
      displayMatrix: Array.from(raster.dorotiCanvasKitDisplayMatrix([
        1, 0, 0, 0.25,
        0, 1, 0, 0.5,
        0, 0, 1, 0,
        7, 9, 0, 1,
      ])),
      failures,
    };
  }, golden.base64);

  expect(result.byteLength).toBe(golden.byteLength);
  expect(result.resourceCount).toBe(5);
  expect(result.stringCount).toBe(9);
  expect(new Set(result.opcodes)).toEqual(new Set([
    1, 2, 3, 4, 5, 6, 7,
    16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31,
    48, 49, 50, 51, 52, 53,
  ]));
  expect(new Set(result.implementedOpcodes)).toEqual(new Set(result.opcodes));
  expect(result.surfaceIdentities).toEqual([0x1_0000_0000 + 7, 0x2_0000_0000 + 7]);
  expect(result.displayMatrix).toEqual([1, 0, 7, 0, 1, 9, 0.25, 0.5, 1]);
  expect(result.failures).toHaveLength(7);
  expect(result.failures).not.toContain("accepted");
});

test("CanvasKit transfer buffers and canvas leases have explicit terminals", async ({ page }) => {
  await page.goto("/", { waitUntil: "domcontentloaded" });
  const result = await page.evaluate(async () => {
    const moduleUrl = "/_content/Doroti.Host.Web/doroti.web.worker-host.js";
    const module = await import(moduleUrl);
    const pool = new module.TransferBufferPool(1);
    const returned = pool.copy(new Uint8Array([1, 2, 3]), 7);
    const received = structuredClone(returned.buffer, { transfer: [returned.buffer] });
    pool.release(returned.transferId, 7, received);
    const abandoned = pool.copy(new Uint8Array([4, 5]), 8);
    structuredClone(abandoned.buffer, { transfer: [abandoned.buffer] });
    pool.abandonSession(8);

    const leases = new module.CanvasLeaseLedger();
    const first = leases.create("surface-1", 7);
    leases.transferred(first);
    leases.retire(first);
    leases.assertClosed();
    return { buffers: pool.snapshot(), leases: leases.snapshot() };
  });
  expect(result.buffers).toMatchObject({
    borrowed: 2,
    returned: 1,
    abandoned: 1,
    outstanding: 0,
    outstandingBytes: 0,
  });
  expect(result.leases).toEqual([expect.objectContaining({
    state: "retired",
    terminalCount: 1,
  })]);
});
