import * as path from 'node:path';

export interface Development { transport: string; debug: boolean; requiresPreparedAcknowledgment: boolean; usesStopSignal: boolean; launchBrowser: boolean; }
export interface DevelopmentSupport { platform: string; runner: string; provider: string; version: string; operations: string[]; development: Development; }
export interface Project { schemaVersion: string; manifest: string; root: string; applicationProject: string; platforms: Record<string, string>; developmentTargets: string[]; developmentSupport: DevelopmentSupport[]; }
export interface Runtime { schemaVersion: string; sessionId: string; runtimeId: string; processId: number; supported: boolean; status: string; requestId?: string; revision: number; error?: string; }
export function parseProject(text: string): Project {
    const value = JSON.parse(text) as Project;
    if (value.schemaVersion !== 'doroti.cli-workspace/v2' || !path.isAbsolute(value.root) ||
        !Array.isArray(value.developmentTargets) || !value.platforms || !value.applicationProject)
        throw new Error('Invalid response from Doroti CLI describe. Update the repository CLI.');
    if (!Array.isArray(value.developmentSupport)) throw new Error('Missing provider development capabilities.');
    const seen = new Set<string>();
    for (const [alias, runner] of Object.entries(value.platforms)) {
        if (!/^[A-Za-z][A-Za-z0-9_-]{0,63}$/.test(alias) || !path.isAbsolute(runner)) throw new Error('Invalid declared platform.');
    }
    for (const support of value.developmentSupport) {
        if (seen.has(support.platform) || !value.platforms[support.platform] || !Array.isArray(support.operations) || !support.development ||
            !['none', 'file', 'browser', 'device'].includes(support.development.transport)) throw new Error('Invalid provider development capabilities.');
        seen.add(support.platform);
    }
    for (const target of value.developmentTargets)
        if (!value.platforms[target] || !value.developmentSupport.some(support => support.platform === target && support.operations.includes('dev'))) throw new Error('Invalid development target.');
    return value;
}
export function validateName(name: string): string | undefined {
    const keywords = new Set('abstract as base bool break byte case catch char checked class const continue decimal default delegate do double else enum event explicit extern false finally fixed float for foreach goto if implicit in int interface internal is lock long namespace new null object operator out override params private protected public readonly ref return sbyte sealed short sizeof stackalloc static string struct switch this throw true try typeof uint ulong unchecked unsafe ushort using virtual void volatile while'.split(' '));
    if (!name.split('.').every(part => /^[\p{L}_][\p{L}\p{N}_]*$/u.test(part) && !keywords.has(part)) ||
        /^(con|prn|aux|nul|com[0-9]|lpt[0-9])(?:\.|$)/i.test(name))
        return 'Use a C# project name (letters, digits, underscores or dots); no path separators or reserved device names.';
    return undefined;
}
export function classifyOutput(text: string): 'compile-error' | 'restart-required' | undefined {
    if (/rude edit|error ENC\d+|restart.*(?:required|needed)|Do you want to restart|Further changes won.t be applied/i.test(text)) return 'restart-required';
    if (/error CS\d+|Build failed|Failed to build project|Unable to apply hot reload due to compilation/i.test(text)) return 'compile-error';
    return undefined;
}
/** The watcher stays alive after its app exits. Only its idle-after-exit sequence ends an IDE session. */
export function watcherExited(text: string): boolean {
    const plain = text.replace(/\x1b\[[0-?]*[ -/]*[@-~]/g, '');
    const prefix = '^\\s*dotnet watch\\s+(?:[^\\w\\[\\r\\n]+\\s*)?(?:\\[[^\\r\\n]+\\]\\s*)?';
    const exits = [...plain.matchAll(new RegExp(prefix + 'Exited(?: with error code -?\\d+)?\\s*$', 'gm'))];
    const last = exits.at(-1);
    if (!last) return false;
    const afterExit = plain.slice(last.index! + last[0].length);
    // Rebuild/relaunch output invalidates an earlier exit. Compilation failures
    // and SDK-driven restarts must keep the watcher available for the next edit.
    if (new RegExp(prefix + '(?:Building|Build failed|Failed to build project|Restarting|Started|Launched|File changed|File change detected)\\b', 'mi').test(afterExit)) return false;
    return new RegExp(prefix + 'Waiting for a file to change before restarting[^\\r\\n]*[\\r\\n]', 'm').test(afterExit);
}
