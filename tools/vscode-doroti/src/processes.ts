import { spawn, ChildProcess } from 'node:child_process';

export function start(file: string, args: string[], cwd: string, output: (text: string) => void,
    errorOutput: (text: string) => void = output): ChildProcess {
    const child = spawn(file, args, { cwd, shell: false, windowsHide: true, detached: process.platform !== 'win32', stdio: 'pipe' });
    child.stdout?.on('data', data => output(data.toString()));
    child.stderr?.on('data', data => errorOutput(data.toString()));
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
        const group = -child.pid;
        try { process.kill(group, 'SIGTERM'); }
        catch (error) { if ((error as NodeJS.ErrnoException).code === 'ESRCH') return; throw error; }
        // The launcher can close before its watcher/app finishes handling
        // SIGTERM. Wait for the whole owned group before allowing Restart.
        const deadline = Date.now() + 30000;
        while (Date.now() < deadline) {
            try { process.kill(group, 0); }
            catch (error) {
                const code = (error as NodeJS.ErrnoException).code;
                if (code === 'ESRCH') return;
                // A probe can report EPERM during group teardown; only ESRCH
                // confirms that no owned descendants remain.
                if (code !== 'EPERM') throw error;
            }
            await new Promise(resolve => setTimeout(resolve, 50));
        }
        throw new Error('Doroti did not finish stopping. Check the session logs.');
    }
}
export function completion(child: ChildProcess): Promise<void> {
    return new Promise((resolve, reject) => {
        child.once('error', reject);
        child.once('close', (code, signal) => code === 0 ? resolve() : reject(new Error(`Process exited ${code ?? signal}. See Doroti logs.`)));
    });
}
