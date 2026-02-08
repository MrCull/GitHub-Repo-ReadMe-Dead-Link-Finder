<script setup lang="ts">
import type { Repository } from '@/types/github'

defineProps<{
  repo: Repository
}>()
</script>

<template>
  <div class="repo-header-row">
    <a class="repo-title" :href="repo.url" target="_blank" rel="noopener">{{ repo.name }}</a>
    <span class="repo-visibility">Public</span>
  </div>
  <p v-if="repo.description" class="repo-description">{{ repo.description }}</p>
  <div v-if="repo.topics && repo.topics.length > 0" class="repo-topics">
    <span v-for="topic in repo.topics.slice(0, 8)" :key="topic" class="repo-topic">{{ topic }}</span>
  </div>
</template>

<style scoped>
.repo-header-row {
  display: flex;
  align-items: baseline;
  gap: 8px;
  margin-bottom: 4px;
}

.repo-title {
  font-size: 16px;
  font-weight: 600;
  color: #0969da;
  text-decoration: none;
}

.repo-title:hover {
  text-decoration: underline;
}

.repo-visibility {
  margin-left: 8px;
  padding: 0 8px;
  font-size: 12px;
  border-radius: 999px;
  border: 1px solid #d0d7de;
  background: #f6f8fa;
  color: #57606a;
  font-weight: 400;
  line-height: 18px;
  white-space: nowrap;
}

.repo-description {
  margin: 4px 0 8px;
  color: #57606a;
  font-size: 14px;
}

.repo-topics {
  display: flex;
  flex-wrap: wrap;
  gap: 4px;
  margin-bottom: 8px;
}

.repo-topic {
  font-size: 12px;
  border-radius: 999px;
  border: 1px solid #d0d7de;
  padding: 2px 8px;
  color: #0969da;
  background: #ddf4ff;
  cursor: default;
  transition: background 0.15s ease;
}

.repo-topic:hover {
  background: #b6e3ff;
}
</style>
