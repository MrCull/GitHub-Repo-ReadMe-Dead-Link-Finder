<template>
  <div id="app" class="container">
    <h1 class="my-4">GitHub "readme" bad link checker</h1>
    <RepoSearch @search="handleSearch" />
    <RepoList :repos="repos" />
  </div>
</template>

<script>
import RepoSearch from './components/RepoSearch.vue'
import RepoList from './components/RepoList.vue'

export default {
  name: 'App',
  components: {
    RepoSearch,
    RepoList
  },
  data() {
    return {
      repos: []
    }
  },
  methods: {
    handleSearch(searchForm) {
      // Here you would typically make an API call to your backend
      // For now, we'll just simulate the response
      if (searchForm.singleRepoUri) {
        this.repos = [{
          repoUri: searchForm.singleRepoUri,
          branch: 'main'
        }]
      } else {
        // Simulate fetching multiple repos
        this.repos = Array(searchForm.numberOfReposToSearchFor).fill().map((_, i) => ({
          repoUri: `https://github.com/example/repo-${i + 1}`,
          branch: 'main'
        }))
      }
    }
  }
}
</script>