export interface ParsedApiError {
  status?: number;
  messages: string[];
  raw?: unknown;
}