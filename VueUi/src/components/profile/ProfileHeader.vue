<script setup lang="ts">
import { ref } from 'vue'

const props = defineProps<{
  userOrOrg: string
}>()

const DEFAULT_AVATAR = 'https://avatars.githubusercontent.com/u/0?v=4'
const avatarError = ref(false)
const avatarLoaded = ref(false)

function onAvatarError() {
  avatarError.value = true
  avatarLoaded.value = true
}

function onAvatarLoad() {
  avatarLoaded.value = true
}

function avatarUrl(): string {
  if (avatarError.value || !props.userOrOrg) return DEFAULT_AVATAR
  return `https://avatars.githubusercontent.com/${props.userOrOrg}`
}
</script>

<template>
  <section class="gh-profile-header">
    <div class="gh-profile-left">
      <div class="gh-avatar" :class="{ 'avatar-loaded': avatarLoaded }">
        <div v-if="!avatarLoaded" class="avatar-skeleton skeleton"></div>
        <img
          :src="avatarUrl()"
          :alt="`${userOrOrg} avatar`"
          @error="onAvatarError"
          @load="onAvatarLoad"
          :class="{ 'img-loaded': avatarLoaded }"
        />
      </div>
      <div class="gh-profile-text">
        <div class="gh-real-name">{{ userOrOrg || 'GitHub User' }}</div>
        <div class="gh-username">@{{ userOrOrg || 'github' }}</div>
      </div>
    </div>
  </section>
</template>

<style scoped>
.gh-profile-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  margin-bottom: 0.75rem;
}

.gh-profile-left {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.gh-avatar {
  width: 72px;
  height: 72px;
  border-radius: 50%;
  overflow: hidden;
  border: 1px solid var(--gh-border);
  background: var(--gh-surface);
}

.gh-avatar img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  opacity: 0;
  transition: opacity 0.3s ease;
}

.gh-avatar img.img-loaded {
  opacity: 1;
}

.avatar-skeleton {
  position: absolute;
  inset: 0;
  border-radius: 50%;
}

.gh-avatar {
  position: relative;
}

.gh-real-name {
  font-size: 1.25rem;
  font-weight: 600;
}

.gh-username {
  color: var(--gh-muted);
  font-size: 1rem;
}

@media (max-width: 768px) {
  .gh-profile-header {
    flex-direction: column;
    align-items: flex-start;
  }
}
</style>
