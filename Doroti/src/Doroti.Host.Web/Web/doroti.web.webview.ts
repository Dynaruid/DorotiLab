import type { NativeIdentity, NativeResource } from "./doroti.web.platform-views.js";

interface Options {
  Html?: string; Profile?: number; AllowedOrigins?: string[]; MessageOrigins?: string[];
  Resources?: Record<string, { ResourceKey: string; MimeType: string }>;
}
export class WebViewFailure extends Error {
  constructor(readonly code: number, message: string) { super(message); }
}
const fail = (code: number, message: string): never => { throw new WebViewFailure(code, message); };

/** The browser's existing profile is explicit. An iframe cannot create or erase a private profile. */
export class BrowserWebView implements NativeResource {
  readonly element: HTMLIFrameElement;
  #closed = false;
  #navigation = 0;
  #document = 0;
  #loading = false;
  #url = "about:srcdoc";
  #lastMessage = 0;
  #nonce = crypto.randomUUID();
  #pending = 0;
  #focusedDocument?: Document;
  #focusTimer = 0;
  readonly #cancelPending = new Set<(error: WebViewFailure) => void>();
  constructor(readonly identity: NativeIdentity, readonly options: Options,
    readonly changed: (event: Record<string, unknown>) => void, document: Document) {
    if (options.Profile !== 2) fail(2, "iframes require BrowserDefault; ephemeral and named/shared native profiles are unavailable.");
    if (Object.keys(options.Resources ?? {}).length) fail(2, "Use a same-origin deployed URL for browser app content; native resource schemes are unavailable.");
    this.element = document.createElement("iframe");
    this.element.title = "Web content";
    this.element.style.border = "0";
    // Same-origin content is trusted app code, not an isolation/security boundary.
    this.element.sandbox.add("allow-scripts", "allow-same-origin", "allow-forms");
    this.element.referrerPolicy = "no-referrer";
    this.element.addEventListener("load", this.#loaded);
    document.defaultView!.addEventListener("message", this.#message);
    document.defaultView!.addEventListener("blur", this.#windowBlur);
    this.#html(options.Html ?? "<!doctype html><title>WebView</title>");
  }
  #notify(kind: number, extra: Record<string, unknown> = {}): void {
    if (!this.#closed) this.changed({ identity: this.identity, NavigationId: this.#navigation,
      DocumentGeneration: this.#document, Kind: kind, Url: this.#url, ...extra });
  }
  #begin(url: string): void {
    this.#cancel(new WebViewFailure(4, "Document changed."));
    this.#navigation++; this.#document++; this.#loading = true; this.#url = url;
    this.#nonce = crypto.randomUUID(); this.#lastMessage = 0; this.#notify(0);
  }
  #html(html: string): void {
    if (new TextEncoder().encode(html).length > 2 * 1024 * 1024) fail(3, "HTML exceeds 2 MiB.");
    this.#begin("about:srcdoc");
    this.element.srcdoc = html;
  }
  #sameOrigin(): Window | null {
    try {
      const window = this.element.contentWindow;
      // Access to document is the browser's same-origin check. srcdoc/about:blank
      // inherit an origin even though location.origin serializes as "null".
      if (!window?.document) return null;
      return window;
    } catch { return null; }
  }
  #loaded = (): void => {
    if (this.#closed) return;
    const window = this.#sameOrigin();
    this.#cancel(new WebViewFailure(4, "Document loaded."));
    this.#focusedDocument?.removeEventListener("focusin", this.#focused);
    this.#focusedDocument = window?.document;
    this.#focusedDocument?.addEventListener("focusin", this.#focused);
    if (window) this.#url = window.location.href;
    // Every load invalidates prior document work, including link-driven navigation.
    this.#document++; this.#loading = false; this.#lastMessage = 0;
    this.#nonce = crypto.randomUUID();
    this.#notify(1);
    const origin = window ? location.origin : new URL(this.#url).origin;
    if (this.options.MessageOrigins?.includes(origin)) {
      this.element.contentWindow?.postMessage({ type: "doroti-webview-init", version: 1,
        documentGeneration: this.#document, nonce: this.#nonce }, origin);
    }
    // iframe load does not disclose HTTP success or first content presentation.
    this.#notify(2);
  };
  #focused = (): void => { if (!this.#closed) this.changed({ identity: this.identity, focused: true }); };
  #windowBlur = (): void => {
    clearTimeout(this.#focusTimer);
    this.#focusTimer = globalThis.setTimeout(() => {
      this.#focusTimer = 0;
      if (this.element.ownerDocument.activeElement === this.element) this.#focused();
    }, 0);
  };
  #cancel(error: WebViewFailure): void { for (const reject of this.#cancelPending) reject(error); this.#cancelPending.clear(); }
  #message = (event: MessageEvent): void => {
    if (this.#closed || this.#loading || event.source !== this.element.contentWindow ||
      !this.options.MessageOrigins?.includes(event.origin)) return;
    const data = event.data;
    if (!data || data.version !== 1 || data.type !== "doroti-webview-message" || data.nonce !== this.#nonce ||
      data.documentGeneration !== this.#document || !Number.isSafeInteger(data.requestId) || data.requestId <= this.#lastMessage ||
      typeof data.name !== "string" || !data.name || data.name.length > 128 || typeof data.json !== "string" ||
      new TextEncoder().encode(data.json).length > 65536) return;
    try { JSON.parse(data.json); } catch { return; }
    this.#lastMessage = data.requestId;
    this.#notify(5, { MessageName: data.name, MessageJson: data.json, MessageRequestId: data.requestId });
  };
  async execute(command: { Operation: number; Text?: string; DocumentGeneration?: number }): Promise<Record<string, unknown>> {
    if (this.#closed) fail(1, "WebView is closed.");
    const state = (): Record<string, unknown> => ({ RequestId: 0, NavigationId: this.#navigation,
      DocumentGeneration: this.#document, Url: this.#url, Title: this.#sameOrigin()?.document.title ?? null,
      IsLoading: this.#loading, CanGoBack: false, CanGoForward: false,
      HistoryKnown: false, NavigationStateKnown: false });
    switch (command.Operation) {
      case 0: return { ...state(), Features: { Navigation: true, JavaScript: !!this.#sameOrigin(),
        EphemeralProfile: false, SharedPersistentProfile: false, ClearAllData: false,
        ScriptMessages: !!this.options.MessageOrigins?.length, AppContentScheme: false, FirstContentFrame: false, BrowserDefaultProfile: true } };
      case 1: return state();
      case 2: {
        let url: URL;
        try { url = new URL(command.Text ?? ""); } catch { return fail(3, "Navigation requires an absolute HTTP(S) URL."); }
        if (!["https:", "http:"].includes(url.protocol) || url.username || url.password) fail(3, "Only HTTP(S) navigation is allowed.");
        if (this.options.AllowedOrigins?.length && !this.options.AllowedOrigins.includes(url.origin)) fail(3, "Navigation origin was not allowed.");
        this.#begin(url.href); this.element.removeAttribute("srcdoc"); this.element.src = url.href; break;
      }
      case 3: this.#html(command.Text ?? ""); break;
      case 4: {
        const window = this.#sameOrigin();
        if (!window) fail(2, "Cross-origin reload is not observable; navigate explicitly.");
        this.#begin(this.#url); window!.location.reload(); break;
      }
      case 5: {
        const window = this.#sameOrigin(); if (!window) fail(2, "Cross-origin stop is unavailable.");
        window!.stop(); this.#loading = false; this.#document++; break;
      }
      case 6: case 7: fail(2, "iframe history is shared with the browser and cannot be queried safely.");
      case 8: {
        const window = this.#sameOrigin();
        if (!window || this.#loading) fail(2, "JavaScript requires a loaded same-origin document.");
        if (command.DocumentGeneration && command.DocumentGeneration !== this.#document) fail(4, "Document changed.");
        if (++this.#pending > 32) { this.#pending--; fail(7, "JavaScript queue is full."); }
        const generation = this.#document;
        let cancel!: (error: WebViewFailure) => void;
        const interrupted = new Promise<never>((_, reject) => { cancel = reject; this.#cancelPending.add(reject); });
        const timer = globalThis.setTimeout(() => cancel(new WebViewFailure(5, "JavaScript exceeded 30 seconds.")), 30000);
        try {
          const value: unknown = await Promise.race([(window as Window & { eval(code: string): unknown }).eval(command.Text ?? ""), interrupted]);
          if (this.#closed) fail(1, "WebView closed during JavaScript.");
          if (generation !== this.#document || window !== this.#sameOrigin()) fail(4, "Document changed during JavaScript.");
          const json = value === undefined ? null : JSON.stringify(value);
          if (value !== undefined && json === undefined) fail(5, "JavaScript result is not JSON serializable.");
          if (json && new TextEncoder().encode(json).length > 2 * 1024 * 1024) fail(3, "JavaScript result exceeds 2 MiB.");
          return { ...state(), Json: json, IsUndefined: value === undefined };
        } catch (error) { if (error instanceof WebViewFailure) throw error; fail(5, String(error)); }
        finally { globalThis.clearTimeout(timer); this.#cancelPending.delete(cancel); this.#pending--; }
      }
      case 9: fail(2, "A browser iframe cannot clear the user's shared browser profile.");
      default: fail(3, "Unknown WebView operation.");
    }
    return state();
  }
  dispose(): void {
    if (this.#closed) return;
    this.#closed = true; this.#document++;
    this.#cancel(new WebViewFailure(1, "WebView closed."));
    this.#focusedDocument?.removeEventListener("focusin", this.#focused);
    this.element.removeEventListener("load", this.#loaded);
    this.element.ownerDocument.defaultView!.removeEventListener("message", this.#message);
    this.element.ownerDocument.defaultView!.removeEventListener("blur", this.#windowBlur);
    clearTimeout(this.#focusTimer);
    this.element.remove();
    this.element.removeAttribute("srcdoc"); this.element.src = "about:blank";
  }
}
