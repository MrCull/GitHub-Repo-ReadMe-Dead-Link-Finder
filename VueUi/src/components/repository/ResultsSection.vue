<script setup lang="ts">
import type { Repository, RepoLinkResults, SortKey, SortDirection, LinkTab } from '@/types/github'
import RepoToolbar from './RepoToolbar.vue'
import RepoList from './RepoList.vue'
import LoadMore from './LoadMore.vue'

defineProps<{
  visibleRepos: Repository[]
  totalCount: number
  showingCount: number
  remainingCount: number
  hasMore: boolean
  filterTerm: string
  sortBy: SortKey
  sortDir: SortDirection
  repoLinks: Record<string, RepoLinkResults>
  checkingRepos: Set<string>
  selectedTabs: Record<string, LinkTab>
}>()

const emit = defineEmits<{
  'update:filterTerm': [value: string]
  'update:sortBy': [value: SortKey]
  'update:sortDir': [value: SortDirection]
  check: [repo: Repository]
  retry: [repo: Repository]
  tabChange: [repoId: string, tab: LinkTab]
  loadMore: []
}>()
</script>

<template>
  <section class="gh-results">
    <RepoToolbar
      :totalCount="totalCount"
      :filterTerm="filterTerm"
      :sortBy="sortBy"
      :sortDir="sortDir"
      @update:filterTerm="emit('update:filterTerm', $event)"
      @update:sortBy="emit('update:sortBy', $event)"
      @update:sortDir="emit('update:sortDir', $event)"
    />

    <RepoList
      :repos="visibleRepos"
      :repoLinks="repoLinks"
      :checkingRepos="checkingRepos"
      :selectedTabs="selectedTabs"
      @check="emit('check', $event)"
      @retry="emit('retry', $event)"
      @tabChange="(repoId, tab) => emit('tabChange', repoId, tab)"
    />

    <LoadMore
      :showingCount="showingCount"
      :remainingCount="remainingCount"
      :hasMore="hasMore"
      @loadMore="emit('loadMore')"
    />
  </section>
</template>

<style scoped>
.gh-results {
  background: transparent;
}
</style>
