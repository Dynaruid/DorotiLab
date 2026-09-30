import * as path from 'node:path';

export interface Project { schemaVersion: string; manifest: string; root: string; applicationProject: string; platforms: Record<string, string>; developmentTargets: string[]; }
export interface Runtime { schemaVersion: string; sessionId: string; runtimeId: string; processId: number; supported: boolean; status: string; requestId?: string; revision: number; error?: string; }
export function parseProject(text: string): Project {
    const value = JSON.parse(text) as Project;
    if (value.schemaVersion !== 'doroti.cli-workspace/v1' || !path.isAbsolute(value.root) ||
        !Array.isArray(value.developmentTargets) || !value.platforms || !value.applicationProject)
        throw new Error('Invalid response from Doroti CLI describe. Update the repository CLI.');
    for (const target of value.developmentTargets)
        if (!['windows', 'web', 'ios', 'macos', 'maccatalyst'].includes(target) || !value.platforms[target]) throw new Error('Invalid development target.');
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
