export function formatRelativeTime(date: Date): string {
  const now = new Date()
  const diffMs = now.getTime() - date.getTime()
  const diffMins = Math.floor(diffMs / 60000)
  const diffHours = Math.floor(diffMs / 3600000)
  const diffDays = Math.floor(diffMs / 86400000)
  const diffMonths = Math.floor(diffDays / 30)
  const diffYears = Math.floor(diffDays / 365)

  if (diffMins < 1) return 'just now'
  if (diffMins < 60) return diffMins + ' min ago'
  if (diffHours < 24) return diffHours + ' hour' + (diffHours > 1 ? 's' : '') + ' ago'
  if (diffDays < 30) return diffDays + ' day' + (diffDays > 1 ? 's' : '') + ' ago'
  if (diffMonths < 12) return diffMonths + ' month' + (diffMonths > 1 ? 's' : '') + ' ago'
  return diffYears + ' year' + (diffYears > 1 ? 's' : '') + ' ago'
}

export function formatAbsoluteDate(date: Date): string {
  return date.toLocaleDateString(undefined, { month: 'short', day: 'numeric', year: 'numeric' })
}

export function convertUriToValidId(uri: string): string {
  return uri.replace(/[^A-Za-z0-9]/gi, '')
}
