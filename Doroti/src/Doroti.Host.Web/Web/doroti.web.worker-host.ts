export function createDorotiWorker(url: URL): Worker {
  return new Worker(url, { type: "module" });
}

export function createDorotiClassicWorker(url: URL): Worker {
  return new Worker(url);
}

export function closeExternalLeases<T>(
  leases: Map<number, T>,
  close: (requestId: number, lease: T) => void,
): void {
  for (const [requestId, lease] of leases) close(requestId, lease);
  leases.clear();
}

export type CanvasLeaseState = "created" | "transferred" | "retired";

export interface CanvasLeaseSnapshot {
  readonly leaseId: number;
  readonly canvasId: string;
  readonly sessionId: number;
  readonly state: CanvasLeaseState;
  readonly terminalCount: number;
}

/** Tracks irreversible OffscreenCanvas transfers independently of Worker life. */
export class CanvasLeaseLedger {
  #sequence = 0;
  readonly #leases = new Map<number, CanvasLeaseSnapshot>();

  create(canvasId: string, sessionId: number): number {
    if (!canvasId || !Number.isSafeInteger(sessionId) || sessionId <= 0)
      throw new Error("Doroti canvas lease requires a canvas id and positive session id.");
    const leaseId = ++this.#sequence;
    this.#leases.set(leaseId, { leaseId, canvasId, sessionId, state: "created", terminalCount: 0 });
    return leaseId;
  }

  transferred(leaseId: number): void {
    const lease = this.#require(leaseId);
    if (lease.state !== "created")
      throw new Error(`Doroti canvas lease ${leaseId} cannot transfer from '${lease.state}'.`);
    this.#leases.set(leaseId, { ...lease, state: "transferred" });
  }

  retire(leaseId: number): void {
    const lease = this.#require(leaseId);
    if (lease.state === "retired")
      throw new Error(`Doroti canvas lease ${leaseId} has duplicate retirement.`);
    this.#leases.set(leaseId, { ...lease, state: "retired", terminalCount: 1 });
  }

  snapshot(): readonly CanvasLeaseSnapshot[] {
    return [...this.#leases.values()];
  }

  activeCount(): number {
    let active = 0;
    for (const lease of this.#leases.values()) if (lease.state !== "retired") active++;
    return active;
  }

  assertClosed(): void {
    const active = this.activeCount();
    if (active !== 0) throw new Error(`Doroti canvas lease ledger closed with ${active} active leases.`);
    for (const lease of this.#leases.values()) {
      if (lease.terminalCount !== 1)
        throw new Error(`Doroti canvas lease ${lease.leaseId} has ${lease.terminalCount} terminals.`);
    }
  }

  #require(leaseId: number): CanvasLeaseSnapshot {
    const lease = this.#leases.get(leaseId);
    if (!lease) throw new Error(`Unknown Doroti canvas lease ${leaseId}.`);
    return lease;
  }
}

/** A small return pool; transport never retains an unbounded detached-buffer backlog. */
export interface TransferBufferLease {
  readonly transferId: number;
  readonly sessionId: number;
  readonly byteLength: number;
  readonly buffer: ArrayBuffer;
}

export class TransferBufferPool {
  readonly #buffers: ArrayBuffer[] = [];
  readonly #outstanding = new Map<number, { sessionId: number; byteLength: number }>();
  readonly #capacity: number;
  #transferSequence = 0;
  created = 0;
  borrowed = 0;
  returned = 0;
  abandoned = 0;
  discarded = 0;

  constructor(capacity = 4) {
    if (!Number.isSafeInteger(capacity) || capacity < 0)
      throw new Error("Doroti transfer buffer pool capacity must be a non-negative integer.");
    this.#capacity = capacity;
  }

  copy(source: Uint8Array, sessionId: number): TransferBufferLease {
    if (!Number.isSafeInteger(sessionId) || sessionId <= 0)
      throw new Error("Doroti transfer buffer requires a positive owner session id.");
    let match = -1;
    for (let index = 0; index < this.#buffers.length; index++) {
      if (this.#buffers[index].byteLength === source.byteLength) {
        match = index;
        break;
      }
    }
    const buffer = match >= 0 ? this.#buffers.splice(match, 1)[0] : new ArrayBuffer(source.byteLength);
    if (match < 0) this.created++;
    this.borrowed++;
    new Uint8Array(buffer).set(source);
    const transferId = ++this.#transferSequence;
    this.#outstanding.set(transferId, { sessionId, byteLength: source.byteLength });
    this.#assertAccounting();
    return { transferId, sessionId, byteLength: source.byteLength, buffer };
  }

  release(transferId: number, sessionId: number, buffer: ArrayBuffer): void {
    const outstanding = this.#outstanding.get(transferId);
    if (!outstanding)
      throw new Error(`Doroti transfer buffer ${transferId} is not outstanding.`);
    if (outstanding.sessionId !== sessionId)
      throw new Error(
        `Doroti transfer buffer ${transferId} belongs to session ${outstanding.sessionId}, not ${sessionId}.`);
    if (buffer.byteLength !== outstanding.byteLength)
      throw new Error(
        `Doroti transfer buffer ${transferId} returned ${buffer.byteLength} bytes; expected ${outstanding.byteLength}.`);
    this.#outstanding.delete(transferId);
    this.returned++;
    if (this.#buffers.length >= this.#capacity) {
      this.discarded++;
      this.#assertAccounting();
      return;
    }
    this.#buffers.push(buffer);
    this.#assertAccounting();
  }

  abandonSession(sessionId: number): number {
    let count = 0;
    for (const [transferId, outstanding] of this.#outstanding) {
      if (outstanding.sessionId !== sessionId) continue;
      this.#outstanding.delete(transferId);
      this.abandoned++;
      this.discarded++;
      count++;
    }
    this.#assertAccounting();
    return count;
  }

  abandonAll(): number {
    const count = this.#outstanding.size;
    this.abandoned += count;
    this.discarded += count;
    this.#outstanding.clear();
    this.#assertAccounting();
    return count;
  }

  clear(): void {
    this.abandonAll();
    this.discarded += this.#buffers.length;
    this.#buffers.length = 0;
  }

  snapshot(): Readonly<Record<string, number>> {
    return {
      capacity: this.#capacity,
      pooled: this.#buffers.length,
      pooledBytes: this.#buffers.reduce((total, buffer) => total + buffer.byteLength, 0),
      created: this.created,
      borrowed: this.borrowed,
      returned: this.returned,
      abandoned: this.abandoned,
      discarded: this.discarded,
      outstanding: this.#outstanding.size,
      outstandingBytes: [...this.#outstanding.values()]
        .reduce((total, transfer) => total + transfer.byteLength, 0),
    };
  }

  #assertAccounting(): void {
    if (this.borrowed !== this.returned + this.abandoned + this.#outstanding.size)
      throw new Error("Doroti transfer-buffer terminal accounting is inconsistent.");
  }
}
