<script setup lang="ts">
import { computed } from 'vue'

interface LinkInfo {
  url: string
  statusCode: number
  statusText: string
  type: 'bad' | 'warning' | 'ok'
}

interface Props {
  repoName: string
  links: LinkInfo[]
  filter: 'all' | 'bad' | 'warning' | 'ok'
}

const props = defineProps<Props>()
const emit = defineEmits<{
  (e: 'update:filter', value: 'all' | 'bad' | 'warning' | 'ok'): void
}>()

const filteredLinks = computed(() => {
  if (props.filter === 'all') return props.links
  return props.links.filter(link => link.type === props.filter)
})

const filterOptions = [
  { value: 'all', label: 'All' },
  { value: 'bad', label: 'Bad' },
  { value: 'warning', label: 'Warning' },
  { value: 'ok', label: 'OK' }
] as const
</script>

<template>
  <div class="links-section">
    <div class="links-header">
      <h3>Links for {{ repoName }}</h3>
      <div class="filter-controls">
        <button 
          v-for="option in filterOptions"
          :key="option.value"
          :class="{ active: filter === option.value }"
          @click="emit('update:filter', option.value)"
        >
          {{ option.label }}
        </button>
      </div>
    </div>
    
    <div class="links-list">
      <div 
        v-for="link in filteredLinks"
        :key="link.url"
        class="link-item"
        :class="link.type"
      >
        <a :href="link.url" target="_blank" rel="noopener noreferrer">
          {{ link.url }}
        </a>
        <span class="status">
          {{ link.statusCode }} - {{ link.statusText }}
        </span>
      </div>
    </div>
  </div>
</template>

<style scoped>
.links-section {
  border: 1px solid var(--border-color);
  border-radius: 8px;
  padding: 20px;
  background-color: var(--card-bg);
}

.links-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.filter-controls {
  display: flex;
  gap: 10px;
}

.filter-controls button {
  padding: 5px 10px;
  background-color: var(--button-bg);
  color: var(--text-color);
  border: 1px solid var(--border-color);
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.2s;
}

.filter-controls button:hover {
  background-color: var(--button-hover-bg);
}

.filter-controls button.active {
  background-color: var(--primary-color);
  color: white;
  border-color: var(--primary-color);
}

.links-list {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.link-item {
  padding: 10px;
  border-radius: 4px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  transition: background-color 0.2s;
}

.link-item.bad {
  background-color: var(--error-bg);
}

.link-item.warning {
  background-color: var(--warning-bg);
}

.link-item.ok {
  background-color: var(--success-bg);
}

.link-item a {
  color: var(--link-color);
  text-decoration: none;
  word-break: break-all;
  margin-right: 10px;
}

.link-item a:hover {
  text-decoration: underline;
}

.status {
  font-size: 0.9em;
  color: var(--text-secondary);
  white-space: nowrap;
}
</style> 