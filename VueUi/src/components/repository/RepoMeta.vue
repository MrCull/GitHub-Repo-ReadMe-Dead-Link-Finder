<script setup lang="ts">
import type { Repository } from '@/types/github'
import { getLanguageColor } from '@/utils/languageColors'
import { formatRelativeTime } from '@/utils/formatters'
import { computed } from 'vue'

const props = defineProps<{
  repo: Repository
  isChecking: boolean
}>()

const updatedText = computed(() => {
  if (!props.repo.updatedAt) return ''
  return 'Updated ' + formatRelativeTime(new Date(props.repo.updatedAt))
})

const langColor = computed(() => getLanguageColor(props.repo.language))
</script>

<template>
  <div class="repo-meta">
    <span v-if="repo.language" class="repo-language">
      <span class="language-dot" :style="{ backgroundColor: langColor }"></span>
      {{ repo.language }}
    </span>
    <span class="repo-license">
      <svg class="icon" height="14" width="14" viewBox="0 0 16 16" aria-hidden="true">
        <path fill="currentColor" d="M8.75.75V2h.985c.304 0 .603.08.867.231l1.29.736c.038.022.08.033.124.033h2.234a.75.75 0 0 1 0 1.5h-.427l2.111 4.692a.75.75 0 0 1-.154.838l-.53-.53.529.531-.001.002-.002.002-.006.006-.006.005-.01.01-.045.04c-.21.176-.441.327-.686.45C14.556 10.78 13.88 11 13 11a4.498 4.498 0 0 1-2.023-.454 3.544 3.544 0 0 1-.686-.45l-.045-.04-.016-.015-.006-.006-.004-.004v-.001a.75.75 0 0 1-.154-.838L12.178 4.5h-.162c-.305 0-.604-.079-.868-.231l-1.29-.736a.245.245 0 0 0-.124-.033H8.75V13h2.5a.75.75 0 0 1 0 1.5h-6.5a.75.75 0 0 1 0-1.5h2.5V3.5h-.984a.245.245 0 0 0-.124.033l-1.289.737c-.265.15-.564.23-.869.23h-.162l2.112 4.692a.75.75 0 0 1-.154.838l-.53-.53.529.531-.001.002-.002.002-.006.006-.016.015-.045.04c-.21.176-.441.327-.686.45C4.556 10.78 3.88 11 3 11a4.498 4.498 0 0 1-2.023-.454 3.544 3.544 0 0 1-.686-.45l-.045-.04-.016-.015-.006-.006-.004-.004v-.001a.75.75 0 0 1-.154-.838L2.178 4.5H1.75a.75.75 0 0 1 0-1.5h2.234a.249.249 0 0 0 .125-.033l1.288-.737c.265-.15.564-.23.869-.23h.984V.75a.75.75 0 0 1 1.5 0Zm2.945 8.477c.285.135.718.273 1.305.273s1.02-.138 1.305-.273L13 6.327Zm-10 0c.285.135.718.273 1.305.273s1.02-.138 1.305-.273L3 6.327Z" />
      </svg>
      {{ repo.license || 'Other' }}
    </span>
    <span class="repo-metric">
      <svg class="icon" height="14" width="14" viewBox="0 0 16 16" aria-hidden="true">
        <path fill="currentColor" d="M8 .25a.75.75 0 0 1 .673.418l1.882 3.815 4.21.612a.75.75 0 0 1 .416 1.279l-3.046 2.97.719 4.192a.751.751 0 0 1-1.088.791L8 12.347l-3.766 1.98a.75.75 0 0 1-1.088-.79l.72-4.194L.818 6.374a.75.75 0 0 1 .416-1.28l4.21-.611L7.327.668A.75.75 0 0 1 8 .25Zm0 2.445L6.615 5.5a.75.75 0 0 1-.564.41l-3.097.45 2.24 2.184a.75.75 0 0 1 .216.664l-.528 3.084 2.769-1.456a.75.75 0 0 1 .698 0l2.77 1.456-.53-3.084a.75.75 0 0 1 .216-.664l2.24-2.183-3.096-.45a.75.75 0 0 1-.564-.41L8 2.694Z" />
      </svg>
      {{ repo.stars ?? 0 }}
    </span>
    <span class="repo-metric">
      <svg class="icon" height="14" width="14" viewBox="0 0 16 16" aria-hidden="true">
        <path fill="currentColor" d="M5 5.372v.878c0 .414.336.75.75.75h4.5a.75.75 0 0 0 .75-.75v-.878a2.25 2.25 0 1 1 1.5 0v.878a2.25 2.25 0 0 1-2.25 2.25h-1.5v2.128a2.251 2.251 0 1 1-1.5 0V8.5h-1.5A2.25 2.25 0 0 1 3.5 6.25v-.878a2.25 2.25 0 1 1 1.5 0ZM5 3.25a.75.75 0 1 0-1.5 0 .75.75 0 0 0 1.5 0Zm6.75.75a.75.75 0 1 0 0-1.5.75.75 0 0 0 0 1.5Zm-3 8.75a.75.75 0 1 0-1.5 0 .75.75 0 0 0 1.5 0Z" />
      </svg>
      {{ repo.forks ?? 0 }}
    </span>
  </div>
  <div class="repo-updated">
    {{ updatedText }}
    <span v-if="isChecking" class="repo-checking-badge">
      <span class="mini-spinner"></span> Checking links...
    </span>
  </div>
</template>

<style scoped>
.repo-meta {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 12px;
  font-size: 12px;
  color: #57606a;
  margin-top: 8px;
}

.repo-meta > span:not(:last-child)::after {
  content: "•";
  margin-left: 12px;
  color: #8c959f;
}

.repo-language {
  display: inline-flex;
  align-items: center;
  gap: 4px;
}

.language-dot {
  width: 10px;
  height: 10px;
  border-radius: 50%;
}

.repo-license,
.repo-metric {
  display: inline-flex;
  align-items: center;
  gap: 4px;
}

.icon {
  width: 14px;
  height: 14px;
  fill: #57606a;
}

.repo-updated {
  color: #57606a;
  font-size: 12px;
  margin-top: 8px;
  display: block;
}

.repo-checking-badge {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  font-size: 12px;
  color: #0f6fff;
  margin-left: 8px;
  background: #eef6ff;
  border: 1px solid #d0e5ff;
  padding: 2px 8px;
  border-radius: 999px;
  animation: pulse-badge 2s ease-in-out infinite;
}

@keyframes pulse-badge {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.7; }
}
</style>
