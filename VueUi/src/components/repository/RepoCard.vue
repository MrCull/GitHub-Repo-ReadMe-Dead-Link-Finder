<script setup lang="ts">
import { computed, onMounted } from 'vue'
import type { Repository, RepoLinkResults, LinkTab } from '@/types/github'
import { convertUriToValidId } from '@/utils/formatters'
import RepoHeader from './RepoHeader.vue'
import RepoMeta from './RepoMeta.vue'
import LinkTabs from '../links/LinkTabs.vue'

const props = defineProps<{
  repo: Repository
  links: RepoLinkResults | undefined
  isChecking: boolean
  activeTab: LinkTab
}>()

const emit = defineEmits<{
  check: [repo: Repository]
  retry: [repo: Repository]
  'update:activeTab': [tab: LinkTab]
}>()

const repoId = computed(() => convertUriToValidId(props.repo.url))

const hasBroken = computed(() => {
  if (!props.links) return false
  return props.links.broken.length > 0
})

onMounted(() => {
  emit('check', props.repo)
})
</script>

<template>
  <div class="repo-card" :class="{ 'has-broken': hasBroken }">
    <div class="repo-main">
      <RepoHeader :repo="repo" />
      <RepoMeta :repo="repo" :isChecking="isChecking" />
      <LinkTabs
        :repoId="repoId"
        :links="links"
        :isChecking="isChecking"
        :activeTab="activeTab"
        @update:activeTab="emit('update:activeTab', $event)"
        @retry="emit('retry', repo)"
      />
    </div>
  </div>
</template>

<style scoped>
.repo-card {
  display: block;
  padding: 16px;
  margin-bottom: 16px;
  border: 1px solid #d0d7de;
  border-radius: 6px;
  background: #ffffff;
  transition: background 0.25s ease, border-color 0.25s ease, box-shadow 0.25s ease, transform 0.15s ease;
}

.repo-card:hover {
  box-shadow: 0 2px 8px rgba(27, 31, 36, 0.08);
}

.repo-card:last-child {
  margin-bottom: 0;
}

.repo-card.has-broken {
  background: #fff5f5;
  border-color: #ffebe9;
}

.repo-card.has-broken:hover {
  border-color: #f9c3bf;
}

.repo-main {
  width: 100%;
}
</style>
