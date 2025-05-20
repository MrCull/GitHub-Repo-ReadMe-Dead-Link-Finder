<script setup lang="ts">
import { ref, computed } from 'vue'
import { Octokit } from '@octokit/rest'
import SearchForm from './search/SearchForm.vue'
import RepoCard from './repo/RepoCard.vue'
import LinkList from './links/LinkList.vue'
import ThemeToggle from './common/ThemeToggle.vue'
import { getApiUrl } from '@/config/api'

interface RepoInfo {
  name: string
  stars: number
  forks: number
  lastUpdated: string
  defaultBranch: string
  linkStats: {
    bad: number
    warning: number
    ok: number
  }
  links: LinkInfo[]
  isFavorite?: boolean
}

interface LinkInfo {
  url: string
  statusCode: number
  statusText: string
  type: 'bad' | 'warning' | 'ok'
}

interface ApiLinkResponse {
  uriText: string
  httpStatusCode: number
  httpStatusCodeText: string
}

type LinkFilter = 'all' | 'bad' | 'warning' | 'ok'

const isLoading = ref(false)
const error = ref('')
const repos = ref<RepoInfo[]>([])
const selectedRepo = ref<RepoInfo | null>(null)
const linkFilter = ref<LinkFilter>('all')
const repoFilter = ref('')
const checkedRepos = new Set<string>()
const currentUsername = ref('')
const loadingRepos = new Set<string>()

const filteredRepos = computed(() => {
  let filtered = repos.value
  if (repoFilter.value) {
    const filter = repoFilter.value.toLowerCase()
    filtered = repos.value.filter(repo => repo.name.toLowerCase().startsWith(filter))
  }
  
  // Only show first 3 repos
  const displayRepos = filtered.slice(0, 3)
  
  // Auto-select first repo if it's different from current selection
  if (displayRepos.length > 0 && (!selectedRepo.value || !displayRepos.includes(selectedRepo.value))) {
    selectedRepo.value = displayRepos[0]
  }
  
  // Check links for newly displayed repos
  displayRepos.forEach(repo => {
    if (!checkedRepos.has(repo.name)) {
      loadingRepos.add(repo.name)
      checkRepoLinks(repo)
    }
  })
  
  return displayRepos
})

const searchRepos = async (username: string) => {
  isLoading.value = true
  error.value = ''
  repos.value = []
  selectedRepo.value = null
  repoFilter.value = ''
  checkedRepos.clear()
  loadingRepos.clear()
  currentUsername.value = username
  
  try {
    const octokit = new Octokit()
    const { data: userRepos } = await octokit.repos.listForUser({
      username,
      sort: 'updated',
      direction: 'desc',
      per_page: 100
    })

    repos.value = userRepos.map(repo => ({
      name: repo.name,
      stars: repo.stargazers_count ?? 0,
      forks: repo.forks_count ?? 0,
      lastUpdated: repo.updated_at ?? new Date().toISOString(),
      defaultBranch: repo.default_branch ?? 'main',
      linkStats: {
        bad: 0,
        warning: 0,
        ok: 0
      },
      links: []
    }))

    // Auto-select first repo and check its links
    if (repos.value.length > 0) {
      selectedRepo.value = repos.value[0]
      loadingRepos.add(repos.value[0].name)
      await checkRepoLinks(repos.value[0])
    }
  } catch (err) {
    error.value = 'Error fetching repositories'
    console.error(err)
  } finally {
    isLoading.value = false
  }
}

const checkRepoLinks = async (repo: RepoInfo) => {
  try {
    const response = await fetch(getApiUrl(`/Home/CheckRepo?projectBaseUrl=${encodeURIComponent(`https://github.com/${currentUsername.value}/${repo.name}`)}&branch=${repo.defaultBranch}`), {
      method: 'GET',
      mode: 'cors',
      headers: {
        'Accept': 'application/json'
      }
    })
    
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }
    
    const apiLinks: ApiLinkResponse[] = await response.json()
    
    // Map API response to our LinkInfo type and sort
    const links: LinkInfo[] = apiLinks
      .map(link => ({
        url: link.uriText,
        statusCode: link.httpStatusCode,
        statusText: link.httpStatusCodeText,
        type: link.httpStatusCode >= 400 ? 'bad' as const : 
              link.httpStatusCode >= 300 ? 'warning' as const : 'ok' as const
      }))
      .sort((a, b) => {
        // First sort by type (bad > warning > ok)
        const typeOrder: Record<LinkInfo['type'], number> = { bad: 0, warning: 1, ok: 2 }
        const typeComparison = typeOrder[a.type] - typeOrder[b.type]
        if (typeComparison !== 0) return typeComparison
        
        // Then sort alphabetically by URL
        return a.url.localeCompare(b.url)
      })
    
    repo.links = links
    repo.linkStats = {
      bad: links.filter(l => l.type === 'bad').length,
      warning: links.filter(l => l.type === 'warning').length,
      ok: links.filter(l => l.type === 'ok').length
    }
  } catch (err) {
    console.error(`Error checking links for ${repo.name}:`, err)
    // Set empty links and stats on error
    repo.links = []
    repo.linkStats = {
      bad: 0,
      warning: 0,
      ok: 0
    }
  } finally {
    loadingRepos.delete(repo.name)
    checkedRepos.add(repo.name)
  }
}

