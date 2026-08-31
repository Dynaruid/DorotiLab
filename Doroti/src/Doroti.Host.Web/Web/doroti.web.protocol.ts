export const dorotiProtocolVersion = 2 as const;

// The control envelope and the binary scene schema evolve independently. The
// former is shared by main/UI/Raster workers, while the latter is the stable
// Doroti.Graphics.DisplayList wire contract.
export const dorotiCanvasKitTopologyVersion = 1 as const;
export const dorotiDisplayListSchemaVersion = 2 as const;
export const dorotiDisplayListMagic = 0x54534c44 as const;
export const dorotiDisplayListHeaderSize = 112 as const;
export const dorotiDisplayListResourceEntrySize = 32 as const;
export const dorotiDisplayListCommandEnvelopeSize = 8 as const;
export const dorotiDisplayListMaximumByteLength = 64 * 1024 * 1024;
export const dorotiDisplayListMaximumCommandCount = 1_000_000;
export const dorotiDisplayListMaximumResourceCount = 65_536;
export const dorotiDisplayListMaximumStringBytes = 16 * 1024 * 1024;
export const dorotiDisplayListMaximumCollectionCount = 1_000_000;
export const dorotiDisplayListMaximumNestingDepth = 32;

export type DorotiSceneTerminal = "submitted" | "superseded" | "failed";

export interface DorotiDisplayListMetadata {
  readonly byteLength: number;
  readonly flags: number;
  readonly viewId: bigint;
  readonly sceneSequence: bigint;
  readonly buildToken: bigint;
  readonly resizeEpoch: bigint;
  readonly surfaceGeneration: bigint;
  readonly contextGeneration: bigint;
  readonly logicalWidth: number;
  readonly logicalHeight: number;
  readonly physicalWidth: number;
  readonly physicalHeight: number;
  readonly devicePixelRatio: number;
  readonly commandCount: number;
  readonly resourceCount: number;
  readonly stringBytes: number;
  readonly commandBytes: number;
  readonly resourceBytes: number;
  readonly checksum: number;
}

export interface DorotiDisplayListCommandEnvelope {
  readonly opcode: number;
  readonly payloadOffset: number;
  readonly payloadLength: number;
}

export interface DorotiDisplayListResourceDescriptor {
  readonly kind: number;
  readonly flags: number;
  readonly version: number;
  readonly id: bigint;
  readonly fingerprintLow: bigint;
  readonly fingerprintHigh: bigint;
}

export interface DorotiValidatedDisplayList {
  readonly metadata: DorotiDisplayListMetadata;
  readonly resources: readonly DorotiDisplayListResourceDescriptor[];
  readonly strings: readonly string[];
  readonly commands: readonly DorotiDisplayListCommandEnvelope[];
}

const displayListKnownFlags = 0b11;
const displayResourceKnownFlags = 0b1;
const knownDisplayListOpcodes = new Set([
  1, 2, 3, 4, 5, 6, 7,
  16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31,
  48, 49, 50, 51, 52, 53,
]);

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

export function displayListSequenceAsNumber(sequence: bigint): number {
  const value = Number(sequence);
  if (!Number.isSafeInteger(value) || value <= 0 || BigInt(value) !== sequence)
    throw new Error(`Doroti DisplayList scene sequence '${sequence}' is not a positive exact JavaScript integer.`);
  return value;
}

export function canvasKitSurfaceGeneration(
  rasterSessionId: number,
  resizeEpochGeneration: number,
): number {
  if (!Number.isSafeInteger(rasterSessionId) || rasterSessionId <= 0 ||
      !Number.isSafeInteger(resizeEpochGeneration) || resizeEpochGeneration <= 0 ||
      resizeEpochGeneration >= 0x1_0000_0000)
    throw new Error("Doroti CanvasKit surface identity requires positive bounded session/resize generations.");
  const value = rasterSessionId * 0x1_0000_0000 + resizeEpochGeneration;
  if (!Number.isSafeInteger(value))
    throw new Error("Doroti CanvasKit surface identity exceeds exact JavaScript integer range.");
  return value;
}

