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

const filteredRepos = computed(() => {
  let filtered = repos.value
  if (repoFilter.value) {
    const filter = repoFilter.value.toLowerCase()
    filtered = repos.value.filter(repo => repo.name.toLowerCase().startsWith(filter))
  }
  const displayRepos = filtered.slice(0, 5)
  
  // Check links for newly displayed repos
  displayRepos.forEach(repo => {
    if (!checkedRepos.has(repo.name)) {
      checkRepoLinks(repo)
      checkedRepos.add(repo.name)
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
      links: [],
      isFavorite: false
    }))

    // Load favorites from localStorage
    const favorites = JSON.parse(localStorage.getItem('favorites') || '[]')
    repos.value = repos.value.map(repo => ({
      ...repo,
      isFavorite: favorites.includes(repo.name)
    }))
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
    
    // Map API response to our LinkInfo type
    const links: LinkInfo[] = apiLinks.map(link => ({
      url: link.uriText,
      statusCode: link.httpStatusCode,
      statusText: link.httpStatusCodeText,
      type: link.httpStatusCode >= 400 ? 'bad' : 
            link.httpStatusCode >= 300 ? 'warning' : 'ok'
    }))
    
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
  }
}

const toggleFavorite = (repo: RepoInfo) => {
  repo.isFavorite = !repo.isFavorite
  const favorites = repos.value
    .filter(r => r.isFavorite)
    .map(r => r.name)
  localStorage.setItem('favorites', JSON.stringify(favorites))
}
</script>

<template>
  <div class="github-readme-checker">
    <ThemeToggle />
    
    <SearchForm 
      :is-loading="isLoading"
      @search="searchRepos"
    />

    <div v-if="error" class="error-message">
      {{ error }}
    </div>

    <div v-if="repos.length" class="results-section">
      <div class="repo-filter">
        <input 
          v-model="repoFilter"
          type="text"
          placeholder="Filter repositories..."
          :disabled="isLoading"
        />
      </div>

      <div class="repos-list">
        <RepoCard 
          v-for="repo in filteredRepos"
          :key="repo.name"
          :repo="repo"
          @select="selectedRepo = repo"
          @toggle-favorite="toggleFavorite(repo)"
        />
      </div>

      <LinkList
        v-if="selectedRepo"
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
  --primary-color: #2c3e50;
  --primary-color-dark: #1a252f;
  --text-color: #2c3e50;
  --text-secondary: #666;
  --border-color: #ddd;
  --card-bg: #fff;
  --input-bg: #fff;
  --button-bg: #f5f5f5;
  --button-hover-bg: #e0e0e0;
  --link-color: #2c3e50;
  --error-color: #c62828;
  --warning-color: #ef6c00;
  --success-color: #2e7d32;
  --error-bg: #ffebee;
  --warning-bg: #fff3e0;
  --success-bg: #e8f5e9;
  --disabled-color: #95a5a6;
}

.dark-theme {
  /* Dark theme variables */
  --primary-color: #3498db;
  --primary-color-dark: #2980b9;
  --text-color: #ecf0f1;
  --text-secondary: #bdc3c7;
  --border-color: #34495e;
  --card-bg: #2c3e50;
  --input-bg: #34495e;
  --button-bg: #34495e;
  --button-hover-bg: #2c3e50;
  --link-color: #3498db;
  --error-color: #e74c3c;
  --warning-color: #f39c12;
  --success-color: #2ecc71;
  --error-bg: #2c1f1f;
  --warning-bg: #2c2a1f;
  --success-bg: #1f2c1f;
  --disabled-color: #7f8c8d;
}

.github-readme-checker {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
}

.repo-filter {
  margin-bottom: 20px;
}

.repo-filter input {
  width: 100%;
  max-width: 300px;
  padding: 8px;
  border: 1px solid var(--border-color);
  border-radius: 4px;
  background-color: var(--input-bg);
  color: var(--text-color);
}

.repos-list {
  display: flex;
  gap: 20px;
  margin-bottom: 30px;
  overflow-x: auto;
  padding-bottom: 10px;
}

.repos-list::-webkit-scrollbar {
  height: 8px;
}

.repos-list::-webkit-scrollbar-track {
  background: var(--button-bg);
  border-radius: 4px;
}

.repos-list::-webkit-scrollbar-thumb {
  background: var(--primary-color);
  border-radius: 4px;
}

.repos-list::-webkit-scrollbar-thumb:hover {
  background: var(--primary-color-dark);
}

.error-message {
  color: var(--error-color);
  margin: 20px 0;
  padding: 10px;
  border-radius: 4px;
  background-color: var(--error-bg);
}
</style>
