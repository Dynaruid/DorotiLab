export const dorotiProtocolVersion = 2 as const;

export type DorotiProtocolEnvelope = Readonly<Record<string, unknown>> & {
  readonly protocolVersion: typeof dorotiProtocolVersion;
  readonly kind: string;
};

export type DorotiRuntimeState = "created" | "booting" | "ready" | "disposing" | "disposed" | "fatal";

const transitions: Readonly<Record<DorotiRuntimeState, readonly DorotiRuntimeState[]>> = {
  created: ["booting", "fatal"],
  booting: ["ready", "disposing", "fatal"],
  ready: ["disposing", "fatal"],
  disposing: ["disposed", "fatal"],
  disposed: [],
  fatal: [],
};

export class DorotiRuntimeStateMachine {
  #state: DorotiRuntimeState = "created";
  get state(): DorotiRuntimeState { return this.#state; }

  transition(next: DorotiRuntimeState): void {
    if (!transitions[this.#state].includes(next))
      throw new Error(`Illegal Doroti runtime transition '${this.#state}' -> '${next}'.`);
    this.#state = next;
  }
}

export function decodeDorotiMessage(
  value: unknown,
  allowedKinds: ReadonlySet<string>,
): DorotiProtocolEnvelope {
  if (!value || typeof value !== "object" || Array.isArray(value))
    throw new Error("Doroti protocol message must be an object.");
  const message = value as Record<string, unknown>;
  if (message.protocolVersion !== dorotiProtocolVersion)
    throw new Error(`Unsupported Doroti protocol version '${String(message.protocolVersion)}'.`);
  if (typeof message.kind !== "string" || !allowedKinds.has(message.kind))
    throw new Error(`Unknown Doroti protocol message kind '${String(message.kind)}'.`);
  return message as DorotiProtocolEnvelope;
}

export function requirePositiveSequence(message: DorotiProtocolEnvelope, name: string): number {
  const value = Number(message[name]);
  if (!Number.isSafeInteger(value) || value <= 0)
    throw new Error(`Doroti protocol '${name}' must be a positive safe integer.`);
  return value;
}
