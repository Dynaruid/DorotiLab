import { spawn, ChildProcess } from 'node:child_process';

export function start(file: string, args: string[], cwd: string, output: (text: string) => void): ChildProcess {
    const child = spawn(file, args, { cwd, shell: false, windowsHide: true, detached: process.platform !== 'win32', stdio: 'pipe' });
    child.stdout?.on('data', data => output(data.toString()));
    child.stderr?.on('data', data => output(data.toString()));
    return child;
}
export async function stop(child: ChildProcess): Promise<void> {
    if (!child.pid || child.exitCode !== null || child.signalCode !== null) return;
    if (process.platform === 'win32') {
        await new Promise<void>((resolve, reject) => {
            const killer = spawn('taskkill.exe', ['/PID', String(child.pid), '/T', '/F'], { shell: false, windowsHide: true });
            killer.once('error', reject);
            killer.once('close', code => {
                if (code !== 0 && child.exitCode === null && child.signalCode === null)
                    reject(new Error(`Unable to stop Doroti process tree ${child.pid} (taskkill exit ${code}).`));
                else resolve();
            });
        });
    } else {
        await new Promise<void>((resolve, reject) => {
            // iOS also owns a simulator app outside the process group. Let its
            // launcher finish cleanup before a Restart launches the same bundle.
            const timer = setTimeout(() => reject(new Error('Doroti did not finish stopping. Check the session logs.')), 30000);
            child.once('close', () => { clearTimeout(timer); resolve(); });
            try { process.kill(-child.pid!, 'SIGTERM'); }
            catch (error) {
                clearTimeout(timer);
                if ((error as NodeJS.ErrnoException).code === 'ESRCH') resolve(); else reject(error);
            }
        });
    }
}
export function completion(child: ChildProcess): Promise<void> {
    return new Promise((resolve, reject) => {
        child.once('error', reject);
        child.once('close', (code, signal) => code === 0 ? resolve() : reject(new Error(`Process exited ${code ?? signal}. See Doroti logs.`)));
    });
}
