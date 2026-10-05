export interface SemanticsFlags {
  checked?: string; selected?: boolean; enabled?: boolean; toggled?: boolean;
  expanded?: boolean; required?: boolean; focused?: boolean; button?: boolean;
  textField?: boolean; header?: boolean; hidden?: boolean; image?: boolean;
  liveRegion?: boolean; multiline?: boolean; readOnly?: boolean; link?: boolean; slider?: boolean;
  focusable?: boolean; obscured?: boolean; mutuallyExclusive?: boolean; keyboardKey?: boolean;
}

export interface SemanticsNode {
  id: number; role?: string; label?: string; value?: string; actions?: number;
  children: number[]; flags?: SemanticsFlags; textSelectionBase?: number; textSelectionExtent?: number;
  identifier?: string; hint?: string; tooltip?: string; increasedValue?: string; decreasedValue?: string;
  headingLevel?: number; linkUrl?: string; validationResult?: string; hitTestBehavior?: string;
  inputType?: string; minValue?: string; maxValue?: string; maxValueLength?: number;
  currentValueLength?: number; scrollPosition?: number; scrollExtentMin?: number;
  scrollExtentMax?: number; scrollChildCount?: number; scrollIndex?: number;
  controlsNodes?: string[]; locale?: string; rect: [number, number, number, number];
}

export interface SemanticsPacket {
  schemaVersion: 1; kind: "full" | "delta"; stream: number; revision: number; baseRevision: number;
  generation: number; nodes: (Partial<SemanticsNode> & { id: number; contentUnchanged?: boolean })[];
  removed: number[]; roots?: number[];
}

export interface SemanticsProjection {
  changed: Set<number>; content: Set<number>; parents: Set<number>; removed: number[]; rootsChanged: boolean;
}

/** Retains the wire tree, not DOM handles. Geometry updates only visit affected nodes and children. */
export class BrowserSemanticsState {
  readonly nodes = new Map<number, SemanticsNode>();
  readonly parents = new Map<number, number>();
  readonly identifiers = new Map<string, number>();
  private readonly dependents = new Map<string, Set<number>>();
  roots: number[] = [];
  private stream = 0;
  private revision = 0;
  private needsSnapshot = false;

  apply(packet: SemanticsPacket): SemanticsProjection | "stale" | "resync" | "waiting" {
    if (packet.schemaVersion !== 1 || !["full", "delta"].includes(packet.kind) ||
        !Number.isSafeInteger(packet.stream) || packet.stream <= 0 ||
        !Number.isSafeInteger(packet.revision) || packet.revision <= 0 ||
        !Array.isArray(packet.nodes) || !Array.isArray(packet.removed))
      throw new Error("Invalid Doroti semantics packet.");
    if (packet.stream < this.stream || (packet.stream === this.stream && packet.revision <= this.revision)) return "stale";
    if (packet.kind === "delta" && (this.needsSnapshot || packet.stream !== this.stream || packet.baseRevision !== this.revision))
      return this.requestSnapshot();
    if (packet.kind === "full" && !Array.isArray(packet.roots)) throw new Error("Missing semantics snapshot roots.");

    // Check compact patches before touching retained state. A missing baseline must recover atomically.
    const ids = new Set<number>();
    for (const patch of packet.nodes) {
      if (!Number.isSafeInteger(patch.id) || ids.has(patch.id)) throw new Error("Invalid semantics node ID.");
      ids.add(patch.id);
      if (patch.contentUnchanged === true && (packet.kind === "full" || !this.nodes.has(patch.id)))
        return this.requestSnapshot();
      if ((patch.contentUnchanged !== true || patch.rect !== undefined) &&
          (!Array.isArray(patch.rect) || patch.rect.length !== 4 || !patch.rect.every(Number.isFinite)))
        throw new Error("Invalid semantics rectangle.");
      if (patch.contentUnchanged !== true && !Array.isArray(patch.children))
        throw new Error("Missing semantics children.");
    }
    const removed = packet.kind === "full" ? [...this.nodes.keys()].filter(id => !ids.has(id)) : packet.removed;
    const plan: SemanticsProjection = { changed: new Set(), content: new Set(), parents: new Set(), removed, rootsChanged: packet.roots !== undefined };
    const relationships = new Set<string>();
    const topology = packet.kind === "full" || removed.length !== 0 || packet.nodes.some(patch => patch.children !== undefined);
    // Remove old edges together before installing replacements, including cross-parent moves.
    for (const id of [...removed, ...packet.nodes.filter(patch => patch.children !== undefined).map(patch => patch.id)]) {
      const old = this.nodes.get(id);
      for (const child of old?.children ?? []) {
        if (this.parents.get(child) === id) this.parents.delete(child);
        plan.changed.add(child);
      }
      plan.parents.add(id);
    }
    for (const id of removed) {
      const old = this.nodes.get(id);
      if (old) this.removeRelationships(old, relationships);
      this.nodes.delete(id);
      this.parents.delete(id);
    }
    for (const patch of packet.nodes) {
      const old = this.nodes.get(patch.id);
      const compact = patch.contentUnchanged === true;
      const { contentUnchanged: _, ...value } = patch;
      const node = (compact ? { ...old, ...value } : value) as SemanticsNode;
      if (!compact) {
        if (old) this.removeRelationships(old, relationships);
        this.addRelationships(node, relationships);
        plan.content.add(node.id);
      }
      this.nodes.set(node.id, node);
      plan.changed.add(node.id);
      // Coordinates are in view space; moving a parent changes its children's relative DOM positions.
      if (!old || old.rect[0] !== node.rect[0] || old.rect[1] !== node.rect[1])
        for (const child of node.children) plan.changed.add(child);
      if (patch.children !== undefined) for (const child of node.children) this.parents.set(child, node.id);
    }
    if (packet.roots !== undefined) this.roots = packet.roots;
    if (topology) {
      // Nodes can be moved into a newly added parent without receiving a content patch.
      for (const id of plan.changed) {
        const parent = this.parents.get(id);
        if (parent !== undefined) plan.parents.add(parent);
      }
    }
    for (const identifier of relationships) for (const id of this.dependents.get(identifier) ?? []) {
      plan.changed.add(id); plan.content.add(id);
    }
    this.stream = packet.stream;
    this.revision = packet.revision;
    this.needsSnapshot = false;
    return plan;
  }

  private requestSnapshot(): "resync" | "waiting" {
    if (this.needsSnapshot) return "waiting";
    this.needsSnapshot = true;
    return "resync";
  }

  private removeRelationships(node: SemanticsNode, changed: Set<string>): void {
    if (node.identifier && this.identifiers.get(node.identifier) === node.id) {
      this.identifiers.delete(node.identifier); changed.add(node.identifier);
    }
    for (const identifier of node.controlsNodes ?? []) {
      const nodes = this.dependents.get(identifier);
      nodes?.delete(node.id);
      if (!nodes?.size) this.dependents.delete(identifier);
    }
  }

  private addRelationships(node: SemanticsNode, changed: Set<string>): void {
    if (node.identifier) {
      this.identifiers.set(node.identifier, node.id); changed.add(node.identifier);
    }
    for (const identifier of node.controlsNodes ?? []) {
      let nodes = this.dependents.get(identifier);
      if (!nodes) this.dependents.set(identifier, nodes = new Set());
      nodes.add(node.id);
    }
  }
}
