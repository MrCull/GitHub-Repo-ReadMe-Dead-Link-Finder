<script setup lang="ts">
import type { SortKey, SortDirection } from '@/types/github'
import RepoFilter from './RepoFilter.vue'
import SortDropdown from './SortDropdown.vue'

const props = defineProps<{
  totalCount: number
  filterTerm: string
  sortBy: SortKey
  sortDir: SortDirection
}>()

const emit = defineEmits<{
  'update:filterTerm': [value: string]
  'update:sortBy': [value: SortKey]
  'update:sortDir': [value: SortDirection]
}>()
</script>

<template>
  <div class="repo-header">
    <RepoFilter
      :modelValue="filterTerm"
      @update:modelValue="emit('update:filterTerm', $event)"
    />
    <div class="repo-toolbar-row">
      <span class="repo-count">
        <strong>{{ totalCount }}</strong> repositories
      </span>
      <div class="repo-toolbar-right">
        <SortDropdown
          :sortBy="sortBy"
          :sortDir="sortDir"
          @update:sortBy="emit('update:sortBy', $event)"
          @update:sortDir="emit('update:sortDir', $event)"
        />
      </div>
    </div>
  </div>
</template>

<style scoped>
.repo-header {
  background: #ffffff;
  border: 1px solid #d0d7de;
  border-radius: 6px;
  padding: 8px 12px;
  margin-bottom: 16px;
}

.repo-toolbar-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.repo-count {
  font-size: 14px;
  color: #57606a;
}

.repo-count strong {
  font-weight: 600;
  color: #24292f;
}

.repo-toolbar-right {
  display: flex;
  gap: 8px;
  align-items: center;
}
</style>
