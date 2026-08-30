export function createDorotiWorker(url: URL): Worker {
  return new Worker(url, { type: "module" });
}

export function closeExternalLeases<T>(
  leases: Map<number, T>,
  close: (requestId: number, lease: T) => void,
): void {
  for (const [requestId, lease] of leases) close(requestId, lease);
  leases.clear();
}