export function validateDorotiDisplayList(value: ArrayBuffer | Uint8Array): DorotiValidatedDisplayList {
  const bytes = value instanceof Uint8Array
    ? new Uint8Array(value.buffer, value.byteOffset, value.byteLength)
    : new Uint8Array(value);
  if (bytes.byteLength < dorotiDisplayListHeaderSize)
    throw new Error(`Doroti DisplayList BufferTooShort: ${bytes.byteLength} < ${dorotiDisplayListHeaderSize}.`);
  if (bytes.byteLength > dorotiDisplayListMaximumByteLength)
    throw new Error(`Doroti DisplayList LimitExceeded: ${bytes.byteLength} bytes.`);
  const view = new DataView(bytes.buffer, bytes.byteOffset, bytes.byteLength);
  const magic = view.getUint32(0, true);
  const schemaVersion = view.getUint16(4, true);
  const headerSize = view.getUint16(6, true);
  const byteLength = view.getUint32(8, true);
  const flags = view.getUint32(12, true);
  if (magic !== dorotiDisplayListMagic)
    throw new Error(`Doroti DisplayList InvalidMagic: 0x${magic.toString(16)}.`);
  if (schemaVersion !== dorotiDisplayListSchemaVersion)
    throw new Error(`Doroti DisplayList UnsupportedVersion: ${schemaVersion}.`);
  if (headerSize !== dorotiDisplayListHeaderSize || view.getUint32(108, true) !== 0)
    throw new Error("Doroti DisplayList InvalidHeader: non-canonical header size or reserved field.");
  if (byteLength !== bytes.byteLength)
    throw new Error(`Doroti DisplayList LengthMismatch: header=${byteLength}, actual=${bytes.byteLength}.`);
  if ((flags & ~displayListKnownFlags) !== 0)
    throw new Error(`Doroti DisplayList UnknownFlags: 0x${flags.toString(16)}.`);

  const metadata: DorotiDisplayListMetadata = {
    byteLength,
    flags,
    viewId: view.getBigUint64(16, true),
    sceneSequence: view.getBigUint64(24, true),
    buildToken: view.getBigUint64(32, true),
    resizeEpoch: view.getBigUint64(40, true),
    surfaceGeneration: view.getBigUint64(48, true),
    contextGeneration: view.getBigUint64(56, true),
    logicalWidth: view.getFloat32(64, true),
    logicalHeight: view.getFloat32(68, true),
    physicalWidth: view.getUint32(72, true),
    physicalHeight: view.getUint32(76, true),
    devicePixelRatio: view.getFloat32(80, true),
    commandCount: view.getUint32(84, true),
    resourceCount: view.getUint32(88, true),
    stringBytes: view.getUint32(92, true),
    commandBytes: view.getUint32(96, true),
    resourceBytes: view.getUint32(100, true),
    checksum: view.getUint32(104, true),
  };
  validateDisplayListMetadata(metadata);
  const computedLength = dorotiDisplayListHeaderSize + metadata.resourceBytes +
    metadata.stringBytes + metadata.commandBytes;
  if (computedLength !== byteLength)
    throw new Error(`Doroti DisplayList LengthMismatch: sections=${computedLength}, header=${byteLength}.`);
  if ((flags & 1) !== 0 && crc32DisplayList(bytes) !== metadata.checksum)
    throw new Error("Doroti DisplayList ChecksumMismatch.");
  if ((flags & 1) === 0 && metadata.checksum !== 0)
    throw new Error("Doroti DisplayList NonCanonicalEncoding: checksum must be zero when absent.");

  const resources = validateResourceTable(view, metadata);
  const strings = validateStringTable(bytes, metadata);
  const commands = validateCommandTable(view, metadata, resources, strings.length);
  return { metadata, resources, strings, commands };
}

function validateDisplayListMetadata(metadata: DorotiDisplayListMetadata): void {
  if (metadata.viewId === 0n || metadata.sceneSequence === 0n || metadata.buildToken === 0n ||
      metadata.resizeEpoch === 0n || metadata.surfaceGeneration === 0n ||
      metadata.contextGeneration === 0n)
    throw new Error("Doroti DisplayList InvalidHeader: scene identities and generations must be nonzero.");
  if (!Number.isFinite(metadata.logicalWidth) || metadata.logicalWidth <= 0 ||
      Object.is(metadata.logicalWidth, -0) ||
      !Number.isFinite(metadata.logicalHeight) || metadata.logicalHeight <= 0 ||
      Object.is(metadata.logicalHeight, -0) ||
      metadata.physicalWidth <= 0 || metadata.physicalHeight <= 0 ||
      !Number.isFinite(metadata.devicePixelRatio) || metadata.devicePixelRatio <= 0 ||
      Object.is(metadata.devicePixelRatio, -0))
    throw new Error("Doroti DisplayList InvalidValue: invalid logical/physical geometry or DPR.");
  if (metadata.commandCount > dorotiDisplayListMaximumCommandCount ||
      metadata.resourceCount > dorotiDisplayListMaximumResourceCount ||
      metadata.stringBytes > dorotiDisplayListMaximumStringBytes)
    throw new Error("Doroti DisplayList LimitExceeded: collection count or string table size.");
  if (metadata.resourceBytes !== metadata.resourceCount * dorotiDisplayListResourceEntrySize)
    throw new Error("Doroti DisplayList InvalidResource: resource table length is non-canonical.");
  if (metadata.commandCount * dorotiDisplayListCommandEnvelopeSize > metadata.commandBytes)
    throw new Error("Doroti DisplayList InvalidHeader: command envelopes exceed the command section.");
}

