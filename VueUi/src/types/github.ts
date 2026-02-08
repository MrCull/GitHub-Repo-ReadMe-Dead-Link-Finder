export interface Repository {
  name: string
  fullName: string
  url: string
  branch: string
  stars: number
  watchers: number
  forks: number
  updatedAt: string | null
  description: string
  language: string
  license: string
  topics: string[]
}

export interface ApiError {
  error: string
  exceptionType?: string
}

export interface LinkStatus {
  uriText: string
  httpStatusCode: number
  httpStatusCodeText: string
}

export interface RepoLinkResults {
  ok: LinkStatus[]
  warning: LinkStatus[]
  broken: LinkStatus[]
  error?: string
}

export type SortKey = 'pushed' | 'name' | 'stars'
export type SortDirection = 'asc' | 'desc'
export type LinkTab = 'broken' | 'warning' | 'ok'
