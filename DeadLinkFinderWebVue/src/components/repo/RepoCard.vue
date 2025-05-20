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
      <div v-if="isLoading" class="loading-spinner"></div>
    </div>
    
    <div class="repo-stats">
      <span class="stat" title="Stars">⭐ {{ repo.stars }}</span>
      <span class="stat" title="Forks">🍴 {{ repo.forks }}</span>
      <span class="stat" title="Last Updated">🕒 {{ formattedDate }}</span>
    </div>
    
    <div class="link-stats">
      <span class="bad" title="Bad Links">❌ {{ repo.linkStats.bad }}</span>
      <span class="warning" title="Warning Links">⚠️ {{ repo.linkStats.warning }}</span>
      <span class="ok" title="OK Links">✅ {{ repo.linkStats.ok }}</span>
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

.link-stats span {
  padding: 4px 10px;
  border-radius: 4px;
  font-weight: 500;
}

.link-stats .bad {
  background-color: var(--error-bg);
  color: var(--error-color);
}

.link-stats .warning {
  background-color: var(--warning-bg);
  color: var(--warning-color);
}

.link-stats .ok {
  background-color: var(--success-bg);
  color: var(--success-color);
}
</style> 