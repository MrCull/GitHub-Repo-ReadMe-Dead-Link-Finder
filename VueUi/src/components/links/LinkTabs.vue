<script setup lang="ts">
import { computed } from 'vue'
import type { RepoLinkResults, LinkTab } from '@/types/github'
import LinkList from './LinkList.vue'

const props = defineProps<{
  repoId: string
  links: RepoLinkResults | undefined
  isChecking: boolean
  activeTab: LinkTab
}>()

const emit = defineEmits<{
  'update:activeTab': [tab: LinkTab]
  retry: []
}>()

const hasError = computed(() => !!props.links?.error)
const brokenCount = computed(() => props.links?.broken.length ?? null)
const warningCount = computed(() => props.links?.warning.length ?? null)
const okCount = computed(() => props.links?.ok.length ?? null)

const activeLinks = computed(() => {
  if (!props.links) return []
  return props.links[props.activeTab] || []
})

const emptyMessage = computed(() => {
  if (props.isChecking) return 'Checking links...'
  if (!props.links) return 'No links checked yet.'
  if (hasError.value) return ''
  if (activeLinks.value.length > 0) return ''
  if (props.activeTab === 'broken') return 'No broken links found! 🎉'
  if (props.activeTab === 'warning') return 'No warnings found.'
  return 'No working links yet.'
})

const emptyClass = computed(() => {
  if (props.isChecking || !props.links) return 'tab-empty-state'
  if (hasError.value) return ''
  if (activeLinks.value.length > 0) return ''
  if (props.activeTab === 'broken') return 'tab-empty-state state-good'
  if (props.activeTab === 'warning') return 'tab-empty-state state-warning'
  return 'tab-empty-state'
})
</script>

<template>
  <div class="repo-link-status">
    <div class="repo-tabs" role="tablist" aria-label="Link status tabs">
      <button
        class="repo-tab"
        :class="{ active: activeTab === 'broken' }"
        role="tab"
        :aria-selected="activeTab === 'broken'"
        :aria-controls="`${repoId}-tab-broken`"
        @click="emit('update:activeTab', 'broken')"
      >
        Broken links (<template v-if="brokenCount !== null">{{ brokenCount }}</template><span v-else class="mini-spinner" aria-label="loading"></span>)
      </button>
      <button
        class="repo-tab"
        :class="{ active: activeTab === 'warning' }"
        role="tab"
        :aria-selected="activeTab === 'warning'"
        :aria-controls="`${repoId}-tab-warning`"
        @click="emit('update:activeTab', 'warning')"
      >
        Links with Warnings (<template v-if="warningCount !== null">{{ warningCount }}</template><span v-else class="mini-spinner" aria-label="loading"></span>)
      </button>
      <button
        class="repo-tab"
        :class="{ active: activeTab === 'ok' }"
        role="tab"
        :aria-selected="activeTab === 'ok'"
        :aria-controls="`${repoId}-tab-ok`"
        @click="emit('update:activeTab', 'ok')"
      >
        Working links (<template v-if="okCount !== null">{{ okCount }}</template><span v-else class="mini-spinner" aria-label="loading"></span>)
      </button>
    </div>
    <div class="repo-tab-content">
      <!-- Error state with retry -->
      <div v-if="hasError && !isChecking" class="tab-error-state">
        <span class="error-icon">⚠️</span>
        <span>Failed to check links: {{ links?.error }}</span>
        <button class="retry-btn" @click="emit('retry')" type="button">
          Retry
        </button>
      </div>
      <Transition v-else name="tab-fade" mode="out-in">
        <LinkList
          v-if="activeLinks.length > 0"
          :key="activeTab + '-list'"
          :links="activeLinks"
          :status="activeTab"
        />
        <div v-else :key="activeTab + '-empty'" :class="emptyClass">{{ emptyMessage }}</div>
      </Transition>
    </div>
  </div>
</template>

<style scoped>
.repo-link-status {
  margin-top: 12px;
}

.repo-tabs {
  display: flex;
  gap: 0;
  border-bottom: 1px solid #d0d7de;
}

.repo-tab {
  background: transparent;
  border: none;
  border-bottom: 2px solid transparent;
  padding: 8px 12px;
  font-size: 14px;
  font-family: inherit;
  color: #57606a;
  cursor: pointer;
  transition: color 0.15s ease, border-color 0.15s ease;
  margin-bottom: -1px;
  white-space: nowrap;
}

.repo-tab:hover {
  color: #24292f;
}

.repo-tab.active {
  color: #24292f;
  border-bottom-color: #fd8c73;
  font-weight: 600;
}

.repo-tab-content {
  margin-top: 12px;
}

.tab-empty-state {
  padding: 16px;
  text-align: center;
  color: #57606a;
  font-size: 14px;
  background: #f6f8fa;
  border-radius: 6px;
  border: 1px solid #d0d7de;
}

.tab-empty-state.state-good {
  color: #1a7f37;
  background: #dafbe1;
  border-color: #abefc6;
}

.tab-empty-state.state-warning {
  color: #9a6700;
  background: #fff8c5;
  border-color: #f2d866;
}

.tab-error-state {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px 16px;
  background: #ffebeb;
  border: 1px solid #ffdede;
  border-radius: 6px;
  color: #cf222e;
  font-size: 14px;
}

.error-icon {
  font-size: 16px;
}

.retry-btn {
  margin-left: auto;
  padding: 4px 12px;
  border: 1px solid #cf222e;
  border-radius: 6px;
  background: #ffffff;
  color: #cf222e;
  font-size: 13px;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.15s ease;
  font-family: inherit;
  white-space: nowrap;
}

.retry-btn:hover {
  background: #ffebeb;
}
</style>
