import { createServer, IncomingMessage, Server } from 'node:http';
import { randomBytes } from 'node:crypto';
import { Runtime } from './contracts';

/** Local editor acknowledgment bridge. Never carries executable code or metadata deltas. */
export class WebBridge {
    private readonly token = randomBytes(24).toString('hex');
    private readonly server: Server;
    private origin?: string;
    private receivedAt = 0;
    private latest?: Runtime;
    private pending?: { runtimeId: string; requestId: string; resolve: () => void; reject: (error: Error) => void; timer: NodeJS.Timeout };
    url = '';
    private constructor(readonly sessionId: string) {
        this.server = createServer((request, response) => { void this.handle(request, response).catch(() => {
            if (!response.headersSent) response.writeHead(400); response.end();
        }); });
    }
    static async start(sessionId: string): Promise<WebBridge> {
        const bridge = new WebBridge(sessionId);
        await new Promise<void>((resolve, reject) => { bridge.server.once('error', reject); bridge.server.listen(0, '127.0.0.1', resolve); });
        const address = bridge.server.address();
        if (!address || typeof address === 'string') throw new Error('Cannot bind Web development bridge.');
        bridge.url = `http://127.0.0.1:${address.port}/${bridge.token}`;
        return bridge;
    }
    browserUrl(value: string): string {
        const url = new URL(value);
        if (!['localhost', '127.0.0.1', '[::1]'].includes(url.hostname)) throw new Error('Web development expects a local runner URL.');
        this.origin = url.origin;
        url.searchParams.set('dorotiDevBridge', this.url);
        url.searchParams.set('dorotiDevSession', this.sessionId);
        return url.href;
    }
    get runtime(): Runtime | undefined {
        return this.latest && Date.now() - this.receivedAt < 5000 ? this.latest : undefined;
    }
    async prepare(runtimeId: string, requestId: string): Promise<void> {
        if (this.pending) throw new Error('A browser reload request is already pending.');
        if (this.runtime?.runtimeId !== runtimeId) throw new Error('Browser session is no longer connected.');
        await new Promise<void>((resolve, reject) => {
            const timer = setTimeout(() => { this.pending = undefined; reject(new Error('Browser did not accept the reload request. No files were saved.')); }, 5000);
            this.pending = { runtimeId, requestId, resolve, reject, timer };
        });
    }
    private async handle(request: IncomingMessage, response: import('node:http').ServerResponse): Promise<void> {
        if (!request.url?.startsWith(`/${this.token}/`) || !this.origin || request.headers.origin !== this.origin) {
            response.writeHead(403); response.end(); return;
        }
        response.setHeader('Access-Control-Allow-Origin', this.origin);
        response.setHeader('Vary', 'Origin');
        response.setHeader('Cache-Control', 'no-store');
        if (request.method === 'OPTIONS') {
            response.setHeader('Access-Control-Allow-Methods', 'GET, POST');
            response.setHeader('Access-Control-Allow-Headers', 'Content-Type');
            response.writeHead(204); response.end(); return;
        }
        const route = request.url.slice(this.token.length + 2);
        if (request.method === 'GET' && route === 'poll') {
            response.setHeader('Content-Type', 'application/json');
            response.end(JSON.stringify({ request: this.pending && { runtimeId: this.pending.runtimeId, requestId: this.pending.requestId } })); return;
        }
        if (request.method !== 'POST') { response.writeHead(405); response.end(); return; }
        const chunks: Buffer[] = []; let size = 0;
        for await (const chunk of request) {
            size += chunk.length; if (size > 65536) { response.writeHead(413); response.end(); return; } chunks.push(chunk);
        }
        const value = JSON.parse(Buffer.concat(chunks).toString('utf8'));
        if (route === 'status') {
            if (value.schemaVersion !== 'doroti.dev/v1' || value.sessionId !== this.sessionId ||
                typeof value.runtimeId !== 'string' || typeof value.supported !== 'boolean' ||
                !Number.isSafeInteger(value.revision) || !['ready', 'applying', 'applied', 'failed', 'closed'].includes(value.status)) {
                response.writeHead(400); response.end(); return;
            }
            this.latest = value; this.receivedAt = Date.now();
        } else if (route === 'prepared') {
            if (!this.pending || value.runtimeId !== this.pending.runtimeId || value.requestId !== this.pending.requestId) {
                response.writeHead(409); response.end(); return;
            }
            const pending = this.pending; this.pending = undefined;
            clearTimeout(pending.timer); pending.resolve();
        } else { response.writeHead(404); response.end(); return; }
        response.writeHead(204); response.end();
    }
    async close(): Promise<void> {
        if (this.pending) { clearTimeout(this.pending.timer); this.pending.reject(new Error('Development session stopped.')); this.pending = undefined; }
        this.latest = undefined; this.origin = undefined;
        this.server.closeAllConnections();
        await new Promise<void>(resolve => this.server.close(() => resolve()));
    }
}
