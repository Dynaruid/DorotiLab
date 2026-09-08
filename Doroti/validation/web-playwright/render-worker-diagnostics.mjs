// Runtime-owned idle pthreads may be parked in Atomics.wait. Only the render
// role services JS diagnostic exports; never await all pthread evaluations.
const owners = new WeakMap();
export async function findRenderWorker(page) {
  let owner = owners.get(page);
  if (!owner) {
    owner = await Promise.any(page.workers().map(async worker => {
      let timer;
      try {
        const ready = await Promise.race([
          worker.evaluate(() => typeof globalThis.__dorotiDirectDiagnostics === 'function'),
          new Promise((_, reject) => { timer = setTimeout(() => reject(Error('Idle pthread')), 3000); }),
        ]);
        if (!ready) throw Error('Not the render role');
        return worker;
      } finally { clearTimeout(timer); }
    }));
    owners.set(page, owner);
  }
  return owner;
}
export async function captureRenderWorker(page) {
  const owner = await findRenderWorker(page);
  const result = await owner.evaluate(() => globalThis.__dorotiDirectDiagnostics());
  if (!result?.managed) throw Error('Managed render diagnostics unavailable');
  return result;
}
