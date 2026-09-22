// The same inputs are used on each backend/product. No retries or forced GC.
export async function runMemoryWorkload({ segment, wheel, clickLabel, wait, dom, save, shot, evaluate, resize, width, profile }) {
  const started = Date.now();
  for (let round = 0; round < 3 || (profile.startsWith('memory-soak') && Date.now() - started < 600000); round++) {
    await segment(`visit-${round}`, async () => {
      // Narrow view has one column. Wide view visits both owners independently.
      for (const x of width > 1000 ? [width * .3, width * .75] : [width * .5]) {
        for (let i = 0; i < 28; i++) await wheel(420, x, 650);
        await save(`bottom-${round}-${x}`, await dom());
        await shot(`bottom-${round}-${x}`);
        for (let i = 0; i < 28; i++) await wheel(-420, x, 650);
      }
      await clickLabel('Toggle brightness');
      await clickLabel('Toggle brightness');
      await clickLabel('Color');
      await clickLabel('Components');
      await wait(1500);
    });
  }
  await segment('landscape', async () => { await resize(900, 390); await wait(3500); });
  await segment('portrait', async () => { await resize(width, 900); await wait(3500); });
  await segment('short-viewport', async () => { await resize(width, 480); await wait(3500); });
  await segment('restored', async () => { await resize(width, 900); await wait(5500); });
  await save('final-dom', await dom());
  await save('final-state', await evaluate(`({visibility:document.visibilityState,focused:document.hasFocus(),policy:{...document.documentElement.dataset}})`));
}
