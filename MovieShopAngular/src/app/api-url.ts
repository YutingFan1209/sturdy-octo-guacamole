declare global {
  interface Window {
    __MOVIESHOP_CONFIG__?: {
      apiBaseUrl?: string;
    };
  }
}

export function apiUrl(path: string): string {
  const configuredBaseUrl = typeof window === 'undefined'
    ? ''
    : window.__MOVIESHOP_CONFIG__?.apiBaseUrl ?? '';
  const baseUrl = configuredBaseUrl.replace(/\/$/, '');
  const normalizedPath = path.startsWith('/') ? path : `/${path}`;
  return `${baseUrl}${normalizedPath}`;
}
