<script setup lang="ts">
import type { Repository, RepoLinkResults, LinkTab } from '@/types/github'
import RepoCard from './RepoCard.vue'

defineProps<{
  repos: Repository[]
  repoLinks: Record<string, RepoLinkResults>
  checkingRepos: Set<string>
  selectedTabs: Record<string, LinkTab>
}>()

const emit = defineEmits<{
  check: [repo: Repository]
  retry: [repo: Repository]
  tabChange: [repoId: string, tab: LinkTab]
}>()

function getRepoId(url: string): string {
  return url.replace(/[^A-Za-z0-9]/gi, '')
}
</script>

<template>
  <div class="repo-list">
    <TransitionGroup name="list" tag="div" v-if="repos.length > 0">
      <RepoCard
        v-for="(repo, index) in repos"
        :key="repo.url"
        :repo="repo"
        :links="repoLinks[getRepoId(repo.url)]"
        :isChecking="checkingRepos.has(getRepoId(repo.url))"
        :activeTab="selectedTabs[getRepoId(repo.url)] || 'broken'"
        :style="{ transitionDelay: `${index * 50}ms` }"
        @check="emit('check', $event)"
        @retry="emit('retry', $event)"
        @update:activeTab="emit('tabChange', getRepoId(repo.url), $event)"
      />
    </TransitionGroup>
    <Transition name="fade">
      <div v-if="repos.length === 0" class="gh-empty-state">
        No repositories found.
      </div>
    </Transition>
  </div>
</template>

<style scoped>
.repo-list {
  margin-top: 16px;
  border: none;
  background: transparent;
}

.gh-empty-state {
  padding: 32px 16px;
  text-align: center;
  color: #57606a;
  font-size: 14px;
}
</style>
