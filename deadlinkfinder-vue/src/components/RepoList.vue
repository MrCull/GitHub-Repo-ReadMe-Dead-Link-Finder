<template>
    <div class="repo-list">
      <div v-for="repo in repos" :key="repo.repoUri" class="card mb-3">
        <div class="card-header">
          <a :href="repo.repoUri" target="_blank">{{ repo.repoUri }}</a>
        </div>
        <div class="card-body">
          <LinkChecker :repo-uri="repo.repoUri" :branch="repo.branch" />
        </div>
      </div>
    </div>
  </template>
  
  <script>
  import LinkChecker from './LinkChecker.vue'
  
  export default {
    name: 'RepoList',
    components: {
      LinkChecker
    },
    props: {
      repos: {
        type: Array,
        required: true
      }
    }
  }
  </script>