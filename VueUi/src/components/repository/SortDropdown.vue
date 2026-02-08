<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import type { SortKey, SortDirection } from '@/types/github'

const props = defineProps<{
  sortBy: SortKey
  sortDir: SortDirection
}>()

const emit = defineEmits<{
  'update:sortBy': [value: SortKey]
  'update:sortDir': [value: SortDirection]
}>()

const open = ref(false)
const triggerRef = ref<HTMLElement | null>(null)

const sortLabels: Record<SortKey, string> = {
  pushed: 'Last pushed',
  name: 'Name',
  stars: 'Stars',
}

const sortIcons: Record<SortKey, string> = {
  pushed: '↩',
  name: 'Aa',
  stars: '★',
}

function toggleMenu() {
  open.value = !open.value
}

function selectSort(key: SortKey) {
  emit('update:sortBy', key)
  open.value = false
}

function selectDir(dir: SortDirection) {
  emit('update:sortDir', dir)
  open.value = false
}

function handleClickOutside(e: MouseEvent) {
  if (triggerRef.value && !triggerRef.value.contains(e.target as Node)) {
    open.value = false
  }
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})
</script>

<template>
  <div class="repo-sort" ref="triggerRef">
    <button class="repo-sort-trigger" @click="toggleMenu" type="button">
      <span>{{ sortLabels[sortBy] }}</span>
      <span class="caret">▾</span>
    </button>
    <div v-if="open" class="repo-sort-menu">
      <button
        v-for="key in (['pushed', 'name', 'stars'] as SortKey[])"
        :key="key"
        class="sort-menu-btn"
        :class="{ 'is-active': sortBy === key }"
        @click="selectSort(key)"
      >
        <span class="menu-icon">{{ sortIcons[key] }}</span>
        <span class="menu-label">{{ sortLabels[key] }}</span>
        <span class="tick" v-if="sortBy === key">✓</span>
      </button>
      <div class="menu-separator"></div>
      <button
        class="sort-menu-btn"
        :class="{ 'is-active': sortDir === 'asc' }"
        @click="selectDir('asc')"
      >
        <span class="menu-icon">↑</span>
        <span class="menu-label">Ascending</span>
        <span class="tick" v-if="sortDir === 'asc'">✓</span>
      </button>
      <button
        class="sort-menu-btn"
        :class="{ 'is-active': sortDir === 'desc' }"
        @click="selectDir('desc')"
      >
        <span class="menu-icon">↓</span>
        <span class="menu-label">Descending</span>
        <span class="tick" v-if="sortDir === 'desc'">✓</span>
      </button>
    </div>
  </div>
</template>

<style scoped>
.repo-sort {
  position: relative;
}

.repo-sort-trigger {
  min-height: 32px;
  padding: 0 12px;
  border-radius: 6px;
  border: 1px solid #d0d7de;
  background: #ffffff;
  font-size: 14px;
  font-family: inherit;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 4px;
  color: #24292f;
}

.repo-sort-trigger:hover {
  background: #f6f8fa;
}

.caret {
  font-size: 10px;
  margin-left: 4px;
}

.repo-sort-menu {
  position: absolute;
  top: 100%;
  right: 0;
  margin-top: 4px;
  background: #ffffff;
  border-radius: 6px;
  border: 1px solid #d0d7de;
  box-shadow: 0 8px 24px rgba(140, 149, 159, 0.2);
  padding: 4px 0;
  width: 180px;
  z-index: 100;
  animation: dropdown-in 0.15s ease;
}

@keyframes dropdown-in {
  from {
    opacity: 0;
    transform: translateY(-4px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.sort-menu-btn {
  display: flex;
  align-items: center;
  width: 100%;
  padding: 6px 12px;
  background: transparent;
  border: 0;
  font-size: 14px;
  cursor: pointer;
  color: #24292f;
  font-family: inherit;
  text-align: left;
}

.sort-menu-btn:hover {
  background: #f6f8fa;
}

.sort-menu-btn.is-active {
  font-weight: 600;
}

.menu-icon {
  opacity: 0.7;
  margin-right: 8px;
  font-size: 14px;
}

.menu-label {
  flex: 1;
}

.tick {
  margin-left: 8px;
  font-size: 12px;
  color: #0969da;
}

.menu-separator {
  height: 1px;
  margin: 4px 0;
  background: #d0d7de;
}
</style>