function validateResourceTable(
  view: DataView,
  metadata: DorotiDisplayListMetadata,
): DorotiDisplayListResourceDescriptor[] {
  let previous: readonly [number, bigint, number] | null = null;
  const resources: DorotiDisplayListResourceDescriptor[] = [];
  for (let index = 0; index < metadata.resourceCount; index++) {
    const offset = dorotiDisplayListHeaderSize + index * dorotiDisplayListResourceEntrySize;
    const kind = view.getUint16(offset, true);
    const flags = view.getUint16(offset + 2, true);
    const version = view.getUint32(offset + 4, true);
    const id = view.getBigUint64(offset + 8, true);
    if (kind < 1 || kind > 4 || (flags & ~displayResourceKnownFlags) !== 0 || version === 0 || id === 0n)
      throw new Error(`Doroti DisplayList InvalidResource at ${offset}.`);
    const current = [kind, id, version] as const;
    if (previous && compareResourceKeys(previous, current) >= 0)
      throw new Error(`Doroti DisplayList DuplicateResource or non-canonical order at ${offset}.`);
    previous = current;
    resources.push({
      kind,
      flags,
      version,
      id,
      fingerprintLow: view.getBigUint64(offset + 16, true),
      fingerprintHigh: view.getBigUint64(offset + 24, true),
    });
  }
  return resources;
}

function validateStringTable(
  bytes: Uint8Array,
  metadata: DorotiDisplayListMetadata,
): string[] {
  const start = dorotiDisplayListHeaderSize + metadata.resourceBytes;
  const end = start + metadata.stringBytes;
  let offset = start;
  let previous: Uint8Array | null = null;
  const strings: string[] = [];
  const decoder = new TextDecoder("utf-8", { fatal: true, ignoreBOM: true });
  while (offset < end) {
    if (strings.length >= dorotiDisplayListMaximumCollectionCount)
      throw new Error("Doroti DisplayList LimitExceeded: string count.");
    if (offset > end - 4)
      throw new Error(`Doroti DisplayList InvalidString: truncated length at ${offset}.`);
    const view = new DataView(bytes.buffer, bytes.byteOffset + offset, end - offset);
    const length = view.getUint32(0, true);
    offset += 4;
    if (length > end - offset)
      throw new Error(`Doroti DisplayList InvalidString: bounds exceeded at ${offset - 4}.`);
    const encoded = bytes.subarray(offset, offset + length);
    if (previous && compareBytes(previous, encoded) >= 0)
      throw new Error(`Doroti DisplayList NonCanonicalEncoding: string order at ${offset - 4}.`);
    try {
      strings.push(decoder.decode(encoded));
    } catch (error) {
      throw new Error(`Doroti DisplayList InvalidString at ${offset}: ${String(error)}`);
    }
    previous = encoded.slice();
    offset += length;
  }
  if (offset !== end)
    throw new Error("Doroti DisplayList InvalidString: trailing string-table bytes.");
  return strings;
}

function compareBytes(left: Uint8Array, right: Uint8Array): number {
  const common = Math.min(left.byteLength, right.byteLength);
  for (let index = 0; index < common; index++) {
    if (left[index] !== right[index]) return left[index] - right[index];
  }
  return left.byteLength - right.byteLength;
}

function compareResourceKeys(
  left: readonly [number, bigint, number],
  right: readonly [number, bigint, number],
): number {
  if (left[0] !== right[0]) return left[0] - right[0];
  if (left[1] !== right[1]) return left[1] < right[1] ? -1 : 1;
  return left[2] - right[2];
}

