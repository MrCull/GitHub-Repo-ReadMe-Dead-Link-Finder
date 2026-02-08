<script setup lang="ts">
import type { LinkStatus, LinkTab } from '@/types/github'
import LinkItem from './LinkItem.vue'

defineProps<{
  links: LinkStatus[]
  status: LinkTab
}>()
</script>

<template>
  <TransitionGroup name="list" tag="div" class="link-list">
    <LinkItem v-for="(link, index) in links" :key="link.uriText" :link="link" :status="status" :style="{ transitionDelay: `${index * 30}ms` }" />
  </TransitionGroup>
</template>

<style scoped>
.link-list {
  display: flex;
  flex-direction: column;
  gap: 8px;
}
</style>
