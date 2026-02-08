<script setup lang="ts">
import type { LinkStatus, LinkTab } from '@/types/github'

defineProps<{
  link: LinkStatus
  status: LinkTab
}>()
</script>

<template>
  <div class="link-item" :class="{
    'error-link': status === 'broken',
    'warning-link': status === 'warning',
    'ok-link': status === 'ok'
  }">
    <span class="link-status">{{ link.httpStatusCodeText }} ({{ link.httpStatusCode }})</span>
    <a :href="link.uriText" target="_blank" rel="noopener" class="link-url">{{ link.uriText }}</a>
  </div>
</template>

<style scoped>
.link-item {
  border: 1px solid #d0d7de;
  border-left-width: 3px;
  padding: 0.6rem 0.75rem;
  border-radius: 4px;
  background: var(--gh-surface);
  display: flex;
  flex-direction: column;
  gap: 4px;
  transition: background 0.15s ease, box-shadow 0.15s ease;
}

.link-item:hover {
  box-shadow: 0 1px 4px rgba(27, 31, 36, 0.06);
}

.error-link {
  border-left-color: var(--gh-danger);
}

.warning-link {
  border-left-color: var(--gh-warning);
}

.ok-link {
  border-left-color: var(--gh-success);
}

.link-status {
  font-weight: 600;
  color: var(--gh-text);
  font-size: 0.9rem;
}

.link-url {
  word-break: break-all;
  font-size: 0.9rem;
  color: var(--gh-accent);
}
</style>
