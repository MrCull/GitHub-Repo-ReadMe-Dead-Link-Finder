export const API_CONFIG = {
  baseUrl: '' // Empty string for relative paths
} as const

export const getApiUrl = (path: string) => {
  return `${API_CONFIG.baseUrl}${path}`
} 