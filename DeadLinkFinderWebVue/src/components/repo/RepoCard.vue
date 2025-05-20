<script setup lang="ts">
import { computed } from 'vue'

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
}

interface LinkInfo {
  url: string
  statusCode: number
  statusText: string
  type: 'bad' | 'warning' | 'ok'
}

const props = defineProps<{
  repo: RepoInfo
  isSelected: boolean
  isLoading: boolean
}>()

const emit = defineEmits<{
  (e: 'select'): void
}>()

const formattedDate = computed(() => {
  return new Date(props.repo.lastUpdated).toLocaleDateString()
})
</script>

<template>
  <div 
    class="repo-card"
    :class="{ 
      'selected': isSelected,
      'loading': isLoading
    }"
    @click="emit('select')"
  >
    <div class="repo-header">
      <h3 class="repo-name">{{ repo.name }}</h3>
      <div class="header-actions">
        <a 
          :href="`https://github.com/${repo.name}`"
          target="_blank"
          rel="noopener noreferrer"
          class="repo-link"
          @click.stop
          title="Open in GitHub"
        >
          <i class="fab fa-github"></i>
        </a>
        <div v-if="isLoading" class="loading-spinner"></div>
      </div>
    </div>
    
    <div class="repo-stats">
      <span class="stat" title="Stars">⭐ {{ repo.stars }}</span>
      <span class="stat" title="Forks">🔱 {{ repo.forks }}</span>
      <span class="stat" title="Last Updated">🕒 {{ formattedDate }}</span>
    </div>
    
    <div class="link-stats">
      <div class="stat-item bad">
        <span class="label">Bad</span>
        <span class="value">
          <template v-if="isLoading">
            <div class="spinner"></div>
          </template>
          <template v-else>
            {{ repo.linkStats.bad }}
          </template>
        </span>
      </div>
      <div class="stat-item warning">
        <span class="label">Warning</span>
        <span class="value">
          <template v-if="isLoading">
            <div class="spinner"></div>
          </template>
          <template v-else>
            {{ repo.linkStats.warning }}
          </template>
        </span>
      </div>
      <div class="stat-item ok">
        <span class="label">Ok</span>
        <span class="value">
          <template v-if="isLoading">
            <div class="spinner"></div>
          </template>
          <template v-else>
            {{ repo.linkStats.ok }}
          </template>
        </span>
      </div>
    </div>
  </div>
</template>

<style scoped>
.repo-card {
  cursor: pointer;
  min-width: 400px;
  border: 1px solid var(--border-color);
  border-radius: 8px;
  padding: 20px;
  background-color: var(--card-bg);
  transition: all 0.2s ease;
}

.repo-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.repo-card.selected {
  border-color: var(--primary-color);
  box-shadow: 0 0 0 2px var(--primary-color);
}

.repo-card.loading {
  opacity: 0.7;
}

.repo-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 15px;
}

.repo-name {
  margin: 0;
  font-size: 1.3em;
  color: var(--text-color);
  font-weight: 600;
}

.loading-spinner {
  width: 20px;
  height: 20px;
  border: 2px solid var(--border-color);
  border-top-color: var(--primary-color);
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.repo-stats {
  display: flex;
  gap: 25px;
  margin-bottom: 15px;
  color: var(--text-secondary);
  font-size: 1em;
}

.stat {
  display: flex;
  align-items: center;
  gap: 6px;
}

.link-stats {
  display: flex;
  gap: 15px;
  font-size: 1em;
}

.stat-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 8px 12px;
  border-radius: 6px;
  min-width: 80px;
  background-color: var(--card-bg);
  border: 1px solid var(--border-color);
}

.stat-item .label {
  font-size: 0.9em;
  font-weight: 500;
  margin-bottom: 4px;
}

.stat-item .value {
  font-size: 1.1em;
  font-weight: 600;
}

.stat-item.bad {
  color: var(--error-color);
}

.stat-item.warning {
  color: var(--warning-color);
}

.stat-item.ok {
  color: var(--success-color);
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 10px;
}

.repo-link {
  color: var(--text-secondary);
  text-decoration: none;
  font-size: 1.1em;
  padding: 6px;
  border-radius: 4px;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.repo-link:hover {
  color: var(--primary-color);
  background-color: var(--button-hover-bg);
}

.spinner {
  display: inline-block;
  width: 16px;
  height: 16px;
  border: 2px solid currentColor;
  border-top-color: transparent;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

.value {
  min-width: 24px;
  height: 24px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
}
</style> 