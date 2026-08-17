export interface EchoPluginRequest {
  channel: string;
  codec: string;
  payloadBase64: string;
}

export function invoke(message: EchoPluginRequest): string {
  return message.payloadBase64;
}
