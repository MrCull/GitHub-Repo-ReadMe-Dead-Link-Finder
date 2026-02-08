<script setup lang="ts">
import { ref } from 'vue'

// Types matching C# API responses
interface Repository {
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

interface ApiError {
  error: string
  exceptionType?: string
}

// Reactive state
const username = ref('octocat')
const repositories = ref<Repository[]>([])
const loading = ref(false)
const error = ref<string | null>(null)

// Fetch repositories from ASP.NET API
async function fetchRepos() {
  if (!username.value.trim()) {
    error.value = 'Please enter a GitHub username or organization'
    return
  }

  loading.value = true
  error.value = null
  repositories.value = []

  try {
    // Using relative URL - works in both dev and production
    const response = await fetch(
      `/Home/GetUserRepos?userOrOrg=${encodeURIComponent(username.value)}&includeForks=false&maxRepos=10`
    )

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }

    const data = await response.json()

    // Check if API returned an error object
    if (isApiError(data)) {
      error.value = data.error
      return
    }

    repositories.value = data as Repository[]
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Failed to fetch repositories'
    console.error('Error fetching repos:', err)
  } finally {
    loading.value = false
  }
}

// Type guard for API error responses
function isApiError(data: unknown): data is ApiError {
  return typeof data === 'object' && data !== null && 'error' in data
}
</script>

<template>
  <div class="app-container">
    <header>
      <h1>GitHub README Dead Link Finder</h1>
      <p>Vue SPA integrated with ASP.NET MVC</p>
    </header>

    <main>
      <div class="search-section">
        <h2>Search GitHub Repositories</h2>
        <div class="input-group">
          <input
            v-model="username"
            type="text"
            placeholder="Enter GitHub username or organization"
            @keyup.enter="fetchRepos"
          />
          <button @click="fetchRepos" :disabled="loading">
            {{ loading ? 'Searching...' : 'Search' }}
          </button>
        </div>
      </div>

      <div v-if="error" class="error-message">
        <strong>Error:</strong> {{ error }}
      </div>

      <div v-if="loading" class="loading">
        <div class="spinner"></div>
        <p>Fetching repositories...</p>
      </div>

      <div v-if="repositories.length > 0" class="results">
        <h3>Found {{ repositories.length }} repositories</h3>
        <div class="repo-grid">
          <div v-for="repo in repositories" :key="repo.url" class="repo-card">
            <h4>{{ repo.name }}</h4>
            <p class="description">{{ repo.description || 'No description' }}</p>
            <div class="repo-meta">
              <span>⭐ {{ repo.stars }}</span>
              <span>🍴 {{ repo.forks }}</span>
              <span v-if="repo.language">{{ repo.language }}</span>
            </div>
            <div class="repo-actions">
              <a :href="repo.url" target="_blank" rel="noopener">View on GitHub</a>
            </div>
          </div>
        </div>
      </div>

      <div v-if="!loading && repositories.length === 0 && !error" class="empty-state">
        <p>Enter a GitHub username or organization to search for repositories</p>
      </div>
    </main>
  </div>
</template>

<style scoped>
.app-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
}

header {
  text-align: center;
  margin-bottom: 3rem;
}

header h1 {
  color: #2c3e50;
  margin-bottom: 0.5rem;
}

header p {
  color: #7f8c8d;
}

.search-section {
  margin-bottom: 2rem;
}

.search-section h2 {
  margin-bottom: 1rem;
  color: #34495e;
}

.input-group {
  display: flex;
  gap: 0.5rem;
  max-width: 600px;
}

input {
  flex: 1;
  padding: 0.75rem;
  border: 2px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
}

input:focus {
  outline: none;
  border-color: #3498db;
}

button {
  padding: 0.75rem 1.5rem;
  background-color: #3498db;
  color: white;
  border: none;
  border-radius: 4px;
  font-size: 1rem;
  cursor: pointer;
  transition: background-color 0.2s;
}

button:hover:not(:disabled) {
  background-color: #2980b9;
}

button:disabled {
  background-color: #95a5a6;
  cursor: not-allowed;
}

.error-message {
  padding: 1rem;
  background-color: #ffe6e6;
  border-left: 4px solid #e74c3c;
  border-radius: 4px;
  margin-bottom: 1rem;
  color: #c0392b;
}

.loading {
  text-align: center;
  padding: 2rem;
}

.spinner {
  border: 4px solid #f3f3f3;
  border-top: 4px solid #3498db;
  border-radius: 50%;
  width: 40px;
  height: 40px;
  animation: spin 1s linear infinite;
  margin: 0 auto 1rem;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.results h3 {
  margin-bottom: 1.5rem;
  color: #2c3e50;
}

.repo-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1.5rem;
}

.repo-card {
  border: 1px solid #e1e4e8;
  border-radius: 6px;
  padding: 1.5rem;
  background: white;
  transition: box-shadow 0.2s;
}

.repo-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.repo-card h4 {
  margin: 0 0 0.5rem 0;
  color: #0366d6;
}

.description {
  color: #586069;
  font-size: 0.9rem;
  margin-bottom: 1rem;
  min-height: 2.7rem;
}

.repo-meta {
  display: flex;
  gap: 1rem;
  font-size: 0.85rem;
  color: #586069;
  margin-bottom: 1rem;
}

.repo-actions a {
  color: #0366d6;
  text-decoration: none;
  font-size: 0.9rem;
}

.repo-actions a:hover {
  text-decoration: underline;
}

.empty-state {
  text-align: center;
  padding: 3rem;
  color: #7f8c8d;
}
</style>
