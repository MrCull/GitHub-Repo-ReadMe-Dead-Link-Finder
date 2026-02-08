<script setup lang="ts">
defineProps<{
  showingCount: number
  remainingCount: number
  hasMore: boolean
}>()

const emit = defineEmits<{
  loadMore: []
}>()
</script>

<template>
  <div class="gh-load-more">
    <span class="gh-load-more-info">{{ showingCount }} shown · {{ remainingCount }} more</span>
    <button
      v-if="hasMore"
      class="gh-btn gh-btn-ghost"
      type="button"
      @click="emit('loadMore')"
    >
      Load more
    </button>
  </div>
</template>

<style scoped>
.gh-load-more {
  display: flex;
  justify-content: flex-start;
  padding: 0.75rem 0.25rem 0;
}

.gh-load-more-info {
  color: var(--gh-muted);
  font-size: 0.9rem;
  margin-right: 0.75rem;
  align-self: center;
}

.gh-btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.4rem;
  padding: 0.55rem 1rem;
  font-size: 0.95rem;
  font-weight: 600;
  border-radius: 6px;
  border: 1px solid var(--gh-border);
  background: var(--gh-surface);
  color: var(--gh-text);
  cursor: pointer;
  transition: background 0.15s ease;
}

.gh-btn-ghost {
  background: transparent;
  border-color: var(--gh-border);
  color: var(--gh-text);
}

.gh-btn-ghost:hover {
  background: #f3f4f6;
}
</style>