function validateCommandTable(
  view: DataView,
  metadata: DorotiDisplayListMetadata,
  resources: readonly DorotiDisplayListResourceDescriptor[],
  stringCount: number,
): DorotiDisplayListCommandEnvelope[] {
  let offset = dorotiDisplayListHeaderSize + metadata.resourceBytes + metadata.stringBytes;
  const end = offset + metadata.commandBytes;
  const commands: DorotiDisplayListCommandEnvelope[] = [];
  const context = new DisplayListValidationContext(resources, stringCount);
  for (let index = 0; index < metadata.commandCount; index++) {
    if (offset > end - dorotiDisplayListCommandEnvelopeSize)
      throw new Error(`Doroti DisplayList InvalidCommand: truncated envelope ${index}.`);
    const opcode = view.getUint16(offset, true);
    const flags = view.getUint16(offset + 2, true);
    const payloadLength = view.getUint32(offset + 4, true);
    const payloadOffset = offset + dorotiDisplayListCommandEnvelopeSize;
    if (!knownDisplayListOpcodes.has(opcode))
      throw new Error(`Doroti DisplayList UnknownOpcode: ${opcode}.`);
    if (flags !== 0 || payloadOffset + payloadLength > end)
      throw new Error(`Doroti DisplayList InvalidCommand at ${offset}.`);
    validateCommandPayload(
      opcode,
      new DisplayListSectionReader(view, payloadOffset, payloadOffset + payloadLength),
      context);
    commands.push({ opcode, payloadOffset, payloadLength });
    offset = payloadOffset + payloadLength;
  }
  if (offset !== end)
    throw new Error(`Doroti DisplayList NonCanonicalEncoding: ${end - offset} trailing command bytes.`);
  return commands;
}

class DisplayListSectionReader {
  readonly #view: DataView;
  readonly #end: number;
  #offset: number;

  constructor(view: DataView, start: number, end: number) {
    this.#view = view;
    this.#offset = start;
    this.#end = end;
  }

  get offset(): number { return this.#offset; }
  get remaining(): number { return this.#end - this.#offset; }

  readByte(): number {
    this.#ensure(1);
    return this.#view.getUint8(this.#offset++);
  }

  readBoolean(): boolean {
    const offset = this.#offset;
    const value = this.readByte();
    if (value > 1)
      throw new Error(`Doroti DisplayList InvalidValue: boolean ${value} at ${offset}.`);
    return value === 1;
  }

  readUint16(): number {
    this.#ensure(2);
    const value = this.#view.getUint16(this.#offset, true);
    this.#offset += 2;
    return value;
  }

  readUint32(): number {
    this.#ensure(4);
    const value = this.#view.getUint32(this.#offset, true);
    this.#offset += 4;
    return value;
  }

  readInt32(): number {
    this.#ensure(4);
    const value = this.#view.getInt32(this.#offset, true);
    this.#offset += 4;
    return value;
  }

  readUint64(): bigint {
    this.#ensure(8);
    const value = this.#view.getBigUint64(this.#offset, true);
    this.#offset += 8;
    return value;
  }

  readSingle(): number {
    this.#ensure(4);
    const offset = this.#offset;
    const bits = this.#view.getUint32(offset, true);
    const value = this.#view.getFloat32(offset, true);
    this.#offset += 4;
    if (bits === 0x8000_0000)
      throw new Error(`Doroti DisplayList NonCanonicalEncoding: negative zero at ${offset}.`);
    if (!Number.isFinite(value))
      throw new Error(`Doroti DisplayList InvalidValue: non-finite float at ${offset}.`);
    return value;
  }

  readCount(name: string): number {
    const offset = this.#offset;
    const value = this.readUint32();
    if (value > dorotiDisplayListMaximumCollectionCount)
      throw new Error(`Doroti DisplayList LimitExceeded: ${name} count at ${offset}.`);
    return value;
  }

  readEnum(maximum: number, name: string): number {
    const offset = this.#offset;
    const value = this.readByte();
    if (value > maximum)
      throw new Error(`Doroti DisplayList InvalidValue: ${name} ${value} at ${offset}.`);
    return value;
  }

  skip(length: number): void {
    this.#ensure(length);
    this.#offset += length;
  }

  requireZero(value: number | bigint, name: string, offset = this.#offset): void {
    if (value !== 0 && value !== 0n)
      throw new Error(`Doroti DisplayList NonCanonicalEncoding: ${name} at ${offset}.`);
  }

  requireFinished(opcode: number): void {
    if (this.remaining !== 0)
      throw new Error(
        `Doroti DisplayList InvalidCommand: opcode ${opcode} has ${this.remaining} trailing payload bytes.`);
  }

  #ensure(length: number): void {
    if (!Number.isSafeInteger(length) || length < 0 || length > this.remaining)
      throw new Error(`Doroti DisplayList BoundsExceeded at ${this.#offset}: ${length} bytes.`);
  }
}

class DisplayListValidationContext {
  readonly #resources: ReadonlySet<string>;
  readonly #stringCount: number;

