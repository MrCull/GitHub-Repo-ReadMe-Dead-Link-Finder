<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  repo: {
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
    isFavorite?: boolean
  }
}

const props = defineProps<Props>()
const emit = defineEmits<{
  (e: 'select'): void
  (e: 'toggle-favorite'): void
}>()

const formattedDate = computed(() => {
  return new Date(props.repo.lastUpdated).toLocaleDateString()
})

const statusColor = computed(() => {
  if (props.repo.linkStats.bad > 0) return 'var(--error-color)'
  if (props.repo.linkStats.warning > 0) return 'var(--warning-color)'
  return 'var(--success-color)'
})
</script>

<template>
  <div 
    class="repo-card"
    @click="emit('select')"
  >
    <div class="repo-header">
      <h3>{{ repo.name }}</h3>
      <button 
        class="favorite-btn"
        @click.stop="emit('toggle-favorite')"
        :class="{ 'is-favorite': repo.isFavorite }"
      >
        {{ repo.isFavorite ? '★' : '☆' }}
      </button>
    </div>
    
    <div class="repo-stats">
      <span title="Stars">⭐ {{ repo.stars }}</span>
      <span title="Forks">🍴 {{ repo.forks }}</span>
      <span title="Last Updated">🕒 {{ formattedDate }}</span>
    </div>
    
    <div class="link-stats">
      <span class="bad" title="Broken Links">❌ {{ repo.linkStats.bad }}</span>
      <span class="warning" title="Warning Links">⚠️ {{ repo.linkStats.warning }}</span>
      <span class="ok" title="Working Links">✅ {{ repo.linkStats.ok }}</span>
    </div>
  </div>
</template>

<style scoped>
.repo-card {
  padding: 20px;
  border: 1px solid var(--border-color);
  border-radius: 8px;
  cursor: pointer;
  transition: transform 0.2s, box-shadow 0.2s;
  background-color: var(--card-bg);
}

.repo-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.repo-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 10px;
}

.favorite-btn {
  background: none;
  border: none;
  font-size: 1.5em;
  cursor: pointer;
  color: var(--text-color);
  transition: color 0.2s;
}

.favorite-btn.is-favorite {
  color: var(--warning-color);
}

.repo-stats {
  display: flex;
  gap: 15px;
  margin: 10px 0;
  color: var(--text-secondary);
}

.link-stats {
  display: flex;
  gap: 15px;
}

.link-stats span {
  padding: 4px 8px;
  border-radius: 4px;
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