/** Browser-only implementations for platform text actions. No clipboard probes:
 * a read remains exclusive to the user's Paste command in the input bridge. */
export class BrowserTextActions {
  private dialog: HTMLDialogElement | null = null;
  private disposed = false;
  private searchWindow: Window | null = null;
  private searchWindowTimer = 0;

  /** Called synchronously from a trusted toolbar tap, before Worker dispatch. */
  reserveSearchWindow(): void {
    if (this.disposed || this.searchWindow && !this.searchWindow.closed) return;
    this.releaseSearchWindow();
    const popup = window.open("about:blank", "_blank");
    if (!popup) return;
    popup.opener = null;
    this.searchWindow = popup;
    // A removed menu/custom callback must not leave an unused blank tab behind.
    this.searchWindowTimer = globalThis.setTimeout(() => this.releaseSearchWindow()?.close(), 10000);
  }

  private releaseSearchWindow(): Window | null {
    clearTimeout(this.searchWindowTimer);
    this.searchWindowTimer = 0;
    const popup = this.searchWindow;
    this.searchWindow = null;
    return popup;
  }

  async invoke(action: string, text: string): Promise<string> {
    if (this.disposed) return "cancelled";
    if (action !== "SearchWeb.invoke" && action !== "Share.invoke")
      throw new Error(`Unsupported text action '${action}'.`);
    if (!text.trim()) {
      if (action === "SearchWeb.invoke") this.releaseSearchWindow()?.close();
      return "empty";
    }
    if (action === "SearchWeb.invoke") {
      const url = new URL("https://www.google.com/search");
      url.searchParams.set("q", text);
      const reserved = this.releaseSearchWindow();
      if (reserved && !reserved.closed) {
        reserved.location.replace(url.href);
        return "opened";
      }
      const opened = window.open(url.href, "_blank");
      if (opened) { opened.opener = null; return "opened"; }
      this.showDialog(text, url.href);
      return "prompted";
    }
    if (typeof navigator.share === "function") {
      try { await navigator.share({ text }); return "shared"; }
      catch (error) {
        if (error instanceof DOMException && error.name === "AbortError") return "cancelled";
      }
    }
    this.showDialog(text);
    return this.disposed ? "cancelled" : "prompted";
  }

  dispose(): void {
    this.disposed = true;
    this.releaseSearchWindow()?.close();
    this.dialog?.remove();
    this.dialog = null;
  }

  private showDialog(text: string, searchUrl?: string): void {
    if (this.disposed) return;
    this.dialog?.remove();
    const dialog = document.createElement("dialog");
    this.dialog = dialog;
    dialog.className = "doroti-text-action-dialog";
    dialog.setAttribute("aria-label", searchUrl ? "Search web" : "Share text");
    const heading = document.createElement("h2");
    heading.textContent = searchUrl ? "Search web" : "Share text";
    const preview = document.createElement("p");
    preview.textContent = text;
    const status = document.createElement("p");
    status.setAttribute("role", "status");
    dialog.append(heading, preview);
    if (searchUrl) {
      // A Worker round trip can outlive popup activation. This link provides a
      // fresh, explicit user gesture without silently navigating the app away.
      const link = document.createElement("a");
      link.href = searchUrl;
      link.target = "_blank";
      link.rel = "noopener noreferrer";
      link.textContent = "Open web search";
      link.onclick = () => dialog.close();
      dialog.append(link);
    } else if (typeof navigator.share === "function" || typeof navigator.clipboard?.writeText === "function") {
      const share = document.createElement("button");
      share.textContent = typeof navigator.share === "function" ? "Share" : "Copy text";
      share.onclick = () => {
        // Do not await anything before invoking a gesture-restricted API.
        const result = typeof navigator.share === "function"
          ? navigator.share({ text }) : navigator.clipboard.writeText(text);
        void result.then(() => dialog.close(), (error: unknown) => {
          if (error instanceof DOMException && error.name === "AbortError") dialog.close();
          else status.textContent = "Unable to share. Try again.";
        });
      };
      dialog.append(share);
    } else {
      status.textContent = "Select the text above to copy it.";
    }
    const close = document.createElement("button");
    close.textContent = "Close";
    close.onclick = () => dialog.close();
    dialog.append(close, status);
    dialog.addEventListener("close", () => {
      dialog.remove();
      if (this.dialog === dialog) this.dialog = null;
    });
    document.body.append(dialog);
    dialog.showModal();
  }
}
