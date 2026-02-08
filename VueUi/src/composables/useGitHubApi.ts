import { ref, computed, watch, type Ref } from 'vue'
import type {
  Repository,
  RepoLinkResults,
  LinkStatus,
  SortKey,
  SortDirection,
  LinkTab,
} from '@/types/github'

const LAST_SEARCH_KEY = 'dlf:lastSearch'

function saveLastSearch(value: string) {
  try {
    if (value) localStorage.setItem(LAST_SEARCH_KEY, value)
  } catch {
    /* ignore */
  }
}

function getLastSearch(): string | null {
  try {
    return localStorage.getItem(LAST_SEARCH_KEY)
  } catch {
    return null
  }
}

function clearLastSearch() {
  try {
    localStorage.removeItem(LAST_SEARCH_KEY)
  } catch {
    /* ignore */
  }
}

export function useGitHubApi() {
  // State
  const allRepos = ref<Repository[]>([])
  const currentUserOrOrg = ref('')
  const loading = ref(false)
  const error = ref<string | null>(null)
  const filterTerm = ref('')
  const visibleCount = ref(5)
  const sortBy = ref<SortKey>('pushed')
  const sortDir = ref<SortDirection>('desc')
  const repoLinkResults = ref<Record<string, RepoLinkResults>>({})
  const checkingRepos = ref<Set<string>>(new Set())
  const selectedTabs = ref<Record<string, LinkTab>>({})

  // Computed: filtered and sorted repos
  const filteredRepos = computed(() => {
    let filtered = [...allRepos.value]

    if (filterTerm.value) {
      const escaped = filterTerm.value
        .replace(/[-/\\^$+?.()|[\]{}]/g, '\\$&')
        .replace(/\*/g, '.*')
      const regex = new RegExp(escaped, 'i')
      filtered = filtered.filter((repo) => regex.test(repo.name))
    }

    filtered.sort((a, b) => {
      if (sortBy.value === 'name') {
        const nameA = (a.name || '').toLowerCase()
        const nameB = (b.name || '').toLowerCase()
        return sortDir.value === 'asc'
          ? nameA.localeCompare(nameB)
          : nameB.localeCompare(nameA)
      }
      if (sortBy.value === 'stars') {
        const sa = a.stars || 0
        const sb = b.stars || 0
        return sortDir.value === 'asc' ? sa - sb : sb - sa
      }
      // pushed/updated
      const da = a.updatedAt ? new Date(a.updatedAt).getTime() : 0
      const db = b.updatedAt ? new Date(b.updatedAt).getTime() : 0
      return sortDir.value === 'asc' ? da - db : db - da
    })

    return filtered
  })

  const visibleRepos = computed(() => filteredRepos.value.slice(0, visibleCount.value))

  const totalCount = computed(() => filteredRepos.value.length)
  const showingCount = computed(() => Math.min(visibleCount.value, totalCount.value))
  const remainingCount = computed(() => Math.max(totalCount.value - showingCount.value, 0))
  const hasMore = computed(() => remainingCount.value > 0)
  const hasResults = computed(() => allRepos.value.length > 0)

  // Actions
  async function searchUser(userOrOrg: string) {
    if (!userOrOrg.trim()) {
      error.value = 'Please enter a GitHub username or organization'
      return
    }

    currentUserOrOrg.value = userOrOrg
    loading.value = true
    error.value = null
    allRepos.value = []
    repoLinkResults.value = {}
    checkingRepos.value = new Set()
    selectedTabs.value = {}
    filterTerm.value = ''
    visibleCount.value = 5
    sortBy.value = 'pushed'
    sortDir.value = 'desc'
    clearLastSearch()

    try {
      const response = await fetch(
        `/Home/GetUserRepos?userOrOrg=${encodeURIComponent(userOrOrg)}&includeForks=false&maxRepos=999`,
      )

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`)
      }

      const data = await response.json()

      if (data && typeof data === 'object' && 'error' in data) {
        error.value = data.error
        loading.value = false
        return
      }

      allRepos.value = data as Repository[]
      saveLastSearch(userOrOrg)
    } catch (err) {
      error.value =
        err instanceof Error
          ? err.message
          : 'Failed to fetch repositories. Please check the username/organization name.'
    } finally {
      loading.value = false
    }
  }

  async function checkRepo(repo: Repository, force = false) {
    const repoId = repo.url.replace(/[^A-Za-z0-9]/gi, '')
    if (checkingRepos.value.has(repoId)) return
    if (!force && repoLinkResults.value[repoId]) return
    // If retrying, clear previous results
    if (force && repoLinkResults.value[repoId]) {
      const newResults = { ...repoLinkResults.value }
      delete newResults[repoId]
      repoLinkResults.value = newResults
    }

    checkingRepos.value = new Set([...checkingRepos.value, repoId])
    if (!selectedTabs.value[repoId]) {
      selectedTabs.value[repoId] = 'broken'
    }

    try {
      const response = await fetch(
        `/Home/CheckRepo?projectBaseUrl=${encodeURIComponent(repo.url)}&branch=${encodeURIComponent(repo.branch)}`,
      )

      if (!response.ok) {
        throw new Error(`HTTP ${response.status}`)
      }

      const data: LinkStatus[] = await response.json()

      const okLinks: LinkStatus[] = []
      const warningLinks: LinkStatus[] = []
      const brokenLinks: LinkStatus[] = []

      for (const link of data) {
        if (link.httpStatusCode === 200) {
          okLinks.push(link)
        } else if (link.httpStatusCode === 404) {
          brokenLinks.push(link)
        } else {
          warningLinks.push(link)
        }
      }

      repoLinkResults.value = {
        ...repoLinkResults.value,
        [repoId]: { ok: okLinks, warning: warningLinks, broken: brokenLinks },
      }
    } catch (err) {
      // Mark as checked with error state
      repoLinkResults.value = {
        ...repoLinkResults.value,
        [repoId]: {
          ok: [],
          warning: [],
          broken: [],
          error: err instanceof Error ? err.message : 'Failed to check links',
        },
      }
    } finally {
      const newSet = new Set(checkingRepos.value)
      newSet.delete(repoId)
      checkingRepos.value = newSet
    }
  }

  function loadMore() {
    visibleCount.value += 5
  }

  function setFilter(term: string) {
    filterTerm.value = term
    visibleCount.value = 5
  }

  function setSort(key: SortKey) {
    sortBy.value = key
    visibleCount.value = 5
  }

  function setSortDir(dir: SortDirection) {
    sortDir.value = dir
    visibleCount.value = 5
  }

  function setSelectedTab(repoId: string, tab: LinkTab) {
    selectedTabs.value = { ...selectedTabs.value, [repoId]: tab }
  }

  function isRepoChecking(repoId: string): boolean {
    return checkingRepos.value.has(repoId)
  }

  function getRepoLinks(repoId: string): RepoLinkResults | undefined {
    return repoLinkResults.value[repoId]
  }

  function getSelectedTab(repoId: string): LinkTab {
    return selectedTabs.value[repoId] || 'broken'
  }

  function resetSearch() {
    allRepos.value = []
    currentUserOrOrg.value = ''
    loading.value = false
    error.value = null
    filterTerm.value = ''
    visibleCount.value = 5
    sortBy.value = 'pushed'
    sortDir.value = 'desc'
    repoLinkResults.value = {}
    checkingRepos.value = new Set()
    selectedTabs.value = {}
    clearLastSearch()
  }

  function getLastSearchValue(): string | null {
    return getLastSearch()
  }

  return {
    // State
    allRepos,
    currentUserOrOrg,
    loading,
    error,
    filterTerm,
    visibleCount,
    sortBy,
    sortDir,
    repoLinkResults,
    checkingRepos,
    selectedTabs,

    // Computed
    filteredRepos,
    visibleRepos,
    totalCount,
    showingCount,
    remainingCount,
    hasMore,
    hasResults,

    // Actions
    searchUser,
    checkRepo,
    loadMore,
    setFilter,
    setSort,
    setSortDir,
    setSelectedTab,
    isRepoChecking,
    getRepoLinks,
    getSelectedTab,
    resetSearch,
    getLastSearchValue,
  }
}
