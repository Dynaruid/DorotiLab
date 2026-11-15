export async function installControlledPicker(context) {
  await context.addInitScript(() => {
    const click = HTMLInputElement.prototype.click;
    HTMLInputElement.prototype.click = function(...args) {
      if (this.type !== "file") return click.apply(this, args);
    };
  });
}

export async function selectFiles(page, activate, files, controlled) {
  if (controlled) {
    await activate();
    const input = page.locator('input[type="file"]');
    await input.waitFor({ state: "attached" });
    if (Array.isArray(files) && files.length === 0) await input.dispatchEvent("cancel");
    else await input.setInputFiles(files);
    return;
  }
  const chooser = page.waitForEvent("filechooser", { timeout: 15000 });
  await activate();
  await (await chooser).setFiles(files);
}
