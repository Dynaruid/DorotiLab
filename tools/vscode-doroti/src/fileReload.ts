import * as fs from 'node:fs/promises';
import * as path from 'node:path';

export interface ReloadRequest { schemaVersion: 'doroti.dev/v1'; sessionId: string; runtimeId: string; requestId: string; }

/** Publish a complete request before a device relay allows the editor to save. */
export async function prepareFileReload(directory: string, request: ReloadRequest, requiresAcknowledgment: boolean,
    active: () => boolean, timeout = 15000): Promise<boolean> {
    if (!active()) return false;
    const temporary = path.join(directory, `request.${request.requestId}.tmp`);
    try {
        await fs.writeFile(temporary, JSON.stringify(request));
        await fs.rename(temporary, path.join(directory, 'request.json'));
    } finally { await fs.rm(temporary, { force: true }); }
    const deadline = Date.now() + timeout;
    while (active()) {
        if (!requiresAcknowledgment) return true;
        let prepared: ReloadRequest | undefined;
        try { prepared = JSON.parse(await fs.readFile(path.join(directory, 'prepared.json'), 'utf8')); }
        catch (error) { if ((error as NodeJS.ErrnoException).code !== 'ENOENT' && !(error instanceof SyntaxError)) throw error; }
        if (prepared?.schemaVersion === request.schemaVersion && prepared.sessionId === request.sessionId &&
            prepared.runtimeId === request.runtimeId && prepared.requestId === request.requestId) return active();
        if (Date.now() >= deadline) throw new Error('Runtime did not accept the reload request. Edits have not been saved.');
        await new Promise(resolve => setTimeout(resolve, 50));
    }
    return false;
}