const toggleFavorite = (repo: RepoInfo) => {
  repo.isFavorite = !repo.isFavorite
  const favorites = repos.value
    .filter(r => r.isFavorite)
    .map(r => r.name)
  localStorage.setItem('favorites', JSON.stringify(favorites))
}

const clearResults = () => {
  repos.value = []
  selectedRepo.value = null
  repoFilter.value = ''
  checkedRepos.clear()
  loadingRepos.clear()
  currentUsername.value = ''
}
</script>

<template>
  <div class="github-readme-checker">
    <ThemeToggle />
    
    <div class="input-section">
      <SearchForm 
        :is-loading="isLoading"
        @search="searchRepos"
        @clear="clearResults"
      />

      <div v-if="error" class="error-message">
        {{ error }}
      </div>

      <div v-if="repos.length" class="repo-filter">
        <input 
          v-model="repoFilter"
          type="text"
          placeholder="Filter repositories..."
          :disabled="isLoading"
        />
      </div>
    </div>

    <div v-if="repos.length" class="repos-list">
      <RepoCard 
        v-for="repo in filteredRepos"
        :key="repo.name"
        :repo="repo"
        :is-selected="selectedRepo?.name === repo.name"
        :is-loading="loadingRepos.has(repo.name)"
        @select="selectedRepo = repo"
      />
    </div>

    <div v-if="selectedRepo" class="links-section">
      <LinkList
        :repo-name="selectedRepo.name"
        :links="selectedRepo.links"
        v-model:filter="linkFilter"
      />
    </div>
  </div>
</template>

<style>
:root {
  /* Light theme variables */
  --primary-color: #2563eb;
  --primary-color-dark: #1d4ed8;
  --text-color: #1f2937;
  --text-secondary: #4b5563;
  --border-color: #e5e7eb;
  --card-bg: #ffffff;
  --input-bg: #ffffff;
  --button-bg: #f3f4f6;
  --button-hover-bg: #e5e7eb;
  --link-color: #2563eb;
  --error-color: #dc2626;
  --warning-color: #d97706;
  --success-color: #059669;
  --error-bg: #fee2e2;
  --warning-bg: #fef3c7;
  --success-bg: #d1fae5;
  --disabled-color: #9ca3af;
  --selected-bg: #eff6ff;
  --selected-border: #93c5fd;
  --page-bg: #ffffff;
}

.dark-theme {
  /* Dark theme variables */
  --primary-color: #3b82f6;
  --primary-color-dark: #2563eb;
  --text-color: #f3f4f6;
  --text-secondary: #d1d5db;
  --border-color: #374151;
  --card-bg: #1f2937;
  --input-bg: #374151;
  --button-bg: #374151;
  --button-hover-bg: #4b5563;
  --link-color: #60a5fa;
  --error-color: #ef4444;
  --warning-color: #f59e0b;
  --success-color: #10b981;
  --error-bg: #7f1d1d;
  --warning-bg: #78350f;
  --success-bg: #064e3b;
  --disabled-color: #6b7280;
  --selected-bg: #1e3a8a;
  --selected-border: #3b82f6;
  --page-bg: #111827;
}

.github-readme-checker {
  position: absolute;
  left: 0;
  right: 0;
  width: 100vw;
  min-height: 100vh;
  padding: 20px;
  box-sizing: border-box;
  background-color: var(--page-bg);
  overflow-x: hidden;
}

.input-section {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 15px;
  margin-bottom: 30px;
  width: 100%;
}

.repo-filter {
  width: 100%;
  max-width: 400px;
  display: flex;
  justify-content: center;
}

.repo-filter input {
  width: 100%;
  padding: 8px 12px;
  border: 1px solid var(--border-color);
  border-radius: 4px;
  background-color: var(--input-bg);
  color: var(--text-color);
  font-size: 1em;
}

.repos-list {
  display: flex;
  gap: 20px;
  margin-bottom: 30px;
  width: 100%;
  padding: 0 20px;
  box-sizing: border-box;
  justify-content: center;
}

.repo-card {
  width: 300px;
  border: 1px solid var(--border-color);
  border-radius: 8px;
  padding: 15px;
  background-color: var(--card-bg);
  transition: all 0.2s ease;
  cursor: pointer;
  box-sizing: border-box;
}

.repo-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.repo-card.selected {
  background-color: var(--selected-bg);
  border-color: var(--selected-border);
  box-shadow: 0 0 0 2px var(--selected-border);
}

.repo-card.loading {
  opacity: 0.7;
}

.error-message {
  color: var(--error-color);
  margin: 20px 0;
  padding: 10px;
  border-radius: 4px;
  background-color: var(--error-bg);
  width: 100%;
  max-width: 400px;
  margin-left: auto;
  margin-right: auto;
}

/* Add styles for the links section */
.links-section {
  width: 100%;
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
  box-sizing: border-box;
}

@media (max-width: 1600px) {
  .repos-list {
    grid-template-columns: repeat(4, 1fr);
  }
}

@media (max-width: 1280px) {
  .repos-list {
    grid-template-columns: repeat(3, 1fr);
  }
}

@media (max-width: 960px) {
  .repos-list {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 640px) {
  .repos-list {
    grid-template-columns: 1fr;
  }
}
</style>
