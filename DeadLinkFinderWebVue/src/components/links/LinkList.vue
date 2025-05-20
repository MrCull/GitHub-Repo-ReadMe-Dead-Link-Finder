<script setup lang="ts">
import { computed, ref } from 'vue'

interface LinkInfo {
  url: string
  statusCode: number
  statusText: string
  type: 'bad' | 'warning' | 'ok'
}

const props = defineProps<{
  repoName: string
  links: LinkInfo[]
}>()

const emit = defineEmits<{
  (e: 'update:filter', value: Set<'bad' | 'warning' | 'ok'>): void
}>()

const showTypes = ref<Set<'bad' | 'warning' | 'ok'>>(new Set(['bad', 'warning', 'ok']))

const filteredLinks = computed(() => {
  return props.links.filter(link => showTypes.value.has(link.type))
})

const toggleType = (type: 'bad' | 'warning' | 'ok') => {
  if (showTypes.value.has(type)) {
    showTypes.value.delete(type)
  } else {
    showTypes.value.add(type)
  }
  emit('update:filter', showTypes.value)
}
</script>

<template>
  <div class="link-list">
    <h3>Links for {{ repoName }}</h3>
    
    <div class="filter-controls">
      <label class="filter-checkbox">
        <input 
          type="checkbox" 
          :checked="showTypes.has('bad')"
          @change="toggleType('bad')"
        />
        <span class="bad">Bad</span>
      </label>
      <label class="filter-checkbox">
        <input 
          type="checkbox" 
          :checked="showTypes.has('warning')"
          @change="toggleType('warning')"
        />
        <span class="warning">Warning</span>
      </label>
      <label class="filter-checkbox">
        <input 
          type="checkbox" 
          :checked="showTypes.has('ok')"
          @change="toggleType('ok')"
        />
        <span class="ok">OK</span>
      </label>
    </div>

    <div class="links">
      <div 
        v-for="link in filteredLinks" 
        :key="link.url"
        class="link-item"
        :class="link.type"
      >
        <div class="link-status">
          <span class="status-code">{{ link.statusCode }}</span>
          <span class="status-text">{{ link.statusText }}</span>
        </div>
        <a :href="link.url" target="_blank" rel="noopener noreferrer" class="link-url">
          {{ link.url }}
        </a>
      </div>
    </div>
  </div>
</template>

<style scoped>
.link-list {
  margin-top: 20px;
  padding: 20px;
  background-color: var(--card-bg);
  border-radius: 8px;
  border: 1px solid var(--border-color);
  width: 100%;
}

.filter-controls {
  display: flex;
  gap: 20px;
  margin-bottom: 20px;
}

.filter-checkbox {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
}

.filter-checkbox input[type="checkbox"] {
  width: 16px;
  height: 16px;
  cursor: pointer;
}

.filter-checkbox span {
  padding: 4px 8px;
  border-radius: 4px;
  font-weight: bold;
}

.filter-checkbox span.bad {
  background-color: var(--error-bg);
  color: var(--error-color);
}

.filter-checkbox span.warning {
  background-color: var(--warning-bg);
  color: var(--warning-color);
}

.filter-checkbox span.ok {
  background-color: var(--success-bg);
  color: var(--success-color);
}

.links {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.link-item {
  padding: 12px;
  border: 1px solid var(--border-color);
  border-radius: 6px;
  margin-bottom: 10px;
  background-color: var(--card-bg);
}

.link-item.bad {
  border-left: 4px solid var(--error-color);
}

.link-item.warning {
  border-left: 4px solid var(--warning-color);
}

.link-item.ok {
  border-left: 4px solid var(--success-color);
}

.link-status {
  margin-top: 8px;
  font-size: 0.9em;
  color: var(--text-secondary);
}

.status-code {
  font-weight: bold;
}

.status-text {
  font-size: 0.9em;
  color: var(--text-secondary);
}

.link-url {
  color: #2563eb; /* Always blue */
  text-decoration: none;
  word-break: break-all;
  font-size: 1.1em;
}

.link-url:hover {
  text-decoration: underline;
}
</style> 