  constructor(resources: readonly DorotiDisplayListResourceDescriptor[], stringCount: number) {
    this.#resources = new Set(resources.map((resource) =>
      displayResourceKey(resource.kind, resource.id, resource.version)));
    this.#stringCount = stringCount;
  }

  readResource(reader: DisplayListSectionReader, expectedKind?: number): void {
    const offset = reader.offset;
    const kind = reader.readUint16();
    if (kind < 1 || kind > 4)
      throw new Error(`Doroti DisplayList InvalidResource: kind ${kind} at ${offset}.`);
    reader.requireZero(reader.readUint16(), "resource-reference reserved field", reader.offset - 2);
    const version = reader.readUint32();
    const id = reader.readUint64();
    if (version === 0 || id === 0n)
      throw new Error(`Doroti DisplayList InvalidResource: zero id/version at ${offset}.`);
    if (expectedKind !== undefined && kind !== expectedKind)
      throw new Error(
        `Doroti DisplayList InvalidResource: kind ${kind} does not match ${expectedKind} at ${offset}.`);
    if (!this.#resources.has(displayResourceKey(kind, id, version)))
      throw new Error(`Doroti DisplayList MissingResource at ${offset}.`);
  }

  readString(reader: DisplayListSectionReader, allowNull: boolean): void {
    const offset = reader.offset;
    const id = reader.readUint32();
    if (allowNull && id === 0xffff_ffff) return;
    if (id >= this.#stringCount)
      throw new Error(`Doroti DisplayList InvalidString: id ${id} at ${offset}.`);
  }
}

function displayResourceKey(kind: number, id: bigint, version: number): string {
  return `${kind}:${id.toString(10)}:${version}`;
}

function validateCommandPayload(
  opcode: number,
  reader: DisplayListSectionReader,
  context: DisplayListValidationContext,
): void {
  switch (opcode) {
    case 1:
    case 2:
      break;
    case 3:
      readOptionalRect(reader);
      readOptionalPaint(reader, context, 0);
      break;
    case 4:
      readMatrix(reader);
      break;
    case 5:
      readRect(reader);
      reader.readEnum(1, "clip operation");
      reader.readBoolean();
      reader.requireZero(reader.readUint16(), "clip-rect reserved field", reader.offset - 2);
      break;
    case 6:
      readRoundedRect(reader);
      reader.readEnum(1, "clip operation");
      reader.readBoolean();
      reader.requireZero(reader.readUint16(), "clip-rounded-rect reserved field", reader.offset - 2);
      break;
    case 7:
      readPath(reader);
      reader.readEnum(1, "clip operation");
      reader.readBoolean();
      reader.requireZero(reader.readUint16(), "clip-path reserved field", reader.offset - 2);
      break;
    case 16:
      reader.readUint32();
      reader.readEnum(28, "blend mode");
      break;
    case 17:
      readPaint(reader, context, 0);
      break;
    case 18:
      readPoint(reader);
      readPoint(reader);
      readPaint(reader, context, 0);
      break;
    case 19: {
      reader.readEnum(2, "point mode");
      const count = reader.readCount("point");
      if (count * 8 > reader.remaining)
        throw new Error(`Doroti DisplayList BoundsExceeded: point array at ${reader.offset}.`);
      for (let index = 0; index < count; index++) readPoint(reader);
      readPaint(reader, context, 0);
      break;
    }
    case 20:
      readRect(reader);
      readPaint(reader, context, 0);
      break;
    case 21:
      readRoundedRect(reader);
      readPaint(reader, context, 0);
      break;
    case 22:
      readRoundedRect(reader);
      readRoundedRect(reader);
      readPaint(reader, context, 0);
      break;
    case 23:
      readPoint(reader);
      readNonnegativeSingle(reader, "circle radius");
      readPaint(reader, context, 0);
      break;
    case 24:
      readRect(reader);
      readPaint(reader, context, 0);
      break;
    case 25:
      readRect(reader);
      reader.readSingle();
      reader.readSingle();
      reader.readBoolean();
      readPaint(reader, context, 0);
      break;
    case 26:
      readPath(reader);
      readPaint(reader, context, 0);
      break;
    case 27:
      readPath(reader);
      reader.readUint32();
      readNonnegativeSingle(reader, "shadow elevation");
      reader.readBoolean();
      break;
    case 28:
      context.readResource(reader, 2);
      readPoint(reader);
      reader.readEnum(3, "sampling quality");
      readPaint(reader, context, 0);
      break;
    case 29:
    case 30:
      context.readResource(reader, 2);
      readRect(reader);
      readRect(reader);
      reader.readEnum(3, "sampling quality");
      readPaint(reader, context, 0);
      break;
    case 31:
      readParagraph(reader, context);
      readPoint(reader);
      break;
    case 48: {
      const opacity = reader.readSingle();
      if (opacity < 0 || opacity > 1)
        throw new Error(`Doroti DisplayList InvalidValue: opacity ${opacity}.`);
      readPoint(reader);
      break;
    }
    case 49:
      readColorFilter(reader, context, 0, false);
      readPoint(reader);
      break;
    case 50:
      readImageFilter(reader, context, 0, false);
      readPoint(reader);
      readOptionalRect(reader);
      break;
    case 51:
      readImageFilter(reader, context, 0, false);
      reader.readEnum(28, "blend mode");
      reader.readUint64();
      readPoint(reader);
      break;
    case 52:
      readShader(reader, context, 0, false);
      readRect(reader);
      reader.readEnum(28, "blend mode");
      break;
    case 53:
      context.readResource(reader, 4);
      readPoint(reader);
      if ((reader.readByte() & ~0b11) !== 0)
        throw new Error("Doroti DisplayList InvalidValue: retained-scene cache hint.");
      break;
    default:
      throw new Error(`Doroti DisplayList UnknownOpcode: ${opcode}.`);
  }
  reader.requireFinished(opcode);
}

function readPoint(reader: DisplayListSectionReader): void {
  reader.readSingle();
  reader.readSingle();
}

function readRect(reader: DisplayListSectionReader): void {
  for (let index = 0; index < 4; index++) reader.readSingle();
}

function readOptionalRect(reader: DisplayListSectionReader): void {
  if (reader.readBoolean()) readRect(reader);
}

function readRoundedRect(reader: DisplayListSectionReader): void {
  readRect(reader);
  for (let index = 0; index < 8; index++)
    readNonnegativeSingle(reader, "rounded-rect radius");
}

function readMatrix(reader: DisplayListSectionReader): void {
  for (let index = 0; index < 16; index++) reader.readSingle();
}

function readOptionalMatrix(reader: DisplayListSectionReader): void {
  if (reader.readBoolean()) readMatrix(reader);
}

function readPositiveSingle(reader: DisplayListSectionReader, name: string): number {
  const value = reader.readSingle();
  if (value <= 0) throw new Error(`Doroti DisplayList InvalidValue: ${name} must be positive.`);
  return value;
}

function readNonnegativeSingle(reader: DisplayListSectionReader, name: string): number {
  const value = reader.readSingle();
  if (value < 0) throw new Error(`Doroti DisplayList InvalidValue: ${name} cannot be negative.`);
  return value;
}

function requireDepth(depth: number, reader: DisplayListSectionReader): void {
  if (depth > dorotiDisplayListMaximumNestingDepth)
    throw new Error(`Doroti DisplayList LimitExceeded: tagged-value depth at ${reader.offset}.`);
}

function readPath(reader: DisplayListSectionReader): void {
  reader.readEnum(1, "path fill type");
  reader.requireZero(reader.readByte(), "path reserved byte", reader.offset - 1);
  reader.requireZero(reader.readUint16(), "path reserved field", reader.offset - 2);
  const verbCount = reader.readCount("path verb");
  const valueCount = reader.readCount("path value");
  if (verbCount > reader.remaining)
    throw new Error(`Doroti DisplayList BoundsExceeded: path verbs at ${reader.offset}.`);
  const valueCounts = [2, 2, 2, 2, 4, 5, 6, 4, 4, 6, 12, 12, 7, 7, 0] as const;
  let expectedValueCount = 0;
  for (let index = 0; index < verbCount; index++) {
    const verb = reader.readEnum(14, "path verb");
    expectedValueCount += valueCounts[verb];
  }
  if (expectedValueCount !== valueCount)
    throw new Error(
      `Doroti DisplayList InvalidValue: path declares ${valueCount}, expected ${expectedValueCount}.`);
  if (valueCount * 4 > reader.remaining)
    throw new Error(`Doroti DisplayList BoundsExceeded: path values at ${reader.offset}.`);
  for (let index = 0; index < valueCount; index++) reader.readSingle();
}

function readOptionalPaint(
  reader: DisplayListSectionReader,
  context: DisplayListValidationContext,
  depth: number,
): void {
  if (reader.readBoolean()) readPaint(reader, context, depth);
}

function readPaint(
  reader: DisplayListSectionReader,
  context: DisplayListValidationContext,
  depth: number,
): void {
  requireDepth(depth, reader);
  reader.readUint32();
  reader.readEnum(1, "paint style");
  reader.readEnum(2, "stroke cap");
  reader.readEnum(2, "stroke join");
  reader.readBoolean();
  reader.readEnum(28, "blend mode");
  reader.readEnum(3, "sampling quality");
  reader.readBoolean();
  reader.requireZero(reader.readByte(), "paint reserved field", reader.offset - 1);
  readNonnegativeSingle(reader, "stroke width");
  readNonnegativeSingle(reader, "stroke miter limit");
  readShader(reader, context, depth + 1, true);
  readColorFilter(reader, context, depth + 1, true);
  readMaskFilter(reader);
  readImageFilter(reader, context, depth + 1, true);
}

function readShader(
  reader: DisplayListSectionReader,
  context: DisplayListValidationContext,
  depth: number,
  allowNull: boolean,
): number {
  requireDepth(depth, reader);
  const tag = reader.readByte();
  if (tag === 0) {
    if (!allowNull) throw new Error("Doroti DisplayList InvalidValue: required shader is null.");
    return tag;
  }
  switch (tag) {
    case 1:
      readPoint(reader);
      readPoint(reader);
      reader.readEnum(3, "tile mode");
      readGradient(reader);
      readOptionalMatrix(reader);
      break;
    case 2:
      readPoint(reader);
      readNonnegativeSingle(reader, "gradient radius");
      reader.readEnum(3, "tile mode");
      if (reader.readBoolean()) readPoint(reader);
      readNonnegativeSingle(reader, "gradient focal radius");
      readGradient(reader);
      readOptionalMatrix(reader);
      break;
    case 3:
      readPoint(reader);
      reader.readSingle();
      reader.readSingle();
      reader.readEnum(3, "tile mode");
      readGradient(reader);
      readOptionalMatrix(reader);
      break;
    case 4:
      context.readResource(reader, 2);
      reader.readEnum(3, "horizontal tile mode");
      reader.readEnum(3, "vertical tile mode");
      reader.readEnum(3, "sampling quality");
      reader.requireZero(reader.readByte(), "image-shader reserved field", reader.offset - 1);
      readMatrix(reader);
      break;
    case 5: {
      context.readResource(reader, 3);
      const uniformCount = reader.readCount("runtime-effect uniform byte");
      reader.skip(uniformCount);
      const childCount = reader.readCount("runtime-effect child");
      if (childCount * 16 > reader.remaining)
        throw new Error(`Doroti DisplayList BoundsExceeded: runtime-effect children at ${reader.offset}.`);
      for (let index = 0; index < childCount; index++) context.readResource(reader);
      break;
    }
    default:
      throw new Error(`Doroti DisplayList InvalidValue: shader tag ${tag}.`);
  }
  return tag;
}

function readGradient(reader: DisplayListSectionReader): void {
  const count = reader.readCount("gradient stop");
  if (count < 2) throw new Error("Doroti DisplayList InvalidValue: gradient requires two stops.");
  if (count * 8 > reader.remaining)
    throw new Error(`Doroti DisplayList BoundsExceeded: gradient stops at ${reader.offset}.`);
  let previous = Number.NEGATIVE_INFINITY;
  for (let index = 0; index < count; index++) {
    reader.readUint32();
    const stop = reader.readSingle();
    if (stop < previous)
      throw new Error(`Doroti DisplayList InvalidValue: unsorted gradient stop at ${reader.offset - 4}.`);
    previous = stop;
  }
}

function readColorFilter(
  reader: DisplayListSectionReader,
  context: DisplayListValidationContext,
  depth: number,
  allowNull: boolean,
): number {
  void context;
  requireDepth(depth, reader);
  const tag = reader.readByte();
  if (tag === 0) {
    if (!allowNull) throw new Error("Doroti DisplayList InvalidValue: required color filter is null.");
    return tag;
  }
  switch (tag) {
    case 1:
      reader.readUint32();
      reader.readEnum(28, "blend mode");
      break;
    case 2:
      for (let index = 0; index < 20; index++) reader.readSingle();
      break;
    case 3:
    case 4:
      break;
    default:
      throw new Error(`Doroti DisplayList InvalidValue: color-filter tag ${tag}.`);
  }
  return tag;
}

function readMaskFilter(reader: DisplayListSectionReader): void {
  if (!reader.readBoolean()) return;
  reader.readEnum(3, "blur style");
  readNonnegativeSingle(reader, "mask-filter sigma");
}

function readImageFilter(
  reader: DisplayListSectionReader,
  context: DisplayListValidationContext,
  depth: number,
  allowNull: boolean,
): number {
  requireDepth(depth, reader);
  const tag = reader.readByte();
  if (tag === 0) {
    if (!allowNull) throw new Error("Doroti DisplayList InvalidValue: required image filter is null.");
    return tag;
  }
  switch (tag) {
    case 1:
      readNonnegativeSingle(reader, "horizontal blur sigma");
      readNonnegativeSingle(reader, "vertical blur sigma");
      reader.readEnum(3, "tile mode");
      readOptionalRect(reader);
      break;
    case 2:
      readColorFilter(reader, context, depth + 1, false);
      break;
    case 3:
      readMatrix(reader);
      reader.readEnum(3, "sampling quality");
      break;
    case 4:
      if (readShader(reader, context, depth + 1, false) !== 5)
        throw new Error("Doroti DisplayList InvalidValue: runtime-effect image filter shader tag.");
      reader.readEnum(3, "sampling quality");
      break;
    case 5:
      readImageFilter(reader, context, depth + 1, false);
      readImageFilter(reader, context, depth + 1, false);
      break;
    case 6:
      reader.readSingle();
      reader.readSingle();
      readNonnegativeSingle(reader, "horizontal shadow sigma");
      readNonnegativeSingle(reader, "vertical shadow sigma");
      reader.readUint32();
      reader.readBoolean();
      break;
    default:
      throw new Error(`Doroti DisplayList InvalidValue: image-filter tag ${tag}.`);
  }
  return tag;
}

function readParagraph(
  reader: DisplayListSectionReader,
  context: DisplayListValidationContext,
): void {
  context.readString(reader, false);
  context.readResource(reader, 1);
  context.readString(reader, false);
  context.readString(reader, false);
  context.readString(reader, true);
  readPositiveSingle(reader, "font size");
  readPositiveSingle(reader, "paragraph height multiplier");
  reader.readUint32();
  const weight = reader.readInt32();
  if (weight < 1 || weight > 1000)
    throw new Error(`Doroti DisplayList InvalidValue: font weight ${weight}.`);
  reader.readEnum(1, "font slant");
  reader.readEnum(1, "text direction");
  reader.readEnum(5, "text alignment");
  reader.requireZero(reader.readByte(), "paragraph reserved field", reader.offset - 1);
  reader.readUint32();
  readNonnegativeSingle(reader, "paragraph layout width");
  readNonnegativeSingle(reader, "paragraph measured width");
  readNonnegativeSingle(reader, "paragraph measured height");
  reader.readUint64();
  const fallbackCount = reader.readCount("fallback font");
  if (fallbackCount * 16 > reader.remaining)
    throw new Error(`Doroti DisplayList BoundsExceeded: fallback fonts at ${reader.offset}.`);
  for (let index = 0; index < fallbackCount; index++) context.readResource(reader, 1);
  const runCount = reader.readCount("paragraph text run");
  for (let index = 0; index < runCount; index++) {
    context.readString(reader, false);
    context.readString(reader, false);
    context.readString(reader, false);
    readPositiveSingle(reader, "run font size");
    readPositiveSingle(reader, "run height multiplier");
    reader.readUint32();
    const runWeight = reader.readInt32();
    if (runWeight < 1 || runWeight > 1000)
      throw new Error(`Doroti DisplayList InvalidValue: run font weight ${runWeight}.`);
    reader.readEnum(1, "run font slant");
    const decoration = reader.readUint32();
    if ((decoration & ~7) !== 0)
      throw new Error(`Doroti DisplayList InvalidValue: run decoration ${decoration}.`);
    if (reader.readBoolean()) reader.readUint32();
    if (reader.readBoolean()) reader.readUint32();
    if (reader.readBoolean()) reader.readEnum(4, "run decoration style");
    if (reader.readBoolean()) readNonnegativeSingle(reader, "run decoration thickness");
    if (reader.readBoolean()) reader.readEnum(1, "run text baseline");
    if (reader.readBoolean()) reader.readSingle();
    if (reader.readBoolean()) reader.readSingle();
    const halfLeading = reader.readByte();
    if (halfLeading > 2)
      throw new Error(`Doroti DisplayList InvalidValue: run half-leading state ${halfLeading}.`);
    const familyCount = reader.readCount("run fallback font family");
    for (let family = 0; family < familyCount; family++) context.readString(reader, false);
    const shadowCount = reader.readCount("run shadow");
    for (let shadow = 0; shadow < shadowCount; shadow++) {
      reader.readUint32();
      reader.readSingle();
      reader.readSingle();
      readNonnegativeSingle(reader, "run shadow blur radius");
    }
    const featureCount = reader.readCount("run font feature");
    for (let feature = 0; feature < featureCount; feature++) {
      context.readString(reader, false);
      reader.readInt32();
    }
    const variationCount = reader.readCount("run font variation");
    for (let variation = 0; variation < variationCount; variation++) {
      context.readString(reader, false);
      reader.readSingle();
    }
  }
}

function crc32DisplayList(bytes: Uint8Array): number {
  let crc = 0xffffffff;
  for (let index = 0; index < bytes.byteLength; index++) {
    const value = index >= 104 && index < 108 ? 0 : bytes[index];
    crc ^= value;
    for (let bit = 0; bit < 8; bit++)
      crc = (crc >>> 1) ^ ((crc & 1) === 0 ? 0 : 0xedb88320);
  }
  return (crc ^ 0xffffffff) >>> 0;
}
