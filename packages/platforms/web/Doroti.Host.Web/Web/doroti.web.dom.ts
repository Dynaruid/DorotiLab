export interface DorotiDomEndpoints {
  readonly root: HTMLElement;
  readonly canvas: HTMLCanvasElement;
  readonly input: HTMLTextAreaElement;
  readonly semantics: HTMLElement;
}

export function createDorotiDomEndpoints(app: HTMLElement): DorotiDomEndpoints {
  const root = document.createElement("main");
  root.className = "doroti-root";
  root.dataset.dorotiHost = "worker-offscreen-canvas";
  const canvas = document.createElement("canvas");
  canvas.id = "doroti-surface";
  canvas.tabIndex = 0;
  canvas.setAttribute("aria-label", "Doroti GPU surface");
  const input = document.createElement("textarea");
  input.id = "doroti-ime";
  input.className = "doroti-ime";
  input.setAttribute("aria-hidden", "true");
  input.hidden = true;
  input.tabIndex = -1;
  const semantics = document.createElement("div");
  semantics.id = "doroti-semantics";
  semantics.className = "doroti-semantics";
  semantics.setAttribute("role", "application");
  semantics.setAttribute("aria-label", "Doroti application");
  root.append(canvas, input, semantics);
  app.replaceChildren(root);
  return { root, canvas, input, semantics };
}

export function createReplacementCanvas(previous: HTMLCanvasElement): HTMLCanvasElement {
  const canvas = document.createElement("canvas");
  canvas.id = previous.id;
  canvas.tabIndex = 0;
  canvas.setAttribute("aria-label", previous.getAttribute("aria-label") ?? "Doroti GPU surface");
  previous.replaceWith(canvas);
  return canvas;
}
