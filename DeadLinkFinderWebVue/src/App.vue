<script setup lang="ts">
import GitHubReadMeChecker from './components/GitHubReadMeChecker.vue'
import './assets/styles/global.css'
</script>

<template>
  <div class="app">
    <GitHubReadMeChecker />
  </div>
</template>

<style>
.app {
  min-height: 100vh;
  padding: 20px;
  background-color: var(--card-bg);
}
</style>
